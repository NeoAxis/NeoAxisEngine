#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing;

namespace NeoAxis.Editor
{
	[DesignerCategory( "code" )]
	[ToolStripItemDesignerAvailability( ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip )]
	class ToolStripTextBoxHost : ToolStripControlHost
	{
		public EngineTextBox TextBox
		{
			get { return Control as EngineTextBox; }
		}

		public ToolStripTextBoxHost()
			: base( CreateControlInstance() )
		{
		}

		static Control CreateControlInstance()
		{
			var textBox = new EngineTextBox();
			textBox.Multiline = false;
			textBox.WordWrap = false;

			return textBox;
		}

		//public override Size GetPreferredSize( Size constrainingSize )
		//{
		//	if( DesignMode || IsOnOverflow || Owner.Orientation == Orientation.Vertical )
		//		return DefaultSize;

		//	int width = Owner.DisplayRectangle.Width;

		//	if( Owner.OverflowButton.Visible )
		//		width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;

		//	int springBoxCount = 0;
		//	foreach( ToolStripItem item in Owner.Items )
		//	{
		//		if( item.IsOnOverflow ) continue;

		//		if( item is ToolStripTextBoxHost )
		//		{
		//			springBoxCount++;
		//			width -= item.Margin.Horizontal;
		//		}
		//		else
		//		{
		//			width = width - item.Width - item.Margin.Horizontal;
		//		}
		//	}

		//	if( springBoxCount > 1 )
		//		width /= springBoxCount;

		//	// If the available width is less than the default width, use the
		//	// default width, forcing one or more items onto the overflow menu.
		//	//if( width < DefaultSize.Width )
		//	//	width = DefaultSize.Width;

		//	Size size = base.GetPreferredSize( constrainingSize );
		//	size.Width = width;
		//	return size;
		//}
	}
}

#endif