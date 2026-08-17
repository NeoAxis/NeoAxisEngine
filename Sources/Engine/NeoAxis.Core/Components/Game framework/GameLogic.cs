// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// A component is intended to implement the logic of the scene, including network synchronization.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Game Logic", -5002 )]
	public class GameLogic : Component
	{
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
		public event Action<GameLogic> ObjectTypeControlledByPlayerChanged;
		ReferenceField<Metadata.TypeInfo> _objectTypeControlledByPlayer = new Reference<Metadata.TypeInfo>( null, "NeoAxis.Character" );

		///////////////////////////////////////////////
		// Common

		//!!!!optimize
		[Browsable( false )]
		public Scene ParentScene
		{
			get { return ParentRoot as Scene; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

#if !CLIENT
			if( NetworkIsServer || NetworkIsSingle )
				ServerOrSingle_OnEnabledInHierarchyChanged();
#endif
			if( NetworkIsClient )
				Client_OnEnabledInHierarchyChanged();
		}

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

#if !CLIENT
			if( NetworkIsServer || NetworkIsSingle )
				ServerOrSingle_OnEnabledInSimulation();
#endif
			if( NetworkIsClient )
				Client_OnEnabledInSimulation();
		}

		protected override void OnDisabledInSimulation()
		{
			base.OnDisabledInSimulation();

#if !CLIENT
			if( NetworkIsServer || NetworkIsSingle )
				ServerOrSingle_OnDisabledInSimulation();
#endif
			if( NetworkIsClient )
				Client_OnDisabledInSimulation();
		}


		///////////////////////////////////////////////
		// Server, Single

		float serverOrSingleUpdateRemainingTime;
		SpawnPoint[] spawnPointsForPlayerCache; //cached only in simulation
		EDictionary<ServerNetworkService_Users.UserInfo, ServerUserItem> serverUsers = new EDictionary<ServerNetworkService_Users.UserInfo, ServerUserItem>();
		EDictionary<long, SingleUserItem> singleUsers = new EDictionary<long, SingleUserItem>();
		long singleBotCounter;

		///////////////////////////////////////////////

		public class ServerUserItem
		{
			public ServerNetworkService_Users.UserInfo User;
			public Component ObjectControlledByPlayer;
			public bool ObjectControlledByPlayerInputEnabled;
			public object AnyData;
		}

		///////////////////////////////////////////////

		public class SingleUserItem
		{
			public long UserID; //zero is player, other are bots
			public Component ObjectControlledByPlayer;
			public object AnyData;

			//

			public bool Bot
			{
				get { return UserID != 0; }
			}
		}

		///////////////////////////////////////////////

		public static GameLogic GetFromComponent( Component component )
		{
			return ( component.ParentRoot as Scene )?.GetGameLogic();
		}

		///////////////////////////////////////////////

		protected virtual void ServerOrSingle_OnEnabledInHierarchyChanged()
		{
			if( NetworkIsSingle && EnabledInHierarchyAndIsInstance )
				singleUsers[ 0 ] = new SingleUserItem();
		}

		protected virtual void ServerOrSingle_OnEnabledInSimulation() { }
		protected virtual void ServerOrSingle_OnDisabledInSimulation() { }

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

#if !CLIENT
			serverOrSingleUpdateRemainingTime -= Time.SimulationDelta;
			if( serverOrSingleUpdateRemainingTime < 0 )
			{
				serverOrSingleUpdateRemainingTime = 1;

				Server_UpdateUsersList();
				ServerOrSingle_UpdateObjectControlledByPlayers();
			}
#endif
		}

		protected virtual ServerUserItem Server_OnNewUserItem()
		{
			return new ServerUserItem();
		}

		protected virtual void Server_AddUser( ServerNetworkService_Users.UserInfo user )
		{
			var item = Server_OnNewUserItem();
			item.User = user;
			serverUsers[ user ] = item;
		}

		protected virtual void Server_RemoveUser( ServerUserItem item )
		{
			serverUsers.Remove( item.User );

			if( item.ObjectControlledByPlayer != null )
			{
				item.ObjectControlledByPlayer.RemoveFromParent( true );
				item.ObjectControlledByPlayerInputEnabled = false;
				item.ObjectControlledByPlayer = null;
			}
		}

		public ServerUserItem[] Server_GetUsers()
		{
			return serverUsers.Values.ToArray();
		}

		void Server_UpdateUsersList()
		{
#if !CLIENT
			var serverNode = ParentRoot.HierarchyController?.NetworkServerNode;
			if( serverNode != null )
			{
				var usersService = serverNode.GetService<ServerNetworkService_Users>();
				if( usersService != null )
				{
					//remove old users
					{
						var toRemove = new List<ServerUserItem>();
						foreach( var item in serverUsers.Values )
						{
							if( usersService.GetUser( item.User.UserID ) == null )
								toRemove.Add( item );
						}
						foreach( var item in toRemove )
							Server_RemoveUser( item );
					}

					//add new users
					foreach( var user in usersService.Users )
					{
						if( !serverUsers.ContainsKey( user ) )
							Server_AddUser( user );
					}
				}
			}
#endif
		}

		public delegate void ObjectControlledByPlayerCreatedDelegate( GameLogic sender, ServerUserItem serverUserItem, SingleUserItem singleUserItem, Component obj );
		public event ObjectControlledByPlayerCreatedDelegate ObjectControlledByPlayerCreated;

		public virtual Component ServerOrSingle_CreateObjectControlledByPlayer( ServerUserItem serverUserItem, SingleUserItem singleUserItem, Metadata.TypeInfo objectType, Transform transform )
		{
			if( ParentScene != null )
			{
				var obj = ParentScene.CreateComponent( objectType, enabled: false, setUniqueName: true );
				obj.NewObjectSetDefaultConfiguration();

				//add input processing component
				if( obj is Character character )
					obj.CreateComponent<CharacterInputProcessing>();
				else if( obj is Character2D character2D )
					obj.CreateComponent<Character2DInputProcessing>();
				else if( obj is Vehicle vehicle )
					obj.CreateComponent<VehicleInputProcessing>();

				obj.Enabled = true;

				//change rotation of the object to the direction of the spawn point
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

				if( serverUserItem != null )
				{
					//server mode user
					serverUserItem.ObjectControlledByPlayer = obj;
					serverUserItem.ObjectControlledByPlayerInputEnabled = true;
					Server_SendSetObjectControlledByPlayer( serverUserItem );
				}
				else if( singleUserItem != null )
				{
					//single mode user
					singleUserItem.ObjectControlledByPlayer = obj;

					if( singleUserItem.UserID == 0 )
					{
						var gameMode = ParentScene.GetGameMode();
						if( gameMode != null )
							gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( obj );
					}
				}

				ObjectControlledByPlayerCreated?.Invoke( this, serverUserItem, singleUserItem, obj );

				return obj;
			}

			return null;
		}

		public delegate void ObjectControlledByPlayerBeforeCreateDelegate( GameLogic sender, ServerUserItem serverUserItem, SingleUserItem singleUserItem, ref bool handled );
		public event ObjectControlledByPlayerBeforeCreateDelegate ServerOrSingle_ObjectControlledByPlayerBeforeCreate;

		protected virtual void ServerOrSingle_UpdateObjectControlledByPlayer( ServerUserItem serverUserItem, SingleUserItem singleUserItem )
		{
			//default behaviour of creation of object controlled by player. can be overridden in ObjectControlledByPlayerBeforeCreate event or in the override of this method

			//get object controlled by player is created
			bool objectControlledByPlayerIsCreated;
			if( serverUserItem != null )
				objectControlledByPlayerIsCreated = serverUserItem.ObjectControlledByPlayer != null;
			else if( singleUserItem != null )
				objectControlledByPlayerIsCreated = singleUserItem.ObjectControlledByPlayer != null;
			else
				objectControlledByPlayerIsCreated = true;

			//create new
			if( !objectControlledByPlayerIsCreated )
			{
				var handled = false;
				ServerOrSingle_ObjectControlledByPlayerBeforeCreate?.Invoke( this, serverUserItem, singleUserItem, ref handled );

				//default behaviour
				if( !handled )
				{
					var objectType = ObjectTypeControlledByPlayer.Value;
					if( objectType != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
					{
						var spawnPoint = GetSpawnPointForPlayer( serverUserItem, singleUserItem, objectType );
						if( spawnPoint != null )
						{
							if( MetadataManager.GetTypeOfNetType( typeof( Character ) ).IsAssignableFrom( objectType ) )
							{
								//Character

								if( ParentScene != null )
								{
									if( CharacterUtility.FindFreePlace( ParentScene, 2, 0.4, spawnPoint.TransformV.Position, 5, -1, 1, null, out var freePlacePosition ) )
									{
										var transform = spawnPoint.TransformV;
										transform = transform.UpdatePosition( freePlacePosition );
										ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, objectType, transform );
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
								ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, objectType, transform );
							}
							else
							{
								//!!!!other components check free place

								var transform = spawnPoint.TransformV;
								ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, objectType, transform );
							}
						}
					}
				}
			}
		}

		protected virtual bool ServerOrSingle_IsAllowToUpdateObjectControlledByPlayers()
		{
			if( NetworkIsServer )
				return true;

			return false;
		}

		public void ServerOrSingle_UpdateObjectControlledByPlayers()
		{
			if( ServerOrSingle_IsAllowToUpdateObjectControlledByPlayers() )
			{
				if( NetworkIsServer )
				{
					//server mode users
					foreach( var serverUserItem in serverUsers.Values )
					{
						if( serverUserItem.ObjectControlledByPlayer != null && serverUserItem.ObjectControlledByPlayer.Parent == null )
							serverUserItem.ObjectControlledByPlayer = null;
						ServerOrSingle_UpdateObjectControlledByPlayer( serverUserItem, null );
					}
				}

				if( NetworkIsSingle )
				{
					//single mode users
					foreach( var singleUserItem in singleUsers.Values )
					{
						if( singleUserItem.ObjectControlledByPlayer != null && singleUserItem.ObjectControlledByPlayer.Parent == null )
							singleUserItem.ObjectControlledByPlayer = null;

						if( singleUserItem.UserID == 0 )
						{
							var gameMode = ParentScene?.GetGameMode();
							if( gameMode != null )
							{
								if( gameMode.ObjectControlledByPlayer.Value != null && gameMode.ObjectControlledByPlayer.Value.Parent == null )
									gameMode.ObjectControlledByPlayer = null;
							}
						}

						ServerOrSingle_UpdateObjectControlledByPlayer( null, singleUserItem );
					}
				}
			}
		}

		public ServerUserItem Server_GetUser( ServerNetworkService_Users.UserInfo user )
		{
			if( user != null && serverUsers.TryGetValue( user, out var item ) )
				return item;
			return null;
		}

		public Component Server_GetObjectControlledByUser( ServerNetworkService_Users.UserInfo user )
		{
			var item = Server_GetUser( user );
			if( item != null )
			{
				if( item.ObjectControlledByPlayerInputEnabled )
					return item.ObjectControlledByPlayer;
			}
			return null;
		}

		public ServerNetworkService_Users.UserInfo Server_GetUserByObjectControlled( Component obj )
		{
			//!!!!slowly. dictionary

			if( obj != null )
			{
				foreach( var item in serverUsers.Values )
				{
					if( item.ObjectControlledByPlayer == obj )
					{
						if( item.ObjectControlledByPlayerInputEnabled )
							return item.User;
						else
							return null;
					}
				}
			}
			return null;
		}

		void Server_SendSetObjectControlledByPlayer( ServerUserItem item )
		{
			var referenceToObject = item.ObjectControlledByPlayer != null ? "root:" + item.ObjectControlledByPlayer.GetPathFromRoot() : "";

			//send update to the user
			var m = BeginNetworkMessage( item.User, "SetObjectControlledByPlayer" );
			if( m != null )
			{
				m.Writer.Write( referenceToObject );
				m.End();
			}

#if !CLIENT
			//send update to all users. can be optional
			var serverNode = ParentRoot.HierarchyController?.NetworkServerNode;
			if( serverNode != null )
			{
				var usersService = serverNode.GetService<ServerNetworkService_Users>();
				if( usersService != null )
					usersService.UpdateObjectControlledByPlayerToClient( item.User, referenceToObject );
			}
#endif
		}

		public void Server_ChangeObjectControlled( ServerNetworkService_Users.UserInfo user, Component obj )
		{
			var item = Server_GetUser( user );
			if( item != null )
			{
				item.ObjectControlledByPlayer = obj;
				item.ObjectControlledByPlayerInputEnabled = true;

				Server_SendSetObjectControlledByPlayer( item );
			}
		}

		public void Server_SendScreenMessageToClient( ServerNetworkService_Components.ClientItem client, string text, bool error )
		{
			var m = BeginNetworkMessage( client, "ScreenMessage" );
			if( m != null )
			{
				m.Writer.Write( text );
				m.Writer.Write( error );
				m.End();
			}
		}

		public void Server_SendScreenMessageToClientByControlledObject( Component controlledObject, string text, bool error )
		{
			var user = Server_GetUserByObjectControlled( controlledObject );
			if( user != null )
			{
				var m = BeginNetworkMessage( user, "ScreenMessage" );
				if( m != null )
				{
					m.Writer.Write( text );
					m.Writer.Write( error );
					m.End();
				}
			}
		}

		public void Server_SendScreenMessageToAllClients( string text, bool error )
		{
			var m = BeginNetworkMessageToEveryone( "ScreenMessage" );
			if( m != null )
			{
				m.Writer.Write( text );
				m.Writer.Write( error );
				m.End();
			}
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			return true;
		}

		protected virtual SpawnPoint GetSpawnPointForPlayer( ServerUserItem serverUserInfo, SingleUserItem singleUserItem, Metadata.TypeInfo objectType )
		{
			var array = spawnPointsForPlayerCache;
			if( spawnPointsForPlayerCache == null || !EngineApp.IsSimulation )
			{
				array = ParentRoot.GetComponents<SpawnPoint>( onlyEnabledInHierarchy: true ).Where( p => p.Mode.Value == SpawnPoint.ModeEnum.Player ).ToArray();
				spawnPointsForPlayerCache = array;
			}

			if( ParentScene != null )
			{
				var random = ParentScene.SceneRandom;
				var index = random.Next( array.Length );
				if( index >= 0 && index < array.Length )
					return array[ index ];
			}

			return null;
		}

		public void ResetSpawnPointsForPlayerCache()
		{
			spawnPointsForPlayerCache = null;
		}

		public long Single_AddBot()
		{
			var userID = Interlocked.Increment( ref singleBotCounter );

			var item = new SingleUserItem();
			item.UserID = userID;
			singleUsers[ userID ] = item;

			return userID;
		}

		public void Single_DeleteBot( long userID )
		{
			if( userID != 0 && singleUsers.TryGetValue( userID, out var item ) )
			{
				if( item.ObjectControlledByPlayer != null )
				{
					item.ObjectControlledByPlayer.RemoveFromParent( true );
					item.ObjectControlledByPlayer = null;
				}

				singleUsers.Remove( userID );
			}
		}

		public SingleUserItem[] Single_GetUsers()
		{
			return singleUsers.Values.ToArray();
		}

		public SingleUserItem Single_GetUserByObjectControlled( Component obj )
		{
			//!!!!slowly? dictionary

			if( obj != null )
			{
				foreach( var item in singleUsers.Values )
					if( item.ObjectControlledByPlayer == obj )
						return item;
			}
			return null;
		}

		public SingleUserItem Single_GetUser( long userID )
		{
			if( singleUsers.TryGetValue( userID, out var item ) )
				return item;
			return null;
		}


		///////////////////////////////////////////////
		// Client

		protected virtual void Client_OnEnabledInHierarchyChanged() { }

		protected virtual void Client_OnEnabledInSimulation()
		{
			//reset GameMode.ObjectControlledByPlayer on the client
			if( ParentScene != null )
			{
				var gameMode = ParentScene.GetGameMode();
				if( gameMode != null )
					gameMode.ObjectControlledByPlayer = null;
			}
		}

		protected virtual void Client_OnDisabledInSimulation() { }

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "SetObjectControlledByPlayer" )
			{
				var referenceToObject = reader.ReadString() ?? string.Empty;
				if( !reader.Complete() )
					return false;

				if( ParentScene != null )
				{
					var gameMode = ParentScene.GetGameMode();
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
				var text = reader.ReadString() ?? string.Empty;
				var error = reader.ReadBoolean();
				if( !reader.Complete() )
					return false;

				ScreenMessages.Add( text, error );
			}

			return true;
		}
	}
}