using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis;

namespace Project
{
	public class ScenesWindow : UIWindow
	{
		//static double savedScrollPosition;

		//

		protected override void OnEnabledInSimulation()
		{
			if( Components[ "Button Load" ] != null )
				( (UIButton)Components[ "Button Load" ] ).Click += ButtonLoad_Click;
			if( Components[ "Button Close" ] != null )
				( (UIButton)Components[ "Button Close" ] ).Click += ButtonClose_Click;

			var list = GetComponent<UIList>( "List" );
			if( list != null )
			{
				var files = VirtualDirectory.GetFiles( "", "*.scene", SearchOption.AllDirectories );
				foreach( var file in files )
				{
					list.Items.Add( file );

					if( PlayScreen.Instance != null && string.Compare( PlayScreen.Instance.PlayFileName, file, true ) == 0 )
						list.SelectedIndex = list.Items.Count - 1;
				}

				list.ItemMouseDoubleClick += List_ItemMouseDoubleClick;

				if( list.SelectedIndex != 0 )
					list.EnsureVisible( list.SelectedIndex );

				//// Apply saved scroll position of the list control.
				//if( list.SelectedIndex != 0 && list.GetScrollBar() != null )
				//	list.GetScrollBar().Value = savedScrollPosition;
			}
		}

		protected override void OnDisabledInSimulation()
		{
			//// Save scroll position of the list control.
			//var list = GetComponent<UIList>( "List" );
			//if( list != null && list.GetScrollBar() != null )
			//	savedScrollPosition = list.GetScrollBar().Value;
		}

		void ButtonClose_Click( UIButton sender )
		{
			Dispose();
		}

		void ButtonLoad_Click( UIButton sender )
		{
			var list = GetComponent<UIList>( "List" );
			if( list != null && list.SelectedItem != null )
			{
				var playFile = list.SelectedItem;
				SimulationApp.PlayFile( playFile );
			}
		}

		private void List_ItemMouseDoubleClick( UIControl sender, EMouseButtons button, int index, ref bool handled )
		{
			ButtonLoad_Click( null );
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				Dispose();
				return true;
			}

			return base.OnKeyDown( e );
		}
	}
}