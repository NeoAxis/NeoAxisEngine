// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A tri-diagonal matrix has non-zero entries only on the main diagonal, the diagonal above the main (super), and the
	/// diagonal below the main (sub).
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is based on the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
	/// </para>
	/// <para>
	/// The entries in the matrix on a particular row are A[i], B[i], and C[i] where i is the row index.
	/// B is the main diagonal, and so for an NxN matrix B is length N and all elements are used.
	/// So for row 0, the first two values are B[0] and C[0].
	/// And for row N-1, the last two values are A[N-1] and B[N-1].
	/// That means that A[0] is not actually on the matrix and is therefore never used, and same with C[N-1].
	/// </para>
	/// </remarks>
	class TriDiagonalMatrixF
	{
		/// <summary>
		/// The values for the sub-diagonal. A[0] is never used.
		/// </summary>
		public float[] A;

		/// <summary>
		/// The values for the main diagonal.
		/// </summary>
		public float[] B;

		/// <summary>
		/// The values for the super-diagonal. C[C.Length-1] is never used.
		/// </summary>
		public float[] C;

		/// <summary>
		/// The width and height of this matrix.
		/// </summary>
		public int N
		{
			get { return ( A != null ? A.Length : 0 ); }
		}

		/// <summary>
		/// Indexer. Setter throws an exception if you try to set any not on the super, main, or sub diagonals.
		/// </summary>
		public float this[ int row, int col ]
		{
			get
			{
				int di = row - col;

				if( di == 0 )
				{
					return B[ row ];
				}
				else if( di == -1 )
				{
					Debug.Assert( row < N - 1 );
					return C[ row ];
				}
				else if( di == 1 )
				{
					Debug.Assert( row > 0 );
					return A[ row ];
				}
				else return 0;
			}
			set
			{
				int di = row - col;

				if( di == 0 )
				{
					B[ row ] = value;
				}
				else if( di == -1 )
				{
					Debug.Assert( row < N - 1 );
					C[ row ] = value;
				}
				else if( di == 1 )
				{
					Debug.Assert( row > 0 );
					A[ row ] = value;
				}
				else
				{
					throw new ArgumentException( "Only the main, super, and sub diagonals can be set." );
				}
			}
		}

		/// <summary>
		/// Construct an NxN matrix.
		/// </summary>
		public TriDiagonalMatrixF( int n )
		{
			this.A = new float[ n ];
			this.B = new float[ n ];
			this.C = new float[ n ];
		}

		/// <summary>
		/// Solve the system of equations this*x=d given the specified d.
		/// </summary>
		/// <remarks>
		/// Uses the Thomas algorithm described in the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
		/// Not optimized. Not destructive.
		/// </remarks>
		/// <param name="d">Right side of the equation.</param>
		public float[] Solve( float[] d )
		{
			int n = this.N;

			if( d.Length != n )
			{
				throw new ArgumentException( "The input d is not the same size as this matrix." );
			}

			// cPrime
			float[] cPrime = new float[ n ];
			cPrime[ 0 ] = C[ 0 ] / B[ 0 ];

			for( int i = 1; i < n; i++ )
			{
				cPrime[ i ] = C[ i ] / ( B[ i ] - cPrime[ i - 1 ] * A[ i ] );
			}

			// dPrime
			float[] dPrime = new float[ n ];
			dPrime[ 0 ] = d[ 0 ] / B[ 0 ];

			for( int i = 1; i < n; i++ )
			{
				dPrime[ i ] = ( d[ i ] - dPrime[ i - 1 ] * A[ i ] ) / ( B[ i ] - cPrime[ i - 1 ] * A[ i ] );
			}

			// Back substitution
			float[] x = new float[ n ];
			x[ n - 1 ] = dPrime[ n - 1 ];

			for( int i = n - 2; i >= 0; i-- )
			{
				x[ i ] = dPrime[ i ] - cPrime[ i ] * x[ i + 1 ];
			}

			return x;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Cubic spline interpolation.
	/// Call Fit to compute spline coefficients, then Eval to evaluate the spline at other X coordinates.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is implemented based on the wikipedia article:
	/// http://en.wikipedia.org/wiki/Spline_interpolation
	/// I'm not sure I have the right to include a copy of the article so the equation numbers referenced in 
	/// comments will end up being wrong at some point.
	/// </para>
	/// <para>
	/// This is not optimized, and is not MT safe.
	/// This can extrapolate off the ends of the splines.
	/// You must provide points in X sort order.
	/// </para>
	/// </remarks>
	class CubicSpline
	{
		// N-1 spline coefficients for N points
		float[] a;
		float[] b;

		// Save the original x and y for Eval
		float[] xOrig;
		float[] yOrig;

		int _lastIndex = 0;

		/// <summary>
		/// Fit x,y and then eval at points xs and return the corresponding y's.
		/// This does the "natural spline" style for ends.
		/// This can extrapolate off the ends of the splines.
		/// You must provide points in X sort order.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		/// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <returns>The computed y values for each xs.</returns>
		public float[] FitAndEval( float[] x, float[] y, float[] xs )
		{
			Fit( x, y );
			return Eval( xs );
		}

		/// <summary>
		/// Fit x,y and then eval at points xs and return the corresponding y's.
		/// This does the "natural spline" style for ends.
		/// This can extrapolate off the ends of the splines.
		/// You must provide points in X sort order.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		/// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <returns>The computed y values for each xs.</returns>
		public float FitAndEval( float[] x, float[] y, float xs )
		{
			Fit( x, y );
			return Eval( xs );
		}

		/// <summary>
		/// Compute spline coefficients for the specified x,y points.
		/// This does the "natural spline" style for ends.
		/// This can extrapolate off the ends of the splines.
		/// You must provide points in X sort order.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		public void Fit( float[] x, float[] y )
		{
			// Save x and y for eval
			this.xOrig = x;
			this.yOrig = y;

			int n = x.Length;
			float[] r = new float[ n ]; // the right hand side numbers: wikipedia page overloads b

			TriDiagonalMatrixF m = new TriDiagonalMatrixF( n );
			float dx1, dx2, dy1, dy2;

			// First row is different (equation 16 from the article)
			dx1 = x[ 1 ] - x[ 0 ];
			m.C[ 0 ] = 1.0f / dx1;
			m.B[ 0 ] = 2.0f * m.C[ 0 ];
			r[ 0 ] = 3 * ( y[ 1 ] - y[ 0 ] ) / ( dx1 * dx1 );

			// Body rows (equation 15 from the article)
			for( int i = 1; i < n - 1; i++ )
			{
				dx1 = x[ i ] - x[ i - 1 ];
				dx2 = x[ i + 1 ] - x[ i ];

				m.A[ i ] = 1.0f / dx1;
				m.C[ i ] = 1.0f / dx2;
				m.B[ i ] = 2.0f * ( m.A[ i ] + m.C[ i ] );

				dy1 = y[ i ] - y[ i - 1 ];
				dy2 = y[ i + 1 ] - y[ i ];
				r[ i ] = 3 * ( dy1 / ( dx1 * dx1 ) + dy2 / ( dx2 * dx2 ) );
			}

			// Last row also different (equation 17 from the article)
			dx1 = x[ n - 1 ] - x[ n - 2 ];
			dy1 = y[ n - 1 ] - y[ n - 2 ];
			m.A[ n - 1 ] = 1.0f / dx1;
			m.B[ n - 1 ] = 2.0f * m.A[ n - 1 ];
			r[ n - 1 ] = 3 * ( dy1 / ( dx1 * dx1 ) );

			// k is the solution to the matrix
			float[] k = m.Solve( r );

			// a and b are each spline's coefficients
			this.a = new float[ n - 1 ];
			this.b = new float[ n - 1 ];

			for( int i = 1; i < n; i++ )
			{
				dx1 = x[ i ] - x[ i - 1 ];
				dy1 = y[ i ] - y[ i - 1 ];
				a[ i - 1 ] = k[ i - 1 ] * dx1 - dy1; // equation 10 from the article
				b[ i - 1 ] = -k[ i ] * dx1 + dy1; // equation 11 from the article
			}
		}

		/// <summary>
		/// Evaluate the spline at the specified x coordinates.
		/// This can extrapolate off the ends of the splines.
		/// You must provide X's in ascending order.
		/// </summary>
		/// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <returns>The computed y values for each x.</returns>
		public float[] Eval( float[] x )
		{
			int n = x.Length;
			float[] y = new float[ n ];
			_lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

			for( int i = 0; i < n; i++ )
			{
				// Find which spline can be used to compute this x
				int j = GetNextXIndex( x[ i ] );

				// Evaluate using j'th spline
				float t = ( x[ i ] - xOrig[ j ] ) / ( xOrig[ j + 1 ] - xOrig[ j ] );
				y[ i ] = ( 1 - t ) * yOrig[ j ] + t * yOrig[ j + 1 ] + t * ( 1 - t ) * ( a[ j ] * ( 1 - t ) + b[ j ] * t ); // equation 9
			}

			return y;
		}

		/// <summary>
		/// Evaluate the spline at the specified x coordinates.
		/// This can extrapolate off the ends of the splines.
		/// You must provide X's in ascending order.
		/// </summary>
		/// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <returns>The computed y values for each x.</returns>
		public float Eval( float x )
		{
			_lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

			// Find which spline can be used to compute this x
			int j = GetNextXIndex( x );

			// Evaluate using j'th spline
			float t = ( x - xOrig[ j ] ) / ( xOrig[ j + 1 ] - xOrig[ j ] );
			float y = ( 1 - t ) * yOrig[ j ] + t * yOrig[ j + 1 ] + t * ( 1 - t ) * ( a[ j ] * ( 1 - t ) + b[ j ] * t ); // equation 9

			return y;
		}

		/// <summary>
		/// Find where in xOrig the specified x falls, by simultaneous traverse.
		/// This allows xs to be less than x[0] and/or greater than x[n-1]. So allows extrapolation.
		/// This keeps state, so requires that x be sorted and xs called in ascending order.
		/// </summary>
		int GetNextXIndex( float x )
		{
			while( ( _lastIndex < xOrig.Length - 2 ) && ( x > xOrig[ _lastIndex + 1 ] ) )
			{
				_lastIndex++;
			}

			return _lastIndex;
		}
	}
}
