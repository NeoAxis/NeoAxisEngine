// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;


//!!!!
//castles


namespace NeoAxis
{
	/// <summary>
	/// Represents a fence in the scene.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Fence\Fence", 510 )]
	[SettingsCell( typeof( FenceSettingsCell ) )]
#endif
	public class Fence : CurveInSpace
	{
		LogicalData logicalData;
		VisualData visualData;
		bool needUpdate;
		bool needUpdateAfterEndModifyingTransformTool;

		//

		const string defaultFenceType = @"Content\Constructors\Fences\Wood fence\Brown 1.fencetype";

		/// <summary>
		/// The type of the fence.
		/// </summary>
		[DefaultValueReference( defaultFenceType )]
		public Reference<FenceType> FenceType
		{
			get { if( _fenceType.BeginGet() ) FenceType = _fenceType.Get( this ); return _fenceType.value; }
			set { if( _fenceType.BeginSet( ref value ) ) { try { FenceTypeChanged?.Invoke( this ); DataWasChanged(); } finally { _fenceType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FenceType"/> property value changes.</summary>
		public event Action<Fence> FenceTypeChanged;
		ReferenceField<FenceType> _fenceType = new Reference<FenceType>( null, defaultFenceType );

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( true )]
		//[Category( "Physics" )]
		public Reference<bool> FenceCollision
		{
			get { if( _fenceCollision.BeginGet() ) FenceCollision = _fenceCollision.Get( this ); return _fenceCollision.value; }
			set { if( _fenceCollision.BeginSet( ref value ) ) { try { FenceCollisionChanged?.Invoke( this ); DataWasChanged(); } finally { _fenceCollision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FenceCollision"/> property value changes.</summary>
		public event Action<Fence> FenceCollisionChanged;
		ReferenceField<bool> _fenceCollision = true;

		//!!!!при обновлении может только удалять visual data
		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> ColorMultiplier
		{
			get { if( _colorMultiplier.BeginGet() ) ColorMultiplier = _colorMultiplier.Get( this ); return _colorMultiplier.value; }
			set { if( _colorMultiplier.BeginSet( ref value ) ) { try { ColorMultiplierChanged?.Invoke( this ); DataWasChanged(); } finally { _colorMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ColorMultiplier"/> property value changes.</summary>
		public event Action<Fence> ColorMultiplierChanged;
		ReferenceField<ColorValue> _colorMultiplier = ColorValue.One;

		/// <summary>
		/// Whether to add special mesh to the point.
		/// </summary>
		[DefaultValue( FencePoint.SpecialtyEnum.None )]
		public Reference<FencePoint.SpecialtyEnum> PointSpecialty
		{
			get { if( _pointSpecialty.BeginGet() ) PointSpecialty = _pointSpecialty.Get( this ); return _pointSpecialty.value; }
			set { if( _pointSpecialty.BeginSet( ref value ) ) { try { PointSpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _pointSpecialty.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointSpecialty"/> property value changes.</summary>
		public event Action<Fence> PointSpecialtyChanged;
		ReferenceField<FencePoint.SpecialtyEnum> _pointSpecialty = FencePoint.SpecialtyEnum.None;

		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		///////////////////////////////////////////////

		public class LogicalData
		{
			public Fence Owner;
			public FenceType FenceType;
			public int FenceTypeVersion;

			public Point[] Points;
			public Curve PositionCurve;
			//public Curve Forward;
			//public Curve Up;
			public Curve ScaleCurve;
			public double Length;

			public Dictionary<LogicalData, List<ConnectedFenceItem>> ConnectedFences = new Dictionary<LogicalData, List<ConnectedFenceItem>>();

			public List<Scene.PhysicsWorldClass.Body> CollisionBodies = new List<Scene.PhysicsWorldClass.Body>();

			/////////////////////

			public class Point
			{
				public Transform Transform;
				internal double _curveTimeOnlyToSort;
				public FencePoint.SpecialtyEnum Specialty;

				public double TimeOnCurve;
			}

			/////////////////////

			public class ConnectedFenceItem
			{
				public LogicalData ConnectedFence;

				public double ThisFence_TimeOnCurve;
				public Vector3 ThisFence_Position;//only for optimization. can get by ThisFence_TimeOnCurve

				public double ConnectedFence_TimeOnCurve;
				public Vector3 ConnectedFence_Position;//only for optimization. can get by ConnectedFence_TimeOnCurve
			}

			internal void GenerateCurveData()
			{
				//generate Points

				var pointComponents = Owner.GetComponents<FencePoint>( onlyEnabledInHierarchy: true );

				Points = new Point[ 1 + pointComponents.Length ];
				Points[ 0 ] = new Point() { Transform = Owner.Transform, _curveTimeOnlyToSort = Owner.Time, Specialty = Owner.PointSpecialty };
				for( int n = 0; n < pointComponents.Length; n++ )
				{
					var pointComponent = pointComponents[ n ];
					Points[ n + 1 ] = new Point() { Transform = pointComponent.Transform, _curveTimeOnlyToSort = pointComponent.Time, Specialty = pointComponent.Specialty };
				}

				//sort by time
				bool allPointsZeroTime = Points.All( p => p._curveTimeOnlyToSort == 0 );
				if( !allPointsZeroTime )
				{
					CollectionUtility.MergeSort( Points, delegate ( Point a, Point b )
					{
						if( a._curveTimeOnlyToSort < b._curveTimeOnlyToSort )
							return -1;
						if( a._curveTimeOnlyToSort > b._curveTimeOnlyToSort )
							return 1;
						return 0;
					} );
				}

				//generate curves

				PositionCurve = new CurveLine();
				//Forward = new CurveLine();
				//Up = new CurveLine();
				ScaleCurve = new CurveLine();

				for( int n = 0; n < Points.Length; n++ )
				{
					var point = Points[ n ];

					point.TimeOnCurve = n;

					var time = n;
					PositionCurve.AddPoint( time, point.Transform.Position );
					ScaleCurve.AddPoint( time, point.Transform.Scale );
				}

				for( int n = 1; n < Points.Length; n++ )
					Length += ( Points[ n ].Transform.Position - Points[ n - 1 ].Transform.Position ).Length();
			}

			public bool GetCurveTimeByPosition( Vector3 position, double maxDistanceToCurve, out double timeOnCurve )
			{


				//!!!!slowly


				var step = 0.05;
				var maxTime = LastPoint.TimeOnCurve;
				var maxTime2 = maxTime + step;

				Vector3? previousPosition = null;

				for( double time = 0; time <= maxTime2; time += step )
				{
					var time2 = time;
					if( time2 > maxTime )
						time2 = maxTime;

					var trPosition = GetPositionByTime( time2 );

					if( previousPosition.HasValue )
					{
						var projected = MathAlgorithms.ProjectPointToLine( previousPosition.Value, trPosition, position );

						var b = new Bounds( previousPosition.Value );
						b.Add( trPosition );
						b.Expand( 0.001 );
						if( b.Contains( projected ) && ( projected - position ).LengthSquared() <= maxDistanceToCurve * maxDistanceToCurve )
						{
							timeOnCurve = time2;
							return true;
						}
					}

					previousPosition = trPosition;
				}

				timeOnCurve = 0;
				return false;
			}

			public int GetPointByPosition( Vector3 position, double maxDistanceToPoint )
			{
				for( int n = 0; n < Points.Length; n++ )
				{
					var point = Points[ n ];
					if( ( point.Transform.Position - position ).LengthSquared() < maxDistanceToPoint * maxDistanceToPoint )
						return n;
				}
				return -1;
			}

			internal void AddConnectedFence( LogicalData data, ConnectedFenceItem item )
			{
				if( !ConnectedFences.TryGetValue( data, out var list ) )
				{
					list = new List<ConnectedFenceItem>();
					ConnectedFences[ data ] = list;
				}
				list.Add( item );
			}

			bool IsAnyTransformToolInModifyingMode()
			{
#if !DEPLOY
				if( EngineApp.IsEditor )
				{
					foreach( var instance in EngineViewportControl.AllInstances )
					{
						var transformTool = instance.TransformTool as TransformTool;
						if( transformTool != null && transformTool.Modifying )
							return true;
					}
				}
#endif
				return false;
			}

			internal void ConnectFences( LogicalData connectOnlyToThisFence )
			{
				if( Points.Length < 2 )
					return;

				if( IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				//connect by start, end points
				for( int nPoint = 0; nPoint < 2; nPoint++ )
				{
					var point = Points[ nPoint == 0 ? 0 : Points.Length - 1 ];
					var pointPosition = point.Transform.Position;

					var scene = Owner.ParentScene;

					//!!!!
					double maxDistanceToCurve = 0.05;

					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Bounds( pointPosition ) );
					scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var fence = item.Object as Fence;
						if( fence != null && fence != Owner )
						{
							if( connectOnlyToThisFence != null && connectOnlyToThisFence.Owner != fence )
								continue;

							var data = fence.GetLogicalData();
							if( data != null && data.GetCurveTimeByPosition( pointPosition, maxDistanceToCurve, out var timeOnCurve ) )
							{
								//!!!!угол или линия

								var positionOnCurve = data.GetPositionByTime( timeOnCurve );

								//add to this fence
								{
									var connectedFenceItem = new ConnectedFenceItem();
									connectedFenceItem.ConnectedFence = data;

									connectedFenceItem.ThisFence_TimeOnCurve = point.TimeOnCurve;
									connectedFenceItem.ThisFence_Position = pointPosition;

									connectedFenceItem.ConnectedFence_TimeOnCurve = timeOnCurve;
									connectedFenceItem.ConnectedFence_Position = positionOnCurve;

									AddConnectedFence( data, connectedFenceItem );
								}

								//add to connected fence
								{
									var connectedFenceItem = new ConnectedFenceItem();
									connectedFenceItem.ConnectedFence = this;

									connectedFenceItem.ThisFence_TimeOnCurve = timeOnCurve;
									connectedFenceItem.ThisFence_Position = positionOnCurve;

									connectedFenceItem.ConnectedFence_TimeOnCurve = point.TimeOnCurve;
									connectedFenceItem.ConnectedFence_Position = pointPosition;

									data.AddConnectedFence( this, connectedFenceItem );
								}

								//need update visual data of the connected fence
								data.Owner.DeleteVisualData();
							}
						}
					}

				}
			}

			internal void DisconnectFences()
			{
				if( ConnectedFences.Count != 0 )
				{
					//remove this fence from connected fences
					foreach( var connectedFence in ConnectedFences.Keys )
					{
						if( connectedFence.ConnectedFences.Remove( this ) )
						{
							//reset visual data
							connectedFence.Owner.DeleteVisualData();
						}
					}

					ConnectedFences.Clear();

					//reset visual data
					Owner.DeleteVisualData();
				}
			}

			public Vector3 GetPositionByTime( double time )
			{
				return PositionCurve.CalculateValueByTime( time );
			}

			public Vector3 GetDirectionByTime( double time )
			{
				var points = Points;
				for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
				{
					var pointFrom = points[ nPoint ];
					var pointTo = points[ nPoint + 1 ];

					if( time >= pointFrom.TimeOnCurve && time <= pointTo.TimeOnCurve )
					{
						var from = pointFrom.Transform.Position;
						var to = pointTo.Transform.Position;

						if( to != from )
						{
							var direction = ( to - from ).GetNormalize();
							return direction;
						}
					}
				}

				return Vector3.XAxis;
			}

			public Point LastPoint
			{
				get { return Points[ Points.Length - 1 ]; }
			}

			public List<(Mesh mesh, Transform transform, double? clipDistancePanelLength, double? clipDistanceFactor, Material replaceMaterial)> GetMeshesToCreate()
			{
				var result = new List<(Mesh mesh, Transform transform, double? clipDistancePanelLength, double? clipDistanceFactor, Material replaceMaterial)>();

				var fenceType = FenceType;
				var panelLength = fenceType.PanelLength.Value;
				var points = Points;

				//posts
				for( int nPoint = 0; nPoint < points.Length; nPoint++ )
				{
					var point = points[ nPoint ];

					if( point.Specialty != FencePoint.SpecialtyEnum.NoPost )
					{
						Mesh mesh = null;

						if( nPoint == 0 || nPoint == points.Length - 1 )
						{
							bool skip = false;
							foreach( var itemsList in ConnectedFences.Values )
							{
								foreach( var item in itemsList )
								{
									var pointIndex = item.ConnectedFence.GetPointByPosition( point.Transform.Position, 0.1 );
									if( pointIndex != -1 )
									{
										skip = true;
										break;
									}
								}
							}

							if( !skip )
								mesh = fenceType.EndPost.Value;
						}
						else
						{
							var vector1 = points[ nPoint ].Transform.Position - points[ nPoint - 1 ].Transform.Position;
							var vector2 = points[ nPoint + 1 ].Transform.Position - points[ nPoint ].Transform.Position;
							var line = MathAlgorithms.GetVectorsAngle( vector1, vector2 ).InDegrees() < 1;

							if( line )
								mesh = fenceType.LinePost.Value;
							else
								mesh = fenceType.CornerPost.Value;
						}

						if( mesh != null )
						{
							var direction2 = GetDirectionByTime( point.TimeOnCurve ).ToVector2().GetNormalize();
							var rot = Quaternion.LookAt( new Vector3( direction2, 0 ), point.Transform.Rotation.GetUp() );
							//var rot = Quaternion.FromDirectionZAxisUp( GetDirectionByTime( point.TimeOnCurve ) );

							result.Add( (mesh, point.Transform.UpdateRotation( rot ), null, null, fenceType.PostReplaceMaterial) );
						}
					}
				}

				//panels, step posts
				for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
				{
					var pointFrom = points[ nPoint ];
					var pointTo = points[ nPoint + 1 ];

					var panelLengthScaled = panelLength * pointFrom.Transform.Scale.MaxComponent();

					var from = pointFrom.Transform.Position;
					var to = pointTo.Transform.Position;

					var direction = ( to - from ).GetNormalize();
					var direction2 = ( to.ToVector2() - from.ToVector2() ).GetNormalize();
					//var length = ( to - from ).Length();
					var length2 = ( to.ToVector2() - from.ToVector2() ).Length();

					var fullPanel = fenceType.FullPanel.Value;
					var halfPanel = fenceType.HalfPanel.Value;
					var quarterPanel = fenceType.QuarterPanel.Value;
					var stepPost = fenceType.StepPost.Value;

					double lengthPos = 0;

					//var direction2 = new Vector3( direction.X, direction.Y, 0 ).GetNormalize();
					var rot = Quaternion.LookAt( new Vector3( direction2, 0 ), pointFrom.Transform.Rotation.GetUp() );
					//var rot = Quaternion.LookAt( direction, pointFrom.Transform.Rotation.GetUp() );

					//full panel
					if( fullPanel != null )
					{
						for( ; lengthPos <= length2 - panelLengthScaled + 0.01; lengthPos += panelLengthScaled )
						{
							var tr = new Transform( from + new Vector3( direction2 * lengthPos, direction.Z * lengthPos ), rot, pointFrom.Transform.Scale );
							result.Add( (fullPanel, tr, null, null, fenceType.PanelReplaceMaterial) );
							if( lengthPos != 0 && stepPost != null )
								result.Add( (stepPost, tr, null, null, fenceType.PostReplaceMaterial) );
						}
					}

					//half panel
					if( halfPanel != null )
					{
						for( ; lengthPos <= length2 - panelLengthScaled * 0.5 + 0.01; lengthPos += panelLengthScaled * 0.5 )
						{
							var tr = new Transform( from + new Vector3( direction2 * lengthPos, direction.Z * lengthPos ), rot, pointFrom.Transform.Scale );
							result.Add( (halfPanel, tr, null, null, fenceType.PanelReplaceMaterial) );
							if( lengthPos != 0 && stepPost != null )
								result.Add( (stepPost, tr, null, null, fenceType.PostReplaceMaterial) );
						}
					}

					//quater panel
					if( quarterPanel != null )
					{
						for( ; lengthPos <= length2 - panelLengthScaled * 0.25 + 0.01; lengthPos += panelLengthScaled * 0.25 )
						{
							var tr = new Transform( from + new Vector3( direction2 * lengthPos, direction.Z * lengthPos ), rot, pointFrom.Transform.Scale );
							result.Add( (quarterPanel, tr, null, null, fenceType.PanelReplaceMaterial) );
							if( lengthPos != 0 && stepPost != null )
								result.Add( (stepPost, tr, null, null, fenceType.PostReplaceMaterial) );
						}
					}

					//fill empty space by CSG
					if( lengthPos < length2 - 0.01 && fullPanel != null && fenceType.FillEmptySpaceByCSG )
					{
						Mesh panel = fullPanel;
						var length = panelLength;
						if( quarterPanel != null )
						{
							panel = quarterPanel;
							length = panelLength / 4;
						}
						else if( halfPanel != null )
						{
							panel = halfPanel;
							length = panelLength / 2;
						}

						var factor = MathEx.Saturate( ( length2 - lengthPos ) / length );
						if( factor > 0.01 )
						//var clipDistance = length2 - lengthPos;
						//if( clipDistance > 0.01 )
						{
							var tr = new Transform( from + new Vector3( direction2 * lengthPos, direction.Z * lengthPos ), rot, pointFrom.Transform.Scale );

							result.Add( (panel, tr, length, factor, fenceType.PanelReplaceMaterial) );

							if( lengthPos != 0 && stepPost != null )
								result.Add( (stepPost, tr, null, null, fenceType.PostReplaceMaterial) );
						}
					}
				}

				return result;
			}

			public void UpdateCollisionBodies()
			{
				DestroyCollisionBodies();

				if( !Owner.FenceCollision.Value )
					return;
				if( IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				var scene = Owner.ParentScene;
				if( scene == null )
					return;

				var collisionDefinitionByMesh = new Dictionary<Mesh, RigidBody>( 32 );

				foreach( var item in GetMeshesToCreate() )
				{
					if( !collisionDefinitionByMesh.TryGetValue( item.mesh, out var collisionDefinition ) )
					{
						collisionDefinition = item.mesh.GetComponent( "Collision Definition" ) as RigidBody;
						if( collisionDefinition == null )
							collisionDefinition = item.mesh?.Result.GetAutoCollisionDefinition();
						collisionDefinitionByMesh[ item.mesh ] = collisionDefinition;
					}

					if( collisionDefinition != null )
					{
						Transform tr;
						if( item.clipDistanceFactor.HasValue )
							tr = item.transform.UpdateScale( item.transform.Scale * new Vector3( item.clipDistanceFactor.Value, 1, 1 ) );
						else
							tr = item.transform;

						var shapeItem = scene.PhysicsWorld.AllocateShape( collisionDefinition, tr.Scale );
						if( shapeItem != null )
						{
							var bodyItem = scene.PhysicsWorld.CreateRigidBodyStatic( shapeItem, true, this, tr.Position, tr.Rotation.ToQuaternionF() );
							if( bodyItem != null )
								CollisionBodies.Add( bodyItem );
						}
					}
				}

				//!!!!
				//UpdateCollisionMaterial();
			}

			//public void UpdateCollisionMaterial()
			//{
			//	if( CollisionBody != null )
			//	{
			//		CollisionBody.Material = PipeType.CollisionMaterial;
			//		CollisionBody.MaterialFrictionMode = PipeType.CollisionFrictionMode;
			//		CollisionBody.MaterialFriction = PipeType.CollisionFriction;
			//		CollisionBody.MaterialAnisotropicFriction = PipeType.CollisionAnisotropicFriction;
			//		CollisionBody.MaterialSpinningFriction = PipeType.CollisionSpinningFriction;
			//		CollisionBody.MaterialRollingFriction = PipeType.CollisionRollingFriction;
			//		CollisionBody.MaterialRestitution = PipeType.CollisionRestitution;
			//	}
			//}

			public void DestroyCollisionBodies()
			{
				foreach( var body in CollisionBodies )
					body.Dispose();
				CollisionBodies.Clear();
			}
		}

		///////////////////////////////////////////////

		public class VisualData
		{
			public GroupOfObjects groupOfObjects;
			public List<GroupOfObjects.SubGroup> groupOfObjectSubGroups = new List<GroupOfObjects.SubGroup>();
			public List<MeshInSpace> meshesInSpace = new List<MeshInSpace>();
		}

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<FencePoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( CurveTypePosition ):
				case nameof( RoundedLineCurvatureRadius ):
				case nameof( CurveTypeRotation ):
				case nameof( CurveTypeScale ):
				case nameof( TimeScale ):
				case nameof( Color ):
				case nameof( Thickness ):
				case nameof( DisplayCurveInEditor ):
				case nameof( DisplayCurveInSimulation ):
					skip = true;
					break;
				}
			}
		}

		public LogicalData GetLogicalData( bool canCreate = true )
		{
			if( logicalData == null && EnabledInHierarchyAndIsInstance && canCreate )
			{
				logicalData = new LogicalData();
				logicalData.Owner = this;
				logicalData.FenceType = FenceType;
				if( logicalData.FenceType == null )
					logicalData.FenceType = new FenceType();
				logicalData.FenceTypeVersion = logicalData.FenceType.Version;

				logicalData.GenerateCurveData();

				logicalData.ConnectFences( null );

				//inform another fences in the bounds to connect this fence
				var scene = ParentScene;
				if( scene != null )
				{
					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, SpaceBounds.BoundingBox );
					scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var fence = item.Object as Fence;
						if( fence != null && fence != this )
						{
							var data = fence.GetLogicalData( false );
							if( data != null )
								data.ConnectFences( logicalData );
						}
					}
				}

				logicalData.UpdateCollisionBodies();
			}
			return logicalData;
		}

		void DeleteLogicalData()
		{
			if( logicalData != null )
			{
				logicalData.DestroyCollisionBodies();
				logicalData.DisconnectFences();
				logicalData = null;
			}
		}

		GroupOfObjects GetOrCreateGroupOfObjects( bool canCreate )
		{
			var scene = ParentScene;
			if( scene == null )
				return null;

			var name = "__GroupOfObjectsFences";

			var group = scene.GetComponent<GroupOfObjects>( name );
			if( group == null && canCreate )
			{
				//need set ShowInEditor = false before AddComponent
				group = ComponentUtility.CreateComponent<GroupOfObjects>( null, false, false );
				group.DisplayInEditor = false;
				scene.AddComponent( group, -1 );
				//var group = scene.CreateComponent<GroupOfObjects>();

				group.Name = name;
				//group.CanBeSelected = false;
				group.SaveSupport = false;
				group.CloneSupport = false;

				group.AnyData = new Dictionary<(Mesh, Material), int>();

				//!!!!
				group.SectorSize = new Vector3( 100, 100, 10000 );

				group.Enabled = true;
			}

			return group;
		}

		ushort GetOrCreateGroupOfObjectsElement( GroupOfObjects group, Mesh mesh, Material replaceMaterial )
		{
			var key = (mesh, replaceMaterial);

			var dictionary = (Dictionary<(Mesh mesh, Material), int>)group.AnyData;

			GroupOfObjectsElement_Mesh element = null;

			if( dictionary.TryGetValue( key, out var elementIndex2 ) )
				return (ushort)elementIndex2;

			//var elements = group.GetComponents<GroupOfObjectsElement_Mesh>();
			//foreach( var e in elements )
			//{
			//	if( e.Mesh.Value == mesh )
			//	{
			//		element = e;
			//		break;
			//	}
			//}

			if( element == null )
			{
				var elementIndex = group.GetFreeElementIndex();
				element = group.CreateComponent<GroupOfObjectsElement_Mesh>( enabled: false );
				element.Name = "Element " + elementIndex.ToString();
				element.Index = elementIndex;
				element.Mesh = mesh;
				element.ReplaceMaterial = replaceMaterial;
				element.AutoAlign = false;
				element.Enabled = true;

				dictionary[ key ] = element.Index;

				group.ElementTypesCacheNeedUpdate();
			}

			return (ushort)element.Index.Value;
		}

		unsafe void CreateMeshObject( Mesh mesh, Transform transform, double? clipDistance, Material replaceMaterial, bool useGroupOfObjects, OpenList<GroupOfObjects.Object> groupOfObjectsObjects )
		{
			if( useGroupOfObjects && !clipDistance.HasValue )
			{
				var group = GetOrCreateGroupOfObjects( true );
				if( group != null )
				{
					var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh, replaceMaterial );
					var pos = transform.Position;
					var rot = transform.Rotation.ToQuaternionF();
					var scl = transform.Scale.ToVector3F();

					var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, ColorMultiplier, Vector4F.Zero, Vector4F.Zero, 0 );

					groupOfObjectsObjects.Add( ref obj );
				}
			}
			else
			{
				//need set ShowInEditor = false before AddComponent
				var meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
				//generator.MeshInSpace = meshInSpace;
				meshInSpace.DisplayInEditor = false;
				AddComponent( meshInSpace, -1 );
				//var meshInSpace = CreateComponent<MeshInSpace>();

				meshInSpace.Name = "__Mesh In Space";
				meshInSpace.CanBeSelected = false;
				meshInSpace.SaveSupport = false;
				meshInSpace.CloneSupport = false;
				meshInSpace.Transform = transform;
				meshInSpace.Color = ColorMultiplier;
				meshInSpace.ReplaceMaterial = replaceMaterial;

				if( clipDistance.HasValue )
				{
					//!!!!Plane always clip. bug

					var box = new Box( transform.Position, new Vector3( clipDistance.Value * 2, 10000, 10000 ), transform.Rotation.ToMatrix3() );
					var item = new RenderingPipeline.RenderSceneData.CutVolumeItem( box, CutVolumeFlags.Invert | CutVolumeFlags.CutScene | CutVolumeFlags.CutShadows );
					//var item = new RenderingPipeline.RenderSceneData.CutVolumeItem( box, true, true, true, false );

					//var item = new RenderingPipeline.RenderSceneData.CutVolumeItem();
					//item.Shape = CutVolumeShape.Plane;
					//item.CutScene = true;
					//item.CutShadows = true;

					//var dir = transform.Rotation.GetForward();
					//item.Plane = Plane.FromPointAndNormal( transform.Position + dir * clipDistance.Value, -dir );

					meshInSpace.CutVolumes = new[] { item };
				}

				meshInSpace.Mesh = mesh;// ReferenceUtility.MakeThisReference( meshInSpace, mesh );
				meshInSpace.Enabled = true;

				visualData.meshesInSpace.Add( meshInSpace );
			}
		}

		public VisualData GetVisualData()
		{
			if( visualData == null )
			{
				GetLogicalData();
				if( logicalData != null )
				{
					visualData = new VisualData();

					var group = GetOrCreateGroupOfObjects( true );
					if( group != null )
					{
						visualData.groupOfObjects = group;

						var groupOfObjectsObjects = new OpenList<GroupOfObjects.Object>( 64 );

						foreach( var item in logicalData.GetMeshesToCreate() )
							CreateMeshObject( item.mesh, item.transform, item.clipDistancePanelLength * item.clipDistanceFactor, item.replaceMaterial, true, groupOfObjectsObjects );

						if( groupOfObjectsObjects.Count != 0 )
						{
							var subGroup = new GroupOfObjects.SubGroup( groupOfObjectsObjects.ArraySegment );
							group.AddSubGroupToQueue( subGroup );
							visualData.groupOfObjectSubGroups.Add( subGroup );
						}
					}
				}
			}
			return visualData;
		}

		void DeleteVisualData()
		{
			if( visualData != null )
			{
				var group = visualData.groupOfObjects;
				if( group != null && visualData.groupOfObjectSubGroups.Count != 0 )
				{
					foreach( var subGroup in visualData.groupOfObjectSubGroups )
						group.RemoveSubGroupToQueue( subGroup );
				}

				foreach( var c in visualData.meshesInSpace )
					c.Dispose();

				visualData = null;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformTool.AllInstances_ModifyCommit += TransformTool_AllInstances_ModifyCommit;
					TransformTool.AllInstances_ModifyCancel += TransformTool_AllInstances_ModifyCancel;
#endif

					if( logicalData == null )
						Update();
				}
				else
				{
					scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformTool.AllInstances_ModifyCommit -= TransformTool_AllInstances_ModifyCommit;
					TransformTool.AllInstances_ModifyCancel -= TransformTool_AllInstances_ModifyCancel;
#endif

					Update();
				}
			}
		}

#if !DEPLOY

		private void TransformTool_AllInstances_ModifyCommit( TransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}

		private void TransformTool_AllInstances_ModifyCancel( TransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}

#endif

		public void Update()
		{
			DeleteVisualData();
			DeleteLogicalData();

			if( EnabledInHierarchyAndIsInstance )
			{
				GetLogicalData();
				SpaceBoundsUpdate();
			}

			needUpdate = false;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var data = GetLogicalData();
			if( data != null && data.Points.Length != 0 )
			{
				var bounds = new Bounds( data.Points[ 0 ].Transform.Position );
				for( int n = 1; n < data.Points.Length; n++ )
					bounds.Add( data.Points[ n ].Transform.Position );

				//!!!!
				bounds.Add( bounds.Minimum + new Vector3( -0.5, -0.5, 0 ) );
				bounds.Add( bounds.Maximum + new Vector3( 0.5, 0.5, 0.5 ) );
				//bounds.Expand( 0.5 );
				//bounds.Expand( data.FenceType.OutsideDiameter * 0.5 );

				newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EditorAutoUpdate && EnabledInHierarchyAndIsInstance )
			{
				if( logicalData != null && logicalData.FenceType.Version != logicalData.FenceTypeVersion )
					needUpdate = true;

				if( needUpdate )
					Update();
			}
		}

		private void Scene_ViewportUpdateBefore( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			//!!!!лучше всё же в OnGetRenderSceneData
			//хотя SpaceBounds видимо больше должен быть, учитывать габариты мешей
			//когда долго не видим объект, данные можно удалять. лишь бы не было что создается/удаляется каждый раз

			//!!!!может VisibleInHierarchy менять флаг в объектах

			if( VisibleInHierarchy )
				GetVisualData();
			else
				DeleteVisualData();
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!тут?
			////!!!!не сразу обновляет если только тут
			//GetVisualData();
		}

		protected override bool GetAllowDisplayCurve()
		{
			return false;
		}

		public override void DataWasChanged()
		{
			base.DataWasChanged();

			if( EnabledInHierarchyAndIsInstance )
				needUpdate = true;
		}
	}
}
