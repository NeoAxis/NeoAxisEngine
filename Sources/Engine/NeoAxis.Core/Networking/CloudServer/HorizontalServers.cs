#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.LiteDB;
using NeoAxis;
using NeoAxis.Networking;
using NeoAxis.Cloud;

namespace NeoAxis.Cloud
{
	/// <summary>
	/// Provides horizontal server functionality.
	/// </summary>
	public static class HorizontalServers
	{
		volatile static EDictionary<string, HorizontalServerItem> servers = new EDictionary<string, HorizontalServerItem>();
		static DateTime updateServersListLastTime;

		///////////////////////////////////////////////

		public class HorizontalServerItem
		{
			public CloudServiceFunctions.ServerGetResult.ServerItem ServerInfo;
			CloudFunctionsClient client;

			/////////////////////

			public class GetClientResult
			{
				public CloudFunctionsClient Client;
				public string Error;
			}

			public async Task<GetClientResult> GetClientAsync( CancellationToken cancellationToken = default )
			{
				var client2 = client;
				if( client2 == null || client2.ConnectionNode.Status == NetworkStatus.Disconnected )
				{
					//destroy old client
					client2?.Destroy();
					client = null;

					//connection settings
					var port = 0;
					if( ServerInfo.Projects.Length == 1 )
						port = ServerInfo.Projects[ 0 ].ProcessPort;
					var password = ServerInfo.ServerCheckCode;

					if( port == 0 )
						return new GetClientResult() { Error = "Server port of the project is not specified." };
					if( string.IsNullOrEmpty( password ) )
						return new GetClientResult() { Error = "Server check code is not specified." };

					//!!!!need AllowReconnect?

					var settings = BasicServiceClient.ConnectionSettingsClass.CreateDirect( CloudUserRole.Developer, ServerInfo.Address, port, password, false );

					//create client and connect
					var createResult = await CloudFunctionsClient.CreateAsync( settings, true, cancellationToken );
					if( !string.IsNullOrEmpty( createResult.Error ) )
						return new GetClientResult() { Error = createResult.Error };

					//now connected
					client2 = createResult.Client;
					client = client2;
				}

				return new GetClientResult() { Client = client2 };
			}

			public CloudFunctionsClient GetClientWithoutCreating()
			{
				return client;
			}

			public void Destroy()
			{
				client?.Destroy();
				client = null;
			}
		}

		///////////////////////////////////////////////

		public class GetOrAllocateServerSettings
		{
			public string? Region;
			//public string? Configuration = null;

			public int MaxCPUUsage = 95;
			public int MaxMemoryUsage = 95;
			public int MaxGPUUsage = 95;
			public int MaxGPUMemoryUsage = 95;
			public int MaxDiskUsage = 95;

			public delegate bool CheckReadyDelegate( HorizontalServerItem server );//, ref bool allow );
			public CheckReadyDelegate CheckReady;

			public bool AllowStartAllocationOfNewServer;
		}

		///////////////////////////////////////////////

		public class GetFreeServerResult
		{
			public HorizontalServerItem Server;
			public bool NoFreeServers;
			public string Error;
		}

		///////////////////////////////////////////////

		public class DeleteServerResult
		{
			public string Error;
		}

		///////////////////////////////////////////////

		public static void Initialize()
		{
		}

		public static HorizontalServerItem[] GetServers( string? region = null )
		{
			if( region != null )
				return servers.Values.Where( s => s.ServerInfo.Region == region ).ToArray();
			else
				return servers.Values.ToArray();
		}

		public static HorizontalServerItem GetServerByAddress( string address )
		{
			servers.TryGetValue( address, out var server );
			return server;
		}

		async static Task UpdateServerListAsync( CancellationToken cancellationToken = default )
		{
			try
			{
				var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
				var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
				//using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );

				var getResult = await CloudServiceFunctions.ServerGetAsync( projectID, null, false, serverCheckCode, cancellationToken: cancellationToken );
				if( !string.IsNullOrEmpty( getResult.Error ) )
					throw new Exception( getResult.Error );
				//var getResult = await GeneralManagerFunctions.HorizontalServerGetAsync( projectID, serverCheckCode, cancellationToken );
				//if( !string.IsNullOrEmpty( getResult.Error ) )
				//	throw new Exception( getResult.Error );

				var newServers = new EDictionary<string, HorizontalServerItem>();

				//add new servers
				foreach( var getResultItem in getResult.Servers )
				{
					if( getResultItem.Horizontal )
					{
						var server = GetServerByAddress( getResultItem.Address );
						if( server == null )
							server = new HorizontalServerItem();
						server.ServerInfo = getResultItem;
						newServers[ server.ServerInfo.Address ] = server;
					}
				}

				//delete old servers
				foreach( var server in servers.Values )
				{
					if( !newServers.ContainsKey( server.ServerInfo.Address ) )
						server.Destroy();
				}

				servers = newServers;
			}
			catch( Exception e )
			{
				ServerLogs.Write( "Horizontal Servers", $"UpdateServerListAsync exception; {e.Message}" );
			}
		}

