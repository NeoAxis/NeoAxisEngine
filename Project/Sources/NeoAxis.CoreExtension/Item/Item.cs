// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A basic component for items of the game framework.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Item\Item", 530 )]
	[NewObjectDefaultName( "Item" )]
	public class Item : MeshInSpace, ItemInterface, InteractiveObjectInterface
	{
		ItemType typeCached = new ItemType();

		//public const string TypeDefault = @"Content\Items\NeoAxis\Key\Key.itemtype";

		protected virtual void OnTypeChanged() { }

		/// <summary>
		/// The type of the item.
		/// </summary>
		[DefaultValue( null )]//[DefaultValueReference( TypeDefault )]
		public Reference<ItemType> ItemType
		{
			get { if( _itemType.BeginGet() ) ItemType = _itemType.Get( this ); return _itemType.value; }
			set
			{
				if( _itemType.BeginSet( this, ref value ) )
				{
					try
					{
						ItemTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _itemType.value;
						if( typeCached == null )
							typeCached = new ItemType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();

						OnTypeChanged();
					}
					finally { _itemType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ItemType"/> property value changes.</summary>
		public event Action<Item> ItemTypeChanged;
		ReferenceField<ItemType> _itemType;// = new Reference<ItemType>( null, TypeDefault );

		/// <summary>
		/// The amount of items in the component.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> ItemCount
		{
			get { if( _itemCount.BeginGet() ) ItemCount = _itemCount.Get( this ); return _itemCount.value; }
			set { if( _itemCount.BeginSet( this, ref value ) ) { try { ItemCountChanged?.Invoke( this ); } finally { _itemCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ItemCount"/> property value changes.</summary>
		public event Action<Item> ItemCountChanged;
		ReferenceField<double> _itemCount = 1.0;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
					skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			ItemType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				UpdateMesh();

				//if( EngineApp.IsSimulation )
				//	TickAnimate( 0.001f );
			}
		}

		void UpdateMesh()
		{
			Mesh = TypeCached.Mesh;

			//enable collision when collision definiation is exists and when not taken by character or other object
			var meshValue = Mesh.Value;
			Collision = meshValue != null && meshValue.GetComponent<RigidBody>( "Collision Definition" ) != null && Parent as ObjectInSpace == null;
		}

		[Browsable( false )]
		public ItemType TypeCached
		{
			get { return typeCached; }
		}

		public virtual void GetInventoryImage( out ImageComponent image, out object anyData )
		{
			image = TypeCached.InventoryImage;
			anyData = null;
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Item sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			//enable an interaction context to take the object by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null && character.ItemCanTake( gameMode, this ) )
			{
				info = new InteractiveObjectObjectInfo();
				info.AllowInteract = true;
				//info.Text.Add( "Take the item" );
				//info.Text.Add( Name );
				//info.Text.Add( $"Click to take. Press {gameMode.KeyDrop1.Value} to drop." );
			}
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//process an interaction context to take the object by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null && character.ItemCanTake( gameMode, this ) )
					{
						if( NetworkIsClient )
						{
							var activate = character.GetActiveItem() == null;
							character.ItemTakeAndActivateClient( this, activate );
						}
						else
						{
							if( character.ItemTake( gameMode, this ) )
							{
								if( character.GetActiveItem() == null )
									character.ItemActivate( gameMode, this );
							}
						}
						return true;
					}
				}
			}

			return false;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
		}
	}
}
