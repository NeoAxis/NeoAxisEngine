#if !DEPLOY
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Internal.ComponentFactory.Krypton.Toolkit;

//
// Author: Adam A. Zgagacz
//Copyright: ProXoft L.L.C. 2013
// Home Page: http://www.proxoft.com
//
//ScrollBarEnhanced control is the replacement for standard VScrollBar and HscrollBar controls from Visual Studio
//It mimics their behavior wile adding at the some time substantial amount of new functionality:
//- extended domain of values to decimal 
//- added customizable graphical bookmarks
//- added customizable dynamic ToolTips.
//- since context menu is accessible as property, it is now easily customizable
//- extended amount of information passed to event handlers and added new events
//- added new property 'BookmarksOnTop' if set to true Bookmarks are displayed as topmost items
//
//History of changes:
//August 6th, 2013:
//Bug fix for Minimum!=0
//August 8th, 2013:
//Added context menu for scroll commands
//August 11th, 2013
//Added ValueRangeScrollBarBookmark
//September 13th, 2013
//Added documentation comments
//December 12th, 2013 
//Cleanup comments and simplifying the code
//Correcting runtime editor to allow adding bookmarks at runtime (previously it was impossible due to exception: cannot instantiate abstract class)
namespace NeoAxis.Editor//ProXoft.WinForms 
{
	[RefreshProperties( RefreshProperties.All )]
	public class EngineScrollBar : UserControl
	{
		#region Private fields

		internal bool MouseUpDownStatus;
		//Input.MouseButtonState MouseUpDownStatus = Input.MouseButtonState.Released;
		internal EngineScrollBarMouseLocation MouseScrollBarArea = EngineScrollBarMouseLocation.OutsideScrollBar;
		Point MouseActivePointPoint;
		int MouseRelativeYFromThumbTop = 0;

		int PrevouslyReportedHotValue = -1;
		int HotValue = -1;

		int PreviousValue = -1;

		int m_Minimum = 0;
		int m_Maximum = 100;
		int m_Value = 0;

		int itemSize = 1;

		bool m_disposed;

		//ToolTip toolTip1;
		//!!!!
		System.Windows.Forms.Timer timerMouseDownRepeater;

		//[NonSerialized]
		//ObservableCollection<ScrollBarBookmark> m_Bookmarks = null;

		Orientation m_Orientation = Orientation.Vertical;
		//bool m_BookmarksOnTop = true;

		//Every time mouse is pressed down this is populated to be used by the repeat timer
		MouseEventArgs m_MouseDownArgs = null;

		#endregion

		#region Public Events

		/// <summary>
		/// Fires every time mouse is clicked over track area.
		/// </summary>
		[Description( "Fires every time mouse is clicked over track area." )]
		public new event EventHandler<EngineScrollBarMouseEventArgs> MouseClick = null;

		/// <summary>
		/// Fires every time mouse moves over track area.
		/// </summary>
		[Description( "Fires every time mouse moves over track area." )]
		public new event EventHandler<EngineScrollBarMouseEventArgs> MouseMove = null;

		/// <summary>
		/// Occurs each time scrollbar orientation has changed.
		/// </summary>
		[Description( "Occurs each time scrollbar orientation has changed." )]
		public event System.EventHandler OrientationChanged = null;

		/// <summary>
		/// Occurs every time scrollbar orientation is about to change.
		/// </summary>
		[Description( "Occurs every time scrollbar orientation is about to change." )]
		public event EventHandler<CancelEventArgs> OrientationChanging = null;

		/// <summary>
		/// 
		/// </summary>
		public new event EventHandler<EngineScrollBarEventArgs> Scroll = null;

		///// <summary>
		///// Fired every time mouse moves over bookmark (or multiple bookmarks).
		///// Allows to overwrite default ToolTip value.
		///// </summary>
		//[Description( "Allows to overwrite default ToolTip value." )]
		//public event EventHandler<TooltipNeededEventArgs> ToolTipNeeded = null;

		/// <summary>
		/// Fired every time <c>Value</c> of the ScrollBar changes.
		/// </summary>
		[Description( "Occurs every time scrollbar value changes." )]
		public event System.EventHandler ValueChanged = null;

		#endregion

		#region Constructor and related

		/// <summary>
		/// Constructor. Initialize properties.
		/// </summary>
		public EngineScrollBar()
		{
			InitializeComponent();

			SetStyle( ControlStyles.ResizeRedraw, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
			SetStyle( ControlStyles.DoubleBuffer, true );

			//Initialize default values for properties
			//Bookmarks = new ObservableCollection<ScrollBarBookmark>();
			//ShowTooltipOnMouseMove = false;
			//InitialDelay = 400;
			//RepeatRate = 62;
			//LargeChange = 10;
			//SmallChange = 1;
			//QuickBookmarkNavigation = true;

			timerMouseDownRepeater.Tick += new System.EventHandler( timerMouseDownRepeater_Tick );

			//Dock = DockStyle.None;
			//this.ClientSize = new Size( SystemInformation.VerticalScrollBarWidth, this.ClientSize.Height );
			//this.Width = SystemInformation.VerticalScrollBarWidth;

			//Orientation = Orientation.Vertical;

			MouseClick += control_MouseClick;
			MouseUp += control_MouseUp;
		}

		/// <summary>
		/// Generates repeat events when mouse is pressed and hold.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void timerMouseDownRepeater_Tick( object sender, EventArgs e )
		{
			base.OnMouseDown( m_MouseDownArgs );

			DoMouseDown( m_MouseDownArgs );

			if( timerMouseDownRepeater.Enabled )
				timerMouseDownRepeater.Interval = RepeatRate;
			else
				timerMouseDownRepeater.Interval = InitialDelay;

			//Do this only if not dragging thumb
			if( MouseScrollBarArea != EngineScrollBarMouseLocation.Thumb )
				timerMouseDownRepeater.Enabled = true;
			else
				timerMouseDownRepeater.Enabled = false;
		}

		IContainer components;
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			//this.toolTip1 = new System.Windows.Forms.ToolTip( this.components );
			this.timerMouseDownRepeater = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// ScrollBarEnhanced
			// 
			this.Name = "EngineScrollBar";
			this.ResumeLayout( false );
		}

		/// <summary>
		/// Dispose overridden method. When called from the host <c>disposing</c> parameter is <b>true</b>.
		/// When called from the finalize parameter is <b>false</b>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( !m_disposed )
			{
				if( disposing )
				{
					if( components != null )
					{
						components.Dispose();
					}
				}
				//foreach( Brush brush in m_BrushesCache.Values )
				//	brush.Dispose();

				//foreach( Pen pen in m_PensCache.Values )
				//	pen.Dispose();

				m_disposed = true;

				base.Dispose();

			}
		}

		///// <summary>
		///// Every bookmark is added or removed, scrollbar has to refresh itself.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//void Bookmarks_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		//{
		//	Invalidate();
		//}

		#endregion

		#region Public Properties

