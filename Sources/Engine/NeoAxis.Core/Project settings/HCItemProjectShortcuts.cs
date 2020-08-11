// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
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
	class HCItemProjectShortcuts : HCItemProperty
	{
		Dictionary<Image, string> images = new Dictionary<Image, string>();
		int imageCounter = 0;

		Component_ProjectSettings.ShortcutSettingsClass.ActionItem selectedActionItem;

		/////////////////////////////////////////

		public HCItemProjectShortcuts( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			var control = new HCItemProjectShortcutsForm();
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
			return EditorLocalization.Translate( "ProjectShortcuts", text );
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var settings = GetSettings();
			if( settings == null )
				return;

			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			control.contentBrowserAll.ItemAfterSelect += ContentBrowserAll_ItemAfterSelect;

			control.kryptonButtonReset.Click += KryptonButtonReset_Click;

			//init all actions
			{
				var items = new List<ContentBrowser.Item>();

				var actions = new List<EditorAction>( EditorActions.Actions );
				CollectionUtility.InsertionSort( actions, delegate ( EditorAction a, EditorAction b )
				{
					return string.Compare( a.Name, b.Name );
				} );

				Dictionary<Image, string> images = new Dictionary<Image, string>();
				int imageCounter = 0;

				foreach( var action in actions )
				{
					var text = action.Name;

					var actionItem = settings.GetActionItem( action.Name );
					if( actionItem != null )
					{
						var keysString = EditorActions.ConvertShortcutKeysToString( actionItem.ToArray() );
						if( keysString != "" )
							text += " (" + keysString + ")";
					}
					//var text = action.Name;
					//var keysString = EditorActions.ConvertShortcutKeysToString( action.ShortcutKeys );
					//if( keysString != "" )
					//	text += " (" + keysString + ")";

					var item = new ContentBrowserItem_Virtual( control.contentBrowserAll, null, text );
					item.Tag = action;
					item.Description = action.Description;

					var smallImage = action.GetImageSmall();
					if( smallImage != null )
					{
						if( !images.TryGetValue( smallImage, out var id ) )
						{
							id = "Name_" + imageCounter.ToString();
							images[ smallImage ] = id;
							control.contentBrowserAll.ImageHelper.AddImage( id, smallImage, action.GetImageBig() );
							imageCounter++;
						}
						item.imageKey = id;
					}

					items.Add( item );
				}

				control.contentBrowserAll.SetData( items, false );
			}
		}

		class ProxyObject
		{
			HCItemProjectShortcuts owner;
			Component_ProjectSettings.ShortcutSettingsClass.ActionItem obj;

			//

			static void Convert( Keys from, out EKeys key, out bool shift, out bool control, out bool alt )
			{
				var from2 = from;
				from2 = from2 & ~Keys.Shift;
				from2 = from2 & ~Keys.Control;
				from2 = from2 & ~Keys.Alt;
				key = (EKeys)from2;

				shift = ( ( from & Keys.Shift ) == Keys.Shift );
				control = ( ( from & Keys.Control ) == Keys.Control );
				alt = ( ( from & Keys.Alt ) == Keys.Alt );
			}

			static void Convert( EKeys key, bool shift, bool control, bool alt, out Keys to )
			{
				to = (Keys)key;
				if( shift )
					to |= Keys.Shift;
				if( control )
					to |= Keys.Control;
				if( alt )
					to |= Keys.Alt;
			}

			[Category( "Shortcut 1" )]
			[DisplayName( "Key" )]
			public EKeys Shortcut1Key
			{
				get
				{
					Convert( obj.Shortcut1, out var value, out _, out _, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut1, out var key, out var shift, out var control, out var alt );
					if( key == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					key = value;
					Convert( key, shift, control, alt, out obj.Shortcut1 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 1" )]
			[DisplayName( "Shift" )]
			public bool Shortcut1Shift
			{
				get
				{
					Convert( obj.Shortcut1, out _, out var value, out _, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut1, out var key, out var shift, out var control, out var alt );
					if( shift == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					shift = value;
					Convert( key, shift, control, alt, out obj.Shortcut1 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 1" )]
			[DisplayName( "Control" )]
			public bool Shortcut1Control
			{
				get
				{
					Convert( obj.Shortcut1, out _, out _, out var value, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut1, out var key, out var shift, out var control, out var alt );
					if( control == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					control = value;
					Convert( key, shift, control, alt, out obj.Shortcut1 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 1" )]
			[DisplayName( "Alt" )]
			public bool Shortcut1Alt
			{
				get
				{
					Convert( obj.Shortcut1, out _, out _, out _, out var value );
					return value;
				}
				set
				{
					Convert( obj.Shortcut1, out var key, out var shift, out var control, out var alt );
					if( alt == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					alt = value;
					Convert( key, shift, control, alt, out obj.Shortcut1 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 2" )]
			[DisplayName( "Key" )]
			public EKeys Shortcut2Key
			{
				get
				{
					Convert( obj.Shortcut2, out var value, out _, out _, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut2, out var key, out var shift, out var control, out var alt );
					if( key == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					key = value;
					Convert( key, shift, control, alt, out obj.Shortcut2 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 2" )]
			[DisplayName( "Shift" )]
			public bool Shortcut2Shift
			{
				get
				{
					Convert( obj.Shortcut2, out _, out var value, out _, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut2, out var key, out var shift, out var control, out var alt );
					if( shift == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					shift = value;
					Convert( key, shift, control, alt, out obj.Shortcut2 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 2" )]
			[DisplayName( "Control" )]
			public bool Shortcut2Control
			{
				get
				{
					Convert( obj.Shortcut2, out _, out _, out var value, out _ );
					return value;
				}
				set
				{
					Convert( obj.Shortcut2, out var key, out var shift, out var control, out var alt );
					if( control == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					control = value;
					Convert( key, shift, control, alt, out obj.Shortcut2 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			[Category( "Shortcut 2" )]
			[DisplayName( "Alt" )]
			public bool Shortcut2Alt
			{
				get
				{
					Convert( obj.Shortcut2, out _, out _, out _, out var value );
					return value;
				}
				set
				{
					Convert( obj.Shortcut2, out var key, out var shift, out var control, out var alt );
					if( alt == value )
						return;

					var settings = owner.GetSettings();
					if( settings != null )
						settings.SetToNotDefault();

					alt = value;
					Convert( key, shift, control, alt, out obj.Shortcut2 );

					if( owner.Owner?.DocumentWindow?.Document != null )
						owner.Owner.DocumentWindow.Document.Modified = true;
				}
			}

			public ProxyObject( HCItemProjectShortcuts owner, Component_ProjectSettings.ShortcutSettingsClass.ActionItem obj )
			{
				this.owner = owner;
				this.obj = obj;
			}
		}

		private void KryptonButtonReset_Click( object sender, EventArgs e )
		{
			var settings = GetSettings();
			if( settings == null )
				return;

			if( EditorMessageBox.ShowQuestion( Translate( "Reset the configuration of shortcuts to default?" ), EMessageBoxButtons.YesNo ) == EDialogResult.No )
				return;

			settings.ResetToDefault();
			if( Owner?.DocumentWindow?.Document != null )
				Owner.DocumentWindow.Document.Modified = true;
		}

		Component_ProjectSettings.ShortcutSettingsClass GetSettings()
		{
			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			var values = GetValues();
			if( values == null || values.Length != 1 )
				return null;
			return values[ 0 ] as Component_ProjectSettings.ShortcutSettingsClass;
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			var settings = GetSettings();
			if( settings == null )
				return;

			control.kryptonButtonReset.Enabled = !settings.UseDefaultSettings;

			UpdateActions();
			UpdateProperties();
		}

		EditorAction GetSelectedAction()
		{
			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			var items = control.contentBrowserAll.SelectedItems;
			if( items.Length == 1 )
				return items[ 0 ].Tag as EditorAction;
			return null;
		}

		void UpdateProperties()
		{
			var settings = GetSettings();
			if( settings == null )
				return;
			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			Component_ProjectSettings.ShortcutSettingsClass.ActionItem toSelect = null;

			var action = GetSelectedAction();
			if( action != null )
				toSelect = settings.GetActionItem( action.Name );

			if( toSelect != selectedActionItem )
			{
				selectedActionItem = toSelect;

				//!!!!

				control.hierarchicalContainerSelected.SetData( null, new object[ 0 ], false );
				control.hierarchicalContainerSelected.UpdateItems();

				if( selectedActionItem != null )
				{
					var proxy = new ProxyObject( this, selectedActionItem );

					control.hierarchicalContainerSelected.SetData( null, new object[] { proxy }, false );
					control.hierarchicalContainerSelected.UpdateItems();
				}

				//if( selectedActionItem != null )
				//{
				//	var proxy = new ProxyObject( this, selectedActionItem );

				//	//!!!!false
				//	control.hierarchicalContainerSelected.SetData( null, new object[] { proxy }, false );
				//}
				//else
				//	control.hierarchicalContainerSelected.SetData( null, new object[ 0 ], false );

				//!!!!

				//control.hierarchicalContainerSelected.UpdateItems();

				//control.hierarchicalContainerSelected.UpdateItems();
				//control.hierarchicalContainerSelected.SetData( null, new object[] { proxy } );
				//control.hierarchicalContainerSelected.UpdateItems();

				//control.hierarchicalContainerSelected.SetData( null, new object[ 0 ] );
				//control.hierarchicalContainerSelected.UpdateItems();
				//control.hierarchicalContainerSelected.SetData( null, new object[] { proxy } );
				//control.hierarchicalContainerSelected.UpdateItems();

				//initialized = true;

				//if( !initialized )
				//	control.hierarchicalContainerSelected.SetData( null, new object[ 0 ] );

			}

			//bool initialized = false;

			//var action = GetSelectedAction();
			//if( action != null )
			//{
			//	var actionItem = settings.GetActionItem( action.Name );
			//	if( actionItem != null )
			//	{
			//		if( selectedActionItem != actionItem )
			//		{
			//			selectedActionItem = actionItem;

			//			//if( control.hierarchicalContainerSelected.SelectedObjects

			//			var proxy = new ProxyObject( this, actionItem );

			//			xx xx;
			//			control.hierarchicalContainerSelected.SetData( null, new object[ 0 ] );
			//			control.hierarchicalContainerSelected.UpdateItems();
			//			control.hierarchicalContainerSelected.SetData( null, new object[] { proxy } );
			//			control.hierarchicalContainerSelected.UpdateItems();

			//			//control.hierarchicalContainerSelected.SetData( null, new object[ 0 ] );
			//			//control.hierarchicalContainerSelected.UpdateItems();
			//			//control.hierarchicalContainerSelected.SetData( null, new object[] { proxy } );
			//			//control.hierarchicalContainerSelected.UpdateItems();

			//			initialized = true;
			//		}
			//	}
			//}

			//if( !initialized )
			//	control.hierarchicalContainerSelected.SetData( null, new object[ 0 ] );
		}

		private void ContentBrowserAll_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
		}

		void UpdateActions()
		{
			var settings = GetSettings();
			if( settings == null )
				return;
			var control = (HCItemProjectShortcutsForm)CreatedControlInsidePropertyItemControl;

			foreach( var item in control.contentBrowserAll.GetAllItems() )
			{
				var action = item.Tag as EditorAction;
				if( action != null )
				{
					var text = action.Name;

					var actionItem = settings.GetActionItem( action.Name );
					if( actionItem != null )
					{
						var keysString = EditorActions.ConvertShortcutKeysToString( actionItem.ToArray() );
						if( keysString != "" )
							text += " (" + keysString + ")";
					}

					( (ContentBrowserItem_Virtual)item ).SetText( text );
				}
			}
		}

	}
}
