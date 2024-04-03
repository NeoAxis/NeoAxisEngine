#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class EditorImageHelper
	{
		static Image defaultSmallImage;
		static Image defaultLargeImage;

		ImageCollection imageListIconsSmall = new ImageCollection();
		ImageCollection imageListIconsLarge = new ImageCollection();

		Dictionary<string, Image> scaledImagesCacheForTree = new Dictionary<string, Image>();
		Dictionary<string, Image> scaledImagesCacheForTreeDisabled = new Dictionary<string, Image>();

		Dictionary<(string, int), Image> imageListDisabled = new Dictionary<(string, int), Image>();

		///////////////////////////////////////////////

		static EditorImageHelper()
		{
			defaultSmallImage = Properties.Resources.Default_16;
			defaultLargeImage = Properties.Resources.Default_32;
		}

		public void Dispose()
		{
			//!!!!?
		}

		public void AddImage( string key, System.Drawing.Image smallImage, System.Drawing.Image largeImage )
		{
			if( !imageListIconsSmall.ContainsKey( key ) )
				imageListIconsSmall.Add( key, smallImage ?? defaultSmallImage );

			if( !imageListIconsLarge.ContainsKey( key ) )
				imageListIconsLarge.Add( key, largeImage ?? defaultLargeImage );
		}

		//tree view right now has no the ability to configure in the editor. image size always same, depending font size of the system
		public Image GetImageScaledForTreeView( string key, bool disabled )
		{
			if( string.IsNullOrEmpty( key ) )
				key = "Default";

			//add if still not exists
			if( !scaledImagesCacheForTree.ContainsKey( key ) )
			{
				if( !imageListIconsSmall.ContainsKey( key ) )
					return null;

				var scaledImage = EditorAPI2.GetImageForDispalyScale( imageListIconsSmall[ key ], imageListIconsLarge[ key ] );//, useCache );
				scaledImagesCacheForTree[ key ] = scaledImage;
			}

			var image = scaledImagesCacheForTree[ key ];

			if( disabled )
			{
				if( !scaledImagesCacheForTreeDisabled.TryGetValue( key, out var imageDisabled ) )
				{
					try
					{
						imageDisabled = ToolStripRenderer.CreateDisabledImage( image );
					}
					catch { }

					if( imageDisabled != null )
						scaledImagesCacheForTreeDisabled[ key ] = imageDisabled;
				}

				image = imageDisabled;
			}

			return image;
		}

		public Image GetImage( string key, int requestedSize, bool disabled )
		{
			if( string.IsNullOrEmpty( key ) )
				key = "Default";

			Image image = null;
			if( requestedSize <= 16 )
				image = imageListIconsSmall[ key ];
			if( image == null )
				image = imageListIconsLarge[ key ];
			if( image == null )
				image = imageListIconsSmall[ key ];

			if( disabled && image != null )
			{
				var key2 = (key, requestedSize);

				if( !imageListDisabled.TryGetValue( key2, out var imageDisabled ) )
				{
					try
					{
						imageDisabled = ToolStripRenderer.CreateDisabledImage( image );
					}
					catch { }

					if( imageDisabled != null )
						imageListDisabled[ key2 ] = imageDisabled;
				}

				image = imageDisabled;
			}

			return image;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class EditorImageHelperBasicImages
	{
		static EditorImageHelper helper = new EditorImageHelper();

		public static EditorImageHelper Helper
		{
			get { return helper; }
		}

		static EditorImageHelperBasicImages()
		{
			Helper.AddImage( "Default", Properties.Resources.Default_16, Properties.Resources.Default_32 );
			Helper.AddImage( "Namespace", Properties.Resources.Namespace_16, null );
			Helper.AddImage( "Class", Properties.Resources.Class, Properties.Resources.Class_32 );
			Helper.AddImage( "Struct", Properties.Resources.Struct, null );
			Helper.AddImage( "Assembly", Properties.Resources.Assembly, null );

			//if( EditorAPI.DarkTheme )
			//	Helper.AddImage( "Resource", Properties.Resources.Resource_16_Dark, Properties.Resources.Resource_32_Dark );
			//else
			Helper.AddImage( "Resource", Properties.Resources.Resource_16, Properties.Resources.Resource_32 );

			Helper.AddImage( "AssemblyList", Properties.Resources.AssemblyList, null );
			Helper.AddImage( "Folder", Properties.Resources.Folder_16, Properties.Resources.Folder_32 );
			Helper.AddImage( "Delegate", Properties.Resources.Delegate, null );
			Helper.AddImage( "Enum", Properties.Resources.Enum, null );
			Helper.AddImage( "Property", Properties.Resources.Property, null );
			Helper.AddImage( "GoUpper", Properties.Resources.GoUpper_16, null );
			Helper.AddImage( "Method", Properties.Resources.Method, null );
			Helper.AddImage( "Event", Properties.Resources.Event_16, null );
			Helper.AddImage( "StaticClass", Properties.Resources.StaticClass, null );
			Helper.AddImage( "StaticEvent", Properties.Resources.StaticEvent, null );
			Helper.AddImage( "StaticMethod", Properties.Resources.StaticMethod, null );
			Helper.AddImage( "StaticProperty", Properties.Resources.StaticProperty, null );
			Helper.AddImage( "Constructor", Properties.Resources.Constructor, null );
			Helper.AddImage( "Operator", Properties.Resources.Operator, null );

			Helper.AddImage( "CSharp", Properties.Resources.CSharp_16, Properties.Resources.CSharp_32 );
			Helper.AddImage( "UI", Properties.Resources.Window_16, Properties.Resources.Window_32 );
			Helper.AddImage( "Image", Properties.Resources.Image_16, Properties.Resources.Image_32 );
			Helper.AddImage( "Sound", Properties.Resources.Sound_16, Properties.Resources.Sound_32 );
			Helper.AddImage( "Mesh", Properties.Resources.Mesh_16, Properties.Resources.Mesh_32 );
			Helper.AddImage( "Material", Properties.Resources.Material_16, Properties.Resources.Material_32 );
			Helper.AddImage( "Scene", Properties.Resources.Scene_16, Properties.Resources.Scene_32 );
			Helper.AddImage( "Product", Properties.Resources.Package_16, Properties.Resources.Package_32 );
			Helper.AddImage( "Store Product", Properties.Resources.Package_16, Properties.Resources.Package_32 );

			Helper.AddImage( "Cog", Properties.Resources.Cog_16, Properties.Resources.Cog_32 );

			Helper.AddImage( "CSharpProject", Properties.Resources.CSharpProject_16, Properties.Resources.CSharpProject_32 );
			Helper.AddImage( "Attach", Properties.Resources.Attach_16, Properties.Resources.Attach_32 );
			Helper.AddImage( "New", Properties.Resources.New_16, Properties.Resources.New_32 );


			Helper.AddImage( "Camera", Properties.Resources.Camera_16, Properties.Resources.Camera_32 );
			Helper.AddImage( "Light", Properties.Resources.Light_16, Properties.Resources.Light_32 );
			Helper.AddImage( "Sky", Properties.Resources.Sky_16, Properties.Resources.Sky_32 );
			Helper.AddImage( "Character", Properties.Resources.Character_16, Properties.Resources.Character_32 );
			Helper.AddImage( "Weapon", Properties.Resources.Weapon_16, Properties.Resources.Weapon_32 );
			Helper.AddImage( "Vehicle", Properties.Resources.Vehicle_16, Properties.Resources.Vehicle_32 );
			Helper.AddImage( "SpawnPoint", Properties.Resources.SpawnPoint_16, Properties.Resources.SpawnPoint_32 );
			Helper.AddImage( "Building", Properties.Resources.Building_16, Properties.Resources.Building_32 );
			Helper.AddImage( "AI", Properties.Resources.AI_16, Properties.Resources.AI_32 );
			Helper.AddImage( "GameMode", Properties.Resources.GameMode_16, Properties.Resources.GameMode_32 );
			Helper.AddImage( "NetworkLogic", Properties.Resources.NetworkLogic_16, Properties.Resources.NetworkLogic_32 );
			Helper.AddImage( "RenderingPipeline", Properties.Resources.RenderingPipeline_16, Properties.Resources.RenderingPipeline_32 );
			Helper.AddImage( "Button", Properties.Resources.Button_16, Properties.Resources.Button_32 );
			Helper.AddImage( "Regulator", Properties.Resources.Regulator_16, Properties.Resources.Regulator_32 );
			Helper.AddImage( "MeasuringTool", Properties.Resources.MeasuringTool_16, Properties.Resources.MeasuringTool_32 );
			Helper.AddImage( "MeshInSpace", Properties.Resources.MeshInSpace_16, Properties.Resources.MeshInSpace_32 );
			Helper.AddImage( "GroupOfObjects", Properties.Resources.GroupOfObjects_16, Properties.Resources.GroupOfObjects_32 );
			Helper.AddImage( "Terrain", Properties.Resources.Terrain_16, Properties.Resources.Terrain_32 );
			//Helper.AddImage( "BasicItem", Properties.Resources.BasicItem_16, Properties.Resources.BasicItem_32 );


			//!!!!
			//CoreExtension.dll
			Helper.AddImage( "Plant", Properties.Resources.Plant_16, Properties.Resources.Plant_32 );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class EditorImageHelperComponentTypes
	{
		public delegate void GetImageKeyEventDelegate( Metadata.TypeInfo type, ref string imageKey );
		public static event GetImageKeyEventDelegate GetImageKeyEvent;

		public static string GetImageKey( Metadata.TypeInfo type )
		{
			try
			{
				string imageKey = null;
				GetImageKeyEvent?.Invoke( type, ref imageKey );
				if( !string.IsNullOrEmpty( imageKey ) )
					return imageKey;

				if( MetadataManager.GetTypeOfNetType( typeof( Scene ) ).IsAssignableFrom( type ) )
					return "Scene";
				else if( MetadataManager.GetTypeOfNetType( typeof( Product_Store ) ).IsAssignableFrom( type ) )
					return "Store Product";
				else if( MetadataManager.GetTypeOfNetType( typeof( Product ) ).IsAssignableFrom( type ) )
					return "Product";
				else if( MetadataManager.GetTypeOfNetType( typeof( Camera ) ).IsAssignableFrom( type ) )
					return "Camera";
				else if( MetadataManager.GetTypeOfNetType( typeof( Light ) ).IsAssignableFrom( type ) )
					return "Light";
				else if( MetadataManager.GetTypeOfNetType( typeof( Sky ) ).IsAssignableFrom( type ) )
					return "Sky";
				else if( MetadataManager.GetTypeOfNetType( typeof( GroupOfObjects ) ).IsAssignableFrom( type ) )
					return "GroupOfObjects";
				else if( MetadataManager.GetTypeOfNetType( typeof( RenderingPipeline ) ).IsAssignableFrom( type ) )
					return "RenderingPipeline";
				else if( MetadataManager.GetTypeOfNetType( typeof( Terrain ) ).IsAssignableFrom( type ) )
					return "Terrain";
				else if( MetadataManager.GetType( "NeoAxis.Character" ).IsAssignableFrom( type ) )
					return "Character";
				else if( MetadataManager.GetType( "NeoAxis.Character2D" ).IsAssignableFrom( type ) )
					return "Character";
				else if( MetadataManager.GetType( "NeoAxis.Weapon" ).IsAssignableFrom( type ) )
					return "Weapon";
				else if( MetadataManager.GetType( "NeoAxis.Weapon2D" ).IsAssignableFrom( type ) )
					return "Weapon";
				//else if( MetadataManager.GetType( "NeoAxis.BasicItem" ).IsAssignableFrom( type ) )
				//	return"BasicItem";
				else if( MetadataManager.GetType( "NeoAxis.Vehicle" ).IsAssignableFrom( type ) )
					return "Vehicle";
				else if( MetadataManager.GetType( "NeoAxis.SpawnPoint" ).IsAssignableFrom( type ) )
					return "SpawnPoint";
				else if( MetadataManager.GetType( "NeoAxis.Building" ).IsAssignableFrom( type ) )
					return "Building";
				else if( MetadataManager.GetType( "NeoAxis.AI" ).IsAssignableFrom( type ) )
					return "AI";
				else if( MetadataManager.GetType( "NeoAxis.GameMode" ).IsAssignableFrom( type ) )
					return "GameMode";
				else if( MetadataManager.GetType( "NeoAxis.NetworkLogic" ).IsAssignableFrom( type ) )
					return "NetworkLogic";
				else if( MetadataManager.GetType( "NeoAxis.Button" ).IsAssignableFrom( type ) )
					return "Button";
				else if( MetadataManager.GetType( "NeoAxis.Regulator" ).IsAssignableFrom( type ) )
					return "Regulator";
				else if( MetadataManager.GetType( "NeoAxis.MeasuringTool" ).IsAssignableFrom( type ) )
					return "MeasuringTool";
				else if( MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) ).IsAssignableFrom( type ) )
					return "MeshInSpace";
			}
			catch { }

			return "Class";
		}
	}

}

#endif