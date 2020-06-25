//// Copyright (C) 2006-2011 NeoAxis Group Ltd.
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Drawing;
//using System.Runtime.InteropServices;
//using Engine.FileSystem;
//using Engine.SoundSystem;
//using Engine.MathEx;
//using Engine.Utils;

//namespace Engine
//{
//   class AndroidPlatformFunctionality : PlatformFunctionality
//   {
//      static AndroidPlatformFunctionality instance;

//      //const int suspendModeTimerID = 31692;

//      unsafe static AndroidAppNativeWrapper.CallbackLogInfo logInfoDelegate = LogInfo;
//      unsafe static AndroidAppNativeWrapper.CallbackLogWarning logWarningDelegate = LogWarning;
//      unsafe static AndroidAppNativeWrapper.CallbackLogFatal logFatalDelegate = LogFatal;

//      IntPtr jniEnvironment;
//      IntPtr javaVM;

//      Vec2 mousePosition;

//      ///////////////////////////////////////////

//      struct Wrapper
//      {
//         public const string library = "AndroidAppNativeWrapper";
//         public const CallingConvention convention = CallingConvention.Cdecl;
//      }

//      ///////////////////////////////////////////

//      struct AndroidAppNativeWrapper
//      {
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_MessageBox", CallingConvention = Wrapper.convention,
//            CharSet = CharSet.Unicode )]
//         public unsafe static extern void MessageBox( string text, string caption );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsWindowVisible", CallingConvention = Wrapper.convention )]
//         [return: MarshalAs( UnmanagedType.U1 )]
//         public unsafe static extern bool IsWindowVisible();
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsWindowActive", CallingConvention = Wrapper.convention )]
//         [return: MarshalAs( UnmanagedType.U1 )]
//         public unsafe static extern bool IsWindowActive();
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsWindowFocused", CallingConvention = Wrapper.convention )]
//         [return: MarshalAs( UnmanagedType.U1 )]
//         public unsafe static extern bool IsWindowFocused();

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetWindowRectangle", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void GetWindowRectangle( out RectI rect );
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetWindowClientRect", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void GetWindowClientRect( out RectI rect );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetClientRectangleCursorPosition", CallingConvention = Wrapper.convention )]
//         //public unsafe static extern void GetClientRectangleCursorPosition( out int x, out int y );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetSystemTime", CallingConvention = Wrapper.convention )]
//         public unsafe static extern double GetSystemTime();
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetScreenSize", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void GetScreenSize( out int width, out int height );
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetScreenBitsPerPixel", CallingConvention = Wrapper.convention )]
//         public unsafe static extern int GetScreenBitsPerPixel();
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_ShutdownApplicationWindow", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void ShutdownApplicationWindow();

//         [UnmanagedFunctionPointer( Wrapper.convention )]
//         public unsafe delegate void CallbackLogInfo( char* text );
//         [UnmanagedFunctionPointer( Wrapper.convention )]
//         public unsafe delegate void CallbackLogWarning( char* text );
//         [UnmanagedFunctionPointer( Wrapper.convention )]
//         public unsafe delegate void CallbackLogFatal( char* text );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_SetLogCallbacks", CallingConvention = Wrapper.convention )]
//         [return: MarshalAs( UnmanagedType.U1 )]
//         public unsafe static extern bool SetLogCallbacks( CallbackLogInfo logInfo, CallbackLogWarning logWarning, CallbackLogFatal logFatal );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetDisplayCount", CallingConvention = Wrapper.convention )]
//         public unsafe static extern int GetDisplayCount();

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsKeyPressed", CallingConvention = Wrapper.convention )]
//         //[return: MarshalAs( UnmanagedType.U1 )]
//         //public unsafe static extern bool IsKeyPressed( EKeys eKey );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsSystemKey", CallingConvention = Wrapper.convention )]
//         //[return: MarshalAs( UnmanagedType.U1 )]
//         //public unsafe static extern bool IsSystemKey( EKeys eKey );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_IsMouseButtonPressed", CallingConvention = Wrapper.convention )]
//         //[return: MarshalAs( UnmanagedType.U1 )]
//         //public unsafe static extern bool IsMouseButtonPressed( int buttonCode );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_AllocMemory", CallingConvention = Wrapper.convention )]
//         public unsafe static extern IntPtr AllocMemory( int size );
//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_FreeMemory", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void FreeMemory( IntPtr buffer );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_CallCustomPlatformSpecificMethod", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         public unsafe static extern IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_SetWindowTitle", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         //public unsafe static extern void SetWindowTitle( string title );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         //public unsafe static extern void GetMouseMoveDelta( out int x, out int y );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_ResetMouseMoveDelta", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         //public unsafe static extern void ResetMouseMoveDelta();

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetLoadedLibraryNames", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         public unsafe static extern void GetLoadedLibraryNames( out IntPtr* list, out int count );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_Init", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         public unsafe static extern void Init( IntPtr javaEnv, IntPtr javaObj );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetActiveDisplayList", CallingConvention = Wrapper.convention )]
//         //public unsafe static extern int GetActiveDisplayList( int bufferLength, uint* buffer );

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetDisplayInfo", CallingConvention = Wrapper.convention )]
//         //public unsafe static extern void GetDisplayInfo( uint display, out IntPtr deviceName,
//         //    out Recti bounds, out Recti workingArea, [MarshalAs( UnmanagedType.U1 )] out bool primary );

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_FreeOutString", CallingConvention = Wrapper.convention )]
//         public unsafe static extern void FreeOutString( IntPtr pointer );

