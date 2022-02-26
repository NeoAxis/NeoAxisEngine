// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	public class ObjectInteractionContext
	{
		public InteractiveObject Obj;
		public object AnyData;
		public GameMode GameMode;
		public Viewport Viewport;

		public ObjectInteractionContext( InteractiveObject obj, GameMode gameMode, Viewport viewport )
		{
			Obj = obj;
			GameMode = gameMode;
			Viewport = viewport;
		}

		public virtual void Dispose() { }
	}
}
