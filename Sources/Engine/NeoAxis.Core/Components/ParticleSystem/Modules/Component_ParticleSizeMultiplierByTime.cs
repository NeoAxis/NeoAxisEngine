// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a size multiplier of the particles depending on their lifetime.
	/// </summary>
	[NewObjectDefaultName( "Size Multiplier By Time" )]
	public class Component_ParticleSizeMultiplierByTime : Component_ParticleModule
	{
		/// <summary>
		/// Specifies which size multiplier components the module affects.
		/// </summary>
		[DefaultValue( AxesEnum.All )]
		public Reference<AxesEnum> Axes
		{
			get { if( _axes.BeginGet() ) Axes = _axes.Get( this ); return _axes.value; }
			set { if( _axes.BeginSet( ref value ) ) { try { AxesChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _axes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Axes"/> property value changes.</summary>
		public event Action<Component_ParticleSizeMultiplierByTime> AxesChanged;
		ReferenceField<AxesEnum> _axes = AxesEnum.All;

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
		public event Action<Component_ParticleSizeMultiplierByTime> TypeChanged;
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
		public event Action<Component_ParticleSizeMultiplierByTime> RangeChanged;
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
		public delegate void CurveChangedDelegate( Component_ParticleSizeMultiplierByTime sender );
		public event CurveChangedDelegate CurveChanged;
		ReferenceList<Component_ParticleSystem.CurvePoint> _curve;

		/////////////////////////////////////////

		[Flags]
		public enum AxesEnum
		{
			X = 1,
			Y = 2,
			Z = 4,
			All = X | Y | Z,
		}

		/////////////////////////////////////////

		public enum TypeEnum
		{
			Range,
			Curve,
		}

		/////////////////////////////////////////

		public Component_ParticleSizeMultiplierByTime()
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

				case nameof( Curve ):
					if( Type != TypeEnum.Curve )
						skip = true;
					break;
				}
			}
		}
	}
}
