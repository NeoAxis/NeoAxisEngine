//// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#pragma once
//#ifndef __NativeMemoryDictionary_H_
//#define __NativeMemoryDictionary_H_
//
//#include "OgreStableHeaders.h"
//#include "StringUtils.h"
//
//#ifdef ANDROID
//#	define _wtoi(arg) wcstol(arg, NULL, 10)
//#	define _wtoi64(arg) wcstoll(arg, NULL, 10)
//#endif
//
//class NativeMemoryDictionary
//{
//	struct Item
//	{
//		unsigned char* valueArray;
//		int valueArraySize;
//		std::wstring valueString;
//
//		~Item()
//		{
//			if (valueArray)
//				delete[] valueArray;
//		}
//	};
//
//	typedef std::map<std::wstring, Item*> ItemMap;
//	ItemMap items;
//
//public:
//	NativeMemoryDictionary()
//	{
//	}
//
//	~NativeMemoryDictionary()
//	{
//		for (ItemMap::iterator it = items.begin(); it != items.end(); it++)
//			delete it->second;
//		items.clear();
//	}
//
//	void SetString(const std::wstring& name, const std::wstring& value)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//			delete it->second;
//
//		Item* item = new Item();
//		item->valueArray = nullptr;
//		item->valueArraySize = 0;
//		item->valueString = value;
//		items[name] = item;
//	}
//	void SetString(const std::string& name, const std::string& value)
//	{
//		SetString(ConvertStringToUTFWide(name), ConvertStringToUTFWide(value));
//	}
//
//	void SetInteger(const std::wstring& name, int value)
//	{
//		SetString(name, std::to_wstring(static_cast<long long>(value)));
//	}
//	void SetInteger(const std::string& name, int value)
//	{
//		SetInteger(ConvertStringToUTFWide(name), value);
//	}
//
//	void SetPointer(const std::wstring& name, void* value)
//	{
//		SetString(name, std::to_wstring((long long)value));
//	}
//	void SetPointer(const std::string& name, void* value)
//	{
//		SetPointer(ConvertStringToUTFWide(name), value);
//	}
//
//	void* AllocArray(const std::wstring& name, int sizeInBytes)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//			delete it->second;
//
//		Item* item = new Item();
//		item->valueArray = new unsigned char[sizeInBytes];
//		//memcpy(item->valueArray, data, dataInBytes);
//		item->valueArraySize = sizeInBytes;
//		items[name] = item;
//
//		return item->valueArray;
//	}
//	void* AllocArray(const std::string& name, int sizeInBytes)
//	{
//		return AllocArray(ConvertStringToUTFWide(name), sizeInBytes);
//	}
//
//	bool GetString(const std::wstring& name, std::wstring& value)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//		{
//			value = it->second->valueString;
//			return true;
//		}
//		return false;
//	}
//	bool GetString(const std::string& name, std::string& value)
//	{
//#	ifdef ANDROID		
//		std::wstring tmp;
//		bool res = GetString(ConvertStringToUTFWide(name), tmp);
//		value = ConvertStringToUTF8(tmp);
//		return res;
//#	else
//		return GetString(ConvertStringToUTFWide(name), ConvertStringToUTFWide(value));
//#	endif
//	}
//
//	std::wstring GetString(const std::wstring& name)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//			return it->second->valueString;
//		return L"";
//	}
//	std::string GetString(const std::string& name)
//	{
//		return ConvertStringToUTF8(GetString(ConvertStringToUTFWide(name)));
//	}
//
//	bool GetArray(const std::wstring& name, void** pData, int* pDataInBytes)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//		{
//			if (pData)
//				*pData = it->second->valueArray;
//			if (pDataInBytes)
//				*pDataInBytes = it->second->valueArraySize;
//			return true;
//		}
//		else
//		{
//			if (pData)
//				*pData = nullptr;
//			if (pDataInBytes)
//				*pDataInBytes = 0;
//			return false;
//		}
//	}
//	bool GetArray(const std::string& name, void** pData, int* pDataInBytes)
//	{
//		return GetArray(ConvertStringToUTFWide(name), pData, pDataInBytes);
//	}
//
//	int GetArraySize(const std::wstring& name)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//			return it->second->valueArraySize;
//		return 0;
//	}
//	int GetArraySize(const std::string& name)
//	{
//		return GetArraySize(ConvertStringToUTFWide(name));
//	}
//
//	void* GetArrayData(const std::wstring& name)
//	{
//		ItemMap::iterator it = items.find(name);
//		if (it != items.end())
//			return it->second->valueArray;
//		return nullptr;
//	}
//	void* GetArrayData(const std::string& name)
//	{
//		return GetArrayData(ConvertStringToUTFWide(name));
//	}
//
//	int GetInteger(const std::wstring& name)
//	{
//		return _wtoi(GetString(name).c_str());
//	}
//	int GetInteger(const std::string& name)
//	{
//		return GetInteger(ConvertStringToUTFWide(name));
//	}
//
//	void* GetPointer(const std::wstring& name)
//	{
//		return (void*)_wtoi64(GetString(name).c_str());
//	}
//	void* GetPointer(const std::string& name)
//	{
//		return GetPointer(ConvertStringToUTFWide(name));
//	}
//
//};
//
//#endif
