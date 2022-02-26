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
#ifndef _String_H__
#define _String_H__

#include "OgrePrerequisites.h"

// If we're using the GCC 3.1 C++ Std lib
#if OGRE_COMPILER == OGRE_COMPILER_GNUC && OGRE_COMP_VER >= 310 && !defined(STLPORT)

// For gcc 4.3 see http://gcc.gnu.org/gcc-4.3/changes.html
#   if OGRE_COMP_VER >= 430
#       include <tr1/unordered_map> 
#   else
#include <ext/hash_map>
namespace __gnu_cxx
{
    template <> struct hash< Ogre::_StringBase >
    {
        size_t operator()( const Ogre::_StringBase _stringBase ) const
        {
            /* This is the PRO-STL way, but it seems to cause problems with VC7.1
               and in some other cases (although I can't recreate it)
            hash<const char*> H;
            return H(_stringBase.c_str());
            */
            /** This is our custom way */
            register size_t ret = 0;
            for( Ogre::_StringBase::const_iterator it = _stringBase.begin(); it != _stringBase.end(); ++it )
                ret = 5 * ret + *it;

            return ret;
        }
    };

    template <> struct hash< Ogre::_WStringBase >
    {
        size_t operator()( const Ogre::_WStringBase _stringBase ) const
        {
            /* This is the PRO-STL way, but it seems to cause problems with VC7.1
               and in some other cases (although I can't recreate it)
            hash<wchar_t*> H;
            return H(_stringBase.c_str());
            */
            /** This is our custom way */
            register size_t ret = 0;
            for( Ogre::_WStringBase::const_iterator it = _stringBase.begin(); it != _stringBase.end(); ++it )
                ret = 5 * ret + *it;

            return ret;
        }
    };

}
#   endif

#endif

// A quick define to overcome different names for the same function
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
#   define locale_t _locale_t
#   define strtod_l _strtod_l
#   define strtoul_l _strtoul_l
#   define strtol_l _strtol_l
#else
#   define stricmp strcasecmp
#   define strnicmp strncasecmp
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_ANDROID || OGRE_PLATFORM == OGRE_PLATFORM_EMSCRIPTEN
#   define locale_t int
#   define strtod_l(ptr, end, l) strtod(ptr, end)
#   define strtoul_l(ptr, end, base, l) strtoul(ptr, end, base)
#   define strtol_l(ptr, end, base, l) strtol(ptr, end, base)
#endif

