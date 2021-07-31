using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
	public abstract class PropertyTab : IExtenderProvider
	{
		public virtual Bitmap Bitmap
		{
			get
			{
				throw null;
			}
		}

		public virtual object[] Components
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

		public abstract string TabName
		{
			get;
		}

		public virtual string HelpKeyword
		{
			get
			{
				throw null;
			}
		}

		~PropertyTab()
		{
			throw null;
		}

		public virtual bool CanExtend(object extendee)
		{
			throw null;
		}

		public virtual void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public virtual PropertyDescriptor GetDefaultProperty(object component)
		{
			throw null;
		}

		public virtual PropertyDescriptorCollection GetProperties(object component)
		{
			throw null;
		}

		public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

		public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			throw null;
		}

		protected PropertyTab()
		{
			throw null;
		}
	}
}
