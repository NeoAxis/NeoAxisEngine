// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents mesh geometry: vertices structure and material data.
	/// </summary>
	public class MeshGeometry : Component
	{
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

		//!!!!а может надо Transform таки?
		////!!!!name
		////TransformOfData
		//ReferenceField<Transform> _transformOfData = NeoAxis.Transform.Identity;
		//[Serialize]
		//[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		//public Reference<Transform> TransformOfData
		//{
		//	get
		//	{
		//		if( _transformOfData.BeginGet() )
		//			TransformOfData = _transformOfData.Get( this );
		//		return _transformOfData.value;
		//	}
		//	set
		//	{
		//		//!!!!threading. надо ли. как. где еще так
		//		//!!!!slowly?
		//		//!!!!!!возможно выставлять без проверок. SetTransformNoChecks

		//		if( _transformOfData.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				TransformOfDataChanged?.Invoke( this );
		//				ShouldRecompile = true;
		//			}
		//			finally { _transformOfData.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Mesh> TransformOfDataChanged;
		////protected virtual void OnTransformChanged()
		////{
		////}

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
		/// A data of the baked mesh for the billboard rendering.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Billboard" )]
		public Reference<byte[]> BillboardData
		{
			get { if( _billboardData.BeginGet() ) BillboardData = _billboardData.Get( this ); return _billboardData.value; }
			set { if( _billboardData.BeginSet( ref value ) ) { try { BillboardDataChanged?.Invoke( this ); ShouldRecompileMesh(); } finally { _billboardData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BillboardData"/> property value changes.</summary>
		public event Action<MeshGeometry> BillboardDataChanged;
		ReferenceField<byte[]> _billboardData = null;

		[Category( "Billboard" )]
		public BillboardDataModeEnum BillboardDataMode
		{
			get
			{
				var data = BillboardData.Value;
				if( data != null )
				{
					unsafe
					{
						if( data.Length >= sizeof( BillboardDataHeader ) )
						{
							fixed( byte* pData2 = data )
								return ( (BillboardDataHeader*)pData2 )->Mode;
						}

						//if( data.Length >= sizeof( BillboardDataHeader ) )
						//{
						//	var header = new BillboardDataHeader();
						//	fixed( byte* pData = data )
						//		NativeUtility.CopyMemory( &header, pData, sizeof( BillboardDataHeader ) );
						//	return header.Mode;
						//}
					}
				}
				return BillboardDataModeEnum.None;
			}
		}

		[Category( "Billboard" )]
		public int BillboardDataImageSize
		{
			get
			{
				var data = BillboardData.Value;
				if( data != null )
				{
					unsafe
					{
						if( data.Length >= sizeof( BillboardDataHeader ) )
						{
							fixed( byte* pData2 = data )
								return ( (BillboardDataHeader*)pData2 )->ImageSize;
						}

						//if( data.Length >= sizeof( BillboardDataHeader ) )
						//{
						//	var header = new BillboardDataHeader();
						//	fixed( byte* pData = data )
						//		NativeUtility.CopyMemory( &header, pData, sizeof( BillboardDataHeader ) );
						//	return header.ImageSize;
						//}
					}
				}
				return 0;
			}
		}

		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct BillboardDataHeader
		{
			public int Version;
			public BillboardDataModeEnum Mode;
			public int ImageSize;
			public int Format;
			public int Unused1;
			public int Unused2;
			public int Unused3;
			public int Unused4;

			public int GetImageCount()
			{
				switch( Mode )
				{
				case BillboardDataModeEnum._1Direction: return 1;
				case BillboardDataModeEnum._5Directions: return 5;
				case BillboardDataModeEnum._26Directions: return 26;
				}
				return 0;
			}

			public int GetImageSizeInBytes()
			{
				return ImageSize * ImageSize * 8;
			}
		}

		/////////////////////////////////////////

		public enum BillboardDataModeEnum
		{
			None,

			[DisplayNameEnum( "1 Direction" )]
			[Description( "1 direction. 1 horizontal angle, 1 vertical angle." )]
			_1Direction,

			[DisplayNameEnum( "5 Directions" )]
			[Description( "5 directions. 1 horizontal angle, 5 vertical angles. -90, -45, 0, 45, 90 degrees. Usable for vegetation." )]
			_5Directions,

			[DisplayNameEnum( "26 Directions" )]
			[Description( "26 directions. Includes all directions in 45 degree increments." )]
			_26Directions,
		}

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
				case nameof( BillboardDataMode ):
				case nameof( BillboardDataImageSize ):
					{
						var data = BillboardData.Value;
						if( data == null || data.Length == 0 )
							skip = true;
					}
					break;
				}
			}
		}

		/////////////////////////////////////////

		protected virtual void OnGetDataOfThisObject( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices,
			ref Material material, ref byte[] billboardData, ref Mesh.StructureClass structure )
		{
		}
		public delegate void GetDataOfThisObjectEventDelegate( MeshGeometry sender, ref VertexElement[] vertexStructure, ref byte[] vertices,
			ref int[] indices, ref Material material, ref byte[] billboardData, ref Mesh.StructureClass structure );
		public event GetDataOfThisObjectEventDelegate GetDataOfThisObjectEvent;

		unsafe void CreateBillboardDataImage( byte[] data, out ImageComponent image, out BillboardDataModeEnum mode )
		{
			image = null;
			mode = BillboardDataModeEnum.None;

			if( data.Length < sizeof( BillboardDataHeader ) )
				return;
			var header = new BillboardDataHeader();
			fixed( byte* pData = data )
				NativeUtility.CopyMemory( &header, pData, sizeof( BillboardDataHeader ) );


			if( header.Version == 1 && header.Format == 0 && header.Mode != BillboardDataModeEnum.None )
			{
				var imageCount = header.GetImageCount();
				var imageSizeInBytes = header.GetImageSizeInBytes();
				var totalImagesSizeInBytes = imageCount * imageSizeInBytes;

				if( data.Length == sizeof( BillboardDataHeader ) + totalImagesSizeInBytes && imageCount > 0 )
				{
					image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

					//!!!!
					//bool mipmaps = true;
					bool mipmaps = false;

					image.CreateType = ImageComponent.TypeEnum._2D;
					image.CreateSize = new Vector2I( header.ImageSize, header.ImageSize );
					image.CreateMipmaps = mipmaps;
					image.CreateFormat = PixelFormat.Float16RGBA;
					image.CreateArrayLayers = imageCount;

					var usage = ImageComponent.Usages.WriteOnly;
					if( mipmaps )
						usage |= ImageComponent.Usages.AutoMipmaps;
					image.CreateUsage = usage;

					image.Enabled = true;

					var gpuTexture = image.Result;
					if( gpuTexture != null )
					{
						var surfaceDatas = new GpuTexture.SurfaceData[ imageCount ];

						var current = sizeof( BillboardDataHeader );
						for( int n = 0; n < imageCount; n++ )
						{
							surfaceDatas[ n ] = new GpuTexture.SurfaceData( new ArraySegment<byte>( data, current, imageSizeInBytes ), n );
							current += imageSizeInBytes;
						}

						gpuTexture.SetData( surfaceDatas );
					}

					mode = header.Mode;
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
			byte[] billboardDataV = BillboardData;
			Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref billboardDataV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref billboardDataV, ref structure );

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

					if( billboardDataV != null && billboardDataV.Length != 0 && RenderingSystem.BillboardData )
						CreateBillboardDataImage( billboardDataV, out op.BillboardDataImage, out op.BillboardDataMode );

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
			byte[] billboardDataV = BillboardData;
			Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref billboardDataV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref billboardDataV, ref structure );

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

					if( billboardDataV != null && billboardDataV.Length != 0 && RenderingSystem.BillboardData )
						CreateBillboardDataImage( billboardDataV, out op.BillboardDataImage, out op.BillboardDataMode );

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

		//!!!!!что отсюда взять

		//Bounds bounds;

		//public Bounds Bounds
		//{
		//	get { return bounds; }
		//}

		//protected virtual void OnBoundsUpdate( ref Bounds newBounds ) { }

		//public delegate void BoundsUpdateEventDelegate( Mesh mesh, ref Bounds newBounds );
		//public event BoundsUpdateEventDelegate BoundsUpdateEvent;

		//public event Action<Mesh> BoundsChanged;

		////!!!!вызывать
		//public void BoundsUpdate()
		//{
		//	//!!!!или обновлять при запросе

		//	Bounds oldBounds = bounds;

		//	//calculate
		//	var newBounds = Bounds.Cleared;
		//	OnBoundsUpdate( ref newBounds );
		//	if( newBounds.IsCleared() )
		//	{
		//		//!!!!temp
		//		//!!!!slowly here

		//		xx xx;

		//	}
		//	BoundsUpdateEvent?.Invoke( this, ref newBounds );

		//	bounds = newBounds;
		//	if( oldBounds != bounds )
		//		BoundsChanged?.Invoke( this );

		//	//!!!!что-то где-то обновлять. но когда
		//}

		protected override void OnPreloadResources()
		{
			base.OnPreloadResources();

			//!!!!!!
			//!!!!!получить ссылку одно - а загружать уже другое отдельное?

			//!!!!!!!!по прелодингу: стрктуру строить, но сами ресурсы не загружать сначала?
		}

		public void SetVertexData( StandardVertex[] vertices, StandardVertex.Components vertexComponents )
		{
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

			VertexStructure = structure;
			Vertices = destArray;
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

		public T[] VerticesExtractChannel<T>( VertexElementSemantic semantic ) where T : unmanaged
		{
			var vertexStructure = VertexStructure.Value;
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );

				var vertices = Vertices.Value;
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
						}
					}
				}
			}

			return null;
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
								byte* src = pVertices + element.Offset;
								for( int n = 0; n < vertexCount; n++ )
								{
									*(T*)src = data[ n ];
									src += vertexSize;
								}
							}
						}
					}
					return true;
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

		public bool VerticesExtractStandardVertex( out StandardVertex[] vertices, out StandardVertex.Components components )
		{
			vertices = null;
			components = 0;

			var vertexStructure = VertexStructure.Value;
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );

				var sourceVertices = Vertices.Value;
				if( sourceVertices != null )
				{
					var vertexCount = sourceVertices.Length / vertexSize;

					vertices = new StandardVertex[ vertexCount ];

					//!!!!slowly?

					//Position
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
							element.Type == VertexElementType.Float3 )
						{
							components |= StandardVertex.Components.Position;

							var values = VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Position = values[ n ];
							}
						}
					}

					//Normal
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) &&
							element.Type == VertexElementType.Float3 )
						{
							components |= StandardVertex.Components.Normal;

							var values = VerticesExtractChannel<Vector3F>( VertexElementSemantic.Normal );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Normal = values[ n ];
							}
						}
					}

					//Tangent
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) &&
							element.Type == VertexElementType.Float4 )
						{
							components |= StandardVertex.Components.Tangent;

							var values = VerticesExtractChannel<Vector4F>( VertexElementSemantic.Tangent );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.Tangent = values[ n ];
							}
						}
					}

					//Color
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
						{
							if( element.Type == VertexElementType.Float4 )
							{
								components |= StandardVertex.Components.Color;

								var values = VerticesExtractChannel<ColorValue>( VertexElementSemantic.Color0 );
								for( int n = 0; n < values.Length; n++ )
								{
									ref var vertex = ref vertices[ n ];
									vertex.Color = values[ n ];
								}
							}
							else if( element.Type == VertexElementType.ColorABGR )
							{
								components |= StandardVertex.Components.Color;

								//!!!!check

								var values = VerticesExtractChannel<uint>( VertexElementSemantic.Color0 );
								for( int n = 0; n < values.Length; n++ )
								{
									ref var vertex = ref vertices[ n ];
									vertex.Color = new ColorValue( ColorByte.FromABGR( values[ n ] ) );
								}
							}
							else if( element.Type == VertexElementType.ColorARGB )
							{
								components |= StandardVertex.Components.Color;

								//!!!!check

								var values = VerticesExtractChannel<uint>( VertexElementSemantic.Color0 );
								for( int n = 0; n < values.Length; n++ )
								{
									ref var vertex = ref vertices[ n ];
									vertex.Color = new ColorValue( ColorByte.FromARGB( values[ n ] ) );
								}
							}
						}
					}

					//TexCoord0
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							components |= StandardVertex.Components.TexCoord0;

							var values = VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate0 );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord0 = values[ n ];
							}
						}
					}

					//TexCoord1
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							components |= StandardVertex.Components.TexCoord1;

							var values = VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate1 );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord1 = values[ n ];
							}
						}
					}

					//TexCoord2
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							components |= StandardVertex.Components.TexCoord2;

							var values = VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate2 );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord2 = values[ n ];
							}
						}
					}

					//TexCoord3
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate3, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							components |= StandardVertex.Components.TexCoord3;

							var values = VerticesExtractChannel<Vector2F>( VertexElementSemantic.TextureCoordinate3 );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.TexCoord3 = values[ n ];
							}
						}
					}

					//BlendIndices
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.BlendIndices, out VertexElement element ) &&
							element.Type == VertexElementType.Integer4 )
						{
							components |= StandardVertex.Components.BlendIndices;

							var values = VerticesExtractChannel<Vector4I>( VertexElementSemantic.BlendIndices );
							for( int n = 0; n < values.Length; n++ )
							{
								ref var vertex = ref vertices[ n ];
								vertex.BlendIndices = values[ n ];
							}
						}
					}

					//BlendWeights
					{
						if( vertexStructure.GetElementBySemantic( VertexElementSemantic.BlendWeights, out VertexElement element ) &&
							element.Type == VertexElementType.Float4 )
						{
							components |= StandardVertex.Components.BlendWeights;

							var values = VerticesExtractChannel<Vector4F>( VertexElementSemantic.BlendWeights );
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

	}
}
