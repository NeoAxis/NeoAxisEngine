using System.Collections;
using System.Drawing;

namespace System.Windows.Forms
{
	public class StatusBar : Control
	{
		public class StatusBarPanelCollection : IList, ICollection, IEnumerable
		{
			public virtual StatusBarPanel this[int index]
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

			public virtual StatusBarPanel this[string key]
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

			public StatusBarPanelCollection(StatusBar owner)
			{
				throw null;
			}

			public virtual StatusBarPanel Add(string text)
			{
				throw null;
			}

			public virtual int Add(StatusBarPanel value)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			public virtual void AddRange(StatusBarPanel[] panels)
			{
				throw null;
			}

			public bool Contains(StatusBarPanel panel)
			{
				throw null;
			}

			bool IList.Contains(object panel)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			public int IndexOf(StatusBarPanel panel)
			{
				throw null;
			}

			int IList.IndexOf(object panel)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public virtual void Insert(int index, StatusBarPanel value)
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

			public virtual void Remove(StatusBarPanel value)
			{
				throw null;
			}

			void IList.Remove(object value)
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

			void ICollection.CopyTo(Array dest, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}
		}

		public override Color BackColor
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

		public override Image BackgroundImage
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

		public override ImageLayout BackgroundImageLayout
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		protected override ImeMode DefaultImeMode
		{
			get
			{
				throw null;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		protected override bool DoubleBuffered
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

		public override DockStyle Dock
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

		public override Font Font
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

		public override Color ForeColor
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

		public new ImeMode ImeMode
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

		public StatusBarPanelCollection Panels
		{
			get
			{
				throw null;
			}
		}

		public override string Text
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

		public bool ShowPanels
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

		public bool SizingGrip
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

		public new bool TabStop
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

		public new event EventHandler BackColorChanged
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

		public new event EventHandler BackgroundImageChanged
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

		public new event EventHandler BackgroundImageLayoutChanged
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

		public new event EventHandler ForeColorChanged
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

		public new event EventHandler ImeModeChanged
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

		public event StatusBarDrawItemEventHandler DrawItem
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

		public event StatusBarPanelClickEventHandler PanelClick
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

		public new event PaintEventHandler Paint
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

		public StatusBar()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
