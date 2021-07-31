using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
	public abstract class ObjectSelectorEditor : System.Drawing.Design.UITypeEditor
	{
		public class Selector : TreeView
		{
			public bool clickSeen;

			public Selector(ObjectSelectorEditor editor)
			{
				throw null;
			}

			public SelectorNode AddNode(string label, object value, SelectorNode parent)
			{
				throw null;
			}

			public void Clear()
			{
				throw null;
			}

			protected void OnAfterSelect(object sender, TreeViewEventArgs e)
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

			protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
			{
				throw null;
			}

			public bool SetSelection(object value, TreeNodeCollection nodes)
			{
				throw null;
			}

			public void Start(IWindowsFormsEditorService edSvc, object value)
			{
				throw null;
			}

			public void Stop()
			{
				throw null;
			}

			protected override void WndProc(ref Message m)
			{
				throw null;
			}
		}

		public class SelectorNode : TreeNode
		{
			public object value;

			public SelectorNode(string label, object value)
			{
				throw null;
			}
		}

		public bool SubObjectSelector;

		public ObjectSelectorEditor()
		{
			throw null;
		}

		public ObjectSelectorEditor(bool subObjectSelector)
		{
			throw null;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			throw null;
		}

		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			throw null;
		}

		public bool EqualsToValue(object value)
		{
			throw null;
		}

		protected virtual void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
		{
			throw null;
		}

		public virtual void SetValue(object value)
		{
			throw null;
		}
	}
}
