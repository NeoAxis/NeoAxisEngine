// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using NeoAxis;

namespace Project
{
	public class ScenesWindow : UIWindow
	{
		List<string> fullPaths = new List<string>();

		[EngineConfig( "ScenesWindow", "UnloadResources" )]
		static bool unloadResources = true;

		//static double savedScrollPosition;

		///////////////////////////////////////////////

		UIButton ButtonLoad { get { return GetComponent<UIButton>( "Button Load" ); } }
		UIButton ButtonClose { get { return GetComponent<UIButton>( "Button Close" ); } }
		UICheck CheckUnloadResources { get { return GetComponent<UICheck>( "Check Unload Resources" ); } }

		///////////////////////////////////////////////

		static ScenesWindow()
		{
			EngineConfig.RegisterClassParameters( typeof( ScenesWindow ) );
		}

		protected override void OnEnabledInSimulation()
		{
			fullPaths.Clear();

			if( CheckUnloadResources != null )
				CheckUnloadResources.Checked = unloadResources ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;

			var list = GetComponent<UIList>( "List" );
			if( list != null )
			{
				var files = VirtualDirectory.GetFiles( "", "*.scene", SearchOption.AllDirectories );

				var showOnlyFileNames = SystemSettings.MobileDevice;

				CollectionUtility.MergeSort( files, delegate ( string name1, string name2 )
				{
					var s1 = name1.Replace( "\\", " \\" ).Replace( "/", " /" );
					var s2 = name2.Replace( "\\", " \\" ).Replace( "/", " /" );
					if( showOnlyFileNames )
					{
						s1 = Path.GetFileName( s1 );
						s2 = Path.GetFileName( s2 );
					}
					return string.Compare( s1, s2 );
				} );

				foreach( var file in files )
				{
					fullPaths.Add( file );

					string itemText = showOnlyFileNames ? Path.GetFileName( file ) : file;
					list.AddItem( itemText );

					if( PlayScreen.Instance != null && string.Compare( PlayScreen.Instance.PlayFileName, file, true ) == 0 )
						list.SelectedIndex = list.Items.Count - 1;
				}

				if( list.SelectedIndex != 0 )
					list.EnsureVisible( list.SelectedIndex );

				//// Apply saved scroll position of the list control.
				//if( list.SelectedIndex != 0 && list.GetScrollBar() != null )
				//	list.GetScrollBar().Value = savedScrollPosition;
			}

			list?.Focus();
		}

		protected override void OnDisabledInSimulation()
		{
			//// Save scroll position of the list control.
			//var list = GetComponent<UIList>( "List" );
			//if( list != null && list.GetScrollBar() != null )
			//	savedScrollPosition = list.GetScrollBar().Value;
		}

		public void ButtonClose_Click( UIButton sender )
		{
			Dispose();
		}

		public void ButtonLoad_Click( UIButton sender )
		{
			var list = GetComponent<UIList>( "List" );
			if( list != null && list.SelectedIndex != -1 )
			{
				var playFile = fullPaths[ list.SelectedIndex ];

				var unloadResources = false;
				if( CheckUnloadResources != null )
					unloadResources = CheckUnloadResources.Checked.Value == UICheck.CheckValue.Checked;

				SimulationApp.PlayFile( playFile, unloadResources );
			}
		}

		public void List_ItemMouseDoubleClick( NeoAxis.UIControl sender, NeoAxis.EMouseButtons button, ref bool handled )
		{
			ButtonLoad_Click( null );
		}

		public void List_KeyDown( NeoAxis.UIControl sender, NeoAxis.KeyEvent e, ref bool handled )
		{
			if( e.Key == EKeys.Return )
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

		public void CheckUnloadResources_Click( NeoAxis.UICheck sender )
		{
			unloadResources = sender.Checked.Value == UICheck.CheckValue.Checked;
		}
	}
}