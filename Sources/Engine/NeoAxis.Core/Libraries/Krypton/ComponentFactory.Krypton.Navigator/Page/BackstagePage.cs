using Internal.ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internal.ComponentFactory.Krypton.Navigator
{
	/// <summary>
	/// Page class used inside visual containers.
	/// </summary>
	public class BackstagePage : KryptonPage
	{
		bool buttonClickOnDownDesigner = true;

		//

		/// <summary>
		/// Initialize a new instance of the KryptonPage class.
		/// </summary>
		public BackstagePage()
			: base( "Page", null )
		{
		}

		/// <summary>
		/// Initialize a new instance of the KryptonPage class.
		/// </summary>
		/// <param name="text">Initial text.</param>
		public BackstagePage( string text )
			: base( text, null, null )
		{
		}


		/// <summary>
		/// Initialize a new instance of the KryptonPage class.
		/// </summary>
		/// <param name="text">Initial text.</param>
		/// <param name="uniqueName">Initial unique name.</param>
		public BackstagePage( string text, string uniqueName )
			: base( text, null, uniqueName )
		{
		}

		/// <summary>
		/// Initialize a new instance of the KryptonPage class.
		/// </summary>
		/// <param name="text">Initial text.</param>
		/// <param name="imageSmall">Initial small image.</param>
		/// <param name="uniqueName">Initial unique name.</param>
		public BackstagePage( string text, Image imageSmall, string uniqueName )
			: base( text, imageSmall, uniqueName )
		{
		}

        /// <summary>
        /// 
        /// </summary>
		[DefaultValue( true )]
		public bool ButtonClickOnDownDesigner
		{
			get { return buttonClickOnDownDesigner; }
			set { buttonClickOnDownDesigner = value; }
		}

        /// <summary>
        /// 
        /// </summary>
		public override bool ButtonClickOnDown
		{
			get { return buttonClickOnDownDesigner; }
		}
	}
}
