// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	class ContentBrowserImageHelper
	{
		static Image defaultSmallImage;
		static Image defaultLargeImage;

		ImageCollection imageListIconsSmall = new ImageCollection();
		ImageCollection imageListIconsLarge = new ImageCollection();

		Dictionary<string, Image> scaledImagesCacheForTree = new Dictionary<string, Image>();
		Dictionary<string, Image> scaledImagesCacheForTreeDisabled = new Dictionary<string, Image>();

		Dictionary<(string, int), Image> imageListDisabled = new Dictionary<(string, int), Image>();

		///////////////////////////////////////////////

		static ContentBrowserImageHelper()
		{
			defaultSmallImage = Properties.Resources.Default_16;
			defaultLargeImage = Properties.Resources.Default_32;
		}

		public void Dispose()
		{
			//!!!!?
		}

		public void AddImage( string key, Image smallImage, Image largeImage )
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

				var scaledImage = EditorAPI.GetImageForDispalyScale( imageListIconsSmall[ key ], imageListIconsLarge[ key ] );//, useCache );
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

	static class ContentBrowserImageHelperBasicImages
	{
		static ContentBrowserImageHelper helper = new ContentBrowserImageHelper();

		public static ContentBrowserImageHelper Helper
		{
			get { return helper; }
		}

		static ContentBrowserImageHelperBasicImages()
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

			Helper.AddImage( "Cog", Properties.Resources.Cog_16, Properties.Resources.Cog_32 );

			Helper.AddImage( "CSharpProject", Properties.Resources.CSharpProject_16, Properties.Resources.CSharpProject_32 );
			Helper.AddImage( "Attach", Properties.Resources.Attach_16, Properties.Resources.Attach_32 );
			Helper.AddImage( "New", Properties.Resources.New_16, Properties.Resources.New_32 );

			//Helper.AddImage( "Character", Properties.Resources.MeshSkeleton_16, Properties.Resources.MeshSkeleton_32 );
		}

	}
}
