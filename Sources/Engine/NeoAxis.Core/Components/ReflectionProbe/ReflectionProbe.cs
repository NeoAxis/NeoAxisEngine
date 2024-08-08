// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component is capturing surroundings of the scene to get reflections and to get ambient lighting data.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.ReflectionProbeSettingsCell" )]
#endif
	public class ReflectionProbe : ObjectInSpace
	{
		//Image captureTexture;
		//bool captureTextureNeedUpdate = true;

		bool processedCubemapNeedUpdate = true;
		ImageComponent processedEnvironmentCubemap;
		Vector4F[] processedIrradianceHarmonics;
		//ImageComponent processedIrradianceCubemap;

		string networkOwnedFileNameOfComponent = "";


		//real-time

		ImageComponent createdImage;
		bool createdImageAtLeastOneTimeUpdated;
		int createdForSize;
		bool createdForHDR;

		Viewport[] createdViewports;
		RenderingPipeline_Basic createdRenderingPipeline;

		bool viewportDuringUpdate;

		double lastVisibleTime;

		/////////////////////////////////////////

		public enum ModeEnum
		{
			Resource,
			Capture,
		}

		/// <summary>
		/// The mode of the reflection probe.
		/// </summary>
		[DefaultValue( ModeEnum.Resource )]//Capture )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set
			{
				if( _mode.BeginSet( this, ref value ) )
				{
					try
					{
						ModeChanged?.Invoke( this );
						//if( Mode.Value != ModeEnum.Capture )
						//	CaptureTextureDispose();
						processedCubemapNeedUpdate = true;
					}
					finally { _mode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<ReflectionProbe> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Resource;// Capture;

		/// <summary>
		/// Whether to affect all scene instead of sphere volume.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Global
		{
			get { if( _global.BeginGet() ) Global = _global.Get( this ); return _global.value; }
			set { if( _global.BeginSet( this, ref value ) ) { try { GlobalChanged?.Invoke( this ); } finally { _global.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Global"/> property value changes.</summary>
		public event Action<ReflectionProbe> GlobalChanged;
		ReferenceField<bool> _global = false;

		//может ColorMultiplier?
		///// <summary>
		///// The brightness of the reflection probe.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 2 )]
		//public Reference<double> Brightness
		//{
		//	get { if( _brightness.BeginGet() ) Brightness = _brightness.Get( this ); return _brightness.value; }
		//	set { if( _brightness.BeginSet( this, ref value ) ) { try { BrightnessChanged?.Invoke( this ); } finally { _brightness.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> BrightnessChanged;
		//ReferenceField<double> _brightness = 1;

		/// <summary>
		/// The cubemap texture of reflection data used by the probe.
		/// </summary>
		//[Category( "Resource" )]
		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		public Reference<ImageComponent> Cubemap
		{
			get { if( _cubemap.BeginGet() ) Cubemap = _cubemap.Get( this ); return _cubemap.value; }
			set
			{
				if( _cubemap.BeginSet( this, ref value ) )
				{
					try
					{
						CubemapChanged?.Invoke( this );
						processedCubemapNeedUpdate = true;
					}
					finally { _cubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Cubemap"/> property value changes.</summary>
		public event Action<ReflectionProbe> CubemapChanged;
		ReferenceField<ImageComponent> _cubemap = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		/// <summary>
		/// The horizontal rotation of the cubemap.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( this, ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<ReflectionProbe> RotationChanged;
		ReferenceField<Degree> _rotation;

		/// <summary>
		/// The factor of the probe effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<ReflectionProbe> IntensityChanged;
		ReferenceField<double> _intensity = 1.0;

		/// <summary>
		/// A cubemap color multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		public Reference<ColorValuePowered> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( this, ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<ReflectionProbe> MultiplierChanged;
		ReferenceField<ColorValuePowered> _multiplier = new ColorValuePowered( 1, 1, 1 );

		///// <summary>
		///// The cubemap texture of irradiance data used by the probe.
		///// </summary>
		//[Category( "Resource" )]
		//public Reference<Image> CubemapIrradiance
		//{
		//	get { if( _cubemapIrradiance.BeginGet() ) CubemapIrradiance = _cubemapIrradiance.Get( this ); return _cubemapIrradiance.value; }
		//	set { if( _cubemapIrradiance.BeginSet( this, ref value ) ) { try { CubemapIrradianceChanged?.Invoke( this ); } finally { _cubemapIrradiance.EndSet(); } } }
		//}
		//public event Action<ReflectionProbe> CubemapIrradianceChanged;
		//ReferenceField<Image> _cubemapIrradiance;


		//!!!!при изменении свойства RealTime удалить ресурсы


		/// <summary>
		/// Whether to capture and pass the information in real-time.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Capture" )]
		public Reference<bool> RealTime
		{
			get { if( _realTime.BeginGet() ) RealTime = _realTime.Get( this ); return _realTime.value; }
			set { if( _realTime.BeginSet( this, ref value ) ) { try { RealTimeChanged?.Invoke( this ); } finally { _realTime.EndSet(); } } }
		}
		public event Action<ReflectionProbe> RealTimeChanged;
		ReferenceField<bool> _realTime = false;

		public enum ResolutionEnum
		{
			//_1,
			//_2,
			//_4,
			//_8,
			//_16,
			_32,
			_64,
			_128,
			_256,
			_512,
			_1024,
			_2048,
			_4096,
			//_8192,
			//_16384,
		}

		//!!!!может зависеть от размера экрана. для realtime
		/// <summary>
		/// The resolution of the capture.
		/// </summary>
		[DefaultValue( ResolutionEnum._512 )]
		[Category( "Capture" )]
		public Reference<ResolutionEnum> Resolution
		{
			get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
			set
			{
				if( _resolution.BeginSet( this, ref value ) )
				{
					try
					{
						ResolutionChanged?.Invoke( this );
						//CaptureTextureDispose();
					}
					finally { _resolution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		public event Action<ReflectionProbe> ResolutionChanged;
		ReferenceField<ResolutionEnum> _resolution = ResolutionEnum._512;

		/// <summary>
		/// Whether the high dynamic range is enabled. For Auto mode HDR is disabled on limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> HighDynamicRange
		{
			get { if( _highDynamicRange.BeginGet() ) HighDynamicRange = _highDynamicRange.Get( this ); return _highDynamicRange.value; }
			set
			{
				if( _highDynamicRange.BeginSet( this, ref value ) )
				{
					try
					{
						HighDynamicRangeChanged?.Invoke( this );
						//CaptureTextureDispose();
					}
					finally { _highDynamicRange.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HighDynamicRange"/> property value changes.</summary>
		public event Action<ReflectionProbe> HighDynamicRangeChanged;
		ReferenceField<AutoTrueFalse> _highDynamicRange = AutoTrueFalse.Auto;

		/// <summary>
		/// Enables using additional render targets during rendering the frame.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> UseRenderTargets
		{
			get { if( _useRenderTargets.BeginGet() ) UseRenderTargets = _useRenderTargets.Get( this ); return _useRenderTargets.value; }
			set { if( _useRenderTargets.BeginSet( this, ref value ) ) { try { UseRenderTargetsChanged?.Invoke( this ); } finally { _useRenderTargets.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseRenderTargets"/> property value changes.</summary>
		public event Action<ReflectionProbe> UseRenderTargetsChanged;
		ReferenceField<AutoTrueFalse> _useRenderTargets = AutoTrueFalse.Auto;

		/// <summary>
		/// Enables the deferred shading. Limited devices (mobile) are not support deferred shading.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> DeferredShading
		{
			get { if( _deferredShading.BeginGet() ) DeferredShading = _deferredShading.Get( this ); return _deferredShading.value; }
			set { if( _deferredShading.BeginSet( this, ref value ) ) { try { DeferredShadingChanged?.Invoke( this ); } finally { _deferredShading.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeferredShading"/> property value changes.</summary>
		public event Action<ReflectionProbe> DeferredShadingChanged;
		ReferenceField<AutoTrueFalse> _deferredShading = AutoTrueFalse.Auto;

		/// <summary>
		/// Whether to visualize shadows.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> Shadows
		{
			get { if( _shadows.BeginGet() ) Shadows = _shadows.Get( this ); return _shadows.value; }
			set { if( _shadows.BeginSet( this, ref value ) ) { try { ShadowsChanged?.Invoke( this ); } finally { _shadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shadows"/> property value changes.</summary>
		public event Action<ReflectionProbe> ShadowsChanged;
		ReferenceField<AutoTrueFalse> _shadows = AutoTrueFalse.Auto;

		/// <summary>
		/// Whether to visualize transparent objects.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Capture" )]
		public Reference<AutoTrueFalse> TransparentObjects
		{
			get { if( _transparentObjects.BeginGet() ) TransparentObjects = _transparentObjects.Get( this ); return _transparentObjects.value; }
			set { if( _transparentObjects.BeginSet( this, ref value ) ) { try { TransparentObjectsChanged?.Invoke( this ); } finally { _transparentObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransparentObjects"/> property value changes.</summary>
		public event Action<ReflectionProbe> TransparentObjectsChanged;
		ReferenceField<AutoTrueFalse> _transparentObjects = AutoTrueFalse.Auto;

		/// <summary>
		/// Whether to visualize meshes with skeleton animation.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Capture" )]
		public Reference<bool> AnimatedObjects
		{
			get { if( _animatedObjects.BeginGet() ) AnimatedObjects = _animatedObjects.Get( this ); return _animatedObjects.value; }
			set { if( _animatedObjects.BeginSet( this, ref value ) ) { try { AnimatedObjectsChanged?.Invoke( this ); } finally { _animatedObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnimatedObjects"/> property value changes.</summary>
		public event Action<ReflectionProbe> AnimatedObjectsChanged;
		ReferenceField<bool> _animatedObjects = true;

		/// <summary>
		/// The max amount of light sources to draw.
		/// </summary>
		[Category( "Capture" )]
		[DefaultValue( 32 )]
		[Range( 10, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> LightMaxCount
		{
			get { if( _lightMaxCount.BeginGet() ) LightMaxCount = _lightMaxCount.Get( this ); return _lightMaxCount.value; }
			set { if( _lightMaxCount.BeginSet( this, ref value ) ) { try { LightMaxCountChanged?.Invoke( this ); } finally { _lightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMaxCount"/> property value changes.</summary>
		public event Action<ReflectionProbe> LightMaxCountChanged;
		ReferenceField<int> _lightMaxCount = 32;

		/// <summary>
		/// Whether to prepare and apply light masks.
		/// </summary>
		[Category( "Capture" )]
		[DefaultValue( true )]
		public Reference<bool> LightMasks
		{
			get { if( _lightMasks.BeginGet() ) LightMasks = _lightMasks.Get( this ); return _lightMasks.value; }
			set { if( _lightMasks.BeginSet( this, ref value ) ) { try { LightMasksChanged?.Invoke( this ); } finally { _lightMasks.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMasks"/> property value changes.</summary>
		public event Action<ReflectionProbe> LightMasksChanged;
		ReferenceField<bool> _lightMasks = true;

		/// <summary>
		/// The distance multiplier when determining the level of detail.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Capture" )]
		public Reference<double> LODScale
		{
			get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
			set { if( _lODScale.BeginSet( this, ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		[DisplayName( "LOD Scale Changed" )]
		public event Action<ReflectionProbe> LODScaleChanged;
		ReferenceField<double> _lODScale = 1.0;

		/// <summary>
		/// The minimum distance of the reflection probe.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Category( "Capture" )]
		[Range( 0.01, 1 )]
		public Reference<double> NearClipPlane
		{
			get { if( _nearClipPlane.BeginGet() ) NearClipPlane = _nearClipPlane.Get( this ); return _nearClipPlane.value; }
			set { if( _nearClipPlane.BeginSet( this, ref value ) ) { try { NearClipPlaneChanged?.Invoke( this ); } finally { _nearClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NearClipPlane"/> property value changes.</summary>
		public event Action<ReflectionProbe> NearClipPlaneChanged;
		ReferenceField<double> _nearClipPlane = 0.1;

		/// <summary>
		/// The maximum distance of the reflection probe.
		/// </summary>
		[DefaultValue( 150.0 )]
		[Category( "Capture" )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> FarClipPlane
		{
			get { if( _farClipPlane.BeginGet() ) FarClipPlane = _farClipPlane.Get( this ); return _farClipPlane.value; }
			set { if( _farClipPlane.BeginSet( this, ref value ) ) { try { FarClipPlaneChanged?.Invoke( this ); } finally { _farClipPlane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FarClipPlane"/> property value changes.</summary>
		public event Action<ReflectionProbe> FarClipPlaneChanged;
		ReferenceField<double> _farClipPlane = 150;

		/// <summary>
		/// Whether to render sky of the scene.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Capture" )]
		public Reference<bool> RenderSky
		{
			get { if( _renderSky.BeginGet() ) RenderSky = _renderSky.Get( this ); return _renderSky.value; }
			set { if( _renderSky.BeginSet( this, ref value ) ) { try { RenderSkyChanged?.Invoke( this ); } finally { _renderSky.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderSky"/> property value changes.</summary>
		public event Action<ReflectionProbe> RenderSkyChanged;
		ReferenceField<bool> _renderSky = true;

		/// <summary>
		/// The amount of blur applied.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Capture" )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( this, ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<ReflectionProbe> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 1.0;

		/// <summary>
		/// The mipmap offset when fetching blurred reflection data to calculate ambient environment lighting.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -5, 2 )]
		[Category( "Capture" )]
		public Reference<double> AffectLightingMipOffset
		{
			get { if( _affectLightingMipOffset.BeginGet() ) AffectLightingMipOffset = _affectLightingMipOffset.Get( this ); return _affectLightingMipOffset.value; }
			set { if( _affectLightingMipOffset.BeginSet( this, ref value ) ) { try { AffectLightingMipOffsetChanged?.Invoke( this ); } finally { _affectLightingMipOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectLightingMipOffset"/> property value changes.</summary>
		public event Action<ReflectionProbe> AffectLightingMipOffsetChanged;
		ReferenceField<double> _affectLightingMipOffset = 0.0;

		/// <summary>
		/// Whether to position of the object in the scene by tracking camera position.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Position Depending Camera" )]
		public Reference<bool> PositionDependingCamera
		{
			get { if( _positionDependingCamera.BeginGet() ) PositionDependingCamera = _positionDependingCamera.Get( this ); return _positionDependingCamera.value; }
			set { if( _positionDependingCamera.BeginSet( this, ref value ) ) { try { PositionDependingCameraChanged?.Invoke( this ); } finally { _positionDependingCamera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionDependingCamera"/> property value changes.</summary>
		public event Action<ReflectionProbe> PositionDependingCameraChanged;
		ReferenceField<bool> _positionDependingCamera = false;

		[DefaultValue( "0 0 1" )]
		[Category( "Position Depending Camera" )]
		public Reference<Vector3> PositionDependingCameraOffset
		{
			get { if( _positionDependingCameraOffset.BeginGet() ) PositionDependingCameraOffset = _positionDependingCameraOffset.Get( this ); return _positionDependingCameraOffset.value; }
			set { if( _positionDependingCameraOffset.BeginSet( this, ref value ) ) { try { PositionDependingCameraOffsetChanged?.Invoke( this ); } finally { _positionDependingCameraOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionDependingCameraOffset"/> property value changes.</summary>
		public event Action<ReflectionProbe> PositionDependingCameraOffsetChanged;
		ReferenceField<Vector3> _positionDependingCameraOffset = new Vector3( 0, 0, 1 );

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Cubemap ):
					//case nameof( CubemapIrradiance ):
					if( Mode.Value != ModeEnum.Resource )
						skip = true;
					break;

				case nameof( RealTime ):
				case nameof( Resolution ):
				case nameof( NearClipPlane ):
				case nameof( FarClipPlane ):
				case nameof( RenderSky ):
					if( Mode.Value != ModeEnum.Capture )
						skip = true;
					break;

				case nameof( HighDynamicRange ):
				case nameof( UseRenderTargets ):
				case nameof( TransparentObjects ):
				case nameof( AnimatedObjects ):
				case nameof( LODScale ):
				case nameof( LightMaxCount ):
				case nameof( LightMasks ):
				case nameof( BlurFactor ):
				case nameof( AffectLightingMipOffset ):
					if( Mode.Value != ModeEnum.Capture || !RealTime )
						skip = true;
					break;

				case nameof( DeferredShading ):
				case nameof( Shadows ):
					if( Mode.Value != ModeEnum.Capture || UseRenderTargets.Value == AutoTrueFalse.False )
						skip = true;
					break;

				case nameof( PositionDependingCamera ):
					if( Mode.Value != ModeEnum.Capture || !RealTime )
						skip = true;
					break;

				case nameof( PositionDependingCameraOffset ):
					if( Mode.Value != ModeEnum.Capture || !PositionDependingCamera || !RealTime )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData -= Scene_GetRenderSceneData;

				if( EnabledInHierarchyAndIsInstance )
					scene.ViewportUpdateCameraSettingsReady += ParentScene_ViewportUpdateCameraSettingsReady;
				else
					scene.ViewportUpdateCameraSettingsReady -= ParentScene_ViewportUpdateCameraSettingsReady;
			}

			if( !EnabledInHierarchy )
				RealTime_DestroyRenderTarget();
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			processedCubemapNeedUpdate = true;

			ServerSendOwnedFileNameOfComponent( null );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var tr = TransformV;
			if( Global )
				newBounds = new SpaceBounds( new Sphere( tr.Position, 1 ) );
			else
				newBounds = new SpaceBounds( new Sphere( tr.Position, tr.Scale.MaxComponent() ) );
		}

		//protected override void OnTransformChanged()
		//{
		//	base.OnTransformChanged();

		//	captureTextureNeedUpdate = true;
		//}

		[Browsable( false )]
		public Sphere Sphere
		{
			get
			{
				var tr = TransformV;
				if( Global )
					return new Sphere( tr.Position, 100000 );
				else
					return new Sphere( tr.Position, tr.Scale.MaxComponent() );

				//return SpaceBounds.BoundingSphere;
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			//if( !ParentScene.GetShowDevelopmentDataInThisApplication() || !ParentScene.ShowReflectionProbes )
			//	return false;
			return base.OnEnabledSelectionByCursor();
		}

		void DebugDrawBorder( Viewport viewport )
		{
			var sphere = Sphere;
			var pos = sphere.Center;
			var r = sphere.Radius;

			const double angleStep = Math.PI / 32;
			for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
			{
				double p1sin = Math.Sin( angle );
				double p1cos = Math.Cos( angle );
				double p2sin = Math.Sin( angle + angleStep );
				double p2cos = Math.Cos( angle + angleStep );

				//может больше линий рисовать. еще под 45 градусов

				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( p1cos, p1sin, 0 ) * r ), pos + ( new Vector3( p2cos, p2sin, 0 ) * r ) );
				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( 0, p1cos, p1sin ) * r ), pos + ( new Vector3( 0, p2cos, p2sin ) * r ) );
				viewport.Simple3DRenderer.AddLine( pos + ( new Vector3( p1cos, 0, p1sin ) * r ), pos + ( new Vector3( p2cos, 0, p2sin ) * r ) );
			}
		}

		public void DebugDraw( Viewport viewport )
		{
			if( !Global )
			{
				DebugDrawBorder( viewport );

				var sphere = Sphere;
				var pos = sphere.Center;
				var r = sphere.Radius;
				viewport.Simple3DRenderer.AddLine( pos - new Vector3( r, 0, 0 ), pos + new Vector3( r, 0, 0 ) );
				viewport.Simple3DRenderer.AddLine( pos - new Vector3( 0, r, 0 ), pos + new Vector3( 0, r, 0 ) );
				viewport.Simple3DRenderer.AddLine( pos - new Vector3( 0, 0, r ), pos + new Vector3( 0, 0, r ) );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateProcessedCubemaps();
		}

		public void GetRotationQuaternion( out QuaternionF result )
		{
			QuaternionF.FromRotateByZ( Rotation.Value.InRadians().ToRadianF(), out result );
		}

		//public void GetRotationMatrix( out Matrix3F result )
		//{
		//	Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F( out result );
		//}

		void GetRenderSceneData( ViewportRenderingContext context, bool insideFrustum )
		{
			//UpdateProcessedCubemaps();

			var sphere = Sphere;
			if( sphere.Radius > 0 && Intensity.Value > 0 )
			{
				//real-time
				if( insideFrustum && RealTime )
					lastVisibleTime = Time.Current;

				var item = new RenderingPipeline.RenderSceneData.ReflectionProbeItem();
				item.Creator = this;
				item.BoundingBox = SpaceBounds.BoundingBox;
				//item.BoundingSphere = SpaceBounds.CalculatedBoundingSphere;
				item.Sphere = sphere;

				//if( Mode.Value == ModeEnum.Resource )
				//{

				if( createdImage != null && RealTime )
				{
					item.CubemapEnvironment = createdImage;
					item.CubemapEnvironmentMipmapsAndBlurRequired = (float)BlurFactor;
					item.CubemapEnvironmentAffectLightingLodOffset = (float)AffectLightingMipOffset;
				}
				else
				{
					if( processedEnvironmentCubemap != null )
						item.CubemapEnvironment = processedEnvironmentCubemap;
					else
						item.CubemapEnvironment = Cubemap;
				}

				item.HarmonicsIrradiance = processedIrradianceHarmonics;
				item.Intensity = (float)Intensity;
				//item.CubemapIrradiance = processedIrradianceCubemap;

				GetRotationQuaternion( out item.Rotation );
				item.Multiplier = Multiplier.Value.ToVector3F();

				//item.CubemapEnvironment = Cubemap;
				//item.CubemapIrradiance = CubemapIrradiance;
				//}
				//else
				//{
				//	item.CubemapEnvironment = CaptureTexture;
				//}

				context.FrameData.RenderSceneData.ReflectionProbes.Add( ref item );
			}

			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayReflectionProbes ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					if( context2.displayReflectionProbesCounter < context2.displayReflectionProbesMax )
					{
						context2.displayReflectionProbesCounter++;

						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						else
							color = ProjectSettings.Get.Colors.SceneShowReflectionProbeColor;

						var viewport = context.Owner;
						if( viewport.Simple3DRenderer != null )
						{
							viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							DebugDraw( viewport );
							//viewport.Simple3DRenderer.AddSphere( Transform.Value.ToMat4(), 0.5, 32 );
						}
					}
				}
				//if( !show )
				//	context.disableShowingLabelForThisObject = true;
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			if( mode == GetRenderSceneDataMode.InsideFrustum && !Global && context.Owner.Mode != Viewport.ModeEnum.ReflectionProbeCubemap && RenderingEffect_Reflection.GlobalMultiplier > 0 )
				GetRenderSceneData( context, mode == GetRenderSceneDataMode.InsideFrustum );
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			//!!!!don't subscribe to event for not Global

			if( VisibleInHierarchy && Global && context.Owner.Mode != Viewport.ModeEnum.ReflectionProbeCubemap && RenderingEffect_Reflection.GlobalMultiplier > 0 )
				GetRenderSceneData( context, true );
		}

		//public bool Contains( Vec3 point )
		//{
		//	var scale = Transform.Value.Scale;
		//	var radius = Math.Max( scale.X, Math.Max( scale.Y, scale.Z ) ) * 0.5;
		//	return new Sphere( Transform.Value.Position, radius ).Contains( point );
		//}

		//public virtual void GetOutputTextures( out Texture texture, out Texture textureIBL )
		//{
		//	texture = null;
		//	textureIBL = null;

		//	if( SourceType.Value == SourceTypeEnum.SpecifiedCubemap )
		//	{
		//		texture = Cubemap;
		//		textureIBL = CubemapIBL;
		//	}
		//}

		//protected override void OnDisabled()
		//{
		//	CaptureTextureDispose();
		//	base.OnDisabled();
		//}

		string GetDestVirtualFileName()
		{
			var ownedFileName = NetworkIsClient ? networkOwnedFileNameOfComponent : ComponentUtility.GetOwnedFileNameOfComponent( this );

			string name = GetPathFromRoot();
			foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
				name = name.Replace( c.ToString(), "_" );
			name = name.Replace( " ", "_" );
			return PathUtility.Combine( ownedFileName + "_Files", name + ".hdr" );
		}

		public int GetDemandedSize()
		{
			var resolution = Resolution.Value;
			var size = int.Parse( resolution.ToString().Replace( "_", "" ) );

			//apply global multiplier
			if( RenderingEffect_Reflection.GlobalMultiplier > 0 )
				size = (int)( size * RenderingEffect_Reflection.GlobalMultiplier );

			size = MathEx.Clamp( size, 1, RenderingSystem.Capabilities.MaxTextureSize );

			return size;
		}

		public bool GetHighDynamicRange()
		{
			var hdr = HighDynamicRange.Value;
			if( hdr == AutoTrueFalse.Auto )
			{
				//!!!!better take from pipeline
				hdr = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			}
			return hdr == AutoTrueFalse.True;
		}

		public void UpdateCaptureCubemap()
		{
			if( Mode.Value != ModeEnum.Capture )
				return;

			ImageComponent texture = null;
			ImageComponent textureRead = null;

			try
			{
				//create

				var hdr = GetHighDynamicRange();
				var size = GetDemandedSize();
				var format = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;

				texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = new Vector2I( size, size );
				texture.CreateMipmaps = false;
				texture.CreateFormat = format;
				texture.CreateUsage = ImageComponent.Usages.RenderTarget;
				texture.CreateFSAA = 0;
				texture.Enabled = true;

				//!!!!свой пайплайн создавать как в RealTime чтобы все свойства поддержать

				var renderTexture = texture.Result.GetRenderTarget( 0, 0 );
				//var viewport = renderTexture.AddViewport( true, false );
				var viewport = renderTexture.AddViewport( false, false );
				viewport.Mode = Viewport.ModeEnum.ReflectionProbeCubemap;
				viewport.AttachedScene = ParentScene;
				viewport.AnyData = this;

				textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				textureRead.CreateType = ImageComponent.TypeEnum._2D;
				textureRead.CreateSize = new Vector2I( size, size );
				textureRead.CreateMipmaps = false;
				textureRead.CreateFormat = format;
				textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
				textureRead.CreateFSAA = 0;
				textureRead.Enabled = true;

				//render
				//!!!!add support for not hdr format
				var image2D = new ImageUtility.Image2D( PixelFormat.Float16RGB, new Vector2I( size * 4, size * 3 ) );
				//var image2D = new ImageUtility.Image2D( hdr ? PixelFormat.Float16RGB : PixelFormat.R8G8B8, new Vector2I( size * 4, size * 3 ) );

				////var image2D = new ImageUtility.Image2D( hdr ? PixelFormat.Float32RGB : PixelFormat.R8G8B8, new Vector2I( size * 4, size * 3 ) );
				////var image2D = new ImageUtility.Image2D( PixelFormat.Float32RGB, new Vector2I( size * 4, size * 3 ) );

				var position = Transform.Value.Position;

				for( int face = 0; face < 6; face++ )
				{
					Vector3 dir = Vector3.Zero;
					Vector3 up = Vector3.Zero;

					//flipped
					switch( face )
					{
					case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
					case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
					case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
					case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
					case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
					case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
					}

					//!!!!renderingPipelineOverride

					var scene = ParentScene;
					var cameraEditor = scene.Mode.Value == Scene.ModeEnum._2D ? scene.CameraEditor2D.Value : scene.CameraEditor.Value;
					if( cameraEditor == null )
						cameraEditor = new Camera();

					var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, cameraEditor.Exposure, cameraEditor.EmissiveFactor, renderSky: RenderSky );

					viewport.Update( true, cameraSettings );

					//!!!!
					//clear temp data
					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
					viewport.RenderingContext.DynamicTexture_DestroyAll();

					texture.Result.GetNativeObject( true ).BlitTo( (ushort)RenderingSystem.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );
					//texture.Result.GetNativeObject( true ).BlitTo( (ushort)viewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetNativeObject( true ), 0, 0 );

					//get data
					var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * size * size;
					var data = new byte[ totalBytes ];
					unsafe
					{
						fixed( byte* pBytes = data )
						{
							var demandedFrame = textureRead.Result.GetNativeObject( true ).Read( (IntPtr)pBytes, 0 );
							while( RenderingSystem.CallFrame() < demandedFrame ) { }
						}
					}

					Vector2I index = Vector2I.Zero;
					switch( face )
					{
					case 1: index = new Vector2I( 2, 1 ); break;
					case 0: index = new Vector2I( 0, 1 ); break;
					case 2: index = new Vector2I( 1, 0 ); break;
					case 3: index = new Vector2I( 1, 2 ); break;
					case 4: index = new Vector2I( 1, 1 ); break;
					case 5: index = new Vector2I( 3, 1 ); break;
					}
					//switch( face )
					//{
					//case 0: index = new Vector2I( 2, 1 ); break;
					//case 1: index = new Vector2I( 0, 1 ); break;
					//case 2: index = new Vector2I( 1, 0 ); break;
					//case 3: index = new Vector2I( 1, 2 ); break;
					//case 4: index = new Vector2I( 1, 1 ); break;
					//case 5: index = new Vector2I( 3, 1 ); break;
					//}

					var faceImage = new ImageUtility.Image2D( format, new Vector2I( size, size ), data );

					//flip by X
					var faceImageFlip = new ImageUtility.Image2D( format, new Vector2I( size, size ) );
					for( int y = 0; y < size; y++ )
					{
						for( int x = 0; x < size; x++ )
						{
							var pixel = faceImage.GetPixel( new Vector2I( x, y ) );
							faceImageFlip.SetPixel( new Vector2I( size - 1 - x, y ), pixel );
						}
					}

					image2D.Blit( index * size, faceImageFlip );
				}

				//reset alpha channel
				for( int y = 0; y < image2D.Size.Y; y++ )
				{
					for( int x = 0; x < image2D.Size.X; x++ )
					{
						var pixel = image2D.GetPixel( new Vector2I( x, y ) );
						pixel.W = 1.0f;
						image2D.SetPixel( new Vector2I( x, y ), pixel );
					}
				}

				var destRealFileName = VirtualPathUtility.GetRealPathByVirtual( GetDestVirtualFileName() );

				if( !Directory.Exists( Path.GetDirectoryName( destRealFileName ) ) )
					Directory.CreateDirectory( Path.GetDirectoryName( destRealFileName ) );

				if( !ImageUtility.Save( destRealFileName, image2D.Data, image2D.Size, 1, image2D.Format, 1, 0, out var error ) )
					throw new Exception( error );

				//delete Gen files
				var names = new string[] { "_Gen.info", "_GenEnv.dds", "_GenIrr.dds" };
				foreach( var name in names )
				{
					var fileName2 = VirtualPathUtility.GetRealPathByVirtual( destRealFileName + name );
					if( File.Exists( fileName2 ) )
						File.Delete( fileName2 );
				}

			}
			catch( Exception e )
			{
				Log.Error( e.Message );
			}
			finally
			{
				texture?.Dispose();
				textureRead?.Dispose();
			}

			processedCubemapNeedUpdate = true;
		}

		//public void Update( bool forceUpdate )
		//{
		//	CaptureTextureUpdate();

		//	if( CaptureTexture != null && ( Realtime || captureTextureNeedUpdate || forceUpdate ) )
		//	{
		//		captureTextureNeedUpdate = false;

		//		var position = Transform.Value.Position;

		//		for( int face = 0; face < 6; face++ )
		//		{
		//			int mip = 0;
		//			var renderTexture = CaptureTexture.Result.GetRenderTarget( mip, face );
		//			if( renderTexture != null )
		//			{
		//				var viewport = renderTexture.Viewports[ 0 ];

		//				Vector3 dir = Vector3.Zero;
		//				Vector3 up = Vector3.Zero;

		//				//flipped
		//				switch( face )
		//				{
		//				case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
		//				case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
		//				case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
		//				case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
		//				case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
		//				case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
		//				}

		//				//!!!!
		//				var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, 1, 1 );

		//				viewport.Update( true, cameraSettings );

		//				//clear temp data
		//				if( !Realtime.Value )
		//				{
		//					viewport.RenderingContext.MultiRenderTarget_DestroyAll();
		//					viewport.RenderingContext.RenderTarget_DestroyAll();
		//				}
		//			}
		//		}
		//	}
		//}

		void UpdateProcessedCubemaps()
		{
			if( processedEnvironmentCubemap != null && processedEnvironmentCubemap.Disposed )
			{
				processedEnvironmentCubemap = null;
				processedCubemapNeedUpdate = true;
			}
			//if( processedIrradianceCubemap != null && processedIrradianceCubemap.Disposed )
			//{
			//	processedIrradianceCubemap = null;
			//	processedCubemapNeedUpdate = true;
			//}

			if( processedCubemapNeedUpdate )//&& AllowProcessEnvironmentCubemap )
			{
				processedCubemapNeedUpdate = false;
				ProcessCubemaps();
			}
		}

		void ProcessCubemaps()
		{
			processedEnvironmentCubemap = null;
			processedIrradianceHarmonics = null;
			//processedIrradianceCubemap = null;

			if( RealTime )
				return;

			string sourceFileName;
			if( Mode.Value == ModeEnum.Resource )
			{
				sourceFileName = Cubemap.Value?.LoadFile.Value?.ResourceName;
				if( string.IsNullOrEmpty( sourceFileName ) )
				{
					var getByReference = Cubemap.GetByReference;
					if( !string.IsNullOrEmpty( getByReference ) )
					{
						try
						{
							if( Path.GetExtension( getByReference ) == ".image" )
								sourceFileName = getByReference;
						}
						catch { }
					}
				}
			}
			else
			{
				sourceFileName = GetDestVirtualFileName();
			}

			if( !string.IsNullOrEmpty( sourceFileName ) && VirtualFile.Exists( sourceFileName ) )
			{
				bool skip = false;
				if( sourceFileName.Length > 11 )
				{
					var s = sourceFileName.Substring( sourceFileName.Length - 11 );
					if( s == "_GenEnv.dds" || s == "_GenIrr.dds" )
						skip = true;
				}

				if( !skip )
				{
					if( !CubemapProcessing.GetOrGenerate( sourceFileName, false, 0, out var envVirtualFileName, out var irradiance/*irrVirtualFileName*/, out var error ) )
					{
						Log.Error( error );
						return;
					}

					if( VirtualFile.Exists( envVirtualFileName ) )
						processedEnvironmentCubemap = ResourceManager.LoadResource<ImageComponent>( envVirtualFileName );
					processedIrradianceHarmonics = irradiance;
					//if( VirtualFile.Exists( irrVirtualFileName ) )
					//	processedIrradianceCubemap = ResourceManager.LoadResource<ImageComponent>( irrVirtualFileName );
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ReflectionProbe" );
		}

		void ServerSendOwnedFileNameOfComponent( ServerNetworkService_Components.ClientItem client )
		{
			if( NetworkIsServer )
			{
				var writer = client != null ? BeginNetworkMessage( client, "OwnedFileName" ) : BeginNetworkMessageToEveryone( "OwnedFileName" );
				writer.Write( ComponentUtility.GetOwnedFileNameOfComponent( this ) );
				EndNetworkMessage();
			}
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			ServerSendOwnedFileNameOfComponent( client );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "OwnedFileName" )
			{
				networkOwnedFileNameOfComponent = reader.ReadString();
				processedCubemapNeedUpdate = true;
			}

			return true;
		}


		void RealTime_CreateRenderTarget()
		{
			RealTime_DestroyRenderTarget();

			var size = GetDemandedSize();
			var hdr = GetHighDynamicRange();
			var mipmaps = false;

			createdImage = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

			createdImage.CreateType = ImageComponent.TypeEnum.Cube;
			createdImage.CreateSize = new Vector2I( size, size );
			createdImage.CreateMipmaps = mipmaps;
			createdImage.CreateFormat = hdr ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;

			var usage = ImageComponent.Usages.RenderTarget;
			if( mipmaps )
				usage |= ImageComponent.Usages.AutoMipmaps;
			createdImage.CreateUsage = usage;
			createdImage.Enabled = true;

			createdViewports = new Viewport[ 6 ];

			for( var face = 0; face < 6; face++ )
			{
				var renderTexture = createdImage.Result.GetRenderTarget( slice: face );

				var createdViewport = renderTexture.AddViewport( false, false );// true, true );
				createdViewport.Mode = Viewport.ModeEnum.ReflectionProbeCubemap;
				createdViewport.AttachedScene = ParentScene;
				createdViewport.AnyData = this;

				//!!!!check android
				//PC: OriginBottomLeft = false, Android: OriginBottomLeft = true
				createdViewport.OutputFlipY = RenderingSystem.Capabilities.OriginBottomLeft;

				createdViewports[ face ] = createdViewport;
			}

			createdForSize = size;
			createdForHDR = hdr;

			createdImageAtLeastOneTimeUpdated = false;
		}

		void RealTime_RecreateRenderTargetIfNeeded()
		{
			if( !RenderingSystem.BackendNull )
			{
				if( createdImage == null || createdForSize != GetDemandedSize() || createdForHDR != GetHighDynamicRange() )
					RealTime_CreateRenderTarget();
			}
		}

		//!!!!вызывать когда не нужен. когда не показывается
		void RealTime_DestroyRenderTarget()
		{
			if( createdImage != null )
			{
				createdImage.Dispose();
				createdImage = null;
				createdViewports = null;
				createdImageAtLeastOneTimeUpdated = false;
			}

			createdRenderingPipeline?.Dispose();
			createdRenderingPipeline = null;
		}

		public virtual Vector3 GetRealTimeCameraPosition( /*Scene scene, */Viewport viewport )//, out Vector3 result )
		{
			Vector3 result;

			if( PositionDependingCamera )
			{
				var cameraSettings = viewport.CameraSettings;
				if( cameraSettings != null )
				{
					var gameMode = ParentScene?.GetGameMode();
					if( gameMode != null )
						result = gameMode.GetBestGlobalReflectionProbePosition( viewport );
					else
						result = cameraSettings.Position;
				}
				else
					result = Transform.Value.Position;

				result += PositionDependingCameraOffset;
			}
			else
				result = Transform.Value.Position;

			return result;
		}

		//public delegate void RenderTargetUpdateBeforeEventDelegate( ReflectionProbe sender );
		//public event RenderTargetUpdateBeforeEventDelegate RenderTargetUpdateBeforeEvent;

		void RealTime_RenderTargetUpdate( Viewport viewport )
		{
			if( !viewportDuringUpdate )
			{
				RealTime_RecreateRenderTargetIfNeeded();

				if( createdViewports != null && ( RenderingEffect_Reflection.GlobalMultiplier > 0 || !createdImageAtLeastOneTimeUpdated ) )
				{
					try
					{
						viewportDuringUpdate = true;

						//need?
						//RenderTargetUpdateBeforeEvent?.Invoke( this );

						var scene = ParentScene;

						//!!!!apply overriding pipeline by camera
						var sourcePipeline = scene.RenderingPipeline.Value as RenderingPipeline_Basic;
						if( sourcePipeline == null )
							sourcePipeline = new RenderingPipeline_Basic();

						//using separate viewports because need smooth update allocated render targets between frames
						//////use only first face rendering context to optimize resource management
						////var firstViewport = createdImage.Result.GetRenderTarget( 0 ).Viewports[ 0 ];

						if( createdRenderingPipeline == null )
						{
							createdRenderingPipeline = ComponentUtility.CreateComponent<RenderingPipeline_Basic>( null, true, false );
							createdRenderingPipeline.RemoveAllComponents( false );
							createdRenderingPipeline.Enabled = true;
						}

						var pipeline = createdRenderingPipeline;


						//update pipeline properties

						var useRenderTargets = UseRenderTargets.Value;
						if( useRenderTargets == AutoTrueFalse.Auto )
							useRenderTargets = sourcePipeline.UseRenderTargets ? AutoTrueFalse.True : AutoTrueFalse.False;
						pipeline.UseRenderTargets = useRenderTargets == AutoTrueFalse.True;

						pipeline.LODScale = sourcePipeline.LODScale * LODScale;
						pipeline.LODScaleShadows = sourcePipeline.LODScaleShadows;

						var deferredShading = DeferredShading.Value;
						if( deferredShading == AutoTrueFalse.Auto )
							deferredShading = sourcePipeline.DeferredShading;
						pipeline.DeferredShading = deferredShading;

						//не совсем так в идеале, потому что HDR раньше нужно знать
						pipeline.HighDynamicRange = HighDynamicRange;

						pipeline.MinimumVisibleSizeOfObjects = sourcePipeline.MinimumVisibleSizeOfObjects * 4;


						var shadows = Shadows.Value;
						if( shadows == AutoTrueFalse.Auto )
						{
							if( sourcePipeline.Shadows )
								shadows = RealTime.Value ? AutoTrueFalse.False : AutoTrueFalse.True;
							else
								shadows = AutoTrueFalse.False;
						}
						pipeline.Shadows = shadows == AutoTrueFalse.True;

						//!!!!может другие опции теней задавать


						var transparentObjects = TransparentObjects.Value;
						if( transparentObjects == AutoTrueFalse.Auto )
							transparentObjects = RealTime.Value ? AutoTrueFalse.False : AutoTrueFalse.True;
						pipeline.DebugDrawForwardTransparentPass = transparentObjects == AutoTrueFalse.True;


						//!!!!про окклюдеры
						pipeline.OcclusionCullingBufferScene = false;
						pipeline.OcclusionCullingBufferDirectionalLight = false;

						pipeline.LightMaxDistance = Math.Min( sourcePipeline.LightMaxDistance, FarClipPlane );
						pipeline.LightMaxCount = Math.Min( sourcePipeline.LightMaxCount, LightMaxCount );

						//light grid is disabled because currently the light grid will be created for each face.
						//need to support sharing one between all faces passes and screen pass
						pipeline.LightGrid = AutoTrueFalse.False;
						//pipeline.LightGridResolution = RenderingPipeline_Basic.LightGridResolutionEnum._128;

						//add to properties?
						pipeline.DisplacementMappingMaxSteps = sourcePipeline.DisplacementMappingMaxSteps / 2;
						pipeline.RemoveTextureTiling = 0;
						pipeline.ProvideColorDepthTextureCopy = AutoTrueFalse.False;
						pipeline.TessellationQuality = 0;

						//for better instancing, less amount of dips
						pipeline.SectorsByDistance = Math.Min( sourcePipeline.SectorsByDistance, 2 );


						//update faces

						var position = GetRealTimeCameraPosition( viewport );

						for( int face = 0; face < 6; face++ )
						{
							Vector3 dir = Vector3.Zero;
							Vector3 up = Vector3.Zero;

							//flipped
							switch( face )
							{
							case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
							case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
							case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
							case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
							case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
							case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
							}


							var faceViewport = createdImage.Result.GetRenderTarget( slice: face ).Viewports[ 0 ];


							Viewport.CameraSettingsClass cameraSettings;

							if( viewport.CameraSettings != null )
							{
								var sourceCamera = viewport.CameraSettings;

								cameraSettings = new Viewport.CameraSettingsClass( faceViewport/*firstViewport*/, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, sourceCamera.Exposure, sourceCamera.EmissiveFactor, renderSky: RenderSky, renderingPipelineOverride: pipeline );
							}
							else
							{
								var cameraEditor = scene.Mode.Value == Scene.ModeEnum._2D ? scene.CameraEditor2D.Value : scene.CameraEditor.Value;
								if( cameraEditor == null )
									cameraEditor = new Camera();

								cameraSettings = new Viewport.CameraSettingsClass( faceViewport/*firstViewport*/, 1, 90, NearClipPlane.Value, FarClipPlane.Value, position, dir, up, ProjectionType.Perspective, 1, cameraEditor.Exposure, cameraEditor.EmissiveFactor, renderSky: RenderSky, renderingPipelineOverride: pipeline );
							}


							//createdViewport.BackgroundColorDefault = BackgroundColor;


							faceViewport.CameraSettings = cameraSettings;

							faceViewport.Update( false, cameraSettings );
							viewport.RenderingContext.UpdateStatisticsCurrent.AddFrom( faceViewport.RenderingContext.UpdateStatisticsCurrent );
							//faceViewport.Update( false, cameraSettings, viewport.RenderingContext.CurrentViewNumber );
							//viewport.RenderingContext.CurrentViewNumber = faceViewport.RenderingContext.CurrentViewNumber;
							//viewport.RenderingContext.UpdateStatisticsCurrent.AddFrom( faceViewport.RenderingContext.UpdateStatisticsCurrent );

							////faceViewport/*firstViewport*/.Update( true, cameraSettings );
						}

						createdImageAtLeastOneTimeUpdated = true;
					}
					finally
					{
						viewportDuringUpdate = false;
					}
				}
			}
		}

		private void ParentScene_ViewportUpdateCameraSettingsReady( Scene scene, Viewport viewport )
		{
			//real-time
			if( EnabledInHierarchyAndIsInstance && RealTime && ( viewport.LastUpdateTime == lastVisibleTime || viewport.PreviousUpdateTime == lastVisibleTime ) && viewport.Mode == Viewport.ModeEnum.Default )
			{
				RealTime_RenderTargetUpdate( viewport );
			}
		}
	}
}
