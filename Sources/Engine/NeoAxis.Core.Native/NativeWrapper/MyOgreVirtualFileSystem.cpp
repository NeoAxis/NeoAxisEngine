// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OgreNativeWrapperGeneral.h"
#include "MyOgreVirtualFileSystem.h"

///////////////////////////////////////////////////////////////////////////////////////////////////

MyOgreVirtualDataStream::MyOgreVirtualDataStream(const WString& name)
   : DataStream(name)
{
	myOgreVirtualArchiveFactory = root->myOgreVirtualArchiveFactory;
	closed = false;
}

void MyOgreVirtualDataStream::setSize(int size)
{
	mSize = size;
}

MyOgreVirtualDataStream::~MyOgreVirtualDataStream()
{
   close();
}

size_t MyOgreVirtualDataStream::read( void* buf, size_t count )
{
	return myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_read(this, buf, count);
}

void MyOgreVirtualDataStream::skip( long count )
{
	myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_skip(this, count);
}

void MyOgreVirtualDataStream::seek( size_t pos )
{
	myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_seek(this, pos);
}

size_t MyOgreVirtualDataStream::tell() const
{
	return myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_tell(this);
}

bool MyOgreVirtualDataStream::eof() const
{
	return myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_tell(this) >= mSize;
}

void MyOgreVirtualDataStream::close()
{
	if(!closed)
	{
		myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_close(this);
		closed = true;
	}
}

///////////////////////////////////////////////////////////////////////////////////////////////////

MyOgreVirtualArchive::MyOgreVirtualArchive(const String& name, const String& archType ) 
	: Archive(name, archType)
{
}

MyOgreVirtualArchive::~MyOgreVirtualArchive()
{
	unload();
}

DataStreamPtr MyOgreVirtualArchive::open(const WString& filename, bool readOnly, bool* fileNotFound) const
{
	if(fileNotFound)
		*fileNotFound = false;

	MyOgreVirtualDataStream* stream = new MyOgreVirtualDataStream(filename);
	int size = 0;
	bool fileNotFoundLocal = false;

	if(!root->myOgreVirtualArchiveFactory->MyOgreVirtualDataStream_open(stream, 
		TO_WCHAR16(filename.c_str()), &size, &fileNotFoundLocal))
	{
		if(fileNotFound)
			*fileNotFound = fileNotFoundLocal;

		stream->closed = true;
		delete stream;
		return DataStreamPtr();
		//OGRE_EXCEPT(Exception::ERR_FILE_NOT_FOUND, 
		//	"Cannot open file: " + StringUtil::toUTF8(filename), "VirtualFile.Open");
	}
	stream->setSize(size);

	return DataStreamPtr(stream);
}

WStringVectorPtr MyOgreVirtualArchive::list(bool recursive, bool dirs)
{
	return find(L"*", recursive, dirs);
}

FileInfoListPtr MyOgreVirtualArchive::listFileInfo(bool recursive, bool dirs)
{
	return findFileInfo(L"*", recursive, dirs);
}

WStringVectorPtr MyOgreVirtualArchive::find(const WString& pattern, bool recursive, bool dirs)
{
	WStringVector* list = new WStringVector();
	if(!root->myOgreVirtualArchiveFactory->MyOgreVirtualArchive_find(TO_WCHAR16(pattern.c_str()), recursive, dirs, list))
	{
		delete list;
		
		OGRE_EXCEPT(Exception::ERR_INVALID_STATE, "Cannot get files", "OgreVirtualArchive.find");
	}
	return WStringVectorPtr(list);
}

FileInfoListPtr MyOgreVirtualArchive::findFileInfo(const WString& pattern, bool recursive, bool dirs)
{
	FileInfoList* list = new FileInfoList();
	if(!root->myOgreVirtualArchiveFactory->MyOgreVirtualArchive_findFileInfo(TO_WCHAR16(pattern.c_str()), 
		recursive, dirs, list))
	{
		delete list;
		
		OGRE_EXCEPT(Exception::ERR_INVALID_STATE, "Cannot get files", 
			"OgreVirtualArchive.findFileInfo");
	}
	return FileInfoListPtr(list);
}

bool MyOgreVirtualArchive::exists(const WString& filename)
{
	return root->myOgreVirtualArchiveFactory->MyOgreVirtualArchive_fileExists(TO_WCHAR16(filename.c_str()));
}

time_t MyOgreVirtualArchive::getModifiedTime(const WString& filename)
{
	return 0;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT MyOgreVirtualArchiveFactory* MyOgreVirtualArchiveFactory_New(Root* root,
	MyOgreVirtualDataStream_openDelegate* open, 
	MyOgreVirtualDataStream_closeDelegate* close, 
	MyOgreVirtualDataStream_readDelegate* read, 
	MyOgreVirtualDataStream_skipDelegate* skip, 
	MyOgreVirtualDataStream_seekDelegate* seek, 
	MyOgreVirtualDataStream_tellDelegate* tell, 
	MyOgreVirtualArchive_findDelegate* find, 
	MyOgreVirtualArchive_findFileInfoDelegate* findFileInfo,
	MyOgreVirtualArchive_fileExistsDelegate* fileExists)
{
	return new MyOgreVirtualArchiveFactory(open, close, read, skip, seek, tell, find, 
		findFileInfo, fileExists);
}

EXPORT void MyOgreVirtualArchiveFactory_Delete( MyOgreVirtualArchiveFactory* _this )
{
	delete _this;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT void MyOgreVirtualFileSystem_findAddItem( Root* root, wchar16* fileName, void* userData )
{
	WStringVector* list = (WStringVector*)userData;
	list->push_back(TO_WCHAR_T(fileName));
}

EXPORT void MyOgreVirtualFileSystem_findFileInfoAddItem( Root* root, wchar16* fileName, wchar16* path,
	wchar16* baseName, int compressedSize, int uncompressedSize, void* userData )
{
	FileInfoList* list = (FileInfoList*)userData;
	MyOgreVirtualArchive* archive = root->myOgreVirtualArchiveFactory->archive;

	FileInfo fileInfo;

	fileInfo.archive = archive;
	fileInfo.filename = TO_WCHAR_T(fileName);
	fileInfo.path = TO_WCHAR_T(path);
	fileInfo.basename = TO_WCHAR_T(baseName);
	fileInfo.compressedSize = compressedSize;
	fileInfo.uncompressedSize = uncompressedSize;

	list->push_back(fileInfo);
}

///////////////////////////////////////////////////////////////////////////////////////////////////