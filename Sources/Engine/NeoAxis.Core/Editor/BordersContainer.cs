#if !DEPLOY
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	[Flags]
	//TODO: add FlagEnumUIEditor to support flag editing in vs grid in design time.
	//[Editor( typeof( Design.FlagEnumUIEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
	public enum BorderSides
	{
		None = 0,
		Top = 1,
		Bottom = 2,
		Left = 4,
		Right = 8,
		All = Top | Bottom | Left | Right
	}

	///// TODO: consider using krypton VisualControl/VisualControlBase as base
	///// because it supports separate borders drawing.
	///// 
	///// or consider using KryptonBorderEdge

	/// <summary>
	/// Helper container control to draw separate borders.
	/// </summary>
	public class BordersContainer : ContainerControl
	{
		private BorderSides _borderSides = BorderSides.All;
		[DefaultValue( BorderSides.All )]
		[Category( "Appearance" )]
		public BorderSides BorderSides
		{
			get
			{
				return _borderSides;
			}
			set
			{
				if( _borderSides != value )
				{
					_borderSides = value;
					this.Invalidate();
				}
			}
		}

		Color _borderColor;
		[Category( "Appearance" )]
		public Color BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		public BordersContainer()
		{
			IPalette palette = KryptonManager.CurrentGlobalPalette;
			_borderColor = palette.GetBorderColor1( PaletteBorderStyle.ControlClient, PaletteState.Normal );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( BorderSides == BorderSides.None )
				return;

			if( BorderSides == BorderSides.All )
			{
#if !ANDROID && !IOS
				using( var p = new Pen( BorderColor ) )
					e.Graphics.DrawRectangle( p, new System.Drawing.Rectangle( 0, 0, Width - 1, Height - 1 ) );
#endif //!ANDROID
			}
			else
			{
				ControlPaint.DrawBorder( e.Graphics, Bounds,
					BorderColor, 1, ( BorderSides & BorderSides.Left ) != 0 ? ButtonBorderStyle.Solid : ButtonBorderStyle.None,
					BorderColor, 1, ( BorderSides & BorderSides.Top ) != 0 ? ButtonBorderStyle.Solid : ButtonBorderStyle.None,
					BorderColor, 1, ( BorderSides & BorderSides.Right ) != 0 ? ButtonBorderStyle.Solid : ButtonBorderStyle.None,
					BorderColor, 1, ( BorderSides & BorderSides.Bottom ) != 0 ? ButtonBorderStyle.Solid : ButtonBorderStyle.None );
			}
		}

		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );
			e.Control.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			if( Controls.Count == 0 )
				return;

			var b = this.Bounds;

			if( ( BorderSides & BorderSides.Top ) != 0 )
				b.Y++; b.Height--;
			if( ( BorderSides & BorderSides.Bottom ) != 0 )
				b.Height--;
			if( ( BorderSides & BorderSides.Left ) != 0 )
				b.X++; b.Width--;
			if( ( BorderSides & BorderSides.Right ) != 0 )
				b.Width--;

			Controls[0].Bounds = b;
		}
	}
}

#endif