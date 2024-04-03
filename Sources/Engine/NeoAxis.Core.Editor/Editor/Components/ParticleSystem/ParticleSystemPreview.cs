#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class ParticleSystemPreview : PreviewControlWithViewport
	{
		public ParticleSystemPreview()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var scene = CreateScene( false );

			var particleSystem = ObjectOfPreview as ParticleSystem;
			if( particleSystem != null )
			{
				var inSpace = scene.CreateComponent<ParticleSystemInSpace>();
				inSpace.ParticleSystem = particleSystem;
			}

			scene.Enabled = true;

			//!!!!
			SetCameraByBounds( new Bounds( -5, -5, -5, 5, 5, 5 ) );
			//SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
		}
	}
}

#endif