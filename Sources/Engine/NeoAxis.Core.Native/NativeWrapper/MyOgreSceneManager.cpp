// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OgreNativeWrapperGeneral.h"
#include "MyOgreSceneManager.h"
#include "OgreRoot.h"
#include "MiniDump.h"

using namespace Ogre;


//!!!!убрать совсем OgreMain.dll? переименовать в NeoAxis.Renderer.Native.
//!!!!!!!!убрать всё об огре

///////////////////////////////////////////////////////////////////////////////////////////////////

//void MyOgreSceneManager::findLightsAffectingFrustum(const Camera* camera)
//{
//	mLightsAffectingFrustum.clear();
//
//	findLightsAffectingFrustumFromOgreDelegate((Camera*)camera);
//
//	for(int n = 0; n < mLightsAffectingFrustum.size(); n++)
//	{
//		Light* light = mLightsAffectingFrustum[n];
//
//		if(mCameraRelativeRendering)
//			light->_setCameraRelative(mCameraInProgress);
//		else
//			light->_setCameraRelative(0);
//	}
//
//	//sort
//	{
//		for(int n = 0; n < mLightsAffectingFrustum.size(); n++)
//		{
//			Light* light = mLightsAffectingFrustum[n];
//
//			if(light->getType() == Light::LT_POINT || light->getType() == Light::LT_SPOTLIGHT)
//			{
//				light->tempFindLightsAffectingFrustumCameraDistanceSqr = 
//					(camera->getDerivedPosition() - light->getDerivedPosition()).squaredLength();
//			}
//			else
//			{
//				light->tempFindLightsAffectingFrustumCameraDistanceSqr = 0;
//			}
//		}
//
//		std::stable_sort( mLightsAffectingFrustum.begin(), mLightsAffectingFrustum.end(), 
//			findLightsAffectingFrustumSort());
//	}
//
//	_notifyLightsDirty();
//}

//void MyOgreSceneManager::_renderScene(Camera* camera, Viewport* vp, bool includeOverlays)
//{
//	SceneManager::_renderScene(camera, vp, includeOverlays);
//
//	renderSceneAfterFromOgreDelegate(camera, vp);
//}

///////////////////////////////////////////////////////////////////////////////////////////////////

//EXPORT void MyOgreSceneManager_setCallbackDelegates( MyOgreSceneManager* _this, 
//	MyOgreSceneManager_viewportUpdateBeforeFromOgreDelegate* viewportUpdateBeforeFromOgre,
//	MyOgreSceneManager_viewportUpdateAfterFromOgreDelegate* viewportUpdateAfterFromOgre,
//	//MyOgreSceneManager_doRenderObjectPassEventFromOgre* doRenderObjectPassEventFromOgre,
//	//MyOgreSceneManager_preRenderVisibleObjectsFromOgreDelegate* preRenderVisibleObjectsFromOgre,
//	//MyOgreSceneManager_postRenderVisibleObjectsFromOgreDelegate* postRenderVisibleObjectsFromOgre,
//	//MyOgreSceneManager_findShadowCastersForLightFromOgreDelegate* findShadowCastersForLightFromOgre,
//	//MyOgreSceneManager_findLightsAffectingFrustumFromOgreDelegate* findLightsAffectingFrustumFromOgre,
//	MyOgreSceneManager_getShadowmapCameraSetupFromOgreDelegate* getShadowmapCameraSetupFromOgre
//	//MyOgreSceneManager_renderSceneAfterFromOgreDelegate* renderSceneAfterFromOgre,
//	//MyOgreSceneManager_prepareRenderQueueFromOgreDelegate* prepareRenderQueueFromOgre,
//	//MyOgreSceneManager_beginCameraStatisticsCountingFromOgreDelegate* beginCameraStatisticsCountingFromOgre,
//	//MyOgreSceneManager_endCameraStatisticsCountingFromOgreDelegate* endCameraStatisticsCountingFromOgre
//	)
//{
//	_this->viewportUpdateBeforeFromOgreDelegate = viewportUpdateBeforeFromOgre;
//	_this->viewportUpdateAfterFromOgreDelegate = viewportUpdateAfterFromOgre;
//	//_this->updateSceneGraphFromOgreDelegate = updateSceneGraphFromOgre;
//	//_this->findVisibleObjectsFromOgreDelegate = findVisibleObjectsFromOgre;
//	//_this->doRenderObjectPassEventFromOgreDelegate = doRenderObjectPassEventFromOgre;
//	//_this->preRenderVisibleObjectsFromOgreDelegate = preRenderVisibleObjectsFromOgre;
//	//_this->postRenderVisibleObjectsFromOgreDelegate = postRenderVisibleObjectsFromOgre;
//	//_this->findShadowCastersForLightFromOgreDelegate = findShadowCastersForLightFromOgre;
//	//_this->findLightsAffectingFrustumFromOgreDelegate = findLightsAffectingFrustumFromOgre;
//	_this->getShadowmapCameraSetupFromOgreDelegate = getShadowmapCameraSetupFromOgre;
//	//_this->renderSceneAfterFromOgreDelegate = renderSceneAfterFromOgre;
//	//_this->prepareRenderQueueFromOgreDelegate = prepareRenderQueueFromOgre;
//	//_this->beginCameraStatisticsCountingFromOgreDelegate = beginCameraStatisticsCountingFromOgre;
//	//_this->endCameraStatisticsCountingFromOgreDelegate = endCameraStatisticsCountingFromOgre;
//}

