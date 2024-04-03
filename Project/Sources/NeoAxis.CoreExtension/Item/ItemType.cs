// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the item.
	/// </summary>
	[ResourceFileExtension( "itemtype" )]
	[NewObjectDefaultName( "Item" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Item\Item Type", 520 )]
	[EditorControl( typeof( ItemTypeEditor ) )]
	[Preview( typeof( ItemTypePreview ) )]
	[PreviewImage( typeof( ItemTypePreviewImage ) )]
#endif
	public class ItemType : Component, ItemTypeInterface
	{
		int version;

		/// <summary>
		/// The mesh of the item.
		/// </summary>
		//[DefaultValueReference( meshDefault )]
		[DefaultValue( null )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<ItemType> MeshChanged;
		ReferenceField<Mesh> _mesh = null;//new Reference<Mesh>( null, meshDefault );

		/// <summary>
		/// The image of the item type to preview in the inventory.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ImageComponent> InventoryImage
		{
			get { if( _inventoryImage.BeginGet() ) InventoryImage = _inventoryImage.Get( this ); return _inventoryImage.value; }
			set { if( _inventoryImage.BeginSet( this, ref value ) ) { try { InventoryImageChanged?.Invoke( this ); } finally { _inventoryImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InventoryImage"/> property value changes.</summary>
		public event Action<ItemType> InventoryImageChanged;
		ReferenceField<ImageComponent> _inventoryImage = null;

		/// <summary>
		/// Whether to merge items with same type into one component.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CanCombineIntoOneItem
		{
			get { if( _canCombineIntoOneItem.BeginGet() ) CanCombineIntoOneItem = _canCombineIntoOneItem.Get( this ); return _canCombineIntoOneItem.value; }
			set { if( _canCombineIntoOneItem.BeginSet( this, ref value ) ) { try { CanCombineIntoOneItemChanged?.Invoke( this ); } finally { _canCombineIntoOneItem.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanCombineIntoOneItem"/> property value changes.</summary>
		public event Action<ItemType> CanCombineIntoOneItemChanged;
		ReferenceField<bool> _canCombineIntoOneItem = true;

		/// <summary>
		/// Whether to be activated when it has taken.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> CanActivate
		{
			get { if( _canActivate.BeginGet() ) CanActivate = _canActivate.Get( this ); return _canActivate.value; }
			set { if( _canActivate.BeginSet( this, ref value ) ) { try { CanActivateChanged?.Invoke( this ); } finally { _canActivate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanActivate"/> property value changes.</summary>
		public event Action<ItemType> CanActivateChanged;
		ReferenceField<bool> _canActivate = false;


		//!!!!not used
		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}
	}
}
