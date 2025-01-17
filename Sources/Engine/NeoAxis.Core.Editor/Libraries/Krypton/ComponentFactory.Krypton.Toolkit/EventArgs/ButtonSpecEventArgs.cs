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
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	/// Details for button specification related events.
	/// </summary>
	public class ButtonSpecEventArgs : EventArgs
	{
		#region Instance Fields
		private ButtonSpec _spec;
		private int _index;
		#endregion

		#region Identity
		/// <summary>
        /// Initialize a new instance of the ButtonSpecEventArgs class.
		/// </summary>
        /// <param name="spec">Button spec effected by event.</param>
		/// <param name="index">Index of page in the owning collection.</param>
        public ButtonSpecEventArgs(ButtonSpec spec, int index)
		{
            Debug.Assert(spec != null);
			Debug.Assert(index >= 0);

			// Remember parameter details
            _spec = spec;
			_index = index;
		}
		#endregion

		#region Public
		/// <summary>
		/// Gets the navigator button spec associated with the event.
		/// </summary>
		public ButtonSpec ButtonSpec
		{
			get { return _spec; }
		}

		/// <summary>
		/// Gets the index of ButtonSpec associated with the event.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}
		#endregion
	}
}

#endif