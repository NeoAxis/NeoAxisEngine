using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class ScrollableControl : Control, IComponent, IDisposable
	{
		public class DockPaddingEdges : ICloneable
		{
			public int All
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

			public int Bottom
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

			public int Left
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

			public int Right
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

			public int Top
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

			public override bool Equals(object other)
			{
				throw null;
			}

			public override int GetHashCode()
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}

			object ICloneable.Clone()
			{
				throw null;
			}
		}

		public class DockPaddingEdgesConverter : TypeConverter
		{
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				throw null;
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				throw null;
			}

			public DockPaddingEdgesConverter()
			{
				throw null;
			}
		}

		public virtual bool AutoScroll
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

		public Size AutoScrollMargin
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

		public Point AutoScrollPosition
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

		public Size AutoScrollMinSize
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

		public override Rectangle DisplayRectangle
		{
			get
			{
				throw null;
			}
		}

		protected bool HScroll
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

		public HScrollProperties HorizontalScroll
		{
			get
			{
				throw null;
			}
		}

		protected bool VScroll
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

		public VScrollProperties VerticalScroll
		{
			get
			{
				throw null;
			}
		}

		public DockPaddingEdges DockPadding
		{
			get
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

		public ScrollableControl()
		{
			throw null;
		}

		protected virtual void AdjustFormScrollbars(bool displayScrollbars)
		{
			throw null;
		}

		protected bool GetScrollState(int bit)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		protected void SetDisplayRectLocation(int x, int y)
		{
			throw null;
		}

		public void ScrollControlIntoView(Control activeControl)
		{
			throw null;
		}

		protected virtual Point ScrollToControl(Control activeControl)
		{
			throw null;
		}

		protected virtual void OnScroll(ScrollEventArgs se)
		{
			throw null;
		}

		public void SetAutoScrollMargin(int x, int y)
		{
			throw null;
		}

		protected void SetScrollState(int bit, bool value)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
