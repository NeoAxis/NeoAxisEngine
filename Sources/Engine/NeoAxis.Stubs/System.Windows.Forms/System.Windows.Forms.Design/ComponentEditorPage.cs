using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
	public abstract class ComponentEditorPage : Panel
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

		protected IComponentEditorPageSite PageSite
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

		protected IComponent Component
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

		protected bool FirstActivate
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

		protected bool LoadRequired
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

		protected int Loading
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

		public bool CommitOnDeactivate
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		public Icon Icon
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

		public virtual string Title
		{
			get
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

		public ComponentEditorPage()
		{
			throw null;
		}

		public virtual void Activate()
		{
			throw null;
		}

		public virtual void ApplyChanges()
		{
			throw null;
		}

		public virtual void Deactivate()
		{
			throw null;
		}

		protected void EnterLoadingMode()
		{
			throw null;
		}

		protected void ExitLoadingMode()
		{
			throw null;
		}

		public virtual Control GetControl()
		{
			throw null;
		}

		protected IComponent GetSelectedComponent()
		{
			throw null;
		}

		public virtual bool IsPageMessage(ref Message msg)
		{
			throw null;
		}

		protected bool IsFirstActivate()
		{
			throw null;
		}

		protected bool IsLoading()
		{
			throw null;
		}

		protected abstract void LoadComponent();

		public virtual void OnApplyComplete()
		{
			throw null;
		}

		protected virtual void ReloadComponent()
		{
			throw null;
		}

		protected abstract void SaveComponent();

		protected virtual void SetDirty()
		{
			throw null;
		}

		public virtual void SetComponent(IComponent component)
		{
			throw null;
		}

		public virtual void SetSite(IComponentEditorPageSite site)
		{
			throw null;
		}

		public virtual void ShowHelp()
		{
			throw null;
		}

		public virtual bool SupportsHelp()
		{
			throw null;
		}
	}
}
