using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewTextBoxCell : DataGridViewCell
	{
		protected class DataGridViewTextBoxCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public DataGridViewTextBoxCellAccessibleObject(DataGridViewCell owner)
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

		public virtual int MaxInputLength
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

		public DataGridViewTextBoxCell()
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

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public override void PositionEditingControl(bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
