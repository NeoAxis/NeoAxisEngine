// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The graphical style of the flow graph.
	/// </summary>
	public abstract class FlowGraphStyle : Component
	{
#if !DEPLOY
		public abstract void RenderBackground( IFlowGraphEditor window );
		public abstract void RenderForeground( IFlowGraphEditor window );
		public abstract void RenderReference( IFlowGraphEditor window, Vector2 from, bool fromInput, Vector2 to, ColorValue color, out bool mouseOver );
#endif
	}
}
