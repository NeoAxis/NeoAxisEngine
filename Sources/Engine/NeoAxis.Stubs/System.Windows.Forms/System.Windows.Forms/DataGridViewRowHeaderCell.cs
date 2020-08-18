using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewRowHeaderCell : DataGridViewHeaderCell
	{
		protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public override Rectangle Bounds
			{
				get
				{
					throw null;
				}
			}

			public override string DefaultAction
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

			public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell owner)
			{
				throw null;
			}

			public override void DoDefaultAction()
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

		public DataGridViewRowHeaderCell()
		{
			throw null;
		}

		public override object Clone()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
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

		public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
		{
			throw null;
		}

		public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
		{
			throw null;
		}

		protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
		{
			throw null;
		}

		protected override object GetValue(int rowIndex)
		{
			throw null;
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		protected override bool SetValue(int rowIndex, object value)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
