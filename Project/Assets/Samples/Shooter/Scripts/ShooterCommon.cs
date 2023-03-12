// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Linq;
using NeoAxis;

namespace Project
{
	public enum ShooterGameTypeEnum
	{
		FreeForAll,
		TeamDeathmatch,
		BattleRoyale
	}

	public enum ShooterGameStatusEnum
	{
		Preparing,
		Playing,
	}
}