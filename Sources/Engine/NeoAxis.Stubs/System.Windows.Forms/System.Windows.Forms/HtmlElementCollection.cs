using System.Collections;

namespace System.Windows.Forms
{
	public sealed class HtmlElementCollection : ICollection, IEnumerable
	{
		public HtmlElement this[int index]
		{
			get
			{
				throw null;
			}
		}

		public HtmlElement this[string elementId]
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

		public HtmlElementCollection GetElementsByName(string name)
		{
			throw null;
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
