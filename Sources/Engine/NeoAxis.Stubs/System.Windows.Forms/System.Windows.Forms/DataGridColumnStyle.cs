using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
	public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
	{
		protected class CompModSwitches
		{
			public static TraceSwitch DGEditColumnEditing
			{
				get
				{
					throw null;
				}
			}

			public CompModSwitches()
			{
				throw null;
			}
		}

		protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
		{
			public override Rectangle Bounds
			{
				get
				{
					throw null;
				}
			}

			public override string Name
			{
				get
				{
					throw null;
				}
			}

			protected DataGridColumnStyle Owner
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleObject Parent
			{
				get
				{
					throw null;
				}
			}

			public override AccessibleRole Role
			{
				get
				{
					throw null;
				}
			}

			public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner)
			{
				throw null;
			}

			public DataGridColumnHeaderAccessibleObject()
			{
				throw null;
			}

			public override AccessibleObject Navigate(AccessibleNavigation navdir)
			{
				throw null;
			}
		}

		public virtual HorizontalAlignment Alignment
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

		public AccessibleObject HeaderAccessibleObject
		{
			get
			{
				throw null;
			}
		}

		public virtual PropertyDescriptor PropertyDescriptor
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

		public virtual DataGridTableStyle DataGridTableStyle
		{
			get
			{
				throw null;
			}
		}

		protected int FontHeight
		{
			get
			{
				throw null;
			}
		}

		public virtual string HeaderText
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

		public string MappingName
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

		public virtual string NullText
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

		public virtual bool ReadOnly
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

		public virtual int Width
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

		public event EventHandler AlignmentChanged
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

		public event EventHandler PropertyDescriptorChanged
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

		public event EventHandler FontChanged
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

		public event EventHandler HeaderTextChanged
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

		public event EventHandler MappingNameChanged
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

		public event EventHandler NullTextChanged
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

		public event EventHandler ReadOnlyChanged
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

		public event EventHandler WidthChanged
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

		public DataGridColumnStyle()
		{
			throw null;
		}

		public DataGridColumnStyle(PropertyDescriptor prop)
		{
			throw null;
		}

		protected virtual AccessibleObject CreateHeaderAccessibleObject()
		{
			throw null;
		}

		protected virtual void SetDataGrid(DataGrid value)
		{
			throw null;
		}

		protected virtual void SetDataGridInColumn(DataGrid value)
		{
			throw null;
		}

		public void ResetHeaderText()
		{
			throw null;
		}

		protected void BeginUpdate()
		{
			throw null;
		}

		protected void EndUpdate()
		{
			throw null;
		}

		protected virtual void Invalidate()
		{
			throw null;
		}

		protected void CheckValidDataSource(CurrencyManager value)
		{
			throw null;
		}

		void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl)
		{
			throw null;
		}
	}
}
