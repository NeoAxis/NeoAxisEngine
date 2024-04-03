// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The fog effect in the scene.
	/// </summary>
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Fog : Component
	{
		[Flags]
		public enum Modes
		{
			/// <summary>Fog density increases exponentially from the camera (fog = 1/e^(distance * density)).</summary>
			Exp = 1,
			/// <summary>Fog density increases at the square of <b>Exp</b>, i.e. even quicker (fog = 1/e^(distance * density)^2).</summary>
			Exp2 = 2,
			///// <summary>Fog density increases linearly between the start and end distances.</summary>
			//Linear,
			Height = 4,
		}

		/// <summary>
		/// The mode of the fog.
		/// </summary>
		[DefaultValue( Modes.Exp2 )]
		[Serialize]
		public Reference<Modes> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( this, ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Fog> ModeChanged;
		ReferenceField<Modes> _mode = Modes.Exp2;

		/// <summary>
		/// The color of the fog.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Fog> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// The distance at which the fog will show up.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> StartDistance
		{
			get { if( _startDistance.BeginGet() ) StartDistance = _startDistance.Get( this ); return _startDistance.value; }
			set { if( _startDistance.BeginSet( this, ref value ) ) { try { StartDistanceChanged?.Invoke( this ); } finally { _startDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StartDistance"/> property value changes.</summary>
		public event Action<Fog> StartDistanceChanged;
		ReferenceField<double> _startDistance = 0.0;

		/// <summary>
		/// The density of the fog.
		/// </summary>
		[DefaultValue( 0.003 )]
		[Serialize]
		[Range( 0.0003, 1, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Density
		{
			get { if( _density.BeginGet() ) Density = _density.Get( this ); return _density.value; }
			set { if( _density.BeginSet( this, ref value ) ) { try { DensityChanged?.Invoke( this ); } finally { _density.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Density"/> property value changes.</summary>
		public event Action<Fog> DensityChanged;
		ReferenceField<double> _density = 0.003;

		/// <summary>
		/// The height of the fog.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Serialize]
		[Range( -100, 100 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( this, ref value ) ) { try { HeightChanged?.Invoke( this ); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Fog> HeightChanged;
		ReferenceField<double> _height = 0.0;

		/// <summary>
		/// The scale of the fog.
		/// </summary>
		[DefaultValue( 100.0 )]
		[Serialize]
		[Range( 10, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> HeightScale
		{
			get { if( _heightScale.BeginGet() ) HeightScale = _heightScale.Get( this ); return _heightScale.value; }
			set { if( _heightScale.BeginSet( this, ref value ) ) { try { HeightScaleChanged?.Invoke( this ); } finally { _heightScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeightScale"/> property value changes.</summary>
		public event Action<Fog> HeightScaleChanged;
		ReferenceField<double> _heightScale = 100.0;

		/// <summary>
		/// Specifies how much fog affects sky or background color.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> AffectBackground
		{
			get { if( _affectBackground.BeginGet() ) AffectBackground = _affectBackground.Get( this ); return _affectBackground.value; }
			set { if( _affectBackground.BeginSet( this, ref value ) ) { try { AffectBackgroundChanged?.Invoke( this ); } finally { _affectBackground.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectBackground"/> property value changes.</summary>
		public event Action<Fog> AffectBackgroundChanged;
		ReferenceField<double> _affectBackground = 1.0;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( StartDistance ):
				case nameof( Density ):
					if( !Mode.Value.HasFlag( Modes.Exp ) && !Mode.Value.HasFlag( Modes.Exp2 ) )
						skip = true;
					break;

				case nameof( Height ):
				case nameof( HeightScale ):
					if( !Mode.Value.HasFlag( Modes.Height ) )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//rendering pipeline optimization
			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Add( this );
				else
					scene.CachedObjectsInSpaceToFastFindByRenderingPipeline.Remove( this );
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Fog", true );
		}
	}
}
