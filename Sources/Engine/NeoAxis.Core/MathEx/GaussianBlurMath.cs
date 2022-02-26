// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Provides the functions for calculating parameters of Gaussian blur.
	/// </summary>
	static class GaussianBlurMath
	{
		static double GaussianDistribution( double x, double y, double standardDeviation )
		{
			if( standardDeviation < 0.00001 )
				standardDeviation = 0.00001;

			//var div = Math.Sqrt( 2.0 * Math.PI * rho * rho );
			//if( div == 0 )
			//	div = 0.00001;
			//double g = 1.0 / div;

			double g = 1.0 / Math.Sqrt( 2.0 * Math.PI * standardDeviation * standardDeviation );

			g *= Math.Exp( -( x * x + y * y ) / ( 2.0 * standardDeviation * standardDeviation ) );
			return g;
		}

		static Vector4F[] ConvertToVector4F( Vector2F[] source )
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
			public Vector4F[] SampleOffsetsAsVector4Array;
			public Vector4F[] SampleWeights;
		}

		public static Result Calculate15( Vector2I textureSize, bool horizontal, double blurFactor, double standardDeviation = 3 )//, double intensity = 1 )
		{
			Vector2F[] sampleOffsets = new Vector2F[ 15 ];
			Vector4F[] sampleWeights = new Vector4F[ 15 ];

			// calculate gaussian texture offsets & weights
			float texelSize = 1.0f / (float)( horizontal ? textureSize.X : textureSize.Y );
			texelSize *= (float)blurFactor;

			// central sample, no offset
			sampleOffsets[ 0 ] = Vector2F.Zero;
			{
				float distribution = (float)GaussianDistribution( 0, 0, standardDeviation );
				sampleWeights[ 0 ] = new Vector4F( distribution, distribution, distribution, 1 );
			}

			// 'pre' samples
			for( int n = 1; n < 8; n++ )
			{
				float distribution = (float)GaussianDistribution( n, 0, standardDeviation );
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

			////apply intensity
			//if( intensity < 1 )
			//{
			//	var averageValue = 1.0f / (float)sampleWeights.Length;
			//	var averageValueVector = new Vector4F( averageValue, averageValue, averageValue, averageValue );

			//	for( int n = 0; n < sampleWeights.Length; n++ )
			//		sampleWeights[ n ] = Vector4F.Lerp( averageValueVector, sampleWeights[ n ], (float)intensity );
			//}

			//normalize weights
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
			result.SampleOffsetsAsVector4Array = ConvertToVector4F( sampleOffsets );
			result.SampleWeights = sampleWeights;
			return result;
		}
	}
}
