// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;
using System.Drawing;
using System.Linq;

namespace NeoAxis
{
	//!!!!Component_SetTextureCoordinates
	//!!!!!!Reference<int> Channel

	//!!!!Component_MeshTesselation

	//!!!!по ссылке получать результат. т.е. например переопределить, чтобы внутри меша (детей) генерилось как-то по другому

	/// <summary>
	/// The construct of mesh geometries.
	/// </summary>
	[ResourceFileExtension( "mesh" )]
	[EditorDocumentWindow( typeof( Component_Mesh_DocumentWindow ) )]
	[EditorPreviewControl( typeof( Component_Mesh_PreviewControl ) )]
	[EditorSettingsCell( typeof( Component_Mesh_SettingsCell ) )]
	public partial class Component_Mesh : Component_ResultCompile<Component_Mesh.CompiledData>
	{
		//!!!!ссылочное? тогда видимое в редакторе. свойство мешать будет?
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

		[Browsable( false )]
		public bool CalculateExtractedDataAndBounds { get; set; } = true;

		//!!!!
		//bool [Use]BoundingConvex;

		//!!!!
		//bool mergeRenderOperations;

		//!!!!impl
		////Include
		////!!!!default value Count = 0 but how to implement
		//ReferenceList<Component_Mesh> _include;
		//[Serialize]
		//public virtual ReferenceList<Component_Mesh> Include
		//{
		//	get { return _include; }
		//}
		//public delegate void IncludeChangedDelegate( Component_Mesh sender );
		//public event IncludeChangedDelegate IncludeChanged;

		/// <summary>
		/// The skeleton of the skinned mesh.
		/// </summary>
		[DefaultValue( null )]
		[Serialize]
		public Reference<Component_Skeleton> Skeleton
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
		public event Action<Component_Mesh> SkeletonChanged;
		ReferenceField<Component_Skeleton> _skeleton;

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
		public event Action<Component_Mesh> BillboardChanged;
		ReferenceField<bool> _billboard = false;

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
		public event Action<Component_Mesh> CastShadowsChanged;
		ReferenceField<bool> _castShadows = true;

