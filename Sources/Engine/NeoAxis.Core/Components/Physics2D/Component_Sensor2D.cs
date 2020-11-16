// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The event trigger in the scene with 2D physics support.
	/// </summary>
	[NewObjectDefaultName( "Sensor 2D" )]
	[AddToResourcesWindow( @"Base\2D\Sensor 2D", -7991 )]
	public class Component_Sensor2D : Component_ObjectInSpace
	{
		//Matrix4 hullFrom;
		//Matrix4 hullTo;
		//ShapeEnum hullShape;
		//Vector3[] hullVertices;
		//int[] hullIndices;
		//Plane[] hullPlanes;

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
		[Category( "Sensor 2D" )]
		public Reference<SourceDataEnum> SourceData
		{
			get { if( _sourceData.BeginGet() ) SourceData = _sourceData.Get( this ); return _sourceData.value; }
			set { if( _sourceData.BeginSet( ref value ) ) { try { SourceDataChanged?.Invoke( this ); } finally { _sourceData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SourceData"/> property value changes.</summary>
		public event Action<Component_Sensor2D> SourceDataChanged;
		ReferenceField<SourceDataEnum> _sourceData = SourceDataEnum.ObjectsInSpace;

		public enum ShapeEnum
		{
			Box,
			//Ellipse,
			//Capsule,
			Ray,
			//Spline и другие
			//Mesh, ConvexMesh
		}

		/// <summary>
		/// The shape of the trigger.
		/// </summary>
		[DefaultValue( ShapeEnum.Box )]
		[Category( "Sensor 2D" )]
		public Reference<ShapeEnum> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set { if( _shape.BeginSet( ref value ) ) { try { ShapeChanged?.Invoke( this ); } finally { _shape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<Component_Sensor2D> ShapeChanged;
		ReferenceField<ShapeEnum> _shape = ShapeEnum.Box;

		/// <summary>
		/// The target by which the sensor ray will trigger.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Sensor 2D" )]
		public Reference<Component_ObjectInSpace> RayTarget
		{
			get { if( _rayTarget.BeginGet() ) RayTarget = _rayTarget.Get( this ); return _rayTarget.value; }
			set { if( _rayTarget.BeginSet( ref value ) ) { try { RayTargetChanged?.Invoke( this ); } finally { _rayTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RayTarget"/> property value changes.</summary>
		public event Action<Component_Sensor2D> RayTargetChanged;
		ReferenceField<Component_ObjectInSpace> _rayTarget;

		///// <summary>
		///// The end point of the sensor in Convex Sweep mode.
		///// </summary>
		//[DefaultValue( null )]
		//public Reference<Component_ObjectInSpace> ConvexSweepTarget
		//{
		//	get { if( _convexSweepTarget.BeginGet() ) ConvexSweepTarget = _convexSweepTarget.Get( this ); return _convexSweepTarget.value; }
		//	set { if( _convexSweepTarget.BeginSet( ref value ) ) { try { ConvexSweepTargetChanged?.Invoke( this ); } finally { _convexSweepTarget.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ConvexSweepTarget"/> property value changes.</summary>
		//public event Action<Component_Sensor2D> ConvexSweepTargetChanged;
		//ReferenceField<Component_ObjectInSpace> _convexSweepTarget;

		public enum ModeEnum
		{
			OneClosestObject,
			AllObjects,
		}

		/// <summary>
		/// The recognition mode of the sensor.
		/// </summary>
		[DefaultValue( ModeEnum.AllObjects )]
		[Category( "Sensor 2D" )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Component_Sensor2D> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.AllObjects;

		//!!!!list?
		/// <summary>
		/// The type of objects that sensor will recognize.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Sensor 2D" )]
		public Reference<Metadata.TypeInfo> FilterByType
		{
			get { if( _filterByType.BeginGet() ) FilterByType = _filterByType.Get( this ); return _filterByType.value; }
			set { if( _filterByType.BeginSet( ref value ) ) { try { FilterByTypeChanged?.Invoke( this ); } finally { _filterByType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FilterByType"/> property value changes.</summary>
		public event Action<Component_Sensor2D> FilterByTypeChanged;
		ReferenceField<Metadata.TypeInfo> _filterByType;

		/// <summary>
		/// The collision categories this fixture is a part of.
		/// </summary>
		[DefaultValue( Category.Category1 )]
		[Category( "Sensor 2D" )]
		public Reference<Category> CollisionCategories
		{
			get { if( _collisionCategories.BeginGet() ) CollisionCategories = _collisionCategories.Get( this ); return _collisionCategories.value; }
			set { if( _collisionCategories.BeginSet( ref value ) ) { try { CollisionCategoriesChanged?.Invoke( this ); } finally { _collisionCategories.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionCategories"/> property value changes.</summary>
		public event Action<Component_Sensor2D> CollisionCategoriesChanged;
		ReferenceField<Category> _collisionCategories = Category.Category1;

		/// <summary>
		/// The collision mask bits. This states the categories that this fixture would accept for collision.
		/// </summary>
		[DefaultValue( Category.All )]
		[Category( "Sensor 2D" )]
		public Reference<Category> CollidesWith
		{
			get { if( _collidesWith.BeginGet() ) CollidesWith = _collidesWith.Get( this ); return _collidesWith.value; }
			set { if( _collidesWith.BeginSet( ref value ) ) { try { CollidesWithChanged?.Invoke( this ); } finally { _collidesWith.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollidesWith"/> property value changes.</summary>
		public event Action<Component_Sensor2D> CollidesWithChanged;
		ReferenceField<Category> _collidesWith = Category.All;

		/// <summary>
		/// Collision groups allow a certain group of objects to never collide or always collide. Zero means no collision group. Non-zero group filtering always wins against the mask bits.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Sensor 2D" )]
		public Reference<int> CollisionGroup
		{
			get { if( _collisionGroup.BeginGet() ) CollisionGroup = _collisionGroup.Get( this ); return _collisionGroup.value; }
			set { if( _collisionGroup.BeginSet( ref value ) ) { try { CollisionGroupChanged?.Invoke( this ); } finally { _collisionGroup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CollisionGroup"/> property value changes.</summary>
		public event Action<Component_Sensor2D> CollisionGroupChanged;
		ReferenceField<int> _collisionGroup = 0;

		/// <summary>
		/// Whether the sensor is ignoring other sensors.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Sensor 2D" )]
		public Reference<bool> IgnoreSensors
		{
			get { if( _ignoreSensors.BeginGet() ) IgnoreSensors = _ignoreSensors.Get( this ); return _ignoreSensors.value; }
			set { if( _ignoreSensors.BeginSet( ref value ) ) { try { IgnoreSensorsChanged?.Invoke( this ); } finally { _ignoreSensors.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IgnoreSensors"/> property value changes.</summary>
		public event Action<Component_Sensor2D> IgnoreSensorsChanged;
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
		[Category( "Sensor 2D" )]
		public Reference<WhenUpdateEnum> WhenUpdate
		{
			get { if( _whenUpdate.BeginGet() ) WhenUpdate = _whenUpdate.Get( this ); return _whenUpdate.value; }
			set { if( _whenUpdate.BeginSet( ref value ) ) { try { WhenUpdateChanged?.Invoke( this ); } finally { _whenUpdate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WhenUpdate"/> property value changes.</summary>
		public event Action<Component_Sensor2D> WhenUpdateChanged;
		ReferenceField<WhenUpdateEnum> _whenUpdate = /*WhenUpdateEnum.SimulationStep |*/ WhenUpdateEnum.Update;

		/// <summary>
		/// The display color of the sensor.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Sensor 2D" )]
		public Reference<ColorValue> DisplayColor
		{
			get { if( _displayColor.BeginGet() ) DisplayColor = _displayColor.Get( this ); return _displayColor.value; }
			set { if( _displayColor.BeginSet( ref value ) ) { try { DisplayColorChanged?.Invoke( this ); } finally { _displayColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayColor"/> property value changes.</summary>
		public event Action<Component_Sensor2D> DisplayColorChanged;
		ReferenceField<ColorValue> _displayColor = new ColorValue( 1, 1, 1 );

		/// <summary>
		/// Whether to draw gizmos on the objects recognized by the sensor.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Sensor 2D" )]
		public Reference<bool> DisplayObjects
		{
			get { if( _displayObjects.BeginGet() ) DisplayObjects = _displayObjects.Get( this ); return _displayObjects.value; }
			set { if( _displayObjects.BeginSet( ref value ) ) { try { DisplayObjectsChanged?.Invoke( this ); } finally { _displayObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayObjects"/> property value changes.</summary>
		public event Action<Component_Sensor2D> DisplayObjectsChanged;
		ReferenceField<bool> _displayObjects = false;

		/// <summary>
		/// The color of the recognized object gizmos.
		/// </summary>
		[DefaultValue( "1 0 0" )]
		[Category( "Sensor 2D" )]
		public Reference<ColorValue> DisplayObjectsColor
		{
			get { if( _displayObjectsColor.BeginGet() ) DisplayObjectsColor = _displayObjectsColor.Get( this ); return _displayObjectsColor.value; }
			set { if( _displayObjectsColor.BeginSet( ref value ) ) { try { DisplayObjectsColorChanged?.Invoke( this ); } finally { _displayObjectsColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayObjectsColor"/> property value changes.</summary>
		public event Action<Component_Sensor2D> DisplayObjectsColorChanged;
		ReferenceField<ColorValue> _displayObjectsColor = new ColorValue( 1, 0, 0 );

		/// <summary>
		/// List of objects recognized by the sensor.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[Category( "Sensor 2D" )]
		public ReferenceList<Component_ObjectInSpace> Objects
		{
			get { return _objects; }
		}
		public delegate void ObjectsChangedDelegate( Component_Sensor2D sender );
		public event ObjectsChangedDelegate ObjectsChanged;
		ReferenceList<Component_ObjectInSpace> _objects;

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

				case nameof( CollisionCategories ):
				case nameof( CollidesWith ):
				case nameof( CollisionGroup ):
					if( SourceData.Value != SourceDataEnum.PhysicsObjects )
						skip = true;
					break;

				case nameof( IgnoreSensors ):
					if( SourceData.Value != SourceDataEnum.ObjectsInSpace )
						skip = true;
					break;

					//case nameof( ConvexSweepTarget ):
					//	if( !( Shape.Value == ShapeEnum.Box || Shape.Value == ShapeEnum.Sphere ) )
					//		skip = true;
					//	break;
				}
			}
		}

		/////////////////////////////////////////

		public Component_Sensor2D()
		{
			_objects = new ReferenceList<Component_ObjectInSpace>( this, () => ObjectsChanged?.Invoke( this ) );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsNotResource )
			{
				if( WhenUpdate.Value.HasFlag( WhenUpdateEnum.Enable ) )
					Update();
			}
			else
				Update();
		}

		Box GetBox()
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			return new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		Box GetBoxExtendedZ()
		{
			var box = GetBox();
			box.Extents.Z = 10000;
			return box;
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

		public Ray2 GetRay2()
		{
			return GetRay().ToRay2();
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
			//if( ConvexSweepTarget.ReferenceSpecified && ( Shape.Value == ShapeEnum.Box || Shape.Value == ShapeEnum.Sphere ) && context.viewport.Simple3DRenderer != null )
			//{
			//	var target = ConvexSweepTarget.Value;
			//	if( target != null )
			//	{
			//		if( hullVertices != null )
			//			context.viewport.Simple3DRenderer.AddTriangles( hullVertices, hullIndices, true, false );
			//		return;
			//	}
			//}

			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				context.viewport.Simple3DRenderer.AddBox( GetBox() );
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

			if( WhenUpdate.Value.HasFlag( WhenUpdateEnum.Update ) )
				Update();
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( WhenUpdate.Value.HasFlag( WhenUpdateEnum.SimulationStep ) )
				Update();
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			//if( WhenUpdate.Value.HasFlag( WhenUpdateEnum.Render ) )
			//	Update();

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplaySensors ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					if( context2.displaySensorsCounter < context2.displaySensorsMax )
					{
						context2.displaySensorsCounter++;

						var displayColor = DisplayColor.Value;
						if( displayColor.Alpha != 0 )
						{
							context.Owner.Simple3DRenderer.SetColor( displayColor, displayColor * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
							RenderShape( context2 );
						}
					}

					if( DisplayObjects )
					{
						var color = DisplayObjectsColor.Value;
						if( color.Alpha != 0 )
						{
							context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
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
			if( item.Object != null && item.Object.EnabledInHierarchy )
			{
				renderer.AddBounds( item.Object.SpaceBounds.CalculatedBoundingBox );

				//!!!!
				//!!!!radius
				if( item.Position.HasValue )
				{
					var p = item.Position.Value;
					renderer.AddSphere( new Sphere( new Vector3( p.X, p.Y, 0 ), 0.05 ), 8 );
				}

				////getting triangle to draw
				//var triIdx = item.RayTestResult?.TriangleIndexProcessed ?? -1;

				//if( triIdx >= 0 )
				//{
				//	if( item.RayTestResult?.Shape is Component_CollisionShape_Mesh csMesh )
				//	{
				//		var t = item.Object.Transform.Value.ToMatrix4();
				//		var local = csMesh.TransformRelativeToParent.Value;
				//		if( !local.IsIdentity )
				//			t *= local.ToMatrix4();

				//		if( csMesh.GetTriangleProcessedData( triIdx, true, out var triangle ) )
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
				//	else if( item.RayTestResult?.Body is Component_SoftBody softBody )
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

			}
		}

		/////////////////////////////////////////

		//!!!!later for optimization. but need save multithreading support
		//bool tempIgnoreSensors;
		//Metadata.TypeInfo tempFilterByType;

		protected virtual void OnFilterObject( ResultItem item, ref bool ignore )
		{
			if( IgnoreSensors && ( item.Object == this || typeof( Component_Sensor2D ).IsAssignableFrom( item.Object.GetType() ) ) )
			{
				ignore = true;
				return;
			}
			var filterByType = FilterByType.Value;
			if( filterByType != null && !filterByType.IsAssignableFrom( item.Object.BaseType ) )
				ignore = true;
		}

		public delegate void FilterObjectEventDelegate( Component_Sensor2D sensor, ResultItem item, ref bool ignore );
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
			public Component_ObjectInSpace Object;
			public Vector2? Position;
			public double DistanceScale;

			public Physics2DRayTestItem.ResultItem RayTestResult;
			//!!!!physics shape
			//!!!!triangle index

			public ResultItem( Component_ObjectInSpace obj, Vector2? position, double distanceScale, Physics2DRayTestItem.ResultItem testResult = null )
			{
				Object = obj;
				Position = position;
				DistanceScale = distanceScale;
				RayTestResult = testResult;
			}

			public override string ToString()
			{
				if( Object != null )
					return Object.ToString();
				else
					return "(Null)";
			}
		}

		//void UpdateSweepHullGeometry( Vector3[] shapeLocalVertices )
		//{
		//	var target = ConvexSweepTarget.Value;

		//	var tr = Transform.Value;
		//	var from = tr.ToMatrix4( true, true, false );
		//	var to = target.Transform.Value.ToMatrix4( true, false, false );

		//	var hullPositions = new Vector3[ shapeLocalVertices.Length * 2 ];
		//	for( int i = 0; i < shapeLocalVertices.Length; i++ )
		//	{
		//		Matrix4.Multiply( ref from, ref shapeLocalVertices[ i ], out hullPositions[ i ] );
		//		Matrix4.Multiply( ref to, ref shapeLocalVertices[ i ], out hullPositions[ i + shapeLocalVertices.Length ] );
		//	}

		//	ConvexHullAlgorithm.Create( hullPositions, out hullVertices, out hullIndices, out hullPlanes );
		//}

		//void UpdateConvexSweepHull()
		//{
		//	var target = ConvexSweepTarget.Value;
		//	if( target != null )
		//	{
		//		var tr = Transform.Value;
		//		var from = tr.ToMatrix4();
		//		var to = target.Transform.Value.ToMatrix4();
		//		var shape = Shape.Value;

		//		var update = hullVertices == null || !hullFrom.Equals( from, .000001 ) || !hullTo.Equals( to, .000001 ) || hullShape != shape;
		//		if( update )
		//		{
		//			hullFrom = from;
		//			hullTo = to;
		//			hullShape = shape;

		//			if( shape == ShapeEnum.Box )
		//			{
		//				var boxLocalVertices = new Bounds( tr.Scale * -.5, tr.Scale * .5 ).ToPoints();
		//				UpdateSweepHullGeometry( boxLocalVertices );
		//			}
		//			else if( shape == ShapeEnum.Sphere )
		//			{
		//				var radius = Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5;
		//				//!!!!12
		//				SimpleMeshGenerator.GenerateSphere( radius, 12, 12, false, out Vector3[] sphereLocalVertices, out _ );
		//				UpdateSweepHullGeometry( sphereLocalVertices );
		//			}
		//		}
		//	}
		//	else
		//	{
		//		hullVertices = null;
		//		hullIndices = null;
		//		hullPlanes = null;
		//	}
		//}

		ResultItem[] CalculateObjects_ObjectsInSpace()
		{
			var scene = ParentScene;
			if( scene != null )
			{
				switch( Shape.Value )
				{
				case ShapeEnum.Box:
					{
						var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, GetBoxExtendedZ() );
						scene.GetObjectsInSpace( item );

						if( item.Result.Length != 0 )
						{
							var result = new List<ResultItem>( item.Result.Length );

							for( int n = 0; n < item.Result.Length; n++ )
							{
								ref var resultItem2 = ref item.Result[ n ];

								var resultItem = new ResultItem( resultItem2.Object, null, 0 );

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

				case ShapeEnum.Ray:
					{
						var ray = GetRay2();
						if( ray.Direction != Vector2.Zero )
						{
							//!!!!slowly. use Box instead Bounds
							//Matrix3.FromRotateByZ( xxx );
							//var boxVolume = new Box(;

							var rayFrom = ray.Origin;
							var rayTo = ray.GetEndPoint();
							var rayBounds = new Rectangle( ray.Origin );
							rayBounds.Add( ray.GetEndPoint() );

							var posZ = TransformV.Position.Z;
							var bounds = new Bounds( rayBounds.Left, rayBounds.Top, posZ - 10000, rayBounds.Right, rayBounds.Bottom, posZ + 10000 );

							var item = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, bounds );
							scene.GetObjectsInSpace( item );

							if( item.Result.Length != 0 )
							{
								var result = new List<ResultItem>( item.Result.Length );

								for( int n = 0; n < item.Result.Length; n++ )
								{
									ref var resultItem2 = ref item.Result[ n ];

									var objBounds = resultItem2.Object.SpaceBounds.CalculatedBoundingBox.ToRectangle();
									var count = MathAlgorithms.IntersectRectangleLine( objBounds, rayFrom, rayTo, out var intersect1, out var intersect2 );
									if( count != 0 )
									{
										Vector2 intersect;
										if( count == 2 )
											intersect = ( intersect1 - rayFrom ).LengthSquared() < ( intersect2 - rayFrom ).LengthSquared() ? intersect1 : intersect2;
										else
											intersect = intersect1;

										var scale = MathEx.Saturate( ( intersect - rayFrom ).Length() / ray.Direction.Length() );

										var resultItem = new ResultItem( resultItem2.Object, intersect, scale );

										bool ignore = false;
										PerformFilterObject( resultItem, ref ignore );
										if( ignore )
											continue;

										result.Add( resultItem );
									}
								}

								if( result.Count != 0 )
								{
									//sort by distance
									CollectionUtility.SelectionSort( result, delegate ( ResultItem x, ResultItem y )
									{
										if( x.DistanceScale < y.DistanceScale )
											return -1;
										if( x.DistanceScale > y.DistanceScale )
											return 1;
										return 0;
									} );

									if( Mode.Value == ModeEnum.OneClosestObject )
										return new ResultItem[] { result[ 0 ] };
									else
										return result.ToArray();
								}
							}
						}
					}
					break;
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
					//case ShapeEnum.Sphere:
					{
						//if( ConvexSweepTarget.ReferenceSpecified )
						//{
						//	//Convex sweep test
						//	var target = ConvexSweepTarget.Value;
						//	if( target != null )
						//	{
						//		UpdateConvexSweepHull();

						//		var tr = Transform.Value;
						//		var from = tr.ToMatrix4( true, true, false );
						//		var to = target.Transform.Value.ToMatrix4( true, false, false );

						//		//!!!!PhysicsConvexSweepTestItem.ModeEnum.OneClosestForEach

						//		PhysicsConvexSweepTestItem item = null;
						//		switch( Shape.Value )
						//		{
						//		case ShapeEnum.Box:
						//			item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Bounds( tr.Scale * -.5, tr.Scale * .5 ) );//GetBox()
						//			break;
						//		case ShapeEnum.Sphere:
						//			item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Sphere( Vector3.Zero, Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5 ) );//GetSphere()
						//			break;
						//		}
						//		if( item != null )
						//		{
						//			scene.PhysicsConvexSweepTest( item );
						//			if( item.Result.Length != 0 )
						//			{
						//				var result = new List<ResultItem>( item.Result.Length );
						//				foreach( var i in item.Result )
						//				{
						//					//!!!!Distance good?
						//					var length = ( to.GetTranslation() - from.GetTranslation() ).Length();
						//					var resultItem = new ResultItem( i.Body, i.Position, i.DistanceScale * length );
						//					//var resultItem = new ResultItem( i.Body, i.Position, i.Distance );

						//					bool ignore = false;
						//					PerformFilterObject( resultItem, ref ignore );
						//					if( ignore )
						//						continue;

						//					result.Add( resultItem );

						//					if( Mode.Value == ModeEnum.OneClosestObject )
						//						break;
						//				}
						//				return result.ToArray();
						//			}
						//		}
						//	}
						//}
						//else
						//{

						Physics2DContactTestItem item = null;
						switch( Shape.Value )
						{
						case ShapeEnum.Box:

							var points = GetBox().ToPoints();

							var points2 = new Vector2[ 4 ];
							for( int n = 0; n < 4; n++ )
								points2[ n ] = points[ n ].ToVector2();

							item = new Physics2DContactTestItem( points2, CollisionCategories, CollidesWith, CollisionGroup, Physics2DContactTestItem.ModeEnum.All );

							break;
						}

						if( item != null )
						{
							scene.Physics2DContactTest( item );
							if( item.Result.Length != 0 )
							{
								var result = new List<ResultItem>( item.Result.Length );
								foreach( var i in item.Result )
								{
									var resultItem = new ResultItem( i.Body, null, 0 );

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

						//}
					}
					break;


				//case ShapeEnum.Box:
				//	//!!!!
				//	//case ShapeEnum.Sphere:
				//	{
				//		//!!!!
				//		//if( ConvexSweepTarget.ReferenceSpecified )
				//		//{
				//		//	//Convex sweep test
				//		//	var target = ConvexSweepTarget.Value;
				//		//	if( target != null )
				//		//	{
				//		//		UpdateConvexSweepHull();

				//		//		var tr = Transform.Value;
				//		//		var from = tr.ToMatrix4( true, true, false );
				//		//		var to = target.Transform.Value.ToMatrix4( true, false, false );

				//		//		//!!!!PhysicsConvexSweepTestItem.ModeEnum.OneClosestForEach

				//		//		PhysicsConvexSweepTestItem item = null;
				//		//		switch( Shape.Value )
				//		//		{
				//		//		case ShapeEnum.Box:
				//		//			item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Bounds( tr.Scale * -.5, tr.Scale * .5 ) );//GetBox()
				//		//			break;
				//		//		case ShapeEnum.Sphere:
				//		//			item = new PhysicsConvexSweepTestItem( from, to, PhysicsFilterGroup, PhysicsFilterMask, PhysicsConvexSweepTestItem.ModeEnum.All, new Sphere( Vector3.Zero, Math.Max( tr.Scale.X, Math.Max( tr.Scale.Y, tr.Scale.Z ) ) * 0.5 ) );//GetSphere()
				//		//			break;
				//		//		}
				//		//		if( item != null )
				//		//		{
				//		//			scene.PhysicsConvexSweepTest( item );
				//		//			if( item.Result.Length != 0 )
				//		//			{
				//		//				var result = new List<ResultItem>( item.Result.Length );
				//		//				foreach( var i in item.Result )
				//		//				{
				//		//					//!!!!Distance good?
				//		//					var length = ( to.GetTranslation() - from.GetTranslation() ).Length();
				//		//					var resultItem = new ResultItem( i.Body, i.Position, i.DistanceScale * length );
				//		//					//var resultItem = new ResultItem( i.Body, i.Position, i.Distance );

				//		//					bool ignore = false;
				//		//					PerformFilterObject( resultItem, ref ignore );
				//		//					if( ignore )
				//		//						continue;

				//		//					result.Add( resultItem );

				//		//					if( Mode.Value == ModeEnum.OneClosestObject )
				//		//						break;
				//		//				}
				//		//				return result.ToArray();
				//		//			}
				//		//		}
				//		//	}
				//		//}
				//		//else
				//		//{
				//		//Contact test
				//		PhysicsContactTestItem item = null;
				//		switch( Shape.Value )
				//		{
				//		case ShapeEnum.Box:
				//			item = new PhysicsContactTestItem( PhysicsFilterGroup, PhysicsFilterMask, PhysicsContactTestItem.ModeEnum.OneClosestForEach, GetBox() );
				//			break;
				//			//!!!!
				//			//case ShapeEnum.Sphere:
				//			//	item = new PhysicsContactTestItem( PhysicsFilterGroup, PhysicsFilterMask, PhysicsContactTestItem.ModeEnum.OneClosestForEach, GetSphere() );
				//			//	break;
				//		}
				//		if( item != null )
				//		{
				//			scene.PhysicsContactTest( item );
				//			if( item.Result.Length != 0 )
				//			{
				//				var result = new List<ResultItem>( item.Result.Length );
				//				foreach( var i in item.Result )
				//				{
				//					var resultItem = new ResultItem( i.Body, i.PositionWorldOnA, i.Distance );

				//					bool ignore = false;
				//					PerformFilterObject( resultItem, ref ignore );
				//					if( ignore )
				//						continue;

				//					result.Add( resultItem );

				//					if( Mode.Value == ModeEnum.OneClosestObject )
				//						break;
				//				}
				//				return result.ToArray();
				//			}
				//		}
				//		//}
				//	}
				//	break;

				case ShapeEnum.Ray:
					{
						var item = new Physics2DRayTestItem( GetRay2(), CollisionCategories, CollidesWith, CollisionGroup, Physics2DRayTestItem.ModeEnum.OneClosestForEach );

						scene.Physics2DRayTest( item );
						if( item.Result.Length != 0 )
						{
							var result = new List<ResultItem>( item.Result.Length );
							foreach( var i in item.Result )
							{
								var resultItem = new ResultItem( i.Body, i.Position, i.DistanceScale, i );

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

		public delegate void CalculateObjectsEventDelegate( Component_Sensor2D sender, ref bool handled, ref ResultItem[] result );
		public event CalculateObjectsEventDelegate CalculateObjectsEvent;

		public ResultItem[] CalculateObjects()
		{
			if( EnabledInHierarchyAndIsNotResource )
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

		protected virtual void OnObjectEnter( Component_ObjectInSpace obj )
		{
		}

		protected virtual void OnObjectLeave( Component_ObjectInSpace obj )
		{
		}

		public delegate void ObjectEnterLeaveDelegate( Component_Sensor2D sender, Component_ObjectInSpace obj );
		public event ObjectEnterLeaveDelegate ObjectEnter;
		public event ObjectEnterLeaveDelegate ObjectLeave;

		void PerformObjectEnter( Component_ObjectInSpace obj )
		{
			OnObjectEnter( obj );
			ObjectEnter?.Invoke( this, obj );
		}

		void PerformObjectLeave( Component_ObjectInSpace obj )
		{
			OnObjectLeave( obj );
			ObjectLeave?.Invoke( this, obj );
		}

		public void Update()
		{
			var newList = CalculateObjects();

			//!!!!slowly

			var newSet = new ESet<Component_ObjectInSpace>( newList.Length );
			foreach( var item in newList )
				newSet.AddWithCheckAlreadyContained( item.Object );

			var currentSet = new ESet<Component_ObjectInSpace>( Objects.Count );
			foreach( var refObject in Objects )
			{
				var obj = refObject.Value;
				if( obj != null )
					currentSet.AddWithCheckAlreadyContained( obj );
			}

			List<Component_ObjectInSpace> toLeave = null;
			List<Component_ObjectInSpace> toEnter = null;

			//get leave list
			foreach( var refObj in Objects )
			{
				var obj = refObj.Value;
				if( obj != null )
				{
					if( !newSet.Contains( obj ) )
					{
						if( toLeave == null )
							toLeave = new List<Component_ObjectInSpace>();
						toLeave.Add( obj );
					}
				}
			}

			//get enter list
			foreach( var item in newList )
			{
				if( !currentSet.Contains( item.Object ) )
				{
					if( toEnter == null )
						toEnter = new List<Component_ObjectInSpace>();
					toEnter.Add( item.Object );
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
					Objects.Add( new Reference<Component_ObjectInSpace>( obj, ReferenceUtility.CalculateRootReference( obj ) ) );
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
