using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolStripTextBox : ToolStripControlHost
	{
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public TextBox TextBox
		{
			get
			{
				throw null;
			}
		}

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

		public bool AcceptsReturn
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

		public AutoCompleteStringCollection AutoCompleteCustomSource
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

		public AutoCompleteMode AutoCompleteMode
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

		public AutoCompleteSource AutoCompleteSource
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

		public bool CanUndo
		{
			get
			{
				throw null;
			}
		}

		public CharacterCasing CharacterCasing
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

		public int MaxLength
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

		public bool Multiline
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

		public string SelectedText
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

		public int SelectionLength
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

		public bool ShortcutsEnabled
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

		public int TextLength
		{
			get
			{
				throw null;
			}
		}

		public HorizontalAlignment TextBoxTextAlign
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

		public event EventHandler TextBoxTextAlignChanged
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

		public ToolStripTextBox()
			:base(null)
		{
			throw null;
		}

		public ToolStripTextBox(string name)
			:base(null)
		{
			throw null;
		}

		public ToolStripTextBox(Control c)
			:base(null)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public override Size GetPreferredSize(Size constrainingSize)
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

		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnModifiedChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMultilineChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnSubscribeControlEvents(Control control)
		{
			throw null;
		}

		protected override void OnUnsubscribeControlEvents(Control control)
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

		public void Cut()
		{
			throw null;
		}

		public void DeselectAll()
		{
			throw null;
		}

		public char GetCharFromPosition(Point pt)
		{
			throw null;
		}

		public int GetCharIndexFromPosition(Point pt)
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

		public int GetLineFromCharIndex(int index)
		{
			throw null;
		}

		public Point GetPositionFromCharIndex(int index)
		{
			throw null;
		}

		public void Paste()
		{
			throw null;
		}

		public void ScrollToCaret()
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

		public void Undo()
		{
			throw null;
		}
	}
}
