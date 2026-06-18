// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorWinForms
{
	[AddToResourcesWindow( @"Addons\Examples\Example Editor WinForms" )]
	[EditorControl( typeof( ExampleEditorWinFormsWindow ) )]
	public class ExampleEditorWinForms : Component
	{
		public string ValueToDisplay { get; set; } = "Value to display";
	}
}
