// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "LibraryBase.h"
#include "shaderc.h"

using namespace bx;
using namespace bgfx;

enum ShaderType
{
	ShaderType_Vertex,
	ShaderType_Fragment,
	ShaderType_Compute,
};

enum ShaderModel
{
	ShaderModel_DX11_SM5,
	ShaderModel_DX12_SM6,
	ShaderModel_OpenGLES,
	ShaderModel_Vulkan,
};

struct Instance
{
	Options options;

	//ShaderType shaderType;
	//std::wstring shaderFile;
	std::wstring varyingFile;

	std::vector<UINT8> resultData;
	std::wstring error;
};

class File
{
public:
	File(const char* _filePath)
		: m_data(NULL)
	{
		bx::FileReader reader;
		if (bx::open(&reader, _filePath))
		{
			m_size = (uint32_t)bx::getSize(&reader);
			m_data = new char[m_size + 1];
			m_size = (uint32_t)bx::read(&reader, m_data, m_size);
			bx::close(&reader);

			if (m_data[0] == '\xef'
				&&  m_data[1] == '\xbb'
				&&  m_data[2] == '\xbf')
			{
				bx::memMove(m_data, &m_data[3], m_size - 3);
				m_size -= 3;
			}

			m_data[m_size] = '\0';
		}
	}

	~File()
	{
		delete[] m_data;
	}

	const char* getData() const
	{
		return m_data;
	}

	uint32_t getSize() const
	{
		return m_size;
	}

private:
	char* m_data;
	uint32_t m_size;
};


namespace bgfx
{
	bool compileShader(const char* _varying, const char* _comment, char* _shader, uint32_t _shaderLen, Options& _options, bx::FileWriter* _writer);
}

///////////////////////////////////////////////////////////////////////////////////////////////////

class MemoryFileWriter : public FileWriter //public FileWriterI
{
public:
	std::vector<UINT8>* vector;

	MemoryFileWriter(std::vector<UINT8>* vector)
	{
		this->vector = vector;
	}

	virtual ~MemoryFileWriter()
	{
	}

	virtual bool open(const FilePath& _filePath, bool _append, Error* _err) override
	{
		return true;
	}

	virtual void close() override
	{
	}

	virtual int64_t seek(int64_t _offset = 0, Whence::Enum _whence = Whence::Current) override
	{
		return 0;
	}

	virtual int32_t write(const void* _data, int32_t _size, Error* _err) override
	{
		vector->insert(vector->end(), (UINT8*)_data, (UINT8*)_data + _size);
		return _size;
	}

private:
	BX_ALIGN_DECL(16, uint8_t) m_internal[64];
};

///////////////////////////////////////////////////////////////////////////////////////////////////

