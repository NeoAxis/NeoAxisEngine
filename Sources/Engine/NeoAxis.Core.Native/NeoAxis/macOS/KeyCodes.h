// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

enum EKeys
{
	/// <summary>
	/// The CANCEL key.
	/// </summary>
	EKeys_Cancel = 3,
	
	/// <summary>
	/// The BACKSPACE key.
	/// </summary>
	EKeys_Back = 8,
	
	/// <summary>
	/// The TAB key.
	/// </summary>
	EKeys_Tab = 9,
	
	/// <summary>
	/// The LINEFEED key.
	/// </summary>
	EKeys_LineFeed = 10,
	
	/// <summary>
	/// The CLEAR key.
	/// </summary>
	EKeys_Clear = 12,
	
	/// <summary>
	/// The ENTER key.
	/// </summary>
	EKeys_Enter = 13,
	
	/// <summary>
	/// The RETURN key.
	/// </summary>
	EKeys_Return = 13,
	
	/// <summary>
	/// The SHIFT key.
	/// </summary>
	EKeys_Shift = 16,
	
	/// <summary>
	/// The CTRL key.
	/// </summary>
	EKeys_Control = 17,
	
	/// <summary>
	/// The ALT key.
	/// </summary>
	EKeys_Menu = 18,
	
	/// <summary>
	/// The ALT key.
	/// </summary>
	EKeys_Alt = 18,
	
	/// <summary>
	/// The PAUSE key.
	/// </summary>
	EKeys_Pause = 19,
	
	/// <summary>
	/// The CAPS LOCK key.
	/// </summary>
	EKeys_CapsLock = 20,
	
	/// <summary>
	/// The CAPS LOCK key.
	/// </summary>
	EKeys_Capital = 20,
	
	/// <summary>
	/// The IME Kana mode key.
	/// </summary>
	EKeys_KanaMode = 21,
	
	/// <summary>
	/// The IME Hangul mode key.
	/// </summary>
	EKeys_HangulMode = 21,
	
	/// <summary>
	/// The IME Junja mode key.
	/// </summary>
	EKeys_JunjaMode = 23,
	
	/// <summary>
	/// The IME final mode key.
	/// </summary>
	EKeys_FinalMode = 24,
	
	/// <summary>
	/// The IME Kanji mode key.
	/// </summary>
	EKeys_KanjiMode = 25,
	
	/// <summary>
	/// The IME Hanja mode key.
	/// </summary>
	EKeys_HanjaMode = 25,
	
	/// <summary>
	/// The ESC key.
	/// </summary>
	EKeys_Escape = 27,
	
	/// <summary>
	/// The IME convert key.
	/// </summary>
	EKeys_IMEConvert = 28,
	
	/// <summary>
	/// The IME nonconvert key.
	/// </summary>
	EKeys_IMENonconvert = 29,
	
	/// <summary>
	/// The IME accept key.
	/// </summary>
	EKeys_IMEAccept = 30,
	
	/// <summary>
	/// The IME mode change key.
	/// </summary>
	EKeys_IMEModeChange = 31,
	
	/// <summary>
	/// The SPACEBAR key.
	/// </summary>
	EKeys_Space = 32,
	
	/// <summary>
	/// The PAGE UP key.
	/// </summary>
	EKeys_Prior = 33,
	
	/// <summary>
	/// The PAGE UP key.
	/// </summary>
	EKeys_PageUp = 33,
	
	/// <summary>
	/// The PAGE DOWN key.
	/// </summary>
	EKeys_Next = 34,
	
	/// <summary>
	/// The PAGE DOWN key.
	/// </summary>
	EKeys_PageDown = 34,
	
	/// <summary>
	/// The END key.
	/// </summary>
	EKeys_End = 35,
	
	/// <summary>
	/// The HOME key.
	/// </summary>
	EKeys_Home = 36,
	
	/// <summary>
	/// The LEFT ARROW key.
	/// </summary>
	EKeys_KeyLeft = 37,
	
	/// <summary>
	/// The UP ARROW key.
	/// </summary>
	EKeys_KeyUp = 38,
	
	/// <summary>
	/// The RIGHT ARROW key.
	/// </summary>
	EKeys_KeyRight = 39,
	
	/// <summary>
	/// The DOWN ARROW key.
	/// </summary>
	EKeys_KeyDown = 40,
	
	/// <summary>
	/// The SELECT key.
	/// </summary>
	EKeys_Select = 41,
	
