// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to specify modular element of the building. It can be a child of BuildingType and Building components.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Building\Building Element", 310 )]
	public class BuildingElement : Component
	{
		public enum ElementTypeEnum
		{
			Side,
			SideEdge,
			Roof,
			RoofEdge,
			RoofCorner,
			//!!!!
			//Foundation,
			//FoundationCorner,
			//FoundationSideCorner,
			//FoundationEntrance,

			//!!!!CellSide
			//!!!!CellCorner
			Cell,

			Surrounding,
			RoofSurrounding,
		}

		/// <summary>
		/// The type of the element.
		/// </summary>
		[DefaultValue( ElementTypeEnum.Side )]
		public Reference<ElementTypeEnum> ElementType
		{
			get { if( _elementType.BeginGet() ) ElementType = _elementType.Get( this ); return _elementType.value; }
			set { if( _elementType.BeginSet( ref value ) ) { try { ElementTypeChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _elementType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ElementType"/> property value changes.</summary>
		public event Action<BuildingElement> ElementTypeChanged;
		ReferenceField<ElementTypeEnum> _elementType = ElementTypeEnum.Side;

		/// <summary>
		/// The mesh of the element.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<BuildingElement> MeshChanged;
		ReferenceField<Mesh> _mesh = null;


		//!!!!BlockSize? Size in grid

		//!!!!
		////!!!!это как кастомный сайз?
		////!!!!default
		//[DefaultValue( "1 1 1" )]
		//public Reference<Vector3> Size
		//{
		//	get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
		//	set { if( _size.BeginSet( ref value ) ) { try { SizeChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _size.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		//public event Action<BuildingElement> SizeChanged;
		//ReferenceField<Vector3> _size = new Vector3( 1, 1, 1 );

		/// <summary>
		/// The level interval where the element can be created.
		/// </summary>
		[DefaultValue( "-10000 10000" )]
		public Reference<RangeI> Levels
		{
			get { if( _levels.BeginGet() ) Levels = _levels.Get( this ); return _levels.value; }
			set { if( _levels.BeginSet( ref value ) ) { try { LevelsChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _levels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Levels"/> property value changes.</summary>
		public event Action<BuildingElement> LevelsChanged;
		ReferenceField<RangeI> _levels = new RangeI( -10000, 10000 );

		[Flags]
		public enum SidesEnum
		{
			None = 0,
			MinusX = 1,
			MinusY = 2,
			PlusX = 4,
			PlusY = 8,
			All = MinusX | MinusY | PlusX | PlusY
		}

		/// <summary>
		/// The sides where the element can be created.
		/// </summary>
		[DefaultValue( SidesEnum.All )]
		public Reference<SidesEnum> Sides
		{
			get { if( _sides.BeginGet() ) Sides = _sides.Get( this ); return _sides.value; }
			set { if( _sides.BeginSet( ref value ) ) { try { SidesChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _sides.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Sides"/> property value changes.</summary>
		public event Action<BuildingElement> SidesChanged;
		ReferenceField<SidesEnum> _sides = SidesEnum.All;

		/// <summary>
		/// The probability of choosing this element from others.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( ref value ) ) { try { ProbabilityChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<BuildingElement> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		/// <summary>
		/// The distance of surrounding objects.
		/// </summary>
		[DefaultValue( 4.0 )]
		//[Category( "Surrounding" )]
		public Reference<double> Distance
		{
			get { if( _distance.BeginGet() ) Distance = _distance.Get( this ); return _distance.value; }
			set { if( _distance.BeginSet( ref value ) ) { try { DistanceChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _distance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Distance"/> property value changes.</summary>
		public event Action<BuildingElement> DistanceChanged;
		ReferenceField<double> _distance = 4.0;

		/// <summary>
		/// The interval of surrounding objects.
		/// </summary>
		[DefaultValue( "0 100000" )]
		//[Category( "Surrounding" )]
		public Reference<Range> Interval
		{
			get { if( _interval.BeginGet() ) Interval = _interval.Get( this ); return _interval.value; }
			set { if( _interval.BeginSet( ref value ) ) { try { IntervalChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _interval.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Interval"/> property value changes.</summary>
		public event Action<BuildingElement> IntervalChanged;
		ReferenceField<Range> _interval = new Range( 0, 100000 );

		/// <summary>
		/// The step of surrounding objects.
		/// </summary>
		[DefaultValue( 8.0 )]
		//[Category( "Surrounding" )]
		public Reference<double> Step
		{
			get { if( _step.BeginGet() ) Step = _step.Get( this ); return _step.value; }
			set { if( _step.BeginSet( ref value ) ) { try { StepChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _step.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Step"/> property value changes.</summary>
		public event Action<BuildingElement> StepChanged;
		ReferenceField<double> _step = 8.0;


		//!!!!всем поддержать

		/// <summary>
		/// The position offset of the element.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> PositionOffset
		{
			get { if( _positionOffset.BeginGet() ) PositionOffset = _positionOffset.Get( this ); return _positionOffset.value; }
			set { if( _positionOffset.BeginSet( ref value ) ) { try { PositionOffsetChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _positionOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		public event Action<BuildingElement> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset = Vector3.Zero;

		/// <summary>
		/// The rotation offset of the element.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		public Reference<Quaternion> RotationOffset
		{
			get { if( _rotationOffset.BeginGet() ) RotationOffset = _rotationOffset.Get( this ); return _rotationOffset.value; }
			set { if( _rotationOffset.BeginSet( ref value ) ) { try { RotationOffsetChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _rotationOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffset"/> property value changes.</summary>
		public event Action<BuildingElement> RotationOffsetChanged;
		ReferenceField<Quaternion> _rotationOffset = Quaternion.Identity;

		/// <summary>
		/// The scale offset of the element.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> ScaleOffset
		{
			get { if( _scaleOffset.BeginGet() ) ScaleOffset = _scaleOffset.Get( this ); return _scaleOffset.value; }
			set { if( _scaleOffset.BeginSet( ref value ) ) { try { ScaleOffsetChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _scaleOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleOffset"/> property value changes.</summary>
		public event Action<BuildingElement> ScaleOffsetChanged;
		ReferenceField<Vector3> _scaleOffset = new Vector3( 1, 1, 1 );



		//!!!!Side: можно ли увидеть комнату


		//!!!!string Specialization? AnyData?


		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Levels ):
					if( ElementType.Value == ElementTypeEnum.Roof || ElementType.Value == ElementTypeEnum.RoofEdge || ElementType.Value == ElementTypeEnum.RoofCorner || ElementType.Value == ElementTypeEnum.Surrounding || ElementType.Value == ElementTypeEnum.RoofSurrounding )
						skip = true;
					break;

				case nameof( Sides ):
					if( ElementType.Value == ElementTypeEnum.Roof )
						skip = true;
					break;

				//!!!!
				//case nameof( Size ):
				//	if( ElementType.Value == ElementTypeEnum.Surrounding )
				//		skip = true;
				//	break;

				case nameof( Distance ):
				case nameof( Interval ):
				case nameof( Step ):
					if( ElementType.Value != ElementTypeEnum.Surrounding && ElementType.Value != ElementTypeEnum.RoofSurrounding )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			ParentNeedUpdate();
		}

		void ParentNeedUpdate()
		{
			var building = Parent as Building;
			building?.NeedUpdate();
		}
	}
}
