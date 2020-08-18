using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class DataGridTextBoxColumn : DataGridColumnStyle
	{
		public virtual TextBox TextBox
		{
			get
			{
				throw null;
			}
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			set
			{
				throw null;
			}
		}

		public string Format
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

		public IFormatProvider FormatInfo
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

		public override bool ReadOnly
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

		public DataGridTextBoxColumn()
		{
			throw null;
		}

		public DataGridTextBoxColumn(PropertyDescriptor prop)
		{
			throw null;
		}

		public DataGridTextBoxColumn(PropertyDescriptor prop, string format)
		{
			throw null;
		}

		public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault)
		{
			throw null;
		}

		public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault)
		{
			throw null;
		}

		protected override void SetDataGridInColumn(DataGrid value)
		{
			throw null;
		}

		protected void HideEditBox()
		{
			throw null;
		}

		protected void EndEdit()
		{
			throw null;
		}

		protected void PaintText(Graphics g, Rectangle bounds, string text, bool alignToRight)
		{
			throw null;
		}

		protected void PaintText(Graphics g, Rectangle textBounds, string text, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			throw null;
		}
	}
}
