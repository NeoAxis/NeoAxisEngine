// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Cloud;
using NeoAxis.Networking;

namespace Project
{
	/// <summary>
	/// An default GUI screen of the scene.
	/// </summary>
	public class BasicSceneScreen : NeoAxis.UIControl
	{
		//match info
		public static Matches.Match MatchInfo { get; set; }

		Scene scene;
		GameMode gameMode;
		GameLogic gameLogic;

		//continuous interaction
		double continuousInteractionAlpha;
		string continuousInteractionMessageID = "";
		string continuousInteractionMessageText = "";
		double continuousInteractionMessageTime;
		double continuousInteractionButtonsAlpha;

		//touching interaction
		double disableInteractionRemainingTime1;
		double disableInteractionRemainingTime2;
		bool touchModeActivated;
		object cameraRotationWithTouchDownObject;
		Vector2 cameraRotationWithTouchLastPosition;
		object moveWithTouchDownObject;
		Vector2 moveWithTouchStartPosition;

		//chat
		volatile bool chatNewMessagesAvailable = true;
		volatile bool chatGettingNewMessages;
		const double chatMessagesOnScreenTime = 10;
		Queue<(string Username, string Text, DateTime Time)> chatMessagesOnScreen = new Queue<(string Username, string Text, DateTime Time)>();

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

