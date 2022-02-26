// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2012. All rights reserved.
//	The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 17/267 Nepean Hwy, 
//  Seaford, Vic 3198, Australia and are supplied subject to licence terms.
// 
//
// *****************************************************************************

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    internal class KryptonNeoAxisRenderer : KryptonProfessionalRenderer
    {
        #region Static Fields
        private static readonly int _gripSquare = 2;
        private static readonly int _gripSize = 3;
        private static readonly int _gripMove = 4;
        private static readonly int _gripLines = 3;
        private static readonly int _checkInset = 1;
        //private static readonly float _contextCheckTickThickness = 1.6f;
        //private static readonly float _cutItemMenu = 1.7f;
        private static readonly Color _disabled = Color.FromArgb(167, 167, 167);

        #endregion

        #region Identity
        /// <summary>
        /// Initialise a new instance of the KryptonOffice2010Renderer class.
        /// </summary>
        /// <param name="kct">Source for text colors.</param>
        public KryptonNeoAxisRenderer(KryptonColorTable kct)
            : base(kct)
        {
        }
        #endregion

        #region OnRenderArrow
        /// <summary>
        /// Raises the RenderArrow event. 
        /// </summary>
        /// <param name="e">An ToolStripArrowRenderEventArgs containing the event data.</param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            // Cannot paint a zero sized area
            if ((e.ArrowRectangle.Width > 0) && (e.ArrowRectangle.Height > 0))
            {
                // Create a path that is used to fill the arrow
                using (GraphicsPath arrowPath = CreateArrowPath(e.Item, e.ArrowRectangle, e.Direction))
                {
                    using (SolidBrush arrowBrush = new SolidBrush(e.Item.Enabled ? KCT.MenuItemText : _disabled))
                        e.Graphics.FillPath(arrowBrush, arrowPath);
                }
            }
        }
        #endregion

        #region OnRenderButtonBackground
        /// <summary>
        /// Raises the RenderButtonBackground event. 
        /// </summary>
        /// <param name="e">An ToolStripItemRenderEventArgs containing the event data.</param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);

            ToolStripButton item = e.Item as ToolStripButton;
            if (item.Pressed)
            {
                using (Pen p = new Pen(KCT.ButtonPressedBorder))
                {
                    Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                    e.Graphics.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
        }
        #endregion

        #region OnRenderItemCheck
        /// <summary>
        /// Raises the RenderItemCheck event. 
        /// </summary>
        /// <param name="e">An ToolStripItemImageRenderEventArgs containing the event data.</param>
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            // Staring size of the checkbox is the image rectangle
            Rectangle imageRect = e.ImageRectangle;

            // Make the border of the check box 1 pixel bigger on all sides, as a minimum
            //imageRect.Inflate(1, 1);

            // Can we extend upwards?
            if (imageRect.Top > _checkInset)
            {
                int diff = imageRect.Top - _checkInset;
                imageRect.Y -= diff;
                imageRect.Height += diff;
            }

            // Can we extend downwards?
            if (imageRect.Height <= (e.Item.Bounds.Height - (_checkInset * 2)))
            {
                int diff = e.Item.Bounds.Height - (_checkInset * 2) - imageRect.Height;
                imageRect.Height += diff;
            }

            //using (Pen borderPen = new Pen(Color.Yellow))
            //    e.Graphics.DrawRectangle(borderPen, imageRect);

            // If there is not an image, then we can draw the tick, square etc...
            if (e.Item.Image == null)
            {
                // Extract the check state from the item
                if (e.Item is ToolStripMenuItem item)
                {
                    //Rectangle imageRect = e.ImageRectangle;
                    Image image = null;
                    if (item.CheckState == CheckState.Checked)
                        image = KCT.Palette.GetContextMenuCheckedImage();
                    else if (item.CheckState == CheckState.Indeterminate)
                        image = KCT.Palette.GetContextMenuIndeterminateImage();

                    if (imageRect != Rectangle.Empty && image != null)
                    {
                        if (!e.Item.Enabled)
                            image = CreateDisabledImage(image);

                        e.Graphics.DrawImage(image, imageRect.X + (imageRect.Width - image.Width) / 2,
                             imageRect.Y + (imageRect.Height - image.Height) / 2,
                            image.Width, image.Height);
                    }
                }
            }
        }
        #endregion

        #region OnRenderItemText
        /// <summary>
        /// Raises the RenderItemText event. 
        /// </summary>
        /// <param name="e">A ToolStripItemTextRenderEventArgs that contains the event data.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if ((e.ToolStrip is ToolStrip) || 
                (e.ToolStrip is ContextMenuStrip) ||
                (e.ToolStrip is ToolStripDropDownMenu))
            {
                if (!e.Item.Enabled)
                    e.TextColor = _disabled;
                else
                {
                    if ((e.ToolStrip is MenuStrip) && !e.Item.Pressed && !e.Item.Selected)
                        e.TextColor = KCT.MenuStripText;
                    else if (e.ToolStrip is MenuStrip)
                        e.TextColor = KCT.MenuItemText;
                    else if ((e.ToolStrip is StatusStrip) && !e.Item.Pressed && !e.Item.Selected)
                        e.TextColor = KCT.StatusStripText;
                    else if ((e.ToolStrip is StatusStrip) && !e.Item.Pressed && e.Item.Selected)
                        e.TextColor = KCT.MenuItemText;
                    else if ((e.ToolStrip is ToolStrip) && !e.Item.Pressed && e.Item.Selected)
                        e.TextColor = KCT.MenuItemText;
                    else if ((e.ToolStrip is ContextMenuStrip) && !e.Item.Pressed && !e.Item.Selected)
                        e.TextColor = KCT.MenuItemText;
                    else if (e.ToolStrip is ToolStripDropDownMenu)
                        e.TextColor = KCT.MenuItemText;
                    else if ((e.Item is ToolStripButton) && (((ToolStripButton)e.Item).Checked))
                        e.TextColor = KCT.MenuItemText;
                    else
                        e.TextColor = KCT.ToolStripText;
                }

                // Status strips under XP cannot use clear type because it ends up being cut off at edges
                if ((e.ToolStrip is StatusStrip) && (Environment.OSVersion.Version.Major < 6))
                    base.OnRenderItemText(e);
                else
                {
                    using (GraphicsTextHint clearTypeGridFit = new GraphicsTextHint(e.Graphics, TextRenderingHint.ClearTypeGridFit))
                        base.OnRenderItemText(e);
                }
            }
            else
                base.OnRenderItemText(e);
        }
        #endregion

        #region OnRenderItemImage
        /// <summary>
        /// Raises the RenderItemImage event. 
        /// </summary>
        /// <param name="e">An ToolStripItemImageRenderEventArgs containing the event data.</param>
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            // We only override the image drawing for context menus
            if ((e.ToolStrip is ContextMenuStrip) ||
                (e.ToolStrip is ToolStripDropDownMenu))
            {
                if (e.Image != null)
                {
                    if (e.Item.Enabled)
                        e.Graphics.DrawImage(e.Image, e.ImageRectangle);
                    else
                    {
                        using (ImageAttributes attribs = new ImageAttributes())
                        {
                            attribs.SetColorMatrix(CommonHelper.MatrixDisabled);

                            // Draw using the disabled matrix to make it look disabled
                            e.Graphics.DrawImage(e.Image, e.ImageRectangle,
                                                 0, 0, e.Image.Width, e.Image.Height,
                                                 GraphicsUnit.Pixel, attribs);
                        }
                    }
                }
            }
            else
            {
                base.OnRenderItemImage(e);
            }
        }
        #endregion

        #region OnRenderSeparator
        /// <summary>
        /// Raises the RenderSeparator event. 
        /// </summary>
        /// <param name="e">An ToolStripSeparatorRenderEventArgs containing the event data.</param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            DrawSeparatorInternal(e.Graphics, e.Item, e.Vertical, e.Item.Bounds, KCT.SeparatorDark);
        }
        #endregion

        #region OnRenderStatusStripSizingGrip
        /// <summary>
        /// Raises the RenderStatusStripSizingGrip event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            using (SolidBrush darkBrush = new SolidBrush(Color.Red))
            {
                // Do we need to invert the drawing edge?
                bool rtl = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

                // Find vertical position of the lowest grip line
                int y = e.AffectedBounds.Bottom - _gripSize * 2;

                // Draw three lines of grips
                for (int i = _gripLines; i >= 1; i--)
                {
                    // Find the rightmost grip position on the line
                    int x = (rtl ? e.AffectedBounds.Left + 1 :
                                   e.AffectedBounds.Right - _gripSize * 2);

                    // Draw grips from right to left on line
                    for (int j = 0; j < i; j++)
                    {
                        // Just the single grip glyph
                        DrawGripGlyph(e.Graphics, x, y, darkBrush);

                        // Move left to next grip position
                        x -= (rtl ? -_gripMove : _gripMove);
                    }

                    // Move upwards to next grip line
                    y -= _gripMove;
                }
            }
        }
        #endregion

        #region OnRenderToolStripBorder
        /// <summary>
        /// Raises the RenderToolStripBorder event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if ((e.ToolStrip is ContextMenuStrip) ||
                (e.ToolStrip is ToolStripDropDownMenu))
            {
                // If there is a connected area to be drawn
                if (!e.ConnectedArea.IsEmpty)
                    using (SolidBrush excludeBrush = new SolidBrush(KCT.ToolStripDropDownBackground))
                        e.Graphics.FillRectangle(excludeBrush, e.ConnectedArea);

                using (Pen borderPen = new Pen(KCT.MenuBorder))
                {
                    var rect = e.AffectedBounds;
                    rect.Width--;
                    rect.Height--;

                    // Draw the border area second, so any overlapping gives it priority
                    e.Graphics.DrawRectangle(borderPen, rect);
                }
            }
            else if (e.ToolStrip is StatusStrip)
            {
                // Draw two lines at top of the status strip
                using (Pen darkBorder = new Pen(KCT.ToolStripBorder),
                           lightBorder = new Pen(KCT.SeparatorLight))
                {
                    e.Graphics.DrawLine(darkBorder, 0, 0, e.ToolStrip.Width - 1, 0);
                    e.Graphics.DrawLine(lightBorder, 0, 1, e.ToolStrip.Width - 1, 1);
                }
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }
        #endregion

        #region OnRenderOverflowButtonBackground

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            // TODO: remove highlight 
            base.OnRenderOverflowButtonBackground(e);
        }
        #endregion

        #region Implementation

        private void DrawGripGlyph(Graphics g,
                                   int x,
                                   int y,
                                   Brush darkBrush)
        {
            g.FillRectangle(darkBrush, x, y, _gripSquare, _gripSquare);
        }


        private void DrawSeparatorInternal(Graphics g, ToolStripItem item, bool vertical, Rectangle bounds, Color darkColor)
        {
            if (item is ToolStripSeparator)
            {
                if (vertical)
                {
                    if (!item.IsOnDropDown)
                    {
                        // center so that it matches office
                        bounds.Y += 1;
                        bounds.Height = Math.Max(0, bounds.Height - 3);
                    }
                }
                else
                {
                    // offset after the image margin
                    if (item.GetCurrentParent() is ToolStripDropDownMenu dropDownMenu)
                    {
                        if (dropDownMenu.RightToLeft == RightToLeft.No)
                        {
                            // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                            // like the line meets up with the text).
                            bounds.X += dropDownMenu.Padding.Left - 2;
                            bounds.Width = dropDownMenu.Width - bounds.X;
                        }
                        else
                        {
                            // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                            // like the line meets up with the text).
                            bounds.X += 2;
                            bounds.Width = dropDownMenu.Width - bounds.X - dropDownMenu.Padding.Right;

                        }
                    }
                }
            }

            using (Pen foreColorPen = new Pen(darkColor))
            {
                if (vertical)
                    g.DrawLine(foreColorPen, bounds.Width / 2, bounds.Top, bounds.Width / 2, bounds.Bottom - 1);
                else
                    g.DrawLine(foreColorPen, bounds.Left, bounds.Height / 2, bounds.Right - 1, bounds.Height / 2);
            }
        }

        //private static GraphicsPath CreateBorderPath(Rectangle rect,
        //                                             Rectangle exclude,
        //                                             float cut)
        //{
        //    // If nothing to exclude, then use quicker method
        //    if (exclude.IsEmpty)
        //        return CreateBorderPath(rect, cut);

        //    // Drawing lines requires we draw inside the area we want
        //    rect.Width--;
        //    rect.Height--;

        //    // Create an array of points to draw lines between
        //    List<PointF> pts = new List<PointF>();

        //    float l = rect.X;
        //    float t = rect.Y;
        //    float r = rect.Right;
        //    float b = rect.Bottom;
        //    float x0 = rect.X + cut;
        //    float x3 = rect.Right - cut;
        //    float y0 = rect.Y + cut;
        //    float y3 = rect.Bottom - cut;
        //    float cutBack = (cut == 0f ? 1 : cut);

        //    // Does the exclude intercept the top line
        //    if ((rect.Y >= exclude.Top) && (rect.Y <= exclude.Bottom))
        //    {
        //        float x1 = exclude.X - 1 - cut;
        //        float x2 = exclude.Right + cut;

        //        if (x0 <= x1)
        //        {
        //            pts.Add(new PointF(x0, t));
        //            pts.Add(new PointF(x1, t));
        //            pts.Add(new PointF(x1 + cut, t - cutBack));
        //        }
        //        else
        //        {
        //            x1 = exclude.X - 1;
        //            pts.Add(new PointF(x1, t));
        //            pts.Add(new PointF(x1, t - cutBack));
        //        }

        //        if (x3 > x2)
        //        {
        //            pts.Add(new PointF(x2 - cut, t - cutBack));
        //            pts.Add(new PointF(x2, t));
        //            pts.Add(new PointF(x3, t));
        //        }
        //        else
        //        {
        //            x2 = exclude.Right;
        //            pts.Add(new PointF(x2, t - cutBack));
        //            pts.Add(new PointF(x2, t));
        //        }
        //    }
        //    else
        //    {
        //        pts.Add(new PointF(x0, t));
        //        pts.Add(new PointF(x3, t));
        //    }

        //    pts.Add(new PointF(r, y0));
        //    pts.Add(new PointF(r, y3));
        //    pts.Add(new PointF(x3, b));
        //    pts.Add(new PointF(x0, b));
        //    pts.Add(new PointF(l, y3));
        //    pts.Add(new PointF(l, y0));

        //    // Create path using a simple set of lines that cut the corner
        //    GraphicsPath path = new GraphicsPath();

        //    // Add a line between each set of points
        //    for (int i = 1; i < pts.Count; i++)
        //        path.AddLine(pts[i - 1], pts[i]);

        //    // Add a line to join the last to the first
        //    path.AddLine(pts[pts.Count - 1], pts[0]);

        //    return path;
        //}

        //private static GraphicsPath CreateBorderPath(Rectangle rect, float cut)
        //{
        //    // Drawing lines requires we draw inside the area we want
        //    rect.Width--;
        //    rect.Height--;

        //    // Create path using a simple set of lines that cut the corner
        //    GraphicsPath path = new GraphicsPath();
        //    path.AddLine(rect.Left + cut, rect.Top, rect.Right - cut, rect.Top);
        //    path.AddLine(rect.Right - cut, rect.Top, rect.Right, rect.Top + cut);
        //    path.AddLine(rect.Right, rect.Top + cut, rect.Right, rect.Bottom - cut);
        //    path.AddLine(rect.Right, rect.Bottom - cut, rect.Right - cut, rect.Bottom);
        //    path.AddLine(rect.Right - cut, rect.Bottom, rect.Left + cut, rect.Bottom);
        //    path.AddLine(rect.Left + cut, rect.Bottom, rect.Left, rect.Bottom - cut);
        //    path.AddLine(rect.Left, rect.Bottom - cut, rect.Left, rect.Top + cut);
        //    path.AddLine(rect.Left, rect.Top + cut, rect.Left + cut, rect.Top);
        //    return path;
        //}

        //private GraphicsPath CreateInsideBorderPath(Rectangle rect, float cut)
        //{
        //    // Adjust rectangle to be 1 pixel inside the original area
        //    rect.Inflate(-1, -1);

        //    // Now create a path based on this inner rectangle
        //    return CreateBorderPath(rect, cut);
        //}

        //private GraphicsPath CreateInsideBorderPath(Rectangle rect,
        //                                            Rectangle exclude,
        //                                            float cut)
        //{
        //    // Adjust rectangle to be 1 pixel inside the original area
        //    rect.Inflate(-1, -1);

        //    // Now create a path based on this inner rectangle
        //    return CreateBorderPath(rect, exclude, cut);
        //}

        //private GraphicsPath CreateClipBorderPath(Rectangle rect, float cut)
        //{
        //    // Clipping happens inside the rect, so make 1 wider and taller
        //    rect.Width++;
        //    rect.Height++;

        //    // Now create a path based on this inner rectangle
        //    return CreateBorderPath(rect, cut);
        //}

        //private GraphicsPath CreateClipBorderPath(Rectangle rect,
        //                                          Rectangle exclude,
        //                                          float cut)
        //{
        //    // Clipping happens inside the rect, so make 1 wider and taller
        //    rect.Width++;
        //    rect.Height++;

        //    // Now create a path based on this inner rectangle
        //    return CreateBorderPath(rect, exclude, cut);
        //}

        //TODO: need scale ?
        private GraphicsPath CreateArrowPath(ToolStripItem item,
                                             Rectangle rect,
                                             ArrowDirection direction)
        {
            int x, y;

            // Find the correct starting position, which depends on direction
            if ((direction == ArrowDirection.Left) || (direction == ArrowDirection.Right))
            {
                x = rect.Right - (rect.Width - 4) / 2;
                y = rect.Y + rect.Height / 2;
            }
            else
            {
                x = rect.X + rect.Width / 2;
                y = rect.Bottom - (rect.Height - 3) / 2;

                // The drop down button is position 1 pixel incorrectly when in RTL
                if ((item is ToolStripDropDownButton) && (item.RightToLeft == RightToLeft.Yes))
                    x++;
            }

            // Create triangle using a series of lines
            GraphicsPath path = new GraphicsPath();

            switch (direction)
            {
                case ArrowDirection.Right:
                    path.AddLine(x, y, x - 4, y - 4);
                    path.AddLine(x - 4, y - 4, x - 4, y + 4);
                    path.AddLine(x - 4, y + 4, x, y);
                    break;
                case ArrowDirection.Left:
                    path.AddLine(x - 4, y, x, y - 4);
                    path.AddLine(x, y - 4, x, y + 4);
                    path.AddLine(x, y + 4, x - 4, y);
                    break;
                case ArrowDirection.Down:
                    path.AddLine(x + 3f, y - 3f, x - 2f, y - 3f);
                    path.AddLine(x - 2f, y - 3f, x, y);
                    path.AddLine(x, y, x + 3f, y - 3f);
                    break;
                case ArrowDirection.Up:
                    path.AddLine(x + 3f, y, x - 3f, y);
                    path.AddLine(x - 3f, y, x, y - 4f);
                    path.AddLine(x, y - 4f, x + 3f, y);
                    break;
            }

            return path;
        }

        //private GraphicsPath CreateTickPath(Rectangle rect)
        //{
        //    // Get the center point of the rect
        //    int x = rect.X + rect.Width / 2;
        //    int y = rect.Y + rect.Height / 2;

        //    GraphicsPath path = new GraphicsPath();
        //    path.AddLine(x - 4, y, x - 2, y + 4);
        //    path.AddLine(x - 2, y + 4, x + 3, y - 5);
        //    return path;
        //}

        //private GraphicsPath CreateIndeterminatePath(Rectangle rect)
        //{
        //    // Get the center point of the rect
        //    int x = rect.X + rect.Width / 2;
        //    int y = rect.Y + rect.Height / 2;

        //    GraphicsPath path = new GraphicsPath();
        //    path.AddLine(x - 3, y, x, y - 3);
        //    path.AddLine(x, y - 3, x + 3, y);
        //    path.AddLine(x + 3, y, x, y + 3);
        //    path.AddLine(x, y + 3, x - 3, y);
        //    return path;
        //}
        #endregion
    }
}
