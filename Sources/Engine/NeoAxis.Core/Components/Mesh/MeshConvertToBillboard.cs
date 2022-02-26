// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.Threading.Tasks;

namespace NeoAxis
{
	class MeshConvertToBillboard
	{
		public Mesh Mesh;
		public MeshGeometry.BillboardDataModeEnum Mode;
		public int ImageSize;
		public bool CenteringByXY;
		public bool FillTransparentPixelsByNearPixels;

		///////////////////////////////////////////////

		enum Channel
		{
			Opacity,
			TextureCoordinate0,
			Depth,
			//!!!!
			//TexCoord1,
			//TexCoord2,
			//Normal,
			//!!!!
			//Tangent,
			//!!!!?
			//Color,
		}

		///////////////////////////////////////////////

		struct DistanceMapItem
		{
			public float distance;
			public Vector2I offset;
		}
		static Vector2I[] distanceMap;
		static Vector2I distanceMapForSize;

		static Vector2I[] GetDistanceMap( ImageUtility.Image2D image )
		{
			if( distanceMap == null || distanceMapForSize != image.Size )
			{
				var map = new List<DistanceMapItem>( image.Size.X * 5 );

				for( int y = -image.Size.Y; y <= image.Size.Y; y++ )
				{
					for( int x = -image.Size.X; x <= image.Size.X; x++ )
					{
						if( x == 0 && y == 0 )
							continue;

						var item = new DistanceMapItem();
						item.offset = new Vector2I( x, y );
						item.distance = MathEx.Sqrt( x * x + y * y );
						map.Add( item );
					}
				}

				//distanceMap = map.ToArray();
				distanceMapForSize = image.Size;

				CollectionUtility.MergeSort( map, delegate ( DistanceMapItem item1, DistanceMapItem item2 )
				{
					if( item1.distance < item2.distance )
						return -1;
					else if( item1.distance > item2.distance )
						return 1;
					return 0;
				}, true );

				distanceMap = new Vector2I[ map.Count ];
				for( var n = 0; n < distanceMap.Length; n++ )
					distanceMap[ n ] = map[ n ].offset;
			}

			return distanceMap;
		}

		///////////////////////////////////////////////

		static void FillTransparentPixelsByNearPixels2( ref ImageUtility.Image2D image, Vector2I[,] opacityImageNearestCellTable )
		{
			for( int y = 0; y < image.Size.Y; y++ )
			{
				for( int x = 0; x < image.Size.X; x++ )
				{
					var takeFrom = opacityImageNearestCellTable[ x, y ];
					var pixel = image.GetPixel( new Vector2I( takeFrom.X, takeFrom.Y ) );

					image.SetPixel( new Vector2I( x, y ), pixel );


					//var transparent = opacityImage.GetPixel( new Vector2I( x, y ) ).ToVector3F() == Vector3F.Zero;

					//ColorByte pixel;
					//if( transparent )
					//{
					//	var takeFrom = opacityImageNearestCellTable[ x, y ];
					//	pixel = image.GetPixelByte( new Vector2I( takeFrom.X, takeFrom.Y ) );
					//}
					//else
					//	pixel = image.GetPixelByte( new Vector2I( x, y ) );

					//image.SetPixel( new Vector2I( x, y ), pixel );
				}
			}
		}

		///////////////////////////////////////////////

		class BillboardDataDepthBuffers
		{
			public List<float[,]> depthBuffers = new List<float[,]>();
		}

