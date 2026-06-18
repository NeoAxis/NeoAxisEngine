// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
