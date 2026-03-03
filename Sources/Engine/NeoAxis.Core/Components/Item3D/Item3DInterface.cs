// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public interface Item3DTypeInterface
	{
	}

	public interface Item3DInterface
	{
		void GetInventoryImage( out ImageComponent image, out object anyData );

		Reference<double> ItemCount
		{
			get;
			set;
		}

		bool Enabled
		{
			get;
			set;
		}
	}
}
