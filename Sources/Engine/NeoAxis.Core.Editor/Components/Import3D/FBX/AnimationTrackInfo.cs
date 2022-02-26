// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Internal.Fbx;

namespace NeoAxis.Import.FBX
{
	class AnimationTrackInfo
	{
		public string Name;
		//public double Duration;
		List<FbxAnimLayer> mLayers;

		public int GetLayerCount() { return mLayers.Count; }
		public FbxAnimLayer GetLayer( int n ) { return mLayers[ n ]; }

		public AnimationTrackInfo( FbxAnimStack pAnimStack, FbxNode pRootNode )
		{
			Name = pAnimStack.GetName();
			//Duration = GetAnimationMaxTime(pRootNode, pAnimStack);

			int animLayersNum = FbxExtensions.GetAnimLayerCount( pAnimStack );
			mLayers = new List<FbxAnimLayer>();
			for( int j = 0; j < animLayersNum; j++ )
			{
				FbxAnimLayer pAnimLayer = FbxExtensions.GetAnimLayer( pAnimStack, j );
				mLayers.Add( pAnimLayer );
			}
		}

		//static double GetCurveMaxTime( FbxAnimCurve pAnimCurve )
		//{
		//	double maxTime = 0.0;

		//	for( int i = 0; i < pAnimCurve.KeyGetCount(); i++ )
		//	{
		//		double keyTime = pAnimCurve.KeyGet( i ).GetTime().GetSecondDouble();
		//		if( keyTime > maxTime )
		//			maxTime = keyTime;
		//	}

		//	return maxTime;
		//}

		////?? Можно определять время потом, найти max,min по загруженным key frames?
		//static double GetLayerMaxTimeRecursive( FbxNode pNode, FbxAnimLayer pAnimLayer )
		//{
		//	double maxTime = 0.0;

		//	FbxAnimCurve pAnimCurve = null;

		//	pAnimCurve = pNode.LclTranslation.GetCurve( pAnimLayer, "X");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclTranslation.GetCurve( pAnimLayer, "Y");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclTranslation.GetCurve( pAnimLayer, "Z");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclRotation.GetCurve( pAnimLayer, "X");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclRotation.GetCurve( pAnimLayer, "Y");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclRotation.GetCurve( pAnimLayer, "Z");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclScaling.GetCurve( pAnimLayer, "X");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclScaling.GetCurve( pAnimLayer, "Y");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	pAnimCurve = pNode.LclScaling.GetCurve( pAnimLayer, "Z");
		//	if( pAnimCurve != null )
		//		maxTime = Math.Max( maxTime, GetCurveMaxTime( pAnimCurve ) );

		//	for( int i = 0; i < pNode.GetChildCount(); i++ )
		//	{
		//		double childMaxTime = GetLayerMaxTimeRecursive( pNode.GetChild( i ), pAnimLayer );
		//		if( childMaxTime > maxTime )
		//			maxTime = childMaxTime;
		//	}

		//	return maxTime;
		//}


		//static double GetAnimationMaxTime( FbxNode pNode, FbxAnimStack pAnimStack )
		//{
		//	double maxTime = 0.0;
		//	int animLayersNum = FbxExtensions.GetAnimLayerCount( pAnimStack );
		//	for( int j = 0; j < animLayersNum; j++ )
		//	{
		//		FbxAnimLayer pAnimLayer = FbxExtensions.GetAnimLayer( pAnimStack, j );
		//		double layerMaxTime = GetLayerMaxTimeRecursive( pNode, pAnimLayer );
		//		if( layerMaxTime > maxTime )
		//			maxTime = layerMaxTime;
		//	}
		//	return maxTime;
		//}

	}
}
