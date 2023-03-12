#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class MaterialPreview : PreviewControlWithViewport
	{
		Mesh previewMeshUsed;
		Mesh defaultMesh;

		Reference<ImageComponent> previewEnvironmentUsed;
		ColorValuePowered previewEnvironmentMultiplierUsed;
		double previewEnvironmentAffectLightingUsed = -1;
		Skybox skybox;

		//

		public MaterialPreview()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			Material material = ObjectOfPreview as Material;

			//create scene
			{
				var scene = CreateScene( false );

				var meshInSpace = scene.CreateComponent<MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.ReplaceMaterial = material;

				UpdatePreviewMesh();
				UpdatePreviewEnvironment();

				scene.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			//check update preview
			{
				var mesh = ProjectSettings.Get.Preview.MaterialPreviewMesh.Value;
				if( previewMeshUsed != mesh )
				{
					UpdatePreviewMesh();
					SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace() );
					//needCameraUpdate = true;
				}
			}

			UpdatePreviewEnvironment();
		}

		void UpdatePreviewMesh()
		{
			var mesh = ProjectSettings.Get.Preview.MaterialPreviewMesh.Value;

			previewMeshUsed = mesh;

			if( mesh == null )
			{
				if( defaultMesh == null )
				{
					//sphere if null
					defaultMesh = Scene.CreateComponent<Mesh>( enabled: false );
					var sphere = defaultMesh.CreateComponent<MeshGeometry_Sphere>();
					sphere.SegmentsHorizontal = 64;
					sphere.SegmentsVertical = 64;
					defaultMesh.Enabled = true;
				}
				mesh = defaultMesh;
			}

			var meshInSpace = (MeshInSpace)Scene.GetComponent( "Mesh In Space" );
			meshInSpace.Mesh = mesh;
		}

		void UpdatePreviewEnvironment()
		{
			var env = ProjectSettings.Get.Preview.GetMaterialPreviewEnvironment();
			var multiplier = ProjectSettings.Get.Preview.MaterialPreviewEnvironmentMultiplier.Value;
			var affect = ProjectSettings.Get.Preview.MaterialPreviewEnvironmentAffectLighting.Value;

			if( !previewEnvironmentUsed.Equals( env ) || previewEnvironmentMultiplierUsed != multiplier || previewEnvironmentAffectLightingUsed != affect )
			{
				previewEnvironmentUsed = env;
				previewEnvironmentMultiplierUsed = multiplier;
				previewEnvironmentAffectLightingUsed = affect;

				if( env.Value != null )
				{
					if( skybox == null )
						skybox = Scene.CreateComponent<Skybox>();
					skybox.Cubemap = env;
					skybox.Multiplier = multiplier;
					skybox.AffectLighting = affect;
				}
				else
				{
					skybox?.Dispose();
					skybox = null;
				}
			}
		}
	}
}

#endif