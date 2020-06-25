// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Basic rendering pipeline.
	/// </summary>
	public abstract partial class Component_RenderingPipeline_Basic : Component_RenderingPipeline
	{
		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

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
			set { if( _debugMode.BeginSet( ref value ) ) { try { DebugModeChanged?.Invoke( this ); } finally { _debugMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugMode"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugModeChanged;
		ReferenceField<DebugModeEnum> _debugMode = DebugModeEnum.None;

		/////////////////////////////////////////

		/// <summary>
		/// Whether shadows are enabled.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shadows" )]
		public Reference<bool> Shadows
		{
			get
			{
				//!!!!ut
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					return false;

				if( _shadows.BeginGet() ) Shadows = _shadows.Get( this ); return _shadows.value;
			}
			set { if( _shadows.BeginSet( ref value ) ) { try { ShadowsChanged?.Invoke( this ); } finally { _shadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shadows"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowsChanged;
		ReferenceField<bool> _shadows = true;

		public enum ShadowQualityEnum
		{
			Low,
			High,
			//!!!!
		}

		/// <summary>
		/// Specifies the quality of shadows.
		/// </summary>
		[DefaultValue( ShadowQualityEnum.High )]
		[Category( "Shadows" )]
		public Reference<ShadowQualityEnum> ShadowQuality
		{
			get { if( _shadowQuality.BeginGet() ) ShadowQuality = _shadowQuality.Get( this ); return _shadowQuality.value; }
			set { if( _shadowQuality.BeginSet( ref value ) ) { try { ShadowQualityChanged?.Invoke( this ); } finally { _shadowQuality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowQuality"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowQualityChanged;
		ReferenceField<ShadowQualityEnum> _shadowQuality = ShadowQualityEnum.High;

		/// <summary>
		/// Rendering range of the shadows.
		/// </summary>
		[DefaultValue( 130.0 )]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shadows" )]
		public Reference<double> ShadowFarDistance
		{
			get { if( _shadowFarDistance.BeginGet() ) ShadowFarDistance = _shadowFarDistance.Get( this ); return _shadowFarDistance.value; }
			set { if( _shadowFarDistance.BeginSet( ref value ) ) { try { ShadowFarDistanceChanged?.Invoke( this ); } finally { _shadowFarDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowFarDistance"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowFarDistanceChanged;
		ReferenceField<double> _shadowFarDistance = 130;

		/// <summary>
		/// The intensity of the shadows.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Shadows" )]
		public Reference<double> ShadowIntensity
		{
			get { if( _shadowIntensity.BeginGet() ) ShadowIntensity = _shadowIntensity.Get( this ); return _shadowIntensity.value; }
			set { if( _shadowIntensity.BeginSet( ref value ) ) { try { ShadowIntensityChanged?.Invoke( this ); } finally { _shadowIntensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowIntensity"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowIntensityChanged;
		ReferenceField<double> _shadowIntensity = 1;

		/// <summary>
		/// Maximum number of Directional Lights that can cast shadows.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 4 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowDirectionalLightMaxCount
		{
			get { if( _shadowDirectionalLightMaxCount.BeginGet() ) ShadowDirectionalLightMaxCount = _shadowDirectionalLightMaxCount.Get( this ); return _shadowDirectionalLightMaxCount.value; }
			set { if( _shadowDirectionalLightMaxCount.BeginSet( ref value ) ) { try { ShadowDirectionalLightMaxCountChanged?.Invoke( this ); } finally { _shadowDirectionalLightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightMaxCount"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightMaxCountChanged;
		ReferenceField<int> _shadowDirectionalLightMaxCount = 1;

		//!!!!default value
		/// <summary>
		/// The size of a shadow texture for Directional Lights.
		/// </summary>
		[DefaultValue( ShadowTextureSize._2048 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSize> ShadowDirectionalLightTextureSize
		{
			get { if( _shadowDirectionalLightTextureSize.BeginGet() ) ShadowDirectionalLightTextureSize = _shadowDirectionalLightTextureSize.Get( this ); return _shadowDirectionalLightTextureSize.value; }
			set { if( _shadowDirectionalLightTextureSize.BeginSet( ref value ) ) { try { ShadowDirectionalLightTextureSizeChanged?.Invoke( this ); } finally { _shadowDirectionalLightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightTextureSize"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightTextureSizeChanged;
		ReferenceField<ShadowTextureSize> _shadowDirectionalLightTextureSize = ShadowTextureSize._2048;

		/// <summary>
		/// The number of cascades used for Directional Lights.
		/// </summary>
		[DefaultValue( 3 )]
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
				if( _shadowDirectionalLightCascades.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadesChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascades.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascades"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadesChanged;
		ReferenceField<int> _shadowDirectionalLightCascades = 3;

		//!!!!default
		/// <summary>
		/// Defines shadow cascades distribution for Directional Lights. The distance of the current cascade, multiplied by this value gives distance to the next cascade.
		/// </summary>
		[DefaultValue( 2.4 )]
		[Category( "Shadows" )]
		[Range( 1, 5 )]
		public Reference<double> ShadowDirectionalLightCascadeDistribution
		{
			get { if( _shadowDirectionalLightCascadeDistribution.BeginGet() ) ShadowDirectionalLightCascadeDistribution = _shadowDirectionalLightCascadeDistribution.Get( this ); return _shadowDirectionalLightCascadeDistribution.value; }
			set { if( _shadowDirectionalLightCascadeDistribution.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadeDistributionChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascadeDistribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascadeDistribution"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadeDistributionChanged;
		ReferenceField<double> _shadowDirectionalLightCascadeDistribution = 2.4;

		//[DefaultValue( 0.05 )]
		//[Category( "Shadows" )]
		//[ApplicableRange( 0, 1 )]
		//[DisplayName( "Shadow Directional Light Cascades Split 1" )]
		//public Reference<double> ShadowDirectionalLightCascadesSplit1
		//{
		//	get { if( _shadowDirectionalLightCascadesSplit1.BeginGet() ) ShadowDirectionalLightCascadesSplit1 = _shadowDirectionalLightCascadesSplit1.Get( this ); return _shadowDirectionalLightCascadesSplit1.value; }
		//	set { if( _shadowDirectionalLightCascadesSplit1.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadesSplit1Changed?.Invoke( this ); } finally { _shadowDirectionalLightCascadesSplit1.EndSet(); } } }
		//}
		//public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadesSplit1Changed;
		//ReferenceField<double> _shadowDirectionalLightCascadesSplit1 = 0.05;

		//[DefaultValue( 0.17 )]
		//[Category( "Shadows" )]
		//[ApplicableRange( 0, 1 )]
		//[DisplayName( "Shadow Directional Light Cascades Split 2" )]
		//public Reference<double> ShadowDirectionalLightCascadesSplit2
		//{
		//	get { if( _shadowDirectionalLightCascadesSplit2.BeginGet() ) ShadowDirectionalLightCascadesSplit2 = _shadowDirectionalLightCascadesSplit2.Get( this ); return _shadowDirectionalLightCascadesSplit2.value; }
		//	set { if( _shadowDirectionalLightCascadesSplit2.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadesSplit2Changed?.Invoke( this ); } finally { _shadowDirectionalLightCascadesSplit2.EndSet(); } } }
		//}
		//public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadesSplit2Changed;
		//ReferenceField<double> _shadowDirectionalLightCascadesSplit2 = 0.17;

		//[DefaultValue( 0.4 )]
		//[Category( "Shadows" )]
		//[ApplicableRange( 0, 1 )]
		//[DisplayName( "Shadow Directional Light Cascades Split 3" )]
		//public Reference<double> ShadowDirectionalLightCascadesSplit3
		//{
		//	get { if( _shadowDirectionalLightCascadesSplit3.BeginGet() ) ShadowDirectionalLightCascadesSplit3 = _shadowDirectionalLightCascadesSplit3.Get( this ); return _shadowDirectionalLightCascadesSplit3.value; }
		//	set { if( _shadowDirectionalLightCascadesSplit3.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadesSplit3Changed?.Invoke( this ); } finally { _shadowDirectionalLightCascadesSplit3.EndSet(); } } }
		//}
		//public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadesSplit3Changed;
		//ReferenceField<double> _shadowDirectionalLightCascadesSplit3 = 0.4;

		/// <summary>
		/// Whether to visualize shadow cascades for Directional Lights.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Shadows" )]
		public Reference<bool> ShadowDirectionalLightCascadeVisualize
		{
			get { if( _shadowDirectionalLightCascadeVisualize.BeginGet() ) ShadowDirectionalLightCascadeVisualize = _shadowDirectionalLightCascadeVisualize.Get( this ); return _shadowDirectionalLightCascadeVisualize.value; }
			set { if( _shadowDirectionalLightCascadeVisualize.BeginSet( ref value ) ) { try { ShadowDirectionalLightCascadeVisualizeChanged?.Invoke( this ); } finally { _shadowDirectionalLightCascadeVisualize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightCascadeVisualize"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightCascadeVisualizeChanged;
		ReferenceField<bool> _shadowDirectionalLightCascadeVisualize = false;

		//!!!!default value, applicable range
		/// <summary>
		/// Maximum distance to camera where shadows from Directional Lights will be cast.
		/// </summary>
		[DefaultValue( 1000.0 )]
		[Category( "Shadows" )]
		[Range( 10, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> ShadowDirectionalLightExtrusionDistance
		{
			get { if( _shadowDirectionalLightExtrusionDistance.BeginGet() ) ShadowDirectionalLightExtrusionDistance = _shadowDirectionalLightExtrusionDistance.Get( this ); return _shadowDirectionalLightExtrusionDistance.value; }
			set { if( _shadowDirectionalLightExtrusionDistance.BeginSet( ref value ) ) { try { ShadowDirectionalLightExtrusionDistanceChanged?.Invoke( this ); } finally { _shadowDirectionalLightExtrusionDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowDirectionalLightExtrusionDistance"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowDirectionalLightExtrusionDistanceChanged;
		ReferenceField<double> _shadowDirectionalLightExtrusionDistance = 1000;

		/// <summary>
		/// Maximum number of Point Lights, that can cast shadows.
		/// </summary>
		[DefaultValue( 8 )]
		[Range( 0, 16 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowPointLightMaxCount
		{
			get { if( _shadowPointLightMaxCount.BeginGet() ) ShadowPointLightMaxCount = _shadowPointLightMaxCount.Get( this ); return _shadowPointLightMaxCount.value; }
			set { if( _shadowPointLightMaxCount.BeginSet( ref value ) ) { try { ShadowPointLightMaxCountChanged?.Invoke( this ); } finally { _shadowPointLightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowPointLightMaxCount"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowPointLightMaxCountChanged;
		ReferenceField<int> _shadowPointLightMaxCount = 8;

		/// <summary>
		/// The size of a shadow texture for Point Lights.
		/// </summary>
		[DefaultValue( ShadowTextureSize._1024 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSize> ShadowPointLightTextureSize
		{
			get { if( _shadowPointLightTextureSize.BeginGet() ) ShadowPointLightTextureSize = _shadowPointLightTextureSize.Get( this ); return _shadowPointLightTextureSize.value; }
			set { if( _shadowPointLightTextureSize.BeginSet( ref value ) ) { try { ShadowPointLightTextureSizeChanged?.Invoke( this ); } finally { _shadowPointLightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowPointLightTextureSize"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowPointLightTextureSizeChanged;
		ReferenceField<ShadowTextureSize> _shadowPointLightTextureSize = ShadowTextureSize._1024;

		/// <summary>
		/// Maximum number of Spotlights, that can cast shadows.
		/// </summary>
		[DefaultValue( 8 )]
		[Range( 0, 16 )]
		[Category( "Shadows" )]
		public Reference<int> ShadowSpotlightMaxCount
		{
			get { if( _shadowSpotlightMaxCount.BeginGet() ) ShadowSpotlightMaxCount = _shadowSpotlightMaxCount.Get( this ); return _shadowSpotlightMaxCount.value; }
			set { if( _shadowSpotlightMaxCount.BeginSet( ref value ) ) { try { ShadowSpotlightMaxCountChanged?.Invoke( this ); } finally { _shadowSpotlightMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowSpotlightMaxCount"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowSpotlightMaxCountChanged;
		ReferenceField<int> _shadowSpotlightMaxCount = 8;

		/// <summary>
		/// The size of shadow texture for Spotlights.
		/// </summary>
		[DefaultValue( ShadowTextureSize._1024 )]
		[Category( "Shadows" )]
		public Reference<ShadowTextureSize> ShadowSpotlightTextureSize
		{
			get { if( _shadowSpotlightTextureSize.BeginGet() ) ShadowSpotlightTextureSize = _shadowSpotlightTextureSize.Get( this ); return _shadowSpotlightTextureSize.value; }
			set { if( _shadowSpotlightTextureSize.BeginSet( ref value ) ) { try { ShadowSpotlightTextureSizeChanged?.Invoke( this ); } finally { _shadowSpotlightTextureSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowSpotlightTextureSize"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> ShadowSpotlightTextureSizeChanged;
		ReferenceField<ShadowTextureSize> _shadowSpotlightTextureSize = ShadowTextureSize._1024;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to display objects that are rendered with deferred shading.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawDeferredPass
		{
			get { if( _debugDrawDeferredPass.BeginGet() ) DebugDrawDeferredPass = _debugDrawDeferredPass.Get( this ); return _debugDrawDeferredPass.value; }
			set { if( _debugDrawDeferredPass.BeginSet( ref value ) ) { try { DebugDrawDeferredPassChanged?.Invoke( this ); } finally { _debugDrawDeferredPass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawDeferredPass"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawDeferredPassChanged;
		ReferenceField<bool> _debugDrawDeferredPass = true;

		/// <summary>
		/// Whether to display opaque objects, that are drawn with forward rendering.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawForwardOpaquePass
		{
			get { if( _debugDrawForwardOpaquePass.BeginGet() ) DebugDrawForwardOpaquePass = _debugDrawForwardOpaquePass.Get( this ); return _debugDrawForwardOpaquePass.value; }
			set { if( _debugDrawForwardOpaquePass.BeginSet( ref value ) ) { try { DebugDrawForwardOpaquePassChanged?.Invoke( this ); } finally { _debugDrawForwardOpaquePass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawForwardOpaquePass"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawForwardOpaquePassChanged;
		ReferenceField<bool> _debugDrawForwardOpaquePass = true;

		/// <summary>
		/// Whether to display transparent objects, that are drawn with forward rendering.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawForwardTransparentPass
		{
			get { if( _debugDrawForwardTransparentPass.BeginGet() ) DebugDrawForwardTransparentPass = _debugDrawForwardTransparentPass.Get( this ); return _debugDrawForwardTransparentPass.value; }
			set { if( _debugDrawForwardTransparentPass.BeginSet( ref value ) ) { try { DebugDrawForwardTransparentPassChanged?.Invoke( this ); } finally { _debugDrawForwardTransparentPass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawForwardTransparentPass"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawForwardTransparentPassChanged;
		ReferenceField<bool> _debugDrawForwardTransparentPass = true;

		/// <summary>
		/// Whether to display layers.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( true )]
		public Reference<bool> DebugDrawLayers
		{
			get { if( _debugDrawLayers.BeginGet() ) DebugDrawLayers = _debugDrawLayers.Get( this ); return _debugDrawLayers.value; }
			set { if( _debugDrawLayers.BeginSet( ref value ) ) { try { DebugDrawLayersChanged?.Invoke( this ); } finally { _debugDrawLayers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawLayers"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawLayersChanged;
		ReferenceField<bool> _debugDrawLayers = true;

		/// <summary>
		/// Whether to display decals.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( true )]
		public Reference<bool> DebugDrawDecals
		{
			get { if( _debugDrawDecals.BeginGet() ) DebugDrawDecals = _debugDrawDecals.Get( this ); return _debugDrawDecals.value; }
			set { if( _debugDrawDecals.BeginSet( ref value ) ) { try { DebugDrawDecalsChanged?.Invoke( this ); } finally { _debugDrawDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawDecals"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawDecalsChanged;
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
			set { if( _debugDrawSimple3DRenderer.BeginSet( ref value ) ) { try { DebugDrawSimple3DRendererChanged?.Invoke( this ); } finally { _debugDrawSimple3DRenderer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawSimple3DRenderer"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawSimple3DRendererChanged;
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
		//		if( _simple3DRendererOpacity.BeginSet( ref value ) )
		//		{
		//			try { Simple3DRendererOpacityChanged?.Invoke( this ); }
		//			finally { _simple3DRendererOpacity.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_RenderingPipeline> Simple3DRendererOpacityChanged;

		/// <summary>
		/// Whether to display UI elements.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Debug Draw UI" )]
		[Category( "Debug" )]
		public Reference<bool> DebugDrawUI
		{
			get { if( _debugDrawUI.BeginGet() ) DebugDrawUI = _debugDrawUI.Get( this ); return _debugDrawUI.value; }
			set { if( _debugDrawUI.BeginSet( ref value ) ) { try { DebugDrawUIChanged?.Invoke( this ); } finally { _debugDrawUI.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugDrawUI"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline_Basic> DebugDrawUIChanged;
		ReferenceField<bool> _debugDrawUI = true;

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
				//case nameof( ShadowDirectionalLightCascadesSplit1 ):
				//	if( ShadowDirectionalLightCascades.Value < 2 )
				//		skip = true;
				//	break;
				//case nameof( ShadowDirectionalLightCascadesSplit2 ):
				//	if( ShadowDirectionalLightCascades.Value < 3 )
				//		skip = true;
				//	break;
				//case nameof( ShadowDirectionalLightCascadesSplit3 ):
				//	if( ShadowDirectionalLightCascades.Value < 4 )
				//		skip = true;
				//	break;
				case nameof( ShadowDirectionalLightCascadeDistribution ):
				case nameof( ShadowDirectionalLightCascadeVisualize ):
					if( ShadowDirectionalLightCascades.Value < 2 )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum ShadowTextureSize
		{
			_256,
			_512,
			_1024,
			_2048,
			_4096,
			_8192,
			//_16384,//!!!!как проверять хватит ли памяти
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
			types.Add( (typeof( Component_RenderingEffect_AmbientOcclusion ), true) );
			types.Add( (typeof( Component_RenderingEffect_Bloom ), false) );
			types.Add( (typeof( Component_RenderingEffect_LensEffects ), true) );
			types.Add( (typeof( Component_RenderingEffect_ToneMapping ), true) );
			types.Add( (typeof( Component_RenderingEffect_ToLDR ), true) );
			types.Add( (typeof( Component_RenderingEffect_Antialiasing ), true) );
			types.Add( (typeof( Component_RenderingEffect_Sharpen ), true) );

			foreach( var item in types )
			{
				var obj = sceneEffects.CreateComponent( item.Item1 );
				obj.Enabled = item.Item2;
				obj.Name = obj.BaseType.GetUserFriendlyNameForInstance();
			}
		}
	}
}
