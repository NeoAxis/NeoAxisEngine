// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "MemoryManagerInternal_precompiled.h"
#pragma hdrstop
#include "MemoryManagerInternal.h"

#ifdef NATIVE_MEMORY_MANAGER_ENABLE

#undef malloc
#undef free
#undef realloc

///////////////////////////////////////////////////////////////////////////////////////////////////

class PagedMemoryManager : public MemoryManager
{
	unsigned int* locked;

	class Page;
	class PageCollection;

	//////////////////////////////////////////////

	struct AllocationHeader
	{
		Page* page;
		AllocationHeader* previous; //for Page.allocatedItems or for MemoryManager.largeMemoryItems
		AllocationHeader* next; //for Page.allocatedItems or for MemoryManager.largeMemoryItems

		int size;
		const char* fileName;
		uint16 lineNumber;
		uint8/*MemoryAllocationType*/ allocationType;
		uint8 _forAlign1;
	};

	//////////////////////////////////////////////

	class Page
	{
	public:
		PageCollection* pageCollection;
		Page* pages_previous;
		Page* pages_next;
		Page* freePages_previous;
		Page* freePages_next;

	private:
		void* data;

		int freeItemCount;
		AllocationHeader** freeItems;
		int allocatedItemCount;
		AllocationHeader* allocatedItems;

	public:

		//

		void Init(PageCollection* pageCollection)
		{
			this->pageCollection = pageCollection;
			pages_previous = NULL;
			pages_next = NULL;
			freePages_previous = NULL;
			freePages_next = NULL;

			freeItemCount = 0;

			int mallocSize;

			mallocSize = pageCollection->GetMaximumItemCount() * sizeof(AllocationHeader*);
			freeItems = (AllocationHeader**)malloc(mallocSize);
			crtAllocatedMemory += mallocSize;
			crtAllocationCount++;

			mallocSize = pageCollection->GetItemSize() * pageCollection->GetMaximumItemCount();
			data = malloc(mallocSize);
			crtAllocatedMemory += mallocSize;
			crtAllocationCount++;

			uint8* pointer = (uint8*)data;
			for(int n = 0; n < pageCollection->GetMaximumItemCount(); n++)
			{
				AllocationHeader* header = (AllocationHeader*)pointer;
				
				header->page = this;
				header->previous = NULL;
				header->next = NULL;

				freeItems[freeItemCount] = header;
				freeItemCount++;

				pointer += pageCollection->GetItemSize();
			}

			allocatedItemCount = 0;
			allocatedItems = NULL;
		}

		void Shutdown()
		{
			if(freeItems)
			{
				crtAllocatedMemory -= pageCollection->GetMaximumItemCount() * sizeof(AllocationHeader*);
				crtAllocationCount--;
				free(freeItems);

				freeItems = NULL;
			}

			if(data)
			{
				crtAllocatedMemory -= pageCollection->GetItemSize() * pageCollection->GetMaximumItemCount();
				crtAllocationCount--;
				free(data);

				data = NULL;
			}
		}

		AllocationHeader* Alloc()
		{
			//if(allocatedItemCount >= pageCollection->GetMaximumItemCount())
			//	Fatal("MemoryManager.Page.Free: allocatedItemCount >= pageCollection->GetMaximumItemCount()");
			//if(freeItemCount <= 0)
			//	Fatal("MemoryManager.Page.Free: freeItemCount <= 0");

			//remove from "freeItems"
			freeItemCount--;
			AllocationHeader* header = freeItems[freeItemCount];

			//add to "allocatedItems"
			{
				header->previous = NULL;
				header->next = allocatedItems;
				if(allocatedItems)
					allocatedItems->previous = header;
				allocatedItems = header;

				allocatedItemCount++;
			}

			return header;
		}

		void Free(AllocationHeader* header)
		{
			//if(allocatedItemCount <= 0)
			//	Fatal("MemoryManager.Page.Free: allocatedItemCount <= 0");
			//if(freeItemCount >= pageCollection->GetMaximumItemCount())
			//	Fatal("MemoryManager.Page.Free: freeItemCount >= pageCollection->GetMaximumItemCount()");

			//remove from "allocatedItems"
			{
				allocatedItemCount--;

				AllocationHeader* previous = header->previous;
				AllocationHeader* next = header->next;
				if(previous)
					previous->next = next;
				if(next)
					next->previous = previous;
				if(allocatedItems == header)
					allocatedItems = next;
			}

			//add to "freeItems"
			freeItems[freeItemCount] = header;
			freeItemCount++;
		}

		bool IsEmpty()
		{
			return allocatedItemCount == 0;
		}

