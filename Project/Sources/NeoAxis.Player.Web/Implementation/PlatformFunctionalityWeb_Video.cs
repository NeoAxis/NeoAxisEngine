// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using NeoAxis.Player.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb
	{
		public override List<Vector2I> GetVideoModes()
		{
			return new List<Vector2I>();

			//return new List<Vector2I>() { GetScreenSize() };
		}

		public override bool ChangeVideoMode( Vector2I mode )
		{
			return true;

			//if( mode == GetScreenSize() )
			//	return true;
			//return false;
		}

		public override void RestoreVideoMode()
		{
		}

		//public override void SetGamma( float value )
		//{
		//}

		public override void ProcessChangingVideoMode()
		{
			var fullscreen = EngineApp.WindowedMode != WindowedModeEnum.Windowed;
			Task.Run( async () =>
			{
				await Interop.SetFullscreenAsync( fullscreen );
			} );
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
