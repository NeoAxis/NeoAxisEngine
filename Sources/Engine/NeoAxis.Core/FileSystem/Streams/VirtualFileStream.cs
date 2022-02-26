// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Defines a file stream for virtual file system.
	/// </summary>
	public abstract class VirtualFileStream : Stream
	{
		public abstract int ReadUnmanaged( IntPtr buffer, int count );
	}
}
