// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Pathfinding
{
	/// <summary>
	/// Represents a geometry data for pathfinding calculation.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Pathfinding Geometry", -9995 )]
	public class Component_Pathfinding_Geometry : Component_ObjectInSpace
	{
		/// <summary>
		/// The available types of a geometry.
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
			/// <summary>
			/// The dynamic obstacle.
			/// </summary>
			TemporaryObstacle,
		}

		/// <summary>
		/// The type of the geometry.
		/// </summary>
		[DefaultValue( TypeEnum.WalkableArea )]
		[Serialize]
		public Reference<TypeEnum> Type
		{
			get { if( _type.BeginGet() ) Type = _type.Get( this ); return _type.value; }
			set
			{
				var oldValue = _type.value.Value;

				if( _type.BeginSet( ref value ) )
				{
					try
					{
						TypeChanged?.Invoke( this );

						if( oldValue == TypeEnum.TemporaryObstacle || value.Value == TypeEnum.TemporaryObstacle )
							UpdateTempObstacles();
					}
					finally { _type.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Type"/> property value changes.</summary>
		public event Action<Component_Pathfinding_Geometry> TypeChanged;
		ReferenceField<TypeEnum> _type = TypeEnum.WalkableArea;

		/// <summary>
		/// The available shapes of a geometry.
		/// </summary>
		public enum ShapeEnum
		{
			Box,
			Cylinder,
			//!!!!что-то еще
		}

		[DefaultValue( ShapeEnum.Box )]
		[Serialize]
		public Reference<ShapeEnum> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set
			{
				if( _shape.BeginSet( ref value ) )
				{
					try
					{
						ShapeChanged?.Invoke( this );

						SpaceBoundsUpdate();
						if( Type.Value == TypeEnum.TemporaryObstacle )
							UpdateTempObstacles();
					}
					finally { _shape.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<Component_Pathfinding_Geometry> ShapeChanged;
		ReferenceField<ShapeEnum> _shape = ShapeEnum.Box;

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Shape ):
					{
						var geometryType = Type.Value;
						if( geometryType == TypeEnum.BakedObstacle || geometryType == TypeEnum.TemporaryObstacle )
							skip = true;
					}
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

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				//!!!!опцию DisplayPathfindingGeometries

				bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() /*!!!! && ParentScene.DisplayLights */) ||
					context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.CanSelectColor;
					else
					{
						//!!!!
						color = new ColorValue( 0, 0, 1 );
						//color = ProjectSettings.Get.SceneShowLightColor;
					}

					var viewport = context.Owner;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					DebugDraw( viewport );
				}
				//if( !show )
				//	context.disableShowingLabelForThisObject = true;




				//var displayColor = DisplayColor.Value;
				//if( displayColor.Alpha != 0 )
				//{
				//	context.viewport.Simple3DRenderer.SetColor( displayColor, displayColor * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
				//	RenderShape( context );
				//}

				//if( DisplayObjects )
				//{
				//	var color = DisplayObjectsColor.Value;
				//	if( color.Alpha != 0 )
				//	{
				//		context.viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
				//		//foreach( var refObject in Objects )
				//		//{
				//		//	var obj = refObject.Value;
				//		//	if( obj != null && obj.EnabledInHierarchy )
				//		//		context.viewport.Simple3DRenderer.AddBounds( obj.SpaceBounds.CalculatedBoundingBox );
				//		//}

				//		foreach( var item in CalculateObjects() )
				//			RenderItem( context.viewport.Simple3DRenderer, item );
				//	}
				//}
			}
		}

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

					SimpleMeshGenerator.GenerateCylinder( 2, cylinder.Radius, cylinder.GetLength(), 16, true, true, true, out Vector3[] verticesLocal, out indices );

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
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( Type.Value == TypeEnum.TemporaryObstacle )
				UpdateTempObstacles();
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			if( Type.Value == TypeEnum.TemporaryObstacle )
				UpdateTempObstacles();
		}

		void UpdateTempObstacles()
		{
			var instances = Component_Pathfinding.Instances;
			for( int n = 0; n < instances.Count; n++ )
			{
				var pathfinding = instances[ n ];

				//!!!!было
				////!!!!пока так
				//pathfinding.tempObstaclesNeedUpdate = true;
			}
		}
	}
}
