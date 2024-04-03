//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Message box of the editor.
	/// </summary>
	public class EditorMessageBox
	{
		public static EDialogResult ShowQuestion( string text, EMessageBoxButtons buttons, string caption = null )
		{
#if !DEPLOY
			if( EditorAssemblyInterface.Instance != null )
				return EditorAssemblyInterface.Instance.ShowQuestion( text, buttons, caption );
			//return (EDialogResult)KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, (MessageBoxButtons)buttons, MessageBoxIcon.Question );
#endif
			return EDialogResult.None;
		}

		public static void ShowWarning( string text, string caption = null )
		{
#if !DEPLOY
			if( EditorAssemblyInterface.Instance != null )
				EditorAssemblyInterface.Instance.ShowWarning( text, caption );
			//KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Warning );
#endif
		}

		public static void ShowInfo( string text, string caption = null )
		{
#if !DEPLOY
			if( EditorAssemblyInterface.Instance != null )
				EditorAssemblyInterface.Instance.ShowInfo( text, caption );
			//KryptonMessageBox.Show( text, caption ?? EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Information );
#endif
		}
	}
}
//#endif