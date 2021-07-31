using System.Runtime.Serialization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
	[Serializable]
	public sealed class TableLayoutSettings : LayoutSettings, ISerializable
	{
		public override LayoutEngine LayoutEngine
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

		public int GetColumnSpan(object control)
		{
			throw null;
		}

		public void SetColumnSpan(object control, int value)
		{
			throw null;
		}

		public int GetRowSpan(object control)
		{
			throw null;
		}

		public void SetRowSpan(object control, int value)
		{
			throw null;
		}

		public int GetRow(object control)
		{
			throw null;
		}

		public void SetRow(object control, int row)
		{
			throw null;
		}

		public TableLayoutPanelCellPosition GetCellPosition(object control)
		{
			throw null;
		}

		public void SetCellPosition(object control, TableLayoutPanelCellPosition cellPosition)
		{
			throw null;
		}

		public int GetColumn(object control)
		{
			throw null;
		}

		public void SetColumn(object control, int column)
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}
	}
}
