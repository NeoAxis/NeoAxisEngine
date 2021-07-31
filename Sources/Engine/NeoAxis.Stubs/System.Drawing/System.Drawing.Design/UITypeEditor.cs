using System.ComponentModel;

namespace System.Drawing.Design
{
	public class UITypeEditor
	{
		public virtual bool IsDropDownResizable
		{
			get
			{
				throw null;
			}
		}

		public UITypeEditor()
		{
			throw null;
		}

		public object EditValue(IServiceProvider provider, object value)
		{
			throw null;
		}

		public virtual object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			throw null;
		}

		public UITypeEditorEditStyle GetEditStyle()
		{
			throw null;
		}

		public bool GetPaintValueSupported()
		{
			throw null;
		}

		public virtual bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			throw null;
		}

		public virtual UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			throw null;
		}

		public void PaintValue(object value, Graphics canvas, Rectangle rectangle)
		{
			throw null;
		}

		public virtual void PaintValue(PaintValueEventArgs e)
		{
			throw null;
		}
	}
}