	/// <summary>
	/// The PRINT key.
	/// </summary>
	EKeys_Print = 42,
	
	/// <summary>
	/// The EXECUTE key.
	/// </summary>
	EKeys_Execute = 43,
	
	/// <summary>
	/// The PRINT SCREEN key.
	/// </summary>
	EKeys_PrintScreen = 44,
	
	/// <summary>
	/// The PRINT SCREEN key.
	/// </summary>
	EKeys_Snapshot = 44,
	
	/// <summary>
	/// The INS key.
	/// </summary>
	EKeys_Insert = 45,
	
	/// <summary>
	/// The DEL key.
	/// </summary>
	EKeys_Delete = 46,
	
	/// <summary>
	/// The HELP key.
	/// </summary>
	EKeys_Help = 47,
	
	/// <summary>
	/// The 0 key.
	/// </summary>
	EKeys_D0 = 48,
	
	/// <summary>
	/// The 1 key.
	/// </summary>
	EKeys_D1 = 49,
	
	/// <summary>
	/// The 2 key.
	/// </summary>
	EKeys_D2 = 50,
	
	/// <summary>
	/// The 3 key.
	/// </summary>
	EKeys_D3 = 51,
	
	
	/// <summary>
	/// The 4 key.
	/// </summary>
	EKeys_D4 = 52,
	
	/// <summary>
	/// The 5 key.
	/// </summary>
	EKeys_D5 = 53,
	
	/// <summary>
	/// The 6 key.
	/// </summary>
	EKeys_D6 = 54,
	
	/// <summary>
	/// The 7 key.
	/// </summary>
	EKeys_D7 = 55,
	
	/// <summary>
	/// The 8 key.
	/// </summary>
	EKeys_D8 = 56,
	
	/// <summary>
	/// The 9 key.
	/// </summary>
	EKeys_D9 = 57,
	
	/// <summary>
	/// The A key.
	/// </summary>
	EKeys_A = 65,
	
	/// <summary>
	/// The B key.
	/// </summary>
	EKeys_B = 66,
	
	/// <summary>
	/// The C key.
	/// </summary>
	EKeys_C = 67,
	
	/// <summary>
	/// The D key.
	/// </summary>
	EKeys_D = 68,
	
	/// <summary>
	/// The E key.
	/// </summary>
	EKeys_E = 69,
	
	/// <summary>
	/// The F key.
	/// </summary>
	EKeys_F = 70,
	
	/// <summary>
	/// The G key.
	/// </summary>
	EKeys_G = 71,
	
	/// <summary>
	/// The H key.
	/// </summary>
	EKeys_H = 72,
	
	/// <summary>
	/// The I key.
	/// </summary>
	EKeys_I = 73,
	
	/// <summary>
	/// The J key.
	/// </summary>
	EKeys_J = 74,
	
	/// <summary>
	/// The K key.
	/// </summary>
	EKeys_K = 75,
	
	/// <summary>
	/// The L key.
	/// </summary>
	EKeys_L = 76,
	
	/// <summary>
	/// The M key.
	/// </summary>
	EKeys_M = 77,
	
	/// <summary>
	/// The N key.
	/// </summary>
	EKeys_N = 78,
	
	/// <summary>
	/// The O key.
	/// </summary>
	EKeys_O = 79,
	
	/// <summary>
	/// The P key.
	/// </summary>
	EKeys_P = 80,
	
	/// <summary>
	/// The Q key.
	/// </summary>
	EKeys_Q = 81,
	
	/// <summary>
	/// The R key.
	/// </summary>
	EKeys_R = 82,
	
	/// <summary>
	/// The S key.
	/// </summary>
	EKeys_S = 83,
	
	/// <summary>
	/// The T key.
	/// </summary>
	EKeys_T = 84,
	
	/// <summary>
	/// The U key.
	/// </summary>
	EKeys_U = 85,
	
	/// <summary>
	/// The V key.
	/// </summary>
	EKeys_V = 86,
	
	/// <summary>
	/// The W key.
	/// </summary>
	EKeys_W = 87,
	
	/// <summary>
	/// The X key.
	/// </summary>
	EKeys_X = 88,
	
	/// <summary>
	/// The Y key.
	/// </summary>
	EKeys_Y = 89,
	
	/// <summary>
	/// The Z key.
	/// </summary>
	EKeys_Z = 90,
	
	/// <summary>
	/// The left Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	EKeys_LWin = 91,
	
	/// <summary>
	/// The right Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	EKeys_RWin = 92,
	
