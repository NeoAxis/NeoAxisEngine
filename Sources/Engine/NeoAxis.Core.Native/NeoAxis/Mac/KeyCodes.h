// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
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


	EKeys_Count,
};

EKeys GetEKeyByKeyCode(int keyCode);
bool IsSystemKey(int keyCode);