		//We don't need BackColor to be exposed as property, so let's try to hide it form property list
		/// <summary>
		/// BackColor doesn't have any meaning for the ScrollBar.
		/// </summary> 
		[Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never )]
		public new Color BackColor { get; set; }

		///// <summary>
		///// List of bookmarks associated with the ScrollBar.
		///// </summary>
		//[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		//[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true )]
		//[DefaultValue( null ), Category( "Enhanced" )]
		//[Description( "List of ScrollBar bookmarks" )]
		//[NotifyParentProperty( true )]
		//[Editor( typeof( BookmarksCollectionEditor ), typeof( UITypeEditor ) )]
		//public ObservableCollection<ScrollBarBookmark> Bookmarks
		//{
		//	get
		//	{
		//		if( m_Bookmarks == null )
		//			m_Bookmarks = new ObservableCollection<ScrollBarBookmark>();
		//		return m_Bookmarks;
		//	}
		//	set
		//	{
		//		if( m_Bookmarks != null )
		//			m_Bookmarks.CollectionChanged -= new NotifyCollectionChangedEventHandler( Bookmarks_CollectionChanged );

		//		m_Bookmarks = value;

		//		if( m_Bookmarks != null )
		//			m_Bookmarks.CollectionChanged += new NotifyCollectionChangedEventHandler( Bookmarks_CollectionChanged );

		//	}
		//}

		///// <summary>
		///// If set to <b>true</b> bookmarks are displayed as the topmost elements. If <b>false</b> thumb covers bookmarks that might be hidden beneath.
		///// </summary>
		//[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( true ), Category( "Enhanced" )]
		//[Description( "If set to 'true' bookmarks are displayed as the topmost elements. If 'false' thumb covers bookmarks that might be hidden beneath." )]
		//public bool BookmarksOnTop
		//{
		//	get { return m_BookmarksOnTop; }
		//	set
		//	{
		//		if( m_BookmarksOnTop != value )
		//		{
		//			m_BookmarksOnTop = value;
		//			Invalidate();
		//		}
		//	}
		//}

		/// <summary>
		/// Delay in milliseconds to start autorepeat behavior when mouse is pressed down and hold.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 400 ), Category( "Enhanced" )]
		[Description( "Delay in milliseconds to start autorepeat behavior when mouse is pressed down and hold." )]
		public int InitialDelay { set; get; } = 400;

		/// <summary>
		/// Large scrollbar change.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 10 ), Category( "Enhanced" )]
		[Description( "Large scrollbar change." )]
		public int LargeChange { set; get; } = 10;

		/// <summary>
		/// "Maximum scrollbar value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 100 ), Category( "Enhanced" )]
		[Description( "Maximum scrollbar value." )]
		public int Maximum
		{
			get { return m_Maximum; }
			set
			{
				if( m_Maximum == value )
					return;

				if( value < Minimum )
					throw new ArgumentException( "Minimum has to be less or equal Maximum", "Minimum" );

				//The following line will throw exception is range is more than decimal.MaxValue
				decimal rangeTestValue = value - Minimum;

				m_Maximum = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Minimum scrollbar value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 0 ), Category( "Enhanced" )]
		[Description( "Minimum scrollbar value." )]
		public int Minimum
		{
			get { return m_Minimum; }
			set
			{
				if( m_Minimum == value )
					return;

				if( Maximum < value )
					throw new ArgumentException( "Minimum has to be less or equal Maximum", "Minimum" );

				//The following line will throw exception is range is more than decimal.MaxValue
				decimal rangeTestValue = Maximum - value;

				m_Minimum = value;
				Invalidate();
			}
		}

		/// <summary>
		/// ScrollBar orientation.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( Orientation.Vertical ), Category( "Enhanced" )]
		[Description( "ScrollBar orientation." )]
		public Orientation Orientation
		{
			set
			{
				if( OrientationChanging != null )
				{
					CancelEventArgs ea = new CancelEventArgs( false );
					OrientationChanging( this, ea );
					if( ea.Cancel )
						return;
				}
				if( m_Orientation != value )
				{
					m_Orientation = value;

					////Switch width with height
					//int tmpWidth = this.Width;
					//this.Width = this.Height;
					//this.Height = tmpWidth;

					////in all bookmarks switch width with height (only basic shape bookmarks)
					//foreach( ScrollBarBookmark Bookmark in Bookmarks )
					//{
					//	if( Bookmark is BasicShapeScrollBarBookmark )
					//	{
					//		BasicShapeScrollBarBookmark BSBookmark = (BasicShapeScrollBarBookmark)Bookmark;
					//		tmpWidth = BSBookmark.Width;
					//		BSBookmark.Width = BSBookmark.Height;
					//		BSBookmark.Height = tmpWidth;
					//	}
					//}

					//if( Orientation == Orientation.Vertical )
					//{
					//	base.MinimumSize = new Size( 0, 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ + ThumbLength );
					//	switch( this.Dock )
					//	{
					//	case DockStyle.Bottom:
					//		Dock = DockStyle.Right; break;
					//	case DockStyle.Top:
					//		Dock = DockStyle.Left; break;
					//	}
					//}
					//else
					//{
					//	base.MinimumSize = new Size( 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ + ThumbLength, 0 );
					//	switch( this.Dock )
					//	{
					//	case DockStyle.Right:
					//		Dock = DockStyle.Bottom; break;
					//	case DockStyle.Left:
					//		Dock = DockStyle.Top; break;
					//	}

					//}

					if( OrientationChanged != null )
					{
						OrientationChanged( this, EventArgs.Empty );
					}
				}

			}
			get
			{
				return m_Orientation;
			}
		}

		///// <summary>
		///// "When <b>true</b>, clicking on bookmark image, changes scrollbar value to the bookmark value 
		///// (moves thumb position to bookmark value).
		///// </summary>
		//[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( true ), Category( "Enhanced" )]
		//[Description( "When 'true', clicking on bookmark image, changes scrollbar value to the bookmark value." )]
		//public bool QuickBookmarkNavigation
		//{ set; get; }

		/// <summary>
		/// Delay in milliseconds between autorepeat MouseDown events when mouse is pressed down and hold.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 62 ), Category( "Enhanced" )]
		[Description( "Delay in milliseconds between autorepeat MouseDown events when mouse is pressed down and hold." )]
		public int RepeatRate { set; get; } = 62;

		///// <summary>
		///// When set to <b>true</b>, allows to show ToolTip when mouse moves over scrollbar area.
		///// </summary>
		//[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( false ), Category( "Enhanced" )]
		//[Description( "When set to 'true', allows to show ToolTip when mouse moves over scrollbar area." )]
		//public bool ShowTooltipOnMouseMove { set; get; }

		/// <summary>
		/// Small scrollbar change.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 1 ), Category( "Enhanced" )]
		[Description( "Small change." )]
		public int SmallChange { set; get; } = 1;

		/// <summary>
		/// Scrollbar value. Determines current thumb position.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 0 ), Category( "Enhanced" )]
		[Description( "Value" )]
		public int Value
		{
			get { return m_Value; }
			set
			{
				if( m_Value == value )
					return;

				if( value < Minimum )
					m_Value = Minimum;
				else if( value > Maximum )
					m_Value = Maximum;
				else
					m_Value = value;

				OnValueChanged();
				Invalidate();
			}
		}

		[EditorBrowsable( EditorBrowsableState.Always ), Browsable( true ), DefaultValue( 1 ), Category( "Enhanced" )]
		[Description( "Value" )]
		public int ItemSize
		{
			get { return itemSize; }
			set { itemSize = value; }
		}

		#endregion

		#region Overridden events

		/// <summary>
		/// What should happen here:
		/// 1. Save information that mouse is down
		/// 2. Call timer event handler (it will repeat periodically MouseDown events as long as mouse is down)
		/// </summary>
		/// <param name="e">Standard <c>MouseEventArgs</c>.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			//Save arguments passed to mouse down
			m_MouseDownArgs = e;
			MouseUpDownStatus = true;//Input.MouseButtonState.Pressed;

			//Make sure timer is disabled
			timerMouseDownRepeater.Enabled = false;

			//Call timer event; it will call DoMouseDown every RepeatRate interval
			timerMouseDownRepeater_Tick( null, EventArgs.Empty );
		}

		/// <summary>
		/// This methods called from repeater timer event handler
		/// </summary>
		/// <param name="e"></param>
		void DoMouseDown( MouseEventArgs e )
		{
			//1. Save info about fact that mouse is presses
			//2. Save scrollbar area where mouse was pressed 
			if( Orientation == Orientation.Vertical )
				MouseScrollBarArea = MouseLocation( e.Y, out MouseRelativeYFromThumbTop );
			else
				MouseScrollBarArea = MouseLocation( e.X, out MouseRelativeYFromThumbTop );

			//3. Save exact location of mouse press 
			MouseActivePointPoint = e.Location;

			//4. Calculate ScrollEvent arguments and adjust Value if needed
			var NewValue = Value;
			ScrollEventType et;
			//List<ScrollBarBookmark> bookmarksUnder = BookmarksUnderPosition( e.X, e.Y );
			//if( ( bookmarksUnder.Count > 0 ) && ( QuickBookmarkNavigation ) )
			//{
			//	et = ScrollEventType.ThumbPosition;
			//	ScrollBarBookmark topMostBookmark = bookmarksUnder[ bookmarksUnder.Count - 1 ];
			//	if( topMostBookmark is ValueRangeScrollBarBookmark )
			//		NavigateTo( HotValue );
			//	else
			//		Value = topMostBookmark.Value;
			//}
			//else
			{
				switch( MouseScrollBarArea )
				{
				case EngineScrollBarMouseLocation.BottomOrRightArrow:
				case EngineScrollBarMouseLocation.BottomOrRightTrack:
					if( MouseScrollBarArea == EngineScrollBarMouseLocation.BottomOrRightArrow )
					{
						NewValue += SmallChange;
						et = ScrollEventType.SmallIncrement;
					}
					else    // EnhancedScrollBarMouseLocation.bottomTrack
					{
						NewValue += LargeChange;
						et = ScrollEventType.LargeIncrement;
					}
					if( NewValue >= Maximum )
					{
						NewValue = Maximum;
						et = ScrollEventType.Last;
					}
					OnScroll( NewValue, Value, et );
					break;
				case EngineScrollBarMouseLocation.Thumb:
					OnScroll( Value, Value, ScrollEventType.ThumbTrack );
					break;
				case EngineScrollBarMouseLocation.TopOrLeftArrow:
				case EngineScrollBarMouseLocation.TopOrLeftTrack:
					if( MouseScrollBarArea == EngineScrollBarMouseLocation.TopOrLeftArrow )
					{
						NewValue -= SmallChange;
						et = ScrollEventType.SmallDecrement;
					}
					else
					{
						NewValue -= LargeChange;
						et = ScrollEventType.LargeIncrement;
					}
					if( NewValue <= Minimum )
					{
						NewValue = Minimum;
						et = ScrollEventType.First;
					}
					OnScroll( NewValue, Value, et );
					break;
				}
				Value = NewValue;
			}

			//3. Repaint
			Invalidate();

		}

		/// <summary>
		/// Captures mouse wheel actions and translates them to small decrement events.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel( MouseEventArgs e )
		{
			int ScrollStep = e.Delta / 120;//System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;

			//Calculate new value
			var NewValue = Value - SmallChange * ScrollStep;

			if( NewValue < Value )
				OnScroll( NewValue, Value, ScrollEventType.SmallDecrement );
			else
				OnScroll( NewValue, Value, ScrollEventType.SmallIncrement );

			Value = NewValue;
			OnScroll( Value, Value, ScrollEventType.EndScroll );

			base.OnMouseWheel( e );
		}

		/// <summary>
		/// MouseClick override. Calls MouseClick event handled with enhanced arguments. 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseClick( MouseEventArgs e )
		{
			base.OnMouseClick( e );
			//enrich arguments and call back the host
			OnMouseClick( e, Value );//, BookmarksUnderPosition( e.X, e.Y ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove( MouseEventArgs e )
		{

			base.OnMouseMove( e );

			//Mouse is down and is over thumb
			if( ( MouseUpDownStatus == true/*Input.MouseButtonState.Pressed*/ ) && ( MouseScrollBarArea == EngineScrollBarMouseLocation.Thumb ) )
			{
				//Dragging thumb button

				//Calculate Value based on new e.Y 
				int NewValue;
				if( Orientation == Orientation.Vertical )
					NewValue = ThumbTopPosition2Value( e.Y - MouseRelativeYFromThumbTop );
				else
					NewValue = ThumbTopPosition2Value( e.X - MouseRelativeYFromThumbTop );

				if( NewValue < Minimum ) NewValue = Minimum;
				if( NewValue > Maximum ) NewValue = Maximum;

				//Call onScroll event 
				OnScroll( NewValue, Value, ScrollEventType.ThumbTrack );

				//Assign new value
				Value = NewValue;  //New value will move scrollbar to proper position

				//Refresh display
				this.Invalidate();
			}
			else
			{
				//string toolTip = "";
				//Moving mouse over different areas

				//1. Save current (before mouse move) mouse location
				EngineScrollBarMouseLocation oldLocation = MouseScrollBarArea;

				int tmpInt;
				EngineScrollBarMouseLocation newLocation;

				int TrackPosition;
				if( Orientation == Orientation.Vertical )
				{
					newLocation = MouseLocation( e.Y, out tmpInt );
					TrackPosition = e.Y - GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
					switch( newLocation )
					{
					case EngineScrollBarMouseLocation.TopOrLeftArrow:
						TrackPosition = 0;
						break;
					case EngineScrollBarMouseLocation.BottomOrRightArrow:
						TrackPosition = ClientSize.Height - 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
						break;

					}
				}
				else
				{
					newLocation = MouseLocation( e.X, out tmpInt );

					TrackPosition = e.X - GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
					switch( newLocation )
					{
					case EngineScrollBarMouseLocation.TopOrLeftArrow:
						TrackPosition = 0;
						break;
					case EngineScrollBarMouseLocation.BottomOrRightArrow:
						TrackPosition = ClientSize.Width - 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
						break;

					}
				}
				HotValue = ( Maximum - Minimum ) * ( TrackPosition / TrackLength ) + Minimum;
				MouseScrollBarArea = newLocation;

				if( ( TrackPosition < 0 ) || ( TrackPosition > TrackLength ) )
				{
					//toolTip1.Hide( this );
				}
				else
				{
					////Get list of bookmarks under cursor
					//List<ScrollBarBookmark> bookmarksOver = BookmarksUnderPosition( e.X, e.Y );

					if( PrevouslyReportedHotValue != HotValue )
					{
						PrevouslyReportedHotValue = HotValue;
						//string defaultToolTip = Name + " " + HotValue.ToString( "###,##0" );
						//if( ( ToolTipNeeded != null ) )
						//{
						//	TooltipNeededEventArgs ea = new TooltipNeededEventArgs( HotValue, defaultToolTip );//, bookmarksOver );
						//	ToolTipNeeded( this, ea );
						//	toolTip = ea.ToolTip;
						//}
						//else  //display default value ToolTip
						//{
						//	toolTip = defaultToolTip;
						//}

						//if( this.toolTip1.GetToolTip( this ) != toolTip )
						//	this.toolTip1.SetToolTip( this, toolTip );
					}

					//Find section of scrollbar the mouse is moving over

					//Call the host to notify about the MouseMove event		
					OnMouseMove( e, HotValue );//, bookmarksOver );


					//If moving over different area- refresh display
					if( oldLocation != MouseScrollBarArea )
						this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Forces repaint of ScrollBar when mouse moves outside of ScrollBar area.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			MouseUpDownStatus = false;//Input.MouseButtonState.Released;
			MouseScrollBarArea = EngineScrollBarMouseLocation.OutsideScrollBar;
			timerMouseDownRepeater.Enabled = false;

			base.OnMouseLeave( e );
			this.Invalidate();
		}

		/// <summary>
		/// Fires <c>Scroll</c> events and refreshes ScrollBar display.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			timerMouseDownRepeater.Enabled = false;
			MouseUpDownStatus = false;//Input.MouseButtonState.Released;

			switch( MouseScrollBarArea )
			{
			case EngineScrollBarMouseLocation.BottomOrRightArrow:
			case EngineScrollBarMouseLocation.TopOrLeftArrow:
			case EngineScrollBarMouseLocation.BottomOrRightTrack:
			case EngineScrollBarMouseLocation.TopOrLeftTrack:
				OnScroll( Value, Value, ScrollEventType.EndScroll );
				break;
			case EngineScrollBarMouseLocation.Thumb:
				OnScroll( Value, Value, ScrollEventType.ThumbPosition );
				OnScroll( Value, Value, ScrollEventType.EndScroll );
				break;
			}
			Invalidate();
		}

		#endregion

		#region Private helpers/wrappers of public events

		void OnValueChanged()
		{
			if( ( ValueChanged != null ) && ( this.Value != PreviousValue ) )
			{
				PreviousValue = this.Value;
				ValueChanged( this, new EventArgs() );
			}
		}

		void OnMouseClick( MouseEventArgs mouseArgs, int Value )//, List<ScrollBarBookmark> BookmarksOver )
		{
			if( MouseClick != null )
			{
				//Call event handler
				EngineScrollBarMouseEventArgs e = new EngineScrollBarMouseEventArgs( HotValue, mouseArgs, /*BookmarksOver, */MouseScrollBarArea );
				MouseClick( this, e );
			}

		}

		void OnMouseMove( MouseEventArgs ea, int HotValue )//, List<ScrollBarBookmark> BookmarksOver )
		{
			if( MouseMove != null )
			{
				//Call event handler
				EngineScrollBarMouseEventArgs e = new EngineScrollBarMouseEventArgs( HotValue, ea, /*BookmarksOver, */MouseScrollBarArea );
				MouseMove( this, e );
			}
		}

		void OnScroll( int newVal, int oldVal, ScrollEventType scrollEventType )
		{
			if( Scroll != null )
			{
				ScrollOrientation ScrollOrientation;
				if( Orientation == Orientation.Horizontal )
					ScrollOrientation = ScrollOrientation.HorizontalScroll;
				else
					ScrollOrientation = ScrollOrientation.VerticalScroll;

				//Make sure NewVal is within valid range
				newVal = Math.Max( Math.Min( Maximum, newVal ), Minimum );

				EngineScrollBarEventArgs ea = new EngineScrollBarEventArgs( oldVal, newVal, scrollEventType, ScrollOrientation );

				Scroll( this, ea );
			}
		}

		#endregion

		#region OnPaint override and DrawBookmark methods

		//void DrawBookmark( Graphics graphics, ScrollBarBookmark bookmark )
		//{
		//	BasicShapeScrollBarBookmark shapeBookmark = null;
		//	ImageScrollBarBookmark imageBookmark = null;

		//	if( bookmark is ValueRangeScrollBarBookmark )
		//	{
		//		shapeBookmark = (ValueRangeScrollBarBookmark)bookmark;
		//		int bookmarkLength = (int)( ( ( ( (ValueRangeScrollBarBookmark)shapeBookmark ).EndValue - shapeBookmark.Value ) / ( Maximum - Minimum ) ) * TrackLength );
		//		if( bookmarkLength == 0 ) bookmarkLength = 1;

		//		//For ValueRangeBook recalculate size in pixels
		//		if( Orientation == Orientation.Vertical )
		//			bookmark.Height = bookmarkLength;  //calculate hight for vertical
		//		else
		//			bookmark.Width = bookmarkLength;  //calculate width for horizontal
		//	}
		//	else if( bookmark is BasicShapeScrollBarBookmark )
		//		shapeBookmark = (BasicShapeScrollBarBookmark)bookmark;
		//	else if( bookmark is ImageScrollBarBookmark )
		//		imageBookmark = (ImageScrollBarBookmark)bookmark;


		//	if( ( shapeBookmark != null ) && ( shapeBookmark.Stretch ) )
		//	{
		//		if( Orientation == Orientation.Vertical )
		//		{
		//			bookmark.X = 0;
		//			shapeBookmark.Width = this.ClientSize.Width;
		//		}
		//		else
		//		{
		//			bookmark.Y = 0;
		//			shapeBookmark.Height = this.ClientSize.Height;
		//		}
		//	}
		//	else
		//	{
		//		CalculateBookmarkEdgePosition( bookmark );
		//	}

		//	//Calculate top Y position of bookmark 
		//	int ArrowLength;
		//	int BookmarkLength;
		//	if( Orientation == Orientation.Vertical )
		//	{
		//		ArrowLength = GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
		//		BookmarkLength = bookmark.Height;
		//	}
		//	else
		//	{
		//		ArrowLength = GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
		//		BookmarkLength = bookmark.Width;
		//	}
		//	int BookmarkLongPosition;
		//	if( Maximum == Minimum )
		//	{
		//		//Move position out of sight if bookmark value != Minimump
		//		BookmarkLongPosition = ( bookmark.Value == Minimum ) ? ArrowLength : -1000;
		//	}
		//	else
		//	{
		//		BookmarkLongPosition = (int)( TrackLength * ( bookmark.Value - Minimum ) / ( Maximum - Minimum ) + ArrowLength );
		//		if( !( bookmark is ValueRangeScrollBarBookmark ) )
		//			BookmarkLongPosition -= BookmarkLength / 2;
		//	}

		//	if( Orientation == Orientation.Vertical )
		//		bookmark.Y = BookmarkLongPosition;
		//	else
		//		bookmark.X = BookmarkLongPosition;


		//	if( imageBookmark != null )
		//	{
		//		if( imageBookmark.Image != null )
		//			graphics.DrawImage( imageBookmark.Image, new Point( bookmark.X, bookmark.Y ) );
		//	}

		//	else
		//	{
		//		//Make sure that brush needed for drawing is ready to use
		//		RefreshBrushFromCache( shapeBookmark );

		//		//Make sure that pen needed for drawing is ready to use
		//		RefreshPenFromCache( shapeBookmark );

		//		if( ( shapeBookmark != null ) && ( shapeBookmark.FillBookmarkShape ) )
		//		{
		//			if( shapeBookmark.Shape == ScrollbarBookmarkShape.Oval )
		//				graphics.FillEllipse( shapeBookmark.Brush, new System.Drawing.Rectangle( bookmark.X, bookmark.Y, shapeBookmark.Width, shapeBookmark.Height ) );
		//			else
		//				graphics.FillRectangle( shapeBookmark.Brush, new System.Drawing.Rectangle( bookmark.X, bookmark.Y, shapeBookmark.Width, shapeBookmark.Height ) );
		//		}
		//		else
		//		{
		//			if( shapeBookmark.Shape == ScrollbarBookmarkShape.Oval )
		//				graphics.DrawEllipse( shapeBookmark.Pen, new System.Drawing.Rectangle( bookmark.X, bookmark.X, shapeBookmark.Width, shapeBookmark.Height ) );
		//			else
		//				graphics.DrawRectangle( shapeBookmark.Pen, new System.Drawing.Rectangle( bookmark.X, bookmark.X, shapeBookmark.Width, shapeBookmark.Height ) );
		//		}
		//	}
		//}

		int ArrowLegth()
		{
			return GetSize();
			//return Orientation == Orientation.Vertical ? GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ : GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
		}

		//void CalculateBookmarkEdgePosition( ScrollBarBookmark bookmark )
		//{
		//	switch( bookmark.Alignment )
		//	{
		//	case ScrollBarBookmarkAlignment.LeftOrTop:
		//		if( Orientation == Orientation.Vertical )
		//			bookmark.X = 0;
		//		else
		//			bookmark.Y = 0;
		//		break;
		//	case ScrollBarBookmarkAlignment.RightOrBottom:
		//		if( Orientation == Orientation.Vertical )
		//			bookmark.X = this.ClientSize.Width - bookmark.Width;
		//		else
		//			bookmark.Y = this.ClientSize.Height - bookmark.Height;
		//		break;
		//	case ScrollBarBookmarkAlignment.Center:
		//		if( Orientation == Orientation.Vertical )
		//			bookmark.X = ( this.ClientSize.Width - bookmark.Width ) / 2;
		//		else
		//			bookmark.Y = ( this.ClientSize.Height - bookmark.Height ) / 2;

		//		break;
		//	}
		//}

		Point ToPoint( Vector2I v )
		{
			return new Point( v.X, v.Y );
		}

		void DrawArrowButton( Graphics g, System.Drawing.Rectangle bounds, ScrollBarArrowButtonState state )
		{
#if !DEPLOY

			Color color = Color.Red;
			Color color2 = Color.Red;
			switch( state )
			{
			case ScrollBarArrowButtonState.UpNormal:
			case ScrollBarArrowButtonState.LeftNormal:
			case ScrollBarArrowButtonState.DownNormal:
			case ScrollBarArrowButtonState.RightNormal:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				color2 = EditorAPI.DarkTheme ? Color.FromArgb( 130, 130, 130 ) : Color.FromArgb( 96, 96, 96 );
				//color2 = EditorAPI.DarkTheme ? Color.FromArgb( 110, 110, 110 ) : Color.FromArgb( 96, 96, 96 );
				break;
			case ScrollBarArrowButtonState.UpHot:
			case ScrollBarArrowButtonState.LeftHot:
			case ScrollBarArrowButtonState.DownHot:
			case ScrollBarArrowButtonState.RightHot:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 80, 80, 80 ) : Color.FromArgb( 218, 218, 218 );
				color2 = EditorAPI.DarkTheme ? Color.FromArgb( 150, 150, 150 ) : Color.FromArgb( 0, 0, 0 );
				//color2 = EditorAPI.DarkTheme ? Color.FromArgb( 120, 120, 120 ) : Color.FromArgb( 0, 0, 0 );
				break;
			case ScrollBarArrowButtonState.UpPressed:
			case ScrollBarArrowButtonState.LeftPressed:
			case ScrollBarArrowButtonState.DownPressed:
			case ScrollBarArrowButtonState.RightPressed:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 90, 90, 90 ) : Color.FromArgb( 96, 96, 96 );
				color2 = EditorAPI.DarkTheme ? Color.FromArgb( 170, 170, 170 ) : Color.FromArgb( 255, 255, 255 );
				//color2 = EditorAPI.DarkTheme ? Color.FromArgb( 140, 140, 140 ) : Color.FromArgb( 255, 255, 255 );
				break;
			case ScrollBarArrowButtonState.UpDisabled:
			case ScrollBarArrowButtonState.LeftDisabled:
			case ScrollBarArrowButtonState.DownDisabled:
			case ScrollBarArrowButtonState.RightDisabled:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				color2 = EditorAPI.DarkTheme ? Color.FromArgb( 90, 90, 90 ) : Color.FromArgb( 200, 200, 200 );
				break;
			}

			using( var brush = new SolidBrush( color ) )
			{
				g.FillRectangle( brush, bounds );
			}

			using( var brush = new SolidBrush( color2 ) )
			{
				RectangleI b2 = new RectangleI( bounds.Left, bounds.Top, bounds.Right, bounds.Bottom );

				var size = b2.Size;
				//var size2 = ( size.MinComponent() / 2 ) / 2 * 2;
				var size2 = ( size.MinComponent() / 2 + 1 ) / 2 * 2;
				var center = b2.GetCenter();

				var b = new RectangleI( center.X - size2 / 2, center.Y - size2 / 2, center.X + size2 / 2, center.Y + size2 / 2 );

				Point[] points = null;
				Point[] points2 = null;

				if( state.ToString().Contains( "Up" ) || state.ToString().Contains( "Down" ) )
				{
					var sz = b.GetCenter().X - b.Left;
					var offset = (int)( (double)sz / 1.5 );

					if( state.ToString().Contains( "Up" ) )
					{
						var topPoint = new Vector2I( center.X, center.Y - sz - offset / 2 + 1 );

						points = new Point[ 4 ];
						points[ 0 ] = ToPoint( topPoint + new Vector2I( -sz, sz ) + new Vector2I( 1, 0 ) );
						points[ 1 ] = ToPoint( topPoint + new Vector2I( 1, 0 ) );
						points[ 2 ] = ToPoint( topPoint + new Vector2I( 0, offset ) + new Vector2I( 1, 0 ) );
						points[ 3 ] = ToPoint( topPoint + new Vector2I( -sz, sz ) + new Vector2I( 0, offset ) + new Vector2I( 1, 0 ) );

						points2 = new Point[ 4 ];
						points2[ 0 ] = ToPoint( topPoint + new Vector2I( +sz, sz ) );
						points2[ 1 ] = ToPoint( topPoint + new Vector2I( +sz, sz ) + new Vector2I( 0, offset ) );
						points2[ 2 ] = ToPoint( topPoint + new Vector2I( 0, offset ) );
						points2[ 3 ] = ToPoint( topPoint );

						//points[ 0 ] = ToPoint( topPoint + new Vector2I( -sz, sz ) );
						//points[ 1 ] = ToPoint( topPoint );
						//points[ 2 ] = ToPoint( topPoint + new Vector2I( sz, sz ) );
						//points[ 3 ] = ToPoint( topPoint + new Vector2I( sz, sz ) + new Vector2I( 0, offset ) );
						//points[ 4 ] = ToPoint( topPoint + new Vector2I( 0, offset ) );
						//points[ 5 ] = ToPoint( topPoint + new Vector2I( -sz, sz ) + new Vector2I( 0, offset ) );
					}
					else
					{
						var bottomPoint = new Vector2I( center.X, center.Y + sz + offset / 2 - 1 );

						points = new Point[ 4 ];
						points[ 0 ] = ToPoint( bottomPoint + new Vector2I( -sz, -sz ) + new Vector2I( 1, 0 ) );
						points[ 1 ] = ToPoint( bottomPoint + new Vector2I( 1, 0 ) );
						points[ 2 ] = ToPoint( bottomPoint - new Vector2I( 0, offset ) + new Vector2I( 1, 0 ) );
						points[ 3 ] = ToPoint( bottomPoint + new Vector2I( -sz, -sz ) - new Vector2I( 0, offset ) + new Vector2I( 1, 0 ) );

						points2 = new Point[ 4 ];
						points2[ 0 ] = ToPoint( bottomPoint + new Vector2I( sz, -sz ) - new Vector2I( 0, offset ) );
						points2[ 1 ] = ToPoint( bottomPoint + new Vector2I( sz, -sz ) );
						points2[ 2 ] = ToPoint( bottomPoint );
						points2[ 3 ] = ToPoint( bottomPoint - new Vector2I( 0, offset ) );

						//points = new Point[ 6 ];
						//points[ 0 ] = ToPoint( bottomPoint + new Vector2I( sz, -sz ) );
						//points[ 1 ] = ToPoint( bottomPoint );
						//points[ 2 ] = ToPoint( bottomPoint + new Vector2I( -sz, -sz ) );
						//points[ 3 ] = ToPoint( bottomPoint + new Vector2I( -sz, -sz ) - new Vector2I( 0, offset ) );
						//points[ 4 ] = ToPoint( bottomPoint - new Vector2I( 0, offset ) );
						//points[ 5 ] = ToPoint( bottomPoint + new Vector2I( sz, -sz ) - new Vector2I( 0, offset ) );
					}
				}

				if( state.ToString().Contains( "Left" ) || state.ToString().Contains( "Right" ) )
				{
					var sz = b.GetCenter().Y - b.Top;
					var offset = (int)( (double)sz / 1.5 );

					if( state.ToString().Contains( "Left" ) )
					{
						var leftPoint = new Vector2I( center.X - sz - offset / 2 + 1, center.Y );

						points = new Point[ 4 ];
						points[ 0 ] = ToPoint( leftPoint + new Vector2I( sz, -sz ) + new Vector2I( 0, 1 ) );
						points[ 1 ] = ToPoint( leftPoint + new Vector2I( 0, 1 ) );
						points[ 2 ] = ToPoint( leftPoint + new Vector2I( offset, 0 ) + new Vector2I( 0, 1 ) );
						points[ 3 ] = ToPoint( leftPoint + new Vector2I( sz, -sz ) + new Vector2I( offset, 0 ) + new Vector2I( 0, 1 ) );

						points2 = new Point[ 4 ];
						points2[ 0 ] = ToPoint( leftPoint + new Vector2I( sz, sz ) + new Vector2I( 1, 0 ) );
						points2[ 1 ] = ToPoint( leftPoint + new Vector2I( sz, sz ) + new Vector2I( offset, 0 ) + new Vector2I( 1, 0 ) );
						points2[ 2 ] = ToPoint( leftPoint + new Vector2I( offset, 0 ) + new Vector2I( 1, 0 ) );
						points2[ 3 ] = ToPoint( leftPoint + new Vector2I( 1, 0 ) );
					}
					else
					{
						var rightPoint = new Vector2I( center.X + sz + offset / 2 - 1, center.Y );

						points = new Point[ 4 ];
						points[ 0 ] = ToPoint( rightPoint + new Vector2I( -sz, -sz ) + new Vector2I( 0, 1 ) );
						points[ 1 ] = ToPoint( rightPoint + new Vector2I( 0, 1 ) );
						points[ 2 ] = ToPoint( rightPoint - new Vector2I( offset, 0 ) + new Vector2I( 0, 1 ) );
						points[ 3 ] = ToPoint( rightPoint + new Vector2I( -sz, -sz ) - new Vector2I( offset, 0 ) + new Vector2I( 0, 1 ) );

						points2 = new Point[ 4 ];
						points2[ 0 ] = ToPoint( rightPoint + new Vector2I( -sz, sz ) - new Vector2I( offset, 0 ) + new Vector2I( -1, 0 ) );
						points2[ 1 ] = ToPoint( rightPoint + new Vector2I( -sz, sz ) + new Vector2I( -1, 0 ) );
						points2[ 2 ] = ToPoint( rightPoint + new Vector2I( -1, 0 ) );
						points2[ 3 ] = ToPoint( rightPoint - new Vector2I( offset, 0 ) + new Vector2I( -1, 0 ) );
					}
				}

				if( points != null )
					g.FillPolygon( brush, points );
				if( points2 != null )
					g.FillPolygon( brush, points2 );
			}

			//if( EditorAPI.DarkTheme )
			//{
			//	Color color = Color.FromArgb( 47, 47, 47 );

			//	switch( state )
			//	{
			//	case ScrollBarState.Disabled:
			//		color = Color.FromArgb( 47, 47, 47 );
			//		break;
			//	case ScrollBarState.Pressed:
			//		color = Color.FromArgb( 80, 80, 80 );
			//		break;
			//	case ScrollBarState.Normal:
			//		color = Color.FromArgb( 47, 47, 47 );
			//		break;
			//	case ScrollBarState.Hot:
			//		color = Color.FromArgb( 65, 65, 65 );
			//		break;
			//	}

			//	e.Graphics.FillRectangle( new SolidBrush( color ), rec );

			//	//e.Graphics.FillRectangle( new SolidBrush( Color.FromArgb( 255, 0, 0 ) ), rec );
			//	//ScrollBarRenderer.DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.UpHot ); break;
			//}
			//else
			//{


			//ScrollBarRenderer.DrawArrowButton( g, bounds, state );

#endif
		}

		void DrawTrack( Graphics g, System.Drawing.Rectangle bounds, ScrollBarState state )
		{
			Color color = Color.Red;
			switch( state )
			{
			case ScrollBarState.Normal:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				break;
			case ScrollBarState.Hot:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				break;
			case ScrollBarState.Pressed:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				break;
			case ScrollBarState.Disabled:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 47, 47, 47 ) : Color.FromArgb( 240, 240, 240 );
				break;
			}

