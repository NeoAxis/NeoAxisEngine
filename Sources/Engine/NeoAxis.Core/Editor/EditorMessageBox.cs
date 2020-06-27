// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class to work with MessageBox.
	/// </summary>
	public class EditorMessageBox
	{
		public static DialogResult ShowQuestion( string text, MessageBoxButtons buttons )
		{
			return KryptonMessageBox.Show( text, EngineInfo.OriginalName, buttons, MessageBoxIcon.Question );
			//return MessageBox.Show( text, EngineInfo.NameWithVersion, buttons, MessageBoxIcon.Question );
		}

		public static void ShowWarning( string text )
		{
			KryptonMessageBox.Show( text, EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Warning );
			//MessageBox.Show( text, EngineInfo.NameWithVersion, MessageBoxButtons.OK, MessageBoxIcon.Warning );
		}

		public static void ShowInfo( string text )
		{
			KryptonMessageBox.Show( text, EngineInfo.OriginalName, MessageBoxButtons.OK, MessageBoxIcon.Information );
			//MessageBox.Show( text, EngineInfo.NameWithVersion, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}
