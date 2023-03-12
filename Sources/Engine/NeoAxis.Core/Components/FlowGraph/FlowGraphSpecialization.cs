// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// The specialization of the flow graph. Specialization affects graph workflow in the editor.
	/// </summary>
	public abstract class FlowGraphSpecialization : Component
	{
		//!!!!
		/// <summary>
		/// Represents a drag and drop data for creation <see cref="FlowGraphSpecialization"/> component.
		/// </summary>
		public class DragDropObjectCreateInitNodeContext
		{
			public Metadata.TypeInfo createComponentType;
			public string memberFullSignature = "";
			public Component createNodeWithComponent;

			public Component controlledObject;
		}

		//!!!!name
		public abstract void DragDropObjectCreateInitNode( FlowGraphNode node, DragDropObjectCreateInitNodeContext context, ref bool handled );
	}
}
