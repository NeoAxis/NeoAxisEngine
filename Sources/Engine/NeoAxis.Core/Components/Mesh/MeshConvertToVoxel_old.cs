// Copyright 2006–2026 Ivan Efimov. All rights reserved.


//}
//else
//{

//	for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
//	{
//		var geometry = geometries[ nGeometry ];
//		var geometryData = geometryDatas[ nGeometry ];

//		if( geometryData.Position == null )
//			continue;

//		using( var meshTest = new MeshTest( geometryData.Position, geometryData.Indices ) )
//		{
//			Parallel.ForEach( voxelIndexes, delegate ( Vector3I index3 )
//			{
//				var x = index3.X;
//				var y = index3.Y;
//				var z = index3.Z;

//				var cellBoundsMin = meshBounds.Minimum + new Vector3F( x, y, z ) * cellSize;
//				var cellBoundsMax = cellBoundsMin + new Vector3F( cellSize, cellSize, cellSize );
//				var cellCenter = ( cellBoundsMin + cellBoundsMax ) * 0.5f;

//				for( int axis = 0; axis < 3; axis++ )
//				{
//					RayF ray = new RayF();
//					switch( axis )
//					{
//					case 0:
//						ray = new RayF( new Vector3F( cellBoundsMin.X, cellCenter.Y, cellCenter.Z ), new Vector3F( cellSize, 0, 0 ) );
//						break;

//					case 1:
//						ray = new RayF( new Vector3F( cellCenter.X, cellBoundsMin.Y, cellCenter.Z ), new Vector3F( 0, cellSize, 0 ) );
//						break;

//					case 2:
//						ray = new RayF( new Vector3F( cellCenter.X, cellCenter.Y, cellBoundsMin.Z ), new Vector3F( 0, 0, cellSize ) );
//						break;
//					}

//					//!!!!можно все перебирать
//					var resultList = meshTest.RayCast( ray, MeshTest.Mode.OneClosest, true );

//					if( resultList != null && resultList.Length != 0 )
//					{
//						var result = resultList[ 0 ];

//						var nTriangle = result.TriangleIndex;

//						var index0 = geometryData.Indices[ nTriangle * 3 + 0 ];
//						var index1 = geometryData.Indices[ nTriangle * 3 + 1 ];
//						var index2 = geometryData.Indices[ nTriangle * 3 + 2 ];

//						ref var v0 = ref geometryData.Position[ index0 ];
//						ref var v1 = ref geometryData.Position[ index1 ];
//						ref var v2 = ref geometryData.Position[ index2 ];

//						var voxelData = new VoxelWithData();
//						voxelData.Index = new Vector3I( x, y, z );


//						var pointOnRay = ray.GetPointOnRay( result.Scale );
//						MathAlgorithms.CalculateBarycentricCoordinates( ref v0, ref v1, ref v2, ref pointOnRay, out var u, out var v, out var w );


//						//!!!!
//						//voxelData.MaterialIndex = 0;


//						//!!!!нужно чтобы точка была внутри треугольника


//						//normal
//						if( geometryData.Normal != null )
//						{
//							ref var normal0 = ref geometryData.Normal[ index0 ];
//							ref var normal1 = ref geometryData.Normal[ index1 ];
//							ref var normal2 = ref geometryData.Normal[ index2 ];
//							voxelData.Normal = ( u * normal0 + v * normal1 + w * normal2 ).GetNormalize();
//						}
//						else
//						{
//							PlaneF.FromPoints( ref v0, ref v1, ref v2, out var plane );
//							voxelData.Normal = plane.Normal;
//						}

//						//tangent
//						if( geometryData.Tangent != null )
//						{
//							ref var t0 = ref geometryData.Tangent[ index0 ];
//							ref var t1 = ref geometryData.Tangent[ index1 ];
//							ref var t2 = ref geometryData.Tangent[ index2 ];
//							var tangent3 = ( u * t0.ToVector3F() + v * t1.ToVector3F() + w * t2.ToVector3F() ).GetNormalize();
//							voxelData.Tangent = new Vector4F( tangent3, t0.W );
//						}

