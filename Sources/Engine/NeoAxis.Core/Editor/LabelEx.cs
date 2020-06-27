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
	/// <summary>
	/// Advanced Label with text selection and context menu copy abilities.
	/// </summary>
	[ToolboxItem( true )]
	//[ToolboxBitmap( typeof( KryptonLabelExt ), "ToolboxBitmaps.KryptonLabel.bmp" )]
	[DefaultProperty( "Text" )]
	[DefaultBindingProperty( "Text" )]
	[Description( "LabelEx" )]
	[ClassInterface( ClassInterfaceType.AutoDispatch )]
	[ComVisible( true )]
	public class LabelEx : KryptonTextBox
	{
		PaletteContentInheritRedirect _paletteCommonRedirect;
		LabelStyle _style;

		[Category( "Visuals" )]
		[Description( "Label style." )]
		public LabelStyle LabelStyle
		{
			get { return _style; }
			set
			{
				if( _style != value )
				{
					_style = value;
					SetLabelStyle( _style );
					PerformNeedPaint( true );
				}
			}
		}

		[Browsable( true )]
		[EditorBrowsable( EditorBrowsableState.Always )]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		[Category( "Behavior" )]
		[Description( "Controls whether the text in the edit control can be changed or not." )]
		[RefreshProperties( RefreshProperties.Repaint )]
		[DefaultValue( true )]
		public new bool ReadOnly
		{
			get { return base.ReadOnly; }
			set { base.ReadOnly = value; }
		}

		[DefaultValue( false )]
		public new bool TabStop
		{
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		public LabelEx()
		{
			_style = LabelStyle.NormalControl;
			_paletteCommonRedirect = new PaletteContentInheritRedirect( Redirector, PaletteContentStyle.LabelNormalControl );

			ReadOnly = true;
			TabStop = false;
			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				StateCommon.Back.Color1 = GetResolvedPalette().GetBackColor1( PaletteBackStyle.PanelClient, PaletteState.Normal );
				StateCommon.Border.Draw = InheritBool.False;
			}
		}

		protected virtual void SetLabelStyle( LabelStyle style )
		{
			if( !IsDisposed && !Disposing )
			{
				//TODO: support states ?
				_paletteCommonRedirect.Style = CommonHelper.ContentStyleFromLabelStyle( style );
				StateCommon.Content.Font = _paletteCommonRedirect.GetContentShortTextFont( PaletteState.Normal );
			}
		}
	}
}
