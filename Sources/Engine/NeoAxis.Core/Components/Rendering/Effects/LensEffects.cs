// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect for rendering lens effects of the camera (lens flares).
	/// </summary>
	[DefaultOrderOfEffect( 2.7 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_LensEffects : Component_RenderingEffect
	{
		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_LensEffects> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		[DefaultValue( 3.0 )]
		[Range( 1, 10 )]
		public Reference<double> FadeSpeed
		{
			get { if( _fadeSpeed.BeginGet() ) FadeSpeed = _fadeSpeed.Get( this ); return _fadeSpeed.value; }
			set { if( _fadeSpeed.BeginSet( ref value ) ) { try { FadeSpeedChanged?.Invoke( this ); } finally { _fadeSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FadeSpeed"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_LensEffects> FadeSpeedChanged;
		ReferenceField<double> _fadeSpeed = 3.0;

		public enum CheckVisibilityMethodEnum
		{
			OcclusionQuery,
			PhysicsRayCast,
		}

		[DefaultValue( CheckVisibilityMethodEnum.OcclusionQuery )]
		public Reference<CheckVisibilityMethodEnum> CheckVisibilityMethod
		{
			get { if( _checkVisibilityMethod.BeginGet() ) CheckVisibilityMethod = _checkVisibilityMethod.Get( this ); return _checkVisibilityMethod.value; }
			set { if( _checkVisibilityMethod.BeginSet( ref value ) ) { try { CheckVisibilityMethodChanged?.Invoke( this ); } finally { _checkVisibilityMethod.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CheckVisibilityMethod"/> property value changes.</summary>
		public event Action<Component_RenderingEffect_LensEffects> CheckVisibilityMethodChanged;
		ReferenceField<CheckVisibilityMethodEnum> _checkVisibilityMethod = CheckVisibilityMethodEnum.OcclusionQuery;

		/////////////////////////////////////////

		class ViewportRenderingContextData
		{
			public class LightItem
			{
				public Component_Light light;
				public double lastTimeVisible;
				public double intensity;
			}
			public List<LightItem> lightItems = new List<LightItem>();

			////////////

			public void Update( double delta, double intensity, double fadeSpeed )
			{
				var toRemove = new List<LightItem>();

				foreach( var item in lightItems )
				{
					if( Time.Current - item.lastTimeVisible < 0.25 + delta )
					{
						item.intensity += delta * fadeSpeed;
						if( item.intensity > intensity )
							item.intensity = intensity;
					}
					else
					{
						item.intensity -= delta * fadeSpeed;
						if( item.intensity < 0 )
							toRemove.Add( item );
					}
				}

				foreach( var item in toRemove )
					lightItems.Remove( item );
			}

			public LightItem GetItem( Component_Light light )
			{
				foreach( var item in lightItems )
				{
					if( item.light == light )
						return item;
				}
				return null;
			}

			public double GetIntensity( Component_Light light )
			{
				var item = GetItem( light );
				if( item != null )
					return item.intensity;
				return 0;
			}

			public void Remove( Component_Light light )
			{
				var item = GetItem( light );
				if( item != null )
					lightItems.Remove( item );
			}

			public void UpdateLastTimeVisible( Component_Light light )
			{
				var item = GetItem( light );
				if( item == null )
				{
					item = new LightItem();
					item.light = light;
					lightItems.Add( item );
				}

				item.lastTimeVisible = Time.Current;
			}
		}

		/////////////////////////////////////////

		void CheckVisibilityScreenPosition( ViewportRenderingContext context, Vector3 position, out bool gotScreenPosition, out bool insideScreen )
		{
			gotScreenPosition = false;
			insideScreen = false;

			if( context.Owner.CameraSettings.ProjectToScreenCoordinates( position, out var screenLightPosition ) )
			{
				gotScreenPosition = true;
				if( new Rectangle( 0, 0, 1, 1 ).Contains( screenLightPosition ) )
					insideScreen = true;
			}
		}

		bool CheckVisibilityPhysics( ViewportRenderingContext context, Vector3 position )
		{
			var scene = context.Owner.AttachedScene;
			if( scene != null )
			{
				var cameraVisibleStartOffset = 1.0;

				var cameraPosition = context.Owner.CameraSettings.Position;
				//if( camera.IsReflected() )
				//	cameraPosition = camera.GetReflectionMatrix() * cameraPosition;

				var direction = ( position - cameraPosition ).GetNormalize();
				var start = cameraPosition + direction * cameraVisibleStartOffset;

				//!!!!contact group
				var item = new PhysicsRayTestItem( new Ray( start, position - start ), 1, -1, PhysicsRayTestItem.ModeEnum.One );
				scene.PhysicsRayTest( item );

				if( item.Result.Length != 0 )
					return false;
			}

			return true;
		}

		protected override void OnRender( ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData, ref Component_Image actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			var pipeline = context.RenderingPipeline as Component_RenderingPipeline_Basic;
			if( pipeline == null )
				return;
			var frameData2 = frameData as Component_RenderingPipeline_Basic.FrameData;
			if( frameData2 == null )
				return;
			if( Intensity.Value == 0 )
				return;

			bool skip = true;
			foreach( var lightIndex in frameData2.LightsInFrustumSorted )
			{
				var lightItem = frameData2.Lights[ lightIndex ];

				if( lightItem.data.children != null )
				{
					foreach( var child in lightItem.data.children )
					{
						var lensFlares = child as Component_LensFlares;
						if( lensFlares != null )
						{
							skip = false;
							break;
						}
					}
				}
			}
			if( skip )
				return;

			//init context data
			ViewportRenderingContextData contextData;
			if( context.anyData.TryGetValue( "LensEffects", out var contextData2 ) )
				contextData = (ViewportRenderingContextData)contextData2;
			else
			{
				contextData = new ViewportRenderingContextData();
				context.anyData[ "LensEffects" ] = contextData;
			}

			//update context data light items
			contextData.Update( context.Owner.LastUpdateTimeStep, Intensity, FadeSpeed );

			//create copy
			var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat, createSimple3DRenderer: true, createCanvasRenderer: true );

			var viewport = newTexture.Result.GetRenderTarget().Viewports[ 0 ];
			//!!!!double precision
			Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();
			Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
			context.SetViewport( viewport, viewMatrix, projectionMatrix );

			pipeline.CopyToCurrentViewport( context, actualTexture );

			var simple3DRenderer = viewport.Simple3DRenderer;
			var canvasRenderer = viewport.CanvasRenderer;

			//render
			foreach( var lightIndex in frameData2.LightsInFrustumSorted )
			{
				var lightItem = frameData2.Lights[ lightIndex ];

				if( lightItem.data.children != null )
				{
					foreach( var child in lightItem.data.children )
					{
						var lensFlares = child as Component_LensFlares;
						if( lensFlares != null )
						{
							var light = lightItem.data.Creator as Component_Light;
							if( light != null )
							{
								CheckVisibilityScreenPosition( context, lightItem.data.Position, out var gotScreenPosition, out var insideScreen );

								if( gotScreenPosition )
								{
									//update
									{
										switch( CheckVisibilityMethod.Value )
										{
										case CheckVisibilityMethodEnum.OcclusionQuery:
											{
												var parameter = (contextData, light);
												if( RenderingSystem.TryCreateOcclusionQuery( OcclusionQueryCallback, parameter, out var query ) )
												{
													var thickness = context.Owner.Simple3DRenderer.GetThicknessByPixelSize( lightItem.data.Position, 5 );
													var bounds = new Bounds( lightItem.data.Position );
													bounds.Expand( thickness * 0.5 );

													simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
													simple3DRenderer.SetOcclusionQuery( query );
													simple3DRenderer.AddBounds( bounds, true );
													//simple3DRenderer.AddSphere( new Sphere( lightItem.data.Position, 0.5 ), 4, true );
													simple3DRenderer.SetOcclusionQuery( null );
												}
											}
											break;

										case CheckVisibilityMethodEnum.PhysicsRayCast:
											if( CheckVisibilityPhysics( context, lightItem.data.Position ) )
												contextData.UpdateLastTimeVisible( light );
											break;
										}
									}

									//render
									var intensity = contextData.GetIntensity( light );
									if( intensity > 0 )
									{
										//simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
										//simple3DRenderer.AddSphere( new Sphere( lightItem.data.Position, 0.5 ), 10, true );

										lensFlares.RenderUI( canvasRenderer, lightItem.data, context.Owner, intensity );
									}
								}
								else
									contextData.Remove( light );
							}
						}
					}
				}
			}

			//render
			if( simple3DRenderer._ViewportRendering_PrepareRenderables() )
			{
				simple3DRenderer._ViewportRendering_RenderToCurrentViewport( context );
				simple3DRenderer._Clear();
			}
			canvasRenderer._ViewportRendering_RenderToCurrentViewport( context, true, context.Owner.LastUpdateTime );


			//free old textures
			context.DynamicTexture_Free( actualTexture );

			//update actual texture
			actualTexture = newTexture;
		}

		static void OcclusionQueryCallback( object callbackParameter, int passingPixels )
		{
			if( passingPixels > 0 )
			{
				var data = ((ViewportRenderingContextData, Component_Light))callbackParameter;
				var contextData = data.Item1;
				var light = data.Item2;

				contextData.UpdateLastTimeVisible( light );
			}
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

	}
}
