using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class TextBoxBase : Control
	{
		public bool AcceptsTab
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

		public virtual bool ShortcutsEnabled
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

		protected override bool CanEnableIme
		{
			get
			{
				throw null;
			}
		}

		public bool CanUndo
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

		protected override bool DoubleBuffered
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

		protected override Cursor DefaultCursor
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

		public bool HideSelection
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

		protected override ImeMode ImeModeBase
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

		public string[] Lines
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

		public virtual int MaxLength
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

		public bool Modified
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

		public virtual bool Multiline
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

		public new Padding Padding
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

		public virtual string SelectedText
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

		public virtual int SelectionLength
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

		public int SelectionStart
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

		public virtual int TextLength
		{
			get
			{
				throw null;
			}
		}

		public bool WordWrap
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

		public event EventHandler AcceptsTabChanged
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

		public event EventHandler BorderStyleChanged
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

		public event EventHandler HideSelectionChanged
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

		public event EventHandler ModifiedChanged
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

		public event EventHandler MultilineChanged
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

		public new event EventHandler PaddingChanged
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

		public event EventHandler ReadOnlyChanged
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

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			throw null;
		}

		public void AppendText(string text)
		{
			throw null;
		}

		public void Clear()
		{
			throw null;
		}

		public void ClearUndo()
		{
			throw null;
		}

		public void Copy()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		public void Cut()
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
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

		public void Paste()
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected virtual void OnAcceptsTabChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnModifiedChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			throw null;
		}

		protected virtual void OnMultilineChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		public virtual char GetCharFromPosition(Point pt)
		{
			throw null;
		}

		public virtual int GetCharIndexFromPosition(Point pt)
		{
			throw null;
		}

		public virtual int GetLineFromCharIndex(int index)
		{
			throw null;
		}

		public virtual Point GetPositionFromCharIndex(int index)
		{
			throw null;
		}

		public int GetFirstCharIndexFromLine(int lineNumber)
		{
			throw null;
		}

		public int GetFirstCharIndexOfCurrentLine()
		{
			throw null;
		}

		public void ScrollToCaret()
		{
			throw null;
		}

		public void DeselectAll()
		{
			throw null;
		}

		public void Select(int start, int length)
		{
			throw null;
		}

		public void SelectAll()
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

		public void Undo()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
