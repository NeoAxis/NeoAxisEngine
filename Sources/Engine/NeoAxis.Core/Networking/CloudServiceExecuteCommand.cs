// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using NeoAxis.Editor;
using System.Threading.Tasks;
using System.Net.Http;

namespace NeoAxis.Networking
{
	public class CloudServiceExecuteCommand
	{
		const int delayBetweenAttemptsMs = 10000;

		static HttpClient httpClient;
		static DateTime lastSuccessHttpRequestTime;

		public string FunctionName = "";
		public bool RequireUserLogin;
		public string ServerCheckCodeOrAccessToken;
		public List<ParameterItem> Parameters = new List<ParameterItem>();
		public RequestMethodEnum RequestMethod;
		public byte[] ContentData;
		public object Tag;
		public int? Timeout;

		public volatile ResultClass Result;

		//ThreadItem currentThread;

		///////////////////////////////////////////////

		public class ParameterItem
		{
			public string Name;
			public string Value;
			public bool EncodeBase64Url;
		}

		///////////////////////////////////////////////

		public enum RequestMethodEnum
		{
			Get,
			Post,
		}

		///////////////////////////////////////////////

		//public delegate void ProcessedDelegate( CloudServiceExecuteCommand command );
		///// <summary>
		///// Called from thread.
		///// </summary>
		//public event ProcessedDelegate Processed;

		///////////////////////////////////////////////

		class ThreadItem
		{
			public Thread thread;
			public bool callProcessedEventFromMainThread;
			public bool needStop;
		}

		///////////////////////////////////////////////

		public class ResultClass
		{
			public TextBlock Data;
			//public string Data = "";
			public string Error = "";
			//public DateTime TimeCreated;
		}

		///////////////////////////////////////////////

		public class ResultDownloadFileClass
		{
			public string Error = "";
			//public DateTime TimeCreated;
		}

		///////////////////////////////////////////////

		//void ThreadFunction( object threadItem2 )
		//{
		//	ThreadItem threadItem = (ThreadItem)threadItem2;

		//	try
		//	{
		//		var url = string.Format( @"{0}/{1}/", CloudServiceFunctions.GetHttpURL(), FunctionName );
		//		var bearerToken = "";

		//		var paramsAdded = false;

		//		if( RequireUserLogin )
		//		{
		//			if( !string.IsNullOrEmpty( CloudClientProcessUtility.LoginForSecureMode ) )
		//			{
		//				//for secure mode use login and verification code from command line
		//				var email64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.LoginForSecureMode );
		//				var projectID64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.ProjectID.ToString() );
		//				var hash64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.VerificationCodeForSecureMode );
		//				bearerToken = $"loginsecuremode.{email64}.{projectID64}.{hash64}";

		//				////for secure mode use login and verification code from command line
		//				//var email64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.LoginForSecureMode );
		//				//var hash64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.VerificationCodeForSecureMode );
		//				//url += $"?user={email64}&hash_code_for_secure_mode={hash64}";
		//				//paramsAdded = true;
		//			}
		//			else
		//			{
		//				if( !LoginUtility.GetCurrentLicense( out string email, out string hash ) )
		//					throw new Exception( "Please login to process." );

		//				var email64 = StringUtility.EncodeToBase64URL( email );
		//				var hash64 = StringUtility.EncodeToBase64URL( hash );
		//				bearerToken = $"login.{email64}.{hash64}";
		//			}
		//		}

		//		foreach( var param in Parameters )
		//		{
		//			if( param.EncodeBase64Url )
		//			{
		//				var param64 = StringUtility.EncodeToBase64URL( param.Value );
		//				url += paramsAdded ? "&" : "?";
		//				url += $"{param.Name}={param64}";
		//				paramsAdded = true;
		//			}
		//			else
		//			{
		//				url += paramsAdded ? "&" : "?";
		//				url += $"{param.Name}={param.Value}";
		//				paramsAdded = true;
		//			}
		//		}

		//		if( string.IsNullOrEmpty( bearerToken ) && !string.IsNullOrEmpty( ServerCheckCodeOrAccessToken ) )
		//			bearerToken = ServerCheckCodeOrAccessToken;


		//		var request = (HttpWebRequest)WebRequest.Create( url );

		//		if( Timeout != null )
		//			request.Timeout = Timeout.Value;
		//		else
		//			request.Timeout = NetworkCommonSettings.CloudServiceExecuteCommandTimeout;

