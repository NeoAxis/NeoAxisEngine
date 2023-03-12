// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The component to cull invisible geometry by volume in real-time.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Volumes\Occluder", 0 )]
	public class Occluder : ObjectInSpace
	{
		Transform cachedTransform;
		Vector3[] cachedBoxShapeVertices;
		int[] cachedBoxShapeIndices;

		///////////////////////////////////////////////

		///// <summary>
		///// The shape of the volume.
		///// </summary>
		//[DefaultValue( OccluderShape.Box )]
		//public Reference<OccluderShape> Shape
		//{
		//	get { if( _shape.BeginGet() ) Shape = _shape.Get( this ); return _shape.value; }
		//	set { if( _shape.BeginSet( ref value ) ) { try { ShapeChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _shape.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Shape"/> property value changes.</summary>
		//public event Action<Occluder> ShapeChanged;
		//ReferenceField<OccluderShape> _shape = OccluderShape.Box;

		///////////////////////////////////////////////

		public void GetBox( out Box box )
		{
			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			box = new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
		}

		public Box GetBox()
		{
			GetBox( out var box );
			return box;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			//switch( Shape.Value )
			//{
			//case OccluderShape.Box:
			GetBox( out var box );
			box.ToBounds( out var bounds );
			newBounds = new SpaceBounds( bounds );
			//	break;
			//}
		}

		protected virtual void RenderShape( RenderingContext context )
		{
			//switch( Shape.Value )
			//{
			//case OccluderShape.Box:
			GetBox( out var box );
			context.viewport.Simple3DRenderer.AddBox( box );
			//	break;
			//}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

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

		protected override Scene.SceneObjectFlags OnGetSceneObjectFlags()
		{
			//!!!!maybe in simulation no sense for visual
			return base.OnGetSceneObjectFlags() | Scene.SceneObjectFlags.Occluder;
		}

		//protected override bool OnOcclusionCullingDataContains()
		//{
		//	return true;
		//}

		protected override void OnOcclusionCullingDataGet( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem, OpenList<RenderingPipeline.OccluderItem> occluders )
		{
			base.OnOcclusionCullingDataGet( context, mode, modeGetObjectsItem, occluders );

			var tr = Transform.Value;

			if( cachedTransform == null || tr != cachedTransform )
			{
				GetBox( out var box );

				//!!!!slowly

				var worldMatrix = new Matrix4( box.Axis * Matrix3.FromScale( box.Extents * 2 ), box.Center );

				SimpleMeshGenerator.GenerateBox( new Vector3( 1, 1, 1 ), out Vector3[] vertices, out int[] indices );

				for( int n = 0; n < vertices.Length; n++ )
					vertices[ n ] = worldMatrix * vertices[ n ];

				cachedTransform = tr;
				cachedBoxShapeVertices = vertices;
				cachedBoxShapeIndices = indices;
			}

			var item = new RenderingPipeline.OccluderItem();
			item.Center = cachedTransform.Position;
			item.Vertices = cachedBoxShapeVertices;
			item.Indices = cachedBoxShapeIndices;
			occluders.Add( ref item );
		}
	}
}
