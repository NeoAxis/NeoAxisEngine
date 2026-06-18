// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Networking;

namespace NeoAxis
{
	/// <summary>
	/// An internal server service to exchange system messages.
	/// </summary>
	class ServerNetworkService_Internal : ServerService
	{
		MessageType pingMessage;
		MessageType pongMessage;
		MessageType statusConnectedMessage;

		///////////////////////////////////////////

		public ServerNetworkService_Internal()
			: base( "Internal", 0 )
		{
			//register message types
			pingMessage = RegisterMessageType( "Ping", 1, ReceiveMessage_Ping );
			pongMessage = RegisterMessageType( "Pong", 2, ReceiveMessage_Pong );
			statusConnectedMessage = RegisterMessageType( "StatusConnected", 4 );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_Ping( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var messageNumberPing = reader.ReadUInt32();
			var messageNumberProcessedMessages = reader.ReadUInt32();
			if( !reader.Complete() )
				return false;
			SendPongToClient( sender, messageNumberPing );
			sender.ProcessMessageProcessed( messageNumberProcessedMessages );
			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_Pong( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var messageNumber = reader.ReadUInt32();
			if( !reader.Complete() )
				return false;
			sender.ProcessPong( messageNumber );
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendPingToClient( ServerNode.Client client, uint messageNumberPing, uint messageNumberProcessedMessages )
		{
			var m = BeginMessage( client, pingMessage );
			m.Writer.Write( messageNumberPing );
			m.Writer.Write( messageNumberProcessedMessages );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendPongToClient( ServerNode.Client client, uint messageNumber )
		{
			var m = BeginMessage( client, pongMessage );
			m.Writer.Write( messageNumber );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendStatusConnectedToClient( ServerNode.Client client )
		{
			var m = BeginMessage( client, statusConnectedMessage );
			m.End();
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An internal client service to exchange system messages.
	/// </summary>
	public class ClientNetworkService_Internal : ClientService
	{
		MessageType pingMessage;
		MessageType pongMessage;
		MessageType statusConnectedMessage;

		///////////////////////////////////////////

		public ClientNetworkService_Internal()
			: base( "Internal", 0 )
		{
			//register message types
			pingMessage = RegisterMessageType( "Ping", 1, ReceiveMessage_Ping );
			pongMessage = RegisterMessageType( "Pong", 2, ReceiveMessage_Pong );
			statusConnectedMessage = RegisterMessageType( "StatusConnected", 4, ReceiveMessage_StatusConnected );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_Ping( MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var messageNumberPing = reader.ReadUInt32();
			var messageNumberProcessedMessages = reader.ReadUInt32();
			if( !reader.Complete() )
				return false;
			SendPongToServer( messageNumberPing );
			owner.ProcessMessageProcessed( messageNumberProcessedMessages );
			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_Pong( MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var messageNumber = reader.ReadUInt32();
			if( !reader.Complete() )
				return false;
			owner.ProcessPong( messageNumber );
			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool ReceiveMessage_StatusConnected( MessageType messageType, ArrayDataReader reader, ref string error )
		{
			if( !reader.Complete() )
				return false;
			owner.ReceivedMessageSetStatusConnected();
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendPingToServer( uint messageNumberPing, uint messageNumberProcessedMessages )
		{
			var m = BeginMessage( pingMessage );
			m.Writer.Write( messageNumberPing );
			m.Writer.Write( messageNumberProcessedMessages );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendPongToServer( uint messageNumber )
		{
			var m = BeginMessage( pongMessage );
			m.Writer.Write( messageNumber );
			m.End();
		}
	}
}