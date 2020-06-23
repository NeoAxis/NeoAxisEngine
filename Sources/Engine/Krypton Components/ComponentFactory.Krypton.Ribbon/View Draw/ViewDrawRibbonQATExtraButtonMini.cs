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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;

namespace ComponentFactory.Krypton.Ribbon
{
    /// <summary>
    /// Positions the quick access toolbar extra button for the minibar in the caption.
    /// </summary>
    internal class ViewDrawRibbonQATExtraButtonMini : ViewDrawRibbonQATExtraButton
    {
		#region Static Fields
		private static readonly int MINI_BUTTON_HEIGHT = DpiHelper.Default.ScaleValue(22);
        private static readonly int MINI_BUTTON_OFFSET = DpiHelper.Default.ScaleValue(24);

        private static readonly int MINI_BUTTON_HEIGHT_OFFICE2016 = DpiHelper.Default.ScaleValue(26);
        private static readonly int MINI_BUTTON_OFFSET_OFFICE2016 = DpiHelper.Default.ScaleValue(30);
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ViewDrawRibbonQATExtraButtonMini class.
        /// </summary>
        /// <param name="ribbon">Reference to owning ribbon control.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public ViewDrawRibbonQATExtraButtonMini(KryptonRibbon ribbon,
                                                NeedPaintHandler needPaint)
            : base(ribbon, needPaint)
        {
        }

        /// <summary>
        /// Obtains the String representation of this instance.
        /// </summary>
        /// <returns>User readable name of the instance.</returns>
        public override string ToString()
        {
            // Return the class name and instance identifier
            return "ViewDrawRibbonQATExtraButtonMini:" + Id;
        }
        #endregion

        #region Layout
        /// <summary>
        /// Perform a layout of the elements.
        /// </summary>
        /// <param name="context">Layout context.</param>
        public override void Layout(ViewLayoutContext context)
        {
            Debug.Assert(context != null);

            Rectangle clientRect = context.DisplayRectangle;

            // For the minibar we have to position ourself at bottom of available area

            if (_ribbon.RibbonShape == PaletteRibbonShape.Office2016)
            {
                clientRect.Y = clientRect.Bottom - 1 - MINI_BUTTON_OFFSET_OFFICE2016;
                clientRect.Height = MINI_BUTTON_HEIGHT_OFFICE2016;
            }
            else
            {
                clientRect.Y = clientRect.Bottom - 1 - MINI_BUTTON_OFFSET;
                clientRect.Height = MINI_BUTTON_HEIGHT;
            }

            // Use modified size to position base class and children
            context.DisplayRectangle = clientRect;

            // Let children be layed out inside border area
            base.Layout(context);

            // Put back the original display value now we have finished
            context.DisplayRectangle = ClientRectangle;
        }
        #endregion
    }
}