	/// <summary>
	/// The application key (Microsoft Natural Keyboard).
	/// </summary>
	EKeys_Apps = 93,
	
	/// <summary>
	/// The computer sleep key.
	/// </summary>
	EKeys_Sleep = 95,
	
	/// <summary>
	/// The 0 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad0 = 96,
	
	/// <summary>
	/// The 1 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad1 = 97,
	
	/// <summary>
	/// The 2 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad2 = 98,
	
	/// <summary>
	/// The 3 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad3 = 99,
	
	/// <summary>
	/// The 4 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad4 = 100,
	
	/// <summary>
	/// The 5 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad5 = 101,
	
	/// <summary>
	/// The 6 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad6 = 102,
	
	/// <summary>
	/// The 7 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad7 = 103,
	
	/// <summary>
	/// The 8 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad8 = 104,
	
	/// <summary>
	/// The 9 key on the numeric keypad.
	/// </summary>
	EKeys_NumPad9 = 105,
	
	/// <summary>
	/// The multiply key.
	/// </summary>
	EKeys_Multiply = 106,
	
	/// <summary>
	/// The add key.
	/// </summary>
	EKeys_Add = 107,
	
	/// <summary>
	/// The separator key.
	/// </summary>
	EKeys_Separator = 108,
	
	/// <summary>
	/// The subtract key.
	/// </summary>
	EKeys_Subtract = 109,
	
	/// <summary>
	/// The decimal key.
	/// </summary>
	EKeys_Decimal = 110,
	
	/// <summary>
	/// The divide key.
	/// </summary>
	EKeys_Divide = 111,
	
	/// <summary>
	/// The F1 key.
	/// </summary>
	EKeys_F1 = 112,
	
	/// <summary>
	/// The F2 key.
	/// </summary>
	EKeys_F2 = 113,
	
	/// <summary>
	/// The F3 key.
	/// </summary>
	EKeys_F3 = 114,
	
	/// <summary>
	/// The F4 key.
	/// </summary>
	EKeys_F4 = 115,
	
	/// <summary>
	/// The F5 key.
	/// </summary>
	EKeys_F5 = 116,
	
	/// <summary>
	/// The F6 key.
	/// </summary>
	EKeys_F6 = 117,
	
	/// <summary>
	/// The F7 key.
	/// </summary>
	EKeys_F7 = 118,
	
	/// <summary>
	/// The F8 key.
	/// </summary>
	EKeys_F8 = 119,
	
	/// <summary>
	/// The F9 key.
	/// </summary>
	EKeys_F9 = 120,
	
	/// <summary>
	/// The F10 key.
	/// </summary>
	EKeys_F10 = 121,
	
	/// <summary>
	/// The F11 key.
	/// </summary>
	EKeys_F11 = 122,
	
	/// <summary>
	/// The F12 key.
	/// </summary>
	EKeys_F12 = 123,
	
	/// <summary>
	/// The F13 key.
	/// </summary>
	EKeys_F13 = 124,
	
	/// <summary>
	/// The F14 key.
	/// </summary>
	EKeys_F14 = 125,
	
	/// <summary>
	/// The F15 key.
	/// </summary>
	EKeys_F15 = 126,
	
	/// <summary>
	/// The F16 key.
	/// </summary>
	EKeys_F16 = 127,
	
	/// <summary>
	/// The F17 key.
	/// </summary>
	EKeys_F17 = 128,
	
	/// <summary>
	/// The F18 key.
	/// </summary>
	EKeys_F18 = 129,
	
	/// <summary>
	/// The F19 key.
	/// </summary>
	EKeys_F19 = 130,
	
	/// <summary>
	/// The F20 key.
	/// </summary>
	EKeys_F20 = 131,
	
	/// <summary>
	/// The F21 key.
	/// </summary>
	EKeys_F21 = 132,
	
	/// <summary>
	/// The F22 key.
	/// </summary>
	EKeys_F22 = 133,
	
	/// <summary>
	/// The F23 key.
	/// </summary>
	EKeys_F23 = 134,
	
	/// <summary>
	/// The F24 key.
	/// </summary>
	EKeys_F24 = 135,
	
	/// <summary>
	/// The NUM LOCK key.
	/// </summary>
	EKeys_NumLock = 144,
	
	/// <summary>
	/// The SCROLL LOCK key.
	/// </summary>
	EKeys_Scroll = 145,
	
	/// <summary>
	/// The left SHIFT key.
	/// </summary>
	EKeys_LShift = 160,
	
