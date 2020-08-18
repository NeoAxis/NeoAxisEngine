using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	public class TableLayoutPanel : Panel, IExtenderProvider
	{
		public override LayoutEngine LayoutEngine
		{
			get
			{
				throw null;
			}
		}

		public TableLayoutSettings LayoutSettings
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

		public new BorderStyle BorderStyle
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

		public TableLayoutPanelCellBorderStyle CellBorderStyle
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

		public new TableLayoutControlCollection Controls
		{
			get
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

		public TableLayoutPanelGrowStyle GrowStyle
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

		public TableLayoutRowStyleCollection RowStyles
		{
			get
			{
				throw null;
			}
		}

		public TableLayoutColumnStyleCollection ColumnStyles
		{
			get
			{
				throw null;
			}
		}

		public event TableLayoutCellPaintEventHandler CellPaint
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

		public TableLayoutPanel()
		{
			throw null;
		}

		protected override ControlCollection CreateControlsInstance()
		{
			throw null;
		}

		bool IExtenderProvider.CanExtend(object obj)
		{
			throw null;
		}

		public int GetColumnSpan(Control control)
		{
			throw null;
		}

		public void SetColumnSpan(Control control, int value)
		{
			throw null;
		}

		public int GetRowSpan(Control control)
		{
			throw null;
		}

		public void SetRowSpan(Control control, int value)
		{
			throw null;
		}

		public int GetRow(Control control)
		{
			throw null;
		}

		public void SetRow(Control control, int row)
		{
			throw null;
		}

		public TableLayoutPanelCellPosition GetCellPosition(Control control)
		{
			throw null;
		}

		public void SetCellPosition(Control control, TableLayoutPanelCellPosition position)
		{
			throw null;
		}

		public int GetColumn(Control control)
		{
			throw null;
		}

		public void SetColumn(Control control, int column)
		{
			throw null;
		}

		public Control GetControlFromPosition(int column, int row)
		{
			throw null;
		}

		public TableLayoutPanelCellPosition GetPositionFromControl(Control control)
		{
			throw null;
		}

		public int[] GetColumnWidths()
		{
			throw null;
		}

		public int[] GetRowHeights()
		{
			throw null;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			throw null;
		}

		protected virtual void OnCellPaint(TableLayoutCellPaintEventArgs e)
		{
			throw null;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			throw null;
		}

		protected override void ScaleCore(float dx, float dy)
		{
			throw null;
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			throw null;
		}
	}
}
