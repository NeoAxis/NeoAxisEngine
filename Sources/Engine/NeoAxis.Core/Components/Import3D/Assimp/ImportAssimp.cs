// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Internal.Assimp;
using Internal.Assimp.Configs;

namespace NeoAxis
{
	class ImportAssimp : ImportGeneral
	{
		static bool initialized;

		//

		public static void Initialize()
		{
			if( initialized )
				return;

			LoadNativeLibrary();

			//initialize logs
			//importer.VerboseLoggingEnabled = true;

			LogStream logStream = new LogStream( delegate ( string message, string userData )
			{
				message = message.Trim( new char[] { '\r', '\n' } );

				if( message.Length > 5 && message.Substring( 0, 5 ) == "Error" )
					Log.InvisibleInfo( "Import3D: Warning: " + message );
				//else
				//	Log.InvisibleInfo( "Assimp Import Library: " + message );
			} );

			logStream.Attach();

			initialized = true;
		}

		static void LoadNativeLibrary()
		{
			string libraryName = "assimp-vc142-mt";
			//string libraryName = "assimp-vc143-mt";

			NativeUtility.PreloadLibrary( libraryName );

			var assimpLibrary = global::Internal.Assimp.Unmanaged.AssimpLibrary.Instance;
			if( !assimpLibrary.IsLibraryLoaded )
			{
				string path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, libraryName + ".dll" );
				assimpLibrary.LoadLibrary( path );
			}
		}

		class ImportContext
		{
			public Internal.Assimp.Scene scene;
			public Settings settings;
			public string directoryName;
			public Component materialsGroup;
			public ESet<Internal.Assimp.Mesh> processedMeshes = new ESet<Internal.Assimp.Mesh>();
			public EDictionary<int, Material> materialByIndex = new EDictionary<int, Material>();

			public Matrix4 globalTransform;

			public MeshesSkeletonStructure skeletonStructure;
		}

		//!!!!?
		//static bool HasTransformMatrixNegParity( Matrix3F m )
		//{
		//	return Vector3F.Dot( Vector3F.Cross( m.Item0, m.Item1 ), m.Item2 ) < 0.0f ? true : false;
		//}

		static void AddMesh( ImportContext importContext, Matrix4 nodeTransform, Mesh meshComponent, Internal.Assimp.Mesh mesh ) //, int[] newIndexFromOldIndex )
		{
			var vertices = new StandardVertex[ mesh.VertexCount ];
			var transformedVertices = new bool[ mesh.VertexCount ];

			bool hasVertexColor = mesh.HasVertexColors( 0 );
			List<System.Numerics.Vector4> colors0 = null;
			var allColorAlphaIsZero = false;
			if( hasVertexColor )
			{
				colors0 = mesh.VertexColorChannels[ 0 ];

				allColorAlphaIsZero = true;
				for( int n = 0; n < colors0.Count; n++ )
				{
					if( colors0[ n ].W != 0 )
					{
						allColorAlphaIsZero = false;
						break;
					}
				}
			}

			int textureCoordsCount = 0;
			for( int n = 0; n < 4 && n < mesh.TextureCoordinateChannelCount; n++ )
			{
				if( mesh.HasTextureCoords( n ) )
					textureCoordsCount++;
				else
					break;
			}
			var texCoords0 = textureCoordsCount > 0 ? mesh.TextureCoordinateChannels[ 0 ] : null;
			var texCoords1 = textureCoordsCount > 1 ? mesh.TextureCoordinateChannels[ 1 ] : null;
			var texCoords2 = textureCoordsCount > 2 ? mesh.TextureCoordinateChannels[ 2 ] : null;
			var texCoords3 = textureCoordsCount > 3 ? mesh.TextureCoordinateChannels[ 3 ] : null;

			////if( HasTransformMatrixNegParity( geometryTransform .GetTranspose().ToMat3() ) )
			////{
			////   //what to do?
			////}

			//get data
			for( int n = 0; n < vertices.Length; n++ )
			{
				var vertex = new StandardVertex();

				if( textureCoordsCount > 0 )
				{
					vertex.TexCoord0 = ToVector2F( texCoords0[ n ] );
					if( textureCoordsCount > 1 )
					{
						vertex.TexCoord1 = ToVector2F( texCoords1[ n ] );
						if( textureCoordsCount > 2 )
						{
							vertex.TexCoord2 = ToVector2F( texCoords2[ n ] );
							if( textureCoordsCount > 3 )
								vertex.TexCoord3 = ToVector2F( texCoords3[ n ] );
						}
					}
				}

				if( hasVertexColor )
				{
					vertex.Color = ToColorValue( colors0[ n ] );
					if( allColorAlphaIsZero )
						vertex.Color.Alpha = 1;
				}

				vertex.BlendIndices = new Vector4I( -1, -1, -1, -1 );
				vertex.BlendWeights = new Vector4F( 0, 0, 0, 0 );

				vertices[ n ] = vertex;
			}

			var meshBoneByBoneName = new Dictionary<string, Bone>();

			var skeletonStructure = importContext.skeletonStructure;
			if( skeletonStructure != null && mesh.HasBones )
			{
				for( int nBone = 0; nBone < mesh.Bones.Count; nBone++ )
				{
					var bone = mesh.Bones[ nBone ];

					meshBoneByBoneName[ bone.Name ] = bone;

					foreach( var weight in bone.VertexWeights )
					{
						if( weight.VertexID >= vertices.Length )
							continue;
						if( weight.Weight <= 0 )
							continue;

						ref var vertex = ref vertices[ weight.VertexID ];
						for( int n = 0; n < 4; n++ )
						{
							if( vertex.BlendIndices[ n ] == -1 )
							{
								vertex.BlendIndices[ n ] = nBone;
								vertex.BlendWeights[ n ] = weight.Weight;
								break;
							}
						}
					}
				}
			}

			int[] indices = new int[ mesh.FaceCount * 3 ];
			for( int n = 0; n < mesh.FaceCount; n++ )
			{
				Face face = mesh.Faces[ n ];
				indices[ n * 3 + 0 ] = face.Indices[ 0 ];
				indices[ n * 3 + 1 ] = face.Indices[ 1 ];
				indices[ n * 3 + 2 ] = face.Indices[ 2 ];
			}

			var geometry = meshComponent.CreateComponent<MeshGeometry>();
			geometry.Name = GetFixedName( mesh.Name );

			var vertexComponents = StandardVertex.Components.Position;
			if( mesh.HasNormals )
				vertexComponents |= StandardVertex.Components.Normal;
			if( mesh.HasTangentBasis )
				vertexComponents |= StandardVertex.Components.Tangent;
			if( hasVertexColor )
				vertexComponents |= StandardVertex.Components.Color;
			if( textureCoordsCount > 0 )
				vertexComponents |= StandardVertex.Components.TexCoord0;
			if( textureCoordsCount > 1 )
				vertexComponents |= StandardVertex.Components.TexCoord1;
			if( textureCoordsCount > 2 )
				vertexComponents |= StandardVertex.Components.TexCoord2;
			if( textureCoordsCount > 3 )
				vertexComponents |= StandardVertex.Components.TexCoord3;

			//affect blend indices

			if( skeletonStructure != null && mesh.HasBones )
			{
				vertexComponents |= StandardVertex.Components.BlendIndices | StandardVertex.Components.BlendWeights;

				var meshBones = mesh.Bones;

				var boneNodeArray = skeletonStructure.boneNodeByName.Values.ToArray();

				for( int nVertex = 0; nVertex < vertices.Length; nVertex++ )
				{
					ref Vector4I bi = ref vertices[ nVertex ].BlendIndices;
					if( bi.X != -1 )
						skeletonStructure.boneIndexByName.TryGetValue( meshBones[ bi.X ].Name, out bi.X );
					if( bi.Y != -1 )
						skeletonStructure.boneIndexByName.TryGetValue( meshBones[ bi.Y ].Name, out bi.Y );
					if( bi.Z != -1 )
						skeletonStructure.boneIndexByName.TryGetValue( meshBones[ bi.Z ].Name, out bi.Z );
					if( bi.W != -1 )
						skeletonStructure.boneIndexByName.TryGetValue( meshBones[ bi.W ].Name, out bi.W );
				}

				//transform vertices
				for( int nVertex = 0; nVertex < vertices.Length; nVertex++ )
				{
					ref var vertex = ref vertices[ nVertex ];

					//var sourcePosition = vertex.Position;
					var outputPosition = Vector3.Zero;
					var outputNormal = Vector3.Zero;
					var outputTangent = Vector3.Zero;
					var outputBinormal = Vector3.Zero;

					var affected = false;

					for( int n = 0; n < 4; n++ )
					{
						var boneIndex = vertex.BlendIndices[ n ];
						var boneWeight = vertex.BlendWeights[ n ];

						if( boneIndex >= 0 && boneWeight > 0 )
						{
							var boneNode = boneNodeArray[ boneIndex ];
							var bone = meshBoneByBoneName[ boneNode.Name ];

							//get global transform for bone in bind pose and apply offset matrix to get final bone matrix for transforming vertex
							var globalBoneTransform = GetNodeFullTransform( importContext, boneNode );

							//merge offset matrix and global transform to get final bone matrix for transforming vertex
							var finalBoneMatrix = globalBoneTransform * ToMatrix4( bone.OffsetMatrix );

							//transform vertex position by final bone matrix and accumulate with weight
							var position = finalBoneMatrix * ToVector3( mesh.Vertices[ nVertex ] );
							outputPosition += position * boneWeight;

							finalBoneMatrix.Decompose( out _, out Matrix3 finalBoneMatrixR, out _ );

							if( mesh.HasNormals )
							{
								var normal = ( finalBoneMatrixR * ToVector3( mesh.Normals[ nVertex ] ) ).GetNormalize();
								outputNormal += normal * boneWeight;
							}

							if( mesh.HasTangentBasis )
							{
								var tangent = ( finalBoneMatrixR * ToVector3( mesh.Tangents[ nVertex ] ) ).GetNormalize();
								var binormal = ( finalBoneMatrixR * ToVector3( mesh.BiTangents[ nVertex ] ) ).GetNormalize();
								outputTangent = tangent * boneWeight;
								outputBinormal = binormal * boneWeight;
							}

							affected = true;
						}
					}

					if( affected )
					{
						vertex.Position = outputPosition.ToVector3F();
						if( mesh.HasNormals )
							vertex.Normal = outputNormal.ToVector3F().GetNormalize();
						if( mesh.HasTangentBasis )
						{
							var parity = ( Vector3.Dot( Vector3.Cross( outputTangent, outputBinormal ), vertex.Normal ) >= 0 ) ? -1 : 1;
							vertex.Tangent = new Vector4F( outputTangent.ToVector3F(), parity );
						}
					}

					transformedVertices[ nVertex ] = affected;
				}
			}

			//transform vertices by node transform if no skeleton or bones, or if some vertices are not affected by bones
			{
				var geometryTransform = nodeTransform;
				geometryTransform.Decompose( out _, out Matrix3 geometryTransformR, out _ );

				for( int nVertex = 0; nVertex < transformedVertices.Length; nVertex++ )
				{
					if( !transformedVertices[ nVertex ] )
					{
						ref var vertex = ref vertices[ nVertex ];

						vertex.Position = ( geometryTransform * ToVector3( mesh.Vertices[ nVertex ] ) ).ToVector3F();
						if( mesh.HasNormals )
							vertex.Normal = ( geometryTransformR * ToVector3( mesh.Normals[ nVertex ] ) ).ToVector3F().GetNormalize();
						if( mesh.HasTangentBasis )
						{
							var tangent = ( geometryTransformR * ToVector3( mesh.Tangents[ nVertex ] ) ).GetNormalize();
							var binormal = ( geometryTransformR * ToVector3( mesh.BiTangents[ nVertex ] ) ).GetNormalize();
							var parity = ( Vector3.Dot( Vector3.Cross( tangent, binormal ), vertex.Normal ) >= 0 ) ? -1 : 1;
							vertex.Tangent = new Vector4F( tangent.ToVector3F(), parity );
						}
					}
				}
			}

			geometry.SetVertexData( vertices, vertexComponents );
			geometry.Indices = indices;

			//material
			importContext.materialByIndex.TryGetValue( mesh.MaterialIndex, out Material material );
			if( material != null )
			{
				var referenceValue = ReferenceUtility.CalculateRootReference( material );
				geometry.Material = ReferenceUtility.MakeReference<Material>( null, referenceValue );
			}

			importContext.processedMeshes.Add( mesh );
		}

