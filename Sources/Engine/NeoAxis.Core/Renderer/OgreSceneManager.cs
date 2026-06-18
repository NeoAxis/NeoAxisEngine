// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	struct MyOgreSceneManager
	{
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class OgreSceneManager
	{
		static unsafe internal MyOgreSceneManager* realObject;

		internal unsafe OgreSceneManager( MyOgreSceneManager* realObject )
		{
			OgreSceneManager.realObject = realObject;
		}

		internal unsafe void Dispose()
		{
			if( realObject != null )
			{
				OgreRoot.destroySceneManager( realObject );
				realObject = null;
			}
		}
	}
}
