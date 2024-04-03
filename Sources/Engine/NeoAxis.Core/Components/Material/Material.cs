// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Defines how a geometry looks.
	/// </summary>
	[ResourceFileExtension( "material" )]
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.MaterialEditor" )]
	[Preview( "NeoAxis.Editor.MaterialPreview" )]
	[PreviewImage( "NeoAxis.Editor.MaterialPreviewImage" )]
	[SettingsCell( "NeoAxis.Editor.MaterialSettingsCell" )]
	[NewObjectSettings( "NeoAxis.Editor.MaterialNewObjectSettings" )]
#endif
	public partial class Material : ResultCompile<Material.CompiledMaterialData>, IEditorUpdateWhenDocumentModified
	{
		const bool shaderGenerationCompile = true;
		const bool shaderGenerationEnable = true;
		const bool shaderGenerationPrintLog = false;
		//const bool shaderGenerationPrintLog = true;

		//static ESet<Material> all = new ESet<Material>();

		/////////////////////////////////////////

		public enum BlendModeEnum
		{
			Opaque,
			Masked,
			//MaskedLayer,
			Transparent,
			Add,
		}

		/// <summary>
		/// Defines how/if the rendered object is blended with the content of the render target.
		/// </summary>
		[DefaultValue( BlendModeEnum.Opaque )]
		[Serialize]
		[Category( "General" )]
		[FlowGraphBrowsable( false )]
		public Reference<BlendModeEnum> BlendMode
		{
			get { if( _blendMode.BeginGet() ) BlendMode = _blendMode.Get( this ); return _blendMode.value; }
			set
			{
				if( _blendMode.BeginSet( this, ref value ) )
				{
					try
					{
						BlendModeChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _blendMode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BlendMode"/> property value changes.</summary>
		public event Action<Material> BlendModeChanged;
		ReferenceField<BlendModeEnum> _blendMode = BlendModeEnum.Opaque;

		/// <summary>
		/// Whether the material use double-sided rendering.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "General" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> TwoSided
		{
			get { if( _twoSided.BeginGet() ) TwoSided = _twoSided.Get( this ); return _twoSided.value; }
			set
			{
				if( _twoSided.BeginSet( this, ref value ) )
				{
					try
					{
						TwoSidedChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _twoSided.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TwoSided"/> property value changes.</summary>
		public event Action<Material> TwoSidedChanged;
		ReferenceField<bool> _twoSided = false;

		/// <summary>
		/// Whether the double-sided material must flip normals for back faces rendering.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "General" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> TwoSidedFlipNormals
		{
			get { if( _twoSidedFlipNormals.BeginGet() ) TwoSidedFlipNormals = _twoSidedFlipNormals.Get( this ); return _twoSidedFlipNormals.value; }
			set
			{
				if( _twoSidedFlipNormals.BeginSet( this, ref value ) )
				{
					try
					{
						TwoSidedFlipNormalsChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _twoSidedFlipNormals.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TwoSidedFlipNormals"/> property value changes.</summary>
		public event Action<Material> TwoSidedFlipNormalsChanged;
		ReferenceField<bool> _twoSidedFlipNormals = true;

		public enum ShadingModelEnum
		{
			Lit,
			Subsurface,
			Foliage,
			Cloth,
			/// <summary>
			/// Formula is BaseColor * 'light color' + Emissive. Usable for billboard materials.
			/// </summary>
			Simple,
			Unlit
		}

		/// <summary>
		/// The reflection mode of the incoming light.
		/// </summary>
		[DefaultValue( ShadingModelEnum.Lit )]
		[Serialize]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<ShadingModelEnum> ShadingModel
		{
			get { if( _shadingModel.BeginGet() ) ShadingModel = _shadingModel.Get( this ); return _shadingModel.value; }
			set
			{
				if( _shadingModel.BeginSet( this, ref value ) )
				{
					try
					{
						ShadingModelChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _shadingModel.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShadingModel"/> property value changes.</summary>
		public event Action<Material> ShadingModelChanged;
		ReferenceField<ShadingModelEnum> _shadingModel = ShadingModelEnum.Lit;

		/// <summary>
		/// Diffuse albedo for non-metallic surfaces, and specular color for metallic surfaces.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		[Category( "Shading" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> BaseColor
		{
			get { if( _baseColor.BeginGet() ) BaseColor = _baseColor.Get( this ); return _baseColor.value; }
			set { if( _baseColor.BeginSet( this, ref value ) ) { try { BaseColorChanged?.Invoke( this ); } finally { _baseColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseColor"/> property value changes.</summary>
		public event Action<Material> BaseColorChanged;
		ReferenceField<ColorValue> _baseColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Whether a surface appears to be dielectric (0.0) or conductor (1.0). Often used as a binary value (0 or 1).
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> Metallic
		{
			get { if( _metallic.BeginGet() ) Metallic = _metallic.Get( this ); return _metallic.value; }
			set { if( _metallic.BeginSet( this, ref value ) ) { try { MetallicChanged?.Invoke( this ); } finally { _metallic.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Metallic"/> property value changes.</summary>
		public event Action<Material> MetallicChanged;
		ReferenceField<double> _metallic = 0.0;

		/// <summary>
		/// Perceived smoothness (1.0) or roughness (0.0) of a surface. Smooth surfaces exhibit sharp reflections.
		/// </summary>
		[DefaultValue( 0.5 )]//1.0 )]
		[Serialize]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> Roughness
		{
			get { if( _roughness.BeginGet() ) Roughness = _roughness.Get( this ); return _roughness.value; }
			set { if( _roughness.BeginSet( this, ref value ) ) { try { RoughnessChanged?.Invoke( this ); } finally { _roughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Roughness"/> property value changes.</summary>
		public event Action<Material> RoughnessChanged;
		ReferenceField<double> _roughness = 0.5;//1.0;

		/// <summary>
		/// Fresnel reflectance at normal incidence for dielectric surfaces. This directly controls the strength of the reflections.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> Reflectance
		{
			get { if( _reflectance.BeginGet() ) Reflectance = _reflectance.Get( this ); return _reflectance.value; }
			set { if( _reflectance.BeginSet( this, ref value ) ) { try { ReflectanceChanged?.Invoke( this ); } finally { _reflectance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Reflectance"/> property value changes.</summary>
		public event Action<Material> ReflectanceChanged;
		ReferenceField<double> _reflectance = 0.5;

		/// <summary>
		/// The material normals. This property is intented to be used with normal mapping.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0" )]
		[Category( "Shading" )]
		public Reference<Vector3> Normal
		{
			get { if( _normal.BeginGet() ) Normal = _normal.Get( this ); return _normal.value; }
			set { if( _normal.BeginSet( this, ref value ) ) { try { NormalChanged?.Invoke( this ); } finally { _normal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Normal"/> property value changes.</summary>
		public event Action<Material> NormalChanged;
		ReferenceField<Vector3> _normal;

		public enum DisplacementMethodEnum
		{
			DisplacementMapping,
			Tessellation,
		}

		/// <summary>
		/// The way to calculate height displacement. You can use displacement mapping, the data of heights are taken from Displacement property. Tessellation method is another more accurate way to render the displacement.
		/// </summary>
		[Category( "Shading" )]
		[DefaultValue( DisplacementMethodEnum.DisplacementMapping )]
		[FlowGraphBrowsable( false )]
		public Reference<DisplacementMethodEnum> DisplacementTechnique
		{
			get { if( _displacementTechnique.BeginGet() ) DisplacementTechnique = _displacementTechnique.Get( this ); return _displacementTechnique.value; }
			set { if( _displacementTechnique.BeginSet( this, ref value ) ) { try { DisplacementTechniqueChanged?.Invoke( this ); } finally { _displacementTechnique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementTechnique"/> property value changes.</summary>
		public event Action<Material> DisplacementTechniqueChanged;
		ReferenceField<DisplacementMethodEnum> _displacementTechnique = DisplacementMethodEnum.DisplacementMapping;

		/// <summary>
		/// The height offset that is specified by the texture.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Shading" )]
		public Reference<double> Displacement
		{
			get { if( _displacement.BeginGet() ) Displacement = _displacement.Get( this ); return _displacement.value; }
			set { if( _displacement.BeginSet( this, ref value ) ) { try { DisplacementChanged?.Invoke( this ); } finally { _displacement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Displacement"/> property value changes.</summary>
		public event Action<Material> DisplacementChanged;
		ReferenceField<double> _displacement = 0.0;

		//!!!!? DisplacementMultiply, DisplacementAdd

		/// <summary>
		/// The scale for Displacement.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Range( 0, 0.2 )]
		[Category( "Shading" )]
		public Reference<double> DisplacementScale
		{
			get { if( _displacementScale.BeginGet() ) DisplacementScale = _displacementScale.Get( this ); return _displacementScale.value; }
			set { if( _displacementScale.BeginSet( this, ref value ) ) { try { DisplacementScaleChanged?.Invoke( this ); } finally { _displacementScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementScale"/> property value changes.</summary>
		public event Action<Material> DisplacementScaleChanged;
		ReferenceField<double> _displacementScale = 0.05;

		/// <summary>
		/// The quality multiplier of the tessellation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Shading" )]
		[Range( 0.25, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> TessellationQuality
		{
			get { if( _tessellationQuality.BeginGet() ) TessellationQuality = _tessellationQuality.Get( this ); return _tessellationQuality.value; }
			set { if( _tessellationQuality.BeginSet( this, ref value ) ) { try { TessellationQualityChanged?.Invoke( this ); } finally { _tessellationQuality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TessellationQuality"/> property value changes.</summary>
		public event Action<Material> TessellationQualityChanged;
		ReferenceField<double> _tessellationQuality = 1.0;

		/// <summary>
		/// Defines how much of the light is accessible to a surface point.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Category( "Shading" )]
		//[ApplicableRange( 0, 1 )]
		public Reference<double> AmbientOcclusion
		{
			get { if( _ambientOcclusion.BeginGet() ) AmbientOcclusion = _ambientOcclusion.Get( this ); return _ambientOcclusion.value; }
			set { if( _ambientOcclusion.BeginSet( this, ref value ) ) { try { AmbientOcclusionChanged?.Invoke( this ); } finally { _ambientOcclusion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AmbientOcclusion"/> property value changes.</summary>
		public event Action<Material> AmbientOcclusionChanged;
		ReferenceField<double> _ambientOcclusion;

		/// <summary>
		/// The color and intensity of light emitted from the surface.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1; 0" )]
		[Category( "Shading" )]
		[ColorValueNoAlpha]
		[ApplicableRangeColorValuePower( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<ColorValuePowered> Emissive
		{
			get { if( _emissive.BeginGet() ) Emissive = _emissive.Get( this ); return _emissive.value; }
			set { if( _emissive.BeginSet( this, ref value ) ) { try { EmissiveChanged?.Invoke( this ); } finally { _emissive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Emissive"/> property value changes.</summary>
		public event Action<Material> EmissiveChanged;
		ReferenceField<ColorValuePowered> _emissive = new ColorValuePowered( 1, 1, 1, 1, 0 );

		/// <summary>
		/// The opacity of the surface.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> Opacity
		{
			get { if( _opacity.BeginGet() ) Opacity = _opacity.Get( this ); return _opacity.value; }
			set { if( _opacity.BeginSet( this, ref value ) ) { try { OpacityChanged?.Invoke( this ); } finally { _opacity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Opacity"/> property value changes.</summary>
		public event Action<Material> OpacityChanged;
		ReferenceField<double> _opacity = 1;

		/// <summary>
		/// Whether to enable dithering for smooth blending of Masked material.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> OpacityDithering
		{
			get { if( _opacityDithering.BeginGet() ) OpacityDithering = _opacityDithering.Get( this ); return _opacityDithering.value; }
			set
			{
				if( _opacityDithering.BeginSet( this, ref value ) )
				{
					try
					{
						OpacityDitheringChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _opacityDithering.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OpacityDithering"/> property value changes.</summary>
		public event Action<Material> OpacityDitheringChanged;
		ReferenceField<bool> _opacityDithering = false;

		/// <summary>
		/// Transparency threshold for Masked mode.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> OpacityMaskThreshold
		{
			get { if( _opacityMaskThreshold.BeginGet() ) OpacityMaskThreshold = _opacityMaskThreshold.Get( this ); return _opacityMaskThreshold.value; }
			set { if( _opacityMaskThreshold.BeginSet( this, ref value ) ) { try { OpacityMaskThresholdChanged?.Invoke( this ); } finally { _opacityMaskThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpacityMaskThreshold"/> property value changes.</summary>
		public event Action<Material> OpacityMaskThresholdChanged;
		ReferenceField<double> _opacityMaskThreshold = 0.5;

		/// <summary>
		/// Strength of the clear coat layer.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> ClearCoat
		{
			get { if( _clearCoat.BeginGet() ) ClearCoat = _clearCoat.Get( this ); return _clearCoat.value; }
			set
			{
				var oldValue = _clearCoat.value.Value;
				if( _clearCoat.BeginSet( this, ref value ) )
				{
					try
					{
						ClearCoatChanged?.Invoke( this );

						//!!!!если компилировать чтобы было несколько комбинаций, то не надо перекомпилироать если стало == 0
						if( ( oldValue == 0 && value.Value != 0 ) || ( oldValue != 0 && value.Value == 0 ) )
							ShouldRecompile = true;
					}
					finally { _clearCoat.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ClearCoat"/> property value changes.</summary>
		public event Action<Material> ClearCoatChanged;
		ReferenceField<double> _clearCoat = 0.0;

		/// <summary>
		/// Perceived smoothness or roughness of the clear coat layer.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> ClearCoatRoughness
		{
			get { if( _clearCoatRoughness.BeginGet() ) ClearCoatRoughness = _clearCoatRoughness.Get( this ); return _clearCoatRoughness.value; }
			set { if( _clearCoatRoughness.BeginSet( this, ref value ) ) { try { ClearCoatRoughnessChanged?.Invoke( this ); } finally { _clearCoatRoughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClearCoatRoughness"/> property value changes.</summary>
		public event Action<Material> ClearCoatRoughnessChanged;
		ReferenceField<double> _clearCoatRoughness = 0.5;

		/// <summary>
		/// A detail normal used to perturb the clear coat layer using bump mapping (normal mapping).
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<Vector3> ClearCoatNormal
		{
			get { if( _clearCoatNormal.BeginGet() ) ClearCoatNormal = _clearCoatNormal.Get( this ); return _clearCoatNormal.value; }
			set { if( _clearCoatNormal.BeginSet( this, ref value ) ) { try { ClearCoatNormalChanged?.Invoke( this ); } finally { _clearCoatNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClearCoatNormal"/> property value changes.</summary>
		public event Action<Material> ClearCoatNormalChanged;
		ReferenceField<Vector3> _clearCoatNormal;

		/// <summary>
		/// Amount of anisotropy.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> Anisotropy
		{
			get { if( _anisotropy.BeginGet() ) Anisotropy = _anisotropy.Get( this ); return _anisotropy.value; }
			set
			{
				var oldValue = _anisotropy.value.Value;
				if( _anisotropy.BeginSet( this, ref value ) )
				{
					try
					{
						AnisotropyChanged?.Invoke( this );

						//!!!!если компилировать чтобы было несколько комбинаций, то не надо перекомпилироать если стало == 0
						if( ( oldValue == 0 && value.Value != 0 ) || ( oldValue != 0 && value.Value == 0 ) )
							ShouldRecompile = true;
					}
					finally { _anisotropy.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Anisotropy"/> property value changes.</summary>
		public event Action<Material> AnisotropyChanged;
		ReferenceField<double> _anisotropy = 0.0;

		///// <summary>
		///// Amount of anisotropy in either the tangent or bitangent direction.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( -1, 1 )]
		//[Serialize]
		//[Category( "Shading" )]
		//public Reference<double> Anisotropy
		//{
		//	get { if( _anisotropy.BeginGet() ) Anisotropy = _anisotropy.Get( this ); return _anisotropy.value; }
		//	set { if( _anisotropy.BeginSet( this, ref value ) ) { try { AnisotropyChanged?.Invoke( this ); } finally { _anisotropy.EndSet(); } } }
		//}
		//public event Action<Material> AnisotropyChanged;
		//ReferenceField<double> _anisotropy = 0.0;

		/// <summary>
		/// Local surface direction. Used by anisotropic materials only.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 0 0" )]
		[Category( "Shading" )]
		public Reference<Vector3> AnisotropyDirection
		{
			get { if( _anisotropyDirection.BeginGet() ) AnisotropyDirection = _anisotropyDirection.Get( this ); return _anisotropyDirection.value; }
			set { if( _anisotropyDirection.BeginSet( this, ref value ) ) { try { AnisotropyDirectionChanged?.Invoke( this ); } finally { _anisotropyDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropyDirection"/> property value changes.</summary>
		public event Action<Material> AnisotropyDirectionChanged;
		ReferenceField<Vector3> _anisotropyDirection = new Vector3( 1, 0, 0 );

		public enum AnisotropyDirectionBasisEnum
		{
			Tangent,
			Bitangent,
		}

		/// <summary>
		/// The direction basis. Used by anisotropic materials only.
		/// </summary>
		[DefaultValue( AnisotropyDirectionBasisEnum.Tangent )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<AnisotropyDirectionBasisEnum> AnisotropyDirectionBasis
		{
			get { if( _anisotropyDirectionBasis.BeginGet() ) AnisotropyDirectionBasis = _anisotropyDirectionBasis.Get( this ); return _anisotropyDirectionBasis.value; }
			set { if( _anisotropyDirectionBasis.BeginSet( this, ref value ) ) { try { AnisotropyDirectionBasisChanged?.Invoke( this ); } finally { _anisotropyDirectionBasis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropyDirectionBasis"/> property value changes.</summary>
		public event Action<Material> AnisotropyDirectionBasisChanged;
		ReferenceField<AnisotropyDirectionBasisEnum> _anisotropyDirectionBasis = AnisotropyDirectionBasisEnum.Tangent;

		/// <summary>
		/// A thickness factor of the surface for subsurface scattering rendering.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( this, ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<Material> ThicknessChanged;
		ReferenceField<double> _thickness = 0.5;

		/// <summary>
		/// A power parameter for subsurface scattering rendering.
		/// </summary>
		[DefaultValue( 12.234 )]
		[Range( 0, 15 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> SubsurfacePower
		{
			get { if( _subsurfacePower.BeginGet() ) SubsurfacePower = _subsurfacePower.Get( this ); return _subsurfacePower.value; }
			set { if( _subsurfacePower.BeginSet( this, ref value ) ) { try { SubsurfacePowerChanged?.Invoke( this ); } finally { _subsurfacePower.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SubsurfacePower"/> property value changes.</summary>
		public event Action<Material> SubsurfacePowerChanged;
		ReferenceField<double> _subsurfacePower = 12.234;

		/// <summary>
		/// Specular tint to create two-tone specular fabrics.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		[Category( "Shading" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> SheenColor
		{
			get { if( _sheenColor.BeginGet() ) SheenColor = _sheenColor.Get( this ); return _sheenColor.value; }
			set { if( _sheenColor.BeginSet( this, ref value ) ) { try { SheenColorChanged?.Invoke( this ); } finally { _sheenColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SheenColor"/> property value changes.</summary>
		public event Action<Material> SheenColorChanged;
		ReferenceField<ColorValue> _sheenColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Tint for the diffuse color after scattering and absorption through the material.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		[Category( "Shading" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> SubsurfaceColor
		{
			get { if( _subsurfaceColor.BeginGet() ) SubsurfaceColor = _subsurfaceColor.Get( this ); return _subsurfaceColor.value; }
			set { if( _subsurfaceColor.BeginSet( this, ref value ) ) { try { SubsurfaceColorChanged?.Invoke( this ); } finally { _subsurfaceColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SubsurfaceColor"/> property value changes.</summary>
		public event Action<Material> SubsurfaceColorChanged;
		ReferenceField<ColorValue> _subsurfaceColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Vertices position offset. Performed in vertex shader.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0" )]
		[Category( "General" )]
		public Reference<Vector3> PositionOffset
		{
			get { if( _positionOffset.BeginGet() ) PositionOffset = _positionOffset.Get( this ); return _positionOffset.value; }
			set
			{
				if( _positionOffset.BeginSet( this, ref value ) )
				{
					try
					{
						PositionOffsetChanged?.Invoke( this );
						//!!!!
					}
					finally { _positionOffset.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		public event Action<Material> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset;

		///// <summary>
		///// Amount of ray-tracing reflection. Used as option to use screen space or raytracing reflection technique.
		///// </summary>
		//[DefaultValue( 0.0 )]//!!!! 1.0 )]
		//[Serialize]
		//[Category( "Shading" )]
		//[Range( 0, 1 )]
		//public Reference<double> RayTracingReflection
		//{
		//	get { if( _rayTracingReflection.BeginGet() ) RayTracingReflection = _rayTracingReflection.Get( this ); return _rayTracingReflection.value; }
		//	set { if( _rayTracingReflection.BeginSet( this, ref value ) ) { try { RayTracingReflectionChanged?.Invoke( this ); } finally { _rayTracingReflection.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RayTracingReflection"/> property value changes.</summary>
		//public event Action<Material> RayTracingReflectionChanged;
		//ReferenceField<double> _rayTracingReflection = 0.0;//!!!! 1.0;

		/// <summary>
		/// Whether the surface receive shadows from other sources.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> ReceiveShadows
		{
			get { if( _receiveShadows.BeginGet() ) ReceiveShadows = _receiveShadows.Get( this ); return _receiveShadows.value; }
			set
			{
				if( _receiveShadows.BeginSet( this, ref value ) )
				{
					try
					{
						ReceiveShadowsChanged?.Invoke( this );
						ShouldRecompile = true;
					}
					finally { _receiveShadows.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ReceiveShadows"/> property value changes.</summary>
		public event Action<Material> ReceiveShadowsChanged;
		ReferenceField<bool> _receiveShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals on the surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( this, ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Material> ReceiveDecalsChanged;
		ReferenceField<bool> _receiveDecals = true;

		/// <summary>
		/// Whether to use vertex color for Base Color and Opacity calculation.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> UseVertexColor
		{
			get { if( _useVertexColor.BeginGet() ) UseVertexColor = _useVertexColor.Get( this ); return _useVertexColor.value; }
			set { if( _useVertexColor.BeginSet( this, ref value ) ) { try { UseVertexColorChanged?.Invoke( this ); } finally { _useVertexColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseVertexColor"/> property value changes.</summary>
		public event Action<Material> UseVertexColorChanged;
		ReferenceField<bool> _useVertexColor = true;

		/// <summary>
		/// Whether to enable a soft particles mode. In this mode, objects increase transparency in those places behind which there is an obstacle nearby. The mode works only for transparent materials.
		/// </summary>
		[Category( "Shading" )]
		[DefaultValue( false )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> SoftParticles
		{
			get { if( _softParticles.BeginGet() ) SoftParticles = _softParticles.Get( this ); return _softParticles.value; }
			set { if( _softParticles.BeginSet( this, ref value ) ) { try { SoftParticlesChanged?.Invoke( this ); ShouldRecompile = true; } finally { _softParticles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoftParticles"/> property value changes.</summary>
		public event Action<Material> SoftParticlesChanged;
		ReferenceField<bool> _softParticles = false;

		/// <summary>
		/// A minimal distance to an obstacle to activate the soft particles mode.
		/// </summary>
		[Category( "Shading" )]
		[DefaultValue( 3.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> SoftParticlesDistance
		{
			get { if( _softParticlesDistance.BeginGet() ) SoftParticlesDistance = _softParticlesDistance.Get( this ); return _softParticlesDistance.value; }
			set { if( _softParticlesDistance.BeginSet( this, ref value ) ) { try { SoftParticlesDistanceChanged?.Invoke( this ); } finally { _softParticlesDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoftParticlesDistance"/> property value changes.</summary>
		public event Action<Material> SoftParticlesDistanceChanged;
		ReferenceField<double> _softParticlesDistance = 3.0;

		public enum DepthOffsetModeEnum
		{
			None,
			GreaterOrEqual,
			LessOrEqual
		}

		/// <summary>
		/// The depth offset mode gives the ability to change output depth in the fragment shader. Works only for deferred shading.
		/// </summary>
		[Category( "Shading" )]
		[DefaultValue( DepthOffsetModeEnum.None )]
		[FlowGraphBrowsable( false )]
		public Reference<DepthOffsetModeEnum> DepthOffsetMode
		{
			get { if( _depthOffsetMode.BeginGet() ) DepthOffsetMode = _depthOffsetMode.Get( this ); return _depthOffsetMode.value; }
			set { if( _depthOffsetMode.BeginSet( this, ref value ) ) { try { DepthOffsetModeChanged?.Invoke( this ); } finally { _depthOffsetMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthOffsetMode"/> property value changes.</summary>
		public event Action<Material> DepthOffsetModeChanged;
		ReferenceField<DepthOffsetModeEnum> _depthOffsetMode = DepthOffsetModeEnum.None;

		/// <summary>
		/// The depth offset value. Limitation: You can specify the value by means additional block in shader graph, but can't set value in this property.
		/// </summary>
		[Category( "Shading" )]
		[DefaultValue( 0.0 )]
		public Reference<double> DepthOffset
		{
			get { if( _depthOffset.BeginGet() ) DepthOffset = _depthOffset.Get( this ); return _depthOffset.value; }
			set { if( _depthOffset.BeginSet( this, ref value ) ) { try { DepthOffsetChanged?.Invoke( this ); } finally { _depthOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthOffset"/> property value changes.</summary>
		public event Action<Material> DepthOffsetChanged;
		ReferenceField<double> _depthOffset = 0.0;

		/// <summary>
		/// Enables advanced blending mode. In this mode, it is possible to configure blending for each channel separately.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> AdvancedBlending
		{
			get { if( _advancedBlending.BeginGet() ) AdvancedBlending = _advancedBlending.Get( this ); return _advancedBlending.value; }
			set { if( _advancedBlending.BeginSet( this, ref value ) ) { try { AdvancedBlendingChanged?.Invoke( this ); } finally { _advancedBlending.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AdvancedBlending"/> property value changes.</summary>
		public event Action<Material> AdvancedBlendingChanged;
		ReferenceField<bool> _advancedBlending = false;

		/// <summary>
		/// Whether to write the Base Color.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectBaseColor
		{
			get { if( _affectBaseColor.BeginGet() ) AffectBaseColor = _affectBaseColor.Get( this ); return _affectBaseColor.value; }
			set { if( _affectBaseColor.BeginSet( this, ref value ) ) { try { AffectBaseColorChanged?.Invoke( this ); } finally { _affectBaseColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectBaseColor"/> property value changes.</summary>
		public event Action<Material> AffectBaseColorChanged;
		ReferenceField<int> _affectBaseColor = 1;

		/// <summary>
		/// Whether to write the Metallic.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectMetallic
		{
			get { if( _affectMetallic.BeginGet() ) AffectMetallic = _affectMetallic.Get( this ); return _affectMetallic.value; }
			set { if( _affectMetallic.BeginSet( this, ref value ) ) { try { AffectMetallicChanged?.Invoke( this ); } finally { _affectMetallic.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectMetallic"/> property value changes.</summary>
		public event Action<Material> AffectMetallicChanged;
		ReferenceField<int> _affectMetallic = 1;

		/// <summary>
		/// Whether to write the Roughness.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectRoughness
		{
			get { if( _affectRoughness.BeginGet() ) AffectRoughness = _affectRoughness.Get( this ); return _affectRoughness.value; }
			set { if( _affectRoughness.BeginSet( this, ref value ) ) { try { AffectRoughnessChanged?.Invoke( this ); } finally { _affectRoughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectRoughness"/> property value changes.</summary>
		public event Action<Material> AffectRoughnessChanged;
		ReferenceField<int> _affectRoughness = 1;

		/// <summary>
		/// Whether to write the Reflectance.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectReflectance
		{
			get { if( _affectReflectance.BeginGet() ) AffectReflectance = _affectReflectance.Get( this ); return _affectReflectance.value; }
			set { if( _affectReflectance.BeginSet( this, ref value ) ) { try { AffectReflectanceChanged?.Invoke( this ); } finally { _affectReflectance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectReflectance"/> property value changes.</summary>
		public event Action<Material> AffectReflectanceChanged;
		ReferenceField<int> _affectReflectance = 1;

		/// <summary>
		/// Whether to write the Ambient Occlusion.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectAmbientOcclusion
		{
			get { if( _affectAmbientOcclusion.BeginGet() ) AffectAmbientOcclusion = _affectAmbientOcclusion.Get( this ); return _affectAmbientOcclusion.value; }
			set { if( _affectAmbientOcclusion.BeginSet( this, ref value ) ) { try { AffectAmbientOcclusionChanged?.Invoke( this ); } finally { _affectAmbientOcclusion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectAmbientOcclusion"/> property value changes.</summary>
		public event Action<Material> AffectAmbientOcclusionChanged;
		ReferenceField<int> _affectAmbientOcclusion = 1;

		/// <summary>
		/// Whether to write the Emissive.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectEmissive
		{
			get { if( _affectEmissive.BeginGet() ) AffectEmissive = _affectEmissive.Get( this ); return _affectEmissive.value; }
			set { if( _affectEmissive.BeginSet( this, ref value ) ) { try { AffectEmissiveChanged?.Invoke( this ); } finally { _affectEmissive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectEmissive"/> property value changes.</summary>
		public event Action<Material> AffectEmissiveChanged;
		ReferenceField<int> _affectEmissive = 1;

		/// <summary>
		/// Whether to write the normals, height data.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<int> AffectGeometry
		{
			get { if( _affectGeometry.BeginGet() ) AffectGeometry = _affectGeometry.Get( this ); return _affectGeometry.value; }
			set { if( _affectGeometry.BeginSet( this, ref value ) ) { try { AffectGeometryChanged?.Invoke( this ); } finally { _affectGeometry.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectGeometry"/> property value changes.</summary>
		public event Action<Material> AffectGeometryChanged;
		ReferenceField<int> _affectGeometry = 1;

		/// <summary>
		/// Whether to enable the advanced shader scripting functionality.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Advanced Scripting" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> AdvancedScripting
		{
			get { if( _advancedScripting.BeginGet() ) AdvancedScripting = _advancedScripting.Get( this ); return _advancedScripting.value; }
			set { if( _advancedScripting.BeginSet( this, ref value ) ) { try { AdvancedScriptingChanged?.Invoke( this ); ShouldRecompile = true; } finally { _advancedScripting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AdvancedScripting"/> property value changes.</summary>
		public event Action<Material> AdvancedScriptingChanged;
		ReferenceField<bool> _advancedScripting = false;

		/// <summary>
		/// Additional functions to the vertex shader code.
		/// </summary>		
		[DefaultValue( "" )]
		[Category( "Advanced Scripting" )]
		//!!!!
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		[FlowGraphBrowsable( false )]
		public Reference<string> VertexFunctions
		{
			get { if( _vertexFunctions.BeginGet() ) VertexFunctions = _vertexFunctions.Get( this ); return _vertexFunctions.value; }
			set { if( _vertexFunctions.BeginSet( this, ref value ) ) { try { VertexFunctionsChanged?.Invoke( this ); ShouldRecompile = true; } finally { _vertexFunctions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VertexFunctions"/> property value changes.</summary>
		public event Action<Material> VertexFunctionsChanged;
		ReferenceField<string> _vertexFunctions = "";

		/// <summary>
		/// The injection to the vertex shader code.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Advanced Scripting" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		[FlowGraphBrowsable( false )]
		public Reference<string> VertexCode
		{
			get { if( _vertexCode.BeginGet() ) VertexCode = _vertexCode.Get( this ); return _vertexCode.value; }
			set { if( _vertexCode.BeginSet( this, ref value ) ) { try { VertexCodeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _vertexCode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VertexCode"/> property value changes.</summary>
		public event Action<Material> VertexCodeChanged;
		ReferenceField<string> _vertexCode = "";

		/// <summary>
		/// Additional functions to the fragment shader code.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Advanced Scripting" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		[FlowGraphBrowsable( false )]
		public Reference<string> FragmentFunctions
		{
			get { if( _fragmentFunctions.BeginGet() ) FragmentFunctions = _fragmentFunctions.Get( this ); return _fragmentFunctions.value; }
			set { if( _fragmentFunctions.BeginSet( this, ref value ) ) { try { FragmentFunctionsChanged?.Invoke( this ); ShouldRecompile = true; } finally { _fragmentFunctions.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FragmentFunctions"/> property value changes.</summary>
		public event Action<Material> FragmentFunctionsChanged;
		ReferenceField<string> _fragmentFunctions = "";

		/// <summary>
		/// The injection to the fragment shader code.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Advanced Scripting" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		[FlowGraphBrowsable( false )]
		public Reference<string> FragmentCode
		{
			get { if( _fragmentCode.BeginGet() ) FragmentCode = _fragmentCode.Get( this ); return _fragmentCode.value; }
			set { if( _fragmentCode.BeginSet( this, ref value ) ) { try { FragmentCodeChanged?.Invoke( this ); ShouldRecompile = true; } finally { _fragmentCode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FragmentCode"/> property value changes.</summary>
		public event Action<Material> FragmentCodeChanged;
		ReferenceField<string> _fragmentCode = "";

		/// <summary>
		/// The parameter is intended to transfer custom data to shader scripts.
		/// </summary>
		[DefaultValue( "0 0 0 0" )]
		[Category( "Advanced Scripting" )]
		[DisplayName( "Custom Parameter 1" )]
		public Reference<Vector4> CustomParameter1
		{
			get { if( _customParameter1.BeginGet() ) CustomParameter1 = _customParameter1.Get( this ); return _customParameter1.value; }
			set { if( _customParameter1.BeginSet( this, ref value ) ) { try { CustomParameter1Changed?.Invoke( this ); } finally { _customParameter1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomParameter1"/> property value changes.</summary>
		public event Action<Material> CustomParameter1Changed;
		ReferenceField<Vector4> _customParameter1;

		/// <summary>
		/// The parameter is intended to transfer custom data to shader scripts.
		/// </summary>
		[DefaultValue( "0 0 0 0" )]
		[Category( "Advanced Scripting" )]
		[DisplayName( "Custom Parameter 2" )]
		public Reference<Vector4> CustomParameter2
		{
			get { if( _customParameter2.BeginGet() ) CustomParameter2 = _customParameter2.Get( this ); return _customParameter2.value; }
			set { if( _customParameter2.BeginSet( this, ref value ) ) { try { CustomParameter2Changed?.Invoke( this ); } finally { _customParameter2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CustomParameter2"/> property value changes.</summary>
		public event Action<Material> CustomParameter2Changed;
		ReferenceField<Vector4> _customParameter2;

		/// <summary>
		/// Whether to allow static shadow optimization for this material.
		/// </summary>
		[Category( "Optimization" )]
		[DefaultValue( true )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> StaticShadows
		{
			get { if( _staticShadows.BeginGet() ) StaticShadows = _staticShadows.Get( this ); return _staticShadows.value; }
			set { if( _staticShadows.BeginSet( this, ref value ) ) { try { StaticShadowsChanged?.Invoke( this ); ShouldRecompile = true; } finally { _staticShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StaticShadows"/> property value changes.</summary>
		public event Action<Material> StaticShadowsChanged;
		ReferenceField<bool> _staticShadows = true;

		//!!!!CastShadows?
		//!!!!!!если выключен, то не нужно генерить special shadow caster data

		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		///////////////////////////////////////////////

		public class NewMaterialData
		{
			//public int Index;
			//public string Name;
			//public ShadingModelEnum ShadingModel = ShadingModelEnum.Lit;
			//public bool TwoSided;

			//public ColorValue? BaseColor;
			public string BaseColorTexture;
			public string MetallicTexture;
			public string RoughnessTexture;
			public string NormalTexture;
			public string DisplacementTexture;
			public string AmbientOcclusionTexture;
			public string EmissiveTexture;
			public string OpacityTexture;

			public string GetTextureValueByName( string name )
			{
				switch( name )
				{
				case "BaseColor": return BaseColorTexture;
				case "Metallic": return MetallicTexture;
				case "Roughness": return RoughnessTexture;
				case "Normal": return NormalTexture;
				case "Displacement": return DisplacementTexture;
				case "AmbientOcclusion": return AmbientOcclusionTexture;
				case "Emissive": return EmissiveTexture;
				case "Opacity": return OpacityTexture;
				}
				return "";
			}

			public void SetTextureValueByName( string name, string value )
			{
				switch( name )
				{
				case "BaseColor": BaseColorTexture = value; break;
				case "Metallic": MetallicTexture = value; break;
				case "Roughness": RoughnessTexture = value; break;
				case "Normal": NormalTexture = value; break;
				case "Displacement": DisplacementTexture = value; break;
				case "AmbientOcclusion": AmbientOcclusionTexture = value; break;
				case "Emissive": EmissiveTexture = value; break;
				case "Opacity": OpacityTexture = value; break;
				}
			}

			public int GetTextureCount()
			{
				int result = 0;
				if( !string.IsNullOrEmpty( BaseColorTexture ) )
					result++;
				if( !string.IsNullOrEmpty( MetallicTexture ) )
					result++;
				if( !string.IsNullOrEmpty( RoughnessTexture ) )
					result++;
				if( !string.IsNullOrEmpty( NormalTexture ) )
					result++;
				if( !string.IsNullOrEmpty( DisplacementTexture ) )
					result++;
				if( !string.IsNullOrEmpty( AmbientOcclusionTexture ) )
					result++;
				if( !string.IsNullOrEmpty( EmissiveTexture ) )
					result++;
				if( !string.IsNullOrEmpty( OpacityTexture ) )
					result++;
				return result;
			}

			public NewMaterialData Clone()
			{
				var result = new NewMaterialData();

				result.BaseColorTexture = BaseColorTexture;
				result.MetallicTexture = MetallicTexture;
				result.RoughnessTexture = RoughnessTexture;
				result.NormalTexture = NormalTexture;
				result.DisplacementTexture = DisplacementTexture;
				result.AmbientOcclusionTexture = AmbientOcclusionTexture;
				result.EmissiveTexture = EmissiveTexture;
				result.OpacityTexture = OpacityTexture;

				return result;
			}
		}

		///////////////////////////////////////////////

		public Material()
		{
			//lock( all )
			//	all.Add( this );
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			//lock( all )
			//	all.Remove( this );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( ReceiveShadows ):
					if( ShadingModel.Value == ShadingModelEnum.Unlit )
						skip = true;
					break;

				case nameof( TwoSidedFlipNormals ):
					if( !TwoSided )
						skip = true;
					break;

				case nameof( AmbientOcclusion ):
				case nameof( Normal ):
					{
						var m = ShadingModel.Value;
						if( m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
							skip = true;
					}
					break;

				case nameof( Displacement ):
					{
						var m = ShadingModel.Value;
						if( m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
							skip = true;
					}
					break;

				case nameof( DisplacementScale ):
					{
						if( !Displacement.ReferenceSpecified )
							skip = true;
						else
						{
							var m = ShadingModel.Value;
							if( m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
								skip = true;
						}
					}
					break;

				case nameof( TessellationQuality ):
					{
						if( DisplacementTechnique.Value != DisplacementMethodEnum.Tessellation )
							skip = true;
						else
						{
							var m = ShadingModel.Value;
							if( m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
								skip = true;
						}
					}
					break;

				case nameof( Opacity ):
					if( BlendMode.Value == BlendModeEnum.Opaque )//|| BlendMode.Value == BlendModeEnum.MaskedLayer )
						skip = true;
					break;

				case nameof( OpacityDithering ):
				case nameof( OpacityMaskThreshold ):
					if( BlendMode.Value != BlendModeEnum.Masked )
						skip = true;
					break;

				case nameof( Metallic ):
				case nameof( Reflectance ):
					{
						var model = ShadingModel.Value;
						if( model != ShadingModelEnum.Lit && model != ShadingModelEnum.Subsurface && model != ShadingModelEnum.Foliage )
							skip = true;
					}
					break;

				case nameof( Roughness ):
					if( ShadingModel.Value == ShadingModelEnum.Simple || ShadingModel.Value == ShadingModelEnum.Unlit )
						skip = true;
					break;

				case nameof( ClearCoat ):
					if( ShadingModel.Value != ShadingModelEnum.Lit )
						skip = true;
					break;

				case nameof( ClearCoatRoughness ):
				case nameof( ClearCoatNormal ):
					{
						if( ShadingModel.Value != ShadingModelEnum.Lit )
							skip = true;
						else
						{
							var r = ClearCoat;
							if( r.Value == 0.0 && string.IsNullOrEmpty( r.GetByReference ) )
								skip = true;
						}
					}
					break;

				case nameof( Anisotropy ):
					if( ShadingModel.Value != ShadingModelEnum.Lit )
						skip = true;
					break;

				case nameof( AnisotropyDirection ):
				case nameof( AnisotropyDirectionBasis ):
					{
						if( ShadingModel.Value != ShadingModelEnum.Lit )
							skip = true;
						else
						{
							var r = Anisotropy;
							if( r.Value == 0.0 && string.IsNullOrEmpty( r.GetByReference ) )
								skip = true;
						}
					}
					break;

				case nameof( Thickness ):
				case nameof( SubsurfacePower ):
					if( ShadingModel.Value != ShadingModelEnum.Subsurface && ShadingModel.Value != ShadingModelEnum.Foliage )
						skip = true;
					break;

				case nameof( SheenColor ):
					if( ShadingModel.Value != ShadingModelEnum.Cloth )
						skip = true;
					break;

				case nameof( SubsurfaceColor ):
					{
						var m = ShadingModel.Value;
						if( m != ShadingModelEnum.Subsurface && m != ShadingModelEnum.Foliage && m != ShadingModelEnum.Cloth )
							skip = true;
					}
					break;

				//case nameof( RayTracingReflection ):
				//	{
				//		var m = ShadingModel.Value;
				//		if( m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
				//			skip = true;
				//	}
				//	break;

				case nameof( AffectBaseColor ):
				case nameof( AffectMetallic ):
				case nameof( AffectRoughness ):
				case nameof( AffectReflectance ):
				case nameof( AffectAmbientOcclusion ):
				case nameof( AffectEmissive ):
				case nameof( AffectGeometry ):
					if( !AdvancedBlending )
						skip = true;
					break;

				case nameof( DepthOffset ):
					if( DepthOffsetMode.Value == DepthOffsetModeEnum.None )
						skip = true;
					break;

				case nameof( SoftParticles ):
					{
						var blendMode = BlendMode.Value;
						if( blendMode == BlendModeEnum.Opaque || blendMode == BlendModeEnum.Masked )
							skip = true;
					}
					break;

				case nameof( SoftParticlesDistance ):
					{
						var blendMode = BlendMode.Value;
						if( blendMode == BlendModeEnum.Opaque || blendMode == BlendModeEnum.Masked || !SoftParticles )
							skip = true;
					}
					break;

				case nameof( VertexFunctions ):
				case nameof( VertexCode ):
				case nameof( FragmentFunctions ):
				case nameof( FragmentCode ):
				case nameof( CustomParameter1 ):
				case nameof( CustomParameter2 ):
					if( !AdvancedScripting )
						skip = true;
					break;
				}
			}
		}

		protected override void OnResultCompile()
		{
			if( Result != null )
				return;
			Result = Compile( CompiledMaterialData.SpecialMode.Usual, null, 0, null, null, 0 );
		}

		string GetDisplayName()
		{
			//!!!!полный путь получать?
			string displayName;
			if( HierarchyController != null && HierarchyController.CreatedByResource != null )
				displayName = HierarchyController.CreatedByResource.Owner.Name;
			else
				displayName = Name;
			return displayName;
		}

		bool GenerateCode( CompiledMaterialData compiledData, bool needSpecialShadowCaster, Material[] multiMaterialSourceMaterialsToGetProperties )
		//bool GenerateCode( CompiledMaterialData compiledData, bool needSpecialShadowCaster, Material[] multiMaterialSeparateMaterialsOfCombinedGroup )
		{
			var sourceMaterials = new List<Material>();
			if( multiMaterialSourceMaterialsToGetProperties != null )
				sourceMaterials.AddRange( multiMaterialSourceMaterialsToGetProperties );
			else
				sourceMaterials.Add( this );

			//multiMaterialSourceMaterialsToGetProperties 

			//var sourceMaterials = new List<Material>();
			//if( compiledData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup != null )
			//{
			//	foreach( var subMaterial in compiledData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup )
			//		sourceMaterials.Add( subMaterial.owner );
			//}
			//else
			//	sourceMaterials.Add( this );

			////if( multiMaterialSeparateMaterialsOfCombinedGroup != null )
			////{
			////	foreach( var subMaterial in multiMaterialSeparateMaterialsOfCombinedGroup )
			////		sourceMaterials.Add( subMaterial );
			////}
			////else
			////	sourceMaterials.Add( this );

			{
				int textureRegisterCounter = SystemSettings.LimitedDevice ? 11 : 18;// 12;//11;

				//vertex
				{
					var properties = new List<(Component, int, Metadata.Property)>();
					properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( PositionOffset ) )) );

					if( AdvancedScripting )
					{
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.vertexShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "vertex_", this, null, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( VertexFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += VertexFunctions.Value;
						}

						if( !string.IsNullOrEmpty( VertexCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += VertexCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Vertex shader code" );

					compiledData.vertexGeneratedCode = code;
				}

				//material index
				if( compiledData.specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass || compiledData.specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
				{
					var properties = new List<(Component, int, Metadata.Property)>();

					if( AdvancedScripting )
					{
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.materialIndexShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "materialIndex_", this, null, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						//!!!!врядли везде это нужно
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Material Index shader code" );

					compiledData.materialIndexGeneratedCode = code;
				}

				//displacement
				if( Displacement.ReferenceSpecified && RenderingSystem.DisplacementMaxSteps != 0 )
				{
					var properties = new List<(Component, int, Metadata.Property)>();
					properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Displacement ) )) );
					properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( DisplacementScale ) )) );

					if( AdvancedScripting )
					{
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
					}

					//if( compiledData.extensionData != null )
					//	properties.AddRange( compiledData.extensionData.vertexShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "displacement_", this, null, ref textureRegisterCounter/*fragmentTextureRegisterCounter*/, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Displacement shader code" );

					compiledData.displacementGeneratedCode = code;
				}

				//fragment
				{
					var properties = new List<(Component, int, Metadata.Property)>( 32 );

					for( int materialIndex = 0; materialIndex < sourceMaterials.Count; materialIndex++ )
					{
						var material = sourceMaterials[ materialIndex ];

						if( RenderingSystem.NormalMapping ) //!!!!new
						{
							if( RenderingSystem.MaterialShading != ProjectSettingsPage_Rendering.MaterialShadingEnum.Simple )
								properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Normal ) )) );
						}

						properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( BaseColor ) )) );

						//if( ( compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerMasked || compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerTransparent ) && !Opacity.ReferenceSpecified )
						//{
						//	var obj = new ShaderGenerator.PaintLayerOpacityPropertyWithMask();
						//	obj.Init();
						//	properties.Add( (obj, materialIndex, (Metadata.Property)obj.MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );
						//}
						//else
						//	properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );

						//properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( OpacityMaskThreshold ) )) );

						if( RenderingSystem.MaterialShading != ProjectSettingsPage_Rendering.MaterialShadingEnum.Simple )
						{
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Metallic ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Roughness ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Reflectance ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoat ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoatRoughness ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoatNormal ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Anisotropy ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AnisotropyDirection ) )) );
							//properties.Add( (material, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AnisotropyDirectionBasis ) )) );

							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Thickness ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SubsurfacePower ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SheenColor ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SubsurfaceColor ) )) );

							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AmbientOcclusion ) )) );
							//properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( RayTracingReflection ) )) );
						}

						properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Emissive ) )) );

						if( RenderingSystem.MaterialShading != ProjectSettingsPage_Rendering.MaterialShadingEnum.Simple )
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SoftParticlesDistance ) )) );

						if( AdvancedScripting )
						{
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
						}

						if( DepthOffsetMode.Value != DepthOffsetModeEnum.None )
						{
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( DepthOffset ) )) );
						}
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.fragmentShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "fragment_", this, compiledData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Fragment shader code" );

					compiledData.fragmentGeneratedCode = code;
				}

				//opacity
				{
					var properties = new List<(Component, int, Metadata.Property)>();

					for( int materialIndex = 0; materialIndex < sourceMaterials.Count; materialIndex++ )
					{
						var material = sourceMaterials[ materialIndex ];

						if( ( compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerMasked || compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerTransparent ) && !Opacity.ReferenceSpecified )
						{
							var obj = new ShaderGenerator.PaintLayerOpacityPropertyWithMask();
							obj.Init();
							properties.Add( (obj, materialIndex, (Metadata.Property)obj.MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );
						}
						else
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );

						properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( OpacityMaskThreshold ) )) );

						if( AdvancedScripting )
						{
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
						}
					}

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "opacity_", this, compiledData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Opacity shader code" );

					compiledData.opacityGeneratedCode = code;
				}
			}

			//special shadow caster
			if( needSpecialShadowCaster )
			{
				int textureRegisterCounter = SystemSettings.LimitedDevice ? 11 : 18;// 16;// 12;//11;

				//////for depth texture
				////if( SoftParticles )
				////	fragmentTextureRegisterCounter++;

				//vertex
				{
					var properties = new List<(Component, int, Metadata.Property)>();
					properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( PositionOffset ) )) );

					if( AdvancedScripting )
					{
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.shadowCasterVertexShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "vertex_", this, null, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( VertexFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += VertexFunctions.Value;
						}

						if( !string.IsNullOrEmpty( VertexCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += VertexCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Shadow caster vertex shader code" );

					compiledData.shadowCasterVertexGeneratedCode = code;
				}

				//material index
				if( compiledData.specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass || compiledData.specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
				{
					var properties = new List<(Component, int, Metadata.Property)>();

					if( AdvancedScripting )
					{
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
						properties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.shadowCasterMaterialIndexShaderProperties );

					var generator = new ShaderGenerator();
					var code = generator.Process( properties, "materialIndex_", this, null, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						//!!!!врядли везде это нужно
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Shadow caster material Index shader code" );

					compiledData.shadowCasterMaterialIndexGeneratedCode = code;
				}

				//fragment. opacity
				{
					var properties = new List<(Component, int, Metadata.Property)>();

					for( int materialIndex = 0; materialIndex < sourceMaterials.Count; materialIndex++ )
					{
						var material = sourceMaterials[ materialIndex ];

						//if( ( compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerMasked || compiledData.specialMode == CompiledMaterialData.SpecialMode.PaintLayerTransparent ) && !Opacity.ReferenceSpecified )
						//{
						//	var obj = new ShaderGenerator.PaintLayerOpacityPropertyWithMask();
						//	obj.Init();
						//	properties.Add( (obj, materialIndex, (Metadata.Property)obj.MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );
						//}
						//else

						properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );

						properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( OpacityMaskThreshold ) )) );

						if( AdvancedScripting )
						{
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter1 ) )) );
							properties.Add( (material, materialIndex, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( CustomParameter2 ) )) );
						}
					}

					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.shadowCasterFragmentShaderProperties );

					var generator = new ShaderGenerator();

					var code = generator.Process( properties, "fragment_", this, compiledData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup, ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
						return false;

					if( AdvancedScripting )
					{
						if( !string.IsNullOrEmpty( FragmentFunctions.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.shaderScripts ) )
								code.shaderScripts += "\r\n";
							code.shaderScripts += FragmentFunctions.Value;
						}

						if( !string.IsNullOrEmpty( FragmentCode.Value ) )
						{
							if( code == null )
								code = new ShaderGenerator.ResultData();
							if( !string.IsNullOrEmpty( code.codeBody ) )
								code.codeBody += "\r\n";
							code.codeBody += FragmentCode.Value;
						}
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Shadow caster fragment shader" );

					compiledData.shadowCasterFragmentGeneratedCode = code;
				}

			}

			return true;
		}

		/// <summary>
		/// Represents material data that were procedurally generated and compiled.
		/// </summary>
		public class CompileExtensionData
		{
			public List<(Component, int, Metadata.Property)> vertexShaderProperties = new List<(Component, int, Metadata.Property)>();
			public List<(Component, int, Metadata.Property)> materialIndexShaderProperties = new List<(Component, int, Metadata.Property)>();
			//displacement?
			public List<(Component, int, Metadata.Property)> fragmentShaderProperties = new List<(Component, int, Metadata.Property)>();

			public List<(Component, int, Metadata.Property)> shadowCasterVertexShaderProperties = new List<(Component, int, Metadata.Property)>();
			public List<(Component, int, Metadata.Property)> shadowCasterMaterialIndexShaderProperties = new List<(Component, int, Metadata.Property)>();
			public List<(Component, int, Metadata.Property)> shadowCasterFragmentShaderProperties = new List<(Component, int, Metadata.Property)>();
		}

		/////////////////////////////////////////

		//public static Material[] GetAll()
		//{
		//	lock( all )
		//		return all.ToArray();
		//}

		protected virtual string OnCheckDeferredShadingSupport( CompiledMaterialData compiledData )
		{

			//if( compiledData.specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
			//	return "";


			if( !RenderingSystem.DeferredShading )
				return "Global settings";
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
			//	return "Android";
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
			//	return "iOS";
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
			//	return "Web";

			if( compiledData != null )
			{
				if( compiledData.blendMode == BlendModeEnum.Transparent || compiledData.blendMode == BlendModeEnum.Add )
					return "Blend Mode";
			}
			else
			{
				if( BlendMode.Value == BlendModeEnum.Transparent || BlendMode.Value == BlendModeEnum.Add )
					return "Blend Mode";
			}

			switch( ShadingModel.Value )
			{
			case ShadingModelEnum.Subsurface:
				if( Emissive.ReferenceSpecified || Emissive.Value.ToVector3() != Vector3.Zero )
					return "Subsurface + Emissive";
				break;

			case ShadingModelEnum.Foliage:
				if( Emissive.ReferenceSpecified || Emissive.Value.ToVector3() != Vector3.Zero )
					return "Foliage + Emissive";
				break;

			case ShadingModelEnum.Cloth:
			case ShadingModelEnum.Unlit:
				return "Shading Model";
			}
			//if( ShadingModel.Value != ShadingModelEnum.Lit && ShadingModel.Value != ShadingModelEnum.Simple )
			//	return "Shading Model";

			if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
				return "Clear Coat";
			if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
				return "Anisotropy";
			if( !ReceiveShadows )
				return "Receive Shadows";
			return "";
		}

		public delegate void CheckDeferredShadingSupportEventDelegate( Material sender, CompiledMaterialData compiledData, ref string reason );
		public event CheckDeferredShadingSupportEventDelegate CheckDeferredShadingSupportEvent;

		public string PerformCheckDeferredShadingSupport( CompiledMaterialData compiledData = null )
		{
			string reason = OnCheckDeferredShadingSupport( compiledData );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckDeferredShadingSupportEvent?.Invoke( this, compiledData, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		protected virtual string OnCheckReceiveDecalsSupport( CompiledMaterialData compiledData )
		{
			if( !ReceiveDecals )
				return "Receive Decals";
			if( !string.IsNullOrEmpty( PerformCheckDeferredShadingSupport( compiledData ) ) )
				return "Deferred Shading";
			return "";
		}

		public delegate void CheckReceiveDecalsSupportEventDelegate( Material sender, CompiledMaterialData compiledData, ref string reason );
		public event CheckReceiveDecalsSupportEventDelegate CheckReceiveDecalsSupportEvent;

		string PerformCheckReceiveDecalsSupport( CompiledMaterialData compiledData )
		{
			string reason = OnCheckReceiveDecalsSupport( compiledData );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckReceiveDecalsSupportEvent?.Invoke( this, compiledData, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		protected virtual string OnCheckDecalSupport( CompiledMaterialData compiledData )
		{
			//if( !ReceiveDecals )
			//	return "Receive Decals";
			if( !string.IsNullOrEmpty( PerformCheckDeferredShadingSupport( compiledData ) ) )
				return "Deferred Shading";
			return "";
		}

		public delegate void CheckDecalSupportEventDelegate( Material sender, CompiledMaterialData compiledData, ref string reason );
		public event CheckDecalSupportEventDelegate CheckDecalSupportEvent;

		string PerformCheckDecalSupport( CompiledMaterialData compiledData )
		{
			string reason = OnCheckDecalSupport( compiledData );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckDecalSupportEvent?.Invoke( this, compiledData, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		protected virtual string OnCheckGISupport( CompiledMaterialData compiledData )
		{
			if( !RenderingSystem.GlobalIllumination )
				return "Global settings";

			return "";
		}

		public delegate void CheckGISupportEventDelegate( Material sender, CompiledMaterialData compiledData, ref string reason );
		public event CheckGISupportEventDelegate CheckGISupportEvent;

		public string PerformCheckGISupport( CompiledMaterialData compiledData = null )
		{
			string reason = OnCheckGISupport( compiledData );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckGISupportEvent?.Invoke( this, compiledData, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		public virtual CompiledMaterialData Compile( CompiledMaterialData.SpecialMode specialMode, CompileExtensionData extensionData, int multiMaterialStartIndexOfCombinedGroup, CompiledMaterialData[] multiMaterialReferencedSeparateMaterialsOfCombinedGroup, Material[] multiMaterialSourceMaterialsToGetProperties, int multiSubMaterialSeparatePassIndex )
		{
			var optimize = true;
#if !DEPLOY
			if( EngineApp.IsEditor )
			{
				var document = EditorAPI.GetDocumentByObject( this );
				if( document != null && document.Modified )
					optimize = false;
			}
#endif

			//var time = DateTime.Now;

			var result = new CompiledMaterialData();
			result.owner = this;
			result.specialMode = specialMode;
			result.extensionData = extensionData;
			result.multiMaterialStartIndexOfCombinedGroup = multiMaterialStartIndexOfCombinedGroup;
			result.multiMaterialReferencedSeparateMaterialsOfCombinedGroup = multiMaterialReferencedSeparateMaterialsOfCombinedGroup;
			result.multiSubMaterialSeparatePassIndex = multiSubMaterialSeparatePassIndex;

			var blendMode = BlendMode.Value;
			var opacityDithering = OpacityDithering.Value;

			if( BlendMode.Value == BlendModeEnum.Opaque )
			{
				if( specialMode == CompiledMaterialData.SpecialMode.PaintLayerMasked )
				{
					blendMode = BlendModeEnum.Masked;
					opacityDithering = true;
				}
				else if( specialMode == CompiledMaterialData.SpecialMode.PaintLayerTransparent )
					blendMode = BlendModeEnum.Transparent;
			}

			result.blendMode = blendMode;

			result.Transparent = blendMode == BlendModeEnum.Transparent || blendMode == BlendModeEnum.Add;
			result.ShadingModel = ShadingModel.Value;

			//deferred shading
			result.deferredShadingSupportReason = PerformCheckDeferredShadingSupport( result );
			result.deferredShadingSupport = string.IsNullOrEmpty( result.deferredShadingSupportReason );

			//receive decals
			result.receiveDecalsSupportReason = PerformCheckReceiveDecalsSupport( result );
			result.receiveDecalsSupport = string.IsNullOrEmpty( result.receiveDecalsSupportReason );

			//decal
			result.decalSupportReason = PerformCheckDecalSupport( result );
			result.decalSupport = string.IsNullOrEmpty( result.decalSupportReason );

			//gi
			result.giSupportReason = PerformCheckGISupport( result );
			result.giSupport = string.IsNullOrEmpty( result.giSupportReason );

			result.staticShadows = StaticShadows;

			if( DisplacementTechnique.Value == DisplacementMethodEnum.Tessellation )
				result.tessellationQuality = (float)TessellationQuality.Value;
			//result.tessellation = DisplacementTechnique.Value == DisplacementMethodEnum.Tessellation;

			////soft particles
			//result.softParticles = SoftParticles;

			//!!!!what else?
			bool needSpecialShadowCaster = PositionOffset.ReferenceSpecified || blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */|| blendMode == BlendModeEnum.Transparent || AdvancedScripting || ( extensionData != null && ( extensionData.shadowCasterVertexShaderProperties.Count != 0 || extensionData.shadowCasterMaterialIndexShaderProperties.Count != 0 || extensionData.shadowCasterFragmentShaderProperties.Count != 0 ) ) || specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass || specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass || DepthOffsetMode.Value != DepthOffsetModeEnum.None;

			//shader generation
			if( shaderGenerationCompile )
			{
				if( !GenerateCode( result, needSpecialShadowCaster, multiMaterialSourceMaterialsToGetProperties ) )
					return null;
			}


			//doing two iterations. first to get list of gpu programs to compile. second to get final result
			for( int nCompileIteration = 0; nCompileIteration < 2; nCompileIteration++ )
			{
				var collecting = nCompileIteration == 0;
				var programsToCompile = new List<GpuProgramManager.GetProgramItem>();


				//forward passes
				if( specialMode != CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
				{
					bool unlit = ShadingModel.Value == ShadingModelEnum.Unlit;
					var receiveShadows = ReceiveShadows.Value && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None;

					var passIterations = 2;// 1;
					if( SystemSettings.LimitedDevice )
						passIterations += 2;

					for( int nPassIteration = 0; nPassIteration < passIterations; nPassIteration++ )
					{
						var voxelPass = nPassIteration == 1;

						if( voxelPass && !RenderingSystem.VoxelLOD )
							continue;
						if( nPassIteration == 2 && RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None )
							continue;

						////one or two iterations depending Ambient light source, ReceiveShadows
						//int shadowsSupportIterations = 1;
						//if( lightType != Light.TypeEnum.Ambient && ReceiveShadows.Value && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None )
						//	shadowsSupportIterations = 2;// 3;// 2;
						//for( int nShadowsSupportCounter = 0; nShadowsSupportCounter < shadowsSupportIterations; nShadowsSupportCounter++ )
						//{

						//generate compile arguments
						var vertexDefines = new List<(string, string)>( 8 );
						var fragmentDefines = new List<(string, string)>( 8 );
						{
							var generalDefines = new List<(string, string)>( 16 );
							//generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
							generalDefines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
							fragmentDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
							fragmentDefines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
							if( TwoSided && TwoSidedFlipNormals )
								fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
							if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
								fragmentDefines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
							if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
								fragmentDefines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );
							if( voxelPass )
								generalDefines.Add( ("VOXEL", "") );

							if( ShadingModel.Value == ShadingModelEnum.Lit )
							{
								if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
									fragmentDefines.Add( ("MATERIAL_HAS_CLEAR_COAT", "") );
								if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
									fragmentDefines.Add( ("MATERIAL_HAS_ANISOTROPY", "") );
							}

							if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
								generalDefines.Add( ("DISPLACEMENT", "") );
							if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
								fragmentDefines.Add( ("OPACITY_DITHERING", "") );
							if( SoftParticles )
								fragmentDefines.Add( ("SOFT_PARTICLES", "") );

							//receive shadows support
							if( receiveShadows && nPassIteration != 3 )//nShadowsSupportCounter != 0 )
							{
								fragmentDefines.Add( ("SHADOW_MAP", "") );

								//if( nShadowsSupportCounter == 2 )
								//	fragmentDefines.Add( ("SHADOW_MAP_HIGH", "") );
								//else
								//	fragmentDefines.Add( ("SHADOW_MAP_LOW", "") );
							}

							if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterOrEqual )
								fragmentDefines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
							else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessOrEqual )
								fragmentDefines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

							if( nPassIteration == 2 || nPassIteration == 3 )
								fragmentDefines.Add( ("LIGHT_DIRECTIONAL_AMBIENT_ONLY", "") );

							vertexDefines.AddRange( generalDefines );
							fragmentDefines.AddRange( generalDefines );

							if( shaderGenerationEnable )
							{
								//vertex
								var vertexCode = result.vertexGeneratedCode;
								if( vertexCode != null )
								{
									if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
									if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
									if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
										vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
										vertexDefines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
								}

								//material index
								var materialIndexCode = result.materialIndexGeneratedCode;
								if( materialIndexCode != null )
								{
									if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
									if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
									if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
								}

								//displacement
								var displacementCode = result.displacementGeneratedCode;
								if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
								{
									if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
									if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
									if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
									if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
								}

								//fragment
								var fragmentCode = result.fragmentGeneratedCode;
								if( fragmentCode != null )
								{
									if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
									if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
								}

								//opacity
								var opacityCode = result.opacityGeneratedCode;
								if( opacityCode != null )
								{
									if( !string.IsNullOrEmpty( opacityCode.parametersBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_PARAMETERS", opacityCode.parametersBody) );
									if( !string.IsNullOrEmpty( opacityCode.samplersBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_SAMPLERS", opacityCode.samplersBody) );
									if( !string.IsNullOrEmpty( opacityCode.shaderScripts ) )
										fragmentDefines.Add( ("OPACITY_CODE_SHADER_SCRIPTS", "\r\n" + opacityCode.shaderScripts) );
									if( !string.IsNullOrEmpty( opacityCode.codeBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_BODY", "\r\n" + opacityCode.codeBody) );
								}
							}
						}

						{
							var vertexParameters = new GpuProgramManager.GetProgramItem( "Standard_Forward_Vertex_", GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Forward_vs.sc", vertexDefines, optimize );

							var fragmentParameters = new GpuProgramManager.GetProgramItem( "Standard_Forward_Fragment_", GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Forward_fs.sc", fragmentDefines, optimize );

							if( collecting )
							{
								programsToCompile.Add( vertexParameters );
								programsToCompile.Add( fragmentParameters );
							}
							else
							{
								string error2;

								//vertex program
								GpuProgram vertexProgram = GpuProgramManager.GetProgram( vertexParameters, out error2 );
								if( !string.IsNullOrEmpty( error2 ) )
								{
									result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
									Log.Warning( result.error );
									//result.Dispose();
									return null;
								}

								//fragment program
								GpuProgram fragmentProgram = GpuProgramManager.GetProgram( fragmentParameters, out error2 );
								if( !string.IsNullOrEmpty( error2 ) )
								{
									result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
									Log.Warning( result.error );
									//result.Dispose();
									return null;
								}

								var pass = new GpuMaterialPass( result, vertexProgram, fragmentProgram );
								result.AllPasses.Add( pass );

								switch( nPassIteration )
								{
								case 0: result.forwardShadingPassUsual = pass; break;
								case 1: result.forwardShadingPassVoxel = pass; break;
								case 2: result.forwardShadingPassDirectionalAmbientOnly = pass; break;
								case 3: result.forwardShadingPassDirectionalAmbientOnlyNoShadows = pass; break;
								}

								//if( nPassIteration == 2 )
								//	result.forwardShadingPassDirectionalAmbientOnlyNoShadows = pass;
								//else if( nPassIteration == 1 )
								//	result.forwardShadingPassDirectionalAmbientOnly = pass;
								//else
								//	result.forwardShadingPass = pass;

								////if( nShadowsSupportCounter == 1 )
								////	group.passWithShadows = pass;
								////else
								////	group.passWithoutShadows = pass;

								////if( nShadowsSupportCounter == 2 )
								////	group.passWithShadowsHigh = pass;
								////else if( nShadowsSupportCounter == 1 )
								////	group.passWithShadowsLow = pass;
								////else
								////	group.passWithoutShadows = pass;

								if( blendMode == BlendModeEnum.Opaque || blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer*/ )
								{
									//if( lightType == Light.TypeEnum.Ambient )
									//{
									pass.DepthWrite = true;
									pass.SourceBlendFactor = SceneBlendFactor.One;
									pass.DestinationBlendFactor = SceneBlendFactor.Zero;
									//}
									//else
									//{
									//	pass.DepthWrite = false;
									//	pass.SourceBlendFactor = SceneBlendFactor.One;
									//	pass.DestinationBlendFactor = SceneBlendFactor.One;
									//}
									////if( lightType != Light.TypeEnum.Ambient || usageMode == CompiledDataStandard.UsageMode.MaterialBlendNotFirst )
									////{
									////	pass.DepthWrite = false;
									////	pass.SourceBlendFactor = SceneBlendFactor.One;
									////	pass.DestinationBlendFactor = SceneBlendFactor.One;
									////}
									////else
									////{
									////	pass.DepthWrite = true;
									////	pass.SourceBlendFactor = SceneBlendFactor.One;
									////	pass.DestinationBlendFactor = SceneBlendFactor.Zero;
									////}
								}
								else if( blendMode == BlendModeEnum.Transparent )
								{
									//!!!!OIT?

									//if( lightType == Light.TypeEnum.Ambient )
									//{
									pass.DepthWrite = false;
									pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
									pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
									//}
									//else
									//{
									//	pass.DepthWrite = false;
									//	pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
									//	pass.DestinationBlendFactor = SceneBlendFactor.One;
									//}
								}
								else if( blendMode == BlendModeEnum.Add )
								{
									pass.DepthWrite = false;
									pass.SourceBlendFactor = SceneBlendFactor.One;
									pass.DestinationBlendFactor = SceneBlendFactor.One;
								}

								if( TwoSided )
									pass.CullingMode = CullingMode.None;
							}
						}

						//}

					}


					//result.passesByLightType = new CompiledMaterialData.PassGroup[ unlit ? 1 : 4 ];

					//foreach( Light.TypeEnum lightType in Enum.GetValues( typeof( Light.TypeEnum ) ) )
					//{
					//	if( unlit && lightType != Light.TypeEnum.Ambient )
					//		break;

					//	var group = new CompiledMaterialData.PassGroup();
					//	result.passesByLightType[ (int)lightType ] = group;

					//	//one or two iterations depending Ambient light source, ReceiveShadows
					//	int shadowsSupportIterations = 1;
					//	if( lightType != Light.TypeEnum.Ambient && ReceiveShadows.Value && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None )
					//		shadowsSupportIterations = 2;// 3;// 2;
					//	for( int nShadowsSupportCounter = 0; nShadowsSupportCounter < shadowsSupportIterations; nShadowsSupportCounter++ )
					//	{
					//		//generate compile arguments
					//		var vertexDefines = new List<(string, string)>( 8 );
					//		var fragmentDefines = new List<(string, string)>( 8 );
					//		{
					//			var generalDefines = new List<(string, string)>( 16 );
					//			generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
					//			generalDefines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
					//			fragmentDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
					//			fragmentDefines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
					//			if( TwoSided && TwoSidedFlipNormals )
					//				fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
					//			if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
					//				fragmentDefines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
					//			if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
					//				fragmentDefines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );

					//			if( ShadingModel.Value == ShadingModelEnum.Lit )
					//			{
					//				if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
					//					fragmentDefines.Add( ("MATERIAL_HAS_CLEAR_COAT", "") );
					//				if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
					//					fragmentDefines.Add( ("MATERIAL_HAS_ANISOTROPY", "") );
					//			}

					//			if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
					//				generalDefines.Add( ("DISPLACEMENT", "") );
					//			if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
					//				fragmentDefines.Add( ("OPACITY_DITHERING", "") );
					//			if( SoftParticles )
					//				fragmentDefines.Add( ("SOFT_PARTICLES", "") );

					//			//receive shadows support
					//			if( nShadowsSupportCounter != 0 )
					//			{
					//				fragmentDefines.Add( ("SHADOW_MAP", "") );

					//				//if( nShadowsSupportCounter == 2 )
					//				//	fragmentDefines.Add( ("SHADOW_MAP_HIGH", "") );
					//				//else
					//				//	fragmentDefines.Add( ("SHADOW_MAP_LOW", "") );
					//			}

					//			if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterOrEqual )
					//				fragmentDefines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
					//			else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessOrEqual )
					//				fragmentDefines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

					//			vertexDefines.AddRange( generalDefines );
					//			fragmentDefines.AddRange( generalDefines );

					//			if( shaderGenerationEnable )
					//			{
					//				//vertex
					//				var vertexCode = result.vertexGeneratedCode;
					//				if( vertexCode != null )
					//				{
					//					if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
					//						vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
					//					if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
					//						vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
					//					if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
					//						vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
					//					if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
					//						vertexDefines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
					//				}

					//				//material index
					//				var materialIndexCode = result.materialIndexGeneratedCode;
					//				if( materialIndexCode != null )
					//				{
					//					if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
					//						fragmentDefines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
					//					if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
					//						fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
					//					if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
					//						fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
					//					if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
					//						fragmentDefines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
					//				}

					//				//displacement
					//				var displacementCode = result.displacementGeneratedCode;
					//				if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
					//				{
					//					if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
					//						fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
					//					if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
					//						fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
					//					if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
					//						fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
					//					if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
					//						fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
					//				}

					//				//fragment
					//				var fragmentCode = result.fragmentGeneratedCode;
					//				if( fragmentCode != null )
					//				{
					//					if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
					//						fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
					//					if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
					//						fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
					//					if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
					//						fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
					//					if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
					//						fragmentDefines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
					//				}
					//			}
					//		}

					//		{
					//			var vertexParameters = new GpuProgramManager.GetProgramItem( "Standard_Forward_Vertex_", GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Forward_vs.sc", vertexDefines, optimize );

					//			var fragmentParameters = new GpuProgramManager.GetProgramItem( "Standard_Forward_Fragment_", GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Forward_fs.sc", fragmentDefines, optimize );

					//			if( collecting )
					//			{
					//				programsToCompile.Add( vertexParameters );
					//				programsToCompile.Add( fragmentParameters );
					//			}
					//			else
					//			{
					//				string error2;

					//				//vertex program
					//				GpuProgram vertexProgram = GpuProgramManager.GetProgram( vertexParameters, out error2 );
					//				if( !string.IsNullOrEmpty( error2 ) )
					//				{
					//					result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
					//					Log.Warning( result.error );
					//					//result.Dispose();
					//					return null;
					//				}

					//				//fragment program
					//				GpuProgram fragmentProgram = GpuProgramManager.GetProgram( fragmentParameters, out error2 );
					//				if( !string.IsNullOrEmpty( error2 ) )
					//				{
					//					result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
					//					Log.Warning( result.error );
					//					//result.Dispose();
					//					return null;
					//				}

					//				var pass = new GpuMaterialPass( result, vertexProgram, fragmentProgram );
					//				result.AllPasses.Add( pass );

					//				if( nShadowsSupportCounter == 1 )
					//					group.passWithShadows = pass;
					//				else
					//					group.passWithoutShadows = pass;

					//				//if( nShadowsSupportCounter == 2 )
					//				//	group.passWithShadowsHigh = pass;
					//				//else if( nShadowsSupportCounter == 1 )
					//				//	group.passWithShadowsLow = pass;
					//				//else
					//				//	group.passWithoutShadows = pass;

					//				if( blendMode == BlendModeEnum.Opaque || blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer*/ )
					//				{
					//					if( lightType == Light.TypeEnum.Ambient )
					//					{
					//						pass.DepthWrite = true;
					//						pass.SourceBlendFactor = SceneBlendFactor.One;
					//						pass.DestinationBlendFactor = SceneBlendFactor.Zero;
					//					}
					//					else
					//					{
					//						pass.DepthWrite = false;
					//						pass.SourceBlendFactor = SceneBlendFactor.One;
					//						pass.DestinationBlendFactor = SceneBlendFactor.One;
					//					}
					//					//if( lightType != Light.TypeEnum.Ambient || usageMode == CompiledDataStandard.UsageMode.MaterialBlendNotFirst )
					//					//{
					//					//	pass.DepthWrite = false;
					//					//	pass.SourceBlendFactor = SceneBlendFactor.One;
					//					//	pass.DestinationBlendFactor = SceneBlendFactor.One;
					//					//}
					//					//else
					//					//{
					//					//	pass.DepthWrite = true;
					//					//	pass.SourceBlendFactor = SceneBlendFactor.One;
					//					//	pass.DestinationBlendFactor = SceneBlendFactor.Zero;
					//					//}
					//				}
					//				else if( blendMode == BlendModeEnum.Transparent )
					//				{
					//					if( lightType == Light.TypeEnum.Ambient )
					//					{
					//						pass.DepthWrite = false;
					//						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
					//						pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
					//					}
					//					else
					//					{
					//						pass.DepthWrite = false;
					//						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
					//						pass.DestinationBlendFactor = SceneBlendFactor.One;
					//					}
					//				}
					//				else if( blendMode == BlendModeEnum.Add )
					//				{
					//					pass.DepthWrite = false;
					//					pass.SourceBlendFactor = SceneBlendFactor.One;
					//					pass.DestinationBlendFactor = SceneBlendFactor.One;
					//				}

					//				if( TwoSided )
					//					pass.CullingMode = CullingMode.None;
					//			}
					//		}
					//	}
					//}
				}


				//special shadow caster material
				if( needSpecialShadowCaster )
				{
					result.specialShadowCasterData = new RenderingPipeline.ShadowCasterData();
					result.specialShadowCasterData.passByLightType = new GpuMaterialPassGroup[ 4 ];

					foreach( Light.TypeEnum lightType in Enum.GetValues( typeof( Light.TypeEnum ) ) )
					{
						if( lightType == Light.TypeEnum.Ambient )
							continue;

						for( int nPassType = 0; nPassType < 3; nPassType++ )//for( int nPassType = 0; nPassType < 4; nPassType++ )
						{
							var voxelPass = nPassType == 1;
							//var virtualizedPass = nPassType == 2;
							var billboardPass = nPassType == 2;// 3;

							//generate compile arguments
							var vertexDefines = new List<(string, string)>( 8 );
							var fragmentDefines = new List<(string, string)>( 8 );
							{
								var generalDefines = new List<(string, string)>();
								generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
								generalDefines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
								if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
									fragmentDefines.Add( ("OPACITY_DITHERING", "") );
								if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
									fragmentDefines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
								if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
									fragmentDefines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );
								//if( SoftParticles )
								//	fragmentDefines.Add( ("SOFT_PARTICLES", "") );
								if( voxelPass )
									generalDefines.Add( ("VOXEL", "") );
								//if( virtualizedPass )
								//	generalDefines.Add( ("VIRTUALIZED", "") );
								if( billboardPass )
									generalDefines.Add( ("BILLBOARD", "") );

								if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterOrEqual )
									fragmentDefines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
								else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessOrEqual )
									fragmentDefines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

								vertexDefines.AddRange( generalDefines );
								fragmentDefines.AddRange( generalDefines );

								if( shaderGenerationEnable )
								{
									//vertex
									var vertexCode = result.vertexGeneratedCode;//result.vertexGeneratedCodeShadowCaster;
									if( vertexCode != null )
									{
										if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
											vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
										if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
											vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
										if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
											vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
										if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
											vertexDefines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
									}

									//material index
									var materialIndexCode = result.materialIndexGeneratedCode;
									if( materialIndexCode != null )
									{
										if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
											fragmentDefines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
										if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
											fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
										if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
											fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
										if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
											fragmentDefines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
									}

									//fragment
									var fragmentCode = result.shadowCasterFragmentGeneratedCode;
									if( fragmentCode != null )
									{
										if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
											fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
										if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
											fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
										if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
											fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
										if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
											fragmentDefines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
									}

									//opacity
									var opacityCode = result.opacityGeneratedCode;
									if( opacityCode != null )
									{
										if( !string.IsNullOrEmpty( opacityCode.parametersBody ) )
											fragmentDefines.Add( ("OPACITY_CODE_PARAMETERS", opacityCode.parametersBody) );
										if( !string.IsNullOrEmpty( opacityCode.samplersBody ) )
											fragmentDefines.Add( ("OPACITY_CODE_SAMPLERS", opacityCode.samplersBody) );
										if( !string.IsNullOrEmpty( opacityCode.shaderScripts ) )
											fragmentDefines.Add( ("OPACITY_CODE_SHADER_SCRIPTS", "\r\n" + opacityCode.shaderScripts) );
										if( !string.IsNullOrEmpty( opacityCode.codeBody ) )
											fragmentDefines.Add( ("OPACITY_CODE_BODY", "\r\n" + opacityCode.codeBody) );
									}

								}
							}

							{
								var vertexParameters = new GpuProgramManager.GetProgramItem( "Standard_ShadowCaster_Vertex_", GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_ShadowCaster_vs.sc", vertexDefines, optimize );

								var fragmentParameters = new GpuProgramManager.GetProgramItem( "Standard_ShadowCaster_Fragment_", GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_ShadowCaster_fs.sc", fragmentDefines, optimize );

								if( collecting )
								{
									programsToCompile.Add( vertexParameters );
									programsToCompile.Add( fragmentParameters );
								}
								else
								{
									string error2;

									//vertex program
									GpuProgram vertexProgram = GpuProgramManager.GetProgram( vertexParameters, out error2 );
									if( !string.IsNullOrEmpty( error2 ) )
									{
										result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
										Log.Warning( result.error );
										//result.Dispose();
										return null;
									}

									//fragment program
									GpuProgram fragmentProgram = GpuProgramManager.GetProgram( fragmentParameters, out error2 );
									if( !string.IsNullOrEmpty( error2 ) )
									{
										result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
										Log.Warning( result.error );
										//result.Dispose();
										return null;
									}

									var pass = new GpuMaterialPass( result, vertexProgram, fragmentProgram );
									result.specialShadowCasterData.passByLightType[ (int)lightType ].Set( pass, voxelPass, /*virtualizedPass,*/ billboardPass );

									if( TwoSided )
										pass.CullingMode = CullingMode.None;
								}
							}
						}
					}
				}

				//deferred shading pass
				if( result.deferredShadingSupport )
				{
					for( int nPassType = 0; nPassType < 3; nPassType++ )//for( int nPassType = 0; nPassType < 4; nPassType++ )
					{
						var voxelPass = nPassType == 1;
						//var virtualizedPass = nPassType == 2;
						var billboardPass = nPassType == 2;// 3;

						//generate compile arguments
						var vertexDefines = new List<(string, string)>( 8 );
						var fragmentDefines = new List<(string, string)>( 8 );
						{
							var generalDefines = new List<(string, string)>();
							generalDefines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
							fragmentDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
							fragmentDefines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
							if( TwoSided && TwoSidedFlipNormals )
								fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
							if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
								fragmentDefines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
							if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
								fragmentDefines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );

							if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
								generalDefines.Add( ("DISPLACEMENT", "") );
							if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
								fragmentDefines.Add( ("OPACITY_DITHERING", "") );
							//if( SoftParticles )
							//	generalDefines.Add( ("SOFT_PARTICLES", "") );
							if( voxelPass )
								generalDefines.Add( ("VOXEL", "") );
							//if( virtualizedPass )
							//	generalDefines.Add( ("VIRTUALIZED", "") );
							if( billboardPass )
								generalDefines.Add( ("BILLBOARD", "") );

							if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterOrEqual )
								fragmentDefines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
							else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessOrEqual )
								fragmentDefines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

							////receive shadows support
							//if( nShadowsSupportCounter != 0 )
							//{
							//	generalDefines.Add( ( "SHADOW_MAP", "" ) );
							//}


							vertexDefines.AddRange( generalDefines );
							fragmentDefines.AddRange( generalDefines );

							if( shaderGenerationEnable )
							{
								//vertex
								var vertexCode = result.vertexGeneratedCode;
								if( vertexCode != null )
								{
									if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
									if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
									if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
										vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
										vertexDefines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
								}

								//material index
								var materialIndexCode = result.materialIndexGeneratedCode;
								if( materialIndexCode != null )
								{
									if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
									if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
									if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
										fragmentDefines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
								}

								//displacement
								var displacementCode = result.displacementGeneratedCode;
								if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
								{
									if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
									if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
									if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
									if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
								}

								//fragment
								var fragmentCode = result.fragmentGeneratedCode;
								if( fragmentCode != null )
								{
									if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
									if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
								}

								//opacity
								var opacityCode = result.opacityGeneratedCode;
								if( opacityCode != null )
								{
									if( !string.IsNullOrEmpty( opacityCode.parametersBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_PARAMETERS", opacityCode.parametersBody) );
									if( !string.IsNullOrEmpty( opacityCode.samplersBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_SAMPLERS", opacityCode.samplersBody) );
									if( !string.IsNullOrEmpty( opacityCode.shaderScripts ) )
										fragmentDefines.Add( ("OPACITY_CODE_SHADER_SCRIPTS", "\r\n" + opacityCode.shaderScripts) );
									if( !string.IsNullOrEmpty( opacityCode.codeBody ) )
										fragmentDefines.Add( ("OPACITY_CODE_BODY", "\r\n" + opacityCode.codeBody) );
								}
							}
						}

						{
							var vertexParameters = new GpuProgramManager.GetProgramItem( "Standard_Deferred_Vertex_", GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Deferred_vs.sc", vertexDefines, optimize );

							var fragmentParameters = new GpuProgramManager.GetProgramItem( "Standard_Deferred_Fragment_", GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Deferred_fs.sc", fragmentDefines, optimize );

							if( collecting )
							{
								programsToCompile.Add( vertexParameters );
								programsToCompile.Add( fragmentParameters );
							}
							else
							{
								string error2;

								//vertex program
								GpuProgram vertexProgram = GpuProgramManager.GetProgram( vertexParameters, out error2 );
								if( !string.IsNullOrEmpty( error2 ) )
								{
									result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
									Log.Warning( result.error );
									//result.Dispose();
									return null;
								}

								//fragment program
								GpuProgram fragmentProgram = GpuProgramManager.GetProgram( fragmentParameters, out error2 );
								if( !string.IsNullOrEmpty( error2 ) )
								{
									result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
									Log.Warning( result.error );
									//result.Dispose();
									return null;
								}

								var pass = new GpuMaterialPass( result, vertexProgram, fragmentProgram );
								result.AllPasses.Add( pass );
								result.deferredShadingPass.Set( pass, voxelPass/*, virtualizedPass*/, billboardPass );

								if( TwoSided )
									pass.CullingMode = CullingMode.None;
							}
						}
					}
				}

				//decal shading pass
				if( result.decalSupport )
				{
					//generate compile arguments
					var vertexDefines = new List<(string, string)>( 8 );
					var fragmentDefines = new List<(string, string)>( 8 );
					{
						var generalDefines = new List<(string, string)>();
						generalDefines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
						fragmentDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
						fragmentDefines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
						if( TwoSided && TwoSidedFlipNormals )
							fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
						if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
							fragmentDefines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
						if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
							fragmentDefines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );

						if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
							generalDefines.Add( ("DISPLACEMENT", "") );
						if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
							fragmentDefines.Add( ("OPACITY_DITHERING", "") );
						//if( SoftParticles )
						//	generalDefines.Add( ("SOFT_PARTICLES", "") );

						if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterOrEqual )
							fragmentDefines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
						else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessOrEqual )
							fragmentDefines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

						//!!!!clusters, maybe voxels

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );

						if( shaderGenerationEnable )
						{
							//vertex
							var vertexCode = result.vertexGeneratedCode;
							if( vertexCode != null )
							{
								if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
								if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
								if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
									vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
								if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
									vertexDefines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
							}

							//material index
							var materialIndexCode = result.materialIndexGeneratedCode;
							if( materialIndexCode != null )
							{
								if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
									fragmentDefines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
								if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
									fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
								if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
									fragmentDefines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
								if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
									fragmentDefines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
							}

							//displacement
							var displacementCode = result.displacementGeneratedCode;
							if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
							{
								if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
								if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
								if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
								if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
							}

							//fragment
							var fragmentCode = result.fragmentGeneratedCode;
							if( fragmentCode != null )
							{
								if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
								if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
							}

							//opacity
							var opacityCode = result.opacityGeneratedCode;
							if( opacityCode != null )
							{
								if( !string.IsNullOrEmpty( opacityCode.parametersBody ) )
									fragmentDefines.Add( ("OPACITY_CODE_PARAMETERS", opacityCode.parametersBody) );
								if( !string.IsNullOrEmpty( opacityCode.samplersBody ) )
									fragmentDefines.Add( ("OPACITY_CODE_SAMPLERS", opacityCode.samplersBody) );
								if( !string.IsNullOrEmpty( opacityCode.shaderScripts ) )
									fragmentDefines.Add( ("OPACITY_CODE_SHADER_SCRIPTS", "\r\n" + opacityCode.shaderScripts) );
								if( !string.IsNullOrEmpty( opacityCode.codeBody ) )
									fragmentDefines.Add( ("OPACITY_CODE_BODY", "\r\n" + opacityCode.codeBody) );
							}
						}
					}

					{
						var vertexParameters = new GpuProgramManager.GetProgramItem( "Standard_DeferredDecal_Vertex_", GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_DeferredDecal_vs.sc", vertexDefines, optimize );

						var fragmentParameters = new GpuProgramManager.GetProgramItem( "Standard_DeferredDecal_Fragment_", GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_DeferredDecal_fs.sc", fragmentDefines, optimize );

						if( collecting )
						{
							programsToCompile.Add( vertexParameters );
							programsToCompile.Add( fragmentParameters );
						}
						else
						{
							string error2;

							//vertex program
							GpuProgram vertexProgram = GpuProgramManager.GetProgram( vertexParameters, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								//result.Dispose();
								return null;
							}

							//fragment program
							GpuProgram fragmentProgram = GpuProgramManager.GetProgram( fragmentParameters, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								//result.Dispose();
								return null;
							}

							var pass = new GpuMaterialPass( result, vertexProgram, fragmentProgram );
							result.AllPasses.Add( pass );
							result.decalShadingPass = pass;

							pass.DepthCheck = false;
							pass.DepthWrite = false;
							pass.CullingMode = CullingMode.Anticlockwise;

							//temp
							//pass.SettempAlphaToCoverage( blendMode == BlendModeEnum.Masked );

							if( AdvancedBlending )
							{
								//!!!!возможность менять если кто-то решит поменять GBuffer?

								uint mask = 0;

								//0
								if( AffectBaseColor.Value != 0 )
								{
									mask |= 1 << ( 0 * 4 + 0 );
									mask |= 1 << ( 0 * 4 + 1 );
									mask |= 1 << ( 0 * 4 + 2 );
									mask |= 1 << ( 0 * 4 + 3 );
								}

								//1
								if( AffectGeometry.Value != 0 )
								{
									mask |= 1 << ( 1 * 4 + 0 );
									mask |= 1 << ( 1 * 4 + 1 );
									mask |= 1 << ( 1 * 4 + 2 );
								}
								if( AffectReflectance.Value != 0 )
									mask |= 1 << ( 1 * 4 + 3 );

								//2
								if( AffectMetallic.Value != 0 )
									mask |= 1 << ( 2 * 4 + 0 );
								if( AffectRoughness.Value != 0 )
									mask |= 1 << ( 2 * 4 + 1 );
								if( AffectAmbientOcclusion.Value != 0 )
									mask |= 1 << ( 2 * 4 + 2 );
								//!!!!rayTracingReflection
								if( AffectMetallic.Value != 0 )
									mask |= 1 << ( 2 * 4 + 3 );

								//3
								if( AffectEmissive.Value != 0 )
								{
									mask |= 1 << ( 3 * 4 + 0 );
									mask |= 1 << ( 3 * 4 + 1 );
									mask |= 1 << ( 3 * 4 + 2 );
									mask |= 1 << ( 3 * 4 + 3 );
								}

								//4
								if( AffectGeometry.Value != 0 )
								{
									mask |= 1 << ( 4 * 4 + 0 );
									mask |= 1 << ( 4 * 4 + 1 );
									mask |= 1 << ( 4 * 4 + 2 );
									mask |= 1 << ( 4 * 4 + 3 );
								}

								//gl_FragData[ 0 ] = encodeRGBE8( baseColor );
								//gl_FragData[ 1 ] = vec4( normal * 0.5 + 0.5, reflectance );
								//gl_FragData[ 2 ] = vec4( metallic, roughness, ambientOcclusion, rayTracingReflection );
								//gl_FragData[ 3 ] = encodeRGBE8( emissive );
								//gl_FragData[ 4 ] = encodeGBuffer4( tangent, shadingModelSimple, false/*receiveDecals*/);

								pass.AdvancedBlendingWriteMask = mask;
							}
						}
					}
				}

				//gi
				if( result.giSupport )
				{

					bool unlit = ShadingModel.Value == ShadingModelEnum.Unlit;

					var receiveShadows = ReceiveShadows.Value && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None;


					//!!!!
					var nPassType = 1;
					//for( int nPassType = 0; nPassType < 3; nPassType++ )//for( int nPassType = 0; nPassType < 4; nPassType++ )
					//{
					var voxelPass = nPassType == 1;
					var billboardPass = nPassType == 2;

					//generate compile arguments
					var defines = new List<(string, string)>( 8 );
					{
						defines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
						defines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
						defines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
						//!!!!
						//if( TwoSided && TwoSidedFlipNormals )
						//	fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
						if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
							defines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
						if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
							defines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );

						//!!!!
						//if( ShadingModel.Value == ShadingModelEnum.Lit )
						//{
						//	if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
						//		fragmentDefines.Add( ("MATERIAL_HAS_CLEAR_COAT", "") );
						//	if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
						//		fragmentDefines.Add( ("MATERIAL_HAS_ANISOTROPY", "") );
						//}

						//!!!!
						//if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
						//	generalDefines.Add( ("DISPLACEMENT", "") );
						//!!!!
						//if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
						//	fragmentDefines.Add( ("OPACITY_DITHERING", "") );
						//!!!!
						//if( SoftParticles )
						//	fragmentDefines.Add( ("SOFT_PARTICLES", "") );
						if( voxelPass )
							defines.Add( ("VOXEL", "") );
						//if( billboardPass )
						//	generalDefines.Add( ("BILLBOARD", "") );

						//receive shadows support
						if( receiveShadows )//nShadowsSupportCounter != 0 )
						{
							//!!!!дефайны не разделены по типу программ
							defines.Add( ("SHADOW_MAP", "") );
							//fragmentDefines.Add( ("SHADOW_MAP", "") );

							////if( nShadowsSupportCounter == 2 )
							////	fragmentDefines.Add( ("SHADOW_MAP_HIGH", "") );
							////else
							////	fragmentDefines.Add( ("SHADOW_MAP_LOW", "") );
						}

						//if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterEqual )
						//	defines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
						//else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessEqual )
						//	defines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

						if( shaderGenerationEnable )
						{
							//vertex
							var vertexCode = result.vertexGeneratedCode;
							if( vertexCode != null )
							{
								if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
									defines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
								if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
									defines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
								if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
									defines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
								if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
									defines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
							}

							//material index
							var materialIndexCode = result.materialIndexGeneratedCode;
							if( materialIndexCode != null )
							{
								if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
									defines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
								if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
									defines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
								if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
									defines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
								if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
									defines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
							}

							////displacement
							//var displacementCode = result.displacementGeneratedCode;
							//if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
							//{
							//	if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
							//		fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
							//	if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
							//		fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
							//	if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
							//		fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
							//	if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
							//		fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
							//}

							//fragment
							var fragmentCode = result.fragmentGeneratedCode;
							if( fragmentCode != null )
							{
								if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
									defines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
									defines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
									defines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
								if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
									defines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
							}
						}
					}

					{
						var parameters = new GpuProgramManager.GetProgramItem( "Standard_GI_Voxel_", GpuProgramType.Compute, @"Base\Shaders\MaterialStandard_GI_Voxel.sc", defines, optimize );

						if( collecting )
							programsToCompile.Add( parameters );
						else
						{
							string error2;

							//vertex program
							var program = GpuProgramManager.GetProgram( parameters, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								return null;
							}

							//!!!!Dispose()? who else
							result.giVoxelProgram = new Program( program.RealObject );
							//result.giVoxelProgram = program;
						}
					}
					//}



					//var nPassType = 1;
					////for( int nPassType = 0; nPassType < 3; nPassType++ )//for( int nPassType = 0; nPassType < 4; nPassType++ )
					////{
					//var voxelPass = nPassType == 1;
					//var billboardPass = nPassType == 2;

					////generate compile arguments
					//var defines = new List<(string, string)>( 8 );
					//{
					//	defines.Add( ("BLEND_MODE_" + blendMode.ToString().ToUpper(), "") );
					//	defines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
					//	defines.Add( ("SHADING_MODEL_INDEX", ( (int)ShadingModel.Value ).ToString()) );
					//	//if( TwoSided && TwoSidedFlipNormals )
					//	//	fragmentDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );
					//	if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
					//		defines.Add( ("MULTI_MATERIAL_SEPARATE_PASS", "") );
					//	if( specialMode == CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
					//		defines.Add( ("MULTI_MATERIAL_COMBINED_PASS", "") );

					//	//if( RenderingSystem.DisplacementMaxSteps > 0 && Displacement.ReferenceSpecified )
					//	//	generalDefines.Add( ("DISPLACEMENT", "") );
					//	//if( ( blendMode == BlendModeEnum.Masked /*|| blendMode == BlendModeEnum.MaskedLayer */) && opacityDithering )
					//	//	fragmentDefines.Add( ("OPACITY_DITHERING", "") );
					//	if( voxelPass )
					//		defines.Add( ("VOXEL", "") );
					//	//if( billboardPass )
					//	//	generalDefines.Add( ("BILLBOARD", "") );

					//	//if( DepthOffsetMode.Value == DepthOffsetModeEnum.GreaterEqual )
					//	//	defines.Add( ("DEPTH_OFFSET_MODE_GREATER_EQUAL", "") );
					//	//else if( DepthOffsetMode.Value == DepthOffsetModeEnum.LessEqual )
					//	//	defines.Add( ("DEPTH_OFFSET_MODE_LESS_EQUAL", "") );

					//	if( shaderGenerationEnable )
					//	{
					//		//vertex
					//		var vertexCode = result.vertexGeneratedCode;
					//		if( vertexCode != null )
					//		{
					//			if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
					//				defines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
					//			if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
					//				defines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
					//			if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
					//				defines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", "\r\n" + vertexCode.shaderScripts) );
					//			if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
					//				defines.Add( ("VERTEX_CODE_BODY", "\r\n" + vertexCode.codeBody) );
					//		}

					//		//material index
					//		var materialIndexCode = result.materialIndexGeneratedCode;
					//		if( materialIndexCode != null )
					//		{
					//			if( !string.IsNullOrEmpty( materialIndexCode.parametersBody ) )
					//				defines.Add( ("MATERIAL_INDEX_CODE_PARAMETERS", materialIndexCode.parametersBody) );
					//			if( !string.IsNullOrEmpty( materialIndexCode.samplersBody ) )
					//				defines.Add( ("MATERIAL_INDEX_CODE_SAMPLERS", materialIndexCode.samplersBody) );
					//			if( !string.IsNullOrEmpty( materialIndexCode.shaderScripts ) )
					//				defines.Add( ("MATERIAL_INDEX_CODE_SHADER_SCRIPTS", "\r\n" + materialIndexCode.shaderScripts) );
					//			if( !string.IsNullOrEmpty( materialIndexCode.codeBody ) )
					//				defines.Add( ("MATERIAL_INDEX_CODE_BODY", "\r\n" + materialIndexCode.codeBody) );
					//		}

					//		////displacement
					//		//var displacementCode = result.displacementGeneratedCode;
					//		//if( RenderingSystem.DisplacementMaxSteps > 0 && displacementCode != null )
					//		//{
					//		//	if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
					//		//		fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
					//		//	if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
					//		//		fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
					//		//	if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
					//		//		fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", "\r\n" + displacementCode.shaderScripts) );
					//		//	if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
					//		//		fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", "\r\n" + displacementCode.codeBody) );
					//		//}

					//		//fragment
					//		var fragmentCode = result.fragmentGeneratedCode;
					//		if( fragmentCode != null )
					//		{
					//			if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
					//				defines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
					//			if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
					//				defines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
					//			if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
					//				defines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", "\r\n" + fragmentCode.shaderScripts) );
					//			if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
					//				defines.Add( ("FRAGMENT_CODE_BODY", "\r\n" + fragmentCode.codeBody) );
					//		}
					//	}
					//}

					//{
					//	var parameters = new GpuProgramManager.GetProgramItem( "Standard_GI_Voxel_", GpuProgramType.Compute, @"Base\Shaders\MaterialStandard_GI_Voxel.sc", defines, optimize );

					//	if( collecting )
					//		programsToCompile.Add( parameters );
					//	else
					//	{
					//		string error2;

					//		//vertex program
					//		var program = GpuProgramManager.GetProgram( parameters, out error2 );
					//		if( !string.IsNullOrEmpty( error2 ) )
					//		{
					//			result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
					//			Log.Warning( result.error );
					//			return null;
					//		}

					//		//!!!!Dispose()? who else
					//		result.giVoxelProgram = new Program( program.RealObject );
					//		//result.giVoxelProgram = program;
					//	}
					//}
					////}
				}


				if( collecting )
				{
					//precompile gpu programs in multithreaded mode
					GpuProgramManager.GetPrograms( programsToCompile );

					foreach( var item in programsToCompile )
					{
						if( !string.IsNullOrEmpty( item.Error ) )
						{
							result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, item.Error );
							Log.Warning( result.error );
							//result.Dispose();
							return null;
						}
					}
				}
			}

			//if( !result.Disposed )
			result.InitDynamicParametersUniformToUpdate();

			//Log.Info( "Total time: " + ( DateTime.Now - time ).TotalSeconds.ToString() );

			return result;
		}

		public void NewObjectCreateShaderGraph( NewMaterialData data = null )
		{
			var graph = CreateComponent<FlowGraph>();
			graph.Name = "Shader graph";
			graph.Specialization = ReferenceUtility.MakeReference(
				MetadataManager.GetTypeOfNetType( typeof( FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

			{
				var node = graph.CreateComponent<FlowGraphNode>();
				if( !string.IsNullOrEmpty( Name ) )
					node.Name = "Node " + Name;
				else
					node.Name = "Node";
				node.Position = new Vector2I( 10, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, this );
			}

			//configure
			if( data != null )
			{
				const int step = 9;
				Vector2I position = new Vector2I( -20, -data.GetTextureCount() * step / 2 );

				//BaseColor
				if( !string.IsNullOrEmpty( data.BaseColorTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "BaseColor";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.BaseColorTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					BaseColor = ReferenceUtility.MakeThisReference( this, sample, "RGBA" );
				}
				//else if( data.BaseColor.HasValue )
				//	BaseColor = data.BaseColor.Value;

				//Metallic
				if( !string.IsNullOrEmpty( data.MetallicTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Metallic";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.MetallicTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Metallic = ReferenceUtility.MakeThisReference( this, sample, "R" );
				}

				//Roughness
				if( !string.IsNullOrEmpty( data.RoughnessTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Roughness";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.RoughnessTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Roughness = ReferenceUtility.MakeThisReference( this, sample, "R" );
				}

				//Normal
				if( !string.IsNullOrEmpty( data.NormalTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Normal";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.NormalTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Normal = ReferenceUtility.MakeThisReference( this, sample, "RGBA" );
				}

				//Displacement
				if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Displacement";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.DisplacementTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Displacement = ReferenceUtility.MakeThisReference( this, sample, "R" );
				}

				//AmbientOcclusion
				if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "AmbientOcclusion";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.AmbientOcclusionTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					AmbientOcclusion = ReferenceUtility.MakeThisReference( this, sample, "R" );
				}

				//Emissive
				if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Emissive";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.EmissiveTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Emissive = ReferenceUtility.MakeThisReference( this, sample, "RGBA" );
				}

				//Opacity
				if( !string.IsNullOrEmpty( data.OpacityTexture ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + "Opacity";
					node.Position = position;
					position.Y += step;

					var sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, data.OpacityTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					Opacity = ReferenceUtility.MakeThisReference( this, sample, "R" );

					BlendMode = BlendModeEnum.Masked;
				}
			}

#if !DEPLOY
			if( Parent == null )
			{
				var toSelect = new Component[] { this, graph };
				EditorDocumentConfiguration = EditorAPI.CreateEditorDocumentXmlConfiguration( toSelect, graph );
			}
#endif
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			//don't create another shader graph if already exists in a base type
			if( !createdFromNewObjectWindow && GetComponent<FlowGraph>( "Shader graph" ) == null )
				NewObjectCreateShaderGraph();
		}

		public void EditorUpdateWhenDocumentModified()
		{
			if( EditorAutoUpdate )
				PerformResultCompile();
		}
	}
}
