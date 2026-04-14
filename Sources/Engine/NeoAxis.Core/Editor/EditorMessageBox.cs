// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Message box of the editor.
	/// </summary>
	public class EditorMessageBox
	{
		public static EDialogResult ShowQuestion( string text, EMessageBoxButtons buttons, string caption = null )
		{
			if( EditorAssemblyInterface.Instance != null )
				return EditorAssemblyInterface.Instance.ShowQuestion( text, buttons, caption );
			return EDialogResult.None;
		}

		public static void ShowWarning( string text, string caption = null )
		{
			if( EditorAssemblyInterface.Instance != null )
				EditorAssemblyInterface.Instance.ShowWarning( text, caption );
		}

		public static void ShowInfo( string text, string caption = null )
		{
			if( EditorAssemblyInterface.Instance != null )
				EditorAssemblyInterface.Instance.ShowInfo( text, caption );
		}
	}
}
