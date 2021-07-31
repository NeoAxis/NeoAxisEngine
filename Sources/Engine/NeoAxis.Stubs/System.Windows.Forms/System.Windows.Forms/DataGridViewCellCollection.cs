using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class DataGridViewCellCollection : BaseCollection, IList, ICollection, IEnumerable
	{
		bool IList.IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		object IList.this[int index]
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

		int ICollection.Count
		{
			get
			{
				throw null;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		object ICollection.SyncRoot
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

		public DataGridViewCell this[int index]
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

		public DataGridViewCell this[string columnName]
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

		int IList.Add(object value)
		{
			throw null;
		}

		void IList.Clear()
		{
			throw null;
		}

		bool IList.Contains(object value)
		{
			throw null;
		}

		int IList.IndexOf(object value)
		{
			throw null;
		}

		void IList.Insert(int index, object value)
		{
			throw null;
		}

		void IList.Remove(object value)
		{
			throw null;
		}

		void IList.RemoveAt(int index)
		{
			throw null;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw null;
		}

		public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public virtual int Add(DataGridViewCell dataGridViewCell)
		{
			throw null;
		}

		public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
		{
			throw null;
		}

		public virtual void Clear()
		{
			throw null;
		}

		public void CopyTo(DataGridViewCell[] array, int index)
		{
			throw null;
		}

		public virtual bool Contains(DataGridViewCell dataGridViewCell)
		{
			throw null;
		}

		public int IndexOf(DataGridViewCell dataGridViewCell)
		{
			throw null;
		}

		public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
		{
			throw null;
		}

		protected void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			throw null;
		}

		public virtual void Remove(DataGridViewCell cell)
		{
			throw null;
		}

		public virtual void RemoveAt(int index)
		{
			throw null;
		}
	}
}
