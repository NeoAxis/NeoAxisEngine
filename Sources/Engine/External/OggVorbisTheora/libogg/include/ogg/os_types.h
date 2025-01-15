/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2002             *
 * by the Xiph.Org Foundation http://www.xiph.org/                  *
 *                                                                  *
 ********************************************************************

 function: #ifdef jail to whip a few platforms into the UNIX ideal.
 last mod: $Id: os_types.h 14997 2008-06-04 03:27:18Z ivo $

 ********************************************************************/
#ifndef _OS_TYPES_H
#define _OS_TYPES_H

/* make it easy on the folks that want to compile the libs with a
   different malloc than stdlib */

//!!!!betauser
#define USE_NATIVE_MEMORY_MANAGER

#ifndef USE_NATIVE_MEMORY_MANAGER

#define _ogg_malloc  malloc
#define _ogg_calloc  calloc
#define _ogg_realloc realloc
#define _ogg_free    free

#else //USE_NATIVE_MEMORY_MANAGER

#ifdef _WIN32
	#include <malloc.h>
#endif
#if (defined(IOS) || defined(__EMSCRIPTEN__))
    #include <string.h>
#endif
#include <stdio.h>
#include <stdlib.h>
#include "MemoryManager.h"

#if (defined(_WIN32) || defined(__WIN32__))
	#define OGG_FORCEINLINE __forceinline
#elif defined(__APPLE_CC__)
	#define OGG_FORCEINLINE __private_extern__ inline __attribute__((always_inline))
#elif defined(ANDROID)
	#define OGG_FORCEINLINE extern inline __attribute__((__gnu_inline__))
#else
	#define OGG_FORCEINLINE inline __attribute__((always_inline))
#endif

#if defined(ANDROID)
extern void* _ogg_calloc2(size_t num, size_t size, char* fileName, int line);
#else
OGG_FORCEINLINE void* _ogg_calloc2(size_t num, size_t size, char* fileName, int line)
{
	int total = (int)(num * size);
	void* pointer = Memory_Alloc(MemoryAllocationType_SoundAndVideo, total, fileName, line);
	if( pointer )
		memset( pointer, 0, total );
	return pointer;
}
#endif

#define _ogg_malloc(__size) (Memory_Alloc(MemoryAllocationType_SoundAndVideo, (int)(__size),__FILE__,__LINE__))
#define _ogg_calloc(__num, __size) (_ogg_calloc2(__num, __size, __FILE__, __LINE__))
#define _ogg_realloc(__memblock, __size) (Memory_Realloc(MemoryAllocationType_SoundAndVideo, (__memblock), (int)(__size), __FILE__, __LINE__))
#define _ogg_free(__memblock) (Memory_Free(__memblock))

#endif //USE_NATIVE_MEMORY_MANAGER



#if defined(_WIN32) 

#  if defined(__CYGWIN__)
#    include <stdint.h>
     typedef int16_t ogg_int16_t;
     typedef uint16_t ogg_uint16_t;
     typedef int32_t ogg_int32_t;
     typedef uint32_t ogg_uint32_t;
     typedef int64_t ogg_int64_t;
     typedef uint64_t ogg_uint64_t;
#  elif defined(__MINGW32__)
#    include <sys/types.h>
     typedef short ogg_int16_t;                                                                             
     typedef unsigned short ogg_uint16_t;                                                                   
     typedef int ogg_int32_t;                                                                               
     typedef unsigned int ogg_uint32_t;                                                                     
     typedef long long ogg_int64_t;                                                                         
     typedef unsigned long long ogg_uint64_t;  
#  elif defined(__MWERKS__)
     typedef long long ogg_int64_t;
     typedef int ogg_int32_t;
     typedef unsigned int ogg_uint32_t;
     typedef short ogg_int16_t;
     typedef unsigned short ogg_uint16_t;
#  else
     /* MSVC/Borland */
     typedef __int64 ogg_int64_t;
     typedef __int32 ogg_int32_t;
     typedef unsigned __int32 ogg_uint32_t;
     typedef __int16 ogg_int16_t;
     typedef unsigned __int16 ogg_uint16_t;
#  endif

#elif defined(__MACOS__)

#  include <sys/types.h>
   typedef SInt16 ogg_int16_t;
   typedef UInt16 ogg_uint16_t;
   typedef SInt32 ogg_int32_t;
   typedef UInt32 ogg_uint32_t;
   typedef SInt64 ogg_int64_t;

#elif (defined(__APPLE__) && defined(__MACH__)) /* MacOS X Framework build */

#  include <sys/types.h>
   typedef int16_t ogg_int16_t;
   typedef u_int16_t ogg_uint16_t;
   typedef int32_t ogg_int32_t;
   typedef u_int32_t ogg_uint32_t;
   typedef int64_t ogg_int64_t;

#elif defined(__HAIKU__)

  /* Haiku */
#  include <sys/types.h>
   typedef short ogg_int16_t;
   typedef unsigned short ogg_uint16_t;
   typedef int ogg_int32_t;
   typedef unsigned int ogg_uint32_t;
   typedef long long ogg_int64_t;

#elif defined(__BEOS__)

   /* Be */
#  include <inttypes.h>
   typedef int16_t ogg_int16_t;
   typedef u_int16_t ogg_uint16_t;
   typedef int32_t ogg_int32_t;
   typedef u_int32_t ogg_uint32_t;
   typedef int64_t ogg_int64_t;

#elif defined (__EMX__)

   /* OS/2 GCC */
   typedef short ogg_int16_t;
   typedef unsigned short ogg_uint16_t;
   typedef int ogg_int32_t;
   typedef unsigned int ogg_uint32_t;
   typedef long long ogg_int64_t;

#elif defined (DJGPP)

   /* DJGPP */
   typedef short ogg_int16_t;
   typedef int ogg_int32_t;
   typedef unsigned int ogg_uint32_t;
   typedef long long ogg_int64_t;

#elif defined(R5900)

   /* PS2 EE */
   typedef long ogg_int64_t;
   typedef int ogg_int32_t;
   typedef unsigned ogg_uint32_t;
   typedef short ogg_int16_t;

#elif defined(__SYMBIAN32__)

   /* Symbian GCC */
   typedef signed short ogg_int16_t;
   typedef unsigned short ogg_uint16_t;
   typedef signed int ogg_int32_t;
   typedef unsigned int ogg_uint32_t;
   typedef long long int ogg_int64_t;

//!!!!dr
#elif defined(ANDROID) || defined(LINUX) /* MacOS X Framework build */

#  include <sys/types.h>
   typedef int16_t ogg_int16_t;
   typedef u_int16_t ogg_uint16_t;
   typedef int32_t ogg_int32_t;
   typedef u_int32_t ogg_uint32_t;
   typedef int64_t ogg_int64_t;

#elif defined(__EMSCRIPTEN__)

#  include <sys/types.h>
typedef int16_t ogg_int16_t;
typedef u_int16_t ogg_uint16_t;
typedef int32_t ogg_int32_t;
typedef u_int32_t ogg_uint32_t;
typedef int64_t ogg_int64_t;

#else

#  include <sys/types.h>
#  include <ogg/config_types.h>

#endif

#endif  /* _OS_TYPES_H */