		static void InitMeshGeometriesRecursive( ImportContext importContext, Node node, Matrix4 nodeTransform, Mesh meshComponent ) //, int[] newIndexFromOldIndex )
		{
			foreach( var meshIndex in node.MeshIndices )
			{
				var mesh = importContext.scene.Meshes[ meshIndex ];
				AddMesh( importContext, nodeTransform, meshComponent, mesh );//, newIndexFromOldIndex );
			}

			foreach( var childNode in node.Children )
			{
				var childTransform = nodeTransform * ToMatrix4( childNode.Transform );
				InitMeshGeometriesRecursive( importContext, childNode, childTransform, meshComponent );//, newIndexFromOldIndex );
			}
		}

		static Matrix4 GetNodeFullTransform( ImportContext importContext, Node node )
		{
			var transform = Matrix4.Identity;
			var node2 = node;
			while( node2 != null )
			{
				transform = ToMatrix4( node2.Transform ) * transform;
				node2 = node2.Parent;
			}

			transform = importContext.globalTransform * transform;

			return transform;
		}

		public static void DoImport( Settings settings, out string error )
		{
			error = "";

			Initialize();

			try
			{
				using( AssimpContext assimpContext = new AssimpContext() )
				{
					//configure

					assimpContext.SetConfig( new SortByPrimitiveTypeConfig( PrimitiveType.Line | PrimitiveType.Point ) );
					assimpContext.Scale = (float)settings.component.Scale;

					//works incorrectly. adding rotation manually.
					//context.XAxisRotation = -90;

					//175 by default
					//importer.SetConfig( new NormalSmoothingAngleConfig( 55.0f ) );

					PostProcessSteps flags =
						PostProcessSteps.CalculateTangentSpace |
						//PostProcessSteps.JoinIdenticalVertices |
						//PostProcessSteps.MakeLeftHanded |
						PostProcessSteps.Triangulate |
						PostProcessSteps.RemoveComponent |
						PostProcessSteps.GenerateSmoothNormals |
						//PostProcessSteps.SplitLargeMeshes |
						//PostProcessSteps.PreTransformVertices |
						PostProcessSteps.LimitBoneWeights |
						//PostProcessSteps.ValidateDataStructure |
						//PostProcessSteps.ImproveCacheLocality |
						//PostProcessSteps.RemoveRedundantMaterials |
						//PostProcessSteps.FixInFacingNormals | 
						PostProcessSteps.SortByPrimitiveType |
						PostProcessSteps.FindDegenerates |
						PostProcessSteps.FindInvalidData |
						PostProcessSteps.GenerateUVCoords |
						//PostProcessSteps.TransformUVCoords |
						//PostProcessSteps.FindInstances |
						PostProcessSteps.OptimizeMeshes |
						//PostProcessSteps.OptimizeGraph |
						//PostProcessSteps.FlipWindingOrder |
						//PostProcessSteps.SplitByBoneCount |
						//PostProcessSteps.Debone |
						0;

					//flip with disabled FlipUVs, no flip when enabled
					if( !settings.component.FlipUVs )
						flags |= PostProcessSteps.FlipUVs;

					Internal.Assimp.Scene scene = null;
					VirtualFileStream stream = null;

					var realFileName = VirtualPathUtility.GetRealPathByVirtual( settings.virtualFileName );
					if( !string.IsNullOrEmpty( realFileName ) && File.Exists( realFileName ) )
					{
						scene = assimpContext.ImportFile( realFileName, flags );
					}
					else if( VirtualFile.Exists( settings.virtualFileName ) )
					{
						stream = VirtualFile.Open( settings.virtualFileName );
						string formatHint = Path.GetExtension( settings.virtualFileName ).Replace( ".", "" ).ToLower();
						scene = assimpContext.ImportFileFromStream( stream, flags, formatHint );
					}
					else
					{
						error = "File is not exists.";
						assimpContext.Dispose();
						return;
					}

					if( scene == null )
					{
						error = "(NO ERROR MESSAGE)";
						assimpContext.Dispose();
						return;
					}


					var context = new ImportContext();
					context.scene = scene;
					context.settings = settings;
					context.directoryName = Path.GetDirectoryName( settings.virtualFileName );

					//get materials data
					var materialsData = GetMaterialsData( context );

					//create Materials group
					context.materialsGroup = context.settings.component.GetComponent( "Materials" );
					if( context.materialsGroup == null && materialsData.Count != 0 && settings.updateMaterials )
					{
						context.materialsGroup = context.settings.component.CreateComponent<Component>();
						context.materialsGroup.Name = "Materials";
					}

					//create materials
					foreach( var data in materialsData )
					{
						Material material = null;
						if( context.settings.updateMaterials )
							material = CreateMaterial( settings, context.materialsGroup, data );
						else
						{
							if( context.materialsGroup != null )
								material = context.materialsGroup.GetComponent( data.Name ) as Material;
						}
						if( material != null )
							context.materialByIndex.Add( data.Index, material );
					}

					var rotateByX = Matrix3.Identity;
					if( settings.component.FixAxes )
						rotateByX = new Matrix3( 1, 0, 0, 0, 0, 1, 0, -1, 0 );
					var globalTransform2 = new Matrix4( settings.component.Rotation.Value.ToMatrix3() * rotateByX, settings.component.Position );
					context.globalTransform = globalTransform2;

					//Matrix4 globalTransform = new Matrix4( rotation * rotateByX, settings.component.Position ) * ToMatrix4( scene.RootNode.Transform );

					var mode = settings.component.Mode.Value;

					//create one mesh (OneMesh mode)
					if( mode == Import3D.ModeEnum.OneMesh && scene.HasMeshes && scene.MeshCount != 0 ) //&& settings.updateMeshes )
					{
						//skeleton and animations
						//var boneTransformsToNormalize = new Dictionary<SkeletonBone, Matrix4>( 128 );
						Skeleton skeletonComponent = null;
						//int[] newIndexFromOldIndex = null;
						skeletonComponent = CreateSkeletonComponent( context, scene );///*, out newIndexFromOldIndex, out var oldBoneFromNewIndex*/, globalTransform );//, out var addedBones );//, boneTransformsToNormalize );


						//!!!!
						//try
						//{
						//	Log.Info( "----------" );
						//	var equalCheck = new Dictionary<string, string>();

						//	void ShowLogRecursive( Node node, int level )
						//	{
						//		var mat = ToMatrix4( node.Transform );
						//		Matrix4Round( ref mat );

						//		Log.Info( $"{new string( ' ', level * 2 )} Node: " + node.Name + " " + node.MeshCount.ToString() );// + " " + mat.ToString() );

						//		if( equalCheck.TryGetValue( node.Name, out var tr ) )
						//		{
						//			if( tr != node.Transform.ToString() )
						//				throw new Exception( "Not equal transform for node with name: " + node.Name );
						//		}
						//		else
						//			equalCheck.Add( node.Name, node.Transform.ToString() );

						//		foreach( var childNode in node.Children )
						//			ShowLogRecursive( childNode, level + 1 );
						//	}

						//	ShowLogRecursive( scene.RootNode, 0 );
						//	Log.Info( "---------- END" );
						//}
						//catch( Exception e )
						//{
						//	Log.Info( "Exception: " + e.Message );
						//}


						//mesh
						var meshComponent = settings.component.CreateComponent<Mesh>( enabled: false );
						meshComponent.Name = "Mesh";

						foreach( var node in scene.RootNode.Children )
						{
							var transform = GetNodeFullTransform( context, node );
							InitMeshGeometriesRecursive( context, node, transform, meshComponent );//, newIndexFromOldIndex );
						}

						//need
						//if( meshComponent.Components.Count == 0 )
						{
							var transform = GetNodeFullTransform( context, scene.RootNode );

							foreach( var mesh in scene.Meshes )
							{
								if( !context.processedMeshes.Contains( mesh ) )
									AddMesh( context, transform, meshComponent, mesh );//, newIndexFromOldIndex );
							}
						}

						//skeleton and animations
						if( skeletonComponent != null )
						{
							meshComponent.AddComponent( skeletonComponent );
							meshComponent.Skeleton = ReferenceUtility.MakeThisReference( meshComponent, skeletonComponent );
							InitAnimations( context, scene, meshComponent/*, oldBoneFromNewIndex*/);//, globalTransform );//, addedBones ); //, boneTransformsToNormalize );
						}

						if( settings.component.MergeGeometries.Value != Import3D.MergeGeometriesEnum.False )
							meshComponent.MergeGeometriesWithEqualVertexStructureAndMaterial();

						meshComponent.Enabled = true;
					}

					//create meshes, object in space (Meshes mode)
					if( mode == Import3D.ModeEnum.Meshes && scene.HasMeshes && scene.MeshCount != 0 )
					{
						//skeleton and animations
						//!!!!is not enabled in Meshes mode
						//int[] newIndexFromOldIndex = null;

						var meshesGroup = settings.component.GetComponent( "Meshes" );

						//Meshes
						//if( settings.updateMeshes )
						{
							meshesGroup = settings.component.CreateComponent<Component>( enabled: false );
							meshesGroup.Name = "Meshes";

							foreach( var node in scene.RootNode.Children )
							{
								var transform = GetNodeFullTransform( context, node );

								var meshComponent = meshesGroup.CreateComponent<Mesh>();
								InitMeshGeometriesRecursive( context, node, transform, meshComponent );//, newIndexFromOldIndex );

								if( meshComponent.Components.Count != 0 )
								{
									if( string.IsNullOrEmpty( node.Name ) )
										meshComponent.Name = meshComponent.Components.ToArray()[ 0 ].Name;
									else
										meshComponent.Name = node.Name;

									//skeleton and animations
									//!!!!is not enabled in Meshes mode

									if( settings.component.MergeGeometries.Value != Import3D.MergeGeometriesEnum.False )
										meshComponent.MergeGeometriesWithEqualVertexStructureAndMaterial();
								}
								else
									meshComponent.Dispose();
							}

							//need
							//if( meshesGroup.Components.Count == 0 )
							{
								var transform = GetNodeFullTransform( context, scene.RootNode );

								foreach( var mesh in scene.Meshes )
								{
									if( !context.processedMeshes.Contains( mesh ) )
									{
										var meshComponent = meshesGroup.CreateComponent<Mesh>();
										AddMesh( context, transform, meshComponent, mesh );//, newIndexFromOldIndex );

										if( meshComponent.Components.Count != 0 )
										{
											meshComponent.Name = meshComponent.Components.ToArray()[ 0 ].Name;

											//skeleton and animations
											//!!!!is not enabled in Meshes mode

											if( settings.component.MergeGeometries.Value != Import3D.MergeGeometriesEnum.False )
												meshComponent.MergeGeometriesWithEqualVertexStructureAndMaterial();
										}
										else
											meshComponent.Dispose();
									}
								}
							}

							meshesGroup.Enabled = true;
						}

						//////Object In Space
						////if( settings.updateObjectsInSpace && meshesGroup != null )
						////{
						////	var objectInSpace = settings.component.CreateComponent<ObjectInSpace>( enabled: false );
						////	objectInSpace.Name = "Object In Space";

						////	foreach( var mesh in meshesGroup.Components )
						////	{
						////		var meshInSpace = objectInSpace.CreateComponent<MeshInSpace>();
						////		meshInSpace.Name = mesh.Name;
						////		meshInSpace.CanBeSelected = false;
						////		meshInSpace.Mesh = ReferenceUtility.MakeReference<Mesh>( null, ReferenceUtility.CalculateRootReference( mesh ) );

						////		//Transform
						////		//!!!!transform?
						////		var pos = Vector3.Zero;
						////		var rot = Quaternion.Identity;
						////		var scl = Vector3.One;
						////		//( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );

						////		var transformOffset = meshInSpace.CreateComponent<TransformOffset>();
						////		transformOffset.Name = "Transform Offset";
						////		transformOffset.PositionOffset = pos;
						////		transformOffset.RotationOffset = rot;
						////		transformOffset.ScaleOffset = scl;
						////		transformOffset.Source = ReferenceUtility.MakeReference<Transform>( null,
						////			ReferenceUtility.CalculateThisReference( transformOffset, objectInSpace, "Transform" ) );

						////		meshInSpace.Transform = ReferenceUtility.MakeReference<Transform>( null,
						////			ReferenceUtility.CalculateThisReference( meshInSpace, transformOffset, "Result" ) );
						////	}

						////	objectInSpace.Enabled = true;
						////}


						////!!!!не Clean update

						////for( int nMesh = 0; nMesh < scene.MeshCount; nMesh++ )
						////{
						////	global::Assimp.Mesh aiMesh = scene.Meshes[ nMesh ];
						////	if( aiMesh.PrimitiveType == PrimitiveType.Triangle )
						////	{
						////		var mesh = CreateMesh( importContext, aiMesh );

						////		importContext.sourcesMeshByIndex.Add( nMesh, mesh );

						////		importContext.sourcesMaterialByIndex.TryGetValue( aiMesh.MaterialIndex, out var material );
						////		if( material != null )
						////			importContext.materialNamePathByMeshNamePath[ mesh.GetNameWithIndexFromParent() ] = material.GetNameWithIndexFromParent();
						////	}
						////}

						////!!!!
						////	//!!!!?
						////	Mat4F transform90Fix = new Mat4F( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );
						////	//var rootTransform = scene.RootNode.Transform.ToMat4();
						////	var rootTransform = transform90Fix * scene.RootNode.Transform.ToMat4();

						////	InitMeshFromNodesRecursive( settings, scene, importContext, groupMeshes, scene.RootNode, rootTransform, 0 /*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );
					}




					//////!!!!
					//////create objects in space (Scene mode)
					////if( false /*settings.component.Mode.Value == Import3D.ModeEnum.Scene*/ &&
					////	( importContext.meshesGroup != null || scene.LightCount != 0 || scene.CameraCount != 0 ) )
					////{
					////	var groupName = "Scene Objects";

					////	//group
					////	importContext.objectsInSpaceGroup = settings.component.GetComponentByName( groupName ) as ObjectInSpace;
					////	if( importContext.objectsInSpaceGroup == null )
					////	{
					////		importContext.objectsInSpaceGroup = settings.component.CreateComponent<ObjectInSpace>( -1, false );
					////		importContext.objectsInSpaceGroup.Name = groupName;
					////	}
					////	else
					////		importContext.objectsInSpaceGroup.Enabled = false;

					////	//meshes in space
					////	if( importContext.meshesGroup != null )
					////	{
					////		foreach( var node in scene.RootNode.Children )
					////		{
					////			if( ContainsMeshesRecursive( node ) )
					////			{
					////				//mesh
					////				var mesh = importContext.meshesGroup.CreateComponent<Mesh>();
					////				mesh.Name = GetFixedName( node.Name );
					////				InitMeshGeometriesRecursive( importContext, node, Mat4F.Identity, mesh );

					////				//object in space

					////				var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<MeshInSpace>();
					////				objectInSpace.Name = GetFixedName( node.Name );

					////				var referenceValue = ReferenceUtils.CalculateRootReference( mesh );
					////				//var referenceValue = settings.virtualFileName + "|" + mesh.GetNamePathToAccessFromRoot();
					////				objectInSpace.Mesh = ReferenceUtils.CreateReference<Mesh>( null, referenceValue );

					////				//Transform
					////				( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );
					////				var transformOffset = objectInSpace.CreateComponent<TransformOffset>();
					////				transformOffset.Name = "Attach Transform Offset";
					////				transformOffset.PositionOffset = pos;
					////				transformOffset.RotationOffset = rot;
					////				transformOffset.ScaleOffset = scl;
					////				transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
					////					ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
					////				objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
					////					ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );

					////				//objectInSpace.Transform = new Transform( pos, rot, scl );
					////			}
					////		}

					////		//foreach( var obj in groupMeshes.Components )
					////		//{
					////		//	var mesh = obj as Mesh;
					////		//	if( mesh != null )
					////		//	{
					////		//		//!!!!по имени проверять? везде так
					////		//		var objectInSpace = groupObjectsInSpace.GetComponentByName( mesh.Name );
					////		//		if( objectInSpace == null )
					////		//		{
					////		//			var objectInSpace2 = groupObjectsInSpace.CreateComponent<MeshInSpace>();
					////		//			objectInSpace2.Name = mesh.Name;

					////		//			var referenceValue = settings.virtualFileName + "|" + mesh.GetNamePathToAccessFromRoot();
					////		//			objectInSpace2.Mesh = ReferenceUtils.CreateReference<Mesh>( null, referenceValue );

					////		//			objectInSpace2.Transform = mesh.TransformRelativeToParent;
					////		//		}
					////		//	}
					////		//}
					////	}

					////	//lights
					////	//!!!!
					////	if( false )
					////	{
					////		foreach( var light in scene.Lights )
					////		{
					////			var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<Light>();
					////			objectInSpace.Name = light.Name;

					////			//Transform
					////			var transformOffset = objectInSpace.CreateComponent<TransformOffset>();
					////			transformOffset.Name = "Attach Transform Offset";
					////			transformOffset.PositionOffset = light.Position.ToVec3();

					////			//!!!!
					////			//globalTransform*

					////			//!!!!temp
					////			transformOffset.PositionOffset = new Vec3( 0, 0, -1 );

					////			//!!!!globalTransform, globalTransformR
					////			transformOffset.RotationOffset = Quat.FromDirectionZAxisUp( light.Direction.ToVec3() );
					////			transformOffset.ScaleOffset = Vec3.One;
					////			transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
					////				ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
					////			objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
					////				ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );
					////			//objectInSpace.Transform = new Transform( light.Position.ToVec3(), Quat.FromDirectionZAxisUp( light.Direction.ToVec3() ), Vec3.One );

					////			//type
					////			switch( light.LightType )
					////			{
					////			case LightSourceType.Directional: objectInSpace.Type = Light.TypeEnum.Directional; break;
					////			case LightSourceType.Point: objectInSpace.Type = Light.TypeEnum.Point; break;
					////			case LightSourceType.Spot: objectInSpace.Type = Light.TypeEnum.Spotlight; break;
					////			default: objectInSpace.Type = Light.TypeEnum.Point; break;
					////			}

					////			//!!!!всё ниже не работает

					////			//power
					////			ColorValue color = new ColorValue( light.ColorDiffuse.R, light.ColorDiffuse.G, light.ColorDiffuse.B );
					////			objectInSpace.Power = new ColorValuePowered( color );
					////			//public Color3D ColorSpecular { get; set; }
					////			//public Color3D ColorAmbient { get; set; }

					////			//spot angles
					////			if( light.LightType == LightSourceType.Spot )
					////			{
					////				if( light.AngleInnerCone != 0 || light.AngleOuterCone != 0 )
					////				{
					////					objectInSpace.SpotlightInnerAngle = new Radian( light.AngleInnerCone ).InDegrees();
					////					objectInSpace.SpotlightOuterAngle = new Radian( light.AngleOuterCone ).InDegrees();
					////				}
					////			}

					////			//attenuation

					////			//!!!!
					////			//public float AttenuationConstant { get; set; }
					////			//public float AttenuationLinear { get; set; }
					////			//public float AttenuationQuadratic { get; set; }
					////			//objectInSpace.AttenuationNear
					////			//objectInSpace.AttenuationFar
					////			//objectInSpace.AttenuationPower
					////		}
					////	}

					////	//cameras
					////	//!!!!
					////	if( false )
					////	{
					////		foreach( var camera in scene.Cameras )
					////		{
					////			var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<Camera>();
					////			objectInSpace.Name = camera.Name;

					////			//Transform
					////			var transformOffset = objectInSpace.CreateComponent<TransformOffset>();
					////			transformOffset.Name = "Attach Transform Offset";
					////			transformOffset.PositionOffset = globalTransform * camera.Position.ToVec3();
					////			transformOffset.RotationOffset = ( globalTransformR * Mat3.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() ) ).ToQuat();
					////			//transformOffset.PositionOffset = camera.Position.ToVec3().ToVec3();
					////			//transformOffset.RotationOffset = Quat.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() );
					////			transformOffset.ScaleOffset = Vec3.One;
					////			transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
					////				ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
					////			objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
					////				ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );
					////			//objectInSpace.Transform = new Transform( camera.Position.ToVec3(), 
					////			//	Quat.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() ), Vec3.One );

					////			objectInSpace.NearClipPlane = camera.ClipPlaneNear;
					////			objectInSpace.FarClipPlane = camera.ClipPlaneFar;
					////			objectInSpace.AspectRatio = camera.AspectRatio;
					////			objectInSpace.FieldOfView = new Radian( camera.FieldOfview ).InDegrees();
					////			//public Matrix4x4 ViewMatrix { get; }
					////		}
					////	}
					////}

					//////enable groups
					//////if( importContext.meshesGroup != null )
					//////	importContext.meshesGroup.Enabled = true;
					//////if( importContext.objectInSpaceGroup != null )
					//////	importContext.objectInSpaceGroup.Enabled = true;
					////if( importContext.objectsInSpaceGroup != null )
					////	importContext.objectsInSpaceGroup.Enabled = true;




					//////create meshes
					////if( scene.HasMeshes && scene.MeshCount != 0 )
					////{
					////	var groupMeshes = settings.component.GetComponentByName( "Meshes" ) as Mesh;
					////	if( groupMeshes == null )
					////	{
					////		groupMeshes = settings.component.CreateComponent<Mesh>( -1, false );
					////		groupMeshes.Name = "Meshes";
					////	}
					////	else
					////		groupMeshes.Enabled = false;

					////	foreach( var item in sourcesMeshByIndex )
					////	{
					////		var original = item.Value;
					////		var type = original.GetProvidedType();
					////		if( type != null )
					////		{
					////			//!!!!тип проверять еще
					////			var obj = groupMeshes.GetComponentByName( original.Name );
					////			if( obj == null )
					////			{
					////				obj = groupMeshes.CreateComponent( type, -1, false );
					////				//var obj = group.CreateComponent( MetadataManager.MetadataGetType( original ), -1, false );
					////				obj.Name = original.Name;
					////				obj.Enabled = true;

					////				//material
					////				var mesh = obj as Mesh;
					////				if( mesh != null )
					////				{
					////					if( groupMaterials != null )
					////					{
					////						materialNamePathByMeshNamePath.TryGetValue( mesh.GetNameWithIndexFromParent(), out string materialNamePath );
					////						if( !string.IsNullOrEmpty( materialNamePath ) )
					////						{
					////							var material = groupMaterials.GetComponentByNamePath( materialNamePath ) as Material;
					////							if( material != null )
					////							{
					////								//не через "this:", впрочем неважно, т.к. полный путь есть при указании типа в "Sources".

					////								var referenceValue = settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
					////								mesh.Material = ReferenceUtils.CreateReference<Material>( null, referenceValue );

					////								//meshData.Material = ReferenceUtils.CreateReference<Material>( null,
					////								//	ReferenceUtils.CalculateThisReference( meshData, material ) );
					////								//meshData.Material = ResourceManager.LoadResource<Material>( "_Dev\\Sphere.material" );
					////							}
					////						}
					////					}
					////				}
					////			}
					////		}
					////	}

					////	groupMeshes.Enabled = true;
					////}


					//////create nodes
					////if( scene.RootNode != null )
					////{
					////	var groupNodes = settings.component.CreateComponent<ObjectInSpace>( -1, false );
					////	groupNodes.Name = "Nodes";

					////	EnumerateNodeRecursive( settings, scene, groupNodes, scene.RootNode, scene.RootNode.Transform.ToMat4(), sourcesMeshByIndex, 0
					////		/*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );

					////	//EnumerateNodeRecursiveNew( settings, scene, /*sceneSource, */scene.RootNode, scene.RootNode,
					////	//	scene.RootNode.Transform.ToMat4()/*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );

					////	//!!!!
					////	groupNodes.Enabled = true;
					////}


					stream?.Dispose();
				}
			}
			catch( Exception e )
			{
				//!!!!
				error = e.Message;
				//return null;
			}
		}