//!!!!optimization option
EXPORT Instance* ShaderC_New(ShaderType shaderType, ShaderModel shaderModel, wchar16* shaderFile, wchar16* varyingFile)
{
	Instance* instance = new Instance();
	//instance->shaderType = shaderType;
	//instance->shaderFile = shaderFile;
	instance->varyingFile = TO_WCHAR_T(varyingFile);

	switch (shaderType)
	{
	case ShaderType_Vertex:instance->options.shaderType = 'v'; break;
	case ShaderType_Fragment:instance->options.shaderType = 'f'; break;
	case ShaderType_Compute:instance->options.shaderType = 'c'; break;
	default: ::Fatal("ShaderC_New: impl."); break;
	}

	//!!!!unicode
	instance->options.inputFilePath = ConvertStringToUTF8(TO_WCHAR_T(shaderFile));
	//instance->options.outputFilePath

#ifdef ANDROID
	instance->options.platform = "android";
#else
	instance->options.platform = "windows";
#endif

	switch(shaderModel)
	{
	case ShaderModel_DX11_SM5:
		switch (shaderType)
		{
		//case ShaderType_Vertex:instance->options.profile = "vs_5_1"; break;
		//case ShaderType_Fragment:instance->options.profile = "ps_5_1"; break;
		case ShaderType_Vertex:instance->options.profile = "vs_5_0"; break;
		case ShaderType_Fragment:instance->options.profile = "ps_5_0"; break;
		case ShaderType_Compute:instance->options.profile = "cs_5_0"; break;
		default: ::Fatal("ShaderC_New: impl."); break;
		}
		break;

	case ShaderModel_DX12_SM6:
		switch (shaderType)
		{
		case ShaderType_Vertex:instance->options.profile = "vs_6_0"; break;
		case ShaderType_Fragment:instance->options.profile = "ps_6_0"; break;
		case ShaderType_Compute:instance->options.profile = "cs_6_0"; break;
		default: ::Fatal("ShaderC_New: impl."); break;
		}
		break;

	case ShaderModel_OpenGLES:
		switch (shaderType)
		{
		case ShaderType_Vertex:instance->options.profile = "300"; break;
		case ShaderType_Fragment:instance->options.profile = "300"; break;
		case ShaderType_Compute:instance->options.profile = "300"; break;

		//case ShaderType_Vertex:instance->options.profile = "430"; break;
		//case ShaderType_Fragment:instance->options.profile = "430"; break;
		//case ShaderType_Compute:instance->options.profile = "430"; break;

		//case ShaderType_Vertex:instance->options.profile = "300 es"; break;
		//case ShaderType_Fragment:instance->options.profile = "300 es"; break;
		//case ShaderType_Compute:instance->options.profile = "300 es"; break;
		//case ShaderType_Vertex:instance->options.profile = "330"; break;
		//case ShaderType_Fragment:instance->options.profile = "330"; break;
		//case ShaderType_Compute:instance->options.profile = "330"; break;
		default: ::Fatal("ShaderC_New: impl."); break;
		}
		break;

	case ShaderModel_Vulkan:
		switch (shaderType)
		{
		//!!!!
		case ShaderType_Vertex:instance->options.profile = "310"; break;
		case ShaderType_Fragment:instance->options.profile = "310"; break;
		case ShaderType_Compute:instance->options.profile = "310"; break;

		//case ShaderType_Vertex:instance->options.profile = "spirv"; break;
		//case ShaderType_Fragment:instance->options.profile = "spirv"; break;
		//case ShaderType_Compute:instance->options.profile = "spirv"; break;

		//case ShaderType_Vertex:instance->options.profile = "vs_5_0"; break;
		//case ShaderType_Fragment:instance->options.profile = "ps_5_0"; break;
		//case ShaderType_Compute:instance->options.profile = "cs_5_0"; break;
		default: ::Fatal("ShaderC_New: impl."); break;
		}
		break;

	default:
		::Fatal("ShaderC_New: Shader model is not implemented.");
		break;
	}

	//!!!!
	instance->options.backwardsCompatibility = true;
	//instance->options.avoidFlowControl = false;
	//instance->options.noPreshader = false;
	//instance->options.partialPrecision = false;
	//instance->options.preferFlowControl = true;

	//std::vector<std::string> dependencies;

	//bool disasm;
	//bool raw;
	//bool preprocessOnly;
	//bool depends;

	//bool debugInformation;

	//bool avoidFlowControl;
	//bool noPreshader;
	//bool partialPrecision;
	//bool preferFlowControl;
	//bool backwardsCompatibility;
	//bool warningsAreErrors;

	//!!!!
	instance->options.optimize = true;// false;
	instance->options.optimizationLevel = 3;
	//instance->options.optimize = false;// false;
	//instance->options.optimizationLevel = 0;

	return instance;
}

EXPORT void ShaderC_Delete(Instance* instance)
{
	delete instance;
}

EXPORT void ShaderC_AddDefine(Instance* instance, wchar16* name, wchar16* value)
{
	std::string str = ConvertStringToUTF8(TO_WCHAR_T(name)) + "=" + ConvertStringToUTF8(TO_WCHAR_T(value));
	instance->options.defines.push_back(str);
}



static thread_local Instance* _currentInstance = NULL;

extern "C" int override_fprintf(FILE* _stream, const char* _format, ...)
{
	char _out[65536];
	va_list argList;
	va_start(argList, _format);
	int32_t len = bx::vsnprintf(_out, 65536, _format, argList);
	va_end(argList);

	_currentInstance->error += ConvertStringToUTFWide(_out);

	return len;
}

