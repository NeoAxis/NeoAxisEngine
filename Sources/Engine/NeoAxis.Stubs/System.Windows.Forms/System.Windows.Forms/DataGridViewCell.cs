using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class DataGridViewCell : DataGridViewElement, ICloneable, IDisposable
	{
		protected class DataGridViewCellAccessibleObject : AccessibleObject
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

			public override string Help
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

			public DataGridViewCell Owner
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
				set
				{
					throw null;
				}
			}

			public DataGridViewCellAccessibleObject()
			{
				throw null;
			}

			public DataGridViewCellAccessibleObject(DataGridViewCell owner)
			{
				throw null;
			}

			public override void DoDefaultAction()
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

			public override AccessibleObject GetFocused()
			{
				throw null;
			}

			public override AccessibleObject GetSelected()
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

		public int ColumnIndex
		{
			get
			{
				throw null;
			}
		}

		public Rectangle ContentBounds
		{
			get
			{
				throw null;
			}
		}

		public virtual ContextMenuStrip ContextMenuStrip
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

		public virtual object DefaultNewRowValue
		{
			get
			{
				throw null;
			}
		}

		public virtual bool Displayed
		{
			get
			{
				throw null;
			}
		}

		public object EditedFormattedValue
		{
			get
			{
				throw null;
			}
		}

		public virtual Type EditType
		{
			get
			{
				throw null;
			}
		}

		public Rectangle ErrorIconBounds
		{
			get
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

		public object FormattedValue
		{
			get
			{
				throw null;
			}
		}

		public virtual Type FormattedValueType
		{
			get
			{
				throw null;
			}
		}

		public virtual bool Frozen
		{
			get
			{
				throw null;
			}
		}

		public bool HasStyle
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewElementStates InheritedState
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellStyle InheritedStyle
		{
			get
			{
				throw null;
			}
		}

		public bool IsInEditMode
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewColumn OwningColumn
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewRow OwningRow
		{
			get
			{
				throw null;
			}
		}

		public Size PreferredSize
		{
			get
			{
				throw null;
			}
		}

		public virtual bool ReadOnly
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

		public virtual bool Resizable
		{
			get
			{
				throw null;
			}
		}

		public int RowIndex
		{
			get
			{
				throw null;
			}
		}

		public virtual bool Selected
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

		public Size Size
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellStyle Style
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

		public object Tag
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

		public string ToolTipText
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

		public object Value
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

		public virtual Type ValueType
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

		public virtual bool Visible
		{
			get
			{
				throw null;
			}
		}

		protected DataGridViewCell()
		{
			throw null;
		}

		~DataGridViewCell()
		{
			throw null;
		}

		public virtual DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
		{
			throw null;
		}

		protected virtual Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
		{
			throw null;
		}

		protected virtual bool ClickUnsharesRow(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		public virtual object Clone()
		{
			throw null;
		}

		protected virtual bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public virtual void DetachEditingControl()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		protected virtual bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual bool EnterUnsharesRow(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected virtual object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
		{
			throw null;
		}

		public Rectangle GetContentBounds(int rowIndex)
		{
			throw null;
		}

		protected virtual Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
		{
			throw null;
		}

		public object GetEditedFormattedValue(int rowIndex, DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		protected virtual Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
		{
			throw null;
		}

		protected virtual object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		public virtual ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
		{
			throw null;
		}

		public virtual DataGridViewElementStates GetInheritedState(int rowIndex)
		{
			throw null;
		}

		public virtual DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
		{
			throw null;
		}

		protected virtual Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
		{
			throw null;
		}

		protected virtual Size GetSize(int rowIndex)
		{
			throw null;
		}

		protected virtual object GetValue(int rowIndex)
		{
			throw null;
		}

		public virtual void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			throw null;
		}

		protected virtual bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		public virtual bool KeyEntersEditMode(KeyEventArgs e)
		{
			throw null;
		}

		protected virtual bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected virtual bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected virtual bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags)
		{
			throw null;
		}

		public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags, out bool widthTruncated)
		{
			throw null;
		}

		public static Size MeasureTextPreferredSize(Graphics graphics, string text, Font font, float maxRatio, TextFormatFlags flags)
		{
			throw null;
		}

		public static Size MeasureTextSize(Graphics graphics, string text, Font font, TextFormatFlags flags)
		{
			throw null;
		}

		public static int MeasureTextWidth(Graphics graphics, string text, Font font, int maxHeight, TextFormatFlags flags)
		{
			throw null;
		}

		protected virtual bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual bool MouseEnterUnsharesRow(int rowIndex)
		{
			throw null;
		}

		protected virtual bool MouseLeaveUnsharesRow(int rowIndex)
		{
			throw null;
		}

		protected virtual bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnContentClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnContentDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnEnter(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected virtual void OnKeyDown(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected virtual void OnKeyPress(KeyPressEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected virtual void OnKeyUp(KeyEventArgs e, int rowIndex)
		{
			throw null;
		}

		protected virtual void OnLeave(int rowIndex, bool throughMouseClick)
		{
			throw null;
		}

		protected virtual void OnMouseClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseDown(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseEnter(int rowIndex)
		{
			throw null;
		}

		protected virtual void OnMouseLeave(int rowIndex)
		{
			throw null;
		}

		protected virtual void OnMouseMove(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMouseUp(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected override void OnDataGridViewChanged()
		{
			throw null;
		}

		protected virtual void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			throw null;
		}

		protected virtual void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
		{
			throw null;
		}

		protected virtual void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText)
		{
			throw null;
		}

		public virtual object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
		{
			throw null;
		}

		public virtual void PositionEditingControl(bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
		{
			throw null;
		}

		public virtual Rectangle PositionEditingPanel(Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
		{
			throw null;
		}

		protected virtual bool SetValue(int rowIndex, object value)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