//						//texCoord0
//						if( geometryData.TexCoord0 != null )
//						{
//							ref var texCoord0 = ref geometryData.TexCoord0[ index0 ];
//							ref var texCoord1 = ref geometryData.TexCoord0[ index1 ];
//							ref var texCoord2 = ref geometryData.TexCoord0[ index2 ];
//							voxelData.TexCoord0 = u * texCoord0 + v * texCoord1 + w * texCoord2;
//						}

//						//texCoord1
//						if( geometryData.TexCoord1 != null )
//						{
//							ref var texCoord0 = ref geometryData.TexCoord1[ index0 ];
//							ref var texCoord1 = ref geometryData.TexCoord1[ index1 ];
//							ref var texCoord2 = ref geometryData.TexCoord1[ index2 ];
//							voxelData.TexCoord1 = u * texCoord0 + v * texCoord1 + w * texCoord2;
//						}

//						//texCoord2
//						if( geometryData.TexCoord2 != null )
//						{
//							ref var texCoord0 = ref geometryData.TexCoord2[ index0 ];
//							ref var texCoord1 = ref geometryData.TexCoord2[ index1 ];
//							ref var texCoord2 = ref geometryData.TexCoord2[ index2 ];
//							voxelData.TexCoord2 = u * texCoord0 + v * texCoord1 + w * texCoord2;
//						}

//						//color
//						if( geometryData.Color0 != null )
//						{
//							ref var color0 = ref geometryData.Color0[ index0 ];
//							ref var color1 = ref geometryData.Color0[ index1 ];
//							ref var color2 = ref geometryData.Color0[ index2 ];
//							voxelData.Color0 = u * color0 + v * color1 + w * color2;
//						}
//						else
//							voxelData.Color0 = ColorValue.One;


//						int dataIndex;

//						lock( voxelsWithDataOnTriangle )
//						{
//							dataIndex = voxelsWithDataOnTriangle.Count;
//							voxelsWithDataOnTriangle.Add( ref voxelData );
//						}

//						ref var voxel = ref voxels[ x, y, z ];
//						voxel.DataIndexInListOnTriangle = dataIndex;

//						//!!!!можно лучший найти
//						//!!!!про несколько геометрий тоже
//						//!!!!сгладить нормали
//						break;
//					}
//				}

//			} );
//		}
//	}
//}






//var format = SourceFormat;
//if( format == VertexFormatEnum.Auto )
//{
//	format = VertexFormatEnum.Basic;

//	foreach( var geometry in geometries )
//	{
//		var structure = geometry.VertexStructure.Value;
//		if( structure != null )
//		{
//			if( structure.GetElementBySemantic( VertexElementSemantic.Color0, out _ ) )
//				format = VertexFormatEnum.Full;
//			else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out _ ) )
//				format = VertexFormatEnum.Full;
//			else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out _ ) )
//				format = VertexFormatEnum.Full;
//		}
//	}
//}
//var fullFormat = format == VertexFormatEnum.Full;






//var detailSize = size * 4;
//var detailCellSize = cellSize / 4;
//var detailVoxels = new DetailVoxel[ detailSize.X, detailSize.Y, detailSize.Z ];

////rasterize geometry to detail voxel grid
//{

//	for( int nGeometry = 0; nGeometry < geometries.Length; nGeometry++ )
//	{
//		var geometry = geometries[ nGeometry ];
//		var geometryData = geometryDatas[ nGeometry ];

//		if( geometryData.Position == null )
//			continue;


//		//!!!!parallel


//		for( int nTriangle = 0; nTriangle < geometryData.Indices.Length / 3; nTriangle++ )
//		{
//			var index0 = geometryData.Indices[ nTriangle * 3 + 0 ];
//			var index1 = geometryData.Indices[ nTriangle * 3 + 1 ];
//			var index2 = geometryData.Indices[ nTriangle * 3 + 2 ];

//			ref var v0 = ref geometryData.Position[ index0 ];
//			ref var v1 = ref geometryData.Position[ index1 ];
//			ref var v2 = ref geometryData.Position[ index2 ];

//			var triangleBounds = new BoundsF( v0 );
//			triangleBounds.Add( ref v1 );
//			triangleBounds.Add( ref v2 );


//			//get detail grid position

//			var detailGridPositionMin = ( triangleBounds.Minimum - meshBounds.Minimum ) / detailCellSize;
//			var detailGridPositionMax = ( triangleBounds.Maximum - meshBounds.Minimum ) / detailCellSize;

