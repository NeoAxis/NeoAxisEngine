using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class BindingsCollection : BaseCollection
	{
		public override int Count
		{
			get
			{
				throw null;
			}
		}

		protected override ArrayList List
		{
			get
			{
				throw null;
			}
		}

		public Binding this[int index]
		{
			get
			{
				throw null;
			}
		}

		public event CollectionChangeEventHandler CollectionChanging
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

		public event CollectionChangeEventHandler CollectionChanged
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

		protected virtual void AddCore(Binding dataBinding)
		{
			throw null;
		}

		protected virtual void ClearCore()
		{
			throw null;
		}

		protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			throw null;
		}

		protected virtual void RemoveCore(Binding dataBinding)
		{
			throw null;
		}
	}
}
