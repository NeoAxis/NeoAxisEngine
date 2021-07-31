using System.Collections;

namespace System.Windows.Forms
{
	public class DomainUpDown : UpDownBase
	{
		public class DomainUpDownItemCollection : ArrayList
		{
			public override object this[int index]
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

			public override int Add(object item)
			{
				throw null;
			}

			public override void Remove(object item)
			{
				throw null;
			}

			public override void RemoveAt(int item)
			{
				throw null;
			}

			public override void Insert(int index, object item)
			{
				throw null;
			}
		}

		public class DomainUpDownAccessibleObject : ControlAccessibleObject
		{
			public override string Name
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

			public override AccessibleRole Role
			{
				get
				{
					throw null;
				}
			}

			public DomainUpDownAccessibleObject(Control owner)
				:base(owner)
			{
				throw null;
			}

			public override AccessibleObject GetChild(int index)
			{
				throw null;
			}

			public override int GetChildCount()
			{
				throw null;
			}
		}

		public class DomainItemAccessibleObject : AccessibleObject
		{
			public override string Name
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

			public override AccessibleStates State
			{
				get
				{
					throw null;
				}
			}

			public override string Value
			{
				get
				{
					throw null;
				}
			}

			public DomainItemAccessibleObject(string name, AccessibleObject parent)
			{
				throw null;
			}
		}

		public DomainUpDownItemCollection Items
		{
			get
			{
				throw null;
			}
		}

		public new Padding Padding
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

		public int SelectedIndex
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

		public object SelectedItem
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

		public bool Sorted
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

		public bool Wrap
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

		public new event EventHandler PaddingChanged
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

		public event EventHandler SelectedItemChanged
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

		public DomainUpDown()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		public override void DownButton()
		{
			throw null;
		}

		protected override void OnChanged(object source, EventArgs e)
		{
			throw null;
		}

		protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
		{
			throw null;
		}

		protected void OnSelectedItemChanged(object source, EventArgs e)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public override void UpButton()
		{
			throw null;
		}

		protected override void UpdateEditText()
		{
			throw null;
		}
	}
}
