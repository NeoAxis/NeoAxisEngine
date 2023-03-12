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
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel; 
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;

namespace Internal.ComponentFactory.Krypton.Docking
{
	/// <summary>
    /// Event arguments for the change in auto hidden page showing state.
	/// </summary>
	public class AutoHiddenShowingStateEventArgs : EventArgs
	{
		#region Instance Fields
        private KryptonPage _page;
        private DockingAutoHiddenShowState _state;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the AutoHiddenShowingStateEventArgs class.
		/// </summary>
        /// <param name="page">Page for which state has changed.</param>
        /// <param name="state">New state of the auto hidden page.</param>
        public AutoHiddenShowingStateEventArgs(KryptonPage page, DockingAutoHiddenShowState state)
		{
            _page = page;
            _state = state;
		}
        #endregion

		#region Public
        /// <summary>
        /// Gets the page that has had the state change.
        /// </summary>
        public KryptonPage Page
        {
            get { return _page; }
        }

        /// <summary>
        /// Gets the new state of the auto hidden page.
        /// </summary>
        public DockingAutoHiddenShowState NewState
        {
            get { return _state; }
        }
        #endregion
    }
}

#endif