	/// <summary>
	/// The right SHIFT key.
	/// </summary>
	EKeys_RShift = 161,
	
	/// <summary>
	/// The left CTRL key.
	/// </summary>
	EKeys_LControl = 162,
	
	/// <summary>
	/// The right CTRL key.
	/// </summary>
	EKeys_RControl = 163,
	
	/// <summary>
	/// The left ALT key.
	/// </summary>
	EKeys_LAlt = 164,
	
	/// <summary>
	/// The left ALT key.
	/// </summary>
	EKeys_LMenu = 164,
	
	/// <summary>
	/// The right ALT key.
	/// </summary>
	EKeys_RAlt = 165,
	
	/// <summary>
	/// The right ALT key.
	/// </summary>
	EKeys_RMenu = 165,
	
	/// <summary>
	/// The browser back key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserBack = 166,
	
	/// <summary>
	/// The browser forward key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserForward = 167,
	
	/// <summary>
	/// The browser refresh key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserRefresh = 168,
	
	/// <summary>
	/// The browser stop key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserStop = 169,
	
	/// <summary>
	/// The browser search key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserSearch = 170,
	
	/// <summary>
	/// The browser favorites key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserFavorites = 171,
	
	/// <summary>
	/// The browser home key (Windows 2000 or later).
	/// </summary>
	EKeys_BrowserHome = 172,
	
	/// <summary>
	/// The volume mute key (Windows 2000 or later).
	/// </summary>
	EKeys_VolumeMute = 173,
	
	/// <summary>
	/// The volume down key (Windows 2000 or later).
	/// </summary>
	EKeys_VolumeDown = 174,
	
	/// <summary>
	/// The volume up key (Windows 2000 or later).
	/// </summary>
	EKeys_VolumeUp = 175,
	
	/// <summary>
	/// The media next track key (Windows 2000 or later).
	/// </summary>
	EKeys_MediaNextTrack = 176,
	
	/// <summary>
	/// The media previous track key (Windows 2000 or later).
	/// </summary>
	EKeys_MediaPreviousTrack = 177,
	
	/// <summary>
	/// The media Stop key (Windows 2000 or later).
	/// </summary>
	EKeys_MediaStop = 178,
	
	/// <summary>
	/// The media play pause key (Windows 2000 or later).
	/// </summary>
	EKeys_MediaPlayPause = 179,
	
	/// <summary>
	/// The launch mail key (Windows 2000 or later).
	/// </summary>
	EKeys_LaunchMail = 180,
	
	/// <summary>
	/// The select media key (Windows 2000 or later).
	/// </summary>
	EKeys_SelectMedia = 181,
	
	/// <summary>
	/// The start application one key (Windows 2000 or later).
	/// </summary>
	EKeys_LaunchApplication1 = 182,
	
	/// <summary>
	/// The start application two key (Windows 2000 or later).
	/// </summary>
	EKeys_LaunchApplication2 = 183,
	
	/// <summary>
	/// The OEM 1 key.
	/// </summary>
	EKeys_Oem1 = 186,
	
	/// <summary>
	/// The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemSemicolon = 186,
	
	/// <summary>
	/// The OEM plus key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_Oemplus = 187,
	
	/// <summary>
	/// The OEM comma key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_Oemcomma = 188,
	
	/// <summary>
	/// The OEM minus key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemMinus = 189,
	
	/// <summary>
	/// The OEM period key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemPeriod = 190,
	
	/// <summary>
	/// The OEM question mark key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemQuestion = 191,
	
	/// <summary>
	/// The OEM 2 key.
	/// </summary>
	EKeys_Oem2 = 191,
	
	/// <summary>
	/// The OEM tilde key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_Oemtilde = 192,
	
	/// <summary>
	/// The OEM 3 key.
	/// </summary>
	EKeys_Oem3 = 192,
	
	/// <summary>
	/// The OEM 4 key.
	/// </summary>
	EKeys_Oem4 = 219,
	
	/// <summary>
	/// The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemOpenBrackets = 219,
	
	/// <summary>
	/// The OEM pipe key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemPipe = 220,
	
	/// <summary>
	/// The OEM 5 key.
	/// </summary>
	EKeys_Oem5 = 220,
	
	/// <summary>
	/// The OEM 6 key.
	/// </summary>
	EKeys_Oem6 = 221,
	
	/// <summary>
	/// The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemCloseBrackets = 221,
	
	/// <summary>
	/// The OEM 7 key.
	/// </summary>
	EKeys_Oem7 = 222,
	
