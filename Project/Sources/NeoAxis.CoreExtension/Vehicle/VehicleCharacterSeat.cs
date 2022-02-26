// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A seat place for character in the vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Vehicle Character Seat", -7998 )]
	public class VehicleCharacterSeat : Component
	{
		/// <summary>
		/// The position and rotation of the seat.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> Transform
		{
			get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
			set { if( _transform.BeginSet( ref value ) ) { try { TransformChanged?.Invoke( this ); } finally { _transform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> TransformChanged;
		ReferenceField<Transform> _transform = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		/// <summary>
		/// Default eyes offset from the seat transform.
		/// </summary>
		[DefaultValue( "0 0 0.5" )]
		public Reference<Vector3> EyeOffset
		{
			get { if( _eyeOffset.BeginGet() ) EyeOffset = _eyeOffset.Get( this ); return _eyeOffset.value; }
			set { if( _eyeOffset.BeginSet( ref value ) ) { try { EyeOffsetChanged?.Invoke( this ); } finally { _eyeOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EyeOffset"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> EyeOffsetChanged;
		ReferenceField<Vector3> _eyeOffset = new Vector3( 0, 0, 0.5 );

		/// <summary>
		/// The position and rotation when exit from the vehicle.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> ExitTransform
		{
			get { if( _exitTransform.BeginGet() ) ExitTransform = _exitTransform.Get( this ); return _exitTransform.value; }
			set { if( _exitTransform.BeginSet( ref value ) ) { try { ExitTransformChanged?.Invoke( this ); } finally { _exitTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExitTransform"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> ExitTransformChanged;
		ReferenceField<Transform> _exitTransform = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		/// <summary>
		/// Whether to show the character.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( false )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set { if( _visible.BeginSet( ref value ) ) { try { VisibleChanged?.Invoke( this ); } finally { _visible.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> VisibleChanged;
		ReferenceField<bool> _visible = false;

		/// <summary>
		/// The animation name of the character when it on the seat.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( "" )]
		public Reference<string> Animation
		{
			get { if( _animation.BeginGet() ) Animation = _animation.Get( this ); return _animation.value; }
			set { if( _animation.BeginSet( ref value ) ) { try { AnimationChanged?.Invoke( this ); } finally { _animation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Animation"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> AnimationChanged;
		ReferenceField<string> _animation = "";

		/// <summary>
		/// The current character on the seat.
		/// </summary>
		[Category( "State" )]
		[DefaultValue( null )]
		public Reference<Character> Character
		{
			get { if( _character.BeginGet() ) Character = _character.Get( this ); return _character.value; }
			set { if( _character.BeginSet( ref value ) ) { try { CharacterChanged?.Invoke( this ); } finally { _character.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Character"/> property value changes.</summary>
		public event Action<VehicleCharacterSeat> CharacterChanged;
		ReferenceField<Character> _character = null;

		/////////////////////////////////////////

		protected virtual void UpdateCharacter( Character character )
		{
			//!!!!animation

			var vehicle = Parent as Vehicle;
			if( vehicle != null )
			{
				character.Visible = Visible;
				character.DestroyCollisionBody();
				character.SetTransform( vehicle.TransformV * Transform.Value, true );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			var character = Character.Value;
			if( character != null )
				UpdateCharacter( character );
		}

		public virtual void PutCharacterToSeat( Character character )
		{
			Character = ReferenceUtility.MakeRootReference( character );

			UpdateCharacter( character );
		}

		public virtual void RemoveCharacterFromSeat()
		{
			var character = Character.Value;
			if( character != null && !character.Disposed )
			{
				character.UpdateCollisionBody();
				character.Visible = true;

				var vehicle = Parent as Vehicle;
				if( vehicle != null )
					character.SetTransform( vehicle.TransformV * ExitTransform.Value, true );
			}

			Character = null;
		}

		public virtual void DebugRender( ViewportRenderingContext context )
		{
			var vehicle = Parent as Vehicle;
			if( vehicle != null )
			{
				var renderer = context.Owner.Simple3DRenderer;
				var vehicleTransform = vehicle.TransformV;

				//seat
				{
					var color = new ColorValue( 0, 1, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );
					var tr = vehicleTransform * Transform.Value;
					var p = tr * new Vector3( 0, 0, 0 );
					renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
					renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
				}

				//exit
				{
					var color = new ColorValue( 1, 0, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );
					var tr = vehicleTransform * ExitTransform.Value;
					var p = tr * new Vector3( 0, 0, 0 );
					renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
					renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
				}
			}
		}
	}
}
