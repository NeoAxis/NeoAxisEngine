// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The component to clip geometry by volume in real-time.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Volumes\Cut Volume", 0 )]
	public class CutVolume : ObjectInSpace
	{
		/// <summary>
		/// The shape of the volume.
		/// </summary>
		[DefaultValue( CutVolumeShape.Box )]
		public Reference<CutVolumeShape> Shape
		{
			get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
			set { if( _shape.BeginSet( this, ref value ) ) { try { ShapeChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _shape.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		public event Action<CutVolume> ShapeChanged;
		ReferenceField<CutVolumeShape> _shape = CutVolumeShape.Box;

		/// <summary>
		/// Whether to cut the scene image.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CutScene
		{
			get { if( _cutScene.BeginGet() ) CutScene = _cutScene.Get( this ); return _cutScene.value; }
			set { if( _cutScene.BeginSet( this, ref value ) ) { try { CutSceneChanged?.Invoke( this ); } finally { _cutScene.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CutScene"/> property value changes.</summary>
		public event Action<CutVolume> CutSceneChanged;
		ReferenceField<bool> _cutScene = true;

		/// <summary>
		/// Whether to cut the shadows.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CutShadows
		{
			get { if( _cutShadows.BeginGet() ) CutShadows = _cutShadows.Get( this ); return _cutShadows.value; }
			set { if( _cutShadows.BeginSet( this, ref value ) ) { try { CutShadowsChanged?.Invoke( this ); } finally { _cutShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CutShadows"/> property value changes.</summary>
		public event Action<CutVolume> CutShadowsChanged;
		ReferenceField<bool> _cutShadows = true;

		/// <summary>
		/// Whether to cut the Simple 3D Renderer.
		/// </summary>
		[DefaultValue( false )]
		[DisplayName( "Cut Simple 3D Renderer" )]
		public Reference<bool> CutSimple3DRenderer
		{
			get { if( _cutSimple3DRenderer.BeginGet() ) CutSimple3DRenderer = _cutSimple3DRenderer.Get( this ); return _cutSimple3DRenderer.value; }
			set { if( _cutSimple3DRenderer.BeginSet( this, ref value ) ) { try { CutSimple3DRendererChanged?.Invoke( this ); } finally { _cutSimple3DRenderer.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CutSimple3DRenderer"/> property value changes.</summary>
		public event Action<CutVolume> CutSimple3DRendererChanged;
		ReferenceField<bool> _cutSimple3DRenderer = false;

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
			case CutVolumeShape.Box:
				newBounds = new SpaceBounds( GetBox().ToBounds() );
				break;

			case CutVolumeShape.Sphere:
				newBounds = new SpaceBounds( GetSphere() );
				break;

			case CutVolumeShape.Cylinder:
				newBounds = new SpaceBounds( GetCylinder().ToBounds() );
				break;
			}
		}

		protected virtual void RenderShape( RenderingContext context )
		{
			switch( Shape.Value )
			{
			case CutVolumeShape.Box:
				context.viewport.Simple3DRenderer.AddBox( GetBox() );
				break;

			case CutVolumeShape.Sphere:
				context.viewport.Simple3DRenderer.AddSphere( GetSphere() );
				break;

			case CutVolumeShape.Cylinder:
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

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var item = new RenderingPipeline.RenderSceneData.CutVolumeItem();
				item.Shape = Shape;
				if( CutScene )
					item.Flags |= CutVolumeFlags.CutScene;
				if( CutShadows )
					item.Flags |= CutVolumeFlags.CutShadows;
				if( CutSimple3DRenderer )
					item.Flags |= CutVolumeFlags.CutSimple3DRenderer;
				//item.CutScene = CutScene;
				//item.CutShadows = CutShadows;
				//item.CutSimple3DRenderer = CutSimple3DRenderer;

				switch( item.Shape )
				{
				case CutVolumeShape.Box:
					item.Transform = Transform;
					break;

				case CutVolumeShape.Sphere:
					{
						var sphere = GetSphere();
						var scl = sphere.Radius * 2;
						item.Transform = new Transform( sphere.Center, Quaternion.Identity, new Vector3( scl, scl, scl ) );
					}
					break;

				case CutVolumeShape.Cylinder:
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
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = ( context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayVolumes ) || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
				if( show )
				{
					//if( context2.displayVolumesCounter < context2.displayVolumesMax )
					//{
					//	context2.displayVolumesCounter++;

					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.CanSelectColor;
					else
						color = ProjectSettings.Get.Colors.SceneShowVolumeColor;

					if( color.Alpha != 0 )
					{
						context.Owner.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						RenderShape( context2 );
					}

					//}
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Volume" );
		}
	}
}
