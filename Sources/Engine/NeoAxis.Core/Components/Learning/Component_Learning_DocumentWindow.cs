// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Learning_DocumentWindow : DocumentWindowWithViewport
	{
		UIControl learningControl;

		//

		public Component_Learning_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_Learning Learning
		{
			get { return (Component_Learning)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			//load screen
			learningControl = ResourceManager.LoadSeparateInstance<UIControl>( @"Base\Learning\LearningScreen.ui", false, true );
			if( learningControl != null )
			{
				Viewport.UIContainer.AddComponent( learningControl );

				//init check boxes

				var doneList = new ESet<string>();
				foreach( var item in Learning.DoneList )
					doneList.AddWithCheckAlreadyContained( item );

				foreach( var check in learningControl.GetComponents<UICheck>( checkChildren: true ) )
				{
					if( check.Name == "Check Done" )
					{
						if( doneList.Contains( check.GetPathFromRoot() ) )
							check.Checked = UICheck.CheckValue.Checked;

						check.Click += Check_Click;
					}
				}

				var tabControl = learningControl.GetComponent<UITabControl>( true );
				tabControl.SelectedIndex = Learning.SelectedPage;
			}
		}

		private void Check_Click( UICheck sender )
		{
			Learning.DoneList.Clear();

			foreach( var check in learningControl.GetComponents<UICheck>( checkChildren: true ) )
			{
				if( check.Name == "Check Done" && check.Checked == UICheck.CheckValue.Checked )
					Learning.DoneList.Add( check.GetPathFromRoot() );
			}

			var tabControl = learningControl.GetComponent<UITabControl>( true );
			Learning.SelectedPage = tabControl.SelectedIndex;

			SaveDocument();
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			Viewport.UIContainer.PerformRenderUI( viewport.CanvasRenderer );
		}
	}
}
