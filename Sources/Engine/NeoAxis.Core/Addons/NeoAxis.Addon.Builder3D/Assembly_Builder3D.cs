// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis;
using NeoAxis.Editor;
using System.Reflection;

namespace NeoAxis.Addon.Builder3D
{
	/// <summary>
	/// The class is intended to register Builder 3D add-on.
	/// </summary>
	public class Assembly_Builder3D : AssemblyUtility.AssemblyRegistration
	{
		static EditorRibbonDefaultConfiguration.Group ribbonGroupForMeshModifiers;

		//

		public override void OnRegister()
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				RegisterEditorActions();
				AddToRibbonDefaultConfiguration();

				EditorAPI.EditorActionGetStateEvent += EditorAPI_GetEditorActionStateEvent;
				EditorAPI.EditorActionClickEvent += EditorAPI_EditorActionClickEvent;
			}
		}

		static void RegisterEditorActions()
		{
			foreach( var name in GetAllNewShapes() )
				RegisterNewShapeAction( name );

			////New Shape
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D New Shape";
			//	a.ImageSmall = NeoAxis.Properties.Resources.New_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.New_32;
			//	a.QatSupport = true;
			//	a.Description = "New Shape";
			//	a.RibbonText = ("Shape", "");
			//	EditorActions.Register( a );
			//}

			////New Polygon Base Shape
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D New Polygon Based Shape";
			//	a.ImageSmall = NeoAxis.Properties.Resources.New_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.New_32;
			//	a.QatSupport = true;
			//	a.Description = "New Polygon Based Shape";
			//	a.RibbonText = ("Polygon", "");
			//	EditorActions.Register( a );
			//}

			////New Curve Shape
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D New Curve Shape";
			//	a.ImageSmall = NeoAxis.Properties.Resources.New_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.New_32;
			//	a.QatSupport = true;
			//	a.Description = "New Curve Shape";
			//	a.RibbonText = ("Curve", "");
			//	EditorActions.Register( a );
			//}

			//Object Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Object Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectionModeObject_32;
				a.QatSupport = true;
				a.Description = "Object Mode\nThis is the usual scene editor mode, enabled by default.";
				a.RibbonText = ("Object", "");
				EditorActions.Register( a );
			}

			//Vertex Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Vertex Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectionModeVertex_32;
				a.QatSupport = true;
				a.Description = "Vertex Mode\nIn this mode you can select the vertices, move them, perform actions to edit them.";
				a.RibbonText = ("Vertex", "");
				EditorActions.Register( a );
			}

			//Edge Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Edge Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectionModeEdge_32;
				a.QatSupport = true;
				a.Description = "Edge Mode\nIn this mode you can select the edges, move them, perform actions to edit them.";
				a.RibbonText = ("Edge", "");
				EditorActions.Register( a );
			}

			//Face Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Face Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectionModeFace_32;
				a.QatSupport = true;
				a.Description = "Face Mode\nIn this mode you can select the faces, move them, perform actions to edit them.";
				a.RibbonText = ("Face", "");
				EditorActions.Register( a );
			}



			//Select All
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Select All";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectAll_32;
				a.QatSupport = true;
				a.Description = "Select All\nSelects all elements.";
				a.RibbonText = ("Select", "All");
				EditorActions.Register( a );
			}

			//Invert Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Invert Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.InvertSelection_32;
				a.QatSupport = true;
				a.Description = "Invert Selection\nSelects all the elements that are not currently selected and removes selection from the currently selected elements.";
				a.RibbonText = ("Invert", "");
				EditorActions.Register( a );
			}

			//Grow Selection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Grow Selection";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.GrowSelection_32;
				a.QatSupport = true;
				a.Description = "Grow Selection\nAdds adjacent elements to the currently selected elements.";
				a.RibbonText = ("Grow", "");
				EditorActions.Register( a );
			}

			//Select By Material
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Select By Material";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SelectByMaterial_32;
				a.QatSupport = true;
				a.Description = "Select By Material\nSelects all the faces that have the same material as currently selected faces.";
				a.RibbonText = ("By", "Material");
				EditorActions.Register( a );
			}

			////Select By Color
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D Select By Color";
			//	a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.Description = "Select By Color";
			//	a.RibbonText = ("By", "Color");
			//	EditorActions.Register( a );
			//}


			//Set Material
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Set Material";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SetMaterial_32;
				a.QatSupport = true;
				a.Description = "Set Material\nSets a material for the selected faces. The material to use has to be selected in the Resources Window.";
				a.RibbonText = ("Set", "Material");
				EditorActions.Register( a );
			}

			//Set Color
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Set Color";
				a.ImageSmall = Properties.Resources.Color_16;
				a.ImageBig = Properties.Resources.Color_32;
				a.QatSupport = true;
				a.Description = "Set Color\nSets a color for the selected objects.";
				//a.Description = "Set Color\nSets a color for the selected vertices.";
				a.RibbonText = ("Set", "Color");
				EditorActions.Register( a );
			}

			////UV Editor
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D UV Editor";
			//	a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.Description = "UV Editor";
			//	a.RibbonText = ("UV", "Editor");

			//	//a.ActionType = EditorAction.ActionTypeEnum.DropDown;
			//	//a.DropDownContextMenu = new KryptonContextMenu();
			//	//var items = new List<KryptonContextMenuItemBase>();
			//	//for( int n = 0; n < 4; n++ )
			//	//{
			//	//	var item = new KryptonContextMenuItem( "Text " + n.ToString() );
			//	//	item.Tag = n;
			//	//	items.Add( item );
			//	//}
			//	//a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );

			//	EditorActions.Register( a );
			//}




			//Merge Objects
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Merge Objects";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MergeObjects_32;
				a.QatSupport = true;
				a.Description = "Merge Objects\nCombines the selected objects into a single one.";
				a.RibbonText = ("Merge", "");
				EditorActions.Register( a );
			}

			//Mirror Objects X
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Mirror Objects X";
				a.ImageSmall = Properties.Resources.Mirror_16;
				a.ImageBig = Properties.Resources.Mirror_32;
				a.QatSupport = true;
				a.Description = "Mirror Objects";
				a.RibbonText = ("Mirror", "");
				a.ContextMenuText = "X";
				EditorActions.Register( a );
			}

			//Mirror Objects Y
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Mirror Objects Y";
				a.ImageSmall = Properties.Resources.Mirror_16;
				a.ImageBig = Properties.Resources.Mirror_32;
				a.QatSupport = true;
				a.Description = "Mirror Objects";
				a.RibbonText = ("Mirror", "");
				a.ContextMenuText = "Y";
				EditorActions.Register( a );
			}

			//Mirror Objects Z
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Mirror Objects Z";
				a.ImageSmall = Properties.Resources.Mirror_16;
				a.ImageBig = Properties.Resources.Mirror_32;
				a.QatSupport = true;
				a.Description = "Mirror Objects";
				a.RibbonText = ("Mirror", "");
				a.ContextMenuText = "Z";
				EditorActions.Register( a );
			}

			//Center Pivot
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Center Pivot";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Center Pivot";
				a.RibbonText = ("Center", "");
				EditorActions.Register( a );
			}

			//Merge Vertices Move To Middle
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Merge Vertices Move To Middle";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Merge vertices, move to the middle.";
				a.RibbonText = ("Merge", "");
				a.ContextMenuText = "Move to the Middle";
				EditorActions.Register( a );
			}

			//Merge Vertices Move To First
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Merge Vertices Move To First";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Merge vertices, move to the first vertex.";
				a.RibbonText = ("Merge", "");
				a.ContextMenuText = "Move to the First Vertex";
				EditorActions.Register( a );
			}

			//Split Vertices
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Split Vertices";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SplitVertices_32;
				a.QatSupport = true;
				a.Description = "Split Vertices\nMakes the vertices that are shared by many triangles independent.";
				a.RibbonText = ("Split", "");
				EditorActions.Register( a );
			}


			//Bridge Edges
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Bridge Edges";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.BridgeEdges_32;
				a.QatSupport = true;
				a.Description = "Bridge Edges\nConnects two selected edges with a new face.";
				a.RibbonText = ("Bridge", "");
				EditorActions.Register( a );
			}

			//Extrude Edges
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Extrude Edges";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Extrude Edges";
				a.RibbonText = ("Extrude", "");
				EditorActions.Register( a );
			}

			//Bevel Edges
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Bevel Edges";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Bevel Edges";
				a.RibbonText = ("Bevel", "");
				EditorActions.Register( a );
			}



			////Clone Faces
			//{
			//	var a = new EditorAction();
			//	a.Name = "Builder 3D Clone Faces";
			//	a.ImageSmall = NeoAxis.Properties.Resources.Copy_16;
			//	a.ImageBig = NeoAxis.Properties.Resources.Copy_32;
			//	a.QatSupport = true;
			//	a.Description = "Clone Faces";
			//	a.RibbonText = ("Clone", "");
			//	EditorActions.Register( a );
			//}

			//Merge Faces
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Merge Faces";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.MergeFaces_32;
				a.QatSupport = true;
				a.Description = "Merge Faces\nCombines selected faces into a single one.";
				a.RibbonText = ("Merge", "");
				EditorActions.Register( a );
			}

			//Detach Faces To Mesh In Space
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Detach Faces To Mesh In Space";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Detach faces to mesh in space.";
				a.RibbonText = ("Detach", "");
				a.ContextMenuText = "Detach Faces to Mesh In Space";
				EditorActions.Register( a );
			}

			//Detach Faces To Mesh Geometry
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Detach Faces To Mesh Geometry";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Detach faces to mesh geometry.";
				a.RibbonText = ("Detach", "");
				a.ContextMenuText = "Detach Faces to Mesh Geometry";
				EditorActions.Register( a );
			}

			//Detach Faces To Mesh Geometry (Split Vertices)
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Detach Faces To Mesh Geometry (Split Vertices)";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Detach faces to mesh geometry (Split Vertices).";
				a.RibbonText = ("Detach", "");
				a.ContextMenuText = "Detach Faces to Mesh Geometry (Split Vertices)";
				EditorActions.Register( a );
			}

			//Triangulate Faces
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Triangulate Faces";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TriangulateFaces_32;
				a.QatSupport = true;
				a.Description = "Triangulate Faces\nSplits selected faces to the separate triangles.";
				a.RibbonText = ("Triangulate", "");
				EditorActions.Register( a );
			}

			//Extrude Faces
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Extrude Faces";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Extrude Faces";
				a.RibbonText = ("Extrude", "");
				EditorActions.Register( a );
			}

			//Subdivide Faces
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Subdivide Faces";
				a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = NeoAxis.Properties.Resources.Default_32;
				a.QatSupport = true;
				a.Description = "Subdivide Faces";
				a.RibbonText = ("Subdivide", "");
				EditorActions.Register( a );
			}

			//Conform Normals
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Conform Normals";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.ConformNormals_32;
				a.QatSupport = true;
				a.Description = "Conform Normals\nFix invalid normals of the mesh.";
				a.RibbonText = ("Conform", "Normals");
				EditorActions.Register( a );
			}

			//Flip Normals
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Flip Normals";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.FlipNormals_32;
				a.QatSupport = true;
				a.Description = "Flip Normals\nInverts the normals of selected faces.";
				a.RibbonText = ("Flip", "Normals");
				EditorActions.Register( a );
			}

			//Smooth Normals
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Smooth Normals";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SmoothNormals_32;
				a.QatSupport = true;
				a.Description = "Smooth Normals\nSmooths the normals of selected faces.";
				a.RibbonText = ("Smooth", "Normals");
				EditorActions.Register( a );
			}

			//Flat Normals
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Flat Normals";
				////a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.FlatNormals_32;
				a.QatSupport = true;
				a.Description = "Flat Normals\nRecalculates the normals of selected faces so that they do not have smoothing.";
				a.RibbonText = ("Flat", "Normals");
				EditorActions.Register( a );
			}

			//Union
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Union";
				a.ImageSmall = Properties.Resources.Union_16;
				a.ImageBig = Properties.Resources.Union_32;
				a.QatSupport = true;
				a.Description = "Union\nExecutes constructive solid geometry union operation for two selected objects.";
				a.RibbonText = ("Union", "");
				EditorActions.Register( a );
			}

			//Difference
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Subtract";
				a.ImageSmall = Properties.Resources.Subtract_16;
				a.ImageBig = Properties.Resources.Subtract_32;
				a.QatSupport = true;
				a.Description = "Subtract\nExecutes constructive solid geometry difference operation for two selected objects.";
				a.RibbonText = ("Subtract", "");
				EditorActions.Register( a );
			}

			//Intersection
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Intersect";
				a.ImageSmall = Properties.Resources.Intersect_16;
				a.ImageBig = Properties.Resources.Intersect_32;
				a.QatSupport = true;
				a.Description = "Intersect\nExecutes constructive solid geometry intersect operation for two selected objects.";
				a.RibbonText = ("Intersect", "");
				EditorActions.Register( a );
			}

			foreach( var type in GetAllMeshModifiers( Assembly.GetExecutingAssembly() ) )
				RegisterNewMeshModifier( type );

			//Add Paint Layer
			{
				var a = new EditorAction();
				a.Name = "Builder 3D Add Paint Layer";
				a.ImageSmall = NeoAxis.Properties.Resources.Layers_16;
				a.ImageBig = NeoAxis.Properties.Resources.Layers_32;
				a.QatSupport = true;
				a.Description = "Add Paint Layer\nAdds a masked paint layer.";
				a.RibbonText = ("Add", "Layer");
				EditorActions.Register( a );
			}

			AssemblyUtility.RegisterAssemblyEvent += AssemblyUtility_RegisterAssemblyEvent;
		}

		private static void AssemblyUtility_RegisterAssemblyEvent( Assembly assembly )
		{
			//Core assembly already registered
			if( assembly == Assembly.GetExecutingAssembly() )
				return;

			foreach( var type in GetAllMeshModifiers( assembly ) )
			{
				RegisterNewMeshModifier( type );

				if( ribbonGroupForMeshModifiers != null )
				{
					var name = type.GetUserFriendlyNameForInstance();
					ribbonGroupForMeshModifiers.AddAction( "Builder 3D New Mesh Modifier " + name );
				}
			}
		}

		static void AddToRibbonDefaultConfiguration()
		{
			var tab = new EditorRibbonDefaultConfiguration.Tab( "Builder 3D", "Builder 3D", MetadataManager.GetTypeOfNetType( typeof( Component_Scene ) ) );
			EditorRibbonDefaultConfiguration.Tabs.Add( tab );

			//New
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "New" );
				tab.Groups.Add( group );

				var groupDevData = new EditorRibbonDefaultConfiguration.Group( "New Shape" );
				group.Children.Add( groupDevData );
				groupDevData.DropDownGroupText = ("Shape", "");
				groupDevData.DropDownGroupImageLarge = Properties.Resources.NewShape_32;
				groupDevData.DropDownGroupDescription = "Creates a new primitive.";
				//groupDevData.ShowArrow = true;

				foreach( var name in GetAllNewShapes() )
					groupDevData.AddAction( "Builder 3D New Shape " + name );

				//group.AddAction( "Builder 3D New Shape" );
				//group.AddAction( "Builder 3D New Polygon Based Shape" );
				//group.AddAction( "Builder 3D New Curve Shape" );
			}

			//Selection Mode
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Selection Mode" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Object Selection" );
				group.AddAction( "Builder 3D Vertex Selection" );
				group.AddAction( "Builder 3D Edge Selection" );
				group.AddAction( "Builder 3D Face Selection" );
			}

			//Selection Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Selection Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Select All" );
				group.AddAction( "Builder 3D Invert Selection" );
				group.AddAction( "Builder 3D Grow Selection" );
				group.AddAction( "Builder 3D Select By Material" );
				//group.AddAction( "Builder 3D Select By Color" );
			}

			//Material Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Material Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Set Material" );
				group.AddAction( "Builder 3D Set Color" );
				//group.AddAction( "Builder 3D UV Editor" );
			}

			//Object Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Object Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Merge Objects" );

				{
					var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Mirror" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Mirror", "");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.Mirror_32;
					groupDevData.DropDownGroupDescription = "Mirror Objects\nCreates a mirror transformation of the selected object.";
					groupDevData.AddAction( "Builder 3D Mirror Objects X" );
					groupDevData.AddAction( "Builder 3D Mirror Objects Y" );
					groupDevData.AddAction( "Builder 3D Mirror Objects Z" );
				}
				//group.AddAction( "Builder 3D Mirror Objects" );

				//group.AddAction( "Builder 3D Center Pivot" );
			}

			//Vertex Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Vertex Tools" );
				tab.Groups.Add( group );

				{
					var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Merge Vertices" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Merge", "");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.MergeVertices_32;
					groupDevData.DropDownGroupDescription = "Merge Vertices\nMerges selected vertices into the single vertex.";
					groupDevData.AddAction( "Builder 3D Merge Vertices Move To Middle" );
					groupDevData.AddAction( "Builder 3D Merge Vertices Move To First" );
				}

				group.AddAction( "Builder 3D Split Vertices" );
			}

			//Edge Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Edge" );
				//var group = new EditorRibbonDefaultConfiguration.Group( "Edge Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Bridge Edges" );
				//group.AddAction( "Builder 3D Extrude Edges" );
				//group.AddAction( "Builder 3D Bevel Edges" );
			}

			//Face Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Face Tools" );
				tab.Groups.Add( group );
				//group.AddAction( "Builder 3D Set Material" );
				//group.AddAction( "Builder 3D UV Editor" );

				//!!!!Clone, Delete по сути с помощью базовых действий

				//group.AddAction( "Builder 3D Clone Faces" );
				group.AddAction( "Builder 3D Merge Faces" );
				group.AddAction( "Builder 3D Triangulate Faces" );

				{
					var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Detach Faces" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Detach", "");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.DetachFaces_32;
					groupDevData.DropDownGroupDescription = "Detach Faces\nRemoves selected faces from a geometry and places them in a new geometry.";
					groupDevData.AddAction( "Builder 3D Detach Faces To Mesh Geometry" );
					groupDevData.AddAction( "Builder 3D Detach Faces To Mesh Geometry (Split Vertices)" );
					groupDevData.AddAction( "Builder 3D Detach Faces To Mesh In Space" );
				}

				//group.AddAction( "Builder 3D Extrude Faces" );
				//group.AddAction( "Builder 3D Subdivide Faces" );
				group.AddAction( "Builder 3D Conform Normals" );
				group.AddAction( "Builder 3D Flip Normals" );
				group.AddAction( "Builder 3D Smooth Normals" );
				group.AddAction( "Builder 3D Flat Normals" );
			}

			//Boolean
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Boolean" );
				tab.Groups.Add( group );
				group.AddAction( "Builder 3D Union" );
				group.AddAction( "Builder 3D Subtract" );
				group.AddAction( "Builder 3D Intersect" );
			}

			//Modify
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Modify" );
				tab.Groups.Add( group );

				var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Add Mesh Modifier" );
				group.Children.Add( groupDevData );
				groupDevData.DropDownGroupText = ("Add", "Modifier");
				groupDevData.DropDownGroupImageLarge = NeoAxis.Properties.Resources.Modify_32;
				groupDevData.DropDownGroupDescription = "Add Mesh Modifier\nAdds a mesh modifier to the object.";

				ribbonGroupForMeshModifiers = groupDevData;

				foreach( var type in GetAllMeshModifiers( Assembly.GetExecutingAssembly() ) )
				{
					var name = type.GetUserFriendlyNameForInstance();
					groupDevData.AddAction( "Builder 3D New Mesh Modifier " + name );
				}

				group.AddAction( "Builder 3D Add Paint Layer" );
			}
		}

		static List<string> GetAllNewShapes()
		{
			var result = new List<string>();
			result.Add( "Arch" );
			result.Add( "Box" );
			result.Add( "Capsule" );
			result.Add( "Cone" );
			result.Add( "Cylinder" );
			result.Add( "Door" );
			result.Add( "Pipe" );
			result.Add( "Plane" );
			result.Add( "Prism" );
			result.Add( "Sphere" );
			result.Add( "Stairs" );
			result.Add( "Torus" );
			result.Add( "PolygonBasedPolyhedron" );
			return result;
		}

		static void RegisterNewShapeAction( string name )
		{
			var displayName = TypeUtility.DisplayNameAddSpaces( name );

			var a = new EditorAction();
			a.Name = "Builder 3D New Shape " + name;
			a.ImageSmall = NeoAxis.Properties.Resources.New_16;
			a.ImageBig = Properties.Resources.NewShape_32;
			a.QatSupport = true;
			var text = displayName;
			a.RibbonText = (text, "");
			a.ContextMenuText = text;
			a.UserData = name;
			a.GetState += delegate ( EditorAction.GetStateContext context )
			{
				if( context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow != null )
					context.Enabled = true;
			};
			a.Click += delegate ( EditorAction.ClickContext context )
			{
				var documentWindow = context.ObjectsInFocus.DocumentWindow as Component_Scene_DocumentWindow;
				var scene = documentWindow.Scene;
				var name2 = (string)a.UserData;

				var selectedType = MetadataManager.GetType( "NeoAxis.Component_MeshGeometry_" + name2 );

				var meshInSpace = scene.CreateComponent<Component_MeshInSpace>( enabled: false );
				meshInSpace.Name = EditorUtility.GetUniqueFriendlyName( meshInSpace, selectedType.GetUserFriendlyNameForInstance() );

				var mesh = meshInSpace.CreateComponent<Component_Mesh>();
				mesh.Name = "Mesh";

				var geometry = mesh.CreateComponent( selectedType );
				geometry.Name = "Mesh Geometry";

				meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

				documentWindow.CalculateCreateObjectPosition( meshInSpace, meshInSpace, new Vector2( 0.5, 0.5 ) );

				meshInSpace.Enabled = true;

				if( selectedType == MetadataManager.GetTypeOfNetType( typeof( Component_MeshGeometry_PolygonBasedPolyhedron ) ) )
				{
					documentWindow.StartObjectCreationMode( selectedType, meshInSpace );
					//delete first start point which created in CreationMode class
					foreach( var c in geometry.GetComponents<Component_MeshGeometry_PolygonBasedPolyhedron_Point>() )
						c.RemoveFromParent( false );
				}
				else
				{
					//undo
					var newObjects = new Component[] { meshInSpace };
					var document = context.ObjectsInFocus.DocumentWindow.Document;
					var action = new UndoActionComponentCreateDelete( document, newObjects, true );
					document.CommitUndoAction( action );
					documentWindow.SelectObjects( newObjects );
				}

				documentWindow.Focus();
			};
			EditorActions.Register( a );
		}

		static List<Metadata.TypeInfo> GetAllMeshModifiers( Assembly assembly )
		{
			var result = new List<Metadata.TypeInfo>();

			foreach( var netType in assembly.GetTypes() )
			{
				try
				{
					if( typeof( Component_MeshModifier ).IsAssignableFrom( netType ) && !netType.IsAbstract )
					{
						var type = MetadataManager.GetTypeOfNetType( netType );
						if( type != null )
							result.Add( type );
					}
				}
				catch { }
			}

			CollectionUtility.InsertionSort( result, delegate ( Metadata.TypeInfo type1, Metadata.TypeInfo type2 )
			{
				if( type1 == MetadataManager.GetTypeOfNetType( typeof( Component_MeshModifier ) ) )
					return -1;
				if( type2 == MetadataManager.GetTypeOfNetType( typeof( Component_MeshModifier ) ) )
					return 1;

				var name1 = type1.GetUserFriendlyNameForInstance();
				var name2 = type2.GetUserFriendlyNameForInstance();
				return string.Compare( name1, name2 );
			} );

			return result;
		}

		static void RegisterNewMeshModifier( Metadata.TypeInfo type )
		{
			var name = type.GetUserFriendlyNameForInstance();

			List<Component_Mesh> GetSelectedMeshes( object[] objects )
			{
				var result = new List<Component_Mesh>();

				foreach( var obj in objects )
				{
					if( obj is Component_Mesh mesh )
						result.Add( mesh );

					var meshInSpace = obj as Component_MeshInSpace;
					if( meshInSpace != null )
					{
						var mesh2 = meshInSpace.Mesh.Value;
						if( mesh2 != null )
							result.Add( mesh2 );
					}
				}

				return result;
			}

			var a = new EditorAction();
			a.Name = "Builder 3D New Mesh Modifier " + name;
			a.ImageSmall = NeoAxis.Properties.Resources.Modify_16;
			a.ImageBig = NeoAxis.Properties.Resources.Modify_32;
			a.QatSupport = true;
			a.RibbonText = (name, "");
			a.ContextMenuText = name;
			a.UserData = type;
			a.GetState += delegate ( EditorAction.GetStateContext context )
			{
				if( GetSelectedMeshes( context.ObjectsInFocus.Objects ).Count != 0 )
					context.Enabled = true;
			};
			a.Click += delegate ( EditorAction.ClickContext context )
			{
				var meshes = GetSelectedMeshes( context.ObjectsInFocus.Objects );
				if( meshes.Count != 0 )
				{
					var documentWindow = context.ObjectsInFocus.DocumentWindow;
					var type2 = (Metadata.TypeInfo)a.UserData;

					var newObjects = new List<Component>();

					foreach( var mesh in meshes )
					{
						var modifier = mesh.CreateComponent( type2, enabled: false );
						modifier.Name = CommonFunctions.GetUniqueFriendlyName( modifier );
						modifier.Enabled = true;

						newObjects.Add( modifier );

						documentWindow.Focus();
					}

					//undo
					var document = context.ObjectsInFocus.DocumentWindow.Document;
					var action = new UndoActionComponentCreateDelete( document, newObjects, true );
					document.CommitUndoAction( action );
					documentWindow.SelectObjects( newObjects.ToArray() );
				}
			};
			EditorActions.Register( a );
		}

		private void EditorAPI_GetEditorActionStateEvent( EditorAction.GetStateContext context )
		{
			var actionContext = new ActionContext( context );

			switch( context.Action.Name )
			{
			//case "Builder 3D New Shape":
			//	context.Enabled = true;
			//	break;
			//case "Builder 3D New Polygon Base Shape":
			//	context.Enabled = true;
			//	break;
			//case "Builder 3D New Curve Shape":
			//	context.Enabled = true;
			//	break;

			case "Builder 3D Object Selection":
				context.Enabled = actionContext.DocumentWindow is Component_Scene_DocumentWindow;
				context.Checked = actionContext.BuilderWorkareaMode == null;
				break;
			case "Builder 3D Vertex Selection":
				context.Enabled = actionContext.DocumentWindow is Component_Scene_DocumentWindow;
				context.Checked = actionContext.SelectionMode == SelectionMode.Vertex;
				break;
			case "Builder 3D Edge Selection":
				context.Enabled = actionContext.DocumentWindow is Component_Scene_DocumentWindow;
				context.Checked = actionContext.SelectionMode == SelectionMode.Edge;
				break;
			case "Builder 3D Face Selection":
				context.Enabled = actionContext.DocumentWindow is Component_Scene_DocumentWindow;
				context.Checked = actionContext.SelectionMode == SelectionMode.Face;
				break;

			case "Builder 3D Merge Vertices Move To Middle":
			case "Builder 3D Merge Vertices Move To First":
				OneMeshActions.MergeVerticesGetState( context, actionContext );
				break;

			case "Builder 3D Split Vertices":
				OneMeshActions.SplitVerticesGetState( context, actionContext );
				break;

			case "Builder 3D Select All":
				actionContext.BuilderWorkareaMode?.SelectAllGetState( context );
				break;

			case "Builder 3D Invert Selection":
				actionContext.BuilderWorkareaMode?.InvertSelectionGetState( context );
				break;

			case "Builder 3D Bridge Edges":
				OneMeshActions.BridgeEdgesGetState( context, actionContext );
				break;

			case "Builder 3D Conform Normals":
				OneMeshActions.ConformNormalsGetState( context, actionContext );
				break;

			case "Builder 3D Flat Normals":
				OneMeshActions.FlatNormalsGetState( context, actionContext );
				break;

			case "Builder 3D Smooth Normals":
				OneMeshActions.SmoothNormalsGetState( context, actionContext );
				break;

			case "Builder 3D Flip Normals":
				OneMeshActions.FlipNormalsGetState( context, actionContext );
				break;

			case "Builder 3D Merge Faces":
				OneMeshActions.MergeFacesGetState( context, actionContext );
				break;

			case "Builder 3D Triangulate Faces":
				OneMeshActions.TriangulateFacesGetState( context, actionContext );
				break;

			case "Builder 3D Detach Faces To Mesh Geometry (Split Vertices)":
				OneMeshActions.DetachFacesGetState( context, actionContext, false );
				break;

			case "Builder 3D Detach Faces To Mesh Geometry":
				OneMeshActions.DetachFacesGetState( context, actionContext, false );
				break;

			case "Builder 3D Detach Faces To Mesh In Space":
				OneMeshActions.DetachFacesGetState( context, actionContext, true );
				break;

			case "Builder 3D Set Material":
				OneMeshActions.SetMaterialGetState( context, actionContext );
				break;

			case "Builder 3D Set Color":
				Actions.SetColorGetState( context, actionContext );
				break;

			case "Builder 3D Grow Selection":
				OneMeshActions.GrowSelectionGetState( context, actionContext );
				break;
			case "Builder 3D Select By Material":
				OneMeshActions.SelectByMaterialGetState( context, actionContext );
				break;

			case "Builder 3D Merge Objects":
				Actions.MergeObjectsGetState( context, actionContext );
				break;

			case "Builder 3D Mirror Objects X":
			case "Builder 3D Mirror Objects Y":
			case "Builder 3D Mirror Objects Z":
				Actions.MirrorObjectsGetState( context, actionContext );
				break;

			case "Builder 3D Union":
			case "Builder 3D Intersect":
			case "Builder 3D Subtract":
				Actions.BoolActionGetState( context, actionContext );
				break;

			case "Builder 3D Add Paint Layer":
				Actions.AddPaintLayerGetState( context, actionContext );
				break;

			}
		}

		private void EditorAPI_EditorActionClickEvent( EditorAction.ClickContext context )
		{
			var actionContext = new ActionContext( context );

			switch( context.Action.Name )
			{
			//case "Builder 3D New Shape":
			//	{
			//	}
			//	break;

			//case "Builder 3D New Polygon Base Shape":
			//	{
			//	}
			//	break;

			//case "Builder 3D New Curve Shape":
			//	{
			//	}
			//	break;

			case "Builder 3D Object Selection":
			case "Builder 3D Vertex Selection":
			case "Builder 3D Edge Selection":
			case "Builder 3D Face Selection":
				{
					////change selection mode
					//switch( context.Action.Name )
					//{
					//case "Builder 3D Object Selection": Settings.SelectionMode = SelectionMode.Object; break;
					//case "Builder 3D Vertex Selection": Settings.SelectionMode = SelectionMode.Vertex; break;
					//case "Builder 3D Edge Selection": Settings.SelectionMode = SelectionMode.Edge; break;
					//case "Builder 3D Face Selection": Settings.SelectionMode = SelectionMode.Face; break;
					//}

					//change workarea mode, selection mode
					//!!!!temp
					var sceneDocumentWindow = actionContext.DocumentWindow as Component_Scene_DocumentWindow;
					if( sceneDocumentWindow != null )
					{
						if( context.Action.Name != "Builder 3D Object Selection" )
						{
							if( sceneDocumentWindow.WorkareaModeName != "Builder3D" )
							{
								var workareaMode2 = new BuilderWorkareaMode( sceneDocumentWindow );
								sceneDocumentWindow.WorkareaModeSet( "Builder3D", workareaMode2 );
							}

							var workareaMode3 = sceneDocumentWindow.WorkareaMode as BuilderWorkareaMode;
							if( workareaMode3 != null )
							{
								switch( context.Action.Name )
								{
								case "Builder 3D Vertex Selection": workareaMode3.ChangeSelectionMode( SelectionMode.Vertex ); break;
								case "Builder 3D Edge Selection": workareaMode3.ChangeSelectionMode( SelectionMode.Edge ); break;
								case "Builder 3D Face Selection": workareaMode3.ChangeSelectionMode( SelectionMode.Face ); break;
								}
							}
						}
						else
							sceneDocumentWindow.WorkareaModeSet( "" );
					}
				}
				break;

			//case "Builder 3D Object Selection":
			//	{
			//		//change selection mode
			//		Settings.SelectionMode = SelectionMode.Object;

			//		//change workarea mode
			//		var documentWindow = context.ObjectsInFocus.DocumentWindow;
			//		//!!!!temp
			//		var sceneDocumentWindow = documentWindow as Component_Scene_DocumentWindow;
			//		if( sceneDocumentWindow != null )
			//		{
			//			sceneDocumentWindow.WorkareaModeSet( "" );
			//		}
			//	}
			//	break;

			//case "Builder 3D Edge Selection":
			//	Settings.SelectionMode = SelectionMode.Edge;
			//	break;
			//case "Builder 3D Face Selection":
			//	Settings.SelectionMode = SelectionMode.Face;
			//	break;

			case "Builder 3D Merge Vertices Move To Middle":
			case "Builder 3D Merge Vertices Move To First":
				{
					bool toFirst = context.Action.Name == "Builder 3D Merge Vertices Move To First";
					//var vertices = BuilderWorkareaMode.GetSelectedVertices( documentWindow );
					OneMeshActions.MergeVertices( actionContext, toFirst );
					//change selection
					//workareaMode?.SelectVertices( new int[] { vertices[ 0 ] } );
				}
				break;

			case "Builder 3D Split Vertices":
				OneMeshActions.SplitVertices( actionContext, false, true );
				break;

			case "Builder 3D Select All":
				actionContext.BuilderWorkareaMode?.SelectAll();
				break;

			case "Builder 3D Invert Selection":
				actionContext.BuilderWorkareaMode?.InvertSelection();
				break;

			case "Builder 3D Bridge Edges":
				OneMeshActions.BridgeEdges( actionContext );
				break;

			case "Builder 3D Conform Normals":
				OneMeshActions.ConformNormals( actionContext );
				break;

			case "Builder 3D Flat Normals":
				OneMeshActions.FlatNormals( actionContext );
				break;

			case "Builder 3D Smooth Normals":
				OneMeshActions.SmoothNormals( actionContext );
				break;

			case "Builder 3D Flip Normals":
				OneMeshActions.FlipNormals( actionContext );
				break;

			case "Builder 3D Merge Faces":
				OneMeshActions.MergeFaces( actionContext );
				break;
			case "Builder 3D Triangulate Faces":
				OneMeshActions.TriangulateFaces( actionContext );
				break;
			case "Builder 3D Detach Faces To Mesh Geometry":
				OneMeshActions.DetachFaces( actionContext, false, false );
				break;
			case "Builder 3D Detach Faces To Mesh Geometry (Split Vertices)":
				OneMeshActions.DetachFaces( actionContext, false, true );
				break;
			case "Builder 3D Detach Faces To Mesh In Space":
				OneMeshActions.DetachFaces( actionContext, true, true );
				break;

			case "Builder 3D Set Material":
				OneMeshActions.SetMaterial( actionContext );
				break;

			case "Builder 3D Set Color":
				{
					var initialColor = Actions.GetInitialColor( actionContext ) ?? new ColorValue( 1, 1, 1 );

					var form = new ColorValuePoweredSelectForm();
					form.StartPosition = FormStartPosition.Manual;
					form.Location = new System.Drawing.Point( Cursor.Position.X + 10, Cursor.Position.Y + 10 );
					form.Init( initialColor.ToColorValuePowered(), false, false, null, false );
					if( form.ShowDialog( EditorForm.Instance ) == DialogResult.OK )
					{
						Actions.SetColor( actionContext, form.CurrentValue.ToColorValue() );
					}
				}
				break;

			case "Builder 3D Grow Selection":
				OneMeshActions.GrowSelection( actionContext );
				break;
			case "Builder 3D Select By Material":
				OneMeshActions.SelectByMaterial( actionContext );
				break;
			case "Builder 3D Merge Objects":
				Actions.MergeObjects( actionContext );
				break;

			case "Builder 3D Mirror Objects X":
				Actions.MirrorObjects( actionContext, 0 );
				break;
			case "Builder 3D Mirror Objects Y":
				Actions.MirrorObjects( actionContext, 1 );
				break;
			case "Builder 3D Mirror Objects Z":
				Actions.MirrorObjects( actionContext, 2 );
				break;

			case "Builder 3D Union":
				Actions.BoolAction( actionContext, Actions.BoolActionEnum.Union );
				break;
			case "Builder 3D Intersect":
				Actions.BoolAction( actionContext, Actions.BoolActionEnum.Intersect );
				break;
			case "Builder 3D Subtract":
				Actions.BoolAction( actionContext, Actions.BoolActionEnum.Subtract );
				break;

			case "Builder 3D Add Paint Layer":
				Actions.AddPaintLayer( actionContext );
				break;

			}
		}
	}
}
#endif