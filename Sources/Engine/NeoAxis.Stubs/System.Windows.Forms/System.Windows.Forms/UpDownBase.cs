using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class UpDownBase : ContainerControl
	{
		public override bool AutoScroll
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

		public new Size AutoScrollMargin
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

		public new Size AutoScrollMinSize
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

		public BorderStyle BorderStyle
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

		protected bool ChangingText
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

		public override ContextMenu ContextMenu
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

		public override ContextMenuStrip ContextMenuStrip
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public new DockPaddingEdges DockPadding
		{
			get
			{
				throw null;
			}
		}

		public override bool Focused
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

		public bool InterceptArrowKeys
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

		public override Size MaximumSize
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

		public override Size MinimumSize
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

		public int PreferredHeight
		{
			get
			{
				throw null;
			}
		}

		public bool ReadOnly
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

		public HorizontalAlignment TextAlign
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

		public LeftRightAlignment UpDownAlign
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

		protected bool UserEdit
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

		public new event EventHandler MouseEnter
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

		public new event EventHandler MouseLeave
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

		public new event EventHandler MouseHover
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

		public UpDownBase()
		{
			throw null;
		}

		public abstract void DownButton();

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		protected virtual void OnChanged(object source, EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextBoxLostFocus(object source, EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextBoxResize(object source, EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextBoxTextChanged(object source, EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			throw null;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		public void Select(int start, int length)
		{
			throw null;
		}

		public abstract void UpButton();

		protected abstract void UpdateEditText();

		protected virtual void ValidateEditText()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
