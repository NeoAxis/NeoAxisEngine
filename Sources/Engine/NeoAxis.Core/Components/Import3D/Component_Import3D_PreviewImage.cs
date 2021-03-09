// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Import3D_PreviewImage : PreviewImageGenerator
	{
		public Component_Import3D_PreviewImage()
		{
		}

		protected override void Update()
		{
			CreateScene( false );

			var import = ObjectOfPreview as Component_Import3D;
			if( import != null )
				import.CreateForPreviewDisplay( Scene, out _, out _ );

			Scene.Enabled = true;

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );// 1.7 );

			var mesh = import.GetComponent( "Mesh" ) as Component_Mesh;
			if( mesh != null && mesh.EditorCameraTransform != null )
			{
				var tr = mesh.EditorCameraTransform;
				CameraDistance = ( tr.Position - CameraLookTo ).Length();
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}
	}
}
