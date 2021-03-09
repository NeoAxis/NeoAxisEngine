// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Assimp.Configs;

namespace NeoAxis.Import
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
			//#if DEBUG
			//         string libraryName = "assimp-vc100-mtd";
			//#else
			string libraryName = "assimp-vc141-mt";
			//#endif

			NativeLibraryManager.PreLoadLibrary( libraryName );

			var assimpLibrary = global::Assimp.Unmanaged.AssimpLibrary.Instance;
			if( !assimpLibrary.IsLibraryLoaded )
			{
				string path = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, libraryName + ".dll" );
				assimpLibrary.LoadLibrary( path );
			}
		}

		class ImportContext
		{
			public Scene scene;
			public Settings settings;
			public string directoryName;
			public Component materialsGroup;
			public EDictionary<int, Component_Material> materialByIndex = new EDictionary<int, Component_Material>();
		}

		//ModelImportSceneSource.BoneSource FindBoneByName( List<ModelImportSceneSource.BoneSource> boneList, string name )
		//{
		//	for( int i = 0; i < boneList.Count; i++ )
		//	{
		//		if( boneList[ i ].Name == name )
		//			return boneList[ i ];
		//	}
		//	return null;
		//}

		//ModelImportSceneSource.BoneSource FindParentBone( List<ModelImportSceneSource.BoneSource> boneList, Node aiParentNode )
		//{
		//	if( aiParentNode == null )
		//		return null;

		//	ModelImportSceneSource.BoneSource parentBone = FindBoneByName( boneList, aiParentNode.Name );
		//	if( parentBone == null )
		//		return FindParentBone( boneList, aiParentNode.Parent );

		//	return parentBone;
		//}

		//int GetChannelIndex( global::Assimp.Animation animation, string name )
		//{
		//	int resultIndex = -1;
		//	for( int i = 0; i < animation.NodeAnimationChannelCount; i++ )
		//	{
		//		if( animation.NodeAnimationChannels[ i ].NodeName == name )
		//		{
		//			resultIndex = i;
		//			break;
		//		}
		//	}
		//	return resultIndex;
		//}

		//int GetKeyFrameIndex( double time, dynamic keys, ref int lastFoundKeyFrameIndex )
		//{
		//	int frameIndex = 0;

		//	if( lastFoundKeyFrameIndex >= 0 && lastFoundKeyFrameIndex < keys.Count )
		//	{
		//		if( keys[ lastFoundKeyFrameIndex ].Time < time )
		//		{
		//			frameIndex = lastFoundKeyFrameIndex;
		//		}
		//		else if( lastFoundKeyFrameIndex > 0 && keys[ lastFoundKeyFrameIndex - 1 ].Time < time )
		//		{
		//			frameIndex = lastFoundKeyFrameIndex - 1;
		//		}
		//	}

		//	while( frameIndex < keys.Count && keys[ frameIndex ].Time < time )
		//		frameIndex++;

		//	lastFoundKeyFrameIndex = frameIndex;

		//	return frameIndex;
		//}

		//Vec3F GetInterpolatedVec3( double time, List<ModelImportSceneSource.BoneSource.BoneVec3Key> keys, ref int lastFoundKeyFrameIndex )
		//{
		//	int frameIndex = GetKeyFrameIndex( time, keys, ref lastFoundKeyFrameIndex );

		//	if( frameIndex == 0 || keys.Count == 1 )
		//		return keys[ 0 ].Offset;

		//	if( frameIndex == keys.Count )
		//		return keys[ keys.Count - 1 ].Offset;

		//	ModelImportSceneSource.BoneSource.BoneVec3Key framePrev = keys[ frameIndex - 1 ];
		//	ModelImportSceneSource.BoneSource.BoneVec3Key frameNext = keys[ frameIndex ];

		//	double length = frameNext.Time - framePrev.Time;
		//	float t = (float)( ( time - framePrev.Time ) / length );
		//	Vec3F result = ( 1 - t ) * framePrev.Offset + t * frameNext.Offset;

		//	return result;
		//}

		//QuatF GetInterpolatedQuat( double time, List<ModelImportSceneSource.BoneSource.BoneQuatKey> keys, ref int lastFoundKeyFrameIndex )
		//{
		//	int frameIndex = GetKeyFrameIndex( time, keys, ref lastFoundKeyFrameIndex );

		//	if( frameIndex == 0 || keys.Count == 1 )
		//		return keys[ 0 ].Rotation;

		//	if( frameIndex == keys.Count )
		//		return keys[ keys.Count - 1 ].Rotation;

		//	ModelImportSceneSource.BoneSource.BoneQuatKey framePrev = keys[ frameIndex - 1 ];
		//	ModelImportSceneSource.BoneSource.BoneQuatKey frameNext = keys[ frameIndex ];

		//	double length = frameNext.Time - framePrev.Time;
		//	float t = (float)( ( time - framePrev.Time ) / length );
		//	return QuatF.Slerp( framePrev.Rotation, frameNext.Rotation, t );
		//}

		//void CreateBones( Dictionary<string, ModelImportSceneSource.BoneSource> boneDictionary,
		//	ModelImportSceneSource sceneSource, Node node, List<Assimp.Animation> aiAnimationVectors,
		//	double[] animationDurations, Set<string> boneNames, Set<string> realBones,
		//	bool needToLoadAnimations, double frameStep )
		//{
		//	string boneName = node.Name;

		//	if( boneName == "" )
		//		boneName = "emptyName";

		//	if( boneNames.Contains( boneName ) )
		//	{
		//		int suffix = 0;
		//		while( boneNames.Contains( boneName + "_" + suffix.ToString() ) )
		//			suffix++;
		//		boneName = boneName + "_" + suffix.ToString();
		//	}

		//	ModelImportSceneSource.BoneSource parentBone = null;
		//	if( node.Parent != null )
		//		parentBone = boneDictionary[ node.Parent.Name ];
		//	Assimp.Node aiParentNode = node.Parent;

		//	Mat4F boneLocalTransform = node.Transform.ToMat4();
		//	if( node.Parent == null )
		//	{
		//		Mat4F transform90Fix = new Mat4F( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );
		//		boneLocalTransform = transform90Fix * boneLocalTransform;
		//	}

		//	Vec3F initialPosition = boneLocalTransform.Item3.ToVec3();
		//	Mat3F rotationM;
		//	Vec3F initialScale;
		//	Vec3F dummy;
		//	boneLocalTransform.ToMat3().QDUDecomposition( out rotationM, out initialScale, out dummy );
		//	QuatF initialRotation = rotationM.ToQuat();

		//	ModelImportSceneSource.BoneSource bone = new ModelImportSceneSource.BoneSource( boneName, parentBone,
		//			 initialPosition, initialRotation, initialScale, realBones.Contains( boneName ) );

		//	boneDictionary.Add( boneName, bone );
		//	boneNames.Add( boneName );
		//	sceneSource.AddBone( bone );

		//	//animations
		//	if( aiAnimationVectors != null && needToLoadAnimations )
		//	{
		//		double animationTrackOffset = 0;
		//		for( int animationIndex = 0; animationIndex < aiAnimationVectors.Count; animationIndex++ )
		//		{
		//			var tempPositionKeys = new List<ModelImportSceneSource.BoneSource.BoneVec3Key>();
		//			var tempRotationKeys = new List<ModelImportSceneSource.BoneSource.BoneQuatKey>();
		//			var tempScalingKeys = new List<ModelImportSceneSource.BoneSource.BoneVec3Key>();

		//			Assimp.Animation aiAnim = aiAnimationVectors[ animationIndex ];
		//			int channelIndex = GetChannelIndex( aiAnim, boneName );
		//			if( channelIndex >= 0 )
		//			{
		//				Assimp.NodeAnimationChannel aiNodeAnimation = aiAnim.NodeAnimationChannels[ channelIndex ];

		//				for( int i = 0; i < aiNodeAnimation.PositionKeyCount; i++ )
		//				{
		//					VectorKey aiPositionKey = aiNodeAnimation.PositionKeys[ i ];
		//					tempPositionKeys.Add( new ModelImportSceneSource.BoneSource.BoneVec3Key(
		//						 aiPositionKey.Time, aiPositionKey.Value.ToVec3() ) );
		//				}

		//				if( aiNodeAnimation.RotationKeyCount > 0 )
		//				{
		//					for( int i = 0; i < aiNodeAnimation.RotationKeyCount; i++ )
		//					{
		//						QuaternionKey aiRotationKey = aiNodeAnimation.RotationKeys[ i ];
		//						tempRotationKeys.Add( new ModelImportSceneSource.BoneSource.BoneQuatKey(
		//							 aiRotationKey.Time, aiRotationKey.Value.ToQuat() ) );
		//					}
		//				}

		//				for( int i = 0; i < aiNodeAnimation.ScalingKeyCount; i++ )
		//				{
		//					VectorKey aiScalingKey = aiNodeAnimation.ScalingKeys[ i ];
		//					tempScalingKeys.Add( new ModelImportSceneSource.BoneSource.BoneVec3Key(
		//						 aiScalingKey.Time, aiScalingKey.Value.ToVec3() ) );
		//				}
		//			}

		//			if( frameStep == 0 )
		//			{
		//				foreach( ModelImportSceneSource.BoneSource.BoneVec3Key key in tempPositionKeys )
		//					bone.AddPositionKey( key.Time + animationTrackOffset, key.Offset );
		//				foreach( ModelImportSceneSource.BoneSource.BoneQuatKey key in tempRotationKeys )
		//					bone.AddRotationKey( key.Time + animationTrackOffset, key.Rotation );
		//				foreach( ModelImportSceneSource.BoneSource.BoneVec3Key key in tempScalingKeys )
		//					bone.AddScalingKey( key.Time + animationTrackOffset, key.Offset );
		//			}
		//			else
		//			{
		//				if( tempPositionKeys.Count == 0 )
		//					tempPositionKeys.Add( new ModelImportSceneSource.BoneSource.BoneVec3Key( 0, initialPosition ) );
		//				if( tempRotationKeys.Count == 0 )
		//					tempRotationKeys.Add( new ModelImportSceneSource.BoneSource.BoneQuatKey( 0, initialRotation ) );
		//				if( tempScalingKeys.Count == 0 )
		//					tempScalingKeys.Add( new ModelImportSceneSource.BoneSource.BoneVec3Key( 0, initialScale ) );

		//				int lastFoundPositionKeyFrameIndex = -1;
		//				int lastFoundRotationKeyFrameIndex = -1;
		//				int lastFoundScaleKeyFrameIndex = -1;

		//				for( double time = 0; time < animationDurations[ animationIndex ] + frameStep; time += frameStep )
		//				{
		//					double frameTime = time;
		//					if( frameTime > animationDurations[ animationIndex ] )
		//						frameTime = animationDurations[ animationIndex ];

		//					Vec3F positionKey = GetInterpolatedVec3( frameTime, tempPositionKeys,
		//						 ref lastFoundPositionKeyFrameIndex );
		//					bone.AddPositionKey( frameTime + animationTrackOffset, positionKey );

		//					QuatF rotationKey = GetInterpolatedQuat( frameTime, tempRotationKeys,
		//						 ref lastFoundRotationKeyFrameIndex );
		//					bone.AddRotationKey( frameTime + animationTrackOffset, rotationKey );

		//					Vec3F scalingKey = GetInterpolatedVec3( frameTime, tempScalingKeys,
		//						 ref lastFoundScaleKeyFrameIndex );
		//					bone.AddScalingKey( frameTime + animationTrackOffset, scalingKey );
		//				}
		//			}

		//			animationTrackOffset = animationTrackOffset + animationDurations[ animationIndex ] + 1;
		//		}
		//	}

		//	for( int i = 0; i < node.ChildCount; i++ )
		//	{
		//		CreateBones( boneDictionary, sceneSource, node.Children[ i ], aiAnimationVectors, animationDurations,
		//				  boneNames, realBones, needToLoadAnimations, frameStep );
		//	}
		//}

		static bool HasTransformMatrixNegParity( Matrix3F m )
		{
			return Vector3F.Dot( Vector3F.Cross( m.Item0, m.Item1 ), m.Item2 ) < 0.0f ? true : false;
		}

		static bool ContainsMeshesRecursive( Node node )
		{
			if( node.HasMeshes )
				return true;
			foreach( var child in node.Children )
			{
				if( ContainsMeshesRecursive( child ) )
					return true;
			}
			return false;
		}

		static void InitMeshGeometriesRecursive( ImportContext importContext, Node node, Matrix4 nodeTransform, Component_Mesh mesh )
		{
			//!!!!пока меши не индексируются/не инстансятся.

			foreach( var meshIndex in node.MeshIndices )
			{
				var aiMesh = importContext.scene.Meshes[ meshIndex ];

				StandardVertex[] vertices = new StandardVertex[ aiMesh.VertexCount ];

				bool hasVertexColor = aiMesh.HasVertexColors( 0 );
				List<Color4D> colors0 = null;
				if( hasVertexColor )
					colors0 = aiMesh.VertexColorChannels[ 0 ];

				int textureCoordsCount = 0;
				for( int n = 0; n < 4 && n < aiMesh.TextureCoordinateChannelCount; n++ )
				{
					if( aiMesh.HasTextureCoords( n ) )
						textureCoordsCount++;
					else
						break;
				}
				List<Vector3D> texCoords0 = textureCoordsCount > 0 ? aiMesh.TextureCoordinateChannels[ 0 ] : null;
				List<Vector3D> texCoords1 = textureCoordsCount > 1 ? aiMesh.TextureCoordinateChannels[ 1 ] : null;
				List<Vector3D> texCoords2 = textureCoordsCount > 2 ? aiMesh.TextureCoordinateChannels[ 2 ] : null;
				List<Vector3D> texCoords3 = textureCoordsCount > 3 ? aiMesh.TextureCoordinateChannels[ 3 ] : null;

				Matrix4 geometryTransform = nodeTransform;
				geometryTransform.Decompose( out _, out Matrix3 geometryTransformR, out _ );

				//{
				//geometryTransform = importContext.settings.globalTransform.ToMat4F() * nodeTransform;// childTransform;
				//geometryTransform = nodeTransform;// childTransform;

				//var importTransform = importContext.settings.component.ImportTransform.Value;
				//geometryTransform = importTransform.ToMat4().ToMat4F();
				//geometryTransformR = importTransform.Rotation.ToMat3().ToMat3F();
				//}

				//Mat4F transform90Fix = new Mat4F( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );
				//Mat3F transform90RFix = transform90Fix.ToMat3();

				//Mat4F geometryTransform;
				//Mat3F geometryTransformR;
				//if( aiMesh.HasBones )
				//{
				//	geometryTransform = transform90Fix * importTransform;
				//	//geometryTransform = nodeTransform;
				//	Vec3F dummy1;
				//	Vec3F dummy2;
				//	geometryTransform.ToMat3().QDUDecomposition( out geometryTransformR, out dummy1, out dummy2 );
				//}
				//else
				//{
				//	geometryTransform = Mat4F.Identity;
				//	geometryTransformR = Mat3F.Identity;
				//}

				//if( HasTransformMatrixNegParity( geometryTransform .GetTranspose().ToMat3() ) )
				//{
				//   //what to do?
				//}

				//get data
				for( int n = 0; n < vertices.Length; n++ )
				{
					StandardVertex vertex = new StandardVertex();

					vertex.Position = ( geometryTransform * ToVector3( aiMesh.Vertices[ n ] ) ).ToVector3F();
					if( aiMesh.HasNormals )
						vertex.Normal = ( geometryTransformR * ToVector3( aiMesh.Normals[ n ] ) ).ToVector3F().GetNormalize();

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
						vertex.Color = ToColorValue( colors0[ n ] );

					if( aiMesh.HasTangentBasis )
					{
						Vector3F tangent = ( geometryTransformR * ToVector3( aiMesh.Tangents[ n ] ) ).ToVector3F().GetNormalize();
						Vector3F binormal = ( geometryTransformR * ToVector3( aiMesh.BiTangents[ n ] ) ).ToVector3F().GetNormalize();

						float parity;
						if( Vector3F.Dot( Vector3F.Cross( tangent, binormal ), vertex.Normal ) >= 0 )
							parity = -1;
						else
							parity = 1;
						vertex.Tangent = new Vector4F( tangent, parity );
					}

					vertices[ n ] = vertex;
				}

				int[] indices = new int[ aiMesh.FaceCount * 3 ];
				for( int n = 0; n < aiMesh.FaceCount; n++ )
				{
					Face face = aiMesh.Faces[ n ];
					indices[ n * 3 + 0 ] = face.Indices[ 0 ];
					indices[ n * 3 + 1 ] = face.Indices[ 1 ];
					indices[ n * 3 + 2 ] = face.Indices[ 2 ];
				}

				//ModelImportSceneSource.MaterialSource material = null;
				//if( aiMesh.MaterialIndex < sceneSource.Materials.Count )
				//	material = sceneSource.Materials[ (int)aiMesh.MaterialIndex ];

				////edit bind pose
				//Dictionary<string, int> boneNameToBoneIndexDictionary = null;
				//if( aiMesh.HasBones )
				//{
				//	Dictionary<string, Assimp.Bone> assimpBonesDictionary = new Dictionary<string, Assimp.Bone>();
				//	for( int i = 0; i < aiMesh.BoneCount; i++ )
				//	{
				//		Assimp.Bone aiBone = aiMesh.Bones[ i ];
				//		try
				//		{
				//			assimpBonesDictionary.Add( aiBone.Name, aiBone );
				//		}
				//		catch { }
				//	}

				//	Dictionary<string, Mat4F> boneGlobalTransforms = new Dictionary<string, Mat4F>();
				//	foreach( ModelImportSceneSource.BoneSource bone in sceneSource.Bones )
				//	{
				//		string boneName = bone.Name;
				//		Assimp.Bone aiBone;
				//		if( assimpBonesDictionary.TryGetValue( boneName, out aiBone ) )
				//		{
				//			Mat4F parentTransform = Mat4F.Identity;
				//			if( bone.Parent != null )
				//				parentTransform = boneGlobalTransforms[ bone.Parent.Name ];
				//			Mat4F boneGlobalTransform = geometryTransform * aiBone.OffsetMatrix.ToMat4().GetInverse();
				//			Mat4F boneBindPoseLocalTransform = parentTransform.GetInverse() * boneGlobalTransform;

				//			Vec3F bonePosition = boneBindPoseLocalTransform.Item3.ToVec3();
				//			Mat3F boneRotationM;
				//			Vec3F boneScale;
				//			Vec3F dummy2;
				//			boneBindPoseLocalTransform.ToMat3().QDUDecomposition( out boneRotationM, out boneScale, out dummy2 );
				//			QuatF boneRotation = boneRotationM.ToQuat();

				//			if( changedBindPoseMatrixBoneDictionary[ boneName ] )
				//			{
				//				if( bone.Position != bonePosition || bone.Rotation != boneRotation || bone.Scale != boneScale )
				//					Log.InvisibleInfo( "Assimp Import Library: " + boneName + " bind pose bone has difference." );
				//			}

				//			bone.Position = bonePosition;
				//			bone.Rotation = boneRotation;
				//			bone.Scale = boneScale;

				//			boneGlobalTransforms.Add( boneName, boneGlobalTransform );
				//			changedBindPoseMatrixBoneDictionary[ boneName ] = true;
				//		}
				//		else
				//		{
				//			Mat4F parentTransform = Mat4F.Identity;
				//			if( bone.Parent != null )
				//				parentTransform = boneGlobalTransforms[ bone.Parent.Name ];

				//			Mat4F localTransform = new Mat4F( bone.Rotation.ToMat3() *
				//				Mat3F.FromScale( bone.Scale ), bone.Position );
				//			Mat4F boneTransform = parentTransform * localTransform;

				//			boneGlobalTransforms.Add( bone.Name, boneTransform );
				//		}
				//	}

				//	boneNameToBoneIndexDictionary = new Dictionary<string, int>();
				//	for( int i = 0; i < sceneSource.Bones.Count; i++ )
				//		boneNameToBoneIndexDictionary.Add( sceneSource.Bones[ i ].Name, i );
				//}

				//ModelImportSceneSource.MeshSource.BoneAssignmentItem[][] boneAssignments = null;
				//if( aiMesh.HasBones )
				//{
				//	boneAssignments = new ModelImportSceneSource.MeshSource.BoneAssignmentItem[ aiMesh.BoneCount ][];

				//	for( int i = 0; i < aiMesh.BoneCount; i++ )
				//	{
				//		Assimp.Bone aiBone = aiMesh.Bones[ i ];
				//		boneAssignments[ i ] = new ModelImportSceneSource.MeshSource.BoneAssignmentItem[ aiBone.VertexWeightCount ];

				//		int boneIndex;
				//		if( boneNameToBoneIndexDictionary.TryGetValue( aiBone.Name, out boneIndex ) )
				//		{
				//			for( int j = 0; j < aiBone.VertexWeightCount; j++ )
				//			{
				//				VertexWeight aiWeight = aiBone.VertexWeights[ j ];
				//				ModelImportSceneSource.MeshSource.BoneAssignmentItem assignment =
				//					new ModelImportSceneSource.MeshSource.BoneAssignmentItem(
				//					(int)aiWeight.VertexID, boneIndex, aiWeight.Weight );
				//				boneAssignments[ i ][ j ] = assignment;
				//			}
				//		}
				//	}
				//}

				//Vec3F position = Vec3F.Zero;
				//QuatF rotation = QuatF.Identity;
				//Vec3F scale = new Vec3F( 1, 1, 1 );
				//if( !aiMesh.HasBones )
				//{
				//	Mat4F nodeTransformRotated = transform90Fix * importTransform;
				//	//Mat4 nodeTransformRotated = nodeTransform;
				//	position = nodeTransformRotated.Item3.ToVec3F();
				//	Mat3F r;
				//	Vec3F dummy;
				//	nodeTransformRotated.ToMat3().QDUDecomposition( out r, out scale, out dummy );
				//	rotation = r.ToQuat();
				//}

				var geometry = mesh.CreateComponent<Component_MeshGeometry>();
				geometry.Name = GetFixedName( aiMesh.Name );

				StandardVertex.Components vertexComponents = StandardVertex.Components.Position;
				if( aiMesh.HasNormals )
					vertexComponents |= StandardVertex.Components.Normal;
				if( aiMesh.HasTangentBasis )
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

				geometry.SetVertexDataWithRemovingHoles( vertices, vertexComponents );
				geometry.Indices = indices;

				//material
				importContext.materialByIndex.TryGetValue( aiMesh.MaterialIndex, out Component_Material material );
				if( material != null )
				{
					var referenceValue = ReferenceUtility.CalculateRootReference( material );
					//var referenceValue = importContext.settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
					//var referenceValue = importContext.settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
					geometry.Material = ReferenceUtility.MakeReference<Component_Material>( null, referenceValue );
					//meshData.Material = ReferenceUtils.CreateReference<Component_Material>( null,
					//	ReferenceUtils.CalculateThisReference( meshData, material ) );
					//meshData.Material = ResourceManager.LoadResource<Component_Material>( "_Dev\\Sphere.material" );
				}
			}

			foreach( var childNode in node.Children )
			{
				var childTransform = nodeTransform * ToMatrix4F( childNode.Transform );
				InitMeshGeometriesRecursive( importContext, childNode, childTransform, mesh );
			}
		}

