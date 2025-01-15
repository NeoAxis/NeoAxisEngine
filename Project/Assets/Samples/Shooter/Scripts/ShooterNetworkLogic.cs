// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NeoAxis;

namespace Project
{
	public class ShooterNetworkLogic : NetworkLogic
	{
		//common properties. some of them are synchronized

		[DefaultValue( ShooterGameTypeEnum.FreeForAll )]
		public Reference<ShooterGameTypeEnum> GameType
		{
			get { if( _gameType.BeginGet() ) GameType = _gameType.Get( this ); return _gameType.value; }
			set { if( _gameType.BeginSet( this, ref value ) ) { try { GameTypeChanged?.Invoke( this ); } finally { _gameType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GameType"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> GameTypeChanged;
		ReferenceField<ShooterGameTypeEnum> _gameType = ShooterGameTypeEnum.FreeForAll;

		[DefaultValue( 30 )]
		public Reference<double> PreparationTime
		{
			get { if( _preparationTime.BeginGet() ) PreparationTime = _preparationTime.Get( this ); return _preparationTime.value; }
			set { if( _preparationTime.BeginSet( this, ref value ) ) { try { PreparationTimeChanged?.Invoke( this ); } finally { _preparationTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreparationTime"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> PreparationTimeChanged;
		ReferenceField<double> _preparationTime = 30;

		[DefaultValue( 120 )]
		public Reference<double> GameTime
		{
			get { if( _gameTime.BeginGet() ) GameTime = _gameTime.Get( this ); return _gameTime.value; }
			set { if( _gameTime.BeginSet( this, ref value ) ) { try { GameTimeChanged?.Invoke( this ); } finally { _gameTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GameTime"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> GameTimeChanged;
		ReferenceField<double> _gameTime = 120;

		[DefaultValue( 10.0 )]
		public Reference<double> ObjectControlledByPlayerHealth
		{
			get { if( _objectControlledByPlayerHealth.BeginGet() ) ObjectControlledByPlayerHealth = _objectControlledByPlayerHealth.Get( this ); return _objectControlledByPlayerHealth.value; }
			set { if( _objectControlledByPlayerHealth.BeginSet( this, ref value ) ) { try { ObjectControlledByPlayerHealthChanged?.Invoke( this ); } finally { _objectControlledByPlayerHealth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectControlledByPlayerHealth"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> ObjectControlledByPlayerHealthChanged;
		ReferenceField<double> _objectControlledByPlayerHealth = 10.0;

		/// <summary>
		/// Whether to save original weapons position at loading and recreate them at game start.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SpawnWeapons
		{
			get { if( _spawnWeapons.BeginGet() ) SpawnWeapons = _spawnWeapons.Get( this ); return _spawnWeapons.value; }
			set { if( _spawnWeapons.BeginSet( this, ref value ) ) { try { SpawnWeaponsChanged?.Invoke( this ); } finally { _spawnWeapons.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpawnWeapons"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> SpawnWeaponsChanged;
		ReferenceField<bool> _spawnWeapons = true;

		/// <summary>
		/// Whether to delete Character components after scene loading in the simulation.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> RemoveInitialCharactersOnServer
		{
			get { if( _removeInitialCharactersOnServer.BeginGet() ) RemoveInitialCharactersOnServer = _removeInitialCharactersOnServer.Get( this ); return _removeInitialCharactersOnServer.value; }
			set { if( _removeInitialCharactersOnServer.BeginSet( this, ref value ) ) { try { RemoveInitialCharactersOnServerChanged?.Invoke( this ); } finally { _removeInitialCharactersOnServer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RemoveInitialCharactersOnServer"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> RemoveInitialCharactersOnServerChanged;
		ReferenceField<bool> _removeInitialCharactersOnServer = true;

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( true )]
		[DefaultValue( ShooterGameStatusEnum.Preparing )]
		public Reference<ShooterGameStatusEnum> CurrentGameStatus
		{
			get { if( _currentGameStatus.BeginGet() ) CurrentGameStatus = _currentGameStatus.Get( this ); return _currentGameStatus.value; }
			set { if( _currentGameStatus.BeginSet( this, ref value ) ) { try { CurrentGameStatusChanged?.Invoke( this ); } finally { _currentGameStatus.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentGameStatus"/> property value changes.</summary>
		public event Action<ShooterNetworkLogic> CurrentGameStatusChanged;
		ReferenceField<ShooterGameStatusEnum> _currentGameStatus = ShooterGameStatusEnum.Preparing;

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
		public event Action<ShooterNetworkLogic> CurrentGameTimeChanged;
		ReferenceField<float> _currentGameTime = 0.0f;

		/////////////////////////////////////////

		public double GetRemainingTime()
		{
			if( CurrentGameStatus == ShooterGameStatusEnum.Preparing )
				return PreparationTime - CurrentGameTime;
			else
				return GameTime - CurrentGameTime;
		}

		public string GetGameTextStatus()
		{
			var text = "";

			switch( CurrentGameStatus.Value )
			{
			case ShooterGameStatusEnum.Preparing:
				text = string.Format( "The game will start in {0} seconds.", (int)GetRemainingTime() );
				break;

			case ShooterGameStatusEnum.Playing:
				text = string.Format( "The game will end in {0} seconds.", (int)GetRemainingTime() );
				break;
			}

			return text;
		}


		/////////////////////////////////////////
		/////////////////////////////////////////
		/////////////////////////////////////////
		//server only
#if !CLIENT

		double sendPlayersInfoToClientsRemainingTime;

		List<SpawnWeapon> spawnWeapons = new List<SpawnWeapon>();

		/////////////////////////////////////////

		public class ShooterServerUserItem : ServerUserItem
		{
			public int Points;//frags
			public int Team;
			//add more here
		}

		/////////////////////////////////////////

		public class SpawnWeapon
		{
			public Weapon ClonedObject;
			public Transform Transform;
		}

		/////////////////////////////////////////

		protected override void ServerOnEnabledInHierarchyChanged()
		{
			base.ServerOnEnabledInHierarchyChanged();

			if( NetworkIsServer )
			{
				if( EnabledInHierarchyAndIsInstance )
					Character.ProcessDamageAfterAll += Character_ProcessDamageAfterAll;
				else
					Character.ProcessDamageAfterAll -= Character_ProcessDamageAfterAll;
			}
		}

		protected override void ServerOnEnabledInSimulation()
		{
			base.ServerOnEnabledInSimulation();

			//update EntranceScreenDescription
			{
				var text = "This is a template for creating shooters. Various modes are available such as Free for all, Team deathmatch and Battle royale.";
				text += "\n\nUse F1 or Enter to open in-game menu.";

				EntranceScreenDescription = text;
			}

			//save the initial state of weapons
			if( SpawnWeapons )
			{
				var scene = ParentRoot as Scene;
				if( scene != null )
				{
					foreach( var weapon in scene.GetComponents<Weapon>() )
					{
						var item = new SpawnWeapon();
						item.ClonedObject = (Weapon)weapon.Clone();
						item.ClonedObject.Enabled = false;
						item.Transform = weapon.Transform;

						spawnWeapons.Add( item );
					}
				}
			}

			//delete characters
			if( RemoveInitialCharactersOnServer && NetworkIsServer )
			{
				var scene = ParentRoot as Scene;
				if( scene != null )
				{
					foreach( var character in scene.GetComponents<Character>() )
						character.RemoveFromParent( false );
				}
			}

			CurrentGameStatusChanged += ServerShooterNetworkLogic_CurrentGameStatusChanged;
		}

		protected override ServerUserItem ServerOnNewUserItem()
		{
			return new ShooterServerUserItem();
		}

		public int GetTeamWithSmallestAmountOfPlayers()
		{
			var table = new int[ 2 ];

			foreach( var user in ServerGetUsers() )
			{
				var user2 = (ShooterServerUserItem)user;
				if( user2.Team >= 0 && user2.Team < table.Length )
					table[ user2.Team ]++;
			}

			int minIndex = Array.IndexOf( table, table.Min() );
			return minIndex;
		}

		protected override void AddUser( ServerNetworkService_Users.UserInfo user )
		{
			var team = 0;
			if( GameType.Value == ShooterGameTypeEnum.TeamDeathmatch )
				team = GetTeamWithSmallestAmountOfPlayers();

			base.AddUser( user );

			//select a team for the user
			if( GameType.Value == ShooterGameTypeEnum.TeamDeathmatch )
			{
				var user2 = ServerGetUser( user );
				user2.Team = team;
			}
		}

		public new ShooterServerUserItem ServerGetUser( ServerNetworkService_Users.UserInfo user )
		{
			return (ShooterServerUserItem)base.ServerGetUser( user );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( NetworkIsServer )
			{
				TickGame();

				//!!!!optimization. send only when changed
				sendPlayersInfoToClientsRemainingTime -= Time.SimulationDelta;
				if( sendPlayersInfoToClientsRemainingTime < 0 )
				{
					sendPlayersInfoToClientsRemainingTime = 1;
					SendUsersInfoToClients();
				}
			}
		}

		public bool ProcessReceivedNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			//if( message == "DeleteBot" )
			//{
			//}

			return true;
		}

		protected override void OnClientDisconnected( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientDisconnected( client );
		}

		void SendUsersInfoToClients()
		{
			var writer = BeginNetworkMessageToEveryone( "UsersInfo" );
			if( writer != null )
			{
				var users = ServerGetUsers();

				writer.WriteVariableInt32( users.Length );
				foreach( ShooterServerUserItem item in users )
				{
					writer.WriteVariableUInt64( (ulong)item.User.UserID );
					writer.WriteVariableInt32( item.Points );
					writer.WriteVariableInt32( item.Team );
				}
				EndNetworkMessage();
			}
		}

		protected override SpawnPoint OnGetSpawnPoint( ServerUserItem userInfo, Metadata.TypeInfo objectType )
		{
			//override default behavior
			if( GameType.Value == ShooterGameTypeEnum.TeamDeathmatch )
			{
				var user = (ShooterServerUserItem)userInfo;

				var array = GetSpawnPointsForTeam( user.Team );

				var random = new FastRandom();
				var index = random.Next( array.Length );
				if( index >= 0 && index < array.Length )
					return array[ index ];
			}

			return base.OnGetSpawnPoint( userInfo, objectType );
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

		void RespawnWeapons()
		{
			var scene = ParentRoot as Scene;
			if( scene != null )
			{
				//delete old weapons
				foreach( var weapon in scene.GetComponents<Weapon>() )
					weapon.RemoveFromParent( true );

				//create new
				foreach( var item in spawnWeapons )
				{
					var weapon = (Weapon)item.ClonedObject.Clone();
					scene.AddComponent( weapon );

					weapon.Transform = item.Transform;
					weapon.Enabled = true;
				}
			}
		}

		private void ServerShooterNetworkLogic_CurrentGameStatusChanged( ShooterNetworkLogic obj )
		{
			if( CurrentGameStatus.Value == ShooterGameStatusEnum.Playing )
			{
				//game started

				//destroy all characters and vehicles
				var scene = ParentRoot as Scene;
				if( scene != null )
				{
					foreach( var vehicle in scene.GetComponents<Vehicle>() )
						vehicle.RemoveFromParent( true );
					foreach( var character in scene.GetComponents<Character>() )
						character.RemoveFromParent( true );
				}

				//recreate initial weapons
				if( SpawnWeapons && EnabledInHierarchyAndIsInstance )
					RespawnWeapons();

				UpdateObjectControlledByPlayers();
			}
			else
			{
				//game ended
			}
		}

		void TickGame()
		{
			var currentGameStatus = CurrentGameStatus.Value;
			var currentGameTime = CurrentGameTime.Value;

			currentGameTime += Time.SimulationDelta;

			if( currentGameStatus == ShooterGameStatusEnum.Preparing )
			{
				if( currentGameTime > PreparationTime.Value )
				{
					//game start

					//check can start
					bool canStart = true;
					{
						var demandedPlayerCount = 1;
						if( GameType.Value == ShooterGameTypeEnum.BattleRoyale )
							demandedPlayerCount = 2;

						var playerCount = ServerGetUsers().Length;
						if( playerCount < demandedPlayerCount )
							canStart = false;
					}

					if( canStart )
					{
						foreach( ShooterServerUserItem userItem in ServerGetUsers() )
							userItem.Points = 0;
						sendPlayersInfoToClientsRemainingTime = 0;

						currentGameStatus = ShooterGameStatusEnum.Playing;
						currentGameTime = 0;
					}
					else
					{
						currentGameStatus = ShooterGameStatusEnum.Preparing;
						currentGameTime = 0;
					}
				}
			}
			else if( currentGameStatus == ShooterGameStatusEnum.Playing )
			{
				//game end

				var end = false;
				if( currentGameTime > GameTime.Value )
					end = true;
				if( GameType.Value == ShooterGameTypeEnum.BattleRoyale )
				{
					var playersInGame = ServerGetUsers().Count( u => u.ObjectControlledByPlayer != null );
					if( currentGameTime > 10 && playersInGame < 2 )
						end = true;
				}

				if( end )
				{
					currentGameStatus = ShooterGameStatusEnum.Preparing;
					currentGameTime = 0;
				}
			}

			CurrentGameStatus = currentGameStatus;
			CurrentGameTime = currentGameTime;
		}

		public override NeoAxis.Component CreateObjectControlledByPlayer( ServerUserItem userItem, Metadata.TypeInfo objectType, Transform transform )
		{
			var obj = base.CreateObjectControlledByPlayer( userItem, objectType, transform );

			if( obj != null )
				ObjectEx.PropertySet( obj, "Health", ObjectControlledByPlayerHealth.Value );

			return obj;
		}

		private void Character_ProcessDamageAfterAll( Character sender, long whoFired, float damage, object anyData, double oldHealth )
		{
			//process events of only our scene
			if( ParentRoot == sender.ParentRoot )
			{
				//when we in a game
				if( CurrentGameStatus.Value == ShooterGameStatusEnum.Playing )
				{
					//add player's point
					if( oldHealth > 0 && sender.Health.Value <= 0 )
					{
						var whoFiredUser = SimulationAppServer.Server.Users.GetUser( whoFired );
						if( whoFiredUser != null )
						{
							var creatorUserItem = ServerGetUser( whoFiredUser );
							if( creatorUserItem != null )
							{
								var addPoints = true;
								if( GameType.Value == ShooterGameTypeEnum.TeamDeathmatch )
								{
									var whoFiredUser2 = ServerGetUser( whoFiredUser );
									if( whoFiredUser2 != null && whoFiredUser2.Team == creatorUserItem.Team )
										addPoints = false;
								}

								if( addPoints )
								{
									//add points to the player
									creatorUserItem.Points++;
								}
							}
						}
					}
				}
			}
		}


#endif


		/////////////////////////////////////////
		/////////////////////////////////////////
		/////////////////////////////////////////
		//client only
		//#if CLIENT //the define must be enabled, but for code editing is better to disable it

		//it is synchronized via a network message of the component. OnReceiveNetworkMessageFromServer method.
		public class ClientUserItem
		{
			public long UserID;
			public int Points;//frags
			public int Team;
		}
		Dictionary<long, ClientUserItem> clientUsers = new Dictionary<long, ClientUserItem>();

		/////////////////////////////////////////

		protected override void ClientOnEnabledInSimulation()
		{
			base.ClientOnEnabledInSimulation();

			var scene = ParentRoot as Scene;
			if( scene != null )
				scene.RenderEvent += Scene_RenderEvent;

			CurrentGameStatusChanged += ClientShooterNetworkLogic_CurrentGameStatusChanged;
		}

		private void ClientShooterNetworkLogic_CurrentGameStatusChanged( ShooterNetworkLogic obj )
		{
			if( CurrentGameStatus.Value == ShooterGameStatusEnum.Playing )
			{
				ScreenMessages.Add( "The game has started!" );

				var scene = ParentRoot as Scene;
				if( scene != null )
					scene.SoundPlay2D( @"Samples\Shooter\Sounds\Game started.ogg" );
			}
			else
			{
				ScreenMessages.Add( "The game has ended." );

				var scene = ParentRoot as Scene;
				if( scene != null )
					scene.SoundPlay2D( @"Samples\Shooter\Sounds\Game ended.ogg" );
			}
		}

		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			//if( NetworkIsClient )
			//{
			//	//restore chairs
			//	if( changedChair != null )
			//	{
			//		changedChair.SpecialEffects = new List<ObjectSpecialRenderingEffect>();

			//		//changedChair.ReplaceMaterial = null;
			//		//changedChair.Color = new ColorValue( 1, 1, 1 );
			//		changedChair = null;
			//	}

			//	{
			//		var thisUser = WorldClientManager.Client.Users.ThisUser;

			//		var game = ShooterSceneScreen.GetGame();
			//		//var game = syncedManager.GetComponent<BoardGame>();
			//		if( game != null )//&& game.PlayerExists( WorldClientManager.Client.Users.ThisUser.UserID ) )
			//		{
			//			var playerIndex = game.GetPlayerIndex( thisUser.UserID );
			//			if( playerIndex != -1 && !game.PlayersOnSeat.Value[ playerIndex ] )
			//			{
			//				var gamePlace = game.GamePlace.Value;
			//				if( gamePlace != null )
			//				{
			//					var chair = gamePlace.GetChair( playerIndex );
			//					if( chair != null )
			//					{
			//						var effects = new List<ObjectSpecialRenderingEffect>();

			//						var outline = new ObjectSpecialRenderingEffect_Outline();
			//						outline.Color = new ColorValue( 0.05, 0.95, 0.05 );
			//						outline.Scale = 0.5 + ( Math.Cos( Time.Current * 2 ) + 1 ) / 2 * 2;
			//						effects.Add( outline );

			//						chair.SpecialEffects = effects;

			//						//chair.ReplaceMaterial = new Reference<Material>( null, @"Shooter\Materials\HighlightObject.material" );
			//						//chair.Color = new ColorValue( 2, 2, 2 );

			//						changedChair = chair;
			//					}

			//				}
			//			}

			//		}
			//	}
			//}
		}

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
					item.UserID = (long)reader.ReadVariableUInt64();
					item.Points = reader.ReadVariableInt32();
					item.Team = reader.ReadVariableInt32();
					newList[ item.UserID ] = item;
				}

				if( !reader.Complete() )
					return false;

				clientUsers = newList;
			}
			else if( message == "PlayerKilled" )
			{
				//!!!!


			}

			return true;
		}

		public ICollection<ClientUserItem> ClientGetUsers()
		{
			return clientUsers.Values;
		}


		//#endif

	}
}