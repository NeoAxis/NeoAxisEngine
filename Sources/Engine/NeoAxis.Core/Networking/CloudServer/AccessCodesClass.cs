#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using NeoAxis.LiteDB;
using NeoAxis;
using NeoAxis.Networking;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// Access codes for the cloud functions.
	/// </summary>
	public static class AccessCodesClass
	{
		static bool initialized;
		static ILiteCollection<AccessCodeItem> accessCodesCollection;

		///////////////////////////////////////////////

		public class AccessCodeItem
		{
			public string Id { get; set; } //Code
			public long UserID { get; set; }
		}

		///////////////////////////////////////////////

		public static void Initialize()
		{
			if( !CloudFunctionsServer.AccessCodes )
				return;

			var databaseImpl = CloudFunctionsServer.ServerNode?.CloudFunctions?.DatabaseImpl;
			if( databaseImpl == null )
			{
				ServerLogs.Write( "Cloud Functions", "AccessCodes: Initialize: Database is not initialized." );
				Console.WriteLine( "AccessCodes: Initialize: Database is not initialized." );
				return;
			}

			accessCodesCollection = databaseImpl.Database.GetCollection<AccessCodeItem>( "AccessCodes" );
			accessCodesCollection.EnsureIndex( x => x.UserID, true );

			//register call methods
			CloudFunctionsServer.ServerNode.CloudFunctions.RegisterCloudMethods( typeof( AccessCodesClass ), out var error );
			if( !string.IsNullOrEmpty( error ) )
			{
				ServerLogs.Write( "Cloud Functions", "AccessCodes: Initialize: RegisterCallMethods error. " + error );
				Console.WriteLine( "AccessCodes: Initialize: RegisterCallMethods error. " + error );
			}

			initialized = true;
		}

		public static bool Enabled
		{
			get { return initialized; }
		}

		public static long GetUserIDByAccessCode( string code )
		{
			if( !initialized )
				throw new InvalidOperationException( "Access code service is not initialized." );

			var item = accessCodesCollection.FindOne( Query.EQ( "_id", code ) );
			if( item == null )
				return 0;
			return item.UserID;
		}

		/// <summary>
		/// Deletes all access codes.
		/// </summary>
		[CloudMethod( CloudUserRole.Developer, AddToCommands = true )]
		[Description( "Delete all access codes." )]
		public static void AccessCodeDeleteAll()
		{
			var database = CloudFunctionsServer.ServerNode.CloudFunctions.DatabaseImpl;
			if( database == null )
				throw new Exception( "Database is not initialized." );

			accessCodesCollection.DeleteAll();
		}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static string AccessCodeGetCurrent( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Access code service is not initialized." );

			//get user ID
			var userID = context.Client.LoginDataUserID;
			if( userID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//get current
			var item = accessCodesCollection.FindOne( Query.EQ( "UserID", userID ) );
			return item != null ? item.Id : "";
		}

		[CloudMethod( MaxCallPerClientPermit = 10 )]
		public static string AccessCodeGetNew( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Access code service is not initialized." );

			//get user ID
			var userID = context.Client.LoginDataUserID;
			if( userID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//delete old
			var item = accessCodesCollection.FindOne( Query.EQ( "UserID", userID ) );
			if( item != null )
				accessCodesCollection.Delete( item.Id );

			//generate new
			while( true )
			{
				var bytes = new byte[ 16 ];
				using( var rng = RandomNumberGenerator.Create() )
					rng.GetBytes( bytes );
				var code = BitConverter.ToString( bytes ).Replace( "-", "" ).ToLower();

				if( accessCodesCollection.FindById( code ) == null )
				{
					accessCodesCollection.Insert( new AccessCodeItem() { Id = code, UserID = userID } );
					return code;
				}
			}
		}

		[CloudMethod( MaxCallPerClientPermit = 10 )]
		public static void AccessCodeDelete( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Access code service is not initialized." );

			//get user ID
			var userID = context.Client.LoginDataUserID;
			if( userID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//delete
			var item = accessCodesCollection.FindOne( Query.EQ( "UserID", userID ) );
			if( item != null )
				accessCodesCollection.Delete( item.Id );
		}
	}
}
#endif