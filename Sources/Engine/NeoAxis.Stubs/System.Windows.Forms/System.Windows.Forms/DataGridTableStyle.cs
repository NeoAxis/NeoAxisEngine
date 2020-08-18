using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridTableStyle : Component, IDataGridEditingService
	{
		public static readonly DataGridTableStyle DefaultTableStyle;

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

		public Color BackColor
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

		public Color ForeColor
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

		public string MappingName
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

		public virtual GridColumnStylesCollection GridColumnStyles
		{
			get
			{
				throw null;
			}
		}

		public virtual DataGrid DataGrid
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

		public event EventHandler AllowSortingChanged
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

		public event EventHandler AlternatingBackColorChanged
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

		public event EventHandler BackColorChanged
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

		public event EventHandler ForeColorChanged
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

		public event EventHandler GridLineColorChanged
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

		public event EventHandler GridLineStyleChanged
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

		public event EventHandler HeaderBackColorChanged
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

		public event EventHandler HeaderFontChanged
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

		public event EventHandler HeaderForeColorChanged
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

		public event EventHandler LinkColorChanged
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

		public event EventHandler LinkHoverColorChanged
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

		public event EventHandler PreferredColumnWidthChanged
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

		public event EventHandler PreferredRowHeightChanged
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

		public event EventHandler ColumnHeadersVisibleChanged
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

		public event EventHandler RowHeadersVisibleChanged
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

		public event EventHandler RowHeaderWidthChanged
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

		public event EventHandler SelectionBackColorChanged
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

		public event EventHandler SelectionForeColorChanged
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

		public event EventHandler MappingNameChanged
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

		public void ResetAlternatingBackColor()
		{
			throw null;
		}

		protected virtual bool ShouldSerializeAlternatingBackColor()
		{
			throw null;
		}

		protected bool ShouldSerializeBackColor()
		{
			throw null;
		}

		protected bool ShouldSerializeForeColor()
		{
			throw null;
		}

		public void ResetBackColor()
		{
			throw null;
		}

		public void ResetForeColor()
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

		protected virtual bool ShouldSerializeLinkColor()
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

		protected bool ShouldSerializePreferredRowHeight()
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

		public DataGridTableStyle(bool isDefaultTableStyle)
		{
			throw null;
		}

		public DataGridTableStyle()
		{
			throw null;
		}

		public DataGridTableStyle(CurrencyManager listManager)
		{
			throw null;
		}

		public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
		{
			throw null;
		}

		public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
		{
			throw null;
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnMappingNameChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAlternatingBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnAllowSortingChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnGridLineColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnGridLineStyleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHeaderBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHeaderFontChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnHeaderForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLinkColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnLinkHoverColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPreferredRowHeightChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnPreferredColumnWidthChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnColumnHeadersVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeadersVisibleChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRowHeaderWidthChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectionForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectionBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}
	}
}
