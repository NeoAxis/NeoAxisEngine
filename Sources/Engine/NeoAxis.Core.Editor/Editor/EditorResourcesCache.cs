#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using NeoAxis.Editor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public static class EditorResourcesCache
	{
		static Dictionary<string, Image> cachedImages = new Dictionary<string, Image>();

		//static Dictionary<(Image, Image), Image> getImageForDispalyScale = new Dictionary<(Image, Image), Image>();

		//

		public static readonly Image New = RenderStandard.GetImageForDispalyScale( Resources.New_16, Resources.New_32 );
		public static readonly Image Edit = RenderStandard.GetImageForDispalyScale( Resources.Edit_16, Resources.Edit_32 );
		public static readonly Image Settings = RenderStandard.GetImageForDispalyScale( Resources.Maximize_16, Resources.Maximize_32 );
		public static readonly Image Cut = RenderStandard.GetImageForDispalyScale( Resources.Cut_16, Resources.Cut_32 );
		public static readonly Image Copy = RenderStandard.GetImageForDispalyScale( Resources.Copy_16, Resources.Copy_32 );
		public static readonly Image Paste = RenderStandard.GetImageForDispalyScale( Resources.Paste_16, Resources.Paste_32 );
		public static readonly Image Clone = RenderStandard.GetImageForDispalyScale( Resources.Copy_16, Resources.Copy_32 );
		public static readonly Image Delete = RenderStandard.GetImageForDispalyScale( Resources.Delete_16, Resources.Delete_32 );
		public static readonly Image Add = RenderStandard.GetImageForDispalyScale( Resources.Add_16, Resources.Add_32 );
		public static readonly Image Undo = RenderStandard.GetImageForDispalyScale( Resources.Undo_16, Resources.Undo_32 );
		public static readonly Image Redo = RenderStandard.GetImageForDispalyScale( Resources.Redo_16, Resources.Redo_32 );

		//!!!!Move_32_Dark, Scale_32_Dark
		public static readonly Image Move = RenderStandard.GetImageForDispalyScale( Resources.Move_16, Resources.Move_32 );
		public static readonly Image Rotate = RenderStandard.GetImageForDispalyScale( Resources.Rotate_16, Resources.Rotate_32 );
		public static readonly Image MoveRotate = RenderStandard.GetImageForDispalyScale( Resources.MoveRotate_16, Resources.MoveRotate_32 );
		public static readonly Image Scale = RenderStandard.GetImageForDispalyScale( Resources.Scale_16, Resources.Scale_32 );
		public static readonly Image Select = RenderStandard.GetImageForDispalyScale( Resources.Select_16, Resources.Select_32 );

		public static readonly Image MoveDown = RenderStandard.GetImageForDispalyScale( Resources.MoveDown_16, Resources.MoveDown_32 );
		public static readonly Image MoveUp = RenderStandard.GetImageForDispalyScale( Resources.MoveUp_16, Resources.MoveUp_32 );
		public static readonly Image NewFolder = RenderStandard.GetImageForDispalyScale( Resources.NewFolder_16, Resources.NewFolder_32 );
		public static readonly Image SelectFolder = RenderStandard.GetImageForDispalyScale( Resources.SelectFolder_16, Resources.SelectFolder_32 );
		public static readonly Image Options = RenderStandard.GetImageForDispalyScale( Resources.Options_16, Resources.Options_32 );
		public static readonly Image Rename = RenderStandard.GetImageForDispalyScale( Resources.Rename_16x, Resources.Rename_32x );
		public static readonly Image Selection = RenderStandard.GetImageForDispalyScale( Resources.Selection_16, Resources.Selection_32 );
		public static readonly Image Import = RenderStandard.GetImageForDispalyScale( Resources.Import_16, Resources.Import_32 );
		public static readonly Image Events = RenderStandard.GetImageForDispalyScale( Resources.Event_16, Resources.Event_32 );
		public static readonly Image Properties = RenderStandard.GetImageForDispalyScale( Resources.Properties_16, Resources.Properties_32 );
		public static readonly Image Refresh = RenderStandard.GetImageForDispalyScale( Resources.Refresh_16, Resources.Refresh_32 );
		public static readonly Image Download = RenderStandard.GetImageForDispalyScale( Resources.Download_16, Resources.Download_32 );

		public static readonly Image Info = RenderStandard.GetImageForDispalyScale( Resources.Info_16, Resources.Info_32 );
		public static readonly Image Warning = RenderStandard.GetImageForDispalyScale( Resources.Warning_16, Resources.Warning_32 );
		public static readonly Image Error = RenderStandard.GetImageForDispalyScale( Resources.Error_16, Resources.Error_32 );

		public static readonly Image Type = RenderStandard.GetImageForDispalyScale( Resources.Class, Resources.Class_32 );
		public static readonly Image Focus = RenderStandard.GetImageForDispalyScale( Resources.Focus_16, Resources.Focus_32 );
		public static readonly Image Help = RenderStandard.GetImageForDispalyScale( Resources.Help_16, Resources.Help_32 );
		public static readonly Image Save = RenderStandard.GetImageForDispalyScale( Resources.Save_16, Resources.Save_32 );
		public static readonly Image Money = RenderStandard.GetImageForDispalyScale( Resources.Money_16, Resources.Money_32 );
		public static readonly Image Filter = RenderStandard.GetImageForDispalyScale( Resources.Filter_16, Resources.Filter_32 );
		public static readonly Image Resource = RenderStandard.GetImageForDispalyScale( Resources.Resource_16, Resources.Resource_32 );
		public static readonly Image Tree = RenderStandard.GetImageForDispalyScale( Resources.Tree_16, Resources.Tree_32 );

		public static readonly Image Synchronize = RenderStandard.GetImageForDispalyScale( Resources.Synchronize_16, Resources.Synchronize_32 );
		public static readonly Image ServerOnly = RenderStandard.GetImageForDispalyScale( Resources.ServerOnly_16, Resources.ServerOnly_32 );
		public static readonly Image StorageOnly = RenderStandard.GetImageForDispalyScale( Resources.StorageOnly_16, Resources.StorageOnly_32 );
		public static readonly Image Database = RenderStandard.GetImageForDispalyScale( Resources.Database_16, Resources.Database_32 );

		//

		public static Image GetImage( string name )
		{
			if( !cachedImages.TryGetValue( name, out var image ) )
			{
				image = Resources.ResourceManager.GetObject( name, Resources.Culture ) as Image;
				cachedImages.Add( name, image );
			}
			return image;
		}

		//public static Image GetImageForDispalyScale( Image image16px, Image image32px )
		//{
		//	if( !getImageForDispalyScale.TryGetValue( (image16px, image32px), out var result ) )
		//	{
		//		result = RenderStandard.GetImageForDispalyScale( image16px, image32px );
		//		getImageForDispalyScale.Add( (image16px, image32px), result );

		//		////!!!!temp
		//		//Log.InvisibleInfo( getImageForDispalyScale.Count.ToString() );
		//	}

		//	return result;
		//}


		static Bitmap defaultImage16;
		static Bitmap defaultImage32;
		static Bitmap defaultImage512;

		public static Bitmap GetDefaultImage( int size )
		{
			if( size > 32 )
			{
				var image = defaultImage512;
				if( image == null || image.PixelFormat == System.Drawing.Imaging.PixelFormat.DontCare )
					defaultImage512 = Resources.Default_512;
				return defaultImage512;
			}
			else if( size > 16 )
			{
				var image = defaultImage32;
				if( image == null || image.PixelFormat == System.Drawing.Imaging.PixelFormat.DontCare )
					defaultImage32 = Resources.Default_32;
				return defaultImage32;
			}
			else
			{
				var image = defaultImage16;
				if( image == null || image.PixelFormat == System.Drawing.Imaging.PixelFormat.DontCare )
					defaultImage16 = Resources.Default_16;
				return defaultImage16;
			}
		}

		public static readonly Image CheckedCircle_32 = Resources.CheckedCircle_32;
		public static readonly Image AddCircle_32 = Resources.AddCircle_32;
		public static readonly Image DeleteCircle_32 = Resources.DeleteCircle_32;
		public static readonly Image Warning_32 = Resources.Warning_32;
		public static readonly Image NotAdded_32 = Resources.NotAdded_32;

		public static readonly Image ServerOnly_32 = Resources.ServerOnly_32;
		public static readonly Image StorageOnly_32 = Resources.StorageOnly_32;
		public static readonly Image Synchronize_32 = Resources.Synchronize_32;

	}
}
#endif