extern "C" int override_vfprintf(FILE* _stream, const char* _format, va_list _argList)
{
	char _out[65536];
	int32_t len = bx::vsnprintf(_out, 65536, _format, _argList);

	_currentInstance->error += ConvertStringToUTFWide(_out);

	return len;
}

EXPORT bool ShaderC_Compile(Instance* instance)
{
	_currentInstance = instance;


	auto inputFile = instance->options.inputFilePath;

	{
		bx::FilePath fp(inputFile.c_str());
		bx::StringView base(fp.getBaseName());
		if (base.getPtr() != inputFile)
		{
			bx::StringView path(fp.getPath());
			std::string dir;
			dir.assign(path.getPtr(), path.getTerm());
			instance->options.includeDirs.push_back(dir);
		}
	}

	//std::string commandLineComment = "// shaderc command line:\n//";
	//for (int32_t ii = 0, num = cmdLine.getNum(); ii < num; ++ii)
	//{
	//	commandLineComment += " ";
	//	commandLineComment += cmdLine.get(ii);
	//}
	//commandLineComment += "\n\n";

	//!!!!unicode
	bx::FileReader reader;
	if (!bx::open(&reader, instance->options.inputFilePath.c_str()))
	{
		//!!!!unicode
		instance->error = ConvertStringToUTFWide(string_format("Unable to open file '%s'.", instance->options.inputFilePath.c_str()));
		return false;
	}

	//!!!!unicode
	auto varyingdef = ConvertStringToUTF8(instance->varyingFile);
	File attribdef(varyingdef.c_str());
	const char* parse = attribdef.getData();
	if (NULL != parse
		&& *parse != '\0')
	{
		instance->options.dependencies.push_back(varyingdef.c_str());
	}
	else
	{
		FPRINTF(stderr, "ERROR: Failed to parse varying def file: \"%s\" No input/output semantics will be generated in the code!\n",varyingdef.c_str());
	}

	const size_t padding = 16384;
	uint32_t size = (uint32_t)bx::getSize(&reader);
	char* data = new char[size + padding + 1];
	size = (uint32_t)bx::read(&reader, data, size);

	if (data[0] == '\xef'
		&&  data[1] == '\xbb'
		&&  data[2] == '\xbf')
	{
		bx::memMove(data, &data[3], size - 3);
		size -= 3;
	}

	// Compiler generates "error X3000: syntax error: unexpected end of file"
	// if input doesn't have empty line at EOF.
	data[size] = '\n';
	bx::memSet(&data[size + 1], 0, padding);
	bx::close(&reader);

	MemoryFileWriter writer(&instance->resultData);

	//bx::FileWriter* writer = NULL;

	//if (NULL != bin2c)
	//{
	//	writer = new Bin2cWriter(bin2c);
	//}
	//else
	//{
	//	writer = new bx::FileWriter;
	//}

	//if (!bx::open(writer, outFilePath))
	//{
	//	fprintf(stderr, "Unable to open output file '%s'.", outFilePath);
	//	return bx::kExitFailure;
	//}

	//std::stringstream oss;
	//std::streambuf* old = std::cerr.rdbuf(oss.rdbuf());

	//bool compiled = false;
	bool compiled = compileShader(attribdef.getData(), "", data, size, instance->options, &writer);

	//std::cerr.rdbuf(old);

	//bx::close(writer);
	//delete writer;

	if (!compiled)
	{
		//!!!!
		if (instance->error.length() == 0)
			instance->error = L"No error text.";
		//instance->error = std::wstring(L"Failed to build shader. ") + ConvertStringToUTFWide(oss.str());
	}

	_currentInstance = nullptr;

	return compiled;
}

EXPORT wchar16* ShaderC_GetError(Instance* instance)
{
	return CreateOutString(instance->error);
}

EXPORT void ShaderC_GetResult(Instance* instance, void** data, int* size)
{
	*data = &instance->resultData[0];
	*size = (int)instance->resultData.size();
}
