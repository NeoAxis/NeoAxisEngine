#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class MeshEditor : CanvasBasedEditor
	{
		//const bool debugVisualizeVoxelGrid = false;//true;

		bool firstCameraUpdate = true;

		MeshInSpaceAnimationController skeletonAnimationController;

		bool needResultCompile;

		MeshInSpace meshInSpace;

		Line3F[] cachedTriangles;
		Mesh cachedTrianglesForMesh;
		Vector3 cachedTrianglesCameraPositionForSort;
		//int cachedTrianglesSortCounter;

		Line3[] cachedNormals;
		Mesh cachedNormalsForMesh;

		Line3[] cachedTangents;
		Mesh cachedTangentsForMesh;

		Line3[] cachedBinormals;
		Mesh cachedBinormalsForMesh;

		//List<MeshInSpace> meshInSpaceClusterBounds;
		//MeshInSpace meshInSpaceClusterTriangles;
		//static Material meshClusterTrianglesMaterial;

		MeshInSpace meshInSpaceVertices;
		static Material meshVerticesMaterial;

		MeshInSpace meshInSpaceVertexColor;
		static Material meshVertexColorMaterial;

		int cachedDisplayUV = -1;
		Mesh.CompiledData cachedDisplayUV_MeshData;
		CanvasRenderer.TriangleVertex[] cachedDisplayUV_Triangles;
		CanvasRenderer.TriangleVertex[] cachedDisplayUV_Lines;

		//MeshInSpace meshInSpaceVoxels;

		//

		public Mesh Mesh
		{
			get { return (Mesh)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );
			if( Mesh != null )
			{
				meshInSpace = scene.CreateComponent<MeshInSpace>();
				meshInSpace.Mesh = Mesh;
				skeletonAnimationController = meshInSpace.CreateComponent<MeshInSpaceAnimationController>();
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

			meshInSpace?.Dispose();//.RemoveFromParent( false );
			meshInSpace = null;
			//DestroyMeshInSpaceVoxels();
			//DestroyMeshInSpaceClusterBounds();
			//DestroyMeshInSpaceClusterTriangles();
			DestroyMeshInSpaceVertices();
			DestroyMeshInSpaceVertexColor();

			base.OnDestroy();
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

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

			if( Mesh != null && Scene.CameraEditor.Value != null )
				Mesh.EditorCameraTransform = Scene.CameraEditor.Value.Transform;

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			//copy from Mesh document window code

			var camera = Scene.CameraEditor.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			//var bounds = meshInSpace != null ? meshInSpace.SpaceBounds.CalculatedBoundingBox : Scene.CalculateTotalBoundsOfObjectsInSpace();
			var cameraLookTo = bounds.GetCenter();

			//!!!!

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;
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

			if( Mesh != null && Mesh.EditorCameraTransform != null )
				camera.Transform = Mesh.EditorCameraTransform;
			else
				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );

			camera.FixedUp = Vector3.ZAxis;
		}

		static T[] ExtractChannel<T>( byte[] vertices, int vertexSize, int vertexCount, int startOffsetInBytes ) where T : unmanaged
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
			if( Mesh.EditorDisplayLOD > 0 )
			{
				var lods = Mesh.Result?.MeshData?.LODs;
				if( lods != null )
					return Math.Min( Mesh.EditorDisplayLOD, lods.Length );
			}
			return 0;
		}

		Mesh.CompiledData GetSelectedLOD()
		{
			return Mesh.Result?.GetLOD( GetSelectedLODIndex() );
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			if( Mesh != null && needResultCompile )
			{
				Mesh.PerformResultCompile();
				needResultCompile = false;

				//DestroyMeshInSpaceVoxels();
				//DestroyMeshInSpaceClusterBounds();
				//DestroyMeshInSpaceClusterTriangles();
				DestroyMeshInSpaceVertices();
				DestroyMeshInSpaceVertexColor();
			}

			//if( Mesh?.Result != null && /*Mesh.EditorDisplayVoxelized && */Scene != null && meshInSpaceVoxels == null && debugVisualizeVoxelGrid )
			//	CreateMeshInSpaceVoxels();

			//if( Mesh?.Result != null && Scene != null && meshInSpaceClusterBounds == null )
			//	CreateMeshInSpaceClusterBounds();
			//if( Mesh?.Result != null && Scene != null && meshInSpaceClusterTriangles == null )
			//	CreateMeshInSpaceClusterTriangles();
			if( Mesh?.Result != null && Scene != null && meshInSpaceVertices == null )
				CreateMeshInSpaceVertices();
			if( Mesh?.Result != null && Scene != null && meshInSpaceVertexColor == null )
				CreateMeshInSpaceVertexColor();
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( Mesh != null && Mesh.Result != null )
			{
				var meshBounds = Mesh.Result.SpaceBounds;
				var selectedLOD = GetSelectedLOD();

				//centering borders
				if( ProjectSettings.Get.General.ShowCenteringBorders )
				{
					double v = 0.8;

					var offsetX = v * 0.5 * Viewport.CanvasRenderer.AspectRatioInv;
					var offsetY = v * 0.5;

					var r = new Rectangle( new Vector2( 0.5, 0.5 ) );
					r.Expand( new Vector2( offsetX, offsetY ) );
					Viewport.CanvasRenderer.AddRectangle( r, new ColorValue( 1, 1, 1, 0.05 ) );
				}

				//center axes
				if( Mesh.EditorDisplayPivot )
				{
					var sizeInPixels = 35 * EditorAPI2.DPIScale;
					var size = Viewport.Simple3DRenderer.GetThicknessByPixelSize( Vector3.Zero, sizeInPixels );
					//var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					//var size = Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 20;

					var thickness = size / 20;
					var headHeight = size / 3;
					var headRadius = headHeight / 3;
					//!!!!рисовать с прозрачность но в режиме чтобы не перекрывалось
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

				////vertex color
				//if( Mesh.EditorDisplayVertexColor )
				//{
				//	var extractedVertices = selectedLOD.GetExtractedVertices( true );

				//	var vertices = new Simple3DRenderer.Vertex[ extractedVertices.Length ];
				//	for( int n = 0; n < vertices.Length; n++ )
				//	{
				//		ref var sourceVertex = ref extractedVertices[ n ];

				//		var vertex = new Simple3DRenderer.Vertex();
				//		vertex.Position = sourceVertex.Position;

				//		vertex.Color = RenderingSystem.ConvertColorValue( ref sourceVertex.Color );
				//		//vertex.color = sourceVertex.Color;

				//		//vertex.colorInvisibleBehindObjects = sourceVertex.color;
				//		vertices[ n ] = vertex;
				//	}

				//	Viewport.Simple3DRenderer.AddTriangles( vertices, selectedLOD.ExtractedIndices, false, true );
				//}

				////clusters
				//if( Mesh.EditorDisplayClusters )
				//{
				//	foreach( var geometry in Mesh.GetComponents<MeshGeometry>() )
				//	{
				//		//var vertices = geometry.Vertices.Value;
				//		var indices = geometry.Indices.Value;
				//		//var clusterData = geometry.ClusterData.Value;

				//		//if( /*vertices != null && indices != null && */clusterData != null )
				//		//{
				//		var clustersInfo = geometry.ExtractClustersInfo();
				//		if( clustersInfo != null )
				//		{
				//			var vertices = geometry.VerticesExtractChannelVector3F( VertexElementSemantic.Position );
				//			if( vertices != null && indices != null )
				//			{
				//				var yellow = new ColorValue( 1, 1, 0 );
				//				var red = new ColorValue( 1, 0, 0 );
				//				var green = new ColorValue( 0, 1, 0 );
				//				var gray = new ColorValue( 0.75, 0.75, 0.75 );
				//				var white = new ColorValue( 1, 1, 1 );
				//				//var blue = new ColorValue( 0, 0, 1 );
				//				var colors = new ColorValue[] { yellow, red, green, gray, white, new ColorValue( 0, 1, 1 ), new ColorValue( 1, 0, 1 ) };

				//				for( int nCluster = 0; nCluster < clustersInfo.Length; nCluster++ )
				//				{
				//					ref var clusterInfo = ref clustersInfo[ nCluster ];

				//					var clusterTriangles = new List<Vector3F>( clusterInfo.TriangleCount * 3 );
				//					for( int n = 0; n < clusterInfo.TriangleCount; n++ )
				//					{
				//						var index0 = indices[ ( clusterInfo.TriangleStartOffset + n ) * 3 + 0 ];
				//						var index1 = indices[ ( clusterInfo.TriangleStartOffset + n ) * 3 + 1 ];
				//						var index2 = indices[ ( clusterInfo.TriangleStartOffset + n ) * 3 + 2 ];

				//						clusterTriangles.Add( vertices[ index0 ] );
				//						clusterTriangles.Add( vertices[ index1 ] );
				//						clusterTriangles.Add( vertices[ index2 ] );
				//					}

				//					//var clusterTriangles = new List<Vector3F>( clusterInfo.IndexCount );
				//					//for( int n = 0; n < clusterInfo.IndexCount; n++ )
				//					//{
				//					//	var index = indices[ clusterInfo.IndexStartOffset + n ];
				//					//	clusterTriangles.Add( vertices[ index ] );
				//					//}

				//					var color = colors[ nCluster % colors.Length ];
				//					color.Alpha *= 0.7f;

				//					var color1 = RenderingSystem.ConvertColorValue( color );
				//					var color2 = RenderingSystem.ConvertColorValue( color * new ColorValue( 0.75f, 0.75f, 0.75f ) );

				//					var gridSize = clusterInfo.GridSize;

				//					var vertices2 = new Simple3DRenderer.Vertex[ gridSize.X * gridSize.Y * 6 ];

				//					zzzzz;
				//					for( int n = 0; n < vertices2.Length; n++ )
				//					{
				//						zzz;

				//						ref var sourceVertex = ref extractedVertices[ n ];

				//						var vertex = new Simple3DRenderer.Vertex();
				//						vertex.Position = sourceVertex.Position;

				//						zzz;
				//						vertex.Color = color1;
				//						vertex.ColorInvisibleBehindObjects = zzzz;
				//						//vertex.color = sourceVertex.Color;

				//						//vertex.colorInvisibleBehindObjects = sourceVertex.color;
				//						vertices2[ n ] = vertex;
				//					}

				//					zz;

				//					for( int y = 0; y < gridSize.Y; y++ )
				//					{
				//						for( int x = 0; x < gridSize.X; x++ )
				//						{
				//							zzzz;
				//						}
				//					}

				//					zzzz;

				//					Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				//					Viewport.Simple3DRenderer.AddTriangles( clusterTriangles, Matrix4.Identity, false, true );

				//					//Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				//					//Viewport.Simple3DRenderer.AddTriangles( clusterTriangles, Matrix4.Identity, false, true );
				//				}
				//			}
				//		}
				//		//}
				//	}
				//}

				//triangles
				if( Mesh.EditorDisplayTriangles )
				{
					//!!!!Mesh.CompiledData
					var selectedMesh = selectedLOD.Owner;

					if( selectedMesh != null )
					{
						if( cachedTriangles == null || cachedTrianglesForMesh != selectedMesh )
						{
							cachedTriangles = null;
							cachedTrianglesForMesh = selectedMesh;
							cachedTrianglesCameraPositionForSort = new Vector3( double.MaxValue, 0, 0 );

							var lines = new ESet<Line3F>( 16384 );

							//if( selectedMesh.ContainsClusterData() )
							//{
							foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
							{
								if( geometry.ExtractActualGeometry( out var vertices, out var indices, out var error ) )
								{
									if( !string.IsNullOrEmpty( error ) )
										Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									for( int tri = 0; tri < indices.Length / 3; tri++ )
									{
										var index0 = indices[ tri * 3 + 0 ];
										var index1 = indices[ tri * 3 + 1 ];
										var index2 = indices[ tri * 3 + 2 ];

										{
											var line = new Line3F( vertices[ index0 ].Position, vertices[ index1 ].Position );
											for( int n = 0; n < 3; n++ )
											{
												if( line.Start[ n ] > line.End[ n ] )
												{
													var q = line.Start;
													line.Start = line.End;
													line.End = q;
												}
											}
											lines.AddWithCheckAlreadyContained( line );
										}

										{
											var line = new Line3F( vertices[ index1 ].Position, vertices[ index2 ].Position );
											for( int n = 0; n < 3; n++ )
											{
												if( line.Start[ n ] > line.End[ n ] )
												{
													var q = line.Start;
													line.Start = line.End;
													line.End = q;
												}
											}
											lines.AddWithCheckAlreadyContained( line );
										}

										{
											var line = new Line3F( vertices[ index2 ].Position, vertices[ index0 ].Position );
											for( int n = 0; n < 3; n++ )
											{
												if( line.Start[ n ] > line.End[ n ] )
												{
													var q = line.Start;
													line.Start = line.End;
													line.End = q;
												}
											}
											lines.AddWithCheckAlreadyContained( line );
										}
									}
								}
							}
							//}
							//else
							//{
							//	if( selectedLOD.ExtractedIndices != null )
							//	{
							//		var vertices = selectedLOD.ExtractedVerticesPositions;
							//		var indices = selectedLOD.ExtractedIndices;

							//		for( int tri = 0; tri < indices.Length / 3; tri++ )
							//		{
							//			var index0 = indices[ tri * 3 + 0 ];
							//			var index1 = indices[ tri * 3 + 1 ];
							//			var index2 = indices[ tri * 3 + 2 ];

							//			{
							//				var line = new Line3( vertices[ index0 ], vertices[ index1 ] );
							//				for( int n = 0; n < 3; n++ )
							//				{
							//					if( line.Start[ n ] > line.End[ n ] )
							//					{
							//						var q = line.Start;
							//						line.Start = line.End;
							//						line.End = q;
							//					}
							//				}
							//				lines.AddWithCheckAlreadyContained( line );
							//			}

							//			{
							//				var line = new Line3( vertices[ index1 ], vertices[ index2 ] );
							//				for( int n = 0; n < 3; n++ )
							//				{
							//					if( line.Start[ n ] > line.End[ n ] )
							//					{
							//						var q = line.Start;
							//						line.Start = line.End;
							//						line.End = q;
							//					}
							//				}
							//				lines.AddWithCheckAlreadyContained( line );
							//			}

							//			{
							//				var line = new Line3( vertices[ index2 ], vertices[ index0 ] );
							//				for( int n = 0; n < 3; n++ )
							//				{
							//					if( line.Start[ n ] > line.End[ n ] )
							//					{
							//						var q = line.Start;
							//						line.Start = line.End;
							//						line.End = q;
							//					}
							//				}
							//				lines.AddWithCheckAlreadyContained( line );
							//			}
							//		}
							//	}
							//}

							cachedTriangles = lines.ToArray();
						}

						if( cachedTriangles != null )
						{
							//!!!!цвета настраивать в опциях
							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.7 ) );

							//sort by distance
							if( cachedTriangles.Length > 100000 && cachedTrianglesCameraPositionForSort != Viewport.CameraSettings.Position )
							{
								cachedTrianglesCameraPositionForSort = Viewport.CameraSettings.Position;

								var cameraPositionF = Viewport.CameraSettings.Position.ToVector3F();

								unsafe
								{
									fixed( Line3F* pCachedTriangles = cachedTriangles )
									{
										CollectionUtility.MergeSortUnmanaged( pCachedTriangles, cachedTriangles.Length, delegate ( Line3F* line1, Line3F* line2 )
										{
											var distanceSquared1 = ( line1->Start - cameraPositionF ).LengthSquared();
											var distanceSquared2 = ( line2->Start - cameraPositionF ).LengthSquared();
											//var distanceSquared1 = ( ( line1->Start + line1->End ) * 0.5f - cameraPositionF ).LengthSquared();
											//var distanceSquared2 = ( ( line2->Start + line2->End ) * 0.5f - cameraPositionF ).LengthSquared();
											if( distanceSquared1 < distanceSquared2 )
												return -1;
											if( distanceSquared1 > distanceSquared2 )
												return 1;
											return 0;
										}, true );
									}
								}
							}

							var count = Math.Min( cachedTriangles.Length, 100000 );
							for( int n = 0; n < count; n++ )
							{
								ref var line = ref cachedTriangles[ n ];
								Viewport.Simple3DRenderer.AddLineThin( line.Start, line.End );
							}


							//var maxCount = 100000;

							//if( cachedTrianglesSortCounter > 0 )
							//	cachedTrianglesSortCounter--;

							//if( cachedTriangles.Length > maxCount && cachedTrianglesSortCounter <= 0 )
							//{
							//	var cameraPosition = Viewport.CameraSettings.Position;

							//	CollectionUtility.MergeSort( cachedTriangles, delegate ( Line3F l1, Line3F l2 )
							//	{
							//		var min1 = Math.Min( ( l1.Start - cameraPosition ).LengthSquared(), ( l1.End - cameraPosition ).LengthSquared() );
							//		var min2 = Math.Min( ( l2.Start - cameraPosition ).LengthSquared(), ( l2.End - cameraPosition ).LengthSquared() );

							//		if( min1 < min2 )
							//			return -1;
							//		if( min1 > min2 )
							//			return 1;
							//		return 0;
							//	}, true );

							//	cachedTrianglesSortCounter = 100;
							//}

							//for( int n = 0; n < cachedTriangles.Length && n < maxCount; n++ )
							//{
							//	ref var line = ref cachedTriangles[ n ];
							//	Viewport.Simple3DRenderer.AddLineThin( line.Start, line.End );
							//}

							////Viewport.Simple3DRenderer.AddTriangles( selectedLOD.ExtractedVerticesPositions, selectedLOD.ExtractedIndices, true, false );
						}
					}
				}

				////vertices
				//if( Mesh.EditorDisplayVertices )
				//{
				//	var size3 = meshBounds.CalculatedBoundingBox.GetSize();
				//	var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 200;
				//	Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 0.7 ) );
				//	foreach( var vertex in selectedLOD.ExtractedVerticesPositions )
				//	{
				//		Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( scale, 0, 0 ), vertex + new Vector3F( scale, 0, 0 ) );
				//		Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( 0, scale, 0 ), vertex + new Vector3F( 0, scale, 0 ) );
				//		Viewport.Simple3DRenderer.AddLineThin( vertex - new Vector3F( 0, 0, scale ), vertex + new Vector3F( 0, 0, scale ) );
				//	}
				//}

				//normals
				if( Mesh.EditorDisplayNormals )
				{
					//!!!!Mesh.CompiledData
					var selectedMesh = selectedLOD.Owner;

					if( selectedMesh != null )
					{
						if( cachedNormals == null || cachedNormalsForMesh != selectedMesh )
						{
							var size3 = meshBounds.BoundingBox.GetSize();
							var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;

							cachedNormals = null;
							cachedNormalsForMesh = selectedMesh;

							var lines = new ESet<Line3>( 16384 );

							foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
							{
								if( geometry.ExtractActualGeometry( out var vertices, out var indices, out var error ) )
								{
									if( !string.IsNullOrEmpty( error ) )
										Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//	var items = geometry.ExtractActualGeometry( true, out var error );
									//if( !string.IsNullOrEmpty( error ) )
									//	Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//foreach( var item in items )
									//{
									//var vertices = item.Vertices;

									for( int n = 0; n < vertices.Length; n++ )
									{
										ref var v = ref vertices[ n ];

										if( v.Normal != Vector3F.Zero )
										{
											var line = new Line3( v.Position, v.Position + v.Normal * scale );
											lines.AddWithCheckAlreadyContained( line );
										}
									}
								}
							}

							cachedNormals = lines.ToArray();
						}

						if( cachedNormals != null )
						{
							//!!!!цвета настраивать в опциях
							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, 0.7 ) );
							for( int n = 0; n < cachedNormals.Length; n++ )
								Viewport.Simple3DRenderer.AddLineThin( cachedNormals[ n ] );
						}
					}

					//var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					//var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					//Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, 0.7 ) );
					//foreach( var vertex in selectedLOD.GetExtractedVertices( true ) )
					//{
					//	if( vertex.Normal != Vector3F.Zero )
					//		Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + vertex.Normal * scale );
					//}
				}

				//tangents
				if( Mesh.EditorDisplayTangents )
				{
					//!!!!Mesh.CompiledData
					var selectedMesh = selectedLOD.Owner;

					if( selectedMesh != null )
					{
						if( cachedTangents == null || cachedTangentsForMesh != selectedMesh )
						{
							var size3 = meshBounds.BoundingBox.GetSize();
							var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;

							cachedTangents = null;
							cachedTangentsForMesh = selectedMesh;

							var lines = new ESet<Line3>( 16384 );

							foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
							{
								if( geometry.ExtractActualGeometry( out var vertices, out var indices, out var error ) )
								{
									if( !string.IsNullOrEmpty( error ) )
										Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//var items = geometry.ExtractActualGeometry( true, out var error );
									//if( !string.IsNullOrEmpty( error ) )
									//	Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//foreach( var item in items )
									//{
									//	var vertices = item.Vertices;

									for( int n = 0; n < vertices.Length; n++ )
									{
										ref var v = ref vertices[ n ];

										if( v.Tangent != Vector4F.Zero )
										{
											var line = new Line3( v.Position, v.Position + v.Tangent.ToVector3F() * scale );
											lines.AddWithCheckAlreadyContained( line );
										}
									}
								}
							}

							cachedTangents = lines.ToArray();
						}

						if( cachedTangents != null )
						{
							//!!!!цвета настраивать в опциях
							Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 0.7 ) );
							for( int n = 0; n < cachedTangents.Length; n++ )
								Viewport.Simple3DRenderer.AddLineThin( cachedTangents[ n ] );
						}
					}

					//var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					//var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					//Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0, 0.7 ) );
					//foreach( var vertex in selectedLOD.GetExtractedVertices( true ) )
					//{
					//	if( vertex.Tangent != Vector4F.Zero )
					//		Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + vertex.Tangent.ToVector3F() * scale );
					//}
				}

				//binormals
				if( Mesh.EditorDisplayBinormals )
				{
					//!!!!Mesh.CompiledData
					var selectedMesh = selectedLOD.Owner;

					if( selectedMesh != null )
					{
						if( cachedBinormals == null || cachedBinormalsForMesh != selectedMesh )
						{
							var size3 = meshBounds.BoundingBox.GetSize();
							var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;

							cachedBinormals = null;
							cachedBinormalsForMesh = selectedMesh;

							var lines = new ESet<Line3>( 16384 );

							foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
							{
								if( geometry.ExtractActualGeometry( out var vertices, out var indices, out var error ) )
								{
									if( !string.IsNullOrEmpty( error ) )
										Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//var items = geometry.ExtractActualGeometry( true, out var error );
									//if( !string.IsNullOrEmpty( error ) )
									//	Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

									//foreach( var item in items )
									//{
									//	var vertices = item.Vertices;

									for( int n = 0; n < vertices.Length; n++ )
									{
										ref var v = ref vertices[ n ];

										if( v.Normal != Vector3F.Zero && v.Tangent != Vector4F.Zero )
										{
											var v2 = Vector3.Cross( v.Tangent.ToVector3F(), v.Normal ) * v.Tangent.W;
											var line = new Line3( v.Position, v.Position + v2 * scale );
											lines.AddWithCheckAlreadyContained( line );
										}
									}
								}
							}

							cachedBinormals = lines.ToArray();
						}

						if( cachedBinormals != null )
						{
							//!!!!цвета настраивать в опциях
							Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.7 ) );
							for( int n = 0; n < cachedBinormals.Length; n++ )
								Viewport.Simple3DRenderer.AddLineThin( cachedBinormals[ n ] );
						}
					}

					//var size3 = meshBounds.CalculatedBoundingBox.GetSize();
					//var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 30;
					//Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1, 0.7 ) );
					//foreach( var vertex in selectedLOD.GetExtractedVertices( true ) )
					//{
					//	if( vertex.Normal != Vector3F.Zero && vertex.Tangent != Vector4F.Zero )
					//	{
					//		var v = Vector3.Cross( vertex.Tangent.ToVector3F(), vertex.Normal ) * vertex.Tangent.W;
					//		Viewport.Simple3DRenderer.AddLineThin( vertex.Position, vertex.Position + v * scale );
					//	}
					//}
				}

				//collision
				if( Mesh.EditorDisplayCollision )
				{
					var collision = Mesh.GetComponent( "Collision Definition" ) as RigidBody;
					if( collision != null )
					{
						ColorValue color = new ColorValue( 0, 0, 1, 0.7 );
						//if( MotionType.Value == MotionTypeEnum.Static )
						//	color = ProjectSettings.Get.SceneShowPhysicsStaticColor;
						//else if( rigidBody.IsActive )
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicInactiveColor;
						Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

						int verticesRendered = 0;
						foreach( var shape in collision.GetComponents<CollisionShape>() )
						{
							if( shape.Enabled )
								shape.Render( Viewport, Transform.Identity, false, ref verticesRendered );
						}
					}
				}

				//update PlayAnimation
				if( !string.IsNullOrEmpty( Mesh.EditorPlayAnimation ) )
				{
					var animation = Mesh.ParentRoot.GetComponentByPath( Mesh.EditorPlayAnimation ) as Animation;
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
						if( skeletonAnimationController.RenderSkeleton( Viewport, SelectedObjectsSet ) )
						{
						}
						//var skeletonArrows = skeletonAnimationController.GetCurrentAnimatedSkeletonArrows();
						//if( skeletonArrows != null )
						//{
						//	var color = new ColorValue( 0, 0.5, 1, 0.7 );
						//	Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

						//	foreach( var arrow in skeletonArrows )
						//		Viewport.Simple3DRenderer.AddArrow( arrow.Start, arrow.End );
						//}
						else
						{
							//!!!!axes

							foreach( var bone in skeleton.GetBones() )
							{
								var tr = bone.Transform.Value;

								var pos = tr.Position;
								var parent = bone.Parent as SkeletonBone;
								if( parent != null )
								{
									var from = parent.Transform.Value.Position;

									var isSelected = SelectedObjectsSet.Contains( bone );

									ColorValue color;
									if( isSelected )
										color = new ColorValue( 0, 1, 0 );
									else
										color = new ColorValue( 0, 0.5, 1, 0.7 );
									Viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

									Viewport.Simple3DRenderer.AddArrow( from, pos );

									if( isSelected )
									{
										var length = ( pos - from ).Length() / 3;

										Viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ), new ColorValue( 1, 0, 0 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
										Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( length, 0, 0 ) );

										Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0 ), new ColorValue( 0, 1, 0 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
										Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( 0, length, 0 ) );

										Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 1 ), new ColorValue( 0, 0, 1 ) * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
										Viewport.Simple3DRenderer.AddArrow( pos, pos + tr.Rotation * new Vector3( 0, 0, length ) );
									}
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

					if( meshBounds.BoundingBoxOriginal )
						Viewport.Simple3DRenderer.AddBounds( meshBounds.BoundingBox );
					if( meshBounds.BoundingSphereOriginal )
						Viewport.Simple3DRenderer.AddSphere( meshBounds.BoundingSphere );

					//if( meshBounds.BoundingBox.HasValue )
					//	Viewport.Simple3DRenderer.AddBounds( meshBounds.BoundingBox.Value );
					//if( meshBounds.BoundingSphere.HasValue )
					//	Viewport.Simple3DRenderer.AddSphere( meshBounds.BoundingSphere.Value );
				}

				//select triangle
				Mesh.CompiledData.RayCastResult triangleIntersectResult = null;
				//int triangleIndex = -1;
				if( !Viewport.MouseRelativeMode )
				{
					var ray = Viewport.CameraSettings.GetRayByScreenCoordinates( Viewport.MousePosition );

					triangleIntersectResult = selectedLOD.RayCast( ray, Mesh.CompiledData.RayCastModes.Auto, false );
					//if( selectedLOD.RayCast( ray, Mesh.CompiledData.RayCastMode.Auto, false, out _, out _, out int triangleIndex2 ) )
					//	triangleIndex = triangleIndex2;
				}

				//selected triangle data
				if( triangleIntersectResult != null )// triangleIndex != -1 )
				{
					var v0 = new StandardVertex();
					var v1 = new StandardVertex();
					var v2 = new StandardVertex();

					if( triangleIntersectResult.ContainsTriangleInfo )
					{
						var vertices = selectedLOD.GetExtractedVertices( true );
						var indices = selectedLOD.ExtractedIndices;

						int index0 = indices[ triangleIntersectResult.TriangleIndex * 3 + 0 ];
						int index1 = indices[ triangleIntersectResult.TriangleIndex * 3 + 1 ];
						int index2 = indices[ triangleIntersectResult.TriangleIndex * 3 + 2 ];
						v0 = vertices[ index0 ];
						v1 = vertices[ index1 ];
						v2 = vertices[ index2 ];
						//var showVertices = new int[] { index0, index1, index2 };
						//p0 = vertices[ index0 ].Position;
						//p1 = vertices[ index1 ].Position;
						//p2 = vertices[ index2 ].Position;
					}
					else if( triangleIntersectResult.ContainsVertexInfo )
					{
						v0 = triangleIntersectResult.Vertex0;
						v1 = triangleIntersectResult.Vertex1;
						v2 = triangleIntersectResult.Vertex2;
					}

					var p0 = v0.Position;
					var p1 = v1.Position;
					var p2 = v2.Position;

					var showVertices = new StandardVertex[] { v0, v1, v2 };

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
						Viewport.Simple3DRenderer.AddTriangles( new Vector3[] { p0, p1, p2 }, false, false );
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
					foreach( var v in showVertices )//foreach( int nVertex in showVertices )
					{
						//var v = vertices[ nVertex ];
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

						foreach( var v in showVertices )//foreach( int nVertex in showVertices )
						{
							//var v = vertices[ nVertex ];
							Vector3F p = v.Position;

							Viewport.Simple3DRenderer.AddSphere( new Sphere( p, vertexRadius ), 32, true );
							//viewport.DebugGeometry.AddTriangles( spherePositions, sphereIndices, Mat4.FromTranslate( p ), false, false );
						}
					}
				}

				//!!!!
				//highlight selected mesh geometries, materials
				if( GetSelectedLODIndex() == 0 )
				{
					var geometriesToHighlight = new ESet<(MeshGeometry, int)>();
					foreach( var geometry in Mesh.GetComponents<MeshGeometry>() )// checkChildren: true ) )
					{
						if( SelectedObjectsSet.Contains( geometry ) )
							geometriesToHighlight.AddWithCheckAlreadyContained( (geometry, -1) );

						var material = geometry.Material.Value;
						if( material != null )
						{
							if( SelectedObjectsSet.Contains( material ) )
								geometriesToHighlight.AddWithCheckAlreadyContained( (geometry, -1) );

							var multiMaterial = material as MultiMaterial;
							if( multiMaterial != null )
							{
								for( int n = 0; n < multiMaterial.Materials.Count; n++ )
								{
									var m = multiMaterial.Materials[ n ].Value;
									if( m != null && SelectedObjectsSet.Contains( m ) )
										geometriesToHighlight.AddWithCheckAlreadyContained( (geometry, n) );
								}
							}
						}
					}

					foreach( var item in geometriesToHighlight )
					{
						var geometry = item.Item1;
						var materialIndex = item.Item2;

						var structure = geometry.VertexStructure.Value;
						var vertices = geometry.Vertices.Value;
						var indices = geometry.Indices.Value;
						var virtualizedData = (byte[])null;// geometry.VirtualizedData.Value;

						try
						{
							if( structure != null && vertices != null & indices != null )
							{
								if( materialIndex != -1 )
								{
									var ranges = GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData( structure, vertices, indices, virtualizedData );
									if( ranges != null && materialIndex < ranges.Length )
									{
										var range = ranges[ materialIndex ];

										var indices2 = new int[ range.Maximum - range.Minimum ];
										Array.Copy( indices, range.Minimum, indices2, 0, indices2.Length );
										indices = indices2;
									}
								}

								structure.GetInfo( out var vertexSize, out _ );
								var vertexCount = vertices.Length / vertexSize;

								if( structure.GetElementBySemantic( VertexElementSemantic.Position, out var element ) && element.Type == VertexElementType.Float3 )
								{
									if( vertices != null && indices != null )
									{
										var positions = ExtractChannel<Vector3F>( vertices, vertexSize, vertexCount, element.Offset );

										Viewport.Simple3DRenderer.SetColor( ProjectSettings.Get.Colors.SelectedColor );
										Viewport.Simple3DRenderer.AddTriangles( positions, indices, Matrix4.Identity, true, true );
									}
								}
							}
						}
						catch( Exception e )
						{
							Log.Warning( e.Message );
						}
					}
				}

				//update selected LOD
				if( Scene.RenderingPipeline.Value != null )
				{
					if( Mesh.EditorDisplayLOD != -1 )
						Scene.RenderingPipeline.Value.LODRange = new RangeI( Mesh.EditorDisplayLOD, Mesh.EditorDisplayLOD );
					else
						Scene.RenderingPipeline.Value.LODRange = new RangeI( 0, 10 );
				}

				if( meshInSpace != null )
					meshInSpace.Visible = /*meshInSpaceClusterBounds == null && meshInSpaceClusterTriangles == null &&*/ meshInSpaceVertexColor == null;
				//if( meshInSpace != null )
				//	meshInSpace.Visible = /*!Mesh.EditorDisplayVoxelized ||*/ meshInSpaceVoxels == null;
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
			return EditorLocalization2.Translate( "MeshDocumentWindow", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( Mesh != null && Mesh.Result != null )
			{
				var lodCount = 1;

				var lods = Mesh.Result?.MeshData?.LODs;
				if( lods != null )
					lodCount = lods.Length + 1;

				var triangles = 0;
				var vertices = 0;
				{
					var data = Mesh.Result;//.GetLOD( lodLevel );
					if( data != null && data.MeshData != null )
					{
						foreach( var op in data.MeshData.RenderOperations )
						{
							//if( MeshGeometry.GetVirtualizedDataInfo( op.VirtualizedData, out var header ) )
							//{
							//	triangles += header.TriangleCount;
							//	vertices += header.VertexCount;
							//}
							//else
							{
								triangles += op.IndexCount / 3;
								vertices += op.VertexCount;
							}

							//if( op.VirtualizedData != null )
							//{
							//if( MeshGeometry.GetClusterInfo( op.VirtualizedData, out var header, out var clustersInfo ) )
							//{
							//	for( int n = 0; n < clustersInfo.Length; n++ )
							//	{
							//		ref var clusterInfo = ref clustersInfo[ n ];

							//		if( ( clusterInfo.Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) == 0 )
							//		{
							//			vertices += clusterInfo.ActualVertexCount;
							//			triangles += clusterInfo.ActualTriangleCount;
							//		}
							//		else
							//		{
							//			vertices += clusterInfo.ActualVertexCount;
							//			triangles += clusterInfo.ActualTriangleCount;
							//		}
							//	}
							//}
							//}
							//else
							//{
							//	triangles += op.IndexCount / 3;
							//	vertices += op.VertexCount;
							//}
						}
					}
				}

				var memory = 0L;
				var gpuMemory = 0L;
				{
					for( int lodLevel = 0; lodLevel < lodCount; lodLevel++ )
					{
						var data = Mesh.Result.GetLOD( lodLevel );
						if( data != null && data.MeshData != null )
						{
							foreach( var op in data.MeshData.RenderOperations )
							{
								if( op.VertexBuffers != null )
								{
									foreach( var buffer in op.VertexBuffers )
									{
										if( buffer.Vertices != null )
											memory += buffer.Vertices.Length;
										buffer.UpdateNativeObject();
										if( buffer.NativeObjectCreated )
											gpuMemory += buffer.VertexSize * buffer.VertexCount;
									}
								}

								if( op.IndexBuffer != null )
								{
									var buffer = op.IndexBuffer;
									if( buffer.Indices != null )
										memory += op.IndexBuffer.Indices.Length * 4;
									buffer.UpdateNativeObject();
									if( buffer.NativeObjectCreated )
										gpuMemory += buffer.IndexCount * ( buffer.NativeObject16Bit ? 2 : 4 );
								}

								if( op.VoxelDataImage != null )
								{
									var texture = op.VoxelDataImage.Result;

									if( texture != null )
									{
										var data2 = texture.GetData();
										if( data2 != null )
										{
											foreach( var surfaceData in data2 )
											{
												if( surfaceData.Data != null )
													memory += surfaceData.Data.Count;
											}
										}

										var realObject = texture.GetNativeObject( true );// false );
										if( realObject != null )
											gpuMemory += realObject.SizeInBytes;
									}
								}

								//if( op.VirtualizedDataImage != null )
								//{
								//	var texture = op.VirtualizedDataImage.Result;

								//	if( texture != null )
								//	{
								//		var data2 = texture.GetData();
								//		if( data2 != null )
								//		{
								//			foreach( var surfaceData in data2 )
								//			{
								//				if( surfaceData.Data != null )
								//					memory += surfaceData.Data.Count;
								//			}
								//		}

								//		var realObject = texture.GetNativeObject( false );
								//		if( realObject != null )
								//			gpuMemory += realObject.SizeInBytes;
								//	}
								//}
							}
						}
					}

					foreach( var geometry in Mesh.GetComponents<MeshGeometry>() )
					{
						var voxelData = geometry.VoxelData.Value;
						if( voxelData != null )
							memory += voxelData.Length;

						//var virtualizedData = geometry.VirtualizedData.Value;
						//if( virtualizedData != null )
						//	memory += virtualizedData.Length;
					}
				}

				lines.Add( Translate( "Triangles" ) + ": " + triangles.ToString( "N0" ) );
				lines.Add( Translate( "Vertices" ) + ": " + vertices.ToString( "N0" ) );

				if( memory >= 1024 * 1024 )
					lines.Add( Translate( "Memory" ) + ": " + ( (double)memory / 1024 / 1024 ).ToString( "F1" ) + " MB" );
				else
					lines.Add( Translate( "Memory" ) + ": " + ( memory / 1024 ).ToString( "N0" ) + " KB" );

				if( gpuMemory >= 1024 * 1024 )
					lines.Add( Translate( "GPU memory" ) + ": " + ( (double)gpuMemory / 1024 / 1024 ).ToString( "F1" ) + " MB" );
				else
					lines.Add( Translate( "GPU memory" ) + ": " + ( gpuMemory / 1024 ).ToString( "N0" ) + " KB" );

				lines.Add( Translate( "Bounding box" ) + ": " + Mesh.Result.SpaceBounds.BoundingBox.GetSize().ToString( 3 ) );
				//lines.Add( Translate( "Bounding sphere radius" ) + ": " + Mesh.Result.SpaceBounds.BoundingSphere.Value.Radius.ToString( "N3" ) );
				//lines.Add( Translate( "Levels of detail" ) + ": " + lodCount.ToString() );

				for( int lodLevel = 0; lodLevel < lodCount; lodLevel++ )
				{
					//lines.Add( "" );
					//lines.Add( Translate( "LOD" ) + " " + lodLevel.ToString() );

					var data = Mesh.Result.GetLOD( lodLevel );
					if( data != null && data.MeshData != null )
					{
						var type = "";
						foreach( var op in data.MeshData.RenderOperations )
						{
							if( op.VoxelDataInfo != null )
								type = "Voxel";
							//if( op.VirtualizedDataImage != null )
							//	type = "Virtualized";
						}
						//if( type == "" )
						//	type = "Usual";

						lines.Add( "" );
						if( type != "" )
							lines.Add( Translate( "LOD" ) + " " + lodLevel.ToString() + " - " + Translate( type ) );
						else
							lines.Add( Translate( "LOD" ) + " " + lodLevel.ToString() );

						//lines.Add( Translate( "Type" ) + ": " + type );
						lines.Add( Translate( "Triangles" ) + ": " + ( data.ExtractedIndices.Length / 3 ).ToString( "N0" ) );
						lines.Add( Translate( "Vertices" ) + ": " + data.ExtractedVerticesPositions.Length.ToString( "N0" ) );
						//lines.Add( Translate( "Triangles to draw" ) + ": " + ( data.ExtractedIndices.Length / 3 ).ToString( "N0" ) );
						//lines.Add( Translate( "Vertices to draw" ) + ": " + data.ExtractedVerticesPositions.Length.ToString( "N0" ) );

						var nodeCount = 0;
						var actualTriangleCount = 0;
						var actualVertexCount = 0;
						//var clusterCount = 0;
						//var vertexCountClustered = 0;
						//var triangleCountClustered = 0;
						//var vertexCountSeparate = 0;
						//var triangleCountSeparate = 0;

						var voxels = "";

						if( data.MeshData != null )
						{
							foreach( var op in data.MeshData.RenderOperations )
							{
								//if( MeshGeometry.GetVirtualizedDataInfo( op.VirtualizedData, out var header ) )
								//{
								//	nodeCount += header.NodeCount;
								//	actualTriangleCount += header.TriangleCount;
								//	actualVertexCount += header.VertexCount;
								//}
								//else
								{
									actualTriangleCount += op.IndexCount / 3;
									actualVertexCount += op.VertexCount;
								}

								//if( op.VirtualizedData != null )
								//{
								//	if( MeshGeometry.GetClusterInfo( op.VirtualizedData, out var header, out var clustersInfo ) )
								//	{
								//		clusterCount += header.ClusterCount;

								//		for( int n = 0; n < clustersInfo.Length; n++ )
								//		{
								//			ref var clusterInfo = ref clustersInfo[ n ];

								//			if( ( clusterInfo.Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) == 0 )
								//			{
								//				vertexCountClustered += clusterInfo.ActualVertexCount;
								//				triangleCountClustered += clusterInfo.ActualTriangleCount;
								//			}
								//			else
								//			{
								//				vertexCountSeparate += clusterInfo.ActualVertexCount;
								//				triangleCountSeparate += clusterInfo.ActualTriangleCount;
								//			}
								//		}
								//	}
								//}

								if( op.VoxelDataInfo != null )
								{
									var gridSize = op.VoxelDataInfo[ 0 ].ToVector3F().ToVector3I();
									voxels = $"{gridSize.X} {gridSize.Y} {gridSize.Z}";
								}
							}
						}

						if( nodeCount != 0 )
						{
							lines.Add( Translate( "Triangles" ) + ": " + actualTriangleCount.ToString( "N0" ) );
							lines.Add( Translate( "Vertices" ) + ": " + actualVertexCount.ToString( "N0" ) );
							lines.Add( Translate( "Acceleration nodes" ) + ": " + nodeCount.ToString( "N0" ) );
						}

						//if( clusterCount != 0 )
						//{
						//	lines.Add( Translate( "Clusters" ) + ": " + clusterCount.ToString( "N0" ) );
						//	lines.Add( Translate( "Triangles" ) + ": " + ( triangleCountClustered + triangleCountSeparate ).ToString( "N0" ) );
						//	lines.Add( Translate( "Vertices" ) + ": " + ( vertexCountClustered + vertexCountSeparate ).ToString( "N0" ) );
						//	lines.Add( Translate( "Clustered triangles" ) + ": " + triangleCountClustered.ToString( "N0" ) );
						//	lines.Add( Translate( "Separate triangles" ) + ": " + triangleCountSeparate.ToString( "N0" ) );
						//}

						if( voxels != "" )
							lines.Add( Translate( "Voxels" ) + ": " + voxels );
					}
				}

				//var data = GetSelectedLOD();
				//if( data != null )
				//{
				//	lines.Add( Translate( "Level of detail" ) + ": " + GetSelectedLODIndex().ToString() );
				//	lines.Add( Translate( "Triangles" ) + ": " + ( data.ExtractedIndices.Length / 3 ).ToString( "N0" ) );
				//	lines.Add( Translate( "Vertices" ) + ": " + data.ExtractedVerticesPositions.Length.ToString( "N0" ) );
				//	lines.Add( Translate( "Render operations" ) + ": " + data.MeshData.RenderOperations.Count.ToString() );
				//	lines.Add( Translate( "Bounding box size" ) + ": " + data.SpaceBounds.BoundingBox.Value.GetSize().ToString( 3 ) );
				//	lines.Add( Translate( "Bounding sphere radius" ) + ": " + data.SpaceBounds.BoundingSphere.Value.Radius.ToString( "N3" ) );
				//}
			}
		}

		/////////////////////////////////////////

		void DrawUV()
		{
			var meshData = GetSelectedLOD();
			var selectedMesh = meshData?.Owner;

			if( selectedMesh != null && Mesh.EditorDisplayUV >= 0 && Mesh.EditorDisplayUV < 4 )
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

				StandardVertex.Components components = 0;
				//if( selectedMesh.ContainsVirtualizedData() )
				//{
				//	components |= StandardVertex.Components.TexCoord0;
				//	components |= StandardVertex.Components.TexCoord1;

				//	foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
				//	{
				//		if( geometry.VirtualizedData.Value != null && geometry.VirtualizedFormat == VertexFormatEnum.Full )
				//		{
				//			components |= StandardVertex.Components.TexCoord2;
				//			components |= StandardVertex.Components.TexCoord3;
				//		}

				//		//if( geometry.GetClusterInfo( out _, out var clustersInfo ) )
				//		//{
				//		//	for( int nCluster = 0; nCluster < clustersInfo.Length; nCluster++ )
				//		//	{
				//		//		ref var cluster = ref clustersInfo[ nCluster ];

				//		//		var fullFormat = ( cluster.Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;
				//		//		if( fullFormat )
				//		//		{
				//		//			components |= StandardVertex.Components.TexCoord2;
				//		//			components |= StandardVertex.Components.TexCoord3;
				//		//		}
				//		//	}
				//		//}
				//	}
				//}
				//else
				components = selectedMesh.Result.ExtractedVerticesComponents;

				if( ( components & component ) != 0 )//if( ( meshData.ExtractedVerticesComponents & component ) != 0 )
				{
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

						var resultVerticesUV = new OpenList<Vector2F>( 16384 );
						var resultIndices = new OpenList<int>( 16384 );

						foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
						{
							if( geometry.ExtractActualGeometry( out var vertices, out var indices, out var error ) )
							{
								if( !string.IsNullOrEmpty( error ) )
									Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

								//	var items = geometry.ExtractActualGeometry( true, out var error );
								//if( !string.IsNullOrEmpty( error ) )
								//	Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

								//foreach( var item in items )
								//{

								var startVertexIndex = resultVerticesUV.Count;

								for( int n = 0; n < vertices.Length; n++ )
								{
									ref var v = ref vertices[ n ];

									var texCoord = Vector2F.Zero;
									if( component == StandardVertex.Components.TexCoord0 )
										texCoord = v.TexCoord0;
									else if( component == StandardVertex.Components.TexCoord1 )
										texCoord = v.TexCoord1;
									else if( component == StandardVertex.Components.TexCoord2 )
										texCoord = v.TexCoord2;
									else if( component == StandardVertex.Components.TexCoord3 )
										texCoord = v.TexCoord3;

									resultVerticesUV.Add( texCoord );
								}

								foreach( var index in indices )
									resultIndices.Add( index + startVertexIndex );
							}
						}

						//!!!!может показывать те, которые за границами 0-1

						var uvArrayConverted = new Vector2F[ resultVerticesUV.Count ];
						for( int n = 0; n < uvArrayConverted.Length; n++ )
							uvArrayConverted[ n ] = Convert( resultVerticesUV[ n ] );

						var trianglesColor = new ColorValue( 0, 1, 0, 0.5 );
						var linesColor = new ColorValue( 1, 1, 1 );

						//triangles
						{
							var triangles = new List<CanvasRenderer.TriangleVertex>( resultIndices.Count / 3 );

							for( int triangle = 0; triangle < resultIndices.Count / 3; triangle++ )
							{
								int index0 = resultIndices[ triangle * 3 + 0 ];
								int index1 = resultIndices[ triangle * 3 + 1 ];
								int index2 = resultIndices[ triangle * 3 + 2 ];

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
							int[] lineIndices = MathAlgorithms.TriangleListToLineList( resultIndices.ToArray() );

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



						//var uvArray = (Vector2F[])StandardVertex.ExtractOneComponentArray( meshData.GetExtractedVertices( true ), component );
						//var indices = meshData.ExtractedIndices;

						////!!!!может показывать те, которые за границами 0-1

						//var uvArrayConverted = new Vector2F[ uvArray.Length ];
						//for( int n = 0; n < uvArrayConverted.Length; n++ )
						//	uvArrayConverted[ n ] = Convert( uvArray[ n ] );

						//var trianglesColor = new ColorValue( 0, 1, 0, 0.5 );
						//var linesColor = new ColorValue( 1, 1, 1 );

						////triangles
						//{
						//	var triangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 3 );

						//	for( int triangle = 0; triangle < indices.Length / 3; triangle++ )
						//	{
						//		int index0 = indices[ triangle * 3 + 0 ];
						//		int index1 = indices[ triangle * 3 + 1 ];
						//		int index2 = indices[ triangle * 3 + 2 ];

						//		Vector2F v0 = uvArrayConverted[ index0 ];
						//		Vector2F v1 = uvArrayConverted[ index1 ];
						//		Vector2F v2 = uvArrayConverted[ index2 ];

						//		triangles.Add( new CanvasRenderer.TriangleVertex( v0, trianglesColor ) );
						//		triangles.Add( new CanvasRenderer.TriangleVertex( v1, trianglesColor ) );
						//		triangles.Add( new CanvasRenderer.TriangleVertex( v2, trianglesColor ) );
						//	}

						//	cachedDisplayUV_Triangles = triangles.ToArray();
						//}

						////lines
						//{
						//	int[] lineIndices = MathAlgorithms.TriangleListToLineList( indices );

						//	var lines = new List<CanvasRenderer.LineItem>( lineIndices.Length / 2 );
						//	for( int nLine = 0; nLine < lineIndices.Length / 2; nLine++ )
						//	{
						//		var v0 = uvArrayConverted[ lineIndices[ nLine * 2 + 0 ] ];
						//		var v1 = uvArrayConverted[ lineIndices[ nLine * 2 + 1 ] ];
						//		lines.Add( new CanvasRenderer.LineItem( v0, v1, linesColor ) );
						//	}

						//	var vertices = new CanvasRenderer.TriangleVertex[ lines.Count * 2 ];
						//	for( int n = 0; n < lines.Count; n++ )
						//	{
						//		var line = lines[ n ];
						//		vertices[ n * 2 + 0 ] = new CanvasRenderer.TriangleVertex( line.start, line.color, Vector2F.Zero );
						//		vertices[ n * 2 + 1 ] = new CanvasRenderer.TriangleVertex( line.end, line.color, Vector2F.Zero );
						//	}

						//	cachedDisplayUV_Lines = vertices;
						//}
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

		//struct Voxel
		//{
		//	public Vector3I index;
		//	public Vector3F normal;
		//	public Vector2F texCoord;

		//	//public float distanceSquared;
		//	//public uint color;
		//	////public ColorValue color;
		//	//public Vector3I index;
		//	////public BoundsF bounds;
		//}

		//unsafe void CreateMeshInSpaceVoxels()
		//{
		//	DestroyMeshInSpaceVoxels();

		//	//RenderingPipeline.RenderSceneData.MeshDataRenderOperation oper = null;

		//	//var meshBounds = Mesh.Result.SpaceBounds;
		//	//var selectedLOD = GetSelectedLOD();

		//	//if( selectedLOD != null )
		//	//{
		//	//	foreach( var op in selectedLOD.MeshData.RenderOperations )
		//	//	{
		//	//		if( op.VoxelDataInfo != null )
		//	//		{
		//	//			oper = op;
		//	//			break;
		//	//		}
		//	//	}
		//	//}

		//	MeshGeometry geometry2 = null;
		//	foreach( var g in Mesh.GetComponents<MeshGeometry>( checkChildren: true ) )
		//	{
		//		if( g.VoxelData.Value != null )
		//		{
		//			geometry2 = g;
		//			break;
		//		}
		//	}
		//	if( geometry2 == null )
		//		return;


		//	var data = geometry2.VoxelData.Value;

		//	if( data == null || data.Length <= sizeof( MeshGeometry.VoxelDataHeader ) )
		//		return;

		//	fixed( byte* pData = data )
		//	{
		//		var header = new MeshGeometry.VoxelDataHeader();
		//		NativeUtility.CopyMemory( &header, pData, sizeof( MeshGeometry.VoxelDataHeader ) );

		//		var gpuSize = data.Length - sizeof( MeshGeometry.VoxelDataHeader );

		//		if( header.Version == 2 && gpuSize != 0 )
		//		{
		//			var gridSize = header.GridSize;
		//			var cellSize = header.CellSize;
		//			var boundsMin = header.BoundsMin;
		//			var boundsMax = header.BoundsMin + gridSize.ToVector3F() * cellSize;

		//			float* pDataBody = (float*)( pData + sizeof( MeshGeometry.VoxelDataHeader ) );

		//			//!!!!
		//			//var elements = voxelized.GetComponents<MeshVoxelizedElement>();

		//			//!!!!
		//			var material = geometry2.Material.Value;


		//			//!!!!
		//			var lists = new List<Voxel>[ 1/*elements.Length*/ ];
		//			for( int n = 0; n < 1/*elements.Length*/; n++ )
		//				lists[ n ] = new List<Voxel>( 256 );
		//			//var list = new List<Voxel>( voxelCount );


		//			for( int z = 0; z < gridSize.Z; z++ )
		//			{
		//				for( int y = 0; y < gridSize.Y; y++ )
		//				{
		//					for( int x = 0; x < gridSize.X; x++ )
		//					{
		//						var index = ( z * gridSize.X * gridSize.Y ) + ( y * gridSize.X ) + x;

		//						var voxelValue = pDataBody[ index ];

		//						if( voxelValue > 0 )
		//						{
		//							int dataStartIndex = (int)voxelValue;

		//							int materialIndex = (int)pDataBody[ dataStartIndex + 0 ];

		//							Vector2F normalSpherical;
		//							normalSpherical.X = pDataBody[ dataStartIndex + 1 ];
		//							normalSpherical.Y = pDataBody[ dataStartIndex + 2 ];

		//							var normalObjectSpace = new SphericalDirectionF( normalSpherical.X, normalSpherical.Y ).GetVector();

		//							Vector2F texCoord;
		//							texCoord.X = pDataBody[ dataStartIndex + 3 ];
		//							texCoord.Y = pDataBody[ dataStartIndex + 4 ];


		//							//!!!!
		//							var meshGeometryIndex = 0;


		//							//if( v1 >= 0.0f )
		//							//{

		//							//var meshGeometryIndex = (int)v1;
		//							//var horizontal = v1 - (int)v1;
		//							//var vertical = v2;

		//							//var sphericalDirection = new SphericalDirectionF( horizontal * MathEx.PI * 2, vertical * MathEx.PI * 2 );
		//							//sphericalDirection.GetVector( out var normal );

		//							////float angle = MathAlgorithms.GetVectorsAngle( new Vector3F( 0, 0, 1 ), normal );
		//							//////float angle = MathAlgorithms.GetVectorsAngle( -cameraDirection, normal );
		//							////angle /= MathEx.PI;// * 2;
		//							////var c = MathEx.Saturate( 1.0f - angle );

		//							////var color = new ColorValue( c, c, c );

		//							var bMin = boundsMin + new Vector3F( x, y, z ) * cellSize;
		//							var b = new BoundsF( bMin, bMin + new Vector3F( cellSize, cellSize, cellSize ) );

		//							var voxel = new Voxel();
		//							//voxel.distanceSquared = (float)( cameraPosition - b.GetCenter() ).LengthSquared();
		//							//voxel.color = RenderingSystem.ConvertColorValue( ref color );
		//							voxel.index = new Vector3I( x, y, z );
		//							//voxel.bounds = b;


		//							voxel.normal = normalObjectSpace;
		//							voxel.texCoord = texCoord;

		//							if( meshGeometryIndex >= 0 && meshGeometryIndex < lists.Length )
		//								lists[ meshGeometryIndex ].Add( voxel );
		//							//list.Add( voxel );
		//							//}
		//						}
		//					}
		//				}
		//			}

		//			SimpleMeshGenerator.GenerateBox( new BoundsF( 0, 0, 0, cellSize, cellSize, cellSize ), out var boxPositions, out var boxIndices );

		//			//create mesh in space

		//			meshInSpaceVoxels = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//			var mesh = meshInSpaceVoxels.CreateComponent<Mesh>();
		//			meshInSpaceVoxels.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceVoxels, mesh );

		//			for( int nElement = 0; nElement < 1/*elements.Length*/; nElement++ )
		//			{
		//				//var element = elements[ nElement ];
		//				var list = lists[ nElement ];

		//				var vertices = new List<StandardVertex>( list.Count * 8 );
		//				var indices = new List<int>( list.Count * 12 );

		//				foreach( var voxel in list )
		//				{
		//					var startIndex = vertices.Count;

		//					var bMin = boundsMin + voxel.index.ToVector3F() * cellSize;
		//					foreach( var p in boxPositions )
		//					{
		//						var vertex = new StandardVertex();
		//						vertex.Position = bMin + p;
		//						vertex.Normal = voxel.normal;
		//						//!!!!
		//						vertex.Tangent = new Vector4F( vertex.Normal.X, vertex.Normal.Z, vertex.Normal.Y, -1 );
		//						vertex.Color = new ColorValue( 1, 1, 1 );
		//						vertex.TexCoord0 = voxel.texCoord;
		//						//vertex.TexCoord1 = voxel.texCoord;
		//						//vertex.TexCoord2 = voxel.texCoord;
		//						//vertex.TexCoord3 = voxel.texCoord;

		//						vertices.Add( vertex );
		//					}

		//					foreach( var i in boxIndices )
		//						indices.Add( startIndex + i );
		//				}

		//				var geometry = mesh.CreateComponent<MeshGeometry>();
		//				geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
		//				geometry.Vertices = CollectionUtility.ToByteArray( vertices.ToArray() );
		//				geometry.Indices = indices.ToArray();
		//				//!!!!
		//				geometry.Material = material;// element.Material.Value;
		//			}

		//			meshInSpaceVoxels.Enabled = true;
		//		}



		//		//var mode = header->Mode;
		//		//var size = header->Size;


		//		//var voxelCount = size.X * size.Y * size.Z;

		//		//if( data.Length == sizeof( MeshVoxelized.DataHeader ) + voxelCount * 8 )
		//		//{
		//		//	Half* pDataBody = (Half*)( pData + sizeof( MeshVoxelized.DataHeader ) );


		//		//	var cameraPosition = Viewport.CameraSettings.Position;
		//		//	var cameraDirection = Viewport.CameraSettings.Rotation.GetForward().ToVector3F();
		//		//	var renderer = Viewport.Simple3DRenderer;

		//		//	//!!!!поддержать не кубические. выравнивать или хранить габариты вокселей
		//		//	var meshBounds = Mesh.Result.SpaceBounds.CalculatedBoundingBox.ToBoundsF();
		//		//	var boundsMaxSide = meshBounds.GetSize().MaxComponent();
		//		//	var cellSize = boundsMaxSide / size.MaxComponent();
		//		//	//var cellSize = bounds.GetSize() / size.ToVector3();

		//		//	var elements = voxelized.GetComponents<MeshVoxelizedElement>();

		//		//	var lists = new List<Voxel>[ elements.Length ];
		//		//	for( int n = 0; n < elements.Length; n++ )
		//		//		lists[ n ] = new List<Voxel>( 64 );
		//		//	//var list = new List<Voxel>( voxelCount );

		//		//	for( int z = 0; z < size.Z; z++ )
		//		//	{
		//		//		for( int y = 0; y < size.Y; y++ )
		//		//		{
		//		//			for( int x = 0; x < size.X; x++ )
		//		//			{
		//		//				var index = ( z * size.X * size.Y ) + ( y * size.X ) + x;

		//		//				var v1h = pDataBody[ index * 4 + 0 ];
		//		//				var v2h = pDataBody[ index * 4 + 1 ];
		//		//				var v3h = pDataBody[ index * 4 + 2 ];
		//		//				var v4h = pDataBody[ index * 4 + 3 ];

		//		//				var v1 = (float)v1h;
		//		//				var v2 = (float)v2h;
		//		//				var v3 = (float)v3h;
		//		//				var v4 = (float)v4h;

		//		//				if( v1 >= 0.0f )
		//		//				{
		//		//					var meshGeometryIndex = (int)v1;
		//		//					var horizontal = v1 - (int)v1;
		//		//					var vertical = v2;

		//		//					var sphericalDirection = new SphericalDirectionF( horizontal * MathEx.PI * 2, vertical * MathEx.PI * 2 );
		//		//					sphericalDirection.GetVector( out var normal );

		//		//					//float angle = MathAlgorithms.GetVectorsAngle( new Vector3F( 0, 0, 1 ), normal );
		//		//					////float angle = MathAlgorithms.GetVectorsAngle( -cameraDirection, normal );
		//		//					//angle /= MathEx.PI;// * 2;
		//		//					//var c = MathEx.Saturate( 1.0f - angle );

		//		//					//var color = new ColorValue( c, c, c );

		//		//					////var color = new ColorValue( normal * 0.5f + new Vector3F( 0.5f, 0.5f, 0.5f ) );

		//		//					var bMin = meshBounds.Minimum + new Vector3F( x, y, z ) * cellSize;
		//		//					var b = new BoundsF( bMin, bMin + new Vector3F( cellSize, cellSize, cellSize ) );

		//		//					var voxel = new Voxel();
		//		//					//voxel.distanceSquared = (float)( cameraPosition - b.GetCenter() ).LengthSquared();
		//		//					//voxel.color = RenderingSystem.ConvertColorValue( ref color );
		//		//					voxel.index = new Vector3I( x, y, z );
		//		//					//voxel.bounds = b;
		//		//					voxel.normal = normal;
		//		//					voxel.texCoord = new Vector2F( v3, v4 );

		//		//					if( meshGeometryIndex >= 0 && meshGeometryIndex < lists.Length )
		//		//						lists[ meshGeometryIndex ].Add( voxel );
		//		//					//list.Add( voxel );
		//		//				}
		//		//			}
		//		//		}
		//		//	}

		//		//	SimpleMeshGenerator.GenerateBox( new BoundsF( 0, 0, 0, cellSize, cellSize, cellSize ), out var boxPositions, out var boxIndices );

		//		//	//create mesh in space

		//		//	meshInSpaceVoxelized = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//		//	var mesh = meshInSpaceVoxelized.CreateComponent<Mesh>();
		//		//	meshInSpaceVoxelized.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceVoxelized, mesh );

		//		//	for( int nElement = 0; nElement < elements.Length; nElement++ )
		//		//	{
		//		//		var element = elements[ nElement ];
		//		//		var list = lists[ nElement ];

		//		//		var vertices = new List<StandardVertex>( list.Count * 8 );
		//		//		var indices = new List<int>( list.Count * 12 );

		//		//		foreach( var voxel in list )
		//		//		{
		//		//			var startIndex = vertices.Count;

		//		//			var bMin = meshBounds.Minimum + voxel.index.ToVector3F() * cellSize;
		//		//			foreach( var p in boxPositions )
		//		//			{
		//		//				var vertex = new StandardVertex();
		//		//				vertex.Position = bMin + p;
		//		//				vertex.Normal = voxel.normal;
		//		//				//!!!!
		//		//				vertex.Tangent = new Vector4F( vertex.Normal.X, vertex.Normal.Z, vertex.Normal.Y, -1 );
		//		//				vertex.Color = new ColorValue( 1, 1, 1 );
		//		//				vertex.TexCoord0 = voxel.texCoord;
		//		//				//vertex.TexCoord1 = voxel.texCoord;
		//		//				//vertex.TexCoord2 = voxel.texCoord;
		//		//				//vertex.TexCoord3 = voxel.texCoord;

		//		//				vertices.Add( vertex );
		//		//			}

		//		//			foreach( var i in boxIndices )
		//		//				indices.Add( startIndex + i );
		//		//		}

		//		//		var geometry = mesh.CreateComponent<MeshGeometry>();
		//		//		geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
		//		//		geometry.Vertices = CollectionUtility.ToByteArray( vertices.ToArray() );
		//		//		geometry.Indices = indices.ToArray();
		//		//		geometry.Material = element.Material.Value;
		//		//	}

		//		//	meshInSpaceVoxelized.Enabled = true;
		//		//}

		//	}

		//}

		//void DestroyMeshInSpaceVoxels()
		//{
		//	meshInSpaceVoxels?.Dispose();
		//	meshInSpaceVoxels = null;
		//}

		static unsafe RangeI[] GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData( VertexElement[] structure, byte[] vertices, int[] indices, byte[] virtualizedData )
		{
			if( virtualizedData != null )
			{
				return MeshGeometry.GetMaterialIndexRangesFromVirtualizedData( virtualizedData );
			}
			else if( structure.GetElementBySemantic( VertexElementSemantic.Color3, out var element ) && element.Type == VertexElementType.Float1 )
			{
				structure.GetInfo( out var vertexSize, out _ );
				var vertexCount = vertices.Length / vertexSize;

				var vertexMaterialIndexes = ExtractChannel<float>( vertices, vertexSize, vertexCount, element.Offset );
				if( vertexMaterialIndexes != null )
				{
					var v = MeshGeometry.GetMaterialIndexRangesFromVertexMaterialIndexes( vertexMaterialIndexes, indices );
					if( v != null )
						return v;
				}
			}

			return null;
		}

		//static ColorValue[] GetClusteredClusterColors()
		//{
		//	return new ColorValue[] {
		//		new ColorValue( 1, 1, 0 ),
		//		new ColorValue( 1, 0, 0 ),
		//		new ColorValue( 0, 1, 0 ),
		//		new ColorValue( 0.75, 0.75, 0.75 ),
		//		new ColorValue( 1, 1, 1 ),
		//		new ColorValue( 0, 1, 1 ),
		//		new ColorValue( 1, 0, 1 ),
		//		new ColorValue( 0.8, 0.4, 0.1 ),
		//		KnownColors.Orange.ToColorValue(),
		//		KnownColors.SpringGreen.ToColorValue() };
		//}

		//ColorValue GetSeparateClusterColor()
		//{
		//	return new ColorValue( 0, 0, 0 );
		//}

		//unsafe void CreateMeshInSpaceClusterBounds()
		//{
		//	DestroyMeshInSpaceClusterBounds();

		//	if( Mesh.EditorDisplayClusters )
		//	{
		//		//!!!!Mesh.CompiledData
		//		var selectedMesh = GetSelectedLOD()?.Owner;

		//		if( selectedMesh != null )
		//		{
		//			meshInSpaceClusterBounds = new List<MeshInSpace>();

		//			var material = ResourceManager.LoadResource<Material>( @"Base\Tools\Mesh Editor\Mesh clusters.material" );

		//			foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
		//			{
		//				//var vertices = geometry.Vertices.Value;
		//				var indices = geometry.Indices.Value;
		//				//var clusterData = geometry.ClusterData.Value;

		//				geometry.GetClusterInfo( out _, out var clustersInfo );
		//				if( clustersInfo != null )
		//				{
		//					var vertices = geometry.VerticesExtractChannelVector3F( VertexElementSemantic.Position );
		//					if( vertices != null && indices != null )
		//					{
		//						var clusteredColors = GetClusteredClusterColors();
		//						//var separateColor = GetSeparateClusterColor();

		//						for( int nCluster = 0; nCluster < clustersInfo.Length; nCluster++ )
		//						{
		//							ref var clusterInfo = ref clustersInfo[ nCluster ];

		//							var trianglesMode = ( clusterInfo.Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;

		//							if( !trianglesMode )
		//							{
		//								var clusterSpaceToObjectSpace = new Matrix4F( clusterInfo.Rotation.ToMatrix3(), clusterInfo.Position );
		//								var objectSpaceToClusterSpace = clusterSpaceToObjectSpace.GetInverse();

		//								var resultVertices = new OpenList<StandardVertex>( clusterInfo.TriangleCount * 3 );
		//								var resultIndices = new OpenList<int>( clusterInfo.TriangleCount * 3 );

		//								var color = clusteredColors[ nCluster % clusteredColors.Length ];
		//								//var color = trianglesMode ? separateColor : clusteredColors[ nCluster % clusteredColors.Length ];
		//								//color.Alpha *= 0.7f;

		//								for( int nTriangle = 0; nTriangle < clusterInfo.TriangleCount; nTriangle++ )
		//								{
		//									for( int i = 0; i < 3; i++ )
		//									{
		//										var index = indices[ ( clusterInfo.TriangleStartOffset + nTriangle ) * 3 + i ];

		//										var vertex = new StandardVertex();
		//										vertex.Position = vertices[ index ];
		//										vertex.Normal = Vector3F.XAxis;
		//										var p = objectSpaceToClusterSpace * vertex.Position;
		//										vertex.TexCoord0 = p.ToVector2() / clusterInfo.CellSize;
		//										vertex.Color = color;
		//										resultIndices.Add( resultVertices.Count );
		//										resultVertices.Add( ref vertex );
		//									}
		//								}

		//								if( resultIndices.Count != 0 )
		//								{
		//									var resultVertices2 = resultVertices.ToArray();

		//									var bounds = BoundsF.Cleared;
		//									foreach( var v in resultVertices2 )
		//										bounds.Add( v.Position );

		//									var center = bounds.GetCenter();

		//									for( int n = 0; n < resultVertices2.Length; n++ )
		//										resultVertices2[ n ].Position -= center;

		//									var meshInSpace = Scene.CreateComponent<MeshInSpace>( enabled: false );
		//									meshInSpace.Transform = new Transform( center );

		//									var mesh = meshInSpace.CreateComponent<Mesh>();
		//									meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

		//									var geometry2 = mesh.CreateComponent<MeshGeometry>();
		//									geometry2.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
		//									geometry2.Vertices = CollectionUtility.ToByteArray( resultVertices2 );//resultVertices.ToArray() );
		//									geometry2.Indices = resultIndices.ToArray();
		//									geometry2.Material = material;
		//									meshInSpace.Enabled = true;

		//									meshInSpaceClusterBounds.Add( meshInSpace );
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}

		//			//if( resultIndices.Count != 0 )
		//			//{
		//			//	var material = ResourceManager.LoadResource<Material>( @"Base\Tools\Mesh Editor\Mesh clusters.material" );

		//			//	meshInSpaceClusterBounds = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//			//	var mesh = meshInSpaceClusterBounds.CreateComponent<Mesh>();
		//			//	meshInSpaceClusterBounds.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceClusterBounds, mesh );

		//			//	var geometry = mesh.CreateComponent<MeshGeometry>();
		//			//	geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
		//			//	geometry.Vertices = CollectionUtility.ToByteArray( resultVertices.ToArray() );
		//			//	geometry.Indices = resultIndices.ToArray();
		//			//	geometry.Material = material;
		//			//	meshInSpaceClusterBounds.Enabled = true;
		//			//}
		//		}
		//	}
		//}

		//void DestroyMeshInSpaceClusterBounds()
		//{
		//	if( meshInSpaceClusterBounds != null )
		//	{
		//		foreach( var obj in meshInSpaceClusterBounds )
		//			obj.Dispose();
		//		meshInSpaceClusterBounds = null;
		//	}
		//	//meshInSpaceClusterBounds?.Dispose();
		//	//meshInSpaceClusterBounds = null;
		//}

		//unsafe void CreateMeshInSpaceClusterTriangles()
		//{
		//	DestroyMeshInSpaceClusterTriangles();

		//	if( Mesh.EditorDisplayTriangles )
		//	{
		//		//!!!!use Mesh.CompiledData. where else
		//		var selectedMesh = GetSelectedLOD()?.Owner;

		//		if( selectedMesh != null )
		//		{
		//			if( selectedMesh.ContainsVirtualizedData() )
		//			{
		//				var resultVertices = new OpenList<StandardVertex>( 16384 );
		//				var resultIndices = new OpenList<int>( 16384 );

		//				foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
		//				{
		//					var items = geometry.ExtractActualGeometry( true, out var error );
		//					if( !string.IsNullOrEmpty( error ) )
		//						Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

		//					var clusteredColors = GetClusteredClusterColors();
		//					var separateColor = GetSeparateClusterColor();

		//					for( int nItem = 0; nItem < items.Length; nItem++ )
		//					{
		//						ref var item = ref items[ nItem ];

		//						//clusters only
		//						//if( item.ClusterIndex >= 0 )
		//						//{
		//						var vertices = item.Vertices;
		//						var indices = item.Indices;

		//						var color = item.ClusterSeparate ? separateColor : clusteredColors[ nItem % clusteredColors.Length ];
		//						//color.Alpha *= 0.7f;

		//						//randomize color by cluster
		//						for( int n = 0; n < vertices.Length; n++ )
		//						{
		//							ref var v = ref vertices[ n ];
		//							v.Color = color;
		//						}

		//						var startVertexIndex = resultVertices.Count;
		//						resultVertices.AddRange( vertices );
		//						foreach( var index in indices )
		//							resultIndices.Add( index + startVertexIndex );
		//						//}
		//					}
		//				}

		//				if( resultIndices.Count != 0 )
		//				{
		//					if( meshClusterTrianglesMaterial == null )
		//					{
		//						meshClusterTrianglesMaterial = ComponentUtility.CreateComponent<Material>( null, true, false );
		//						meshClusterTrianglesMaterial.ShadingModel = Material.ShadingModelEnum.Unlit;
		//						//meshClusterTrianglesMaterial.BlendMode = Material.BlendModeEnum.Transparent;
		//						meshClusterTrianglesMaterial.Enabled = true;
		//					}

		//					var material = meshClusterTrianglesMaterial;

		//					meshInSpaceClusterTriangles = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//					var mesh = meshInSpaceClusterTriangles.CreateComponent<Mesh>();
		//					meshInSpaceClusterTriangles.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceClusterTriangles, mesh );

		//					//!!!!need only Position and Color

		//					var geometry = mesh.CreateComponent<MeshGeometry>();
		//					geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
		//					geometry.Vertices = CollectionUtility.ToByteArray( resultVertices.ToArray() );
		//					geometry.Indices = resultIndices.ToArray();
		//					geometry.Material = material;
		//					meshInSpaceClusterTriangles.Enabled = true;
		//				}
		//			}
		//		}
		//	}
		//}

		//void DestroyMeshInSpaceClusterTriangles()
		//{
		//	meshInSpaceClusterTriangles?.Dispose();
		//	meshInSpaceClusterTriangles = null;
		//}

		class ExtractActualGeometryItem
		{
			public StandardVertex[] Vertices;
			public int[] Indices;
		}

		unsafe void CreateMeshInSpaceVertices()
		{
			DestroyMeshInSpaceVertices();

			if( Mesh.EditorDisplayVertices )
			{
				//!!!!use Mesh.CompiledData. where else
				var selectedMesh = GetSelectedLOD()?.Owner;

				if( selectedMesh != null )
				{
					var resultVertices = new OpenList<Vector3F>( 16384 );
					var resultIndices = new OpenList<int>( 16384 );

					var items = new List<ExtractActualGeometryItem>();
					foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
					{
						var item = new ExtractActualGeometryItem();
						if( geometry.ExtractActualGeometry( out item.Vertices, out item.Indices, out var error ) )
							items.Add( item );
						else
							Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );
					}

					//var items = new List<MeshGeometry.ExtractActualGeometryItem>();
					//foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
					//{
					//	var items2 = geometry.ExtractActualGeometry( true, out var error );
					//	if( !string.IsNullOrEmpty( error ) )
					//		Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

					//	items.AddRange( items2 );
					//}

					var bounds = BoundsF.Cleared;
					var vertexCount = 0;
					foreach( var item in items )
					{
						var vertices = item.Vertices;
						for( int n = 0; n < vertices.Length; n++ )
						{
							var p = vertices[ n ].Position;
							bounds.Add( ref p );
							vertexCount++;
						}
					}

					var radius = (float)bounds.GetSize().MaxComponent() / 600;
					if( vertexCount > 100000 )
						radius /= vertexCount / 100000.0f;

					foreach( var item in items )
					{
						var vertices = item.Vertices;
						for( int n = 0; n < vertices.Length; n++ )
						{
							var p = vertices[ n ].Position;

							Vector3F[] vertices2;
							int[] indices2;
							if( vertexCount < 100000 )
								SimpleMeshGenerator.GenerateSphere( new SphereF( p, radius ), 6, 6, false, out vertices2, out indices2 );
							else
							{
								var b = new BoundsF( p );
								b.Expand( radius );
								SimpleMeshGenerator.GenerateBox( b, out vertices2, out indices2 );
							}

							var startVertexIndex = resultVertices.Count;
							resultVertices.AddRange( vertices2 );
							foreach( var index in indices2 )
								resultIndices.Add( index + startVertexIndex );
						}
					}

					if( resultIndices.Count != 0 )
					{
						if( meshVerticesMaterial == null )
						{
							meshVerticesMaterial = ComponentUtility.CreateComponent<Material>( null, true, false );
							meshVerticesMaterial.ShadingModel = Material.ShadingModelEnum.Unlit;
							meshVerticesMaterial.BaseColor = new ColorValue( 0.9, 0.1, 0.1 );
							meshVerticesMaterial.Enabled = true;
						}

						var material = meshVerticesMaterial;

						meshInSpaceVertices = Scene.CreateComponent<MeshInSpace>( enabled: false );

						var mesh = meshInSpaceVertices.CreateComponent<Mesh>();
						meshInSpaceVertices.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceVertices, mesh );

						var geometry = mesh.CreateComponent<MeshGeometry>();
						geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.Position, true, out _ );
						geometry.Vertices = CollectionUtility.ToByteArray( resultVertices.ToArray() );
						geometry.Indices = resultIndices.ToArray();
						geometry.Material = material;
						meshInSpaceVertices.Enabled = true;
					}
				}
			}
		}

		void DestroyMeshInSpaceVertices()
		{
			meshInSpaceVertices?.Dispose();
			meshInSpaceVertices = null;
		}

		unsafe void CreateMeshInSpaceVertexColor()
		{
			DestroyMeshInSpaceVertexColor();

			if( Mesh.EditorDisplayVertexColor )
			{
				//!!!!use Mesh.CompiledData. where else
				var selectedMesh = GetSelectedLOD()?.Owner;

				if( selectedMesh != null )
				{
					var resultVertices = new OpenList<StandardVertex>( 16384 );
					var resultIndices = new OpenList<int>( 16384 );

					var items = new List<ExtractActualGeometryItem>();
					foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
					{
						var item = new ExtractActualGeometryItem();
						if( geometry.ExtractActualGeometry( out item.Vertices, out item.Indices, out var error ) )
							items.Add( item );
						else
							Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );
					}

					//var items = new List<MeshGeometry.ExtractActualGeometryItem>();
					//foreach( var geometry in selectedMesh.GetComponents<MeshGeometry>() )
					//{
					//	var items2 = geometry.ExtractActualGeometry( true, out var error );
					//	if( !string.IsNullOrEmpty( error ) )
					//		Log.Warning( "Mesh Editor: ExtractActualGeometry: " + error );

					//	items.AddRange( items2 );
					//}

					foreach( var item in items )
					{
						var startVertexIndex = resultVertices.Count;
						resultVertices.AddRange( item.Vertices );
						foreach( var index in item.Indices )
							resultIndices.Add( index + startVertexIndex );
					}

					if( resultIndices.Count != 0 )
					{
						if( meshVertexColorMaterial == null )
						{
							meshVertexColorMaterial = ComponentUtility.CreateComponent<Material>( null, true, false );
							meshVertexColorMaterial.ShadingModel = Material.ShadingModelEnum.Unlit;
							meshVertexColorMaterial.Enabled = true;
						}

						var material = meshVertexColorMaterial;

						meshInSpaceVertexColor = Scene.CreateComponent<MeshInSpace>( enabled: false );

						var mesh = meshInSpaceVertexColor.CreateComponent<Mesh>();
						meshInSpaceVertexColor.Mesh = ReferenceUtility.MakeThisReference( meshInSpaceVertexColor, mesh );

						var geometry = mesh.CreateComponent<MeshGeometry>();
						geometry.VertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out _ );
						geometry.Vertices = CollectionUtility.ToByteArray( resultVertices.ToArray() );
						geometry.Indices = resultIndices.ToArray();
						geometry.Material = material;
						meshInSpaceVertexColor.Enabled = true;
					}
				}
			}
		}

		void DestroyMeshInSpaceVertexColor()
		{
			meshInSpaceVertexColor?.Dispose();
			meshInSpaceVertexColor = null;
		}

	}
}

#endif