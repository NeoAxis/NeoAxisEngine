// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Represents a linked GPU programs, usually a pair of vertex and fragment programs.
	/// </summary>
	public class GpuLinkedProgram
	{
		GpuProgram[] programs;
		Program realObject;
		bool realObjectInitialized;

		//

		internal GpuLinkedProgram( GpuProgram[] programs, Program realObject )
		{
			this.programs = programs;
			this.realObject = realObject;
			realObjectInitialized = true;
		}

		public GpuProgram[] Programs
		{
			get { return programs; }
		}

		public Program RealObject
		{
			get { return realObject; }
		}

		//!!!!
		internal void _Dispose()
		{
			if( realObjectInitialized )
			{
				realObject.Dispose();
				realObjectInitialized = false;
			}
		}
	}
}
