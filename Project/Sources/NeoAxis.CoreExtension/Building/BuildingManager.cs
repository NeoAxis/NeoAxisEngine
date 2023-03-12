// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A building manager to control all buildings on the scene.
	/// </summary>
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Building\Building Manager", 350 )]
	[SettingsCell( typeof( BuildingManagerSettingsCell ) )]
	[WhenCreatingShowWarningIfItAlreadyExists]
#endif
	public class BuildingManager : Component
	{
		/// <summary>
		/// Whether to create a collision body of buildings.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( ref value ) ) { try { CollisionChanged?.Invoke( this ); BuildingsNeedUpdate(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<BuildingManager> CollisionChanged;
		ReferenceField<bool> _collision = true;

		/// <summary>
		/// Whether to display buildings.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> Display
		{
			get { if( _display.BeginGet() ) Display = _display.Get( this ); return _display.value; }
			set { if( _display.BeginSet( ref value ) ) { try { DisplayChanged?.Invoke( this ); BuildingsNeedUpdate(); } finally { _display.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Display"/> property value changes.</summary>
		public event Action<BuildingManager> DisplayChanged;
		ReferenceField<bool> _display = true;

		/// <summary>
		/// Whether to display facade of buildings.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> DisplayFacade
		{
			get { if( _displayFacade.BeginGet() ) DisplayFacade = _displayFacade.Get( this ); return _displayFacade.value; }
			set { if( _displayFacade.BeginSet( ref value ) ) { try { DisplayFacadeChanged?.Invoke( this ); BuildingsNeedUpdate(); } finally { _displayFacade.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayFacade"/> property value changes.</summary>
		public event Action<BuildingManager> DisplayFacadeChanged;
		ReferenceField<bool> _displayFacade = true;

		/// <summary>
		/// Whether to display cells of buildings.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> DisplayCells
		{
			get { if( _displayCells.BeginGet() ) DisplayCells = _displayCells.Get( this ); return _displayCells.value; }
			set { if( _displayCells.BeginSet( ref value ) ) { try { DisplayCellsChanged?.Invoke( this ); BuildingsNeedUpdate(); } finally { _displayCells.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayCells"/> property value changes.</summary>
		public event Action<BuildingManager> DisplayCellsChanged;
		ReferenceField<bool> _displayCells = true;

		/// <summary>
		/// Whether to display surrounding objects of buildings.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Display" )]
		public Reference<bool> DisplaySurrounding
		{
			get { if( _displaySurrounding.BeginGet() ) DisplaySurrounding = _displaySurrounding.Get( this ); return _displaySurrounding.value; }
			set { if( _displaySurrounding.BeginSet( ref value ) ) { try { DisplaySurroundingChanged?.Invoke( this ); BuildingsNeedUpdate(); } finally { _displaySurrounding.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplaySurrounding"/> property value changes.</summary>
		public event Action<BuildingManager> DisplaySurroundingChanged;
		ReferenceField<bool> _displaySurrounding = true;

		/// <summary>
		/// The size of the sector in the scene. The sector size allows to optimize the culling and rendering of objects.
		/// </summary>
		[DefaultValue( "150 150 10000" )]
		[Category( "Optimization" )]
		public Reference<Vector3> SectorSize
		{
			get { if( _sectorSize.BeginGet() ) SectorSize = _sectorSize.Get( this ); return _sectorSize.value; }
			set
			{
				var v = value.Value;
				if( v.X < 1.0 || v.Y < 1.0 || v.Z < 1.0 )
				{
					if( v.X < 1.0 ) v.X = 1.0;
					if( v.Y < 1.0 ) v.Y = 1.0;
					if( v.Z < 1.0 ) v.Z = 1.0;
					value = new Reference<Vector3>( v, value.GetByReference );
				}
				if( _sectorSize.BeginSet( ref value ) ) { try { SectorSizeChanged?.Invoke( this ); UpdateGroupOfObjects(); } finally { _sectorSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="SectorSize"/> property value changes.</summary>
		public event Action<BuildingManager> SectorSizeChanged;
		ReferenceField<Vector3> _sectorSize = new Vector3( 150, 150, 10000 );

		//!!!!default
		/// <summary>
		/// The maximal amount of objects in one group/batch.
		/// </summary>
		[DefaultValue( 100000 )]
		[Category( "Optimization" )]
		public Reference<int> MaxObjectsInGroup
		{
			get { if( _maxObjectsInGroup.BeginGet() ) MaxObjectsInGroup = _maxObjectsInGroup.Get( this ); return _maxObjectsInGroup.value; }
			set { if( _maxObjectsInGroup.BeginSet( ref value ) ) { try { MaxObjectsInGroupChanged?.Invoke( this ); UpdateGroupOfObjects(); } finally { _maxObjectsInGroup.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxObjectsInGroup"/> property value changes.</summary>
		public event Action<BuildingManager> MaxObjectsInGroupChanged;
		ReferenceField<int> _maxObjectsInGroup = 100000;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( DisplayFacade ):
				case nameof( DisplayCells ):
				case nameof( DisplaySurrounding ):
					if( !Display )
						skip = true;
					break;
				}
			}
		}

		void BuildingsNeedUpdate()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			foreach( var building in scene.GetComponents<Building>( checkChildren: true ) )
				building.NeedUpdate();
		}

		void UpdateGroupOfObjects()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			var name = "__GroupOfObjectsBuildings";

			var group = scene.GetComponent<GroupOfObjects>( name );
			if( group == null )
				return;

			Building.UpdateGroupOfObjects( scene, group );
		}
	}
}
