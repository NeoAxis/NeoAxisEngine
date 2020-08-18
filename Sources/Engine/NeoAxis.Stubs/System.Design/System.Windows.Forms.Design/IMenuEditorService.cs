namespace System.Windows.Forms.Design
{
	public interface IMenuEditorService
	{
		Menu GetMenu();

		bool IsActive();

		void SetMenu(Menu menu);

		void SetSelection(MenuItem item);

		bool MessageFilter(ref Message m);
	}
}
