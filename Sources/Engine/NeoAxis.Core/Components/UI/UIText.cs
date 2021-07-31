// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

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
		public Reference<Component_Font> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( ref value ) ) { try { FontChanged?.Invoke( this ); } finally { _font.EndSet(); } } }
		}
		public event Action<UIText> FontChanged;
		ReferenceField<Component_Font> _font = null;

		/// <summary>
		/// Font size of rendered text.
		/// </summary>
		[DefaultValue( "Screen 0.02" )]
		public Reference<UIMeasureValueDouble> FontSize
		{
			get { if( _fontSize.BeginGet() ) FontSize = _fontSize.Get( this ); return _fontSize.value; }
			set { if( _fontSize.BeginSet( ref value ) ) { try { FontSizeChanged?.Invoke( this ); } finally { _fontSize.EndSet(); } } }
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
			set { if( _textHorizontalAlignment.BeginSet( ref value ) ) { try { TextHorizontalAlignChanged?.Invoke( this ); } finally { _textHorizontalAlignment.EndSet(); } } }
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
			set { if( _textVerticalAlignment.BeginSet( ref value ) ) { try { TextVerticalAlignChanged?.Invoke( this ); } finally { _textVerticalAlignment.EndSet(); } } }
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
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
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
			set { if( _offset.BeginSet( ref value ) ) { try { OffsetChanged?.Invoke( this ); } finally { _offset.EndSet(); } } }
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
			set { if( _wordWrap.BeginSet( ref value ) ) { try { WordWrapChanged?.Invoke( this ); } finally { _wordWrap.EndSet(); } } }
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
			set { if( _verticalIndention.BeginSet( ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
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
		//	set { if( _verticalIndention.BeginSet( ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
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
		//	set { if( _autoSize.BeginSet( ref value ) ) { try { AutoSizeChanged?.Invoke( this ); } finally { _autoSize.EndSet(); } } }
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
			set { if( _shadow.BeginSet( ref value ) ) { try { ShadowChanged?.Invoke( this ); } finally { _shadow.EndSet(); } } }
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
			set { if( _shadowOffset.BeginSet( ref value ) ) { try { ShadowOffsetChanged?.Invoke( this ); } finally { _shadowOffset.EndSet(); } } }
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
			set { if( _shadowColor.BeginSet( ref value ) ) { try { ShadowColorChanged?.Invoke( this ); } finally { _shadowColor.EndSet(); } } }
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
		//	set { if( _supportLocalization.BeginSet( ref value ) ) { try { SupportLocalizationChanged?.Invoke( this ); } finally { _supportLocalization.EndSet(); } } }
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
			set { if( _clipRectangle.BeginSet( ref value ) ) { try { ClipRectangleChanged?.Invoke( this ); } finally { _clipRectangle.EndSet(); } } }
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

		public void RenderDefaultStyle( CanvasRenderer renderer )
		{
			string localizedText;
			//!!!!
			//if( SupportLocalization )
			//	localizedText = LanguageManager.Instance.Translate( "UISystem", Text );
			//else
			localizedText = Text;

			//!!!!было
			//if( AutoSize )
			//	UpdateAutoSize( renderer, localizedText );

			if( !string.IsNullOrEmpty( localizedText ) )
			{
				if( !WordWrap )
					NoWordWrapRenderUI( renderer, localizedText );
				else
					WordWrapRenderUI( renderer, localizedText );
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

		void NoWordWrapRenderUI( CanvasRenderer renderer, string localizedText )
		{
			Vector2 localPos = GetLocalOffsetByValue( Offset );

			switch( TextHorizontalAlignment.Value )
			{
			case EHorizontalAlignment.Left: localPos.X += 0; break;
			case EHorizontalAlignment.Center: localPos.X += .5f; break;
			case EHorizontalAlignment.Right: localPos.X += 1; break;
			}

			switch( TextVerticalAlignment.Value )
			{
			case EVerticalAlignment.Top: localPos.Y += 0; break;
			case EVerticalAlignment.Center: localPos.Y += .5f; break;
			case EVerticalAlignment.Bottom: localPos.Y += 1; break;
			}

			var fontSize = GetFontSizeScreen();

			if( Shadow )
			{
				ColorValue color = ShadowColor.Value.GetSaturate();
				renderer.AddText( Font, fontSize, localizedText,
					ConvertLocalToScreen( localPos + GetLocalOffsetByValue( ShadowOffset ) ),
					TextHorizontalAlignment, TextVerticalAlignment, color );
			}

			{
				ColorValue color = Color.Value.GetSaturate();
				renderer.AddText( Font, fontSize, localizedText, ConvertLocalToScreen( localPos ), TextHorizontalAlignment, TextVerticalAlignment, color );
			}
		}

		struct LineItem
		{
			public string text;
			public bool alignByWidth;
			public LineItem( string text, bool alignByWidth )
			{
				this.text = text;
				this.alignByWidth = alignByWidth;
			}
		}

		void WordWrapRenderUI( CanvasRenderer renderer, string localizedText )
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
			var lines = new List<LineItem>();
			{
				double posY = 0;

				var word = new StringBuilder( localizedText.Length );
				double wordLength = 0;

				var toDraw = new StringBuilder( localizedText.Length );
				double toDrawLength = 0;

				double spaceCharacterLength = font.GetCharacterWidth( fontSize, renderer, ' ' );

				foreach( char c in localizedText )
				{
					if( c == ' ' )
					{
						if( word.Length != 0 )
						{
							toDraw.Append( ' ' );
							toDraw.Append( word );
							toDrawLength += spaceCharacterLength + wordLength;
							word.Length = 0;
							wordLength = 0;
						}
						else
						{
							toDraw.Append( ' ' );
							toDrawLength += spaceCharacterLength;
						}
						continue;
					}

					if( c == '\n' )
					{
						toDraw.Append( ' ' );
						toDraw.Append( word );
						toDrawLength += wordLength;
						lines.Add( new LineItem( toDraw.ToString().Trim(), false ) );

						posY += fontSize + screenVerticalIndention;
						//if( posY >= screenSize.Y )
						//   break;

						toDraw.Length = 0;
						toDrawLength = 0;
						word.Length = 0;
						wordLength = 0;
					}

					//slowly?
					double characterWidth = font.GetCharacterWidth( fontSize, renderer, c );

					if( toDrawLength + wordLength + characterWidth >= screenSize.X )
					{
						if( toDraw.Length == 0 )
						{
							toDraw.Length = 0;
							toDraw.Append( word.ToString() );
							toDrawLength = wordLength;
							word.Length = 0;
							wordLength = 0;
						}
						lines.Add( new LineItem( toDraw.ToString().Trim(), TextHorizontalAlignment.Value == EHorizontalAlignment.Stretch ) );

						posY += fontSize + screenVerticalIndention;
						//if( posY >= screenSize.Y )
						//   break;

						toDraw.Length = 0;
						toDrawLength = 0;
					}

					word.Append( c );
					wordLength += characterWidth;
				}

				string s = string.Format( "{0} {1}", toDraw, word );
				s = s.Trim();
				if( s.Length != 0 )
				{
					//bool skip = false;
					//if( clipRectangle && posY >= screenSize.Y )
					//   skip = true;
					//if( !skip )
					lines.Add( new LineItem( s, false ) );
				}
			}

			if( lines.Count != 0 )
			{
				var shadowColor = ShadowColor.Value;
				var textColor = Color.Value;
				var shadowScreenOffset = Shadow ? GetScreenOffsetByValue( ShadowOffset ) : Vector2.Zero;
				//var shadowLocalOffset = Shadow ? GetLocalOffsetByValue( ShadowOffset ) : Vec2.Zero;

				double startY = 0;
				switch( TextVerticalAlignment.Value )
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

				//!!!!
				double stepY = fontSize + screenVerticalIndention;

				for( int nStep = Shadow ? 0 : 1; nStep < 2; nStep++ )
				{
					double positionY = startY;
					foreach( LineItem line in lines )
					{
						if( line.alignByWidth )
						{
							string[] words = line.text.Split( new char[] { ' ' } );
							double[] lengths = new double[ words.Length ];
							double totalLength = 0;
							for( int n = 0; n < lengths.Length; n++ )
							{
								double length = font.GetTextLength( fontSize, renderer, words[ n ] );
								lengths[ n ] = length;
								totalLength += length;
							}

							double space = 0;
							if( words.Length > 1 )
								space = ( screenSize.X - totalLength ) / ( words.Length - 1 );

							double posX = screenRect.Left;
							for( int n = 0; n < words.Length; n++ )
							{
								Vector2 pos = new Vector2( posX, positionY ) + GetScreenOffsetByValue( Offset );

								if( nStep == 0 )
								{
									renderer.AddText( font, fontSize, words[ n ],
										pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
										EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
								}
								else
									renderer.AddText( font, fontSize, words[ n ], pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );

								posX += lengths[ n ] + space;
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
								positionX = screenRect.Left + ( screenRect.GetSize().X - font.GetTextLength( fontSize, renderer, line.text ) ) / 2;
								break;
							case EHorizontalAlignment.Right:
								positionX = screenRect.Right - font.GetTextLength( fontSize, renderer, line.text );
								break;
							}

							Vector2 pos = new Vector2( positionX, positionY ) + GetScreenOffsetByValue( Offset );

							if( nStep == 0 )
							{
								renderer.AddText( font, fontSize, line.text,
									pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
									EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor );
							}
							else
								renderer.AddText( font, fontSize, line.text, pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor );
						}

						positionY += stepY;
					}
				}
			}
		}

		//public Component_Font GetFont()
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
