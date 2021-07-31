// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An interface for the object to interact Player app with the scene.
	/// </summary>
	public interface IComponent_GameMode
	{
		Reference<Component> ObjectControlledByPlayer { get; set; }
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Component_GameMode_ObjectInteractionContextClass
	{
		public IComponent_InteractiveObject Obj;
		public object AnyData;
		public UIControl PlayScreen;
		public IComponent_GameMode GameMode;
		public Viewport Viewport;

		public Component_GameMode_ObjectInteractionContextClass( IComponent_InteractiveObject obj, UIControl playScreen, IComponent_GameMode gameMode, Viewport viewport )
		{
			Obj = obj;
			PlayScreen = playScreen;
			GameMode = gameMode;
			Viewport = viewport;
		}

		public virtual void Dispose() { }
	}

}
