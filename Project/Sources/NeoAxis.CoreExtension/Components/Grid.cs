// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Visual 2D grid in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Base\Scene objects\Additional\Grid", 0 )]
	public class Grid : ObjectInSpace
	{
		/// <summary>
		/// The step of drawing lines.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Step
		{
			get { if( _step.BeginGet() ) Step = _step.Get( this ); return _step.value; }
			set { if( _step.BeginSet( ref value ) ) { try { StepChanged?.Invoke( this ); } finally { _step.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Step"/> property value changes.</summary>
		public event Action<Grid> StepChanged;
		ReferenceField<double> _step = 1.0;

		/// <summary>
		/// The step of drawing thick lines.
		/// </summary>
		[DefaultValue( 10 )]
		public Reference<int> ThickStep
		{
			get { if( _thickStep.BeginGet() ) ThickStep = _thickStep.Get( this ); return _thickStep.value; }
			set { if( _thickStep.BeginSet( ref value ) ) { try { ThickStepChanged?.Invoke( this ); } finally { _thickStep.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThickStep"/> property value changes.</summary>
		public event Action<Grid> ThickStepChanged;
		ReferenceField<int> _thickStep = 10;

		/// <summary>
		/// The color of the lines.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Grid> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// The color of the lines when they are behind other objects.
		/// </summary>
		[DefaultValue( "1 1 1 0.3" )]
		public Reference<ColorValue> ColorInvisible
		{
			get { if( _colorInvisible.BeginGet() ) ColorInvisible = _colorInvisible.Get( this ); return _colorInvisible.value; }
			set { if( _colorInvisible.BeginSet( ref value ) ) { try { ColorInvisibleChanged?.Invoke( this ); } finally { _colorInvisible.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ColorInvisible"/> property value changes.</summary>
		public event Action<Grid> ColorInvisibleChanged;
		ReferenceField<ColorValue> _colorInvisible = new ColorValue( 1, 1, 1, 0.3 );

		/// <summary>
		/// How far from the camera lines are visible.
		/// </summary>
		[DefaultValue( 50.0 )]
		[Range( 1, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Grid> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 50.0;

		/// <summary>
		/// Whether the component is visible in the simulation mode.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DisplayInSimulation
		{
			get { if( _displayInSimulation.BeginGet() ) DisplayInSimulation = _displayInSimulation.Get( this ); return _displayInSimulation.value; }
			set { if( _displayInSimulation.BeginSet( ref value ) ) { try { DisplayInSimulationChanged?.Invoke( this ); } finally { _displayInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayInSimulation"/> property value changes.</summary>
		public event Action<Grid> DisplayInSimulationChanged;
		ReferenceField<bool> _displayInSimulation = true;

		/////////////////////////////////////////

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var tr = Transform.Value;
			tr.Rotation.ToMatrix3( out var rot );
			var box = new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
			newBounds = new SpaceBounds( box.ToBounds() );
		}

		void RenderLine( RenderingContext context, Vector2 start2, Vector2 end2, bool thick, ref Vector3 position, ref Quaternion rotation,
			bool rotationIdentity )
		{
			Vector3 start, end;
			if( !rotationIdentity )
			{
				start = position + rotation * new Vector3( start2.X, start2.Y, 0 );
				end = position + rotation * new Vector3( end2.X, end2.Y, 0 );
			}
			else
			{
				start = position + new Vector3( start2.X, start2.Y, 0 );
				end = position + new Vector3( end2.X, end2.Y, 0 );
			}

			//clip visibility by distance
			var cameraPosition = context.viewport.CameraSettings.Position;
			var sphere = new Sphere( cameraPosition, VisibilityDistance );
			if( !sphere.Contains( start ) || !sphere.Contains( end ) )
			{
				var ray = new Ray( start, end - start );
				if( !sphere.Intersects( ray, out var scale1, out var scale2 ) )
					return;
				start = ray.GetPointOnRay( MathEx.Saturate( scale1 ) );
				end = ray.GetPointOnRay( MathEx.Saturate( scale2 ) );
			}

			var renderer = context.viewport.Simple3DRenderer;
			if( thick )
				renderer.AddLine( start, end, 0.04 );
			else
				renderer.AddLineThin( start, end );
		}

		void Render( RenderingContext context )
		{
			var renderer = context.viewport.Simple3DRenderer;

			Vector2 halfSize = TransformV.Scale.ToVector2() / 2;
			var maxDistance = halfSize.MaxComponent();
			var step = Step.Value;
			var thickStep = ThickStep.Value;

			var transform = Transform.Value;
			var position = transform.Position;
			var rotation = transform.Rotation;
			var rotationIdentity = rotation == Quaternion.Identity;

			renderer.SetColor( Color, ColorInvisible );

			int thickCounter = 0;
			for( double distance = 0; distance <= halfSize.X; distance += step, thickCounter++ )
			{
				var thick = thickStep != 0 && thickCounter % thickStep == 0;

				RenderLine( context, new Vector2( distance, -halfSize.Y ), new Vector2( distance, halfSize.Y ), thick, ref position, ref rotation, rotationIdentity );

				if( distance != 0 )
					RenderLine( context, new Vector2( -distance, -halfSize.Y ), new Vector2( -distance, halfSize.Y ), thick, ref position, ref rotation, rotationIdentity );
			}

			thickCounter = 0;
			for( double distance = 0; distance <= halfSize.Y; distance += step, thickCounter++ )
			{
				var thick = thickStep != 0 && thickCounter % thickStep == 0;

				RenderLine( context, new Vector2( -halfSize.X, distance ), new Vector2( halfSize.X, distance ), thick, ref position, ref rotation, rotationIdentity );

				if( distance != 0 )
					RenderLine( context, new Vector2( -halfSize.X, -distance ), new Vector2( halfSize.X, -distance ), thick, ref position, ref rotation, rotationIdentity );
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( EngineApp.ApplicationType != EngineApp.ApplicationTypeEnum.Simulation ||
					EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && DisplayInSimulation )
				{
					var cameraPosition = context.Owner.CameraSettings.Position;
					var sphere = new Sphere( cameraPosition, VisibilityDistance );

					if( SpaceBounds.CalculatedBoundingSphere.Intersects( ref sphere ) )
					{
						if( Step.Value >= 0 )
							Render( context2 );
					}
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			Transform = new Transform( TransformV.Position, TransformV.Rotation, new Vector3( 10, 10, 1 ) );
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Grid" );
		}
	}
}
