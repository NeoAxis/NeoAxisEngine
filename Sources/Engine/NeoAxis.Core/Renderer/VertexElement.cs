// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//same as SharpBgfx.VertexAttributeUsage
	/// <summary>
	/// Vertex element semantics, used to identify the meaning of vertex buffer contents.
	/// </summary>
	public enum VertexElementSemantic
	{
		/// <summary>
		/// Position data.
		/// </summary>
		Position,

		/// <summary>
		/// Normals.
		/// </summary>
		Normal,

		/// <summary>
		/// Tangents.
		/// </summary>
		Tangent,

		/// <summary>
		/// Bitangents.
		/// </summary>
		Bitangent,

		/// <summary>
		/// First color channel.
		/// </summary>
		Color0,

		/// <summary>
		/// Second color channel.
		/// </summary>
		Color1,

		/// <summary>
		/// Third color channel.
		/// </summary>
		Color2,

		/// <summary>
		/// Fourth color channel.
		/// </summary>
		Color3,

		/// <summary>
		/// Indices.
		/// </summary>
		BlendIndices,//Indices

		/// <summary>
		/// Animation weights.
		/// </summary>
		BlendWeights,//Weight

		/// <summary>
		/// First texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate0,

		/// <summary>
		/// Second texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate1,

		/// <summary>
		/// Third texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate2,

		/// <summary>
		/// Fourth texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate3,

		/// <summary>
		/// Fifth texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate4,

		/// <summary>
		/// Sixth texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate5,

		/// <summary>
		/// Seventh texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate6,

		/// <summary>
		/// Eighth texture coordinate channel (arbitrary data).
		/// </summary>
		TextureCoordinate7,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Vertex element type, used to identify the base types of the vertex contents.</summary>
	public enum VertexElementType
	{
		/// <summary>Float 1.</summary>
		Float1 = 0,//Ogre::VET_FLOAT1,
		/// <summary>Float 2.</summary>
		Float2 = 1,//Ogre::VET_FLOAT2,
		/// <summary>Float 3.</summary>
		Float3 = 2,//Ogre::VET_FLOAT3,
		/// <summary>Float 4.</summary>
		Float4 = 3,//Ogre::VET_FLOAT4,
		/// <summary>alias to more specific color type - use the current rendersystem's color packing.</summary>
		//Color = 4,//Ogre::VET_COLOUR,
		//		  /// <summary>Short 1.</summary>
		Short1 = 5,//Ogre::VET_SHORT1,
		/// <summary>Short 2.</summary>
		Short2 = 6,//Ogre::VET_SHORT2,
		/// <summary>Short 3.</summary>
		Short3 = 7,//Ogre::VET_SHORT3,
		/// <summary>Short 4.</summary>
		Short4 = 8,//Ogre::VET_SHORT4,
		/// <summary>Byte 4.</summary>
		UByte4 = 9,//Ogre::VET_UBYTE4,

		//!!!!
		/// <summary>D3D style compact color.</summary>
		ColorARGB = 10,//Ogre::VET_COLOUR_ARGB,
		/// <summary>GL style compact color.</summary>
		ColorABGR = 11,//Ogre::VET_COLOUR_ABGR

		Integer1 = 12,
		Integer2 = 13,
		Integer3 = 14,
		Integer4 = 15,

		Half1 = 16,
		Half2 = 17,
		Half3 = 18,
		Half4 = 19,

		//Double1 = 12,
		//Double2 = 13,
		//Double3 = 14,
		//Double4 = 15,
		//UShort1 = 16,
		//UShort2 = 17,
		//UShort3 = 18,
		//UShort4 = 19,
		//Int1 = 20,
		//Int2 = 21,
		//Int3 = 22,
		//Int4 = 23,
		//UInt1 = 24,
		//UInt2 = 25,
		//UInt3 = 26,
		//UInt4 = 27
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This structure declares the usage of a single vertex buffer as a component of a complete vertex declaration.
	/// </summary>
	/// <remarks>
	/// Several vertex buffers can be used to supply the input geometry for a
	/// rendering operation, and in each case a vertex buffer can be used in
	/// different ways for different operations; the buffer itself does not
	/// define the semantics (position, normal etc), the VertexElement
	/// class does.
	/// </remarks>
	public struct VertexElement
	{
		int source;
		int offset;
		VertexElementType type;
		VertexElementSemantic semantic;
		//int index;

		public VertexElement( int source, int offset, VertexElementType type, VertexElementSemantic semantic )//, int index = 0 )
		{
			this.source = source;
			this.offset = offset;
			this.type = type;
			this.semantic = semantic;
			//this.index = index;
		}

		/// <summary>Gets the vertex buffer index from where this element draws it's values.</summary>
		[Serialize]
		[DefaultValue( 0 )]
		public int Source
		{
			get { return source; }
			set { source = value; }
		}

		/// <summary>Gets the offset into the buffer where this element starts.</summary>
		[Serialize]
		public int Offset
		{
			get { return offset; }
			set { offset = value; }
		}

		/// <summary>Gets the data format of this element.</summary>
		[Serialize]
		public VertexElementType Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>Gets the meaning of this element.</summary>
		[Serialize]
		public VertexElementSemantic Semantic
		{
			get { return semantic; }
			set { semantic = value; }
		}

		///// <summary>Gets the index of this element, only applicable for repeating elements.</summary>
		//[Serialize]
		//[DefaultValue( 0 )]
		//public int Index
		//{
		//	get { return index; }
		//	set { index = value; }
		//}

		/// <summary>Gets the size of this element in bytes.</summary>
		/// <returns>The size of this element in bytes.</returns>
		public int GetSizeInBytes()
		{
			return GetSizeInBytes( type );
		}

		/// <summary>Utility method for helping to calculate offsets.</summary>
		/// <param name="type">The element type.</param>
		/// <returns>The type size in bytes.</returns>
		public static int GetSizeInBytes( VertexElementType type )
		{
			switch( type )
			{
			//case VertexElementType.Color:
			case VertexElementType.ColorABGR:
			case VertexElementType.ColorARGB:
				return 4;
			case VertexElementType.Float1:
				return sizeof( float );
			case VertexElementType.Float2:
				return sizeof( float ) * 2;
			case VertexElementType.Float3:
				return sizeof( float ) * 3;
			case VertexElementType.Float4:
				return sizeof( float ) * 4;
			case VertexElementType.Short1:
				return sizeof( short );
			case VertexElementType.Short2:
				return sizeof( short ) * 2;
			case VertexElementType.Short3:
				return sizeof( short ) * 3;
			case VertexElementType.Short4:
				return sizeof( short ) * 4;
			case VertexElementType.UByte4:
				return 4;
			case VertexElementType.Integer1:
				return sizeof( int );
			case VertexElementType.Integer2:
				return sizeof( int ) * 2;
			case VertexElementType.Integer3:
				return sizeof( int ) * 3;
			case VertexElementType.Integer4:
				return sizeof( int ) * 4;
			case VertexElementType.Half1:
				return 2;
			case VertexElementType.Half2:
				return 2 * 2;
			case VertexElementType.Half3:
				return 2 * 3;
			case VertexElementType.Half4:
				return 2 * 4;

				//case VertexElementType.Double1:
				//	return sizeof( double );
				//case VertexElementType.Double2:
				//	return sizeof( double ) * 2;
				//case VertexElementType.Double3:
				//	return sizeof( double ) * 3;
				//case VertexElementType.Double4:
				//	return sizeof( double ) * 4;
				//case VertexElementType.UShort1:
				//	return sizeof( ushort );
				//case VertexElementType.UShort2:
				//	return sizeof( ushort ) * 2;
				//case VertexElementType.UShort3:
				//	return sizeof( ushort ) * 3;
				//case VertexElementType.UShort4:
				//	return sizeof( ushort ) * 4;
				//case VertexElementType.UInt1:
				//	return sizeof( uint );
				//case VertexElementType.UInt2:
				//	return sizeof( uint ) * 2;
				//case VertexElementType.UInt3:
				//	return sizeof( uint ) * 3;
				//case VertexElementType.UInt4:
				//	return sizeof( uint ) * 4;
			}
			return 0;

			//unsafe
			//{
			//	return OgreVertexElement.getTypeSize( type );
			//}
		}

		///// <summary>Utility method which returns the count of values in a given type.</summary>
		///// <param name="type">The element type.</param>
		///// <returns>The types count.</returns>
		//public static int GetTypeCount( VertexElementTypes type )
		//{
		//          switch( type )
		//          {
		//          case VertexElementTypes.Color:
		//          case VET_COLOUR_ABGR:
		//          case VET_COLOUR_ARGB:
		//              return 1;
		//          case VET_FLOAT1:
		//              return 1;
		//          case VET_FLOAT2:
		//              return 2;
		//          case VET_FLOAT3:
		//              return 3;
		//          case VET_FLOAT4:
		//              return 4;
		//          case VET_SHORT1:
		//              return 1;
		//          case VET_SHORT2:
		//              return 2;
		//          case VET_SHORT3:
		//              return 3;
		//          case VET_SHORT4:
		//              return 4;
		//          case VET_UBYTE4:
		//              return 4;
		//          }
		//          //OGRE_EXCEPT(Exception::ERR_INVALIDPARAMS, "Invalid type", 
		//          //	"VertexElement::getTypeCount");
		//          return 0;
		//}

		///// <summary>
		///// Simple converter function which will turn a single-value type into a
		///// multi-value type based on a parameter.
		///// </summary>
		///// <param name="baseType">The base type.</param>
		///// <param name="count">The count.</param>
		///// <returns>The element type.</returns>
		//public static VertexElementTypes MultiplyTypeCount( VertexElementTypes baseType, int count )
		//{
		//   unsafe
		//   {
		//      return OgreVertexElement.multiplyTypeCount( baseType, count );
		//   }
		//}

		///// <summary>
		///// Simple converter function which will a type into it's single-value
		///// equivalent - makes switches on type easier.
		///// </summary>
		///// <param name="multiType">The multi type.</param>
		///// <returns>The element type.</returns>
		//public static VertexElementTypes GetBaseType( VertexElementTypes multiType )
		//{
		//   unsafe
		//   {
		//      return OgreVertexElement.getBaseType( multiType );
		//   }
		//}

		/// <summary>Utility method for converting color to a packed 32-bit color type.</summary>
		/// <param name="color">The source color.</param>
		/// <param name="destinationType">The destination type.</param>
		/// <returns>The packed color type.</returns>
		public static uint ConvertColorValue( ColorValue color, VertexElementType destinationType )
		{
			//fix parameters
			color.Saturate();
			if( destinationType != VertexElementType.ColorARGB && destinationType != VertexElementType.ColorABGR )
			{
				//!!!!Windows? not D3D? RenderSystem.ConvertColorValue
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					destinationType = VertexElementType.ColorARGB;
				else
					destinationType = VertexElementType.ColorABGR;
			}

			if( destinationType == VertexElementType.ColorARGB )
			{
				byte val8;
				uint val32 = 0;
				val8 = (byte)( color.Alpha * 255 );
				val32 = (uint)val8 << 24;
				val8 = (byte)( color.Red * 255 );
				val32 += (uint)val8 << 16;
				val8 = (byte)( color.Green * 255 );
				val32 += (uint)val8 << 8;
				val8 = (byte)( color.Blue * 255 );
				val32 += (uint)val8;
				return val32;
			}
			else //if( destinationType == VertexElementTypes.ColorABGR )
			{
				byte val8;
				uint val32 = 0;
				val8 = (byte)( color.Alpha * 255 );
				val32 = (uint)val8 << 24;
				val8 = (byte)( color.Blue * 255 );
				val32 += (uint)val8 << 16;
				val8 = (byte)( color.Green * 255 );
				val32 += (uint)val8 << 8;
				val8 = (byte)( color.Red * 255 );
				val32 += (uint)val8;
				return val32;
			}

			//return ConvertColorValue( ref color, destinationType );
		}

		///// <summary>
		///// Utility method to get the most appropriate packed color vertex element format.
		///// </summary>
		///// <returns>the most appropriate packed color vertex element format.</returns>
		//public static VertexElementType GetBestColorVertexElementType()
		//{
		//   unsafe
		//   {
		//      return OgreVertexElement.getBestColourVertexElementType();
		//   }
		//}

		public override string ToString()
		{
			return string.Format( "Semantic: {0}; Type: {1}", Semantic, Type );
			//return string.Format( "Semantic: {0}; Type: {1}; Index: {2}", Semantic, Type, Index );
		}

		public override bool Equals( object obj )
		{
			return ( obj is VertexElement && this == (VertexElement)obj );
		}

		public override int GetHashCode()
		{
			return Source.GetHashCode() ^ Offset.GetHashCode() ^ Type.GetHashCode() ^ Semantic.GetHashCode();
			//return Source.GetHashCode() ^ Offset.GetHashCode() ^ Type.GetHashCode() ^ Semantic.GetHashCode() ^ Index.GetHashCode();
		}

		public static bool operator ==( VertexElement v1, VertexElement v2 )
		{
			return
				v1.Source == v2.Source &&
				v1.Offset == v2.Offset &&
				v1.Type == v2.Type &&
				v1.Semantic == v2.Semantic;
			// &&
			//v1.Index == v2.Index;
		}

		public static bool operator !=( VertexElement v1, VertexElement v2 )
		{
			return
				v1.Source != v2.Source ||
				v1.Offset != v2.Offset ||
				v1.Type != v2.Type ||
				v1.Semantic != v2.Semantic;
			// ||
			//v1.Index != v2.Index;
		}

		internal void GetBfgx( out Internal.SharpBgfx.VertexAttributeUsage attribute, out int count, out Internal.SharpBgfx.VertexAttributeType type, out bool asInt )
		{
			attribute = (Internal.SharpBgfx.VertexAttributeUsage)semantic;

			switch( this.type )
			{
			case VertexElementType.Float1:
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 1;
				asInt = false;
				break;
			case VertexElementType.Float2:
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 2;
				asInt = false;
				break;
			case VertexElementType.Float3:
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 3;
				asInt = false;
				break;
			case VertexElementType.Float4:
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 4;
				asInt = false;
				break;

			case VertexElementType.Short1:
				type = Internal.SharpBgfx.VertexAttributeType.Int16;
				count = 1;
				asInt = true;
				break;
			case VertexElementType.Short2:
				type = Internal.SharpBgfx.VertexAttributeType.Int16;
				count = 2;
				asInt = true;
				break;
			case VertexElementType.Short3:
				type = Internal.SharpBgfx.VertexAttributeType.Int16;
				count = 3;
				asInt = true;
				break;
			case VertexElementType.Short4:
				type = Internal.SharpBgfx.VertexAttributeType.Int16;
				count = 4;
				asInt = true;
				break;

			case VertexElementType.UByte4:
			case VertexElementType.ColorABGR:
			case VertexElementType.ColorARGB:
				type = Internal.SharpBgfx.VertexAttributeType.UInt8;
				count = 4;
				asInt = true;
				break;

			case VertexElementType.Integer1:
				//!!!!works?
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 1;
				asInt = true;
				break;
			case VertexElementType.Integer2:
				//!!!!works?
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 2;
				asInt = true;
				break;
			case VertexElementType.Integer3:
				//!!!!works?
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 3;
				asInt = true;
				break;
			case VertexElementType.Integer4:
				//!!!!works?
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 4;
				asInt = true;
				break;

			case VertexElementType.Half1:
				type = Internal.SharpBgfx.VertexAttributeType.Half;
				count = 1;
				asInt = false;
				break;
			case VertexElementType.Half2:
				type = Internal.SharpBgfx.VertexAttributeType.Half;
				count = 2;
				asInt = false;
				break;
			case VertexElementType.Half3:
				type = Internal.SharpBgfx.VertexAttributeType.Half;
				count = 3;
				asInt = false;
				break;
			case VertexElementType.Half4:
				type = Internal.SharpBgfx.VertexAttributeType.Half;
				count = 4;
				asInt = false;
				break;


			default:
				Log.Fatal( "VertexElement: GetBfgx: Unknown type." );
				type = Internal.SharpBgfx.VertexAttributeType.Float;
				count = 1;
				asInt = false;
				break;
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Extensions for working with an array of vertex elements.
	/// </summary>
	public static class VertexElements
	{
		//!!!!IList<> ?

		public static bool GetElementBySemantic( this VertexElement[] elements, VertexElementSemantic semantic, out VertexElement element )
		{
			//!!!!slowly

			for( int n = 0; n < elements.Length; n++ )
			{
				if( elements[ n ].Semantic == semantic )
				{
					element = elements[ n ];
					return true;
				}
			}

			element = new VertexElement();
			return false;
		}

		public static void GetInfo( this VertexElement[] elements, out int vertexSize, out bool holes )
		{
			int r = 0;
			foreach( VertexElement e in elements )
			{
				int r2 = e.Offset + e.GetSizeInBytes();
				if( r2 > r )
					r = r2;
			}
			vertexSize = r;

			bool[] filled = new bool[ vertexSize ];
			foreach( VertexElement e in elements )
			{
				int size = e.GetSizeInBytes();
				for( int n = 0; n < size; n++ )
					filled[ n + e.Offset ] = true;
			}

			holes = false;
			foreach( bool v in filled )
			{
				if( !v )
				{
					holes = true;
					break;
				}
			}
		}

		public static Internal.SharpBgfx.VertexLayout CreateVertexDeclaration( this VertexElement[] elements, int forSource )
		{
			GetInfo( elements, out var vertexSize, out var holes );

			//!!!!
			if( holes )
				Log.Fatal( "VertexElement: CreateVertexDeclaration: holes. impl." );

			//!!!!
			if( forSource != 0 )
				Log.Fatal( "VertexElement: CreateVertexDeclaration: forSource != 0. impl." );
			//if( element.Source == forSource )
			//{
			//}


			var result = new Internal.SharpBgfx.VertexLayout();
			result.Begin();

			var sorted = (VertexElement[])elements.Clone();
			CollectionUtility.SelectionSort( sorted, delegate ( VertexElement element1, VertexElement element2 )
			{
				if( element1.Offset < element2.Offset )
					return -1;
				if( element1.Offset > element2.Offset )
					return 1;
				return 0;
			} );

			foreach( var element in sorted )
			{
				element.GetBfgx( out var attribute, out var count, out var type, out var asInt );
				result.Add( attribute, count, type, false, asInt );
			}

			result.End();

			if( vertexSize != result.Stride )
				Log.Fatal( "VertexElement: CreateVertexDeclaration: vertexSize != result.Stride." );

			return result;
		}

		public static bool Equals( this VertexElement[] elements1, VertexElement[] elements2 )
		{
			if( elements1.Length != elements2.Length )
				return false;
			for( int n = 0; n < elements1.Length; n++ )
				if( elements1[ n ] != elements2[ n ] )
					return false;
			return true;
		}
	}
}
