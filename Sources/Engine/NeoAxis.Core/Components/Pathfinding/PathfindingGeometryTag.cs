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

	}
}
