// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ComponentFactory.Krypton.Ribbon;

namespace NeoAxis.Editor
{
	public static class EditorRibbonDefaultConfiguration
	{
		public static List<Tab> Tabs = new List<Tab>();

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

		public class Group
		{
			public string Name;
			public (string, string) DropDownGroupText;
			public Image DropDownGroupImageSmall;
			public Image DropDownGroupImageLarge;
			public string DropDownGroupDescription = "";
			//can be Group, Action, action name (string), null (separator)
			public List<object> Children = new List<object>();
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

			public void AddSeparator()
			{
				Children.Add( null );
			}
		}

		/////////////////////////////////////////

		public class Tab
		{
			public string Name;
			public string Type;

			public Metadata.TypeInfo VisibleOnlyForType;

			public delegate bool VisibleConditionDelegate();
			public VisibleConditionDelegate VisibleCondition;

			public List<Group> Groups = new List<Group>();

			public Tab()
			{
			}

			public Tab( string name, string type, Metadata.TypeInfo visibleOnlyForType = null, VisibleConditionDelegate visibleCondition = null )
			{
				Name = name;
				Type = type;
				VisibleOnlyForType = visibleOnlyForType;
				VisibleCondition = visibleCondition;
			}
		}

		/////////////////////////////////////////

#if !DEPLOY

