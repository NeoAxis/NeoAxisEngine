// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis.Editor;
using System.Net.WebSockets;

namespace NeoAxis
{
	/// <summary>
	/// Standard UI element for text input.
	/// </summary>
	public class UIEdit : UIControl
	{
		//!!!!add CanvasRenderer or Viewport as a key?
		internal RectangleF[] lastCharacterRectangles;

		//EHorizontalAlignment textHorizontalAlignment = EHorizontalAlignment.Left;

		/////////////////////////////////////////

		/// <summary>
		/// Edit box maximum text characters count.
		/// </summary>
		[DefaultValue( 1000000 )]
		public Reference<int> MaxCharacterCount
		{
			get { if( _maxCharacterCount.BeginGet() ) MaxCharacterCount = _maxCharacterCount.Get( this ); return _maxCharacterCount.value; }
			set
			{
				if( _maxCharacterCount.BeginSet( this, ref value ) )
				{
					try
					{
						MaxCharacterCountChanged?.Invoke( this );

						var text = Text.Value;
						Text = text;
						//if( Text != null && Text.Length > MaxCharacterCount )
						//	Text = Text.Substring( 0, MaxCharacterCount );
					}
					finally { _maxCharacterCount.EndSet(); }
				}
			}
		}
		public event Action<UIEdit> MaxCharacterCountChanged;
		ReferenceField<int> _maxCharacterCount = 1000000;

