// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis
{
	/// <summary>
	/// Class for generating unique names.
	/// </summary>
	/*public */class UniqueNameGenerator
	{
		long counter1;
		long counter2;

		public string Get( string prefix )
		{
			unchecked
			{
				counter1++;
				if( counter1 == long.MaxValue )
				{
					counter1 = 0;
					counter2++;
				}
			}

			if( counter2 != 0 )
				return $"{prefix} {counter1} {counter2}";
			else
				return $"{prefix} {counter1}";
		}
	}
}
