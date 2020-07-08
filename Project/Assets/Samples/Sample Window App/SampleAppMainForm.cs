using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;
using System.Windows.Forms;

namespace Project
{
	public class SampleAppMainForm : NeoAxis.UIControl
	{
        public void ButtonExit_Click(NeoAxis.UIButton sender)
        {
        	EngineApp.NeedExit = true;
        }

        public void ButtonSystemMessageBox_Click(NeoAxis.UIButton sender)
        {
			MessageBox.Show( "The text of the message.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }

        public void ButtonMessageBox_Click(NeoAxis.UIButton sender)
        {
			MessageBoxWindow.ShowInfo( this, "The text of the message.", "Message" );
        }
    }
}