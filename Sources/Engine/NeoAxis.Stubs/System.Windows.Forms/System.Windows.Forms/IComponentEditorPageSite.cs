namespace System.Windows.Forms
{
	public interface IComponentEditorPageSite
	{
		Control GetControl();

		void SetDirty();
	}
}
