namespace System.Windows.Forms
{
	public class DataGridViewTextBoxEditingControl : TextBox, IDataGridViewEditingControl
	{
		public virtual DataGridView EditingControlDataGridView
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

		public virtual object EditingControlFormattedValue
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

		public virtual int EditingControlRowIndex
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

		public virtual bool EditingControlValueChanged
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

		public virtual Cursor EditingPanelCursor
		{
			get
			{
				throw null;
			}
		}

		public virtual bool RepositionEditingControlOnValueChange
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewTextBoxEditingControl()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			throw null;
		}

		public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			throw null;
		}

		public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		public virtual void PrepareEditingControlForEdit(bool selectAll)
		{
			throw null;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			throw null;
		}
	}
}
