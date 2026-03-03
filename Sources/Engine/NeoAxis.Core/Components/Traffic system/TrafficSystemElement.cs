// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An element to specify participant of the traffic system.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Traffic System\Traffic System Element", 10585 )]
#endif
	public class TrafficSystemElement : Component
	{

		//!!!!обновлять при изменениях

		//!!!!может указывать возможные цвета. или материалы

		/// <summary>
		/// The object type of the element.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component> ObjectType
		{
			get { if( _objectType.BeginGet() ) ObjectType = _objectType.Get( this ); return _objectType.value; }
			set { if( _objectType.BeginSet( this, ref value ) ) { try { ObjectTypeChanged?.Invoke( this ); } finally { _objectType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectType"/> property value changes.</summary>
		public event Action<TrafficSystemElement> ObjectTypeChanged;
		ReferenceField<Component> _objectType = null;

		[Flags]
		public enum RolesEnum
		{
			None = 0,
			Ground = 1,
			Flying = 2,
		}

		/// <summary>
		/// The roles of the object type.
		/// </summary>
		[DefaultValue( RolesEnum.Ground )]
		public Reference<RolesEnum> Roles
		{
			get { if( _roles.BeginGet() ) Roles = _roles.Get( this ); return _roles.value; }
			set { if( _roles.BeginSet( this, ref value ) ) { try { RolesChanged?.Invoke( this ); } finally { _roles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Roles"/> property value changes.</summary>
		public event Action<TrafficSystemElement> RolesChanged;
		ReferenceField<RolesEnum> _roles = RolesEnum.Ground;

		/// <summary>
		/// The range of the flying height.
		/// </summary>
		[DefaultValue( "50 200" )]
		public Reference<Range> FlyingHeightRange
		{
			get { if( _flyingHeightRange.BeginGet() ) FlyingHeightRange = _flyingHeightRange.Get( this ); return _flyingHeightRange.value; }
			set { if( _flyingHeightRange.BeginSet( this, ref value ) ) { try { FlyingHeightRangeChanged?.Invoke( this ); } finally { _flyingHeightRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingHeightRange"/> property value changes.</summary>
		public event Action<TrafficSystemElement> FlyingHeightRangeChanged;
		ReferenceField<Range> _flyingHeightRange = new Range( 50, 200 );

		/// <summary>
		/// The speed of the flying height.
		/// </summary>
		[DefaultValue( "20 50" )]
		public Reference<Range> FlyingSpeedRange
		{
			get { if( _flyingSpeedRange.BeginGet() ) FlyingSpeedRange = _flyingSpeedRange.Get( this ); return _flyingSpeedRange.value; }
			set { if( _flyingSpeedRange.BeginSet( this, ref value ) ) { try { FlyingSpeedRangeChanged?.Invoke( this ); } finally { _flyingSpeedRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyingSpeedRange"/> property value changes.</summary>
		public event Action<TrafficSystemElement> FlyingSpeedRangeChanged;
		ReferenceField<Range> _flyingSpeedRange = new Range( 20, 50 );

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( FlyingHeightRange ):
				case nameof( FlyingSpeedRange ):
					if( ( Roles.Value & RolesEnum.Flying ) == 0 )
						skip = true;
					break;
				}
			}
		}
	}
}