//			var detailGridIndexMin = detailGridPositionMin.ToVector3I();
//			var detailGridIndexMax = detailGridPositionMax.ToVector3I() + Vector3I.One;

//			for( int z = detailGridIndexMin.Z; z <= detailGridIndexMax.Z; z++ )
//			{
//				for( int y = detailGridIndexMin.Y; y <= detailGridIndexMax.Y; y++ )
//				{
//					for( int x = detailGridIndexMin.X; x <= detailGridIndexMax.X; x++ )
//					{

//						//!!!если уже что-то есть надо проверять?

//						var detailIndex = new Vector3I( x, y, z );

//						//get cell bounds
//						var bMin = meshBounds.Minimum + detailCellSize * detailIndex.ToVector3F();
//						var b = new BoundsF( bMin, bMin + new Vector3F( detailCellSize, detailCellSize, detailCellSize ) );

//						//if( MathAlgorithms.IntersectTriangleRay( v0, v1, v2, ray ) )
//						//{
//						//}

//						//if( b.Intersects( ray, out var scale ) )
//						//{
//						//}



//						//!!!!
//						b.Expand( 0.001f );



//					}
//				}
//			}

//			//var min = new Vector3I( triangleBounds.Minimum );

//			////var detailGridRange = zzz;

//			//var voxelData = new VoxelWithData();
//			//voxelData.Index = new Vector3I( x, y, z );


//		}
//	}
//}







//var voxelWithoutData = new List<Vector3I>( voxelIndexes.Length );
//foreach( var index in voxelIndexes )
//{
//	ref var voxel = ref voxels[ index.X, index.Y, index.Z ];
//	if( voxel.DataIndexInListResult == -1 )
//		voxelWithoutData.Add( index );
//}
//var voxelWithoutDataArray = voxelWithoutData.ToArray();


//var voxelsWithDataResultIndexes = new Vector3I[ voxelsWithDataResult.Count ];
//for( int n = 0; n < voxelsWithDataResultIndexes.Length; n++ )
//	voxelsWithDataResultIndexes[ n ] = voxelsWithDataResult.Data[ n ].Index;


//Parallel.For( 0, voxelWithoutDataArray.Length, delegate ( int index )
////Parallel.ForEach( voxelIndexes, delegate ( Vector3I index3 )
//{
//	var index3 = voxelWithoutDataArray[ index ];
//	int index3X = index3.X;
//	int index3Y = index3.Y;
//	int index3Z = index3.Z;

//	//if( voxel.DataIndexInList == -1 )
//	//{

//	var minDistanceSquared = int.MaxValue;
//	var minIndex = new Vector3I( -1, -1, -1 );

//	for( int n = 0; n < voxelsWithDataResultIndexes.Length; n++ )//for( int n = 0; n < voxelsWithDataResult.Count; n++ )
//	{
//		ref var indexToCheck = ref voxelsWithDataResultIndexes[ n ];
//		//ref var voxelWithData = ref voxelsWithDataResult.Data[ n ];
//		//var indexToCheck = voxelWithData.Index;

//		var xx = indexToCheck.X - index3X;
//		var yy = indexToCheck.Y - index3Y;
//		var zz = indexToCheck.Z - index3Z;
//		var distanceSquared = xx * xx + yy * yy + zz * zz;
//		//var distanceSquared = ( indexToCheck - index3 ).ToVector3F().LengthSquared();

//		if( distanceSquared < minDistanceSquared )
//		{
//			minDistanceSquared = distanceSquared;
//			minIndex = indexToCheck;
//		}
//	}

//	//!!!!
//	//если ничего нету
//	if( minIndex.X == -1 )
//		minIndex = new Vector3I( 0, 0, 0 );

//	ref var voxel = ref voxels[ index3X, index3Y, index3Z ];
//	voxel.NearestVoxelWithData = minIndex;

//	//}
//} );


//Parallel.For( 0, voxelWithoutData.Count, delegate ( int index )
////Parallel.ForEach( voxelIndexes, delegate ( Vector3I index3 )
//{
//	var index3 = voxelWithoutData[ index ];

//	//if( voxel.DataIndexInList == -1 )
//	{
//		var minDistanceSquared = int.MaxValue;
//		var minIndex = new Vector3I( -1, -1, -1 );

