// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// A helper class to manage level of details of objects.
	/// </summary>
	public static class SceneLODUtility
	{
		public unsafe struct LodState
		{
			const int maxCount = 3;
			//const int maxCount = 10;

			public int Count;
			public fixed int levels[ maxCount ];
			public fixed float minRanges[ maxCount ];
			public fixed float maxRanges[ maxCount ];

			public void GetItem( int index, out int lodLevel, out RangeF lodRange )
			{
				lodLevel = levels[ index ];
				lodRange = new RangeF( minRanges[ index ], maxRanges[ index ] );
			}

			public void Add( int lodLevel, RangeF lodRange )
			{
				if( Count < maxCount )
				{
					levels[ Count ] = lodLevel;
					minRanges[ Count ] = lodRange.Minimum;
					maxRanges[ Count ] = lodRange.Maximum;
					Count++;
				}
			}

			public void FixMinMax()
			{
				if( Count != 0 )
				{
					minRanges[ 0 ] = 0;
					maxRanges[ Count - 1 ] = 1000000;
				}
			}
		}

		/////////////////////////////////////////

		public static float GetCameraDistanceMinSquared( Viewport.CameraSettingsClass cameraSettings, SpaceBounds objectBounds )
		{
			objectBounds.GetCalculatedBoundingBox( out var objBounds );

			var nearestInBounds = cameraSettings.Position;
			MathEx.Clamp( ref nearestInBounds.X, objBounds.Minimum.X, objBounds.Maximum.X );
			MathEx.Clamp( ref nearestInBounds.Y, objBounds.Minimum.Y, objBounds.Maximum.Y );
			MathEx.Clamp( ref nearestInBounds.Z, objBounds.Minimum.Z, objBounds.Maximum.Z );

			return (float)( cameraSettings.Position - nearestInBounds ).LengthSquared();
		}

		public static float GetCameraDistanceMinSquared( Viewport.CameraSettingsClass cameraSettings, ref Bounds objectBounds )
		{
			var nearestInBounds = cameraSettings.Position;
			MathEx.Clamp( ref nearestInBounds.X, objectBounds.Minimum.X, objectBounds.Maximum.X );
			MathEx.Clamp( ref nearestInBounds.Y, objectBounds.Minimum.Y, objectBounds.Maximum.Y );
			MathEx.Clamp( ref nearestInBounds.Z, objectBounds.Minimum.Z, objectBounds.Maximum.Z );

			return (float)( cameraSettings.Position - nearestInBounds ).LengthSquared();
		}

		public static float GetCameraDistanceMax( Viewport.CameraSettingsClass cameraSettings, SpaceBounds objectBounds )
		{
			objectBounds.GetCalculatedBoundingSphere( out var objSphere );

			var centerDistance = ( cameraSettings.Position - objSphere.Origin ).Length();
			var max = centerDistance + objSphere.Radius;

			return (float)max;
		}

		public static float GetCameraDistanceMax( Viewport.CameraSettingsClass cameraSettings, ref Sphere objectBoundingSphere )
		{
			var centerDistance = ( cameraSettings.Position - objectBoundingSphere.Origin ).Length();
			var max = centerDistance + objectBoundingSphere.Radius;

			return (float)max;
		}

		public unsafe static void GetDemandedLODs( ViewportRenderingContext context, Component_Mesh mesh, float cameraDistanceMinSquared, float cameraDistanceMaxSquared, out LodState lodState )
		{
			//var cameraDistanceMinSquared2 = cameraDistanceMinSquared;
			//var cameraDistanceMaxSquared2 = cameraDistanceMaxSquared;

			////!!!!fix for shadows. в случае простых билбордов тень рисуется дальше объекта другого лода, резко пропадает
			//cameraDistanceRangeSquared2.Maximum *= 1.1f;


			lodState = new LodState();

			var lods = mesh.Result.MeshData.LODs;
			if( lods != null )
			{
				RangeF GetLodRange( int lodIndex )
				{
					if( lodIndex == 0 )
						return new RangeF( 0, lods[ 0 ].Distance );
					else
					{
						var arrayIndex = lodIndex - 1;

						var lod = lods[ arrayIndex ];

						float far;
						if( arrayIndex + 1 < lods.Length )
							far = lods[ arrayIndex + 1 ].Distance;
						else
							far = 1000000.0f;

						return new RangeF( lod.Distance, far );
					}
				}

				var lodCount = lods.Length + 1;

				var pipeline = context.renderingPipeline;
				var contextLodRange = pipeline.LODRange.Value;
				var contextLodScale = (float)pipeline.LODScale.Value;

				var lodStart = contextLodRange.Minimum;
				var lodEnd = Math.Min( lodCount - 1, contextLodRange.Maximum );
				if( lodStart > lodEnd )
					lodStart = lodEnd;

				for( int lodIndex = lodStart; lodIndex <= lodEnd; lodIndex++ )
				{
					var lodRange = GetLodRange( lodIndex ) * contextLodScale;
					if( lodIndex == lodStart )
						lodRange.Minimum = 0;
					if( lodIndex == lodEnd )
						lodRange.Maximum = 1000000;

					var min = lodRange.Minimum * 0.9f;
					var minSquared = min * min;

					var max = lodRange.Maximum;
					var maxSquared = max * max;

					//early exit
					if( minSquared > cameraDistanceMaxSquared )
						break;

					if( cameraDistanceMaxSquared >= minSquared && cameraDistanceMinSquared < maxSquared )
						lodState.Add( lodIndex, lodRange );
				}

				if( lodState.Count == 0 )
					lodState.Add( lodEnd, new RangeF( 0, 1000000 ) );

				lodState.FixMinMax();
			}
			else
				lodState.Add( 0, new RangeF( 0, 1000000 ) );
		}

		public static float GetLodValue( RangeF lodRange, float cameraDistance )
		{
			var lodRangeMin = lodRange.Minimum;
			var lodRangeMax = lodRange.Maximum;

			float min2 = lodRangeMin * 0.9f;
			float max2 = lodRangeMax * 0.9f;

			if( cameraDistance > max2 )
			{
				//if(cameraDistance < lodRangeMax)
				if( lodRangeMax != max2 )
					return MathEx.Saturate( ( cameraDistance - max2 ) / ( lodRangeMax - max2 ) );
				else
					return 0.0f;
				//else
				//	return 1.0;
			}
			else if( cameraDistance < lodRangeMin )
			{
				if( cameraDistance > min2 && lodRangeMin != min2 )
					return -MathEx.Saturate( ( cameraDistance - min2 ) / ( lodRangeMin - min2 ) );
				else
					return 1.0f;
			}
			else
				return 0.0f;
		}

	}
}



