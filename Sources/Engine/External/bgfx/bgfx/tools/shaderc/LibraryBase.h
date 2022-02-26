// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

//#include "targetver.h"

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX

#if _WIN32
#	include <windows.h>
#endif

///////////////////////////////////////////////////////////////////////////////////////////////////

#include <string>
#include <vector>
#include <map>
#include <set>
#include <algorithm>
#include <stdarg.h>
#include <memory>
#include <iostream>
#include <sstream>

#ifdef _WIN32
#define EXPORT extern "C" __declspec(dllexport)
#else
#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

#ifdef _WIN32
typedef wchar_t wchar16;
#else
typedef unsigned short wchar16;
typedef unsigned char UINT8;
#endif

///////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef _WIN32

#define TO_WCHAR16(_str) _str
#define TO_WCHAR_T(_str) _str

#else

class UTF32ToUTF16Converter
{
	wchar16* buffer;

public:
	UTF32ToUTF16Converter(const wchar_t* str)
	{
		int len = wcslen(str);
		buffer = new wchar16[len + 1];
		for (int n = 0; n < len; n++)
			buffer[n] = (wchar16)str[n];
		buffer[len] = 0;
	}

	~UTF32ToUTF16Converter()
	{
		delete[] buffer;
	}

	operator const wchar16*()
	{
		return buffer;
	}
};

class UTF16ToUTF32Converter
{
	std::wstring buffer;

public:
	UTF16ToUTF32Converter(const wchar16* str)
	{
		int len = 0;
		for (int n = 0; ; n++)
		{
			if (str[n] == 0)
				break;
			len++;
		}
		buffer.resize(len);
		for (int n = 0; n < len; n++)
			buffer[n] = (wchar_t)str[n];
		buffer[len] = 0;
	}

	operator const std::wstring&()
	{
		return buffer;
	}
};

#define TO_WCHAR16(_str) UTF32ToUTF16Converter( (_str) )
#define TO_WCHAR_T(_str) UTF16ToUTF32Converter( (_str) )

#endif

///////////////////////////////////////////////////////////////////////////////////////////////////

wchar16* CreateOutString(const std::wstring& str);
std::string ConvertStringToUTF8(const std::wstring& str);
std::wstring ConvertStringToUTFWide(const std::string& str);

///////////////////////////////////////////////////////////////////////////////////////////////////

void Fatal(const char* text);

///////////////////////////////////////////////////////////////////////////////////////////////////

//!!!!unicode
std::string string_format(const std::string fmt_str, ...);