		//public override bool IsSupportedExportToFormat( string extension )
		//{
		//	return ExportFormats.Contains( extension );
		//}

		//public const bool UseCustomColladaWriter = false;

		//private bool ShowUseNewColladaWriterMessageBox()
		//{
		//	var msg = "There are two ways to export to the Collada (DAE). Use new method to export?";
		//	return MessageBox.Show( msg, "Assimp Exporter", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes;
		//}

		//public override bool Save( NeoAxis.Mesh_Old mesh, string realFileName )
		//{
		//	string extension = Path.GetExtension( realFileName.ToLower() );
		//	extension = extension.Replace( ".", "" );

		//	if( !IsSupportedExportToFormat( extension ) )
		//	{
		//		Log.Warning( "AssimpModelImporter: Export to \"{0}\" is not supported.", extension );
		//		return false;
		//	}

		//	if( extension == "dae" && !ShowUseNewColladaWriterMessageBox() )
		//	{
		//		ColladaModelWriter writer = new ColladaModelWriter();
		//		return writer.Save( mesh, realFileName );
		//	}
		//	else
		//	{
		//		Initialize();

		//		AssimpModelWriter writer = new AssimpModelWriter();

		//		if( extension == "dae" )
		//			writer.FlipYZUp = true;

		//		return writer.Save( mesh, realFileName );
		//	}
		//}

