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
	public partial class Component_Mesh_PreviewControl : PreviewControlWithViewport
	{
		public Component_Mesh_PreviewControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = CreateScene( false );

			Component_Mesh mesh = ObjectForPreview as Component_Mesh;
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
