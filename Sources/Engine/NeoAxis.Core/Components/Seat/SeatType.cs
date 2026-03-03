// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the seat type.
	/// </summary>
	[ResourceFileExtension( "seattype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Seat\Seat Type", 10577 )]
	[EditorControl( typeof( SeatTypeEditor ) )]
	[Preview( typeof( SeatTypePreview ) )]
	[PreviewImage( typeof( SeatTypePreviewImage ) )]
#endif
	public class SeatType : Component
	{
		int version;

		//

		const string meshDefault = @"Content\Seats\Default\Data\scene.gltf|$Mesh";

		/// <summary>
		/// The main mesh of the seat.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Common" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<SeatType> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );

		/////////////////////////////////////////

		//protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		//{
		//	base.OnMetadataGetMembersFilter( context, member, ref skip );

		//	if( member is Metadata.Property )
		//	{
		//		switch( member.Name )
		//		{
		//		case nameof( TransmissionSwitchLatency ):
		//			if( !TransmissionAuto )
		//				skip = true;
		//			break;
		//		}
		//	}
		//}

		//!!!!not used
		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				var seat = CreateComponent<SeatItem>();
				seat.Name = "Seat Item";
				seat.ExitTransform = new Transform( new Vector3( 1, 0, 0 ), Quaternion.Identity, Vector3.One );
			}
		}
	}
}