//         public static string GetOutString( IntPtr pointer )
//         {
//            if( pointer != IntPtr.Zero )
//            {
//               string result = Marshal.PtrToStringUni( pointer );
//               FreeOutString( pointer );
//               return result;
//            }
//            else
//               return null;
//         }

//         [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_GetGLContext", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         public unsafe static extern IntPtr GetGLContext();

//         //[DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_CallUserCustomMethod", CallingConvention = Wrapper.convention, CharSet = CharSet.Unicode )]
//         //public unsafe static extern int CallUserCustomMethod( string methodName,
//         //     void*/*AndroidValue*/ arguments, int argumentCount,
//         //     void*/*AndroidValue*/ returnValue );
//      }

//      ///////////////////////////////////////////

//      [StructLayout( LayoutKind.Sequential )]
//      struct AndroidMainModuleInitData
//      {
//         internal EngineApp.MainModuleInitData baseStructure;
//         internal IntPtr javaAppEnv;
//         internal IntPtr javaAppObj;
//         internal IntPtr javaVM;
//      }

//      ///////////////////////////////////////////

//      enum AndroidWindowMessages
//      {
//         Paint,
//         Timer,
//         MouseDown,
//         MouseUp,
//         MouseMove,
//         KeyDown,
//         KeyUp
//      }

//      ///////////////////////////////////////////

//      [StructLayout( LayoutKind.Sequential )]
//      struct AndroidWindowMessageData
//      {
//         public AndroidWindowMessages message;
//         public int parameter1;
//         public int parameter2;
//         public int result;
//      }

//      ///////////////////////////////////////////

//      public static EngineApp App
//      {
//         get { return EngineApp.Instance; }
//      }

//      unsafe public AndroidPlatformFunctionality()
//      {
//         instance = this;
//         AndroidAppNativeWrapper.SetLogCallbacks( logInfoDelegate, logWarningDelegate, logFatalDelegate );
//      }

//      public unsafe override void Init( IntPtr mainModuleData )
//      {
//         base.Init( mainModuleData );

//         if( mainModuleData == IntPtr.Zero )
//            Log.Fatal( "AndroidPlatformFunctionality: Init: mainModuleData == IntPtr.Zero." );

//         AndroidMainModuleInitData* initData = (AndroidMainModuleInitData*)mainModuleData;
//         jniEnvironment = initData->javaAppEnv;
//         javaVM = initData->javaVM;
//         AndroidAppNativeWrapper.Init( jniEnvironment, initData->javaAppObj );
//      }

//      public override IntPtr GetGLContext()
//      {
//         return AndroidAppNativeWrapper.GetGLContext();
//      }

//      public override bool ChangeVideoMode( Vec2I mode, bool allowChangeDisplayFrequency )
//      {
//         по идее не надо никогда вызывать

//         return true;
//      }

//      public override System.IntPtr InitApplicationWindow()
//      {
//         return IntPtr.Zero;
//      }

//      public override void ShutdownApplicationWindow()
//      {
//         AndroidAppNativeWrapper.ShutdownApplicationWindow();
//      }

//      public override void DoMessageEvents()
//      {
//      }

//      public override List<Vec2I> GenerateVideoModes()
//      {
//         List<Vec2I> videoModes = new List<Vec2I>();
//         videoModes.Add( GetScreenSize() );
//         return videoModes;
//      }

//      public override RectI GetClientRectangle()
//      {
//         RectI clientRect;
//         AndroidAppNativeWrapper.GetWindowClientRect( out clientRect );
//         return clientRect;
//      }

//      public override Vec2 GetMousePosition()
//      {
//         return mousePosition;
//      }

