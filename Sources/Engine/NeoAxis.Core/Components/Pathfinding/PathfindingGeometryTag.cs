// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents the tag to mark a geometry data for pathfinding calculation.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Pathfinding\Pathfinding Geometry Tag", 560 )]
	public class PathfindingGeometryTag : Component
	{
		//!!!!RemoveObstacle, Obstacle, Ground/Walkable/ObstacleWalkableOver

		///// <summary>
		///// The available types of a geometry.
		///// </summary>
		//public enum TypeEnum
		//{
		//	///// <summary>
		//	///// A character can walk on top of a geometry.
		//	///// </summary>
		//	//WalkableArea,

		//	/// <summary>
		//	/// A character can't walk on top of a geometry.
		//	/// </summary>
		//	BakedObstacle,
		//}

		///// <summary>
		///// The type of the geometry tag.
		///// </summary>
		//[DefaultValue( TypeEnum.BakedObstacle )]
		//[Serialize]
		//public Reference<TypeEnum> Type
		//{
		//	get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
		//	set { if( _type.BeginSet( this, ref value ) ) { try { TypeChanged?.Invoke( this ); } finally { _type.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		//public event Action<PathfindingGeometryTag> TypeChanged;
		//ReferenceField<TypeEnum> _type = TypeEnum.BakedObstacle;

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
						UpdateParentSubscription();
						DynamicMode_UpdatePathfindingComponents();
					}
					finally { _dynamic.EndSet(); }
				}
			}
		}

		public event Action<PathfindingGeometryTag> DynamicChanged;
		ReferenceField<bool> _dynamic = false;

		[DefaultValue( true )]
		public Reference<bool> Walkable
		{
			get { if( _walkable.BeginGet() ) Walkable = _walkable.Get( this ); return _walkable.value; }
			set
			{
				if( _walkable.BeginSet( this, ref value ) )
				{
					try
					{
						WalkableChanged?.Invoke( this );
						if( Dynamic )
							DynamicMode_UpdatePathfindingComponents();
					}
					finally { _walkable.EndSet(); }
				}
			}
		}
		public event Action<PathfindingGeometryTag> WalkableChanged;
		ReferenceField<bool> _walkable = true;

		public Box GetBox()
		{
			var parent = Parent as ObjectInSpace;
			if( parent == null )
				return new Box( Vector3.Zero, Vector3.Zero, Matrix3.Identity );

			var meshResult = ( parent as MeshInSpace )?.MeshOutput?.Result;
			if( meshResult == null )
				return new Box( parent.SpaceBounds.BoundingBox );

			var tr = parent.Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );

			var localBounds = meshResult.SpaceBounds.BoundingBox;
			var scaledBounds = new Bounds( localBounds.Minimum * tr.Scale, localBounds.Maximum * tr.Scale );

			return new Box( scaledBounds, tr.Position, rot );
		}

		ObjectInSpace subscribedParent;

		void UpdateParentSubscription()
		{
			var newParent = EnabledInHierarchy && Dynamic ? Parent as ObjectInSpace : null;
			if( subscribedParent != newParent )
			{
				if( subscribedParent != null )
					subscribedParent.TransformChanged -= ParentTransformChanged;
				subscribedParent = newParent;
				if( subscribedParent != null )
					subscribedParent.TransformChanged += ParentTransformChanged;
			}
		}

		void ParentTransformChanged( ObjectInSpace sender )
		{
			DynamicMode_UpdatePathfindingComponents();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			UpdateParentSubscription();
			if( Dynamic )
				DynamicMode_UpdatePathfindingComponents();
		}

		internal void DynamicMode_UpdatePathfindingComponents( Pathfinding specifiedPathfinding = null )
		{
			var data = new Pathfinding.DynamicGeometriesToUpdateItem();
			data.Add = EnabledInHierarchy && Dynamic && Parent is ObjectInSpace;
			data.Walkable = Walkable;
			if( data.Add )
				data.Box = GetBox();

			Pathfinding.UpdateDynamicGeometry( this, FindParent<Scene>(), data, specifiedPathfinding );
		}
	}
}