		//public override bool Save( ICollection<SaveGeometryItem> geometry, string realFileName )
		//{
		//	string extension = Path.GetExtension( realFileName.ToLower() );
		//	extension = extension.Replace( ".", "" );

		//	if( !IsSupportedExportToFormat( extension ) )
		//	{
		//		Log.Warning( "AssimpModelImporter: Export to \"{0}\" is not supported.", extension );
		//		return false;
		//	}

		//	if( extension == "dae" && !ShowUseNewColladaWriterMessageBox() )
		//	{
		//		ColladaModelWriter writer = new ColladaModelWriter();
		//		return writer.Save( geometry, realFileName );
		//	}
		//	else
		//	{
		//		Initialize();

		//		AssimpModelWriter writer = new AssimpModelWriter();

		//		if( extension == "dae" )
		//			writer.FlipYZUp = true;

		//		return writer.Save( geometry, realFileName );
		//	}
		//}

		/////////////////////////////////////////

		public static Vector2F ToVector2F( System.Numerics.Vector3 value )
		{
			return new Vector2F( value.X, value.Y );
		}

		public static Vector3 ToVector3( System.Numerics.Vector3 value )
		{
			return new Vector3( value.X, value.Y, value.Z );
		}

		public static Vector3F ToVector3F( System.Numerics.Vector3 value )
		{
			return new Vector3F( value.X, value.Y, value.Z );
		}

		public static Vector4F ToVector4F( System.Numerics.Vector4 value )
		{
			return new Vector4F( value.X, value.Y, value.Z, value.W );
		}

		public static Matrix4 ToMatrix4( System.Numerics.Matrix4x4 value )
		{
			return new Matrix4(
				value.M11, value.M21, value.M31, value.M41,
				value.M12, value.M22, value.M32, value.M42,
				value.M13, value.M23, value.M33, value.M43,
				value.M14, value.M24, value.M34, value.M44 );
		}

		public static Matrix4F ToMatrix4F( System.Numerics.Matrix4x4 value )
		{
			return new Matrix4F(
				value.M11, value.M21, value.M31, value.M41,
				value.M12, value.M22, value.M32, value.M42,
				value.M13, value.M23, value.M33, value.M43,
				value.M14, value.M24, value.M34, value.M44 );
		}

		public static Quaternion ToQuaternionNormalize( System.Numerics.Quaternion value )
		{
			var q = new Quaternion( value.X, value.Y, value.Z, value.W );
			return q.GetNormalize();
		}

		public static ColorValue ToColorValue( System.Numerics.Vector4 value )
		{
			return new ColorValue( value.X, value.Y, value.Z, value.W );
		}

		public static void Matrix4Round( ref Matrix4 matrix )
		{
			for( int y = 0; y < 4; y++ )
				for( int x = 0; x < 4; x++ )
					matrix[ x, y ] = Math.Round( matrix[ x, y ], 5 );
		}

		/////////////////////////////////////////
		// Materials data

