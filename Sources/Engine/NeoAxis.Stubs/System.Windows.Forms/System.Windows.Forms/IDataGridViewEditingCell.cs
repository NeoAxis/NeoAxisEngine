namespace System.Windows.Forms
{
	public interface IDataGridViewEditingCell
	{
		object EditingCellFormattedValue
		{
			get;
			set;
		}

		bool EditingCellValueChanged
		{
			get;
			set;
		}

		object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context);

		void PrepareEditingCellForEdit(bool selectAll);
	}
}
