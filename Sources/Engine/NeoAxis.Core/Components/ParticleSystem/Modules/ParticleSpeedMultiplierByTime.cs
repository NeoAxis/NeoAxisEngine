// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The base class of the module for changing the speed multiplier of a particle.
	/// </summary>
	public abstract class ParticleSpeedMultiplierByTime : ParticleModule
	{
		/// <summary>
		/// The method for calculating the value.
		/// </summary>
		[DefaultValue( TypeEnum.Constant )]
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set { if( _type.BeginSet( this, ref value ) ) { try { TypeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _type.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<ParticleSpeedMultiplierByTime> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.Constant;

		/// <summary>
		/// Constant value over all time.
		/// </summary>
		[DefaultValue( 0.5f )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<float> Constant
		{
			get { if( _constant.BeginGet() ) Constant = _constant.Get( this ); return _constant.value; }
			set { if( _constant.BeginSet( this, ref value ) ) { try { ConstantChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _constant.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Constant"/> property value changes.</summary>
		public event Action<ParticleSpeedMultiplierByTime> ConstantChanged;
		ReferenceField<float> _constant = 0.5f;

		/// <summary>
		/// Interval value over all time.
		/// </summary>
		[DefaultValue( "0 1" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<RangeF> Range
		{
			get { if( _range.BeginGet() ) Range = _range.Get( this ); return _range.value; }
			set { if( _range.BeginSet( this, ref value ) ) { try { RangeChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } finally { _range.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Range"/> property value changes.</summary>
		public event Action<ParticleSpeedMultiplierByTime> RangeChanged;
		ReferenceField<RangeF> _range = new RangeF( 0, 1 );

		/// <summary>
		/// A value specified by the curve over time.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public ReferenceList<CurvePoint1F> Curve
		{
			get { return _curve; }
		}
		public delegate void CurveChangedDelegate( ParticleSpeedMultiplierByTime sender );
		public event CurveChangedDelegate CurveChanged;
		ReferenceList<CurvePoint1F> _curve;

		/////////////////////////////////////////

		public enum TypeEnum
		{
			Constant,
			Range,
			Curve,
		}

		/////////////////////////////////////////

		public ParticleSpeedMultiplierByTime()
		{
			_curve = new ReferenceList<CurvePoint1F>( this, delegate () { CurveChanged?.Invoke( this ); ShouldRecompileParticleSystem(); } );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Constant ):
					if( Type != TypeEnum.Constant )
						skip = true;
					break;

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

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies a linear speed multiplier of the particles depending on their lifetime.
	/// </summary>
	[NewObjectDefaultName( "Linear Speed Multiplier By Time" )]
	public class ParticleLinearSpeedMultiplierByTime : ParticleSpeedMultiplierByTime
	{
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Specifies an angular speed multiplier of the particles depending on their lifetime.
	/// </summary>
	[NewObjectDefaultName( "Angular Speed Multiplier By Time" )]
	public class ParticleAngularSpeedMultiplierByTime : ParticleSpeedMultiplierByTime
	{
	}
}
