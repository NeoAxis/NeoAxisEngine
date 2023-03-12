// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ClientNetworkService : NetworkService
	{
		internal NetworkClientNode owner;

		//send data
		bool sendingData;
		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////

		protected ClientNetworkService( string name, byte identifier )
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

		protected ArrayDataWriter BeginMessage( byte messageID )
		{
			if( sendingData )
				Log.Fatal( "ClientNetworkService: BeginMessage: The message is already begun." );
			if( messageID <= 0 || messageID > 255 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 255." );
			//if( messageType == null )
			//	Log.Fatal( "ClientNetworkService: BeginMessage: messageType = null." );

			sendingData = true;
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			sendDataWriter.Write( (byte)messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		protected void EndMessage()
		{
			if( !sendingData )
				Log.Fatal( "ClientNetworkService: EndMessage: The message is not begun." );

			if( owner != null && owner.client != null )
			{
				var connectedNode = Owner.ServerConnectedNode;
				connectedNode.AddDataForSending( sendDataWriter );
			}

			sendingData = false;
			sendDataWriter.Reset();
		}
	}
}
