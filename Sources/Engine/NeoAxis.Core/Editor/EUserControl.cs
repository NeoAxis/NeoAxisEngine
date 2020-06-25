// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using System.IO;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Provides an empty control that can be used to create other controls. It differs from the base <see cref="UserControl"/>in that the <see cref="OnDestroy"/> method provides.
	/// </summary>
	public partial class EUserControl : UserControl
	{
		bool destroyed;
		bool? isDesignerHosted; //contains information about design mode state

		/////////////////////////////////////////

		public EUserControl()
		{
			InitializeComponent();
		}

		protected override void WndProc( ref Message m )
		{
			const int WM_DESTROY = 0x0002;

			switch( m.Msg )
			{
			case WM_DESTROY:
				Destroy();
				break;
			}

			base.WndProc( ref m );
		}

		protected virtual void OnDestroy()
		{
			destroyed = true;
		}

		void Destroy()
		{
			OnDestroy();
		}

		[Browsable( false )]
		public bool Destroyed
		{
			get { return destroyed; }
		}

		/// <summary>
		/// The DesignMode property does not correctly tell you if
		/// you are in design mode.  IsDesignerHosted is a corrected
		/// version of that property.
		/// (see https://connect.microsoft.com/VisualStudio/feedback/details/553305
		/// and https://stackoverflow.com/a/2693338/238419 )
		/// </summary>
		[Browsable( false )]
		public bool IsDesignerHosted
		{
			get
			{
				if( isDesignerHosted.HasValue )
					return isDesignerHosted.Value;
				else
				{
					if( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
					{
						isDesignerHosted = true;
						return true;
					}
					Control ctrl = this;
					while( ctrl != null )
					{
						if( ( ctrl.Site != null ) && ctrl.Site.DesignMode )
						{
							isDesignerHosted = true;
							return true;
						}
						ctrl = ctrl.Parent;
					}
					isDesignerHosted = false;
					return false;
				}
			}
		}
	}
}