//      public override void SetMousePosition( Vec2 value )
//      {
//      }

//      public unsafe override string[] GetNativeModuleNames()
//      {
//         IntPtr* list;
//         int count;
//         AndroidAppNativeWrapper.GetLoadedLibraryNames( out list, out count );

//         string[] result = new string[ count ];

//         for( int n = 0; n < count; n++ )
//         {
//            IntPtr namePointer = list[ n ];

//            ansi
//            string name = Marshal.PtrToStringAnsi( namePointer );
//            result[ n ] = name;

//            AndroidAppNativeWrapper.FreeMemory( namePointer );
//         }

//         AndroidAppNativeWrapper.FreeMemory( (IntPtr)list );

//         return result;
//      }

//      public override Vec2I GetScreenSize()
//      {
//         int width, height;
//         AndroidAppNativeWrapper.GetScreenSize( out width, out height );
//         return new Vec2I( width, height );
//      }

//      public override Vec2I GetSmallIconSize()
//      {
//         return new Vec2I( 0, 0 );
//      }

//      public override double GetSystemTime()
//      {
//         return AndroidAppNativeWrapper.GetSystemTime();
//      }

//      public override RectI GetWindowRectangle()
//      {
//         RectI windowRect;
//         AndroidAppNativeWrapper.GetWindowRectangle( out windowRect );
//         return windowRect;
//      }

//      public override WindowStates GetWindowState()
//      {
//         
//         return WindowStates.Normal;
//      }

//      public override bool IsWindowActive()
//      {
//         return AndroidAppNativeWrapper.IsWindowActive();
//      }

//      public override bool IsWindowVisible()
//      {
//         return AndroidAppNativeWrapper.IsWindowVisible();
//      }

//      public override void MessageLoopWaitMessage()
//      {
//         DoMessageEvents();
//      }

//      public override void RestoreVideoMode()
//      {
//      }

//      public override void RunMessageLoop()
//      {
//         Log.Fatal( "AndroidPlatformFunctionality: RunMessageLoop: Must never call." );
//      }

//      bool IsApplicationIdlePossibly()
//      {
//         bool needIdle = GetWindowState() != WindowStates.Minimized;
//         return needIdle;
//      }

//      public override bool IsIntoMenuLoop()
//      {
//         return false;
//      }

//      unsafe static void LogInfo( char* text )
//      {
//         ansi
//         string result = Marshal.PtrToStringAnsi( (IntPtr)text );
//         Log.Info( result );
//      }

//      unsafe static void LogWarning( char* text )
//      {
//         ansi
//         string result = Marshal.PtrToStringAnsi( (IntPtr)text );
//         Log.Warning( result );
//      }

//      unsafe static void LogFatal( char* text )
//      {
//         ansi
//         string result = Marshal.PtrToStringAnsi( (IntPtr)text );
//         Log.Fatal( result );
//      }

//      public override void SetGamma( float value )
//      {
//      }

//      public override void SetWindowSize( Vec2I size )
//      {
//         ?
//      }

//      public override void SetWindowState( WindowStates value )
//      {
//         ?
//      }

//      public override void UpdateInputDevices()
//      {
//         
//         if( AndroidAppNativeWrapper.IsWindowFocused() )
//            UpdateShowSystemCursor();

//         нужно отжимать кнопки при деактивации окна и других подобных событиях. делать это в java

//         ////mouse
//         //{
//         //   if( App.IsMouseButtonPressed( EMouseButtons.Left ) &&
//         //      !AndroidAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Left ) )
//         //   {
//         //      App.DoMouseUp( EMouseButtons.Left );
//         //   }
//         //   if( App.IsMouseButtonPressed( EMouseButtons.Right ) &&
//         //      !AndroidAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Right ) )
//         //   {
//         //      App.DoMouseUp( EMouseButtons.Right );
//         //   }
//         //   if( App.IsMouseButtonPressed( EMouseButtons.Middle ) &&
//         //      !AndroidAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.Middle ) )
//         //   {
//         //      App.DoMouseUp( EMouseButtons.Middle );
//         //   }
//         //   if( App.IsMouseButtonPressed( EMouseButtons.XButton1 ) &&
//         //      !AndroidAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton1 ) )
//         //   {
//         //      App.DoMouseUp( EMouseButtons.XButton1 );
//         //   }
//         //   if( App.IsMouseButtonPressed( EMouseButtons.XButton2 ) &&
//         //      !AndroidAppNativeWrapper.IsMouseButtonPressed( (int)EMouseButtons.XButton2 ) )
//         //   {
//         //      App.DoMouseUp( EMouseButtons.XButton2 );
//         //   }
//         //}

