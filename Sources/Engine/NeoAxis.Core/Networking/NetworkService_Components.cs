// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NeoAxis.Networking;

namespace NeoAxis
{
	public class ServerNetworkService_Components : ServerNetworkService
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
		Dictionary<NetworkNode.ConnectedNode, ClientItem> clientItemByConnectedNode = new Dictionary<NetworkNode.ConnectedNode, ClientItem>();

		ClientItem[] allClientItemsCached;

		///////////////////////////////////////////

		public class ClientItem
		{
			//!!!!public

			public ServerNetworkService_Users.UserInfo User;

			//!!!!
			//bool sceneSynchronization;

			internal Dictionary<Component, Dictionary<Metadata.Property, string>> synchronizedFlagAndSentPropertyValues = new Dictionary<Component, Dictionary<Metadata.Property, string>>();
		}

		///////////////////////////////////////////

		const byte SceneCreateBeginToClient = 1;
		const byte SceneCreateEndToClient = 2;
		const byte SceneDestroyToClient = 3;
		//const byte SceneInstanceIDToServer = 4;

		const byte ComponentCreateBeginToClient = 10;
		const byte ComponentSetEnabledToClient = 11;
		const byte ComponentRemoveFromParentToClient = 12;
		const byte ComponentSetPropertyValueToClient = 13;
		const byte ComponentSendNetworkMessageToClient = 14;
		const byte SimulationStepToClient = 15;

		const byte ComponentSendNetworkMessageToServer = 16;

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
			RegisterMessageType( nameof( ComponentSetPropertyValueToClient ), ComponentSetPropertyValueToClient );
			RegisterMessageType( nameof( ComponentSendNetworkMessageToClient ), ComponentSendNetworkMessageToClient );
			RegisterMessageType( nameof( SimulationStepToClient ), SimulationStepToClient );

			RegisterMessageType( nameof( ComponentSendNetworkMessageToServer ), ComponentSendNetworkMessageToServer, ReceiveMessage_ComponentSendNetworkMessageToServer );

			//!!!!

			//!!!!
			//ChangeSimulationFlag


			users.AddUserEvent += Users_AddUserEvent;
			users.RemoveUserEvent += Users_RemoveUserEvent;
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

			users.AddUserEvent -= Users_AddUserEvent;
			users.RemoveUserEvent -= Users_RemoveUserEvent;

