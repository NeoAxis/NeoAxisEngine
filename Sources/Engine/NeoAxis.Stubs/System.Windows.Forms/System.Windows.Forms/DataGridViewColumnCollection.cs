using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public class DataGridViewColumnCollection : BaseCollection, IList, ICollection, IEnumerable
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

		protected DataGridView DataGridView
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewColumn this[int index]
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewColumn this[string columnName]
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

		public DataGridViewColumnCollection(DataGridView dataGridView)
		{
			throw null;
		}

		public virtual int Add(string columnName, string headerText)
		{
			throw null;
		}

		public virtual int Add(DataGridViewColumn dataGridViewColumn)
		{
			throw null;
		}

		public virtual void AddRange(params DataGridViewColumn[] dataGridViewColumns)
		{
			throw null;
		}

		public virtual void Clear()
		{
			throw null;
		}

		public virtual bool Contains(DataGridViewColumn dataGridViewColumn)
		{
			throw null;
		}

		public virtual bool Contains(string columnName)
		{
			throw null;
		}

		public void CopyTo(DataGridViewColumn[] array, int index)
		{
			throw null;
		}

		public int GetColumnCount(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public int GetColumnsWidth(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public DataGridViewColumn GetFirstColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public DataGridViewColumn GetLastColumn(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public DataGridViewColumn GetNextColumn(DataGridViewColumn dataGridViewColumnStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public DataGridViewColumn GetPreviousColumn(DataGridViewColumn dataGridViewColumnStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
		{
			throw null;
		}

		public int IndexOf(DataGridViewColumn dataGridViewColumn)
		{
			throw null;
		}

		public virtual void Insert(int columnIndex, DataGridViewColumn dataGridViewColumn)
		{
			throw null;
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			throw null;
		}

		public virtual void Remove(DataGridViewColumn dataGridViewColumn)
		{
			throw null;
		}

		public virtual void Remove(string columnName)
		{
			throw null;
		}

		public virtual void RemoveAt(int index)
		{
			throw null;
		}
	}
}
