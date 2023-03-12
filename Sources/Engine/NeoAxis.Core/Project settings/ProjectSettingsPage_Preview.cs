// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Preview page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_Preview : ProjectSettingsPage
	{
		/// <summary>
		/// The brightness of the ambient light.
		/// </summary>
		[Category( "Preview" )]
		[DefaultValue( 50000.0 )]
		[Range( 0, 100000 )]
		public Reference<double> PreviewAmbientLightBrightness
		{
			get { if( _previewAmbientLightBrightness.BeginGet() ) PreviewAmbientLightBrightness = _previewAmbientLightBrightness.Get( this ); return _previewAmbientLightBrightness.value; }
			set { if( _previewAmbientLightBrightness.BeginSet( ref value ) ) { try { PreviewAmbientLightBrightnessChanged?.Invoke( this ); } finally { _previewAmbientLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewAmbientLightBrightness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> PreviewAmbientLightBrightnessChanged;
		ReferenceField<double> _previewAmbientLightBrightness = 50000.0;

		/// <summary>
		/// The brightness of the directional light.
		/// </summary>
		[Category( "Preview" )]
		[DefaultValue( 200000.0 )]
		[Range( 0, 1000000 )]
		public Reference<double> PreviewDirectionalLightBrightness
		{
			get { if( _previewDirectionalLightBrightness.BeginGet() ) PreviewDirectionalLightBrightness = _previewDirectionalLightBrightness.Get( this ); return _previewDirectionalLightBrightness.value; }
			set { if( _previewDirectionalLightBrightness.BeginSet( ref value ) ) { try { PreviewDirectionalLightBrightnessChanged?.Invoke( this ); } finally { _previewDirectionalLightBrightness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreviewDirectionalLightBrightness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> PreviewDirectionalLightBrightnessChanged;
		ReferenceField<double> _previewDirectionalLightBrightness = 200000.0;

		/// <summary>
		/// The material of preview environment for the light theme.
		/// </summary>
		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Light Theme" )]
		public Reference<ImageComponent> MaterialPreviewEnvironmentLightTheme
		{
			get { if( _materialPreviewEnvironmentLightTheme.BeginGet() ) MaterialPreviewEnvironmentLightTheme = _materialPreviewEnvironmentLightTheme.Get( this ); return _materialPreviewEnvironmentLightTheme.value; }
			set { if( _materialPreviewEnvironmentLightTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentLightThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentLightTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentLightTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> MaterialPreviewEnvironmentLightThemeChanged;
		ReferenceField<ImageComponent> _materialPreviewEnvironmentLightTheme = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		/// <summary>
		/// The preview mesh for the material.
		/// </summary>
		[DefaultValueReference( @"Base\Tools\Material Preview\Sphere.mesh" )]
		[Category( "Preview" )]
		public Reference<Mesh> MaterialPreviewMesh
		{
			get { if( _materialPreviewMesh.BeginGet() ) MaterialPreviewMesh = _materialPreviewMesh.Get( this ); return _materialPreviewMesh.value; }
			set { if( _materialPreviewMesh.BeginSet( ref value ) ) { try { MaterialPreviewMeshChanged?.Invoke( this ); } finally { _materialPreviewMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewMesh"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> MaterialPreviewMeshChanged;
		ReferenceField<Mesh> _materialPreviewMesh = new Reference<Mesh>( null, @"Base\Tools\Material Preview\Sphere.mesh" );

		/// <summary>
		/// The material of preview environment for the dark theme.
		/// </summary>
		[DefaultValueReference( @"Content\Environments\Base\Forest.image" )]
		[Category( "Preview" )]
		[DisplayName( "Material Preview Environment for Dark Theme" )]
		public Reference<ImageComponent> MaterialPreviewEnvironmentDarkTheme
		{
			get { if( _materialPreviewEnvironmentDarkTheme.BeginGet() ) MaterialPreviewEnvironmentDarkTheme = _materialPreviewEnvironmentDarkTheme.Get( this ); return _materialPreviewEnvironmentDarkTheme.value; }
			set { if( _materialPreviewEnvironmentDarkTheme.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentDarkThemeChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentDarkTheme.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentDarkTheme"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> MaterialPreviewEnvironmentDarkThemeChanged;
		ReferenceField<ImageComponent> _materialPreviewEnvironmentDarkTheme = new Reference<ImageComponent>( null, @"Content\Environments\Base\Forest.image" );

		public Reference<ImageComponent> GetMaterialPreviewEnvironment()
		{
			if( EditorAPI.DarkTheme )
				return MaterialPreviewEnvironmentDarkTheme;
			else
				return MaterialPreviewEnvironmentLightTheme;
		}

		/// <summary>
		/// The color multiplier to preview material.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		[Category( "Preview" )]
		public Reference<ColorValuePowered> MaterialPreviewEnvironmentMultiplier
		{
			get { if( _materialPreviewEnvironmentMultiplier.BeginGet() ) MaterialPreviewEnvironmentMultiplier = _materialPreviewEnvironmentMultiplier.Get( this ); return _materialPreviewEnvironmentMultiplier.value; }
			set { if( _materialPreviewEnvironmentMultiplier.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentMultiplierChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentMultiplier"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> MaterialPreviewEnvironmentMultiplierChanged;
		ReferenceField<ColorValuePowered> _materialPreviewEnvironmentMultiplier = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// The factor of lighting for preview material.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Preview" )]
		public Reference<double> MaterialPreviewEnvironmentAffectLighting
		{
			get { if( _materialPreviewEnvironmentAffectLighting.BeginGet() ) MaterialPreviewEnvironmentAffectLighting = _materialPreviewEnvironmentAffectLighting.Get( this ); return _materialPreviewEnvironmentAffectLighting.value; }
			set { if( _materialPreviewEnvironmentAffectLighting.BeginSet( ref value ) ) { try { MaterialPreviewEnvironmentAffectLightingChanged?.Invoke( this ); } finally { _materialPreviewEnvironmentAffectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialPreviewEnvironmentAffectLighting"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Preview> MaterialPreviewEnvironmentAffectLightingChanged;
		ReferenceField<double> _materialPreviewEnvironmentAffectLighting = 0.5;
	}
}
