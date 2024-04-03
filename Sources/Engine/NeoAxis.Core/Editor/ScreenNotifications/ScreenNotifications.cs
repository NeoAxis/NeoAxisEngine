//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis.Editor
{
	public static class ScreenNotifications
	{
		public interface IStickyNotificationItem 
		{
			void Close();
		}

		/////////////////////////////////////////

		public static void Show( string text, bool error = false )
		{
			EditorAssemblyInterface.Instance.ShowScreenNotification( text, error, false );
		}

		public static IStickyNotificationItem ShowSticky( string text, bool error = false )
		{
			return EditorAssemblyInterface.Instance.ShowScreenNotification( text, error, true );
		}
	}
}

//#endif