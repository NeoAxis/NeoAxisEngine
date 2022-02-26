// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.IO;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for <see cref="FastRandom"/>.
	/// </summary>
	public static class RandomUtility
	{
		public static int GetRandomIndexByProbabilities( FastRandom random, ArraySegment<double> probabilities )
		{
			var length = probabilities.Count;

			int result = 0;

			double total = 0;
			for( int n = 0; n < length; n++ )
				total += probabilities[ n ];

			var v = random.Next( total );

			double previous = 0;
			double current = 0;
			for( int n = 0; n < length; n++ )
			{
				current += probabilities[ n ];

				if( v >= previous && v < current && probabilities[ n ] > 0 )
				{
					result = n;
					break;
				}

				previous = current;
			}

			if( result >= length )
				result = length - 1;
			if( result < 0 )
				result = 0;
			return result;
		}

		public static int GetRandomIndexByProbabilities( FastRandom random, double[] probabilities )
		{
			return GetRandomIndexByProbabilities( random, new ArraySegment<double>( probabilities, 0, probabilities.Length ) );
		}

		public unsafe static int GetRandomIndexByProbabilities( FastRandom random, double* probabilities, int length )
		{
			int result = 0;

			double total = 0;
			for( int n = 0; n < length; n++ )
				total += probabilities[ n ];

			var v = random.Next( total );

			double previous = 0;
			double current = 0;
			for( int n = 0; n < length; n++ )
			{
				current += probabilities[ n ];

				if( v >= previous && v < current && probabilities[ n ] > 0 )
				{
					result = n;
					break;
				}

				previous = current;
			}

			if( result >= length )
				result = length - 1;
			if( result < 0 )
				result = 0;
			return result;
		}
	}
}