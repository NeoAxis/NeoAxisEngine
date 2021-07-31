// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorWinForms
{
	[AddToResourcesWindow( @"Addons\Example Editor WinForms" )]
	[EditorDocumentWindow( typeof( ExampleEditorWinFormsWindow ) )]
	public class Component_ExampleEditorWinForms : Component
	{
		public string ValueToDisplay { get; set; } = "Value to display";
	}
}