		public async static Task<GetFreeServerResult> GetFreeServerAsync( GetOrAllocateServerSettings settings, CancellationToken cancellationToken )
		{
			try
			{
				//refresh servers list
				var utcNow = DateTime.UtcNow;
				if( ( utcNow - updateServersListLastTime ).TotalSeconds >= 1 )
				{
					updateServersListLastTime = utcNow;
					await UpdateServerListAsync( cancellationToken );
				}

				//get updated servers list
				var servers = GetServers( region: settings.Region );


				//try to find usable server
				{
					ServerLogs.Write( "Horizontal Servers", $"GetOrAllocateServerAsync; Find suitable server from {servers.Length} available servers." );

					HorizontalServerItem foundServer = null;
					foreach( var server in servers )
					{
						var serverInfo = server.ServerInfo;

						ServerLogs.Write( "Horizontal Servers", $"GetOrAllocateServerAsync; Check suitable server {serverInfo.Address}, Region {serverInfo.Region}, Status {serverInfo.Status}, CPU {serverInfo.CPUUsage}%, Memory {serverInfo.MemoryUsage}%, GPU {serverInfo.GPUUsage}%, GPU Memory {serverInfo.GPUMemoryUsage}%, Disk {serverInfo.DiskUsage}%" );

						if( serverInfo.Status == CloudServiceFunctions.ServerGetResult.ServerItem.StatusEnum.Connected )
						{
							if( settings.Region == null || settings.Region == serverInfo.Region )
							{
								if( serverInfo.CPUUsage <= settings.MaxCPUUsage &&
									serverInfo.MemoryUsage <= settings.MaxMemoryUsage &&
									serverInfo.GPUUsage <= settings.MaxGPUUsage &&
									serverInfo.GPUMemoryUsage <= settings.MaxGPUMemoryUsage &&
									serverInfo.DiskUsage <= settings.MaxDiskUsage )
								{
									if( settings.CheckReady == null || settings.CheckReady( server ) )
									{
										foundServer = server;
										break;
									}
								}
							}
						}
					}

					if( foundServer != null )
						ServerLogs.Write( "Horizontal Servers", $"GetOrAllocateServerAsync; Found server {foundServer.ServerInfo.Address}." );
					else
						ServerLogs.Write( "Horizontal Servers", $"GetOrAllocateServerAsync; Free server not found." );

					//found usable server
					if( foundServer != null )
						return new GetFreeServerResult() { Server = foundServer };
				}


				//no free server found. need a new server. check maybe a new server is already creating
				{
					bool IsServerReady( HorizontalServerItem server )
					{
						var serverInfo = server.ServerInfo;
						if( serverInfo.Status != CloudServiceFunctions.ServerGetResult.ServerItem.StatusEnum.Connected )
							return false;
						if( settings.CheckReady != null && !settings.CheckReady( server ) )
							return false;
						return true;
					}

					var existNotReadyServers = servers.Any( s => !IsServerReady( s ) );

					//var existCreatingServers = servers.FirstOrDefault( s => s.ServerInfo.Status == GeneralManagerFunctions.ServerGetResult.ServerItem.StatusEnum.Creating ) != null;

					//start allocation of a new server
					if( !existNotReadyServers && settings.AllowStartAllocationOfNewServer && servers.Length < CloudServerProcessUtility.CommandLineParameters.ProjectHorizontalServersMaxCount )
					{
						//var task = new Task( async delegate ()
						_ = TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "HorizontalServers: GetFreeServerAsync", async delegate ()
						{
							//!!!!custom configuration

							var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
							var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
							using var cts = new CancellationTokenSource( new TimeSpan( 1, 0, 0 ) );

							var createResult = await CloudServiceFunctions.HorizontalServerNewAsync( projectID, settings.Region, null, null, serverCheckCode, cancellationToken: cts.Token );
							//if( !string.IsNullOrEmpty( createResult.Error ) )
							//	return new GetFreeServerResult() { Error = createResult.Error };
						} );
						//task.Start();
					}

					return new GetFreeServerResult() { NoFreeServers = true };
				}
			}
			catch( Exception e )
			{
				ServerLogs.Write( "Horizontal Servers", $"GetFreeServerAsync exception; {e.Message}" );
				return new GetFreeServerResult() { Error = e.Message };
			}
		}

		public async static Task<DeleteServerResult> DeleteServerAsync( HorizontalServerItem server )
		{
			try
			{
				var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
				var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
				using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 30, 0 ) );

				var deleteResult = await CloudServiceFunctions.HorizontalServerDeleteAsync( projectID, server.ServerInfo.Address, serverCheckCode, cancellationToken.Token );
				if( !string.IsNullOrEmpty( deleteResult.Error ) )
					return new DeleteServerResult() { Error = deleteResult.Error };

				return new DeleteServerResult();
			}
			catch( Exception e )
			{
				ServerLogs.Write( "Horizontal Servers", $"DeleteServerAsync exception; {e.Message}" );
				return new DeleteServerResult() { Error = e.Message };
			}
		}

		public static void Update( DateTime utcNow )
		{
			try
			{

				//!!!!task loop. UpdateServerListAsync called from another places


				//update servers list
				if( ( utcNow - updateServersListLastTime ).TotalSeconds >= 60 )
				{
					updateServersListLastTime = utcNow;

					TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "HorizontalServers: Update", async delegate ()
					{
						using var cts = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );
						await UpdateServerListAsync( cts.Token );
					} );
				}
			}
			catch( Exception e )
			{
				ServerLogs.Write( "Horizontal Servers", $"Update exception; {e.Message}" );
			}
		}
	}
}
#endif