		static List<MaterialData> GetMaterialsData( ImportContext importContext )
		{
			var result = new List<MaterialData>();

			var scene = importContext.scene;
			if( scene.HasMaterials && scene.MaterialCount != 0 )
			{
				var extractedTextures = new Dictionary<int, string>();

				var embedTexturesOutputVirtualDirectory = Path.Combine( Path.GetDirectoryName( importContext.settings.virtualFileName ), Path.GetFileName( importContext.settings.virtualFileName ) + "_files" );

				for( int nMaterial = 0; nMaterial < scene.MaterialCount; nMaterial++ )
				{
					var material = scene.Materials[ nMaterial ];

					var data = new MaterialData();
					data.Index = nMaterial;
					data.Name = GetFixedName( material.Name );

					try
					{
						//BlendMode
						{
							var property = material.GetProperty( "$mat.gltf.alphaMode", TextureType.None, 0 );
							if( property != null )
							{
								var alphaMode = property.GetStringValue() ?? "";
								if( alphaMode == "MASK" )
									data.BlendMode = Material.BlendModeEnum.Masked;
								else if( alphaMode == "BLEND" )
									data.BlendMode = Material.BlendModeEnum.Transparent;
							}
						}

						//OpacityMaskThreshold
						{
							var property = material.GetProperty( "$mat.gltf.alphaCutoff", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
									data.OpacityMaskThreshold = property.GetFloatValue();
								else if( property.PropertyType == PropertyType.Integer )
									data.OpacityMaskThreshold = property.GetIntegerValue();
							}
						}

						//TwoSided
						if( material.HasTwoSided && material.IsTwoSided )
							data.TwoSided = true;
						//TwoSided fix
						{
							var property = material.GetProperty( "$mat.twosided", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Buffer )
								{
									if( property.ByteCount == 1 && property.RawData[ 0 ] != 0 )
										data.TwoSided = true;
								}
							}
						}

						//BaseColor
						if( material.HasColorDiffuse )
						{
							var value = ToVector4F( material.ColorDiffuse );
							data.BaseColor = new ColorValue( value.X, value.Y, value.Z );

							//!!!!check
							if( value.W != 1 )
								data.Opacity *= value.W;
						}

						//Opacity
						if( material.HasOpacity && material.Opacity < 1 )
							data.Opacity *= material.Opacity;

						//!!!!check
						//Opacity from TransparencyFactor
						if( material.HasTransparencyFactor && material.TransparencyFactor > 0 )
							data.Opacity *= 1.0f - material.TransparencyFactor;


						//Metallic
						{
							var property = material.GetProperty( "$mat.metallicFactor", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
									data.Metallic = property.GetFloatValue();
								else if( property.PropertyType == PropertyType.Integer )
									data.Metallic = property.GetIntegerValue();
							}
						}

						//Roughness
						{
							var property = material.GetProperty( "$mat.roughnessFactor", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
									data.Roughness = property.GetFloatValue();
								else if( property.PropertyType == PropertyType.Integer )
									data.Roughness = property.GetIntegerValue();
							}
						}

						//Emissive color
						if( material.HasColorEmissive )
						{
							var value = ToVector4F( material.ColorEmissive ).ToVector3F();

							var intensityProperty = material.GetProperty( "$mat.emissiveIntensity", TextureType.None, 0 );
							if( intensityProperty != null )
							{
								if( intensityProperty.PropertyType == PropertyType.Float )
									value *= intensityProperty.GetFloatValue();
								else if( intensityProperty.PropertyType == PropertyType.Integer )
									value *= intensityProperty.GetIntegerValue();
							}

							if( value != Vector3F.Zero )
								data.EmissionColor = new ColorValue( value );
						}

						//Clearcoat
						{
							var property = material.GetProperty( "$mat.clearcoat.factor", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
									data.Clearcoat = property.GetFloatValue();
								else if( property.PropertyType == PropertyType.Integer )
									data.Clearcoat = property.GetIntegerValue();
							}
						}

						//ClearcoatRoughness
						{
							var property = material.GetProperty( "$mat.clearcoat.roughnessFactor", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
									data.ClearcoatRoughness = property.GetFloatValue();
								else if( property.PropertyType == PropertyType.Integer )
									data.ClearcoatRoughness = property.GetIntegerValue();
							}
						}

						//SheenColor
						{
							var property = material.GetProperty( "$clr.sheen.factor", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Float )
								{
									var array = property.GetFloatArrayValue();
									if( array != null && array.Length >= 3 )
										data.SheenColor = new ColorValue( array[ 0 ], array[ 1 ], array[ 2 ] );
								}
								else if( property.PropertyType == PropertyType.Integer )
								{
									var array = property.GetIntegerArrayValue();
									if( array != null && array.Length >= 3 )
										data.SheenColor = new ColorValue( array[ 0 ], array[ 1 ], array[ 2 ] );
								}
							}
						}


						//!!!!sheen.roughnessFactor


						//!!!!need modern implementation of Transmission

						////Transmission factor
						//float? transmissionFactor = null;
						//foreach( var p in material.GetAllProperties() )
						//{
						//	if( p.Name == "$mat.transmission.factor" )
						//	{
						//		if( p.PropertyType == PropertyType.Float )
						//			transmissionFactor = p.GetFloatValue();
						//		else if( p.PropertyType == PropertyType.Double )
						//			transmissionFactor = (float)p.GetDoubleValue();
						//	}
						//}
						//if( data.Opacity == 1 && transmissionFactor.HasValue && transmissionFactor.Value > 0 )
						//{
						//	data.Opacity = MathEx.Saturate( 1.0f - MathEx.Sqrt( transmissionFactor.Value ) );
						//}


						//unlit
						{
							var property = material.GetProperty( "$mat.gltf.unlit", TextureType.None, 0 );
							if( property != null )
							{
								if( property.PropertyType == PropertyType.Buffer )
								{
									if( property.ByteCount == 1 && property.RawData[ 0 ] != 0 )
										data.ShadingModel = Material.ShadingModelEnum.Unlit;
								}
							}
						}


						var textureTypes = new List<(TextureType, int)>();
						textureTypes.Add( (TextureType.Diffuse, 0) );
						textureTypes.Add( (TextureType.Normals, 0) );
						textureTypes.Add( (TextureType.Emissive, 0) );
						textureTypes.Add( (TextureType.Lightmap, 0) );
						textureTypes.Add( (TextureType.AmbientOcclusion, 0) );
						textureTypes.Add( (TextureType.Metalness, 0) );
						textureTypes.Add( (TextureType.Roughness, 0) );
						textureTypes.Add( (TextureType.Displacement, 0) );
						textureTypes.Add( (TextureType.Height, 0) );
						textureTypes.Add( (TextureType.Opacity, 0) );
						textureTypes.Add( (TextureType.BaseColor, 0) );
						textureTypes.Add( (TextureType.EmissionColor, 0) );
						textureTypes.Add( (TextureType.GltfMetallicRoughness, 0) );
						textureTypes.Add( (TextureType.MayaBase, 0) );
						textureTypes.Add( (TextureType.MayaSpecularRoughness, 0) );
						textureTypes.Add( (TextureType.Ambient, 0) );
						textureTypes.Add( (TextureType.Clearcoat, 0) );
						textureTypes.Add( (TextureType.Clearcoat, 1) );
						textureTypes.Add( (TextureType.Clearcoat, 2) );
						textureTypes.Add( (TextureType.Sheen, 0) );


						//!!!!

						///// <summary>
						///// Simulates transmission through the surface.
						///// May include further information such as wall thickness.
						///// </summary>
						//Transmission = 21,

						///// <summary>
						///// Simulates a surface with directional properties.
						///// </summary>
						//Anisotropy = 26,


						//!!!!SheenRoughness
						//add support to the engine
						//index == 1
						//#define AI_MATKEY_SHEEN_ROUGHNESS_TEXTURE aiTextureType_SHEEN, 1



						foreach( var textureTypeItem in textureTypes )
						{
							var textureType = textureTypeItem.Item1;
							var textureIndex = textureTypeItem.Item2;

							material.GetMaterialTexture( textureType, textureIndex, out var slot );
							if( !string.IsNullOrEmpty( slot.FilePath ) )
							{
								string fullPath = "";

								if( slot.FilePath.Length > 1 && slot.FilePath[ 0 ] == '*' )
								{
									var filePath = slot.FilePath.Substring( 1 );

									//replace %20
									filePath = System.Web.HttpUtility.UrlDecode( filePath );

									//embed
									if( int.TryParse( filePath, out var embedIndex ) )
									{
										if( embedIndex < scene.TextureCount )
										{
											if( !extractedTextures.TryGetValue( embedIndex, out var fullPath2 ) )
											{
												var embedTexture = scene.Textures[ embedIndex ];
												if( embedTexture.HasCompressedData )
												{
													//compressed data
													var ext = embedTexture.CompressedFormatHint;
													if( !string.IsNullOrEmpty( ext ) )
													{
														var fileName = embedTexture.Filename;
														if( string.IsNullOrEmpty( fileName ) )
															fileName = embedIndex.ToString();
														fullPath2 = Path.Combine( embedTexturesOutputVirtualDirectory, fileName + "." + ext );

														var realPath = VirtualPathUtility.GetRealPathByVirtual( fullPath2 );

														//write file
														{
															var realPathFolder = Path.GetDirectoryName( realPath );
															if( !Directory.Exists( realPathFolder ) )
																Directory.CreateDirectory( realPathFolder );
															File.WriteAllBytes( realPath, embedTexture.CompressedData );
														}

														extractedTextures[ embedIndex ] = fullPath2;
													}
												}
												else
												{
													//uncompressed data

													//impl?
													//Log.InvisibleInfo( "impl" );
												}
											}

											fullPath = fullPath2;
										}
									}
								}
								else
								{
									//usual reference to file

									var filePath = slot.FilePath;
									if( filePath.Length > 2 && filePath.Substring( 0, 2 ) == "./" )
										filePath = filePath.Substring( 2 );
									filePath = VirtualPathUtility.NormalizePath( filePath );

									//replace %20
									filePath = System.Web.HttpUtility.UrlDecode( filePath );

									var fullPath2 = Path.Combine( importContext.directoryName, filePath );
									if( VirtualFile.Exists( fullPath2 ) )
										fullPath = fullPath2;
								}

								if( !string.IsNullOrEmpty( fullPath ) )
								{
									var filePath = Path.GetFileName( fullPath );

									//parse uv transform
									TextureUVTransform uvTransform = null;
									{
										var p = material.GetAllProperties().FirstOrDefault( p => p.Name == "$tex.uvtrafo" && p.TextureType == textureType && p.TextureIndex == 0 );
										if( p != null && p.RawData != null && p.RawData.Length == 20 )
										{
											var offset = new Vector2F( BitConverter.ToSingle( p.RawData, 0 ), BitConverter.ToSingle( p.RawData, 4 ) );
											var scale = new Vector2F( BitConverter.ToSingle( p.RawData, 8 ), BitConverter.ToSingle( p.RawData, 12 ) );
											var rotation = BitConverter.ToSingle( p.RawData, 16 );

											if( !offset.Equals( Vector2F.Zero, 0.001f ) || !scale.Equals( Vector2F.One, 0.001f ) || Math.Abs( rotation ) > 0.001f )
											{
												uvTransform = new TextureUVTransform() { Translation = offset, Scale = scale, Rotation = rotation };
											}
										}
									}

									switch( textureType )
									{
									case TextureType.Diffuse:
									case TextureType.BaseColor:
									case TextureType.MayaBase:
										data.BaseColorTexture = fullPath;
										data.BaseColorTextureUVIndex = slot.UVIndex;
										data.BaseColorTextureUVTransform = uvTransform;
										break;

									case TextureType.Normals:
										data.NormalTexture = fullPath;
										data.NormalTextureUVIndex = slot.UVIndex;
										data.NormalTextureUVTransform = uvTransform;
										break;

									case TextureType.Emissive:
										data.EmissiveTexture = fullPath;
										data.EmissiveTextureUVIndex = slot.UVIndex;
										data.EmissiveTextureUVTransform = uvTransform;
										break;

									//case TextureType.EmissionColor:
									//	data.EmissiveTexture = fullPath;
									//	break;

									case TextureType.Metalness:
										data.MetallicTexture = fullPath;
										data.MetallicTextureUVIndex = slot.UVIndex;
										data.MetallicTextureUVTransform = uvTransform;
										break;

									case TextureType.Roughness:
									case TextureType.MayaSpecularRoughness:
										data.RoughnessTexture = fullPath;
										data.RoughnessTextureUVIndex = slot.UVIndex;
										data.RoughnessTextureUVTransform = uvTransform;
										break;

									case TextureType.Displacement:
									case TextureType.Height:
										data.DisplacementTexture = fullPath;
										data.DisplacementTextureUVIndex = slot.UVIndex;
										data.DisplacementTextureUVTransform = uvTransform;
										break;

									case TextureType.Opacity:
										data.OpacityTexture = fullPath;
										data.OpacityTextureUVIndex = slot.UVIndex;
										data.OpacityTextureUVTransform = uvTransform;
										break;

									case TextureType.Lightmap:
									case TextureType.AmbientOcclusion:
									case TextureType.Ambient:
										data.AmbientOcclusionTexture = fullPath;
										data.AmbientOcclusionTextureUVIndex = slot.UVIndex;
										data.AmbientOcclusionTextureUVTransform = uvTransform;
										break;

									case TextureType.GltfMetallicRoughness:
										data.RoughnessTexture = fullPath;
										data.RoughnessTextureChannel = "G";
										data.RoughnessTextureUVIndex = slot.UVIndex;
										data.RoughnessTextureUVTransform = uvTransform;
										data.MetallicTexture = fullPath;
										data.MetallicTextureChannel = "B";
										data.MetallicTextureUVIndex = slot.UVIndex;
										data.MetallicTextureUVTransform = uvTransform;
										break;

									case TextureType.Clearcoat:
										switch( textureIndex )
										{
										case 0:
											data.ClearcoatTexture = fullPath;
											data.ClearcoatTextureUVTransform = uvTransform;
											data.ClearcoatTextureUVIndex = slot.UVIndex;
											break;
										case 1:
											data.ClearcoatRoughnessTexture = fullPath;
											data.ClearcoatRoughnessTextureUVTransform = uvTransform;
											data.ClearcoatRoughnessTextureUVIndex = slot.UVIndex;
											break;
										case 2:
											data.ClearcoatNormalTexture = fullPath;
											data.ClearcoatNormalTextureUVTransform = uvTransform;
											data.ClearcoatNormalTextureUVIndex = slot.UVIndex;
											break;
										}
										break;

									case TextureType.Sheen:
										data.SheenColorTexture = fullPath;
										data.SheenColorTextureUVIndex = slot.UVIndex;
										data.SheenColorTextureUVTransform = uvTransform;
										break;
									}
								}
							}
						}

						//fix channels for merged metallic, roughness textures
						if( !string.IsNullOrEmpty( data.MetallicTexture ) && !string.IsNullOrEmpty( data.RoughnessTexture ) && data.MetallicTexture == data.RoughnessTexture && data.MetallicTextureChannel == "R" && data.RoughnessTextureChannel == "R" )
						{
							data.RoughnessTextureChannel = "G";
							data.MetallicTextureChannel = "B";
						}

						//add opacity texture from base color texture
						if( data.BlendMode == Material.BlendModeEnum.Masked || data.BlendMode == Material.BlendModeEnum.Transparent )
						{
							if( string.IsNullOrEmpty( data.OpacityTexture ) && !string.IsNullOrEmpty( data.BaseColorTexture ) )
							{
								data.OpacityTexture = data.BaseColorTexture;
								data.OpacityTextureChannel = "A";
								data.OpacityTextureUVIndex = data.BaseColorTextureUVIndex;
							}
						}

						//detect transparency by alpha channel of the texture. Pixels must contain alpha.
						if( string.IsNullOrEmpty( data.OpacityTexture ) && !string.IsNullOrEmpty( data.BaseColorTexture ) )
						{
							var containsAlpha = false;

							try
							{
								if( ImageUtility.LoadFromVirtualFile( data.BaseColorTexture, out var data2, out var size, out _, out var format, out _, out _, out var error ) )
								{
									var opacityTexture = new ImageUtility.Image2D( format, size, data2 );

									var allZero = true;

									for( int y = 0; y < size.Y; y++ )
									{
										for( int x = 0; x < size.X; x++ )
										{
											var c = opacityTexture.GetPixel( new Vector2I( x, y ) );

											if( c.W != 0 )
												allZero = false;
											if( c.W != 1 )
												containsAlpha = true;
										}
									}

									if( allZero )
										containsAlpha = false;
								}
							}
							catch( Exception e )
							{
								Log.Warning( "ImportAssimp: GetMaterialsData: Unable to read opacity data. " + e.Message );
							}

							if( containsAlpha )
							{
								data.OpacityTexture = data.BaseColorTexture;
								data.OpacityTextureChannel = "A";
								data.OpacityTextureUVIndex = data.BaseColorTextureUVIndex;

								data.BlendMode = Material.BlendModeEnum.Masked;
							}
						}

						//fix Blend mode
						if( data.Opacity < 1 && data.BlendMode == Material.BlendModeEnum.Opaque )
							data.BlendMode = Material.BlendModeEnum.Masked;

					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
					}

					result.Add( data );
				}
			}

			return result;
		}


		//skeleton and animations

		static void InitBoneRecursive( ImportContext importContext, Component parentComponent, Node boneNode )
		{
			var skeletonStructure = importContext.skeletonStructure;

			var boneComponent = parentComponent.CreateComponent<SkeletonBone>();
			boneComponent.Name = boneNode.Name;

			//calculate full transform from root to this bone
			var transform = GetNodeFullTransform( importContext, boneNode );
			transform.Decompose( out var translation, out Quaternion rotation, out var scale );

			//round
			translation.X = Math.Round( translation.X, 5 );
			translation.Y = Math.Round( translation.Y, 5 );
			translation.Z = Math.Round( translation.Z, 5 );
			rotation.X = Math.Round( rotation.X, 5 );
			rotation.Y = Math.Round( rotation.Y, 5 );
			rotation.Z = Math.Round( rotation.Z, 5 );
			rotation.W = Math.Round( rotation.W, 5 );
			scale.X = Math.Round( scale.X, 5 );
			scale.Y = Math.Round( scale.Y, 5 );
			scale.Z = Math.Round( scale.Z, 5 );

			boneComponent.Transform = new Transform( translation, rotation, scale );

			skeletonStructure.nodeBySkeletonBone[ boneComponent ] = boneNode;

			foreach( var childBone in boneNode.Children )
			{
				if( skeletonStructure.boneIndexByName.ContainsKey( childBone.Name ) )
					InitBoneRecursive( importContext, boneComponent, childBone );
			}
		}

		class MeshesSkeletonStructure
		{
			Internal.Assimp.Scene scene;

			public Dictionary<Node, bool> potentiallyMeshBones = new Dictionary<Node, bool>();
			public Dictionary<string, Matrix4> boneOffsetMatrixByName = new Dictionary<string, Matrix4>();

			//public ESet<string> nodesWithAnimationData = new ESet<string>(); //to detect rootSkeletonNode
			public Node rootSkeletonNode;
			public EDictionary<string, int> boneIndexByName = new EDictionary<string, int>();
			public EDictionary<string, Node> boneNodeByName = new EDictionary<string, Node>();
			public EDictionary<SkeletonBone, Node> nodeBySkeletonBone = new EDictionary<SkeletonBone, Node>();

			//

			public MeshesSkeletonStructure( Internal.Assimp.Scene scene )
			{
				this.scene = scene;

				CalculatePotentiallyMeshBones();
				//CalculateNodeWithAnimationData();
				CalculateRootSkeletonNode();
				CalculateAllBoneNamesRecursive( rootSkeletonNode );
			}

			Node FindNodeByName( Node node, string name )
			{
				if( node.Name == name )
					return node;
				return node.Children.Select( c => FindNodeByName( c, name ) ).FirstOrDefault( n => n != null );
			}

			void CalculatePotentiallyMeshBones()
			{
				foreach( var mesh in scene.Meshes )
				{
					foreach( var bone in mesh.Bones )
					{
						var node = FindNodeByName( scene.RootNode, bone.Name );
						if( node != null )
						{
							potentiallyMeshBones[ node ] = true;

							while( node != null )
							{
								potentiallyMeshBones[ node ] = true;
								node = node.Parent;
							}
						}

						//get bone offset matrix
						var offsetMatrix = ToMatrix4( bone.OffsetMatrix );
						if( boneOffsetMatrixByName.TryGetValue( bone.Name, out var currentMatrix ) )
						{
							//check that offset matrix is the same for all meshes
							if( offsetMatrix != currentMatrix )
								Log.Warning( $"Bone \"{bone.Name}\" has different offset matrices in different meshes." );
						}
						boneOffsetMatrixByName[ bone.Name ] = offsetMatrix;
					}
				}
			}

			//void CalculateNodeWithAnimationData()
			//{
			//	for( int nAnimation = 0; nAnimation < scene.AnimationCount; nAnimation++ )
			//	{
			//		var animation = scene.Animations[ nAnimation ];

			//		foreach( var channel in animation.NodeAnimationChannels )
			//			nodesWithAnimationData.AddWithCheckAlreadyContained( channel.NodeName );
			//	}
			//}

			void CalculateRootSkeletonNodeRecursive( Node node )
			{
				var childrenPotentiallyMeshBones = 0;
				foreach( var childNode in node.Children )
				{
					if( potentiallyMeshBones.ContainsKey( childNode ) )
						childrenPotentiallyMeshBones++;
				}

				//more than 1 child nodes with potentially mesh bones or contains animation tracks, so this node is root skeleton node
				if( childrenPotentiallyMeshBones > 1 ) //|| nodesWithAnimationData.Contains( node.Name ) )
				{
					//found root skeleton node, exit
					rootSkeletonNode = node;
					return;
				}

				//enumerate child nodes
				foreach( var childNode2 in node.Children )
				{
					CalculateRootSkeletonNodeRecursive( childNode2 );

					//already found root skeleton node, exit
					if( rootSkeletonNode != null )
						return;
				}
			}

			void CalculateRootSkeletonNode()
			{
				//!!!!
				//not work
				////unable from Assimp to detect root skeleton node, so just find first node with more than 1 child nodes with potentially mesh bones
				//CalculateRootSkeletonNodeRecursive( scene.RootNode );

				//or use root node if not found
				if( rootSkeletonNode == null )
					rootSkeletonNode = scene.RootNode;
			}

			void CalculateAllBoneNamesRecursive( Node node )
			{
				boneIndexByName.Add( node.Name, boneIndexByName.Count );
				boneNodeByName.Add( node.Name, node );

				foreach( var childNode in node.Children )
				{
					if( potentiallyMeshBones.ContainsKey( childNode ) )
						CalculateAllBoneNamesRecursive( childNode );
				}
			}
		}

		////Node rootNode, out int[] newIndexFromOldIndex, out SkeletonBone[] oldBoneFromNewIndex //, Matrix4 additionalTransform/*, out EDictionary<SkeletonBone, Node> addedBones*/ ) //, Dictionary<SkeletonBone, Matrix4> boneTransformsToNormalize )	
		////newIndexFromOld - an array mapping from old bone indices to a new : ret[oldIndex]==newIndex

		static Skeleton CreateSkeletonComponent( ImportContext importContext, Internal.Assimp.Scene scene )

		{
			//newIndexFromOldIndex = null;
			//oldBoneFromNewIndex = null;

			var skeletonStructure = new MeshesSkeletonStructure( scene );

			//no bones
			if( skeletonStructure.potentiallyMeshBones.Count == 0 )
				return null;

			importContext.skeletonStructure = skeletonStructure;

			//create skeleton component
			var skeletonComponent = new Skeleton();
			skeletonComponent.Name = "Skeleton";

			//create bone components
			InitBoneRecursive( importContext, skeletonComponent, skeletonStructure.rootSkeletonNode );


			////var oldBones = new Dictionary<NeoAxis.SkeletonBone, SkeletonBone>();
			////foreach( var firstLevelBone in skeleton.RootBone.Children )
			////	InitBoneRecursive( importContext, skeletonComponent, firstLevelBone, skeleton, oldBones, additionalTransform, boneTransformsToNormalize );

			////var allBones = skeletonComponent.GetBones(); //contains information about new bone indices
			////int maxOldIndex = oldBones.Count == 0 ? -1 : oldBones.Values.Select( _ => skeleton.GetBoneIndexByNode( _.Node ) ).Max();
			////newIndexFromOldIndex = new int[ maxOldIndex + 1 ];
			////for( int i = 0; i < newIndexFromOldIndex.Length; i++ )
			////	newIndexFromOldIndex[ i ] = -1;
			////for( int newIndex = 0; newIndex < allBones.Length; newIndex++ )
			////{
			////	var bone = oldBones[ allBones[ newIndex ] ];
			////	newIndexFromOldIndex[ skeleton.GetBoneIndexByNode( bone.Node ) ] = newIndex;
			////}

			////oldBoneFromNewIndex = new SkeletonBone[ allBones.Length ];
			////for( int boneIndex = 0; boneIndex < oldBoneFromNewIndex.Length; boneIndex++ )
			////	oldBoneFromNewIndex[ boneIndex ] = oldBones[ allBones[ boneIndex ] ];


			return skeletonComponent;
		}

		class KeyCalculator
		{
			public NodeAnimationChannel channel;
			public double timeFactor;

			//

			double RepeatTime( double time, double firstTime, double lastTime )
			{
				var length = lastTime - firstTime;
				if( length <= 0 )
					return firstTime;

				time = firstTime + ( time - firstTime ) % length;
				if( time < firstTime )
					time += length;
				return time;
			}

			double GetPositionTime( int index ) => channel.PositionKeys[ index ].Time * timeFactor;
			double GetRotationTime( int index ) => channel.RotationKeys[ index ].Time * timeFactor;
			double GetScalingTime( int index ) => channel.ScalingKeys[ index ].Time * timeFactor;

			public Vector3 EvaluatePosition( double time )
			{
				//if( channel.PositionKeyCount == 0 )
				//	return null;
				if( channel.PositionKeyCount == 1 )
					return ToVector3( channel.PositionKeys[ 0 ].Value );

				var firstTime = GetPositionTime( 0 );
				var lastTime = GetPositionTime( channel.PositionKeyCount - 1 );

				if( time < firstTime )
				{
					switch( channel.PreState )
					{
					case AnimationBehaviour.Constant:
						return ToVector3( channel.PositionKeys[ 0 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default: //case AnimationBehaviour.Linear:
						{
							var t0 = GetPositionTime( 0 );
							var t1 = GetPositionTime( 1 );
							var v0 = ToVector3( channel.PositionKeys[ 0 ].Value );
							var v1 = ToVector3( channel.PositionKeys[ 1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Vector3.Lerp( v0, v1, factor );
						}
					}
				}
				else if( time > lastTime )
				{
					switch( channel.PostState )
					{
					case AnimationBehaviour.Constant:
						return ToVector3( channel.PositionKeys[ channel.PositionKeyCount - 1 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default:// case AnimationBehaviour.Linear:
						{
							var i0 = channel.PositionKeyCount - 2;
							var i1 = channel.PositionKeyCount - 1;
							var t0 = GetPositionTime( i0 );
							var t1 = GetPositionTime( i1 );
							var v0 = ToVector3( channel.PositionKeys[ i0 ].Value );
							var v1 = ToVector3( channel.PositionKeys[ i1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Vector3.Lerp( v0, v1, factor );
						}
					}
				}

				for( int n = 0; n < channel.PositionKeyCount - 1; n++ )
				{
					var t0 = GetPositionTime( n );
					var t1 = GetPositionTime( n + 1 );

					if( time <= t1 || n == channel.PositionKeyCount - 2 )
					{
						var v0 = ToVector3( channel.PositionKeys[ n ].Value );
						var v1 = ToVector3( channel.PositionKeys[ n + 1 ].Value );
						var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
						return Vector3.Lerp( v0, v1, factor );
					}
				}

				return ToVector3( channel.PositionKeys[ channel.PositionKeyCount - 1 ].Value );
			}

			public Quaternion EvaluateRotation( double time )
			{
				//if( channel.RotationKeyCount == 0 )
				//	return defaultRotation;
				if( channel.RotationKeyCount == 1 )
					return ToQuaternionNormalize( channel.RotationKeys[ 0 ].Value );

				var firstTime = GetRotationTime( 0 );
				var lastTime = GetRotationTime( channel.RotationKeyCount - 1 );

				if( time < firstTime )
				{
					switch( channel.PreState )
					{
					case AnimationBehaviour.Constant:
						return ToQuaternionNormalize( channel.RotationKeys[ 0 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default://case AnimationBehaviour.Linear:
						{
							var t0 = GetRotationTime( 0 );
							var t1 = GetRotationTime( 1 );
							var v0 = ToQuaternionNormalize( channel.RotationKeys[ 0 ].Value );
							var v1 = ToQuaternionNormalize( channel.RotationKeys[ 1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Quaternion.Slerp( v0, v1, factor );
						}
					}
				}
				else if( time > lastTime )
				{
					switch( channel.PostState )
					{
					case AnimationBehaviour.Constant:
						return ToQuaternionNormalize( channel.RotationKeys[ channel.RotationKeyCount - 1 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default://case AnimationBehaviour.Linear:
						{
							var i0 = channel.RotationKeyCount - 2;
							var i1 = channel.RotationKeyCount - 1;
							var t0 = GetRotationTime( i0 );
							var t1 = GetRotationTime( i1 );
							var v0 = ToQuaternionNormalize( channel.RotationKeys[ i0 ].Value );
							var v1 = ToQuaternionNormalize( channel.RotationKeys[ i1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Quaternion.Slerp( v0, v1, factor );
						}
					}
				}

				for( int n = 0; n < channel.RotationKeyCount - 1; n++ )
				{
					var t0 = GetRotationTime( n );
					var t1 = GetRotationTime( n + 1 );

					if( time <= t1 || n == channel.RotationKeyCount - 2 )
					{
						var v0 = ToQuaternionNormalize( channel.RotationKeys[ n ].Value );
						var v1 = ToQuaternionNormalize( channel.RotationKeys[ n + 1 ].Value );
						var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
						return Quaternion.Slerp( v0, v1, factor );
					}
				}

				return ToQuaternionNormalize( channel.RotationKeys[ channel.RotationKeyCount - 1 ].Value );
			}

			public Vector3 EvaluateScale( double time )
			{
				//if( channel.ScalingKeyCount == 0 )
				//	return defaultScale;
				if( channel.ScalingKeyCount == 1 )
					return ToVector3( channel.ScalingKeys[ 0 ].Value );

				var firstTime = GetScalingTime( 0 );
				var lastTime = GetScalingTime( channel.ScalingKeyCount - 1 );

				if( time < firstTime )
				{
					switch( channel.PreState )
					{
					case AnimationBehaviour.Constant:
						return ToVector3( channel.ScalingKeys[ 0 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default://case AnimationBehaviour.Linear:
						{
							var t0 = GetScalingTime( 0 );
							var t1 = GetScalingTime( 1 );
							var v0 = ToVector3( channel.ScalingKeys[ 0 ].Value );
							var v1 = ToVector3( channel.ScalingKeys[ 1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Vector3.Lerp( v0, v1, factor );
						}
					}
				}
				else if( time > lastTime )
				{
					switch( channel.PostState )
					{
					case AnimationBehaviour.Constant:
						return ToVector3( channel.ScalingKeys[ channel.ScalingKeyCount - 1 ].Value );
					case AnimationBehaviour.Repeat:
						time = RepeatTime( time, firstTime, lastTime );
						break;
					default://case AnimationBehaviour.Linear:
						{
							var i0 = channel.ScalingKeyCount - 2;
							var i1 = channel.ScalingKeyCount - 1;
							var t0 = GetScalingTime( i0 );
							var t1 = GetScalingTime( i1 );
							var v0 = ToVector3( channel.ScalingKeys[ i0 ].Value );
							var v1 = ToVector3( channel.ScalingKeys[ i1 ].Value );
							var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
							return Vector3.Lerp( v0, v1, factor );
						}
					}
				}

				for( int n = 0; n < channel.ScalingKeyCount - 1; n++ )
				{
					var t0 = GetScalingTime( n );
					var t1 = GetScalingTime( n + 1 );

					if( time <= t1 || n == channel.ScalingKeyCount - 2 )
					{
						var v0 = ToVector3( channel.ScalingKeys[ n ].Value );
						var v1 = ToVector3( channel.ScalingKeys[ n + 1 ].Value );
						var factor = t1 != t0 ? ( time - t0 ) / ( t1 - t0 ) : 0.0;
						return Vector3.Lerp( v0, v1, factor );
					}
				}

				return ToVector3( channel.ScalingKeys[ channel.ScalingKeyCount - 1 ].Value );
			}
		}

		static void InitAnimations( ImportContext importContext, Internal.Assimp.Scene scene, Mesh meshComponent )
		{
			var skeletonStructure = importContext.skeletonStructure;
			//var skeletonBonesArray = skeletonStructure.nodeBySkeletonBone.Keys.ToArray();

			//create animations group component
			var animationsComponent = meshComponent.CreateComponent<Component>();
			animationsComponent.Name = "Animations";

			//enumerate all animations
			for( int nAnimation = 0; nAnimation < scene.AnimationCount; nAnimation++ )
			{
				var animation = scene.Animations[ nAnimation ];

				//get time factor
				if( animation.TicksPerSecond == 0 )
					continue;
				double timeFactor = 1.0 / animation.TicksPerSecond;

				//create skeleton animation component
				var skeletonAnimationComponent = animationsComponent.CreateComponent<SkeletonAnimation>();
				skeletonAnimationComponent.Name = string.IsNullOrEmpty( animation.Name ) ? $"Animation {nAnimation + 1}" : animation.Name;

				//create track as a child of animation component
				var skeletonAnimationTrackComponent = skeletonAnimationComponent.CreateComponent<SkeletonAnimationTrack>();

				//set animation name
				var name = skeletonAnimationComponent.Name;
				var prefix = "Track ";
				if( name.Length < prefix.Length || name.Substring( 0, prefix.Length ) != prefix )
					name = ( prefix + name ).Trim();
				skeletonAnimationTrackComponent.Name = name;


				//fill track data

				var trackData = new List<SkeletonAnimationTrack.KeyFrame>();


				////show info about animation and channels for testing
				////Log.Info( "---------------- ANIMATION: " + skeletonAnimationComponent.Name );
				////Log.Info( "Channels: " + animation.NodeAnimationChannelCount.ToString() + ", Bones: " + skeletonStructure.boneIndexByName.Count.ToString() );
				////foreach( var channel in animation.NodeAnimationChannels )
				////{
				////	if( !skeletonStructure.boneIndexByName.TryGetValue( channel.NodeName, out var boneIndex ) )
				////		continue;
				////	Log.Info( "Channel: " + channel.NodeName + ", Position keys: " + channel.PositionKeys.Count.ToString() + ", Rotation keys: " + channel.RotationKeys.Count.ToString() + ", Scaling keys: " + channel.ScalingKeys.Count.ToString() );
				////}




				//!!!!something wrong in the code of getting track data

				//useful info:
				//NeoAxis skeleton bone components contains global transform, not relative.
				//NeoAxis track data contains relative transforms, not global.
				//ImportFBX.cs works right (import via FBX SDK), so the problem is in the code of getting track data from Assimp.
				//To disable any transforms, disable FixAxes in the Import3D settings. Then global transform will be identity.

				//Import skeleton: OK.
				//Bind pose: OK.
				//List of animations: OK.
				//Animation tracks: Invalid transforms.

				//how test models and snow hierarchy:
				//https://gltf-viewer.donmccurdy.com/
				//https://sandbox.babylonjs.com/

				//useful samples:
				//glTF-Sample-Assets library on guthub. SimpleSkin sample
				//sketchfab models as example: https://sketchfab.com/3d-models/blue-flower-animated-c20b1f12833148e09f7f49c3dd444906



				//minor nuance, but ok: in Assimp can't detect root skeleton node, so just use root node of the scene as root skeleton node
				//CalculateRootSkeletonNode() method





				{
					//get sorted times from all channels of this animation
					double[] availableTimes;
					{
						var availableTimesSet = new ESet<double>();
						foreach( var channel in animation.NodeAnimationChannels )
						{
							foreach( var key in channel.PositionKeys )
								availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );
							foreach( var key in channel.RotationKeys )
								availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );
							foreach( var key in channel.ScalingKeys )
								availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );
						}
						availableTimes = availableTimesSet.ToArray();
						CollectionUtility.MergeSort( availableTimes, delegate ( double v1, double v2 )
						{
							return ( v1 < v2 ) ? -1 : 1;
						} );
					}


					NodeAnimationChannel GetChannel( string nodeName )
					{
						foreach( var channel in animation.NodeAnimationChannels )
						{
							if( channel.NodeName == nodeName )
								return channel;
						}
						return null;
					}

					for( int nTime = 0; nTime < availableTimes.Length; nTime++ )
					{
						var time = availableTimes[ nTime ];


						var boneGlobalTransforms = new Dictionary<string, Matrix4>();

						void InitAnimationRecursive( Node node, Matrix4 parentTransform )
						{
							var nodeName = node.Name;
							var nodeTransform = ToMatrix4( node.Transform );

							var boneChannel = GetChannel( nodeName );
							if( boneChannel != null )
							{
								var keyCalculator = new KeyCalculator { channel = boneChannel, timeFactor = timeFactor };

								var position = keyCalculator.EvaluatePosition( time );
								var rotation = keyCalculator.EvaluateRotation( time );
								var scale = keyCalculator.EvaluateScale( time );

								//this order gives right result on SimpleSkin
								var localTransform = Matrix4.FromTranslate( position ) * rotation.ToMatrix3().ToMatrix4() * Matrix3.FromScale( scale ).ToMatrix4();
								//var localTransform = Matrix3.FromScale( scale ).ToMatrix4() * rotation.ToMatrix3().ToMatrix4() * Matrix4.FromTranslate( position );

								//this seems correct. because if disabled, the glitch remains
								nodeTransform = localTransform;
							}

							var globalTransform = parentTransform * nodeTransform;

							boneGlobalTransforms[ nodeName ] = globalTransform;

							foreach( var child in node.Children )
								InitAnimationRecursive( child, globalTransform );
						}

						InitAnimationRecursive( scene.RootNode, Matrix4.Identity );


						var bonesWithData = new bool[ skeletonStructure.boneIndexByName.Count ];

						foreach( var boneName in boneGlobalTransforms.Keys )
						{
							if( !skeletonStructure.boneIndexByName.TryGetValue( boneName, out var boneIndex ) )
								continue;

							bonesWithData[ boneIndex ] = true;

							var boneGlobalTransform = boneGlobalTransforms[ boneName ];

							Matrix4 result;

							skeletonStructure.boneNodeByName.TryGetValue( boneName, out var boneNode );
							var parentNode = boneNode.Parent;
							if( parentNode != null )
							{
								var parentGlobalTransform = boneGlobalTransforms[ parentNode.Name ];

								result = parentGlobalTransform.GetInverse() * boneGlobalTransform;
								//result = boneGlobalTransform * parentGlobalTransform.GetInverse();
							}
							else
							{
								//!!!!right?
								result = importContext.globalTransform * boneGlobalTransform;
								//result = importContext.globalTransform * ToMatrix4( boneNode.Transform );
							}

							if( !result.Decompose( out var finalPosition, out Quaternion finalRotation, out var finalScale ) )
								continue;

							trackData.Add( new SkeletonAnimationTrack.KeyFrame
							{
								Time = (float)time,
								BoneIndex = boneIndex,
								Position = finalPosition.ToVector3F(),
								Rotation = finalRotation.ToQuaternionF(),
								Scale = finalScale.ToVector3F()
							} );
						}

						//verify input data
						for( int nBone = 0; nBone < bonesWithData.Length; nBone++ )
						{
							if( !bonesWithData[ nBone ] )
							{
								Log.Warning( "Bone \"" + skeletonStructure.boneNodeByName[ skeletonStructure.boneIndexByName.First( p => p.Value == nBone ).Key ].Name + "\" has no animation data in animation \"" + skeletonAnimationComponent.Name + "\"." );
							}
						}

					}
				}



				//test
				//{
				//	trackData = new List<SkeletonAnimationTrack.KeyFrame>();

				//	for( int n = 0; n < skeletonStructure.boneIndexByName.Count; n++ )
				//	{
				//		var boneNode = skeletonStructure.boneNodeByName.Values.FirstOrDefault( n2 => skeletonStructure.boneIndexByName[ n2.Name ] == n );

				//		var tr = ToMatrix4( boneNode.Transform );

				//		//!!!!
				//		tr = Matrix4.Identity;

				//		//!!!!
				//		if( skeletonStructure.boneOffsetMatrixByName.TryGetValue( boneNode.Name, out var offsetMatrix ) )
				//		{
				//			tr = offsetMatrix;//.GetInverse();
				//			//tr = offsetMatrix.GetInverse() * tr;

				//			//tr *= offsetMatrix;
				//		}

				//		tr.Decompose( out var p, out Quaternion r, out var s );

				//		//var p = new Vector3( 0, 0, 0 );
				//		//var r = Quaternion.Identity;
				//		//var s = new Vector3( 1, 1, 1 );

				//		trackData.Add( new SkeletonAnimationTrack.KeyFrame
				//		{
				//			Time = (float)0,
				//			BoneIndex = n,
				//			Position = p.ToVector3F(),
				//			Rotation = r.ToQuaternionF(),
				//			Scale = s.ToVector3F()
				//		} );
				//	}
				//}



				////not working code
				//if( false )
				//{
				//	var bonesWithData = new bool[ skeletonStructure.boneIndexByName.Count ];

				//	//enumerate channels
				//	foreach( var channel in animation.NodeAnimationChannels )
				//	{
				//		//get bone index
				//		if( !skeletonStructure.boneIndexByName.TryGetValue( channel.NodeName, out var boneIndex ) )
				//			continue;

				//		//mark that this bone has animation data to fill later not initialized bones with default key frames
				//		bonesWithData[ boneIndex ] = true;

				//		if( channel.PositionKeys.Count == 0 || channel.RotationKeys.Count == 0 || channel.ScalingKeys.Count == 0 )
				//		{
				//			//!!!!possible?

				//			Log.Warning( "Not all key types (position, rotation, scale) are present in the animation channel. This may lead to incorrect animation. Channel: " + channel.NodeName );
				//			continue;
				//		}

				//		//get sorted times
				//		double[] availableTimes;
				//		{
				//			var availableTimesSet = new ESet<double>();
				//			foreach( var key in channel.PositionKeys )
				//				availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );
				//			foreach( var key in channel.RotationKeys )
				//				availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );
				//			foreach( var key in channel.ScalingKeys )
				//				availableTimesSet.AddWithCheckAlreadyContained( key.Time * timeFactor );

				//			availableTimes = availableTimesSet.ToArray();

				//			CollectionUtility.MergeSort( availableTimes, delegate ( double v1, double v2 )
				//			{
				//				return ( v1 < v2 ) ? -1 : 1;
				//			} );
				//		}

				//		var keyCalculator = new KeyCalculator { channel = channel, timeFactor = timeFactor };

				//		//add key frames to track data
				//		for( int nTime = 0; nTime < availableTimes.Length; nTime++ )
				//		{
				//			var time = availableTimes[ nTime ];

				//			var position = keyCalculator.EvaluatePosition( time );
				//			var rotation = keyCalculator.EvaluateRotation( time );
				//			var scale = keyCalculator.EvaluateScale( time );

				//			//make matrix from SRT
				//			var localTransform = Matrix3.FromScale( scale ).ToMatrix4() * rotation.ToMatrix3().ToMatrix4() * Matrix4.FromTranslate( position );
				//			//var localTransform = Matrix4.FromTranslate( position ) * rotation.ToMatrix3().ToMatrix4() * Matrix3.FromScale( scale ).ToMatrix4();

				//			//decompose matrix to get final transform
				//			if( !localTransform.Decompose( out var finalPosition, out Quaternion finalRotation, out var finalScale ) )
				//				continue;

				//			trackData.Add( new SkeletonAnimationTrack.KeyFrame
				//			{
				//				Time = (float)time,
				//				BoneIndex = boneIndex,
				//				Position = finalPosition.ToVector3F(),
				//				Rotation = finalRotation.ToQuaternionF(),
				//				Scale = finalScale.ToVector3F()
				//			} );
				//		}
				//	}

				//	//add default key frames for bones without animation data
				//	for( int nBone = 0; nBone < bonesWithData.Length; nBone++ )
				//	{
				//		if( !bonesWithData[ nBone ] )
				//		{
				//			var boneNode = skeletonStructure.boneNodeByName.Values.FirstOrDefault( n => skeletonStructure.boneIndexByName[ n.Name ] == nBone );

				//			if( boneNode == null )
				//			{
				//				//!!!!possible?

				//				Log.Warning( "Bone node not found for bone index: " + nBone.ToString() );
				//				continue;
				//			}

				//			var transform = ToMatrix4( boneNode.Transform );
				//			transform.Decompose( out var translation, out Quaternion rotation, out var scale );

				//			//!!!!right?
				//			rotation.Normalize();

				//			trackData.Add( new SkeletonAnimationTrack.KeyFrame
				//			{
				//				Time = 0,
				//				BoneIndex = nBone,
				//				Position = translation.ToVector3F(),
				//				Rotation = rotation.ToQuaternionF(),
				//				Scale = scale.ToVector3F()
				//			} );
				//		}
				//	}
				//}


				//find min,max time and fill float.NaN time (for non animated bones with single keyframe) with minTime.
				float minTime = float.PositiveInfinity;
				float maxTime = float.NegativeInfinity;
				bool empty = true;
				for( int i = 0; i < trackData.Count; i++ )
				{
					float t = trackData[ i ].Time;
					if( float.IsNaN( t ) ) //no animation - static key frame
						continue;
					empty = false;
					if( t < minTime )
						minTime = t;
					if( maxTime < t )
						maxTime = t;
				}
				if( empty )
				{
					maxTime = 0;
					minTime = 0;
				}
				for( int i = 0; i < trackData.Count; i++ )
				{
					if( float.IsNaN( trackData[ i ].Time ) )
					{
						var e = trackData[ i ];
						e.Time = minTime;
						trackData[ i ] = e;
					}
				}


				//var length = (float)animation.DurationInTicks / (float)animation.TicksPerSecond;
				//Log.Info( $"Animation '{skeletonAnimationComponent.Name}' track '{skeletonAnimationTrackComponent.Name}' has {trackData.Count} key frames, time range: {minTime} - {maxTime}. length: {length}" );


				//sort. by bone index first, then by time
				CollectionUtility.MergeSort( trackData, ( a, b ) =>
				{
					var c = a.BoneIndex.CompareTo( b.BoneIndex );
					if( c != 0 )
						return c;
					return a.Time.CompareTo( b.Time );
				} );



				//////apply parent transforms to the key frames to make them relative to the skeleton root
				////for( int nBone = 0; nBone < skeletonStructure.boneIndexByName.Count; nBone++ )
				////{
				////	var bone = skeletonBonesArray[ nBone ];
				////	var parentBone = bone.Parent as SkeletonBone;
				////	var parentBoneIndex = parentBone != null ? Array.IndexOf( skeletonBonesArray, parentBone ) : -1;

				////	if( parentBone != null )
				////	{
				////		var parentFrames = trackData.Where( e => e.BoneIndex == parentBoneIndex ).ToArray();

				////		for( int nTrack = 0; nTrack < trackData.Count; nTrack++ )
				////		{
				////			var keyFrame = trackData[ nTrack ];
				////			if( keyFrame.BoneIndex == nBone )
				////			{
				////				var time = keyFrame.Time;

				////				//get interpolated parent frame for the same time. if not exists, interpolate between nearest frames

				////				for( int n = 0; n < parentFrames.Length - 1; n++ )
				////				{
				////					var t0 = parentFrames[ n ];
				////					var t1 = parentFrames[ n + 1 ];

				////					if( time <= t1.Time || n == parentFrames.Length - 2 )
				////					{
				////						var factor = t1.Time != t0.Time ? ( time - t0.Time ) / ( t1.Time - t0.Time ) : 0.0f;
				////						MathEx.Saturate( ref factor );
				////						var position = Vector3.Lerp( t0.Position, t1.Position, factor );
				////						var rotation = Quaternion.Slerp( t0.Rotation, t1.Rotation, factor );
				////						var scale = Vector3.Lerp( t0.Scale, t1.Scale, factor );

				////						var parentTransform = new Transform( position, rotation, scale ).ToMatrix4();
				////						var transform = new Transform( keyFrame.Position, keyFrame.Rotation, keyFrame.Scale ).ToMatrix4();

				////						var resultTransform = parentTransform * transform;
				////						resultTransform.Decompose( out Vector3 t, out Quaternion r, out Vector3 s );

				////						trackData[ nTrack ] = new SkeletonAnimationTrack.KeyFrame
				////						{
				////							BoneIndex = keyFrame.BoneIndex,
				////							Time = keyFrame.Time,
				////							Position = t.ToVector3F(),
				////							Rotation = r.ToQuaternionF(),
				////							Scale = s.ToVector3F()
				////						};

				////						break;
				////					}
				////				}
				////			}
				////		}
				////	}
				////	else
				////	{
				////		for( int nTrack = 0; nTrack < trackData.Count; nTrack++ )
				////		{
				////			var keyFrame = trackData[ nTrack ];
				////			if( keyFrame.BoneIndex == nBone )
				////			{
				////				var transform = new Transform( keyFrame.Position, keyFrame.Rotation, keyFrame.Scale ).ToMatrix4();

				////				var resultTransform = importContext.globalTransform * transform;
				////				resultTransform.Decompose( out Vector3 t, out Quaternion r, out Vector3 s );

				////				trackData[ nTrack ] = new SkeletonAnimationTrack.KeyFrame
				////				{
				////					BoneIndex = keyFrame.BoneIndex,
				////					Time = keyFrame.Time,
				////					Position = t.ToVector3F(),
				////					Rotation = r.ToQuaternionF(),
				////					Scale = s.ToVector3F()
				////				};

				////			}
				////		}
				////	}
				////}


				skeletonAnimationTrackComponent.KeyFrames = SkeletonAnimationTrack.ToBytes( trackData );
				skeletonAnimationComponent.Track = ReferenceUtility.MakeThisReference( skeletonAnimationComponent, skeletonAnimationTrackComponent );

				skeletonAnimationComponent.TrackStartTime = minTime;
				skeletonAnimationComponent.Length = maxTime - minTime;
			}
		}
	}
}
