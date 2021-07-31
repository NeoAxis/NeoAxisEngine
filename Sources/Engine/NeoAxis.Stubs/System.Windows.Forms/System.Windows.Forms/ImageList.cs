using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public sealed class ImageList : Component
	{
		public sealed class ImageCollection : IList, ICollection, IEnumerable
		{
			public StringCollection Keys
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

			public bool Empty
			{
				get
				{
					throw null;
				}
			}

			public Image this[int index]
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

			public Image this[string key]
			{
				get
				{
					throw null;
				}
			}

			public void Add(string key, Image image)
			{
				throw null;
			}

			public void Add(string key, Icon icon)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			public void Add(Icon value)
			{
				throw null;
			}

			public void Add(Image value)
			{
				throw null;
			}

			public int Add(Image value, Color transparentColor)
			{
				throw null;
			}

			public void AddRange(Image[] images)
			{
				throw null;
			}

			public int AddStrip(Image value)
			{
				throw null;
			}

			public void Clear()
			{
				throw null;
			}

			public bool Contains(Image image)
			{
				throw null;
			}

			bool IList.Contains(object image)
			{
				throw null;
			}

			public bool ContainsKey(string key)
			{
				throw null;
			}

			public int IndexOf(Image image)
			{
				throw null;
			}

			int IList.IndexOf(object image)
			{
				throw null;
			}

			public int IndexOfKey(string key)
			{
				throw null;
			}

			void IList.Insert(int index, object value)
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

			public void Remove(Image image)
			{
				throw null;
			}

			void IList.Remove(object image)
			{
				throw null;
			}

			public void RemoveAt(int index)
			{
				throw null;
			}

			public void RemoveByKey(string key)
			{
				throw null;
			}

			public void SetKeyName(int index, string name)
			{
				throw null;
			}
		}

		public ColorDepth ColorDepth
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

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public bool HandleCreated
		{
			get
			{
				throw null;
			}
		}

		public ImageCollection Images
		{
			get
			{
				throw null;
			}
		}

		public Size ImageSize
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

		public ImageListStreamer ImageStream
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

		public Color TransparentColor
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

		public event EventHandler RecreateHandle
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

		public ImageList()
		{
			throw null;
		}

		public ImageList(IContainer container)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public void Draw(Graphics g, Point pt, int index)
		{
			throw null;
		}

		public void Draw(Graphics g, int x, int y, int index)
		{
			throw null;
		}

		public void Draw(Graphics g, int x, int y, int width, int height, int index)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
