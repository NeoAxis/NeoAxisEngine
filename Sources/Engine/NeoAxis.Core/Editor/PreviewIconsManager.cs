// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NeoAxis.Editor
{
	public static class PreviewIconsManager
	{
		//!!!!
		static Image newImage;
		//static Image deleteImage;

		public static Image GetIcon( Component component )
		{
			//!!!!

			if( newImage == null )
				newImage = Properties.Resources.Default_32;
			return newImage;

			//if( Time.Current % 2.0 < 1 )
			//{
			//	if( newImage == null )
			//		newImage = Properties.Resources.Add_32;
			//	return newImage;
			//}
			//else
			//{
			//	if( deleteImage == null )
			//		deleteImage = Properties.Resources.Delete_32;
			//	return deleteImage;
			//}

			//if( Time.Current % 2.0 < 1 )
			//{
			//	return Properties.Resources.New_32;
			//}
			//else
			//{
			//	return Properties.Resources.Delete_32;
			//}

		}

		public static void Update()
		{
			//!!!!
		}

		////!!!!temp
		//static ESet<Component> requested = new ESet<Component>();


		//public delegate void IconChangedDelegate( Component component, Image icon );
		//public static event IconChangedDelegate IconUpdated;

		////

		//public static void RequestIcon( Component component )
		//{
		//	requested.AddWithCheckAlreadyContained( component );

		//	//Image a;
		//}

		//public static void Update()
		//{
		//	if( Time.Current % 2.0 < 1 )
		//	{
		//		foreach( var c in requested )
		//			IconUpdated?.Invoke( c, Properties.Resources.New_32 );
		//	}
		//	else
		//	{
		//		foreach( var c in requested )
		//			IconUpdated?.Invoke( c, Properties.Resources.Delete_32 );
		//	}

		//}
	}
}
