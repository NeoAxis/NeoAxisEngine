// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The modifier of the road.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Road\Road Modifier", 10520 )]
	public class RoadModifier : Component
	{
		//!!!!impl
		//[DefaultValue( "0 100000" )]
		//public Reference<Range> LengthRange
		//{
		//	get { if( _lengthRange.BeginGet() ) LengthRange = _lengthRange.Get( this ); return _lengthRange.value; }
		//	set { if( _lengthRange.BeginSet( this, ref value ) ) { try { LengthRangeChanged?.Invoke( this ); DataWasChanged(); } finally { _lengthRange.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LengthRange"/> property value changes.</summary>
		//public event Action<RoadModifier> LengthRangeChanged;
		//ReferenceField<Range> _lengthRange = new Range( 0, 100000 );

		[Flags]
		public enum PredefinedModifiersEnum
		{
			None = 0,

			ShoulderLeft = 1,
			ShoulderRight = 2,
			SidewalkLeft = 4,
			SidewalkRight = 8,

			//RoadsideLeft = 1,
			////RoadsideLeftFence = 2,
			//RoadsideRight = 4,
			////RoadsideRightFence = 8,

			//!!!!
			//Overpass = 16,
		}

		/// <summary>
		/// The list of predefined modifiers of the road.
		/// </summary>
		[DefaultValue( PredefinedModifiersEnum.None )]
		public Reference<PredefinedModifiersEnum> PredefinedModifiers
		{
			get { if( _predefinedModifiers.BeginGet() ) PredefinedModifiers = _predefinedModifiers.Get( this ); return _predefinedModifiers.value; }
			set { if( _predefinedModifiers.BeginSet( this, ref value ) ) { try { PredefinedModifiersChanged?.Invoke( this ); DataWasChanged(); } finally { _predefinedModifiers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PredefinedModifiers"/> property value changes.</summary>
		public event Action<RoadModifier> PredefinedModifiersChanged;
		ReferenceField<PredefinedModifiersEnum> _predefinedModifiers = PredefinedModifiersEnum.None;


		//!!!!OverrideRoadsideTypeLeft, OverrideRoadsideTypeRight, OverrideOverpassType


		//[DefaultValue( -1.0 )]
		//public Reference<double> OverpassSupportHeight
		//{
		//	get { if( _overpassSupportHeight.BeginGet() ) OverpassSupportHeight = _overpassSupportHeight.Get( this ); return _overpassSupportHeight.value; }
		//	set { if( _overpassSupportHeight.BeginSet( this, ref value ) ) { try { OverpassSupportHeightChanged?.Invoke( this ); DataWasChanged(); } finally { _overpassSupportHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportHeight"/> property value changes.</summary>
		//public event Action<RoadModifier> OverpassSupportHeightChanged;
		//ReferenceField<double> _overpassSupportHeight = -1.0;

		///////////////////////////////////////////////

		//protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		//{
		//	base.OnMetadataGetMembersFilter( context, member, ref skip );

		//	if( member is Metadata.Property )
		//	{
		//		switch( member.Name )
		//		{
		//		case nameof( OverpassSupportHeight ):
		//			if( !PredefinedModifiers.Value.HasFlag( PredefinedModifiersEnum.Overpass ) )
		//				skip = true;
		//			break;
		//		}
		//	}
		//}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			DataWasChanged();
		}

		public void DataWasChanged()
		{
			var obj = Parent as CurveInSpace;
			obj?.DataWasChanged();
		}
	}
}
