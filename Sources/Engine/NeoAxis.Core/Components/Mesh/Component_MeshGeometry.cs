// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents mesh geometry: vertices structure and material data.
	/// </summary>
	public class Component_MeshGeometry : Component
	{
		//!!!!может надо всё же трансформ?
		////!!!!name
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
		//public event Action<Component_MeshGeometry> TransformRelativeToParentChanged;
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
		//public event Action<Component_Mesh> TransformOfDataChanged;
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
		public event Action<Component_MeshGeometry> VertexStructureChanged;
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
		public event Action<Component_MeshGeometry> UnwrappedUVChanged;
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
		public event Action<Component_MeshGeometry> VerticesChanged;
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
		public event Action<Component_MeshGeometry> IndicesChanged;
		ReferenceField<int[]> _indices;

		/// <summary>
		/// The material of a mesh geometry.
		/// </summary>
		[Serialize]
		//[Category( "Data" )]
		[Cloneable( CloneType.Shallow )]
		public Reference<Component_Material> Material
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
		public event Action<Component_MeshGeometry> MaterialChanged;
		ReferenceField<Component_Material> _material;

		/////////////////////////////////////////

		public Component_MeshGeometry()
		{
		}

		/////////////////////////////////////////

		protected virtual void OnGetDataOfThisObject( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices,
			ref Component_Material material, ref Component_Mesh.StructureClass structure )
		{
		}
		public delegate void GetDataOfThisObjectEventDelegate( Component_MeshGeometry sender, ref VertexElement[] vertexStructure, ref byte[] vertices,
			ref int[] indices, ref Component_Material material, ref Component_Mesh.StructureClass structure );
		public event GetDataOfThisObjectEventDelegate GetDataOfThisObjectEvent;

		internal void CompileDataOfThisObject( Component_Mesh.CompiledData compiledData )// Component_Mesh mesh, Component_Mesh.CompiledData result )
		{
			VertexElement[] vertexStructureV = VertexStructure;
			UnwrappedUVEnum unwrappedUVV = UnwrappedUV;
			byte[] verticesV = Vertices;
			int[] indicesV = Indices;
			Component_Material materialV = Material;
			Component_Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref structure );

			//add to result
			if( vertexStructureV != null && vertexStructureV.Length != 0 && verticesV != null && verticesV.Length != 0 && ( indicesV == null || indicesV.Length != 0 ) )
			{
				vertexStructureV.GetInfo( out var vertexSize, out var holes );
				//if( !holes )
				{
					int vertexCount = verticesV.Length / vertexSize;

					var op = new Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation( this );

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

					//!!!!так?
					op.Material = materialV;

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

		internal void CompileDataOfThisObject( out Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation operation )
		{
			operation = null;

			VertexElement[] vertexStructureV = VertexStructure;
			UnwrappedUVEnum unwrappedUVV = UnwrappedUV;
			byte[] verticesV = Vertices;
			int[] indicesV = Indices;
			Component_Material materialV = Material;
			Component_Mesh.StructureClass structure = null;

			OnGetDataOfThisObject( ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref structure );
			GetDataOfThisObjectEvent?.Invoke( this, ref vertexStructureV, ref verticesV, ref indicesV, ref materialV, ref structure );

			//add to result
			if( vertexStructureV != null && vertexStructureV.Length != 0 && verticesV != null && verticesV.Length != 0 && ( indicesV == null || indicesV.Length != 0 ) )
			{
				vertexStructureV.GetInfo( out var vertexSize, out var holes );
				//if( !holes )
				{
					int vertexCount = verticesV.Length / vertexSize;

					var op = new Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation( this );
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

					//!!!!так?
					op.Material = materialV;

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
		//		provider.AddCell( typeof( Component_Mesh_SettingsCell ), true );
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!!что отсюда взять

		//Bounds bounds;

		//public Bounds Bounds
		//{
		//	get { return bounds; }
		//}

		//protected virtual void OnBoundsUpdate( ref Bounds newBounds ) { }

		//public delegate void BoundsUpdateEventDelegate( Component_Mesh mesh, ref Bounds newBounds );
		//public event BoundsUpdateEventDelegate BoundsUpdateEvent;

		//public event Action<Component_Mesh> BoundsChanged;

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

		public void SetVertexDataWithRemovingHoles( StandardVertex[] vertices, StandardVertex.Components vertexComponents )
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
		public Component_Mesh ParentMesh
		{
			get { return FindParent<Component_Mesh>(); }
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

			var mesh = oldParent.FindThisOrParent<Component_Mesh>();
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

	}
}