		/// <summary>
		/// Maximum visibility range of the object.
		/// </summary>
		[DefaultValue( 10000.0 )]
		[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Component_Mesh> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 10000.0;

		//[DisplayName( "Structure" )]
		//public bool StructureAvailable
		//{
		//	get { return Structure != null; }
		//}

		///////////////////////////////////////////////

		//!!!!Wireframe, sockets, collision

		//!!!!impl всем

		/// <summary>
		/// Whether the mesh pivot displayed in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayPivot { get; set; }

		/// <summary>
		/// Whether to display mesh bounds in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayBounds { get; set; }

		/// <summary>
		/// Whether to display mesh triangles in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayTriangles { get; set; }

		/// <summary>
		/// Whether to display mesh vertices in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayVertices { get; set; }

		/// <summary>
		/// Whether to display mesh normals in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayNormals { get; set; }

		/// <summary>
		/// Whether to display mesh tangents in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayTangents { get; set; }

		/// <summary>
		/// Whether to display mesh binormals in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayBinormals { get; set; }

		/// <summary>
		/// Whether to display mesh vertex colors in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayVertexColor { get; set; }

		/// <summary>
		/// Whether to display mesh uv's in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( -1 )]
		public int EditorDisplayUV { get; set; } = -1;

		/// <summary>
		/// Whether to display number of level of detail in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( 0 )]
		public int EditorDisplayLOD { get; set; }

		/// <summary>
		/// Whether the mesh collision displayed in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplayCollision { get; set; }

		/// <summary>
		/// Whether to the mesh skeleton displayed in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( false )]
		public bool EditorDisplaySkeleton { get; set; }

		/// <summary>
		/// Specifies the animation name to play in editor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( "" )]
		public string EditorPlayAnimation { get; set; } = "";

		[Browsable( false )]
		public bool AllowDisposeBuffers { get; set; } = true;

		///////////////////////////////////////////////

		/// <summary>
		/// Represents precalculated data of <see cref="Component_Mesh"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			Component_Mesh owner;

			MeshDataClass meshData = new MeshDataClass();

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

			object meshRayTestOptimizedLockObject = new object();
			MeshRayTestOptimized meshRayTestOptimized;

			////////////

			//public class RenderOperation
			//{
			//	//!!!!public fields

			//	public Component_MeshGeometry creator;
			//	public bool disposeBuffersByCreator = true;
			//	//public Component_Mesh disposeBuffersByObject;
			//	//public bool disposeBuffersByMesh = true;

			//	public VertexElement[] vertexStructure;//public GpuVertexDeclaration vertexDeclaration;
			//	public IList<GpuVertexBuffer> vertexBuffers;
			//	public int vertexStartOffset;
			//	public int vertexCount;
			//	public GpuIndexBuffer indexBuffer;
			//	public int indexStartOffset;
			//	public int indexCount;

			//	///// The type of operation to perform
			//	//OperationType operationType;

			//	//!!!!pass/material
			//	public Component_Material material;

			//	//!!!!
			//	//public Transform transform = NeoAxis.Transform.Identity;

			//	//!!!!?
			//	//public Mat4 transform = Mat4.Identity;

			//	//xx xx;
			//	////!!!!bounds тут считать

			//	//xx xx;//vertexPositions. что еще

			//	////!!!!!что еще?

			//	//xx xx;

			//	//!!!!может нужно
			//	//public RenderOperation Clone()
			//	//{
			//	//	RenderOperation newOp = new RenderOperation();

			//	//	//!!!!what else

			//	//	newOp.creator = creator;
			//	//	newOp.disposeBuffersByCreator = disposeBuffersByCreator;

			//	//	newOp.vertexDeclaration = vertexDeclaration;
			//	//	newOp.vertexBuffers = vertexBuffers;
			//	//	newOp.vertexStartOffset = vertexStartOffset;
			//	//	newOp.vertexCount = vertexCount;
			//	//	newOp.indexBuffer = indexBuffer;
			//	//	newOp.indexStartOffset = indexStartOffset;
			//	//	newOp.indexCount = indexCount;

			//	//	newOp.material = material;

			//	//	newOp.transform = transform;

			//	//	return newOp;
			//	//}

			//	public bool ContainsDisposedBuffers()
			//	{
			//		//!!!!slowly где вызывается?

			//		if( vertexBuffers != null )
			//		{
			//			for( int n = 0; n < vertexBuffers.Count; n++ )
			//			{
			//				if( vertexBuffers[ n ].Disposed )
			//					return true;
			//			}
			//		}
			//		if( indexBuffer != null && indexBuffer.Disposed )
			//			return true;
			//		return false;
			//	}
			//}

			////////////

			//public class RenderOperationItem
			//{
			//	//!!!!public

			//	public RenderOperation operation;
			//	//!!!!надо ли. не юзается.
			//	public Transform transform = Transform.Identity;
			//	//public Mat4F transform;

			//	public RenderOperationItem()
			//	{
			//	}

			//	public RenderOperationItem( RenderOperation operation, Transform transform )
			//	{
			//		this.operation = operation;
			//		this.transform = transform;
			//	}

			//	public RenderOperationItem( RenderOperation operation )
			//	{
			//		this.operation = operation;
			//	}
			//}

			////////////

			/// <summary>
			/// Represents geometry data of the mesh for rendering pipeline.
			/// </summary>
			public class MeshDataClass : Component_RenderingPipeline.RenderSceneData.IMeshData
			{
				public object Creator { get; set; }
				public object AnyData { get; set; }
				public List<Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation> RenderOperations { get; } = new List<Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation>();
				//public IList<Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation> RenderOperations { get; } = new List<Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation>();
				public SpaceBounds SpaceBounds { get; set; }
				public Component_RenderingPipeline.RenderSceneData.IMeshDataLODLevel[] LODs { get; set; }
				public int BillboardMode { get; set; }
				public Component_RenderingPipeline.RenderSceneData.LayerItem[] PaintLayers { get; set; }

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
					{
						if( oper.VertexBuffers != null )
						{
							for( int n = 0; n < oper.VertexBuffers.Count; n++ )
								oper.VertexBuffers[ n ]?.Dispose();
						}
						oper.IndexBuffer?.Dispose();
					}
				}
			}

			////////////

			public CompiledData( Component_Mesh owner )
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

