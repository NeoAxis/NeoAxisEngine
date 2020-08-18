using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
	public abstract class Menu : Component
	{
		public class MenuItemCollection : IList, ICollection, IEnumerable
		{
			public virtual MenuItem this[int index]
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

			public virtual MenuItem this[string key]
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

			public MenuItemCollection(Menu owner)
			{
				throw null;
			}

			public virtual MenuItem Add(string caption)
			{
				throw null;
			}

			public virtual MenuItem Add(string caption, EventHandler onClick)
			{
				throw null;
			}

			public virtual MenuItem Add(string caption, MenuItem[] items)
			{
				throw null;
			}

			public virtual int Add(MenuItem item)
			{
				throw null;
			}

			public virtual int Add(int index, MenuItem item)
			{
				throw null;
			}

			public virtual void AddRange(MenuItem[] items)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			public bool Contains(MenuItem value)
			{
				throw null;
			}

			bool IList.Contains(object value)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			public MenuItem[] Find(string key, bool searchAllChildren)
			{
				throw null;
			}

			public int IndexOf(MenuItem value)
			{
				throw null;
			}

			int IList.IndexOf(object value)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			void IList.Insert(int index, object value)
			{
				throw null;
			}

			public virtual void Clear()
			{
				throw null;
			}

			public void CopyTo(Array dest, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			public virtual void RemoveAt(int index)
			{
				throw null;
			}

			public virtual void RemoveByKey(string key)
			{
				throw null;
			}

			public virtual void Remove(MenuItem item)
			{
				throw null;
			}

			void IList.Remove(object value)
			{
				throw null;
			}
		}

		public const int FindHandle = 0;

		public const int FindShortcut = 1;

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public virtual bool IsParent
		{
			get
			{
				throw null;
			}
		}

		public MenuItem MdiListItem
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

		public MenuItemCollection MenuItems
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

		protected Menu(MenuItem[] items)
		{
			throw null;
		}

		protected virtual IntPtr CreateMenuHandle()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public MenuItem FindMenuItem(int type, IntPtr value)
		{
			throw null;
		}

		protected int FindMergePosition(int mergeOrder)
		{
			throw null;
		}

		public ContextMenu GetContextMenu()
		{
			throw null;
		}

		public MainMenu GetMainMenu()
		{
			throw null;
		}

		public virtual void MergeMenu(Menu menuSrc)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
