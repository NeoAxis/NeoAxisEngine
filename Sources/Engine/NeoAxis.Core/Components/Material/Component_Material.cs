// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all materials.
	/// </summary>
	[ResourceFileExtension( "material" )]
	[EditorDocumentWindow( typeof( Component_Material_DocumentWindow ) )]
	[EditorPreviewControl( typeof( Component_Material_PreviewControl ) )]
	[EditorSettingsCell( typeof( Component_Material_SettingsCell ) )]
	[EditorNewObjectSettings( typeof( NewObjectSettingsMaterial ) )]
	public partial class Component_Material : Component_ResultCompile<Component_Material.CompiledMaterialData>, IComponent_EditorUpdateWhenDocumentModified
	{
		const bool shaderGenerationCompile = true;
		const bool shaderGenerationEnable = true;
		const bool shaderGenerationPrintLog = false;
		//const bool shaderGenerationPrintLog = true;

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
				if( _blendMode.BeginSet( ref value ) )
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
		public event Action<Component_Material> BlendModeChanged;
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
				if( _twoSided.BeginSet( ref value ) )
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
		public event Action<Component_Material> TwoSidedChanged;
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
				if( _twoSidedFlipNormals.BeginSet( ref value ) )
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
		public event Action<Component_Material> TwoSidedFlipNormalsChanged;
		ReferenceField<bool> _twoSidedFlipNormals = true;

		public enum ShadingModelEnum
		{
			Lit,
			Subsurface,
			Cloth,
			Specular,
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
				if( _shadingModel.BeginSet( ref value ) )
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
		public event Action<Component_Material> ShadingModelChanged;
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
			set { if( _baseColor.BeginSet( ref value ) ) { try { BaseColorChanged?.Invoke( this ); } finally { _baseColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseColor"/> property value changes.</summary>
		public event Action<Component_Material> BaseColorChanged;
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
			set { if( _metallic.BeginSet( ref value ) ) { try { MetallicChanged?.Invoke( this ); } finally { _metallic.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Metallic"/> property value changes.</summary>
		public event Action<Component_Material> MetallicChanged;
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
			set { if( _roughness.BeginSet( ref value ) ) { try { RoughnessChanged?.Invoke( this ); } finally { _roughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Roughness"/> property value changes.</summary>
		public event Action<Component_Material> RoughnessChanged;
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
			set { if( _reflectance.BeginSet( ref value ) ) { try { ReflectanceChanged?.Invoke( this ); } finally { _reflectance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Reflectance"/> property value changes.</summary>
		public event Action<Component_Material> ReflectanceChanged;
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
			set { if( _normal.BeginSet( ref value ) ) { try { NormalChanged?.Invoke( this ); } finally { _normal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Normal"/> property value changes.</summary>
		public event Action<Component_Material> NormalChanged;
		ReferenceField<Vector3> _normal;

		/// <summary>
		/// The height offset that is specified by the texture.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Shading" )]
		public Reference<double> Displacement
		{
			get { if( _displacement.BeginGet() ) Displacement = _displacement.Get( this ); return _displacement.value; }
			set { if( _displacement.BeginSet( ref value ) ) { try { DisplacementChanged?.Invoke( this ); } finally { _displacement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Displacement"/> property value changes.</summary>
		public event Action<Component_Material> DisplacementChanged;
		ReferenceField<double> _displacement = 0.0;

		/// <summary>
		/// The scale for Displacement.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Range( 0, 0.2 )]
		[Category( "Shading" )]
		public Reference<double> DisplacementScale
		{
			get { if( _displacementScale.BeginGet() ) DisplacementScale = _displacementScale.Get( this ); return _displacementScale.value; }
			set { if( _displacementScale.BeginSet( ref value ) ) { try { DisplacementScaleChanged?.Invoke( this ); } finally { _displacementScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplacementScale"/> property value changes.</summary>
		public event Action<Component_Material> DisplacementScaleChanged;
		ReferenceField<double> _displacementScale = 0.05;

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
			set { if( _ambientOcclusion.BeginSet( ref value ) ) { try { AmbientOcclusionChanged?.Invoke( this ); } finally { _ambientOcclusion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AmbientOcclusion"/> property value changes.</summary>
		public event Action<Component_Material> AmbientOcclusionChanged;
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
			set { if( _emissive.BeginSet( ref value ) ) { try { EmissiveChanged?.Invoke( this ); } finally { _emissive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Emissive"/> property value changes.</summary>
		public event Action<Component_Material> EmissiveChanged;
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
			set { if( _opacity.BeginSet( ref value ) ) { try { OpacityChanged?.Invoke( this ); } finally { _opacity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Opacity"/> property value changes.</summary>
		public event Action<Component_Material> OpacityChanged;
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
				if( _opacityDithering.BeginSet( ref value ) )
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
		public event Action<Component_Material> OpacityDitheringChanged;
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
			set { if( _opacityMaskThreshold.BeginSet( ref value ) ) { try { OpacityMaskThresholdChanged?.Invoke( this ); } finally { _opacityMaskThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpacityMaskThreshold"/> property value changes.</summary>
		public event Action<Component_Material> OpacityMaskThresholdChanged;
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
				if( _clearCoat.BeginSet( ref value ) )
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
		public event Action<Component_Material> ClearCoatChanged;
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
			set { if( _clearCoatRoughness.BeginSet( ref value ) ) { try { ClearCoatRoughnessChanged?.Invoke( this ); } finally { _clearCoatRoughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClearCoatRoughness"/> property value changes.</summary>
		public event Action<Component_Material> ClearCoatRoughnessChanged;
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
			set { if( _clearCoatNormal.BeginSet( ref value ) ) { try { ClearCoatNormalChanged?.Invoke( this ); } finally { _clearCoatNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClearCoatNormal"/> property value changes.</summary>
		public event Action<Component_Material> ClearCoatNormalChanged;
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
				if( _anisotropy.BeginSet( ref value ) )
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
		public event Action<Component_Material> AnisotropyChanged;
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
		//	set { if( _anisotropy.BeginSet( ref value ) ) { try { AnisotropyChanged?.Invoke( this ); } finally { _anisotropy.EndSet(); } } }
		//}
		//public event Action<Component_Material> AnisotropyChanged;
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
			set { if( _anisotropyDirection.BeginSet( ref value ) ) { try { AnisotropyDirectionChanged?.Invoke( this ); } finally { _anisotropyDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropyDirection"/> property value changes.</summary>
		public event Action<Component_Material> AnisotropyDirectionChanged;
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
			set { if( _anisotropyDirectionBasis.BeginSet( ref value ) ) { try { AnisotropyDirectionBasisChanged?.Invoke( this ); } finally { _anisotropyDirectionBasis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnisotropyDirectionBasis"/> property value changes.</summary>
		public event Action<Component_Material> AnisotropyDirectionBasisChanged;
		ReferenceField<AnisotropyDirectionBasisEnum> _anisotropyDirectionBasis = AnisotropyDirectionBasisEnum.Tangent;

		/// <summary>
		/// Object thickness. Used for subsurface scattering only.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<Component_Material> ThicknessChanged;
		ReferenceField<double> _thickness = 0.5;

		/// <summary>
		/// Amount of subsurface scattering.
		/// </summary>
		[DefaultValue( 12.234 )]
		[Range( 0, 14 )]
		[Serialize]
		[Category( "Shading" )]
		public Reference<double> SubsurfacePower
		{
			get { if( _subsurfacePower.BeginGet() ) SubsurfacePower = _subsurfacePower.Get( this ); return _subsurfacePower.value; }
			set { if( _subsurfacePower.BeginSet( ref value ) ) { try { SubsurfacePowerChanged?.Invoke( this ); } finally { _subsurfacePower.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SubsurfacePower"/> property value changes.</summary>
		public event Action<Component_Material> SubsurfacePowerChanged;
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
			set { if( _sheenColor.BeginSet( ref value ) ) { try { SheenColorChanged?.Invoke( this ); } finally { _sheenColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SheenColor"/> property value changes.</summary>
		public event Action<Component_Material> SheenColorChanged;
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
			set { if( _subsurfaceColor.BeginSet( ref value ) ) { try { SubsurfaceColorChanged?.Invoke( this ); } finally { _subsurfaceColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SubsurfaceColor"/> property value changes.</summary>
		public event Action<Component_Material> SubsurfaceColorChanged;
		ReferenceField<ColorValue> _subsurfaceColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The shininess of material.
		/// </summary>
		[DefaultValue( 20.0 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Shading" )]
		public Reference<double> Shininess
		{
			get { if( _shininess.BeginGet() ) Shininess = _shininess.Get( this ); return _shininess.value; }
			set { if( _shininess.BeginSet( ref value ) ) { try { ShininessChanged?.Invoke( this ); } finally { _shininess.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shininess"/> property value changes.</summary>
		public event Action<Component_Material> ShininessChanged;
		ReferenceField<double> _shininess = 20.0;

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
				if( _positionOffset.BeginSet( ref value ) )
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
		public event Action<Component_Material> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset;

		/// <summary>
		/// Amount of ray-tracing reflection. Used for screen space reflection.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Category( "Shading" )]
		[Range( 0, 1 )]
		public Reference<double> RayTracingReflection
		{
			get { if( _rayTracingReflection.BeginGet() ) RayTracingReflection = _rayTracingReflection.Get( this ); return _rayTracingReflection.value; }
			set { if( _rayTracingReflection.BeginSet( ref value ) ) { try { RayTracingReflectionChanged?.Invoke( this ); } finally { _rayTracingReflection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RayTracingReflection"/> property value changes.</summary>
		public event Action<Component_Material> RayTracingReflectionChanged;
		ReferenceField<double> _rayTracingReflection = 0.0;

		/// <summary>
		/// Whether the surface receive shadows from other sources.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> ReceiveShadows
		{
			get
			{
				//!!!!ut
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					return false;

				if( _receiveShadows.BeginGet() ) ReceiveShadows = _receiveShadows.Get( this ); return _receiveShadows.value;
			}
			set
			{
				if( _receiveShadows.BeginSet( ref value ) )
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
		public event Action<Component_Material> ReceiveShadowsChanged;
		ReferenceField<bool> _receiveShadows = true;

		/// <summary>
		/// Whether it is possible to apply decals the surface.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shading" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> ReceiveDecals
		{
			get { if( _receiveDecals.BeginGet() ) ReceiveDecals = _receiveDecals.Get( this ); return _receiveDecals.value; }
			set { if( _receiveDecals.BeginSet( ref value ) ) { try { ReceiveDecalsChanged?.Invoke( this ); } finally { _receiveDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ReceiveDecals"/> property value changes.</summary>
		public event Action<Component_Material> ReceiveDecalsChanged;
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
			set { if( _useVertexColor.BeginSet( ref value ) ) { try { UseVertexColorChanged?.Invoke( this ); } finally { _useVertexColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseVertexColor"/> property value changes.</summary>
		public event Action<Component_Material> UseVertexColorChanged;
		ReferenceField<bool> _useVertexColor = true;

		/// <summary>
		/// Enables advanced blending mode. In this mode, it is possible to configure blending for each channel separately.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Advanced Blending" )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> AdvancedBlending
		{
			get { if( _advancedBlending.BeginGet() ) AdvancedBlending = _advancedBlending.Get( this ); return _advancedBlending.value; }
			set { if( _advancedBlending.BeginSet( ref value ) ) { try { AdvancedBlendingChanged?.Invoke( this ); } finally { _advancedBlending.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AdvancedBlending"/> property value changes.</summary>
		public event Action<Component_Material> AdvancedBlendingChanged;
		ReferenceField<bool> _advancedBlending = false;

		//!!!!flow graph
		//!!!!double

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
			set { if( _affectBaseColor.BeginSet( ref value ) ) { try { AffectBaseColorChanged?.Invoke( this ); } finally { _affectBaseColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectBaseColor"/> property value changes.</summary>
		public event Action<Component_Material> AffectBaseColorChanged;
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
			set { if( _affectMetallic.BeginSet( ref value ) ) { try { AffectMetallicChanged?.Invoke( this ); } finally { _affectMetallic.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectMetallic"/> property value changes.</summary>
		public event Action<Component_Material> AffectMetallicChanged;
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
			set { if( _affectRoughness.BeginSet( ref value ) ) { try { AffectRoughnessChanged?.Invoke( this ); } finally { _affectRoughness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectRoughness"/> property value changes.</summary>
		public event Action<Component_Material> AffectRoughnessChanged;
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
			set { if( _affectReflectance.BeginSet( ref value ) ) { try { AffectReflectanceChanged?.Invoke( this ); } finally { _affectReflectance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectReflectance"/> property value changes.</summary>
		public event Action<Component_Material> AffectReflectanceChanged;
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
			set { if( _affectAmbientOcclusion.BeginSet( ref value ) ) { try { AffectAmbientOcclusionChanged?.Invoke( this ); } finally { _affectAmbientOcclusion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectAmbientOcclusion"/> property value changes.</summary>
		public event Action<Component_Material> AffectAmbientOcclusionChanged;
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
			set { if( _affectEmissive.BeginSet( ref value ) ) { try { AffectEmissiveChanged?.Invoke( this ); } finally { _affectEmissive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectEmissive"/> property value changes.</summary>
		public event Action<Component_Material> AffectEmissiveChanged;
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
			set { if( _affectGeometry.BeginSet( ref value ) ) { try { AffectGeometryChanged?.Invoke( this ); } finally { _affectGeometry.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectGeometry"/> property value changes.</summary>
		public event Action<Component_Material> AffectGeometryChanged;
		ReferenceField<int> _affectGeometry = 1;



		//!!!!CastShadows?
		//!!!!!!если выключен, то не нужно генерить special shadow caster data

		//!!!!later
		////ShadingFlow
		//ReferenceField<Component> _shadingFlow;
		//[Serialize]
		//[DefaultValue( null )]
		//[Category( "Shading" )]
		//public Reference<Component> ShadingFlow
		//{
		//	get
		//	{
		//		if( _shadingFlow.BeginGet() )
		//			ShadingFlow = _shadingFlow.Get( this );
		//		return _shadingFlow.value;
		//	}
		//	set
		//	{
		//		if( _shadingFlow.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				ShadingFlowChanged?.Invoke( this );
		//				//!!!!
		//			}
		//			finally { _shadingFlow.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_MaterialStandard> ShadingFlowChanged;

		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		/////////////////////////////////////////

		/// <summary>
		/// A set of settings for <see cref="Component_Material"/> creation in the editor.
		/// </summary>
		public class NewObjectSettingsMaterial : NewObjectSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			[DisplayName( "Shader graph" )]
			public bool ShaderGraph { get; set; } = true;

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				var newObject2 = (Component_Material)context.newObject;

				if( ShaderGraph )
					newObject2.NewObjectCreateShaderGraph();

				return base.Creation( context );
			}
		}

		/////////////////////////////////////////

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
						if( model != ShadingModelEnum.Lit && model != ShadingModelEnum.Subsurface )
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
					if( ShadingModel.Value != ShadingModelEnum.Subsurface )
						skip = true;
					break;

				case nameof( SheenColor ):
					if( ShadingModel.Value != ShadingModelEnum.Cloth )
						skip = true;
					break;

				case nameof( SubsurfaceColor ):
					{
						var m = ShadingModel.Value;
						if( m != ShadingModelEnum.Subsurface && m != ShadingModelEnum.Cloth )
							skip = true;
					}
					break;

				case nameof( Shininess ):
					if( ShadingModel.Value != ShadingModelEnum.Specular )
						skip = true;
					break;

				case nameof( RayTracingReflection ):
					{
						var m = ShadingModel.Value;
						if( m == ShadingModelEnum.Specular || m == ShadingModelEnum.Simple || m == ShadingModelEnum.Unlit )
							skip = true;
					}
					break;

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
				}
			}
		}

		protected override void OnResultCompile()
		{
			if( Result != null )
				return;
			Result = Compile( CompiledMaterialData.UsageMode.Usual );
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

		bool GenerateCode( CompiledMaterialData compiledData, bool needSpecialShadowCaster )
		{
			//vertex
			{
				var properties = new List<(Component, Metadata.Property)>();
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( PositionOffset ) )) );
				if( compiledData.extensionData != null )
					properties.AddRange( compiledData.extensionData.vertexShaderProperties );

				var generator = new ShaderGenerator();
				int textureRegisterCounter = 9;
				var code = generator.Process( properties, "vertex_", ref textureRegisterCounter, out string error );

				//process error
				if( !string.IsNullOrEmpty( error ) )
				{
					//!!!!

					return false;
				}

				//print to log
				if( code != null && shaderGenerationPrintLog )
					code.PrintToLog( GetDisplayName() + ", Vertex shader" );

				compiledData.vertexGeneratedCode = code;
			}

			int fragmentTextureRegisterCounter = 9;

			//displacement
			if( Displacement.ReferenceSpecified )
			{
				var properties = new List<(Component, Metadata.Property)>();
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Displacement ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( DisplacementScale ) )) );
				//if( compiledData.extensionData != null )
				//	properties.AddRange( compiledData.extensionData.vertexShaderProperties );

				var generator = new ShaderGenerator();
				var code = generator.Process( properties, "displacement_", ref fragmentTextureRegisterCounter, out string error );

				//process error
				if( !string.IsNullOrEmpty( error ) )
				{
					//!!!!

					return false;
				}

				//print to log
				if( code != null && shaderGenerationPrintLog )
					code.PrintToLog( GetDisplayName() + ", Displacement shader code" );

				compiledData.displacementGeneratedCode = code;
			}

			//fragment
			{
				var properties = new List<(Component, Metadata.Property)>( 16 );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Normal ) )) );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( BaseColor ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( OpacityMaskThreshold ) )) );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Metallic ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Roughness ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Reflectance ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoat ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoatRoughness ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( ClearCoatNormal ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Anisotropy ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AnisotropyDirection ) )) );
				//properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AnisotropyDirectionBasis ) )) );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Thickness ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SubsurfacePower ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SheenColor ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( SubsurfaceColor ) )) );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Shininess ) )) );

				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( AmbientOcclusion ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( RayTracingReflection ) )) );
				properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Emissive ) )) );


				if( compiledData.extensionData != null )
					properties.AddRange( compiledData.extensionData.fragmentShaderProperties );

				var generator = new ShaderGenerator();
				var code = generator.Process( properties, "fragment_", ref fragmentTextureRegisterCounter, out string error );

				//process error
				if( !string.IsNullOrEmpty( error ) )
				{
					//!!!!

					return false;
				}

				//print to log
				if( code != null && shaderGenerationPrintLog )
					code.PrintToLog( GetDisplayName() + ", Fragment shader" );

				compiledData.fragmentGeneratedCode = code;
			}

			//special shadow caster
			if( needSpecialShadowCaster )
			{
				//vertex
				{
					var properties = new List<(Component, Metadata.Property)>();
					properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( PositionOffset ) )) );
					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.specialShadowCasterVertexShaderProperties );

					var generator = new ShaderGenerator();
					int textureRegisterCounter = 9;
					var code = generator.Process( properties, "vertex_", ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
					{
						//!!!!

						return false;
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Shadow caster vertex shader" );

					compiledData.vertexGeneratedCodeShadowCaster = code;
				}

				//fragment
				{
					var properties = new List<(Component, Metadata.Property)>();
					properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( Opacity ) )) );
					properties.Add( (this, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( OpacityMaskThreshold ) )) );
					if( compiledData.extensionData != null )
						properties.AddRange( compiledData.extensionData.specialShadowCasterFragmentShaderProperties );

					var generator = new ShaderGenerator();
					int textureRegisterCounter = 9;
					var code = generator.Process( properties, "fragment_", ref textureRegisterCounter, out string error );

					//process error
					if( !string.IsNullOrEmpty( error ) )
					{
						//!!!!

						return false;
					}

					//print to log
					if( code != null && shaderGenerationPrintLog )
						code.PrintToLog( GetDisplayName() + ", Shadow caster fragment shader" );

					compiledData.fragmentGeneratedCodeShadowCaster = code;
				}
			}

			return true;
		}

		/// <summary>
		/// Represents material data that were procedurally generated and compiled.
		/// </summary>
		public class CompileExtensionData
		{
			public List<(Component, Metadata.Property)> vertexShaderProperties = new List<(Component, Metadata.Property)>();
			//!!!!displacement?
			public List<(Component, Metadata.Property)> fragmentShaderProperties = new List<(Component, Metadata.Property)>();
			public List<(Component, Metadata.Property)> specialShadowCasterVertexShaderProperties = new List<(Component, Metadata.Property)>();
			public List<(Component, Metadata.Property)> specialShadowCasterFragmentShaderProperties = new List<(Component, Metadata.Property)>();
		}

		/////////////////////////////////////////

		protected virtual string OnCheckDeferredShadingSupport()
		{
			if( BlendMode.Value == BlendModeEnum.Transparent || BlendMode.Value == BlendModeEnum.Add )
				return "Blend Mode";
			if( ShadingModel.Value != ShadingModelEnum.Lit && ShadingModel.Value != ShadingModelEnum.Simple )
				return "Shading Model";
			if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
				return "Clear Coat";
			if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
				return "Anisotropy";
			if( !ReceiveShadows )
				return "Receive Shadows";
			return "";
		}

		public delegate void CheckDeferredShadingSupportEventDelegate( Component_Material sender, ref string reason );
		public event CheckDeferredShadingSupportEventDelegate CheckDeferredShadingSupportEvent;

		string PerformCheckDeferredShadingSupport()
		{
			string reason = OnCheckDeferredShadingSupport();
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckDeferredShadingSupportEvent?.Invoke( this, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		protected virtual string OnCheckReceiveDecalsSupport()
		{
			if( !ReceiveDecals )
				return "Receive Decals";
			if( !string.IsNullOrEmpty( PerformCheckDeferredShadingSupport() ) )
				return "Deferred Shading";
			return "";
		}

		public delegate void CheckReceiveDecalsSupportEventDelegate( Component_Material sender, ref string reason );
		public event CheckReceiveDecalsSupportEventDelegate CheckReceiveDecalsSupportEvent;

		string PerformCheckReceiveDecalsSupport()
		{
			string reason = OnCheckReceiveDecalsSupport();
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckReceiveDecalsSupportEvent?.Invoke( this, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		protected virtual string OnCheckDecalSupport()
		{
			//if( !ReceiveDecals )
			//	return "Receive Decals";
			if( !string.IsNullOrEmpty( PerformCheckDeferredShadingSupport() ) )
				return "Deferred Shading";
			return "";
		}

		public delegate void CheckDecalSupportEventDelegate( Component_Material sender, ref string reason );
		public event CheckDecalSupportEventDelegate CheckDecalSupportEvent;

		string PerformCheckDecalSupport()
		{
			string reason = OnCheckDecalSupport();
			if( !string.IsNullOrEmpty( reason ) )
				return reason;
			CheckDecalSupportEvent?.Invoke( this, ref reason );
			return reason;
		}

		/////////////////////////////////////////

		public virtual CompiledMaterialData Compile( CompiledMaterialData.UsageMode usageMode, CompileExtensionData extensionData = null )
		{
			var result = new CompiledMaterialData();
			result.owner = this;
			result.usageMode = usageMode;
			result.extensionData = extensionData;

			result.Transparent = BlendMode.Value == BlendModeEnum.Transparent || BlendMode.Value == BlendModeEnum.Add;
			result.ShadingModel = ShadingModel.Value;

			//deferred shading
			result.deferredShadingSupportReason = PerformCheckDeferredShadingSupport();
			result.deferredShadingSupport = string.IsNullOrEmpty( result.deferredShadingSupportReason );

			//receive decals
			result.receiveDecalsSupportReason = PerformCheckReceiveDecalsSupport();
			result.receiveDecalsSupport = string.IsNullOrEmpty( result.receiveDecalsSupportReason );

			//decal
			result.decalSupportReason = PerformCheckDecalSupport();
			result.decalSupport = string.IsNullOrEmpty( result.decalSupportReason );

			//!!!!
			EngineThreading.ExecuteFromMainThreadWait( delegate ()
			{

				//!!!!что еще?
				bool needSpecialShadowCaster = PositionOffset.ReferenceSpecified || BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer */|| BlendMode.Value == BlendModeEnum.Transparent;

				//shader generation
				if( shaderGenerationCompile )
				{
					if( !GenerateCode( result, needSpecialShadowCaster ) )
						return;
				}


				//base material

				bool unlit = ShadingModel.Value == ShadingModelEnum.Unlit;
				if( unlit )
					result.passesByLightType = new CompiledMaterialData.PassGroup[ 1 ];
				else
				{
					//!!!! 4
					result.passesByLightType = new CompiledMaterialData.PassGroup[ 4 ];
				}

				foreach( Component_Light.TypeEnum lightType in Enum.GetValues( typeof( Component_Light.TypeEnum ) ) )
				{
					if( unlit && lightType != Component_Light.TypeEnum.Ambient )
						break;

					CompiledMaterialData.PassGroup group = new CompiledMaterialData.PassGroup();
					result.passesByLightType[ (int)lightType ] = group;

					//one or two iterations depending Ambient light source, ReceiveShadows
					int shadowsSupportIterations = 1;
					if( lightType != Component_Light.TypeEnum.Ambient && ReceiveShadows.Value )
						shadowsSupportIterations = 3;// 2;
					for( int nShadowsSupportCounter = 0; nShadowsSupportCounter < shadowsSupportIterations; nShadowsSupportCounter++ )
					{
						//generate compile arguments
						var vertexDefines = new List<(string, string)>( 8 );
						var fragmentDefines = new List<(string, string)>( 8 );
						{
							var generalDefines = new List<(string, string)>( 16 );
							generalDefines.Add( ("USAGE_MODE_" + usageMode.ToString().ToUpper(), "") );
							generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
							generalDefines.Add( ("BLEND_MODE_" + BlendMode.Value.ToString().ToUpper(), "") );
							generalDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
							if( TwoSided && TwoSidedFlipNormals )
								generalDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );

							if( ShadingModel.Value == ShadingModelEnum.Lit )
							{
								if( ClearCoat.ReferenceSpecified || ClearCoat.Value != 0 )
									generalDefines.Add( ("MATERIAL_HAS_CLEAR_COAT", "") );
								if( Anisotropy.ReferenceSpecified || Anisotropy.Value != 0 )
									generalDefines.Add( ("MATERIAL_HAS_ANISOTROPY", "") );
							}

							if( Displacement.ReferenceSpecified )
								generalDefines.Add( ("DISPLACEMENT", "") );
							if( ( BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer */) && OpacityDithering )
								generalDefines.Add( ("OPACITY_DITHERING", "") );

							//receive shadows support
							if( nShadowsSupportCounter != 0 )
							{
								generalDefines.Add( ("SHADOW_MAP", "") );

								if( nShadowsSupportCounter == 2 )
									generalDefines.Add( ("SHADOW_MAP_HIGH", "") );
								else
									generalDefines.Add( ("SHADOW_MAP_LOW", "") );
							}

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
										vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", vertexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
										vertexDefines.Add( ("VERTEX_CODE_BODY", vertexCode.codeBody) );
								}

								//displacement
								var displacementCode = result.displacementGeneratedCode;
								if( displacementCode != null )
								{
									if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
									if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
									if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", displacementCode.shaderScripts) );
									if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
										fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", displacementCode.codeBody) );
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
										fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", fragmentCode.shaderScripts) );
									if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_BODY", fragmentCode.codeBody) );
								}
							}
						}

						{
							string error2;

							//vertex program
							GpuProgram vertexProgram = GpuProgramManager.GetProgram( "Standard_Forward_Vertex_",
								GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Forward_vs.sc", vertexDefines, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								result.Dispose();
								return;
							}

							//fragment program
							GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "Standard_Forward_Fragment_",
								GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Forward_fs.sc", fragmentDefines, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								result.Dispose();
								return;
							}

							var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
							result.AllPasses.Add( pass );
							if( nShadowsSupportCounter == 2 )
								group.passWithShadowsHigh = pass;
							else if( nShadowsSupportCounter == 1 )
								group.passWithShadowsLow = pass;
							else
								group.passWithoutShadows = pass;
							//if( nShadowsSupportCounter == 2 )
							//	group.passesWithShadowsHigh.Add( pass );
							//else if( nShadowsSupportCounter == 1 )
							//	group.passesWithShadowsLow.Add( pass );
							//else
							//	group.passesWithoutShadows.Add( pass );

							if( BlendMode.Value == BlendModeEnum.Opaque || BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer*/ )
							{
								if( lightType == Component_Light.TypeEnum.Ambient )
								{
									pass.DepthWrite = true;
									pass.SourceBlendFactor = SceneBlendFactor.One;
									pass.DestinationBlendFactor = SceneBlendFactor.Zero;
								}
								else
								{
									pass.DepthWrite = false;
									pass.SourceBlendFactor = SceneBlendFactor.One;
									pass.DestinationBlendFactor = SceneBlendFactor.One;
								}
								//if( lightType != Component_Light.TypeEnum.Ambient || usageMode == CompiledDataStandard.UsageMode.MaterialBlendNotFirst )
								//{
								//	pass.DepthWrite = false;
								//	pass.SourceBlendFactor = SceneBlendFactor.One;
								//	pass.DestinationBlendFactor = SceneBlendFactor.One;
								//}
								//else
								//{
								//	pass.DepthWrite = true;
								//	pass.SourceBlendFactor = SceneBlendFactor.One;
								//	pass.DestinationBlendFactor = SceneBlendFactor.Zero;
								//}
							}
							else if( BlendMode.Value == BlendModeEnum.Transparent )
							{
								//!!!!usageMode

								if( lightType == Component_Light.TypeEnum.Ambient )
								{
									pass.DepthWrite = false;
									pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
									pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
								}
								else
								{
									pass.DepthWrite = false;
									pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
									pass.DestinationBlendFactor = SceneBlendFactor.One;
								}
							}
							else if( BlendMode.Value == BlendModeEnum.Add )
							{
								pass.DepthWrite = false;
								pass.SourceBlendFactor = SceneBlendFactor.One;
								pass.DestinationBlendFactor = SceneBlendFactor.One;
							}

							if( TwoSided )
								pass.CullingMode = CullingMode.None;

							//Component_Texture baseTextureV = BaseTexture;
							////!!!!ниже проверять незагурежнность? есть инвалидные текстуры для показа ошибки, а есть белые или еще какие-то, которые для замены
							//if( baseTextureV == null )
							//	baseTextureV = ResourceUtils.GetWhiteTexture2D();

							//GpuMaterialPass.TextureParameterValue textureValue = new GpuMaterialPass.TextureParameterValue( baseTextureV,
							//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
							//pass.ConstantParameterValues.Set( "baseTexture", textureValue, ParameterType.Texture2D );
						}
					}
				}


				//special shadow caster material
				if( needSpecialShadowCaster )
				{
					result.specialShadowCasterData = new Component_RenderingPipeline.ShadowCasterData();

					//!!!! 4
					result.specialShadowCasterData.passByLightType = new GpuMaterialPass[ 4 ];

					foreach( Component_Light.TypeEnum lightType in Enum.GetValues( typeof( Component_Light.TypeEnum ) ) )
					{
						if( lightType == Component_Light.TypeEnum.Ambient )
							continue;

						//generate compile arguments
						var vertexDefines = new List<(string, string)>( 8 );
						var fragmentDefines = new List<(string, string)>( 8 );
						{
							var generalDefines = new List<(string, string)>();
							generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
							generalDefines.Add( ("BLEND_MODE_" + BlendMode.Value.ToString().ToUpper(), "") );

							if( ( BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer */) && OpacityDithering )
								generalDefines.Add( ("OPACITY_DITHERING", "") );

							vertexDefines.AddRange( generalDefines );
							fragmentDefines.AddRange( generalDefines );

							if( shaderGenerationEnable )
							{
								//vertex
								var vertexCode = result.vertexGeneratedCodeShadowCaster;
								if( vertexCode != null )
								{
									if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
									if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
										vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
									if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
										vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", vertexCode.shaderScripts) );
									if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
										vertexDefines.Add( ("VERTEX_CODE_BODY", vertexCode.codeBody) );
								}

								//!!!!displacement?

								//fragment
								var fragmentCode = result.fragmentGeneratedCodeShadowCaster;
								if( fragmentCode != null )
								{
									if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
									if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", fragmentCode.shaderScripts) );
									if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
										fragmentDefines.Add( ("FRAGMENT_CODE_BODY", fragmentCode.codeBody) );
								}
							}
						}

						{
							string error2;

							//vertex program
							GpuProgram vertexProgram = GpuProgramManager.GetProgram( "Standard_ShadowCaster_Vertex_",
								GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_ShadowCaster_vs.sc", vertexDefines, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								result.Dispose();
								return;
							}

							//fragment program
							GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "Standard_ShadowCaster_Fragment_",
								GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_ShadowCaster_fs.sc", fragmentDefines, out error2 );
							if( !string.IsNullOrEmpty( error2 ) )
							{
								result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
								Log.Warning( result.error );
								result.Dispose();
								return;
							}

							var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
							result.specialShadowCasterData.passByLightType[ (int)lightType ] = pass;

							if( TwoSided )
								pass.CullingMode = CullingMode.None;
						}
					}
				}

				//deferred shading pass
				if( result.deferredShadingSupport )
				{
					//generate compile arguments
					var vertexDefines = new List<(string, string)>( 8 );
					var fragmentDefines = new List<(string, string)>( 8 );
					{
						var generalDefines = new List<(string, string)>();
						generalDefines.Add( ("BLEND_MODE_" + BlendMode.Value.ToString().ToUpper(), "") );
						generalDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
						if( TwoSided && TwoSidedFlipNormals )
							generalDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );

						if( Displacement.ReferenceSpecified )
							generalDefines.Add( ("DISPLACEMENT", "") );
						if( ( BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer */) && OpacityDithering )
							generalDefines.Add( ("OPACITY_DITHERING", "") );

						////receive shadows support
						//if( nShadowsSupportCounter != 0 )
						//{
						//	generalDefines.Add( ( "SHADOW_MAP", "" ) );

						//	//!!!!
						//	generalDefines.Add( ( "SHADOW_MAP_LOW", "" ) );
						//}

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );

						if( shaderGenerationEnable )
						{
							//!!!!может тут попроще для деферреда

							//vertex
							var vertexCode = result.vertexGeneratedCode;
							if( vertexCode != null )
							{
								if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
								if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
								if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
									vertexDefines.Add( ("VERTEX_CODE_BODY", vertexCode.codeBody) );
								if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
									vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", vertexCode.shaderScripts) );
							}

							//displacement
							var displacementCode = result.displacementGeneratedCode;
							if( displacementCode != null )
							{
								if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
								if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
								if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", displacementCode.shaderScripts) );
								if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", displacementCode.codeBody) );
							}

							//fragment
							var fragmentCode = result.fragmentGeneratedCode;
							if( fragmentCode != null )
							{
								if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_BODY", fragmentCode.codeBody) );
								if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", fragmentCode.shaderScripts) );
							}
						}
					}

					{
						string error2;

						//vertex program
						GpuProgram vertexProgram = GpuProgramManager.GetProgram( "Standard_Deferred_Vertex_",
							GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Deferred_vs.sc", vertexDefines, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
							Log.Warning( result.error );
							result.Dispose();
							return;
						}

						//fragment program
						GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "Standard_Deferred_Fragment_",
							GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Deferred_fs.sc", fragmentDefines, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
							Log.Warning( result.error );
							result.Dispose();
							return;
						}

						var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
						result.AllPasses.Add( pass );
						result.deferredShadingPass = pass;
						//if( nShadowsSupportCounter != 0 )
						//	group.passesWithShadows.Add( pass );
						//else
						//	group.passesWithoutShadows.Add( pass );

						if( TwoSided )
							pass.CullingMode = CullingMode.None;

						//Component_Texture baseTextureV = BaseTexture;
						////!!!!ниже проверять незагурежнность? есть инвалидные текстуры для показа ошибки, а есть белые или еще какие-то, которые для замены
						//if( baseTextureV == null )
						//	baseTextureV = ResourceUtils.GetWhiteTexture2D();

						//GpuMaterialPass.TextureParameterValue textureValue = new GpuMaterialPass.TextureParameterValue( baseTextureV,
						//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
						//pass.ConstantParameterValues.Set( "baseTexture", textureValue, ParameterType.Texture2D );
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
						generalDefines.Add( ("BLEND_MODE_" + BlendMode.Value.ToString().ToUpper(), "") );
						generalDefines.Add( ("SHADING_MODEL_" + ShadingModel.Value.ToString().ToUpper(), "") );
						if( TwoSided && TwoSidedFlipNormals )
							generalDefines.Add( ("TWO_SIDED_FLIP_NORMALS", "") );

						if( Displacement.ReferenceSpecified )
							generalDefines.Add( ("DISPLACEMENT", "") );
						if( ( BlendMode.Value == BlendModeEnum.Masked /*|| BlendMode.Value == BlendModeEnum.MaskedLayer */) && OpacityDithering )
							generalDefines.Add( ("OPACITY_DITHERING", "") );

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );

						if( shaderGenerationEnable )
						{
							//!!!!может тут попроще для деферреда

							//vertex
							var vertexCode = result.vertexGeneratedCode;
							if( vertexCode != null )
							{
								if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_PARAMETERS", vertexCode.parametersBody) );
								if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
									vertexDefines.Add( ("VERTEX_CODE_SAMPLERS", vertexCode.samplersBody) );
								if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
									vertexDefines.Add( ("VERTEX_CODE_BODY", vertexCode.codeBody) );
								if( !string.IsNullOrEmpty( vertexCode.shaderScripts ) )
									vertexDefines.Add( ("VERTEX_CODE_SHADER_SCRIPTS", vertexCode.shaderScripts) );
							}

							//displacement
							var displacementCode = result.displacementGeneratedCode;
							if( displacementCode != null )
							{
								if( !string.IsNullOrEmpty( displacementCode.parametersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_PARAMETERS", displacementCode.parametersBody) );
								if( !string.IsNullOrEmpty( displacementCode.samplersBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SAMPLERS", displacementCode.samplersBody) );
								if( !string.IsNullOrEmpty( displacementCode.shaderScripts ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_SHADER_SCRIPTS", displacementCode.shaderScripts) );
								if( !string.IsNullOrEmpty( displacementCode.codeBody ) )
									fragmentDefines.Add( ("DISPLACEMENT_CODE_BODY", displacementCode.codeBody) );
							}

							//fragment
							var fragmentCode = result.fragmentGeneratedCode;
							if( fragmentCode != null )
							{
								if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody) );
								if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_BODY", fragmentCode.codeBody) );
								if( !string.IsNullOrEmpty( fragmentCode.shaderScripts ) )
									fragmentDefines.Add( ("FRAGMENT_CODE_SHADER_SCRIPTS", fragmentCode.shaderScripts) );
							}
						}
					}

					{
						string error2;

						//vertex program
						GpuProgram vertexProgram = GpuProgramManager.GetProgram( "Standard_DeferredDecal_Vertex_",
							GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_DeferredDecal_vs.sc", vertexDefines, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
							Log.Warning( result.error );
							result.Dispose();
							return;
						}

						//fragment program
						GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "Standard_DeferredDecal_Fragment_",
							GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_DeferredDecal_fs.sc", fragmentDefines, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							result.error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
							Log.Warning( result.error );
							result.Dispose();
							return;
						}

						var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
						result.AllPasses.Add( pass );
						result.decalShadingPass = pass;

						pass.DepthCheck = false;
						pass.DepthWrite = false;
						pass.CullingMode = CullingMode.Anticlockwise;

						////!!!!temp
						//pass.SettempAlphaToCoverage( BlendMode.Value == BlendModeEnum.Masked );

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

			} );

			if( !result.Disposed )
				result.InitDynamicParametersUniformToUpdate();

			return result;
		}

		void NewObjectCreateShaderGraph()
		{
			var graph = CreateComponent<Component_FlowGraph>();
			graph.Name = "Shader graph";
			graph.Specialization = ReferenceUtility.MakeReference(
				MetadataManager.GetTypeOfNetType( typeof( Component_FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

			var node = graph.CreateComponent<Component_FlowGraphNode>();
			node.Name = "Node " + Name;
			node.Position = new Vector2I( 10, -7 );
			node.ControlledObject = ReferenceUtility.MakeThisReference( node, this );

			if( Parent == null )
			{
				var toSelect = new Component[] { this, graph };
				EditorDocumentConfiguration = KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( toSelect, graph );
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			if( !createdFromNewObjectWindow )
				NewObjectCreateShaderGraph();
		}

		public void EditorUpdateWhenDocumentModified()
		{
			if( EditorAutoUpdate )
				ResultCompile();
		}
	}
}
