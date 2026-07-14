// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register seat resources.
	/// </summary>
	public class SeatAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			ResourceManager.RegisterType( "Seat Type", new string[] { "seattype" }, typeof( Resource ) );
		}
	}
}
