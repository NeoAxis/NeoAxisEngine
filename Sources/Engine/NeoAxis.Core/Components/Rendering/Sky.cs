// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using Internal.SharpBgfx;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A basic sky component in the engine.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.SkySettingsCell" )]
	[EditorControl( "NeoAxis.Editor.SkyEditor" )]
	[Preview( "NeoAxis.Editor.SkyPreview" )]
	[PreviewImage( "NeoAxis.Editor.SkyPreviewImage" )]
	[WhenCreatingShowWarningIfItAlreadyExists]
#endif
	public class Sky : Component
	{
		static GpuMaterialPass materialPassCube;
		static GpuMaterialPass materialPass2D;

		static Mesh mesh;
		//Image createdCubemap;
		//bool createdCubemapNeedUpdate = true;

		bool processedCubemapNeedUpdate = true;
		ImageComponent processedEnvironmentCubemap;
		Vector4F[] processedIrradianceHarmonics;
		//ImageComponent processedIrradianceCubemap;

		ImageComponent proceduralTexture;
		int proceduralTextureSize;
		PixelFormat proceduralTextureFormat;
		//Vector4F[] proceduralHarmonics;

		int proceduralUpdateCurrentItem;
		double proceduralLastUpdateTime;

		//[Browsable( false )]
		//internal bool ReflectionCubemapGeneration { get; set; } = false;

		///////////////////////////////////////////////

		public enum ModeEnum
		{
			Resource,
			Procedural,
			Mixed,
		}

		/// <summary>
		/// The way to visualize sky.
		/// </summary>
		[DefaultValue( ModeEnum.Resource )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( this, ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Sky> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Resource;

		/// <summary>
		/// The mixing factor for Mixed mode.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> MixedModeProceduralFactor
		{
			get { if( _mixedModeProceduralFactor.BeginGet() ) MixedModeProceduralFactor = _mixedModeProceduralFactor.Get( this ); return _mixedModeProceduralFactor.value; }
			set { if( _mixedModeProceduralFactor.BeginSet( this, ref value ) ) { try { MixedModeProceduralFactorChanged?.Invoke( this ); } finally { _mixedModeProceduralFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MixedModeProceduralFactor"/> property value changes.</summary>
		public event Action<Sky> MixedModeProceduralFactorChanged;
		ReferenceField<double> _mixedModeProceduralFactor = 0.5;

		///// <summary>
		///// The transparency level of the procedural mode.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "Procedural" )]
		//public Reference<double> ProceduralIntensity
		//{
		//	get { if( _proceduralIntensity.BeginGet() ) ProceduralIntensity = _proceduralIntensity.Get( this ); return _proceduralIntensity.value; }
		//	set { if( _proceduralIntensity.BeginSet( this, ref value ) ) { try { ProceduralIntensityChanged?.Invoke( this ); } finally { _proceduralIntensity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ProceduralIntensity"/> property value changes.</summary>
		//public event Action<Sky> ProceduralIntensityChanged;
		//ReferenceField<double> _proceduralIntensity = 0.0;

		[DefaultValue( "0.6 0.7 1" )]
		[ColorValueNoAlpha]
		[Category( "Procedural" )]
		public Reference<ColorValuePowered> ProceduralLuminance
		{
			get { if( _proceduralLuminance.BeginGet() ) ProceduralLuminance = _proceduralLuminance.Get( this ); return _proceduralLuminance.value; }
			set { if( _proceduralLuminance.BeginSet( this, ref value ) ) { try { ProceduralLuminanceChanged?.Invoke( this ); } finally { _proceduralLuminance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralLuminance"/> property value changes.</summary>
		public event Action<Sky> ProceduralLuminanceChanged;
		ReferenceField<ColorValuePowered> _proceduralLuminance = new ColorValuePowered( 0.6, 0.7, 1 );

		[DefaultValue( true )]
		[Category( "Procedural" )]
		public Reference<bool> ProceduralApplySunPower
		{
			get { if( _proceduralApplySunPower.BeginGet() ) ProceduralApplySunPower = _proceduralApplySunPower.Get( this ); return _proceduralApplySunPower.value; }
			set { if( _proceduralApplySunPower.BeginSet( this, ref value ) ) { try { ProceduralApplySunPowerChanged?.Invoke( this ); } finally { _proceduralApplySunPower.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralApplySunPower"/> property value changes.</summary>
		public event Action<Sky> ProceduralApplySunPowerChanged;
		ReferenceField<bool> _proceduralApplySunPower = true;

		[DefaultValue( 2.15 )]
		[Range( 1.9, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Procedural" )]
		public Reference<double> ProceduralTurbidity
		{
			get { if( _proceduralTurbidity.BeginGet() ) ProceduralTurbidity = _proceduralTurbidity.Get( this ); return _proceduralTurbidity.value; }
			set { if( _proceduralTurbidity.BeginSet( this, ref value ) ) { try { ProceduralTurbidityChanged?.Invoke( this ); } finally { _proceduralTurbidity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralTurbidity"/> property value changes.</summary>
		public event Action<Sky> ProceduralTurbidityChanged;
		ReferenceField<double> _proceduralTurbidity = 2.15;

		[DefaultValue( 0.02 )]
		[Category( "Procedural" )]
		[Range( 0.0, 0.1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ProceduralSunSize
		{
			get { if( _proceduralSunSize.BeginGet() ) ProceduralSunSize = _proceduralSunSize.Get( this ); return _proceduralSunSize.value; }
			set { if( _proceduralSunSize.BeginSet( this, ref value ) ) { try { ProceduralSunSizeChanged?.Invoke( this ); } finally { _proceduralSunSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralSunSize"/> property value changes.</summary>
		public event Action<Sky> ProceduralSunSizeChanged;
		ReferenceField<double> _proceduralSunSize = 0.02;

		[DefaultValue( 3.0 )]
		[Category( "Procedural" )]
		[Range( 0.0, 10 )]
		public Reference<double> ProceduralSunBloom
		{
			get { if( _proceduralSunBloom.BeginGet() ) ProceduralSunBloom = _proceduralSunBloom.Get( this ); return _proceduralSunBloom.value; }
			set { if( _proceduralSunBloom.BeginSet( this, ref value ) ) { try { ProceduralSunBloomChanged?.Invoke( this ); } finally { _proceduralSunBloom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralSunBloom"/> property value changes.</summary>
		public event Action<Sky> ProceduralSunBloomChanged;
		ReferenceField<double> _proceduralSunBloom = 3.0;

		[DefaultValue( 0.1 )]
		[Category( "Procedural" )]
		[Range( 0.0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ProceduralExposition
		{
			get { if( _proceduralExposition.BeginGet() ) ProceduralExposition = _proceduralExposition.Get( this ); return _proceduralExposition.value; }
			set { if( _proceduralExposition.BeginSet( this, ref value ) ) { try { ProceduralExpositionChanged?.Invoke( this ); } finally { _proceduralExposition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralExposition"/> property value changes.</summary>
		public event Action<Sky> ProceduralExpositionChanged;
		ReferenceField<double> _proceduralExposition = 0.1;

		[DefaultValue( true )]
		[Category( "Procedural" )]
		public Reference<bool> ProceduralPreventBanding
		{
			get { if( _proceduralPreventBanding.BeginGet() ) ProceduralPreventBanding = _proceduralPreventBanding.Get( this ); return _proceduralPreventBanding.value; }
			set { if( _proceduralPreventBanding.BeginSet( this, ref value ) ) { try { ProceduralPreventBandingChanged?.Invoke( this ); } finally { _proceduralPreventBanding.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralPreventBanding"/> property value changes.</summary>
		public event Action<Sky> ProceduralPreventBandingChanged;
		ReferenceField<bool> _proceduralPreventBanding = true;

		///// <summary>
		///// Whether to affect to ambient lighting.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//[Category( "Procedural" )]
		//public Reference<double> ProceduralAffectLighting
		//{
		//	get { if( _proceduralAffectLighting.BeginGet() ) ProceduralAffectLighting = _proceduralAffectLighting.Get( this ); return _proceduralAffectLighting.value; }
		//	set { if( _proceduralAffectLighting.BeginSet( this, ref value ) ) { try { ProceduralAffectLightingChanged?.Invoke( this ); } finally { _proceduralAffectLighting.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ProceduralAffectLighting"/> property value changes.</summary>
		//public event Action<Sky> ProceduralAffectLightingChanged;
		//ReferenceField<double> _proceduralAffectLighting = 1.0;

		public enum ResolutionEnum
		{
			_64,
			_128,
			_256,
			_512,
			_1024,
			_2048,
			_4096,
			//_8192,
			//_16384,//!!!!как проверять хватит ли памяти
		}

		/// <summary>
		/// The resolution of the texture for the procedural mode.
		/// </summary>
		[DefaultValue( ResolutionEnum._512 )]
		[Category( "Procedural" )]
		public Reference<ResolutionEnum> ProceduralResolution
		{
			get { if( _proceduralResolution.BeginGet() ) ProceduralResolution = _proceduralResolution.Get( this ); return _proceduralResolution.value; }
			set { if( _proceduralResolution.BeginSet( this, ref value ) ) { try { ProceduralResolutionChanged?.Invoke( this ); } finally { _proceduralResolution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralResolution"/> property value changes.</summary>
		public event Action<Sky> ProceduralResolutionChanged;
		ReferenceField<ResolutionEnum> _proceduralResolution = ResolutionEnum._512;

		///// <summary>
		///// The time between procedural mode updates in seconds.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Category( "Procedural" )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> ProceduralUpdateTime
		//{
		//	get { if( _proceduralUpdateTime.BeginGet() ) ProceduralUpdateTime = _proceduralUpdateTime.Get( this ); return _proceduralUpdateTime.value; }
		//	set { if( _proceduralUpdateTime.BeginSet( this, ref value ) ) { try { ProceduralUpdateTimeChanged?.Invoke( this ); } finally { _proceduralUpdateTime.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ProceduralUpdateTime"/> property value changes.</summary>
		//public event Action<Sky> ProceduralUpdateTimeChanged;
		//ReferenceField<double> _proceduralUpdateTime = 1.0;

		//[DefaultValue( 50.0 )]
		//[Range( -90.0, 90.0 )]
		//[Category( "Procedural" )]
		//public Reference<double> Latitude
		//{
		//	get { if( _latitude.BeginGet() ) Latitude = _latitude.Get( this ); return _latitude.value; }
		//	set { if( _latitude.BeginSet( this, ref value ) ) { try { LatitudeChanged?.Invoke( this ); } finally { _latitude.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Latitude"/> property value changes.</summary>
		//public event Action<Sky> LatitudeChanged;
		//ReferenceField<double> _latitude = 50.0;

		//[DefaultValue( 12.0 )]
		//[Range( 0.0, 24.0 )]
		//[Category( "Procedural" )]
		//public Reference<double> Time
		//{
		//	get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
		//	set { if( _time.BeginSet( this, ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		//public event Action<Sky> TimeChanged;
		//ReferenceField<double> _time = 12.0;

		//[DefaultValue( 6.0 )]
		//[Range( 0.0, 12.0 )]
		//[Category( "Procedural" )]
		//public Reference<double> Month
		//{
		//	get { if( _month.BeginGet() ) Month = _month.Get( this ); return _month.value; }
		//	set { if( _month.BeginSet( this, ref value ) ) { try { MonthChanged?.Invoke( this ); } finally { _month.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Month"/> property value changes.</summary>
		//public event Action<Sky> MonthChanged;
		//ReferenceField<double> _month = 6.0;

		///////////////////////////////////////////////

		/// <summary>
		/// The texture used by the skybox.
		/// </summary>
		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]//[DefaultValue( null )]
		[Category( "Cubemap" )]
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
						//createdCubemapNeedUpdate = true;
						processedCubemapNeedUpdate = true;
					}
					finally { _cubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Cubemap"/> property value changes.</summary>
		public event Action<Sky> CubemapChanged;
		ReferenceField<ImageComponent> _cubemap = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		///// <summary>
		///// Positive X side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap X+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveX
		//{
		//	get { if( _cubemapPositiveX.BeginGet() ) CubemapPositiveX = _cubemapPositiveX.Get( this ); return _cubemapPositiveX.value; }
		//	set
		//	{
		//		if( _cubemapPositiveX.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveXChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveX.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapPositiveXChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveX;

		///// <summary>
		///// Negative X side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap X-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeX
		//{
		//	get { if( _cubemapNegativeX.BeginGet() ) CubemapNegativeX = _cubemapNegativeX.Get( this ); return _cubemapNegativeX.value; }
		//	set
		//	{
		//		if( _cubemapNegativeX.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeXChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeX.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapNegativeXChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeX;

		///// <summary>
		///// Positive Y side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Y+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveY
		//{
		//	get { if( _cubemapPositiveY.BeginGet() ) CubemapPositiveY = _cubemapPositiveY.Get( this ); return _cubemapPositiveY.value; }
		//	set
		//	{
		//		if( _cubemapPositiveY.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveYChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveY.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapPositiveYChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveY;

		///// <summary>
		///// Negative Y side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Y-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeY
		//{
		//	get { if( _cubemapNegativeY.BeginGet() ) CubemapNegativeY = _cubemapNegativeY.Get( this ); return _cubemapNegativeY.value; }
		//	set
		//	{
		//		if( _cubemapNegativeY.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeYChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeY.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapNegativeYChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeY;

		///// <summary>
		///// Positive Z side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Z+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveZ
		//{
		//	get { if( _cubemapPositiveZ.BeginGet() ) CubemapPositiveZ = _cubemapPositiveZ.Get( this ); return _cubemapPositiveZ.value; }
		//	set
		//	{
		//		if( _cubemapPositiveZ.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveZChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveZ.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapPositiveZChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveZ;

		///// <summary>
		///// Negative Z side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Z-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeZ
		//{
		//	get { if( _cubemapNegativeZ.BeginGet() ) CubemapNegativeZ = _cubemapNegativeZ.Get( this ); return _cubemapNegativeZ.value; }
		//	set
		//	{
		//		if( _cubemapNegativeZ.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeZChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeZ.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Sky> CubemapNegativeZChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeZ;

		///// <summary>
		///// The irradiance map displays somewhat like an average color or lighting display of the environment.
		///// </summary>
		//[DefaultValue( null )]
		//public Reference<Image> CubemapIrradiance
		//{
		//	get { if( _cubemapIrradiance.BeginGet() ) CubemapIrradiance = _cubemapIrradiance.Get( this ); return _cubemapIrradiance.value; }
		//	set { if( _cubemapIrradiance.BeginSet( this, ref value ) ) { try { CubemapIrradianceChanged?.Invoke( this ); } finally { _cubemapIrradiance.EndSet(); } } }
		//}
		//public event Action<Skybox> CubemapIrradianceChanged;
		//ReferenceField<Image> _cubemapIrradiance;

		/// <summary>
		/// The horizontal rotation of the skybox.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		[Category( "Cubemap" )]
		public Reference<Degree> CubemapRotation
		{
			get { if( _cubemapRotation.BeginGet() ) CubemapRotation = _cubemapRotation.Get( this ); return _cubemapRotation.value; }
			set { if( _cubemapRotation.BeginSet( this, ref value ) ) { try { CubemapRotationChanged?.Invoke( this ); } finally { _cubemapRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CubemapRotation"/> property value changes.</summary>
		public event Action<Sky> CubemapRotationChanged;
		ReferenceField<Degree> _cubemapRotation;

		/// <summary>
		/// Vertical stretch multiplier.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.8, 1.2 )]
		[Category( "Cubemap" )]
		public Reference<double> CubemapStretch
		{
			get { if( _cubemapStretch.BeginGet() ) CubemapStretch = _cubemapStretch.Get( this ); return _cubemapStretch.value; }
			set { if( _cubemapStretch.BeginSet( this, ref value ) ) { try { CubemapStretchChanged?.Invoke( this ); } finally { _cubemapStretch.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CubemapStretch"/> property value changes.</summary>
		public event Action<Sky> CubemapStretchChanged;
		ReferenceField<double> _cubemapStretch = 1.0;

		/// <summary>
		/// A skybox color multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		[Category( "Cubemap" )]
		public Reference<ColorValuePowered> CubemapMultiplier
		{
			get { if( _cubemapMultiplier.BeginGet() ) CubemapMultiplier = _cubemapMultiplier.Get( this ); return _cubemapMultiplier.value; }
			set { if( _cubemapMultiplier.BeginSet( this, ref value ) ) { try { CubemapMultiplierChanged?.Invoke( this ); } finally { _cubemapMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CubemapMultiplier"/> property value changes.</summary>
		public event Action<Sky> CubemapMultiplierChanged;
		ReferenceField<ColorValuePowered> _cubemapMultiplier = new ColorValuePowered( 1, 1, 1 );

		///// <summary>
		///// Whether to affect to ambient lighting.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//[Category( "Cubemap" )]
		//public Reference<double> CubemapAffectLighting
		//{
		//	get { if( _cubemapAffectLighting.BeginGet() ) CubemapAffectLighting = _cubemapAffectLighting.Get( this ); return _cubemapAffectLighting.value; }
		//	set { if( _cubemapAffectLighting.BeginSet( this, ref value ) ) { try { CubemapAffectLightingChanged?.Invoke( this ); } finally { _cubemapAffectLighting.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CubemapAffectLighting"/> property value changes.</summary>
		//public event Action<Sky> CubemapAffectLightingChanged;
		//ReferenceField<double> _cubemapAffectLighting = 1.0;

		/// <summary>
		/// Whether to affect to ambient lighting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Lighting" )]
		public Reference<double> AffectLighting
		{
			get { if( _affectLighting.BeginGet() ) AffectLighting = _affectLighting.Get( this ); return _affectLighting.value; }
			set { if( _affectLighting.BeginSet( this, ref value ) ) { try { AffectLightingChanged?.Invoke( this ); } finally { _affectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectLighting"/> property value changes.</summary>
		public event Action<Sky> AffectLightingChanged;
		ReferenceField<double> _affectLighting = 1.0;

		/// <summary>
		/// The texture used for the reflection. When it is null, the specified cubemap at Cubemap property is used for lighting.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Lighting" )]
		public Reference<ImageComponent> LightingCubemap
		{
			get { if( _lightingCubemap.BeginGet() ) LightingCubemap = _lightingCubemap.Get( this ); return _lightingCubemap.value; }
			set
			{
				if( _lightingCubemap.BeginSet( this, ref value ) )
				{
					try
					{
						LightingCubemapChanged?.Invoke( this );
						//createdCubemapNeedUpdate = true;
						processedCubemapNeedUpdate = true;
					}
					finally { _lightingCubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LightingCubemap"/> property value changes.</summary>
		public event Action<Sky> LightingCubemapChanged;
		ReferenceField<ImageComponent> _lightingCubemap = null;

		//!!!!
		/// <summary>
		/// The horizontal rotation of the lighting cubemap.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		[Category( "Lighting" )]
		public Reference<Degree> LightingCubemapRotation
		{
			get { if( _lightingCubemapRotation.BeginGet() ) LightingCubemapRotation = _lightingCubemapRotation.Get( this ); return _lightingCubemapRotation.value; }
			set { if( _lightingCubemapRotation.BeginSet( this, ref value ) ) { try { LightingCubemapRotationChanged?.Invoke( this ); } finally { _lightingCubemapRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightingCubemapRotation"/> property value changes.</summary>
		public event Action<Sky> LightingCubemapRotationChanged;
		ReferenceField<Degree> _lightingCubemapRotation;

		////!!!!need?
		///// <summary>
		///// A skybox color multiplier that affects ambient lighting.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[ApplicableRangeColorValuePower( 0, 4 )]
		//[ColorValueNoAlpha]
		//[Category( "Cubemap" )]
		//public Reference<ColorValuePowered> LightingMultiplier
		//{
		//	get { if( _lightingMultiplier.BeginGet() ) LightingMultiplier = _lightingMultiplier.Get( this ); return _lightingMultiplier.value; }
		//	set { if( _lightingMultiplier.BeginSet( this, ref value ) ) { try { LightingMultiplierChanged?.Invoke( this ); } finally { _lightingMultiplier.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LightingMultiplier"/> property value changes.</summary>
		//public event Action<Sky> LightingMultiplierChanged;
		//ReferenceField<ColorValuePowered> _lightingMultiplier = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// Whether to use the processed cubemap for the background instead of the original image.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( false )]
		public Reference<bool> AlwaysUseProcessedCubemap
		{
			get { if( _alwaysUseProcessedCubemap.BeginGet() ) AlwaysUseProcessedCubemap = _alwaysUseProcessedCubemap.Get( this ); return _alwaysUseProcessedCubemap.value; }
			set { if( _alwaysUseProcessedCubemap.BeginSet( this, ref value ) ) { try { AlwaysUseProcessedCubemapChanged?.Invoke( this ); } finally { _alwaysUseProcessedCubemap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysUseProcessedCubemap"/> property value changes.</summary>
		public event Action<Sky> AlwaysUseProcessedCubemapChanged;
		ReferenceField<bool> _alwaysUseProcessedCubemap = false;

		/// <summary>
		/// Whether to allow processing the specified cubemap to 6-sided cubemap.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( true )]
		public Reference<bool> AllowProcessEnvironmentCubemap
		{
			get { if( _allowProcessEnvironmentCubemap.BeginGet() ) AllowProcessEnvironmentCubemap = _allowProcessEnvironmentCubemap.Get( this ); return _allowProcessEnvironmentCubemap.value; }
			set { if( _allowProcessEnvironmentCubemap.BeginSet( this, ref value ) ) { try { AllowProcessEnvironmentCubemapChanged?.Invoke( this ); } finally { _allowProcessEnvironmentCubemap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowProcessEnvironmentCubemap"/> property value changes.</summary>
		public event Action<Sky> AllowProcessEnvironmentCubemapChanged;
		ReferenceField<bool> _allowProcessEnvironmentCubemap = true;

		///////////////////////////////////////////////

		//ReferenceValueType_Resource GetCubemapSide( int face )
		//{
		//	switch( face )
		//	{
		//	case 0: return CubemapPositiveX;
		//	case 1: return CubemapNegativeX;
		//	case 2: return CubemapPositiveY;
		//	case 3: return CubemapNegativeY;
		//	case 4: return CubemapPositiveZ;
		//	case 5: return CubemapNegativeZ;
		//	}
		//	return null;
		//}

		//bool AnyCubemapSideIsSpecified()
		//{
		//	for( int n = 0; n < 6; n++ )
		//		if( GetCubemapSide( n ) != null )
		//			return true;
		//	return false;
		//}

		//bool AllCubemapSidesAreaSpecified()
		//{
		//	for( int n = 0; n < 6; n++ )
		//		if( GetCubemapSide( n ) == null )
		//			return false;
		//	return true;
		//}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( LightingCubemap ):
					//case nameof( LightingMultiplier ):
					if( AffectLighting.Value == 0 )
						skip = true;
					break;

				case nameof( LightingCubemapRotation ):
					if( !LightingCubemap.ReferenceSpecified && LightingCubemap.Value == null )
						skip = true;
					if( AffectLighting.Value == 0 )
						skip = true;
					break;

				//case nameof( Cubemap ):
				//	if( AnyCubemapSideIsSpecified() )
				//		skip = true;
				//	break;

				//case nameof( CubemapPositiveX ):
				//case nameof( CubemapNegativeX ):
				//case nameof( CubemapPositiveY ):
				//case nameof( CubemapNegativeY ):
				//case nameof( CubemapPositiveZ ):
				//case nameof( CubemapNegativeZ ):
				//	if( Cubemap.Value != null )
				//		skip = true;
				//	break;


				//case nameof( Latitude ):
				case nameof( ProceduralTurbidity ):
				case nameof( ProceduralLuminance ):
				case nameof( ProceduralApplySunPower ):
				case nameof( ProceduralSunSize ):
				case nameof( ProceduralSunBloom ):
				case nameof( ProceduralExposition ):
				case nameof( ProceduralPreventBanding ):
				//case nameof( ProceduralAffectLighting ):
				case nameof( ProceduralResolution ):
					//case nameof( ProceduralUpdateTime ):
					//case nameof( Time ):
					//case nameof( Month ):
					if( Mode.Value == ModeEnum.Resource )
						skip = true;
					break;

				case nameof( Cubemap ):
				case nameof( CubemapRotation ):
				case nameof( CubemapStretch ):
				case nameof( CubemapMultiplier ):
					if( Mode.Value == ModeEnum.Procedural )
						skip = true;
					break;

				case nameof( MixedModeProceduralFactor ):
					if( Mode.Value != ModeEnum.Mixed )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				//rendering pipeline optimization
				if( EnabledInHierarchyAndIsInstance )
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Add( this );
				else
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Remove( this );

				if( EnabledInHierarchyAndIsInstance )
					scene.RenderAfterSetCommonUniforms += Scene_RenderAfterSetCommonUniforms;
				else
					scene.RenderAfterSetCommonUniforms -= Scene_RenderAfterSetCommonUniforms;

				//if( EnabledInHierarchyAndIsInstance )
				//	scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
				//else
				//	scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
			}

			if( !EnabledInHierarchy )
				ProceduralTextureDispose();
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Sky", true );
		}

		[Browsable( false )]
		public static Mesh Mesh
		{
			get { return mesh; }
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			if( mesh == null )
			{
				mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
				mesh.CreateComponent<MeshGeometry_Box>();
				mesh.Enabled = true;
			}

			processedCubemapNeedUpdate = true;
		}

		protected override void OnDisabled()
		{
			//mesh?.Dispose();
			//mesh = null;

			//CreatedCubemapDispose();

			base.OnDisabled();
		}

		//void CreatedCubemapUpdate()
		//{
		//	if( AllCubemapSidesAreaSpecified() )
		//	{
		//		if( createdCubemap == null || createdCubemapNeedUpdate )
		//		{
		//			CreatedCubemapDispose();

		//			//need set ShowInEditor = false before AddComponent
		//			createdCubemap = ComponentUtility.CreateComponent<Image>( null, false, false );
		//			createdCubemap.DisplayInEditor = false;
		//			AddComponent( createdCubemap );
		//			//createdCubemap = CreateComponent<Texture>( enable: false );

		//			createdCubemap.SaveSupport = false;
		//			createdCubemap.CloneSupport = false;

		//			createdCubemap.LoadCubePositiveX = GetCubemapSide( 0 );
		//			createdCubemap.LoadCubeNegativeX = GetCubemapSide( 1 );
		//			createdCubemap.LoadCubePositiveY = GetCubemapSide( 2 );
		//			createdCubemap.LoadCubeNegativeY = GetCubemapSide( 3 );
		//			createdCubemap.LoadCubePositiveZ = GetCubemapSide( 4 );
		//			createdCubemap.LoadCubeNegativeZ = GetCubemapSide( 5 );

		//			createdCubemap.Enabled = true;

		//			createdCubemapNeedUpdate = false;
		//		}
		//	}
		//	else
		//		CreatedCubemapDispose();
		//}

		//void CreatedCubemapDispose()
		//{
		//	createdCubemap?.Dispose();
		//	createdCubemap = null;
		//}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateProcessedCubemaps();
			//if( !RenderingSystem.BackendNull )
			//	UpdateProceduralCubemap();
		}

		static RenderingPipeline_Basic.LightItem GetDirectionalLight( RenderingPipeline_Basic.FrameData frameData )
		{
			foreach( var item in frameData.Lights )
			{
				if( item.data.Type == Light.TypeEnum.Directional )
					return item;
			}
			return null;
		}

		static Vector3F[] ABCDE =
		{
			new Vector3F( -0.2592f, -0.2608f, -1.4630f ),
			new Vector3F( 0.0008f,  0.0092f,  0.4275f ),
			new Vector3F( 0.2125f,  0.2102f,  5.3251f ),
			new Vector3F( -0.8989f, -1.6537f, -2.5771f ),
			new Vector3F( 0.0452f,  0.0529f,  0.3703f ),
		};

		static Vector3F[] ABCDE_t =
		{
			new Vector3F( -0.0193f, -0.0167f,  0.1787f ),
			new Vector3F( -0.0665f, -0.0950f, -0.3554f ),
			new Vector3F( -0.0004f, -0.0079f, -0.0227f ),
			new Vector3F( -0.0641f, -0.0441f,  0.1206f ),
			new Vector3F( -0.0033f, -0.0109f, -0.0670f ),
		};

		static void ComputePerezCoeff( float turbidity, Vector4F[] _outPerezCoeff )
		{
			for( int n = 0; n < 5; ++n )
			{
				var v = ABCDE_t[ n ] * turbidity + ABCDE[ n ];
				_outPerezCoeff[ n ] = new Vector4F( v.X, v.Y, v.Z, 0 );
			}
		}

		//static Dictionary<float, ColorValue> skyLuminanceXYZTable;

		//static ColorValue GetSkyLuminance( float time )
		//{
		//	if( skyLuminanceXYZTable == null )
		//	{
		//		skyLuminanceXYZTable = new Dictionary<float, ColorValue>();
		//		skyLuminanceXYZTable.Add( 0.0f, new ColorValue( 0.308f, 0.308f, 0.411f ) );
		//		skyLuminanceXYZTable.Add( 1.0f, new ColorValue( 0.308f, 0.308f, 0.410f ) );
		//		skyLuminanceXYZTable.Add( 2.0f, new ColorValue( 0.301f, 0.301f, 0.402f ) );
		//		skyLuminanceXYZTable.Add( 3.0f, new ColorValue( 0.287f, 0.287f, 0.382f ) );
		//		skyLuminanceXYZTable.Add( 4.0f, new ColorValue( 0.258f, 0.258f, 0.344f ) );
		//		skyLuminanceXYZTable.Add( 5.0f, new ColorValue( 0.258f, 0.258f, 0.344f ) );
		//		skyLuminanceXYZTable.Add( 7.0f, new ColorValue( 0.962851f, 1.000000f, 1.747835f ) );
		//		skyLuminanceXYZTable.Add( 8.0f, new ColorValue( 0.967787f, 1.000000f, 1.776762f ) );
		//		skyLuminanceXYZTable.Add( 9.0f, new ColorValue( 0.970173f, 1.000000f, 1.788413f ) );
		//		skyLuminanceXYZTable.Add( 10.0f, new ColorValue( 0.971431f, 1.000000f, 1.794102f ) );
		//		skyLuminanceXYZTable.Add( 11.0f, new ColorValue( 0.972099f, 1.000000f, 1.797096f ) );
		//		skyLuminanceXYZTable.Add( 12.0f, new ColorValue( 0.972385f, 1.000000f, 1.798389f ) );
		//		skyLuminanceXYZTable.Add( 13.0f, new ColorValue( 0.972361f, 1.000000f, 1.798278f ) );
		//		skyLuminanceXYZTable.Add( 14.0f, new ColorValue( 0.972020f, 1.000000f, 1.796740f ) );
		//		skyLuminanceXYZTable.Add( 15.0f, new ColorValue( 0.971275f, 1.000000f, 1.793407f ) );
		//		skyLuminanceXYZTable.Add( 16.0f, new ColorValue( 0.969885f, 1.000000f, 1.787078f ) );
		//		skyLuminanceXYZTable.Add( 17.0f, new ColorValue( 0.967216f, 1.000000f, 1.773758f ) );
		//		skyLuminanceXYZTable.Add( 18.0f, new ColorValue( 0.961668f, 1.000000f, 1.739891f ) );
		//		skyLuminanceXYZTable.Add( 20.0f, new ColorValue( 0.264f, 0.264f, 0.352f ) );
		//		skyLuminanceXYZTable.Add( 21.0f, new ColorValue( 0.264f, 0.264f, 0.352f ) );
		//		skyLuminanceXYZTable.Add( 22.0f, new ColorValue( 0.290f, 0.290f, 0.386f ) );
		//		skyLuminanceXYZTable.Add( 23.0f, new ColorValue( 0.303f, 0.303f, 0.404f ) );
		//	}

		//	if( skyLuminanceXYZTable.TryGetValue( (int)time, out var c ) )
		//		return c;
		//	return new ColorValue( 0, 0, 0 );
		//}

		public virtual unsafe void Render( RenderingPipeline pipeline, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData )
		{
			Render2( pipeline, context, frameData, false );
		}

		public double GetProceduralIntensity()
		{
			switch( Mode.Value )
			{
			case ModeEnum.Procedural: return 1;
			case ModeEnum.Mixed: return MixedModeProceduralFactor;
			}
			return 0;
		}

		unsafe void Render2( RenderingPipeline pipeline, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, bool reflectionCubemapGeneration )
		{
			UpdateProcessedCubemaps();

			if( mesh != null )
			{
				Matrix4F worldMatrixRelative = Matrix4F.Identity;
				//Matrix4F worldMatrix = Matrix4.FromTranslate( context.Owner.CameraSettings.Position ).ToMatrix4F();

				foreach( var item in mesh.Result.MeshData.RenderOperations )
				{
					ParameterContainer generalContainer = new ParameterContainer();
					generalContainer.Set( "u_skyCubemapMultiplier", CubemapMultiplier.Value.ToColorValue() );
					GetRotationQuaternion( out var rotation );
					generalContainer.Set( "u_skyCubemapRotation", rotation );// Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F() );
					generalContainer.Set( "u_skyCubemapStretch", (float)CubemapStretch.Value );

					generalContainer.Set( "u_skyParams1", new Vector4( GetProceduralIntensity(), ProceduralPreventBanding ? 1 : 0, 0/*reflectionCubemapGeneration ? 1 : 0*/, 0 ).ToVector4F() );
					generalContainer.Set( "u_skyParams2", new Vector4( ProceduralSunSize.Value, ProceduralSunBloom, ProceduralExposition, 0 ).ToVector4F() );

					var lightDirection = new Vector3F( 1, 0, 0 );
					var directionalLight = GetDirectionalLight( frameData );
					if( directionalLight != null )
						lightDirection = -directionalLight.data.Rotation.GetForward();

					generalContainer.Set( "u_skyLightDirection", new Vector4F( lightDirection.X, lightDirection.Y, lightDirection.Z, 0 ) );

					var perezCoeff = new Vector4F[ 5 ];
					ComputePerezCoeff( (float)ProceduralTurbidity.Value, perezCoeff );
					generalContainer.Set( "u_skyPerezCoeff", perezCoeff );

					var skyLuminance = ProceduralLuminance.Value.ToVector3();
					if( ProceduralApplySunPower && directionalLight != null )
						skyLuminance *= directionalLight.data.Power / 100000;

					var skyColorxyY = Vector3.Zero;
					var sum = skyLuminance.X + skyLuminance.Y + skyLuminance.Z;
					if( sum != 0 )
					{
						skyColorxyY = new Vector3(
							  skyLuminance.X / ( skyLuminance.X + skyLuminance.Y + skyLuminance.Z )
							, skyLuminance.Y / ( skyLuminance.X + skyLuminance.Y + skyLuminance.Z )
							, skyLuminance.Y );
					}
					generalContainer.Set( "u_skyColorxyY", new Vector4( skyColorxyY.X, skyColorxyY.Y, skyColorxyY.Z, 0 ).ToVector4F() );

					//var skyLimunance = GetSkyLuminance( (float)Time.Value );
					//generalContainer.Set( "u_skyLuminanceXYZ", new Vector4F( skyLimunance.Red, skyLimunance.Green, skyLimunance.Blue, 0 ) );


					ImageComponent tex = null;
					////!!!!hack. by idea need mirror 6-sided loaded cubemaps
					//bool flipCubemap = false;

					if( AlwaysUseProcessedCubemap )
						tex = processedEnvironmentCubemap;
					if( tex == null )
					{
						tex = Cubemap;
						//if( tex != null && tex.AnyCubemapSideIsSpecified() )
						//	flipCubemap = true;
					}
					if( tex == null )
						tex = processedEnvironmentCubemap;
					if( tex == null )
						tex = ResourceUtility.BlackTextureCube;

					//generalContainer.Set( "flipCubemap", new Vector4F( flipCubemap ? -1 : 1, 0, 0, 0 ) );

					////!!!!сделать GetResultEnvironmentCubemap()?
					////!!!!!!где еще его использовать
					//if( processedEnvironmentCubemap != null )
					//	tex = processedEnvironmentCubemap;
					////else if( AnyCubemapSideIsSpecified() )
					////	tex = createdCubemap;
					//else
					//	tex = Cubemap;
					//if( tex == null )
					//	tex = ResourceUtility.BlackTextureCube;

					if( tex.Result != null )
					{
						var cube = tex.Result.TextureType == ImageComponent.TypeEnum.Cube;

						var pass = GetMaterialPass( cube );
						if( pass != null )
						{
							TextureAddressingMode addressingMode;
							if( cube )
								addressingMode = TextureAddressingMode.Wrap;
							else
								addressingMode = TextureAddressingMode.WrapU | TextureAddressingMode.ClampV;

							context.BindTexture( 0/*"skyboxTexture"*/, tex, addressingMode, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

							var containers = new List<ParameterContainer>();
							containers.Add( generalContainer );

							Bgfx.SetTransform( (float*)&worldMatrixRelative );
							( (RenderingPipeline_Basic)pipeline ).RenderOperation( context, item, pass, containers );
						}
					}
				}

				Bgfx.Discard( DiscardFlags.All );
			}
		}

		public void GetRotationQuaternion( out QuaternionF result )
		{
			QuaternionF.FromRotateByZ( CubemapRotation.Value.InRadians().ToRadianF(), out result );
		}

		//public void GetRotationMatrix( out Matrix3F result )
		//{
		//	Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F( out result );
		//}

		public void GetLightingCubemapRotationQuaternion( out QuaternionF result )
		{
			QuaternionF.FromRotateByZ( LightingCubemapRotation.Value.InRadians().ToRadianF(), out result );
		}

		//public void GetLightingCubemapRotationMatrix( out Matrix3F result )
		//{
		//	Matrix3.FromRotateByZ( LightingCubemapRotation.Value.InRadians() ).ToMatrix3F( out result );
		//}

		static GpuMaterialPass GetMaterialPass( bool cube )
		{
			if( cube && materialPassCube == null || !cube && materialPass2D == null )
			{
				//generate compile arguments
				var generalDefines = new List<(string, string)>();

				if( !cube )
					generalDefines.Add( ("USE_2D", "") );

				//vertex program
				GpuProgram vertexProgram = GpuProgramManager.GetProgram( $"Sky_Vertex_", GpuProgramType.Vertex,
					$@"Base\Shaders\Sky_vs.sc", generalDefines, true, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( error );
					return null;
				}

				//fragment program
				GpuProgram fragmentProgram = GpuProgramManager.GetProgram( $"Sky_Fragment_", GpuProgramType.Fragment,
					$@"Base\Shaders\Sky_fs.sc", generalDefines, true, out error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( error );
					return null;
				}

				var pass = new GpuMaterialPass( null, vertexProgram, fragmentProgram );
				pass.CullingMode = CullingMode.None;
				pass.DepthCheck = true;
				pass.DepthWrite = false;

				if( cube )
					materialPassCube = pass;
				else
					materialPass2D = pass;
			}

			return cube ? materialPassCube : materialPass2D;
		}

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

			if( processedCubemapNeedUpdate && AllowProcessEnvironmentCubemap )
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

			var useCubemap = LightingCubemap;
			if( !useCubemap.ReferenceSpecified && useCubemap.Value == null )
				useCubemap = Cubemap;

			var sourceFileName = useCubemap.Value?.LoadFile.Value?.ResourceName;
			if( string.IsNullOrEmpty( sourceFileName ) )
			{
				var getByReference = useCubemap.GetByReference;
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
					if( !CubemapProcessing.GetOrGenerate( sourceFileName, false, 0, out var envVirtualFileName, out var irradiance /*irrVirtualFileName*/, out var error ) )
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

		//public void GetEnvironmentCubemaps( out Image environmentCubemap, out Image irradianceCubemap )
		//{
		//	environmentCubemap = processedEnvironmentCubemap;
		//	irradianceCubemap = processedIrradianceCubemap;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		public virtual bool GetEnvironmentTextureData( out RenderingPipeline.EnvironmentTextureData environmentCubemap, out RenderingPipeline.EnvironmentIrradianceData irradianceHarmonics )
		{
			var affect = (float)AffectLighting.Value;
			if( affect > 0 )
			{
				var proceduralIntensity = (float)GetProceduralIntensity();
				if( proceduralIntensity > 0 )
				{
					Vector4F[] proceduralHarmonics;
					if( proceduralTexture != null )
					{
						var mipOffset = 0;
						proceduralHarmonics = RenderingPipeline.EnvironmentIrradianceData.GetHarmonicsToUseMap( proceduralTexture, mipOffset );
					}
					else
						proceduralHarmonics = RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;

					Vector4F[] harmonics;
					if( proceduralIntensity < 1 )
					{
						//!!!!it is not mixed correctly. need transfer factor to shader, mix both paths in shader
						proceduralHarmonics = RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;

						var cubemapHarmonics = processedIrradianceHarmonics ?? RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;

						harmonics = new Vector4F[ 9 ];
						for( int n = 0; n < harmonics.Length; n++ )
							harmonics[ n ] = Vector4F.Lerp( cubemapHarmonics[ n ], proceduralHarmonics[ n ], proceduralIntensity );
					}
					else
						harmonics = proceduralHarmonics;

					var rotation = QuaternionF.Identity;
					var multiplier = Vector3F.One;
					environmentCubemap = new RenderingPipeline.EnvironmentTextureData( proceduralTexture ?? ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
					irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( harmonics, affect, ref rotation, ref multiplier );


					////Vector4F[] harmonics;

					////var proceduralHarmonics2 = proceduralHarmonics ?? RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;
					//////var proceduralHarmonics = RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;


					////if( proceduralIntensity < 1 )
					////{
					////	var cubemapHarmonics = processedIrradianceHarmonics ?? RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;

					////	harmonics = new Vector4F[ 9 ];
					////	for( int n = 0; n < harmonics.Length; n++ )
					////		harmonics[ n ] = Vector4F.Lerp( cubemapHarmonics[ n ], proceduralHarmonics2[ n ], proceduralIntensity );
					////}
					////else
					////	harmonics = proceduralHarmonics2;

					////var rotation = QuaternionF.Identity;
					////var multiplier = Vector3F.One;
					////environmentCubemap = new RenderingPipeline.EnvironmentTextureData( proceduralTexture ?? ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
					////irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( harmonics, affect, ref rotation, ref multiplier );
				}
				else
				{
					Vector3F multiplier;
					QuaternionF rotation;
					if( LightingCubemap.ReferenceSpecified || LightingCubemap.Value != null )
					{
						multiplier = Vector3F.One;//LightingMultiplier.Value.ToVector3F();
						GetLightingCubemapRotationQuaternion( out rotation );
					}
					else
					{
						multiplier = CubemapMultiplier.Value.ToVector3F();// * LightingMultiplier.Value.ToVector3F();
						GetRotationQuaternion( out rotation );
					}

					environmentCubemap = new RenderingPipeline.EnvironmentTextureData( processedEnvironmentCubemap ?? ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
					irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( processedIrradianceHarmonics ?? RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics, affect, ref rotation, ref multiplier );
				}

				return true;
			}


			//if( ProceduralIntensity > 0 )
			//{
			//	var affect = (float)ProceduralAffectLighting.Value;
			//	var rotation = QuaternionF.Identity;
			//	var multiplier = new Vector3F( affect, affect, affect );

			//	//Vector3F multiplier;
			//	//QuaternionF rotation;
			//	//if( LightingCubemap.ReferenceSpecified || LightingCubemap.Value != null )
			//	//{
			//	//	multiplier = LightingMultiplier.Value.ToVector3F();
			//	//	GetLightingCubemapRotationQuaternion( out rotation );
			//	//}
			//	//else
			//	//{
			//	//	multiplier = CubemapMultiplier.Value.ToVector3F() * LightingMultiplier.Value.ToVector3F();
			//	//	GetRotationQuaternion( out rotation );
			//	//}

			//	//if( processedEnvironmentCubemap != null )
			//	//	environmentCubemap = new RenderingPipeline.EnvironmentTextureData( processedEnvironmentCubemap, affect, ref rotation, ref multiplier );
			//	//else

			//	environmentCubemap = new RenderingPipeline.EnvironmentTextureData( proceduralTexture ?? ResourceUtility.WhiteTextureCube/*GrayTextureCube*/, 1, ref rotation, ref multiplier );

			//	//if( processedIrradianceHarmonics != null )
			//	//	irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( processedIrradianceHarmonics, affect, ref rotation, ref multiplier );
			//	//else
			//	//{

			//	irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics/*GrayHarmonics*/, 1, ref rotation, ref multiplier );

			//	//}

			//	return true;
			//}

			//{
			//	var affect = (float)CubemapAffectLighting.Value;
			//	if( affect > 0 )
			//	{
			//		Vector3F multiplier;
			//		QuaternionF rotation;
			//		if( LightingCubemap.ReferenceSpecified || LightingCubemap.Value != null )
			//		{
			//			multiplier = Vector3F.One;//LightingMultiplier.Value.ToVector3F();
			//			GetLightingCubemapRotationQuaternion( out rotation );
			//		}
			//		else
			//		{
			//			multiplier = CubemapMultiplier.Value.ToVector3F();// * LightingMultiplier.Value.ToVector3F();
			//			GetRotationQuaternion( out rotation );
			//		}

			//		if( processedEnvironmentCubemap != null )
			//			environmentCubemap = new RenderingPipeline.EnvironmentTextureData( processedEnvironmentCubemap, affect, ref rotation, ref multiplier );
			//		else
			//			environmentCubemap = new RenderingPipeline.EnvironmentTextureData( ResourceUtility.GrayTextureCube, affect );

			//		if( processedIrradianceHarmonics != null )
			//			irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( processedIrradianceHarmonics, affect, ref rotation, ref multiplier );
			//		else
			//		{
			//			irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( RenderingPipeline.EnvironmentIrradianceData.GrayHarmonics, affect );
			//			//irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData( ResourceUtility.GrayTextureCube, affect );
			//		}

			//		return true;
			//	}
			//}

			environmentCubemap = new RenderingPipeline.EnvironmentTextureData();
			irradianceHarmonics = new RenderingPipeline.EnvironmentIrradianceData();
			return false;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//old version compatibility
			if( block.AttributeExists( "Multiplier" ) && ColorValuePowered.TryParse( block.GetAttribute( "Multiplier" ), out var v ) )
				CubemapMultiplier = v;
			if( block.AttributeExists( "Rotation" ) && Degree.TryParse( block.GetAttribute( "Rotation" ), out var v2 ) )
				CubemapRotation = v2;
			if( block.AttributeExists( "Stretch" ) && double.TryParse( block.GetAttribute( "Stretch" ), out var v3 ) )
				CubemapStretch = v3;
			//if( block.AttributeExists( "AffectLighting" ) && double.TryParse( block.GetAttribute( "AffectLighting" ), out var v4 ) )
			//	CubemapAffectLighting = v4;

			return true;
		}

		void UpdateProceduralCubemap( Scene scene, RenderingPipeline_Basic pipeline, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData )
		{
			if( GetProceduralIntensity() > 0 )//&& !ReflectionCubemapGeneration )
			{
				var time = EngineApp.EngineTime;
				if( ( time - proceduralLastUpdateTime ) < 0.00001 )// ProceduralUpdateTime )
					return;
				proceduralLastUpdateTime = time;

				var needSize = int.Parse( ProceduralResolution.Value.ToString().Replace( "_", "" ) );
				var needFormat = pipeline.GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8;

				var fullUpdate = false;

				//create or recreate scene
				if( proceduralTexture == null || proceduralTextureSize != needSize || proceduralTextureFormat != needFormat )
				{
					ProceduralTextureDispose();

					//create cubemap render target
					var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
					texture.CreateType = ImageComponent.TypeEnum.Cube;
					texture.CreateSize = new Vector2I( needSize, needSize );
					texture.CreateFormat = needFormat;
					texture.CreateUsage = ImageComponent.Usages.RenderTarget;
					texture.CreateMipmaps = true;
					texture.Enabled = true;

					for( int mip = 0; mip < texture.Result.ResultMipLevels; mip++ )
					{
						for( int face = 0; face < 6; face++ )
						{
							var renderTexture = texture.Result.GetRenderTarget( mip, face );
							if( renderTexture != null )
								renderTexture.AddViewport( false, false );
						}
					}

					proceduralTexture = texture;
					proceduralTextureSize = needSize;
					proceduralTextureFormat = needFormat;

					fullUpdate = true;
				}

				//stay allocated render target for each mip size
				for( int mip = 0; mip < proceduralTexture.Result.ResultMipLevels; mip++ )
				{
					var size = needSize;
					if( mip > 0 )
						size = (int)( (double)size / (double)mip * 3.0 );
					if( size < 1 )
						size = 1;
					var tempTarget = context.RenderTarget2D_Alloc( new Vector2I( size, size ), needFormat );
					context.DynamicTexture_Free( tempTarget );
				}

				//render
				var counter = 0;
				for( int mip = 0; mip < proceduralTexture.Result.ResultMipLevels; mip++ )
				{
					for( int face = 0; face < 6; face++ )
					{
						if( fullUpdate || counter == proceduralUpdateCurrentItem )
						{
							var renderTexture = proceduralTexture.Result.GetRenderTarget( mip, face );
							if( renderTexture != null )
							{
								var size = needSize;
								if( mip > 0 )
									size = (int)( (double)size / (double)mip * 3.0 );
								if( size < 1 )
									size = 1;
								var tempTarget = context.RenderTarget2D_Alloc( new Vector2I( size, size ), needFormat );

								var viewport = renderTexture.Viewports[ 0 ];

								var dir = Vector3.Zero;
								var up = Vector3.Zero;
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

								var ownerCameraSettings = context.OwnerCameraSettings;
								var cameraSettings = new Viewport.CameraSettingsClass( viewport, 1, 90, 0.1, 1000, Vector3.Zero, dir, up, ProjectionType.Perspective, 1, ownerCameraSettings.Exposure, ownerCameraSettings.EmissiveFactor );

								//render sky
								var viewMatrix = cameraSettings.ViewMatrixRelative;
								var projectionMatrix = cameraSettings.ProjectionMatrix;
								context.SetViewport( tempTarget.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix );
								Render2( pipeline, context, frameData, true );

								//copy to cubemap
								context.SetViewport( renderTexture.Viewports[ 0 ] );
								var filtering = context.CurrentViewport.SizeInPixels != tempTarget.Result.ResultSize ? FilterOption.Linear : FilterOption.Point;
								pipeline.CopyToCurrentViewport( context, tempTarget, filtering: filtering );

								context.DynamicTexture_Free( tempTarget );
							}
						}

						counter++;
					}
				}

				var totalItems = counter;//6 * proceduralTexture.Result.ResultMipLevels;
				proceduralUpdateCurrentItem++;
				if( proceduralUpdateCurrentItem >= totalItems )
					proceduralUpdateCurrentItem = 0;


				//////procedural harmonics
				////{
				////	if( proceduralHarmonics == null )
				////		proceduralHarmonics = new Vector4F[ 9 ] { Vector4F.One, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero };

				////	//var lightDirection = new Vector3F( 1, 0, 0 );
				////	//var directionalLight = GetDirectionalLight( frameData );
				////	//if( directionalLight != null )
				////	//	lightDirection = -directionalLight.data.Rotation.GetForward();

				////	////!!!!
				////	//var color = new Vector4F( 1, 1, 1, 0 );

				////	//proceduralHarmonics[ 0 ] = new Vector4F( 0.5f, 0.5f, 0.5f, 0 );

				////	//{
				////	//	var angleCoef = (float)( MathAlgorithms.GetVectorsAngle( -lightDirection, Vector3F.YAxis ) / ( Math.PI / 2 ) );
				////	//	proceduralHarmonics[ 1 ] = angleCoef * color;
				////	//}

				////	//{
				////	//	var angleCoef = (float)( MathAlgorithms.GetVectorsAngle( -lightDirection, Vector3F.ZAxis ) / ( Math.PI / 2 ) );
				////	//	proceduralHarmonics[ 2 ] = angleCoef * color;
				////	//}

				////	//{
				////	//	var angleCoef = (float)( MathAlgorithms.GetVectorsAngle( -lightDirection, Vector3F.XAxis ) / ( Math.PI / 2 ) );
				////	//	proceduralHarmonics[ 3 ] = angleCoef * color;
				////	//}

				////	////var proceduralHarmonics2 = proceduralHarmonics ?? RenderingPipeline.EnvironmentIrradianceData.WhiteHarmonics;

				////}
			}
			else
				ProceduralTextureDispose();
		}

		void ProceduralTextureDispose()
		{
			proceduralTexture?.Dispose();
			proceduralTexture = null;
		}

		//public void NeedUpdateProceduralTexture()
		//{
		//	proceduralLastUpdateTime = 0;
		//}

		//private void Scene_ViewportUpdateBefore( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		//{
		//	if( !RenderingSystem.BackendNull )
		//		UpdateProceduralCubemap();
		//}

		private void Scene_RenderAfterSetCommonUniforms( Scene scene, RenderingPipeline_Basic pipeline, ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData )
		{
			if( !RenderingSystem.BackendNull )
				UpdateProceduralCubemap( scene, pipeline, context, frameData );
		}
	}
}