//         ////keys
//         //foreach( EKeys eKey in App.AllKeys )
//         //{
//         //   if( eKey != EKeys.Shift && eKey != EKeys.Control && eKey != EKeys.Alt && eKey != EKeys.Command )
//         //   {
//         //      if( App.IsKeyPressed( eKey ) )
//         //      {
//         //         if( !AndroidAppNativeWrapper.IsKeyPressed( eKey ) )
//         //         {
//         //            KeyEvent keyEvent = new KeyEvent( eKey );
//         //            App.DoKeyUp( keyEvent );
//         //         }
//         //      }
//         //   }
//         //}

//         
//x;
//         //mouse outside window client rectangle

//         //mouse relative mode
//         if( App.MouseRelativeMode )
//         {
//            x;
//            ////clip cursor by window rectangle
//            //if( IsFocused() )
//            //{
//            //   SetCapture( App.WindowHandle );

//            //   Recti rectangle = GetWindowRectangle();
//            //   rectangle.Left += 1;
//            //   rectangle.Top += 1;
//            //   rectangle.Right -= 1;
//            //   rectangle.Bottom -= 1;
//            //   ClipCursor( (IntPtr)( &rectangle ) );
//            //}

//            App.MousePosition = new Vec2( .5f, .5f );
//         }
//      }

//      public override void UpdateShowSystemCursor()
//      {
//         x;
//      }

//      public override void UpdateWindowIcon( Icon smallIcon, Icon icon )
//      {
//      }

//      public override bool IsWindowInitialized()
//      {
//         return true;
//      }

//      public override void UpdateWindowTitle( string title )
//      {
//         //AndroidAppNativeWrapper.SetWindowTitle( title );
//      }

//      public override bool IsFocused()
//      {
//         return AndroidAppNativeWrapper.IsWindowFocused();
//      }

//      public override int GetScreenBitsPerPixel()
//      {
//         return AndroidAppNativeWrapper.GetScreenBitsPerPixel();
//      }

//      public override bool IsKeyLocked( EKeys key )
//      {
//         x;
//         return false;
//      }

//      public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
//      {
//         Log.Fatal( "AndroidPlatformFunctionality: ShowMessageBoxYesNoQuestion: method is not implemented." );
//         return false;
//      }

//      public override void OnChangeMouseRelativeMode()
//      {
//         x;

//         //if( !App.MouseRelativeMode )
//         //{
//         //ReleaseCapture();
//         //ClipCursor( IntPtr.Zero );
//         //}

//         //AndroidAppNativeWrapper.ResetMouseMoveDelta();
//      }

//      public override void UpdateMouseRelativeMove( out Vec2 delta )
//      {
//         delta = Vec2.Zero;
//         //int x, y;
//         //AndroidAppNativeWrapper.GetMouseMoveDelta( out x, out y );
//         //delta = new Vec2( x, y ) / App.VideoMode.ToVec2();
//         //AndroidAppNativeWrapper.ResetMouseMoveDelta();
//      }

//      public override IntPtr CallCustomPlatformSpecificMethod( string message, IntPtr param )
//      {
//         if( message == "GetJNIEnvironment" )
//            return jniEnvironment;
//         if( message == "GetJavaVM" )
//            return javaVM;

//         return AndroidAppNativeWrapper.CallCustomPlatformSpecificMethod( message, param );
//      }

//      public unsafe override void MainModule_WindowMessage( IntPtr data2 )
//      {
//         AndroidWindowMessageData* data = (AndroidWindowMessageData*)data2;

//         switch( data->message )
//         {
//         case AndroidWindowMessages.Timer:
//            //if( (int)data->parameter1 == suspendModeTimerID )
//            //{
//            //   if( !instance.IsApplicationIdlePossibly() )
//            //      App.ApplicationIdle( true );

//            //   //return IntPtr.Zero;
//            //}
//            //else if( (int)data->parameter1 == 11111 )
//            //{
//            //   App.ApplicationIdle( false );
//            //   //return IntPtr.Zero;
//            //}
//            break;

//         case AndroidWindowMessages.Paint:
//            x;
//            App.ApplicationIdle( false );
//            break;

//         case AndroidWindowMessages.MouseDown:
//            App.DoMouseDown( (EMouseButtons)data->parameter1 );
//            break;

//         case AndroidWindowMessages.MouseUp:
//            App.DoMouseUp( (EMouseButtons)data->parameter1 );
//            break;

