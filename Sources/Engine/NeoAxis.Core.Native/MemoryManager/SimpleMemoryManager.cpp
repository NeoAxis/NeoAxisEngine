// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "MemoryManagerInternal_precompiled.h"
#pragma hdrstop
#include "MemoryManagerInternal.h"

//#ifdef NATIVE_MEMORY_MANAGER_ENABLE
//
//#define ALLOW_CHECK_BROKEN_MEMORY
////#define ALLOW_HARD_CHECK_BROKEN_MEMORY
//
//#undef malloc
//#undef free
//#undef realloc
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//class SimpleMemoryManager : public MemoryManager
//{
//	struct AllocationHeader
//	{
//		int size;
//		const char* fileName;
//		int lineNumber;
//
//		AllocationHeader* previous;
//		AllocationHeader* next;
//
//		uint8/*MemoryAllocationType*/ allocationType;
//
//		enum State
//		{
//			State_Allocated = 66,
//			State_Freed = 123,
//		};
//		uint8/*State*/ state;
//		
//		uint8 _forAlign1;
//		uint8 _forAlign2;
//	};
//
//	//////////////////////////////////////////////
//
//	AllocationHeader* allocations;
//
//	int allocatedMemory[MemoryAllocationType_Count];
//	int allocationCount[MemoryAllocationType_Count];
//	int crtAllocatedMemory;
//	int crtAllocationCount;
//
//	//////////////////////////////////////////////
//
//public:
//
//	SimpleMemoryManager()
//	{
//		allocations = NULL;
//		memset(allocatedMemory, 0, sizeof(allocatedMemory));
//		memset(allocationCount, 0, sizeof(allocationCount));
//		crtAllocatedMemory = 0;
//		crtAllocationCount = 0;
//
//		if(sizeof(AllocationHeader) % 8 != 0)
//			Fatal("MemoryManager: sizeof(AllocationInfo) % 8 != 0");
//		if(sizeof(MemoryAllocationType) != 4)
//			Fatal("MemoryManager: MemoryAllocationType != 4");
//	}
//
//	~SimpleMemoryManager()
//	{
//	}
//
//	xx xx; int64_t
//	void GetStatistics(MemoryAllocationType allocationType,
//		int* allocatedMemory, int* allocationCount)
//	{
//		*allocatedMemory = this->allocatedMemory[allocationType];
//		*allocationCount = this->allocationCount[allocationType];
//	}
//
//	void GetCRTStatistics(int* allocatedMemory, int* allocationCount)
//	{
//		*allocatedMemory = crtAllocatedMemory;
//		*allocationCount = crtAllocationCount;
//	}
//
//	void* Alloc( MemoryAllocationType allocationType, int size, 
//		const char* fileName, const int lineNumber )
//	{
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		AllocationHeader* header = (AllocationHeader*)malloc(sizeof(AllocationHeader) + size + 4);
//		crtAllocatedMemory += sizeof(AllocationHeader) + size + 4;
//		crtAllocationCount++;
//
//		*((int*)((uint8*)header + sizeof(AllocationHeader) + size)) = 1;
//#else
//		AllocationHeader* header = (AllocationHeader*)malloc(sizeof(AllocationHeader) + size);
//		crtAllocatedMemory += sizeof(AllocationHeader) + size;
//		crtAllocationCount++;
//#endif
//
//		header->size = size;
//		header->fileName = fileName;
//		header->lineNumber = lineNumber;
//		header->previous = NULL;
//		header->next = allocations;
//		if(allocations)
//			allocations->previous = header;
//		allocations = header;
//
//		header->allocationType = allocationType;
//
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		header->state = AllocationHeader::State_Allocated;
//#endif
//
//		allocatedMemory[header->allocationType] += size;
//		allocationCount[header->allocationType]++;
//
//#ifdef ALLOW_HARD_CHECK_BROKEN_MEMORY
//		CheckBrokenMemory();
//#endif
//
//		return (uint8*)header + sizeof(AllocationHeader);
//	}
//
//	void* Realloc( MemoryAllocationType allocationType, void* pointer, int newSize, 
//		const char* fileName, const int lineNumber )
//	{
//		AllocationHeader* header = (AllocationHeader*)((uint8*)pointer - sizeof(AllocationHeader));
//
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		if(header->state == AllocationHeader::State_Freed)
//		{
//			const char* file = GetCorrectFileNamePointer(header->fileName);
//			if(!file)
//				file = "NULL";
//			int line = header->lineNumber;
//			char str[256];
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//			sprintf_s(str, sizeof(str), "NativeMemoryManager: Doublicate free (%s:%d).", file, line);
//#else
//			sprintf(str, "NativeMemoryManager: Doublicate free (%s:%d).", file, line);
//#endif
//			Fatal(str);
//		}
//		else
//		{
//			if(header->state != AllocationHeader::State_Allocated)
//			{
//				Fatal("NativeMemoryManager: Invalid memory.");
//			}
//		}
//#endif
//
//		int oldSize = header->size;
//
//		void* newPointer;
//		if(newSize)
//		{
//			newPointer = Alloc(allocationType, newSize, fileName, lineNumber);
//			memcpy(newPointer, pointer, (oldSize < newSize ? oldSize : newSize));
//		}
//		else
//			newPointer = NULL;
//		Free(pointer);
//
//#ifdef ALLOW_HARD_CHECK_BROKEN_MEMORY
//		CheckBrokenMemory();
//#endif
//
//		return newPointer;
//	}
//
//	void Free( void* pointer )
//	{
//		AllocationHeader* header = (AllocationHeader*)((uint8*)pointer - sizeof(AllocationHeader));
//
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		if(header->state == AllocationHeader::State_Freed)
//		{
//			const char* file = GetCorrectFileNamePointer(header->fileName);
//			if(!file)
//				file = "NULL";
//			int line = header->lineNumber;
//			char str[256];
//#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
//			sprintf_s(str, sizeof(str), "NativeMemoryManager: Doublicate free (%s:%d).", file, line);
//#else
//			sprintf(str, "NativeMemoryManager: Doublicate free (%s:%d).", file, line);
//#endif
//			Fatal(str);
//		}
//		else
//		{
//			if(header->state != AllocationHeader::State_Allocated)
//			{
//				Fatal("NativeMemoryManager: Invalid memory.");
//			}
//		}
//#endif
//
//		AllocationHeader* previous = header->previous;
//		AllocationHeader* next = header->next;
//
//		if(previous)
//			previous->next = next;
//		if(next)
//			next->previous = previous;
//
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		header->state = AllocationHeader::State_Freed;
//#endif
//
//		if(allocations == header)
//			allocations = next;
//
//		allocatedMemory[header->allocationType] -= header->size;
//		allocationCount[header->allocationType]--;
//
//#ifdef ALLOW_CHECK_BROKEN_MEMORY
//		crtAllocatedMemory -= sizeof(AllocationHeader) + header->size + 4;
//		crtAllocationCount--;
//#else
//		crtAllocatedMemory -= sizeof(AllocationHeader) + header->size;
//		crtAllocationCount--;
//#endif
//
//		free(header);
//
//#ifdef ALLOW_HARD_CHECK_BROKEN_MEMORY
//		CheckBrokenMemory();
//#endif
//	}
//
//#ifdef ALLOW_HARD_CHECK_BROKEN_MEMORY
//	void CheckBrokenMemory()
//	{
//		AllocationHeader* current = allocations;
//		while(current)
//		{
//			if(current->allocationType < 0 || current->allocationType >= MemoryAllocationType_Count)
//				Fatal("MemoryManager: Broken memory.");
//
//			if(*((int*)((uint8*)current + sizeof(AllocationHeader) + current->size)) != 1)
//				Fatal("MemoryManager: Broken memory.");
//
//			current = current->next;
//		}
//	}
//#endif
//
//	struct GetAllocationInformation_Compare
//	{
//		bool operator()(const AllocationHeader* left, const AllocationHeader* right) const
//		{
//			if(left->allocationType < right->allocationType)
//				return true;
//			if(left->allocationType > right->allocationType)
//				return false;
//
//			if(left->size > right->size)
//				return true;
//			if(left->size < right->size)
//				return false;
//
//			if(left->fileName > right->fileName)
//				return true;
//			if(left->fileName < right->fileName)
//				return false;
//
//			if(left->lineNumber > right->lineNumber)
//				return true;
//			if(left->lineNumber < right->lineNumber)
//				return false;
//
//			return false;
//		}
//	};
//
//	/////////////////////
//
//	template <class T> class stdmap_crt_allocator
//	{
//		public:
//		typedef T value_type;
//		typedef T* pointer;
//		typedef const T* const_pointer;
//		typedef T& reference;
//		typedef const T& const_reference;
//		typedef std::size_t size_type;
//		typedef std::ptrdiff_t difference_type;
//
//		template <class U> struct rebind
//		{
//			typedef stdmap_crt_allocator<U> other;
//		};
//
//		pointer address(reference value) const
//		{
//			return &value;
//		}
//
//		const_pointer address(const_reference value) const
//		{
//			return &value;
//		}
//
//		stdmap_crt_allocator() throw()
//		{
//		}
//
//		stdmap_crt_allocator(const stdmap_crt_allocator&) throw()
//		{
//		}
//
//		template <class U> stdmap_crt_allocator(const stdmap_crt_allocator<U>&) throw()
//		{
//		}
//
//		~stdmap_crt_allocator() throw()
//		{
//		}
//
//		size_type max_size() const throw()
//		{
//			size_t c = (size_t)(-1) / sizeof(T);
//			return (0 < c ? c : 1);
//		}
//
//		pointer allocate(size_type num, const void* = 0)
//		{
//			return (T*)malloc(num * sizeof(T));
//		}
//
//		void construct(pointer p, const T& value)
//		{
//			new((void*)p)T(value);
//		}
//
//		void destroy(pointer p)
//		{
//			p->~T();
//		}
//
//		void deallocate(pointer p, size_type num)
//		{
//			free(p);
//		}
//	
////#ifdef PLATFORM_OSX
////
////		typedef std::pair<AllocationHeader* const, int> mypair;
////
////		void construct(mypair* p, const mypair& val)
////		{
////			::new(p)std::pair<AllocationHeader*, int>(val);
////		}
////
////		void destroy(mypair* p)
////		{
////			p->~mypair();
////		}
////
////#endif
//
//	};
//
//	/////////////////////
//
//	void GetAllocationInformation2( MemoryManager_GetAllocationInformationDelegate* callback )
//	{
//		typedef std::map<AllocationHeader*, int, GetAllocationInformation_Compare, 
//			stdmap_crt_allocator<AllocationHeader*> > stdcrtmap;
//
//		stdcrtmap list;
//
//		AllocationHeader* current = allocations;
//		while(current)
//		{
//			stdcrtmap::iterator it = list.find(current);
//			if(it != list.end())
//			{
//				int allocationCount = (*it).second;
//				allocationCount++;
//				list[current] = allocationCount;
//			}
//			else
//			{
//				list[current] = 1;
//			}
//
//			current = current->next;
//		}
//
//		for(stdcrtmap::iterator it = list.begin(); it != list.end(); it++)
//		{
//			AllocationHeader* header = (*it).first;
//			int allocationCount = (*it).second;
//
//			callback((MemoryAllocationType)header->allocationType, header->size, 
//				GetCorrectFileNamePointer(header->fileName), header->lineNumber, allocationCount);
//		}
//	}
//
//	void GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback )
//	{
//		GetAllocationInformation2(callback);
//	}
//
//};
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//MemoryManager* CreateSimpleMemoryManager()
//{
//	return new SimpleMemoryManager();
//}
//
/////////////////////////////////////////////////////////////////////////////////////////////////////
//
//#endif// NATIVE_MEMORY_MANAGER_ENABLE
