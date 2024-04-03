#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	partial class HCItemProjectRibbonAndToolbarActionsForm : EUserControl
	{
		public HCItemProjectRibbonAndToolbarActionsForm()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			toolStrip1.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();

			//double distance = 22.0 * EditorAPI.DPIScale;
			//kryptonSplitContainer2.Panel1MinSize = (int)distance;
			//kryptonSplitContainer2.SplitterDistance = (int)distance;
		}

		private void HCItemProjectRibbonAndToolbarActionsForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//update toolstrip sizes
			{
				var dpiScale = Math.Min( EditorAPI2.DPIScale, 2 );

				void UpdateSize( ToolStripItem item )
				{
					item.Size = new Size( (int)( 20 * dpiScale ), (int)( 20 * dpiScale + 2 ) );
					//item.Size = new Size( (int)( width * EditorAPI.DPIScale + 2 ), (int)( 20 * EditorAPI.DPIScale + 2 ) );
				}

				toolStrip1.Padding = new Padding( (int)dpiScale );
				toolStrip1.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );
				kryptonSplitContainer2.SplitterDistance = (int)( 21 * dpiScale + 2 + (int)dpiScale - 1 );

				foreach( var item in toolStrip1.Items )
				{
					var button = item as ToolStripButton;
					if( button != null )
						UpdateSize( button );
				}
			}
		}

	}
}

#endif