//EXPORT Camera* MyOgreSceneManager_createCamera( MyOgreSceneManager* _this, wchar16* name )
//{
//	return _this->createCamera(TO_WCHAR_T(name));
//}
//
//EXPORT void MyOgreSceneManager_destroyCamera( MyOgreSceneManager* _this, Camera* camera )
//{
//	_this->destroyCamera(camera);
//}

//EXPORT Light* MyOgreSceneManager_createLight( MyOgreSceneManager* _this )
//{
//	return _this->createLight();
//}
//
//EXPORT void MyOgreSceneManager_destroyLight( MyOgreSceneManager* _this, Light* light )
//{
//	_this->destroyLight(light);
//}
//
//EXPORT void MyOgreSceneManager_setAmbientLight( MyOgreSceneManager* _this, const ColourValue& value )
//{
//	_this->setAmbientLight(value);
//}
//
//EXPORT void MyOgreSceneManager_setShadowColour( MyOgreSceneManager* _this, const ColourValue& value )
//{
//	_this->setShadowColour(value);
//}
//
//EXPORT void MyOgreSceneManager_setShadowFarDistance( MyOgreSceneManager* _this, float value )
//{
//	_this->setShadowFarDistance(value);
//}
//
//EXPORT void MyOgreSceneManager_setShadowTextureFadeStart( MyOgreSceneManager* _this, float value )
//{
//	_this->setShadowTextureFadeStart(value);
//}
//
//EXPORT void MyOgreSceneManager_setShadowCasterRenderBackFaces(	MyOgreSceneManager* _this, bool value )
//{
//	_this->setShadowCasterRenderBackFaces(value);
//}
//
//EXPORT void MyOgreSceneManager_setShadowTextureSelfShadow( MyOgreSceneManager* _this, bool value )
//{
//	_this->setShadowTextureSelfShadow(value);
//}

//EXPORT void MyOgreSceneManager_setShadowDirLightTextureOffset( MyOgreSceneManager* _this, float value )
//{
//	_this->setShadowDirLightTextureOffset(value);
//}

//EXPORT void MyOgreSceneManager_setShadowDirectionalLightExtrusionDistance( MyOgreSceneManager* _this, float value )
//{
//	_this->setShadowDirectionalLightExtrusionDistance(value);
//}

