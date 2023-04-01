// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// An instance of a pipe.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Pipe\Pipe", 9200 )]
	[SettingsCell( typeof( PipeSettingsCell ) )]
#endif
	public class Pipe : CurveInSpace
	{
		//readonly bool useGroupOfObjects = true;

		LogicalData logicalData;
		VisualData visualData;
		bool needUpdate;
		bool needUpdateAfterEndModifyingTransformTool;
		bool needUpdateVisualData;

		//

		[DefaultValue( null )]
		public Reference<PipeType> PipeType
		{
			get { if( _pipeType.BeginGet() ) PipeType = _pipeType.Get( this ); return _pipeType.value; }
			set { if( _pipeType.BeginSet( ref value ) ) { try { PipeTypeChanged?.Invoke( this ); DataWasChanged(); } finally { _pipeType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PipeType"/> property value changes.</summary>
		public event Action<Pipe> PipeTypeChanged;
		ReferenceField<PipeType> _pipeType = null;

		/// <summary>
		/// Whether to have a collision body.
		/// </summary>
		[DefaultValue( false )]
		//[Category( "Physics" )]
		public Reference<bool> PipeCollision
		{
			get { if( _pipeCollision.BeginGet() ) PipeCollision = _pipeCollision.Get( this ); return _pipeCollision.value; }
			set { if( _pipeCollision.BeginSet( ref value ) ) { try { PipeCollisionChanged?.Invoke( this ); DataWasChanged(); } finally { _pipeCollision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PipeCollision"/> property value changes.</summary>
		public event Action<Pipe> PipeCollisionChanged;
		ReferenceField<bool> _pipeCollision = false;

		/// <summary>
		/// Whether to add special mesh to the point.
		/// </summary>
		//[Category( "Pipe Point" )]
		[DefaultValue( PipePoint.SpecialtyEnum.None )]
		public Reference<PipePoint.SpecialtyEnum> PointSpecialty
		{
			get { if( _pointSpecialty.BeginGet() ) PointSpecialty = _pointSpecialty.Get( this ); return _pointSpecialty.value; }
			set { if( _pointSpecialty.BeginSet( ref value ) ) { try { PointSpecialtyChanged?.Invoke( this ); DataWasChanged(); } finally { _pointSpecialty.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PointSpecialty"/> property value changes.</summary>
		public event Action<Pipe> PointSpecialtyChanged;
		ReferenceField<PipePoint.SpecialtyEnum> _pointSpecialty = PipePoint.SpecialtyEnum.None;

		/// <summary>
		/// Replaces the geometry of the pipe by another material.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Calibrate Rendering" )]
		public Reference<Material> PipeReplaceMaterial
		{
			get { if( _pipeReplaceMaterial.BeginGet() ) PipeReplaceMaterial = _pipeReplaceMaterial.Get( this ); return _pipeReplaceMaterial.value; }
			set { if( _pipeReplaceMaterial.BeginSet( ref value ) ) { try { PipeReplaceMaterialChanged?.Invoke( this ); needUpdateVisualData = true; } finally { _pipeReplaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PipeReplaceMaterial"/> property value changes.</summary>
		public event Action<Pipe> PipeReplaceMaterialChanged;
		ReferenceField<Material> _pipeReplaceMaterial;

		/// <summary>
		/// The base color and opacity multiplier of the pipe.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Calibrate Rendering" )]
		public Reference<ColorValue> PipeColor
		{
			get { if( _pipeColor.BeginGet() ) PipeColor = _pipeColor.Get( this ); return _pipeColor.value; }
			set { if( _pipeColor.BeginSet( ref value ) ) { try { PipeColorChanged?.Invoke( this ); needUpdateVisualData = true; } finally { _pipeColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PipeColor"/> property value changes.</summary>
		public event Action<Pipe> PipeColorChanged;
		ReferenceField<ColorValue> _pipeColor = ColorValue.One;

		/// <summary>
		/// Replaces the geometry of the meshes by another material.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Calibrate Rendering" )]
		public Reference<Material> MeshesReplaceMaterial
		{
			get { if( _meshesReplaceMaterial.BeginGet() ) MeshesReplaceMaterial = _meshesReplaceMaterial.Get( this ); return _meshesReplaceMaterial.value; }
			set { if( _meshesReplaceMaterial.BeginSet( ref value ) ) { try { MeshesReplaceMaterialChanged?.Invoke( this ); needUpdateVisualData = true; } finally { _meshesReplaceMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MeshesReplaceMaterial"/> property value changes.</summary>
		public event Action<Pipe> MeshesReplaceMaterialChanged;
		ReferenceField<Material> _meshesReplaceMaterial;

		/// <summary>
		/// The base color and opacity multiplier of the meshes.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Calibrate Rendering" )]
		public Reference<ColorValue> MeshesColor
		{
			get { if( _meshesColor.BeginGet() ) MeshesColor = _meshesColor.Get( this ); return _meshesColor.value; }
			set { if( _meshesColor.BeginSet( ref value ) ) { try { MeshesColorChanged?.Invoke( this ); needUpdateVisualData = true; } finally { _meshesColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MeshesColor"/> property value changes.</summary>
		public event Action<Pipe> MeshesColorChanged;
		ReferenceField<ColorValue> _meshesColor = ColorValue.One;

		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorAutoUpdate { get; set; } = true;

		///////////////////////////////////////////////

		public class LogicalData
		{
			public Pipe Owner;
			public PipeType PipeType;
			public int PipeTypeVersion;
			//public int PipeTypeVersion2;

			public Point[] Points;
			public Curve PositionCurve;
			//public Curve Forward;
			//public Curve Up;
			public Curve ScaleCurve;
			//public double Length;
			//public Range TimeRange;

			public Dictionary<LogicalData, List<ConnectedPipeItem>> ConnectedPipes = new Dictionary<LogicalData, List<ConnectedPipeItem>>();

			(double time, Vector3 position)[] getCurveTimeByPositionCache;

			public List<RigidBody> CollisionBodiesPipe = new List<RigidBody>();
			public List<RigidBody> CollisionBodiesMeshes = new List<RigidBody>();

			/////////////////////

			public class Point
			{
				public ObjectInSpace ObjectInSpace;
				public Transform Transform;
				internal double _curveTimeOnlyToSort;
				public PipePoint.SpecialtyEnum Specialty;

				public double TimeOnCurve;
			}

			/////////////////////

			public class ConnectedPipeItem
			{
				public LogicalData ConnectedPipe;

				public double ThisPipe_TimeOnCurve;
				public Vector3 ThisPipe_Position;//only for optimization. can get by ThisPipe_TimeOnCurve

				public double ConnectedPipe_TimeOnCurve;
				public Vector3 ConnectedPipe_Position;//only for optimization. can get by ConnectedPipe_TimeOnCurve
			}

			internal void GenerateCurveData()
			{
				//generate Points

				var pointComponents = Owner.GetComponents<PipePoint>( onlyEnabledInHierarchy: true );

				Points = new Point[ 1 + pointComponents.Length ];
				Points[ 0 ] = new Point() { ObjectInSpace = Owner, Transform = Owner.Transform, _curveTimeOnlyToSort = Owner.Time, Specialty = Owner.PointSpecialty };
				for( int n = 0; n < pointComponents.Length; n++ )
				{
					var pointComponent = pointComponents[ n ];
					Points[ n + 1 ] = new Point() { ObjectInSpace = pointComponent, Transform = pointComponent.Transform, _curveTimeOnlyToSort = pointComponent.Time, Specialty = pointComponent.Specialty };
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

				PositionCurve = new CurveRoundedLine();
				//Forward = new CurveLine();
				//Up = new CurveLine();
				ScaleCurve = new CurveLine();

				for( int n = 0; n < Points.Length; n++ )
				{
					var point = Points[ n ];

					point.TimeOnCurve = n;

					var time = n;
					//var time = (double)n / (double)Points.Length;

					//double time = ( allPointsZeroTime ? n : point.Time ) * Owner.TimeScale;

					//object additionalData = null;
					//additionalData = CurvatureRadius.Value;
					//var rounded = data.positionCurve as CurveRoundedLine;
					//if( rounded != null )
					//	additionalData = RoundedLineCurvatureRadius.Value;
					PositionCurve.AddPoint( time, point.Transform.Position, PipeType.CurvatureRadius.Value );

					//transform.Rotation.GetForward( out var forward );
					//transform.Rotation.GetUp( out var up );
					//curveData.Forward.AddPoint( time, forward );
					//curveData.Up.AddPoint( time, up );

					ScaleCurve.AddPoint( time, point.Transform.Scale );
				}

				//calculate Length
				//if( Points.Length > 1 )
				//	curveData.TimeRange = new Range( PositionCurve.Points[ 0 ].Time, PositionCurve.Points[ Points.Length - 1 ].Time );
			}

			public bool GetCurveTimeByPosition( Vector3 position, double maxDistanceToCurve, out double timeOnCurve )
			{
				var step = 0.05;
				var maxTime = LastPoint.TimeOnCurve;
				var maxTime2 = maxTime + step;

				if( getCurveTimeByPositionCache == null )
				{
					var list = new List<(double, Vector3)>( (int)( maxTime2 / step ) + 2 );

					for( double time = 0; time <= maxTime2; time += step )
					{
						var time2 = time;
						if( time2 > maxTime )
							time2 = maxTime;

						var trPosition = GetPositionByTime( time2 );

						list.Add( (time2, trPosition) );
					}

					getCurveTimeByPositionCache = list.ToArray();
				}


				Vector3 previousPosition = Vector3.Zero;
				bool previousPositionHasValue = false;

				for( int nStep = 0; nStep < getCurveTimeByPositionCache.Length; nStep++ )
				{
					ref var pair = ref getCurveTimeByPositionCache[ nStep ];
					var time2 = pair.time;
					ref var trPosition = ref pair.position;

					if( previousPositionHasValue )
					{
						var b = new Bounds( previousPosition );
						b.Add( ref trPosition );

						var b2 = b;
						b2.Expand( maxDistanceToCurve );
						if( b2.Contains( ref position ) )
						{
							MathAlgorithms.ProjectPointToLine( ref previousPosition, ref trPosition, ref position, out var projected );
							var b3 = b;
							b3.Expand( 0.001 );
							if( b3.Contains( ref projected ) && ( projected - position ).LengthSquared() <= maxDistanceToCurve * maxDistanceToCurve )
							{
								timeOnCurve = time2;
								return true;
							}
						}
					}

					previousPosition = trPosition;
					previousPositionHasValue = true;
				}

				timeOnCurve = 0;
				return false;
			}

			internal void AddConnectedPipe( LogicalData data, ConnectedPipeItem item )
			{
				if( !ConnectedPipes.TryGetValue( data, out var list ) )
				{
					list = new List<ConnectedPipeItem>();
					ConnectedPipes[ data ] = list;
				}
				list.Add( item );
			}

			bool IsAnyTransformToolInModifyingMode()
			{
#if !DEPLOY
				foreach( var instance in EngineViewportControl.AllInstances )
				{
					var transformTool = instance.TransformTool as TransformTool;
					if( transformTool != null && transformTool.Modifying )
						return true;
				}
#endif
				return false;
			}

			internal void ConnectPipes( LogicalData connectOnlyToThisPipe )
			{
				if( Points.Length < 2 )
					return;

				if( IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				//connect points with other pipes
				//for( int nPoint = 0; nPoint < 2; nPoint++ )
				for( int nPoint = 0; nPoint < Points.Length; nPoint++ )
				{
					var point = Points[ nPoint ];
					//var point = Points[ nPoint == 0 ? 0 : Points.Length - 1 ];
					var pointPosition = point.Transform.Position;

					var scene = Owner.ParentScene;

					//!!!!
					double maxDistanceToCurve = 0.05;

					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, new Bounds( pointPosition ) );
					scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var pipe = item.Object as Pipe;
						if( pipe != null && pipe != Owner )
						{
							if( connectOnlyToThisPipe != null && connectOnlyToThisPipe.Owner != pipe )
								continue;

							var data = pipe.GetLogicalData();
							if( data != null && data.GetCurveTimeByPosition( pointPosition, maxDistanceToCurve, out var timeOnCurve ) )
							{
								var positionOnCurve = data.GetPositionByTime( timeOnCurve );

								//add to this pipe
								{
									var connectedPipeItem = new ConnectedPipeItem();
									connectedPipeItem.ConnectedPipe = data;

									connectedPipeItem.ThisPipe_TimeOnCurve = point.TimeOnCurve;
									connectedPipeItem.ThisPipe_Position = pointPosition;

									connectedPipeItem.ConnectedPipe_TimeOnCurve = timeOnCurve;
									connectedPipeItem.ConnectedPipe_Position = positionOnCurve;

									AddConnectedPipe( data, connectedPipeItem );
								}

								//add to connected pipe
								{
									var connectedPipeItem = new ConnectedPipeItem();
									connectedPipeItem.ConnectedPipe = this;

									connectedPipeItem.ThisPipe_TimeOnCurve = timeOnCurve;
									connectedPipeItem.ThisPipe_Position = positionOnCurve;

									connectedPipeItem.ConnectedPipe_TimeOnCurve = point.TimeOnCurve;
									connectedPipeItem.ConnectedPipe_Position = pointPosition;

									data.AddConnectedPipe( this, connectedPipeItem );
								}

								//need update visual data of the connected pipe
								data.Owner.DeleteVisualData();
							}
						}
					}

				}
			}

			internal void DisconnectPipes()
			{
				if( ConnectedPipes.Count != 0 )
				{
					//remove this pipe from connected pipes
					foreach( var connectedPipe in ConnectedPipes.Keys )
					{
						if( connectedPipe.ConnectedPipes.Remove( this ) )
						{
							//reset visual data
							connectedPipe.Owner.DeleteVisualData();
						}
					}

					ConnectedPipes.Clear();

					//reset visual data
					Owner.DeleteVisualData();
				}
			}

			//public Transform GetTransformByTime( double time )
			//{
			//	//!!!!
			//	var step = 0.01;

			//	var time2 = time;
			//	var maxTime2 = (double)Points.Length - step;
			//	if( time2 > maxTime2 )
			//		time2 = maxTime2;

			//	var pos = PositionCurve.CalculateValueByTime( time2 );

			//	//!!!!чтобы не было резкого выкручивания

			//	var pos2 = PositionCurve.CalculateValueByTime( time2 + step );

			//	//!!!!баг

			//	var rot = Quaternion.FromDirectionZAxisUp( ( pos2 - pos ).GetNormalize() );

			//	//!!!!
			//	if( double.IsNaN( rot.X ) )
			//		rot = Quaternion.Identity;


			//	//Vector3 forward = Forward.CalculateValueByTime( time );
			//	//Vector3 up = Up.CalculateValueByTime( time );

			//	//forward.Normalize();
			//	//var left = Vector3.Cross( up, forward );
			//	//left.Normalize();
			//	//up = Vector3.Cross( forward, left );
			//	//up.Normalize();

			//	//var mat = new Matrix3( forward, left, up );
			//	//mat.ToQuaternion( out rot );
			//	//rot.Normalize();

			//	var scl = ScaleCurve.CalculateValueByTime( time2 );

			//	return new Transform( pos, rot, scl );
			//}

			public Vector3 GetPositionByTime( double time )
			{
				return PositionCurve.CalculateValueByTime( time );
			}

			public Vector3 GetScaleByTime( double time )
			{
				return ScaleCurve.CalculateValueByTime( time );
			}

			public Vector3 GetDirectionByTime( double time )
			{
				//!!!!0.01

				Vector3 dir;

				if( time < 0.01 )
					dir = GetPositionByTime( 0.01 ) - GetPositionByTime( 0 );
				else if( time > LastPoint.TimeOnCurve - 0.01 )
					dir = GetPositionByTime( LastPoint.TimeOnCurve ) - GetPositionByTime( LastPoint.TimeOnCurve - 0.01 );
				else
					dir = GetPositionByTime( time + 0.01 ) - GetPositionByTime( time - 0.01 );

				return dir.GetNormalize();
			}

			public Point LastPoint
			{
				get { return Points[ Points.Length - 1 ]; }
			}

			public List<(Mesh mesh, Transform transform)> GetMeshesToCreate()
			{
				var result = new List<(Mesh mesh, Transform transform)>();

				var elbow90 = PipeType.Elbow90.Value;
				//var elbow135 = pipeType.Elbow135.Value;
				if( elbow90 != null )//|| elbow135 != null )
				{
					for( int n = 1; n < Points.Length - 1; n++ )
					{
						var p1 = Points[ n - 1 ].Transform.Position;
						var p2 = Points[ n ].Transform.Position;
						var p3 = Points[ n + 1 ].Transform.Position;

						var dir1 = ( p1 - p2 ).GetNormalize();
						var dir2 = ( p3 - p2 ).GetNormalize();

						if( elbow90 != null && Math.Abs( MathAlgorithms.GetVectorsAngle( dir1, dir2 ).InDegrees() - 90 ) < 3 )
						{
							var rot = Quaternion.LookAt( dir2, dir2.Cross( dir1 ) );
							var tr = new Transform( p2, rot, Points[ n ].Transform.Scale );
							result.Add( (elbow90, tr) );
						}

						//if( elbow135 != null && Math.Abs( MathAlgorithms.GetVectorsAngle( p1 - p2, p3 - p2 ).InDegrees() - 135 ) < 3 )
						//	CreateMeshObject( elbow135, new Transform( p2 ), true );
					}
				}

				//var tee45 = pipeType.Tee45.Value;
				var tee90 = PipeType.Tee90.Value;
				if( /*tee45 != null ||*/ tee90 != null )
				{
					foreach( var itemsList in ConnectedPipes.Values )
					{
						foreach( var item in itemsList )
						{
							if( item.ThisPipe_TimeOnCurve == 0 || item.ThisPipe_TimeOnCurve == LastPoint.TimeOnCurve )
							{
								//!!!!tee45

								var thisDir = GetDirectionByTime( item.ThisPipe_TimeOnCurve );
								if( item.ThisPipe_TimeOnCurve == 0 )
									thisDir = -thisDir;

								var connectedDir = item.ConnectedPipe.GetDirectionByTime( item.ConnectedPipe_TimeOnCurve );

								if( tee90 != null && Math.Abs( MathAlgorithms.GetVectorsAngle( thisDir, connectedDir ).InDegrees() - 90 ) < 3 )
								{
									var rot = Quaternion.LookAt( -thisDir, -thisDir.Cross( connectedDir ) );
									var scl = GetScaleByTime( item.ThisPipe_TimeOnCurve );
									var tr = new Transform( item.ThisPipe_Position, rot, scl );
									result.Add( (tee90, tr) );
								}
							}
						}
					}
				}

				var cross90 = PipeType.Cross.Value;
				if( cross90 != null )
				{
					foreach( var itemsList in ConnectedPipes.Values )
					{
						foreach( var item in itemsList )
						{
							if( item.ThisPipe_TimeOnCurve > 0 && item.ThisPipe_TimeOnCurve < LastPoint.TimeOnCurve && ( item.ThisPipe_TimeOnCurve % 1 ) == 0 )
							{
								var thisDir = GetDirectionByTime( item.ThisPipe_TimeOnCurve );
								//if( item.ThisPipe_TimeOnCurve == 0 )
								//	thisDir = -thisDir;

								var connectedDir = item.ConnectedPipe.GetDirectionByTime( item.ConnectedPipe_TimeOnCurve );

								if( cross90 != null && Math.Abs( MathAlgorithms.GetVectorsAngle( thisDir, connectedDir ).InDegrees() - 90 ) < 3 )
								{
									var rot = Quaternion.LookAt( -thisDir, -thisDir.Cross( connectedDir ) );
									var scl = GetScaleByTime( item.ThisPipe_TimeOnCurve );
									var tr = new Transform( item.ThisPipe_Position, rot, scl );
									result.Add( (cross90, tr) );
								}
							}
						}
					}
				}

				for( int n = 0; n < Points.Length; n++ )
				{
					var point = Points[ n ];
					if( point.Specialty != PipePoint.SpecialtyEnum.None )
					{
						Mesh mesh = null;

						switch( point.Specialty )
						{
						case PipePoint.SpecialtyEnum.OpenHole: mesh = PipeType.OpenHole.Value; break;
						case PipePoint.SpecialtyEnum.Cap: mesh = PipeType.Cap.Value; break;
						case PipePoint.SpecialtyEnum.Socket: mesh = PipeType.Socket.Value; break;
						case PipePoint.SpecialtyEnum.Holder: mesh = PipeType.Holder.Value; break;
						}

						if( mesh != null )
						{
							var direction = GetDirectionByTime( point.TimeOnCurve );
							if( n == 0 )
								direction = -direction;

							var up = point.Transform.Rotation.GetUp();

							Quaternion rot;
							if( !up.Equals( direction, .001 ) )
								rot = Quaternion.LookAt( direction, up );
							else
								rot = Quaternion.FromDirectionZAxisUp( direction );

							var tr = new Transform( point.Transform.Position, rot, point.Transform.Scale );
							result.Add( (mesh, tr) );
						}
					}
				}

				return result;
			}

			public void UpdateCollisionBodies()
			{
				DestroyCollisionBodies();

				if( !Owner.PipeCollision.Value )
					return;
				if( IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				//!!!!use BodyItem

				//pipe
				{
					//need set ShowInEditor = false before AddComponent
					var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
					body.DisplayInEditor = false;
					Owner.AddComponent( body, -1 );
					//var group = scene.CreateComponent<GroupOfObjects>();

					//CollisionBody.Name = "__Collision Body";
					body.SaveSupport = false;
					body.CloneSupport = false;
					body.CanBeSelected = false;
					body.SpaceBoundsUpdateEvent += CollisionBody_SpaceBoundsUpdateEvent;

					var ownerPosition = Owner.TransformV.Position;
					body.Transform = new Transform( ownerPosition );

					var shape = body.CreateComponent<CollisionShape_Mesh>();
					shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.TriangleMesh;
					shape.CheckValidData = false;
					shape.MergeEqualVerticesRemoveInvalidTriangles = false;

					Owner.GeneratePipeMeshData( null, shape );

					UpdateCollisionMaterial();

					body.Enabled = true;

					CollisionBodiesPipe.Add( body );
				}

				//meshes
				foreach( var item in GetMeshesToCreate() )
				{
					var mesh = item.mesh;
					var tr = item.transform;


					//!!!!use BodyItem


					var collisionDefinition = mesh.GetComponent( "Collision Definition" ) as RigidBody;
					if( collisionDefinition != null )
					{
						var body = (RigidBody)collisionDefinition.Clone();
						body.Enabled = false;

						////need set ShowInEditor = false before AddComponent
						//var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
						body.DisplayInEditor = false;
						Owner.AddComponent( body, -1 );
						//var group = scene.CreateComponent<GroupOfObjects>();

						//CollisionBody.Name = "__Collision Body";
						body.SaveSupport = false;
						body.CloneSupport = false;
						body.CanBeSelected = false;
						body.Transform = tr;

						//!!!!
						//shape.CheckValidData = false;
						//shape.MergeEqualVerticesRemoveInvalidTriangles = false;

						body.Enabled = true;

						CollisionBodiesMeshes.Add( body );
					}
				}
			}

			public void UpdateCollisionMaterial()
			{
				foreach( var body in CollisionBodiesPipe )
				{
					body.Material = PipeType.CollisionMaterial;
					//!!!!
					//body.MaterialFrictionMode = PipeType.CollisionFrictionMode;
					body.MaterialFriction = PipeType.CollisionFriction;
					//!!!!
					//body.MaterialAnisotropicFriction = PipeType.CollisionAnisotropicFriction;
					//body.MaterialSpinningFriction = PipeType.CollisionSpinningFriction;
					//body.MaterialRollingFriction = PipeType.CollisionRollingFriction;
					body.MaterialRestitution = PipeType.CollisionRestitution;
				}
			}

			private void CollisionBody_SpaceBoundsUpdateEvent( ObjectInSpace obj, ref SpaceBounds newBounds )
			{
				newBounds = Owner.SpaceBounds;
			}

			public void DestroyCollisionBodies()
			{
				foreach( var body in CollisionBodiesPipe )
				{
					body.SpaceBoundsUpdateEvent -= CollisionBody_SpaceBoundsUpdateEvent;
					body.Dispose();
				}
				CollisionBodiesPipe.Clear();

				foreach( var body in CollisionBodiesMeshes )
					body.Dispose();
				CollisionBodiesMeshes.Clear();
			}
		}

		///////////////////////////////////////////////

		public class VisualData
		{
			public List<int> groupOfObjectObjects = new List<int>();
			public List<Mesh> meshesToDispose = new List<Mesh>();
		}

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<PipePoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 5, 0, 0 ), Quaternion.Identity );
		}

		//!!!!в Fence, Road
		public override void NewObjectSetDefaultConfigurationUpdate()
		{
			var point = GetComponent<PipePoint>();
			if( point != null )
				point.Transform = new Transform( Transform.Value.Position + new Vector3( 5, 0, 0 ), Quaternion.Identity );
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
				case nameof( DisplayCurveInEditor ):
				case nameof( DisplayCurveInSimulation ):
				case nameof( Thickness ):
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
				logicalData.PipeType = PipeType;
				if( logicalData.PipeType == null )
					logicalData.PipeType = new PipeType();

				logicalData.PipeTypeVersion = logicalData.PipeType.Version;

				//var componentType = logicalData.PipeType.BaseType as Metadata.ComponentTypeInfo;
				//if( componentType != null )
				//{
				//	var pipeObject = componentType.BasedOnObject as Pipe;
				//	if( pipeObject != null )
				//		logicalData.PipeTypeVersion2 = pipeObject.Version;
				//}

				logicalData.GenerateCurveData();

				logicalData.ConnectPipes( null );

				//inform another pipes in the bounds to connect this pipe
				var scene = ParentScene;
				if( scene != null )
				{
					var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, SpaceBounds.BoundingBox );
					scene.GetObjectsInSpace( getObjectsItem );

					foreach( var item in getObjectsItem.Result )
					{
						var pipe = item.Object as Pipe;
						if( pipe != null && pipe != this )
						{
							var data = pipe.GetLogicalData( false );
							if( data != null )
								data.ConnectPipes( logicalData );
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
				logicalData.DisconnectPipes();
				logicalData = null;
			}
		}

		void GeneratePipeMeshData( Mesh mesh, CollisionShape_Mesh collisionShape )
		{
			var pipeType = logicalData.PipeType;
			var ownerPosition = TransformV.Position;
			var uvTilesLength = pipeType.UVTilesLength.Value;
			var uvTilesCircle = pipeType.UVTilesCircle.Value;
			var uvFlip = pipeType.UVFlip.Value;
			var outsideDiameter = pipeType.OutsideDiameter.Value;
			var points = logicalData.Points;
			var segmentsLength = pipeType.SegmentsLength.Value;
			if( segmentsLength <= 0.001 )
				segmentsLength = 0.001;
			var segmentsCircle = pipeType.SegmentsCircle.Value;
			if( segmentsCircle <= 3 )
				segmentsCircle = 3;

			var circleSteps = segmentsCircle + 1;

			double totalLengthNoCurvature = 0;
			{
				for( int n = 1; n < points.Length; n++ )
					totalLengthNoCurvature += ( points[ n ].Transform.Position - points[ n - 1 ].Transform.Position ).Length();
				if( totalLengthNoCurvature <= 0.001 )
					totalLengthNoCurvature = 0.001;
			}

			//generate time steps
			List<double> timeSteps;
			{
				var stepDivide = 2.0;

				timeSteps = new List<double>( (int)( totalLengthNoCurvature / segmentsLength * stepDivide * 2 ) );

				for( int nPoint = 0; nPoint < points.Length - 1; nPoint++ )
				{
					var pointFrom = points[ nPoint ];
					var pointTo = points[ nPoint + 1 ];
					var timeFrom = (double)nPoint;
					var timeTo = (double)nPoint + 1;

					var length = ( pointTo.Transform.Position - pointFrom.Transform.Position ).Length();
					var timeStep = segmentsLength / length / stepDivide;

					var timeEnd = timeTo - timeStep * 0.1;
					for( double time = timeFrom; time < timeEnd; time += timeStep )
						timeSteps.Add( time );
				}
				timeSteps.Add( points.Length - 1 );
			}

			//detect skipped steps
			var skipSteps = new bool[ timeSteps.Count ];
			{
				double currentLength = 0;
				var previousPosition = logicalData.GetPositionByTime( 0 );
				var previousVector = ( logicalData.GetPositionByTime( 0.01 ) - previousPosition ).GetNormalize();
				var currentUp = points[ 0 ].Transform.Rotation.GetUp();
				if( currentUp.Equals( previousVector, 0.01 ) )
					currentUp = points[ 0 ].Transform.Rotation.GetLeft();
				Vector3 previousUsedScale = points[ 0 ].Transform.Scale;

				double lengthRemainder = 0;
				var previousUsedVector = previousVector;


				for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
				{
					var time = timeSteps[ nTime ];
					var firstStep = nTime == 0;
					var lastStep = nTime == timeSteps.Count - 1;

					var position = logicalData.GetPositionByTime( time );

					var vector = position - previousPosition;
					double stepLength = 0;
					if( vector != Vector3.Zero )
						stepLength = vector.Normalize();
					else
						vector = previousVector;

					if( currentUp.Equals( vector, 0.01 ) )
						currentUp = Quaternion.FromDirectionZAxisUp( vector ).GetUp();
					var rotation = Quaternion.LookAt( vector, currentUp );

					var scale = logicalData.GetScaleByTime( time );

					var skip = false;
					if( !firstStep && !lastStep )
					{
						if( lengthRemainder + stepLength < segmentsLength )
							skip = true;
						else
						{
							//!!!!нужно проверять scale vector, т.е. искривление поверхности. не сам scale

							//threshold 0.25, 0.01
							if( MathAlgorithms.GetVectorsAngle( vector, previousUsedVector ).InDegrees() < 0.25 && scale.Equals( previousUsedScale, 0.01 ) )
								skip = true;
							else
							{
								//don't skip previous step
								skipSteps[ nTime - 1 ] = false;
							}
						}
					}

					if( !skip )
						lengthRemainder = 0;
					else
						lengthRemainder += stepLength;

					currentLength += stepLength;
					previousPosition = position;
					previousVector = vector;
					if( !skip )
						previousUsedVector = vector;
					//!!!!может обновлять только когда юзается. или как-то по другому реже
					currentUp = rotation.GetUp();
					if( !skip )
						previousUsedScale = scale;

					skipSteps[ nTime ] = skip;
				}
			}

			//start calculation
			var actualLengthSteps = skipSteps.Count( v => false );
			var vertexCount = actualLengthSteps * circleSteps;

			//!!!!можно на нативных массивах, тогда обнулять не нужно. индексы тоже. только тогда точно с запасом массив выделять
			var positions = new List<Vector3F>( vertexCount );
			var normals = new List<Vector3F>( vertexCount );
			var tangents = new List<Vector4F>( vertexCount );
			var texCoords = new List<Vector2F>( vertexCount );

			//int currentVertex = 0;
			int lengthStepsAdded = 0;

			{
				double currentLength = 0;
				var previousPosition = logicalData.GetPositionByTime( 0 );
				var previousVector = ( logicalData.GetPositionByTime( 0.01 ) - previousPosition ).GetNormalize();
				var currentUp = points[ 0 ].Transform.Rotation.GetUp();
				if( currentUp.Equals( previousVector, 0.01 ) )
					currentUp = points[ 0 ].Transform.Rotation.GetLeft();

				double lengthRemainder = 0;
				var previousUsedVector = previousVector;


				for( int nTime = 0; nTime < timeSteps.Count; nTime++ )
				{
					var time = timeSteps[ nTime ];
					var firstStep = nTime == 0;
					var lastStep = nTime == timeSteps.Count - 1;

					var position = logicalData.GetPositionByTime( time );

					var vector = position - previousPosition;
					double stepLength = 0;
					if( vector != Vector3.Zero )
						stepLength = vector.Normalize();
					else
						vector = previousVector;

					var rotation = Quaternion.LookAt( vector, currentUp );

					var scale = logicalData.GetScaleByTime( time );

					var skip = skipSteps[ nTime ];

					if( !skip )
					{
						for( int circleStep = 0; circleStep < circleSteps; circleStep++ )
						{
							var circleFactor = (double)circleStep / (double)( circleSteps - 1 );

							var p = new Vector2(
								Math.Cos( Math.PI * 2 * circleFactor ),
								Math.Sin( Math.PI * 2 * circleFactor ) );

							var offsetNotScaled = rotation * new Vector3( 0, p.X, p.Y );
							var offset = rotation * new Vector3( 0, p.X * scale.X * outsideDiameter * 0.5, p.Y * scale.Y * outsideDiameter * 0.5 );

							var pos = position - ownerPosition + offset;

							positions.Add( pos.ToVector3F() );

							var normal = offsetNotScaled.GetNormalize();
							var tangent = rotation.GetForward().Cross( normal );

							normals.Add( normal.ToVector3F() );
							tangents.Add( new Vector4F( tangent.ToVector3F(), -1 ) );

							var uv = new Vector2( currentLength * uvTilesLength, circleFactor * uvTilesCircle );
							if( uvFlip )
								uv = new Vector2( uv.Y, uv.X );
							texCoords.Add( uv.ToVector2F() );

							//currentVertex++;
						}

						lengthStepsAdded++;
					}


					if( !skip )
						lengthRemainder = 0;
					else
						lengthRemainder += stepLength;

					currentLength += stepLength;
					previousPosition = position;
					previousVector = vector;
					if( !skip )
						previousUsedVector = vector;
					//!!!!может обновлять только когда юзается. или как-то по другому реже
					currentUp = rotation.GetUp();
				}
			}

			//if( currentVertex != positions.Count )
			//	Log.Fatal( "PipeInSpace: GenerateMeshData: currentVertex != positions.Count." );

			//indices
			var lengthSegmentsAdded = lengthStepsAdded - 1;
			var indexCount = lengthSegmentsAdded * segmentsCircle * 6;
			var indices = new int[ indexCount ];
			int currentIndex = 0;

			for( int lengthStep = 0; lengthStep < lengthSegmentsAdded; lengthStep++ )
			{
				for( int circleStep = 0; circleStep < segmentsCircle; circleStep++ )
				{
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep + 1;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * ( lengthStep + 1 ) + circleStep;
					indices[ currentIndex++ ] = ( segmentsCircle + 1 ) * lengthStep + circleStep;
				}
			}

			if( currentIndex != indexCount )
				Log.Fatal( "Pipe: GenerateMeshData: currentIndex != indexCount." );
			foreach( var index in indices )
				if( index < 0 || index >= positions.Count )
					Log.Fatal( "Pipe: GenerateMeshData: index < 0 || index >= positions.Count." );

			//init objects
			if( collisionShape != null )
			{
				collisionShape.Vertices = positions.ToArray();
				collisionShape.Indices = indices;
			}
			else
			{
				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var vertices = new byte[ vertexSize * positions.Count ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

						for( int n = 0; n < positions.Count; n++ )
						{
							pVertex->Position = positions[ n ];
							pVertex->Normal = normals[ n ];
							pVertex->Tangent = tangents[ n ];
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = texCoords[ n ];

							pVertex++;
						}
					}
				}

				//create mesh geometry
				if( vertexStructure.Length != 0 && vertices.Length != 0 && indices.Length != 0 )
				{
					var meshGeometry = mesh.CreateComponent<MeshGeometry>();
					meshGeometry.Name = "Mesh Geometry";
					meshGeometry.VertexStructure = vertexStructure;
					meshGeometry.Vertices = vertices;
					meshGeometry.Indices = indices;
					meshGeometry.Material = pipeType.Material.Value;
					//meshGeometry.Material = PipeReplaceMaterial.Value != null ? PipeReplaceMaterial.Value : pipeType.Material.Value;
					//meshGeometry.Material = PipeReplaceMaterial.ReferenceSpecified ? PipeReplaceMaterial : pipeType.Material;
				}
			}
		}

		GroupOfObjects GetOrCreateGroupOfObjects( bool canCreate )
		{
			var scene = ParentScene;
			if( scene == null )
				return null;

			var name = "__GroupOfObjectsPipes";

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
			//группы объектов нельзя использовать для геометрии пайпа, потому что мешей много. они придуманы для множества объектов одного типа

			var key = (mesh, replaceMaterial);

			var dictionary = (Dictionary<(Mesh, Material), int>)group.AnyData;

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
				element.ReplaceMaterial = replaceMaterial;
				element.Mesh = mesh;
				element.AutoAlign = false;
				element.Enabled = true;

				dictionary[ key ] = element.Index;

				group.ElementTypesCacheNeedUpdate();
			}

			return (ushort)element.Index.Value;
		}

		unsafe void CreateMeshObject( Mesh mesh, Transform transform, bool useGroupOfObjects, bool isPipe )
		{
			var replaceMaterial = isPipe ? PipeReplaceMaterial.Value : MeshesReplaceMaterial.Value;
			var color = isPipe ? PipeColor : MeshesColor;

			if( useGroupOfObjects )
			{
				var group = GetOrCreateGroupOfObjects( true );
				if( group != null )
				{
					var elementIndex = GetOrCreateGroupOfObjectsElement( group, mesh, replaceMaterial );
					var pos = transform.Position;
					var rot = transform.Rotation.ToQuaternionF();
					var scl = transform.Scale.ToVector3F();

					var obj = new GroupOfObjects.Object( elementIndex, 0, 0, GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible, pos, rot, scl, Vector4F.Zero, color, Vector4F.Zero, Vector4F.Zero, 0 );

					//!!!!может обновление сразу вызывается, тогда не круто
					var objects = group.ObjectsAdd( &obj, 1 );
					visualData.groupOfObjectObjects.AddRange( objects );
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
				meshInSpace.ReplaceMaterial = replaceMaterial;
				meshInSpace.Color = color;

				meshInSpace.Mesh = mesh;// ReferenceUtility.MakeThisReference( meshInSpace, mesh );
				meshInSpace.Enabled = true;
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

					var pipeType = logicalData.PipeType;
					var points = logicalData.Points;

					//pipe geometry
					if( points.Length > 1 )
					{
						//!!!!не будет ли много жрат или тормозить когда каждый меш отдельной иерархией?

						//var mesh = CreateComponent<Mesh>( enabled: false );
						//mesh.Name = "Mesh";
						var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );

						GeneratePipeMeshData( mesh, null );
						mesh.VisibilityDistanceFactor = pipeType.VisibilityDistanceFactor;
						mesh.Enabled = true;

						visualData.meshesToDispose.Add( mesh );

						CreateMeshObject( mesh, new Transform( TransformV.Position ), false, true );
					}

					if( points.Length > 1 )
					{
						foreach( var item in logicalData.GetMeshesToCreate() )
						{
							var mesh = item.mesh;
							var tr = item.transform;
							CreateMeshObject( mesh, tr, true, false );
						}
					}
				}

				needUpdateVisualData = false;
			}

			return visualData;
		}

		void DeleteVisualData()
		{
			if( visualData != null )
			{
				//if( useGroupOfObjects )
				//{
				var group = GetOrCreateGroupOfObjects( false );
				if( group != null )
				{
					//!!!!может обновление сразу вызывается, тогда не круто
					group.ObjectsRemove( visualData.groupOfObjectObjects.ToArray() );
				}
				//}
				//else
				//{
				foreach( var c in GetComponents<MeshInSpace>() )
					c.Dispose();
				//}

				foreach( var mesh in visualData.meshesToDispose )
					mesh.Dispose();

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
				bounds.Expand( data.PipeType.OutsideDiameter * 0.5 );
				newBounds = new SpaceBounds( bounds );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EditorAutoUpdate && EnabledInHierarchyAndIsInstance )
			{
				if( logicalData != null )
				{
					if( logicalData.PipeType.Version != logicalData.PipeTypeVersion )
					{
						needUpdate = true;
					}

					//var componentType = logicalData.PipeType.BaseType as Metadata.ComponentTypeInfo;
					//if( componentType != null )
					//{
					//	var pipeObject = componentType.BasedOnObject as Pipe;
					//	if( pipeObject != null )
					//	{
					//		if( logicalData.PipeTypeVersion2 != pipeObject.Version )
					//			needUpdate = true;
					//	}
					//}
				}

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

			if( needUpdateVisualData )
				DeleteVisualData();

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

		protected override bool GetAllowUpdateData()
		{
			return false;
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
