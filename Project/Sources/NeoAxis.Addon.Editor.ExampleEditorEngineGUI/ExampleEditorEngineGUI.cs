// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorEngineGUI
{
	[AddToResourcesWindow( @"Addons\Examples\Example Editor Engine GUI" )]
	[EditorControl( typeof( ExampleEditorEngineGUIEditor ) )]
	public class ExampleEditorEngineGUI : Component
	{
		public string ValueToDisplay { get; set; } = "Value to display";
	}
}
