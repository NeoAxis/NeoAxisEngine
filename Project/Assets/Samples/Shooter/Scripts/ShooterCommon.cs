// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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