		static EditorRibbonDefaultConfiguration()
		{
			EditorStandardActions.Register();

			//Home
			{
				//!!!!
				var tab = new Tab( "Home", "Home" );
				//var tab = new Tab( "General" );
				Tabs.Add( tab );

				//New
				{
					var group = new Group( "Resource" );
					//var group = new Group( "New" );
					tab.Groups.Add( group );

					group.AddAction( "New Resource" );
					group.AddAction( "Import Resource" );
				}

				//Save
				{
					var group = new Group( "Save" );
					tab.Groups.Add( group );

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
					tab.Groups.Add( group );

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
					tab.Groups.Add( group );

					group.AddAction( "Select" );
					group.AddAction( "Move" );
					group.AddAction( "Rotate" );
					group.AddAction( "Scale" );
					//!!!!
					//group.AddAction( "Move, Rotate, Scale" );
					group.AddAction( "Transform Using Local Coordinates" );
				}

				//Simulation
				{
					var group = new Group( "Play" );
					tab.Groups.Add( group );

					group.AddAction( "Play" );
					group.AddAction( "Run Player" );
					group.AddAction( "Run Device" );
					//group.AddAction( "Run Device 2" );
				}

				//Project
				{
					var group = new Group( "Project" );
					tab.Groups.Add( group );

					group.AddAction( "Project Settings" );
				}

				//Additions
				{
					var group = new Group( "Additions" );
					tab.Groups.Add( group );

					group.AddAction( "Store" );
					group.AddAction( "Packages" );
					//group.AddAction( "Addons" );
					//group.AddAction( "Store" );
				}

				//Docs
				{
					var group = new Group( "Docs" );
					tab.Groups.Add( group );

					group.AddAction( "Manual" );
					group.AddAction( "Tips" );
				}
			}

			//Scripting
			{
				var tab = new Tab( "Scripting", "Scripting" );
				Tabs.Add( tab );

				//Project's Solution
				{
					var group = new Group( "Solution" );
					tab.Groups.Add( group );

					group.AddAction( "Build Project's Solution" );
					group.AddAction( "Build and Update Project's Solution" );
					//group.AddAction( "Find in Files" );
				}

				//C# Project
				{
					var group = new Group( "C# Project" );
					//var group = new Group( "C# project" );
					tab.Groups.Add( group );

					group.AddAction( "C# File" );
					group.AddAction( "Add C# files to Project.csproj" );
					group.AddAction( "Remove C# files from Project.csproj" );
					//group.AddAction( "Add to Project.csproj" );
					//group.AddAction( "Remove from Project.csproj" );
				}

				//Components
				{
					var group = new Group( "Components" );
					tab.Groups.Add( group );

					group.AddAction( "C# Script" );
					group.AddAction( "Flow Graph" );
				}

				//C#
				{
					var group = new Group( "C# Editing" );
					tab.Groups.Add( group );

					group.AddAction( "Comment Selection" );
					group.AddAction( "Uncomment Selection" );
					group.AddAction( "Rename" );
					group.AddAction( "Format Document" );
					group.AddAction( "Add Property Code" );
				}

				//Debug
				{
					var group = new Group( "Debug" );
					tab.Groups.Add( group );

					group.AddAction( "Debug Start" );// Start Debugging" );
					group.AddAction( "Debug Break" );// "Debug Break All" );
					group.AddAction( "Debug Stop" );// "Stop Debugging" );

					//group.AddAction( "Debug Restart" );
					group.AddAction( "Debug Next Statement" );
					group.AddAction( "Debug Step Into" );
					group.AddAction( "Debug Step Over" );
					group.AddAction( "Debug Step Out" );
				}

				//External IDE
				{
					var group = new Group( "External" );
					//var group = new Group( "External IDE" );
					tab.Groups.Add( group );

					//group.AddAction( "Open Project Solution in External IDE" );
					group.AddAction( "Open Sources Solution in External IDE" );
				}
			}

			//Windows
			{
				var tab = new Tab( "Windows", "Windows" );
				Tabs.Add( tab );

				//Windows
				{
					var group = new Group( "Windows" );
					tab.Groups.Add( group );

					group.AddAction( "Resources Window" );
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
					group.AddAction( "Packages" );
					group.AddAction( "Tips" );
				}

				//Settings
				{
					var group = new Group( "Settings" );
					tab.Groups.Add( group );

					//Export
					//!!!!

					group.AddAction( "Reset Windows Settings" );
				}

				//Document
				{
					var group = new Group( "Document" );
					tab.Groups.Add( group );

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
				Tabs.Add( tab );

				//Caches
				{
					var group = new Group( "Caches" );
					tab.Groups.Add( group );

					group.AddAction( "Purge Images" );
				}

				//Packing
				{
					var group = new Group( "Packing" );
					tab.Groups.Add( group );

					group.AddAction( "Create NeoAxis Baking File" );
				}
			}

			//Scene
			{
				var tab = new Tab( "Scene Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( Component_Scene ) ) );
				Tabs.Add( tab );

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
					tab.Groups.Add( group );

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

				//Snap
				{
					var group = new Group( "Snap" );
					tab.Groups.Add( group );
					group.AddAction( "Snap All Axes" );
					group.AddAction( "Snap X" );
					group.AddAction( "Snap Y" );
					group.AddAction( "Snap Z" );
				}

				//Physics
				{
					var group = new Group( "Physics" );
					tab.Groups.Add( group );
					group.AddAction( "Add Collision" );
					group.AddAction( "Delete Collision" );
				}

				//Attachment
				{
					var group = new Group( "Attachment" );
					tab.Groups.Add( group );
					group.AddAction( "Attach Second to First" );
					group.AddAction( "Detach from Another Object" );
				}

				//Find
				{
					var group = new Group( "Find" );
					tab.Groups.Add( group );
					group.AddAction( "Focus Camera On Selected Object" );
				}

				//Creation Of Objects
				{
					var group = new Group( "Create Objects" );
					tab.Groups.Add( group );
					group.AddAction( "Create Objects By Drag & Drop" );
					group.AddAction( "Create Objects By Click" );
					group.AddAction( "Create Objects By Brush" );
					group.AddAction( "Create Objects Destination" );
					group.AddAction( "Create Objects Brush Radius" );
					group.AddAction( "Create Objects Brush Strength" );
					group.AddAction( "Create Objects Brush Hardness" );
				}

			}

			//Terrain
			{
				bool VisibleCondition()
				{
					var rootComponent = EditorForm.Instance.WorkspaceController.SelectedDocumentWindow?.ObjectOfWindow as Component_Scene;
					return rootComponent != null && rootComponent.GetComponent<Component_Terrain>( true ) != null;
				}

				var tab = new Tab( "Terrain Editor", "TerrainEditor", null, VisibleCondition );
				Tabs.Add( tab );

				//Geometry
				{
					var group = new Group( "Geometry" );
					tab.Groups.Add( group );
					group.AddAction( "Terrain Geometry Raise" );
					group.AddAction( "Terrain Geometry Lower" );
					group.AddAction( "Terrain Geometry Smooth" );
					group.AddAction( "Terrain Geometry Flatten" );
				}

				////Hole
				//{
				//	var group = new Group( "Hole" );
				//	tab.Groups.Add( group );
				//	group.AddAction( "Terrain Hole Add" );
				//	group.AddAction( "Terrain Hole Delete" );
				//}

				//Shape
				{
					var group = new Group( "Shape" );
					tab.Groups.Add( group );
					group.AddAction( "Terrain Shape Circle" );
					group.AddAction( "Terrain Shape Square" );
				}

				//Tool Settings
				{
					var group = new Group( "Tool Settings" );
					tab.Groups.Add( group );
					group.AddAction( "Terrain Tool Radius" );
					group.AddAction( "Terrain Tool Strength" );
					group.AddAction( "Terrain Tool Hardness" );
				}

				//Painting
				{
					var group = new Group( "Paint" );
					tab.Groups.Add( group );
					group.AddAction( "Terrain Paint Paint" );
					group.AddAction( "Terrain Paint Clear" );
					group.AddAction( "Terrain Paint Smooth" );
					group.AddAction( "Terrain Paint Flatten" );
					group.AddAction( "Terrain Paint Layers" );
				}

			}

			//Mesh
			{
				var tab = new Tab( "Mesh Editor", "ComponentTypeSpecific", MetadataManager.GetTypeOfNetType( typeof( Component_Mesh ) ) );
				Tabs.Add( tab );

				//Display
				{
					var group = new Group( "Display" );
					tab.Groups.Add( group );

					group.AddAction( "Mesh Display Pivot" );
					group.AddAction( "Mesh Display Bounds" );
					group.AddAction( "Mesh Display Triangles" );
					group.AddAction( "Mesh Display Vertices" );
					group.AddAction( "Mesh Display Vertex Color" );
					group.AddAction( "Mesh Display Normals" );
					group.AddAction( "Mesh Display Tangents" );
					group.AddAction( "Mesh Display Binormals" );
					//group.AddAction( "Mesh Display Vertex Color" );
					group.AddAction( "Mesh Display UV" );
					group.AddAction( "Mesh Display LOD" );
					//group.AddAction( "Mesh Display Collision" );
					group.AddAction( "Mesh Display Skeleton" );
					group.AddAction( "Mesh Play Animation" );
					group.AddAction( "Mesh Display Collision" );
				}

				//Collision
				{
					var group = new Group( "Collision" );
					tab.Groups.Add( group );

					group.AddAction( "Mesh Add Collision" );
					group.AddAction( "Mesh Delete Collision" );
				}

				//Structure
				{
					var group = new Group( "Structure" );
					tab.Groups.Add( group );

					group.AddAction( "Mesh Add Structure" );
					group.AddAction( "Mesh Delete Structure" );
				}

				//Modify
				{
					var group = new Group( "Modify" );
					tab.Groups.Add( group );

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
				Tabs.Add( tab );

				//Snap
				{
					var group = new Group( "Snap" );
					tab.Groups.Add( group );
					group.AddAction( "Snap All Axes" );
					group.AddAction( "Snap X" );
					group.AddAction( "Snap Y" );
				}
			}
		}

#endif

		public static Tab GetTab( string name )
		{
			foreach( var tab in Tabs )
				if( tab.Name == name )
					return tab;
			return null;
		}
	}
}
