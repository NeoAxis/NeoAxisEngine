// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NeoAxis.Widget
{
	/// <summary>
	/// Represents an engine widget for WPF.
	/// </summary>
	public partial class WidgetControlWPF : UserControl
	{
		static List<WidgetControlWPF> allInstances = new List<WidgetControlWPF>();

		WidgetControlWinForms innerControl;
		System.Windows.Forms.Timer updateTimer;

		//

		public WidgetControlWPF()
		{
			InitializeComponent();

			if( DesignerProperties.GetIsInDesignMode( this ) )
				return;

			innerControl = new WidgetControlWinForms();
			innerControl.DestroyEvent += InnerControl_DestroyEvent;
			HostControl.Child = innerControl;

			float interval = 1;
			//float interval = ( automaticUpdateFPS != 0 ) ? ( ( 1.0f / automaticUpdateFPS ) * 1000.0f ) : 100;
			updateTimer = new System.Windows.Forms.Timer();
			updateTimer.Interval = (int)interval;
			updateTimer.Tick += UpdateTimer_Tick;
			updateTimer.Enabled = true;

			//Loaded += WidgetControlWPF_Loaded;

			allInstances.Add( this );
		}

		public static List<WidgetControlWPF> AllInstances
		{
			get { return allInstances; }
		}

		//private void WidgetControlWPF_Loaded( object sender, RoutedEventArgs e )
		//{
		//	//innerControl = new WidgetControlWinForms();
		//	//HostControl.Child = innerControl;
		//}

		//protected override void OnInitialized( EventArgs e )
		//{
		//	base.OnInitialized( e );

		//	if( DesignerProperties.GetIsInDesignMode( this ) )
		//		return;
		//}

		private void InnerControl_DestroyEvent( EngineViewportControl sender )
		{
			allInstances.Remove( this );

			updateTimer?.Dispose();
			updateTimer = null;
		}

		protected override void OnRender( DrawingContext drawingContext )
		{
			base.OnRender( drawingContext );

			if( DesignerProperties.GetIsInDesignMode( this ) )
				return;

			innerControl?.TryRender();
		}

		public WidgetControlWinForms InnerControl
		{
			get { return innerControl; }
		}

		void UpdateTimer_Tick( object sender, EventArgs e )
		{
			if( DesignerProperties.GetIsInDesignMode( this ) )
				return;
			if( innerControl == null || innerControl.Destroyed )
				return;
			if( !IsLoaded )
				return;

			//!!!!
			//if( WPFAppWorld.DuringWarningOrErrorMessageBox )
			//	return;

			if( innerControl.AutomaticUpdateFPS != 0 && innerControl.IsTimeToUpdate() && innerControl.IsAllowRender() )
				InvalidateVisual();
		}
	}
}
