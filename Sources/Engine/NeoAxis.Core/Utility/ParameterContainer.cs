// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Internal.SharpBgfx;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Container for parameters of different types. In some cases, used for the preparation of parameters for the GPU.
	/// </summary>
	public class ParameterContainer
	{
		public Dictionary<string, NamedParameter> NamedParameters;
		public OpenList<UniformParameter> UniformParameters;
		public OpenList<ViewportRenderingContext.BindTextureData> TextureParameters;

		byte[] data;
		int dataUsedBytes;
		ulong version;

		//!!!!don't forget about Clear()

		/////////////////////////////////////////

		/// <summary>
		/// Represents parameter data of <see cref="ParameterContainer"/> which specified by name.
		/// </summary>
		public struct NamedParameter
		{
			//public string Name;
			public ParameterType Type;
			public int ElementCount;
			public int ValueDataPosition;// = -1;
			public object ValueReference;

			public int GetTotalSizeInBytes()
			{
				return ParameterTypeUtility.GetElementSizeInBytes( Type ) * ElementCount;
			}

			public ArraySegment<byte> GetValue( ParameterContainer container )
			{
				//if( ValueDataPosition == -1 )
				//	Log.Fatal( "ParameterContainer: NamedParameter: GetValue: No data in byte array." );

				//!!!!check valid array size
				return new ArraySegment<byte>( container.data, ValueDataPosition, GetTotalSizeInBytes() );
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents parameter data of <see cref="ParameterContainer"/> which specified by uniform identifier.
		/// </summary>
		public struct UniformParameter
		{
			public Uniform Uniform;
			public ParameterType Type;
			public int ElementCount;
			public int ValueDataPosition;

			public int GetTotalSizeInBytes()
			{
				return ParameterTypeUtility.GetElementSizeInBytes( Type ) * ElementCount;
			}

			public ArraySegment<byte> GetValue( ParameterContainer container )
			{
				if( ValueDataPosition == -1 )
					Log.Fatal( "ParameterContainer: UniformParameter: GetValue: No data in byte array." );

				//!!!!check valid array size
				return new ArraySegment<byte>( container.data, ValueDataPosition, GetTotalSizeInBytes() );
			}
		}

		/////////////////////////////////////////

		//public class ParameterItem
		//{
		//	internal ParameterContainer container;

		//	internal string name;
		//	internal ParameterType type;
		//	internal int elementCount;

		//	internal int valueDataPosition = -1;
		//	internal object valueReference;

		//	//

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public ParameterType Type
		//	{
		//		get { return type; }
		//		//!!!!set?
		//	}

		//	public int ElementCount
		//	{
		//		get { return elementCount; }
		//		//!!!!set?
		//	}

		//	//!!!!было
		//	//unsafe object MakeObjectFromArrayPointer( byte* p )
		//	//{
		//	//	//!!!!пашет?

		//	//	Type classType = ParameterTypeUtility.GetClassType( Type );
		//	//	if( classType == null )
		//	//		return null;
		//	//	return Marshal.PtrToStructure( (IntPtr)p, classType );
		//	//}

		//	//!!!!было
		//	//public object Value
		//	//{
		//	//	get
		//	//	{
		//	//		if( valueReference != null )
		//	//			return valueReference;
		//	//		else if( valueDataPosition != -1 )
		//	//		{
		//	//			//!!!!check valid array size

		//	//			int bytes = GetTotalSizeInBytes();

		//	//			unsafe
		//	//			{
		//	//				fixed ( byte* pDataContainer = container.data )
		//	//				{
		//	//					return MakeObjectFromArrayPointer( pDataContainer + valueDataPosition );
		//	//				}
		//	//			}
		//	//		}
		//	//		return null;
		//	//	}
		//	//}

		//	public int GetTotalSizeInBytes()
		//	{
		//		return ParameterTypeUtility.GetElementSizeInBytes( Type ) * elementCount;
		//	}

		//	public ArraySegment<byte> GetValue()
		//	{
		//		//!!!!!так?
		//		if( valueDataPosition == -1 )
		//			Log.Fatal( "StringKeyDataContainer: ParameterItem: GetValue: No data in byte array." );

		//		//!!!!check valid array size
		//		return new ArraySegment<byte>( container.data, valueDataPosition, GetTotalSizeInBytes() );
		//	}

		//	////!!!!надо ли
		//	//public void GetValue( byte[] outputArray, int outputArrayIndex )
		//	//{
		//	//	//!!!!!так?
		//	//	if( valueDataPosition == -1 )
		//	//		Log.Fatal( "StringKeyDataContainer: ParameterItem: GetValue: No data in byte array." );

		//	//	//!!!!check valid array size

		//	//	Buffer.BlockCopy( container.data, valueDataPosition, outputArray, outputArrayIndex, GetTotalSizeInBytes() );
		//	//}

		//	//public object DefaultValue
		//	//{
		//	//	get { return defaultValue; }
		//	//	//set
		//	//	//{
		//	//	//	this.defaultValue = value;
		//	//	//}
		//	//}

		//	public override string ToString()
		//	{
		//		return string.Format( "{0}{1} {2}", type, ( elementCount != 0 ? string.Format( "[{0}]", elementCount ) : "" ), name );
		//	}
		//}

		////////////

		//public ICollection<ParameterItem> AllParameters
		//{
		//	get
		//	{
		//		if( parameters == null )
		//			return new ParameterItem[ 0 ];
		//		return parameters.Values;
		//	}
		//}

		//!!!!было
		//public ParameterItem GetByName( string name )
		//{
		//	if( parameters == null )
		//		return null;
		//	ParameterItem item;
		//	parameters.TryGetValue( name, out item );
		//	return item;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Set( ref ViewportRenderingContext.BindTextureData data )
		{
			if( TextureParameters == null )
				TextureParameters = new OpenList<ViewportRenderingContext.BindTextureData>( 16 );
			TextureParameters.Add( ref data );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Set( ViewportRenderingContext.BindTextureData data )
		{
			Set( ref data );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void Set( string name, object value, ParameterType type = ParameterType.Unknown, int elementCount = 0 )
		{
			if( string.IsNullOrEmpty( name ) )
				Log.Fatal( "ParameterContainer: Set: Name can't be empty." );
			if( value == null )
				Log.Fatal( "ParameterContainer: Set: value == null." );

			//detect elementCount
			if( elementCount == 0 )
			{
				if( value.GetType().IsArray )
				{
					Array array = (Array)value;
					elementCount = array.GetLength( 0 );
				}
				else
					elementCount = 1;
			}

			//detect type
			if( type == ParameterType.Unknown )
			{
				Type elementType = null;
				{
					if( value.GetType().IsArray )
						elementType = value.GetType().GetElementType();
					else
						elementType = value.GetType();
				}

				//!!!!!reference values in array

				type = ParameterTypeUtility.DetectTypeByClassType( elementType );

				//!!!!было
				////!!!!!так?
				////detect texture type by the value
				//if( type == ParameterType.Unknown )
				//{
				//	GpuMaterialPass.TextureParameterValue textureValue = null;
				//	if( value.GetType().IsArray )
				//	{
				//		Array array = (Array)value;
				//		if( array.GetLength( 0 ) > 0 )
				//			textureValue = array.GetValue( 0 ) as GpuMaterialPass.TextureParameterValue;
				//	}
				//	else
				//		textureValue = value as GpuMaterialPass.TextureParameterValue;

				//	if( textureValue != null && textureValue.Texture != null )
				//	{
				//		var gpuTexture = textureValue.Texture.Result;
				//		if( gpuTexture != null )
				//		{
				//			switch( gpuTexture.TextureType )
				//			{
				//			//case ImageComponent.TypeEnum._1D: type = ParameterType.Texture1D; break;
				//			case ImageComponent.TypeEnum._2D: type = ParameterType.Texture2D; break;
				//			case ImageComponent.TypeEnum._3D: type = ParameterType.Texture3D; break;
				//			case ImageComponent.TypeEnum.Cube: type = ParameterType.TextureCube; break;
				//			}
				//		}
				//	}
				//}
			}

			//!!!!!check for invalid data

			//ParameterItem parameter = null;

			////use current parameter or new depending format
			//{
			//	ParameterItem currentItem;
			//	if( parameters.TryGetValue( name, out currentItem ) )
			//	{
			//		if( type == currentItem.type && elementCount == currentItem.elementCount )
			//		{
			//			//!!!!!так?
			//			parameter = currentItem;
			//		}
			//		else
			//			Remove( name );
			//	}
			//}

			////create parameter
			//if( parameter == null )
			//{
			//	parameter = new ParameterItem();
			//	parameter.container = this;
			//	parameter.name = name;
			//	parameter.type = type;
			//	parameter.elementCount = elementCount;
			//	parameters[ name ] = parameter;
			//}

			var parameter = new NamedParameter();
			parameter.Type = type;
			parameter.ElementCount = elementCount;

			//update value
			if( ParameterTypeUtility.CanConvertToByteArray( type ) )
			{
				//prepare data array
				//if( parameter.valueDataPosition == -1 )
				{
					int needSize = dataUsedBytes + parameter.GetTotalSizeInBytes();

					if( data == null )
						data = new byte[ 256 ];

					if( needSize > data.Length )
					{
						int newSize = data.Length * 2;
						while( newSize < needSize )
							newSize *= 2;
						byte[] newData = new byte[ newSize ];
						Buffer.BlockCopy( data, 0, newData, 0, dataUsedBytes );
						data = newData;
					}

					parameter.ValueDataPosition = dataUsedBytes;
					dataUsedBytes += parameter.GetTotalSizeInBytes();
				}

				//copy value
				unsafe
				{
					fixed( byte* pData = data )
					{
						//!!!!slowly

						if( value.GetType().IsArray )
						{
							byte* pDest = pData + parameter.ValueDataPosition;

							int elementSize = ParameterTypeUtility.GetElementSizeInBytes( parameter.Type );
							foreach( var item in (IList)value )
							{
								Marshal.StructureToPtr( item, (IntPtr)pDest, false );
								pDest += elementSize;
							}
						}
						else
							Marshal.StructureToPtr( value, (IntPtr)( pData + parameter.ValueDataPosition ), false );
					}
				}
			}
			else
				parameter.ValueReference = value;

			if( NamedParameters == null )
				NamedParameters = new Dictionary<string, NamedParameter>( 32 );
			NamedParameters[ name ] = parameter;

			unchecked
			{
				version++;
			}

			//return parameter;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void Set( string name, ParameterType type, int elementCount, IntPtr value, int checkTotalSizeInBytes )
		{
			if( NamedParameters == null )
				NamedParameters = new Dictionary<string, NamedParameter>( 32 );

			if( string.IsNullOrEmpty( name ) )
				Log.Fatal( "ParameterContainer: Set: Name can't be empty." );
			if( !ParameterTypeUtility.CanConvertToByteArray( type ) )
				Log.Fatal( "ParameterContainer: Set: !ParameterTypeUtils.CanConvertToByteArray( type )" );

			//ParameterItem parameter = null;

			////use current parameter or new depending format
			//{
			//	ParameterItem currentItem;
			//	if( parameters.TryGetValue( name, out currentItem ) )
			//	{
			//		if( type == currentItem.type && elementCount == currentItem.elementCount )
			//		{
			//			//!!!!!так?
			//			parameter = currentItem;
			//		}
			//		else
			//			Remove( name );
			//	}
			//}

			////create parameter
			//if( parameter == null )
			//{
			//	parameter = new ParameterItem();
			//	parameter.container = this;
			//	parameter.name = name;
			//	parameter.type = type;
			//	parameter.elementCount = elementCount;
			//	parameters[ name ] = parameter;
			//}

			var parameter = new NamedParameter();
			parameter.Type = type;
			parameter.ElementCount = elementCount;

			if( parameter.GetTotalSizeInBytes() != checkTotalSizeInBytes )
				Log.Fatal( "ParameterContainer: Set: parameter.GetValueSizeInBytes() != checkTotalSizeInBytes." );

			//prepare data array
			//if( parameter.valueDataPosition == -1 )
			{
				int needSize = dataUsedBytes + parameter.GetTotalSizeInBytes();

				if( data == null )
					data = new byte[ 256 ];

				if( needSize > data.Length )
				{
					int newSize = data.Length * 2;
					while( newSize < needSize )
						newSize *= 2;
					byte[] newData = new byte[ newSize ];
					Buffer.BlockCopy( data, 0, newData, 0, dataUsedBytes );
					data = newData;
				}

				parameter.ValueDataPosition = dataUsedBytes;
				dataUsedBytes += parameter.GetTotalSizeInBytes();
			}

			//copy value
			unsafe
			{
				fixed( byte* pData = data )
					NativeUtility.CopyMemory( (IntPtr)( pData + parameter.ValueDataPosition ), (IntPtr)value, parameter.GetTotalSizeInBytes() );
			}

			if( NamedParameters == null )
				NamedParameters = new Dictionary<string, NamedParameter>( 32 );
			NamedParameters[ name ] = parameter;

			unchecked
			{
				version++;
			}

			//return parameter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe void Set( string name, ParameterType type, int elementCount, void* value, int checkTotalSizeInBytes )
		{
			Set( name, type, elementCount, (IntPtr)value, checkTotalSizeInBytes );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe void Set( string name, ParameterType type, int elementCount, byte[] value, int valueStartIndex, int checkTotalSizeInBytes )
		{
			fixed( byte* pValue = value )
				Set( name, type, elementCount, pValue + valueStartIndex, checkTotalSizeInBytes );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Remove( string name )
		{
			if( NamedParameters == null )
				return false;
			if( !NamedParameters.Remove( name ) )
				return false;

			unchecked
			{
				version++;
			}

			return true;
		}

		//public void ShrinkDataArray()
		//{
		//	//!!!!!
		//	Log.Fatal( "ParameterContainer: ShrinkDataArray: impl." );
		//}

		public ulong Version
		{
			get { return version; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsEmpty()
		{
			return NamedParameters == null && UniformParameters == null && TextureParameters == null;
			//return parameters == null || parameters.Count == 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clear()
		{
			NamedParameters?.Clear();
			UniformParameters?.Clear();
			TextureParameters?.Clear();

			data = null;
			dataUsedBytes = 0;
			version = 0L;
		}
	}
}
