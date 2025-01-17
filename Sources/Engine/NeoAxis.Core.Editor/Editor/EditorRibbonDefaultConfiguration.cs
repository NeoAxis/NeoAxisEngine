﻿#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NeoAxis.Editor
{
	public static class EditorRibbonDefaultConfiguration
	{
		static List<Tab> tabs = new List<Tab>();
		static bool basicTabsRegistered;
		//public static List<Tab> Tabs = new List<Tab>();

		/////////////////////////////////////////

		//public abstract class Element
		//{
		//	//public string name;

		//	public Element()
		//	{
		//	}

		//	//public Element( string name, Type type, bool disabled = false )
		//	//{
		//	//	this.name = name;
		//	//	this.type = type;
		//	//	this.disabled = disabled;
		//	//}
		//}

		/////////////////////////////////////////

		//public class ActionElement : Element
		//{
		//	public EditorAction action;

		//	public ActionElement()
		//	{
		//	}

		//	public ActionElement( string actionName )
		//	{
		//		action = EditorActions.GetByName( actionName );
		//		if( action == null )
		//			Log.Fatal( "EditorRibbonStandardTabs: ActionElement: No action with name \"{0}\".", actionName );
		//	}

		//	//public ActionElement( EditorAction action )
		//	//{
		//	//	this.action = action;
		//	//}
		//}

		/////////////////////////////////////////

		public class Group : IEditorRibbonDefaultConfigurationGroup
		{
			public string Name { get; set; }
			public (string, string) DropDownGroupText;
			public Image DropDownGroupImageSmall;
			public Image DropDownGroupImageLarge;
			public string DropDownGroupDescription = "";
			//can be Group, Action, action name (string), null (separator)
			public List<object> Children { get; set; } = new List<object>();
			public bool ShowArrow;
			//public List<Group> SubGroups = new List<Group>();
			//public List<EditorAction> Actions = new List<EditorAction>();

			public Group()
			{
			}

			public Group( string name )
			{
				this.Name = name;
			}

			public bool AddAction( string actionName )
			{
				var action = EditorActions.GetByName( actionName );
				if( action == null )
				{
					//add string name
					Children.Add( actionName );
					return true;
					//Log.Warning( "EditorRibbonDefaultConfiguration: Group: AddAction: No action with name \"{0}\".", actionName );
					//return false;
				}
				Children.Add( action );
				return true;
			}

			public void RemoveAction( string actionName )
			{
				var action = EditorActions.GetByName( actionName );
				if( action != null )
					Children.Remove( action );

				for( int n = 0; n < Children.Count; n++ )
				{
					var str = Children[ n ] as string;
					if( str != null && str == actionName )
					{
						Children.RemoveAt( n );
						break;
					}
				}
			}

			public void AddSeparator()
			{
				Children.Add( null );
			}

			public bool AreAllChildrenCompletelyDisabled()
			{
				foreach( var child in Children )
				{
					var subGroup = child as Group;
					if( subGroup != null && !subGroup.AreAllChildrenCompletelyDisabled() )
						return false;

					var action = child as EditorAction;
					if( action != null && !action.CompletelyDisabled )
						return false;

					var actionName = child as string;
					if( actionName != null )
					{
						var action2 = EditorActions.GetByName( actionName );
						if( action2 != null && !action2.CompletelyDisabled )
							return false;
					}
				}

				return true;
			}
		}

		/////////////////////////////////////////

		public class Tab : IEditorRibbonDefaultConfigurationTab
		{
			public string Name { get; set; }
			public string Type { get; set; }

			public Metadata.TypeInfo VisibleOnlyForType { get; set; }

			public EditorRibbonDefaultConfigurationTabVisibleConditionDelegate VisibleCondition { get; set; }

			public List<Group> Groups2 { get; set; } = new List<Group>();

			public IEditorRibbonDefaultConfigurationGroup[] Groups { get { return Groups2.ToArray(); } }

			public Tab()
			{
			}

			public Tab( string name, string type, Metadata.TypeInfo visibleOnlyForType = null, EditorRibbonDefaultConfigurationTabVisibleConditionDelegate visibleCondition = null )
			{
				Name = name;
				Type = type;
				VisibleOnlyForType = visibleOnlyForType;
				VisibleCondition = visibleCondition;
			}
		}

		/////////////////////////////////////////

#if !DEPLOY

		//static EditorRibbonDefaultConfiguration()
		static void RegisterBasicTabs()
		{
			if( basicTabsRegistered )
				return;
			basicTabsRegistered = true;

			EditorStandardActions.Register();

			//Home
			{
				var tab = new Tab( "Home", "Home" );
				//var tab = new Tab( "General" );
				AddTab( tab );

				//New
				{
					var group = new Group( "Resource" );
					//var group = new Group( "New" );
					tab.Groups2.Add( group );

					group.AddAction( "New Resource" );
					//group.AddAction( "Import Resource" );
					group.AddAction( "Stores" );
				}

				//Save
				{
					var group = new Group( "Save" );
					tab.Groups2.Add( group );

					group.AddAction( "Save" );
					group.AddAction( "Save As" );
					group.AddAction( "Save All" );
				}

				////Restart
				//{
				//	var group = new Group( "Restart" );
				//	tab.groups.Add( group );

				//	group.AddAction("Restart Application" );
				//}

				//Undo
				{
					var group = new Group( "Editing" );
					tab.Groups2.Add( group );

					group.AddAction( "Undo" );
					group.AddAction( "Redo" );
					//group.AddAction( "Cut" );
					//group.AddAction( "Copy" );
					//group.AddAction( "Paste" );
					group.AddAction( "Duplicate" );
					group.AddAction( "Delete" );
				}

				//Transform
				{
					var group = new Group( "Transform" );
					tab.Groups2.Add( group );

					group.AddAction( "Select" );
					group.AddAction( "Move & Rotate" );
					group.AddAction( "Move" );
					group.AddAction( "Rotate" );
					group.AddAction( "Scale" );
					//group.AddAction( "Move, Rotate, Scale" );
					group.AddAction( "Transform Using Local Coordinates" );
				}

				//Simulation
				{
					var group = new Group( "Play" );
					tab.Groups2.Add( group );

					group.AddAction( "Play" );
					group.AddAction( "Run Player" );
					//group.AddAction( "Run Device" );
					//group.AddAction( "Run Device 2" );
				}

				//Project
				{
					var group = new Group( "Project" );
					tab.Groups2.Add( group );

					group.AddAction( "Project Settings" );
				}

				////Additions
				//{
				//	var group = new Group( "Additions" );
				//	tab.Groups.Add( group );

				//	group.AddAction( "Stores" );
				//	//group.AddAction( "Store" );
				//}

				//Docs
				{
					var group = new Group( "Docs" );
					tab.Groups2.Add( group );

					group.AddAction( "Manual" );
					group.AddAction( "Tips" );
				}

				//Cloud Project
				//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				{
					var group = new Group( "Cloud" );
					//var group = new Group( "Cloud Project" );
					tab.Groups2.Add( group );

					group.AddAction( "Cloud Project Get" );
					group.AddAction( "Cloud Project Commit" );
					//group.AddAction( "Cloud Project Scene" );
				}
			}

			//Scripting
			{
				var tab = new Tab( "Scripting", "Scripting" );
				AddTab( tab );

				//Project's Solution
				{
					var group = new Group( "Solution" );
					tab.Groups2.Add( group );

					group.AddAction( "Build Project's Solution" );
					//group.AddAction( "Build Project's Solution and Update Resources" );
					//group.AddAction( "Build and Update Project's Solution" );
					//group.AddAction( "Find in Files" );
				}

				//C# Project
				{
					var group = new Group( "C# Project" );
					//var group = new Group( "C# project" );
					tab.Groups2.Add( group );

					group.AddAction( "C# File" );
					group.AddAction( "Add C# files to Project.csproj" );
					group.AddAction( "Remove C# files from Project.csproj" );
					//group.AddAction( "Add to Project.csproj" );
					//group.AddAction( "Remove from Project.csproj" );
				}

				//Components
				{
					var group = new Group( "Components" );
					tab.Groups2.Add( group );

					group.AddAction( "C# Script" );
					group.AddAction( "Flow Graph" );
				}

				//C#
				{
					var group = new Group( "C# Editing" );
					tab.Groups2.Add( group );

					group.AddAction( "Comment Selection" );
					group.AddAction( "Uncomment Selection" );
					group.AddAction( "Rename" );
					group.AddAction( "Format Document" );
					group.AddAction( "Add Property Code" );
				}

				////Debug
				//{
				//	var group = new Group( "Debug" );
				//	tab.Groups.Add( group );

				//	group.AddAction( "Debug Start" );// Start Debugging" );
				//	group.AddAction( "Debug Break" );// "Debug Break All" );
				//	group.AddAction( "Debug Stop" );// "Stop Debugging" );

				//	//group.AddAction( "Debug Restart" );
				//	group.AddAction( "Debug Next Statement" );
				//	group.AddAction( "Debug Step Into" );
				//	group.AddAction( "Debug Step Over" );
				//	group.AddAction( "Debug Step Out" );
				//}

				//External IDE
				{
					var group = new Group( "External" );
					//var group = new Group( "External IDE" );
					tab.Groups2.Add( group );

					//group.AddAction( "Open Project Solution in External IDE" );
					group.AddAction( "Open Sources Solution in External IDE" );
				}
			}

			//Windows
			{
				var tab = new Tab( "Windows", "Windows" );
				AddTab( tab );

				//Windows
				{
					var group = new Group( "Windows" );
					tab.Groups2.Add( group );

					group.AddAction( "Resources Window" );
					group.AddAction( "Stores Window" );
					//group.AddAction( "Resources Window New" );
					group.AddAction( "Objects Window" );
					//group.AddAction( "Objects Window New" );
					group.AddAction( "Settings Window" );
					//group.AddAction( "Solution Explorer" );
					group.AddAction( "Preview Window" );
					group.AddAction( "Message Log Window" );
					group.AddAction( "Output Window" );
					group.AddAction( "Debug Info Window" );

					group.AddAction( "Start Page" );
					group.AddAction( "Store" );
					//group.AddAction( "Packages" );
					group.AddAction( "Tips" );
				}

				//Settings
				{
					var group = new Group( "Settings" );
					tab.Groups2.Add( group );

					//Export
					//!!!!

					group.AddAction( "Reset Windows Settings" );
				}

				//Document
				{
					var group = new Group( "Document" );
					tab.Groups2.Add( group );

					//group.AddAction( "Viewport No Split" );
					//group.AddAction( "Viewport 2 Split" );
					//group.AddAction( "Viewport 3 Split" );
					//group.AddAction( "Viewport 4 Split" );
					//group.AddAction( "Add Viewport" );
					group.AddAction( "Find Resource" );
				}
			}

			//Tools
			{
				var tab = new Tab( "Tools", "Tools" );
				AddTab( tab );

				////Caches
				//{
				//	var group = new Group( "Caches" );
				//	tab.Groups.Add( group );

				//	group.AddAction( "Purge Images" );
				//}

				//Packing
				{
					var group = new Group( "Packing" );
					tab.Groups2.Add( group );

					group.AddAction( "Create NeoAxis Baking File" );
				}
			}

			//Scene
			{
				var tab = new Tab( "Scene Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( Scene ) ) );
				AddTab( tab );

				////New
				//{
				//	var group = new Group( "New" );
				//	tab.Groups.Add( group );

				//	group.AddAction( "New Object" );
				//	//group.AddAction( "Box" );
				//	//group.AddAction( "Sphere" );
				//	//group.AddAction( "Cylinder" );
				//	//group.AddAction( "Cone" );
				//	//group.AddAction( "Capsule" );
				//	//group.AddAction( "Light" );
				//	//group.AddAction( "Skybox" );
				//	//group.AddAction( "Fog" );
				//	//group.AddAction( "Character" );
				//	//group.AddAction( "Camera" );
				//	//group.AddAction( "Sensor" );
				//	//group.AddAction( "Particle System" );
				//	//group.AddAction( "Material" );
				//}

				//Display Settings
				{
					var group = new Group( "Display" );
					tab.Groups2.Add( group );

					{
						var groupDevData = new Group( "Display Development Data" );
						group.Children.Add( groupDevData );
						groupDevData.DropDownGroupText = ("Development", /*"  " +*/ "Data");
						groupDevData.DropDownGroupImageLarge = Properties.Resources.ArrangeUp_32;
						groupDevData.DropDownGroupDescription = "Specifies the display settings of the development data.";
						//groupDevData.ShowArrow = true;

						groupDevData.AddAction( "Scene Display Development Data In Editor" );
						groupDevData.AddAction( "Scene Display Development Data In Simulation" );
						groupDevData.AddSeparator();
						groupDevData.AddAction( "Scene Display Text Info" );
						groupDevData.AddAction( "Scene Display Labels" );
						groupDevData.AddAction( "Scene Display Lights" );
						groupDevData.AddAction( "Scene Display Decals" );
						groupDevData.AddAction( "Scene Display Reflection Probes" );
						groupDevData.AddAction( "Scene Display Cameras" );

						groupDevData.AddAction( "Scene Display Physical Objects" );
						//groupDevData.AddAction( "Scene Display Physics Static" );
						//groupDevData.AddAction( "Scene Display Physics Dynamic" );
						//groupDevData.AddAction( "Scene Display Physics Internal" );
						groupDevData.AddAction( "Scene Display Areas" );
						groupDevData.AddAction( "Scene Display Volumes" );
						groupDevData.AddAction( "Scene Display Sensors" );
						groupDevData.AddAction( "Scene Display Sound Sources" );
						groupDevData.AddAction( "Scene Display Object In Space Bounds" );
						groupDevData.AddAction( "Scene Display Scene Octree" );
						groupDevData.AddAction( "Scene Frustum Culling Test" );
						//groupDevData.AddAction("Scene Display Development Data In Simulation" );

						//var group = new Group( "Display Development Data" );
						//tab.Groups.Add( group );
						//group.AddAction( "Scene Display Development Data" );
						//group.AddAction( "Scene Display Text Info" );
						//group.AddAction( "Scene Display Labels" );
						//group.AddAction( "Scene Display Lights" );
						//group.AddAction( "Scene Display Reflection Probes" );
						//group.AddAction( "Scene Display Cameras" );
						//group.AddAction( "Scene Display Physics Static" );
						//group.AddAction( "Scene Display Physics Dynamic" );
						//group.AddAction( "Scene Display Physics Internal" );
						//group.AddAction( "Scene Display Sound Sources" );
						//group.AddAction( "Scene Display Object In Space Bounds" );
						//group.AddAction( "Scene Display Scene Octree" );
						//group.AddAction( "Scene Frustum Culling Test" );
						////group.AddAction("Scene Display Development Data In Simulation" );
					}

					{
						var groupDevData = new Group( "Debug Mode" );
						group.Children.Add( groupDevData );
						groupDevData.DropDownGroupText = ("Debug", "Mode");
						groupDevData.DropDownGroupImageLarge = Properties.Resources.RenderingDebugMode_32;
						groupDevData.DropDownGroupDescription = "Changes the debug mode of the rendering pipeline.";
						//groupDevData.ShowArrow = true;

						foreach( var mode in (RenderingPipeline_Basic.DebugModeEnum[])Enum.GetValues( typeof( RenderingPipeline_Basic.DebugModeEnum ) ) )
						{
							var modeName = EnumUtility.GetValueDisplayName( mode );
							//var modeName = TypeUtility.DisplayNameAddSpaces( mode.ToString() );
							groupDevData.AddAction( "Rendering Debug Mode " + modeName );
						}
					}
				}

				//Snap
				{
					var group = new Group( "Snap" );
					tab.Groups2.Add( group );
					group.AddAction( "Snap All Axes" );
					group.AddAction( "Snap X" );
					group.AddAction( "Snap Y" );
					group.AddAction( "Snap Z" );
				}

				//Physics
				{
					var group = new Group( "Physics" );
					tab.Groups2.Add( group );
					group.AddAction( "Add Collision" );
					group.AddAction( "Delete Collision" );
					group.AddAction( "Simulate Physics" );
				}

				//Attachment
				{
					var group = new Group( "Attachment" );
					tab.Groups2.Add( group );
					group.AddAction( "Attach Second to First" );
					group.AddAction( "Detach from Another Object" );
				}

				//Find
				{
					var group = new Group( "Find" );
					tab.Groups2.Add( group );
					group.AddAction( "Focus Camera On Selected Object" );
				}

				//Creation Of Objects
				{
					var group = new Group( "Create Objects" );
					tab.Groups2.Add( group );
					group.AddAction( "Create Objects By Drag & Drop" );
					group.AddAction( "Create Objects By Click" );
					group.AddAction( "Create Objects By Brush" );
					group.AddAction( "Create Objects Destination" );
					group.AddAction( "Create Objects Brush Radius" );
					group.AddAction( "Create Objects Brush Strength" );
					group.AddAction( "Create Objects Brush Hardness" );
				}

			}

			////Terrain
			//if( EngineInfo.ExtendedEdition )
			//{
			//	bool VisibleCondition()
			//	{
			//		var rootComponent = EditorForm.Instance.WorkspaceController.SelectedDocumentWindow?.ObjectOfWindow as Scene;
			//		return rootComponent != null && rootComponent.GetComponent<ITerrain>( true ) != null;
			//	}

			//	var tab = new Tab( "Terrain Editor", "TerrainEditor", null, VisibleCondition );
			//	Tabs.Add( tab );

			//	//Geometry
			//	{
			//		var group = new Group( "Geometry" );
			//		tab.Groups.Add( group );
			//		group.AddAction( "Terrain Geometry Raise" );
			//		group.AddAction( "Terrain Geometry Lower" );
			//		group.AddAction( "Terrain Geometry Smooth" );
			//		group.AddAction( "Terrain Geometry Flatten" );
			//	}

			//	////Hole
			//	//{
			//	//	var group = new Group( "Hole" );
			//	//	tab.Groups.Add( group );
			//	//	group.AddAction( "Terrain Hole Add" );
			//	//	group.AddAction( "Terrain Hole Delete" );
			//	//}

			//	//Shape
			//	{
			//		var group = new Group( "Shape" );
			//		tab.Groups.Add( group );
			//		group.AddAction( "Terrain Shape Circle" );
			//		group.AddAction( "Terrain Shape Square" );
			//	}

			//	//Tool Settings
			//	{
			//		var group = new Group( "Tool Settings" );
			//		tab.Groups.Add( group );
			//		group.AddAction( "Terrain Tool Radius" );
			//		group.AddAction( "Terrain Tool Strength" );
			//		group.AddAction( "Terrain Tool Hardness" );
			//	}

			//	//Painting
			//	{
			//		var group = new Group( "Paint" );
			//		tab.Groups.Add( group );
			//		group.AddAction( "Terrain Paint Paint" );
			//		group.AddAction( "Terrain Paint Clear" );
			//		group.AddAction( "Terrain Paint Smooth" );
			//		group.AddAction( "Terrain Paint Flatten" );
			//		group.AddAction( "Terrain Paint Layers" );
			//		group.AddAction( "Terrain Paint Add Layer" );
			//	}
			//}

			//Mesh
			{
				var tab = new Tab( "Mesh Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( Mesh ) ) );
				AddTab( tab );

				//Display
				{
					var group = new Group( "Display" );
					tab.Groups2.Add( group );

					group.AddAction( "Mesh Display Pivot" );
					group.AddAction( "Mesh Display Bounds" );
					group.AddAction( "Mesh Display LOD" );
					//group.AddAction( "Mesh Display Clusters" );
					group.AddAction( "Mesh Display Triangles" );
					group.AddAction( "Mesh Display Vertices" );
					//group.AddAction( "Mesh Display Vertex Color" );
					group.AddAction( "Mesh Display Normals" );
					group.AddAction( "Mesh Display Tangents" );
					group.AddAction( "Mesh Display Binormals" );
					group.AddAction( "Mesh Display Vertex Color" );
					group.AddAction( "Mesh Display UV" );
					//group.AddAction( "Mesh Display Proxy Mesh" );
					//group.AddAction( "Mesh Display LOD" );
					//group.AddAction( "Mesh Display Collision" );
					group.AddAction( "Mesh Display Skeleton" );
					group.AddAction( "Mesh Play Animation" );
					group.AddAction( "Mesh Display Collision" );
				}

				//Collision
				{
					var group = new Group( "Collision" );
					tab.Groups2.Add( group );

					group.AddAction( "Mesh Add Collision" );
					group.AddAction( "Mesh Delete Collision" );
				}

				////Structure
				//{
				//	var group = new Group( "Structure" );
				//	tab.Groups.Add( group );

				//	group.AddAction( "Mesh Build Structure" );
				//	group.AddAction( "Mesh Delete Structure" );
				//}

				//Modify
				{
					var group = new Group( "Modify" );
					tab.Groups2.Add( group );

					var groupDevData = new Group( "Mesh Add Modifier" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Add", "Modifier");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.Modify_32;
					groupDevData.DropDownGroupDescription = "Adds a new mesh modifier.";

					foreach( var type in EditorStandardActions.GetAllMeshModifiers() )
					{
						var name = type.GetUserFriendlyNameForInstance();
						groupDevData.AddAction( "Mesh Add Modifier " + name );
					}

					group.AddAction( "Mesh Add Paint Layer" );
				}
			}

			//UI Editor
			{
				var tab = new Tab( "UI Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( UIControl ) ) );
				AddTab( tab );

				//Snap
				{
					var group = new Group( "Snap" );
					tab.Groups2.Add( group );
					group.AddAction( "Snap All Axes" );
					group.AddAction( "Snap X" );
					group.AddAction( "Snap Y" );
				}

				//Horizontal Alignment
				{
					var group = new Group( "Horizontal Alignment" );
					tab.Groups2.Add( group );
					group.AddAction( "UI Align Left" );
					group.AddAction( "UI Align Center Horizontal" );
					group.AddAction( "UI Align Right" );
					group.AddAction( "UI Align Stretch Horizontal" );
				}

				//Vertical Alignment
				{
					var group = new Group( "Vertical Alignment" );
					tab.Groups2.Add( group );
					group.AddAction( "UI Align Top" );
					group.AddAction( "UI Align Center Vertical" );
					group.AddAction( "UI Align Bottom" );
					group.AddAction( "UI Align Stretch Vertical" );
				}
			}

			TerrainEditorExtensions.AddToRibbonDefaultConfiguration();
			BuilderEditorExtensions.AddToRibbonDefaultConfiguration();
		}

#endif

		public static Tab GetTab( string name )
		{
			foreach( var tab in tabs )
				if( tab.Name == name )
					return tab;
			return null;
		}

		public static void AddTab( Tab tab )
		{
			RegisterBasicTabs();
			tabs.Add( tab );
		}

		public static Tab[] Tabs
		{
			get 
			{
				RegisterBasicTabs();
				return tabs.ToArray(); 
			}
		}
	}
}

#endif