// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbx;

namespace NeoAxis.Import.FBX
{
	class SceneLoader
	{
		public Skeleton Skeleton;
		public List<AnimationTrackInfo> AnimationTracks;
		public List<MeshData> Geometries = new List<MeshData>();
		public double FrameRate; //??? Как-то использовать?
		FbxSkin[] Skins;
		//bool mInvertTransparency;
		FbxScene scene;

		public void Load( FbxScene scene, FbxManager manager, ImportOptions options, Matrix4 additionalTransform /*nodeTransform*/)
		{
			this.scene = scene;

			FbxNode rootNode = scene.GetRootNode();
			if( rootNode != null )
			{
				LoadAnimations( scene );

				var skins = new List<FbxSkin>();
				LoadSkinsFromNodeRecursive( rootNode, skins );
				Skins = skins.ToArray();

				if( HasSceneSkeleton( rootNode ) )
					Skeleton = new Skeleton( rootNode, this, Skins, AnimationTracks );

				//for (int i = 0; i < rootNode.GetChildCount(); i++)
				//	EnumerateNodeRecursive(rootNode.GetChild(i));
				//CheckTransparencyInverting();
				ReadMeshGeometriesRecursive( manager, rootNode, options, additionalTransform );
				//if (options.ImportPostProcessFlags.HasFlag(ImportPostProcessFlags.MergeGeometriesByMaterials))
				//	MergeGeometriesByMaterials();
			}

		}

		//void EnumerateNodeRecursive(FbxNode pNode)
		//{
		//	FbxMesh pMesh;
		//	if (IsNodeMesh(pNode, out pMesh))
		//	{
		//		CreateMaterialsFromNode(pNode);
		//		CreateMeshesFromNode(pNode, pMesh);
		//	}

		//	for (int j = 0; j < pNode.GetChildCount(); j++)
		//		EnumerateNodeRecursive(pNode.GetChild(j));
		//}

		void LoadAnimations( FbxScene scene )
		{
			FbxTime.EMode timeMode = scene.GetGlobalSettings().GetTimeMode();
			FrameRate = FbxTime.GetFrameRate( timeMode );

			int animationCount = FbxExtensions.GetAnimStackCount( scene );
			AnimationTracks = new List<AnimationTrackInfo>();
			for( int i = 0; i < animationCount; i++ )
			{
				FbxAnimStack pAnimStack = FbxExtensions.GetAnimStack( scene, i );
				AnimationTrackInfo pAnimationTrack = new AnimationTrackInfo( pAnimStack, scene.GetRootNode() );
				AnimationTracks.Add( pAnimationTrack );
			}
		}

		static void LoadSkinsFromNodeRecursive( FbxNode pNode, List<FbxSkin> skinsResult )
		{
			FbxNodeAttribute pNodeAttribute = pNode.GetNodeAttribute();
			if( pNodeAttribute?.GetAttributeType() == FbxNodeAttribute.EType.eMesh )
			{
				FbxMesh pMesh = pNode.GetMesh();
				int skinCount = pMesh.GetDeformerCount( FbxDeformer.EDeformerType.eSkin );
				for( int i = 0; i < skinCount; i++ )
				{
					FbxSkin pSkin = FbxSkin.Cast( pMesh.GetDeformer( i, FbxDeformer.EDeformerType.eSkin ) );
					skinsResult.Add( pSkin );
				}
			}

			for( int i = 0; i < pNode.GetChildCount(); i++ )
				LoadSkinsFromNodeRecursive( pNode.GetChild( i ), skinsResult );
		}

		bool HasSceneSkeleton( FbxNode pNode )
		{
			if( Skeleton.IsNodeSkeleton( pNode ) )
				return true;

			for( int i = 0; i < pNode.GetChildCount(); i++ )
			{
				if( HasSceneSkeleton( pNode.GetChild( i ) ) )
					return true;
			}
			return false;
		}

		void ReadMeshGeometriesRecursive( FbxManager manager, FbxNode node, ImportOptions options, Matrix4 additionalTransform/*nodeTransform*/)
		{
			FbxExtensions.GetNodeMesh( node, out FbxMesh fbxMesh );
			if( fbxMesh != null )
			{
				var res = ProcessMesh.ProcMesh( manager, node, fbxMesh, options, additionalTransform );
				if( res != null )
				{
					foreach( var geom in res )
					{
						geom.Name = ImportGeneral.GetFixedName( geom.Node.GetName() );
						if( Skeleton != null )
							FillBoneAssignments( geom, Skeleton );
						Geometries.Add( geom );
					}
				}
			}

			for( int i = 0; i < node.GetChildCount(); i++ )
			{
				var childNode = node.GetChild( i );
				ReadMeshGeometriesRecursive( manager, childNode, options, additionalTransform );
			}
		}

