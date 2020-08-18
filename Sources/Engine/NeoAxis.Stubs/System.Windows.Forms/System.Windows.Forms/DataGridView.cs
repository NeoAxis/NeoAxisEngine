using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridView : Control, ISupportInitialize
	{
		protected class DataGridViewAccessibleObject : ControlAccessibleObject
		{
			public override string Name
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

			public DataGridViewAccessibleObject(DataGridView owner)
				:base(owner)
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

			public override AccessibleObject HitTest(int x, int y)
			{
				throw null;
			}

			public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
			{
				throw null;
			}
		}

		public class DataGridViewControlCollection : ControlCollection
		{
			public DataGridViewControlCollection(DataGridView owner)
				:base(owner)
			{
				throw null;
			}

			public void CopyTo(Control[] array, int index)
			{
				throw null;
			}

			public void Insert(int index, Control value)
			{
				throw null;
			}

			public override void Remove(Control value)
			{
				throw null;
			}

			public override void Clear()
			{
				throw null;
			}
		}

		public sealed class HitTestInfo
		{
			public static readonly HitTestInfo Nowhere;

			public int ColumnIndex
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

			public int ColumnX
			{
				get
				{
					throw null;
				}
			}

			public int RowY
			{
				get
				{
					throw null;
				}
			}

			public DataGridViewHitTestType Type
			{
				get
				{
					throw null;
				}
			}

			public override bool Equals(object value)
			{
				throw null;
			}

			public override int GetHashCode()
			{
				throw null;
			}

			public override string ToString()
			{
				throw null;
			}
		}

		protected class DataGridViewTopRowAccessibleObject : AccessibleObject
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

			public DataGridView Owner
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

			public override string Value
			{
				get
				{
					throw null;
				}
			}

			public DataGridViewTopRowAccessibleObject()
			{
				throw null;
			}

			public DataGridViewTopRowAccessibleObject(DataGridView owner)
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

			public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
			{
				throw null;
			}
		}

		public virtual DataGridViewAdvancedBorderStyle AdjustedTopLeftHeaderBorderStyle
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewAdvancedBorderStyle AdvancedCellBorderStyle
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewAdvancedBorderStyle AdvancedColumnHeadersBorderStyle
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewAdvancedBorderStyle AdvancedRowHeadersBorderStyle
		{
			get
			{
				throw null;
			}
		}

		public bool AllowUserToAddRows
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

		public bool AllowUserToDeleteRows
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

		public bool AllowUserToOrderColumns
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

		public bool AllowUserToResizeColumns
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

		public bool AllowUserToResizeRows
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

		public DataGridViewCellStyle AlternatingRowsDefaultCellStyle
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

		public bool AutoGenerateColumns
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

		public override bool AutoSize
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

		public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
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

		public DataGridViewAutoSizeRowsMode AutoSizeRowsMode
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

		public override Color BackColor
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

		public Color BackgroundColor
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

		public override Image BackgroundImage
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

		public override ImageLayout BackgroundImageLayout
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

		public BorderStyle BorderStyle
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

		protected override bool CanEnableIme
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellBorderStyle CellBorderStyle
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

		public DataGridViewClipboardCopyMode ClipboardCopyMode
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

		public int ColumnCount
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

		public DataGridViewHeaderBorderStyle ColumnHeadersBorderStyle
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

		public DataGridViewCellStyle ColumnHeadersDefaultCellStyle
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

		public int ColumnHeadersHeight
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

		public DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
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

		public bool ColumnHeadersVisible
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

		public DataGridViewColumnCollection Columns
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCell CurrentCell
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

		public Point CurrentCellAddress
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewRow CurrentRow
		{
			get
			{
				throw null;
			}
		}

		public string DataMember
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

		public object DataSource
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

		public DataGridViewCellStyle DefaultCellStyle
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewEditMode EditMode
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

		public Control EditingControl
		{
			get
			{
				throw null;
			}
		}

		public Panel EditingPanel
		{
			get
			{
				throw null;
			}
		}

		public bool EnableHeadersVisualStyles
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

		public DataGridViewCell FirstDisplayedCell
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

		public int FirstDisplayedScrollingColumnHiddenWidth
		{
			get
			{
				throw null;
			}
		}

		public int FirstDisplayedScrollingColumnIndex
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

		public int FirstDisplayedScrollingRowIndex
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

		public override Color ForeColor
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

		public override Font Font
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

		public Color GridColor
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

		protected ScrollBar HorizontalScrollBar
		{
			get
			{
				throw null;
			}
		}

		public int HorizontalScrollingOffset
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

		public bool IsCurrentCellDirty
		{
			get
			{
				throw null;
			}
		}

		public bool IsCurrentCellInEditMode
		{
			get
			{
				throw null;
			}
		}

		public bool IsCurrentRowDirty
		{
			get
			{
				throw null;
			}
		}

		public bool MultiSelect
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

		public int NewRowIndex
		{
			get
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

		public bool ReadOnly
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

		public int RowCount
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

		public DataGridViewHeaderBorderStyle RowHeadersBorderStyle
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

		public DataGridViewCellStyle RowHeadersDefaultCellStyle
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

		public bool RowHeadersVisible
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

		public int RowHeadersWidth
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

		public DataGridViewRowHeadersWidthSizeMode RowHeadersWidthSizeMode
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

		public DataGridViewRowCollection Rows
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewCellStyle RowsDefaultCellStyle
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

		public DataGridViewRow RowTemplate
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

		public ScrollBars ScrollBars
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

		public DataGridViewSelectedCellCollection SelectedCells
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewSelectedColumnCollection SelectedColumns
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewSelectedRowCollection SelectedRows
		{
			get
			{
				throw null;
			}
		}

		public DataGridViewSelectionMode SelectionMode
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

		public bool ShowCellErrors
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

		public bool ShowCellToolTips
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

		public bool ShowEditingIcon
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

		public bool ShowRowErrors
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

		public DataGridViewColumn SortedColumn
		{
			get
			{
				throw null;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				throw null;
			}
		}

		public bool StandardTab
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

		public override string Text
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

		public DataGridViewCell this[int columnIndex, int rowIndex]
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

		public DataGridViewCell this[string columnName, int rowIndex]
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

		public DataGridViewHeaderCell TopLeftHeaderCell
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

		public Cursor UserSetCursor
		{
			get
			{
				throw null;
			}
		}

		protected ScrollBar VerticalScrollBar
		{
			get
			{
				throw null;
			}
		}

		public int VerticalScrollingOffset
		{
			get
			{
				throw null;
			}
		}

		public bool VirtualMode
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

		public event EventHandler AllowUserToAddRowsChanged
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

		public event EventHandler AllowUserToDeleteRowsChanged
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

		public event EventHandler AllowUserToOrderColumnsChanged
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

		public event EventHandler AllowUserToResizeColumnsChanged
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

		public event EventHandler AllowUserToResizeRowsChanged
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

		public event EventHandler AlternatingRowsDefaultCellStyleChanged
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

		public event EventHandler AutoGenerateColumnsChanged
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

		public event DataGridViewAutoSizeColumnsModeEventHandler AutoSizeColumnsModeChanged
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

		public event DataGridViewAutoSizeModeEventHandler AutoSizeRowsModeChanged
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

		public new event EventHandler BackColorChanged
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

		public event EventHandler BackgroundColorChanged
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

		public new event EventHandler BackgroundImageChanged
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

		public new event EventHandler BackgroundImageLayoutChanged
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

		public event EventHandler BorderStyleChanged
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

		public event EventHandler CellBorderStyleChanged
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

		public event EventHandler ColumnHeadersBorderStyleChanged
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

		public event EventHandler ColumnHeadersDefaultCellStyleChanged
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

		public event EventHandler ColumnHeadersHeightChanged
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

		public event DataGridViewAutoSizeModeEventHandler ColumnHeadersHeightSizeModeChanged
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

		public event EventHandler DataMemberChanged
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

		public event EventHandler DataSourceChanged
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

		public event EventHandler DefaultCellStyleChanged
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

		public event EventHandler EditModeChanged
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

		public new event EventHandler ForeColorChanged
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

		public new event EventHandler FontChanged
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

		public event EventHandler GridColorChanged
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

		public event EventHandler MultiSelectChanged
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

		public new event EventHandler PaddingChanged
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

		public event EventHandler ReadOnlyChanged
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

		public event EventHandler RowHeadersBorderStyleChanged
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

		public event EventHandler RowHeadersDefaultCellStyleChanged
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

		public event EventHandler RowHeadersWidthChanged
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

		public event DataGridViewAutoSizeModeEventHandler RowHeadersWidthSizeModeChanged
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

		public event EventHandler RowsDefaultCellStyleChanged
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

		public new event EventHandler TextChanged
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

		public event DataGridViewAutoSizeColumnModeEventHandler AutoSizeColumnModeChanged
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

		public event QuestionEventHandler CancelRowEdit
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

		public event DataGridViewCellCancelEventHandler CellBeginEdit
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

		public event DataGridViewCellEventHandler CellClick
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

		public event DataGridViewCellEventHandler CellContentClick
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

		public event DataGridViewCellEventHandler CellContentDoubleClick
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

		public event DataGridViewCellEventHandler CellContextMenuStripChanged
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

		public event DataGridViewCellContextMenuStripNeededEventHandler CellContextMenuStripNeeded
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

		public event DataGridViewCellEventHandler CellDoubleClick
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

		public event DataGridViewCellEventHandler CellEndEdit
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

		public event DataGridViewCellEventHandler CellEnter
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

		public event DataGridViewCellEventHandler CellErrorTextChanged
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

		public event DataGridViewCellErrorTextNeededEventHandler CellErrorTextNeeded
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

		public event DataGridViewCellFormattingEventHandler CellFormatting
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

		public event DataGridViewCellEventHandler CellLeave
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

		public event DataGridViewCellMouseEventHandler CellMouseClick
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

		public event DataGridViewCellMouseEventHandler CellMouseDoubleClick
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

		public event DataGridViewCellMouseEventHandler CellMouseDown
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

		public event DataGridViewCellEventHandler CellMouseEnter
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

		public event DataGridViewCellEventHandler CellMouseLeave
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

		public event DataGridViewCellMouseEventHandler CellMouseMove
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

		public event DataGridViewCellMouseEventHandler CellMouseUp
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

		public event DataGridViewCellPaintingEventHandler CellPainting
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

		public event DataGridViewCellParsingEventHandler CellParsing
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

		public event DataGridViewCellStateChangedEventHandler CellStateChanged
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

		public event DataGridViewCellEventHandler CellStyleChanged
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

		public event DataGridViewCellStyleContentChangedEventHandler CellStyleContentChanged
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

		public event DataGridViewCellEventHandler CellToolTipTextChanged
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

		public event DataGridViewCellToolTipTextNeededEventHandler CellToolTipTextNeeded
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

		public event DataGridViewCellEventHandler CellValidated
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

		public event DataGridViewCellValidatingEventHandler CellValidating
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

		public event DataGridViewCellEventHandler CellValueChanged
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

		public event DataGridViewCellValueEventHandler CellValueNeeded
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

		public event DataGridViewCellValueEventHandler CellValuePushed
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

		public event DataGridViewColumnEventHandler ColumnAdded
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

		public event DataGridViewColumnEventHandler ColumnContextMenuStripChanged
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

		public event DataGridViewColumnEventHandler ColumnDataPropertyNameChanged
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

		public event DataGridViewColumnEventHandler ColumnDefaultCellStyleChanged
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

		public event DataGridViewColumnEventHandler ColumnDisplayIndexChanged
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

		public event DataGridViewColumnDividerDoubleClickEventHandler ColumnDividerDoubleClick
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

		public event DataGridViewColumnEventHandler ColumnDividerWidthChanged
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

		public event DataGridViewCellMouseEventHandler ColumnHeaderMouseClick
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

		public event DataGridViewCellMouseEventHandler ColumnHeaderMouseDoubleClick
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

		public event DataGridViewColumnEventHandler ColumnHeaderCellChanged
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

		public event DataGridViewColumnEventHandler ColumnMinimumWidthChanged
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

		public event DataGridViewColumnEventHandler ColumnNameChanged
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

		public event DataGridViewColumnEventHandler ColumnRemoved
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

		public event DataGridViewColumnEventHandler ColumnSortModeChanged
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

		public event DataGridViewColumnStateChangedEventHandler ColumnStateChanged
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

		public event DataGridViewColumnEventHandler ColumnToolTipTextChanged
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

		public event DataGridViewColumnEventHandler ColumnWidthChanged
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

		public event EventHandler CurrentCellChanged
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

		public event EventHandler CurrentCellDirtyStateChanged
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

		public event DataGridViewBindingCompleteEventHandler DataBindingComplete
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

		public event DataGridViewDataErrorEventHandler DataError
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

		public event DataGridViewRowEventHandler DefaultValuesNeeded
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

		public event DataGridViewEditingControlShowingEventHandler EditingControlShowing
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

		public event DataGridViewRowEventHandler NewRowNeeded
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

		public event DataGridViewRowEventHandler RowContextMenuStripChanged
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

		public event DataGridViewRowContextMenuStripNeededEventHandler RowContextMenuStripNeeded
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

		public event DataGridViewRowEventHandler RowDefaultCellStyleChanged
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

		public event QuestionEventHandler RowDirtyStateNeeded
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

		public event DataGridViewRowDividerDoubleClickEventHandler RowDividerDoubleClick
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

		public event DataGridViewRowEventHandler RowDividerHeightChanged
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

		public event DataGridViewCellEventHandler RowEnter
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

		public event DataGridViewRowEventHandler RowErrorTextChanged
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

		public event DataGridViewRowErrorTextNeededEventHandler RowErrorTextNeeded
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

		public event DataGridViewCellMouseEventHandler RowHeaderMouseClick
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

		public event DataGridViewCellMouseEventHandler RowHeaderMouseDoubleClick
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

		public event DataGridViewRowEventHandler RowHeaderCellChanged
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

		public event DataGridViewRowEventHandler RowHeightChanged
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

		public event DataGridViewRowHeightInfoNeededEventHandler RowHeightInfoNeeded
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

		public event DataGridViewRowHeightInfoPushedEventHandler RowHeightInfoPushed
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

		public event DataGridViewCellEventHandler RowLeave
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

		public event DataGridViewRowEventHandler RowMinimumHeightChanged
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

		public event DataGridViewRowPostPaintEventHandler RowPostPaint
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

		public event DataGridViewRowPrePaintEventHandler RowPrePaint
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

		public event DataGridViewRowsAddedEventHandler RowsAdded
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

		public event DataGridViewRowsRemovedEventHandler RowsRemoved
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

		public event DataGridViewRowStateChangedEventHandler RowStateChanged
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

		public event DataGridViewRowEventHandler RowUnshared
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

		public event DataGridViewCellEventHandler RowValidated
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

		public event DataGridViewCellCancelEventHandler RowValidating
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

		public event ScrollEventHandler Scroll
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

		public event EventHandler SelectionChanged
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

		public event DataGridViewSortCompareEventHandler SortCompare
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

		public event EventHandler Sorted
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

		public new event EventHandler StyleChanged
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

		public event DataGridViewRowEventHandler UserAddedRow
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

		public event DataGridViewRowEventHandler UserDeletedRow
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

		public event DataGridViewRowCancelEventHandler UserDeletingRow
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

		public DataGridView()
		{
			throw null;
		}

		void ISupportInitialize.BeginInit()
		{
			throw null;
		}

		void ISupportInitialize.EndInit()
		{
			throw null;
		}

		protected virtual void AccessibilityNotifyCurrentCellChanged(Point cellAddress)
		{
			throw null;
		}

		public virtual DataGridViewAdvancedBorderStyle AdjustColumnHeaderBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, bool isFirstDisplayedColumn, bool isLastVisibleColumn)
		{
			throw null;
		}

		public bool AreAllCellsSelected(bool includeInvisibleCells)
		{
			throw null;
		}

		public void AutoResizeColumn(int columnIndex)
		{
			throw null;
		}

		public void AutoResizeColumn(int columnIndex, DataGridViewAutoSizeColumnMode autoSizeColumnMode)
		{
			throw null;
		}

		protected void AutoResizeColumn(int columnIndex, DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
		{
			throw null;
		}

		public void AutoResizeColumnHeadersHeight()
		{
			throw null;
		}

		public void AutoResizeColumnHeadersHeight(int columnIndex)
		{
			throw null;
		}

		protected void AutoResizeColumnHeadersHeight(bool fixedRowHeadersWidth, bool fixedColumnsWidth)
		{
			throw null;
		}

		protected void AutoResizeColumnHeadersHeight(int columnIndex, bool fixedRowHeadersWidth, bool fixedColumnWidth)
		{
			throw null;
		}

		public void AutoResizeColumns()
		{
			throw null;
		}

		public void AutoResizeColumns(DataGridViewAutoSizeColumnsMode autoSizeColumnsMode)
		{
			throw null;
		}

		protected void AutoResizeColumns(DataGridViewAutoSizeColumnsMode autoSizeColumnsMode, bool fixedHeight)
		{
			throw null;
		}

		public void AutoResizeRow(int rowIndex)
		{
			throw null;
		}

		public void AutoResizeRow(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode)
		{
			throw null;
		}

		protected void AutoResizeRow(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
		{
			throw null;
		}

		public void AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode)
		{
			throw null;
		}

		protected void AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool fixedColumnHeadersHeight, bool fixedRowsHeight)
		{
			throw null;
		}

		public void AutoResizeRowHeadersWidth(int rowIndex, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode)
		{
			throw null;
		}

		protected void AutoResizeRowHeadersWidth(int rowIndex, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool fixedColumnHeadersHeight, bool fixedRowHeight)
		{
			throw null;
		}

		public void AutoResizeRows()
		{
			throw null;
		}

		public void AutoResizeRows(DataGridViewAutoSizeRowsMode autoSizeRowsMode)
		{
			throw null;
		}

		protected void AutoResizeRows(DataGridViewAutoSizeRowsMode autoSizeRowsMode, bool fixedWidth)
		{
			throw null;
		}

		protected void AutoResizeRows(int rowIndexStart, int rowsCount, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
		{
			throw null;
		}

		public virtual bool BeginEdit(bool selectAll)
		{
			throw null;
		}

		public bool CancelEdit()
		{
			throw null;
		}

		public void ClearSelection()
		{
			throw null;
		}

		protected void ClearSelection(int columnIndexException, int rowIndexException, bool selectExceptionElement)
		{
			throw null;
		}

		public bool CommitEdit(DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		protected virtual DataGridViewColumnCollection CreateColumnsInstance()
		{
			throw null;
		}

		protected virtual DataGridViewRowCollection CreateRowsInstance()
		{
			throw null;
		}

		public int DisplayedColumnCount(bool includePartialColumns)
		{
			throw null;
		}

		public int DisplayedRowCount(bool includePartialRow)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public bool EndEdit()
		{
			throw null;
		}

		public bool EndEdit(DataGridViewDataErrorContexts context)
		{
			throw null;
		}

		protected override AccessibleObject GetAccessibilityObjectById(int objectId)
		{
			throw null;
		}

		public int GetCellCount(DataGridViewElementStates includeFilter)
		{
			throw null;
		}

		public Rectangle GetCellDisplayRectangle(int columnIndex, int rowIndex, bool cutOverflow)
		{
			throw null;
		}

		public virtual DataObject GetClipboardContent()
		{
			throw null;
		}

		public Rectangle GetColumnDisplayRectangle(int columnIndex, bool cutOverflow)
		{
			throw null;
		}

		public Rectangle GetRowDisplayRectangle(int rowIndex, bool cutOverflow)
		{
			throw null;
		}

		public HitTestInfo HitTest(int x, int y)
		{
			throw null;
		}

		public void InvalidateCell(DataGridViewCell dataGridViewCell)
		{
			throw null;
		}

		public void InvalidateCell(int columnIndex, int rowIndex)
		{
			throw null;
		}

		public void InvalidateColumn(int columnIndex)
		{
			throw null;
		}

		public void InvalidateRow(int rowIndex)
		{
			throw null;
		}

		protected override bool IsInputChar(char charCode)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		public virtual void NotifyCurrentCellDirty(bool dirty)
		{
			throw null;
		}

		protected virtual void OnAllowUserToAddRowsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowUserToDeleteRowsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowUserToOrderColumnsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowUserToResizeColumnsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowUserToResizeRowsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAlternatingRowsDefaultCellStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAutoGenerateColumnsChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAutoSizeColumnModeChanged(DataGridViewAutoSizeColumnModeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnAutoSizeColumnsModeChanged(DataGridViewAutoSizeColumnsModeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnAutoSizeRowsModeChanged(DataGridViewAutoSizeModeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackgroundColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCancelRowEdit(QuestionEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellContentClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellContentDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellContextMenuStripChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellContextMenuStripNeeded(DataGridViewCellContextMenuStripNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellDoubleClick(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellEndEdit(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellEnter(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellErrorTextChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellErrorTextNeeded(DataGridViewCellErrorTextNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellLeave(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseEnter(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseLeave(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseMove(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellParsing(DataGridViewCellParsingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellStyleChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellStyleContentChanged(DataGridViewCellStyleContentChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellToolTipTextChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellToolTipTextNeeded(DataGridViewCellToolTipTextNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellValidated(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellValidating(DataGridViewCellValidatingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellValueChanged(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCellValuePushed(DataGridViewCellValueEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnAdded(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnContextMenuStripChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnDataPropertyNameChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnDefaultCellStyleChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnDisplayIndexChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnDividerDoubleClick(DataGridViewColumnDividerDoubleClickEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnDividerWidthChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeaderCellChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeaderMouseDoubleClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeadersBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeadersDefaultCellStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeadersHeightChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeadersHeightSizeModeChanged(DataGridViewAutoSizeModeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnMinimumWidthChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnNameChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnRemoved(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnSortModeChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnStateChanged(DataGridViewColumnStateChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnToolTipTextChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
		{
			throw null;
		}

		protected virtual void OnCurrentCellChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCurrentCellDirtyStateChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnCursorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataMemberChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDefaultCellStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDefaultValuesNeeded(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
		{
			throw null;
		}

		protected virtual void OnEditModeChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnGridColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			throw null;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			throw null;
		}

		protected override void OnLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			throw null;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnMultiSelectChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnNewRowNeeded(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			throw null;
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowContextMenuStripChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowContextMenuStripNeeded(DataGridViewRowContextMenuStripNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowDefaultCellStyleChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowDirtyStateNeeded(QuestionEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowDividerDoubleClick(DataGridViewRowDividerDoubleClickEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowDividerHeightChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowEnter(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowErrorTextChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowErrorTextNeeded(DataGridViewRowErrorTextNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeaderCellChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeaderMouseClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeaderMouseDoubleClick(DataGridViewCellMouseEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeadersBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeadersDefaultCellStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeadersWidthChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeadersWidthSizeModeChanged(DataGridViewAutoSizeModeEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeightChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeightInfoNeeded(DataGridViewRowHeightInfoNeededEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeightInfoPushed(DataGridViewRowHeightInfoPushedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowLeave(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowMinimumHeightChanged(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowsDefaultCellStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowStateChanged(int rowIndex, DataGridViewRowStateChangedEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowUnshared(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowValidating(DataGridViewCellCancelEventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowValidated(DataGridViewCellEventArgs e)
		{
			throw null;
		}

		protected virtual void OnScroll(ScrollEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectionChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSortCompare(DataGridViewSortCompareEventArgs e)
		{
			throw null;
		}

		protected virtual void OnSorted(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnUserAddedRow(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnUserDeletedRow(DataGridViewRowEventArgs e)
		{
			throw null;
		}

		protected virtual void OnUserDeletingRow(DataGridViewRowCancelEventArgs e)
		{
			throw null;
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			throw null;
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void PaintBackground(Graphics graphics, Rectangle clipBounds, Rectangle gridBounds)
		{
			throw null;
		}

		protected bool ProcessAKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessDeleteKey(Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessDownKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessEndKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessEnterKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessEscapeKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessF2Key(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessF3Key(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessHomeKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessInsertKey(Keys keyData)
		{
			throw null;
		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			throw null;
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			throw null;
		}

		protected bool ProcessLeftKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessNextKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessPriorKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessRightKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessSpaceKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessTabKey(Keys keyData)
		{
			throw null;
		}

		protected virtual bool ProcessDataGridViewKey(KeyEventArgs e)
		{
			throw null;
		}

		protected bool ProcessUpKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessZeroKey(Keys keyData)
		{
			throw null;
		}

		public bool RefreshEdit()
		{
			throw null;
		}

		public override void ResetText()
		{
			throw null;
		}

		public void SelectAll()
		{
			throw null;
		}

		protected virtual bool SetCurrentCellAddressCore(int columnIndex, int rowIndex, bool setAnchorCellAddress, bool validateCurrentCell, bool throughMouseClick)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		protected virtual void SetSelectedCellCore(int columnIndex, int rowIndex, bool selected)
		{
			throw null;
		}

		protected virtual void SetSelectedColumnCore(int columnIndex, bool selected)
		{
			throw null;
		}

		protected virtual void SetSelectedRowCore(int rowIndex, bool selected)
		{
			throw null;
		}

		public virtual void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
		{
			throw null;
		}

		public virtual void Sort(IComparer comparer)
		{
			throw null;
		}

		public void UpdateCellErrorText(int columnIndex, int rowIndex)
		{
			throw null;
		}

		public void UpdateCellValue(int columnIndex, int rowIndex)
		{
			throw null;
		}

		public void UpdateRowErrorText(int rowIndex)
		{
			throw null;
		}

		public void UpdateRowErrorText(int rowIndexStart, int rowIndexEnd)
		{
			throw null;
		}

		public void UpdateRowHeightInfo(int rowIndex, bool updateToEnd)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
