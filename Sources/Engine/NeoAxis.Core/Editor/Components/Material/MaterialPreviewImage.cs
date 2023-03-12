#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class MaterialPreviewImage : PreviewImageGenerator
	{
		public MaterialPreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var material = ObjectOfPreview as Material;

			//create scene
			{
				var scene = CreateScene( false );

				var meshInSpace = scene.CreateComponent<MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.ReplaceMaterial = material;

				scene.Enabled = true;
			}

			UpdatePreviewMesh();
			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );// 1.6 );
		}

		void UpdatePreviewMesh()
		{
			var mesh = ProjectSettings.Get.Preview.MaterialPreviewMesh.Value;

			if( mesh == null )
			{
				//sphere if null
				mesh = Scene.CreateComponent<Mesh>( enabled: false );
				var sphere = mesh.CreateComponent<MeshGeometry_Sphere>();
				sphere.SegmentsHorizontal = 64;
				sphere.SegmentsVertical = 64;
				mesh.Enabled = true;
			}

			var meshInSpace = (MeshInSpace)Scene.GetComponent( "Mesh In Space" );
			meshInSpace.Mesh = mesh;
		}

	}
}

#endif