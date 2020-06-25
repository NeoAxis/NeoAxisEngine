// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Ribbon;

namespace NeoAxis.Editor
{
	public partial class KryptonRibbonGroupListBox : KryptonRibbonGroupCustomControl
	{
		KryptonRibbonGroupListBoxControl control;

		//!!!!temp
		Dictionary<Image, string> images = new Dictionary<Image, string>();
		//long nameCounter;

		//

		public KryptonRibbonGroupListBox()
		{
			InitializeComponent();

			control = new KryptonRibbonGroupListBoxControl();
			CustomControl = control;
		}

		[Browsable( false )]
		public KryptonRibbonGroupListBoxControl Control
		{
			get { return control; }
		}

		public void SetItems( IList<(string, Image)> dataItems )
		{
			var browser = Control.contentBrowser1;

			//save old selected index
			int oldSelectedIndex;
			if( browser.SelectedItems.Length != 0 )
				oldSelectedIndex = (int)browser.SelectedItems[ 0 ].Tag;
			else
				oldSelectedIndex = -1;

			var items = new List<ContentBrowser.Item>();

			var needUpdateImageList = false;

			for( int n = 0; n < dataItems.Count; n++ )
			{
				var dataItem = dataItems[ n ];

				var item = new ContentBrowserItem_Virtual( browser, null, dataItem.Item1 );
				item.Tag = n;

				var image = dataItem.Item2;
				if( image != null )
				{
					if( !images.TryGetValue( image, out var imageKey ) )
					{
						//!!!!

						imageKey = "_Image_" + images.Count.ToString();

						browser.ImageHelper.AddImage( imageKey, image, image );

						images.Add( image, imageKey );

						needUpdateImageList = true;
					}

					item.imageKey = imageKey;
				}

				items.Add( item );
			}

			if( needUpdateImageList )
				browser.UpdateListImageList();

			//update browser's items
			browser.SetData( items, false );

			//select
			if( oldSelectedIndex >= 0 && oldSelectedIndex < browser.Items.Count )
				browser.SelectItems( new ContentBrowser.Item[] { items[ oldSelectedIndex ] } );
			else if( browser.Items.Count != 0 )
				browser.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
		}
	}
}