/////////////////////////////////////////

//public static RangeF GetCameraDistanceRangeSquared( Viewport.CameraSettingsClass cameraSettings, SpaceBounds objectBounds )
//{
//	//!!!!slowly?

//	var objBounds = objectBounds.CalculatedBoundingBox;

//	var nearestInBounds = cameraSettings.Position;
//	MathEx.Clamp( ref nearestInBounds.X, objBounds.Minimum.X, objBounds.Maximum.X );
//	MathEx.Clamp( ref nearestInBounds.Y, objBounds.Minimum.Y, objBounds.Maximum.Y );
//	MathEx.Clamp( ref nearestInBounds.Z, objBounds.Minimum.Z, objBounds.Maximum.Z );

//	var minSquared = ( cameraSettings.Position - nearestInBounds ).LengthSquared();


//	zzzzzzz;

//	objBounds.GetCenter( out var center );
//	var p = cameraSettings.Position;
//	if( p.X > center.X )
//		p.X = objBounds.Minimum.X;
//	else
//		p.X = objBounds.Maximum.X;
//	if( p.Y > center.Y )
//		p.Y = objBounds.Minimum.Y;
//	else
//		p.Y = objBounds.Maximum.Y;
//	if( p.Z > center.Z )
//		p.Z = objBounds.Minimum.Z;
//	else
//		p.Z = objBounds.Maximum.Z;
//	var max = p;

//	zzzzzz;
//	return new RangeF( (float)minSquared, (float)max.Length() );




//	//var objSphere = objectBounds.CalculatedBoundingSphere;

