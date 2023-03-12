// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Base class for GPU programs.
	/// </summary>
	public class GpuProgram
	{
		GpuProgramType type;
		Shader realObject;
		bool realObjectInitialized;

		//

		internal GpuProgram( GpuProgramType type, Shader realObject )
		{
			this.type = type;
			this.realObject = realObject;
			realObjectInitialized = true;
		}

		public GpuProgramType Type
		{
			get { return type; }
		}

		public Shader RealObject
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
