// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class Component_Material_PreviewControl : PreviewControlWithViewport
	{
		Component_Mesh previewMeshUsed;
		Component_Mesh defaultMesh;
		Reference<Component_Image> previewEnvironmentUsed;
		Component skybox;

		//

		public Component_Material_PreviewControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			Component_Material material = ObjectOfPreview as Component_Material;

			//create scene
			{
				var scene = CreateScene( false );

				var meshInSpace = scene.CreateComponent<Component_MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.ReplaceMaterial = material;

				UpdatePreviewMesh();
				UpdatePreviewEnvironment();

				scene.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			//check update preview
			{
				var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;
				if( previewMeshUsed != mesh )
				{
					UpdatePreviewMesh();
					SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );
					//needCameraUpdate = true;
				}
			}

			//check update preview environment
			{
				var env = ProjectSettings.Get.GetMaterialPreviewEnvironment();
				if( !previewEnvironmentUsed.Equals( env ) )
					UpdatePreviewEnvironment();
			}
		}

		void UpdatePreviewMesh()
		{
			var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;

			previewMeshUsed = mesh;

			if( mesh == null )
			{
				if( defaultMesh == null )
				{
					//sphere if null
					defaultMesh = Scene.CreateComponent<Component_Mesh>( enabled: false );
					var sphere = defaultMesh.CreateComponent<Component_MeshGeometry_Sphere>();
					sphere.SegmentsHorizontal = 64;
					sphere.SegmentsVertical = 64;
					defaultMesh.Enabled = true;
				}
				mesh = defaultMesh;
			}

			var meshInSpace = (Component_MeshInSpace)Scene.GetComponent( "Mesh In Space" );
			meshInSpace.Mesh = mesh;
		}

		void UpdatePreviewEnvironment()
		{
			var env = ProjectSettings.Get.GetMaterialPreviewEnvironment();

			previewEnvironmentUsed = env;

			if( env.Value != null )
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