//	//var centerDistance = ( cameraSettings.Position - objSphere.Origin ).Length();
//	//var min = centerDistance - objSphere.Radius;
//	//if( min < 0 )
//	//	min = 0;
//	//var max = centerDistance + objSphere.Radius;

//	//return new RangeF( (float)min, (float)max );

//}



////struct? для структуры менять в списке
//public class RenderingContextItem
//{
//	public ViewportRenderingContext context;
//	public double lastUpdateTime;
//	public int currentLOD;
//	public int toLOD;
//	public float transitionTime;

//	public RenderingContextItem Clone()
//	{
//		var result = new RenderingContextItem();
//		result.context = context;
//		result.lastUpdateTime = lastUpdateTime;
//		result.currentLOD = currentLOD;
//		result.toLOD = toLOD;
//		result.transitionTime = transitionTime;
//		return result;
//	}
//}

/////////////////////////////////////////





//!!!!squared

//!!!!если сфера есть то по ней можно

//var cameraPosition = cameraSettings.Position;

//var objBounds = objectBounds.CalculatedBoundingBox;
//var min = cameraPosition;
//MathEx.Clamp( ref min.X, objBounds.Minimum.X, objBounds.Maximum.X );
//MathEx.Clamp( ref min.Y, objBounds.Minimum.Y, objBounds.Maximum.Y );
//MathEx.Clamp( ref min.Z, objBounds.Minimum.Z, objBounds.Maximum.Z );

//var diff = cameraPosition - objBounds.GetCenter();
//var max = objBounds.GetCenter() - diff;
//MathEx.Clamp( ref max.X, objBounds.Minimum.X, objBounds.Maximum.X );
//MathEx.Clamp( ref max.Y, objBounds.Minimum.Y, objBounds.Maximum.Y );
//MathEx.Clamp( ref max.Z, objBounds.Minimum.Z, objBounds.Maximum.Z );

//return new RangeF( (float)( cameraPosition - min ).Length(), (float)( cameraPosition - max ).Length() );




////detect what LOD is need
//public static void DetectDemandedLOD( ViewportRenderingContext context, Component_Mesh mesh, ref Vector3 objectPosition, out LodState lodState )
//{
//	var result = new LodState();

//	result.Add( 0, new RangeF( -1000000, 1000000 ) );

//	//result.Count = 1;
//	//result.lodLevel0Range = new RangeF( 0, 1000000 );


//	////zzzzzz;//на концах заменить на -1000000, 1000000


//	////!!!!значение с минусом. чем ближе к -1, тем темнее/больше заполненных


//	////!!!!
//	//if( mesh.Result.MeshData.LODs != null )
//	//{
//	//	result.Count = 2;
//	//	result.lodLevel0Index = 0;
//	//	result.lodLevel0Range = new RangeF( 0, 20 );
//	//	result.lodLevel1Index = 1;
//	//	result.lodLevel1Range = new RangeF( 20, 1000000 );

//	//}
//	//lodState = result;
//	//return;


//	////!!!!
//	////if( mesh.Result.MeshData.LODs != null )
//	////{
//	////	result.lodLevel0Index = 1;
//	////	result.lodLevel0Range = new RangeF( 23, 40 );

//	////}
//	////lodState = result;
//	////return;



//	//var lods = mesh.Result.MeshData.LODs;
//	//if( lods != null )
//	//{
//	//	var cameraSettings = context.Owner.CameraSettings;

//	//	//!!!!может для LODScale нужно иначе. выдавать другие range

//	//	//!!!!
//	//	var cameraDistance = (float)( objectPosition - cameraSettings.Position ).Length() * context.renderingPipeline.LODScale.Value;
//	//	//var distanceSquared = (float)( objectPosition - cameraSettings.Position ).LengthSquared() * context.renderingPipeline.LODScale.Value;
//	//	var range = context.renderingPipeline.LODRange.Value.ToVector2I() - new Vector2I( 1, 1 );

//	//	//!!!!range

//	//	RangeF GetLodRange( int lodIndex )
//	//	{
//	//		if( lodIndex == 0 )
//	//			return new RangeF( 0, lods[ 0 ].Distance );
//	//		else
//	//		{
//	//			var arrayIndex = lodIndex - 1;

