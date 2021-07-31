/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2009 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#include "OgreStableHeaders.h"
#include "OgreString.h"
#include "OgreStringVector.h"

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
	#define WIN32_LEAN_AND_MEAN
	#define NOMINMAX
	#include <windows.h>
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE
	#include <iconv.h>	
#endif

#ifdef ANDROID
#include <codecvt>
#include <string>
//#include "ConvertUTF.h"
#endif


namespace Ogre {

	//-----------------------------------------------------------------------
	const String StringUtil::BLANK;
	const WString StringUtil::WBLANK;
	//-----------------------------------------------------------------------
    void StringUtil::trim(String& str, bool left, bool right)
    {
        /*
        size_t lspaces, rspaces, len = length(), i;

        lspaces = rspaces = 0;

        if( left )
        {
            // Find spaces / tabs on the left
            for( i = 0;
                i < len && ( at(i) == ' ' || at(i) == '\t' || at(i) == '\r');
                ++lspaces, ++i );
        }
        
        if( right && lspaces < len )
        {
            // Find spaces / tabs on the right
            for( i = len - 1;
                i >= 0 && ( at(i) == ' ' || at(i) == '\t' || at(i) == '\r');
                rspaces++, i-- );
        }

        *this = substr(lspaces, len-lspaces-rspaces);
        */
        static const String delims = " \t\r";
        if(right)
            str.erase(str.find_last_not_of(delims)+1); // trim right
        if(left)
            str.erase(0, str.find_first_not_of(delims)); // trim left
    }
	//-----------------------------------------------------------------------
	//!!!vladimir
    void StringUtil::trim(WString& str, bool left, bool right)
    {
        static const WString delims = L" \t\r";
        if(right)
            str.erase(str.find_last_not_of(delims)+1); // trim right
        if(left)
            str.erase(0, str.find_first_not_of(delims)); // trim left
    }

    //-----------------------------------------------------------------------
    StringVector StringUtil::split( const String& str, const String& delims, unsigned int maxSplits)
    {
        StringVector ret;
        // Pre-allocate some space for performance
        ret.reserve(maxSplits ? maxSplits+1 : 10);    // 10 is guessed capacity for most case

        unsigned int numSplits = 0;

        // Use STL methods 
        size_t start, pos;
        start = 0;
        do 
        {
            pos = str.find_first_of(delims, start);
            if (pos == start)
            {
                // Do nothing
                start = pos + 1;
            }
            else if (pos == String::npos || (maxSplits && numSplits == maxSplits))
            {
                // Copy the rest of the string
                ret.push_back( str.substr(start) );
                break;
            }
            else
            {
                // Copy up to delimiter
                ret.push_back( str.substr(start, pos - start) );
                start = pos + 1;
            }
            // parse up to next real data
            start = str.find_first_not_of(delims, start);
            ++numSplits;

        } while (pos != String::npos);



        return ret;
    }

    WStringVector StringUtil::split( const WString& str, const WString& delims, unsigned int maxSplits)
    {
        WStringVector ret;
        // Pre-allocate some space for performance
        ret.reserve(maxSplits ? maxSplits+1 : 10);    // 10 is guessed capacity for most case

        unsigned int numSplits = 0;

        // Use STL methods 
        size_t start, pos;
        start = 0;
        do 
        {
            pos = str.find_first_of(delims, start);
            if (pos == start)
            {
                // Do nothing
                start = pos + 1;
            }
            else if (pos == WString::npos || (maxSplits && numSplits == maxSplits))
            {
                // Copy the rest of the string
                ret.push_back( str.substr(start) );
                break;
            }
            else
            {
                // Copy up to delimiter
                ret.push_back( str.substr(start, pos - start) );
                start = pos + 1;
            }
            // parse up to next real data
            start = str.find_first_not_of(delims, start);
            ++numSplits;

        } while (pos != WString::npos);



        return ret;
    }

	//-----------------------------------------------------------------------
	StringVector StringUtil::tokenise( const String& str, const String& singleDelims, const String& doubleDelims, unsigned int maxSplits)
	{
        StringVector ret;
        // Pre-allocate some space for performance
        ret.reserve(maxSplits ? maxSplits+1 : 10);    // 10 is guessed capacity for most case

        unsigned int numSplits = 0;
		String delims = singleDelims + doubleDelims;

		// Use STL methods 
        size_t start, pos;
		char curDoubleDelim = 0;
        start = 0;
        do 
        {
			if (curDoubleDelim != 0)
			{
				pos = str.find(curDoubleDelim, start);
			}
			else
			{
				pos = str.find_first_of(delims, start);
			}

            if (pos == start)
            {
				char curDelim = str.at(pos);
				if (doubleDelims.find_first_of(curDelim) != String::npos)
				{
					curDoubleDelim = curDelim;
				}
                // Do nothing
                start = pos + 1;
            }
            else if (pos == String::npos || (maxSplits && numSplits == maxSplits))
            {
				if (curDoubleDelim != 0)
				{
					//Missing closer. Warn or throw exception?
				}
                // Copy the rest of the string
                ret.push_back( str.substr(start) );
                break;
            }
            else
            {
				if (curDoubleDelim != 0)
				{
					curDoubleDelim = 0;
				}

				// Copy up to delimiter
				ret.push_back( str.substr(start, pos - start) );
				start = pos + 1;
            }
			if (curDoubleDelim == 0)
			{
				// parse up to next real data
				start = str.find_first_not_of(singleDelims, start);
			}
            
            ++numSplits;

        } while (pos != String::npos);

        return ret;
    }
    //-----------------------------------------------------------------------
    void StringUtil::toLowerCase(String& str)
    {
		for(int n = 0; n < str.size(); n++)
			str[n] = tolower(str[n]);
   //     std::transform(
   //         str.begin(),
   //         str.end(),
   //         str.begin(),
			//tolower);
    }

