// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A spawn point helper.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Spawn Point", -4000 )]
	[NewObjectDefaultName( "Spawn Point" )]
	public class SpawnPoint : ObjectInSpace
	{
		/// <summary>
		/// The mode of the spawn point. In the Player mode, it is used to spawn players. In the Component mode, it is used to spawn components of a specified type.
		/// </summary>
		[DefaultValue( ModeEnum.Player )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( this, ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<SpawnPoint> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Player;

		/// <summary>
		/// The component type to spawn in the Component mode.
		/// </summary>
		[DefaultValueReference( "NeoAxis.Weapon" )]
		public Reference<Metadata.TypeInfo> ComponentType
		{
			get { if( _componentType.BeginGet() ) ComponentType = _componentType.Get( this ); return _componentType.value; }
			set { if( _componentType.BeginSet( this, ref value ) ) { try { ComponentTypeChanged?.Invoke( this ); } finally { _componentType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ComponentType"/> property value changes.</summary>
		public event Action<SpawnPoint> ComponentTypeChanged;
		ReferenceField<Metadata.TypeInfo> _componentType = new Reference<Metadata.TypeInfo>( null, "NeoAxis.Weapon" );

		/// <summary>
		/// The time in seconds after the last spawn when the next spawn can occur. Only for the Component mode.
		/// </summary>
		[DefaultValue( 10000000.0 )]
		public Reference<double> SpawnTime
		{
			get { if( _spawnTime.BeginGet() ) SpawnTime = _spawnTime.Get( this ); return _spawnTime.value; }
			set { if( _spawnTime.BeginSet( this, ref value ) ) { try { SpawnTimeChanged?.Invoke( this ); } finally { _spawnTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpawnTime"/> property value changes.</summary>
		public event Action<SpawnPoint> SpawnTimeChanged;
		ReferenceField<double> _spawnTime = 10000000.0;

		/// <summary>
		/// The distance to detect the previous component. If the previous component is within this distance, the next spawn will not occur. Only for the Component mode.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0.1, 2.0 )]
		public Reference<double> DistanceToDetectPreviousComponent
		{
			get { if( _distanceToDetectPreviousComponent.BeginGet() ) DistanceToDetectPreviousComponent = _distanceToDetectPreviousComponent.Get( this ); return _distanceToDetectPreviousComponent.value; }
			set { if( _distanceToDetectPreviousComponent.BeginSet( this, ref value ) ) { try { DistanceToDetectPreviousComponentChanged?.Invoke( this ); } finally { _distanceToDetectPreviousComponent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DistanceToDetectPreviousComponent"/> property value changes.</summary>
		public event Action<SpawnPoint> DistanceToDetectPreviousComponentChanged;
		ReferenceField<double> _distanceToDetectPreviousComponent = 0.5;

		/// <summary>
		/// The number of the team.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( this, ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<SpawnPoint> TeamChanged;
		ReferenceField<int> _team = 0;

		///// <summary>
		///// The number of the team.
		///// </summary>
		//[DefaultValue( TeamEnum.None )]
		//public Reference<TeamEnum> Team
		//{
		//	get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
		//	set { if( _team.BeginSet( this, ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		//public event Action<SpawnPoint> TeamChanged;
		//ReferenceField<TeamEnum> _team = TeamEnum.None;

		/// <summary>
		/// A string property to store any data.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> AnyText
		{
			get { if( _anyText.BeginGet() ) AnyText = _anyText.Get( this ); return _anyText.value; }
			set { if( _anyText.BeginSet( this, ref value ) ) { try { AnyTextChanged?.Invoke( this ); } finally { _anyText.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AnyText"/> property value changes.</summary>
		public event Action<SpawnPoint> AnyTextChanged;
		ReferenceField<string> _anyText = "";

		///////////////////////////////////////////////

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( false )]
		public Reference<Component> LastCreatedComponent
		{
			get { if( _lastCreatedComponent.BeginGet() ) LastCreatedComponent = _lastCreatedComponent.Get( this ); return _lastCreatedComponent.value; }
			set { if( _lastCreatedComponent.BeginSet( this, ref value ) ) { try { LastCreatedComponentChanged?.Invoke( this ); } finally { _lastCreatedComponent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LastCreatedComponent"/> property value changes.</summary>
		public event Action<SpawnPoint> LastCreatedComponentChanged;
		ReferenceField<Component> _lastCreatedComponent;

		[Browsable( false )]
		[Serialize( SerializeType.Enable )]
		[NetworkSynchronize( false )]
		public Reference<double> LastExistTime
		{
			get { if( _lastExistTime.BeginGet() ) LastExistTime = _lastExistTime.Get( this ); return _lastExistTime.value; }
			set { if( _lastExistTime.BeginSet( this, ref value ) ) { try { LastExistTimeChanged?.Invoke( this ); } finally { _lastExistTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LastExistTime"/> property value changes.</summary>
		public event Action<SpawnPoint> LastExistTimeChanged;
		ReferenceField<double> _lastExistTime = 0.0;

		///////////////////////////////////////////////

		public enum ModeEnum
		{
			Player,
			Component,
		}

		///////////////////////////////////////////////

		//public enum TeamEnum
		//{
		//	None,
		//	[DisplayNameEnum( "Team 1" )]
		//	Team1,
		//	[DisplayNameEnum( "Team 2" )]
		//	Team2,
		//	//[DisplayNameEnum( "Team 3" )]
		//	//Team3,
		//	//[DisplayNameEnum( "Team 4" )]
		//	//Team4,
		//}

		///////////////////////////////////////////////

		public delegate void IsAllowToCreateComponentDelegate( SpawnPoint sender, ref bool allow );
		public event IsAllowToCreateComponentDelegate IsAllowToCreateComponent;

		public delegate void ComponentCreatingDelegate( SpawnPoint sender, Component component );
		public event ComponentCreatingDelegate ComponentCreating;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( ComponentType ):
				case nameof( SpawnTime ):
				case nameof( DistanceToDetectPreviousComponent ):
					if( Mode.Value != ModeEnum.Component )
						skip = true;
					break;
				}
			}
		}

		protected virtual void OnIsAllowToCreateComponent( ref bool allow ) { }
		protected virtual void OnComponentCreating( Component obj ) { }

		protected virtual bool IsExistPreviousCreatedComponent()
		{
			var previousObject = LastCreatedComponent.Value;
			if( previousObject != null && previousObject.EnabledInHierarchy )
			{
				var previousObjectInSpace = previousObject as ObjectInSpace;
				if( previousObjectInSpace != null )
				{
					var trPosition = TransformV.Position;

					//check by position
					var distanceSquared = ( previousObjectInSpace.TransformV.Position - trPosition ).LengthSquared();
					if( distanceSquared < DistanceToDetectPreviousComponent * DistanceToDetectPreviousComponent )
						return true;

					////check by bounding box
					//if( previousObjectInSpace.SpaceBounds.BoundingBox.Contains( trPosition ) )
					//	return true;
				}
			}

			return false;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( Mode.Value == ModeEnum.Component )
			{
				var scene = ParentScene;
				if( scene != null )
				{
					var type = ComponentType.Value;
					if( type != null )
					{
						var exists = IsExistPreviousCreatedComponent();
						if( exists )
							LastExistTime = EngineApp.EngineTime;
						else
						{
							if( EngineApp.EngineTime - LastExistTime > SpawnTime || LastExistTime == 0 )
							{
								var allow = true;
								OnIsAllowToCreateComponent( ref allow );
								IsAllowToCreateComponent?.Invoke( this, ref allow );

								if( allow )
								{
									var obj = scene.CreateComponent( type, enabled: false, setUniqueName: true );
									obj.NewObjectSetDefaultConfiguration();

									var objectInSpace = obj as ObjectInSpace;
									if( objectInSpace != null )
										objectInSpace.Transform = Transform;

									OnComponentCreating( obj );
									ComponentCreating?.Invoke( this, obj );

									obj.Enabled = true;

									LastExistTime = EngineApp.EngineTime;
									LastCreatedComponent = obj;
								}
							}
						}
					}
				}
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//draw selection
			if( EngineApp.IsEditor )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else
						color = ProjectSettings.Get.Colors.CanSelectColor;

					var viewport = context.Owner;

					var renderer = viewport.Simple3DRenderer;
					renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					var tr = TransformV;
					renderer.AddArrow( tr.Position, tr.Position + tr.Rotation.GetForward() * tr.Scale.MaxComponent(), 0, 0, true, 0 );
				}
			}
		}

		public void ResetLastSpawnData()
		{
			LastExistTime = 0;
			LastCreatedComponent = null;
		}
	}
}