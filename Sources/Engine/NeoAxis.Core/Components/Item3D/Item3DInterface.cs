// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