//         case AndroidWindowMessages.MouseMove:
//            {
//               Vec2I pos = new Vec2I( data->parameter1, data->parameter2 );
//               mousePosition = pos.ToVec2() / App.VideoMode.ToVec2();
//               App.DoMouseMove();
//            }
//            break;

//         case AndroidWindowMessages.KeyDown:
//            {
//               EKeys eKey = (EKeys)data->parameter1;
//               int character = (int)data->parameter2;

//               bool handled = false;
//               bool suppressKeyPress = false;

//               if( eKey != (EKeys)0 )
//               {
//                  KeyEvent keyEvent = new KeyEvent( eKey );
//                  if( App.DoKeyDown( keyEvent ) )
//                     handled = true;

//                  if( eKey == EKeys.LShift || eKey == EKeys.RShift )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Shift );
//                     if( App.DoKeyDown( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LControl || eKey == EKeys.RControl )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Control );
//                     if( App.DoKeyDown( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Alt );
//                     if( App.DoKeyDown( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Command );
//                     if( App.DoKeyDown( keyEvent ) )
//                        handled = true;
//                  }

//                  if( keyEvent.SuppressKeyPress )
//                     suppressKeyPress = true;
//               }

//               if( handled )
//               {
//               }

//               if( !suppressKeyPress && character != 0 )
//               {
//                  KeyPressEvent keyPressEvent = new KeyPressEvent( (char)character );
//                  App.DoKeyPress( keyPressEvent );
//               }
//            }
//            break;

//         case AndroidWindowMessages.KeyUp:
//            {
//               EKeys eKey = (EKeys)data->parameter1;

//               bool handled = false;

//               if( eKey != (EKeys)0 )
//               {
//                  KeyEvent keyEvent = new KeyEvent( eKey );
//                  if( App.DoKeyUp( keyEvent ) )
//                     handled = true;

//                  if( eKey == EKeys.LShift || eKey == EKeys.RShift )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Shift );
//                     if( App.DoKeyUp( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LControl || eKey == EKeys.RControl )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Control );
//                     if( App.DoKeyUp( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LAlt || eKey == EKeys.RAlt )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Alt );
//                     if( App.DoKeyUp( keyEvent ) )
//                        handled = true;
//                  }
//                  if( eKey == EKeys.LCommand || eKey == EKeys.RCommand )
//                  {
//                     keyEvent = new KeyEvent( EKeys.Command );
//                     if( App.DoKeyUp( keyEvent ) )
//                        handled = true;
//                  }
//               }

//               if( handled )
//               {
//               }
//            }
//            break;
//         }
//      }

//      public override void ProcessChangingVideoMode()
//      {
//      }

//      public override void UpdateSystemCursorFileName()
//      {
//         x;
//      }

//      unsafe public override IList<DisplayInfo> GetAllDisplays()
//      {
//         List<DisplayInfo> result = new List<DisplayInfo>();

//         //uint[] buffer = new uint[ 1024 ];
//         //int count;

//         //fixed( uint* pBuffer = buffer )
//         //{
//         //   count = AndroidAppNativeWrapper.GetActiveDisplayList( buffer.Length, pBuffer );
//         //}

//         //for( int n = 0; n < count; n++ )
//         //{
//         //   uint display = buffer[ n ];

//         //   IntPtr deviceNamePointer;
//         //   Recti bounds;
//         //   Recti workingArea;
//         //   bool primary;

//         //   AndroidAppNativeWrapper.GetDisplayInfo( display, out deviceNamePointer, out bounds,
//         //       out workingArea, out primary );

//         //   string deviceName = AndroidAppNativeWrapper.GetOutString( deviceNamePointer );

//         //   DisplayInfo displayInfo = new DisplayInfo( deviceName, bounds, workingArea, primary );
//         //   result.Add( displayInfo );
//         //}

//         if( result.Count == 0 )
//         {
//            RectI area = new RectI( Vec2I.Zero, GetScreenSize() );
//            DisplayInfo info = new DisplayInfo( "Primary", area, area, true );
//            result.Add( info );
//         }

//         return result;
//      }

//      public override void SetWindowRectangle( RectI rectangle )
//      {
//         x;

//         x;//vladimir we can't resize window in android
//         //App.preventCallSetWindowSize = true;
//         //App.VideoMode = rectangle.Size;
//         //App.preventCallSetWindowSize = false;

//         //AndroidAppNativeWrapper.SetWindowRectangle(rectangle.Left, rectangle.Top, rectangle.Right,
//         //    rectangle.Bottom);

//         //App.DoResize();
//      }

//      public override void GetSystemLanguage( out string name, out string englishName )
//      {
//         x;
//         name = "";
//         englishName = "";
//      }

//   }
//}
