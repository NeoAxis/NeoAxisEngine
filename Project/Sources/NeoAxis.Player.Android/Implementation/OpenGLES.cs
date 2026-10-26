// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#if OPENGLES
using System;
using System.Collections.Generic;
using System.Threading;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using Android.Content;
using Android.Views.InputMethods;
using Android.Views;
using Android.Text;

namespace NeoAxis.Player.Android
{
	public class RendererClass : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		volatile bool surfaceResized;

		//!!!!better to recreate gpu resources, but harder
		//restart app after surface recreate
		public bool needRestartEngine;

		/////////////////////////////////////////

		public void OnSurfaceCreated( IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config )
		{
			Log.InvisibleInfo( "Renderer: OnSurfaceCreated." );

			if( Engine.engineInitialized )
				needRestartEngine = true;
		}

		public void OnSurfaceChanged( IGL10 gl, int width, int height )
		{
			PlatformFunctionalityAndroid.screenSize = new Vector2I( width, height );
			surfaceResized = true;
		}

		public void OnDrawFrame( IGL10 gl )
		{
			//restart app after surface recreate
			if( needRestartEngine )
			{
				needRestartEngine = false;

				if( Engine.engineInitialized )
				{
					Engine.activity.RestartApp();
					return;

					////UI thread can be changed when initially app ran with disabled screen. update it.
					//VirtualFileSystem.SetMainThread( Thread.CurrentThread );

					//Engine.ShutdownEngine();

					//Engine.engineInitialized = false;
				}
			}

			//init engine
			if( !Engine.engineInitialized )
			{
				try
				{
					Engine.InitEngine();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
					return;
				}
				Engine.engineInitialized = true;
			}

			try
			{
				//UI thread can be changed when initially app ran with disabled screen. update it.
				VirtualFileSystem.SetMainThread( Thread.CurrentThread );

				//update screen size
				if( surfaceResized )
				{
					RenderingSystem.ApplicationRenderTarget?.WindowMovedOrResized( PlatformFunctionalityAndroid.screenSize );
					surfaceResized = false;
				}

				//process input
				Engine.ProcessInputEvents();
				Engine.activity.UpdateSoftInput();

				//engine tick and render
				EngineApp.CreatedWindowApplicationIdle( false );

				//update screen settings
				Engine.activity.UpdateScreenOrientation();

				if( EngineApp.NeedExit )
					Java.Lang.JavaSystem.Exit( 0 );
			}
			catch( Exception e )
			{
				Log.FatalAsException( e.ToString() );
				return;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EngineInputConnection : BaseInputConnection
	{
		private string _currentComposingText = string.Empty;

		public EngineInputConnection( View targetView, bool fullEditor )
			: base( targetView, fullEditor )
		{
		}

		// 1. Commits fully formed text (regular typing, selecting a suggested word)
		public override bool CommitText( Java.Lang.ICharSequence text, int newCursorPosition )
		{
			if( text != null && text.Length() > 0 )
			{
				// Reset active composing state once the final text is committed
				_currentComposingText = string.Empty;
				EnqueueString( text.ToString() );
				return true;
			}

			return base.CommitText( text, newCursorPosition );
		}

		// 2. Handles backspace/delete requests from software keyboards
		// Most modern virtual keyboards call this method instead of sending KeyEvent.Keycode.Del
		public override bool DeleteSurroundingText( int beforeLength, int afterLength )
		{
			if( Engine.engineInitialized && beforeLength > 0 )
			{
				lock( Engine.inputEventQueue )
				{
					for( int i = 0; i < beforeLength; i++ )
					{
						if( Engine.inputEventQueue.Count >= 200 ) break;

						Engine.inputEventQueue.Enqueue( new Engine.KeyDownEventItem
						{
							KeyCode = EKeys.Back
						} );
					}
				}
				return true;
			}

			return base.DeleteSurroundingText( beforeLength, afterLength );
		}

		// 3. Handles hardware keys or keyboards forwarding raw key events (Enter, Del, etc.)
		public override bool SendKeyEvent( global::Android.Views.KeyEvent? e )
		{
			if( e.Action == KeyEventActions.Down && Engine.engineInitialized )
			{
				if( e.KeyCode == Keycode.Del )
				{
					EnqueueKey( EKeys.Back, '\0' );
					return true;
				}
				if( e.KeyCode == Keycode.Enter )
				{
					EnqueueKey( EKeys.Return, '\n' );
					return true;
				}

				// Fallback for hardware keyboards or printable characters
				if( e.UnicodeChar != 0 )
				{
					EnqueueKey( EKeys.None, (char)e.UnicodeChar );
					return true;
				}
			}

			return base.SendKeyEvent( e );
		}

		// 4. Handles software keyboard action buttons ("Done", "Search", "Send", "Go")
		public override bool PerformEditorAction( ImeAction actionCode )
		{
			EnqueueKey( EKeys.Return, '\n' );
			return true;
		}

		// 5. Handles predictive text, autocorrect suggestions, and swipe typing
		public override bool SetComposingText( Java.Lang.ICharSequence text, int newCursorPosition )
		{
			if( text == null )
				return false;

			var newText = text.ToString();

			// Remove previously previewed composition characters from the engine buffer
			for( int i = 0; i < _currentComposingText.Length; i++ )
				EnqueueKey( EKeys.Back, '\0' );

			// Apply new composing segment
			_currentComposingText = newText;
			EnqueueString( _currentComposingText );

			return true;
		}

		// Called when the IME confirms the composing text
		public override bool FinishComposingText()
		{
			_currentComposingText = string.Empty;
			return base.FinishComposingText();
		}

		private void EnqueueString( string str )
		{
			if( !Engine.engineInitialized )
				return;

			lock( Engine.inputEventQueue )
			{
				// Use StringInfo to avoid splitting UTF-16 surrogate pairs (e.g. emojis)
				var enumerator = System.Globalization.StringInfo.GetTextElementEnumerator( str );
				while( enumerator.MoveNext() )
				{
					if( Engine.inputEventQueue.Count >= 200 ) break;

					string element = enumerator.GetTextElement();
					if( element.Length == 1 )
						Engine.inputEventQueue.Enqueue( new Engine.KeyDownEventItem { Character = element[ 0 ] } );
					else
					{
						// Fallback for multi-char surrogate pairs if your engine accepts char units
						foreach( char ch in element )
							Engine.inputEventQueue.Enqueue( new Engine.KeyDownEventItem { Character = ch } );
					}
				}
			}
		}

		private void EnqueueKey( EKeys key, char ch )
		{
			if( !Engine.engineInitialized )
				return;

			lock( Engine.inputEventQueue )
			{
				if( Engine.inputEventQueue.Count < 200 )
					Engine.inputEventQueue.Enqueue( new Engine.KeyDownEventItem { KeyCode = key, Character = ch } );
			}
		}
	}

	//public class EngineInputConnection : BaseInputConnection
	//{
	//	public EngineInputConnection( MyGLSurfaceView targetView, bool fullEditor )
	//		: base( targetView, fullEditor )
	//	{
	//	}

	//	public override bool CommitText( Java.Lang.ICharSequence text, int newCursorPosition )
	//	{
	//		if( text != null && text.Length() > 0 )
	//		{
	//			if( Engine.engineInitialized )
	//			{
	//				var characters = text.ToString();

	//				lock( Engine.inputEventQueue )
	//				{
	//					foreach( var c in characters )
	//					{
	//						if( Engine.inputEventQueue.Count >= 200 )
	//							break;

	//						var item = new Engine.KeyDownEventItem();
	//						item.Character = c;

	//						Engine.inputEventQueue.Enqueue( item );
	//					}
	//				}
	//			}

	//			return true;
	//		}

	//		return base.CommitText( text, newCursorPosition );
	//	}

	//	//public override bool DeleteSurroundingText( int beforeLength, int afterLength )
	//	//{
	//	//	if( Engine.engineInitialized )
	//	//	{
	//	//		lock( Engine.inputEventQueue )
	//	//		{
	//	//			for( int n = 0; n < beforeLength; n++ )
	//	//			{
	//	//				if( Engine.inputEventQueue.Count >= 200 )
	//	//					break;

	//	//				var item = new Engine.KeyDownEventItem();
	//	//				item.KeyCode = EKeys.Back;
	//	//				Engine.inputEventQueue.Enqueue( item );
	//	//			}
	//	//		}
	//	//		return true;
	//	//	}

	//	//	return base.DeleteSurroundingText( beforeLength, afterLength );
	//	//}

	//}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class MyGLSurfaceView : GLSurfaceView
	{
		public MyGLSurfaceView( Context? context ) : base( context )
		{
		}

		public override IInputConnection? OnCreateInputConnection( EditorInfo? outAttrs )
		{
			// NoExtractUi: prevents the IME from going full-screen in landscape orientation
			// ImeAction.Done: replaces default Enter with a "Done" action button
			outAttrs.ImeOptions = ImeFlags.NoExtractUi | (ImeFlags)ImeAction.Done;
			outAttrs.InputType = InputTypes.ClassText | InputTypes.TextVariationNormal;

			return new EngineInputConnection( this, false );
		}
	}

}
#endif