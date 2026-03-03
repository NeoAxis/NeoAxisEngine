// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A component to make a seat place.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Seat\Seat", 10579 )]
	public class Seat : MeshInSpace, InteractiveObjectInterface
	{
		//static FastRandom boundsPredictionAndUpdateRandom = new FastRandom( 0 );

		//

		DynamicData dynamicData;
		bool needRecreateDynamicData;

		//bool duringTransformUpdateWithoutRecreating;

		//float remainingTimeToUpdateObjectsOnSeat;

		/////////////////////////////////////////
		//Basic

		const string seatTypeDefault = @"Content\Seats\Default\Default.seattype";

		/// <summary>
		/// The type of the seat.
		/// </summary>
		[DefaultValueReference( seatTypeDefault )]
		public Reference<SeatType> SeatType
		{
			get { if( _seatType.BeginGet() ) SeatType = _seatType.Get( this ); return _seatType.value; }
			set { if( _seatType.BeginSet( this, ref value ) ) { try { SeatTypeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _seatType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SeatType"/> property value changes.</summary>
		public event Action<Seat> SeatTypeChanged;
		ReferenceField<SeatType> _seatType = new Reference<SeatType>( null, seatTypeDefault );

		/// <summary>
		/// Whether to visualize debug info.
		/// </summary>
		//[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Seat> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/// <summary>
		/// The list of objects on the seats.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		[Serialize]
		public ReferenceList<ObjectInSpace> ObjectsOnSeats
		{
			get { return _objectsOnSeats; }
		}
		public delegate void ObjectsOnSeatsChangedDelegate( Seat sender );
		public event ObjectsOnSeatsChangedDelegate ObjectsOnSeatsChanged;
		ReferenceList<ObjectInSpace> _objectsOnSeats;

		/////////////////////////////////////////

		public class DynamicData
		{
			public SeatType SeatType;
			public Mesh Mesh;

			public SeatItem[] Seats;

			/////////////////////

			public class SeatItem
			{
				public NeoAxis.SeatItem SeatComponent;

				//!!!!need make copy? in type can be with reference
				public Transform Transform;
				public Degree SpineAngle;
				public Degree LegsAngle;

				//public Vector3 EyeOffset;
				public Transform ExitTransform;
			}
		}

		/////////////////////////////////////////

		public Seat()
		{
			_objectsOnSeats = new ReferenceList<ObjectInSpace>( this, () => ObjectsOnSeatsChanged?.Invoke( this ) );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
				case nameof( Collision ):
					skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
				CreateDynamicData();
			else
				DestroyDynamicData();
		}

		//public void SetTransform( Transform value, bool recreate )
		//{
		//	if( !recreate )
		//		duringTransformUpdateWithoutRecreating = true;
		//	Transform = value;
		//	if( !recreate )
		//		duringTransformUpdateWithoutRecreating = false;
		//}

		///////////////////////////////////////////

		void UpdateObjectsOnSeats()
		{
			if( dynamicData != null )
			{
				//when update not each time?

				//remainingTimeToUpdateObjectsOnSeat -= delta;
				//if( remainingTimeToUpdateObjectsOnSeat <= 0 )
				//{
				//remainingTimeToUpdateObjectsOnSeat = 0.25f + boundsPredictionAndUpdateRandom.Next( 0.05f );

				for( int seatIndex = 0; seatIndex < dynamicData.Seats.Length; seatIndex++ )
					UpdateObjectOnSeat( seatIndex );

				//}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			UpdateObjectsOnSeats();
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			UpdateObjectsOnSeats();
		}

		void UpdateObjectOnSeat( int seatIndex )
		{
			var objectOnSeat = GetObjectOnSeat( seatIndex );
			if( objectOnSeat != null )
			{
				var seatItem = dynamicData.Seats[ seatIndex ];

				objectOnSeat.Visible = seatItem.SeatComponent.DisplayObject && Visible;

				var character = objectOnSeat as Character;
				if( character != null )
				{
					character.Collision = false;
					character.Sitting = true;
					character.SittingSpineAngle = seatItem.SpineAngle;
					character.SittingLegsAngle = seatItem.LegsAngle;

					if( NetworkIsSingle || NetworkIsClient )
					{
						var seatTransform = seatItem.Transform;
						seatTransform = seatTransform.UpdatePosition( seatTransform.Position + new Vector3( 0, 0, -character.TypeCached.SitButtHeight ) );
						character.SetTransformAndTurnToDirectionInstantly( TransformV * seatTransform );
					}
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needRecreateDynamicData )
				CreateDynamicData();
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			//if( EngineApp.IsEditor && !duringTransformUpdateWithoutRecreating )
			//	NeedRecreateDynamicData();
		}

		void DebugRenderSeat( ViewportRenderingContext context, DynamicData.SeatItem seatItem )
		{
			var renderer = context.Owner.Simple3DRenderer;
			var seatTransform = TransformV;

			//seat
			{
				var color = new ColorValue( 0, 1, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = seatTransform * seatItem.Transform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.02 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}

			//exit
			{
				var color = new ColorValue( 1, 0, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = seatTransform * seatItem.ExitTransform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.02 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!slowly?

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( DebugVisualization )
				{
					var scene = context.Owner.AttachedScene;

					if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && dynamicData != null )//!!!! scene.DisplayPhysicalObjects )
					{
						var renderer = context.Owner.Simple3DRenderer;
						var tr = TransformV;

						//seats
						for( int n = 0; n < dynamicData.Seats.Length; n++ )
						{
							var seatItem = dynamicData.Seats[ n ];
							DebugRenderSeat( context, seatItem );
						}
					}
				}

				//!!!!
				var showLabels = /*show &&*/ dynamicData == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		public virtual void GetFirstPersonCameraPosition( bool useEyesPositionOfModel, out Vector3 position, out Vector3 forward, out Vector3 up )
		{
			var seatTransform = TransformV;

			position = seatTransform.Position;
			forward = seatTransform.Rotation.GetForward();
			up = seatTransform.Rotation.GetUp();

			if( useEyesPositionOfModel )
			{

				//!!!!select seat

				//!!!!turn head support


				if( dynamicData != null && dynamicData.Seats.Length != 0 )
				{
					var seatIndex = 0;

					var objectOnSeat = GetObjectOnSeat( seatIndex );
					if( objectOnSeat != null )
					{
						//var seatItem = dynamicData.Seats[ seatIndex ];

						var character = objectOnSeat as Character;
						if( character != null )
						{
							character.GetFirstPersonCameraPosition( true, out position, out forward, out up );

							//var seatTransform = seatItem.Transform;
							//position = seatTransform * ( seatTransform * seatItem.EyeOffset );
						}
					}
				}
			}
		}

		public delegate void SoundPlayBeforeDelegate( Seat sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public void NeedRecreateDynamicData()
		{
			needRecreateDynamicData = true;
		}

		protected virtual DynamicData OnNewDynamicData()
		{
			return new DynamicData();
		}

		public void CreateDynamicData()
		{
			DestroyDynamicData();

			if( !EnabledInHierarchyAndIsInstance )
				return;

			var scene = ParentScene;
			if( scene == null )
				return;

			var type = SeatType.Value;
			if( type == null )
				return;

			dynamicData = OnNewDynamicData();//new DynamicData();
			dynamicData.SeatType = type;
			dynamicData.Mesh = type.Mesh;

			//var tr = TransformV;

			Mesh = dynamicData.Mesh;//type.Mesh;

			Collision = true;

			//!!!!slowly. make caching in the SeatType
			var seatComponents = dynamicData.SeatType.GetComponents<SeatItem>();
			if( seatComponents.Length != 0 )
			{
				var seatItems = new List<DynamicData.SeatItem>( seatComponents.Length );

				foreach( var seatComponent in seatComponents )
				{
					if( seatComponent.Enabled )
					{
						var seatItem = new DynamicData.SeatItem();
						seatItem.SeatComponent = seatComponent;
						seatItem.Transform = seatComponent.Transform;
						seatItem.SpineAngle = seatComponent.SpineAngle;
						seatItem.LegsAngle = seatComponent.LegsAngle;
						//seatItem.EyeOffset = seatComponent.EyeOffset;
						seatItem.ExitTransform = seatComponent.ExitTransform;

						seatItems.Add( seatItem );
					}
				}

				if( seatItems.Count != 0 )
					dynamicData.Seats = seatItems.ToArray();
			}

			//also can take seats from Seat component

			if( dynamicData.Seats == null )
				dynamicData.Seats = Array.Empty<DynamicData.SeatItem>();

			SpaceBoundsUpdate();

			needRecreateDynamicData = false;
		}

		public void DestroyDynamicData()
		{
			if( dynamicData != null )
			{
				dynamicData = null;
			}
		}

		///////////////////////////////////////////////

		public int GetFreeSeat()// ObjectInSpace forObject )
		{
			if( dynamicData != null )
			{
				var count = dynamicData.Seats.Length;
				for( int n = 0; n < count; n++ )
				{
					if( n >= ObjectsOnSeats.Count || ObjectsOnSeats[ n ].Value == null )
						return n;
				}
			}
			return -1;
		}

		public int GetSeatIndexByObject( ObjectInSpace obj )
		{
			if( dynamicData != null )
			{
				var count = dynamicData.Seats.Length;
				for( int n = 0; n < count; n++ )
				{
					if( n < ObjectsOnSeats.Count && ObjectsOnSeats[ n ].Value == obj )
						return n;
				}
			}
			return -1;
		}

		public delegate void InteractionGetInfoEventDelegate( Seat sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			//control by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null && !character.Sitting )
			{
				var seatIndex = GetFreeSeat();
				if( seatIndex != -1 )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;
					info.Text = $"Press {gameMode.KeyInteract1.Value} to sit";
				}
			}
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		public ObjectInSpace GetObjectOnSeat( int seatIndex )
		{
			if( seatIndex < ObjectsOnSeats.Count )
				return ObjectsOnSeats[ seatIndex ];
			return null;
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				if( keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2 )
				{
					//start control by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null )
					{
						var seatIndex = GetFreeSeat();
						if( seatIndex != -1 )
						{
							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "PutObjectToSeat" );
								if( m != null )
								{
									m.Writer.WriteVariableInt32( seatIndex );
									m.Writer.WriteVariableUInt64( (ulong)character.NetworkID );
									m.End();
								}
							}
							else
							{
								PutObjectToSeat( gameMode, seatIndex, character );
								GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
							}

							return true;
						}
					}
				}
			}

			return false;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;


			//!!!!security check is needed


			if( message == "PutObjectToSeat" )
			{
				var seatIndex = reader.ReadVariableInt32();
				var characterNetworkID = (long)reader.ReadVariableUInt64();
				if( !reader.Complete() )
					return false;

				var obj = ParentRoot.HierarchyController.GetComponentByNetworkID( characterNetworkID ) as ObjectInSpace;
				if( obj != null )
				{
					var networkLogic = NetworkLogicUtility.GetNetworkLogic( obj );
					if( networkLogic != null )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();//var gameMode = ParentScene?.GetComponent<GameMode>();
						if( gameMode != null )
						{
							PutObjectToSeat( gameMode, seatIndex, obj );
						}
					}
				}
			}

			return true;
		}

		public virtual void PutObjectToSeat( GameMode gameMode, int seatIndex, ObjectInSpace obj )
		{
			while( seatIndex >= ObjectsOnSeats.Count )
				ObjectsOnSeats.Add( null );
			ObjectsOnSeats[ seatIndex ] = ReferenceUtility.MakeRootReference( obj );

			//deactivate active items for character
			var character = obj as Character;
			if( character != null )
				character.DeactivateAllItems( gameMode );

			if( NetworkIsSingle || NetworkIsClient )
				UpdateObjectOnSeat( seatIndex );

			//remainingTimeToUpdateObjectsOnSeat = 0;
		}

		public virtual void RemoveObjectFromSeat( int seatIndex )
		{
			if( dynamicData != null )
			{
				DynamicData.SeatItem seatItem = null;
				if( seatIndex < dynamicData.Seats.Length )
					seatItem = dynamicData.Seats[ seatIndex ];

				var objectOnSeat = GetObjectOnSeat( seatIndex );
				if( objectOnSeat != null && seatItem != null )
				{
					if( !objectOnSeat.Disposed )
					{
						var character = objectOnSeat as Character;
						if( character != null && !character.Disposed )
						{
							character.Sitting = false;
							character.Collision = true;
							character.Visible = true;

							var tr = TransformV * seatItem.ExitTransform;
							var scaleFactor = character.GetScaleFactor();
							if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 3, -character.TypeCached.Height * scaleFactor / 3, character.TypeCached.Height * scaleFactor / 10, out var freePlacePosition ) )
							{
								if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 3, -character.TypeCached.Height * scaleFactor / 3, character.TypeCached.Height * scaleFactor / 3, out freePlacePosition ) )
								{
									if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 5, -character.TypeCached.Height * scaleFactor, character.TypeCached.Height * scaleFactor, out freePlacePosition ) )
									{
										freePlacePosition = tr.Position + new Vector3( 0, 0, character.TypeCached.Height * scaleFactor );
									}
								}
							}
							tr = tr.UpdatePosition( freePlacePosition );
							character.SetTransformAndTurnToDirectionInstantly( tr );
						}
					}

					ObjectsOnSeats[ seatIndex ] = null;
				}
			}

			//remainingTimeToUpdateObjectsOnSeat = 0;
		}

		//protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		//{
		//	if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
		//		return false;

		//	return true;
		//}
	}
}