		unsafe byte[] CreateBillboardData( MeshGeometry sourceGeometry, Sphere boundingSphere/*double boundingRadius*/, /*Bounds bounds, */bool needDepthBuffer, out BillboardDataDepthBuffers depthBuffers )
		{
			depthBuffers = new BillboardDataDepthBuffers();

			var header = new MeshGeometry.BillboardDataHeader();
			header.Version = 1;
			header.Mode = Mode;
			header.ImageSize = ImageSize;
			header.Format = 0;

			var imageSize = header.ImageSize;
			var imageCount = header.GetImageCount();
			var imageSizeInBytes = header.GetImageSizeInBytes();

			var resultData = new byte[ sizeof( MeshGeometry.BillboardDataHeader ) + imageCount * imageSizeInBytes ];
			fixed( byte* pResultData = resultData )
				NativeUtility.CopyMemory( pResultData, &header, sizeof( MeshGeometry.BillboardDataHeader ) );

			var cameraDistance = boundingSphere.Radius + 1;// boundingRadius + 1;

			//var cameraDistance = 0.0;
			//foreach( var p in bounds.ToPoints() )
			//	cameraDistance = Math.Max( cameraDistance, p.Length() );
			//cameraDistance += 1;

			try
			{
				//create scene

				var scene = ComponentUtility.CreateComponent<Scene>( null, true, false );
				scene.DisplayDevelopmentDataInEditor = false;
				scene.OctreeEnabled = false;

				var pipeline = (RenderingPipeline_Basic)scene.CreateComponent( RenderingSystem.RenderingPipelineBasic );
				scene.RenderingPipeline = pipeline;
				pipeline.DeferredShading = AutoTrueFalse.False;


				//scene.BackgroundColor = new ColorValue( 255, 0, 0 );
				//scene.BackgroundColorAffectLighting = 1;
				//scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );

				//var backgroundEffects = pipeline.CreateComponent<Component>();
				//backgroundEffects.Name = "Background Effects";

				var sceneEffects = pipeline.CreateComponent<Component>();
				sceneEffects.Name = "Scene Effects";

				//ShowRenderTarget to get depth
				var showRenderTargetEffect = sceneEffects.CreateComponent<RenderingEffect_ShowRenderTarget>();
				showRenderTargetEffect.Texture = RenderingEffect_ShowRenderTarget.TextureType.Depth;
				showRenderTargetEffect.DepthMultiplier = 1;

				//ambient light
				//!!!!
				//if( channel == MaterialChannel.Opacity )
				//{
				//	var light = scene.CreateComponent<Light>();
				//	light.Type = Light.TypeEnum.Ambient;
				//	light.Brightness = 1000000;// ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$General\\PreviewAmbientLightBrightness" );
				//							   //light.Brightness = ProjectSettings.Get.PreviewAmbientLightBrightness.Value;
				//}

				////directional light
				//{
				//	var light = scene.CreateComponent<Light>();
				//	light.Type = Light.TypeEnum.Directional;
				//	light.Transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.FromDirectionZAxisUp( new Vector3( 0, 0, -1 ) ), Vector3.One );
				//	light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$General\\PreviewDirectionalLightBrightness" );
				//	//light.Brightness = ProjectSettings.Get.PreviewDirectionalLightBrightness.Value;
				//	light.Shadows = false;
				//	//light.Type = Light.TypeEnum.Point;
				//	//light.Transform = new Transform( new Vec3( 0, 0, 2 ), Quat.Identity, Vec3.One );
				//}

				//create mesh
				var mesh = scene.CreateComponent<Mesh>();

				var meshGeometry = mesh.CreateComponent<MeshGeometry>();
				meshGeometry.Name = sourceGeometry.Name;
				meshGeometry.VertexStructure = sourceGeometry.VertexStructure;
				meshGeometry.UnwrappedUV = sourceGeometry.UnwrappedUV;
				meshGeometry.Vertices = sourceGeometry.Vertices;
				meshGeometry.Indices = sourceGeometry.Indices;
				meshGeometry.Material = sourceGeometry.Material;


				var meshInSpace = scene.CreateComponent<MeshInSpace>();
				meshInSpace.Transform = new Transform( -boundingSphere.Center, Quaternion.Identity );

				meshInSpace.Mesh = ReferenceUtility.MakeRootReference( mesh );

				//enable the scene
				scene.Enabled = true;


				ImageComponent texture = null;
				ImageComponent textureRead = null;


				try
				{
					var format = PixelFormat.Float32RGBA;

					texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
					texture.CreateType = ImageComponent.TypeEnum._2D;
					texture.CreateSize = new Vector2I( imageSize, imageSize );
					texture.CreateMipmaps = false;
					texture.CreateFormat = format;
					texture.CreateUsage = ImageComponent.Usages.RenderTarget;
					texture.CreateFSAA = 0;
					texture.Enabled = true;

					var renderTexture = texture.Result.GetRenderTarget();
					var viewport = renderTexture.AddViewport( false, false );
					viewport.AttachedScene = scene;

					textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
					textureRead.CreateType = ImageComponent.TypeEnum._2D;
					textureRead.CreateSize = new Vector2I( imageSize, imageSize );
					textureRead.CreateMipmaps = false;
					textureRead.CreateFormat = format;
					textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
					textureRead.CreateFSAA = 0;
					textureRead.Enabled = true;

					var currentResultDataOffset = sizeof( MeshGeometry.BillboardDataHeader );

					for( int nImage = 0; nImage < imageCount; nImage++ )
					{
						var result = new ImageUtility.Image2D( PixelFormat.Float32RGBA, new Vector2I( imageSize, imageSize ) );
						Vector2I[,] opacityImageNearestCellTable = null;


						//!!!!режим Clamp или как сейчас отступ оставлять

						//!!!!

						//double maxOffset;
						//{
						//	var offsetY = Math.Max( Math.Abs( bounds.Minimum.Y ), Math.Abs( bounds.Maximum.Y ) );
						//	//empty border
						//	offsetY *= 1.2;

						//	//!!!!выровнить
						//	var offsetZ = Math.Max( Math.Abs( bounds.Minimum.Z ), Math.Abs( bounds.Maximum.Z ) );
						//	//empty border
						//	offsetZ *= 1.2;

						//	maxOffset = Math.Max( offsetY, offsetZ );
						//}

						var height = boundingSphere.Radius * 2;// boundingRadius * 2;

						//var height = maxOffset * 2;

						//!!!!
						//var center = new Vector3F( 0, 0, 0 );
						//var center = Bounds.GetCenter();

						var cameraPosition = Vector3.Zero;

						if( header.Mode == MeshGeometry.BillboardDataModeEnum._1Direction )
						{
							cameraPosition = new Vector3( 1, 0, 0 );
						}
						else if( header.Mode == MeshGeometry.BillboardDataModeEnum._5Directions )
						{
							switch( nImage )
							{
							case 0: cameraPosition = new Vector3( 0, 0, -1 ); break;
							case 1: cameraPosition = new Vector3( 1, 0, -1 ); break;
							case 2: cameraPosition = new Vector3( 1, 0, 0 ); break;
							case 3: cameraPosition = new Vector3( 1, 0, 1 ); break;
							case 4: cameraPosition = new Vector3( 0, 0, 1 ); break;
							}

							//switch( nImage )
							//{
							//case 0: cameraPosition = new Vector3( 1, 0, -1 ); break;
							//case 1: cameraPosition = new Vector3( 1, 0, 0 ); break;
							//case 2: cameraPosition = new Vector3( 1, 0, 1 ); break;
							//case 3: cameraPosition = new Vector3( 0.0001, 0, 1 ); break;
							//}
						}
						else if( header.Mode == MeshGeometry.BillboardDataModeEnum._26Directions )
						{
							var cameraPositions = new Vector3[ 26 ]
							{
								new Vector3(-1, -1, -1), //0
								new Vector3(0, -1, -1), //1
								new Vector3(1, -1, -1), //2
								new Vector3(-1, 0, -1), //3
								new Vector3(0, 0, -1), //4
								new Vector3(1, 0, -1), //5
								new Vector3(-1, 1, -1), //6
								new Vector3(0, 1, -1), //7
								new Vector3(1, 1, -1), //8
								new Vector3(-1, -1, 0), //9
								new Vector3(0, -1, 0), //10
								new Vector3(1, -1, 0), //11
								new Vector3(-1, 0, 0), //12
								new Vector3(1, 0, 0), //13
								new Vector3(-1, 1, 0), //14
								new Vector3(0, 1, 0), //15
								new Vector3(1, 1, 0), //16
								new Vector3(-1, -1, 1), //17
								new Vector3(0, -1, 1), //18
								new Vector3(1, -1, 1), //19
								new Vector3(-1, 0, 1), //20
								new Vector3(0, 0, 1), //21
								new Vector3(1, 0, 1), //22
								new Vector3(-1, 1, 1), //23
								new Vector3(0, 1, 1), //24
								new Vector3(1, 1, 1) //25
							};

							cameraPosition = cameraPositions[ nImage ];
						}

						cameraPosition.Normalize();

						var up = Vector3.ZAxis;
						if( cameraPosition.Z > 0.99 )
							up = Vector3.XAxis;
						if( cameraPosition.Z < -0.99 )
							up = -Vector3.XAxis;

						var cameraDirection = -cameraPosition;
						cameraPosition *= cameraDistance;

						var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, 0.1, cameraDistance * 2, cameraPosition, cameraDirection, up, ProjectionType.Orthographic, height, 1, 1 );


						foreach( var channel2 in Enum.GetValues( typeof( Channel ) ) )
						{
							var channel = (Channel)channel2;

							if( channel == Channel.Depth && !needDepthBuffer )
								continue;

							//!!!!dds. mipmaps


							if( channel == Channel.Opacity )
								pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal;
							if( channel == Channel.TextureCoordinate0 )
								pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.TextureCoordinate0;
							if( channel == Channel.Depth )
								pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal;
							//!!!!

							showRenderTargetEffect.Enabled = channel == Channel.Depth;


							viewport.Update( true, cameraSettings );

							texture.Result.GetNativeObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

							//get data
							var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * imageSize * imageSize;
							var data = new byte[ totalBytes ];
							unsafe
							{
								fixed( byte* pBytes = data )
								{
									var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );
									while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
								}
							}

							var image = new ImageUtility.Image2D( format, new Vector2I( imageSize, imageSize ), data );


							if( channel == Channel.Opacity )
							{
								var rotationNormals = Quaternion.LookAt( -cameraDirection, up ).GetInverse().ToQuaternionF();

								for( int y = 0; y < image.Size.Y; y++ )
								{
									for( int x = 0; x < image.Size.X; x++ )
									{
										var pixel = image.GetPixel( new Vector2I( x, y ) );

										var v = pixel.ToVector3F();
										if( v != Vector3F.Zero )
										{
											v = ( v * 2.0f - Vector3F.One ).GetNormalize();

											//rotate normal to object space
											v = rotationNormals * v;

											var dir = SphericalDirectionF.FromVector( v );
											result.SetPixel( new Vector2I( x, y ), new Vector4F( dir.Horizontal, dir.Vertical, 0, 0 ) );
										}
										else
											result.SetPixel( new Vector2I( x, y ), new Vector4F( 100.0f, 0, 0, 0 ) );
									}
								}

								//opacityImageNearestCellTable
								if( FillTransparentPixelsByNearPixels )
								{
									var boolOpacityImage = new int[ image.Size.X, image.Size.Y ];
									for( int y = 0; y < image.Size.Y; y++ )
									{
										for( int x = 0; x < image.Size.X; x++ )
										{
											var c = image.GetPixel( new Vector2I( x, y ) );
											boolOpacityImage[ x, y ] = c.X > 50.0f ? 1 : 0;
											//boolOpacityImage[ x, y ] = c.ToVector3F() == Vector3F.Zero ? 1 : 0;
										}
									}

									var distanceMap = GetDistanceMap( image );

									opacityImageNearestCellTable = new Vector2I[ image.Size.X, image.Size.Y ];
									for( int y = 0; y < image.Size.Y; y++ )
										for( int x = 0; x < image.Size.X; x++ )
											opacityImageNearestCellTable[ x, y ] = new Vector2I( x, y );

									var imageSizeX = image.Size.X;
									var imageSizeY = image.Size.Y;

									Parallel.For( 0, image.Size.X * image.Size.Y, delegate ( int xy )
									{
										var y = xy / imageSizeX;
										var x = xy % imageSizeX;

										var transparent = boolOpacityImage[ x, y ];
										if( transparent != 0 )//if( transparent )
										{
											for( int n = 0; n < distanceMap.Length; n++ )
											{
												ref var indexItem = ref distanceMap[ n ];

												var takeFromX = x + indexItem.X;
												var takeFromY = y + indexItem.Y;
												if( takeFromX >= 0 && takeFromX < imageSizeX && takeFromY >= 0 && takeFromY < imageSizeY )
												{
													var transparent2 = boolOpacityImage[ takeFromX, takeFromY ];
													if( transparent2 == 0 )//if( !transparent2 )
													{
														opacityImageNearestCellTable[ x, y ] = new Vector2I( takeFromX, takeFromY );
														break;
													}
												}
											}
										}
									} );
								}
							}

							if( channel == Channel.TextureCoordinate0 )
							{
								//fill transparent pixels by near pixels
								if( opacityImageNearestCellTable != null )
									FillTransparentPixelsByNearPixels2( ref image, opacityImageNearestCellTable );

								for( int y = 0; y < image.Size.Y; y++ )
								{
									for( int x = 0; x < image.Size.X; x++ )
									{
										var pixel = image.GetPixel( new Vector2I( x, y ) );

										var current = result.GetPixel( new Vector2I( x, y ) );
										current.Z = pixel.X;
										current.W = pixel.Y;
										result.SetPixel( new Vector2I( x, y ), current );
									}
								}
							}

							if( channel == Channel.Depth )
							{
								var depthBuffer = new float[ image.Size.X, image.Size.Y ];

								for( int y = 0; y < image.Size.Y; y++ )
								{
									for( int x = 0; x < image.Size.X; x++ )
									{
										var pixel = image.GetPixel( new Vector2I( x, y ) );
										depthBuffer[ x, y ] = pixel.X;
									}
								}

								depthBuffers.depthBuffers.Add( depthBuffer );
							}

							//!!!!
							//if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal )
							//{
							//	//rotate normal map
							//	if( channel == MaterialChannel.Normal )
							//	{
							//		//var rot = QuaternionF.Identity;
							//		var rot = QuaternionF.FromRotateByY( new RadianF( MathEx.PI / 2 ) );
							//		//var rot2 = QuaternionF.FromRotateByX( new RadianF( MathEx.PI / 2 ) );

							//		for( int y = 0; y < image.Size.Y; y++ )
							//		{
							//			for( int x = 0; x < image.Size.X; x++ )
							//			{
							//				var pixel = image.GetPixel( new Vector2I( x, y ) );

							//				var vector = pixel.ToVector3F();
							//				vector -= new Vector3F( 0.5f, 0.5f, 0.5f );
							//				vector *= 2.0f;

							//				//vector = rot2 * vector;
							//				vector = rot * vector;
							//				//vector = rot2 * vector;

							//				vector.Normalize();

							//				vector *= 0.5f;
							//				vector += new Vector3F( 0.5f, 0.5f, 0.5f );

							//				pixel = new Vector4F( vector, pixel.W );

							//				image.SetPixel( new Vector2I( x, y ), pixel );
							//			}
							//		}
							//	}

							//}

						}

						//convert Float to Half
						byte[] halfArray = new byte[ result.Data.Length / 2 ];
						fixed( byte* pHalfArray = halfArray )
						{
							fixed( byte* pFloatArray = result.Data )
							{
								HalfType* pHalf = (HalfType*)pHalfArray;
								float* pFloat = (float*)pFloatArray;

								for( int n = 0; n < halfArray.Length / 2; n++ )
								{
									*pHalf = new HalfType( *pFloat );
									pHalf++;
									pFloat++;
								}
							}
						}

						Array.Copy( halfArray, 0, resultData, currentResultDataOffset, imageSizeInBytes );


						//Array.Copy( result.Data, 0, resultData, currentResultDataOffset, imageSizeInBytes );
						currentResultDataOffset += imageSizeInBytes;
					}
				}
				finally
				{
					texture?.Dispose();
					textureRead?.Dispose();
					scene?.Dispose();
				}

			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return null;
			}

