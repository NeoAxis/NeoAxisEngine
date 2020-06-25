// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Input
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
