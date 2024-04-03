// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Project;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A useful component to make server-client logic of the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Network Logic", -5002 )]
	public class NetworkLogic : NetworkLogicAbstract
	{
		//common properties. some of them are synchronized

		/// <summary>
		/// Whether to show the entrance screen for entered players.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> EntranceScreen
		{
			get { if( _entranceScreen.BeginGet() ) EntranceScreen = _entranceScreen.Get( this ); return _entranceScreen.value; }
			set { if( _entranceScreen.BeginSet( this, ref value ) ) { try { EntranceScreenChanged?.Invoke( this ); } finally { _entranceScreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EntranceScreen"/> property value changes.</summary>
		public event Action<NetworkLogic> EntranceScreenChanged;
		ReferenceField<bool> _entranceScreen = true;

		/// <summary>
		/// Description of the world that is shown at the start.
		/// </summary>
		[DefaultValue( "A description of your world." )]
		public Reference<string> EntranceScreenDescription
		{
			get { if( _entranceScreenDescription.BeginGet() ) EntranceScreenDescription = _entranceScreenDescription.Get( this ); return _entranceScreenDescription.value; }
			set { if( _entranceScreenDescription.BeginSet( this, ref value ) ) { try { EntranceScreenDescriptionChanged?.Invoke( this ); } finally { _entranceScreenDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EntranceScreenDescription"/> property value changes.</summary>
		public event Action<NetworkLogic> EntranceScreenDescriptionChanged;
		ReferenceField<string> _entranceScreenDescription = "A description of your world.";

		/// <summary>
		/// The ability the select avatar before entrance to the world.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AvatarWindow
		{
			get { if( _avatarWindow.BeginGet() ) AvatarWindow = _avatarWindow.Get( this ); return _avatarWindow.value; }
			set { if( _avatarWindow.BeginSet( this, ref value ) ) { try { AvatarWindowChanged?.Invoke( this ); } finally { _avatarWindow.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AvatarWindow"/> property value changes.</summary>
		public event Action<NetworkLogic> AvatarWindowChanged;
		ReferenceField<bool> _avatarWindow = true;

		/// <summary>
		/// The type of object to control by the player.
		/// </summary>
		[DefaultValueReference( "NeoAxis.Character" )]
		[NetworkSynchronize( false )]
		public Reference<Metadata.TypeInfo> ObjectTypeControlledByPlayer
		{
			get { if( _objectTypeControlledByPlayer.BeginGet() ) ObjectTypeControlledByPlayer = _objectTypeControlledByPlayer.Get( this ); return _objectTypeControlledByPlayer.value; }
			set { if( _objectTypeControlledByPlayer.BeginSet( this, ref value ) ) { try { ObjectTypeControlledByPlayerChanged?.Invoke( this ); } finally { _objectTypeControlledByPlayer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectTypeControlledByPlayer"/> property value changes.</summary>
		public event Action<NetworkLogic> ObjectTypeControlledByPlayerChanged;
		ReferenceField<Metadata.TypeInfo> _objectTypeControlledByPlayer = new Reference<Metadata.TypeInfo>( null, "NeoAxis.Character" );

		/////////////////////////////////////////

		public Scene GetScene()
		{
			return ParentRoot as Scene;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( EntranceScreenDescription ):
					if( !EntranceScreen )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

#if !CLIENT
			if( NetworkIsServer && SimulationAppServer.Created )
				ServerOnEnabledInHierarchyChanged();
#endif
			if( NetworkIsClient && SimulationAppClient.Created )
				ClientOnEnabledInHierarchyChanged();
		}

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

#if !CLIENT
			if( NetworkIsServer && SimulationAppServer.Created )
				ServerOnEnabledInSimulation();
#endif
			if( NetworkIsClient && SimulationAppClient.Created )
				ClientOnEnabledInSimulation();
		}

		protected override void OnDisabledInSimulation()
		{
			base.OnDisabledInSimulation();

#if !CLIENT
			if( NetworkIsServer && SimulationAppServer.Created )
				ServerOnDisabledInSimulation();
#endif
			if( NetworkIsClient && SimulationAppClient.Created )
				ClientOnDisabledInSimulation();
		}

		/////////////////////////////////////////
		/////////////////////////////////////////
		/////////////////////////////////////////
		//server only
#if !CLIENT

		float serverUpdateRemainingTime;
		Dictionary<ServerNetworkService_Users.UserInfo, ServerUserItem> serverUsers = new Dictionary<ServerNetworkService_Users.UserInfo, ServerUserItem>();

		//!!!!cache only for simulation
		SpawnPoint[] spawnPointsCache;

		///////////////////////////////////////////////

		public class ServerUserItem
		{
			public ServerNetworkService_Users.UserInfo User;

			public bool EnteredToWorld;
			public Component ObjectControlledByPlayer;
			public bool ObjectControlledByPlayerInputEnabled;

			public object AnyData;
		}

		///////////////////////////////////////////////

		protected virtual void ServerOnEnabledInHierarchyChanged()
		{
		}

		protected virtual void ServerOnEnabledInSimulation()
		{
		}

		protected virtual void ServerOnDisabledInSimulation()
		{
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( SimulationAppServer.Created )
			{
				serverUpdateRemainingTime -= Time.SimulationDelta;
				if( serverUpdateRemainingTime < 0 )
				{
					serverUpdateRemainingTime = 1;

					UpdateUsersList();
					UpdateObjectControlledByPlayers();
				}
			}
		}

		protected virtual ServerUserItem ServerOnNewUserItem()
		{
			return new ServerUserItem();
		}

		protected virtual void AddUser( ServerNetworkService_Users.UserInfo user )
		{
			var item = ServerOnNewUserItem();
			item.User = user;
			serverUsers[ user ] = item;
		}

		protected virtual void RemoveUser( ServerUserItem item )
		{
			serverUsers.Remove( item.User );

			if( item.ObjectControlledByPlayer != null )
			{
				item.ObjectControlledByPlayer.RemoveFromParent( true );
				item.ObjectControlledByPlayerInputEnabled = false;
				item.ObjectControlledByPlayer = null;
			}
		}

		public ServerUserItem[] ServerGetUsers()
		{
			var result = new ServerUserItem[ serverUsers.Count ];
			serverUsers.Values.CopyTo( result, 0 );
			return result;
		}

		void UpdateUsersList()
		{
			//remove old users
			{
				var toRemove = new List<ServerUserItem>();

				foreach( var item in serverUsers.Values )
				{
					if( SimulationAppServer.Server.Users.GetUser( item.User.UserID ) == null )
						toRemove.Add( item );
				}

				foreach( var item in toRemove )
					RemoveUser( item );
			}

			//add new users
			foreach( var user in SimulationAppServer.Server.Users.Users )
			{
				if( !serverUsers.ContainsKey( user ) )
					AddUser( user );
			}
		}

		public delegate void ObjectControlledByPlayerCreatedDelegate( NetworkLogic sender, Component obj );
		public event ObjectControlledByPlayerCreatedDelegate ObjectControlledByPlayerCreated;

		public delegate void ObjectControlledByPlayerRemovingDelegate( NetworkLogic sender, Component obj );
		public event ObjectControlledByPlayerRemovingDelegate ObjectControlledByPlayerRemoving;

		public virtual Component CreateObjectControlledByPlayer( ServerUserItem userItem, Metadata.TypeInfo objectType, Transform transform )
		{
			var scene = ParentRoot as Scene;
			if( scene == null )
				return null;

			var obj = scene.CreateComponent( objectType, enabled: false, setUniqueName: true );
			obj.NewObjectSetDefaultConfiguration();

			if( obj is Character character )
			{
				//Character

				var inputProcessing = obj.CreateComponent<CharacterInputProcessing>();

				string avatarSettings = "";

				if( SimulationAppServer.NetworkMode == SimulationAppServer.NetworkModeEnum.CloudProject )
				{

					//!!!!impl cloud

					avatarSettings = userItem.User.DirectServerAvatar;

				}
				else
					avatarSettings = userItem.User.DirectServerAvatar;

				var block = TextBlock.Parse( avatarSettings, out var error );
				if( !string.IsNullOrEmpty( error ) )
					Log.Warning( "NetworkLogic: CreateObjectControlledByPlayer: Unable to parse avatar settings. " + error );

				if( block != null )
				{
					var settings = new AvatarSettings();
					if( settings.Load( block ) )
					{
						var name = settings.NamedCharacter;
						if( !string.IsNullOrEmpty( name ) )
						{
							var typeFileName = @$"Content\Characters\Authors\NeoAxis\{name}\{name}.charactertype";

							if( VirtualFile.Exists( typeFileName ) )
							{
								//it is synchronized with clients via GetByReference property
								character.CharacterType = new ReferenceNoValue( typeFileName );

								if( character.CharacterType.Value == null )
									Log.Warning( "NetworkLogic: CreateObjectControlledByPlayer: character.CharacterType.Value == null. Reference value: " + typeFileName );
							}
						}
					}
				}
			}
			else if( obj is Character2D character2D )
			{
				//Character2D

				var inputProcessing = obj.CreateComponent<Character2DInputProcessing>();

				//!!!!avatars for 2D

				//string avatarSettings = "";

				//if( SimulationAppServer.NetworkMode == SimulationAppServer.NetworkModeEnum.CloudProject )
				//{

				//	//!!!!impl cloud

				//}
				//else
				//	avatarSettings = userItem.User.DirectServerAvatar;

				//var block = TextBlock.Parse( avatarSettings, out var error );
				//if( !string.IsNullOrEmpty( error ) )
				//	Log.Warning( "NetworkLogic: CreateObjectControlledByPlayer: Unable to parse avatar settings. " + error );

				//if( block != null )
				//{
				//	var settings = new AvatarSettings();
				//	if( settings.Load( block ) )
				//	{
				//		var name = settings.NamedCharacter;
				//		if( !string.IsNullOrEmpty( name ) )
				//		{
				//			var typeFileName = @$"Content\Characters 2D\Authors\NeoAxis\{name}\{name}.character2dtype";
				//			if( VirtualFile.Exists( typeFileName ) )
				//			{
				//				//it is synchronized with clients via GetByReference property
				//				character2D.CharacterType = new ReferenceNoValue( typeFileName );

				//				if( character2D.CharacterType.Value == null )
				//					Log.Warning( "NetworkLogic: CreateObjectControlledByPlayer: character.CharacterType.Value == null. Reference value: " + typeFileName );
				//			}
				//		}
				//	}
				//}
			}
			else if( obj is Vehicle vehicle )
			{
				//Vehicle

				var inputProcessing = obj.CreateComponent<VehicleInputProcessing>();

				//!!!!
				//vehicle.VehicleType = ;
			}

			obj.Enabled = true;

			if( obj is Character character2 )
			{
				//Character specific
				character2.SetTransformAndTurnToDirectionInstantly( transform );
			}
			else if( obj is Character2D character2D2 )
			{
				//Character2D specific
				character2D2.SetTransform( transform );
				//character2D2.SetTransformAndTurnToDirectionInstantly( transform );
			}
			else if( obj is Vehicle vehicle2 )
			{
				//Vehicle specific
				vehicle2.SetTransform( transform, true );
			}
			else if( obj is ObjectInSpace objectInSpace )
			{
				//ObjectInSpace specific
				objectInSpace.SetPosition( transform.Position );
				objectInSpace.SetRotation( transform.Rotation );
			}

			userItem.ObjectControlledByPlayer = obj;
			userItem.ObjectControlledByPlayerInputEnabled = true;

			SendSetObjectControlledByPlayer( userItem );

			ObjectControlledByPlayerCreated?.Invoke( this, obj );

			return obj;
		}

		public virtual SpawnPoint[] GetSpawnPoints()
		{
			if( spawnPointsCache == null )
				spawnPointsCache = ParentRoot.GetComponents<SpawnPoint>( onlyEnabledInHierarchy: true );
			return spawnPointsCache;
		}

		public void ResetSpawnPointsCache()
		{
			spawnPointsCache = null;
		}

		public virtual SpawnPoint[] GetSpawnPointsForTeam( int team )
		{
			return GetSpawnPoints().Where( p => (int)p.Team.Value == team ).ToArray();
		}

		protected virtual SpawnPoint OnGetSpawnPoint( ServerUserItem userInfo, Metadata.TypeInfo objectType )
		{
			var array = GetSpawnPoints();

			var random = new FastRandom();
			var index = random.Next( array.Length );
			if( index >= 0 && index < array.Length )
				return array[ index ];

			return null;
		}

		public delegate void ObjectControlledByPlayerBeforeCreateDelegate( NetworkLogic sender, ServerUserItem userItem, ref bool handled );
		public event ObjectControlledByPlayerBeforeCreateDelegate ObjectControlledByPlayerBeforeCreate;

		protected virtual void UpdateObjectControlledByPlayer( ServerUserItem userItem )
		{
			//create new
			if( userItem.EnteredToWorld && userItem.ObjectControlledByPlayer == null )
			{
				var handled = false;
				ObjectControlledByPlayerBeforeCreate?.Invoke( this, userItem, ref handled );

				//default behaviour
				if( !handled )
				{
					var objectType = ObjectTypeControlledByPlayer.Value;
					if( objectType != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
					{
						var spawnPoint = OnGetSpawnPoint( userItem, objectType );
						if( spawnPoint != null )
						{
							if( MetadataManager.GetTypeOfNetType( typeof( Character ) ).IsAssignableFrom( objectType ) )
							{
								//Character

								var scene = GetScene();
								if( scene != null )
								{
									if( CharacterUtility.FindFreePlace( scene, 2, 0.5, spawnPoint.TransformV.Position, 4, -1, 1, null, out var freePlacePosition ) )
									{
										var tr = spawnPoint.TransformV;
										tr = tr.UpdatePosition( freePlacePosition );
										CreateObjectControlledByPlayer( userItem, objectType, tr );
									}
								}
							}
							else if( MetadataManager.GetTypeOfNetType( typeof( Character2D ) ).IsAssignableFrom( objectType ) )
							{
								//Character2D

								//!!!!

								////!!!!not only for characters
								//var objectBounds = new Bounds( -0.2, -0.2, -0.9, 0.2, 0.2, 0.9 );

								var transform = new Transform( spawnPoint.TransformV.Position );//GetFreePlaceForObject( objectPosition, objectBounds );
								if( transform != null )
									CreateObjectControlledByPlayer( userItem, objectType, transform );
							}
							else
							{
								//!!!!other components check free place

								var transform = spawnPoint.TransformV;

								CreateObjectControlledByPlayer( userItem, objectType, transform );
							}
						}
					}
				}
			}

			//destroy when leave the world (to World Entrance screen or exited)
			if( !userItem.EnteredToWorld && userItem.ObjectControlledByPlayer != null )
			{
				ObjectControlledByPlayerRemoving?.Invoke( this, userItem.ObjectControlledByPlayer );

				userItem.ObjectControlledByPlayer.RemoveFromParent( true );
				userItem.ObjectControlledByPlayer = null;
			}
		}

		public void UpdateObjectControlledByPlayers()
		{
			foreach( var userItem in serverUsers.Values )
			{
				//clear deleted
				//!!!!maybe better to check EnabledInHierarchy because object can be deleted with parent. where else
				if( userItem.ObjectControlledByPlayer != null && userItem.ObjectControlledByPlayer.Parent == null )
					userItem.ObjectControlledByPlayer = null;

				UpdateObjectControlledByPlayer( userItem );
			}
		}

		public ServerUserItem ServerGetUser( ServerNetworkService_Users.UserInfo user )
		{
			if( user != null && serverUsers.TryGetValue( user, out var item ) )
				return item;
			return null;
		}

		public override Component ServerGetObjectControlledByUser( ServerNetworkService_Users.UserInfo user, bool inputMustEnabled = true )
		{
			var item = ServerGetUser( user );
			if( item != null )
			{
				if( item.ObjectControlledByPlayerInputEnabled || !inputMustEnabled )
					return item.ObjectControlledByPlayer;
			}
			return null;
		}

		public override ServerNetworkService_Users.UserInfo ServerGetUserByObjectControlled( Component obj, bool inputMustEnabled = true )
		{
			//!!!!slowly. dictionary

			if( obj != null )
			{
				foreach( var item in serverUsers.Values )
				{
					if( item.ObjectControlledByPlayer == obj )
					{
						if( item.ObjectControlledByPlayerInputEnabled || !inputMustEnabled )
							return item.User;
						else
							return null;
					}
				}
			}
			return null;
		}

		void SendSetObjectControlledByPlayer( ServerUserItem item )
		{
			var referenceToObject = item.ObjectControlledByPlayer != null ? "root:" + item.ObjectControlledByPlayer.GetPathFromRoot() : "";

			//send update to the user
			var writer = BeginNetworkMessage( item.User, "SetObjectControlledByPlayer" );
			if( writer != null )
			{
				writer.Write( referenceToObject );
				EndNetworkMessage();
			}

			//send update to all users. can be optional
			SimulationAppServer.Server?.Users.UpdateObjectControlledByPlayerToClient( item.User, referenceToObject );
		}

		public override void ServerChangeObjectControlled( ServerNetworkService_Users.UserInfo user, Component obj )
		{
			var item = ServerGetUser( user );
			if( item != null )
			{
				item.ObjectControlledByPlayer = obj;
				item.ObjectControlledByPlayerInputEnabled = true;

				SendSetObjectControlledByPlayer( item );
			}
		}

		public delegate void AllowEnterToWorldDelegate( NetworkLogic sender, ServerUserItem userItem, ref string reason );
		public event AllowEnterToWorldDelegate AllowEnterToWorld;

		protected virtual string OnAllowEnterToWorld( ServerUserItem userItem )
		{
			return "";
		}

		public string PerformAllowEnterToWorld( ServerUserItem userItem )
		{
			var reason = OnAllowEnterToWorld( userItem );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;

			AllowEnterToWorld?.Invoke( this, userItem, ref reason );
			if( !string.IsNullOrEmpty( reason ) )
				return reason;

			return "";
		}

		public void SendScreenMessageToClient( ServerNetworkService_Components.ClientItem client, string text, bool error )
		{
			var writer = BeginNetworkMessage( client, "ScreenMessage" );
			if( writer != null )
			{
				writer.Write( text );
				writer.Write( error );
				EndNetworkMessage();
			}
		}

		public void SendScreenMessageToAllClients( string text, bool error )
		{
			var writer = BeginNetworkMessageToEveryone( "ScreenMessage" );
			if( writer != null )
			{
				writer.Write( text );
				writer.Write( error );
				EndNetworkMessage();
			}
		}

		void SendSetEnteredToWorld( ServerUserItem userItem )
		{
			var writer = BeginNetworkMessage( userItem.User, "SetEnteredToWorld" );
			if( writer != null )
			{
				writer.Write( userItem.EnteredToWorld );
				EndNetworkMessage();
			}
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			var userItem = ServerGetUser( client.User );
			if( userItem != null )
			{
				if( message == "TryEnterToWorld" )
				{
					if( !userItem.EnteredToWorld )
					{
						var reason = PerformAllowEnterToWorld( userItem );

						if( string.IsNullOrEmpty( reason ) )
						{
							userItem.EnteredToWorld = true;
							SendSetEnteredToWorld( userItem );
						}
						else
							SendScreenMessageToClient( client, reason, true );
					}
				}
				else if( message == "TryLeaveWorld" )
				{
					if( userItem.EnteredToWorld )
					{
						userItem.EnteredToWorld = false;
						SendSetEnteredToWorld( userItem );
					}
				}
			}

			return true;
		}

#endif


		/////////////////////////////////////////
		/////////////////////////////////////////
		/////////////////////////////////////////
		//client only
		//#if CLIENT //the define must be enabled, but for code editing is better to disable it

		//client state
		[Browsable( false )]
		public bool EnteredToWorld;

		/////////////////////////////////////////

		protected virtual void ClientOnEnabledInHierarchyChanged()
		{
			////subscribe/unsubscribe to scene render event
			//var scene = ParentRoot as Scene;
			//if( scene != null )
			//{
			//	if( EnabledInHierarchy )
			//		scene.RenderEvent += Scene_RenderEvent;
			//	else
			//		scene.RenderEvent -= Scene_RenderEvent;
			//}
		}

		//do it in BasicSceneScreen
		//private void Scene_RenderEvent( Scene sender, Viewport viewport )
		//{
		//	var renderer = viewport.CanvasRenderer;

		//	renderer.AddText( "Test", new Vector2( 0.5, 0.5 ) );
		//}

		protected virtual void ClientOnEnabledInSimulation()
		{
			//reset GameMode.ObjectControlledByPlayer on the client
			if( SimulationAppClient.Created )
			{
				var scene = SimulationAppClient.Client?.Components.Scene;
				if( scene != null )
				{
					var gameMode = (GameMode)scene.GetGameMode();
					if( gameMode != null )
						gameMode.ObjectControlledByPlayer = null;
				}
			}
		}

		protected virtual void ClientOnDisabledInSimulation()
		{
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "SetEnteredToWorld" )
			{
				var enteredToWorld = reader.ReadBoolean();
				if( !reader.Complete() )
					return false;
				EnteredToWorld = enteredToWorld;
			}
			else if( message == "SetObjectControlledByPlayer" )
			{
				var referenceToObject = reader.ReadString();
				if( !reader.Complete() )
					return false;

				var scene = GetScene();
				if( scene != null )
				{
					var gameMode = (GameMode)scene.GetGameMode();
					if( gameMode != null )
					{
						gameMode.ObjectControlledByPlayer = new Reference<Component>( null, referenceToObject );

						//configure third person camera
						var obj = gameMode.ObjectControlledByPlayer.Value as ObjectInSpace;
						if( obj != null )
						{
							var direction = obj.TransformV.Rotation.GetForward().ToVector2();
							gameMode.ThirdPersonCameraHorizontalAngle = new Radian( Math.Atan2( direction.Y, direction.X ) ).InDegrees();
						}

						GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
					}
				}
			}
			else if( message == "ScreenMessage" )
			{
				var text = reader.ReadString();
				var error = reader.ReadBoolean();
				if( !reader.Complete() )
					return false;

				ScreenMessages.Add( text, error );
			}

			return true;
		}

		//#endif

	}
}