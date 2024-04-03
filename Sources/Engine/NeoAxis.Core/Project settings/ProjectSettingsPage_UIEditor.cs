// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a UI Editor page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_UIEditor : ProjectSettingsPage
	{
		/// <summary>
		/// The aspect ratio of the canvas of UI Editor.
		/// </summary>
		[Category( "UI Editor: General" )]
		[DisplayName( "Editor Aspect Ratio" )]
		[DefaultValue( 1.77777777777 )]
		public Reference<double> UIEditorAspectRatio
		{
			get { if( _uiEditorAspectRatio.BeginGet() ) UIEditorAspectRatio = _uiEditorAspectRatio.Get( this ); return _uiEditorAspectRatio.value; }
			set { if( _uiEditorAspectRatio.BeginSet( this, ref value ) ) { try { UIEditorAspectRatioChanged?.Invoke( this ); } finally { _uiEditorAspectRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorAspectRatio"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorAspectRatioChanged;
		ReferenceField<double> _uiEditorAspectRatio = 1.77777777777;

		/// <summary>
		/// Whether to display a grid.
		/// </summary>
		[Category( "UI Editor: General" )]
		[DisplayName( "Display Grid" )]
		[DefaultValue( true )]
		public Reference<bool> UIEditorDisplayGrid
		{
			get { if( _uIEditorDisplayGrid.BeginGet() ) UIEditorDisplayGrid = _uIEditorDisplayGrid.Get( this ); return _uIEditorDisplayGrid.value; }
			set { if( _uIEditorDisplayGrid.BeginSet( this, ref value ) ) { try { UIEditorDisplayGridChanged?.Invoke( this ); } finally { _uIEditorDisplayGrid.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorDisplayGrid"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorDisplayGridChanged;
		ReferenceField<bool> _uIEditorDisplayGrid = true;

		/////////////////////////////////////////

		/// <summary>
		/// The size of move step for Parent measure.
		/// </summary>
		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Parent Measure: Step Movement" )]
		[DefaultValue( 0.01 )]
		public Reference<double> UIEditorParentMeasureStepMovement
		{
			get { if( _uIEditorParentMeasureStepMovement.BeginGet() ) UIEditorParentMeasureStepMovement = _uIEditorParentMeasureStepMovement.Get( this ); return _uIEditorParentMeasureStepMovement.value; }
			set { if( _uIEditorParentMeasureStepMovement.BeginSet( this, ref value ) ) { try { UIEditorParentMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorParentMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorParentMeasureStepMovement"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorParentMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorParentMeasureStepMovement = 0.01;

		/// <summary>
		/// The size of move step for Units measure.
		/// </summary>
		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Units Measure: Step Movement" )]
		[DefaultValue( 10.0 )]
		public Reference<double> UIEditorUnitsMeasureStepMovement
		{
			get { if( _uIEditorUnitsMeasureStepMovement.BeginGet() ) UIEditorUnitsMeasureStepMovement = _uIEditorUnitsMeasureStepMovement.Get( this ); return _uIEditorUnitsMeasureStepMovement.value; }
			set { if( _uIEditorUnitsMeasureStepMovement.BeginSet( this, ref value ) ) { try { UIEditorUnitsMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorUnitsMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorUnitsMeasureStepMovement"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorUnitsMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorUnitsMeasureStepMovement = 10.0;

		/// <summary>
		/// The size of move step for Pixels measure.
		/// </summary>
		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Pixels Measure: Step Movement" )]
		[DefaultValue( 10.0 )]
		public Reference<double> UIEditorPixelsMeasureStepMovement
		{
			get { if( _uIEditorPixelsMeasureStepMovement.BeginGet() ) UIEditorPixelsMeasureStepMovement = _uIEditorPixelsMeasureStepMovement.Get( this ); return _uIEditorPixelsMeasureStepMovement.value; }
			set { if( _uIEditorPixelsMeasureStepMovement.BeginSet( this, ref value ) ) { try { UIEditorPixelsMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorPixelsMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorPixelsMeasureStepMovement"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorPixelsMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorPixelsMeasureStepMovement = 10.0;

		/// <summary>
		/// The size of move step for Screen measure.
		/// </summary>
		[Category( "UI Editor: Snapping" )]
		[DisplayName( "Screen Measure: Step Movement" )]
		[DefaultValue( 0.01 )]
		public Reference<double> UIEditorScreenMeasureStepMovement
		{
			get { if( _uIEditorScreenMeasureStepMovement.BeginGet() ) UIEditorScreenMeasureStepMovement = _uIEditorScreenMeasureStepMovement.Get( this ); return _uIEditorScreenMeasureStepMovement.value; }
			set { if( _uIEditorScreenMeasureStepMovement.BeginSet( this, ref value ) ) { try { UIEditorScreenMeasureStepMovementChanged?.Invoke( this ); } finally { _uIEditorScreenMeasureStepMovement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIEditorScreenMeasureStepMovement"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_UIEditor> UIEditorScreenMeasureStepMovementChanged;
		ReferenceField<double> _uIEditorScreenMeasureStepMovement = 0.01;

		public double GetUIEditorStepMovement( UIMeasure measure )
		{
			switch( measure )
			{
			case UIMeasure.Parent: return UIEditorParentMeasureStepMovement;
			case UIMeasure.Units: return UIEditorUnitsMeasureStepMovement;
			case UIMeasure.Pixels: return UIEditorPixelsMeasureStepMovement;
			case UIMeasure.PixelsScaled: return UIEditorPixelsMeasureStepMovement;//!!!!?
			case UIMeasure.Screen: return UIEditorScreenMeasureStepMovement;
			}
			return 0;
		}
	}
}
