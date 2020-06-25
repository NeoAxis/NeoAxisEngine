// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "MemoryManagerInternal_precompiled.h"
#pragma hdrstop
#include "MemoryManagerInternal.h"

//#ifdef NATIVE_MEMORY_MANAGER_ENABLE
//
//#include "tcmalloc.h"
//#include "tcmalloc.h.in"
//
//#undef malloc
//#undef free
//#undef realloc
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//class TCMallocMemoryManager : public MemoryManager
//{
//public:
//
//	TCMallocMemoryManager()
//	{
//	}
//
//	void GetStatistics(MemoryAllocationType allocationType, int64_t* allocatedMemory, int* allocationCount)
//	{
//		int64_t physicalUsed = 0;
//		int64_t usedByApp = 0;
//		tc_get_stats(&physicalUsed, &usedByApp);
//		*allocatedMemory = usedByApp;
//		*allocationCount = 0;
//	}
//
//	void GetCRTStatistics(int64_t* allocatedMemory, int* allocationCount)
//	{
//		int64_t physicalUsed = 0;
//		int64_t usedByApp = 0;
//		tc_get_stats(&physicalUsed, &usedByApp);
//		*allocatedMemory = physicalUsed;
//		*allocationCount = 0;
//	}
//
//	void* Alloc( MemoryAllocationType allocationType, int size, const char* fileName, const int lineNumber )
//	{
//		//!!!!optimization: fully separate by allocationType
//
//		return tc_malloc( size );
//	}
//
//	void* Realloc( MemoryAllocationType allocationType, void* pointer, int newSize, const char* fileName, const int lineNumber )
//	{
//		return tc_realloc( pointer, newSize );
//	}
//
//	void Free( void* pointer )
//	{
//		tc_free( pointer );
//	}
//
//	//struct GetAllocationInformation_Compare
//	//{
//	//	bool operator()(const AllocationHeader* left, const AllocationHeader* right) const
//	//	{
//	//		if(left->allocationType < right->allocationType)
//	//			return true;
//	//		if(left->allocationType > right->allocationType)
//	//			return false;
//
//	//		if(left->size > right->size)
//	//			return true;
//	//		if(left->size < right->size)
//	//			return false;
//
//	//		if(left->fileName > right->fileName)
//	//			return true;
//	//		if(left->fileName < right->fileName)
//	//			return false;
//
//	//		if(left->lineNumber > right->lineNumber)
//	//			return true;
//	//		if(left->lineNumber < right->lineNumber)
//	//			return false;
//
//	//		return false;
//	//	}
//	//};
//
//	/////////////////////
//
////	template <class T> class stdmap_crt_allocator
////	{
////		public:
////		typedef T value_type;
////		typedef T* pointer;
////		typedef const T* const_pointer;
////		typedef T& reference;
////		typedef const T& const_reference;
////		typedef std::size_t size_type;
////		typedef std::ptrdiff_t difference_type;
////
////		template <class U> struct rebind
////		{
////			typedef stdmap_crt_allocator<U> other;
////		};
////
////		pointer address(reference value) const
////		{
////			return &value;
////		}
////
////		const_pointer address(const_reference value) const
////		{
////			return &value;
////		}
////
////		stdmap_crt_allocator() throw()
////		{
////		}
////
////		stdmap_crt_allocator(const stdmap_crt_allocator&) throw()
////		{
////		}
////
////		template <class U> stdmap_crt_allocator(const stdmap_crt_allocator<U>&) throw()
////		{
////		}
////
////		~stdmap_crt_allocator() throw()
////		{
////		}
////
////		size_type max_size() const throw()
////		{
////			size_t c = (size_t)(-1) / sizeof(T);
////			return (0 < c ? c : 1);
////		}
////
////		pointer allocate(size_type num, const void* = 0)
////		{
////			return (T*)malloc(num * sizeof(T));
////		}
////
////		void construct(pointer p, const T& value)
////		{
////			new((void*)p)T(value);
////		}
////
////		void destroy(pointer p)
////		{
////			p->~T();
////		}
////
////		void deallocate(pointer p, size_type num)
////		{
////			free(p);
////		}
////
//////#ifdef PLATFORM_MACOS
//////
//////		typedef std::pair<AllocationHeader* const, int> mypair;
//////
//////		void construct(mypair* p, const mypair& val)
//////		{
//////			::new(p)std::pair<AllocationHeader*, int>(val);
//////		}
//////
//////		void destroy(mypair* p)
//////		{
//////			p->~mypair();
//////		}
//////
//////#endif
////
////	};
//
//	/////////////////////
//
//	//void GetAllocationInformation2( MemoryManager_GetAllocationInformationDelegate* callback )
//	//{
//	//	typedef std::map<AllocationHeader*, int, GetAllocationInformation_Compare, 
//	//		stdmap_crt_allocator<AllocationHeader*> > stdcrtmap;
//
//	//	stdcrtmap list;
//
//	//	//usual memory
//	//	for(int n = 0; n < AllocationSizeTypes_Count; n++)
//	//	{
//	//		PageCollection* pageCollection = pageCollections[n];
//
//	//		Page* page = pageCollection->GetPages();
//	//		while(page)
//	//		{
//	//			AllocationHeader* header = page->GetAllocatedItems();
//	//			while(header)
//	//			{
//	//				stdcrtmap::iterator it = list.find(header);
//	//				if(it != list.end())
//	//				{
//	//					int allocationCount = (*it).second;
//	//					allocationCount++;
//	//					list[header] = allocationCount;
//	//				}
//	//				else
//	//					list[header] = 1;
//
//	//				header = header->next;
//	//			}
//
//	//			page = page->pages_next;
//	//		}
//	//	}
//
//	//	//large memory
//	//	{
//	//		AllocationHeader* header = largeMemoryItems;
//	//		while(header)
//	//		{
//	//			stdcrtmap::iterator it = list.find(header);
//	//			if(it != list.end())
//	//			{
//	//				int allocationCount = (*it).second;
//	//				allocationCount++;
//	//				list[header] = allocationCount;
//	//			}
//	//			else
//	//				list[header] = 1;
//
//	//			header = header->next;
//	//		}
//	//	}
//
//	//	for(stdcrtmap::iterator it = list.begin(); it != list.end(); it++)
//	//	{
//	//		AllocationHeader* header = (*it).first;
//	//		int allocationCount = (*it).second;
//
//	//		callback((MemoryAllocationType)header->allocationType, header->size, 
//	//			GetCorrectFileNamePointer(header->fileName), header->lineNumber, allocationCount);
//	//	}
//	//}
//
//	void GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback )
//	{
//		//GetAllocationInformation2(callback);
//	}
//
//};
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//MemoryManager* CreateTCMallocMemoryManager()
//{
//	return new TCMallocMemoryManager();
//}
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//#endif// NATIVE_MEMORY_MANAGER_ENABLE