//EXPORT void MyOgreSceneManager_setShadowTextureCasterMaterial( MyOgreSceneManager* _this, 
//	Light::LightTypes lightType, wchar16* value )
//{
//	_this->setShadowTextureCasterMaterial(lightType, TO_WCHAR_T(value));
//}

//EXPORT void MyOgreSceneManager_setFog( MyOgreSceneManager* _this, FogMode mode, const ColourValue& color, float expDensity, 
//	float linearStart, float linearEnd )
//{
//	_this->setFog(mode, color, expDensity, linearStart, linearEnd);
//}

//EXPORT void MyOgreSceneManager_addRenderQueueListener( MyOgreSceneManager* _this, MyOgreRenderQueueListener* listener )
//{
//	_this->addRenderQueueListener(listener);
//}
//
//EXPORT void MyOgreSceneManager_removeRenderQueueListener( MyOgreSceneManager* _this, MyOgreRenderQueueListener* listener )
//{
//	_this->removeRenderQueueListener(listener);
//}

//EXPORT void MyOgreSceneManager_setShadowTechnique( MyOgreSceneManager* _this, ShadowTechnique value )
//{
//	_this->setShadowTechnique(value);
//
//}

//EXPORT void MyOgreSceneManager_addListener( MyOgreSceneManager* _this, MyOgreSceneManagerListener* listener )
//{
//	_this->addListener(listener);
//}
//
//EXPORT void MyOgreSceneManager_removeListener( MyOgreSceneManager* _this, MyOgreSceneManagerListener* listener )
//{
//	_this->removeListener(listener);
//}

//EXPORT void MyOgreSceneManager_getShadowTexturesStatistics( MyOgreSceneManager* _this, int* triangleCount, int* batchCount )
//{
//	_this->root->mShadowTextureManager->getShadowTexturesStatistics(triangleCount, batchCount);
//}
//
//EXPORT void MyOgreSceneManager_resetShadowTexturesStatistics(MyOgreSceneManager* _this)
//{
//	_this->root->mShadowTextureManager->resetShadowTexturesStatistics();
//}

//EXPORT int MyOgreSceneManager_getLightsAffectingFrustum_size(MyOgreSceneManager* _this)
//{
//	return _this->_getLightsAffectingFrustum().size();
//}
//
//EXPORT Light* MyOgreSceneManager_getLightsAffectingFrustum_item(MyOgreSceneManager* _this, int index)
//{
//	return _this->_getLightsAffectingFrustum()[index];
//}

//EXPORT int MyOgreSceneManager_getParticleSystemsBoundsUpdatedCount(MyOgreSceneManager* _this)
//{
//	return _this->particleSystemsBoundsUpdated.size();
//}
//
//EXPORT void MyOgreSceneManager_getParticleSystemsBoundsUpdated( MyOgreSceneManager* _this, ParticleSystem** list )
//{
//	ParticleSystem** pointer = list;
//
//	std::set<ParticleSystem*>& set = _this->particleSystemsBoundsUpdated;
//	for(std::set<ParticleSystem*>::iterator i = set.begin(); i != set.end(); i++)
//	{
//		*pointer = *i;
//		pointer++;
//	}
//}
//
//EXPORT void MyOgreSceneManager_clearParticleSystemsBoundsUpdated(MyOgreSceneManager* _this)
//{
//	_this->particleSystemsBoundsUpdated.clear();
//}

//EXPORT const Pass* MyOgreSceneManager_GetCurrentRenderPass(MyOgreSceneManager* _this)
//{
//	return _this->currentRenderPass;
//}
//
//EXPORT void MyOgreSceneManager_ResetCurrentRenderPass(MyOgreSceneManager* _this)
//{
//	_this->currentRenderPass = NULL;
//}

