// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// The class allows to store the data information in the hierarchical form.
	/// Supports creation of children and attributes.
	/// </summary>
	public class DataBlock
	{
		DataBlock parent;
		string name;
		List<DataBlock> children = new List<DataBlock>();
		List<Attribute> attributes = new List<Attribute>();

		//

		/// <summary>
		/// Defines <see cref="NeoAxis.DataBlock"/> attribute.
		/// </summary>
		public sealed class Attribute
		{
			internal string name;
			internal ArraySegment<byte> value;

			internal Attribute() { }

			/// <summary>
			/// Gets the attribute name.
			/// </summary>
			public string Name
			{
				get { return name; }
			}

			public ArraySegment<byte> Value
			{
				get { return value; }
			}

			/// <summary>
			/// Returns a string that represents the current attribute.
			/// </summary>
			/// <returns>A string that represents the current attribute.</returns>
			public override string ToString()
			{
				return string.Format( "Name: \"{0}\", Length \"{1}\"", name, Value.Count );
			}
		}

		/// <summary>
		/// It is applied only to creation root blocks. Not for creation of children.
		/// </summary>
		public DataBlock() { }

		/// <summary>
		/// Gets the parent block.
		/// </summary>
		public DataBlock Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Gets or set block name.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets the children collection. <b>Don't modify.</b>
		/// </summary>
		public List<DataBlock> Children
		{
			get { return children; }
		}

		/// <summary>
		/// Finds child block by name.
		/// </summary>
		/// <param name="name">The block name.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been exists; otherwise, <b>null</b>.</returns>
		public DataBlock FindChild( string name )
		{
			for( int n = 0; n < children.Count; n++ )
			{
				DataBlock child = children[ n ];
				if( child.Name == name )
					return child;
			}
			return null;
		}

		/// <summary>
		/// Creates the child block.
		/// </summary>
		/// <param name="name">The block name.</param>
		/// <returns>The child block.</returns>
		/// <remarks>
		/// Names of blocks can repeat.
		/// </remarks>
		public DataBlock AddChild( string name )
		{
			if( string.IsNullOrEmpty( name ) )
				Log.Fatal( "DataBlock: AddChild: \"name\" is null or empty." );

			DataBlock child = new DataBlock();
			child.parent = this;
			child.name = name;
			children.Add( child );
			return child;
		}

		/// <summary>
		/// Deletes child block.
		/// </summary>
		/// <param name="child">The child block.</param>
		public void DeleteChild( DataBlock child )
		{
			children.Remove( child );
			child.parent = null;
			child.name = "";
			child.children = null;
			child.attributes = null;
		}

		/// <summary>
		/// Returns the attribute by name if exists.
		/// </summary>
		public ArraySegment<byte> GetAttribute( string name )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				Attribute attribute = attributes[ n ];
				if( attribute.Name == name )
					return attribute.Value;
			}
			return new ArraySegment<byte>();
		}

		/// <summary>
		/// Gets the attributes collection. <b>Don't modify.</b>
		/// </summary>
		public List<Attribute> Attributes
		{
			get { return attributes; }
		}

		/// <summary>
		/// Checks existence of attribute.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <returns><b>true</b> if the block exists; otherwise, <b>false</b>.</returns>
		public bool AttributeExists( string name )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				Attribute attribute = attributes[ n ];
				if( attribute.Name == name )
					return true;
			}
			return false;
		}

		/// <summary>
		/// Sets an attribute.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="cloneArray"></param>
		/// <param name="replaceWithSameName"></param>
		void SetAttribute( string name, ArraySegment<byte> value, bool cloneArray, bool replaceWithSameName )
		{
			if( string.IsNullOrEmpty( name ) )
				Log.Fatal( "DataBlock: SetAttribute: \"name\" is null or empty." );
			if( value.Array == null )
				Log.Fatal( "DataBlock: SetAttribute: \"value.Array\" is null." );

			ArraySegment<byte> newValue;
			if( cloneArray )
			{
				byte[] data = new byte[ value.Count ];
				Array.Copy( value.Array, value.Offset, data, 0, value.Count );
				newValue = new ArraySegment<byte>( data );
			}
			else
			{
				newValue = value;
			}

			if( replaceWithSameName )
			{
				for( int n = 0; n < attributes.Count; n++ )
				{
					Attribute attribute = attributes[ n ];
					if( attribute.Name == name )
					{
						attribute.value = newValue;
						return;
					}
				}
			}
			Attribute a = new Attribute();
			a.name = name;
			a.value = newValue;
			attributes.Add( a );
		}

		/// <summary>
		/// Deletes attribute if he exists.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		public void DeleteAttribute( string name )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				if( name == attributes[ n ].name )
				{
					Attribute attribute = attributes[ n ];
					attribute.name = "";
					attribute.value = new ArraySegment<byte>();
					attributes.RemoveAt( n );
					return;
				}
			}
		}

		/// <summary>
		/// Deletes all attributes.
		/// </summary>
		public void DeleteAllAttributes()
		{
			attributes.Clear();
		}

		///////////////////////////////////////////

		class DataWriter
		{
			byte[] data = new byte[ 4 ];
			int dataLength;

			//

			public DataWriter()
			{
			}

			public byte[] Data
			{
				get { return data; }
			}

			public int DataLength
			{
				get { return dataLength; }
			}

			void ExpandBuffer( int demandLength )
			{
				if( data.Length < demandLength )
				{
					int newLength = data.Length;
					while( newLength < demandLength )
						newLength *= 2;
					Array.Resize<byte>( ref data, newLength );
				}
			}

			public void Write( byte[] source, int byteOffset, int byteLength )
			{
				int newLength = dataLength + byteLength;
				ExpandBuffer( newLength );
				Array.Copy( source, byteOffset, data, dataLength, byteLength );
				dataLength = newLength;
			}

			public void Write( byte[] source )
			{
				Write( source, 0, source.Length );
			}

			public void Write( byte source )
			{
				int newLength = dataLength + 1;
				ExpandBuffer( newLength );
				data[ dataLength ] = source;
				dataLength = newLength;
			}

			public int WriteVariableUInt32( uint source )
			{
				int retval = 1;
				uint num1 = (uint)source;
				while( num1 >= 0x80 )
				{
					Write( (byte)( num1 | 0x80 ) );
					num1 = num1 >> 7;
					retval++;
				}
				Write( (byte)num1 );
				return retval;
			}

			public void Write( string source )
			{
				if( string.IsNullOrEmpty( source ) )
				{
					WriteVariableUInt32( 0 );
					return;
				}

				byte[] bytes = Encoding.UTF8.GetBytes( source );
				WriteVariableUInt32( (uint)bytes.Length );
				Write( bytes );
			}
		}

		///////////////////////////////////////////

		class DataReader
		{
			byte[] data;
			int dataPosition;
			bool overflow;

			//

			public DataReader( byte[] data )
			{
				this.data = data;
			}

			public byte[] Data
			{
				get { return data; }
			}

			public int DataPosition
			{
				get { return dataPosition; }
			}

			public bool Overflow
			{
				get { return overflow; }
			}

			public void SkipBytes( int byteLength )
			{
				int newPosition = dataPosition + byteLength;
				if( overflow || newPosition > data.Length )
				{
					overflow = true;
					return;
				}
				dataPosition = newPosition;
			}

			public byte ReadByte()
			{
				int newPosition = dataPosition + 1;
				if( overflow || newPosition > data.Length )
				{
					overflow = true;
					return 0;
				}
				byte value = data[ dataPosition ];
				dataPosition = newPosition;
				return value;
			}

			public uint ReadVariableUInt32()
			{
				if( overflow )
					return 0;

				int num1 = 0;
				int num2 = 0;
				while( true )
				{
					if( num2 == 0x23 )
					{
						overflow = true;
						return 0;
					}

					byte num3 = ReadByte();
					if( overflow )
						return 0;

					num1 |= ( num3 & 0x7f ) << ( num2 & 0x1f );
					num2 += 7;
					if( ( num3 & 0x80 ) == 0 )
						return (uint)num1;
				}
			}

			public string ReadString()
			{
				int byteLength = (int)ReadVariableUInt32();

				if( byteLength == 0 )
					return "";

				if( overflow || dataPosition + byteLength > data.Length )
				{
					overflow = true;
					return "";
				}

				string result = System.Text.Encoding.UTF8.GetString( data, dataPosition, byteLength );
				dataPosition += byteLength;
				return result;
			}
		}

		///////////////////////////////////////////

		void DumpToDataWriter( DataWriter writer )
		{
			writer.WriteVariableUInt32( (uint)children.Count );
			foreach( DataBlock child in children )
			{
				writer.Write( child.Name );
				child.DumpToDataWriter( writer );
			}

			writer.WriteVariableUInt32( (uint)attributes.Count );
			foreach( Attribute attribute in attributes )
			{
				writer.Write( attribute.Name );
				writer.WriteVariableUInt32( (uint)attribute.Value.Count );
				writer.Write( attribute.Value.Array, attribute.Value.Offset, attribute.Value.Count );
			}
		}

		/// <summary>
		/// Returns a buffer containing all data about the block and his children.
		/// </summary>
		/// <returns>A string containing all data about the block and his children.</returns>
		/// <remarks>
		/// This method is applied at preservation of data of the block in a file.
		/// </remarks>
		public ArraySegment<byte> DumpToBuffer()
		{
			DataWriter writer = new DataWriter();

			//format version
			writer.WriteVariableUInt32( 0 );

			DumpToDataWriter( writer );

			return new ArraySegment<byte>( writer.Data, 0, writer.DataLength );
		}

		/// <summary>
		/// Returns a string that represents the current text block.
		/// </summary>
		/// <returns>A string that represents the current text block.</returns>
		public override string ToString()
		{
			return string.Format( "Name: \"{0}\"", name );
		}

		bool ParseFromDataReader( DataReader reader )
		{
			int childrenCount = (int)reader.ReadVariableUInt32();
			if( reader.Overflow )
				return false;

			for( int n = 0; n < childrenCount; n++ )
			{
				string childName = reader.ReadString();
				if( reader.Overflow )
					return false;

				DataBlock child = AddChild( childName );
				if( !child.ParseFromDataReader( reader ) )
					return false;
			}

			int attributeCount = (int)reader.ReadVariableUInt32();
			if( reader.Overflow )
				return false;

			for( int n = 0; n < attributeCount; n++ )
			{
				string attributeName = reader.ReadString();
				if( reader.Overflow )
					return false;

				int length = (int)reader.ReadVariableUInt32();
				if( reader.Overflow )
					return false;

				ArraySegment<byte> value = new ArraySegment<byte>( reader.Data, reader.DataPosition,
					length );

				SetAttribute( attributeName, value, false, true );

				reader.SkipBytes( length );
				if( reader.Overflow )
					return false;
			}

			return true;
		}

		/// <summary>
		/// Parses the text with data of the block and his children.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="cloneDataArray"></param>
		/// <param name="error">The information on an error.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been parsed; otherwise, <b>null</b>.</returns>
		/// <remarks>
		/// For convenience of loading of blocks there is auxiliary class <see cref="NeoAxis.DataBlockUtility"/>.
		/// </remarks>
		public static DataBlock Parse( byte[] data, bool cloneDataArray, out string error )
		{
			DataBlock block = new DataBlock();

			DataReader reader = new DataReader( cloneDataArray ? ( (byte[])data.Clone() ) : data );

			//format version
			uint version = reader.ReadVariableUInt32();
			if( reader.Overflow )
			{
				error = "Unexpected end of data.";
				return null;
			}

			if( version != 0 )
			{
				error = string.Format( "Unknown format version \"{0}\".", version );
				return null;
			}

			if( !block.ParseFromDataReader( reader ) )
			{
				error = "Unexpected end of data.";
				return null;
			}

			error = "";
			return block;
		}
	}
}
