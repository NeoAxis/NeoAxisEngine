//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if !LIDGREN
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Net;
//using System.Runtime.CompilerServices;
//using WebSocketSharp;
//using WebSocketSharp.Server;

//namespace NeoAxis.Networking
//{
//	public abstract class NetworkNode
//	{
//		internal const int maxServiceIdentifier = 255;

//		//!!!!
//		//ArrayDataReader receiveDataReader = new ArrayDataReader();

//		bool disposed;

//		//profiler
//		ProfilerDataClass profilerData;

//		///////////////////////////////////////////////

//		public class ProfilerDataClass
//		{
//			public DateTime TimeStarted;
//			public float WorkingTime;

//			//public long TotalReceivedMessages;
//			public long TotalReceivedSize;
//			//public long TotalSentMessages;
//			public long TotalSentSize;
//			public List<ServiceItem> Services = new List<ServiceItem>();

//			/////////////////////

//			public class ServiceItem
//			{
//				public List<MessageTypeItem> MessagesByType = new List<MessageTypeItem>();

//				//

//				public class MessageTypeItem
//				{
//					public long ReceivedMessages;
//					public long ReceivedSize;
//					public long SentMessages;
//					public long SentSize;

//					public struct CustomData
//					{
//						public long Messages;
//						public long Size;
//					}
//					public Dictionary<string, CustomData> ReceivedCustomData;
//					public Dictionary<string, CustomData> SentCustomData;
//				}

//				//

//				public MessageTypeItem GetMessageTypeItem( int identifier )
//				{
//					while( identifier >= MessagesByType.Count )
//						MessagesByType.Add( null );
//					var item = MessagesByType[ identifier ];
//					if( item == null )
//					{
//						item = new MessageTypeItem();
//						MessagesByType[ identifier ] = item;
//					}
//					return item;
//				}
//			}

//			/////////////////////

//			public ServiceItem GetServiceItem( int identifier )
//			{
//				while( identifier >= Services.Count )
//					Services.Add( null );
//				var item = Services[ identifier ];
//				if( item == null )
//				{
//					item = new ServiceItem();
//					Services[ identifier ] = item;
//				}
//				return item;
//			}
//		}

//		///////////////////////////////////////////////

//		protected NetworkNode()
//		{
//		}

//		internal void NetworkNode_Dispose()
//		{
//			disposed = true;
//		}

//		protected virtual void OnUpdate()
//		{
//			if( ProfilerData != null )
//			{
//				var workedTime = DateTime.Now - ProfilerData.TimeStarted;
//				if( workedTime.TotalSeconds >= ProfilerData.WorkingTime )
//					ProfilerStop( true );
//			}
//		}

//		public void Update()
//		{
//			OnUpdate();
//		}

//		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
//		//internal void ProcessReceivedMessage( ConnectedClient connectedNode, byte[] data, int position, int length )
//		//{
//		//	zzzz;//client only

//		//	var reader = new ArrayDataReader( data, position, length );

//		//	var pair = reader.ReadByte();
//		//	var serviceIdentifier = (byte)( pair >> 4 );
//		//	var messageIdentifier = (byte)( pair & 15 );

//		//	if( reader.Overflow )
//		//	{
//		//		OnReceiveProtocolErrorInternal( connectedNode, "Invalid message." );
//		//		return;
//		//	}

//		//	//service message
//		//	var service = GetService( serviceIdentifier );
//		//	if( service == null )
//		//	{
//		//		//no such service
//		//		return;
//		//	}

//		//	service.ProcessReceivedMessage( connectedNode, reader, length, messageIdentifier );
//		//}

//		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
//		//internal void ProcessReceivedMessage( ConnectedNode connectedNode, NetIncomingMessage incomingMessage, int bytePosition, int byteLength )
//		//{
//		//	receiveDataReader.Init( incomingMessage.PeekDataBuffer(), bytePosition, byteLength );

//		//	var pair = receiveDataReader.ReadByte();
//		//	var serviceIdentifier = (byte)( pair >> 4 );
//		//	var messageIdentifier = (byte)( pair & 15 );

//		//	if( receiveDataReader.Overflow )
//		//	{
//		//		OnReceiveProtocolErrorInternal( connectedNode, "Invalid message." );
//		//		return;
//		//	}

//		//	//service message
//		//	var service = GetService( serviceIdentifier );
//		//	if( service == null )
//		//	{
//		//		//no such service
//		//		return;
//		//	}