namespace Ogre {
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup General
	*  @{
	*/

    /** Utility class for manipulating Strings.  */
    class _OgreExport StringUtil
    {
	public:
        //typedef ostringstream StrStreamType;
        //typedef wostringstream WStrStreamType;

        /** Removes any whitespace characters, be it standard space or
            TABs and so on.
            @remarks
                The user may specify wether they want to trim only the
                beginning or the end of the String ( the default action is
                to trim both).
        */
        static void trim( String& str, bool left = true, bool right = true );


        /** Removes any whitespace characters, be it standard space or
            TABs and so on.
            @remarks
                The user may specify wether they want to trim only the
                beginning or the end of the String ( the default action is
                to trim both).
        */
        static void trim( WString& str, bool left = true, bool right = true );

        /** Returns a StringVector that contains all the substrings delimited
            by the characters in the passed <code>delims</code> argument.
            @param
                delims A list of delimiter characters to split by
            @param
                maxSplits The maximum number of splits to perform (0 for unlimited splits). If this
                parameters is > 0, the splitting process will stop after this many splits, left to right.
        */
		static std::nvector< String > split( const String& str, const String& delims = "\t\n ", unsigned int maxSplits = 0);
		static std::nvector< WString > split( const WString& str, const WString& delims = L"\t\n ", unsigned int maxSplits = 0);

		/** Returns a StringVector that contains all the substrings delimited
            by the characters in the passed <code>delims</code> argument, 
			or in the <code>doubleDelims</code> argument, which is used to include (normal) 
			delimeters in the tokenised string. For example, "strings like this".
            @param
                delims A list of delimiter characters to split by
			@param
                delims A list of double delimeters characters to tokenise by
            @param
                maxSplits The maximum number of splits to perform (0 for unlimited splits). If this
                parameters is > 0, the splitting process will stop after this many splits, left to right.
        */
		static std::vector<String> tokenise( const String& str, const String& delims = "\t\n ", const String& doubleDelims = "\"", unsigned int maxSplits = 0);

		/** Lower-cases all the characters in the string.
        */
        static void toLowerCase( String& str );
        static void toLowerCase( WString& str );

        /** Upper-cases all the characters in the string.
        */
        static void toUpperCase( String& str );
        static void toUpperCase( WString& str );


        /** Returns whether the string begins with the pattern passed in.
        @param pattern The pattern to compare with.
        @param lowerCase If true, the start of the string will be lower cased before
            comparison, pattern should also be in lower case.
        */
        static bool startsWith(const String& str, const String& pattern, bool lowerCase = true);
        static bool startsWith(const WString& str, const WString& pattern, bool lowerCase = true);

        /** Returns whether the string ends with the pattern passed in.
        @param pattern The pattern to compare with.
        @param lowerCase If true, the end of the string will be lower cased before
            comparison, pattern should also be in lower case.
        */
        static bool endsWith(const String& str, const String& pattern, bool lowerCase = true);

        /** Method for standardising paths - use forward slashes only, end with slash.
        */
        static String standardisePath( const String &init);

        /** Method for splitting a fully qualified filename into the base name
            and path.
        @remarks
            Path is standardised as in standardisePath
        */
        static void splitFilename(const String& qualifiedName,
            String& outBasename, String& outPath);

		/** Method for splitting a fully qualified filename into the base name,
		extension and path.
		@remarks
		Path is standardised as in standardisePath
		*/
		static void splitFullFilename(const Ogre::String& qualifiedName, 
			Ogre::String& outBasename, Ogre::String& outExtention, 
			Ogre::String& outPath);

		/** Method for splitting a filename into the base name
		and extension.
		*/
		static void splitBaseFilename(const Ogre::String& fullName, 
			Ogre::String& outBasename, Ogre::String& outExtention);


        /** Simple pattern-matching routine allowing a wildcard pattern.
        @param str String to test
        @param pattern Pattern to match against; can include simple '*' wildcards
        @param caseSensitive Whether the match is case sensitive or not
        */
        static bool match(const String& str, const String& pattern, bool caseSensitive = true);


		/** replace all instances of a sub-string with a another sub-string.
		@param source Source string
		@param replaceWhat Sub-string to find and replace
		@param replaceWithWhat Sub-string to replace with (the new sub-string)
		@returns An updated string with the sub-string replaced
		*/
		static const String replaceAll(const String& source, const String& replaceWhat, const String& replaceWithWhat);

		static String toUTF8(const WString& str);
		static WString toUTFWide(const String& str);

        /// Constant blank string, useful for returning by ref where local does not exist
        static const String BLANK;
        static const WString WBLANK;
    };


#if OGRE_COMPILER == OGRE_COMPILER_GNUC && OGRE_COMP_VER >= 310 && !defined(STLPORT)
#   if OGRE_COMP_VER < 430
    typedef ::__gnu_cxx::hash< _StringBase > _StringHash;
    typedef ::__gnu_cxx::hash< _WStringBase > _WStringHash;
#   else
	typedef ::std::tr1::hash< _StringBase > _StringHash;
	typedef ::std::tr1::hash< _WStringBase > _WStringHash;
#   endif
#elif OGRE_COMPILER == OGRE_COMPILER_MSVC && OGRE_COMP_VER >= 1600 && !defined(STLPORT) // VC++ 10.0
	typedef ::std::tr1::hash< _StringBase > _StringHash;
	typedef ::std::tr1::hash< _WStringBase > _WStringHash;
//!!!!for iOS
//#elif !defined( _STLP_HASH_FUN_H )
//	typedef stdext::hash_compare< _StringBase, std::less< _StringBase > > _StringHash;
//	typedef stdext::hash_compare< _WStringBase, std::less< _WStringBase > > _WStringHash;
#else
    typedef std::hash< _StringBase > _StringHash;
	typedef std::hash< _WStringBase > _WStringHash;
#endif
	/** @} */
	/** @} */

} // namespace Ogre

#endif // _String_H__
