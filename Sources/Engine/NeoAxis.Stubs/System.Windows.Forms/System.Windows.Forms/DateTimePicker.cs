using System.Drawing;

namespace System.Windows.Forms
{
	public class DateTimePicker : Control
	{
		public class DateTimePickerAccessibleObject : ControlAccessibleObject
		{
			public override string KeyboardShortcut
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

			public override AccessibleStates State
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

			public DateTimePickerAccessibleObject(DateTimePicker owner)
				:base(owner)
			{
				throw null;
			}
		}

		public static readonly DateTime MinDateTime;

		public static readonly DateTime MaxDateTime;

		public override Color BackColor
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

		public override Image BackgroundImage
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

		public override ImageLayout BackgroundImageLayout
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

		public Color CalendarForeColor
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

		public Font CalendarFont
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

		public Color CalendarTitleBackColor
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

		public Color CalendarTitleForeColor
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

		public Color CalendarTrailingForeColor
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

		public Color CalendarMonthBackground
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

		public string CustomFormat
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

		protected override Size DefaultSize
		{
			get
			{
				throw null;
			}
		}

		protected override bool DoubleBuffered
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

		public LeftRightAlignment DropDownAlign
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

		public override Color ForeColor
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

		public DateTimePickerFormat Format
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

		public DateTime MaxDate
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

		public static DateTime MaximumDateTime
		{
			get
			{
				throw null;
			}
		}

		public DateTime MinDate
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

		public static DateTime MinimumDateTime
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

		public int PreferredHeight
		{
			get
			{
				throw null;
			}
		}

		public virtual bool RightToLeftLayout
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

		public bool ShowCheckBox
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

		public bool ShowUpDown
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

		public override string Text
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

		public DateTime Value
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

		public new event EventHandler BackColorChanged
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

		public new event EventHandler BackgroundImageChanged
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

		public new event EventHandler BackgroundImageLayoutChanged
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

		public new event EventHandler Click
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

		public new event EventHandler ForeColorChanged
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

		public event EventHandler FormatChanged
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

		public new event PaintEventHandler Paint
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

		public new event MouseEventHandler MouseClick
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

		public new event EventHandler TextChanged
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

		public event EventHandler CloseUp
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

		public event EventHandler RightToLeftLayoutChanged
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

		public event EventHandler ValueChanged
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

		public event EventHandler DropDown
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

		public DateTimePicker()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected override void DestroyHandle()
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			throw null;
		}

		protected virtual void OnCloseUp(EventArgs eventargs)
		{
			throw null;
		}

		protected virtual void OnDropDown(EventArgs eventargs)
		{
			throw null;
		}

		protected virtual void OnFormatChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			throw null;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnValueChanged(EventArgs eventargs)
		{
			throw null;
		}

		protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}
	}
}
