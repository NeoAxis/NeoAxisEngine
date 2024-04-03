// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ClientNetworkService : NetworkService
	{
		internal NetworkClientNode owner;

		//send data
		bool sendingData;
		int sendingMessageID;
		ArrayDataWriter sendingDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////

		protected ClientNetworkService( string name, int identifier )
			: base( name, identifier )
		{
		}

		public NetworkClientNode Owner
		{
			get { return owner; }
		}

		protected override void OnDispose()
		{
			//owner = null;
			base.OnDispose();
		}

		public bool SendingData
		{
			get { return sendingData; }
		}

		public int SendingDataWriterLength
		{
			get { return sendingDataWriter.Length; }
		}

		[ MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( int messageID )
		{
			if( sendingData )
				Log.Fatal( "ClientNetworkService: BeginMessage: The message is already begun." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ClientNetworkService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			sendingDataWriter.Reset();

			sendingDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );
			//sendDataWriter.Write( (byte)Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			//sendDataWriter.Write( (byte)messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendingDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected void EndMessage()
		{
			if( !sendingData )
				Log.Fatal( "ClientNetworkService: EndMessage: The message is not begun." );

			if( owner != null && owner.client != null )
			{
				var connectedNode = Owner.ServerConnectedNode;
				connectedNode.AddDataForSending( sendingDataWriter );

				if( owner.ProfilerData != null )
				{
					var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
					var messageTypeItem = serviceItem.GetMessageTypeItem( sendingMessageID );
					messageTypeItem.SentMessages++;
					messageTypeItem.SentSize += sendingDataWriter.Length;
				}
			}

			sendingData = false;
			sendingMessageID = -1;
			sendingDataWriter.Reset();
		}
	}
}
