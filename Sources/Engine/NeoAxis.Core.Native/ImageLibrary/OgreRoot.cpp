//!!!!!!в огре не будет таймингов или чего-то там еще. всё снаружи вызывается.

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
// Ogre includes
#include "OgreStableHeaders.h"

#include "OgreRoot.h"

#include "OgreRenderSystem.h"
#include "OgreException.h"
//#include "OgreControllerManager.h"
#include "OgreLogManager.h"
#include "OgreMath.h"
#include "OgreDynLibManager.h"
#include "OgreDynLib.h"
//#include "OgreMaterialManager.h"
//#include "OgreMeshManager.h"
//#include "OgreSkeletonManager.h"
#include "OgreStringConverter.h"
#include "OgreArchiveManager.h"
////#include "OgreShadowVolumeExtrudeProgram.h"
//#include "OgreResourceBackgroundQueue.h"
//#include "OgreLight.h"
#include "OgrePlatformInformation.h"
#include "YUVToRGBConverter.h"
//#include "Threading/OgreDefaultWorkQueue.h"
	
#if OGRE_NO_FREEIMAGE == 0
#include "OgreFreeImageCodec.h"
#endif
#if OGRE_NO_DEVIL == 0
#include "OgreILCodecs.h"
#endif
#if OGRE_NO_DDS_CODEC == 0
#include "OgreDDSCodec.h"
#endif

#if OGRE_NO_PVRTC_CODEC == 0
#  include "OgrePVRTCCodec.h"
#endif

#include "OgreSceneManager.h"

#include "NULLRenderSystem.h"

namespace Ogre {

    typedef void (*DLL_START_PLUGIN)();
    typedef void (*DLL_STOP_PLUGIN)();

#ifdef ANDROID
	extern "C" void initializeRenderSystemGLES2();
#endif

	Root* root = NULL;

	_OgreExport Root* getRoot()
	{
		return root;
	}

