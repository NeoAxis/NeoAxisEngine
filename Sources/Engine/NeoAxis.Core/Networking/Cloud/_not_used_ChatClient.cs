//// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Collections.Concurrent;

//namespace NeoAxis.Networking
//{
//	/// <summary>
//	/// A client to access the Chat service. Another way is using ClientNetworkService_Chat inside your service node.
//	/// </summary>
//	public class ChatClient : CloudFunctionsClient // !!!! BasicServiceClient
//	{
//		//static List<ChatClient> instances;
//		//static ChatClient firstInstance;

//		//!!!!
//		string languages = string.Empty;

//		long requestIdCounter;

//		ConcurrentDictionary<long, AnswerItem> answers = new ConcurrentDictionary<long, AnswerItem>();
//		DateTime answersLastOldRemoveTime;

//		///////////////////////////////////////////////

//		public class CreateResult
//		{
//			public ChatClient Client;
//			public string Error;
//		}

//		///////////////////////////////////////////////

//		class AnswerItem
//		{
//			public MultiChatResult Result;
//			public DateTime CreationTime;
//		}

//		///////////////////////////////////////////////

//		public enum FormatEnum
//		{
//			Text,
//			Html,
//		}

//		///////////////////////////////////////////////

//		public class SimpleResult
//		{
//			public string Error { get; set; }
//		}

//		///////////////////////////////////////////////

//		public class CreateProfileResult
//		{
//			public long ProfileID { get; set; }
//			public string Error { get; set; }
//		}

//		///////////////////////////////////////////////

//		public class GetProfilesResult
//		{
//			public long[] ProfileIDs { get; set; }
//			public string Error { get; set; }
//		}

//		///////////////////////////////////////////////

//		public class SayResult
//		{
//			public long MessageID { get; set; }
//			public string Error { get; set; }
//		}

//		///////////////////////////////////////////////

//		static ChatClient()
//		{
//			//instances = new List<ChatClient>();
//		}

//		ChatClient( bool autoUpdate )
//			: base( autoUpdate )
//		{
//			ServiceName = "Chat";
//		}

//		//public static ChatClient[] GetInstances()
//		//{
//		//	lock( instances )
//		//		return instances.ToArray();
//		//}

//		//public static ChatClient FirstInstance
//		//{
//		//	get { return firstInstance; }
//		//}

//		public static async Task<CreateResult> CreateAsync( ConnectionSettingsClass connectionSettings, bool autoUpdate, bool connect )
//		{
//			var instance = new ChatClient( autoUpdate );
//			instance.ConnectionSettings = connectionSettings;

//			//lock( instances )
//			//{
//			//	instances.Add( instance );
//			//	firstInstance = instances.Count > 0 ? instances[ 0 ] : null;
//			//}

//			if( connect )
//			{
//				var error = await instance.ReconnectAsync();
//				if( !string.IsNullOrEmpty( error ) )
//					return new CreateResult() { Error = error };
//			}

//			return new CreateResult() { Client = instance };
//		}

//		public string Languages
//		{
//			get { return languages; }
//		}

//		void RemoveOldNotUsedAnswers( DateTime now )
//		{
//			foreach( var pair in answers.ToArray() )
//			{
//				var requestID = pair.Key;
//				var item = pair.Value;

//				if( ( now - item.CreationTime ).TotalMinutes > 10 )
//					answers.Remove( requestID, out _ );
//			}
//		}

//		protected override void OnUpdate()
//		{
//			base.OnUpdate();

//			var now = DateTime.UtcNow;
//			if( ( now - answersLastOldRemoveTime ).TotalSeconds > 10 )
//			{
//				RemoveOldNotUsedAnswers( now );
//				answersLastOldRemoveTime = now;
//			}
//		}

//		protected override void OnDestroy()
//		{
//			//lock( instances )
//			//{
//			//	instances.Remove( this );
//			//	firstInstance = instances.Count > 0 ? instances[ 0 ] : null;
//			//}

//			base.OnDestroy();
//		}

//		long GetRequestID()
//		{
//			return Interlocked.Increment( ref requestIdCounter );
//		}

//		AnswerItem GetAnswerAndRemove( long requestID )
//		{
//			if( answers.Remove( requestID, out var item ) )
//				return item;
//			return null;
//		}

//		public void SetCurrentProfile( long profileID )
//		{
//			//!!!!
//		}

//		///////////////////////////////////////////////

//		public async Task<CreateProfileResult> CreateProfileAsync( string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<SimpleResult> UpdateProfileAsync( long profileID, string QQQavatarFullPath, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<SimpleResult> DeleteProfileAsync( long profileID, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetProfilesResult> GetProfilesAsync( string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetProfileInfoResult> GetProfileInfoAsync( long profileID, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		///////////////////////////////////////////////

//		public async Task<CreateChannelResult> CreateChannelAsync( string name, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<SimpleResult> UpdateChannelAsync( long channelID, string name, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<SimpleResult> DeleteChannelAsync( long channelID, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetChannelInfoResult> GetChannelInfoAsync( long channelID, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetChannelMessagesResult> GetChannelMessagesAsync( long channelID, int page, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<SayResult> SayToChannelAsync( long channelID, string text, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		///////////////////////////////////////////////

