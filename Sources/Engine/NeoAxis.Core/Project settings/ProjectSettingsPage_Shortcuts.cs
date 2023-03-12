// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Windows.Forms;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Shortcuts page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_Shortcuts : ProjectSettingsPage
	{
#if !DEPLOY
		public sealed class ShortcutSettingsClass
		{
			[Serialize]
			public bool UseDefaultSettings { get; set; } = true;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<ActionItem> Actions
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && actions.Count == 0 )
					{
						foreach( var action in EditorActions.Actions )
						{
							if( !action.CompletelyDisabled )
							{
								var item = new ActionItem();
								item.Name = action.Name;
								if( action.ShortcutKeys != null && action.ShortcutKeys.Length >= 1 )
								{
									item.Shortcut1 = action.ShortcutKeys[ 0 ];
									if( action.ShortcutKeys.Length >= 2 )
										item.Shortcut2 = action.ShortcutKeys[ 1 ];
								}
								actions.Add( item );
							}
						}
					}
					return actions;
				}
				set { actions = value; }
			}
			List<ActionItem> actions = new List<ActionItem>();

			//////////////

			public sealed class ActionItem
			{
				[Serialize]
				public string Name;

				[Serialize]
				public Keys Shortcut1;

				[Serialize]
				public Keys Shortcut2;

				//

				public bool Equals( ActionItem obj )
				{
					return Name == obj.Name && Shortcut1 == obj.Shortcut1 && Shortcut2 == obj.Shortcut2;
				}

				public ActionItem Clone()
				{
					var item = new ActionItem();
					item.Name = Name;
					item.Shortcut1 = Shortcut1;
					item.Shortcut2 = Shortcut2;
					return item;
				}

				public Keys[] ToArray()
				{
					var list = new List<Keys>();
					if( Shortcut1 != Keys.None )
						list.Add( Shortcut1 );
					if( Shortcut2 != Keys.None )
						list.Add( Shortcut2 );
					return list.ToArray();
				}
			}

			////////////

			public void ResetToDefault()
			{
				UseDefaultSettings = false;
				actions.Clear();
				UseDefaultSettings = true;
			}

			public void SetToNotDefault()
			{
				var a = Actions;
				UseDefaultSettings = false;
			}

			public bool Equals( ShortcutSettingsClass obj )
			{
				if( UseDefaultSettings != obj.UseDefaultSettings )
					return false;

				if( !UseDefaultSettings )
				{
					if( actions.Count != obj.actions.Count )
						return false;
					for( int n = 0; n < actions.Count; n++ )
						if( !actions[ n ].Equals( obj.actions[ n ] ) )
							return false;
				}

				return true;
			}

			public ShortcutSettingsClass Clone()
			{
				var obj = new ShortcutSettingsClass();
				obj.UseDefaultSettings = UseDefaultSettings;

				if( !UseDefaultSettings )
				{
					obj.actions.Clear();
					foreach( var item in actions )
						obj.actions.Add( item.Clone() );
				}

				return obj;
			}

			public ActionItem GetActionItem( string name )
			{
				//!!!!slowly

				return Actions.Find( a => a.Name == name );
			}
		}

		[Category( "Shortcuts" )]
		[Serialize]
		public ShortcutSettingsClass ShortcutSettings
		{
			get { return shortcutSettings; }
			set { shortcutSettings = value; }
		}
		ShortcutSettingsClass shortcutSettings = new ShortcutSettingsClass();
#endif
	}
}
