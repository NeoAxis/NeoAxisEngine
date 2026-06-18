// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class RegulatorTypePreviewImage : PreviewImageGenerator
	{
		public RegulatorTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Regulator = ObjectOfPreview as RegulatorType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Regulator>( enabled: false );
				objectInSpace.RegulatorType = Regulator;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 2.5 );
		}
	}
}
