// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Base class for making weapons.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Weapon", -6000 )]
	[NewObjectDefaultName( "Weapon" )]
	public class Component_Weapon : Component_MeshInSpace, IComponent_Item, IComponent_InteractiveObject
	{
		bool firing;
		double firingCurrentTime;

		/////////////////////////////////////////

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> FiringTotalTime
		{
			get { if( _firingTotalTime.BeginGet() ) FiringTotalTime = _firingTotalTime.Get( this ); return _firingTotalTime.value; }
			set { if( _firingTotalTime.BeginSet( ref value ) ) { try { FiringTotalTimeChanged?.Invoke( this ); } finally { _firingTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringTotalTime"/> property value changes.</summary>
		public event Action<Component_Weapon> FiringTotalTimeChanged;
		ReferenceField<double> _firingTotalTime = 1.0;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> FiringFireTime
		{
			get { if( _firingFireTime.BeginGet() ) FiringFireTime = _firingFireTime.Get( this ); return _firingFireTime.value; }
			set { if( _firingFireTime.BeginSet( ref value ) ) { try { FiringFireTimeChanged?.Invoke( this ); } finally { _firingFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringFireTime"/> property value changes.</summary>
		public event Action<Component_Weapon> FiringFireTimeChanged;
		ReferenceField<double> _firingFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Samples\Starter Content\Scene objects\Default Bullet\Default bullet type.scene|$Bullet" )]
		public Reference<Metadata.TypeInfo> BulletType
		{
			get { if( _bulletType.BeginGet() ) BulletType = _bulletType.Get( this ); return _bulletType.value; }
			set { if( _bulletType.BeginSet( ref value ) ) { try { BulletTypeChanged?.Invoke( this ); } finally { _bulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletType"/> property value changes.</summary>
		public event Action<Component_Weapon> BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _bulletType = new Reference<Metadata.TypeInfo>( null, @"Samples\Starter Content\Scene objects\Default Bullet\Default bullet type.scene|$Bullet" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.5 0 0; 0 0 0 1; 1 1 1" )]
		public Reference<Transform> BulletTransform
		{
			get { if( _bulletTransform.BeginGet() ) BulletTransform = _bulletTransform.Get( this ); return _bulletTransform.value; }
			set
			{
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _bulletTransform.BeginSet( ref value ) ) { try { BulletTransformChanged?.Invoke( this ); } finally { _bulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="BulletTransform"/> property value changes.</summary>
		public event Action<Component_Weapon> BulletTransformChanged;
		ReferenceField<Transform> _bulletTransform = new Transform( new Vector3( 0.5, 0, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 10.0 )]
		public Reference<double> BulletSpeed
		{
			get { if( _bulletSpeed.BeginGet() ) BulletSpeed = _bulletSpeed.Get( this ); return _bulletSpeed.value; }
			set { if( _bulletSpeed.BeginSet( ref value ) ) { try { BulletSpeedChanged?.Invoke( this ); } finally { _bulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletSpeed"/> property value changes.</summary>
		public event Action<Component_Weapon> BulletSpeedChanged;
		ReferenceField<double> _bulletSpeed = 10.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Samples\Starter Content\Sounds\Fire.ogg" )]
		public Reference<Component_Sound> SoundFiringBegin
		{
			get { if( _soundFiringBegin.BeginGet() ) SoundFiringBegin = _soundFiringBegin.Get( this ); return _soundFiringBegin.value; }
			set { if( _soundFiringBegin.BeginSet( ref value ) ) { try { SoundFiringBeginChanged?.Invoke( this ); } finally { _soundFiringBegin.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFiringBegin"/> property value changes.</summary>
		public event Action<Component_Weapon> SoundFiringBeginChanged;
		ReferenceField<Component_Sound> _soundFiringBegin = new Reference<Component_Sound>( null, @"Samples\Starter Content\Sounds\Fire.ogg" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Sound> SoundFire
		{
			get { if( _soundFire.BeginGet() ) SoundFire = _soundFire.Get( this ); return _soundFire.value; }
			set { if( _soundFire.BeginSet( ref value ) ) { try { SoundFireChanged?.Invoke( this ); } finally { _soundFire.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFire"/> property value changes.</summary>
		public event Action<Component_Weapon> SoundFireChanged;
		ReferenceField<Component_Sound> _soundFire = null;

		/// <summary>
		///The sound that is played when the firing ends.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Sound> SoundFiringEnd
		{
			get { if( _soundFiringEnd.BeginGet() ) SoundFiringEnd = _soundFiringEnd.Get( this ); return _soundFiringEnd.value; }
			set { if( _soundFiringEnd.BeginSet( ref value ) ) { try { SoundFiringEndChanged?.Invoke( this ); } finally { _soundFiringEnd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFiringEnd"/> property value changes.</summary>
		public event Action<Component_Weapon> SoundFiringEndChanged;
		ReferenceField<Component_Sound> _soundFiringEnd = null;

		/// <summary>
		/// Whether to display initial position and direction of a bullet.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AlwaysDisplayBulletInitialPosition
		{
			get { if( _alwaysDisplayBulletInitialPosition.BeginGet() ) AlwaysDisplayBulletInitialPosition = _alwaysDisplayBulletInitialPosition.Get( this ); return _alwaysDisplayBulletInitialPosition.value; }
			set { if( _alwaysDisplayBulletInitialPosition.BeginSet( ref value ) ) { try { AlwaysDisplayBulletInitialPositionChanged?.Invoke( this ); } finally { _alwaysDisplayBulletInitialPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayBulletInitialPosition"/> property value changes.</summary>
		public event Action<Component_Weapon> AlwaysDisplayBulletInitialPositionChanged;
		ReferenceField<bool> _alwaysDisplayBulletInitialPosition = false;

		/////////////////////////////////////////

		[Browsable( false )]
		public bool Firing
		{
			get { return firing; }
		}

		[Browsable( false )]
		public double FiringCurrentTime
		{
			get { return firingCurrentTime; }
		}

		//

		protected virtual void OnCanFire( ref bool canFire )
		{
			if( BulletType.Value == null )
				canFire = false;
		}

		public delegate void CanFireDelegate( Component_Weapon sender, ref bool canFire );
		public event CanFireDelegate CanFire;

		public bool PerformCanFire()
		{
			var canFire = true;
			OnCanFire( ref canFire );
			CanFire?.Invoke( this, ref canFire );
			return canFire;
		}

		//

		protected virtual void OnFire()
		{
			var bulletType = BulletType.Value;
			if( bulletType != null )
			{
				var bullet = Parent.CreateComponent( bulletType, enabled: false, setUniqueName: true );

				var bullet2 = bullet as Component_ObjectInSpace;
				if( bullet2 != null )
				{
					var initialTransform = GetBulletWorldInitialTransform();

					var collisionBody = bullet2.GetComponent( "Collision Body" ) as Component_RigidBody;
					if( collisionBody != null )
					{
						collisionBody.Transform = initialTransform;
						collisionBody.LinearVelocity = initialTransform.Rotation.GetForward() * BulletSpeed;
					}
					else
						bullet2.Transform = initialTransform;

					//interface IComponent_Bullet
					//{
					//	void BulletCreate( Component creator )
					//}

				}

				bullet.Enabled = true;
			}
		}

		public delegate void FireDelegate( Component_Weapon sender );
		public event FireDelegate Fire;

		public bool PerformFire()
		{
			if( !PerformCanFire() )
				return false;

			SoundPlay( SoundFire );
			OnFire();
			Fire?.Invoke( this );

			return true;
		}

		//

		public event Action<Component_Weapon> FiringBeginEvent;
		public event Action<Component_Weapon> FiringEndEvent;

		public bool FiringBegin()
		{
			if( Firing )
				return false;
			if( !PerformCanFire() )
				return false;

			firing = true;
			firingCurrentTime = 0;
			SoundPlay( SoundFiringBegin );
			FiringBeginEvent?.Invoke( this );
			return true;
		}

		public void FiringEnd()
		{
			if( !Firing )
				return;

			firing = false;
			firingCurrentTime = 0;

			SoundPlay( SoundFiringEnd );

			FiringEndEvent?.Invoke( this );
		}

		//

		void Simulate( float delta )
		{
			if( Firing )
			{
				var before = firingCurrentTime < FiringFireTime || ( firingCurrentTime == 0 && FiringFireTime == 0 );
				firingCurrentTime += delta;
				var after = firingCurrentTime < FiringFireTime;

				if( before != after )
					PerformFire();

				if( firingCurrentTime >= FiringTotalTime )
					FiringEnd();
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				//Mesh
				{
					var mesh = CreateComponent<Component_Mesh>();
					mesh.Name = "Mesh";
					Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<Component_MeshGeometry_Pipe>();
					geometry.Name = "Mesh Geometry";
					geometry.Axis = 0;
					geometry.Radius = 0.04;
					geometry.Thickness = 0.01;
					geometry.Material = ReferenceUtility.MakeReference( "Base\\Materials\\White.material" );
				}

				//Stock
				{
					var meshInSpace = CreateComponent<Component_MeshInSpace>();
					meshInSpace.Name = "Stock";

					var mesh = meshInSpace.CreateComponent<Component_Mesh>();
					mesh.Name = "Mesh";
					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( this, mesh );

					var geometry = mesh.CreateComponent<Component_MeshGeometry_Box>();
					geometry.Name = "Mesh Geometry";
					geometry.Dimensions = new Vector3( 0.4, 0.05, 0.15 );
					geometry.Material = ReferenceUtility.MakeReference( "Base\\Materials\\White.material" );

					meshInSpace.Mesh = ReferenceUtility.MakeThisReference( meshInSpace, mesh );

					var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
					transformOffset.Name = "Attach Transform Offset";
					transformOffset.PositionOffset = new Vector3( -0.22, 0, -0.1 );
					transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );

					meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
				}
			}
		}

		public void SoundPlay( Component_Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public void ObjectInteractionGetInfo( UIControl playScreen, Component_GameMode gameMode, ref IComponent_InteractiveObject_ObjectInfo info )
		{
			//take by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Component_Character;
			if( character != null && character.ItemGetEnabledFirst() == null )
			{
				info = new IComponent_InteractiveObject_ObjectInfo();
				info.AllowInteract = true;//CanTake;
				info.SelectionTextInfo.Add( Name );
				info.SelectionTextInfo.Add( "Click mouse button to take. Press T to drop." );
			}
		}

		public virtual bool ObjectInteractionInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//take by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Component_Character;
					if( character != null && character.ItemGetEnabledFirst() == null )
					{
						character.ItemTake( this );
						character.ItemActivate( this );
						return true;
					}
				}
			}

			return false;
		}

		public void ObjectInteractionEnter( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionExit( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionUpdate( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public delegate void GetBulletWorldInitialTransformEventDelegate( Component_Weapon sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform()
		{
			var result = Transform.Value * BulletTransform.Value;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				bool show = ( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this ) || AlwaysDisplayBulletInitialPosition;
				if( show )
				{
					var renderer = context.Owner.Simple3DRenderer;

					var tr = GetBulletWorldInitialTransform();
					var ray = new Ray( tr.Position, tr.Rotation.GetForward() * 0.1 );

					renderer.SetColor( new ColorValue( 1, 0, 0 ) );
					renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
				}
			}
		}

	}
}
