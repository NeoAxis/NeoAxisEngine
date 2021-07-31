namespace System.Windows.Forms.Design
{
	public interface IWindowsFormsEditorService
	{
		void CloseDropDown();

		void DropDownControl(Control control);

		DialogResult ShowDialog(Form dialog);
	}
}
