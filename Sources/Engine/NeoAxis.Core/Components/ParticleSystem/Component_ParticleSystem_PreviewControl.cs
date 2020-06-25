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
	public partial class Component_ParticleSystem_PreviewControl : PreviewControlWithViewport
	{
		public Component_ParticleSystem_PreviewControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = CreateScene( false );

			var particleSystem = ObjectForPreview as Component_ParticleSystem;
			if( particleSystem != null )
			{
				var inSpace = scene.CreateComponent<Component_ParticleSystemInSpace>();
				inSpace.ParticleSystem = particleSystem;
			}

			scene.Enabled = true;

			//!!!!
			SetCameraByBounds( new Bounds( -5, -5, -5, 5, 5, 5 ) );
			//SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
		}
	}
}
