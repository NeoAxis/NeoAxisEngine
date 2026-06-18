// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class CharacterTypePreviewImage : PreviewImageGenerator
	{
		public CharacterTypePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var Character = ObjectOfPreview as CharacterType;

			//create scene
			{
				var scene = CreateScene( false );
				scene.Enabled = true;
			}

			//create object
			{
				var objectInSpace = Scene.CreateComponent<Character>( enabled: false );
				objectInSpace.CharacterType = Character;
				objectInSpace.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 1.5 );
		}
	}
}