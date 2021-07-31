// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the tag to mark a geometry data for pathfinding calculation.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Pathfinding Geometry Tag", -9994 )]
	public class Component_Pathfinding_GeometryTag : Component
	{
		/// <summary>
		/// The available types of a geometry tag.
		/// </summary>
		public enum TypeEnum
		{
			/// <summary>
			/// A character can walk on top of a geometry.
			/// </summary>
			WalkableArea,
			/// <summary>
			/// A character can't walk on top of a geometry.
			/// </summary>
			BakedObstacle,
			//!!!!пока нет. TemporaryObstacle,
		}

		/// <summary>
		/// The type of the geometry tag.
		/// </summary>
		[DefaultValue( TypeEnum.WalkableArea )]
		[Serialize]
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set { if( _type.BeginSet( ref value ) ) { try { TypeChanged?.Invoke( this ); } finally { _type.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_Pathfinding_GeometryTag> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.WalkableArea;

		//public enum ObstacleShapeEnum
		//{
		//	Cylinder,
		//	Box,
		//	//!!!!что-то еще
		//}

		//[DefaultValue( ObstacleShapeEnum.Cylinder )]
		//[Serialize]
		//public Reference<ObstacleShapeEnum> ObstacleShape
		//{
		//	get { if( _obstacleShape.BeginGet() ) ObstacleShape = _obstacleShape.Get( this ); return _obstacleShape.value; }
		//	set { if( _obstacleShape.BeginSet( ref value ) ) { try { ObstacleShapeChanged?.Invoke( this ); } finally { _obstacleShape.EndSet(); } } }
		//}
		//public event Action<Component_Pathfinding_GeometryTag> ObstacleShapeChanged;
		//ReferenceField<ObstacleShapeEnum> _obstacleShape = ObstacleShapeEnum.Cylinder;

		//protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		//{
		//	base.OnMetadataGetMembersFilter( context, member, ref skip );

		//	var p = member as Metadata.Property;
		//	if( p != null )
		//	{
		//		switch( p.Name )
		//		{
		//		case nameof( ObstacleShape ):
		//			{
		//				var geometryType = Type.Value;
		//				if( geometryType == TypeEnum.BakedObstacle || geometryType == TypeEnum.TemporaryObstacle )
		//					skip = true;
		//			}
		//			break;
		//		}
		//	}
		//}
	}
}
