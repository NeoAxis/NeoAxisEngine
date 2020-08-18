using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms
{
	[DefaultMember("Item")]
	public class CurrencyManager : BindingManagerBase
	{
		public override int Count
		{
			get
			{
				throw null;
			}
		}

		public override object Current
		{
			get
			{
				throw null;
			}
		}

		public IList List
		{
			get
			{
				throw null;
			}
		}

		public override int Position
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

		public event ItemChangedEventHandler ItemChanged
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

		public event ListChangedEventHandler ListChanged
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

		public event EventHandler MetaDataChanged
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

		public override void AddNew()
		{
			throw null;
		}

		public override void CancelCurrentEdit()
		{
			throw null;
		}

		protected void CheckEmpty()
		{
			throw null;
		}

		public override void RemoveAt(int index)
		{
			throw null;
		}

		public override void EndCurrentEdit()
		{
			throw null;
		}

		public override PropertyDescriptorCollection GetItemProperties()
		{
			throw null;
		}

		protected virtual void OnItemChanged(ItemChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPositionChanged(EventArgs e)
		{
			throw null;
		}

		public void Refresh()
		{
			throw null;
		}

		public override void ResumeBinding()
		{
			throw null;
		}

		public override void SuspendBinding()
		{
			throw null;
		}

		protected override void UpdateIsBinding()
		{
			throw null;
		}
	}
}
