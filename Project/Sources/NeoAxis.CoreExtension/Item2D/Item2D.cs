// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A basic component for items of the game framework.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Item 2D\Item 2D", 23010 )]
	[NewObjectDefaultName( "Item 2D" )]
	public class Item2D : Sprite, ItemInterface, InteractiveObjectInterface
	{
		Item2DType typeCached = new Item2DType();

		//optimization
		SpriteAnimationController animationControllerCached;

		/////////////////////////////////////////

		//public const string TypeDefault = @"Content\Items 2D\Default\Default.item2dtype";

		protected virtual void OnTypeChanged() { }

		/// <summary>
		/// The type of the item.
		/// </summary>
		[DefaultValue( null )]//[DefaultValueReference( TypeDefault )]
		public Reference<Item2DType> ItemType
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
							typeCached = new Item2DType();

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
		public event Action<Item2D> ItemTypeChanged;
		ReferenceField<Item2DType> _itemType;// = new Reference<Item2DType>( null, TypeDefault );

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
		public event Action<Item2D> ItemCountChanged;
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
			animationControllerCached = null;
			ItemType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				UpdateMesh();

				if( EngineApp.IsSimulation )
					TickAnimate( 0.001f );
			}
		}

		void UpdateMesh()
		{
			//reset animation
			var controller = GetAnimationController();
			if( controller != null )
			{
				controller.PlayAnimation = null;
				controller.Speed = 1;
			}

			TickAnimate( 0 );
		}

		[Browsable( false )]
		public Item2DType TypeCached
		{
			get { return typeCached; }
		}

		public virtual void GetInventoryImage( out ImageComponent image, out object anyData )
		{
			image = TypeCached.InventoryImage;
			anyData = null;
		}

		public delegate void InteractionGetInfoEventDelegate( Item2D sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			//enable an interaction context to take the object by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
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
					var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
					if( character != null && character.ItemCanTake( gameMode, this ) )
					{
						if( NetworkIsClient )
							character.ItemTakeAndActivateClient( this );
						else
						{
							if( character.ItemTake( gameMode, this ) )
								character.ItemActivate( gameMode, this );
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

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
		}

		public SpriteAnimationController GetAnimationController()
		{
			if( animationControllerCached == null )
			{
				animationControllerCached = GetComponent<SpriteAnimationController>();
				if( animationControllerCached == null && !NetworkIsClient )
				{
					animationControllerCached = CreateComponent<SpriteAnimationController>();
					animationControllerCached.Name = "Animation Controller";
				}
			}
			return animationControllerCached;
		}

		void TickAnimate( float delta )
		{
			var controller = GetAnimationController();
			if( controller != null )
			{
				////update event animation
				//if( eventAnimation != null )
				//	eventAnimationUpdateMethod?.Invoke();
				//if( eventAnimation != null )
				//{
				//	var current = controller.PlayAnimation.Value;
				//	if( current == eventAnimation && controller.CurrentAnimationTime == eventAnimation.Length )
				//		EventAnimationEnd();
				//}

				//update controller

				Animation animation = null;
				bool autoRewind = true;
				double speed = 1;

				////event animation
				//if( eventAnimation != null )
				//{
				//	animation = eventAnimation;
				//	autoRewind = false;
				//}

				//current state animation
				if( animation == null && EngineApp.IsSimulation )
				{
					//animation = FlyAnimation;
					//autoRewind = true;
				}

				if( animation == null )
				{
					animation = TypeCached.IdleAnimation;
					autoRewind = true;
				}

				//update controller
				controller.PlayAnimation = animation;
				controller.AutoRewind = autoRewind;
				controller.Speed = speed;
			}
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
		}
	}
}