		bool IsFull()
		{
			return freeItemCount == 0;
		}

		AllocationHeader* GetAllocatedItems()
		{
			return allocatedItems;
		}

	};

	//////////////////////////////////////////////

	class PageCollection
	{
		int itemSize; //size with header
		int maximumItemCount;
		Page* pages;
		Page* freePages;

		//

		void AddToPages(Page* page)
		{
			if(page->pages_previous)
				Fatal("Page.AddToPages: page->pages_previous != NULL");
			if(page->pages_next)
				Fatal("Page.AddToPages: page->pages_next != NULL");

			//page->pages_previous = NULL;
			page->pages_next = pages;
			if(pages)
				pages->pages_previous = page;
			pages = page;
		}
		
		void RemoveFromPages(Page* page)
		{
			Page* previous = page->pages_previous;
			Page* next = page->pages_next;
			if(previous)
				previous->pages_next = next;
			if(next)
				next->pages_previous = previous;
			if(pages == page)
				pages = next;

			page->pages_previous = NULL;
			page->pages_next = NULL;
		}

		void AddToFreePages(Page* page)
		{
			if(page->freePages_previous)
				Fatal("Page.AddToFreePages: page->freePages_previous != NULL");
			if(page->freePages_next)
				Fatal("Page.AddToFreePages: page->freePages_next != NULL");

			//page->freePages_previous = NULL;
			page->freePages_next = freePages;
			if(freePages)
				freePages->freePages_previous = page;
			freePages = page;
		}

		void RemoveFromFreePages(Page* page)
		{
			Page* previous = page->freePages_previous;
			Page* next = page->freePages_next;
			if(previous)
				previous->freePages_next = next;
			if(next)
				next->freePages_previous = previous;
			if(freePages == page)
				freePages = next;

			page->freePages_previous = NULL;
			page->freePages_next = NULL;
		}

		Page* CreatePage()
		{
			Page* page = (Page*)malloc(sizeof(Page));
			crtAllocatedMemory += sizeof(Page);
			crtAllocationCount++;

			page->Init(this);
			//Page* page = new Page(this);

			AddToPages(page);
			AddToFreePages(page);

			return page;
		}

		void DeletePage(Page* page)
		{
			//need if empty
			if(!page->IsEmpty())
				Fatal("MemoryManager: PageCollection: DeletePage: Not empty.");
			RemoveFromPages(page);
			RemoveFromFreePages(page);

			page->Shutdown();

			crtAllocatedMemory -= sizeof(Page);
			crtAllocationCount--;
			free(page);
		}

	public:

		//PageCollection(int itemSize)
		void Init(int itemSize, int maximumItemCount)
		{
			this->itemSize = itemSize;
			this->maximumItemCount = maximumItemCount;
			pages = NULL;
			freePages = NULL;
		}

		int GetItemSize()
		{
			return itemSize;
		}

		int GetMaximumItemCount()
		{
			return maximumItemCount;
		}

		AllocationHeader* Alloc()
		{
			Page* page;
			{
				if(!freePages)
					CreatePage();
				page = freePages;
			}

			AllocationHeader* header = page->Alloc();

			if(page->IsFull())
				RemoveFromFreePages(page);

			return header;
		}

		void Free(AllocationHeader* header)
		{
			Page* page = header->page;

			bool lastFull = page->IsFull();

			page->Free(header);

			if(lastFull)
				AddToFreePages(page);

			if(page->IsEmpty())
			{
				DeletePage(page);
			}
		}

		Page* GetPages()
		{
			return pages;
		}
	};

	//////////////////////////////////////////////

	enum AllocationSizeTypes
	{
		AllocationSizeTypes_Large = -1,

		AllocationSizeTypes_0_8 = 0,
		AllocationSizeTypes_9_16,
		AllocationSizeTypes_17_32,
		AllocationSizeTypes_33_64,
		AllocationSizeTypes_65_128,
		AllocationSizeTypes_129_256,
		AllocationSizeTypes_257_512,
		AllocationSizeTypes_513_1024,

		AllocationSizeTypes_Count,
	};

	//////////////////////////////////////////////

	PageCollection* pageCollections[AllocationSizeTypes_Count];
	AllocationHeader* largeMemoryItems;

	int allocatedMemory[MemoryAllocationType_Count];
	int allocationCount[MemoryAllocationType_Count];
	//"static" for access from all classes
	static int crtAllocatedMemory;
	static int crtAllocationCount;

	//////////////////////////////////////////////

public:

