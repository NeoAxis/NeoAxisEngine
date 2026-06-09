// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Schema;

namespace NeoAxis
{
	/// <summary>
	/// Represents a geometry data for pathfinding calculation.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Pathfinding\Pathfinding Geometry", 550 )]
	public class PathfindingGeometry : ObjectInSpace
	{
		/// <summary>
		/// The available shapes of a geometry.
		/// </summary>
		public enum ShapeEnum
		{
			Box,
			Cylinder,
		}

		/// <summary>
		/// The shape of the geometry.
		/// </summary>
		[DefaultValue( ShapeEnum.Box )]
		[Serialize]
		public Reference<ShapeEnum> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set
			{
				if( _shape.BeginSet( this, ref value ) )
				{
					try
					{
						ShapeChanged?.Invoke( this );

						SpaceBoundsUpdate();
						if( Dynamic )
							DynamicMode_UpdatePathfindingComponents();
					}
					finally { _shape.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<PathfindingGeometry> ShapeChanged;
		ReferenceField<ShapeEnum> _shape = ShapeEnum.Box;

		/// <summary>
		/// Whether to update the geometry data during the simulation. This option is intended for dynamic obstacles.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Dynamic
		{
			get { if( _dynamic.BeginGet() ) Dynamic = _dynamic.Get( this ); return _dynamic.value; }
			set
			{
				if( _dynamic.BeginSet( this, ref value ) )
				{
					try
					{
						DynamicChanged?.Invoke( this );
						DynamicMode_UpdatePathfindingComponents();
					}
					finally { _dynamic.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Dynamic"/> property value changes.</summary>
		public event Action<PathfindingGeometry> DynamicChanged;
		ReferenceField<bool> _dynamic = false;


		//!!!!RemoveObstacle, Obstacle, Ground/Walkable/ObstacleWalkableOver

		/// <summary>
		/// Whether to walk on top of the geometry. This option is available only for static obstacles.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Walkable
		{
			get { if( _walkable.BeginGet() ) Walkable = _walkable.Get( this ); return _walkable.value; }
			set { if( _walkable.BeginSet( this, ref value ) ) { try { WalkableChanged?.Invoke( this ); } finally { _walkable.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Walkable"/> property value changes.</summary>
		public event Action<PathfindingGeometry> WalkableChanged;
		ReferenceField<bool> _walkable = false;

		//

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Walkable ):
					if( Dynamic )
						skip = true;
					break;
				}
			}
		}

		public Box GetBox()
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			return new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		public Cylinder GetCylinder()
		{
			var tr = Transform.Value;
			var up = tr.Rotation.GetUp();
			var v = up * tr.Scale.Z * 0.5;
			return new Cylinder( tr.Position - v, tr.Position + v, Math.Max( tr.Scale.X, tr.Scale.Y ) * 0.5 );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				newBounds = new SpaceBounds( GetBox().ToBounds() );
				break;
			case ShapeEnum.Cylinder:
				newBounds = new SpaceBounds( GetCylinder().ToBounds() );
				break;
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected virtual void DebugDraw( Viewport viewport )
		{
			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				viewport.Simple3DRenderer.AddBox( GetBox() );
				break;
			case ShapeEnum.Cylinder:
				viewport.Simple3DRenderer.AddCylinder( GetCylinder() );
				break;
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				//!!!!? option DisplayPathfindingGeometries

				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication /* && ParentScene.DisplayLights */) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.CanSelectColor;
					else
					{
						//!!!!
						color = new ColorValue( 0, 0, 1 );
						//color = ProjectSettings.Get.SceneShowLightColor;
					}

					var viewport = context.Owner;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
					DebugDraw( viewport );
				}
				//if( !show )
				//	context.disableShowingLabelForThisObject = true;
			}
		}

		//!!!!not work
		//public Pathfinding.ConvexVolume GetConvexVolume()
		//{
		//	var tr = Transform.Value;
		//	if( tr.Rotation.GetUp().Equals( Vector3.ZAxis, 0.01 ) )
		//	{
		//		switch( Shape.Value )
		//		{
		//		case ShapeEnum.Box:
		//			{
		//				//SimpleMeshGenerator.GenerateBox( Vector3.One, out Vector3[] verticesLocal, out _ );

		//				//var transform = Transform.Value.ToMatrix4();

		//				//var vertices = new Vector3[ verticesLocal.Length ];
		//				//for( int n = 0; n < vertices.Length; n++ )
		//				//	vertices[ n ] = transform * verticesLocal[ n ];

		//				var box = GetBox();
		//				var boxCenter = box.Center;
		//				var points = box.ToPoints();

		//				var volume = new Pathfinding.ConvexVolume();
		//				volume.HeightMin = double.MaxValue;
		//				volume.HeightMax = double.MinValue;

		//				foreach( var p in points )
		//				{
		//					if( p.Z < box.Center.Z )
		//						volume.Vertices.Add( p );
		//					var h = p.Z;
		//					if( h < volume.HeightMin )
		//						volume.HeightMin = h;
		//					if( h > volume.HeightMax )
		//						volume.HeightMax = h;
		//				}

		//				return volume;
		//			}

		//			//case ShapeEnum.Cylinder:
		//			//	{
		//			//		var cylinder = GetCylinder();
		//			//		SimpleMeshGenerator.GenerateCylinder( 2, cylinder.Radius, cylinder.GetLength(), 16, true, true, true, out Vector3[] verticesLocal, out int[] indices );
		//			//		var transform = Transform.Value.UpdateScale( Vector3.One );
		//			//		var vertices = new Vector3[ verticesLocal.Length ];
		//			//		for( int n = 0; n < vertices.Length; n++ )
		//			//			vertices[ n ] = transform * verticesLocal[ n ];
		//			//		return new Pathfinding.ConvexVolume( vertices, indices );
		//			//	}

		//		}
		//	}
		//	return null;
		//}

		public void GetGeometry( out Vector3[] vertices, out int[] indices )
		{
			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				{
					SimpleMeshGenerator.GenerateBox( Vector3.One, out Vector3[] verticesLocal, out indices );

					var transform = Transform.Value.ToMatrix4();

					vertices = new Vector3[ verticesLocal.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = transform * verticesLocal[ n ];
				}
				break;

			case ShapeEnum.Cylinder:
				{
					var cylinder = GetCylinder();

					var segments = 16;
					if( cylinder.Radius < 10 )
						segments = 12;
					if( cylinder.Radius < 5 )
						segments = 10;

					SimpleMeshGenerator.GenerateCylinder( 2, cylinder.Radius, cylinder.GetLength(), segments, true, true, true, out Vector3[] verticesLocal, out indices );

					var transform = Transform.Value.UpdateScale( Vector3.One );

					vertices = new Vector3[ verticesLocal.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = transform * verticesLocal[ n ];
				}
				break;

			default:
				vertices = null;
				indices = null;
				break;
			}

			if( indices != null && !Walkable )
			{
				//invert triangle order
				for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				{
					var index0 = indices[ nTriangle * 3 + 0 ];
					var index1 = indices[ nTriangle * 3 + 1 ];
					var index2 = indices[ nTriangle * 3 + 2 ];

					var vertex0 = vertices[ index0 ];
					var vertex1 = vertices[ index1 ];
					var vertex2 = vertices[ index2 ];

					var normal = Vector3.Cross( vertex1 - vertex0, vertex2 - vertex0 );
					if( normal.Z > 0 )
					{
						indices[ nTriangle * 3 + 0 ] = index0;
						indices[ nTriangle * 3 + 1 ] = index2;
						indices[ nTriangle * 3 + 2 ] = index1;
					}
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( Dynamic )
				DynamicMode_UpdatePathfindingComponents();
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			if( Dynamic )
				DynamicMode_UpdatePathfindingComponents();
		}

		internal void DynamicMode_UpdatePathfindingComponents( Pathfinding specifiedPathfinding = null )
		{
			var add = EnabledInHierarchy && Dynamic;

			var scene = ParentScene;
			if( scene != null )
			{
				var instances = Pathfinding.Instances;
				for( int n = 0; n < instances.Count; n++ )
				{
					var pathfinding = instances[ n ];

					if( scene == pathfinding.ParentScene && ( specifiedPathfinding == null || pathfinding == specifiedPathfinding ) )
					{
						var data = new Pathfinding.DynamicGeometriesToUpdateItem();
						data.Add = add;
						if( add )
						{
							switch( Shape.Value )
							{
							case ShapeEnum.Box:
								data.Box = GetBox();
								break;
							case ShapeEnum.Cylinder:
								data.Cylinder = GetCylinder();
								break;
							}
						}

						pathfinding.OnUpdatePathfindingGeometry( this, data );
					}
				}
			}
		}
	}
}
