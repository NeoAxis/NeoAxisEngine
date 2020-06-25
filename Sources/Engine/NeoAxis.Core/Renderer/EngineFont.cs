// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;

namespace NeoAxis
{
	/////////////////////////////////////////////////////////////////////////////////////////////

	struct FreeType
	{
		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_Init", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern IntPtr/*FT_Library*/ Init();

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_Shutdown", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern void Shutdown( IntPtr/*FT_Library*/ library );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_CreateFace", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern IntPtr/*FT_Face*/ CreateFace( IntPtr/*FT_Library*/ library, IntPtr ttfData,
			int ttfDataSize );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_DestroyFace", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public static extern void DestroyFace( IntPtr/*FT_Face*/ face );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_SetPixelSizes", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool SetPixelSizes( IntPtr/*FT_Face*/ face, int sizeX, int sizeY );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_IsGlyphExists", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool IsGlyphExists( IntPtr/*FT_Library*/ library, IntPtr/*FT_Face*/ face, int character );

		[DllImport( OgreWrapper.library, EntryPoint = "FreeType_GetGlyphData", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool GetGlyphData( IntPtr/*FT_Library*/ library, IntPtr/*FT_Face*/ face, int character,
			int bufferSizeX, int bufferSizeY, IntPtr buffer, out int drawOffsetX,
			out int drawOffsetY, out int outSizeX, out int outSizeY, out int advance );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Class representing a font in the system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is simply a way of getting a font textures into the engine system and
	/// to easily retrieve the texture coordinates required to accurately render them.
	/// Fonts can either be loaded from precreated textures, or the texture can be generated
	/// using a truetype font.
	/// </para>
	/// </remarks>
	//[TypeConverter( typeof( FontConverter ) )]
	//class EngineFont //: IDisposable
	//{
	////string name;
	//string language;

	//float height;
	//EngineFontDefinition definition;
	////PrecompiledImageFontDefinition precompiledImageDefinition;
	//TrueTypeFontDefinition trueTypeDefinition;

	////Key: screen size y for true type. "0" for precompiled image or for in game 3d gui.
	//Dictionary<int, Variant> variants = new Dictionary<int, Variant>();
	//Variant lastVariant;

	////true type specific
	//bool[] trueTypeCharactersInitializedFlags;
	//IntPtr ttfFileData;
	//int ttdFileDataSize;
	//IntPtr freeTypeFace;

	///////////////////////////////////////////

	//class Variant
	//{
	//	public EngineFont owner;
	//	public int key;

	//	public Component_Image[] textures;
	//	public int trueTypeFontSizeYInPixels;
	//	public TrueTypeCharacterInfo[] trueTypeCharacters;
	//	public double trueTypeLastUsedTime;
	//}

	///////////////////////////////////////////

	//internal EngineFont( string name, string language, float height, EngineFontDefinition definition )
	//{
	//	//!!!!пока так
	//	EngineThreading.CheckMainThread();

	//	this.name = name;
	//	this.language = language;
	//	this.height = height;
	//	this.definition = definition;

	//	if( definition is PrecompiledImageFontDefinition )
	//	{
	//		precompiledImageDefinition = (PrecompiledImageFontDefinition)definition;
	//	}
	//	else if( definition is TrueTypeFontDefinition )
	//	{
	//		trueTypeDefinition = (TrueTypeFontDefinition)definition;
	//		LoadTTFFileData();
	//	}
	//	else
	//	{
	//		Log.Fatal( "Font: Unknown font definition class." );
	//	}

	//	//if( trueTypeDefinition != null )
	//	//	RenderingSystem.RenderSystemEvent += RenderSystem_RenderSystemEvent;
	//}

	//public EngineFontDefinition Definition
	//{
	//	get { return definition; }
	//}

	///// <summary>Releases the resources that are used by the object.</summary>
	//public void Dispose()
	//{
	//	//!!!!пока так
	//	EngineThreading.CheckMainThread();

	//	if( definition != null )
	//	{
	//		//after shutdown check
	//		if( RendererWorld.Disposed )
	//		{
	//			//waiting for .NET Standard 2.0
	//			Log.Fatal( "Renderer: Dispose after Shutdown." );
	//			//Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );
	//		}

	//		if( trueTypeDefinition != null )
	//			RenderingSystem.RenderSystemEvent -= RenderSystem_RenderSystemEvent;

	//		RemoveAllVariants();

	//		EngineFontManager.Instance.RemoveFont( this );

	//		definition = null;
	//		precompiledImageDefinition = null;
	//		trueTypeDefinition = null;
	//	}

	//	FreeTTFFileData();

	//	//GC.SuppressFinalize( this );
	//}

	//internal bool IsDisposed()
	//{
	//	return definition == null;
	//}

	///// <summary>Gets the font name.</summary>
	//public string Name
	//{
	//	get { return name; }
	//}

	///// <summary>Gets the font language.</summary>
	//public string Language
	//{
	//	get { return language; }
	//}

	///// <summary>Gets the font screen height.</summary>
	//public float Height
	//{
	//	get { return height; }
	//}

	///// <summary>
	///// Returns the font name and height in the form of a string.
	///// </summary>
	///// <returns>The font name and height in the form of a string.</returns>
	//public override string ToString()
	//{
	//	return Name + " " + Height.ToString();
	//}

	//int GetVariantKey( CanvasRenderer forRenderer )
	//{
	//	if( definition.Type == EngineFontDefinition.Types.TrueType )
	//	{
	//		if( forRenderer.ViewportForScreenCanvasRenderer != null )
	//			return forRenderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
	//	}
	//	return 0;
	//}

	//Variant GetVariant( CanvasRenderer forRenderer )
	//{
	//	int key = GetVariantKey( forRenderer );

	//	//use last variant for better performance
	//	if( lastVariant != null && lastVariant.key == key )
	//	{
	//		lastVariant.trueTypeLastUsedTime = EngineApp.EngineTime;//!!!!!!так? RendererWorld.FrameRenderTime;
	//		return lastVariant;
	//	}

	//	Variant variant;
	//	if( !variants.TryGetValue( key, out variant ) )
	//		variant = CreateVariant( forRenderer );

	//	if( variant != null )
	//		variant.trueTypeLastUsedTime = EngineApp.EngineTime;// RendererWorld.FrameRenderTime;

	//	lastVariant = variant;

	//	return variant;
	//}

	//Variant CreateVariant( CanvasRenderer forRenderer )
	//{
	//	int key = GetVariantKey( forRenderer );

	//	Variant variant = new Variant();
	//	variant.owner = this;
	//	variant.key = key;
	//	variants.Add( key, variant );

	//	if( precompiledImageDefinition != null )
	//	{
	//		variant.textures = new Component_Image[ precompiledImageDefinition.TextureNames.Length ];
	//		for( int n = 0; n < variant.textures.Length; n++ )
	//		{
	//			string textureName = precompiledImageDefinition.TextureNames[ n ];

	//			//!!!!!
	//			//precompiledImageDefinition.TextureMipMaps
	//			//их как бы по идее подгружать можно было бы. а вот юзание включать мипмапы или не включать - вопрос другой

	//			//!!!!Wait?
	//			//!!!!выгружается?
	//			variant.textures[ n ] = ResourceManager.LoadResource<Component_Image>( textureName );

	//			//variant.textures[ n ] = TextureManager.Instance.Load( textureName,
	//			//	Texture.Type.Type2D, precompiledImageDefinition.TextureMipMaps ? -1 : 0 );
	//		}
	//	}
	//	if( trueTypeDefinition != null )
	//	{
	//		if( !GenerateTrueTypeFontData( forRenderer, variant ) )
	//		{
	//			variants.Remove( key );
	//			return null;
	//		}
	//	}

	//	return variant;
	//}

	//void RemoveVariant( Variant variant )
	//{
	//	if( trueTypeDefinition != null )
	//	{
	//		if( variant.textures != null )
	//		{
	//			foreach( var texture in variant.textures )
	//			{
	//				if( texture != null )
	//					texture.Dispose();
	//			}
	//		}
	//	}

	//	variants.Remove( variant.key );
	//	lastVariant = null;
	//}

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

	//void RemoveAllVariants()
	//{
	//	again:
	//	foreach( Variant variant in variants.Values )
	//	{
	//		RemoveVariant( variant );
	//		goto again;
	//	}
	//}

	//public void UpdateTrueTypeFromDefinition()
	//{
	//	if( trueTypeDefinition != null )
	//	{
	//		RemoveAllVariants();
	//		FreeTTFFileData();

	//		LoadTTFFileData();
	//	}
	//}

	//!!!!
	//internal void CheckAndRemoveNotNeededVariants()
	//{
	//	if( trueTypeDefinition != null )
	//	{
	//		//again:
	//		foreach( Variant variant in variants.Values )
	//		{
	//			double diff = EngineApp.EngineTime - variant.trueTypeLastUsedTime;
	//			//float diff = RendererWorld.FrameRenderTime - variant.trueTypeLastUsedTime;

	//			//!!!!very strange code

	//			//!!!!need show detailed и наглядную statistics

	//			//!!!!!как менять? может лучше проверять по заниаемому размеру и удалять паоследние?
	//			//!!!!!!!!!!где еще такое?

	//			//!!!!было
	//			//if( diff > 10.0f )
	//			//{
	//			//	bool usingByGuiRenderer = false;
	//			//	{
	//			//		if( variant.textures != null )
	//			//		{
	//			//			foreach( var texture in variant.textures )
	//			//			{
	//			//				lock( RendererWorld.guiRenderers )
	//			//				{
	//			//					foreach( GuiRenderer guiRenderer in RendererWorld.guiRenderers )
	//			//					{
	//			//						if( guiRenderer.IsTextureCurrentlyIsUse( texture ) )
	//			//						{
	//			//							usingByGuiRenderer = true;
	//			//							break;
	//			//						}
	//			//					}
	//			//				}
	//			//				if( usingByGuiRenderer )
	//			//					break;
	//			//			}
	//			//		}
	//			//	}

	//			//	if( !usingByGuiRenderer )
	//			//	{
	//			//		RemoveVariant( variant );
	//			//		goto again;
	//			//	}
	//			//}
	//		}
	//	}
	//}

	//internal bool IsContainsTexture( Component_Image texture )
	//{
	//	foreach( Variant variant in variants.Values )
	//	{
	//		if( variant.textures != null )
	//		{
	//			foreach( var t in variant.textures )
	//			{
	//				if( t == texture )
	//					return true;
	//			}
	//		}
	//	}
	//	return false;
	//}

	//public IList<Component_Image> GetAllLoadedTextures()
	//{
	//	List<Component_Image> list = new List<Component_Image>();

	//	foreach( Variant variant in variants.Values )
	//	{
	//		if( variant.textures != null )
	//			list.AddRange( variant.textures );
	//	}

	//	return list;
	//}

	//}
}
