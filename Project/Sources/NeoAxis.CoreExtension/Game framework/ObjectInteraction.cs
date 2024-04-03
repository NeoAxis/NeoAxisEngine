// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A result data of <see cref="InteractiveObjectInterface.ObjectInteractionGetInfo(UIControl, GameMode, ref InteractiveObjectObjectInfo)"/>.
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A context of the player's interaction with the objects.
	/// </summary>
	public class ObjectInteractionContext
	{
		public InteractiveObjectInterface Obj;
		public object AnyData;
		public GameMode GameMode;
		public Viewport Viewport;

		public ObjectInteractionContext( InteractiveObjectInterface obj, GameMode gameMode, Viewport viewport )
		{
			Obj = obj;
			GameMode = gameMode;
			Viewport = viewport;
		}

		//public virtual void Dispose() { }
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface of interactive object in the scene.
	/// </summary>
	public interface InteractiveObjectInterface
	{
		void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info );

		bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message );

		void ObjectInteractionEnter( ObjectInteractionContext context );
		void ObjectInteractionExit( ObjectInteractionContext context );
		void ObjectInteractionUpdate( ObjectInteractionContext context );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A component to add interactive object functionality to a parent component.
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

		public delegate void ObjectInteractionGetInfoEventDelegate( InteractiveObject sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public delegate void ObjectInteractionInputMessageEventDelegate( InteractiveObject sender, GameMode gameMode, InputMessage message, ref bool handled );
		public event ObjectInteractionInputMessageEventDelegate ObjectInteractionInputMessageEvent;

		public delegate void ObjectInteractionEnterEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event ObjectInteractionEnterEventDelegate ObjectInteractionEnterEvent;

		public delegate void ObjectInteractionExitEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event ObjectInteractionExitEventDelegate ObjectInteractionExitEvent;

		public delegate void ObjectInteractionUpdateEventDelegate( InteractiveObject sender, ObjectInteractionContext context );
		public event ObjectInteractionUpdateEventDelegate ObjectInteractionUpdateEvent;

		///////////////////////////////////////////////

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			info = new InteractiveObjectObjectInfo();
			info.AllowInteract = AllowInteract;
			info.Text = Text;
			ObjectInteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var handled = false;
			ObjectInteractionInputMessageEvent?.Invoke( this, gameMode, message, ref handled );
			return handled;
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
			ObjectInteractionEnterEvent?.Invoke( this, context );
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
			ObjectInteractionExitEvent?.Invoke( this, context );
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
			ObjectInteractionUpdateEvent?.Invoke( this, context );
		}
	}
}