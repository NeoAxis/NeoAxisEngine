namespace System.Windows.Forms.Design
{
	public class ComponentEditorForm : Form
	{
		public override bool AutoSize
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

		public new event EventHandler AutoSizeChanged
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

		public ComponentEditorForm(object component, Type[] pageTypes)
		{
			throw null;
		}

		protected override void OnActivated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHelpRequested(HelpEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelChangeSelector(object source, TreeViewEventArgs e)
		{
			throw null;
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			throw null;
		}

		public virtual DialogResult ShowForm()
		{
			throw null;
		}

		public virtual DialogResult ShowForm(int page)
		{
			throw null;
		}

		public virtual DialogResult ShowForm(IWin32Window owner)
		{
			throw null;
		}

		public virtual DialogResult ShowForm(IWin32Window owner, int page)
		{
			throw null;
		}
	}
}
