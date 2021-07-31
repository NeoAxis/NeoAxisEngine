using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGrid : Control, ISupportInitialize, IDataGridEditingService
	{
		public sealed class HitTestInfo
		{
			public static readonly HitTestInfo Nowhere;

			public int Column
			{
				get
				{
					throw null;
				}
			}

			public int Row
			{
				get
				{
					throw null;
				}
			}

			public HitTestType Type
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

		public enum HitTestType
		{
			None = 0,
			Cell = 1,
			ColumnHeader = 2,
			RowHeader = 4,
			ColumnResize = 8,
			RowResize = 0x10,
			Caption = 0x20,
			ParentRows = 0x40
		}

		public bool AllowSorting
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

		public Color AlternatingBackColor
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public Color CaptionBackColor
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

		public Color CaptionForeColor
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

		public Font CaptionFont
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

		public string CaptionText
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

		public bool CaptionVisible
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

		public DataGridCell CurrentCell
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

		public Color SelectionBackColor
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

		public Color SelectionForeColor
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

		public int CurrentRowIndex
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

		public GridTableStylesCollection TableStyles
		{
			get
			{
				throw null;
			}
		}

		public Color GridLineColor
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

		public DataGridLineStyle GridLineStyle
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

		public DataGridParentRowsLabelStyle ParentRowsLabelStyle
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

		public int FirstVisibleColumn
		{
			get
			{
				throw null;
			}
		}

		public bool FlatMode
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

		public Color HeaderBackColor
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

		public Font HeaderFont
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

		public Color HeaderForeColor
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

		protected ScrollBar HorizScrollBar
		{
			get
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

		public Color LinkHoverColor
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

		public bool AllowNavigation
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

		public override Cursor Cursor
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

		public Color ParentRowsBackColor
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

		public Color ParentRowsForeColor
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

		public int PreferredColumnWidth
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

		public int PreferredRowHeight
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

		public bool ParentRowsVisible
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

		public int RowHeaderWidth
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

		protected ScrollBar VertScrollBar
		{
			get
			{
				throw null;
			}
		}

		public int VisibleColumnCount
		{
			get
			{
				throw null;
			}
		}

		public int VisibleRowCount
		{
			get
			{
				throw null;
			}
		}

		public object this[int rowIndex, int columnIndex]
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

		public object this[DataGridCell cell]
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

		public override ISite Site
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

		public event EventHandler CaptionVisibleChanged
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

		public event EventHandler ParentRowsLabelStyleChanged
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

		public event EventHandler FlatModeChanged
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

		public event EventHandler AllowNavigationChanged
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

		public new event EventHandler CursorChanged
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

		public event EventHandler ParentRowsVisibleChanged
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

		public event NavigateEventHandler Navigate
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

		protected event EventHandler RowHeaderClick
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

		public event EventHandler Scroll
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

		public event EventHandler BackButtonClick
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

		public event EventHandler ShowParentDetailsButtonClick
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

		public DataGrid()
		{
			throw null;
		}

		public void ResetAlternatingBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeAlternatingBackColor()
		{
			throw null;
		}

		public override void ResetBackColor()
		{
			throw null;
		}

		public override void ResetForeColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeCaptionBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeCaptionForeColor()
		{
			throw null;
		}

		protected bool ShouldSerializeSelectionBackColor()
		{
			throw null;
		}

		public void ResetSelectionBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeSelectionForeColor()
		{
			throw null;
		}

		public void ResetSelectionForeColor()
		{
			throw null;
		}

		public void SetDataBinding(object dataSource, string dataMember)
		{
			throw null;
		}

		protected virtual bool ShouldSerializeGridLineColor()
		{
			throw null;
		}

		public void ResetGridLineColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeHeaderBackColor()
		{
			throw null;
		}

		public void ResetHeaderBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeBackgroundColor()
		{
			throw null;
		}

		protected bool ShouldSerializeHeaderFont()
		{
			throw null;
		}

		public void ResetHeaderFont()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeHeaderForeColor()
		{
			throw null;
		}

		public void ResetHeaderForeColor()
		{
			throw null;
		}

		public void ResetLinkColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeLinkHoverColor()
		{
			throw null;
		}

		public void ResetLinkHoverColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeParentRowsBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeParentRowsForeColor()
		{
			throw null;
		}

		protected bool ShouldSerializePreferredRowHeight()
		{
			throw null;
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCaptionVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCurrentCellChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFlatModeChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackgroundColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowNavigationChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentRowsVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnParentRowsLabelStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			throw null;
		}

		protected void OnNavigate(NavigateEventArgs e)
		{
			throw null;
		}

		protected void OnRowHeaderClick(EventArgs e)
		{
			throw null;
		}

		protected void OnScroll(EventArgs e)
		{
			throw null;
		}

		protected virtual void GridHScrolled(object sender, ScrollEventArgs se)
		{
			throw null;
		}

		protected virtual void GridVScrolled(object sender, ScrollEventArgs se)
		{
			throw null;
		}

		protected void OnBackButtonClicked(object sender, EventArgs e)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			throw null;
		}

		protected void OnShowParentDetailsButtonClicked(object sender, EventArgs e)
		{
			throw null;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs ebe)
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
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

		protected override void OnEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnLeave(EventArgs e)
		{
			throw null;
		}

		protected override void OnKeyDown(KeyEventArgs ke)
		{
			throw null;
		}

		protected override void OnKeyPress(KeyPressEventArgs kpe)
		{
			throw null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
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

		protected override void OnPaint(PaintEventArgs pe)
		{
			throw null;
		}

		protected override void OnResize(EventArgs e)
		{
			throw null;
		}

		public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
		{
			throw null;
		}

		public void BeginInit()
		{
			throw null;
		}

		public void Collapse(int row)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
		{
			throw null;
		}

		public void Expand(int row)
		{
			throw null;
		}

		protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault)
		{
			throw null;
		}

		protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop)
		{
			throw null;
		}

		public void EndInit()
		{
			throw null;
		}

		public Rectangle GetCurrentCellBounds()
		{
			throw null;
		}

		public Rectangle GetCellBounds(int row, int col)
		{
			throw null;
		}

		public Rectangle GetCellBounds(DataGridCell dgc)
		{
			throw null;
		}

		public HitTestInfo HitTest(int x, int y)
		{
			throw null;
		}

		public HitTestInfo HitTest(Point position)
		{
			throw null;
		}

		public bool IsExpanded(int rowNumber)
		{
			throw null;
		}

		public bool IsSelected(int row)
		{
			throw null;
		}

		public void NavigateBack()
		{
			throw null;
		}

		public void NavigateTo(int rowNumber, string relationName)
		{
			throw null;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			throw null;
		}

		protected bool ProcessGridKey(KeyEventArgs ke)
		{
			throw null;
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			throw null;
		}

		protected bool ProcessTabKey(Keys keyData)
		{
			throw null;
		}

		protected virtual void CancelEditing()
		{
			throw null;
		}

		protected void ResetSelection()
		{
			throw null;
		}

		public void Select(int row)
		{
			throw null;
		}

		public void SubObjectsSiteChange(bool site)
		{
			throw null;
		}

		public void UnSelect(int row)
		{
			throw null;
		}

		protected virtual string GetOutputTextDelimiter()
		{
			throw null;
		}
	}
}
