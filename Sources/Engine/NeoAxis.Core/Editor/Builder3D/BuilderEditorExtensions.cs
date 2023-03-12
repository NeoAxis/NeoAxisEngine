// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;

namespace NeoAxis.Editor
{
	/// <summary>
	/// The class is intended to register Modeling add-on.
	/// </summary>
	class BuilderEditorExtensions : EditorExtensions
	{
		static EditorRibbonDefaultConfiguration.Group ribbonGroupForMeshModifiers;

		//

		public override void OnRegister()
		{
			RegisterEditorActions();
			//AddToRibbonDefaultConfiguration();

			EditorAPI.EditorActionGetStateEvent += EditorAPI_GetEditorActionStateEvent;
			EditorAPI.EditorActionClickEvent += EditorAPI_EditorActionClickEvent;
		}

		static void RegisterEditorActions()
		{
			foreach( var name in GetAllNewShapes() )
				RegisterNewShapeAction( name );

			////New Shape
			//{
			//	var a = new EditorAction();
			//	a.Name = "Modeling New Shape";
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
			//	a.Name = "Modeling New Polygon Based Shape";
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
			//	a.Name = "Modeling New Curve Shape";
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
				a.Name = "Modeling Object Selection";
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
				a.Name = "Modeling Vertex Selection";
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
				a.Name = "Modeling Edge Selection";
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
				a.Name = "Modeling Face Selection";
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
				a.Name = "Modeling Select All";
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
				a.Name = "Modeling Invert Selection";
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
				a.Name = "Modeling Grow Selection";
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
				a.Name = "Modeling Select By Material";
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
			//	a.Name = "Modeling Select By Color";
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
				a.Name = "Modeling Set Material";
				//a.ImageSmall = NeoAxis.Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.SetMaterial_32;
				a.QatSupport = true;
				a.Description = "Set Material\nSets a material for the selected objects or faces. The material must be selected in the Resources Window. Also the material can be changed from the Settings Window.";
				//a.Description = "Set Material\nSets a material for the selected objects or faces. The material must be selected in the Resources Window or when click the action.";
				a.RibbonText = ("Set", "Material");
				EditorActions.Register( a );
			}

			//Set Color
			{
				var a = new EditorAction();
				a.Name = "Modeling Set Color";
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
			//	a.Name = "Modeling UV Editor";
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
				a.Name = "Modeling Merge Objects";
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
				a.Name = "Modeling Mirror Objects X";
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
				a.Name = "Modeling Mirror Objects Y";
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
				a.Name = "Modeling Mirror Objects Z";
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
				a.Name = "Modeling Center Pivot";
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
				a.Name = "Modeling Merge Vertices Move To Middle";
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
				a.Name = "Modeling Merge Vertices Move To First";
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
				a.Name = "Modeling Split Vertices";
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
				a.Name = "Modeling Bridge Edges";
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
				a.Name = "Modeling Extrude Edges";
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
				a.Name = "Modeling Bevel Edges";
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
			//	a.Name = "Modeling Clone Faces";
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
				a.Name = "Modeling Merge Faces";
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
				a.Name = "Modeling Detach Faces To Mesh In Space";
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
				a.Name = "Modeling Detach Faces To Mesh Geometry";
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
				a.Name = "Modeling Detach Faces To Mesh Geometry (Split Vertices)";
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
				a.Name = "Modeling Triangulate Faces";
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
				a.Name = "Modeling Extrude Faces";
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
				a.Name = "Modeling Subdivide Faces";
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
				a.Name = "Modeling Conform Normals";
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
				a.Name = "Modeling Flip Normals";
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
				a.Name = "Modeling Smooth Normals";
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
				a.Name = "Modeling Flat Normals";
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
				a.Name = "Modeling Union";
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
				a.Name = "Modeling Subtract";
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
				a.Name = "Modeling Intersect";
				a.ImageSmall = Properties.Resources.Intersect_16;
				a.ImageBig = Properties.Resources.Intersect_32;
				a.QatSupport = true;
				a.Description = "Intersect\nExecutes constructive solid geometry intersect operation for two selected objects.";
				a.RibbonText = ("Intersect", "");
				EditorActions.Register( a );
			}

