// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	//<see cref="InteractiveObjectInterface.ObjectInteractionGetInfo(UIControl, GameMode, ref InteractiveObjectObjectInfo)"/>.
	/// <summary>
	/// A result data of InteractiveObjectInterface.ObjectInteractionGetInfo method.
	/// </summary>
	public class InteractiveObjectObjectInfo
	{
		public bool AllowInteract;

		public delegate void OverrideRenderDelegate( GameMode gameMode, InteractiveObjectObjectInfo info, CanvasRenderer renderer, ref bool handled );
		public OverrideRenderDelegate OverrideRender;

		public bool DefaultRender = true;

		public string Text = "";
		public ColorValue? OverrideColor;

		public object AnyData;


		//public enum AllowInteractEnum
		//{
		//	NotAllow,
		//	Deny,
		//	Allow,
		//}
		//public AllowInteractEnum AllowInteract = AllowInteractEnum.NotAllow;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A context of the player's interaction with the objects.
	/// </summary>
	public class ObjectInteractionContext
	{
		public InteractiveObjectInterface Obj;
		public Component Initiator;
		public object AnyData;
		public GameMode GameMode;
		public Viewport Viewport;

		public ObjectInteractionContext( InteractiveObjectInterface obj, Component initiator, GameMode gameMode, Viewport viewport )
		{
			Obj = obj;
			Initiator = initiator;
			GameMode = gameMode;
			Viewport = viewport;
		}

		//public virtual void Dispose() { }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface of interactive object in the scene.
	/// </summary>
	public interface InteractiveObjectInterface
	{
		void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );

		bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message );

		void InteractionEnter( ObjectInteractionContext context );
		void InteractionExit( ObjectInteractionContext context );
		void InteractionUpdate( ObjectInteractionContext context );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A component to add interactive object functionality to the parent component.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Interactive Object", -2980 )]
	public class InteractiveObject : Component, InteractiveObjectInterface
	{
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
		public event Action<InteractiveObject> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

		[DefaultValue( "" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> Text
		{
			get { if( _text.BeginGet() ) Text = _text.Get( this ); return _text.value; }
			set { if( _text.BeginSet( this, ref value ) ) { try { TextChanged?.Invoke( this ); } finally { _text.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Text"/> property value changes.</summary>
		public event Action<InteractiveObject> TextChanged;
		ReferenceField<string> _text = "";

		///////////////////////////////////////////////

		public delegate void InteractionGetInfoEventDelegate( InteractiveObject sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public delegate void InteractionInputMessageEventDelegate( InteractiveObject sender, GameMode gameMode, Component initiator, InputMessage message, ref bool handled );
		public event InteractionInputMessageEventDelegate InteractionInputMessageEvent;

		public delegate void InteractionEnterEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event InteractionEnterEventDelegate InteractionEnterEvent;

		public delegate void InteractionExitEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event InteractionExitEventDelegate InteractionExitEvent;

		public delegate void InteractionUpdateEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event InteractionUpdateEventDelegate InteractionUpdateEvent;

		///////////////////////////////////////////////

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			info = new InteractiveObjectObjectInfo();
			info.AllowInteract = AllowInteract;
			info.Text = Text;
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		///////////////////////////////////////////////

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var handled = false;
			InteractionInputMessageEvent?.Invoke( this, gameMode, initiator, message, ref handled );
			if( handled )
				return true;

			//!!!!add touch support

			//Click functionality, special for this component
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					TryClick( initiator );
					return true;
				}
			}

			return handled;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
			InteractionEnterEvent?.Invoke( this, context );
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
			InteractionExitEvent?.Invoke( this, context );
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
			InteractionUpdateEvent?.Invoke( this, context );
		}

		///////////////////////////////////////////////

		protected virtual void OnCanClick( Component initiator, ref bool canClick ) { }

		public delegate void CanClickEventDelegate( InteractiveObject sender, Component initiator, ref bool canClick );
		public event CanClickEventDelegate CanClick;

		public delegate void ClickEventDelegate( InteractiveObject sender, Component initiator );
		public event ClickEventDelegate Click;

		public void TryClick( Component initiator )
		{
			//CanClick
			var canClick = true;
			OnCanClick( initiator, ref canClick );
			CanClick?.Invoke( this, initiator, ref canClick );
			if( !canClick )
				return;

			//do clicking
			if( NetworkIsClient )
			{
				var m = BeginNetworkMessageToServer( "Click" );
				if( m != null )
				{
					m.Writer.WriteVariableUInt64( initiator != null ? (ulong)initiator.NetworkID : 0 );
					m.End();
				}
			}
			else
			{
				Click?.Invoke( this, initiator );
			}
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			if( message == "Click" )
			{
				var initiatorNetworkID = (long)reader.ReadVariableUInt64();
				if( !reader.Complete() )
					return false;
				var initiator = ParentRoot.HierarchyController.GetComponentByNetworkID( initiatorNetworkID );
				TryClick( initiator );
			}

			return true;
		}
	}
}
