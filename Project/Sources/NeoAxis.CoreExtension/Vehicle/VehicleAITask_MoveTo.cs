// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Base class for tasks of the vehicle to move to the specified position.
	/// </summary>
	public class VehicleAITask_MoveTo : AITask
	{
		/// <summary>
		/// The demanded speed.
		/// </summary>
		[DefaultValue( 30.0 )]
		[Range( 1, 200, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Speed
		{
			get { if( _speed.BeginGet() ) Speed = _speed.Get( this ); return _speed.value; }
			set { if( _speed.BeginSet( this, ref value ) ) { try { SpeedChanged?.Invoke( this ); } finally { _speed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Speed"/> property value changes.</summary>
		public event Action<VehicleAITask_MoveTo> SpeedChanged;
		ReferenceField<double> _speed = 30.0;

		//!!!!
		//[DefaultValue( true )]
		//public Reference<bool> UseRoads
		//{
		//	get { if( _useRoads.BeginGet() ) UseRoads = _useRoads.Get( this ); return _useRoads.value; }
		//	set { if( _useRoads.BeginSet( this, ref value ) ) { try { UseRoadsChanged?.Invoke( this ); } finally { _useRoads.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UseRoads"/> property value changes.</summary>
		//public event Action<VehicleAITask_MoveTo> UseRoadsChanged;
		//ReferenceField<bool> _useRoads = true;

		/// <summary>
		/// The required distance to the point to complete the task.
		/// </summary>
		[DefaultValue( 3.0 )]
		[Range( 0, 20, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> DistanceToReach
		{
			get { if( _distanceToReach.BeginGet() ) DistanceToReach = _distanceToReach.Get( this ); return _distanceToReach.value; }
			set { if( _distanceToReach.BeginSet( this, ref value ) ) { try { DistanceToReachChanged?.Invoke( this ); } finally { _distanceToReach.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DistanceToReach"/> property value changes.</summary>
		public event Action<VehicleAITask_MoveTo> DistanceToReachChanged;
		ReferenceField<double> _distanceToReach = 3.0;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the vehicle to move to the specified position in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle AI Move To Position", 22005 )]
	[NewObjectDefaultName( "Move To Position" )]
	public class VehicleAITask_MoveToPosition : VehicleAITask_MoveTo
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
		public event Action<VehicleAITask_MoveToPosition> TargetChanged;
		ReferenceField<Vector3> _target = new Vector3( 0, 0, 0 );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The task of the vehicle to move to the specified object in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle AI Move To Object", 22006 )]
	[NewObjectDefaultName( "Move To Object" )]
	public class VehicleAITask_MoveToObject : VehicleAITask_MoveTo
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
		public event Action<VehicleAITask_MoveToObject> TargetChanged;
		ReferenceField<ObjectInSpace> _target = null;
	}
}