		/// <summary>
		/// The character used to mask characters of a password.
		/// </summary>
		[DefaultValue( '\0' )]
		public Reference<char> PasswordCharacter
		{
			get { if( _passwordCharacter.BeginGet() ) PasswordCharacter = _passwordCharacter.Get( this ); return _passwordCharacter.value; }
			set { if( _passwordCharacter.BeginSet( this, ref value ) ) { try { PasswordCharacterChanged?.Invoke( this ); } finally { _passwordCharacter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PasswordCharacter"/> property value changes.</summary>
		public event Action<UIEdit> PasswordCharacterChanged;
		ReferenceField<char> _passwordCharacter = '\0';

		/// <summary>
		/// The displayed text when text of the edit box is empty.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> HintWhenEmpty
		{
			get { if( _hintWhenEmpty.BeginGet() ) HintWhenEmpty = _hintWhenEmpty.Get( this ); return _hintWhenEmpty.value; }
			set { if( _hintWhenEmpty.BeginSet( this, ref value ) ) { try { HintWhenEmptyChanged?.Invoke( this ); } finally { _hintWhenEmpty.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HintWhenEmpty"/> property value changes.</summary>
		public event Action<UIEdit> HintWhenEmptyChanged;
		ReferenceField<string> _hintWhenEmpty = "";

		[DefaultValue( 0 )]
		public Reference<int> SelectionStart
		{
			get { if( _selectionStart.BeginGet() ) SelectionStart = _selectionStart.Get( this ); return _selectionStart.value; }
			set { if( _selectionStart.BeginSet( this, ref value ) ) { try { SelectionStartChanged?.Invoke( this ); } finally { _selectionStart.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SelectionStart"/> property value changes.</summary>
		public event Action<UIEdit> SelectionStartChanged;
		ReferenceField<int> _selectionStart = 0;

		[DefaultValue( 0 )]
		public Reference<int> SelectionLength
		{
			get { if( _selectionLength.BeginGet() ) SelectionLength = _selectionLength.Get( this ); return _selectionLength.value; }
			set { if( _selectionLength.BeginSet( this, ref value ) ) { try { SelectionLengthChanged?.Invoke( this ); } finally { _selectionLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SelectionLength"/> property value changes.</summary>
		public event Action<UIEdit> SelectionLengthChanged;
		ReferenceField<int> _selectionLength = 0;

		[DefaultValue( -1 )]
		public Reference<int> CaretPosition
		{
			get { if( _caretPosition.BeginGet() ) CaretPosition = _caretPosition.Get( this ); return _caretPosition.value; }
			set { if( _caretPosition.BeginSet( this, ref value ) ) { try { CaretPositionChanged?.Invoke( this ); } finally { _caretPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CaretPosition"/> property value changes.</summary>
		public event Action<UIEdit> CaretPositionChanged;
		ReferenceField<int> _caretPosition = -1;

		//[DefaultValue( false )]
		//public Reference<bool> Multiline
		//{
		//	get { if( _multiline.BeginGet() ) Multiline = _multiline.Get( this ); return _multiline.value; }
		//	set { if( _multiline.BeginSet( this, ref value ) ) { try { MultilineChanged?.Invoke( this ); } finally { _multiline.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Multiline"/> property value changes.</summary>
		//public event Action<UIEdit> MultilineChanged;
		//ReferenceField<bool> _multiline = false;

		[Browsable( false )]
		public double EditingOrChangeCaretPositionLastTime { get; set; }

		///////////////////////////////////////////

		//!!!!was before

		//public delegate void UpdatingTextControlDelegate( UIEdit sender, ref string text );
		//UpdatingTextControlDelegate updatingTextControl;

		//public delegate bool TextTypingFilterDelegate( UIEdit sender, EKeys key, char character, string newText );
		//TextTypingFilterDelegate textTypingFilter;

		///////////////////////////////////////////

		public UIEdit()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 40 );

			TextChanging += UIEdit_TextChanging;
			//TextChanged += UIEdit_TextChanged;
		}

		private void UIEdit_TextChanging( UIControl sender, ref Reference<string> text )
		{
			if( text.Value.Length > MaxCharacterCount )
				text = new Reference<string>( text.Value.Substring( 0, MaxCharacterCount ), text.GetByReference );
		}

		//private void UIEdit_TextChanged( UIControl obj )
		//{
		//	var v = Text;
		//	if( v.Value.Length > MaxCharacterCount )
		//		Text = new Reference<string>( v.Value.Substring( 0, MaxCharacterCount ), v.GetByReference );

		//	//var text = Text.Value;
		//	//if( text.Length > MaxCharacterCount )
		//	//	text = text.Substring( 0, MaxCharacterCount );
		//	//Text = text;
		//}

		///// <summary>
		///// The text control associated with the edit box.
		///// </summary>
		//[Serialize]
		//[Browsable( false )]
		//public UIText TextControl
		//{
		//	get { return textControl; }
		//	set
		//	{
		//		if( textControl != null )
		//			RemoveComponent( textControl, false );
		//		textControl = value;
		//		if( textControl != null && textControl.Parent == null )
		//			AddComponent( value );

		//		//!!!!
		//		//if( textControl != null )
		//		//	textControl.SupportLocalization = false;
		//	}
		//}

		/// <summary>
		/// Whether control can be focused.
		/// </summary>
		[Browsable( false )]
		public override bool CanFocus
		{
			get
			{
				if( EnabledInHierarchy && VisibleInHierarchy )
				{
					//var parentControl = Parent as UIControl;
					//if( parentControl != null && parentControl.ReadOnlyInHierarchy )
					//	return false;

					var parentCombo = Parent as UICombo;
					if( parentCombo != null && ( parentCombo.GetTextControl() == this || parentCombo.ReadOnlyInHierarchy ) )
						return false;

					return true;
				}
				return false;
				//return EnabledInHierarchy && VisibleInHierarchy; 
			}
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			if( CanFocus )
			{
				bool intoArea = new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition );

				if( intoArea && EnabledInHierarchy )
				{
					if( button == EMouseButtons.Left )
					{
						if( /*Focused &&*/ lastCharacterRectangles != null && ParentContainer != null )
						{
							var screenMousePosition = ParentContainer.ContainerGetMousePosition().ToVector2F();

							var characterIndex = -1;
							var minDistance = float.MaxValue;

							for( var n = 0; n < lastCharacterRectangles.Length; n++ )
							{
								var characterRectangle = lastCharacterRectangles[ n ];

								var distance = characterRectangle.GetPointDistance( screenMousePosition );
								if( characterIndex == -1 || distance < minDistance )
								{
									characterIndex = n;
									if( screenMousePosition.X > characterRectangle.GetCenter().X )
										characterIndex++;
									minDistance = distance;
								}
							}

							if( characterIndex != -1 )
							{
								var oldCaret = GetCaretPosition();
								CaretPosition = characterIndex;

								if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
									UpdateSelection( oldCaret );
								else
									DeselectAll();

								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
							}
						}
					}

					Focus();
					return true;
				}
				else
					Unfocus();
			}

			return base.OnMouseDown( button );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//debug
			//if( EngineApp._DebugCapsLock )
			//{
			//	var text = Text.Value;

			//	if( lastCharacterRectangles != null )
			//	{
			//		//foreach( var r in lastCharacterRectangles )
			//		for( int n = 0; n < lastCharacterRectangles.Length; n++ )
			//		{
			//			try
			//			{
			//				var r = lastCharacterRectangles[ n ];
			//				var t = text[ n ];

			//				renderer.AddQuad( r, new ColorValue( 0, 1, 0, .5f ) );

			//				var rr = r;
			//				rr.Expand( 0.001f );

			//				if( t == '\n' )
			//				{
			//					renderer.AddLine( r.LeftTop, r.RightBottom, new ColorValue( 1, 0, 0, 1 ) );
			//					renderer.AddFillEllipse( rr, 6, new ColorValue( 1, 0, 0 ) );
			//				}
			//				if( t == '\t' )
			//				{
			//					renderer.AddLine( r.LeftBottom, r.RightTop, new ColorValue( 0, 0, 1, 1 ) );
			//					renderer.AddFillEllipse( rr, 6, new ColorValue( 0, 0, 1 ) );
			//				}
			//			}
			//			catch { }
			//		}
			//	}
			//}
		}

		//public delegate void PreKeyDownDelegate( UIEdit sender, KeyEvent e, ref bool handled );
		//public event PreKeyDownDelegate PreKeyDown;
		////public delegate bool PreKeyUpDelegate( KeyEvent e, ref bool handled );
		////public event PreKeyUpDelegate PreKeyUp;
		//public delegate void PreKeyPressDelegate( KeyPressEvent e, ref bool handled );
		//public event PreKeyPressDelegate PreKeyPress;

		static int GetLineStartPosition( string text, int caretPosition )
		{
			if( text.Length == 0 )
				return 0;

			var caretPosition2 = MathEx.Clamp( caretPosition - 1, 0, text.Length - 1 );
			for( int n = caretPosition2; n >= 0; n-- )
			{
				if( text[ n ] == '\n' )
					return MathEx.Clamp( n + 1, 0, text.Length - 1 );
			}
			return 0;
		}

		static int GetLineLength( string text, int startLineIndex )
		{
			for( int n = startLineIndex; n < text.Length; n++ )
			{
				if( text[ n ] == '\n' )
					return n - startLineIndex;
			}
			return text.Length - startLineIndex;
		}

		static int GetLineEndPosition( string text, int caretPosition, out bool gotEnd )
		{
			gotEnd = false;
			if( text.Length == 0 )
				return 0;

			var caretPosition2 = MathEx.Clamp( caretPosition, 0, text.Length - 1 );
			for( int n = caretPosition2; n < text.Length; n++ )
			{
				if( text[ n ] == '\n' )
					return MathEx.Clamp( n, 0, text.Length - 1 );
			}
			gotEnd = true;
			return text.Length - 1;
		}

		void UpdateSelection( int oldCaret )
		{
			var caret = GetCaretPosition();

			var start = SelectionStart.Value;
			var length = SelectionLength.Value;

			if( length == 0 )
			{
				start = Math.Min( caret, oldCaret );
				length = Math.Abs( caret - oldCaret );
			}

			var min = start;
			var max = start + length;

			if( caret < min )
				min = caret;
			if( caret > max )
				max = caret;

			Select( min, max - min );
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			var textControl = GetTextControl();
			if( textControl != null )
			{
				//!!!!was before
				//if( PreKeyDown != null )
				//{
				//	bool handled = false;
				//	PreKeyDown( e, ref handled );
				//	if( handled )
				//		return true;
				//}

				if( Focused )//&& EnabledInHierarchy && !ReadOnly )
				{
					switch( e.Key )
					{
					case EKeys.Back:
						if( !ReadOnlyInHierarchy && Text != "" )
						{
							GetSelection( out var start, out var length );
							if( length != 0 )
							{
								Text = Text.Value.Remove( start, length );
								CaretPosition = start;
								DeselectAll();
							}
							else
							{
								var caret = GetCaretPosition();
								if( caret > 0 )
								{
									var newText = Text.Value.Remove( caret - 1, 1 );
									if( OnTextTypingFilter( e.Key, (char)0, newText ) )
									{
										Text = newText;
										CaretPosition = caret - 1;
									}
								}
							}

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Delete:
						if( !ReadOnlyInHierarchy && Text != "" )
						{
							GetSelection( out var start, out var length );
							if( length != 0 )
							{
								Text = Text.Value.Remove( start, length );
								CaretPosition = start;
								DeselectAll();
							}
							else
							{
								var caret = GetCaretPosition();
								if( caret < Text.Value.Length )
								{
									var newText = Text.Value.Remove( caret, 1 );
									if( OnTextTypingFilter( e.Key, (char)0, newText ) )
										Text = newText;
								}
							}

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					//it is custom
					//case EKeys.Escape:
					//	Unfocus();
					//	break;

					case EKeys.Return:
						if( !ReadOnlyInHierarchy && textControl != null && textControl.WordWrap )// Multiline )
						{
							{
								GetSelection( out var start, out var length );
								if( length != 0 )
								{
									Text = Text.Value.Remove( start, length );
									CaretPosition = start;
									DeselectAll();
								}
							}

							if( Text.Value.Length < MaxCharacterCount )
							{
								var caret = GetCaretPosition();

								var newText = Text.Value.Insert( caret, "\n" );
								if( OnTextTypingFilter( e.Key, (char)0, newText ) )
								{
									Text = newText;

									caret++;
									if( caret == Text.Value.Length )
										caret = -1;
									CaretPosition = caret;
								}
							}

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Left:
						{
							var caret = GetCaretPosition();
							var oldCaret = caret;
							caret--;
							while( caret > 0 && caret < Text.Value.Length && Text.Value[ caret ] < 32 && Text.Value[ caret ] != '\n' )
								caret--;
							if( caret < 0 )
								caret = 0;
							CaretPosition = caret;

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Right:
						{
							var caret = GetCaretPosition();
							var oldCaret = caret;
							caret++;
							while( caret < Text.Value.Length && Text.Value[ caret ] < 32 && Text.Value[ caret ] != '\n' )
								caret++;
							if( caret >= Text.Value.Length )
								caret = -1;
							CaretPosition = caret;

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Home:
						{
							int caret = GetCaretPosition();
							int oldCaret = caret;
							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
								caret = 0;
							else
								caret = GetLineStartPosition( Text.Value, caret );
							CaretPosition = caret;

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.End:
						{
							var caret = GetCaretPosition();
							var oldCaret = caret;
							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
							{
								caret = Text.Value.Length;
								CaretPosition = -1;
							}
							else
							{
								caret = GetLineEndPosition( Text.Value, caret, out var gotEnd );
								CaretPosition = gotEnd ? -1 : caret;
							}

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Up:
						{
							int caret = GetCaretPosition();
							int oldCaret = caret;

							var currentLineStart = GetLineStartPosition( Text.Value, caret );
							var currentLinePosition = caret - currentLineStart;
							var previousLineStart = GetLineStartPosition( Text.Value, currentLineStart - 1 );
							caret = previousLineStart + Math.Min( currentLinePosition, GetLineLength( Text.Value, previousLineStart ) );

							if( lastCharacterRectangles != null && lastCharacterRectangles.Length > 0 )
							{
								var oldCaret2 = MathEx.Clamp( oldCaret, 0, lastCharacterRectangles.Length - 1 );
								var oldCharacterRectangle = lastCharacterRectangles[ oldCaret2 ];
								var oldPosition = new Vector2F( oldCharacterRectangle.Left, oldCharacterRectangle.GetCenter().Y );

								float? heightOffsetFromRectangles = null;
								{
									var current = lastCharacterRectangles[ 0 ];
									for( int n = 1; n < lastCharacterRectangles.Length; n++ )
									{
										var next = lastCharacterRectangles[ n ];
										if( next.Top != current.Top )
										{
											heightOffsetFromRectangles = next.Top - current.Top;
											break;
										}
										current = next;
									}
								}

								if( heightOffsetFromRectangles != null )
								{
									var center = oldPosition + new Vector2F( 0, -Math.Abs( heightOffsetFromRectangles.Value ) );

									var characterIndex = -1;
									var minDistance = float.MaxValue;

									for( var n = 0; n < lastCharacterRectangles.Length; n++ )
									{
										var characterRectangle = lastCharacterRectangles[ n ];
										if( center.Y > characterRectangle.Top && center.Y < characterRectangle.Bottom )
										{
											var distance = Math.Abs( characterRectangle.GetCenter().X - center.X );
											if( characterIndex == -1 || distance < minDistance )
											{
												characterIndex = n;
												if( center.X > characterRectangle.GetCenter().X )
													characterIndex++;
												minDistance = distance;
											}
										}
									}

									if( characterIndex != -1 )
										caret = characterIndex;
								}
							}

							CaretPosition = caret;

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;

					case EKeys.Down:
						{
							var caret = GetCaretPosition();
							var oldCaret = caret;

							var text = Text.Value;
							var currentLineStart = GetLineStartPosition( text, caret );
							var currentLinePosition = caret - currentLineStart;
							var nextLineStart = GetLineEndPosition( text, currentLineStart + 1, out _ );
							while( nextLineStart < text.Length && text[ nextLineStart ] == '\n' )
								nextLineStart++;
							caret = nextLineStart + Math.Min( currentLinePosition, GetLineLength( Text.Value, nextLineStart ) );

							if( lastCharacterRectangles != null && lastCharacterRectangles.Length > 0 )
							{
								var oldCaret2 = MathEx.Clamp( oldCaret, 0, lastCharacterRectangles.Length - 1 );
								var oldCharacterRectangle = lastCharacterRectangles[ oldCaret2 ];
								var oldPosition = new Vector2F( oldCharacterRectangle.Left, oldCharacterRectangle.GetCenter().Y );

								float? heightOffsetFromRectangles = null;
								{
									var current = lastCharacterRectangles[ 0 ];
									for( int n = 1; n < lastCharacterRectangles.Length; n++ )
									{
										var next = lastCharacterRectangles[ n ];
										if( next.Top != current.Top )
										{
											heightOffsetFromRectangles = next.Top - current.Top;
											break;
										}
										current = next;
									}
								}

								if( heightOffsetFromRectangles != null )
								{
									var center = oldPosition + new Vector2F( 0, Math.Abs( heightOffsetFromRectangles.Value ) );

									var characterIndex = -1;
									var minDistance = float.MaxValue;

									for( var n = 0; n < lastCharacterRectangles.Length; n++ )
									{
										var characterRectangle = lastCharacterRectangles[ n ];
										if( center.Y > characterRectangle.Top && center.Y < characterRectangle.Bottom )
										{
											var distance = Math.Abs( characterRectangle.GetCenter().X - center.X );
											if( characterIndex == -1 || distance < minDistance )
											{
												characterIndex = n;
												if( center.X > characterRectangle.GetCenter().X )
													characterIndex++;
												minDistance = distance;
											}
										}
									}

									if( characterIndex != -1 )
										caret = characterIndex;
								}
							}

							CaretPosition = caret >= text.Length ? -1 : caret;

							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Shift ) )
								UpdateSelection( oldCaret );
							else
								DeselectAll();

							EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
						}
						return true;
					}

					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					{
						switch( e.Key )
						{
						case EKeys.X:
							if( !ReadOnlyInHierarchy && ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
							{
								try
								{
									GetSelection( out var start, out var length );
									if( length != 0 )
									{
										PlatformSpecificUtility.Instance.SetClipboardText( SelectedText );
										//System.Windows.Forms.Clipboard.SetText( SelectedText );

										Text = Text.Value.Remove( start, length );
										CaretPosition = start;
										DeselectAll();
									}
								}
								catch { }

								DeselectAll();
								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
								return true;
							}
							break;

						case EKeys.C:
							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
							{
								try
								{
									GetSelection( out var start, out var length );
									if( length != 0 )
									{
										PlatformSpecificUtility.Instance.SetClipboardText( SelectedText );
										//System.Windows.Forms.Clipboard.SetText( SelectedText );
									}
								}
								catch { }

								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
								return true;
							}
							break;

						case EKeys.V:
							if( !ReadOnlyInHierarchy && ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
							{
								GetSelection( out var start, out var length );
								if( length != 0 )
								{
									Text = Text.Value.Remove( start, length );
									CaretPosition = start;
									DeselectAll();
								}

								try
								{
									var text = PlatformSpecificUtility.Instance.GetClipboardText();
									//var text = System.Windows.Forms.Clipboard.GetText();
									if( !string.IsNullOrEmpty( text ) )
									{
										var font = GetFont();

										var caret = GetCaretPosition();

										var newText = Text.Value;

										foreach( var c in text )
										{
											if( IsAllowCharacter( font, textControl, c ) && OnTextTypingFilter( e.Key, c, newText ) )
											{
												newText = newText.Insert( caret, c.ToString() );
												caret++;
											}
										}

										Text = newText;

										if( caret == Text.Value.Length )
											caret = -1;
										CaretPosition = caret;
									}
								}
								catch { }

								DeselectAll();
								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
								return true;
							}
							break;

						case EKeys.A:
							if( ParentContainer.Viewport.IsKeyPressed( EKeys.Control ) )
							{
								SelectAll();
								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
								return true;
							}
							break;
						}
					}

					//!!!!other platforms specific

				}
			}

			return base.OnKeyDown( e );
		}

		//protected override bool OnKeyUp( KeyEvent e )
		//{
		//   if( PreKeyUp != null )
		//   {
		//      bool handled;
		//      PreKeyUp( e, ref handled );
		//      if( handled )
		//         return true;
		//   }

		//   return base.OnKeyUp( e );
		//}

		FontComponent GetFont()
		{
			var textControl = GetTextControl();
			if( textControl != null )
			{
				var font = textControl.Font.Value;
				if( font == null )
				{
					//!!!!!!right?
					if( ParentContainer != null && ParentContainer.Viewport.CanvasRenderer != null )
						font = ParentContainer.Viewport.CanvasRenderer.DefaultFont;
					//if( EngineApp.Instance.ScreenGuiRenderer != null )
					//   font = EngineApp.Instance.ScreenGuiRenderer.DefaultFont;
				}

				return font;
			}

			return null;
		}

		static bool IsAllowCharacter( FontComponent font, UIText textControl, char c )
		{
			if( c == '\n' && textControl.WordWrap )
				return true;

			if( font != null )
				return c >= 32 && font.IsCharacterInitialized( c );
			else
				return c >= 32 && c < 128;
		}

		protected override bool OnKeyPress( KeyPressEvent e )
		{
			var textControl = GetTextControl();
			if( textControl != null )
			{
				//!!!!was before
				//if( PreKeyPress != null )
				//{
				//	bool handled = false;
				//	PreKeyPress( e, ref handled );
				//	if( handled )
				//		return true;
				//}

				if( Focused && EnabledInHierarchy && !ReadOnlyInHierarchy )
				{
					if( Text.Value.Length < MaxCharacterCount )
					{
						var font = GetFont();
						var allowCharacter = IsAllowCharacter( font, textControl, e.KeyChar );

						//bool allowCharacter;
						//if( font != null )
						//	allowCharacter = e.KeyChar >= 32 && font.IsCharacterInitialized( e.KeyChar );
						//else
						//	allowCharacter = e.KeyChar >= 32 && e.KeyChar < 128;

						if( allowCharacter )
						{
							{
								GetSelection( out var start, out var length );
								if( length != 0 )
								{
									Text = Text.Value.Remove( start, length );
									CaretPosition = start;
									DeselectAll();
								}
							}

							var newText = Text.Value;
							newText = newText.Insert( GetCaretPosition(), e.KeyChar.ToString() );
							//string newText = Text + e.KeyChar.ToString();

							if( OnTextTypingFilter( 0, e.KeyChar, newText ) )
							{
								Text = newText;
								CaretPosition = GetCaretPosition() + 1;
								DeselectAll();
								EditingOrChangeCaretPositionLastTime = EngineApp.EngineTime;
							}
						}
					}

					return true;
				}
			}

			return base.OnKeyPress( e );
		}

		//!!!!
		//protected override UIControl.StandardChildSlotItem[] OnGetStandardChildSlots()
		//{
		//	UIControl.StandardChildSlotItem[] array = new StandardChildSlotItem[ 1 ];
		//	array[ 0 ] = new StandardChildSlotItem( "TextControl", TextControl );
		//	return array;
		//}

		//!!!!was before

		//protected override void OnComponentRemoved( Component component )
		//{
		//	base.OnComponentRemoved( component );

		//	if( component == TextControl )
		//		TextControl = null;
		//}

		///// <summary>
		///// Called when edit box is updating text control.
		///// </summary>
		//[Browsable( false )]
		//public UpdatingTextControlDelegate UpdatingTextControl
		//{
		//	get { return updatingTextControl; }
		//	set { updatingTextControl = value; }
		//}

		//protected virtual void OnUpdatingTextControl( ref string text )
		//{
		//	UpdatingTextControl?.Invoke( this, ref text );
		//}

		//[Browsable( false )]
		//public TextTypingFilterDelegate TextTypingFilter
		//{
		//	get { return textTypingFilter; }
		//	set { textTypingFilter = value; }
		//}

		//!!!!
		bool OnTextTypingFilter( EKeys key, char character, string newText )
		{
			return true;
		}
		//protected virtual bool OnTextTypingFilter( EKeys key, char character, string newText )
		//{
		//	if( TextTypingFilter != null )
		//	{
		//		if( !TextTypingFilter( this, key, character, newText ) )
		//			return false;
		//	}
		//	return true;
		//}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			var obj = CreateComponent<UIText>();
			obj.Name = "Text";
			obj.Text = ReferenceUtility.MakeThisReference( obj, this, "DisplayText" );//"Text" );
			obj.TextHorizontalAlignment = EHorizontalAlignment.Left;
			//obj.TextHorizontalAlignment = ReferenceUtility.MakeThisReference( obj, this, "TextHorizontalAlignment" );
			obj.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );
			obj.CanBeSelected = false;
			obj.HorizontalAlignment = EHorizontalAlignment.Stretch;
			obj.VerticalAlignment = EVerticalAlignment.Stretch;
			//obj.Offset = new UIMeasureValueVector2( UIMeasure.Units, 4, 0 );
			obj.ClipRectangle = true;

			//UIStyle.EditTextMargin
			obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 4, 2, 4, 2 );
			//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );
		}

		public UIText GetTextControl()
		{
			return GetComponentByPath( "Text" ) as UIText;
		}

		[Browsable( false )]
		public virtual string DisplayText
		{
			get
			{
				string text = Text;
				if( PasswordCharacter.Value != '\0' )
					text = new string( PasswordCharacter.Value, text.Length );

				if( Focused && !ReadOnlyInHierarchy && CaretPosition == -1 )
					text += " ";
				//if( Focused && !ReadOnlyInHierarchy )
				//	text += " ";

				if( !Focused && string.IsNullOrEmpty( text ) )
					text = HintWhenEmpty;

				//OnUpdatingTextControl( ref text );
				return text;
			}
		}

		//[Browsable( false )]
		//public EHorizontalAlignment TextHorizontalAlignment
		//{
		//	get { return textHorizontalAlignment; }
		//}

		public void GetSelection( out int start, out int length )
		{
			start = SelectionStart.Value;
			length = SelectionLength.Value;

			if( start < 0 )
				start = 0;
			if( length < 0 )
				length = 0;

			var text = Text.Value;
			if( start + length > text.Length )
				length = text.Length - start;
		}

		[Browsable( false )]
		public string SelectedText
		{
			get
			{
				GetSelection( out var start, out var length );
				return Text.Value.Substring( start, length );
			}
		}

		public int GetCaretPosition()
		{
			var text = Text.Value;
			var caret = CaretPosition.Value;
			if( caret < 0 )
				caret = text.Length;
			if( caret > text.Length )
				caret = text.Length;
			return caret;
		}

		public void Select( int start, int length )
		{
			if( start < 0 )
				start = 0;
			if( length < 0 )
				length = 0;

			var text = Text.Value;
			if( start + length > text.Length )
				length = text.Length - start;

			SelectionStart = start;
			SelectionLength = length;
		}

		public void SelectAll()
		{
			var text = Text.Value;
			SelectionStart = 0;
			SelectionLength = text.Length;
		}

		public void DeselectAll()
		{
			Select( 0, 0 );
		}

		protected override bool OnTouch( TouchData e )
		{
			//!!!!положение курсора выставлять куда нажали

			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( VisibleInHierarchy && EnabledInHierarchy && !ReadOnlyInHierarchy )
				{
					if( ParentContainer != null && ParentContainer.IsControlCoveredByOther( this, e.Position ) )
						break;

					GetScreenRectangle( out var rect );
					var rectInPixels = rect * ParentContainer.Viewport.SizeInPixels.ToVector2();
					var distanceInPixels = rectInPixels.GetPointDistance( e.PositionInPixels.ToVector2() );

					var item = new TouchData.TouchDownRequestToProcessTouch( this, 0, distanceInPixels, null,
						delegate ( UIControl sender, TouchData touchData, object anyData )
						{
							//focus
							if( CanFocus )
							{
								bool intoArea = rect.Contains( e.Position );
								if( intoArea && EnabledInHierarchy )
								{
									Focus();
									//return true;
								}
								else
									Unfocus();
							}

						} );
					e.TouchDownRequestToControlActions.Add( item );
				}
				break;
			}

			return base.OnTouch( e );
		}

	}
}
