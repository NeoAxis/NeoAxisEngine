// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Networking;
using System.Reflection;

#if !NO_LITE_DB
using Internal.LiteDB;
#endif

namespace NeoAxis
{
#if !NO_LITE_DB
	public class ServerNetworkService_CloudFunctions : ServerService
	{
		MessageType saveStringType;
		MessageType loadStringType;
		MessageType stringAnswerType;

		MessageType callMethodType;
		MessageType callMethodAnswerType;

		string fullPathToDatabase;
		DatabaseImplClass databaseImpl;

		ConcurrentDictionary<string, MethodItem> callMethods = new ConcurrentDictionary<string, MethodItem>();

		///////////////////////////////////////////

		public delegate void SaveStringDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] keys, string[] values, ref bool handled );
		public event SaveStringDelegate SaveString;

		public delegate void LoadStringDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] keys, string[] outputValues, ref bool handled );
		public event LoadStringDelegate LoadString;

		///////////////////////////////////////////

		public class DatabaseImplClass
		{
			ServerNetworkService_CloudFunctions owner;
			LiteDatabase database;
			ILiteCollection<StringItem> stringCollection;

			/////////////////////

			public class StringItem
			{
				public int Id { get; set; }

				public string Key { get; set; }
				public string Value { get; set; }
			}

			/////////////////////

			public DatabaseImplClass( ServerNetworkService_CloudFunctions owner )
			{
				this.owner = owner;
			}

			public ServerNetworkService_CloudFunctions Owner
			{
				get { return owner; }
			}

			public LiteDatabase Database
			{
				get { return database; }
			}

			public bool Init( out string error )
			{
				error = "";

				try
				{
					var folder = Path.GetDirectoryName( owner.FullPathToDatabase );
					Directory.CreateDirectory( folder );

					//var fileName = Path.Combine( App.DataFolder, "Database.litedb" );
					var connection = "direct"; //"shared"
					var connectionString = $"Filename={owner.FullPathToDatabase};Connection={connection};Upgrade=true";

					int attemp = 0;
again:
					try
					{
						database = new LiteDatabase( connectionString );

						stringCollection = database.GetCollection<StringItem>( "Strings" );
						stringCollection.EnsureIndex( "Key", true );
					}
					catch( Exception )
					{
						if( attemp < 3 )
						{
							attemp++;
							Thread.Sleep( 500 );
							goto again;
						}
						else
							throw;
					}
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}

				return true;
			}

			public void SaveString( string key, string text )
			{
				if( text != null )
				{
					var existingItem = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
					if( existingItem != null )
					{
						existingItem.Value = text;
						stringCollection.Update( existingItem );
					}
					else
					{
						var newItem = new StringItem
						{
							Key = key,
							Value = text
						};
						stringCollection.Insert( newItem );
					}
				}
				else
				{
					var existingItem = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
					if( existingItem != null )
						stringCollection.Delete( existingItem.Id );
				}
			}

			public string LoadString( string key )
			{
				var item = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
				return item?.Value;
			}

			public int GetStringCount()
			{
				return stringCollection.Count();
			}

			public void Clear()
			{
				stringCollection.DeleteAll();
			}
		}

		///////////////////////////////////////////

		public class MethodItem
		{
			public string Key;
			public MethodInfo Method;
		}

		///////////////////////////////////////////

		public ServerNetworkService_CloudFunctions( string fullPathToDatabase, out string error )
			: base( "CloudFunctions", 6 )
		{
			this.fullPathToDatabase = fullPathToDatabase;
			error = null;

			saveStringType = RegisterMessageType( "SaveString", 1, ReceiveMessage_SaveStringToServer );
			loadStringType = RegisterMessageType( "LoadString", 2, ReceiveMessage_LoadStringToServer );
			stringAnswerType = RegisterMessageType( "StringAnswer", 3 );

			callMethodType = RegisterMessageType( "CallMethod", 4, ReceiveMessage_CallMethodAnswerToServer );
			callMethodAnswerType = RegisterMessageType( "CallMethodAnswer", 5 );

			if( !string.IsNullOrEmpty( FullPathToDatabase ) )
			{
				databaseImpl = new DatabaseImplClass( this );
				if( !databaseImpl.Init( out var error2 ) )
				{
					error = "Unable to initialize datatabase. " + error2;
					databaseImpl = null;
					return;
				}
			}
		}

		public string FullPathToDatabase
		{
			get { return fullPathToDatabase; }
		}

		public DatabaseImplClass DatabaseImpl
		{
			get { return databaseImpl; }
		}

		public void SendStringAnswer( ServerNode.Client recepient, long requestID, string[] values )
		{
			var writer = BeginMessage( recepient, stringAnswerType );
			writer.WriteVariableUInt64( (ulong)requestID );
			if( values != null )
			{
				writer.WriteVariableInt32( values.Length );
				for( int n = 0; n < values.Length; n++ )
					writer.WriteWithNullSupport( values[ n ] );
			}
			else
				writer.WriteVariableInt32( 0 );
			EndMessage();
		}

		bool ReceiveMessage_SaveStringToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = (long)reader.ReadVariableUInt64();
			var count = reader.ReadVariableInt32();
			var keys = new string[ count ];
			var values = new string[ count ];
			for( int n = 0; n < count; n++ )
			{
				keys[ n ] = reader.ReadString();
				values[ n ] = reader.ReadStringWithNullSupport();
			}
			if( !reader.Complete() )
				return false;

			try
			{
				var handled = false;
				SaveString?.Invoke( this, sender, requestID, keys, values, ref handled );

				//!!!!parallel? or save several at same time

				if( !handled && databaseImpl != null )
				{
					for( int n = 0; n < count; n++ )
						databaseImpl.SaveString( keys[ n ], values[ n ] );
				}

				SendStringAnswer( sender, requestID, null );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		bool ReceiveMessage_LoadStringToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = (long)reader.ReadVariableUInt64();

			//!!!!check max count, max string length. where else

			var count = reader.ReadVariableInt32();
			var keys = new string[ count ];
			for( int n = 0; n < count; n++ )
				keys[ n ] = reader.ReadString();
			if( !reader.Complete() )
				return false;

			try
			{
				var values = new string[ count ];

				var handled = false;
				LoadString?.Invoke( this, sender, requestID, keys, values, ref handled );

				//!!!!parallel? or load several at same time

				if( !handled && databaseImpl != null )
				{
					for( int n = 0; n < count; n++ )
						values[ n ] = databaseImpl.LoadString( keys[ n ] );
				}

				SendStringAnswer( sender, requestID, values );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}


		public void RegisterCallMethods( Type type )
		{
			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;// BindingFlags.Instance;

			foreach( var method in type.GetMethods( bindingFlags ) )
			{
				var attrib = method.GetCustomAttribute<CloudMethodAttribute>();
				if( attrib != null )
				{
					var item = new MethodItem();
					item.Key = $"{type.Name}.{method.Name}";
					item.Method = method;

					callMethods[ item.Key ] = item;
				}
			}
		}

		public int CallMethodCount
		{
			get { return callMethods.Count; }
		}

		bool CallMethod( string className, string methodName, string[] parameters, out string resultValue, out string error )
		{
			resultValue = null;
			error = null;

			//!!!!arrays, tuples support

			//!!!!slowly. don't transfer class, method names

			try
			{
				var key = $"{className}.{methodName}";

				if( !callMethods.TryGetValue( key, out var item ) )
				{
					error = $"Method \"{className}.{methodName}\" is not registered.";
					return false;
				}

				var inputParams = new List<ParameterInfo>();
				foreach( var p in item.Method.GetParameters() )
				{
					if( !p.IsRetval )
						inputParams.Add( p );
				}

				var newParameters = new object[ parameters.Length ];
				for( int nParam = 0; nParam < inputParams.Count; nParam++ )
				{
					var demandedType = inputParams[ nParam ].ParameterType;
					var value = parameters[ nParam ];

					if( value != null )
					{
						if( demandedType == value.GetType() )
							newParameters[ nParam ] = value;
						else if( demandedType.IsAssignableFrom( value.GetType() ) )
							newParameters[ nParam ] = Convert.ChangeType( value, demandedType );
						else
						{
							newParameters[ nParam ] = SimpleTypes.ParseValue( demandedType, value );
							//newParameters[ nParam ] = MetadataManager.AutoConvertValue( value, demandedType );
						}
					}
				}

				var methodValue = item.Method.Invoke( null, newParameters ); // = ObjectEx.MethodInvoke( null, null, item.Method, newParameters );
				resultValue = methodValue != null ? methodValue.ToString() : null;

				return true;
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
		}

		public void SendCallMethodAnswer( ServerNode.Client recepient, long requestID, object value, string error )
		{
			var writer = BeginMessage( recepient, callMethodAnswerType );
			writer.WriteVariableUInt64( (ulong)requestID );
			writer.WriteWithNullSupport( value != null ? value.ToString() : null );
			writer.Write( error );
			EndMessage();
		}

		bool ReceiveMessage_CallMethodAnswerToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = (long)reader.ReadVariableUInt64();
			var className = reader.ReadString();
			var methodName = reader.ReadString();
			var parameterCount = reader.ReadVariableInt32();
			var parameters = new string[ parameterCount ];
			for( int n = 0; n < parameterCount; n++ )
				parameters[ n ] = reader.ReadStringWithNullSupport();
			if( !reader.Complete() )
				return false;

			try
			{
				CallMethod( className, methodName, parameters, out var resultValue, out var error2 );
				SendCallMethodAnswer( sender, requestID, resultValue, error2 );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}
	}
#endif

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_CloudFunctions : ClientService
	{
		MessageType saveStringType;
		MessageType loadStringType;
		MessageType stringAnswerType;

		MessageType callMethodType;
		MessageType callMethodAnswerType;

		long requestIdCounter;

		ConcurrentDictionary<long, StringAnswerItem> stringAnswers = new ConcurrentDictionary<long, StringAnswerItem>();
		DateTime stringAnswersLastOldRemoveTime;

		ConcurrentDictionary<long, CallMethodAnswerItem> callMethodAnswers = new ConcurrentDictionary<long, CallMethodAnswerItem>();
		DateTime callMethodAnswersLastOldRemoveTime;

		string connectionErrorReceived;

		///////////////////////////////////////////////

		public delegate void StringAnswerDelegate( ClientNetworkService_CloudFunctions sender, long requestID, string[] values, ref bool handled );
		public event StringAnswerDelegate StringAnswer;

		public delegate void CallMethodAnswerDelegate( ClientNetworkService_CloudFunctions sender, long requestID, string resultValue, ref bool handled );
		public event CallMethodAnswerDelegate CallMethodAnswer;

		///////////////////////////////////////////////

		class StringAnswerItem
		{
			public string[] Values;
			//!!!!
			//public string Error;
			public DateTime CreationTime;
		}

		///////////////////////////////////////////////

		class CallMethodAnswerItem
		{
			public string Value;
			public string Error;
			public DateTime CreationTime;
		}

		///////////////////////////////////////////////

		public class SaveStringResult
		{
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class LoadStringResult
		{
			public string[] Values { get; set; }
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class CallMethodResult<T>
		{
			public T Value { get; set; }
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public ClientNetworkService_CloudFunctions()
			: base( "CloudFunctions", 6 )
		{
			saveStringType = RegisterMessageType( "SaveString", 1 );
			loadStringType = RegisterMessageType( "LoadString", 2 );
			stringAnswerType = RegisterMessageType( "StringAnswer", 3, ReceiveMessage_StringAnswerToClient );

			callMethodType = RegisterMessageType( "CallMethod", 4 );
			callMethodAnswerType = RegisterMessageType( "CallMethodAnswer", 5, ReceiveMessage_CallMethodAnswerToClient );
		}

		bool ReceiveMessage_StringAnswerToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = (long)reader.ReadVariableUInt64();
			var count = reader.ReadVariableInt32();
			string[] values = null;
			if( count > 0 )
			{
				values = new string[ count ];
				for( int n = 0; n < count; n++ )
					values[ n ] = reader.ReadStringWithNullSupport();
			}
			if( !reader.Complete() )
				return false;

			try
			{
				var handled = false;
				StringAnswer?.Invoke( this, requestID, values, ref handled );

				if( !handled )
				{
					//add answer
					var answerItem = new StringAnswerItem();
					answerItem.Values = values;
					answerItem.CreationTime = DateTime.UtcNow;
					stringAnswers[ requestID ] = answerItem;
				}
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		bool ReceiveMessage_CallMethodAnswerToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = (long)reader.ReadVariableUInt64();
			var resultValue = reader.ReadStringWithNullSupport();
			var error = reader.ReadString();
			if( !reader.Complete() )
				return false;

			try
			{
				var handled = false;
				CallMethodAnswer?.Invoke( this, requestID, resultValue, ref handled );

				if( !handled )
				{
					//add answer
					var answerItem = new CallMethodAnswerItem();
					answerItem.Value = resultValue;
					answerItem.Error = error;
					answerItem.CreationTime = DateTime.UtcNow;
					callMethodAnswers[ requestID ] = answerItem;
				}
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		long GetRequestID()
		{
			return Interlocked.Increment( ref requestIdCounter );
		}

		public string ConnectionErrorReceived
		{
			get { return connectionErrorReceived; }
			set { connectionErrorReceived = value; }
		}

		StringAnswerItem GetStringAnswerAndRemove( long requestID )
		{
			if( stringAnswers.Remove( requestID, out var item ) )
				return item;
			return null;
		}

		CallMethodAnswerItem GetCallMethodAnswerAndRemove( long requestID )
		{
			if( callMethodAnswers.Remove( requestID, out var item ) )
				return item;
			return null;
		}

		public void SendSaveString( long requestID, string[] keys, string[] values )
		{
			var writer = BeginMessage( saveStringType );
			writer.WriteVariableUInt64( (ulong)requestID );
			writer.WriteVariableInt32( keys.Length );
			for( int n = 0; n < keys.Length; n++ )
			{
				writer.Write( keys[ n ] );
				writer.WriteWithNullSupport( values != null ? values[ n ] : null );
			}
			EndMessage();
		}

		public void SendLoadString( long requestID, string[] keys )
		{
			var writer = BeginMessage( loadStringType );
			writer.WriteVariableUInt64( (ulong)requestID );
			writer.WriteVariableInt32( keys.Length );
			for( int n = 0; n < keys.Length; n++ )
				writer.Write( keys[ n ] );
			EndMessage();
		}

		public async Task<SaveStringResult> SaveStringsAsync( string[] keys, string[] values, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendSaveString( requestID, keys, values );

			while( true )
			{
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new SaveStringResult() { Error = ConnectionErrorReceived };

				var answer = GetStringAnswerAndRemove( requestID );
				if( answer != null )
					return new SaveStringResult();

				await Task.Delay( 1 );//, cancellationToken );
				if( cancellationToken.IsCancellationRequested )
					return new SaveStringResult { Error = "Operation was canceled." };
			}
		}

		//public async Task<SaveStringResult> SaveStringAsync( string[] keys, string[] values )
		//{
		//	Error

		//	var requestID = GetRequestID();
		//	SendSaveString( requestID, keys, values );

		//	exit from loop? wher else. timeout with Error

		//	while( true )
		//	{
		//		var answer = GetStringAnswerAndRemove( requestID );
		//		if( answer != null )
		//			return new SaveStringResult();

		//		await Task.Delay( 1 );
		//	}
		//}

		public async Task<LoadStringResult> LoadStringsAsync( string[] keys, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendLoadString( requestID, keys );

			while( true )
			{
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new LoadStringResult() { Error = ConnectionErrorReceived };

				var answer = GetStringAnswerAndRemove( requestID );
				if( answer != null )
					return new LoadStringResult { Values = answer.Values };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
					return new LoadStringResult { Error = "Operation was canceled." };
			}
		}

		//public async Task<LoadStringResult> LoadStringAsync( string[] keys )
		//{
		//	var requestID = GetRequestID();
		//	SendLoadString( requestID, keys );

		//	while( true )
		//	{
		//		var answer = GetStringAnswerAndRemove( requestID );
		//		if( answer != null )
		//			return new LoadStringResult { Values = answer.Values };

		//		await Task.Delay( 1 );
		//	}
		//}

		void RemoveOldNotUsedStringAnswers( DateTime now )
		{
			foreach( var pair in stringAnswers.ToArray() )
			{
				var requestID = pair.Key;
				var item = pair.Value;

				if( ( now - item.CreationTime ).TotalMinutes > 10 )
					stringAnswers.Remove( requestID, out _ );
			}
		}

		void RemoveOldNotUsedCallMethodAnswers( DateTime now )
		{
			foreach( var pair in callMethodAnswers.ToArray() )
			{
				var requestID = pair.Key;
				var item = pair.Value;

				if( ( now - item.CreationTime ).TotalMinutes > 10 )
					callMethodAnswers.Remove( requestID, out _ );
			}
		}

		protected internal override void OnUpdate()
		{
			base.OnUpdate();

			var now = DateTime.UtcNow;
			if( ( now - stringAnswersLastOldRemoveTime ).TotalSeconds > 30 )
			{
				RemoveOldNotUsedStringAnswers( now );
				stringAnswersLastOldRemoveTime = now;
			}
			if( ( now - callMethodAnswersLastOldRemoveTime ).TotalSeconds > 30 )
			{
				RemoveOldNotUsedCallMethodAnswers( now );
				callMethodAnswersLastOldRemoveTime = now;
			}
		}

		public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, object[] parameters, CancellationToken cancellationToken = default )
		{

			//!!!!error when not supported

			//!!!!optimize
			//cache names. transfer IDs

			//!!!!more parameter types. arrays, tuples


			var parameters2 = parameters;
			if( parameters2 == null )
				parameters2 = Array.Empty<object>();

			var requestID = GetRequestID();

			var writer = BeginMessage( callMethodType );
			writer.WriteVariableUInt64( (ulong)requestID );
			writer.Write( className );
			writer.Write( methodName );
			writer.WriteVariableInt32( parameters2.Length );
			for( int n = 0; n < parameters2.Length; n++ )
			{
				var p = parameters2[ n ];
				writer.WriteWithNullSupport( p != null ? p.ToString() : null );
			}
			EndMessage();


			while( true )
			{
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new CallMethodResult<T>() { Error = ConnectionErrorReceived };

				var answer = GetCallMethodAnswerAndRemove( requestID );
				if( answer != null )
				{
					if( string.IsNullOrEmpty( answer.Error ) )
					{
						var value = answer.Value;
						if( value != null )
						{
							if( SimpleTypes.TryParseValue<T>( value, out var resultValue, out var error ) )
								return new CallMethodResult<T> { Value = resultValue };
							else
								return new CallMethodResult<T> { Error = error };
						}
						else
							return default;
					}
					else
						return new CallMethodResult<T> { Error = answer.Error };
				}

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
					return new CallMethodResult<T> { Error = "Operation was canceled." };
			}
		}

		//public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, object[] parameters )
		//{

		//	//!!!!error when not supported

		//	//!!!!optimize


		//	if( parameters == null )
		//		parameters = Array.Empty<object>();

		//	var requestID = GetRequestID();

		//	var writer = BeginMessage( callMethodType );
		//	writer.WriteVariableUInt64( (ulong)requestID );
		//	writer.Write( className );
		//	writer.Write( methodName );
		//	writer.WriteVariableInt32( parameters.Length );
		//	for( int n = 0; n < parameters.Length; n++ )
		//	{
		//		var p = parameters[ n ];
		//		if( p != null )
		//			writer.Write( p.ToString() );
		//		else
		//			writer.Write( "{[__NULL__]}" );
		//	}
		//	EndMessage();


		//	while( true )
		//	{
		//		var answer = GetCallMethodAnswerAndRemove( requestID );
		//		if( answer != null )
		//		{
		//			if( string.IsNullOrEmpty( answer.Error ) )
		//			{
		//				var value = answer.ResultValue;
		//				if( value != null )
		//				{
		//					if( SimpleTypes.TryParseValue<T>( value, out var resultValue, out var error ) )
		//						return new CallMethodResult<T> { ResultValue = resultValue };
		//					else
		//						return new CallMethodResult<T> { Error = error };
		//				}
		//				else
		//					return default;
		//			}
		//			else
		//				return new CallMethodResult<T> { Error = answer.Error };
		//		}

		//		await Task.Delay( 1 );
		//	}
		//}
	}
}