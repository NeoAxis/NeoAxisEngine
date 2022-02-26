// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;

namespace NeoAxis
{
	public interface IVisibleInHierarchy
	{
		[Browsable( false )]
		bool VisibleInHierarchy { get; }
	}

	public interface ICanBeSelectedInHierarchy
	{
		[Browsable( false )]
		bool CanBeSelectedInHierarchy { get; }
	}
}
