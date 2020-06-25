// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	internal interface IHCItemСontentBrowser
	{
		ContentBrowser Browser { get; }
	}

	// this is HCItemProperty for Read Only СontentBrowser. Only Selection supported.

	internal class HCItemСontentBrowser : HCItemProperty
	{
		public HCItemСontentBrowser( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override UserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridСontentBrowser();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCItemСontentBrowser)CreatedControlInsidePropertyItemControl;

			try
			{
				var items = new List<ContentBrowser.Item>( GetItems( control.Browser ) );
				if( items != null && items.Count != 0 )
					control.Browser.SetData( items, false );
			}
			catch( Exception exc )
			{
				control.Browser.SetError( "Error: " + exc.Message );
			}

			control.Browser.ItemAfterSelect += Browser_ItemAfterSelect;
		}

		protected virtual List<ContentBrowser.Item> GetItems( ContentBrowser browser )
		{
			return new List<ContentBrowser.Item>();
		}
		//protected virtual IEnumerable<ContentBrowser.Item> GetItems( ContentBrowser browser )
		//{
		//	return Enumerable.Empty<ContentBrowser.Item>();
		//}

		private void Browser_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( selectedByUser && items.Count != 0 )
			{
				var item = items[ 0 ];
				if( CanEditValue() )
					SetValue( item.Tag, true );
			}
		}

		object currentValue;

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCItemСontentBrowser)CreatedControlInsidePropertyItemControl;
			control.Browser.ReadOnlyHierarchy = !CanEditValue();

			var values = GetValues();
			if( values == null )
				return;

			//!!!!multiselection
			var value = values[ 0 ];

			object unrefValue = ReferenceUtility.GetUnreferencedValue( value );
			if( !object.Equals( currentValue, unrefValue ) )
			{
				OnValueChanged( currentValue, unrefValue );
				currentValue = unrefValue;
			}
		}

		public virtual void OnValueChanged( object oldValue, object newValue )
		{
			var control = (IHCItemСontentBrowser)CreatedControlInsidePropertyItemControl;

			var item = control.Browser.GetAllItems().FirstOrDefault( i => Equals( i.Tag, newValue ) );
			if( item == null )
				throw new Exception( $"{newValue} not found in browser." );

			control.Browser.SelectItems( new ContentBrowser.Item[] { item } );
		}
	}

	[AttributeUsage( AttributeTargets.Property )]
	internal class EditorDataSourceAttribute : Attribute
	{
		/// <summary>
		/// Property or method name
		/// </summary>
		public string Source { get; }

		public EditorDataSourceAttribute( string source )
		{
			Source = source;
		}
	}

	internal class HCItemСontentBrowserForCollection<T> : HCItemСontentBrowser
	{
		public HCItemСontentBrowserForCollection( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		protected override List<ContentBrowser.Item> GetItems( ContentBrowser browser )
		//protected override IEnumerable<ContentBrowser.Item> GetItems( ContentBrowser browser )
		{
			var attr = Property.GetCustomAttribute<EditorDataSourceAttribute>();
			if( attr == null )
				throw new Exception( nameof( HCItemСontentBrowserForCollection<T> ) +
					" editor must be used with " + nameof( EditorDataSourceAttribute ) );

			var ownerType = Property.Owner as Metadata.NetTypeInfo;
			var obj = GetOneControlledObject<object>();
			var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
				| BindingFlags.GetProperty | BindingFlags.InvokeMethod;
			var collection = (IEnumerable<T>)ownerType.Type.InvokeMember( attr.Source, flags, null, obj, null );
			return GetItemsForCollection( browser, collection );
		}

		protected virtual List<ContentBrowser.Item> GetItemsForCollection( ContentBrowser browser, IEnumerable<T> collection )
		{
			var result = new List<ContentBrowser.Item>();
			foreach( var val in collection )
				result.Add( new ContentBrowserItem_Virtual( browser, null, val.ToString() ) { Tag = val } );
			return result;
		}
		//protected virtual IEnumerable<ContentBrowser.Item> GetItemsForCollection( ContentBrowser browser, IEnumerable<T> collection )
		//{
		//	foreach( var val in collection )
		//		yield return new ContentBrowserItem_Virtual( browser, null, val.ToString() ) { Tag = val };
		//}

		public override UserControl CreateControlInsidePropertyItemControl()
		{
			var cb = new HCGridСontentBrowser();
			cb.Browser.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			cb.Browser.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
			cb.Browser.UseSelectedTreeNodeAsRootForList = false;
			cb.Browser.Options.Breadcrumb = false;
			cb.Browser.ShowToolBar = false;
			return cb;
		}
	}
}