//		//	service.ProcessReceivedMessage( connectedNode, receiveDataReader, byteLength, messageIdentifier );
//		//}

//		public bool Disposed
//		{
//			get { return disposed; }
//		}

//		//internal abstract NetworkService GetService( int identifier );
//		//internal abstract NetworkService GetService( string name );

//		//internal abstract void OnReceiveProtocolErrorInternal( ConnectedClient connectedNode, string message );

//		public ProfilerDataClass ProfilerData
//		{
//			get { return profilerData; }
//		}

//		public void ProfilerStart( float workingTime )
//		{
//			ProfilerStop( false );
//			profilerData = new ProfilerDataClass();
//			profilerData.TimeStarted = DateTime.Now;
//			profilerData.WorkingTime = workingTime;

//			Log.Info( "Network profiler started." );
//		}

//		public void ProfilerStop( bool writeToLogs )
//		{
//			if( profilerData == null )
//				return;
//			Log.Info( "Network profiler stopped." );
//			if( writeToLogs )
//				DumpProfilerDataToLogs();
//			profilerData = null;
//		}

//		static string FormatCount( long count )
//		{
//			return count.ToString( "N0" );
//		}

//		static string FormatSize( long byteCount )
//		{
//			//copyright: from LiteDB
//			var suf = new[] { "B", "KB", "MB", "GB", "TB" }; //Longs run out around EB
//			if( byteCount == 0 ) return "0 " + suf[ 0 ];
//			var bytes = Math.Abs( byteCount );
//			var place = Convert.ToInt64( Math.Floor( Math.Log( bytes, 1024 ) ) );
//			var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
//			return ( Math.Sign( byteCount ) * num ).ToString() + " " + suf[ place ];
//		}

//		void DumpProfilerDataToLogs()
//		{
//			var lines = new List<string>();

//			lines.Add( "--------------------------------------------------------------" );
//			lines.Add( string.Format( "Total received; {0}", FormatSize( profilerData.TotalReceivedSize ) ) );
//			//lines.Add( string.Format( "Total received. Messages: {0}; Size: {1}", FormatCount( profilerData.TotalReceivedMessages ), FormatSize( profilerData.TotalReceivedSize ) ) );

//			for( int serviceId = 0; serviceId < profilerData.Services.Count; serviceId++ )
//			{
//				var serviceItem = profilerData.Services[ serviceId ];
//				if( serviceItem != null )
//				{
//					var service = GetService( serviceId );
//					lines.Add( string.Format( "> {0}", service.Name ) );

//					var messageByTypeItems = new List<(ProfilerDataClass.ServiceItem.MessageTypeItem, int)>();

//					for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
//					{
//						var messageType = service.GetMessageType( messageTypeId );
//						if( messageType != null )
//						{
//							var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
//							if( messageByTypeItem != null && messageByTypeItem.ReceivedMessages != 0 )
//								messageByTypeItems.Add( (messageByTypeItem, messageTypeId) );
//						}
//					}

//					CollectionUtility.MergeSort( messageByTypeItems, delegate ( (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item1, (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item2 )
//					{
//						if( item1.Item1.ReceivedSize > item2.Item1.ReceivedSize )
//							return -1;
//						if( item1.Item1.ReceivedSize < item2.Item1.ReceivedSize )
//							return 1;
//						return 0;
//					} );

//					foreach( var messageByTypeItemPair in messageByTypeItems )
//					{
//						var messageByTypeItem = messageByTypeItemPair.Item1;
//						var messageTypeId = messageByTypeItemPair.Item2;

//						var messageType = service.GetMessageType( messageTypeId );

//						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), FormatSize( messageByTypeItem.ReceivedSize ) ) );

//						var customData = messageByTypeItem.ReceivedCustomData;
//						if( customData != null )
//						{
//							var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
//							foreach( var item in customData )
//								items.Add( (item.Key, item.Value) );

//							CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
//							{
//								if( item1.Item2.Size > item2.Item2.Size )
//									return -1;
//								if( item1.Item2.Size < item2.Item2.Size )
//									return 1;
//								return 0;
//							} );

//							foreach( var item in items )
//							{
//								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
//							}
//						}
//					}


