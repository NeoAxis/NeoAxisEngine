namespace System.Windows.Forms
{
	public class DataGridViewElement
	{
		public virtual DataGridViewElementStates State
		{
			get
			{
				throw null;
			}
		}

		public DataGridView DataGridView
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewElement()
		{
			throw null;
		}

		protected virtual void OnDataGridViewChanged()
		{
			throw null;
		}

		protected void RaiseCellClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected void RaiseCellContentClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected void RaiseCellValueChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected void RaiseDataError(DataGridViewDataErrorEventArgs e)
		{
			throw null;
		}

		protected void RaiseMouseWheel(MouseEventArgs e)
		{
			throw null;
		}
	}
}
