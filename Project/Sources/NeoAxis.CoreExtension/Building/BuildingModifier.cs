//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	//!!!!never used

//	/// <summary>
//	/// A building modifier to add more customization. It can be a child of BuildingType and Building components.
//	/// </summary>
//	[AddToResourcesWindow( @"Addons\Building\Building Modifier", 320 )]
//	public class BuildingModifier : Component
//	{

//		//!!!!дополняющий модификатор. сейчас только заменяющий


//		public enum PartEnum
//		{
//			Cell,
//			Side,
//			Custom,
//		}

//		[DefaultValue( PartEnum.Cell )]
//		public Reference<PartEnum> Part
//		{
//			get { if( _part.BeginGet() ) Part = _part.Get( this ); return _part.value; }
//			set { if( _part.BeginSet( this, ref value ) ) { try { PartChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _part.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Part"/> property value changes.</summary>
//		public event Action<BuildingModifier> PartChanged;
//		ReferenceField<PartEnum> _part = PartEnum.Cell;

//		[DefaultValue( "0 0 0" )]
//		public Reference<Vector3I> CellPosition
//		{
//			get { if( _cellPosition.BeginGet() ) CellPosition = _cellPosition.Get( this ); return _cellPosition.value; }
//			set { if( _cellPosition.BeginSet( this, ref value ) ) { try { CellPositionChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _cellPosition.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="CellPosition"/> property value changes.</summary>
//		public event Action<BuildingModifier> CellPositionChanged;
//		ReferenceField<Vector3I> _cellPosition = new Vector3I( 0, 0, 0 );

//		[DefaultValue( 0 )]
//		public Reference<int> Side
//		{
//			get { if( _side.BeginGet() ) Side = _side.Get( this ); return _side.value; }
//			set { if( _side.BeginSet( this, ref value ) ) { try { SideChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _side.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Side"/> property value changes.</summary>
//		public event Action<BuildingModifier> SideChanged;
//		ReferenceField<int> _side = 0;

//		//!!!!Vector2
//		[DefaultValue( "0 0" )]
//		public Reference<Vector2I> Position
//		{
//			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
//			set { if( _position.BeginSet( this, ref value ) ) { try { PositionChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _position.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
//		public event Action<BuildingModifier> PositionChanged;
//		ReferenceField<Vector2I> _position = new Vector2I( 0, 0 );

//		///// <summary>
//		///// The position where applied the modifier. For Cell position type, it is 3D index. For Side type, X is number of side. Y, Z are position.
//		///// </summary>
//		//[DefaultValue( "0 0 0" )]
//		//public Reference<Vector3I> Position
//		//{
//		//	get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
//		//	set { if( _position.BeginSet( this, ref value ) ) { try { PositionChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _position.EndSet(); } } }
//		//}
//		///// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
//		//public event Action<BuildingModifier> PositionChanged;
//		//ReferenceField<Vector3I> _position = new Vector3I( 0, 0, 0 );

//		[DefaultValue( null )]
//		public Reference<BuildingElement> Element
//		{
//			get { if( _element.BeginGet() ) Element = _element.Get( this ); return _element.value; }
//			set { if( _element.BeginSet( this, ref value ) ) { try { ElementChanged?.Invoke( this ); ParentNeedUpdate(); } finally { _element.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Element"/> property value changes.</summary>
//		public event Action<BuildingModifier> ElementChanged;
//		ReferenceField<BuildingElement> _element = null;

//		//!!!!
//		//Specialization = "Window"

//		///////////////////////////////////////////////

//		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
//		{
//			base.OnMetadataGetMembersFilter( context, member, ref skip );

//			if( member is Metadata.Property )
//			{
//				switch( member.Name )
//				{
//				case nameof( CellPosition ):
//					if( Part.Value != PartEnum.Cell )
//						skip = true;
//					break;

//				case nameof( Side ):
//				case nameof( Position ):
//					if( Part.Value != PartEnum.Side )
//						skip = true;
//					break;
//				}
//			}
//		}

//		protected override void OnEnabledChanged()
//		{
//			base.OnEnabledChanged();

//			ParentNeedUpdate();
//		}

//		void ParentNeedUpdate()
//		{
//			var building = Parent as Building;
//			building?.NeedUpdate();
//		}
//	}

//	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	//!!!!
//	//public class BuildingModifier_Element
//	//{

//	//	//Entrance

//	//}
//}