			foreach( var type in GetMeshModifiersOfAssembly( Assembly.GetExecutingAssembly() ) )
				RegisterNewMeshModifier( type, false );

			//Add Paint Layer
			{
				var a = new EditorAction();
				a.Name = "Modeling Add Paint Layer";
				a.ImageSmall = NeoAxis.Properties.Resources.Layers_16;
				a.ImageBig = NeoAxis.Properties.Resources.Layers_32;
				a.QatSupport = true;
				a.Description = "Add Paint Layer\nAdds a masked paint layer.";
				a.RibbonText = ("Add", "Layer");
				EditorActions.Register( a );
			}

			//Export To FBX
			{
				var a = new EditorAction();
				a.Name = "Modeling Export To FBX";
				a.ImageSmall = Properties.Resources.External_16;
				a.ImageBig = Properties.Resources.External_32;
				a.QatSupport = true;
				a.Description = "Export To FBX\nExports the selected 3D model to a FBX file.";
				a.RibbonText = ("Export", "FBX");
				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					var objects = context.ObjectsInFocus.Objects;
					if( objects.Length == 1 )
					{
						var obj = objects[ 0 ];
						if( obj is MeshInSpace || obj is Mesh )
							context.Enabled = true;
					}
				};
				a.Click += delegate ( EditorAction.ClickContext context )
				{
					var objects = context.ObjectsInFocus.Objects;
					if( objects.Length == 1 )
					{
						var obj = objects[ 0 ];

						Mesh mesh = obj as Mesh;
						if( mesh == null && obj is MeshInSpace meshInSpace )
							mesh = meshInSpace.Mesh.Value;

						if( mesh != null )
						{
							if( !EditorUtility.ShowSaveFileDialog( "", "Mesh.fbx", "FBX files (*.fbx)|*.fbx", out var fileName ) )
								return;

							if( !EditorAssemblyInterface.Instance.ExportToFBX( mesh, fileName, out var error ) )
								EditorMessageBox.ShowWarning( error );
							//if( !mesh.ExportToFBX( fileName, out var error ) )
							//	EditorMessageBox.ShowWarning( error );
						}
					}
				};
				EditorActions.Register( a );
			}

