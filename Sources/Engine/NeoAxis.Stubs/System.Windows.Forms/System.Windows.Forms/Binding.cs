namespace System.Windows.Forms
{
	public class Binding
	{
		public object DataSource
		{
			get
			{
				throw null;
			}
		}

		public BindingMemberInfo BindingMemberInfo
		{
			get
			{
				throw null;
			}
		}

		public IBindableComponent BindableComponent
		{
			get
			{
				throw null;
			}
		}

		public Control Control
		{
			get
			{
				throw null;
			}
		}

		public bool IsBinding
		{
			get
			{
				throw null;
			}
		}

		public BindingManagerBase BindingManagerBase
		{
			get
			{
				throw null;
			}
		}

		public string PropertyName
		{
			get
			{
				throw null;
			}
		}

		public bool FormattingEnabled
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

		public string FormatString
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

		public object NullValue
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

		public object DataSourceNullValue
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

		public ControlUpdateMode ControlUpdateMode
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

		public DataSourceUpdateMode DataSourceUpdateMode
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

		public event BindingCompleteEventHandler BindingComplete
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

		public event ConvertEventHandler Parse
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

		public event ConvertEventHandler Format
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

		public Binding(string propertyName, object dataSource, string dataMember)
		{
			throw null;
		}

		public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
		{
			throw null;
		}

		public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode)
		{
			throw null;
		}

		public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue)
		{
			throw null;
		}

		public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString)
		{
			throw null;
		}

		public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
		{
			throw null;
		}

		protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
		{
			throw null;
		}

		protected virtual void OnParse(ConvertEventArgs cevent)
		{
			throw null;
		}

		protected virtual void OnFormat(ConvertEventArgs cevent)
		{
			throw null;
		}

		public void ReadValue()
		{
			throw null;
		}

		public void WriteValue()
		{
			throw null;
		}
	}
}
