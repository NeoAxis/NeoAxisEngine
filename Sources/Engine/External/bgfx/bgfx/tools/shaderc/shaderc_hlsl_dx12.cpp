// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
/*
 * Copyright 2011-2023 Branimir Karadzic. All rights reserved.
 * License: https://github.com/bkaradzic/bgfx/blob/master/LICENSE
 */

#include "shaderc.h"

#include "../../src/bgfx_p.h"

#if SHADERC_CONFIG_HLSL

#include "../../src/renderer_d3d12.h"

#if defined(__MINGW32__)
#	define __REQUIRED_RPCNDR_H_VERSION__ 475
#	define __in
#	define __out
#endif // defined(__MINGW32__)

#include <windows.h>

#include <dxcapi.h>
#include <d3d12shader.h>
#include <bx/os.h>

#include <wrl/client.h>
using namespace Microsoft::WRL;
#include <atlcomcli.h>

#include "LibraryBase.h"

namespace bgfx { namespace hlsl_dx12
{
	struct D3DCompiler
	{
		const char* fileName;
	};

	static const D3DCompiler s_d3dcompiler[] =
	{
#if BX_PLATFORM_WINRT
		{ "NeoAxis.Internal\\Platforms\\UWP\\dxcompiler.dll" },
#else
		{ "dxcompiler.dll" },
#endif
	};

	static const D3DCompiler* s_compiler;
	static DxcCreateInstanceProc CreateInstanceProc;
	//static void* s_d3dcompilerdll;
	static ComPtr<IDxcUtils> pUtils;

	const D3DCompiler* load(bx::WriterI* _messageWriter)
	{
		bx::Error messageErr;

		for (uint32_t ii = 0; ii < BX_COUNTOF(s_d3dcompiler); ++ii)
		{
			const D3DCompiler* compiler = &s_d3dcompiler[ii];
			void* s_d3dcompilerdll = bx::dlopen(compiler->fileName); //s_d3dcompilerdll = bx::dlopen(compiler->fileName);
			if (NULL == s_d3dcompilerdll)
			{
				continue;
			}

			CreateInstanceProc = (DxcCreateInstanceProc)bx::dlsym(s_d3dcompilerdll, "DxcCreateInstance");

			if(CreateInstanceProc == NULL)
			{
				bx::dlclose(s_d3dcompilerdll);
				continue;
			}

			if (g_verbose)
			{
				char filePath[bx::kMaxFilePath];
				GetModuleFileNameA( (HMODULE)s_d3dcompilerdll, filePath, sizeof(filePath) );
				BX_TRACE("Loaded %s compiler (%s).", compiler->fileName, filePath);
			}

			return compiler;
		}

		bx::write(_messageWriter, &messageErr, "Error: Unable to open dxcompiler.dll shader compiler.\n");
		return NULL;
	}

	//void unload()
	//{
		//bx::dlclose(s_d3dcompilerdll);
	//}

	struct CTHeader
	{
		uint32_t Size;
		uint32_t Creator;
		uint32_t Version;
		uint32_t Constants;
		uint32_t ConstantInfo;
		uint32_t Flags;
		uint32_t Target;
	};

	struct CTInfo
	{
		uint32_t Name;
		uint16_t RegisterSet;
		uint16_t RegisterIndex;
		uint16_t RegisterCount;
		uint16_t Reserved;
		uint32_t TypeInfo;
		uint32_t DefaultValue;
	};

	struct CTType
	{
		uint16_t Class;
		uint16_t Type;
		uint16_t Rows;
		uint16_t Columns;
		uint16_t Elements;
		uint16_t StructMembers;
		uint32_t StructMemberInfo;
	};

	struct RemapInputSemantic
	{
		bgfx::Attrib::Enum m_attr;
		const char* m_name;
		uint8_t m_index;
	};

