using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class ButtonBase : Control
	{
		public class ButtonBaseAccessibleObject : ControlAccessibleObject
		{
			public override AccessibleStates State
			{
				get
				{
					throw null;
				}
			}

			public ButtonBaseAccessibleObject(Control owner)
				:base(owner)
			{
				throw null;
			}

			public override void DoDefaultAction()
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

		protected override Size DefaultSize
		{
			get
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

		public FlatButtonAppearance FlatAppearance
		{
			get
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

		public TextImageRelation TextImageRelation
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

		public bool UseVisualStyleBackColor
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

		protected ButtonBase()
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

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseEnter(EventArgs eventargs)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs eventargs)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs mevent)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			throw null;
		}

		protected void ResetFlagsandPaint()
		{
			throw null;
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs kevent)
		{
			throw null;
		}

		protected override void OnKeyUp(KeyEventArgs kevent)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			throw null;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