    //-----------------------------------------------------------------------
    Root::Root(const WString& nativeLibrariesDirectory)
      : mLogManager(0)
	  , mNextFrame(0)
	  , mRemoveQueueStructuresOnClear(false)
	  //, mNextMovableObjectTypeFlag(1)
	  , mIsInitialised(false)
    {
		root = this;

		 sceneManager = NULL;
		 renderSystem = NULL;
		 //mTokenActionMap = NULL;
		 ddsCodec = NULL;
		 pvrtcCodec = NULL;
		 //shadowVolumeExtrudeProgramInitialised = false;
		mLogManager = NULL;
		//mControllerManager = NULL;
		//mDynLibManager = NULL;
		mArchiveManager = NULL;
		//mMaterialManager = NULL;
		//mMeshManager = NULL;
		//mSkeletonManager = NULL;
		mResourceGroupManager = NULL;
		//mResourceBackgroundQueue = NULL;	  
		//!!!!!было
		//mShadowTextureManager = NULL;
		//mAdditionalMRTManager = NULL;
		myOgreVirtualArchiveFactory = NULL;
		//renderingDeviceIndex = 0;
		//profilingToolBeginOperationDelegate = NULL;
		//profilingToolEndOperationDelegate = NULL;

		this->nativeLibrariesDirectory = nativeLibrariesDirectory;

		  //mTokenActionMap = new CompositorScriptCompiler::TokenActionMap();

        // superclass will do singleton checking
        String msg;

        // Init
        //mActiveRenderer = 0;
        mVersion = StringConverter::toString(OGRE_VERSION_MAJOR) + "." +
            StringConverter::toString(OGRE_VERSION_MINOR) + "." +
            StringConverter::toString(OGRE_VERSION_PATCH) + 
			OGRE_VERSION_SUFFIX + " " +
            "(" + OGRE_VERSION_NAME + ")";

		// Create log manager and default log file if there is no log manager yet
		if(!mLogManager)
		{
			mLogManager = OGRE_NEW LogManager();
			mLogManager->createLog(""/*logFileName*/, true, true);
		}

        //// Dynamic library manager
        //mDynLibManager = OGRE_NEW DynLibManager();

        mArchiveManager = OGRE_NEW ArchiveManager();

		// ResourceGroupManager
		mResourceGroupManager = OGRE_NEW ResourceGroupManager();

		//// WorkQueue (note: users can replace this if they want)
		//DefaultWorkQueue* defaultQ = OGRE_NEW DefaultWorkQueue(this, "Root");
		//// never process responses in main thread for longer than 10ms by default
		//defaultQ->setResponseProcessingTimeLimit(10);
		// match threads to hardware
//#if OGRE_THREAD_SUPPORT
//		unsigned threadCount = OGRE_THREAD_HARDWARE_CONCURRENCY;
//		if (!threadCount)
//			threadCount = 1;
//		defaultQ->setWorkerThreadCount(threadCount);
//#endif
//		// only allow workers to access rendersystem if threadsupport is 1
//#if OGRE_THREAD_SUPPORT == 1
//		defaultQ->setWorkersCanAccessRenderSystem(true);
//#else
//		defaultQ->setWorkersCanAccessRenderSystem(false);
//#endif
//		mWorkQueue = defaultQ;
//
//		// ResourceBackgroundQueue
//		mResourceBackgroundQueue = OGRE_NEW ResourceBackgroundQueue(this);

		//!!!!было
		////mShadowTextureManager = OGRE_NEW ShadowTextureManager();
		//mShadowTextureManager = OGRE_NEW MyShadowTextureManager(this);

		//mAdditionalMRTManager = new AdditionalMRTManager(this);

        //// ..material manager
        //mMaterialManager = OGRE_NEW MaterialManager(this);

        //// Mesh manager
        //mMeshManager = OGRE_NEW MeshManager(this);

        //// Skeleton manager
        //mSkeletonManager = OGRE_NEW SkeletonManager(this);

		// Compiler manager
		//mScriptCompilerManager = OGRE_NEW ScriptCompilerManager(root);

#if OGRE_PROFILING
        // Profiler
        mProfiler = OGRE_NEW Profiler();
		Profiler::getSingleton().setTimer(mTimer);
#endif
		  //betauser
        //mFileSystemArchiveFactory = OGRE_NEW FileSystemArchiveFactory();
        //ArchiveManager::getSingleton().addArchiveFactory( mFileSystemArchiveFactory );
        //mZipArchiveFactory = OGRE_NEW ZipArchiveFactory();
        //ArchiveManager::getSingleton().addArchiveFactory( mZipArchiveFactory );
#if OGRE_NO_FREEIMAGE == 0
		// Register image codecs
		FreeImageCodec::startup();
#endif
#if OGRE_NO_DEVIL == 0
	    // Register image codecs
	    ILCodecs::registerCodecs();
#endif
#if OGRE_NO_DDS_CODEC == 0
		// Register image codecs
		DDSCodec::startup();
#endif
#if OGRE_NO_PVRTC_CODEC == 0
        PVRTCCodec::startup(this);
#endif

		//betauser
		  //mExternalTextureSourceManager = OGRE_NEW ExternalTextureSourceManager();

        // Auto window
        //mAutoWindow = 0;

		//mLightFactory = OGRE_NEW LightFactory(this);
		//addMovableObjectFactory(mLightFactory);
		//betauser
		//mManualObjectFactory = OGRE_NEW ManualObjectFactory();
		//addMovableObjectFactory(mManualObjectFactory);

		mLogManager->logMessage("*-*-* OGRE Initialising");
        msg = "*-*-* Version " + mVersion;
        mLogManager->logMessage(msg);

        //// Can't create managers until initialised
        //mControllerManager = 0;

		YUVToRGBConverter::Init();
    }

    //-----------------------------------------------------------------------
    Root::~Root()
    {
        shutdown();

		YUVToRGBConverter::Shutdown();

		  //betauser
        //OGRE_DELETE mSceneManagerEnum;
		//!!!!было
		//OGRE_DELETE mShadowTextureManager;
		//OGRE_DELETE mAdditionalMRTManager;
		//betauser
		//OGRE_DELETE mRenderSystemCapabilitiesManager;

		//betauser
		  //OGRE_DELETE mExternalTextureSourceManager;

#if OGRE_NO_FREEIMAGE == 0
		FreeImageCodec::shutdown();
#endif
#if OGRE_NO_DEVIL == 0
        ILCodecs::deleteCodecs();
#endif
#if OGRE_NO_DDS_CODEC == 0
		DDSCodec::shutdown();
#endif
#if OGRE_NO_PVRTC_CODEC == 0
		PVRTCCodec::shutdown(this);
#endif
#if OGRE_PROFILING
        OGRE_DELETE mProfiler;
#endif
		  OGRE_DELETE mArchiveManager;
        //OGRE_DELETE mSkeletonManager;
        //OGRE_DELETE mMeshManager;

		//OGRE_DELETE mScriptCompilerManager;

		  //if( mControllerManager )
    //        OGRE_DELETE mControllerManager;

		  //betauser. crash
        //unloadPlugins();

  //      OGRE_DELETE mMaterialManager;
  //      Pass::processPendingPassUpdates(this); // make sure passes are cleaned
		//OGRE_DELETE mResourceBackgroundQueue;
        OGRE_DELETE mResourceGroupManager;

		//OGRE_DELETE mEntityFactory;
		//OGRE_DELETE mLightFactory;
		//OGRE_DELETE mBillboardSetFactory;
		//OGRE_DELETE mBillboardChainFactory;
		//OGRE_DELETE mRibbonTrailFactory;

		//OGRE_DELETE mWorkQueue;

        //OGRE_DELETE mDynLibManager;
        OGRE_DELETE mLogManager;

        StringInterface::cleanupDictionary ();

		  //delete (CompositorScriptCompiler::TokenActionMap*)mTokenActionMap;
    }

