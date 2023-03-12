// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a material to configure plant type.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Plant\Plant Material", 10010 )]
	public class PlantMaterial : Component
	{
		/// <summary>
		/// The probability of choosing this element from others.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 10.0, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "General" )]
		public Reference<double> Probability
		{
			get { if( _probability.BeginGet() ) Probability = _probability.Get( this ); return _probability.value; }
			set { if( _probability.BeginSet( ref value ) ) { try { ProbabilityChanged?.Invoke( this ); } finally { _probability.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Probability"/> property value changes.</summary>
		public event Action<PlantMaterial> ProbabilityChanged;
		ReferenceField<double> _probability = 1.0;

		/// <summary>
		/// The reference to the material.
		/// </summary>
		[DefaultValue( null )]
		[Category( "General" )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<PlantMaterial> MaterialChanged;
		ReferenceField<Material> _material = null;

		public enum PartTypeEnum
		{
			Bark,
			BranchWithLeaves,
			Leaf,
			Flower,
		}

		/// <summary>
		/// The type of the part.
		/// </summary>
		[DefaultValue( PartTypeEnum.Bark )]
		[Category( "General" )]
		public Reference<PartTypeEnum> PartType
		{
			get { if( _partType.BeginGet() ) PartType = _partType.Get( this ); return _partType.value; }
			set { if( _partType.BeginSet( ref value ) ) { try { PartTypeChanged?.Invoke( this ); } finally { _partType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PartType"/> property value changes.</summary>
		public event Action<PlantMaterial> PartTypeChanged;
		ReferenceField<PartTypeEnum> _partType = PartTypeEnum.Bark;

		/// <summary>
		/// The amount of leafs.
		/// </summary>
		[DefaultValue( 1 )]
		[Category( "General" )]
		public Reference<int> LeafCount
		{
			get { if( _leafCount.BeginGet() ) LeafCount = _leafCount.Get( this ); return _leafCount.value; }
			set { if( _leafCount.BeginSet( ref value ) ) { try { LeafCountChanged?.Invoke( this ); } finally { _leafCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeafCount"/> property value changes.</summary>
		public event Action<PlantMaterial> LeafCountChanged;
		ReferenceField<int> _leafCount = 1;

		/// <summary>
		/// The age.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "General" )]
		public Reference<double> Age
		{
			get { if( _age.BeginGet() ) Age = _age.Get( this ); return _age.value; }
			set { if( _age.BeginSet( ref value ) ) { try { AgeChanged?.Invoke( this ); } finally { _age.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Age"/> property value changes.</summary>
		public event Action<PlantMaterial> AgeChanged;
		ReferenceField<double> _age = 0.0;

		/// <summary>
		/// Season of a year. 0 - summer, 1 - fall, 2 - winter, 3 - spring.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 4 )]
		[Category( "General" )]
		public Reference<double> Season
		{
			get { if( _season.BeginGet() ) Season = _season.Get( this ); return _season.value; }
			set { if( _season.BeginSet( ref value ) ) { try { SeasonChanged?.Invoke( this ); } finally { _season.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Season"/> property value changes.</summary>
		public event Action<PlantMaterial> SeasonChanged;
		ReferenceField<double> _season = 0.0;

		/// <summary>
		/// The dead factor.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "General" )]
		public Reference<double> Dead
		{
			get { if( _dead.BeginGet() ) Dead = _dead.Get( this ); return _dead.value; }
			set { if( _dead.BeginSet( ref value ) ) { try { DeadChanged?.Invoke( this ); } finally { _dead.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Dead"/> property value changes.</summary>
		public event Action<PlantMaterial> DeadChanged;
		ReferenceField<double> _dead = 0.0;

		/// <summary>
		/// The fired factor.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "General" )]
		public Reference<double> Fired
		{
			get { if( _fired.BeginGet() ) Fired = _fired.Get( this ); return _fired.value; }
			set { if( _fired.BeginSet( ref value ) ) { try { FiredChanged?.Invoke( this ); } finally { _fired.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Fired"/> property value changes.</summary>
		public event Action<PlantMaterial> FiredChanged;
		ReferenceField<double> _fired = 0.0;

		//!!!!может иначе. это дополнение, его можно убрать
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//public Reference<double> Lichen
		//{
		//	get { if( _lichen.BeginGet() ) Lichen = _lichen.Get( this ); return _lichen.value; }
		//	set { if( _lichen.BeginSet( ref value ) ) { try { LichenChanged?.Invoke( this ); } finally { _lichen.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Lichen"/> property value changes.</summary>
		//public event Action<PlantMaterial> LichenChanged;
		//ReferenceField<double> _lichen = 0.0;

		/// <summary>
		/// The real length of the element.
		/// </summary>
		[Category( "Texture" )]
		[DefaultValue( 1.0 )]
		public Reference<double> RealLength
		{
			get { if( _realLength.BeginGet() ) RealLength = _realLength.Get( this ); return _realLength.value; }
			set { if( _realLength.BeginSet( ref value ) ) { try { RealLengthChanged?.Invoke( this ); } finally { _realLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RealLength"/> property value changes.</summary>
		public event Action<PlantMaterial> RealLengthChanged;
		ReferenceField<double> _realLength = 1.0;

		//[DefaultValue( 1.0 )]
		//[Range( 0.1, 2.0 )]
		//[Category( "Texture" )]
		//public Reference<double> BarkLength
		//{
		//	get { if( _barkLength.BeginGet() ) BarkLength = _barkLength.Get( this ); return _barkLength.value; }
		//	set { if( _barkLength.BeginSet( ref value ) ) { try { BarkLengthChanged?.Invoke( this ); } finally { _barkLength.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BarkLength"/> property value changes.</summary>
		//public event Action<PlantMaterial> BarkLengthChanged;
		//ReferenceField<double> _barkLength = 1.0;

		//[DefaultValue( 1.0 )]
		//[Range( 0.1, 2.0 )]
		//[Category( "Texture" )]
		//public Reference<double> BarkWidth
		//{
		//	get { if( _barkWidth.BeginGet() ) BarkWidth = _barkWidth.Get( this ); return _barkWidth.value; }
		//	set { if( _barkWidth.BeginSet( ref value ) ) { try { BarkWidthChanged?.Invoke( this ); } finally { _barkWidth.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BarkWidth"/> property value changes.</summary>
		//public event Action<PlantMaterial> BarkWidthChanged;
		//ReferenceField<double> _barkWidth = 1.0;


		public enum UVModeEnum
		{
			All,
			//Rectangle,
			Point,
		}

		/// <summary>
		/// The UV mode.
		/// </summary>
		[Category( "Texture" )]
		[DisplayName( "UV Mode" )]
		[DefaultValue( UVModeEnum.All )]
		public Reference<UVModeEnum> UVMode
		{
			get { if( _uVMode.BeginGet() ) UVMode = _uVMode.Get( this ); return _uVMode.value; }
			set { if( _uVMode.BeginSet( ref value ) ) { try { UVModeChanged?.Invoke( this ); } finally { _uVMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVMode"/> property value changes.</summary>
		public event Action<PlantMaterial> UVModeChanged;
		ReferenceField<UVModeEnum> _uVMode = UVModeEnum.All;

		/// <summary>
		/// The UV front position.
		/// </summary>
		[DefaultValue( "0.5 0.5" )]
		[Range( 0, 1 )]
		[Category( "Texture" )]
		[DisplayName( "UV Front Position" )]
		public Reference<Vector2> UVFrontPosition
		{
			get { if( _uVFrontPosition.BeginGet() ) UVFrontPosition = _uVFrontPosition.Get( this ); return _uVFrontPosition.value; }
			set { if( _uVFrontPosition.BeginSet( ref value ) ) { try { UVFrontPositionChanged?.Invoke( this ); } finally { _uVFrontPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVFrontPosition"/> property value changes.</summary>
		public event Action<PlantMaterial> UVFrontPositionChanged;
		ReferenceField<Vector2> _uVFrontPosition = new Vector2( 0.5, 0.5 );

		/// <summary>
		/// The UV front direction.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		[Category( "Texture" )]
		[DisplayName( "UV Front Direction" )]
		public Reference<Degree> UVFrontDirection
		{
			get { if( _uVFrontDirection.BeginGet() ) UVFrontDirection = _uVFrontDirection.Get( this ); return _uVFrontDirection.value; }
			set { if( _uVFrontDirection.BeginSet( ref value ) ) { try { UVFrontDirectionChanged?.Invoke( this ); } finally { _uVFrontDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVFrontDirection"/> property value changes.</summary>
		public event Action<PlantMaterial> UVFrontDirectionChanged;
		ReferenceField<Degree> _uVFrontDirection;

		/// <summary>
		/// The UV length range.
		/// </summary>
		[DefaultValue( "-0.05 0.5" )]
		[Range( -1, 1 )]
		[Category( "Texture" )]
		[DisplayName( "UV Length Range" )]
		public Reference<Range> UVLengthRange
		{
			get { if( _uVLengthRange.BeginGet() ) UVLengthRange = _uVLengthRange.Get( this ); return _uVLengthRange.value; }
			set { if( _uVLengthRange.BeginSet( ref value ) ) { try { UVLengthRangeChanged?.Invoke( this ); } finally { _uVLengthRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVLengthRange"/> property value changes.</summary>
		public event Action<PlantMaterial> UVLengthRangeChanged;
		ReferenceField<Range> _uVLengthRange = new Range( -0.05, 0.5 );

		/// <summary>
		/// The UV width.
		/// </summary>
		[DefaultValue( 0.25 )]
		[Range( 0, 1 )]
		[Category( "Texture" )]
		[DisplayName( "UV Width" )]
		public Reference<double> UVWidth
		{
			get { if( _uVWidth.BeginGet() ) UVWidth = _uVWidth.Get( this ); return _uVWidth.value; }
			set { if( _uVWidth.BeginSet( ref value ) ) { try { UVWidthChanged?.Invoke( this ); } finally { _uVWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVWidth"/> property value changes.</summary>
		public event Action<PlantMaterial> UVWidthChanged;
		ReferenceField<double> _uVWidth = 0.25;

		/// <summary>
		/// The UV radius.
		/// </summary>
		[DefaultValue( 0.25 )]
		[Range( 0, 1 )]
		[Category( "Texture" )]
		[DisplayName( "UV Radius" )]
		public Reference<double> UVRadius
		{
			get { if( _uVRadius.BeginGet() ) UVRadius = _uVRadius.Get( this ); return _uVRadius.value; }
			set { if( _uVRadius.BeginSet( ref value ) ) { try { UVRadiusChanged?.Invoke( this ); } finally { _uVRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVRadius"/> property value changes.</summary>
		public event Action<PlantMaterial> UVRadiusChanged;
		ReferenceField<double> _uVRadius = 0.25;

		//[DefaultValue( "0.5 0.5" )]
		//[Range( 0, 1 )]
		//[Category( "Left Front" )]
		//public Reference<Range> UVFrontWidthRange
		//{
		//	get { if( _uVFrontWidthRange.BeginGet() ) UVFrontWidthRange = _uVFrontWidthRange.Get( this ); return _uVFrontWidthRange.value; }
		//	set { if( _uVFrontWidthRange.BeginSet( ref value ) ) { try { UVFrontWidthRangeChanged?.Invoke( this ); } finally { _uVFrontWidthRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UVFrontWidthRange"/> property value changes.</summary>
		//public event Action<PlantMaterial> UVFrontWidthRangeChanged;
		//ReferenceField<Range> _uVFrontWidthRange = new Range( 0.5, 0.5 );

		/// <summary>
		/// Whether to specify UV of the back side.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Texture" )]
		[DisplayName( "UV Back" )]
		public Reference<bool> UVBack
		{
			get { if( _uVBack.BeginGet() ) UVBack = _uVBack.Get( this ); return _uVBack.value; }
			set { if( _uVBack.BeginSet( ref value ) ) { try { UVBackChanged?.Invoke( this ); } finally { _uVBack.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVBack"/> property value changes.</summary>
		public event Action<PlantMaterial> UVBackChanged;
		ReferenceField<bool> _uVBack = false;

		/// <summary>
		/// The UV position of the back side.
		/// </summary>
		[DefaultValue( "0.5 0.5" )]
		[Range( 0, 1 )]
		[Category( "Texture" )]
		[DisplayName( "UV Back Position" )]
		public Reference<Vector2> UVBackPosition
		{
			get { if( _uVBackPosition.BeginGet() ) UVBackPosition = _uVBackPosition.Get( this ); return _uVBackPosition.value; }
			set { if( _uVBackPosition.BeginSet( ref value ) ) { try { UVBackPositionChanged?.Invoke( this ); } finally { _uVBackPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVBackPosition"/> property value changes.</summary>
		public event Action<PlantMaterial> UVBackPositionChanged;
		ReferenceField<Vector2> _uVBackPosition = new Vector2( 0.5, 0.5 );

		/// <summary>
		/// The UV direction of the back side.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		[Category( "Texture" )]
		[DisplayName( "UV Back Direction" )]
		public Reference<Degree> UVBackDirection
		{
			get { if( _uVBackDirection.BeginGet() ) UVBackDirection = _uVBackDirection.Get( this ); return _uVBackDirection.value; }
			set { if( _uVBackDirection.BeginSet( ref value ) ) { try { UVBackDirectionChanged?.Invoke( this ); } finally { _uVBackDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVBackDirection"/> property value changes.</summary>
		public event Action<PlantMaterial> UVBackDirectionChanged;
		ReferenceField<Degree> _uVBackDirection;


		//[DefaultValue( "-0.05 0.5" )]
		//[Range( -1, 1 )]
		//[Category( "Left Back" )]
		//public Reference<Range> LeafBackLengthRange
		//{
		//	get { if( _leafBackLengthRange.BeginGet() ) LeafBackLengthRange = _leafBackLengthRange.Get( this ); return _leafBackLengthRange.value; }
		//	set { if( _leafBackLengthRange.BeginSet( ref value ) ) { try { LeafBackLengthRangeChanged?.Invoke( this ); } finally { _leafBackLengthRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeafBackLengthRange"/> property value changes.</summary>
		//public event Action<PlantMaterial> LeafBackLengthRangeChanged;
		//ReferenceField<Range> _leafBackLengthRange = new Range( -0.05, 0.5 );

		//[DefaultValue( "0.5 0.5" )]
		//[Range( 0, 1 )]
		//[Category( "Left Back" )]
		//public Reference<Range> LeafBackWidthRange
		//{
		//	get { if( _leafBackWidthRange.BeginGet() ) LeafBackWidthRange = _leafBackWidthRange.Get( this ); return _leafBackWidthRange.value; }
		//	set { if( _leafBackWidthRange.BeginSet( ref value ) ) { try { LeafBackWidthRangeChanged?.Invoke( this ); } finally { _leafBackWidthRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeafBackWidthRange"/> property value changes.</summary>
		//public event Action<PlantMaterial> LeafBackWidthRangeChanged;
		//ReferenceField<Range> _leafBackWidthRange = new Range( 0.5, 0.5 );

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( UVMode ):
					if( PartType.Value != PartTypeEnum.Bark )
						skip = true;
					break;

				case nameof( UVFrontPosition ):
					if( PartType.Value == PartTypeEnum.BranchWithLeaves || PartType.Value == PartTypeEnum.Leaf || PartType.Value == PartTypeEnum.Flower || PartType.Value == PartTypeEnum.Bark && UVMode.Value == UVModeEnum.Point )
					{
					}
					else
						skip = true;
					break;

				case nameof( UVFrontDirection ):
				case nameof( UVLengthRange ):
				case nameof( UVWidth ):
					if( PartType.Value != PartTypeEnum.BranchWithLeaves && PartType.Value != PartTypeEnum.Leaf )
						skip = true;
					break;

				case nameof( UVBack ):
					if( PartType.Value != PartTypeEnum.BranchWithLeaves && PartType.Value != PartTypeEnum.Leaf && PartType.Value != PartTypeEnum.Flower )
						skip = true;
					break;

				case nameof( UVRadius ):
					if( PartType.Value != PartTypeEnum.Flower )
						skip = true;
					break;

				case nameof( UVBackPosition ):
					if( PartType.Value != PartTypeEnum.BranchWithLeaves && PartType.Value != PartTypeEnum.Leaf && PartType.Value != PartTypeEnum.Flower || !UVBack )
						skip = true;
					break;

				case nameof( UVBackDirection ):
					if( PartType.Value != PartTypeEnum.BranchWithLeaves && PartType.Value != PartTypeEnum.Leaf || !UVBack )
						skip = true;
					break;
				}
			}
		}
	}
}
#endif