		//const int maxNameLength = 40;

		//static void AppendName(ref string name1, string name2)
		//{
		//	if (name1.Length < maxNameLength)
		//	{
		//		name1 = name1 + " + " + name2;
		//		if (name1.Length >= maxNameLength) //no more append
		//			name1 += " + ...";
		//	}
		//}

		//void MergeGeometriesByMaterials()
		//{
		//	int mergedCount = 0;
		//	for (int i = 0; i < Geometries.Count; i++)
		//	{
		//		MeshData g1 = Geometries[i];
		//		if(g1 == null)
		//			continue;
		//		for (int j = i + 1; j < Geometries.Count; j++)
		//		{
		//			MeshData g2 = Geometries[j];
		//			if (g2 != null && g1.MaterialIndex == g2.MaterialIndex && g1.VertexComponents == g2.VertexComponents && g1.PolygonSize == g2.PolygonSize)
		//			{
		//				var newVertices = new VertexInfo[g1.Vertices.Length + g2.Vertices.Length];
		//				g1.Vertices.CopyTo(newVertices, 0);
		//				g2.Vertices.CopyTo(newVertices, g1.Vertices.Length);
		//				g1.Vertices = newVertices;
		//				g1.ClearCache(); //No longer valid
		//				AppendName(ref g1.Name, g2.Name);
		//				//m1.NormalsSource remains from first
		//				Geometries[j] = null;
		//				mergedCount++;
		//			}
		//		}
		//	}

		//	int count = Geometries.Count - mergedCount;
		//	var res = new List<MeshData>(count);
		//	for (int i = 0; i < count; i++)
		//		if (Geometries[i] != null)
		//			res.Add(Geometries[i]);
		//	Geometries = res;
		//}


		static void FillBoneAssignments( MeshData data, Skeleton skeleton )
		{
			if( skeleton == null )
				for( int i = 0; i < data.Vertices.Length; i++ )
					data.Vertices[ i ].Vertex.BlendIndices = new Vector4I( -1, -1, -1, -1 );
			data.VertexComponents |= StandardVertex.Components.BlendIndices | StandardVertex.Components.BlendWeights;
			Dictionary<int, BoneAssignment> boneAssignments = ProcessSkeleton.GetBoneAssignments( data.Mesh, skeleton );
			if( boneAssignments != null )
				for( int i = 0; i < data.Vertices.Length; i++ )
				{
					ref VertexInfo v = ref data.Vertices[ i ];
					if( boneAssignments.TryGetValue( v.ControlPointIndex, out BoneAssignment ba ) )
						FbxMath.ConvertBoneAssignment( ref ba, out v.Vertex.BlendIndices, out v.Vertex.BlendWeights );
					else
						v.Vertex.BlendIndices = new Vector4I( -1, -1, -1, -1 );
				}
			else
			{
				//Warning : Некоторые меши (например в модели xmasguy.fbx from turbosquid) не имеют привязки vertex to the bones.
				//  Для них mesh.GetDeformerCount(FbxDeformer.EDeformerType.eSkin) == 0. Но в Maya они тоже анимируются.
				//  Поэтому для таких мешей сделан поиск parent bone, и привязка к этой единственной bone. Тогда анимируется корректно. Хотя не известно всегда ли так корректно делать.
				bool found = false;
				int parentBoneIndex = -1;
				if( skeleton != null )
					for( FbxNode parent = data.Mesh.GetNode(); parent != null; parent = parent.GetParent() )
					{
						if( skeleton.BoneIndexDictionary.TryGetValue( parent.GetUniqueID(), out parentBoneIndex ) )
						{
							found = true;
							break;
						}
					}

				if( found )
				{
					for( int i = 0; i < data.Vertices.Length; i++ )
					{
						ref StandardVertex v = ref data.Vertices[ i ].Vertex;
						v.BlendIndices = new Vector4I( parentBoneIndex, -1, -1, -1 );
						v.BlendWeights = new Vector4F( 1, 0, 0, 0 );
					}
				}
			}
		}

		public int CurrentAnimationTrack = -1;
		public void SetCurrentAnimationTrack( int trackIndex )
		{
			if( CurrentAnimationTrack != trackIndex )
			{
				FbxAnimStack pAnimStack = FbxExtensions.GetAnimStack( scene, trackIndex );
				scene.SetCurrentAnimationStack( pAnimStack );
				CurrentAnimationTrack = trackIndex;
			}
		}
	}
}