#if !DEPLOY
			using( var brush = new SolidBrush( color ) )
			{
				g.FillRectangle( brush, bounds );
			}
#endif
		}

		void DrawUpperVerticalTrack( Graphics g, System.Drawing.Rectangle bounds, ScrollBarState state )
		{
			DrawTrack( g, bounds, state );
			//ScrollBarRenderer.DrawUpperVerticalTrack( g, bounds, state );
		}

		void DrawLeftHorizontalTrack( Graphics g, System.Drawing.Rectangle bounds, ScrollBarState state )
		{
			DrawTrack( g, bounds, state );
			//ScrollBarRenderer.DrawLeftHorizontalTrack( g, bounds, state );
		}

		void DrawLowerVerticalTrack( Graphics g, System.Drawing.Rectangle bounds, ScrollBarState state )
		{
			DrawTrack( g, bounds, state );
			//ScrollBarRenderer.DrawLowerVerticalTrack( g, bounds, state );
		}

		void DrawRightHorizontalTrack( Graphics g, System.Drawing.Rectangle bounds, ScrollBarState state )
		{
			DrawTrack( g, bounds, state );
			//ScrollBarRenderer.DrawRightHorizontalTrack( g, bounds, state );
		}

		void DrawThumb( Graphics graphics, int thumbLength, int thumbPosition )
		{
			//!!!!betauser
			if( thumbPosition < ArrowLegth() )
			{
				thumbLength -= ArrowLegth() - thumbPosition;
				thumbPosition = ArrowLegth();
			}

			ScrollBarState state = GetScrollBarAreaState( EngineScrollBarMouseLocation.Thumb );

			System.Drawing.Rectangle rec;
			if( Orientation == Orientation.Vertical )
				rec = new System.Drawing.Rectangle( 0, thumbPosition, ClientSize.Width, thumbLength );
			else
				rec = new System.Drawing.Rectangle( thumbPosition, 0, thumbLength, ClientSize.Height );

			Color color = Color.Red;
			switch( state )
			{
			case ScrollBarState.Normal:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 60, 60, 60 ) : Color.FromArgb( 205, 205, 205 );
				break;
			case ScrollBarState.Hot:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 70, 70, 70 ) : Color.FromArgb( 166, 166, 166 );
				break;
			case ScrollBarState.Pressed:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 80, 80, 80 ) : Color.FromArgb( 96, 96, 96 );
				break;
			case ScrollBarState.Disabled:
				color = EditorAPI.DarkTheme ? Color.FromArgb( 54, 54, 54 ) : Color.FromArgb( 220, 220, 220 );
				break;
			}

