using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	class ColorWheel : UserControl
	{
		System.ComponentModel.Container components = null;
		Bitmap renderBitmap = null;
		bool tracking = false;
		Point lastMouseXY;

		// this number controls what you might call the tesselation of the color wheel. higher #'s = slower, lower #'s = looks worse
		const int colorCount = 64 * 4;

		System.Windows.Forms.PictureBox wheelPictureBox;

		HSVColor hsvColor;

		public HSVColor HsvColor
		{
			get
			{
				return hsvColor;
			}

			set
			{
				if( hsvColor != value )
				{
					HSVColor oldColor = hsvColor;
					hsvColor = value;
					this.OnColorChanged();
					Invalidate( true );
				}
			}
		}

		public ColorWheel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//wheelRegion = new PdnRegion();
			hsvColor = new HSVColor( 0, 0, 0 );
		}

		private static PointF SphericalToCartesian( float r, float theta )
		{
			float x;
			float y;

			x = r * (float)Math.Cos( theta );
			y = r * (float)Math.Sin( theta );

			return new PointF( x, y );
		}

		private static PointF[] GetCirclePoints( float r, PointF center )
		{
			PointF[] points = new PointF[ colorCount ];

			for( int i = 0; i < colorCount; i++ )
			{
				float theta = ( (float)i / (float)colorCount ) * 2 * (float)Math.PI;
				points[ i ] = SphericalToCartesian( r, theta );
				points[ i ].X += center.X;
				points[ i ].Y += center.Y;
			}

			return points;
		}

		private Color[] GetColors()
		{
			Color[] colors = new Color[ colorCount ];

			for( int i = 0; i < colorCount; i++ )
			{
				int hue = ( i * 360 ) / colorCount;
				colors[ i ] = new HSVColor( hue, 1, 1 ).ToColor();
			}

			return colors;
		}

		protected override void OnLoad( EventArgs e )
		{
			InitRendering();
			base.OnLoad( e );
		}


		protected override void OnPaint( PaintEventArgs e )
		{
			InitRendering();
			base.OnPaint( e );
		}

		private void InitRendering()
		{
			if( this.renderBitmap == null )
			{
				InitRenderSurface();
				this.wheelPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
				int size = (int)Math.Ceiling( ComputeDiameter( this.Size ) );
				this.wheelPictureBox.Size = new Size( size, size );
				this.wheelPictureBox.Image = this.renderBitmap;
			}
		}

		private void wheelPictureBox_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			float radius = ComputeRadius( Size );
			float theta = ( (float)HsvColor.Hue / 360.0f ) * 2.0f * (float)Math.PI;
			float alpha = ( (float)HsvColor.Saturation );
			float x = ( alpha * ( radius - 1 ) * (float)Math.Cos( theta ) ) + radius;
			float y = ( alpha * ( radius - 1 ) * (float)Math.Sin( theta ) ) + radius;

			GraphicsContainer container = e.Graphics.BeginContainer();
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.DrawRectangle( Pens.Black, x - 1, y - 1, 3, 3 );
			e.Graphics.DrawRectangle( Pens.White, x, y, 1, 1 );
			e.Graphics.EndContainer( container );
		}

		private void InitRenderSurface()
		{
			if( renderBitmap != null )
			{
				renderBitmap.Dispose();
			}

			int wheelDiameter = (int)ComputeDiameter( Size );

			renderBitmap = new Bitmap( 
				Math.Max( 1, ( wheelDiameter * 4 ) / 3 ),
				Math.Max( 1, ( wheelDiameter * 4 ) / 3 ), System.Drawing.Imaging.PixelFormat.Format24bppRgb );

			using( Graphics g1 = Graphics.FromImage( renderBitmap ) )
			{
#if !ANDROID && !IOS
				g1.Clear( this.BackColor );
#endif //!ANDROID
				DrawWheel( g1, renderBitmap.Width, renderBitmap.Height );
			}
		}

		private void DrawWheel( Graphics g, int width, int height )
		{
			float radius = ComputeRadius( new Size( width, height ) );
			PointF[] points = GetCirclePoints( Math.Max( 1.0f, (float)radius - 1 ), new PointF( radius, radius ) );

#if !ANDROID && !IOS
			using( PathGradientBrush pgb = new PathGradientBrush( points ) )
			{
				pgb.CenterColor = new HSVColor( 0, 0, 1 ).ToColor();
				pgb.CenterPoint = new PointF( radius, radius );
				pgb.SurroundColors = GetColors();

				g.FillEllipse( pgb, 0, 0, radius * 2, radius * 2 );
			}
#endif //!ANDROID
		}

		private static float ComputeRadius( Size size )
		{
			return Math.Min( (float)size.Width / 2, (float)size.Height / 2 );
		}

		private static float ComputeDiameter( Size size )
		{
			return Math.Min( (float)size.Width, (float)size.Height );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

#if !ANDROID && !IOS
			if( renderBitmap != null && ( ComputeRadius( Size ) != ComputeRadius( renderBitmap.Size ) ) )
			{
				renderBitmap.Dispose();
				renderBitmap = null;
			}
#endif //!ANDROID

			Invalidate();
		}

		public event System.EventHandler ColorChanged;
		protected virtual void OnColorChanged()
		{
			if( ColorChanged != null )
			{
				ColorChanged( this, EventArgs.Empty );
			}
		}

		private void GrabColor( Point mouseXY )
		{
			// center our coordinate system so the middle is (0,0), and positive Y is facing up
			int cx = mouseXY.X - ( Width / 2 );
			int cy = mouseXY.Y - ( Height / 2 );

			double theta = Math.Atan2( cy, cx );

			if( theta < 0 )
			{
				theta += 2 * Math.PI;
			}

			double alpha = Math.Sqrt( ( cx * cx ) + ( cy * cy ) );

			double h = ( theta / ( Math.PI * 2 ) ) * 360.0;
			double s = Math.Min( 1, ( alpha / (double)( Width / 2 ) ) );
			double v = 1;

			hsvColor = new HSVColor( h, s, v );
			OnColorChanged();
			Invalidate( true );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( e.Button == MouseButtons.Left )
			{
				tracking = true;
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( tracking )
			{
				GrabColor( new Point( e.X, e.Y ) );
			}

			tracking = false;
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			lastMouseXY = new Point( e.X, e.Y );

			if( tracking )
			{
				GrabColor( new Point( e.X, e.Y ) );
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
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

#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.wheelPictureBox = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// wheelPictureBox
			// 
			this.wheelPictureBox.Location = new System.Drawing.Point( 0, 0 );
			this.wheelPictureBox.Name = "wheelPictureBox";
			this.wheelPictureBox.TabIndex = 0;
			this.wheelPictureBox.TabStop = false;
			this.wheelPictureBox.Click += new System.EventHandler( this.wheelPictureBox_Click );
			this.wheelPictureBox.Paint += new System.Windows.Forms.PaintEventHandler( this.wheelPictureBox_Paint );
			this.wheelPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler( this.wheelPictureBox_MouseUp );
			this.wheelPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler( this.wheelPictureBox_MouseMove );
			this.wheelPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler( this.wheelPictureBox_MouseDown );
			// 
			// ColorWheel
			// 
			this.Controls.Add( this.wheelPictureBox );
			this.Name = "ColorWheel";
			this.ResumeLayout( false );

		}
#endregion

		private void wheelPictureBox_MouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			OnMouseMove( e );
		}

		private void wheelPictureBox_MouseUp( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			OnMouseUp( e );
		}

		private void wheelPictureBox_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			OnMouseDown( e );
		}

		private void wheelPictureBox_Click( object sender, System.EventArgs e )
		{
			OnClick( e );
		}
	}
}
