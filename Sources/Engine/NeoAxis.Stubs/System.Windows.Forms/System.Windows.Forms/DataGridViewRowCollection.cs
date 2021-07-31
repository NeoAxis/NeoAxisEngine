using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class DataGridViewRowCollection : ICollection, IEnumerable, IList
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

		public int Count
		{
			get
			{
				throw null;
			}
		}

		protected ArrayList List
		{
			get
			{
				throw null;
			}
		}

		protected DataGridView DataGridView
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewRow this[int index]
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

		public DataGridViewRowCollection(DataGridView dataGridView)
		{
			throw null;
		}

		public DataGridViewRow SharedRow(int rowIndex)
		{
			throw null;
		}

		public virtual int Add()
		{
			throw null;
		}

		public virtual int Add(params object[] values)
		{
			throw null;
		}

		public virtual int Add(DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public virtual int Add(int count)
		{
			throw null;
		}

		public virtual int AddCopy(int indexSource)
		{
			throw null;
		}

		public virtual int AddCopies(int indexSource, int count)
		{
			throw null;
		}

		public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
		{
			throw null;
		}

		public virtual void Clear()
		{
			throw null;
		}

		public virtual bool Contains(DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public void CopyTo(DataGridViewRow[] array, int index)
		{
			throw null;
		}

		public int GetFirstRow(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetFirstRow(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public int GetLastRow(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public int GetRowCount(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetRowsHeight(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public virtual DataGridViewElementStates GetRowState(int rowIndex)
		{
			throw null;
		}

		public int IndexOf(DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public virtual void Insert(int rowIndex, params object[] values)
		{
			throw null;
		}

		public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public virtual void Insert(int rowIndex, int count)
		{
			throw null;
		}

		public virtual void InsertCopy(int indexSource, int indexDestination)
		{
			throw null;
		}

		public virtual void InsertCopies(int indexSource, int indexDestination, int count)
		{
			throw null;
		}

		public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
		{
			throw null;
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			throw null;
		}

		public virtual void Remove(DataGridViewRow dataGridViewRow)
		{
			throw null;
		}

		public virtual void RemoveAt(int index)
		{
			throw null;
		}
	}
}
