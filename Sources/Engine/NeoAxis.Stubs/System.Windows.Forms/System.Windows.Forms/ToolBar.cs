using System.Collections;
using System.Drawing;

namespace System.Windows.Forms
{
	public class ToolBar : Control
	{
		public class ToolBarButtonCollection : IList, ICollection, IEnumerable
		{
			public virtual ToolBarButton this[int index]
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

			public virtual ToolBarButton this[string key]
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

			public ToolBarButtonCollection(ToolBar owner)
			{
				throw null;
			}

			public int Add(ToolBarButton button)
			{
				throw null;
			}

			public int Add(string text)
			{
				throw null;
			}

			int IList.Add(object button)
			{
				throw null;
			}

			public void AddRange(ToolBarButton[] buttons)
			{
				throw null;
			}

			public void Clear()
			{
				throw null;
			}

			public bool Contains(ToolBarButton button)
			{
				throw null;
			}

			bool IList.Contains(object button)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				throw null;
			}

			public int IndexOf(ToolBarButton button)
			{
				throw null;
			}

			int IList.IndexOf(object button)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public void Insert(int index, ToolBarButton button)
			{
				throw null;
			}

			void IList.Insert(int index, object button)
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

			public void Remove(ToolBarButton button)
			{
				throw null;
			}

			void IList.Remove(object button)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}
		}

		public ToolBarAppearance Appearance
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

		public override bool AutoSize
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

		public BorderStyle BorderStyle
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

		public ToolBarButtonCollection Buttons
		{
			get
			{
				throw null;
			}
		}

		public Size ButtonSize
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

		public bool Divider
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

		public bool DropDownArrows
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

		public ImageList ImageList
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

		public Size ImageSize
		{
			get
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

		public override RightToLeft RightToLeft
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

		public bool ShowToolTips
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

		public ToolBarTextAlign TextAlign
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

		public bool Wrappable
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

		public new event EventHandler AutoSizeChanged
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

		public new event EventHandler RightToLeftChanged
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

		public new event EventHandler TextChanged
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

		public event ToolBarButtonClickEventHandler ButtonClick
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

		public event ToolBarButtonClickEventHandler ButtonDropDown
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

		public ToolBar()
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
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

		protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e)
		{
			throw null;
		}

		protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
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
