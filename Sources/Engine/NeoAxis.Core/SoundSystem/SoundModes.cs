// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	[Flags]
	public enum SoundModes
	{
		Mode3D = 0x00000002,
		Loop = 0x00000004,
		Stream = 0x00000008,
		Software = 0x000000010,
		Record = 0x000000020,
		//ListenerRelative = 0x000000040,
	}
}
