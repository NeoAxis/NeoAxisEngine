// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.IO;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for <see cref="Random"/>.
	/// </summary>
	public static class RandomUtility
	{
		public static int GetRandomIndexByProbabilities( Random random, double[] probabilities, int length )
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

		public static int GetRandomIndexByProbabilities( Random random, double[] probabilities )
		{
			int result = 0;

			double total = 0;
			foreach( var p in probabilities )
				total += p;

			var v = random.Next( total );

			double previous = 0;
			double current = 0;
			for( int n = 0; n < probabilities.Length; n++ )
			{
				current += probabilities[ n ];

				if( v >= previous && v < current && probabilities[ n ] > 0 )
				{
					result = n;
					break;
				}

				previous = current;
			}

			if( result >= probabilities.Length )
				result = probabilities.Length - 1;
			if( result < 0 )
				result = 0;
			return result;
		}
	}
}