#if !DEPLOY
			using( var brush = new SolidBrush( color ) )
			{
				graphics.FillRectangle( brush, rec );
			}
#endif

			//ScrollBarState state = GetScrollBarAreaState( EnhancedScrollBarMouseLocation.Thumb );
			//System.Drawing.Rectangle rec;
			//if( Orientation == Orientation.Vertical )
			//{
			//	rec = new System.Drawing.Rectangle( 0, thumbPosition, ClientSize.Width, thumbLength );

			//	ScrollBarRenderer.DrawVerticalThumb( graphics, rec, state );
			//	//draw thumb grip
			//	if( thumbLength >= SystemInformation.VerticalScrollBarThumbHeight )
			//		ScrollBarRenderer.DrawVerticalThumbGrip( graphics, rec, ScrollBarState.Normal );
			//}
			//else
			//{
			//	rec = new System.Drawing.Rectangle( thumbPosition, 0, thumbLength, ClientSize.Height );

			//	ScrollBarRenderer.DrawHorizontalThumb( graphics, rec, state );
			//	//draw thumb grip
			//	if( thumbLength >= SystemInformation.HorizontalScrollBarThumbWidth )
			//		ScrollBarRenderer.DrawHorizontalThumbGrip( graphics, rec, ScrollBarState.Normal );
			//}
		}

		/// <summary>
		/// Overridden OnPaint. Draws all EnhancedScrollBar elements and draws all associated bookmarks.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			//Draw top arrow
			System.Drawing.Rectangle rec;
			ScrollBarState state = GetScrollBarAreaState( EngineScrollBarMouseLocation.TopOrLeftArrow );
			if( Orientation == Orientation.Vertical )
			{
				rec = new System.Drawing.Rectangle( 0, 0, ClientSize.Width, ArrowLegth() );
				switch( state )
				{
				case ScrollBarState.Disabled:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.UpDisabled ); break;
				case ScrollBarState.Pressed:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.UpPressed ); break;
				case ScrollBarState.Normal:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.UpNormal ); break;
				case ScrollBarState.Hot:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.UpHot ); break;
				}
			}
			else
			{
				rec = new System.Drawing.Rectangle( 0, 0, ArrowLegth(), ClientSize.Height );
				switch( state )
				{
				case ScrollBarState.Disabled:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.LeftDisabled ); break;
				case ScrollBarState.Pressed:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.LeftPressed ); break;
				case ScrollBarState.Normal:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.LeftNormal ); break;
				case ScrollBarState.Hot:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.LeftHot ); break;
				}
			}

			//Draw top track
			int ThumbPos = Value2ThumbTopPosition( Value );
			state = GetScrollBarAreaState( EngineScrollBarMouseLocation.TopOrLeftTrack );
			if( Orientation == Orientation.Vertical )
			{
				rec = new System.Drawing.Rectangle( 0, ArrowLegth(), ClientSize.Width, ThumbPos - ArrowLegth() );
				//rec = new System.Drawing.Rectangle( 0, ArrowLegth() + 1, ClientSize.Width, ThumbPos - ArrowLegth() );
				DrawUpperVerticalTrack( e.Graphics, rec, state );
			}
			else
			{
				rec = new System.Drawing.Rectangle( ArrowLegth(), 0, ThumbPos - ArrowLegth(), ClientSize.Height );
				//rec = new System.Drawing.Rectangle( ArrowLegth() + 1, 0, ThumbPos - ArrowLegth(), ClientSize.Height );
				DrawLeftHorizontalTrack( e.Graphics, rec, state );
			}

			//draw thumb
			int nThumbLength = ThumbLength;
			DrawThumb( e.Graphics, nThumbLength, ThumbPos );

			//Draw bottom track
			state = GetScrollBarAreaState( EngineScrollBarMouseLocation.BottomOrRightTrack );
			if( Orientation == Orientation.Vertical )
			{
				rec = new System.Drawing.Rectangle( 0, ThumbPos + nThumbLength, ClientSize.Width, TrackLength + ArrowLegth() - ( ThumbPos + nThumbLength ) );
				DrawLowerVerticalTrack( e.Graphics, rec, state );
			}
			else
			{
				rec = new System.Drawing.Rectangle( ThumbPos + nThumbLength, 0, TrackLength + ArrowLegth() - ( ThumbPos + nThumbLength ), ClientSize.Height );
				DrawRightHorizontalTrack( e.Graphics, rec, state );
			}

			//Draw bottom arrow 
			state = GetScrollBarAreaState( EngineScrollBarMouseLocation.BottomOrRightArrow );
			if( Orientation == Orientation.Vertical )
			{
				rec = new System.Drawing.Rectangle( 0, ClientSize.Height - ArrowLegth(), ClientSize.Width, ArrowLegth() );
				switch( state )
				{
				case ScrollBarState.Disabled:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.DownDisabled ); break;
				case ScrollBarState.Pressed:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.DownPressed ); break;
				case ScrollBarState.Normal:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.DownNormal ); break;
				case ScrollBarState.Hot:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.DownHot ); break;
				}
			}
			else
			{
				rec = new System.Drawing.Rectangle( ClientSize.Width - ArrowLegth(), 0, ArrowLegth(), ClientSize.Height );
				switch( state )
				{
				case ScrollBarState.Disabled:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.RightDisabled ); break;
				case ScrollBarState.Pressed:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.RightPressed ); break;
				case ScrollBarState.Normal:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.RightNormal ); break;
				case ScrollBarState.Hot:
					DrawArrowButton( e.Graphics, rec, ScrollBarArrowButtonState.RightHot ); break;
				}
			}

			////Draw  bookmarks 
			//DrawBookmarks( e.Graphics );

			//if( !BookmarksOnTop )  //Redraw thumb over bookmarks
			//	DrawThumb( e.Graphics, nThumbLength, ThumbPos );

		}

		//void DrawBookmarks( Graphics g )
		//{
		//	if( Bookmarks != null )
		//	{
		//		foreach( ScrollBarBookmark bk in Bookmarks )
		//			DrawBookmark( g, bk );
		//	}

		//}

		//void RefreshBrushFromCache( BasicShapeScrollBarBookmark shapeBookmark )
		//{
		//	if( shapeBookmark.Brush == null )  //this bookmark is used to the first time
		//	{
		//		if( !m_BrushesCache.ContainsKey( shapeBookmark.Color ) )  //this color is used for the first time
		//			m_BrushesCache.Add( shapeBookmark.Color, new SolidBrush( shapeBookmark.Color ) );

		//		shapeBookmark.Brush = m_BrushesCache[ shapeBookmark.Color ];
		//	}
		//}

		//void RefreshPenFromCache( BasicShapeScrollBarBookmark shapeBookmark )
		//{
		//	if( shapeBookmark.Pen == null )  //this bookmark is used to the first time
		//	{
		//		if( !m_PensCache.ContainsKey( shapeBookmark.Color ) )  //this color is used for the first time
		//			m_PensCache.Add( shapeBookmark.Color, new Pen( shapeBookmark.Color ) );

		//		shapeBookmark.Pen = m_PensCache[ shapeBookmark.Color ];
		//	}
		//}

		ScrollBarState GetScrollBarAreaState( EngineScrollBarMouseLocation mouseHotLocation )
		{
			if( this.Enabled )
			{
				if( MouseScrollBarArea == mouseHotLocation )
					return MouseUpDownStatus == true/*Input.MouseButtonState.Pressed*/ ? ScrollBarState.Pressed : ScrollBarState.Hot;
				else
					return ScrollBarState.Normal;
			}
			else
				return ScrollBarState.Disabled;

		}

