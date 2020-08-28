// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	partial class EditorForm
	{
		Component_ProjectSettings.RibbonAndToolbarActionsClass ribbonUpdatedForConfiguration;
		bool needRecreatedRibbonButtons;

		ESet<string> tabUsedKeyTips = new ESet<string>( new string[] { "P" } );

		//

		string alphabetNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		string GetTabKeyTip( string name )
		{
			foreach( var c in name + alphabetNumbers )
			{
				var s = c.ToString().ToUpper();
				if( s != " " && !tabUsedKeyTips.Contains( s ) )
				{
					tabUsedKeyTips.AddWithCheckAlreadyContained( s );
					return s;
				}
			}
			return "";
		}

		void RibbonSubGroupAddItemsRecursive( EditorRibbonDefaultConfiguration.Group subGroup, KryptonContextMenuCollection contextMenuItems )
		{
			var items = new List<KryptonContextMenuItemBase>();

			foreach( var child in subGroup.Children )
			{
				//!!!!impl
				////sub group
				//var subGroup = child as EditorRibbonDefaultConfiguration.Group;
				//if( subGroup != null )
				//{
				//	var tripple = new KryptonRibbonGroupTriple();
				//	ribbonGroup.Items.Add( tripple );

				//	var button = new KryptonRibbonGroupButton();
				//	//button.Tag = action;
				//	button.TextLine1 = subGroup.DropDownGroupText.Item1;
				//	button.TextLine2 = subGroup.DropDownGroupText.Item2;
				//	//button.ImageSmall = action.imageSmall;
				//	button.ImageLarge = subGroup.DropDownGroupImage;
				//	button.ToolTipBody = subGroup.Name;
				//	button.ButtonType = GroupButtonType.DropDown;

				//	button.KryptonContextMenu = new KryptonContextMenu();
				//	RibbonSubGroupAddItemsRecursive( subGroup, button.KryptonContextMenu.Items );

				//	tripple.Items.Add( button );
				//}

				//action
				var action = child as EditorAction;
				if( action == null )
				{
					var actionName = child as string;
					if( actionName != null )
						action = EditorActions.GetByName( actionName );
				}
				if( action != null )
				{
					EventHandler clickHandler = delegate ( object s, EventArgs e2 )
					{
						var item2 = (KryptonContextMenuItem)s;

						var action2 = item2.Tag as EditorAction;
						if( action2 != null )
							EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action2.Name );
					};

					var item = new KryptonContextMenuItem( action.GetContextMenuText(), null, clickHandler );
					//var item = new KryptonContextMenuItem( action.GetContextMenuText(), action.imageSmall, clickHandler );
					item.Tag = action;
					items.Add( item );
				}
				//separator
				else if( child == null )
					items.Add( new KryptonContextMenuSeparator() );
			}

			if( items.Count != 0 )
				contextMenuItems.Add( new KryptonContextMenuItems( items.ToArray() ) );
		}

		void InitRibbon()
		{
			kryptonRibbon.RibbonStrings.AppButtonKeyTip = "P";

			RibbonUpdate();
		}

		private void Button_Click( object sender, EventArgs e )
		{
			var control = (KryptonRibbonGroupButton)sender;

			var action = control.Tag as EditorAction;
			if( action != null )
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
		}

		private void Slider_ValueChanged( object sender, EventArgs e )
		{
			var control = (KryptonRibbonGroupSliderControl)sender;

			var action = control.Tag as EditorAction;
			if( action != null )
			{
				action.Slider.Value = control.GetValue();
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
			}
		}

		//private void ComboBox_SelectedIndexChanged( object sender, EventArgs e )
		//{
		//	var control = (KryptonRibbonGroupComboBox)sender;

		//	var action = control.Tag as EditorAction;
		//	if( action != null )
		//	{
		//		action.ComboBox.SelectedIndex = control.SelectedIndex;
		//		EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
		//	}
		//}

		private void ListBrowser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			var action = sender.Tag as EditorAction;
			if( action != null )
			{
				if( sender.SelectedItems.Length != 0 )
					action.ListBox.SelectedIndex = (int)sender.SelectedItems[ 0 ].Tag;
				else
					action.ListBox.SelectedIndex = -1;

				action.ListBox.LastSelectedIndexChangedByUser = selectedByUser;

				EditorAPI.EditorActionClick( EditorAction.HolderEnum.RibbonQAT, action.Name );
			}
		}

		void RibbonButtonsCheckForRecreate()
		{
			var config = ProjectSettings.Get.RibbonAndToolbarActions;
			if( ribbonUpdatedForConfiguration == null || !config.Equals( ribbonUpdatedForConfiguration ) || needRecreatedRibbonButtons )
			{
				ribbonUpdatedForConfiguration = config.Clone();
				needRecreatedRibbonButtons = false;

				ribbonLastSelectedTabTypeByUser_DisableUpdate = true;

				kryptonRibbon.RibbonTabs.Clear();

				foreach( var tabSettings in ProjectSettings.Get.RibbonAndToolbarActions.RibbonTabs )
				{
					if( !tabSettings.Enabled )
						continue;

					//can be null
					EditorRibbonDefaultConfiguration.Tab tab = null;
					if( tabSettings.Type == Component_ProjectSettings.RibbonAndToolbarActionsClass.TabItem.TypeEnum.Basic )
						tab = EditorRibbonDefaultConfiguration.GetTab( tabSettings.Name );

					var ribbonTab = new KryptonRibbonTab();
					ribbonTab.Tag = tab;

					if( tabSettings.Type == Component_ProjectSettings.RibbonAndToolbarActionsClass.TabItem.TypeEnum.Basic )
						ribbonTab.Text = EditorLocalization.Translate( "Ribbon.Tab", tabSettings.Name );
					else
						ribbonTab.Text = tabSettings.Name;

					ribbonTab.KeyTip = GetTabKeyTip( tabSettings.Name );

					kryptonRibbon.RibbonTabs.Add( ribbonTab );

					var usedKeyTips = new ESet<string>();

					string GetKeyTip( string name )
					{
						foreach( var c in name + alphabetNumbers )
						{
							var s = c.ToString().ToUpper();
							if( s != " " && !usedKeyTips.Contains( s ) )
							{
								usedKeyTips.AddWithCheckAlreadyContained( s );
								return s;
							}
						}
						return "";
					}

					foreach( var groupSettings in tabSettings.Groups )
					{
						if( !groupSettings.Enabled )
							continue;

						var ribbonGroup = new KryptonRibbonGroup();

						if( groupSettings.Type == Component_ProjectSettings.RibbonAndToolbarActionsClass.GroupItem.TypeEnum.Basic )
							ribbonGroup.TextLine1 = EditorLocalization.Translate( "Ribbon.Group", groupSettings.Name );
						else
							ribbonGroup.TextLine1 = groupSettings.Name;

						ribbonGroup.DialogBoxLauncher = false;//!!!!для группы Transform можно было бы в настройки снеппинга переходить
						ribbonTab.Groups.Add( ribbonGroup );

						foreach( var groupSettingsChild in groupSettings.Actions )
						{
							if( !groupSettingsChild.Enabled )
								continue;

							//sub group
							if( groupSettingsChild.Type == Component_ProjectSettings.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.SubGroupOfActions )
							{
								EditorRibbonDefaultConfiguration.Group subGroup = null;
								if( tab != null )
								{
									var group = tab.Groups.Find( g => g.Name == groupSettings.Name );
									if( group != null )
									{
										foreach( var child in group.Children )
										{
											var g = child as EditorRibbonDefaultConfiguration.Group;
											if( g != null && g.Name == groupSettingsChild.Name )
											{
												subGroup = g;
												break;
											}
										}
									}
								}

								if( subGroup != null )
								{
									var tripple = new KryptonRibbonGroupTriple();
									ribbonGroup.Items.Add( tripple );

									var button = new KryptonRibbonGroupButton();
									button.Tag = "SubGroup";
									//button.Tag = action;

									var str = subGroup.DropDownGroupText.Item1;
									if( subGroup.DropDownGroupText.Item2 != "" )
										str += "\n" + subGroup.DropDownGroupText.Item2;

									var str2 = EditorLocalization.Translate( "Ribbon.Action", str );
									var strs = str2.Split( new char[] { '\n' } );

									button.TextLine1 = strs[ 0 ];
									if( strs.Length > 1 )
										button.TextLine2 = strs[ 1 ];

									//button.TextLine1 = subGroup.DropDownGroupText.Item1;
									//button.TextLine2 = subGroup.DropDownGroupText.Item2;

									if( subGroup.DropDownGroupImageSmall != null )
										button.ImageSmall = subGroup.DropDownGroupImageSmall;
									else if( subGroup.DropDownGroupImageLarge != null )
										button.ImageSmall = EditorAction.ResizeImage( subGroup.DropDownGroupImageLarge, 16, 16 );
									button.ImageLarge = subGroup.DropDownGroupImageLarge;

									//EditorLocalization.Translate( "EditorAction.Description",

									if( !string.IsNullOrEmpty( subGroup.DropDownGroupDescription ) )
										button.ToolTipBody = EditorLocalization.Translate( "EditorAction.Description", subGroup.DropDownGroupDescription );
									else
										button.ToolTipBody = subGroup.Name;

									button.ButtonType = GroupButtonType.DropDown;
									button.ShowArrow = subGroup.ShowArrow;

									button.KryptonContextMenu = new KryptonContextMenu();
									RibbonSubGroupAddItemsRecursive( subGroup, button.KryptonContextMenu.Items );

									tripple.Items.Add( button );
								}
							}

							//action
							if( groupSettingsChild.Type == Component_ProjectSettings.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.Action )
							{
								var action = EditorActions.GetByName( groupSettingsChild.Name );

								if( action != null )
								{
									if( action.ActionType == EditorAction.ActionTypeEnum.Button || action.ActionType == EditorAction.ActionTypeEnum.DropDown )
									{
										//Button, DropDown

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupButton();

										//!!!!
										//control.ImageSmall = NeoAxis.Properties.Resources.Android_16;

										control.Tag = action;

										var str = action.RibbonText.Item1;
										if( action.RibbonText.Item2 != "" )
											str += "\n" + action.RibbonText.Item2;

										var str2 = EditorLocalization.Translate( "Ribbon.Action", str );
										var strs = str2.Split( new char[] { '\n' } );

										control.TextLine1 = strs[ 0 ];
										if( strs.Length > 1 )
											control.TextLine2 = strs[ 1 ];

										//control.TextLine1 = action.RibbonText.Item1;
										//control.TextLine2 = action.RibbonText.Item2;

										control.ImageSmall = action.GetImageSmall();
										control.ImageLarge = action.GetImageBig();
										control.ToolTipBody = action.ToolTip;
										control.KeyTip = GetKeyTip( action.Name );

										//_buttonType = GroupButtonType.Push;
										//_toolTipImageTransparentColor = Color.Empty;
										//_toolTipTitle = string.Empty;
										//_toolTipBody = string.Empty;
										//_toolTipStyle = LabelStyle.SuperTip;

										if( action.ActionType == EditorAction.ActionTypeEnum.DropDown )
										{
											control.ButtonType = GroupButtonType.DropDown;
											control.KryptonContextMenu = action.DropDownContextMenu;
										}

										control.Click += Button_Click;

										tripple.Items.Add( control );
									}
									else if( action.ActionType == EditorAction.ActionTypeEnum.Slider )
									{
										//Slider

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupSlider();
										control.Tag = action;
										control.ToolTipBody = action.ToolTip;

										control.Control.Size = new System.Drawing.Size( (int)( (float)control.Control.Size.Width * EditorAPI.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer2.Size = new System.Drawing.Size( (int)( (float)control.Control.kryptonSplitContainer2.Size.Width * EditorAPI.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer2.Panel1MinSize = (int)( (float)control.Control.kryptonSplitContainer2.Panel1MinSize * EditorAPI.DPIScale );
										control.Control.kryptonSplitContainer1.Panel2MinSize = (int)( (float)control.Control.kryptonSplitContainer1.Panel2MinSize * EditorAPI.DPIScale );
										control.Control.kryptonSplitContainer1.SplitterDistance = 10000;

										control.Control.kryptonLabel1.Text = EditorLocalization.Translate( "Ribbon.Action", action.RibbonText.Item1 );
										control.Control.Init( action.Slider.Minimum, action.Slider.Maximum, action.Slider.ExponentialPower );
										control.Control.SetValue( action.Slider.Value );

										control.Control.Tag = action;
										control.Control.ValueChanged += Slider_ValueChanged;

										tripple.Items.Add( control );
									}
									//else if( action.ActionType == EditorAction.ActionTypeEnum.ComboBox )
									//{
									//	//ComboBox

									//	var tripple = new KryptonRibbonGroupTriple();
									//	ribbonGroup.Items.Add( tripple );

									//	var control = new KryptonRibbonGroupComboBox();
									//	control.Tag = action;

									//	control.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
									//	foreach( var item in action.ComboBox.Items )
									//		control.Items.Add( item );

									//	if( control.Items.Count != 0 )
									//		control.SelectedIndex = 0;

									//	//control.MinimumLength = action.Slider.Length;
									//	//control.MaximumLength = action.Slider.Length;

									//	control.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

									//	tripple.Items.Add( control );
									//}
									else if( action.ActionType == EditorAction.ActionTypeEnum.ListBox )
									{
										//ListBox

										var tripple = new KryptonRibbonGroupTriple();
										ribbonGroup.Items.Add( tripple );

										var control = new KryptonRibbonGroupListBox();
										control.Tag = action;
										control.ToolTipBody = action.ToolTip;

										control.Control.Size = new System.Drawing.Size( (int)( (float)action.ListBox.Length * EditorAPI.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer1.Size = new System.Drawing.Size( (int)( (float)action.ListBox.Length * EditorAPI.DPIScale ), control.Control.Size.Height );
										control.Control.kryptonSplitContainer1.Panel2MinSize = (int)( (float)control.Control.kryptonSplitContainer1.Panel2MinSize * EditorAPI.DPIScale );
										control.Control.kryptonSplitContainer1.SplitterDistance = 10000;
										//if( action.ListBox.Length != 172 )
										//	control.Control.Size = new System.Drawing.Size( action.ListBox.Length, control.Control.Size.Height );

										control.Control.kryptonLabel1.Text = EditorLocalization.Translate( "Ribbon.Action", action.RibbonText.Item1 );

										var browser = control.Control.contentBrowser1;

										if( action.ListBox.Mode == EditorAction.ListBoxSettings.ModeEnum.Tiles )
										{
											browser.ListViewModeOverride = new ContentBrowserListModeTilesRibbon( browser );
											browser.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
											browser.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
											browser.UseSelectedTreeNodeAsRootForList = false;
											browser.Options.Breadcrumb = false;
											browser.ListViewBorderDraw = BorderSides.Left | BorderSides.Right | BorderSides.Bottom;

											browser.Options.TileImageSize = (int)( (float)23 * EditorAPI.DPIScale );
											//browser.Options.TileImageSize = 28;// 32;
										}
										else
										{
											browser.RemoveTreeViewIconsColumn();
											browser.TreeView.RowHeight -= 2;
										}

										browser.Tag = action;

										//update items
										control.SetItems( action.ListBox.Items );

										browser.ItemAfterSelect += ListBrowser_ItemAfterSelect;

										if( browser.Items.Count != 0 )
											browser.SelectItems( new ContentBrowser.Item[] { browser.Items.ToArray()[ 0 ] } );

										//browser.ItemAfterSelect += ListBrowser_ItemAfterSelect;

										tripple.Items.Add( control );
									}

								}
							}
						}
					}

					//select
					var tabType = "";
					if( tab != null )
						tabType = tab.Type;
					if( ribbonLastSelectedTabTypeByUser != "" && tabType == ribbonLastSelectedTabTypeByUser )
						kryptonRibbon.SelectedTab = ribbonTab;
				}

				ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
			}
		}

		void RibbonSubGroupUpdateItemsRecursive( KryptonContextMenuCollection contextMenuItems, out bool existsEnabled )
		{
			existsEnabled = false;

			foreach( var item in contextMenuItems )
			{
				var items = item as KryptonContextMenuItems;
				if( items != null )
				{
					foreach( var item2 in items.Items )
					{
						var item3 = item2 as KryptonContextMenuItem;
						if( item3 != null )
						{
							var action = item3.Tag as EditorAction;
							if( action != null )
							{
								var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.RibbonQAT, action );

								item3.Enabled = state.Enabled;
								if( item3.Checked != state.Checked )
									item3.Checked = state.Checked;

								if( item3.Enabled )
									existsEnabled = true;
							}
						}
					}
				}
			}
		}

		void RibbonButtonsUpdateProperties()
		{
			//update tabs visibility
			{
				Metadata.TypeInfo selectedType = null;
				{
					var obj = workspaceController.SelectedDocumentWindow?.ObjectOfWindow;
					if( obj != null )
						selectedType = MetadataManager.MetadataGetType( obj );
				}
				foreach( var ribbonTab in kryptonRibbon.RibbonTabs )
				{
					var tab = ribbonTab.Tag as EditorRibbonDefaultConfiguration.Tab;
					if( tab != null )
					{
						bool visible = true;
						if( tab.VisibleOnlyForType != null && visible )
							visible = selectedType != null && tab.VisibleOnlyForType.IsAssignableFrom( selectedType );
						if( tab.VisibleCondition != null && visible )
							visible = tab.VisibleCondition();

						if( ribbonTab.Visible != visible )
						{
							ribbonLastSelectedTabTypeByUser_DisableUpdate = true;
							ribbonTab.Visible = visible;
							ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
						}

						if( ribbonTab.Visible && ribbonLastSelectedTabTypeByUser != "" && ribbonLastSelectedTabTypeByUser == tab.Type )
						{
							ribbonLastSelectedTabTypeByUser_DisableUpdate = true;
							kryptonRibbon.SelectedTab = ribbonTab;
							ribbonLastSelectedTabTypeByUser_DisableUpdate = false;
						}
					}
				}
			}

			//update controls
			foreach( var ribbonTab in kryptonRibbon.RibbonTabs )
			{
				foreach( var ribbonGroup in ribbonTab.Groups )
				{
					foreach( var groupItem in ribbonGroup.Items )
					{
						var tripple = groupItem as KryptonRibbonGroupTriple;
						if( tripple != null )
						{
							foreach( var trippleItem in tripple.Items )
							{
								//Button, DropDown
								var button = trippleItem as KryptonRibbonGroupButton;
								if( button != null )
								{
									//sub group
									if( button.Tag as string == "SubGroup" && button.KryptonContextMenu != null )
									{
										RibbonSubGroupUpdateItemsRecursive( button.KryptonContextMenu.Items, out var existsEnabled );

										//disable group when all items are disabled
										button.Enabled = existsEnabled;
									}

									//action
									var action = button.Tag as EditorAction;
									if( action != null )
									{
										var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.RibbonQAT, action );

										button.Enabled = state.Enabled;

										//кнопка меняет тип, т.к. в действиях не указывается может ли она быть Checked.
										//сделано так, что меняет при первом изменении isChecked.
										if( state.Checked && action.ActionType == EditorAction.ActionTypeEnum.Button )
											button.ButtonType = GroupButtonType.Check;

										button.Checked = state.Checked;
									}
								}

								//Slider
								var slider = trippleItem as KryptonRibbonGroupSlider;
								if( slider != null )
								{
									var action = slider.Tag as EditorAction;
									if( action != null )
									{
										var lastValue = action.Slider.Value;

										var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.RibbonQAT, action );

										slider.Enabled = state.Enabled;
										if( lastValue != action.Slider.Value )
										{
											slider.Control.SetValue( action.Slider.Value );
											//trackBar.Value = action.Slider.Value;
										}
									}
								}

								////ComboBox
								//var comboBox = trippleItem as KryptonRibbonGroupComboBox;
								//if( comboBox != null )
								//{
								//	var action = comboBox.Tag as EditorAction;
								//	if( action != null )
								//	{
								//		var lastItems = action.ComboBox.Items;
								//		var lastSelectedIndex = action.ComboBox.SelectedIndex;

								//		var state = EditorAPI.GetEditorActionState( EditorAction.HolderEnum.RibbonQAT, action );

								//		comboBox.Enabled = state.Enabled;

								//		if( !action.ComboBox.Items.SequenceEqual( lastItems ) )
								//		{
								//			var oldSelectedIndex = comboBox.SelectedIndex;

								//			comboBox.Items.Clear();
								//			foreach( var item in action.ComboBox.Items )
								//				comboBox.Items.Add( item );

								//			if( oldSelectedIndex >= 0 && oldSelectedIndex < comboBox.Items.Count )
								//				comboBox.SelectedIndex = oldSelectedIndex;
								//			else if( comboBox.Items.Count != 0 )
								//				comboBox.SelectedIndex = 0;
								//		}
								//		if( lastSelectedIndex != action.ComboBox.SelectedIndex )
								//			comboBox.SelectedIndex = action.ComboBox.SelectedIndex;
								//	}
								//}

								//ListBox
								var listBox = trippleItem as KryptonRibbonGroupListBox;
								if( listBox != null )
								{
									var action = listBox.Tag as EditorAction;
									if( action != null )
									{
										var lastItems = action.ListBox.Items;
										var lastSelectedIndex = action.ListBox.SelectedIndex;

										var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.RibbonQAT, action );

										//!!!!не listBox2?
										listBox.Enabled = state.Enabled;

										var browser = listBox.Control.contentBrowser1;

										//update items
										if( !action.ListBox.Items.SequenceEqual( lastItems ) )
											listBox.SetItems( action.ListBox.Items );

										//update selected item
										if( action.ListBox.SelectIndex != null )
										{
											int selectIndex = action.ListBox.SelectIndex.Value;

											var itemToSelect = browser.Items.FirstOrDefault( i => (int)i.Tag == selectIndex );
											if( itemToSelect != null )
											{
												var toSelect = new ContentBrowser.Item[] { itemToSelect };
												if( !toSelect.SequenceEqual( browser.SelectedItems ) )
													browser.SelectItems( toSelect );
											}
											else
											{
												if( browser.SelectedItems.Length != 0 )
													browser.SelectItems( null );
											}

											action.ListBox.SelectIndex = null;
										}

										//{
										//	int selectIndex = action.ListBox.SelectedIndex;
										//	int currentSelectedIndex = -1;
										//	if( browser.SelectedItems.Length == 1 )
										//		currentSelectedIndex = (int)browser.SelectedItems[ 0 ].Tag;
										//	//if( browser.SelectedItems.Length == 1 )
										//	//	currentSelectedIndex = browser.RootItems.IndexOf( browser.SelectedItems[ 0 ] );

										//	if( selectIndex != currentSelectedIndex )
										//	{
										//		var itemToSelect = browser.Items.FirstOrDefault( i => (int)i.Tag == selectIndex );
										//		if( itemToSelect != null )
										//			browser.SelectItems( new ContentBrowser.Item[] { itemToSelect } );
										//		else
										//			browser.SelectItems( null );
										//	}
										//}

										//!!!!?
										//if( lastSelectedIndex != action.ListBox.SelectedIndex )
										//{
										//	if(browser.Items
										//	browser.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );

										//	//browser.SelectedIndex = action.ListBox.SelectedIndex;
										//}

									}
								}

							}
						}

					}
				}
			}
		}

		void RibbonUpdate()
		{
			RibbonButtonsCheckForRecreate();
			RibbonButtonsUpdateProperties();
		}
	}
}
