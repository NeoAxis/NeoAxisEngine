#ifndef _TIF_CONFIG_H_
#define _TIF_CONFIG_H_

/* Define to 1 if you have the <assert.h> header file. */
#define HAVE_ASSERT_H 1

/* Define to 1 if you have the <fcntl.h> header file. */
#define HAVE_FCNTL_H 1

/* Define as 0 or 1 according to the floating point format suported by the
   machine */
#define HAVE_IEEEFP 1

/* Define to 1 if you have the <sys/types.h> header file. */
#define HAVE_SYS_TYPES_H 1

/* Signed 64-bit type formatter */
#define TIFF_INT64_FORMAT "%I64d"

/* Signed 64-bit type */
#define TIFF_INT64_T int64_t

/* Unsigned 64-bit type formatter */
#define TIFF_UINT64_FORMAT "%I64u"

/* Unsigned 64-bit type */
#define TIFF_UINT64_T uint64_t

/* Unsigned size type formatter */
#define TIFF_SIZE_FORMAT "%u"

/* Signed size type formatter */
#define TIFF_SSIZE_FORMAT "%d"

/* check for 32-bit or 64-bit CPU */
#include <stdint.h>
#if UINTPTR_MAX == 0xffffffff
/* 32-bit */
/* The size of `size_t', as computed by sizeof. */
#define SIZEOF_SIZE_T 4
#elif UINTPTR_MAX == 0xffffffffffffffff
/* 64-bit */
/* The size of `size_t', as computed by sizeof. */
#define SIZEOF_SIZE_T 8
#else
/* undefined */
#define SIZEOF_SIZE_T 0
#endif


/*
Define WORDS_BIGENDIAN to 1 if your processor stores words with the most
significant byte first (like Motorola and SPARC, unlike Intel).
Some versions of gcc may have BYTE_ORDER or __BYTE_ORDER defined
If your big endian system isn't being detected, add an OS specific check
*/
#if (defined(BYTE_ORDER) && BYTE_ORDER==BIG_ENDIAN) || (defined(__BYTE_ORDER) && __BYTE_ORDER==__BIG_ENDIAN) || defined(__BIG_ENDIAN__)
/* Set the native cpu bit order (FILLORDER_LSB2MSB or FILLORDER_MSB2LSB) */
#define HOST_FILLORDER FILLORDER_MSB2LSB
/* Native cpu byte order: 1 if big-endian (Motorola) or 0 if little-endian (Intel) */
#define WORDS_BIGENDIAN 1
/* Native cpu byte order: 1 if big-endian (Motorola) or 0 if little-endian (Intel) */
#define HOST_BIGENDIAN 1
#else
/* Set the native cpu bit order (FILLORDER_LSB2MSB or FILLORDER_MSB2LSB) */
#define HOST_FILLORDER FILLORDER_LSB2MSB
/* Native cpu byte order: 1 if big-endian (Motorola) or 0 if little-endian (Intel) */
#undef WORDS_BIGENDIAN
/* Native cpu byte order: 1 if big-endian (Motorola) or 0 if little-endian (Intel) */
#undef HOST_BIGENDIAN
#endif // BYTE_ORDER

#ifdef _WIN32
/* Visual Studio 2015 / VC 14 / MSVC 19.00 finally has snprintf() */
#if defined(_MSC_VER) && (_MSC_VER < 1900)
#define snprintf _snprintf
#endif // _MSC_VER
#define lfind _lfind
#endif // _WIN32

/* Define to `__inline__' or `__inline' if that's what the C compiler
   calls it, or to nothing if 'inline' is not supported under any name.  */
#ifndef __cplusplus
#ifndef inline
#define inline __inline
#endif
#endif


#endif // _TIF_CONFIG_H_
