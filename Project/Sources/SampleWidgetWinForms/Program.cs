// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NeoAxis.Widget;

namespace SampleWidgetWinForms
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			WidgetApp.WinFormsMain( typeof( SampleForm ), true, "user:Logs/SampleWidgetWinForms.log", "user:Configs/SampleWidgetWinForms.config" );
		}
	}
}