	/// <summary>
	/// The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemQuotes = 222,
	
	/// <summary>
	/// The OEM 8 key.
	/// </summary>
	EKeys_Oem8 = 223,
	
	/// <summary>
	/// The OEM 102 key.
	/// </summary>
	EKeys_Oem102 = 226,
	
	/// <summary>
	/// The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000 or later).
	/// </summary>
	EKeys_OemBackslash = 226,
	
	/// <summary>
	/// The PROCESS KEY key.
	/// </summary>
	EKeys_ProcessKey = 229,
	
	/// <summary>
	/// The ATTN key.
	/// </summary>
	EKeys_Attn = 246,
	
	/// <summary>
	/// The CRSEL key.
	/// </summary>
	EKeys_Crsel = 247,
	
	/// <summary>
	/// The EXSEL key.
	/// </summary>
	EKeys_Exsel = 248,
	
	/// <summary>
	/// The ERASE EOF key.
	/// </summary>
	EKeys_EraseEof = 249,
	
	/// <summary>
	/// The PLAY key.
	/// </summary>
	EKeys_Play = 250,
	
	/// <summary>
	/// The ZOOM key.
	/// </summary>
	EKeys_Zoom = 251,
	
	/// <summary>
	/// The PA1 key.
	/// </summary>
	EKeys_Pa1 = 253,
	
	/// <summary>
	/// The CLEAR key.
	/// </summary>
	EKeys_OemClear = 254,
	
	/// <summary>
	/// The paragraph key.
	/// </summary>
	EKeys_Paragraph = 255,
	
	/// <summary>
	/// The function key.
	/// </summary>
	EKeys_Function = 256,

	/// <summary>
	/// The command key.
	/// </summary>
	EKeys_Command = 257,

	/// <summary>
	/// The left command key.
	/// </summary>
	EKeys_LCommand = 258,

	/// <summary>
	/// The right command key.
	/// </summary>
	EKeys_RCommand = 259,


	//Android start

	/// <summary>
	/// The media eject key (Android).
	/// </summary>
	EKeys_MediaClose = 260,

	/// <summary>
	/// The media eject key (Android).
	/// </summary>
	EKeys_MediaEject = 261,

	/// <summary>
	/// The media fast forward key (Android).
	/// </summary>
	EKeys_MediaFastForward = 262,

	/// <summary>
	/// The media pause key (Android).
	/// </summary>
	EKeys_MediaPause = 263,

	/// <summary>
	/// The media play key (Android).
	/// </summary>
	EKeys_MediaPlay = 264,

	/// <summary>
	/// The media record key (Android).
	/// </summary>
	EKeys_MediaRecord = 265,

	/// <summary>
	/// The media rewind key (Android).
	/// </summary>
	EKeys_MediaRewind = 266,

	/// <summary>
	/// The menu key (Android).
	/// </summary>
	EKeys_AndroidMenu = 267,

	/// <summary>
	/// The zoom in key (Android).
	/// </summary>
	EKeys_ZoomIn = 268,

	/// <summary>
	/// The zoom out key (Android).
	/// </summary>
	EKeys_ZoomOut = 269,

	/// <summary>
	/// The meta left key (Android).
	/// </summary>
	EKeys_MetaLeft = 270,

	/// <summary>
	/// The meta right key (Android).
	/// </summary>
	EKeys_MetaRight = 271,

	/// <summary>
	/// The notification key (Android).
	/// </summary>
	EKeys_Notification = 272,

	/// <summary>
	/// The num key (Android).
	/// </summary>
	EKeys_AndroidNum = 273,

	/// <summary>
	/// App switch key. Should bring up the application switcher dialog (Android).
	/// </summary>
	EKeys_ApplicationSwitch = 274,

	/// <summary>
	/// Bookmark key. On some TV remotes, bookmarks content or web pages (Android).
	/// </summary>
	EKeys_Bookmark = 275,

	/// <summary>
	/// The call key (Android).
	/// </summary>
	EKeys_Call = 276,

	/// <summary>
	/// The end call key (Android).
	/// </summary>
	EKeys_EndCall = 277,

	/// <summary>
	/// Explorer special function key. Used to launch a browser application (Android).
	/// </summary>
	EKeys_Explorer = 278,

	/// <summary>
	/// Camera key. Used to launch a camera application or take pictures (Android).
	/// </summary>
	EKeys_Camera = 279,

	/// <summary>
	/// Camera Focus key. Used to focus the camera (Android).
	/// </summary>
	EKeys_Focus = 280,

