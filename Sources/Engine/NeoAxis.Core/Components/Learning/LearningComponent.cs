// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	[ResourceFileExtension( "learning" )]
#if !DEPLOY
	[EditorControl( typeof( LearningEditor ) )]
#endif
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
