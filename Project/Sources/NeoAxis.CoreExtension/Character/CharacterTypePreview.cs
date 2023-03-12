// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class CharacterTypePreview : CanvasBasedPreview
	{
		Character objectInSpace;

		//

		public CharacterType CharacterType
		{
			get { return ObjectOfPreview as CharacterType; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Character>( enabled: false );
			objectInSpace.CharacterType = CharacterType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( CharacterType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );
			}
		}
	}
}
#endif