// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A result data of <see cref="InteractiveObject.ObjectInteractionGetInfo(UIControl, GameMode, ref InteractiveObjectObjectInfo)"/>.
	/// </summary>
	public class InteractiveObjectObjectInfo
	{
		public bool AllowInteract;
		public bool DisplaySelectionRectangle = true;
		public List<string> SelectionTextInfo = new List<string>();
	}

	/// <summary>
	/// An interface of interactive object in the scene.
	/// </summary>
	public interface InteractiveObject
	{
		void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info );

		bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message );

		void ObjectInteractionEnter( ObjectInteractionContext context );
		void ObjectInteractionExit( ObjectInteractionContext context );
		void ObjectInteractionUpdate( ObjectInteractionContext context );
	}
}