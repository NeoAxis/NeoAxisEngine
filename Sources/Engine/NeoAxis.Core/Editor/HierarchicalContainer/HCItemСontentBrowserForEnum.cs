//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace NeoAxis.Editor
//{
//	/// <summary>
//	/// Content browser for enum example
//	/// </summary>
//	internal class HCItemСontentBrowserForEnum : HCItemСontentBrowser
//	{
//		public HCItemСontentBrowserForEnum( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
//			: base( owner, parent, controlledObjects, property, indexers )
//		{
//		}

//		protected override List<ContentBrowser.Item> GetItems( ContentBrowser browser )
//		{
//			var result = new List<ContentBrowser.Item>();

//			var propertyType = ReferenceUtility.GetUnreferencedType( Property.Type.GetNetType() );
//			foreach( var val in Enum.GetValues( propertyType ) )
//			{
//				result.Add( new ContentBrowserItem_Virtual( browser, null, TypeUtility.DisplayNameAddSpaces( val.ToString() ) ) { Tag = val } );
//			}

//			return result;
//		}
//		//protected override IEnumerable<ContentBrowser.Item> GetItems( ContentBrowser browser )
//		//{
//		//	var propertyType = ReferenceUtility.GetUnreferencedType( Property.Type.GetNetType() );
//		//	foreach( var val in Enum.GetValues( propertyType ) )
//		//	{
//		//		yield return new ContentBrowserItem_Virtual( browser, null,
//		//			TypeUtility.DisplayNameAddSpaces( val.ToString() ) ) { Tag = val };
//		//	}
//		//}

//		public override UserControl CreateControlInsidePropertyItemControl()
//		{
//			var cb = new HCGridСontentBrowser();
//			cb.Height = 200;
//			cb.Browser.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
//			cb.Browser.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
//			cb.Browser.Options.ListColumnWidth = 100;
//			cb.Browser.UseSelectedTreeNodeAsRootForList = false;
//			cb.Browser.Options.Breadcrumb = false;
//			cb.Browser.ShowToolBar = false;
//			return cb;
//		}
//	}
//}