#endregion

#region helper methods

		void NavigateTo( int NewValue )
		{
			OnScroll( Value, Value, ScrollEventType.ThumbTrack );
			OnScroll( Value, NewValue, ScrollEventType.ThumbTrack );
			Value = NewValue;
			OnScroll( Value, Value, ScrollEventType.ThumbPosition );
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		[Browsable( false )]
		/*public */
		int TrackLength
		{
			get
			{
				if( Orientation == Orientation.Vertical )
					return this.ClientSize.Height - 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
				else
					return this.ClientSize.Width - 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
			}
		}

		int ThumbLength
		{
			get
			{
				if( Minimum == Maximum ) return TrackLength;

				var totalSize = ItemSize * ( Maximum - Minimum + 1 ) + ( Orientation == Orientation.Vertical ? Height : Width );
				if( totalSize <= TrackLength || totalSize == 0 )
					return TrackLength;

				var scale = (double)TrackLength / (double)totalSize;
				int thumbLength = (int)( scale * TrackLength );

				//int thumbLength = (int)( (double)TrackLength * (double)LargeChange / ( (double)Maximum - (double)Minimum + 1 ) );

				//int nThumbLength = (int)( (double)TrackLength * (double)LargeChange / ( (double)Maximum - (double)Minimum + 1 - (double)LargeChange ) );
				//int nThumbLength = (int)( TrackLength / ( Maximum - Minimum + 1 ) );

				if( Orientation == Orientation.Vertical )
				{
					if( thumbLength < GetSize()/*SystemInformation.VerticalScrollBarThumbHeight*/ )
						thumbLength = GetSize()/*SystemInformation.VerticalScrollBarThumbHeight*/;
				}
				else
				{
					if( thumbLength < GetSize()/*SystemInformation.HorizontalScrollBarThumbWidth*/ )
						thumbLength = GetSize()/*SystemInformation.HorizontalScrollBarThumbWidth*/;
				}
				return thumbLength;
			}
		}

		int Value2ThumbTopPosition( int value )
		{
			if( Orientation == Orientation.Vertical )
			{
				if( Maximum == Minimum ) return GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;

				decimal ratio = (decimal)( ClientSize.Height - 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ - ThumbLength ) / ( Maximum - Minimum );
				return (int)( ( value - Minimum ) * ratio ) + GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
			}
			else
			{
				if( Maximum == Minimum ) return GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;

				decimal ratio = (decimal)( ClientSize.Width - 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ - ThumbLength ) / ( Maximum - Minimum );
				return (int)( ( value - Minimum ) * ratio ) + GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
			}
		}

		int ThumbTopPosition2Value( int y )
		{
			if( Orientation == Orientation.Vertical )
			{
				if( ClientSize.Height - 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ - ThumbLength == 0 )
					return 0;
				else
				{
					decimal ratio = (decimal)( ( y - GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ ) ) / ( ClientSize.Height - 2 * GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ - ThumbLength );
					return (int)( ( Maximum - Minimum ) * ratio + Minimum );
				}
			}
			else
			{
				if( ClientSize.Width - 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ - ThumbLength == 0 )
					return 0;
				else
				{
					decimal ratio = (decimal)( ( y - GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ ) ) / ( ClientSize.Width - 2 * GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ - ThumbLength );
					return (int)( ( Maximum - Minimum ) * ratio + Minimum );
				}
			}
		}

		EngineScrollBarMouseLocation MouseLocation( int absolutePosition, out int relativeY )
		{
			if( Orientation == Orientation.Vertical )
			{
				if( absolutePosition <= GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ )
				{
					relativeY = absolutePosition;
					return EngineScrollBarMouseLocation.TopOrLeftArrow;
				}
				else if( absolutePosition > ClientSize.Height - GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/ )
				{
					relativeY = absolutePosition - ClientSize.Height + GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
					return EngineScrollBarMouseLocation.BottomOrRightArrow;
				}
				else
				{
					int thumbTop = Value2ThumbTopPosition( Value );
					if( absolutePosition < thumbTop )
					{
						relativeY = absolutePosition - GetSize()/*SystemInformation.VerticalScrollBarArrowHeight*/;
						return EngineScrollBarMouseLocation.TopOrLeftTrack;
					}
					else if( absolutePosition < thumbTop + ThumbLength )
					{
						relativeY = absolutePosition - thumbTop;
						return EngineScrollBarMouseLocation.Thumb;
					}
					else
					{
						relativeY = absolutePosition - thumbTop - ThumbLength;
						return EngineScrollBarMouseLocation.BottomOrRightTrack;
					}
				}
			}
			else
			{
				if( absolutePosition <= GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ )
				{
					relativeY = absolutePosition;
					return EngineScrollBarMouseLocation.TopOrLeftArrow;
				}
				else if( absolutePosition > ClientSize.Width - GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/ )
				{
					relativeY = absolutePosition - ClientSize.Width + GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
					return EngineScrollBarMouseLocation.BottomOrRightArrow;
				}
				else
				{
					int thumbTop = Value2ThumbTopPosition( Value );
					if( absolutePosition < thumbTop )
					{
						relativeY = absolutePosition - GetSize()/*SystemInformation.HorizontalScrollBarArrowWidth*/;
						return EngineScrollBarMouseLocation.TopOrLeftTrack;
					}
					else if( absolutePosition < thumbTop + ThumbLength )
					{
						relativeY = absolutePosition - thumbTop;
						return EngineScrollBarMouseLocation.Thumb;
					}
					else
					{
						relativeY = absolutePosition - thumbTop - ThumbLength;
						return EngineScrollBarMouseLocation.BottomOrRightTrack;
					}
				}
			}
		}

		//List<ScrollBarBookmark> BookmarksUnderPosition( int x, int y )
		//{
		//	List<ScrollBarBookmark> bookmarksUnderPosition = new List<ScrollBarBookmark>();
		//	if( Bookmarks != null )
		//	{
		//		foreach( ScrollBarBookmark bookmark in Bookmarks )
		//		{
		//			if( Orientation == Orientation.Vertical )
		//			{
		//				if( ( bookmark.Y <= y ) && ( bookmark.Y + bookmark.Height >= y ) && ( bookmark.X <= x ) && ( bookmark.X + bookmark.Width >= x ) )
		//					bookmarksUnderPosition.Add( bookmark );
		//			}
		//			else
		//			{
		//				if( bookmark is BasicShapeScrollBarBookmark )
		//				{
		//					if( ( bookmark.Y <= y ) && ( bookmark.Y + bookmark.Height >= y ) && ( bookmark.X <= x ) && ( bookmark.X + bookmark.Width >= x ) )
		//						bookmarksUnderPosition.Add( bookmark );

		//				}
		//				else
		//				{
		//					if( ( bookmark.X <= x ) && ( bookmark.X + bookmark.Width >= x ) && ( bookmark.Y <= y ) && ( bookmark.Y + bookmark.Height >= y ) )
		//						bookmarksUnderPosition.Add( bookmark );
		//				}
		//			}
		//		}
		//	}
		//	return bookmarksUnderPosition;
		//}

#endregion

#region Context menu event handlers

		void topToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Minimum, Value, ScrollEventType.First );
			Value = Minimum;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		void bottomToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Maximum, Value, ScrollEventType.Last );
			Value = Maximum;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		void scrollHereToolStripMenuItem_Click( object sender, EventArgs e )
		{
			NavigateTo( HotValue );
		}

		void pageUpToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Value - LargeChange, Value, ScrollEventType.LargeDecrement );
			Value -= LargeChange;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		void pageDownToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Value + LargeChange, Value, ScrollEventType.LargeIncrement );
			Value += LargeChange;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		void scrollUpToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Value - SmallChange, Value, ScrollEventType.SmallDecrement );
			Value -= SmallChange;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

		void scrollDownToolStripMenuItem_Click( object sender, EventArgs e )
		{
			OnScroll( Value + SmallChange, Value, ScrollEventType.SmallIncrement );
			Value += SmallChange;
			OnScroll( Value, Value, ScrollEventType.EndScroll );
		}

