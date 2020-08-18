using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class GridTableStylesCollection : BaseCollection, IList, ICollection, IEnumerable
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

		public DataGridTableStyle this[int index]
		{
			get
			{
				throw null;
			}
		}

		public DataGridTableStyle this[string tableName]
		{
			get
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

		public virtual int Add(DataGridTableStyle table)
		{
			throw null;
		}

		public virtual void AddRange(DataGridTableStyle[] tables)
		{
			throw null;
		}

		public void Clear()
		{
			throw null;
		}

		public bool Contains(DataGridTableStyle table)
		{
			throw null;
		}

		public bool Contains(string name)
		{
			throw null;
		}

		protected void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			throw null;
		}

		public void Remove(DataGridTableStyle table)
		{
			throw null;
		}

		public void RemoveAt(int index)
		{
			throw null;
		}
	}
}