//	//			var lod = lods[ arrayIndex ];

//	//			float far;
//	//			if( arrayIndex + 1 < lods.Length )
//	//				far = lods[ arrayIndex + 1 ].Distance;
//	//			else
//	//				far = 1000000.0f;

//	//			return new RangeF( lod.Distance, far );
//	//		}
//	//	}

//	//	var lodCount = lods.Length + 1;

//	//	//for( int lodIndex = 0; lodIndex < lodCount; lodIndex++ )
//	//	for( int lodIndex = lodCount - 1; lodIndex >= 0; lodIndex-- )
//	//	{
//	//		var lodRange = GetLodRange( lodIndex );

//	//		if( cameraDistance > lodRange.Minimum )
//	//		{
//	//			result.lodLevel0Index = lodIndex;
//	//			result.lodLevel0Range = lodRange;

//	//			var max2 = lodRange.Maximum * 0.95;
//	//			if( cameraDistance > max2 && lodIndex < lodCount - 1 )
//	//			{
//	//				result.Count = 2;
//	//				result.lodLevel1Index = lodIndex + 1;
//	//				result.lodLevel1Range = GetLodRange( result.lodLevel1Index );
//	//			}


//	//			////!!!!
//	//			//result.Count = 2;
//	//			//result.lodLevel0Range = new RangeF( 0, 1000000 );
//	//			//result.lodLevel1Range = new RangeF( 0, 1000000 );


//	//			break;
//	//		}
//	//	}


//	//	//float previousLodDistance = 1000000.0f;

//	//	//!!!!

//	//	//for( int n = Math.Min( lods.Length - 1, range.Y ); n >= 0; n-- )
//	//	//{
//	//	//	ref var lod = ref lods[ n ];

//	//	//	var lodDistance = lod.Distance;
//	//	//	//var lodDistanceSquared = lodDistance * lodDistance;

//	//	//	if( cameraDistance > lodDistance || n <= range.X )//if( distanceSquared > lodDistanceSquared || n <= range.X )
//	//	//	{
//	//	//		var lodMeshData = lod.Mesh?.Result?.MeshData;
//	//	//		if( lodMeshData != null )
//	//	//		{
//	//	//			var demandLOD = n + 1;

//	//	//			var lodRange = GetLodRange( demandLOD );

//	//	//			var max2 = lodDistance * 0.95;
//	//	//			if( cameraDistance > max2 )
//	//	//			{
//	//	//				result.Count = 2;

//	//	//				zzzzzz;

//	//	//				//!!!!
//	//	//				result.lodLevel0Index = demandLOD;
//	//	//				//result.lodLevel0Range;


//	//	//			}
//	//	//			else
//	//	//			{
//	//	//				result.lodLevel0Index = demandLOD;
//	//	//				result.lodLevel0Range = lodRange;// new RangeF( lodDistance, previousLodDistance );
//	//	//			}

//	//	//			zzzzz;
//	//	//			break;
//	//	//		}
//	//	//	}

//	//	//	//previousLodDistance = lodDistance;
//	//	//}

//	//	//zzzzzzz;//нулевой лод еще

//	//}

//	////var lods2 = mesh.Result.MeshData.LODs;
//	////if( lods2 != null )
//	////{
//	////	result.Count = 1;
//	////	result.lodLevel0Index = lods2.Length;
//	////}
//	////else
//	////{
//	////	result.Count = 1;
//	////}


//	lodState = result;
//	return;




//	//!!!!множители


//	//int demandLOD;

//	//var cameraSettings = context.Owner.CameraSettings;

//	//double maxDistance;
//	//zzzz;
//	//if( mesh != null )
//	//	maxDistance = Math.Min( objectVisibilityDistance, mesh.VisibilityDistance );
//	//else
//	//	maxDistance = objectVisibilityDistance;

//	//if( maxDistance < cameraSettings.FarClipDistance && ( cameraSettings.Position - objectPosition ).LengthSquared() > maxDistance * maxDistance )
//	//{
//	//	//!!!!

//	//	demandLOD = -1;
//	//}
//	//else
//	//{

