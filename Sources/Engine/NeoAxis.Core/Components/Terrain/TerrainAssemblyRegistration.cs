// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class TerrainAssemblyRegistration : AssemblyUtility.AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				RegisterTerrain();
				AddToRibbonDefaultConfiguration();
			}
		}

		static void RegisterTerrain()
		{
			//Terrain Geometry Raise
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Raise";
				a.Description = "The mode of increasing the height of the terrain. Holding down the Shift key will decrease the height.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainRaise_32;
				a.QatSupport = true;
				a.RibbonText = ("Raise", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Lower
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Lower";
				a.Description = "The mode of decreasing the height of the terrain. Holding down the Shift key will increase the height.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainLower_32;
				a.QatSupport = true;
				a.RibbonText = ("Lower", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Smooth
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Smooth";
				a.Description = "The mode of smoothing the height of the terrain.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainSmooth_32;
				a.QatSupport = true;
				a.RibbonText = ("Smooth", "");
				EditorActions.Register( a );
			}

			//Terrain Geometry Flatten
			{
				var a = new EditorAction();
				a.Name = "Terrain Geometry Flatten";
				a.Description = "The mode of flattening the height of the terrain.";
				//a.ImageSmall = Properties.Resources.Default_16;
				a.ImageBig = Properties.Resources.TerrainFlatten_32;
				a.QatSupport = true;
				a.RibbonText = ("Flatten", "");
				EditorActions.Register( a );
			}

			////Terrain Hole Add
			//{
			//	var a = new EditorAction();
			//	a.Name = "Terrain Hole Add";
			//	//!!!!
			//	//a.Description = "Focus the camera on the selected object.";
			//	a.ImageSmall = Properties.Resources.Default_16;
			//	a.ImageBig = Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Add", "");
			//	EditorActions.Register( a );
			//}

			////Terrain Hole Delete
			//{
			//	var a = new EditorAction();
			//	a.Name = "Terrain Hole Delete";
			//	//!!!!
			//	//a.Description = "Focus the camera on the selected object.";
			//	a.ImageSmall = Properties.Resources.Default_16;
			//	a.ImageBig = Properties.Resources.Default_32;
			//	a.QatSupport = true;
			//	a.RibbonText = ("Delete", "");
			//	EditorActions.Register( a );
			//}

			//Terrain Shape Circle
			{
				var a = new EditorAction();
				a.Name = "Terrain Shape Circle";
				a.Description = "The circle shape mode of the editing tool.";
				a.ImageSmall = Properties.Resources.Circle_16;
				a.ImageBig = Properties.Resources.Circle_32;
				a.QatSupport = true;
				a.RibbonText = ("Circle", "");
				EditorActions.Register( a );
			}

			//Terrain Shape Square
			{
				var a = new EditorAction();
				a.Name = "Terrain Shape Square";
				a.Description = "The square shape mode of the editing tool.";
				a.ImageSmall = Properties.Resources.Square_16;
				a.ImageBig = Properties.Resources.Square_32;
				a.QatSupport = true;
				a.RibbonText = ("Square", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Radius
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Radius";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Minimum = 0;// 0.1;
				a.Slider.Maximum = 100;
				a.Slider.ExponentialPower = 3;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolRadius;
				a.Description = "Tool size.";
				a.RibbonText = ("Radius", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Strength
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Strength";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolStrength;
				a.Description = "The strength of impact of the editing tool.";
				a.RibbonText = ("Strength", "");
				EditorActions.Register( a );
			}

			//Terrain Tool Hardness
			{
				var a = new EditorAction();
				a.Name = "Terrain Tool Hardness";
				a.ActionType = EditorAction.ActionTypeEnum.Slider;
				a.Slider.Value = Component_Scene_DocumentWindow.TerrainToolHardness;
				a.Description = "The hardness of the editing tool. Determines the strength of impact depending on the distance to the center of the tool.";
				a.RibbonText = ("Hardness", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Paint
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Paint";
				a.Description = "The mode of paint a layer. Holding down the Shift key will erasing a layer.";
				a.ImageSmall = Properties.Resources.Paint_16;
				a.ImageBig = Properties.Resources.Paint_32;
				a.QatSupport = true;
				a.RibbonText = ("Paint", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Clear
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Clear";
				a.Description = "The mode of erasing a layer. Holding down the Shift key will paint a layer.";
				a.ImageSmall = Properties.Resources.Eraser_16;
				a.ImageBig = Properties.Resources.Eraser_32;
				a.QatSupport = true;
				a.RibbonText = ("Clear", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Smooth
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Smooth";
				a.Description = "The mode of smoothing a layer.";
				a.ImageSmall = Properties.Resources.PaintSmooth_16;
				a.ImageBig = Properties.Resources.PaintSmooth_32;
				a.QatSupport = true;
				a.RibbonText = ("Smooth", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Flatten
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Flatten";
				a.Description = "The mode of flattening a layer.";
				a.ImageSmall = Properties.Resources.PaintFlatten_16;
				a.ImageBig = Properties.Resources.PaintFlatten_32;
				a.QatSupport = true;
				a.RibbonText = ("Flatten", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Layers
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Layers";
				a.ActionType = EditorAction.ActionTypeEnum.ListBox;
				a.ListBox.Length = 400;
				a.ListBox.Mode = EditorAction.ListBoxSettings.ModeEnum.Tiles;
				a.Description = "The list of layers to paint.";
				a.RibbonText = ("Layers", "");
				EditorActions.Register( a );
			}

			//Terrain Paint Add Layer
			{
				var a = new EditorAction();
				a.Name = "Terrain Paint Add Layer";
				a.ImageSmall = NeoAxis.Properties.Resources.Layers_16;
				a.ImageBig = NeoAxis.Properties.Resources.Layers_32;
				a.QatSupport = true;
				a.Description = "Add Paint Layer\nAdds a paint layer to the terrain.";
				a.RibbonText = ("Add", "Layer");
				EditorActions.Register( a );
			}

		}

		static void AddToRibbonDefaultConfiguration()
		{
			bool VisibleCondition()
			{
				var rootComponent = EditorForm.Instance.WorkspaceController.SelectedDocumentWindow?.ObjectOfWindow as Component_Scene;
				return rootComponent != null && rootComponent.GetComponent<IComponent_Terrain>( true ) != null;
			}

			var tab = new EditorRibbonDefaultConfiguration.Tab( "Terrain Editor", "TerrainEditor", null, VisibleCondition );
			EditorRibbonDefaultConfiguration.Tabs.Add( tab );

			//Geometry
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Geometry" );
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
				var group = new EditorRibbonDefaultConfiguration.Group( "Shape" );
				tab.Groups.Add( group );
				group.AddAction( "Terrain Shape Circle" );
				group.AddAction( "Terrain Shape Square" );
			}

			//Tool Settings
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Tool Settings" );
				tab.Groups.Add( group );
				group.AddAction( "Terrain Tool Radius" );
				group.AddAction( "Terrain Tool Strength" );
				group.AddAction( "Terrain Tool Hardness" );
			}

			//Painting
			{
				var group = new EditorRibbonDefaultConfiguration.Group( "Paint" );
				tab.Groups.Add( group );
				group.AddAction( "Terrain Paint Paint" );
				group.AddAction( "Terrain Paint Clear" );
				group.AddAction( "Terrain Paint Smooth" );
				group.AddAction( "Terrain Paint Flatten" );
				group.AddAction( "Terrain Paint Layers" );
				group.AddAction( "Terrain Paint Add Layer" );
			}
		}

	}
}
