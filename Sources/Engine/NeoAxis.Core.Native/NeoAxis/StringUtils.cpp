// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "StringUtils.h"

//extern void Fatal(const char* text);

#ifdef _WIN32
	#define WIN32_LEAN_AND_MEAN
	#define NOMINMAX
	#include <windows.h>
#endif
#ifdef PLATFORM_OSX
	#include <errno.h>
	#include <iconv.h>	
	#import <Carbon/Carbon.h>
#endif

#ifdef PLATFORM_ANDROID
#	include <codecvt>
#endif

#ifdef PLATFORM_IOS
	#include <iconv.h>	
#endif

#if defined(PLATFORM_OSX) || defined(PLATFORM_IOS)

template <class In, class Out>
void ConvertString(iconv_t cd, const In& in, Out* out, const typename Out::value_type errorSign)
{
	typedef typename In::value_type InType;
	typedef typename Out::value_type OutType;

	char* inPointer = (char*)in.data();
	size_t inLength = in.length() * sizeof(InType);

	const size_t bufferSize = 4096;
	OutType buffer[bufferSize];

	out->clear();

	while(inLength != 0)
	{
		char* tempPointer = (char*)buffer;
		size_t tempLength = bufferSize * sizeof(OutType);

		size_t result = iconv(cd, &inPointer, &inLength, &tempPointer, &tempLength);
		size_t n = (OutType*)(tempPointer) - buffer;

		out->append(buffer, n);

		if(result == (size_t)-1)
		{
			if(errno == EINVAL || errno == EILSEQ)
			{
				out->append(1, errorSign);
				inPointer += sizeof(InType);
				inLength -= sizeof(InType);
			}
			else if(errno == E2BIG && n == 0)
			{
				//Fatal("iconv: The buffer is too small.");
			}
		}
	}
}

#endif

std::string ConvertStringToUTF8(const std::wstring& str)
{
	std::string result;
	if(!str.empty())
	{

#ifdef _WIN32
		int size = WideCharToMultiByte(CP_UTF8, 0, str.c_str(), -1, NULL, 0, NULL, NULL);
		if(size != 0)
		{
			char* aString = (char*)_alloca(size * sizeof(char));
			//char* aString = (char*)malloc(size * sizeof(char));
			if(WideCharToMultiByte(CP_UTF8, 0, str.c_str(), -1, aString, size, NULL, NULL) != 0)
				result = aString;
		}
#elif defined(PLATFORM_ANDROID)
		// can't use ConvertString/iconv because __LP64__ is defined somewhere..
		// https://stackoverflow.com/questions/4804298/how-to-convert-wstring-into-string
		using convert_type = std::codecvt_utf8<wchar_t>;
		std::wstring_convert<convert_type, wchar_t> converter;
		//use converter (.to_bytes: wstr->str, .from_bytes: str->wstr)
		result = converter.to_bytes(str);
#else
		static iconv_t cd = (iconv_t)-1;
		if (cd == (iconv_t)-1)
		{
			cd = iconv_open("UTF-8", "UTF-32LE");
			if (cd == (iconv_t)-1)
				Fatal("iconv_open failed.");
		}
		ConvertString(cd, str, &result, '?');
#endif

	}
	return result;
}


std::wstring ConvertStringToUTFWide(const std::string& str)
{
	std::wstring result;
	if(!str.empty())
	{

#ifdef _WIN32
		int size = MultiByteToWideChar(CP_UTF8, 0, str.c_str(), -1, NULL, 0);
		if(size)
		{
			wchar_t* wString = (wchar_t*)_alloca(size * sizeof(wchar_t));
			if(MultiByteToWideChar(CP_UTF8, 0, str.c_str(), -1, wString, size) != 0)
				result = wString;
		}
#elif defined(PLATFORM_ANDROID)
		using convert_type = std::codecvt_utf8<wchar_t>;
		std::wstring_convert<convert_type, wchar_t> converter;
		//use converter (.to_bytes: wstr->str, .from_bytes: str->wstr)
		result = converter.from_bytes(str);
#else
		static iconv_t cd = (iconv_t)-1;
		if(cd == (iconv_t)-1)
		{
			cd = iconv_open("UTF-32LE", "UTF-8");
			if (cd == (iconv_t)(-1))
				Fatal("iconv_open failed.");
		}
		ConvertString(cd, str, &result, '?');
#endif

	}
	return result;
}
