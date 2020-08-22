using NeoAxis.Editor;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NeoAxis
{
	class ColorGradientControl : UserControl
	{
		private System.ComponentModel.Container components = null;

		int tracking = -1;
		int highlight = -1;

		const int triangleSize = 7;
		const int triangleSides = ( triangleSize - 1 ) / 2;

		int[] values;

		public int Value
		{
			get { return GetValue( 0 ); }
			set { SetValue( 0, value ); }
		}

		public int Count
		{
			get { return values.Length; }

			set
			{
				if( value < 0 || value > 16 )
					throw new ArgumentOutOfRangeException( "value", value, "Count must be between 0 and 16" );

				values = new int[ value ];

				if( value > 1 )
				{
					for( int i = 0; i < value; i++ )
						values[ i ] = i * 255 / ( value - 1 );
				}
				else if( value == 1 )
				{
					values[ 0 ] = 128;
				}

				OnValueChanged( /*0*/ );
				Invalidate();
			}
		}

		public int GetValue( int index )
		{
			if( index < 0 || index >= values.Length )
				throw new ArgumentOutOfRangeException( "index", index, "Index must be within the bounds of the array" );
			return values[ index ];
		}

		public void SetValue( int index, int val )
		{
			int min = -1;
			int max = 256;

			if( index < 0 || index >= values.Length )
				throw new ArgumentOutOfRangeException( "index", index, "Index must be within the bounds of the array" );

			if( index - 1 >= 0 )
				min = values[ index - 1 ];
			if( index + 1 < values.Length )
				max = values[ index + 1 ];

			if( values[ index ] != val )
			{
				values[ index ] = val;// Utility.Clamp( val, min + 1, max - 1 );
				if( values[ index ] < min + 1 )
					values[ index ] = min + 1;
				if( values[ index ] > max - 1 )
					values[ index ] = max - 1;
				OnValueChanged(/* index */);
				Invalidate();
			}

			Update();
		}

		public event /*Index*/EventHandler ValueChanged;
		protected virtual void OnValueChanged( /*int index*/ )
		{
			if( ValueChanged != null )
				ValueChanged( this, EventArgs.Empty );// new IndexEventArgs( index ) );
		}

		private Color topColor;
		public Color TopColor
		{
			get { return topColor; }
			set
			{
				if( topColor != value )
				{
					topColor = value;
					Invalidate();
				}
			}
		}

		private Color bottomColor;
		public Color BottomColor
		{
			get { return bottomColor; }
			set
			{
				if( bottomColor != value )
				{
					bottomColor = value;
					Invalidate();
				}
			}
		}

		public ColorGradientControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.ResizeRedraw = true;
			this.Count = 1;
		}

		private void DrawGradient( Graphics g )
		{
#if !ANDROID
			System.Drawing.Rectangle gradientRect;

			// draw gradient
			using( LinearGradientBrush lgb = new LinearGradientBrush( this.ClientRectangle, topColor, bottomColor, 90, false ) )
			{
				gradientRect = ClientRectangle;
				gradientRect.Inflate( -triangleSize, -triangleSides );
				g.FillRectangle( lgb, gradientRect );
			}

			using( SolidBrush brush = new SolidBrush( BackColor ) )
			{
				g.FillRectangle( brush, new System.Drawing.Rectangle( 0, 0, triangleSize, ClientRectangle.Bottom ) );
				g.FillRectangle( brush, new System.Drawing.Rectangle( ClientRectangle.Width - triangleSize, 0, triangleSize, ClientRectangle.Bottom ) );
				g.FillRectangle( brush, new System.Drawing.Rectangle( 0, 0, ClientRectangle.Width, triangleSides ) );
				g.FillRectangle( brush, new System.Drawing.Rectangle( 0, ClientRectangle.Height - triangleSides, ClientRectangle.Width, triangleSides ) );
			}

			// draw value triangles
			for( int i = 0; i < values.Length; i++ )
			{
				int valueY = ValueToPosition( values[ i ] );
				Brush brush;

				//if( i == highlight )
				//{
				//   brush = Brushes.Blue;
				//}
				//else
				//{
				if( EditorAPI.DarkTheme )
					brush = new SolidBrush( Color.FromArgb( 140, 140, 140 ) );
				else
					brush = Brushes.Black;
				//}

				g.SmoothingMode = SmoothingMode.AntiAlias;

				Point a1 = new Point( 0, valueY - triangleSides );
				Point b1 = new Point( triangleSize - 1, valueY );
				Point c1 = new Point( 0, valueY + triangleSides );
				g.FillPolygon( brush, new Point[] { a1, b1, c1, a1 } );

				Point a2 = new Point( Width - 1 - a1.X, a1.Y );
				Point b2 = new Point( Width - 1 - b1.X, b1.Y );
				Point c2 = new Point( Width - 1 - c1.X, c1.Y );
				g.FillPolygon( brush, new Point[] { a2, b2, c2, a2 } );

				if( EditorAPI.DarkTheme )
					brush.Dispose();
			}
#endif //!ANDROID
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			DrawGradient( e.Graphics );
		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			DrawGradient( pevent.Graphics );
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
					components = null;
				}
			}
			base.Dispose( disposing );
		}

		int PositionToValue( int position )
		{
			return ( ( ( Height - triangleSize ) - ( position - triangleSides ) ) * 255 ) / ( Height - triangleSize );
		}

		int ValueToPosition( int val )
		{
			return triangleSides + ( ( Height - triangleSize ) - ( ( ( val * ( Height - triangleSize ) ) / 255 ) ) );
		}

		private int WhichTriangle( int yval )
		{
			int bestIndex = -1, bestDistance = int.MaxValue;
			int y = PositionToValue( yval );

			for( int i = 0; i < values.Length; i++ )
			{
				int distance = Math.Abs( values[ i ] - y );
				if( distance < bestDistance )
				{
					bestDistance = distance;
					bestIndex = i;
				}
			}
			return bestIndex;
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( e.Button == MouseButtons.Left )
			{
				tracking = WhichTriangle( e.Y );
				Invalidate();
				OnMouseMove( e );
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( e.Button == MouseButtons.Left )
			{
				OnMouseMove( e );
				tracking = -1;
				Invalidate();
			}
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( tracking >= 0 )
			{
				this.SetValue( tracking, PositionToValue( e.Y ) );
			}
			else
			{
				int oldHighlight = highlight;
				highlight = WhichTriangle( e.Y );

				if( highlight != oldHighlight )
				{
					this.InvalidateTriangle( oldHighlight );
					this.InvalidateTriangle( highlight );
				}
			}
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			int oldhighlight = highlight;
			highlight = -1;
			this.InvalidateTriangle( oldhighlight );
		}

		private void InvalidateTriangle( int index )
		{
			if( index < 0 || index >= values.Length )
			{
				return;
			}

			int valueY = ValueToPosition( values[ index ] );
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle( 0, valueY - triangleSides, this.Width, triangleSize );

			this.Invalidate( rect, true );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
