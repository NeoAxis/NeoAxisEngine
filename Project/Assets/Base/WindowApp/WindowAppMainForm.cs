using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class WindowAppMainForm : NeoAxis.UIControl
	{
		public void ButtonExit_Click(NeoAxis.UIButton sender)
		{
			EngineApp.NeedExit = true;
		}

		public void ButtonMessageBox_Click(NeoAxis.UIButton sender)
		{
			MessageBoxWindow.ShowInfo(this, "The text of the message.", "Message");
		}
	}
}