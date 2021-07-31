using System.Drawing;
using System.Windows.Forms.Automation;

namespace System.Windows.Forms
{
	public class Label : Control, IAutomationLiveRegion
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

		public bool AutoEllipsis
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

		public virtual BorderStyle BorderStyle
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public FlatStyle FlatStyle
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

		public Image Image
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

		public int ImageIndex
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

		public string ImageKey
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

		public ImageList ImageList
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

		public ContentAlignment ImageAlign
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

		public AutomationLiveSetting LiveSetting
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

		public virtual int PreferredHeight
		{
			get
			{
				throw null;
			}
		}

		public virtual int PreferredWidth
		{
			get
			{
				throw null;
			}
		}

		protected virtual bool RenderTransparent
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

		public virtual ContentAlignment TextAlign
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

		public bool UseCompatibleTextRendering
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

		public bool UseMnemonic
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

		public new event KeyEventHandler KeyUp
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

		public new event KeyEventHandler KeyDown
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

		public new event KeyPressEventHandler KeyPress
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

		public new event EventHandler TabStopChanged
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

		public event EventHandler TextAlignChanged
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

		public Label()
		{
			throw null;
		}

		protected Rectangle CalcImageRenderBounds(Image image, Rectangle r, ContentAlignment align)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected void DrawImage(Graphics g, Image image, Rectangle r, ContentAlignment align)
		{
			throw null;
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			throw null;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}
	}
}
