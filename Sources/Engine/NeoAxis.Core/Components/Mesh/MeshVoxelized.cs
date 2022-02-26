// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// A component for storage voxelized data of the mesh.
	/// </summary>
	public class MeshVoxelized : Component
	{
		//!!!!рекомпилить меш?

		/// <summary>
		/// The voxelized data.
		/// </summary>
		[DefaultValue( null )]
		public Reference<byte[]> Data
		{
			get { if( _data.BeginGet() ) Data = _data.Get( this ); return _data.value; }
			set { if( _data.BeginSet( ref value ) ) { try { DataChanged?.Invoke( this ); } finally { _data.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Data"/> property value changes.</summary>
		public event Action<MeshVoxelized> DataChanged;
		ReferenceField<byte[]> _data = null;

		/// <summary>
		/// The voxelization mode of the mesh.
		/// </summary>
		public ModeEnum Mode
		{
			get
			{
				var data = Data.Value;
				if( data != null )
				{
					unsafe
					{
						if( data.Length >= sizeof( DataHeader ) )
						{
							fixed( byte* pData2 = data )
								return ( (DataHeader*)pData2 )->Mode;
						}
					}
				}
				return ModeEnum.Basic;
			}
		}

		/// <summary>
		/// The size of the voxelized mesh.
		/// </summary>
		public Vector3I Size
		{
			get
			{
				var data = Data.Value;
				if( data != null )
				{
					unsafe
					{
						if( data.Length >= sizeof( DataHeader ) )
						{
							fixed( byte* pData2 = data )
								return ( (DataHeader*)pData2 )->Size;
						}
					}
				}
				return Vector3I.Zero;
			}
		}

		/////////////////////////////////////////

		public enum ModeEnum
		{
			//!!!!name
			Basic,

			//!!!!что именно забейклено
			//BakedMaterialData,
		}

		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct DataHeader
		{
			public int Version;
			public ModeEnum Mode;
			public Vector3I Size;
			//public int Format;
			public int Unused1;
			public int Unused2;
			public int Unused3;
			public int Unused4;
		}

		/////////////////////////////////////////

	}
}