		/// <summary>
		/// Whether to automatically hide the in-game menu when a chat message is sent.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> InGameMenuAutoHideWhenSentChatMessage
		{
			get { if( _inGameMenuAutoHideWhenSentChatMessage.BeginGet() ) InGameMenuAutoHideWhenSentChatMessage = _inGameMenuAutoHideWhenSentChatMessage.Get( this ); return _inGameMenuAutoHideWhenSentChatMessage.value; }
			set { if( _inGameMenuAutoHideWhenSentChatMessage.BeginSet( this, ref value ) ) { try { InGameMenuAutoHideWhenSentChatMessageChanged?.Invoke( this ); } finally { _inGameMenuAutoHideWhenSentChatMessage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InGameMenuAutoHideWhenSentChatMessage"/> property value changes.</summary>
		public event Action<BasicSceneScreen> InGameMenuAutoHideWhenSentChatMessageChanged;
		ReferenceField<bool> _inGameMenuAutoHideWhenSentChatMessage = true;

		/// <summary>
		/// Whether to always hide the in-game menu. If true, the in-game menu will not be displayed even if the F1 key is pressed.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> InGameMenuAlwaysHide
		{
			get { if( _inGameMenuAlwaysHide.BeginGet() ) InGameMenuAlwaysHide = _inGameMenuAlwaysHide.Get( this ); return _inGameMenuAlwaysHide.value; }
			set
			{
				if( _inGameMenuAlwaysHide.BeginSet( this, ref value ) )
				{
					try
					{
						InGameMenuAlwaysHideChanged?.Invoke( this );

						if( InGameMenuAlwaysHide && EngineApp.IsSimulation )
							InGameMenuShow( false );
					}
					finally { _inGameMenuAlwaysHide.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="InGameMenuAlwaysHide"/> property value changes.</summary>
		public event Action<BasicSceneScreen> InGameMenuAlwaysHideChanged;
		ReferenceField<bool> _inGameMenuAlwaysHide = false;

		///////////////////////////////////////////////

		//controls to open menus
		[Browsable( false )]
		public UIButton ButtonInGameMenu { get { return GetComponent<UIButton>( "Button In-Game Menu" ); } }
		[Browsable( false )]
		public UIButton ButtonSystemMenu { get { return GetComponent<UIButton>( "Button System Menu" ); } }
		//[Browsable( false )]
		//public UIText TextShowInGameMenu { get { return GetComponent<UIText>( "Text Show In-Game Menu" ); } }

		//In-game menu controls
		[Browsable( false )]
		public UIList ListChat { get { return GetInGameMenu()?.GetComponent<UIList>( "List Chat" ); } }
		[Browsable( false )]
		public UIEdit EditChatMessage { get { return GetInGameMenu()?.GetComponent<UIEdit>( "Edit Chat Message" ); } }
		[Browsable( false )]
		public UIButton ButtonChatSend { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Chat Send" ); } }
		[Browsable( false )]
		public UIButton ButtonMatchReset { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Match Reset" ); } }
		[Browsable( false )]
		public UIButton ButtonMatchDelete { get { return GetInGameMenu()?.GetComponent<UIButton>( "Button Match Delete" ); } }

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
		public GameLogic GameLogic
		{
			get { return gameLogic; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EngineApp.IsSimulation )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene = PlayScreen.Instance?.Scene;
					if( scene != null )
					{
						gameMode = scene.GetGameMode();
						gameLogic = scene.GetGameLogic();
					}
				}

				//scene, game mode
				if( EnabledInHierarchyAndIsInstance )
				{
					if( scene != null )
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

				//multiplayer mode chat
				if( SimulationAppClient.ConnectionNode?.Chat != null )
				{
					if( EnabledInHierarchyAndIsInstance )
						SimulationAppClient.ConnectionNode.Chat.ReceivedRoomMessage += Chat_ReceivedRoomMessage;
					else
						SimulationAppClient.ConnectionNode.Chat.ReceivedRoomMessage -= Chat_ReceivedRoomMessage;
				}

				//cloud service messages
				{
					var client = CloudServiceClient.Client;
					if( client != null )
					{
						if( EnabledInHierarchyAndIsInstance )
						{
							client.ConnectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
							client.ConnectionNode.Messages.ReceiveMessageBinary += Messages_ReceiveMessageBinary;
						}
						else
						{
							client.ConnectionNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;
							client.ConnectionNode.Messages.ReceiveMessageBinary -= Messages_ReceiveMessageBinary;
						}
					}
				}

				//subscribe/unsubscribe to scene render event
				if( scene != null )
				{
					if( EnabledInHierarchyAndIsInstance )
					{
						scene.RenderEvent += Scene_RenderEvent;
						scene.SimulationStep += Scene_SimulationStep;
					}
					else
					{
						scene.RenderEvent -= Scene_RenderEvent;
						scene.SimulationStep -= Scene_SimulationStep;
					}
				}

				//initial configuration of the controls
				if( EnabledInHierarchyAndIsInstance )
				{
					ConfigureContinuousInteractionWidget();

					//disable in-game menu at the start
					InGameMenuShow( false );

					OnTouchControlsUpdate( 0 );

					////disable free camera when debugger is not attached
					//if( !System.Diagnostics.Debugger.IsAttached )
					//{
					//	GameMode.FreeCamera = false;
					//	GameMode.FreeCameraKey = EKeys.None;
					//}

					//load chat messages from the default room
					{
						var defaultRoom = SimulationAppClient.ConnectionNode?.Chat?.GetRoom( "Default" );
						if( defaultRoom != null )
						{
							foreach( var message in defaultRoom.Messages )
								ChatAddMesageToList( message );
						}
					}

					//disable chat controls when no network connection or no chat service
					if( !IsChatEnabled() )
					{
						if( ListChat != null )
							ListChat.ReadOnly = true;
						if( EditChatMessage != null )
							EditChatMessage.ReadOnly = true;
						if( ButtonChatSend != null )
							ButtonChatSend.ReadOnly = true;

						if( EditChatMessage != null )
							EditChatMessage.Text = "Chat is disabled when no network";

						//if( ListChat != null )
						//	ListChat.Enabled = false;
						//if( EditChatMessage != null )
						//	EditChatMessage.Enabled = false;
						//if( ButtonChatSend != null )
						//	ButtonChatSend.Enabled = false;
					}

					UpdateControlsToOpenMenus();

					//disable the Cutscene control
					{
						var cutsceneControl = GetCutscene();
						if( cutsceneControl != null )
							cutsceneControl.Enabled = false;
					}
				}
			}
		}

		public bool IsAnyWindowOpened()
		{
			return ParentRoot.GetComponent<UIWindow>( true, true ) != null;
		}

		public bool IsOverlappedByOtherWindows()
		{
			if( EngineConsole.Active )
				return true;
			if( MenuWindow.Instance != null )
				return true;
			if( GetComponent<MessageBoxWindow>( false, true ) != null )
				return true;
			return false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( scene != null )
			{
				UpdateInventoryWidget();
				UpdateContinuousInteractionWidget( delta );
				UpdateInGameMenu();
				UpdateChat();
				UpdateCutscene();
				UpdateControlsToOpenMenus();
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

			if( Scene != null )
				ShowChatMessagesOnScreen( renderer );

			//display network client connection status
			if( SimulationAppClient.ConnectionNode != null )
			{
				var color = ColorValue.Zero;
				var text = "";
				if( SimulationAppClient.ConnectionNode.Status == NetworkStatus.Disconnected )
				{
					color = new ColorValue( 1, 0, 0 );
					text = SimulationAppClient.ConnectionNode.DisconnectionReason;
				}
				else if( SimulationAppClient.ConnectionNode.Status == NetworkStatus.Connected )
				{
					var seconds = SimulationAppClient.ConnectionNode.GetRoundtripLastInSeconds();
					if( seconds > 4 )
					{
						color = new ColorValue( 1, 1, 0 );
						text = $"Doesn't receive messages from the server {(int)seconds} seconds.";
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
			if( IsContinuousInteractionEnabled() )
				enabled = false;

			var menu = GetInGameMenu();
			if( menu != null && menu.Enabled )
				enabled = false;

			//disable when cutscene
			if( gameMode != null && gameMode.CutsceneStarted )
				enabled = false;
		}

		protected virtual void GameMode_ShowControlledObject( GameMode sender, Viewport viewport, ref bool show )
		{
		}

		protected virtual void GameMode_RenderTargetImageBefore( GameMode sender, CanvasRenderer renderer, ref bool show )
		{
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			//manage In-Game Menu
			if( scene != null && !InGameMenuAlwaysHide && !IsOverlappedByOtherWindows() )
			{
				if( e.Key == EKeys.F1 )
				{
					var menu = GetInGameMenu();
					if( menu != null )
					{
						menu.Enabled = !menu.Enabled;
						return true;
					}
				}

				//open In-Game Menu and focus chat message edit
				if( e.Key == EKeys.Return && IsChatEnabled() && EditChatMessage != null && !EditChatMessage.Focused )
				{
					InGameMenuShow( true );
					EditChatMessage.Focus();
					return true;
				}

				if( e.Key == EKeys.Escape )
				{
					var menu = GetInGameMenu();
					if( menu != null && menu.Enabled )
					{
						menu.Enabled = false;
						return true;
					}
				}
			}

			//skip continuous interaction fading in
			if( scene != null && e.Key == EKeys.Space && IsContinuousInteractionEnabled() )
			{
				if( continuousInteractionMessageTime < 10000 )
				{
					continuousInteractionMessageTime = 10000;
					continuousInteractionButtonsAlpha = 1;
					return true;
				}
			}

			//switch active item
			if( scene != null && gameMode != null && e.Key >= EKeys.D1 && e.Key <= EKeys.D8 && !IsOverlappedByOtherWindows() )
			{
				var index = e.Key - EKeys.D1;
				gameMode.SwitchActiveItem( index );
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

		public void TouchControlsEnable( bool enable )
		{
			var controlNames = new string[] { "Forward", "Backward", "Left", "Right", "Up", "Down", "Jump", "Fire", "Fire 2", "Interact", "Camera" };

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
			var enable = SystemSettings.MobileDevice && !gameMode.FreeCamera && gameMode.UseBuiltInCamera.Value != GameMode.BuiltInCameraEnum.None;
			TouchControlsEnable( enable );
		}

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
				var gameLogic = scene.GetGameLogic();
				var userService = SimulationAppClient.ConnectionNode?.Users;
				var chatService = SimulationAppClient.ConnectionNode?.Chat;
				var chatServiceDefaultRoom = chatService?.GetRoom( "Default" );

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
						if( gameLogic != null && chatServiceDefaultRoom != null )
						{
							var referenceToObject = "root:" + obj.GetPathFromRoot();
							var user = userService.GetUserByObjectControlledByPlayer( referenceToObject );
							if( user != null )
							{
								var lastMessage = chatService.GetLastRoomMessageFromUser( chatServiceDefaultRoom, user.UserID );
								if( lastMessage != null && EngineApp.EngineTime - lastMessage.ReceivedEngineTime < DisplayMessagesAboveObjectsTime )
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

		protected virtual void Scene_SimulationStep( NeoAxis.Component obj )
		{
		}

		///////////////////////////////////////////////

		InputProcessing GetCharacterInputProcessing()
		{
			var controlledObject = gameMode?.ObjectControlledByPlayer.Value;
			if( controlledObject != null )
				return controlledObject.GetComponent<InputProcessing>();
			return null;
		}

		public bool IsPointInsideControl( string controlName, Vector2 screenPosition )
		{
			var control = Components.GetByPath( controlName ) as UIControl;//var control = GetComponent( controlName ) as UIControl;
			if( control != null )
				return control.GetScreenRectangle().Contains( screenPosition );
			return false;
		}

		public bool IsControlTouched( InputProcessing inputProcessing, string controlName )
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
			var keyJump = gameMode.KeyJump1.Value;
			if( keyJump == EKeys.None )
				keyJump = gameMode.KeyJump2.Value;
			if( keyJump != EKeys.None )
			{
				if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Jump", e.Position ) )
					gameMode.ProcessInputMessage( new InputMessageKeyDown( keyJump ) );
			}

			//if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Jump", e.Position ) )
			//	gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Space ) );
		}

		void ProcessJumpSimulationStep( GameMode gameMode, InputProcessing inputProcessing )
		{
			var keyJump = gameMode.KeyJump1.Value;
			if( keyJump == EKeys.None )
				keyJump = gameMode.KeyJump2.Value;
			if( keyJump != EKeys.None )
			{
				if( IsControlTouched( inputProcessing, "Jump" ) )
					gameMode.ProcessInputMessage( new InputMessageKeyDown( keyJump ) );
				else
					gameMode.ProcessInputMessage( new InputMessageKeyUp( keyJump ) );
			}

			//if( IsControlTouched( inputProcessing, "Jump" ) )
			//	gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.Space ) );
			//else
			//	gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.Space ) );
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
			var keyInteract = gameMode.KeyInteract1.Value;
			if( keyInteract == EKeys.None )
				keyInteract = gameMode.KeyInteract2.Value;
			if( keyInteract != EKeys.None )
			{
				if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Interact", e.Position ) )
					gameMode.ProcessInputMessage( new InputMessageKeyDown( keyInteract ) );
				else
					gameMode.ProcessInputMessage( new InputMessageKeyUp( keyInteract ) );
			}

			//if( e.Action == TouchData.ActionEnum.Down && IsPointInsideControl( "Interact", e.Position ) )
			//	gameMode.ProcessInputMessage( new InputMessageKeyDown( EKeys.E ) );
			//else
			//	gameMode.ProcessInputMessage( new InputMessageKeyUp( EKeys.E ) );
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
									gameMode.SwitchActiveItem( nItem2 - 1 );
								} );
							e.TouchDownRequestToControlActions.Add( item );
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
			if( !gameMode.FreeCamera && gameMode.UseBuiltInCamera.Value != GameMode.BuiltInCameraEnum.None && gameMode.Scene.Mode.Value == Scene.ModeEnum._3D )
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
			var allow = !gameMode.FreeCamera && gameMode.UseBuiltInCamera.Value != GameMode.BuiltInCameraEnum.None;

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

			//this code is executed on client too, same as in single, server modes

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

		//enable focus to allows unfocusing controls in the screen
		[Browsable( false )]
		public override bool CanFocus
		{
			get { return true; }
		}

		protected virtual void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			if( MatchInfo != null && message == "Chat.NewMessage" )
			{
				if( long.TryParse( data, out var chatID ) )
				{
					if( MatchInfo.ChatID == chatID )
						chatNewMessagesAvailable = true;
				}
			}

			if( MatchInfo != null && message == "Match.StatusChanged" )
			{
				var rootBlock = TextBlock.Parse( data, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( "Messages_ReceiveMessageString: Match.StatusChanged error: " + error );
					return;
				}

				long.TryParse( rootBlock.GetAttribute( "MatchID" ), out var matchID );
				var status = rootBlock.GetAttribute( "Status" );

				if( matchID == MatchInfo.Id )
				{
					//update status without receiving new match info
					MatchInfo.Status = status;

					switch( status )
					{
					case "Deleted":
						EngineThreading.ExecuteFromMainThreadLater( delegate ()
						{
							MessageBoxWindow.Show( this, "The match was deleted.", "Match Deleted", EMessageBoxButtons.OK, EMessageBoxIcon.None, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
							{
								//go to the matches window
								SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui", false );
								//CloudClientInitialization.Instance?.PlayInCloud();
							} );
						} );
						break;
					}
				}
			}
		}

		protected virtual void Messages_ReceiveMessageBinary( ClientNetworkService_Messages sender, string message, byte[] data )
		{
		}

		public void SoundPlay2D( string virtualFileName )
		{
			if( VirtualFile.Exists( virtualFileName ) )
				Scene.SoundPlay2D( virtualFileName );
		}

		///////////////////////////////////////////////
		// Inventory widget

		public UIControl GetInventoryWidget()
		{
			return GetComponent<UIControl>( "Inventory Widget" );
		}

		void UpdateInventoryWidget()
		{
			var widget = GetInventoryWidget();
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

		///////////////////////////////////////////////
		// Continuous interaction

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
		// In-game menu

		public UIControl GetInGameMenu()
		{
			return GetComponent<UIControl>( "In-Game Menu" );
		}

		public void InGameMenuShow( bool enable )
		{
			var menu = GetInGameMenu();
			if( menu != null )
				menu.Enabled = enable && !InGameMenuAlwaysHide;
		}

		void UpdateInGameMenu()
		{
			if( CloudServiceClient.Client != null )
			{
				//cloud mode
				if( ButtonMatchReset != null )
					ButtonMatchReset.ReadOnly = MatchInfo == null || CloudServiceClient.ThisUserID != MatchInfo.UserID;
				if( ButtonMatchDelete != null )
					ButtonMatchDelete.ReadOnly = MatchInfo == null || CloudServiceClient.ThisUserID != MatchInfo.UserID;
			}
			else
			{
				//multiplayer, single modes
				if( ButtonMatchReset != null )
					ButtonMatchReset.ReadOnly = true;
				if( ButtonMatchDelete != null )
					ButtonMatchDelete.ReadOnly = true;
			}
		}

		public virtual void MatchDelete()
		{
			MessageBoxWindow.Show( this, "Delete the match?", "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
			{
				if( result == EDialogResult.Yes )
				{
					Task.Run( async delegate ()
					{
						var client = CloudServiceClient.Client;
						if( client == null )
							return;

						//delete match
						{
							using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
							var result = await client.CallMethodAsync( "Matches", "UpdateMatch", cts.Token, MatchInfo.Id, "Deleted", null, null );
							if( !string.IsNullOrEmpty( result.Error ) )
							{
								Log.Warning( "Error: " + result.Error );
								return;
							}
						}
					} );
				}
			} );
		}

		public virtual void MatchReset()
		{
			MessageBoxWindow.Show( this, "Reset the match?", "Confirm", EMessageBoxButtons.YesNo, EMessageBoxIcon.Question, null, delegate ( MessageBoxWindow sender, EDialogResult result, object anyData )
			{
				if( result == EDialogResult.Yes )
				{
					//reset match
					Task.Run( async delegate ()
					{
						var client = CloudServiceClient.Client;
						if( client == null )
							return;

						using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
						var result = await client.CallMethodAsync( "CloudServerImplementation", "ResetMatch", cts.Token, MatchInfo.Id );
						if( !string.IsNullOrEmpty( result.Error ) )
						{
							Log.Warning( "Error: " + result.Error );
							return;
						}
					} );
				}
			} );
		}

		public void ButtonMatchDelete_Click( NeoAxis.UIButton sender )
		{
			//cloud service mode
			var client = CloudServiceClient.Client;
			if( client != null && MatchInfo != null )
			{
				MatchDelete();
				return;
			}

			//go to entrance screen
			if( GameLogic != null )
			{
				var m = GameLogic.BeginNetworkMessageToServer( "TryLeaveWorld" );
				if( m != null )
					m.End();
				return;
			}
		}

		public void ButtonMatchReset_Click( NeoAxis.UIButton sender )
		{
			MatchReset();
		}

		public void ButtonInGameMenu_Click( NeoAxis.UIButton sender )
		{
			var menu = GetInGameMenu();
			if( menu != null )
				InGameMenuShow( !menu.Enabled );
		}

		public void ButtonSystemMenu_Click( NeoAxis.UIButton sender )
		{
			PlayScreen.Instance?.OpenOrCloseMenu();
		}

		///////////////////////////////////////////////
		// Chat

		public bool IsChatEnabled()
		{
			var defaultRoom = SimulationAppClient.ConnectionNode?.Chat?.GetRoom( "Default" );
			var client = CloudServiceClient.Client;
			if( defaultRoom != null || client != null && MatchInfo != null && MatchInfo.ChatID != 0 )
				return true;
			return false;
		}

		protected async Task ChatGetNewMessagesAsync( object obj )
		{
			try
			{
				var client = CloudServiceClient.Client;
				if( client == null )
					return;

				var lastMessage = (Chats.Message)obj;
				var timeFrom = lastMessage != null ? lastMessage.CreationTime : DateTime.MinValue;
				var getFromEnd = lastMessage == null;

				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var getMessagesResult = await client.CallMethodAsync<Chats.Message[]>( "Chats", "GetMessages", cts.Token, MatchInfo.ChatID, new[] { "Enabled" }, timeFrom, DateTime.MaxValue, 200, getFromEnd );
				if( !string.IsNullOrEmpty( getMessagesResult.Error ) )
				{
					Log.Warning( "ChatGetNewMessagesAsync: Chats.GetMessages error: " + getMessagesResult.Error );
					chatGettingNewMessages = false;
					return;
				}
				var messages = getMessagesResult.Value;

				//update controls
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					if( ListChat != null )
					{
						foreach( var message in messages )
						{
							//find item with same message ID
							var found = false;
							foreach( var item in ListChat.Items )
							{
								var itemMessage = (Chats.Message)item.Tag;
								if( itemMessage.Id == message.Id )
								{
									found = true;
									break;
								}
							}

							if( !found )
							{
								ListChat.AddItem( message.Username + ": " + message.Text, message );
								ListChat.SelectedIndex = ListChat.Items.Count - 1;
								ListChat.EnsureVisible( ListChat.SelectedIndex );

								if( chatMessagesOnScreen.Count > 1000 )
									chatMessagesOnScreen.Dequeue();
								chatMessagesOnScreen.Enqueue( (message.Username, message.Text, DateTime.UtcNow) );
							}
						}
					}

					chatGettingNewMessages = false;
				} );
			}
			catch( Exception e )
			{
				Log.Warning( "ChatGetNewMessagesAsync error: " + e.ToString() );
				chatGettingNewMessages = false;
			}
		}

		public void ShowChatMessagesOnScreen( CanvasRenderer renderer )
		{
			if( ListChat != null )
			{
				var lines = new List<string>();
				foreach( var message in chatMessagesOnScreen )
					lines.Add( message.Username + ": " + message.Text );

				var listRectangle = ListChat.GetScreenRectangle();
				var rectangle = new Rectangle( listRectangle.Left, 0, 1, listRectangle.Bottom );

				CanvasRendererUtility.AddTextLinesWithShadow( renderer.ViewportForScreenCanvasRenderer, lines, rectangle, EHorizontalAlignment.Left, EVerticalAlignment.Bottom, new ColorValue( 0.95, 0.95, 0.95 ) );
			}
		}

		private void Chat_ReceivedRoomMessage( ClientNetworkService_Chat sender, ClientNetworkService_Chat.RoomMessage message )
		{
			//receive chat message from the server in multiplayer mode

			var user = sender.UsersService.GetUser( message.UserID );
			var userString = user != null ? user.Username : message.UserID.ToString();

			//show on the screen
			if( chatMessagesOnScreen.Count > 1000 )
				chatMessagesOnScreen.Dequeue();
			chatMessagesOnScreen.Enqueue( (userString, message.Text, DateTime.UtcNow) );

			//add to the list in the in-game menu
			ChatAddMesageToList( message );
		}

		void ChatSendMessage()
		{
			//get message text
			var message = EditChatMessage.Text.Value.Trim();
			if( string.IsNullOrEmpty( message ) )
				return;

			//multiplayer mode
			var defaultRoom = SimulationAppClient.ConnectionNode?.Chat?.GetRoom( "Default" );
			if( defaultRoom != null )
			{
				SimulationAppClient.ConnectionNode.Chat.SayInRoom( defaultRoom, message );
				EditChatMessage.Text = "";
				if( InGameMenuAutoHideWhenSentChatMessage )
					InGameMenuShow( false );
			}

			//cloud service mode
			var client = CloudServiceClient.Client;
			if( client != null && MatchInfo != null && MatchInfo.ChatID != 0 )
			{
				Task.Run( async delegate ()
				{
					try
					{
						using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
						var result = await client.CallMethodAsync<long>( "Chats", "NewMessage", cts.Token, MatchInfo.ChatID, message, null, null );
						if( !string.IsNullOrEmpty( result.Error ) )
						{
							Log.Warning( "Error: " + result.Error );
							return;
						}

						EngineThreading.ExecuteFromMainThreadLater( delegate ()
						{
							EditChatMessage.Text = "";
							if( InGameMenuAutoHideWhenSentChatMessage )
								InGameMenuShow( false );
						} );
					}
					catch( Exception e )
					{
						Log.Warning( "ChatSendMessage error: " + e.ToString() );
					}
				} );
			}
		}

		void UpdateChat()
		{
			//get new chat messages in cloud service mode
			if( MatchInfo != null && chatNewMessagesAvailable && !chatGettingNewMessages )
			{
				chatNewMessagesAvailable = false;
				chatGettingNewMessages = true;

				Chats.Message lastMessage = null;
				if( ListChat.Items.Count > 0 )
				{
					var lastItem = ListChat.Items[ ListChat.Items.Count - 1 ];
					lastMessage = lastItem.Tag as Chats.Message;
				}
				Task.Run( () => ChatGetNewMessagesAsync( lastMessage ) );
			}

			//delete old chat messages from the screen
			{
				var utcNow = DateTime.UtcNow;
				while( chatMessagesOnScreen.TryPeek( out var item ) )
				{
					if( ( utcNow - item.Time ).TotalSeconds > chatMessagesOnScreenTime )
						chatMessagesOnScreen.Dequeue();
					else
						break;
				}
			}

			//update ButtonChatSend
			if( ButtonChatSend != null )
			{
				if( IsChatEnabled() )
					ButtonChatSend.ReadOnly = EditChatMessage == null || string.IsNullOrEmpty( EditChatMessage.Text.Value.Trim() );
				else
					ButtonChatSend.ReadOnly = true;
			}
		}

		public void EditChatMessage_KeyDownBefore( NeoAxis.UIControl sender, NeoAxis.KeyEvent e, ref bool handled )
		{
			if( e.Key == EKeys.Enter && EditChatMessage != null && EditChatMessage.Focused )
			{
				ChatSendMessage();
				handled = true;
			}
		}

		public void ButtonChatSend_Click( NeoAxis.UIButton sender )
		{
			ChatSendMessage();
		}

		public virtual void ChatAddListMessage( string text )
		{
			var list = ListChat;

			list.AddItem( text );

			//multiplayer mode
			if( SimulationAppClient.ConnectionNode?.Chat != null )
			{
				while( list.Items.Count > SimulationAppClient.ConnectionNode.Chat.MaxMessagesInRoom )
					list.RemoveItem( 0 );
			}

			//cloud service mode
			var client = CloudServiceClient.Client;
			if( client != null && MatchInfo != null && MatchInfo.ChatID != 0 )
			{
				while( list.Items.Count > 200 )
					list.RemoveItem( 0 );
			}

			list.SelectedIndex = list.Items.Count - 1;
			list.EnsureVisible( list.Items.Count - 1 );
		}

		void ChatAddMesageToList( ClientNetworkService_Chat.RoomMessage message )
		{
			//multiplayer mode

			var chatService = SimulationAppClient.ConnectionNode?.Chat;
			var user = chatService.UsersService.GetUser( message.UserID );
			var userString = user != null ? user.Username : message.UserID.ToString();

			ChatAddListMessage( $"{userString}: {message.Text}" );
		}

		///////////////////////////////////////////////
		// Cutscene

		public UIControl GetCutscene()
		{
			return GetComponent<UIControl>( "Cutscene" );
		}

		void UpdateCutscene()
		{
			var cutsceneControl = GetCutscene();
			if( cutsceneControl != null && gameMode != null )
			{
				cutsceneControl.ColorMultiplier = new ColorValue( 1, 1, 1, gameMode.CutsceneGuiFadingFactor );
				cutsceneControl.Enabled = gameMode.CutsceneGuiFadingFactor > 0;

				var textControl = cutsceneControl.Components[ "Bottom\\Text" ] as UIText;
				if( textControl != null )
					textControl.Text = gameMode.CutsceneText;
			}
		}

		///////////////////////////////////////////////

		protected virtual void UpdateControlsToOpenMenus()
		{

			//!!!!change to machines with keyboard

			var keyboardAvailable = !SystemSettings.MobileDevice; //var keyboardAvailable = SystemSettings.KeyboardAvailable;

			var inGameMenu = GetInGameMenu();

			//disable menu buttons on PC by default
			if( ButtonInGameMenu != null )
				ButtonInGameMenu.Enabled = !keyboardAvailable && !InGameMenuAlwaysHide && inGameMenu != null;
			if( ButtonSystemMenu != null )
				ButtonSystemMenu.Enabled = !keyboardAvailable && !InGameMenuAlwaysHide;
			//if( TextShowInGameMenu != null )
			//	TextShowInGameMenu.Enabled = keyboardAvailable && !InGameMenuAlwaysHide && inGameMenu != null;
		}
	}
}