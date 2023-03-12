// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using NeoAxis;

namespace Project
{
	public class MultiplayerCreateWindow : UIWindow
	{
		[EngineConfig( "MultiplayerCreateWindow", "MultiplayerLastPort" )]
		public static int lastPort { get; set; } = 52000;
		[EngineConfig( "MultiplayerCreateWindow", "MultiplayerLastPassword" )]
		public static string lastPassword { get; set; } = "";
		[EngineConfig( "MultiplayerCreateWindow", "MultiplayerLastRendering" )]
		public static bool lastRendering { get; set; } = false;
		[EngineConfig( "MultiplayerCreateWindow", "MultiplayerLastScene" )]
		public static string lastScene { get; set; } = "";

		List<string> scenePaths = new List<string>();

		/////////////////////////////////////////

		UIText GetTextPort() { return GetComponent<UIText>( "Text Port" ); }
		UIEdit GetEditPort() { return GetComponent<UIEdit>( "Edit Port" ); }
		UIText GetTextPassword() { return GetComponent<UIText>( "Text Password" ); }
		UIEdit GetEditPassword() { return GetComponent<UIEdit>( "Edit Password" ); }
		UICheck GetCheckRendering() { return GetComponent<UICheck>( "Check Rendering" ); }
		UIText GetTextScene() { return GetComponent<UIText>( "Text Scene" ); }
		UIList GetListScenes() { return GetComponent<UIList>( "List Scenes" ); }
		UIButton GetButtonStart() { return GetComponent<UIButton>( "Button Start" ); }
		UIButton GetButtonStop() { return GetComponent<UIButton>( "Button Stop" ); }
		UIButton GetButtonChangeScene() { return GetComponent<UIButton>( "Button Change Scene" ); }
		UIText GetTextStatus() { return GetComponent<UIText>( "Text Status" ); }
		//UIButton GetButtonClose() { return GetComponent<UIButton>( "Button Close" ); }

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//register [EngineConfig] fields, properties
			EngineConfig.RegisterClassParameters( typeof( MultiplayerCreateWindow ) );

			GetEditPort().Text = lastPort.ToString();
			GetEditPassword().Text = lastPassword;
			GetCheckRendering().Checked = lastRendering ? UICheck.CheckValue.Checked : UICheck.CheckValue.Unchecked;

			UpdateSceneList();

			//!!!!impl
			GetButtonChangeScene().Enabled = false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				var running = RunServer.Running;

				GetTextPort().ReadOnly = running;
				GetEditPort().ReadOnly = running;
				GetTextPassword().ReadOnly = running;
				GetEditPassword().ReadOnly = running;
				GetCheckRendering().ReadOnly = running;
				GetTextScene().ReadOnly = running;
				GetListScenes().ReadOnly = running;

				GetButtonStart().ReadOnly = running;
				GetButtonStop().ReadOnly = !running;
				GetButtonChangeScene().ReadOnly = !running || string.IsNullOrEmpty( GetScene() );

				GetTextStatus().Text = RunServer.TextStatus;
			}
		}

		public void ButtonClose_Click( NeoAxis.UIButton sender )
		{
			Dispose();
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

		void UpdateSceneList()
		{
			var list = GetListScenes();
			if( list != null )
			{
				scenePaths.Clear();

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
					scenePaths.Add( file );

					string itemText = showOnlyFileNames ? Path.GetFileName( file ) : file;
					list.Items.Add( itemText );

					if( file == lastScene )
						list.SelectedIndex = list.Items.Count - 1;
				}

				if( list.SelectedIndex != 0 )
					list.EnsureVisible( list.SelectedIndex );

				//// Apply saved scroll position of the list control.
				//if( list.SelectedIndex != 0 && list.GetScrollBar() != null )
				//	list.GetScrollBar().Value = savedScrollPosition;
			}
		}

		int GetPort()
		{
			var text = GetEditPort().Text.Value.Trim( ' ', '\t' );
			if( int.TryParse( text, out var result ) )
				return result;
			return 0;
		}

		string GetPassword()
		{
			return GetEditPassword().Text.Value;
		}

		bool GetRendering()
		{
			return GetCheckRendering().Checked.Value == UICheck.CheckValue.Checked;
		}

		string GetScene()
		{
			var list = GetListScenes();
			if( list.SelectedIndex >= 0 && list.SelectedIndex < scenePaths.Count )
				return scenePaths[ list.SelectedIndex ];
			return "";
		}

		public void EditPort_TextChanged( NeoAxis.UIControl obj )
		{
			//lastPort = GetPort();
		}

		public void EditPassword_TextChanged( NeoAxis.UIControl obj )
		{
			//lastPassword = GetPassword();
		}

		public void ButtonStart_Click( NeoAxis.UIButton sender )
		{
			lastPort = GetPort();
			lastPassword = GetPassword();
			lastRendering = GetRendering();
			lastScene = GetScene();

			Log.InvisibleInfo( $"Run server process. Port: {GetPort()}, Password {GetPassword()}, Rendering {GetRendering()}, Scene {GetScene()}" );

			if( !RunServer.Start( GetPort(), GetPassword(), GetRendering(), GetScene(), out var error ) )
			{
				Log.InvisibleInfo( "Run server process failed. " + error );
				ScreenMessages.Add( error, true );
			}
		}

		public void ButtonStop_Click( NeoAxis.UIButton sender )
		{
			RunServer.Stop();
		}

		public void ButtonChangeScene_Click( NeoAxis.UIButton sender )
		{
			//!!!!
		}
	}
}