//		for( int n = 0; n < voxelsWithDataResult.Count; n++ )
//		{
//			ref var voxelWithData = ref voxelsWithDataResult.Data[ n ];
//			var indexToCheck = voxelWithData.Index;

//			var xx = indexToCheck.X - index3.X;
//			var yy = indexToCheck.Y - index3.Y;
//			var zz = indexToCheck.Z - index3.Z;
//			var distanceSquared = xx * xx + yy * yy + zz * zz;
//			//var distanceSquared = ( indexToCheck - index3 ).ToVector3F().LengthSquared();

//			if( distanceSquared < minDistanceSquared )
//			{
//				minDistanceSquared = distanceSquared;
//				minIndex = indexToCheck;
//			}
//		}

//		//!!!!
//		//если ничего нету
//		if( minIndex.X == -1 )
//			minIndex = new Vector3I( 0, 0, 0 );

//		ref var voxel = ref voxels[ index3.X, index3.Y, index3.Z ];
//		voxel.NearestVoxelWithData = minIndex;
//	}
//} );



//var now2 = DateTime.Now;
//Log.Info( "END " + ( now2 - now ).TotalSeconds.ToString() );
//#endif


//another way. was slower

//var offsets = new List<Vector3I>( ( size.X + 1 ) * 2 * ( size.Y + 1 ) * 2 * ( size.Z + 1 ) * 2 );

//for( int z = -size.Z; z <= size.Z; z++ )
//{
//	for( int y = -size.Y; y <= size.Y; y++ )
//	{
//		for( int x = -size.X; x <= size.X; x++ )
//		{
//			if( x == 0 && y == 0 && z == 0 )
//				continue;

//			offsets.Add( new Vector3I( x, y, z ) );
//		}
//	}
//}

//CollectionUtility.MergeSort( offsets, delegate ( Vector3I offset1, Vector3I offset2 )
//{
//	var length1 = offset1.ToVector3F().LengthSquared();
//	var length2 = offset2.ToVector3F().LengthSquared();

//	if( length1 < length2 )
//		return -1;
//	else if( length1 > length2 )
//		return 1;
//	return 0;
//}, true );


//var voxelWithoutData = new List<Vector3I>( voxelIndexes.Length );
//foreach( var index in voxelIndexes )
//{
//	ref var voxel = ref voxels[ index.X, index.Y, index.Z ];
//	if( voxel.DataIndexInList == -1 )
//		voxelWithoutData.Add( index );
//}

//var now = DateTime.Now;
//Log.Info( "START" );

//Parallel.For( 0, voxelWithoutData.Count, delegate ( int index )
////Parallel.ForEach( voxelIndexes, delegate ( Vector3I index3 )
//{
//	var index3 = voxelWithoutData[ index ];
//	ref var voxel = ref voxels[ index3.X, index3.Y, index3.Z ];

//	//if( voxel.DataIndexInList == -1 )
//	{
//		var minIndex = new Vector3I( -1, -1, -1 );

//		foreach( var offset in offsets )
//		{
//			var indexTo = index3 + offset;
//			if( indexTo.X >= 0 && indexTo.X < size.X && indexTo.Y >= 0 && indexTo.Y < size.Y && indexTo.Z >= 0 && indexTo.Z < size.Z )
//			{
//				ref var voxelTo = ref voxels[ indexTo.X, indexTo.Y, indexTo.Z ];

//				if( voxelTo.DataIndexInList != -1 )
//				{
//					minIndex = indexTo;
//					break;
//				}
//			}
//		}


//		//var minDistanceSquared = int.MaxValue;
//		////var minIndex = new Vector3I( -1, -1, -1 );

//		//for( int n = 0; n < voxelsWithData.Count; n++ )
//		//{
//		//	ref var voxelWithData = ref voxelsWithData.Data[ n ];
//		//	var indexToCheck = voxelWithData.Index;

//		//	var xx = indexToCheck.X - index3.X;
//		//	var yy = indexToCheck.Y - index3.Y;
//		//	var zz = indexToCheck.Z - index3.Z;
//		//	var distanceSquared = xx * xx + yy * yy + zz * zz;
//		//	//var distanceSquared = ( indexToCheck - index3 ).ToVector3F().LengthSquared();

