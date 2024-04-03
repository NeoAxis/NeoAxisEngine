#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class MeshPreviewImage : PreviewImageGenerator
	{
		public MeshPreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var scene = CreateScene( false );

			var mesh = ObjectOfPreview as Mesh;
			if( mesh != null )
			{
				var objInSpace = scene.CreateComponent<MeshInSpace>();
				objInSpace.Mesh = mesh;
			}

			scene.Enabled = true;

			var distanceScale = 2.6;

			SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), distanceScale );// 1.7 );

			if( mesh != null && mesh.EditorCameraTransform != null )
			{
				var tr = mesh.EditorCameraTransform;
				CameraDistance = ( tr.Position - CameraLookTo ).Length();// * distanceScale;
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}
	}
}

#endif