    void StringUtil::toLowerCase(WString& str)
    {
		for(int n = 0; n < str.size(); n++)
			str[n] = tolower(str[n]);
   //     std::transform(
   //         str.begin(),
   //         str.end(),
   //         str.begin(),
			//tolower);
    }

    //-----------------------------------------------------------------------
    void StringUtil::toUpperCase(String& str) 
    {
		for(int n = 0; n < str.size(); n++)
			str[n] = toupper(str[n]);
   //     std::transform(
   //         str.begin(),
   //         str.end(),
   //         str.begin(),
			//toupper);
    }

    void StringUtil::toUpperCase(WString& str) 
    {
		for(int n = 0; n < str.size(); n++)
			str[n] = toupper(str[n]);
   //     std::transform(
   //         str.begin(),
   //         str.end(),
   //         str.begin(),
			//toupper);
    }

    //-----------------------------------------------------------------------
    bool StringUtil::startsWith(const String& str, const String& pattern, bool lowerCase)
    {
        size_t thisLen = str.length();
        size_t patternLen = pattern.length();
        if (thisLen < patternLen || patternLen == 0)
            return false;

        String startOfThis = str.substr(0, patternLen);
        if (lowerCase)
            StringUtil::toLowerCase(startOfThis);

        return (startOfThis == pattern);
    }

    bool StringUtil::startsWith(const WString& str, const WString& pattern, bool lowerCase)
    {
        size_t thisLen = str.length();
        size_t patternLen = pattern.length();
        if (thisLen < patternLen || patternLen == 0)
            return false;

        WString startOfThis = str.substr(0, patternLen);
        if (lowerCase)
            StringUtil::toLowerCase(startOfThis);

        return (startOfThis == pattern);
    }

    //-----------------------------------------------------------------------
    bool StringUtil::endsWith(const String& str, const String& pattern, bool lowerCase)
    {
        size_t thisLen = str.length();
        size_t patternLen = pattern.length();
        if (thisLen < patternLen || patternLen == 0)
            return false;

        String endOfThis = str.substr(thisLen - patternLen, patternLen);
        if (lowerCase)
            StringUtil::toLowerCase(endOfThis);

        return (endOfThis == pattern);
    }
    //-----------------------------------------------------------------------
    String StringUtil::standardisePath(const String& init)
    {
        String path = init;

        std::replace( path.begin(), path.end(), '\\', '/' );
        if( path[path.length() - 1] != '/' )
            path += '/';

        return path;
    }
    //-----------------------------------------------------------------------
    void StringUtil::splitFilename(const String& qualifiedName, 
        String& outBasename, String& outPath)
    {
        String path = qualifiedName;
        // Replace \ with / first
        std::replace( path.begin(), path.end(), '\\', '/' );
        // split based on final /
        size_t i = path.find_last_of('/');

        if (i == String::npos)
        {
            outPath.clear();
			outBasename = qualifiedName;
        }
        else
        {
            outBasename = path.substr(i+1, path.size() - i - 1);
            outPath = path.substr(0, i+1);
        }

    }
	//-----------------------------------------------------------------------
	void StringUtil::splitBaseFilename(const Ogre::String& fullName, 
		Ogre::String& outBasename, Ogre::String& outExtention)
	{
		size_t i = fullName.find_last_of(".");
		if (i == Ogre::String::npos)
		{
			outExtention.clear();
			outBasename = fullName;
		}
		else
		{
			outExtention = fullName.substr(i+1);
			outBasename = fullName.substr(0, i);
		}
	}
	// ----------------------------------------------------------------------------------------------------------------------------------------------
	void StringUtil::splitFullFilename(	const Ogre::String& qualifiedName, 
		Ogre::String& outBasename, Ogre::String& outExtention, Ogre::String& outPath )
	{
		Ogre::String fullName;
		splitFilename( qualifiedName, fullName, outPath );
		splitBaseFilename( fullName, outBasename, outExtention );
	}
    //-----------------------------------------------------------------------
    bool StringUtil::match(const String& str, const String& pattern, bool caseSensitive)
    {
        String tmpStr = str;
		String tmpPattern = pattern;
        if (!caseSensitive)
        {
            StringUtil::toLowerCase(tmpStr);
            StringUtil::toLowerCase(tmpPattern);
        }

        String::const_iterator strIt = tmpStr.begin();
        String::const_iterator patIt = tmpPattern.begin();
		String::const_iterator lastWildCardIt = tmpPattern.end();
        while (strIt != tmpStr.end() && patIt != tmpPattern.end())
        {
            if (*patIt == '*')
            {
				lastWildCardIt = patIt;
                // Skip over looking for next character
                ++patIt;
                if (patIt == tmpPattern.end())
				{
					// Skip right to the end since * matches the entire rest of the string
					strIt = tmpStr.end();
				}
				else
                {
					// scan until we find next pattern character
                    while(strIt != tmpStr.end() && *strIt != *patIt)
                        ++strIt;
                }
            }
            else
            {
                if (*patIt != *strIt)
                {
					if (lastWildCardIt != tmpPattern.end())
					{
						// The last wildcard can match this incorrect sequence
						// rewind pattern to wildcard and keep searching
						patIt = lastWildCardIt;
						lastWildCardIt = tmpPattern.end();
					}
					else
					{
						// no wildwards left
						return false;
					}
                }
                else
                {
                    ++patIt;
                    ++strIt;
                }
            }

        }
		// If we reached the end of both the pattern and the string, we succeeded
		if (patIt == tmpPattern.end() && strIt == tmpStr.end())
		{
        	return true;
		}
		else
		{
			return false;
		}

    }
	//-----------------------------------------------------------------------
	const String StringUtil::replaceAll(const String& source, const String& replaceWhat, const String& replaceWithWhat)
	{
		String result = source;
        String::size_type pos = 0;
		while(1)
		{
			pos = result.find(replaceWhat,pos);
			if (pos == String::npos) break;
			result.replace(pos,replaceWhat.size(),replaceWithWhat);
            pos += replaceWithWhat.size();
		}
		return result;
	}


#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE

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
					Fatal("iconv: The buffer is too small.");
				}
			}
		}
	}

