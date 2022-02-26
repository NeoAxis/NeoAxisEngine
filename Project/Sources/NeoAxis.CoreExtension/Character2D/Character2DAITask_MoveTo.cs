// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Base class for tasks of the 2D character to move to the specified position.
	/// </summary>
	public class Character2DAITask_MoveTo : AITask
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
		public event Action<Character2DAITask_MoveTo> RunChanged;
		ReferenceField<bool> _run = false;

		/// <summary>
		/// The required distance to the point to complete the task.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> DistanceToReach
		{
			get { if( _distanceToReach.BeginGet() ) DistanceToReach = _distanceToReach.Get( this ); return _distanceToReach.value; }
			set { if( _distanceToReach.BeginSet( ref value ) ) { try { DistanceToReachChanged?.Invoke( this ); } finally { _distanceToReach.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DistanceToReach"/> property value changes.</summary>
		public event Action<Character2DAITask_MoveTo> DistanceToReachChanged;
		ReferenceField<double> _distanceToReach = 1.0;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the 2D character to move to the specified position in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Character 2D AI Move To Position", -7896 )]
	[NewObjectDefaultName( "Move To Position" )]
	public class Character2DAITask_MoveToPosition : Character2DAITask_MoveTo
	{
		/// <summary>
		/// The target position.
		/// </summary>
		[DefaultValue( "0 0" )]
		public Reference<Vector2> Target
		{
			get { if( _target.BeginGet() ) Target = _target.Get( this ); return _target.value; }
			set { if( _target.BeginSet( ref value ) ) { try { TargetChanged?.Invoke( this ); } finally { _target.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Target"/> property value changes.</summary>
		public event Action<Character2DAITask_MoveToPosition> TargetChanged;
		ReferenceField<Vector2> _target = new Vector2( 0, 0 );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the 2D character to move to the specified object in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Character 2D AI Move To Object", -7895 )]
	[NewObjectDefaultName( "Move To Object" )]
	public class Character2DAITask_MoveToObject : Character2DAITask_MoveTo
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
		public event Action<Character2DAITask_MoveToObject> TargetChanged;
		ReferenceField<ObjectInSpace> _target = null;
	}
}
