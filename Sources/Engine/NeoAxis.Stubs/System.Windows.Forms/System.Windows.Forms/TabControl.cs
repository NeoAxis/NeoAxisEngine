using System.Collections;
using System.Drawing;

namespace System.Windows.Forms
{
	public class TabControl : Control
	{
		public class TabPageCollection : IList, ICollection, IEnumerable
		{
			public virtual TabPage this[int index]
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

			public virtual TabPage this[string key]
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

			public TabPageCollection(TabControl owner)
			{
				throw null;
			}

			public void Add(TabPage value)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			public void Add(string text)
			{
				throw null;
			}

			public void Add(string key, string text)
			{
				throw null;
			}

			public void Add(string key, string text, int imageIndex)
			{
				throw null;
			}

			public void Add(string key, string text, string imageKey)
			{
				throw null;
			}

			public void AddRange(TabPage[] pages)
			{
				throw null;
			}

			public bool Contains(TabPage page)
			{
				throw null;
			}

			bool IList.Contains(object page)
			{
				throw null;
			}

			public virtual bool ContainsKey(string key)
			{
				throw null;
			}

			public int IndexOf(TabPage page)
			{
				throw null;
			}

			int IList.IndexOf(object page)
			{
				throw null;
			}

			public virtual int IndexOfKey(string key)
			{
				throw null;
			}

			public void Insert(int index, TabPage tabPage)
			{
				throw null;
			}

			void IList.Insert(int index, object tabPage)
			{
				throw null;
			}

			public void Insert(int index, string text)
			{
				throw null;
			}

			public void Insert(int index, string key, string text)
			{
				throw null;
			}

			public void Insert(int index, string key, string text, int imageIndex)
			{
				throw null;
			}

			public void Insert(int index, string key, string text, string imageKey)
			{
				throw null;
			}

			public virtual void Clear()
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

			public void Remove(TabPage value)
			{
				throw null;
			}

			void IList.Remove(object value)
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
		}

		public new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(TabControl owner)
				:base(owner)
			{
				throw null;
			}

			public override void Add(Control value)
			{
				throw null;
			}

			public override void Remove(Control value)
			{
				throw null;
			}
		}

		public TabAlignment Alignment
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

		public TabAppearance Appearance
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				throw null;
			}
		}

		public TabDrawMode DrawMode
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

		public bool HotTrack
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

		public Size ItemSize
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

		public bool Multiline
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

		public new Point Padding
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

		public virtual bool RightToLeftLayout
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

		public int RowCount
		{
			get
			{
				throw null;
			}
		}

		public int SelectedIndex
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

		public TabPage SelectedTab
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

		public TabSizeMode SizeMode
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

		public int TabCount
		{
			get
			{
				throw null;
			}
		}

		public TabPageCollection TabPages
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

		public event DrawItemEventHandler DrawItem
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

		public event EventHandler RightToLeftLayoutChanged
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

		public event EventHandler SelectedIndexChanged
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

		public event TabControlCancelEventHandler Selecting
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

		public event TabControlEventHandler Selected
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

		public event TabControlCancelEventHandler Deselecting
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

		public event TabControlEventHandler Deselected
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

		public TabControl()
		{
			throw null;
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		public void DeselectTab(int index)
		{
			throw null;
		}

		public void DeselectTab(TabPage tabPage)
		{
			throw null;
		}

		public void DeselectTab(string tabPageName)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public Control GetControl(int index)
		{
			throw null;
		}

		protected virtual object[] GetItems()
		{
			throw null;
		}

		protected virtual object[] GetItems(Type baseType)
		{
			throw null;
		}

		public Rectangle GetTabRect(int index)
		{
			throw null;
		}

		protected string GetToolTipText(object item)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
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

		protected virtual void OnDrawItem(DrawItemEventArgs e)
		{
			throw null;
		}

		protected override void OnEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs ke)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelecting(TabControlCancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelected(TabControlEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDeselecting(TabControlCancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDeselected(TabControlEventArgs e)
		{
			throw null;
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected void RemoveAll()
		{
			throw null;
		}

		public void SelectTab(int index)
		{
			throw null;
		}

		public void SelectTab(TabPage tabPage)
		{
			throw null;
		}

		public void SelectTab(string tabPageName)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected void UpdateTabSelection(bool updateFocus)
		{
			throw null;
		}

		protected override void OnStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