			return resultData;
		}

		public void Convert()
		{
			//!!!!если были процедурные геометрии

			var wasEnabled = Mesh.Enabled;
			Mesh.Enabled = false;

			try
			{
				var geometries = Mesh.GetComponents<MeshGeometry>();

				//!!!!когда несколько геометрий по депту куллить

				//!!!!
				//!!!!для всех геометрий один прямоугольник, тогда анимация норм. ну разные
				//!!!!!!!!это полезно только для анимаций растений. может опцией типа OptimizeGeometry
				//!!!!с разными еще разные depthBuffer

				//!!!!пока так. потом с коэффициэентами position, scale для каждого изображения

				Sphere boundingSphere;
				{
					var positions = new List<Vector3F>( 1024 );
					foreach( var geometry in geometries )
						positions.AddRange( geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position ) );

					//!!!!can find better

					BoundsF bounds = BoundsF.Cleared;
					foreach( var p in positions )
						bounds.Add( p );

					var center = bounds.GetCenter();
					if( CenteringByXY )
					{
						center.X = 0;
						center.Y = 0;
					}

					float radiusSquared = 0;
					foreach( var p in positions )
					{
						var r = ( p - center ).LengthSquared();
						if( r > radiusSquared )
							radiusSquared = r;
					}

					boundingSphere = new SphereF( center, MathEx.Sqrt( radiusSquared ) );

					//boundingSphere = MathAlgorithms.BoundingSphereFromPoints( positions );

					//add offset
					boundingSphere.Radius *= 1.01;
				}

				double boundingRadius = boundingSphere.Radius;

				//double boundingRadius = 0;
				//{
				//	foreach( var geometry in geometries )
				//	{
				//		var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
				//		foreach( var p in positions )
				//			boundingRadius = Math.Max( boundingRadius, p.Length() );
				//	}

				//	//!!!!add offset
				//	boundingRadius *= 1.05;
				//}

				//var bounds = Bounds.Cleared;
				//{
				//	foreach( var geometry in geometries )
				//	{
				//		var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
				//		foreach( var p in positions )
				//			bounds.Add( p );
				//	}
				//}

				var depthBufferItems = new List<BillboardDataDepthBuffers>();

				for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
				{
					var geometry = geometries[ nGeometry ];

					//!!!!
					//var bounds = Bounds.Cleared;
					//{
					//	var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
					//	foreach( var p in positions )
					//		bounds.Add( p );
					//}


					var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

					var billboardData = CreateBillboardData( geometry, boundingSphere/*boundingRadius*/, geometries.Length > 1, out var depthBufferItem );

					var vertices = new List<StandardVertex.StaticOneTexCoord>();
					var indices = new List<int>();

					//!!!!

					//double maxOffset;
					//{
					//	var offsetY = Math.Max( Math.Abs( bounds.Minimum.Y ), Math.Abs( bounds.Maximum.Y ) );
					//	//empty border
					//	offsetY *= 1.2;

					//	//!!!!выровнить
					//	var offsetZ = Math.Max( Math.Abs( bounds.Minimum.Z ), Math.Abs( bounds.Maximum.Z ) );
					//	//empty border
					//	offsetZ *= 1.2;

					//	maxOffset = Math.Max( offsetY, offsetZ );
					//}

					//Vector2 min = new Vector2( -maxOffset, -maxOffset );
					//Vector2 max = new Vector2( maxOffset, maxOffset );



					//!!!!что-то не работает
					//{

					//тут еще отступ нужен, края обрежет

					//	{
					//		var vertex = new StandardVertex.StaticOneTexCoord();
					//		vertex.Position = Vector3F.Zero;
					//		vertex.Normal = Vector3F.XAxis;

					//		//!!!!check
					//		vertex.Tangent = new Vector4F( 0, 1, 0, -1 );

					//		vertex.Color = ColorValue.One;
					//		vertex.TexCoord0 = new Vector2F( 0.5f, 0.5f );

					//		vertices.Add( vertex );
					//	}

					//	var steps = 8;
					//	float step = MathEx.PI * 2 / (float)steps;
					//	float angle = 0;

					//	for( int n = 0; n < steps; n++ )
					//	{
					//		var point = new Vector2F( MathEx.Cos( angle ), MathEx.Sin( angle ) );
					//		var position = point * (float)boundingRadius;

					//		{
					//			var vertex = new StandardVertex.StaticOneTexCoord();
					//			vertex.Position = new Vector3F( 0, position.X, position.Y );
					//			vertex.Normal = Vector3F.XAxis;

					//			//!!!!check
					//			vertex.Tangent = new Vector4F( 0, 1, 0, -1 );

					//			vertex.Color = ColorValue.One;
					//			vertex.TexCoord0 = new Vector2F( 0.5f, 0.5f ) + new Vector2F( point.X, -point.Y ) * 0.5f;

					//			vertices.Add( vertex );
					//		}

					//		angle += step;
					//	}

					//	for( int n = 0; n < steps; n++ )
					//	{
					//		indices.Add( 0 );
					//		indices.Add( 1 + n );
					//		indices.Add( 1 + ( n + 1 ) % steps );
					//		//indices.Add( 1 + ( n + 1 ) % steps );
					//		//indices.Add( 1 + n );
					//	}
					//}


					{
						for( int z = 0; z < 2; z++ )
						{
							for( int y = 0; y < 2; y++ )
							{
								var vertex = new StandardVertex.StaticOneTexCoord();

								var pos = Vector3.Zero;

								//!!!!
								//pos.X += boundingRadius;

								//!!!!

								if( y == 0 )
									pos.Y = -boundingRadius;
								else
									pos.Y = boundingRadius;
								if( z == 0 )
									pos.Z = -boundingRadius;
								else
									pos.Z = boundingRadius;

								//if( y == 0 )
								//	pos.Y = bounds.Minimum.Y;
								//else
								//	pos.Y = bounds.Maximum.Y;
								//if( z == 0 )
								//	pos.Z = bounds.Minimum.Z;
								//else
								//	pos.Z = bounds.Maximum.Z;

								vertex.Position = pos.ToVector3F();

								vertex.Normal = Vector3F.XAxis;

								//var normal = Vector3F.XAxis;

								////!!!!
								//if( y == 0 )
								//	normal.Y = -4;
								//else
								//	normal.Y = 4;
								//if( z == 0 )
								//	normal.Z = -1;
								//else
								//	normal.Z = 1;


								//if( y == 0 )
								//	normal.Y = -4;
								//else
								//	normal.Y = 4;
								//if( z == 0 )
								//	normal.Z = -4;
								//else
								//	normal.Z = 4;

								//if( y == 0 )
								//	normal.Y = -1;
								//else
								//	normal.Y = 1;
								//if( z == 0 )
								//	normal.Z = -1;
								//else
								//	normal.Z = 1;

								//vertex.Normal = normal.GetNormalize();


								//!!!!check
								vertex.Tangent = new Vector4F( 0, 1, 0, -1 );

								vertex.Color = new ColorValue( 1, 1, 1, 1 );

								var texCoord = Vector2F.Zero;

								//!!!!
								if( y == 0 )
									texCoord.X = 0;
								else
									texCoord.X = 1;
								if( z == 0 )
									texCoord.Y = 1;
								else
									texCoord.Y = 0;

								//texCoord.X = (float)( ( pos.Y - min.X ) / ( maxOffset * 2 ) );
								//texCoord.Y = 1.0f - (float)( ( pos.Z - min.Y ) / ( maxOffset * 2 ) );

								vertex.TexCoord0 = texCoord;

								vertices.Add( vertex );
							}
						}

						indices = new List<int>( new int[] { 0, 1, 3, 3, 2, 0 } );
					}




					geometry.VertexStructure = vertexStructure;
					geometry.Vertices = CollectionUtility.ToByteArray( vertices.ToArray() );
					geometry.Indices = indices.ToArray();
					geometry.BillboardData = billboardData;

					depthBufferItems.Add( depthBufferItem );
				}

				//cull invisible pixels by depth
				unsafe
				{
					if( geometries.Length > 1 )
					{
						var header = new MeshGeometry.BillboardDataHeader();
						fixed( byte* pData = geometries[ 0 ].BillboardData.Value )
							NativeUtility.CopyMemory( &header, pData, sizeof( MeshGeometry.BillboardDataHeader ) );

						var imageSize = header.ImageSize;
						var imageCount = header.GetImageCount();
						var imageSizeInBytes = header.GetImageSizeInBytes();

						var currentOffset = sizeof( MeshGeometry.BillboardDataHeader );

						for( int nImage = 0; nImage < imageCount; nImage++ )
						{
							for( int y = 0; y < imageSize; y++ )
							{
								for( int x = 0; x < imageSize; x++ )
								{

									//find smallest depth

									var smallestDepthIndex = -1;
									var smallestDepth = float.MaxValue;

									for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
									{
										var depthBuffer = depthBufferItems[ nGeometry ].depthBuffers[ nImage ];
										var depth = depthBuffer[ x, y ];

										if( ( smallestDepthIndex == -1 || depth < smallestDepth ) && depth != 0 )
										{
											smallestDepth = depth;
											smallestDepthIndex = nGeometry;
										}
									}

									//update opacity for buffers with bigger depth
									if( smallestDepthIndex != -1 )
									{
										for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
										{
											if( nGeometry != smallestDepthIndex )
											{
												var billboardData = geometries[ nGeometry ].BillboardData.Value;

												fixed( byte* pBillboardData = billboardData )
												{
													byte* pImage = pBillboardData + currentOffset;
													byte* pPixel2 = pImage + ( y * imageSize + x ) * 8;
													HalfType* pPixel = (HalfType*)pPixel2;

													pPixel[ 0 ] = new HalfType( 100.0f );
													pPixel[ 1 ] = new HalfType( 0.0f );
												}
											}
										}
									}

								}
							}

							currentOffset += imageSizeInBytes;
						}
					}
				}

				Mesh.Billboard = true;
				Mesh.BillboardPositionOffset = boundingSphere.Center;
				//Mesh.BillboardShadowOffset = boundingSphere.Radius / 20;
			}
			finally
			{
				Mesh.Enabled = wasEnabled;
			}
		}
	}
}
