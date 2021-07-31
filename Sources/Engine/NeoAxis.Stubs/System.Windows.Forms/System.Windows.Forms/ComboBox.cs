using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class ComboBox : ListControl
	{
		public class ObjectCollection : IList, ICollection, IEnumerable
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

			public virtual object this[int index]
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

			public ObjectCollection(ComboBox owner)
			{
				throw null;
			}

			public int Add(object item)
			{
				throw null;
			}

			int IList.Add(object item)
			{
				throw null;
			}

			public void AddRange(object[] items)
			{
				throw null;
			}

			public void Clear()
			{
				throw null;
			}

			public bool Contains(object value)
			{
				throw null;
			}

			public void CopyTo(object[] destination, int arrayIndex)
			{
				throw null;
			}

			void ICollection.CopyTo(Array destination, int index)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			public int IndexOf(object value)
			{
				throw null;
			}

			public void Insert(int index, object item)
			{
				throw null;
			}

			public void RemoveAt(int index)
			{
				throw null;
			}

			public void Remove(object value)
			{
				throw null;
			}
		}

		public class ChildAccessibleObject : AccessibleObject
		{
			public override string Name
			{
				get
				{
					throw null;
				}
			}

			public ChildAccessibleObject(ComboBox owner, IntPtr handle)
			{
				throw null;
			}
		}

		public AutoCompleteMode AutoCompleteMode
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

		public AutoCompleteSource AutoCompleteSource
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

		public AutoCompleteStringCollection AutoCompleteCustomSource
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

		protected override CreateParams CreateParams
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

		public new object DataSource
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

		public DrawMode DrawMode
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

		public int DropDownWidth
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

		public int DropDownHeight
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

		public bool DroppedDown
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

		public FlatStyle FlatStyle
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

		public override bool Focused
		{
			get
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

		public bool IntegralHeight
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

		public int ItemHeight
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

		public ObjectCollection Items
		{
			get
			{
				throw null;
			}
		}

		public int MaxDropDownItems
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

		public override Size MaximumSize
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

		public override Size MinimumSize
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

		public int MaxLength
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

		public new Padding Padding
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

		public int PreferredHeight
		{
			get
			{
				throw null;
			}
		}

		public override int SelectedIndex
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

		public object SelectedItem
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

		public string SelectedText
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

		public int SelectionLength
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

		public int SelectionStart
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

		public bool Sorted
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

		public ComboBoxStyle DropDownStyle
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

		public new event EventHandler PaddingChanged
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

		public new event EventHandler DoubleClick
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

		public event EventHandler DropDown
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

		public event EventHandler SelectionChangeCommitted
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

		public event EventHandler DropDownStyleChanged
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

		public event EventHandler TextUpdate
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

		public event EventHandler DropDownClosed
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

		public ComboBox()
		{
			throw null;
		}

		protected virtual void AddItemsCore(object[] value)
		{
			throw null;
		}

		public void BeginUpdate()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public void EndUpdate()
		{
			throw null;
		}

		public int FindString(string s)
		{
			throw null;
		}

		public int FindString(string s, int startIndex)
		{
			throw null;
		}

		public int FindStringExact(string s)
		{
			throw null;
		}

		public int FindStringExact(string s, int startIndex)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}

		public int GetItemHeight(int index)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		protected override void CreateHandle()
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

		protected virtual void OnDropDown(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMeasureItem(MeasureItemEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectionChangeCommitted(EventArgs e)
		{
			throw null;
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnSelectedValueChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDropDownStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnParentBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected override void OnDataSourceChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnDisplayMemberChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDropDownClosed(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnTextUpdate(EventArgs e)
		{
			throw null;
		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			throw null;
		}

		protected override void RefreshItems()
		{
			throw null;
		}

		protected override void RefreshItem(int index)
		{
			throw null;
		}

		public override void ResetText()
		{
			throw null;
		}

		public void Select(int start, int length)
		{
			throw null;
		}

		public void SelectAll()
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected override void SetItemsCore(IList value)
		{
			throw null;
		}

		protected override void SetItemCore(int index, object value)
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