	static const RemapInputSemantic s_remapInputSemantic[bgfx::Attrib::Count + 1] =
	{
		{ bgfx::Attrib::Position,  "POSITION",     0 },
		{ bgfx::Attrib::Normal,    "NORMAL",       0 },
		{ bgfx::Attrib::Tangent,   "TANGENT",      0 },
		{ bgfx::Attrib::Bitangent, "BITANGENT",    0 },
		{ bgfx::Attrib::Color0,    "COLOR",        0 },
		{ bgfx::Attrib::Color1,    "COLOR",        1 },
		{ bgfx::Attrib::Color2,    "COLOR",        2 },
		{ bgfx::Attrib::Color3,    "COLOR",        3 },
		{ bgfx::Attrib::Indices,   "BLENDINDICES", 0 },
		{ bgfx::Attrib::Weight,    "BLENDWEIGHT",  0 },
		{ bgfx::Attrib::TexCoord0, "TEXCOORD",     0 },
		{ bgfx::Attrib::TexCoord1, "TEXCOORD",     1 },
		{ bgfx::Attrib::TexCoord2, "TEXCOORD",     2 },
		{ bgfx::Attrib::TexCoord3, "TEXCOORD",     3 },
		{ bgfx::Attrib::TexCoord4, "TEXCOORD",     4 },
		{ bgfx::Attrib::TexCoord5, "TEXCOORD",     5 },
		{ bgfx::Attrib::TexCoord6, "TEXCOORD",     6 },
		{ bgfx::Attrib::TexCoord7, "TEXCOORD",     7 },
		{ bgfx::Attrib::Count,     "",             0 },
	};

	const RemapInputSemantic& findInputSemantic(const char* _name, uint8_t _index)
	{
		for (uint32_t ii = 0; ii < bgfx::Attrib::Count; ++ii)
		{
			const RemapInputSemantic& ris = s_remapInputSemantic[ii];
			if (0 == bx::strCmp(ris.m_name, _name)
			&&  ris.m_index == _index)
			{
				return ris;
			}
		}

		return s_remapInputSemantic[bgfx::Attrib::Count];
	}

	struct UniformRemap
	{
		UniformType::Enum id;
		D3D_SHADER_VARIABLE_CLASS paramClass;
		D3D_SHADER_VARIABLE_TYPE paramType;
		uint8_t columns;
		uint8_t rows;
	};

	static const UniformRemap s_uniformRemap[] =
	{
		{ UniformType::Sampler, D3D_SVC_SCALAR,         D3D_SVT_INT,         0, 0 },
		{ UniformType::Vec4, D3D_SVC_VECTOR,         D3D_SVT_FLOAT,       0, 0 },
		{ UniformType::Mat3, D3D_SVC_MATRIX_COLUMNS, D3D_SVT_FLOAT,       3, 3 },
		{ UniformType::Mat4, D3D_SVC_MATRIX_COLUMNS, D3D_SVT_FLOAT,       4, 4 },
		{ UniformType::Sampler, D3D_SVC_OBJECT,         D3D_SVT_SAMPLER,     0, 0 },
		{ UniformType::Sampler, D3D_SVC_OBJECT,         D3D_SVT_SAMPLER2D,   0, 0 },
		{ UniformType::Sampler, D3D_SVC_OBJECT,         D3D_SVT_SAMPLER3D,   0, 0 },
		{ UniformType::Sampler, D3D_SVC_OBJECT,         D3D_SVT_SAMPLERCUBE, 0, 0 },
	};

	UniformType::Enum findUniformType(const D3D12_SHADER_TYPE_DESC& constDesc)
	{
		for (uint32_t ii = 0; ii < BX_COUNTOF(s_uniformRemap); ++ii)
		{
			const UniformRemap& remap = s_uniformRemap[ii];

			if (remap.paramClass == constDesc.Class
			&&  remap.paramType == constDesc.Type)
			{
				if (D3D_SVC_MATRIX_COLUMNS != constDesc.Class)
				{
					return remap.id;
				}

				if (remap.columns == constDesc.Columns
				&&  remap.rows == constDesc.Rows)
				{
					return remap.id;
				}
			}
		}

		return UniformType::Count;
	}

	static wchar_t* s_optimizationLevelD3D12[4] =
	{
		DXC_ARG_OPTIMIZATION_LEVEL0,
		DXC_ARG_OPTIMIZATION_LEVEL1,
		DXC_ARG_OPTIMIZATION_LEVEL2,
		DXC_ARG_OPTIMIZATION_LEVEL3,
	};

	typedef std::vector<std::string> UniformNameList;

