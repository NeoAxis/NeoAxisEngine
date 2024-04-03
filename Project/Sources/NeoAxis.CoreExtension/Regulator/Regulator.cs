// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Regulator in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Regulator\Regulator", 10110 )]
	[NewObjectDefaultName( "Regulator" )]
	public class Regulator : MeshInSpace, InteractiveObjectInterface
	{
		RegulatorType typeCached = new RegulatorType();

		bool changing;
		bool changingForward;

		bool needUpdateAdditionalItems;

		/////////////////////////////////////////

		const string typeDefault = @"Content\Regulators\Default\Default.regulatortype";

		/// <summary>
		/// The type of the item.
		/// </summary>
		[DefaultValueReference( typeDefault )]
		public Reference<RegulatorType> RegulatorType
		{
			get { if( _regulatorType.BeginGet() ) RegulatorType = _regulatorType.Get( this ); return _regulatorType.value; }
			set
			{
				if( _regulatorType.BeginSet( this, ref value ) )
				{
					try
					{
						RegulatorTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _regulatorType.value;
						if( typeCached == null )
							typeCached = new RegulatorType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();

						needUpdateAdditionalItems = true;
					}
					finally { _regulatorType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="RegulatorType"/> property value changes.</summary>
		public event Action<Regulator> RegulatorTypeChanged;
		ReferenceField<RegulatorType> _regulatorType = new Reference<RegulatorType>( null, typeDefault );

		/// <summary>
		/// The current position of the switch.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Value
		{
			get { if( _value.BeginGet() ) Value = _value.Get( this ); return _value.value; }
			set { if( _value.BeginSet( this, ref value ) ) { try { ValueChanged?.Invoke( this ); needUpdateAdditionalItems = true; } finally { _value.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Value"/> property value changes.</summary>
		public event Action<Regulator> ValueChanged;
		ReferenceField<double> _value = 0.0;

		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( this, ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<Regulator> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

		/////////////////////////////////////////

		[Browsable( false )]
		public RegulatorType TypeCached
		{
			get { return typeCached; }
		}

		[Browsable( false )]
		public bool Changing
		{
			get { return changing; }
		}

		[Browsable( false )]
		public bool ChangingForward
		{
			get { return changingForward; }
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
					skip = true;
					break;

				case nameof( AllowInteract ):
					if( !TypeCached.AllowInteract )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			RegulatorType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				UpdateMesh();
				UpdateAdditionalItems();
			}
		}

		void UpdateMesh()
		{
			Mesh = TypeCached.BaseMesh;
		}

		void UpdateAdditionalItems()
		{
			var additionalItems = new List<AdditionalItem>();

			//button
			{
				var mesh = TypeCached.ButtonMesh.Value;
				if( mesh != null )
				{
					var angle = GetValueAngle() - 90;
					var rotation = new Angles( angle, 0, 0 ).ToQuaternion();
					additionalItems.Add( new AdditionalItem( TypeCached.ButtonMesh, TypeCached.ButtonMeshPosition, rotation, Vector3.One, ColorValue.One ) );
				}
			}

			//indicator min
			{
				var mesh = TypeCached.IndicatorMinMesh.Value;
				if( mesh != null )
				{
					var item = new AdditionalItem( mesh, TypeCached.IndicatorMinMeshPosition, Quaternion.Identity, Vector3.One, ColorValue.One );
					item.ReplaceMaterial = Value.Value <= TypeCached.ValueRange.Value.Minimum ? TypeCached.IndicatorMinMaterialActivated.Value : null;
					additionalItems.Add( item );
				}
			}

			//indicator max
			{
				var mesh = TypeCached.IndicatorMaxMesh.Value;
				if( mesh != null )
				{
					var item = new AdditionalItem( mesh, TypeCached.IndicatorMaxMeshPosition, Quaternion.Identity, Vector3.One, ColorValue.One );
					item.ReplaceMaterial = Value.Value >= TypeCached.ValueRange.Value.Maximum ? TypeCached.IndicatorMaxMaterialActivated.Value : null;
					additionalItems.Add( item );
				}
			}

			var angleOffset = TypeCached.MarkerMeshPositionDependingAngleOffset.Value;

			//marker min
			{
				var mesh = TypeCached.MarkerMinMesh.Value;
				if( mesh != null )
				{
					var angle = TypeCached.AngleRange.Value.Minimum - 90;
					var angleR = MathEx.DegreeToRadian( angle );
					var position = TypeCached.MarkerMinMeshPosition + new Vector3( 0, Math.Cos( angleR ) * angleOffset, Math.Sin( -angleR ) * angleOffset );
					var rotation = new Angles( angle, 0, 0 ).ToQuaternion();

					var item = new AdditionalItem( mesh, position, rotation, Vector3.One, ColorValue.One );
					additionalItems.Add( item );
				}
			}

			//marker max
			{
				var mesh = TypeCached.MarkerMaxMesh.Value;
				if( mesh != null )
				{
					var angle = TypeCached.AngleRange.Value.Maximum - 90;
					var angleR = MathEx.DegreeToRadian( angle );
					var position = TypeCached.MarkerMaxMeshPosition + new Vector3( 0, Math.Cos( angleR ) * angleOffset, Math.Sin( -angleR ) * angleOffset );
					var rotation = new Angles( angle, 0, 0 ).ToQuaternion();

					var item = new AdditionalItem( mesh, position, rotation, Vector3.One, ColorValue.One );
					additionalItems.Add( item );
				}
			}

			//marker current
			{
				var mesh = TypeCached.MarkerCurrentMesh.Value;
				if( mesh != null )
				{
					var angle = GetValueAngle() - 90;
					var angleR = MathEx.DegreeToRadian( angle );
					var position = TypeCached.MarkerCurrentMeshPosition + new Vector3( 0, Math.Cos( angleR ) * angleOffset, Math.Sin( -angleR ) * angleOffset );
					var rotation = new Angles( angle, 0, 0 ).ToQuaternion();

					var item = new AdditionalItem( mesh, position, rotation, Vector3.One, ColorValue.One );
					additionalItems.Add( item );
				}
			}

			AdditionalItems = additionalItems.ToArray();
			needUpdateAdditionalItems = false;
		}

		protected virtual void OnChangingBegin() { }
		protected virtual void OnChangingEnd() { }

		public event Action<Regulator> ChangingBeginEvent;
		public event Action<Regulator> ChangingEndEvent;

		public void ChangingBegin( bool forward )
		{
			if( Changing && ChangingForward == forward )
				return;

			if( Changing && ChangingForward != forward )
				ChangingEnd();

			changing = true;
			changingForward = forward;
			NetworkSendChanging( null );
			OnChangingBegin();
			ChangingBeginEvent?.Invoke( this );
			needUpdateAdditionalItems = true;
		}

		public void ChangingEnd()
		{
			if( !Changing )
				return;

			changing = false;
			changingForward = false;
			NetworkSendChanging( null );
			OnChangingEnd();
			ChangingEndEvent?.Invoke( this );
			needUpdateAdditionalItems = true;
		}

		public double GetValueFactor()
		{
			var r = TypeCached.ValueRange.Value;
			if( r.Size != 0 )
				return ( Value.Value - r.Minimum ) / r.Size;
			return 0;
		}

		public double GetValueAngle()
		{
			return MathEx.Lerp( TypeCached.AngleRange.Value.Minimum, TypeCached.AngleRange.Value.Maximum, GetValueFactor() );
		}

		void SimulateTickSound( double oldValue, double newValue )
		{
			bool play = false;

			if( TypeCached.SoundTickFrequency > 0 )
			{
				for( var tickTime = TypeCached.ValueRange.Value.Minimum; tickTime <= TypeCached.ValueRange.Value.Maximum; tickTime += TypeCached.SoundTickFrequency )
				{
					var before = oldValue < tickTime;
					var after = newValue < tickTime;
					if( before != after )
					{
						play = true;
						break;
					}
				}
			}

			if( play )
			{
				SoundPlay( TypeCached.SoundTick );
				if( NetworkIsServer && TypeCached.SoundTick.ReferenceOrValueSpecified )
				{
					BeginNetworkMessageToEveryone( "SoundTick" );
					EndNetworkMessage();
				}
			}
		}

		void Simulate( float delta )
		{
			if( Changing )
			{
				var oldValue = Value.Value;

				//calculate new value
				var newValue = oldValue;
				var step = TypeCached.ChangeSpeed * delta;
				if( ChangingForward )
					newValue += step;
				else
					newValue -= step;
				newValue = MathEx.Clamp( newValue, TypeCached.ValueRange.Value.Minimum, TypeCached.ValueRange.Value.Maximum );

				//update value
				Value = newValue;

				SimulateTickSound( oldValue, newValue );
			}
		}

		public void SimulateRequiredValue( double requiredValue, float delta )
		{
			var oldValue = Value.Value;

			//calculate new value
			var newValue = oldValue;
			var step = TypeCached.ChangeSpeed * delta;
			if( newValue < requiredValue )
			{
				newValue += step;
				if( newValue > requiredValue )
					newValue = requiredValue;
			}
			else
			{
				newValue -= step;
				if( newValue < requiredValue )
					newValue = requiredValue;
			}
			newValue = MathEx.Clamp( newValue, TypeCached.ValueRange.Value.Minimum, TypeCached.ValueRange.Value.Maximum );

			//update value
			Value = newValue;

			SimulateTickSound( oldValue, newValue );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//if( EngineApp.IsEditor )
			//	Simulate( delta );

			//update the value for case when reference is specified
			Value.Touch();

			if( needUpdateAdditionalItems )
				UpdateAdditionalItems();
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					if( NetworkIsClient )
					{
						var writer = BeginNetworkMessageToServer( "ChangingBegin" );
						if( writer != null )
						{
							writer.Write( mouseDown.Button == EMouseButtons.Left );
							EndNetworkMessage();
						}
					}
					else
						ChangingBegin( mouseDown.Button == EMouseButtons.Left );
					return true;
				}
			}

			var mouseUp = message as InputMessageMouseButtonUp;
			if( mouseUp != null )
			{
				if( mouseUp.Button == EMouseButtons.Left && Changing && ChangingForward )
				{
					if( NetworkIsClient )
					{
						BeginNetworkMessageToServer( "ChangingEnd" );
						EndNetworkMessage();
					}
					else
						ChangingEnd();
					return true;
				}
				if( mouseUp.Button == EMouseButtons.Right && Changing && !ChangingForward )
				{
					if( NetworkIsClient )
					{
						BeginNetworkMessageToServer( "ChangingEnd" );
						EndNetworkMessage();
					}
					else
						ChangingEnd();
					return true;
				}
			}

			return false;
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Regulator sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			info = new InteractiveObjectObjectInfo();
			info.AllowInteract = AllowInteract && TypeCached.AllowInteract;
			//info.Text.Add( Name );
			ObjectInteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
			if( NetworkIsClient )
			{
				BeginNetworkMessageToServer( "ChangingEnd" );
				EndNetworkMessage();
			}
			else
				ChangingEnd();
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}

		void NetworkSendChanging( ServerNetworkService_Components.ClientItem client )
		{
			if( NetworkIsServer )
			{
				var writer = client != null ? BeginNetworkMessage( client, "Changing" ) : BeginNetworkMessageToEveryone( "Changing" );
				writer.Write( changing );
				writer.Write( changingForward );
				EndNetworkMessage();
			}
		}

		protected override void OnClientConnectedBeforeRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedBeforeRootComponentEnabled( client );

			NetworkSendChanging( client );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "SoundTick" )
				SoundPlay( TypeCached.SoundTick );
			else if( message == "Changing" )
			{
				changing = reader.ReadBoolean();
				changingForward = reader.ReadBoolean();
				needUpdateAdditionalItems = true;
			}

			return true;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;


			//!!!!security check is needed. who interact


			if( message == "ChangingBegin" )
			{
				var forward = reader.ReadBoolean();
				if( !reader.Complete() )
					return false;

				ChangingBegin( forward );
			}
			else if( message == "ChangingEnd" )
			{
				ChangingEnd();
			}

			return true;
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//base.OnSpaceBoundsUpdate( ref newBounds );

			Mesh m = MeshOutput;
			var result = m?.Result;
			if( result != null )
			{
				var meshSpaceBounds = result.SpaceBounds.BoundingBox;

				var v = TypeCached.ExpandSpaceBounds.Value;
				meshSpaceBounds.Maximum.X += v.X;
				meshSpaceBounds.Minimum.Y -= v.Y;
				meshSpaceBounds.Minimum.Z -= v.Z;
				meshSpaceBounds.Maximum.Y += v.Y;
				meshSpaceBounds.Maximum.Z += v.Z;

				newBounds = SpaceBounds.Multiply( Transform, new SpaceBounds( meshSpaceBounds ) );

				//var b = SpaceBounds.Multiply( Transform, new SpaceBounds( meshSpaceBounds ) );
				//newBounds = SpaceBounds.Merge( newBounds, b );
			}
		}
	}
}