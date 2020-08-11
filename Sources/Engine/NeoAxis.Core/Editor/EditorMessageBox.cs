// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Message box of the editor.
	/// </summary>
	public class EditorMessageBox
	{
		public static EDialogResult ShowQuestion( string text, EMessageBoxButtons buttons )
		{
			return (EDialogResult)KryptonMessageBox.Show( text, EngineInfo.OriginalName, (MessageBoxButtons)buttons, MessageBoxIcon.Question );
		}

		public static void ShowWarning( string text )
		{
			KryptonMessageBox.Show( text, EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Warning );
		}

		public static void ShowInfo( string text )
		{
			KryptonMessageBox.Show( text, EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}