	PagedMemoryManager()
	{
		locked = (unsigned int*)malloc(4);
		*locked = 0;

		largeMemoryItems = NULL;
		memset(allocatedMemory, 0, sizeof(allocatedMemory));
		memset(allocationCount, 0, sizeof(allocationCount));
		crtAllocatedMemory = 0;
		crtAllocationCount = 0;

		if(sizeof(AllocationHeader) % 8 != 0)
			Fatal("MemoryManager: sizeof(AllocationInfo) % 8 != 0");
		if(sizeof(MemoryAllocationType) != 4)
			Fatal("MemoryManager: MemoryAllocationType != 4");

		int headerSize = sizeof(AllocationHeader);

		for(int n = 0; n < AllocationSizeTypes_Count; n++)
		{
			pageCollections[n] = (PageCollection*)malloc(sizeof(PageCollection));
			crtAllocatedMemory += sizeof(PageCollection);
			crtAllocationCount++;
		}

		pageCollections[AllocationSizeTypes_0_8]->Init(headerSize + 8, 256);
		pageCollections[AllocationSizeTypes_9_16]->Init(headerSize + 16, 256);
		pageCollections[AllocationSizeTypes_17_32]->Init(headerSize + 32, 128);
		pageCollections[AllocationSizeTypes_33_64]->Init(headerSize + 64, 128);
		pageCollections[AllocationSizeTypes_65_128]->Init(headerSize + 128, 128);
		pageCollections[AllocationSizeTypes_129_256]->Init(headerSize + 256, 64);
		pageCollections[AllocationSizeTypes_257_512]->Init(headerSize + 512, 32);
		pageCollections[AllocationSizeTypes_513_1024]->Init(headerSize + 1024, 16);
	}

	xx xx; int64_t
	void GetStatistics(MemoryAllocationType allocationType, int64_t* allocatedMemory, int* allocationCount)
	{
		xx xx;

		*allocatedMemory = this->allocatedMemory[allocationType];
		*allocationCount = this->allocationCount[allocationType];
	}

	void GetCRTStatistics(int64_t* allocatedMemory, int* allocationCount)
	{
		*allocatedMemory = crtAllocatedMemory;
		*allocationCount = crtAllocationCount;
	}

	AllocationSizeTypes GetAllocationSizeTypeBySize(int size)
	{
		if(size <= 64)
		{
			if(size <= 16)
			{
				if(size <= 8)
					return AllocationSizeTypes_0_8;
				else
					return AllocationSizeTypes_9_16;
			}
			else
			{
				if(size <= 32)
					return AllocationSizeTypes_17_32;
				else
					return AllocationSizeTypes_33_64;
			}
		}
		else
		{
			if(size <= 256)
			{
				if(size <= 128)
					return AllocationSizeTypes_65_128;
				else
					return AllocationSizeTypes_129_256;
			}
			else
			{
				if(size <= 1024)
				{
					if(size <= 512)
						return AllocationSizeTypes_257_512;
					else
						return AllocationSizeTypes_513_1024;
				}
				else
					return AllocationSizeTypes_Large;
			}
		}
	}

	void MutexEnter()
	{
		while (*locked == 1 || InterlockedCompareExchange(locked, 1, 0) == 1)
		{
		}
	}

	void MutexLeave()
	{
		*locked = 0;
	}

	void* Alloc( MemoryAllocationType allocationType, int size, const char* fileName, const int lineNumber )
	{
		//!!!!!!optimization: fully separate by allocationType

		AllocationSizeTypes allocationSizeType = GetAllocationSizeTypeBySize(size);

		AllocationHeader* header;

		if(allocationSizeType != AllocationSizeTypes_Large)
		{
			//usual memory allocation
			PageCollection* pageCollection = pageCollections[allocationSizeType];
			header = pageCollection->Alloc();
		}
		else
		{
			//large memory allocation

			header = (AllocationHeader*)malloc(sizeof(AllocationHeader) + size);
			crtAllocatedMemory += sizeof(AllocationHeader) + size;
			crtAllocationCount++;

			header->page = NULL;
			//header->previous = NULL;
			//header->next = NULL;

			//add to "largeMemoryItems"
			{
				header->previous = NULL;
				header->next = largeMemoryItems;
				if(largeMemoryItems)
					largeMemoryItems->previous = header;
				largeMemoryItems = header;
			}
		}

		header->size = size;
		header->fileName = fileName;
		int num = lineNumber;
		if(num > 65535)
			num = 0;
		header->lineNumber = (uint16)num;
		header->allocationType = allocationType;

		allocatedMemory[allocationType] += size;
		allocationCount[allocationType]++;

		return (uint8*)header + sizeof(AllocationHeader);
	}

