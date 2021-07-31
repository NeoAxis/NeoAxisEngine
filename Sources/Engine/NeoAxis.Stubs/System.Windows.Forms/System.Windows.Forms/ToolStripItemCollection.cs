using System.Collections;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class ToolStripItemCollection : ArrangedElementCollection, IList, ICollection, IEnumerable
	{
		public virtual ToolStripItem this[int index]
		{
			get
			{
				throw null;
			}
		}

		public virtual ToolStripItem this[string key]
		{
			get
			{
				throw null;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				throw null;
			}
		}

		bool IList.IsFixedSize
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

		public ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value)
		{
			throw null;
		}

		public ToolStripItem Add(string text)
		{
			throw null;
		}

		public ToolStripItem Add(Image image)
		{
			throw null;
		}

		public ToolStripItem Add(string text, Image image)
		{
			throw null;
		}

		public ToolStripItem Add(string text, Image image, EventHandler onClick)
		{
			throw null;
		}

		public int Add(ToolStripItem value)
		{
			throw null;
		}

		public void AddRange(ToolStripItem[] toolStripItems)
		{
			throw null;
		}

		public void AddRange(ToolStripItemCollection toolStripItems)
		{
			throw null;
		}

		public bool Contains(ToolStripItem value)
		{
			throw null;
		}

		public virtual void Clear()
		{
			throw null;
		}

		public virtual bool ContainsKey(string key)
		{
			throw null;
		}

		public ToolStripItem[] Find(string key, bool searchAllChildren)
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

		public void Insert(int index, ToolStripItem value)
		{
			throw null;
		}

		public int IndexOf(ToolStripItem value)
		{
			throw null;
		}

		public virtual int IndexOfKey(string key)
		{
			throw null;
		}

		public void Remove(ToolStripItem value)
		{
			throw null;
		}

		public void RemoveAt(int index)
		{
			throw null;
		}

		public virtual void RemoveByKey(string key)
		{
			throw null;
		}

		public void CopyTo(ToolStripItem[] array, int index)
		{
			throw null;
		}
	}
}
