// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using Internal;

namespace NeoAxis
{
	partial class PlatformFunctionalityAndroid : PlatformFunctionality
	{
		public override List<Vector2I> GetVideoModes()
		{
			return new List<Vector2I>() { GetScreenSize() };
		}

		public override bool ChangeVideoMode( Vector2I mode )
		{
			if( mode == GetScreenSize() )
				return true;
			return false;
		}

		public override void RestoreVideoMode()
		{
		}

		public override void SetGamma( float value )
		{
		}

		public override void ProcessChangingVideoMode()
		{
		}

		public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
		{
			var result = new List<SystemSettings.DisplayInfo>();

			RectangleI area = new RectangleI( Vector2I.Zero, GetScreenSize() );
			SystemSettings.DisplayInfo info = new SystemSettings.DisplayInfo( "Primary", area, area, true );
			result.Add( info );

			return result;
		}
	}
}
