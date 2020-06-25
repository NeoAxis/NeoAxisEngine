using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	//TODO: merge class with LabelEx ?

	/// <summary>
	/// Advanced KryptonTextBox for Grid. It may look like a label.
	/// </summary>
	[ToolboxItem( true )]
	//[ToolboxBitmap( typeof( KryptonLabelExt ), "ToolboxBitmaps.KryptonLabel.bmp" )]
	[DefaultProperty( "Text" )]
	[DefaultBindingProperty( "Text" )]
	[Description( "KryptonTextBox for Grid" )]
	[ClassInterface( ClassInterfaceType.AutoDispatch )]
	[ComVisible( true )]
	public class HCKryptonTextBox : KryptonTextBox
	{
		bool lookLikeLabel = false;
		string errorMessage = string.Empty;
		Color? commonBorderColor;

		[Browsable( true )]
		[EditorBrowsable( EditorBrowsableState.Always )]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		//TODO: rename prop? it not only "looks" but also "behaves" like a label.
		public bool LookLikeLabel
		{
			get
			{
				return lookLikeLabel;
			}
			set
			{
				lookLikeLabel = value;

				ReadOnly = value;
				//TODO: use Krypton themes system.
				StateCommon.Back.Color1 = GetResolvedPalette().GetBackColor1( lookLikeLabel ? PaletteBackStyle.PanelClient : PaletteBackStyle.InputControlStandalone, PaletteState.Normal );
				StateCommon.Border.Draw = lookLikeLabel ? InheritBool.False : InheritBool.True;
				TabStop = !lookLikeLabel;
			}
		}

		public void SetError( string message )
		{
			if( errorMessage == message )
				return;

			errorMessage = message;

			if( commonBorderColor == null )
				commonBorderColor = StateCommon.Border.Color1;

			if( !string.IsNullOrEmpty( errorMessage ) )
				StateCommon.Border.Color1 = Color.Red; // HARDCODED: use Krypton themes system.
			else
				StateCommon.Border.Color1 = commonBorderColor.Value;

			//TODO: add tooltip ?
		}
	}
}
