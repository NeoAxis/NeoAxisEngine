//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;

//namespace NeoAxis.Networking
//{
//	public abstract class NetworkService
//	{
//		internal const int maxMessageTypeIdentifier = 15;// ( 1 << NetworkDefines.bitsForServiceMessageTypeIdentifier ) - 1;

//		//

//		internal NetworkNode baseOwner;

//		//general
//		string name;
//		int identifier;

//		//message types
//		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
//		List<MessageType> messageTypesByID = new List<MessageType>();

//		///////////////////////////////////////////

//		public sealed class MessageType
//		{
//			string name;
//			int identifier;
//			ReceiveHandlerDelegate receiveHandler;

//			///////////////

//			public delegate bool ReceiveHandlerDelegate( NetworkNode.ConnectedClient sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage );

//			///////////////

//			internal MessageType( string name, int identifier, ReceiveHandlerDelegate receiveHandler )
//			{
//				this.name = name;
//				this.identifier = identifier;
//				this.receiveHandler = receiveHandler;
//			}

//			public string Name
//			{
//				get { return name; }
//			}

//			public int Identifier
//			{
//				get { return identifier; }
//			}

//			public ReceiveHandlerDelegate ReceiveHandler
//			{
//				get { return receiveHandler; }
//			}
//		}

//		///////////////////////////////////////////

//		protected NetworkService( string name, int identifier )
//		{
//			this.name = name;
//			this.identifier = identifier;
//		}

//		public string Name
//		{
//			get { return name; }
//		}

//		public int Identifier
//		{
//			get { return identifier; }
//		}

//		protected virtual void OnDispose()
//		{
//			//baseOwner = null;
//		}

//		internal void PerformDispose()
//		{
//			OnDispose();
//		}

//		protected MessageType RegisterMessageType( string name, int identifier, MessageType.ReceiveHandlerDelegate receiveHandler )
//		{
//			if( identifier == 0 )
//				Log.Fatal( "NetworkService: RegisterMessageType: Invalid message type identifier. Identifier can not be zero." );

//			if( identifier > maxMessageTypeIdentifier )
//				Log.Fatal( "NetworkService: RegisterMessageType: Invalid message type identifier. Max identifier is \"{0}\".", maxMessageTypeIdentifier );

//			if( GetMessageType( name ) != null )
//				Log.Fatal( "NetworkService: RegisterMessageType: Message type \"{0}\" is already registered.", name );

//			if( GetMessageType( identifier ) != null )
//				Log.Fatal( "NetworkService: RegisterMessageType: Message type with identifier \"{0}\" is already registered.", identifier );

//			var messageType = new MessageType( name, identifier, receiveHandler );
//			messageTypesByName.Add( name, messageType );

//			while( messageTypesByID.Count <= identifier )
//				messageTypesByID.Add( null );
//			messageTypesByID[ identifier ] = messageType;

//			return messageType;
//		}

//		protected MessageType RegisterMessageType( string name, int identifier )
//		{
//			return RegisterMessageType( name, identifier, null );
//		}

//		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
//		public MessageType GetMessageType( string name )
//		{
//			MessageType messageType;
//			if( !messageTypesByName.TryGetValue( name, out messageType ) )
//				return null;
//			return messageType;
//		}

//		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
//		public MessageType GetMessageType( int identifier )
//		{
//			if( identifier >= messageTypesByID.Count )
//				return null;
//			return messageTypesByID[ identifier ];
//		}

//		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
//		internal void ProcessReceivedMessage( NetworkNode.ConnectedClient connectedNode, ArrayDataReader reader, int lengthForProfiler, int messageIdentifier )
//		{
//			var messageType = GetMessageType( messageIdentifier );
//			if( messageType == null )
//			{
//				//no such message type
//				return;
//			}

//			//!!!!some data can be received, but message type is not registered
//			if( baseOwner.ProfilerData != null )
//			{
//				var serviceItem = baseOwner.ProfilerData.GetServiceItem( Identifier );
//				var messageTypeItem = serviceItem.GetMessageTypeItem( messageType.Identifier );
//				messageTypeItem.ReceivedMessages++;
//				messageTypeItem.ReceivedSize += lengthForProfiler;
//			}

//			if( messageType.ReceiveHandler == null )
//			{
//				//no receive handler
//				return;
//			}

//			string additionalErrorMessage = null;
//			if( !messageType.ReceiveHandler( connectedNode, messageType, reader, ref additionalErrorMessage ) )
//			{
//				var text = string.Format( "Invalid service message \'{0}\'.", messageType.Name );
//				if( !string.IsNullOrEmpty( additionalErrorMessage ) )
//					text += " " + additionalErrorMessage;
//				baseOwner.OnReceiveProtocolErrorInternal( connectedNode, text );
//				return;
//			}
//		}

//		protected internal virtual void OnUpdate() { }
//	}
//}
