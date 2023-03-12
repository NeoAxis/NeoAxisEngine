#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Base class for forms in the editor.
	/// </summary>
	public partial class EngineForm : KryptonForm, ControlDoubleBufferComposited.IDoubleBufferComposited
	{
		public EngineForm()
		{
			InitializeComponent();
		}

		//ControlDoubleBufferComposited.IDoubleBufferComposited
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

#endif