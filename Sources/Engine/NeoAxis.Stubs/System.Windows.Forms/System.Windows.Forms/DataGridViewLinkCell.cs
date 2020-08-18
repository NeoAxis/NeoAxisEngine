using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridViewLinkCell : DataGridViewCell
	{
		protected class DataGridViewLinkCellAccessibleObject : DataGridViewCellAccessibleObject
		{
			public override string DefaultAction
			{
				get
				{
					throw null;
				}
			}

			public DataGridViewLinkCellAccessibleObject(DataGridViewCell owner)
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

		public Color ActiveLinkColor
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

		public LinkBehavior LinkBehavior
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

		public Color LinkColor
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

		public bool LinkVisited
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

		public bool TrackVisitedState
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

		public bool UseColumnTextForLinkValue
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

		public Color VisitedLinkColor
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

		public DataGridViewLinkCell()
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

		protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
		{
			throw null;
		}

		protected override object GetValue(int rowIndex)
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

		protected override bool MouseLeaveUnsharesRow(int rowIndex)
		{
			throw null;
		}

		protected override bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
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

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