#endif

	String StringUtil::toUTF8(const WString& str)
	{
		String result;
		if(!str.empty())
		{

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
			int size = WideCharToMultiByte(CP_UTF8, 0, str.c_str(), -1, NULL, 0, NULL, NULL);
			if(size != 0)
			{
				char* aString = (char*)_alloca(size * sizeof(char));
				//char* aString = new char[size];
				if(WideCharToMultiByte(CP_UTF8, 0, str.c_str(), -1, aString, size, NULL, NULL) != 0)
					result = aString;
				//delete[] aString;
			}
#elif defined ANDROID

			std::wstring_convert<std::codecvt_utf8<wchar_t>> myconv;
			result = myconv.to_bytes(str);

			////vladimir
			//UTF8 *resultUTF8, *utf8Start;
			//const UTF32 *utf32String = (const UTF32*)str.c_str();

			//size_t stringLength = str.length();

			//utf8Start = resultUTF8 = (UTF8*) malloc(stringLength * sizeof(UTF8) + 1);

			//ConversionResult conversionResult = ConvertUTF32toUTF8(&utf32String, &utf32String[stringLength], 
			//	&utf8Start, &utf8Start[stringLength], strictConversion);

			//*utf8Start = 0;

			//if(conversionResult == conversionOK)
			//	result = String((char*)resultUTF8);

			//free(resultUTF8);

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

	WString StringUtil::toUTFWide(const String& str)
	{
		WString result;
		if(!str.empty())
		{

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
			int size = MultiByteToWideChar(CP_UTF8, 0, str.c_str(), -1, NULL, 0);
			if(size)
			{
				wchar_t* wString = (wchar_t*)_alloca(size * sizeof(wchar_t));
				//wchar_t* wString = new wchar_t[size];
				if(MultiByteToWideChar(CP_UTF8, 0, str.c_str(), -1, wString, size) != 0)
					result = wString;
				//delete[] wString;
			}

#elif defined ANDROID

			std::wstring_convert<std::codecvt_utf8<wchar_t>> myconv;
			result = myconv.from_bytes(str);

			////!!!!is not UTF8 conversion

			//int length = str.length();
			//const char* str2 = str.c_str();

			//WString s;
			//for (int n = 0; n < length; n++)
			//	s.push_back(str2[n]);

			//result = s;


			////vladimir
			//UTF32 *resultUTF32, *utf32Start;
			//const UTF8 *utf8String = (const UTF8*)str.c_str();

			//size_t stringLength = str.length();

			//utf32Start = resultUTF32 = (UTF32*) malloc(stringLength * sizeof(UTF32) + 1);

			//ConversionResult conversionResult = ConvertUTF8toUTF32(&utf8String, &utf8String[stringLength], 
			//	&utf32Start, &utf32Start[stringLength], strictConversion);

			//*utf32Start = 0;

			//if(conversionResult == conversionOK)
			//	result = WString((wchar_t*)resultUTF32);

			//free(resultUTF32);

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

}