//	//demandLOD = 0;

//	//!!!!меш всегда должен быть. можно mesh data давать

//	//if( mesh != null )
//	//{
//	//	var lods = mesh.Result.MeshData.LODs;
//	//	if( lods != null )
//	//	{
//	//		//!!!!может множить не squared

//	//		var distanceSquared = (float)( objectPosition - cameraSettings.Position ).LengthSquared() * context.renderingPipeline.LODScale.Value;
//	//		var range = context.renderingPipeline.LODRange.Value.ToVector2I() - new Vector2I( 1, 1 );

//	//		for( int n = Math.Min( lods.Length - 1, range.Y ); n >= 0; n-- )
//	//		{
//	//			ref var lod = ref lods[ n ];

//	//			var lodDistanceSquared = lod.DistanceSquared;
//	//			if( distanceSquared > lodDistanceSquared || n <= range.X )
//	//			{
//	//				var lodMeshData = lod.Mesh?.Result?.MeshData;
//	//				if( lodMeshData != null )
//	//				{
//	//					demandLOD = n + 1;
//	//					break;
//	//				}
//	//			}
//	//		}
//	//	}
//	//}

//	//}

//	//return demandLOD;
//}

//static void DeleteOldRenderingContextItems( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context )
//{
//	if( renderingContextItems != null )
//	{
//		var checkTime = context.Owner.LastUpdateTime - context.Owner.LastUpdateTimeStep - .00001;

//		for( int n = renderingContextItems.Count - 1; n >= 0; n-- )
//		{
//			var item = renderingContextItems[ n ];
//			if( item.lastUpdateTime < checkTime )
//				renderingContextItems.RemoveAt( n );
//		}
//	}
//}

//static RenderingContextItem GetContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context )
//{
//	if( renderingContextItems != null )
//	{
//		for( int n = 0; n < renderingContextItems.Count; n++ )
//			if( renderingContextItems[ n ].context == context )
//				return renderingContextItems[ n ];
//	}
//	return null;
//}

//public static RenderingContextItem UpdateAndGetContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context, Component_Mesh mesh, double objectVisibilityDistance, ref Vector3 objectPosition, int demandLOD )
//{
//	DeleteOldRenderingContextItems( ref renderingContextItems, context );

//	//get rendering context item
//	var contextItem = GetContextItem( ref renderingContextItems, context );

//	//update LOD transition state
//	if( contextItem == null )
//	{
//		contextItem = new RenderingContextItem();
//		contextItem.context = context;
//		contextItem.lastUpdateTime = EngineApp.EngineTime;
//		contextItem.currentLOD = demandLOD;
//		contextItem.toLOD = demandLOD;
//		contextItem.transitionTime = 0;

//		if( renderingContextItems == null )
//			renderingContextItems = new List<RenderingContextItem>();

//		//remove when too much items
//		while( renderingContextItems.Count > 20 )
//			renderingContextItems.RemoveAt( 0 );

//		renderingContextItems.Add( contextItem );
//	}
//	else
//	{
//		contextItem.lastUpdateTime = EngineApp.EngineTime;

//		if( contextItem.toLOD == demandLOD )
//		{
//			if( contextItem.currentLOD != contextItem.toLOD )
//			{
//				var lodTransitionTime = context.renderingPipeline.LODTransitionTime.Value;
//				if( lodTransitionTime != 0 )
//				{
//					contextItem.transitionTime += (float)( context.Owner.LastUpdateTimeStep / lodTransitionTime );
//					if( contextItem.transitionTime >= 1 )
//					{
//						//end transition
//						contextItem.currentLOD = demandLOD;
//						contextItem.transitionTime = 0;
//					}
//				}
//				else
//				{
//					//end transition
//					contextItem.currentLOD = demandLOD;
//					contextItem.transitionTime = 0;
//				}
//			}
//		}
//		else
//		{
//			if( contextItem.currentLOD == demandLOD )
//			{
//				//swap. transition to another direction
//				var c = contextItem.currentLOD;
//				contextItem.currentLOD = contextItem.toLOD;
//				contextItem.toLOD = c;
//				contextItem.transitionTime = 1.0f - contextItem.transitionTime;
//			}
//			else
//			{
//				contextItem.toLOD = demandLOD;

