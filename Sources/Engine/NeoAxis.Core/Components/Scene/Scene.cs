// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a scene.
	/// </summary>
	[ResourceFileExtension( "scene" )]
#if !DEPLOY
	[EditorControl( typeof( SceneEditor ) )]
	//[EditorNewObjectCell( typeof( Scene_NewObjectCell ) )]
	[NewObjectSettings( typeof( NewObjectSettingsScene ) )]
	[SettingsCell( typeof( SceneSettingsCell ) )]
#endif
	public partial class Scene : Component
	{
		static List<Scene> all = new List<Scene>();
		static List<Scene> allInstancesEnabled = new List<Scene>();

		double initTime;
		Bounds boundsWhenSimulationStarted;

		Sound backgroundSoundCurrent;
		SoundData backgroundSoundInstance;
		SoundVirtualChannel backgroundSoundChannel;

		//!!!!
		//Particles freeParticles;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The rendering pipeline of the scene.
		/// </summary>
		[Serialize]
		//[DefaultValue( "" )]
		public Reference<RenderingPipeline> RenderingPipeline
		{
			get { if( _renderingPipeline.BeginGet() ) RenderingPipeline = _renderingPipeline.Get( this ); return _renderingPipeline.value; }
			set { if( _renderingPipeline.BeginSet( ref value ) ) { try { RenderingPipelineChanged?.Invoke( this ); } finally { _renderingPipeline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderingPipeline"/> property value changes.</summary>
		public event Action<Scene> RenderingPipelineChanged;
		ReferenceField<RenderingPipeline> _renderingPipeline;

		/// <summary>
		/// The background color of the scene.
		/// </summary>
		[Serialize]
		[DefaultValue( "0.9 0.9 0.9" )]
		[ColorValueNoAlpha]
		public Reference<ColorValue> BackgroundColor
		{
			get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
			set { if( _backgroundColor.BeginSet( ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BackgroundColor"/> property value changes.</summary>
		public event Action<Scene> BackgroundColorChanged;
		ReferenceField<ColorValue> _backgroundColor = new ColorValue( 0.9, 0.9, 0.9 );

		/// <summary>
		/// Whether to affect the background color to ambient lighting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> BackgroundColorAffectLighting
		{
			get { if( _backgroundColorAffectLighting.BeginGet() ) BackgroundColorAffectLighting = _backgroundColorAffectLighting.Get( this ); return _backgroundColorAffectLighting.value; }
			set { if( _backgroundColorAffectLighting.BeginSet( ref value ) ) { try { BackgroundColorAffectLightingChanged?.Invoke( this ); } finally { _backgroundColorAffectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BackgroundColorAffectLighting"/> property value changes.</summary>
		public event Action<Scene> BackgroundColorAffectLightingChanged;
		ReferenceField<double> _backgroundColorAffectLighting = 1.0;

		[Browsable( false )]
		public ColorValue? BackgroundColorEnvironmentOverride { get; set; }

		/// <summary>
		/// The element of the user interface of the scene that will be used in a simulation.
		/// </summary>
		[DefaultValue( null )]
		[Serialize]
		[DisplayName( "UI Screen" )]
		public Reference<UIControl> UIScreen
		{
			get { if( _uIScreen.BeginGet() ) UIScreen = _uIScreen.Get( this ); return _uIScreen.value; }
			set { if( _uIScreen.BeginSet( ref value ) ) { try { UIScreenChanged?.Invoke( this ); } finally { _uIScreen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIScreen"/> property value changes.</summary>
		public event Action<Scene> UIScreenChanged;
		ReferenceField<UIControl> _uIScreen;

		public enum ModeEnum
		{
			_3D,
			_2D,
			//Special,
		}

		/// <summary>
		/// The mode of the scene.
		/// </summary>
		[DefaultValue( ModeEnum._3D )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		public event Action<Scene> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum._3D;

		/// <summary>
		/// The camera used by the editor in 3D mode.
		/// </summary>
		[Serialize]
		public Reference<Camera> CameraEditor
		{
			get { if( _cameraEditor.BeginGet() ) CameraEditor = _cameraEditor.Get( this ); return _cameraEditor.value; }
			set { if( _cameraEditor.BeginSet( ref value ) ) { try { CameraEditorChanged?.Invoke( this ); } finally { _cameraEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraEditor"/> property value changes.</summary>
		public event Action<Scene> CameraEditorChanged;
		ReferenceField<Camera> _cameraEditor;

		/// <summary>
		/// The camera used by the editor in 2D mode.
		/// </summary>
		[Serialize]
		[DisplayName( "Camera Editor 2D" )]
		public Reference<Camera> CameraEditor2D
		{
			get { if( _cameraEditor2D.BeginGet() ) CameraEditor2D = _cameraEditor2D.Get( this ); return _cameraEditor2D.value; }
			set { if( _cameraEditor2D.BeginSet( ref value ) ) { try { CameraEditor2DChanged?.Invoke( this ); } finally { _cameraEditor2D.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraEditor2D"/> property value changes.</summary>
		[DisplayName( "Camera Editor 2D Changed" )]
		public event Action<Scene> CameraEditor2DChanged;
		ReferenceField<Camera> _cameraEditor2D;

		/// <summary>
		/// Z position of the camera in 2D mode.
		/// </summary>
		[Browsable( false )]
		public double CameraEditor2DPositionZ { get; set; } = 10;

		//!!!!name: CameraByDefault
		/// <summary>
		/// The default camera used.
		/// </summary>
		[Serialize]
		public Reference<Camera> CameraDefault
		{
			get { if( _cameraDefault.BeginGet() ) CameraDefault = _cameraDefault.Get( this ); return _cameraDefault.value; }
			set { if( _cameraDefault.BeginSet( ref value ) ) { try { CameraDefaultChanged?.Invoke( this ); } finally { _cameraDefault.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CameraDefault"/> property value changes.</summary>
		public event Action<Scene> CameraDefaultChanged;
		ReferenceField<Camera> _cameraDefault;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The gravity applied on the physical objects.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 -9.81" )]
		[Category( "Physics" )]
		public Reference<Vector3> Gravity
		{
			get { if( _gravity.BeginGet() ) Gravity = _gravity.Get( this ); return _gravity.value; }
			set
			{
				if( _gravity.BeginSet( ref value ) )
				{
					try
					{
						GravityChanged?.Invoke( this );
						PhysicsUpdateGravity();
					}
					finally { _gravity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Gravity"/> property value changes.</summary>
		public event Action<Scene> GravityChanged;
		ReferenceField<Vector3> _gravity = new Vector3( 0, 0, -9.81 );

		//!!!!в Environment

		/// <summary>
		/// The direction of the wind.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> WindDirection
		{
			get { if( _windDirection.BeginGet() ) WindDirection = _windDirection.Get( this ); return _windDirection.value; }
			set { if( _windDirection.BeginSet( ref value ) ) { try { WindDirectionChanged?.Invoke( this ); } finally { _windDirection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindDirection"/> property value changes.</summary>
		public event Action<Scene> WindDirectionChanged;
		ReferenceField<Degree> _windDirection;

		/// <summary>
		/// The speed of the wind.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> WindSpeed
		{
			get { if( _windSpeed.BeginGet() ) WindSpeed = _windSpeed.Get( this ); return _windSpeed.value; }
			set { if( _windSpeed.BeginSet( ref value ) ) { try { WindSpeedChanged?.Invoke( this ); } finally { _windSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WindSpeed"/> property value changes.</summary>
		public event Action<Scene> WindSpeedChanged;
		ReferenceField<double> _windSpeed = 0.0;

		//!!!!default
		//!!!!обрабатывать когда переполнение
		//!!!!update when changed
		/// <summary>
		/// The max amount of bodies at the same time in the scene. Reopen the scene to apply changes.
		/// </summary>
		[DefaultValue( 100000 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsMaxBodies
		{
			get { if( _maxBodies.BeginGet() ) PhysicsMaxBodies = _maxBodies.Get( this ); return _maxBodies.value; }
			set { if( _maxBodies.BeginSet( ref value ) ) { try { MaxBodiesChanged?.Invoke( this ); } finally { _maxBodies.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMaxBodies"/> property value changes.</summary>
		public event Action<Scene> MaxBodiesChanged;
		ReferenceField<int> _maxBodies = 100000;

		/// <summary>
		/// Whether to enabled advanced settings of the physics world.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsAdvancedSettings
		{
			get { if( _physicsAdvancedSettings.BeginGet() ) PhysicsAdvancedSettings = _physicsAdvancedSettings.Get( this ); return _physicsAdvancedSettings.value; }
			set { if( _physicsAdvancedSettings.BeginSet( ref value ) ) { try { PhysicsAdvancedSettingsChanged?.Invoke( this ); UpdateAdvancedPhysicsSettings(); } finally { _physicsAdvancedSettings.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsAdvancedSettings"/> property value changes.</summary>
		public event Action<Scene> PhysicsAdvancedSettingsChanged;
		ReferenceField<bool> _physicsAdvancedSettings = false;

		internal const double physicsDefaultConvexRadiusDefault = 0.05;

		/// <summary>
		/// In order to speed up the collision detection system, all convex shapes use a convex radius. The provided shape will first be shrunken by the convex radius and then inflated again by the same amount.
		/// </summary>
		[DefaultValue( physicsDefaultConvexRadiusDefault )]
		[Category( "Physics" )]
		public Reference<double> PhysicsDefaultConvexRadius
		{
			get { if( _physicsDefaultConvexRadius.BeginGet() ) PhysicsDefaultConvexRadius = _physicsDefaultConvexRadius.Get( this ); return _physicsDefaultConvexRadius.value; }
			set { if( _physicsDefaultConvexRadius.BeginSet( ref value ) ) { try { PhysicsDefaultConvexRadiusChanged?.Invoke( this ); } finally { _physicsDefaultConvexRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsDefaultConvexRadius"/> property value changes.</summary>
		public event Action<Scene> PhysicsDefaultConvexRadiusChanged;
		ReferenceField<double> _physicsDefaultConvexRadius = physicsDefaultConvexRadiusDefault;

		/// <summary>
		/// Maximum amount of body pairs to process (anything else will fall through the world), this number should generally be much higher than the max amount of contact points as there will be lots of bodies close that are not actually touching. Set for to auto mode.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsMaxBodyPairs
		{
			get { if( _physicsMaxBodyPairs.BeginGet() ) PhysicsMaxBodyPairs = _physicsMaxBodyPairs.Get( this ); return _physicsMaxBodyPairs.value; }
			set { if( _physicsMaxBodyPairs.BeginSet( ref value ) ) { try { PhysicsMaxBodyPairsChanged?.Invoke( this ); } finally { _physicsMaxBodyPairs.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMaxBodyPairs"/> property value changes.</summary>
		public event Action<Scene> PhysicsMaxBodyPairsChanged;
		ReferenceField<int> _physicsMaxBodyPairs = 0;

		/// <summary>
		/// Maximum amount of contact constraints to process (anything else will fall through the world). Set 0 for auto mode.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsMaxContactConstraints
		{
			get { if( _physicsMaxContactConstraints.BeginGet() ) PhysicsMaxContactConstraints = _physicsMaxContactConstraints.Get( this ); return _physicsMaxContactConstraints.value; }
			set { if( _physicsMaxContactConstraints.BeginSet( ref value ) ) { try { PhysicsMaxContactConstraintsChanged?.Invoke( this ); } finally { _physicsMaxContactConstraints.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMaxContactConstraints"/> property value changes.</summary>
		public event Action<Scene> PhysicsMaxContactConstraintsChanged;
		ReferenceField<int> _physicsMaxContactConstraints = 0;

		/// <summary>
		/// Size of body pairs array, corresponds to the maximum amount of potential body pairs that can be in flight at any time. Setting this to a low value will use less memory but slow down simulation as threads may run out of narrow phase work. Set 0 for auto mode.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsMaxInFlightBodyPairs
		{
			get { if( _physicsMaxInFlightBodyPairs.BeginGet() ) PhysicsMaxInFlightBodyPairs = _physicsMaxInFlightBodyPairs.Get( this ); return _physicsMaxInFlightBodyPairs.value; }
			set { if( _physicsMaxInFlightBodyPairs.BeginSet( ref value ) ) { try { PhysicsMaxInFlightBodyPairsChanged?.Invoke( this ); UpdateAdvancedPhysicsSettings(); } finally { _physicsMaxInFlightBodyPairs.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMaxInFlightBodyPairs"/> property value changes.</summary>
		public event Action<Scene> PhysicsMaxInFlightBodyPairsChanged;
		ReferenceField<int> _physicsMaxInFlightBodyPairs = 0;

		/// <summary>
		/// The amount of collision steps for one update.
		/// </summary>
		[DefaultValue( 2 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsCollisionSteps
		{
			get { if( _physicsCollisionSteps.BeginGet() ) PhysicsCollisionSteps = _physicsCollisionSteps.Get( this ); return _physicsCollisionSteps.value; }
			set { if( _physicsCollisionSteps.BeginSet( ref value ) ) { try { PhysicsCollisionStepsChanged?.Invoke( this ); } finally { _physicsCollisionSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsCollisionSteps"/> property value changes.</summary>
		public event Action<Scene> PhysicsCollisionStepsChanged;
		ReferenceField<int> _physicsCollisionSteps = 2;

		/// <summary>
		/// The amount of integration sub steps for one update.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 1, 4 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsIntegrationSubSteps
		{
			get { if( _physicsIntegrationSubSteps.BeginGet() ) PhysicsIntegrationSubSteps = _physicsIntegrationSubSteps.Get( this ); return _physicsIntegrationSubSteps.value; }
			set { if( _physicsIntegrationSubSteps.BeginSet( ref value ) ) { try { PhysicsIntegrationSubStepsChanged?.Invoke( this ); } finally { _physicsIntegrationSubSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsIntegrationSubSteps"/> property value changes.</summary>
		public event Action<Scene> PhysicsIntegrationSubStepsChanged;
		ReferenceField<int> _physicsIntegrationSubSteps = 1;

		/// <summary>
		/// How many PhysicsStepListeners to notify in 1 batch.
		/// </summary>
		[DefaultValue( 8 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsStepListenersBatchSize
		{
			get { if( _physicsStepListenersBatchSize.BeginGet() ) PhysicsStepListenersBatchSize = _physicsStepListenersBatchSize.Get( this ); return _physicsStepListenersBatchSize.value; }
			set { if( _physicsStepListenersBatchSize.BeginSet( ref value ) ) { try { PhysicsStepListenersBatchSizeChanged?.Invoke( this ); UpdateAdvancedPhysicsSettings(); } finally { _physicsStepListenersBatchSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsStepListenersBatchSize"/> property value changes.</summary>
		public event Action<Scene> PhysicsStepListenersBatchSizeChanged;
		ReferenceField<int> _physicsStepListenersBatchSize = 8;

		/// <summary>
		/// How many step listener batches are needed before spawning another job. Set to 2147483647 (INT_MAX) if no parallelism is desired.
		/// </summary>
		[DefaultValue( 1 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsStepListenerBatchesPerJob
		{
			get { if( _physicsStepListenerBatchesPerJob.BeginGet() ) PhysicsStepListenerBatchesPerJob = _physicsStepListenerBatchesPerJob.Get( this ); return _physicsStepListenerBatchesPerJob.value; }
			set { if( _physicsStepListenerBatchesPerJob.BeginSet( ref value ) ) { try { PhysicsStepListenerBatchesPerJobChanged?.Invoke( this ); } finally { _physicsStepListenerBatchesPerJob.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsStepListenerBatchesPerJob"/> property value changes.</summary>
		public event Action<Scene> PhysicsStepListenerBatchesPerJobChanged;
		ReferenceField<int> _physicsStepListenerBatchesPerJob = 1;

		/// <summary>
		/// Baumgarte stabilization factor (how much of the position error to 'fix' in 1 update) (unit: dimensionless, 0 = nothing, 1 = 100%).
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsBaumgarte
		{
			get { if( _physicsBaumgarte.BeginGet() ) PhysicsBaumgarte = _physicsBaumgarte.Get( this ); return _physicsBaumgarte.value; }
			set { if( _physicsBaumgarte.BeginSet( ref value ) ) { try { PhysicsBaumgarteChanged?.Invoke( this ); } finally { _physicsBaumgarte.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsBaumgarte"/> property value changes.</summary>
		public event Action<Scene> PhysicsBaumgarteChanged;
		ReferenceField<double> _physicsBaumgarte = 0.2;

		/// <summary>
		/// Radius around objects inside which speculative contact points will be detected (unit: meters).
		/// </summary>
		/// <remarks>Note that if this is too big you will get ghost collisions as speculative contacts are based on the closest points during the collision detection step which may not be the actual closest points by the time the two objects hit.</remarks>
		[DefaultValue( 0.01 )]// 0.02 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsSpeculativeContactDistance
		{
			get { if( _physicsSpeculativeContactDistance.BeginGet() ) PhysicsSpeculativeContactDistance = _physicsSpeculativeContactDistance.Get( this ); return _physicsSpeculativeContactDistance.value; }
			set { if( _physicsSpeculativeContactDistance.BeginSet( ref value ) ) { try { PhysicsSpeculativeContactDistanceChanged?.Invoke( this ); } finally { _physicsSpeculativeContactDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsSpeculativeContactDistance"/> property value changes.</summary>
		public event Action<Scene> PhysicsSpeculativeContactDistanceChanged;
		ReferenceField<double> _physicsSpeculativeContactDistance = 0.01;//0.02;

		/// <summary>
		/// How much bodies are allowed to sink into eachother (unit: meters).
		/// </summary>
		[DefaultValue( 0.01 )]// 0.02 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsPenetrationSlop
		{
			get { if( _physicsPenetrationSlop.BeginGet() ) PhysicsPenetrationSlop = _physicsPenetrationSlop.Get( this ); return _physicsPenetrationSlop.value; }
			set { if( _physicsPenetrationSlop.BeginSet( ref value ) ) { try { PhysicsPenetrationSlopChanged?.Invoke( this ); } finally { _physicsPenetrationSlop.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsPenetrationSlop"/> property value changes.</summary>
		public event Action<Scene> PhysicsPenetrationSlopChanged;
		ReferenceField<double> _physicsPenetrationSlop = 0.01;// 0.02;

		/// <summary>
		/// Fraction of its inner radius a body must move per step to enable casting for the LinearCast motion quality.
		/// </summary>
		[DefaultValue( 0.75 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsLinearCastThreshold
		{
			get { if( _physicsLinearCastThreshold.BeginGet() ) PhysicsLinearCastThreshold = _physicsLinearCastThreshold.Get( this ); return _physicsLinearCastThreshold.value; }
			set { if( _physicsLinearCastThreshold.BeginSet( ref value ) ) { try { PhysicsLinearCastThresholdChanged?.Invoke( this ); } finally { _physicsLinearCastThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsLinearCastThreshold"/> property value changes.</summary>
		public event Action<Scene> PhysicsLinearCastThresholdChanged;
		ReferenceField<double> _physicsLinearCastThreshold = 0.75;

		/// <summary>
		/// Fraction of its inner radius a body may penetrate another body for the LinearCast motion quality.
		/// </summary>
		[DefaultValue( 0.25 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsLinearCastMaxPenetration
		{
			get { if( _physicsLinearCastMaxPenetration.BeginGet() ) PhysicsLinearCastMaxPenetration = _physicsLinearCastMaxPenetration.Get( this ); return _physicsLinearCastMaxPenetration.value; }
			set { if( _physicsLinearCastMaxPenetration.BeginSet( ref value ) ) { try { PhysicsLinearCastMaxPenetrationChanged?.Invoke( this ); } finally { _physicsLinearCastMaxPenetration.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsLinearCastMaxPenetration"/> property value changes.</summary>
		public event Action<Scene> PhysicsLinearCastMaxPenetrationChanged;
		ReferenceField<double> _physicsLinearCastMaxPenetration = 0.25;

		/// <summary>
		/// Max squared distance to use to determine if two points are on the same plane for determining the contact manifold between two shape faces (unit: meter).
		/// </summary>
		[DefaultValue( 0.001 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsManifoldTolerance
		{
			get { if( _physicsManifoldTolerance.BeginGet() ) PhysicsManifoldTolerance = _physicsManifoldTolerance.Get( this ); return _physicsManifoldTolerance.value; }
			set { if( _physicsManifoldTolerance.BeginSet( ref value ) ) { try { PhysicsManifoldToleranceChanged?.Invoke( this ); } finally { _physicsManifoldTolerance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsManifoldTolerance"/> property value changes.</summary>
		public event Action<Scene> PhysicsManifoldToleranceChanged;
		ReferenceField<double> _physicsManifoldTolerance = 0.001;

		/// <summary>
		/// Maximum distance to correct in a single iteration when solving position constraints (unit: meters).
		/// </summary>
		[DefaultValue( 0.02 )]//0.2 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsMaxPenetrationDistance
		{
			get { if( _physicsMaxPenetrationDistance.BeginGet() ) PhysicsMaxPenetrationDistance = _physicsMaxPenetrationDistance.Get( this ); return _physicsMaxPenetrationDistance.value; }
			set { if( _physicsMaxPenetrationDistance.BeginSet( ref value ) ) { try { PhysicsMaxPenetrationDistanceChanged?.Invoke( this ); } finally { _physicsMaxPenetrationDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMaxPenetrationDistance"/> property value changes.</summary>
		public event Action<Scene> PhysicsMaxPenetrationDistanceChanged;
		ReferenceField<double> _physicsMaxPenetrationDistance = 0.02;//0.2;

		/// <summary>
		/// Maximum relative delta position for body pairs to be able to reuse collision results from last frame (units: meter).
		/// </summary>
		[DefaultValue( 0.001 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsBodyPairCacheMaxDeltaPosition
		{
			get { if( _physicsBodyPairCacheMaxDeltaPosition.BeginGet() ) PhysicsBodyPairCacheMaxDeltaPosition = _physicsBodyPairCacheMaxDeltaPosition.Get( this ); return _physicsBodyPairCacheMaxDeltaPosition.value; }
			set { if( _physicsBodyPairCacheMaxDeltaPosition.BeginSet( ref value ) ) { try { PhysicsBodyPairCacheMaxDeltaPositionChanged?.Invoke( this ); } finally { _physicsBodyPairCacheMaxDeltaPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsBodyPairCacheMaxDeltaPosition"/> property value changes.</summary>
		public event Action<Scene> PhysicsBodyPairCacheMaxDeltaPositionChanged;
		ReferenceField<double> _physicsBodyPairCacheMaxDeltaPosition = 0.001;

		/// <summary>
		/// Maximum relative delta orientation for body pairs to be able to reuse collision results from last frame, stored as cos(max angle / 2). Default: cos(2 degrees / 2).
		/// </summary>
		[DefaultValue( 0.99984769515639123915701155881391 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsBodyPairCacheCosMaxDeltaRotationDiv2
		{
			get { if( _physicsBodyPairCacheCosMaxDeltaRotationDiv2.BeginGet() ) PhysicsBodyPairCacheCosMaxDeltaRotationDiv2 = _physicsBodyPairCacheCosMaxDeltaRotationDiv2.Get( this ); return _physicsBodyPairCacheCosMaxDeltaRotationDiv2.value; }
			set { if( _physicsBodyPairCacheCosMaxDeltaRotationDiv2.BeginSet( ref value ) ) { try { PhysicsBodyPairCacheCosMaxDeltaRotationDiv2Changed?.Invoke( this ); } finally { _physicsBodyPairCacheCosMaxDeltaRotationDiv2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsBodyPairCacheCosMaxDeltaRotationDiv2"/> property value changes.</summary>
		public event Action<Scene> PhysicsBodyPairCacheCosMaxDeltaRotationDiv2Changed;
		ReferenceField<double> _physicsBodyPairCacheCosMaxDeltaRotationDiv2 = 0.99984769515639123915701155881391;

		/// <summary>
		/// Maximum angle between normals that allows manifolds between different sub shapes of the same body pair to be combined. Default: cos(5 degree).
		/// </summary>
		[DefaultValue( 0.99619469809174553229501040247389 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsContactNormalCosMaxDeltaRotation
		{
			get { if( _physicsContactNormalCosMaxDeltaRotation.BeginGet() ) PhysicsContactNormalCosMaxDeltaRotation = _physicsContactNormalCosMaxDeltaRotation.Get( this ); return _physicsContactNormalCosMaxDeltaRotation.value; }
			set { if( _physicsContactNormalCosMaxDeltaRotation.BeginSet( ref value ) ) { try { PhysicsContactNormalCosMaxDeltaRotationChanged?.Invoke( this ); } finally { _physicsContactNormalCosMaxDeltaRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsContactNormalCosMaxDeltaRotation"/> property value changes.</summary>
		public event Action<Scene> PhysicsContactNormalCosMaxDeltaRotationChanged;
		ReferenceField<double> _physicsContactNormalCosMaxDeltaRotation = 0.99619469809174553229501040247389;

		/// <summary>
		/// Maximum allowed distance between old and new contact point to preserve contact forces for warm start (units: meter).
		/// </summary>
		[DefaultValue( 0.01 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsContactPointPreserveLambdaMaxDist
		{
			get { if( _physicsContactPointPreserveLambdaMaxDist.BeginGet() ) PhysicsContactPointPreserveLambdaMaxDist = _physicsContactPointPreserveLambdaMaxDist.Get( this ); return _physicsContactPointPreserveLambdaMaxDist.value; }
			set { if( _physicsContactPointPreserveLambdaMaxDist.BeginSet( ref value ) ) { try { PhysicsContactPointPreserveLambdaMaxDistChanged?.Invoke( this ); } finally { _physicsContactPointPreserveLambdaMaxDist.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsContactPointPreserveLambdaMaxDist"/> property value changes.</summary>
		public event Action<Scene> PhysicsContactPointPreserveLambdaMaxDistChanged;
		ReferenceField<double> _physicsContactPointPreserveLambdaMaxDist = 0.01;

		/// <summary>
		/// Number of solver velocity iterations to run. Note that this needs to be >= 2 in order for friction to work (friction is applied using the non-penetration impulse from the previous iteration).
		/// </summary>
		[DefaultValue( 10 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsNumVelocitySteps
		{
			get { if( _physicsNumVelocitySteps.BeginGet() ) PhysicsNumVelocitySteps = _physicsNumVelocitySteps.Get( this ); return _physicsNumVelocitySteps.value; }
			set { if( _physicsNumVelocitySteps.BeginSet( ref value ) ) { try { PhysicsNumVelocityStepsChanged?.Invoke( this ); } finally { _physicsNumVelocitySteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsNumVelocitySteps"/> property value changes.</summary>
		public event Action<Scene> PhysicsNumVelocityStepsChanged;
		ReferenceField<int> _physicsNumVelocitySteps = 10;

		/// <summary>
		/// Number of solver position iterations to run.
		/// </summary>
		[DefaultValue( 2 )]
		[Category( "Physics" )]
		public Reference<int> PhysicsNumPositionSteps
		{
			get { if( _physicsNumPositionSteps.BeginGet() ) PhysicsNumPositionSteps = _physicsNumPositionSteps.Get( this ); return _physicsNumPositionSteps.value; }
			set { if( _physicsNumPositionSteps.BeginSet( ref value ) ) { try { PhysicsNumPositionStepsChanged?.Invoke( this ); } finally { _physicsNumPositionSteps.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsNumPositionSteps"/> property value changes.</summary>
		public event Action<Scene> PhysicsNumPositionStepsChanged;
		ReferenceField<int> _physicsNumPositionSteps = 2;

		/// <summary>
		/// Minimal velocity needed before a collision can be elastic (unit: m).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsMinVelocityForRestitution
		{
			get { if( _physicsMinVelocityForRestitution.BeginGet() ) PhysicsMinVelocityForRestitution = _physicsMinVelocityForRestitution.Get( this ); return _physicsMinVelocityForRestitution.value; }
			set { if( _physicsMinVelocityForRestitution.BeginSet( ref value ) ) { try { PhysicsMinVelocityForRestitutionChanged?.Invoke( this ); } finally { _physicsMinVelocityForRestitution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMinVelocityForRestitution"/> property value changes.</summary>
		public event Action<Scene> PhysicsMinVelocityForRestitutionChanged;
		ReferenceField<double> _physicsMinVelocityForRestitution = 1.0;

		/// <summary>
		/// Time before object is allowed to go to sleep (unit: seconds).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsTimeBeforeSleep
		{
			get { if( _physicsTimeBeforeSleep.BeginGet() ) PhysicsTimeBeforeSleep = _physicsTimeBeforeSleep.Get( this ); return _physicsTimeBeforeSleep.value; }
			set { if( _physicsTimeBeforeSleep.BeginSet( ref value ) ) { try { PhysicsTimeBeforeSleepChanged?.Invoke( this ); } finally { _physicsTimeBeforeSleep.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsTimeBeforeSleep"/> property value changes.</summary>
		public event Action<Scene> PhysicsTimeBeforeSleepChanged;
		ReferenceField<double> _physicsTimeBeforeSleep = 0.5;

		/// <summary>
		/// Velocity of points on bounding box of object below which an object can be considered sleeping (unit: m/s).
		/// </summary>
		[DefaultValue( 0.01 )]// 0.03 )]
		[Category( "Physics" )]
		public Reference<double> PhysicsPointVelocitySleepThreshold
		{
			get { if( _physicsPointVelocitySleepThreshold.BeginGet() ) PhysicsPointVelocitySleepThreshold = _physicsPointVelocitySleepThreshold.Get( this ); return _physicsPointVelocitySleepThreshold.value; }
			set { if( _physicsPointVelocitySleepThreshold.BeginSet( ref value ) ) { try { PhysicsPointVelocitySleepThresholdChanged?.Invoke( this ); } finally { _physicsPointVelocitySleepThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsPointVelocitySleepThreshold"/> property value changes.</summary>
		public event Action<Scene> PhysicsPointVelocitySleepThresholdChanged;
		ReferenceField<double> _physicsPointVelocitySleepThreshold = 0.01;//0.03;

		/// <summary>
		/// Whether or not to use warm starting for constraints (initially applying previous frames impulses).
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsConstraintWarmStart
		{
			get { if( _physicsConstraintWarmStart.BeginGet() ) PhysicsConstraintWarmStart = _physicsConstraintWarmStart.Get( this ); return _physicsConstraintWarmStart.value; }
			set { if( _physicsConstraintWarmStart.BeginSet( ref value ) ) { try { PhysicsConstraintWarmStartChanged?.Invoke( this ); } finally { _physicsConstraintWarmStart.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsConstraintWarmStart"/> property value changes.</summary>
		public event Action<Scene> PhysicsConstraintWarmStartChanged;
		ReferenceField<bool> _physicsConstraintWarmStart = true;

		/// <summary>
		/// Whether or not to use the body pair cache, which removes the need for narrow phase collision detection when orientation between two bodies didn't change.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsUseBodyPairContactCache
		{
			get { if( _physicsUseBodyPairContactCache.BeginGet() ) PhysicsUseBodyPairContactCache = _physicsUseBodyPairContactCache.Get( this ); return _physicsUseBodyPairContactCache.value; }
			set { if( _physicsUseBodyPairContactCache.BeginSet( ref value ) ) { try { PhysicsUseBodyPairContactCacheChanged?.Invoke( this ); } finally { _physicsUseBodyPairContactCache.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsUseBodyPairContactCache"/> property value changes.</summary>
		public event Action<Scene> PhysicsUseBodyPairContactCacheChanged;
		ReferenceField<bool> _physicsUseBodyPairContactCache = true;

		/// <summary>
		/// Whether or not to reduce manifolds with similar contact normals into one contact manifold.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsUseManifoldReduction
		{
			get { if( _physicsUseManifoldReduction.BeginGet() ) PhysicsUseManifoldReduction = _physicsUseManifoldReduction.Get( this ); return _physicsUseManifoldReduction.value; }
			set { if( _physicsUseManifoldReduction.BeginSet( ref value ) ) { try { PhysicsUseManifoldReductionChanged?.Invoke( this ); } finally { _physicsUseManifoldReduction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsUseManifoldReduction"/> property value changes.</summary>
		public event Action<Scene> PhysicsUseManifoldReductionChanged;
		ReferenceField<bool> _physicsUseManifoldReduction = true;

		/// <summary>
		/// If objects can go to sleep or not.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsAllowSleeping
		{
			get { if( _physicsAllowSleeping.BeginGet() ) PhysicsAllowSleeping = _physicsAllowSleeping.Get( this ); return _physicsAllowSleeping.value; }
			set { if( _physicsAllowSleeping.BeginSet( ref value ) ) { try { PhysicsAllowSleepingChanged?.Invoke( this ); } finally { _physicsAllowSleeping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsAllowSleeping"/> property value changes.</summary>
		public event Action<Scene> PhysicsAllowSleepingChanged;
		ReferenceField<bool> _physicsAllowSleeping = true;

		/// <summary>
		/// When false, we prevent collision against non-active (shared) edges. Mainly for debugging the algorithm.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> PhysicsCheckActiveEdges
		{
			get { if( _physicsCheckActiveEdges.BeginGet() ) PhysicsCheckActiveEdges = _physicsCheckActiveEdges.Get( this ); return _physicsCheckActiveEdges.value; }
			set { if( _physicsCheckActiveEdges.BeginSet( ref value ) ) { try { PhysicsCheckActiveEdgesChanged?.Invoke( this ); } finally { _physicsCheckActiveEdges.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsCheckActiveEdges"/> property value changes.</summary>
		public event Action<Scene> PhysicsCheckActiveEdgesChanged;
		ReferenceField<bool> _physicsCheckActiveEdges = true;


		////!!!!default
		///// <summary>
		///// The number of simulation sub-steps performed by the physics system per one update.
		///// </summary>
		//[DefaultValue( 2 )]
		//[Serialize]
		//[Category( "Physics" )]
		//public Reference<int> PhysicsSimulationSteps
		//{
		//	get { if( _physicsSimulationSteps.BeginGet() ) PhysicsSimulationSteps = _physicsSimulationSteps.Get( this ); return _physicsSimulationSteps.value; }
		//	set { if( _physicsSimulationSteps.BeginSet( ref value ) ) { try { PhysicsSimulationStepsChanged?.Invoke( this ); } finally { _physicsSimulationSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PhysicsSimulationSteps"/> property value changes.</summary>
		//public event Action<Scene> PhysicsSimulationStepsChanged;
		//ReferenceField<int> _physicsSimulationSteps = 2;

		////!!!!default
		///// <summary>
		///// The number of iterations performed by the physics system.
		///// </summary>
		//[DefaultValue( 4 )]// 10 )]
		//[Serialize]
		//[Category( "Physics" )]
		//public Reference<int> PhysicsNumberIterations
		//{
		//	get { if( _physicsNumberIterations.BeginGet() ) PhysicsNumberIterations = _physicsNumberIterations.Get( this ); return _physicsNumberIterations.value; }
		//	set { if( _physicsNumberIterations.BeginSet( ref value ) ) { try { PhysicsNumberIterationsChanged?.Invoke( this ); } finally { _physicsNumberIterations.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PhysicsNumberIterations"/> property value changes.</summary>
		//public event Action<Scene> PhysicsNumberIterationsChanged;
		//ReferenceField<int> _physicsNumberIterations = 4;// 10;

		///////////////////////////////////////////////

		/// <summary>
		/// The gravity applied on the 2D physical objects.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 -9.81" )]
		[Category( "Physics 2D" )]
		[DisplayName( "Gravity 2D" )]
		public Reference<Vector2> Gravity2D
		{
			get { if( _gravity2D.BeginGet() ) Gravity2D = _gravity2D.Get( this ); return _gravity2D.value; }
			set
			{
				if( _gravity2D.BeginSet( ref value ) )
				{
					try
					{
						Gravity2DChanged?.Invoke( this );
						Physics2DUpdateGravity();
					}
					finally { _gravity2D.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Gravity2D"/> property value changes.</summary>
		[DisplayName( "Gravity 2D Changed" )]
		public event Action<Scene> Gravity2DChanged;
		ReferenceField<Vector2> _gravity2D = new Vector2( 0, -9.81 );

		///////////////////////////////////////////////

		/// <summary>
		/// Specifies background sound of the scene. Usually it is a music.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Background Sound" )]
		public Reference<Sound> BackgroundSound
		{
			get { if( _backgroundSound.BeginGet() ) BackgroundSound = _backgroundSound.Get( this ); return _backgroundSound.value; }
			set
			{
				if( _backgroundSound.BeginSet( ref value ) )
				{
					try
					{
						BackgroundSoundChanged?.Invoke( this );
						BackgroundSoundUpdate();
					}
					finally { _backgroundSound.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BackgroundSound"/> property value changes.</summary>
		public event Action<Scene> BackgroundSoundChanged;
		ReferenceField<Sound> _backgroundSound = null;

		/// <summary>
		/// Specifies background sound volume in the editor.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Background Sound" )]
		public Reference<double> BackgroundSoundVolumeInEditor
		{
			get { if( _backgroundSoundVolumeInEditor.BeginGet() ) BackgroundSoundVolumeInEditor = _backgroundSoundVolumeInEditor.Get( this ); return _backgroundSoundVolumeInEditor.value; }
			set
			{
				if( _backgroundSoundVolumeInEditor.BeginSet( ref value ) )
				{
					try
					{
						BackgroundSoundVolumeInEditorChanged?.Invoke( this );
						BackgroundSoundUpdate();
					}
					finally { _backgroundSoundVolumeInEditor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BackgroundSoundVolumeInEditor"/> property value changes.</summary>
		public event Action<Scene> BackgroundSoundVolumeInEditorChanged;
		ReferenceField<double> _backgroundSoundVolumeInEditor = 0.0;

		/// <summary>
		/// Specifies background sound volume in the simulation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Background Sound" )]
		public Reference<double> BackgroundSoundVolumeInSimulation
		{
			get { if( _backgroundSoundVolumeInSimulation.BeginGet() ) BackgroundSoundVolumeInSimulation = _backgroundSoundVolumeInSimulation.Get( this ); return _backgroundSoundVolumeInSimulation.value; }
			set
			{
				if( _backgroundSoundVolumeInSimulation.BeginSet( ref value ) )
				{
					try
					{
						BackgroundSoundVolumeInSimulationChanged?.Invoke( this );
						BackgroundSoundUpdate();
					}
					finally { _backgroundSoundVolumeInSimulation.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BackgroundSoundVolumeInSimulation"/> property value changes.</summary>
		public event Action<Scene> BackgroundSoundVolumeInSimulationChanged;
		ReferenceField<double> _backgroundSoundVolumeInSimulation = 1.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!в terrain?
		//[Category( "Environment" )]
		//[DefaultValue( 0.75 )]
		//[Range( 0, 1 )]
		//public Reference<double> MineralsQuality
		//{
		//	get { if( _mineralsQuality.BeginGet() ) MineralsQuality = _mineralsQuality.Get( this ); return _mineralsQuality.value; }
		//	set { if( _mineralsQuality.BeginSet( ref value ) ) { try { MineralsQualityChanged?.Invoke( this ); } finally { _mineralsQuality.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MineralsQuality"/> property value changes.</summary>
		//public event Action<Scene> MineralsQualityChanged;
		//ReferenceField<double> _mineralsQuality = 0.75;

		//!!!!
		//[Category( "Environment" )]
		//[DefaultValue( 0.75 )]
		//[Range( 0, 1 )]
		//public Reference<double> AverageSolarEnergy
		//{
		//	get { if( _averageSolarEnergy.BeginGet() ) AverageSolarEnergy = _averageSolarEnergy.Get( this ); return _averageSolarEnergy.value; }
		//	set { if( _averageSolarEnergy.BeginSet( ref value ) ) { try { AverageSolarEnergyChanged?.Invoke( this ); } finally { _averageSolarEnergy.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AverageSolarEnergy"/> property value changes.</summary>
		//public event Action<Scene> AverageSolarEnergyChanged;
		//ReferenceField<double> _averageSolarEnergy = 0.75;

		//!!!!
		//[Category( "Environment" )]
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//public Reference<double> AverageHumidity
		//{
		//	get { if( _averageAverageHumidity.BeginGet() ) AverageHumidity = _averageAverageHumidity.Get( this ); return _averageAverageHumidity.value; }
		//	set { if( _averageAverageHumidity.BeginSet( ref value ) ) { try { AverageHumidityChanged?.Invoke( this ); } finally { _averageAverageHumidity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AverageHumidity"/> property value changes.</summary>
		//public event Action<Scene> AverageHumidityChanged;
		//ReferenceField<double> _averageAverageHumidity = 0.5;

		/// <summary>
		/// Season of a year. 0 - summer, 1 - fall, 2 - winter, 3 - spring.
		/// </summary>
		[Category( "Environment" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 4 )]
		public Reference<double> Season
		{
			get { if( _season.BeginGet() ) Season = _season.Get( this ); return _season.value; }
			set { if( _season.BeginSet( ref value ) ) { try { SeasonChanged?.Invoke( this ); } finally { _season.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Season"/> property value changes.</summary>
		public event Action<Scene> SeasonChanged;
		ReferenceField<double> _season = 0.0;

		[Category( "Environment" )]
		[DefaultValue( 20 )]
		[Range( -40, 60 )]
		[DisplayName( "Temperature °C" )]
		public Reference<double> Temperature
		{
			get { if( _temperature.BeginGet() ) Temperature = _temperature.Get( this ); return _temperature.value; }
			set { if( _temperature.BeginSet( ref value ) ) { try { TemperatureChanged?.Invoke( this ); } finally { _temperature.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Temperature"/> property value changes.</summary>
		public event Action<Scene> TemperatureChanged;
		ReferenceField<double> _temperature = 20;

		[Category( "Environment" )]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		public Reference<double> Humidity
		{
			get { if( _humidity.BeginGet() ) Humidity = _humidity.Get( this ); return _humidity.value; }
			set { if( _humidity.BeginSet( ref value ) ) { try { HumidityChanged?.Invoke( this ); } finally { _humidity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Humidity"/> property value changes.</summary>
		public event Action<Scene> HumidityChanged;
		ReferenceField<double> _humidity = 0.5;

		[Category( "Environment" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> Precipitation
		{
			get { if( _precipitation.BeginGet() ) Precipitation = _precipitation.Get( this ); return _precipitation.value; }
			set { if( _precipitation.BeginSet( ref value ) ) { try { PrecipitationChanged?.Invoke( this ); } finally { _precipitation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Precipitation"/> property value changes.</summary>
		public event Action<Scene> PrecipitationChanged;
		ReferenceField<double> _precipitation = 0.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Whether to show development data in the editor.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayDevelopmentDataInEditor
		{
			get { if( _displayDevelopmentDataInEditor.BeginGet() ) DisplayDevelopmentDataInEditor = _displayDevelopmentDataInEditor.Get( this ); return _displayDevelopmentDataInEditor.value; }
			set { if( _displayDevelopmentDataInEditor.BeginSet( ref value ) ) { try { DisplayDevelopmentDataInEditorChanged?.Invoke( this ); } finally { _displayDevelopmentDataInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayDevelopmentDataInEditor"/> property value changes.</summary>
		public event Action<Scene> DisplayDevelopmentDataInEditorChanged;
		ReferenceField<bool> _displayDevelopmentDataInEditor = true;

		/// <summary>
		/// Whether to show development data in the simulation.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayDevelopmentDataInSimulation
		{
			get { if( _displayDevelopmentDataInSimulation.BeginGet() ) DisplayDevelopmentDataInSimulation = _displayDevelopmentDataInSimulation.Get( this ); return _displayDevelopmentDataInSimulation.value; }
			set { if( _displayDevelopmentDataInSimulation.BeginSet( ref value ) ) { try { DisplayDevelopmentDataInSimulationChanged?.Invoke( this ); } finally { _displayDevelopmentDataInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayDevelopmentDataInSimulation"/> property value changes.</summary>
		public event Action<Scene> DisplayDevelopmentDataInSimulationChanged;
		ReferenceField<bool> _displayDevelopmentDataInSimulation = false;

		////DisplayDevelopmentDataInSimulation
		//ReferenceField<bool> _displayDevelopmentDataInSimulation;
		///// <summary>
		///// Whether development data is displayed during simulation.
		///// </summary>
		//[DefaultValue( false )]
		//[Serialize]
		//[Category( "Development Data" )]
		//public Reference<bool> DisplayDevelopmentDataInSimulation
		//{
		//	get
		//	{
		//		if( _displayDevelopmentDataInSimulation.BeginGet() )
		//			DisplayDevelopmentDataInSimulation = _displayDevelopmentDataInSimulation.Get( this );
		//		return _displayDevelopmentDataInSimulation.value;
		//	}
		//	set
		//	{
		//		if( _displayDevelopmentDataInSimulation.BeginSet( ref value ) )
		//		{
		//			try { DisplayAdditionalDataInSimulationChanged?.Invoke( this ); }
		//			finally { _displayDevelopmentDataInSimulation.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Scene> DisplayAdditionalDataInSimulationChanged;

		//DisplayTextInfo
		ReferenceField<bool> _displayTextInfo = false;
		/// <summary>
		/// Whether to display text information of the development data.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayTextInfo
		{
			get { if( _displayTextInfo.BeginGet() ) DisplayTextInfo = _displayTextInfo.Get( this ); return _displayTextInfo.value; }
			set { if( _displayTextInfo.BeginSet( ref value ) ) { try { DisplayTextInfoChanged?.Invoke( this ); } finally { _displayTextInfo.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayTextInfo"/> property value changes.</summary>
		public event Action<Scene> DisplayTextInfoChanged;

		//DisplayLabels
		ReferenceField<bool> _displayLabels = true;
		/// <summary>
		/// Whether to display development data of the labels.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayLabels
		{
			get
			{
				if( _displayLabels.BeginGet() )
					DisplayLabels = _displayLabels.Get( this );
				return _displayLabels.value;
			}
			set
			{
				if( _displayLabels.BeginSet( ref value ) )
				{
					try { DisplayLabelsChanged?.Invoke( this ); }
					finally { _displayLabels.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DisplayLabels"/> property value changes.</summary>
		public event Action<Scene> DisplayLabelsChanged;

		/// <summary>
		/// Whether to display development data of the lights.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayLights
		{
			get { if( _displayLights.BeginGet() ) DisplayLights = _displayLights.Get( this ); return _displayLights.value; }
			set { if( _displayLights.BeginSet( ref value ) ) { try { DisplayLightsChanged?.Invoke( this ); } finally { _displayLights.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayLights"/> property value changes.</summary>
		public event Action<Scene> DisplayLightsChanged;
		ReferenceField<bool> _displayLights;

		/// <summary>
		/// Whether to display development data of the decals.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayDecals
		{
			get { if( _displayDecals.BeginGet() ) DisplayDecals = _displayDecals.Get( this ); return _displayDecals.value; }
			set { if( _displayDecals.BeginSet( ref value ) ) { try { DisplayDecalsChanged?.Invoke( this ); } finally { _displayDecals.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayDecals"/> property value changes.</summary>
		public event Action<Scene> DisplayDecalsChanged;
		ReferenceField<bool> _displayDecals;

		//DisplayReflectionProbes
		ReferenceField<bool> _displayReflectionProbes;
		/// <summary>
		/// Whether to display development data of the reflection probes.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayReflectionProbes
		{
			get
			{
				if( _displayReflectionProbes.BeginGet() )
					DisplayReflectionProbes = _displayReflectionProbes.Get( this );
				return _displayReflectionProbes.value;
			}
			set
			{
				if( _displayReflectionProbes.BeginSet( ref value ) )
				{
					try { DisplayReflectionProbesChanged?.Invoke( this ); }
					finally { _displayReflectionProbes.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DisplayReflectionProbes"/> property value changes.</summary>
		public event Action<Scene> DisplayReflectionProbesChanged;

		//DisplayCameras
		ReferenceField<bool> _displayCameras;
		/// <summary>
		/// Whether to display development data of the cameras.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplayCameras
		{
			get
			{
				if( _displayCameras.BeginGet() )
					DisplayCameras = _displayCameras.Get( this );
				return _displayCameras.value;
			}
			set
			{
				if( _displayCameras.BeginSet( ref value ) )
				{
					try { DisplayCamerasChanged?.Invoke( this ); }
					finally { _displayCameras.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DisplayCameras"/> property value changes.</summary>
		public event Action<Scene> DisplayCamerasChanged;

		/// <summary>
		/// Whether to display physical objects.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplayPhysicalObjects
		{
			get { if( _displayPhysicalObjects.BeginGet() ) DisplayPhysicalObjects = _displayPhysicalObjects.Get( this ); return _displayPhysicalObjects.value; }
			set { if( _displayPhysicalObjects.BeginSet( ref value ) ) { try { DisplayPhysicalObjectsChanged?.Invoke( this ); } finally { _displayPhysicalObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayPhysicalObjects"/> property value changes.</summary>
		public event Action<Scene> DisplayPhysicalObjectsChanged;
		ReferenceField<bool> _displayPhysicalObjects = false;

		/// <summary>
		/// Whether to display areas.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplayAreas
		{
			get { if( _displayAreas.BeginGet() ) DisplayAreas = _displayAreas.Get( this ); return _displayAreas.value; }
			set { if( _displayAreas.BeginSet( ref value ) ) { try { DisplayAreasChanged?.Invoke( this ); } finally { _displayAreas.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayAreas"/> property value changes.</summary>
		public event Action<Scene> DisplayAreasChanged;
		ReferenceField<bool> _displayAreas = false;

		/// <summary>
		/// Whether to display volumes.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplayVolumes
		{
			get { if( _displayVolumes.BeginGet() ) DisplayVolumes = _displayVolumes.Get( this ); return _displayVolumes.value; }
			set { if( _displayVolumes.BeginSet( ref value ) ) { try { DisplayVolumesChanged?.Invoke( this ); } finally { _displayVolumes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayVolumes"/> property value changes.</summary>
		public event Action<Scene> DisplayVolumesChanged;
		ReferenceField<bool> _displayVolumes = false;

		/// <summary>
		/// Whether to display sensors.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplaySensors
		{
			get { if( _displaySensors.BeginGet() ) DisplaySensors = _displaySensors.Get( this ); return _displaySensors.value; }
			set { if( _displaySensors.BeginSet( ref value ) ) { try { DisplaySensorsChanged?.Invoke( this ); } finally { _displaySensors.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplaySensors"/> property value changes.</summary>
		public event Action<Scene> DisplaySensorsChanged;
		ReferenceField<bool> _displaySensors = false;


		////DisplayPhysicsStatic
		//ReferenceField<bool> _displayPhysicsStatic;
		///// <summary>
		///// Whether to display development data of the static physical objects.
		///// </summary>
		//[Serialize]
		//[DefaultValue( false )]
		//[Category( "Development Data" )]
		//public Reference<bool> DisplayPhysicsStatic
		//{
		//	get
		//	{
		//		if( _displayPhysicsStatic.BeginGet() )
		//			DisplayPhysicsStatic = _displayPhysicsStatic.Get( this );
		//		return _displayPhysicsStatic.value;
		//	}
		//	set
		//	{
		//		if( _displayPhysicsStatic.BeginSet( ref value ) )
		//		{
		//			try { DisplayPhysicsStaticChanged?.Invoke( this ); }
		//			finally { _displayPhysicsStatic.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Scene> DisplayPhysicsStaticChanged;

		////DisplayPhysicsDynamic
		//ReferenceField<bool> _displayPhysicsDynamic;
		///// <summary>
		///// Whether to display development data of the dynamic physical objects.
		///// </summary>
		//[Serialize]
		//[DefaultValue( false )]
		//[Category( "Development Data" )]
		//public Reference<bool> DisplayPhysicsDynamic
		//{
		//	get
		//	{
		//		if( _displayPhysicsDynamic.BeginGet() )
		//			DisplayPhysicsDynamic = _displayPhysicsDynamic.Get( this );
		//		return _displayPhysicsDynamic.value;
		//	}
		//	set
		//	{
		//		if( _displayPhysicsDynamic.BeginSet( ref value ) )
		//		{
		//			try { DisplayPhysicsDynamicChanged?.Invoke( this ); }
		//			finally { _displayPhysicsDynamic.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Scene> DisplayPhysicsDynamicChanged;

		//DisplaySoundSources
		ReferenceField<bool> _displaySoundSources;
		/// <summary>
		/// Whether to display development data of the sound sources.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> DisplaySoundSources
		{
			get { if( _displaySoundSources.BeginGet() ) DisplaySoundSources = _displaySoundSources.Get( this ); return _displaySoundSources.value; }
			set { if( _displaySoundSources.BeginSet( ref value ) ) { try { DisplaySoundSourcesChanged?.Invoke( this ); } finally { _displaySoundSources.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplaySoundSources"/> property value changes.</summary>
		public event Action<Scene> DisplaySoundSourcesChanged;

		//DisplayObjectInSpaceBounds
		ReferenceField<bool> _displayObjectInSpaceBounds;
		/// <summary>
		/// Whether to display the bounds of the objects in space.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplayObjectInSpaceBounds
		{
			get
			{
				if( _displayObjectInSpaceBounds.BeginGet() )
					DisplayObjectInSpaceBounds = _displayObjectInSpaceBounds.Get( this );
				return _displayObjectInSpaceBounds.value;
			}
			set
			{
				if( _displayObjectInSpaceBounds.BeginSet( ref value ) )
				{
					try { DisplayObjectInSpaceBoundsChanged?.Invoke( this ); }
					finally { _displayObjectInSpaceBounds.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DisplayObjectInSpaceBounds"/> property value changes.</summary>
		public event Action<Scene> DisplayObjectInSpaceBoundsChanged;

		//DisplaySceneOctree
		ReferenceField<bool> _displaySceneOctree;
		/// <summary>
		/// Whether to display the scene octree.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		[Category( "Development Data" )]
		public Reference<bool> DisplaySceneOctree
		{
			get
			{
				if( _displaySceneOctree.BeginGet() )
					DisplaySceneOctree = _displaySceneOctree.Get( this );
				return _displaySceneOctree.value;
			}
			set
			{
				if( _displaySceneOctree.BeginSet( ref value ) )
				{
					try { DisplaySceneOctreeChanged?.Invoke( this ); }
					finally { _displaySceneOctree.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DisplaySceneOctree"/> property value changes.</summary>
		public event Action<Scene> DisplaySceneOctreeChanged;

		/// <summary>
		/// Enables the frustum culling test.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Development Data" )]
		public Reference<bool> FrustumCullingTest
		{
			get { if( _frustumCullingTest.BeginGet() ) FrustumCullingTest = _frustumCullingTest.Get( this ); return _frustumCullingTest.value; }
			set { if( _frustumCullingTest.BeginSet( ref value ) ) { try { FrustumCullingTestChanged?.Invoke( this ); } finally { _frustumCullingTest.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrustumCullingTest"/> property value changes.</summary>
		public event Action<Scene> FrustumCullingTestChanged;
		ReferenceField<bool> _frustumCullingTest = false;

		//!!!!Draw scene graphs
		//!!!!еще всякое: frustum test, Draw Shadow Debugging, portal system. 
		//!!!!еще выключать рисование чего-нибудь

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//OctreeEnabled
		ReferenceField<bool> _octreeEnabled = true;
		/// <summary>
		/// Enables the scene octree.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<bool> OctreeEnabled
		{
			get
			{
				if( _octreeEnabled.BeginGet() )
					OctreeEnabled = _octreeEnabled.Get( this );
				return _octreeEnabled.value;
			}
			set
			{
				if( _octreeEnabled.BeginSet( ref value ) )
				{
					try
					{
						OctreeEnabledChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeEnabled.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeEnabled"/> property value changes.</summary>
		public event Action<Scene> OctreeEnabledChanged;

		//OctreeObjectCountOutsideOctreeToRebuld
		ReferenceField<int> _octreeObjectCountOutsideOctreeToRebuld = 50;
		/// <summary>
		/// The number of objects to rebuild outside the octree.
		/// </summary>
		[DefaultValue( 50 )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<int> OctreeObjectCountOutsideOctreeToRebuld
		{
			get
			{
				if( _octreeObjectCountOutsideOctreeToRebuld.BeginGet() )
					OctreeObjectCountOutsideOctreeToRebuld = _octreeObjectCountOutsideOctreeToRebuld.Get( this );
				return _octreeObjectCountOutsideOctreeToRebuld.value;
			}
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _octreeObjectCountOutsideOctreeToRebuld.BeginSet( ref value ) )
				{
					try
					{
						OctreeObjectCountOutsideOctreeToRebuldChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeObjectCountOutsideOctreeToRebuld.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeObjectCountOutsideOctreeToRebuld"/> property value changes.</summary>
		public event Action<Scene> OctreeObjectCountOutsideOctreeToRebuldChanged;

		//OctreeBoundsRebuildExpand
		ReferenceField<Vector3> _octreeBoundsRebuildExpand = new Vector3( 100, 100, 100 );
		/// <summary>
		/// The expand vector of the octree bounds.
		/// </summary>
		[DefaultValue( "100 100 100" )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<Vector3> OctreeBoundsRebuildExpand
		{
			get
			{
				if( _octreeBoundsRebuildExpand.BeginGet() )
					OctreeBoundsRebuildExpand = _octreeBoundsRebuildExpand.Get( this );
				return _octreeBoundsRebuildExpand.value;
			}
			set
			{
				var v = value.Value;
				if( v.X < 0 || v.Y < 0 || v.Z < 0 )
				{
					if( v.X < 0 ) v.X = 0;
					if( v.Y < 0 ) v.Y = 0;
					if( v.Z < 0 ) v.Z = 0;
					value = new Reference<Vector3>( v, value.GetByReference );
				}
				if( _octreeBoundsRebuildExpand.BeginSet( ref value ) )
				{
					try
					{
						OctreeBoundsRebuildExpandChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeBoundsRebuildExpand.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeBoundsRebuildExpand"/> property value changes.</summary>
		public event Action<Scene> OctreeBoundsRebuildExpandChanged;

		//OctreeMinNodeSize
		ReferenceField<Vector3> _octreeMinNodeSize = new Vector3( 10, 10, 10 );
		/// <summary>
		/// The minimum node size of the octree.
		/// </summary>
		[DefaultValue( "10 10 10" )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<Vector3> OctreeMinNodeSize
		{
			get
			{
				if( _octreeMinNodeSize.BeginGet() )
					OctreeMinNodeSize = _octreeMinNodeSize.Get( this );
				return _octreeMinNodeSize.value;
			}
			set
			{
				var v = value.Value;
				if( v.X < 1 || v.Y < 1 || v.Z < 1 )
				{
					if( v.X < 1 ) v.X = 1;
					if( v.Y < 1 ) v.Y = 1;
					if( v.Z < 1 ) v.Z = 1;
					value = new Reference<Vector3>( v, value.GetByReference );
				}
				if( _octreeMinNodeSize.BeginSet( ref value ) )
				{
					try
					{
						OctreeMinNodeSizeChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeMinNodeSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeMinNodeSize"/> property value changes.</summary>
		public event Action<Scene> OctreeMinNodeSizeChanged;

		//OctreeObjectCountToCreateChildNodes
		ReferenceField<int> _octreeObjectCountToCreateChildNodes = 50;
		/// <summary>
		/// The number of objects needed to create child nodes.
		/// </summary>
		[DefaultValue( 50 )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<int> OctreeObjectCountToCreateChildNodes
		{
			get
			{
				if( _octreeObjectCountToCreateChildNodes.BeginGet() )
					OctreeObjectCountToCreateChildNodes = _octreeObjectCountToCreateChildNodes.Get( this );
				return _octreeObjectCountToCreateChildNodes.value;
			}
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _octreeObjectCountToCreateChildNodes.BeginSet( ref value ) )
				{
					try
					{
						OctreeObjectCountToCreateChildNodesChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeObjectCountToCreateChildNodes.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeObjectCountToCreateChildNodes"/> property value changes.</summary>
		public event Action<Scene> OctreeObjectCountToCreateChildNodesChanged;

		//OctreeMaxNodeCount
		ReferenceField<int> _octreeMaxNodeCount = 100000;
		/// <summary>
		/// The maximum number of nodes created by the octree.
		/// </summary>
		[DefaultValue( 100000 )]
		[Serialize]
		[Category( "Octree" )]
		public Reference<int> OctreeMaxNodeCount
		{
			get
			{
				if( _octreeMaxNodeCount.BeginGet() )
					OctreeMaxNodeCount = _octreeMaxNodeCount.Get( this );
				return _octreeMaxNodeCount.value;
			}
			set
			{
				if( value < 10 )
					value = new Reference<int>( 10, value.GetByReference );
				if( _octreeMaxNodeCount.BeginSet( ref value ) )
				{
					try
					{
						OctreeMaxNodeCountChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeMaxNodeCount.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeMaxNodeCount"/> property value changes.</summary>
		public event Action<Scene> OctreeMaxNodeCountChanged;

		//!!!!default
		[Category( "Octree" )]
		[DefaultValue( OctreeContainer.ThreadingModeEnum.SingleThreaded )]//BackgroundThread )]
		public Reference<OctreeContainer.ThreadingModeEnum> OctreeThreadingMode
		{
			get { if( _octreeThreadingMode.BeginGet() ) OctreeThreadingMode = _octreeThreadingMode.Get( this ); return _octreeThreadingMode.value; }
			set
			{
				if( _octreeThreadingMode.BeginSet( ref value ) )
				{
					try
					{
						OctreeThreadingModeChanged?.Invoke( this );
						OctreeUpdate( true );// EngineApp.IsEditor );
					}
					finally { _octreeThreadingMode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OctreeThreadingMode"/> property value changes.</summary>
		public event Action<Scene> OctreeThreadingModeChanged;
		ReferenceField<OctreeContainer.ThreadingModeEnum> _octreeThreadingMode = OctreeContainer.ThreadingModeEnum.SingleThreaded;//BackgroundThread;

		[Category( "MeshInSpace Static Mode" )]
		[DefaultValue( "150 150 10000" )]
		public Reference<Vector3> MeshInSpaceStaticModeSectorSize
		{
			get { if( _meshInSpaceStaticModeSectorSize.BeginGet() ) MeshInSpaceStaticModeSectorSize = _meshInSpaceStaticModeSectorSize.Get( this ); return _meshInSpaceStaticModeSectorSize.value; }
			set
			{
				if( _meshInSpaceStaticModeSectorSize.BeginSet( ref value ) )
				{
					try
					{
						MeshInSpaceStaticModeSectorSizeChanged?.Invoke( this );

						GroupOfObjectsUtility.UpdateGroupOfObjects( this, "__GroupOfObjectsMeshInSpaceStatic", MeshInSpaceStaticModeSectorSize, MeshInSpaceStaticModeMaxObjectsInGroup );
					}
					finally { _meshInSpaceStaticModeSectorSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MeshInSpaceStaticModeSectorSize"/> property value changes.</summary>
		public event Action<Scene> MeshInSpaceStaticModeSectorSizeChanged;
		ReferenceField<Vector3> _meshInSpaceStaticModeSectorSize = new Vector3( 150, 150, 10000 );

		[Category( "MeshInSpace Static Mode" )]
		[DefaultValue( 1000 )]
		public Reference<int> MeshInSpaceStaticModeMaxObjectsInGroup
		{
			get { if( _meshInSpaceStaticModeMaxObjectsInGroup.BeginGet() ) MeshInSpaceStaticModeMaxObjectsInGroup = _meshInSpaceStaticModeMaxObjectsInGroup.Get( this ); return _meshInSpaceStaticModeMaxObjectsInGroup.value; }
			set
			{
				if( _meshInSpaceStaticModeMaxObjectsInGroup.BeginSet( ref value ) )
				{
					try
					{
						MeshInSpaceStaticModeMaxObjectsInGroupChanged?.Invoke( this );

						GroupOfObjectsUtility.UpdateGroupOfObjects( this, "__GroupOfObjectsMeshInSpaceStatic", MeshInSpaceStaticModeSectorSize, MeshInSpaceStaticModeMaxObjectsInGroup );
					}
					finally { _meshInSpaceStaticModeMaxObjectsInGroup.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MeshInSpaceStaticModeMaxObjectsInGroup"/> property value changes.</summary>
		public event Action<Scene> MeshInSpaceStaticModeMaxObjectsInGroupChanged;
		ReferenceField<int> _meshInSpaceStaticModeMaxObjectsInGroup = 1000;



		///// <summary>
		///// Whether to display the internal physics data.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Debug" )]
		//public Reference<bool> DisplayPhysicsInternal
		//{
		//	get { if( _displayPhysicsInternal.BeginGet() ) DisplayPhysicsInternal = _displayPhysicsInternal.Get( this ); return _displayPhysicsInternal.value; }
		//	set { if( _displayPhysicsInternal.BeginSet( ref value ) ) { try { DisplayPhysicsInternalChanged?.Invoke( this ); } finally { _displayPhysicsInternal.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="DisplayPhysicsInternal"/> property value changes.</summary>
		//public event Action<Scene> DisplayPhysicsInternalChanged;
		//ReferenceField<bool> _displayPhysicsInternal;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( OctreeObjectCountOutsideOctreeToRebuld ):
				case nameof( OctreeBoundsRebuildExpand ):
				case nameof( OctreeMinNodeSize ):
				case nameof( OctreeObjectCountToCreateChildNodes ):
				case nameof( OctreeMaxNodeCount ):
				case nameof( OctreeThreadingMode ):
					if( !OctreeEnabled )
						skip = true;
					break;

				case nameof( PhysicsDefaultConvexRadius ):
				case nameof( PhysicsMaxBodyPairs ):
				case nameof( PhysicsMaxContactConstraints ):
				case nameof( PhysicsCollisionSteps ):
				case nameof( PhysicsIntegrationSubSteps ):
				case nameof( PhysicsMaxInFlightBodyPairs ):
				case nameof( PhysicsStepListenersBatchSize ):
				case nameof( PhysicsStepListenerBatchesPerJob ):
				case nameof( PhysicsBaumgarte ):
				case nameof( PhysicsSpeculativeContactDistance ):
				case nameof( PhysicsPenetrationSlop ):
				case nameof( PhysicsLinearCastThreshold ):
				case nameof( PhysicsLinearCastMaxPenetration ):
				case nameof( PhysicsManifoldTolerance ):
				case nameof( PhysicsMaxPenetrationDistance ):
				case nameof( PhysicsBodyPairCacheMaxDeltaPosition ):
				case nameof( PhysicsBodyPairCacheCosMaxDeltaRotationDiv2 ):
				case nameof( PhysicsContactNormalCosMaxDeltaRotation ):
				case nameof( PhysicsContactPointPreserveLambdaMaxDist ):
				case nameof( PhysicsNumVelocitySteps ):
				case nameof( PhysicsNumPositionSteps ):
				case nameof( PhysicsMinVelocityForRestitution ):
				case nameof( PhysicsTimeBeforeSleep ):
				case nameof( PhysicsPointVelocitySleepThreshold ):
				case nameof( PhysicsConstraintWarmStart ):
				case nameof( PhysicsUseBodyPairContactCache ):
				case nameof( PhysicsUseManifoldReduction ):
				case nameof( PhysicsAllowSleeping ):
				case nameof( PhysicsCheckActiveEdges ):
					if( !PhysicsAdvancedSettings )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!weather settings. скорость ветра. для травы и флагов из варкрафта


		//!!!!parameter: OptimizationSettings (или прост Optimization)




		//!!!!!
		//входы в мир компонент:
		//ViewportUpdate. ему нужна камера и список объектов. по сути ему их нужно предоставить как результат.
		//SimulationTick. этому ничего не нужно. просто кому нужен





		//!!!!что с полностью независимой загрузкой карт? типы там и т.д.

		//!!!!!сделать класс MapManagement для управления ей? или просто методы с префиксом MapManagement_PerformXXXX. лучше скрыть их по сути за классом.

		//!!!!!!ideas: можно чтобы в папка карты все ресурсы сохранялись с префиксом имени карты. тогда не будет пересечений.

		//internal SimulationTypes simulationType;
		//!!!!!заюзать
		//internal List<Assembly> additionalClassAssemblies = new List<Assembly>();

		//!!!!
		//internal SimulationStatuses simulationStatus = SimulationStatuses.StillNotSimulated;

		//!!!!!
		////Physics
		//PhysicsScene physicsScene;

		//file name
		//!!!!!
		//internal string fileName = "";
		//!!!!!или всегда настроенной хранить
		//!!!!![Serialize( "sourceMapFileName", SerializeAttribute.SerializationModes.AlreadySimulated )]
		//string sourceMapFileName = "";

		// ticks
		//EDictionary<MapObject, int> objectsSubscribedToTicks = new EDictionary<MapObject, int>();

		//!!!!!
		////Ticks
		////!!!!!почему тут. карте и компонентам тоже полезно
		//internal static double simulationTickDelta = 1.0 / MapSystemWorld.SimulationTicksPerSecond;
		//double simulationTickTime;

		//!!!!!!нужно для оптимизации
		//ESet<MapObject> simulationTickSubscribedObjects = new ESet<MapObject>();

		//!!!!
		////Flags
		//bool simulationEnabled;
		//bool systemPauseOfSimulationEnabled;

		//!!!!!
		////render time
		//double lastRenderTime;
		//double lastRenderTimeStep;

		//!!!!!
		////!!!!!так? не нативные объекты будут?
		////!!!!!!обновлять только нужное
		////Zones, portals, occluders
		//internal List<Zone> zones = new List<Zone>();
		//internal List<Portal> portals = new List<Portal>();
		//internal List<Occluder> occluders = new List<Occluder>();
		//internal bool needUpdatePortalZones;

		//!!!!!
		//static PerformanceCounter.TimeCounter sceneManagementPerformanceCounter =
		//   new PerformanceCounter.TimeCounter( "Scene Management", true, new ColorValue( 0, 0, 1 ), .1f );

		//!!!!!
		//Bounds initialCollisionBounds;

		//!!!!!!
		//!!!!!xx xx;
		//bool unhideLayersInSimulation;

		//!!!!!!было
		//internal NetworkingInterface networkingInterface;
		//List<RemoteEntityWorld> remoteEntityWorlds = new List<RemoteEntityWorld>();
		//ReadOnlyCollection<RemoteEntityWorld> remoteEntityWorldsReadOnly;
		//int networkTickCounter;
		//double client_tickTime;
		//double client_timeWhenTickTimeWasUpdated;

		//!!!!!!
		//internal StaticBatchingRenderingManager staticBatchingRenderingManager;
		//internal StaticBatchingCollisionManager staticBatchingCollisionManager;

		///////////////////////////////////////////

		//!!!!!было
		//enum NetworkMessages
		//{
		//   VirtualFileNameToClient,
		//}

		///////////////////////////////////////////

		//!!!!!!
		////!!!!Flags?
		///// <summary>
		///// Specifies the world simulation types.
		///// </summary>
		//[Flags]
		//public enum SimulationTypes
		//{
		//	/// <summary>
		//	/// The map of this type is created on a server. A network mode.
		//	/// </summary>
		//	DedicatedServer,
		//	/// <summary>
		//	/// The map of this type is created on a server and working as client too. A network mode.
		//	/// </summary>
		//	ServerAndClient,
		//	/// <summary>
		//	/// The map of this type is created on a client. A network mode.
		//	/// </summary>
		//	ClientOnly,
		//	/// <summary>
		//	/// The map of this type is created as a single simulation.
		//	/// </summary>
		//	Single,
		//	/// <summary>
		//	/// The map of this type is created in the NeoAxis Engine.
		//	/// </summary>
		//	Editor,
		//}

		///////////////////////////////////////////

		//!!!!!!
		//public enum SimulationStatuses
		//{
		//	StillNotSimulated,
		//	AlreadySimulated,
		//}

		///////////////////////////////////////////

		public Scene()
		{
			lock( all )
				all.Add( this );

			//!!!!

			//simulationTickTime = EngineApp.Instance.Time;


			//sceneGraphSettings.map = this;
			//shadowSettings.map = this;
			//staticBatchingSettings.map = this;

			//simulationTickTime = EngineApp.Instance.Time;

			////physics scene
			////!!!!!!указывать количество тредов
			//physicsScene = PhysicsWorld.Instance.CreateScene( string.Format( "Scene for map \"{0}\"", Name ) );

			////mapObjectTypes = new MapObjectTypesClass();
			////mapObjectTypes.map = this;
			////mapObjectTypes.Init();

			////!!!!тут?
			//SceneObjects_Init();
			//MapObjects_Init();

			//!!!!!
			//rootLayer = new LayerClass( this, null );

			//if( needSetVirtualFileName != null )
			//   virtualFileName = needSetVirtualFileName;

			//!!!!!
			//staticBatchingRenderingManager = new StaticBatchingRenderingManager( staticBatchingGridCellSize );
			//staticBatchingCollisionManager = new StaticBatchingCollisionManager( staticBatchingGridCellSize );

			//TransformEnabled = false;
		}

		//#region ViewportUpdate

		//!!!!!
		//void BeforeViewportUpdate()
		//{
		////update batching manager
		//StaticBatching_UpdateRenderingBatches( false );
		//StaticBatching_UpdateCollisionBatches( false );

		//!!!!!было
		//AutoDeleteSceneObjects_BeforeViewportUpdate();

		//OnBeginRenderFrame();
		//}

		//!!!!!
		//protected override void OnViewportUpdate( Viewport viewport )
		//{
		//	base.OnViewportUpdate( viewport );

		//	//!!!!было
		//	//if( needUpdatePortalZones )
		//	//   UpdatePortalZones();

		//	//!!!!!надо. тут?
		//	////debug draw physics for StaticMesh'es with static batching
		//	//if( EngineDebugSettings.DrawStaticPhysics && camera.Purpose == RenderCamera.Purposes.MainCamera )
		//	//   staticBatchingCollisionManager.DebugDrawStaticPhysics( camera );
		//}

		//!!!!!
		//public delegate void Client_MapLoadingDelegate();
		//public static event Client_MapLoadingDelegate Client_MapLoadingBegin;
		//public static event Client_MapLoadingDelegate Client_MapLoadingEnd;

		//!!!!temp. flares
		///// <summary>
		///// Render lens flares and debug information to gui renderer.
		///// </summary>
		///// <param name="renderer">The destination gui renderer.</param>
		//public void XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX( GuiRenderer renderer )
		//{
		//   if( !LongOperationCallbackManager.IsSubscribed && !MapSystemWorld.MapLoading )
		//   {
		//      //!!!!!флары надо рисовать до гуя, но при этом с применением постэффектов (каких-то только).
		//      if( EngineDebugSettings.DrawFullScreenEffects )
		//      {
		//         if( LensFlareManager.Instance != null && LensFlareManager.Instance.Enabled )
		//            LensFlareManager.Instance.Draw( renderer );
		//      }
		//   }
		//}

		//!!!!!нужное внутри метода
		///// <summary>
		///// Called when has occured Tick in the world. 
		///// </summary>
		///// <remarks>
		///// <para>This method will be caused, only if the entity is signed on timer events. To subscribe for timer events it is possible by means of method <see cref="NeoAxis.MapObject.SubscribeToTickEvent()"/>.</para>
		///// </remarks>
		///// <seealso cref="NeoAxis.MapObject.SubscribeToTickEvent()"/>
		///// <seealso cref="NeoAxis.MapObject.UnsubscribeToTickEvent()"/>
		///// <seealso cref="NeoAxis.MapObject.SimulationTickDelta"/>
		///// <seealso cref="NeoAxis.MapObject.SimulationTick"/>
		///// <seealso cref="NeoAxis.MapSystemWorld.SimulationTicksPerSecond"/>
		//protected override void OnTick()
		//{
		//	base.OnTick();

		//	StaticBatching_UpdateCollisionBatches( false );
		//	//!!!!!было
		//	//AutoDeleteSceneObjects_Tick( TickDelta );
		//}

		//!!!!!
		//protected override void Client_OnTick()
		//{
		//	base.Client_OnTick();

		//	StaticBatching_UpdateCollisionBatches( false );
		//	//!!!!!было
		//	//AutoDeleteSceneObjects_Tick( TickDelta );
		//}

		//!!!!!
		//void UpdateSceneGraphsSettings( bool forceTreeRebuild )
		//{
		//	if( sceneObjectsSceneGraphContainer != null )
		//	{
		//		sceneObjectsSceneGraphContainer.UpdateSettings( sceneGraphSettings.AmountOfObjectsOutsideOctreeBoundsToRebuld,
		//			sceneGraphSettings.OctreeBoundsRebuildExpand, sceneGraphSettings.MinNodeSize,
		//			sceneGraphSettings.ObjectCountThresholdToCreateChildNodes, sceneGraphSettings.MaxNodeCount, forceTreeRebuild );
		//	}
		//	//sceneObjects.sceneGraphLightsContainer.UpdateSettings( sceneGraphSettings.AmountOfObjectsOutsideOctreeBoundsToRebuld,
		//	//	sceneGraphSettings.OctreeBoundsRebuildExpand, sceneGraphSettings.MinNodeSize,
		//	//	sceneGraphSettings.ObjectCountThresholdToCreateChildNodes, sceneGraphSettings.MaxNodeCount, forceTreeRebuild );

		//	if( mapObjectsSceneGraphContainer != null )
		//	{
		//		mapObjectsSceneGraphContainer.UpdateSettings( sceneGraphSettings.AmountOfObjectsOutsideOctreeBoundsToRebuld,
		//			sceneGraphSettings.OctreeBoundsRebuildExpand, sceneGraphSettings.MinNodeSize,
		//			sceneGraphSettings.ObjectCountThresholdToCreateChildNodes, sceneGraphSettings.MaxNodeCount, forceTreeRebuild );
		//	}
		//}

		//!!!!!!
		//protected override bool OnLoad( TextBlock block, ref string error )
		//{
		//	if( !base.OnLoad( block, ref error ) )
		//		return false;

		//	//!!!!!
		//	if( SimulationStatus == SimulationStatuses.StillNotSimulated )
		//		sourceMapFileName = FileName;

		//	//!!!!!
		//	////layers
		//	//if( !Layers_OnLoad( block ) )
		//	//	return false;
		//	//settings
		//	if( !Settings_OnLoad( block ) )
		//		return false;

		//	//savedRealFileDirectory = GetRealFileDirectory();

		//	//!!!!!тут?
		//	UpdateSceneGraphsSettings( true );
		//	UpdateSoundSettings();

		//	return true;
		//}

		//!!!!!!
		//protected override bool OnSave( TextBlock block, ref bool canSave, out string errorString )
		//{
		//	if( !base.OnSave( block, ref canSave, out errorString ) )
		//		return false;

		//	//!!!!объекты

		//	//!!!!было
		//	//if( EntitySystemWorld.Instance.SerializationMode == SerializationModes.MapSceneFile )
		//	//{
		//	//   Log.Warning( "Scene export: Map is not supported." );
		//	//   return;
		//	//}

		//	if( SimulationStatus == SimulationStatuses.StillNotSimulated )
		//		sourceMapFileName = FileName;

		//	//!!!!!
		//	////layers
		//	//Layers_OnSave( block );
		//	//setting
		//	//!!!!
		//	Settings_OnSave( block );

		//	//StaticMesh_OnSave( block );

		//	return true;
		//}

		//void UpdatePortalZones()
		//{
		//	//!!!!!было

		//	////!!!!!slowly всем сразу всегда обновлять. жесть жеж? или если Enabled менять, то норм. но не двигать

		//	//foreach( Zone zone in zones )
		//	//   zone.Portals.Clear();
		//	//foreach( Portal portal in portals )
		//	//   portal.UpdateZones();

		//	//needUpdatePortalZones = false;
		//}

		//[Browsable( false )]
		//public string FileName
		//{
		//	get { return fileName; }
		//}

		//!!!!!!
		///// <summary>
		///// Returns the virtual file directory name of a map.
		///// </summary>
		///// <returns>The virtual file directory name of a map.</returns>
		//public string GetFileDirectory_Virtual()
		//{
		//	if( !string.IsNullOrEmpty( FileName ) )
		//		return Path.GetDirectoryName( FileName );
		//	return "";
		//}

		//!!!!!!
		//public string GetFileDirectory_Real()
		//{
		//	if( !string.IsNullOrEmpty( FileName ) )
		//		return PathUtils.GetRealPathByVirtual( GetFileDirectory_Virtual() );
		//	return "";
		//}

		//!!!!!directory? тогда кучи методов тут не будет.
		//!!!!!!!Save [As] изменится и создание карты
		///// <summary>
		///// Gets the virtual file path of a source map before world serialization.
		///// </summary>
		//[Browsable( false )]
		//public string SourceMapFileName
		//{
		//	get { return sourceMapFileName; }
		//}

		///// <summary>
		///// Returns the virtual file directory name of a source map before world serialization.
		///// </summary>
		///// <returns>The virtual file directory name of a map.</returns>
		//public string GetSourceMapVirtualFileDirectory()
		//{
		//   if( !string.IsNullOrEmpty( SourceMapVirtualFileName ) )
		//      return Path.GetDirectoryName( SourceMapVirtualFileName );
		//   return "";
		//}

		//static DateTime GetVirtualFileTimeIncludingCheckingInsideArchive( string virtualFileName )
		//{
		//   string realFileName = PathUtils.GetRealPathByVirtual( virtualFileName );

		//   if( File.Exists( realFileName ) )
		//   {
		//      return File.GetLastWriteTime( realFileName );
		//   }
		//   else
		//   {
		//      ArchiveManager.FileInfo fileInfo;
		//      if( ArchiveManager.Instance.GetFileInfo( virtualFileName, out fileInfo ) )
		//      {
		//         return File.GetLastWriteTime( fileInfo.Archive.FileName );
		//      }
		//      else
		//      {
		//         throw new Exception( "Map: GetVirtualFileTimeIncludingCheckingInsideArchive: Internal error." );
		//      }
		//   }
		//}

		//!!!!
		///// <summary>
		///// Don't modify.
		///// </summary>
		//[Browsable( false )]
		//public List<Zone> Zones
		//{
		//   get { return zones; }
		//}

		//!!!!
		///// <summary>
		///// Don't modify.
		///// </summary>
		//[Browsable( false )]
		//public List<Portal> Portals
		//{
		//   get { return portals; }
		//}

		//!!!!!!
		///// <summary>
		///// Gets the bounds of all shapes with <see cref="ContactGroup.Collision">Collision</see>
		///// contact group at creating map.
		///// </summary>
		//[Browsable( false )]
		//public Bounds InitialCollisionBounds
		//{
		//	get { return initialCollisionBounds; }
		//}

		//!!!!!было
		//void Server_SendVirtualFileNameToClient( RemoteEntityWorld remoteEntityWorld )
		//{
		//   SendDataWriter writer = BeginNetworkMessage( remoteEntityWorld, typeof( Map ),
		//      (ushort)NetworkMessages.VirtualFileNameToClient );
		//   writer.Write( VirtualFileName );
		//   EndNetworkMessage();
		//}

		//!!!!!было
		//[NetworkReceive( NetworkDirections.ToClient, (ushort)NetworkMessages.VirtualFileNameToClient )]
		//void Client_ReceiveVirtualFileName( RemoteEntityWorld sender, ReceiveDataReader reader )
		//{
		//   string value = reader.ReadString();
		//   if( !reader.Complete() )
		//      return;

		//   if( Client_MapLoadingBegin != null )
		//      Client_MapLoadingBegin();

		//   bool success = MapSystemWorld.Client_LoadMapForClient( value );

		//   if( Client_MapLoadingEnd != null )
		//      Client_MapLoadingEnd();
		//}

		//!!!!!было
		//protected internal override void Server_OnClientConnectedBeforePostCreate( RemoteEntityWorld remoteEntityWorld )
		//{
		//   base.Server_OnClientConnectedBeforePostCreate( remoteEntityWorld );

		//   if( !string.IsNullOrEmpty( VirtualFileName ) )
		//      Server_SendVirtualFileNameToClient( remoteEntityWorld );
		//}

		//public void StaticBatching_UpdateRenderingBatches( bool forceFullUpdate )
		//{
		//	//DateTime t = DateTime.Now;

		//	//!!!!!!
		//	//staticBatchingRenderingManager.UpdateBatches( forceFullUpdate );

		//	//TimeSpan ts = DateTime.Now - t;
		//	//if( ts.TotalMilliseconds > 2 )
		//	//   Log.Info( "rendering time: " + ts.TotalSeconds );
		//}

		//public void StaticBatching_UpdateCollisionBatches( bool forceFullUpdate )
		//{
		//	//DateTime t = DateTime.Now;

		//	//!!!!!!
		//	//staticBatchingCollisionManager.UpdateBatches( forceFullUpdate );

		//	//TimeSpan ts = DateTime.Now - t;
		//	//if( ts.TotalMilliseconds > 2 )
		//	//   Log.Info( "collision time: " + ts.TotalSeconds );
		//}

		//!!!!!

		//[Browsable( false )]
		//public SimulationTypes SimulationType
		//{
		//	get { return simulationType; }
		//}

		//public bool SimulationType_IsServer()
		//{
		//	return simulationType == SimulationTypes.DedicatedServer || simulationType == SimulationTypes.ServerAndClient;
		//}

		//[Browsable( false )]
		//public SimulationStatuses SimulationStatus
		//{
		//	get { return simulationStatus; }
		//}

		///// <summary>
		///// Gets or sets a value indicating whether the simulation is enabled.
		///// </summary>
		//[Browsable( false )]
		//public bool SimulationEnabled
		//{
		//	get { return simulationEnabled; }
		//	set
		//	{
		//		if( SimulationType == SimulationTypes.ClientOnly )
		//		{
		//			Log.Warning( "Map: set SimulationEnabled: \"SimulationEnabled\" flag cannot be changed on the client in networking mode." );
		//			return;
		//		}

		//		SetSimulationInternal( value );
		//	}
		//}

		///// <summary>
		///// Represents the method that handles a <see cref="SimulationEnabledChanged"/> event.
		///// </summary>
		///// <param name="map">The map object.</param>
		//public delegate void SimulationEnabledChangedDelegate( Map map );

		////!!!!name Was?
		///// <summary>
		///// Occurs when the object has been added to scene graph.
		///// </summary>
		//public event SimulationEnabledChangedDelegate SimulationEnabledChanged;


		//void SetSimulationInternal( bool value )
		//{
		//	if( simulationEnabled == value )
		//		return;

		//	simulationEnabled = value;
		//	simulationTickTime = EngineApp.Instance.Time;

		//	SimulationEnabledChanged?.Invoke( this );

		//	//!!!!было
		//	//if( SimulationType_IsServer() && networkingInterface != null )
		//	//{
		//	//   networkingInterface.Server_SendChangeSimulationFlagMessage( Instance.RemoteEntityWorlds );
		//	//}

		//	//update enable time progress
		//	if( SimulationType != SimulationTypes.Editor )
		//	{
		//		//!!!!!!!рендер всегда должен идти. а вот как управлять объектами - уже другой вопрос
		//		if( RendererWorld.Instance != null )
		//			RendererWorld.EnableTimeProgress = simulationEnabled && !systemPauseOfSimulationEnabled;
		//	}
		//}

		//[Browsable( false )]
		//public bool SystemPauseOfSimulationEnabled
		//{
		//	get { return systemPauseOfSimulationEnabled; }
		//	set
		//	{
		//		if( systemPauseOfSimulationEnabled == value )
		//			return;

		//		systemPauseOfSimulationEnabled = value;

		//		simulationTickTime = EngineApp.Instance.Time;

		//		//update enable time progress
		//		if( SimulationType != SimulationTypes.Editor )
		//		{
		//			if( RendererWorld.Instance != null )
		//				RendererWorld.EnableTimeProgress = simulationEnabled && !systemPauseOfSimulationEnabled;
		//		}
		//	}
		//}

		//!!!!!!
		//void TickOneStep()
		//{
		//	//send WorldTick message
		//	if( SimulationType_IsServer() )
		//	{
		//		//!!!!было
		//		//networkTickCounter++;
		//		//networkingInterface.SendWorldTickMessage();
		//	}

		//	//physics world tick
		//	if( SimulationType != SimulationTypes.ClientOnly )
		//	{
		//		//physicsPerformanceCounter.Start();
		//		physicsScene.Simulate( SimulationTickDelta );
		//		//physicsPerformanceCounter.End();
		//	}

		//	//timer ticks
		//	//entitySystemPerformanceCounter.Start();

		//	//MapObjects.PerformSimulationTick( simulationTickExecutedTime, SimulationType == SimulationTypes.ClientOnly );
		//	{
		//		SimulationTick?.Invoke( this );

		//		//MapObject[] array = new MapObject[ objectsSubscribedToTicks.Count ];
		//		//objectsSubscribedToTicks.Keys.CopyTo( array, 0 );

		//		//foreach( MapObject obj in array )
		//		//{
		//		//	if( !obj.IsMustDestroy && obj.CreateTime != simulationTickTime )
		//		//	{
		//		//		//!!!!slowly. можно иметь ESet удаленных
		//		//		if( objectsSubscribedToTicks.ContainsKey( obj ) )
		//		//		{
		//		//			if( !clientTick )
		//		//				obj.CallTick();
		//		//			else
		//		//				obj.Client_CallTick();
		//		//		}
		//		//	}
		//		//}
		//	}

		//	//entitySystemPerformanceCounter.End();

		//	//!!!!!так?
		//	//!!!!где еще выывать?
		//	_ProcessRootObjectsDeletionQueue();
		//}

		//void DoClientTick( int tickCounter )
		//{
		//!!!!!было

		//networkTickCounter = tickCounter;

		//double time = EngineApp.Instance.Time;

		//double serverTickTime = ( (double)networkTickCounter ) * Entity.TickDelta;

		////update client_tickTime

		//client_timeWhenTickTimeWasUpdated = time;
		//client_tickTime += Entity.TickDelta;

		////very big delay between ticks
		//if( client_tickTime > serverTickTime + .5f )
		//   client_tickTime = serverTickTime + .5f;
		////big delay between ticks
		//if( client_tickTime > serverTickTime + Entity.TickDelta * 2 )
		//{
		//   double diff = ( client_tickTime - serverTickTime ) * .05f;
		//   client_tickTime -= diff;
		//}
		//if( client_tickTime < serverTickTime )
		//   client_tickTime = serverTickTime;

		//executedTimeForTicks = time;
		//TickOneStep();
		//}

		///// <summary>
		///// To execute all ticks till current time of simulation.
		///// </summary>
		//public void ProcessSimulationTicks()
		//{
		//	//!!!!!вызывать

		//	//!!!!!тут?

		//	//!!!!можно было бы: чтобы можно было вне зависимости от времени прокрутить. тогда это значит что в коде и классах объектов не должно быть EngineApp.Instance.Time.

		//	//!!!!!когда вызывать?
		//	//!!!!!вызывать

		//	simulationStatus = SimulationStatuses.AlreadySimulated;

		//	if( SimulationType == SimulationTypes.ClientOnly )
		//		return;

		//	//!!!!!
		//	double time = EngineApp.Instance.Time;

		//	if( !simulationEnabled || systemPauseOfSimulationEnabled )
		//	{
		//		simulationTickTime = time;
		//		return;
		//	}

		//	while( time > simulationTickTime + SimulationTickDelta )
		//	{
		//		simulationTickTime += SimulationTickDelta;
		//		TickOneStep();
		//	}
		//}

		///// <summary>
		///// To reset current simulation time.
		///// </summary>
		///// <remarks>
		///// This method need call, after there was a long loading.
		///// This method is called, that the timer did not try to catch up with lagged behind time.
		///// </remarks>
		//public void ResetExecutedTimeForTicks()
		//{
		//	simulationTickTime = EngineApp.Instance.Time;

		//	lastRenderTime = simulationTickTime;
		//	lastRenderTimeStep = 0;
		//}

		//!!!!!было
		//public IList<RemoteEntityWorld> RemoteEntityWorlds
		//{
		//   get { return remoteEntityWorldsReadOnly; }
		//}

		//!!!!!было
		//public NetworkingInterface _GetNetworkingInterface()
		//{
		//   return networkingInterface;
		//}

		///// <summary>
		///// You can specify this value in the Base\Constants\EntitySystem.config. "defaultWorldType" attribute.
		///// </summary>
		//public WorldType DefaultWorldType
		//{
		//   get { return defaultWorldType; }
		//}

		//!!!!!было
		//public int NetworkTickCounter
		//{
		//   get { return networkTickCounter; }
		//}

		//!!!!!было
		//public float Client_TickTime
		//{
		//   get { return client_tickTime; }
		//}

		//!!!!!было
		//public float Client_TimeWhenTickTimeWasUpdated
		//{
		//   get { return client_timeWhenTickTimeWasUpdated; }
		//}

		//!!!!!
		//[Browsable( false )]
		//public PhysicsScene PhysicsScene
		//{
		//	get { return physicsScene; }
		//}

		//!!!!!
		//public static IList<Scene> Instances
		//{
		//	get { return SceneManager.GetScenes(); }
		//}

		//[Browsable( false )]
		//public List<Assembly> AdditionalClassAssemblies
		//{
		//	get { return additionalClassAssemblies; }
		//}

		//[Browsable( false )]
		//public double LastRenderTime
		//{
		//	get { return lastRenderTime; }
		//}

		//[Browsable( false )]
		//public double LastRenderTimeStep
		//{
		//	get { return lastRenderTimeStep; }
		//}

		internal override void OnEnabledInHierarchyChangedBefore()
		{
			base.OnEnabledInHierarchyChangedBefore();

			if( EnabledInHierarchyAndIsInstance )
			{
				lock( allInstancesEnabled )
					allInstancesEnabled.Add( this );
			}
			//if( EnabledInHierarchy )
			//{
			//	var ins = ComponentUtility.GetResourceInstanceByComponent( this );
			//	var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;

			//	if( !isResource )
			//	{
			//		lock( allInstancesEnabled )
			//			allInstancesEnabled.Add( this );
			//	}
			//}

			if( EnabledInHierarchyAndIsInstance )
				PhysicsWorldCreate();
		}

		internal override void OnEnabledInHierarchyChangedAfter()
		{
			//!!!!new
			if( EnabledInHierarchyAndIsInstance )
			//if( EnabledInHierarchy )
			{
				octreeCanCreate = true;
				OctreeUpdate( false );
			}

			//!!!!if( EnabledInHierarchy )
			{

				//!!!!что не тут?

				//!!!!!было
				////preload resources
				//if( EngineApp.Instance.ApplicationType != EngineApp.ApplicationTypes.ShaderCacheCompiler )
				//	CallPreloadResources();

				//!!!!когда вызывать?
				//StaticBatching_UpdateRenderingBatches( false );
				//StaticBatching_UpdateCollisionBatches( false );



				////!!!!когда вызывать?
				//UpdateSoundSettings();

				////!!!!когда вызывать?
				//UpdatePortalZones();
				////StaticMesh_OnPostCreate( loaded );
				////configure and rebuild scene graph
				////!!!!когда вызывать?
				//UpdateSceneGraphsSettings( true );

				//!!!!!
				////!!!!когда вызывать?
				////initialCollisionBounds
				//{
				//	initialCollisionBounds = Bounds.Cleared;
				//	foreach( Body body in physicsScene.Bodies )
				//	{
				//		foreach( Shape shape in body.Shapes )
				//		{
				//			if( shape.ContactGroup == (int)ContactGroup.Collision )
				//				initialCollisionBounds.Add( shape.GetGlobalBounds() );
				//		}
				//	}
				//	if( initialCollisionBounds.IsCleared() )
				//		initialCollisionBounds = new Bounds( -1, -1, -1, 1, 1, 1 );
				//}

			}

			if( !EnabledInHierarchy )
			{
				octreeCanCreate = false;
				OctreeUpdate( false );
			}

			if( !EnabledInHierarchy )
			{
				PhysicsWorldDestroy();
				Physics2DWorldDestroy();
			}

			if( !EnabledInHierarchy )
			{
				lock( allInstancesEnabled )
					allInstancesEnabled.Remove( this );
			}
			//if( !EnabledInHierarchy )
			//{
			//	lock( allScenesEnabled )
			//		allScenesEnabled.Remove( this );
			//}

			if( EnabledInHierarchy )
				PhysicsWorldPostCreate();

			if( EnabledInHierarchy )
				initTime = EngineApp.EngineTime;

			if( EnabledInHierarchy && EngineApp.IsSimulation )
			{
				if( octree != null )
					boundsWhenSimulationStarted = octree.GetOctreeBoundsWithBoundsOfObjectsOutsideOctree();
				else
					boundsWhenSimulationStarted = CalculateTotalBoundsOfObjectsInSpace();
			}

			BackgroundSoundUpdate();

			base.OnEnabledInHierarchyChangedAfter();
		}

		///// <summary>
		///// Gets the current time of simulation.
		///// </summary>
		//[Browsable( false )]
		//public double SimulationTickTime
		//{
		//	get { return simulationTickTime; }
		//}

		//!!!!

		public delegate void ViewportUpdateBeginDelegate( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings );
		public event ViewportUpdateBeginDelegate ViewportUpdateBefore;
		public event ViewportUpdateBeginDelegate ViewportUpdateBegin;
		public delegate void ViewportUpdateEndDelegate( Scene scene, Viewport viewport );
		public event ViewportUpdateEndDelegate ViewportUpdateEnd;

		public delegate void ViewportUpdateGetCameraSettingsDelegate( Scene scene, Viewport viewport, ref bool processed );
		public event ViewportUpdateGetCameraSettingsDelegate ViewportUpdateGetCameraSettings;

		protected virtual void OnViewportUpdateBefore( Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
		}

		internal void PerformViewportUpdateBefore( Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			OnViewportUpdateBefore( viewport, overrideCameraSettings );
			ViewportUpdateBefore?.Invoke( this, viewport, overrideCameraSettings );
		}

		protected virtual void OnViewportUpdateBegin( Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			//!!!!тут? так?
			//update camera settings
			if( overrideCameraSettings != null )
				viewport.CameraSettings = overrideCameraSettings;
			else
			{
				//get camera settings from event
				bool processed = false;
				ViewportUpdateGetCameraSettings?.Invoke( this, viewport, ref processed );

				//default behaviour
				if( !processed )
				{
					//get camera settings from Camera parameter
					Camera cam = CameraDefault;
					if( cam != null )
					{
						//!!!!каждый раз обновлять норм?
						viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, cam, FrustumCullingTest );

						//var context = new Metadata.CloneContext();
						//context.unreferenceProperties = true;
						//Camera settings = (Camera)cam.Clone( context );
						//viewport.CameraSettings = settings;

						//cam.UpdateData( viewport );
						//if( cam.LastUpdateData != null )
						//	viewport.CameraSettings = cam.LastUpdateData;
					}
					else
					{
						//!!!!
					}
				}
			}
		}

		internal void PerformViewportUpdateBegin( Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			OnViewportUpdateBegin( viewport, overrideCameraSettings );
			ViewportUpdateBegin?.Invoke( this, viewport, overrideCameraSettings );

			//update physics
			PhysicsViewportUpdateBegin( viewport );
			Physics2DViewportUpdateBegin( viewport );
		}

		protected virtual void OnViewportUpdateEnd( Viewport viewport )
		{
		}

		internal void PerformViewportUpdateEnd( Viewport viewport )
		{
			OnViewportUpdateEnd( viewport );
			ViewportUpdateEnd?.Invoke( this, viewport );
		}

		//public void _PerformViewportUpdateBegin( Viewport viewport, double time )
		//{
		//	//!!!!!!time

		//	//!!!!!максимальный time step

		//	//!!!!
		//	////update render time
		//	//double oldTime = map.lastRenderTime;
		//	//map.lastRenderTime = time;
		//	//map.lastRenderTimeStep = map.lastRenderTime - oldTime;

		//	MapObject[] a = mapObjectsAll.ToArray();
		//	foreach( var obj in a )
		//	{
		//		if( obj.EnabledInHierarchy && !obj.QueuedToRemoveFromParent )
		//			obj._PerformViewportUpdateBegin( viewport );
		//	}
		//}

		//public void _PerformViewportUpdateEnd( Viewport viewport )
		//{
		//	MapObject[] a = mapObjectsAll.ToArray();
		//	foreach( var obj in a )
		//	{
		//		if( obj.EnabledInHierarchy && !obj.QueuedToRemoveFromParent )
		//			obj._PerformViewportUpdateEnd( viewport );
		//	}

		//	ViewportUpdateEnd?.Invoke( this, viewport );
		//}

		//!!!!MapObject
		//public static double SimulationTickDelta
		//{
		//	get { return simulationTickDelta; }
		//}

		//!!!!
		////!!!!как отписываться при разрушении объекта?
		//public delegate void SimulationTickDelegate( Map map );//!!!!, float simulationTickDelta );
		//public event SimulationTickDelegate SimulationTick;

		//!!!!что еще нужно сцене для обновления?
		//public void Simulate( double time = 0 )
		//{
		//	//!!!!!
		//}

		/// <summary>
		/// Gets the list of all scene objects including resources.
		/// </summary>
		public static Scene[] GetAll()
		{
			lock( all )
				return all.ToArray();
		}

		/// <summary>
		/// Gets the list of all enabled instances of scenes.
		/// </summary>
		public static Scene[] GetAllInstancesEnabled()
		{
			lock( allInstancesEnabled )
				return allInstancesEnabled.ToArray();
		}

		internal static bool AllInstancesEnabledContains( Scene scene )
		{
			lock( allInstancesEnabled )
				return allInstancesEnabled.Contains( scene );
		}

		/// <summary>
		/// Gets first enabled scene.
		/// </summary>
		public static Scene First
		{
			get
			{
				lock( allInstancesEnabled )
					return allInstancesEnabled.Count != 0 ? allInstancesEnabled[ 0 ] : null;
			}
		}

		internal override void OnDisposeAfter()
		{
			lock( allInstancesEnabled )
				allInstancesEnabled.Remove( this );
			lock( all )
				all.Remove( this );

			base.OnDisposeAfter();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Bounds CalculateTotalBoundsOfObjectsInSpace( bool onlyEnabledInHierarchy = false )
		{
			var bounds = Bounds.Cleared;

			foreach( var obj in GetComponents<ObjectInSpace>( false, true, onlyEnabledInHierarchy ) )
			{
				if( obj.EnabledInHierarchy && obj.VisibleInHierarchy )//!!!!параметрами может
				{
					var b = obj.SpaceBounds.BoundingBox;
					bounds.Add( b );
				}
			}
			if( bounds.IsCleared() )
				bounds = Bounds.Zero;

			return bounds;
		}

		internal override void OnSimulationStepBefore()
		{
			base.OnSimulationStepBefore();

			PhysicsSimulate( false, null );
			Physics2DSimulate( false, null );
		}

		public delegate void GetDisplayDevelopmentDataInThisApplicationOverrideDelegate( Scene sender, ref bool display );
		public event GetDisplayDevelopmentDataInThisApplicationOverrideDelegate GetDisplayDevelopmentDataInThisApplicationOverride;

		public bool GetDisplayDevelopmentDataInThisApplication()
		{
			bool result;
			if( EngineApp.IsSimulation )
				result = DisplayDevelopmentDataInSimulation;
			else
				result = DisplayDevelopmentDataInEditor;

			GetDisplayDevelopmentDataInThisApplicationOverride?.Invoke( this, ref result );

			return result;
		}

		protected virtual void OnRender( Viewport viewport )
		{
			if( GetDisplayDevelopmentDataInThisApplication() && DisplaySceneOctree )
				octree?.DebugRender( viewport );
		}
		public delegate void RenderEventDelegate( Scene sender, Viewport viewport );
		public event RenderEventDelegate RenderEvent;

		internal void PerformRender( Viewport viewport )
		{
			OnRender( viewport );
			RenderEvent?.Invoke( this, viewport );
		}

		public virtual void OnRenderBeforeOutput( Viewport viewport )
		{
			//FrustumCullingTest
			if( GetDisplayDevelopmentDataInThisApplication() && FrustumCullingTest )
			{
				var points = viewport.CameraSettings.Frustum.Points;
				Rectangle rect = Rectangle.Cleared;
				for( int n = 4; n < 8; n++ )
				{
					Vector2 screenPosition;
					if( viewport.CameraSettings.ProjectToScreenCoordinates( points[ n ], out screenPosition ) )
						rect.Add( screenPosition );
				}

				if( !rect.IsCleared() )
				{
					var renderer = viewport.CanvasRenderer;

					ColorValue color = new ColorValue( 0, 0, 0, .5f );
					renderer.AddQuad( new Rectangle( -100, -100, 101, rect.Top ), color );
					renderer.AddQuad( new Rectangle( -100, rect.Bottom, 101, 101 ), color );
					renderer.AddQuad( new Rectangle( -100, rect.Top, rect.Left, rect.Bottom ), color );
					renderer.AddQuad( new Rectangle( rect.Right, rect.Top, 101, rect.Bottom ), color );

					renderer.AddRectangle( rect, new ColorValue( 1, 1, 0 ) );
					renderer.AddText( "Frustum Culling Test", new Vector2( .5f, rect.Bottom + .001f ), EHorizontalAlignment.Center, EVerticalAlignment.Top, new ColorValue( 1, 1, 0 ) );
				}
			}
		}

		[Browsable( false )]
		public double Time
		{
			get { return EngineApp.EngineTime - initTime; }
		}

		protected virtual void OnGetRenderSceneData( ViewportRenderingContext context ) { }

		public delegate void GetRenderSceneDataDelegate( Scene scene, ViewportRenderingContext context );
		public event GetRenderSceneDataDelegate GetRenderSceneData;
		public static event GetRenderSceneDataDelegate AllScenes_GetRenderSceneData;

		internal event GetRenderSceneDataDelegate GetRenderSceneData2ForGroupOfObjects;
		internal event GetRenderSceneDataDelegate GetRenderSceneData3ForGroupOfObjects;

		internal void PerformGetRenderSceneData( ViewportRenderingContext context )
		{
			OnGetRenderSceneData( context );

			if( EngineApp.IsEditor )
			{
				try
				{
					GetRenderSceneData?.Invoke( this, context );
				}
				catch( Exception e )
				{
					Log.Warning( "Scene: PerformGetRenderSceneData: GetRenderSceneData: " + e.Message );
					return;
				}
			}
			else
				GetRenderSceneData?.Invoke( this, context );

			if( EngineApp.IsEditor )
			{
				try
				{
					AllScenes_GetRenderSceneData?.Invoke( this, context );
				}
				catch( Exception e )
				{
					Log.Warning( "Scene: PerformGetRenderSceneData: AllScenes_GetRenderSceneData: " + e.Message );
					return;
				}
			}
			else
				AllScenes_GetRenderSceneData?.Invoke( this, context );


			//process group of objects queued actions
			if( EngineApp.IsEditor )
			{
				try
				{
					GetRenderSceneData2ForGroupOfObjects?.Invoke( this, context );
				}
				catch( Exception e )
				{
					Log.Warning( "Scene: PerformGetRenderSceneData: GetRenderSceneData2ForGroupOfObjects: " + e.Message );
					return;
				}
			}
			else
				GetRenderSceneData2ForGroupOfObjects?.Invoke( this, context );

			//process group of objects queued actions
			if( EngineApp.IsEditor )
			{
				try
				{
					GetRenderSceneData3ForGroupOfObjects?.Invoke( this, context );
				}
				catch( Exception e )
				{
					Log.Warning( "Scene: PerformGetRenderSceneData: GetRenderSceneData3ForGroupOfObjects: " + e.Message );
					return;
				}
			}
			else
				GetRenderSceneData3ForGroupOfObjects?.Invoke( this, context );
		}

		/////////////////////////////////////////

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			BackgroundSoundUpdate();

			octree?.SetEngineTimeToGetStatistics( EngineApp.EngineTime );
		}

		void BackgroundSoundUpdate()
		{
			//get demand sound
			Sound demandSound = null;
			if( EnabledInHierarchyAndIsInstance )
			{
				demandSound = BackgroundSound;
				if( demandSound != null && demandSound.Result == null )
					demandSound = null;
			}

			//change sound
			if( demandSound != backgroundSoundCurrent )
			{
				//stop previous
				if( backgroundSoundCurrent != null )
				{
					backgroundSoundChannel?.Stop();
					backgroundSoundChannel = null;
					backgroundSoundInstance = null;
				}

				backgroundSoundCurrent = demandSound;

				if( backgroundSoundCurrent != null )
				{
					backgroundSoundInstance = backgroundSoundCurrent.Result.LoadSoundByMode( SoundModes.Loop | SoundModes.Stream );
					if( backgroundSoundInstance != null )
					{
						var group = SoundWorld.GetChannelGroup( "Music" );
						if( group == null )
							group = EngineApp.DefaultSoundChannelGroup;
						backgroundSoundChannel = SoundWorld.SoundPlay( this, backgroundSoundInstance, group, 1, true );
					}
				}
			}

			//update channel
			if( backgroundSoundChannel != null )
			{
				backgroundSoundChannel.Volume = EngineApp.IsEditor ? BackgroundSoundVolumeInEditor.Value : BackgroundSoundVolumeInSimulation.Value;
				backgroundSoundChannel.Pause = false;
			}

			//Sound demandSound = null;
			//if( EnabledInHierarchy )
			//{
			//	demandSound = BackgroundSound;
			//	if( demandSound != null && demandSound.Result == null )
			//		demandSound = null;
			//}

			//if( demandSound != null )
			//	backgroundSoundInstance = demandSound.Result.LoadSoundByMode( SoundModes.Loop | SoundModes.Stream );
			//else
			//	backgroundSoundInstance = null;

			//if( backgroundSoundInstance != null )
			//{
			//	//!!!!EngineApp.DefaultSoundChannelGroup, Priority
			//	backgroundSoundChannel = SoundWorld.SoundPlay( this, backgroundSoundInstance, EngineApp.DefaultSoundChannelGroup, 0.5, true );
			//	if( backgroundSoundChannel != null )
			//	{
			//		backgroundSoundChannel.Volume = EngineApp.IsEditor ? BackgroundSoundVolumeInEditor.Value : BackgroundSoundVolumeInSimulation.Value;
			//		backgroundSoundChannel.Pause = false;
			//	}
			//}
			//else
			//{
			//	backgroundSoundChannel?.Stop();
			//	backgroundSoundChannel = null;
			//}
		}

		public Vector2 GetWindSpeedVector()
		{
			var dir = WindDirection.Value.InRadians();
			return new Vector2( Math.Cos( dir ), Math.Sin( dir ) ) * WindSpeed.Value;
		}

		//!!!!threading?
		[Browsable( false )]
		public ESet<Component> CachedObjectsInSpaceToFastFindByRenderingPipeline { get; } = new ESet<Component>();


		public void SoundPlay( SoundData sound, Vector3 position )
		{
			if( SoundWorld.BackendNull )
				return;

			var channel = SoundWorld.SoundPlay( this, sound, EngineApp.DefaultSoundChannelGroup, 0.5, true );
			if( channel != null )
			{
				channel.Position = position;// TransformV.Position;

				//!!!!
				//channel.Velocity = Velocity3D;
				channel.Pause = false;
			}
		}

		public void SoundPlay( string name, Vector3 position )
		{
			if( string.IsNullOrEmpty( name ) || SoundWorld.BackendNull )
				return;

			var sound = SoundWorld.SoundCreate( name, SoundModes.Mode3D );
			if( sound != null )
				SoundPlay( sound, position );
		}

		public void SoundPlay( Sound sound, Vector3 position )
		{
			var sound2 = sound?.Result?.LoadSoundByMode( SoundModes.Mode3D );
			if( sound2 != null )
				SoundPlay( sound2, position );
		}

		public void SoundPlay2D( SoundData sound )
		{
			if( SoundWorld.BackendNull )
				return;

			SoundWorld.SoundPlay( this, sound, EngineApp.DefaultSoundChannelGroup, 0.5 );
		}

		public void SoundPlay2D( Sound sound )
		{
			var sound2 = sound?.Result?.LoadSoundByMode( 0 );
			if( sound2 != null )
				SoundPlay2D( sound2 );
		}

		public void SoundPlay2D( string name )
		{
			if( string.IsNullOrEmpty( name ) || SoundWorld.BackendNull )
				return;

			var sound = SoundWorld.SoundCreate( name, 0 );
			if( sound != null )
				SoundPlay2D( sound );
		}

		//public OctreeContainer GetOctree()
		//{
		//	return octree;
		//}

		[Browsable( false )]
		public Bounds BoundsWhenSimulationStarted
		{
			get { return boundsWhenSimulationStarted; }
		}

	}
}
