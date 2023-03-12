// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The modifier of the road lane.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Road\Road Lane Modifier", 10530 )]
	public class RoadLaneModifier : Component
	{
		/// <summary>
		/// The list of affected lanes.
		/// </summary>
		//!!!!impl
		[DefaultValue( LanesEnum.Lane1 )]
		public Reference<LanesEnum> Lanes
		{
			get { if( _lanes.BeginGet() ) Lanes = _lanes.Get( this ); return _lanes.value; }
			set { if( _lanes.BeginSet( ref value ) ) { try { LanesChanged?.Invoke( this ); } finally { _lanes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Lanes"/> property value changes.</summary>
		public event Action<RoadLaneModifier> LanesChanged;
		ReferenceField<LanesEnum> _lanes = LanesEnum.Lane1;

		//!!!!impl
		/// <summary>
		/// The affected length interval of lanes.
		/// </summary>
		[DefaultValue( "0 100000" )]
		public Reference<Range> LengthRange
		{
			get { if( _lengthRange.BeginGet() ) LengthRange = _lengthRange.Get( this ); return _lengthRange.value; }
			set { if( _lengthRange.BeginSet( ref value ) ) { try { LengthRangeChanged?.Invoke( this ); DataWasChanged(); } finally { _lengthRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LengthRange"/> property value changes.</summary>
		public event Action<RoadLaneModifier> LengthRangeChanged;
		ReferenceField<Range> _lengthRange = new Range( 0, 100000 );

		[Flags]
		public enum PredefinedModifiersEnum
		{
			None = 0,
			NotExist = 1,
		}

		/// <summary>
		/// The list of predefined modifiers of lanes.
		/// </summary>
		[DefaultValue( PredefinedModifiersEnum.None )]
		public Reference<PredefinedModifiersEnum> PredefinedModifiers
		{
			get { if( _predefinedModifiers.BeginGet() ) PredefinedModifiers = _predefinedModifiers.Get( this ); return _predefinedModifiers.value; }
			set { if( _predefinedModifiers.BeginSet( ref value ) ) { try { PredefinedModifiersChanged?.Invoke( this ); DataWasChanged(); } finally { _predefinedModifiers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PredefinedModifiers"/> property value changes.</summary>
		public event Action<RoadLaneModifier> PredefinedModifiersChanged;
		ReferenceField<PredefinedModifiersEnum> _predefinedModifiers = PredefinedModifiersEnum.None;

		///////////////////////////////////////////////

		[Flags]
		public enum LanesEnum
		{
			None = 0,
			Lane1 = 1,
			Lane2 = 2,
			Lane3 = 4,
			Lane4 = 8,
			Lane5 = 16,
			Lane6 = 32,
			Lane7 = 64,
			Lane8 = 128,
			Lane9 = 256,
			Lane10 = 512,
			Lane11 = 1024,
			Lane12 = 2048,
			Lane13 = 4096,
			Lane14 = 8192,
			Lane15 = 16384,
			Lane16 = 32768,
			Lane17 = 65536,
			Lane18 = 131072,
			Lane19 = 262144,
			Lane20 = 524288,
			AllLanes = 0x7FFFFFFF,
		}

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
