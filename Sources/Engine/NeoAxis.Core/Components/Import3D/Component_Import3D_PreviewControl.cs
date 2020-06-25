// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	public partial class Component_Import3D_PreviewControl : PreviewControlWithViewport
	{
		Component_Import3D displayObject;
		long displayObjectVersion;

		bool onlyOneMaterial;
		Component_Mesh materialPreviewMeshUsed;
		Reference<Component_Image> previewEnvironmentUsed;
		Component skybox;

		//

		public Component_Import3D_PreviewControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			CreateScene( false );
			UpdateDisplayObject();
			Scene.Enabled = true;
			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			//check update preview environment
			{
				var env = onlyOneMaterial ? ProjectSettings.Get.GetMaterialPreviewEnvironment() : new Reference<Component_Image>();
				if( !previewEnvironmentUsed.Equals( env ) )
					UpdatePreviewEnvironment();
			}
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			UpdateDisplayObject();

			//if( Scene != null )
			//{
			//	foreach( var obj in Scene.GetComponents<Component_ObjectInSpace>( false, true, true ) )
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

			var import = ObjectForPreview as Component_Import3D;
			if( import != null )
			{
				if( displayObject == null || displayObjectVersion != import.VersionForPreviewDisplay || materialPreviewMeshUsed != ProjectSettings.Get.MaterialPreviewMesh.Value )
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

					materialPreviewMeshUsed = ProjectSettings.Get.MaterialPreviewMesh.Value;
				}
			}
		}

		void UpdatePreviewEnvironment()
		{
			var env = onlyOneMaterial ? ProjectSettings.Get.GetMaterialPreviewEnvironment() : new Reference<Component_Image>();

			previewEnvironmentUsed = env;

			if( env.ReferenceSpecified )
			{
				var type = MetadataManager.GetType( "NeoAxis.Component_Skybox" );
				if( type != null )
				{
					if( skybox == null )
						skybox = Scene.CreateComponent( type );
					skybox.PropertySet( "Cubemap", env );
				}
			}
			else
			{
				skybox?.Dispose();
				skybox = null;
			}
		}
	}
}
