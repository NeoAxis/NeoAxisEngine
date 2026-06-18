// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents input device.
	/// </summary>
	public abstract class InputDevice
	{
		string name;

		//

		InputDevice() { }

		protected InputDevice( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		protected abstract void OnUpdateState();
		protected abstract void OnShutdown();

		internal void CallOnUpdateState()
		{
			OnUpdateState();
		}

		internal void CallOnShutdown()
		{
			OnShutdown();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
