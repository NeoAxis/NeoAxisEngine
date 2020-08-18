using System.Collections;
using System.Drawing.Design;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
	public class CollectionEditor : System.Drawing.Design.UITypeEditor
	{
		protected abstract class CollectionForm : Form
		{
			protected Type CollectionItemType
			{
				get
				{
					throw null;
				}
			}

			protected Type CollectionType
			{
				get
				{
					throw null;
				}
			}

			protected ITypeDescriptorContext Context
			{
				get
				{
					throw null;
				}
			}

			public object EditValue
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

			protected object[] Items
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

			protected Type[] NewItemTypes
			{
				get
				{
					throw null;
				}
			}

			public CollectionForm(CollectionEditor editor)
			{
				throw null;
			}

			protected bool CanRemoveInstance(object value)
			{
				throw null;
			}

			protected virtual bool CanSelectMultipleInstances()
			{
				throw null;
			}

			protected object CreateInstance(Type itemType)
			{
				throw null;
			}

			protected void DestroyInstance(object instance)
			{
				throw null;
			}

			protected virtual void DisplayError(Exception e)
			{
				throw null;
			}

			protected override object GetService(Type serviceType)
			{
				throw null;
			}

			protected abstract void OnEditValueChanged();
		}

		protected Type CollectionItemType
		{
			get
			{
				throw null;
			}
		}

		protected Type CollectionType
		{
			get
			{
				throw null;
			}
		}

		protected ITypeDescriptorContext Context
		{
			get
			{
				throw null;
			}
		}

		protected Type[] NewItemTypes
		{
			get
			{
				throw null;
			}
		}

		protected virtual string HelpTopic
		{
			get
			{
				throw null;
			}
		}

		protected virtual void CancelChanges()
		{
			throw null;
		}

		public CollectionEditor(Type type)
		{
			throw null;
		}

		protected virtual bool CanRemoveInstance(object value)
		{
			throw null;
		}

		protected virtual bool CanSelectMultipleInstances()
		{
			throw null;
		}

		protected virtual CollectionForm CreateCollectionForm()
		{
			throw null;
		}

		protected virtual object CreateInstance(Type itemType)
		{
			throw null;
		}

		protected virtual IList GetObjectsFromInstance(object instance)
		{
			throw null;
		}

		protected virtual string GetDisplayText(object value)
		{
			throw null;
		}

		protected virtual Type CreateCollectionItemType()
		{
			throw null;
		}

		protected virtual Type[] CreateNewItemTypes()
		{
			throw null;
		}

		protected virtual void DestroyInstance(object instance)
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

		protected virtual object[] GetItems(object editValue)
		{
			throw null;
		}

		protected object GetService(Type serviceType)
		{
			throw null;
		}

		protected virtual object SetItems(object editValue, object[] value)
		{
			throw null;
		}

		protected virtual void ShowHelp()
		{
			throw null;
		}
	}
}
