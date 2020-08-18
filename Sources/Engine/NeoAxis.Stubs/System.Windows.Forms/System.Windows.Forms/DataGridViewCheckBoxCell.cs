using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
	{
		protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public override AccessibleStates State
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

			public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell owner)
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

		public virtual object EditingCellFormattedValue
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

		public virtual bool EditingCellValueChanged
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

		public object FalseValue
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

		public override Type FormattedValueType
		{
			get
			{
				throw null;
			}
		}

		public object IndeterminateValue
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

		public bool ThreeState
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

		public object TrueValue
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

		public DataGridViewCheckBoxCell()
		{
			throw null;
		}

		public DataGridViewCheckBoxCell(bool threeState)
		{
			throw null;
		}

		public virtual object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		public virtual void PrepareEditingCellForEdit(bool selectAll)
		{
			throw null;
		}

		public override object Clone()
		{
			throw null;
		}

		protected override bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected override bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
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

		protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override bool MouseEnterUnsharesRow(int rowIndex)
		{
			throw null;
		}

		protected override bool MouseLeaveUnsharesRow(int rowIndex)
		{
			throw null;
		}

		protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override void OnContentClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected override void OnLeave(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
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

		protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
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