			base.OnDispose();
		}

		public Scene Scene
		{
			get { return scene; }
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

		public ClientItem GetClientItem( ServerNetworkService_Users.UserInfo user )
		{
			if( clientItemByUser.TryGetValue( user, out var value ) )
				return value;
			return null;
		}

		public ClientItem GetClientItem( long userID )
		{
			if( clientItemByUserID.TryGetValue( userID, out var value ) )
				return value;
			return null;
		}

		public ClientItem GetClientItem( NetworkNode.ConnectedNode connectedNode )
		{
			if( clientItemByConnectedNode.TryGetValue( connectedNode, out var value ) )
				return value;
			return null;
		}

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

		void Users_AddUserEvent( ServerNetworkService_Users service, ServerNetworkService_Users.UserInfo user )
		{
			var clientItem = new ClientItem();
			clientItem.User = user;

			clientItemByUser[ user ] = clientItem;
			clientItemByUserID[ user.UserID ] = clientItem;
			if( user.ConnectedNode != null )
				clientItemByConnectedNode[ user.ConnectedNode ] = clientItem;
			allClientItemsCached = null;
		}

		void Users_RemoveUserEvent( ServerNetworkService_Users service, ServerNetworkService_Users.UserInfo user )
		{
			var clientItem = GetClientItem( user );
			if( clientItem != null )
			{
				//!!!!там вызывался Server_OnClientDisconnected
				//networkingInterface.DisconnectRemoteEntityWorld( remoteEntityWorld );

				clientItemByUser.Remove( user );
				clientItemByUserID.Remove( user.UserID );
				if( user.ConnectedNode != null )
					clientItemByConnectedNode.Remove( user.ConnectedNode );
				allClientItemsCached = null;
			}
		}

		ArrayDataWriter BeginMessage( ICollection<ClientItem> clients, byte messageID )
		{
			var writer = BeginMessage( messageID );

			var asArray = clients as ClientItem[];
			if( asArray != null )
			{
				foreach( var client in asArray )
					AddMessageRecipient( client );
			}
			else
			{
				foreach( var client in clients )
					AddMessageRecipient( client );
			}

			return writer;
		}

		ArrayDataWriter BeginMessage( ClientItem client, byte messageID )
		{
			return BeginMessage( client.User.ConnectedNode, messageID );
		}

		void AddMessageRecipient( ClientItem client )
		{
			AddMessageRecipient( client.User.ConnectedNode );
		}

		//!!!!use array
		ICollection<ClientItem> GetClientsWithAccessToComponentByNetworkMode( Component component, ClientItem onlyThisClient )
		{
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

		List<ClientItem> GetSynchronizedClientsOfComponent( Component component, ClientItem onlyThisClient )
		{
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

		private void NetworkInterface_AddComponent( ComponentHierarchyController.NetworkServerInterface sender, Component child, bool createComponent )
		{
			SendAddComponent( null, child, createComponent );
		}

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

		private void NetworkInterface_RemoveFromParent( ComponentHierarchyController.NetworkServerInterface sender, Component component, bool queued, bool disposing )
		{
			SendRemoveFromParent( null, component, queued, disposing );
		}

		void SendComponentSetPropertyValue( ClientItem clientItem, Component component, Metadata.Property property )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				//fast exit
				if( clientItem != null && !clientItem.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
					return;

				var context = new Metadata.SaveContext();
				context.UseDefaultValue = false;

				//!!!!slowly?
				//!!!!не только строками передавать. тогда networkPropertyValuesSent иначе
				var block = new TextBlock();
				if( MetadataManager.Serialization.SaveSerializableMember( context, component, property, block, out var serialized, out var error ) && serialized )
				{
					var emptyValue = string.IsNullOrEmpty( block.Name ) && block.Children.Count == 0 && block.Attributes.Count == 0;
					var valueString = emptyValue ? "" : block.DumpToString( false );

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

								var writer = BeginMessage( client, ComponentSetPropertyValueToClient );
								writer.WriteVariableUInt64( (ulong)component.networkID );
								writer.Write( property.Signature );
								writer.Write( valueString );
								EndMessage();

								sentPropertyItem[ property ] = valueString;

								sent = true;
							}
						}
					}

					if( sent )
					{
						if( NetworkCommonSettings.NetworkLogging )
							Log.Info( "Network log: Server: Send component set property: " + property.Signature + ", Network ID: " + component.networkID.ToString() + ", Value: " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );

						////!!!!temp
						//if( property.Name == "BulletType" )
						//{
						//	Log.Info( "Network log: Server: Send component set property: " + property.Signature + ", Network ID: " + component.networkID.ToString() + ", Value: " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );
						//	Log.Info( "emptyValue: " + emptyValue.ToString() + " -:" + block.Data + ":" );

						//	var v = property.GetValue( component, null );
						//	if( v != null )
						//	{
						//		var vv = (IReference)v;
						//		Log.Info( "vv: " + vv.GetByReference.ToString() );
						//		Log.Info( "vvv: " + vv.ValueAsObject != null ? "not null" : "null" );
						//	}
						//	else
						//		Log.Info( "v: null" );

						//}

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

		private void NetworkInterface_PropertyChangedEvent( ComponentHierarchyController.NetworkServerInterface sender, Component component, Metadata.Property property )
		{
			SendComponentSetPropertyValue( null, component, property );
		}

		private void NetworkInterface_SimulationStep( ComponentHierarchyController.NetworkServerInterface sender )
		{
			BeginMessageToEveryone( SimulationStepToClient );
			EndMessage();

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Server: Send simulation step ------------------------------------------------------------------" );
		}

		private void NetworkInterface_BeginNetworkMessage( ComponentHierarchyController.NetworkServerInterface sender, Component component, IList<ClientItem> clientRecipients, ClientItem clientRecipient, IList<ServerNetworkService_Users.UserInfo> userRecipients, ServerNetworkService_Users.UserInfo userRecipient, bool toEveryone, string message, ref ArrayDataWriter writer )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				writer = BeginMessage( ComponentSendNetworkMessageToClient );

				if( toEveryone )
				{
					var allClients = GetAllClientItems();
					for( int n = 0; n < allClients.Length; n++ )
					{
						var client = allClients[ n ];
						if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							AddMessageRecipient( client );
					}
				}
				else if( clientRecipient != null )
				{
					if( clientRecipient.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
						AddMessageRecipient( clientRecipient );
				}
				else if( clientRecipients != null )
				{
					for( int n = 0; n < clientRecipients.Count; n++ )
					{
						var client = clientRecipients[ n ];
						if( client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							AddMessageRecipient( client );
					}
				}
				else if( userRecipient != null )
				{
					var client = GetClientItem( userRecipient );
					if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
						AddMessageRecipient( client );
				}
				else if( userRecipients != null )
				{
					for( int n = 0; n < userRecipients.Count; n++ )
					{
						var client = GetClientItem( userRecipients[ n ] );
						if( client != null && client.synchronizedFlagAndSentPropertyValues.ContainsKey( component ) )
							AddMessageRecipient( client );
					}
				}

				writer.WriteVariableUInt64( (ulong)component.networkID );
				writer.Write( message );

				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Server: Send network message: " + component.networkID.ToString() + " " + message );
			}
		}

		private void NetworkInterface_EndNetworkMessage( ComponentHierarchyController.NetworkServerInterface sender )
		{
			if( SendingData )
				EndMessage();
		}

		Component GetComponentByNetworkID( long networkID )
		{
			if( componentByNetworkID.TryGetValue( networkID, out var component ) )
				return component;
			return null;
		}

		private void NetworkInterface_GetComponentByNetworkID( ComponentHierarchyController.NetworkServerInterface sender, long networkID, ref Component component )
		{
			component = GetComponentByNetworkID( networkID );
		}

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

		private void NetworkInterface_NetworkModeAddUser( ComponentHierarchyController.NetworkServerInterface sender, ClientItem clientItem, Component component )
		{
			if( component.NetworkMode.Value == NetworkModeEnum.SelectedUsers )
				SendAddComponent( clientItem, component, false );
		}

		private void NetworkInterface_NetworkModeRemoveUser( ComponentHierarchyController.NetworkServerInterface sender, ClientItem clientItem, Component component )
		{
			if( component.NetworkMode.Value == NetworkModeEnum.SelectedUsers )
				SendRemoveFromParent( clientItem, component, false, true );
		}

		bool ReceiveMessage_ComponentSendNetworkMessageToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var sceneInstanceID = reader.ReadVariableInt32();
			var componentID = (long)reader.ReadVariableUInt64();
			var message = reader.ReadString();
			if( reader.Overflow )
				return false;

			if( this.sceneInstanceID == sceneInstanceID && scene != null && !scene.Disposed )
			{
				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Server: Receive network message: " + componentID.ToString() + " " + message );

				var component = GetComponentByNetworkID( componentID );
				if( component != null )
				{
					var client = GetClientItem( sender );
					if( client != null )
						return component.PerformReceiveNetworkMessageFromClient( client, message, reader );
				}

				//if( !reader.Complete() )
				//	return false;
			}

			return true;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Components : ClientNetworkService
	{
		ClientNetworkService_Users users;

		int sceneInstanceID;
		Scene scene;
		//!!!!проверить чтобы не копились
		//!!!!объект может быть удален на клиенте. хотя это вроде как норм, просто он тут уже не нужен
		Dictionary<long, Component> componentByNetworkID = new Dictionary<long, Component>();

		///////////////////////////////////////////

		public delegate void SceneCreateBeginDelegate( ClientNetworkService_Components sender, string sceneInfo );// string mapVirtualFileName );
		public event SceneCreateBeginDelegate SceneCreateBegin;

		public delegate void SceneCreateEndDelegate( ClientNetworkService_Components sender );
		public event SceneCreateEndDelegate SceneCreateEnd;

		public delegate void SceneDestroyDelegate( ClientNetworkService_Components sender );//, bool newMapWillBeLoaded );
		public event SceneDestroyDelegate SceneDestroy;

		///////////////////////////////////////////

		const byte SceneCreateBeginToClient = 1;
		const byte SceneCreateEndToClient = 2;
		const byte SceneDestroyToClient = 3;
		//const byte SceneInstanceIDToServer = 4;

		const byte ComponentCreateBeginToClient = 10;
		const byte ComponentSetEnabledToClient = 11;
		const byte ComponentRemoveFromParentToClient = 12;
		const byte ComponentSetPropertyValueToClient = 13;
		const byte ComponentSendNetworkMessageToClient = 14;
		const byte SimulationStepToClient = 15;

		const byte ComponentSendNetworkMessageToServer = 16;

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
			RegisterMessageType( nameof( ComponentSetPropertyValueToClient ), ComponentSetPropertyValueToClient, ReceiveMessage_ComponentSetPropertyValueToClient );
			RegisterMessageType( nameof( ComponentSendNetworkMessageToClient ), ComponentSendNetworkMessageToClient, ReceiveMessage_ComponentSendNetworkMessageToClient );
			RegisterMessageType( nameof( SimulationStepToClient ), SimulationStepToClient, ReceiveMessage_SimulationStepToClient );

			RegisterMessageType( nameof( ComponentSendNetworkMessageToServer ), ComponentSendNetworkMessageToServer );
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

		void CheckSceneDisposed()
		{
			if( scene != null && scene.Disposed )
				scene = null;
		}

		bool ReceiveMessage_SceneCreateBeginToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		bool ReceiveMessage_SceneCreateEndToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		bool ReceiveMessage_SceneDestroyToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		Component GetComponentByNetworkID( long networkID )
		{
			if( componentByNetworkID.TryGetValue( networkID, out var component ) )
				return component;
			return null;
		}

		private void NetworkInterface_GetComponentByNetworkID( ComponentHierarchyController.NetworkClientInterface sender, long networkID, ref Component component )
		{
			component = GetComponentByNetworkID( networkID );
		}

		bool ReceiveMessage_ComponentCreateBeginToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var parentComponentID = (long)reader.ReadVariableUInt64();
			var componentID = (long)reader.ReadVariableUInt64();
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

		bool ReceiveMessage_ComponentSetEnabledToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		bool ReceiveMessage_ComponentRemoveFromParentToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		bool ReceiveMessage_ComponentSetPropertyValueToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var componentID = (long)reader.ReadVariableUInt64();
			var signature = reader.ReadString();
			var valueString = reader.ReadString();
			if( !reader.Complete() )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive component set property: " + signature + " " + componentID.ToString() + " " + valueString.Replace( '\n', ' ' ).Replace( "\r", "" ) );

			CheckSceneDisposed();
			if( scene != null )
			{
				var component = GetComponentByNetworkID( componentID );
				if( component != null )
				{
					var property = component.MetadataGetMemberBySignature( signature ) as Metadata.Property;
					if( property != null )
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
								var context = new Metadata.LoadContext();
								MetadataManager.Serialization.LoadSerializableMember( context, component, property, block, out _ );
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

		bool ReceiveMessage_SimulationStepToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
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

		bool ReceiveMessage_ComponentSendNetworkMessageToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var componentID = (long)reader.ReadVariableUInt64();
			var message = reader.ReadString();
			if( reader.Overflow )
				return false;

			if( NetworkCommonSettings.NetworkLogging )
				Log.Info( "Network log: Client: Receive network message: " + componentID.ToString() + " " + message );

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

		private void NetworkInterface_BeginNetworkMessage( ComponentHierarchyController.NetworkClientInterface sender, Component component, string message, ref ArrayDataWriter writer )
		{
			if( component.networkID != 0 && !component.Disposed && !component.RemoveFromParentQueued )
			{
				writer = BeginMessage( ComponentSendNetworkMessageToServer );
				writer.WriteVariableInt32( sceneInstanceID );
				writer.WriteVariableUInt64( (ulong)component.networkID );
				writer.Write( message );

				if( NetworkCommonSettings.NetworkLogging )
					Log.Info( "Network log: Client: Send network message: " + component.networkID.ToString() + " " + message );
			}
		}

		private void NetworkInterface_EndNetworkMessage( ComponentHierarchyController.NetworkClientInterface sender )
		{
			if( SendingData )
				EndMessage();
		}

	}
}