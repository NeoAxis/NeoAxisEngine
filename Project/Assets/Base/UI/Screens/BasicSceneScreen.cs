// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// An default GUI screen of the scene.
	/// </summary>
	public class BasicSceneScreen : NeoAxis.UIControl
	{
		Scene scene;
		GameMode gameMode;
		NetworkLogic networkLogic;

		EntranceScreen entranceScreen;
		InGameContextScreen inGameContextScreen;

		double continuousInteractionAlpha;
		string continuousInteractionMessageID = "";
		string continuousInteractionMessageText = "";
		double continuousInteractionMessageTime;
		double continuousInteractionButtonsAlpha;

		double disableInteractionRemainingTime1;
		double disableInteractionRemainingTime2;
		bool touchModeActivated;

		object cameraRotationWithTouchDownObject;
		Vector2 cameraRotationWithTouchLastPosition;

		object moveWithTouchDownObject;
		Vector2 moveWithTouchStartPosition;

		///////////////////////////////////////////////

		/// <summary>
		/// Whether to display messages above characters and other objects.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DisplayMessagesAboveObjects
		{
			get { if( _displayMessagesAboveObjects.BeginGet() ) DisplayMessagesAboveObjects = _displayMessagesAboveObjects.Get( this ); return _displayMessagesAboveObjects.value; }
			set { if( _displayMessagesAboveObjects.BeginSet( this, ref value ) ) { try { DisplayMessagesAboveObjectsChanged?.Invoke( this ); } finally { _displayMessagesAboveObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayMessagesAboveObjects"/> property value changes.</summary>
		public event Action<BasicSceneScreen> DisplayMessagesAboveObjectsChanged;
		ReferenceField<bool> _displayMessagesAboveObjects = true;

		/// <summary>
		/// The maximal length of the message above the controlled object by the player.
		/// </summary>
		[DefaultValue( 50 )]
		public Reference<int> DisplayMessagesAboveObjectsMaxLength
		{
			get { if( _displayMessagesAboveObjectsMaxLength.BeginGet() ) DisplayMessagesAboveObjectsMaxLength = _displayMessagesAboveObjectsMaxLength.Get( this ); return _displayMessagesAboveObjectsMaxLength.value; }
			set { if( _displayMessagesAboveObjectsMaxLength.BeginSet( this, ref value ) ) { try { DisplayMessagesAboveObjectsMaxLengthChanged?.Invoke( this ); } finally { _displayMessagesAboveObjectsMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayMessagesAboveObjectsMaxLength"/> property value changes.</summary>
		public event Action<BasicSceneScreen> DisplayMessagesAboveObjectsMaxLengthChanged;
		ReferenceField<int> _displayMessagesAboveObjectsMaxLength = 50;

		/// <summary>
		/// The displaying time of the message above the controlled object by the player.
		/// </summary>
		[DefaultValue( 4.0 )]
		public Reference<double> DisplayMessagesAboveObjectsTime
		{
			get { if( _displayMessagesAboveObjectsTime.BeginGet() ) DisplayMessagesAboveObjectsTime = _displayMessagesAboveObjectsTime.Get( this ); return _displayMessagesAboveObjectsTime.value; }
			set { if( _displayMessagesAboveObjectsTime.BeginSet( this, ref value ) ) { try { DisplayMessagesAboveObjectsTimeChanged?.Invoke( this ); } finally { _displayMessagesAboveObjectsTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayMessagesAboveObjectsTime"/> property value changes.</summary>
		public event Action<BasicSceneScreen> DisplayMessagesAboveObjectsTimeChanged;
		ReferenceField<double> _displayMessagesAboveObjectsTime = 4.0;

		/// <summary>
		/// The visibility distance of the messages above objects.
		/// </summary>
		[DefaultValue( 20.0 )]
		public Reference<double> DisplayMessagesAboveObjectsVisibilityDistance
		{
			get { if( _displayMessagesAboveObjectsVisibilityDistance.BeginGet() ) DisplayMessagesAboveObjectsVisibilityDistance = _displayMessagesAboveObjectsVisibilityDistance.Get( this ); return _displayMessagesAboveObjectsVisibilityDistance.value; }
			set { if( _displayMessagesAboveObjectsVisibilityDistance.BeginSet( this, ref value ) ) { try { DisplayMessagesAboveObjectsVisibilityDistanceChanged?.Invoke( this ); } finally { _displayMessagesAboveObjectsVisibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayMessagesAboveObjectsVisibilityDistance"/> property value changes.</summary>
		public event Action<BasicSceneScreen> DisplayMessagesAboveObjectsVisibilityDistanceChanged;
		ReferenceField<double> _displayMessagesAboveObjectsVisibilityDistance = 20.0;

		///////////////////////////////////////////////

		static BasicSceneScreen()
		{
			EngineConfig.RegisterClassParameters( typeof( BasicSceneScreen ) );
		}

		public static BasicSceneScreen GetInstance()
		{
			return PlayScreen.Instance?.UIControl as BasicSceneScreen;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( DisplayMessagesAboveObjectsMaxLength ):
				case nameof( DisplayMessagesAboveObjectsTime ):
				case nameof( DisplayMessagesAboveObjectsVisibilityDistance ):
					if( !DisplayMessagesAboveObjects )
						skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public Scene Scene
		{
			get { return scene; }
		}

		[Browsable( false )]
		public GameMode GameMode
		{
			get { return gameMode; }
		}

		[Browsable( false )]
		public NetworkLogic NetworkLogic
		{
			get { return networkLogic; }
		}

		[Browsable( false )]
		public EntranceScreen EntranceScreen
		{
			get { return entranceScreen; }
		}

		[Browsable( false )]
		public InGameContextScreen InGameContextScreen
		{
			get { return inGameContextScreen; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EngineApp.IsSimulation && EnabledInHierarchyAndIsInstance )
			{
				scene = ClientUtility.GetScene();
				gameMode = ClientUtility.GetGameMode();
				networkLogic = ClientUtility.GetNetworkLogic();
			}

			if( EngineApp.IsSimulation )
			{
				//scene, game mode
				if( EnabledInHierarchyAndIsInstance )
				{
					if( scene != null && scene.NetworkIsClient )
					{
						if( PlayScreen.Instance != null )
							PlayScreen.Instance.InputEnabledEvent += PlayScreen_InputEnabledEvent;

						if( gameMode != null )
						{
							gameMode.GetInteractiveObjectInfoEvent += GameMode_GetInteractiveObjectInfoEvent;
							gameMode.GetCameraSettingsEvent += GameMode_GetCameraSettingsEvent;
							gameMode.ShowControlledObject += GameMode_ShowControlledObject;
							gameMode.RenderTargetImageBefore += GameMode_RenderTargetImageBefore;
						}
					}
				}
				else
				{
					if( PlayScreen.Instance != null )
						PlayScreen.Instance.InputEnabledEvent -= PlayScreen_InputEnabledEvent;

					if( gameMode != null )
					{
						gameMode.GetInteractiveObjectInfoEvent -= GameMode_GetInteractiveObjectInfoEvent;
						gameMode.GetCameraSettingsEvent -= GameMode_GetCameraSettingsEvent;
						gameMode.ShowControlledObject -= GameMode_ShowControlledObject;
						gameMode.RenderTargetImageBefore -= GameMode_RenderTargetImageBefore;
					}
				}

				//chat
				if( EnabledInHierarchyAndIsInstance )
				{
					if( SimulationAppClient.Client != null )
						SimulationAppClient.Client.Chat.ReceiveText += Chat_ReceiveText;
				}
				else
				{
					if( SimulationAppClient.Client != null )
						SimulationAppClient.Client.Chat.ReceiveText -= Chat_ReceiveText;
				}
			}

			if( EngineApp.IsSimulation && EnabledInHierarchyAndIsInstance )
				ConfigureContinuousInteractionWidget();

			//subscribe/unsubscribe to scene render event
			if( EngineApp.IsSimulation )
			{
				if( scene != null )
				{
					if( EnabledInHierarchyAndIsInstance )
						scene.RenderEvent += Scene_RenderEvent;
					else
						scene.RenderEvent -= Scene_RenderEvent;
				}
			}
		}

		public virtual void TouchControlsEnable( bool enable )
		{
			var controlNames = new string[] { "Forward", "Backward", "Left", "Right", "Jump", "Fire", "Fire 2", "Interact", "Camera" };

			foreach( var controlName in controlNames )
			{
				var control = GetComponent( controlName ) as UIControl;
				if( control != null )
					control.Enabled = enable;
			}
		}

		protected virtual void OnTouchControlsUpdate( float delta )
		{
			//default implementation
			var enable = SystemSettings.MobileDevice && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson );
			TouchControlsEnable( enable );
		}

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			OnTouchControlsUpdate( 0 );
		}

		void CheckRemovedInGameContextScreen()
		{
			if( inGameContextScreen != null && inGameContextScreen.Parent == null )
				InGameContextScreenDestroy();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( scene != null )
			{
				//clear closed in-game screens
				CheckRemovedInGameContextScreen();

				if( scene.NetworkIsClient )
					UpdateEntranceScreen();

				UpdateInventoryWidget();
				UpdateContinuousInteractionWidget( delta );
			}

			if( disableInteractionRemainingTime1 > 0 )
			{
				disableInteractionRemainingTime1 -= delta;
				if( disableInteractionRemainingTime1 < 0 )
					disableInteractionRemainingTime1 = 0;
			}
			if( disableInteractionRemainingTime2 > 0 )
			{
				disableInteractionRemainingTime2 -= delta;
				if( disableInteractionRemainingTime2 < 0 )
					disableInteractionRemainingTime2 = 0;
			}

			if( EngineApp.IsSimulation && gameMode != null )
				OnTouchControlsUpdate( delta );
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			base.OnRenderUI( renderer );

			//display network client connection status
			if( SimulationAppClient.Client != null )
			{
				var color = ColorValue.Zero;
				var text = "";
				if( SimulationAppClient.Client.Status == NetworkStatus.Disconnected )
				{
					color = new ColorValue( 1, 0, 0 );
					text = SimulationAppClient.Client.DisconnectionReason;
				}
				else if( SimulationAppClient.Client.Status == NetworkStatus.Connected )
				{
					if( EngineApp.EngineTime - SimulationAppClient.Client.LastReceivedMessageTime > 3 )
					{
						color = new ColorValue( 1, 1, 0 );
						text = "Doesn't receive messages from the server.";
					}
				}

				if( color != ColorValue.Zero )
				{
					var sizeX = 0.017;
					var sizeY = sizeX * renderer.AspectRatio;
					var rect = new Rectangle( 1.0 - sizeX, 1.0 - sizeY, 1, 1 );

					var texture = ResourceManager.LoadResource<ImageComponent>( @"Base\UI\Images\Circle.png" );
					renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture, color, true );

					if( !string.IsNullOrEmpty( text ) )
						renderer.AddText( text, new Vector2( 1.0 - sizeX * 1.1, 1.0 - renderer.DefaultFontSize * 0.1 ), EHorizontalAlignment.Right, EVerticalAlignment.Bottom, color );
				}
			}
		}

		protected virtual void GameMode_GetInteractiveObjectInfoEvent( GameMode sender, InteractiveObjectInterface obj, ref InteractiveObjectObjectInfo result )
		{
		}

		protected virtual void GameMode_GetCameraSettingsEvent( GameMode sender, Viewport viewport, Camera cameraDefault, ref Viewport.CameraSettingsClass cameraSettings )
		{
		}

		protected virtual void PlayScreen_InputEnabledEvent( PlayScreen sender, ref bool enabled )
		{
			if( inGameContextScreen != null )
				enabled = false;
		}

		protected virtual void GameMode_ShowControlledObject( GameMode sender, Viewport viewport, ref bool show )
		{
			//show = false;
		}

		protected virtual void GameMode_RenderTargetImageBefore( GameMode sender, CanvasRenderer renderer, ref bool show )
		{
			//show = false;
		}

		void UpdateEntranceScreen()
		{
			if( networkLogic != null )
			{
				if( networkLogic.EnteredToWorld )
				{
					if( entranceScreen != null )
						EntranceScreenDestroy();
				}
				else
				{
					if( entranceScreen == null )
						EntranceScreenCreate();
				}
			}
		}

		protected virtual void EntranceScreenCreate()
		{
			EntranceScreenDestroy();

			var fileName = @"Base\UI\Screens\EntranceScreen.ui";
			if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
			{
				var screen = ResourceManager.LoadSeparateInstance<EntranceScreen>( fileName, false, true );
				if( screen != null )
				{
					entranceScreen = screen;
					AddComponent( entranceScreen );
				}
			}
		}

		protected virtual void EntranceScreenDestroy()
		{
			entranceScreen?.Dispose();
			entranceScreen = null;
		}

		protected virtual void InGameContextScreenCreate()// bool focusEditMessage )
		{
			InGameContextScreenDestroy();

			//reset control keys
			PlayScreen.Instance?.ParentContainer.Viewport.KeysAndMouseButtonUpAll();

			{
				var fileName = @"Base\UI\Screens\InGameContextScreen.ui";
				if( !string.IsNullOrEmpty( fileName ) && VirtualFile.Exists( fileName ) )
				{
					var screen = ResourceManager.LoadSeparateInstance<InGameContextScreen>( fileName, false, true );
					if( screen != null )
					{
						inGameContextScreen = screen;
						inGameContextScreen.ColorMultiplier = new ColorValue( 1, 1, 1, 0 );
						AddComponent( inGameContextScreen );
					}
				}
			}

			//focus edit message
			//if( focusEditMessage )
			inGameContextScreen?.GetEditMessage()?.Focus();
		}

		protected virtual void InGameContextScreenDestroy()
		{
			inGameContextScreen?.Dispose();
			inGameContextScreen = null;
		}

		void SwitchActiveItem( int index )
		{
			var objectControlledByPlayer = gameMode?.ObjectControlledByPlayer.Value;
			if( objectControlledByPlayer != null )
			{
				//Character
				{
					var character = objectControlledByPlayer as Character;
					if( character != null )
					{
						//don't do action when active weapon is firing
						var activeWeapon = character.GetActiveWeapon();
						if( activeWeapon == null || !activeWeapon.IsFiringAnyMode() )
						{
							var items = character.GetAllItems();
							if( index < items.Length )
							{
								var item = items[ index ];
								if( item.Enabled )
								{
									if( character.NetworkIsClient )
										character.ItemDeactivateClient( item );
									else
										character.ItemDeactivate( gameMode, item );
								}
								else
								{
									if( character.NetworkIsClient )
										character.ItemActivateClient( item );
									else
										character.ItemActivate( gameMode, item );
								}
							}
						}
					}
				}

				//Character2D
				{
					var character = objectControlledByPlayer as Character2D;
					if( character != null )
					{
						var items = character.GetAllItems();
						if( index < items.Length )
						{
							var item = items[ index ];
							if( item.Enabled )
							{
								if( character.NetworkIsClient )
									character.ItemDeactivateClient( item );
								else
									character.ItemDeactivate( gameMode, item );
							}
							else
							{
								if( character.NetworkIsClient )
									character.ItemActivateClient( item );
								else
									character.ItemActivate( gameMode, item );
							}
						}
					}
				}
			}
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( scene != null && scene.NetworkIsClient && entranceScreen == null )
			{
				//clear closed in-game screens
				CheckRemovedInGameContextScreen();

				if( e.Key == EKeys.F1 )
				{
					if( inGameContextScreen == null )
						InGameContextScreenCreate();
					else
						InGameContextScreenDestroy();
					return true;
				}

				if( e.Key == EKeys.Return )
				{
					if( inGameContextScreen == null )
						InGameContextScreenCreate();
					return true;
				}

				if( e.Key == EKeys.Escape )
				{
					if( inGameContextScreen != null )
					{
						InGameContextScreenDestroy();
						return true;
					}
				}
			}

			//skip interaction fading in
			if( e.Key == EKeys.Space )
			{
				if( scene != null && IsContinuousInteractionEnabled() )
				{
					if( continuousInteractionMessageTime < 10000 )
					{
						continuousInteractionMessageTime = 10000;
						continuousInteractionButtonsAlpha = 1;
						return true;
					}
				}
			}

			//switch active item
			if( scene != null && gameMode != null && e.Key >= EKeys.D1 && e.Key <= EKeys.D8 )
			{
				var index = e.Key - EKeys.D1;

				SwitchActiveItem( index );

				//var objectControlledByPlayer = gameMode?.ObjectControlledByPlayer.Value;
				//if( objectControlledByPlayer != null )
				//{
				//	//Character
				//	{
				//		var character = objectControlledByPlayer as Character;
				//		if( character != null )
				//		{
				//			//don't do action when active weapon is firing
				//			var activeWeapon = character.GetActiveWeapon();
				//			if( activeWeapon == null || !activeWeapon.IsFiringAnyMode() )
				//			{
				//				var items = character.GetAllItems();
				//				if( index < items.Length )
				//				{
				//					var item = items[ index ];
				//					if( item.Enabled )
				//					{
				//						if( character.NetworkIsClient )
				//							character.ItemDeactivateClient( item );
				//						else
				//							character.ItemDeactivate( gameMode, item );
				//					}
				//					else
				//					{
				//						if( character.NetworkIsClient )
				//							character.ItemActivateClient( item );
				//						else
				//							character.ItemActivate( gameMode, item );
				//					}
				//				}
				//			}
				//		}
				//	}

				//	//Character2D
				//	{
				//		var character = objectControlledByPlayer as Character2D;
				//		if( character != null )
				//		{
				//			var items = character.GetAllItems();
				//			if( index < items.Length )
				//			{
				//				var item = items[ index ];
				//				if( item.Enabled )
				//				{
				//					if( character.NetworkIsClient )
				//						character.ItemDeactivateClient( item );
				//					else
				//						character.ItemDeactivate( gameMode, item );
				//				}
				//				else
				//				{
				//					if( character.NetworkIsClient )
				//						character.ItemActivateClient( item );
				//					else
				//						character.ItemActivate( gameMode, item );
				//				}
				//			}
				//		}
				//	}
				//}
			}

			return base.OnKeyDown( e );
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			//skip interaction fading in
			if( scene != null && IsContinuousInteractionEnabled() )
			{
				if( continuousInteractionMessageTime < 10000 )
				{
					continuousInteractionMessageTime = 10000;
					continuousInteractionButtonsAlpha = 1;
					return true;
				}
			}

			return base.OnMouseDown( button );
		}

		protected virtual void Chat_ReceiveText( ClientNetworkService_Chat sender, ClientNetworkService_Users.UserInfo fromUser, string text )
		{
			var str = $"{fromUser.Username}: {text}";
			ScreenMessages.Add( str, false );
		}

		void UpdateInventoryWidget()
		{
			var widget = GetComponent( "Inventory Widget" );
			if( widget != null )
			{
				//enable/disable the inventory widget

				var objectControlledByPlayer = gameMode?.ObjectControlledByPlayer.Value;

				var enable = false;
				if( gameMode != null && gameMode.InventoryWidget && objectControlledByPlayer != null && !gameMode.FreeCamera && !gameMode.CutsceneStarted )
				{
					var character = objectControlledByPlayer as Character;
					if( character != null && character.TypeCached.AllowManageInventory )
						enable = true;
					if( objectControlledByPlayer as Character2D != null )
						enable = true;
				}

				widget.Enabled = enable;

				//widget.Enabled = gameMode != null && gameMode.InventoryWidget && gameMode.ObjectControlledByPlayer.Value != null && !gameMode.FreeCamera && !gameMode.CutsceneStarted && ( gameMode.ObjectControlledByPlayer.Value as Character != null || gameMode.ObjectControlledByPlayer.Value as Character2D != null );

				// && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null );

				if( widget.Enabled )
				{
					//get the list of items
					var items = new List<ItemInterface>( 8 );
					if( objectControlledByPlayer != null )
					{
						foreach( var c in objectControlledByPlayer.GetComponents<ItemInterface>() )
							items.Add( c );
					}

					//update the inventory widget
					for( int n = 0; ; n++ )
					{
						var itemControl = widget.GetComponent( "Item " + ( n + 1 ).ToString() ) as UIImage;
						if( itemControl == null )
							break;

						var item = n < items.Count ? items[ n ] : null;
						var itemAsComponent = (NeoAxis.Component)item;

						ImageComponent image = null;
						if( item != null )
							item.GetInventoryImage( out image, out _ );

						itemControl.SourceImage = image;

						var textNumberControl = itemControl.GetComponent<UIText>( "Text Number" );
						if( textNumberControl != null )
						{
							textNumberControl.Visible = item != null;
							if( item != null )
								textNumberControl.Color = itemAsComponent.Enabled ? new ColorValue( 0, 1, 0 ) : new ColorValue( 1, 1, 1 );
						}

						var textCountControl = itemControl.GetComponent<UIText>( "Text Count" );
						if( textCountControl != null )
							textCountControl.Text = ( item != null && item.ItemCount != 1 ) ? item.ItemCount.ToString() : "";
					}
				}
			}
		}

		ContinuousInteraction FindContinuousInteraction()
		{
			if( gameMode != null )
			{
				var objectControlledByPlayer = gameMode.ObjectControlledByPlayer.Value;
				if( objectControlledByPlayer != null )
				{
					foreach( var interaction in gameMode.GetComponents<ContinuousInteraction>() )
					{
						if( interaction.SecondParticipant.Value == objectControlledByPlayer )
							return interaction;
					}
				}
			}
			return null;
		}

		public bool IsContinuousInteractionEnabled()
		{
			return FindContinuousInteraction() != null;
		}

		public UIControl GetContinuousInteractionWidget()
		{
			return GetComponent<UIControl>( "Continuous Interaction Widget" );
		}

		void ConfigureContinuousInteractionWidget()
		{
			var widget = GetContinuousInteractionWidget();
			if( widget != null )
			{
				for( int answer = 1; ; answer++ )
				{
					var buttonAnswer = widget.GetComponent<UIButton>( $"Button Answer {answer}" );
					if( buttonAnswer == null )
						break;

					buttonAnswer.Click += ButtonAnswer_Click;
				}
			}
		}

		private void ButtonAnswer_Click( UIButton sender )
		{
			string answerNumber = new string( sender.Name.Where( char.IsDigit ).ToArray() );

			var widget = GetContinuousInteractionWidget();
			if( widget != null && gameMode != null )
			{
				var interaction = FindContinuousInteraction();
				if( interaction != null )
				{
					var block = new TextBlock();
					block.SetAttribute( "MessageID", continuousInteractionMessageID );
					block.SetAttribute( "Answer", answerNumber.ToString() );

					interaction.MessageFromParticipant( block.DumpToString() );
				}
			}
		}

		bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>( true, true ) != null;
		}

		protected virtual void UpdateContinuousInteractionWidget( float delta )
		{
			var widget = GetContinuousInteractionWidget();
			if( widget != null && gameMode != null )
			{
				var interaction = FindContinuousInteraction();

				//transparency animation
				if( interaction != null )
				{
					continuousInteractionAlpha += delta;
					if( continuousInteractionAlpha > 1 )
						continuousInteractionAlpha = 1;
				}
				else
				{
					continuousInteractionAlpha -= delta;
					if( continuousInteractionAlpha < 0 )
						continuousInteractionAlpha = 0;
				}

				continuousInteractionMessageTime += delta;

				widget.Enabled = continuousInteractionAlpha != 0;
				widget.ColorMultiplier = new ColorValue( 1, 1, 1, continuousInteractionAlpha );

				//update message and buttons text
				if( interaction != null )
				{
					var block = TextBlock.Parse( interaction.CurrentMessageFromCreator, out _ );
					if( block == null )
						block = new TextBlock();

					var messageID = block.GetAttribute( "MessageID" );
					if( continuousInteractionMessageID != messageID )
					{
						continuousInteractionMessageTime = -2.0;//pause before showing message in seconds
						continuousInteractionMessageID = messageID;
						continuousInteractionMessageText = block.GetAttribute( "Message" );

						for( int answer = 1; ; answer++ )
						{
							var buttonAnswer = widget.GetComponent<UIControl>( $"Button Answer {answer}" );
							if( buttonAnswer == null )
								break;

							var text = block.GetAttribute( $"Answer {answer}" );
							buttonAnswer.Text = text;
							buttonAnswer.Enabled = !string.IsNullOrEmpty( text );
						}
					}
				}

				var allMessageTextOnScreen = false;

				//update message text
				{
					const int charactersPerSecond = 20;

					var maxCharacters = (int)( continuousInteractionMessageTime * charactersPerSecond );
					if( interaction == null )
						maxCharacters = 1000000;
					if( maxCharacters < 0 )
						maxCharacters = 0;
					if( maxCharacters > continuousInteractionMessageText.Length )
					{
						maxCharacters = continuousInteractionMessageText.Length;
						allMessageTextOnScreen = true;
					}

					var textMessage = widget.GetComponent<UIControl>( "Text Message" );
					if( textMessage != null )
						textMessage.Text = continuousInteractionMessageText.Substring( 0, maxCharacters );
				}

				//update buttons transparency
				if( allMessageTextOnScreen )
				{
					continuousInteractionButtonsAlpha += delta;
					if( continuousInteractionButtonsAlpha > 1 )
						continuousInteractionButtonsAlpha = 1;
				}
				else
					continuousInteractionButtonsAlpha = 0;

				//update buttons ReadOnly, Visible
				for( int answer = 1; ; answer++ )
				{
					var buttonAnswer = widget.GetComponent<UIControl>( $"Button Answer {answer}" );
					if( buttonAnswer == null )
						break;
					buttonAnswer.Visible = interaction != null;
					buttonAnswer.ColorMultiplier = new ColorValue( 1, 1, 1, continuousInteractionButtonsAlpha );
					buttonAnswer.ReadOnly = interaction == null || continuousInteractionButtonsAlpha == 0 || IsAnyWindowOpened();
				}
			}
		}

		///////////////////////////////////////////////

		class MessageToShow
		{
			public string Text;
			public Vector3 Position;
			public double Alpha;

			public double DistanceSquared;
		}

		protected virtual void Scene_RenderEvent( Scene scene, Viewport viewport )
		{
			var renderer = viewport.CanvasRenderer;

			if( DisplayMessagesAboveObjects )
			{
				var networkLogic = NetworkLogicUtility.GetNetworkLogic( scene );
				var userService = SimulationAppClient.Client?.Users;
				var chatService = SimulationAppClient.Client?.Chat;

				var position = viewport.CameraSettings.Position;
				var radius = DisplayMessagesAboveObjectsVisibilityDistance.Value;
				var sphere = new Sphere( position, radius );

				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
				scene.GetObjectsInSpace( item );

				var messagesToShow = new List<MessageToShow>();

				for( int n = 0; n < item.Result.Length; n++ )
				{
					ref var itemResult = ref item.Result[ n ];
					var obj = itemResult.Object;

					if( obj is Character || obj is Character2D || obj is Vehicle )
					{
						var message = "";

						//get chat message
						if( networkLogic != null && chatService != null )
						{
							var referenceToObject = "root:" + obj.GetPathFromRoot();
							var user = userService.GetUserByObjectControlledByPlayer( referenceToObject );
							if( user != null )
							{
								var lastMessage = chatService.GetLastMessageFromUser( user );
								if( lastMessage != null && EngineApp.EngineTime - lastMessage.Time < DisplayMessagesAboveObjectsTime )
									message = lastMessage.Text;
							}
						}

						//get PermanentMessage from AI
						if( string.IsNullOrEmpty( message ) )
						{
							var ai = obj.GetComponent<AI>();
							if( ai != null )
								message = ai.PermanentMessage.Value;
						}

						if( !string.IsNullOrEmpty( message ) )
						{
							if( message.Length > DisplayMessagesAboveObjectsMaxLength )
								message = message.Substring( 0, DisplayMessagesAboveObjectsMaxLength ) + "...";

							var tr = obj.TransformV;
							var distance = ( position - tr.Position ).Length();
							if( distance < radius ) //if( ( position - tr.Position ).LengthSquared() < radius * radius )
							{
								Vector3 pos = new Vector3( tr.Position.X, tr.Position.Y, obj.SpaceBounds.BoundingBox.Maximum.Z );
								if( obj is Character || obj is Vehicle )
								{
									//!!!!offset
									pos.Z += 0.5;
								}
								else if( obj is Character2D )
								{
									pos = new Vector3( tr.Position.X, obj.SpaceBounds.BoundingBox.Maximum.Y, 0 );
									//!!!!offset
									pos.Y += 0.5;
								}

								var messageToShow = new MessageToShow() { Text = message, Position = pos };
								messageToShow.DistanceSquared = ( position - pos ).LengthSquared();

								//fade by distance
								var startFading = radius * 0.9;
								var div = radius - startFading;
								if( div == 0 )
									div = 0.00001;
								messageToShow.Alpha = 1.0 - MathEx.Saturate( ( distance - startFading ) / div );

								messagesToShow.Add( messageToShow );
							}
						}
					}
				}

				CollectionUtility.MergeSort( messagesToShow, delegate ( MessageToShow item1, MessageToShow item2 )
				{
					if( item1.DistanceSquared < item2.DistanceSquared )
						return -1;
					if( item1.DistanceSquared > item2.DistanceSquared )
						return 1;
					return 0;
				} );

				var text2D = new Text2DFunctionality();

				foreach( var messageItem in messagesToShow )
				{
					if( viewport.CameraSettings.ProjectToScreenCoordinates( messageItem.Position, out var screenPosition ) )
					{
						text2D.BackColor = new ColorValue( 0, 0.65, 1, messageItem.Alpha );
						text2D.Color = new ColorValue( 1, 1, 1, messageItem.Alpha );
						text2D.Text = messageItem.Text;
						text2D.Render( viewport.RenderingContext, screenPosition );
					}
				}
			}
		}

		///////////////////////////////////////////////

		InputProcessing GetCharacterInputProcessing()
		{
			var controlledObject = gameMode?.ObjectControlledByPlayer.Value;
			if( controlledObject != null )
				return controlledObject.GetComponent<InputProcessing>();
			return null;
		}

		bool IsPointInsideControl( string controlName, Vector2 screenPosition )
		{
			var control = Components.GetByPath( controlName ) as UIControl;//var control = GetComponent( controlName ) as UIControl;
			if( control != null )
				return control.GetScreenRectangle().Contains( screenPosition );
			return false;
		}

		bool IsControlTouched( InputProcessing inputProcessing, string controlName )
		{
			foreach( var pointer in inputProcessing.TouchPointers )
			{
				if( IsPointInsideControl( controlName, pointer.Position ) )
					return true;
			}
			return false;
		}

		void TouchProcessLeftRight( GameMode gameMode, InputProcessing inputProcessing )
		{
			var leftPushed = IsControlTouched( inputProcessing, "Left" );
			var rightPushed = IsControlTouched( inputProcessing, "Right" );

			if( leftPushed )
				gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Left ) );
			else
				gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.Left ) );

			if( rightPushed )
				gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Right ) );
			else
				gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.Right ) );
		}

		void ProcessJumpOnTouch( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Jump", e.Position ) )
				gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Space ) );
		}

		void ProcessJumpSimulationStep( GameMode gameMode, InputProcessing inputProcessing )
		{
			if( IsControlTouched( inputProcessing, "Jump" ) )
				gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Space ) );
			else
				gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.Space ) );
		}

		void TouchProcessFireAndInteract( GameMode gameMode, InputProcessing inputProcessing )
		{
			var objectControlledByPlayer = gameMode?.ObjectControlledByPlayer.Value;
			var initiator = objectControlledByPlayer;

			if( disableInteractionRemainingTime1 == 0 )
			{
				var pushed = IsControlTouched( inputProcessing, "Fire" );

				//interact via interaction context
				{
					//get an object to interaction
					var interactionContext = gameMode.ObjectInteractionContext;
					if( interactionContext != null )
					{
						if( pushed )
						{
							var message = new InputMessageMouseButtonDown( EMouseButtons.Left );
							if( interactionContext.Obj.InteractionInputMessage( gameMode, initiator, message ) )
							{
								//temporary disable ProcessFireAndInteract execution to prevent fire of just taken weapon
								disableInteractionRemainingTime1 = 0.25;
							}
						}
						else
						{
							var message = new InputMessageMouseButtonUp( EMouseButtons.Left );
							interactionContext.Obj.InteractionInputMessage( gameMode, initiator, message );
						}
					}
				}

				//fire
				if( disableInteractionRemainingTime1 == 0 )
				{
					if( pushed )
						gameMode.ProcessInputMessage( new InputMessageMouseButtonDown( EMouseButtons.Left ) );
					else
						gameMode.ProcessInputMessage( new InputMessageMouseButtonUp( EMouseButtons.Left ) );
				}
			}

			if( disableInteractionRemainingTime2 == 0 )
			{
				var pushed = IsControlTouched( inputProcessing, "Fire 2" );

				//interact via interaction context
				{
					//get an object to interaction
					var interactionContext = gameMode.ObjectInteractionContext;
					if( interactionContext != null )
					{
						if( pushed )
						{
							var message = new InputMessageMouseButtonDown( EMouseButtons.Right );
							if( interactionContext.Obj.InteractionInputMessage( gameMode, initiator, message ) )
							{
								//temporary disable ProcessFireAndInteract execution to prevent fire of just taken weapon
								disableInteractionRemainingTime2 = 0.25;
							}
						}
						else
						{
							var message = new InputMessageMouseButtonUp( EMouseButtons.Right );
							interactionContext.Obj.InteractionInputMessage( gameMode, initiator, message );
						}
					}
				}

				//fire
				if( disableInteractionRemainingTime2 == 0 )
				{
					if( pushed )
						gameMode.ProcessInputMessage( new InputMessageMouseButtonDown( EMouseButtons.Right ) );
					else
						gameMode.ProcessInputMessage( new InputMessageMouseButtonUp( EMouseButtons.Right ) );
				}
			}
		}

		void ProcessInteractOnTouch( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Interact", e.Position ) )
				gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.E ) );
			else
				gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.E ) );
		}

		//void ProcessAutoTake()
		//{
		//	var inputProcessing = GetInputProcessing();

		//	if (gameMode != null && inputProcessing != null)
		//	{
		//		//get an object to interaction
		//		var interactionContext = gameMode.ObjectInteractionContext;
		//		if (interactionContext != null)
		//		{
		//			//call input message to the object in context
		//			var message = new InputMessageMouseButtonDown(EMouseButtons.Left);
		//			interactionContext.Obj.ObjectInteractionInputMessage(gameMode, message);
		//		}
		//	}
		//}

		void TouchProcessInventoryWidget( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Inventory Widget", e.Position ) )
			{
				for( int nItem = 1; ; nItem++ )
				{
					var namePath = $"Inventory Widget\\Item {nItem}";

					var itemControl = Components.GetByPath( namePath ) as UIControl;
					if( itemControl != null )
					{
						if( IsPointInsideControl( namePath, e.Position ) )
						{
							//sense to use touch down request in scene screen?
							var item = new TouchData.TouchDownRequestToProcessTouch( itemControl, 0, 0, nItem,
								delegate ( UIControl sender, TouchData touchData, object anyData )
								{
									var nItem2 = (int)anyData;
									SwitchActiveItem( nItem2 - 1 );
								} );
							e.TouchDownRequestToControlActions.Add( item );

							//SwitchActiveItem( nItem - 1 );
						}
					}
					else
						break;
				}
			}
		}

		void TouchCameraType( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Camera", e.Position ) )
				gameMode.ChangeCameraType();
		}

		void ProcessCameraRotationWithTouch( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			if( !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson ) && gameMode.Scene.Mode.Value == Scene.ModeEnum._3D )
			{
				switch( e.Action )
				{
				case TouchData.ActionEnum.Down:
					if( e.Position.X > 0.5 )
					{
						cameraRotationWithTouchDownObject = null;

						var item = new TouchData.TouchDownRequestToProcessTouch( this, -10, 0, null,
							delegate ( UIControl sender, TouchData touchData, object anyData )
							{
								//start touch
								cameraRotationWithTouchDownObject = e.PointerIdentifier;
								cameraRotationWithTouchLastPosition = e.Position;
							} );
						e.TouchDownRequestToControlActions.Add( item );
					}
					break;

				case TouchData.ActionEnum.Up:
					if( cameraRotationWithTouchDownObject != null && ReferenceEquals( e.PointerIdentifier, cameraRotationWithTouchDownObject ) )
						cameraRotationWithTouchDownObject = null;
					break;

				case TouchData.ActionEnum.Move:
					if( cameraRotationWithTouchDownObject != null && ReferenceEquals( e.PointerIdentifier, cameraRotationWithTouchDownObject ) )
					{
						var diff = e.Position - cameraRotationWithTouchLastPosition;
						diff.X *= ParentContainer.AspectRatio;

						cameraRotationWithTouchLastPosition = e.Position;

						//update camera

						if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
						{
							//!!!!refactor
							//!!!!character specific

							var characterInputProcessing = inputProcessing as CharacterInputProcessing;
							if( characterInputProcessing != null )
							{
								var character = characterInputProcessing.Character;
								if( character != null && !character.Sitting )
								{
									var sensitivity = GameMode.MouseSensitivity * 3;

									var mouseOffset = new Vector2( diff.X, diff.Y ) * sensitivity;
									characterInputProcessing.UpdateTurnToDirectionAndLookToToPosition( gameMode, mouseOffset.ToVector2F() );
								}
							}
						}

						if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
						{
							var sensitivity = GameMode.MouseSensitivity * 5;

							var h = gameMode.ThirdPersonCameraHorizontalAngle.Value - new Radian( diff.X ).InDegrees() * sensitivity;
							if( h < 0 ) h += 360;
							if( h > 360 ) h -= 360;

							var v = gameMode.ThirdPersonCameraVerticalAngle.Value - new Radian( diff.Y ).InDegrees() * sensitivity;
							v = MathEx.Clamp( (double)v, -80, 80 );

							gameMode.ThirdPersonCameraHorizontalAngle = h;
							gameMode.ThirdPersonCameraVerticalAngle = v;
						}
					}
					break;
				}
			}
		}

		void ProcessMoveWithTouch( GameMode gameMode, InputProcessing inputProcessing, TouchData e )
		{
			var allow = !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson );

			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( allow && e.Position.X < 0.5 )
				{
					moveWithTouchDownObject = null;

					var item = new TouchData.TouchDownRequestToProcessTouch( this, -10, 0, null,
						delegate ( UIControl sender, TouchData touchData, object anyData )
						{
							//start touch
							moveWithTouchDownObject = e.PointerIdentifier;
							moveWithTouchStartPosition = e.Position;
						} );
					e.TouchDownRequestToControlActions.Add( item );
				}
				break;

			case TouchData.ActionEnum.Up:
				if( moveWithTouchDownObject != null && ReferenceEquals( e.PointerIdentifier, moveWithTouchDownObject ) )
				{
					moveWithTouchDownObject = null;
					gameMode.ProcessInputMessage( new InputMessageTouchSliderChanged( 0, Vector2.Zero ) );
				}
				break;

			case TouchData.ActionEnum.Move:
				if( moveWithTouchDownObject != null && ReferenceEquals( e.PointerIdentifier, moveWithTouchDownObject ) )
				{
					var diff = e.Position - moveWithTouchStartPosition;
					diff.X *= ParentContainer.AspectRatio;

					var sensitivity = GameMode.MouseSensitivity * 10;

					var value = diff * sensitivity;
					gameMode.ProcessInputMessage( new InputMessageTouchSliderChanged( 0, value ) );
				}
				break;
			}
		}

		UIControl GetControlOverPosition( Vector2 position )
		{
			foreach( var child in GetComponents<UIControl>( onlyEnabledInHierarchy: true ) )
			{
				child.GetScreenRectangle( out var r );
				if( r.Contains( ref position ) )
					return child;
			}
			return null;
		}

		protected override bool OnTouch( TouchData e )
		{
			touchModeActivated = true;

			if( !IsAnyWindowOpened() && touchModeActivated )
			{
				var inputProcessing = GetCharacterInputProcessing();
				if( gameMode != null && inputProcessing != null )
				{
					TouchProcessLeftRight( gameMode, inputProcessing );
					ProcessJumpOnTouch( gameMode, inputProcessing, e );
					TouchProcessFireAndInteract( gameMode, inputProcessing );
					ProcessInteractOnTouch( gameMode, inputProcessing, e );
					TouchProcessInventoryWidget( gameMode, inputProcessing, e );
					TouchCameraType( gameMode, inputProcessing, e );

					//camera rotation with touch, move with touch
					UIControl overControl = null;
					if( e.Action == TouchData.ActionEnum.Down )
						overControl = GetControlOverPosition( e.Position );
					if( overControl == null || e.Action == TouchData.ActionEnum.Up || e.Action == TouchData.ActionEnum.Move )
					{
						ProcessCameraRotationWithTouch( gameMode, inputProcessing, e );
						ProcessMoveWithTouch( gameMode, inputProcessing, e );
					}
				}
			}

			return base.OnTouch( e );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( !IsAnyWindowOpened() && touchModeActivated )
			{
				var inputProcessing = GetCharacterInputProcessing();
				if( gameMode != null && inputProcessing != null )
				{
					TouchProcessLeftRight( gameMode, inputProcessing );
					ProcessJumpSimulationStep( gameMode, inputProcessing );
					TouchProcessFireAndInteract( gameMode, inputProcessing );

					//if (SystemSettings.MobileDevice)
					//	ProcessAutoTake();
				}
			}
		}
	}
}