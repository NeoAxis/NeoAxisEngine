#if !DEPLOY
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
using System.Data;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
    /// <summary>
    /// Displays a shadow effect for a visual popup.
    /// </summary>
    public class VisualPopupSimpleShadow : Form, IVisualPopupShadow
    {
        #region Identity
        static VisualPopupSimpleShadow()
        {
        }

        /// <summary>
        /// Initialize a new instance of the VisualPopupShadow class. 
		/// </summary>
        public VisualPopupSimpleShadow()
        {
            // Update form properties so we do not have a border and do not show
            // in the task bar. We draw the background in Magenta and set that as
            // the transparency key so it is a see through window.
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Opacity = 0.75f;
        }

        #endregion

        #region Public
        /// <summary>
        /// Show the popup using the provided rectangle as the screen rect.
        /// </summary>
        /// <param name="screenRect">Screen rectangle for showing the popup.</param>
        public virtual void Show(Rectangle screenRect)
        {
            // Update the screen position
            Location = screenRect.Location;
            ClientSize = screenRect.Size;

            // Show the window without activating it (i.e. do not take focus)
            PI.ShowWindow(this.Handle, (short)PI.SW_SHOWNOACTIVATE);
        }

        #endregion

        #region Protected
        /// <summary>
        /// Gets the creation parameters.
        /// </summary>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Parent = IntPtr.Zero;
                cp.Style |= PI.WS_POPUP;
                cp.ExStyle |= PI.WS_EX_TOPMOST + PI.WS_EX_TOOLWINDOW;
                if (OSFeature.IsPresent(SystemParameter.DropShadow))
                cp.ClassStyle |= PI.CS_DROPSHADOW;
                return cp;
            }
        }

        #endregion
    }
}

#endif