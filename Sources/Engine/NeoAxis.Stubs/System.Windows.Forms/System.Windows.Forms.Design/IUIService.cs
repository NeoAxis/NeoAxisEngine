using System.Collections;

namespace System.Windows.Forms.Design
{
	public interface IUIService
	{
		IDictionary Styles
		{
			get;
		}

		bool CanShowComponentEditor(object component);

		IWin32Window GetDialogOwnerWindow();

		void SetUIDirty();

		bool ShowComponentEditor(object component, IWin32Window parent);

		DialogResult ShowDialog(Form form);

		void ShowError(string message);

		void ShowError(Exception ex);

		void ShowError(Exception ex, string message);

		void ShowMessage(string message);

		void ShowMessage(string message, string caption);

		DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

		bool ShowToolWindow(Guid toolWindow);
	}
}
