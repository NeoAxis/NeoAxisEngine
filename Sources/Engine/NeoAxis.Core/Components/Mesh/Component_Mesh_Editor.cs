// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_Mesh_Editor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		int cachedDisplayUV = -1;
		Component_Mesh.CompiledData cachedDisplayUV_MeshData;
		CanvasRenderer.TriangleVertex[] cachedDisplayUV_Triangles;
		CanvasRenderer.TriangleVertex[] cachedDisplayUV_Lines;

		Component_MeshInSpaceAnimationController skeletonAnimationController;

		bool needResultCompile;

		//

		public Component_Mesh Mesh
		{
			get { return (Component_Mesh)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );
			if( Mesh != null )
			{
				Component_MeshInSpace objInSpace = scene.CreateComponent<Component_MeshInSpace>();
				objInSpace.Mesh = Mesh;
				skeletonAnimationController = objInSpace.CreateComponent<Component_MeshInSpaceAnimationController>();
			}
			scene.Enabled = true;

			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			//!!!!?
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( firstCameraUpdate && Scene.CameraEditor.Value != null )
			{
				InitCamera();
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );
			}

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			//copy from Mesh document window code

			var camera = Scene.CameraEditor.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			var cameraLookTo = bounds.GetCenter();

			//!!!!

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;// 2.3;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;// GetSceneCenter();

			Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
			Vector3 to = center;
			Degree fov = 65;// 75;

			//!!!!
			//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			camera.FixedUp = Vector3.ZAxis;
		}

		T[] ExtractChannel<T>( byte[] vertices, int vertexSize, int vertexCount, int startOffsetInBytes ) where T : unmanaged
		{
			T[] result = new T[ vertexCount ];
			unsafe
			{
				fixed( byte* pVertices = vertices )
				{
					byte* src = pVertices + startOffsetInBytes;
					for( int n = 0; n < vertexCount; n++ )
					{
						result[ n ] = *(T*)src;
						src += vertexSize;
					}
				}
			}
			return result;
		}

		int GetSelectedLODIndex()
		{
			if( Mesh.EditorDisplayLOD != 0 )
			{
				var lods = Mesh.Result?.MeshData?.LODs;
				if( lods != null )
					return Math.Min( Mesh.EditorDisplayLOD, lods.Length );
			}
			return 0;
		}

		Component_Mesh.CompiledData GetSelectedLOD()
		{
			return Mesh.Result?.GetLOD( GetSelectedLODIndex() );
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( Mesh != null && needResultCompile )
			{
				Mesh.ResultCompile();
				needResultCompile = false;
			}

			if( Mesh != null && Mesh.Result != null )
			{
				var meshBounds = Mesh.Result.SpaceBounds;
				var selectedLOD = GetSelectedLOD();

				//center axes
				if( Mesh.EditorDisplayPivot )
				{
					var sizeInPixels = 35 * EditorAPI.DPIScale;
					var size = Viewport.Simple3DRenderer.GetThicknessByPixelSize( Vector3.Zero, sizeInPixels );
					//var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					//var size = Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 20;

					var thickness = size / 20;
					var headHeight = size / 3;
					var headRadius = headHeight / 3;
					var alpha = 1.0;// 0.5;

					//viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, alpha ), false, true );
					//viewport.Simple3DRenderer.AddLine( Vector3.Zero, Vector3.XAxis * size );
					//viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, alpha ), false, true );
					//viewport.Simple3DRenderer.AddLine( Vector3.Zero, Vector3.YAxis * size );
					//viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, alpha ), false, true );
					//viewport.Simple3DRenderer.AddLine( Vector3.Zero, Vector3.ZAxis * size );

					Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.XAxis * size, headHeight, headRadius, true, thickness );
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.YAxis * size, headHeight, headRadius, true, thickness );
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, alpha ), false );//, true );
					Viewport.Simple3DRenderer.AddArrow( Vector3.Zero, Vector3.ZAxis * size, headHeight, headRadius, true, thickness );
				}

				//vertex color
				if( Mesh.EditorDisplayVertexColor )
				{
					var vertices = new Simple3DRenderer.Vertex[ selectedLOD.ExtractedVertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
					{
						var sourceVertex = selectedLOD.ExtractedVertices[ n ];

						var vertex = new Simple3DRenderer.Vertex();
						vertex.position = sourceVertex.Position;

						vertex.color = RenderingSystem.ConvertColorValue( ref sourceVertex.Color );
						//vertex.color = sourceVertex.Color;

						//vertex.colorInvisibleBehindObjects = sourceVertex.color;
						vertices[ n ] = vertex;
					}

					Viewport.Simple3DRenderer.AddTriangles( vertices, selectedLOD.ExtractedIndices, false, true );
				}

				//triangles
				if( Mesh.EditorDisplayTriangles )
				{
					//!!!!цвета настраивать в опциях
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.7 ) );
					//viewport.DebugGeometry.SetColor( new ColorValue( 0, 0, 1, 0.3 ) );
					Viewport.Simple3DRenderer.AddTriangles( selectedLOD.ExtractedVerticesPositions, selectedLOD.ExtractedIndices, true, false );
				}

				//vertices
				if( Mesh.EditorDisplayVertices )
				{
					var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 200;
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 0.7 ) );
					//viewport.DebugGeometry.SetColor( new ColorValue( 0, 1, 0, 0.7 ) );
					foreach( var vertex in selectedLOD.ExtractedVerticesPositions )
					{
						Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( scale, 0, 0 ), vertex + new Vector3F( scale, 0, 0 ) );
						Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( 0, scale, 0 ), vertex + new Vector3F( 0, scale, 0 ) );
						Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( 0, 0, scale ), vertex + new Vector3F( 0, 0, scale ) );
					}
				}

				//normals
				if( Mesh.EditorDisplayNormals )
				{
					var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, 0.7 ) );
					foreach( var vertex in selectedLOD.ExtractedVertices )
					{
						if( vertex.Normal != Vector3F.Zero )
							Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + vertex.Normal * scale );
					}
				}

				//tangents
				if( Mesh.EditorDisplayTangents )
				{
					var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 0.7 ) );
					foreach( var vertex in selectedLOD.ExtractedVertices )
					{
						if( vertex.Tangent != Vector4F.Zero )
							Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + vertex.Tangent.ToVector3F() * scale );
					}
				}

				//binormals
				if( Mesh.EditorDisplayBinormals )
				{
					var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.7 ) );
					foreach( var vertex in selectedLOD.ExtractedVertices )
					{
						if( vertex.Normal != Vector3F.Zero && vertex.Tangent != Vector4F.Zero )
						{
							var v = Vector3.Cross( vertex.Tangent.ToVector3F(), vertex.Normal ) * vertex.Tangent.W;
							Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + v * scale );
						}
					}
				}

				//collision
				if( Mesh.EditorDisplayCollision )
				{
					var collision = Mesh.GetComponent( "Collision Definition" ) as Component_RigidBody;
					if( collision != null )
					{
						ColorValue color = new ColorValue( 0, 0, 1, 0.7 );
						//if( MotionType.Value == MotionTypeEnum.Static )
						//	color = ProjectSettings.Get.SceneShowPhysicsStaticColor;
						//else if( rigidBody.IsActive )
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicInactiveColor;
						Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

						int verticesRendered = 0;
						foreach( var shape in collision.GetComponents<Component_CollisionShape>( false, true, true ) )
							shape.Render( Viewport, Transform.Identity, false, ref verticesRendered );
					}
				}

				//update PlayAnimation
				if( !string.IsNullOrEmpty( Mesh.EditorPlayAnimation ) )
				{
					var animation = Mesh.ParentRoot.GetComponentByPath( Mesh.EditorPlayAnimation ) as Component_Animation;
					skeletonAnimationController.PlayAnimation = animation;
					//viewport.CanvasRenderer.AddText( animation != null ? animation.ToString() : "null", new Vec2( .5, .5 ) );
				}
				else
					skeletonAnimationController.PlayAnimation = null;

				//skeleton
				if( Mesh.EditorDisplaySkeleton )
				{
					var skeleton = Mesh.Skeleton.Value;
					if( skeleton != null )
					{
						var skeletonArrows = skeletonAnimationController.GetCurrentAnimatedSkeletonArrows();
						if( skeletonArrows != null )
						{
							var color = new ColorValue( 0, 0.5, 1, 0.7 );
							Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

							foreach( var arrow in skeletonArrows )
								Viewport.Simple3DRenderer.AddArrow( arrow.Start, arrow.End );
						}
						else
						{
							foreach( var bone in skeleton.GetBones() )
							{
								var pos = bone.Transform.Value.Position;
								var parent = bone.Parent as Component_SkeletonBone;
								if( parent != null )
								{
									var from = parent.Transform.Value.Position;

									ColorValue color;
									if( SelectedObjectsSet.Contains( bone ) )
										color = new ColorValue( 0, 1, 0 );
									else
										color = new ColorValue( 0, 0.5, 1, 0.7 );
									Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

									Viewport.Simple3DRenderer.AddArrow( from, pos );
								}
							}
						}
					}
				}

				//bounds
				if( Mesh.EditorDisplayBounds )
				{
					Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 1, 0, 0.7 ) );
					//viewport.DebugGeometry.SetColor( new ColorValue( 1, 1, 0, 0.5 ) );
					if( meshBounds.BoundingBox.HasValue )
						Viewport.Simple3DRenderer.AddBounds( meshBounds.BoundingBox.Value );
					if( meshBounds.BoundingSphere.HasValue )
						Viewport.Simple3DRenderer.AddSphere( meshBounds.BoundingSphere.Value );
				}

				//select triangle
				int triangleIndex = -1;
				if( !Viewport.MouseRelativeMode )
				{
					var ray = Viewport.CameraSettings.GetRayByScreenCoordinates( Viewport.MousePosition );
					if( selectedLOD.RayCast( ray, Component_Mesh.CompiledData.RayCastMode.BruteforceNoCache, false, out double scale2, out int triangleIndex2 ) )
						triangleIndex = triangleIndex2;
				}

				//selected triangle data
				if( triangleIndex != -1 )
				{
					var vertices = selectedLOD.ExtractedVertices;
					var indices = selectedLOD.ExtractedIndices;

					int index0 = indices[ triangleIndex * 3 + 0 ];
					int index1 = indices[ triangleIndex * 3 + 1 ];
					int index2 = indices[ triangleIndex * 3 + 2 ];
					var showVertices = new int[] { index0, index1, index2 };
					Vector3F p0 = vertices[ index0 ].Position;
					Vector3F p1 = vertices[ index1 ].Position;
					Vector3F p2 = vertices[ index2 ].Position;

					////draw connected triangles
					//{
					//	ESet<Vector3I> triangles = new ESet<Vector3I>();

					//	//find triangles with indexes
					//	for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
					//	{
					//		var i0 = indices[ nTriangle * 3 + 0 ];
					//		var i1 = indices[ nTriangle * 3 + 1 ];
					//		var i2 = indices[ nTriangle * 3 + 2 ];

					//		int c = 0;
					//		if( index0 == i0 || index1 == i0 || index2 == i0 )
					//			c++;
					//		if( index0 == i1 || index1 == i1 || index2 == i1 )
					//			c++;
					//		if( index0 == i2 || index1 == i2 || index2 == i2 )
					//			c++;

					//		//if( index0 == i0 || index0 == i1 || index0 == i2 ||
					//		//	index1 == i0 || index1 == i1 || index1 == i2 ||
					//		//	index2 == i0 || index2 == i1 || index2 == i2 )
					//		if( c == 2 )
					//		{
					//			Vector3I triangle = new Vector3I( i0, i1, i2 );
					//			if( triangle[ 1 ] < triangle[ 0 ] ) { var v = triangle[ 0 ]; triangle[ 0 ] = triangle[ 1 ]; triangle[ 1 ] = v; }
					//			if( triangle[ 2 ] < triangle[ 0 ] ) { var v = triangle[ 0 ]; triangle[ 0 ] = triangle[ 2 ]; triangle[ 2 ] = v; }
					//			if( triangle[ 2 ] < triangle[ 1 ] ) { var v = triangle[ 1 ]; triangle[ 1 ] = triangle[ 2 ]; triangle[ 2 ] = v; }

					//			triangles.AddWithCheckAlreadyContained( triangle );
					//		}
					//	}

					//	foreach( var triangle in triangles )
					//	{
					//		var v0 = vertices[ triangle[ 0 ] ].Position;
					//		var v1 = vertices[ triangle[ 1 ] ].Position;
					//		var v2 = vertices[ triangle[ 2 ] ].Position;

					//		//viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.3 ) );
					//		//viewport.Simple3DRenderer.AddTriangles( new Vector3F[] { v0, v1, v2 }, Matrix4.Identity, false, false );
					//		viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1 ) );
					//		viewport.Simple3DRenderer.AddLine( v0, v1 );
					//		viewport.Simple3DRenderer.AddLine( v1, v2 );
					//		viewport.Simple3DRenderer.AddLine( v2, v0 );
					//	}
					//}

					//draw triangle
					{
						Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 1, 0, 0.3 ) );
						Viewport.Simple3DRenderer.AddTriangles( new Vector3F[] { p0, p1, p2 }, false, false );
						Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 1, 0 ) );
						Viewport.Simple3DRenderer.AddLine( p0, p1 );
						Viewport.Simple3DRenderer.AddLine( p1, p2 );
						Viewport.Simple3DRenderer.AddLine( p2, p0 );
					}

					float maxLength = Math.Max( ( p0 - p1 ).Length(), Math.Max( ( p1 - p2 ).Length(), ( p2 - p0 ).Length() ) );
					float arrowLength = maxLength / 5;
					float vertexRadius = maxLength / 40;

					//!!!!можно еще буквами подписать как в transform tool
					//normals, tangents
					foreach( int nVertex in showVertices )
					{
						var v = vertices[ nVertex ];
						Vector3F p = v.Position;

						if( v.Normal != Vector3F.Zero )
						{
							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1 ) );
							Viewport.Simple3DRenderer.AddArrow( p, p + v.Normal * arrowLength, 0, 0, true );

							if( v.Tangent != Vector4F.Zero )
							{
								Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
								Viewport.Simple3DRenderer.AddArrow( p, p + v.Tangent.ToVector3F() * arrowLength, 0, 0, true );

								var bitangent = Vector3F.Cross( v.Tangent.ToVector3F(), v.Normal ) * v.Tangent.W;
								Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0 ) );
								Viewport.Simple3DRenderer.AddArrow( p, p + bitangent * arrowLength, 0, 0, true );
							}
						}
					}

					//positions
					{
						Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
						//SimpleMeshGenerator.GenerateSphere( vertexRadius, 10, 10, false, out Vec3F[] spherePositions, out int[] sphereIndices );

						foreach( int nVertex in showVertices )
						{
							var v = vertices[ nVertex ];
							Vector3F p = v.Position;
							Viewport.Simple3DRenderer.AddSphere( new Sphere( p, vertexRadius ), 10, true );
							//viewport.DebugGeometry.AddTriangles( spherePositions, sphereIndices, Mat4.FromTranslate( p ), false, false );
						}
					}
				}

				//highlight selected mesh geometries, materials
				if( GetSelectedLODIndex() == 0 )
				{
					var geometriesToHighlight = new ESet<Component_MeshGeometry>();
					foreach( var geometry in Mesh.GetComponents<Component_MeshGeometry>() )// checkChildren: true ) )
					{
						if( SelectedObjectsSet.Contains( geometry ) )
							geometriesToHighlight.AddWithCheckAlreadyContained( geometry );

						var material = geometry.Material.Value;
						if( material != null && SelectedObjectsSet.Contains( material ) )
							geometriesToHighlight.AddWithCheckAlreadyContained( geometry );
					}

					foreach( var geometry in geometriesToHighlight )
					{
						var structure = geometry.VertexStructure.Value;
						var vertices = geometry.Vertices.Value;
						var indices = geometry.Indices.Value;

						if( vertices != null & indices != null )
						{
							structure.GetInfo( out var vertexSize, out var holes );
							var vertexCount = vertices.Length / vertexSize;

							try
							{
								if( structure.GetElementBySemantic( VertexElementSemantic.Position, out var element ) && element.Type == VertexElementType.Float3 )
								{
									if( vertices != null && indices != null )
									{
										//!!!!кешировать?

										var positions = ExtractChannel<Vector3F>( vertices, vertexSize, vertexCount, element.Offset );

										Viewport.Simple3DRenderer.SetColor( ProjectSettings.Get.SelectedColor );
										Viewport.Simple3DRenderer.AddTriangles( positions, indices, true, true );
									}
								}
							}
							catch( Exception e )
							{
								Log.Warning( e.Message );
							}
						}
					}
				}

				//update selected LOD
				if( Scene.RenderingPipeline.Value != null )
					Scene.RenderingPipeline.Value.LODRange = new RangeI( Mesh.EditorDisplayLOD, Mesh.EditorDisplayLOD );
			}
		}

		protected override void OnViewportUpdateBeforeOutput2()
		{
			base.OnViewportUpdateBeforeOutput2();

			if( Mesh != null && Mesh.Result != null )
			{
				//UV
				if( Mesh.EditorDisplayUV != -1 )
					DrawUV();
			}
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "MeshDocumentWindow", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( Mesh != null )
			{
				var data = GetSelectedLOD();
				if( data != null )
				{
					lines.Add( Translate( "Level of detail" ) + ": " + GetSelectedLODIndex().ToString() );
					lines.Add( Translate( "Vertices" ) + ": " + data.ExtractedVerticesPositions.Length.ToString() );
					lines.Add( Translate( "Triangles" ) + ": " + ( data.ExtractedIndices.Length / 3 ).ToString() );
					lines.Add( Translate( "Render operations" ) + ": " + data.MeshData.RenderOperations.Count.ToString() );
					lines.Add( Translate( "Bounding box size" ) + ": " + data.SpaceBounds.BoundingBox.Value.GetSize().ToString( 3 ) );
					lines.Add( Translate( "Bounding sphere radius" ) + ": " + data.SpaceBounds.BoundingSphere.Value.Radius.ToString( "N3" ) );
				}
			}
		}

		/////////////////////////////////////////

		void DrawUV()
		{
			var meshData = GetSelectedLOD();

			if( Mesh.EditorDisplayUV >= 0 && Mesh.EditorDisplayUV < 4 )
			{
				StandardVertex.Components component = StandardVertex.Components.TexCoord0;
				if( Mesh.EditorDisplayUV == 0 )
					component = StandardVertex.Components.TexCoord0;
				else if( Mesh.EditorDisplayUV == 1 )
					component = StandardVertex.Components.TexCoord1;
				else if( Mesh.EditorDisplayUV == 2 )
					component = StandardVertex.Components.TexCoord2;
				else if( Mesh.EditorDisplayUV == 3 )
					component = StandardVertex.Components.TexCoord3;

				if( ( meshData.ExtractedVerticesComponents & component ) != 0 )
				{
					var uvArray = (Vector2F[])StandardVertex.ExtractOneComponentArray( meshData.ExtractedVertices, component );

					//bool valid = Array.Exists( uvArray, item => item != Vec2F.Zero );
					//if( valid )
					//{

					//check old cache
					if( cachedDisplayUV_Triangles != null )
					{
						if( cachedDisplayUV != Mesh.EditorDisplayUV || cachedDisplayUV_MeshData != meshData )
						{
							cachedDisplayUV_Triangles = null;
							cachedDisplayUV_Lines = null;
						}
					}

					Vector2F screenMultiplier = new Vector2F( Viewport.CanvasRenderer.AspectRatioInv * .8f, 0.8f );
					Vector2F screenOffset = new Vector2F( 1.0f - screenMultiplier.X - 0.01f, 1.0f - screenMultiplier.Y - 0.01f );

					Vector2F Convert( Vector2F v )
					{
						return v * screenMultiplier + screenOffset;
					};

					//update
					if( cachedDisplayUV_Triangles == null )
					{
						cachedDisplayUV = Mesh.EditorDisplayUV;
						cachedDisplayUV_MeshData = meshData;

						var indices = meshData.ExtractedIndices;

						//!!!!показывать те, которые за границами 0-1.


						var uvArrayConverted = new Vector2F[ uvArray.Length ];
						for( int n = 0; n < uvArrayConverted.Length; n++ )
							uvArrayConverted[ n ] = Convert( uvArray[ n ] );

						var trianglesColor = new ColorValue( 0, 1, 0, 0.5 );
						var linesColor = new ColorValue( 1, 1, 1 );

						//triangles
						{
							var triangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 3 );

							for( int triangle = 0; triangle < indices.Length / 3; triangle++ )
							{
								int index0 = indices[ triangle * 3 + 0 ];
								int index1 = indices[ triangle * 3 + 1 ];
								int index2 = indices[ triangle * 3 + 2 ];

								Vector2F v0 = uvArrayConverted[ index0 ];
								Vector2F v1 = uvArrayConverted[ index1 ];
								Vector2F v2 = uvArrayConverted[ index2 ];

								triangles.Add( new CanvasRenderer.TriangleVertex( v0, trianglesColor ) );
								triangles.Add( new CanvasRenderer.TriangleVertex( v1, trianglesColor ) );
								triangles.Add( new CanvasRenderer.TriangleVertex( v2, trianglesColor ) );
							}

							cachedDisplayUV_Triangles = triangles.ToArray();
						}

						//lines
						{
							int[] lineIndices = MathAlgorithms.TriangleListToLineList( indices );

							var lines = new List<CanvasRenderer.LineItem>( lineIndices.Length / 2 );
							for( int nLine = 0; nLine < lineIndices.Length / 2; nLine++ )
							{
								var v0 = uvArrayConverted[ lineIndices[ nLine * 2 + 0 ] ];
								var v1 = uvArrayConverted[ lineIndices[ nLine * 2 + 1 ] ];
								lines.Add( new CanvasRenderer.LineItem( v0, v1, linesColor ) );
							}

							var vertices = new CanvasRenderer.TriangleVertex[ lines.Count * 2 ];
							for( int n = 0; n < lines.Count; n++ )
							{
								var line = lines[ n ];
								vertices[ n * 2 + 0 ] = new CanvasRenderer.TriangleVertex( line.start, line.color, Vector2F.Zero );
								vertices[ n * 2 + 1 ] = new CanvasRenderer.TriangleVertex( line.end, line.color, Vector2F.Zero );
							}

							cachedDisplayUV_Lines = vertices;
						}
					}

					//draw
					if( cachedDisplayUV_Triangles != null )
					{
						Viewport.CanvasRenderer.AddQuad( new RectangleF( Convert( Vector2F.Zero ), Convert( Vector2F.One ) ), new ColorValue( 0, 0, 0, .5 ) );
						Viewport.CanvasRenderer.AddTriangles( cachedDisplayUV_Triangles );
						Viewport.CanvasRenderer.AddLines( cachedDisplayUV_Lines );
					}
				}
			}
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needResultCompile = true;
		}
	}
}
