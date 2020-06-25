// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	//!!!!!names

	/// <summary>
	/// Specifies keyboard key codes.
	/// </summary>
	public enum EKeys
	{
		/// <summary>
		/// No key.
		/// </summary>
		None = 0,

		/// <summary>
		/// The CANCEL key.
		/// </summary>
		Cancel = 3,

		/// <summary>
		/// The BACKSPACE key.
		/// </summary>
		Back = 8,

		/// <summary>
		/// The TAB key.
		/// </summary>
		Tab = 9,

		/// <summary>
		/// The LINEFEED key.
		/// </summary>
		LineFeed = 10,

		/// <summary>
		/// The CLEAR key.
		/// </summary>
		Clear = 12,

		/// <summary>
		/// The ENTER key.
		/// </summary>
		Enter = 13,

		/// <summary>
		/// The RETURN key.
		/// </summary>
		Return = 13,

		/// <summary>
		/// The SHIFT key.
		/// </summary>
		Shift = 16,

		/// <summary>
		/// The CTRL key.
		/// </summary>
		Control = 17,

		/// <summary>
		/// The ALT key.
		/// </summary>
		Menu = 18,

		/// <summary>
		/// The ALT key.
		/// </summary>
		Alt = 18,

		/// <summary>
		/// The PAUSE key.
		/// </summary>
		Pause = 19,

		/// <summary>
		/// The CAPS LOCK key.
		/// </summary>
		CapsLock = 20,

		/// <summary>
		/// The CAPS LOCK key.
		/// </summary>
		Capital = 20,

		/// <summary>
		/// The IME Kana mode key.
		/// </summary>
		KanaMode = 21,

		/// <summary>
		/// The IME Hangul mode key.
		/// </summary>
		HangulMode = 21,

		/// <summary>
		/// The IME Junja mode key.
		/// </summary>
		JunjaMode = 23,

		/// <summary>
		/// The IME final mode key.
		/// </summary>
		FinalMode = 24,

		/// <summary>
		/// The IME Kanji mode key.
		/// </summary>
		KanjiMode = 25,

		/// <summary>
		/// The IME Hanja mode key.
		/// </summary>
		HanjaMode = 25,

		/// <summary>
		/// The ESC key.
		/// </summary>
		Escape = 27,

		/// <summary>
		/// The IME convert key.
		/// </summary>
		IMEConvert = 28,

		/// <summary>
		/// The IME nonconvert key.
		/// </summary>
		IMENonconvert = 29,

		/// <summary>
		/// The IME accept key.
		/// </summary>
		IMEAccept = 30,

		/// <summary>
		/// The IME mode change key.
		/// </summary>
		IMEModeChange = 31,

		/// <summary>
		/// The SPACEBAR key.
		/// </summary>
		Space = 32,

		/// <summary>
		/// The PAGE UP key.
		/// </summary>
		Prior = 33,

		/// <summary>
		/// The PAGE UP key.
		/// </summary>
		PageUp = 33,

		/// <summary>
		/// The PAGE DOWN key.
		/// </summary>
		Next = 34,

		/// <summary>
		/// The PAGE DOWN key.
		/// </summary>
		PageDown = 34,

		/// <summary>
		/// The END key.
		/// </summary>
		End = 35,

		/// <summary>
		/// The HOME key.
		/// </summary>
		Home = 36,

		/// <summary>
		/// The LEFT ARROW key.
		/// </summary>
		Left = 37,

		/// <summary>
		/// The UP ARROW key.
		/// </summary>
		Up = 38,

		/// <summary>
		/// The RIGHT ARROW key.
		/// </summary>
		Right = 39,

		/// <summary>
		/// The DOWN ARROW key.
		/// </summary>
		Down = 40,

		/// <summary>
		/// The SELECT key.
		/// </summary>
		Select = 41,

		/// <summary>
		/// The PRINT key.
		/// </summary>
		Print = 42,

		/// <summary>
		/// The EXECUTE key.
		/// </summary>
		Execute = 43,

		/// <summary>
		/// The PRINT SCREEN key.
		/// </summary>
		PrintScreen = 44,

		/// <summary>
		/// The PRINT SCREEN key.
		/// </summary>
		Snapshot = 44,

		/// <summary>
		/// The INS key.
		/// </summary>
		Insert = 45,

		/// <summary>
		/// The DEL key.
		/// </summary>
		Delete = 46,

		/// <summary>
		/// The HELP key.
		/// </summary>
		Help = 47,

		/// <summary>
		/// The 0 key.
		/// </summary>
		D0 = 48,

		/// <summary>
		/// The 1 key.
		/// </summary>
		D1 = 49,

		/// <summary>
		/// The 2 key.
		/// </summary>
		D2 = 50,

		/// <summary>
		/// The 3 key.
		/// </summary>
		D3 = 51,


		/// <summary>
		/// The 4 key.
		/// </summary>
		D4 = 52,

		/// <summary>
		/// The 5 key.
		/// </summary>
		D5 = 53,

		/// <summary>
		/// The 6 key.
		/// </summary>
		D6 = 54,

		/// <summary>
		/// The 7 key.
		/// </summary>
		D7 = 55,

		/// <summary>
		/// The 8 key.
		/// </summary>
		D8 = 56,

		/// <summary>
		/// The 9 key.
		/// </summary>
		D9 = 57,

		/// <summary>
		/// The A key.
		/// </summary>
		A = 65,

		/// <summary>
		/// The B key.
		/// </summary>
		B = 66,

		/// <summary>
		/// The C key.
		/// </summary>
		C = 67,

		/// <summary>
		/// The D key.
		/// </summary>
		D = 68,

		/// <summary>
		/// The E key.
		/// </summary>
		E = 69,

		/// <summary>
		/// The F key.
		/// </summary>
		F = 70,

		/// <summary>
		/// The G key.
		/// </summary>
		G = 71,

		/// <summary>
		/// The H key.
		/// </summary>
		H = 72,

		/// <summary>
		/// The I key.
		/// </summary>
		I = 73,

		/// <summary>
		/// The J key.
		/// </summary>
		J = 74,

		/// <summary>
		/// The K key.
		/// </summary>
		K = 75,

		/// <summary>
		/// The L key.
		/// </summary>
		L = 76,

		/// <summary>
		/// The M key.
		/// </summary>
		M = 77,

		/// <summary>
		/// The N key.
		/// </summary>
		N = 78,

		/// <summary>
		/// The O key.
		/// </summary>
		O = 79,

		/// <summary>
		/// The P key.
		/// </summary>
		P = 80,

		/// <summary>
		/// The Q key.
		/// </summary>
		Q = 81,

		/// <summary>
		/// The R key.
		/// </summary>
		R = 82,

		/// <summary>
		/// The S key.
		/// </summary>
		S = 83,

		/// <summary>
		/// The T key.
		/// </summary>
		T = 84,

		/// <summary>
		/// The U key.
		/// </summary>
		U = 85,

		/// <summary>
		/// The V key.
		/// </summary>
		V = 86,

		/// <summary>
		/// The W key.
		/// </summary>
		W = 87,

		/// <summary>
		/// The X key.
		/// </summary>
		X = 88,

		/// <summary>
		/// The Y key.
		/// </summary>
		Y = 89,

		/// <summary>
		/// The Z key.
		/// </summary>
		Z = 90,

		/// <summary>
		/// The left Windows logo key (Microsoft Natural Keyboard).
		/// </summary>
		LWin = 91,

		/// <summary>
		/// The right Windows logo key (Microsoft Natural Keyboard).
		/// </summary>
		RWin = 92,

		/// <summary>
		/// The application key (Microsoft Natural Keyboard).
		/// </summary>
		Apps = 93,

		/// <summary>
		/// The computer sleep key.
		/// </summary>
		Sleep = 95,

		/// <summary>
		/// The 0 key on the numeric keypad.
		/// </summary>
		NumPad0 = 96,

		/// <summary>
		/// The 1 key on the numeric keypad.
		/// </summary>
		NumPad1 = 97,

		/// <summary>
		/// The 2 key on the numeric keypad.
		/// </summary>
		NumPad2 = 98,

		/// <summary>
		/// The 3 key on the numeric keypad.
		/// </summary>
		NumPad3 = 99,

		/// <summary>
		/// The 4 key on the numeric keypad.
		/// </summary>
		NumPad4 = 100,

		/// <summary>
		/// The 5 key on the numeric keypad.
		/// </summary>
		NumPad5 = 101,

		/// <summary>
		/// The 6 key on the numeric keypad.
		/// </summary>
		NumPad6 = 102,

		/// <summary>
		/// The 7 key on the numeric keypad.
		/// </summary>
		NumPad7 = 103,

		/// <summary>
		/// The 8 key on the numeric keypad.
		/// </summary>
		NumPad8 = 104,

		/// <summary>
		/// The 9 key on the numeric keypad.
		/// </summary>
		NumPad9 = 105,

		/// <summary>
		/// The multiply key.
		/// </summary>
		Multiply = 106,

		/// <summary>
		/// The add key.
		/// </summary>
		Add = 107,

		/// <summary>
		/// The separator key.
		/// </summary>
		Separator = 108,

		/// <summary>
		/// The subtract key.
		/// </summary>
		Subtract = 109,

		/// <summary>
		/// The decimal key.
		/// </summary>
		Decimal = 110,

		/// <summary>
		/// The divide key.
		/// </summary>
		Divide = 111,

		/// <summary>
		/// The F1 key.
		/// </summary>
		F1 = 112,

		/// <summary>
		/// The F2 key.
		/// </summary>
		F2 = 113,

		/// <summary>
		/// The F3 key.
		/// </summary>
		F3 = 114,

		/// <summary>
		/// The F4 key.
		/// </summary>
		F4 = 115,

		/// <summary>
		/// The F5 key.
		/// </summary>
		F5 = 116,

		/// <summary>
		/// The F6 key.
		/// </summary>
		F6 = 117,

		/// <summary>
		/// The F7 key.
		/// </summary>
		F7 = 118,

		/// <summary>
		/// The F8 key.
		/// </summary>
		F8 = 119,

		/// <summary>
		/// The F9 key.
		/// </summary>
		F9 = 120,

		/// <summary>
		/// The F10 key.
		/// </summary>
		F10 = 121,

		/// <summary>
		/// The F11 key.
		/// </summary>
		F11 = 122,

		/// <summary>
		/// The F12 key.
		/// </summary>
		F12 = 123,

		/// <summary>
		/// The F13 key.
		/// </summary>
		F13 = 124,

		/// <summary>
		/// The F14 key.
		/// </summary>
		F14 = 125,

		/// <summary>
		/// The F15 key.
		/// </summary>
		F15 = 126,

		/// <summary>
		/// The F16 key.
		/// </summary>
		F16 = 127,

		/// <summary>
		/// The F17 key.
		/// </summary>
		F17 = 128,

		/// <summary>
		/// The F18 key.
		/// </summary>
		F18 = 129,

		/// <summary>
		/// The F19 key.
		/// </summary>
		F19 = 130,

		/// <summary>
		/// The F20 key.
		/// </summary>
		F20 = 131,

		/// <summary>
		/// The F21 key.
		/// </summary>
		F21 = 132,

		/// <summary>
		/// The F22 key.
		/// </summary>
		F22 = 133,

		/// <summary>
		/// The F23 key.
		/// </summary>
		F23 = 134,

		/// <summary>
		/// The F24 key.
		/// </summary>
		F24 = 135,

		/// <summary>
		/// The NUM LOCK key.
		/// </summary>
		NumLock = 144,

		/// <summary>
		/// The SCROLL LOCK key.
		/// </summary>
		Scroll = 145,

		/// <summary>
		/// The left SHIFT key.
		/// </summary>
		LShift = 160,

		/// <summary>
		/// The right SHIFT key.
		/// </summary>
		RShift = 161,

		/// <summary>
		/// The left CTRL key.
		/// </summary>
		LControl = 162,

		/// <summary>
		/// The right CTRL key.
		/// </summary>
		RControl = 163,

		/// <summary>
		/// The left ALT key.
		/// </summary>
		LAlt = 164,

		/// <summary>
		/// The left ALT key.
		/// </summary>
		LMenu = 164,

		/// <summary>
		/// The right ALT key.
		/// </summary>
		RAlt = 165,

		/// <summary>
		/// The right ALT key.
		/// </summary>
		RMenu = 165,

		/// <summary>
		/// The browser back key (Windows 2000 or later).
		/// </summary>
		BrowserBack = 166,

		/// <summary>
		/// The browser forward key (Windows 2000 or later).
		/// </summary>
		BrowserForward = 167,

		/// <summary>
		/// The browser refresh key (Windows 2000 or later).
		/// </summary>
		BrowserRefresh = 168,

		/// <summary>
		/// The browser stop key (Windows 2000 or later).
		/// </summary>
		BrowserStop = 169,

		/// <summary>
		/// The browser search key (Windows 2000 or later).
		/// </summary>
		BrowserSearch = 170,

		/// <summary>
		/// The browser favorites key (Windows 2000 or later).
		/// </summary>
		BrowserFavorites = 171,

		/// <summary>
		/// The browser home key (Windows 2000 or later).
		/// </summary>
		BrowserHome = 172,

		/// <summary>
		/// The volume mute key (Windows 2000 or later).
		/// </summary>
		VolumeMute = 173,

		/// <summary>
		/// The volume down key (Windows 2000 or later).
		/// </summary>
		VolumeDown = 174,

		/// <summary>
		/// The volume up key (Windows 2000 or later).
		/// </summary>
		VolumeUp = 175,

		/// <summary>
		/// The media next track key (Windows 2000 or later).
		/// </summary>
		MediaNextTrack = 176,

		/// <summary>
		/// The media previous track key (Windows 2000 or later).
		/// </summary>
		MediaPreviousTrack = 177,

		/// <summary>
		/// The media Stop key (Windows 2000 or later).
		/// </summary>
		MediaStop = 178,

		/// <summary>
		/// The media play pause key (Windows 2000 or later).
		/// </summary>
		MediaPlayPause = 179,

		/// <summary>
		/// The launch mail key (Windows 2000 or later).
		/// </summary>
		LaunchMail = 180,

		/// <summary>
		/// The select media key (Windows 2000 or later).
		/// </summary>
		SelectMedia = 181,

		/// <summary>
		/// The start application one key (Windows 2000 or later).
		/// </summary>
		LaunchApplication1 = 182,

		/// <summary>
		/// The start application two key (Windows 2000 or later).
		/// </summary>
		LaunchApplication2 = 183,

		/// <summary>
		/// The OEM 1 key.
		/// </summary>
		Oem1 = 186,

		/// <summary>
		/// The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemSemicolon = 186,

		/// <summary>
		/// The OEM plus key on any country/region keyboard (Windows 2000 or later).
		/// </summary>
		Oemplus = 187,

		/// <summary>
		/// The OEM comma key on any country/region keyboard (Windows 2000 or later).
		/// </summary>
		Oemcomma = 188,

		/// <summary>
		/// The OEM minus key on any country/region keyboard (Windows 2000 or later).
		/// </summary>
		OemMinus = 189,

		/// <summary>
		/// The OEM period key on any country/region keyboard (Windows 2000 or later).
		/// </summary>
		OemPeriod = 190,

		/// <summary>
		/// The OEM question mark key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemQuestion = 191,

		/// <summary>
		/// The OEM 2 key.
		/// </summary>
		Oem2 = 191,

		/// <summary>
		/// The OEM tilde key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		Oemtilde = 192,

		/// <summary>
		/// The OEM 3 key.
		/// </summary>
		Oem3 = 192,

		/// <summary>
		/// The OEM 4 key.
		/// </summary>
		Oem4 = 219,

		/// <summary>
		/// The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemOpenBrackets = 219,

		/// <summary>
		/// The OEM pipe key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemPipe = 220,

		/// <summary>
		/// The OEM 5 key.
		/// </summary>
		Oem5 = 220,

		/// <summary>
		/// The OEM 6 key.
		/// </summary>
		Oem6 = 221,

		/// <summary>
		/// The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemCloseBrackets = 221,

		/// <summary>
		/// The OEM 7 key.
		/// </summary>
		Oem7 = 222,

		/// <summary>
		/// The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
		/// </summary>
		OemQuotes = 222,

		/// <summary>
		/// The OEM 8 key.
		/// </summary>
		Oem8 = 223,

		/// <summary>
		/// The OEM 102 key.
		/// </summary>
		Oem102 = 226,

		/// <summary>
		/// The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000 or later).
		/// </summary>
		OemBackslash = 226,

		/// <summary>
		/// The PROCESS KEY key.
		/// </summary>
		ProcessKey = 229,

		/// <summary>
		/// The ATTN key.
		/// </summary>
		Attn = 246,

		/// <summary>
		/// The CRSEL key.
		/// </summary>
		Crsel = 247,

		/// <summary>
		/// The EXSEL key.
		/// </summary>
		Exsel = 248,

		/// <summary>
		/// The ERASE EOF key.
		/// </summary>
		EraseEof = 249,

		/// <summary>
		/// The PLAY key.
		/// </summary>
		Play = 250,

		/// <summary>
		/// The ZOOM key.
		/// </summary>
		Zoom = 251,

		/// <summary>
		/// The PA1 key.
		/// </summary>
		Pa1 = 253,

		/// <summary>
		/// The CLEAR key.
		/// </summary>
		OemClear = 254,

		/// <summary>
		/// The paragraph key.
		/// </summary>
		Paragraph = 255,

		/// <summary>
		/// The function key.
		/// </summary>
		Function = 256,

		/// <summary>
		/// The command key.
		/// </summary>
		Command = 257,

		/// <summary>
		/// The left command key.
		/// </summary>
		LCommand = 258,

		/// <summary>
		/// The right command key.
		/// </summary>
		RCommand = 259,

		//Android        

		/// <summary>
		/// The media eject key (Android).
		/// </summary>
		MediaClose = 260,

		/// <summary>
		/// The media eject key (Android).
		/// </summary>
		MediaEject = 261,

		/// <summary>
		/// The media fast forward key (Android).
		/// </summary>
		MediaFastForward = 262,

		/// <summary>
		/// The media pause key (Android).
		/// </summary>
		MediaPause = 263,

		/// <summary>
		/// The media play key (Android).
		/// </summary>
		MediaPlay = 264,

		/// <summary>
		/// The media record key (Android).
		/// </summary>
		MediaRecord = 265,

		/// <summary>
		/// The media rewind key (Android).
		/// </summary>
		MediaRewind = 266,

		/// <summary>
		/// The menu key (Android).
		/// </summary>
		AndroidMenu = 267,

		/// <summary>
		/// The zoom in key (Android).
		/// </summary>
		ZoomIn = 268,

		/// <summary>
		/// The zoom out key (Android).
		/// </summary>
		ZoomOut = 269,

		/// <summary>
		/// The meta left key (Android).
		/// </summary>
		MetaLeft = 270,

		/// <summary>
		/// The meta right key (Android).
		/// </summary>
		MetaRight = 271,

		/// <summary>
		/// The notification key (Android).
		/// </summary>
		Notification = 272,

		/// <summary>
		/// The num key (Android).
		/// </summary>
		AndroidNum = 273,

		/// <summary>
		/// App switch key. Should bring up the application switcher dialog (Android).
		/// </summary>
		ApplicationSwitch = 274,

		/// <summary>
		/// Bookmark key. On some TV remotes, bookmarks content or web pages (Android).
		/// </summary>
		Bookmark = 275,

		/// <summary>
		/// The call key (Android).
		/// </summary>
		Call = 276,

		/// <summary>
		/// The end call key (Android).
		/// </summary>
		EndCall = 277,

		/// <summary>
		/// Explorer special function key. Used to launch a browser application (Android).
		/// </summary>
		Explorer = 278,

		/// <summary>
		/// Camera key. Used to launch a camera application or take pictures (Android).
		/// </summary>
		Camera = 279,

		/// <summary>
		/// Camera Focus key. Used to focus the camera (Android).
		/// </summary>
		Focus = 280,

		/// <summary>
		/// Headset Hook key. Used to hang up calls and stop media (Android).
		/// </summary>
		HeadsetHook = 281,

		/// <summary>
		/// Numeric keypad '=' key (Android).
		/// </summary>
		NumPadEquals = 282,

		/// <summary>
		/// Numeric keypad '(' key (Android).
		/// </summary>
		NumPadLeftParen = 283,

		/// <summary>
		/// Numeric keypad ')' key (Android).
		/// </summary>
		NumPadRightParen = 284,

		/// <summary>
		/// The power key (Android).
		/// </summary>
		Power = 285,

		/// <summary>
		/// The settings key. Starts the system settings activity (Android).
		/// </summary>
		Settings = 286,

		/// <summary>
		/// The soft left key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom left of the display (Android).
		/// </summary>
		SoftLeft = 287,

		/// <summary>
		/// The soft right key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom right of the display (Android).
		/// </summary>
		SoftRight = 288,

		/// <summary>
		/// The symbol modifier key. Used to enter alternate symbols (Android).
		/// </summary>
		Symbol = 289,

		/// <summary>
		/// The picture symbols modifier key. Used to switch symbol sets (Emoji, Kao-moji) (Android).
		/// </summary>
		PictureSymbols = 290,

		/// <summary>
		/// The switch charset modifier key. Used to switch character sets (Kanji, Katakana) (Android).
		/// </summary>
		SwitchCharset = 291,

		/// <summary>
		/// The A/V Receiver input key. On TV remotes, switches the input mode on an external A/V Receiver (Android)
		/// </summary>
		AVRInput = 292,

		/// <summary>
		/// The A/V Receiver power key. On TV remotes, toggles the power on an external A/V Receiver (Android)
		/// </summary>
		AVRPower = 293,

		/// <summary>
		/// The toggle captions key. Switches the mode for closed-captioning text, for example during television shows (Android)
		/// </summary>
		Captions = 294,

		/// <summary>
		/// The channel down key. On TV remotes, decrements the television channel (Android)
		/// </summary>
		ChannelDown = 295,

		/// <summary>
		/// The channel up key. On TV remotes, increments the television channel (Android)
		/// </summary>
		ChannelUp = 296,

		/// <summary>
		/// The DVR key. On some TV remotes, switches to a DVR mode for recorded shows (Android)
		/// </summary>
		DVR = 297,

		/// <summary>
		/// The guide key. On TV remotes, shows a programming guide(Android)
		/// </summary>
		Guide = 298,

		/// <summary>
		/// The info key. Common on TV remotes to show additional information related to what is currently being viewed(Android)
		/// </summary>
		Info = 299,

		/// <summary>
		/// The blue "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
		/// </summary>
		ProgBlue = 300,

		/// <summary>
		/// The green "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
		/// </summary>
		ProgGreen = 301,

		/// <summary>
		/// The red "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
		/// </summary>
		ProgRed = 302,

		/// <summary>
		/// The yellow "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
		/// </summary>
		ProgYellow = 303,

		/// <summary>
		/// The set-top-box input key. On TV remotes, switches the input mode on an external set-top-box (Android)
		/// </summary>
		STBInput = 304,

		/// <summary>
		/// The set-top-box power key. On TV remotes, toggles the power on an external Set-top-box (Android)
		/// </summary>
		STBPower = 305,

		/// <summary>
		/// The TV key. On TV remotes, switches to viewing live TV (Android)
		/// </summary>
		TV = 306,

		/// <summary>
		/// The TV input key. On TV remotes, switches the input on a television screen (Android)
		/// </summary>
		TVInput = 307,

		/// <summary>
		/// The TV power key. On TV remotes, toggles the power on a television screen (Android)
		/// </summary>
		TVPower = 308,

		/// <summary>
		/// The window key. On TV remotes, toggles picture-in-picture mode or other windowing functions (Android)
		/// </summary>
		Window = 309,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public enum EMouseButtons
	{
		/// <summary>
		/// The left mouse button.
		/// </summary>
		Left,

		/// <summary>
		/// The right mouse button.
		/// </summary>
		Right,

		/// <summary>
		/// The middle mouse button.
		/// </summary>
		Middle,

		/// <summary>
		/// The first XButton.
		/// </summary>
		XButton1,

		/// <summary>
		/// The second XButton.
		/// </summary>
		XButton2,
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides data for the key down and key up events.
	/// </summary>
	public class KeyEvent
	{
		EKeys key;
		bool suppressKeyPress;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyEvent"/> class.
		/// </summary>
		/// <param name="key"></param>
		public KeyEvent( EKeys key )
		{
			this.key = key;
		}

		/// <summary>
		/// Gets the keyboard code.
		/// </summary>
		public EKeys Key
		{
			get { return key; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the key event should be passed on
		/// to the underlying control.
		/// </summary>
		public bool SuppressKeyPress
		{
			get { return suppressKeyPress; }
			set { suppressKeyPress = value; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Provides data for the key press event.
	/// </summary>
	public class KeyPressEvent
	{
		char keyChar;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyPressEvent"/> class.
		/// </summary>
		/// <param name="keyChar">
		/// The ASCII character corresponding to the key the user pressed.
		/// </param>
		public KeyPressEvent( char keyChar )
		{
			this.keyChar = keyChar;
		}

		/// <summary>
		/// Gets the character corresponding to the key pressed.
		/// <remarks>
		/// The ASCII character that is composed. For example, if the user presses SHIFT
		/// + K, this property returns an uppercase K.
		/// </remarks>
		/// </summary>
		public char KeyChar
		{
			get { return keyChar; }
		}
	}

}