//EXPORT void MyOgreSceneManager_setShadowTextureSettings( MyOgreSceneManager* _this, 
//	PixelFormat shadowDirectionalLightTextureFormat, int shadowDirectionalLightTextureSize, 
//	int shadowDirectionalLightMaxTextureCount, int shadowDirectionalLightSplitTextureCount,
//	PixelFormat shadowSpotLightTextureFormat, int shadowSpotLightTextureSize, int shadowSpotLightMaxTextureCount, 
//	PixelFormat shadowPointLightTextureFormat, int shadowPointLightTextureSize, int shadowPointLightMaxTextureCount )
//{
//	bool dirty = false;
//
//	if(_this->shadowTexturePixelFormat[Light::LT_DIRECTIONAL] != shadowDirectionalLightTextureFormat)
//	{
//		_this->shadowTexturePixelFormat[Light::LT_DIRECTIONAL] = shadowDirectionalLightTextureFormat;
//		dirty = true;
//	}
//	if(_this->shadowTextureSize[Light::LT_DIRECTIONAL] != shadowDirectionalLightTextureSize)
//	{
//		_this->shadowTextureSize[Light::LT_DIRECTIONAL] = shadowDirectionalLightTextureSize;
//		dirty = true;
//	}
//	if(_this->shadowMaxTextureCount[Light::LT_DIRECTIONAL] != shadowDirectionalLightMaxTextureCount)
//	{
//		_this->shadowMaxTextureCount[Light::LT_DIRECTIONAL] = shadowDirectionalLightMaxTextureCount;
//		dirty = true;
//	}
//	if(_this->shadowDirectionalLightSplitTextureCount != shadowDirectionalLightSplitTextureCount)
//	{
//		_this->shadowDirectionalLightSplitTextureCount = shadowDirectionalLightSplitTextureCount;
//		dirty = true;
//	}
//
//	if(_this->shadowTexturePixelFormat[Light::LT_SPOTLIGHT] != shadowSpotLightTextureFormat)
//	{
//		_this->shadowTexturePixelFormat[Light::LT_SPOTLIGHT] = shadowSpotLightTextureFormat;
//		dirty = true;
//	}
//	if(_this->shadowTextureSize[Light::LT_SPOTLIGHT] != shadowSpotLightTextureSize)
//	{
//		_this->shadowTextureSize[Light::LT_SPOTLIGHT] = shadowSpotLightTextureSize;
//		dirty = true;
//	}
//	if(_this->shadowMaxTextureCount[Light::LT_SPOTLIGHT] != shadowSpotLightMaxTextureCount)
//	{
//		_this->shadowMaxTextureCount[Light::LT_SPOTLIGHT] = shadowSpotLightMaxTextureCount;
//		dirty = true;
//	}
//
//	if(_this->shadowTexturePixelFormat[Light::LT_POINT] != shadowPointLightTextureFormat)
//	{
//		_this->shadowTexturePixelFormat[Light::LT_POINT] = shadowPointLightTextureFormat;
//		dirty = true;
//	}
//	if(_this->shadowTextureSize[Light::LT_POINT] != shadowPointLightTextureSize)
//	{
//		_this->shadowTextureSize[Light::LT_POINT] = shadowPointLightTextureSize;
//		dirty = true;
//	}
//	if(_this->shadowMaxTextureCount[Light::LT_POINT] != shadowPointLightMaxTextureCount)
//	{
//		_this->shadowMaxTextureCount[Light::LT_POINT] = shadowPointLightMaxTextureCount;
//		dirty = true;
//	}
//
//	//!!!!!было
//	//if(dirty)
//	//{
//	//	_this->destroyShadowTextures();
//	//}
//}
//
//EXPORT void MyOgreSceneManager_setShadowDirectionalLightSplitDistances( MyOgreSceneManager* _this, 
//	float* shadowDirectionalLightSplitDistancesArray, int shadowDirectionalLightSplitDistancesLength )
//{
//	_this->shadowDirectionalLightSplitDistances.clear();
//	for(int n = 0; n < shadowDirectionalLightSplitDistancesLength; n++)
//		_this->shadowDirectionalLightSplitDistances.push_back( shadowDirectionalLightSplitDistancesArray[n] );
//}