			Internal.AssemblyUtility.RegisterAssemblyEvent += AssemblyUtility_RegisterAssemblyEvent;
			Internal.AssemblyUtility.UnregisterAssemblyEvent += AssemblyUtility_UnregisterAssemblyEvent;
		}

		static void RegisterAssemblyMeshModifiers( Assembly assembly, bool unregister )
		{
			//Core assembly already registered
			if( assembly == Assembly.GetExecutingAssembly() )
				return;

			foreach( var type in GetMeshModifiersOfAssembly( assembly ) )
			{
				RegisterNewMeshModifier( type, unregister );

				if( ribbonGroupForMeshModifiers != null )
				{
					var name = type.GetUserFriendlyNameForInstance();
					if( unregister )
						ribbonGroupForMeshModifiers.RemoveAction( "Modeling New Mesh Modifier " + name );
					else
						ribbonGroupForMeshModifiers.AddAction( "Modeling New Mesh Modifier " + name );
				}
			}
		}

		private static void AssemblyUtility_RegisterAssemblyEvent( Assembly assembly, Assembly reloadingOldAssembly )
		{
			RegisterAssemblyMeshModifiers( assembly, false );
		}

		private static void AssemblyUtility_UnregisterAssemblyEvent( Assembly assembly, Assembly reloadingNewAssembly )
		{
			RegisterAssemblyMeshModifiers( assembly, true );
		}

		internal static void AddToRibbonDefaultConfiguration()
		{
			var tab = new EditorRibbonDefaultConfiguration.Tab( "Modeling", "Modeling", MetadataManager.GetTypeOfNetType( typeof( Scene ) ) );
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
					groupDevData.AddAction( "Modeling New Shape " + name );

				//group.AddAction( "Modeling New Shape" );
				//group.AddAction( "Modeling New Polygon Based Shape" );
				//group.AddAction( "Modeling New Curve Shape" );
			}

			//Selection Mode
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Selection Mode" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Object Selection" );
				group.AddAction( "Modeling Vertex Selection" );
				group.AddAction( "Modeling Edge Selection" );
				group.AddAction( "Modeling Face Selection" );
			}

			//Selection Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Selection Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Select All" );
				group.AddAction( "Modeling Invert Selection" );
				group.AddAction( "Modeling Grow Selection" );
				group.AddAction( "Modeling Select By Material" );
				//group.AddAction( "Modeling Select By Color" );
			}

			//Material Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Material Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Set Material" );
				group.AddAction( "Modeling Set Color" );
				//group.AddAction( "Modeling UV Editor" );
			}

			//Object Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Object Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Merge Objects" );

				{
					var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Mirror" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Mirror", "");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.Mirror_32;
					groupDevData.DropDownGroupDescription = "Mirror Objects\nCreates a mirror transformation of the selected object.";
					groupDevData.AddAction( "Modeling Mirror Objects X" );
					groupDevData.AddAction( "Modeling Mirror Objects Y" );
					groupDevData.AddAction( "Modeling Mirror Objects Z" );
				}
				//group.AddAction( "Modeling Mirror Objects" );

				//group.AddAction( "Modeling Center Pivot" );
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
					groupDevData.AddAction( "Modeling Merge Vertices Move To Middle" );
					groupDevData.AddAction( "Modeling Merge Vertices Move To First" );
				}

				group.AddAction( "Modeling Split Vertices" );
			}

			//Edge Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Edge" );
				//var group = new EditorRibbonDefaultConfiguration.Group( "Edge Tools" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Bridge Edges" );
				//group.AddAction( "Modeling Extrude Edges" );
				//group.AddAction( "Modeling Bevel Edges" );
			}

			//Face Tools
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Face Tools" );
				tab.Groups.Add( group );
				//group.AddAction( "Modeling Set Material" );
				//group.AddAction( "Modeling UV Editor" );

				//!!!!Clone, Delete с помощью базовых действий

				//group.AddAction( "Modeling Clone Faces" );
				group.AddAction( "Modeling Merge Faces" );
				group.AddAction( "Modeling Triangulate Faces" );

				{
					var groupDevData = new EditorRibbonDefaultConfiguration.Group( "Detach Faces" );
					group.Children.Add( groupDevData );
					groupDevData.DropDownGroupText = ("Detach", "");
					groupDevData.DropDownGroupImageLarge = Properties.Resources.DetachFaces_32;
					groupDevData.DropDownGroupDescription = "Detach Faces\nRemoves selected faces from a geometry and places them in a new geometry.";
					groupDevData.AddAction( "Modeling Detach Faces To Mesh Geometry" );
					groupDevData.AddAction( "Modeling Detach Faces To Mesh Geometry (Split Vertices)" );
					groupDevData.AddAction( "Modeling Detach Faces To Mesh In Space" );
				}

				//group.AddAction( "Modeling Extrude Faces" );
				//group.AddAction( "Modeling Subdivide Faces" );
				group.AddAction( "Modeling Conform Normals" );
				group.AddAction( "Modeling Flip Normals" );
				group.AddAction( "Modeling Smooth Normals" );
				group.AddAction( "Modeling Flat Normals" );
			}

			//Boolean
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Boolean" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Union" );
				group.AddAction( "Modeling Subtract" );
				group.AddAction( "Modeling Intersect" );
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

				//add actions for mesh modifiers
				foreach( var type in GetMeshModifiersOfAssembly( Assembly.GetExecutingAssembly() ) )
				{
					var name = type.GetUserFriendlyNameForInstance();
					groupDevData.AddAction( "Modeling New Mesh Modifier " + name );
				}
				foreach( var item in Internal.AssemblyUtility.RegisteredAssemblies )
				{
					if( item.Assembly != Assembly.GetExecutingAssembly() )
					{
						foreach( var type in GetMeshModifiersOfAssembly( item.Assembly ) )
						{
							var name = type.GetUserFriendlyNameForInstance();
							groupDevData.AddAction( "Modeling New Mesh Modifier " + name );
						}
					}
				}

				group.AddAction( "Modeling Add Paint Layer" );
			}

			//Export To FBX
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Export" );
				tab.Groups.Add( group );
				group.AddAction( "Modeling Export To FBX" );
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
			a.Name = "Modeling New Shape " + name;
			a.ImageSmall = NeoAxis.Properties.Resources.New_16;
			a.ImageBig = Properties.Resources.NewShape_32;
			a.QatSupport = true;
			var text = displayName;
			a.RibbonText = (text, "");
			a.ContextMenuText = text;
			a.UserData = name;
			a.GetState += delegate ( EditorAction.GetStateContext context )
			{
				if( context.ObjectsInFocus.DocumentWindow as SceneEditor != null )
					context.Enabled = true;
			};
			a.Click += delegate ( EditorAction.ClickContext context )
			{
				var documentWindow = context.ObjectsInFocus.DocumentWindow as SceneEditor;
				var scene = documentWindow.Scene;
				var name2 = (string)a.UserData;

				var selectedType = MetadataManager.GetType( "NeoAxis.MeshGeometry_" + name2 );

				var meshInSpace = scene.CreateComponent<MeshInSpace>( enabled: false );
				meshInSpace.Name = EditorUtility.GetUniqueFriendlyName( meshInSpace, selectedType.GetUserFriendlyNameForInstance() );

				var mesh = meshInSpace.CreateComponent<Mesh>();
				mesh.Name = "Mesh";

				var geometry = mesh.CreateComponent( selectedType );
				geometry.Name = "Mesh Geometry";

				meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

				documentWindow.CalculateCreateObjectPosition( meshInSpace, meshInSpace, new Vector2( 0.5, 0.5 ) );

				meshInSpace.Enabled = true;

				if( selectedType == MetadataManager.GetTypeOfNetType( typeof( MeshGeometry_PolygonBasedPolyhedron ) ) )
				{
					documentWindow.StartObjectCreationMode( selectedType, meshInSpace );
					//delete first start point which created in CreationMode class
					foreach( var c in geometry.GetComponents<MeshGeometry_PolygonBasedPolyhedron_Point>() )
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

		static List<Metadata.TypeInfo> GetMeshModifiersOfAssembly( Assembly assembly )
		{
			var result = new List<Metadata.TypeInfo>();

			foreach( var netType in assembly.GetTypes() )
			{
				try
				{
					if( typeof( MeshModifier ).IsAssignableFrom( netType ) && !netType.IsAbstract )
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
				if( type1 == MetadataManager.GetTypeOfNetType( typeof( MeshModifier ) ) )
					return -1;
				if( type2 == MetadataManager.GetTypeOfNetType( typeof( MeshModifier ) ) )
					return 1;

				var name1 = type1.GetUserFriendlyNameForInstance();
				var name2 = type2.GetUserFriendlyNameForInstance();
				return string.Compare( name1, name2 );
			} );

			return result;
		}

		static List<(MeshInSpace, Mesh)> GetSelectedMeshes( object[] objects )
		{
			var result = new List<(MeshInSpace, Mesh)>();

			foreach( var obj in objects )
			{
				if( obj is Mesh mesh )
					result.Add( (null, mesh) );

				var meshInSpace = obj as MeshInSpace;
				if( meshInSpace != null )
				{
					var mesh2 = meshInSpace.Mesh.Value;
					if( mesh2 != null )
						result.Add( (meshInSpace, mesh2) );
				}
			}

			return result;
		}

		static void RegisterNewMeshModifier( Metadata.TypeInfo type, bool unregister )
		{
			var name = type.GetUserFriendlyNameForInstance();

			var actionName = "Modeling New Mesh Modifier " + name;

			if( unregister )
			{
				EditorActions.Unregister( actionName );
			}
			else
			{
				var a = new EditorAction();
				a.Name = actionName;//"Modeling New Mesh Modifier " + name;
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
						var document = documentWindow.Document;
						var type2 = (Metadata.TypeInfo)a.UserData;

						var newObjects = new List<Component>();

						var undo = new List<UndoSystem.Action>();
						var undoLocked = false;

						foreach( (var meshInSpace, var mesh2) in meshes )
						{
							var mesh = mesh2;

							//make copy of the mesh if needed
							if( meshInSpace != null )
								BuilderCommonFunctions.CopyExternalMesh( document, meshInSpace, ref mesh, undo, ref undoLocked );

							var modifier = mesh.CreateComponent( type2, enabled: false );
							modifier.Name = BuilderCommonFunctions.GetUniqueFriendlyName( modifier );
							modifier.Enabled = true;

							newObjects.Add( modifier );

							documentWindow.Focus();
						}

						//undo
						var undoMultiAction = new UndoMultiAction();
						if( undo.Count != 0 )
							undoMultiAction.AddActions( undo );
						undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, newObjects, true ) );
						document.CommitUndoAction( undoMultiAction );

						//select the new objects
						documentWindow.SelectObjects( newObjects.ToArray() );
					}
				};
				EditorActions.Register( a );
			}
		}

		private void EditorAPI_GetEditorActionStateEvent( EditorAction.GetStateContext context )
		{
			var actionContext = new BuilderActionContext( context );

			switch( context.Action.Name )
			{
			//case "Modeling New Shape":
			//	context.Enabled = true;
			//	break;
			//case "Modeling New Polygon Base Shape":
			//	context.Enabled = true;
			//	break;
			//case "Modeling New Curve Shape":
			//	context.Enabled = true;
			//	break;

			case "Modeling Object Selection":
				context.Enabled = actionContext.DocumentWindow is SceneEditor;
				context.Checked = actionContext.BuilderWorkareaMode == null;
				break;
			case "Modeling Vertex Selection":
				context.Enabled = actionContext.DocumentWindow is SceneEditor;
				context.Checked = actionContext.SelectionMode == BuilderSelectionMode.Vertex;
				break;
			case "Modeling Edge Selection":
				context.Enabled = actionContext.DocumentWindow is SceneEditor;
				context.Checked = actionContext.SelectionMode == BuilderSelectionMode.Edge;
				break;
			case "Modeling Face Selection":
				context.Enabled = actionContext.DocumentWindow is SceneEditor;
				context.Checked = actionContext.SelectionMode == BuilderSelectionMode.Face;
				break;

			case "Modeling Merge Vertices Move To Middle":
			case "Modeling Merge Vertices Move To First":
				BuilderOneMeshActions.MergeVerticesGetState( context, actionContext );
				break;

			case "Modeling Split Vertices":
				BuilderOneMeshActions.SplitVerticesGetState( context, actionContext );
				break;

			case "Modeling Select All":
				actionContext.BuilderWorkareaMode?.SelectAllGetState( context );
				break;

			case "Modeling Invert Selection":
				actionContext.BuilderWorkareaMode?.InvertSelectionGetState( context );
				break;

			case "Modeling Bridge Edges":
				BuilderOneMeshActions.BridgeEdgesGetState( context, actionContext );
				break;

			case "Modeling Conform Normals":
				BuilderOneMeshActions.ConformNormalsGetState( context, actionContext );
				break;

			case "Modeling Flat Normals":
				BuilderOneMeshActions.FlatNormalsGetState( context, actionContext );
				break;

			case "Modeling Smooth Normals":
				BuilderOneMeshActions.SmoothNormalsGetState( context, actionContext );
				break;

			case "Modeling Flip Normals":
				BuilderOneMeshActions.FlipNormalsGetState( context, actionContext );
				break;

			case "Modeling Merge Faces":
				BuilderOneMeshActions.MergeFacesGetState( context, actionContext );
				break;

			case "Modeling Triangulate Faces":
				BuilderOneMeshActions.TriangulateFacesGetState( context, actionContext );
				break;

			case "Modeling Detach Faces To Mesh Geometry (Split Vertices)":
				BuilderOneMeshActions.DetachFacesGetState( context, actionContext, false );
				break;

			case "Modeling Detach Faces To Mesh Geometry":
				BuilderOneMeshActions.DetachFacesGetState( context, actionContext, false );
				break;

			case "Modeling Detach Faces To Mesh In Space":
				BuilderOneMeshActions.DetachFacesGetState( context, actionContext, true );
				break;

			case "Modeling Set Material":
				BuilderOneMeshActions.SetMaterialGetState( context, actionContext );
				break;

			case "Modeling Set Color":
				BuilderActions.SetColorGetState( context, actionContext );
				break;

			case "Modeling Grow Selection":
				BuilderOneMeshActions.GrowSelectionGetState( context, actionContext );
				break;
			case "Modeling Select By Material":
				BuilderOneMeshActions.SelectByMaterialGetState( context, actionContext );
				break;

			case "Modeling Merge Objects":
				BuilderActions.MergeObjectsGetState( context, actionContext );
				break;

			case "Modeling Mirror Objects X":
			case "Modeling Mirror Objects Y":
			case "Modeling Mirror Objects Z":
				BuilderActions.MirrorObjectsGetState( context, actionContext );
				break;

			case "Modeling Union":
			case "Modeling Intersect":
			case "Modeling Subtract":
				BuilderActions.BoolActionGetState( context, actionContext );
				break;

			case "Modeling Add Paint Layer":
				BuilderActions.AddPaintLayerGetState( context, actionContext );
				break;

			}
		}

		private void EditorAPI_EditorActionClickEvent( EditorAction.ClickContext context )
		{
			var actionContext = new BuilderActionContext( context );

			switch( context.Action.Name )
			{
			//case "Modeling New Shape":
			//	{
			//	}
			//	break;

			//case "Modeling New Polygon Base Shape":
			//	{
			//	}
			//	break;

			//case "Modeling New Curve Shape":
			//	{
			//	}
			//	break;

			case "Modeling Object Selection":
			case "Modeling Vertex Selection":
			case "Modeling Edge Selection":
			case "Modeling Face Selection":
				{
					////change selection mode
					//switch( context.Action.Name )
					//{
					//case "Modeling Object Selection": Settings.SelectionMode = SelectionMode.Object; break;
					//case "Modeling Vertex Selection": Settings.SelectionMode = SelectionMode.Vertex; break;
					//case "Modeling Edge Selection": Settings.SelectionMode = SelectionMode.Edge; break;
					//case "Modeling Face Selection": Settings.SelectionMode = SelectionMode.Face; break;
					//}

					//change workarea mode, selection mode
					var sceneDocumentWindow = actionContext.DocumentWindow as SceneEditor;
					if( sceneDocumentWindow != null )
					{
						if( context.Action.Name != "Modeling Object Selection" )
						{
							if( sceneDocumentWindow.WorkareaModeName != "Modeling" )
							{
								var workareaMode2 = new BuilderWorkareaMode( sceneDocumentWindow );
								sceneDocumentWindow.WorkareaModeSet( "Modeling", workareaMode2 );
							}

							var workareaMode3 = sceneDocumentWindow.WorkareaMode as BuilderWorkareaMode;
							if( workareaMode3 != null )
							{
								switch( context.Action.Name )
								{
								case "Modeling Vertex Selection": workareaMode3.ChangeSelectionMode( BuilderSelectionMode.Vertex ); break;
								case "Modeling Edge Selection": workareaMode3.ChangeSelectionMode( BuilderSelectionMode.Edge ); break;
								case "Modeling Face Selection": workareaMode3.ChangeSelectionMode( BuilderSelectionMode.Face ); break;
								}
							}
						}
						else
							sceneDocumentWindow.WorkareaModeSet( "" );
					}
				}
				break;

			//case "Modeling Object Selection":
			//	{
			//		//change selection mode
			//		Settings.SelectionMode = SelectionMode.Object;

			//		//change workarea mode
			//		var documentWindow = context.ObjectsInFocus.DocumentWindow;
			//		var sceneDocumentWindow = documentWindow as Scene_DocumentWindow;
			//		if( sceneDocumentWindow != null )
			//		{
			//			sceneDocumentWindow.WorkareaModeSet( "" );
			//		}
			//	}
			//	break;

			//case "Modeling Edge Selection":
			//	Settings.SelectionMode = SelectionMode.Edge;
			//	break;
			//case "Modeling Face Selection":
			//	Settings.SelectionMode = SelectionMode.Face;
			//	break;

			case "Modeling Merge Vertices Move To Middle":
			case "Modeling Merge Vertices Move To First":
				{
					bool toFirst = context.Action.Name == "Modeling Merge Vertices Move To First";
					//var vertices = BuilderWorkareaMode.GetSelectedVertices( documentWindow );
					BuilderOneMeshActions.MergeVertices( actionContext, toFirst );
					//change selection
					//workareaMode?.SelectVertices( new int[] { vertices[ 0 ] } );
				}
				break;

			case "Modeling Split Vertices":
				BuilderOneMeshActions.SplitVertices( actionContext, false, true );
				break;

			case "Modeling Select All":
				actionContext.BuilderWorkareaMode?.SelectAll();
				break;

			case "Modeling Invert Selection":
				actionContext.BuilderWorkareaMode?.InvertSelection();
				break;

			case "Modeling Bridge Edges":
				BuilderOneMeshActions.BridgeEdges( actionContext );
				break;

			case "Modeling Conform Normals":
				BuilderOneMeshActions.ConformNormals( actionContext );
				break;

			case "Modeling Flat Normals":
				BuilderOneMeshActions.FlatNormals( actionContext );
				break;

			case "Modeling Smooth Normals":
				BuilderOneMeshActions.SmoothNormals( actionContext );
				break;

			case "Modeling Flip Normals":
				BuilderOneMeshActions.FlipNormals( actionContext );
				break;

			case "Modeling Merge Faces":
				BuilderOneMeshActions.MergeFaces( actionContext );
				break;
			case "Modeling Triangulate Faces":
				BuilderOneMeshActions.TriangulateFaces( actionContext );
				break;
			case "Modeling Detach Faces To Mesh Geometry":
				BuilderOneMeshActions.DetachFaces( actionContext, false, false );
				break;
			case "Modeling Detach Faces To Mesh Geometry (Split Vertices)":
				BuilderOneMeshActions.DetachFaces( actionContext, false, true );
				break;
			case "Modeling Detach Faces To Mesh In Space":
				BuilderOneMeshActions.DetachFaces( actionContext, true, true );
				break;

			case "Modeling Set Material":
				BuilderOneMeshActions.SetMaterial( actionContext );
				break;

			case "Modeling Set Color":
				{
					var initialColor = BuilderActions.GetInitialColor( actionContext ) ?? new ColorValue( 1, 1, 1 );

					var location = new System.Drawing.Point( Cursor.Position.X + 10, Cursor.Position.Y + 10 );
					if( EditorAssemblyInterface.Instance.ColorValuePoweredSelectFormShowDialog( location, initialColor.ToColorValuePowered(), out var resultColor ) )
						BuilderActions.SetColor( actionContext, resultColor.ToColorValue() );

					//var form = new ColorValuePoweredSelectForm();
					//form.StartPosition = FormStartPosition.Manual;
					//form.Location = new System.Drawing.Point( Cursor.Position.X + 10, Cursor.Position.Y + 10 );
					//form.Init( initialColor.ToColorValuePowered(), false, false, null, false );
					//if( form.ShowDialog( EditorForm.Instance ) == DialogResult.OK )
					//	BuilderActions.SetColor( actionContext, form.CurrentValue.ToColorValue() );
				}
				break;

			case "Modeling Grow Selection":
				BuilderOneMeshActions.GrowSelection( actionContext );
				break;
			case "Modeling Select By Material":
				BuilderOneMeshActions.SelectByMaterial( actionContext );
				break;
			case "Modeling Merge Objects":
				BuilderActions.MergeObjects( actionContext );
				break;

			case "Modeling Mirror Objects X":
				BuilderActions.MirrorObjects( actionContext, 0 );
				break;
			case "Modeling Mirror Objects Y":
				BuilderActions.MirrorObjects( actionContext, 1 );
				break;
			case "Modeling Mirror Objects Z":
				BuilderActions.MirrorObjects( actionContext, 2 );
				break;

			case "Modeling Union":
				BuilderActions.BoolAction( actionContext, BuilderActions.BoolActionEnum.Union );
				break;
			case "Modeling Intersect":
				BuilderActions.BoolAction( actionContext, BuilderActions.BoolActionEnum.Intersect );
				break;
			case "Modeling Subtract":
				BuilderActions.BoolAction( actionContext, BuilderActions.BoolActionEnum.Subtract );
				break;

			case "Modeling Add Paint Layer":
				BuilderActions.AddPaintLayer( actionContext );
				break;
			}
		}
	}
}
#endif