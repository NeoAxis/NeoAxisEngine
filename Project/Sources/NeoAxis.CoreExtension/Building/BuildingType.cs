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
			set { if( _structure.BeginSet( this, ref value ) ) { try { StructureChanged?.Invoke( this ); DataWasChanged(); } finally { _structure.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Structure"/> property value changes.</summary>
		public event Action<BuildingType> StructureChanged;
		ReferenceField<StructureEnum> _structure = StructureEnum.Basic;

		/// <summary>
		/// The default size of the element, horizontal width and height.
		/// </summary>
		[DefaultValue( "6 3" )]
		public Reference<Vector2> ElementSize
		{
			get { if( _elementSize.BeginGet() ) ElementSize = _elementSize.Get( this ); return _elementSize.value; }
			set { if( _elementSize.BeginSet( this, ref value ) ) { try { ElementSizeChanged?.Invoke( this ); DataWasChanged(); } finally { _elementSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ElementSize"/> property value changes.</summary>
		public event Action<BuildingType> ElementSizeChanged;
		ReferenceField<Vector2> _elementSize = new Vector2( 6, 3 );

		/// <summary>
		/// The size of shrinking the volume of occluder by horizontal axes.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 20 )]
		public Reference<double> OccluderShrinkHorizontal
		{
			get { if( _occluderShrinkHorizontal.BeginGet() ) OccluderShrinkHorizontal = _occluderShrinkHorizontal.Get( this ); return _occluderShrinkHorizontal.value; }
			set { if( _occluderShrinkHorizontal.BeginSet( this, ref value ) ) { try { OccluderShrinkHorizontalChanged?.Invoke( this ); } finally { _occluderShrinkHorizontal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OccluderShrinkHorizontal"/> property value changes.</summary>
		public event Action<BuildingType> OccluderShrinkHorizontalChanged;
		ReferenceField<double> _occluderShrinkHorizontal = 0.0;

		/// <summary>
		/// The additional height of the occluder at bottom.
		/// </summary>
		[DefaultValue( 5 )]
		[Range( 0, 20 )]
		public Reference<double> OccluderExtendBottom
		{
			get { if( _occluderExtendBottom.BeginGet() ) OccluderExtendBottom = _occluderExtendBottom.Get( this ); return _occluderExtendBottom.value; }
			set { if( _occluderExtendBottom.BeginSet( this, ref value ) ) { try { OccluderExtendBottomChanged?.Invoke( this ); DataWasChanged(); } finally { _occluderExtendBottom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OccluderExtendBottom"/> property value changes.</summary>
		public event Action<BuildingType> OccluderExtendBottomChanged;
		ReferenceField<double> _occluderExtendBottom = 5;

		///// <summary>
		///// The size of extending of the occluder.
		///// </summary>
		//[DefaultValue( "-1 -1 4 -1 -1 0" )]
		//public Reference<Bounds> OccluderExtend
		//{
		//	get { if( _occluderExtend.BeginGet() ) OccluderExtend = _occluderExtend.Get( this ); return _occluderExtend.value; }
		//	set
		//	{
		//		if( _occluderExtend.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				OccluderExtendChanged?.Invoke( this );
		//				occluderCachedTransform = null;
		//			}
		//			finally { _occluderExtend.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="OccluderExtend"/> property value changes.</summary>
		//public event Action<Building> OccluderExtendChanged;
		//ReferenceField<Bounds> _occluderExtend = new Bounds( -1, -1, 4, -1, -1, 0 );

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
