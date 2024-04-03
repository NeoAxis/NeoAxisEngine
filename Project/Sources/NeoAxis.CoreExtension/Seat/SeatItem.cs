// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A seat item for a character or other object in the seat.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Seat\Seat Item", 10578 )]
	public class SeatItem : Component
	{
		/// <summary>
		/// The position and rotation of the seat.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( "0 0 0; 0 0 0 1; 1 1 1" )]
		public Reference<Transform> Transform
		{
			get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
			set { if( _transform.BeginSet( this, ref value ) ) { try { TransformChanged?.Invoke( this ); } finally { _transform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<SeatItem> TransformChanged;
		ReferenceField<Transform> _transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The angle of spine from vertical to back.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<Degree> SpineAngle
		{
			get { if( _spineAngle.BeginGet() ) SpineAngle = _spineAngle.Get( this ); return _spineAngle.value; }
			set { if( _spineAngle.BeginSet( this, ref value ) ) { try { SpineAngleChanged?.Invoke( this ); } finally { _spineAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpineAngle"/> property value changes.</summary>
		public event Action<SeatItem> SpineAngleChanged;
		ReferenceField<Degree> _spineAngle = new Degree( 0.0 );

		/// <summary>
		/// The angle of legs from vertical to front.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<Degree> LegsAngle
		{
			get { if( _legsAngle.BeginGet() ) LegsAngle = _legsAngle.Get( this ); return _legsAngle.value; }
			set { if( _legsAngle.BeginSet( this, ref value ) ) { try { LegsAngleChanged?.Invoke( this ); } finally { _legsAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LegsAngle"/> property value changes.</summary>
		public event Action<SeatItem> LegsAngleChanged;
		ReferenceField<Degree> _legsAngle = new Degree( 0.0 );

		/// <summary>
		/// The position and rotation when exit from the seat.
		/// </summary>
		[DefaultValue( "1 0 0; 0 0 0 1; 1 1 1" )]
		public Reference<Transform> ExitTransform
		{
			get { if( _exitTransform.BeginGet() ) ExitTransform = _exitTransform.Get( this ); return _exitTransform.value; }
			set { if( _exitTransform.BeginSet( this, ref value ) ) { try { ExitTransformChanged?.Invoke( this ); } finally { _exitTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExitTransform"/> property value changes.</summary>
		public event Action<SeatItem> ExitTransformChanged;
		ReferenceField<Transform> _exitTransform = new Transform( new Vector3( 1, 0, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// Whether to show the object on the seat.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DisplayObject
		{
			get { if( _displayObject.BeginGet() ) DisplayObject = _displayObject.Get( this ); return _displayObject.value; }
			set { if( _displayObject.BeginSet( this, ref value ) ) { try { DisplayObjectChanged?.Invoke( this ); } finally { _displayObject.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayObject"/> property value changes.</summary>
		public event Action<SeatItem> DisplayObjectChanged;
		ReferenceField<bool> _displayObject = true;


		///// <summary>
		///// Default eyes offset from the seat transform.
		///// </summary>
		//[DefaultValue( "0 0 0.5" )]
		//public Reference<Vector3> EyeOffset
		//{
		//	get { if( _eyeOffset.BeginGet() ) EyeOffset = _eyeOffset.Get( this ); return _eyeOffset.value; }
		//	set { if( _eyeOffset.BeginSet( this, ref value ) ) { try { EyeOffsetChanged?.Invoke( this ); } finally { _eyeOffset.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EyeOffset"/> property value changes.</summary>
		//public event Action<SeatSeat> EyeOffsetChanged;
		//ReferenceField<Vector3> _eyeOffset = new Vector3( 0, 0, 0.5 );

		///// <summary>
		///// The animation name of the character when it on the seat.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( "" )]
		//public Reference<string> Animation
		//{
		//	get { if( _animation.BeginGet() ) Animation = _animation.Get( this ); return _animation.value; }
		//	set { if( _animation.BeginSet( this, ref value ) ) { try { AnimationChanged?.Invoke( this ); } finally { _animation.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Animation"/> property value changes.</summary>
		//public event Action<SeatSeat> AnimationChanged;
		//ReferenceField<string> _animation = "";

		/////////////////////////////////////////

		//public virtual void DebugRender( ViewportRenderingContext context )
		//{
		//	var seat = Parent as Seat;
		//	if( seat != null )
		//	{
		//		var renderer = context.Owner.Simple3DRenderer;
		//		var seatTransform = seat.TransformV;

		//		//seat
		//		{
		//			var color = new ColorValue( 0, 1, 0 );
		//			renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
		//			var tr = seatTransform * Transform.Value;
		//			var p = tr * new Vector3( 0, 0, 0 );
		//			renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
		//			renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
		//		}

		//		//exit
		//		{
		//			var color = new ColorValue( 1, 0, 0 );
		//			renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
		//			var tr = seatTransform * ExitTransform.Value;
		//			var p = tr * new Vector3( 0, 0, 0 );
		//			renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
		//			renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
		//		}
		//	}
		//}
	}
}
