// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The info about structure of a building.
	/// </summary>
	[ResourceFileExtension( "buildingtype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Building\Building Type", 300 )]
	//!!!!
	//[EditorControl( typeof( BuildingTypeEditor ) )]
	//[Preview( typeof( BuildingTypePreview ) )]
	//[PreviewImage( typeof( BuildingTypePreviewImage ) )]
#endif
	public class BuildingType : Component
	{
		int version;

		/////////////////////////////////////////

		/// <summary>
		/// The structure type of the building.
		/// </summary>
		[DefaultValue( StructureEnum.Basic )]
		public Reference<StructureEnum> Structure
		{
			get { if( _structure.BeginGet() ) Structure = _structure.Get( this ); return _structure.value; }
			set { if( _structure.BeginSet( ref value ) ) { try { StructureChanged?.Invoke( this ); DataWasChanged(); } finally { _structure.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Structure"/> property value changes.</summary>
		public event Action<BuildingType> StructureChanged;
		ReferenceField<StructureEnum> _structure = StructureEnum.Basic;

		/// <summary>
		/// The size of the element.
		/// </summary>
		[DefaultValue( "6 3" )]
		public Reference<Vector2> ElementSize
		{
			get { if( _elementSize.BeginGet() ) ElementSize = _elementSize.Get( this ); return _elementSize.value; }
			set { if( _elementSize.BeginSet( ref value ) ) { try { ElementSizeChanged?.Invoke( this ); DataWasChanged(); } finally { _elementSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ElementSize"/> property value changes.</summary>
		public event Action<BuildingType> ElementSizeChanged;
		ReferenceField<Vector2> _elementSize = new Vector2( 6, 3 );

		/////////////////////////////////////////

		public enum StructureEnum
		{
			Basic,
			Custom,
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//	switch( member.Name )
			//	{
			//	case nameof( SurfaceCollisionFriction ):
			//		if( SurfaceCollisionMaterial.Value != null )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		//!!!!use
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
	}
}