#endregion

		//protected override CreateParams CreateParams
		//{
		//	get
		//	{
		//		CreateParams handleParam = base.CreateParams;
		//		handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
		//		return handleParam;
		//	}
		//}

		string TranslateContextMenu( string text )
		{
			return EditorContextMenuWinForms.Translate( text );
		}

		void ShowContextMenu( Point location )
		{
			var items = new List<KryptonContextMenuItemBase>();

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Scroll Here" ), null, delegate ( object s, EventArgs e2 )
				{
					NavigateTo( HotValue );
				} );
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Top" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Minimum, Value, ScrollEventType.First );
					Value = Minimum;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Bottom" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Maximum, Value, ScrollEventType.Last );
					Value = Maximum;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Page Up" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Value - LargeChange, Value, ScrollEventType.LargeDecrement );
					Value -= LargeChange;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Page Down" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Value + LargeChange, Value, ScrollEventType.LargeIncrement );
					Value += LargeChange;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Scroll Up" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Value - SmallChange, Value, ScrollEventType.SmallDecrement );
					Value -= SmallChange;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			{
				var item = new KryptonContextMenuItem( TranslateContextMenu( "Scroll Down" ), null, delegate ( object s, EventArgs e2 )
				{
					OnScroll( Value + SmallChange, Value, ScrollEventType.SmallIncrement );
					Value += SmallChange;
					OnScroll( Value, Value, ScrollEventType.EndScroll );
				} );
				items.Add( item );
			}

			//EditorContextMenu.AddActionsToMenu( EditorContextMenu.MenuTypeEnum.Document, items );

			EditorContextMenuWinForms.Show( items, this );
		}

		private void control_MouseClick( object sender, MouseEventArgs e )
		{
			//if( e.Button == MouseButtons.Right )
			//	ShowContextMenu( e.Location );
		}

		private void control_MouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
				ShowContextMenu( e.Location );
		}

		int GetSize()
		{
			return Orientation == Orientation.Vertical ? Width : Height;
		}

	}

