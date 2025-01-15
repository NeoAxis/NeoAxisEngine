// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NeoAxis.Networking;

namespace NeoAxis
{
	static class ServerNetworkService_Components_SpecialSerialization
	{
		static bool initialized;
		static ArrayDataWriter writer = new ArrayDataWriter();

		//ObjectInSpace.Transform
		[Flags]
		enum ObjectInSpaceTransformModes
		{
			PositionFloat = 1,
			ScaleAllEqual = 2,
			ScaleOne = 4,
		}
		static Metadata.Property objectInSpaceTransformProperty;

		//

		static void Init()
		{
			if( !initialized )
			{
				initialized = true;

				//ObjectInSpace.Transform
				{
					var type = MetadataManager.GetTypeOfNetType( typeof( ObjectInSpace ) );
					objectInSpaceTransformProperty = type.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
					if( objectInSpaceTransformProperty == null )
						Log.Fatal( "ServerNetworkService_Components_SpecialSerialization: transformProperty == null." );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool WriteSpecialSerialized( Metadata.Property property, Component component, out ArraySegment<byte> arraySegment )
		{
			Init();

			//ObjectInSpace.Transform
			if( property == objectInSpaceTransformProperty )
			{
				var value = (Reference<Transform>)property.GetValue( component, null );
				if( !value.ReferenceSpecified && value.Value != null )
				{
					var tr = value.Value;
					var position = tr.Position;
					var scale = tr.Scale;

					//get mode
					ObjectInSpaceTransformModes mode = 0;
					if( Math.Abs( position.X ) < 10000 && Math.Abs( position.Y ) < 10000 && Math.Abs( position.Z ) < 10000 )
						mode |= ObjectInSpaceTransformModes.PositionFloat;
					if( scale.X == scale.Y && scale.X == scale.Z )
					{
						if( scale.X == 1 )
							mode |= ObjectInSpaceTransformModes.ScaleOne;
						else
							mode |= ObjectInSpaceTransformModes.ScaleAllEqual;
					}

					writer.Reset();
					writer.Write( (byte)mode );

					//position
					if( ( mode & ObjectInSpaceTransformModes.PositionFloat ) != 0 )
						writer.Write( position.ToVector3F() );
					else
						writer.Write( ref position );

					//rotation
					writer.Write( tr.Rotation.ToQuaternionH() );

					//scale

					if( ( mode & ObjectInSpaceTransformModes.ScaleOne ) != 0 )
					{
					}
					else if( ( mode & ObjectInSpaceTransformModes.ScaleAllEqual ) != 0 )
						writer.Write( new HalfType( scale.X ) );
					else
						writer.Write( new Vector3H( scale.ToVector3F() ) );

					arraySegment = writer.ToArraySegment();
					return true;
				}
			}

			arraySegment = new ArraySegment<byte>();
			return false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void SetSpecialSerialized( Metadata.Property property, Component component, byte[] data )
		{
			Init();

			//ObjectInSpace.Transform
			if( property == objectInSpaceTransformProperty )
			{
				var reader = new ArrayDataReader( data );

				//mode
				var mode = (ObjectInSpaceTransformModes)reader.ReadByte();

				//position
				Vector3 position;
				if( ( mode & ObjectInSpaceTransformModes.PositionFloat ) != 0 )
					position = reader.ReadVector3F();
				else
					position = reader.ReadVector3();

				//rotation
				var rotation = reader.ReadQuaternionH().ToQuaternion();
				rotation.Normalize();

				//scale
				Vector3 scale;
				if( ( mode & ObjectInSpaceTransformModes.ScaleOne ) != 0 )
					scale = Vector3.One;
				else if( ( mode & ObjectInSpaceTransformModes.ScaleAllEqual ) != 0 )
				{
					var v = reader.ReadHalf();
					scale = new Vector3F( v, v, v );
				}
				else
					scale = reader.ReadVector3H();

				var tr = new Transform( position, rotation, scale );
				property.SetValue( component, new Reference<Transform>( tr ), null );
			}
		}
	}

	public class ServerNetworkService_Components : ServerService
	{
		ServerNetworkService_Users users;

		int sceneInstanceID;
		Scene scene;
		string sceneInfo = "";
		long componentIDCounter;
		//!!!!проверить чтобы не копились
		//!!!!объект может быть удален на клиенте. хотя это вроде как норм, просто он тут уже не нужен
		Dictionary<long, Component> componentByNetworkID = new Dictionary<long, Component>();

		Dictionary<ServerNetworkService_Users.UserInfo, ClientItem> clientItemByUser = new Dictionary<ServerNetworkService_Users.UserInfo, ClientItem>();
		Dictionary<long, ClientItem> clientItemByUserID = new Dictionary<long, ClientItem>();
		Dictionary<ServerNode.Client, ClientItem> clientItemByClient = new Dictionary<ServerNode.Client, ClientItem>();

		ClientItem[] allClientItemsCached;

		Dictionary<string, int> registeredPropertySignatures = new Dictionary<string, int>();
		Dictionary<string, int> registeredNetworkMessageNames = new Dictionary<string, int>();

		///////////////////////////////////////////

		public class ClientItem
		{
			internal ServerNetworkService_Users.UserInfo user;

			public ServerNetworkService_Users.UserInfo User { get { return user; } }

			//bool sceneSynchronization;

			internal Dictionary<Component, Dictionary<Metadata.Property, string>> synchronizedFlagAndSentPropertyValues = new Dictionary<Component, Dictionary<Metadata.Property, string>>();

			//!!!!use Bit list. https://gist.github.com/Wouterdek/c94b2dad6985b447c5a87348faceb442
			internal List<bool> sentPropertySignatures = new List<bool>();
			internal List<bool> sentNetworkMessageNames = new List<bool>();

			internal List<string> receivedNetworkMessageNames = new List<string>();
		}

		///////////////////////////////////////////

		const int SceneCreateBeginToClient = 1;
		const int SceneCreateEndToClient = 2;
		const int SceneDestroyToClient = 3;
		//const int SceneInstanceIDToServer = 4;

		const int ComponentCreateBeginToClient = 5;
		const int ComponentSetEnabledToClient = 6;
		const int ComponentRemoveFromParentToClient = 7;
		const int ComponentSetPropertySignatureToClient = 8;
		const int ComponentSetPropertyValueToClient = 9;
		const int ComponentSendNetworkMessageName = 10;//both directions
		const int ComponentSendNetworkMessage = 11;//both directions
		const int SimulationStepToClient = 12;

		//const int ComponentSendNetworkMessageNameToServer = 13;
		//const int ComponentSendNetworkMessageToServer = 14;

		///////////////////////////////////////////

		public ServerNetworkService_Components( ServerNetworkService_Users users )
			: base( "Components", 4 )
		{
			this.users = users;

			RegisterMessageType( nameof( SceneCreateBeginToClient ), SceneCreateBeginToClient );
			RegisterMessageType( nameof( SceneCreateEndToClient ), SceneCreateEndToClient );
			RegisterMessageType( nameof( SceneDestroyToClient ), SceneDestroyToClient );
			//RegisterMessageType( "SceneInstanceIDToServer", 4, ReceiveMessage_SceneInstanceIDToServer );

			RegisterMessageType( nameof( ComponentCreateBeginToClient ), ComponentCreateBeginToClient );
			RegisterMessageType( nameof( ComponentSetEnabledToClient ), ComponentSetEnabledToClient );
			RegisterMessageType( nameof( ComponentRemoveFromParentToClient ), ComponentRemoveFromParentToClient );
			RegisterMessageType( nameof( ComponentSetPropertySignatureToClient ), ComponentSetPropertySignatureToClient );
			RegisterMessageType( nameof( ComponentSetPropertyValueToClient ), ComponentSetPropertyValueToClient );
			RegisterMessageType( nameof( ComponentSendNetworkMessageName ), ComponentSendNetworkMessageName, ReceiveMessage_ComponentSendNetworkMessageNameToServer );
			RegisterMessageType( nameof( ComponentSendNetworkMessage ), ComponentSendNetworkMessage, ReceiveMessage_ComponentSendNetworkMessageToServer );
			RegisterMessageType( nameof( SimulationStepToClient ), SimulationStepToClient );

			//RegisterMessageType( nameof( ComponentSendNetworkMessageNameToServer ), ComponentSendNetworkMessageNameToServer, ReceiveMessage_ComponentSendNetworkMessageNameToServer );
			//RegisterMessageType( nameof( ComponentSendNetworkMessageToServer ), ComponentSendNetworkMessageToServer, ReceiveMessage_ComponentSendNetworkMessageToServer );

			//!!!!
			//ChangeSimulationFlag


			users.UserAdded += Users_UserAdded;
			users.UserRemoved += Users_UserRemoved;
		}

		protected override void OnDispose()
		{
			//!!!!там вызывался Server_OnClientDisconnected
			//if( EntitySystemWorld.Instance != null && networkingInterface != null )
			//{
			//	while( clientRemoteEntityWorlds.Count != 0 )
			//	{
			//		ClientRemoteEntityWorld remoteEntityWorld = clientRemoteEntityWorlds[ 0 ];

			//		networkingInterface.DisconnectRemoteEntityWorld( remoteEntityWorld );
			//		clientRemoteEntityWorlds.Remove( remoteEntityWorld );
			//	}
			//}

			users.UserAdded -= Users_UserAdded;
			users.UserRemoved -= Users_UserRemoved;

			base.OnDispose();
		}

		public Scene Scene
		{
			get { return scene; }
		}

		public string SceneInfo
		{
			get { return sceneInfo; }
		}

		void SubscribeToEventsChanged( Component component )
		{
			var controller = component.ParentRoot.HierarchyController;
			if( controller != null )
				controller.NetworkSubscribeToEventsChanged( component );
		}

		void UnsubscribeToEventsChanged( Component component )
		{
			var controller = component.ParentRoot.HierarchyController;
			if( controller != null )
				controller.NetworkUnsubscribeToEventsChanged( component );
		}

		public void SetScene( Scene scene, string sceneInfo )
		{
			ResetScene();

			if( scene.HierarchyController.networkServerInterface != null )
				Log.Fatal( "ServerNetworkService_Components: SetScene: scene.HierarchyController.networkServerInterface != null." );

			sceneInstanceID++;
			this.scene = scene;
			this.sceneInfo = sceneInfo;
			componentIDCounter = 1;
			componentByNetworkID.Clear();

			componentByNetworkID[ componentIDCounter ] = scene;
			scene.networkID = componentIDCounter;
			componentIDCounter++;
			//SubscribeToEventsChanged( scene );

			var networkInterface = new ComponentHierarchyController.NetworkServerInterface();

			networkInterface.AddComponent += NetworkInterface_AddComponent;
			networkInterface.RemoveFromParent += NetworkInterface_RemoveFromParent;
			networkInterface.PropertyChangedEvent += NetworkInterface_PropertyChangedEvent;
			networkInterface.SimulationStep += NetworkInterface_SimulationStep;
			networkInterface.BeginNetworkMessage += NetworkInterface_BeginNetworkMessage;
			networkInterface.EndNetworkMessage += NetworkInterface_EndNetworkMessage;
			networkInterface.GetComponentByNetworkID += NetworkInterface_GetComponentByNetworkID;
			networkInterface.ChangeNetworkMode += NetworkInterface_ChangeNetworkMode;
			networkInterface.NetworkModeAddUser += NetworkInterface_NetworkModeAddUser;
			networkInterface.NetworkModeRemoveUser += NetworkInterface_NetworkModeRemoveUser;

			scene.HierarchyController.networkServerInterface = networkInterface;


			//!!!!возможно не всем высылать
			//send initial scene state to all clients
			SendSceneCreate( null );
		}

		public void ResetScene()
		{
			if( scene != null )
			{
				var networkInterface = scene.HierarchyController.networkServerInterface;
				if( networkInterface != null )
				{
					networkInterface.AddComponent -= NetworkInterface_AddComponent;
					networkInterface.RemoveFromParent -= NetworkInterface_RemoveFromParent;
					networkInterface.PropertyChangedEvent -= NetworkInterface_PropertyChangedEvent;
					networkInterface.SimulationStep -= NetworkInterface_SimulationStep;
					networkInterface.BeginNetworkMessage -= NetworkInterface_BeginNetworkMessage;
					networkInterface.EndNetworkMessage -= NetworkInterface_EndNetworkMessage;
					networkInterface.GetComponentByNetworkID -= NetworkInterface_GetComponentByNetworkID;
					networkInterface.ChangeNetworkMode -= NetworkInterface_ChangeNetworkMode;
					networkInterface.NetworkModeAddUser -= NetworkInterface_NetworkModeAddUser;
					networkInterface.NetworkModeRemoveUser -= NetworkInterface_NetworkModeRemoveUser;

					//!!!!что еще
				}

				foreach( var clientItem in GetAllClientItems() )
					clientItem.synchronizedFlagAndSentPropertyValues.Clear();

				scene = null;
				sceneInfo = "";

				//send reset scene state to all clients
				SendSceneDestroy( null );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ClientItem GetClientItem( ServerNetworkService_Users.UserInfo user )
		{
			if( clientItemByUser.TryGetValue( user, out var value ) )
				return value;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ClientItem GetClientItem( long userID )
		{
			if( clientItemByUserID.TryGetValue( userID, out var value ) )
				return value;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ClientItem GetClientItem( ServerNode.Client client )
		{
			if( clientItemByClient.TryGetValue( client, out var value ) )
				return value;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ClientItem[] GetAllClientItems()
		{
			if( allClientItemsCached == null )
			{
				var result = new ClientItem[ clientItemByUser.Count ];
				clientItemByUser.Values.CopyTo( result, 0 );
				allClientItemsCached = result;
			}
			return allClientItemsCached;
		}

		void Users_UserAdded( ServerNetworkService_Users service, ServerNetworkService_Users.UserInfo user )
		{
			var clientItem = new ClientItem();
			clientItem.user = user;

			clientItemByUser[ user ] = clientItem;
			clientItemByUserID[ user.UserID ] = clientItem;
			if( user.Client != null )
				clientItemByClient[ user.Client ] = clientItem;
			allClientItemsCached = null;
		}

		void Users_UserRemoved( ServerNetworkService_Users service, ServerNetworkService_Users.UserInfo user )
		{
			var clientItem = GetClientItem( user );
			if( clientItem != null )
			{
				//!!!!там вызывался Server_OnClientDisconnected
				//networkingInterface.DisconnectRemoteEntityWorld( remoteEntityWorld );

				clientItemByUser.Remove( user );
				clientItemByUserID.Remove( user.UserID );
				if( user.Client != null )
					clientItemByClient.Remove( user.Client );
				allClientItemsCached = null;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		ArrayDataWriter BeginMessage( ICollection<ClientItem> clients, byte messageID )
		{
			var writer = BeginMessage( messageID );

			if( clients is List<ClientItem> list )
			{
				for( int n = 0; n < list.Count; n++ )
					AddMessageRecipient( list[ n ] );
			}
			else if( clients is ClientItem[] array )
			{
				foreach( var client in array )
					AddMessageRecipient( client );
			}
			else
			{
				foreach( var client in clients )
					AddMessageRecipient( client );
			}

			return writer;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		ArrayDataWriter BeginMessage( ClientItem client, byte messageID )
		{
			return BeginMessage( client.User.Client, messageID );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void AddMessageRecipient( ClientItem client )
		{
			AddMessageRecipient( client.User.Client );
		}

		//!!!!use array
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		ICollection<ClientItem> GetClientsWithAccessToComponentByNetworkMode( Component component, ClientItem onlyThisClient )
		{
			//!!!!GC

			if( onlyThisClient != null )
			{
				if( component.NetworkModeIsEnabledForUser( onlyThisClient ) )
					return new ClientItem[] { onlyThisClient };
			}
			else
			{
				switch( component.NetworkMode.Value )
				{
				case NetworkModeEnum.SelectedUsers: return component.NetworkModeUsers;
				case NetworkModeEnum.True: return GetAllClientItems();
				}
			}

			return Array.Empty<ClientItem>();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		List<ClientItem> GetSynchronizedClientsOfComponent( Component component, ClientItem onlyThisClient )
		{
			//!!!!GC

			if( onlyThisClient != null )
			{
				var result = new List<ClientItem>( 1 );
				if( onlyThisClient.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
					result.Add( onlyThisClient );
				return result;
			}
			else
			{
				var allClients = GetAllClientItems();

				var result = new List<ClientItem>( allClients.Length );
				for( int n = 0; n < allClients.Length; n++ )
				{
					var client = allClients[ n ];
					if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
						result.Add( client );
				}

				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		ArrayDataWriter BeginMessage2( ClientItem clientItem, byte messageID )
		{
			if( clientItem != null )
				return BeginMessage( clientItem, messageID );
			else
				return BeginMessageToEveryone( messageID );
		}

		public void SendSceneCreate( ClientItem clientItem )
		{
			if( scene == null )
				Log.Fatal( "ServerNetworkService_Components: SendInitialSceneState: scene == null." );

			//if( user.ConnectedNode != null )//check for local user
			//{

			//create a scene
			{
				var writer = BeginMessage2( clientItem, SceneCreateBeginToClient );
				writer.WriteVariableInt32( sceneInstanceID );
				writer.Write( sceneInfo );
				writer.WriteVariableUInt64( (ulong)scene.networkID );

				//type
				var typeName = scene.BaseType.Name;
				writer.Write( typeName );

				writer.Write( scene.Name );

				//if( Map.Instance != null )
				//	writer.Write( Map.Instance.VirtualFileName );
				//else
				//	writer.Write( "" );

				EndMessage();

				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Server: Send scene create begin" );
			}

			SubscribeToEventsChanged( scene );

			//make similar how load/save works. before create components hierarchy, then init properties

			var clients = clientItem != null ? new ClientItem[] { clientItem } : GetAllClientItems();
			foreach( var client in clients )
				client.synchronizedFlagAndSentPropertyValues[ scene ] = new Dictionary<Metadata.Property, string>();

			//create children recursive
			foreach( var child in scene.GetComponents() )
				SendComponentCreateWithChildrenRecursive( clientItem, child, false );

			var children = scene.GetComponents( checkChildren: true, depthFirstSearch: true );

			try
			{
				//disable changed events to prevent double sending
				scene.networkDisableChangedEvents = true;
				foreach( var child in children )
					child.networkDisableChangedEvents = true;

				//send children properties and Enabled for all hierarchy
				foreach( var child in children )
					SendComponentProperties( clientItem, child, false, true );

				//send scene properties
				SendComponentProperties( clientItem, scene, false, false );

				if( clientItem != null )
					scene.PerformClientConnectedBeforeRootComponentEnabled( clientItem );

				//scene create end
				{
					var writer = BeginMessage2( clientItem, SceneCreateEndToClient );
					writer.Write( scene.Enabled );
					EndMessage();

					if( NetworkCommonSettings.NetworkLogging )
						Log.Info( "Network log: Server: Send scene create end" );
				}

				if( clientItem != null )
					scene.PerformClientConnectedAfterRootComponentEnabled( clientItem );
			}
			finally
			{
				scene.networkDisableChangedEvents = false;
				foreach( var child in children )
					child.networkDisableChangedEvents = false;
			}

			//}
		}

		public void SendSceneDestroy( ClientItem clientItem )// ServerNetworkService_Users.UserInfo user )
		{
			BeginMessage2( clientItem, SceneDestroyToClient );
			EndMessage();

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Server: Send scene destroy" );
		}

		public void ClientDisconnected( ClientItem clientItem )// ServerNetworkService_Users.UserInfo user )
		{
			if( scene != null && !scene.Disposed )
				scene.PerformClientDisconnected( clientItem );
		}

		//static bool IsComponentTypeSynced( Component component )
		//{
		//	var type = component.BaseType;
		//	do
		//	{
		//		if( type.NetworkMode.HasValue )
		//			return type.NetworkMode.Value;
		//		type = type.BaseType;
		//	}
		//	while( type != null );

		//	return true;
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendComponentCreateWithChildrenRecursive( ClientItem clientItem, Component component, bool sendInsertIndex )
		{
			var parent = component.Parent;
			if( parent != null && parent.networkID != 0 && component.NetworkMode.Value != NetworkModeEnum.False )//&& IsComponentTypeSynced( component ) )
			{
				var subscribeToEventsChanged = false;

				//assign network ID
				if( component.networkID == 0 )
				{
					componentByNetworkID[ componentIDCounter ] = component;
					component.networkID = componentIDCounter;
					componentIDCounter++;

					subscribeToEventsChanged = true;
					//SubscribeToEventsChanged( component );
				}

				int insertIndex = -1;
				if( sendInsertIndex )
				{
					//!!!!может для убыстрения с конца искать
					insertIndex = parent != null ? parent.Components.IndexOf( component ) : -1;
				}

				var clients = GetClientsWithAccessToComponentByNetworkMode( component, clientItem );
				//var clients = clientItem != null ? new ClientItem[] { clientItem } : GetClientsWithAccessToComponentByNetworkMode( component );

				var clients2 = new List<ClientItem>( clients.Count );
				foreach( var client in clients )
				{
					if( !client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component.Parent ) )
					{
						clients2.Add( client );

						client.synchronizedFlagAndSentPropertyValues[ component ] = new Dictionary<Metadata.Property, string>();
					}
				}

				//send create begin
				if( clients2.Count != 0 )
				{
					var writer = BeginMessage( clients2, ComponentCreateBeginToClient );
					writer.WriteVariableUInt64( (ulong)parent.networkID );
					writer.WriteVariableUInt64( (ulong)component.networkID );

					//type
					var typeName = component.BaseType.Name;
					writer.Write( typeName );
					writer.WriteVariableInt32( insertIndex );
					writer.Write( component.Name );
					//writer.Write( component.Enabled );
					EndMessage();

					if( NetworkCommonSettings.NetworkLogging )
						Log.Info( "Network log: Server: Send component create begin: parent " + parent.networkID.ToString() + ", component " + component.networkID.ToString() + ", name " + component.Name );
				}

				if( subscribeToEventsChanged )
					SubscribeToEventsChanged( component );

				//create children
				foreach( var child in component.GetComponents() )
					SendComponentCreateWithChildrenRecursive( clientItem, child, false );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendComponentProperties( ClientItem clientItem, Component component, bool sendName, bool sendEnabled )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				//fast exit
				if( clientItem != null && !clientItem.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
					return;

				//send property values except Enabled. Name optionally
				{
					var context = new Metadata.GetMembersContext( false );

					foreach( var member in component.MetadataGetMembers( context ).ToArray() )
					{
						var property = member as Metadata.Property;
						if( property != null && property.NetworkMode && MetadataManager.Serialization.IsMemberSerializable( property ) )
						{
							if( !sendName && property.Name == "Name" )
								continue;
							if( !sendEnabled && property.Name == "Enabled" )
								continue;

							//!!!!какие не отправлять?

							SendComponentSetPropertyValue( clientItem, component, property );
						}
					}
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendComponentEnabled( ClientItem clientItem, Component component )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				var clients = GetSynchronizedClientsOfComponent( component, clientItem );
				if( clients.Count != 0 )
				{
					var writer = BeginMessage( clients, ComponentSetEnabledToClient );
					writer.WriteVariableUInt64( (ulong)component.networkID );
					writer.Write( component.Enabled );
					EndMessage();

					if( NetworkCommonSettings.NetworkLogging )
						Log.Info( "Network log: Server: Send component set enabled: " + component.networkID.ToString() + " " + component.Enabled.ToString() );
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendAddComponent( ClientItem clientItem, Component child, bool createComponent )
		{
			if( child.NetworkMode.Value != NetworkModeEnum.False )//&& IsComponentTypeSynced( child ) )
			{
				SendComponentCreateWithChildrenRecursive( clientItem, child, true );

				if( child.networkID != 0 )
				{
					//send properties when add component
					if( !createComponent )
					{
						foreach( var child2 in child.GetComponents( checkChildren: true ) )
							SendComponentProperties( clientItem, child2, false, true );
						SendComponentProperties( clientItem, child, false, false );
					}

					//send Enabled
					SendComponentEnabled( clientItem, child );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_AddComponent( ComponentHierarchyController.NetworkServerInterface sender, Component child, bool createComponent )
		{
			SendAddComponent( null, child, createComponent );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendRemoveFromParent( ClientItem clientItem, Component component, bool queued, bool disposing )
		{
			if( component.networkID != 0 )
			{
				//fast exit
				if( clientItem != null && !clientItem.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
					return;

				var unregisterOnServer = clientItem == null;
				var clients = GetSynchronizedClientsOfComponent( component, clientItem );

				if( clients.Count != 0 )
				{
					var writer = BeginMessage( clients, ComponentRemoveFromParentToClient );
					writer.WriteVariableUInt64( (ulong)component.networkID );
					//writer.Write( queued );
					writer.Write( disposing );
					EndMessage();

					if( NetworkCommonSettings.NetworkLogging )
						Log.Info( "Network log: Server: Send component remove from parent: " + component.networkID.ToString() );
				}

				if( unregisterOnServer )
				{
					componentByNetworkID.Remove( component.networkID );
					component.networkID = 0;
					UnsubscribeToEventsChanged( component );
				}
				foreach( var client in clients )
					client.synchronizedFlagAndSentPropertyValues.Remove( component );

				foreach( var child in component.GetComponents( checkChildren: true ) )
				{
					if( child.networkID != 0 )
					{
						if( unregisterOnServer )
						{
							componentByNetworkID.Remove( child.networkID );
							child.networkID = 0;
							UnsubscribeToEventsChanged( child );
						}
						foreach( var client in clients )
							client.synchronizedFlagAndSentPropertyValues.Remove( child );
					}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_RemoveFromParent( ComponentHierarchyController.NetworkServerInterface sender, Component component, bool queued, bool disposing )
		{
			SendRemoveFromParent( null, component, queued, disposing );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SendComponentSetPropertyValue( ClientItem clientItem, Component component, Metadata.Property property )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				//fast exit
				if( clientItem != null && !clientItem.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
					return;

				//disabled synchronization
				if( component.NetworkIsDisabledPropertySynchronization( property.Name ) )
					return;

				var context = new Metadata.SaveContext();
				context.UseDefaultValue = false;

				//!!!!slowly?
				//!!!!не нужно сериализовывать если потом special serialization. возможно лучше свойство вручную через network message слать
				var block = new TextBlock();
				if( MetadataManager.Serialization.SaveSerializableMember( context, component, property, block, out var serialized, out var error ) && serialized )
				{
					var emptyValue = string.IsNullOrEmpty( block.Name ) && block.Children.Count == 0 && block.Attributes.Count == 0;
					var valueString = emptyValue ? "" : block.DumpToString( false );

					int signatureID = int.MaxValue;// -1;

					var sent = false;

					//!!!!parallel?

					var clients = GetSynchronizedClientsOfComponent( component, clientItem );
					//var clients = clientItem != null ? new ClientItem[] { clientItem } : GetClientsWithAccessToComponentByNetworkMode( component );
					foreach( var client in clients )
					{
						if( client.synchronizedFlagAndSentPropertyValues.TryGetValue( component, out var sentPropertyItem ) )
						{
							//if( !client.synchronizedFlagAndSentPropertyValues.TryGetValue( component, out var sentPropertyItem ) )
							//{
							//	sentPropertyItem = new Dictionary<Metadata.Property, string>();
							//	client.synchronizedFlagAndSentPropertyValues[ component ] = sentPropertyItem;
							//}

							var sameValue = false;
							if( sentPropertyItem.TryGetValue( property, out var sentValue ) )
								sameValue = valueString == sentValue;

							if( !sameValue )
							{
								//!!!!может лучше их тут всех сразу слать

								//register property signature
								if( signatureID == int.MaxValue )//- 1 )
								{
									if( !registeredPropertySignatures.TryGetValue( property.Signature, out signatureID ) )
									{
										//+1 - to make signatureID always positive. it combined with special serialization flag
										signatureID = registeredPropertySignatures.Count + 1;
										registeredPropertySignatures[ property.Signature ] = signatureID;
									}
								}

								//send property signature
								{
									while( signatureID >= client.sentPropertySignatures.Count )
										client.sentPropertySignatures.Add( false );
									if( !client.sentPropertySignatures[ signatureID ] )
									{
										var writer2 = BeginMessage( client, ComponentSetPropertySignatureToClient );
										writer2.Write( property.Signature );
										writer2.WriteVariableInt32( signatureID );
										EndMessage();

										client.sentPropertySignatures[ signatureID ] = true;
									}
								}

								//send property value
								{
									var specialSerialized = ServerNetworkService_Components_SpecialSerialization.WriteSpecialSerialized( property, component, out var arraySegment );

									var writer = BeginMessage( client, ComponentSetPropertyValueToClient );
									writer.WriteVariableUInt64( (ulong)component.networkID );
									writer.WriteVariableInt32( signatureID * ( specialSerialized ? -1 : 1 ) );
									if( specialSerialized )
									{
										writer.WriteVariableUInt32( (uint)arraySegment.Count );
										writer.Write( arraySegment.Array, 0, arraySegment.Count );
									}
									else
										writer.Write( valueString );
									EndMessage();
								}

								sentPropertyItem[ property ] = valueString;
								sent = true;
							}
						}
					}

					if( sent )
					{
						if( NetworkCommonSettings.NetworkLogging )
							Log.Info( "Network log: Server: Send component set property: " + property.Signature + ", Network ID: " + component.networkID.ToString() + ", Value: " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );
					}


					//if( !clientItem.sendPropertyValues.TryGetValue( component, out var sentPropertyItem ) )
					//{
					//	sentPropertyItem = new Dictionary<Metadata.Property, string>();
					//	clientItem.sendPropertyValues[ component ] = sentPropertyItem;
					//}

					//var sameValue = false;
					//if( sentPropertyItem.TryGetValue( property, out var sentValue ) )
					//	sameValue = valueString == sentValue;
					////if( clientItem.sendPropertyValues.TryGetValue( component, out var sentPropertyItem ) )
					////{
					////	if( sentPropertyItem.TryGetValue( property, out var sentValue ) )
					////		sameValue = valueString == sentValue;
					////}
					////if( component.networkPropertyValuesSent != null && component.networkPropertyValuesSent.TryGetValue( property, out var sentValue ) )
					////	sameValue = valueString == sentValue;

					//if( !sameValue )
					//{
					//	var writer = BeginMessage2( clientItem, ComponentSetPropertyValueToClient );
					//	writer.WriteVariableUInt64( (ulong)component.networkID );
					//	writer.Write( property.Signature );
					//	writer.Write( valueString );
					//	EndMessage();

					//	sentPropertyItem[ property ] = valueString;
					//	//if( component.networkPropertyValuesSent == null )
					//	//	component.networkPropertyValuesSent = new Dictionary<Metadata.Property, string>();
					//	//component.networkPropertyValuesSent[ property ] = valueString;

					//	if( NetworkCommonSettings.NetworkLogging )
					//		Log.Info( "Network log: Server: Send component set property: " + property.Signature + " " + component.networkID.ToString() + " " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );
					//}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_PropertyChangedEvent( ComponentHierarchyController.NetworkServerInterface sender, Component component, Metadata.Property property )
		{
			SendComponentSetPropertyValue( null, component, property );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_SimulationStep( ComponentHierarchyController.NetworkServerInterface sender )
		{
			BeginMessageToEveryone( SimulationStepToClient );
			EndMessage();

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Server: Send simulation step ------------------------------------------------------------------" );
		}

		List<ClientItem> _tempNetworkMessageRecepients = new List<ClientItem>();

		[MethodImpl( (MethodImplOptions)512 )]
		private void NetworkInterface_BeginNetworkMessage( ComponentHierarchyController.NetworkServerInterface sender, Component component, IList<ClientItem> clientRecipients, ClientItem clientRecipient, IList<ServerNetworkService_Users.UserInfo> userRecipients, ServerNetworkService_Users.UserInfo userRecipient, bool toEveryone, string message, ref ArrayDataWriter writer )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				//get nameID
				int nameID;
				if( !registeredNetworkMessageNames.TryGetValue( message, out nameID ) )
				{
					nameID = registeredNetworkMessageNames.Count;
					registeredNetworkMessageNames[ message ] = nameID;
				}

				//get list of recepients
				_tempNetworkMessageRecepients.Clear();
				if( toEveryone )
				{
					var allClients = GetAllClientItems();
					for( int n = 0; n < allClients.Length; n++ )
					{
						var client = allClients[ n ];
						if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							_tempNetworkMessageRecepients.Add( client );
					}
				}
				else if( clientRecipient != null )
				{
					if( clientRecipient.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
						_tempNetworkMessageRecepients.Add( clientRecipient );
				}
				else if( clientRecipients != null )
				{
					for( int n = 0; n < clientRecipients.Count; n++ )
					{
						var client = clientRecipients[ n ];
						if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							_tempNetworkMessageRecepients.Add( client );
					}
				}
				else if( userRecipient != null )
				{
					var client = GetClientItem( userRecipient );
					if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
						_tempNetworkMessageRecepients.Add( client );
				}
				else if( userRecipients != null )
				{
					for( int n = 0; n < userRecipients.Count; n++ )
					{
						var client = GetClientItem( userRecipients[ n ] );
						if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							_tempNetworkMessageRecepients.Add( client );
					}
				}

				//send nameID
				for( int n = 0; n < _tempNetworkMessageRecepients.Count; n++ )
				{
					var client = _tempNetworkMessageRecepients[ n ];

					while( nameID >= client.sentNetworkMessageNames.Count )
						client.sentNetworkMessageNames.Add( false );

					if( !client.sentNetworkMessageNames[ nameID ] )
					{
						var writer2 = BeginMessage( client, ComponentSendNetworkMessageName );
						writer2.Write( message );
						writer2.WriteVariableInt32( nameID );
						EndMessage();

						client.sentNetworkMessageNames[ nameID ] = true;
					}
				}

				//begin network message
				writer = BeginMessage( ComponentSendNetworkMessage );
				for( int n = 0; n < _tempNetworkMessageRecepients.Count; n++ )
					AddMessageRecipient( _tempNetworkMessageRecepients[ n ] );
				writer.WriteVariableUInt64( (ulong)component.networkID );
				writer.WriteVariableInt32( nameID );

				_tempNetworkMessageRecepients.Clear();

				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Server: Send network message: " + component.networkID.ToString() + " " + message );



				//writer = BeginMessage( ComponentSendNetworkMessageToClient );

				//if( toEveryone )
				//{
				//	var allClients = GetAllClientItems();
				//	for( int n = 0; n < allClients.Length; n++ )
				//	{
				//		var client = allClients[ n ];
				//		if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
				//			AddMessageRecipient( client );
				//	}
				//}
				//else if( clientRecipient != null )
				//{
				//	if( clientRecipient.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
				//		AddMessageRecipient( clientRecipient );
				//}
				//else if( clientRecipients != null )
				//{
				//	for( int n = 0; n < clientRecipients.Count; n++ )
				//	{
				//		var client = clientRecipients[ n ];
				//		if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
				//			AddMessageRecipient( client );
				//	}
				//}
				//else if( userRecipient != null )
				//{
				//	var client = GetClientItem( userRecipient );
				//	if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
				//		AddMessageRecipient( client );
				//}
				//else if( userRecipients != null )
				//{
				//	for( int n = 0; n < userRecipients.Count; n++ )
				//	{
				//		var client = GetClientItem( userRecipients[ n ] );
				//		if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
				//			AddMessageRecipient( client );
				//	}
				//}

				//writer.WriteVariableUInt64( (ulong)component.networkID );
				//writer.Write( message );

				//if( NetworkCommonSettings.NetworkLogging )
				//	Log.Info( "Network log: Server: Send network message: " + component.networkID.ToString() + " " + message );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_EndNetworkMessage( ComponentHierarchyController.NetworkServerInterface sender )
		{
			if( SendingData )
				EndMessage();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		Component GetComponentByNetworkID( long networkID )
		{
			if( componentByNetworkID.TryGetValue( networkID, out var component ) )
				return component;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_GetComponentByNetworkID( ComponentHierarchyController.NetworkServerInterface sender, long networkID, ref Component component )
		{
			component = GetComponentByNetworkID( networkID );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		private void NetworkInterface_ChangeNetworkMode( ComponentHierarchyController.NetworkServerInterface sender, Component component )
		{
			if( component.Parent == null )
				return;

			switch( component.NetworkMode.Value )
			{
			case NetworkModeEnum.True:
				SendAddComponent( null, component, false );
				break;

			case NetworkModeEnum.SelectedUsers:
				{
					//!!!!slowly
					var clientsWithAccess = new ESet<ClientItem>( GetClientsWithAccessToComponentByNetworkMode( component, null ) );

					foreach( var client in GetAllClientItems() )
					{
						if( clientsWithAccess.Contains( client ) )
						{
							if( !client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
								SendAddComponent( client, component, false );
						}
						else
						{
							if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
								SendRemoveFromParent( client, component, false, true );
						}
					}
				}
				break;

			case NetworkModeEnum.False:
				SendRemoveFromParent( null, component, false, true );
				break;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_NetworkModeAddUser( ComponentHierarchyController.NetworkServerInterface sender, ClientItem clientItem, Component component )
		{
			if( component.NetworkMode.Value == NetworkModeEnum.SelectedUsers )
				SendAddComponent( clientItem, component, false );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_NetworkModeRemoveUser( ComponentHierarchyController.NetworkServerInterface sender, ClientItem clientItem, Component component )
		{
			if( component.NetworkMode.Value == NetworkModeEnum.SelectedUsers )
				SendRemoveFromParent( clientItem, component, false, true );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSendNetworkMessageNameToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var name = reader.ReadString();
			var nameID = reader.ReadVariableInt32();
			if( !reader.Complete() )
				return false;

			var client = GetClientItem( sender );
			if( client != null )
			{
				while( nameID >= client.receivedNetworkMessageNames.Count )
					client.receivedNetworkMessageNames.Add( "" );
				client.receivedNetworkMessageNames[ nameID ] = name;
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSendNetworkMessageToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var sceneInstanceID = reader.ReadVariableInt32();
			var componentID = (long)reader.ReadVariableUInt64();
			var nameID = reader.ReadVariableInt32();
			if( reader.Overflow )
				return false;

			if( this.sceneInstanceID == sceneInstanceID && scene != null && !scene.Disposed )
			{
				//if( NetworkCommonSettings.NetworkLogging )
				//	Log.Info( "Network log: Server: Receive network message: " + componentID.ToString() + " " + message );

				var component = GetComponentByNetworkID( componentID );
				if( component != null )
				{
					var client = GetClientItem( sender );
					if( client != null )
					{
						var message = client.receivedNetworkMessageNames[ nameID ];

						if( NetworkCommonSettings.NetworkLogging )
							Log.Info( "Network log: Server: Receive network message: " + componentID.ToString() + " " + message );

						return component.PerformReceiveNetworkMessageFromClient( client, message, reader );
					}
				}

				//if( !reader.Complete() )
				//	return false;
			}

			return true;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Components : ClientService
	{
		ClientNetworkService_Users users;

		int sceneInstanceID;
		Scene scene;
		//!!!!проверить чтобы не копились
		//!!!!объект может быть удален на клиенте. хотя это вроде как норм, просто он тут уже не нужен
		Dictionary<long, Component> componentByNetworkID = new Dictionary<long, Component>();

		List<string> receivedPropertyNameSignatures = new List<string>();
		List<string> receivedNetworkMessageNames = new List<string>();

		Dictionary<string, int> registeredAndSentNetworkMessageNames = new Dictionary<string, int>();

		//for profiler
		string sendingNetworkMessage = "";

		///////////////////////////////////////////

		public delegate void SceneCreateBeginDelegate( ClientNetworkService_Components sender, string sceneInfo );// string mapVirtualFileName );
		public event SceneCreateBeginDelegate SceneCreateBegin;

		public delegate void SceneCreateEndDelegate( ClientNetworkService_Components sender );
		public event SceneCreateEndDelegate SceneCreateEnd;

		public delegate void SceneDestroyDelegate( ClientNetworkService_Components sender );//, bool newMapWillBeLoaded );
		public event SceneDestroyDelegate SceneDestroy;

		///////////////////////////////////////////

		const int SceneCreateBeginToClient = 1;
		const int SceneCreateEndToClient = 2;
		const int SceneDestroyToClient = 3;
		//const int SceneInstanceIDToServer = 4;

		const int ComponentCreateBeginToClient = 5;
		const int ComponentSetEnabledToClient = 6;
		const int ComponentRemoveFromParentToClient = 7;
		const int ComponentSetPropertySignatureToClient = 8;
		const int ComponentSetPropertyValueToClient = 9;
		const int ComponentSendNetworkMessageName = 10;//both directions
		const int ComponentSendNetworkMessage = 11;//both directions
		const int SimulationStepToClient = 12;

		//const int ComponentSendNetworkMessageNameToServer = 13;
		//const int ComponentSendNetworkMessageToServer = 14;

		///////////////////////////////////////////

		public ClientNetworkService_Components( ClientNetworkService_Users users )
			: base( "Components", 4 )
		{
			this.users = users;

			RegisterMessageType( nameof( SceneCreateBeginToClient ), SceneCreateBeginToClient, ReceiveMessage_SceneCreateBeginToClient );
			RegisterMessageType( nameof( SceneCreateEndToClient ), SceneCreateEndToClient, ReceiveMessage_SceneCreateEndToClient );
			RegisterMessageType( nameof( SceneDestroyToClient ), SceneDestroyToClient, ReceiveMessage_SceneDestroyToClient );
			//RegisterMessageType( "SceneInstanceIDToServer", 4 );

			RegisterMessageType( nameof( ComponentCreateBeginToClient ), ComponentCreateBeginToClient, ReceiveMessage_ComponentCreateBeginToClient );
			RegisterMessageType( nameof( ComponentSetEnabledToClient ), ComponentSetEnabledToClient, ReceiveMessage_ComponentSetEnabledToClient );
			RegisterMessageType( nameof( ComponentRemoveFromParentToClient ), ComponentRemoveFromParentToClient, ReceiveMessage_ComponentRemoveFromParentToClient );
			RegisterMessageType( nameof( ComponentSetPropertySignatureToClient ), ComponentSetPropertySignatureToClient, ReceiveMessage_ComponentSetPropertySignatureToClient );
			RegisterMessageType( nameof( ComponentSetPropertyValueToClient ), ComponentSetPropertyValueToClient, ReceiveMessage_ComponentSetPropertyValueToClient );
			RegisterMessageType( nameof( ComponentSendNetworkMessageName ), ComponentSendNetworkMessageName, ReceiveMessage_ComponentSendNetworkMessageNameToClient );
			RegisterMessageType( nameof( ComponentSendNetworkMessage ), ComponentSendNetworkMessage, ReceiveMessage_ComponentSendNetworkMessageToClient );
			RegisterMessageType( nameof( SimulationStepToClient ), SimulationStepToClient, ReceiveMessage_SimulationStepToClient );

			//RegisterMessageType( nameof( ComponentSendNetworkMessageNameToServer ), ComponentSendNetworkMessageNameToServer );
			//RegisterMessageType( nameof( ComponentSendNetworkMessageToServer ), ComponentSendNetworkMessageToServer );
		}

		protected override void OnDispose()
		{
			//!!!!там вызывался Server_OnClientDisconnected
			//if( EntitySystemWorld.Instance != null && networkingInterface != null && serverRemoteEntityWorld != null )
			//{
			//	networkingInterface.DisconnectRemoteEntityWorld( serverRemoteEntityWorld );
			//}
			//serverRemoteEntityWorld = null;

			base.OnDispose();
		}

		public Scene Scene
		{
			get { return scene; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void CheckSceneDisposed()
		{
			if( scene != null && scene.Disposed )
				scene = null;
		}

		bool ReceiveMessage_SceneCreateBeginToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var sceneInstanceID = reader.ReadVariableInt32();
			var sceneInfo = reader.ReadString();
			var componentID = (long)reader.ReadVariableUInt64();
			var typeName = reader.ReadString();
			var name = reader.ReadString();
			//var mapVirtualFileName = reader.ReadString();
			//var worldCheckIdentifier = reader.ReadInt32();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive scene create begin" );

			//!!!!проверить кастомный класс
			var type = MetadataManager.GetType( typeName );
			if( type == null )
			{
				Log.Warning( $"ClientNetworkService_Components: ReceiveMessage_SceneCreateBeginToClient: A type with name \'{typeName}\' is not exists." );
				return true;
			}

			this.sceneInstanceID = sceneInstanceID;

			scene = (Scene)ComponentUtility.CreateComponent( type, null, true, false );
			var networkInterface = new ComponentHierarchyController.NetworkClientInterface();

			networkInterface.BeginNetworkMessage += NetworkInterface_BeginNetworkMessage;
			networkInterface.EndNetworkMessage += NetworkInterface_EndNetworkMessage;
			networkInterface.GetComponentByNetworkID += NetworkInterface_GetComponentByNetworkID;

			scene.hierarchyController.networkClientInterface = networkInterface;

			componentByNetworkID[ componentID ] = scene;
			scene.networkID = componentID;
			scene.Name = name;


			////send scene instance ID back to server
			//{
			//	var messageType2 = GetMessageType( "SceneInstanceIDToServer" );
			//	var writer = BeginMessage( messageType2 );
			//	writer.Write( sceneInstanceID );
			//	EndMessage();
			//}

			SceneCreateBegin?.Invoke( this, sceneInfo );

			return true;
		}

		bool ReceiveMessage_SceneCreateEndToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var enabled = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive scene create end" );

			CheckSceneDisposed();
			if( scene != null )
			{
				scene.Enabled = enabled;

				SceneCreateEnd?.Invoke( this );
			}

			return true;
		}

		bool ReceiveMessage_SceneDestroyToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//var newMapWillBeLoaded = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive scene destroy" );

			//!!!!не только когда Destroy?
			if( scene != null )
			{
				var networkInterface = scene.HierarchyController.networkClientInterface;
				if( networkInterface != null )
				{
					networkInterface.BeginNetworkMessage -= NetworkInterface_BeginNetworkMessage;
					networkInterface.EndNetworkMessage -= NetworkInterface_EndNetworkMessage;
					networkInterface.GetComponentByNetworkID -= NetworkInterface_GetComponentByNetworkID;
				}
			}

			componentByNetworkID.Clear();
			scene?.Dispose();
			scene = null;

			SceneDestroy?.Invoke( this );//, newMapWillBeLoaded );

			//!!!!там вызывался Server_OnClientDisconnected
			//if( EntitySystemWorld.Instance != null && networkingInterface != null )
			//{
			//	if( serverRemoteEntityWorld != null )
			//		networkingInterface.DisconnectRemoteEntityWorld( serverRemoteEntityWorld );
			//}
			//serverRemoteEntityWorld = null;

			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		Component GetComponentByNetworkID( long networkID )
		{
			if( componentByNetworkID.TryGetValue( networkID, out var component ) )
				return component;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		private void NetworkInterface_GetComponentByNetworkID( ComponentHierarchyController.NetworkClientInterface sender, long networkID, ref Component component )
		{
			component = GetComponentByNetworkID( networkID );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentCreateBeginToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var parentComponentID = (long)reader.ReadVariableUInt64();
			var componentID = (long)reader.ReadVariableUInt64();

			//!!!!тоже сделать id? typeNameID
			//!!!!может ли быть динамическим?
			var typeName = reader.ReadString();

			var insertIndex = reader.ReadVariableInt32();
			var name = reader.ReadString();
			//var enabled = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive component create begin: parent " + parentComponentID.ToString() + ", component " + componentID.ToString() + ", type name " + typeName + ", name " + name );

			CheckSceneDisposed();
			if( scene != null )
			{

				//на всякий случай проверка если уже создан
				if( GetComponentByNetworkID( componentID ) != null )
					return true;

				if( parentComponentID == 0 )
				{
					Log.Warning( "ClientNetworkService_Components: ReceiveMessage_ComponentCreateToClient: parentComponentID == 0." );
					return true;
				}

				var parent = GetComponentByNetworkID( parentComponentID );
				if( parent != null )
				{
					//it is ok when a type is not exists
					var type = MetadataManager.GetType( typeName );
					//if( type == null )
					//{
					//	Log.Warning( $"ClientNetworkService_Components: ReceiveMessage_ComponentCreateToClient: A type with name \'{typeName}\' is not exists." );
					//	return true;
					//}

					if( type != null )
					{
						Component component;

						var componentTypeInfo = type as Metadata.ComponentTypeInfo;
						if( componentTypeInfo != null )
							component = (Component)componentTypeInfo.InvokeInstance( null, false );
						else
							component = (Component)type.InvokeInstance( null );

						//component.RemoveAllComponents( false );

						component.NetworkMode = NetworkModeEnum.False;
						component.Enabled = false;
						parent.AddComponentInternal( component, insertIndex, true );

						//var component = parent.CreateComponent( type, insertIndex, false, false );


						componentByNetworkID[ componentID ] = component;
						component.networkID = componentID;
						component.Name = name;
						//component.Enabled = enabled;
					}
				}
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSetEnabledToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var componentID = (long)reader.ReadVariableUInt64();
			var enabled = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive component set enabled: " + componentID.ToString() + " " + enabled.ToString() );

			CheckSceneDisposed();
			if( scene != null )
			{
				var component = GetComponentByNetworkID( componentID );
				if( component != null )
					component.Enabled = enabled;
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentRemoveFromParentToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var componentID = (long)reader.ReadVariableUInt64();
			//var queued = reader.ReadBoolean();
			var disposing = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive component remove from parent: " + componentID.ToString() );

			CheckSceneDisposed();
			if( scene != null )
			{
				var component = GetComponentByNetworkID( componentID );
				if( component != null )
				{
					foreach( var child in component.GetComponents( checkChildren: true ) )
					{
						//!!!!у всех очищается?
						if( child.networkID != 0 )
						{
							componentByNetworkID.Remove( child.networkID );
							child.networkID = 0;
						}
					}

					if( disposing )
						component.Dispose();
					else
						component.RemoveFromParent( false );// queued );

					componentByNetworkID.Remove( componentID );
					component.networkID = 0;
				}
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSetPropertySignatureToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var signature = reader.ReadString();
			var signatureID = reader.ReadVariableInt32();
			if( !reader.Complete() )
				return false;

			while( signatureID >= receivedPropertyNameSignatures.Count )
				receivedPropertyNameSignatures.Add( "" );
			receivedPropertyNameSignatures[ signatureID ] = signature;

			return true;
		}

		byte[] tempSpecialSerializedData;

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSetPropertyValueToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var size = reader.EndPosition - reader.CurrentPosition;

			var componentID = (long)reader.ReadVariableUInt64();

			var signatureIDAndSpecialSerialized = reader.ReadVariableInt32();
			var signatureID = Math.Abs( signatureIDAndSpecialSerialized );
			var specialSerialized = signatureIDAndSpecialSerialized < 0;

			string valueString = "";
			if( specialSerialized )
			{
				var specialDataSize = reader.ReadVariableUInt32();
				if( tempSpecialSerializedData == null || tempSpecialSerializedData.Length < specialDataSize )
					tempSpecialSerializedData = new byte[ specialDataSize ];
				reader.ReadBuffer( tempSpecialSerializedData, 0, (int)specialDataSize );
			}
			else
				valueString = reader.ReadString();

			if( !reader.Complete() )
				return false;


			//!!!!maybe cache Metadata.Property
			var signature = receivedPropertyNameSignatures[ signatureID ];

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive component set property: " + signature + " " + componentID.ToString() + " " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );

			if( owner.ProfilerData != null )
			{
				var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
				var messageTypeItem = serviceItem.GetMessageTypeItem( messageType.Identifier );

				if( messageTypeItem.ReceivedCustomData == null )
					messageTypeItem.ReceivedCustomData = new Dictionary<string, ClientNode.ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData>();

				messageTypeItem.ReceivedCustomData.TryGetValue( signature, out var item );
				item.Messages++;
				item.Size += size;
				messageTypeItem.ReceivedCustomData[ signature ] = item;
			}

			CheckSceneDisposed();
			if( scene != null )
			{
				var component = GetComponentByNetworkID( componentID );
				if( component != null )
				{
					var context = NetworkUtility.metadataGetMembersContextNoFilter;
					var property = component.MetadataGetMemberBySignature( signature, context ) as Metadata.Property;
					if( property != null )
					{
						if( specialSerialized )
						{
							ServerNetworkService_Components_SpecialSerialization.SetSpecialSerialized( property, component, tempSpecialSerializedData );
						}
						else
						{
							var emptyValue = string.IsNullOrEmpty( valueString );
							if( emptyValue )
							{
								var netType = property.Type.GetNetType();

								try
								{
									if( !netType.IsValueType )
									{
										property.SetValue( component, null, null );
									}
									else
									{
										if( ReferenceUtility.IsReferenceType( netType ) )
										{
											var value = (IReference)netType.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { null, "" } );
											property.SetValue( component, value, null );
										}
									}
								}
								catch { }
							}
							else
							{
								var block = TextBlock.Parse( valueString, out _ );
								if( block != null )
								{
									var context2 = new Metadata.LoadContext();
									MetadataManager.Serialization.LoadSerializableMember( context2, component, property, block, out _ );
								}
							}
						}

						//var memberData = new MetadataPropertyData( property );

						//if( !Serialization_LoadMemberRecursive( context, ref obj, memberData, block, out var error2 ) )
						//{
						//	error = error2 + $" Property \'{property.Name}\'.";
						//	return false;
						//}

					}
				}
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_SimulationStepToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//!!!!может какое-то время синхронизации
			//!!!!может указывать сколько вызовов передавать
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive simulation step ------------------------------------------------------------------" );

			CheckSceneDisposed();
			if( scene != null )
			{
				var controller = scene.ParentRoot.HierarchyController;
				controller.PerformSimulationStepClient();
			}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSendNetworkMessageNameToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var name = reader.ReadString();
			var nameID = reader.ReadVariableInt32();
			if( !reader.Complete() )
				return false;

			while( nameID >= receivedNetworkMessageNames.Count )
				receivedNetworkMessageNames.Add( "" );
			receivedNetworkMessageNames[ nameID ] = name;

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_ComponentSendNetworkMessageToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var size = reader.EndPosition - reader.CurrentPosition;

			var componentID = (long)reader.ReadVariableUInt64();
			var nameID = reader.ReadVariableInt32();
			if( reader.Overflow )
				return false;

			var message = receivedNetworkMessageNames[ nameID ];

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive network message: " + componentID.ToString() + " " + message );

			if( owner.ProfilerData != null )
			{
				var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
				var messageTypeItem = serviceItem.GetMessageTypeItem( messageType.Identifier );

				if( messageTypeItem.ReceivedCustomData == null )
					messageTypeItem.ReceivedCustomData = new Dictionary<string, ClientNode.ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData>();

				messageTypeItem.ReceivedCustomData.TryGetValue( message, out var item );
				item.Messages++;
				item.Size += size;
				messageTypeItem.ReceivedCustomData[ message ] = item;
			}

			CheckSceneDisposed();
			if( scene != null )
			{
				var component = GetComponentByNetworkID( componentID );
				if( component != null )
					return component.PerformReceiveNetworkMessageFromServer( message, reader );
			}

			//if( !reader.Complete() )
			//	return false;

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		private void NetworkInterface_BeginNetworkMessage( ComponentHierarchyController.NetworkClientInterface sender, Component component, string message, ref ArrayDataWriter writer )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				//get nameID, register and send
				int nameID;
				if( !registeredAndSentNetworkMessageNames.TryGetValue( message, out nameID ) )
				{
					//register
					nameID = registeredAndSentNetworkMessageNames.Count;
					registeredAndSentNetworkMessageNames[ message ] = nameID;

					//send
					var writer2 = BeginMessage( ComponentSendNetworkMessageName );
					writer2.Write( message );
					writer2.WriteVariableInt32( nameID );
					EndMessage();
				}

				//begin network message
				writer = BeginMessage( ComponentSendNetworkMessage );
				writer.WriteVariableInt32( sceneInstanceID );
				writer.WriteVariableUInt64( (ulong)component.networkID );
				writer.WriteVariableInt32( nameID );

				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Client: Send network message: " + component.networkID.ToString() + " " + message );

				sendingNetworkMessage = message;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		private void NetworkInterface_EndNetworkMessage( ComponentHierarchyController.NetworkClientInterface sender )
		{
			if( SendingData )
			{
				if( owner.ProfilerData != null )
				{
					var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
					var messageTypeItem = serviceItem.GetMessageTypeItem( ComponentSendNetworkMessage );// messageType.Identifier );

					if( messageTypeItem.SentCustomData == null )
						messageTypeItem.SentCustomData = new Dictionary<string, ClientNode.ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData>();

					messageTypeItem.SentCustomData.TryGetValue( sendingNetworkMessage, out var item );
					item.Messages++;
					item.Size += SendingDataWriterLength;
					messageTypeItem.SentCustomData[ sendingNetworkMessage ] = item;
				}

				EndMessage();

				sendingNetworkMessage = "";
			}
		}
	}
}