#if FFFF

		static Component_Mesh CreateMesh( ImportContext importContext, global::Assimp.Mesh aiMesh )
		/*Mat4F nodeTransform, global::Assimp.Node rootNode, global::Assimp.Scene scene,
		Dictionary<string, ModelImportSceneSource.BoneSource> boneDictionary,
		Dictionary<string, bool> changedBindPoseMatrixBoneDictionary*///)
		{
			//get name
			string name = GetFixedName( aiMesh.Name );
			if( string.IsNullOrEmpty( name ) || importContext.groupMeshes.GetComponentByName( name ) != null )
			{
				//!!!!

				string emptyNamePrefix = Path.GetFileNameWithoutExtension( importContext.settings.virtualFileName );
				string prefix = string.IsNullOrEmpty( name ) ? emptyNamePrefix : name;
				for( int counter = 1; ; counter++ )
				{
					name = prefix;
					if( counter != 1 )
						name += counter.ToString();
					if( importContext.groupMeshes.GetComponentByName( name ) == null )
						break;
				}
			}

			StandardVertexF[] vertices = new StandardVertexF[ aiMesh.VertexCount ];

			bool hasVertexColor = aiMesh.HasVertexColors( 0 );
			List<Color4D> colors0 = null;
			if( hasVertexColor )
				colors0 = aiMesh.VertexColorChannels[ 0 ];

			int textureCoordsCount = 0;
			for( int n = 0; n < 4 && n < aiMesh.TextureCoordinateChannelCount; n++ )
			{
				if( aiMesh.HasTextureCoords( n ) )
					textureCoordsCount++;
				else
					break;
			}
			List<Vector3D> texCoords0 = textureCoordsCount > 0 ? aiMesh.TextureCoordinateChannels[ 0 ] : null;
			List<Vector3D> texCoords1 = textureCoordsCount > 1 ? aiMesh.TextureCoordinateChannels[ 1 ] : null;
			List<Vector3D> texCoords2 = textureCoordsCount > 2 ? aiMesh.TextureCoordinateChannels[ 2 ] : null;
			List<Vector3D> texCoords3 = textureCoordsCount > 3 ? aiMesh.TextureCoordinateChannels[ 3 ] : null;

			//!!!!
			Mat4F geometryTransform;
			Mat3F geometryTransformR;
			{
				var importTransform = importContext.settings.component.ImportTransform.Value;
				geometryTransform = importTransform.ToMat4().ToMat4F();
				geometryTransformR = importTransform.Rotation.ToMat3().ToMat3F();
			}

			//!!!!
			//Mat4F transform90Fix = new Mat4F( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );
			//Mat3F transform90RFix = transform90Fix.ToMat3();

			//!!!!
			//Mat4F geometryTransform;
			//Mat3F geometryTransformR;
			//if( aiMesh.HasBones )
			//{
			//	geometryTransform = transform90Fix * importTransform;
			//	//geometryTransform = nodeTransform;
			//	Vec3F dummy1;
			//	Vec3F dummy2;
			//	geometryTransform.ToMat3().QDUDecomposition( out geometryTransformR, out dummy1, out dummy2 );
			//}
			//else
			//{
			//	geometryTransform = Mat4F.Identity;
			//	geometryTransformR = Mat3F.Identity;
			//}

			//if( HasTransformMatrixNegParity( geometryTransform .GetTranspose().ToMat3() ) )
			//{
			//   //what to do?
			//}

			//get data
			for( int n = 0; n < vertices.Length; n++ )
			{
				StandardVertexF vertex = new StandardVertexF();

				vertex.position = geometryTransform * aiMesh.Vertices[ n ].ToVec3();
				if( aiMesh.HasNormals )
					vertex.normal = ( geometryTransformR * aiMesh.Normals[ n ].ToVec3() ).GetNormalize();

				if( textureCoordsCount > 0 )
				{
					vertex.texCoord0 = texCoords0[ n ].ToVec2();
					if( textureCoordsCount > 1 )
					{
						vertex.texCoord1 = texCoords1[ n ].ToVec2();
						if( textureCoordsCount > 2 )
						{
							vertex.texCoord2 = texCoords2[ n ].ToVec2();
							if( textureCoordsCount > 3 )
								vertex.texCoord3 = texCoords3[ n ].ToVec2();
						}
					}
				}

				if( hasVertexColor )
					vertex.color = colors0[ n ].ToColorValue();

				if( aiMesh.HasTangentBasis )
				{
					Vec3F tangent = ( geometryTransformR * aiMesh.Tangents[ n ].ToVec3() ).GetNormalize();
					Vec3F binormal = ( geometryTransformR * aiMesh.BiTangents[ n ].ToVec3() ).GetNormalize();

					float parity;
					if( Vec3F.Dot( Vec3F.Cross( tangent, binormal ), vertex.normal ) >= 0 )
						parity = -1;
					else
						parity = 1;
					vertex.tangent = new Vec4F( tangent, parity );
				}

				vertices[ n ] = vertex;
			}

			int[] indices = new int[ aiMesh.FaceCount * 3 ];
			for( int n = 0; n < aiMesh.FaceCount; n++ )
			{
				Face face = aiMesh.Faces[ n ];
				indices[ n * 3 + 0 ] = face.Indices[ 0 ];
				indices[ n * 3 + 1 ] = face.Indices[ 1 ];
				indices[ n * 3 + 2 ] = face.Indices[ 2 ];
			}

			//!!!!
			//ModelImportSceneSource.MaterialSource material = null;
			//if( aiMesh.MaterialIndex < sceneSource.Materials.Count )
			//	material = sceneSource.Materials[ (int)aiMesh.MaterialIndex ];

			////edit bind pose
			//Dictionary<string, int> boneNameToBoneIndexDictionary = null;
			//if( aiMesh.HasBones )
			//{
			//	Dictionary<string, Assimp.Bone> assimpBonesDictionary = new Dictionary<string, Assimp.Bone>();
			//	for( int i = 0; i < aiMesh.BoneCount; i++ )
			//	{
			//		Assimp.Bone aiBone = aiMesh.Bones[ i ];
			//		try
			//		{
			//			assimpBonesDictionary.Add( aiBone.Name, aiBone );
			//		}
			//		catch { }
			//	}

			//	Dictionary<string, Mat4F> boneGlobalTransforms = new Dictionary<string, Mat4F>();
			//	foreach( ModelImportSceneSource.BoneSource bone in sceneSource.Bones )
			//	{
			//		string boneName = bone.Name;
			//		Assimp.Bone aiBone;
			//		if( assimpBonesDictionary.TryGetValue( boneName, out aiBone ) )
			//		{
			//			Mat4F parentTransform = Mat4F.Identity;
			//			if( bone.Parent != null )
			//				parentTransform = boneGlobalTransforms[ bone.Parent.Name ];
			//			Mat4F boneGlobalTransform = geometryTransform * aiBone.OffsetMatrix.ToMat4().GetInverse();
			//			Mat4F boneBindPoseLocalTransform = parentTransform.GetInverse() * boneGlobalTransform;

			//			Vec3F bonePosition = boneBindPoseLocalTransform.Item3.ToVec3();
			//			Mat3F boneRotationM;
			//			Vec3F boneScale;
			//			Vec3F dummy2;
			//			boneBindPoseLocalTransform.ToMat3().QDUDecomposition( out boneRotationM, out boneScale, out dummy2 );
			//			QuatF boneRotation = boneRotationM.ToQuat();

			//			if( changedBindPoseMatrixBoneDictionary[ boneName ] )
			//			{
			//				if( bone.Position != bonePosition || bone.Rotation != boneRotation || bone.Scale != boneScale )
			//					Log.InvisibleInfo( "Assimp Import Library: " + boneName + " bind pose bone has difference." );
			//			}

			//			bone.Position = bonePosition;
			//			bone.Rotation = boneRotation;
			//			bone.Scale = boneScale;

			//			boneGlobalTransforms.Add( boneName, boneGlobalTransform );
			//			changedBindPoseMatrixBoneDictionary[ boneName ] = true;
			//		}
			//		else
			//		{
			//			Mat4F parentTransform = Mat4F.Identity;
			//			if( bone.Parent != null )
			//				parentTransform = boneGlobalTransforms[ bone.Parent.Name ];

			//			Mat4F localTransform = new Mat4F( bone.Rotation.ToMat3() *
			//				Mat3F.FromScale( bone.Scale ), bone.Position );
			//			Mat4F boneTransform = parentTransform * localTransform;

			//			boneGlobalTransforms.Add( bone.Name, boneTransform );
			//		}
			//	}

			//	boneNameToBoneIndexDictionary = new Dictionary<string, int>();
			//	for( int i = 0; i < sceneSource.Bones.Count; i++ )
			//		boneNameToBoneIndexDictionary.Add( sceneSource.Bones[ i ].Name, i );
			//}

			//ModelImportSceneSource.MeshSource.BoneAssignmentItem[][] boneAssignments = null;
			//if( aiMesh.HasBones )
			//{
			//	boneAssignments = new ModelImportSceneSource.MeshSource.BoneAssignmentItem[ aiMesh.BoneCount ][];

			//	for( int i = 0; i < aiMesh.BoneCount; i++ )
			//	{
			//		Assimp.Bone aiBone = aiMesh.Bones[ i ];
			//		boneAssignments[ i ] = new ModelImportSceneSource.MeshSource.BoneAssignmentItem[ aiBone.VertexWeightCount ];

			//		int boneIndex;
			//		if( boneNameToBoneIndexDictionary.TryGetValue( aiBone.Name, out boneIndex ) )
			//		{
			//			for( int j = 0; j < aiBone.VertexWeightCount; j++ )
			//			{
			//				VertexWeight aiWeight = aiBone.VertexWeights[ j ];
			//				ModelImportSceneSource.MeshSource.BoneAssignmentItem assignment =
			//					new ModelImportSceneSource.MeshSource.BoneAssignmentItem(
			//					(int)aiWeight.VertexID, boneIndex, aiWeight.Weight );
			//				boneAssignments[ i ][ j ] = assignment;
			//			}
			//		}
			//	}
			//}

			//!!!!
			//Vec3F position = Vec3F.Zero;
			//QuatF rotation = QuatF.Identity;
			//Vec3F scale = new Vec3F( 1, 1, 1 );
			//if( !aiMesh.HasBones )
			//{
			//	Mat4F nodeTransformRotated = transform90Fix * importTransform;
			//	//Mat4 nodeTransformRotated = nodeTransform;
			//	position = nodeTransformRotated.Item3.ToVec3F();
			//	Mat3F r;
			//	Vec3F dummy;
			//	nodeTransformRotated.ToMat3().QDUDecomposition( out r, out scale, out dummy );
			//	rotation = r.ToQuat();
			//}

			Component_Mesh mesh = importContext.groupMeshes.CreateComponent<Component_Mesh>( -1, false );
			mesh.Name = GetFixedName( name );

			StandardVertexF.Components vertexComponents = StandardVertexF.Components.Position;
			if( aiMesh.HasNormals )
				vertexComponents |= StandardVertexF.Components.Normal;
			if( aiMesh.HasTangentBasis )
				vertexComponents |= StandardVertexF.Components.Tangent;
			if( hasVertexColor )
				vertexComponents |= StandardVertexF.Components.Color;
			if( textureCoordsCount > 0 )
				vertexComponents |= StandardVertexF.Components.TexCoord0;
			if( textureCoordsCount > 1 )
				vertexComponents |= StandardVertexF.Components.TexCoord1;
			if( textureCoordsCount > 2 )
				vertexComponents |= StandardVertexF.Components.TexCoord2;
			if( textureCoordsCount > 3 )
				vertexComponents |= StandardVertexF.Components.TexCoord3;

			mesh.SetVertexDataWithRemovingHoles( vertices, vertexComponents );
			mesh.Indices = indices;

			////material
			//materialByIndex.TryGetValue( aiMesh.MaterialIndex, out Component_Material material );
			//if( material != null )
			//{
			//	//не через "this:", впрочем неважно, т.к. полный путь есть при указании типа в "Sources".

			//	var referenceValue = settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
			//	meshData.Material = ReferenceUtils.CreateReference<Component_Material>( null, referenceValue );

			//	//meshData.Material = ReferenceUtils.CreateReference<Component_Material>( null,
			//	//	ReferenceUtils.CalculateThisReference( meshData, material ) );
			//	//meshData.Material = ResourceManager.LoadResource<Component_Material>( "_Dev\\Sphere.material" );
			//}

			//!!!!так? тут?
			if( aiMesh.HasBones )
			{
				//!!!!temp

				var skeleton = mesh.CreateComponent<Component_Skeleton>( -1, false );
				skeleton.Name = "Skeleton";

				mesh.Skeleton = new Reference<Component_Skeleton>( null, ReferenceUtils.CalculateThisReference( mesh, skeleton ) );

				var bone1 = skeleton.CreateComponent<Component_Bone>( -1, false );
				bone1.Name = "Bone 1";
				var bone2 = bone1.CreateComponent<Component_Bone>( -1, false );
				bone2.Name = "Bone 2";
				var bone3 = bone1.CreateComponent<Component_Bone>( -1, false );
				bone3.Name = "Bone 3";

				var animation1 = skeleton.CreateComponent<Component_Animation>( -1, false );
				animation1.Name = "Animation 1";
				var animation2 = skeleton.CreateComponent<Component_Animation>( -1, false );
				animation2.Name = "Animation 2";
			}

			//LOD
			{
				//!!!!temp

				var lod1 = mesh.CreateComponent<Component_MeshLevelOfDetail>( -1, false );
				lod1.Name = "LOD 1";
				//lod1.Mesh = xxx;
				lod1.Distance = 50;

				var lod2 = mesh.CreateComponent<Component_MeshLevelOfDetail>( -1, false );
				lod2.Name = "LOD 2";
				lod2.Distance = 100;
			}

			mesh.Enabled = true;

			//name,
			//vertices, 
			//textureCoordsCount, 
			//aiMesh.HasTangentBasis, 
			//hasVertexColor, 
			//indices, 
			//material,
			//position, 
			//rotation, 
			//scale, 
			//boneAssignments

			////create mesh source
			//ModelImportSceneSource.MeshSource meshSource = new ModelImportSceneSource.MeshSource( name,
			//	vertices, textureCoordsCount, aiMesh.HasTangentBasis, hasVertexColor, indices, material,
			//	position, rotation, scale, boneAssignments );
			//sceneSource.AddMesh( meshSource );

			return mesh;
		}

		static void InitMeshFromNodesRecursive( Settings settings, global::Assimp.Scene scene,
			ImportContext importContext, Component_Mesh group, global::Assimp.Node node,
			Mat4F nodeTransform, int level /*,
			Dictionary<string, ModelImportSceneSource.BoneSource> boneDictionary,
			Dictionary<string, bool> changedBindPoseMatrixBoneDictionary*/ )
		{
			//set transform
			{
				nodeTransform.Decompose( out Vec3F pos, out QuatF rot, out Vec3F scl );
				group.TransformRelativeToParent = new Reference<Transform>( new Transform( pos, rot, scl ) );
			}

			//children nodes
			for( int n = 0; n < node.ChildCount; n++ )
			{
				global::Assimp.Node childNode = node.Children[ n ];

				//!!!!просто по имени?
				//!!!!тип проверять еще
				var childGroup = group.GetComponentByName( childNode.Name );
				if( childGroup == null )
				{
					childGroup = group.CreateComponent<Component_Mesh>();
					childGroup.Name = childNode.Name;
				}

				if( childGroup is Component_Mesh )
					InitMeshFromNodesRecursive( settings, scene, importContext, (Component_Mesh)childGroup, childNode, nodeTransform * childNode.Transform.ToMat4(), level + 1 /*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );

				//remove empty
				if( childGroup.Components.Count == 0 )
					childGroup.RemoveFromParent( false );
			}

			//meshes
			for( int nMesh = 0; nMesh < node.MeshCount; nMesh++ )
			{
				if( importContext.sourcesMeshByIndex.TryGetValue( node.MeshIndices[ nMesh ], out Component_Mesh sourceMesh ) )
				{
					var type = sourceMesh.GetProvidedType();
					if( type != null )
					{
						//!!!!просто по имени?
						//!!!!тип проверять еще
						var obj = group.GetComponentByName( sourceMesh.Name );
						if( obj == null )
						{
							obj = group.CreateComponent( type );
							//var obj = group.CreateComponent( MetadataManager.MetadataGetType( original ), -1, false );
							obj.Name = sourceMesh.Name;

							//set material
							if( importContext.groupMaterials != null )
							{
								var mesh = (Component_Mesh)obj;

								importContext.materialNamePathByMeshNamePath.TryGetValue( mesh.GetNameWithIndexFromParent(), out string materialNamePath );
								if( !string.IsNullOrEmpty( materialNamePath ) )
								{
									var material = importContext.groupMaterials.GetComponentByNamePath( materialNamePath ) as Component_Material;
									if( material != null )
									{
										//не через "this:", впрочем неважно, т.к. полный путь есть при указании типа в "Sources".

										var referenceValue = settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
										mesh.Material = ReferenceUtils.CreateReference<Component_Material>( null, referenceValue );

										//meshData.Material = ReferenceUtils.CreateReference<Component_Material>( null,
										//	ReferenceUtils.CalculateThisReference( meshData, material ) );
										//meshData.Material = ResourceManager.LoadResource<Component_Material>( "_Dev\\Sphere.material" );
									}
								}
							}
						}
					}

					////!!!!так?
					//global::Assimp.Mesh aiMesh = scene.Meshes[ node.MeshIndices[ nMesh ] ];

					//if( aiMesh.PrimitiveType == PrimitiveType.Triangle )
					//{
					//	////"this:..\\..\\Meshes\\{0}"
					//	//string r = "this:";
					//	//for( int n = 0; n < level + 2; n++ )
					//	//	r += "..\\";
					//	//r += string.Format( "$Meshes\\{0}", mesh.GetNameWithIndexFromParent() );
					//	////r += string.Format( "Meshes\\{0}", mesh.Name );
					//	//Reference<Component_Mesh> referenceToMesh = new Reference<Component_Mesh>( null, r );

					//	Component_MeshInSpace objectInSpace = group.CreateComponent<Component_MeshInSpace>();
					//	objectInSpace.Mesh = ReferenceUtils.CreateReference<Component_Mesh>( null,
					//		ReferenceUtils.CalculateThisReference( objectInSpace, mesh ) );
					//	//objInSpace.Mesh = referenceToMesh;

					//	nodeTransform.Decompose( out Vec3F pos, out QuatF rot, out Vec3F scl );

					//	//!!!!не так
					//	objectInSpace.Transform = new Reference<Transform>( new Transform( pos, rot, scl ) );

					//	//!!!!что-то еще выставлять?
					//}
				}
			}
		}

		//bool HasSceneSkeleton( global::Assimp.Scene scene )
		//{
		//	bool hasSkeleton = false;
		//	for( int i = 0; i < scene.MeshCount; i++ )
		//	{
		//		if( scene.Meshes[ i ].HasBones )
		//		{
		//			hasSkeleton = true;
		//			break;
		//		}
		//	}
		//	return hasSkeleton;
		//}

		//bool AddRealBonesRecursive( global::Assimp.Node boneNode, Set<string> realBones )
		//{
		//	bool needToAddBone = realBones.Contains( boneNode.Name );

		//	for( int n = 0; n < boneNode.ChildCount; n++ )
		//	{
		//		Assimp.Node childNode = boneNode.Children[ n ];
		//		if( AddRealBonesRecursive( childNode, realBones ) )
		//			needToAddBone = true;
		//	}

		//	if( needToAddBone )
		//		realBones.AddWithCheckAlreadyContained( boneNode.Name );

		//	return needToAddBone;
		//}

		//Set<string> GetRealBones( Assimp.Scene scene )
		//{
		//	Set<string> realBones = new Set<string>();

		//	if( scene.HasMeshes )
		//	{
		//		for( int i = 0; i < scene.MeshCount; i++ )
		//		{
		//			Assimp.Mesh aiMesh = scene.Meshes[ i ];
		//			for( int j = 0; j < aiMesh.BoneCount; j++ )
		//			{
		//				Assimp.Bone aiBone = aiMesh.Bones[ j ];
		//				realBones.AddWithCheckAlreadyContained( aiBone.Name );
		//			}
		//		}
		//		AddRealBonesRecursive( scene.RootNode, realBones );
		//	}

		//	return realBones;
		//}

#endif

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
						PostProcessSteps.JoinIdenticalVertices |
						//PostProcessSteps.MakeLeftHanded |
						PostProcessSteps.Triangulate |
						PostProcessSteps.RemoveComponent |
						PostProcessSteps.GenerateSmoothNormals |
						//PostProcessSteps.SplitLargeMeshes |
						//PostProcessSteps.PreTransformVertices |
						PostProcessSteps.LimitBoneWeights |
						PostProcessSteps.ValidateDataStructure |
						PostProcessSteps.ImproveCacheLocality |
						//PostProcessSteps.RemoveRedundantMaterials |
						//PostProcessSteps.FixInFacingNormals | //!!!!?
						PostProcessSteps.SortByPrimitiveType |
						PostProcessSteps.FindDegenerates |
						PostProcessSteps.FindInvalidData |
						PostProcessSteps.GenerateUVCoords |
						PostProcessSteps.TransformUVCoords |
						//PostProcessSteps.FindInstances | //!!!!как опцию
						PostProcessSteps.OptimizeMeshes |
						//PostProcessSteps.OptimizeGraph | //!!!!как опцию
						//PostProcessSteps.FlipUVs |
						//PostProcessSteps.FlipWindingOrder |
						//PostProcessSteps.SplitByBoneCount |
						//PostProcessSteps.Debone |
						0;

					//flip with disabled FlipUVs, no flip when enabled
					if( !settings.component.FlipUVs )
						flags |= PostProcessSteps.FlipUVs;

					Scene scene = null;
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

					//public List<Animation> Animations { get; }
					//public SceneFlags SceneFlags { get; set; }
					//public List<EmbeddedTexture> Textures { get; }

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
						Component_Material material = null;
						if( context.settings.updateMaterials )
							material = CreateMaterial( context.materialsGroup, data );
						else
						{
							if( context.materialsGroup != null )
								material = context.materialsGroup.GetComponent( data.Name ) as Component_Material;
						}
						if( material != null )
							context.materialByIndex.Add( data.Index, material );
					}

					////animation
					//double[] animationDurations = null;
					//if( scene.HasAnimations )
					//{
					//	double animationTrackOffset = 0;
					//	List<ModelImportSceneSource.AnimationTrackItem> animationTracks =
					//		new List<ModelImportSceneSource.AnimationTrackItem>();
					//	animationDurations = new double[ scene.AnimationCount ];

					//	for( int animationIndex = 0; animationIndex < scene.AnimationCount; animationIndex++ )
					//	{
					//		Assimp.Animation animation = scene.Animations[ animationIndex ];

					//		double animationTrackDuration = animation.DurationInTicks;
					//		animationDurations[ animationIndex ] = animationTrackDuration;

					//		animationTracks.Add( new ModelImportSceneSource.AnimationTrackItem( animation.Name,
					//			animationTrackOffset, animationTrackOffset + animationTrackDuration ) );

					//		animationTrackOffset = animationTrackOffset + animationTrackDuration + 1;
					//	}

					//	sceneSource.HasAnimations = true;
					//	if( scene.Animations[ 0 ].TicksPerSecond != 0.0 )
					//		sceneSource.AnimationFrameRate = (float)scene.Animations[ 0 ].TicksPerSecond;
					//	sceneSource.AnimationTracks = animationTracks.ToArray();
					//}

					////create skeleton
					//Dictionary<string, ModelImportSceneSource.BoneSource> boneDictionary = null;
					//Dictionary<string, bool> changedBindPoseMatrixBoneDictionary = null;
					//if( HasSceneSkeleton( scene ) )
					//{
					//	boneDictionary = new Dictionary<string, ModelImportSceneSource.BoneSource>();

					//	Set<string> boneNames = new Set<string>();
					//	Set<string> realBones = GetRealBones( scene );

					//	CreateBones( boneDictionary, sceneSource, scene.RootNode, scene.Animations,
					//		 animationDurations, boneNames, realBones, needToLoadAnimations, frameStep );

					//	changedBindPoseMatrixBoneDictionary = new Dictionary<string, bool>();
					//	foreach( ModelImportSceneSource.BoneSource bone in sceneSource.Bones )
					//		changedBindPoseMatrixBoneDictionary.Add( bone.Name, false );
					//}

					Matrix3 rotation = settings.component.Rotation.Value.ToMatrix3();

					Matrix3 rotateByX = Matrix3.Identity;
					if( settings.component.ForceFrontXAxis )
						rotateByX = new Matrix3( 1, 0, 0, 0, 0, 1, 0, -1, 0 );

					Matrix4 globalTransform = new Matrix4( rotation * rotateByX, settings.component.Position ) * ToMatrix4( scene.RootNode.Transform );
					globalTransform.Decompose( out _, out Matrix3 globalTransformR, out _ );

					//Matrix4 rotation = Matrix4.Identity;
					//if( settings.component.Rotation.Value != Quaternion.Identity )
					//	rotation = settings.component.Rotation.Value.ToMatrix3().ToMatrix4();

					//Matrix4 rotateByX = Matrix4.Identity;
					//if( settings.component.ForceFrontXAxis )
					//	rotateByX = new Matrix4( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );

					//Matrix4 globalTransform = rotation * rotateByX * scene.RootNode.Transform.ToMat4();

					var mode = settings.component.Mode.Value;
					if( mode == Component_Import3D.ModeEnum.Auto )
						mode = Component_Import3D.ModeEnum.OneMesh;

					//create one mesh (OneMesh mode)
					if( mode == Component_Import3D.ModeEnum.OneMesh && scene.HasMeshes && scene.MeshCount != 0 && settings.updateMeshes )
					{
						//mesh
						var mesh = settings.component.CreateComponent<Component_Mesh>( enabled: false );
						mesh.Name = "Mesh";

						foreach( var node in scene.RootNode.Children )
						{
							var transform = globalTransform * ToMatrix4( node.Transform );
							InitMeshGeometriesRecursive( context, node, transform, mesh );
						}

						if( settings.component.MergeMeshGeometries )
							mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();

						mesh.Enabled = true;
					}

					//create meshes, object in space (Meshes mode)
					if( mode == Component_Import3D.ModeEnum.Meshes && scene.HasMeshes && scene.MeshCount != 0 )
					{
						var meshesGroup = settings.component.GetComponent( "Meshes" );

						//Meshes
						if( settings.updateMeshes )
						{
							meshesGroup = settings.component.CreateComponent<Component>( enabled: false );
							meshesGroup.Name = "Meshes";

							foreach( var node in scene.RootNode.Children )
							{
								//!!!!transform?

								var transform = globalTransform * ToMatrix4( node.Transform );

								var mesh = meshesGroup.CreateComponent<Component_Mesh>();
								InitMeshGeometriesRecursive( context, node, transform, mesh );

								if( mesh.Components.Count != 0 )
								{
									mesh.Name = mesh.Components.ToArray()[ 0 ].Name;

									//!!!!transform?

									if( settings.component.MergeMeshGeometries )
										mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();
								}
								else
									mesh.Dispose();
							}

							meshesGroup.Enabled = true;
						}

						//Object In Space
						if( settings.updateObjectsInSpace && meshesGroup != null )
						{
							var objectInSpace = settings.component.CreateComponent<Component_ObjectInSpace>( enabled: false );
							objectInSpace.Name = "Object In Space";

							foreach( var mesh in meshesGroup.Components )
							{
								var meshInSpace = objectInSpace.CreateComponent<Component_MeshInSpace>();
								meshInSpace.Name = mesh.Name;
								meshInSpace.CanBeSelected = false;
								meshInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateRootReference( mesh ) );

								//Transform
								//!!!!transform?
								var pos = Vector3.Zero;
								var rot = Quaternion.Identity;
								var scl = Vector3.One;
								//( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );

								var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
								transformOffset.Name = "Transform Offset";
								transformOffset.PositionOffset = pos;
								transformOffset.RotationOffset = rot;
								transformOffset.ScaleOffset = scl;
								transformOffset.Source = ReferenceUtility.MakeReference<Transform>( null,
									ReferenceUtility.CalculateThisReference( transformOffset, objectInSpace, "Transform" ) );

								meshInSpace.Transform = ReferenceUtility.MakeReference<Transform>( null,
									ReferenceUtility.CalculateThisReference( meshInSpace, transformOffset, "Result" ) );
							}

							objectInSpace.Enabled = true;
						}


						//!!!!не Clean update

						//for( int nMesh = 0; nMesh < scene.MeshCount; nMesh++ )
						//{
						//	global::Assimp.Mesh aiMesh = scene.Meshes[ nMesh ];
						//	if( aiMesh.PrimitiveType == PrimitiveType.Triangle )
						//	{
						//		xx xx;

						//		var mesh = CreateMesh( importContext, aiMesh );

						//		importContext.sourcesMeshByIndex.Add( nMesh, mesh );

						//		importContext.sourcesMaterialByIndex.TryGetValue( aiMesh.MaterialIndex, out var material );
						//		if( material != null )
						//			importContext.materialNamePathByMeshNamePath[ mesh.GetNameWithIndexFromParent() ] = material.GetNameWithIndexFromParent();
						//	}
						//}

						//!!!!
						//	//!!!!?
						//	Mat4F transform90Fix = new Mat4F( 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1 );
						//	//var rootTransform = scene.RootNode.Transform.ToMat4();
						//	var rootTransform = transform90Fix * scene.RootNode.Transform.ToMat4();

						//xx xx;
						//	InitMeshFromNodesRecursive( settings, scene, importContext, groupMeshes, scene.RootNode, rootTransform, 0 /*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );
					}

#if DISABLED
					//!!!!
					//create objects in space (Scene mode)
					if( false /*settings.component.Mode.Value == Component_Import3D.ModeEnum.Scene*/ &&
						( importContext.meshesGroup != null || scene.LightCount != 0 || scene.CameraCount != 0 ) )
					{
						var groupName = "Scene Objects";

						//group
						importContext.objectsInSpaceGroup = settings.component.GetComponentByName( groupName ) as Component_ObjectInSpace;
						if( importContext.objectsInSpaceGroup == null )
						{
							importContext.objectsInSpaceGroup = settings.component.CreateComponent<Component_ObjectInSpace>( -1, false );
							importContext.objectsInSpaceGroup.Name = groupName;
						}
						else
							importContext.objectsInSpaceGroup.Enabled = false;

						//meshes in space
						if( importContext.meshesGroup != null )
						{
							foreach( var node in scene.RootNode.Children )
							{
								if( ContainsMeshesRecursive( node ) )
								{
									//mesh
									var mesh = importContext.meshesGroup.CreateComponent<Component_Mesh>();
									mesh.Name = GetFixedName( node.Name );
									InitMeshGeometriesRecursive( importContext, node, Mat4F.Identity, mesh );

									//object in space

									var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<Component_MeshInSpace>();
									objectInSpace.Name = GetFixedName( node.Name );

									var referenceValue = ReferenceUtils.CalculateRootReference( mesh );
									//var referenceValue = settings.virtualFileName + "|" + mesh.GetNamePathToAccessFromRoot();
									objectInSpace.Mesh = ReferenceUtils.CreateReference<Component_Mesh>( null, referenceValue );

									//Transform
									( globalTransform * node.Transform.ToMat4() ).Decompose( out var pos, out Quat rot, out var scl );
									var transformOffset = objectInSpace.CreateComponent<Component_TransformOffset>();
									transformOffset.Name = "Attach Transform Offset";
									transformOffset.PositionOffset = pos;
									transformOffset.RotationOffset = rot;
									transformOffset.ScaleOffset = scl;
									transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
										ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
									objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
										ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );

									//objectInSpace.Transform = new Transform( pos, rot, scl );
								}
							}

							//foreach( var obj in groupMeshes.Components )
							//{
							//	var mesh = obj as Component_Mesh;
							//	if( mesh != null )
							//	{
							//		//!!!!по имени проверять? везде так
							//		var objectInSpace = groupObjectsInSpace.GetComponentByName( mesh.Name );
							//		if( objectInSpace == null )
							//		{
							//			var objectInSpace2 = groupObjectsInSpace.CreateComponent<Component_MeshInSpace>();
							//			objectInSpace2.Name = mesh.Name;

							//			var referenceValue = settings.virtualFileName + "|" + mesh.GetNamePathToAccessFromRoot();
							//			objectInSpace2.Mesh = ReferenceUtils.CreateReference<Component_Mesh>( null, referenceValue );

							//			objectInSpace2.Transform = mesh.TransformRelativeToParent;
							//		}
							//	}
							//}
						}

						//lights
						//!!!!
						if( false )
						{
							foreach( var light in scene.Lights )
							{
								var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<Component_Light>();
								objectInSpace.Name = light.Name;

								//Transform
								var transformOffset = objectInSpace.CreateComponent<Component_TransformOffset>();
								transformOffset.Name = "Attach Transform Offset";
								transformOffset.PositionOffset = light.Position.ToVec3();

								//!!!!
								//globalTransform*

								//!!!!temp
								transformOffset.PositionOffset = new Vec3( 0, 0, -1 );

								//!!!!globalTransform, globalTransformR
								transformOffset.RotationOffset = Quat.FromDirectionZAxisUp( light.Direction.ToVec3() );
								transformOffset.ScaleOffset = Vec3.One;
								transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
									ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
								objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
									ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );
								//objectInSpace.Transform = new Transform( light.Position.ToVec3(), Quat.FromDirectionZAxisUp( light.Direction.ToVec3() ), Vec3.One );

								//type
								switch( light.LightType )
								{
								case LightSourceType.Directional: objectInSpace.Type = Component_Light.TypeEnum.Directional; break;
								case LightSourceType.Point: objectInSpace.Type = Component_Light.TypeEnum.Point; break;
								case LightSourceType.Spot: objectInSpace.Type = Component_Light.TypeEnum.Spotlight; break;
								default: objectInSpace.Type = Component_Light.TypeEnum.Point; break;
								}

								//!!!!всё ниже не работает

								//power
								ColorValue color = new ColorValue( light.ColorDiffuse.R, light.ColorDiffuse.G, light.ColorDiffuse.B );
								objectInSpace.Power = new ColorValuePowered( color );
								//public Color3D ColorSpecular { get; set; }
								//public Color3D ColorAmbient { get; set; }

								//spot angles
								if( light.LightType == LightSourceType.Spot )
								{
									if( light.AngleInnerCone != 0 || light.AngleOuterCone != 0 )
									{
										objectInSpace.SpotlightInnerAngle = new Radian( light.AngleInnerCone ).InDegrees();
										objectInSpace.SpotlightOuterAngle = new Radian( light.AngleOuterCone ).InDegrees();
									}
								}

								//attenuation

								//!!!!
								//public float AttenuationConstant { get; set; }
								//public float AttenuationLinear { get; set; }
								//public float AttenuationQuadratic { get; set; }
								//objectInSpace.AttenuationNear
								//objectInSpace.AttenuationFar
								//objectInSpace.AttenuationPower
							}
						}

						//cameras
						//!!!!
						if( false )
						{
							foreach( var camera in scene.Cameras )
							{
								var objectInSpace = importContext.objectsInSpaceGroup.CreateComponent<Component_Camera>();
								objectInSpace.Name = camera.Name;

								//Transform
								var transformOffset = objectInSpace.CreateComponent<Component_TransformOffset>();
								transformOffset.Name = "Attach Transform Offset";
								transformOffset.PositionOffset = globalTransform * camera.Position.ToVec3();
								transformOffset.RotationOffset = ( globalTransformR * Mat3.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() ) ).ToQuat();
								//transformOffset.PositionOffset = camera.Position.ToVec3().ToVec3();
								//transformOffset.RotationOffset = Quat.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() );
								transformOffset.ScaleOffset = Vec3.One;
								transformOffset.Source = ReferenceUtils.CreateReference<Transform>( null,
									ReferenceUtils.CalculateThisReference( transformOffset, importContext.objectsInSpaceGroup, "Transform" ) );
								objectInSpace.Transform = ReferenceUtils.CreateReference<Transform>( null,
									ReferenceUtils.CalculateThisReference( objectInSpace, transformOffset, "Result" ) );
								//objectInSpace.Transform = new Transform( camera.Position.ToVec3(), 
								//	Quat.LookAt( camera.Direction.ToVec3(), camera.Up.ToVec3() ), Vec3.One );

								objectInSpace.NearClipPlane = camera.ClipPlaneNear;
								objectInSpace.FarClipPlane = camera.ClipPlaneFar;
								objectInSpace.AspectRatio = camera.AspectRatio;
								objectInSpace.FieldOfView = new Radian( camera.FieldOfview ).InDegrees();
								//public Matrix4x4 ViewMatrix { get; }
							}
						}
					}

					//enable groups
					//if( importContext.meshesGroup != null )
					//	importContext.meshesGroup.Enabled = true;
					//if( importContext.objectInSpaceGroup != null )
					//	importContext.objectInSpaceGroup.Enabled = true;
					if( importContext.objectsInSpaceGroup != null )
						importContext.objectsInSpaceGroup.Enabled = true;