#region Enhanced event argument definitions

	///// <summary>
	///// Argument for <c>ToolTipNeeded</c> event.
	///// </summary>
	//public class TooltipNeededEventArgs : EventArgs
	//{
	//	/// <summary>
	//	/// Constructor
	//	/// </summary>
	//	/// <param name="Value">ToolTip value. Hot ToolTip value mouse is moving over.</param>
	//	/// <param name="ToolTip">Default ToolTip message.</param>
	//	///// <param name="Bookmarks">List of over-wrapping bookmarks under <c>Value</c> position.</param>
	//	public TooltipNeededEventArgs( decimal Value, string ToolTip )//, List<ScrollBarBookmark> Bookmarks )
	//	{
	//		this.Value = Value;
	//		this.ToolTip = ToolTip;
	//		//this.Bookmarks = Bookmarks;
	//	}

	//	/// <summary>
	//	/// ToolTip value. Hot ToolTip value mouse is moving over.
	//	/// </summary>
	//	public decimal Value { set; get; }

	//	/// <summary>
	//	/// Default ToolTip message.
	//	/// </summary>
	//	public string ToolTip { set; get; }

	//	///// <summary>
	//	///// List of over-wrapping bookmarks under <c>Value</c> position.
	//	///// </summary>
	//	//public List<ScrollBarBookmark> Bookmarks { set; get; }
	//}

	/// <summary>
	/// Arguments for EnhancedScrollEvent.
	/// </summary>
	public class EngineScrollBarEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EngineScrollBarEventArgs()
		{
			this.NewValue = 0;
			this.OldValue = 0;
			this.Type = ScrollEventType.EndScroll;
			this.ScrollOrientation = ScrollOrientation.VerticalScroll;

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="OldValue">Previous EnhancedScrollBar value.</param>
		/// <param name="NewValue">New EnhancedScrollBar value.</param>
		/// <param name="Type">Type of the scroll event.</param>
		/// <param name="ScrollOrientation">EnhancedScrollBar orientation.</param>
		public EngineScrollBarEventArgs( int OldValue, int NewValue, ScrollEventType Type, ScrollOrientation ScrollOrientation )
		{
			this.NewValue = NewValue;
			this.OldValue = OldValue;
			this.ScrollOrientation = ScrollOrientation;
			this.Type = Type;
		}

		/// <summary>
		/// Previous EnhancedScrollBar value.
		/// </summary>
		public int OldValue { set; get; }

		/// <summary>
		/// New EnhancedScrollBar value.
		/// </summary>
		public int NewValue { set; get; }

		/// <summary>
		/// EnhancedScrollBar orientation.
		/// </summary>
		public ScrollOrientation ScrollOrientation { set; get; }

		/// <summary>
		/// Type of the scroll event.
		/// </summary>
		public ScrollEventType Type { set; get; }
	}

	/// <summary>
	/// Arguments for mouse related events.
	/// </summary>
	public class EngineScrollBarMouseEventArgs : MouseEventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Value">ScrollBar <c>Value when event occurred.</c></param>
		/// <param name="MouseArgs">Original <c>MouseArgs</c>.</param>
		///// <param name="Bookmarks">List of bookmarks under mouse position.</param>
		/// <param name="ScrollBarSection">Section of the EnhancedScrollBar where mouse pointer is located.</param>
		public EngineScrollBarMouseEventArgs( int Value, MouseEventArgs MouseArgs, /*List<ScrollBarBookmark> Bookmarks, */EngineScrollBarMouseLocation ScrollBarSection ) : base( MouseArgs.Button, MouseArgs.Clicks, MouseArgs.X, MouseArgs.Y, MouseArgs.Delta )
		{

			this.Value = Value;
			//this.Bookmarks = Bookmarks;
			this.ScrollBarSection = ScrollBarSection;

		}

		/// <summary>
		/// ScrollBar <c>Value</c> when event occurred.
		/// </summary>
		public int Value { set; get; }

		///// <summary>
		///// List of bookmarks under mouse position.
		///// </summary>
		//public List<ScrollBarBookmark> Bookmarks { set; get; }

		/// <summary>
		/// Section of the EnhancedScrollBar where mouse pointer is located.
		/// </summary>
		public EngineScrollBarMouseLocation ScrollBarSection { set; get; }
	}

