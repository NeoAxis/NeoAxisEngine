using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewComboBoxCell : DataGridViewCell
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

			public ObjectCollection(DataGridViewComboBoxCell owner)
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

			public void AddRange(params object[] items)
			{
				throw null;
			}

			public void AddRange(ObjectCollection value)
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

			public void Remove(object value)
			{
				throw null;
			}

			public void RemoveAt(int index)
			{
				throw null;
			}
		}

		protected class DataGridViewComboBoxCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public DataGridViewComboBoxCellAccessibleObject(DataGridViewCell owner)
			{
				throw null;
			}
		}

		public virtual bool AutoComplete
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

		public virtual object DataSource
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

		public virtual string DisplayMember
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

		public DataGridViewComboBoxDisplayStyle DisplayStyle
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

		public bool DisplayStyleForCurrentCellOnly
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

		public virtual int DropDownWidth
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

		public override Type EditType
		{
			get
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

		public override Type FormattedValueType
		{
			get
			{
				throw null;
			}
		}

		public virtual ObjectCollection Items
		{
			get
			{
				throw null;
			}
		}

		public virtual int MaxDropDownItems
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

		public virtual bool Sorted
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

		public virtual string ValueMember
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

		public override Type ValueType
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewComboBoxCell()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public override object Clone()
		{
			throw null;
		}

		public override void DetachEditingControl()
		{
			throw null;
		}

		protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
		{
			throw null;
		}

		protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
		{
			throw null;
		}

		protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
		{
			throw null;
		}

		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			throw null;
		}

		public override bool KeyEntersEditMode(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnDataGridViewChanged()
		{
			throw null;
		}

		protected override void OnEnter(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected override void OnLeave(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseEnter(int rowIndex)
		{
			throw null;
		}

		protected override void OnMouseLeave(int rowIndex)
		{
			throw null;
		}

		protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
