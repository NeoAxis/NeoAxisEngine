#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Docking;

namespace NeoAxis.Editor
{
	// KryptonDockableWorkspace with low profile square tabs
	public class LowProfileDockableWorkspace : KryptonDockableWorkspace
	{
		protected readonly TabStyle CellBarTabStyle = TabStyle.LowProfile;
		protected readonly TabBorderStyle CellBarTabBorderStyle = TabBorderStyle.SquareEqualMedium;

		protected override void NewCellInitialize( KryptonWorkspaceCell cell )
		{
			base.NewCellInitialize( cell );

			cell.Bar.TabStyle = CellBarTabStyle;
			cell.Bar.TabBorderStyle = CellBarTabBorderStyle;

			// Do not show any navigator level buttons
			cell.Button.CloseButtonDisplay = ButtonDisplay.Hide;
			cell.Button.ButtonDisplayLogic = ButtonDisplayLogic.Context;

			// Do not need the secondary header for header modes
			cell.Header.HeaderVisibleSecondary = false;
		}

		protected override void OnActiveCellChanged( ActiveCellChangedEventArgs e )
		{
			base.OnActiveCellChanged( e );

			// Ensure all but the newly selected cell have a lower profile appearance
			KryptonWorkspaceCell cell = FirstCell();
			while( cell != null )
			{
				if( e.NewCell != cell )
				{
					cell.Bar.TabStyle = CellBarTabStyle;
					cell.Bar.TabBorderStyle = CellBarTabBorderStyle;
				}
				cell = NextCell( cell );
			}

			// Ensure the newly selected cell has a lower profile appearance
			if( e.NewCell != null )
			{
				e.NewCell.Bar.TabStyle = CellBarTabStyle;
				e.NewCell.Bar.TabBorderStyle = CellBarTabBorderStyle;
			}
		}
	}
}

#endif