// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The light source in the scene.
	/// </summary>
	public class Light : ObjectInSpace
	{
		static long uniqueIdentifierCounterForStaticShadows = 1L;
		static object uniqueIdentifierLockForStaticShadows = new object();
		long? uniqueIdentifierForStaticShadows;

		//RenderSceneData renderSceneDataCached;
		//int _internalRenderSceneIndex = -1;

		Plane[] cachedSpotlightClipPlanes;
		InclusiveVolumeData cachedInclusiveVolume;
		//ConvexPolyhedron cachedInclusiveVolume;
		//Bounds cachedInclusiveVolumeBounds;

		/////////////////////////////////////////

		/// <summary>
		/// Represents data for <see cref="GetInclusiveVolume"/> method.
		/// </summary>
		public class InclusiveVolumeData
		{
			public ConvexPolyhedron ConvexPolyhedron;
			public Bounds Bounds;
		}

		/////////////////////////////////////////

		//!!!!какие-то настройки теней специфицировать для источника
		//!!!!!!ссылкой по дефолту ссылаться на scene. RenderingPipeline?
		//!!!!!!!!может тогда ввести префикс в ссылке "scene:". хотя, возможно это слишком специализированно


		//!!!!тоже типом. что еще
		//!!!!!!!возможность добавлять новые
		/// <summary>
		/// The type of light source.
		/// </summary>
		//[DefaultValue( TypeEnum.Point )]//надо ли тут дефолтное? где еще так
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set
			{
				if( _type.BeginSet( this, ref value ) )
				{
					try
					{
						TypeChanged?.Invoke( this );
						SpaceBoundsUpdate();

						//rendering pipeline optimization
						var scene = ParentScene;
						if( scene != null )
						{
							if( EnabledInHierarchyAndIsInstance )
							{
								scene.CachedDirectionalLightsToFastFindByRenderingPipeline.Remove( this );
								if( _type.value == TypeEnum.Directional )
									scene.CachedDirectionalLightsToFastFindByRenderingPipeline.AddWithCheckAlreadyContained( this );
							}
						}
					}
					finally { _type.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Light> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.Point;

		/// <summary>
		/// The light's brightness.
		/// </summary>
		[DefaultValue( 100000.0 )]
		[Range( 0, 10000000, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> Brightness
		{
			get { if( _brightness.BeginGet() ) Brightness = _brightness.Get( this ); return _brightness.value; }
			set { if( _brightness.BeginSet( this, ref value ) ) { try { BrightnessChanged?.Invoke( this ); } finally { _brightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Brightness"/> property value changes.</summary>
		public event Action<Light> BrightnessChanged;
		ReferenceField<double> _brightness = 100000.0;

		/// <summary>
		/// The color of emitted light.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Light> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );

		////!!!!можно указывать функцию расчета света
		////!!!!ну и всё в понятных единицах может. смотреть где как сделано в других движках
		///// <summary>
		///// The power of light.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[ApplicableRangeColorValuePower( 0, 100, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		//[ColorValueNoAlpha]
		//public Reference<ColorValuePowered> Power
		//{
		//	get { if( _power.BeginGet() ) Power = _power.Get( this ); return _power.value; }
		//	set { if( _power.BeginSet( this, ref value ) ) { try { PowerChanged?.Invoke( this ); } finally { _power.EndSet(); } } }
		//}
		//public event Action<Light> PowerChanged;
		//ReferenceField<ColorValuePowered> _power = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// The minimum distance of the source light.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 )]
		public Reference<double> AttenuationNear
		{
			get { if( _attenuationNear.BeginGet() ) AttenuationNear = _attenuationNear.Get( this ); return _attenuationNear.value; }
			set { if( _attenuationNear.BeginSet( this, ref value ) ) { try { AttenuationNearChanged?.Invoke( this ); } finally { _attenuationNear.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AttenuationNear"/> property value changes.</summary>
		public event Action<Light> AttenuationNearChanged;
		ReferenceField<double> _attenuationNear = 0;

		/// <summary>
		/// The maximum distance of the source light.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 )]
		public Reference<double> AttenuationFar
		{
			get { if( _attenuationFar.BeginGet() ) AttenuationFar = _attenuationFar.Get( this ); return _attenuationFar.value; }
			set
			{
				if( _attenuationFar.BeginSet( this, ref value ) )
				{
					try
					{
						AttenuationFarChanged?.Invoke( this );
						SpaceBoundsUpdate();
					}
					finally { _attenuationFar.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AttenuationFar"/> property value changes.</summary>
		public event Action<Light> AttenuationFarChanged;
		ReferenceField<double> _attenuationFar = 1.0;

		/// <summary>
		/// The multiplier of light attenuation power.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 )]
		public Reference<double> AttenuationPower
		{
			get { if( _attenuationPower.BeginGet() ) AttenuationPower = _attenuationPower.Get( this ); return _attenuationPower.value; }
			set
			{
				if( _attenuationPower.BeginSet( this, ref value ) )
				{
					try { AttenuationPowerChanged?.Invoke( this ); }
					finally { _attenuationPower.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AttenuationPower"/> property value changes.</summary>
		public event Action<Light> AttenuationPowerChanged;
		ReferenceField<double> _attenuationPower = 2;

		/// <summary>
		/// The inner angle of the spot light.
		/// </summary>
		[DefaultValue( "40" )]
		[Range( 0, 180, RangeAttribute.ConvenientDistributionEnum.Linear )]
		public Reference<Degree> SpotlightInnerAngle
		{
			get { if( _spotlightInnerAngle.BeginGet() ) SpotlightInnerAngle = _spotlightInnerAngle.Get( this ); return _spotlightInnerAngle.value; }
			set
			{
				if( _spotlightInnerAngle.BeginSet( this, ref value ) )
				{
					try { SpotlightInnerAngleChanged?.Invoke( this ); }
					finally { _spotlightInnerAngle.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SpotlightInnerAngle"/> property value changes.</summary>
		public event Action<Light> SpotlightInnerAngleChanged;
		ReferenceField<Degree> _spotlightInnerAngle = new Degree( 40 );

		/// <summary>
		/// The outer angle of the spot light.
		/// </summary>
		[DefaultValue( "50" )]
		[Range( 0, 180, RangeAttribute.ConvenientDistributionEnum.Linear )]
		public Reference<Degree> SpotlightOuterAngle
		{
			get { if( _spotlightOuterAngle.BeginGet() ) SpotlightOuterAngle = _spotlightOuterAngle.Get( this ); return _spotlightOuterAngle.value; }
			set
			{
				if( _spotlightOuterAngle.BeginSet( this, ref value ) )
				{
					try
					{
						SpotlightOuterAngleChanged?.Invoke( this );
						SpaceBoundsUpdate();
					}
					finally { _spotlightOuterAngle.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SpotlightOuterAngle"/> property value changes.</summary>
		public event Action<Light> SpotlightOuterAngleChanged;
		ReferenceField<Degree> _spotlightOuterAngle = new Degree( 50 );

		/// <summary>
		/// The spot light fall-off.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Linear )]//!!!!так?
		public Reference<double> SpotlightFalloff
		{
			get { if( _spotlightFalloff.BeginGet() ) SpotlightFalloff = _spotlightFalloff.Get( this ); return _spotlightFalloff.value; }
			set { if( _spotlightFalloff.BeginSet( this, ref value ) ) { try { SpotlightFalloffChanged?.Invoke( this ); } finally { _spotlightFalloff.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpotlightFalloff"/> property value changes.</summary>
		public event Action<Light> SpotlightFalloffChanged;
		ReferenceField<double> _spotlightFalloff = 1;

		/// <summary>
		/// The distance of lighting effect from the light position.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0.0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> StartDistance
		{
			get { if( _startDistance.BeginGet() ) StartDistance = _startDistance.Get( this ); return _startDistance.value; }
			set { if( _startDistance.BeginSet( this, ref value ) ) { try { StartDistanceChanged?.Invoke( this ); } finally { _startDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StartDistance"/> property value changes.</summary>
		public event Action<Light> StartDistanceChanged;
		ReferenceField<double> _startDistance = 0.0;

		//!!!!impl

		////!!!!range, default
		///// <summary>
		///// The radius of the light source. The parameter affects the softness of shadows.
		///// </summary>
		//[DefaultValue( 0.05 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		//public Reference<double> SourceRadius
		//{
		//	get { if( _sourceRadius.BeginGet() ) SourceRadius = _sourceRadius.Get( this ); return _sourceRadius.value; }
		//	set { if( _sourceRadius.BeginSet( this, ref value ) ) { try { SourceRadiusChanged?.Invoke( this ); } finally { _sourceRadius.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SourceRadius"/> property value changes.</summary>
		//public event Action<Light> SourceRadiusChanged;
		//ReferenceField<double> _sourceRadius = 0.05;

		////!!!!range, default
		///// <summary>
		///// The distribution angle of the light source. The parameter affects the softness of shadows.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		//public Reference<Degree> SourceAngle
		//{
		//	get { if( _sourceAngle.BeginGet() ) SourceAngle = _sourceAngle.Get( this ); return _sourceAngle.value; }
		//	set { if( _sourceAngle.BeginSet( this, ref value ) ) { try { SourceAngleChanged?.Invoke( this ); } finally { _sourceAngle.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SourceAngle"/> property value changes.</summary>
		//public event Action<Light> SourceAngleChanged;
		//ReferenceField<Degree> _sourceAngle = new Degree( 1.0 );

		/// <summary>
		/// The light mask used by the light source.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ImageComponent> Mask
		{
			get { if( _mask.BeginGet() ) Mask = _mask.Get( this ); return _mask.value; }
			set { if( _mask.BeginSet( this, ref value ) ) { try { MaskChanged?.Invoke( this ); } finally { _mask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mask"/> property value changes.</summary>
		public event Action<Light> MaskChanged;
		ReferenceField<ImageComponent> _mask;

		/// <summary>
		/// The position, rotation and scale of the light mask.
		/// </summary>
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> MaskTransform
		{
			get { if( _maskTransform.BeginGet() ) Transform = _maskTransform.Get( this ); return _maskTransform.value; }
			set
			{
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _maskTransform.BeginSet( this, ref value ) ) { try { MaskTransformChanged?.Invoke( this ); } finally { _maskTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<ObjectInSpace> MaskTransformChanged;
		ReferenceField<Transform> _maskTransform = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		///// <summary>
		///// The scale of the light mask.
		///// </summary>
		//[DefaultValue( 100.0 )]
		//[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> MaskScale
		//{
		//	get { if( _maskScale.BeginGet() ) MaskScale = _maskScale.Get( this ); return _maskScale.value; }
		//	set { if( _maskScale.BeginSet( this, ref value ) ) { try { MaskScaleChanged?.Invoke( this ); } finally { _maskScale.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MaskScale"/> property value changes.</summary>
		//public event Action<Light> MaskScaleChanged;
		//ReferenceField<double> _maskScale = 100;

		/// <summary>
		/// If active, the light will cast shadows on the surfaces.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shadows" )]
		public Reference<bool> Shadows
		{
			get { if( _shadows.BeginGet() ) Shadows = _shadows.Get( this ); return _shadows.value; }
			set { if( _shadows.BeginSet( this, ref value ) ) { try { ShadowsChanged?.Invoke( this ); } finally { _shadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shadows"/> property value changes.</summary>
		public event Action<Light> ShadowsChanged;
		ReferenceField<bool> _shadows = true;

		/// <summary>
		/// The intensity of the shadows.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowIntensity
		{
			get { if( _shadowIntensity.BeginGet() ) ShadowIntensity = _shadowIntensity.Get( this ); return _shadowIntensity.value; }
			set { if( _shadowIntensity.BeginSet( this, ref value ) ) { try { ShadowIntensityChanged?.Invoke( this ); } finally { _shadowIntensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowIntensity"/> property value changes.</summary>
		public event Action<Light> ShadowIntensityChanged;
		ReferenceField<double> _shadowIntensity = 1.0;

		///// <summary>
		///// The softness of the shadows.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> ShadowSoftness
		//{
		//	get { if( _shadowSoftness.BeginGet() ) ShadowSoftness = _shadowSoftness.Get( this ); return _shadowSoftness.value; }
		//	set { if( _shadowSoftness.BeginSet( this, ref value ) ) { try { ShadowSoftnessChanged?.Invoke( this ); } finally { _shadowSoftness.EndSet(); } } }
		//}
		//public event Action<Light> ShadowSoftnessChanged;
		//ReferenceField<double> _shadowSoftness = 1.0;

		/// <summary>
		/// Shadow bias moves the shadow away from the light source. Adjusting it may help to fix shadow artifacts.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowBias
		{
			get { if( _shadowBias.BeginGet() ) ShadowBias = _shadowBias.Get( this ); return _shadowBias.value; }
			set { if( _shadowBias.BeginSet( this, ref value ) ) { try { ShadowBiasChanged?.Invoke( this ); } finally { _shadowBias.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowBias"/> property value changes.</summary>
		public event Action<Light> ShadowBiasChanged;
		ReferenceField<double> _shadowBias = 0.5;

		/// <summary>
		/// Normal bias moves the shadow perpendicular to the shadowed surface. Adjusting it may help to fix shadow artifacts.
		/// </summary>
		[DefaultValue( 1.0 )]//0.5 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowNormalBias
		{
			get { if( _shadowNormalBias.BeginGet() ) ShadowNormalBias = _shadowNormalBias.Get( this ); return _shadowNormalBias.value; }
			set { if( _shadowNormalBias.BeginSet( this, ref value ) ) { try { ShadowNormalBiasChanged?.Invoke( this ); } finally { _shadowNormalBias.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowNormalBias"/> property value changes.</summary>
		public event Action<Light> ShadowNormalBiasChanged;
		ReferenceField<double> _shadowNormalBias = 1.0;//0.5;

		/// <summary>
		/// The softness multiplier of shadows.
		/// </summary>
		[DefaultValue( 1.5 )]
		[Range( 0, 3 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowSoftness
		{
			get { if( _shadowSoftness.BeginGet() ) ShadowSoftness = _shadowSoftness.Get( this ); return _shadowSoftness.value; }
			set { if( _shadowSoftness.BeginSet( this, ref value ) ) { try { ShadowSoftnessChanged?.Invoke( this ); } finally { _shadowSoftness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowSoftness"/> property value changes.</summary>
		public event Action<Light> ShadowSoftnessChanged;
		ReferenceField<double> _shadowSoftness = 1.5;

		public enum ShadowTextureSizeType
		{
			Default,
			Value,
			Quarter,
			Half,
			x2,
			x4,
		}

		/// <summary>
		/// A method to get the size of a shadow map.
		/// </summary>
		[DefaultValue( ShadowTextureSizeType.Default )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSizeType> ShadowTextureSize
		{
			get { if( _shadowTextureSize.BeginGet() ) ShadowTextureSize = _shadowTextureSize.Get( this ); return _shadowTextureSize.value; }
			set { if( _shadowTextureSize.BeginSet( this, ref value ) ) { try { ShadowTextureSizeChanged?.Invoke( this ); } finally { _shadowTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTextureSize"/> property value changes.</summary>
		public event Action<Light> ShadowTextureSizeChanged;
		ReferenceField<ShadowTextureSizeType> _shadowTextureSize = ShadowTextureSizeType.Default;

		/// <summary>
		/// The size of a shadow texture.
		/// </summary>
		[DefaultValue( ShadowTextureSizeEnum._1024 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSizeEnum> ShadowTextureSizeValue
		{
			get { if( _shadowTextureSizeValue.BeginGet() ) ShadowTextureSizeValue = _shadowTextureSizeValue.Get( this ); return _shadowTextureSizeValue.value; }
			set { if( _shadowTextureSizeValue.BeginSet( this, ref value ) ) { try { ShadowTextureSizeValueChanged?.Invoke( this ); } finally { _shadowTextureSizeValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTextureSizeValue"/> property value changes.</summary>
		public event Action<Light> ShadowTextureSizeValueChanged;
		ReferenceField<ShadowTextureSizeEnum> _shadowTextureSizeValue = ShadowTextureSizeEnum._1024;

		/// <summary>
		/// The minimal distance from the light source to generate shadows.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0.01, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowNearClipDistance
		{
			get { if( _shadowNearClipDistance.BeginGet() ) ShadowNearClipDistance = _shadowNearClipDistance.Get( this ); return _shadowNearClipDistance.value; }
			set { if( _shadowNearClipDistance.BeginSet( this, ref value ) ) { try { ShadowNearClipDistanceChanged?.Invoke( this ); } finally { _shadowNearClipDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowNearClipDistance"/> property value changes.</summary>
		public event Action<Light> ShadowNearClipDistanceChanged;
		ReferenceField<double> _shadowNearClipDistance = 0.1;

		/// <summary>
		/// Whether to detail the shadows by means the screen-space contact shadows technique. The contact shadows works only for deferred shading.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Shadows" )]
		public Reference<bool> ShadowContact
		{
			get { if( _shadowContact.BeginGet() ) ShadowContact = _shadowContact.Get( this ); return _shadowContact.value; }
			set { if( _shadowContact.BeginSet( this, ref value ) ) { try { ShadowContactChanged?.Invoke( this ); } finally { _shadowContact.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowContact"/> property value changes.</summary>
		public event Action<Light> ShadowContactChanged;
		ReferenceField<bool> _shadowContact = false;

		/// <summary>
		/// The maximal length of the contact shadows.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Range( 0, 1.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowContactLength
		{
			get { if( _shadowContactLength.BeginGet() ) ShadowContactLength = _shadowContactLength.Get( this ); return _shadowContactLength.value; }
			set { if( _shadowContactLength.BeginSet( this, ref value ) ) { try { ShadowContactLengthChanged?.Invoke( this ); } finally { _shadowContactLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowContactLength"/> property value changes.</summary>
		public event Action<Light> ShadowContactLengthChanged;
		ReferenceField<double> _shadowContactLength = 0.1;

		/// <summary>
		/// Whether to enable static shadow optimization for this light. Only for Point and Spot lights.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Shadows" )]
		public Reference<bool> ShadowStatic
		{
			get { if( _shadowStatic.BeginGet() ) ShadowStatic = _shadowStatic.Get( this ); return _shadowStatic.value; }
			set
			{
				if( _shadowStatic.BeginSet( this, ref value ) )
				{
					try
					{
						ShadowStaticChanged?.Invoke( this );
						uniqueIdentifierForStaticShadows = null;
					}
					finally { _shadowStatic.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShadowStatic"/> property value changes.</summary>
		public event Action<Light> ShadowStaticChanged;
		ReferenceField<bool> _shadowStatic = false;


		//!!!!можно было бы материал, не только текстурой


		/// <summary>
		/// The image of the flare.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Flare" )]
		public Reference<ImageComponent> FlareImage
		{
			get { if( _flareImage.BeginGet() ) FlareImage = _flareImage.Get( this ); return _flareImage.value; }
			set { if( _flareImage.BeginSet( this, ref value ) ) { try { FlareImageChanged?.Invoke( this ); } finally { _flareImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareImage"/> property value changes.</summary>
		public event Action<Light> FlareImageChanged;
		ReferenceField<ImageComponent> _flareImage = null;

		/// <summary>
		/// The method of drawing the image of the flare on the screen.
		/// </summary>
		[DefaultValue( CanvasRenderer.BlendingType.AlphaAdd )]
		[Category( "Flare" )]
		public Reference<CanvasRenderer.BlendingType> FlareBlending
		{
			get { if( _flareBlending.BeginGet() ) FlareBlending = _flareBlending.Get( this ); return _flareBlending.value; }
			set { if( _flareBlending.BeginSet( this, ref value ) ) { try { FlareBlendingChanged?.Invoke( this ); } finally { _flareBlending.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareBlending"/> property value changes.</summary>
		public event Action<Light> FlareBlendingChanged;
		ReferenceField<CanvasRenderer.BlendingType> _flareBlending = CanvasRenderer.BlendingType.AlphaAdd;

		/// <summary>
		/// The color of the flare.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Flare" )]
		public Reference<ColorValuePowered> FlareColor
		{
			get { if( _flareColor.BeginGet() ) FlareColor = _flareColor.Get( this ); return _flareColor.value; }
			set { if( _flareColor.BeginSet( this, ref value ) ) { try { FlareColorChanged?.Invoke( this ); } finally { _flareColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareColor"/> property value changes.</summary>
		public event Action<Light> FlareColorChanged;
		ReferenceField<ColorValuePowered> _flareColor = ColorValuePowered.One;

		/// <summary>
		/// The position of the flare.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( -10, 1, RangeAttribute.ConvenientDistributionEnum.Exponential, 0.2 )]
		[Category( "Flare" )]
		public Reference<double> FlarePosition
		{
			get { if( _flarePosition.BeginGet() ) FlarePosition = _flarePosition.Get( this ); return _flarePosition.value; }
			set { if( _flarePosition.BeginSet( this, ref value ) ) { try { FlarePositionChanged?.Invoke( this ); } finally { _flarePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlarePosition"/> property value changes.</summary>
		public event Action<Light> FlarePositionChanged;
		ReferenceField<double> _flarePosition = 1;

		/// <summary>
		/// The size of the flare. It is indicated as a ratio of screen size vertically.
		/// </summary>
		[DefaultValue( "0.1 0.1" )]
		[Range( 0, 0.5 )]
		[Category( "Flare" )]
		public Reference<Vector2> FlareSize
		{
			get { if( _flareSize.BeginGet() ) FlareSize = _flareSize.Get( this ); return _flareSize.value; }
			set { if( _flareSize.BeginSet( this, ref value ) ) { try { FlareSizeChanged?.Invoke( this ); } finally { _flareSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareSize"/> property value changes.</summary>
		public event Action<Light> FlareSizeChanged;
		ReferenceField<Vector2> _flareSize = new Vector2( 0.1, 0.1 );

		[DefaultValue( false )]
		[Category( "Flare" )]
		public Reference<bool> FlareSizeFadeByDistance
		{
			get { if( _flareSizeFadeByDistance.BeginGet() ) FlareSizeFadeByDistance = _flareSizeFadeByDistance.Get( this ); return _flareSizeFadeByDistance.value; }
			set { if( _flareSizeFadeByDistance.BeginSet( this, ref value ) ) { try { FlareSizeFadeByDistanceChanged?.Invoke( this ); } finally { _flareSizeFadeByDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareSizeFadeByDistance"/> property value changes.</summary>
		public event Action<Light> FlareSizeFadeByDistanceChanged;
		ReferenceField<bool> _flareSizeFadeByDistance = false;

		[DefaultValue( 0.1 )]
		[Range( 0, 2, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Flare" )]
		public Reference<double> FlareDepthCheckOffset
		{
			get { if( _flareDepthCheckOffset.BeginGet() ) FlareDepthCheckOffset = _flareDepthCheckOffset.Get( this ); return _flareDepthCheckOffset.value; }
			set { if( _flareDepthCheckOffset.BeginSet( this, ref value ) ) { try { FlareDepthCheckOffsetChanged?.Invoke( this ); } finally { _flareDepthCheckOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlareDepthCheckOffset"/> property value changes.</summary>
		public event Action<Light> FlareDepthCheckOffsetChanged;
		ReferenceField<double> _flareDepthCheckOffset = 0.1;


		//!!!!volume. чтобы directional ограничивать. да и просто ограничивать?

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum TypeEnum
		{
			Ambient,
			/// <summary>Directional lights simulate parallel light beams from a distant source, hence have direction but no position.</summary>
			Directional,
			/// <summary>Point light sources give off light equally in all directions, so require only position not direction.</summary>
			Point,
			/// <summary>Spotlights simulate a cone of light from a source so require position and direction, plus extra values for falloff.</summary>
			Spotlight,

			//!!!!!
			//Sky
			//Area
			//Special. нужен ли особый тип? или другим свойством, а это типа базисные. уже над базовыми надстраиваются новые
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class RenderSceneData
		//{
		//	public Transform transform;
		//	public TypeEnum type;
		//	public Vector3 power;
		//	public double attenuationNear;
		//	public double attenuationFar;
		//	public double attenuationPower;
		//	public Degree spotlightInnerAngle;
		//	public Degree spotlightOuterAngle;
		//	public double spotlightFalloff;
		//	public Plane[] spotlightClipPlanes;
		//	public bool castShadows;
		//	public double shadowIntensity;
		//	public double shadowBias;
		//	public double shadowNormalBias;
		//	public Image mask;
		//	//!!!!
		//	public double maskScale;
		//	//public Vec2 maskPosition;
		//	//public Vec2 maskScale;
		//	//public Mat4 maskMatrix;
		//	public SpaceBounds spaceBounds;
		//}

		//public virtual void GetRenderSceneData( ref RenderSceneData data )
		//{
		//	if( EnabledInHierarchy && VisibleInHierarchy )
		//	{
		//		if( renderSceneDataCached == null )
		//			renderSceneDataCached = new RenderSceneData();

		//		renderSceneDataCached.transform = Transform;
		//		renderSceneDataCached.type = Type;

		//		//!!!!
		//		renderSceneDataCached.power = Color.Value.ToVec3() * Brightness.Value / 100000.0;
		//		//renderSceneDataCached.power = Power.Value.ToVec3();

		//		var scale = renderSceneDataCached.transform.Scale.MaxComponent();
		//		renderSceneDataCached.attenuationNear = AttenuationNear * scale;
		//		renderSceneDataCached.attenuationFar = AttenuationFar * scale;
		//		renderSceneDataCached.attenuationPower = AttenuationPower;
		//		renderSceneDataCached.spotlightInnerAngle = SpotlightInnerAngle;
		//		renderSceneDataCached.spotlightOuterAngle = SpotlightOuterAngle;
		//		renderSceneDataCached.spotlightFalloff = SpotlightFalloff;
		//		if( renderSceneDataCached.type == TypeEnum.Spotlight )
		//			renderSceneDataCached.spotlightClipPlanes = SpotlightClipPlanes;
		//		renderSceneDataCached.castShadows = Shadows;
		//		renderSceneDataCached.shadowIntensity = ShadowIntensity;
		//		renderSceneDataCached.shadowBias = ShadowBias;
		//		renderSceneDataCached.shadowNormalBias = ShadowNormalBias;

		//		renderSceneDataCached.mask = Mask;
		//		//renderSceneDataCached.maskPosition = MaskPosition;
		//		renderSceneDataCached.maskScale = MaskScale;
		//		//renderSceneDataCached.maskMatrix = MaskMatrix;
		//		renderSceneDataCached.spaceBounds = SpaceBounds;
		//	}
		//	else
		//		renderSceneDataCached = null;

		//	data = renderSceneDataCached;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var power = Color.Value.ToVector3() * Brightness.Value;// / 100000.0;
				if( power != Vector3.Zero )
				{
					var item = new RenderingPipeline.RenderSceneData.LightItem();
					item.Creator = this;
					item.BoundingBox = SpaceBounds.boundingBox;
					item.BoundingSphere = SpaceBounds.boundingSphere;

					var tr = Transform.Value;
					item.Position = tr.Position;
					item.Rotation = tr.Rotation.ToQuaternionF();
					item.Scale = tr.Scale.ToVector3F();

					item.Type = Type;
					item.Power = power.ToVector3F();
					//renderSceneDataCached.power = Power.Value.ToVec3();

					var skipByDistance = false;
					if( context.LightFarDistance < 100000.0f && ( item.Type == TypeEnum.Point || item.Type == TypeEnum.Spotlight ) )
					{
						var length = ( context.OwnerCameraSettings.position - item.BoundingSphere.Center ).Length();
						var diff = length - context.LightFarDistance;
						if( diff < item.BoundingSphere.Radius && item.BoundingSphere.Radius > 0 )
						{
							if( diff > 0 )
							{
								var factor = (float)( diff / item.BoundingSphere.Radius );
								var powerScale = MathEx.Saturate( 1.0f - factor );
								if( powerScale > 0 )
									item.Power *= powerScale;
								else
									skipByDistance = true;
							}
						}
						else
							skipByDistance = true;
					}

					if( !skipByDistance )
					{
						var scale = item.Scale.MaxComponent();
						item.AttenuationNear = (float)AttenuationNear * scale;
						item.AttenuationFar = (float)AttenuationFar * scale;
						item.AttenuationPower = (float)AttenuationPower;
						if( item.Type == TypeEnum.Spotlight )
						{
							item.SpotlightInnerAngle = SpotlightInnerAngle.Value.ToDegreeF();
							item.SpotlightOuterAngle = SpotlightOuterAngle.Value.ToDegreeF();
							item.SpotlightFalloff = (float)SpotlightFalloff;
							item.SpotlightClipPlanes = SpotlightClipPlanes;
						}
						item.StartDistance = (float)StartDistance;

						if( item.Type != TypeEnum.Ambient )
						{
							item.CastShadows = Shadows;
							if( item.CastShadows )
							{
								item.ShadowIntensity = (float)ShadowIntensity;
								//item.ShadowSoftness = (float)ShadowSoftness;
								item.ShadowBias = (float)ShadowBias;
								item.ShadowNormalBias = (float)ShadowNormalBias;
								item.ShadowSoftness = (float)ShadowSoftness;
								item.ShadowTextureSize = ShadowTextureSize;
								if( item.ShadowTextureSize == ShadowTextureSizeType.Value )
									item.ShadowTextureSizeValue = ShadowTextureSizeValue;
								//item.ShadowContact = ShadowContact;
								if( ShadowContact )
									item.ShadowContactLength = (float)ShadowContactLength;
							}

							//if( item.Type == TypeEnum.Spotlight || item.Type == TypeEnum.Point )
							//	item.SourceRadiusOrAngle = (float)SourceRadius;
							//else if( item.Type == TypeEnum.Directional )
							//	item.SourceRadiusOrAngle = SourceAngle.Value.ToDegreeF();
						}

						item.Mask = Mask;
						if( item.Type == TypeEnum.Directional && item.Mask != null )
							item.MaskTransform = MaskTransform;
						//item.MaskScale = MaskScale;

						//renderSceneDataCached.maskPosition = MaskPosition;
						//renderSceneDataCached.maskMatrix = MaskMatrix;
						//item.SpaceBounds = SpaceBounds;

						if( Components.Count != 0 )
						{
							item.children = GetComponents( onlyEnabledInHierarchy: true );
							//item.children = GetComponents<ILightChild>( onlyEnabledInHierarchy: true );
						}

						if( ( item.Type == TypeEnum.Point || item.Type == TypeEnum.Spotlight ) && context.StaticShadows && ShadowStatic )
						{
							item.StaticShadows = true;
							//item.UniqueIdentifierForStaticShadows = GetUniqueIdentifierForStaticShadows();
						}

						item.ShadowNearClipDistance = (float)ShadowNearClipDistance;

						context.FrameData.RenderSceneData.Lights.Add( ref item );
					}
				}

				//display editor selection
				{
					var context2 = context.ObjectInSpaceRenderingContext;

					bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayLights ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
					if( show )
					{
						if( context2.displayLightsCounter < context2.displayLightsMax )
						{
							context2.displayLightsCounter++;

							ColorValue color;
							if( context2.selectedObjects.Contains( this ) )
								color = ProjectSettings.Get.Colors.SelectedColor;
							else if( context2.canSelectObjects.Contains( this ) )
								color = ProjectSettings.Get.Colors.CanSelectColor;
							else
								color = ProjectSettings.Get.Colors.SceneShowLightColor;

							var viewport = context.Owner;
							if( viewport.Simple3DRenderer != null )
							{
								viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
								DebugDraw( viewport );
							}
						}
					}
					//if( !show )
					//	context.disableShowingLabelForThisObject = true;
				}
			}
		}

		void AddLineSegmented( Simple3DRenderer renderer, Vector3 from, Vector3 to )
		{
			RendererUtility.AddLineSegmented( renderer, from, to );

			////draw line segments so that there is no problem with the thickness of the lines
			//var ray = new Ray( from, to - from );
			//int steps = (int)MathEx.Lerp( 2, 10, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );
			////int steps = (int)MathEx.Lerp( 5, 40, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );
			////int steps = (int)MathEx.Lerp( 10, 100, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );

			//for( int n = 0; n < steps; n++ )
			//{
			//	var p0 = ray.GetPointOnRay( (double)n / steps );
			//	var p1 = ray.GetPointOnRay( (double)( n + 1 ) / steps );
			//	renderer.AddLine( p0, p1 );
			//}
		}

		void DebugDrawBorder( Viewport viewport, double distance, bool isFarDistance )
		{
			var type = Type.Value;
			var trans = Transform.Value;
			var pos = trans.Position;
			var rot = trans.Rotation;
			var rotMat = rot.ToMatrix3();

			if( type == TypeEnum.Point )
			{
				var t = new Matrix4( rotMat, pos );
				double r = distance;

				double thickness = 0;
				//if( distance < 20 )
				//	thickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.LineThickness );

				double steps = isFarDistance ? 32 : 16;
				double angleStep = Math.PI / steps;
				for( double angle = 0; angle < Math.PI * 2 - angleStep * .5; angle += angleStep )
				{
					double p1sin = Math.Sin( angle );
					double p1cos = Math.Cos( angle );
					double p2sin = Math.Sin( angle + angleStep );
					double p2cos = Math.Cos( angle + angleStep );

					//может больше линий рисовать. еще под 45 градусов

					viewport.Simple3DRenderer.AddLine( t * ( new Vector3( p1cos, p1sin, 0 ) * r ), t * ( new Vector3( p2cos, p2sin, 0 ) * r ), thickness );
					viewport.Simple3DRenderer.AddLine( t * ( new Vector3( 0, p1cos, p1sin ) * r ), t * ( new Vector3( 0, p2cos, p2sin ) * r ), thickness );
					viewport.Simple3DRenderer.AddLine( t * ( new Vector3( p1cos, 0, p1sin ) * r ), t * ( new Vector3( p2cos, 0, p2sin ) * r ), thickness );
				}
			}
			else if( type == TypeEnum.Spotlight )
			{
				const double axisStep = Math.PI / 4;//Math.PI / 8;
				const double circleStep = Math.PI / 32;

				double outerAngle = SpotlightOuterAngle.Value.InRadians();
				Matrix3 outerAngleRotation = Matrix3.FromRotateByY( outerAngle / 2 );

				for( double axisAngle = 0; axisAngle < Math.PI * 2 - axisStep * .5; axisAngle += axisStep )
				{
					Matrix3 worldAxisRotation = rotMat * Matrix3.FromRotateByX( axisAngle );

					if( isFarDistance )
					{
						Matrix3 pointRotation = worldAxisRotation * outerAngleRotation;
						Vector3 point = pos + pointRotation * new Vector3( distance, 0, 0 );

						AddLineSegmented( viewport.Simple3DRenderer, pos, point );
						////draw line segments so that there is no problem with the thickness of the lines
						//var from = pos;
						//var to = point;
						//var ray = new Ray( from, to - from );
						//int steps = (int)MathEx.Lerp( 3, 10, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );
						////int steps = (int)MathEx.Lerp( 5, 40, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );
						////int steps = (int)MathEx.Lerp( 10, 100, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );

						//for( int n = 0; n < steps; n++ )
						//{
						//	var p0 = ray.GetPointOnRay( (double)n / steps );
						//	var p1 = ray.GetPointOnRay( (double)( n + 1 ) / steps );
						//	viewport.Simple3DRenderer.AddLine( p0, p1 );
						//}
						////viewport.Simple3DRenderer.AddLine( pos, point );
					}

					{
						Vector3 lastPoint2 = Vector3.Zero;
						for( double nOuterAngle = 0; nOuterAngle < outerAngle / 2.0 + circleStep * 1.5; nOuterAngle += circleStep )
						{
							double outerAngle2 = nOuterAngle;
							if( outerAngle2 > outerAngle / 2 )
								outerAngle2 = outerAngle / 2;

							Matrix3 point2Rotation = worldAxisRotation * Matrix3.FromRotateByY( outerAngle2 );
							Vector3 point2 = pos + point2Rotation * new Vector3( distance, 0, 0 );

							if( nOuterAngle != 0 )
								viewport.Simple3DRenderer.AddLine( lastPoint2, point2 );

							lastPoint2 = point2;
						}
					}
				}

				{
					Vector3 lastPoint = Vector3.Zero;
					for( double axisAngle = 0; axisAngle < Math.PI * 2 + circleStep * .5; axisAngle += circleStep )
					{
						Matrix3 worldAxisRotation = rotMat * Matrix3.FromRotateByX( axisAngle );
						Matrix3 pointRotation = worldAxisRotation * Matrix3.FromRotateByY( outerAngle / 2 );
						Vector3 point = pos + pointRotation * new Vector3( distance, 0, 0 );

						if( axisAngle != 0 )
							viewport.Simple3DRenderer.AddLine( lastPoint, point );

						lastPoint = point;
					}
				}
			}
		}

		public void DebugDraw( Viewport viewport )
		{
			var type = Type.Value;
			var trans = Transform.Value;
			var pos = trans.Position;
			var rot = trans.Rotation;

			var scale = trans.Scale.MaxComponent();
			double near = AttenuationNear * scale;
			double far = AttenuationFar * scale;

			//!!!!так?
			//!!!!.3? может не больше какого-то числа?
			//!!!!билбордом?
			//viewport.DebugGeometry.AddSphere( Transform.Value.Position, .3 );

			//Direction arrow
			if( type == TypeEnum.Directional || type == TypeEnum.Spotlight )
			{
				//!!!!TransformToolConfig

				var toolSize = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.SceneEditor.TransformToolSizeScaled );
				double thickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( pos, ProjectSettings.Get.SceneEditor.TransformToolLineThicknessScaled );
				var length = toolSize / 1.5;
				var headHeight = length / 4;

				viewport.Simple3DRenderer.AddArrow( pos, pos + rot * new Vector3( length, 0, 0 ), headHeight, 0, true, thickness );
			}

			if( type == TypeEnum.Point || type == TypeEnum.Spotlight )
			{
				//AttenuationNear
				if( near != 0 && near < far - .01f )
					DebugDrawBorder( viewport, near, false );

				//AttenuationFar
				if( far != 0 )
				{
					DebugDrawBorder( viewport, far, true );

					if( type == TypeEnum.Point )
					{
						AddLineSegmented( viewport.Simple3DRenderer, pos - rot * new Vector3( far, 0, 0 ), pos + rot * new Vector3( far, 0, 0 ) );
						AddLineSegmented( viewport.Simple3DRenderer, pos - rot * new Vector3( 0, far, 0 ), pos + rot * new Vector3( 0, far, 0 ) );
						AddLineSegmented( viewport.Simple3DRenderer, pos - rot * new Vector3( 0, 0, far ), pos + rot * new Vector3( 0, 0, far ) );
						//viewport.Simple3DRenderer.AddLine( pos - rot * new Vector3( far, 0, 0 ), pos + rot * new Vector3( far, 0, 0 ) );
						//viewport.Simple3DRenderer.AddLine( pos - rot * new Vector3( 0, far, 0 ), pos + rot * new Vector3( 0, far, 0 ) );
						//viewport.Simple3DRenderer.AddLine( pos - rot * new Vector3( 0, 0, far ), pos + rot * new Vector3( 0, 0, far ) );
					}
				}
			}

			//if( type == TypeEnum.Spotlight )
			//{
			//	if( cachedInclusiveVolume != null )
			//	{
			//		viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
			//		foreach( var p in cachedInclusiveVolume.Vertices )
			//			viewport.Simple3DRenderer.AddSphere( new Sphere( p, 0.3 ) );
			//	}
			//}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			cachedSpotlightClipPlanes = null;
			cachedInclusiveVolume = null;

			switch( Type.Value )
			{
			case TypeEnum.Point:
				{
					var tr = Transform.Value;
					var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();
					newBounds = new SpaceBounds( new Sphere( tr.Position, attenuationFar ) );
				}
				break;

			case TypeEnum.Spotlight:
				{
					var volume = GetInclusiveVolume();
					if( volume != null )
						newBounds = new SpaceBounds( volume.Bounds );
					else
					{
						var tr = Transform.Value;
						var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();
						newBounds = new SpaceBounds( new Sphere( tr.Position, attenuationFar ) );
					}

					//if( GetInclusiveVolume( out var volume, out var volumeBounds ) )
					//	newBounds = new SpaceBounds( volumeBounds );
					//else
					//{
					//	var tr = Transform.Value;
					//	var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();
					//	newBounds = new SpaceBounds( new Sphere( tr.Position, attenuationFar ) );
					//}
				}
				break;
			}

			uniqueIdentifierForStaticShadows = null;
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			//if( !ParentScene.GetShowDevelopmentDataInThisApplication() || !ParentScene.ShowLights )
			//	return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( SpotlightInnerAngle ):
				case nameof( SpotlightOuterAngle ):
				case nameof( SpotlightFalloff ):
					if( Type.Value != TypeEnum.Spotlight )
						skip = true;
					break;

				case nameof( AttenuationNear ):
				case nameof( AttenuationFar ):
				case nameof( AttenuationPower ):
					{
						var t = Type.Value;
						if( t == TypeEnum.Ambient || t == TypeEnum.Directional )
							skip = true;
					}
					break;

				case nameof( StartDistance ):
				case nameof( Shadows ):
					if( Type.Value == TypeEnum.Ambient )
						skip = true;
					break;

				case nameof( ShadowIntensity ):
				case nameof( ShadowBias ):
				case nameof( ShadowNormalBias ):
				case nameof( ShadowSoftness ):
				case nameof( ShadowContact ):
				case nameof( ShadowTextureSize ):
				case nameof( ShadowNearClipDistance ):
					if( Type.Value == TypeEnum.Ambient || !Shadows )
						skip = true;
					break;

				case nameof( ShadowTextureSizeValue ):
					if( Type.Value == TypeEnum.Ambient || !Shadows || ShadowTextureSize.Value != ShadowTextureSizeType.Value )
						skip = true;
					break;

				case nameof( ShadowContactLength ):
					if( Type.Value == TypeEnum.Ambient || !Shadows || !ShadowContact )
						skip = true;
					break;

				case nameof( ShadowStatic ):
					if( Type.Value == TypeEnum.Ambient || Type.Value == TypeEnum.Directional || !Shadows )
						skip = true;
					break;

				//case nameof( SourceRadius ):
				//	if( Type.Value == TypeEnum.Ambient || Type.Value == TypeEnum.Directional )
				//		skip = true;
				//	break;

				//case nameof( SourceAngle ):
				//	if( Type.Value == TypeEnum.Ambient || Type.Value == TypeEnum.Point || Type.Value == TypeEnum.Spotlight )
				//		skip = true;
				//	break;

				case nameof( Mask ):
					if( Type.Value == TypeEnum.Ambient )
						skip = true;
					break;

				case nameof( MaskTransform ):
					//case nameof( MaskScale ):
					if( Mask.Value == null || Type.Value != TypeEnum.Directional )
						skip = true;
					break;

				case nameof( FlareImage ):
					if( Type.Value == TypeEnum.Ambient )
						skip = true;
					break;

				case nameof( FlareBlending ):
				case nameof( FlareColor ):
				case nameof( FlarePosition ):
				case nameof( FlareSize ):
				case nameof( FlareSizeFadeByDistance ):
				case nameof( FlareDepthCheckOffset ):
					if( FlareImage.Value == null || Type.Value == TypeEnum.Ambient )
						skip = true;
					break;

					//case nameof( MaskMatrix ):
					//	if( Mask.Value == null )
					//		skip = true;
					//	break;
				}
			}
		}

		///// <summary>
		///// A value indicating whether the inclusion in the process of calculating a static lighting enabled.
		///// </summary>
		///// <remarks>
		///// Static lighting can be configured using the static lighting Manager (type Base\StaticLightingManager).
		///// </remarks>
		//[Description( "A value indicating whether the inclusion in the process of calculating a static lighting enabled. Static lighting can be configured using the static lighting Manager (type Base\\StaticLightingManager)." )]
		//[Category( "Light")]
		//[DefaultValue( false )]
		//public bool AllowStaticLighting
		//{
		//   get { return allowStaticLighting; }
		//   set
		//   {
		//      if( allowStaticLighting == value )
		//         return;
		//      allowStaticLighting = value;

		//      if( renderLight != null )
		//         renderLight.AllowStaticLighting = allowStaticLighting;
		//   }
		//}

		//!!!!!!
		///// <summary>
		///// Special option for developers that allows you to transmit additional information about the source in the shader.
		///// </summary>
		//[Description( "Special option for developers that allows you to transmit additional information about the source in the shader." )]
		//[Category( "Light" )]
		//[DefaultValue( typeof( Vec4 ), "0 0 0 0" )]
		//public Vec4 CustomShaderParameter
		//{
		//	get { return customShaderParameter; }
		//	set
		//	{
		//		if( customShaderParameter == value )
		//			return;
		//		customShaderParameter = value;

		//		if( renderLight != null )
		//			renderLight.CustomShaderParameter = customShaderParameter;
		//	}
		//}

		///// <summary>
		///// Path to a texture file with billboard image.
		///// </summary>
		//[Description( "Path to a texture file with billboard image." )]
		//[DefaultValue( "" )]//Base\\Types\\Lighting\\Sun\\SunDefault.png" )]//!!!!!текстуры в одно месло сложить? или в Textures\Sun billboards?
		//					//!!!!!![Editor( typeof( EditorTextureUITypeEditor ), typeof( UITypeEditor ) )]
		//public Reference<string> BillboardTexture
		//{
		//	get { return billboardTexture; }
		//	set
		//	{
		//		if( billboardTexture == value )
		//			return;
		//		billboardTexture = value;

		//		needRecreateBillboard = true;
		//	}
		//}

		///// <summary>
		///// The color multiplier of the billboard.
		///// </summary>
		//[Description( "The color multiplier of the billboard." )]
		//[DefaultValue( typeof( ColorValue ), "255 255 255" )]
		//[ColorValueNoAlphaChannel]
		//public Reference<ColorValue> BillboardColor
		//{
		//	get { return billboardColor; }
		//	set
		//	{
		//		if( billboardColor == value )
		//			return;
		//		billboardColor = value;
		//		//!!!!needUpdateMaterialParameters = true;
		//	}
		//}

		///// <summary>
		///// The brightness modifier of the billboard.
		///// </summary>
		//[Description( "The brightness modifier of the billboard." )]
		//[DefaultValue( 1.0 )]
		//[Editor( typeof( DoubleValueEditor ), typeof( UITypeEditor ) )]
		//[EditorLimitsRange( 0, 5 )]
		//public Reference<double> BillboardPower
		//{
		//	get { return billboardPower; }
		//	set
		//	{
		//		if( billboardPower == value )
		//			return;
		//		billboardPower = value;
		//		//!!!!needUpdateMaterialParameters = true;
		//	}
		//}

		////!!!!как быть с point и spot? указывать BillboardSizeType = ViewportHeight, WorldSize
		///// <summary>
		///// The size of the billboard.
		///// </summary>
		//[Description( "The size of the billboard." )]
		//[DefaultValue( 1.0 )]
		//[Editor( typeof( DoubleValueEditor ), typeof( UITypeEditor ) )]
		//[EditorLimitsRange( 0, 4 )]//!!!!!!тогда больше
		//public Reference<double> BillboardSize
		//{
		//	get { return billboardSize; }
		//	set
		//	{
		//		if( billboardSize == value )
		//			return;
		//		billboardSize = value;

		//		//!!!!!?
		//	}
		//}

		////!!!!read-only для Point и Spot
		///// <summary>
		///// The position of the billboard for directional light. If nonzero, the billboard is drawn at the specified point relative to the camera. Otherwise, the coordinates for rendering the billboard are determined depending on the direction of the object.
		///// </summary>
		//[Description( "The position of the billboard  for directional light. If nonzero, the billboard is drawn at the specified point relative to the camera. Otherwise, the coordinates for rendering the billboard are determined depending on the direction of the object." )]
		//[DefaultValue( typeof( Vec3 ), "0 0 0" )]
		//public Reference<Vec3> BillboardOverridePosition
		//{
		//	get { return billboardOverridePosition; }
		//	set { billboardOverridePosition = value; }
		//}

		//[Browsable( false )]
		//public int _InternalRenderSceneIndex
		//{
		//	get { return _internalRenderSceneIndex; }
		//	set { _internalRenderSceneIndex = value; }
		//}

		/// <summary>
		/// Returns 10 planes. Order: backward plane (0), far distance plane (1) and 8 side planes (2-9).
		/// </summary>
		[Browsable( false )]
		public Plane[] SpotlightClipPlanes
		{
			get
			{
				if( cachedSpotlightClipPlanes == null )
				{
					var cachedSpotlightClipPlanes2 = new Plane[ 10 ];

					var tr = Transform.Value;
					var position = tr.Position;
					var direction = tr.Rotation.GetForward();
					var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();

					//backward plane
					cachedSpotlightClipPlanes2[ 0 ] = Plane.FromPointAndNormal( position, -direction );

					//far plane
					cachedSpotlightClipPlanes2[ 1 ] = Plane.FromPointAndNormal( position + direction * attenuationFar, direction );

					//side planes
					{
						Matrix3 worldRotation = Quaternion.FromDirectionZAxisUp( direction ).ToMatrix3();
						Matrix3 outerAngleRotation = Matrix3.FromRotateByY( SpotlightOuterAngle.Value.InRadians() / 2 + Math.PI / 2 );

						for( int nAxisAngle = 0; nAxisAngle < 8; nAxisAngle++ )
						{
							double axisAngle = Math.PI * 2.0f * ( (float)nAxisAngle / 8 );

							Matrix3 m;
							Matrix3.FromRotateByX( axisAngle, out m );
							Matrix3 worldAxisRotation;
							Matrix3.Multiply( ref worldRotation, ref m, out worldAxisRotation );
							//Mat3 worldAxisRotation = worldRotation * Mat3.FromRotateByX( axisAngle );

							Matrix3 pointRotation;
							Matrix3.Multiply( ref worldAxisRotation, ref outerAngleRotation, out pointRotation );
							//Mat3 pointRotation = worldAxisRotation * outerAngleRotation;

							cachedSpotlightClipPlanes2[ nAxisAngle + 2 ] = Plane.FromPointAndNormal( position, pointRotation * Vector3.XAxis );
						}
					}

					cachedSpotlightClipPlanes = cachedSpotlightClipPlanes2;
				}

				return cachedSpotlightClipPlanes;
			}
		}

		ConvexPolyhedron MakeConvexPolyhedronForPointLight()
		{
			var tr = Transform.Value;
			var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();

			double radius = attenuationFar;
			radius /= Math.Cos( Math.PI * 2.0f / 32.0f );

			SimpleMeshGenerator.GenerateSphere( radius, 8, 8, false, out Vector3[] vertices, out int[] indices );

			for( int n = 0; n < vertices.Length; n++ )
				vertices[ n ] = tr.Position + vertices[ n ];

			return new ConvexPolyhedron( vertices, indices, 0.0001 );
		}

		ConvexPolyhedron MakeConvexPolyhedronForSpotlight()
		{
			double outerAngle = SpotlightOuterAngle.Value.InRadians();
			if( outerAngle < new Degree( 1 ).InRadians() )
				outerAngle = new Degree( 1 ).InRadians();
			if( outerAngle > new Degree( 179 ).InRadians() )
				outerAngle = new Degree( 179 ).InRadians();

			List<Vector3> vertices = new List<Vector3>( 10 );
			List<ConvexPolyhedron.Face> faces = new List<ConvexPolyhedron.Face>( 16 );

			var tr = Transform.Value;
			var attenuationFar = AttenuationFar * tr.Scale.MaxComponent();
			Matrix3 worldRotation = Quaternion.FromDirectionZAxisUp( tr.Rotation.GetForward() ).ToMatrix3();

			double sideAngle;
			{
				double radius = Math.Sin( outerAngle / 2 ) * attenuationFar;

				double l = Math.Sqrt( attenuationFar * attenuationFar - radius * radius );
				radius /= Math.Cos( Math.PI * 2 / 16 );

				sideAngle = Math.Atan( radius / l );
			}

			Vector3 farPoint;
			{
				Matrix3 pointRotation = worldRotation * Matrix3.FromRotateByY( outerAngle / 4 );
				Vector3 direction = pointRotation * Vector3.XAxis;

				Vector3 point = tr.Position + direction * attenuationFar;

				Plane plane = Plane.FromPointAndNormal( point, direction );
				Ray ray = new Ray( tr.Position, tr.Rotation.GetForward() * attenuationFar );

				plane.Intersects( ray, out double scale );
				farPoint = ray.GetPointOnRay( scale * 1.05f );
			}

			vertices.Add( tr.Position );
			vertices.Add( farPoint );

			for( int nAxisAngle = 0; nAxisAngle < 8; nAxisAngle++ )
			{
				double axisAngle = ( Math.PI * 2 ) * ( (float)nAxisAngle / 8 );

				Matrix3 worldAxisRotation = worldRotation * Matrix3.FromRotateByX( axisAngle );

				Plane sidePlane;
				{
					Matrix3 sideAngleRotation = Matrix3.FromRotateByY( sideAngle + Math.PI / 2 );
					Matrix3 pointRotation = worldAxisRotation * sideAngleRotation;
					sidePlane = Plane.FromPointAndNormal( tr.Position, pointRotation * Vector3.XAxis );
				}

				{
					Matrix3 pointRotation = worldAxisRotation * Matrix3.FromRotateByY( outerAngle / 4 );
					Vector3 direction = pointRotation * Vector3.XAxis;
					Vector3 point = tr.Position + direction * ( attenuationFar * 1.05f );

					Ray ray = new Ray( farPoint, point - farPoint );

					sidePlane.Intersects( ray, out double scale );
					Vector3 p = ray.GetPointOnRay( scale );

					vertices.Add( p );
				}
			}

			for( int n = 0; n < 8; n++ )
			{
				faces.Add( new ConvexPolyhedron.Face( 0, n + 2, ( n + 1 ) % 8 + 2 ) );
				faces.Add( new ConvexPolyhedron.Face( 1, ( n + 1 ) % 8 + 2, n + 2 ) );
			}

			return new ConvexPolyhedron( vertices.ToArray(), faces.ToArray(), .0001 );
		}

		/// <summary>
		/// Returns precalculated inclusive volume of the light. Its includes convex polyedron and bounding box.
		/// </summary>
		/// <returns></returns>
		public InclusiveVolumeData GetInclusiveVolume()
		{
			if( cachedInclusiveVolume == null )
			{
				InclusiveVolumeData cachedInclusiveVolume2 = null;

				ConvexPolyhedron polyhedron = null;
				switch( Type.Value )
				{
				case TypeEnum.Point:
					polyhedron = MakeConvexPolyhedronForPointLight();
					break;
				case TypeEnum.Spotlight:
					polyhedron = MakeConvexPolyhedronForSpotlight();
					break;
				}

				if( polyhedron != null )
				{
					cachedInclusiveVolume2 = new InclusiveVolumeData();
					cachedInclusiveVolume2.ConvexPolyhedron = polyhedron;
					cachedInclusiveVolume2.Bounds = new Bounds( polyhedron.Vertices[ 0 ] );
					for( int n = 1; n < polyhedron.Vertices.Length; n++ )
						cachedInclusiveVolume2.Bounds.Add( polyhedron.Vertices[ n ] );
				}

				cachedInclusiveVolume = cachedInclusiveVolume2;
			}
			return cachedInclusiveVolume;
		}

		//void UpdateCachedInclusiveVolume()
		//{
		//	cachedInclusiveVolume = null;
		//	switch( Type.Value )
		//	{
		//	case TypeEnum.Point:
		//		cachedInclusiveVolume = MakeConvexPolyhedronForPointLight();
		//		break;
		//	case TypeEnum.Spotlight:
		//		cachedInclusiveVolume = MakeConvexPolyhedronForSpotlight();
		//		break;
		//	}

		//	if( cachedInclusiveVolume != null )
		//	{
		//		cachedInclusiveVolumeBounds = new Bounds( cachedInclusiveVolume.Vertices[ 0 ] );
		//		for( int n = 1; n < cachedInclusiveVolume.Vertices.Length; n++ )
		//			cachedInclusiveVolumeBounds.Add( cachedInclusiveVolume.Vertices[ n ] );
		//	}
		//}

		//public bool GetInclusiveVolume( out ConvexPolyhedron volume, out Bounds bounds )
		//{
		//	if( cachedInclusiveVolume == null )
		//		UpdateCachedInclusiveVolume();
		//	if( cachedInclusiveVolume != null )
		//	{
		//		volume = cachedInclusiveVolume;
		//		bounds = cachedInclusiveVolumeBounds;
		//		return true;
		//	}
		//	volume = null;
		//	bounds = Bounds.Cleared;
		//	return false;
		//}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//rendering pipeline optimization
			var scene = ParentScene;//FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Add( this );
					if( Type.Value == TypeEnum.Directional )
						scene.CachedDirectionalLightsToFastFindByRenderingPipeline.AddWithCheckAlreadyContained( this );
				}
				else
				{
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Remove( this );
					scene.CachedDirectionalLightsToFastFindByRenderingPipeline.Remove( this );
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			AttenuationFar = 10;
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Light", Type.Value <= TypeEnum.Directional );
		}

		internal long GetUniqueIdentifierForStaticShadows()
		{
			if( !uniqueIdentifierForStaticShadows.HasValue )
			{
				lock( uniqueIdentifierLockForStaticShadows )
				{
					unchecked
					{
						uniqueIdentifierCounterForStaticShadows++;
						uniqueIdentifierForStaticShadows = uniqueIdentifierCounterForStaticShadows;
					}
				}

				//Log.Info( uniqueIdentifierForStaticShadows.Value.ToString() );

			}
			return uniqueIdentifierForStaticShadows.Value;
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			uniqueIdentifierForStaticShadows = null;
		}

		public void ResetStaticShadowsCache()
		{
			uniqueIdentifierForStaticShadows = null;
		}

		internal void RenderFlare( ViewportRenderingContext context, CanvasRenderer renderer, RenderingPipeline.RenderSceneData.LightItem lightItem, Viewport viewport, double intensity, bool occlusionDepthCheck )
		{
			var image = FlareImage.Value;
			if( image != null )
			{
				var lightPosition = lightItem.Position;

				if( viewport.CameraSettings.ProjectToScreenCoordinates( lightPosition, out var screenLightPosition ) )
				{
					var cameraPosition = viewport.CameraSettings.Position;
					var pipeline = context.RenderingPipeline;
					var minimumVisibleSizeOfObjects = pipeline.MinimumVisibleSizeOfObjects.Value;

					var flareVector = screenLightPosition - new Vector2( 0.5, 0.5 );
					var flarePosition = new Vector2( 0.5, 0.5 ) + flareVector * FlarePosition;

					var size = FlareSize.Value;
					if( FlareSizeFadeByDistance )
					{
						Vector3.Lerp( ref cameraPosition, ref lightPosition, FlarePosition, out var flarePosition3D );

						double distance = ( flarePosition3D - viewport.CameraSettings.Position ).Length();
						if( distance != 0 )
							size *= 1.0 / distance;
					}

					var flareSize = new Vector2( size.X * renderer.AspectRatioInv, size.Y );

					//cull by size
					var flareSizeInPixels = flareSize * viewport.SizeInPixels.ToVector2();
					if( flareSizeInPixels.MaxComponent() >= minimumVisibleSizeOfObjects )
					{
						var rectangle = new Rectangle( flarePosition - flareSize * 0.5, flarePosition + flareSize * 0.5 );
						var flareColor = Color.Value * FlareColor.Value.ToColorValue();// * new ColorValue( 1, 1, 1, intensity );
						flareColor.Alpha *= (float)intensity;
						if( flareColor.Alpha > 0 )
						{
							if( occlusionDepthCheck )
							{
								//!!!!
								var screenSize = flareSize.Y / 20;

								var compareDepth = ( cameraPosition - lightPosition ).Length() - FlareDepthCheckOffset.Value;
								renderer.PushOcclusionDepthCheck( screenLightPosition.ToVector2F(), (float)screenSize, (float)compareDepth );
							}

							renderer.PushBlendingType( FlareBlending.Value );
							renderer.AddQuad( rectangle, new RectangleF( 0, 0, 1, 1 ), image, flareColor, true );
							renderer.PopBlendingType();

							if( occlusionDepthCheck )
								renderer.PopOcclusionDepthCheck();
						}
					}
				}
			}
		}
	}
}
