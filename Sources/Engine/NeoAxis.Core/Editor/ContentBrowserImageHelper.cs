// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	internal class ContentBrowserImageHelper
	{
		Image defaultSmallImage;
		Image defaultLargeImage;

		ImageListAdv imageListIconsSmall;
		ImageListAdv imageListIconsLarge;

		Dictionary<string, Image> scaledImagesCacheForTree = new Dictionary<string, Image>();
		Dictionary<string, Image> scaledImagesCacheForTreeDisabled = new Dictionary<string, Image>();


		public ContentBrowserImageHelper( IContainer container )
		{
			imageListIconsSmall = new ImageListAdv( container );
			imageListIconsLarge = new ImageListAdv( container );
			imageListIconsLarge.ImageSize = new Size( 32, 32 );
		}

		internal void LoadDefaultImages()
		{
			defaultSmallImage = Properties.Resources.Default_16;
			defaultLargeImage = Properties.Resources.Default_32;

			AddImage( "Default", Properties.Resources.Default_16, Properties.Resources.Default_32 );
			AddImage( "Namespace", Properties.Resources.Namespace_16, null );
			AddImage( "Class", Properties.Resources.Class, Properties.Resources.Class_32 );
			AddImage( "Struct", Properties.Resources.Struct, null );
			AddImage( "Assembly", Properties.Resources.Assembly, null );

			//!!!!tr
			//if( EditorAPI.DarkTheme )
			//	AddImage( "Resource", Properties.Resources.Resource_16_Dark, Properties.Resources.Resource_32_Dark );
			//else
			AddImage( "Resource", Properties.Resources.Resource_16, Properties.Resources.Resource_32 );

			AddImage( "AssemblyList", Properties.Resources.AssemblyList, null );
			AddImage( "Folder", Properties.Resources.Folder_16, Properties.Resources.Folder_32 );
			AddImage( "Delegate", Properties.Resources.Delegate, null );
			AddImage( "Enum", Properties.Resources.Enum, null );
			AddImage( "Property", Properties.Resources.Property, null );
			AddImage( "GoUpper", Properties.Resources.GoUpper_16, null );
			AddImage( "Method", Properties.Resources.Method, null );
			AddImage( "Event", Properties.Resources.Event_16, null );
			AddImage( "StaticClass", Properties.Resources.StaticClass, null );
			AddImage( "StaticEvent", Properties.Resources.StaticEvent, null );
			AddImage( "StaticMethod", Properties.Resources.StaticMethod, null );
			AddImage( "StaticProperty", Properties.Resources.StaticProperty, null );
			AddImage( "Constructor", Properties.Resources.Constructor, null );
			AddImage( "Operator", Properties.Resources.Operator, null );

			AddImage( "CSharp", Properties.Resources.CSharp_16, Properties.Resources.CSharp_32 );
			AddImage( "UI", Properties.Resources.Window_16, Properties.Resources.Window_32 );
			AddImage( "Image", Properties.Resources.Image_16, Properties.Resources.Image_32 );
			AddImage( "Sound", Properties.Resources.Sound_16, Properties.Resources.Sound_32 );
			AddImage( "Mesh", Properties.Resources.Mesh_16, Properties.Resources.Mesh_32 );
			AddImage( "Material", Properties.Resources.Material_16, Properties.Resources.Material_32 );
			AddImage( "Scene", Properties.Resources.Scene_16, Properties.Resources.Scene_32 );

			AddImage( "Cog", Properties.Resources.Cog_16, Properties.Resources.Cog_32 );

			AddImage( "CSharpProject", Properties.Resources.CSharpProject_16, Properties.Resources.CSharpProject_32 );
			AddImage( "Attach", Properties.Resources.Attach_16, Properties.Resources.Attach_32 );
			AddImage( "New", Properties.Resources.New_16, Properties.Resources.New_32 );

			//AddImage( "Character", Properties.Resources.MeshSkeleton_16, Properties.Resources.MeshSkeleton_32 );

			//HACK: recreate images to fix render image size at different system scales.
			int listImageSize = imageListIconsLarge.ImageSize.Height;
			imageListIconsLarge = ImageListAdv.MakeResizedImageList( imageListIconsLarge, listImageSize, listImageSize );
		}

		internal void AddImage( string key, Image smallImage, Image largeImage )//, bool useCache )
		{
			if( !imageListIconsSmall.Images.ContainsKey( key ) )
				imageListIconsSmall.Images.Add( key, smallImage ?? defaultSmallImage );

			if( !imageListIconsLarge.Images.ContainsKey( key ) )
				imageListIconsLarge.Images.Add( key, largeImage ?? defaultLargeImage );

			var scaledImage = EditorAPI.GetImageForDispalyScale( imageListIconsSmall.Images[ key ], imageListIconsLarge.Images[ key ] );//, useCache );
			scaledImagesCacheForTree[ key ] = scaledImage;
		}

		internal Image GetImageForTreeNode( string key, bool disabled )
		{
			if( string.IsNullOrEmpty( key ) )
				key = "Default";

			if( !scaledImagesCacheForTree.TryGetValue( key, out var image ) )
				return null;

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

			//return scaledImagesCacheForTree[ key ];
		}

		internal ImageListAdv ResizeImagesForListView( Size size )
		{
			if( imageListIconsLarge.ImageSize == size )
				return imageListIconsLarge;
			return ImageListAdv.MakeResizedImageList( imageListIconsLarge, size.Width, size.Height ); ;
		}

		//!!!!maybe temp
		ESet<string> imageListIconsLargeAvailable;

		//TODO: return Image. but ovl list item cached image, and we have problems with resize imagelist
		internal string GetImageForListView( string key )
		{
			if( string.IsNullOrEmpty( key ) )
				key = "Default";

			if( imageListIconsLargeAvailable == null )
			{
				imageListIconsLargeAvailable = new ESet<string>();
				foreach( var name in imageListIconsLarge.Images.Keys )
					imageListIconsLargeAvailable.AddWithCheckAlreadyContained( name );
			}

			//check availability
			if( !imageListIconsLargeAvailable.Contains( key ) )
				key = "Default";

			return key;

		}
	}
}
