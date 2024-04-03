//!!!!betauser
/*
 * Copyright 2011-2023 Branimir Karadzic. All rights reserved.
 * License: https://github.com/bkaradzic/bgfx/blob/master/LICENSE
 */

#include <bx/platform.h>

#include "special.h"
#include <bgfx/embedded_shader.h>
#include <bx/file.h>
#include <bx/mutex.h>

#include "topology.h"

#if BX_PLATFORM_OSX
#	include <objc/message.h>
#endif // BX_PLATFORM_OSX

//!!!!
#if defined(WINAPI_FAMILY) && (WINAPI_FAMILY == WINAPI_FAMILY_APP)

void* call_special(const char* name, void* parameter1, void* parameter2, void* parameter3, void* parameter4)
{
	return NULL;
}

#else //defined(WINAPI_FAMILY) && (WINAPI_FAMILY == WINAPI_FAMILY_APP)

//#define USE_D3D11 1
//#if USE_D3D11

//#include <DXGI.h>
//#include <D3D11.h>
//#include "GFSDK_VXGI.h"

////#include "DeviceManager11.h"
//#include "GFSDK_NVRHI_D3D11.h"
//#define API_STRING "D3D11"
//NVRHI::RendererInterfaceD3D11* g_pRendererInterface = NULL;
//#define FLUSH_COMMAND_LIST 

//#elif USE_D3D12
//
//#include "DeviceManager12.h"
//#include "GFSDK_NVRHI_D3D12.h"
//#define API_STRING "D3D12"
//NVRHI::RendererInterfaceD3D12* g_pRendererInterface = NULL;
//#define FLUSH_COMMAND_LIST g_pRendererInterface->flushCommandList()
//
//#endif


void* call_special(const char* name, void* parameter1, void* parameter2, void* parameter3, void* parameter4)
{
	BX_UNUSED(name);
	BX_UNUSED(parameter1);
	BX_UNUSED(parameter2);
	BX_UNUSED(parameter3);
	BX_UNUSED(parameter4);

	//!!!!

	//if (bx::strCmp(name, "VXGI_Create") == 0)
	//{
	//	//!!!!

	//	VXGI::IGlobalIllumination* g_pGI = NULL;

	//	VXGI::GIParameters params;
	//	if (VXGI_FAILED(VFX_VXGI_CreateGIObject(params, &g_pGI)))
	//	{
	//		//MessageBoxA(g_DeviceManager->GetHWND(), "Failed to create a VXGI object.", "VXGI Sample", MB_ICONERROR);
	//		return (void*)1;
	//	}



	//	//VXGI::GIParameters params;
	//	//params.rendererInterface = g_pRendererInterface;
	//	//params.errorCallback = &g_ErrorCallback;

	//	//VXGI::ShaderCompilerParameters comparams;
	//	//comparams.errorCallback = &g_ErrorCallback;
	//	//comparams.graphicsAPI = g_pRendererInterface->getGraphicsAPI();
	//	//comparams.d3dCompilerDLLName = "d3dcompiler_47.dll";

	//	//if (VXGI_FAILED(VFX_VXGI_CreateShaderCompiler(comparams, &g_pGICompiler)))
	//	//{
	//	//	MessageBoxA(g_DeviceManager->GetHWND(), "Failed to create a VXGI shader compiler.", "VXGI Sample", MB_ICONERROR);
	//	//	return E_FAIL;
	//	//}

	//	//if (VXGI_FAILED(VFX_VXGI_CreateGIObject(params, &g_pGI)))
	//	//{
	//	//	MessageBoxA(g_DeviceManager->GetHWND(), "Failed to create a VXGI object.", "VXGI Sample", MB_ICONERROR);
	//	//	return E_FAIL;
	//	//}

	//	//if (VXGI_FAILED(g_pGI->createBasicTracer(&g_pGITracer, g_pGICompiler)))
	//	//{
	//	//	MessageBoxA(g_DeviceManager->GetHWND(), "Failed to create a VXGI tracer.", "VXGI Sample", MB_ICONERROR);
	//	//	return E_FAIL;
	//	//}

	//	//VXGI::VoxelizationParameters voxelizationParams;
	//	//voxelizationParams.ambientOcclusionMode = true;
	//	//voxelizationParams.mapSize = VXGI::uint3(g_nMapSize);

	//	//if (VXGI_FAILED(g_pGI->setVoxelizationParameters(voxelizationParams)))
	//	//{
	//	//	MessageBoxA(g_DeviceManager->GetHWND(), "Failed to initialize VXGI voxelization.", "VXGI Sample", MB_ICONERROR);
	//	//	return E_FAIL;
	//	//}

	//	return 0;
	//}

	//if (bx::strCmp(name, "VXGI_Delete") == 0)
	//{
	//	//!!!!
	//}

	return NULL;
}

#endif //defined(WINAPI_FAMILY) && (WINAPI_FAMILY == WINAPI_FAMILY_APP)
