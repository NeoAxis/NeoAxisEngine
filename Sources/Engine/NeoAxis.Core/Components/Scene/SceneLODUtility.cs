// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		/////////////////////////////////////////

		//struct? дл€ структуры мен€ть в списке
		public class RenderingContextItem
		{
			public ViewportRenderingContext context;
			public double lastUpdateTime;
			public int currentLOD;
			public int toLOD;
			public float transitionTime;
		}

		/////////////////////////////////////////

		//detect what LOD is need. can be -1 when skip by visibility distance
		static int DetectDemandedLOD( ViewportRenderingContext context, Component_Mesh mesh, double objectVisibilityDistance, ref Vector3 objectPosition )
		{
			int demandLOD;

			var cameraSettings = context.Owner.CameraSettings;

			double maxDistance;
			if( mesh != null )
				maxDistance = Math.Min( objectVisibilityDistance, mesh.VisibilityDistance );
			else
				maxDistance = objectVisibilityDistance;

			if( maxDistance < cameraSettings.FarClipDistance && ( cameraSettings.Position - objectPosition ).LengthSquared() > maxDistance * maxDistance )
			{
				demandLOD = -1;
			}
			else
			{
				demandLOD = 0;

				if( mesh != null )
				{
					var lods = mesh.Result.MeshData.LODs;
					if( lods != null )
					{
						//!!!!что про дребежжание когда быстро туда сюда переключатьс€ нужно?

						var distanceSquared = (float)( objectPosition - cameraSettings.Position ).LengthSquared() * context.renderingPipeline.LODScale.Value;
						var range = context.renderingPipeline.LODRange.Value.ToVector2I() - new Vector2I( 1, 1 );

						for( int n = Math.Min( lods.Length - 1, range.Y ); n >= 0; n-- )
						{
							ref var lod = ref lods[ n ];

							var lodDistanceSquared = lod.DistanceSquared;
							if( distanceSquared > lodDistanceSquared || n <= range.X )
							{
								var lodMeshData = lod.Mesh?.Result?.MeshData;
								if( lodMeshData != null )
								{
									demandLOD = n + 1;
									break;
								}
							}
						}
					}
				}
			}

			return demandLOD;
		}

		static void DeleteOldRenderingContextItems( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context )
		{
			if( renderingContextItems != null )
			{
				var checkTime = context.Owner.LastUpdateTime - context.Owner.LastUpdateTimeStep - .00001;

				for( int n = renderingContextItems.Count - 1; n >= 0; n-- )
				{
					var item = renderingContextItems[ n ];
					if( item.lastUpdateTime < checkTime )
						renderingContextItems.RemoveAt( n );
				}
			}
		}

		static RenderingContextItem GetContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context )
		{
			if( renderingContextItems != null )
			{
				for( int n = 0; n < renderingContextItems.Count; n++ )
					if( renderingContextItems[ n ].context == context )
						return renderingContextItems[ n ];
			}
			return null;
		}

		public static RenderingContextItem UpdateAndGetContextItem( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext context, Component_Mesh mesh, double objectVisibilityDistance, ref Vector3 objectPosition )
		{
			var demandLOD = DetectDemandedLOD( context, mesh, objectVisibilityDistance, ref objectPosition );

			DeleteOldRenderingContextItems( ref renderingContextItems, context );

			//get rendering context item
			var contextItem = GetContextItem( ref renderingContextItems, context );

			//update LOD transition state
			if( contextItem == null )
			{
				contextItem = new RenderingContextItem();
				contextItem.context = context;
				contextItem.lastUpdateTime = EngineApp.EngineTime;
				contextItem.currentLOD = demandLOD;
				contextItem.toLOD = demandLOD;
				contextItem.transitionTime = 0;

				if( renderingContextItems == null )
					renderingContextItems = new List<RenderingContextItem>();

				//remove when too much items
				while( renderingContextItems.Count > 20 )
					renderingContextItems.RemoveAt( 0 );

				renderingContextItems.Add( contextItem );
			}
			else
			{
				contextItem.lastUpdateTime = EngineApp.EngineTime;

				if( contextItem.toLOD == demandLOD )
				{
					if( contextItem.currentLOD != contextItem.toLOD )
					{
						var lodTransitionTime = context.renderingPipeline.LODTransitionTime.Value;
						if( lodTransitionTime != 0 )
						{
							contextItem.transitionTime += (float)( context.Owner.LastUpdateTimeStep / lodTransitionTime );
							if( contextItem.transitionTime >= 1 )
							{
								//end transition
								contextItem.currentLOD = demandLOD;
								contextItem.transitionTime = 0;
							}
						}
						else
						{
							//end transition
							contextItem.currentLOD = demandLOD;
							contextItem.transitionTime = 0;
						}
					}
				}
				else
				{
					if( contextItem.currentLOD == demandLOD )
					{
						//swap. transition to another direction
						var c = contextItem.currentLOD;
						contextItem.currentLOD = contextItem.toLOD;
						contextItem.toLOD = c;
						contextItem.transitionTime = 1.0f - contextItem.transitionTime;
					}
					else
					{
						contextItem.toLOD = demandLOD;

						//if need switch more than 1 then switch without transition
						if( mesh != null )
						{
							var lods = mesh.Result.MeshData.LODs;
							int maxLOD = lods != null ? lods.Length : 0;
							var current = contextItem.currentLOD != -1 ? contextItem.currentLOD : maxLOD + 1;
							var to = contextItem.toLOD != -1 ? contextItem.toLOD : maxLOD + 1;
							if( Math.Abs( current - to ) > 1 )
								contextItem.currentLOD = contextItem.toLOD;
						}

						if( contextItem.currentLOD == contextItem.toLOD )
							contextItem.transitionTime = 0;
					}
				}
			}

			return contextItem;
		}

		public static void ResetLodTransitionStates( ref List<RenderingContextItem> renderingContextItems, ViewportRenderingContext resetOnlySpecifiedContext = null )
		{
			if( resetOnlySpecifiedContext != null )
			{
				if( renderingContextItems != null )
				{
					for( int n = 0; n < renderingContextItems.Count; n++ )
					{
						if( renderingContextItems[ n ].context == resetOnlySpecifiedContext )
						{
							renderingContextItems.RemoveAt( n );
							break;
						}
					}
				}
			}
			else
				renderingContextItems = null;
		}

	}
}
