using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewRow : DataGridViewBand
	{
		protected class DataGridViewRowAccessibleObject : AccessibleObject
		{
			public override Rectangle Bounds
			{
				get
				{
					throw null;
				}
			}

			public override string Name
			{
				get
				{
					throw null;
				}
			}

			public DataGridViewRow Owner
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

			public override AccessibleObject Parent
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleRole Role
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleStates State
			{
				get
				{
					throw null;
				}
			}

			public override string Value
			{
				get
				{
					throw null;
				}
			}

			public DataGridViewRowAccessibleObject()
			{
				throw null;
			}

			public DataGridViewRowAccessibleObject(DataGridViewRow owner)
			{
				throw null;
			}

			public override AccessibleObject GetChild(int index)
			{
				throw null;
			}

			public override int GetChildCount()
			{
				throw null;
			}

			public override AccessibleObject GetSelected()
			{
				throw null;
			}

			public override AccessibleObject GetFocused()
			{
				throw null;
			}

			public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
			{
				throw null;
			}

			public override void Select(AccessibleSelection flags)
			{
				throw null;
			}
		}

		public AccessibleObject AccessibilityObject
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellCollection Cells
		{
			get
			{
				throw null;
			}
		}

		public override ContextMenuStrip ContextMenuStrip
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

		public object DataBoundItem
		{
			get
			{
				throw null;
			}
		}

		public override DataGridViewCellStyle DefaultCellStyle
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

		public override bool Displayed
		{
			get
			{
				throw null;
			}
		}

		public int DividerHeight
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

		public string ErrorText
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

		public override bool Frozen
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

		public DataGridViewRowHeaderCell HeaderCell
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

		public int Height
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

		public override DataGridViewCellStyle InheritedStyle
		{
			get
			{
				throw null;
			}
		}

		public bool IsNewRow
		{
			get
			{
				throw null;
			}
		}

		public int MinimumHeight
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

		public override bool ReadOnly
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

		public override DataGridViewTriState Resizable
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

		public override bool Selected
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

		public override DataGridViewElementStates State
		{
			get
			{
				throw null;
			}
		}

		public override bool Visible
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

		public DataGridViewRow()
		{
			throw null;
		}

		public virtual DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedRow, bool isLastVisibleRow)
		{
			throw null;
		}

		public override object Clone()
		{
			throw null;
		}

		protected virtual AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public void CreateCells(DataGridView dataGridView)
		{
			throw null;
		}

		public void CreateCells(DataGridView dataGridView, params object[] values)
		{
			throw null;
		}

		protected virtual DataGridViewCellCollection CreateCellsInstance()
		{
			throw null;
		}

		public ContextMenuStrip GetContextMenuStrip(int rowIndex)
		{
			throw null;
		}

		public string GetErrorText(int rowIndex)
		{
			throw null;
		}

		public virtual int GetPreferredHeight(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
		{
			throw null;
		}

		public virtual DataGridViewElementStates GetState(int rowIndex)
		{
			throw null;
		}

		public bool SetValues(params object[] values)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