	/// <summary>
	/// Headset Hook key. Used to hang up calls and stop media (Android).
	/// </summary>
	EKeys_HeadsetHook = 281,

	/// <summary>
	/// Numeric keypad '=' key (Android).
	/// </summary>
	EKeys_NumPadEquals = 282,

	/// <summary>
	/// Numeric keypad '(' key (Android).
	/// </summary>
	EKeys_NumPadLeftParen = 283,

	/// <summary>
	/// Numeric keypad ')' key (Android).
	/// </summary>
	EKeys_NumPadRightParen = 284,

	/// <summary>
	/// The power key (Android).
	/// </summary>
	EKeys_Power = 285,

	/// <summary>
	/// The settings key. Starts the system settings activity (Android).
	/// </summary>
	EKeys_Settings = 286,

	/// <summary>
	/// The soft left key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom left of the display (Android).
	/// </summary>
	EKeys_SoftLeft = 287,

	/// <summary>
	/// The soft right key. Usually situated below the display on phones and used as a multi-function feature key for selecting a software defined function shown on the bottom right of the display (Android).
	/// </summary>
	EKeys_SoftRight = 288,

	/// <summary>
	/// The symbol modifier key. Used to enter alternate symbols (Android).
	/// </summary>
	EKeys_Symbol = 289,

	/// <summary>
	/// The picture symbols modifier key. Used to switch symbol sets (Emoji, Kao-moji) (Android).
	/// </summary>
	EKeys_PictureSymbols = 290,

	/// <summary>
	/// The switch charset modifier key. Used to switch character sets (Kanji, Katakana) (Android).
	/// </summary>
	EKeys_SwitchCharset = 291,

	/// <summary>
	/// The A/V Receiver input key. On TV remotes, switches the input mode on an external A/V Receiver (Android)
	/// </summary>
	EKeys_AVRInput = 292,

	/// <summary>
	/// The A/V Receiver power key. On TV remotes, toggles the power on an external A/V Receiver (Android)
	/// </summary>
	EKeys_AVRPower = 293,

	/// <summary>
	/// The toggle captions key. Switches the mode for closed-captioning text, for example during television shows (Android)
	/// </summary>
	EKeys_Captions = 294,

	/// <summary>
	/// The channel down key. On TV remotes, decrements the television channel (Android)
	/// </summary>
	EKeys_ChannelDown = 295,

	/// <summary>
	/// The channel up key. On TV remotes, increments the television channel (Android)
	/// </summary>
	EKeys_ChannelUp = 296,

	/// <summary>
	/// The DVR key. On some TV remotes, switches to a DVR mode for recorded shows (Android)
	/// </summary>
	EKeys_DVR = 297,

	/// <summary>
	/// The guide key. On TV remotes, shows a programming guide(Android)
	/// </summary>
	EKeys_Guide = 298,

	/// <summary>
	/// The info key. Common on TV remotes to show additional information related to what is currently being viewed(Android)
	/// </summary>
	EKeys_Info = 299,

	/// <summary>
	/// The blue "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
	/// </summary>
	EKeys_ProgBlue = 300,

	/// <summary>
	/// The green "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
	/// </summary>
	EKeys_ProgGreen = 301,

	/// <summary>
	/// The red "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
	/// </summary>
	EKeys_ProgRed = 302,

	/// <summary>
	/// The yellow "programmable" key. On TV remotes, acts as a contextual/programmable key (Android)
	/// </summary>
	EKeys_ProgYellow = 303,

	/// <summary>
	/// The set-top-box input key. On TV remotes, switches the input mode on an external set-top-box (Android)
	/// </summary>
	EKeys_STBInput = 304,

	/// <summary>
	/// The set-top-box power key. On TV remotes, toggles the power on an external Set-top-box (Android)
	/// </summary>
	EKeys_STBPower = 305,

	/// <summary>
	/// The TV key. On TV remotes, switches to viewing live TV (Android)
	/// </summary>
	EKeys_TV = 306,

	/// <summary>
	/// The TV input key. On TV remotes, switches the input on a television screen (Android)
	/// </summary>
	EKeys_TVInput = 307,

	/// <summary>
	/// The TV power key. On TV remotes, toggles the power on a television screen (Android)
	/// </summary>
	EKeys_TVPower = 308,

	/// <summary>
	/// The window key. On TV remotes, toggles picture-in-picture mode or other windowing functions (Android)
	/// </summary>
	EKeys_Window = 309,

	//Android end

