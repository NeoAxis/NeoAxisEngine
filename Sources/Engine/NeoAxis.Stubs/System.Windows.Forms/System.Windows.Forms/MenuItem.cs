namespace System.Windows.Forms
{
	public class MenuItem : Menu
	{
		public bool BarBreak
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

		public bool Break
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

		public bool DefaultItem
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

		public bool OwnerDraw
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

		public bool Enabled
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
			set
			{
				throw null;
			}
		}

		public override bool IsParent
		{
			get
			{
				throw null;
			}
		}

		public bool MdiList
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

		protected int MenuID
		{
			get
			{
				throw null;
			}
		}

		public MenuMerge MergeType
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

		public int MergeOrder
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

		public char Mnemonic
		{
			get
			{
				throw null;
			}
		}

		public Menu Parent
		{
			get
			{
				throw null;
			}
		}

		public bool RadioCheck
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

		public Shortcut Shortcut
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

		public bool ShowShortcut
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

		public bool Visible
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

		public event EventHandler Click
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

		public event MeasureItemEventHandler MeasureItem
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

		public event EventHandler Popup
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

		public event EventHandler Select
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

		public MenuItem()
			:base(null)
		{
			throw null;
		}

		public MenuItem(string text)
			: base( null )
		{
			throw null;
		}

		public MenuItem(string text, EventHandler onClick)
			: base( null )
		{
			throw null;
		}

		public MenuItem(string text, EventHandler onClick, Shortcut shortcut)
			: base( null )
		{
			throw null;
		}

		public MenuItem(string text, MenuItem[] items)
			: base( null )
		{
			throw null;
		}

		public MenuItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect, MenuItem[] items)
			: base( null )
		{
			throw null;
		}

		public virtual MenuItem CloneMenu()
		{
			throw null;
		}

		protected void CloneMenu(MenuItem itemSrc)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public virtual MenuItem MergeMenu()
		{
			throw null;
		}

		public void MergeMenu(MenuItem itemSrc)
		{
			throw null;
		}

		protected virtual void OnClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDrawItem(DrawItemEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMeasureItem(MeasureItemEventArgs e)
		{
			throw null;
		}

		protected virtual void OnPopup(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelect(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnInitMenuPopup(EventArgs e)
		{
			throw null;
		}

		public void PerformClick()
		{
			throw null;
		}

		public virtual void PerformSelect()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
