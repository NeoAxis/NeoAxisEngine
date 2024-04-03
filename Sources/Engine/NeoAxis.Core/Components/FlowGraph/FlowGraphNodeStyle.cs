// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The style of the flow graph node.
	/// </summary>
	public abstract class FlowGraphNodeStyle : Component
	{
#if !DEPLOY
		public abstract Vector2 GetSocketPositionInUnits( FlowGraphNode.Representation.Item socket, bool input );

		//EditorRenderSelectionState.Selected
		public abstract void RenderNodeReferences( IFlowGraphEditor window, FlowGraphNode node,
			Dictionary<Component, List<FlowGraphNode>> objectToNodes,
			Dictionary<FlowGraphNode.Representation.Item, RenderSelectionState> referenceSelectionStates,
			out FlowGraphNode.Representation.Item outMouseOverReference );

		public abstract void RenderNode( IFlowGraphEditor window, FlowGraphNode node,
			RenderSelectionState selectionStateNode, RenderSelectionState selectionStateControlledObject, object mouseOverObject,
			FlowGraphNode.Representation.Connector referenceCreationSocketFrom, IDragDropSetReferenceData dragDropSetReferenceData );

		public abstract object GetMouseOverObject( IFlowGraphEditor window, FlowGraphNode node );

		public abstract bool IsIntersectsWithRectangle( IFlowGraphEditor window, FlowGraphNode node, Rectangle rectInUnits );
#endif
	}
}
