// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class VehiclePreviewImage : ObjectInSpacePreviewImage
	{
		public VehiclePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if( Scene != null )
				SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1 );// 1.6 );
		}
	}
}
#endif