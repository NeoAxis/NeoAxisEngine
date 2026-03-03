//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Collections.Concurrent;

//namespace NeoAxis.Networking
//{
//	/// <summary>
//	/// A client to access the Translate service.
//	/// </summary>
//	public class TextToSpeechClient : CloudFunctionsClient // !!!! BasicServiceClient
//	{
//		//static List<TextToSpeechClient> instances;
//		//static TextToSpeechClient firstInstance;

//		//!!!!
//		string languages = string.Empty;

//		long requestIdCounter;

//		ConcurrentDictionary<long, AnswerItem> answers = new ConcurrentDictionary<long, AnswerItem>();
//		DateTime answersLastOldRemoveTime;

//		///////////////////////////////////////////////

//		public class CreateResult
//		{
//			public TextToSpeechClient Client;
//			public string Error;
//		}

//		///////////////////////////////////////////////

//		class AnswerItem
//		{
//			public MultiTextToSpeechResult Result;
//			public DateTime CreationTime;
//		}

//		///////////////////////////////////////////////

//		public enum FormatEnum
//		{
//			Text,
//			Html,
//		}

//		///////////////////////////////////////////////

//		public class MultiTextToSpeechResult
//		{
//			public string[] OutputFiles { get; set; }
//			public string Error { get; set; }
//		}

//		///////////////////////////////////////////////

//		//public class TextToSpeechResult
//		//{

//		//	public string RecordedFile { get; set; }
//		//	//public string TranslatedText { get; set; }
//		//	public string Error { get; set; }
//		//}

//		///////////////////////////////////////////////

//		static TextToSpeechClient()
//		{
//			//instances = new List<TextToSpeechClient>();
//		}

//		TextToSpeechClient( bool autoUpdate )
//			: base( autoUpdate )
//		{
//			ServiceName = "TextToSpeech";
//		}

//		//public static TextToSpeechClient[] GetInstances()
//		//{
//		//	lock( instances )
//		//		return instances.ToArray();
//		//}

//		//public static TextToSpeechClient FirstInstance
//		//{
//		//	get { return firstInstance; }
//		//}

//		public static async Task<CreateResult> CreateAsync( ConnectionSettingsClass connectionSettings, bool autoUpdate, bool connect )
//		{
//			var instance = new TextToSpeechClient( autoUpdate );
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

//		public void SetOutputDirectory( string fullPath, float holdGeneratedFilesInSeconds, bool clearDirectoryFromOldAnswers )
//		{
//		}

//		public async Task<MultiTextToSpeechResult> GenerateAsync( string sourceLanguage, string generator, IList<string> texts, string anyData = null, CancellationToken cancellationToken = default )
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
//					return new MultiTextToSpeechResult() { Error = ConnectionErrorReceived };

//				var answer = GetAnswerAndRemove( requestID );
//				if( answer != null )
//					return answer.Result;

//				await Task.Delay( 1 );
//				if( cancellationToken.IsCancellationRequested )
//					return new MultiTextToSpeechResult { Error = "Operation was canceled." };
//			}
//		}

//		//public async Task<MultiTextToSpeechResult> GenerateAsync( string sourceLanguage, string generator /*string targetLanguage*/, IList<string> texts/*, FormatEnum format = FormatEnum.Text*/, string anyData = null, CancellationToken cancellationToken = default )
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
//		//			return new MultiTextToSpeechResult() { Error = ConnectionErrorReceived };

//		//		var answer = GetAnswerAndRemove( requestID );
//		//		if( answer != null )
//		//			return answer.Result;

//		//		await Task.Delay( 1 );
//		//		if( cancellationToken.IsCancellationRequested )
//		//			return new MultiTextToSpeechResult { Error = "Operation was canceled." };
//		//	}
//		//}

//		//public async Task<TextToSpeechResult> GenerateAsync( string sourceLanguage, string generator, string text/*, FormatEnum format = FormatEnum.Text*/, string anyData = null, CancellationToken cancellationToken = default )
//		//{
//		//	var result = await GenerateAsync( sourceLanguage, generator, new string[] { text }, anyData, cancellationToken );
//		//	if( !string.IsNullOrEmpty( result.Error ) )
//		//		return new TextToSpeechResult() { Error = result.Error };
//		//	return new TextToSpeechResult() { TranslatedText = result.TranslatedTexts[ 0 ] };
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
//					answerItem.Result = new MultiTextToSpeechResult() { TranslatedTexts = translatedTexts };
//				}
//				else
//				{
//					answerItem.Result = new MultiTextToSpeechResult() { Error = reader.ReadString() };
//				}

//				if( !reader.Complete() )
//					return;

//				answerItem.CreationTime = DateTime.UtcNow;
//				answers[ requestID ] = answerItem;
//			}
//		}
//	}
//}
