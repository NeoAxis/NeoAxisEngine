// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The component to clip geometry by volume in real-time.
	/// </summary>
	public class Component_CutVolume : Component_ObjectInSpace
	{
		public enum ShapeEnum
		{
			Box,
			Sphere,//Ellipsoid,
			Cylinder
		}

		/// <summary>
		/// The shape of the volume.
		/// </summary>
		[DefaultValue( ShapeEnum.Box )]
		public Reference<ShapeEnum> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set { if( _shape.BeginSet( ref value ) ) { try { ShapeChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _shape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<Component_CutVolume> ShapeChanged;
		ReferenceField<ShapeEnum> _shape = ShapeEnum.Box;

		///////////////////////////////////////////////

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

		public Cylinder GetCylinder()
		{
			var tr = Transform.Value;
			var forward = tr.Rotation.GetForward();
			var v = forward * tr.Scale.X * 0.5;
			return new Cylinder( tr.Position - v, tr.Position + v, Math.Max( tr.Scale.Y, tr.Scale.Z ) * 0.5 );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				newBounds = new SpaceBounds( GetBox().ToBounds() );
				break;

			case ShapeEnum.Sphere:
				newBounds = new SpaceBounds( GetSphere() );
				break;

			case ShapeEnum.Cylinder:
				newBounds = new SpaceBounds( GetCylinder().ToBounds() );
				break;
			}
		}

		protected virtual void RenderShape( RenderingContext context )
		{
			switch( Shape.Value )
			{
			case ShapeEnum.Box:
				context.viewport.Simple3DRenderer.AddBox( GetBox() );
				break;

			case ShapeEnum.Sphere:
				context.viewport.Simple3DRenderer.AddSphere( GetSphere() );
				break;

			case ShapeEnum.Cylinder:
				{
					var tr = Transform.Value;

					tr.ToMatrix4( true, true, false, out var matrix );
					var radius = Math.Max( tr.Scale.Y, tr.Scale.Z ) * 0.5;
					var height = tr.Scale.X;

					context.viewport.Simple3DRenderer.AddCylinder( matrix, 0, radius, height );
				}
				break;
			}
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var item = new Component_RenderingPipeline.RenderSceneData.CutVolumeItem();
				item.Shape = Shape;

				switch( item.Shape )
				{
				case ShapeEnum.Box:
					item.Transform = Transform;
					break;

				case ShapeEnum.Sphere:
					{
						var sphere = GetSphere();
						var scl = sphere.Radius * 2;
						item.Transform = new Transform( sphere.Origin, Quaternion.Identity, new Vector3( scl, scl, scl ) );
					}
					break;

				case ShapeEnum.Cylinder:
					{
						var tr = Transform.Value;
						var sclYZ = Math.Max( tr.Scale.Y, tr.Scale.Z );
						item.Transform = new Transform( tr.Position, tr.Rotation, new Vector3( tr.Scale.X, sclYZ, sclYZ ) );
					}
					break;
				}

				context.FrameData.RenderSceneData.CutVolumes.Add( item );
			}

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayVolumes ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					//if( context2.displayVolumesCounter < context2.displayVolumesMax )
					//{
					//	context2.displayVolumesCounter++;

					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.CanSelectColor;
					else
						color = ProjectSettings.Get.SceneShowVolumeColor;

					if( color.Alpha != 0 )
					{
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						RenderShape( context2 );
					}

					//}
				}
			}
		}

	}
}
