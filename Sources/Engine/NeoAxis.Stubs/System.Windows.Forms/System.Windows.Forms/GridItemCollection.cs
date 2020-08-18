using System.Collections;

namespace System.Windows.Forms
{
	public class GridItemCollection : ICollection, IEnumerable
	{
		public static GridItemCollection Empty;

		public int Count
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

		public GridItem this[int index]
		{
			get
			{
				throw null;
			}
		}

		public GridItem this[string label]
		{
			get
			{
				throw null;
			}
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			throw null;
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}
	}
}
