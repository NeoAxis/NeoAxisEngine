// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	public class PointContainer3D
	{
		Bounds bounds;
		double sizeX;
		Cell[] cells;

		struct Cell
		{
			public List<Vector3> points;
		}

		public PointContainer3D( Bounds bounds, int cellCount )
		{
			this.bounds = bounds;
			sizeX = bounds.GetSize().X;
			cells = new Cell[ cellCount ];
		}

		int GetCellIndex( double x )
		{
			if( sizeX == 0 )
				return 0;
			var a = ( x - bounds.Minimum.X ) / sizeX;
			var index = (int)( a * cells.Length );
			return MathEx.Clamp( index, 0, cells.Length - 1 );
		}

		public void Add( ref Vector3 point )
		{
			var index = GetCellIndex( point.X );

			ref var cell = ref cells[ index ];
			if( cell.points == null )
				cell.points = new List<Vector3>();
			cell.points.Add( point );
		}

		public bool Contains( Sphere sphere )
		{
			var b = sphere.ToBounds();
			var indexMin = GetCellIndex( b.Minimum.X );
			var indexMax = GetCellIndex( b.Maximum.X );

			for( int index = indexMin; index <= indexMax; index++ )
			{
				ref var cell = ref cells[ index ];
				if( cell.points != null )
				{
					for( int n = 0; n < cell.points.Count; n++ )
					{
						if( sphere.Contains( cell.points[ n ] ) )
							return true;
					}
				}
			}

			return false;
		}
	}
}
