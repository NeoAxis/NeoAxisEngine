// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Class for adding engine metadata extensions. Can be used for engine add-ons creation.
	/// </summary>
	public abstract class MetadataExtensions
	{
		/// <summary>
		/// Method for registering metadata extensions. Called when initializing engine metadata.
		/// </summary>
		public abstract void Register();
	}
}
