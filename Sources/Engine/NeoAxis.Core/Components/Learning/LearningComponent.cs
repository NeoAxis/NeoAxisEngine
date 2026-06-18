// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	[ResourceFileExtension( "learning" )]
	[EditorControl( "NeoAxis.Editor.LearningEditor" )]
	public class LearningComponent : Component
	{
		[Serialize]
		[Browsable( false )]
		public List<string> DoneList { get; set; } = new List<string>();

		[Serialize]
		[Browsable( false )]
		public int SelectedPage { get; set; }
	}
}
