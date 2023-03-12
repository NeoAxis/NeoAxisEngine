// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Colors page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_Colors : ProjectSettingsPage
	{
		/// <summary>
		/// The color of selected object in the scene view.
		/// </summary>
		[DefaultValue( "0 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SelectedColor
		{
			get
			{
				if( _selectedColor.BeginGet() )
					SelectedColor = _selectedColor.Get( this );
				return _selectedColor.value;
			}
			set
			{
				if( _selectedColor.BeginSet( ref value ) )
				{
					try { SelectedColorChanged?.Invoke( this ); }
					finally { _selectedColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SelectedColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SelectedColorChanged;
		ReferenceField<ColorValue> _selectedColor = new ColorValue( 0, 1, 0 );


		//CanSelectColor
		ReferenceField<ColorValue> _canSelectColor = new ColorValue( 1, 1, 0 );
		/// <summary>
		/// The color of selectable object in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> CanSelectColor
		{
			get
			{
				if( _canSelectColor.BeginGet() )
					CanSelectColor = _canSelectColor.Get( this );
				return _canSelectColor.value;
			}
			set
			{
				if( _canSelectColor.BeginSet( ref value ) )
				{
					try { CanSelectColorChanged?.Invoke( this ); }
					finally { _canSelectColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CanSelectColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> CanSelectColorChanged;


		//!!!!надо каждой настройке такое. тогда их парами указывать
		//!!!!может просто "double AlphaMultiplier"
		//HiddenByOtherObjectsColorMultiplier
		ReferenceField<ColorValue> _hiddenByOtherObjectsColorMultiplier = new ColorValue( 1, 1, 1, .3 );
		/// <summary>
		/// The color multiplier applied when object is hidden by the other objects.
		/// </summary>
		[DefaultValue( "1 1 1 0.3" )]
		[Category( "Colors" )]
		public Reference<ColorValue> HiddenByOtherObjectsColorMultiplier
		{
			get
			{
				if( _hiddenByOtherObjectsColorMultiplier.BeginGet() )
					HiddenByOtherObjectsColorMultiplier = _hiddenByOtherObjectsColorMultiplier.Get( this );
				return _hiddenByOtherObjectsColorMultiplier.value;
			}
			set
			{
				if( _hiddenByOtherObjectsColorMultiplier.BeginSet( ref value ) )
				{
					try { HiddenByOtherObjectsColorMultiplierChanged?.Invoke( this ); }
					finally { _hiddenByOtherObjectsColorMultiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="HiddenByOtherObjectsColorMultiplier"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> HiddenByOtherObjectsColorMultiplierChanged;


		//!!!!name: может без "Draw" или без "Debug", хотя Debug - это секция. везде так
		//SceneShowLightColor
		ReferenceField<ColorValue> _sceneShowLightColor = new ColorValue( 1, 1, 0, .8 );
		/// <summary>
		/// The color of light source objects in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0 0.8" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowLightColor
		{
			get
			{
				if( _sceneShowLightColor.BeginGet() )
					SceneShowLightColor = _sceneShowLightColor.Get( this );
				return _sceneShowLightColor.value;
			}
			set
			{
				if( _sceneShowLightColor.BeginSet( ref value ) )
				{
					try { SceneShowLightColorChanged?.Invoke( this ); }
					finally { _sceneShowLightColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowLightColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowLightColorChanged;

		/// <summary>
		/// The color of decals in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowDecalColor
		{
			get { if( _sceneShowDecalColor.BeginGet() ) SceneShowDecalColor = _sceneShowDecalColor.Get( this ); return _sceneShowDecalColor.value; }
			set { if( _sceneShowDecalColor.BeginSet( ref value ) ) { try { SceneShowDecalColorChanged?.Invoke( this ); } finally { _sceneShowDecalColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowDecalColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowDecalColorChanged;
		ReferenceField<ColorValue> _sceneShowDecalColor = new ColorValue( 1, 1, 0 );

		//SceneShowPhysicsStaticColor
		ReferenceField<ColorValue> _sceneShowPhysicsStaticColor = new ColorValue( 0, 0, .4 );
		/// <summary>
		/// The color of static physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 0.4" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsStaticColor
		{
			get
			{
				if( _sceneShowPhysicsStaticColor.BeginGet() )
					SceneShowPhysicsStaticColor = _sceneShowPhysicsStaticColor.Get( this );
				return _sceneShowPhysicsStaticColor.value;
			}
			set
			{
				if( _sceneShowPhysicsStaticColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsStaticColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsStaticColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsStaticColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowPhysicsStaticColorChanged;


		//SceneShowPhysicsDynamicActiveColor
		ReferenceField<ColorValue> _sceneShowPhysicsDynamicActiveColor = new ColorValue( 0, 0, 1 );
		/// <summary>
		/// The color of active dynamic physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsDynamicActiveColor
		{
			get
			{
				if( _sceneShowPhysicsDynamicActiveColor.BeginGet() )
					SceneShowPhysicsDynamicActiveColor = _sceneShowPhysicsDynamicActiveColor.Get( this );
				return _sceneShowPhysicsDynamicActiveColor.value;
			}
			set
			{
				if( _sceneShowPhysicsDynamicActiveColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsDynamicActiveColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsDynamicActiveColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsDynamicActiveColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowPhysicsDynamicActiveColorChanged;


		//SceneShowPhysicsDynamicInactiveColor
		ReferenceField<ColorValue> _sceneShowPhysicsDynamicInactiveColor = new ColorValue( 0, 0, .6 );
		/// <summary>
		/// The color of inactive dynamic physics objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 0.6" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowPhysicsDynamicInactiveColor
		{
			get
			{
				if( _sceneShowPhysicsDynamicInactiveColor.BeginGet() )
					SceneShowPhysicsDynamicInactiveColor = _sceneShowPhysicsDynamicInactiveColor.Get( this );
				return _sceneShowPhysicsDynamicInactiveColor.value;
			}
			set
			{
				if( _sceneShowPhysicsDynamicInactiveColor.BeginSet( ref value ) )
				{
					try { SceneShowPhysicsDynamicInactiveColorChanged?.Invoke( this ); }
					finally { _sceneShowPhysicsDynamicInactiveColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowPhysicsDynamicInactiveColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowPhysicsDynamicInactiveColorChanged;

		/// <summary>
		/// The color of areas in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowAreaColor
		{
			get { if( _sceneShowAreaColor.BeginGet() ) SceneShowAreaColor = _sceneShowAreaColor.Get( this ); return _sceneShowAreaColor.value; }
			set { if( _sceneShowAreaColor.BeginSet( ref value ) ) { try { SceneShowAreaColorChanged?.Invoke( this ); } finally { _sceneShowAreaColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowAreaColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowAreaColorChanged;
		ReferenceField<ColorValue> _sceneShowAreaColor = new ColorValue( 0, 0, 1 );

		/// <summary>
		/// The color of volumes in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowVolumeColor
		{
			get { if( _sceneShowVolumeColor.BeginGet() ) SceneShowVolumeColor = _sceneShowVolumeColor.Get( this ); return _sceneShowVolumeColor.value; }
			set { if( _sceneShowVolumeColor.BeginSet( ref value ) ) { try { SceneShowVolumeColorChanged?.Invoke( this ); } finally { _sceneShowVolumeColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowVolumeColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowVolumeColorChanged;
		ReferenceField<ColorValue> _sceneShowVolumeColor = new ColorValue( 0, 0, 1 );

		/// <summary>
		/// The color of sound source objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowSoundSourceColor
		{
			get { if( _sceneShowSoundSourceColor.BeginGet() ) SceneShowSoundSourceColor = _sceneShowSoundSourceColor.Get( this ); return _sceneShowSoundSourceColor.value; }
			set { if( _sceneShowSoundSourceColor.BeginSet( ref value ) ) { try { SceneShowSoundSourceColorChanged?.Invoke( this ); } finally { _sceneShowSoundSourceColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SceneShowSoundSourceColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowSoundSourceColorChanged;
		ReferenceField<ColorValue> _sceneShowSoundSourceColor = new ColorValue( 0, 0, 1 );

		//SceneShowObjectInSpaceBoundsColor
		ReferenceField<ColorValue> _sceneShowObjectInSpaceBoundsColor = new ColorValue( 1, 1, 0, .8 );
		/// <summary>
		/// The color of abstract objects in the scene view.
		/// </summary>
		[DefaultValue( "1 1 0 0.8" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowObjectInSpaceBoundsColor
		{
			get
			{
				if( _sceneShowObjectInSpaceBoundsColor.BeginGet() )
					SceneShowObjectInSpaceBoundsColor = _sceneShowObjectInSpaceBoundsColor.Get( this );
				return _sceneShowObjectInSpaceBoundsColor.value;
			}
			set
			{
				if( _sceneShowObjectInSpaceBoundsColor.BeginSet( ref value ) )
				{
					try { SceneShowObjectInSpaceBoundsColorChanged?.Invoke( this ); }
					finally { _sceneShowObjectInSpaceBoundsColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowObjectInSpaceBoundsColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowObjectInSpaceBoundsColorChanged;


		//SceneShowReflectionProbeColor
		ReferenceField<ColorValue> _sceneShowReflectionProbeColor = new ColorValue( 0, 0, 1 );
		/// <summary>
		/// The color of reflection probe objects in the scene view.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		[Category( "Colors" )]
		public Reference<ColorValue> SceneShowReflectionProbeColor
		{
			get
			{
				if( _sceneShowReflectionProbeColor.BeginGet() )
					SceneShowReflectionProbeColor = _sceneShowReflectionProbeColor.Get( this );
				return _sceneShowReflectionProbeColor.value;
			}
			set
			{
				if( _sceneShowReflectionProbeColor.BeginSet( ref value ) )
				{
					try { SceneShowReflectionProbeColorChanged?.Invoke( this ); }
					finally { _sceneShowReflectionProbeColor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="SceneShowReflectionProbeColor"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Colors> SceneShowReflectionProbeColorChanged;
	}
}
