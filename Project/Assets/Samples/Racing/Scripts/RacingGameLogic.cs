// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// A component that implements the logic of the racing game.
	/// </summary>
	public class RacingGameLogic : GameLogic
	{
		//constants
		Vector2 vehicleSizeToCalculateStartPositions = new Vector2( 5.7, 2.3 );

		//checkpoints of the race
		CheckpointItem[] checkpoints = Array.Empty<CheckpointItem>();

		//vehicles of the players
		VehicleItem[] vehicles = Array.Empty<VehicleItem>();

		//checkpoints passed by players
		int[] checkpointsPassed = Array.Empty<int>();
		float[] checkpointsPassedTime = Array.Empty<float>(); //index = playerIndex * checkpointsInfo.Count + checkpointIndex


		///////////////////////////////////////////////
		//settings

		[DefaultValue( 3 )]
		public Reference<int> Laps
		{
			get { if( _laps.BeginGet() ) Laps = _laps.Get( this ); return _laps.value; }
			set { if( _laps.BeginSet( this, ref value ) ) { try { LapsChanged?.Invoke( this ); } finally { _laps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Laps"/> property value changes.</summary>
		public event Action<RacingGameLogic> LapsChanged;
		ReferenceField<int> _laps = 3;

		[DefaultValue( 10.0 )]
		public Reference<double> PreRaceTime
		{
			get { if( _preRaceTime.BeginGet() ) PreRaceTime = _preRaceTime.Get( this ); return _preRaceTime.value; }
			set { if( _preRaceTime.BeginSet( this, ref value ) ) { try { PreRaceTimeChanged?.Invoke( this ); } finally { _preRaceTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PreRaceTime"/> property value changes.</summary>
		public event Action<RacingGameLogic> PreRaceTimeChanged;
		ReferenceField<double> _preRaceTime = 10.0;

		//RaceTimeLimit


		[DefaultValue( true )]
		public Reference<bool> DrawCheckpointsEditor
		{
			get { if( _drawCheckpointsEditor.BeginGet() ) DrawCheckpointsEditor = _drawCheckpointsEditor.Get( this ); return _drawCheckpointsEditor.value; }
			set { if( _drawCheckpointsEditor.BeginSet( this, ref value ) ) { try { DrawCheckpointsEditorChanged?.Invoke( this ); } finally { _drawCheckpointsEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DrawCheckpointsEditor"/> property value changes.</summary>
		public event Action<RacingGameLogic> DrawCheckpointsEditorChanged;
		ReferenceField<bool> _drawCheckpointsEditor = true;

		[DefaultValue( false )]
		public Reference<bool> DrawCheckpointsSimulation
		{
			get { if( _drawCheckpointsSimulation.BeginGet() ) DrawCheckpointsSimulation = _drawCheckpointsSimulation.Get( this ); return _drawCheckpointsSimulation.value; }
			set { if( _drawCheckpointsSimulation.BeginSet( this, ref value ) ) { try { DrawCheckpointsSimulationChanged?.Invoke( this ); } finally { _drawCheckpointsSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DrawCheckpointsSimulation"/> property value changes.</summary>
		public event Action<RacingGameLogic> DrawCheckpointsSimulationChanged;
		ReferenceField<bool> _drawCheckpointsSimulation = false;

		///////////////////////////////////////////////
		//playing state

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( true )]
		[DefaultValue( 0.0f )]
		public Reference<float> CurrentGameTime
		{
			get { if( _currentGameTime.BeginGet() ) CurrentGameTime = _currentGameTime.Get( this ); return _currentGameTime.value; }
			set { if( _currentGameTime.BeginSet( this, ref value ) ) { try { CurrentGameTimeChanged?.Invoke( this ); } finally { _currentGameTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentGameTime"/> property value changes.</summary>
		public event Action<RacingGameLogic> CurrentGameTimeChanged;
		ReferenceField<float> _currentGameTime = 0.0f;

		///////////////////////////////////////////////

		public class CheckpointItem
		{
			public Vector3 Position1;
			public Vector3 Position2;
			public double Height;
			public List<MeshInSpace> MeshObjects = new List<MeshInSpace>();
		}

		///////////////////////////////////////////////

		public class VehicleItem
		{
			public Vehicle Vehicle;
			public long UserID;
		}

		///////////////////////////////////////////////

		public enum GameStatusEnum
		{
			Prepare,
			Play,
			Finished,
		}

		///////////////////////////////////////////////

		public class StatusInfo
		{
			public float PreRaceTime;
			public float PreRaceTimeRemaining;
			public float RaceTime;
			public GameStatusEnum GameStatus;
		}


		///////////////////////////////////////////////
		// Common

		[Browsable( false )]
		public CheckpointItem[] Checkpoints
		{
			get { return checkpoints; }
		}

		[Browsable( false )]
		public int[] CheckpointsPassed
		{
			get { return checkpointsPassed; }
		}

		[Browsable( false )]
		public float[] CheckpointsPassedTime
		{
			get { return checkpointsPassedTime; }
		}

		public delegate void CheckpointsPassedTimeChangedDelegate( RacingGameLogic sender );
		public event CheckpointsPassedTimeChangedDelegate CheckpointsPassedTimeChanged;

		public StatusInfo GetStatus()
		{
			var result = new StatusInfo();

			var currentGameTime = CurrentGameTime.Value;
			if( currentGameTime < PreRaceTime )
			{
				result.PreRaceTime = currentGameTime;
				result.PreRaceTimeRemaining = (float)PreRaceTime.Value - currentGameTime;
				result.GameStatus = GameStatusEnum.Prepare;
			}
			else
			{
				result.RaceTime = Math.Max( currentGameTime - (float)PreRaceTime, 0.0001f );
				if( IsMatchOver() )
					result.GameStatus = GameStatusEnum.Finished;
				else
					result.GameStatus = GameStatusEnum.Play;
			}

			return result;
		}

		/// <summary>
		/// Determines whether the match has been completed based on the number of laps and checkpoints passed.
		/// </summary>
		public bool IsMatchOver()
		{
			if( checkpoints.Length == 0 )
				return false;

			var totalCheckpointsToPass = checkpoints.Length * Laps;
			return checkpointsPassed.All( cp => cp >= totalCheckpointsToPass );
		}

		public (int, float)[] GetEndedGamePlayerIndexesWithTime()
		{
			var result = new List<(int, float)>();

			var totalCheckpointsToPass = Laps * Checkpoints.Length;

			for( int playerIndex = 0; playerIndex < vehicles.Length; playerIndex++ )
			{
				if( playerIndex < CheckpointsPassed.Length && CheckpointsPassed[ playerIndex ] >= totalCheckpointsToPass )
					result.Add( (playerIndex, CheckpointsPassedTime.LastOrDefault()) );
			}

			CollectionUtility.SelectionSort( result, delegate ( (int, float) a, (int, float) b )
			{
				return a.Item2.CompareTo( b.Item2 );
			} );

			return result.ToArray();
		}

		public void UpdateCheckpoints()
		{
			var list = new List<CheckpointItem>();

			var scene = ParentScene;
			if( scene != null )
			{
				for( int counter = 1; ; counter++ )
				{
					var point1 = scene.GetComponent<ObjectInSpace>( $"Checkpoint {counter} 1", onlyEnabledInHierarchy: true );
					var point2 = scene.GetComponent<ObjectInSpace>( $"Checkpoint {counter} 2", onlyEnabledInHierarchy: true );
					var crossbarBottom = scene.GetComponent<ObjectInSpace>( $"Checkpoint {counter} 3", onlyEnabledInHierarchy: true );
					var crossbarTop = scene.GetComponent<ObjectInSpace>( $"Checkpoint {counter} 4", onlyEnabledInHierarchy: true );

					if( point1 == null || point2 == null )
						break;

					var spaceBounds1 = point1.SpaceBounds.BoundingBox;
					var spaceBounds2 = point2.SpaceBounds.BoundingBox;
					var heightMin = Math.Min( spaceBounds1.Minimum.Z, spaceBounds2.Minimum.Z );
					var heightMax = Math.Max( spaceBounds1.Maximum.Z, spaceBounds2.Maximum.Z );

					var checkpoint = new CheckpointItem();
					checkpoint.Position1 = new Vector3( point1.TransformV.Position.ToVector2(), heightMin );
					checkpoint.Position2 = new Vector3( point2.TransformV.Position.ToVector2(), heightMin );
					checkpoint.Height = heightMax - heightMin;

					//add to MeshObjects
					var objects = new ObjectInSpace[] { point1, point2, crossbarBottom, crossbarTop };
					foreach( var obj in objects )
					{
						var meshInSpace = obj as MeshInSpace;
						if( meshInSpace != null )
							checkpoint.MeshObjects.Add( meshInSpace );
					}

					list.Add( checkpoint );
				}
			}

			checkpoints = list.ToArray();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.RenderEvent += Scene_RenderEvent;
				else
					scene.RenderEvent -= Scene_RenderEvent;
			}
		}

		void DrawCheckpoints( Scene scene, Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;

			foreach( var checkpoint in Checkpoints )
			{
				renderer.SetColor( new ColorValue( 1, 1, 0 ) );
				renderer.AddLine( checkpoint.Position1, checkpoint.Position2 );
				renderer.AddLine( checkpoint.Position2, checkpoint.Position2 + new Vector3( 0, 0, checkpoint.Height ) );
				renderer.AddLine( checkpoint.Position2 + new Vector3( 0, 0, checkpoint.Height ), checkpoint.Position1 + new Vector3( 0, 0, checkpoint.Height ) );
				renderer.AddLine( checkpoint.Position1 + new Vector3( 0, 0, checkpoint.Height ), checkpoint.Position1 );
			}
		}

		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			//draw checkpoints
			if( EngineApp.IsEditor ? DrawCheckpointsEditor : DrawCheckpointsSimulation )
			{
				UpdateCheckpoints();
				DrawCheckpoints( sender, viewport );
			}
		}


		///////////////////////////////////////////////
		// Server, Single

		double sendPlayersInfoToClientsRemainingTime;
		bool firstSimulationStep = true;

		//cloud mode specific
		[Browsable( false )]
		public long[] CloudPlayers { get; set; }

		///////////////////////////////////////////////

		public class RacingServerUserItem : ServerUserItem
		{
			//public int Frags;
			//public int Team;
		}

		///////////////////////////////////////////////

		protected override bool ServerOrSingle_IsAllowToUpdateObjectControlledByPlayers()
		{
			//disable recreation of the object controlled by the player. The vehicle is created at the start of the race and is not recreated during the game.
			return false;
		}

		protected override void ServerOrSingle_OnEnabledInHierarchyChanged()
		{
			base.ServerOrSingle_OnEnabledInHierarchyChanged();
		}

		protected override ServerUserItem Server_OnNewUserItem()
		{
			return new RacingServerUserItem();
		}

		protected override void Server_AddUser( ServerNetworkService_Users.UserInfo user )
		{
			base.Server_AddUser( user );
		}

		public new RacingServerUserItem Server_GetUser( ServerNetworkService_Users.UserInfo user )
		{
			return (RacingServerUserItem)base.Server_GetUser( user );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			var status = GetStatus();

			//!!!!optimization. //send only when changed. also send to new clients. maybe send changes only
			//send players info to clients
			sendPlayersInfoToClientsRemainingTime -= Time.SimulationDelta;
			if( sendPlayersInfoToClientsRemainingTime < 0 )
			{
				sendPlayersInfoToClientsRemainingTime = 1;
				Server_SendUsersInfoToClients();
			}

			if( firstSimulationStep )
				UpdateCheckpoints();

			//update game time
			CurrentGameTime += Time.SimulationDelta;
			if( vehicles.Length == 0 )
				CurrentGameTime = 0;

			//create vehicles when the game is in the Prepare state and the number of vehicles does not match the number of players
			if( status.GameStatus == GameStatusEnum.Prepare )
			{
				var serverUsers = NetworkIsServer ? Server_GetUsers() : null;
				//cloud specific: additional settings
				if( serverUsers != null && CloudPlayers != null )
				{
					//remove spectators from serverUsers
					serverUsers = serverUsers.Where( u => Array.IndexOf( CloudPlayers, u.User.UserID ) != -1 ).ToArray();
					//sort users by CloudPlayers order
					serverUsers = serverUsers.OrderBy( u => Array.IndexOf( CloudPlayers, u.User.UserID ) ).ToArray();
				}

				var singleUsers = NetworkIsSingle ? Single_GetUsers() : null;

				var playerCount = 0;
				if( serverUsers != null )
					playerCount += serverUsers.Length;
				if( singleUsers != null )
					playerCount += singleUsers.Length;

				if( vehicles.Length != playerCount )
				{
					CreateVehicles( serverUsers, singleUsers, playerCount );

					checkpointsPassed = new int[ playerCount ];
					checkpointsPassedTime = new float[ playerCount * Checkpoints.Length * Laps ];

					//send to all clients about the new player count
					{
						var m = BeginNetworkMessageToEveryone( "ChangePlayerCount" );
						if( m != null )
						{
							m.Writer.WriteVariableInt32( playerCount );
							m.End();
						}
					}
				}
			}

			//update checkpoints passed states and send to all clients
			try
			{
				//updates checkpoint passed states and send to all clients
				if( Laps != 0 && checkpoints.Length != 0 )
				{
					var totalCheckpointsToPass = checkpoints.Length * Laps;

					for( int nVehicle = 0; nVehicle < vehicles.Length; nVehicle++ )
					{
						var vehicleItem = vehicles[ nVehicle ];
						var vehicle = vehicleItem.Vehicle;
						if( vehicle == null )
							continue;

						if( checkpointsPassed[ nVehicle ] < totalCheckpointsToPass )
						{
							var checkpointIndexToCheck = ( checkpointsPassed[ nVehicle ] + 1 ) % checkpoints.Length;
							var checkpointInfo = checkpoints[ checkpointIndexToCheck ];

							//!!!!
							//not exact check. need check oriented box and 3D

							var vehicleBounds = vehicle.SpaceBounds.BoundingBox.ToRectangle();

							var checkpointRay = new Ray2( checkpointInfo.Position1.ToVector2(), checkpointInfo.Position2.ToVector2() - checkpointInfo.Position1.ToVector2() );
							//var checkpointRay = new Ray2( checkpointInfo.Position1, checkpointInfo.Position2 - checkpointInfo.Position1 );

							var passedNow = vehicleBounds.Intersects( checkpointRay );
							if( passedNow )
							{
								var previousCheckpointPassedIndex = checkpointsPassed[ nVehicle ];
								checkpointsPassed[ nVehicle ]++;

								//send to all clients about the new checkpoint passed count
								{
									var m = BeginNetworkMessageToEveryone( "CheckpointsPassed" );
									if( m != null )
									{
										m.Writer.WriteVariableInt32( nVehicle );
										m.Writer.WriteVariableInt32( checkpointsPassed[ nVehicle ] );
										m.End();
									}
								}

								var timeIndex = nVehicle * totalCheckpointsToPass + previousCheckpointPassedIndex;
								if( timeIndex < checkpointsPassedTime.Length )
								{
									checkpointsPassedTime[ timeIndex ] = status.RaceTime;

									CheckpointsPassedTimeChanged?.Invoke( this );

									//send to all clients about the new checkpoint passed time
									{
										var m = BeginNetworkMessageToEveryone( "CheckpointsPassedTime" );
										if( m != null )
										{
											m.Writer.WriteVariableInt32( timeIndex );
											m.Writer.Write( checkpointsPassedTime[ timeIndex ] );
											m.End();
										}
									}
								}
							}
						}
					}
				}
			}
			catch( Exception e )
			{
				Log.Warning( "RacingGameLogic: OnSimulationStep: " + e.Message );
			}

			//manage allowing of driving vehicles depends on the game status
			foreach( var vehicleItem in vehicles )
			{
				var vehicle = vehicleItem.Vehicle;

				var inputProcessing = vehicle.GetComponent<InputProcessing>();
				if( inputProcessing != null )
				{
					inputProcessing.AllowInput = status.GameStatus == GameStatusEnum.Play;
					if( status.GameStatus != GameStatusEnum.Play )
					{
						vehicle.Throttle = 0;
						vehicle.Steering = 0;
						vehicle.Brake = 1;
						vehicle.HandBrake = 1;
					}
				}
			}

			firstSimulationStep = false;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			switch( message )
			{
			case "AddBot":
				{
					var usersService = Server_GetUsersService();
					if( usersService != null )
						usersService.AddUserBot( null, null, null );
				}
				break;

			case "DeleteBot":
				{
					var userID = reader.ReadVariableInt64();
					if( !reader.Complete() )
						return false;
					var usersService = Server_GetUsersService();
					if( usersService != null )
					{
						var user = usersService.GetUser( userID );
						if( user != null && user.Bot )
							usersService.RemoveUser( user );
					}
				}
				break;
			}

			return true;
		}

		void Server_SendUsersInfoToClients()
		{
			var m = BeginNetworkMessageToEveryone( "UsersInfo" );
			if( m != null )
			{
				var writer = m.Writer;
				var users = Server_GetUsers();

				writer.WriteVariableInt32( users.Length );
				foreach( RacingServerUserItem item in users )
				{
					writer.WriteVariableInt64( item.User.UserID );
					writer.Write( item.User.Username );
					writer.Write( item.User.Bot );
				}
				m.End();
			}
		}

		/// <summary>
		/// Creates and initializes a new vehicle at the specified position and rotation.
		/// </summary>
		protected virtual Vehicle CreateVehicle( ServerUserItem serverUserItem, SingleUserItem singleUserItem, Vector3 position, Radian rotation )
		{
			var vehicleTypeWithReference = ObjectTypeControlledByPlayer;
			if( vehicleTypeWithReference.ReferenceSpecified && MetadataManager.GetTypeOfNetType( typeof( VehicleType ) ).IsAssignableFrom( vehicleTypeWithReference ) )
			{

				//!!!!calculate position Z

				var transform = new Transform( position, Quaternion.FromRotateByZ( rotation ) );

				var vehicle = ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, vehicleTypeWithReference, transform ) as Vehicle;

				return vehicle;
			}

			return null;
		}

		/// <summary>
		/// Creates and positions the specified number of vehicles at the starting checkpoint.
		/// </summary>
		void CreateVehicles( ServerUserItem[] serverUserItems, SingleUserItem[] singleUserItems, int vehicleCount )
		{
			DeleteVehicles();

			if( vehicleCount <= 0 )
				return;

			var checkpoints = Checkpoints;
			if( checkpoints.Length == 0 )
			{
				Log.Warning( "RacingGameLogic: No checkpoints defined in the scene." );
				return;
			}

			var checkpoint = checkpoints[ 0 ];
			var checkpointDirection = ( checkpoint.Position2 - checkpoint.Position1 ).GetNormalize();
			var checkpointCenter = ( checkpoint.Position1 + checkpoint.Position2 ) * 0.5;

			var checkpointRotationPerpendicular = Quaternion.FromDirectionZAxisUp( new Vector3( checkpointDirection.ToVector2(), 0 ) );
			checkpointRotationPerpendicular *= Quaternion.FromRotateByZ( MathEx.DegreeToRadian( -90 ) );

			var vehicleRotation = MathEx.DegreeToRadian( checkpointRotationPerpendicular.Angles.Yaw );

			var right = checkpointDirection;
			var forward = new Vector2( -right.Y, right.X );

			var vehicleSize = vehicleSizeToCalculateStartPositions;

			var sideGap = vehicleSize.Y * 0.5;
			var backGap = vehicleSize.X * 0.5;

			var rowSpacing = vehicleSize.Y + sideGap;
			var vehicleSpacing = vehicleSize.X + backGap;
			var firstVehicleBackOffset = vehicleSize.X * 0.5 + backGap;

			vehicles = new VehicleItem[ vehicleCount ];

			for( int nVehicle = 0; nVehicle < vehicleCount; nVehicle++ )
			{
				var serverUserItem = serverUserItems != null ? serverUserItems[ nVehicle ] : null;
				var singleUserItem = singleUserItems != null ? singleUserItems[ nVehicle ] : null;

				var row = nVehicle % 2;
				var indexInRow = nVehicle / 2;

				var sideOffset = row == 0 ? -rowSpacing * 0.5 : rowSpacing * 0.5;
				var backOffset = firstVehicleBackOffset + indexInRow * vehicleSpacing;

				var vehiclePosition2 = checkpointCenter.ToVector2() + right.ToVector2() * sideOffset - forward * backOffset;
				var vehiclePosition = new Vector3( vehiclePosition2, checkpointCenter.Z );

				//!!!!calculate exact position Z
				vehiclePosition.Z += 0.5;

				var vehicle = CreateVehicle( serverUserItem, singleUserItem, vehiclePosition, vehicleRotation );

				var vehicleItem = new VehicleItem();
				vehicleItem.Vehicle = vehicle;
				if( serverUserItem != null && serverUserItem.User != null )
					vehicleItem.UserID = serverUserItem.User.UserID;
				if( singleUserItem != null )
					vehicleItem.UserID = singleUserItem.UserID;

				vehicles[ nVehicle ] = vehicleItem;
			}
		}

		void DeleteVehicles()
		{
			foreach( var vehicleItem in vehicles )
				vehicleItem.Vehicle.RemoveFromParent( true );
			vehicles = Array.Empty<VehicleItem>();
		}

		public override NeoAxis.Component ServerOrSingle_CreateObjectControlledByPlayer( ServerUserItem serverUserItem, SingleUserItem singleUserItem, Reference<Metadata.TypeInfo> objectTypeWithReference, Transform transform )
		{
			var obj = base.ServerOrSingle_CreateObjectControlledByPlayer( serverUserItem, singleUserItem, objectTypeWithReference, transform );

			if( obj != null )
			{
				//var character = obj as Character;

				////add CharacterAI to bot
				//if( character != null )
				//{
				//	var bot = false;
				//	if( serverUserItem != null )
				//		bot = serverUserItem.User.Bot;
				//	if( singleUserItem != null )
				//		bot = singleUserItem.Bot;
				//	if( bot )
				//	{
				//		var ai = character.CreateComponent<CharacterAI>();
				//		ai.CombatMode = true;
				//	}
				//}
			}

			return obj;
		}

		ServerNetworkService_Users Server_GetUsersService()
		{
			var serverNode = ParentRoot.HierarchyController?.NetworkServerNode;
			if( serverNode != null )
				return serverNode.GetService<ServerNetworkService_Users>();
			return null;
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			//send to the client about the player count
			{
				var m = BeginNetworkMessage( client, "ChangePlayerCount" );
				if( m != null )
				{
					m.Writer.WriteVariableInt32( vehicles.Length );
					m.End();
				}
			}

			//send to the client about the checkpoint passed count
			for( int nVehicle = 0; nVehicle < vehicles.Length; nVehicle++ )
			{
				if( nVehicle < checkpointsPassed.Length && checkpointsPassed[ nVehicle ] != 0 )
				{
					var m = BeginNetworkMessage( client, "CheckpointsPassed" );
					if( m != null )
					{
						m.Writer.WriteVariableInt32( nVehicle );
						m.Writer.WriteVariableInt32( checkpointsPassed[ nVehicle ] );
						m.End();
					}
				}
			}

			//send to the clients about the checkpoint passed time
			for( int timeIndex = 0; timeIndex < checkpointsPassedTime.Length; timeIndex++ )
			{
				var m = BeginNetworkMessage( client, "CheckpointsPassedTime" );
				if( m != null )
				{
					m.Writer.WriteVariableInt32( timeIndex );
					m.Writer.Write( checkpointsPassedTime[ timeIndex ] );
					m.End();
				}
			}
		}


		///////////////////////////////////////////////
		// Client

		//it is synchronized via a network message of the component. OnReceiveNetworkMessageFromServer method.
		public class ClientUserItem
		{
			public long UserID;
			public string Username;
			public bool Bot;
		}
		Dictionary<long, ClientUserItem> clientUsers = new Dictionary<long, ClientUserItem>();

		bool firstSimulationStepClient = true;

		///////////////////////////////////////////////

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			if( firstSimulationStepClient )
				UpdateCheckpoints();

			firstSimulationStepClient = false;
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "UsersInfo" )
			{
				var count = reader.ReadVariableInt32();
				var newList = new Dictionary<long, ClientUserItem>( count );
				for( int n = 0; n < count; n++ )
				{
					var item = new ClientUserItem();
					item.UserID = reader.ReadVariableInt64();
					item.Username = reader.ReadString();
					item.Bot = reader.ReadBoolean();
					newList[ item.UserID ] = item;
				}
				if( !reader.Complete() )
					return false;

				clientUsers = newList;
			}

			if( message == "ChangePlayerCount" )
			{
				var playerCount = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;

				checkpointsPassed = new int[ playerCount ];
				checkpointsPassedTime = new float[ playerCount * Checkpoints.Length * Laps ];
			}

			if( message == "CheckpointsPassed" )
			{
				var nVehicle = reader.ReadVariableInt32();
				var count = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;

				if( nVehicle < checkpointsPassed.Length )
					checkpointsPassed[ nVehicle ] = count;
			}

			if( message == "CheckpointsPassedTime" )
			{
				var timeIndex = reader.ReadVariableInt32();
				var time = reader.ReadSingle();
				if( !reader.Complete() )
					return false;

				if( timeIndex < checkpointsPassedTime.Length )
					checkpointsPassedTime[ timeIndex ] = time;

				CheckpointsPassedTimeChanged?.Invoke( this );
			}

			return true;
		}

		public ReadOnlyDictionary<long, ClientUserItem> Client_GetUsers()
		{
			return clientUsers.AsReadOnly();
		}

		public ClientUserItem Client_GetUser( long userID )
		{
			if( clientUsers.TryGetValue( userID, out var item ) )
				return item;
			return null;
		}

		public void Client_SendRequestAddBot()
		{
			//!!!!? check can send only if the user is admin

			var m = BeginNetworkMessageToServer( "AddBot" );
			if( m != null )
				m.End();
		}

		public void Client_SendRequestDeleteBot( long userID )
		{
			//!!!!? check can send only if the user is admin

			var m = BeginNetworkMessageToServer( "DeleteBot" );
			if( m != null )
			{
				m.Writer.WriteVariableInt64( userID );
				m.End();
			}
		}
	}
}


//code for 2D physics implementation
//	{

//		void CreateVehicleFor2D()
//		{
//			//for 2D

//			//var rigidBody = carsGroup.CreateComponent<RigidBody2D>( enabled: false );
//			//rigidBody.Transform = new Transform( new Vector3( position, 0 ), Quaternion.FromRotateByZ( rotation ) );
//			//rigidBody.MotionType = RigidBody2D.MotionTypeEnum.Dynamic;
//			//rigidBody.Mass = 1;
//			//rigidBody.LinearDamping = 1;
//			//rigidBody.AngularDamping = 1;

//			//var shape = rigidBody.CreateComponent<CollisionShape2D_Box>();
//			//shape.Dimensions = CarSize;

//			//rigidBody.Enabled = true;

//			//return rigidBody;
//		}

//		///<summary>
//		///Creates an obstacle in the scene with specified position, rotation, and dimensions.
//		///</summary>
//		RigidBody2D CreateObstacle( Vector2 position, Radian rotation, Vector2 dimensions )
//		{
//			var rigidBody = obstaclesGroup.CreateComponent<RigidBody2D>( enabled: false );
//			rigidBody.Transform = new Transform( new Vector3( position, 0 ), Quaternion.FromRotateByZ( rotation ) );

//			var shape = rigidBody.CreateComponent<CollisionShape2D_Box>();
//			shape.Dimensions = dimensions;

//			rigidBody.Enabled = true;

//			return rigidBody;
//		}

//		/// <summary>
//		/// Creates a new 2D checkpoint sensor between two points in world space.
//		/// </summary>
//		public ObjectInSpace CreateCheckpoint( Vector2 from, Vector2 to )
//		{
//			var obj = checkpointsGroup.CreateComponent<ObjectInSpace>( enabled: false );
//			obj.Transform = new Transform( new Vector3( from, 0 ) );

//			var targetObject = obj.CreateComponent<ObjectInSpace>();
//			targetObject.Transform = new Transform( new Vector3( to, 0 ) );

//			//add physics
//			{
//				//column from
//				{
//					var rigidBody = obj.CreateComponent<RigidBody2D>();
//					rigidBody.Transform = new Transform( new Vector3( from, 0 ) );
//					var shape = rigidBody.CreateComponent<CollisionShape2D_Ellipse>();
//					shape.Dimensions = new Vector2( CheckpointColumnRadius * 2, CheckpointColumnRadius * 2 );
//				}
//				//column to
//				{
//					var rigidBody = obj.CreateComponent<RigidBody2D>();
//					rigidBody.Transform = new Transform( new Vector3( to, 0 ) );
//					var shape = rigidBody.CreateComponent<CollisionShape2D_Ellipse>();
//					shape.Dimensions = new Vector2( CheckpointColumnRadius * 2, CheckpointColumnRadius * 2 );
//				}
//			}

//			obj.Enabled = true;

//			checkpointsInfo.Add( new CheckpointInfo { Position1 = from, Position2 = to } );

//			return obj;
//		}

//	}
//}
