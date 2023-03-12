// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A road manager to control all roads in the scene.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Road\Road Manager", 10570 )]
	[SettingsCell( typeof( RoadManagerSettingsCell ) )]
	[WhenCreatingShowWarningIfItAlreadyExists]
#endif
	public class RoadManager : Component
	{
		/// <summary>
		/// Whether to create collision bodies.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( ref value ) ) { try { CollisionChanged?.Invoke( this ); RoadsNeedUpdate(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<RoadManager> CollisionChanged;
		ReferenceField<bool> _collision = true;

		/// <summary>
		/// Whether to display roads.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> Display
		{
			get { if( _display.BeginGet() ) Display = _display.Get( this ); return _display.value; }
			set { if( _display.BeginSet( ref value ) ) { try { DisplayChanged?.Invoke( this ); RoadsNeedUpdate(); } finally { _display.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Display"/> property value changes.</summary>
		public event Action<RoadManager> DisplayChanged;
		ReferenceField<bool> _display = true;

		/// <summary>
		/// Whether to display markup.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> DisplayMarkup
		{
			get { if( _displayMarkup.BeginGet() ) DisplayMarkup = _displayMarkup.Get( this ); return _displayMarkup.value; }
			set { if( _displayMarkup.BeginSet( ref value ) ) { try { DisplayMarkupChanged?.Invoke( this ); RoadsNeedUpdate(); } finally { _displayMarkup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayMarkup"/> property value changes.</summary>
		public event Action<RoadManager> DisplayMarkupChanged;
		ReferenceField<bool> _displayMarkup = true;


		//!!!!need?
		////!!!!default value
		///// <summary>
		///// The size of the sector in the scene. The sector size allows to optimize the culling and rendering of objects.
		///// </summary>
		//[DefaultValue( "200 200 10000" )]
		//[Category( "Optimization" )]
		//public Reference<Vector3> SectorSize
		//{
		//	get { if( _sectorSize.BeginGet() ) SectorSize = _sectorSize.Get( this ); return _sectorSize.value; }
		//	set
		//	{
		//		var v = value.Value;
		//		if( v.X < 1.0 || v.Y < 1.0 || v.Z < 1.0 )
		//		{
		//			if( v.X < 1.0 ) v.X = 1.0;
		//			if( v.Y < 1.0 ) v.Y = 1.0;
		//			if( v.Z < 1.0 ) v.Z = 1.0;
		//			value = new Reference<Vector3>( v, value.GetByReference );
		//		}
		//		if( _sectorSize.BeginSet( ref value ) ) { try { SectorSizeChanged?.Invoke( this ); RoadsNeedUpdate();/*UpdateGroupOfObjects();*/ } finally { _sectorSize.EndSet(); } }
		//	}
		//}
		///// <summary>Occurs when the <see cref="SectorSize"/> property value changes.</summary>
		//public event Action<RoadManager> SectorSizeChanged;
		//ReferenceField<Vector3> _sectorSize = new Vector3( 200, 200, 10000 );

		//!!!!need?
		////!!!!default
		///// <summary>
		///// The maximal amount of objects in one group/batch.
		///// </summary>
		//[DefaultValue( 100000 )]
		//[Category( "Optimization" )]
		//public Reference<int> MaxObjectsInGroup
		//{
		//	get { if( _maxObjectsInGroup.BeginGet() ) MaxObjectsInGroup = _maxObjectsInGroup.Get( this ); return _maxObjectsInGroup.value; }
		//	set { if( _maxObjectsInGroup.BeginSet( ref value ) ) { try { MaxObjectsInGroupChanged?.Invoke( this ); RoadsNeedUpdate();/*UpdateGroupOfObjects();*/ } finally { _maxObjectsInGroup.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MaxObjectsInGroup"/> property value changes.</summary>
		//public event Action<RoadManager> MaxObjectsInGroupChanged;
		//ReferenceField<int> _maxObjectsInGroup = 100000;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( DisplayMarkup ):
					if( !Display )
						skip = true;
					break;
				}
			}
		}

		void RoadsNeedUpdate()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			foreach( var road in scene.GetComponents<Road>( checkChildren: true ) )
				road.NeedUpdateLogicalData();
			foreach( var roadNode in scene.GetComponents<RoadNode>( checkChildren: true ) )
				roadNode.NeedUpdate();
		}

		//void UpdateGroupOfObjects()
		//{
		//	var scene = FindParent<Scene>();
		//	if( scene == null )
		//		return;

		//	var name = "__GroupOfObjectsRoads";

		//	var group = scene.GetComponent<GroupOfObjects>( name );
		//	if( group == null )
		//		return;

		//	Road.UpdateGroupOfObjects( scene, group );
		//}

	}
}