		//		if( !string.IsNullOrEmpty( bearerToken ) )
		//			request.Headers[ "Authorization" ] = "Bearer " + bearerToken;

		//		if( RequestMethod == RequestMethodEnum.Post )
		//		{
		//			var contentData = ContentData ?? new byte[ 0 ];
		//			request.Method = "POST";
		//			request.ContentLength = contentData.Length;
		//			request.ContentType = "application/x-www-form-urlencoded";
		//			var dataStream = request.GetRequestStream();
		//			dataStream.Write( contentData, 0, contentData.Length );
		//			dataStream.Close();
		//		}

		//		string blockString = "";


		//		//!!!!can freeze? need cts, or thread will freezed?


		//		using( var response = (HttpWebResponse)request.GetResponse() )
		//		using( var stream = response.GetResponseStream() )
		//		using( var reader = new StreamReader( stream ) )
		//			blockString = reader.ReadToEnd();

		//		if( threadItem.needStop || EditorAPI.ClosingApplication )
		//			return;

		//		var block = TextBlock.Parse( blockString, out var error );
		//		if( !string.IsNullOrEmpty( error ) )
		//			throw new Exception( "Error of parsing the response data. " + error );

		//		if( threadItem.needStop || EditorAPI.ClosingApplication )
		//			return;

		//		var result = new ResultClass();

		//		var errorInResultData = block.GetAttribute( "Error" );
		//		if( !string.IsNullOrEmpty( errorInResultData ) )
		//			result.Error = errorInResultData;
		//		else
		//			result.Data = block;
		//		//result.TimeCreated = DateTime.Now;

		//		Result = result;

		//		if( threadItem.callProcessedEventFromMainThread )
		//		{
		//			EngineThreading.ExecuteFromMainThreadLater( delegate ()
		//			{
		//				Processed?.Invoke( this );
		//			} );
		//		}
		//		else
		//			Processed?.Invoke( this );
		//	}
		//	catch( Exception e )
		//	{
		//		if( threadItem.needStop || EditorAPI.ClosingApplication )
		//			return;

		//		var result = new ResultClass();
		//		result.Error = e.Message;
		//		//result.TimeCreated = DateTime.Now;

		//		Result = result;

		//		if( threadItem.callProcessedEventFromMainThread )
		//		{
		//			EngineThreading.ExecuteFromMainThreadLater( delegate ()
		//			{
		//				Processed?.Invoke( this );
		//			} );
		//		}
		//		else
		//			Processed?.Invoke( this );
		//	}
		//}

		//public void _InternalBeginExecution( bool callProcessedEventFromMainThread )
		//{
		//	_InternalStopExecution();

		//	var thread = new Thread( ThreadFunction );
		//	thread.IsBackground = true;
		//	var threadItem = new ThreadItem() { thread = thread, callProcessedEventFromMainThread = callProcessedEventFromMainThread };
		//	currentThread = threadItem;

		//	thread.Start( threadItem );
		//}

		//public void _InternalStopExecution()
		//{
		//	var item = currentThread;
		//	if( item != null )
		//		item.needStop = true;
		//	currentThread = null;
		//}

		async Task<string> SendRequestAsync( string url, RequestMethodEnum requestMethod, byte[] contentData, string bearerToken, CancellationToken cancellationToken = default )
		{
			if( httpClient == null )
			{
				httpClient = new HttpClient();
				httpClient.Timeout = TimeSpan.FromMilliseconds( NetworkCommonSettings.CloudServiceExecuteCommandTimeout );
			}

			var useNewClient = Timeout != null;
			var client = useNewClient ? new HttpClient() : httpClient;
			try
			{
				if( useNewClient )
				{
					if( Timeout != null )
						client.Timeout = TimeSpan.FromMilliseconds( Timeout.Value );
				}

				using var request = new HttpRequestMessage( requestMethod == RequestMethodEnum.Post ? HttpMethod.Post : HttpMethod.Get, url );

				if( !string.IsNullOrEmpty( bearerToken ) )
					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", bearerToken );

				if( requestMethod == RequestMethodEnum.Post )
				{
					request.Content = new ByteArrayContent( contentData ?? new byte[ 0 ] );
					request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( "application/x-www-form-urlencoded" );
				}

				using var response = await client.SendAsync( request, cancellationToken );
				response.EnsureSuccessStatusCode();

				lastSuccessHttpRequestTime = DateTime.UtcNow;

#if UWP
				string blockString = await response.Content.ReadAsStringAsync();
#else
				string blockString = await response.Content.ReadAsStringAsync( cancellationToken );
#endif
				return blockString;
			}
			finally
			{
				if( useNewClient )
					client.Dispose();
			}
		}

