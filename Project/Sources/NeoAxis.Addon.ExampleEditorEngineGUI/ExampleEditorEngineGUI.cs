// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorEngineGUI
{
	[AddToResourcesWindow( @"Addons\Example Editor Engine GUI" )]
	[EditorDocumentWindow( typeof( ExampleEditorEngineGUIEditor ) )]
	public class Component_ExampleEditorEngineGUI : Component
	{
		public string ValueToDisplay { get; set; } = "Value to display";
	}
}