//					//for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
//					//{
//					//	var messageType = service.GetMessageType( (byte)messageTypeId );
//					//	if( messageType != null )
//					//	{
//					//		var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
//					//		if( messageByTypeItem != null && messageByTypeItem.ReceivedMessages != 0 )
//					//		{
//					//			lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), FormatSize( messageByTypeItem.ReceivedSize ) ) );

//					//			var customData = messageByTypeItem.ReceivedCustomData;
//					//			if( customData != null )
//					//			{
//					//				var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
//					//				foreach( var item in customData )
//					//					items.Add( (item.Key, item.Value) );

//					//				CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
//					//				{
//					//					if( item1.Item2.Size > item2.Item2.Size )
//					//						return -1;
//					//					if( item1.Item2.Size < item2.Item2.Size )
//					//						return 1;
//					//					return 0;
//					//				} );

//					//				foreach( var item in items )
//					//				{
//					//					lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
//					//				}
//					//			}
//					//		}
//					//	}
//					//}
//				}
//			}

//			lines.Add( "--------------------------------------------------------------" );
//			lines.Add( string.Format( "Total sent; Size: {0}", FormatSize( profilerData.TotalSentSize ) ) );
//			//lines.Add( string.Format( "Total sent. Messages: {0}; Size: {1}", FormatCount( profilerData.TotalSentMessages ), FormatSize( profilerData.TotalSentSize ) ) );

//			for( int serviceId = 0; serviceId < profilerData.Services.Count; serviceId++ )
//			{
//				var serviceItem = profilerData.Services[ serviceId ];
//				if( serviceItem != null )
//				{
//					var service = GetService( serviceId );
//					lines.Add( string.Format( "> {0}", service.Name ) );

//					var messageByTypeItems = new List<(ProfilerDataClass.ServiceItem.MessageTypeItem, int)>();

//					for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
//					{
//						var messageType = service.GetMessageType( messageTypeId );
//						if( messageType != null )
//						{
//							var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
//							if( messageByTypeItem != null && messageByTypeItem.SentMessages != 0 )
//								messageByTypeItems.Add( (messageByTypeItem, messageTypeId) );
//						}
//					}

//					CollectionUtility.MergeSort( messageByTypeItems, delegate ( (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item1, (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item2 )
//					{
//						if( item1.Item1.SentSize > item2.Item1.SentSize )
//							return -1;
//						if( item1.Item1.SentSize < item2.Item1.SentSize )
//							return 1;
//						return 0;
//					} );

//					foreach( var messageByTypeItemPair in messageByTypeItems )
//					{
//						var messageByTypeItem = messageByTypeItemPair.Item1;
//						var messageTypeId = messageByTypeItemPair.Item2;

//						var messageType = service.GetMessageType( messageTypeId );

//						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), FormatSize( messageByTypeItem.SentSize ) ) );

//						var customData = messageByTypeItem.SentCustomData;
//						if( customData != null )
//						{
//							var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
//							foreach( var item in customData )
//								items.Add( (item.Key, item.Value) );

//							CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
//							{
//								if( item1.Item2.Size > item2.Item2.Size )
//									return -1;
//								if( item1.Item2.Size < item2.Item2.Size )
//									return 1;
//								return 0;
//							} );

//							foreach( var item in items )
//							{
//								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
//							}
//						}
//					}


//					//for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
//					//{
//					//	var messageType = service.GetMessageType( messageTypeId );
//					//	if( messageType != null )
//					//	{
//					//		var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
//					//		if( messageByTypeItem != null && messageByTypeItem.SentMessages != 0 )
//					//		{
//					//			lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), FormatSize( messageByTypeItem.SentSize ) ) );

//					//			var customData = messageByTypeItem.SentCustomData;
//					//			if( customData != null )
//					//			{
//					//				var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
//					//				foreach( var item in customData )
//					//					items.Add( (item.Key, item.Value) );

//					//				CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
//					//				{
//					//					if( item1.Item2.Size > item2.Item2.Size )
//					//						return -1;
//					//					if( item1.Item2.Size < item2.Item2.Size )
//					//						return 1;
//					//					return 0;
//					//				} );

//					//				foreach( var item in items )
//					//				{
//					//					lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
//					//				}
//					//			}
//					//		}
//					//	}
//					//}
//				}
//			}

//			lines.Add( "--------------------------------------------------------------" );

//			var result = "";
//			foreach( var line in lines )
//			{
//				if( result != "" )
//					result += "\r\n";
//				result += line;
//			}
//			Log.Info( result );
//		}
//	}
//}
//#endif