// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbx;

namespace NeoAxis.Import.FBX
{
	static class ProcessSkeleton
	{
		// key - control point index
		public static Dictionary<int, BoneAssignment> GetBoneAssignments( FbxMesh mesh, Skeleton skeleton )
		{
			var boneAssignments = new Dictionary<int, BoneAssignment>();
			var weightLists = new Dictionary<int, List<(int, double)>>();

			int skinCount = mesh.GetDeformerCount( FbxDeformer.EDeformerType.eSkin );
			if( skinCount == 0 )
				return null;
			if( 1 < skinCount )
				FbxImportLog.LogMessage( mesh.GetNode(), "Warning! Multiple skins for the mesh" ); //??? Может ли быть в одном Mesh несколько Skins? Скорее всего нет, хоть API позволяет.
			FbxSkin pSkin = FbxSkin.Cast( mesh.GetDeformer( 0, FbxDeformer.EDeformerType.eSkin ) );

			int clusterCount = pSkin.GetClusterCount();
			for( int iCluster = 0; iCluster < clusterCount; iCluster++ )
			{
				FbxCluster pCluster = pSkin.GetCluster( iCluster );

				FbxNode pLink = pCluster.GetLink();
				if( pLink == null )
					continue;

				int weightCount = pCluster.GetControlPointIndicesCount();
				if( weightCount == 0 )
					continue;

				int boneIndex = skeleton.GetBoneIndexByNode( pLink );

				var weightIndices = IntArray.frompointer( pCluster.GetControlPointIndices() );
				var weightValues = DoubleArray.frompointer( pCluster.GetControlPointWeights() );
				for( int i = 0; i < weightCount; i++ )
				{
					int vertexIndex = weightIndices.getitem( i );
					double weight = weightValues.getitem( i );

					if( !weightLists.TryGetValue( vertexIndex, out var lst ) )
					{
						lst = new List<(int, double)>();
						weightLists[ vertexIndex ] = lst;
					}

					lst.Add( (boneIndex, weight) );
				}
			}

			foreach( var pair in weightLists )
				boneAssignments[ pair.Key ] = ConvertBoneWeightListToBoneAssignment( pair.Value );
			return boneAssignments;
		}

		static BoneAssignment ConvertBoneWeightListToBoneAssignment( List<(int, double)> weightList )
		{
			BoneAssignment boneAssignment = new BoneAssignment();
			while( weightList.Count > 4 )
				DeleteTheSmallestWeight( weightList );

			double weightSum = 0;
			for( int i = 0; i < weightList.Count; i++ )
				weightSum += weightList[ i ].Item2;

			boneAssignment.count = weightList.Count;

			if( 1 <= boneAssignment.count )
			{
				boneAssignment.boneIndex0 = weightList[ 0 ].Item1;
				boneAssignment.weight0 = weightList[ 0 ].Item2 / weightSum;
			}
			if( 2 <= boneAssignment.count )
			{
				boneAssignment.boneIndex1 = weightList[ 1 ].Item1;
				boneAssignment.weight1 = weightList[ 1 ].Item2 / weightSum;
			}
			if( 3 <= boneAssignment.count )
			{
				boneAssignment.boneIndex2 = weightList[ 2 ].Item1;
				boneAssignment.weight2 = weightList[ 2 ].Item2 / weightSum;
			}

			if( 4 == boneAssignment.count )
			{
				boneAssignment.boneIndex3 = weightList[ 3 ].Item1;
				boneAssignment.weight3 = weightList[ 3 ].Item2 / weightSum;
			}

			return boneAssignment;
		}

		static void DeleteTheSmallestWeight( List<(int, double)> list )
		{
			if( list == null || list.Count == 0 )
				return;
			double min = list[ 0 ].Item2;
			int minIndex = 0;

			for( int i = 1; i < list.Count; i++ )
			{
				if( list[ i ].Item2 < min )
				{
					min = list[ i ].Item2;
					minIndex = i;
				}
			}
			list.RemoveAt( minIndex );
		}
	}
}