		public void AddParameter( string name, string value, bool encodeBase64Url )
		{
			var param = new ParameterItem();
			param.Name = name;
			param.Value = value;
			param.EncodeBase64Url = encodeBase64Url;
			Parameters.Add( param );
		}

		public async Task<ResultClass> ExecuteAsync( CancellationToken cancellationToken = default )
		{
			try
			{
				var url = string.Format( @"{0}/{1}/", CloudServiceFunctions.GetHttpURL(), FunctionName );
				var bearerToken = "";

				var paramsAdded = false;

				if( RequireUserLogin )
				{
					if( !string.IsNullOrEmpty( CloudClientProcessUtility.LoginForSecureMode ) )
					{
						//for secure mode use login and verification code from command line
						var email64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.LoginForSecureMode );
						var projectID64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.ProjectID.ToString() );
						var hash64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.VerificationCodeForSecureMode );
						bearerToken = $"loginsecuremode.{email64}.{projectID64}.{hash64}";

						////for secure mode use login and verification code from command line
						//var email64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.LoginForSecureMode );
						//var projectID64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.ProjectID.ToString() );
						//var hash64 = StringUtility.EncodeToBase64URL( CloudClientProcessUtility.VerificationCodeForSecureMode );
						//url += $"?user={email64}&project_for_secure_mode={projectID64}&hash_code_for_secure_mode={hash64}";
						//paramsAdded = true;
					}
					else
					{
						if( !LoginUtility.GetCurrentLicense( out string email, out string hash ) )
							throw new Exception( "Please login to process." );

						var email64 = StringUtility.EncodeToBase64URL( email );
						var hash64 = StringUtility.EncodeToBase64URL( hash );
						bearerToken = $"login.{email64}.{hash64}";
					}
				}

				foreach( var param in Parameters )
				{
					if( param.EncodeBase64Url )
					{
						var param64 = StringUtility.EncodeToBase64URL( param.Value );
						url += paramsAdded ? "&" : "?";
						url += $"{param.Name}={param64}";
						paramsAdded = true;
					}
					else
					{
						url += paramsAdded ? "&" : "?";
						url += $"{param.Name}={param.Value}";
						paramsAdded = true;
					}
				}

				if( string.IsNullOrEmpty( bearerToken ) && !string.IsNullOrEmpty( ServerCheckCodeOrAccessToken ) )
					bearerToken = ServerCheckCodeOrAccessToken;

				//get result or exception

				string blockString = null;
				Exception lastException = null;
				while( true )
				{
					//send request
					try
					{
						blockString = await SendRequestAsync( url, RequestMethod, ContentData, bearerToken, cancellationToken );
						break;
					}
					catch( Exception e )
					{
						lastException = e;
					}

					//wait 10 seconds, or until cancelled, or until a successful request happens in another execution
					var startWaitTime = DateTime.UtcNow;
					var waitSteps = 20;
					for( int n = 0; n < waitSteps; n++ )
					{
						if( cancellationToken.IsCancellationRequested || EditorAPI.ClosingApplication )
							break;
						if( lastSuccessHttpRequestTime > startWaitTime )
							break;
						await Task.Delay( delayBetweenAttemptsMs / waitSteps, cancellationToken );
					}
					//await Task.Delay( delayBetweenAttemptsMs, cancellationToken );

					if( cancellationToken.IsCancellationRequested )
					{
						if( lastException != null )
							throw new Exception( "Operation was cancelled. Last error: " + lastException.Message );
						else
							return new ResultClass() { Error = "Operation was cancelled." };
					}
					if( EditorAPI.ClosingApplication )
						throw new Exception( "Operation was cancelled. Closing application." );
				}

				//parse result
				var block = TextBlock.Parse( blockString, out var error );
				if( !string.IsNullOrEmpty( error ) )
					throw new Exception( "Error of parsing the response data. " + error );

				//return result
				var result = new ResultClass();
				var errorInResultData = block.GetAttribute( "Error" );
				if( !string.IsNullOrEmpty( errorInResultData ) )
					result.Error = errorInResultData;
				else
					result.Data = block;
				//result.TimeCreated = DateTime.Now;
				return result;
			}
			catch( Exception e )
			{
				var result = new ResultClass();
				result.Error = e.Message;
				//result.TimeCreated = DateTime.Now;
				return result;
			}
		}
	}
}