// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Standard UI element for text input.
	/// </summary>
	public class UIEdit : UIControl
	{
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
				if( _maxCharacterCount.BeginSet( ref value ) )
				{
					try
					{
						MaxCharacterCountChanged?.Invoke( this );

						//!!!!check
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

		///////////////////////////////////////////

		//!!!!было

		//public delegate void UpdatingTextControlDelegate( UIEdit sender, ref string text );
		//UpdatingTextControlDelegate updatingTextControl;

		//public delegate bool TextTypingFilterDelegate( UIEdit sender, EKeys key, char character, string newText );
		//TextTypingFilterDelegate textTypingFilter;

		///////////////////////////////////////////

		public UIEdit()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 40 );

			TextChanged += UIEdit_TextChanged;
		}

		private void UIEdit_TextChanged( UIControl obj )
		{
			var v = Text;
			if( v.Value.Length > MaxCharacterCount )
				Text = new Reference<string>( v.Value.Substring( 0, MaxCharacterCount ), v.GetByReference );

			//var text = Text.Value;
			//if( text.Length > MaxCharacterCount )
			//	text = text.Substring( 0, MaxCharacterCount );
			//Text = text;
		}

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
		/// Whether focus on the edit box is allowed.
		/// </summary>
		[Browsable( false )]
		public override bool CanFocus
		{
			get { return EnabledInHierarchy; }
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			if( !ReadOnly )
			{
				bool intoArea = new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition );

				if( intoArea && EnabledInHierarchy )
				{
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

			var textControl = GetTextControl();
			if( textControl != null )
			{
				//!!!!
				//textControl.SupportLocalization = false;

				//textControl.ScreenClipRectangle = textControl.GetScreenRectangle();

				//string text = Text;
				//if( Focused )
				//	text += "_";
				//OnUpdatingTextControl( ref text );
				//textControl.Text = text;

				//update textHorizontalAlignment
				{
					//!!!!не всегда нужно менять. может стиль какой-нибудь особый

					EHorizontalAlignment horizontalAlign = EHorizontalAlignment.Left;

					if( Focused )
					{
						var font = textControl.Font.Value;
						if( font == null )
							font = renderer.DefaultFont;
						if( font != null )
						{
							var fontSize = textControl.GetFontSizeScreen();

							float textLength = font.GetTextLength( fontSize, renderer, textControl.Text );
							if( textLength > textControl.GetScreenSize().X )
								horizontalAlign = EHorizontalAlignment.Right;
						}
					}

					textControl.TextHorizontalAlignment = horizontalAlign;
				}
			}
		}

		//!!!!было

		//public delegate void PreKeyDownDelegate( UIEdit sender, KeyEvent e, ref bool handled );
		//public event PreKeyDownDelegate PreKeyDown;
		////public delegate bool PreKeyUpDelegate( KeyEvent e, ref bool handled );
		////public event PreKeyUpDelegate PreKeyUp;
		//public delegate void PreKeyPressDelegate( KeyPressEvent e, ref bool handled );
		//public event PreKeyPressDelegate PreKeyPress;

		protected override bool OnKeyDown( KeyEvent e )
		{
			var textControl = GetTextControl();
			if( textControl != null )
			{
				//!!!!было
				//if( PreKeyDown != null )
				//{
				//	bool handled = false;
				//	PreKeyDown( e, ref handled );
				//	if( handled )
				//		return true;
				//}

				if( Focused && EnabledInHierarchy )
				{
					switch( e.Key )
					{
					case EKeys.Back:
						if( Text != "" )
						{
							string newText = Text.Value.Substring( 0, Text.Value.Length - 1 );
							if( OnTextTypingFilter( e.Key, (char)0, newText ) )
								Text = newText;
							//Text = Text.Substring( 0, Text.Length - 1 );
						}
						break;

					case EKeys.Escape:
						Unfocus();
						break;

					case EKeys.Return:
						if( textControl != null && textControl.WordWrap )
						{
							if( Text.Value.Length < MaxCharacterCount )
							{
								string newText = Text + "\n";
								if( OnTextTypingFilter( e.Key, (char)0, newText ) )
									Text = newText;
								//Text += "\n";
							}
						}
						break;
					}

					return true;
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

		protected override bool OnKeyPress( KeyPressEvent e )
		{
			var textControl = GetTextControl();
			if( textControl != null )
			{
				//!!!!было
				//if( PreKeyPress != null )
				//{
				//	bool handled = false;
				//	PreKeyPress( e, ref handled );
				//	if( handled )
				//		return true;
				//}

				if( Focused && EnabledInHierarchy )
				{
					if( Text.Value.Length < MaxCharacterCount )
					{
						var font = textControl.Font.Value;
						if( font == null )
						{
							//!!!!!!так правильно?
							if( ParentContainer != null && ParentContainer.Viewport.CanvasRenderer != null )
								font = ParentContainer.Viewport.CanvasRenderer.DefaultFont;
							//if( EngineApp.Instance.ScreenGuiRenderer != null )
							//   font = EngineApp.Instance.ScreenGuiRenderer.DefaultFont;
						}

						bool allowCharacter;
						if( font != null )
							allowCharacter = e.KeyChar >= 32 && font.IsCharacterInitialized( e.KeyChar );
						else
							allowCharacter = e.KeyChar >= 32 && e.KeyChar < 128;

						if( allowCharacter )
						{
							string newText = Text + e.KeyChar.ToString();
							if( OnTextTypingFilter( 0, e.KeyChar, newText ) )
								Text = newText;
							//Text += e.KeyChar.ToString();
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

		//!!!!было

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
		public string DisplayText
		{
			get
			{
				string text = Text;
				if( Focused && !ReadOnly )
					text += "_";
				//OnUpdatingTextControl( ref text );
				return text;
			}
		}

		//[Browsable( false )]
		//public EHorizontalAlignment TextHorizontalAlignment
		//{
		//	get { return textHorizontalAlignment; }
		//}
	}
}
