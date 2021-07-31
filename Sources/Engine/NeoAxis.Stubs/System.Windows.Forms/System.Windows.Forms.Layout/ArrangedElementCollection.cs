using System.Collections;
using System.Reflection;

namespace System.Windows.Forms.Layout
{
	[DefaultMember("Item")]
	public class ArrangedElementCollection : IList, ICollection, IEnumerable
	{
		bool IList.IsFixedSize
		{
			get
			{
				throw null;
			}
		}

		public virtual bool IsReadOnly
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

		public virtual int Count
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

		bool ICollection.IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		public override bool Equals(object obj)
		{
			throw null;
		}

		public override int GetHashCode()
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

		void IList.RemoveAt(int index)
		{
			throw null;
		}

		void IList.Remove(object value)
		{
			throw null;
		}

		int IList.Add(object value)
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

		public void CopyTo(Array array, int index)
		{
			throw null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			throw null;
		}
	}
}
