#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public interface IHCDropDownButton
	{
		KryptonDropButton Button { get; }
	}

	public partial class HCGridDropDownButton : EUserControl, IHCDropDownButton
	{
		public HCGridDropDownButton()
		{
			InitializeComponent();

			if( EditorAPI2.DarkTheme )
				kryptonDropButton.Images.Common = Properties.Resources.DropDownButton_Dark;
			else
				kryptonDropButton.Images.Common = Properties.Resources.DropDownButton;

			kryptonDropButton.Location = new Point( 0, DpiHelper.Default.ScaleValue( 3 ) );
			kryptonDropButton.AutoSize = false;
			kryptonDropButton.Height = Math.Max( DpiHelper.Default.ScaleValue( 18 ), kryptonDropButton.PreferredSize.Height );
		}

		public KryptonDropButton Button
		{
			get { return kryptonDropButton; }
		}

		//protected override void OnResize( EventArgs e )
		//{
		//	base.OnResize( e );

		//	if( Parent != null )
		//		Width = Parent.Width;
		//}
	}
}

#endif