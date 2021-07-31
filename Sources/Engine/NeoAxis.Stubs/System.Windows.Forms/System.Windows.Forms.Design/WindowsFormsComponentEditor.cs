using System.ComponentModel;

namespace System.Windows.Forms.Design
{
	public abstract class WindowsFormsComponentEditor : ComponentEditor
	{
		public override bool EditComponent(ITypeDescriptorContext context, object component)
		{
			throw null;
		}

		public bool EditComponent(object component, IWin32Window owner)
		{
			throw null;
		}

		public virtual bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner)
		{
			throw null;
		}

		protected virtual Type[] GetComponentEditorPages()
		{
			throw null;
		}

		protected virtual int GetInitialComponentEditorPageIndex()
		{
			throw null;
		}

		protected WindowsFormsComponentEditor()
		{
			throw null;
		}
	}
}
