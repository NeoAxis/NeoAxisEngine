#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Import3DPreview : CanvasBasedPreview
	{
		Import3D displayObject;
		long displayObjectVersion;

		bool onlyOneMaterial;
		Mesh materialPreviewMeshUsed;
		Reference<ImageComponent> previewEnvironmentUsed;
		Sky sky;

		//

		protected override void OnCreate()
		{
			base.OnCreate();

			CreateScene( false );
			UpdateDisplayObject();
			Scene.Enabled = true;
			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );

			var import = ObjectOfPreview as Import3D;
			var mesh = import?.GetComponent( "Mesh" ) as Mesh;
			if( mesh != null && mesh.EditorCameraTransform != null )
			{
				var tr = mesh.EditorCameraTransform;
				CameraInitialDistance = ( tr.Position - CameraLookTo ).Length();
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			//check update preview environment
			{
				var env = onlyOneMaterial ? ProjectSettings.Get.Preview.GetMaterialPreviewEnvironment() : new Reference<ImageComponent>();
				if( !previewEnvironmentUsed.Equals( env ) )
					UpdatePreviewEnvironment();
			}
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			UpdateDisplayObject();
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			//if( Scene != null )
			//{
			//	foreach( var obj in Scene.GetComponents<ObjectInSpace>( false, true, true ) )
			//	{
			//		if( obj.VisibleInHierarchy )
			//		{
			//			var b = obj.SpaceBounds;

			//			//!!!!
			//			viewport.DebugGeometry.SetColor( new ColorValue( 1, 1, 0, .3 ) );
			//			if( b.BoundingBox.HasValue )
			//				viewport.DebugGeometry.AddBounds( b.BoundingBox.Value );
			//			if( b.BoundingSphere.HasValue )
			//				viewport.DebugGeometry.AddSphere( b.BoundingSphere.Value );
			//		}
			//	}
			//}
		}

		void UpdateDisplayObject()
		{
			if( Scene == null )
				return;

			var import = ObjectOfPreview as Import3D;
			if( import != null )
			{
				if( displayObject == null || displayObjectVersion != import.VersionForPreviewDisplay || materialPreviewMeshUsed != ProjectSettings.Get.Preview.MaterialPreviewMesh.Value )
				{
					Scene.Enabled = false;

					if( displayObject != null )
					{
						displayObject.RemoveFromParent( false );
						displayObject.Dispose();
					}

					displayObject = import.CreateForPreviewDisplay( Scene, out onlyOneMaterial, out _ );
					displayObjectVersion = import.VersionForPreviewDisplay;

					Scene.Enabled = true;

					materialPreviewMeshUsed = ProjectSettings.Get.Preview.MaterialPreviewMesh.Value;
				}
			}
		}

		void UpdatePreviewEnvironment()
		{
			var env = onlyOneMaterial ? ProjectSettings.Get.Preview.GetMaterialPreviewEnvironment() : new Reference<ImageComponent>();

			previewEnvironmentUsed = env;

			if( env.ReferenceSpecified )
			{
				if( sky == null )
					sky = Scene.CreateComponent<Sky>();
				sky.Cubemap = env;
			}
			else
			{
				sky?.Dispose();
				sky = null;
			}
		}
	}
}

#endif