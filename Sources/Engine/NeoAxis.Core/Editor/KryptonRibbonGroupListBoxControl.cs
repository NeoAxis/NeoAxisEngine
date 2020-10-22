using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class KryptonRibbonGroupListBoxControl : EUserControl
	{
		public KryptonRibbonGroupListBoxControl()
		{
			InitializeComponent();

			if( kryptonLabel1.Height < kryptonLabel1.PreferredSize.Height )
				kryptonLabel1.Height = kryptonLabel1.PreferredSize.Height;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}
	}
}
