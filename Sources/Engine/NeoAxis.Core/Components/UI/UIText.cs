// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Standard UI element for text drawing.
	/// </summary>
	public class UIText : UIControl
	{
		//!!!!может Font в UIControl?

		/// <summary>
		/// The font of rendered text.
		/// </summary>
		[DefaultValue( null )]
		public Reference<FontComponent> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( this, ref value ) ) { try { FontChanged?.Invoke( this ); } finally { _font.EndSet(); } } }
		}
		public event Action<UIText> FontChanged;
		ReferenceField<FontComponent> _font = null;

		/// <summary>
		/// Font size of rendered text.
		/// </summary>
		[DefaultValue( "Screen 0.02" )]
		public Reference<UIMeasureValueDouble> FontSize
		{
			get { if( _fontSize.BeginGet() ) FontSize = _fontSize.Get( this ); return _fontSize.value; }
			set { if( _fontSize.BeginSet( this, ref value ) ) { try { FontSizeChanged?.Invoke( this ); } finally { _fontSize.EndSet(); } } }
		}
		public event Action<UIText> FontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _fontSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.02 );

		/// <summary>
		/// The horizontal alignment of the text.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Serialize]
		[Category( "Text" )]
		public Reference<EHorizontalAlignment> TextHorizontalAlignment
		{
			get { if( _textHorizontalAlignment.BeginGet() ) TextHorizontalAlignment = _textHorizontalAlignment.Get( this ); return _textHorizontalAlignment.value; }
			set { if( _textHorizontalAlignment.BeginSet( this, ref value ) ) { try { TextHorizontalAlignChanged?.Invoke( this ); } finally { _textHorizontalAlignment.EndSet(); } } }
		}
		public event Action<UIText> TextHorizontalAlignChanged;
		ReferenceField<EHorizontalAlignment> _textHorizontalAlignment = EHorizontalAlignment.Center;

		/// <summary>
		/// The vertical alignment of the text.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Center )]
		[Serialize]
		[Category( "Text" )]
		public Reference<EVerticalAlignment> TextVerticalAlignment
		{
			get { if( _textVerticalAlignment.BeginGet() ) TextVerticalAlignment = _textVerticalAlignment.Get( this ); return _textVerticalAlignment.value; }
			set { if( _textVerticalAlignment.BeginSet( this, ref value ) ) { try { TextVerticalAlignChanged?.Invoke( this ); } finally { _textVerticalAlignment.EndSet(); } } }
		}
		public event Action<UIText> TextVerticalAlignChanged;
		ReferenceField<EVerticalAlignment> _textVerticalAlignment = EVerticalAlignment.Center;

		/// <summary>
		/// The color of the text.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		[Category( "Text" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		public event Action<UIText> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// Extra offset added to the text position.
		/// </summary>
		[DefaultValue( "Units 0 0" )]
		[Serialize]
		[Category( "Text" )]
		public Reference<UIMeasureValueVector2> Offset
		{
			get { if( _offset.BeginGet() ) Offset = _offset.Get( this ); return _offset.value; }
			set { if( _offset.BeginSet( this, ref value ) ) { try { OffsetChanged?.Invoke( this ); } finally { _offset.EndSet(); } } }
		}
		public event Action<UIText> OffsetChanged;
		ReferenceField<UIMeasureValueVector2> _offset = new UIMeasureValueVector2( UIMeasure.Units, 0, 0 );

		/// <summary>
		/// Wordwrap the text to fit within the width of the control.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Text" )]
		public Reference<bool> WordWrap
		{
			get { if( _wordWrap.BeginGet() ) WordWrap = _wordWrap.Get( this ); return _wordWrap.value; }
			set { if( _wordWrap.BeginSet( this, ref value ) ) { try { WordWrapChanged?.Invoke( this ); } finally { _wordWrap.EndSet(); } } }
		}
		public event Action<UIText> WordWrapChanged;
		ReferenceField<bool> _wordWrap = false;

		/// <summary>
		/// Vertical space between the margin of control and the start of text.
		/// </summary>
		[DefaultValue( "Units 0" )]
		[Serialize]
		[Category( "Text" )]
		public Reference<UIMeasureValueDouble> VerticalIndention
		{
			get { if( _verticalIndention.BeginGet() ) VerticalIndention = _verticalIndention.Get( this ); return _verticalIndention.value; }
			set { if( _verticalIndention.BeginSet( this, ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
		}
		public event Action<UIText> VerticalIndentionChanged;
		ReferenceField<UIMeasureValueDouble> _verticalIndention = new UIMeasureValueDouble( UIMeasure.Units, 0 );

		////!!!!в каких единицах указывается?
		///// <summary>
		///// Vertical space between the margin of control and the start of text.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[Category( "Text" )]
		//public Reference<double> VerticalIndention
		//{
		//	get { if( _verticalIndention.BeginGet() ) VerticalIndention = _verticalIndention.Get( this ); return _verticalIndention.value; }
		//	set { if( _verticalIndention.BeginSet( this, ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
		//}
		//public event Action<UIText> VerticalIndentionChanged;
		//ReferenceField<double> _verticalIndention = 0.0;

		///// <summary>
		///// Automatically adjust the size of control to the length of the text.
		///// </summary>
		//[DefaultValue( false )]
		//[Serialize]
		//[Category( "Text" )]
		//public Reference<bool> AutoSize
		//{
		//	get { if( _autoSize.BeginGet() ) AutoSize = _autoSize.Get( this ); return _autoSize.value; }
		//	set { if( _autoSize.BeginSet( this, ref value ) ) { try { AutoSizeChanged?.Invoke( this ); } finally { _autoSize.EndSet(); } } }
		//}
		//public event Action<UIText> AutoSizeChanged;
		//ReferenceField<bool> _autoSize = false;

		/// <summary>
		/// Draw the shadow behind the text.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Text" )]
		public Reference<bool> Shadow
		{
			get { if( _shadow.BeginGet() ) Shadow = _shadow.Get( this ); return _shadow.value; }
			set { if( _shadow.BeginSet( this, ref value ) ) { try { ShadowChanged?.Invoke( this ); } finally { _shadow.EndSet(); } } }
		}
		public event Action<UIText> ShadowChanged;
		ReferenceField<bool> _shadow = false;

		/// <summary>
		/// Extra offset added to the text shadow position.
		/// </summary>
		[DefaultValue( "Units 1 1" )]
		[Serialize]
		[Category( "Text" )]
		public Reference<UIMeasureValueVector2> ShadowOffset
		{
			get { if( _shadowOffset.BeginGet() ) ShadowOffset = _shadowOffset.Get( this ); return _shadowOffset.value; }
			set { if( _shadowOffset.BeginSet( this, ref value ) ) { try { ShadowOffsetChanged?.Invoke( this ); } finally { _shadowOffset.EndSet(); } } }
		}
		public event Action<UIText> ShadowOffsetChanged;
		ReferenceField<UIMeasureValueVector2> _shadowOffset = new UIMeasureValueVector2( UIMeasure.Units, 1, 1 );

		/// <summary>
		/// The color of the text shadow.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Serialize]
		[Category( "Text" )]
		public Reference<ColorValue> ShadowColor
		{
			get { if( _shadowColor.BeginGet() ) ShadowColor = _shadowColor.Get( this ); return _shadowColor.value; }
			set { if( _shadowColor.BeginSet( this, ref value ) ) { try { ShadowColorChanged?.Invoke( this ); } finally { _shadowColor.EndSet(); } } }
		}
		public event Action<UIText> ShadowColorChanged;
		ReferenceField<ColorValue> _shadowColor = new ColorValue( 0, 0, 0 );

		//!!!!
		///// <summary>
		///// Does the text support localization.
		///// </summary>
		//[DefaultValue( true )]
		//[Serialize]
		//[Category( "Text" )]
		//public Reference<bool> SupportLocalization
		//{
		//	get { if( _supportLocalization.BeginGet() ) SupportLocalization = _supportLocalization.Get( this ); return _supportLocalization.value; }
		//	set { if( _supportLocalization.BeginSet( this, ref value ) ) { try { SupportLocalizationChanged?.Invoke( this ); } finally { _supportLocalization.EndSet(); } } }
		//}
		//public event Action<UIText> SupportLocalizationChanged;
		//ReferenceField<bool> _supportLocalization = true;

		//!!!!тут?
		/// <summary>
		/// Restrict text to a rectangular region.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Text" )]
		public Reference<bool> ClipRectangle
		{
			get { if( _clipRectangle.BeginGet() ) ClipRectangle = _clipRectangle.Get( this ); return _clipRectangle.value; }
			set { if( _clipRectangle.BeginSet( this, ref value ) ) { try { ClipRectangleChanged?.Invoke( this ); } finally { _clipRectangle.EndSet(); } } }
		}
		public event Action<UIText> ClipRectangleChanged;
		ReferenceField<bool> _clipRectangle = false;

		/////////////////////////////////////////

		public UIText()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 30 );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( ShadowOffset ):
				case nameof( ShadowColor ):
					if( !Shadow )
						skip = true;
					break;
				}
			}
		}

		protected override void OnBeforeRenderUIWithChildren( CanvasRenderer renderer )
		{
			if( ClipRectangle )
				ScreenClipRectangle = GetScreenRectangle();
			else
				ScreenClipRectangle = null;

			base.OnBeforeRenderUIWithChildren( renderer );
		}

		//protected override void OnRenderUI( CanvasRenderer renderer )
		//{
		//	base.OnRenderUI( renderer );

		//	string localizedText;
		//	//!!!!
		//	//if( SupportLocalization )
		//	//	localizedText = LanguageManager.Instance.Translate( "UISystem", Text );
		//	//else
		//	localizedText = Text;

		//	//!!!!было
		//	//if( AutoSize )
		//	//	UpdateAutoSize( renderer, localizedText );

		//	if( !string.IsNullOrEmpty( localizedText ) )
		//	{
		//		if( !WordWrap )
		//			NoWordWrapRenderUI( renderer, localizedText );
		//		else
		//			WordWrapRenderUI( renderer, localizedText );
		//	}
		//}

		public class RenderParentEditData
		{
			public UIEdit Edit;
			public ColorValue SelectionColor;
			public ColorValue CaretColor;
		}

		public void RenderDefaultStyle( CanvasRenderer renderer, RenderParentEditData parentEditData )
		{
			string text;
			//!!!!
			//if( SupportLocalization )
			//	localizedText = LanguageManager.Instance.Translate( "UISystem", Text );
			//else
			text = Text;

			//!!!!было
			//if( AutoSize )
			//	UpdateAutoSize( renderer, localizedText );

			//if( !string.IsNullOrEmpty( text ) )
			{
				if( !WordWrap || string.IsNullOrEmpty( text ) )
					NoWordWrapRenderUI( renderer, parentEditData, text );
				else
					WordWrapRenderUI( renderer, parentEditData, text );
			}
		}

		//int GetTextLineCount( string text )
		//{
		//	if( string.IsNullOrEmpty( text ) )
		//		return 0;

		//	int result = 1;
		//	foreach( char c in text )
		//	{
		//		if( c == '\n' )
		//			result++;
		//	}
		//	return result;
		//}

		//!!!!было
		//void UpdateAutoSize( CanvasRenderer renderer, string localizedText )
		//{
		//if( WordWrap )
		//	return;

		//EngineFont f = font;
		//if( f == null )
		//	f = renderer.DefaultFont;

		//double length = f.GetTextLength( renderer, localizedText );
		//double height = f.Height * GetTextLineCount( localizedText );
		//Size = new ScaleValue( ScaleType.Screen, new Vec2( length, height ) );
		//}

		/////////////////////////////////////////

		struct CharacterItem
		{
			public char character;
			public int index;
			public double width;

			public CharacterItem( char character, int index, double width )
			{
				this.character = character;
				this.index = index;
				this.width = width;
			}
		}

		/////////////////////////////////////////

		class TextItem
		{
			public List<CharacterItem> characters;

			public TextItem( int capacity )
			{
				characters = new List<CharacterItem>( capacity );
			}

			public TextItem( List<CharacterItem> characters )
			{
				this.characters = characters;
			}

			public void Add( CharacterItem item )
			{
				characters.Add( item );
			}

			public void AddRange( ICollection<CharacterItem> items )
			{
				characters.AddRange( items );
			}

			public void AddRange( TextItem text )
			{
				characters.AddRange( text.characters );
			}

			public int Count
			{
				get { return characters.Count; }
			}

			public void Clear()
			{
				characters.Clear();
			}

			//!!!!slowly несколько раз вызывать?
			public string GetText()
			{
				var builder = new StringBuilder( characters.Count + 1 );
				foreach( var item in characters )
					builder.Append( item.character );
				return builder.ToString();
			}

			//!!!!slowly несколько раз вызывать?
			public double GetWidth()
			{
				var result = 0.0;
				foreach( var item in characters )
					result += item.width;
				return result;
			}
		}

		/////////////////////////////////////////

		struct LineItem
		{
			public TextItem text;
			public bool alignByWidth;

			//

			public LineItem( TextItem text, bool alignByWidth )
			{
				this.text = text;
				this.alignByWidth = alignByWidth;
			}

			public LineItem( List<TextItem> texts, bool alignByWidth )
			{
				if( texts.Count > 1 )
				{
					text = texts[ 0 ];
					for( int n = 1; n < texts.Count; n++ )
						text.AddRange( texts[ n ] );
				}
				else if( texts.Count == 1 )
					text = texts[ 0 ];
				else
					text = new TextItem( 4 );

				this.alignByWidth = alignByWidth;
			}
		}

		/////////////////////////////////////////

		void RenderEditSelectionAndCursor( CanvasRenderer renderer, RenderParentEditData parentEditData, TextItem text, double fontSize, Vector2 screenPos )
		{
			var font = Font.Value ?? renderer.DefaultFont;

			var start = 0;
			var length = 0;
			if( parentEditData.Edit.Focused )
				parentEditData.Edit.GetSelection( out start, out length );
			var caret = parentEditData.Edit.GetCaretPosition();

			var pixelSize = renderer.ViewportForScreenCanvasRenderer != null ? 1.0 / renderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2() : Vector2.Zero;

			var posX = screenPos.X;

			for( var n = 0; n < text.Count; n++ )
			{
				var c = text.characters[ n ].character;
				var characterIndex = text.characters[ n ].index;

				//render selection
				float textLength = 0;
				if( c != '\0' )
				{
					textLength = font.GetCharacterWidth( fontSize, renderer, c );

					if( characterIndex >= start && characterIndex < start + length )
					{
						var r = new Rectangle( posX + pixelSize.X, screenPos.Y, posX + textLength + pixelSize.X, screenPos.Y + fontSize );
						r.Bottom += pixelSize.Y;

						renderer.AddQuad( r, parentEditData.SelectionColor );
					}
				}

				//render caret
				if( parentEditData.Edit.Focused && caret == characterIndex )
				{
					var width = fontSize / 25;
					if( width < pixelSize.X )
						width = pixelSize.X;

					var r = new Rectangle( posX, screenPos.Y, posX + width, screenPos.Y + fontSize );
					r.Expand( new Vector2( 0, pixelSize.Y ) );

					renderer.AddQuad( r, parentEditData.CaretColor );
				}

				posX += textLength;
			}
		}

		static int GetTextLineCount( string text )
		{
			int result = 1;
			foreach( char c in text )
			{
				if( c == '\n' )
					result++;
			}
			return result;
		}

		void NoWordWrapRenderUI( CanvasRenderer renderer, RenderParentEditData parentEditData, string text )
		{
			var horizontalAlign = TextHorizontalAlignment.Value;
			var verticalAlign = TextVerticalAlignment.Value;

			Vector2 localPos = GetLocalOffsetByValue( Offset );

			switch( horizontalAlign )
			{
			//case EHorizontalAlignment.Left: localPos.X += 0; break;
			case EHorizontalAlignment.Center: localPos.X += .5f; break;
			case EHorizontalAlignment.Right: localPos.X += 1; break;
			}

			switch( verticalAlign )
			{
			//case EVerticalAlignment.Top: localPos.Y += 0; break;
			case EVerticalAlignment.Center: localPos.Y += .5f; break;
			case EVerticalAlignment.Bottom: localPos.Y += 1; break;
			}

			var fontSize = GetFontSizeScreen();

			var screenPos = ConvertLocalToScreen( localPos );

			//UIEdit: change screen position by caret
			if( parentEditData != null && parentEditData.Edit.Focused && ( horizontalAlign == EHorizontalAlignment.Left || horizontalAlign == EHorizontalAlignment.Right ) )
			{
				var font = Font.Value ?? renderer.DefaultFont;

				try
				{
					var caret = parentEditData.Edit.GetCaretPosition();

					if( horizontalAlign == EHorizontalAlignment.Left )
					{
						var textToSeeCaret = text.Substring( 0, caret ) + " ";
						float textWidthToSeeCaret = font.GetTextLength( fontSize, renderer, textToSeeCaret );

						GetScreenSize( out var screenSize );
						if( textWidthToSeeCaret > screenSize.X )
							screenPos.X -= textWidthToSeeCaret - screenSize.X;
					}

					if( horizontalAlign == EHorizontalAlignment.Right )
					{
						var textToSeeCaret = text.Substring( caret ) + " ";
						float textWidthToSeeCaret = font.GetTextLength( fontSize, renderer, textToSeeCaret );

						GetScreenSize( out var screenSize );
						if( textWidthToSeeCaret > screenSize.X )
							screenPos.X += textWidthToSeeCaret - screenSize.X;
					}

				}
				catch { }
			}


			if( Shadow )
			{
				ColorValue color = ShadowColor.Value.GetSaturate();
				renderer.AddText( Font, fontSize, text, screenPos + GetScreenOffsetByValue( ShadowOffset ), horizontalAlign, verticalAlign, color );
				//renderer.AddText( Font, fontSize, text, ConvertLocalToScreen( localPos + GetLocalOffsetByValue( ShadowOffset ) ), TextHorizontalAlignment, TextVerticalAlignment, color );
			}

			{
				//var screenPos = ConvertLocalToScreen( localPos );

				ColorValue color = Color.Value.GetSaturate();
				renderer.AddText( Font, fontSize, text, screenPos, horizontalAlign, verticalAlign, color );


				//selection, caret of parent UIEdit
				if( parentEditData != null && ( parentEditData.Edit.SelectionLength.Value > 0 || parentEditData.Edit.Focused ) )
				{
					var font = Font.Value ?? renderer.DefaultFont;

					Vector2F startPos = screenPos.ToVector2F();

					if( horizontalAlign == EHorizontalAlignment.Center )
						startPos.X -= font.GetTextLength( fontSize, renderer, text ) * .5f;
					else if( horizontalAlign == EHorizontalAlignment.Right )
						startPos.X -= font.GetTextLength( fontSize, renderer, text );

					if( verticalAlign == EVerticalAlignment.Center )
						startPos.Y -= (float)fontSize * .5f * (float)GetTextLineCount( text );
					else if( verticalAlign == EVerticalAlignment.Bottom )
						startPos.Y -= (float)fontSize * (float)GetTextLineCount( text );


					var textItem = new TextItem( text.Length + 1 );
					for( int n = 0; n < text.Length; n++ )
						textItem.Add( new CharacterItem( text[ n ], n, 0 ) );

					//add caret to end
					var caret = parentEditData.Edit.GetCaretPosition();
					if( caret == textItem.Count )
						textItem.Add( new CharacterItem( '\0', caret, 0 ) );


					RenderEditSelectionAndCursor( renderer, parentEditData, textItem, fontSize, startPos );
				}
			}
		}

		//static TextItem Trim( TextItem text )
		//{
		//	int start;
		//	for( start = 0; start < text.characters.Count; start++ )
		//	{
		//		if( text.characters[ start ].character != ' ' )
		//			break;
		//	}

		//	int end;
		//	for( end = text.characters.Count - 1; end >= 0; end-- )
		//	{
		//		if( text.characters[ end ].character != ' ' )
		//			break;
		//	}

		//	var length = end - start + 1;
		//	if( length != 0 )
		//		return new TextItem( text.characters.GetRange( start, length ) );
		//	else
		//		return new TextItem( 4 );
		//}

		static List<TextItem> Split( TextItem text, char splitCharacter )
		{
			var result = new List<TextItem>( 32 );

			var current = new TextItem( 32 );

			foreach( var item in text.characters )
			{
				current.Add( item );

				if( item.character == splitCharacter )
				{
					result.Add( current );
					current = new TextItem( 32 );
				}

				//if( item.character == splitCharacter )
				//{
				//	var c = Trim( current );
				//	if( c.Count != 0 )
				//		result.Add( c );
				//	current.Clear();
				//}
				//else
				//	current.Add( item );
			}

			{
				if( current.Count != 0 )
					result.Add( current );

				//var c = Trim( current );
				//if( c.Count != 0 )
				//	result.Add( c );
			}

			return result;
		}

		void WordWrapRenderUI( CanvasRenderer renderer, RenderParentEditData parentEditData, string text )
		{
			var screenRect = GetScreenRectangle();
			var screenSize = GetScreenSize();
			var verticalIndention = VerticalIndention.Value;
			var screenVerticalIndention = GetScreenOffsetByValue( new UIMeasureValueVector2( verticalIndention.Measure, 0, verticalIndention.Value ) ).Y;

			var font = Font.Value;
			if( font == null )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;
			//if(font == null || font.Disposed)
			//	fon
			//EngineFont fontInternal = font;
			//if( fontInternal == null )
			//	fontInternal = renderer.DefaultFont;

			var fontSize = GetFontSizeScreen();

			//slowly to calculate every time?
			var lines = new List<LineItem>( 32 );
			{
				//calculate lengths
				var allCharacters = new TextItem( text.Length );
				for( int n = 0; n < text.Length; n++ )
				{
					var c = text[ n ];
					var width = font.GetCharacterWidth( fontSize, renderer, c );
					allCharacters.Add( new CharacterItem( c, n, width ) );
				}


				foreach( var characters in Split( allCharacters, '\n' ) )
				{

					var words = new List<TextItem>( 32 );
					{
						var currentWord = new TextItem( 32 );

						for( int n = 0; n < characters.Count; n++ )
						{
							var c = characters.characters[ n ];

							currentWord.Add( c );

							if( c.character == ' ' )
							{
								words.Add( currentWord );
								currentWord = new TextItem( 32 );
							}
						}

						if( currentWord.Count != 0 )
							words.Add( currentWord );
					}


					var currentLine = new List<TextItem>();
					var currentLineWidth = 0.0;

					foreach( var word in words )
					{
						var wordWidth = word.GetWidth();

						var words2 = new List<TextItem>();
						if( wordWidth >= screenSize.X )
						{
							//split long word to small
							foreach( var c in word.characters )
							{
								var w = new TextItem( 1 );
								w.Add( c );
								words2.Add( w );
							}
						}
						else
							words2.Add( word );

						foreach( var word2 in words2 )
						{
							var word2Width = word2.GetWidth();

							if( currentLineWidth + word2Width > screenSize.X )
							{
								lines.Add( new LineItem( currentLine, TextHorizontalAlignment.Value == EHorizontalAlignment.Stretch ) );
								currentLine = new List<TextItem>();
								currentLineWidth = 0;
							}

							currentLine.Add( word2 );
							currentLineWidth += word2Width;
						}
					}

					if( currentLine.Count != 0 )
						lines.Add( new LineItem( currentLine, false ) );
				}


				//add caret to end
				if( parentEditData != null && parentEditData.Edit.Focused )
				{
					var caret = parentEditData.Edit.GetCaretPosition();
					if( caret == allCharacters.Count )
					{
						if( lines.Count == 0 )
							lines.Add( new LineItem( new TextItem( 1 ), false ) );
						var line = lines[ lines.Count - 1 ];
						line.text.Add( new CharacterItem( '\0', caret, 0 ) );
					}
				}




				////double posY = 0;

				//var word = new TextItem( text.Length );
				//double wordLength = 0;

				//var toDraw = new TextItem( text.Length );
				//double toDrawLength = 0;

				//double spaceCharacterLength = font.GetCharacterWidth( fontSize, renderer, ' ' );

				//for( int index = 0; index < text.Length; index++ )
				//{
				//	var c = text[ index ];

				//	if( c == ' ' )
				//	{
				//		if( word.Count != 0 )
				//		{
				//			toDraw.Add( new CharacterItem( ' ', -1 ) );
				//			toDraw.AddRange( word );
				//			toDrawLength += spaceCharacterLength + wordLength;
				//			word.Clear();
				//			wordLength = 0;
				//		}
				//		else
				//		{
				//			toDraw.Add( new CharacterItem( ' ', -1 ) );
				//			toDrawLength += spaceCharacterLength;
				//		}
				//		continue;
				//	}

				//	if( c == '\n' )
				//	{
				//		toDraw.Add( new CharacterItem( ' ', -1 ) );
				//		toDraw.AddRange( word );
				//		toDrawLength += wordLength;
				//		lines.Add( new LineItem( Trim( toDraw ), false ) );

				//		//posY += fontSize + screenVerticalIndention;
				//		////if( posY >= screenSize.Y )
				//		////   break;

				//		toDraw.Clear();
				//		toDrawLength = 0;
				//		word.Clear();
				//		wordLength = 0;
				//	}

				//	//slowly?
				//	double characterWidth = font.GetCharacterWidth( fontSize, renderer, c );

				//	if( toDrawLength + wordLength + characterWidth >= screenSize.X )
				//	{
				//		if( toDraw.Count == 0 )
				//		{
				//			toDraw.Clear();
				//			toDraw.AddRange( word );
				//			toDrawLength = wordLength;
				//			word.Clear();
				//			wordLength = 0;
				//		}
				//		lines.Add( new LineItem( Trim( toDraw ), TextHorizontalAlignment.Value == EHorizontalAlignment.Stretch ) );

				//		//posY += fontSize + screenVerticalIndention;
				//		////if( posY >= screenSize.Y )
				//		////   break;

				//		toDraw.Clear();
				//		toDrawLength = 0;
				//	}

				//	word.Add( new CharacterItem( c, index ) );
				//	wordLength += characterWidth;
				//}

				//var s = new TextItem( toDraw.Count + 1 + word.Count );
				//s.AddRange( toDraw );
				//s.Add( new CharacterItem( ' ', -1 ) );
				//s.AddRange( word );
				//s = Trim( s );
				//if( s.Count != 0 )
				//{
				//	//bool skip = false;
				//	//if( clipRectangle && posY >= screenSize.Y )
				//	//   skip = true;
				//	//if( !skip )
				//	lines.Add( new LineItem( s, false ) );
				//}
			}

			if( lines.Count != 0 )
			{
				var shadowColor = ShadowColor.Value;
				var textColor = Color.Value;
				double stepY = fontSize + screenVerticalIndention;
				var verticalAlign = TextVerticalAlignment.Value;

				var shadowScreenOffset = Shadow ? GetScreenOffsetByValue( ShadowOffset ) : Vector2.Zero;

				double startY = 0;
				switch( verticalAlign )
				{
				case EVerticalAlignment.Top:
					startY = screenRect.Top;
					break;
				case EVerticalAlignment.Center:
					{
						double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
						startY = screenRect.Top + ( screenRect.GetSize().Y - height ) / 2;
					}
					break;
				case EVerticalAlignment.Bottom:
					{
						double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
						startY = screenRect.Bottom - height;
					}
					break;
				}

				//UIEdit: change screen position by caret
				if( parentEditData != null && parentEditData.Edit.Focused && ( verticalAlign == EVerticalAlignment.Top || verticalAlign == EVerticalAlignment.Bottom ) )
				{
					var caret = parentEditData.Edit.GetCaretPosition();

					if( verticalAlign == EVerticalAlignment.Top )
					{
						var caretInLine = 0;
						{
							for( int nLine = 0; nLine < lines.Count; nLine++ )
							{
								var line = lines[ nLine ];

								foreach( var c in line.text.characters )
								{
									if( c.index == caret )
									{
										caretInLine = nLine;
										goto q;
									}
								}
							}
							q:;
						}

						var textWidthToSeeCaret = stepY * ( caretInLine + 1 );

						if( textWidthToSeeCaret > screenSize.Y )
							startY -= textWidthToSeeCaret - screenSize.Y;
					}

					//!!!!
					//if( verticalAlign == EVerticalAlignment.Bottom )
					//{
					//var textToSeeCaret = text.Substring( caret ) + " ";
					//float textWidthToSeeCaret = font.GetTextLength( fontSize, renderer, textToSeeCaret );

					//GetScreenSize( out var screenSize );
					//if( textWidthToSeeCaret > screenSize.X )
					//	screenPos.X += textWidthToSeeCaret - screenSize.X;
					//}
				}


				for( int nStep = Shadow ? 0 : 1; nStep < 2; nStep++ )
				{
					double positionY = startY;
					foreach( var line in lines )
					{
						if( line.alignByWidth )
						{
							//zero width of last space
							if( line.text.Count != 0 )
							{
								var lastCharacter = line.text.characters[ line.text.Count - 1 ];
								if( lastCharacter.character == ' ' )
								{
									lastCharacter.width = 0;
									line.text.characters[ line.text.Count - 1 ] = lastCharacter;
								}
							}

							var words = Split( line.text, ' ' );
							double[] widths = new double[ words.Count ];
							double totalWidth = 0;
							for( int n = 0; n < widths.Length; n++ )
							{
								double width = words[ n ].GetWidth();
								widths[ n ] = width;
								totalWidth += width;
							}

							double space = 0;
							if( words.Count > 1 )
								space = ( screenSize.X - totalWidth ) / ( words.Count - 1 );

							double posX = screenRect.Left;
							for( int n = 0; n < words.Count; n++ )
							{
								var word = words[ n ];

								Vector2 pos = new Vector2( posX, positionY ) + GetScreenOffsetByValue( Offset );

								if( nStep == 0 )
								{
									renderer.AddText( font, fontSize, word.GetText(), pos + shadowScreenOffset, EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
								}
								else
								{
									renderer.AddText( font, fontSize, word.GetText(), pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );

									//selection, caret of parent UIEdit
									if( parentEditData != null && ( parentEditData.Edit.SelectionLength.Value > 0 || parentEditData.Edit.Focused ) )
										RenderEditSelectionAndCursor( renderer, parentEditData, word, fontSize, pos );
								}

								posX += widths[ n ] + space;
							}
						}
						else
						{
							double positionX = 0;
							switch( TextHorizontalAlignment.Value )
							{
							case EHorizontalAlignment.Left:
							case EHorizontalAlignment.Stretch:
								positionX = screenRect.Left;
								break;
							case EHorizontalAlignment.Center:
								positionX = screenRect.Left + ( screenRect.GetSize().X - font.GetTextLength( fontSize, renderer, line.text.GetText() ) ) / 2;
								break;
							case EHorizontalAlignment.Right:
								positionX = screenRect.Right - font.GetTextLength( fontSize, renderer, line.text.GetText() );
								break;
							}

							Vector2 pos = new Vector2( positionX, positionY ) + GetScreenOffsetByValue( Offset );

							if( nStep == 0 )
							{
								renderer.AddText( font, fontSize, line.text.GetText(), pos + shadowScreenOffset, EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
							}
							else
							{
								renderer.AddText( font, fontSize, line.text.GetText(), pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );

								//selection, caret of parent UIEdit
								if( parentEditData != null && ( parentEditData.Edit.SelectionLength.Value > 0 || parentEditData.Edit.Focused ) )
									RenderEditSelectionAndCursor( renderer, parentEditData, line.text, fontSize, pos );
							}
						}

						positionY += stepY;
					}
				}
			}
		}

		//struct LineItem
		//{
		//public string text;
		//public bool alignByWidth;

		//public LineItem( string text, bool alignByWidth )
		//{
		//	this.text = text;
		//	this.alignByWidth = alignByWidth;
		//}
		//}

		//void WordWrapRenderUI( CanvasRenderer renderer, RenderParentEditData parentEditData, string text )
		//{
		//	var screenRect = GetScreenRectangle();
		//	var screenSize = GetScreenSize();
		//	var verticalIndention = VerticalIndention.Value;
		//	var screenVerticalIndention = GetScreenOffsetByValue( new UIMeasureValueVector2( verticalIndention.Measure, 0, verticalIndention.Value ) ).Y;

		//	var font = Font.Value;
		//	if( font == null )
		//		font = renderer.DefaultFont;
		//	if( font == null || font.Disposed )
		//		return;
		//	//if(font == null || font.Disposed)
		//	//	fon
		//	//EngineFont fontInternal = font;
		//	//if( fontInternal == null )
		//	//	fontInternal = renderer.DefaultFont;

		//	var fontSize = GetFontSizeScreen();

		//	//slowly to calculate every time?
		//	var lines = new List<LineItem>();
		//	{
		//		double posY = 0;

		//		var word = new StringBuilder( text.Length );
		//		double wordLength = 0;

		//		var toDraw = new StringBuilder( text.Length );
		//		double toDrawLength = 0;

		//		double spaceCharacterLength = font.GetCharacterWidth( fontSize, renderer, ' ' );

		//		foreach( char c in text )
		//		{
		//			if( c == ' ' )
		//			{
		//				if( word.Length != 0 )
		//				{
		//					toDraw.Append( ' ' );
		//					toDraw.Append( word );
		//					toDrawLength += spaceCharacterLength + wordLength;
		//					word.Length = 0;
		//					wordLength = 0;
		//				}
		//				else
		//				{
		//					toDraw.Append( ' ' );
		//					toDrawLength += spaceCharacterLength;
		//				}
		//				continue;
		//			}

		//			if( c == '\n' )
		//			{
		//				toDraw.Append( ' ' );
		//				toDraw.Append( word );
		//				toDrawLength += wordLength;
		//				lines.Add( new LineItem( toDraw.ToString().Trim(), false ) );

		//				posY += fontSize + screenVerticalIndention;
		//				//if( posY >= screenSize.Y )
		//				//   break;

		//				toDraw.Length = 0;
		//				toDrawLength = 0;
		//				word.Length = 0;
		//				wordLength = 0;
		//			}

		//			//slowly?
		//			double characterWidth = font.GetCharacterWidth( fontSize, renderer, c );

		//			if( toDrawLength + wordLength + characterWidth >= screenSize.X )
		//			{
		//				if( toDraw.Length == 0 )
		//				{
		//					toDraw.Length = 0;
		//					toDraw.Append( word.ToString() );
		//					toDrawLength = wordLength;
		//					word.Length = 0;
		//					wordLength = 0;
		//				}
		//				lines.Add( new LineItem( toDraw.ToString().Trim(), TextHorizontalAlignment.Value == EHorizontalAlignment.Stretch ) );

		//				posY += fontSize + screenVerticalIndention;
		//				//if( posY >= screenSize.Y )
		//				//   break;

		//				toDraw.Length = 0;
		//				toDrawLength = 0;
		//			}

		//			word.Append( c );
		//			wordLength += characterWidth;
		//		}

		//		string s = string.Format( "{0} {1}", toDraw, word );
		//		s = s.Trim();
		//		if( s.Length != 0 )
		//		{
		//			//bool skip = false;
		//			//if( clipRectangle && posY >= screenSize.Y )
		//			//   skip = true;
		//			//if( !skip )
		//			lines.Add( new LineItem( s, false ) );
		//		}
		//	}

		//	if( lines.Count != 0 )
		//	{
		//		var shadowColor = ShadowColor.Value;
		//		var textColor = Color.Value;

		//		var shadowScreenOffset = Shadow ? GetScreenOffsetByValue( ShadowOffset ) : Vector2.Zero;
		//		//var shadowLocalOffset = Shadow ? GetLocalOffsetByValue( ShadowOffset ) : Vec2.Zero;

		//		double startY = 0;
		//		switch( TextVerticalAlignment.Value )
		//		{
		//		case EVerticalAlignment.Top:
		//			startY = screenRect.Top;
		//			break;
		//		case EVerticalAlignment.Center:
		//			{
		//				double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
		//				startY = screenRect.Top + ( screenRect.GetSize().Y - height ) / 2;
		//			}
		//			break;
		//		case EVerticalAlignment.Bottom:
		//			{
		//				double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
		//				startY = screenRect.Bottom - height;
		//			}
		//			break;
		//		}

		//		//!!!!
		//		double stepY = fontSize + screenVerticalIndention;

		//		for( int nStep = Shadow ? 0 : 1; nStep < 2; nStep++ )
		//		{
		//			double positionY = startY;
		//			foreach( LineItem line in lines )
		//			{
		//				if( line.alignByWidth )
		//				{
		//					string[] words = line.text.Split( new char[] { ' ' } );
		//					double[] lengths = new double[ words.Length ];
		//					double totalLength = 0;
		//					for( int n = 0; n < lengths.Length; n++ )
		//					{
		//						double length = font.GetTextLength( fontSize, renderer, words[ n ] );
		//						lengths[ n ] = length;
		//						totalLength += length;
		//					}

		//					double space = 0;
		//					if( words.Length > 1 )
		//						space = ( screenSize.X - totalLength ) / ( words.Length - 1 );

		//					double posX = screenRect.Left;
		//					for( int n = 0; n < words.Length; n++ )
		//					{
		//						Vector2 pos = new Vector2( posX, positionY ) + GetScreenOffsetByValue( Offset );

		//						if( nStep == 0 )
		//						{
		//							renderer.AddText( font, fontSize, words[ n ],
		//								pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
		//								EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
		//						}
		//						else
		//							renderer.AddText( font, fontSize, words[ n ], pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );

		//						posX += lengths[ n ] + space;
		//					}
		//				}
		//				else
		//				{
		//					double positionX = 0;
		//					switch( TextHorizontalAlignment.Value )
		//					{
		//					case EHorizontalAlignment.Left:
		//					case EHorizontalAlignment.Stretch:
		//						positionX = screenRect.Left;
		//						break;
		//					case EHorizontalAlignment.Center:
		//						positionX = screenRect.Left + ( screenRect.GetSize().X - font.GetTextLength( fontSize, renderer, line.text ) ) / 2;
		//						break;
		//					case EHorizontalAlignment.Right:
		//						positionX = screenRect.Right - font.GetTextLength( fontSize, renderer, line.text );
		//						break;
		//					}

		//					Vector2 pos = new Vector2( positionX, positionY ) + GetScreenOffsetByValue( Offset );

		//					if( nStep == 0 )
		//					{
		//						renderer.AddText( font, fontSize, line.text,
		//							pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
		//							EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
		//					}
		//					else
		//						renderer.AddText( font, fontSize, line.text, pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );
		//				}

		//				positionY += stepY;
		//			}
		//		}
		//	}
		//}

		//public Font GetFont()
		//{
		//	xx
		//	var font = Font.Value;
		//	if( font != null )
		//		font = renderer.DefaultFont;
		//	if( font == null || font.Disposed )
		//		return;
		//}

		public double GetFontSizeScreen()
		{
			var value = FontSize.Value;
			var fontSize = GetScreenOffsetByValue( new UIMeasureValueVector2( value.Measure, 0, value.Value ) ).Y;
			return fontSize;
		}
	}
}
