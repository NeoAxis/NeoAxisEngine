// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The console of the Player app.
	/// </summary>
	public static class EngineConsole
	{
		//visual
		static bool needLoadTextureAndFont = true;
		static FontComponent font;
		static double fontSize = 0.025;
		static ImageComponent texture;

		static Vector2 textureOffset;
		static float maxTransparency = .9f;

		//commands
		static List<Command> commands = new List<Command>();

		//actions
		static bool active;
		static bool autoOpening = true;
		static double transparency;

		struct OldString
		{
			public string text;
			public ColorValue color;

			public OldString( string text, ColorValue color )
			{
				this.text = text;
				this.color = color;
			}
		}
		static List<OldString> strings = new List<OldString>();
		static int stringDownPosition = -1;
		static string currentString = "";

		//history
		static List<string> history = new List<string>();
		static int currentHistoryIndex;

		///////////////////////////////////////////

		public class Command
		{
			internal string name;
			internal Method handler;
			internal MethodExtended extendedHandler;
			internal string description;
			internal object userData;

			public delegate void Method( string arguments );
			public delegate void MethodExtended( string arguments, object userData );

			public string Name
			{
				get { return name; }
			}

			public Method Handler
			{
				get { return handler; }
			}

			public MethodExtended ExtendedHandler
			{
				get { return extendedHandler; }
			}

			public string Description
			{
				get { return description; }
			}

			public object UserData
			{
				get { return userData; }
			}
		}

		///////////////////////////////////////////

		public delegate void ExecuteStringEventDelegate( string text, ref bool skip );
		public static event ExecuteStringEventDelegate ExecuteStringEvent;

		public delegate void UnknownCommandHandlerDelegate( string name, string arguments, ref bool handled );
		public static event UnknownCommandHandlerDelegate UnknownCommandHandler;

		public delegate void KeyDownDelegate( KeyEvent e, ref bool handled, ref bool result );
		public static event KeyDownDelegate KeyDown;

		public delegate void KeyPressDelegate( KeyPressEvent e, ref bool handled, ref bool result );
		public static event KeyPressDelegate KeyPress;

		public delegate void MouseWheelDelegate( int delta, ref bool handled, ref bool result );
		public static event MouseWheelDelegate MouseWheel;

		public delegate void TouchDelegate( TouchData e, ref bool handled, ref bool result );
		public static event TouchDelegate Touch;

		public delegate void TickDelegate( float delta, ref bool handled );
		public static event TickDelegate Tick;

		public delegate void RenderUIDelegate( ref bool handled );
		public static event RenderUIDelegate RenderUI;

		///////////////////////////////////////////

		public static void Init()
		{
			AddStandardCommands();

			//add commands for EngineApp.Instance.Config fields and properties
			foreach( EngineConfig.Parameter parameter in EngineConfig.Parameters )
				RegisterConfigParameter( parameter );

			Print( string.Format( "Welcome to {0}!", EngineInfo.NameWithVersion ), new ColorValue( 1, 1, 0 ) );
			Print( "" );
			Print( "Use \"Commands\" command to look the list of available commands." );
			Print( "Press \"Tab\" to autocompletion the command name." );
			Print( "Press \"Up\", \"Down\" to select the last processed commands." );
			Print( "Press \"Page Up\", \"Page Down\" to scroll the history of the commands." );
			Print( "-------------------------------------------------------------------------------" );
		}

		public static bool Active
		{
			get { return active; }
			set
			{
				if( active == value )
					return;

				active = value;
				currentString = "";
			}
		}

		public static bool AutoOpening
		{
			get { return autoOpening; }
			set { autoOpening = value; }
		}

		static void AddCommand( string name, Command.Method handler, Command.MethodExtended extendedHandler, string description, object userData )
		{
			if( GetCommandByName( name ) != null )
				return;

			Command command = new Command();
			command.name = name;
			command.handler = handler;
			command.extendedHandler = extendedHandler;
			command.description = description;
			command.userData = userData;

			for( int n = 0; n < commands.Count; n++ )
			{
				if( string.Equals( name, commands[ n ].Name, StringComparison.OrdinalIgnoreCase ) )
				{
					commands.Insert( n, command );
					return;
				}
			}
			commands.Add( command );
		}

		public static void AddCommand( string name, Command.Method handler, string description )
		{
			AddCommand( name, handler, null, description, null );
		}

		public static void AddCommand( string name, Command.MethodExtended handler, object userData, string description )
		{
			AddCommand( name, null, handler, description, userData );
		}

		public static void AddCommand( string name, Command.Method handler )
		{
			AddCommand( name, handler, null );
		}

		public static void AddCommand( string name, Command.MethodExtended handler, object userData )
		{
			AddCommand( name, handler, userData, null );
		}

		public static void RemoveCommand( string name )
		{
			Command command = GetCommandByName( name );
			if( command != null )
				commands.Remove( command );
		}

		public static ReadOnlyCollection<Command> Commands
		{
			get { return commands.AsReadOnly(); }
		}

		static Command GetCommandByName( string name )
		{
			foreach( Command command in commands )
				if( string.Equals( command.Name, name, StringComparison.OrdinalIgnoreCase ) )
					return command;
			return null;
		}

		//Actions

		static Viewport MainViewport
		{
			get { return RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ]; }
		}

		public static void Print( string text, ColorValue color )
		{
			if( text == null )
				text = "null";

			while( strings.Count > 256 )
				strings.RemoveAt( 0 );

			CanvasRenderer renderer = MainViewport.CanvasRenderer;

			if( font != null && renderer != null )
			{
				var items = font.GetWordWrapLines( fontSize, renderer, text, .98f );
				foreach( var item in items )
				{
					if( stringDownPosition == strings.Count - 1 )
						stringDownPosition++;
					strings.Add( new OldString( item.Text, color ) );
				}
			}
			else
			{
				if( stringDownPosition == strings.Count - 1 )
					stringDownPosition++;
				strings.Add( new OldString( text, color ) );
			}
		}

		public static void Print( string text )
		{
			Print( text, new ColorValue( 1, 1, 1 ) );
		}

		public static void PrintFormatted( string format, params object[] arguments )
		{
			Print( string.Format( format, arguments ) );
		}

		public static bool ExecuteString( string text )
		{
			if( text == null )
				return false;

			var textFixed = text.Trim();
			if( textFixed == "" )
				return false;

			var skip = false;
			ExecuteStringEvent?.Invoke( textFixed, ref skip );
			if( skip )
				return false;

			string name;
			string arguments;
			{
				int index = textFixed.IndexOf( ' ' );

				if( index != -1 )
				{
					name = textFixed.Substring( 0, index );
					arguments = textFixed.Substring( index + 1, textFixed.Length - index - 1 );
				}
				else
				{
					name = textFixed;
					arguments = "";
				}
			}

			name = name.Trim();
			arguments = arguments.Trim();

			history.Add( textFixed );
			currentHistoryIndex = history.Count;
			while( history.Count > 256 )
				history.RemoveAt( 0 );

			var command = GetCommandByName( name );
			if( command == null )
			{
				var handled = false;
				UnknownCommandHandler?.Invoke( name, arguments, ref handled );
				if( !handled )
					Print( string.Format( "Unknown command \"{0}\".", name ), new ColorValue( 1, 0, 0 ) );
				return false;
			}

			if( command.Handler != null )
				command.Handler( arguments );
			if( command.ExtendedHandler != null )
				command.ExtendedHandler( arguments, command.UserData );

			return true;
		}

		public static void Clear()
		{
			strings.Clear();
			stringDownPosition = -1;
		}

		public static bool PerformKeyDown( KeyEvent e )
		{
			{
				var handled = false;
				var result = true;
				KeyDown?.Invoke( e, ref handled, ref result );
				if( handled )
					return result;
			}

			if( e.Key == EKeys.Oemtilde || e.Key == EKeys.Paragraph || e.Key == EKeys.F12 )
			{
				e.SuppressKeyPress = true;
				if( !Active )
					Active = AutoOpening || MainViewport.IsKeyPressed( EKeys.Control );
				else
				{
					Active = false;
					if( MainViewport.IsKeyPressed( EKeys.Control ) )
						AutoOpening = !AutoOpening;
				}
				return true;
			}

			if( !active )
				return false;

			switch( e.Key )
			{
			case EKeys.Up:
				if( currentHistoryIndex > 0 )
					currentHistoryIndex--;
				if( currentHistoryIndex < history.Count )
					currentString = history[ currentHistoryIndex ];
				return true;

			case EKeys.Down:
				if( currentHistoryIndex < history.Count )
					currentHistoryIndex++;
				if( currentHistoryIndex != 0 && currentHistoryIndex <= history.Count )
				{
					currentString = "";
					if( currentHistoryIndex < history.Count )
						currentString = history[ currentHistoryIndex ];
				}
				return true;

			case EKeys.PageUp:
				stringDownPosition -= 8;
				if( stringDownPosition < 0 )
					stringDownPosition = 0;
				if( stringDownPosition > strings.Count - 1 )
					stringDownPosition = strings.Count - 1;
				return true;

			case EKeys.PageDown:
				stringDownPosition += 8;
				if( stringDownPosition > strings.Count - 1 )
					stringDownPosition = strings.Count - 1;
				return true;

			case EKeys.Home:
				stringDownPosition = 0;
				if( stringDownPosition < 0 )
					stringDownPosition = 0;
				if( stringDownPosition > strings.Count - 1 )
					stringDownPosition = strings.Count - 1;
				return true;

			case EKeys.End:
				stringDownPosition = strings.Count - 1;
				return true;

			case EKeys.Back:
				if( currentString != "" )
					currentString = currentString.Substring( 0, currentString.Length - 1 );
				return true;

			case EKeys.Return:
				{
					stringDownPosition = strings.Count - 1;

					currentString = currentString.Trim();

					if( currentString != "" )
					{
						Print( currentString, new ColorValue( 0, 1, 0 ) );
						ExecuteString( currentString );
						currentString = "";
					}
				}
				return true;

			case EKeys.Tab:
				{
					string str = currentString.Trim();

					if( str.Length != 0 )
					{
						int count = 0;
						string lastFounded = "";
						for( int n = 0; n < commands.Count; n++ )
						{
							string name = commands[ n ].Name;

							if( name.Length >= str.Length && string.Equals( name.Substring( 0, str.Length ), str, StringComparison.OrdinalIgnoreCase ) )
							{
								count++;
								lastFounded = commands[ n ].Name;
							}
						}

						if( count != 1 )
						{
							List<string> list = new List<string>( 128 );

							for( int n = 0; n < commands.Count; n++ )
							{
								string name = commands[ n ].Name;

								if( name.Length >= str.Length && string.Equals( name.Substring( 0, str.Length ), str, StringComparison.OrdinalIgnoreCase ) )
								{
									list.Add( name );
								}
							}

							foreach( string s in list )
								Print( s );

							currentString = str;

							if( list.Count != 0 )
							{
								int pos = currentString.Length;
								while( true )
								{
									if( list[ 0 ].Length <= pos )
										break;
									char c = list[ 0 ][ pos ];
									for( int n = 1; n < list.Count; n++ )
									{
										if( list[ n ].Length <= pos )
											goto end;
										if( c != list[ n ][ pos ] )
											goto end;
									}
									currentString += c.ToString();
									pos++;
								}
								end:;
							}
						}
						else
							currentString = lastFounded + " ";
					}
				}
				return true;
			}

			return true;
		}

		public static bool PerformKeyPress( KeyPressEvent e )
		{
			{
				var handled = false;
				var result = true;
				KeyPress?.Invoke( e, ref handled, ref result );
				if( handled )
					return result;
			}

			if( !active )
				return false;

			if( currentString.Length < 1024 )
			{
				bool allowCharacter;

				if( font != null )
					allowCharacter = e.KeyChar >= 32 && font.IsCharacterInitialized( e.KeyChar );
				else
					allowCharacter = e.KeyChar >= 32 && e.KeyChar < 128;

				if( allowCharacter )
					currentString += e.KeyChar.ToString();
			}

			return true;
		}

		public static bool PerformMouseWheel( int delta )
		{
			{
				var handled = false;
				var result = true;
				MouseWheel?.Invoke( delta, ref handled, ref result );
				if( handled )
					return result;
			}

			if( !active )
				return false;

			int step = delta / 10;
			stringDownPosition -= step;
			if( stringDownPosition < 0 )
				stringDownPosition = 0;
			if( stringDownPosition > strings.Count - 1 )
				stringDownPosition = strings.Count - 1;

			return true;
		}

		public static bool PerformTouch( TouchData e )
		{
			{
				var handled = false;
				var result = true;
				Touch?.Invoke( e, ref handled, ref result );
				if( handled )
					return result;
			}

			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( Active && e.Position.Y < 0.5 )
				{
					Active = false;
					return true;
				}
				break;
			}

			return false;
		}

		public static void PerformTick( float delta )
		{
			{
				var handled = false;
				Tick?.Invoke( delta, ref handled );
				if( handled )
					return;
			}

			if( active )
			{
				transparency += delta;
				if( transparency > maxTransparency )
					transparency = maxTransparency;
			}
			else
			{
				transparency -= delta;
				if( transparency < 0 )
					transparency = 0;
			}

			textureOffset.X += delta / 30.0f;
			if( textureOffset.X > 1.0f )
				textureOffset.X -= 1.0f;
			textureOffset.Y += delta / 120.0f;
			if( textureOffset.Y > 1.0f )
				textureOffset.Y -= 1.0f;
		}

		public static void PerformRenderUI()
		{
			var handled = false;
			RenderUI?.Invoke( ref handled );
			if( handled )
				return;

			CanvasRenderer renderer = MainViewport.CanvasRenderer;

			if( transparency == 0.0f )
				return;

			//load backgrouund texture and font
			if( texture != null && texture.Disposed )
				needLoadTextureAndFont = true;
			if( needLoadTextureAndFont )
			{
				texture = ResourceManager.LoadResource<ImageComponent>( "Base\\UI\\Images\\Console.png" );

				font = renderer.DefaultFont;
				//font = EngineFontManager.Instance.LoadFont( "Default", .025f );
				needLoadTextureAndFont = false;
			}

			if( font == null )
				return;

			Vector2F viewportSize = renderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2F();
			Vector2F shadowOffset = new Vector2F( 1, 1 ) / viewportSize;

			//draw background
			var textureRect = new Rectangle( 0, 0, 10 * renderer.AspectRatio, 10 / 2 );
			textureRect -= textureOffset;
			renderer.AddQuad( new Rectangle( 0, 0, 1, .5f ), textureRect, texture, new ColorValue( 1, 1, 1, transparency ), false );

			//var textureRect = new Rectangle( 0, 1.0 - renderer.AspectRatioInv, 1, 1 );
			//renderer.AddQuad( new Rectangle( 0, 0, 1, .5f ), textureRect, texture, new ColorValue( 0.7, 0.7, 0.7, transparency ), false );

			//draw border line
			renderer.AddQuad( new Rectangle( 0, .5f, 1, .508f ), new ColorValue( 0.29f, 0.6f, 0.86f, 0.9f * transparency ) );

			//draw background info
			string staticText;
			if( SystemSettings.MobileDevice )
				staticText = "Click to the console to hide it";
			else
				staticText = "Press \"~\" or \"F12\" to hide console\r\nPress \"Ctrl + ~\" to hide and disable auto opening";
			renderer.AddTextWordWrap( staticText, new RectangleF( 0, 0, .995f, .495f ), EHorizontalAlignment.Right, false, EVerticalAlignment.Bottom, 0,
				new ColorValue( 0.8, 0.8, 0.8, transparency ) );

			float fontheight = (float)fontSize;// font.Height;

			float x = .01f;

			float y = .5f - fontheight;

			string str;
			if( stringDownPosition != strings.Count - 1 )
			{
				str = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
					" - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
					" - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
					" - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
					" - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -";
			}
			else
				str = currentString + "_";

			renderer.PushClipRectangle( new RectangleF( 0, 0, .975f, 1 ) );
			renderer.AddText( font, fontSize, str, new Vector2F( x, y ) + shadowOffset, EHorizontalAlignment.Left, EVerticalAlignment.Center, new ColorValue( 0, 0, 0, transparency / 2 ) );
			renderer.AddText( font, fontSize, str, new Vector2F( x, y ), EHorizontalAlignment.Left, EVerticalAlignment.Center, new ColorValue( 1, 1, 1, transparency ) );
			renderer.PopClipRectangle();

			y -= fontheight + fontheight * .5f;

			int startpos = stringDownPosition;
			if( startpos > strings.Count - 1 )
				startpos = strings.Count - 1;
			for( int n = startpos; n >= 0 && y - fontheight > 0; n-- )
			{
				var text = strings[ n ].text;
				int lineCount = text.Count( f => f == '\n' ) + 1;

				float y2 = y - fontheight * ( lineCount - 1 ) / 2;

				renderer.AddText( font, fontSize, text, new Vector2F( x, y2 ) + shadowOffset, EHorizontalAlignment.Left, EVerticalAlignment.Center, strings[ n ].color * new ColorValue( 0, 0, 0, transparency / 2 ) );
				renderer.AddText( font, fontSize, text, new Vector2F( x, y2 ), EHorizontalAlignment.Left, EVerticalAlignment.Center, strings[ n ].color * new ColorValue( 1, 1, 1, transparency ) );
				y -= fontheight * lineCount;
			}
		}

		static void OnConsoleConfigCommand( string arguments, object userData )
		{
			EngineConfig.Parameter parameter = (EngineConfig.Parameter)userData;

			if( string.IsNullOrEmpty( arguments ) )
			{
				object value = parameter.GetValue();
				Print( string.Format( "Value: \"{0}\", Default value: \"{1}\"", value != null ? value : "(null)", parameter.DefaultValue ) );
				return;
			}

			try
			{
				if( parameter.Field != null )
				{
					object value = SimpleTypes.ParseValue( parameter.Field.FieldType, arguments );
					if( value == null )
						throw new Exception( "Not simple type" );
					parameter.Field.SetValue( null, value );
				}
				else if( parameter.Property != null )
				{
					object value = SimpleTypes.ParseValue( parameter.Property.PropertyType, arguments );
					if( value == null )
						throw new Exception( "Not simple type" );
					parameter.Property.SetValue( null, value, null );
				}
			}
			catch( FormatException e )
			{
				string s = "";
				if( parameter.Field != null )
					s = parameter.Field.FieldType.ToString();
				else if( parameter.Property != null )
					s = parameter.Property.PropertyType.ToString();
				Print( string.Format( "Config : Invalid parameter format \"{0}\" {1}", s, e.Message ), new ColorValue( 1, 0, 0 ) );
			}
		}

		public static void RegisterConfigParameter( EngineConfig.Parameter parameter )
		{
			string strType = "";
			if( parameter.Field != null )
				strType = parameter.Field.FieldType.Name;
			else if( parameter.Property != null )
				strType = parameter.Property.PropertyType.Name;

			string description = string.Format( "\"{0}\", Default: \"{1}\"", strType, parameter.DefaultValue );
			AddCommand( parameter.Name, OnConsoleConfigCommand, parameter, description );
		}

		public static void ScrollDown()
		{
			stringDownPosition = strings.Count - 1;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////

		//standard commands

		static void OnConsoleQuit( string arguments )
		{
			EngineApp.NeedExit = true;
		}

		static void OnConsoleClear( string arguments )
		{
			Clear();
		}

		static void OnConsoleCommands( string arguments )
		{
			Print( "The list of commands:" );
			foreach( Command command in commands )
			{
				string text = command.Name;
				if( command.Description != null )
					text += " (" + command.Description + ")";
				Print( text );
			}
		}

		static void OnConsoleFullscreen( string arguments )
		{
			if( !string.IsNullOrEmpty( arguments ) )
			{
				try
				{
					bool value;
					if( arguments == "1" )
						value = true;
					else if( arguments == "0" )
						value = false;
					else
						value = bool.Parse( arguments );

					//!!!!
					EngineApp.SetFullscreenMode( value, EngineApp.FullscreenSize );
					//EngineApp.Fullscreen = value;
				}
				catch( Exception ex )
				{
					Log.Warning( ex.Message );
				}
			}
			else
				Print( string.Format( "Value: \"{0}\", Default value: \"{1}\"", EngineApp.FullscreenEnabled, true ) );
		}

		static void OnConsoleVideoMode( string arguments )
		{
			if( !string.IsNullOrEmpty( arguments ) )
			{
				try
				{
					Vector2I mode = Vector2I.Parse( arguments );

					if( EngineApp.FullscreenEnabled && !SystemSettings.VideoModeExists( mode ) )
					{
						string text = string.Format( "Cannot change screen resolution to \"{0}x{1}\". This resolution is not supported by the system.", mode.X, mode.Y );
						Log.Warning( text );
						return;
					}

					//!!!!
					EngineApp.SetFullscreenMode( EngineApp.FullscreenEnabled, mode );
					//EngineApp.Instance.VideoMode = mode;
				}
				catch( Exception ex )
				{
					Log.Warning( ex.Message );
				}
			}
			else
				Print( string.Format( "Value: \"{0}\"", EngineApp.FullscreenSize ) );
		}

		//static void OnLogNativeMemoryStatistics( string arguments )
		//{
		//	NativeMemoryManager.LogAllocationStatistics();
		//	Print( "Done. See log file in UserSettings\\Logs folder." );
		//}

		static void OnConsoleEngineTimeScale( string arguments )
		{
			if( !string.IsNullOrEmpty( arguments ) )
			{
				try
				{
					EngineApp.EngineTimeScale = float.Parse( arguments );
				}
				catch( Exception ex )
				{
					Log.Warning( ex.Message );
				}
			}
			else
			{
				Print( string.Format( "Value: \"{0}\"", EngineApp.EngineTimeScale ) );
			}
		}

		static void OnConsoleSoundPitchScale( string arguments )
		{
			if( !string.IsNullOrEmpty( arguments ) )
			{
				try
				{
					SoundWorld.MasterChannelGroup.Pitch = float.Parse( arguments );
				}
				catch( Exception ex )
				{
					Log.Warning( ex.Message );
				}
			}
			else
			{
				Print( string.Format( "Value: \"{0}\"", SoundWorld.MasterChannelGroup.Pitch ) );
			}
		}

		static void AddStandardCommands()
		{
			AddCommand( "Quit", OnConsoleQuit );
			AddCommand( "Exit", OnConsoleQuit );
			AddCommand( "Clear", OnConsoleClear );
			AddCommand( "Commands", OnConsoleCommands );
			AddCommand( "Fullscreen", OnConsoleFullscreen );
			AddCommand( "VideoMode", OnConsoleVideoMode );
			//AddCommand( "LogNativeMemoryStatistics", OnLogNativeMemoryStatistics );
			AddCommand( "EngineTimeScale", OnConsoleEngineTimeScale );
			AddCommand( "SoundPitchScale", OnConsoleSoundPitchScale );
		}
	}
}
