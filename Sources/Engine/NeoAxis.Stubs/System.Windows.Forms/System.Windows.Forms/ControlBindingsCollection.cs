namespace System.Windows.Forms
{
	public class ControlBindingsCollection : BindingsCollection
	{
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

		public Binding this[string propertyName]
		{
			get
			{
				throw null;
			}
		}

		public DataSourceUpdateMode DefaultDataSourceUpdateMode
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

		public ControlBindingsCollection(IBindableComponent control)
		{
			throw null;
		}

		public void Add(Binding binding)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString)
		{
			throw null;
		}

		public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString, IFormatProvider formatInfo)
		{
			throw null;
		}

		protected override void AddCore(Binding dataBinding)
		{
			throw null;
		}

		public void Clear()
		{
			throw null;
		}

		protected override void ClearCore()
		{
			throw null;
		}

		public void Remove(Binding binding)
		{
			throw null;
		}

		public void RemoveAt(int index)
		{
			throw null;
		}

		protected override void RemoveCore(Binding dataBinding)
		{
			throw null;
		}
	}
}
