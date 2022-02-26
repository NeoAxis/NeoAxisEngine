// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Scene Editor page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_SceneEditor : ProjectSettingsPage
	{
		/// <summary>
		/// The snap value applied when object is moved.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Movement" )]
		public Reference<double> SceneEditorStepMovement
		{
			get { if( _sceneEditorStepMovement.BeginGet() ) SceneEditorStepMovement = _sceneEditorStepMovement.Get( this ); return _sceneEditorStepMovement.value; }
			set { if( _sceneEditorStepMovement.BeginSet( ref value ) ) { try { SceneEditorStepMovementChanged?.Invoke( this ); } finally { _sceneEditorStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepMovement"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorStepMovementChanged;
		ReferenceField<double> _sceneEditorStepMovement = 1.0;

		/// <summary>
		/// The snap value applied when object is rotated.
		/// </summary>
		[DefaultValue( "10" )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Rotation" )]
		public Reference<Degree> SceneEditorStepRotation
		{
			get { if( _sceneEditorStepRotation.BeginGet() ) SceneEditorStepRotation = _sceneEditorStepRotation.Get( this ); return _sceneEditorStepRotation.value; }
			set { if( _sceneEditorStepRotation.BeginSet( ref value ) ) { try { SceneEditorStepRotationChanged?.Invoke( this ); } finally { _sceneEditorStepRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepRotation"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorStepRotationChanged;
		ReferenceField<Degree> _sceneEditorStepRotation = new Degree( 10 );

		/// <summary>
		/// The snap value applied when object is scaled.
		/// </summary>
		[DefaultValue( 0.1 )]
		[Category( "Scene Editor: Snapping" )]
		[DisplayName( "Step Scaling" )]
		public Reference<double> SceneEditorStepScaling
		{
			get { if( _sceneEditorStepScaling.BeginGet() ) SceneEditorStepScaling = _sceneEditorStepScaling.Get( this ); return _sceneEditorStepScaling.value; }
			set { if( _sceneEditorStepScaling.BeginSet( ref value ) ) { try { SceneEditorStepScalingChanged?.Invoke( this ); } finally { _sceneEditorStepScaling.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorStepScaling"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorStepScalingChanged;
		ReferenceField<double> _sceneEditorStepScaling = 0.1;

		/////////////////////////////////////////

		//TransformToolRotationSensitivity
		ReferenceField<double> _transformToolRotationSensitivity = 1.0;
		/// <summary>
		/// The sensitivity of the object rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Rotation Sensitivity" )]
		[Range( 0.1, 10.0 )]
		public Reference<double> TransformToolRotationSensitivity
		{
			get { if( _transformToolRotationSensitivity.BeginGet() ) TransformToolRotationSensitivity = _transformToolRotationSensitivity.Get( this ); return _transformToolRotationSensitivity.value; }
			set { if( _transformToolRotationSensitivity.BeginSet( ref value ) ) { try { TransformToolRotationSensitivityChanged?.Invoke( this ); } finally { _transformToolRotationSensitivity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolRotationSensitivity"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> TransformToolRotationSensitivityChanged;

		//TransformToolSize
		ReferenceField<int> _transformToolSize = 125;
		/// <summary>
		/// The size of the transform tool.
		/// </summary>
		[DefaultValue( 125 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Size" )]
		[Range( 50, 300 )]
		public Reference<int> TransformToolSize
		{
			get { if( _transformToolSize.BeginGet() ) TransformToolSize = _transformToolSize.Get( this ); return _transformToolSize.value; }
			set { if( _transformToolSize.BeginSet( ref value ) ) { try { TransformToolSizeChanged?.Invoke( this ); } finally { _transformToolSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolSize"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> TransformToolSizeChanged;

		[Browsable( false )]
		public double TransformToolSizeScaled
		{
			get
			{
				var result = (double)TransformToolSize;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						result *= DpiHelper.Default.DpiScaleFactor;
					}
					catch { }
				}
				return result;
			}
		}

		//TransformToolLineThickness
		ReferenceField<double> _transformToolLineThickness = 2.0;
		/// <summary>
		/// The thickness of the transform tool line.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Line Thickness" )]
		[Range( 1.0, 5.0 )]
		public Reference<double> TransformToolLineThickness
		{
			get { if( _transformToolLineThickness.BeginGet() ) TransformToolLineThickness = _transformToolLineThickness.Get( this ); return _transformToolLineThickness.value; }
			set { if( _transformToolLineThickness.BeginSet( ref value ) ) { try { TransformToolLineThicknessChanged?.Invoke( this ); } finally { _transformToolLineThickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolLineThickness"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> TransformToolLineThicknessChanged;

		[Browsable( false )]
		public double TransformToolLineThicknessScaled
		{
			get
			{
				var result = (double)TransformToolLineThickness;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					try
					{
						result *= DpiHelper.Default.DpiScaleFactor;
					}
					catch { }
				}
				return result;
			}
		}

		//TransformToolShadowIntensity
		ReferenceField<double> _transformToolShadowIntensity = 0.3;
		/// <summary>
		/// The intensity of the shadows drawn by the transform tool.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Category( "Scene Editor: Transform Tool" )]
		[DisplayName( "Shadow Intensity" )]
		[Range( 0, 1 )]
		public Reference<double> TransformToolShadowIntensity
		{
			get { if( _transformToolShadowIntensity.BeginGet() ) TransformToolShadowIntensity = _transformToolShadowIntensity.Get( this ); return _transformToolShadowIntensity.value; }
			set { if( _transformToolShadowIntensity.BeginSet( ref value ) ) { try { TransformToolShadowIntensityChanged?.Invoke( this ); } finally { _transformToolShadowIntensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransformToolShadowIntensity"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> TransformToolShadowIntensityChanged;

		/////////////////////////////////////////

		//CameraKeyboardMovementSpeedNormal
		ReferenceField<double> _cameraKeyboardMovementSpeedNormal = 5.0;
		/// <summary>
		/// The normal keyboard speed of the camera movement.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Movement Speed Normal" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraKeyboardMovementSpeedNormal
		{
			get { if( _cameraKeyboardMovementSpeedNormal.BeginGet() ) CameraKeyboardMovementSpeedNormal = _cameraKeyboardMovementSpeedNormal.Get( this ); return _cameraKeyboardMovementSpeedNormal.value; }
			set { if( _cameraKeyboardMovementSpeedNormal.BeginSet( ref value ) ) { try { CameraKeyboardMovementSpeedNormalChanged?.Invoke( this ); } finally { _cameraKeyboardMovementSpeedNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardMovementSpeedNormal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraKeyboardMovementSpeedNormalChanged;

		//CameraKeyboardMovementSpeedFast
		ReferenceField<double> _cameraKeyboardMovementSpeedFast = 40.0;
		/// <summary>
		/// The keyboard speed of the camera movement in fast mode. Hold Shift key to turn the fast camera mode.
		/// </summary>
		[DefaultValue( 40.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Movement Speed Fast" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraKeyboardMovementSpeedFast
		{
			get { if( _cameraKeyboardMovementSpeedFast.BeginGet() ) CameraKeyboardMovementSpeedFast = _cameraKeyboardMovementSpeedFast.Get( this ); return _cameraKeyboardMovementSpeedFast.value; }
			set { if( _cameraKeyboardMovementSpeedFast.BeginSet( ref value ) ) { try { CameraKeyboardMovementSpeedFastChanged?.Invoke( this ); } finally { _cameraKeyboardMovementSpeedFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardMovementSpeedFast"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraKeyboardMovementSpeedFastChanged;

		//CameraKeyboardRotationSpeedNormal
		ReferenceField<Degree> _cameraKeyboardRotationSpeedNormal = new Degree( 30 );
		/// <summary>
		/// The normal keyboard speed of the camera rotation.
		/// </summary>
		[DefaultValue( "30" )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Rotation Speed Normal" )]
		[Range( 1, 360, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Degree> CameraKeyboardRotationSpeedNormal
		{
			get { if( _cameraKeyboardRotationSpeedNormal.BeginGet() ) CameraKeyboardRotationSpeedNormal = _cameraKeyboardRotationSpeedNormal.Get( this ); return _cameraKeyboardRotationSpeedNormal.value; }
			set { if( _cameraKeyboardRotationSpeedNormal.BeginSet( ref value ) ) { try { CameraKeyboardRotationSpeedNormalChanged?.Invoke( this ); } finally { _cameraKeyboardRotationSpeedNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardRotationSpeedNormal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraKeyboardRotationSpeedNormalChanged;

		//CameraKeyboardRotationSpeedFast
		ReferenceField<Degree> _cameraKeyboardRotationSpeedFast = new Degree( 90 );
		/// <summary>
		/// The keyboard speed of the camera rotation in fast mode.
		/// </summary>
		[DefaultValue( "90" )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Keyboard Rotation Speed Fast" )]
		[Range( 1, 360, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<Degree> CameraKeyboardRotationSpeedFast
		{
			get { if( _cameraKeyboardRotationSpeedFast.BeginGet() ) CameraKeyboardRotationSpeedFast = _cameraKeyboardRotationSpeedFast.Get( this ); return _cameraKeyboardRotationSpeedFast.value; }
			set { if( _cameraKeyboardRotationSpeedFast.BeginSet( ref value ) ) { try { CameraKeyboardRotationSpeedFastChanged?.Invoke( this ); } finally { _cameraKeyboardRotationSpeedFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraKeyboardRotationSpeedFast"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraKeyboardRotationSpeedFastChanged;

		//CameraMouseMovementSensitivityNormal
		ReferenceField<double> _cameraMouseMovementSensitivityNormal = 1.0;
		/// <summary>
		/// The normal mouse sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Movement Sensitivity Normal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseMovementSensitivityNormal
		{
			get { if( _cameraMouseMovementSensitivityNormal.BeginGet() ) CameraMouseMovementSensitivityNormal = _cameraMouseMovementSensitivityNormal.Get( this ); return _cameraMouseMovementSensitivityNormal.value; }
			set { if( _cameraMouseMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseMovementSensitivityNormalChanged;

		//CameraMouseMovementSensitivityFast
		ReferenceField<double> _cameraMouseMovementSensitivityFast = 5.0;
		/// <summary>
		/// The mouse sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Movement Sensitivity Fast" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseMovementSensitivityFast
		{
			get { if( _cameraMouseMovementSensitivityFast.BeginGet() ) CameraMouseMovementSensitivityFast = _cameraMouseMovementSensitivityFast.Get( this ); return _cameraMouseMovementSensitivityFast.value; }
			set { if( _cameraMouseMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseMovementSensitivityFast"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseMovementSensitivityFastChanged;

		//CameraMouseRotationSensitivityHorizontal
		ReferenceField<double> _cameraMouseRotationSensitivityHorizontal = 1.0;
		/// <summary>
		/// The horizontal mouse sensitivity of the camera rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Rotation Sensitivity Horizontal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseRotationSensitivityHorizontal
		{
			get { if( _cameraMouseRotationSensitivityHorizontal.BeginGet() ) CameraMouseRotationSensitivityHorizontal = _cameraMouseRotationSensitivityHorizontal.Get( this ); return _cameraMouseRotationSensitivityHorizontal.value; }
			set { if( _cameraMouseRotationSensitivityHorizontal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseRotationSensitivityHorizontal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseRotationSensitivityHorizontalChanged;

		////CameraMouseRotationSensitivityHorizontalNormal
		//ReferenceField<double> _cameraMouseRotationSensitivityHorizontalNormal = 1.0;
		//[DefaultValue( 1.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Horizontal Normal" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityHorizontalNormal
		//{
		//	get { if( _cameraMouseRotationSensitivityHorizontalNormal.BeginGet() ) CameraMouseRotationSensitivityHorizontalNormal = _cameraMouseRotationSensitivityHorizontalNormal.Get( this ); return _cameraMouseRotationSensitivityHorizontalNormal.value; }
		//	set { if( _cameraMouseRotationSensitivityHorizontalNormal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalNormalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontalNormal.EndSet(); } } }
		//}
		//public event Action<ProjectSettingsComponentPage_SceneEditor> CameraMouseRotationSensitivityHorizontalNormalChanged;

		////CameraMouseRotationSensitivityHorizontalFast
		//ReferenceField<double> _cameraMouseRotationSensitivityHorizontalFast = 3.0;
		//[DefaultValue( 3.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Horizontal Fast" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityHorizontalFast
		//{
		//	get { if( _cameraMouseRotationSensitivityHorizontalFast.BeginGet() ) CameraMouseRotationSensitivityHorizontalFast = _cameraMouseRotationSensitivityHorizontalFast.Get( this ); return _cameraMouseRotationSensitivityHorizontalFast.value; }
		//	set { if( _cameraMouseRotationSensitivityHorizontalFast.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityHorizontalFastChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityHorizontalFast.EndSet(); } } }
		//}
		//public event Action<ProjectSettingsComponentPage_SceneEditor> CameraMouseRotationSensitivityHorizontalFastChanged;

		//CameraMouseRotationSensitivityVertical
		ReferenceField<double> _cameraMouseRotationSensitivityVertical = 1.0;
		/// <summary>
		/// The vertical mouse sensitivity of the camera rotation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Rotation Sensitivity Vertical" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseRotationSensitivityVertical
		{
			get { if( _cameraMouseRotationSensitivityVertical.BeginGet() ) CameraMouseRotationSensitivityVertical = _cameraMouseRotationSensitivityVertical.Get( this ); return _cameraMouseRotationSensitivityVertical.value; }
			set { if( _cameraMouseRotationSensitivityVertical.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVertical.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseRotationSensitivityVertical"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseRotationSensitivityVerticalChanged;

		////CameraMouseRotationSensitivityVerticalNormal
		//ReferenceField<double> _cameraMouseRotationSensitivityVerticalNormal = 1.0;
		//[DefaultValue( 1.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Vertical Normal" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityVerticalNormal
		//{
		//	get { if( _cameraMouseRotationSensitivityVerticalNormal.BeginGet() ) CameraMouseRotationSensitivityVerticalNormal = _cameraMouseRotationSensitivityVerticalNormal.Get( this ); return _cameraMouseRotationSensitivityVerticalNormal.value; }
		//	set { if( _cameraMouseRotationSensitivityVerticalNormal.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalNormalChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVerticalNormal.EndSet(); } } }
		//}
		//public event Action<ProjectSettingsComponentPage_SceneEditor> CameraMouseRotationSensitivityVerticalNormalChanged;

		////CameraMouseRotationSensitivityVerticalFast
		//ReferenceField<double> _cameraMouseRotationSensitivityVerticalFast = 3.0;
		//[DefaultValue( 3.0 )]
		//[Category( "Camera" )]
		//[DisplayName( "Mouse Rotation Sensitivity Vertical Fast" )]
		//[ApplicableRange( 0.1, 10, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> CameraMouseRotationSensitivityVerticalFast
		//{
		//	get { if( _cameraMouseRotationSensitivityVerticalFast.BeginGet() ) CameraMouseRotationSensitivityVerticalFast = _cameraMouseRotationSensitivityVerticalFast.Get( this ); return _cameraMouseRotationSensitivityVerticalFast.value; }
		//	set { if( _cameraMouseRotationSensitivityVerticalFast.BeginSet( ref value ) ) { try { CameraMouseRotationSensitivityVerticalFastChanged?.Invoke( this ); } finally { _cameraMouseRotationSensitivityVerticalFast.EndSet(); } } }
		//}
		//public event Action<ProjectSettingsComponentPage_SceneEditor> CameraMouseRotationSensitivityVerticalFastChanged;

		//CameraMouseTrackMovementSensitivityNormal
		ReferenceField<double> _cameraMouseTrackMovementSensitivityNormal = 5.0;
		/// <summary>
		/// The normal mouse sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Track Movement Sensitivity Normal" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseTrackMovementSensitivityNormal
		{
			get { if( _cameraMouseTrackMovementSensitivityNormal.BeginGet() ) CameraMouseTrackMovementSensitivityNormal = _cameraMouseTrackMovementSensitivityNormal.Get( this ); return _cameraMouseTrackMovementSensitivityNormal.value; }
			set { if( _cameraMouseTrackMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseTrackMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseTrackMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseTrackMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseTrackMovementSensitivityNormalChanged;

		/// <summary>
		/// The mouse sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 15.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Track Movement Sensitivity Fast" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseTrackMovementSensitivityFast
		{
			get { if( _cameraMouseTrackMovementSensitivityFast.BeginGet() ) CameraMouseTrackMovementSensitivityFast = _cameraMouseTrackMovementSensitivityFast.Get( this ); return _cameraMouseTrackMovementSensitivityFast.value; }
			set { if( _cameraMouseTrackMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseTrackMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseTrackMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseTrackMovementSensitivityFast"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseTrackMovementSensitivityFastChanged;
		ReferenceField<double> _cameraMouseTrackMovementSensitivityFast = 15.0;

		/// <summary>
		/// The normal mouse wheel sensitivity of the camera movement.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Wheel Movement Sensitivity Normal" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseWheelMovementSensitivityNormal
		{
			get { if( _cameraMouseWheelMovementSensitivityNormal.BeginGet() ) CameraMouseWheelMovementSensitivityNormal = _cameraMouseWheelMovementSensitivityNormal.Get( this ); return _cameraMouseWheelMovementSensitivityNormal.value; }
			set { if( _cameraMouseWheelMovementSensitivityNormal.BeginSet( ref value ) ) { try { CameraMouseWheelMovementSensitivityNormalChanged?.Invoke( this ); } finally { _cameraMouseWheelMovementSensitivityNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseWheelMovementSensitivityNormal"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseWheelMovementSensitivityNormalChanged;
		ReferenceField<double> _cameraMouseWheelMovementSensitivityNormal = 1.0;

		/// <summary>
		/// The mouse wheel sensitivity of the camera movement in fast mode.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Category( "Scene Editor: Camera" )]
		[DisplayName( "Mouse Wheel Movement Sensitivity Fast" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> CameraMouseWheelMovementSensitivityFast
		{
			get { if( _cameraMouseWheelMovementSensitivityFast.BeginGet() ) CameraMouseWheelMovementSensitivityFast = _cameraMouseWheelMovementSensitivityFast.Get( this ); return _cameraMouseWheelMovementSensitivityFast.value; }
			set { if( _cameraMouseWheelMovementSensitivityFast.BeginSet( ref value ) ) { try { CameraMouseWheelMovementSensitivityFastChanged?.Invoke( this ); } finally { _cameraMouseWheelMovementSensitivityFast.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraMouseWheelMovementSensitivityFast"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> CameraMouseWheelMovementSensitivityFastChanged;
		ReferenceField<double> _cameraMouseWheelMovementSensitivityFast = 5.0;

		/////////////////////////////////////////

		/// <summary>
		/// The radius of the selection of objects with a double click.
		/// </summary>
		[DefaultValue( 20.0 )]
		[Category( "Scene Editor: Select" )]
		[DisplayName( "Select By Double Click Radius" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> SceneEditorSelectByDoubleClickRadius
		{
			get { if( _sceneEditorSelectByDoubleClickRadius.BeginGet() ) SceneEditorSelectByDoubleClickRadius = _sceneEditorSelectByDoubleClickRadius.Get( this ); return _sceneEditorSelectByDoubleClickRadius.value; }
			set { if( _sceneEditorSelectByDoubleClickRadius.BeginSet( ref value ) ) { try { SceneEditorSelectByDoubleClickRadiusChanged?.Invoke( this ); } finally { _sceneEditorSelectByDoubleClickRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorSelectByDoubleClickRadius"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorSelectByDoubleClickRadiusChanged;
		ReferenceField<double> _sceneEditorSelectByDoubleClickRadius = 20.0;

		/// <summary>
		/// Whether to allow using outline effect for selected objects.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Scene Editor: Select" )]
		[DisplayName( "Outline Effect Enabled" )]
		public Reference<bool> SceneEditorSelectOutlineEffectEnabled
		{
			get { if( _sceneEditorSelectOutlineEffectEnabled.BeginGet() ) SceneEditorSelectOutlineEffectEnabled = _sceneEditorSelectOutlineEffectEnabled.Get( this ); return _sceneEditorSelectOutlineEffectEnabled.value; }
			set { if( _sceneEditorSelectOutlineEffectEnabled.BeginSet( ref value ) ) { try { SceneEditorSelectOutlineEffectEnabledChanged?.Invoke( this ); } finally { _sceneEditorSelectOutlineEffectEnabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorSelectOutlineEffectEnabled"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorSelectOutlineEffectEnabledChanged;
		ReferenceField<bool> _sceneEditorSelectOutlineEffectEnabled = true;

		/// <summary>
		/// The size of the outline effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Scene Editor: Select" )]
		[DisplayName( "Outline Effect Scale" )]
		public Reference<double> SceneEditorSelectOutlineEffectScale
		{
			get { if( _sceneEditorSelectOutlineEffectScale.BeginGet() ) SceneEditorSelectOutlineEffectScale = _sceneEditorSelectOutlineEffectScale.Get( this ); return _sceneEditorSelectOutlineEffectScale.value; }
			set { if( _sceneEditorSelectOutlineEffectScale.BeginSet( ref value ) ) { try { SceneEditorSelectOutlineEffectScaleChanged?.Invoke( this ); } finally { _sceneEditorSelectOutlineEffectScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneEditorSelectOutlineEffectScale"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_SceneEditor> SceneEditorSelectOutlineEffectScaleChanged;
		ReferenceField<double> _sceneEditorSelectOutlineEffectScale = 1.0;
	}
}
