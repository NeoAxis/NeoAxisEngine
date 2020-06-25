/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

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
#ifndef __ROOT__
#define __ROOT__

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreSingleton.h"
#include "OgreString.h"
//betauser
//#include "OgreSceneManagerEnumerator.h"
#include "OgreResourceGroupManager.h"
//betauser
#include "OgreSceneManager.h"
#include "OgreSingleton.h"
#include "OgreIteratorWrappers.h"
//#include "OgreWorkQueue.h"
//#include "OgreMovableObject.h"

#include <exception>

namespace Ogre
{
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup General
	*  @{
	*/

    typedef vector<RenderSystem*> RenderSystemList;
	//class MovableObjectFactory;

    /** The root class of the Ogre system.
        @remarks
            The Ogre::Root class represents a starting point for the client
            application. From here, the application can gain access to the
            fundamentals of the system, namely the rendering systems
            available, management of saved configurations, logging, and
            access to other classes in the system. Acts as a hub from which
            all other objects may be reached. An instance of Root must be
            created before any other Ogre operations are called. Once an
            instance has been created, the same instance is accessible
            throughout the life of that object by using Root::getSingleton
            (as a reference) or Root::getSingletonPtr (as a pointer).
    */
    class _OgreExport Root : public RootAlloc
    {
        // To allow update of active renderer if
        // RenderSystem::initialise is used directly
        friend class RenderSystem;
    private:
	 public:
		SceneManager* sceneManager;
		RenderSystem* renderSystem;

        //RenderSystemList mRenderers;
        //RenderSystem* mActiveRenderer;
        String mVersion;
	    bool mQueuedEnd;

        // Singletons
        LogManager* mLogManager;
        //ControllerManager* mControllerManager;
		  //betauser
        //SceneManagerEnumerator* mSceneManagerEnum;
        //SceneManager* mCurrentSceneManager;
        //DynLibManager* mDynLibManager;
        ArchiveManager* mArchiveManager;
        //MaterialManager* mMaterialManager;
        //MeshManager* mMeshManager;
        //SkeletonManager* mSkeletonManager;
		ResourceGroupManager* mResourceGroupManager;
		//ResourceBackgroundQueue* mResourceBackgroundQueue;
		//!!!!!было
		//MyShadowTextureManager* mShadowTextureManager;

		//AdditionalMRTManager* mAdditionalMRTManager;

        //CompositorManager* mCompositorManager;      
        unsigned long mNextFrame;
		bool mRemoveQueueStructuresOnClear;

	protected:

		//typedef map<String, MovableObjectFactory*> MovableObjectFactoryMap;
		//MovableObjectFactoryMap mMovableObjectFactoryMap;
		//uint32 mNextMovableObjectTypeFlag;
		// stock movable factories
		//MovableObjectFactory* mEntityFactory;
		//MovableObjectFactory* mLightFactory;
		//MovableObjectFactory* mBillboardSetFactory;
		//betauser
		//MovableObjectFactory* mManualObjectFactory;
		//MovableObjectFactory* mBillboardChainFactory;
		//MovableObjectFactory* mRibbonTrailFactory;

		/// Are we initialised yet?
		bool mIsInitialised;

        ///** Set of registered frame listeners */
        //set<FrameListener*> mFrameListeners;

        ///** Set of frame listeners marked for removal*/
        //set<FrameListener*> mRemovedFrameListeners;

  //      /** Indicates the type of event to be considered by calculateEventTime(). */
  //      enum FrameEventTimeType {
  //          FETT_ANY = 0, 
		//	FETT_STARTED = 1, 
		//	FETT_QUEUED = 2, 
		//	FETT_ENDED = 3, 
		//	FETT_COUNT = 4
  //      };

  //      /// Contains the times of recently fired events
		//typedef deque<unsigned long> EventTimesQueue;
  //      EventTimesQueue mEventTimes[FETT_COUNT];

  //      /** Internal method for calculating the average time between recently fired events.
  //      @param now The current time in ms.
  //      @param type The type of event to be considered.
  //      */
  //      Real calculateEventTime(unsigned long now, FrameEventTimeType type);

		///** Update a set of event times (note, progressive, only call once for each type per frame) */
		//void populateFrameEvent(FrameEventTimeType type, FrameEvent& evtToUpdate);

    public:

        /** Constructor
        @param pluginFileName The file that contains plugins information.
            Defaults to "plugins.cfg", may be left blank to ignore.
		@param configFileName The file that contains the configuration to be loaded.
			Defaults to "ogre.cfg", may be left blank to load nothing.
		@param logFileName The logfile to create, defaults to Ogre.log, may be 
			left blank if you've already set up LogManager & Log yourself
		*/
        Root(const WString& nativeLibrariesDirectory);
        ~Root();

        /** Retrieve a pointer to the currently selected render system.
        */
        RenderSystem* getRenderSystem(void);

        /** Initialises the renderer.
            @remarks
                This method can only be called after a renderer has been
                selected with Root::setRenderSystem, and it will initialise
                the selected rendering system ready for use.
            @param
                autoCreateWindow If true, a rendering window will
                automatically be created (saving a call to
                Root::createRenderWindow). The window will be
                created based on the options currently set on the render
                system.
            @returns
                A pointer to the automatically created window, if
                requested, otherwise <b>NULL</b>.
        */
	    void initialise();
	    //RenderWindow* initialise(bool autoCreateWindow, const String& windowTitle = "OGRE Render Window",
     //                               const String& customCapabilitiesConfig = StringUtil::BLANK);

		/** Returns whether the system is initialised or not. */
		bool isInitialised(void) const { return mIsInitialised; }

		/** Get whether the entire render queue structure should be emptied on clearing, 
			or whether just the objects themselves should be cleared.
		*/
		bool getRemoveRenderQueueStructuresOnClear() const { return mRemoveQueueStructuresOnClear; }

		/** Set whether the entire render queue structure should be emptied on clearing, 
		or whether just the objects themselves should be cleared.
		*/
		void setRemoveRenderQueueStructuresOnClear(bool r) { mRemoveQueueStructuresOnClear = r; }

        void shutdown(void);

		void registerCodec( Codec *pCodec );
		bool isCodecRegistered( const String& codecType );
		void unRegisterCodec( Codec *pCodec );
		Codec* getCodec(const String& extension);
		Codec* getCodec(char *magicNumberPtr, size_t maxbytes);


		////moved from Node
		//typedef std::nvector<Node*> QueuedUpdates;
		//QueuedUpdates msQueuedUpdates;

		////moved from Pass
		//typedef std::set<Pass*> PassSet;
		//PassSet msDirtyHashList;
		//PassSet msPassGraveyard;

		//moved from Codec
		typedef std::map< String, Codec* > CodecList; 
		CodecList ms_mapCodecs;

		////moved from CompositorScriptCompiler
		//void* mTokenActionMap;

		//moved from DDSCodec
		DDSCodec* ddsCodec;

		//moved from PVRTCCodec
		PVRTCCodec* pvrtcCodec;

		//moved from FreeImageCodec
		typedef std::list<ImageCodec*> RegisteredCodecList;
		RegisteredCodecList freeImageCodecList;

		MyOgreVirtualArchiveFactory* myOgreVirtualArchiveFactory;

		//String renderingDeviceName;
		//int renderingDeviceIndex;

		WString nativeLibrariesDirectory;

		//Pass::HashFunc* passHashFunc;

		//OgreRoot_profilingToolBeginOperationDelegate* profilingToolBeginOperationDelegate;
		//OgreRoot_profilingToolEndOperationDelegate* profilingToolEndOperationDelegate;

		std::map<WString, WString> generalSettings;
		WString GetGeneralSetting( const WString& key );
		void SetGeneralSetting( const WString& key, const WString& value );
    };

extern Root* root;
_OgreExport Root* getRoot();

	/** @} */
	/** @} */
} // Namespace Ogre
#endif
