/*
-----------------------------------------------------------------------------
This source file is part of OGRE
(Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org

Copyright (c) 2000-2009 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#include "OgreStableHeaders.h"

#include "OgreSceneManager.h"

//#include "OgreCamera.h"
#include "OgreRenderSystem.h"
//#include "OgreMeshManager.h"
//#include "OgreMesh.h"
//#include "OgreSubMesh.h"
//#include "OgreLight.h"
#include "OgreMath.h"
//#include "OgreControllerManager.h"
//#include "OgreMaterialManager.h"
//#include "OgreAnimation.h"
//#include "OgreAnimationTrack.h"
#include "OgreStringConverter.h"
//#include "OgreTechnique.h"
//#include "OgreTextureUnitState.h"
#include "OgreException.h"
#include "OgreLogManager.h"
#include "OgreRoot.h"
#include "OgreDataStream.h"
#include "OgreNativeWrapperGeneral.h"
//#include "DebugGeometryRenderable.h"

#include <cstdio>

namespace Ogre {

//-----------------------------------------------------------------------
//uint32 SceneManager::ENTITY_TYPE_MASK			= 0x40000000;
//uint32 SceneManager::FX_TYPE_MASK				= 0x20000000;
//uint32 SceneManager::LIGHT_TYPE_MASK			= 0x08000000;
//uint32 SceneManager::FRUSTUM_TYPE_MASK			= 0x04000000;
//uint32 SceneManager::USER_TYPE_MASK_LIMIT         = SceneManager::FRUSTUM_TYPE_MASK;
//-----------------------------------------------------------------------
SceneManager::SceneManager(const String& name) :
mName(name)
{
	if(root->sceneManager)
		OGRE_EXCEPT(Exception::ERR_INTERNAL_ERROR,  "root->sceneManager != NULL", "");
	root->sceneManager = this;

    // Root scene node
    //mSceneRoot = new SceneNode(this, "root node");
	//mSceneRoot = new SceneNode(root, "root node");

	mDestRenderSystem = root->renderSystem;
    //Root *root = Root::getSingletonPtr();
    //if (root)
    //    _setDestinationRenderSystem(root->getRenderSystem());

	// init shadow texture config
	//betauser
	//setShadowTextureCount(1);
	//setShadowTextureCount(0, 0);
	//setShadowTextureSize(512, 256);

	//betauser
	//// init shadow texture count per type.
	//mShadowTextureCountPerType[Light::LT_POINT] = 1;
	//mShadowTextureCountPerType[Light::LT_DIRECTIONAL] = 1;
	//mShadowTextureCountPerType[Light::LT_SPOTLIGHT] = 1;

	//// create the auto param data source instance
	//mAutoParamDataSource = createAutoParamDataSource();

	////betauser
	//setNormaliseNormalsOnScale(false);

	////betauser
	//shadowTexturePixelFormat[Light::LT_DIRECTIONAL] = PF_X8R8G8B8;
	//shadowTexturePixelFormat[Light::LT_SPOTLIGHT] = PF_X8R8G8B8;
	//shadowTexturePixelFormat[Light::LT_POINT] = PF_X8R8G8B8;
	//shadowTextureSize[Light::LT_DIRECTIONAL] = 512;
	//shadowTextureSize[Light::LT_SPOTLIGHT] = 512;
	//shadowTextureSize[Light::LT_POINT] = 256;
	//shadowMaxTextureCount[Light::LT_DIRECTIONAL] = 1;
	//shadowMaxTextureCount[Light::LT_SPOTLIGHT] = 1;
	//shadowMaxTextureCount[Light::LT_POINT] = 1;
	//shadowDirectionalLightSplitTextureCount = 1;

	//memset(mShadowTextureCustomCasterPass, 0, sizeof(mShadowTextureCustomCasterPass));
	//shadowMapGeneratingCurrentLight = NULL;
	//shadowMapGeneratingCurrentPSSMIndexOrFaceIndex = 0;

	//dummyCameraForGuiRendererItemRenderables = NULL;

	//drawShadowDebugging = false;

	//memset(shadowLightBiasDirectionalLight, 0, sizeof(shadowLightBiasDirectionalLight));
	//shadowLightBiasPointLight = Vector2::ZERO;
	//shadowLightBiasSpotLight = Vector2::ZERO;

	//deferredShadingCachedParameters = false;
	//deferredShadingCachedDeferredShadingEnabled = false;
	//deferredShadingCachedDeferredShadingMode = false;

	//viewportUpdateCurrentViewport = NULL;
}
//-----------------------------------------------------------------------
SceneManager::~SceneManager()
{
}

//---------------------------------------------------------------------
RenderSystem *SceneManager::getDestinationRenderSystem()
{
	return mDestRenderSystem;
}
}