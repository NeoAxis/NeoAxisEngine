// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

class MyOgreVirtualDataStream;
class MyOgreVirtualArchiveFactory;

///////////////////////////////////////////////////////////////////////////////////////////////////

typedef bool MyOgreVirtualDataStream_openDelegate(const MyOgreVirtualDataStream* stream, 
	const wchar16* fileName, int* streamSize, bool* fileNotFound);
typedef void MyOgreVirtualDataStream_closeDelegate(const MyOgreVirtualDataStream* stream);
typedef int MyOgreVirtualDataStream_readDelegate(const MyOgreVirtualDataStream* stream, 
	void* buf, int count);
typedef void MyOgreVirtualDataStream_skipDelegate(const MyOgreVirtualDataStream* stream, int count);
typedef void MyOgreVirtualDataStream_seekDelegate(const MyOgreVirtualDataStream* stream, int pos);
typedef int MyOgreVirtualDataStream_tellDelegate(const MyOgreVirtualDataStream* stream);
typedef bool MyOgreVirtualArchive_findDelegate(const wchar16* pattern, bool recursive, bool dirs, 
	void* userData);
typedef bool MyOgreVirtualArchive_findFileInfoDelegate(const wchar16* pattern, bool recursive, bool dirs, 
	void* userData);
typedef bool MyOgreVirtualArchive_fileExistsDelegate(const wchar16* fileName);

///////////////////////////////////////////////////////////////////////////////////////////////////

class MyOgreVirtualDataStream : public DataStream
{
public:
	MyOgreVirtualArchiveFactory* myOgreVirtualArchiveFactory;
	bool closed;

	MyOgreVirtualDataStream(const WString& name);
	void setSize(int size);
	virtual ~MyOgreVirtualDataStream();
	virtual size_t read( void* buf, size_t count );
	virtual void skip( long count );
	virtual void seek( size_t pos );
	virtual size_t tell() const;
	virtual bool eof() const;
	virtual void close();
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class MyOgreVirtualArchive : public Archive
{
public:
	MyOgreVirtualArchive(const String& name, const String& archType );
	~MyOgreVirtualArchive();
	bool isCaseSensitive(void) const { return false; }
	void load() {}
	void unload() {}
	DataStreamPtr open(const WString& filename, bool readOnly = true, bool* fileNotFound = NULL) const;
	WStringVectorPtr list(bool recursive = true, bool dirs = false);
	FileInfoListPtr listFileInfo(bool recursive = true, bool dirs = false);
	WStringVectorPtr find(const WString& pattern, bool recursive = true, bool dirs = false);
	FileInfoListPtr findFileInfo(const WString& pattern, bool recursive = true, bool dirs = false);
	bool exists(const WString& filename);
	time_t getModifiedTime(const WString& filename);
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class MyOgreVirtualArchiveFactory : public ArchiveFactory
{
public:
	String type;

	MyOgreVirtualDataStream_openDelegate* MyOgreVirtualDataStream_open;
	MyOgreVirtualDataStream_closeDelegate* MyOgreVirtualDataStream_close;
	MyOgreVirtualDataStream_readDelegate* MyOgreVirtualDataStream_read;
	MyOgreVirtualDataStream_skipDelegate* MyOgreVirtualDataStream_skip;
	MyOgreVirtualDataStream_seekDelegate* MyOgreVirtualDataStream_seek;
	MyOgreVirtualDataStream_tellDelegate* MyOgreVirtualDataStream_tell;
	MyOgreVirtualArchive_findDelegate* MyOgreVirtualArchive_find;
	MyOgreVirtualArchive_findFileInfoDelegate* MyOgreVirtualArchive_findFileInfo;
	MyOgreVirtualArchive_fileExistsDelegate* MyOgreVirtualArchive_fileExists;

	MyOgreVirtualArchive* archive;

	//

	MyOgreVirtualArchiveFactory(
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
		type = "VirtualFileSystem";

		if(root->myOgreVirtualArchiveFactory)
			OGRE_EXCEPT(Exception::ERR_INTERNAL_ERROR,  "root->myOgreVirtualArchiveFactory != NULL", "");
		root->myOgreVirtualArchiveFactory = this;

		MyOgreVirtualDataStream_open = open;
		MyOgreVirtualDataStream_close = close;
		MyOgreVirtualDataStream_read = read;
		MyOgreVirtualDataStream_skip = skip;
		MyOgreVirtualDataStream_seek = seek;
		MyOgreVirtualDataStream_tell = tell;
		MyOgreVirtualArchive_find = find;
		MyOgreVirtualArchive_findFileInfo = findFileInfo;
		MyOgreVirtualArchive_fileExists = fileExists;

		archive = NULL;
	}
	
	virtual ~MyOgreVirtualArchiveFactory() {}

   const String& getType() const
	{
		return type;
	}

   Archive* createInstance( const String& name ) 
   {
		if(archive)
		{
			OGRE_EXCEPT(Exception::ERR_INTERNAL_ERROR,  
				"MyOgreVirtualArchiveFactory: createInstance: archive != NULL", "");
		}

		archive = new MyOgreVirtualArchive(name, "VirtualFileSystem");
		return archive;
   }

   void destroyInstance( Archive* arch )
   {
		delete arch;
		archive = NULL;
   }
};