	static bool isSampler(D3D_SHADER_VARIABLE_TYPE _svt)
	{
		switch (_svt)
		{
		case D3D_SVT_SAMPLER:
		case D3D_SVT_SAMPLER1D:
		case D3D_SVT_SAMPLER2D:
		case D3D_SVT_SAMPLER3D:
		case D3D_SVT_SAMPLERCUBE:
			return true;

		default:
			break;
		}

		return false;
	}

	bool getReflectionDataD3D12(ComPtr<IDxcResult>& pCompileResult, bool _vshader, UniformArray& _uniforms, uint8_t& _numAttrs, uint16_t* _attrs, uint16_t& _size, UniformNameList& unusedUniforms, bx::WriterI* _messageWriter)
	{
		bx::Error messageErr;

		ComPtr<IDxcBlob> pReflectionData;
		HRESULT hr = pCompileResult->GetOutput(DXC_OUT_REFLECTION, IID_PPV_ARGS(pReflectionData.GetAddressOf()), nullptr);
		if (FAILED(hr))
		{
			bx::write(_messageWriter, &messageErr, "Error: DXC reflect failed 0x%08x\n", (uint32_t)hr);
			return false;
		}

		DxcBuffer reflectionBuffer;
		reflectionBuffer.Ptr = pReflectionData->GetBufferPointer();
		reflectionBuffer.Size = pReflectionData->GetBufferSize();
		reflectionBuffer.Encoding = 0;

		ComPtr<ID3D12ShaderReflection> pShaderReflection;
		pUtils->CreateReflection(&reflectionBuffer, IID_PPV_ARGS(pShaderReflection.GetAddressOf()));

		ID3D12ShaderReflection* reflect = pShaderReflection.Get();

		D3D12_SHADER_DESC desc;
		hr = reflect->GetDesc(&desc);
		if (FAILED(hr) )
		{
			bx::write(_messageWriter, &messageErr, "Error: ID3D12ShaderReflection::GetDesc failed 0x%08x\n", (uint32_t)hr);
			return false;
		}

		BX_TRACE("Creator: %s 0x%08x", desc.Creator, desc.Version);
		BX_TRACE("Num constant buffers: %d", desc.ConstantBuffers);

		BX_TRACE("Input:");

		if (_vshader) // Only care about input semantic on vertex shaders
		{
			for (uint32_t ii = 0; ii < desc.InputParameters; ++ii)
			{
				D3D12_SIGNATURE_PARAMETER_DESC spd;
				reflect->GetInputParameterDesc(ii, &spd);
				BX_TRACE("\t%2d: %s%d, vt %d, ct %d, mask %x, reg %d"
					, ii
					, spd.SemanticName
					, spd.SemanticIndex
					, spd.SystemValueType
					, spd.ComponentType
					, spd.Mask
					, spd.Register
					);

				const RemapInputSemantic& ris = findInputSemantic(spd.SemanticName, uint8_t(spd.SemanticIndex) );
				if (ris.m_attr != bgfx::Attrib::Count)
				{
					_attrs[_numAttrs] = bgfx::attribToId(ris.m_attr);
					++_numAttrs;
				}
			}
		}

		BX_TRACE("Output:");
		for (uint32_t ii = 0; ii < desc.OutputParameters; ++ii)
		{
			D3D12_SIGNATURE_PARAMETER_DESC spd;
			reflect->GetOutputParameterDesc(ii, &spd);
			BX_TRACE("\t%2d: %s%d, %d, %d", ii, spd.SemanticName, spd.SemanticIndex, spd.SystemValueType, spd.ComponentType);
		}

		for (uint32_t ii = 0, num = bx::uint32_min(1, desc.ConstantBuffers); ii < num; ++ii)
		{
			ID3D12ShaderReflectionConstantBuffer* cbuffer = reflect->GetConstantBufferByIndex(ii);
			D3D12_SHADER_BUFFER_DESC bufferDesc;
			hr = cbuffer->GetDesc(&bufferDesc);

			_size = (uint16_t)bufferDesc.Size;

			if (SUCCEEDED(hr) )
			{
				BX_TRACE("%s, %d, vars %d, size %d"
					, bufferDesc.Name
					, bufferDesc.Type
					, bufferDesc.Variables
					, bufferDesc.Size
					);

				for (uint32_t jj = 0; jj < bufferDesc.Variables; ++jj)
				{
					ID3D12ShaderReflectionVariable* var = cbuffer->GetVariableByIndex(jj);
					ID3D12ShaderReflectionType* type = var->GetType();
					D3D12_SHADER_VARIABLE_DESC varDesc;
					hr = var->GetDesc(&varDesc);
					if (SUCCEEDED(hr) )
					{
						D3D12_SHADER_TYPE_DESC constDesc;
						hr = type->GetDesc(&constDesc);
						if (SUCCEEDED(hr) )
						{
							UniformType::Enum uniformType = findUniformType(constDesc);

							if (UniformType::Count != uniformType
							&&  0 != (varDesc.uFlags & D3D_SVF_USED) )
							{
								Uniform un;
								un.name = varDesc.Name;
								un.type = uniformType;
								un.num = uint8_t(constDesc.Elements);
								un.regIndex = uint16_t(varDesc.StartOffset);
								un.regCount = uint16_t(bx::alignUp(varDesc.Size, 16) / 16);
								_uniforms.push_back(un);

								BX_TRACE("\t%s, %d, size %d, flags 0x%08x, %d (used)"
									, varDesc.Name
									, varDesc.StartOffset
									, varDesc.Size
									, varDesc.uFlags
									, uniformType
									);
							}
							else
							{
								if (0 == (varDesc.uFlags & D3D_SVF_USED) )
								{
									unusedUniforms.push_back(varDesc.Name);
								}

								BX_TRACE("\t%s, unknown type", varDesc.Name);
							}
						}
					}
				}
			}
		}

		BX_TRACE("Bound:");
		for (uint32_t ii = 0; ii < desc.BoundResources; ++ii)
		{
			D3D12_SHADER_INPUT_BIND_DESC bindDesc;

			hr = reflect->GetResourceBindingDesc(ii, &bindDesc);
			if (SUCCEEDED(hr) )
			{
				if (D3D_SIT_SAMPLER == bindDesc.Type || D3D_SIT_TEXTURE == bindDesc.Type)
				{
					BX_TRACE("\t%s, %d, %d, %d"
						, bindDesc.Name
						, bindDesc.Type
						, bindDesc.BindPoint
						, bindDesc.BindCount
						);

					bx::StringView end = bx::strFind(bindDesc.Name, "Sampler");
					if (end.isEmpty())
						end = bx::strFind(bindDesc.Name, "Texture");

					if (!end.isEmpty() )
					{
						Uniform un;
						un.name.assign(bindDesc.Name, (end.getPtr() - bindDesc.Name) );
						un.type = UniformType::Enum(kUniformSamplerBit | UniformType::Sampler);
						un.num = 1;
						un.regIndex = uint16_t(bindDesc.BindPoint);
						un.regCount = uint16_t(bindDesc.BindCount);
						_uniforms.push_back(un);
					}
				}
				else
				{
					BX_TRACE("\t%s, unknown bind data", bindDesc.Name);
				}
			}
		}

		//if (NULL != reflect)
		//{
		//	reflect->Release();
		//}

		return true;
	}

