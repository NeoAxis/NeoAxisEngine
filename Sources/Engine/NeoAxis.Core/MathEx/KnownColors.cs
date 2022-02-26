// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace NeoAxis
{
	/// <summary>
	/// Provides a set of predefined color values for convenience.
	/// </summary>
	public static class KnownColors
	{
		static KnownColors()
		{
			TransparentBlack = new ColorByte( 0 );
			Transparent = new ColorByte( 0 );
			AliceBlue = new ColorByte( 0xfffff8f0 );
			AntiqueWhite = new ColorByte( 0xffd7ebfa );
			Aqua = new ColorByte( 0xffffff00 );
			Aquamarine = new ColorByte( 0xffd4ff7f );
			Azure = new ColorByte( 0xfffffff0 );
			Beige = new ColorByte( 0xffdcf5f5 );
			Bisque = new ColorByte( 0xffc4e4ff );
			Black = new ColorByte( 0xff000000 );
			BlanchedAlmond = new ColorByte( 0xffcdebff );
			Blue = new ColorByte( 0xffff0000 );
			BlueViolet = new ColorByte( 0xffe22b8a );
			Brown = new ColorByte( 0xff2a2aa5 );
			BurlyWood = new ColorByte( 0xff87b8de );
			CadetBlue = new ColorByte( 0xffa09e5f );
			Chartreuse = new ColorByte( 0xff00ff7f );
			Chocolate = new ColorByte( 0xff1e69d2 );
			Coral = new ColorByte( 0xff507fff );
			CornflowerBlue = new ColorByte( 0xffed9564 );
			Cornsilk = new ColorByte( 0xffdcf8ff );
			Crimson = new ColorByte( 0xff3c14dc );
			Cyan = new ColorByte( 0xffffff00 );
			DarkBlue = new ColorByte( 0xff8b0000 );
			DarkCyan = new ColorByte( 0xff8b8b00 );
			DarkGoldenrod = new ColorByte( 0xff0b86b8 );
			DarkGray = new ColorByte( 0xffa9a9a9 );
			DarkGreen = new ColorByte( 0xff006400 );
			DarkKhaki = new ColorByte( 0xff6bb7bd );
			DarkMagenta = new ColorByte( 0xff8b008b );
			DarkOliveGreen = new ColorByte( 0xff2f6b55 );
			DarkOrange = new ColorByte( 0xff008cff );
			DarkOrchid = new ColorByte( 0xffcc3299 );
			DarkRed = new ColorByte( 0xff00008b );
			DarkSalmon = new ColorByte( 0xff7a96e9 );
			DarkSeaGreen = new ColorByte( 0xff8bbc8f );
			DarkSlateBlue = new ColorByte( 0xff8b3d48 );
			DarkSlateGray = new ColorByte( 0xff4f4f2f );
			DarkTurquoise = new ColorByte( 0xffd1ce00 );
			DarkViolet = new ColorByte( 0xffd30094 );
			DeepPink = new ColorByte( 0xff9314ff );
			DeepSkyBlue = new ColorByte( 0xffffbf00 );
			DimGray = new ColorByte( 0xff696969 );
			DodgerBlue = new ColorByte( 0xffff901e );
			Firebrick = new ColorByte( 0xff2222b2 );
			FloralWhite = new ColorByte( 0xfff0faff );
			ForestGreen = new ColorByte( 0xff228b22 );
			Fuchsia = new ColorByte( 0xffff00ff );
			Gainsboro = new ColorByte( 0xffdcdcdc );
			GhostWhite = new ColorByte( 0xfffff8f8 );
			Gold = new ColorByte( 0xff00d7ff );
			Goldenrod = new ColorByte( 0xff20a5da );
			Gray = new ColorByte( 0xff808080 );
			Green = new ColorByte( 0xff008000 );
			GreenYellow = new ColorByte( 0xff2fffad );
			Honeydew = new ColorByte( 0xfff0fff0 );
			HotPink = new ColorByte( 0xffb469ff );
			IndianRed = new ColorByte( 0xff5c5ccd );
			Indigo = new ColorByte( 0xff82004b );
			Ivory = new ColorByte( 0xfff0ffff );
			Khaki = new ColorByte( 0xff8ce6f0 );
			Lavender = new ColorByte( 0xfffae6e6 );
			LavenderBlush = new ColorByte( 0xfff5f0ff );
			LawnGreen = new ColorByte( 0xff00fc7c );
			LemonChiffon = new ColorByte( 0xffcdfaff );
			LightBlue = new ColorByte( 0xffe6d8ad );
			LightCoral = new ColorByte( 0xff8080f0 );
			LightCyan = new ColorByte( 0xffffffe0 );
			LightGoldenrodYellow = new ColorByte( 0xffd2fafa );
			LightGray = new ColorByte( 0xffd3d3d3 );
			LightGreen = new ColorByte( 0xff90ee90 );
			LightPink = new ColorByte( 0xffc1b6ff );
			LightSalmon = new ColorByte( 0xff7aa0ff );
			LightSeaGreen = new ColorByte( 0xffaab220 );
			LightSkyBlue = new ColorByte( 0xffface87 );
			LightSlateGray = new ColorByte( 0xff998877 );
			LightSteelBlue = new ColorByte( 0xffdec4b0 );
			LightYellow = new ColorByte( 0xffe0ffff );
			Lime = new ColorByte( 0xff00ff00 );
			LimeGreen = new ColorByte( 0xff32cd32 );
			Linen = new ColorByte( 0xffe6f0fa );
			Magenta = new ColorByte( 0xffff00ff );
			Maroon = new ColorByte( 0xff000080 );
			MediumAquamarine = new ColorByte( 0xffaacd66 );
			MediumBlue = new ColorByte( 0xffcd0000 );
			MediumOrchid = new ColorByte( 0xffd355ba );
			MediumPurple = new ColorByte( 0xffdb7093 );
			MediumSeaGreen = new ColorByte( 0xff71b33c );
			MediumSlateBlue = new ColorByte( 0xffee687b );
			MediumSpringGreen = new ColorByte( 0xff9afa00 );
			MediumTurquoise = new ColorByte( 0xffccd148 );
			MediumVioletRed = new ColorByte( 0xff8515c7 );
			MidnightBlue = new ColorByte( 0xff701919 );
			MintCream = new ColorByte( 0xfffafff5 );
			MistyRose = new ColorByte( 0xffe1e4ff );
			Moccasin = new ColorByte( 0xffb5e4ff );
			MonoGameOrange = new ColorByte( 0xff003ce7 );
			NavajoWhite = new ColorByte( 0xffaddeff );
			Navy = new ColorByte( 0xff800000 );
			OldLace = new ColorByte( 0xffe6f5fd );
			Olive = new ColorByte( 0xff008080 );
			OliveDrab = new ColorByte( 0xff238e6b );
			Orange = new ColorByte( 0xff00a5ff );
			OrangeRed = new ColorByte( 0xff0045ff );
			Orchid = new ColorByte( 0xffd670da );
			PaleGoldenrod = new ColorByte( 0xffaae8ee );
			PaleGreen = new ColorByte( 0xff98fb98 );
			PaleTurquoise = new ColorByte( 0xffeeeeaf );
			PaleVioletRed = new ColorByte( 0xff9370db );
			PapayaWhip = new ColorByte( 0xffd5efff );
			PeachPuff = new ColorByte( 0xffb9daff );
			Peru = new ColorByte( 0xff3f85cd );
			Pink = new ColorByte( 0xffcbc0ff );
			Plum = new ColorByte( 0xffdda0dd );
			PowderBlue = new ColorByte( 0xffe6e0b0 );
			Purple = new ColorByte( 0xff800080 );
			Red = new ColorByte( 0xff0000ff );
			RosyBrown = new ColorByte( 0xff8f8fbc );
			RoyalBlue = new ColorByte( 0xffe16941 );
			SaddleBrown = new ColorByte( 0xff13458b );
			Salmon = new ColorByte( 0xff7280fa );
			SandyBrown = new ColorByte( 0xff60a4f4 );
			SeaGreen = new ColorByte( 0xff578b2e );
			SeaShell = new ColorByte( 0xffeef5ff );
			Sienna = new ColorByte( 0xff2d52a0 );
			Silver = new ColorByte( 0xffc0c0c0 );
			SkyBlue = new ColorByte( 0xffebce87 );
			SlateBlue = new ColorByte( 0xffcd5a6a );
			SlateGray = new ColorByte( 0xff908070 );
			Snow = new ColorByte( 0xfffafaff );
			SpringGreen = new ColorByte( 0xff7fff00 );
			SteelBlue = new ColorByte( 0xffb48246 );
			Tan = new ColorByte( 0xff8cb4d2 );
			Teal = new ColorByte( 0xff808000 );
			Thistle = new ColorByte( 0xffd8bfd8 );
			Tomato = new ColorByte( 0xff4763ff );
			Turquoise = new ColorByte( 0xffd0e040 );
			Violet = new ColorByte( 0xffee82ee );
			Wheat = new ColorByte( 0xffb3def5 );
			White = new ColorByte( uint.MaxValue );
			WhiteSmoke = new ColorByte( 0xfff5f5f5 );
			Yellow = new ColorByte( 0xff00ffff );
			YellowGreen = new ColorByte( 0xff32cd9a );
		}

		/// <summary>
		/// TransparentBlack color (R:0,G:0,B:0,A:0).
		/// </summary>
		public static ColorByte TransparentBlack
		{
			get;
			private set;
		}

		/// <summary>
		/// Transparent color (R:0,G:0,B:0,A:0).
		/// </summary>
		public static ColorByte Transparent
		{
			get;
			private set;
		}

		/// <summary>
		/// AliceBlue color (R:240,G:248,B:255,A:255).
		/// </summary>
		public static ColorByte AliceBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// AntiqueWhite color (R:250,G:235,B:215,A:255).
		/// </summary>
		public static ColorByte AntiqueWhite
		{
			get;
			private set;
		}

		/// <summary>
		/// Aqua color (R:0,G:255,B:255,A:255).
		/// </summary>
		public static ColorByte Aqua
		{
			get;
			private set;
		}

		/// <summary>
		/// Aquamarine color (R:127,G:255,B:212,A:255).
		/// </summary>
		public static ColorByte Aquamarine
		{
			get;
			private set;
		}

		/// <summary>
		/// Azure color (R:240,G:255,B:255,A:255).
		/// </summary>
		public static ColorByte Azure
		{
			get;
			private set;
		}

		/// <summary>
		/// Beige color (R:245,G:245,B:220,A:255).
		/// </summary>
		public static ColorByte Beige
		{
			get;
			private set;
		}

		/// <summary>
		/// Bisque color (R:255,G:228,B:196,A:255).
		/// </summary>
		public static ColorByte Bisque
		{
			get;
			private set;
		}

		/// <summary>
		/// Black color (R:0,G:0,B:0,A:255).
		/// </summary>
		public static ColorByte Black
		{
			get;
			private set;
		}

		/// <summary>
		/// BlanchedAlmond color (R:255,G:235,B:205,A:255).
		/// </summary>
		public static ColorByte BlanchedAlmond
		{
			get;
			private set;
		}

		/// <summary>
		/// Blue color (R:0,G:0,B:255,A:255).
		/// </summary>
		public static ColorByte Blue
		{
			get;
			private set;
		}

		/// <summary>
		/// BlueViolet color (R:138,G:43,B:226,A:255).
		/// </summary>
		public static ColorByte BlueViolet
		{
			get;
			private set;
		}

		/// <summary>
		/// Brown color (R:165,G:42,B:42,A:255).
		/// </summary>
		public static ColorByte Brown
		{
			get;
			private set;
		}

		/// <summary>
		/// BurlyWood color (R:222,G:184,B:135,A:255).
		/// </summary>
		public static ColorByte BurlyWood
		{
			get;
			private set;
		}

		/// <summary>
		/// CadetBlue color (R:95,G:158,B:160,A:255).
		/// </summary>
		public static ColorByte CadetBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// Chartreuse color (R:127,G:255,B:0,A:255).
		/// </summary>
		public static ColorByte Chartreuse
		{
			get;
			private set;
		}

		/// <summary>
		/// Chocolate color (R:210,G:105,B:30,A:255).
		/// </summary>
		public static ColorByte Chocolate
		{
			get;
			private set;
		}

		/// <summary>
		/// Coral color (R:255,G:127,B:80,A:255).
		/// </summary>
		public static ColorByte Coral
		{
			get;
			private set;
		}

		/// <summary>
		/// CornflowerBlue color (R:100,G:149,B:237,A:255).
		/// </summary>
		public static ColorByte CornflowerBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// Cornsilk color (R:255,G:248,B:220,A:255).
		/// </summary>
		public static ColorByte Cornsilk
		{
			get;
			private set;
		}

		/// <summary>
		/// Crimson color (R:220,G:20,B:60,A:255).
		/// </summary>
		public static ColorByte Crimson
		{
			get;
			private set;
		}

		/// <summary>
		/// Cyan color (R:0,G:255,B:255,A:255).
		/// </summary>
		public static ColorByte Cyan
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkBlue color (R:0,G:0,B:139,A:255).
		/// </summary>
		public static ColorByte DarkBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkCyan color (R:0,G:139,B:139,A:255).
		/// </summary>
		public static ColorByte DarkCyan
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkGoldenrod color (R:184,G:134,B:11,A:255).
		/// </summary>
		public static ColorByte DarkGoldenrod
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkGray color (R:169,G:169,B:169,A:255).
		/// </summary>
		public static ColorByte DarkGray
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkGreen color (R:0,G:100,B:0,A:255).
		/// </summary>
		public static ColorByte DarkGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkKhaki color (R:189,G:183,B:107,A:255).
		/// </summary>
		public static ColorByte DarkKhaki
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkMagenta color (R:139,G:0,B:139,A:255).
		/// </summary>
		public static ColorByte DarkMagenta
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkOliveGreen color (R:85,G:107,B:47,A:255).
		/// </summary>
		public static ColorByte DarkOliveGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkOrange color (R:255,G:140,B:0,A:255).
		/// </summary>
		public static ColorByte DarkOrange
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkOrchid color (R:153,G:50,B:204,A:255).
		/// </summary>
		public static ColorByte DarkOrchid
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkRed color (R:139,G:0,B:0,A:255).
		/// </summary>
		public static ColorByte DarkRed
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkSalmon color (R:233,G:150,B:122,A:255).
		/// </summary>
		public static ColorByte DarkSalmon
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkSeaGreen color (R:143,G:188,B:139,A:255).
		/// </summary>
		public static ColorByte DarkSeaGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkSlateBlue color (R:72,G:61,B:139,A:255).
		/// </summary>
		public static ColorByte DarkSlateBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkSlateGray color (R:47,G:79,B:79,A:255).
		/// </summary>
		public static ColorByte DarkSlateGray
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkTurquoise color (R:0,G:206,B:209,A:255).
		/// </summary>
		public static ColorByte DarkTurquoise
		{
			get;
			private set;
		}

		/// <summary>
		/// DarkViolet color (R:148,G:0,B:211,A:255).
		/// </summary>
		public static ColorByte DarkViolet
		{
			get;
			private set;
		}

		/// <summary>
		/// DeepPink color (R:255,G:20,B:147,A:255).
		/// </summary>
		public static ColorByte DeepPink
		{
			get;
			private set;
		}

		/// <summary>
		/// DeepSkyBlue color (R:0,G:191,B:255,A:255).
		/// </summary>
		public static ColorByte DeepSkyBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// DimGray color (R:105,G:105,B:105,A:255).
		/// </summary>
		public static ColorByte DimGray
		{
			get;
			private set;
		}

		/// <summary>
		/// DodgerBlue color (R:30,G:144,B:255,A:255).
		/// </summary>
		public static ColorByte DodgerBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// Firebrick color (R:178,G:34,B:34,A:255).
		/// </summary>
		public static ColorByte Firebrick
		{
			get;
			private set;
		}

		/// <summary>
		/// FloralWhite color (R:255,G:250,B:240,A:255).
		/// </summary>
		public static ColorByte FloralWhite
		{
			get;
			private set;
		}

		/// <summary>
		/// ForestGreen color (R:34,G:139,B:34,A:255).
		/// </summary>
		public static ColorByte ForestGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// Fuchsia color (R:255,G:0,B:255,A:255).
		/// </summary>
		public static ColorByte Fuchsia
		{
			get;
			private set;
		}

		/// <summary>
		/// Gainsboro color (R:220,G:220,B:220,A:255).
		/// </summary>
		public static ColorByte Gainsboro
		{
			get;
			private set;
		}

		/// <summary>
		/// GhostWhite color (R:248,G:248,B:255,A:255).
		/// </summary>
		public static ColorByte GhostWhite
		{
			get;
			private set;
		}
		/// <summary>
		/// Gold color (R:255,G:215,B:0,A:255).
		/// </summary>
		public static ColorByte Gold
		{
			get;
			private set;
		}

		/// <summary>
		/// Goldenrod color (R:218,G:165,B:32,A:255).
		/// </summary>
		public static ColorByte Goldenrod
		{
			get;
			private set;
		}

		/// <summary>
		/// Gray color (R:128,G:128,B:128,A:255).
		/// </summary>
		public static ColorByte Gray
		{
			get;
			private set;
		}

		/// <summary>
		/// Green color (R:0,G:128,B:0,A:255).
		/// </summary>
		public static ColorByte Green
		{
			get;
			private set;
		}

		/// <summary>
		/// GreenYellow color (R:173,G:255,B:47,A:255).
		/// </summary>
		public static ColorByte GreenYellow
		{
			get;
			private set;
		}

		/// <summary>
		/// Honeydew color (R:240,G:255,B:240,A:255).
		/// </summary>
		public static ColorByte Honeydew
		{
			get;
			private set;
		}

		/// <summary>
		/// HotPink color (R:255,G:105,B:180,A:255).
		/// </summary>
		public static ColorByte HotPink
		{
			get;
			private set;
		}

		/// <summary>
		/// IndianRed color (R:205,G:92,B:92,A:255).
		/// </summary>
		public static ColorByte IndianRed
		{
			get;
			private set;
		}

		/// <summary>
		/// Indigo color (R:75,G:0,B:130,A:255).
		/// </summary>
		public static ColorByte Indigo
		{
			get;
			private set;
		}

		/// <summary>
		/// Ivory color (R:255,G:255,B:240,A:255).
		/// </summary>
		public static ColorByte Ivory
		{
			get;
			private set;
		}

		/// <summary>
		/// Khaki color (R:240,G:230,B:140,A:255).
		/// </summary>
		public static ColorByte Khaki
		{
			get;
			private set;
		}

		/// <summary>
		/// Lavender color (R:230,G:230,B:250,A:255).
		/// </summary>
		public static ColorByte Lavender
		{
			get;
			private set;
		}

		/// <summary>
		/// LavenderBlush color (R:255,G:240,B:245,A:255).
		/// </summary>
		public static ColorByte LavenderBlush
		{
			get;
			private set;
		}

		/// <summary>
		/// LawnGreen color (R:124,G:252,B:0,A:255).
		/// </summary>
		public static ColorByte LawnGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// LemonChiffon color (R:255,G:250,B:205,A:255).
		/// </summary>
		public static ColorByte LemonChiffon
		{
			get;
			private set;
		}

		/// <summary>
		/// LightBlue color (R:173,G:216,B:230,A:255).
		/// </summary>
		public static ColorByte LightBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// LightCoral color (R:240,G:128,B:128,A:255).
		/// </summary>
		public static ColorByte LightCoral
		{
			get;
			private set;
		}

		/// <summary>
		/// LightCyan color (R:224,G:255,B:255,A:255).
		/// </summary>
		public static ColorByte LightCyan
		{
			get;
			private set;
		}

		/// <summary>
		/// LightGoldenrodYellow color (R:250,G:250,B:210,A:255).
		/// </summary>
		public static ColorByte LightGoldenrodYellow
		{
			get;
			private set;
		}

		/// <summary>
		/// LightGray color (R:211,G:211,B:211,A:255).
		/// </summary>
		public static ColorByte LightGray
		{
			get;
			private set;
		}

		/// <summary>
		/// LightGreen color (R:144,G:238,B:144,A:255).
		/// </summary>
		public static ColorByte LightGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// LightPink color (R:255,G:182,B:193,A:255).
		/// </summary>
		public static ColorByte LightPink
		{
			get;
			private set;
		}

		/// <summary>
		/// LightSalmon color (R:255,G:160,B:122,A:255).
		/// </summary>
		public static ColorByte LightSalmon
		{
			get;
			private set;
		}

		/// <summary>
		/// LightSeaGreen color (R:32,G:178,B:170,A:255).
		/// </summary>
		public static ColorByte LightSeaGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// LightSkyBlue color (R:135,G:206,B:250,A:255).
		/// </summary>
		public static ColorByte LightSkyBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// LightSlateGray color (R:119,G:136,B:153,A:255).
		/// </summary>
		public static ColorByte LightSlateGray
		{
			get;
			private set;
		}

		/// <summary>
		/// LightSteelBlue color (R:176,G:196,B:222,A:255).
		/// </summary>
		public static ColorByte LightSteelBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// LightYellow color (R:255,G:255,B:224,A:255).
		/// </summary>
		public static ColorByte LightYellow
		{
			get;
			private set;
		}

		/// <summary>
		/// Lime color (R:0,G:255,B:0,A:255).
		/// </summary>
		public static ColorByte Lime
		{
			get;
			private set;
		}

		/// <summary>
		/// LimeGreen color (R:50,G:205,B:50,A:255).
		/// </summary>
		public static ColorByte LimeGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// Linen color (R:250,G:240,B:230,A:255).
		/// </summary>
		public static ColorByte Linen
		{
			get;
			private set;
		}

		/// <summary>
		/// Magenta color (R:255,G:0,B:255,A:255).
		/// </summary>
		public static ColorByte Magenta
		{
			get;
			private set;
		}

		/// <summary>
		/// Maroon color (R:128,G:0,B:0,A:255).
		/// </summary>
		public static ColorByte Maroon
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumAquamarine color (R:102,G:205,B:170,A:255).
		/// </summary>
		public static ColorByte MediumAquamarine
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumBlue color (R:0,G:0,B:205,A:255).
		/// </summary>
		public static ColorByte MediumBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumOrchid color (R:186,G:85,B:211,A:255).
		/// </summary>
		public static ColorByte MediumOrchid
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumPurple color (R:147,G:112,B:219,A:255).
		/// </summary>
		public static ColorByte MediumPurple
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumSeaGreen color (R:60,G:179,B:113,A:255).
		/// </summary>
		public static ColorByte MediumSeaGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumSlateBlue color (R:123,G:104,B:238,A:255).
		/// </summary>
		public static ColorByte MediumSlateBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumSpringGreen color (R:0,G:250,B:154,A:255).
		/// </summary>
		public static ColorByte MediumSpringGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumTurquoise color (R:72,G:209,B:204,A:255).
		/// </summary>
		public static ColorByte MediumTurquoise
		{
			get;
			private set;
		}

		/// <summary>
		/// MediumVioletRed color (R:199,G:21,B:133,A:255).
		/// </summary>
		public static ColorByte MediumVioletRed
		{
			get;
			private set;
		}

		/// <summary>
		/// MidnightBlue color (R:25,G:25,B:112,A:255).
		/// </summary>
		public static ColorByte MidnightBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// MintCream color (R:245,G:255,B:250,A:255).
		/// </summary>
		public static ColorByte MintCream
		{
			get;
			private set;
		}

		/// <summary>
		/// MistyRose color (R:255,G:228,B:225,A:255).
		/// </summary>
		public static ColorByte MistyRose
		{
			get;
			private set;
		}

		/// <summary>
		/// Moccasin color (R:255,G:228,B:181,A:255).
		/// </summary>
		public static ColorByte Moccasin
		{
			get;
			private set;
		}

		/// <summary>
		/// MonoGame orange theme color (R:231,G:60,B:0,A:255).
		/// </summary>
		public static ColorByte MonoGameOrange
		{
			get;
			private set;
		}

		/// <summary>
		/// NavajoWhite color (R:255,G:222,B:173,A:255).
		/// </summary>
		public static ColorByte NavajoWhite
		{
			get;
			private set;
		}

		/// <summary>
		/// Navy color (R:0,G:0,B:128,A:255).
		/// </summary>
		public static ColorByte Navy
		{
			get;
			private set;
		}

		/// <summary>
		/// OldLace color (R:253,G:245,B:230,A:255).
		/// </summary>
		public static ColorByte OldLace
		{
			get;
			private set;
		}

		/// <summary>
		/// Olive color (R:128,G:128,B:0,A:255).
		/// </summary>
		public static ColorByte Olive
		{
			get;
			private set;
		}

		/// <summary>
		/// OliveDrab color (R:107,G:142,B:35,A:255).
		/// </summary>
		public static ColorByte OliveDrab
		{
			get;
			private set;
		}

		/// <summary>
		/// Orange color (R:255,G:165,B:0,A:255).
		/// </summary>
		public static ColorByte Orange
		{
			get;
			private set;
		}

		/// <summary>
		/// OrangeRed color (R:255,G:69,B:0,A:255).
		/// </summary>
		public static ColorByte OrangeRed
		{
			get;
			private set;
		}

		/// <summary>
		/// Orchid color (R:218,G:112,B:214,A:255).
		/// </summary>
		public static ColorByte Orchid
		{
			get;
			private set;
		}

		/// <summary>
		/// PaleGoldenrod color (R:238,G:232,B:170,A:255).
		/// </summary>
		public static ColorByte PaleGoldenrod
		{
			get;
			private set;
		}

		/// <summary>
		/// PaleGreen color (R:152,G:251,B:152,A:255).
		/// </summary>
		public static ColorByte PaleGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// PaleTurquoise color (R:175,G:238,B:238,A:255).
		/// </summary>
		public static ColorByte PaleTurquoise
		{
			get;
			private set;
		}
		/// <summary>
		/// PaleVioletRed color (R:219,G:112,B:147,A:255).
		/// </summary>
		public static ColorByte PaleVioletRed
		{
			get;
			private set;
		}

		/// <summary>
		/// PapayaWhip color (R:255,G:239,B:213,A:255).
		/// </summary>
		public static ColorByte PapayaWhip
		{
			get;
			private set;
		}

		/// <summary>
		/// PeachPuff color (R:255,G:218,B:185,A:255).
		/// </summary>
		public static ColorByte PeachPuff
		{
			get;
			private set;
		}

		/// <summary>
		/// Peru color (R:205,G:133,B:63,A:255).
		/// </summary>
		public static ColorByte Peru
		{
			get;
			private set;
		}

		/// <summary>
		/// Pink color (R:255,G:192,B:203,A:255).
		/// </summary>
		public static ColorByte Pink
		{
			get;
			private set;
		}

		/// <summary>
		/// Plum color (R:221,G:160,B:221,A:255).
		/// </summary>
		public static ColorByte Plum
		{
			get;
			private set;
		}

		/// <summary>
		/// PowderBlue color (R:176,G:224,B:230,A:255).
		/// </summary>
		public static ColorByte PowderBlue
		{
			get;
			private set;
		}

		/// <summary>
		///  Purple color (R:128,G:0,B:128,A:255).
		/// </summary>
		public static ColorByte Purple
		{
			get;
			private set;
		}

		/// <summary>
		/// Red color (R:255,G:0,B:0,A:255).
		/// </summary>
		public static ColorByte Red
		{
			get;
			private set;
		}

		/// <summary>
		/// RosyBrown color (R:188,G:143,B:143,A:255).
		/// </summary>
		public static ColorByte RosyBrown
		{
			get;
			private set;
		}

		/// <summary>
		/// RoyalBlue color (R:65,G:105,B:225,A:255).
		/// </summary>
		public static ColorByte RoyalBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// SaddleBrown color (R:139,G:69,B:19,A:255).
		/// </summary>
		public static ColorByte SaddleBrown
		{
			get;
			private set;
		}

		/// <summary>
		/// Salmon color (R:250,G:128,B:114,A:255).
		/// </summary>
		public static ColorByte Salmon
		{
			get;
			private set;
		}

		/// <summary>
		/// SandyBrown color (R:244,G:164,B:96,A:255).
		/// </summary>
		public static ColorByte SandyBrown
		{
			get;
			private set;
		}

		/// <summary>
		/// SeaGreen color (R:46,G:139,B:87,A:255).
		/// </summary>
		public static ColorByte SeaGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// SeaShell color (R:255,G:245,B:238,A:255).
		/// </summary>
		public static ColorByte SeaShell
		{
			get;
			private set;
		}

		/// <summary>
		/// Sienna color (R:160,G:82,B:45,A:255).
		/// </summary>
		public static ColorByte Sienna
		{
			get;
			private set;
		}

		/// <summary>
		/// Silver color (R:192,G:192,B:192,A:255).
		/// </summary>
		public static ColorByte Silver
		{
			get;
			private set;
		}

		/// <summary>
		/// SkyBlue color (R:135,G:206,B:235,A:255).
		/// </summary>
		public static ColorByte SkyBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// SlateBlue color (R:106,G:90,B:205,A:255).
		/// </summary>
		public static ColorByte SlateBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// SlateGray color (R:112,G:128,B:144,A:255).
		/// </summary>
		public static ColorByte SlateGray
		{
			get;
			private set;
		}

		/// <summary>
		/// Snow color (R:255,G:250,B:250,A:255).
		/// </summary>
		public static ColorByte Snow
		{
			get;
			private set;
		}

		/// <summary>
		/// SpringGreen color (R:0,G:255,B:127,A:255).
		/// </summary>
		public static ColorByte SpringGreen
		{
			get;
			private set;
		}

		/// <summary>
		/// SteelBlue color (R:70,G:130,B:180,A:255).
		/// </summary>
		public static ColorByte SteelBlue
		{
			get;
			private set;
		}

		/// <summary>
		/// Tan color (R:210,G:180,B:140,A:255).
		/// </summary>
		public static ColorByte Tan
		{
			get;
			private set;
		}

		/// <summary>
		/// Teal color (R:0,G:128,B:128,A:255).
		/// </summary>
		public static ColorByte Teal
		{
			get;
			private set;
		}

		/// <summary>
		/// Thistle color (R:216,G:191,B:216,A:255).
		/// </summary>
		public static ColorByte Thistle
		{
			get;
			private set;
		}

		/// <summary>
		/// Tomato color (R:255,G:99,B:71,A:255).
		/// </summary>
		public static ColorByte Tomato
		{
			get;
			private set;
		}

		/// <summary>
		/// Turquoise color (R:64,G:224,B:208,A:255).
		/// </summary>
		public static ColorByte Turquoise
		{
			get;
			private set;
		}

		/// <summary>
		/// Violet color (R:238,G:130,B:238,A:255).
		/// </summary>
		public static ColorByte Violet
		{
			get;
			private set;
		}

		/// <summary>
		/// Wheat color (R:245,G:222,B:179,A:255).
		/// </summary>
		public static ColorByte Wheat
		{
			get;
			private set;
		}

		/// <summary>
		/// White color (R:255,G:255,B:255,A:255).
		/// </summary>
		public static ColorByte White
		{
			get;
			private set;
		}

		/// <summary>
		/// WhiteSmoke color (R:245,G:245,B:245,A:255).
		/// </summary>
		public static ColorByte WhiteSmoke
		{
			get;
			private set;
		}

		/// <summary>
		/// Yellow color (R:255,G:255,B:0,A:255).
		/// </summary>
		public static ColorByte Yellow
		{
			get;
			private set;
		}

		/// <summary>
		/// YellowGreen color (R:154,G:205,B:50,A:255).
		/// </summary>
		public static ColorByte YellowGreen
		{
			get;
			private set;
		}
	}
}
