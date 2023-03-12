// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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
			set { if( _entranceScreen.BeginSet( ref value ) ) { try { EntranceScreenChanged?.Invoke( this ); } finally { _entranceScreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EntranceScreen"/> property value changes.</summary>
		public event Action<NetworkLogic> EntranceScreenChanged;
		ReferenceField<bool> _entranceScreen = true;

		[DefaultValue( "A description of your world." )]
		public Reference<string> EntranceScreenDescription
		{
			get { if( _entranceScreenDescription.BeginGet() ) EntranceScreenDescription = _entranceScreenDescription.Get( this ); return _entranceScreenDescription.value; }
			set { if( _entranceScreenDescription.BeginSet( ref value ) ) { try { EntranceScreenDescriptionChanged?.Invoke( this ); } finally { _entranceScreenDescription.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EntranceScreenDescription"/> property value changes.</summary>
		public event Action<NetworkLogic> EntranceScreenDescriptionChanged;
		ReferenceField<string> _entranceScreenDescription = "A description of your world.";

		/// <summary>
		/// Whether to use avatars. Your character will be changed by your avatar settings. Entrance screen will have the ability to configure avatar.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Avatars
		{
			get { if( _avatars.BeginGet() ) Avatars = _avatars.Get( this ); return _avatars.value; }
			set { if( _avatars.BeginSet( ref value ) ) { try { AvatarsChanged?.Invoke( this ); } finally { _avatars.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Avatars"/> property value changes.</summary>
		public event Action<NetworkLogic> AvatarsChanged;
		ReferenceField<bool> _avatars = true;

		/// <summary>
		/// The type of object to control by the player.
		/// </summary>
		[DefaultValueReference( "NeoAxis.Character" )]
		[NetworkSynchronize( false )]
		public Reference<Metadata.TypeInfo> ObjectTypeControlledByPlayer
		{
			get { if( _objectTypeControlledByPlayer.BeginGet() ) ObjectTypeControlledByPlayer = _objectTypeControlledByPlayer.Get( this ); return _objectTypeControlledByPlayer.value; }
			set { if( _objectTypeControlledByPlayer.BeginSet( ref value ) ) { try { ObjectTypeControlledByPlayerChanged?.Invoke( this ); } finally { _objectTypeControlledByPlayer.EndSet(); } } }
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
		//SpawnPoint[] spawnPointsCache;

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

		//!!!!
		//protected override void OnComponentAdded( Component component )
		//{
		//	base.OnComponentAdded( component );

		//	они же в сцене, не тут
		//	if( component is SpawnPoint )
		//		spawnPointsCache = null;
		//}

		//!!!!
		//protected override void OnComponentRemoved( Component component )
		//{
		//	base.OnComponentRemoved( component );

		//	они же в сцене, не тут
		//	if( component is SpawnPoint )
		//		spawnPointsCache = null;
		//}

		bool IsFreePlace( Bounds bounds )
		{
			var scene = ParentRoot as Scene;
			if( scene != null )
			{
				var contactTestItem = new PhysicsVolumeTestItem( bounds, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneForEach*/ );
				scene.PhysicsVolumeTest( contactTestItem );

				foreach( var item in contactTestItem.Result )
				{
					//what to skip?
					//if( item.Body == mainBody )
					//	continue;

					return false;

					//var body = item.Body as RigidBody;
					//if( body != null )
					//	return false;
				}
			}

			return true;
		}

		Transform GetFreePlaceForObjectDefault( Vector3 objectPosition, Bounds objectBounds )
		{
			if( IsFreePlace( objectBounds + objectPosition ) )
				return new Transform( objectPosition );

			var maxRadius = objectBounds.GetSize().X * 15;
			var radiusStep = objectBounds.GetSize().X;

			for( var radius = radiusStep; radius < maxRadius; radius += radiusStep )
			{
				for( var angle = 0.0; angle < Math.PI * 2; angle += Math.PI / 8 )
				{
					var maxHeight = objectBounds.GetSize().Z / 2;

					for( var height = 0.0; height < maxHeight; height += maxHeight / 4 )
					{
						var pos = objectPosition + new Vector3( Math.Cos( angle ) * radius, Math.Sin( angle ) * radius, height );

						if( IsFreePlace( objectBounds + pos ) )
							return new Transform( objectPosition );
					}
				}
			}

			return null;
		}

		public delegate void GetFreePlaceForObjectEventDelegate( NetworkLogic sender, Vector3 objectPosition, Bounds objectBounds, ref Transform transform );
		public event GetFreePlaceForObjectEventDelegate GetFreePlaceForObjectEvent;

		public virtual Transform GetFreePlaceForObject( Vector3 objectPosition, Bounds objectBounds )
		{
			Transform result = null;
			GetFreePlaceForObjectEvent?.Invoke( this, objectPosition, objectBounds, ref result );
			if( result != null )
				return result;

			return GetFreePlaceForObjectDefault( objectPosition, objectBounds );
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

		//!!!!
		TextBlock GetUserAvatarSettings( ServerUserItem userIter )
		{
			var block = new TextBlock();

			block.SetAttribute( "Mesh", "Content\\Characters\\Default\\Human.fbx|$Mesh" );
			//block.SetAttribute( "Mesh", "Content\\Characters\\Kachujin\\Kachujin G Rosales.fbx|$Mesh" );

			return block;
		}

		public virtual Component CreateObjectControlledByPlayer( ServerUserItem userItem, Metadata.TypeInfo objectType, Transform transform )
		{
			var scene = ParentRoot as Scene;
			if( scene == null )
				return null;

			var obj = scene.CreateComponent( objectType, enabled: false, setUniqueName: true );
			obj.NewObjectSetDefaultConfiguration();

			//!!!!возможно передавать на клиента только имя
			if( obj is Character character )
			{
				var inputProcessing = obj.CreateComponent<CharacterInputProcessing>();
				inputProcessing.Name = "Character Input Processing";


				//!!!!было

				//var avatarSettings = GetUserAvatarSettings( userItem );

				//var path = avatarSettings.GetAttribute( "Mesh" );
				////var path = "Content\\Characters\\Kachujin\\Kachujin G Rosales.fbx|$Mesh";

				//var meshInSpace = character.GetComponent<MeshInSpace>();
				//if( meshInSpace != null )
				//	meshInSpace.Mesh = new ReferenceNoValue( path );


				//!!!!было

				//character.IdleAnimation = new ReferenceNoValue( path + "\\$Animations\\$Idle" );
				//character.WalkAnimation = new ReferenceNoValue( path + "\\$Animations\\$Walk" );
				//character.RunAnimation = new ReferenceNoValue( path + "\\$Animations\\$Run" );
				//character.FlyAnimation = new ReferenceNoValue( path + "\\$Animations\\$Fly" );
				//character.JumpAnimation = new ReferenceNoValue( path + "\\$Animations\\$Jump" );
				//character.LeftTurnAnimation = new ReferenceNoValue( path + "\\$Animations\\$Left Turn" );
				//character.RightTurnAnimation = new ReferenceNoValue( path + "\\$Animations\\$Right Turn" );
			}


			obj.Enabled = true;

			if( obj is Character character2 )
			{
				//Character specific
				character2.SetTransformAndTurnToDirectionInstantly( transform );
				//character2.SetTransform( transform, true );
				//character2.SetTurnToDirection( transform.Rotation.GetForward(), true );
			}
			else if( obj is Vehicle vehicle )
			{
				//Vehicle specific
				vehicle.SetTransform( transform, true );
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

			return obj;
		}

		public SpawnPoint[] GetSpawnPoints()
		{
			//!!!!slowly
			return ParentRoot.GetComponents<SpawnPoint>( onlyEnabledInHierarchy: true );

			//if( spawnPointsCache == null )
			//	spawnPointsCache = ParentRoot.GetComponents<SpawnPoint>( onlyEnabledInHierarchy: true );
			//return spawnPointsCache;
		}

		public SpawnPoint[] GetSpawnPointsForTeam( int team )
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

		protected virtual void UpdateObjectControlledByPlayer( ServerUserItem userItem )
		{
			//create new
			if( userItem.EnteredToWorld && userItem.ObjectControlledByPlayer == null )
			{
				var objectType = ObjectTypeControlledByPlayer.Value;
				if( objectType != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
				{
					var spawnPoint = OnGetSpawnPoint( userItem, objectType );
					if( spawnPoint != null )
					{
						var objectPosition = spawnPoint.TransformV.Position;

						//Vector3 objectPosition;
						//{
						//	var spawnPoint = OnGetSpawnPoint( userItem, objectType );// = SpawnPoint.Value;
						//	if( spawnPoint != null )
						//		objectPosition = spawnPoint.TransformV.Position;
						//	else
						//		objectPosition = Vector3.Zero;
						//}

						//!!!!not only for characters
						var objectBounds = new Bounds( -0.2, -0.2, -0.9, 0.2, 0.2, 0.9 );

						var transform = GetFreePlaceForObject( objectPosition, objectBounds );
						if( transform != null )
							CreateObjectControlledByPlayer( userItem, objectType, transform );
					}
				}

				//var objectType = ObjectTypeControlledByPlayer.Value;

				//if( objectType != null && MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) ).IsAssignableFrom( objectType ) )
				//{

				//Vector3 objectPosition;
				//{
				//	var spawnPoint = SpawnPoint.Value;
				//	if( spawnPoint != null )
				//		objectPosition = spawnPoint.TransformV.Position;
				//	else
				//		objectPosition = Vector3.Zero;
				//}

				////!!!!not only for characters
				//var objectBounds = new Bounds( -0.2, -0.2, -0.9, 0.2, 0.2, 0.9 );

				//zzzzz;

				//var transform = GetFreePlaceForObject( objectPosition, objectBounds );
				//if( transform != null )
				//{
				//	var scene = ParentRoot as Scene;
				//	if( scene != null )
				//	{
				//		var obj = (ObjectInSpace)scene.CreateComponent( objectType, enabled: false, setUniqueName: true );
				//		obj.NewObjectSetDefaultConfiguration();


				//		//!!!!
				//		//!!!!возможно передавать на клиента только имя
				//		if( obj is Character character )
				//		{
				//			var avatarSettings = GetUserAvatarSettings( userItem );

				//			var path = avatarSettings.GetAttribute( "Mesh" );
				//			//var path = "Content\\Characters\\Kachujin\\Kachujin G Rosales.fbx|$Mesh";

				//			var meshInSpace = character.GetComponent<MeshInSpace>();
				//			if( meshInSpace != null )
				//				meshInSpace.Mesh = new ReferenceNoValue( path );

				//			character.IdleAnimation = new ReferenceNoValue( path + "\\$Animations\\$Idle" );
				//			character.WalkAnimation = new ReferenceNoValue( path + "\\$Animations\\$Walk" );
				//			character.RunAnimation = new ReferenceNoValue( path + "\\$Animations\\$Run" );
				//			character.FlyAnimation = new ReferenceNoValue( path + "\\$Animations\\$Fly" );
				//			character.JumpAnimation = new ReferenceNoValue( path + "\\$Animations\\$Jump" );
				//			character.LeftTurnAnimation = new ReferenceNoValue( path + "\\$Animations\\$Left Turn" );
				//			character.RightTurnAnimation = new ReferenceNoValue( path + "\\$Animations\\$Right Turn" );
				//		}


				//		obj.Enabled = true;

				//		if( obj is Character character2 )
				//		{
				//			character2.SetTransform( transform, true );
				//			character2.SetTurnToDirection( transform.Rotation.GetForward(), true );
				//		}
				//		else if( obj is Vehicle vehicle )
				//		{
				//			vehicle.SetTransform( transform );
				//		}
				//		else
				//		{
				//			obj.SetPosition( transform.Position );
				//			obj.SetRotation( transform.Rotation );
				//		}

				//		userItem.ObjectControlledByPlayer = obj;
				//		userItem.ObjectControlledByPlayerInputEnabled = true;

				//		SendSetObjectControlledByPlayer( userItem );
				//	}
				//}
				//}

			}

			//destroy when leave the world to World Entrance screen
			if( !userItem.EnteredToWorld && userItem.ObjectControlledByPlayer != null )
			{
				userItem.ObjectControlledByPlayer.RemoveFromParent( true );
				userItem.ObjectControlledByPlayer = null;
			}
		}

		public void UpdateObjectControlledByPlayers()
		{
			foreach( var userItem in serverUsers.Values )
			{
				//clear deleted
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

		public override Component ServerGetObjectControlledByUser( ServerNetworkService_Users.UserInfo user, bool inputMustEnabled )
		{
			var item = ServerGetUser( user );
			if( item != null )
			{
				if( item.ObjectControlledByPlayerInputEnabled || !inputMustEnabled )
					return item.ObjectControlledByPlayer;
			}
			return null;
		}

		public override ServerNetworkService_Users.UserInfo ServerGetUserByObjectControlled( Component obj, bool inputMustEnabled )
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

			var writer = BeginNetworkMessage( item.User, "SetObjectControlledByPlayer" );
			if( writer != null )
			{
				writer.Write( referenceToObject );
				EndNetworkMessage();
			}
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
		}

		protected virtual void ClientOnEnabledInSimulation()
		{
			//reset GameMode.ObjectControlledByPlayer on the client
			if( SimulationAppClient.Created )
			{
				var scene = SimulationAppClient.Client?.Components.Scene;
				if( scene != null )
				{
					var gameMode = scene.GetComponent<GameMode>();
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
					var gameMode = scene.GetComponent<GameMode>();
					if( gameMode != null )
					{
						gameMode.ObjectControlledByPlayer = new Reference<Component>( null, referenceToObject );
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