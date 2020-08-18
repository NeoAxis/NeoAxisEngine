using System.Drawing;

namespace System.Windows.Forms
{
	public class MonthCalendar : Control
	{
		public sealed class HitTestInfo
		{
			public Point Point
			{
				get
				{
					throw null;
				}
			}

			public HitArea HitArea
			{
				get
				{
					throw null;
				}
			}

			public DateTime Time
			{
				get
				{
					throw null;
				}
			}
		}

		public enum HitArea
		{
			Nowhere,
			TitleBackground,
			TitleMonth,
			TitleYear,
			NextMonthButton,
			PrevMonthButton,
			CalendarBackground,
			Date,
			NextMonthDate,
			PrevMonthDate,
			DayOfWeek,
			WeekNumbers,
			TodayLink
		}

		public DateTime[] AnnuallyBoldedDates
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

		public DateTime[] BoldedDates
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

		public Size CalendarDimensions
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

		protected override ImeMode DefaultImeMode
		{
			get
			{
				throw null;
			}
		}

		protected override Padding DefaultMargin
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

		public Day FirstDayOfWeek
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

		public new ImeMode ImeMode
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

		public int MaxSelectionCount
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

		public DateTime[] MonthlyBoldedDates
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

		public int ScrollChange
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

		public DateTime SelectionEnd
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

		public DateTime SelectionStart
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

		public SelectionRange SelectionRange
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

		public bool ShowToday
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

		public bool ShowTodayCircle
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

		public bool ShowWeekNumbers
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

		public Size SingleMonthSize
		{
			get
			{
				throw null;
			}
		}

		public new Size Size
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

		public DateTime TodayDate
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

		public bool TodayDateSet
		{
			get
			{
				throw null;
			}
		}

		public Color TitleBackColor
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

		public Color TitleForeColor
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

		public Color TrailingForeColor
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

		public new event EventHandler ImeModeChanged
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

		public event DateRangeEventHandler DateChanged
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

		public event DateRangeEventHandler DateSelected
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

		public MonthCalendar()
		{
			throw null;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw null;
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			throw null;
		}

		public void AddAnnuallyBoldedDate(DateTime date)
		{
			throw null;
		}

		public void AddBoldedDate(DateTime date)
		{
			throw null;
		}

		public void AddMonthlyBoldedDate(DateTime date)
		{
			throw null;
		}

		protected override void CreateHandle()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
			throw null;
		}

		public SelectionRange GetDisplayRange(bool visible)
		{
			throw null;
		}

		public HitTestInfo HitTest(int x, int y)
		{
			throw null;
		}

		public HitTestInfo HitTest(Point point)
		{
			throw null;
		}

		protected override bool IsInputKey(Keys keyData)
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

		protected virtual void OnDateChanged(DateRangeEventArgs drevent)
		{
			throw null;
		}

		protected virtual void OnDateSelected(DateRangeEventArgs drevent)
		{
			throw null;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			throw null;
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			throw null;
		}

		protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
		{
			throw null;
		}

		public void RemoveAllAnnuallyBoldedDates()
		{
			throw null;
		}

		public void RemoveAllBoldedDates()
		{
			throw null;
		}

		public void RemoveAllMonthlyBoldedDates()
		{
			throw null;
		}

		public void RemoveAnnuallyBoldedDate(DateTime date)
		{
			throw null;
		}

		public void RemoveBoldedDate(DateTime date)
		{
			throw null;
		}

		public void RemoveMonthlyBoldedDate(DateTime date)
		{
			throw null;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			throw null;
		}

		public void SetCalendarDimensions(int x, int y)
		{
			throw null;
		}

		public void SetDate(DateTime date)
		{
			throw null;
		}

		public void SetSelectionRange(DateTime date1, DateTime date2)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public void UpdateBoldedDates()
		{
			throw null;
		}

		protected override void WndProc(ref Message m)
		{
			throw null;
		}

		protected override void DefWndProc(ref Message m)
		{
			throw null;
		}
	}
}
