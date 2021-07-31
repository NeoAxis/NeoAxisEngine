using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	public class ErrorProvider : Component, IExtenderProvider, ISupportInitialize
	{
		public override ISite Site
		{
			set
			{
				throw null;
			}
		}

		public ErrorBlinkStyle BlinkStyle
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

		public ContainerControl ContainerControl
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

		public virtual bool RightToLeft
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

		public int BlinkRate
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

		public Icon Icon
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

		public event EventHandler RightToLeftChanged
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

		public ErrorProvider()
		{
			throw null;
		}

		public ErrorProvider(ContainerControl parentControl)
		{
			throw null;
		}

		public ErrorProvider(IContainer container)
		{
			throw null;
		}

		public void BindToDataAndErrors(object newDataSource, string newDataMember)
		{
			throw null;
		}

		public void UpdateBinding()
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

		public void Clear()
		{
			throw null;
		}

		public bool CanExtend(object extendee)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public string GetError(Control control)
		{
			throw null;
		}

		public ErrorIconAlignment GetIconAlignment(Control control)
		{
			throw null;
		}

		public int GetIconPadding(Control control)
		{
			throw null;
		}

		protected virtual void OnRightToLeftChanged(EventArgs e)
		{
			throw null;
		}

		public void SetError(Control control, string value)
		{
			throw null;
		}

		public void SetIconAlignment(Control control, ErrorIconAlignment value)
		{
			throw null;
		}

		public void SetIconPadding(Control control, int padding)
		{
			throw null;
		}
	}
}
