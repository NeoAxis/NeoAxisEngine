// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Base class for tasks of the character to turn to the specified position.
	/// </summary>
	public class CharacterAITask_TurnTo : AITask
	{
		///// <summary>
		///// Whether to turn instantly without animation.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> TurnInstantly
		//{
		//	get { if( _turnInstantly.BeginGet() ) TurnInstantly = _turnInstantly.Get( this ); return _turnInstantly.value; }
		//	set { if( _turnInstantly.BeginSet( this, ref value ) ) { try { TurnInstantlyChanged?.Invoke( this ); } finally { _turnInstantly.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TurnInstantly"/> property value changes.</summary>
		//public event Action<CharacterAITask_TurnTo> TurnInstantlyChanged;
		//ReferenceField<bool> _turnInstantly = false;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the character to turn to the specified position in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Turn To Position", -8994 )]
	[NewObjectDefaultName( "Turn To Position" )]
	public class CharacterAITask_TurnToPosition : CharacterAITask_TurnTo
	{
		/// <summary>
		/// The target position.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( this, ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<CharacterAITask_TurnToPosition> TargetChanged;
		ReferenceField<Vector3> _target = new Vector3( 0, 0, 0 );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the character to turn to the specified object in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Turn To Object", -8993 )]
	[NewObjectDefaultName( "Turn To Object" )]
	public class CharacterAITask_TurnToObject : CharacterAITask_TurnTo
	{
		/// <summary>
		/// The target object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ObjectInSpace> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( this, ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<CharacterAITask_TurnToObject> TargetChanged;
		ReferenceField<ObjectInSpace> _target = null;
	}
}
