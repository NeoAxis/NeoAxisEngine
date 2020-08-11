// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	internal class HCItemNewSceneTemplates : HCItemUserControl
	{
		ContentBrowser contentBrowser1;

		public HCItemNewSceneTemplates( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			//create parent UserControl
			var userControl = base.CreateControlInsidePropertyItemControl();

			//init browser
			contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			contentBrowser1.CanSelectObjectSettings = false;
			contentBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			contentBrowser1.Location = new System.Drawing.Point( 0, 0 );
			contentBrowser1.Margin = new System.Windows.Forms.Padding( 2 );
			contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			contentBrowser1.Name = "contentBrowser1";
			contentBrowser1.Size = new System.Drawing.Size( 215, 247 );
			contentBrowser1.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowser1.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
			contentBrowser1.UseSelectedTreeNodeAsRootForList = false;
			contentBrowser1.Options.Breadcrumb = false;
			contentBrowser1.Options.TileImageSize = 128;
			contentBrowser1.ShowToolBar = false;
			contentBrowser1.ItemAfterSelect += Browser_ItemAfterSelect;
			userControl.Controls.Add( this.contentBrowser1 );
			
			userControl.Height = ( 128 + ContentBrowserRendererTiles.TilePadding.Height + 4 ) * 2;

			//add items
			try
			{
				var items = new List<ContentBrowser.Item>();

				foreach( var template in Component_Scene.NewObjectSettingsScene.GetTemplates() )
				{
					contentBrowser1.ImageHelper.AddImage( template.Name, null, template.Preview );

					var item = new ContentBrowserItem_Virtual( contentBrowser1, null, template.ToString() );
					item.Tag = template;
					item.imageKey = template.Name;
					items.Add( item );
				}

				if( items.Count != 0 )
				{
					contentBrowser1.SetData( items, false );
					contentBrowser1.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
				}
			}
			catch( Exception exc )
			{
				contentBrowser1.SetError( "Error: " + exc.Message );
			}

			return userControl;
		}

		private void Browser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( selectedByUser && items.Count != 0 )
				SetValue( items[ 0 ].Tag, true );
		}

		//public override void OnPropertyValueChanged( object oldValue, object newValue )
		//{
		//	var item = contentBrowser1.GetAllItems().FirstOrDefault( i => Equals( i.Tag, newValue ) );
		//	if( item == null )
		//		throw new Exception( $"{newValue} not found in browser." );

		//	contentBrowser1.SelectItems( new ContentBrowser.Item[] { item } );
		//}
	}
}
