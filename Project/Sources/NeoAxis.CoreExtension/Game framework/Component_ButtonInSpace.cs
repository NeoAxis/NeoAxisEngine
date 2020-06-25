// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Button in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Button", -8000 )]
	[NewObjectDefaultName( "Button" )]
	[EditorSettingsCell( typeof( Component_ButtonInSpace_SettingsCell ) )]
	public class Component_ButtonInSpace : Component_MeshInSpace, IComponent_InteractiveObject
	{
		bool clicking;
		double clickingCurrentTime;

		/////////////////////////////////////////

		/// <summary>
		/// Specifies activated state of the button.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Activated
		{
			get { if( _activated.BeginGet() ) Activated = _activated.Get( this ); return _activated.value; }
			set { if( _activated.BeginSet( ref value ) ) { try { ActivatedChanged?.Invoke( this ); } finally { _activated.EndSet(); } } }
		}
		public event Action<Component_ButtonInSpace> ActivatedChanged;
		ReferenceField<bool> _activated = false;

		/// <summary>
		/// Whether to change activation state on click.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SwitchActivateOnClick
		{
			get { if( _switchActivateOnClick.BeginGet() ) SwitchActivateOnClick = _switchActivateOnClick.Get( this ); return _switchActivateOnClick.value; }
			set { if( _switchActivateOnClick.BeginSet( ref value ) ) { try { SwitchActivateOnClickChanged?.Invoke( this ); } finally { _switchActivateOnClick.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SwitchActivateOnClick"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> SwitchActivateOnClickChanged;
		ReferenceField<bool> _switchActivateOnClick = true;

		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

		/// <summary>
		/// Total time of clicking animation.
		/// </summary>
		[DefaultValue( 0.4 )]
		public Reference<double> ClickingTotalTime
		{
			get { if( _clickingTotalTime.BeginGet() ) ClickingTotalTime = _clickingTotalTime.Get( this ); return _clickingTotalTime.value; }
			set { if( _clickingTotalTime.BeginSet( ref value ) ) { try { ClickingTotalTimeChanged?.Invoke( this ); } finally { _clickingTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClickingTotalTime"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> ClickingTotalTimeChanged;
		ReferenceField<double> _clickingTotalTime = 0.4;

		/// <summary>
		/// The time of the click during clicking animation.
		/// </summary>
		[DefaultValue( 0.2 )]
		public Reference<double> ClickingClickTime
		{
			get { if( _clickingClickTime.BeginGet() ) ClickingClickTime = _clickingClickTime.Get( this ); return _clickingClickTime.value; }
			set { if( _clickingClickTime.BeginSet( ref value ) ) { try { ClickingClickTimeChanged?.Invoke( this ); } finally { _clickingClickTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClickingClickTime"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> ClickingClickTimeChanged;
		ReferenceField<double> _clickingClickTime = 0.2;

		/// <summary>
		/// The sound that is played when the clicking begins.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Sound> SoundClickingBegin
		{
			get { if( _soundClickingBegin.BeginGet() ) SoundClickingBegin = _soundClickingBegin.Get( this ); return _soundClickingBegin.value; }
			set { if( _soundClickingBegin.BeginSet( ref value ) ) { try { SoundClickingBeginChanged?.Invoke( this ); } finally { _soundClickingBegin.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClickingBegin"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> SoundClickingBeginChanged;
		ReferenceField<Component_Sound> _soundClickingBegin = null;

		/// <summary>
		/// The sound that is played when a click occurs.
		/// </summary>
		[DefaultValueReference( @"Base\UI\Styles\Sounds\ButtonClick.ogg" )]
		public Reference<Component_Sound> SoundClick
		{
			get { if( _soundClick.BeginGet() ) SoundClick = _soundClick.Get( this ); return _soundClick.value; }
			set { if( _soundClick.BeginSet( ref value ) ) { try { SoundClickChanged?.Invoke( this ); } finally { _soundClick.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClick"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> SoundClickChanged;
		ReferenceField<Component_Sound> _soundClick = new Reference<Component_Sound>( null, @"Base\UI\Styles\Sounds\ButtonClick.ogg" );

		/// <summary>
		/// The sound that is played when the clicking ends.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Sound> SoundClickingEnd
		{
			get { if( _soundClickingEnd.BeginGet() ) SoundClickingEnd = _soundClickingEnd.Get( this ); return _soundClickingEnd.value; }
			set { if( _soundClickingEnd.BeginSet( ref value ) ) { try { SoundClickingEndChanged?.Invoke( this ); } finally { _soundClickingEnd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClickingEnd"/> property value changes.</summary>
		public event Action<Component_ButtonInSpace> SoundClickingEndChanged;
		ReferenceField<Component_Sound> _soundClickingEnd = null;

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

		//

		protected virtual void OnCanClick( ref bool canClick ) { }

		public delegate void CanClickDelegate( Component_ButtonInSpace sender, ref bool canClick );
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

		public delegate void ClickDelegate( Component_ButtonInSpace sender );
		public event ClickDelegate Click;

		public bool PerformClick()
		{
			if( !PerformCanClick() )
				return false;

			if( SwitchActivateOnClick )
				Activated = !Activated;

			SoundPlay( SoundClick );
			OnClick();
			Click?.Invoke( this );

			return true;
		}

		//

		public event Action<Component_ButtonInSpace> ClickingBeginEvent;
		public event Action<Component_ButtonInSpace> ClickingEndEvent;

		public bool ClickingBegin()
		{
			if( Clicking )
				return false;
			if( !PerformCanClick() )
				return false;

			clicking = true;
			clickingCurrentTime = 0;
			SoundPlay( SoundClickingBegin );
			ClickingBeginEvent?.Invoke( this );

			return true;
		}

		public void ClickingEnd()
		{
			if( !Clicking )
				return;

			clicking = false;
			clickingCurrentTime = 0;
			SoundPlay( SoundClickingEnd );
			ClickingEndEvent?.Invoke( this );
		}

		//

		void Simulate( float delta )
		{
			if( Clicking )
			{
				var before = clickingCurrentTime < ClickingClickTime || ( clickingCurrentTime == 0 && ClickingClickTime == 0 );
				clickingCurrentTime += delta;
				var after = clickingCurrentTime < ClickingClickTime;

				if( before != after )
					PerformClick();

				if( clickingCurrentTime >= ClickingTotalTime )
					ClickingEnd();
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				Simulate( delta );
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				Color = new ColorValue( 0.6784314, 0.6784314, 0.6784314 );

				//Mesh
				{
					var mesh = CreateComponent<Component_Mesh>();
					mesh.Name = "Mesh";
					Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<Component_MeshGeometry_Box>();
					geometry.Name = "Mesh Geometry";
					geometry.Dimensions = new Vector3( 0.05, 0.3, 0.3 );
					geometry.Material = ReferenceUtility.MakeReference( "Base\\Materials\\White.material" );
				}

				//Button
				{
					var meshInSpace = CreateComponent<Component_MeshInSpace>();
					meshInSpace.Name = "Button";

					var mesh = meshInSpace.CreateComponent<Component_Mesh>();
					mesh.Name = "Mesh";
					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<Component_MeshGeometry_Box>();
					geometry.Name = "Mesh Geometry";
					geometry.Dimensions = new Vector3( 0.05, 0.15, 0.15 );
					geometry.Material = ReferenceUtility.MakeReference( "Base\\Materials\\White.material" );

					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

					var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
					transformOffset.Name = "Attach Transform Offset";
					transformOffset.PositionOffset = new Vector3( 0.05, 0, 0 );
					transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );

					meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
				}

				//Indicator
				{
					var meshInSpace = CreateComponent<Component_MeshInSpace>();
					meshInSpace.Name = "Indicator";
					meshInSpace.Color = new ColorValue( 0, 1, 0 );

					var mesh = meshInSpace.CreateComponent<Component_Mesh>();
					mesh.Name = "Mesh";
					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<Component_MeshGeometry_Cylinder>();
					geometry.Name = "Mesh Geometry";
					geometry.Axis = 0;
					geometry.Radius = 0.02;
					geometry.Height = 0.02;
					geometry.Segments = 16;
					geometry.Material = ReferenceUtility.MakeReference( "Base\\Materials\\White.material" );

					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

					var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
					transformOffset.Name = "Attach Transform Offset";
					transformOffset.PositionOffset = new Vector3( 0.03, 0, 0.12 );
					transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );

					meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
				}

				//C# Script
				{
					var script = CreateComponent<Component_CSharpScript>();
					script.Name = "C# Script";
					script.Code = "public void InteractiveObjectButton_UpdateEvent(NeoAxis.Component sender, float delta)\r\n{\r\n\tvar _this = sender as Component_ButtonInSpace;\r\n\tif (_this != null)\r\n\t{\r\n\t\tvar indicator = _this.GetComponent(\"Indicator\") as Component_MeshInSpace;\r\n\t\tif (indicator != null)\r\n\t\t\tindicator.Color = _this.Activated ? new ColorValue(0, 1, 0) : new ColorValue(0.5, 0.5, 0.5);\r\n\r\n\t\tvar buttonOffset = _this.Components.GetByPath(\"Button\\\\Attach Transform Offset\") as Component_TransformOffset;\r\n\t\tif (buttonOffset != null)\r\n\t\t{\r\n\t\t\tvar offsetPushed = 0.01;\r\n\t\t\tvar offsetDefault = 0.05;\r\n\r\n\t\t\tvar coef = 0.0;\r\n\t\t\tif (_this.Clicking && _this.ClickingTotalTime != 0)\r\n\t\t\t{\r\n\t\t\t\tvar timeFactor = MathEx.Saturate(_this.ClickingCurrentTime / _this.ClickingTotalTime);\r\n\r\n\t\t\t\tif(timeFactor < 0.5)\r\n\t\t\t\t\tcoef = timeFactor * 2;\r\n\t\t\t\telse\r\n\t\t\t\t\tcoef = (1.0f - timeFactor) * 2;\r\n\t\t\t}\r\n\r\n\t\t\tvar offset = MathEx.Lerp(offsetDefault, offsetPushed, coef);\r\n\t\t\tbuttonOffset.PositionOffset = new Vector3(offset, 0, 0);\r\n\t\t}\r\n\t}\r\n}";

					var handler = script.CreateComponent<Component_EventHandler>();
					handler.Name = "Event Handler UpdateEvent";
					handler.WhenEnable = Component_EventHandler.WhenEnableEnum.Editor | Component_EventHandler.WhenEnableEnum.Simulation | Component_EventHandler.WhenEnableEnum.Instance;
					handler.Event = ReferenceUtility.MakeReference( "this:..\\..\\event:UpdateEvent" );
					handler.HandlerMethod = ReferenceUtility.MakeReference( "this:..\\method:InteractiveObjectButton_UpdateEvent(NeoAxis.Component,System.Single)" );
				}
			}
		}

		public void SoundPlay( Component_Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public virtual bool ObjectInteractionInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					ClickingBegin();
					return true;
				}
			}

			return false;
		}

		public void ObjectInteractionGetInfo( UIControl playScreen, Component_GameMode gameMode, ref IComponent_InteractiveObject_ObjectInfo info )
		{
			info = new IComponent_InteractiveObject_ObjectInfo();
			info.AllowInteract = AllowInteract;
			info.SelectionTextInfo.Add( Name );
		}

		public void ObjectInteractionEnter( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionExit( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionUpdate( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}
	}
}