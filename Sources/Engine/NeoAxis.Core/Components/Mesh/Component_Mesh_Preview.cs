// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_Mesh_Preview : CanvasBasedPreview
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );

			Component_Mesh mesh = ObjectOfPreview as Component_Mesh;
			if( mesh != null )
			{
				Component_MeshInSpace objInSpace = scene.CreateComponent<Component_MeshInSpace>();
				objInSpace.Mesh = mesh;
			}

			scene.Enabled = true;

			SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
		}
	}
}
