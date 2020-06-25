// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[EditorDocumentWindow( typeof( Component_Font_DocumentWindow ) )]
	/// <summary>
	/// Represents font import settings.
	/// </summary>
	[EditorPreviewControl( typeof( Component_Font_PreviewControl ) )]
	public class Component_Font : Component
	{
		//true type specific
		bool[] trueTypeCharactersInitializedFlags;
		IntPtr ttfFileData;
		int ttdFileDataSize;
		IntPtr freeTypeFace;

		List<CompiledData> compiledDataItems = new List<CompiledData>();
		//Dictionary<int, CompiledData> compiledDataItems = new Dictionary<int, CompiledData>();

		///////////////////////////////////////////

		/// <summary>
		/// The list of characters that can be used as a list of ranges.
		/// </summary>
		[Category( "Characters" )]
		public Reference<List<RangeI>> CharacterRanges
		{
			get { if( _characterRanges.BeginGet() ) CharacterRanges = _characterRanges.Get( this ); return _characterRanges.value; }
			set { if( _characterRanges.BeginSet( ref value ) ) { try { CharacterRangesChanged?.Invoke( this ); } finally { _characterRanges.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CharacterRanges"/> property value changes.</summary>
		public event Action<Component_Font> CharacterRangesChanged;
		ReferenceField<List<RangeI>> _characterRanges = new List<RangeI>();

		/// <summary>
		/// The list of characters that can be used as a string.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Characters" )]
		public Reference<string> CharacterString
		{
			get { if( _characterString.BeginGet() ) CharacterString = _characterString.Get( this ); return _characterString.value; }
			set { if( _characterString.BeginSet( ref value ) ) { try { CharacterStringChanged?.Invoke( this ); } finally { _characterString.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CharacterString"/> property value changes.</summary>
		public event Action<Component_Font> CharacterStringChanged;
		ReferenceField<string> _characterString = "";

		/// <summary>
		/// The multiplier of the defined font size.
		/// </summary>
		[DefaultValue( "1 1" )]
		[Range( 0.5, 2.0 )]
		[Category( "Transform" )]
		public Reference<Vector2> SizeMultiplier
		{
			get { if( _sizeMultiplier.BeginGet() ) SizeMultiplier = _sizeMultiplier.Get( this ); return _sizeMultiplier.value; }
			set { if( _sizeMultiplier.BeginSet( ref value ) ) { try { SizeMultiplierChanged?.Invoke( this ); } finally { _sizeMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SizeMultiplier"/> property value changes.</summary>
		public event Action<Component_Font> SizeMultiplierChanged;
		ReferenceField<Vector2> _sizeMultiplier = new Vector2( 1, 1 );

		//!!!!неправильно рассчитывает тогда для экрана?
		/// <summary>
		/// The multiplier of the size of spaces between symbols.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.5, 2.0 )]
		[Category( "Transform" )]
		public Reference<double> AdvanceMultiplier
		{
			get { if( _advanceMultiplier.BeginGet() ) AdvanceMultiplier = _advanceMultiplier.Get( this ); return _advanceMultiplier.value; }
			set { if( _advanceMultiplier.BeginSet( ref value ) ) { try { AdvanceMultiplierChanged?.Invoke( this ); } finally { _advanceMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AdvanceMultiplier"/> property value changes.</summary>
		public event Action<Component_Font> AdvanceMultiplierChanged;
		ReferenceField<double> _advanceMultiplier = 1.0;

		/// <summary>
		/// The offset by vertical.
		/// </summary>
		[DefaultValue( -0.15 )]
		[Range( -1.0, 1.0 )]
		[Category( "Transform" )]
		public Reference<double> DrawOffsetY
		{
			get { if( _drawOffsetY.BeginGet() ) DrawOffsetY = _drawOffsetY.Get( this ); return _drawOffsetY.value; }
			set { if( _drawOffsetY.BeginSet( ref value ) ) { try { DrawOffsetYChanged?.Invoke( this ); } finally { _drawOffsetY.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DrawOffsetY"/> property value changes.</summary>
		public event Action<Component_Font> DrawOffsetYChanged;
		ReferenceField<double> _drawOffsetY = -0.15;

		//!!!!default
		/// <summary>
		/// Maximum pixel size of a character. This limit may be useful in case of big font size, which may consume a lot of memory.
		/// </summary>
		[DefaultValue( 64 )]
		[Category( "Optimization" )]
		public Reference<int> BakingMaxCharacterSizeInPixels
		{
			get { if( _bakingMaxCharacterSizeInPixels.BeginGet() ) BakingMaxCharacterSizeInPixels = _bakingMaxCharacterSizeInPixels.Get( this ); return _bakingMaxCharacterSizeInPixels.value; }
			set { if( _bakingMaxCharacterSizeInPixels.BeginSet( ref value ) ) { try { BakingMaxCharacterSizeInPixelsChanged?.Invoke( this ); } finally { _bakingMaxCharacterSizeInPixels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BakingMaxCharacterSizeInPixels"/> property value changes.</summary>
		public event Action<Component_Font> BakingMaxCharacterSizeInPixelsChanged;
		ReferenceField<int> _bakingMaxCharacterSizeInPixels = 64;
		//!!!!
		//TextureSizes textureSize = TextureSizes._512;
		//int textureIndentBetweenCharacters = 8;

		//int renderingIn3DHeightInPixels = 24;

		/////////////////////////////////////////

		internal struct TrueTypeCharacterInfo
		{
			public Vector2I sizeInPixels;
			public int advanceInPixels;
			public Vector2I drawOffsetInPixels;

			//packed texture info
			public int textureIndex;
			public Vector2I texturePixelPosition;
		}

		/////////////////////////////////////////

		class TextureCharacterPacker
		{
			public struct CharacterInfo
			{
				char character;
				Vector2I size;

				//output
				int textureIndex;
				Vector2I texturePixelPosition;

				//

				public CharacterInfo( char character, Vector2I size )
				{
					this.character = character;
					this.size = size;
					this.textureIndex = 0;
					this.texturePixelPosition = Vector2I.Zero;
				}

				public char Character
				{
					get { return character; }
				}

				public Vector2I Size
				{
					get { return size; }
				}

				public int TextureIndex
				{
					get { return textureIndex; }
					set { textureIndex = value; }
				}

				public Vector2I TexturePixelPosition
				{
					get { return texturePixelPosition; }
					set { texturePixelPosition = value; }
				}
			}

			//

			public static bool Pack( int textureSize, int indent, CharacterInfo[] characters )
			{
				//sort by size.Y
				CollectionUtility.SelectionSort( characters, delegate ( CharacterInfo c1, CharacterInfo c2 )
				{
					if( c1.Size.Y < c2.Size.Y )
						return 1;
					if( c1.Size.Y > c2.Size.Y )
						return -1;
					return 0;
				} );

				int actualTextureSize = textureSize - indent * 2;

				int currentTextureIndex;
				Vector2I currentTexturePosition;
				int maxSizeYInThisLine;

				currentTextureIndex = 0;
				currentTexturePosition = Vector2I.Zero;
				maxSizeYInThisLine = 0;

				for( int nCharacter = 0; nCharacter < characters.Length; nCharacter++ )
				{
					CharacterInfo characterInfo = characters[ nCharacter ];

					if( characterInfo.Size.X > actualTextureSize )
					{
						//texture is so small
						string text = string.Format( "Width of character ({0} pixels) is more than texture size.", characterInfo.Size.X );

						////!!!!
						//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )//  RendererWorld.isEditor )
						//	Log.Info( text );
						//else
						Log.Warning( text );

						return false;
					}

					//check for next line
					if( currentTexturePosition.X + characterInfo.Size.X > actualTextureSize )
					{
						//next line
						currentTexturePosition.X = 0;
						currentTexturePosition.Y += maxSizeYInThisLine + indent;
						maxSizeYInThisLine = 0;

						//check for next texture
						if( currentTexturePosition.Y + characterInfo.Size.Y > actualTextureSize )
						{
							//next texture
							currentTexturePosition.Y = 0;
							currentTextureIndex++;
						}
					}

					if( characterInfo.Size.Y > maxSizeYInThisLine )
						maxSizeYInThisLine = characterInfo.Size.Y;

					characterInfo.TextureIndex = currentTextureIndex;
					characterInfo.TexturePixelPosition = new Vector2I( indent, indent ) + currentTexturePosition;
					characters[ nCharacter ] = characterInfo;

					currentTexturePosition.X += characterInfo.Size.X + indent;
				}

				return true;
			}
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents precalculated data of <see cref="Component_Font"/>.
		/// </summary>
		public class CompiledData
		{
			internal Component_Font owner;
			internal Component_Image[] textures;
			internal int trueTypeFontSizeYInPixels;
			internal bool trueTypeFontSizeYInPixelsScaled;
			internal TrueTypeCharacterInfo[] trueTypeCharacters;
			//!!!!было
			//internal double trueTypeLastUsedTime;

			//string name;
			//string language;

			//float height;
			//EngineFontDefinition definition;
			//PrecompiledImageFontDefinition precompiledImageDefinition;
			//TrueTypeFontDefinition trueTypeDefinition;

			////Key: screen size y for true type. "0" for precompiled image or for in game 3d gui.
			//Dictionary<int, Variant> variants = new Dictionary<int, Variant>();
			//Variant lastVariant;

			////////////

			internal void Dispose()
			{
				if( textures != null )
				{
					foreach( var texture in textures )
						texture?.Dispose();
				}
			}

			public Component_Image[] Textures
			{
				get { return textures; }
			}

			public float GetCharacterWidth( double fontSize, CanvasRenderer forRenderer, char c )
			{
				//if( precompiledImageDefinition != null )
				//	return precompiledImageDefinition.GetCharacterAdvance( c ) * height * forRenderer.AspectRatioInv;

				//if( trueTypeDefinition != null )
				//{
				//	Variant variant = GetVariant( forRenderer );

				//	if( variant != null )
				//	{
				if( !owner.IsCharacterInitialized( c ) )
					c = (char)0;
				if( owner.IsCharacterInitialized( c ) )
				{
					if( forRenderer.IsScreen && !trueTypeFontSizeYInPixelsScaled )//&& !forRenderer._OutGeometryTransformEnabled )
					{
						Vector2F viewportSize = forRenderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2F();
						float width = (float)trueTypeCharacters[ c ].advanceInPixels / viewportSize.X * (float)owner.AdvanceMultiplier.Value;
						return width;
					}
					else
					{
						float coef = (float)fontSize / (float)trueTypeFontSizeYInPixels;
						float width = (float)( trueTypeCharacters[ c ].advanceInPixels ) * (float)owner.AdvanceMultiplier.Value * coef;
						width *= forRenderer.AspectRatioInv;
						return width;
					}
				}
				//	}
				//}

				return 0;
			}

			internal int GetCharacterTextureIndex( char c )
			{
				if( !owner.IsCharacterInitialized( c ) )
					c = (char)0;
				if( owner.IsCharacterInitialized( c ) )
					return trueTypeCharacters[ c ].textureIndex;

				//if( precompiledImageDefinition != null )
				//	return precompiledImageDefinition.GetCharacterTextureIndex( c );

				//if( trueTypeDefinition != null )
				//{
				//	Variant variant = GetVariant( forRenderer );

				//	if( variant != null )
				//	{
				//		bool initialized = false;
				//		if( c < trueTypeCharactersInitializedFlags.Length )
				//			initialized = trueTypeCharactersInitializedFlags[ c ];
				//		if( !initialized )
				//			c = (char)0;

				//		if( trueTypeCharactersInitializedFlags[ c ] )
				//			return variant.trueTypeCharacters[ c ].textureIndex;
				//	}
				//}

				return 0;
			}

			public float GetTextLength( double fontSize, CanvasRenderer forRenderer, string text )
			{
				if( text == null )
					return 0;

				float maxLineLength = 0;
				float lineLength = 0;

				foreach( char c in text )
				{
					if( c == '\n' )
					{
						if( lineLength > maxLineLength )
							maxLineLength = lineLength;
						lineLength = 0;
						continue;
					}

					lineLength += GetCharacterWidth( fontSize, forRenderer, c );
				}

				if( lineLength > maxLineLength )
					maxLineLength = lineLength;

				return maxLineLength;
			}

			//public bool GetTrueTypeCharacterInfo( CanvasRenderer forRenderer, char c, out Vector2I sizeInPixels, out int advanceInPixels,
			//	out Vector2I drawOffsetInPixels, out int textureIndex, out Vector2I texturePixelPosition )
			//{
			//	//if( trueTypeDefinition != null )
			//	//{
			//	//	Variant variant = GetVariant( forRenderer );

			//	//	if( variant != null )
			//	//	{
			//	if( !owner.IsCharacterInitialized( c ) )
			//		c = (char)0;
			//	if( owner.IsCharacterInitialized( c ) )
			//	{
			//		TrueTypeCharacterInfo info = trueTypeCharacters[ c ];

			//		sizeInPixels = info.sizeInPixels;
			//		advanceInPixels = info.advanceInPixels;
			//		drawOffsetInPixels = info.drawOffsetInPixels;
			//		textureIndex = info.textureIndex;
			//		texturePixelPosition = info.texturePixelPosition;
			//		return true;
			//	}
			//	//	}
			//	//}

			//	sizeInPixels = Vector2I.Zero;
			//	advanceInPixels = 0;
			//	drawOffsetInPixels = Vector2I.Zero;
			//	textureIndex = 0;
			//	texturePixelPosition = Vector2I.Zero;
			//	return false;
			//}

			public bool GetCharacterInfo( double fontSize, CanvasRenderer forRenderer, char c, out float width, out int textureIndex, out Vector2F drawOffset, out Vector2F drawSize, out RectangleF textureCoordinates )
			{
				//if( precompiledImageDefinition != null )
				//{
				//	if( precompiledImageDefinition.GetCharacterInfo( c, out width, out textureIndex, out drawOffset, out textureCoordinates ) )
				//	{
				//		width *= height;
				//		drawOffset *= height;
				//		drawSize = textureCoordinates.Size / precompiledImageDefinition.TextureCharacterSizeScale * height;

				//		width *= forRenderer.AspectRatioInv;
				//		drawOffset.X *= forRenderer.AspectRatioInv;
				//		drawSize.X *= forRenderer.AspectRatioInv;

				//		return true;
				//	}
				//}

				//if( trueTypeDefinition != null )
				//{
				//	Variant variant = GetVariant( forRenderer );

				//	if( variant != null )
				//	{

				if( !owner.IsCharacterInitialized( c ) )
					c = (char)0;
				if( owner.IsCharacterInitialized( c ) )
				{
					TrueTypeCharacterInfo info = trueTypeCharacters[ c ];

					textureIndex = info.textureIndex;

					int textureSize = GetTextureSize();

					if( forRenderer.IsScreen && !trueTypeFontSizeYInPixelsScaled )//&& !forRenderer._OutGeometryTransformEnabled )
					{
						Vector2F viewportSize = forRenderer.ViewportForScreenCanvasRenderer.SizeInPixels.ToVector2F();

						width = (float)info.advanceInPixels / viewportSize.X * (float)owner.AdvanceMultiplier.Value;
						drawOffset = info.drawOffsetInPixels.ToVector2F() / viewportSize;
						drawSize = info.sizeInPixels.ToVector2F() / viewportSize;

						Vector2F positionInTexture = info.texturePixelPosition.ToVector2F() / (float)textureSize;

						Vector2F sizeInTexture = info.sizeInPixels.ToVector2F() / (float)textureSize;

						textureCoordinates = new RectangleF( positionInTexture, positionInTexture + sizeInTexture );
					}
					else
					{
						float coef = (float)fontSize / (float)trueTypeFontSizeYInPixels;

						width = (float)( trueTypeCharacters[ c ].advanceInPixels ) * (float)owner.AdvanceMultiplier.Value * coef;

						drawOffset = info.drawOffsetInPixels.ToVector2F() * coef;
						drawSize = info.sizeInPixels.ToVector2F() * coef;

						Vector2F positionInTexture = info.texturePixelPosition.ToVector2F() / (float)textureSize;

						Vector2F sizeInTexture = ( info.sizeInPixels.ToVector2F() + new Vector2F( 1, 1 ) ) / (float)textureSize;

						textureCoordinates = new RectangleF( positionInTexture, positionInTexture + sizeInTexture );

						width *= forRenderer.AspectRatioInv;
						drawOffset.X *= forRenderer.AspectRatioInv;
						drawSize.X *= forRenderer.AspectRatioInv;
					}

					return true;
				}
				//	}
				//}

				width = 0;
				textureIndex = 0;
				drawOffset = Vector2F.Zero;
				drawSize = Vector2F.Zero;
				textureCoordinates = RectangleF.Zero;
				return false;
			}
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents an item for <see cref="GetWordWrapLines(double, CanvasRenderer, string, double)"/> method.
		/// </summary>
		public struct WordWrapLinesItem
		{
			string text;
			bool alignByWidth;

			public WordWrapLinesItem( string text, bool alignByWidth )
			{
				this.text = text;
				this.alignByWidth = alignByWidth;
			}

			public string Text
			{
				get { return text; }
			}

			public bool AlignByWidth
			{
				get { return alignByWidth; }
			}
		}

		///////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			////!!!!пока так
			//EngineThreading.CheckMainThread();

			if( EnabledInHierarchy )
			{
				LoadTTFFileData();

				//!!!!
				//if( trueTypeDefinition != null )
				//	RenderingSystem.RenderSystemEvent += RenderSystem_RenderSystemEvent;
			}
			else
			{
				DisposeAllCompiledData();

				//if( definition != null )
				//{
				//	//!!!!
				//	//if( trueTypeDefinition != null )
				//	//	RenderingSystem.RenderSystemEvent -= RenderSystem_RenderSystemEvent;

				//	RemoveAllVariants();

				//	definition = null;
				//	precompiledImageDefinition = null;
				//	trueTypeDefinition = null;
				//}

				FreeTTFFileData();
			}
		}

		/// <summary>
		/// Returns the character screen width.
		/// </summary>
		/// <param name="forRenderer"></param>
		/// <param name="c">The character.</param>
		/// <returns>The character screen width.</returns>
		public float GetCharacterWidth( double fontSize, CanvasRenderer forRenderer, char c )
		{
			var compiledData = GetCompiledData( fontSize, forRenderer );
			if( compiledData == null )
				return 0;
			return compiledData.GetCharacterWidth( fontSize, forRenderer, c );
		}

		/// <summary>
		/// Returns the text screen width.
		/// </summary>
		/// <param name="forRenderer"></param>
		/// <param name="text">The text.</param>
		/// <returns>The text screen width.</returns>
		public float GetTextLength( double fontSize, CanvasRenderer forRenderer, string text )
		{
			var compiledData = GetCompiledData( fontSize, forRenderer );
			if( compiledData == null )
				return 0;
			return compiledData.GetTextLength( fontSize, forRenderer, text );
		}

		public bool IsCharacterInitialized( char c )
		{
			//if( precompiledImageDefinition != null )
			//	return precompiledImageDefinition.IsCharacterInitialized( c );

			//if( trueTypeDefinition != null )
			//{
			if( trueTypeCharactersInitializedFlags == null )
				return false;
			if( c >= trueTypeCharactersInitializedFlags.Length )
				return false;
			return trueTypeCharactersInitializedFlags[ c ];
			//}

			//return false;
		}

		public CompiledData GetCompiledData( double fontSize, CanvasRenderer forRenderer )
		{
			if( ttfFileData == IntPtr.Zero )
				return null;

			int sizeInPixels = 24;
			if( forRenderer.IsScreen )
			{
				if( forRenderer.ViewportForScreenCanvasRenderer != null )
					sizeInPixels = (int)( (double)fontSize * (double)forRenderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y );
			}
			else
			{
				//!!!!
			}

			bool scaled = false;
			if( sizeInPixels >= BakingMaxCharacterSizeInPixels.Value )
			{
				scaled = true;
				sizeInPixels = BakingMaxCharacterSizeInPixels.Value;
			}

			var key = sizeInPixels;

			CompiledData compiledData = null;
			if( key < compiledDataItems.Count )
				compiledData = compiledDataItems[ key ];

			if( compiledData == null )//if( !compiledDataItems.TryGetValue( key, out compiledData ) )
			{
				compiledData = new CompiledData();
				compiledData.owner = this;
				compiledData.trueTypeFontSizeYInPixels = sizeInPixels;
				compiledData.trueTypeFontSizeYInPixelsScaled = scaled;

				if( !GenerateTrueTypeFontData( forRenderer, compiledData ) )
				{
					compiledData.Dispose();
					return null;
				}

				//!!!!
				//internal double trueTypeLastUsedTime;

				while( key >= compiledDataItems.Count )
					compiledDataItems.Add( null );
				compiledDataItems[ key ] = compiledData;
				//compiledDataItems.Add( key, compiledData );
			}

			return compiledData;
		}

		public WordWrapLinesItem[] GetWordWrapLines( double fontSize, CanvasRenderer forRenderer, string text, double width )
		{
			var compiledData = GetCompiledData( fontSize, forRenderer );
			if( compiledData == null )
				return new WordWrapLinesItem[ 0 ];

			List<WordWrapLinesItem> lines = new List<WordWrapLinesItem>();

			//float posY = 0;
			int count = 0;

			StringBuilder word = new StringBuilder( text.Length );
			float wordLength = 0;

			StringBuilder toDraw = new StringBuilder( text.Length );
			float toDrawLength = 0;

			float spaceCharacterLength = compiledData.GetCharacterWidth( fontSize, forRenderer, ' ' );

			foreach( char c in text )
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
					count++;
					lines.Add( new WordWrapLinesItem( toDraw.ToString().Trim(), false ) );

					//posY += Height + textVerticalIndention;

					//if( posY >= size.Y )
					//   break;

					toDraw.Length = 0;
					toDrawLength = 0;
					word.Length = 0;
					wordLength = 0;
				}

				//slowly?
				float characterWidth = compiledData.GetCharacterWidth( fontSize, forRenderer, c );

				if( toDrawLength + wordLength + characterWidth >= width )
				{
					if( toDraw.Length == 0 )
					{
						toDraw.Length = 0;
						toDraw.Append( word.ToString() );
						toDrawLength = wordLength;
						word.Length = 0;
						wordLength = 0;
					}
					count++;
					lines.Add( new WordWrapLinesItem( toDraw.ToString().Trim(), true ) );//alignByWidth ) );

					//posY += fontInternal.Height + textVerticalIndention;
					//if( posY >= size.Y )
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
				//if( posY < size.Y )
				lines.Add( new WordWrapLinesItem( s, false ) );
				count++;
			}

			return lines.ToArray();
		}

		bool LoadTTFFileData()
		{
			var fileName = ComponentUtility.GetOwnedFileNameOfComponent( this );
			if( string.IsNullOrEmpty( fileName ) )
				return false;

			if( ttfFileData == IntPtr.Zero )
			{
				if( !VirtualFile.Exists( fileName ) )
				{
					Log.Warning( "Font: Cannot load true type font \"{0}\".", fileName );
					return false;
				}

				using( VirtualFileStream stream = VirtualFile.Open( fileName ) )
				{
					ttdFileDataSize = (int)stream.Length;

					ttfFileData = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, ttdFileDataSize );

					int readed = stream.ReadUnmanaged( ttfFileData, ttdFileDataSize );
					if( readed != ttdFileDataSize )
					{
						Log.Warning( "Font: Cannot load true type font \"{0}\".", fileName );
						FreeTTFFileData();
						return false;
					}
				}
			}

			IntPtr freeTypeLibrary = EngineFontManager.GetOrInitFreeTypeLibrary();

			freeTypeFace = FreeType.CreateFace( freeTypeLibrary, ttfFileData, ttdFileDataSize );
			if( freeTypeFace == IntPtr.Zero )
			{
				Log.Warning( "Component_Font: LoadTTFFileData: Creating face failed for \"{0}\".", fileName );
				return false;
			}

			//generate trueTypeCharactersInitializedFlags
			{
				SetTrueTypeFaceSettings( null );

				int maxCharacter = 0;
				{
					foreach( var characterRange in CharacterRanges.Value )
					{
						if( characterRange.Minimum > maxCharacter )
							maxCharacter = characterRange.Minimum;
						if( characterRange.Maximum > maxCharacter )
							maxCharacter = characterRange.Maximum;
						//if( characterRange.Begin > maxCharacter )
						//	maxCharacter = characterRange.Begin;
						//if( characterRange.End > maxCharacter )
						//	maxCharacter = characterRange.End;
					}

					foreach( char character in CharacterString.Value )
					{
						if( character > maxCharacter )
							maxCharacter = character;
					}
				}

				trueTypeCharactersInitializedFlags = new bool[ maxCharacter + 1 ];

				{
					char character = (char)0;
					if( !trueTypeCharactersInitializedFlags[ character ] )
					{
						if( FreeType.IsGlyphExists( freeTypeLibrary, freeTypeFace, character ) )
							trueTypeCharactersInitializedFlags[ character ] = true;
					}
				}

				foreach( var characterRange in CharacterRanges.Value )
				{
					for( int character = characterRange.Minimum; character <= characterRange.Maximum; character++ )
					{
						if( !trueTypeCharactersInitializedFlags[ character ] )
						{
							if( FreeType.IsGlyphExists( freeTypeLibrary, freeTypeFace, character ) )
								trueTypeCharactersInitializedFlags[ character ] = true;
						}
					}
				}

				foreach( char character in CharacterString.Value )
				{
					if( !trueTypeCharactersInitializedFlags[ character ] )
					{
						if( FreeType.IsGlyphExists( freeTypeLibrary, freeTypeFace, character ) )
							trueTypeCharactersInitializedFlags[ character ] = true;
					}
				}

			}

			return true;
		}

		void FreeTTFFileData()
		{
			if( freeTypeFace != IntPtr.Zero )
			{
				FreeType.DestroyFace( freeTypeFace );
				freeTypeFace = IntPtr.Zero;
			}

			if( ttfFileData != IntPtr.Zero )
			{
				NativeUtility.Free( ttfFileData );
				ttfFileData = IntPtr.Zero;
				ttdFileDataSize = 0;
			}

			trueTypeCharactersInitializedFlags = null;
		}

		bool GenerateTrueTypeFontData( CanvasRenderer forRenderer, CompiledData variant )
		{
			SetTrueTypeFaceSettings( variant );

			int textureSize = GetTextureSize();

			variant.trueTypeCharacters = new TrueTypeCharacterInfo[ trueTypeCharactersInitializedFlags.Length ];

			for( int character = 0; character < trueTypeCharactersInitializedFlags.Length; character++ )
			{
				if( !trueTypeCharactersInitializedFlags[ character ] )
					continue;

				TrueTypeCharacterInfo characterInfo = new TrueTypeCharacterInfo();

				IntPtr freeTypeLibrary = EngineFontManager.GetOrInitFreeTypeLibrary();

				bool glyphGenerated = FreeType.GetGlyphData( freeTypeLibrary, freeTypeFace, character, 0, 0,
					IntPtr.Zero, out var glyphDrawOffsetX, out var glyphDrawOffsetY, out var glyphSizeX,
					out var glyphSizeY, out var glyphAdvance );

				if( glyphGenerated )
				{
					characterInfo.sizeInPixels = new Vector2I( glyphSizeX, glyphSizeY );
					characterInfo.advanceInPixels = glyphAdvance;

					float sizeY = (float)variant.trueTypeFontSizeYInPixels * (float)SizeMultiplier.Value.Y;

					//float offset = (float)variant.trueTypeFontSizeYInPixels - (float)glyphDrawOffsetY + sizeY * trueTypeDefinition.DrawOffsetY;
					//Log.Info( "char: " + ( (char)character ).ToString() + " " + offset.ToString() );

					characterInfo.drawOffsetInPixels = new Vector2I( glyphDrawOffsetX,
						(int)( (float)variant.trueTypeFontSizeYInPixels - (float)glyphDrawOffsetY +
						sizeY * DrawOffsetY.Value ) );
				}

				variant.trueTypeCharacters[ character ] = characterInfo;
			}

			int maxTextureIndex = 0;

			//pack to textures (calculate textureIndex, texturePixelPosition)
			{
				var list = new List<TextureCharacterPacker.CharacterInfo>( variant.trueTypeCharacters.Length );

				for( int character = 0; character < variant.trueTypeCharacters.Length; character++ )
				{
					TrueTypeCharacterInfo characterInfo = variant.trueTypeCharacters[ character ];
					if( !trueTypeCharactersInitializedFlags[ character ] )
						continue;

					var packCharacter = new TextureCharacterPacker.CharacterInfo( (char)character, characterInfo.sizeInPixels );
					list.Add( packCharacter );
				}

				TextureCharacterPacker.CharacterInfo[] array = list.ToArray();

				int indentBetweenCharacters = 8;// trueTypeDefinition.TextureIndentBetweenCharacters;

				if( !TextureCharacterPacker.Pack( textureSize, indentBetweenCharacters, array ) )
					return false;

				foreach( TextureCharacterPacker.CharacterInfo packCharacter in array )
				{
					int character = packCharacter.Character;

					TrueTypeCharacterInfo characterInfo = variant.trueTypeCharacters[ character ];
					characterInfo.textureIndex = packCharacter.TextureIndex;
					characterInfo.texturePixelPosition = packCharacter.TexturePixelPosition;

					variant.trueTypeCharacters[ character ] = characterInfo;

					if( packCharacter.TextureIndex > maxTextureIndex )
						maxTextureIndex = packCharacter.TextureIndex;
				}
			}

			variant.textures = new Component_Image[ maxTextureIndex + 1 ];

			//update texture
			if( !RenderingSystem.IsDeviceLost() )
				UpdateTrueTypeTextures( variant );

			return true;
		}

		void SetTrueTypeFaceSettings( CompiledData variant )
		{
			Vector2F pixels;

			if( variant != null )
			{
				pixels = new Vector2F( variant.trueTypeFontSizeYInPixels, variant.trueTypeFontSizeYInPixels ) * SizeMultiplier.Value.ToVector2F();
				if( pixels.X < 1 )
					pixels.X = 1;
				if( pixels.Y < 1 )
					pixels.Y = 1;
			}
			else
				pixels = new Vector2F( 16, 16 );

			if( !FreeType.SetPixelSizes( freeTypeFace, (int)pixels.X, (int)pixels.Y ) )
			{
				Log.Fatal( "Font: SetTrueTypeFaceSettings: FreeType.SetPixelSizes failed" );
			}
		}

		void UpdateTrueTypeTextures( CompiledData variant )
		{
			if( ttfFileData == IntPtr.Zero )
				return;

			int glyphBufferSize = 0;
			{
				for( int character = 0; character < variant.trueTypeCharacters.Length; character++ )
				{
					TrueTypeCharacterInfo characterInfo = variant.trueTypeCharacters[ character ];
					if( !trueTypeCharactersInitializedFlags[ character ] )
						continue;

					if( characterInfo.sizeInPixels.X > glyphBufferSize )
						glyphBufferSize = characterInfo.sizeInPixels.X;
					if( characterInfo.sizeInPixels.Y > glyphBufferSize )
						glyphBufferSize = characterInfo.sizeInPixels.Y;
				}
				glyphBufferSize += 1;
			}

			IntPtr glyphBuffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, glyphBufferSize * glyphBufferSize );

			SetTrueTypeFaceSettings( variant );

			int textureSize = GetTextureSize();

			for( int nTexture = 0; nTexture < variant.textures.Length; nTexture++ )
			{
				var texture = variant.textures[ nTexture ];

				bool useLA = true;
				////!!!!!!don't work on DX11
				useLA = false;
				//if( !RenderingSystem.IsOpenGL() )
				//	useLA = false;

				//create texture
				if( texture == null )
				{
					//!!!!
					bool mipmaps = false;
					////mipMaps only for 3d ingame gui
					//bool mipmaps = variant.key == 0;

					//!!!!
					//string namePrefix = string.Format( "__FontTexture_{0}_{1}", definition.FileName, nTexture );
					texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );

					texture.CreateType = Component_Image.TypeEnum._2D;
					texture.CreateSize = new Vector2I( textureSize, textureSize );
					texture.CreateMipmaps = mipmaps;// ? -1 : 0;

					if( useLA )
						texture.CreateFormat = PixelFormat.ByteLA;
					else
						texture.CreateFormat = PixelFormat.A8R8G8B8;

					var usage = Component_Image.Usages.WriteOnly;
					if( mipmaps )
						usage |= Component_Image.Usages.AutoMipmaps;
					texture.CreateUsage = usage;

					texture.Enabled = true;

					variant.textures[ nTexture ] = texture;
				}

				//fill texture
				unsafe
				{
					GpuTexture gpuTexture = texture.Result;

					//GpuPixelBuffer pixelBuffer = gpuTexture.GetBuffer();

					//!!!!для компрессированных сложнее
					int totalSize = PixelFormatUtility.GetNumElemBytes( gpuTexture.ResultFormat ) * gpuTexture.ResultSize.X * gpuTexture.ResultSize.Y;
					byte[] data = new byte[ totalSize ];
					//byte[] data = new byte[ pixelBuffer.GetSizeInBytes() ];

					//IntPtr lockPointer = pixelBuffer.Lock( HardwareBuffer.LockOptions.Normal );
					//PixelBox pixelBox = pixelBuffer.GetCurrentLock();

					fixed ( byte* pData = data )
					{
						//!!!!для компрессированных сложнее
						int rowPitch = gpuTexture.ResultSize.X;
						//int rowPitch = pixelBuffer.GetRowPitch();

						//clear texture
						unsafe
						{
							Vector2I size = gpuTexture.ResultSize;
							for( int y = 0; y < size.Y; y++ )
							{
								for( int x = 0; x < size.X; x++ )
								{
									if( useLA )
									{
										byte* pointer = (byte*)pData + y * rowPitch * 2 + x * 2;
										pointer[ 0 ] = 255;
										pointer[ 1 ] = 0;
									}
									else
									{
										byte* pointer = (byte*)pData + y * rowPitch * 4 + x * 4;

										pointer[ 0 ] = 255;
										pointer[ 1 ] = 255;
										pointer[ 2 ] = 255;
										pointer[ 3 ] = 0;

										//pointer[ 0 ] = 0; // A
										//pointer[ 1 ] = pointer[ 2 ] = pointer[ 3 ] = 255; // BGR
									}
								}
							}
						}

						for( int character = 0; character < variant.trueTypeCharacters.Length; character++ )
						{
							TrueTypeCharacterInfo characterInfo = variant.trueTypeCharacters[ character ];
							if( !trueTypeCharactersInitializedFlags[ character ] )
								continue;
							if( characterInfo.textureIndex != nTexture )
								continue;

							IntPtr freeTypeLibrary = EngineFontManager.GetOrInitFreeTypeLibrary();

							bool glyphGenerated = FreeType.GetGlyphData( freeTypeLibrary, freeTypeFace, character,
								glyphBufferSize, glyphBufferSize, glyphBuffer, out var glyphDrawOffsetX,
								out var glyphDrawOffsetY, out var glyphSizeX, out var glyphSizeY, out var glyphAdvance );

							if( glyphGenerated )
							{
								for( int y = 0; y < glyphSizeY; y++ )
								{
									for( int x = 0; x < glyphSizeX; x++ )
									{
										Vector2I destPixelPosition = characterInfo.texturePixelPosition +
											new Vector2I( x, y );

										if( destPixelPosition.X >= textureSize || destPixelPosition.Y >= textureSize )
											Log.Fatal( "Font: UpdateTrueTypeTextures: Pixel position outside the texture." );

										unsafe
										{
											if( useLA )
											{
												byte* source = (byte*)glyphBuffer + y * glyphBufferSize + x;
												byte* pointer = (byte*)pData + destPixelPosition.Y * rowPitch * 2 + destPixelPosition.X * 2;

												//!!!!!!так?
												pointer[ 1 ] = source[ 0 ];
												//byte src = source[ 0 ];
												//pointer[ 0 ] = src;
												//pointer[ 1 ] = (byte)( src != 0 ? 255 : 0 );
											}
											else
											{
												byte* source = (byte*)glyphBuffer + y * glyphBufferSize + x;
												byte* pointer = (byte*)pData + destPixelPosition.Y * rowPitch * 4 + destPixelPosition.X * 4;

												//pointer[ 0 ] = source[ 0 ];
												pointer[ 3 ] = source[ 0 ];
												//pointer[ 1 ] = 0;
												//pointer[ 2 ] = 0;
												//pointer[ 3 ] = 0;

												//!!!!!
												//pointer[ 1 ] = pointer[ 2 ] = source[ 0 ];
											}
										}
									}
								}
							}
						}
					}
					//pixelBuffer.Unlock();

					var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) };
					gpuTexture.SetData( d );
					//pixelBuffer.SetData( data );
				}
			}

			NativeUtility.Free( glyphBuffer );
			glyphBuffer = IntPtr.Zero;
		}

		static int GetTextureSize()
		{
			//!!!!в зависимости от размера выставлять 1024
			return 512;
		}

		//!!!!
		//void RenderSystem_RenderSystemEvent( RenderSystemEvents name )
		//{
		//	if( name == RenderSystemEvents.DeviceRestored )
		//	{
		//		if( trueTypeDefinition != null )
		//		{
		//			foreach( Variant variant in variants.Values )
		//				UpdateTrueTypeTextures( variant );
		//		}
		//	}
		//}

		void DisposeAllCompiledData()
		{
			foreach( var compiledData in compiledDataItems )//.Values )
				compiledData?.Dispose();
			compiledDataItems.Clear();
		}
	}
}
