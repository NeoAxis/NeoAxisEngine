// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The event trigger in the scene. The sensor allows you to select scene objects by a given volume or ray.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Sensors\Sensor", 10 )]
	[AddToResourcesWindow( @"Base\Physics\Sensor", 0 )]
	public class Sensor : ObjectInSpace
	{
		//!!!!optional batching support for sensors
		//!!!!SubscribeToDeletionEvent( obj ); вручную вызывать Update кому надо?

		Matrix4 hullFrom;
		Matrix4 hullTo;
		ShapeEnum hullShape;
		Vector3[] hullVertices;
		int[] hullIndices;
		Plane[] hullPlanes;

		/////////////////////////////////////////

		public enum SourceDataEnum
		{
			ObjectsInSpace,
			PhysicsObjects,
		}

		/// <summary>
		/// What kind of data is used as a source of events.
		/// </summary>
		[DefaultValue( SourceDataEnum.ObjectsInSpace )]
		public Reference<SourceDataEnum> SourceData
		{
			get { if( _sourceData.BeginGet() ) SourceData = _sourceData.Get( this ); return _sourceData.value; }
			set { if( _sourceData.BeginSet( ref value ) ) { try { SourceDataChanged?.Invoke( this ); } finally { _sourceData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SourceData"/> property value changes.</summary>
		public event Action<Sensor> SourceDataChanged;
		ReferenceField<SourceDataEnum> _sourceData = SourceDataEnum.ObjectsInSpace;

		public enum ShapeEnum
		{
			Box,
			Sphere,
			Ray,
			//Spline и другие
			//Mesh, ConvexMesh
		}

		/// <summary>
		/// The shape of the trigger.
		/// </summary>
		[DefaultValue( ShapeEnum.Box )]
		public Reference<ShapeEnum> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set { if( _shape.BeginSet( ref value ) ) { try { ShapeChanged?.Invoke( this ); } finally { _shape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<Sensor> ShapeChanged;
		ReferenceField<ShapeEnum> _shape = ShapeEnum.Box;

		/// <summary>
		/// The target by which the sensor ray will trigger.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ObjectInSpace> RayTarget
		{
			get { if( _rayTarget.BeginGet() ) RayTarget = _rayTarget.Get( this ); return _rayTarget.value; }
			set { if( _rayTarget.BeginSet( ref value ) ) { try { RayTargetChanged?.Invoke( this ); } finally { _rayTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RayTarget"/> property value changes.</summary>
		public event Action<Sensor> RayTargetChanged;
		ReferenceField<ObjectInSpace> _rayTarget;

		/// <summary>
		/// The end point of the sensor in volume mode.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ObjectInSpace> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<Sensor> TargetChanged;
		ReferenceField<ObjectInSpace> _target;

		public enum ModeEnum
		{
			OneClosestObject,
			AllObjects,
		}

		/// <summary>
		/// The recognition mode of the sensor.
		/// </summary>
		[DefaultValue( ModeEnum.AllObjects )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Sensor> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.AllObjects;

		//!!!!list?
		/// <summary>
		/// The type of objects that sensor will recognize.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Metadata.TypeInfo> FilterByType
		{
			get { if( _filterByType.BeginGet() ) FilterByType = _filterByType.Get( this ); return _filterByType.value; }
			set { if( _filterByType.BeginSet( ref value ) ) { try { FilterByTypeChanged?.Invoke( this ); } finally { _filterByType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FilterByType"/> property value changes.</summary>
		public event Action<Sensor> FilterByTypeChanged;
		ReferenceField<Metadata.TypeInfo> _filterByType;

		/// <summary>
		/// The group of physical objects that sensor will recognize.
		/// </summary>
		[DefaultValue( 1 )]
		public Reference<int> PhysicsFilterGroup
		{
			get { if( _physicsFilterGroup.BeginGet() ) PhysicsFilterGroup = _physicsFilterGroup.Get( this ); return _physicsFilterGroup.value; }
			set { if( _physicsFilterGroup.BeginSet( ref value ) ) { try { PhysicsFilterGroupChanged?.Invoke( this ); } finally { _physicsFilterGroup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsFilterGroup"/> property value changes.</summary>
		public event Action<Sensor> PhysicsFilterGroupChanged;
		ReferenceField<int> _physicsFilterGroup = 1;

		/// <summary>
		/// The mask of physical objects that sensor will recognize.
		/// </summary>
		[DefaultValue( -1 )]
		public Reference<int> PhysicsFilterMask
		{
			get { if( _physicsFilterMask.BeginGet() ) PhysicsFilterMask = _physicsFilterMask.Get( this ); return _physicsFilterMask.value; }
			set { if( _physicsFilterMask.BeginSet( ref value ) ) { try { PhysicsFilterMaskChanged?.Invoke( this ); } finally { _physicsFilterMask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsFilterMask"/> property value changes.</summary>
		public event Action<Sensor> PhysicsFilterMaskChanged;
		ReferenceField<int> _physicsFilterMask = -1;

		/// <summary>
		/// Whether the sensor is ignoring other sensors.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> IgnoreSensors
		{
			get { if( _ignoreSensors.BeginGet() ) IgnoreSensors = _ignoreSensors.Get( this ); return _ignoreSensors.value; }
			set { if( _ignoreSensors.BeginSet( ref value ) ) { try { IgnoreSensorsChanged?.Invoke( this ); } finally { _ignoreSensors.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IgnoreSensors"/> property value changes.</summary>
		public event Action<Sensor> IgnoreSensorsChanged;
		ReferenceField<bool> _ignoreSensors = true;

		[Flags]
		public enum WhenUpdateEnum
		{
			Manual = 0,
			Enable = 1,
			Update = 2,
			SimulationStep = 4,
		}

		/// <summary>
		/// The update case mode of the sensor.
		/// </summary>
		[DefaultValue( /*WhenUpdateEnum.SimulationStep |*/ WhenUpdateEnum.Update )]
		public Reference<WhenUpdateEnum> WhenUpdate
		{
			get { if( _whenUpdate.BeginGet() ) WhenUpdate = _whenUpdate.Get( this ); return _whenUpdate.value; }
			set { if( _whenUpdate.BeginSet( ref value ) ) { try { WhenUpdateChanged?.Invoke( this ); } finally { _whenUpdate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WhenUpdate"/> property value changes.</summary>
		public event Action<Sensor> WhenUpdateChanged;
		ReferenceField<WhenUpdateEnum> _whenUpdate = /*WhenUpdateEnum.SimulationStep |*/ WhenUpdateEnum.Update;

		/// <summary>
		/// The display color of the sensor.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> DisplayColor
		{
			get { if( _displayColor.BeginGet() ) DisplayColor = _displayColor.Get( this ); return _displayColor.value; }
			set { if( _displayColor.BeginSet( ref value ) ) { try { DisplayColorChanged?.Invoke( this ); } finally { _displayColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayColor"/> property value changes.</summary>
		public event Action<Sensor> DisplayColorChanged;
		ReferenceField<ColorValue> _displayColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Whether to draw gizmos on the objects recognized by the sensor.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplayObjects
		{
			get { if( _displayObjects.BeginGet() ) DisplayObjects = _displayObjects.Get( this ); return _displayObjects.value; }
			set { if( _displayObjects.BeginSet( ref value ) ) { try { DisplayObjectsChanged?.Invoke( this ); } finally { _displayObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayObjects"/> property value changes.</summary>
		public event Action<Sensor> DisplayObjectsChanged;
		ReferenceField<bool> _displayObjects = false;

		/// <summary>
		/// The color of the recognized object gizmos.
		/// </summary>
		[DefaultValue( "1 0 0" )]
		public Reference<ColorValue> DisplayObjectsColor
		{
			get { if( _displayObjectsColor.BeginGet() ) DisplayObjectsColor = _displayObjectsColor.Get( this ); return _displayObjectsColor.value; }
			set { if( _displayObjectsColor.BeginSet( ref value ) ) { try { DisplayObjectsColorChanged?.Invoke( this ); } finally { _displayObjectsColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayObjectsColor"/> property value changes.</summary>
		public event Action<Sensor> DisplayObjectsColorChanged;
		ReferenceField<ColorValue> _displayObjectsColor = new ColorValue( 1, 0, 0 );

		/// <summary>
		/// List of objects recognized by the sensor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		public ReferenceList<ObjectInSpace> Objects
		{
			get { return _objects; }
		}
		public delegate void ObjectsChangedDelegate( Sensor sender );
		public event ObjectsChangedDelegate ObjectsChanged;
		ReferenceList<ObjectInSpace> _objects;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( RayTarget ):
					if( Shape.Value != ShapeEnum.Ray )
						skip = true;
					break;

				case nameof( PhysicsFilterGroup ):
				case nameof( PhysicsFilterMask ):
					if( SourceData.Value != SourceDataEnum.PhysicsObjects )
						skip = true;
					break;

				case nameof( IgnoreSensors ):
					if( SourceData.Value != SourceDataEnum.ObjectsInSpace )
						skip = true;
					break;

				case nameof( Target ):
					if( !( Shape.Value == ShapeEnum.Box || Shape.Value == ShapeEnum.Sphere ) )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////

		public Sensor()
		{
			_objects = new ReferenceList<ObjectInSpace>( this, () => ObjectsChanged?.Invoke( this ) );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				if( ( WhenUpdate.Value & WhenUpdateEnum.Enable ) != 0 )
					Update();
			}
			else
				Update();
		}

		public Box GetBox()
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			return new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		public Sphere GetSphere()
		{
			var tr = Transform.Value;
			return new Sphere( tr.Position, Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5 );
		}

		public Ray GetRay()
		{
			var tr = Transform.Value;

			var rayTarget = RayTarget;
			if( rayTarget.ReferenceSpecified )
			{
				var target = rayTarget.Value;
				if( target != null )
					return new Ray( tr.Position, target.Transform.Value.Position - tr.Position );
				else
					return new Ray( tr.Position, Vector3.Zero );
			}
			else
				return new Ray( tr.Position, tr.Rotation * new Vector3( tr.Scale.X, 0, 0 ) );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			//!!!!bounding sphere

			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				newBounds = new SpaceBounds( GetBox().ToBounds() );
				break;

			case ShapeEnum.Sphere:
				newBounds = new SpaceBounds( GetSphere() );
				break;

			case ShapeEnum.Ray:
				{
					var ray = GetRay();
					Bounds b = new Bounds( ray.Origin );
					b.Add( ray.GetEndPoint() );
					newBounds = new SpaceBounds( b );
				}
				break;
			}
		}

		protected virtual void RenderShape( RenderingContext context )
		{
			if( Target.ReferenceSpecified && ( Shape.Value == ShapeEnum.Box || Shape.Value == ShapeEnum.Sphere ) && context.viewport.Simple3DRenderer != null )
			{
				var target = Target.Value;
				if( target != null )
				{
					if( hullVertices != null )
						context.viewport.Simple3DRenderer.AddTriangles( hullVertices, hullIndices, true, false );

					//var from = Transform.Value.ToMat4();
					//var to = target.Transform.Value.ToMat4();

					//context.viewport.Simple3DRenderer.AddLine( from.GetTranslation(), to.GetTranslation() );

					//switch( Shape.Value )
					//{
					//case ShapeEnum.Box:
					//	context.viewport.Simple3DRenderer.AddBox( GetBox() );
					//	break;

					//case ShapeEnum.Sphere:
					//	context.viewport.Simple3DRenderer.AddSphere( GetSphere() );
					//	break;
					//}

					return;
				}
			}

			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				context.viewport.Simple3DRenderer.AddBox( GetBox() );
				break;

			case ShapeEnum.Sphere:
				context.viewport.Simple3DRenderer.AddSphere( GetSphere() );
				break;

			case ShapeEnum.Ray:
				{
					var ray = GetRay();
					context.viewport.Simple3DRenderer.AddLine( ray.Origin, ray.GetEndPoint() );
				}
				break;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( ( WhenUpdate.Value & WhenUpdateEnum.Update ) != 0 )
				Update();
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( ( WhenUpdate.Value & WhenUpdateEnum.SimulationStep ) != 0 )
				Update();
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//if( WhenUpdate.Value.HasFlag( WhenUpdateEnum.Render ) )
			//	Update();

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplaySensors ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					if( context2.displaySensorsCounter < context2.displaySensorsMax )
					{
						context2.displaySensorsCounter++;

						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						else
							color = DisplayColor.Value;

						if( color.Alpha != 0 )
						{
							context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							RenderShape( context2 );
						}
					}

					if( DisplayObjects )
					{
						var color = DisplayObjectsColor.Value;
						if( color.Alpha != 0 )
						{
							context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							//foreach( var refObject in Objects )
							//{
							//	var obj = refObject.Value;
							//	if( obj != null && obj.EnabledInHierarchy )
							//		context.viewport.Simple3DRenderer.AddBounds( obj.SpaceBounds.CalculatedBoundingBox );
							//}

							foreach( var item in CalculateObjects() )
								RenderItem( context.Owner.Simple3DRenderer, item );
						}
					}
				}
			}
		}

		void RenderItem( Simple3DRenderer renderer, ResultItem item )
		{
			//position
			//!!!!radius
			if( item.Position.HasValue )
				renderer.AddSphere( new Sphere( item.Position.Value, 0.05 ), 8 );

			////!!!!length
			//if( item.RayTestResult != null )
			//	renderer.AddArrow( item.RayTestResult.Position, item.RayTestResult.Position + item.RayTestResult.Normal );

			//bounds
			if( item.SceneObject != null )
				renderer.AddBounds( item.SceneObject.SpaceBounds.BoundingBox );
			else if( item.PhysicalBody != null )
			{
				item.PhysicalBody.GetBounds( out var bounds );
				renderer.AddBounds( bounds );
			}


			//!!!!render all data. triange data



			//if( item.Object != null && item.Object.EnabledInHierarchy )
			//{
			//	renderer.AddBounds( item.Object.SpaceBounds.CalculatedBoundingBox );

			////!!!!radius
			//if( item.Position.HasValue )
			//	renderer.AddSphere( new Sphere( item.Position.Value, 0.05 ), 8 );


			//!!!!need


			////getting triangle to draw
			//var triIdx = item.RayTestResult?.TriangleIndexProcessed ?? -1;
			////var tIdx = item.RayTestResult?.TriangleIndexSource ?? -1;

			//if( triIdx >= 0 )
			//{
			//	//!!!!тут не так будет. где еще

			//	if( item.RayTestResult?.Shape is CollisionShape_Mesh csMesh )
			//	{
			//		var t = item.Object.Transform.Value.ToMatrix4();
			//		var local = csMesh.TransformRelativeToParent.Value;
			//		if( !local.IsIdentity )
			//			t *= local.ToMatrix4();

			//		//!!!!need
			//		//if( csMesh.GetTriangleProcessedData( triIdx, true, out var triangle ) )
			//		//{
			//		//	var v1 = triangle.A;
			//		//	var v2 = triangle.B;
			//		//	var v3 = triangle.C;

			//		//	var thickness = Math.Max( ( v1 - v2 ).Length(), Math.Max( ( v2 - v3 ).Length(), ( v3 - v1 ).Length() ) ) * 0.02;
			//		//	renderer.AddLine( v1, v2, thickness );
			//		//	renderer.AddLine( v2, v3, thickness );
			//		//	renderer.AddLine( v3, v1, thickness );
			//		//}
			//	}
			//	else if( item.RayTestResult?.Body is SoftBody softBody )
			//	{
			//		var t = item.Object.Transform.Value.ToMatrix4();

			//		if( softBody.GetTriangleSimulatedData( triIdx, true, out var triangle ) )
			//		{
			//			var v1 = triangle.A;
			//			var v2 = triangle.B;
			//			var v3 = triangle.C;

			//			var thickness = Math.Max( ( v1 - v2 ).Length(), Math.Max( ( v2 - v3 ).Length(), ( v3 - v1 ).Length() ) ) * 0.02;
			//			renderer.AddLine( v1, v2, thickness );
			//			renderer.AddLine( v2, v3, thickness );
			//			renderer.AddLine( v3, v1, thickness );
			//		}
			//	}
			//}
			//}
		}

		/////////////////////////////////////////

		//!!!!later for optimization. but need save multithreading support
		//bool tempIgnoreSensors;
		//Metadata.TypeInfo tempFilterByType;

		protected virtual void OnFilterObject( ResultItem item, ref bool ignore )
		{
			if( IgnoreSensors && item.SceneObject != null && ( item.SceneObject == this || typeof( Sensor ).IsAssignableFrom( item.SceneObject.GetType() ) ) )
			{
				ignore = true;
				return;
			}
			var filterByType = FilterByType.Value;
			if( filterByType != null && item.SceneObject != null && !filterByType.IsAssignableFrom( item.SceneObject.BaseType ) )
				ignore = true;

			//filter PhysicalBody?
		}

		public delegate void FilterObjectEventDelegate( Sensor sensor, ResultItem item, ref bool ignore );
		public event FilterObjectEventDelegate FilterObjectEvent;

		public void PerformFilterObject( ResultItem item, ref bool ignore )
		{
			OnFilterObject( item, ref ignore );
			if( !ignore )
				FilterObjectEvent?.Invoke( this, item, ref ignore );
		}

		/// <summary>
		/// Represents an item for <see cref="CalculateObjects"/> method.
		/// </summary>
		public struct ResultItem
		{
			public ObjectInSpace SceneObject;
			public Scene.PhysicsWorldClass.Body PhysicalBody;

			//ray specific
			public Vector3? Position;
			public double? DistanceScale;

			//public PhysicsRayTestItem.ResultItem RayTestResult;
			//shape index?
			//triangle index?

			//

			public ResultItem( ObjectInSpace sceneObject, Scene.PhysicsWorldClass.Body physicalBody, Vector3? position = null, double? distanceScale = null/*, PhysicsRayTestItem.ResultItem testResult = null*/ )
			{
				SceneObject = sceneObject;
				PhysicalBody = physicalBody;
				Position = position;
				DistanceScale = distanceScale;
				//RayTestResult = testResult;
			}

			//public ResultItem( ObjectInSpace sceneObject, Scene.PhysicsWorldClass.Body physicalBody )
			//{
			//	SceneObject = sceneObject;
			//	PhysicalBody = physicalBody;
			//}

			public override string ToString()
			{
				if( SceneObject != null )
					return SceneObject.ToString();
				else if( PhysicalBody != null )
					return PhysicalBody.ToString();
				else
					return "(Null)";
			}
		}

		void UpdateSweepHullGeometry( Vector3[] shapeLocalVertices )
		{
			var target = Target.Value;

			var tr = Transform.Value;
			var from = tr.ToMatrix4( true, true, false );
			var to = target.Transform.Value.ToMatrix4( true, false, false );

			var hullPositions = new Vector3[ shapeLocalVertices.Length * 2 ];
			for( int i = 0; i < shapeLocalVertices.Length; i++ )
			{
				Matrix4.Multiply( ref from, ref shapeLocalVertices[ i ], out hullPositions[ i ] );
				Matrix4.Multiply( ref to, ref shapeLocalVertices[ i ], out hullPositions[ i + shapeLocalVertices.Length ] );
			}

			MathAlgorithms.ConvexHullFromMesh( hullPositions, out hullVertices, out hullIndices, out hullPlanes );

			//ConvexHullAlgorithm.Create( hullPositions, out hullVertices, out hullIndices );
			////Bullet's implementation makes additional margin
			////BulletUtils.GetHullVertices( hullPositions, out hullVertices, out hullIndices );

			//hullPlanes = new Plane[ hullIndices.Length / 3 ];

			//for( int i = 0; i < hullIndices.Length; i += 3 )
			//{
			//	var v0 = hullVertices[ hullIndices[ i ] ];
			//	var v1 = hullVertices[ hullIndices[ i + 1 ] ];
			//	var v2 = hullVertices[ hullIndices[ i + 2 ] ];

			//	hullPlanes[ i / 3 ] = Plane.FromPoints( v0, v1, v2 );
			//}
		}

		void UpdateConvexSweepHull()
		{
			var target = Target.Value;
			if( target != null )
			{
				var tr = Transform.Value;
				var from = tr.ToMatrix4();
				var to = target.Transform.Value.ToMatrix4();
				var shape = Shape.Value;

				var update = hullVertices == null || !hullFrom.Equals( from, .000001 ) || !hullTo.Equals( to, .000001 ) || hullShape != shape;
				if( update )
				{
					hullFrom = from;
					hullTo = to;
					hullShape = shape;

					if( shape == ShapeEnum.Box )
					{
						var boxLocalVertices = new Bounds( tr.Scale * -.5, tr.Scale * .5 ).ToPoints();
						UpdateSweepHullGeometry( boxLocalVertices );
					}
					else if( shape == ShapeEnum.Sphere )
					{
						var radius = Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5;
						//!!!!12
						SimpleMeshGenerator.GenerateSphere( radius, 12, 12, false, out Vector3[] sphereLocalVertices, out _ );
						UpdateSweepHullGeometry( sphereLocalVertices );
					}
				}
			}
			else
			{
				hullVertices = null;
				hullIndices = null;
				hullPlanes = null;
			}
		}

		ResultItem[] CalculateObjects_ObjectsInSpace()
		{
			var scene = ParentScene;
			if( scene != null )
			{
				Scene.GetObjectsInSpaceItem item = null;

				if( Target.ReferenceSpecified && ( Shape.Value == ShapeEnum.Box || Shape.Value == ShapeEnum.Sphere ) )
				{
					//Convex sweep test
					var target = Target.Value;
					if( target != null )
					{
						UpdateConvexSweepHull();

						if( hullPlanes != null )
							item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, hullPlanes );
					}
				}
				else
				{
					switch( Shape.Value )
					{
					case ShapeEnum.Box:
						item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, GetBox() );
						break;
					case ShapeEnum.Sphere:
						item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, GetSphere() );
						break;
					case ShapeEnum.Ray:
						item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, GetRay() );
						break;
					}
				}

				if( item != null )
				{
					scene.GetObjectsInSpace( item );

					if( item.Result.Length != 0 )
					{
						var result = new List<ResultItem>( item.Result.Length );
						for( int n = 0; n < item.Result.Length; n++ )
						{
							ref var resultItem2 = ref item.Result[ n ];

							ResultItem resultItem;
							if( Shape.Value == ShapeEnum.Ray )
								resultItem = new ResultItem( resultItem2.Object, null, resultItem2.Position, resultItem2.DistanceScale );
							else
								resultItem = new ResultItem( resultItem2.Object, null );

							bool ignore = false;
							PerformFilterObject( resultItem, ref ignore );
							if( ignore )
								continue;

							result.Add( resultItem );

							if( Mode.Value == ModeEnum.OneClosestObject )
								break;
						}

						return result.ToArray();
					}
				}
			}

			return Array.Empty<ResultItem>();
		}

		ResultItem[] CalculateObjects_PhysicsObjects()
		{
			var scene = ParentScene;
			if( scene != null )
			{
				switch( Shape.Value )
				{
				case ShapeEnum.Box:
				case ShapeEnum.Sphere:
					{
						var direction = Vector3.Zero;

						if( Target.ReferenceSpecified )
						{
							//Convex sweep test
							var target = Target.Value;
							if( target != null )
							{
								UpdateConvexSweepHull();

								var tr = TransformV;
								//var from = tr.ToMatrix4( true, true, false );
								//var to = target.Transform.Value.ToMatrix4( true, false, false );

								direction = target.TransformV.Position - tr.Position;
							}
						}

						//Volume test
						PhysicsVolumeTestItem item = null;
						switch( Shape.Value )
						{
						case ShapeEnum.Box:
							item = new PhysicsVolumeTestItem( GetBox(), direction, PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );
							break;
						case ShapeEnum.Sphere:
							item = new PhysicsVolumeTestItem( GetSphere(), direction, PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );
							break;
						}


						//!!!!
						//var c = new Capsule( TransformV.Position, TransformV.Position + direction, GetSphere().Radius );
						//item = new PhysicsVolumeTestItem( c, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );


						//!!!!цилиндр глючит
						//var c = new Cylinder( TransformV.Position, TransformV.Position + direction, GetSphere().Radius );
						//item = new PhysicsVolumeTestItem( c, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );



						if( item != null )
						{
							scene.PhysicsVolumeTest( item );

							if( item.Result.Length != 0 )
							{
								var result = new List<ResultItem>( item.Result.Length );
								foreach( var i in item.Result )
								{
									var resultItem = new ResultItem( i.Body.Owner as ObjectInSpace, i.Body );
									//var resultItem = new ResultItem( i.Body.RigidBodyOwner, i.Body, i.PositionWorldOnA, i.Distance );

									bool ignore = false;
									PerformFilterObject( resultItem, ref ignore );
									if( ignore )
										continue;

									result.Add( resultItem );

									if( Mode.Value == ModeEnum.OneClosestObject )
										break;
								}
								return result.ToArray();
							}
						}
					}
					break;

				//old Bullet
				//case ShapeEnum.Box:
				//case ShapeEnum.Sphere:
				//	{
				//		if( ConvexSweepTarget.ReferenceSpecified )
				//		{
				//			//Convex sweep test
				//			var target = ConvexSweepTarget.Value;
				//			if( target != null )
				//			{
				//				UpdateConvexSweepHull();

				//				var tr = Transform.Value;
				//				var from = tr.ToMatrix4( true, true, false );
				//				var to = target.Transform.Value.ToMatrix4( true, false, false );

				//				//!!!!impl

				//				//PhysicsConvexSweepTestItem item = null;
				//				//switch( Shape.Value )
				//				//{
				//				//case ShapeEnum.Box:
				//				//	item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Bounds( tr.Scale * -.5, tr.Scale * .5 ) );//GetBox()
				//				//	break;
				//				//case ShapeEnum.Sphere:
				//				//	item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Sphere( Vector3.Zero, Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5 ) );//GetSphere()
				//				//	break;
				//				//}
				//				//if( item != null )
				//				//{
				//				//	zzzz;
				//				//	scene.PhysicsConvexSweepTest( item );
				//				//	if( item.Result.Length != 0 )
				//				//	{
				//				//		var result = new List<ResultItem>( item.Result.Length );
				//				//		foreach( var i in item.Result )
				//				//		{
				//				//			//!!!!Distance good?
				//				//			var length = ( to.GetTranslation() - from.GetTranslation() ).Length();
				//				//			var resultItem = new ResultItem( i.Body.RigidBodyOwner, i.Body, i.Position, i.DistanceScale * length );
				//				//			//var resultItem = new ResultItem( i.Body, i.Position, i.Distance );

				//				//			bool ignore = false;
				//				//			PerformFilterObject( resultItem, ref ignore );
				//				//			if( ignore )
				//				//				continue;

				//				//			result.Add( resultItem );

				//				//			if( Mode.Value == ModeEnum.OneClosestObject )
				//				//				break;
				//				//		}
				//				//		return result.ToArray();
				//				//	}
				//				//}
				//			}
				//		}
				//		else
				//		{
				//			//Volume test
				//			PhysicsVolumeTestItem item = null;
				//			switch( Shape.Value )
				//			{
				//			case ShapeEnum.Box:
				//				item = new PhysicsVolumeTestItem( GetBox(), PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );
				//				break;
				//			case ShapeEnum.Sphere:
				//				item = new PhysicsVolumeTestItem( GetSphere(), PhysicsVolumeTestItem.ModeEnum.All/*OneClosestForEach*/, PhysicsFilterGroup, PhysicsFilterMask );
				//				break;
				//			}
				//			if( item != null )
				//			{
				//				scene.PhysicsVolumeTest( item );
				//				if( item.Result.Length != 0 )
				//				{
				//					var result = new List<ResultItem>( item.Result.Length );
				//					foreach( var i in item.Result )
				//					{
				//						var resultItem = new ResultItem( i.Body.RigidBodyOwner, i.Body );
				//						//var resultItem = new ResultItem( i.Body.RigidBodyOwner, i.Body, i.PositionWorldOnA, i.Distance );

				//						bool ignore = false;
				//						PerformFilterObject( resultItem, ref ignore );
				//						if( ignore )
				//							continue;

				//						result.Add( resultItem );

				//						if( Mode.Value == ModeEnum.OneClosestObject )
				//							break;
				//					}
				//					return result.ToArray();
				//				}
				//			}
				//		}
				//	}
				//	break;

				case ShapeEnum.Ray:
					{
						var item = new PhysicsRayTestItem( GetRay(), PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.CalculateNormal, PhysicsFilterGroup, PhysicsFilterMask );

						scene.PhysicsRayTest( item );
						if( item.Result.Length != 0 )
						{
							var result = new List<ResultItem>( item.Result.Length );
							for( int n = 0; n < item.Result.Length; n++ )//foreach( var i in item.Result )
							{
								ref var i = ref item.Result[ n ];

								var resultItem = new ResultItem( i.Body.Owner as ObjectInSpace, i.Body, i.Position, i.DistanceScale );//, i );

								bool ignore = false;
								PerformFilterObject( resultItem, ref ignore );
								if( ignore )
									continue;

								result.Add( resultItem );

								if( Mode.Value == ModeEnum.OneClosestObject )
									break;
							}
							return result.ToArray();
						}
					}
					break;
				}
			}

			return Array.Empty<ResultItem>();
		}

		protected virtual ResultItem[] OnCalculateObjects()
		{
			if( SourceData.Value == SourceDataEnum.ObjectsInSpace )
				return CalculateObjects_ObjectsInSpace();
			else
				return CalculateObjects_PhysicsObjects();
		}

		public delegate void CalculateObjectsEventDelegate( Sensor sender, ref bool handled, ref ResultItem[] result );
		public event CalculateObjectsEventDelegate CalculateObjectsEvent;

		public ResultItem[] CalculateObjects()
		{
			if( EnabledInHierarchyAndIsInstance )
			{
				bool handled = false;
				ResultItem[] result = null;
				CalculateObjectsEvent?.Invoke( this, ref handled, ref result );
				if( handled )
				{
					if( result == null )
						result = Array.Empty<ResultItem>();
					return result;
				}

				return OnCalculateObjects();
			}

			return Array.Empty<ResultItem>();
		}

		/////////////////////////////////////////

		protected virtual void OnObjectEnter( ObjectInSpace obj )
		{
		}

		protected virtual void OnObjectLeave( ObjectInSpace obj )
		{
		}

		public delegate void ObjectEnterLeaveDelegate( Sensor sender, ObjectInSpace obj );
		public event ObjectEnterLeaveDelegate ObjectEnter;
		public event ObjectEnterLeaveDelegate ObjectLeave;

		void PerformObjectEnter( ObjectInSpace obj )
		{
			OnObjectEnter( obj );
			ObjectEnter?.Invoke( this, obj );
		}

		void PerformObjectLeave( ObjectInSpace obj )
		{
			OnObjectLeave( obj );
			ObjectLeave?.Invoke( this, obj );
		}

		public void Update()
		{
			var newList = CalculateObjects();

			//!!!!slowly

			var newSet = new ESet<ObjectInSpace>( newList.Length );
			foreach( var item in newList )
			{
				if( item.SceneObject != null )
					newSet.AddWithCheckAlreadyContained( item.SceneObject );
			}

			var currentSet = new ESet<ObjectInSpace>( Objects.Count );
			foreach( var refObject in Objects )
			{
				var obj = refObject.Value;
				if( obj != null )
					currentSet.AddWithCheckAlreadyContained( obj );
			}

			List<ObjectInSpace> toLeave = null;
			List<ObjectInSpace> toEnter = null;

			//get leave list
			foreach( var refObj in Objects )
			{
				var obj = refObj.Value;
				if( obj != null )
				{
					if( !newSet.Contains( obj ) )
					{
						if( toLeave == null )
							toLeave = new List<ObjectInSpace>();
						toLeave.Add( obj );
					}
				}
			}

			//get enter list
			foreach( var item in newList )
			{
				if( item.SceneObject != null && !currentSet.Contains( item.SceneObject ) )
				{
					if( toEnter == null )
						toEnter = new List<ObjectInSpace>();
					toEnter.Add( item.SceneObject );
				}
			}

			//remove nulls
			for( int n = Objects.Count - 1; n >= 0; n-- )
			{
				if( Objects[ n ].Value == null )
					Objects.RemoveAt( n );
			}

			//leave
			if( toLeave != null )
			{
				foreach( var obj in toLeave )
				{
					var index = Objects.IndexOf( obj );
					if( index != -1 )
					{
						Objects.RemoveAt( index );
						PerformObjectLeave( obj );
					}
				}
			}

			//enter
			if( toEnter != null )
			{
				foreach( var obj in toEnter )
				{
					Objects.Add( new Reference<ObjectInSpace>( obj, ReferenceUtility.CalculateRootReference( obj ) ) );
					PerformObjectEnter( obj );
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Sensor" );
		}
	}
}
