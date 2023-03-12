// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Represents mesh geometry: vertices structure and material data.
	/// </summary>
	public class MeshGeometry : Component
	{
		internal const int CurrentVoxelDataVersion = 3;

		//

		/// <summary>
		/// The structure of the mesh vertex.
		/// </summary>
		[Serialize]
		//[Category( "Data" )]
		[Cloneable( CloneType.Deep )]//[Cloneable( CloneType.Shallow )]//!!!!или только для типов Shallow? а как обычно - полностью?
		[DefaultValue( null )]
		public virtual Reference<VertexElement[]> VertexStructure
		{
			get { if( _vertexStructure.BeginGet() ) VertexStructure = _vertexStructure.Get( this ); return _vertexStructure.value; }
			set
			{
				if( _vertexStructure.BeginSet( ref value ) )
				{
					try
					{
						VertexStructureChanged?.Invoke( this );
						ShouldRecompileMesh();
						//!!!!проверять при компиляции правильные ли данные. всё проверять
					}
					finally { _vertexStructure.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="VertexStructure"/> property value changes.</summary>
		public event Action<MeshGeometry> VertexStructureChanged;
		ReferenceField<VertexElement[]> _vertexStructure;

		/// <summary>
		/// Specifies what texture coordinate channel contains unwrapped UV coordinates.
		/// </summary>
		[DefaultValue( UnwrappedUVEnum.None )]
		public Reference<UnwrappedUVEnum> UnwrappedUV
		{
			get { if( _unwrappedUV.BeginGet() ) UnwrappedUV = _unwrappedUV.Get( this ); return _unwrappedUV.value; }
			set
			{
				if( _unwrappedUV.BeginSet( ref value ) )
				{
					try
					{
						UnwrappedUVChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _unwrappedUV.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="UnwrappedUV"/> property value changes.</summary>
		public event Action<MeshGeometry> UnwrappedUVChanged;
		ReferenceField<UnwrappedUVEnum> _unwrappedUV = UnwrappedUVEnum.None;

		/// <summary>
		/// The mesh geometry vertices data.
		/// </summary>
		[Serialize]
		//[Category( "Data" )]
		[Cloneable( CloneType.Deep )]//[Cloneable( CloneType.Shallow )]
		[DefaultValue( null )]
		public virtual Reference<byte[]> Vertices
		{
			get { if( _vertices.BeginGet() ) Vertices = _vertices.Get( this ); return _vertices.value; }
			set
			{
				//!!!!так проверять? массив ведь. где еще так
				if( _vertices.BeginSet( ref value ) )
				{
					try
					{
						VerticesChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _vertices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Vertices"/> property value changes.</summary>
		public event Action<MeshGeometry> VerticesChanged;
		ReferenceField<byte[]> _vertices;

		/// <summary>
		/// The mesh geometry indices data.
		/// </summary>
		[Serialize]
		//[Category( "Data" )]
		[Cloneable( CloneType.Deep )]//[Cloneable( CloneType.Shallow )]
		[DefaultValue( null )]
		public Reference<int[]> Indices
		{
			get { if( _indices.BeginGet() ) Indices = _indices.Get( this ); return _indices.value; }
			set
			{
				if( _indices.BeginSet( ref value ) )
				{
					try
					{
						IndicesChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _indices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Indices"/> property value changes.</summary>
		public event Action<MeshGeometry> IndicesChanged;
		ReferenceField<int[]> _indices;

		/// <summary>
		/// The material of a mesh geometry.
		/// </summary>
		[Serialize]
		//[Category( "Data" )]
		[Cloneable( CloneType.Shallow )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set
			{
				if( _material.BeginSet( ref value ) )
				{
					try
					{
						MaterialChanged?.Invoke( this );

						//!!!!так?
						ShouldRecompileMesh();
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<MeshGeometry> MaterialChanged;
		ReferenceField<Material> _material;

		/// <summary>
		/// A voxelized data of the mesh.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Voxelized" )]
		public Reference<byte[]> VoxelData
		{
			get { if( _voxelData.BeginGet() ) VoxelData = _voxelData.Get( this ); return _voxelData.value; }
			set { if( _voxelData.BeginSet( ref value ) ) { try { VoxelDataChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _voxelData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelData"/> property value changes.</summary>
		public event Action<MeshGeometry> VoxelDataChanged;
		ReferenceField<byte[]> _voxelData = null;

		/// <summary>
		/// The size of a voxel grid.
		/// </summary>
		[Category( "Voxelized" )]
		public Vector3I VoxelGridSize
		{
			get
			{
				unsafe
				{
					var data = VoxelData.Value;
					if( data != null && data.Length >= sizeof( VoxelDataHeader ) )
					{
						fixed( byte* pData2 = data )
							return ( (VoxelDataHeader*)pData2 )->GridSize;
					}
				}
				return Vector3I.Zero;
			}
		}

		/// <summary>
		/// The amount of voxels in the grid.
		/// </summary>
		[Category( "Voxelized" )]
		public int VoxelCount
		{
			get
			{
				unsafe
				{
					var data = VoxelData.Value;
					if( data != null && data.Length >= sizeof( VoxelDataHeader ) )
					{
						fixed( byte* pData2 = data )
							return ( (VoxelDataHeader*)pData2 )->VoxelCount;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// The format of a voxel grid.
		/// </summary>
		[Category( "Voxelized" )]
		public VertexFormatEnum VoxelFormat
		{
			get
			{
				unsafe
				{
					var data = VoxelData.Value;
					if( data != null && data.Length >= sizeof( VoxelDataHeader ) )
					{
						fixed( byte* pData2 = data )
							return ( (VoxelDataHeader*)pData2 )->Format != 0 ? VertexFormatEnum.Full : VertexFormatEnum.Basic;
					}
				}
				return VertexFormatEnum.Basic;
			}
		}

		/// <summary>
		/// The fill holes distance setting of a voxel grid.
		/// </summary>
		[Category( "Voxelized" )]
		public double VoxelFillHolesDistance
		{
			get
			{
				unsafe
				{
					var data = VoxelData.Value;
					if( data != null && data.Length >= sizeof( VoxelDataHeader ) )
					{
						fixed( byte* pData2 = data )
						{
							VoxelDataHeader* pData3 = (VoxelDataHeader*)pData2;
							if( pData3->Version >= CurrentVoxelDataVersion )
								return pData3->FillHolesDistance;
						}
					}
				}
				return 1.1;
			}
		}

		///// <summary>
		///// A virtualized data of the geometry.
		///// </summary>
		//[DefaultValue( null )]
		//[Category( "Virtualized" )]
		//public Reference<byte[]> VirtualizedData
		//{
		//	get { if( _virtualizedData.BeginGet() ) VirtualizedData = _virtualizedData.Get( this ); return _virtualizedData.value; }
		//	set { if( _virtualizedData.BeginSet( ref value ) ) { try { VirtualizedDataChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _virtualizedData.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VirtualizedData"/> property value changes.</summary>
		//public event Action<MeshGeometry> VirtualizedDataChanged;
		//ReferenceField<byte[]> _virtualizedData = null;

		///// <summary>
		///// The format of the virtualized mesh.
		///// </summary>
		//[Category( "Virtualized" )]
		//public VertexFormatEnum VirtualizedFormat
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			var data = VirtualizedData.Value;
		//			if( data != null && data.Length >= sizeof( VirtualizedDataHeader ) )
		//			{
		//				fixed( byte* pData2 = data )
		//				{
		//					return ( ( (VirtualizedDataHeader*)pData2 )->Flags & VirtualizedDataHeader.FlagsEnum.FullFormat ) != 0 ? VertexFormatEnum.Full : VertexFormatEnum.Basic;
		//				}
		//			}
		//		}
		//		return VertexFormatEnum.Basic;
		//	}
		//}

		///// <summary>
		///// The amount of acceleration structure nodes in the virtualized mesh.
		///// </summary>
		//[Category( "Virtualized" )]
		//public int VirtualizedNodes
		//{
		//	get
		//	{
		//		unsafe
		//		{
		//			var data = VirtualizedData.Value;
		//			if( data != null && data.Length >= sizeof( VirtualizedDataHeader ) )
		//			{
		//				fixed( byte* pData2 = data )
		//					return ( (VirtualizedDataHeader*)pData2 )->NodeCount;
		//			}
		//		}
		//		return 0;
		//	}
		//}


		//bad for rendering. need additional transform
		////TransformRelativeToParent
		//ReferenceField<Transform> _transformRelativeToParent = NeoAxis.Transform.Identity;
		//[Serialize]
		//[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		//public Reference<Transform> TransformRelativeToParent
		//{
		//	get
		//	{
		//		if( _transformRelativeToParent.BeginGet() )
		//			TransformRelativeToParent = _transformRelativeToParent.Get( this );
		//		return _transformRelativeToParent.value;
		//	}
		//	set
		//	{
		//		//!!!!threading. надо ли. как. где еще так
		//		//!!!!slowly?
		//		//!!!!!!возможно выставлять без проверок. SetTransformNoChecks

		//		if( _transformRelativeToParent.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				TransformRelativeToParentChanged?.Invoke( this );
		//				ShouldRecompile = true;
		//			}
		//			finally { _transformRelativeToParent.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<MeshGeometry> TransformRelativeToParentChanged;
		////protected virtual void OnTransformChanged()
		////{
		////}

		/////////////////////////////////////////

		//48 bytes
		[StructLayout( LayoutKind.Sequential )]
		public struct VoxelDataHeader
		{
			public int Version;
			public Vector3I GridSize;
			public Vector3F BoundsMin;
			public float CellSize;
			public int Format;
			public int VoxelCount;//VoxelDataCount
			public float FillHolesDistance;

			////16 bytes
			//public float Version;
			////public float TotalSize;
			//public float VoxelCount;
			////public float VoxelDataCount;
			//public float Format;
			//public float FillHolesDistance;

			////16 bytes
			//public Vector3F GridSize;
			//public float CellSize;

			////16 bytes
			//public Vector3F BoundsMin;
			//public float Unused;
		}

		/////////////////////////////////////////

		//[StructLayout( LayoutKind.Sequential )]
		//public struct VirtualizedDataHeader
		//{
		//	//16 bytes
		//	public int Version;
		//	public FlagsEnum Flags;
		//	public int VertexCount;
		//	public int TriangleCount;

		//	//16 bytes
		//	public int NodeCount;
		//	public int Unused1;
		//	public int Unused2;
		//	public int Unused3;

		//	[Flags]
		//	public enum FlagsEnum
		//	{
		//		None,
		//		FullFormat = 1,
		//	}
		//}

		/////////////////////////////////////////

		//[StructLayout( LayoutKind.Sequential )]
		//public struct ClusterDataHeaderClusterInfo
		//{
		//	public FlagsEnum Flags;

		//	public int TriangleStartOffset;
		//	public int TriangleCount;

		//	public int ActualVertexCount;
		//	public int ActualTriangleCount;

		//	public int MaterialIndex;
		//	public Vector3F Position;
		//	public QuaternionF Rotation;
		//	public float CellSize;
		//	public Vector2I GridSize;
		//	public float Height;
		//	public int CellTriangleBatches;

		//	public int DataPositionInBytes;
		//	public int DataSizeInBytes;

		//	//

		//	[Flags]
		//	public enum FlagsEnum
		//	{
		//		None,
		//		TrianglesMode = 1,
		//		FullFormat = 2,
		//	}
		//}

		/////////////////////////////////////////

		public MeshGeometry()
		{
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( VoxelGridSize ):
				case nameof( VoxelCount ):
				case nameof( VoxelFormat ):
				case nameof( VoxelFillHolesDistance ):
					{
						var data = VoxelData.Value;
						if( data == null || data.Length == 0 )
							skip = true;
					}
					break;

					//case nameof( VirtualizedFormat ):
					//case nameof( VirtualizedNodes ):
					//	{
					//		var data = VirtualizedData.Value;
					//		if( data == null || data.Length == 0 )
					//			skip = true;
					//	}
					//	break;
				}
			}
		}

		/////////////////////////////////////////

		protected virtual void OnGetDataOfThisObject( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] virtualizedData, ref Mesh.StructureClass structure )
		{
		}
		public delegate void GetDataOfThisObjectEventDelegate( MeshGeometry sender, ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] virtualizedData, ref Mesh.StructureClass structure );
		public event GetDataOfThisObjectEventDelegate GetDataOfThisObjectEvent;

		unsafe void CreateVoxelDataImage( byte[] data, out ImageComponent image, out Vector4F[] voxelDataInfo )
		{
			image = null;
			voxelDataInfo = null;

			if( data.Length < sizeof( VoxelDataHeader ) )
				return;
			var header = new VoxelDataHeader();
			fixed( byte* pData = data )
				NativeUtility.CopyMemory( &header, pData, sizeof( VoxelDataHeader ) );

			var sizeInBytes = data.Length - sizeof( VoxelDataHeader );

			if( header.Version == CurrentVoxelDataVersion && sizeInBytes != 0 )
			{
				int textureSize = 1;
				while( true )
				{
					//!!!!необязательно в степени 2

					var bytes = textureSize * textureSize * 4;
					//var bytes = textureSize * textureSize * 16;
					if( bytes >= sizeInBytes )
						break;
					textureSize *= 2;
				}

				if( textureSize <= RenderingSystem.Capabilities.MaxTextureSize )
				{
					//!!!!юзать данные напрямую из VoxelData?
					var gpuData = new byte[ textureSize * textureSize * 4 ];
					//var gpuData = new byte[ textureSize * textureSize * 16 ];
					Array.Copy( data, sizeof( VoxelDataHeader ), gpuData, 0, sizeInBytes );


					image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

					bool mipmaps = false;

					image.CreateType = ImageComponent.TypeEnum._2D;
					image.CreateSize = new Vector2I( textureSize, textureSize );
					image.CreateMipmaps = mipmaps;
					image.CreateFormat = PixelFormat.Float32R;//PixelFormat.Float32RGBA;

					//image.CreateArrayLayers = imageCount;

					var usage = ImageComponent.Usages.WriteOnly;
					if( mipmaps )
						usage |= ImageComponent.Usages.AutoMipmaps;
					image.CreateUsage = usage;

					image.Enabled = true;

					var gpuTexture = image.Result;
					if( gpuTexture != null )
					{
						gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );

						//var surfaceDatas = new GpuTexture.SurfaceData[ imageCount ];

						//stride when not square

						//var current = sizeof( VoxelDataHeader );
						//for( int n = 0; n < imageCount; n++ )
						//{
						//	surfaceDatas[ n ] = new GpuTexture.SurfaceData( new ArraySegment<byte>( data, current, imageSizeInBytes ), n );
						//	current += imageSizeInBytes;
						//}

						//gpuTexture.SetData( surfaceDatas );
					}

					voxelDataInfo = new Vector4F[ 2 ];

					float fillHolesDistanceAndFormat = header.FillHolesDistance;
					if( fillHolesDistanceAndFormat < 0.001f )
						fillHolesDistanceAndFormat = 0.001f;
					if( header.Format != 0 )
						fillHolesDistanceAndFormat *= -1.0f;
					voxelDataInfo[ 0 ] = new Vector4F( header.GridSize.ToVector3F(), fillHolesDistanceAndFormat );

					//float formatAndFillHolesDistance = 0;
					//HalfType* pFormatAndFillHolesDistance = (HalfType*)&formatAndFillHolesDistance;
					//*( pFormatAndFillHolesDistance + 0 ) = new HalfType( header.Format );
					//*( pFormatAndFillHolesDistance + 1 ) = new HalfType( header.FillHolesDistance );
					//voxelDataInfo[ 0 ] = new Vector4F( header.GridSize.ToVector3F(), formatAndFillHolesDistance );

					//voxelDataInfo[ 0 ] = new Vector4F( header.GridSize.ToVector3F(), header.Format );

					voxelDataInfo[ 1 ] = new Vector4F( header.BoundsMin, header.CellSize );
				}
			}
		}

		internal void CompileDataOfThisObject( Mesh.CompiledData compiledData )// Mesh mesh, Mesh.CompiledData result )
		{
			VertexElement[] vertexStructureV = VertexStructure;
			UnwrappedUVEnum unwrappedUVV = UnwrappedUV;
			byte[] verticesV = Vertices;
			int[] indicesV = Indices;
			Material materialV = Material;
			byte[] voxelDataV = VoxelData;
			byte[] virtualizedDataV = null;//VirtualizedData;
			Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref voxelDataV, ref virtualizedDataV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref voxelDataV, ref virtualizedDataV, ref structure );

			//add to result
			if( vertexStructureV != null && vertexStructureV.Length != 0 && verticesV != null && verticesV.Length != 0 && ( indicesV == null || indicesV.Length != 0 ) )
			{
				vertexStructureV.GetInfo( out var vertexSize, out var holes );
				//if( !holes )
				{
					int vertexCount = verticesV.Length / vertexSize;

					var op = new RenderingPipeline.RenderSceneData.MeshDataRenderOperation( this, Parent.Components.IndexOf( this ) );

					op.VertexStructure = vertexStructureV;
					op.VertexStructureContainsColor = op.VertexStructure.Any( e => e.Semantic == VertexElementSemantic.Color0 );
					op.UnwrappedUV = unwrappedUVV;

					var vertexDeclaration = op.VertexStructure.CreateVertexDeclaration( 0 );
					op.VertexBuffers = new GpuVertexBuffer[] { GpuBufferManager.CreateVertexBuffer( verticesV, vertexDeclaration ) };
					op.VertexStartOffset = 0;
					op.VertexCount = vertexCount;

					if( indicesV != null )
					{
						op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( indicesV );
						op.IndexStartOffset = 0;
						op.IndexCount = indicesV.Length;
					}

					op.Material = materialV;

					if( voxelDataV != null && voxelDataV.Length != 0 && RenderingSystem.VoxelLOD )
					{
						CreateVoxelDataImage( voxelDataV, out op.VoxelDataImage, out op.VoxelDataInfo );
						if( op.VoxelDataImage != null )
							op.SourceVoxelData = voxelDataV;
					}

					//if( virtualizedDataV != null && virtualizedDataV.Length != 0 && RenderingSystem.VirtualizedGeometry && indicesV != null )
					//{
					//	CreateVirtualizedDataImage( virtualizedDataV, indicesV, out op.VirtualizedDataImage, out op.VirtualizedDataInfo );
					//	if( op.VirtualizedDataImage != null )
					//		op.VirtualizedData = virtualizedDataV;
					//}

					op.GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData();

					compiledData.MeshData.RenderOperations.Add( op );

					//!!!!может мержить когда несколько. индексы MeshGeometry в RawVertices
					if( compiledData.MeshData.Structure == null )
						compiledData.MeshData.Structure = structure;
				}
				//else
				//{
				//	//!!!!!error
				//}
			}
		}

		internal void CompileDataOfThisObject( out RenderingPipeline.RenderSceneData.MeshDataRenderOperation operation )
		{
			operation = null;

			VertexElement[] vertexStructureV = VertexStructure;
			UnwrappedUVEnum unwrappedUVV = UnwrappedUV;
			byte[] verticesV = Vertices;
			int[] indicesV = Indices;
			Material materialV = Material;
			byte[] voxelDataV = VoxelData;
			byte[] virtualizedDataV = null;//VirtualizedData;
			Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref voxelDataV, ref virtualizedDataV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref voxelDataV, ref virtualizedDataV, ref structure );

			//add to result
			if( vertexStructureV != null && vertexStructureV.Length != 0 && verticesV != null && verticesV.Length != 0 && ( indicesV == null || indicesV.Length != 0 ) )
			{
				vertexStructureV.GetInfo( out var vertexSize, out var holes );
				//if( !holes )
				{
					int vertexCount = verticesV.Length / vertexSize;

					var op = new RenderingPipeline.RenderSceneData.MeshDataRenderOperation( this, Parent.Components.IndexOf( this ) );
					op.VertexStructure = vertexStructureV;
					op.VertexStructureContainsColor = op.VertexStructure.Any( e => e.Semantic == VertexElementSemantic.Color0 );
					op.UnwrappedUV = unwrappedUVV;

					var vertexDeclaration = op.VertexStructure.CreateVertexDeclaration( 0 );
					op.VertexBuffers = new GpuVertexBuffer[] { GpuBufferManager.CreateVertexBuffer( verticesV, vertexDeclaration ) };
					op.VertexStartOffset = 0;
					op.VertexCount = vertexCount;

					if( indicesV != null )
					{
						op.IndexBuffer = GpuBufferManager.CreateIndexBuffer( indicesV );
						op.IndexStartOffset = 0;
						op.IndexCount = indicesV.Length;
					}

					op.Material = materialV;

					if( voxelDataV != null && voxelDataV.Length != 0 && RenderingSystem.VoxelLOD )
					{
						CreateVoxelDataImage( voxelDataV, out op.VoxelDataImage, out op.VoxelDataInfo );
						if( op.VoxelDataImage != null )
							op.SourceVoxelData = voxelDataV;
					}

					//if( virtualizedDataV != null && virtualizedDataV.Length != 0 && RenderingSystem.VirtualizedGeometry && indicesV != null )
					//{
					//	CreateVirtualizedDataImage( virtualizedDataV, indicesV, out op.VirtualizedDataImage, out op.VirtualizedDataInfo );
					//	if( op.VirtualizedDataImage != null )
					//		op.VirtualizedData = virtualizedDataV;
					//}

					op.GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData();

					operation = op;
				}
				//else
				//{
				//	//!!!!!error
				//}
			}
		}

		//public override void OnGetSettings( SettingsProvider provider, int objectIndex )
		//{
		//	//!!!!!									
		//	//!!!!!!видать надо просто вызвать создание
		//	//provider.showComponentsTree = true;
		//	//provider.showPreview = true;

		//	base.OnGetSettings( provider, objectIndex );

		//	//provider.AddCell( typeof( SettingsHeader_Components ), true );

		//	if( provider.SelectedObjects.Length == 1 )//!!!!!!
		//		provider.AddCell( typeof( MeshSettingsCell ), true );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnPreloadResources()
		{
			base.OnPreloadResources();

			//!!!!!!
			//!!!!!получить ссылку одно - а загружать уже другое отдельное?

			//!!!!!!!!по прелодингу: стрктуру строить, но сами ресурсы не загружать сначала?
		}

		public static void MakeVertices( StandardVertex[] vertices, StandardVertex.Components vertexComponents, out VertexElement[] vertexStructure, out byte[] resultVertices )
		{
			//!!!!halfs? bool halfs;

			int vertexSize;
			var structure = StandardVertex.MakeStructure( vertexComponents, true, out vertexSize );

			int destNormalOffset = 0;
			int destTangentOffset = 0;
			int destColorOffset = 0;
			int destTexCoord0Offset = 0;
			int destTexCoord1Offset = 0;
			int destTexCoord2Offset = 0;
			int destTexCoord3Offset = 0;
			int destBlendIndicesOffset = 0;
			int destBlendWeightsOffset = 0;
			{
				foreach( var c in structure )
				{
					switch( c.Semantic )
					{
					case VertexElementSemantic.Normal: destNormalOffset = c.Offset; break;
					case VertexElementSemantic.Tangent: destTangentOffset = c.Offset; break;
					case VertexElementSemantic.Color0: destColorOffset = c.Offset; break;
					case VertexElementSemantic.TextureCoordinate0: destTexCoord0Offset = c.Offset; break;
					case VertexElementSemantic.TextureCoordinate1: destTexCoord1Offset = c.Offset; break;
					case VertexElementSemantic.TextureCoordinate2: destTexCoord2Offset = c.Offset; break;
					case VertexElementSemantic.TextureCoordinate3: destTexCoord3Offset = c.Offset; break;
					case VertexElementSemantic.BlendIndices: destBlendIndicesOffset = c.Offset; break;
					case VertexElementSemantic.BlendWeights: destBlendWeightsOffset = c.Offset; break;
					}
				}
			}

			byte[] destArray = new byte[ vertices.Length * vertexSize ];
			unsafe
			{
				fixed( byte* pDestArray = destArray )
				{
					byte* destVertex = pDestArray;

					for( int n = 0; n < vertices.Length; n++ )
					{
						StandardVertex vertex = vertices[ n ];

						*(Vector3F*)( destVertex + 0 ) = vertex.Position;

						if( ( vertexComponents & StandardVertex.Components.Normal ) != 0 )
							*(Vector3F*)( destVertex + destNormalOffset ) = vertex.Normal;
						if( ( vertexComponents & StandardVertex.Components.Tangent ) != 0 )
							*(Vector4F*)( destVertex + destTangentOffset ) = vertex.Tangent;
						if( ( vertexComponents & StandardVertex.Components.Color ) != 0 )
							*(ColorValue*)( destVertex + destColorOffset ) = vertex.Color;
						if( ( vertexComponents & StandardVertex.Components.TexCoord0 ) != 0 )
							*(Vector2F*)( destVertex + destTexCoord0Offset ) = vertex.TexCoord0;
						if( ( vertexComponents & StandardVertex.Components.TexCoord1 ) != 0 )
							*(Vector2F*)( destVertex + destTexCoord1Offset ) = vertex.TexCoord1;
						if( ( vertexComponents & StandardVertex.Components.TexCoord2 ) != 0 )
							*(Vector2F*)( destVertex + destTexCoord2Offset ) = vertex.TexCoord2;
						if( ( vertexComponents & StandardVertex.Components.TexCoord3 ) != 0 )
							*(Vector2F*)( destVertex + destTexCoord3Offset ) = vertex.TexCoord3;
						if( ( vertexComponents & StandardVertex.Components.BlendIndices ) != 0 )
							*(Vector4I*)( destVertex + destBlendIndicesOffset ) = vertex.BlendIndices;
						if( ( vertexComponents & StandardVertex.Components.BlendWeights ) != 0 )
							*(Vector4F*)( destVertex + destBlendWeightsOffset ) = vertex.BlendWeights;
						destVertex += vertexSize;
					}
				}
			}

			vertexStructure = structure;
			resultVertices = destArray;
		}

		public void SetVertexData( StandardVertex[] vertices, StandardVertex.Components vertexComponents )
		{
			MakeVertices( vertices, vertexComponents, out var vertexStructure, out var resultVertices );
			VertexStructure = vertexStructure;
			Vertices = resultVertices;

			////!!!!halfs? bool halfs;

			//int vertexSize;
			//var structure = StandardVertex.MakeStructure( vertexComponents, true, out vertexSize );

			//int destNormalOffset = 0;
			//int destTangentOffset = 0;
			//int destColorOffset = 0;
			//int destTexCoord0Offset = 0;
			//int destTexCoord1Offset = 0;
			//int destTexCoord2Offset = 0;
			//int destTexCoord3Offset = 0;
			//int destBlendIndicesOffset = 0;
			//int destBlendWeightsOffset = 0;
			//{
			//	foreach( var c in structure )
			//	{
			//		switch( c.Semantic )
			//		{
			//		case VertexElementSemantic.Normal: destNormalOffset = c.Offset; break;
			//		case VertexElementSemantic.Tangent: destTangentOffset = c.Offset; break;
			//		case VertexElementSemantic.Color0: destColorOffset = c.Offset; break;
			//		case VertexElementSemantic.TextureCoordinate0: destTexCoord0Offset = c.Offset; break;
			//		case VertexElementSemantic.TextureCoordinate1: destTexCoord1Offset = c.Offset; break;
			//		case VertexElementSemantic.TextureCoordinate2: destTexCoord2Offset = c.Offset; break;
			//		case VertexElementSemantic.TextureCoordinate3: destTexCoord3Offset = c.Offset; break;
			//		case VertexElementSemantic.BlendIndices: destBlendIndicesOffset = c.Offset; break;
			//		case VertexElementSemantic.BlendWeights: destBlendWeightsOffset = c.Offset; break;
			//		}
			//	}
			//}

			//byte[] destArray = new byte[ vertices.Length * vertexSize ];
			//unsafe
			//{
			//	fixed( byte* pDestArray = destArray )
			//	{
			//		byte* destVertex = pDestArray;

			//		for( int n = 0; n < vertices.Length; n++ )
			//		{
			//			StandardVertex vertex = vertices[ n ];

			//			*(Vector3F*)( destVertex + 0 ) = vertex.Position;

			//			if( ( vertexComponents & StandardVertex.Components.Normal ) != 0 )
			//				*(Vector3F*)( destVertex + destNormalOffset ) = vertex.Normal;
			//			if( ( vertexComponents & StandardVertex.Components.Tangent ) != 0 )
			//				*(Vector4F*)( destVertex + destTangentOffset ) = vertex.Tangent;
			//			if( ( vertexComponents & StandardVertex.Components.Color ) != 0 )
			//				*(ColorValue*)( destVertex + destColorOffset ) = vertex.Color;
			//			if( ( vertexComponents & StandardVertex.Components.TexCoord0 ) != 0 )
			//				*(Vector2F*)( destVertex + destTexCoord0Offset ) = vertex.TexCoord0;
			//			if( ( vertexComponents & StandardVertex.Components.TexCoord1 ) != 0 )
			//				*(Vector2F*)( destVertex + destTexCoord1Offset ) = vertex.TexCoord1;
			//			if( ( vertexComponents & StandardVertex.Components.TexCoord2 ) != 0 )
			//				*(Vector2F*)( destVertex + destTexCoord2Offset ) = vertex.TexCoord2;
			//			if( ( vertexComponents & StandardVertex.Components.TexCoord3 ) != 0 )
			//				*(Vector2F*)( destVertex + destTexCoord3Offset ) = vertex.TexCoord3;
			//			if( ( vertexComponents & StandardVertex.Components.BlendIndices ) != 0 )
			//				*(Vector4I*)( destVertex + destBlendIndicesOffset ) = vertex.BlendIndices;
			//			if( ( vertexComponents & StandardVertex.Components.BlendWeights ) != 0 )
			//				*(Vector4F*)( destVertex + destBlendWeightsOffset ) = vertex.BlendWeights;
			//			destVertex += vertexSize;
			//		}
			//	}
			//}

			//VertexStructure = structure;
			//Vertices = destArray;
		}

		[Browsable( false )]
		public Mesh ParentMesh
		{
			get { return Parent as Mesh; }
			//get { return FindParent<Mesh>(); }
		}

		public void ShouldRecompileMesh()
		{
			//if( EnabledInHierarchy )
			//{
			var mesh = ParentMesh;
			if( mesh != null )
				mesh.ShouldRecompile = true;
			//}
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldRecompileMesh();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			var mesh = oldParent as Mesh;//.FindThisOrParent<Mesh>();
			if( mesh != null )
				mesh.ShouldRecompile = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var mesh = ParentMesh;
			if( mesh != null )
				mesh.ShouldRecompile = true;
		}

		public static T[] VerticesExtractChannel<T>( byte[] vertices, VertexElement[] vertexComponents, VertexElementSemantic semantic ) where T : unmanaged
		{
			var vertexStructure = vertexComponents;//VertexStructure.Value;
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );

				//var vertices = Vertices.Value;
				if( vertices != null )
				{
					var vertexCount = vertices.Length / vertexSize;

					if( vertexStructure.GetElementBySemantic( semantic, out var element ) )
					{
						unsafe
						{
							if( VertexElement.GetSizeInBytes( element.Type ) == sizeof( T ) )
							{
								T[] result = new T[ vertexCount ];
								fixed( byte* pVertices = vertices )
								{
									byte* src = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										result[ n ] = *(T*)src;
										src += vertexSize;
									}
								}
								return result;
							}
							else
							{
								if( sizeof( T ) == sizeof( Vector4F ) && element.Type == VertexElementType.Half4 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector4H*)src ).ToVector4F();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								if( sizeof( T ) == sizeof( Vector4H ) && element.Type == VertexElementType.Float4 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector4F*)src ).ToVector4H();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								if( sizeof( T ) == sizeof( Vector3F ) && element.Type == VertexElementType.Half3 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector3H*)src ).ToVector3F();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								if( sizeof( T ) == sizeof( Vector3H ) && element.Type == VertexElementType.Float3 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector3F*)src ).ToVector3H();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								if( sizeof( T ) == sizeof( Vector2F ) && element.Type == VertexElementType.Half2 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector2H*)src ).ToVector2F();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								if( sizeof( T ) == sizeof( Vector2H ) && element.Type == VertexElementType.Float2 )
								{
									T[] result = new T[ vertexCount ];
									fixed( byte* pVertices = vertices )
									{
										byte* src = pVertices + element.Offset;
										for( int n = 0; n < vertexCount; n++ )
										{
											var v = ( *(Vector2F*)src ).ToVector2H();
											result[ n ] = *(T*)&v;
											src += vertexSize;
										}
									}
									return result;
								}

								//!!!!support all useful convertions

							}
						}
					}
				}
			}

			return null;
		}

		public T[] VerticesExtractChannel<T>( VertexElementSemantic semantic ) where T : unmanaged
		{
			return VerticesExtractChannel<T>( Vertices, VertexStructure, semantic );
		}

		public float[] VerticesExtractChannelSingle( VertexElementSemantic semantic ) { return VerticesExtractChannel<float>( semantic ); }
		public Vector2F[] VerticesExtractChannelVector2F( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector2F>( semantic ); }
		public Vector3F[] VerticesExtractChannelVector3F( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector3F>( semantic ); }
		public Vector4F[] VerticesExtractChannelVector4F( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector4F>( semantic ); }
		public int[] VerticesExtractChannelInteger( VertexElementSemantic semantic ) { return VerticesExtractChannel<int>( semantic ); }
		public Vector2I[] VerticesExtractChannelVector2I( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector2I>( semantic ); }
		public Vector3I[] VerticesExtractChannelVector3I( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector3I>( semantic ); }
		public Vector4I[] VerticesExtractChannelVector4I( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector4I>( semantic ); }
		public ColorValue[] VerticesExtractChannelColorValue( VertexElementSemantic semantic ) { return VerticesExtractChannel<ColorValue>( semantic ); }
		public ColorByte[] VerticesExtractChannelColorByte( VertexElementSemantic semantic ) { return VerticesExtractChannel<ColorByte>( semantic ); }
		public HalfType[] VerticesExtractChannelHalf( VertexElementSemantic semantic ) { return VerticesExtractChannel<HalfType>( semantic ); }
		public Vector2H[] VerticesExtractChannelVector2H( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector2H>( semantic ); }
		public Vector3H[] VerticesExtractChannelVector3H( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector3H>( semantic ); }
		public Vector4H[] VerticesExtractChannelVector4H( VertexElementSemantic semantic ) { return VerticesExtractChannel<Vector4H>( semantic ); }


		public bool VerticesWriteChannel<T>( VertexElementSemantic semantic, T[] data, byte[] writeToVertices ) where T : unmanaged
		{
			var vertexStructure = VertexStructure.Value;
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );
				var vertexCount = writeToVertices.Length / vertexSize;

				if( vertexStructure.GetElementBySemantic( semantic, out var element ) )
				{
					unsafe
					{
						if( VertexElement.GetSizeInBytes( element.Type ) == sizeof( T ) )
						{
							fixed( byte* pVertices = writeToVertices )
							{
								byte* dest = pVertices + element.Offset;
								for( int n = 0; n < vertexCount; n++ )
								{
									*(T*)dest = data[ n ];
									dest += vertexSize;
								}
							}
							return true;
						}
						else
						{
							if( element.Type == VertexElementType.Half4 && sizeof( T ) == sizeof( Vector4F ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector4H*)dest = *(Vector4F*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							if( element.Type == VertexElementType.Float4 && sizeof( T ) == sizeof( Vector4H ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector4F*)dest = *(Vector4H*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							if( element.Type == VertexElementType.Half3 && sizeof( T ) == sizeof( Vector3F ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector3H*)dest = *(Vector3F*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							if( element.Type == VertexElementType.Float3 && sizeof( T ) == sizeof( Vector3H ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector3F*)dest = *(Vector3H*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							if( element.Type == VertexElementType.Half2 && sizeof( T ) == sizeof( Vector2F ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector2H*)dest = *(Vector2F*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							if( element.Type == VertexElementType.Float2 && sizeof( T ) == sizeof( Vector2H ) )
							{
								fixed( byte* pVertices = writeToVertices )
								{
									byte* dest = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										var v = data[ n ];
										*(Vector2F*)dest = *(Vector2H*)&v;
										dest += vertexSize;
									}
								}
								return true;
							}

							//!!!!support all useful convertions

						}
					}
					//return true;
				}
			}

			return false;
		}

		public unsafe void VerticesWriteChannelSingle( VertexElementSemantic semantic, float[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector2F( VertexElementSemantic semantic, Vector2F[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector3F( VertexElementSemantic semantic, Vector3F[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector4F( VertexElementSemantic semantic, Vector4F[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelInteger( VertexElementSemantic semantic, int[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector2I( VertexElementSemantic semantic, Vector2I[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector3I( VertexElementSemantic semantic, Vector3I[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector4I( VertexElementSemantic semantic, Vector4I[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelColorValue( VertexElementSemantic semantic, ColorValue[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelColorByte( VertexElementSemantic semantic, ColorByte[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelHalf( VertexElementSemantic semantic, HalfType[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector2H( VertexElementSemantic semantic, Vector2H[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector3H( VertexElementSemantic semantic, Vector3H[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }
		public unsafe void VerticesWriteChannelVector4H( VertexElementSemantic semantic, Vector4H[] data, byte[] writeToVertices ) { VerticesWriteChannel( semantic, data, writeToVertices ); }

		public static bool VerticesExtractStandardVertex( byte[] sourceVertices, VertexElement[] vertexComponents, out StandardVertex[] vertices, out StandardVertex.Components components )
		{
			vertices = null;
			components = 0;

			var vertexStructure = vertexComponents;// VertexStructure.Value;
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );

				//var sourceVertices = Vertices.Value;
				if( sourceVertices != null )
				{
					var vertexCount = sourceVertices.Length / vertexSize;

					vertices = new StandardVertex[ vertexCount ];

					//!!!!slowly?

					//Position
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out _ ) )
					{
						var values = VerticesExtractChannel<Vector3F>( sourceVertices, vertexComponents, VertexElementSemantic.Position );
						if( values != null )
						{
							components |= StandardVertex.Components.Position;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Position = values[ n ];
							}
						}
					}

					//Normal
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out _ ) )
					{
						var values = VerticesExtractChannel<Vector3F>( sourceVertices, vertexComponents, VertexElementSemantic.Normal );
						if( values != null )
						{
							components |= StandardVertex.Components.Normal;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Normal = values[ n ];
							}
						}
					}

					//Tangent
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out _ ) )
					{
						var values = VerticesExtractChannel<Vector4F>( sourceVertices, vertexComponents, VertexElementSemantic.Tangent );
						if( values != null )
						{
							components |= StandardVertex.Components.Tangent;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Tangent = values[ n ];
							}
						}
					}

					//Color
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out _ ) )
					{
						var values = VerticesExtractChannel<ColorValue>( sourceVertices, vertexComponents, VertexElementSemantic.Color0 );
						if( values != null )
						{
							components |= StandardVertex.Components.Color;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Color = values[ n ];
							}
						}
					}
					//{
					//	if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
					//	{
					//		if( element.Type == VertexElementType.Float4 )
					//		{
					//			components |= StandardVertex.Components.Color;

					//			var values = VerticesExtractChannel<ColorValue>( VertexElementSemantic.Color0 );
					//			for( int n = 0; n < values.Length; n++ )
					//			{
					//				ref var vertex = ref vertices[ n ];
					//				vertex.Color = values[ n ];
					//			}
					//		}
					//		else if( element.Type == VertexElementType.ColorABGR )
					//		{
					//			components |= StandardVertex.Components.Color;

					//			//!!!!check

					//			var values = VerticesExtractChannel<uint>( VertexElementSemantic.Color0 );
					//			for( int n = 0; n < values.Length; n++ )
					//			{
					//				ref var vertex = ref vertices[ n ];
					//				vertex.Color = new ColorValue( ColorByte.FromABGR( values[ n ] ) );
					//			}
					//		}
					//		else if( element.Type == VertexElementType.ColorARGB )
					//		{
					//			components |= StandardVertex.Components.Color;

					//			//!!!!check

					//			var values = VerticesExtractChannel<uint>( VertexElementSemantic.Color0 );
					//			for( int n = 0; n < values.Length; n++ )
					//			{
					//				ref var vertex = ref vertices[ n ];
					//				vertex.Color = new ColorValue( ColorByte.FromARGB( values[ n ] ) );
					//			}
					//		}
					//	}
					//}

					//TexCoord0
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out _ ) )
					{
						var values = VerticesExtractChannel<Vector2F>( sourceVertices, vertexComponents, VertexElementSemantic.TextureCoordinate0 );
						if( values != null )
						{
							components |= StandardVertex.Components.TexCoord0;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord0 = values[ n ];
							}
						}
					}

					//TexCoord1
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out _ ) )
					{
						var values = VerticesExtractChannel<Vector2F>( sourceVertices, vertexComponents, VertexElementSemantic.TextureCoordinate1 );
						if( values != null )
						{
							components |= StandardVertex.Components.TexCoord1;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord1 = values[ n ];
							}
						}
					}

					//TexCoord2
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out _ ) )
					{
						var values = VerticesExtractChannel<Vector2F>( sourceVertices, vertexComponents, VertexElementSemantic.TextureCoordinate2 );
						if( values != null )
						{

							components |= StandardVertex.Components.TexCoord2;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord2 = values[ n ];
							}
						}
					}

					//TexCoord3
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate3, out _ ) )
					{
						var values = VerticesExtractChannel<Vector2F>( sourceVertices, vertexComponents, VertexElementSemantic.TextureCoordinate3 );
						if( values != null )
						{
							components |= StandardVertex.Components.TexCoord3;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord3 = values[ n ];
							}
						}
					}

					//BlendIndices
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.BlendIndices, out _ ) )
					{
						var values = VerticesExtractChannel<Vector4I>( sourceVertices, vertexComponents, VertexElementSemantic.BlendIndices );
						if( values != null )
						{
							components |= StandardVertex.Components.BlendIndices;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.BlendIndices = values[ n ];
							}
						}
					}

					//BlendWeights
					if( vertexStructure.GetElementBySemantic( VertexElementSemantic.BlendWeights, out _ ) )
					{
						var values = VerticesExtractChannel<Vector4F>( sourceVertices, vertexComponents, VertexElementSemantic.BlendWeights );
						if( values != null )
						{
							components |= StandardVertex.Components.BlendWeights;

							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.BlendWeights = values[ n ];
							}
						}
					}

					return true;
				}
			}

			return false;
		}

		public bool VerticesExtractStandardVertex( out StandardVertex[] vertices, out StandardVertex.Components components )
		{
			return VerticesExtractStandardVertex( Vertices, VertexStructure, out vertices, out components );
		}

		[DllImport( NativeUtility.library, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl )]
		static extern void meshopt_optimizeVertexCache( uint[] destination, uint[] indices, UIntPtr indexCount, UIntPtr vertexCount );

		[DllImport( NativeUtility.library, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl )]
		static extern void meshopt_optimizeOverdraw( uint[] destination, uint[] indices, UIntPtr indexCount, IntPtr vVertexPositions, UIntPtr vertexCount, UIntPtr stride, float threshold );

		[DllImport( NativeUtility.library, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl )]
		static extern uint meshopt_optimizeVertexFetch( IntPtr destination, uint[] indices, UIntPtr indexCount, IntPtr vertices, UIntPtr vertexCount, UIntPtr vertexSize );

		public void OptimizeVertexCache()
		{
			if( VertexStructure.Value != null && Vertices.Value != null && Indices.Value != null )
			{
				var structure = VertexStructure.Value;
				VertexElements.GetInfo( structure, out var vertexSize, out _ );
				var vertexCount = Vertices.Value.Length / vertexSize;

				var indices = Indices.Value;

				var indicesU = new uint[ indices.Length ];
				for( int n = 0; n < indicesU.Length; n++ )
					indicesU[ n ] = (uint)indices[ n ];

				var newIndicesU = new uint[ indices.Length ];

				meshopt_optimizeVertexCache( newIndicesU, indicesU, (UIntPtr)indicesU.Length, (UIntPtr)vertexCount );

				var newIndices = new int[ indices.Length ];
				for( int n = 0; n < indicesU.Length; n++ )
					newIndices[ n ] = (int)newIndicesU[ n ];

				Indices = newIndices;
			}
		}

		public void OptimizeOverdraw( double threshold = 1.05 )
		{
			if( VertexStructure.Value != null && Vertices.Value != null && Indices.Value != null )
			{
				var structure = VertexStructure.Value;
				VertexElements.GetInfo( structure, out var vertexSize, out _ );
				var vertexCount = Vertices.Value.Length / vertexSize;

				if( structure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement positionElement ) && positionElement.Type == VertexElementType.Float3 && positionElement.Offset == 0 )
				{
					var vertices = Vertices.Value;
					var indices = Indices.Value;

					var indicesU = new uint[ indices.Length ];
					for( int n = 0; n < indicesU.Length; n++ )
						indicesU[ n ] = (uint)indices[ n ];

					var newIndicesU = new uint[ indices.Length ];

					unsafe
					{
						fixed( byte* pVertices = vertices )
						{
							meshopt_optimizeOverdraw( newIndicesU, indicesU, (UIntPtr)indicesU.Length, (IntPtr)pVertices, (UIntPtr)vertexCount, (UIntPtr)vertexSize, (float)threshold );
						}
					}

					var newIndices = new int[ indices.Length ];
					for( int n = 0; n < indicesU.Length; n++ )
						newIndices[ n ] = (int)newIndicesU[ n ];

					Indices = newIndices;
				}
			}
		}

		public void OptimizeVertexFetch()
		{
			if( VertexStructure.Value != null && Vertices.Value != null && Indices.Value != null )
			{
				var structure = VertexStructure.Value;
				VertexElements.GetInfo( structure, out var vertexSize, out _ );
				var vertexCount = Vertices.Value.Length / vertexSize;

				var vertices = Vertices.Value;
				var indices = Indices.Value;

				var newVertices = (byte[])vertices.Clone();

				var newIndicesU = new uint[ indices.Length ];
				for( int n = 0; n < indices.Length; n++ )
					newIndicesU[ n ] = (uint)indices[ n ];

				unsafe
				{
					fixed( byte* pNewVertices = newVertices )
					{
						meshopt_optimizeVertexFetch( (IntPtr)pNewVertices, newIndicesU, (UIntPtr)newIndicesU.Length, (IntPtr)pNewVertices, (UIntPtr)vertexCount, (UIntPtr)vertexSize );
					}
				}

				var newIndices = new int[ newIndicesU.Length ];
				for( int n = 0; n < newIndicesU.Length; n++ )
					newIndices[ n ] = (int)newIndicesU[ n ];

				Vertices = newVertices;
				Indices = newIndices;
			}
		}

		public void CompressVertices()
		{
			var structure = VertexStructure.Value;
			var vertices = Vertices.Value;

			if( structure != null && vertices != null )
			{
				unsafe
				{
					VertexElements.GetInfo( structure, out var oldVertexSize, out _ );
					var vertexCount = Vertices.Value.Length / oldVertexSize;

					VertexElement[] newStructure;
					var needCompress = false;
					{
						var list = new List<VertexElement>( structure.Length );

						var offset = 0;

						foreach( var element in structure )
						{
							var newElement = element;

							//!!!!Vector3H makes stride
							//if( element.Semantic == VertexElementSemantic.Normal && element.Type == VertexElementType.Float3 )
							//{
							//	needCompress = true;
							//	newElement.Type = VertexElementType.Half3;
							//}

							if( element.Semantic == VertexElementSemantic.Tangent && element.Type == VertexElementType.Float4 ||
								element.Semantic == VertexElementSemantic.Color0 && element.Type == VertexElementType.Float4 ||
								element.Semantic == VertexElementSemantic.Color1 && element.Type == VertexElementType.Float4 ||
								element.Semantic == VertexElementSemantic.Color2 && element.Type == VertexElementType.Float4 ||
								element.Semantic == VertexElementSemantic.Color3 && element.Type == VertexElementType.Float4 ||
								element.Semantic == VertexElementSemantic.BlendWeights && element.Type == VertexElementType.Float4 )
							{
								//!!!!color, weights can be Byte8

								newElement.Type = VertexElementType.Half4;
								needCompress = true;
							}

							if( element.Semantic >= VertexElementSemantic.TextureCoordinate0 && element.Semantic <= VertexElementSemantic.TextureCoordinate7 && element.Type == VertexElementType.Float2 )
							{
								needCompress = true;
								newElement.Type = VertexElementType.Half2;
							}

							newElement.Offset = offset;
							offset += newElement.GetSizeInBytes();

							list.Add( newElement );
						}

						newStructure = list.ToArray();
					}

					if( needCompress )
					{
						newStructure.GetInfo( out var newVertexSize, out _ );

						var newVertices = new byte[ newVertexSize * vertexCount ];
						fixed( byte* pOldVertices = vertices )
						{
							fixed( byte* pNewVertices = newVertices )
							{
								byte* oldPointer = pOldVertices;
								byte* newPointer = pNewVertices;

								for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
								{
									for( int nElement = 0; nElement < structure.Length; nElement++ )
									{
										ref var oldElement = ref structure[ nElement ];
										ref var newElement = ref newStructure[ nElement ];

										var oldMemory = oldPointer + oldElement.Offset;
										var newMemory = newPointer + newElement.Offset;

										if( oldElement.Type != newElement.Type )
										{
											//!!!!Vector3H makes stride
											//if( oldElement.Semantic == VertexElementSemantic.Normal && oldElement.Type == VertexElementType.Float3 )
											//	*(Vector3H*)newMemory = *(Vector3F*)oldMemory;

											if( oldElement.Semantic == VertexElementSemantic.Tangent && oldElement.Type == VertexElementType.Float4 ||
												oldElement.Semantic == VertexElementSemantic.Color0 && oldElement.Type == VertexElementType.Float4 ||
												oldElement.Semantic == VertexElementSemantic.Color1 && oldElement.Type == VertexElementType.Float4 ||
												oldElement.Semantic == VertexElementSemantic.Color2 && oldElement.Type == VertexElementType.Float4 ||
												oldElement.Semantic == VertexElementSemantic.Color3 && oldElement.Type == VertexElementType.Float4 ||
												oldElement.Semantic == VertexElementSemantic.BlendWeights && oldElement.Type == VertexElementType.Float4 )
											{
												*(Vector4H*)newMemory = *(Vector4F*)oldMemory;
											}

											if( oldElement.Semantic >= VertexElementSemantic.TextureCoordinate0 && oldElement.Semantic <= VertexElementSemantic.TextureCoordinate7 && oldElement.Type == VertexElementType.Float2 )
											{
												*(Vector2H*)newMemory = *(Vector2F*)oldMemory;
											}

										}
										else
											NativeUtility.CopyMemory( newMemory, oldMemory, newElement.GetSizeInBytes() );
									}

									oldPointer += oldVertexSize;
									newPointer += newVertexSize;
								}
							}
						}

						VertexStructure = newStructure;
						Vertices = newVertices;
					}
				}
			}
		}

		//public bool CalculateVirtualizedData( double proxyMeshFactor, bool proxyMeshCompress, bool proxyMeshOptimize )
		//{
		//	if( VertexStructure.Value == null || Vertices.Value == null || Indices.Value == null )
		//		return false;
		//	if( VoxelData.Value != null )
		//		return false;

		//	var calculator = new MeshGeometry_CalculateVirtualized();
		//	calculator.Geometry = this;
		//	calculator.ProxyMeshFactor = proxyMeshFactor;
		//	calculator.ProxyMeshCompress = proxyMeshCompress;
		//	calculator.ProxyMeshOptimize = proxyMeshOptimize;
		//	return calculator.Calculate();
		//}

		//public bool GetClusterInfo( out VirtualizedDataHeader header, out ClusterDataHeaderClusterInfo[] clustersInfo )
		//{
		//	return GetClusterInfo( ClusterData.Value, out header, out clustersInfo );
		//}

		//public bool ExtractClusterInfo( out ClusterDataHeader header, out ClusterDataHeaderClusterInfo[] clustersInfo )
		//{
		//	unsafe
		//	{
		//		var clusterData = ClusterData.Value;
		//		if( clusterData != null && clusterData.Length >= sizeof( ClusterDataHeader ) )
		//		{
		//			fixed( byte* pClusterData = clusterData )
		//			{
		//				var header2 = (ClusterDataHeader*)pClusterData;
		//				header = *header2;

		//				var clusterCount = header2->ClusterCount;

		//				clustersInfo = new ClusterDataHeaderClusterInfo[ clusterCount ];

		//				var clustersInfoSize = sizeof( ClusterDataHeaderClusterInfo ) * clusterCount;

		//				if( clusterData.Length >= sizeof( ClusterDataHeader ) + clustersInfoSize )
		//				{
		//					fixed( ClusterDataHeaderClusterInfo* pResult = clustersInfo )
		//						NativeUtility.CopyMemory( pResult, pClusterData + sizeof( ClusterDataHeader ), clustersInfoSize );
		//				}
		//			}
		//		}
		//	}

		//	header = default;
		//	clustersInfo = null;
		//	return false;
		//}

		//unsafe void CreateVirtualizedDataImage( byte[] virtualizedData, int[] indices, out ImageComponent image, out Vector4F[] virtualizedDataInfo )
		//{
		//	image = null;
		//	virtualizedDataInfo = null;

		//	if( virtualizedData.Length < sizeof( VirtualizedDataHeader ) )
		//		return;

		//	fixed( byte* pVirtualizedData = virtualizedData )
		//	{
		//		var header = (VirtualizedDataHeader*)pVirtualizedData;

		//		if( header->Version == 1 )
		//		{
		//			var fullFormat = ( header->Flags & VirtualizedDataHeader.FlagsEnum.FullFormat ) != 0;


		//			//!!!!передавать ArraySegment. вокселям тоже

		//			var dataSize = virtualizedData.Length - sizeof( VirtualizedDataHeader );

		//			var writer = new ArrayDataWriter( dataSize );
		//			writer.Write( pVirtualizedData + sizeof( VirtualizedDataHeader ), dataSize );




		//			//var clusterCount = header->ClusterCount;
		//			//var triangleCount = indices.Length / 3;
		//			//var tableSizeInVec4 = ( triangleCount + 3 ) / 4;

		//			//var clusterDataSizesInVec4 = new Dictionary<int, int>();
		//			//{
		//			//	for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//			//	{
		//			//		var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

		//			//		var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
		//			//		var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

		//			//		int dataSizeInVec4;

		//			//		if( !trianglesMode )
		//			//		{
		//			//			//clustered
		//			//			dataSizeInVec4 = 3 + clusterHeader->DataSizeInBytes / 16;
		//			//		}
		//			//		else
		//			//		{
		//			//			//separate
		//			//			var verticesInVec4 = clusterHeader->ActualVertexCount * ( 2 + ( fullFormat ? 1 : 0 ) );
		//			//			var trianglesInVec4 = clusterHeader->ActualTriangleCount * 1;
		//			//			dataSizeInVec4 = verticesInVec4 + trianglesInVec4;
		//			//		}

		//			//		clusterDataSizesInVec4[ nCluster ] = dataSizeInVec4;
		//			//	}
		//			//}

		//			//var clusterDataOffsetsInVec4 = new Dictionary<int, int>();
		//			//var totalDataSizeInVec4 = tableSizeInVec4;
		//			//foreach( var i in clusterDataSizesInVec4 )
		//			//{
		//			//	clusterDataOffsetsInVec4[ i.Key ] = totalDataSizeInVec4;
		//			//	totalDataSizeInVec4 += i.Value;
		//			//}

		//			//var writer = new ArrayDataWriter( header->DataSize );//!!!! totalDataSizeInVec4 * 16 );
		//			//writer.Write( pVirtualizedData + sizeof( VirtualizedDataHeader ), header->DataSize );

		//			////write triangle index to data table
		//			//{
		//			//	var table = new float[ tableSizeInVec4 * 4 ];

		//			//	for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//			//	{
		//			//		var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );
		//			//		var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
		//			//		var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

		//			//		var clusterDataOffsetInVec4 = clusterDataOffsetsInVec4[ nCluster ];

		//			//		if( !trianglesMode )
		//			//		{
		//			//			for( var n = 0; n < clusterHeader->TriangleCount; n++ )
		//			//				table[ clusterHeader->TriangleStartOffset + n ] = clusterDataOffsetInVec4;
		//			//		}
		//			//		else
		//			//		{
		//			//			var vertexSizeInVec4 = 2 + ( fullFormat ? 1 : 0 );
		//			//			//find cluster + triangles in cluster offset
		//			//			var verticesSizeInVec4 = clusterHeader->ActualVertexCount * vertexSizeInVec4;
		//			//			var trianglesPositionInVec4 = clusterDataOffsetInVec4 + verticesSizeInVec4;

		//			//			for( var n = 0; n < clusterHeader->TriangleCount; n++ )
		//			//			{
		//			//				var value = trianglesPositionInVec4 + n;
		//			//				value = -( value + 1 );
		//			//				table[ clusterHeader->TriangleStartOffset + n ] = value;
		//			//			}
		//			//		}
		//			//	}

		//			//	writer.Write( table );
		//			//}

		//			////write clusters
		//			//for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//			//{
		//			//	var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

		//			//	var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
		//			//	var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;
		//			//	//var vertexSizeInVec4 = 2 + ( fullFormat ? 1 : 0 );

		//			//	var gridOffsetLocal = 3;
		//			//	var cellTrianglesOffsetLocal = gridOffsetLocal + clusterHeader->GridSize.X * clusterHeader->GridSize.Y;
		//			//	var trianglesOffsetLocal = cellTrianglesOffsetLocal + clusterHeader->CellTriangleBatches;
		//			//	var verticesOffsetLocal = trianglesOffsetLocal + ( clusterHeader->ActualTriangleCount + 1 ) / 2;

		//			//	//var clusterDataOffsetInFloats = clusterDataOffsetsInFloats[ nCluster ];

		//			//	if( !trianglesMode )
		//			//	{
		//			//		//clustered

		//			//		//header

		//			//		//16 bytes
		//			//		writer.Write( clusterHeader->Position );
		//			//		writer.Write( (HalfType)clusterHeader->Rotation.X );
		//			//		writer.Write( (HalfType)clusterHeader->Rotation.Y );

		//			//		//16 bytes
		//			//		writer.Write( (HalfType)clusterHeader->Rotation.Z );
		//			//		writer.Write( (HalfType)clusterHeader->Rotation.W );
		//			//		writer.Write( (HalfType)clusterHeader->GridSize.X );
		//			//		writer.Write( (HalfType)clusterHeader->GridSize.Y );
		//			//		writer.Write( (HalfType)clusterHeader->CellSize );
		//			//		writer.Write( (HalfType)( clusterHeader->Height * ( fullFormat ? -1.0f : 1.0f ) ) );
		//			//		writer.Write( (HalfType)clusterHeader->MaterialIndex );
		//			//		writer.Write( (HalfType)clusterHeader->CellTriangleBatches );

		//			//		//16 bytes
		//			//		writer.Write( (float)verticesOffsetLocal );
		//			//		writer.Write( (float)clusterHeader->ActualTriangleCount );
		//			//		writer.Write( (float)0.0f );
		//			//		writer.Write( (float)0.0f );

		//			//		//body
		//			//		writer.Write( pVirtualizedData + clusterHeader->DataPositionInBytes, clusterHeader->DataSizeInBytes );
		//			//	}
		//			//	else
		//			//	{
		//			//		//separate

		//			//		var sourceVertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
		//			//		var sourceVerticesPointer = pVirtualizedData + clusterHeader->DataPositionInBytes;
		//			//		var sourceTrianglesPointer = sourceVerticesPointer + clusterHeader->ActualVertexCount * sourceVertexSizeInBytes;

		//			//		var destVertexOffsetInVec4 = new int[ clusterHeader->ActualVertexCount ];

		//			//		//write vertices
		//			//		for( int nVertex = 0; nVertex < clusterHeader->ActualVertexCount; nVertex++ )
		//			//		{
		//			//			destVertexOffsetInVec4[ nVertex ] = writer.Length / 16;

		//			//			var sourceVertexPointer = sourceVerticesPointer + nVertex * sourceVertexSizeInBytes;
		//			//			writer.Write( sourceVertexPointer, sourceVertexSizeInBytes );
		//			//		}

		//			//		//write triangles
		//			//		for( int nTriangle = 0; nTriangle < clusterHeader->ActualTriangleCount; nTriangle++ )
		//			//		{
		//			//			var sourceTrianglePointer = sourceTrianglesPointer + nTriangle * 12;
		//			//			var vertexIndex0 = *(int*)( sourceTrianglePointer + 0 );
		//			//			var vertexIndex1 = *(int*)( sourceTrianglePointer + 4 );
		//			//			var vertexIndex2 = *(int*)( sourceTrianglePointer + 8 );

		//			//			Vector3F data;
		//			//			data.X = destVertexOffsetInVec4[ vertexIndex0 ];
		//			//			data.Y = destVertexOffsetInVec4[ vertexIndex1 ];
		//			//			data.Z = destVertexOffsetInVec4[ vertexIndex2 ];
		//			//			writer.Write( &data, 12 );

		//			//			//!!!!check
		//			//			writer.Write( new HalfType( clusterHeader->MaterialIndex ) );
		//			//			writer.Write( new HalfType( fullFormat ? 1.0f : -1.0f ) );

		//			//			//data.W = fullFormat ? 1 : 0;
		//			//			//writer.Write( &data, 16 );
		//			//		}
		//			//	}
		//			//}

		//			//if( writer.Length != totalDataSizeInVec4 * 16 )
		//			//	Log.Fatal( "MeshGeometry: CreateVirtualizedDataImage: writer.Length != totalDataSizeInVec4 * 16." );


		//			//create texture

		//			var resultData = writer.ToArray();
		//			var sizeInBytes = resultData.Length;

		//			//!!!!необязательно степени 2. лишь бы stride тот же был. где еще так
		//			int textureSize = 1;
		//			while( true )
		//			{
		//				var bytes = textureSize * textureSize * 16;
		//				if( bytes >= sizeInBytes )
		//					break;
		//				textureSize *= 2;
		//			}

		//			if( textureSize <= RenderingSystem.Capabilities.MaxTextureSize )
		//			{
		//				var gpuData = new byte[ textureSize * textureSize * 16 ];
		//				Array.Copy( resultData, 0, gpuData, 0, sizeInBytes );

		//				image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

		//				image.CreateType = ImageComponent.TypeEnum._2D;
		//				image.CreateSize = new Vector2I( textureSize, textureSize );
		//				image.CreateMipmaps = false;
		//				image.CreateFormat = PixelFormat.Float32RGBA;
		//				//image.CreateArrayLayers = imageCount;
		//				image.CreateUsage = ImageComponent.Usages.WriteOnly;

		//				image.Enabled = true;

		//				var gpuTexture = image.Result;
		//				if( gpuTexture != null )
		//					gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );


		//				var vertexSizeInVec4 = 2 + ( fullFormat ? 1 : 0 );
		//				var verticesSizeInVec4 = header->VertexCount * vertexSizeInVec4;
		//				var trianglesSizeInVec4 = header->TriangleCount;

		//				var trianglesOffset = verticesSizeInVec4;
		//				var nodesOffset = trianglesOffset + trianglesSizeInVec4;

		//				virtualizedDataInfo = new Vector4F[ 1 ];
		//				virtualizedDataInfo[ 0 ] = new Vector4F( trianglesOffset, nodesOffset, fullFormat ? 1.0f : -1.0f, 0 );
		//			}
		//		}
		//	}
		//}

		//public static bool GetVirtualizedDataInfo( byte[] virtualizedData, out VirtualizedDataHeader header/*, out ClusterDataHeaderClusterInfo[] clustersInfo */)
		//{
		//	unsafe
		//	{
		//		if( virtualizedData != null && virtualizedData.Length >= sizeof( VirtualizedDataHeader ) )
		//		{
		//			fixed( byte* pVirtualizedData = virtualizedData )
		//			{
		//				var header2 = (VirtualizedDataHeader*)pVirtualizedData;
		//				header = *header2;

		//				//var clusterCount = header2->ClusterCount;
		//				//clustersInfo = new ClusterDataHeaderClusterInfo[ clusterCount ];
		//				//var clustersInfoSize = sizeof( ClusterDataHeaderClusterInfo ) * clusterCount;

		//				//if( virtualizedData.Length >= sizeof( VirtualizedDataHeader ) + clustersInfoSize )
		//				//{
		//				//	fixed( ClusterDataHeaderClusterInfo* pResult = clustersInfo )
		//				//		NativeUtility.CopyMemory( pResult, pClusterData + sizeof( VirtualizedDataHeader ), clustersInfoSize );
		//				//}
		//			}

		//			return true;
		//		}
		//	}

		//	header = default;
		//	//clustersInfo = null;
		//	return false;
		//}

		public static RangeI[] GetMaterialIndexRangesFromVirtualizedData( byte[] virtualizedData )
		{
			return null;

			//unsafe
			//{
			//	if( clusterData != null && clusterData.Length >= sizeof( VirtualizedDataHeader ) )
			//	{
			//		fixed( byte* pClusterData = clusterData )
			//		{
			//			var header = (VirtualizedDataHeader*)pClusterData;
			//			var clusterCount = header->ClusterCount;

			//			if( header->Version == 1 )
			//			{
			//				var result = new OpenList<RangeI>( 32 );

			//				for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
			//				{
			//					var clusterHeader = (ClusterDataHeaderClusterInfo*)( pClusterData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

			//					for( int n = 0; n < clusterHeader->TriangleCount; n++ )
			//					{
			//						var tri = clusterHeader->TriangleStartOffset + n;

			//						for( int i = 0; i < 3; i++ )
			//						{
			//							var nIndex = tri * 3 + i;
			//							var materialIndex = clusterHeader->MaterialIndex;

			//							if( materialIndex >= 0 && materialIndex < 4096 ) //check for invalid data
			//							{
			//								while( materialIndex >= result.Count )
			//									result.Add( new RangeI( -1, -1 ) );

			//								ref var refRange = ref result.Data[ materialIndex ];

			//								if( refRange.Minimum != -1 )
			//								{
			//									if( nIndex < refRange.Minimum )
			//										refRange.Minimum = nIndex;
			//									if( nIndex + 1 > refRange.Maximum )
			//										refRange.Maximum = nIndex + 1;
			//								}
			//								else
			//								{
			//									refRange.Minimum = nIndex;
			//									refRange.Maximum = nIndex + 1;
			//								}
			//							}
			//						}
			//					}

			//					if( result.Count != 0 )
			//					{
			//						for( int n = 0; n < result.Count; n++ )
			//						{
			//							ref var refRange = ref result.Data[ n ];
			//							if( refRange.Minimum == -1 )
			//							{
			//								refRange.Minimum = 0;
			//								refRange.Maximum = 0;
			//							}
			//						}

			//						return result.ToArray();
			//					}
			//				}
			//			}
			//		}
			//	}

			//	return null;
			//}
		}

		public static RangeI[] GetMaterialIndexRangesFromVertexMaterialIndexes( float[] vertexMaterialIndexes, int[] indices )
		{
			var result = new OpenList<RangeI>( 32 );

			for( int nIndex = 0; nIndex < indices.Length; nIndex++ )
			{
				var index = indices[ nIndex ];
				var materialIndex = (int)vertexMaterialIndexes[ index ];

				if( materialIndex >= 0 && materialIndex < 4096 ) //check for invalid data
				{
					while( materialIndex >= result.Count )
						result.Add( new RangeI( -1, -1 ) );

					ref var refRange = ref result.Data[ materialIndex ];

					if( refRange.Minimum != -1 )
					{
						if( nIndex < refRange.Minimum )
							refRange.Minimum = nIndex;
						if( nIndex + 1 > refRange.Maximum )
							refRange.Maximum = nIndex + 1;
					}
					else
					{
						refRange.Minimum = nIndex;
						refRange.Maximum = nIndex + 1;
					}
				}
			}

			if( result.Count != 0 )
			{
				for( int n = 0; n < result.Count; n++ )
				{
					ref var refRange = ref result.Data[ n ];
					if( refRange.Minimum == -1 )
					{
						refRange.Minimum = 0;
						refRange.Maximum = 0;
					}
				}

				return result.ToArray();
			}

			return null;
		}

		//public class ExtractActualGeometryInfo
		//{
		//	public int TriangleCount;
		//	public int VertexCount;
		//}

		//public ExtractActualGeometryInfo ExtractActualGeometryInfo()
		//{
		//}

		//public class ExtractActualGeometryItem
		//{
		//	public bool Virtualized;
		//	//public int ClusterIndex;//can be -1
		//	//public bool ClusterSeparate;
		//	public StandardVertex[] Vertices;
		//	public StandardVertex.Components VertexComponents;
		//	public int[] Indices;


		//	//public Vector3F TransformPosition;
		//	//public QuaternionF TransformRotation;

		//	//Transform transform;

		//	//

		//	//public Transform Transform
		//	//{
		//	//	get
		//	//	{
		//	//		if( transform == null )
		//	//			transform = new Transform( TransformPosition, TransformRotation );
		//	//		return transform;
		//	//	}
		//	//}

		//	//public BoundsF Bounds;

		//}

		/// <summary>
		/// Extracts actual geometry from a virtualized geometry data or from vertex index arrays.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="vertexComponents"></param>
		/// <param name="indices"></param>
		/// <param name="virtualizedData"></param>
		/// <param name="resultVertices"></param>
		/// <param name="resultIndices"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public static bool ExtractActualGeometry( byte[] vertices, VertexElement[] vertexComponents, int[] indices/*, byte[] virtualizedData*/ /*, bool checkData*/, out StandardVertex[] resultVertices, out int[] resultIndices/*, out int[] resultMaterialIndexes*/, out string error )
		{
			resultVertices = null;
			resultIndices = null;
			error = string.Empty;

			if( vertices != null && indices != null )
			{
				if( VerticesExtractStandardVertex( vertices, vertexComponents, out resultVertices, out _ ) )
				{
					resultIndices = indices;
					return true;
				}
			}

			return false;


			//if( virtualizedData != null )
			//{
			//	unsafe
			//	{
			//		if( virtualizedData.Length >= sizeof( VirtualizedDataHeader ) )
			//		{
			//			fixed( byte* pVirtualizedData = virtualizedData )
			//			{
			//				var header = (VirtualizedDataHeader*)pVirtualizedData;

			//				if( header->Version == 1 )
			//				{
			//					var fullFormat = ( header->Flags & VirtualizedDataHeader.FlagsEnum.FullFormat ) != 0;
			//					var vertexCount = header->VertexCount;
			//					var triangleCount = header->TriangleCount;

			//					var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
			//					var verticesSizeInBytes = header->VertexCount * vertexSizeInBytes;
			//					var trianglesSizeInBytes = header->TriangleCount * 16;
			//					var nodesSizeInBytes = header->NodeCount * 32;

			//					var pVertices = pVirtualizedData + sizeof( VirtualizedDataHeader );
			//					var pTriangles = pVertices + verticesSizeInBytes;
			//					var pNodes = pTriangles + trianglesSizeInBytes;

			//					if( sizeof( VirtualizedDataHeader ) + verticesSizeInBytes + trianglesSizeInBytes + nodesSizeInBytes != virtualizedData.Length )
			//					{
			//						error = "Invalid structure.";
			//						return false;
			//					}


			//					resultVertices = new StandardVertex[ vertexCount ];
			//					resultIndices = new int[ triangleCount * 3 ];

			//					//write vertices
			//					for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
			//					{
			//						var pVertex = pVertices + nVertex * vertexSizeInBytes;

			//						ref var v = ref resultVertices[ nVertex ];

			//						//16 bytes
			//						v.Position.X = *(float*)( pVertex + 0 );
			//						v.Position.Y = *(float*)( pVertex + 4 );
			//						v.Position.Z = *(float*)( pVertex + 8 );
			//						v.TexCoord0.X = *(HalfType*)( pVertex + 12 );
			//						v.TexCoord0.Y = *(HalfType*)( pVertex + 14 );

			//						//16 bytes
			//						v.Normal.X = *(HalfType*)( pVertex + 16 );
			//						v.Normal.Y = *(HalfType*)( pVertex + 18 );
			//						v.Normal.Z = *(HalfType*)( pVertex + 20 );
			//						//one half is not used
			//						v.Tangent.X = *(HalfType*)( pVertex + 24 );
			//						v.Tangent.Y = *(HalfType*)( pVertex + 26 );
			//						v.Tangent.Z = *(HalfType*)( pVertex + 28 );
			//						v.Tangent.W = *(HalfType*)( pVertex + 30 );

			//						if( fullFormat )
			//						{
			//							//16 bytes
			//							v.TexCoord1.X = *(HalfType*)( pVertex + 32 );
			//							v.TexCoord1.Y = *(HalfType*)( pVertex + 34 );
			//							v.TexCoord2.X = *(HalfType*)( pVertex + 36 );
			//							v.TexCoord2.Y = *(HalfType*)( pVertex + 38 );
			//							v.Color.Red = *(HalfType*)( pVertex + 40 );
			//							v.Color.Green = *(HalfType*)( pVertex + 42 );
			//							v.Color.Blue = *(HalfType*)( pVertex + 44 );
			//							v.Color.Alpha = *(HalfType*)( pVertex + 46 );
			//						}
			//					}

			//					//write triangles
			//					for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			//					{
			//						var sourceTrianglePointer = pTriangles + nTriangle * 16;

			//						var vertexIndex0 = (int)*(float*)( sourceTrianglePointer + 0 );
			//						var vertexIndex1 = (int)*(float*)( sourceTrianglePointer + 4 );
			//						var vertexIndex2 = (int)*(float*)( sourceTrianglePointer + 8 );
			//						//var materialIndex = (int)*(float*)( sourceTrianglePointer + 12 );

			//						//var vertexIndex0 = *(int*)( sourceTrianglePointer + 0 );
			//						//var vertexIndex1 = *(int*)( sourceTrianglePointer + 4 );
			//						//var vertexIndex2 = *(int*)( sourceTrianglePointer + 8 );
			//						//var materialIndex = *(int*)( sourceTrianglePointer + 12 );

			//						if( vertexIndex0 < 0 || vertexIndex0 >= resultVertices.Length ||
			//							vertexIndex1 < 0 || vertexIndex1 >= resultVertices.Length ||
			//							vertexIndex2 < 0 || vertexIndex2 >= resultVertices.Length )
			//						{
			//							error = "Invalid indices.";
			//							return false;
			//						}

			//						resultIndices[ nTriangle * 3 + 0 ] = vertexIndex0;
			//						resultIndices[ nTriangle * 3 + 1 ] = vertexIndex1;
			//						resultIndices[ nTriangle * 3 + 2 ] = vertexIndex2;
			//					}

			//					return true;


			//					//var clusterCount = header->ClusterCount;

			//					//var result = new List<ExtractActualGeometryItem>( clusterCount );

			//					////write clusters
			//					//for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
			//					//{
			//					//	var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

			//					//	var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
			//					//	var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

			//					//	var clusterVertexComponents = StandardVertex.Components.Position | StandardVertex.Components.Normal | StandardVertex.Components.Tangent | StandardVertex.Components.TexCoord0;
			//					//	if( fullFormat )
			//					//		clusterVertexComponents |= StandardVertex.Components.TexCoord1 | StandardVertex.Components.TexCoord2 | StandardVertex.Components.Color;


			//					//	if( !trianglesMode )
			//					//	{
			//					//		//clustered

			//					//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );

			//					//		var vertexCount = clusterHeader->ActualVertexCount;
			//					//		var triangleCount = clusterHeader->ActualTriangleCount;

			//					//		var clusterVertices = new StandardVertex[ vertexCount ];
			//					//		var clusterIndices = new int[ triangleCount * 3 ];

			//					//		var pGrid = pVirtualizedData + clusterHeader->DataPositionInBytes;
			//					//		var pCellTriangles = pGrid + clusterHeader->GridSize.X * clusterHeader->GridSize.Y * 16;
			//					//		var pTriangles = pCellTriangles + clusterHeader->CellTriangleBatches * 16;
			//					//		var pVertices = pTriangles + ( clusterHeader->ActualTriangleCount + 1 ) / 2 * 16;

			//					//		for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
			//					//		{
			//					//			ref var v = ref clusterVertices[ nVertex ];

			//					//			var pVertex = pVertices + nVertex * vertexSizeInBytes;

			//					//			//16 bytes
			//					//			v.Position.X = *(float*)pVertex;
			//					//			pVertex += 4;
			//					//			v.Position.Y = *(float*)pVertex;
			//					//			pVertex += 4;
			//					//			v.Position.Z = *(float*)pVertex;
			//					//			pVertex += 4;
			//					//			v.TexCoord0.X = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.TexCoord0.Y = *(HalfType*)pVertex;
			//					//			pVertex += 2;

			//					//			//16 bytes
			//					//			v.Normal.X = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.Normal.Y = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.Normal.Z = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			pVertex += 2;
			//					//			v.Tangent.X = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.Tangent.Y = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.Tangent.Z = *(HalfType*)pVertex;
			//					//			pVertex += 2;
			//					//			v.Tangent.W = *(HalfType*)pVertex;
			//					//			pVertex += 2;

			//					//			if( fullFormat )
			//					//			{
			//					//				//16 bytes
			//					//				v.TexCoord1.X = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.TexCoord1.Y = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.TexCoord2.X = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.TexCoord2.Y = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.Color.Red = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.Color.Green = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.Color.Blue = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//				v.Color.Alpha = *(HalfType*)pVertex;
			//					//				pVertex += 2;
			//					//			}


			//					//			//var pVertex2 = (HalfType*)pVertex;

			//					//			//ref var v = ref clusterVertices[ nVertex ];

			//					//			//v.Position.X = *( pVertex2 + 0 );
			//					//			//v.Position.Y = *( pVertex2 + 1 );
			//					//			//v.Position.Z = *( pVertex2 + 2 );

			//					//			////!!!!normal, tangent
			//					//			////*( pVertex2 + 3 );
			//					//			////*( pVertex2 + 4 );
			//					//			////*( pVertex2 + 5 );

			//					//			//v.TexCoord0.X = *( pVertex2 + 6 );
			//					//			//v.TexCoord0.Y = *( pVertex2 + 7 );

			//					//			//if( fullFormat )
			//					//			//{
			//					//			//	v.TexCoord1.X = *( pVertex2 + 8 );
			//					//			//	v.TexCoord1.Y = *( pVertex2 + 9 );
			//					//			//	v.TexCoord2.X = *( pVertex2 + 10 );
			//					//			//	v.TexCoord2.Y = *( pVertex2 + 11 );
			//					//			//	v.Color.Red = *( pVertex2 + 12 );
			//					//			//	v.Color.Green = *( pVertex2 + 13 );
			//					//			//	v.Color.Blue = *( pVertex2 + 14 );
			//					//			//	v.Color.Alpha = *( pVertex2 + 15 );
			//					//			//}
			//					//		}

			//					//		for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
			//					//		{
			//					//			var pTriangle = pTriangles + nTriangle * 8;

			//					//			var index0 = *(HalfType*)( pTriangle + 0 );
			//					//			var index1 = *(HalfType*)( pTriangle + 2 );
			//					//			var index2 = *(HalfType*)( pTriangle + 4 );

			//					//			clusterIndices[ nTriangle * 3 + 0 ] = (int)index0;
			//					//			clusterIndices[ nTriangle * 3 + 1 ] = (int)index1;
			//					//			clusterIndices[ nTriangle * 3 + 2 ] = (int)index2;
			//					//		}


			//					//		if( checkData )
			//					//		{
			//					//			//get triangles from grid, cell triangles to compare

			//					//			var usedTriangles = new bool[ triangleCount ];

			//					//			for( int y = 0; y < clusterHeader->GridSize.Y; y++ )
			//					//			{
			//					//				for( int x = 0; x < clusterHeader->GridSize.X; x++ )
			//					//				{
			//					//					var pCell = pGrid + ( clusterHeader->GridSize.X * y + x ) * 16;

			//					//					//var cellHeight = *(float*)( pCell + 0 );
			//					//					var cellTrianglesCodeF = *(float*)( pCell + 4 );
			//					//					var cellTrianglesCode = (int)cellTrianglesCodeF;

			//					//					if( cellTrianglesCode >= 0 )
			//					//					{
			//					//						int cellTrianglesIndex = cellTrianglesCode / 2;
			//					//						bool cellTrianglesTwoBatches = ( cellTrianglesCode % 2 ) != 0;

			//					//						var batchCount = cellTrianglesTwoBatches ? 2 : 1;
			//					//						for( int nBatch = 0; nBatch < batchCount; nBatch++ )
			//					//						{
			//					//							var pCellTriangle = pCellTriangles + ( cellTrianglesIndex + nBatch ) * 16;

			//					//							var pValue = (HalfType*)pCellTriangle;
			//					//							for( int n = 0; n < 4; n++ )
			//					//							{
			//					//								var triangleMaxHeight = (float)*pValue++;
			//					//								var triangleId = (int)*pValue++;

			//					//								if( triangleMaxHeight >= 0.0f )
			//					//									usedTriangles[ triangleId ] = true;
			//					//							}

			//					//							//for( int n = 0; n < 4; n++ )
			//					//							//{
			//					//							//	HalfType* pair = (HalfType*)( pCellTriangle + n * 4 );

			//					//							//	var triangleMaxHeightH = *( pair + 0 );
			//					//							//	var triangleMaxHeight = (float)triangleMaxHeightH;
			//					//							//	if( triangleMaxHeight >= 0.0f )
			//					//							//	{
			//					//							//		var triangleId = *( pair + 1 );

			//					//							//		usedTriangles[ (int)triangleId ] = true;
			//					//							//	}
			//					//							//}
			//					//						}
			//					//					}
			//					//				}
			//					//			}

			//					//			//!!!!почему-то нулевой треугольник не находится
			//					//			//foreach( var used in usedTriangles )
			//					//			//{
			//					//			//	if( !used )
			//					//			//	{
			//					//			//		error = "There are triangles that are not referenced from the grid.";
			//					//			//		return Array.Empty<ExtractActualGeometryItem>();
			//					//			//	}
			//					//			//}
			//					//		}

			//					//		var item = new ExtractActualGeometryItem();
			//					//		item.Virtualized = true;
			//					//		//item.ClusterIndex = nCluster;
			//					//		//item.TransformPosition = clusterHeader->Position;
			//					//		//item.TransformRotation = clusterHeader->Rotation;
			//					//		item.Vertices = clusterVertices;
			//					//		item.VertexComponents = clusterVertexComponents;
			//					//		item.Indices = clusterIndices;
			//					//		result.Add( item );
			//					//	}
			//					//	else
			//					//	{
			//					//		//separate

			//					//		var vertexCount = clusterHeader->ActualVertexCount;
			//					//		var triangleCount = clusterHeader->ActualTriangleCount;

			//					//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
			//					//		var pVertices = pVirtualizedData + clusterHeader->DataPositionInBytes;
			//					//		var pTriangles = pVertices + clusterHeader->ActualVertexCount * vertexSizeInBytes;

			//					//		var clusterVertices = new StandardVertex[ vertexCount ];
			//					//		var clusterIndices = new int[ triangleCount * 3 ];

			//					//		//write vertices
			//					//		for( int nVertex = 0; nVertex < clusterHeader->ActualVertexCount; nVertex++ )
			//					//		{
			//					//			var pVertex = pVertices + nVertex * vertexSizeInBytes;

			//					//			ref var v = ref clusterVertices[ nVertex ];

			//					//			//16 bytes
			//					//			v.Position.X = *(float*)( pVertex + 0 );
			//					//			v.Position.Y = *(float*)( pVertex + 4 );
			//					//			v.Position.Z = *(float*)( pVertex + 8 );
			//					//			v.TexCoord0.X = *(HalfType*)( pVertex + 12 );
			//					//			v.TexCoord0.Y = *(HalfType*)( pVertex + 14 );

			//					//			//16 bytes
			//					//			v.Normal.X = *(HalfType*)( pVertex + 16 );
			//					//			v.Normal.Y = *(HalfType*)( pVertex + 18 );
			//					//			v.Normal.Z = *(HalfType*)( pVertex + 20 );
			//					//			//one half is not used
			//					//			v.Tangent.X = *(HalfType*)( pVertex + 24 );
			//					//			v.Tangent.Y = *(HalfType*)( pVertex + 26 );
			//					//			v.Tangent.Z = *(HalfType*)( pVertex + 28 );
			//					//			v.Tangent.W = *(HalfType*)( pVertex + 30 );

			//					//			if( fullFormat )
			//					//			{
			//					//				//16 bytes
			//					//				v.TexCoord1.X = *(HalfType*)( pVertex + 32 );
			//					//				v.TexCoord1.Y = *(HalfType*)( pVertex + 34 );
			//					//				v.TexCoord2.X = *(HalfType*)( pVertex + 36 );
			//					//				v.TexCoord2.Y = *(HalfType*)( pVertex + 38 );
			//					//				v.Color.Red = *(HalfType*)( pVertex + 40 );
			//					//				v.Color.Green = *(HalfType*)( pVertex + 42 );
			//					//				v.Color.Blue = *(HalfType*)( pVertex + 44 );
			//					//				v.Color.Alpha = *(HalfType*)( pVertex + 46 );
			//					//			}
			//					//		}

			//					//		//write triangles
			//					//		for( int nTriangle = 0; nTriangle < clusterHeader->ActualTriangleCount; nTriangle++ )
			//					//		{
			//					//			var sourceTrianglePointer = pTriangles + nTriangle * 12;
			//					//			var vertexIndex0 = *(int*)( sourceTrianglePointer + 0 );
			//					//			var vertexIndex1 = *(int*)( sourceTrianglePointer + 4 );
			//					//			var vertexIndex2 = *(int*)( sourceTrianglePointer + 8 );

			//					//			clusterIndices[ nTriangle * 3 + 0 ] = vertexIndex0;
			//					//			clusterIndices[ nTriangle * 3 + 1 ] = vertexIndex1;
			//					//			clusterIndices[ nTriangle * 3 + 2 ] = vertexIndex2;
			//					//		}

			//					//		var item = new ExtractActualGeometryItem();
			//					//		item.Virtualized = true;
			//					//		//item.ClusterIndex = nCluster;
			//					//		//item.ClusterSeparate = true;
			//					//		//item.TransformRotation = QuaternionF.Identity;
			//					//		item.Vertices = clusterVertices;
			//					//		item.VertexComponents = clusterVertexComponents;
			//					//		item.Indices = clusterIndices;
			//					//		result.Add( item );
			//					//	}
			//					//}

			//					//return result.ToArray();
			//				}
			//			}
			//		}
			//	}
			//}
			//else
			//{
			//if( vertices != null && indices != null )
			//{
			//	if( VerticesExtractStandardVertex( vertices, vertexComponents, out resultVertices, out _ ) )
			//	{
			//		resultIndices = indices;
			//		return true;
			//	}
			//}
			//}

			//return false;
		}

		//public static ExtractActualGeometryItem[] ExtractActualGeometry( byte[] vertices, VertexElement[] vertexComponents, int[] indices, byte[] virtualizedData, bool checkData, out string error )
		//{
		//	error = "";
		//	//!!!!need try
		//	{
		//		if( virtualizedData != null )
		//		{
		//			unsafe
		//			{
		//				if( virtualizedData.Length >= sizeof( VirtualizedDataHeader ) )
		//				{
		//					fixed( byte* pVirtualizedData = virtualizedData )
		//					{
		//						var header = (VirtualizedDataHeader*)pVirtualizedData;

		//						if( header->Version == 1 )
		//						{

		//							zzzz;


		//							var clusterCount = header->ClusterCount;

		//							var result = new List<ExtractActualGeometryItem>( clusterCount );

		//							//write clusters
		//							for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//							{
		//								var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

		//								var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
		//								var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

		//								var clusterVertexComponents = StandardVertex.Components.Position | StandardVertex.Components.Normal | StandardVertex.Components.Tangent | StandardVertex.Components.TexCoord0;
		//								if( fullFormat )
		//									clusterVertexComponents |= StandardVertex.Components.TexCoord1 | StandardVertex.Components.TexCoord2 | StandardVertex.Components.Color;


		//								if( !trianglesMode )
		//								{
		//									//clustered

		//									var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );

		//									var vertexCount = clusterHeader->ActualVertexCount;
		//									var triangleCount = clusterHeader->ActualTriangleCount;

		//									var clusterVertices = new StandardVertex[ vertexCount ];
		//									var clusterIndices = new int[ triangleCount * 3 ];

		//									var pGrid = pVirtualizedData + clusterHeader->DataPositionInBytes;
		//									var pCellTriangles = pGrid + clusterHeader->GridSize.X * clusterHeader->GridSize.Y * 16;
		//									var pTriangles = pCellTriangles + clusterHeader->CellTriangleBatches * 16;
		//									var pVertices = pTriangles + ( clusterHeader->ActualTriangleCount + 1 ) / 2 * 16;

		//									for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
		//									{
		//										ref var v = ref clusterVertices[ nVertex ];

		//										var pVertex = pVertices + nVertex * vertexSizeInBytes;

		//										//16 bytes
		//										v.Position.X = *(float*)pVertex;
		//										pVertex += 4;
		//										v.Position.Y = *(float*)pVertex;
		//										pVertex += 4;
		//										v.Position.Z = *(float*)pVertex;
		//										pVertex += 4;
		//										v.TexCoord0.X = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.TexCoord0.Y = *(HalfType*)pVertex;
		//										pVertex += 2;

		//										//16 bytes
		//										v.Normal.X = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.Normal.Y = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.Normal.Z = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										pVertex += 2;
		//										v.Tangent.X = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.Tangent.Y = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.Tangent.Z = *(HalfType*)pVertex;
		//										pVertex += 2;
		//										v.Tangent.W = *(HalfType*)pVertex;
		//										pVertex += 2;

		//										if( fullFormat )
		//										{
		//											//16 bytes
		//											v.TexCoord1.X = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.TexCoord1.Y = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.TexCoord2.X = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.TexCoord2.Y = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.Color.Red = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.Color.Green = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.Color.Blue = *(HalfType*)pVertex;
		//											pVertex += 2;
		//											v.Color.Alpha = *(HalfType*)pVertex;
		//											pVertex += 2;
		//										}


		//										//var pVertex2 = (HalfType*)pVertex;

		//										//ref var v = ref clusterVertices[ nVertex ];

		//										//v.Position.X = *( pVertex2 + 0 );
		//										//v.Position.Y = *( pVertex2 + 1 );
		//										//v.Position.Z = *( pVertex2 + 2 );

		//										////!!!!normal, tangent
		//										////*( pVertex2 + 3 );
		//										////*( pVertex2 + 4 );
		//										////*( pVertex2 + 5 );

		//										//v.TexCoord0.X = *( pVertex2 + 6 );
		//										//v.TexCoord0.Y = *( pVertex2 + 7 );

		//										//if( fullFormat )
		//										//{
		//										//	v.TexCoord1.X = *( pVertex2 + 8 );
		//										//	v.TexCoord1.Y = *( pVertex2 + 9 );
		//										//	v.TexCoord2.X = *( pVertex2 + 10 );
		//										//	v.TexCoord2.Y = *( pVertex2 + 11 );
		//										//	v.Color.Red = *( pVertex2 + 12 );
		//										//	v.Color.Green = *( pVertex2 + 13 );
		//										//	v.Color.Blue = *( pVertex2 + 14 );
		//										//	v.Color.Alpha = *( pVertex2 + 15 );
		//										//}
		//									}

		//									for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
		//									{
		//										var pTriangle = pTriangles + nTriangle * 8;

		//										var index0 = *(HalfType*)( pTriangle + 0 );
		//										var index1 = *(HalfType*)( pTriangle + 2 );
		//										var index2 = *(HalfType*)( pTriangle + 4 );

		//										clusterIndices[ nTriangle * 3 + 0 ] = (int)index0;
		//										clusterIndices[ nTriangle * 3 + 1 ] = (int)index1;
		//										clusterIndices[ nTriangle * 3 + 2 ] = (int)index2;
		//									}


		//									if( checkData )
		//									{
		//										//get triangles from grid, cell triangles to compare

		//										var usedTriangles = new bool[ triangleCount ];

		//										for( int y = 0; y < clusterHeader->GridSize.Y; y++ )
		//										{
		//											for( int x = 0; x < clusterHeader->GridSize.X; x++ )
		//											{
		//												var pCell = pGrid + ( clusterHeader->GridSize.X * y + x ) * 16;

		//												//var cellHeight = *(float*)( pCell + 0 );
		//												var cellTrianglesCodeF = *(float*)( pCell + 4 );
		//												var cellTrianglesCode = (int)cellTrianglesCodeF;

		//												if( cellTrianglesCode >= 0 )
		//												{
		//													int cellTrianglesIndex = cellTrianglesCode / 2;
		//													bool cellTrianglesTwoBatches = ( cellTrianglesCode % 2 ) != 0;

		//													var batchCount = cellTrianglesTwoBatches ? 2 : 1;
		//													for( int nBatch = 0; nBatch < batchCount; nBatch++ )
		//													{
		//														var pCellTriangle = pCellTriangles + ( cellTrianglesIndex + nBatch ) * 16;

		//														var pValue = (HalfType*)pCellTriangle;
		//														for( int n = 0; n < 4; n++ )
		//														{
		//															var triangleMaxHeight = (float)*pValue++;
		//															var triangleId = (int)*pValue++;

		//															if( triangleMaxHeight >= 0.0f )
		//																usedTriangles[ triangleId ] = true;
		//														}

		//														//for( int n = 0; n < 4; n++ )
		//														//{
		//														//	HalfType* pair = (HalfType*)( pCellTriangle + n * 4 );

		//														//	var triangleMaxHeightH = *( pair + 0 );
		//														//	var triangleMaxHeight = (float)triangleMaxHeightH;
		//														//	if( triangleMaxHeight >= 0.0f )
		//														//	{
		//														//		var triangleId = *( pair + 1 );

		//														//		usedTriangles[ (int)triangleId ] = true;
		//														//	}
		//														//}
		//													}
		//												}
		//											}
		//										}

		//										//!!!!почему-то нулевой треугольник не находится
		//										//foreach( var used in usedTriangles )
		//										//{
		//										//	if( !used )
		//										//	{
		//										//		error = "There are triangles that are not referenced from the grid.";
		//										//		return Array.Empty<ExtractActualGeometryItem>();
		//										//	}
		//										//}
		//									}

		//									var item = new ExtractActualGeometryItem();
		//									item.Virtualized = true;
		//									//item.ClusterIndex = nCluster;
		//									//item.TransformPosition = clusterHeader->Position;
		//									//item.TransformRotation = clusterHeader->Rotation;
		//									item.Vertices = clusterVertices;
		//									item.VertexComponents = clusterVertexComponents;
		//									item.Indices = clusterIndices;
		//									result.Add( item );
		//								}
		//								else
		//								{
		//									//separate

		//									var vertexCount = clusterHeader->ActualVertexCount;
		//									var triangleCount = clusterHeader->ActualTriangleCount;

		//									var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
		//									var pVertices = pVirtualizedData + clusterHeader->DataPositionInBytes;
		//									var pTriangles = pVertices + clusterHeader->ActualVertexCount * vertexSizeInBytes;

		//									var clusterVertices = new StandardVertex[ vertexCount ];
		//									var clusterIndices = new int[ triangleCount * 3 ];

		//									//write vertices
		//									for( int nVertex = 0; nVertex < clusterHeader->ActualVertexCount; nVertex++ )
		//									{
		//										var pVertex = pVertices + nVertex * vertexSizeInBytes;

		//										ref var v = ref clusterVertices[ nVertex ];

		//										//16 bytes
		//										v.Position.X = *(float*)( pVertex + 0 );
		//										v.Position.Y = *(float*)( pVertex + 4 );
		//										v.Position.Z = *(float*)( pVertex + 8 );
		//										v.TexCoord0.X = *(HalfType*)( pVertex + 12 );
		//										v.TexCoord0.Y = *(HalfType*)( pVertex + 14 );

		//										//16 bytes
		//										v.Normal.X = *(HalfType*)( pVertex + 16 );
		//										v.Normal.Y = *(HalfType*)( pVertex + 18 );
		//										v.Normal.Z = *(HalfType*)( pVertex + 20 );
		//										//one half is not used
		//										v.Tangent.X = *(HalfType*)( pVertex + 24 );
		//										v.Tangent.Y = *(HalfType*)( pVertex + 26 );
		//										v.Tangent.Z = *(HalfType*)( pVertex + 28 );
		//										v.Tangent.W = *(HalfType*)( pVertex + 30 );

		//										if( fullFormat )
		//										{
		//											//16 bytes
		//											v.TexCoord1.X = *(HalfType*)( pVertex + 32 );
		//											v.TexCoord1.Y = *(HalfType*)( pVertex + 34 );
		//											v.TexCoord2.X = *(HalfType*)( pVertex + 36 );
		//											v.TexCoord2.Y = *(HalfType*)( pVertex + 38 );
		//											v.Color.Red = *(HalfType*)( pVertex + 40 );
		//											v.Color.Green = *(HalfType*)( pVertex + 42 );
		//											v.Color.Blue = *(HalfType*)( pVertex + 44 );
		//											v.Color.Alpha = *(HalfType*)( pVertex + 46 );
		//										}
		//									}

		//									//write triangles
		//									for( int nTriangle = 0; nTriangle < clusterHeader->ActualTriangleCount; nTriangle++ )
		//									{
		//										var sourceTrianglePointer = pTriangles + nTriangle * 12;
		//										var vertexIndex0 = *(int*)( sourceTrianglePointer + 0 );
		//										var vertexIndex1 = *(int*)( sourceTrianglePointer + 4 );
		//										var vertexIndex2 = *(int*)( sourceTrianglePointer + 8 );

		//										clusterIndices[ nTriangle * 3 + 0 ] = vertexIndex0;
		//										clusterIndices[ nTriangle * 3 + 1 ] = vertexIndex1;
		//										clusterIndices[ nTriangle * 3 + 2 ] = vertexIndex2;
		//									}

		//									var item = new ExtractActualGeometryItem();
		//									item.Virtualized = true;
		//									//item.ClusterIndex = nCluster;
		//									//item.ClusterSeparate = true;
		//									//item.TransformRotation = QuaternionF.Identity;
		//									item.Vertices = clusterVertices;
		//									item.VertexComponents = clusterVertexComponents;
		//									item.Indices = clusterIndices;
		//									result.Add( item );
		//								}
		//							}

		//							return result.ToArray();


		//							zzzzzzz;

		//							//var clusterCount = header->ClusterCount;

		//							//var result = new List<ExtractActualGeometryItem>( clusterCount );

		//							////write clusters
		//							//for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
		//							//{
		//							//	var clusterHeader = (ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( VirtualizedDataHeader ) + sizeof( ClusterDataHeaderClusterInfo ) * nCluster );

		//							//	var trianglesMode = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
		//							//	var fullFormat = ( clusterHeader->Flags & ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

		//							//	var clusterVertexComponents = StandardVertex.Components.Position | StandardVertex.Components.Normal | StandardVertex.Components.Tangent | StandardVertex.Components.TexCoord0;
		//							//	if( fullFormat )
		//							//		clusterVertexComponents |= StandardVertex.Components.TexCoord1 | StandardVertex.Components.TexCoord2 | StandardVertex.Components.Color;


		//							//	if( !trianglesMode )
		//							//	{
		//							//		//clustered

		//							//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );

		//							//		var vertexCount = clusterHeader->ActualVertexCount;
		//							//		var triangleCount = clusterHeader->ActualTriangleCount;

		//							//		var clusterVertices = new StandardVertex[ vertexCount ];
		//							//		var clusterIndices = new int[ triangleCount * 3 ];

		//							//		var pGrid = pVirtualizedData + clusterHeader->DataPositionInBytes;
		//							//		var pCellTriangles = pGrid + clusterHeader->GridSize.X * clusterHeader->GridSize.Y * 16;
		//							//		var pTriangles = pCellTriangles + clusterHeader->CellTriangleBatches * 16;
		//							//		var pVertices = pTriangles + ( clusterHeader->ActualTriangleCount + 1 ) / 2 * 16;

		//							//		for( int nVertex = 0; nVertex < vertexCount; nVertex++ )
		//							//		{
		//							//			ref var v = ref clusterVertices[ nVertex ];

		//							//			var pVertex = pVertices + nVertex * vertexSizeInBytes;

		//							//			//16 bytes
		//							//			v.Position.X = *(float*)pVertex;
		//							//			pVertex += 4;
		//							//			v.Position.Y = *(float*)pVertex;
		//							//			pVertex += 4;
		//							//			v.Position.Z = *(float*)pVertex;
		//							//			pVertex += 4;
		//							//			v.TexCoord0.X = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.TexCoord0.Y = *(HalfType*)pVertex;
		//							//			pVertex += 2;

		//							//			//16 bytes
		//							//			v.Normal.X = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.Normal.Y = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.Normal.Z = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			pVertex += 2;
		//							//			v.Tangent.X = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.Tangent.Y = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.Tangent.Z = *(HalfType*)pVertex;
		//							//			pVertex += 2;
		//							//			v.Tangent.W = *(HalfType*)pVertex;
		//							//			pVertex += 2;

		//							//			if( fullFormat )
		//							//			{
		//							//				//16 bytes
		//							//				v.TexCoord1.X = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.TexCoord1.Y = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.TexCoord2.X = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.TexCoord2.Y = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.Color.Red = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.Color.Green = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.Color.Blue = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//				v.Color.Alpha = *(HalfType*)pVertex;
		//							//				pVertex += 2;
		//							//			}


		//							//			//var pVertex2 = (HalfType*)pVertex;

		//							//			//ref var v = ref clusterVertices[ nVertex ];

		//							//			//v.Position.X = *( pVertex2 + 0 );
		//							//			//v.Position.Y = *( pVertex2 + 1 );
		//							//			//v.Position.Z = *( pVertex2 + 2 );

		//							//			////!!!!normal, tangent
		//							//			////*( pVertex2 + 3 );
		//							//			////*( pVertex2 + 4 );
		//							//			////*( pVertex2 + 5 );

		//							//			//v.TexCoord0.X = *( pVertex2 + 6 );
		//							//			//v.TexCoord0.Y = *( pVertex2 + 7 );

		//							//			//if( fullFormat )
		//							//			//{
		//							//			//	v.TexCoord1.X = *( pVertex2 + 8 );
		//							//			//	v.TexCoord1.Y = *( pVertex2 + 9 );
		//							//			//	v.TexCoord2.X = *( pVertex2 + 10 );
		//							//			//	v.TexCoord2.Y = *( pVertex2 + 11 );
		//							//			//	v.Color.Red = *( pVertex2 + 12 );
		//							//			//	v.Color.Green = *( pVertex2 + 13 );
		//							//			//	v.Color.Blue = *( pVertex2 + 14 );
		//							//			//	v.Color.Alpha = *( pVertex2 + 15 );
		//							//			//}
		//							//		}

		//							//		for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
		//							//		{
		//							//			var pTriangle = pTriangles + nTriangle * 8;

		//							//			var index0 = *(HalfType*)( pTriangle + 0 );
		//							//			var index1 = *(HalfType*)( pTriangle + 2 );
		//							//			var index2 = *(HalfType*)( pTriangle + 4 );

		//							//			clusterIndices[ nTriangle * 3 + 0 ] = (int)index0;
		//							//			clusterIndices[ nTriangle * 3 + 1 ] = (int)index1;
		//							//			clusterIndices[ nTriangle * 3 + 2 ] = (int)index2;
		//							//		}


		//							//		if( checkData )
		//							//		{
		//							//			//get triangles from grid, cell triangles to compare

		//							//			var usedTriangles = new bool[ triangleCount ];

		//							//			for( int y = 0; y < clusterHeader->GridSize.Y; y++ )
		//							//			{
		//							//				for( int x = 0; x < clusterHeader->GridSize.X; x++ )
		//							//				{
		//							//					var pCell = pGrid + ( clusterHeader->GridSize.X * y + x ) * 16;

		//							//					//var cellHeight = *(float*)( pCell + 0 );
		//							//					var cellTrianglesCodeF = *(float*)( pCell + 4 );
		//							//					var cellTrianglesCode = (int)cellTrianglesCodeF;

		//							//					if( cellTrianglesCode >= 0 )
		//							//					{
		//							//						int cellTrianglesIndex = cellTrianglesCode / 2;
		//							//						bool cellTrianglesTwoBatches = ( cellTrianglesCode % 2 ) != 0;

		//							//						var batchCount = cellTrianglesTwoBatches ? 2 : 1;
		//							//						for( int nBatch = 0; nBatch < batchCount; nBatch++ )
		//							//						{
		//							//							var pCellTriangle = pCellTriangles + ( cellTrianglesIndex + nBatch ) * 16;

		//							//							var pValue = (HalfType*)pCellTriangle;
		//							//							for( int n = 0; n < 4; n++ )
		//							//							{
		//							//								var triangleMaxHeight = (float)*pValue++;
		//							//								var triangleId = (int)*pValue++;

		//							//								if( triangleMaxHeight >= 0.0f )
		//							//									usedTriangles[ triangleId ] = true;
		//							//							}

		//							//							//for( int n = 0; n < 4; n++ )
		//							//							//{
		//							//							//	HalfType* pair = (HalfType*)( pCellTriangle + n * 4 );

		//							//							//	var triangleMaxHeightH = *( pair + 0 );
		//							//							//	var triangleMaxHeight = (float)triangleMaxHeightH;
		//							//							//	if( triangleMaxHeight >= 0.0f )
		//							//							//	{
		//							//							//		var triangleId = *( pair + 1 );

		//							//							//		usedTriangles[ (int)triangleId ] = true;
		//							//							//	}
		//							//							//}
		//							//						}
		//							//					}
		//							//				}
		//							//			}

		//							//			//!!!!почему-то нулевой треугольник не находится
		//							//			//foreach( var used in usedTriangles )
		//							//			//{
		//							//			//	if( !used )
		//							//			//	{
		//							//			//		error = "There are triangles that are not referenced from the grid.";
		//							//			//		return Array.Empty<ExtractActualGeometryItem>();
		//							//			//	}
		//							//			//}
		//							//		}

		//							//		var item = new ExtractActualGeometryItem();
		//							//		item.Virtualized = true;
		//							//		//item.ClusterIndex = nCluster;
		//							//		//item.TransformPosition = clusterHeader->Position;
		//							//		//item.TransformRotation = clusterHeader->Rotation;
		//							//		item.Vertices = clusterVertices;
		//							//		item.VertexComponents = clusterVertexComponents;
		//							//		item.Indices = clusterIndices;
		//							//		result.Add( item );
		//							//	}
		//							//	else
		//							//	{
		//							//		//separate

		//							//		var vertexCount = clusterHeader->ActualVertexCount;
		//							//		var triangleCount = clusterHeader->ActualTriangleCount;

		//							//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
		//							//		var pVertices = pVirtualizedData + clusterHeader->DataPositionInBytes;
		//							//		var pTriangles = pVertices + clusterHeader->ActualVertexCount * vertexSizeInBytes;

		//							//		var clusterVertices = new StandardVertex[ vertexCount ];
		//							//		var clusterIndices = new int[ triangleCount * 3 ];

		//							//		//write vertices
		//							//		for( int nVertex = 0; nVertex < clusterHeader->ActualVertexCount; nVertex++ )
		//							//		{
		//							//			var pVertex = pVertices + nVertex * vertexSizeInBytes;

		//							//			ref var v = ref clusterVertices[ nVertex ];

		//							//			//16 bytes
		//							//			v.Position.X = *(float*)( pVertex + 0 );
		//							//			v.Position.Y = *(float*)( pVertex + 4 );
		//							//			v.Position.Z = *(float*)( pVertex + 8 );
		//							//			v.TexCoord0.X = *(HalfType*)( pVertex + 12 );
		//							//			v.TexCoord0.Y = *(HalfType*)( pVertex + 14 );

		//							//			//16 bytes
		//							//			v.Normal.X = *(HalfType*)( pVertex + 16 );
		//							//			v.Normal.Y = *(HalfType*)( pVertex + 18 );
		//							//			v.Normal.Z = *(HalfType*)( pVertex + 20 );
		//							//			//one half is not used
		//							//			v.Tangent.X = *(HalfType*)( pVertex + 24 );
		//							//			v.Tangent.Y = *(HalfType*)( pVertex + 26 );
		//							//			v.Tangent.Z = *(HalfType*)( pVertex + 28 );
		//							//			v.Tangent.W = *(HalfType*)( pVertex + 30 );

		//							//			if( fullFormat )
		//							//			{
		//							//				//16 bytes
		//							//				v.TexCoord1.X = *(HalfType*)( pVertex + 32 );
		//							//				v.TexCoord1.Y = *(HalfType*)( pVertex + 34 );
		//							//				v.TexCoord2.X = *(HalfType*)( pVertex + 36 );
		//							//				v.TexCoord2.Y = *(HalfType*)( pVertex + 38 );
		//							//				v.Color.Red = *(HalfType*)( pVertex + 40 );
		//							//				v.Color.Green = *(HalfType*)( pVertex + 42 );
		//							//				v.Color.Blue = *(HalfType*)( pVertex + 44 );
		//							//				v.Color.Alpha = *(HalfType*)( pVertex + 46 );
		//							//			}
		//							//		}

		//							//		//write triangles
		//							//		for( int nTriangle = 0; nTriangle < clusterHeader->ActualTriangleCount; nTriangle++ )
		//							//		{
		//							//			var sourceTrianglePointer = pTriangles + nTriangle * 12;
		//							//			var vertexIndex0 = *(int*)( sourceTrianglePointer + 0 );
		//							//			var vertexIndex1 = *(int*)( sourceTrianglePointer + 4 );
		//							//			var vertexIndex2 = *(int*)( sourceTrianglePointer + 8 );

		//							//			clusterIndices[ nTriangle * 3 + 0 ] = vertexIndex0;
		//							//			clusterIndices[ nTriangle * 3 + 1 ] = vertexIndex1;
		//							//			clusterIndices[ nTriangle * 3 + 2 ] = vertexIndex2;
		//							//		}

		//							//		var item = new ExtractActualGeometryItem();
		//							//		item.Virtualized = true;
		//							//		//item.ClusterIndex = nCluster;
		//							//		//item.ClusterSeparate = true;
		//							//		//item.TransformRotation = QuaternionF.Identity;
		//							//		item.Vertices = clusterVertices;
		//							//		item.VertexComponents = clusterVertexComponents;
		//							//		item.Indices = clusterIndices;
		//							//		result.Add( item );
		//							//	}
		//							//}

		//							//return result.ToArray();
		//						}
		//					}
		//				}
		//			}
		//		}
		//		else
		//		{
		//			//VerticesExtractStandardVertex( out var vertices, out var vertexComponents );
		//			//var indices = Indices.Value;

		//			if( vertices != null && indices != null )
		//			{
		//				if( VerticesExtractStandardVertex( vertices, vertexComponents, out var vertices2, out var vertexComponents2 ) )
		//				{
		//					var item = new ExtractActualGeometryItem();
		//					//item.ClusterIndex = -1;
		//					//item.TransformRotation = QuaternionF.Identity;
		//					item.Vertices = vertices2;
		//					item.VertexComponents = vertexComponents2;
		//					item.Indices = indices;
		//					return new ExtractActualGeometryItem[] { item };
		//				}
		//			}
		//		}
		//	}
		//	//!!!!
		//	//catch( Exception e )
		//	//{
		//	//	error = e.Message;
		//	//}

		//	return Array.Empty<ExtractActualGeometryItem>();
		//}

		public bool ExtractActualGeometry(/*bool checkData, */ out StandardVertex[] resultVertices, out int[] resultIndices/*, out int[] resultMaterialIndexes*/, out string error )
		{
			return ExtractActualGeometry( Vertices, VertexStructure, Indices/*, VirtualizedData*/, out resultVertices, out resultIndices, out error );
		}

		//public ExtractActualGeometryItem[] ExtractActualGeometry( bool checkData, out string error )
		//{
		//	return ExtractActualGeometry( Vertices, VertexStructure, Indices, VirtualizedData, checkData, out error );
		//}

#if !DEPLOY

		internal static unsafe void VerticesWriteChannel<T>( VertexElement element, T[] data, byte[] writeToVertices, int vertexSize, int vertexCount ) where T : unmanaged
		{
			if( VertexElement.GetSizeInBytes( element.Type ) == sizeof( T ) )
			{
				fixed( byte* pVertices = writeToVertices )
				{
					byte* src = pVertices + element.Offset;
					for( int n = 0; n < vertexCount; n++ )
					{
						*(T*)src = data[ n ];
						src += vertexSize;
					}
				}
			}
		}

		public bool CalculateSimplification( bool carefully, int lodIndex, /*MeshSimplificationMethod method, */double simplificationFactor, out byte[] newVertices, out VertexElement[] newVertexStructure, out int[] newIndices, out string error )
		{
			newVertices = null;
			newVertexStructure = null;
			newIndices = null;
			error = "";

			//if( method == MeshSimplificationMethod.Clusters )
			//{
			//	var structure = VertexStructure.Value;
			//	VertexElements.GetInfo( structure, out var vertexSize, out _ );

			//	var simplifier = new ClusterMeshSimplifier();
			//	simplifier.SimplificationFactor = simplificationFactor;
			//	simplifier.SourceVertices = Vertices.Value;
			//	simplifier.SourceVertexStructure = VertexStructure;
			//	simplifier.Indices = Indices;

			//	//!!!!
			//	//float MaxClusterSize;

			//	return simplifier.Calculate( out newVertices, out newVertexStructure, out newIndices, out error );
			//}
			//else
			//{

			var structure = VertexStructure.Value;
			VertexElements.GetInfo( structure, out var vertexSize, out _ );

			var simplifier = new MeshSimplifier();

			foreach( var element in structure )
			{
				switch( element.Semantic )
				{
				case VertexElementSemantic.Position:
					{
						var v = VerticesExtractChannel<Vector3F>( element.Semantic );
						if( v != null )
							simplifier.Vertices = v.Select( v2 => v2.ToVector3() ).ToArray();
					}
					break;

				case VertexElementSemantic.Normal:
					{
						var v = VerticesExtractChannel<Vector3F>( element.Semantic );
						if( v != null )
							simplifier.Normals = v.Select( v2 => v2.ToVector3() ).ToArray();
					}
					break;

				case VertexElementSemantic.Tangent:
					{
						var v = VerticesExtractChannel<Vector4F>( element.Semantic );
						if( v != null )
							simplifier.Tangents = v.Select( v2 => v2.ToVector4() ).ToArray();
					}
					break;

				case VertexElementSemantic.TextureCoordinate0:
				case VertexElementSemantic.TextureCoordinate1:
				case VertexElementSemantic.TextureCoordinate2:
				case VertexElementSemantic.TextureCoordinate3:
				case VertexElementSemantic.TextureCoordinate4:
				case VertexElementSemantic.TextureCoordinate5:
				case VertexElementSemantic.TextureCoordinate6:
				case VertexElementSemantic.TextureCoordinate7:
					{
						var v = VerticesExtractChannel<Vector2F>( element.Semantic );
						if( v != null )
						{
							var channel = element.Semantic - VertexElementSemantic.TextureCoordinate0;
							simplifier.SetUVs( channel, v.Select( v2 => v2.ToVector2() ).ToArray() );
						}
					}
					break;

				case VertexElementSemantic.Color0:
					{
						if( element.Type == VertexElementType.Float4 )
						{
							var v = VerticesExtractChannel<Vector4F>( element.Semantic );
							simplifier.Colors = v.Select( v2 => v2.ToColorValue() ).ToArray();
						}
						else if( element.Type == VertexElementType.ColorABGR )
						{
							//!!!!check
							var v = VerticesExtractChannel<uint>( element.Semantic );
							simplifier.Colors = v.Select( v2 => ColorByte.FromABGR( v2 ).ToColorValue() ).ToArray();
						}
						else if( element.Type == VertexElementType.ColorARGB )
						{
							//!!!!check
							var v = VerticesExtractChannel<uint>( element.Semantic );
							simplifier.Colors = v.Select( v2 => ColorByte.FromARGB( v2 ).ToColorValue() ).ToArray();
						}
					}
					break;

				case VertexElementSemantic.BlendIndices:
					{
						var indices = VerticesExtractChannel<Vector4I>( VertexElementSemantic.BlendIndices );
						var weights = VerticesExtractChannel<Vector4F>( VertexElementSemantic.BlendWeights );
						if( indices != null && weights != null )
						{
							var array = new MeshSimplifier.BoneWeight[ indices.Length ];
							for( int n = 0; n < array.Length; n++ )
								array[ n ] = new MeshSimplifier.BoneWeight( indices[ n ], weights[ n ] );
							simplifier.BoneWeights = array;
						}
					}
					break;
				}
			}

			//this.bindposes = mesh.bindposes;

			var options = MeshSimplifier.SimplificationOptionsStruct.Default;

			//if( carefully )
			//{
			options.PreserveBorderEdges = true;
			options.PreserveUVSeamEdges = true;
			options.PreserveUVFoldoverEdges = true;

			//!!!!slowly very
			//options.PreserveSurfaceCurvature = true;
			//}

			var totalBounds = Bounds.Cleared;
			foreach( var v in simplifier.Vertices )
				totalBounds.Add( v );

			options.EnableSmartLink = true;
			if( carefully )
			{
				options.VertexLinkDistance = totalBounds.GetSize().MaxComponent() * 0.0025;
				options.VertexLinkDistance *= lodIndex * lodIndex;
			}
			else
			{
				options.VertexLinkDistance = totalBounds.GetSize().MaxComponent() * 0.0025;
				options.VertexLinkDistance /= simplificationFactor;
			}

			//options.MaxIterationCount = 200;
			//options.Agressiveness = 14.0;
			simplifier.SimplificationOptions = options;


			simplifier.AddSubMeshTriangles( Indices );

			try
			{
				if( carefully )
				{
					//var threshold = 0.001 * ( lodIndex * lodIndex );
					var threshold = 0.0005 * ( lodIndex * lodIndex );
					//var threshold = 0.0001 * ( lodIndex * lodIndex );
					//var threshold = 0.00001 * ( lodIndex * lodIndex );
					//var threshold = 0.0000001 * lodIndex;
					//var threshold = 0.000000001 * lodIndex;
					simplifier.SimplifyMeshCarefully( threshold );
				}
				else
					simplifier.SimplifyMeshDefault( (float)simplificationFactor );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			var vertexCount = simplifier.Vertices.Length;
			newVertices = new byte[ vertexCount * vertexSize ];

			foreach( var element in structure )
			{
				switch( element.Semantic )
				{
				case VertexElementSemantic.Position:
					if( simplifier.Vertices != null )
					{
						var v = simplifier.Vertices.Select( v2 => v2.ToVector3F() ).ToArray();
						VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
					}
					break;

				case VertexElementSemantic.Normal:
					if( simplifier.Normals != null )
					{
						var v = simplifier.Normals.Select( v2 => v2.ToVector3F() ).ToArray();
						VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
					}
					break;

				case VertexElementSemantic.Tangent:
					if( simplifier.Tangents != null )
					{
						var v = simplifier.Tangents.Select( v2 => v2.ToVector4F() ).ToArray();
						VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
					}
					break;

				case VertexElementSemantic.TextureCoordinate0:
				case VertexElementSemantic.TextureCoordinate1:
				case VertexElementSemantic.TextureCoordinate2:
				case VertexElementSemantic.TextureCoordinate3:
				case VertexElementSemantic.TextureCoordinate4:
				case VertexElementSemantic.TextureCoordinate5:
				case VertexElementSemantic.TextureCoordinate6:
				case VertexElementSemantic.TextureCoordinate7:
					{
						var channel = element.Semantic - VertexElementSemantic.TextureCoordinate0;
						var uv = simplifier.GetUVs2D( channel );
						if( uv != null )
						{
							var v = uv.Select( v2 => v2.ToVector2F() ).ToArray();
							VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
						}
					}
					break;

				case VertexElementSemantic.Color0:
					if( simplifier.Colors != null )
					{
						if( element.Type == VertexElementType.Float4 )
						{
							var v = simplifier.Colors.Select( v2 => v2.ToVector4F() ).ToArray();
							VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
						}
						else if( element.Type == VertexElementType.ColorABGR )
						{
							//!!!!check
							var v = simplifier.Colors.Select( v2 => v2.ToColorPacked().ToABGR() ).ToArray();
							VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
						}
						else if( element.Type == VertexElementType.ColorARGB )
						{
							//!!!!check
							var v = simplifier.Colors.Select( v2 => v2.ToColorPacked().ToARGB() ).ToArray();
							VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
						}
					}
					break;

				case VertexElementSemantic.BlendIndices:
					if( simplifier.BoneWeights != null )
					{
						var v = simplifier.BoneWeights.Select( v2 => v2.Indices ).ToArray();
						VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
					}
					break;

				case VertexElementSemantic.BlendWeights:
					if( simplifier.BoneWeights != null )
					{
						var v = simplifier.BoneWeights.Select( v2 => v2.Weights ).ToArray();
						VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
					}
					break;
				}
			}

			newIndices = simplifier.GetSubMeshTriangles( 0 );

			newVertexStructure = VertexStructure;

			return true;

			//}
		}

#endif
	}
}