//		//	if( distanceSquared < minDistanceSquared )
//		//	{
//		//		minDistanceSquared = distanceSquared;
//		//		minIndex = indexToCheck;
//		//	}
//		//}

//		//!!!!
//		//если ничего нету
//		if( minIndex.X == -1 )
//			minIndex = new Vector3I( 0, 0, 0 );

//		voxel.NearestVoxelWithData = minIndex;
//	}
//} );








////create scene

//var scene = ComponentUtility.CreateComponent<Scene>( null, true, false );
//scene.DisplayDevelopmentDataInEditor = false;
//scene.OctreeEnabled = false;

//var pipeline = (RenderingPipeline_Basic)scene.CreateComponent( RenderingSystem.RenderingPipelineBasic );
//scene.RenderingPipeline = pipeline;
//pipeline.DeferredShading = AutoTrueFalse.False;


////scene.BackgroundColor = new ColorValue( 255, 0, 0 );
////scene.BackgroundColorAffectLighting = 1;
////scene.BackgroundColorEnvironmentOverride = new ColorValue( 0.8, 0.8, 0.8 );

////var backgroundEffects = pipeline.CreateComponent<Component>();
////backgroundEffects.Name = "Background Effects";

//var sceneEffects = pipeline.CreateComponent<Component>();
//sceneEffects.Name = "Scene Effects";

////ShowRenderTarget to get depth
//var showRenderTargetEffect = sceneEffects.CreateComponent<RenderingEffect_ShowRenderTarget>();
//showRenderTargetEffect.Texture = RenderingEffect_ShowRenderTarget.TextureType.Depth;
//showRenderTargetEffect.DepthMultiplier = 1;

////ambient light
////!!!!
////if( channel == MaterialChannel.Opacity )
////{
////	var light = scene.CreateComponent<Light>();
////	light.Type = Light.TypeEnum.Ambient;
////	light.Brightness = 1000000;// ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$General\\PreviewAmbientLightBrightness" );
////							   //light.Brightness = ProjectSettings.Get.PreviewAmbientLightBrightness.Value;
////}

//////directional light
////{
////	var light = scene.CreateComponent<Light>();
////	light.Type = Light.TypeEnum.Directional;
////	light.Transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.FromDirectionZAxisUp( new Vector3( 0, 0, -1 ) ), Vector3.One );
////	light.Brightness = ReferenceUtility.MakeReference( "Base\\ProjectSettings.component|$General\\PreviewDirectionalLightBrightness" );
////	//light.Brightness = ProjectSettings.Get.PreviewDirectionalLightBrightness.Value;
////	light.Shadows = false;
////	//light.Type = Light.TypeEnum.Point;
////	//light.Transform = new Transform( new Vec3( 0, 0, 2 ), Quat.Identity, Vec3.One );
////}

////create mesh
//var mesh = scene.CreateComponent<Mesh>();

//var meshGeometry = mesh.CreateComponent<MeshGeometry>();
//meshGeometry.Name = sourceGeometry.Name;
//meshGeometry.VertexStructure = sourceGeometry.VertexStructure;
//meshGeometry.UnwrappedUV = sourceGeometry.UnwrappedUV;
//meshGeometry.Vertices = sourceGeometry.Vertices;
//meshGeometry.Indices = sourceGeometry.Indices;
//meshGeometry.Material = sourceGeometry.Material;


//var meshInSpace = scene.CreateComponent<MeshInSpace>();
//meshInSpace.Transform = new Transform( -boundingSphere.Center, Quaternion.Identity );

//meshInSpace.Mesh = ReferenceUtility.MakeRootReference( mesh );

////enable the scene
//scene.Enabled = true;


//ImageComponent texture = null;
//ImageComponent textureRead = null;


//try
//{
//	var format = PixelFormat.Float32RGBA;

//	texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	texture.CreateType = ImageComponent.TypeEnum._2D;
//	texture.CreateSize = new Vector2I( imageSize, imageSize );
//	texture.CreateMipmaps = false;
//	texture.CreateFormat = format;
//	texture.CreateUsage = ImageComponent.Usages.RenderTarget;
//	texture.CreateFSAA = 0;
//	texture.Enabled = true;

