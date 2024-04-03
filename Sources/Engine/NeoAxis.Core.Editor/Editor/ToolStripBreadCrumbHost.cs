#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Windows.Forms;
using System.ComponentModel;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms.Design;
using System.Drawing;

namespace NeoAxis.Editor
{
	[DesignerCategory( "code" )]
	[ToolStripItemDesignerAvailability( ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip )]
	class ToolStripBreadCrumbHost : ToolStripControlHost
	{
		public KryptonBreadCrumb BreadCrumb
		{
			get { return Control as KryptonBreadCrumb; }
		}

		public ToolStripBreadCrumbHost()
			: base( CreateControlInstance() )
		{
		}

		static Control CreateControlInstance()
		{
			KryptonBreadCrumb bc = new KryptonBreadCrumb();
			bc.StateCommon.Border.Draw = InheritBool.False;

			bc.StateCommon.BreadCrumb.Content.Padding = new Padding( 0 );

			////WORKAROUND:
			///*	Now we do not scale toolstrip at high dpi, its height is always equal to 27.
			//	However, BreadCrumb and the text inside it and font are scaled according to the general rules
			//	to prevent the toolstrip height increasing I reduce the BreadCrumb text (content) padding from 3 to 2.
			//	tested only at 125%
			//*/
			//if( DpiHelper.Default.DpiScaleFactor > 1.0 )
			//	bc.StateCommon.BreadCrumb.Content.Padding = new Padding( 2 );
			////

			//bc.MinimumSize = new Size( 100, 22 );

			return bc;
		}

		public override Size GetPreferredSize( Size constrainingSize )
		{
			if( DesignMode || IsOnOverflow || Owner.Orientation == Orientation.Vertical )
				return DefaultSize;

			int width = Owner.DisplayRectangle.Width;

			if( Owner.OverflowButton.Visible )
				width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;

			int springBoxCount = 0;
			foreach( ToolStripItem item in Owner.Items )
			{
				if( item.IsOnOverflow ) continue;

				if( item is ToolStripBreadCrumbHost )
				{
					springBoxCount++;
					width -= item.Margin.Horizontal;
				}
				else
				{
					width = width - item.Width - item.Margin.Horizontal;
				}
			}

			if( springBoxCount > 1 )
				width /= springBoxCount;

			// If the available width is less than the default width, use the
			// default width, forcing one or more items onto the overflow menu.
			//if( width < DefaultSize.Width )
			//	width = DefaultSize.Width;

			Size size = base.GetPreferredSize( constrainingSize );
			size.Width = width;
			return size;
		}
	}
}

#endif