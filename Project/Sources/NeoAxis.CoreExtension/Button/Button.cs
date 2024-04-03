// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Button in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Button\Button", 430 )]
	[NewObjectDefaultName( "Button" )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.ButtonSettingsCell ) )]
#endif
	public class Button : MeshInSpace, InteractiveObjectInterface
	{
		ButtonType typeCached = new ButtonType();

		bool clicking;
		double clickingCurrentTime;

		bool needUpdateAdditionalItems;

		/////////////////////////////////////////

		const string typeDefault = @"Content\Buttons\Default\Default.buttontype";

		/// <summary>
		/// The type of the item.
		/// </summary>
		[DefaultValueReference( typeDefault )]
		public Reference<ButtonType> ButtonType
		{
			get { if( _buttonType.BeginGet() ) ButtonType = _buttonType.Get( this ); return _buttonType.value; }
			set
			{
				if( _buttonType.BeginSet( this, ref value ) )
				{
					try
					{
						ButtonTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _buttonType.value;
						if( typeCached == null )
							typeCached = new ButtonType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();

						needUpdateAdditionalItems = true;
					}
					finally { _buttonType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ButtonType"/> property value changes.</summary>
		public event Action<Button> ButtonTypeChanged;
		ReferenceField<ButtonType> _buttonType = new Reference<ButtonType>( null, typeDefault );

		/// <summary>
		/// Specifies activated state of the button.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Activated
		{
			get { if( _activated.BeginGet() ) Activated = _activated.Get( this ); return _activated.value; }
			set { if( _activated.BeginSet( this, ref value ) ) { try { ActivatedChanged?.Invoke( this ); needUpdateAdditionalItems = true; } finally { _activated.EndSet(); } } }
		}
		public event Action<Button> ActivatedChanged;
		ReferenceField<bool> _activated = false;

		/// <summary>
		/// Whether to change activation state on click.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SwitchActivateOnClick
		{
			get { if( _switchActivateOnClick.BeginGet() ) SwitchActivateOnClick = _switchActivateOnClick.Get( this ); return _switchActivateOnClick.value; }
			set { if( _switchActivateOnClick.BeginSet( this, ref value ) ) { try { SwitchActivateOnClickChanged?.Invoke( this ); } finally { _switchActivateOnClick.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SwitchActivateOnClick"/> property value changes.</summary>
		public event Action<Button> SwitchActivateOnClickChanged;
		ReferenceField<bool> _switchActivateOnClick = true;

		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( this, ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<Button> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

		/////////////////////////////////////////

		[Browsable( false )]
		public bool Clicking
		{
			get { return clicking; }
		}

		[Browsable( false )]
		public double ClickingCurrentTime
		{
			get { return clickingCurrentTime; }
		}

		/////////////////////////////////////////

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
			ButtonType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				UpdateMesh();
				UpdateAdditionalItems();
			}
		}

		void UpdateMesh()
		{
			Mesh = TypeCached.BaseMesh;
		}

		void UpdateAdditionalItems()
		{
			var additionalItems = new List<AdditionalItem>();

			//button
			{
				var mesh = TypeCached.ButtonMesh.Value;
				if( mesh != null )
				{
					var coef = 0.0;
					if( Clicking && TypeCached.ClickingTotalTime != 0 )
					{
						var timeFactor = MathEx.Saturate( ClickingCurrentTime / TypeCached.ClickingTotalTime );
						if( timeFactor < 0.5 )
							coef = timeFactor * 2;
						else
							coef = ( 1.0f - timeFactor ) * 2;
					}

					var position = Vector3.Lerp( TypeCached.ButtonMeshPosition, TypeCached.ButtonMeshPositionPushed, coef );
					additionalItems.Add( new AdditionalItem( mesh, position, Quaternion.Identity, Vector3.One, ColorValue.One ) );
				}
			}

			//indicator
			{
				var mesh = TypeCached.IndicatorMesh.Value;
				if( mesh != null )
				{
					var position = TypeCached.IndicatorMeshPosition;
					var replaceMaterial = Activated ? TypeCached.IndicatorMeshMaterialActivated.Value : null;

					var item = new AdditionalItem( mesh, position, Quaternion.Identity, Vector3.One, ColorValue.One );
					item.ReplaceMaterial = replaceMaterial;

					additionalItems.Add( item );
				}
			}

			AdditionalItems = additionalItems.ToArray();
			needUpdateAdditionalItems = false;
		}

		[Browsable( false )]
		public ButtonType TypeCached
		{
			get { return typeCached; }
		}

		protected virtual void OnCanClick( ref bool canClick ) { }

		public delegate void CanClickDelegate( Button sender, ref bool canClick );
		public event CanClickDelegate CanClick;

		public bool PerformCanClick()
		{
			var canClick = true;
			OnCanClick( ref canClick );
			CanClick?.Invoke( this, ref canClick );
			return canClick;
		}

		//

		protected virtual void OnClick() { }

		public delegate void ClickDelegate( Button sender );
		public event ClickDelegate Click;

		public bool PerformClick()
		{
			if( !PerformCanClick() )
				return false;

			if( SwitchActivateOnClick )
				Activated = !Activated;

			SoundPlay( TypeCached.SoundClick );
			if( NetworkIsServer && TypeCached.SoundClick.ReferenceOrValueSpecified )
			{
				BeginNetworkMessageToEveryone( "SoundClick" );
				EndNetworkMessage();
			}

			OnClick();
			Click?.Invoke( this );

			return true;
		}

		//

		public event Action<Button> ClickingBeginEvent;
		public event Action<Button> ClickingEndEvent;

		public bool ClickingBegin()
		{
			if( Clicking )
				return false;
			if( !PerformCanClick() )
				return false;

			clicking = true;
			clickingCurrentTime = 0;
			NetworkSendClicking( null );

			SoundPlay( TypeCached.SoundClickingBegin );
			if( NetworkIsServer && TypeCached.SoundClickingBegin.ReferenceOrValueSpecified )
			{
				BeginNetworkMessageToEveryone( "SoundClickingBegin" );
				EndNetworkMessage();
			}

			ClickingBeginEvent?.Invoke( this );
			needUpdateAdditionalItems = true;

			return true;
		}

		public void ClickingEnd()
		{
			if( !Clicking )
				return;

			clicking = false;
			clickingCurrentTime = 0;
			NetworkSendClicking( null );

			SoundPlay( TypeCached.SoundClickingEnd );
			if( NetworkIsServer && TypeCached.SoundClickingEnd.ReferenceOrValueSpecified )
			{
				BeginNetworkMessageToEveryone( "SoundClickingEnd" );
				EndNetworkMessage();
			}

			ClickingEndEvent?.Invoke( this );
			needUpdateAdditionalItems = true;
		}

		//

		void Simulate( float delta )
		{
			if( Clicking )
			{
				var before = clickingCurrentTime < TypeCached.ClickingClickTime || ( clickingCurrentTime == 0 && TypeCached.ClickingClickTime == 0 );
				clickingCurrentTime += delta;
				var after = clickingCurrentTime < TypeCached.ClickingClickTime;

				if( before != after )
					PerformClick();

				if( clickingCurrentTime >= TypeCached.ClickingTotalTime )
					ClickingEnd();

				NetworkSendClicking( null );

				needUpdateAdditionalItems = true;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsEditor )
				Simulate( delta );

			if( needUpdateAdditionalItems )
				UpdateAdditionalItems();
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					if( NetworkIsClient )
					{
						BeginNetworkMessageToServer( "ClickingBegin" );
						EndNetworkMessage();
					}
					else
						ClickingBegin();
					return true;
				}
			}

			return false;
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Button sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			info = new InteractiveObjectObjectInfo();
			info.AllowInteract = AllowInteract;
			//info.Text.Add( Name );
			ObjectInteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		void NetworkSendClicking( ServerNetworkService_Components.ClientItem client )
		{
			if( NetworkIsServer )
			{
				var writer = client != null ? BeginNetworkMessage( client, "Clicking" ) : BeginNetworkMessageToEveryone( "Clicking" );
				writer.Write( clicking );
				writer.Write( (float)clickingCurrentTime );
				EndNetworkMessage();
			}
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			NetworkSendClicking( client );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "SoundClick" )
				SoundPlay( TypeCached.SoundClick );
			else if( message == "SoundClickingBegin" )
				SoundPlay( TypeCached.SoundClickingBegin );
			else if( message == "SoundClickingEnd" )
				SoundPlay( TypeCached.SoundClickingEnd );
			else if( message == "Clicking" )
			{
				clicking = reader.ReadBoolean();
				clickingCurrentTime = reader.ReadSingle();
				needUpdateAdditionalItems = true;
			}

			return true;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;


			//!!!!security check is needed. who interact


			if( message == "ClickingBegin" )
				ClickingBegin();

			return true;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//base.OnSpaceBoundsUpdate( ref newBounds );

			Mesh m = MeshOutput;
			var result = m?.Result;
			if( result != null )
			{
				var meshSpaceBounds = result.SpaceBounds.BoundingBox;

				var v = TypeCached.ExpandSpaceBounds.Value;
				meshSpaceBounds.Maximum.X += v.X;
				meshSpaceBounds.Minimum.Y -= v.Y;
				meshSpaceBounds.Minimum.Z -= v.Z;
				meshSpaceBounds.Maximum.Y += v.Y;
				meshSpaceBounds.Maximum.Z += v.Z;

				newBounds = SpaceBounds.Multiply( Transform, new SpaceBounds( meshSpaceBounds ) );

				//var b = SpaceBounds.Multiply( Transform, new SpaceBounds( meshSpaceBounds ) );
				//newBounds = SpaceBounds.Merge( newBounds, b );
			}
		}
	}
}