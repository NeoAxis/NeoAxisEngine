// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
#include "NeoAxisCoreNative.h"
#include "UtilsNativeWrapper.h"
#include "MaskedOcclusionCulling.h"

EXPORT MaskedOcclusionCulling* MaskedOcclusionCulling_Create()
{
	MaskedOcclusionCulling* instance = MaskedOcclusionCulling::Create(MaskedOcclusionCulling::SSE41);
	return instance;
}

EXPORT void MaskedOcclusionCulling_Destroy(MaskedOcclusionCulling* instance)
{
	MaskedOcclusionCulling::Destroy(instance);
}

EXPORT void MaskedOcclusionCulling_Init(MaskedOcclusionCulling* instance, int width, int height, bool ortho)
{
	instance->SetResolution(width, height);
	instance->SetOrtho(ortho);
	instance->SetNearClipPlane(0);
}

EXPORT void MaskedOcclusionCulling_ClearBuffer(MaskedOcclusionCulling* instance)
{
	instance->ClearBuffer();
}

EXPORT MaskedOcclusionCulling::CullingResult MaskedOcclusionCulling_RenderTriangles(MaskedOcclusionCulling* instance, float* inVtx, const unsigned int* inTris, int nTris, float* modelToClipMatrix)//, bool stride16)
{
	//if(stride16)
	//	return instance->RenderTriangles(inVtx, inTris, nTris, modelToClipMatrix, MaskedOcclusionCulling::BACKFACE_CW, MaskedOcclusionCulling::CLIP_PLANE_ALL, MaskedOcclusionCulling::VertexLayout(16, 4, 8));
	//else
	return instance->RenderTriangles(inVtx, inTris, nTris, modelToClipMatrix, MaskedOcclusionCulling::BACKFACE_CW, MaskedOcclusionCulling::CLIP_PLANE_ALL, MaskedOcclusionCulling::VertexLayout(12, 4, 8));
}

EXPORT MaskedOcclusionCulling::CullingResult MaskedOcclusionCulling_TestTriangles(MaskedOcclusionCulling* instance, float* inVtx, const unsigned int* inTris, int nTris, float* modelToClipMatrix)
{
	return instance->TestTriangles(inVtx, inTris, nTris, modelToClipMatrix, MaskedOcclusionCulling::BACKFACE_CW, MaskedOcclusionCulling::CLIP_PLANE_ALL, MaskedOcclusionCulling::VertexLayout(12, 4, 8));
}

EXPORT MaskedOcclusionCulling::CullingResult MaskedOcclusionCulling_TestRect(MaskedOcclusionCulling* instance, float xmin, float ymin, float xmax, float ymax, float wmin)
{
	return instance->TestRect(xmin, ymin, xmax, ymax, wmin);
}

EXPORT void MaskedOcclusionCulling_ComputePixelDepthBuffer(MaskedOcclusionCulling* instance, float* depthData)
{
	instance->ComputePixelDepthBuffer(depthData, false);
}
#endif