//	var renderTexture = texture.Result.GetRenderTarget();
//	var viewport = renderTexture.AddViewport( false, false );
//	viewport.AttachedScene = scene;

//	textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	textureRead.CreateType = ImageComponent.TypeEnum._2D;
//	textureRead.CreateSize = new Vector2I( imageSize, imageSize );
//	textureRead.CreateMipmaps = false;
//	textureRead.CreateFormat = format;
//	textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
//	textureRead.CreateFSAA = 0;
//	textureRead.Enabled = true;

//	var currentResultDataOffset = sizeof( MeshGeometry.VoxelDataHeader );

//	for( int nImage = 0; nImage < imageCount; nImage++ )
//	{
//		var result = new ImageUtility.Image2D( PixelFormat.Float32RGBA, new Vector2I( imageSize, imageSize ) );
//		Vector2I[,] opacityImageNearestCellTable = null;


//		//!!!!режим Clamp или как сейчас отступ оставлять

//		//!!!!

//		//double maxOffset;
//		//{
//		//	var offsetY = Math.Max( Math.Abs( bounds.Minimum.Y ), Math.Abs( bounds.Maximum.Y ) );
//		//	//empty border
//		//	offsetY *= 1.2;

//		//	//!!!!выровнить
//		//	var offsetZ = Math.Max( Math.Abs( bounds.Minimum.Z ), Math.Abs( bounds.Maximum.Z ) );
//		//	//empty border
//		//	offsetZ *= 1.2;

//		//	maxOffset = Math.Max( offsetY, offsetZ );
//		//}

//		var height = boundingSphere.Radius * 2;// boundingRadius * 2;

//		//var height = maxOffset * 2;

//		//!!!!
//		//var center = new Vector3F( 0, 0, 0 );
//		//var center = Bounds.GetCenter();

//		var cameraPosition = Vector3.Zero;

//		if( header.Mode == MeshGeometry.BillboardDataModeEnum._1Direction )
//		{
//			cameraPosition = new Vector3( 1, 0, 0 );
//		}
//		else if( header.Mode == MeshGeometry.BillboardDataModeEnum._5Directions )
//		{
//			switch( nImage )
//			{
//			case 0: cameraPosition = new Vector3( 0, 0, -1 ); break;
//			case 1: cameraPosition = new Vector3( 1, 0, -1 ); break;
//			case 2: cameraPosition = new Vector3( 1, 0, 0 ); break;
//			case 3: cameraPosition = new Vector3( 1, 0, 1 ); break;
//			case 4: cameraPosition = new Vector3( 0, 0, 1 ); break;
//			}

//			//switch( nImage )
//			//{
//			//case 0: cameraPosition = new Vector3( 1, 0, -1 ); break;
//			//case 1: cameraPosition = new Vector3( 1, 0, 0 ); break;
//			//case 2: cameraPosition = new Vector3( 1, 0, 1 ); break;
//			//case 3: cameraPosition = new Vector3( 0.0001, 0, 1 ); break;
//			//}
//		}
//		else if( header.Mode == MeshGeometry.BillboardDataModeEnum._26Directions )
//		{
//			var cameraPositions = new Vector3[ 26 ]
//			{
//				new Vector3(-1, -1, -1), //0
//				new Vector3(0, -1, -1), //1
//				new Vector3(1, -1, -1), //2
//				new Vector3(-1, 0, -1), //3
//				new Vector3(0, 0, -1), //4
//				new Vector3(1, 0, -1), //5
//				new Vector3(-1, 1, -1), //6
//				new Vector3(0, 1, -1), //7
//				new Vector3(1, 1, -1), //8
//				new Vector3(-1, -1, 0), //9
//				new Vector3(0, -1, 0), //10
//				new Vector3(1, -1, 0), //11
//				new Vector3(-1, 0, 0), //12
//				new Vector3(1, 0, 0), //13
//				new Vector3(-1, 1, 0), //14
//				new Vector3(0, 1, 0), //15
//				new Vector3(1, 1, 0), //16
//				new Vector3(-1, -1, 1), //17
//				new Vector3(0, -1, 1), //18
//				new Vector3(1, -1, 1), //19
//				new Vector3(-1, 0, 1), //20
//				new Vector3(0, 0, 1), //21
//				new Vector3(1, 0, 1), //22
//				new Vector3(-1, 1, 1), //23
//				new Vector3(0, 1, 1), //24
//				new Vector3(1, 1, 1) //25
//			};

