// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	partial class EditorForm
	{
		Component_ProjectSettings.RibbonAndToolbarActionsClass qatUpdatedForConfiguration;

		//

		void AddActionToQAT( EditorAction action )
		{
			var button = new KryptonRibbonQATButton();
			//action.createdQATButton = button;
			button.Enabled = false;

			button.Image = EditorAPI.GetImageForDispalyScale( action.GetImageSmall(), action.GetImageBig() );

			//!!!!
			//button.ShortcutKeys = action.shortcutKeys;

			button.Tag = action;
			//button.Tag = action.name;
			//!!!!
			button.Text = action.Name;

			//set tool tip
			var toolTip = EditorLocalization.Translate( "EditorAction.Name", action.Name );
			if( action.Description != "" )
				toolTip += "\n" + EditorLocalization.Translate( "EditorAction.Description", action.Description );
			var keysString = EditorActions.ConvertShortcutKeysToString( action.ShortcutKeys );
			if( keysString != "" )
				toolTip += " (" + keysString + ")";
			button.ToolTipBody = toolTip;

			if( action.ActionType == EditorAction.ActionTypeEnum.DropDown )
			{
				button.IsDropDownButton = true;
				button.KryptonContextMenu = action.DropDownContextMenu;
			}

			kryptonRibbon.QATButtons.Add( button );

			button.Click += QATButton_Click;
		}

		void QATButton_Click( object sender, EventArgs e )
		{
			var button = (KryptonRibbonQATButton)sender;

			var action = button.Tag as EditorAction;
			if( action != null )
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
		}

		void InitQAT()
		{
			qatInitialized = true;

			QATButtonsUpdate();
		}

		void QATButtonsUpdate()
		{
			if( qatInitialized )
			{
				bool needRepaint = false;
				IgnoreRepaint = true;

				try
				{
					QATButtonsCheckForRecreate( ref needRepaint );
					QATButtonsUpdateProperties( ref needRepaint );
				}
				finally
				{
					IgnoreRepaint = false;
					if( needRepaint )
						PerformNeedPaint( true );
				}
			}
		}

		void QATButtonsCheckForRecreate( ref bool needRepaint )
		{
			var config = ProjectSettings.Get.RibbonAndToolbarActions;
			if( qatUpdatedForConfiguration == null || !config.Equals( qatUpdatedForConfiguration ) )
			{
				qatUpdatedForConfiguration = config.Clone();

				kryptonRibbon.QATButtons.Clear();

				foreach( var actionItem in config.ToolbarActions )
				{
					var action = EditorActions.GetByName( actionItem.Name );
					if( action != null && action.QatSupport && actionItem.Enabled )
						AddActionToQAT( action );
				}

				needRepaint = true;
			}
		}

		void QATButtonsUpdateProperties( ref bool needRepaint )
		{
			foreach( KryptonRibbonQATButton button in kryptonRibbon.QATButtons )
			{
				if( button.Visible )
				{
					var action = button.Tag as EditorAction;
					if( action != null )
					{
						var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.RibbonQAT, action );
						if( button.Enabled != state.Enabled )
						{
							button.Enabled = state.Enabled;
							needRepaint = true;
						}
						if( button.Checked != state.Checked )
						{
							button.Checked = state.Checked;
							needRepaint = true;
						}
					}
				}
			}
		}
	}
}