    //-----------------------------------------------------------------------
    RenderSystem* Root::getRenderSystem(void)
    {
		 return renderSystem;
        // Gets the currently active renderer
        //return mActiveRenderer;

    }

	//-----------------------------------------------------------------------
	//RenderWindow* Root::initialise(bool autoCreateWindow, const String& windowTitle, const String& customCapabilitiesConfig)
	void Root::initialise()
    {
		//init NULL render system
		{
			renderSystem = new NULLRenderSystem(root);
		}

		//if (!mActiveRenderer)
		if (!renderSystem)
		{
			OGRE_EXCEPT(Exception::ERR_INVALID_STATE,
			"Cannot initialise - no render "
			"system has been selected.", "Root::initialise");
		}

   //     if (!mControllerManager)
			//mControllerManager = OGRE_NEW ControllerManager(this);

		PlatformInformation::log(mLogManager->getDefaultLog());
		//mAutoWindow =  mActiveRenderer->_initialise(autoCreateWindow, windowTitle);


        //if (autoCreateWindow && !mFirstTimePostWindowInit)
        //{
        //    oneTimePostWindowInit();
        //    mAutoWindow->_setPrimary();
        //}

		mIsInitialised = true;

        //return mAutoWindow;

    }

    //-----------------------------------------------------------------------
    void Root::shutdown(void)
    {
		//// Since background thread might be access resources,
		//// ensure shutdown before destroying resource manager.
		//mResourceBackgroundQueue->shutdown();
		//mWorkQueue->shutdown();

        //ShadowVolumeExtrudeProgram::shutdown(this);
        mResourceGroupManager->shutdownAll();

		mIsInitialised = false;

		mLogManager->logMessage("*-*-* OGRE Shutdown");
    }

	void Root::registerCodec( Codec *pCodec )
	{
		ms_mapCodecs[pCodec->getType()] = pCodec;
	}

	bool Root::isCodecRegistered( const String& codecType )
	{
		return ms_mapCodecs.find(codecType) != ms_mapCodecs.end();
	}

	void Root::unRegisterCodec( Codec *pCodec )
	{
		ms_mapCodecs.erase(pCodec->getType());
	}

    Codec* Root::getCodec(const String& extension)
    {
        String lwrcase = extension;
		StringUtil::toLowerCase(lwrcase);
        CodecList::const_iterator i = ms_mapCodecs.find(lwrcase);
        if (i == ms_mapCodecs.end())
        {
            OGRE_EXCEPT(Exception::ERR_ITEM_NOT_FOUND, 
                "Cannot find codec for extension " + extension,
                "Codec::getCodec");
        }

        return i->second;

    }

	Codec* Root::getCodec(char *magicNumberPtr, size_t maxbytes)
	{
		for (CodecList::const_iterator i = ms_mapCodecs. begin(); 
			i != ms_mapCodecs.end(); ++i)
		{
			String ext = i->second->magicNumberToFileExt(magicNumberPtr, maxbytes);
			if (!ext.empty())
			{
				// check codec type matches
				// if we have a single codec class that can handle many types, 
				// and register many instances of it against different types, we
				// can end up matching the wrong one here, so grab the right one
				if (ext == i->second->getType())
					return i->second;
				else
					return getCodec(ext);
			}
		}

		return 0;

	}

	WString Root::GetGeneralSetting( const WString& key )
	{
		std::map<WString, WString>::const_iterator i = generalSettings.find( key );
		if( i == generalSettings.end() )
			return L"";
		return i->second;
	}

	void Root::SetGeneralSetting( const WString& key, const WString& value )
	{
		generalSettings[ key ] = value;
	}
}
