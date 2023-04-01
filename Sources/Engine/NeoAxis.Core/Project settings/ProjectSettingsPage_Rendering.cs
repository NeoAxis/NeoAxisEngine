// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Rendering page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_Rendering : ProjectSettingsPage
	{
		/// <summary>
		/// The default thickness of debug lines.
		/// </summary>
		[Category( "Rendering: General" )]
		[DefaultValue( 1.5 )]
		public Reference<double> LineThickness
		{
			get { if( _lineThickness.BeginGet() ) LineThickness = _lineThickness.Get( this ); return _lineThickness.value; }
			set { if( _lineThickness.BeginSet( ref value ) ) { try { LineThicknessChanged?.Invoke( this ); } finally { _lineThickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LineThickness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> LineThicknessChanged;
		ReferenceField<double> _lineThickness = 1.5;

		public enum MaterialShadingEnum
		{
			Quality,
			Basic,
			Simple,

			//Simple,
			//Basic,
			//Quality,
		}

		/// <summary>
		/// The quality of lit shading of materials.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Material Shading (Restart to apply changes)" )]
		[DefaultValue( MaterialShadingEnum.Quality )]
		public Reference<MaterialShadingEnum> MaterialShading
		{
			get { if( _materialShading.BeginGet() ) MaterialShading = _materialShading.Get( this ); return _materialShading.value; }
			set { if( _materialShading.BeginSet( ref value ) ) { try { MaterialShadingChanged?.Invoke( this ); } finally { _materialShading.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialShading"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> MaterialShadingChanged;
		ReferenceField<MaterialShadingEnum> _materialShading = MaterialShadingEnum.Quality;

		/// <summary>
		/// The quality of lit shading of materials on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Material Shading" )]
		[DefaultValue( MaterialShadingEnum.Simple )]
		public Reference<MaterialShadingEnum> MaterialShadingLimitedDevice
		{
			get { if( _materialShadingLimitedDevice.BeginGet() ) MaterialShadingLimitedDevice = _materialShadingLimitedDevice.Get( this ); return _materialShadingLimitedDevice.value; }
			set { if( _materialShadingLimitedDevice.BeginSet( ref value ) ) { try { MaterialShadingLimitedDeviceChanged?.Invoke( this ); } finally { _materialShadingLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialShadingLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> MaterialShadingLimitedDeviceChanged;
		ReferenceField<MaterialShadingEnum> _materialShadingLimitedDevice = MaterialShadingEnum.Simple;

		public enum ShadowTechniqueEnum
		{
			[DisplayNameEnum( "Percentage Closer Filtering 32" )]
			PercentageCloserFiltering32,
			[DisplayNameEnum( "Percentage Closer Filtering 22" )]
			PercentageCloserFiltering22,
			[DisplayNameEnum( "Percentage Closer Filtering 16" )]
			PercentageCloserFiltering16,
			[DisplayNameEnum( "Percentage Closer Filtering 12" )]
			PercentageCloserFiltering12,
			[DisplayNameEnum( "Percentage Closer Filtering 8" )]
			PercentageCloserFiltering8,
			[DisplayNameEnum( "Percentage Closer Filtering 4" )]
			PercentageCloserFiltering4,
			Simple,
			None,

			//None,
			//Simple,
			//[DisplayNameEnum( "Percentage Closer Filtering 4" )]
			//PercentageCloserFiltering4,
			//[DisplayNameEnum( "Percentage Closer Filtering 8" )]
			//PercentageCloserFiltering8,
			//[DisplayNameEnum( "Percentage Closer Filtering 12" )]
			//PercentageCloserFiltering12,
			//[DisplayNameEnum( "Percentage Closer Filtering 16" )]
			//PercentageCloserFiltering16,

			//ContactHardening,
			//ExponentialVarianceShadowMaps,
		}

		/// <summary>
		/// The shadow technique of the project.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DefaultValue( ShadowTechniqueEnum.PercentageCloserFiltering22 )]//12 )]
		[DisplayName( "Shadow Technique (Restart to apply changes)" )]
		public Reference<ShadowTechniqueEnum> ShadowTechnique
		{
			get { if( _shadowTechnique.BeginGet() ) ShadowTechnique = _shadowTechnique.Get( this ); return _shadowTechnique.value; }
			set { if( _shadowTechnique.BeginSet( ref value ) ) { try { ShadowTechniqueChanged?.Invoke( this ); } finally { _shadowTechnique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTechnique"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowTechniqueChanged;
		ReferenceField<ShadowTechniqueEnum> _shadowTechnique = ShadowTechniqueEnum.PercentageCloserFiltering22;//12;

		/// <summary>
		/// The shadow technique of the project on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Shadow Technique" )]
		[DefaultValue( ShadowTechniqueEnum.PercentageCloserFiltering8 )]
		public Reference<ShadowTechniqueEnum> ShadowTechniqueLimitedDevice
		{
			get { if( _shadowTechniqueLimitedDevice.BeginGet() ) ShadowTechniqueLimitedDevice = _shadowTechniqueLimitedDevice.Get( this ); return _shadowTechniqueLimitedDevice.value; }
			set { if( _shadowTechniqueLimitedDevice.BeginSet( ref value ) ) { try { ShadowTechniqueLimitedDeviceChanged?.Invoke( this ); } finally { _shadowTechniqueLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTechniqueLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowTechniqueLimitedDeviceChanged;
		ReferenceField<ShadowTechniqueEnum> _shadowTechniqueLimitedDevice = ShadowTechniqueEnum.PercentageCloserFiltering8;

		public enum ShadowTextureFormatEnum
		{
			[DisplayNameEnum( "Float 32" )]
			Float32,
			[DisplayNameEnum( "Byte 4" )]
			Byte4,
		}

		/// <summary>
		/// The format of shadow textures. Byte4 is used when a GPU is not supports Float32 format, mostly it is low-end mobile devices. Auto mode has not been added for this parameter, because some mobile devices incorrectly provide info about the support for Float32.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Shadow Texture Format (Restart to apply changes)" )]
		[DefaultValue( ShadowTextureFormatEnum.Float32 )]
		public Reference<ShadowTextureFormatEnum> ShadowTextureFormat
		{
			get { if( _shadowTextureFormat.BeginGet() ) ShadowTextureFormat = _shadowTextureFormat.Get( this ); return _shadowTextureFormat.value; }
			set { if( _shadowTextureFormat.BeginSet( ref value ) ) { try { ShadowTextureFormatChanged?.Invoke( this ); } finally { _shadowTextureFormat.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTextureFormat"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowTextureFormatChanged;
		ReferenceField<ShadowTextureFormatEnum> _shadowTextureFormat = ShadowTextureFormatEnum.Float32;

		//[Category( "Rendering: Basic Device" )]
		//[DisplayName( "Shadow Max Texture Size Directional Light" )]
		//[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._8192 )]
		//public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizeDirectionalLight
		//{
		//	get { if( _shadowMaxTextureSizeDirectionalLight.BeginGet() ) ShadowMaxTextureSizeDirectionalLight = _shadowMaxTextureSizeDirectionalLight.Get( this ); return _shadowMaxTextureSizeDirectionalLight.value; }
		//	set { if( _shadowMaxTextureSizeDirectionalLight.BeginSet( ref value ) ) { try { ShadowMaxTextureSizeDirectionalLightChanged?.Invoke( this ); } finally { _shadowMaxTextureSizeDirectionalLight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowMaxTextureSizeDirectionalLight"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizeDirectionalLightChanged;
		//ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizeDirectionalLight = RenderingPipeline_Basic.ShadowTextureSize._8192;

		//[Category( "Rendering: Basic Device" )]
		//[DisplayName( "Shadow Max Texture Size Point Light" )]
		//[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._8192 )]
		//public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizePointLight
		//{
		//	get { if( _shadowMaxTextureSizePointLight.BeginGet() ) ShadowMaxTextureSizePointLight = _shadowMaxTextureSizePointLight.Get( this ); return _shadowMaxTextureSizePointLight.value; }
		//	set { if( _shadowMaxTextureSizePointLight.BeginSet( ref value ) ) { try { ShadowMaxTextureSizePointLightChanged?.Invoke( this ); } finally { _shadowMaxTextureSizePointLight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowMaxTextureSizePointLight"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizePointLightChanged;
		//ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizePointLight = RenderingPipeline_Basic.ShadowTextureSize._8192;

		//[Category( "Rendering: Basic Device" )]
		//[DisplayName( "Shadow Max Texture Size Spot Light" )]
		//[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._8192 )]
		//public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizeSpotLight
		//{
		//	get { if( _shadowMaxTextureSizeSpotLight.BeginGet() ) ShadowMaxTextureSizeSpotLight = _shadowMaxTextureSizeSpotLight.Get( this ); return _shadowMaxTextureSizeSpotLight.value; }
		//	set { if( _shadowMaxTextureSizeSpotLight.BeginSet( ref value ) ) { try { ShadowMaxTextureSizeSpotLightChanged?.Invoke( this ); } finally { _shadowMaxTextureSizeSpotLight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ShadowMaxTextureSizeSpotLight"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizeSpotLightChanged;
		//ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizeSpotLight = RenderingPipeline_Basic.ShadowTextureSize._8192;

		/// <summary>
		/// The format of shadow textures. Byte4 is used when a GPU is not supports Float32 format, mostly it is low-end mobile devices. Auto mode has not been added for this parameter, because some mobile devices incorrectly provide info about the support for Float32.
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Shadow Texture Format" )]
		[DefaultValue( ShadowTextureFormatEnum.Byte4 )]
		public Reference<ShadowTextureFormatEnum> ShadowTextureFormatLimitedDevice
		{
			get { if( _shadowTextureFormatLimitedDevice.BeginGet() ) ShadowTextureFormatLimitedDevice = _shadowTextureFormatLimitedDevice.Get( this ); return _shadowTextureFormatLimitedDevice.value; }
			set { if( _shadowTextureFormatLimitedDevice.BeginSet( ref value ) ) { try { ShadowTextureFormatLimitedDeviceChanged?.Invoke( this ); } finally { _shadowTextureFormatLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowTextureFormatLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowTextureFormatLimitedDeviceChanged;
		ReferenceField<ShadowTextureFormatEnum> _shadowTextureFormatLimitedDevice = ShadowTextureFormatEnum.Byte4;

		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Shadow Max Texture Size Directional Light" )]
		[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._1024 )]
		public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizeDirectionalLightLimitedDevice
		{
			get { if( _shadowMaxTextureSizeDirectionalLightLimitedDevice.BeginGet() ) ShadowMaxTextureSizeDirectionalLightLimitedDevice = _shadowMaxTextureSizeDirectionalLightLimitedDevice.Get( this ); return _shadowMaxTextureSizeDirectionalLightLimitedDevice.value; }
			set { if( _shadowMaxTextureSizeDirectionalLightLimitedDevice.BeginSet( ref value ) ) { try { ShadowMaxTextureSizeDirectionalLightLimitedDeviceChanged?.Invoke( this ); } finally { _shadowMaxTextureSizeDirectionalLightLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowMaxTextureSizeDirectionalLightLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizeDirectionalLightLimitedDeviceChanged;
		ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizeDirectionalLightLimitedDevice = RenderingPipeline_Basic.ShadowTextureSize._1024;

		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Shadow Max Texture Size Point Light" )]
		[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._512 )]
		public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizePointLightLimitedDevice
		{
			get { if( _shadowMaxTextureSizePointLightLimitedDevice.BeginGet() ) ShadowMaxTextureSizePointLightLimitedDevice = _shadowMaxTextureSizePointLightLimitedDevice.Get( this ); return _shadowMaxTextureSizePointLightLimitedDevice.value; }
			set { if( _shadowMaxTextureSizePointLightLimitedDevice.BeginSet( ref value ) ) { try { ShadowMaxTextureSizePointLightLimitedDeviceChanged?.Invoke( this ); } finally { _shadowMaxTextureSizePointLightLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowMaxTextureSizePointLightLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizePointLightLimitedDeviceChanged;
		ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizePointLightLimitedDevice = RenderingPipeline_Basic.ShadowTextureSize._512;

		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Shadow Max Texture Size Spot Light" )]
		[DefaultValue( RenderingPipeline_Basic.ShadowTextureSize._1024 )]
		public Reference<RenderingPipeline_Basic.ShadowTextureSize> ShadowMaxTextureSizeSpotLightLimitedDevice
		{
			get { if( _shadowMaxTextureSizeSpotLightLimitedDevice.BeginGet() ) ShadowMaxTextureSizeSpotLightLimitedDevice = _shadowMaxTextureSizeSpotLightLimitedDevice.Get( this ); return _shadowMaxTextureSizeSpotLightLimitedDevice.value; }
			set { if( _shadowMaxTextureSizeSpotLightLimitedDevice.BeginSet( ref value ) ) { try { ShadowMaxTextureSizeSpotLightLimitedDeviceChanged?.Invoke( this ); } finally { _shadowMaxTextureSizeSpotLightLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadowMaxTextureSizeSpotLightLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> ShadowMaxTextureSizeSpotLightLimitedDeviceChanged;
		ReferenceField<RenderingPipeline_Basic.ShadowTextureSize> _shadowMaxTextureSizeSpotLightLimitedDevice = RenderingPipeline_Basic.ShadowTextureSize._1024;


		//public enum CompressVerticesEnum
		//{
		//	/// <summary>
		//	/// No compression, maximally fast uploading to GPU.
		//	/// </summary>
		//	Original,

		//	/// <summary>
		//	/// Saving full quality.
		//	/// </summary>
		//	Quality,

		//	/// <summary>
		//	/// Positions and texture coordinates are converted to 16-bit.
		//	/// </summary>
		//	Performance,
		//}

		///// <summary>
		///// The vertex data compression mode.
		///// </summary>
		//[Category( "Rendering: Basic Device" )]
		//[DisplayName( "Compress Vertices (Restart to apply changes)" )]
		//[DefaultValue( CompressVerticesEnum.Quality )]
		////[Browsable( false )]//!!!!
		//public Reference<CompressVerticesEnum> CompressVertices
		//{
		//	get { if( _compressVertices.BeginGet() ) CompressVertices = _compressVertices.Get( this ); return _compressVertices.value; }
		//	set { if( _compressVertices.BeginSet( ref value ) ) { try { CompressVerticesChanged?.Invoke( this ); } finally { _compressVertices.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CompressVertices"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> CompressVerticesChanged;
		//ReferenceField<CompressVerticesEnum> _compressVertices = CompressVerticesEnum.Quality;

		///// <summary>
		///// The vertex data compression mode on limited devices (mobile).
		///// </summary>
		//[Category( "Rendering: Limited Device" )]
		//[DisplayName( "Compress Vertices" )]
		//[DefaultValue( CompressVerticesEnum.Performance )]
		////[Browsable( false )]//!!!!
		//public Reference<CompressVerticesEnum> CompressVerticesLimitedDevice
		//{
		//	get { if( _compressVerticesLimitedDevice.BeginGet() ) CompressVerticesLimitedDevice = _compressVerticesLimitedDevice.Get( this ); return _compressVerticesLimitedDevice.value; }
		//	set { if( _compressVerticesLimitedDevice.BeginSet( ref value ) ) { try { CompressVerticesLimitedDeviceChanged?.Invoke( this ); } finally { _compressVerticesLimitedDevice.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CompressVerticesLimitedDevice"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> CompressVerticesLimitedDeviceChanged;
		//ReferenceField<CompressVerticesEnum> _compressVerticesLimitedDevice = CompressVerticesEnum.Performance;

		/// <summary>
		/// Whether to allow using skeletal animation.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Skeletal Animation (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> SkeletalAnimation
		{
			get { if( _skeletalAnimation.BeginGet() ) SkeletalAnimation = _skeletalAnimation.Get( this ); return _skeletalAnimation.value; }
			set { if( _skeletalAnimation.BeginSet( ref value ) ) { try { SkeletalAnimationChanged?.Invoke( this ); } finally { _skeletalAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkeletalAnimation"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> SkeletalAnimationChanged;
		ReferenceField<bool> _skeletalAnimation = true;

		/// <summary>
		/// Whether to allow using skeletal animation on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Skeletal Animation" )]
		[DefaultValue( true )]
		public Reference<bool> SkeletalAnimationLimitedDevice
		{
			get { if( _skeletalAnimationLimitedDevice.BeginGet() ) SkeletalAnimationLimitedDevice = _skeletalAnimationLimitedDevice.Get( this ); return _skeletalAnimationLimitedDevice.value; }
			set { if( _skeletalAnimationLimitedDevice.BeginSet( ref value ) ) { try { SkeletalAnimationLimitedDeviceChanged?.Invoke( this ); } finally { _skeletalAnimationLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkeletalAnimationLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> SkeletalAnimationLimitedDeviceChanged;
		ReferenceField<bool> _skeletalAnimationLimitedDevice = true;

		/// <summary>
		/// Whether to allow using light mask for lights.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Light Mask (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> LightMask
		{
			get { if( _lightMask.BeginGet() ) LightMask = _lightMask.Get( this ); return _lightMask.value; }
			set { if( _lightMask.BeginSet( ref value ) ) { try { LightMaskChanged?.Invoke( this ); } finally { _lightMask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMask"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> LightMaskChanged;
		ReferenceField<bool> _lightMask = true;

		/// <summary>
		/// Whether to allow using light mask for lights on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Light Mask" )]
		[DefaultValue( false )]
		public Reference<bool> LightMaskLimitedDevice
		{
			get { if( _lightMaskLimitedDevice.BeginGet() ) LightMaskLimitedDevice = _lightMaskLimitedDevice.Get( this ); return _lightMaskLimitedDevice.value; }
			set { if( _lightMaskLimitedDevice.BeginSet( ref value ) ) { try { LightMaskLimitedDeviceChanged?.Invoke( this ); } finally { _lightMaskLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightMaskLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> LightMaskLimitedDeviceChanged;
		ReferenceField<bool> _lightMaskLimitedDevice = false;

		/// <summary>
		/// Whether to allow using normal mapping.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Normal Mapping (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> NormalMapping
		{
			get { if( _normalMapping.BeginGet() ) NormalMapping = _normalMapping.Get( this ); return _normalMapping.value; }
			set { if( _normalMapping.BeginSet( ref value ) ) { try { NormalMappingChanged?.Invoke( this ); } finally { _normalMapping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NormalMapping"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> NormalMappingChanged;
		ReferenceField<bool> _normalMapping = true;

		/// <summary>
		/// Whether to allow using normal mapping on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Normal Mapping" )]
		[DefaultValue( false )]
		public Reference<bool> NormalMappingLimitedDevice
		{
			get { if( _normalMappingLimitedDevice.BeginGet() ) NormalMappingLimitedDevice = _normalMappingLimitedDevice.Get( this ); return _normalMappingLimitedDevice.value; }
			set { if( _normalMappingLimitedDevice.BeginSet( ref value ) ) { try { NormalMappingLimitedDeviceChanged?.Invoke( this ); } finally { _normalMappingLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NormalMappingLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> NormalMappingLimitedDeviceChanged;
		ReferenceField<bool> _normalMappingLimitedDevice = false;

		/// <summary>
		/// Whether to enable anisotropic filtering for textures.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Anisotropic Filtering (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> AnisotropicFiltering
		{
			get { if( _anisotropicFiltering.BeginGet() ) AnisotropicFiltering = _anisotropicFiltering.Get( this ); return _anisotropicFiltering.value; }
			set { if( _anisotropicFiltering.BeginSet( ref value ) ) { try { AnisotropicFilteringChanged?.Invoke( this ); } finally { _anisotropicFiltering.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropicFiltering"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> AnisotropicFilteringChanged;
		ReferenceField<bool> _anisotropicFiltering = true;

		/// <summary>
		/// Whether to enable anisotropic filtering for textures on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Anisotropic Filtering" )]
		[DefaultValue( false )]
		public Reference<bool> AnisotropicFilteringLimitedDevice
		{
			get { if( _anisotropicFilteringLimitedDevice.BeginGet() ) AnisotropicFilteringLimitedDevice = _anisotropicFilteringLimitedDevice.Get( this ); return _anisotropicFilteringLimitedDevice.value; }
			set { if( _anisotropicFilteringLimitedDevice.BeginSet( ref value ) ) { try { AnisotropicFilteringLimitedDeviceChanged?.Invoke( this ); } finally { _anisotropicFilteringLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropicFilteringLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> AnisotropicFilteringLimitedDeviceChanged;
		ReferenceField<bool> _anisotropicFilteringLimitedDevice = false;

		/// <summary>
		/// The maximal amount of steps for the displacement mapping of materials.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Displacement Max Steps (Restart to apply changes)" )]
		[DefaultValue( 32 )]
		[Range( 0, 64 )]
		public Reference<int> DisplacementMaxSteps
		{
			get { if( _displacementMaxSteps.BeginGet() ) DisplacementMaxSteps = _displacementMaxSteps.Get( this ); return _displacementMaxSteps.value; }
			set { if( _displacementMaxSteps.BeginSet( ref value ) ) { try { DisplacementMaxStepsChanged?.Invoke( this ); } finally { _displacementMaxSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementMaxSteps"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> DisplacementMaxStepsChanged;
		ReferenceField<int> _displacementMaxSteps = 32;

		/// <summary>
		/// The maximal amount of steps for the displacement mapping of materials on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Displacement Max Steps" )]
		[DefaultValue( 0 )]
		[Range( 0, 64 )]
		public Reference<int> DisplacementMaxStepsLimitedDevice
		{
			get { if( _displacementMaxStepsLimitedDevice.BeginGet() ) DisplacementMaxStepsLimitedDevice = _displacementMaxStepsLimitedDevice.Get( this ); return _displacementMaxStepsLimitedDevice.value; }
			set { if( _displacementMaxStepsLimitedDevice.BeginSet( ref value ) ) { try { DisplacementMaxStepsLimitedDeviceChanged?.Invoke( this ); } finally { _displacementMaxStepsLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementMaxStepsLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> DisplacementMaxStepsLimitedDeviceChanged;
		ReferenceField<int> _displacementMaxStepsLimitedDevice = 0;

		/// <summary>
		/// Whether to allow using the technique to remove texture tiling.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Remove Texture Tiling (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> RemoveTextureTiling
		{
			get { if( _removeTextureTiling.BeginGet() ) RemoveTextureTiling = _removeTextureTiling.Get( this ); return _removeTextureTiling.value; }
			set { if( _removeTextureTiling.BeginSet( ref value ) ) { try { RemoveTextureTilingChanged?.Invoke( this ); } finally { _removeTextureTiling.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RemoveTextureTiling"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> RemoveTextureTilingChanged;
		ReferenceField<bool> _removeTextureTiling = true;

		/// <summary>
		/// Whether to allow using the technique to remove texture tiling on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Remove Texture Tiling" )]
		[DefaultValue( false )]
		public Reference<bool> RemoveTextureTilingLimitedDevice
		{
			get { if( _removeTextureTilingLimitedDevice.BeginGet() ) RemoveTextureTilingLimitedDevice = _removeTextureTilingLimitedDevice.Get( this ); return _removeTextureTilingLimitedDevice.value; }
			set { if( _removeTextureTilingLimitedDevice.BeginSet( ref value ) ) { try { RemoveTextureTilingLimitedDeviceChanged?.Invoke( this ); } finally { _removeTextureTilingLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RemoveTextureTilingLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> RemoveTextureTilingLimitedDeviceChanged;
		ReferenceField<bool> _removeTextureTilingLimitedDevice = false;

		/// <summary>
		/// Whether to allow using the motion vectors to enable a motion blur or a temporal antialiasing.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Motion Vector (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> MotionVector
		{
			get { if( _motionVector.BeginGet() ) MotionVector = _motionVector.Get( this ); return _motionVector.value; }
			set { if( _motionVector.BeginSet( ref value ) ) { try { MotionVectorChanged?.Invoke( this ); } finally { _motionVector.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotionVector"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> MotionVectorChanged;
		ReferenceField<bool> _motionVector = true;

		/// <summary>
		/// Whether to allow using the motion vectors to enable a motion blur or a temporal antialiasing on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Motion Vector" )]
		[DefaultValue( false )]
		public Reference<bool> MotionVectorLimitedDevice
		{
			get { if( _motionVectorLimitedDevice.BeginGet() ) MotionVectorLimitedDevice = _motionVectorLimitedDevice.Get( this ); return _motionVectorLimitedDevice.value; }
			set { if( _motionVectorLimitedDevice.BeginSet( ref value ) ) { try { MotionVectorLimitedDeviceChanged?.Invoke( this ); } finally { _motionVectorLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotionVectorLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> MotionVectorLimitedDeviceChanged;
		ReferenceField<bool> _motionVectorLimitedDevice = false;

		/// <summary>
		/// Whether to allow using the indirect lighting in a full mode.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Indirect Lighting Full Mode (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> IndirectLightingFullMode
		{
			get { if( _indirectLightingFullMode.BeginGet() ) IndirectLightingFullMode = _indirectLightingFullMode.Get( this ); return _indirectLightingFullMode.value; }
			set { if( _indirectLightingFullMode.BeginSet( ref value ) ) { try { IndirectLightingFullModeChanged?.Invoke( this ); } finally { _indirectLightingFullMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndirectLightingFullMode"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> IndirectLightingFullModeChanged;
		ReferenceField<bool> _indirectLightingFullMode = true;

		/// <summary>
		/// Whether to allow using the indirect lighting in a full mode on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Indirect Lighting Full Mode" )]
		[DefaultValue( false )]
		public Reference<bool> IndirectLightingFullModeLimitedDevice
		{
			get { if( _indirectLightingFullModeLimitedDevice.BeginGet() ) IndirectLightingFullModeLimitedDevice = _indirectLightingFullModeLimitedDevice.Get( this ); return _indirectLightingFullModeLimitedDevice.value; }
			set { if( _indirectLightingFullModeLimitedDevice.BeginSet( ref value ) ) { try { IndirectLightingFullModeLimitedDeviceChanged?.Invoke( this ); } finally { _indirectLightingFullModeLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndirectLightingFullModeLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> IndirectLightingFullModeLimitedDeviceChanged;
		ReferenceField<bool> _indirectLightingFullModeLimitedDevice = false;

		/// <summary>
		/// The amount of maximal amount of cut volumes.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Cut Volume Max Amount (Restart to apply changes)" )]
		[DefaultValue( 4 )]
		[Range( 0, 10 )]
		public Reference<int> CutVolumeMaxAmount
		{
			get { if( _cutVolumeMaxAmount.BeginGet() ) CutVolumeMaxAmount = _cutVolumeMaxAmount.Get( this ); return _cutVolumeMaxAmount.value; }
			set { if( _cutVolumeMaxAmount.BeginSet( ref value ) ) { try { CutVolumeMaxAmountChanged?.Invoke( this ); } finally { _cutVolumeMaxAmount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CutVolumeMaxAmount"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> CutVolumeMaxAmountChanged;
		ReferenceField<int> _cutVolumeMaxAmount = 4;

		/// <summary>
		/// The amount of maximal amount of cut volumes on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Cut Volume Max Amount" )]
		[DefaultValue( 0 )]
		[Range( 0, 10 )]
		public Reference<int> CutVolumeMaxAmountLimitedDevice
		{
			get { if( _cutVolumeMaxAmountLimitedDevice.BeginGet() ) CutVolumeMaxAmountLimitedDevice = _cutVolumeMaxAmountLimitedDevice.Get( this ); return _cutVolumeMaxAmountLimitedDevice.value; }
			set { if( _cutVolumeMaxAmountLimitedDevice.BeginSet( ref value ) ) { try { CutVolumeMaxAmountLimitedDeviceChanged?.Invoke( this ); } finally { _cutVolumeMaxAmountLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CutVolumeMaxAmountLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> CutVolumeMaxAmountLimitedDeviceChanged;
		ReferenceField<int> _cutVolumeMaxAmountLimitedDevice = 0;

		/// <summary>
		/// Whether to allow using the fog effect.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Fog (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> Fog
		{
			get { if( _fog.BeginGet() ) Fog = _fog.Get( this ); return _fog.value; }
			set { if( _fog.BeginSet( ref value ) ) { try { FogChanged?.Invoke( this ); } finally { _fog.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Fog"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> FogChanged;
		ReferenceField<bool> _fog = true;

		/// <summary>
		/// Whether to allow using the fog effect on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Fog" )]
		[DefaultValue( false )]
		public Reference<bool> FogLimitedDevice
		{
			get { if( _fogLimitedDevice.BeginGet() ) FogLimitedDevice = _fogLimitedDevice.Get( this ); return _fogLimitedDevice.value; }
			set { if( _fogLimitedDevice.BeginSet( ref value ) ) { try { FogLimitedDeviceChanged?.Invoke( this ); } finally { _fogLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FogLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> FogLimitedDeviceChanged;
		ReferenceField<bool> _fogLimitedDevice = false;

		/// <summary>
		/// Whether to enable the smooth transition between levels of detail.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Smooth LOD (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> SmoothLOD
		{
			get { if( _smoothLOD.BeginGet() ) SmoothLOD = _smoothLOD.Get( this ); return _smoothLOD.value; }
			set { if( _smoothLOD.BeginSet( ref value ) ) { try { SmoothLODChanged?.Invoke( this ); } finally { _smoothLOD.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SmoothLOD"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> SmoothLODChanged;
		ReferenceField<bool> _smoothLOD = true;

		/// <summary>
		/// Whether to enable the smooth transition between levels of detail on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Smooth LOD" )]
		[DefaultValue( false )]
		public Reference<bool> SmoothLODLimitedDevice
		{
			get { if( _smoothLODLimitedDevice.BeginGet() ) SmoothLODLimitedDevice = _smoothLODLimitedDevice.Get( this ); return _smoothLODLimitedDevice.value; }
			set { if( _smoothLODLimitedDevice.BeginSet( ref value ) ) { try { SmoothLODLimitedDeviceChanged?.Invoke( this ); } finally { _smoothLODLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SmoothLODLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> SmoothLODLimitedDeviceChanged;
		ReferenceField<bool> _smoothLODLimitedDevice = false;

		/// <summary>
		/// Whether to allow using voxel-based LOD technique.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Voxel LOD (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> VoxelLOD
		{
			get { if( _voxelLOD.BeginGet() ) VoxelLOD = _voxelLOD.Get( this ); return _voxelLOD.value; }
			set { if( _voxelLOD.BeginSet( ref value ) ) { try { VoxelLODChanged?.Invoke( this ); } finally { _voxelLOD.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelLOD"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> VoxelLODChanged;
		ReferenceField<bool> _voxelLOD = true;

		/// <summary>
		/// Whether to allow using voxel-based LOD technique on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Voxel LOD" )]
		[DefaultValue( false )]
		public Reference<bool> VoxelLODLimitedDevice
		{
			get { if( _voxelLODLimitedDevice.BeginGet() ) VoxelLODLimitedDevice = _voxelLODLimitedDevice.Get( this ); return _voxelLODLimitedDevice.value; }
			set { if( _voxelLODLimitedDevice.BeginSet( ref value ) ) { try { VoxelLODLimitedDeviceChanged?.Invoke( this ); } finally { _voxelLODLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelLODLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> VoxelLODLimitedDeviceChanged;
		ReferenceField<bool> _voxelLODLimitedDevice = false;

		/// <summary>
		/// The maximal abount of ray matching steps in the fragment shader.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Voxel LOD Max Steps (Restart to apply changes)" )]
		[DefaultValue( 12 )]
		[Range( 4, 64, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> VoxelLODMaxSteps
		{
			get { if( _voxelLODMaxSteps.BeginGet() ) VoxelLODMaxSteps = _voxelLODMaxSteps.Get( this ); return _voxelLODMaxSteps.value; }
			set { if( _voxelLODMaxSteps.BeginSet( ref value ) ) { try { VoxelLODMaxStepsChanged?.Invoke( this ); } finally { _voxelLODMaxSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelLODMaxSteps"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> VoxelLODMaxStepsChanged;
		ReferenceField<int> _voxelLODMaxSteps = 12;

		/// <summary>
		/// The maximal abount of ray matching steps in the fragment shader on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Voxel LOD Max Steps" )]
		[DefaultValue( 8 )]
		[Range( 4, 64, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> VoxelLODMaxStepsLimitedDevice
		{
			get { if( _voxelLODMaxStepsLimitedDevice.BeginGet() ) VoxelLODMaxStepsLimitedDevice = _voxelLODMaxStepsLimitedDevice.Get( this ); return _voxelLODMaxStepsLimitedDevice.value; }
			set { if( _voxelLODMaxStepsLimitedDevice.BeginSet( ref value ) ) { try { VoxelLODMaxStepsLimitedDeviceChanged?.Invoke( this ); } finally { _voxelLODMaxStepsLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelLODMaxStepsLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> VoxelLODMaxStepsLimitedDeviceChanged;
		ReferenceField<int> _voxelLODMaxStepsLimitedDevice = 8;

		///// <summary>
		///// Whether to allow using virtualized geometry.
		///// </summary>
		//[Category( "Rendering: Basic Device" )]
		//[DisplayName( "Virtualized Geometry (Restart to apply changes)" )]
		//[DefaultValue( true )]
		//public Reference<bool> VirtualizedGeometry
		//{
		//	get { if( _virtualizedGeometry.BeginGet() ) VirtualizedGeometry = _virtualizedGeometry.Get( this ); return _virtualizedGeometry.value; }
		//	set { if( _virtualizedGeometry.BeginSet( ref value ) ) { try { VirtualizedGeometryChanged?.Invoke( this ); } finally { _virtualizedGeometry.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VirtualizedGeometry"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> VirtualizedGeometryChanged;
		//ReferenceField<bool> _virtualizedGeometry = true;

		////!!!!default. what else
		///// <summary>
		///// Whether to allow using virtualized geometry on limited devices (mobile).
		///// </summary>
		//[Category( "Rendering: Limited Device" )]
		//[DisplayName( "Virtualized Geometry" )]
		//[DefaultValue( false )]
		//public Reference<bool> VirtualizedGeometryLimitedDevice
		//{
		//	get { if( _virtualizedGeometryLimitedDevice.BeginGet() ) VirtualizedGeometryLimitedDevice = _virtualizedGeometryLimitedDevice.Get( this ); return _virtualizedGeometryLimitedDevice.value; }
		//	set { if( _virtualizedGeometryLimitedDevice.BeginSet( ref value ) ) { try { VirtualizedGeometryLimitedDeviceChanged?.Invoke( this ); } finally { _virtualizedGeometryLimitedDevice.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VirtualizedGeometryLimitedDevice"/> property value changes.</summary>
		//public event Action<ProjectSettingsPage_Rendering> VirtualizedGeometryLimitedDeviceChanged;
		//ReferenceField<bool> _virtualizedGeometryLimitedDevice = false;

		/// <summary>
		/// Whether to use smooth fading of objects by visibility distance.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Fade By Visibility Distance (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> FadeByVisibilityDistance
		{
			get { if( _fadeByVisibilityDistance.BeginGet() ) FadeByVisibilityDistance = _fadeByVisibilityDistance.Get( this ); return _fadeByVisibilityDistance.value; }
			set { if( _fadeByVisibilityDistance.BeginSet( ref value ) ) { try { FadeByVisibilityDistanceChanged?.Invoke( this ); } finally { _fadeByVisibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FadeByVisibilityDistance"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> FadeByVisibilityDistanceChanged;
		ReferenceField<bool> _fadeByVisibilityDistance = true;

		/// <summary>
		/// Whether to use smooth fading of objects by visibility distance on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Fade By Visibility Distance" )]
		[DefaultValue( false )]
		public Reference<bool> FadeByVisibilityDistanceLimitedDevice
		{
			get { if( _fadeByVisibilityDistanceLimitedDevice.BeginGet() ) FadeByVisibilityDistanceLimitedDevice = _fadeByVisibilityDistanceLimitedDevice.Get( this ); return _fadeByVisibilityDistanceLimitedDevice.value; }
			set { if( _fadeByVisibilityDistanceLimitedDevice.BeginSet( ref value ) ) { try { FadeByVisibilityDistanceLimitedDeviceChanged?.Invoke( this ); } finally { _fadeByVisibilityDistanceLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FadeByVisibilityDistanceLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> FadeByVisibilityDistanceLimitedDeviceChanged;
		ReferenceField<bool> _fadeByVisibilityDistanceLimitedDevice = false;

		/// <summary>
		/// Whether to allow using Debug Mode of the scene.
		/// </summary>
		[Category( "Rendering: Basic Device" )]
		[DisplayName( "Debug Mode (Restart to apply changes)" )]
		[DefaultValue( true )]
		public Reference<bool> DebugMode
		{
			get { if( _debugMode.BeginGet() ) DebugMode = _debugMode.Get( this ); return _debugMode.value; }
			set { if( _debugMode.BeginSet( ref value ) ) { try { DebugModeChanged?.Invoke( this ); } finally { _debugMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugMode"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> DebugModeChanged;
		ReferenceField<bool> _debugMode = true;

		/// <summary>
		/// Whether to allow using Debug Mode of the scene on limited devices (mobile).
		/// </summary>
		[Category( "Rendering: Limited Device" )]
		[DisplayName( "Debug Mode" )]
		[DefaultValue( false )]
		public Reference<bool> DebugModeLimitedDevice
		{
			get { if( _debugModeLimitedDevice.BeginGet() ) DebugModeLimitedDevice = _debugModeLimitedDevice.Get( this ); return _debugModeLimitedDevice.value; }
			set { if( _debugModeLimitedDevice.BeginSet( ref value ) ) { try { DebugModeLimitedDeviceChanged?.Invoke( this ); } finally { _debugModeLimitedDevice.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugModeLimitedDevice"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Rendering> DebugModeLimitedDeviceChanged;
		ReferenceField<bool> _debugModeLimitedDevice = false;

		//!!!!нельзя грузить до инициализации рендера, т.к. MaterialEnvironmentPreview загружается
		//public enum RenderingAPIEnum
		//{
		//	Auto,
		//	DirectX11,
		//	DirectX12,
		//}
		///// <summary>
		///// Rendering API used in the project. Changing the value of this parameter takes effect after the restart of the editor.
		///// </summary>
		//[Category( "Rendering" )]
		//[DefaultValue( RenderingAPIEnum.Auto )]
		//public Reference<RenderingAPIEnum> RenderingAPI
		//{
		//	get { if( _renderingAPI.BeginGet() ) RenderingAPI = _renderingAPI.Get( this ); return _renderingAPI.value; }
		//	set { if( _renderingAPI.BeginSet( ref value ) ) { try { RenderingAPIChanged?.Invoke( this ); } finally { _renderingAPI.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RenderingAPI"/> property value changes.</summary>
		//public event Action<ProjectSettingsComponentPage_Rendering> RenderingAPIChanged;
		//ReferenceField<RenderingAPIEnum> _renderingAPI = RenderingAPIEnum.Auto;
	}
}
