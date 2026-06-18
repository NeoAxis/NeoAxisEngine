// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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