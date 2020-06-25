// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Character2D_PreviewControl : Component_ObjectInSpace_PreviewControl
	{
		double lastUpdateTime;
		Component_Character2D instanceInScene;

		//

		public Component_Character2D_PreviewControl()
		{
			InitializeComponent();
		}

		public Component_Character2D Character
		{
			get { return ObjectForPreview as Component_Character2D; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Character != null && Character.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
			}
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//update
			if( instanceInScene != null && Time.Current > lastUpdateTime + 0.1 )
			{
				CreateObject();
				lastUpdateTime = Time.Current;
			}
		}

		void CreateObject()
		{
			instanceInScene?.Dispose();

			instanceInScene = (Component_Character2D)Character.Clone();
			Scene.AddComponent( instanceInScene );
		}
	}
}
