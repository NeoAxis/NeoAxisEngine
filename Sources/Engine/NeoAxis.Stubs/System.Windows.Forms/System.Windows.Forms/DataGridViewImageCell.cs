using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewImageCell : DataGridViewCell
	{
		protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public override string DefaultAction
			{
				get
				{
					throw null;
				}
			}

			public override string Description
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
				set
				{
					throw null;
				}
			}

			public DataGridViewImageCellAccessibleObject(DataGridViewCell owner)
			{
				throw null;
			}

			public override void DoDefaultAction()
			{
				throw null;
			}

			public override int GetChildCount()
			{
				throw null;
			}
		}

		public override object DefaultNewRowValue
		{
			get
			{
				throw null;
			}
		}

		public string Description
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

		public override Type FormattedValueType
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewImageCellLayout ImageLayout
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

		public bool ValueIsIcon
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
			set
			{
				throw null;
			}
		}

		public DataGridViewImageCell()
		{
			throw null;
		}

		public DataGridViewImageCell(bool valueIsIcon)
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

		protected override object GetValue(int rowIndex)
		{
			throw null;
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
