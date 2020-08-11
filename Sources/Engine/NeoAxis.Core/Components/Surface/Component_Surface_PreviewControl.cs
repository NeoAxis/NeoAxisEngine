// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class Component_Surface_PreviewControl : PreviewControlWithViewport
	{
		double lastUpdateTime;

		//

		public Component_Surface_PreviewControl()
		{
			InitializeComponent();
		}

		public Component_Surface Surface
		{
			get { return ObjectOfPreview as Component_Surface; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//create scene
			if( Surface != null )
			{
				var scene = CreateScene( false );
				CreateObjects( scene );
				scene.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );
			//var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			//var bounds2 = new Bounds( bounds.GetCenter() );
			//bounds2.Expand( bounds.GetSize() / 4 );
			//SetCameraByBounds( bounds2 );
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//update scene
			if( Time.Current > lastUpdateTime + 0.1 )
			{
				if( Surface != null && Scene != null )
					CreateObjects( Scene );
			}
		}

		void CreateObjects( Component_Scene scene )
		{
			Component_SurfaceUtility.CreatePreviewObjects( scene, Surface );
			lastUpdateTime = Time.Current;
		}
	}
}
