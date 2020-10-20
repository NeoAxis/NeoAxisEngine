// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Material_PreviewImage : PreviewImageGenerator
	{
		public Component_Material_PreviewImage()
		{
		}

		protected override void Update()
		{
			var material = ObjectOfPreview as Component_Material;

			//create scene
			{
				var scene = CreateScene( false );

				var meshInSpace = scene.CreateComponent<Component_MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.ReplaceMaterial = material;

				scene.Enabled = true;
			}

			UpdatePreviewMesh();
			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );// 1.6 );
		}

		void UpdatePreviewMesh()
		{
			var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;

			if( mesh == null )
			{
				//sphere if null
				mesh = Scene.CreateComponent<Component_Mesh>( enabled: false );
				var sphere = mesh.CreateComponent<Component_MeshGeometry_Sphere>();
				sphere.SegmentsHorizontal = 64;
				sphere.SegmentsVertical = 64;
				mesh.Enabled = true;
			}

			var meshInSpace = (Component_MeshInSpace)Scene.GetComponent( "Mesh In Space" );
			meshInSpace.Mesh = mesh;
		}

	}
}
