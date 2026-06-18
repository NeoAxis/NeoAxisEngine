// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using NeoAxis.Widget;

namespace SampleWidgetWPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );

			WidgetApp.WPFInit( "user:Logs/SampleWidgetWPF.log", "user:Configs/SampleWidgetWPF.config" );
		}

		protected override void OnExit( ExitEventArgs e )
		{
			WidgetApp.WPFShutdown();

			base.OnExit( e );
		}
	}
}