	static bool compile(const Options& _options, uint32_t _version, const std::string& _code, bx::WriterI* _shaderWriter, bx::WriterI* _messageWriter, bool _firstPass)
	{
		bx::Error messageErr;

		const char* profile = _options.profile.c_str();

		if (profile[0] == '\0')
		{
			bx::write(_messageWriter, &messageErr, "Error: Shader profile must be specified.\n");
			return false;
		}

		//initialize compiler
		if (s_compiler == NULL)
		{
			s_compiler = load(_messageWriter);

			ComPtr<IDxcUtils> pUtils2;
			HRESULT hr = CreateInstanceProc(CLSID_DxcUtils, IID_PPV_ARGS(pUtils2.GetAddressOf()));
			if (FAILED(hr))
			{
				bx::write(_messageWriter, &messageErr, "Error: Failed to instantiate DXC utils 0x%08x\n", (uint32_t)hr);
				return false;
			}
			pUtils = pUtils2;
		}

		std::vector<LPWSTR> arguments;

		arguments.push_back(L"-E");
		arguments.push_back(L"main");

		std::wstring profileAndType;
		profileAndType.append(1, (_options.shaderType == 'f') ? 'p' : _options.shaderType);
		profileAndType.append(ConvertStringToUTFWide(profile + 1));

		// -T for the target profile (eg. 'ps_6_6')
		arguments.push_back(L"-T");
		arguments.push_back(profileAndType.data());

		//!!!!need debug in release?
		// Strip reflection data and pdbs (see later)
		arguments.push_back(L"-Qstrip_debug");
		arguments.push_back(L"-Qstrip_reflect");

		bool debug = _options.debugInformation;
		bool werror = _options.warningsAreErrors;

		if (debug)
			arguments.push_back(DXC_ARG_DEBUG);
		if (werror)
			arguments.push_back(DXC_ARG_WARNINGS_ARE_ERRORS);
		//if(_options.backwardsCompatibility )
		//	arguments.push_back(DXC_ARG_ENABLE_BACKWARDS_COMPATIBILITY);
		if (_options.avoidFlowControl)
			arguments.push_back(DXC_ARG_AVOID_FLOW_CONTROL);
		if (_options.preferFlowControl)
			arguments.push_back(DXC_ARG_PREFER_FLOW_CONTROL);

		if (_options.optimize)
		{
			uint32_t optimization = bx::uint32_min(_options.optimizationLevel, BX_COUNTOF(s_optimizationLevelD3D12) - 1);
			arguments.push_back(s_optimizationLevelD3D12[optimization]);
		}
		else
			arguments.push_back(DXC_ARG_SKIP_OPTIMIZATIONS);


		//flags |= _options.noPreshader ? D3DCOMPILE_NO_PRESHADER : 0;
		//flags |= _options.partialPrecision ? D3DCOMPILE_PARTIAL_PRECISION : 0;


		//!!!!выключить релизу? или для deploy
		//#define DXC_ARG_SKIP_VALIDATION L"-Vd"

		//#define DXC_ARG_PACK_MATRIX_ROW_MAJOR L"-Zpr"
		//#define DXC_ARG_PACK_MATRIX_COLUMN_MAJOR L"-Zpc"
		//#define DXC_ARG_ENABLE_STRICTNESS L"-Ges"
		//#define DXC_ARG_IEEE_STRICTNESS L"-Gis"
		//#define DXC_ARG_RESOURCES_MAY_ALIAS L"-res_may_alias"
		//#define DXC_ARG_ALL_RESOURCES_BOUND L"-all_resources_bound"
		//#define DXC_ARG_DEBUG_NAME_FOR_SOURCE L"-Zss"
		//#define DXC_ARG_DEBUG_NAME_FOR_BINARY L"-Zsb"


		CComPtr<IDxcCompiler3> compiler;

		HRESULT hr;

		hr = CreateInstanceProc(CLSID_DxcCompiler, __uuidof(IDxcCompiler3), reinterpret_cast<LPVOID*>(&compiler));
		if (FAILED(hr))
		{
			bx::write(_messageWriter, &messageErr, "Error: Failed to instantiate compiler.\n");
			return false;
		}

		//hr = library->CreateIncludeHandler(&includeHandler);
		//if (FAILED(hr))
		//{
		//	PRINT("Failed to create include handler.");
		//	return hr;
		//}


		// Output preprocessed shader so that HLSL can be debugged via GPA
		// or PIX. Compiling through memory won't embed preprocessed shader
		// file path.
		std::string hlslfp;

		if (debug)
		{
			hlslfp = _options.outputFilePath + ".hlsl";
			writeFile(hlslfp.c_str(), _code.c_str(), (int32_t)_code.size());
		}


		bx::ErrorAssert err;


		DxcBuffer sourceBuffer;
		sourceBuffer.Ptr = _code.c_str();
		sourceBuffer.Size = _code.size();
		sourceBuffer.Encoding = 0;

		ComPtr<IDxcResult> pCompileResult;
		hr = compiler->Compile(
			&sourceBuffer,
			(LPCWSTR*)arguments.data(),
			(uint32_t)arguments.size(),
			NULL,
			IID_PPV_ARGS(pCompileResult.GetAddressOf()));

		pCompileResult->GetStatus(&hr);
		if (FAILED(hr)) // || (werror && NULL != errorMsg))
		{
			//error handling. note that this will also include warnings unless disabled.
			ComPtr<IDxcBlobUtf8> pErrors;
			pCompileResult->GetOutput(DXC_OUT_ERRORS, IID_PPV_ARGS(pErrors.GetAddressOf()), nullptr);
			if (pErrors && pErrors->GetStringLength() > 0)
			{
				CComPtr<IDxcBlobEncoding> pErrors2;
				hr = pCompileResult->GetErrorBuffer(&pErrors2);
				if (FAILED(hr))
				{
					bx::write(_messageWriter, &messageErr, "Error: Failed to retrieve compiler error buffer.\n");
					return false;
				}

				const char* log = (char*)pErrors2->GetBufferPointer();

				int32_t line = 0;
				int32_t column = 0;
				int32_t start = 0;
				int32_t end = INT32_MAX;

				if (!hlslfp.empty())
				{
					bx::StringView logfp = bx::strFind(log, hlslfp.c_str());
					if (!logfp.isEmpty())
					{
						log = logfp.getPtr() + hlslfp.length();
					}
				}

				bool found = false
					|| 2 == sscanf(log, "(%u,%u", &line, &column)
					|| 2 == sscanf(log, " :%u:%u: ", &line, &column)
					;

				if (found
					&& 0 != line)
				{
					start = bx::uint32_imax(1, line - 10);
					end = start + 20;
				}

				printCode(_code.c_str(), line, start, end, column);
				bx::write(_messageWriter, &messageErr, "Error: DXC compile failed 0x%08x %s\n", (uint32_t)hr, log);
				//errorMsg->Release();
				return false;
			}
			else
			{
				bx::write(_messageWriter, &messageErr, "Error: DXC compile failed. No error message.\n");
				return false;
			}
		}


		//compiled

		bool result = false;

		UniformArray uniforms;
		uint8_t numAttrs = 0;
		uint16_t attrs[bgfx::Attrib::Count];
		uint16_t size = 0;

		{
			UniformNameList unusedUniforms;
			if (!getReflectionDataD3D12(pCompileResult, profileAndType[0] == 'v', uniforms, numAttrs, attrs, size, unusedUniforms, _messageWriter))
			{
				bx::write(_messageWriter, &messageErr, "Error: Unable to get D3D12 reflection data.\n");
				goto error;
			}

			if (_firstPass && unusedUniforms.size() > 0)
			{
				// first time through, we just find unused uniforms and get rid of them
				std::string output;
				bx::LineReader reader(_code.c_str() );
				while (!reader.isDone() )
				{
					bx::StringView strLine = reader.next();
					bool found = false;

					for (UniformNameList::iterator it = unusedUniforms.begin(), itEnd = unusedUniforms.end(); it != itEnd; ++it)
					{
						bx::StringView str = strFind(strLine, "uniform ");
						if (str.isEmpty() )
						{
							continue;
						}

						// matching lines like:  uniform u_name;
						// we want to replace "uniform" with "static" so that it's no longer
						// included in the uniform blob that the application must upload
						// we can't just remove them, because unused functions might still reference
						// them and cause a compile error when they're gone
						if (!bx::findIdentifierMatch(strLine, it->c_str() ).isEmpty() )
						{
							output.append(strLine.getPtr(), str.getPtr() );
							output += "static ";
							output.append(str.getTerm(), strLine.getTerm() );
							output += "\n";
							found = true;

							unusedUniforms.erase(it);
							break;
						}
					}

					if (!found)
					{
						output.append(strLine.getPtr(), strLine.getTerm() );
						output += "\n";
					}
				}

				// recompile with the unused uniforms converted to statics
				return compile(_options, _version, output.c_str(), _shaderWriter, _messageWriter, false);
			}
		}

		{
			uint16_t count = (uint16_t)uniforms.size();
			bx::write(_shaderWriter, count, &err);

			uint32_t fragmentBit = profileAndType[0] == 'p' ? kUniformFragmentBit : 0;
			for (UniformArray::const_iterator it = uniforms.begin(); it != uniforms.end(); ++it)
			{
				const Uniform& un = *it;
				uint8_t nameSize = (uint8_t)un.name.size();
				bx::write(_shaderWriter, nameSize, &err);
				bx::write(_shaderWriter, un.name.c_str(), nameSize, &err);
				uint8_t type = uint8_t(un.type | fragmentBit);
				bx::write(_shaderWriter, type, &err);
				bx::write(_shaderWriter, un.num, &err);
				bx::write(_shaderWriter, un.regIndex, &err);
				bx::write(_shaderWriter, un.regCount, &err);
				bx::write(_shaderWriter, un.texComponent, &err);
				bx::write(_shaderWriter, un.texDimension, &err);
				bx::write(_shaderWriter, un.texFormat, &err);

				BX_TRACE("%s, %s, %d, %d, %d"
					, un.name.c_str()
					, getUniformTypeName(UniformType::Enum(un.type & ~kUniformMask))
					, un.num
					, un.regIndex
					, un.regCount
					);
			}
		}


		IDxcBlob* code = nullptr;

		{
			hr = pCompileResult->GetResult(&code);
			if (FAILED(hr))
			{
				bx::write(_messageWriter, &messageErr, "Error: Failed to retrieve compiled code.\n");
				return false;
			}

			//RefCountPtr<IDxcBlob> pHash;
			//if (SUCCEEDED(pCompileResult->GetOutput(DXC_OUT_SHADER_HASH, IID_PPV_ARGS(pHash.GetAddressOf()), nullptr)))
			//{
			//	DxcShaderHash* pHashBuf = (DxcShaderHash*)pHash->GetBufferPointer();
			//}

		}


		//!!!!do stripping?


		//{
		//	ID3DBlob* stripped;
		//	hr = D3DStripShader(code->GetBufferPointer()
		//		, code->GetBufferSize()
		//		, D3DCOMPILER_STRIP_REFLECTION_DATA
		//		| D3DCOMPILER_STRIP_TEST_BLOBS
		//		, &stripped
		//		);

		//	if (SUCCEEDED(hr) )
		//	{
		//		code->Release();
		//		code = stripped;
		//	}
		//}

		{
			uint32_t shaderSize = uint32_t(code->GetBufferSize() );
			bx::write(_shaderWriter, shaderSize, &err);
			bx::write(_shaderWriter, code->GetBufferPointer(), shaderSize, &err);
			uint8_t nul = 0;
			bx::write(_shaderWriter, nul, &err);
		}

		{
			bx::write(_shaderWriter, numAttrs, &err);
			bx::write(_shaderWriter, attrs, numAttrs*sizeof(uint16_t), &err);

			bx::write(_shaderWriter, size, &err);
		}

		if (_options.disasm )
		{
			bx::write(_messageWriter, &messageErr, "Error: Disasm is not supported.\n");

			//ID3DBlob* disasm;
			//D3DDisassemble(code->GetBufferPointer()
			//	, code->GetBufferSize()
			//	, 0
			//	, NULL
			//	, &disasm
			//	);

			//if (NULL != disasm)
			//{
			//	std::string disasmfp = _options.outputFilePath + ".disasm";

			//	writeFile(disasmfp.c_str(), disasm->GetBufferPointer(), (uint32_t)disasm->GetBufferSize() );
			//	disasm->Release();
			//}
		}

		code->Release();

		//if (NULL != errorMsg)
		//{
		//	errorMsg->Release();
		//}

		result = true;

	error:
		//code->Release();
		//unload();
		return result;

	}

} // namespace hlsl

	bool compileHLSLShaderDX12(const Options& _options, uint32_t _version, const std::string& _code, bx::WriterI* _shaderWriter, bx::WriterI* _messageWriter)
	{
		return hlsl_dx12::compile(_options, _version, _code, _shaderWriter, _messageWriter, true);
	}

} // namespace bgfx

#else

namespace bgfx
{
	bool compileHLSLShaderDX12(const Options& _options, uint32_t _version, const std::string& _code, bx::WriterI* _shaderWriter, bx::WriterI* _messageWriter)
	{
		BX_UNUSED(_options, _version, _code, _shaderWriter);
		bx::Error messageErr;
		bx::write(_messageWriter, &messageErr, "HLSL compiler is not supported on this platform.\n");
		return false;
	}

} // namespace bgfx

#endif // SHADERC_CONFIG_HLSL
