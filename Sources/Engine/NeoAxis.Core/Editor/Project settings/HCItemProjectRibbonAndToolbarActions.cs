#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	class HCItemProjectRibbonAndToolbarActions : HCItemProperty
	{
		Dictionary<Image, string> images = new Dictionary<Image, string>();
		int imageCounter = 0;

		/////////////////////////////////////////

		class ContentBrowserItem_Group : ContentBrowserItem_Virtual
		{
			HCItemProjectRibbonAndToolbarActions hcItem;
			HCItemProjectRibbonAndToolbarActionsForm control;
			public ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem group;

			EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem, ContentBrowserItem_Virtual> items = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem, ContentBrowserItem_Virtual>();

			//

			public ContentBrowserItem_Group( ContentBrowser owner, ContentBrowser.Item parent, string text, HCItemProjectRibbonAndToolbarActions hcItem, HCItemProjectRibbonAndToolbarActionsForm control, ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem group )
				: base( owner, parent, text )
			{
				this.hcItem = hcItem;
				this.control = control;
				this.group = group;
			}

			public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
			{
				//update children
				if( !onlyAlreadyCreated )
				{
					var newItems = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem, ContentBrowserItem_Virtual>( items.Count );

					var actions = group != null ? group.Actions : hcItem.GetSettings().ToolbarActions;
					foreach( var actionItem in actions )
					{
						var action = EditorActions.GetByName( actionItem.Name );

						//if( action != null )//&& action.QatSupport )
						//{
						//get item or create
						if( !items.TryGetValue( actionItem, out var item ) )
						{
							item = new ContentBrowserItem_Virtual( control.contentBrowserProject, this, actionItem.Name );
							item.Tag = actionItem;
							if( action != null )
								item.Description = action.Description;

							if( action != null && actionItem.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.Action )
							{
								var smallImage = action.GetImageSmall();
								if( smallImage != null )
								{
									if( !hcItem.images.TryGetValue( smallImage, out var id ) )
									{
										id = "Name_" + hcItem.imageCounter.ToString();
										hcItem.images[ smallImage ] = id;
										control.contentBrowserProject.AddImageKey( id, smallImage, action.GetImageBig() );
										hcItem.imageCounter++;
									}
									item.imageKey = id;
								}
							}
							else if( actionItem.Type == ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem.TypeEnum.SubGroupOfActions )
							{
								//!!!!impl

								item.imageKey = "";
							}
						}

						if( item.ShowDisabled != !actionItem.Enabled )
						{
							item.ShowDisabled = !actionItem.Enabled;
							Owner?.Invalidate( true );
						}

						newItems[ actionItem ] = item;
						//}
					}

					items = newItems;
				}

				var result = new List<ContentBrowser.Item>( items.Count );
				foreach( var item in items.Values )
					result.Add( item );
				return result;
			}
		}

		/////////////////////////////////////////

		class ContentBrowserItem_Tab : ContentBrowserItem_Virtual
		{
			HCItemProjectRibbonAndToolbarActions hcItem;
			HCItemProjectRibbonAndToolbarActionsForm control;
			public ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem tabItem;

			EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem, ContentBrowserItem_Virtual> items = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem, ContentBrowserItem_Virtual>();

			//

			public ContentBrowserItem_Tab( ContentBrowser owner, ContentBrowser.Item parent, string text, HCItemProjectRibbonAndToolbarActions hcItem, HCItemProjectRibbonAndToolbarActionsForm control, ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem tabItem )
				: base( owner, parent, text )
			{
				this.hcItem = hcItem;
				this.control = control;
				this.tabItem = tabItem;
			}

			public override void LightweightUpdate()
			{
				base.LightweightUpdate();

				if( ShowDisabled != !tabItem.Enabled )
				{
					ShowDisabled = !tabItem.Enabled;
					Owner?.Invalidate( true );
				}
			}

			public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
			{
				//update children
				if( !onlyAlreadyCreated )
				{
					var newItems = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem, ContentBrowserItem_Virtual>( items.Count );

					foreach( var groupItem in tabItem.Groups )
					{
						//get item or create
						if( !items.TryGetValue( groupItem, out var item ) )
						{
							item = new ContentBrowserItem_Group( control.contentBrowserProject, this, groupItem.Name, hcItem, control, groupItem );
							item.Tag = groupItem;
							item.imageKey = "Folder";
							item.expandAtStartup = true;
						}

						if( item.ShowDisabled != !groupItem.Enabled )
						{
							item.ShowDisabled = !groupItem.Enabled;
							Owner?.Invalidate( true );
						}

						newItems[ groupItem ] = item;
					}

					items = newItems;
				}

				var result = new List<ContentBrowser.Item>( items.Count );
				foreach( var item in items.Values )
					result.Add( item );
				return result;
			}
		}

		/////////////////////////////////////////

		class ContentBrowserItem_Ribbon : ContentBrowserItem_Virtual
		{
			HCItemProjectRibbonAndToolbarActions hcItem;
			HCItemProjectRibbonAndToolbarActionsForm control;

			EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem, ContentBrowserItem_Virtual> items = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem, ContentBrowserItem_Virtual>();

			//

			public ContentBrowserItem_Ribbon( ContentBrowser owner, ContentBrowser.Item parent, string text, HCItemProjectRibbonAndToolbarActions hcItem, HCItemProjectRibbonAndToolbarActionsForm control )
				: base( owner, parent, text )
			{
				this.hcItem = hcItem;
				this.control = control;
			}

			public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
			{
				//update children
				if( !onlyAlreadyCreated )
				{
					var newItems = new EDictionary<ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem, ContentBrowserItem_Virtual>( items.Count );

					foreach( var tabItem in hcItem.GetSettings().RibbonTabs )
					{
						//var action = EditorActions.GetByName( tabItem.Name );
						//if( action != null && action.QatSupport )
						//{

						//get item or create
						if( !items.TryGetValue( tabItem, out var item ) )
						{
							item = new ContentBrowserItem_Tab( control.contentBrowserProject, this, tabItem.Name, hcItem, control, tabItem );
							item.Tag = tabItem;
							item.imageKey = "Folder";

							//item.Description = action.Description;

							//var smallImage = action.GetImageSmall();
							//if( smallImage != null )
							//{
							//	if( !hcItem.images.TryGetValue( smallImage, out var id ) )
							//	{
							//		id = "Name_" + hcItem.imageCounter.ToString();
							//		hcItem.images[ smallImage ] = id;
							//		control.contentBrowserProject.ImageHelper.AddImage( id, smallImage, action.ImageBig );
							//		hcItem.imageCounter++;
							//	}
							//	item.imageKey = id;
							//}
						}

						newItems[ tabItem ] = item;

						//}
					}

					items = newItems;
				}

				var result = new List<ContentBrowser.Item>( items.Count );
				foreach( var item in items.Values )
					result.Add( item );
				return result;
			}
		}

		/////////////////////////////////////////

		public HCItemProjectRibbonAndToolbarActions( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			var control = new HCItemProjectRibbonAndToolbarActionsForm();
			control.contentBrowserProject.ShowContextMenuEvent += ContentBrowserProject_ShowContextMenuEvent;
			return control;
		}

		public override EUserControl CreateControlImpl()
		{
			var control = (HCGridProperty)base.CreateControlImpl();
			control.ShowOnlyEditorControl = true;
			return control;
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "ProjectSettingsRibbonAndToolbarActions", text );
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var settings = GetSettings();
			if( settings == null )
				return;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			control.kryptonButtonReset.Click += KryptonButtonReset_Click;
			control.toolStripButtonEnabled.Click += ToolStripButtonEnabled_Click;
			control.toolStripButtonDelete.Click += ToolStripButtonDelete_Click;
			control.toolStripButtonAdd.Click += ToolStripButtonAdd_Click;
			control.toolStripButtonRename.Click += ToolStripButtonRename_Click;
			control.toolStripButtonMoveUp.Click += ToolStripButtonMoveUp_Click;
			control.toolStripButtonMoveDown.Click += ToolStripButtonMoveDown_Click;
			control.toolStripButtonNewGroup.Click += ToolStripButtonNewGroup_Click;
			control.contentBrowserProject.KeyDownOverride += ContentBrowserProject_KeyDownOverride;

			//init all actions
			{
				var items = new List<ContentBrowser.Item>();

				var actions = new List<EditorAction>( EditorActions.Actions ).Where( a => !a.CompletelyDisabled ).ToArray();
				CollectionUtility.InsertionSort( actions, delegate ( EditorAction a, EditorAction b )
				{
					return string.Compare( a.Name, b.Name );
				} );

				Dictionary<Image, string> images = new Dictionary<Image, string>();
				int imageCounter = 0;

				foreach( var action in actions )
				{
					var item = new ContentBrowserItem_Virtual( control.contentBrowserAll, null, action.Name );
					item.Tag = action;
					item.Description = action.Description;

					var smallImage = action.GetImageSmall();
					if( smallImage != null )
					{
						if( !images.TryGetValue( smallImage, out var id ) )
						{
							id = "Name_" + imageCounter.ToString();
							images[ smallImage ] = id;
							control.contentBrowserAll.AddImageKey( id, smallImage, action.GetImageBig() );
							imageCounter++;
						}
						item.imageKey = id;
					}

					items.Add( item );
				}

				control.contentBrowserAll.SetData( items, false );
			}

			//init current configuration
			{
				var items = new List<ContentBrowser.Item>();

				{
					var itemRibbon = new ContentBrowserItem_Ribbon( control.contentBrowserProject, null, "Ribbon", this, control );
					itemRibbon.imageKey = "Folder";
					itemRibbon.expandAtStartup = true;
					items.Add( itemRibbon );
				}

				{
					var itemToolbar = new ContentBrowserItem_Group( control.contentBrowserProject, null, "Quick Access Toolbar", this, control, null );
					itemToolbar.imageKey = "Folder";
					itemToolbar.expandAtStartup = true;
					items.Add( itemToolbar );
				}

				control.contentBrowserProject.SetData( items, true );

				//control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { firstItem } );
			}
		}

		internal/*obfuscator*/ void ContentBrowserProject_KeyDownOverride( ContentBrowser browser, object sender, KeyEventArgs e, ref bool handled )
		{
			//Delete
			{
				var shortcuts = EditorAPI.GetActionShortcuts( "Delete" );
				if( shortcuts != null )
				{
					foreach( var shortcut in shortcuts )
					{
						Keys keys = e.KeyCode | Control.ModifierKeys;
						if( shortcut == keys )
						{
							TryDeleteObjects();
							handled = true;
							break;
						}
					}
				}
			}

			//Rename
			{
				var shortcuts = EditorAPI.GetActionShortcuts( "Rename" );
				if( shortcuts != null )
				{
					foreach( var shortcut in shortcuts )
					{
						Keys keys = e.KeyCode | Control.ModifierKeys;
						if( shortcut == keys )
						{
							TryRename();
							handled = true;
							break;
						}
					}
				}
			}
		}

		internal/*obfuscator*/ void KryptonButtonReset_Click( object sender, EventArgs e )
		{
			var settings = GetSettings();
			if( settings == null )
				return;

			if( EditorMessageBox.ShowQuestion( Translate( "Reset the configuration of the Ribbon and Quick Access Toolbar to default?" ), EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return;

			settings.ResetToDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;
		}

		ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass GetSettings()
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			var values = GetValues();
			if( values == null || values.Length != 1 )
				return null;
			return values[ 0 ] as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass;
		}

		void PerformChildrenChanged()
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			foreach( var item in control.contentBrowserProject.GetAllItems() )
				item.PerformChildrenChanged();
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			var settings = GetSettings();
			if( settings == null )
				return;

			control.kryptonButtonReset.Enabled = !settings.UseDefaultSettings;

			PerformChildrenChanged();

			control.toolStripButtonEnabled.Enabled = CanSetEnabled( out _, out var enabledNewValue );
			control.toolStripButtonEnabled.Checked = control.toolStripButtonEnabled.Enabled && !enabledNewValue;
			control.toolStripButtonAdd.Enabled = CanAddAction( out _, out _ );
			control.toolStripButtonDelete.Enabled = CanDeleteObjects( out _ );
			control.toolStripButtonRename.Enabled = CanRename( out _ );
			control.toolStripButtonMoveUp.Enabled = CanMoveUp( out _, out _, out _ );
			control.toolStripButtonMoveDown.Enabled = CanMoveDown( out _, out _, out _ );

			control.toolStripButtonNewGroup.Enabled = CanNewGroup( out var newGroupText, out _ );
			if( control.toolStripButtonNewGroup.Text != newGroupText )
				control.toolStripButtonNewGroup.Text = newGroupText;
		}

		internal/*obfuscator*/ void ContentBrowserProject_ShowContextMenuEvent( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
		{
			var settings = GetSettings();
			if( settings == null )
				return;

			//Enabled
			{
				var item = new KryptonContextMenuItem( Translate( "Enabled" ), null,
					delegate ( object s, EventArgs e2 )
					{
						TrySetEnabled();
					} );
				item.Enabled = CanSetEnabled( out _, out var newValue );
				item.Checked = item.Enabled && !newValue;
				items.Add( item );
			}

			//separator
			items.Add( new KryptonContextMenuSeparator() );

			//New Group
			{
				var can = CanNewGroup( out var newGroupText, out _ );

				var item = new KryptonContextMenuItem( Translate( newGroupText ), EditorResourcesCache.New,
					delegate ( object s, EventArgs e2 )
					{
						TryNewGroup();
					} );
				item.Enabled = can;
				items.Add( item );
			}

			//Add
			{
				var item = new KryptonContextMenuItem( Translate( "Add Action" ), EditorResourcesCache.Add,
					delegate ( object s, EventArgs e2 )
					{
						TryAddAction();
					} );
				item.Enabled = CanAddAction( out _, out _ );
				items.Add( item );
			}

			////New object
			//{
			//	EditorContextMenu.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type )
			//	{
			//		TryNewObject( type );
			//	} );

			//	////!!!!! imageListContextMenu.Images[ "New_16.png" ] );
			//	//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16,
			//	//	delegate ( object s, EventArgs e2 )
			//	//	{
			//	//		TryNewObject();
			//	//	} );
			//	//Item dummy;
			//	//item.Enabled = CanNewObject( out dummy );
			//	//items.Add( item );
			//}

			////separator
			//items.Add( new KryptonContextMenuSeparator() );

			////Cut
			//{
			//	var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
			//		delegate ( object s, EventArgs e2 )
			//		{
			//			Cut();
			//		} );
			//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
			//	item.Enabled = CanCut();
			//	items.Add( item );
			//}

			////Copy
			//{
			//	var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
			//		delegate ( object s, EventArgs e2 )
			//		{
			//			Copy();
			//		} );
			//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
			//	item.Enabled = CanCopy();
			//	items.Add( item );
			//}

			////Paste
			//{
			//	var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
			//		delegate ( object s, EventArgs e2 )
			//		{
			//			Paste();
			//		} );
			//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
			//	item.Enabled = CanPaste( out _, out _, out _, out _ );
			//	items.Add( item );
			//}

			////Clone
			//{
			//	var item = new KryptonContextMenuItem( Translate( "Duplicate" ), EditorResourcesCache.Clone,
			//		delegate ( object s, EventArgs e2 )
			//		{
			//			TryCloneObjects();
			//		} );
			//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
			//	item.Enabled = CanCloneObjects( out _ );
			//	items.Add( item );
			//}

			////separator
			//items.Add( new KryptonContextMenuSeparator() );

			//Delete
			{
				var item = new KryptonContextMenuItem( EditorLocalization.Translate( "General", "Delete" ), EditorResourcesCache.Delete,
					delegate ( object s, EventArgs e2 )
					{
						TryDeleteObjects();
					} );
				item.Enabled = CanDeleteObjects( out _ );
				items.Add( item );
			}

			//Rename
			{
				var item = new KryptonContextMenuItem( EditorLocalization.Translate( "General", "Rename" ), null,
					delegate ( object s, EventArgs e2 )
					{
						TryRename();
					} );
				item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
				item.Enabled = CanRename( out _ );
				items.Add( item );
			}
		}

		public bool CanDeleteObjects( out List<ContentBrowser.Item> resultItemsToDelete )
		{
			resultItemsToDelete = new List<ContentBrowser.Item>();

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			foreach( var item in control.contentBrowserProject.SelectedItems )
			{
				var actionItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem != null )
				{
					var groupItem = item.Parent as ContentBrowserItem_Group;
					if( groupItem != null )
						resultItemsToDelete.Add( item );
				}

				var groupItem2 = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem2 != null )
					resultItemsToDelete.Add( item );

				var tabItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem != null )
					resultItemsToDelete.Add( item );
			}

			if( resultItemsToDelete.Count == 0 )
				return false;

			return true;
		}

		public void TryDeleteObjects()
		{
			if( !CanDeleteObjects( out var itemsToDelete ) )
				return;

			string text;
			if( itemsToDelete.Count == 1 )
				text = string.Format( EditorLocalization.Translate( "General", "Delete \'{0}\'?" ), itemsToDelete[ 0 ].ToString() );
			else
				text = EditorLocalization.Translate( "General", "Delete selected objects?" );

			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return;

			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			foreach( var item in itemsToDelete )
			{
				var actionItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem != null )
				{
					var groupItem = item.Parent as ContentBrowserItem_Group;
					if( groupItem != null )
					{
						if( groupItem.group != null )
							groupItem.group.Actions.Remove( actionItem );
						else
							settings.ToolbarActions.Remove( actionItem );
					}
				}

				var groupItem2 = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem2 != null && item.Parent != null )
				{
					var tabItem2 = item.Parent.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
					if( tabItem2 != null )
						tabItem2.Groups.Remove( groupItem2 );
				}

				var tabItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem != null )
					settings.RibbonTabs.Remove( tabItem );
			}
		}

		internal/*obfuscator*/ void ToolStripButtonDelete_Click( object sender, EventArgs e )
		{
			TryDeleteObjects();
		}

		EditorAction GetSelectedActionToAdd()
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			var items = control.contentBrowserAll.SelectedItems;
			if( items.Length == 1 )
				return items[ 0 ].Tag as EditorAction;
			return null;
		}

		bool GetSelectedDestinationGroupToAdd( out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem destinationGroup )
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			var items = control.contentBrowserProject.SelectedItems;
			if( items.Length == 1 )
			{
				var item = items[ 0 ];

				var groupItem = item as ContentBrowserItem_Group;
				if( groupItem != null )
				{
					destinationGroup = groupItem.group;
					return true;
				}

				var actionItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem != null )
				{
					var groupItem2 = item.Parent as ContentBrowserItem_Group;
					if( groupItem2 != null )
					{
						destinationGroup = groupItem2.group;
						return true;
					}
				}
			}

			destinationGroup = null;
			return false;
		}

		public bool CanAddAction( out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem destinationGroup, out EditorAction actionToAdd )
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			actionToAdd = GetSelectedActionToAdd();
			if( actionToAdd != null )
			{
				if( GetSelectedDestinationGroupToAdd( out var destinationGroup2 ) )
				{
					destinationGroup = destinationGroup2;
					return true;
				}
			}

			destinationGroup = null;
			actionToAdd = null;
			return false;
		}

		public void TryAddAction()
		{
			if( !CanAddAction( out var destinationGroup, out var actionToAdd ) )
				return;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			if( destinationGroup != null )
			{
				var item = new ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem();
				item.Name = actionToAdd.Name;
				destinationGroup.Actions.Add( item );
			}
			else
			{
				var item = new ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem();
				item.Name = actionToAdd.Name;
				settings.ToolbarActions.Add( item );
			}

			////clear selected objects
			//SelectObjects( null );
		}

		internal/*obfuscator*/ void ToolStripButtonAdd_Click( object sender, EventArgs e )
		{
			TryAddAction();
		}

		internal/*obfuscator*/ void ToolStripButtonRename_Click( object sender, EventArgs e )
		{
			TryRename();
		}

		bool CanMoveUp( out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem tab, out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem group, out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem actionItem )
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			var items = control.contentBrowserProject.SelectedItems;
			if( items.Length == 1 )
			{
				var actionItem2 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem2 != null )
				{
					var groupItem2 = items[ 0 ].Parent.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
					var actions = groupItem2 != null ? groupItem2.Actions : settings.ToolbarActions;

					var index = actions.IndexOf( actionItem2 );
					if( index != -1 && index > 0 )
					{
						tab = null;
						group = groupItem2;
						actionItem = actionItem2;
						return true;
					}
				}

				var groupItem3 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem3 != null )
				{
					var tab2 = items[ 0 ].Parent?.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
					if( tab2 != null )
					{
						var index = tab2.Groups.IndexOf( groupItem3 );
						if( index != -1 && index > 0 )
						{
							tab = tab2;
							group = groupItem3;
							actionItem = null;
							return true;
						}
					}
				}

				var tabItem2 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem2 != null )
				{
					var index = settings.RibbonTabs.IndexOf( tabItem2 );
					if( index != -1 && index > 0 )
					{
						tab = tabItem2;
						group = null;
						actionItem = null;
						return true;
					}
				}
			}

			tab = null;
			group = null;
			actionItem = null;
			return false;
		}

		void TryMoveUp()
		{
			if( !CanMoveUp( out var tab, out var group, out var actionItem ) )
				return;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			if( actionItem != null )
			{
				//move action

				var actions = group != null ? group.Actions : settings.ToolbarActions;

				var index = actions.IndexOf( actionItem );
				if( index == -1 || index == 0 )
					return;

				actions.RemoveAt( index );
				actions.Insert( index - 1, actionItem );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( actionItem );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
			else if( group != null )
			{
				//move group

				var index = tab.Groups.IndexOf( group );
				if( index == -1 || index == 0 )
					return;

				tab.Groups.RemoveAt( index );
				tab.Groups.Insert( index - 1, group );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( group );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
			else
			{
				//move tab

				var index = settings.RibbonTabs.IndexOf( tab );
				if( index == -1 || index == 0 )
					return;

				settings.RibbonTabs.RemoveAt( index );
				settings.RibbonTabs.Insert( index - 1, tab );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( tab );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
		}

		bool CanMoveDown( out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem tab, out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem group, out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem actionItem )
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			var items = control.contentBrowserProject.SelectedItems;
			if( items.Length == 1 )
			{
				var actionItem2 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem2 != null )
				{
					var groupItem2 = items[ 0 ].Parent.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
					var actions = groupItem2 != null ? groupItem2.Actions : settings.ToolbarActions;

					var index = actions.IndexOf( actionItem2 );
					if( index != -1 && index < actions.Count - 1 )
					{
						tab = null;
						group = groupItem2;
						actionItem = actionItem2;
						return true;
					}
				}

				var groupItem3 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem3 != null )
				{
					var tab2 = items[ 0 ].Parent?.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
					if( tab2 != null )
					{
						var index = tab2.Groups.IndexOf( groupItem3 );
						if( index != -1 && index < tab2.Groups.Count - 1 )
						{
							tab = tab2;
							group = groupItem3;
							actionItem = null;
							return true;
						}
					}
				}

				var tabItem2 = items[ 0 ].Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem2 != null )
				{
					var index = settings.RibbonTabs.IndexOf( tabItem2 );
					if( index != -1 && index < settings.RibbonTabs.Count - 1 )
					{
						tab = tabItem2;
						group = null;
						actionItem = null;
						return true;
					}
				}
			}

			tab = null;
			group = null;
			actionItem = null;
			return false;
		}

		void TryMoveDown()
		{
			if( !CanMoveDown( out var tab, out var group, out var actionItem ) )
				return;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			if( actionItem != null )
			{
				//move action

				var actions = group != null ? group.Actions : settings.ToolbarActions;

				var index = actions.IndexOf( actionItem );
				if( index == -1 || index == actions.Count - 1 )
					return;

				actions.RemoveAt( index );
				actions.Insert( index + 1, actionItem );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( actionItem );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
			else if( group != null )
			{
				//move group

				var index = tab.Groups.IndexOf( group );
				if( index == -1 || index == tab.Groups.Count - 1 )
					return;

				tab.Groups.RemoveAt( index );
				tab.Groups.Insert( index + 1, group );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( group );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
			else
			{
				//move tab

				var index = settings.RibbonTabs.IndexOf( tab );
				if( index == -1 || index == settings.RibbonTabs.Count - 1 )
					return;

				settings.RibbonTabs.RemoveAt( index );
				settings.RibbonTabs.Insert( index + 1, tab );

				//select item
				{
					PerformChildrenChanged();

					var item = control.contentBrowserProject.FindItemByTag( tab );
					if( item != null )
						control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
				}
			}
		}

		internal/*obfuscator*/ void ToolStripButtonMoveUp_Click( object sender, EventArgs e )
		{
			TryMoveUp();
		}

		internal/*obfuscator*/ void ToolStripButtonMoveDown_Click( object sender, EventArgs e )
		{
			TryMoveDown();
		}

		internal/*obfuscator*/ void ToolStripButtonEnabled_Click( object sender, EventArgs e )
		{
			TrySetEnabled();
		}

		public bool CanSetEnabled( out List<ContentBrowser.Item> itemsToUpdate, out bool newValue )
		{
			itemsToUpdate = new List<ContentBrowser.Item>();
			newValue = false;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			foreach( var item in control.contentBrowserProject.SelectedItems )
			{
				var actionItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem != null )
				{
					if( !actionItem.Enabled )
						newValue = true;
					itemsToUpdate.Add( item );
				}

				var groupItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem != null )
				{
					if( !groupItem.Enabled )
						newValue = true;
					itemsToUpdate.Add( item );
				}

				var tabItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem != null )
				{
					if( !tabItem.Enabled )
						newValue = true;
					itemsToUpdate.Add( item );
				}
			}

			if( itemsToUpdate.Count == 0 )
				return false;

			return true;
		}

		public void TrySetEnabled()
		{
			if( !CanSetEnabled( out var itemsToUpdate, out var newValue ) )
				return;

			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			foreach( var item in itemsToUpdate )
			{
				var actionItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.ActionItem;
				if( actionItem != null )
					actionItem.Enabled = newValue;

				var groupItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem != null )
					groupItem.Enabled = newValue;

				var tabItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem != null )
					tabItem.Enabled = newValue;
			}
		}

		public bool CanRename( out ContentBrowser.Item itemToRename )
		{
			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			if( control.contentBrowserProject.SelectedItems.Length == 1 )
			{
				var item = control.contentBrowserProject.SelectedItems[ 0 ];

				var groupItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
				if( groupItem != null )
				{
					itemToRename = item;
					return true;
				}

				var tabItem = item.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
				if( tabItem != null )
				{
					itemToRename = item;
					return true;
				}
			}

			itemToRename = null;
			return false;
		}

		public void TryRename()
		{
			if( !CanRename( out var itemToRename ) )
				return;

			var settings = GetSettings();

			var groupItem = itemToRename.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem;
			if( groupItem != null )
			{
				var form = new OKCancelTextBoxForm( EditorLocalization.TranslateLabel( "General", "Name:" ), groupItem.Name, EditorLocalization.Translate( "General", "Rename" ),
					delegate ( string text, ref string error )
					{
						if( string.IsNullOrEmpty( text.Trim() ) )
							return false;
						return true;
					},
					delegate ( string text, ref string error )
					{
						groupItem.Name = text;
						return true;
					}
				);

				if( form.ShowDialog() == DialogResult.Cancel )
					return;

				settings.SetToNotDefault();
				if( Owner?.DocumentWindow?.Document != null )
					Owner.DocumentWindow.Document.Modified = true;

				groupItem.Name = form.TextBoxText;

				( (ContentBrowserItem_Virtual)itemToRename ).SetText( groupItem.Name );
				Owner?.Invalidate( true );
			}

			var tabItem = itemToRename.Tag as ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem;
			if( tabItem != null )
			{
				var form = new OKCancelTextBoxForm( EditorLocalization.TranslateLabel( "General", "Name:" ), tabItem.Name, EditorLocalization.Translate( "General", "Rename" ),
					delegate ( string text, ref string error )
					{
						if( string.IsNullOrEmpty( text.Trim() ) )
							return false;
						return true;
					},
					delegate ( string text, ref string error )
					{
						tabItem.Name = text;
						return true;
					}
				);

				if( form.ShowDialog() == DialogResult.Cancel )
					return;

				settings.SetToNotDefault();
				if( Owner?.DocumentWindow?.Document != null )
					Owner.DocumentWindow.Document.Modified = true;

				tabItem.Name = form.TextBoxText;

				( (ContentBrowserItem_Virtual)itemToRename ).SetText( tabItem.Name );
				Owner?.Invalidate( true );
			}
		}

		public bool CanNewGroup( out string newGroupText, out ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem destinationTab )
		{
			newGroupText = "New Group";
			destinationTab = null;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;

			var items = control.contentBrowserProject.SelectedItems;
			if( items.Length == 1 )
			{
				var item = items[ 0 ];

				if( item is ContentBrowserItem_Ribbon )
				{
					newGroupText = "New Tab";
					return true;
				}

				var tabItem = item as ContentBrowserItem_Tab;
				if( tabItem != null )
				{
					destinationTab = tabItem.tabItem;
					return true;
				}
			}

			return false;
		}

		public void TryNewGroup()
		{
			if( !CanNewGroup( out _, out var destinationTab ) )
				return;

			var control = (HCItemProjectRibbonAndToolbarActionsForm)CreatedControlInsidePropertyItemControl;
			var settings = GetSettings();

			settings.SetToNotDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;

			object objectToSelect = null;

			if( destinationTab == null )
			{
				//new tab

				var tab = new ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem();
				tab.Name = "My Tab";
				tab.Type = ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.TabItem.TypeEnum.Additional;

				settings.RibbonTabs.Add( tab );

				objectToSelect = tab;
			}
			else
			{
				//new group

				var group = new ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem();
				group.Name = "My Group";
				group.Type = ProjectSettingsPage_RibbonAndToolbar.RibbonAndToolbarActionsClass.GroupItem.TypeEnum.Additional;

				destinationTab.Groups.Add( group );

				objectToSelect = group;
			}

			//select item

			PerformChildrenChanged();

			var item = control.contentBrowserProject.FindItemByTag( objectToSelect );
			if( item != null )
				control.contentBrowserProject.SelectItems( new ContentBrowser.Item[] { item } );
		}

		internal/*obfuscator*/ void ToolStripButtonNewGroup_Click( object sender, EventArgs e )
		{
			TryNewGroup();
		}
	}
}

#endif