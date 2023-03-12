// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Base class for tasks of the character to move to the specified position.
	/// </summary>
	public class CharacterAITask_MoveTo : AITask
	{
		/// <summary>
		/// Whether to run when moving.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Run
		{
			get { if( _run.BeginGet() ) Run = _run.Get( this ); return _run.value; }
			set { if( _run.BeginSet( ref value ) ) { try { RunChanged?.Invoke( this ); } finally { _run.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Run"/> property value changes.</summary>
		public event Action<CharacterAITask_MoveTo> RunChanged;
		ReferenceField<bool> _run = false;

		/// <summary>
		/// The required distance to the point to complete the task.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> DistanceToReach
		{
			get { if( _distanceToReach.BeginGet() ) DistanceToReach = _distanceToReach.Get( this ); return _distanceToReach.value; }
			set { if( _distanceToReach.BeginSet( ref value ) ) { try { DistanceToReachChanged?.Invoke( this ); } finally { _distanceToReach.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DistanceToReach"/> property value changes.</summary>
		public event Action<CharacterAITask_MoveTo> DistanceToReachChanged;
		ReferenceField<double> _distanceToReach = 0.5;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the character to move to the specified position in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Move To Position", -8996 )]
	[NewObjectDefaultName( "Move To Position" )]
	public class CharacterAITask_MoveToPosition : CharacterAITask_MoveTo
	{
		/// <summary>
		/// The target position.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<CharacterAITask_MoveToPosition> TargetChanged;
		ReferenceField<Vector3> _target = new Vector3( 0, 0, 0 );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the character to move to the specified object in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI Move To Object", -8995 )]
	[NewObjectDefaultName( "Move To Object" )]
	public class CharacterAITask_MoveToObject : CharacterAITask_MoveTo
	{
		/// <summary>
		/// The target object.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ObjectInSpace> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<CharacterAITask_MoveToObject> TargetChanged;
		ReferenceField<ObjectInSpace> _target = null;
	}
}
