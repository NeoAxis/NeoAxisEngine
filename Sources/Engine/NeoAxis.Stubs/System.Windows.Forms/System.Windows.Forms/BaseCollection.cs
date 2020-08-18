using System.Collections;

namespace System.Windows.Forms
{
	public class BaseCollection : MarshalByRefObject, ICollection, IEnumerable
	{
		public virtual int Count
		{
			get
			{
				throw null;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		public object SyncRoot
		{
			get
			{
				throw null;
			}
		}

		protected virtual ArrayList List
		{
			get
			{
				throw null;
			}
		}

		public void CopyTo(Array ar, int index)
		{
			throw null;
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}

		public BaseCollection()
		{
			throw null;
		}
	}
}