//			cameraPosition = cameraPositions[ nImage ];
//		}

//		cameraPosition.Normalize();

//		var up = Vector3.ZAxis;
//		if( cameraPosition.Z > 0.99 )
//			up = Vector3.XAxis;
//		if( cameraPosition.Z < -0.99 )
//			up = -Vector3.XAxis;

//		var cameraDirection = -cameraPosition;
//		cameraPosition *= cameraDistance;

//		var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, 0.1, cameraDistance * 2, cameraPosition, cameraDirection, up, ProjectionType.Orthographic, height, 1, 1 );


//		foreach( var channel2 in Enum.GetValues( typeof( Channel ) ) )
//		{
//			var channel = (Channel)channel2;

//			if( channel == Channel.Depth && !needDepthBuffer )
//				continue;

//			//!!!!dds. mipmaps


//			if( channel == Channel.Opacity )
//				pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal;
//			if( channel == Channel.TextureCoordinate0 )
//				pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.TextureCoordinate0;
//			if( channel == Channel.Depth )
//				pipeline.DebugMode = RenderingPipeline_Basic.DebugModeEnum.Normal;
//			//!!!!

//			showRenderTargetEffect.Enabled = channel == Channel.Depth;


//			viewport.Update( true, cameraSettings );

//			texture.Result.GetNativeObject( true ).BlitTo( viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

//			//get data
//			var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * imageSize * imageSize;
//			var data = new byte[ totalBytes ];
//			unsafe
//			{
//				fixed( byte* pBytes = data )
//				{
//					var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );
//					while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
//				}
//			}

//			var image = new ImageUtility.Image2D( format, new Vector2I( imageSize, imageSize ), data );


//			if( channel == Channel.Opacity )
//			{
//				var rotationNormals = Quaternion.LookAt( -cameraDirection, up ).GetInverse().ToQuaternionF();

//				for( int y = 0; y < image.Size.Y; y++ )
//				{
//					for( int x = 0; x < image.Size.X; x++ )
//					{
//						var pixel = image.GetPixel( new Vector2I( x, y ) );

//						var v = pixel.ToVector3F();
//						if( v != Vector3F.Zero )
//						{
//							v = ( v * 2.0f - Vector3F.One ).GetNormalize();

//							//rotate normal to object space
//							v = rotationNormals * v;

//							var dir = SphericalDirectionF.FromVector( v );
//							result.SetPixel( new Vector2I( x, y ), new Vector4F( dir.Horizontal, dir.Vertical, 0, 0 ) );
//						}
//						else
//							result.SetPixel( new Vector2I( x, y ), new Vector4F( 100.0f, 0, 0, 0 ) );
//					}
//				}

//				//opacityImageNearestCellTable
//				if( FillTransparentPixelsByNearPixels )
//				{
//					var boolOpacityImage = new int[ image.Size.X, image.Size.Y ];
//					for( int y = 0; y < image.Size.Y; y++ )
//					{
//						for( int x = 0; x < image.Size.X; x++ )
//						{
//							var c = image.GetPixel( new Vector2I( x, y ) );
//							boolOpacityImage[ x, y ] = c.X > 50.0f ? 1 : 0;
//							//boolOpacityImage[ x, y ] = c.ToVector3F() == Vector3F.Zero ? 1 : 0;
//						}
//					}

//					var distanceMap = GetDistanceMap( image );

//					opacityImageNearestCellTable = new Vector2I[ image.Size.X, image.Size.Y ];
//					for( int y = 0; y < image.Size.Y; y++ )
//						for( int x = 0; x < image.Size.X; x++ )
//							opacityImageNearestCellTable[ x, y ] = new Vector2I( x, y );

//					var imageSizeX = image.Size.X;
//					var imageSizeY = image.Size.Y;

//					Parallel.For( 0, image.Size.X * image.Size.Y, delegate ( int xy )
//					{
//						var y = xy / imageSizeX;
//						var x = xy % imageSizeX;

//						var transparent = boolOpacityImage[ x, y ];
//						if( transparent != 0 )//if( transparent )
//						{
//							for( int n = 0; n < distanceMap.Length; n++ )
//							{
//								ref var indexItem = ref distanceMap[ n ];

