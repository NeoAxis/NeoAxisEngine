// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows;
using NeoAxis;

namespace SampleWidgetWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			Close();
		}

		private void Button_Click_1( object sender, RoutedEventArgs e )
		{
			if( Scene.GetFirst() == null )
			{
				MessageBox.Show( "The scene has not been created yet." );
				return;
			}

			AdditionalWindow window = new AdditionalWindow();
			window.Show();
		}
	}
}
