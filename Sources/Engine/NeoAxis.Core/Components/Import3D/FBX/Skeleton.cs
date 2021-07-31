// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Collections.Generic;
using Fbx;

namespace NeoAxis.Import.FBX
{
	// В FBX SDK тоже есть FBXSkeleton.
	class Skeleton
	{
		public List<SkeletonBone> Bones;
		public Dictionary<ulong, int> BoneIndexDictionary; // [nodeUniqueID] = index

		public SceneLoader Scene;

		public SkeletonBone RootBone;

		public Skeleton( FbxNode rootNode, SceneLoader scene, FbxSkin[] skins, List<AnimationTrackInfo> animationTracks )
		{
			Scene = scene;
			BoneIndexDictionary = new Dictionary<ulong, int>();
			Bones = new List<SkeletonBone>();
			var skeletonNodeTree = new Dictionary<ulong, bool>(); // [nodeUniqueID] = isSkeleton
			CheckIsNodeContainsSkeletonRecursive( rootNode, skeletonNodeTree );
			RootBone = AddNodeToSkeletonRecursive( rootNode, null, skeletonNodeTree, skins );

			//Temp - Результат вычисления Skeleton до этого места не зависит от текущего SetCurrentAnimationTrack(..)
			//----------------------------

			foreach( var bone in Bones )
				bone.AnimTracks = new AnimTrack[ animationTracks.Count ];

			for( int i = 0; i < animationTracks.Count; i++ )
			{
				Scene.SetCurrentAnimationTrack( i );
				foreach( var bone in Bones )
					bone.AnimTracks[ i ] = bone.LoadAnimationTrack( bone.Node, animationTracks[ i ] );
			}

			//foreach (var bone in Bones)
			//{
			//	bone.HasAnimation = bone.GetHasAnimation();
			//	//if (!bone.HasAnimation)
			//	//	bone.NotAnimatedLocalTransform = bone.Node.EvaluateLocalTransform().ToMat4();
			//}
		}

		public static bool IsNodeSkeleton( FbxNode node )
		{
			for( int i = 0; i < node.GetNodeAttributeCount(); i++ )
			{
				if( node.GetNodeAttributeByIndex( i ).GetAttributeType() == FbxNodeAttribute.EType.eSkeleton )
					return true;
			}
			return false;
		}

		//Find the cluster that links to the skeleton bone node
		public static FbxCluster FindCluster( FbxNode boneNode, FbxSkin[] skins, out FbxSkin skin )
		{
			for( int i = 0; i < skins.Length; i++ )
			{
				skin = skins[ i ];

				int nClusterCount = skin.GetClusterCount();
				for( int j = 0; j < nClusterCount; j++ )
				{
					FbxCluster fbxCluster = skin.GetCluster( j );
					if( fbxCluster == null )
						continue;

					if( fbxCluster.GetLinkMode() == FbxCluster.ELinkMode.eAdditive && fbxCluster.GetAssociateModel() != null )
						FbxImportLog.LogMessage( boneNode, "Warning! Associated model." );

					if( fbxCluster.GetLink()?.GetUniqueID() == boneNode.GetUniqueID() )
						return fbxCluster;
				}
			}

			skin = null;
			return null;
		}

		static bool CheckIsNodeContainsSkeletonRecursive( FbxNode node, Dictionary<ulong, bool> skeletonNodeTree )
		{
			bool isSkeletonNode = IsNodeSkeleton( node );

			for( int i = 0; i < node.GetChildCount(); i++ )
			{
				FbxNode pChild = node.GetChild( i );
				if( CheckIsNodeContainsSkeletonRecursive( pChild, skeletonNodeTree ) )
					isSkeletonNode = true;
			}

			skeletonNodeTree[ node.GetUniqueID() ] = isSkeletonNode;

			return isSkeletonNode;
		}

		SkeletonBone AddNodeToSkeletonRecursive(
			FbxNode node,
			SkeletonBone parentBone,
			Dictionary<ulong, bool> skeletonNodeTree,
			FbxSkin[] skins )
		{
			if( BoneIndexDictionary.ContainsKey( node.GetUniqueID() ) )
				return null;

			SkeletonBone pBone = new SkeletonBone( node, parentBone, skins, Scene );
			//pBone.CalculateAnimations(animationTracks);
			BoneIndexDictionary[ node.GetUniqueID() ] = Bones.Count;
			Bones.Add( pBone );

			for( int i = 0; i < node.GetChildCount(); i++ )
			{
				FbxNode pChild = node.GetChild( i );
				if( skeletonNodeTree[ pChild.GetUniqueID() ] )
				{
					var childBone = AddNodeToSkeletonRecursive( pChild, pBone, skeletonNodeTree, skins );
					if( childBone != null )
						pBone.Children.Add( childBone );
				}
			}
			return pBone;
		}

		public int GetBoneIndexByNode( FbxNode node )
		{
			return BoneIndexDictionary[ node.GetUniqueID() ];
		}
	}
}
