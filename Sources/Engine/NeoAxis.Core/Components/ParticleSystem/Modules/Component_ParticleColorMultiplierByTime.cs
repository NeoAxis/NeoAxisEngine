// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a color multiplier of the particles depending on their lifetime.
	/// </summary>
	[NewObjectDefaultName( "Color Multiplier By Time" )]
	public class Component_ParticleColorMultiplierByTime : Component_ParticleModule
	{
		/// <summary>
		/// Specifies which color channels the module affects.
		/// </summary>
		[DefaultValue( ChannelsEnum.All )]
		public Reference<ChannelsEnum> Channels
		{
			get { if( _channels.BeginGet() ) Channels = _channels.Get( this ); return _channels.value; }
			set { if( _channels.BeginSet( ref value ) ) { try { ChannelsChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _channels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Channels"/> property value changes.</summary>
		public event Action<Component_ParticleColorMultiplierByTime> ChannelsChanged;
		ReferenceField<ChannelsEnum> _channels = ChannelsEnum.All;

		/// <summary>
		/// The method for calculating the value.
		/// </summary>
		[DefaultValue( TypeEnum.Range )]
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set { if( _type.BeginSet( ref value ) ) { try { TypeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _type.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_ParticleColorMultiplierByTime> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.Range;

		/// <summary>
		/// Interval value over all time.
		/// </summary>
		[DefaultValue( "0 1" )]
		public Reference<RangeF> Range
		{
			get { if( _range.BeginGet() ) Range = _range.Get( this ); return _range.value; }
			set { if( _range.BeginSet( ref value ) ) { try { RangeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _range.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Range"/> property value changes.</summary>
		public event Action<Component_ParticleColorMultiplierByTime> RangeChanged;
		ReferenceField<RangeF> _range = new RangeF( 0, 1 );

		/// <summary>
		/// A value specified by the curve over time.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public ReferenceList<Component_ParticleSystem.CurvePoint> Curve
		{
			get { return _curve; }
		}
		public delegate void CurveChangedDelegate( Component_ParticleColorMultiplierByTime sender );
		public event CurveChangedDelegate CurveChanged;
		ReferenceList<Component_ParticleSystem.CurvePoint> _curve;

		/////////////////////////////////////////

		[Flags]
		public enum ChannelsEnum
		{
			Red = 1,
			Green = 2,
			Blue = 4,
			Alpha = 8,
			All = Red | Green | Blue | Alpha,
		}

		/////////////////////////////////////////

		public enum TypeEnum
		{
			Range,
			Curve,
		}

		/////////////////////////////////////////

		public Component_ParticleColorMultiplierByTime()
		{
			_curve = new ReferenceList<Component_ParticleSystem.CurvePoint>( this, delegate () { CurveChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Range ):
					if( Type != TypeEnum.Range )
						skip = true;
					break;

				case nameof( NeoAxis.Curve ):
					if( Type != TypeEnum.Curve )
						skip = true;
					break;
				}
			}
		}
	}
}
