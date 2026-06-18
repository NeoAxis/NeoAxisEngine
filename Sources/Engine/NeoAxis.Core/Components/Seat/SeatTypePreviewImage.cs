// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SeatTypePreviewImage : PreviewImageGenerator
	{
		public SeatTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Seat = ObjectOfPreview as SeatType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Seat>( enabled: false );
				objectInSpace.SeatType = Seat;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}
