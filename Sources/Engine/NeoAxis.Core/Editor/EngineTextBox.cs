#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Advanced KryptonTextBox for Grid. It may look like a label.
	/// </summary>
	[ToolboxItem( true )]
	//[ToolboxBitmap( typeof( KryptonLabelExt ), "ToolboxBitmaps.KryptonLabel.bmp" )]
	[DefaultProperty( "Text" )]
	[DefaultBindingProperty( "Text" )]
	[Description( "EngineTextBox" )]
	//[ClassInterface( ClassInterfaceType.AutoDispatch )]
	[ComVisible( true )]
	public class EngineTextBox : KryptonTextBox
	{
		bool likeLabel;
		string errorMessage = string.Empty;
		Color? commonBorderColor;

		[Browsable( true )]
		[EditorBrowsable( EditorBrowsableState.Always )]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		[DefaultValue( false )]
		public bool LikeLabel
		{
			get { return likeLabel; }
			set
			{
				if( likeLabel == value )
					return;
				likeLabel = value;

				ReadOnly = value;
				//TODO: use Krypton themes system.
				StateCommon.Back.Color1 = GetResolvedPalette().GetBackColor1( likeLabel ? PaletteBackStyle.PanelClient : PaletteBackStyle.InputControlStandalone, PaletteState.Normal );
				StateCommon.Border.Draw = likeLabel ? InheritBool.False : InheritBool.True;
				TabStop = !likeLabel;
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
				StateCommon.Border.Color1 = Color.Red;
			else
				StateCommon.Border.Color1 = commonBorderColor.Value;

			//TODO: add tooltip ?
		}
	}
}

#endif