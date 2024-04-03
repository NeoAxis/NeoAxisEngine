// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// The basic rendering pipeline.
	/// </summary>
	public partial class RenderingPipeline_Basic : RenderingPipeline
	{
		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		public static double GlobalTextureQuality = 1;
		public static double GlobalShadowQuality = 1;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum DebugModeEnum
		{
			None = 0,
			Wireframe = 1,
			Geometry = 2,
			Surface = 3,
			BaseColor = 4,
			Metallic = 5,
			Roughness = 6,
			Reflectance = 7,
			Emissive = 8,
			Normal = 9,
			SubsurfaceColor = 10,
			[DisplayNameEnum( "Texture Coordinate 0" )]
			TextureCoordinate0 = 11,
			[DisplayNameEnum( "Texture Coordinate 1" )]
			TextureCoordinate1 = 12,
			[DisplayNameEnum( "Texture Coordinate 2" )]
			TextureCoordinate2 = 13,
			OcclusionCullingBuffer = 14,

			//AmbientOcclusionMap,
			//SpecialF0,
		}

		/// <summary>
		/// Specifies the debug mode.
		/// </summary>
		[DefaultValue( DebugModeEnum.None )]
		[Category( "General" )]
		public Reference<DebugModeEnum> DebugMode
		{
			get { if( _debugMode.BeginGet() ) DebugMode = _debugMode.Get( this ); return _debugMode.value; }
			set { if( _debugMode.BeginSet( this, ref value ) ) { try { DebugModeChanged?.Invoke( this ); } finally { _debugMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugMode"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugModeChanged;
		ReferenceField<DebugModeEnum> _debugMode = DebugModeEnum.None;

		/////////////////////////////////////////

		//!!!!defaults

		/// <summary>
		/// Whether to calculate the indirect lighting. Alternatively, you can use screen space ambient occlusion by using AmbientOcclusion effect component.
		/// </summary>
		[Category( "Global Illumination" )]
		[DefaultValue( false )]
		public Reference<bool> IndirectLighting
		{
			get { if( _indirectLighting.BeginGet() ) IndirectLighting = _indirectLighting.Get( this ); return _indirectLighting.value; }
			set { if( _indirectLighting.BeginSet( this, ref value ) ) { try { IndirectLightingChanged?.Invoke( this ); } finally { _indirectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndirectLighting"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> IndirectLightingChanged;
		ReferenceField<bool> _indirectLighting = false;

		/// <summary>
		/// Whether to calculate reflections. Alternatively, you can use screen space reflections by using Reflection effect component.
		/// </summary>
		[Category( "Global Illumination" )]
		[DefaultValue( false )]
		public Reference<bool> Reflection
		{
			get { if( _reflection.BeginGet() ) Reflection = _reflection.Get( this ); return _reflection.value; }
			set { if( _reflection.BeginSet( this, ref value ) ) { try { ReflectionChanged?.Invoke( this ); } finally { _reflection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Reflection"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ReflectionChanged;
		ReferenceField<bool> _reflection = false;

		/// <summary>
		/// Maximum distance of the global illumination.
		/// </summary>
		[Category( "Global Illumination" )]
		[DefaultValue( 70.0 )]
		[DisplayName( "GI Distance" )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> GIDistance
		{
			get { if( _giDistance.BeginGet() ) GIDistance = _giDistance.Get( this ); return _giDistance.value; }
			set { if( _giDistance.BeginSet( this, ref value ) ) { try { GIDistanceChanged?.Invoke( this ); } finally { _giDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GIDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GIDistanceChanged;
		ReferenceField<double> _giDistance = 70.0;

		public enum GIGridSizeEnum
		{
			_64,
			_128,
			_256,
		}

		/// <summary>
		/// The size of the 3D grid for the global illumination calculation.
		/// </summary>
		[Category( "Global Illumination" )]
		[DefaultValue( GIGridSizeEnum._128 )]
		[DisplayName( "GI Grid Size" )]
		public Reference<GIGridSizeEnum> GIGridSize
		{
			get { if( _giGridSize.BeginGet() ) GIGridSize = _giGridSize.Get( this ); return _giGridSize.value; }
			set { if( _giGridSize.BeginSet( this, ref value ) ) { try { GIGridSizeChanged?.Invoke( this ); } finally { _giGridSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GIGridSize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GIGridSizeChanged;
		ReferenceField<GIGridSizeEnum> _giGridSize = GIGridSizeEnum._128;

		/// <summary>
		/// The number of cascades used for the global illumination calculation.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 1, 6 )]
		[Category( "Global Illumination" )]
		[DisplayName( "GI Cascades" )]
		public Reference<int> GICascades
		{
			get { if( _giCascades.BeginGet() ) GICascades = _giCascades.Get( this ); return _giCascades.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( value > 4 )
					value = new Reference<int>( 6, value.GetByReference );
				if( _giCascades.BeginSet( this, ref value ) ) { try { GICascadesChanged?.Invoke( this ); } finally { _giCascades.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="GICascades"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GICascadesChanged;
		ReferenceField<int> _giCascades = 4;

		/// <summary>
		/// Defines grid cascades distribution of the global illumination.
		/// </summary>
		[DefaultValue( 2 )]//3.5/*2.4*/ )]
		[Range( 1, 10 )]
		[Category( "Global Illumination" )]
		[DisplayName( "GI Cascade Distribution" )]
		public Reference<double> GICascadeDistribution
		{
			get { if( _giCascadeDistribution.BeginGet() ) GICascadeDistribution = _giCascadeDistribution.Get( this ); return _giCascadeDistribution.value; }
			set { if( _giCascadeDistribution.BeginSet( this, ref value ) ) { try { GICascadeDistributionChanged?.Invoke( this ); } finally { _giCascadeDistribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GICascadeDistribution"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GICascadeDistributionChanged;
		ReferenceField<double> _giCascadeDistribution = 2;//3.5;//2.4;

		/// <summary>
		/// Whether to visualize grid cascades of the global illumination.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Global Illumination" )]
		[DisplayName( "GI Cascade Visualize" )]
		public Reference<bool> GICascadeVisualize
		{
			get { if( _giCascadeVisualize.BeginGet() ) GICascadeVisualize = _giCascadeVisualize.Get( this ); return _giCascadeVisualize.value; }
			set { if( _giCascadeVisualize.BeginSet( this, ref value ) ) { try { GICascadeVisualizeChanged?.Invoke( this ); } finally { _giCascadeVisualize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GICascadeVisualize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GICascadeVisualizeChanged;
		ReferenceField<bool> _giCascadeVisualize = false;

		//!!!!
		[Category( "Global Illumination" )]
		[Range( 0, 2 )]
		[DefaultValue( 1.0 )]
		[DisplayName( "GI Voxelization Conservative" )]
		public Reference<double> GIVoxelizationConservative
		{
			get { if( _giVoxelizationConservative.BeginGet() ) GIVoxelizationConservative = _giVoxelizationConservative.Get( this ); return _giVoxelizationConservative.value; }
			set { if( _giVoxelizationConservative.BeginSet( this, ref value ) ) { try { GIVoxelizationConservativeChanged?.Invoke( this ); } finally { _giVoxelizationConservative.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GIVoxelizationConservative"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> GIVoxelizationConservativeChanged;
		ReferenceField<double> _giVoxelizationConservative = 1.0;

		/////////////////////////////////////////

		/// <summary>
		/// Whether shadows are enabled.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shadows" )]
		public Reference<bool> Shadows
		{
			get { if( _shadows.BeginGet() ) Shadows = _shadows.Get( this ); return _shadows.value; }
			set { if( _shadows.BeginSet( this, ref value ) ) { try { ShadowsChanged?.Invoke( this ); } finally { _shadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shadows"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowsChanged;
		ReferenceField<bool> _shadows = true;

		//public enum ShadowQualityEnum
		//{
		//	Low,
		//	High,
		//}

		///// <summary>
		///// Specifies the quality of shadows.
		///// </summary>
		//[DefaultValue( ShadowQualityEnum.High )]
		//[Category( "Shadows" )]
		//public Reference<ShadowQualityEnum> ShadowQuality
		//{
		//	get { if( _shadowQuality.BeginGet() ) ShadowQuality = _shadowQuality.Get( this ); return _shadowQuality.value; }
		//	set { if( _shadowQuality.BeginSet( this, ref value ) ) { try { ShadowQualityChanged?.Invoke( this ); } finally { _shadowQuality.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowQuality"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> ShadowQualityChanged;
		//ReferenceField<ShadowQualityEnum> _shadowQuality = ShadowQualityEnum.High;

		/// <summary>
		/// The intensity of the shadows. The Light component also has a Shadow Intensity parameter to configure per light.
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
		public event Action<RenderingPipeline_Basic> ShadowIntensityChanged;
		ReferenceField<double> _shadowIntensity = 1.0;

		///// <summary>
		///// The softness of the shadows. The Light component also has a Shadow Softness parameter to configure per light.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//[Category( "Shadows" )]
		//public Reference<double> ShadowSoftness
		//{
		//	get { if( _shadowSoftness.BeginGet() ) ShadowSoftness = _shadowSoftness.Get( this ); return _shadowSoftness.value; }
		//	set { if( _shadowSoftness.BeginSet( this, ref value ) ) { try { ShadowSoftnessChanged?.Invoke( this ); } finally { _shadowSoftness.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowSoftness"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> ShadowSoftnessChanged;
		//ReferenceField<double> _shadowSoftness = 1.0;

		/// <summary>
		/// Rendering range of the shadows for Directional lights.
		/// </summary>
		[DefaultValue( 130.0 )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowDirectionalDistance
		{
			get { if( _shadowDirectionalDistance.BeginGet() ) ShadowDirectionalDistance = _shadowDirectionalDistance.Get( this ); return _shadowDirectionalDistance.value; }
			set { if( _shadowDirectionalDistance.BeginSet( this, ref value ) ) { try { ShadowDirectionalDistanceChanged?.Invoke( this ); } finally { _shadowDirectionalDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalDistanceChanged;
		ReferenceField<double> _shadowDirectionalDistance = 130;

		//!!!!not supported on current pipeline

		/// <summary>
		/// Maximum number of Directional Lights that can cast shadows. The current pipeline is not support more than 1 directional light shadow casters.
		/// </summary>
		[Category( "Shadows" )]
		public int ShadowDirectionalLightMaxCount
		{
			get { return 1; }
		}

		///// <summary>
		///// Maximum number of Directional Lights that can cast shadows.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Range( 0, 4 )]
		//[Category( "Shadows" )]
		//public Reference<int> ShadowDirectionalLightMaxCount
		//{
		//	get { if( _shadowDirectionalLightMaxCount.BeginGet() ) ShadowDirectionalLightMaxCount = _shadowDirectionalLightMaxCount.Get( this ); return _shadowDirectionalLightMaxCount.value; }
		//	set { if( _shadowDirectionalLightMaxCount.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightMaxCountChanged?.Invoke( this ); } finally { _shadowDirectionalLightMaxCount.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowDirectionalLightMaxCount"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> ShadowDirectionalLightMaxCountChanged;
		//ReferenceField<int> _shadowDirectionalLightMaxCount = 1;

		/// <summary>
		/// The size of a shadow texture for Directional Lights.
		/// </summary>
		[DefaultValue( ShadowTextureSizeEnum._4096 )]//_2048 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSizeEnum> ShadowDirectionalLightTextureSize
		{
			get { if( _shadowDirectionalLightTextureSize.BeginGet() ) ShadowDirectionalLightTextureSize = _shadowDirectionalLightTextureSize.Get( this ); return _shadowDirectionalLightTextureSize.value; }
			set { if( _shadowDirectionalLightTextureSize.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightTextureSizeChanged?.Invoke( this ); } finally { _shadowDirectionalLightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightTextureSize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalLightTextureSizeChanged;
		ReferenceField<ShadowTextureSizeEnum> _shadowDirectionalLightTextureSize = ShadowTextureSizeEnum._4096;// _2048;

		/// <summary>
		/// The number of cascades used for Directional Lights.
		/// </summary>
		[DefaultValue( 2 )] //3 )]
		[Range( 1, 4 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowDirectionalLightCascades
		{
			get { if( _shadowDirectionalLightCascades.BeginGet() ) ShadowDirectionalLightCascades = _shadowDirectionalLightCascades.Get( this ); return _shadowDirectionalLightCascades.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( value > 4 )
					value = new Reference<int>( 4, value.GetByReference );
				if( _shadowDirectionalLightCascades.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightCascadesChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascades.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascades"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalLightCascadesChanged;
		ReferenceField<int> _shadowDirectionalLightCascades = 2;//3;

		/// <summary>
		/// Defines shadow cascades distribution for Directional Lights. The distance of the current cascade, multiplied by this value gives distance to the next cascade.
		/// </summary>
		[DefaultValue( 3.5/*2.4*/ )]
		[Category( "Shadows" )]
		[Range( 1, 10 )]
		public Reference<double> ShadowDirectionalLightCascadeDistribution
		{
			get { if( _shadowDirectionalLightCascadeDistribution.BeginGet() ) ShadowDirectionalLightCascadeDistribution = _shadowDirectionalLightCascadeDistribution.Get( this ); return _shadowDirectionalLightCascadeDistribution.value; }
			set { if( _shadowDirectionalLightCascadeDistribution.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightCascadeDistributionChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascadeDistribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascadeDistribution"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalLightCascadeDistributionChanged;
		ReferenceField<double> _shadowDirectionalLightCascadeDistribution = 3.5;//2.4;

		///// <summary>
		///// Defines shadow cascades overlapping for Directional Lights.
		///// </summary>
		//[DefaultValue( 1.2 )]
		//[Category( "Shadows" )]
		//[Range( 1, 1.5 )]
		//public Reference<double> ShadowDirectionalLightCascadeOverlapping
		//{
		//	get { if( _shadowDirectionalLightCascadeOverlapping.BeginGet() ) ShadowDirectionalLightCascadeOverlapping = _shadowDirectionalLightCascadeOverlapping.Get( this ); return _shadowDirectionalLightCascadeOverlapping.value; }
		//	set { if( _shadowDirectionalLightCascadeOverlapping.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightCascadeOverlappingChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascadeOverlapping.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowDirectionalLightCascadeOverlapping"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> ShadowDirectionalLightCascadeOverlappingChanged;
		//ReferenceField<double> _shadowDirectionalLightCascadeOverlapping = 1.2;

		/// <summary>
		/// Whether to visualize shadow cascades for Directional Lights.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Shadows" )]
		public Reference<bool> ShadowDirectionalLightCascadeVisualize
		{
			get { if( _shadowDirectionalLightCascadeVisualize.BeginGet() ) ShadowDirectionalLightCascadeVisualize = _shadowDirectionalLightCascadeVisualize.Get( this ); return _shadowDirectionalLightCascadeVisualize.value; }
			set { if( _shadowDirectionalLightCascadeVisualize.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightCascadeVisualizeChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascadeVisualize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascadeVisualize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalLightCascadeVisualizeChanged;
		ReferenceField<bool> _shadowDirectionalLightCascadeVisualize = false;

		/// <summary>
		/// Maximum distance to camera where shadows from Directional Lights will be cast.
		/// </summary>
		[DefaultValue( 1000.0 )]//issues on mobile. byte4. 2000.0 )]
		[Category( "Shadows" )]
		[Range( 10, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ShadowDirectionalLightExtrusionDistance
		{
			get { if( _shadowDirectionalLightExtrusionDistance.BeginGet() ) ShadowDirectionalLightExtrusionDistance = _shadowDirectionalLightExtrusionDistance.Get( this ); return _shadowDirectionalLightExtrusionDistance.value; }
			set { if( _shadowDirectionalLightExtrusionDistance.BeginSet( this, ref value ) ) { try { ShadowDirectionalLightExtrusionDistanceChanged?.Invoke( this ); } finally { _shadowDirectionalLightExtrusionDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightExtrusionDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowDirectionalLightExtrusionDistanceChanged;
		ReferenceField<double> _shadowDirectionalLightExtrusionDistance = 1000;//2000;

		/// <summary>
		/// Rendering range of the shadows for Point and Spotlights.
		/// </summary>
		[DefaultValue( 130.0 )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowPointSpotlightDistance
		{
			get { if( _shadowPointSpotlightDistance.BeginGet() ) ShadowPointSpotlightDistance = _shadowPointSpotlightDistance.Get( this ); return _shadowPointSpotlightDistance.value; }
			set { if( _shadowPointSpotlightDistance.BeginSet( this, ref value ) ) { try { ShadowPointSpotlightDistanceChanged?.Invoke( this ); } finally { _shadowPointSpotlightDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowPointSpotlightDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowPointSpotlightDistanceChanged;
		ReferenceField<double> _shadowPointSpotlightDistance = 130;

		/// <summary>
		/// Maximum number of point lights, that can cast shadows.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 0, 16 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowPointLightMaxCount
		{
			get { if( _shadowPointLightMaxCount.BeginGet() ) ShadowPointLightMaxCount = _shadowPointLightMaxCount.Get( this ); return _shadowPointLightMaxCount.value; }
			set { if( _shadowPointLightMaxCount.BeginSet( this, ref value ) ) { try { ShadowPointLightMaxCountChanged?.Invoke( this ); } finally { _shadowPointLightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowPointLightMaxCount"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowPointLightMaxCountChanged;
		ReferenceField<int> _shadowPointLightMaxCount = 4;

		/// <summary>
		/// The size of a shadow texture for point lights.
		/// </summary>
		[DefaultValue( ShadowTextureSizeEnum._512 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSizeEnum> ShadowPointLightTextureSize
		{
			get { if( _shadowPointLightTextureSize.BeginGet() ) ShadowPointLightTextureSize = _shadowPointLightTextureSize.Get( this ); return _shadowPointLightTextureSize.value; }
			set { if( _shadowPointLightTextureSize.BeginSet( this, ref value ) ) { try { ShadowPointLightTextureSizeChanged?.Invoke( this ); } finally { _shadowPointLightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowPointLightTextureSize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowPointLightTextureSizeChanged;
		ReferenceField<ShadowTextureSizeEnum> _shadowPointLightTextureSize = ShadowTextureSizeEnum._512;

		/// <summary>
		/// Maximum number of spotlights, that can cast shadows.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 0, 16 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowSpotlightMaxCount
		{
			get { if( _shadowSpotlightMaxCount.BeginGet() ) ShadowSpotlightMaxCount = _shadowSpotlightMaxCount.Get( this ); return _shadowSpotlightMaxCount.value; }
			set { if( _shadowSpotlightMaxCount.BeginSet( this, ref value ) ) { try { ShadowSpotlightMaxCountChanged?.Invoke( this ); } finally { _shadowSpotlightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowSpotlightMaxCount"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowSpotlightMaxCountChanged;
		ReferenceField<int> _shadowSpotlightMaxCount = 4;

		/// <summary>
		/// The size of shadow texture for spotlights.
		/// </summary>
		[DefaultValue( ShadowTextureSizeEnum._1024 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSizeEnum> ShadowSpotlightTextureSize
		{
			get { if( _shadowSpotlightTextureSize.BeginGet() ) ShadowSpotlightTextureSize = _shadowSpotlightTextureSize.Get( this ); return _shadowSpotlightTextureSize.value; }
			set { if( _shadowSpotlightTextureSize.BeginSet( this, ref value ) ) { try { ShadowSpotlightTextureSizeChanged?.Invoke( this ); } finally { _shadowSpotlightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowSpotlightTextureSize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowSpotlightTextureSizeChanged;
		ReferenceField<ShadowTextureSizeEnum> _shadowSpotlightTextureSize = ShadowTextureSizeEnum._1024;

		/// <summary>
		/// The multiplier of shadow visibility distance depending of object visibility distance.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowObjectVisibilityDistanceFactor
		{
			get { if( _shadowObjectVisibilityDistanceFactor.BeginGet() ) ShadowObjectVisibilityDistanceFactor = _shadowObjectVisibilityDistanceFactor.Get( this ); return _shadowObjectVisibilityDistanceFactor.value; }
			set { if( _shadowObjectVisibilityDistanceFactor.BeginSet( this, ref value ) ) { try { ShadowObjectVisibilityDistanceFactorChanged?.Invoke( this ); } finally { _shadowObjectVisibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowObjectVisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowObjectVisibilityDistanceFactorChanged;
		ReferenceField<double> _shadowObjectVisibilityDistanceFactor = 1.0;

		/// <summary>
		/// The multiplier of OpacityMaskThreshold parameter of materials when user for shadow caster generation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowMaterialOpacityMaskThresholdFactor
		{
			get { if( _shadowMaterialOpacityMaskThresholdFactor.BeginGet() ) ShadowMaterialOpacityMaskThresholdFactor = _shadowMaterialOpacityMaskThresholdFactor.Get( this ); return _shadowMaterialOpacityMaskThresholdFactor.value; }
			set { if( _shadowMaterialOpacityMaskThresholdFactor.BeginSet( this, ref value ) ) { try { ShadowMaterialOpacityMaskThresholdFactorChanged?.Invoke( this ); } finally { _shadowMaterialOpacityMaskThresholdFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowMaterialOpacityMaskThresholdFactor"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowMaterialOpacityMaskThresholdFactorChanged;
		ReferenceField<double> _shadowMaterialOpacityMaskThresholdFactor = 1.0;

		/// <summary>
		/// Whether to enable the static shadows optimization. Use Shadow Static property of the Light component to configure static shadows.
		/// </summary>
		[Category( "Shadows" )]
		[DefaultValue( true )]
		public Reference<bool> ShadowStatic
		{
			get { if( _shadowStatic.BeginGet() ) ShadowStatic = _shadowStatic.Get( this ); return _shadowStatic.value; }
			set { if( _shadowStatic.BeginSet( this, ref value ) ) { try { ShadowStaticChanged?.Invoke( this ); } finally { _shadowStatic.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowStatic"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ShadowStaticChanged;
		ReferenceField<bool> _shadowStatic = true;

		/////////////////////////////////////////

		/// <summary>
		/// Maximal vilibility distance for spotlight and point lights.
		/// </summary>
		[Category( "Lights" )]
		[DefaultValue( 100000.0 )]
		[Range( 10, 100000, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> LightMaxDistance
		{
			get { if( _lightMaxDistance.BeginGet() ) LightMaxDistance = _lightMaxDistance.Get( this ); return _lightMaxDistance.value; }
			set { if( _lightMaxDistance.BeginSet( this, ref value ) ) { try { LightMaxDistanceChanged?.Invoke( this ); } finally { _lightMaxDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMaxDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> LightMaxDistanceChanged;
		ReferenceField<double> _lightMaxDistance = 100000.0;

		/// <summary>
		/// The max amount of light sources to draw.
		/// </summary>
		[Category( "Lights" )]
		[DefaultValue( 1024 )]
		[Range( 10, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> LightMaxCount
		{
			get { if( _lightMaxCount.BeginGet() ) LightMaxCount = _lightMaxCount.Get( this ); return _lightMaxCount.value; }
			set { if( _lightMaxCount.BeginSet( this, ref value ) ) { try { LightMaxCountChanged?.Invoke( this ); } finally { _lightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMaxCount"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> LightMaxCountChanged;
		ReferenceField<int> _lightMaxCount = 1024;

		/// <summary>
		/// Whether to use an acceleration grid for lights rendering optimization. The grid is disabled on limited devices in Auto mode.
		/// </summary>
		/// <remarks>You can visualize the light grid by means Show Render Target screen effect, select Texture = LightGrid.</remarks>
		[Category( "Lights" )]
		[DefaultValue( AutoTrueFalse.Auto )]
		public Reference<AutoTrueFalse> LightGrid
		{
			get { if( _lightGrid.BeginGet() ) LightGrid = _lightGrid.Get( this ); return _lightGrid.value; }
			set { if( _lightGrid.BeginSet( this, ref value ) ) { try { LightGridChanged?.Invoke( this ); } finally { _lightGrid.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightGrid"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> LightGridChanged;
		ReferenceField<AutoTrueFalse> _lightGrid = AutoTrueFalse.Auto;

		public enum LightGridResolutionEnum
		{
			//_64,
			_128,
			_256,
			_512,
			_1024,
		}

		/// <summary>
		/// The size of light grid. The light grid is a 3D texture with sizes LightGridResolution * LightGridResolution * 8 * sizeof( Vector4F ).
		/// </summary>
		[Category( "Lights" )]
		[DefaultValue( LightGridResolutionEnum._256 )]
		public Reference<LightGridResolutionEnum> LightGridResolution
		{
			get { if( _lightGridResolution.BeginGet() ) LightGridResolution = _lightGridResolution.Get( this ); return _lightGridResolution.value; }
			set { if( _lightGridResolution.BeginSet( this, ref value ) ) { try { LightGridResolutionChanged?.Invoke( this ); } finally { _lightGridResolution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightGridResolution"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> LightGridResolutionChanged;
		ReferenceField<LightGridResolutionEnum> _lightGridResolution = LightGridResolutionEnum._256;

		/////////////////////////////////////////

		/// <summary>
		/// The maximal number of iterations for the displacement mapping.
		/// </summary>
		[DefaultValue( 24 )]
		[Category( "Displacement Mapping" )]
		[Range( 8, 32 )]
		public Reference<int> DisplacementMappingMaxSteps
		{
			get { if( _displacementMappingMaxSteps.BeginGet() ) DisplacementMappingMaxSteps = _displacementMappingMaxSteps.Get( this ); return _displacementMappingMaxSteps.value; }
			set { if( _displacementMappingMaxSteps.BeginSet( this, ref value ) ) { try { DisplacementMappingMaxStepsChanged?.Invoke( this ); } finally { _displacementMappingMaxSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementMappingMaxSteps"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DisplacementMappingMaxStepsChanged;
		ReferenceField<int> _displacementMappingMaxSteps = 24;

		/// <summary>
		/// The height multiplier for the displacement mapping.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Displacement Mapping" )]
		[Range( 0, 2 )]
		public Reference<double> DisplacementMappingScale
		{
			get { if( _displacementMappingScale.BeginGet() ) DisplacementMappingScale = _displacementMappingScale.Get( this ); return _displacementMappingScale.value; }
			set { if( _displacementMappingScale.BeginSet( this, ref value ) ) { try { DisplacementMappingScaleChanged?.Invoke( this ); } finally { _displacementMappingScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementMappingScale"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DisplacementMappingScaleChanged;
		ReferenceField<double> _displacementMappingScale = 1.0;

		/////////////////////////////////////////

		///// <summary>
		///// Maximum distance of the tessellation effect.
		///// </summary>
		//[Category( "Tesselation" )]
		//[DefaultValue( 10.0 )]
		//[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> TessellationDistance
		//{
		//	get { if( _tessellationDistance.BeginGet() ) TessellationDistance = _tessellationDistance.Get( this ); return _tessellationDistance.value; }
		//	set { if( _tessellationDistance.BeginSet( this, ref value ) ) { try { TessellationDistanceChanged?.Invoke( this ); } finally { _tessellationDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TessellationDistance"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> TessellationDistanceChanged;
		//ReferenceField<double> _tessellationDistance = 10.0;

		/// <summary>
		/// The quality multiplier of the tessellation.
		/// </summary>
		[Category( "Tesselation" )]
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		public Reference<double> TessellationQuality
		{
			get { if( _tessellationQuality.BeginGet() ) TessellationQuality = _tessellationQuality.Get( this ); return _tessellationQuality.value; }
			set { if( _tessellationQuality.BeginSet( this, ref value ) ) { try { TessellationQualityChanged?.Invoke( this ); } finally { _tessellationQuality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TessellationQuality"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> TessellationQualityChanged;
		ReferenceField<double> _tessellationQuality = 1.0;

		/////////////////////////////////////////

		/// <summary>
		/// The intesity of the technique to remove texture tiling.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Materials" )]
		[Range( 0, 1 )]
		public Reference<double> RemoveTextureTiling
		{
			get { if( _removeTextureTiling.BeginGet() ) RemoveTextureTiling = _removeTextureTiling.Get( this ); return _removeTextureTiling.value; }
			set { if( _removeTextureTiling.BeginSet( this, ref value ) ) { try { RemoveTextureTilingChanged?.Invoke( this ); } finally { _removeTextureTiling.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RemoveTextureTiling"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> RemoveTextureTilingChanged;
		ReferenceField<double> _removeTextureTiling = 1.0;

		/// <summary>
		/// Whether to provide color and depth data for transparent materials. It need to work for soft particles and refraction effects. When Auto mode is enabled, the mode is disabled on mobile devices.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "Materials" )]
		public Reference<AutoTrueFalse> ProvideColorDepthTextureCopy
		{
			get { if( _provideColorDepthTextureCopy.BeginGet() ) ProvideColorDepthTextureCopy = _provideColorDepthTextureCopy.Get( this ); return _provideColorDepthTextureCopy.value; }
			set { if( _provideColorDepthTextureCopy.BeginSet( this, ref value ) ) { try { ProvideColorDepthTextureCopyChanged?.Invoke( this ); } finally { _provideColorDepthTextureCopy.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProvideColorDepthTextureCopy"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> ProvideColorDepthTextureCopyChanged;
		ReferenceField<AutoTrueFalse> _provideColorDepthTextureCopy = AutoTrueFalse.Auto;

		///// <summary>
		///// Whether to allow Soft Particles technique. When Auto mode is enabled, soft particles are disabled on mobile devices.
		///// </summary>
		//[DefaultValue( AutoTrueFalse.Auto )]
		//[Category( "Materials" )]
		//public Reference<AutoTrueFalse> SoftParticles
		//{
		//	get { if( _softParticles.BeginGet() ) SoftParticles = _softParticles.Get( this ); return _softParticles.value; }
		//	set { if( _softParticles.BeginSet( this, ref value ) ) { try { SoftParticlesChanged?.Invoke( this ); } finally { _softParticles.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SoftParticles"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> SoftParticlesChanged;
		//ReferenceField<AutoTrueFalse> _softParticles = AutoTrueFalse.Auto;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to use the software occlusion culling buffer to skip invisible objects on the screen.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Occlusion Culling" )]
		public Reference<bool> OcclusionCullingBufferScene
		{
			get { if( _occlusionCullingBufferScene.BeginGet() ) OcclusionCullingBufferScene = _occlusionCullingBufferScene.Get( this ); return _occlusionCullingBufferScene.value; }
			set { if( _occlusionCullingBufferScene.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferSceneChanged?.Invoke( this ); } finally { _occlusionCullingBufferScene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferScene"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferSceneChanged;
		ReferenceField<bool> _occlusionCullingBufferScene = true;

		/// <summary>
		/// Whether to use the software occlusion culling buffer to skip invisible objects for directional light shadows.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Occlusion Culling" )]
		public Reference<bool> OcclusionCullingBufferDirectionalLight
		{
			get { if( _occlusionCullingBufferDirectionalLight.BeginGet() ) OcclusionCullingBufferDirectionalLight = _occlusionCullingBufferDirectionalLight.Get( this ); return _occlusionCullingBufferDirectionalLight.value; }
			set { if( _occlusionCullingBufferDirectionalLight.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferDirectionalLightChanged?.Invoke( this ); } finally { _occlusionCullingBufferDirectionalLight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferDirectionalLight"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferDirectionalLightChanged;
		ReferenceField<bool> _occlusionCullingBufferDirectionalLight = true;

		/// <summary>
		/// The height of the occlusion culling buffer in pixels.
		/// </summary>
		[DefaultValue( 600 )]//800 )] //400
		[Category( "Occlusion Culling" )]
		public Reference<int> OcclusionCullingBufferSize
		{
			get { if( _occlusionCullingBufferSize.BeginGet() ) OcclusionCullingBufferSize = _occlusionCullingBufferSize.Get( this ); return _occlusionCullingBufferSize.value; }
			set { if( _occlusionCullingBufferSize.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferSizeChanged?.Invoke( this ); } finally { _occlusionCullingBufferSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferSize"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferSizeChanged;
		ReferenceField<int> _occlusionCullingBufferSize = 600;//800;//400;

		/// <summary>
		/// Whether to cull octree nodes by the occlusion culling buffer.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Occlusion Culling" )]
		public Reference<bool> OcclusionCullingBufferCullNodes
		{
			get { if( _occlusionCullingBufferCullNodes.BeginGet() ) OcclusionCullingBufferCullNodes = _occlusionCullingBufferCullNodes.Get( this ); return _occlusionCullingBufferCullNodes.value; }
			set { if( _occlusionCullingBufferCullNodes.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferCullNodesChanged?.Invoke( this ); } finally { _occlusionCullingBufferCullNodes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferCullNodes"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferCullNodesChanged;
		ReferenceField<bool> _occlusionCullingBufferCullNodes = true;

		/// <summary>
		/// Whether to cull scene objects by the occlusion culling buffer.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Occlusion Culling" )]
		public Reference<bool> OcclusionCullingBufferCullObjects
		{
			get { if( _occlusionCullingBufferCullObjects.BeginGet() ) OcclusionCullingBufferCullObjects = _occlusionCullingBufferCullObjects.Get( this ); return _occlusionCullingBufferCullObjects.value; }
			set { if( _occlusionCullingBufferCullObjects.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferCullObjectsChanged?.Invoke( this ); } finally { _occlusionCullingBufferCullObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferCullObjects"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferCullObjectsChanged;
		ReferenceField<bool> _occlusionCullingBufferCullObjects = true;

		/// <summary>
		/// The maximal amount of occluders can be rendered for the frame.
		/// </summary>
		[DefaultValue( 300 )]
		[Category( "Occlusion Culling" )]
		public Reference<int> OcclusionCullingBufferMaxOccluders
		{
			get { if( _occlusionCullingBufferMaxOccluders.BeginGet() ) OcclusionCullingBufferMaxOccluders = _occlusionCullingBufferMaxOccluders.Get( this ); return _occlusionCullingBufferMaxOccluders.value; }
			set { if( _occlusionCullingBufferMaxOccluders.BeginSet( this, ref value ) ) { try { OcclusionCullingBufferMaxOccludersChanged?.Invoke( this ); } finally { _occlusionCullingBufferMaxOccluders.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OcclusionCullingBufferMaxOccluders"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> OcclusionCullingBufferMaxOccludersChanged;
		ReferenceField<int> _occlusionCullingBufferMaxOccluders = 300;

		/// <summary>
		/// The amount of groups of objects sorted by the distance. The groups are rendered from near to far by the distance to the camera. The settings mainly helps to calibrate the GPU instancing.
		/// </summary>
		[DefaultValue( 8 )]
		[Range( 1, 16 )]
		[Category( "Optimization" )]
		public Reference<int> SectorsByDistance
		{
			get { if( _sectorsByDistance.BeginGet() ) SectorsByDistance = _sectorsByDistance.Get( this ); return _sectorsByDistance.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( value > 16 )
					value = new Reference<int>( 16, value.GetByReference );
				if( _sectorsByDistance.BeginSet( this, ref value ) ) { try { SectorsByDistanceChanged?.Invoke( this ); } finally { _sectorsByDistance.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="SectorsByDistance"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> SectorsByDistanceChanged;
		ReferenceField<int> _sectorsByDistance = 8;

		////!!!!default
		////!!!!description
		//[DefaultValue( true )]
		//public Reference<bool> MultiLightOptimization
		//{
		//	get { if( _multiLightOptimization.BeginGet() ) MultiLightOptimization = _multiLightOptimization.Get( this ); return _multiLightOptimization.value; }
		//	set { if( _multiLightOptimization.BeginSet( this, ref value ) ) { try { MultiLightOptimizationChanged?.Invoke( this ); } finally { _multiLightOptimization.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MultiLightOptimization"/> property value changes.</summary>
		//public event Action<RenderingPipeline_Basic> MultiLightOptimizationChanged;
		//ReferenceField<bool> _multiLightOptimization = true;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to display shadows.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( true )]
		public Reference<bool> DebugDrawShadows
		{
			get { if( _debugDrawShadows.BeginGet() ) DebugDrawShadows = _debugDrawShadows.Get( this ); return _debugDrawShadows.value; }
			set { if( _debugDrawShadows.BeginSet( this, ref value ) ) { try { DebugDrawShadowsChanged?.Invoke( this ); } finally { _debugDrawShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawShadows"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawShadowsChanged;
		ReferenceField<bool> _debugDrawShadows = true;

		/// <summary>
		/// Whether to display objects that are rendered with deferred shading.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawDeferredPass
		{
			get { if( _debugDrawDeferredPass.BeginGet() ) DebugDrawDeferredPass = _debugDrawDeferredPass.Get( this ); return _debugDrawDeferredPass.value; }
			set { if( _debugDrawDeferredPass.BeginSet( this, ref value ) ) { try { DebugDrawDeferredPassChanged?.Invoke( this ); } finally { _debugDrawDeferredPass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawDeferredPass"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawDeferredPassChanged;
		ReferenceField<bool> _debugDrawDeferredPass = true;

		/// <summary>
		/// Whether to display opaque objects, that are drawn with forward rendering.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawForwardOpaquePass
		{
			get { if( _debugDrawForwardOpaquePass.BeginGet() ) DebugDrawForwardOpaquePass = _debugDrawForwardOpaquePass.Get( this ); return _debugDrawForwardOpaquePass.value; }
			set { if( _debugDrawForwardOpaquePass.BeginSet( this, ref value ) ) { try { DebugDrawForwardOpaquePassChanged?.Invoke( this ); } finally { _debugDrawForwardOpaquePass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawForwardOpaquePass"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawForwardOpaquePassChanged;
		ReferenceField<bool> _debugDrawForwardOpaquePass = true;

		/// <summary>
		/// Whether to display transparent objects, that are drawn with forward rendering.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawForwardTransparentPass
		{
			get { if( _debugDrawForwardTransparentPass.BeginGet() ) DebugDrawForwardTransparentPass = _debugDrawForwardTransparentPass.Get( this ); return _debugDrawForwardTransparentPass.value; }
			set { if( _debugDrawForwardTransparentPass.BeginSet( this, ref value ) ) { try { DebugDrawForwardTransparentPassChanged?.Invoke( this ); } finally { _debugDrawForwardTransparentPass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawForwardTransparentPass"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawForwardTransparentPassChanged;
		ReferenceField<bool> _debugDrawForwardTransparentPass = true;

		/// <summary>
		/// Whether to display layers.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( true )]
		public Reference<bool> DebugDrawLayers
		{
			get { if( _debugDrawLayers.BeginGet() ) DebugDrawLayers = _debugDrawLayers.Get( this ); return _debugDrawLayers.value; }
			set { if( _debugDrawLayers.BeginSet( this, ref value ) ) { try { DebugDrawLayersChanged?.Invoke( this ); } finally { _debugDrawLayers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawLayers"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawLayersChanged;
		ReferenceField<bool> _debugDrawLayers = true;

		/// <summary>
		/// Whether to display decals.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( true )]
		public Reference<bool> DebugDrawDecals
		{
			get { if( _debugDrawDecals.BeginGet() ) DebugDrawDecals = _debugDrawDecals.Get( this ); return _debugDrawDecals.value; }
			set { if( _debugDrawDecals.BeginSet( this, ref value ) ) { try { DebugDrawDecalsChanged?.Invoke( this ); } finally { _debugDrawDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawDecals"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawDecalsChanged;
		ReferenceField<bool> _debugDrawDecals = true;

		/// <summary>
		/// Whether to display various auxiliary geometry that is drawn with Simple 3D Renderer.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Debug Draw Simple 3D Renderer" )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawSimple3DRenderer
		{
			get { if( _debugDrawSimple3DRenderer.BeginGet() ) DebugDrawSimple3DRenderer = _debugDrawSimple3DRenderer.Get( this ); return _debugDrawSimple3DRenderer.value; }
			set { if( _debugDrawSimple3DRenderer.BeginSet( this, ref value ) ) { try { DebugDrawSimple3DRendererChanged?.Invoke( this ); } finally { _debugDrawSimple3DRenderer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawSimple3DRenderer"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawSimple3DRendererChanged;
		ReferenceField<bool> _debugDrawSimple3DRenderer = true;

		////Simple3DRendererOpacity
		//ReferenceField<double> _simple3DRendererOpacity = 1;
		//[DefaultValue( 1.0 )]
		//[ApplicableRange( 0, 1 )]
		//[Category( "Debug" )]
		//[DisplayName( "Simple 3D Renderer Opacity" )]
		//public Reference<double> Simple3DRendererOpacity
		//{
		//	get
		//	{
		//		if( _simple3DRendererOpacity.BeginGet() )
		//			Simple3DRendererOpacity = _simple3DRendererOpacity.Get( this );
		//		return _simple3DRendererOpacity.value;
		//	}
		//	set
		//	{
		//		if( _simple3DRendererOpacity.BeginSet( this, ref value ) )
		//		{
		//			try { Simple3DRendererOpacityChanged?.Invoke( this ); }
		//			finally { _simple3DRendererOpacity.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<RenderingPipeline> Simple3DRendererOpacityChanged;

		/// <summary>
		/// Whether to display UI elements.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Debug Draw UI" )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawUI
		{
			get { if( _debugDrawUI.BeginGet() ) DebugDrawUI = _debugDrawUI.Get( this ); return _debugDrawUI.value; }
			set { if( _debugDrawUI.BeginSet( this, ref value ) ) { try { DebugDrawUIChanged?.Invoke( this ); } finally { _debugDrawUI.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawUI"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawUIChanged;
		ReferenceField<bool> _debugDrawUI = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether to visualize triangle meshes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Geometry Type" )]
		public Reference<bool> DebugDrawMeshes
		{
			get { if( _debugDrawMeshes.BeginGet() ) DebugDrawMeshes = _debugDrawMeshes.Get( this ); return _debugDrawMeshes.value; }
			set { if( _debugDrawMeshes.BeginSet( this, ref value ) ) { try { DebugDrawMeshesChanged?.Invoke( this ); } finally { _debugDrawMeshes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawMeshes"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawMeshesChanged;
		ReferenceField<bool> _debugDrawMeshes = true;

		/// <summary>
		/// Whether to visualize voxelized mesh geometry.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Geometry Type" )]
		public Reference<bool> DebugDrawVoxels
		{
			get { if( _debugDrawVoxels.BeginGet() ) DebugDrawVoxels = _debugDrawVoxels.Get( this ); return _debugDrawVoxels.value; }
			set { if( _debugDrawVoxels.BeginSet( this, ref value ) ) { try { DebugDrawVoxelsChanged?.Invoke( this ); } finally { _debugDrawVoxels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawVoxels"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawVoxelsChanged;
		ReferenceField<bool> _debugDrawVoxels = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether to visualize batched data.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Batching" )]
		public Reference<bool> DebugDrawBatchedData
		{
			get { if( _debugDrawBatchedData.BeginGet() ) DebugDrawBatchedData = _debugDrawBatchedData.Get( this ); return _debugDrawBatchedData.value; }
			set { if( _debugDrawBatchedData.BeginSet( this, ref value ) ) { try { DebugDrawBatchedDataChanged?.Invoke( this ); } finally { _debugDrawBatchedData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawBatchedData"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawBatchedDataChanged;
		ReferenceField<bool> _debugDrawBatchedData = true;

		/// <summary>
		/// Whether to visualize not batched data.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Batching" )]
		public Reference<bool> DebugDrawNotBatchedData
		{
			get { if( _debugDrawNotBatchedData.BeginGet() ) DebugDrawNotBatchedData = _debugDrawNotBatchedData.Get( this ); return _debugDrawNotBatchedData.value; }
			set { if( _debugDrawNotBatchedData.BeginSet( this, ref value ) ) { try { DebugDrawNotBatchedDataChanged?.Invoke( this ); } finally { _debugDrawNotBatchedData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawNotBatchedData"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDrawNotBatchedDataChanged;
		ReferenceField<bool> _debugDrawNotBatchedData = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether to add direct lighting to output image.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Lighting" )]
		public Reference<bool> DebugDirectLighting
		{
			get { if( _debugDirectLighting.BeginGet() ) DebugDirectLighting = _debugDirectLighting.Get( this ); return _debugDirectLighting.value; }
			set { if( _debugDirectLighting.BeginSet( this, ref value ) ) { try { DebugDirectLightingChanged?.Invoke( this ); } finally { _debugDirectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDirectLighting"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugDirectLightingChanged;
		ReferenceField<bool> _debugDirectLighting = true;

		/// <summary>
		/// Whether to add indirect lighting to output image.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug Lighting" )]
		public Reference<bool> DebugIndirectLighting
		{
			get { if( _debugIndirectLighting.BeginGet() ) DebugIndirectLighting = _debugIndirectLighting.Get( this ); return _debugIndirectLighting.value; }
			set { if( _debugIndirectLighting.BeginSet( this, ref value ) ) { try { DebugIndirectLightingChanged?.Invoke( this ); } finally { _debugIndirectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugIndirectLighting"/> property value changes.</summary>
		public event Action<RenderingPipeline_Basic> DebugIndirectLightingChanged;
		ReferenceField<bool> _debugIndirectLighting = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				if( !Shadows.Value && p.Name.Length > 6 && p.Name.Substring( 0, 6 ) == "Shadow" && p.Name != nameof( Shadows ) )
				{
					skip = true;
					return;
				}

				switch( p.Name )
				{
				case nameof( ShadowDirectionalLightCascadeDistribution ):
				//case nameof( ShadowDirectionalLightCascadeOverlapping ):
				case nameof( ShadowDirectionalLightCascadeVisualize ):
					if( ShadowDirectionalLightCascades.Value < 2 )
						skip = true;
					break;

				case nameof( OcclusionCullingBufferSize ):
				case nameof( OcclusionCullingBufferCullNodes ):
				case nameof( OcclusionCullingBufferCullObjects ):
				case nameof( OcclusionCullingBufferMaxOccluders ):
					if( !OcclusionCullingBufferScene && !OcclusionCullingBufferDirectionalLight )
						skip = true;
					break;

				case nameof( LightGridResolution ):
					if( LightGrid.Value == AutoTrueFalse.False )
						skip = true;
					break;

				case nameof( GIDistance ):
				case nameof( GIGridSize ):
				case nameof( GICascades ):
				case nameof( GICascadeDistribution ):
				case nameof( GICascadeVisualize ):
				case nameof( GIVoxelizationConservative ):
					if( !IndirectLighting && !Reflection )
						skip = true;
					break;

				//!!!!temp gi
				case nameof( IndirectLighting ):
				case nameof( Reflection ):
					skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var items = new List<string>();
			items.Add( "Background Effects" );
			items.Add( "Scene Effects" );
			items.Add( "Final Image Effects" );
			foreach( var item in items )
			{
				var child = CreateComponent<Component>();
				child.Name = item;
			}

			var sceneEffects = GetComponent( "Scene Effects" );

			var types = new List<(Type, bool)>();
			types.Add( (typeof( RenderingEffect_AmbientOcclusion ), true) );
			types.Add( (typeof( RenderingEffect_Bloom ), false) );
			types.Add( (typeof( RenderingEffect_LensEffects ), true) );
			types.Add( (typeof( RenderingEffect_ToneMapping ), true) );
			types.Add( (typeof( RenderingEffect_ToLDR ), true) );
			types.Add( (typeof( RenderingEffect_Antialiasing ), true) );
			types.Add( (typeof( RenderingEffect_ResolutionUpscale ), true) );
			types.Add( (typeof( RenderingEffect_Sharpen ), true) );

			foreach( var item in types )
			{
				var obj = sceneEffects.CreateComponent( item.Item1 );
				obj.Enabled = item.Item2;
				obj.Name = obj.BaseType.GetUserFriendlyNameForInstance();
			}
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//old version compatibility
			if( block.AttributeExists( "ShadowFarDistance" ) && double.TryParse( block.GetAttribute( "ShadowFarDistance" ), out var v ) )
			{
				ShadowDirectionalDistance = v;
				ShadowPointSpotlightDistance = v;
			}

			return true;
		}
	}
}
