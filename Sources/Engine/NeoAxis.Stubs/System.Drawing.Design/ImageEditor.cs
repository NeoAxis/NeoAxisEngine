using System.ComponentModel;
using System.IO;

namespace System.Drawing.Design
{
	public class ImageEditor : UITypeEditor
	{
		protected virtual Type[] GetImageExtenders()
		{
			throw null;
		}

		protected static string CreateExtensionsString(string[] extensions, string sep)
		{
			throw null;
		}

		protected static string CreateFilterEntry(ImageEditor e)
		{
			throw null;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			throw null;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			throw null;
		}

		protected virtual string GetFileDialogDescription()
		{
			throw null;
		}

		protected virtual string[] GetExtensions()
		{
			throw null;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			throw null;
		}

		protected virtual Image LoadFromStream(Stream stream)
		{
			throw null;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			throw null;
		}

		public ImageEditor()
		{
			throw null;
		}
	}
}
