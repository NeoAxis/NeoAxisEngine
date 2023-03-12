// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
			if( Scene.First == null )
			{
				MessageBox.Show( "The scene has not been created yet." );
				return;
			}

			AdditionalWindow window = new AdditionalWindow();
			window.Show();
		}
	}
}
