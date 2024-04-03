#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IDropDownHolder
	{
		// bool ProcessHotKeys { get; set; } // for example
		event System.EventHandler HolderClosed;
		bool Visible { get; set; }
		void Show( Control openerControl );
		void Close();
		void Close( bool commitChanges );

		// internal
		bool ProcessResizing( ref Message m );
	}
}

#endif