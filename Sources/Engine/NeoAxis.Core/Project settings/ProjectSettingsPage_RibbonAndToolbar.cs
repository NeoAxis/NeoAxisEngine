// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Ribbon And Toolbar page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_RibbonAndToolbar : ProjectSettingsPage
	{
		public sealed class RibbonAndToolbarActionsClass
		{
			[Serialize]
			public bool UseDefaultSettings { get; set; } = true;

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<TabItem> RibbonTabs
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && ribbonTabs.Count == 0 )
					{
						foreach( var tab in EditorRibbonDefaultConfiguration.Tabs )
						{
							var tabItem = new TabItem();
							tabItem.Name = tab.Name;

							foreach( var group in tab.Groups )
							{
								var groupItem = new GroupItem();
								groupItem.Name = group.Name;

								foreach( var child in group.Children )
								{
									//sub group
									var subGroup = child as EditorRibbonDefaultConfiguration.Group;
									if( subGroup != null )
									{
										var actionItem = new ActionItem();
										actionItem.Type = ActionItem.TypeEnum.SubGroupOfActions;
										actionItem.Name = subGroup.Name;

										groupItem.Actions.Add( actionItem );
									}

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
										var actionItem = new ActionItem();
										actionItem.Name = action.Name;

										groupItem.Actions.Add( actionItem );
									}
								}

								tabItem.Groups.Add( groupItem );
							}

							ribbonTabs.Add( tabItem );
						}
					}
					return ribbonTabs;
				}
				set { ribbonTabs = value; }
			}
			List<TabItem> ribbonTabs = new List<TabItem>();

			[Serialize]
			[Cloneable( CloneType.Deep )]
			public List<ActionItem> ToolbarActions
			{
				get
				{
					//init default configuration
					if( UseDefaultSettings && toolbarActions.Count == 0 )
					{
						foreach( var action in EditorActions.Actions )
						{
							if( !action.CompletelyDisabled && action.QatSupport && action.QatAddByDefault )
							{
								var item = new ActionItem();
								item.Name = action.Name;
								toolbarActions.Add( item );
							}
						}
					}
					return toolbarActions;
				}
				set { toolbarActions = value; }
			}
			List<ActionItem> toolbarActions = new List<ActionItem>();

			////////////

			public sealed class TabItem
			{
				public enum TypeEnum
				{
					Basic,
					Additional,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Basic;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				[Serialize]
				[Cloneable( CloneType.Deep )]
				public List<GroupItem> Groups = new List<GroupItem>();

				//

				public bool Equals( TabItem obj )
				{
					if( Type != obj.Type || Name != obj.Name || Enabled != obj.Enabled )
						return false;

					if( Groups.Count != obj.Groups.Count )
						return false;
					for( int n = 0; n < Groups.Count; n++ )
						if( !Groups[ n ].Equals( obj.Groups[ n ] ) )
							return false;

					return true;
				}

				public TabItem Clone()
				{
					var item = new TabItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;

					foreach( var group in Groups )
						item.Groups.Add( group.Clone() );

					return item;
				}
			}

			////////////

			public sealed class GroupItem
			{
				public enum TypeEnum
				{
					Basic,
					Additional,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Basic;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				[Serialize]
				[Cloneable( CloneType.Deep )]
				public List<ActionItem> Actions = new List<ActionItem>();

				//

				public bool Equals( GroupItem obj )
				{
					if( Type != obj.Type || Name != obj.Name || Enabled != obj.Enabled )
						return false;

					if( Actions.Count != obj.Actions.Count )
						return false;
					for( int n = 0; n < Actions.Count; n++ )
						if( !Actions[ n ].Equals( obj.Actions[ n ] ) )
							return false;

					return true;
				}

				public GroupItem Clone()
				{
					var item = new GroupItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;

					foreach( var action in Actions )
						item.Actions.Add( action.Clone() );

					return item;
				}
			}

			////////////

			public sealed class ActionItem
			{
				public enum TypeEnum
				{
					Action,
					SubGroupOfActions,
				}

				[Serialize]
				public TypeEnum Type = TypeEnum.Action;

				[Serialize]
				public string Name;

				[Serialize]
				public bool Enabled = true;

				//

				public bool Equals( ActionItem obj )
				{
					return Type == obj.Type && Name == obj.Name && Enabled == obj.Enabled;
				}

				public ActionItem Clone()
				{
					var item = new ActionItem();
					item.Type = Type;
					item.Name = Name;
					item.Enabled = Enabled;
					return item;
				}
			}

			////////////

			public void ResetToDefault()
			{
				UseDefaultSettings = false;
				ribbonTabs.Clear();
				toolbarActions.Clear();
				UseDefaultSettings = true;
			}

			public void SetToNotDefault()
			{
				var r = RibbonTabs;
				var a = ToolbarActions;
				UseDefaultSettings = false;
			}

			public bool Equals( RibbonAndToolbarActionsClass obj )
			{
				if( UseDefaultSettings != obj.UseDefaultSettings )
					return false;

				if( !UseDefaultSettings )
				{
					if( ribbonTabs.Count != obj.ribbonTabs.Count )
						return false;
					for( int n = 0; n < ribbonTabs.Count; n++ )
						if( !ribbonTabs[ n ].Equals( obj.ribbonTabs[ n ] ) )
							return false;

					if( toolbarActions.Count != obj.toolbarActions.Count )
						return false;
					for( int n = 0; n < toolbarActions.Count; n++ )
						if( !toolbarActions[ n ].Equals( obj.toolbarActions[ n ] ) )
							return false;
				}

				return true;
			}

			public RibbonAndToolbarActionsClass Clone()
			{
				var obj = new RibbonAndToolbarActionsClass();
				obj.UseDefaultSettings = UseDefaultSettings;

				if( !UseDefaultSettings )
				{
					obj.ribbonTabs.Clear();
					foreach( var tab in ribbonTabs )
						obj.ribbonTabs.Add( tab.Clone() );

					obj.toolbarActions.Clear();
					foreach( var item in toolbarActions )
						obj.toolbarActions.Add( item.Clone() );
				}

				return obj;
			}
		}

		[Category( "Ribbon and Toolbar Actions" )]
		[Serialize]
		public RibbonAndToolbarActionsClass RibbonAndToolbarActions
		{
			get { return ribbonAndToolbarSettings; }
			set { ribbonAndToolbarSettings = value; }
		}
		RibbonAndToolbarActionsClass ribbonAndToolbarSettings = new RibbonAndToolbarActionsClass();
	}
}
