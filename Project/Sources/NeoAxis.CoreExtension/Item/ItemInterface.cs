// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public interface ItemTypeInterface
	{
	}

	public interface ItemInterface
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
