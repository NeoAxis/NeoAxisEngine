using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
	{
		protected class DataGridViewTopLeftHeaderCellAccessibleObject : DataGridViewColumnHeaderCellAccessibleObject
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

			public DataGridViewTopLeftHeaderCellAccessibleObject(DataGridViewTopLeftHeaderCell owner)
				:base(owner)
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

		public DataGridViewTopLeftHeaderCell()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
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

		protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
		{
			throw null;
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
