// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Provides the functions for calculating parameters of Gaussian blur.
	/// </summary>
	public static class GaussianBlurMath
	{
		static double GaussianDistribution( double x, double y, double rho )
		{
			double g = 1.0 / Math.Sqrt( 2.0 * Math.PI * rho * rho );
			g *= Math.Exp( -( x * x + y * y ) / ( 2.0 * rho * rho ) );
			return g;
		}

		static Vector4F[] ConvertToVec4F( Vector2F[] source )
		{
			Vector4F[] result = new Vector4F[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = new Vector4F( source[ n ].X, source[ n ].Y, 0, 0 );
			return result;
		}

		/// <summary>
		/// Represents a result data for <see cref="GaussianBlurMath"/>.
		/// </summary>
		public class Result
		{
			public Vector2F[] SampleOffsets;
			public Vector4F[] SampleOffsetsAsVec4Array;
			public Vector4F[] SampleWeights;
		}

		public static Result Calculate15( Vector2I textureSize, bool horizontal, double blurFactor )
		{
			Vector2F[] sampleOffsets = new Vector2F[ 15 ];
			Vector4F[] sampleWeights = new Vector4F[ 15 ];

			// calculate gaussian texture offsets & weights
			float texelSize = 1.0f / (float)( horizontal ? textureSize.X : textureSize.Y );
			texelSize *= (float)blurFactor;

			// central sample, no offset
			sampleOffsets[ 0 ] = Vector2F.Zero;
			{
				float distribution = (float)GaussianDistribution( 0, 0, 3 );
				sampleWeights[ 0 ] = new Vector4F( distribution, distribution, distribution, 1 );
			}

			// 'pre' samples
			for( int n = 1; n < 8; n++ )
			{
				float distribution = (float)GaussianDistribution( n, 0, 3 );
				sampleWeights[ n ] = new Vector4F( distribution, distribution, distribution, 0 );

				if( horizontal )
					sampleOffsets[ n ] = new Vector2F( (float)n * texelSize, 0 );
				else
					sampleOffsets[ n ] = new Vector2F( 0, (float)n * texelSize );
			}

			// 'post' samples
			for( int n = 8; n < 15; n++ )
			{
				sampleWeights[ n ] = sampleWeights[ n - 7 ];
				sampleOffsets[ n ] = -sampleOffsets[ n - 7 ];
			}

			//normalize weights (fix)
			{
				float total = 0;
				foreach( var v in sampleWeights )
					total += v.X;
				for( int n = 0; n < sampleWeights.Length; n++ )
					sampleWeights[ n ] = new Vector4F( sampleWeights[ n ].ToVector3F() * ( 1.0f / total ), sampleWeights[ n ].W );

				//Log.Info( total.ToString() );

				//Vec4F total2 = Vec4F.Zero;
				//foreach( var v in sampleWeights )
				//{
				//	total2 += v;
				//}
				//Log.Info( total2.ToString() );
			}

			var result = new Result();
			result.SampleOffsets = sampleOffsets;
			result.SampleOffsetsAsVec4Array = ConvertToVec4F( sampleOffsets );
			result.SampleWeights = sampleWeights;
			return result;
		}
	}
}