//		public async Task<SayResult> SayPrivateAsync( long recepientProfileID, string text, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetDialogInfoResult> GetDialogInfoAsync( long anotherProfileID, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}

//		public async Task<GetDialogMessagesResult> GetDialogMessagesAsync( long anotherProfileID, int page, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			//!!!!
//			return null;
//		}



//		public async Task<MultiChatResult> GenerateAsync( string sourceLanguage, string generator, IList<string> texts, string anyData = null, CancellationToken cancellationToken = default )
//		{
//			надо выставить directory

//			var requestID = GetRequestID();

//			//!!!!GC. double copy. make internal pool for ArrayDataWriter?
//			var writer = new ArrayDataWriter( 1024 );
//			writer.WriteVariableUInt64( (ulong)requestID );
//			writer.Write( sourceLanguage );
//			writer.Write( generator );
//			writer.WriteVariableInt32( texts.Count );
//			for( int n = 0; n < texts.Count; n++ )
//				writer.Write( texts[ n ] );
//			//writer.Write( (byte)format );
//			writer.Write( anyData );
//			ConnectionNode.Messages.SendToServer( "Generate", writer.ToArraySegment() );

//			while( true )
//			{
//				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
//					return new MultiChatResult() { Error = ConnectionErrorReceived };

//				var answer = GetAnswerAndRemove( requestID );
//				if( answer != null )
//					return answer.Result;

//				await Task.Delay( 1 );
//				if( cancellationToken.IsCancellationRequested )
//					return new MultiChatResult { Error = "Operation was canceled." };
//			}
//		}

//		//public async Task<MultiChatResult> GenerateAsync( string sourceLanguage, string generator /*string targetLanguage*/, IList<string> texts/*, FormatEnum format = FormatEnum.Text*/, string anyData = null, CancellationToken cancellationToken = default )
//		//{
//		//	var requestID = GetRequestID();

//		//	//!!!!GC. double copy. make internal pool for ArrayDataWriter?
//		//	var writer = new ArrayDataWriter( 1024 );
//		//	writer.WriteVariableUInt64( (ulong)requestID );
//		//	writer.Write( sourceLanguage );
//		//	writer.Write( generator );
//		//	writer.WriteVariableInt32( texts.Count );
//		//	for( int n = 0; n < texts.Count; n++ )
//		//		writer.Write( texts[ n ] );
//		//	//writer.Write( (byte)format );
//		//	writer.Write( anyData );
//		//	ConnectionNode.Messages.SendToServer( "Generate", writer.ToArraySegment() );

//		//	while( true )
//		//	{
//		//		if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
//		//			return new MultiChatResult() { Error = ConnectionErrorReceived };

//		//		var answer = GetAnswerAndRemove( requestID );
//		//		if( answer != null )
//		//			return answer.Result;

//		//		await Task.Delay( 1 );
//		//		if( cancellationToken.IsCancellationRequested )
//		//			return new MultiChatResult { Error = "Operation was canceled." };
//		//	}
//		//}

//		//public async Task<ChatResult> GenerateAsync( string sourceLanguage, string generator, string text/*, FormatEnum format = FormatEnum.Text*/, string anyData = null, CancellationToken cancellationToken = default )
//		//{
//		//	var result = await GenerateAsync( sourceLanguage, generator, new string[] { text }, anyData, cancellationToken );
//		//	if( !string.IsNullOrEmpty( result.Error ) )
//		//		return new ChatResult() { Error = result.Error };
//		//	return new ChatResult() { TranslatedText = result.TranslatedTexts[ 0 ] };
//		//}

//		protected override void OnMessages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
//		{
//			base.OnMessages_ReceiveMessageString( sender, message, data );

//			if( message == "Languages" )
//				languages = data;
//		}

//		protected override void OnMessages_ReceiveMessageBinary( ClientNetworkService_Messages sender, string message, byte[] data )
//		{
//			base.OnMessages_ReceiveMessageBinary( sender, message, data );

//			if( message == "Answer" )
//			{
//				var answerItem = new AnswerItem();

//				var reader = new ArrayDataReader( data );

//				var requestID = (long)reader.ReadVariableUInt64();
//				var count = reader.ReadVariableInt32();
//				if( count != 0 )
//				{
//					var translatedTexts = new string[ count ];
//					for( int n = 0; n < count; n++ )
//						translatedTexts[ n ] = reader.ReadString() ?? string.Empty;
//					answerItem.Result = new MultiChatResult() { TranslatedTexts = translatedTexts };
//				}
//				else
//				{
//					answerItem.Result = new MultiChatResult() { Error = reader.ReadString() };
//				}

//				if( !reader.Complete() )
//					return;

//				answerItem.CreationTime = DateTime.UtcNow;
//				answers[ requestID ] = answerItem;
//			}
//		}
//	}
//}
