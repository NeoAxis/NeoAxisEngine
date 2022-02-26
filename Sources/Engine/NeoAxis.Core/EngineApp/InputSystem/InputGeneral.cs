// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace NeoAxis
{
	public enum JoystickButtons
	{
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Button20,
		Button21,
		Button22,
		Button23,
		Button24,
		Button25,
		Button26,
		Button27,
		Button28,
		Button29,
		Button30,
		Button31,
		Button32,

		//XBox360 Controller
		XBox360_A,
		XBox360_B,
		XBox360_X,
		XBox360_Y,
		XBox360_LeftShoulder,
		XBox360_RightShoulder,
		XBox360_Back,
		XBox360_Start,
		XBox360_LeftThumbstick,
		XBox360_RightThumbstick,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public enum JoystickAxes
	{
		X,
		Y,
		Z,
		Rx,
		Ry,
		Rz,

		//XBox360 Controller
		XBox360_LeftThumbstickX,
		XBox360_LeftThumbstickY,
		XBox360_RightThumbstickX,
		XBox360_RightThumbstickY,
		XBox360_LeftTrigger,
		XBox360_RightTrigger,

		Special1,
		Special2,
		Special3,
		Special4,
		Special5,
		Special6,
		Special7,
		Special8,
		Special9,
		Special10,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public enum JoystickPOVs
	{
		POV1,
		POV2,
		POV3,
		POV4,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[Flags]
	public enum JoystickPOVDirections
	{
		Centered = 0x00000000,
		North = 0x00000001,
		South = 0x00000010,
		East = 0x00000100,
		West = 0x00001000,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public enum JoystickSliders
	{
		Slider1,
		Slider2,
		Slider3,
		Slider4,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public enum JoystickSliderAxes
	{
		X,
		Y,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class TouchData
	{
		//!!!!more properties

		public Vector2I PositionInPixels;
		public Vector2 Position;
		//!!!!
		public object PointerIdentifier;

		public enum ActionEnum
		{
			/// <summary>
			/// A pressed gesture has started, the motion contains the initial starting location.
			/// </summary>
			Down,
			/// <summary>
			/// A pressed gesture has ended.
			/// </summary>
			Up,
			/// <summary>
			/// A pressed gesture has moved.
			/// </summary>
			Move,
			///// <summary>
			///// The current gesture has been aborted. You will not receive any more points in it.
			///// </summary>
			//Cancel,
			///// <summary>
			///// A movement has happened outside of the normal bounds of the UI element.
			///// </summary>
			//Outside,

			//Other,
		}
		public ActionEnum Action;

		//!!!!
		public class TouchDownRequestToProcessTouch
		{
			public UIControl Sender;
			public double ProcessPriority;
			public double DistanceInPixels;
			public object AnyData;
			public delegate void ActionDelegate( UIControl sender, TouchData touchData, object anyData );
			public ActionDelegate Action;

			public TouchDownRequestToProcessTouch( UIControl sender, double processPriority, double distanceInPixels, object anyData, ActionDelegate action )
			{
				Sender = sender;
				ProcessPriority = processPriority;
				DistanceInPixels = distanceInPixels;
				AnyData = anyData;
				Action = action;
			}
		}
		public List<TouchDownRequestToProcessTouch> TouchDownRequestToControlActions = new List<TouchDownRequestToProcessTouch>();

		///// <summary>
		///// Use reflection to get values from this object without adding reference to Android, iOS dlls.
		///// </summary>
		//public object AndroidView;

		///// <summary>
		///// Use reflection to get values from this object without adding reference to Android, iOS dlls.
		///// </summary>
		//public object AndroidMotionEvent;

		////
		//// Summary:
		////     Android.Views.MotionEvent.GetTouchMinor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetTouchMinor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float TouchMinor { get; }
		////
		//// Summary:
		////     The number of pointers of data contained in this event.
		////
		//// Remarks:
		////     The number of pointers of data contained in this event. Always >= 1.
		////     [Android Documentation]
		//public int PointerCount { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetToolMinor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetToolMinor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float ToolMinor { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetToolMajor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetToolMajor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float ToolMajor { get; }
		////
		//// Summary:
		////     Gets the source of the event.
		////
		//// Remarks:
		////     Gets the source of the event.
		////     [Android Documentation]
		//public InputSourceType Source { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetSize(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetSize(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float Size { get; }
		////
		//// Summary:
		////     Returns the original raw Y coordinate of this event.
		////
		//// Remarks:
		////     Returns the original raw Y coordinate of this event. For touch events on the
		////     screen, this is the original location of the event on the screen, before it had
		////     been adjusted for the containing window and views.
		////     [Android Documentation]
		//public float RawY { get; }
		////
		//// Summary:
		////     Returns the original raw X coordinate of this event.
		////
		//// Remarks:
		////     Returns the original raw X coordinate of this event. For touch events on the
		////     screen, this is the original location of the event on the screen, before it had
		////     been adjusted for the containing window and views.
		////     [Android Documentation]
		//public float RawX { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetPressure(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetPressure(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float Pressure { get; }
		////
		//// Summary:
		////     Return the precision of the X coordinates being reported.
		////
		//// Remarks:
		////     Return the precision of the X coordinates being reported. You can multiply this
		////     number with Android.Views.MotionEvent.GetX to find the actual hardware value
		////     of the X coordinate.
		////     [Android Documentation]
		//public float XPrecision { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetTouchMajor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetTouchMajor(System.Int32) for the first pointer index
		////     (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float TouchMajor { get; }
		////
		//// Summary:
		////     Gets the state of all buttons that are pressed such as a mouse or stylus button.
		////
		//// Remarks:
		////     Gets the state of all buttons that are pressed such as a mouse or stylus button.
		////     [Android Documentation]
		//public MotionEventButtonState ButtonState { get; }
		////public JniPeerMembers JniPeerMembers { get; }
		////
		//// Summary:
		////     Return the kind of action being performed.
		////
		//// Remarks:
		////     Get method documentation [Android Documentation] Return the kind of action being
		////     performed. Consider using Android.Views.MotionEvent.ActionMasked and Android.Views.MotionEvent.ActionIndex
		////     to retrieve the separate masked action and pointer index.
		////     Set method documentation [Android Documentation] Sets this event's action.
		//public MotionEventActions Action { get; set; }
		//public MotionEventButtonState ActionButton { get; }
		////
		//// Summary:
		////     For Android.Views.MotionEvent.ACTION_POINTER_DOWN or Android.Views.MotionEvent.ACTION_POINTER_UP
		////     as returned by Android.Views.MotionEvent.ActionMasked, this returns the associated
		////     pointer index.
		////
		//// Remarks:
		////     For Android.Views.MotionEvent.ACTION_POINTER_DOWN or Android.Views.MotionEvent.ACTION_POINTER_UP
		////     as returned by Android.Views.MotionEvent.ActionMasked, this returns the associated
		////     pointer index. The index may be used with Android.Views.MotionEvent.GetPointerId(System.Int32),
		////     Android.Views.MotionEvent.GetX(System.Int32), Android.Views.MotionEvent.GetY(System.Int32),
		////     Android.Views.MotionEvent.GetPressure(System.Int32), and Android.Views.MotionEvent.GetSize(System.Int32)
		////     to get information about the pointer that has gone down or up.
		////     [Android Documentation]
		//public int ActionIndex { get; }
		////
		//// Summary:
		////     Return the masked action being performed, without pointer index information.
		////
		//// Remarks:
		////     Return the masked action being performed, without pointer index information.
		////     Use Android.Views.MotionEvent.ActionIndex to return the index associated with
		////     pointer actions.
		////     [Android Documentation]
		//public MotionEventActions ActionMasked { get; }
		////
		//// Summary:
		////     Return the precision of the Y coordinates being reported.
		////
		//// Remarks:
		////     Return the precision of the Y coordinates being reported. You can multiply this
		////     number with Android.Views.MotionEvent.GetY to find the actual hardware value
		////     of the Y coordinate.
		////     [Android Documentation]
		//public float YPrecision { get; }
		////
		//// Summary:
		////     Gets the id for the device that this event came from.
		////
		//// Remarks:
		////     Gets the id for the device that this event came from. An id of zero indicates
		////     that the event didn't come from a physical device and maps to the default keymap.
		////     The other numbers are arbitrary and you shouldn't depend on the values.
		////     [Android Documentation]
		//public int DeviceId { get; }
		////
		//// Summary:
		////     Returns the time (in ms) when the user originally pressed down to start a stream
		////     of position events.
		////
		//// Remarks:
		////     Returns the time (in ms) when the user originally pressed down to start a stream
		////     of position events.
		////     [Android Documentation]
		//public long DownTime { get; }
		////
		//// Summary:
		////     Returns a bitfield indicating which edges, if any, were touched by this MotionEvent.
		////
		//// Remarks:
		////     Get method documentation [Android Documentation] Returns a bitfield indicating
		////     which edges, if any, were touched by this MotionEvent. For touch events, clients
		////     can use this to determine if the user's finger was touching the edge of the display.
		////     This property is only set for Android.Support.V4.View.MotionEventCompat.ActionPointerDownReplaceLinkValue
		////     events.
		////     Set method documentation [Android Documentation] Sets the bitfield indicating
		////     which edges, if any, were touched by this MotionEvent.
		//public Edge EdgeFlags { get; set; }
		////
		//// Summary:
		////     Retrieve the time this event occurred, in the Android.OS.SystemClock.UptimeMillis
		////     time base.
		////
		//// Remarks:
		////     Retrieve the time this event occurred, in the Android.OS.SystemClock.UptimeMillis
		////     time base.
		////     [Android Documentation]
		//public long EventTime { get; }
		////
		//// Summary:
		////     Gets the motion event flags.
		////
		//// Remarks:
		////     Gets the motion event flags.
		////     [Android Documentation]
		//public MotionEventFlags Flags { get; }
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetOrientation(System.Int32) for the first pointer
		////     index (may be an arbitrary pointer identifier).
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetOrientation(System.Int32) for the first pointer
		////     index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//public float Orientation { get; }
		////
		//// Summary:
		////     Returns the number of historical points in this event.
		////
		//// Remarks:
		////     Returns the number of historical points in this event. These are movements that
		////     have occurred between this event and the previous event. This only applies to
		////     ACTION_MOVE events -- all other actions will have a size of 0.
		////     [Android Documentation]
		//public int HistorySize { get; }
		////
		//// Summary:
		////     Returns the state of any meta / modifier keys that were in effect when the event
		////     was generated.
		////
		//// Remarks:
		////     Returns the state of any meta / modifier keys that were in effect when the event
		////     was generated. This is the same values as those returned by Android.Views.KeyEvent.MetaState.
		////     [Android Documentation]
		//public MetaKeyStates MetaState { get; }
		//////
		////// Summary:
		//////     This API supports the Mono for Android infrastructure and is not intended to
		//////     be used directly from your code.
		//////
		////// Remarks:
		//////     This property is used to control which jclass is provided to methods like Android.Runtime.JNIEnv.CallNonvirtualVoidMethodReplaceLinkValue.
		////protected IntPtr ThresholdClass { get; }
		//////
		////// Summary:
		//////     This API supports the Mono for Android infrastructure and is not intended to
		//////     be used directly from your code.
		//////
		////// Remarks:
		//////     This property is used to control virtual vs. non virtual method dispatch against
		//////     the underlying JNI object. When this property is equal to the declaring type,
		//////     then virtual method invocation against the JNI object is performed; otherwise,
		//////     we assume that the method was overridden by a derived type, and perform non-virtual
		//////     methdo invocation.
		////protected Type ThresholdType { get; }

		////
		//// Summary:
		////     Given a pointer identifier, find the index of its data in the event.
		////
		//// Parameters:
		////   pointerId:
		////     The identifier of the pointer to be found.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Given a pointer identifier, find the index of its data in the event.
		////     [Android Documentation]
		//[Register( "findPointerIndex", "(I)I", "", ApiSince = 5 )]
		//public int FindPointerIndex( int pointerId );
		////
		//// Summary:
		////     Returns the value of the requested axis for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   axis:
		////     The axis identifier for the axis value to retrieve.
		////
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the value of the requested axis for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////     [Android Documentation]
		//[Register( "getAxisValue", "(II)F", "", ApiSince = 12 )]
		//public float GetAxisValue( [GeneratedEnum] Axis axis, int pointerIndex );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetAxisValue(Android.Views.Axis) for the first pointer
		////     index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   axis:
		////     The axis identifier for the axis value to retrieve.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetAxisValue(Android.Views.Axis) for the first pointer
		////     index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getAxisValue", "(I)F", "", ApiSince = 12 )]
		//public float GetAxisValue( [GeneratedEnum] Axis axis );
		////
		//// Summary:
		////     Returns the historical value of the requested axis, as per Android.Views.MotionEvent.GetAxisValue(Android.Views.Axis,
		////     System.Int32), occurred between this event and the previous event for the given
		////     pointer.
		////
		//// Parameters:
		////   axis:
		////     The axis identifier for the axis value to retrieve.
		////
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the historical value of the requested axis, as per Android.Views.MotionEvent.GetAxisValue(Android.Views.Axis,
		////     System.Int32), occurred between this event and the previous event for the given
		////     pointer. Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalAxisValue", "(III)F", "", ApiSince = 12 )]
		//public float GetHistoricalAxisValue( [GeneratedEnum] Axis axis, int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalAxisValue(Android.Views.Axis, System.Int32,
		////     System.Int32) for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   axis:
		////     The axis identifier for the axis value to retrieve.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalAxisValue(Android.Views.Axis, System.Int32,
		////     System.Int32) for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalAxisValue", "(II)F", "", ApiSince = 12 )]
		//public float GetHistoricalAxisValue( [GeneratedEnum] Axis axis, int pos );
		////
		//// Summary:
		////     Returns the time that a historical movement occurred between this event and the
		////     previous event, in the Android.OS.SystemClock.UptimeMillis time base.
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the time that a historical movement occurred between this event and the
		////     previous event, in the Android.OS.SystemClock.UptimeMillis time base.
		////     This only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalEventTime", "(I)J", "" )]
		//public long GetHistoricalEventTime( int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalOrientation(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalOrientation(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalOrientation", "(I)F", "", ApiSince = 9 )]
		//public float GetHistoricalOrientation( int pos );
		////
		//// Summary:
		////     Returns a historical orientation coordinate, as per Android.Views.MotionEvent.GetOrientation(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical orientation coordinate, as per Android.Views.MotionEvent.GetOrientation(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalOrientation", "(II)F", "", ApiSince = 9 )]
		//public float GetHistoricalOrientation( int pointerIndex, int pos );
		//[Register( "getHistoricalPointerCoords", "(IILandroid/view/MotionEvent$PointerCoords;)V", "", ApiSince = 9 )]
		//public void GetHistoricalPointerCoords( int pointerIndex, int pos, PointerCoords outPointerCoords );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalPressure(System.Int32, System.Int32) for
		////     the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalPressure(System.Int32, System.Int32) for
		////     the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalPressure", "(I)F", "" )]
		//public float GetHistoricalPressure( int pos );
		////
		//// Summary:
		////     Returns a historical pressure coordinate, as per Android.Views.MotionEvent.GetPressure(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical pressure coordinate, as per Android.Views.MotionEvent.GetPressure(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalPressure", "(II)F", "", ApiSince = 5 )]
		//public float GetHistoricalPressure( int pointerIndex, int pos );
		////
		//// Summary:
		////     Returns a historical size coordinate, as per Android.Views.MotionEvent.GetSize(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical size coordinate, as per Android.Views.MotionEvent.GetSize(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalSize", "(II)F", "", ApiSince = 5 )]
		//public float GetHistoricalSize( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalSize(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalSize(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalSize", "(I)F", "" )]
		//public float GetHistoricalSize( int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalToolMajor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalToolMajor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalToolMajor", "(I)F", "", ApiSince = 9 )]
		//public float GetHistoricalToolMajor( int pos );
		////
		//// Summary:
		////     Returns a historical tool major axis coordinate, as per Android.Views.MotionEvent.GetToolMajor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical tool major axis coordinate, as per Android.Views.MotionEvent.GetToolMajor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalToolMajor", "(II)F", "", ApiSince = 9 )]
		//public float GetHistoricalToolMajor( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalToolMinor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalToolMinor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalToolMinor", "(I)F", "", ApiSince = 9 )]
		//public float GetHistoricalToolMinor( int pos );
		////
		//// Summary:
		////     Returns a historical tool minor axis coordinate, as per Android.Views.MotionEvent.GetToolMinor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical tool minor axis coordinate, as per Android.Views.MotionEvent.GetToolMinor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalToolMinor", "(II)F", "", ApiSince = 9 )]
		//public float GetHistoricalToolMinor( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalTouchMajor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalTouchMajor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalTouchMajor", "(I)F", "", ApiSince = 9 )]
		//public float GetHistoricalTouchMajor( int pos );
		////
		//// Summary:
		////     Returns a historical touch major axis coordinate, as per Android.Views.MotionEvent.GetTouchMajor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical touch major axis coordinate, as per Android.Views.MotionEvent.GetTouchMajor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalTouchMajor", "(II)F", "", ApiSince = 9 )]
		//public float GetHistoricalTouchMajor( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalTouchMinor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalTouchMinor(System.Int32, System.Int32)
		////     for the first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalTouchMinor", "(I)F", "", ApiSince = 9 )]
		//public float GetHistoricalTouchMinor( int pos );
		////
		//// Summary:
		////     Returns a historical touch minor axis coordinate, as per Android.Views.MotionEvent.GetTouchMinor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical touch minor axis coordinate, as per Android.Views.MotionEvent.GetTouchMinor(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalTouchMinor", "(II)F", "", ApiSince = 9 )]
		//public float GetHistoricalTouchMinor( int pointerIndex, int pos );
		////
		//// Summary:
		////     Returns a historical X coordinate, as per Android.Views.MotionEvent.GetX(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical X coordinate, as per Android.Views.MotionEvent.GetX(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalX", "(II)F", "", ApiSince = 5 )]
		//public float GetHistoricalX( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalX(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalX(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalX", "(I)F", "" )]
		//public float GetHistoricalX( int pos );
		////
		//// Summary:
		////     Returns a historical Y coordinate, as per Android.Views.MotionEvent.GetY(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a historical Y coordinate, as per Android.Views.MotionEvent.GetY(System.Int32),
		////     that occurred between this event and the previous event for the given pointer.
		////     Only applies to ACTION_MOVE events.
		////     [Android Documentation]
		//[Register( "getHistoricalY", "(II)F", "", ApiSince = 5 )]
		//public float GetHistoricalY( int pointerIndex, int pos );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetHistoricalY(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////
		//// Parameters:
		////   pos:
		////     Which historical value to return; must be less than Android.Views.MotionEvent.HistorySize
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetHistoricalY(System.Int32, System.Int32) for the
		////     first pointer index (may be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getHistoricalY", "(I)F", "" )]
		//public float GetHistoricalY( int pos );
		////
		//// Summary:
		////     Returns the orientation of the touch area and tool area in radians clockwise
		////     from vertical for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the orientation of the touch area and tool area in radians clockwise
		////     from vertical for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). An angle of 0 radians indicates
		////     that the major axis of contact is oriented upwards, is perfectly circular or
		////     is of unknown orientation. A positive angle indicates that the major axis of
		////     contact is oriented to the right. A negative angle indicates that the major axis
		////     of contact is oriented to the left. The full range is from -PI/2 radians (finger
		////     pointing fully left) to PI/2 radians (finger pointing fully right).
		////     [Android Documentation]
		//[Register( "getOrientation", "(I)F", "", ApiSince = 9 )]
		//public float GetOrientation( int pointerIndex );
		////
		//[Register( "getPointerCoords", "(ILandroid/view/MotionEvent$PointerCoords;)V", "", ApiSince = 9 )]
		//public void GetPointerCoords( int pointerIndex, PointerCoords outPointerCoords );
		////
		//// Summary:
		////     Return the pointer identifier associated with a particular pointer data index
		////     is this event.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Return the pointer identifier associated with a particular pointer data index
		////     is this event. The identifier tells you the actual pointer number associated
		////     with the data, accounting for individual pointers going up and down since the
		////     start of the current gesture.
		////     [Android Documentation]
		//[Register( "getPointerId", "(I)I", "", ApiSince = 5 )]
		//public int GetPointerId( int pointerIndex );
		////
		//[Register( "getPointerProperties", "(ILandroid/view/MotionEvent$PointerProperties;)V", "", ApiSince = 14 )]
		//public void GetPointerProperties( int pointerIndex, PointerProperties outPointerProperties );
		////
		//// Summary:
		////     Returns the current pressure of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the current pressure of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). The pressure generally ranges
		////     from 0 (no pressure at all) to 1 (normal pressure), however values higher than
		////     1 may be generated depending on the calibration of the input device.
		////     [Android Documentation]
		//[Register( "getPressure", "(I)F", "", ApiSince = 5 )]
		//public float GetPressure( int pointerIndex );
		////
		//// Summary:
		////     Returns a scaled value of the approximate size for the given pointer index (use
		////     Android.Views.MotionEvent.GetPointerId(System.Int32) to find the pointer identifier
		////     for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns a scaled value of the approximate size for the given pointer index (use
		////     Android.Views.MotionEvent.GetPointerId(System.Int32) to find the pointer identifier
		////     for this index). This represents some approximation of the area of the screen
		////     being pressed; the actual value in pixels corresponding to the touch is normalized
		////     with the device specific range of values and scaled to a value between 0 and
		////     1. The value of size can be used to determine fat touch events.
		////     [Android Documentation]
		//[Register( "getSize", "(I)F", "", ApiSince = 5 )]
		//public float GetSize( int pointerIndex );
		////
		//// Summary:
		////     Returns the length of the major axis of an ellipse that describes the size of
		////     the approaching tool for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the length of the major axis of an ellipse that describes the size of
		////     the approaching tool for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). The tool area represents the
		////     estimated size of the finger or pen that is touching the device independent of
		////     its actual touch area at the point of contact.
		////     [Android Documentation]
		//[Register( "getToolMajor", "(I)F", "", ApiSince = 9 )]
		//public float GetToolMajor( int pointerIndex );
		////
		//// Summary:
		////     Returns the length of the minor axis of an ellipse that describes the size of
		////     the approaching tool for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the length of the minor axis of an ellipse that describes the size of
		////     the approaching tool for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). The tool area represents the
		////     estimated size of the finger or pen that is touching the device independent of
		////     its actual touch area at the point of contact.
		////     [Android Documentation]
		//[Register( "getToolMinor", "(I)F", "", ApiSince = 9 )]
		//public float GetToolMinor( int pointerIndex );
		////
		//// Summary:
		////     Gets the tool type of a pointer for the given pointer index.
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Gets the tool type of a pointer for the given pointer index. The tool type indicates
		////     the type of tool used to make contact such as a finger or stylus, if known.
		////     [Android Documentation]
		//[Register( "getToolType", "(I)I", "", ApiSince = 14 )]
		//[return: GeneratedEnum]
		//public MotionEventToolType GetToolType( int pointerIndex );
		////
		//// Summary:
		////     Returns the length of the major axis of an ellipse that describes the touch area
		////     at the point of contact for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the length of the major axis of an ellipse that describes the touch area
		////     at the point of contact for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////     [Android Documentation]
		//[Register( "getTouchMajor", "(I)F", "", ApiSince = 9 )]
		//public float GetTouchMajor( int pointerIndex );
		////
		//// Summary:
		////     Returns the length of the minor axis of an ellipse that describes the touch area
		////     at the point of contact for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the length of the minor axis of an ellipse that describes the touch area
		////     at the point of contact for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////     [Android Documentation]
		//[Register( "getTouchMinor", "(I)F", "", ApiSince = 9 )]
		//public float GetTouchMinor( int pointerIndex );
		////
		//// Summary:
		////     Returns the X coordinate of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the X coordinate of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). Whole numbers are pixels; the
		////     value may have a fraction for input devices that are sub-pixel precise.
		////     [Android Documentation]
		//[Register( "getX", "(I)F", "", ApiSince = 5 )]
		//public float GetX( int pointerIndex );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetX(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetX(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getX", "()F", "" )]
		//public float GetX();
		////
		//// Summary:
		////     Returns the Y coordinate of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index).
		////
		//// Parameters:
		////   pointerIndex:
		////     Raw index of pointer to retrieve. Value may be from 0 (the first pointer that
		////     is down) to Android.Views.MotionEvent.PointerCount-1.
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Returns the Y coordinate of this event for the given pointer index (use Android.Views.MotionEvent.GetPointerId(System.Int32)
		////     to find the pointer identifier for this index). Whole numbers are pixels; the
		////     value may have a fraction for input devices that are sub-pixel precise.
		////     [Android Documentation]
		//[Register( "getY", "(I)F", "", ApiSince = 5 )]
		//public float GetY( int pointerIndex );
		////
		//// Summary:
		////     Android.Views.MotionEvent.GetY(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Android.Views.MotionEvent.GetY(System.Int32) for the first pointer index (may
		////     be an arbitrary pointer identifier).
		////     [Android Documentation]
		//[Register( "getY", "()F", "" )]
		//public float GetY();
		////
		//// Summary:
		////     Checks if a mouse or stylus button (or combination of buttons) is pressed.
		////
		//// Parameters:
		////   button:
		////     Button (or combination of buttons).
		////
		//// Returns:
		////     To be added.
		////
		//// Remarks:
		////     Checks if a mouse or stylus button (or combination of buttons) is pressed.
		////     [Android Documentation]
		//[Register( "isButtonPressed", "(I)Z", "", ApiSince = 21 )]
		//public bool IsButtonPressed( [GeneratedEnum] MotionEventButtonState button );

		////
		//// Summary:
		////     Adjust this event's location.
		////
		//// Parameters:
		////   deltaX:
		////     Amount to add to the current X coordinate of the event.
		////
		////   deltaY:
		////     Amount to add to the current Y coordinate of the event.
		////
		//// Remarks:
		////     Adjust this event's location.
		////     [Android Documentation]
		//[Register( "offsetLocation", "(FF)V", "" )]
		//public void OffsetLocation( float deltaX, float deltaY );
		////
		//// Summary:
		////     Recycle the MotionEvent, to be re-used by a later caller.
		////
		//// Remarks:
		////     Recycle the MotionEvent, to be re-used by a later caller. After calling this
		////     function you must not ever touch the event again.
		////     [Android Documentation]
		//[Register( "recycle", "()V", "" )]
		//public void Recycle();
		////
		//// Summary:
		////     Set this event's location.
		////
		//// Parameters:
		////   x:
		////     New absolute X location.
		////
		////   y:
		////     New absolute Y location.
		////
		//// Remarks:
		////     Set this event's location. Applies Android.Views.MotionEvent.OffsetLocation(System.Single,
		////     System.Single) with a delta from the current location to the given new location.
		////     [Android Documentation]
		//[Register( "setLocation", "(FF)V", "" )]
		//public void SetLocation( float x, float y );
		////
		//// Summary:
		////     Modifies the source of the event.
		////
		//// Parameters:
		////   source:
		////     The new source.
		////
		//// Remarks:
		////     Modifies the source of the event.
		////     [Android Documentation]
		//[Register( "setSource", "(I)V", "", ApiSince = 12 )]
		//public void SetSource( [GeneratedEnum] InputSourceType source );
		////
		//// Summary:
		////     Applies a transformation matrix to all of the points in the event.
		////
		//// Parameters:
		////   matrix:
		////     The transformation matrix to apply.
		////
		//// Remarks:
		////     Applies a transformation matrix to all of the points in the event.
		////     [Android Documentation]
		//[Register( "transform", "(Landroid/graphics/Matrix;)V", "", ApiSince = 11 )]
		//public void Transform( Matrix matrix );
		////
		//// Summary:
		////     Flatten this object in to a Parcel.
		////
		//// Parameters:
		////   out:
		////     The Parcel in which the object should be written.
		////
		////   flags:
		////     Additional flags about how the object should be written. May be 0 or Android.OS.Parcelable.ParcelableWriteReturnValue.
		////
		//// Remarks:
		////     Flatten this object in to a Parcel.
		////     [Android Documentation]
		//[Register( "writeToParcel", "(Landroid/os/Parcel;I)V", "" )]
		//public override void WriteToParcel( Parcel @out, [GeneratedEnum] ParcelableWriteFlags flags );

	}

}
