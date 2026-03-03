// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class GateTypePreview : CanvasBasedPreview
	{
		bool needRecreateInstance;
		Gate objectInSpace;
		int createdVersionOfType;

		//

		public GateType GateType
		{
			get { return ObjectOfPreview as GateType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Gate>( enabled: false );
			objectInSpace.GateType = GateType;
			objectInSpace.Enabled = true;

			createdVersionOfType = GateType.Version;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( GateType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );
			}
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( createdVersionOfType != GateType.Version )
				needRecreateInstance = true;

			if( needRecreateInstance )
			{
				CreateObject();
				needRecreateInstance = false;
			}
		}
	}
}
#endif