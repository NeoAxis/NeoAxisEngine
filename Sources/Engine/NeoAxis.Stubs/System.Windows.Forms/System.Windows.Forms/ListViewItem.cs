using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
	[Serializable]
	public class ListViewItem : ICloneable, ISerializable
	{
		[Serializable]
		public class ListViewSubItem
		{
			public Color BackColor
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

			public Rectangle Bounds
			{
				get
				{
					throw null;
				}
			}

			public Font Font
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

			public Color ForeColor
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

			public object Tag
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

			public string Text
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

			public string Name
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

			public ListViewSubItem()
			{
				throw null;
			}

			public ListViewSubItem(ListViewItem owner, string text)
			{
				throw null;
			}

			public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
			{
				throw null;
			}

			public void ResetStyle()
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}
		}

		public class ListViewSubItemCollection : IList, ICollection, IEnumerable
		{
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

			bool IList.IsFixedSize
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

			public ListViewSubItem this[int index]
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

			public virtual ListViewSubItem this[string key]
			{
				get
				{
					throw null;
				}
			}

			public ListViewSubItemCollection(ListViewItem owner)
			{
				throw null;
			}

			public ListViewSubItem Add(ListViewSubItem item)
			{
				throw null;
			}

			public ListViewSubItem Add(string text)
			{
				throw null;
			}

			public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
			{
				throw null;
			}

			public void AddRange(ListViewSubItem[] items)
			{
				throw null;
			}

			public void AddRange(string[] items)
			{
				throw null;
			}

			public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
			{
				throw null;
			}

			int IList.Add(object item)
			{
				throw null;
			}

			public void Clear()
			{
				throw null;
			}

			public bool Contains(ListViewSubItem subItem)
			{
				throw null;
			}

			bool IList.Contains(object subItem)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			public int IndexOf(ListViewSubItem subItem)
			{
				throw null;
			}

			int IList.IndexOf(object subItem)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public void Insert(int index, ListViewSubItem item)
			{
				throw null;
			}

			void IList.Insert(int index, object item)
			{
				throw null;
			}

			public void Remove(ListViewSubItem item)
			{
				throw null;
			}

			void IList.Remove(object item)
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

			void ICollection.CopyTo(Array dest, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}
		}

		public Color BackColor
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

		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public bool Checked
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

		public bool Focused
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

		public Font Font
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

		public Color ForeColor
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

		public ListViewGroup Group
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

		public int ImageIndex
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

		public string ImageKey
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

		public ImageList ImageList
		{
			get
			{
				throw null;
			}
		}

		public int IndentCount
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

		public int Index
		{
			get
			{
				throw null;
			}
		}

		public ListView ListView
		{
			get
			{
				throw null;
			}
		}

		public string Name
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

		public Point Position
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

		public bool Selected
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

		public int StateImageIndex
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

		public ListViewSubItemCollection SubItems
		{
			get
			{
				throw null;
			}
		}

		public object Tag
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

		public string Text
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

		public string ToolTipText
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

		public bool UseItemStyleForSubItems
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

		public ListViewItem()
		{
			throw null;
		}

		protected ListViewItem(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public ListViewItem(string text)
		{
			throw null;
		}

		public ListViewItem(string text, int imageIndex)
		{
			throw null;
		}

		public ListViewItem(string[] items)
		{
			throw null;
		}

		public ListViewItem(string[] items, int imageIndex)
		{
			throw null;
		}

		public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font)
		{
			throw null;
		}

		public ListViewItem(ListViewSubItem[] subItems, int imageIndex)
		{
			throw null;
		}

		public ListViewItem(ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string text, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string text, int imageIndex, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string[] items, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string[] items, int imageIndex, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string text, string imageKey)
		{
			throw null;
		}

		public ListViewItem(string[] items, string imageKey)
		{
			throw null;
		}

		public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font)
		{
			throw null;
		}

		public ListViewItem(ListViewSubItem[] subItems, string imageKey)
		{
			throw null;
		}

		public ListViewItem(string text, string imageKey, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string[] items, string imageKey, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group)
		{
			throw null;
		}

		public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group)
		{
			throw null;
		}

		public void BeginEdit()
		{
			throw null;
		}

		public virtual object Clone()
		{
			throw null;
		}

		public virtual void EnsureVisible()
		{
			throw null;
		}

		public ListViewItem FindNearestItem(SearchDirectionHint searchDirection)
		{
			throw null;
		}

		public Rectangle GetBounds(ItemBoundsPortion portion)
		{
			throw null;
		}

		public ListViewSubItem GetSubItemAt(int x, int y)
		{
			throw null;
		}

		public virtual void Remove()
		{
			throw null;
		}

		protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		protected virtual void Serialize(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