	EKeys_Count = 310,
};

EKeys GetEKeyByKeyCode(int keyCode)
{
	switch (keyCode)
	{
	case kVK_Return:
		return EKeys_Return;
	case kVK_Tab:
		return EKeys_Tab;
	case kVK_Space:
		return EKeys_Space;
	case kVK_Delete:
		return EKeys_Back;
	case kVK_Escape:
		return EKeys_Escape;
	case kVK_Command:
		return EKeys_LCommand;
	case 0x36:
		return EKeys_RCommand;
	case kVK_Shift:
		return EKeys_LShift;
	case kVK_CapsLock:
		return EKeys_CapsLock;
	case kVK_Option:
		return EKeys_LAlt;
	case kVK_Control:
		return EKeys_LControl;
	case kVK_RightShift:
		return EKeys_RShift;
	case kVK_RightOption:
		return EKeys_RAlt;
	case kVK_RightControl:
		return EKeys_RControl;
	case kVK_Function:
		return EKeys_Function;
	case kVK_F17:
		return EKeys_F17;
	case kVK_VolumeUp:
		return EKeys_VolumeUp;
	case kVK_VolumeDown:
		return EKeys_VolumeDown;
	case kVK_Mute:
		return EKeys_VolumeMute;
	case kVK_F18:
		return EKeys_F18;
	case kVK_F19:
		return EKeys_F19;
	case kVK_F20:
		return EKeys_F20;
	case kVK_F5:
		return EKeys_F5;
	case kVK_F6:
		return EKeys_F6;
	case kVK_F7:
		return EKeys_F7;
	case kVK_F3:
		return EKeys_F3;
	case kVK_F8:
		return EKeys_F8;
	case kVK_F9:
		return EKeys_F9;
	case kVK_F11:
		return EKeys_F11;
	case kVK_F13:
		return EKeys_F13;
	case kVK_F16:
		return EKeys_F16;
	case kVK_F14:
		return EKeys_F14;
	case kVK_F10:
		return EKeys_F10;
	case kVK_F12:
		return EKeys_F12;
	case kVK_F15:
		return EKeys_F15;
	case kVK_Help:
		return EKeys_Help;
	case kVK_Home:
		return EKeys_Home;
	case kVK_PageUp:
		return EKeys_PageUp;
	case kVK_ForwardDelete:
		return EKeys_Delete;
	case kVK_F4:
		return EKeys_F4;
	case kVK_End:
		return EKeys_End;
	case kVK_F2:
		return EKeys_F2;
	case kVK_PageDown:
		return EKeys_PageDown;
	case kVK_F1:
		return EKeys_F1;
	case kVK_LeftArrow:
		return EKeys_KeyLeft;
	case kVK_RightArrow:
		return EKeys_KeyRight;
	case kVK_DownArrow:
		return EKeys_KeyDown;
	case kVK_UpArrow:
		return EKeys_KeyUp;

	case kVK_ANSI_A:
		return EKeys_A;
	case kVK_ANSI_S:
		return EKeys_S;
	case kVK_ANSI_D:
		return EKeys_D;
	case kVK_ANSI_F:
		return EKeys_F;
	case kVK_ANSI_H:
		return EKeys_H;
	case kVK_ANSI_G:
		return EKeys_G;
	case kVK_ANSI_Z:
		return EKeys_Z;
	case kVK_ANSI_X:
		return EKeys_X;
	case kVK_ANSI_C:
		return EKeys_C;
	case kVK_ANSI_V:
		return EKeys_V;
	case kVK_ANSI_B:
		return EKeys_B;
	case kVK_ANSI_Q:
		return EKeys_Q;
	case kVK_ANSI_W:
		return EKeys_W;
	case kVK_ANSI_E:
		return EKeys_E;
	case kVK_ANSI_R:
		return EKeys_R;
	case kVK_ANSI_Y:
		return EKeys_Y;
	case kVK_ANSI_T:
		return EKeys_T;
	case kVK_ANSI_1:
		return EKeys_D1;
	case kVK_ANSI_2:
		return EKeys_D2;
	case kVK_ANSI_3:
		return EKeys_D3;
	case kVK_ANSI_4:
		return EKeys_D4;
	case kVK_ANSI_6:
		return EKeys_D6;
	case kVK_ANSI_5:
		return EKeys_D5;
	case kVK_ANSI_Equal:
		return EKeys_Oemplus;
	case kVK_ANSI_9:
		return EKeys_D9;
	case kVK_ANSI_7:
		return EKeys_D7;
	case kVK_ANSI_Minus:
		return EKeys_OemMinus;
	case kVK_ANSI_8:
		return EKeys_D8;
	case kVK_ANSI_0:
		return EKeys_D0;
	case kVK_ANSI_RightBracket:
		return EKeys_OemCloseBrackets;
	case kVK_ANSI_O:
		return EKeys_O;
	case kVK_ANSI_U:
		return EKeys_U;
	case kVK_ANSI_LeftBracket:
		return EKeys_OemOpenBrackets;
	case kVK_ANSI_I:
		return EKeys_I;
	case kVK_ANSI_P:
		return EKeys_P;
	case kVK_ANSI_L:
		return EKeys_L;
	case kVK_ANSI_J:
		return EKeys_J;
	case kVK_ANSI_Quote:
		return EKeys_OemQuotes;
	case kVK_ANSI_K:
		return EKeys_K;
	case kVK_ANSI_Semicolon:
		return EKeys_OemSemicolon;
	case kVK_ANSI_Backslash:
		return EKeys_OemBackslash;
	case kVK_ANSI_Comma:
		return EKeys_Oemcomma;
	case kVK_ANSI_Slash:
		return EKeys_Divide;
	case kVK_ANSI_N:
		return EKeys_N;
	case kVK_ANSI_M:
		return EKeys_M;
	case kVK_ANSI_Period:
		return EKeys_OemPeriod;
	case kVK_ANSI_Grave:
		return EKeys_Oemtilde;
	case kVK_ANSI_KeypadDecimal:
		return EKeys_Decimal;
	case kVK_ANSI_KeypadMultiply:
		return EKeys_Multiply;
	case kVK_ANSI_KeypadPlus:
		return EKeys_Oemplus;
	case kVK_ANSI_KeypadClear:
		return EKeys_Clear;
	case kVK_ANSI_KeypadDivide:
		return EKeys_Divide;
	case kVK_ANSI_KeypadEnter:
		return EKeys_Enter;
	case kVK_ANSI_KeypadMinus:
		return EKeys_OemMinus;
	case kVK_ANSI_KeypadEquals:
		return EKeys_Oemplus;
	case kVK_ANSI_Keypad0:
		return EKeys_NumPad0;
	case kVK_ANSI_Keypad1:
		return EKeys_NumPad1;
	case kVK_ANSI_Keypad2:
		return EKeys_NumPad2;
	case kVK_ANSI_Keypad3:
		return EKeys_NumPad3;
	case kVK_ANSI_Keypad4:
		return EKeys_NumPad4;
	case kVK_ANSI_Keypad5:
		return EKeys_NumPad5;
	case kVK_ANSI_Keypad6:
		return EKeys_NumPad6;
	case kVK_ANSI_Keypad7:
		return EKeys_NumPad7;
	case kVK_ANSI_Keypad8:
		return EKeys_NumPad8;
	case kVK_ANSI_Keypad9:
		return EKeys_NumPad9;

	case kVK_ISO_Section:
		return EKeys_Paragraph;
	}

	return (EKeys)0;
}