//								var takeFromX = x + indexItem.X;
//								var takeFromY = y + indexItem.Y;
//								if( takeFromX >= 0 && takeFromX < imageSizeX && takeFromY >= 0 && takeFromY < imageSizeY )
//								{
//									var transparent2 = boolOpacityImage[ takeFromX, takeFromY ];
//									if( transparent2 == 0 )//if( !transparent2 )
//									{
//										opacityImageNearestCellTable[ x, y ] = new Vector2I( takeFromX, takeFromY );
//										break;
//									}
//								}
//							}
//						}
//					} );
//				}
//			}

//			if( channel == Channel.TextureCoordinate0 )
//			{
//				//fill transparent pixels by near pixels
//				if( opacityImageNearestCellTable != null )
//					FillTransparentPixelsByNearPixels2( ref image, opacityImageNearestCellTable );

//				for( int y = 0; y < image.Size.Y; y++ )
//				{
//					for( int x = 0; x < image.Size.X; x++ )
//					{
//						var pixel = image.GetPixel( new Vector2I( x, y ) );

//						var current = result.GetPixel( new Vector2I( x, y ) );
//						current.Z = pixel.X;
//						current.W = pixel.Y;
//						result.SetPixel( new Vector2I( x, y ), current );
//					}
//				}
//			}

//			if( channel == Channel.Depth )
//			{
//				var depthBuffer = new float[ image.Size.X, image.Size.Y ];

//				for( int y = 0; y < image.Size.Y; y++ )
//				{
//					for( int x = 0; x < image.Size.X; x++ )
//					{
//						var pixel = image.GetPixel( new Vector2I( x, y ) );
//						depthBuffer[ x, y ] = pixel.X;
//					}
//				}

//				depthBuffers.depthBuffers.Add( depthBuffer );
//			}

//			//!!!!
//			//if( channel == MaterialChannel.BaseColor || channel == MaterialChannel.Roughness || channel == MaterialChannel.Normal )
//			//{
//			//	//rotate normal map
//			//	if( channel == MaterialChannel.Normal )
//			//	{
//			//		//var rot = QuaternionF.Identity;
//			//		var rot = QuaternionF.FromRotateByY( new RadianF( MathEx.PI / 2 ) );
//			//		//var rot2 = QuaternionF.FromRotateByX( new RadianF( MathEx.PI / 2 ) );

//			//		for( int y = 0; y < image.Size.Y; y++ )
//			//		{
//			//			for( int x = 0; x < image.Size.X; x++ )
//			//			{
//			//				var pixel = image.GetPixel( new Vector2I( x, y ) );

//			//				var vector = pixel.ToVector3F();
//			//				vector -= new Vector3F( 0.5f, 0.5f, 0.5f );
//			//				vector *= 2.0f;

//			//				//vector = rot2 * vector;
//			//				vector = rot * vector;
//			//				//vector = rot2 * vector;

//			//				vector.Normalize();

//			//				vector *= 0.5f;
//			//				vector += new Vector3F( 0.5f, 0.5f, 0.5f );

//			//				pixel = new Vector4F( vector, pixel.W );

//			//				image.SetPixel( new Vector2I( x, y ), pixel );
//			//			}
//			//		}
//			//	}

//			//}

//		}

//		//convert Float to Half
//		byte[] halfArray = new byte[ result.Data.Length / 2 ];
//		fixed( byte* pHalfArray = halfArray )
//		{
//			fixed( byte* pFloatArray = result.Data )
//			{
//				HalfType* pHalf = (HalfType*)pHalfArray;
//				float* pFloat = (float*)pFloatArray;

//				for( int n = 0; n < halfArray.Length / 2; n++ )
//				{
//					*pHalf = new HalfType( *pFloat );
//					pHalf++;
//					pFloat++;
//				}
//			}
//		}

//		Array.Copy( halfArray, 0, resultData, currentResultDataOffset, imageSizeInBytes );


//		//Array.Copy( result.Data, 0, resultData, currentResultDataOffset, imageSizeInBytes );
//		currentResultDataOffset += imageSizeInBytes;
//	}
//}
//finally
//{
//	texture?.Dispose();
//	textureRead?.Dispose();
//	scene?.Dispose();
//}
