//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	//!!!!impl

//	/// <summary>
//	/// A definition of the aero engine of the vehicle.
//	/// </summary>
//	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Aero Engine", 22002 )]
//	[NewObjectDefaultName( "Aero Engine" )]
//	public class VehicleAeroEngine : qqqMeshInSpace
//	{

//		///// <summary>
//		///// The position and rotation of the engine.
//		///// </summary>
//		//[DefaultValue( "0 0 0; 0 0 0 1; 1 1 1" )]
//		//public Reference<Transform> Transform
//		//{
//		//	get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
//		//	set { if( _transform.BeginSet( this, ref value ) ) { try { TransformChanged?.Invoke( this ); DataWasChanged(); } finally { _transform.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
//		//public event Action<VehicleTypeAeroEngine> TransformChanged;
//		//ReferenceField<Transform> _transform = new Transform( new Vector3( 0, 0, 0 ), Quaternion.Identity, Vector3.One );

//		//!!!!need?
//		///// <summary>
//		///// Whether to create two engine, instead of one.
//		///// </summary>
//		//[DefaultValue( true )]
//		//public Reference<bool> Pair
//		//{
//		//	get { if( _pair.BeginGet() ) Pair = _pair.Get( this ); return _pair.value; }
//		//	set { if( _pair.BeginSet( this, ref value ) ) { try { PairChanged?.Invoke( this ); DataWasChanged(); } finally { _pair.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Pair"/> property value changes.</summary>
//		//public event Action<VehicleAeroEngine> PairChanged;
//		//ReferenceField<bool> _pair = true;


//		////!!!!default
//		//const string meshDefault = "";//@"Content\Vehicles\Default\Wheel.gltf|$Mesh";

//		///// <summary>
//		///// The optional mesh of the aero engine.
//		///// </summary>
//		//[DefaultValueReference( meshDefault )]
//		//public Reference<Mesh> Mesh
//		//{
//		//	get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
//		//	set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
//		//public event Action<VehicleTypeAeroEngine> MeshChanged;
//		//ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );


//		//!!!!default
//		const string movingPartMeshDefault = "";//@"Content\Vehicles\Default\Wheel.gltf|$Mesh";

//		/// <summary>
//		/// The moving part mesh of the aero engine.
//		/// </summary>
//		[DefaultValueReference( movingPartMeshDefault )]
//		public Reference<Mesh> MovingPartMesh
//		{
//			get { if( _movingPartMesh.BeginGet() ) Mesh = _movingPartMesh.Get( this ); return _movingPartMesh.value; }
//			set { if( _movingPartMesh.BeginSet( this, ref value ) ) { try { MovingPartMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _movingPartMesh.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
//		public event Action<VehicleAeroEngine> MovingPartMeshChanged;
//		ReferenceField<Mesh> _movingPartMesh = new Reference<Mesh>( null, movingPartMeshDefault );

//		/// <summary>
//		/// The force multiplier to the throttle.
//		/// </summary>
//		[DefaultValue( 10000000.0 )]
//		public Reference<double> Force
//		{
//			get { if( _force.BeginGet() ) Force = _force.Get( this ); return _force.value; }
//			set { if( _force.BeginSet( this, ref value ) ) { try { ForceChanged?.Invoke( this ); } finally { _force.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Force"/> property value changes.</summary>
//		public event Action<VehicleAeroEngine> ForceChanged;
//		ReferenceField<double> _force = 10000000.0;


//		//!!!!крыло, подъемная сила

//		//!!!!скорость вращения пропеллера, турбины


//		/////////////////////////////////////////////////

//		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
//		{
//			base.OnMetadataGetMembersFilter( context, member, ref skip );

//			//if( member is Metadata.Property )
//			//{
//			//	switch( member.Name )
//			//	{
//			//	case nameof( TracksLateralFriction ):
//			//		{
//			//			var parent = Parent as VehicleType;
//			//			var tracks = parent != null && parent.Chassis.Value == VehicleType.ChassisEnum.Tracks;
//			//			if( !tracks )
//			//				skip = true;
//			//		}
//			//		break;
//			//	}
//			//}
//		}

//		public void DataWasChanged()
//		{
//			var parent = Parent as VehicleType;
//			parent?.DataWasChanged();
//		}
//	}
//}