bool IsSystemKey(int keyCode)
{
	switch (keyCode)
	{
	case kVK_Return:
	case kVK_Tab:
	case kVK_Delete:
	case kVK_Escape:
	case kVK_Command:
	case 0x36://right command
	case kVK_Shift:
	case kVK_CapsLock:
	case kVK_Option:
	case kVK_Control:
	case kVK_RightShift:
	case kVK_RightOption:
	case kVK_RightControl:
	case kVK_Function:
	case kVK_F17:
	case kVK_VolumeUp:
	case kVK_VolumeDown:
	case kVK_Mute:
	case kVK_F18:
	case kVK_F19:
	case kVK_F20:
	case kVK_F5:
	case kVK_F6:
	case kVK_F7:
	case kVK_F3:
	case kVK_F8:
	case kVK_F9:
	case kVK_F11:
	case kVK_F13:
	case kVK_F16:
	case kVK_F14:
	case kVK_F10:
	case kVK_F12:
	case kVK_F15:
	case kVK_Help:
	case kVK_Home:
	case kVK_PageUp:
	case kVK_ForwardDelete:
	case kVK_F4:
	case kVK_End:
	case kVK_F2:
	case kVK_PageDown:
	case kVK_F1:
	case kVK_LeftArrow:
	case kVK_RightArrow:
	case kVK_DownArrow:
	case kVK_UpArrow:
		return true;
	}

	return false;
}
