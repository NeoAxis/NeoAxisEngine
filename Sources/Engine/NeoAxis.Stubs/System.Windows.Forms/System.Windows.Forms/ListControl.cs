using System.Collections;

namespace System.Windows.Forms
{
	public abstract class ListControl : Control
	{
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

		protected CurrencyManager DataManager
		{
			get
			{
				throw null;
			}
		}

		public string DisplayMember
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

		public string ValueMember
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

		protected virtual bool AllowSelection
		{
			get
			{
				throw null;
			}
		}

		public abstract int SelectedIndex
		{
			get;
			set;
		}

		public object SelectedValue
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

		public event EventHandler DataSourceChanged
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

		public event EventHandler DisplayMemberChanged
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

		public event ListControlConvertEventHandler Format
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

		public event EventHandler FormatInfoChanged
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

		public event EventHandler FormatStringChanged
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

		public event EventHandler FormattingEnabledChanged
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

		public event EventHandler ValueMemberChanged
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

		public event EventHandler SelectedValueChanged
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

		protected object FilterItemOnProperty(object item)
		{
			throw null;
		}

		protected object FilterItemOnProperty(object item, string field)
		{
			throw null;
		}

		public string GetItemText(object item)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnDisplayMemberChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormat(ListControlConvertEventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormatInfoChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormatStringChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnFormattingEnabledChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnValueMemberChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			throw null;
		}

		protected abstract void RefreshItem(int index);

		protected virtual void RefreshItems()
		{
			throw null;
		}

		protected abstract void SetItemsCore(IList items);

		protected virtual void SetItemCore(int index, object value)
		{
			throw null;
		}

		protected ListControl()
		{
			throw null;
		}
	}
}
