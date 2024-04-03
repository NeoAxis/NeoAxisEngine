#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Import3DPreviewImage : PreviewImageGenerator
	{
		public Import3DPreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			CreateScene( false );

			var import = ObjectOfPreview as Import3D;
			if( import != null )
				import.CreateForPreviewDisplay( Scene, out _, out _ );

			Scene.Enabled = true;

			var distanceScale = 2.6;

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), distanceScale );// 1.7 );

			var mesh = import.GetComponent( "Mesh" ) as Mesh;
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