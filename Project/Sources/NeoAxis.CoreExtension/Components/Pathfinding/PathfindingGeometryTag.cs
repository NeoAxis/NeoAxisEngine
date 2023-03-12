// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents the tag to mark a geometry data for pathfinding calculation.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Pathfinding Geometry Tag", -9994 )]
	public class PathfindingGeometryTag : Component
	{
		/// <summary>
		/// The available types of a geometry.
		/// </summary>
		public enum TypeEnum
		{
			///// <summary>
			///// A character can walk on top of a geometry.
			///// </summary>
			//WalkableArea,

			/// <summary>
			/// A character can't walk on top of a geometry.
			/// </summary>
			BakedObstacle,

			//!!!!DynamicObstacle,
		}

		/// <summary>
		/// The type of the geometry tag.
		/// </summary>
		[DefaultValue( TypeEnum.BakedObstacle )]
		[Serialize]
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set { if( _type.BeginSet( ref value ) ) { try { TypeChanged?.Invoke( this ); } finally { _type.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<PathfindingGeometryTag> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.BakedObstacle;

		/// <summary>
		/// The area of the walkable geometry, which is intended to configure walking cost. Zero value is a non-walkable area.
		/// </summary>
		[DefaultValue( (uint)255 )]
		[Range( 0, 255 )]
		public Reference<uint> Area
		{
			get { if( _area.BeginGet() ) Area = _area.Get( this ); return _area.value; }
			set { if( _area.BeginSet( ref value ) ) { try { AreaChanged?.Invoke( this ); } finally { _area.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Area"/> property value changes.</summary>
		public event Action<PathfindingGeometryTag> AreaChanged;
		ReferenceField<uint> _area = 255;
	}
}
