using System.Drawing;

namespace System.Windows.Forms
{
	public class RadioButton : ButtonBase
	{
		public class RadioButtonAccessibleObject : ButtonBaseAccessibleObject
		{
			public override string DefaultAction
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

			public RadioButtonAccessibleObject(RadioButton owner)
				:base(owner)
			{
				throw null;
			}

			public override void DoDefaultAction()
			{
				throw null;
			}
		}

		public bool AutoCheck
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

		public Appearance Appearance
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

		public ContentAlignment CheckAlign
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

		public bool Checked
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

		protected override CreateParams CreateParams
		{
			get
			{
				throw null;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		public new bool TabStop
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

		public override ContentAlignment TextAlign
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

		public event EventHandler AppearanceChanged
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

		public new event EventHandler DoubleClick
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

		public new event MouseEventHandler MouseDoubleClick
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

		public event EventHandler CheckedChanged
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

		public RadioButton()
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnClick(EventArgs e)
		{
			throw null;
		}

		protected override void OnEnter(EventArgs e)
		{
			throw null;
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			throw null;
		}

		public void PerformClick()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
