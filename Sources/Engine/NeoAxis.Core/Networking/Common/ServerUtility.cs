// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Networking
{
	static class ServerUtility
	{
		public static string GetServerNodesCommonInfoText()
		{
			var text = new StringBuilder();//var text = new StringBuilder( "Server mode.\n" );
			text.Append( "Engine time: " + EngineApp.EngineTime.ToString( "F2" ) + "\n" );

			var instances = ServerNode.GetInstances();
			text.Append( $"Server nodes: {instances.Length}\n" );
			if( !string.IsNullOrEmpty( ServerNode.BeginListenLastError ) )
				text.Append( $"Begin listen last error: {ServerNode.BeginListenLastError}\n" );
			text.Append( "\n" );

			foreach( var instance in instances )
			{
				text.Append( $"{instance.ServerName}\nClients: {instance.ClientCount}\n" );

				var components = instance.GetService( "Components" ) as ServerNetworkService_Components;
				var scene = components?.Scene;
				if( scene != null )
				{
					var sceneInfo = components.SceneInfo ?? "";

					text.Append( $"A scene is loaded.\n" );
					text.Append( $"Scene info: " + sceneInfo + "\n" );

					//text.Append( $"A scene is loaded with a scene info \"{sceneInfo}\".\n" );
				}

				text.Append( $"Received total: {instance.TotalDataMessagesReceivedCounter} messages, {instance.TotalDataSizeReceivedCounter} bytes.\n" );
				text.Append( $"Sent total: {instance.TotalDataMessagesSentCounter} messages, {instance.TotalDataSizeSentCounter} bytes.\n" );

				instance.GetDataMessageStatistics( 1, out var receivedMessages, out var receivedSize, out var sentMessages, out var sentSize );

				var receivedSpeed = StringUtility.FormatSize( (long)receivedSize );
				var sentSpeed = StringUtility.FormatSize( (long)sentSize );

				text.Append( $"Received speed: {receivedMessages} messages per second, {receivedSpeed} per second.\n" );
				text.Append( $"Sent speed: {sentMessages} messages per second, {sentSpeed} per second.\n" );

				text.Append( "\n" );
			}

			return text.ToString();
		}
	}
}