	void* Realloc( MemoryAllocationType allocationType, void* pointer, int newSize, const char* fileName, const int lineNumber )
	{
		AllocationHeader* header = (AllocationHeader*)((uint8*)pointer - sizeof(AllocationHeader));

		int oldSize = header->size;

		void* newPointer;
		if(newSize)
		{
			newPointer = Alloc(allocationType, newSize, fileName, lineNumber);
			memcpy(newPointer, pointer, (oldSize < newSize ? oldSize : newSize));
		}
		else
			newPointer = NULL;
		Free(pointer);

		return newPointer;
	}

	void Free( void* pointer )
	{
		AllocationHeader* header = (AllocationHeader*)((uint8*)pointer - sizeof(AllocationHeader));

		allocatedMemory[header->allocationType] -= header->size;
		allocationCount[header->allocationType]--;

		if(header->page)
		{
			//usual memory allocation
			header->page->pageCollection->Free(header);
		}
		else
		{
			//large memory allocation

			//remove from "largeMemoryItems"
			{
				AllocationHeader* previous = header->previous;
				AllocationHeader* next = header->next;
				if(previous)
					previous->next = next;
				if(next)
					next->previous = previous;
				if(largeMemoryItems == header)
					largeMemoryItems = next;
			}

			crtAllocatedMemory -= sizeof(AllocationHeader) + header->size;
			crtAllocationCount--;
			free(header);
		}
	}

	//struct GetAllocationInformation_Compare
	//{
	//	bool operator()(const AllocationHeader* left, const AllocationHeader* right) const
	//	{
	//		if(left->allocationType < right->allocationType)
	//			return true;
	//		if(left->allocationType > right->allocationType)
	//			return false;

	//		if(left->size > right->size)
	//			return true;
	//		if(left->size < right->size)
	//			return false;

	//		if(left->fileName > right->fileName)
	//			return true;
	//		if(left->fileName < right->fileName)
	//			return false;

	//		if(left->lineNumber > right->lineNumber)
	//			return true;
	//		if(left->lineNumber < right->lineNumber)
	//			return false;

	//		return false;
	//	}
	//};

	/////////////////////

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

	/////////////////////

	//void GetAllocationInformation2( MemoryManager_GetAllocationInformationDelegate* callback )
	//{
	//	typedef std::map<AllocationHeader*, int, GetAllocationInformation_Compare, 
	//		stdmap_crt_allocator<AllocationHeader*> > stdcrtmap;

	//	stdcrtmap list;

	//	//usual memory
	//	for(int n = 0; n < AllocationSizeTypes_Count; n++)
	//	{
	//		PageCollection* pageCollection = pageCollections[n];

	//		Page* page = pageCollection->GetPages();
	//		while(page)
	//		{
	//			AllocationHeader* header = page->GetAllocatedItems();
	//			while(header)
	//			{
	//				stdcrtmap::iterator it = list.find(header);
	//				if(it != list.end())
	//				{
	//					int allocationCount = (*it).second;
	//					allocationCount++;
	//					list[header] = allocationCount;
	//				}
	//				else
	//					list[header] = 1;

	//				header = header->next;
	//			}

	//			page = page->pages_next;
	//		}
	//	}

	//	//large memory
	//	{
	//		AllocationHeader* header = largeMemoryItems;
	//		while(header)
	//		{
	//			stdcrtmap::iterator it = list.find(header);
	//			if(it != list.end())
	//			{
	//				int allocationCount = (*it).second;
	//				allocationCount++;
	//				list[header] = allocationCount;
	//			}
	//			else
	//				list[header] = 1;

	//			header = header->next;
	//		}
	//	}

	//	for(stdcrtmap::iterator it = list.begin(); it != list.end(); it++)
	//	{
	//		AllocationHeader* header = (*it).first;
	//		int allocationCount = (*it).second;

	//		callback((MemoryAllocationType)header->allocationType, header->size, 
	//			GetCorrectFileNamePointer(header->fileName), header->lineNumber, allocationCount);
	//	}
	//}

	void GetAllocationInformation( MemoryManager_GetAllocationInformationDelegate* callback )
	{
		//GetAllocationInformation2(callback);
	}

};

///////////////////////////////////////////////////////////////////////////////////////////////////

xx xx;
int PagedMemoryManager::crtAllocatedMemory;
int PagedMemoryManager::crtAllocationCount;

///////////////////////////////////////////////////////////////////////////////////////////////////

MemoryManager* CreatePagedMemoryManager()
{
	return new PagedMemoryManager();
}

///////////////////////////////////////////////////////////////////////////////////////////////////

#endif// NATIVE_MEMORY_MANAGER_ENABLE
