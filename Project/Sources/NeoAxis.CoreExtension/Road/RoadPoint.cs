// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a curve point of the road.
	/// </summary>
	public class RoadPoint : CurveInSpacePoint
	{
		/// <summary>
		/// The modifiers of the point.
		/// </summary>
		[DefaultValue( ModifiersEnum.None )]
		public Reference<ModifiersEnum> Modifiers
		{
			get { if( _modifiers.BeginGet() ) Modifiers = _modifiers.Get( this ); return _modifiers.value; }
			set { if( _modifiers.BeginSet( this, ref value ) ) { try { ModifiersChanged?.Invoke( this ); DataWasChanged(); } finally { _modifiers.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Modifiers"/> property value changes.</summary>
		public event Action<RoadPoint> ModifiersChanged;
		ReferenceField<ModifiersEnum> _modifiers = ModifiersEnum.None;

		//!!!!
		////!!!!нужна поддержка null значений. или еще одно свойство добавить
		//[DefaultValue( -1.0 )]
		//public Reference<double> OverpassSupportHeight
		//{
		//	get { if( _overpassSupportHeight.BeginGet() ) OverpassSupportHeight = _overpassSupportHeight.Get( this ); return _overpassSupportHeight.value; }
		//	set { if( _overpassSupportHeight.BeginSet( this, ref value ) ) { try { OverpassSupportHeightChanged?.Invoke( this ); DataWasChanged(); } finally { _overpassSupportHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OverpassSupportHeight"/> property value changes.</summary>
		//public event Action<RoadPoint> OverpassSupportHeightChanged;
		//ReferenceField<double> _overpassSupportHeight = -1.0;

		///////////////////////////////////////////////

		[Flags]
		public enum ModifiersEnum
		{
			None = 0,
			//OverpassSupport = 1,
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//	switch( member.Name )
			//	{
			//	case nameof( OverpassSupportHeight ):
			//		if( !Modifiers.Value.HasFlag( ModifiersEnum.OverpassSupport ) )
			//			skip = true;
			//		break;
			//	}
			//}
		}
	}
}
