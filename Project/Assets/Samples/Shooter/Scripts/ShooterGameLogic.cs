// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// A component that implements the logic of the shooter game.
	/// </summary>
	public class ShooterGameLogic : GameLogic
	{
		//for server, single
		int freeForAllTeamCounter;
		double deleteObjectsBelowHeightLastTime;

		[Browsable( false )]
		public double PlayerDamagedEffectTime { get; set; }

		bool debugVisualization;

		///////////////////////////////////////////////
		//settings

		[DefaultValue( GameTypeEnum.FreeForAll )]
		public Reference<GameTypeEnum> GameType
		{
			get { if( _gameType.BeginGet() ) GameType = _gameType.Get( this ); return _gameType.value; }
			set { if( _gameType.BeginSet( this, ref value ) ) { try { GameTypeChanged?.Invoke( this ); } finally { _gameType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GameType"/> property value changes.</summary>
		public event Action<ShooterGameLogic> GameTypeChanged;
		ReferenceField<GameTypeEnum> _gameType = GameTypeEnum.FreeForAll;

		[DefaultValue( 60 )]
		public Reference<double> PreparationTime
		{
			get { if( _preparationTime.BeginGet() ) PreparationTime = _preparationTime.Get( this ); return _preparationTime.value; }
			set { if( _preparationTime.BeginSet( this, ref value ) ) { try { PreparationTimeChanged?.Invoke( this ); } finally { _preparationTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreparationTime"/> property value changes.</summary>
		public event Action<ShooterGameLogic> PreparationTimeChanged;
		ReferenceField<double> _preparationTime = 60;

		[DefaultValue( 300 )]
		public Reference<double> GameTime
		{
			get { if( _gameTime.BeginGet() ) GameTime = _gameTime.Get( this ); return _gameTime.value; }
			set { if( _gameTime.BeginSet( this, ref value ) ) { try { GameTimeChanged?.Invoke( this ); } finally { _gameTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GameTime"/> property value changes.</summary>
		public event Action<ShooterGameLogic> GameTimeChanged;
		ReferenceField<double> _gameTime = 300;

		/// <summary>
		/// The initial health of the object controlled by the player.
		/// </summary>
		[DefaultValue( 10.0 )]
		public Reference<double> ObjectControlledByPlayerHealth
		{
			get { if( _objectControlledByPlayerHealth.BeginGet() ) ObjectControlledByPlayerHealth = _objectControlledByPlayerHealth.Get( this ); return _objectControlledByPlayerHealth.value; }
			set { if( _objectControlledByPlayerHealth.BeginSet( this, ref value ) ) { try { ObjectControlledByPlayerHealthChanged?.Invoke( this ); } finally { _objectControlledByPlayerHealth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectControlledByPlayerHealth"/> property value changes.</summary>
		public event Action<ShooterGameLogic> ObjectControlledByPlayerHealthChanged;
		ReferenceField<double> _objectControlledByPlayerHealth = 10.0;

		/// <summary>
		/// A weapon that the object controlled by the player will have after creation.
		/// </summary>
		[DefaultValue( null )]
		public Reference<WeaponType> ObjectControlledByPlayerWeapon
		{
			get { if( _objectControlledByPlayerWeapon.BeginGet() ) ObjectControlledByPlayerWeapon = _objectControlledByPlayerWeapon.Get( this ); return _objectControlledByPlayerWeapon.value; }
			set { if( _objectControlledByPlayerWeapon.BeginSet( this, ref value ) ) { try { ObjectControlledByPlayerWeaponChanged?.Invoke( this ); } finally { _objectControlledByPlayerWeapon.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectControlledByPlayerWeapon"/> property value changes.</summary>
		public event Action<ShooterGameLogic> ObjectControlledByPlayerWeaponChanged;
		ReferenceField<WeaponType> _objectControlledByPlayerWeapon;

		/// <summary>
		/// Whether to delete Character components after scene loading in the simulation.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DeleteInitialCharacters
		{
			get { if( _deleteInitialCharacters.BeginGet() ) DeleteInitialCharacters = _deleteInitialCharacters.Get( this ); return _deleteInitialCharacters.value; }
			set { if( _deleteInitialCharacters.BeginSet( this, ref value ) ) { try { DeleteInitialCharactersChanged?.Invoke( this ); } finally { _deleteInitialCharacters.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeleteInitialCharacters"/> property value changes.</summary>
		public event Action<ShooterGameLogic> DeleteInitialCharactersChanged;
		ReferenceField<bool> _deleteInitialCharacters = true;

		/// <summary>
		/// Whether to subtract a frag from the player who killed himself.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SubtractFragForSuicide
		{
			get { if( _subtractFragForSuicide.BeginGet() ) SubtractFragForSuicide = _subtractFragForSuicide.Get( this ); return _subtractFragForSuicide.value; }
			set { if( _subtractFragForSuicide.BeginSet( this, ref value ) ) { try { SubtractFragForSuicideChanged?.Invoke( this ); } finally { _subtractFragForSuicide.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SubtractFragForSuicide"/> property value changes.</summary>
		public event Action<ShooterGameLogic> SubtractFragForSuicideChanged;
		ReferenceField<bool> _subtractFragForSuicide = true;

		/// <summary>
		/// The height below which objects will be deleted. This can be used to remove objects that fall out of the playable area.
		/// </summary>
		[DefaultValue( -100.0 )]
		public Reference<double> DeleteObjectsBelowHeight
		{
			get { if( _deleteObjectsBelowHeight.BeginGet() ) DeleteObjectsBelowHeight = _deleteObjectsBelowHeight.Get( this ); return _deleteObjectsBelowHeight.value; }
			set { if( _deleteObjectsBelowHeight.BeginSet( this, ref value ) ) { try { DeleteObjectsBelowHeightChanged?.Invoke( this ); } finally { _deleteObjectsBelowHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeleteObjectsBelowHeight"/> property value changes.</summary>
		public event Action<ShooterGameLogic> DeleteObjectsBelowHeightChanged;
		ReferenceField<double> _deleteObjectsBelowHeight = -100.0;

		///////////////////////////////////////////////
		//playing state

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( true )]
		[DefaultValue( GameStatusEnum.Prepare )]
		public Reference<GameStatusEnum> CurrentGameStatus
		{
			get { if( _currentGameStatus.BeginGet() ) CurrentGameStatus = _currentGameStatus.Get( this ); return _currentGameStatus.value; }
			set { if( _currentGameStatus.BeginSet( this, ref value ) ) { try { CurrentGameStatusChanged?.Invoke( this ); } finally { _currentGameStatus.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentGameStatus"/> property value changes.</summary>
		public event Action<ShooterGameLogic> CurrentGameStatusChanged;
		ReferenceField<GameStatusEnum> _currentGameStatus = GameStatusEnum.Prepare;

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( true )]
		[DefaultValue( 0.0f )]
		public Reference<float> CurrentGameTime
		{
			get { if( _currentGameTime.BeginGet() ) CurrentGameTime = _currentGameTime.Get( this ); return _currentGameTime.value; }
			set { if( _currentGameTime.BeginSet( this, ref value ) ) { try { CurrentGameTimeChanged?.Invoke( this ); } finally { _currentGameTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentGameTime"/> property value changes.</summary>
		public event Action<ShooterGameLogic> CurrentGameTimeChanged;
		ReferenceField<float> _currentGameTime = 0.0f;

		///////////////////////////////////////////////

		public enum GameTypeEnum
		{
			FreeForAll,
			//TeamDeathmatch,
			//BattleRoyale
		}

		///////////////////////////////////////////////

		public enum GameStatusEnum
		{
			Prepare,
			Play,
		}


		///////////////////////////////////////////////
		// Common

		public double GetRemainingTime()
		{
			if( CurrentGameStatus == GameStatusEnum.Prepare )
				return PreparationTime - CurrentGameTime;
			else
				return GameTime - CurrentGameTime;
		}


		///////////////////////////////////////////////
		// Server, Single

		double sendPlayersInfoToClientsRemainingTime;
		Dictionary<long, int> singleFrags = new Dictionary<long, int>(); // zero index is player

		//cloud mode specific
		[Browsable( false )]
		public long[] CloudPlayers { get; set; }

		///////////////////////////////////////////////

		public class ShooterServerUserItem : ServerUserItem
		{
			public int Frags;
			public int Team;
		}

		///////////////////////////////////////////////

		protected override bool ServerOrSingle_IsAllowToUpdateObjectControlledByPlayers()
		{
			return true;
		}

		protected override void ServerOrSingle_OnEnabledInHierarchyChanged()
		{
			base.ServerOrSingle_OnEnabledInHierarchyChanged();

			//subscribe to the Character.ProcessDamageAfterAll event only on the server and single-player mode. This event is used to detect frags.
			if( EnabledInHierarchyAndIsInstance )
				Character.ProcessDamageAfterAll += Character_ProcessDamageAfterAll;
			else
				Character.ProcessDamageAfterAll -= Character_ProcessDamageAfterAll;

			if( EnabledInHierarchyAndIsInstance )
			{
				//delete characters
				if( DeleteInitialCharacters && ParentScene != null )
				{
					foreach( var character in ParentScene.GetComponents<Character>() )
						character.RemoveFromParent( false );
				}

				//subscribe to the CurrentGameStatusChanged event to reset spawn points and delete objects of the previous round when the game status changes.
				CurrentGameStatusChanged += ServerOrSingleShooterGameLogic_CurrentGameStatusChanged;
			}
		}

		protected override ServerUserItem Server_OnNewUserItem()
		{
			return new ShooterServerUserItem();
		}

		public int GetTeamWithSmallestAmountOfPlayers()
		{
			var table = new int[ 2 ];

			foreach( var user in Server_GetUsers() )
			{
				var user2 = (ShooterServerUserItem)user;
				if( user2.Team >= 0 && user2.Team < table.Length )
					table[ user2.Team ]++;
			}

			int minIndex = Array.IndexOf( table, table.Min() );
			return minIndex;
		}

		protected override void Server_AddUser( ServerNetworkService_Users.UserInfo user )
		{
			//!!!!

			//var team = 0;
			//if( GameType.Value == GameTypeEnum.TeamDeathmatch )
			//	team = GetTeamWithSmallestAmountOfPlayers();

			base.Server_AddUser( user );

			////select a team for the user
			//if( GameType.Value == GameTypeEnum.TeamDeathmatch )
			//{
			//	var user2 = ServerGetUser( user );
			//	user2.Team = team;
			//}
		}

		public new ShooterServerUserItem Server_GetUser( ServerNetworkService_Users.UserInfo user )
		{
			return (ShooterServerUserItem)base.Server_GetUser( user );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			ServerOrSingle_TickGame();

			//!!!!optimization. //send only when changed. also send to new clients. maybe send changes only
			//send players info to clients
			sendPlayersInfoToClientsRemainingTime -= Time.SimulationDelta;
			if( sendPlayersInfoToClientsRemainingTime < 0 )
			{
				sendPlayersInfoToClientsRemainingTime = 1;
				Server_SendUsersInfoToClients();
			}

			//delete objects below height
			deleteObjectsBelowHeightLastTime -= Time.SimulationDelta;
			if( deleteObjectsBelowHeightLastTime < 0 && ParentScene != null )
			{
				deleteObjectsBelowHeightLastTime = 1;

				var plane = new Plane( Vector3.ZAxis, DeleteObjectsBelowHeight );
				var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, [ plane ] );
				ParentScene.GetObjectsInSpace( item );
				foreach( var resultItem in item.Result )
					resultItem.Object.RemoveFromParent( true );
			}
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			switch( message )
			{
			case "AddBot":
				{
					var usersService = Server_GetUsersService();
					if( usersService != null )
						usersService.AddUserBot( null, null, null );
				}
				break;

			case "DeleteBot":
				{
					var userID = reader.ReadVariableInt64();
					if( !reader.Complete() )
						return false;
					var usersService = Server_GetUsersService();
					if( usersService != null )
					{
						var user = usersService.GetUser( userID );
						if( user != null && user.Bot )
							usersService.RemoveUser( user );
					}
				}
				break;
			}

			return true;
		}

		void Server_SendUsersInfoToClients()
		{
			var m = BeginNetworkMessageToEveryone( "UsersInfo" );
			if( m != null )
			{
				var writer = m.Writer;
				var users = Server_GetUsers();

				writer.WriteVariableInt32( users.Length );
				foreach( ShooterServerUserItem item in users )
				{
					writer.WriteVariableInt64( item.User.UserID );
					writer.Write( item.User.Username );
					writer.Write( item.User.Bot );
					writer.WriteVariableInt32( item.Frags );
					writer.WriteVariableInt32( item.Team );
				}
				m.End();
			}
		}

		protected override SpawnPoint GetSpawnPointForPlayer( ServerUserItem serverUserInfo, SingleUserItem singleUserItem, Metadata.TypeInfo objectType )
		{
			//!!!!get spawn point maximally far from other players
			//{
			//	var spawnPoints = GetCachedSpawnPointsForPlayer();
			//}


			////override default behavior
			//if( GameType.Value == ShooterGameTypeEnum.TeamDeathmatch )
			//{
			//	var user = (ShooterServerUserItem)userInfo;

			//!!!!check
			//	var array = GetSpawnPointsForTeam( user.Team );

			//	var random = new FastRandom();
			//	var index = random.Next( array.Length );
			//	if( index >= 0 && index < array.Length )
			//		return array[ index ];
			//}

			return base.GetSpawnPointForPlayer( serverUserInfo, singleUserItem, objectType );
		}

		//protected override void OnUpdateObjectControlledByPlayer( UserItem userItem )
		//{
		//	//override default behavior

		//	//create new
		//	if( userItem.EnteredToWorld && userItem.ObjectControlledByPlayer == null )
		//	{
		//		var objectType = ObjectTypeControlledByPlayer.Value;
		//		if( objectType != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
		//		{
		//			Vector3 objectPosition;
		//			{
		//				var spawnPoint = OnGetSpawnPoint( userItem, objectType );
		//				if( spawnPoint != null )
		//					objectPosition = spawnPoint.TransformV.Position;
		//				else
		//					objectPosition = Vector3.Zero;
		//			}

		//			//!!!!not only for characters
		//			var objectBounds = new Bounds( -0.2, -0.2, -0.9, 0.2, 0.2, 0.9 );

		//			var transform = GetFreePlaceForObject( objectPosition, objectBounds );
		//			if( transform != null )
		//				CreateObjectControlledByPlayer( userItem, objectType, transform );
		//		}
		//	}

		//	//destroy when leave the world to World Entrance screen
		//	if( !userItem.EnteredToWorld && userItem.ObjectControlledByPlayer != null )
		//	{
		//		userItem.ObjectControlledByPlayer.RemoveFromParent( true );
		//		userItem.ObjectControlledByPlayer = null;
		//	}

		//	//base.OnUpdateObjectControlledByPlayer( userItem );
		//}

		void ServerOrSingle_ResetSpawnPoints()
		{
			if( ParentScene != null )
			{
				foreach( var spawnPoint in ParentScene.GetComponents<SpawnPoint>() )
					spawnPoint.ResetLastSpawnData();
			}
		}

		private void ServerOrSingleShooterGameLogic_CurrentGameStatusChanged( ShooterGameLogic gameLogic )
		{
			//reset spawn points
			if( EnabledInHierarchyAndIsInstance )
				ServerOrSingle_ResetSpawnPoints();

			if( CurrentGameStatus.Value == GameStatusEnum.Play )
			{
				//game started

				//destroy all characters, vehicles, weapons, bullets
				if( ParentScene != null )
				{
					foreach( var obj in ParentScene.GetComponents( checkChildren: true ) )
					{
						if( obj is Vehicle || obj is Character || obj is Weapon || obj is Bullet || obj is Character2D || obj is Weapon2D )
							obj.RemoveFromParent( true );
					}
				}

				ServerOrSingle_UpdateObjectControlledByPlayers();
			}
			else
			{
				//game ended
			}
		}

		void ServerOrSingle_TickGame()
		{
			var currentGameStatus = CurrentGameStatus.Value;
			var previousGameStatus = currentGameStatus;
			var currentGameTime = CurrentGameTime.Value;

			currentGameTime += Time.SimulationDelta;

			if( currentGameStatus == GameStatusEnum.Prepare )
			{
				if( currentGameTime > PreparationTime.Value )
				{
					//game start

					//check can start
					bool canStart = true;
					{
						var demandedPlayerCount = 1;
						//!!!!
						//if( GameType.Value == ShooterGameTypeEnum.BattleRoyale )
						//	demandedPlayerCount = 2;

						if( NetworkIsServer )
						{
							if( Server_GetUsers().Length < demandedPlayerCount )
								canStart = false;
						}
						else
						{
							if( Single_GetUsers().Length < demandedPlayerCount )
								canStart = false;
						}
					}

					if( canStart )
					{
						foreach( ShooterServerUserItem userItem in Server_GetUsers() )
							userItem.Frags = 0;
						sendPlayersInfoToClientsRemainingTime = 0;
						singleFrags.Clear();

						currentGameStatus = GameStatusEnum.Play;
						currentGameTime = 0;
					}
					else
					{
						currentGameStatus = GameStatusEnum.Prepare;
						currentGameTime = 0;
					}
				}
			}
			else if( currentGameStatus == GameStatusEnum.Play )
			{
				//game end

				var end = false;
				if( currentGameTime > GameTime.Value )
					end = true;

				//!!!!
				//if( GameType.Value == ShooterGameTypeEnum.BattleRoyale )
				//{
				//	var playersInGame = ServerGetUsers().Count( u => u.ObjectControlledByPlayer != null );
				//	if( currentGameTime > 10 && playersInGame < 2 )
				//		end = true;
				//}

				if( end )
				{
					currentGameStatus = GameStatusEnum.Prepare;
					currentGameTime = 0;
				}
			}

			if( previousGameStatus != currentGameStatus )
				Server_SendUsersInfoToClients();

			CurrentGameStatus = currentGameStatus;
			CurrentGameTime = currentGameTime;
		}

		public override NeoAxis.Component ServerOrSingle_CreateObjectControlledByPlayer( ServerUserItem serverUserItem, SingleUserItem singleUserItem, Reference<Metadata.TypeInfo> objectTypeWithReference, Transform transform )
		{
			//cloud specific: check for spectator
			if( serverUserItem != null && CloudPlayers != null )
			{
				if( Array.IndexOf( CloudPlayers, serverUserItem.User.UserID ) == -1 )
					return null;
			}

			var obj = base.ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, objectTypeWithReference, transform );

			if( obj != null )
			{
				var character = obj as Character;

				//initial health
				ObjectEx.PropertySet( obj, "Health", ObjectControlledByPlayerHealth.Value );

				//initial weapon
				if( character != null )
				{
					var weaponTypeWithReference = ObjectControlledByPlayerWeapon;//.Value;
					if( weaponTypeWithReference.Value != null )
					{
						var weapon = ParentScene.CreateComponent<Weapon>( enabled: false, setUniqueName: true );
						weapon.WeaponType = weaponTypeWithReference;
						weapon.NewObjectSetDefaultConfiguration();
						character.ItemTake( null, weapon );
						weapon.Enabled = true;
					}
				}

				//add CharacterAI to bot
				if( character != null )
				{
					var bot = false;
					if( serverUserItem != null )
						bot = serverUserItem.User.Bot;
					if( singleUserItem != null )
						bot = singleUserItem.Bot;
					if( bot )
					{
						var ai = character.CreateComponent<CharacterAI>();
						ai.CombatMode = true;
						ai.DebugVisualization = debugVisualization;

						//!!!!
						ai.AllowRun = false;
					}
				}

				//set team
				if( character != null )
				{
					if( GameType.Value == GameTypeEnum.FreeForAll )
					{
						freeForAllTeamCounter++;
						character.Team = freeForAllTeamCounter;
					}

					//!!!!other game types
				}
			}

			return obj;
		}

		ServerNetworkService_Users Server_GetUsersService()
		{
			var serverNode = ParentRoot.HierarchyController?.NetworkServerNode;
			if( serverNode != null )
				return serverNode.GetService<ServerNetworkService_Users>();
			return null;
		}

		private void Character_ProcessDamageAfterAll( Character sender, long whoFired, float damage, object anyData, double oldHealth )
		{
			//detect frags
			//process events only for the current scene
			if( ParentRoot == sender.ParentRoot )
			{
				//check the game is playing
				if( CurrentGameStatus.Value == GameStatusEnum.Play )
				{
					//add frags to the player who fired the shot if the character was killed
					if( oldHealth > 0 && sender.Health.Value <= 0 )
					{
						//server mode
						if( NetworkIsServer )
						{
							var killedUser = Server_GetUserByObjectControlled( sender );
							if( killedUser != null )
							{
								var usersService = Server_GetUsersService();

								var whoFiredUser = usersService?.GetUser( whoFired );
								if( whoFiredUser != null )
								{
									var whoFiredUserItem = Server_GetUser( whoFiredUser );
									if( whoFiredUserItem != null )
									{
										var addFrags = true;

										//!!!!
										//if( GameType.Value == GameTypeEnum.TeamDeathmatch )
										//{
										//	var whoFiredUser2 = ServerGetUser( whoFiredUser );
										//	if( whoFiredUser2 != null && whoFiredUser2.Team == creatorUserItem.Team )
										//		addFlags = false;
										//}

										if( addFrags )
										{
											if( whoFiredUser != killedUser )
											{
												//add frag to the player
												whoFiredUserItem.Frags++;
											}
											else
											{
												//kill yourself
												if( SubtractFragForSuicide )
													whoFiredUserItem.Frags--;
											}
										}
									}
								}
							}
						}

						//single mode
						if( NetworkIsSingle )
						{
							var killedUser = Single_GetUserByObjectControlled( sender );
							if( killedUser != null )
							{
								var whoFiredUser = Single_GetUser( whoFired );
								if( whoFiredUser != null )
								{
									var addFrags = true;

									//!!!!
									//if( GameType.Value == GameTypeEnum.TeamDeathmatch )
									//{
									//	var whoFiredUser2 = ServerGetUser( whoFiredUser );
									//	if( whoFiredUser2 != null && whoFiredUser2.Team == creatorUserItem.Team )
									//		addFlags = false;
									//}

									if( addFrags )
									{
										if( whoFiredUser != killedUser )
										{
											//add frag to the player
											singleFrags.TryGetValue( whoFired, out var frags );
											singleFrags[ whoFired ] = frags + 1;
										}
										else
										{
											//kill yourself
											if( SubtractFragForSuicide )
											{
												singleFrags.TryGetValue( whoFired, out var frags );
												singleFrags[ whoFired ] = frags - 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			//visualize damage in single mode
			if( NetworkIsSingle )
			{
				var damagedUser = Single_GetUserByObjectControlled( sender );
				if( damagedUser != null && !damagedUser.Bot )
					PlayerDamagedEffectTime = EngineApp.EngineTime;
			}

			//send message to damaged player to visualize damage
			if( NetworkIsServer )
			{
				var user = Server_GetUserByObjectControlled( sender );
				if( user != null && !user.Bot )
				{
					var m = BeginNetworkMessage( user, "VisualizeDamage" );
					if( m != null )
						m.End();
				}
			}
		}

		public int Single_GetFrags( long userID )
		{
			singleFrags.TryGetValue( userID, out var frags );
			return frags;
		}


		///////////////////////////////////////////////
		// Client

		//it is synchronized via a network message of the component. OnReceiveNetworkMessageFromServer method.
		public class ClientUserItem
		{
			public long UserID;
			public string Username;
			public bool Bot;
			public int Frags;
			public int Team;
		}
		Dictionary<long, ClientUserItem> clientUsers = new Dictionary<long, ClientUserItem>();

		///////////////////////////////////////////////

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "UsersInfo" )
			{
				var count = reader.ReadVariableInt32();
				var newList = new Dictionary<long, ClientUserItem>( count );
				for( int n = 0; n < count; n++ )
				{
					var item = new ClientUserItem();
					item.UserID = reader.ReadVariableInt64();
					item.Username = reader.ReadString();
					item.Bot = reader.ReadBoolean();
					item.Frags = reader.ReadVariableInt32();
					item.Team = reader.ReadVariableInt32();
					newList[ item.UserID ] = item;
				}
				if( !reader.Complete() )
					return false;

				clientUsers = newList;
			}

			if( message == "VisualizeDamage" )
			{
				if( !reader.Complete() )
					return false;
				PlayerDamagedEffectTime = EngineApp.EngineTime;
			}

			return true;
		}

		public ReadOnlyDictionary<long, ClientUserItem> Client_GetUsers()
		{
			return clientUsers.AsReadOnly();
		}

		public ClientUserItem Client_GetUser( long userID )
		{
			if( clientUsers.TryGetValue( userID, out var item ) )
				return item;
			return null;
		}

		public void Client_SendRequestAddBot()
		{
			//!!!!? check can send only if the user is admin

			var m = BeginNetworkMessageToServer( "AddBot" );
			if( m != null )
				m.End();
		}

		public void Client_SendRequestDeleteBot( long userID )
		{
			//!!!!? check can send only if the user is admin

			var m = BeginNetworkMessageToServer( "DeleteBot" );
			if( m != null )
			{
				m.Writer.WriteVariableInt64( userID );
				m.End();
			}
		}

		public void DebugVisualizationSwitch()
		{
			//change mode
			debugVisualization = !debugVisualization;

			//update all CharacterAI components in the scene
			foreach( var ai in ParentScene.GetComponents<CharacterAI>( checkChildren: true ) )
				ai.DebugVisualization = debugVisualization;
		}
	}
}