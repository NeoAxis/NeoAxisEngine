// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a 3D mesh in the engine. The child mesh geometries defines the mesh data.
	/// </summary>
	[ResourceFileExtension( "mesh" )]
#if !DEPLOY
	[EditorControl( typeof( MeshEditor ) )]
	[Preview( typeof( MeshPreview ) )]
	[PreviewImage( typeof( MeshPreviewImage ) )]
	[SettingsCell( typeof( MeshSettingsCell ) )]
#endif
	public partial class Mesh : ResultCompile<Mesh.CompiledData>
	{
		static ESet<Mesh> all = new ESet<Mesh>();

		//

		/// <summary>
		/// The skeleton of the skinned mesh.
		/// </summary>
		[DefaultValue( null )]
		[Serialize]
		public Reference<Skeleton> Skeleton
		{
			get { if( _skeleton.BeginGet() ) Skeleton = _skeleton.Get( this ); return _skeleton.value; }
			set
			{
				if( _skeleton.BeginSet( ref value ) )
				{
					try
					{
						SkeletonChanged?.Invoke( this );

						//!!!!?
					}
					finally { _skeleton.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Skeleton"/> property value changes.</summary>
		public event Action<Mesh> SkeletonChanged;
		ReferenceField<Skeleton> _skeleton;

		/// <summary>
		/// Whether to cast shadows on the other surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> CastShadows
		{
			get { if( _castShadows.BeginGet() ) CastShadows = _castShadows.Get( this ); return _castShadows.value; }
			set { if( _castShadows.BeginSet( ref value ) ) { try { CastShadowsChanged?.Invoke( this ); } finally { _castShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CastShadows"/> property value changes.</summary>
		public event Action<Mesh> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<Mesh> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the object.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<Mesh> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		/// <summary>
		/// The distance multiplier when determining the level of detail.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale" )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> LODScale
		{
			get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
			set { if( _lODScale.BeginSet( ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		[DisplayName( "LOD Scale Changed" )]
		public event Action<Mesh> LODScaleChanged;
		ReferenceField<double> _lODScale = 1.0;

		/// <summary>
		/// Whether to treat the mesh as a billboard. When drawing the mesh will turn to face the camera.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Billboard
		{
			get { if( _billboard.BeginGet() ) Billboard = _billboard.Get( this ); return _billboard.value; }
			set
			{
				if( _billboard.BeginSet( ref value ) )
				{
					try
					{
						BillboardChanged?.Invoke( this );

						//!!!!
					}
					finally { _billboard.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Billboard"/> property value changes.</summary>
		public event Action<Mesh> BillboardChanged;
		ReferenceField<bool> _billboard = false;

		/// <summary>
		/// The billboard's translation offset from the pivot of the mesh.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> BillboardPositionOffset
		{
			get { if( _billboardPositionOffset.BeginGet() ) BillboardPositionOffset = _billboardPositionOffset.Get( this ); return _billboardPositionOffset.value; }
			set { if( _billboardPositionOffset.BeginSet( ref value ) ) { try { BillboardPositionOffsetChanged?.Invoke( this ); } finally { _billboardPositionOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BillboardPositionOffset"/> property value changes.</summary>
		public event Action<Mesh> BillboardPositionOffsetChanged;
		ReferenceField<Vector3> _billboardPositionOffset = Vector3.Zero;

		/// <summary>
		/// Indent multiplier when rendering shadows to fix overlapping effect of the object with the shadow.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> BillboardShadowOffset
		{
			get { if( _billboardShadowOffset.BeginGet() ) BillboardShadowOffset = _billboardShadowOffset.Get( this ); return _billboardShadowOffset.value; }
			set { if( _billboardShadowOffset.BeginSet( ref value ) ) { try { BillboardShadowOffsetChanged?.Invoke( this ); } finally { _billboardShadowOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BillboardShadowOffset"/> property value changes.</summary>
		public event Action<Mesh> BillboardShadowOffsetChanged;
		ReferenceField<double> _billboardShadowOffset = 1.0;

		///////////////////////////////////////////////

		//!!!!Wireframe, sockets, collision

		/// <summary>
		/// Whether the mesh pivot displayed in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorDisplayPivot { get; set; } = true;

		/// <summary>
		/// Whether to display mesh bounds in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayBounds { get; set; }

		/// <summary>
		/// Whether to display mesh triangles in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayTriangles { get; set; }

		///// <summary>
		///// Whether to display mesh clusters in the editor.
		///// </summary>
		//[Serialize]
		//[Browsable( false )]
		//[DefaultValue( false )]
		//public bool EditorDisplayClusters { get; set; }

		/// <summary>
		/// Whether to display mesh vertices in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayVertices { get; set; }

		/// <summary>
		/// Whether to display mesh normals in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayNormals { get; set; }

		/// <summary>
		/// Whether to display mesh tangents in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayTangents { get; set; }

		/// <summary>
		/// Whether to display mesh binormals in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayBinormals { get; set; }

		/// <summary>
		/// Whether to display mesh vertex colors in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayVertexColor { get; set; }

		/// <summary>
		/// Whether to display mesh uv's in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( -1 )]
		public int EditorDisplayUV { get; set; } = -1;

		/// <summary>
		/// Whether to display the proxy mesh of the virtualized mesh.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayProxyMesh { get; set; }

		/// <summary>
		/// Whether to display number of level of detail in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( -1 )]
		public int EditorDisplayLOD { get; set; } = -1;

		/// <summary>
		/// Whether the mesh collision displayed in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayCollision { get; set; }

		/// <summary>
		/// Whether to the mesh skeleton displayed in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplaySkeleton { get; set; }

		/// <summary>
		/// Specifies the animation name to play in the editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( "" )]
		public string EditorPlayAnimation { get; set; } = "";

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;


		//!!!!add new editor properties to Re-import (Reset Editor Settings)


		[Browsable( false )]
		public bool AllowDisposeBuffers { get; set; } = true;

		///////////////////////////////////////////////

		[Browsable( false )]
		[Cloneable]
		public StructureClass Structure
		{
			get { return _structure; }
			set
			{
				if( _structure == value )
					return;
				_structure = value;
				ShouldRecompile = true;
			}
		}
		StructureClass _structure;

		[DisplayName( "Structure" )]
		public StructureClass StructureInfo
		{
			get { return Structure; }
		}

		///////////////////////////////////////////////

		[Browsable( false )]
		public bool CalculateExtractedDataAndBounds { get; set; } = true;

		//!!!!
		//bool [Use]BoundingConvex;

		//!!!!
		//bool mergeRenderOperations;

		//!!!!impl
		////Include
		////!!!!default value Count = 0 but how to implement
		//ReferenceList<Mesh> _include;
		//[Serialize]
		//public virtual ReferenceList<Mesh> Include
		//{
		//	get { return _include; }
		//}
		//public delegate void IncludeChangedDelegate( Mesh sender );
		//public event IncludeChangedDelegate IncludeChanged;

		///////////////////////////////////////////////

		/// <summary>
		/// Represents precalculated data of <see cref="Mesh"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			Mesh owner;

			MeshDataClass meshData = new MeshDataClass();

			List<IDisposable> objectsToDispose;

			////!!!!
			//bool meshDataDisposeBuffersByCreator = true;

			////!!!!array
			//List<RenderOperationItem> renderOperations = new List<RenderOperationItem>();
			//List<RenderOperation> renderOperations = new List<RenderOperation>();
			//bool renderOperationsDisposeByMesh = true;

			//!!!!можно по идее. пригодится. хотя в памяти висеть же будет
			//Vec3F[] vertexPositions;

			//!!!!
			//!!!!name: compiledBounds, bounds, localBounds
			SpaceBounds spaceBounds;
			//!!!!sphere
			//Bounds bounds;

			//!!!!по идее тут данные для рендера. а у меша могут быть данные для чего-то другого
			//!!!!!!!видать так: в ResultDataClass посчитанные данные. а какие-то еще можно досчитать, если нужно. ну или сразу

			//!!!!генерить для рейкаста, чтобы объекты выделять

			//!!!!как-то это слишком весить будет. инстанситься?
			//!!!!может что-то на уровне render operation или _MeshGeometry?
			//!!!!!!типа такого: int[] extractedVerticesRenderOperationIndex;
			StandardVertex[] extractedVertices;
			StandardVertex.Components extractedVerticesComponents;
			Vector3F[] extractedVerticesPositions;
			int[] extractedIndices;

			object meshTestLockObject = new object();
			MeshTest meshTest;

			RigidBody autoCollisionDefinition;

			////////////

			/// <summary>
			/// Represents geometry data of the mesh for rendering pipeline.
			/// </summary>
			public class MeshDataClass : RenderingPipeline.RenderSceneData.IMeshData
			{
				public object Creator { get; set; }
				public object AnyData { get; set; }
				public List<RenderingPipeline.RenderSceneData.MeshDataRenderOperation> RenderOperations { get; } = new List<RenderingPipeline.RenderSceneData.MeshDataRenderOperation>();
				//public IList<RenderingPipeline.RenderSceneData.MeshDataRenderOperation> RenderOperations { get; } = new List<RenderingPipeline.RenderSceneData.MeshDataRenderOperation>();
				public SpaceBounds SpaceBounds { get; set; }
				public float VisibilityDistanceFactor { get; set; }
				//public float VisibilityDistance { get; set; }
				public float LODScale { get; set; } = 1;
				public bool CastShadows { get; set; }
				public RenderingPipeline.RenderSceneData.IMeshDataLODLevel[] LODs { get; set; }
				public int BillboardMode { get; set; }
				public Vector3F BillboardPositionOffset { get; set; }
				public float BillboardShadowOffset { get; set; }
				public RenderingPipeline.RenderSceneData.LayerItem[] PaintLayers { get; set; }

				public StructureClass Structure { get; set; }

				//

				public bool ContainsDisposedBuffers()
				{
					//!!!!slowly где вызывается?

					for( int n = 0; n < RenderOperations.Count; n++ )
						if( RenderOperations[ n ].ContainsDisposedBuffers() )
							return true;
					return false;
				}

				public void DisposeBuffers()
				{
					foreach( var oper in RenderOperations )
						oper.DisposeBuffers();
				}
			}

			////////////

			public CompiledData( Mesh owner )
			{
				this.owner = owner;
			}

			//public bool Disposed
			//{
			//	get { return disposed; }
			//}

			public virtual void Dispose()
			{
				//!!!!threading: как будет, если из другого потока еще юзаем, но меш обновился

				if( objectsToDispose != null )
				{
					foreach( var obj in objectsToDispose )
						obj.Dispose();
				}

				//if( meshDataDisposeBuffersByCreator )
				if( Owner.AllowDisposeBuffers )
					meshData?.DisposeBuffers();
				meshData = null;

				//foreach( var item in renderOperations )
				//{
				//	var oper = item.operation;

				//	//!!!!так?
				//	if( oper.disposeBuffersByCreator && oper.creator.ParentMesh == owner )
				//	//if( ro.disposeBuffersByMesh )
				//	{
				//		if( oper.vertexBuffers != null )
				//		{
				//			foreach( GpuVertexBuffer b in oper.vertexBuffers )
				//				b.Dispose();
				//		}
				//		if( oper.indexBuffer != null )
				//			oper.indexBuffer.Dispose();
				//	}

				//	//!!!!material?
				//}
				//renderOperations.Clear();

				lock( meshTestLockObject )
				{
					meshTest?.Dispose();
					meshTest = null;
				}

				//if( renderOperationsDisposeByMesh )
				//{
				//	foreach( RenderOperation ro in renderOperations )
				//	{
				//		if( ro.vertexBuffers != null )
				//		{
				//			foreach( GpuVertexBuffer b in ro.vertexBuffers )
				//				b.Dispose();
				//		}
				//		if( ro.indexBuffer != null )
				//			ro.indexBuffer.Dispose();

				//		//!!!!material?
				//	}
				//	renderOperations.Clear();
				//}
			}

			public Mesh Owner
			{
				get { return owner; }
			}

			public MeshDataClass MeshData
			{
				get { return meshData; }
			}

			public List<IDisposable> ObjectsToDispose
			{
				get
				{
					if( objectsToDispose == null )
						objectsToDispose = new List<IDisposable>();
					return objectsToDispose;
				}
			}

			//public List<RenderOperationItem> RenderOperations
			//{
			//	get { return renderOperations; }
			//}

			//public bool RenderOperationsDisposeByMesh
			//{
			//	get { return renderOperationsDisposeByMesh; }
			//	set { renderOperationsDisposeByMesh = value; }
			//}

			//!!!!
			public SpaceBounds SpaceBounds
			{
				get { return spaceBounds; }
				set { spaceBounds = value; }
			}

			internal void ExtractData()
			{
				if( !owner.CalculateExtractedDataAndBounds )
				{
					//extractedVertices = new StandardVertex[ 0 ];
					extractedVerticesComponents = 0;
					extractedVerticesPositions = new Vector3F[ 0 ];
					extractedIndices = new int[ 0 ];
					return;
				}

				//check no data
				foreach( var oper in MeshData.RenderOperations )
				{
					foreach( var buffer in oper.VertexBuffers )
					{
						if( buffer.Vertices == null )
						{
							//extractedVertices = new StandardVertex[ 0 ];
							extractedVerticesComponents = 0;
							extractedVerticesPositions = new Vector3F[ 0 ];
							extractedIndices = new int[ 0 ];
							return;
						}
					}
					if( oper.IndexBuffer != null && oper.IndexBuffer.Indices == null )
					{
						//extractedVertices = new StandardVertex[ 0 ];
						extractedVerticesComponents = 0;
						extractedVerticesPositions = new Vector3F[ 0 ];
						extractedIndices = new int[ 0 ];
						return;
					}
				}


				int totalVertices = 0;
				int totalIndices = 0;
				foreach( var oper in MeshData.RenderOperations )
				{
					totalVertices += oper.VertexCount;
					totalIndices += oper.IndexCount;
				}

				//extractedVertices = new StandardVertex[ totalVertices ];
				extractedVerticesComponents = 0;
				extractedVerticesPositions = new Vector3F[ totalVertices ];
				extractedIndices = new int[ totalIndices ];

				int currentVertex = 0;
				int currentIndex = 0;

				foreach( var oper in MeshData.RenderOperations )
				{
					int startIndex = currentVertex;

					//!!!!
					if( oper.VertexStartOffset != 0 )
						Log.Fatal( "impl. oper.vertexStartOffset != 0" );

					//Position
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
							element.Type == VertexElementType.Float3 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector3F>( element.Offset );

							int destIndex = startIndex;
							foreach( var p in values )
								extractedVerticesPositions[ destIndex++ ] = p;

							extractedVerticesComponents |= StandardVertex.Components.Position;
						}
					}

					//Normal
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) &&
							( element.Type == VertexElementType.Float3 || element.Type == VertexElementType.Half3 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.Normal;
						}
					}

					//Tangent
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) &&
							( element.Type == VertexElementType.Float4 || element.Type == VertexElementType.Half4 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.Tangent;
						}
					}

					//Color
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
						{
							if( element.Type == VertexElementType.Float4 || element.Type == VertexElementType.Half4 || element.Type == VertexElementType.ColorABGR || element.Type == VertexElementType.ColorARGB )
							{
								extractedVerticesComponents |= StandardVertex.Components.Color;
							}
						}
					}

					//TexCoord0
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out VertexElement element ) &&
							( element.Type == VertexElementType.Float2 || element.Type == VertexElementType.Half2 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.TexCoord0;
						}
					}

					//TexCoord1
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out VertexElement element ) &&
							( element.Type == VertexElementType.Float2 || element.Type == VertexElementType.Half2 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.TexCoord1;
						}
					}

					//TexCoord2
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out VertexElement element ) &&
							( element.Type == VertexElementType.Float2 || element.Type == VertexElementType.Half2 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.TexCoord2;
						}
					}

					//TexCoord3
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate3, out VertexElement element ) &&
							( element.Type == VertexElementType.Float2 || element.Type == VertexElementType.Half2 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.TexCoord3;
						}
					}

					//BlendIndices
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.BlendIndices, out VertexElement element ) &&
							element.Type == VertexElementType.Integer4 )
						{
							extractedVerticesComponents |= StandardVertex.Components.BlendIndices;
						}
					}

					//BlendWeights
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.BlendWeights, out VertexElement element ) &&
							( element.Type == VertexElementType.Float4 || element.Type == VertexElementType.Half4 ) )
						{
							extractedVerticesComponents |= StandardVertex.Components.BlendWeights;
						}
					}

					currentVertex += oper.VertexCount;

					//indices
					{
						//!!!!если нет, то генерить?
						var indices2 = oper.IndexBuffer.Indices;
						foreach( var index in indices2 )
							extractedIndices[ currentIndex++ ] = startIndex + index;
					}
				}

				//for( int n = 0; n < ExtractedVerticesPositions.Length; n++ )
				//	extractedVerticesPositions[ n ] = extractedVertices[ n ].Position;

				if( currentVertex != totalVertices )
					Log.Fatal( "Mesh.CompiledData: ExtractData: currentVertex != totalVertices." );
				if( currentIndex != totalIndices )
					Log.Fatal( "Mesh.CompiledData: ExtractData: currentIndex != totalIndices." );
			}

			void PrepareExtractedVertices()
			{
				if( !owner.CalculateExtractedDataAndBounds )
				{
					extractedVertices = new StandardVertex[ 0 ];
					return;
				}

				//check no data
				foreach( var oper in MeshData.RenderOperations )
				{
					foreach( var buffer in oper.VertexBuffers )
					{
						if( buffer.Vertices == null )
						{
							extractedVertices = new StandardVertex[ 0 ];
							return;
						}
					}
					if( oper.IndexBuffer != null && oper.IndexBuffer.Indices == null )
					{
						extractedVertices = new StandardVertex[ 0 ];
						return;
					}
				}


				int totalVertices = 0;
				foreach( var oper in MeshData.RenderOperations )
					totalVertices += oper.VertexCount;

				extractedVertices = new StandardVertex[ totalVertices ];

				int currentVertex = 0;

				foreach( var oper in MeshData.RenderOperations )
				{
					int startIndex = currentVertex;

					//!!!!
					if( oper.VertexStartOffset != 0 )
						Log.Fatal( "impl. oper.vertexStartOffset != 0" );

					//!!!!slowly

					//Position
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) &&
							element.Type == VertexElementType.Float3 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector3F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].Position = p;
						}
					}

					//Normal
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float3 )
							{
								var values = buffer.ExtractChannel<Vector3F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Normal = p;
							}
							else if( element.Type == VertexElementType.Half3 )
							{
								var values = buffer.ExtractChannel<Vector3H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Normal = p;
							}
						}
					}

					//Tangent
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float4 )
							{
								var values = buffer.ExtractChannel<Vector4F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Tangent = p;
							}
							else if( element.Type == VertexElementType.Half4 )
							{
								var values = buffer.ExtractChannel<Vector4H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Tangent = p;
							}
						}
					}

					//Color
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float4 )
							{
								var values = buffer.ExtractChannel<Vector4F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = p.ToColorValue();
							}
							else if( element.Type == VertexElementType.Half4 )
							{
								var values = buffer.ExtractChannel<Vector4H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = p.ToVector4F().ToColorValue();
							}
							else if( element.Type == VertexElementType.ColorABGR )
							{
								//!!!!check

								var values = buffer.ExtractChannel<uint>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = new ColorValue( ColorByte.FromABGR( p ) );
							}
							else if( element.Type == VertexElementType.ColorARGB )
							{
								//!!!!check

								var values = buffer.ExtractChannel<uint>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = new ColorValue( ColorByte.FromARGB( p ) );
							}
						}
					}

					//TexCoord0
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float2 )
							{
								var values = buffer.ExtractChannel<Vector2F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord0 = p;
							}
							else if( element.Type == VertexElementType.Half2 )
							{
								var values = buffer.ExtractChannel<Vector2H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord0 = p;
							}
						}
					}

					//TexCoord1
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float2 )
							{
								var values = buffer.ExtractChannel<Vector2F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord1 = p;
							}
							else if( element.Type == VertexElementType.Half2 )
							{
								var values = buffer.ExtractChannel<Vector2H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord1 = p;
							}
						}
					}

					//TexCoord2
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float2 )
							{
								var values = buffer.ExtractChannel<Vector2F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord2 = p;
							}
							else if( element.Type == VertexElementType.Half2 )
							{
								var values = buffer.ExtractChannel<Vector2H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord2 = p;
							}
						}
					}

					//TexCoord3
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate3, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float2 )
							{
								var values = buffer.ExtractChannel<Vector2F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord3 = p;
							}
							else if( element.Type == VertexElementType.Half2 )
							{
								var values = buffer.ExtractChannel<Vector2H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].TexCoord3 = p;
							}
						}
					}

					//BlendIndices
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.BlendIndices, out VertexElement element ) &&
							element.Type == VertexElementType.Integer4 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector4I>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].BlendIndices = p;
						}
					}

					//BlendWeights
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.BlendWeights, out VertexElement element ) )
						{
							var buffer = oper.VertexBuffers[ element.Source ];

							if( element.Type == VertexElementType.Float4 )
							{
								var values = buffer.ExtractChannel<Vector4F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].BlendWeights = p;
							}
							else if( element.Type == VertexElementType.Half4 )
							{
								var values = buffer.ExtractChannel<Vector4H>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].BlendWeights = p;
							}
						}
					}

					currentVertex += oper.VertexCount;
				}

				if( currentVertex != totalVertices )
					Log.Fatal( "Mesh.CompiledData: PrepareExtractedVertices: currentVertex != totalVertices." );
			}

			public StandardVertex[] GetExtractedVertices( bool canPrepare )
			{
				if( extractedVertices == null && canPrepare )
					PrepareExtractedVertices();
				return extractedVertices;
			}

			public StandardVertex.Components ExtractedVerticesComponents
			{
				get { return extractedVerticesComponents; }
			}

			public Vector3F[] ExtractedVerticesPositions
			{
				get { return extractedVerticesPositions; }
			}

			public int[] ExtractedIndices
			{
				get { return extractedIndices; }
			}

			[Flags]
			public enum RayCastModes
			{
				None = 0,
				Optimized = 1,
				//VirtualizedData = 2,
				Auto = Optimized// | VirtualizedData,
			}

			//public enum RayCastMode
			//{
			//	Auto,
			//	BruteforceNoCache,
			//	OctreeOptimizedCached
			//}

			//!!!!struct?
			public class RayCastResult
			{
				public double Scale;
				public Vector3F Normal;

				//!!!!public int MaterialIndex;

				//for usual mesh
				public bool ContainsTriangleInfo;
				public int TriangleIndex;

				//for cluster data
				public bool ContainsVertexInfo;
				public StandardVertex Vertex0;
				public StandardVertex Vertex1;
				public StandardVertex Vertex2;
			}

			public RayCastResult RayCast( Ray ray, RayCastModes mode, bool twoSided )
			//public bool RayCast( Ray ray, RayCastModes mode, bool twoSided, out double scale, out Vector3F normal, out int triangleIndex )
			{
				//scale = 0;
				//normal = Vector3F.Zero;
				//triangleIndex = -1;

				if( !SpaceBounds.BoundingBox.Intersects( ray ) )
					return null;// false;
				if( !SpaceBounds.BoundingSphere.Intersects( ray ) )
					return null;// false;

				//!!!!support when mixed virtualized and usual meshes. нужно пропускать некластерные

				//if( ( mode & RayCastModes.VirtualizedData ) != 0 && ContainsVirtualizedData() )
				//{
				//	RayCastResult bestResult = null;

				//	for( int n = 0; n < MeshData.RenderOperations.Count; n++ )
				//	{
				//		var op = MeshData.RenderOperations[ n ];

				//		var result = op.VirtualizedRayCast( ray, mode, twoSided );
				//		if( result != null )
				//		{
				//			if( bestResult == null || result.Scale < bestResult.Scale )
				//				bestResult = result;
				//		}
				//	}

				//	return bestResult;
				//}

				////var useOptimized = ( mode & RayCastModes.Optimized ) != 0;

				////if( useOptimized )
				////{
				////	int minTriangles = 16;

				////	if( ExtractedIndices.Length < minTriangles * 3 )
				////		useOptimized = false;

				////	//if( ExtractedIndices.Length > minTriangles * 3 )
				////	//	mode = RayCastMode.OctreeOptimizedCached;
				////	//else
				////	//	mode = RayCastMode.BruteforceNoCache;
				////}

				if( ExtractedIndices != null && ExtractedVerticesPositions != null )
				{
					RayF rayF = ray.ToRayF();

					int minTriangles = 16;

					if( ( mode & RayCastModes.Optimized ) != 0 && ExtractedIndices.Length > minTriangles * 3 )
					//if( useOptimized )//if( mode == RayCastMode.OctreeOptimizedCached )
					{
						//octree optimized cached

						lock( meshTestLockObject )
						{
							if( meshTest == null )
								meshTest = new MeshTest( ExtractedVerticesPositions, ExtractedIndices );
						}

						//!!!!GC
						var result = meshTest.RayCast( rayF, MeshTest.Mode.OneClosest, twoSided );

						if( result.Length > 0 )
						{
							var item = result[ 0 ];

							var r = new RayCastResult();
							r.Scale = item.Scale;
							r.Normal = item.Normal;
							r.ContainsTriangleInfo = true;
							r.TriangleIndex = item.TriangleIndex;
							return r;
						}

						return null;

						//if( result.Length > 0 )
						//{
						//	var item = result[ 0 ];
						//	scale = item.Scale;
						//	normal = item.Normal;
						//	triangleIndex = item.TriangleIndex;
						//	return true;
						//}

						//return false;
					}
					else
					{
						//Brute-force no cache

						var vertices = ExtractedVerticesPositions;
						var indices = ExtractedIndices;

						RayCastResult found = null;
						//bool found = false;

						//process data
						for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
						{
							ref Vector3F vertex0 = ref vertices[ indices[ nTriangle * 3 + 0 ] ];
							ref Vector3F vertex1 = ref vertices[ indices[ nTriangle * 3 + 1 ] ];
							ref Vector3F vertex2 = ref vertices[ indices[ nTriangle * 3 + 2 ] ];

							var bounds = new BoundsF( vertex0 );
							bounds.Add( ref vertex1 );
							bounds.Add( ref vertex2 );

							if( bounds.Intersects( ref rayF ) )
							{
								//!!!!надо ли два раз искать IntersectTriangleRay?
								var found2 = MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref rayF, out float localScale );
								var normal2 = Vector3F.Zero;
								if( found2 )
									MathAlgorithms.CalculateTriangleNormal( ref vertex0, ref vertex1, ref vertex2, out normal2 );
								else
								{
									found2 = twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref rayF, out localScale );
									if( found2 )
										MathAlgorithms.CalculateTriangleNormal( ref vertex0, ref vertex2, ref vertex1, out normal2 );
								}

								////if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref rayF, out var localScale ) ||
								////twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref rayF, out localScale ) )
								if( found2 )
								{
									if( found == null || localScale < found.Scale )
									{
										found = new RayCastResult();
										found.Scale = localScale;
										found.Normal = normal2;
										found.ContainsTriangleInfo = true;
										found.TriangleIndex = nTriangle;
									}

									//if( !found || localScale < scale )
									//{
									//	found = true;
									//	scale = localScale;
									//	normal = normal2;
									//	triangleIndex = nTriangle;
									//}

									////Vec3 localPoint = localRay.GetPointOnRay( localScale );
									////Vec3 worldPoint = transform * localPoint;

									////float worldScale;
									////if( rayLength != 0 )
									////	worldScale = ( worldPoint - ray.Origin ).Length() / rayLength;
									////else
									////	worldScale = 0;

									////if( list.Count == 0 )
									////	list.Add( new Result( worldScale, nSubMesh ) );
									////else
									////{
									////	if( piercing )
									////		list.Add( new Result( worldScale, nSubMesh ) );
									////	else
									////	{
									////		if( worldScale < list[ 0 ].rayScale )
									////			list[ 0 ] = new Result( worldScale, nSubMesh );
									////	}
									////}
								}
							}
						}

						return found;
					}
				}

				return null;
			}

			//!!!!
			internal bool _IntersectsFast( Plane[] planes, ref Bounds bounds )
			{
				if( !SpaceBounds.BoundingBox.Intersects( ref bounds ) )
					return false;
				if( !SpaceBounds.BoundingSphere.Intersects( ref bounds ) )
					return false;

				//if( ExtractedIndices.Length < 2048 )
				//{

				lock( meshTestLockObject )
				{
					if( meshTest == null )
						meshTest = new MeshTest( ExtractedVerticesPositions, ExtractedIndices );
				}
				return meshTest.IntersectsFast( planes, ref bounds );

				//}
				//else
				//{
				//	foreach( var plane in planes )
				//	{
				//		if( plane.GetSide( SpaceBounds.CalculatedBoundingBox ) == Plane.Side.Positive )
				//			return false;

				//		//!!!!sphere
				//	}

				//	return true;
				//}
			}

			//!!!!
			internal int[] _GetIntersectedTrianglesFast(/* Plane[] planes, */ref Bounds bounds )
			{
				if( !SpaceBounds.BoundingBox.Intersects( ref bounds ) )
					return Array.Empty<int>();
				if( !SpaceBounds.BoundingSphere.Intersects( ref bounds ) )
					return Array.Empty<int>();

				lock( meshTestLockObject )
				{
					if( meshTest == null )
						meshTest = new MeshTest( ExtractedVerticesPositions, ExtractedIndices );
				}
				return meshTest.GetIntersectedTrianglesFast( /*planes, */ref bounds );
			}

			//!!!!!

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

			public CompiledData GetLOD( int level )
			{
				if( level > 0 )
				{
					var data = MeshData.LODs[ level - 1 ].Mesh?.Result;
					if( data != null )
						return data;
				}
				return this;
			}

			//public bool ContainsVirtualizedData()
			//{
			//	for( int n = 0; n < MeshData.RenderOperations.Count; n++ )
			//	{
			//		var op = MeshData.RenderOperations[ n ];
			//		if( op.VirtualizedData != null )
			//			return true;
			//	}
			//	return false;
			//}

			static Mesh GetLowestNotVoxelLod( Mesh.CompiledData meshResult )
			{
				var lods = meshResult.MeshData?.LODs;
				if( lods != null )
				{
					for( int n = lods.Length - 1; n >= 0; n-- )
					{
						var lod = lods[ n ];

						var lodMesh = lod.Mesh;
						if( lodMesh != null )
						{
							var isVoxel = false;

							foreach( var geometry in lodMesh.GetComponents<MeshGeometry>() )
							{
								if( geometry.VoxelData.Value != null )
								{
									isVoxel = true;
									break;
								}
							}

							if( !isVoxel )
								return lodMesh;
						}
					}
				}

				return meshResult.Owner;
			}

			public RigidBody GetAutoCollisionDefinition()
			{
				if( autoCollisionDefinition == null )
				{
					var body = new RigidBody();
					var collisionShape = body.CreateComponent<CollisionShape_Mesh>();
					collisionShape.Mesh = GetLowestNotVoxelLod( this );
					autoCollisionDefinition = body;
				}
				return autoCollisionDefinition;
			}
		}

		/////////////////////////////////////////

		public class ExtractedData
		{
			public StructureClass Structure;
			public MeshGeometryItem[] MeshGeometries;

			public class MeshGeometryItem
			{
				public VertexElement[] VertexStructure;
				public byte[] Vertices;
				public int[] Indices;
				public Reference<Material> Material;
			}
		}

		/////////////////////////////////////////

		public static Mesh[] GetAll()
		{
			lock( all )
				return all.ToArray();
		}

		public Mesh()
		{
			lock( all )
				all.Add( this );

			//_include = new ReferenceList<Mesh>( this, delegate ()
			//{
			//	IncludeChanged?.Invoke( this );
			//} );
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			lock( all )
				all.Remove( this );
		}

		/////////////////////////////////////////

		protected override void OnResultCompile()
		{
			if( Result == null )
			{
				var compiledData = new CompiledData( this );
				compiledData.MeshData.Creator = this;
				compiledData.MeshData.Structure = Structure;

				OnMeshCompile( compiledData );
				MeshCompileEvent?.Invoke( this, compiledData );

				var modifiers = GetComponents<MeshModifier>();
				foreach( var modifier in modifiers )
				{
					if( modifier.Enabled )
						modifier.ApplyToMeshData( compiledData );
				}

				OnMeshCompilePostProcess( compiledData );
				MeshCompilePostProcessEvent?.Invoke( this, compiledData );

				compiledData.ExtractData();

				//Space bounds
				var bounds = Bounds.Cleared;
				var sphere = Sphere.Cleared;
				//!!!!так?
				//!!!!как переопределять
				if( compiledData.ExtractedVerticesPositions.Length != 0 )
				{
					if( Billboard )
					{
						double maxLength = 0;
						foreach( var p in compiledData.ExtractedVerticesPositions )
						{
							var length = p.Length();
							if( length > maxLength )
								maxLength = length;
						}
						bounds = new Bounds( -maxLength, -maxLength, -maxLength, maxLength, maxLength, maxLength );
						sphere = new Sphere( Vector3.Zero, maxLength );
					}
					else
					{
						var positions = compiledData.ExtractedVerticesPositions;
						foreach( var p in positions )
							bounds.Add( p );

						if( positions.Length != 0 )
						{
							sphere = MathAlgorithms.BoundingSphereFromPoints( positions ).ToSphere();
							//Vec3[] positions2 = new Vec3[ positions.Length ];
							//for( int n = 0; n < positions.Length; n++ )
							//	positions2[ n ] = positions[ n ].ToVec3();
							//sphere = MathAlgorithms.BoundingSphereFromPoints( positions2 );
						}
					}
				}
				else
				{
					bounds = new Bounds( -MathEx.Epsilon, -MathEx.Epsilon, -MathEx.Epsilon, MathEx.Epsilon, MathEx.Epsilon, MathEx.Epsilon );
					sphere = new Sphere( Vector3.Zero, MathEx.Epsilon );
				}

				if( bounds.IsCleared() )
					bounds = Bounds.Zero;
				if( sphere.IsCleared() )
					sphere = Sphere.Zero;

				////expand zero sizes
				//{
				//	var size = bounds.GetSize();
				//	for( int n = 0; n < 3; n++ )
				//	{
				//		if( size[ n ] <= 0.001 )
				//		{
				//			bounds.Minimum[ n ] -= 0.001;
				//			bounds.Maximum[ n ] += 0.001;
				//		}
				//	}
				//	if( sphere.Radius < 0.001 )
				//		sphere.Radius = 0.001;
				//}

				compiledData.SpaceBounds = new SpaceBounds( bounds, sphere );

				compiledData.MeshData.SpaceBounds = compiledData.SpaceBounds;
				//compiledData.MeshData.BoundingBox = bounds;
				//compiledData.MeshData.BoundingSphere = sphere;

				compiledData.MeshData.VisibilityDistanceFactor = (float)VisibilityDistanceFactor;
				compiledData.MeshData.LODScale = (float)LODScale;
				compiledData.MeshData.CastShadows = CastShadows;

				//!!!!тут?
				//LODs
				if( compiledData.MeshData.LODs == null )
				{
					var components = GetComponents<MeshLevelOfDetail>();

					int enabledCount = 0;
					foreach( var lod in components )
					{
						if( lod.Enabled )
							enabledCount++;
					}

					if( enabledCount != 0 )
					{
						var lods = new RenderingPipeline.RenderSceneData.IMeshDataLODLevel[ enabledCount ];
						int current = 0;
						foreach( var lod in components )
						{
							if( lod.Enabled )
							{
								var item = new RenderingPipeline.RenderSceneData.IMeshDataLODLevel();
								item.Mesh = lod.Mesh;
								item.Distance = (float)lod.Distance;
								//item.DistanceSquared = (float)( lod.Distance * lod.Distance );

								if( item.Mesh != null )
								{
									foreach( var geometry in item.Mesh.GetComponents<MeshGeometry>() )
									{
										var size = geometry.VoxelGridSize.MaxComponent();
										if( size != 0 )
										{
											item.VoxelGridSize = size;
											break;
										}
									}
								}

								lods[ current ] = item;
								current++;
							}
						}

						//sort by distance
						CollectionUtility.InsertionSort( lods, delegate ( RenderingPipeline.RenderSceneData.IMeshDataLODLevel l1, RenderingPipeline.RenderSceneData.IMeshDataLODLevel l2 )
						 {
							 if( l1.Distance < l2.Distance )
								 return -1;
							 if( l1.Distance > l2.Distance )
								 return 1;
							 //if( l1.DistanceSquared < l2.DistanceSquared )
							 // return -1;
							 //if( l1.DistanceSquared > l2.DistanceSquared )
							 // return 1;
							 return 0;
						 } );

						compiledData.MeshData.LODs = lods;
					}
				}

				//Billboard data
				if( Billboard )
				{
					var positions = compiledData.ExtractedVerticesPositions;
					var indices = compiledData.ExtractedIndices;
					if( indices.Length >= 3 )
					{
						var p0 = positions[ indices[ 0 ] ];
						var p1 = positions[ indices[ 1 ] ];
						var p2 = positions[ indices[ 2 ] ];
						var normal = Plane.FromPoints( p0, p1, p2 ).Normal;

						if( normal.Y < -.9f )
							compiledData.MeshData.BillboardMode = 1;// -MathEx.PI / 2.0f + MathEx.PI * 2.0f;
						else if( normal.Y > .9f )
							compiledData.MeshData.BillboardMode = 2;// MathEx.PI / 2.0f;
						else if( normal.X > .9f )
							compiledData.MeshData.BillboardMode = 3;// MathEx.PI / 2.0f - MathEx.PI * 2.0f;
						else
							compiledData.MeshData.BillboardMode = 4;// -MathEx.PI / 2.0f;
					}
					else
						compiledData.MeshData.BillboardMode = 1;// -MathEx.PI / 2.0f + MathEx.PI * 2.0f;

					compiledData.MeshData.BillboardPositionOffset = BillboardPositionOffset.Value.ToVector3F();
					compiledData.MeshData.BillboardShadowOffset = (float)BillboardShadowOffset.Value;
				}

				//Paint layers
				if( compiledData.MeshData.PaintLayers == null )
				{
					var items = new List<RenderingPipeline.RenderSceneData.LayerItem>();
					foreach( var layer in GetComponents<PaintLayer>() )
					{
						if( layer.Enabled )
						{
							var mask = layer.GetMaskImage( out var uniqueMaskDataCounter );
							if( mask != null )
								items.Add( new RenderingPipeline.RenderSceneData.LayerItem( layer, mask, uniqueMaskDataCounter, true ) );
						}
					}
					if( items.Count != 0 )
						compiledData.MeshData.PaintLayers = items.ToArray();
				}

				Result = compiledData;
			}
		}

		protected virtual void OnMeshCompile( CompiledData compiledData )
		{
			//var tr = TransformRelativeToParent;

			//!!!!check children? везде проверить
			foreach( var geometry in GetComponents<MeshGeometry>() )// false, true, false ) )
			{
				if( geometry.Enabled )
					geometry.CompileDataOfThisObject( compiledData );

				//var childMeshData = geometry.Result;
				//if( childMeshData != null )
				//{
				//	foreach( var sourceOperationItem in childMeshData.RenderOperations )
				//	{
				//		var item = new CompiledData.RenderOperationItem();
				//		item.operation = sourceOperationItem.operation;

				//		xx xx;
				//		//item.transform = TransformOfData.Value * childMesh.TransformRelativeToParent.Value * sourceOperationItem.transform;
				//		////item.transform = tr * sourceOperationItem.transform;

				//		compiledData.RenderOperations.Add( item );
				//	}

				//	//compiledData.RenderOperations.AddRange( data.RenderOperations );

				//	//foreach( var op in data.RenderOperations )
				//	//{
				//	//	var newOp = op.Clone();
				//	//	newOp.transform = tr * newOp.transform;
				//	//	compiledData.RenderOperations.Add( newOp );
				//	//}
				//}

				//!!!!c.VisibleInHierarchy
				//!!!!так?
				//!!!!!может от режима обновления зависит
				//c.ResultCompile( this, result );
			}

			//foreach( var listItem in Include )
			//{
			//	Mesh c = listItem.GetValue( this );

			//	//!!!!EnabledInHierarchy?
			//	if( c != null && c.Enabled )// && c.EnabledInHierarchy )
			//	{
			//		var data = c.Result;
			//		if( data != null )
			//		{
			//			foreach( var sourceOperationItem in data.RenderOperations )
			//			{
			//				var item = new CompiledData.RenderOperationItem();
			//				item.operation = sourceOperationItem.operation;

			//				item.transform = TransformOfData.Value * sourceOperationItem.transform;
			//				//item.transform = tr * sourceItem.transform;

			//				compiledData.RenderOperations.Add( item );
			//			}

			//			//compiledData.RenderOperations.AddRange( data.RenderOperations );

			//			//foreach( var op in data.RenderOperations )
			//			//{
			//			//	var newOp = op.Clone();
			//			//	newOp.transform = tr * newOp.transform;
			//			//	compiledData.RenderOperations.Add( newOp );
			//			//}
			//		}
			//	}
			//}

			//CompileDataOfThisObject( compiledData );
		}

		public delegate void MeshCompileEventDelegate( Mesh sender, CompiledData compiledData );
		public event MeshCompileEventDelegate MeshCompileEvent;


		protected virtual void OnMeshCompilePostProcess( CompiledData compiledData )
		{
		}
		public delegate void MeshCompilePostProcessEventDelegate( Mesh sender, CompiledData compiledData );
		public event MeshCompilePostProcessEventDelegate MeshCompilePostProcessEvent;

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

			//!!!!где еще. партиклы
		}

		class MergeGroup
		{
			public List<MeshGeometry> geometries = new List<MeshGeometry>();
		}

		public void MergeGeometriesWithEqualVertexStructureAndMaterial()
		{
			//!!!!соединять ли совпадающие вертексы из разных геометрий. может параметром bool mergeEqualVertices. или отдельным методом

			//get groups
			var groups = new List<MergeGroup>();
			foreach( var geometry in GetComponents<MeshGeometry>() )// false, true ) )
			{
				var group = groups.Find( delegate ( MergeGroup g )
				{
					if( g.geometries[ 0 ].Material.Value == geometry.Material.Value )
					{
						var structure1 = g.geometries[ 0 ].VertexStructure.Value;
						var structure2 = geometry.VertexStructure.Value;
						if( structure1 != null && structure2 != null && VertexElements.Equals( structure1, structure2 ) )
							return true;
					}
					return false;
				} );

				if( group == null )
				{
					group = new MergeGroup();
					groups.Add( group );
				}
				group.geometries.Add( geometry );
			}

			//update
			bool updated = groups.Any( g => g.geometries.Count != 1 );
			if( updated )
			{
				bool restoreEnabled = Enabled;
				try
				{
					if( restoreEnabled )
						Enabled = false;

					foreach( var group in groups )
					{
						if( group.geometries.Count != 1 )
						{
							var geometry0 = group.geometries[ 0 ];
							geometry0.VertexStructure.Value.GetInfo( out var vertexSize, out _ );

							int totalVertices = 0;
							int totalIndices = 0;
							foreach( var g in group.geometries )
							{
								totalVertices += g.Vertices.Value.Length / vertexSize;
								totalIndices += g.Indices.Value.Length;
							}

							var newVertices = new byte[ totalVertices * vertexSize ];
							var newIndices = new int[ totalIndices ];

							int currentVertex = 0;
							int currentIndex = 0;
							foreach( var g in group.geometries )
							{
								g.Vertices.Value.CopyTo( newVertices, currentVertex * vertexSize );

								foreach( var index in g.Indices.Value )
									newIndices[ currentIndex++ ] = index + currentVertex;

								currentVertex += g.Vertices.Value.Length / vertexSize;
							}

							if( !string.IsNullOrEmpty( geometry0.Name ) )
								geometry0.Name += $" + {group.geometries.Count - 1} merged";
							geometry0.Vertices = newVertices;
							geometry0.Indices = newIndices;

							for( int n = 1; n < group.geometries.Count; n++ )
							{
								group.geometries[ n ].RemoveFromParent( false );
								//!!!!
								group.geometries[ n ].Dispose();
							}
						}
					}

				}
				finally
				{
					if( restoreEnabled )
						Enabled = true;
				}
			}
		}

		public ExtractedData ExtractData()
		{
			var result = new ExtractedData();
			var resultMeshGeometries = new List<ExtractedData.MeshGeometryItem>();

			//!!!!проверять валидность? out string error

			result.Structure = (StructureClass)Structure?.Clone();
			if( result.Structure == null )
				result.Structure = BuildStructure( this );

			var meshGeometries = GetComponents<MeshGeometry>();

			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				var meshGeometry = meshGeometries[ i ];

				var item = new ExtractedData.MeshGeometryItem();

				if( meshGeometry is MeshGeometry_Procedural meshGeometryProcedural )
				{
					Material material = null;
					byte[] voxelData = null;
					byte[] clusterData = null;
					StructureClass structure = null;
					meshGeometryProcedural.GetProceduralGeneratedData( ref item.VertexStructure, ref item.Vertices, ref item.Indices, ref material, ref voxelData, ref clusterData, ref structure );
					result.Structure = StructureClass.Concat( result.Structure, structure, i );

					item.Material = meshGeometryProcedural.Material;
				}
				else
				{
					item.VertexStructure = meshGeometry.VertexStructure;
					item.Vertices = meshGeometry.Vertices;
					item.Indices = meshGeometry.Indices;
					item.Material = meshGeometry.Material;
				}

				resultMeshGeometries.Add( item );
			}

			result.MeshGeometries = resultMeshGeometries.ToArray();
			return result;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			var childBlock = block.FindChild( "Structure" );
			if( childBlock != null )
			{
				var structure = new StructureClass();
				if( !structure.Load( context, childBlock, out error ) )
					return false;
				Structure = structure;
			}

			return true;
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			if( Structure != null )
			{
				var childBlock = block.AddChild( "Structure" );
				if( !Structure.Save( context, childBlock, out error ) )
					return false;
			}

			return true;
		}

		//public bool ExportToFBX( string realFileName, out string error )
		//{
		//	return MeshExportImport.ExportToFBX( this, realFileName, out error );
		//}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( BillboardPositionOffset ):
				case nameof( BillboardShadowOffset ):
					if( !Billboard || !CastShadows )
						skip = true;
					break;
				}
			}
		}

		public delegate void ConvertToVoxelOverrideDelegate( Mesh sender, int gridSize, double thinFactor, bool bakeMaterialOpacity, bool optimizeMaterials, double fillHolesFactor, ref bool handled );
		public static event ConvertToVoxelOverrideDelegate ConvertToVoxelOverride;

		public void ConvertToVoxel( int gridSize, double thinFactor, bool bakeMaterialOpacity, bool optimizeMaterials, double fillHolesFactor )
		{
			var handled = false;
			ConvertToVoxelOverride?.Invoke( this, gridSize, thinFactor, bakeMaterialOpacity, optimizeMaterials, fillHolesFactor, ref handled );
			if( handled )
				return;

			var convert = new MeshConvertToVoxel();
			convert.Mesh = this;
			convert.InitialGridSize = gridSize;
			convert.ThinFactor = thinFactor;
			convert.BakeMaterialOpacity = bakeMaterialOpacity;
			convert.OptimizeMaterials = optimizeMaterials;
			convert.FillHolesDistance = fillHolesFactor;

			convert.Convert();
		}

		public bool SupportsBatching()
		{
			var result = Result;
			if( Result != null )
			{
				var meshData = result.MeshData;

				if( meshData.LODs != null )
				{
					//check only last lod for transparent
					var lod = meshData.LODs[ meshData.LODs.Length - 1 ];
					if( lod.Mesh != null && !lod.Mesh.SupportsBatching() )
						return false;
				}
				else
				{
					for( int n = 0; n < meshData.RenderOperations.Count; n++ )
					{
						var oper = meshData.RenderOperations[ n ];
						if( oper.Material != null && oper.Material.Result != null && oper.Material.Result.Transparent )
							return false;
					}
				}

				//for( int n = 0; n < meshData.RenderOperations.Count; n++ )
				//{
				//	var oper = meshData.RenderOperations[ n ];
				//	if( oper.Material != null && oper.Material.Result != null && oper.Material.Result.Transparent )
				//		return false;
				//}

				//if( meshData.LODs != null )
				//{
				//	foreach( var lod in meshData.LODs )
				//		if( lod.Mesh != null && !lod.Mesh.SupportsBatching() )
				//			return false;
				//}
			}

			return true;
		}

		public bool ContainsTransparent()
		{
			var result = Result;
			if( Result != null )
			{
				var meshData = result.MeshData;

				for( int n = 0; n < meshData.RenderOperations.Count; n++ )
				{
					var oper = meshData.RenderOperations[ n ];
					if( oper.Material != null && oper.Material.Result != null && oper.Material.Result.Transparent )
						return true;
				}

				if( meshData.LODs != null )
				{
					foreach( var lod in meshData.LODs )
						if( lod.Mesh != null && lod.Mesh.ContainsTransparent() )
							return true;
				}
			}

			return false;
		}

		//public bool ContainsVirtualizedData()
		//{
		//	foreach( var geometry in GetComponents<MeshGeometry>() )
		//	{
		//		if( geometry.VirtualizedData.Value != null )
		//			return true;
		//	}
		//	return false;
		//}
	}
}