#endregion

#region Public Enumerators

	/// <summary>
	/// Area of ScrollBar definitions. Used to describe relation of mouse pointer location
	/// to the distinct part of ScrollBar.
	/// </summary>
	public enum EngineScrollBarMouseLocation
	{
		/// <summary>
		/// Located outside of the ScrollBar.
		/// </summary>
		OutsideScrollBar,

		/// <summary>
		/// Located over top (for vertical ScrollBar) or 
		/// over left hand side arrow (for horizontal ScrollBar).
		/// </summary>
		TopOrLeftArrow,

		/// <summary>
		/// Located over top (for vertical Scrollbar) or
		/// over left hand side track (for horizontal ScrollBar).
		/// Track is the area between arrow and thumb images.
		/// </summary>
		TopOrLeftTrack,

		/// <summary>
		/// Located over ScrollBar thumb. Thumb is movable portion of the ScrollBar.
		/// </summary>
		Thumb,

		/// <summary>
		/// Located over bottom (for vertical Scrollbar) or
		/// over right hand side track (for horizontal ScrollBar).
		/// Track is the area between arrow and thumb images.
		/// </summary>
		BottomOrRightTrack,

		/// <summary>
		/// Located over bottom (for vertical ScrollBar) or 
		/// over right hand side arrow (for horizontal ScrollBar).
		/// </summary>
		BottomOrRightArrow
	}

#endregion

	//#region Collection editor for Bookmark collection, to allow editing in Design Mode.

	///// <summary>
	///// Collection editor for bookmark. Without it, adding bookmark in design mode is impossible 
	///// (exception is thrown). With it collection editor add button has drop-down list to pick from.
	///// List is defined in CreateNewItemTypes override.
	///// </summary>
	//public class BookmarksCollectionEditor : CollectionEditor
	//{
	//	public BookmarksCollectionEditor( Type type ) : base( type ) { }

	//	protected override Type[] CreateNewItemTypes()
	//	{
	//		return new Type[] { typeof( BasicShapeScrollBarBookmark ), typeof( ImageScrollBarBookmark ), typeof( ValueRangeScrollBarBookmark ) };
	//	}
	//}

	//#endregion

}
#endif