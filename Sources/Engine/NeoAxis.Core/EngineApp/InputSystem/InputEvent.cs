// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents general input event. Custom events should be generalized from this one.
	/// </summary>
	public abstract class InputEvent
	{
		InputDevice device;

		//

		public InputEvent( InputDevice device )
		{
			this.device = device;
		}

		/// <summary>
		/// Gets input device description.
		/// </summary>
		public InputDevice Device
		{
			get { return device; }
		}
	}
}