//				//if need switch more than 1 then switch without transition
//				if( mesh != null )
//				{
//					var lods = mesh.Result.MeshData.LODs;
//					int maxLOD = lods != null ? lods.Length : 0;
//					var current = contextItem.currentLOD != -1 ? contextItem.currentLOD : maxLOD + 1;
//					var to = contextItem.toLOD != -1 ? contextItem.toLOD : maxLOD + 1;
//					if( Math.Abs( current - to ) > 1 )
//						contextItem.currentLOD = contextItem.toLOD;
//				}

//				if( contextItem.currentLOD == contextItem.toLOD )
//					contextItem.transitionTime = 0;
//			}
//		}
//	}

//	return contextItem;
//}

//public static RenderingContextItem UpdateAndGetContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context, Component_Mesh mesh, double objectVisibilityDistance, ref Vector3 objectPosition )
//{
//	var demandLOD = DetectDemandedLOD( context, mesh, objectVisibilityDistance, ref objectPosition );
//	return UpdateAndGetContextItem( ref renderingContextItems, context, mesh, objectVisibilityDistance, ref objectPosition, demandLOD );
//}

//public static void ResetLodTransitionStates( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext resetOnlySpecifiedContext = null )
//{
//	if( resetOnlySpecifiedContext != null )
//	{
//		if( renderingContextItems != null )
//		{
//			for( int n = 0; n < renderingContextItems.Count; n++ )
//			{
//				if( renderingContextItems[ n ].context == resetOnlySpecifiedContext )
//				{
//					renderingContextItems.RemoveAt( n );
//					break;
//				}
//			}
//		}
//	}
//	else
//		renderingContextItems = null;
//}

////!!!!
//public static RenderingContextItem GetAndRemoveContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context )
//{
//	if( renderingContextItems != null )
//	{
//		for( int n = renderingContextItems.Count - 1; n >= 0; n-- )
//		{
//			var item = renderingContextItems[ n ];
//			if( item.context == context )
//			{
//				renderingContextItems.RemoveAt( n );
//				return item;
//			}
//		}
//	}
//	return null;
//}

//public static void AddContextItem( ref List<RenderingContextItem> renderingContextItems, RenderingContextItem contextItem )
//{
//	if( renderingContextItems == null )
//		renderingContextItems = new List<RenderingContextItem>();

//	//remove when too much items
//	while( renderingContextItems.Count > 20 )
//		renderingContextItems.RemoveAt( 0 );

//	renderingContextItems.Add( contextItem );
//}




////copy LOD state from another group
//{

//	//for( int n = 0; n < batch.ObjectsSeparateRendering.Count; n++ )
//	//{
//	//	ref var item = ref batch.ObjectsSeparateRendering.Data[ n ];
//	//	if( item.renderingContextItems != null )
//	//		item.renderingContextItems.Clear();
//	//}

//	//if( batch.renderingContextItems != null )
//	//	batch.renderingContextItems.Clear();

//	if( useBatching )
//	{
//		var found = false;

//		for( int n = 0; n < batch.ObjectsSeparateRendering.Count; n++ )
//		{
//			ref var item = ref batch.ObjectsSeparateRendering.Data[ n ];

//			var contextItem = SceneLODUtility.GetAndRemoveContextItem( ref item.renderingContextItems, context );
//			if( contextItem != null && !found )
//			{
//				SceneLODUtility.AddContextItem( ref batch.renderingContextItems, contextItem );
//				found = true;
//			}
//		}
//	}
//	else
//	{
//		var contextItem = SceneLODUtility.GetAndRemoveContextItem( ref batch.renderingContextItems, context );
//		if( contextItem != null )
//		{
//			for( int n = 0; n < batch.ObjectsSeparateRendering.Count; n++ )
//			{
//				ref var item = ref batch.ObjectsSeparateRendering.Data[ n ];
//				SceneLODUtility.AddContextItem( ref item.renderingContextItems, n == 0 ? contextItem : contextItem.Clone() );
//			}
//		}
//	}
//}
