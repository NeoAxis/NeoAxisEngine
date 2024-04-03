// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class EntranceScreen : NeoAxis.UIControl
	{
		AvatarWindow avatarWindow;

		double requestEntranceScreenInfoRemainingTime = 5;

		bool autoEnterTriedEnterToWorld;

		/////////////////////////////////////////

		UIControl GetWindow() { return GetComponent( "Window" ) as UIControl; }
		UIText GetTextWelcome() { return GetWindow().GetComponent( "Text Welcome" ) as UIText; }
		UIText GetTextDescription() { return GetWindow().GetComponent( "Text Description" ) as UIText; }
		UIText GetTextStatus() { return GetWindow().GetComponent( "Text Status" ) as UIText; }
		UIButton GetButtonAvatar() { return GetWindow().GetComponent( "Button Avatar" ) as UIButton; }
		//UIButton GetButtonEnter() { return GetWindow().GetComponent( "Button Enter" ) as UIButton; }

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			var worldName = ProjectSettings.Get.General.ProjectName;
			if( EngineInfo.CloudProjectInfo != null )
				worldName = EngineInfo.CloudProjectInfo.Name;
			GetTextWelcome().Text = GetTextWelcome().Text.Value.Replace( "{name}", worldName );

			UpdateDescriptionAndStatus();

			var clientLogic = GetNetworkLogic();
			if( clientLogic != null )
				GetButtonAvatar().Enabled = clientLogic.AvatarWindow;

			RequestEntranceScreenInfo();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsSimulation )
			{
				UpdateDescriptionAndStatus();

				requestEntranceScreenInfoRemainingTime -= delta;
				if( requestEntranceScreenInfoRemainingTime < 0 )
				{
					requestEntranceScreenInfoRemainingTime = 5;
					RequestEntranceScreenInfo();
				}

				//auto enter
				var clientLogic = GetNetworkLogic();
				if( clientLogic != null )
				{
					if( !autoEnterTriedEnterToWorld && !clientLogic.EntranceScreen )
					{
						autoEnterTriedEnterToWorld = true;
						TryEnterToWorld();
					}
				}
			}
		}

		void AvatarWindowCreate()
		{
			AvatarWindowDestroy();

			var fileName = @"Base\UI\Screens\AvatarWindow.ui";
			if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
			{
				var screen = ResourceManager.LoadSeparateInstance<AvatarWindow>( fileName, false, true );
				if( screen != null )
				{
					avatarWindow = screen;
					AddComponent( avatarWindow );
				}
			}
		}

		void AvatarWindowDestroy()
		{
			avatarWindow?.Dispose();
			avatarWindow = null;
		}

		public void ButtonAvatar_Click( NeoAxis.UIButton sender )
		{
			if( avatarWindow != null && avatarWindow.Parent == null )
				avatarWindow = null;

			if( avatarWindow == null )
				AvatarWindowCreate();
			else
				AvatarWindowDestroy();
		}

		public Scene GetScene()
		{
			return PlayScreen.Instance?.Scene;
		}

		public NetworkLogic GetNetworkLogic()
		{
			var scene = GetScene();
			if( scene != null )
				return NetworkLogicUtility.GetNetworkLogic( scene ) as NetworkLogic;
			return null;
		}

		void TryEnterToWorld()
		{
			var networkLogic = GetNetworkLogic();
			if( networkLogic != null )
			{
				networkLogic.BeginNetworkMessageToServer( "TryEnterToWorld" );
				networkLogic.EndNetworkMessage();
			}
		}

		public void ButtonEnter_Click( NeoAxis.UIButton sender )
		{
			TryEnterToWorld();
		}

		public void ButtonExit_Click( NeoAxis.UIButton sender )
		{
			EngineApp.NeedExit = true;
		}

		void UpdateDescriptionAndStatus()
		{
			var description = "";
			var status = "";

			var clientLogic = GetNetworkLogic();
			if( clientLogic != null && SimulationAppClient.Client != null )
			{
				description = clientLogic.EntranceScreenDescription;

				var players = 0;
				var bots = 0;
				foreach( var user in SimulationAppClient.Client.Users.Users )
				{
					if( user.Bot )
						bots++;
					else
						players++;
				}

				status = $"{players} players online and {bots} bots.";// in {games.Length} games.";
			}

			GetTextDescription().Text = description;
			GetTextStatus().Text = status;
		}

		void RequestEntranceScreenInfo()
		{
			var clientLogic = GetNetworkLogic();
			if( clientLogic != null )
			{
				clientLogic.BeginNetworkMessageToServer( "RequestEntranceScreenInfo" );
				clientLogic.EndNetworkMessage();
			}
		}
	}
}