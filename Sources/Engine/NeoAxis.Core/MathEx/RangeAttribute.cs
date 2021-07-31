// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;

namespace NeoAxis
{
	/// <summary>
	/// This attribute specifies the range of possible values of a property or field in the editor.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public class RangeAttribute : Attribute
	{
		double minimum;
		double maximum;
		//!!!!name: userFriendlyDistribution
		ConvenientDistributionEnum convenientDistribution;
		double exponentialPower;

		//

		public enum ConvenientDistributionEnum
		{
			Linear,
			Exponential,
		}

		//

		public RangeAttribute( double minimum, double maximum, ConvenientDistributionEnum convenientDistribution = ConvenientDistributionEnum.Linear, double exponentialPower = 2 )
		{
			this.minimum = minimum;
			this.maximum = maximum;
			this.convenientDistribution = convenientDistribution;
			this.exponentialPower = exponentialPower;
		}

		public double Minimum
		{
			get { return minimum; }
		}

		public double Maximum
		{
			get { return maximum; }
		}

		public ConvenientDistributionEnum ConvenientDistribution
		{
			get { return convenientDistribution; }
		}

		public double ExponentialPower
		{
			get { return exponentialPower; }
		}


		//!!!!тут?

		public void GetTrackBarMinMax( bool isInteger, out int trackBarMinimum, out int trackBarMaximum )
		{
			if( ConvenientDistribution == ConvenientDistributionEnum.Exponential )
			{
				trackBarMinimum = 0;
				trackBarMaximum = 1000;
			}
			else
			{
				if( isInteger )
				{
					trackBarMinimum = (int)Minimum;
					trackBarMaximum = (int)Maximum;
				}
				else
				{
					trackBarMinimum = (int)( Minimum * 1000 );
					trackBarMaximum = (int)( Maximum * 1000 );
				}
			}
		}

		public int GetTrackBarValue( bool isInteger, double value )
		{
			if( ConvenientDistribution == ConvenientDistributionEnum.Exponential )
			{
				double v = MathEx.Saturate( ( value - Minimum ) / ( Maximum - Minimum ) );
				v = Math.Pow( v, 1.0 / ExponentialPower );
				return MathEx.Clamp( (int)( v * 1000 ), 0, 1000 );
			}
			else
			{
				GetTrackBarMinMax( isInteger, out int min, out int max );
				if( isInteger )
					return MathEx.Clamp( (int)( value ), min, max );
				else
					return MathEx.Clamp( (int)( value * 1000 ), min, max );
			}
		}

		public double GetValueFromTrackBar( bool isInteger, int trackBarValue )
		{
			double value;

			if( ConvenientDistribution == ConvenientDistributionEnum.Exponential )
			{
				double v = (double)trackBarValue / 1000;
				v = Math.Pow( v, ExponentialPower );
				value = Minimum + v * ( Maximum - Minimum );
			}
			else
			{
				if( isInteger )
					value = trackBarValue;
				else
					value = (double)trackBarValue / 1000;
			}

			value = MathEx.Clamp( value, Minimum, Maximum );
			return value;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This attribute specifies the range of possible Power component of color values of a property or field in the editor.
	/// </summary>
	public class ApplicableRangeColorValuePowerAttribute : RangeAttribute
	{
		public ApplicableRangeColorValuePowerAttribute( double minimum, double maximum, ConvenientDistributionEnum convenientDistribution = ConvenientDistributionEnum.Linear, double exponentialPower = 2 )
			: base( minimum, maximum, convenientDistribution, exponentialPower )
		{
		}
	}
}
