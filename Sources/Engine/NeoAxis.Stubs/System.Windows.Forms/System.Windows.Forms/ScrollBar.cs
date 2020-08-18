using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class ScrollBar : Control
	{
		public override bool AutoSize
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Color BackColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Image BackgroundImage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		protected override ImeMode DefaultImeMode
		{
			get
			{
				throw null;
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				throw null;
			}
		}

		public override Color ForeColor
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override Font Font
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public new ImeMode ImeMode
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int LargeChange
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Maximum
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Minimum
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int SmallChange
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public new bool TabStop
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public override string Text
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int Value
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool ScaleScrollBarForDpiChange
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public new event EventHandler AutoSizeChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler BackColorChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler BackgroundImageChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler BackgroundImageLayoutChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler ForeColorChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler FontChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler ImeModeChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler TextChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler Click
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event PaintEventHandler Paint
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event EventHandler DoubleClick
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseClick
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseDoubleClick
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseDown
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseUp
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public new event MouseEventHandler MouseMove
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event ScrollEventHandler Scroll
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event EventHandler ValueChanged
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public ScrollBar()
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnScroll(ScrollEventArgs se)
		{
			throw null;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected void UpdateScrollInfo()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
