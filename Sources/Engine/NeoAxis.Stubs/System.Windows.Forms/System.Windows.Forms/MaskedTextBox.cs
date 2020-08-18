using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
	public class MaskedTextBox : TextBoxBase
	{
		public new bool AcceptsTab
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

		public bool AllowPromptAsInput
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

		public bool AsciiOnly
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

		public bool BeepOnError
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

		public new bool CanUndo
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

		public CultureInfo Culture
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

		public MaskFormat CutCopyMaskFormat
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

		public IFormatProvider FormatProvider
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

		public bool HidePromptOnLeave
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

		public InsertKeyMode InsertKeyMode
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

		public bool IsOverwriteMode
		{
			get
			{
				throw null;
			}
		}

		public new string[] Lines
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

		public string Mask
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

		public bool MaskCompleted
		{
			get
			{
				throw null;
			}
		}

		public bool MaskFull
		{
			get
			{
				throw null;
			}
		}

		public MaskedTextProvider MaskedTextProvider
		{
			get
			{
				throw null;
			}
		}

		public override int MaxLength
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

		public override bool Multiline
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

		public char PasswordChar
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

		public char PromptChar
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

		public new bool ReadOnly
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

		public bool RejectInputOnFirstFailure
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

		public bool ResetOnPrompt
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

		public bool ResetOnSpace
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

		public bool SkipLiterals
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

		public override string SelectedText
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

		public override int TextLength
		{
			get
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

		public MaskFormat TextMaskFormat
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

		public bool UseSystemPasswordChar
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

		public Type ValidatingType
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

		public new bool WordWrap
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

		public new event EventHandler AcceptsTabChanged
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

		public event EventHandler IsOverwriteModeChanged
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

		public event EventHandler MaskChanged
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

		public event MaskInputRejectedEventHandler MaskInputRejected
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

		public new event EventHandler MultilineChanged
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

		public event TypeValidationEventHandler TypeValidationCompleted
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

		public MaskedTextBox()
		{
			throw null;
		}

		public MaskedTextBox(string mask)
		{
			throw null;
		}

		public MaskedTextBox(MaskedTextProvider maskedTextProvider)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public new void ClearUndo()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		public override char GetCharFromPosition(Point pt)
		{
			throw null;
		}

		public override int GetCharIndexFromPosition(Point pt)
		{
			throw null;
		}

		public new int GetFirstCharIndexOfCurrentLine()
		{
			throw null;
		}

		public new int GetFirstCharIndexFromLine(int lineNumber)
		{
			throw null;
		}

		public override int GetLineFromCharIndex(int index)
		{
			throw null;
		}

		public override Point GetPositionFromCharIndex(int index)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnIsOverwriteModeChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMaskChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnMultilineChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			throw null;
		}

		public new void ScrollToCaret()
		{
			throw null;
		}

		public new void Undo()
		{
			throw null;
		}

		public object ValidateText()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