				lock( meshRayTestOptimizedLockObject )
				{
					meshRayTestOptimized?.Dispose();
					meshRayTestOptimized = null;
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

			public Component_Mesh Owner
			{
				get { return owner; }
			}

			public MeshDataClass MeshData
			{
				get { return meshData; }
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
					extractedVertices = new StandardVertex[ 0 ];
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
							extractedVertices = new StandardVertex[ 0 ];
							extractedVerticesComponents = 0;
							extractedVerticesPositions = new Vector3F[ 0 ];
							extractedIndices = new int[ 0 ];
							return;
						}
					}
					if( oper.IndexBuffer != null && oper.IndexBuffer.Indices == null )
					{
						extractedVertices = new StandardVertex[ 0 ];
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

				extractedVertices = new StandardVertex[ totalVertices ];
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
							{
								extractedVertices[ destIndex++ ].Position = p;
								////!!!!slowly
								//extractedVertices[ destIndex++ ].Position = ( item.transform * p.ToVector3() ).ToVector3F();
							}

							extractedVerticesComponents |= StandardVertex.Components.Position;
						}
					}

					//Normal
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) &&
							element.Type == VertexElementType.Float3 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector3F>( element.Offset );

							int destIndex = startIndex;
							foreach( var p in values )
							{
								extractedVertices[ destIndex++ ].Normal = p;

								////!!!!slowly
								////!!!!true?

								//var v = ( item.transform.Rotation * p.ToVector3() ).ToVector3F();
								////if( normalizeNormalsTangents )
								//v.Normalize();
								//extractedVertices[ destIndex++ ].Normal = v;
							}

							extractedVerticesComponents |= StandardVertex.Components.Normal;
						}
					}

					//Tangent
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) &&
							element.Type == VertexElementType.Float4 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector4F>( element.Offset );

							int destIndex = startIndex;
							foreach( var p in values )
							{
								extractedVertices[ destIndex++ ].Tangent = p;

								////!!!!slowly
								////!!!!true?

								//var v = ( item.transform.Rotation * p.ToVector3F().ToVector3() ).ToVector3F();
								////if( normalizeNormalsTangents )
								//v.Normalize();
								//extractedVertices[ destIndex++ ].Tangent = new Vector4F( v, p.W );
							}

							extractedVerticesComponents |= StandardVertex.Components.Tangent;
						}
					}

					//Color
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
						{
							if( element.Type == VertexElementType.Float4 )
							{
								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<Vector4F>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = p.ToColorValue();
								extractedVerticesComponents |= StandardVertex.Components.Color;
							}
							else if( element.Type == VertexElementType.ColorABGR )
							{
								//!!!!check

								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<uint>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = new ColorValue( ColorByte.FromABGR( p ) );
								extractedVerticesComponents |= StandardVertex.Components.Color;
							}
							else if( element.Type == VertexElementType.ColorARGB )
							{
								//!!!!check

								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<uint>( element.Offset );
								int destIndex = startIndex;
								foreach( var p in values )
									extractedVertices[ destIndex++ ].Color = new ColorValue( ColorByte.FromARGB( p ) );
								extractedVerticesComponents |= StandardVertex.Components.Color;
							}
						}
					}

					//TexCoord0
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector2F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].TexCoord0 = p;
							extractedVerticesComponents |= StandardVertex.Components.TexCoord0;
						}
					}

					//TexCoord1
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector2F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].TexCoord1 = p;
							extractedVerticesComponents |= StandardVertex.Components.TexCoord1;
						}
					}

					//TexCoord2
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector2F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].TexCoord2 = p;
							extractedVerticesComponents |= StandardVertex.Components.TexCoord2;
						}
					}

					//TexCoord3
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate3, out VertexElement element ) &&
							element.Type == VertexElementType.Float2 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector2F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].TexCoord3 = p;
							extractedVerticesComponents |= StandardVertex.Components.TexCoord3;
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
							extractedVerticesComponents |= StandardVertex.Components.BlendIndices;
						}
					}

					//BlendWeights
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.BlendWeights, out VertexElement element ) &&
							element.Type == VertexElementType.Float4 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var values = buffer.ExtractChannel<Vector4F>( element.Offset );
							int destIndex = startIndex;
							foreach( var p in values )
								extractedVertices[ destIndex++ ].BlendWeights = p;
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

				for( int n = 0; n < ExtractedVerticesPositions.Length; n++ )
					extractedVerticesPositions[ n ] = extractedVertices[ n ].Position;

				if( currentVertex != totalVertices )
					Log.Fatal( "Component_Mesh.CompiledData: ExtractData: currentVertex != totalVertices." );
				if( currentIndex != totalIndices )
					Log.Fatal( "Component_Mesh.CompiledData: ExtractData: currentIndex != totalIndices." );
			}

			public StandardVertex[] ExtractedVertices
			{
				get { return extractedVertices; }
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

			public enum RayCastMode
			{
				Auto,
				BruteforceNoCache,
				OctreeOptimizedCached
			}

			public bool RayCast( Ray ray, RayCastMode mode, bool twoSided, out double scale, out int triangleIndex )
			{
				scale = 0;
				triangleIndex = -1;

				if( !SpaceBounds.CalculatedBoundingBox.Intersects( ray ) )
					return false;
				if( !SpaceBounds.CalculatedBoundingSphere.Intersects( ray ) )
					return false;

				if( mode == RayCastMode.Auto )
				{
					int minTriangles = 20;

					if( ExtractedIndices.Length > minTriangles * 3 )
						mode = RayCastMode.OctreeOptimizedCached;
					else
						mode = RayCastMode.BruteforceNoCache;
				}

				RayF rayF = ray.ToRayF();

				if( mode == RayCastMode.OctreeOptimizedCached )
				{
					//octree optimized cached

					//!!!!может сразу создавать?
					//!!!!threading

					MeshRayTestOptimized.ResultItem[] result = null;

					lock( meshRayTestOptimizedLockObject )
					{
						//!!!!поточность в octree поддержать
						if( meshRayTestOptimized == null )
							meshRayTestOptimized = new MeshRayTestOptimized( ExtractedVerticesPositions, ExtractedIndices );
						result = meshRayTestOptimized.RayTest( rayF, MeshRayTestOptimized.Mode.OneClosest, twoSided );
					}

					if( result.Length > 0 )
					{
						var item = result[ 0 ];
						scale = item.Scale;
						triangleIndex = item.TriangleIndex;
						return true;
					}

					return false;
				}
				else
				{
					//Brute-force no cache

					var vertices = ExtractedVerticesPositions;
					var indices = ExtractedIndices;

					bool found = false;

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
							if( MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex1, ref vertex2, ref rayF, out var localScale ) ||
							twoSided && MathAlgorithms.IntersectTriangleRay( ref vertex0, ref vertex2, ref vertex1, ref rayF, out localScale ) )
							{
								if( !found || localScale < scale )
								{
									found = true;
									scale = localScale;
									triangleIndex = nTriangle;
								}

								//Vec3 localPoint = localRay.GetPointOnRay( localScale );
								//Vec3 worldPoint = transform * localPoint;

								//float worldScale;
								//if( rayLength != 0 )
								//	worldScale = ( worldPoint - ray.Origin ).Length() / rayLength;
								//else
								//	worldScale = 0;

								//if( list.Count == 0 )
								//	list.Add( new Result( worldScale, nSubMesh ) );
								//else
								//{
								//	if( piercing )
								//		list.Add( new Result( worldScale, nSubMesh ) );
								//	else
								//	{
								//		if( worldScale < list[ 0 ].rayScale )
								//			list[ 0 ] = new Result( worldScale, nSubMesh );
								//	}
								//}
							}
						}
					}

					return found;
				}
			}

			//!!!!
			internal bool _Intersects( Plane[] planes, ref Bounds bounds )
			{
				if( !SpaceBounds.CalculatedBoundingBox.Intersects( ref bounds ) )
					return false;
				if( !SpaceBounds.CalculatedBoundingSphere.Intersects( ref bounds ) )
					return false;

				//if( ExtractedIndices.Length < 2048 )
				//{

				lock( meshRayTestOptimizedLockObject )
				{
					//!!!!поточность в octree поддержать
					if( meshRayTestOptimized == null )
						meshRayTestOptimized = new MeshRayTestOptimized( ExtractedVerticesPositions, ExtractedIndices );
					return meshRayTestOptimized._Intersects( planes, ref bounds );
				}

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
		}

		/////////////////////////////////////////

		public class ExtractedStructure
		{
			public StructureClass Structure;
			public MeshGeometryItem[] MeshGeometries;

			public class MeshGeometryItem
			{
				public VertexElement[] VertexStructure;
				public byte[] Vertices;
				public int[] Indices;
				public Reference<Component_Material> Material;//public Component_Material Material;
			}
		}

		/////////////////////////////////////////

		public Component_Mesh()
		{
			//_include = new ReferenceList<Component_Mesh>( this, delegate ()
			//{
			//	IncludeChanged?.Invoke( this );
			//} );
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

				var modifiers = GetComponents<Component_MeshModifier>();
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
				compiledData.SpaceBounds = new SpaceBounds( bounds, sphere );

				compiledData.MeshData.SpaceBounds = compiledData.SpaceBounds;
				//compiledData.MeshData.BoundingBox = bounds;
				//compiledData.MeshData.BoundingSphere = sphere;

				//!!!!тут?
				//LODs
				if( compiledData.MeshData.LODs == null )
				{
					var components = GetComponents<Component_MeshLevelOfDetail>();

					int enabledCount = 0;
					foreach( var lod in components )
					{
						if( lod.Enabled )
							enabledCount++;
					}

					if( enabledCount != 0 )
					{
						var lods = new Component_RenderingPipeline.RenderSceneData.IMeshDataLODLevel[ enabledCount ];
						int current = 0;
						foreach( var lod in components )
						{
							if( lod.Enabled )
							{
								var item = new Component_RenderingPipeline.RenderSceneData.IMeshDataLODLevel();
								item.Mesh = lod.Mesh;
								item.DistanceSquared = (float)( lod.Distance * lod.Distance );

								lods[ current ] = item;
								current++;
							}
						}

						//sort by distance
						CollectionUtility.InsertionSort( lods, delegate ( Component_RenderingPipeline.RenderSceneData.IMeshDataLODLevel l1, Component_RenderingPipeline.RenderSceneData.IMeshDataLODLevel l2 )
						 {
							 if( l1.DistanceSquared < l2.DistanceSquared )
								 return -1;
							 if( l1.DistanceSquared > l2.DistanceSquared )
								 return 1;
							 return 0;
						 } );

						compiledData.MeshData.LODs = lods;
					}
				}

				//BillboardData
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
				}

				//Paint layers
				if( compiledData.MeshData.PaintLayers == null )
				{
					var items = new List<Component_RenderingPipeline.RenderSceneData.LayerItem>();
					foreach( var layer in GetComponents<Component_PaintLayer>() )
					{
						if( layer.Enabled )
						{
							//if( layer.MaskImage.Value != null || layer.Mask.Value != null )
							var image = layer.GetImage( out var uniqueMaskDataCounter );
							if( image != null )
							{
								var item = new Component_RenderingPipeline.RenderSceneData.LayerItem();
								item.Material = layer.Material;
								item.Mask = image;
								item.UniqueMaskDataCounter = uniqueMaskDataCounter;
								item.Color = layer.Color;
								items.Add( item );
							}
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
			foreach( var geometry in GetComponents<Component_MeshGeometry>() )// false, true, false ) )
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
			//	Component_Mesh c = listItem.GetValue( this );

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

		public delegate void MeshCompileEventDelegate( Component_Mesh sender, CompiledData compiledData );
		public event MeshCompileEventDelegate MeshCompileEvent;


		protected virtual void OnMeshCompilePostProcess( CompiledData compiledData )
		{
		}
		public delegate void MeshCompilePostProcessEventDelegate( Component_Mesh sender, CompiledData compiledData );
		public event MeshCompilePostProcessEventDelegate MeshCompilePostProcessEvent;

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

			//!!!!где еще. партиклы
		}

		class MergeGroup
		{
			public List<Component_MeshGeometry> geometries = new List<Component_MeshGeometry>();
		}

		public void MergeGeometriesWithEqualVertexStructureAndMaterial()
		{
			//!!!!соединять ли совпадающие вертексы из разных геометрий. может параметром bool mergeEqualVertices. или отдельным методом

			//get groups
			var groups = new List<MergeGroup>();
			foreach( var geometry in GetComponents<Component_MeshGeometry>() )// false, true ) )
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

		public ExtractedStructure ExtractStructure()
		{
			var result = new ExtractedStructure();
			var resultMeshGeometries = new List<ExtractedStructure.MeshGeometryItem>();

			//!!!!проверять валидность? out string error

			result.Structure = (StructureClass)Structure?.Clone();
			var meshGeometries = GetComponents<Component_MeshGeometry>();

			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				var meshGeometry = meshGeometries[ i ];

				var item = new ExtractedStructure.MeshGeometryItem();

				if( meshGeometry is Component_MeshGeometry_Procedural meshGeometryProcedural )
				{
					StructureClass structure = null;
					Component_Material material = null;
					meshGeometryProcedural.GetProceduralGeneratedData( ref item.VertexStructure, ref item.Vertices, ref item.Indices, ref material /*item.Material*/, ref structure );
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
	}
}