//!!!!!было
//EXPORT void MyOgreSceneManager_destroyShadowTextures(MyOgreSceneManager* _this)
//{
//	_this->destroyShadowTextures();
//}

//EXPORT void MyOgreSceneManager_destroyAdditionalMRTs(MyOgreSceneManager* _this)
//{
//	_this->destroyAdditionalMRTs();
//}

//EXPORT void MyOgreSceneManager_findShadowCastersForLight_addMovableObject(
//	MyOgreSceneManager* _this, MovableObject* movableObject)
//{
//	_this->currentStencilShadowsCasterItem->movableObjects.push_back(movableObject);
//}
//
//EXPORT void MyOgreSceneManager_findShadowCastersForLight_addStaticMeshObject(
//	MyOgreSceneManager* _this, StaticMeshObjectRenderable* renderable)
//{
//	_this->currentStencilShadowsCasterItem->staticMeshObjectRenderables.push_back(renderable);
//}

//EXPORT void MyOgreSceneManager_findLightsAffectingFrustum_addLight(MyOgreSceneManager* _this, Light* light,
//	bool allowCastShadows)
//{
//	_this->mLightsAffectingFrustum.push_back(light);
//	light->lightsAffectingFrustumAllowCastShadows = allowCastShadows;
//}

//EXPORT void MyOgreSceneManager_setDrawShadowDebugging( MyOgreSceneManager* _this, bool value )
//{
//	_this->drawShadowDebugging = value;
//}
//
//EXPORT void MyOgreSceneManager_setShadowLightBias( MyOgreSceneManager* _this, Light::LightTypes lightType, 
//	int directionalLightPSSMIndex, float constant, float slope )
//{
//	Vector2 value = Vector2(constant, slope);
//
//	switch(lightType)
//	{
//	case Light::LT_DIRECTIONAL:
//		_this->shadowLightBiasDirectionalLight[directionalLightPSSMIndex] = value;
//		break;
//	case Light::LT_POINT:
//		_this->shadowLightBiasPointLight = value;
//		break;
//	case Light::LT_SPOTLIGHT:
//		_this->shadowLightBiasSpotLight = value;
//		break;
//	}
//}

//EXPORT void MyOgreSceneManager_setStencilShadowsMaterial( MyOgreSceneManager* _this, 
//	Light::LightTypes lightType, wchar16* value )
//{
//	_this->stencilShadowsMaterials[(int)lightType] = WString(TO_WCHAR_T(value));
//}

//EXPORT bool MyOgreSceneManager_checkActiveCompositorChainTargetOperation( MyOgreSceneManager* _this )
//{
//	return _this->activeCompositorChainTargetOperation != NULL;
//}
//
//EXPORT bool MyOgreSceneManager_activeCompositorChainTargetOperationTestRenderQueueGroup( 
//	MyOgreSceneManager* _this, int groupID )
//{
//	if(_this->activeCompositorChainTargetOperation)
//	{
//		CompositorInstance::TargetOperation* operation = (CompositorInstance::TargetOperation*)
//			_this->activeCompositorChainTargetOperation;
//		if(operation->renderQueues.test(groupID))
//			return true;
//	}
//	return false;
//}

//EXPORT void MyOgreSceneManager_resetDataBeforeRendering( void* _this, int sceneNodesCount, SceneNode** sceneNodes )
//{
//	for(int n = 0;n < sceneNodesCount; n++)
//	{
//		SceneNode* sceneNode = sceneNodes[n];
//		sceneNode->sceneRenderingData_affectedLights.resize(0);
//	}
//}

///////////////////////////////////////////////////////////////////////////////////////////////////
