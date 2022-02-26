/*
 * Copyright 2010-2020 Branimir Karadzic. All rights reserved.
 * License: https://github.com/bkaradzic/bx#license-bsd-2-clause
 */

#include "bx_p.h"
#include <bx/allocator.h>

#ifndef IOS
#include <malloc.h>
#endif

//!!!!betauser
#if BX_PLATFORM_WINDOWS //#ifndef __ANDROID__
#define USE_NATIVE_MEMORY_MANAGER
#endif

#ifdef USE_NATIVE_MEMORY_MANAGER
#include "MemoryManager.h"
#endif //USE_NATIVE_MEMORY_MANAGER

#ifndef BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT
#	define BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT 8
#endif // BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT

namespace bx
{
	DefaultAllocator::DefaultAllocator()
	{
	}

	DefaultAllocator::~DefaultAllocator()
	{
	}

	void* DefaultAllocator::realloc(void* _ptr, size_t _size, size_t _align, const char* _file, uint32_t _line)
	{
		//!!!!betauser
#ifdef USE_NATIVE_MEMORY_MANAGER


		if (0 == _size)
		{
			if (NULL != _ptr)
			{
				if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
				{
					Memory_Free(_ptr);
					return NULL;
				}

//#	if BX_COMPILER_MSVC
//				BX_UNUSED(_file, _line);
//				Memory_FreeAligned(_ptr);
//#	else
				bx::alignedFree(this, _ptr, _align, _file, _line);
//#	endif // BX_
			}

			return NULL;
		}
		else if (NULL == _ptr)
		{
			if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
			{
				return Memory_Alloc(MemoryAllocationType_Renderer, (int)_size, __FILE__, __LINE__);
			}

//#	if BX_COMPILER_MSVC
//			BX_UNUSED(_file, _line);
//			return Memory_AllocAligned(MemoryAllocationType_Renderer, _size, _align, __FILE__, __LINE__);
//#	else
			return bx::alignedAlloc(this, _size, _align, _file, _line);
//#	endif // BX_
		}

		if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
		{
			return Memory_Realloc(MemoryAllocationType_Renderer, _ptr, (int)_size, __FILE__, __LINE__);
		}

//#	if BX_COMPILER_MSVC
//		BX_UNUSED(_file, _line);
//		return Memory_ReallocAligned(MemoryAllocationType_Renderer, _ptr, _size, _align, __FILE__, __LINE__);
//#	else
		return bx::alignedRealloc(this, _ptr, _size, _align, _file, _line);
//#	endif // BX_


#else //USE_NATIVE_MEMORY_MANAGER


		if (0 == _size)
		{
			if (NULL != _ptr)
			{
				if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
				{
					::free(_ptr);
					return NULL;
				}

#	if BX_COMPILER_MSVC
				BX_UNUSED(_file, _line);
				_aligned_free(_ptr);
#	else
				bx::alignedFree(this, _ptr, _align, _file, _line);
#	endif // BX_
			}

			return NULL;
		}
		else if (NULL == _ptr)
		{
			if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
			{
				return ::malloc(_size);
			}

#	if BX_COMPILER_MSVC
			BX_UNUSED(_file, _line);
			return _aligned_malloc(_size, _align);
#	else
			return bx::alignedAlloc(this, _size, _align, _file, _line);
#	endif // BX_
		}

		if (BX_CONFIG_ALLOCATOR_NATURAL_ALIGNMENT >= _align)
		{
			return ::realloc(_ptr, _size);
		}

#	if BX_COMPILER_MSVC
		BX_UNUSED(_file, _line);
		return _aligned_realloc(_ptr, _size, _align);
#	else
		return bx::alignedRealloc(this, _ptr, _size, _align, _file, _line);
#	endif // BX_


#endif //USE_NATIVE_MEMORY_MANAGER

	}

} // namespace bx
