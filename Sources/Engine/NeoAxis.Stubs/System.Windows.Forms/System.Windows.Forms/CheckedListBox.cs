using System.Collections;

namespace System.Windows.Forms
{
	public class CheckedListBox : ListBox
	{
		public new class ObjectCollection : ListBox.ObjectCollection
		{
			public ObjectCollection(CheckedListBox owner)
				: base( owner )
			{
				throw null;
			}

			public int Add(object item, bool isChecked)
			{
				throw null;
			}

			public int Add(object item, CheckState check)
			{
				throw null;
			}
		}

		public class CheckedIndexCollection : IList, ICollection, IEnumerable
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

			public int this[int index]
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

			int IList.Add(object value)
			{
				throw null;
			}

			void IList.Clear()
			{
				throw null;
			}

			void IList.Insert(int index, object value)
			{
				throw null;
			}

			void IList.Remove(object value)
			{
				throw null;
			}

			void IList.RemoveAt(int index)
			{
				throw null;
			}

			public bool Contains(int index)
			{
				throw null;
			}

			bool IList.Contains(object index)
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

			public int IndexOf(int index)
			{
				throw null;
			}

			int IList.IndexOf(object index)
			{
				throw null;
			}
		}

		public class CheckedItemCollection : IList, ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			public object this[int index]
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

			public bool Contains(object item)
			{
				throw null;
			}

			public int IndexOf(object item)
			{
				throw null;
			}

			int IList.Add(object value)
			{
				throw null;
			}

			void IList.Clear()
			{
				throw null;
			}

			void IList.Insert(int index, object value)
			{
				throw null;
			}

			void IList.Remove(object value)
			{
				throw null;
			}

			void IList.RemoveAt(int index)
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
		}

		public bool CheckOnClick
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

		public CheckedIndexCollection CheckedIndices
		{
			get
			{
				throw null;
			}
		}

		public CheckedItemCollection CheckedItems
		{
			get
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

		public new string DisplayMember
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

		public override DrawMode DrawMode
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

		public override int ItemHeight
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

		public new ObjectCollection Items
		{
			get
			{
				throw null;
			}
		}

		public override SelectionMode SelectionMode
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

		public bool ThreeDCheckBoxes
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

		public bool UseCompatibleTextRendering
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

		public new string ValueMember
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

		public new event EventHandler DataSourceChanged
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

		public new event EventHandler DisplayMemberChanged
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

		public event ItemCheckEventHandler ItemCheck
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

		public new event EventHandler Click
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

		public new event MouseEventHandler MouseClick
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

		public new event DrawItemEventHandler DrawItem
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

		public new event MeasureItemEventHandler MeasureItem
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

		public new event EventHandler ValueMemberChanged
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

		public CheckedListBox()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override ListBox.ObjectCollection CreateItemCollection()
		{
			throw null;
		}

		public CheckState GetItemCheckState(int index)
		{
			throw null;
		}

		public bool GetItemChecked(int index)
		{
			throw null;
		}

		protected override void OnClick(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			throw null;
		}

		protected virtual void OnItemCheck(ItemCheckEventArgs ice)
		{
			throw null;
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			throw null;
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			throw null;
		}

		protected override void RefreshItems()
		{
			throw null;
		}

		public void SetItemCheckState(int index, CheckState value)
		{
			throw null;
		}

		public void SetItemChecked(int index, bool value)
		{
			throw null;
		}

		protected override void WmReflectCommand(ref Message m)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
