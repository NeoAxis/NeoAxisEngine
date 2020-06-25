// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	class ContentBrowserDropSink : SimpleDropSink
	{
		public ContentBrowserDropSink()
		{
			CanDropBetween = true;
			FeedbackColor = Color.Black;
		}

#if !ANDROID

		protected override void DrawBetweenLine( Graphics g, int x1, int y1, int x2, int y2 )
		{
			//if( ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left )
			{
				x1 += 3;
				x2 -= 3;
			}

			using( Pen p = new Pen( this.FeedbackColor, 3.0f ) )
				g.DrawLine( p, x1, y1, x2, y2 );
		}

		protected override void DrawFeedbackBackgroundTarget( Graphics g, System.Drawing.Rectangle bounds )
		{
			float penWidth = 12.0f;
			var r = bounds;
			r.Inflate( (int)-penWidth / 2, (int)-penWidth / 2 );
			using( Pen p = new Pen( Color.FromArgb( 128, this.FeedbackColor ), penWidth ) )
				g.DrawRectangle( p, r );
		}

		protected override void DrawFeedbackItemTarget( Graphics g, System.Drawing.Rectangle bounds )
		{
#if HIGHLIGHT_ITEM_AT_DRAG
			if( this.DropTargetItem == null )
				return;

			var r = this.CalculateDropTargetRectangle( this.DropTargetItem, this.DropTargetSubItemIndex );

			//if( ColumnIsPrimary && CellHorizontalAlignment == HorizontalAlignment.Left )
			{
				r.X += 3;
				r.Width -= 3;
			}

			using( SolidBrush b = new SolidBrush( Color.FromArgb( 48, this.FeedbackColor ) ) )
				g.FillRectangle( b, r );
			//using( Pen p = new Pen( this.FeedbackColor, 3.0f ) )
			//	g.DrawRectangle( p, r );
#endif
		}

#endif //!ANDROID

	}
}