#endif


					////create meshes
					//if( scene.HasMeshes && scene.MeshCount != 0 )
					//{
					//	var groupMeshes = settings.component.GetComponentByName( "Meshes" ) as Component_Mesh;
					//	if( groupMeshes == null )
					//	{
					//		groupMeshes = settings.component.CreateComponent<Component_Mesh>( -1, false );
					//		groupMeshes.Name = "Meshes";
					//	}
					//	else
					//		groupMeshes.Enabled = false;

					//	foreach( var item in sourcesMeshByIndex )
					//	{
					//		var original = item.Value;
					//		var type = original.GetProvidedType();
					//		if( type != null )
					//		{
					//			//!!!!тип проверять еще
					//			var obj = groupMeshes.GetComponentByName( original.Name );
					//			if( obj == null )
					//			{
					//				obj = groupMeshes.CreateComponent( type, -1, false );
					//				//var obj = group.CreateComponent( MetadataManager.MetadataGetType( original ), -1, false );
					//				obj.Name = original.Name;
					//				obj.Enabled = true;

					//				//material
					//				var mesh = obj as Component_Mesh;
					//				if( mesh != null )
					//				{
					//					if( groupMaterials != null )
					//					{
					//						materialNamePathByMeshNamePath.TryGetValue( mesh.GetNameWithIndexFromParent(), out string materialNamePath );
					//						if( !string.IsNullOrEmpty( materialNamePath ) )
					//						{
					//							var material = groupMaterials.GetComponentByNamePath( materialNamePath ) as Component_Material;
					//							if( material != null )
					//							{
					//								//не через "this:", впрочем неважно, т.к. полный путь есть при указании типа в "Sources".

					//								var referenceValue = settings.virtualFileName + "|" + material.GetNamePathToAccessFromRoot();
					//								mesh.Material = ReferenceUtils.CreateReference<Component_Material>( null, referenceValue );

					//								//meshData.Material = ReferenceUtils.CreateReference<Component_Material>( null,
					//								//	ReferenceUtils.CalculateThisReference( meshData, material ) );
					//								//meshData.Material = ResourceManager.LoadResource<Component_Material>( "_Dev\\Sphere.material" );
					//							}
					//						}
					//					}
					//				}
					//			}
					//		}
					//	}

					//	groupMeshes.Enabled = true;
					//}

					////!!!!
					////create nodes
					//if( scene.RootNode != null )
					//{
					//	//!!!!
					//	var groupNodes = settings.component.CreateComponent<Component_ObjectInSpace>( -1, false );
					//	groupNodes.Name = "Nodes";

					//	//!!!!
					//	EnumerateNodeRecursive( settings, scene, groupNodes, scene.RootNode, scene.RootNode.Transform.ToMat4(), sourcesMeshByIndex, 0
					//		/*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );

					//	//EnumerateNodeRecursiveNew( settings, scene, /*sceneSource, */scene.RootNode, scene.RootNode,
					//	//	scene.RootNode.Transform.ToMat4()/*, boneDictionary, changedBindPoseMatrixBoneDictionary*/ );

					//	//!!!!
					//	groupNodes.Enabled = true;
					//}

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

		public static Vector2F ToVector2F( Vector3D value )
		{
			return new Vector2F( value.X, value.Y );
		}

		public static Vector3 ToVector3( Vector3D value )
		{
			return new Vector3( value.X, value.Y, value.Z );
		}

		public static Vector3F ToVector3F( Vector3D value )
		{
			return new Vector3F( value.X, value.Y, value.Z );
		}

		public static Vector4 ToVector4( Color4D value )
		{
			return new Vector4( value.R, value.G, value.B, value.A );
		}

		public static Vector4F ToVector4F( Color4D value )
		{
			return new Vector4F( value.R, value.G, value.B, value.A );
		}

		public static Matrix4 ToMatrix4( Matrix4x4 value )
		{
			return new Matrix4(
				value.A1, value.B1, value.C1, value.D1,
				value.A2, value.B2, value.C2, value.D2,
				value.A3, value.B3, value.C3, value.D3,
				value.A4, value.B4, value.C4, value.D4 );
		}

		public static Matrix4F ToMatrix4F( Matrix4x4 value )
		{
			return new Matrix4F(
				value.A1, value.B1, value.C1, value.D1,
				value.A2, value.B2, value.C2, value.D2,
				value.A3, value.B3, value.C3, value.D3,
				value.A4, value.B4, value.C4, value.D4 );
		}

		public static Quaternion ToQuat( Assimp.Quaternion value )
		{
			return new Quaternion( value.X, value.Y, value.Z, value.W );
		}

		public static QuaternionF ToQuatF( Assimp.Quaternion value )
		{
			return new QuaternionF( value.X, value.Y, value.Z, value.W );
		}

		public static ColorValue ToColorValue( Color4D value )
		{
			return new ColorValue( value.R, value.G, value.B, value.A );
		}

		public static Vector3D ToVector3( Vector2F value )
		{
			return new Vector3D( value.X, value.Y, 0.0f );
		}

		public static Vector3D ToVector3( Vector3F value )
		{
			return new Vector3D( value.X, value.Y, value.Z );
		}

		public static Color4D ToColor( Vector4F value )
		{
			return new Color4D( value.X, value.Y, value.Z, value.W );
		}

		public static Assimp.Quaternion ToQuaternion( QuaternionF value )
		{
			return new Assimp.Quaternion( value.X, value.Y, value.Z, value.W );
		}

		public static Color4D ToColor4D( ColorValue value )
		{
			return new Color4D( value.Red, value.Green, value.Blue, value.Alpha );
		}

		public static List<Vector3D> ToVector3DList( Vector3F[] value )
		{
			var list = new List<Vector3D>( value.Length );
			foreach( var item in value )
				list.Add( ToVector3( item ) );
			return list;
		}

		public static List<Vector3D> ToVector3DList( Vector2F[] value )
		{
			var list = new List<Vector3D>( value.Length );
			foreach( var item in value )
				list.Add( ToVector3( item ) );
			return list;
		}

		public static List<Color4D> ToColor4DList( Vector4F[] value )
		{
			var list = new List<Color4D>( value.Length );
			foreach( var item in value )
				list.Add( ToColor( item ) );
			return list;
		}

		/////////////////////////////////////////
		// Materials data

		static List<MaterialData> GetMaterialsData( ImportContext importContext )
		{
			var result = new List<MaterialData>();

			var scene = importContext.scene;
			if( scene.HasMaterials && scene.MaterialCount != 0 )
			{
				for( int nMaterial = 0; nMaterial < scene.MaterialCount; nMaterial++ )
				{
					var aiMaterial = scene.Materials[ nMaterial ];

					var data = new MaterialData();
					data.Index = nMaterial;
					data.Name = GetFixedName( aiMaterial.Name );

					try
					{
						//TwoSided
						if( aiMaterial.HasTwoSided && aiMaterial.IsTwoSided )
							data.TwoSided = true;

						//BaseColor
						if( aiMaterial.HasColorDiffuse )
						{
							var scale = ToVector4F( aiMaterial.ColorDiffuse );
							data.BaseColor = new ColorValue( scale.X, scale.Y, scale.Z );
						}


						//!!!!support texture coord channels. right now support only channel 0


						var textureTypes = new List<TextureType>();
						textureTypes.Add( TextureType.Diffuse );
						textureTypes.Add( TextureType.Normals );
						textureTypes.Add( TextureType.Emissive );
						textureTypes.Add( TextureType.Lightmap );
						textureTypes.Add( TextureType.AmbientOcclusion );
						textureTypes.Add( TextureType.Metalness );
						textureTypes.Add( TextureType.Roughness );
						textureTypes.Add( TextureType.Displacement );
						textureTypes.Add( TextureType.Height );
						textureTypes.Add( TextureType.Opacity );


						foreach( var textureType in textureTypes )
						{
							for( int nTexCoord = 0; nTexCoord < 4; nTexCoord++ )
							{
								aiMaterial.GetMaterialTexture( textureType, nTexCoord, out var slot );

								if( !string.IsNullOrEmpty( slot.FilePath ) && nTexCoord == 0 )
								{
									var filePath = slot.FilePath;
									if( filePath.Length > 2 && filePath.Substring( 0, 2 ) == "./" )
										filePath = filePath.Substring( 2 );

									var fullPath = Path.Combine( importContext.directoryName, filePath );
									if( VirtualFile.Exists( fullPath ) )
									{

										switch( textureType )
										{
										case TextureType.Diffuse:
											data.BaseColorTexture = fullPath;
											break;

										case TextureType.Normals:
											data.NormalTexture = fullPath;
											break;

										case TextureType.Emissive:
											data.EmissiveTexture = fullPath;
											break;

										case TextureType.Metalness:
											data.MetallicTexture = fullPath;
											break;

										case TextureType.Roughness:
											data.RoughnessTexture = fullPath;
											break;

										case TextureType.Displacement:
										case TextureType.Height:
											data.DisplacementTexture = fullPath;
											break;

										case TextureType.Opacity:
											data.OpacityTexture = fullPath;
											break;

										case TextureType.Lightmap:
										case TextureType.AmbientOcclusion:
											data.AmbientOcclusionTexture = fullPath;

											//Apps specific (Sketchfab)
											if( filePath.ToLower().Contains( "_metallicroughness." ) )
											{
												data.RoughnessTexture = fullPath;
												data.RoughnessTextureChannel = "G";
												data.MetallicTexture = fullPath;
												data.MetallicTextureChannel = "B";
											}

											break;
										}

									}
								}
							}
						}

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
	}
}
