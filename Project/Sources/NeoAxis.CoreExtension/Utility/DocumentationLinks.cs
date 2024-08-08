// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class DocumentationLinks : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsEditor )
			{
				var items = new (string, Type)[]
				{
					//Common
					( "Component", typeof( Component ) ),
					( "Text File", typeof( Component ) ),// NewResourceType_TextFile ) ),
					( "Component Host", typeof( ComponentHost ) ),

					//Scripting
					( "CSharp File", typeof( Component ) ),// NewResourceType_CSharpClass ) ),
					( "CSharp Script", typeof( CSharpScript ) ),
					( "Flow Graph", typeof( FlowGraph ) ),

					//Metadata
					( "Property", typeof( VirtualProperty ) ),
					( "Method", typeof( VirtualMethod ) ),
					( "Member Parameter", typeof( VirtualMemberParameter ) ),

					//Flow scripting
					( "Invoke Member", typeof( InvokeMember ) ),
					( "Event Handler", typeof( EventHandlerComponent ) ),
					( "Declare Variable", typeof( FlowDeclareVariable ) ),
					( "Set Variable", typeof( FlowSetVariable ) ),
					( "If", typeof( FlowIf ) ),
					( "Switch", typeof( FlowSwitch ) ),
					( "While", typeof( FlowWhile ) ),
					( "Do While", typeof( FlowDoWhile ) ),
					( "Do Number", typeof( FlowDoNumber ) ),
					( "For Each", typeof( FlowForEach ) ),
					( "For", typeof( FlowFor ) ),
					( "Sequence", typeof( FlowSequence ) ),
					( "Flow Start", typeof( FlowStart ) ),
					( "Convert To", typeof( FlowConvertTo ) ),
					( "Sleep", typeof( FlowSleep ) ),

					//Shader scripting
					( "Shader Script", typeof( ShaderScript ) ),

					//UI
					( "UIControl", typeof( UIControl ) ),
					( "UIWindow", typeof( UIWindow ) ),
					( "UIText", typeof( UIText ) ),
					( "UIImage", typeof( UIImage ) ),
					( "UIButton", typeof( UIButton ) ),
					( "UICheck", typeof( UICheck ) ),
					( "UIEdit", typeof( UIEdit ) ),
					( "UISlider", typeof( UISlider ) ),
					( "UIProgress", typeof( UIProgress ) ),
					( "UIScroll", typeof( UIScroll ) ),
					( "UICombo", typeof( UICombo ) ),
					( "UIList", typeof( UIList ) ),
					( "UIGrid", typeof( UIGrid ) ),
					( "UIVideo", typeof( UIVideo ) ),
					( "UIWebBrowser", typeof( UIWebBrowser ) ),
					( "UIRenderTarget", typeof( UIRenderTarget ) ),
					( "UITabControl", typeof( UITabControl ) ),
					( "UIGrid", typeof( UIGrid ) ),
					( "UIToolbar", typeof( UIToolbar ) ),
					( "UIStyle", typeof( UIStyle ) ),
					( "UITooltip", typeof( UITooltip ) ), 

					//Scene common
					( "Scene", typeof( Scene ) ),
					( "Material", typeof( Material ) ),
					( "Multi Material", typeof( MultiMaterial ) ),
					( "Surface", typeof( Surface ) ),
					( "Image", typeof( ImageComponent ) ),
					( "Paint Layer", typeof( PaintLayer ) ),
					( "Rendering Pipeline Basic", typeof( RenderingPipeline_Basic ) ),
					( "Mesh", typeof( Mesh ) ),
					( "Mesh Modifier", typeof( MeshModifier ) ),

					//Scene objects
					( "Object In Space", typeof( ObjectInSpace ) ),
					( "Group Of Objects", typeof( GroupOfObjects ) ),
					( "Layer", typeof( Layer ) ),
					( "Mesh In Space", typeof( MeshInSpace ) ),
					( "Mesh In Space Animation Controller", typeof( MeshInSpaceAnimationController ) ),
					( "Animation Trigger", typeof( AnimationTrigger ) ),
					( "Particle System In Space", typeof( ParticleSystemInSpace ) ),
					( "Light", typeof( Light ) ),
					( "Camera", typeof( Camera ) ),
					( "Terrain", typeof( Terrain ) ),
					( "Billboard", typeof( Billboard ) ),
					( "Decal", typeof( Decal ) ),
					( "Reflection Probe", typeof( ReflectionProbe ) ),
					( "Sound Source", typeof( SoundSource ) ),
					( "Render Target In Space", typeof( RenderTargetInSpace ) ),
					( "Sky", typeof( Sky ) ),
					( "Fog", typeof( Fog ) ),
					( "Lens Flares", typeof( LensFlares ) ),
					( "Area", typeof( Area ) ),
					( "Surface Area", typeof( SurfaceArea ) ),
					( "Cut Volume", typeof( CutVolume ) ),
					( "Liquid Volume", typeof( LiquidVolume ) ),
					( "Sensor", typeof( Sensor ) ),
					( "Destroying Sensor", typeof( DestroyingSensor ) ),
					( "Text 2D", typeof( Text2D ) ),
					( "Text 3D", typeof( Text3D ) ),
					( "Measuring Tool", typeof( MeasuringTool ) ),
					( "Measuring Grid", typeof( MeasuringGrid ) ),
					( "Curve In Space", typeof( CurveInSpace ) ),
					( "Curve In Space Objects", typeof( CurveInSpaceObjects ) ),
					( "Render To File", typeof( RenderToFile ) ),
					( "World Generator", typeof( WorldGenerator ) ), 

					//Primitives
					( "Arch", typeof( MeshGeometry_Arch ) ),
					( "Box", typeof( MeshGeometry_Box ) ),
					( "Capsule", typeof( MeshGeometry_Capsule ) ),
					( "Cone", typeof( MeshGeometry_Cone ) ),
					( "Cylinder", typeof( MeshGeometry_Cylinder ) ),
					( "Door", typeof( MeshGeometry_Door ) ),
					( "Pipe", typeof( MeshGeometry_Pipe ) ),
					( "Plane", typeof( MeshGeometry_Plane ) ),
					( "Prism", typeof( MeshGeometry_Prism ) ),
					( "Sphere", typeof( MeshGeometry_Sphere ) ),
					( "Stairs", typeof( MeshGeometry_Stairs ) ),
					( "Torus", typeof( MeshGeometry_Torus ) ),
					( "Polygon Based Polyhedron", typeof( MeshGeometry_PolygonBasedPolyhedron ) ), 

					//Physics
					( "Rigid Body", typeof( RigidBody ) ),
					( "Box Shape", typeof( CollisionShape_Box ) ),
					( "Sphere Shape", typeof( CollisionShape_Sphere ) ),
					( "Cylinder Shape", typeof( CollisionShape_Cylinder ) ),
					( "Cone Shape", typeof( CollisionShape_Cone ) ),
					( "Capsule Shape", typeof( CollisionShape_Capsule ) ),
					( "Mesh Shape", typeof( CollisionShape_Mesh ) ),
					( "Physical Material", typeof( PhysicalMaterial ) ),
					( "Soft Body", typeof( SoftBody ) ),
					( "Constraint SixDOF", typeof( Constraint_SixDOF ) ),
					//( "Sensor", typeof( Sensor ) ), 

					//Particles
					( "Particle System", typeof( ParticleSystem ) ),
					( "Particle Emitter", typeof( ParticleEmitter ) ), 

					//Shapes
					( "Particle Emitter Shape Box", typeof( ParticleEmitterShape_Box ) ),
					( "Particle Emitter Shape Point", typeof( ParticleEmitterShape_Point ) ),
					( "Particle Emitter Shape Sphere", typeof( ParticleEmitterShape_Sphere ) ),
					( "Particle Emitter Shape Cylinder", typeof( ParticleEmitterShape_Cylinder ) ),
					( "Particle Emitter Shape", typeof( ParticleEmitterShape ) ), 

					//Modules
					( "Particle Size Multiplier By Time", typeof( ParticleSizeMultiplierByTime ) ),
					( "Particle Color Multiplier By Time", typeof( ParticleColorMultiplierByTime ) ),
					( "Particle Linear Acceleration By Time", typeof( ParticleLinearAccelerationByTime ) ),
					( "Particle Linear Velocity By Time", typeof( ParticleLinearVelocityByTime ) ),
					( "Particle Linear Speed Multiplier By Time", typeof( ParticleLinearSpeedMultiplierByTime ) ),
					( "Particle Angular Acceleration By Time", typeof( ParticleAngularAccelerationByTime ) ),
					( "Particle Angular Velocity By Time", typeof( ParticleAngularVelocityByTime ) ),
					( "Particle Angular Speed Multiplier By Time", typeof( ParticleAngularSpeedMultiplierByTime ) ),
					( "Particle Module Custom", typeof( ParticleModuleCustom ) ),

					//Screen effects
					( "Ambient Occlusion", typeof( RenderingEffect_AmbientOcclusion ) ),
					( "Antialiasing", typeof( RenderingEffect_Antialiasing ) ),
					( "Bloom", typeof( RenderingEffect_Bloom ) ),
					( "Bokeh Blur", typeof( RenderingEffect_BokehBlur ) ),
					( "Chromatic Aberration", typeof( RenderingEffect_ChromaticAberration ) ),
					( "Color Grading", typeof( RenderingEffect_ColorGrading ) ),
					( "Depth Of Field", typeof( RenderingEffect_DepthOfField ) ),
					( "Edge Detection", typeof( RenderingEffect_EdgeDetection ) ),
					( "Gaussion Blur", typeof( RenderingEffect_GaussianBlur ) ),
					( "Grayscale", typeof( RenderingEffect_Grayscale ) ),
					( "Precipitation", typeof( RenderingEffect_Precipitation ) ),
					( "Indirect Lighting", typeof( RenderingEffect_IndirectLighting ) ),
					( "Lens Effects", typeof( RenderingEffect_LensEffects ) ),
					( "Light Shafts", typeof( RenderingEffect_LightShafts ) ),
					( "Microparticles In Air", typeof( RenderingEffect_MicroparticlesInAir ) ),
					( "Motion Blur", typeof( RenderingEffect_MotionBlur ) ),
					( "Noise", typeof( RenderingEffect_Noise ) ),
					( "Outline", typeof( RenderingEffect_Outline ) ),
					( "Pixelate", typeof( RenderingEffect_Pixelate ) ),
					( "Posterize", typeof( RenderingEffect_Posterize ) ),
					( "Radial Blur", typeof( RenderingEffect_RadialBlur ) ),
					( "Reflection", typeof( RenderingEffect_Reflection ) ),
					( "Resolution Upscale", typeof( RenderingEffect_ResolutionUpscale ) ),
					( "Script Effect", typeof( RenderingEffect_Script ) ),
					( "Sharpen", typeof( RenderingEffect_Sharpen ) ),
					( "Show Render Target", typeof( RenderingEffect_ShowRenderTarget ) ),
					( "To LDR", typeof( RenderingEffect_ToLDR ) ),
					( "Tone Mapping", typeof( RenderingEffect_ToneMapping ) ),
					( "Vignetting", typeof( RenderingEffect_Vignetting ) ),

					//Game framework
					( "Game Mode", typeof( GameMode ) ),
					( "Input Processing", typeof( InputProcessing ) ),
					( "Camera Management", typeof( CameraManagement ) ),
					( "AI", typeof( AI ) ),
					( "Pathfinding", typeof( Pathfinding ) ),
					( "Pathfinding Geometry", typeof( PathfindingGeometry ) ),
					( "Pathfinding Geometry Tag", typeof( PathfindingGeometryTag ) ),
					( "Button", typeof( Button ) ),
					( "Regulator", typeof( Regulator ) ), 

					//3D
					( "Character Type", typeof( CharacterType ) ),
					( "Character", typeof( Character ) ),
					( "Character Input Processing", typeof( CharacterInputProcessing ) ),
					( "Character AI", typeof( CharacterAI ) ),
					( "Character AI Move To Position", typeof( CharacterAITask_MoveToPosition ) ),
					( "Character AI Move To Object", typeof( CharacterAITask_MoveToObject ) ),
					( "Character AI Turn To Position", typeof( CharacterAITask_TurnToPosition ) ),
					( "Character AI Turn To Object", typeof( CharacterAITask_TurnToObject ) ),
					( "Character AI Press Button", typeof( CharacterAITask_PressButton ) ),
					( "Character AI Turn Switch", typeof( CharacterAITask_TurnSwitch ) ),
					( "Character AI Wait", typeof( CharacterAITask_Wait ) ),
					( "Vehicle Type", typeof( VehicleType ) ),
					( "Vehicle Type Wheel", typeof( VehicleTypeWheel ) ),
					//!!!!( "Vehicle Aero Engine", typeof( VehicleAeroEngine ) ),
					( "Vehicle", typeof( Vehicle ) ),
					( "Vehicle Input Processing", typeof( VehicleInputProcessing ) ),
					( "Vehicle AI", typeof( VehicleAI ) ),
					( "Vehicle AI Move To Position", typeof( VehicleAITask_MoveToPosition ) ),
					( "Vehicle AI Move To Object", typeof( VehicleAITask_MoveToObject ) ),
					( "Weapon Type", typeof( WeaponType ) ),
					( "Weapon", typeof( Weapon ) ),
					( "Bullet", typeof( Bullet ) ),
					//( "Explosion", typeof( Explosion ) ),
					//( "Door Component", typeof( Door ) ),
					( "Seat Type", typeof( SeatType ) ),
					( "Seat", typeof( Seat ) ),
					( "Seat Item", typeof( SeatItem ) ),

					//2D
					( "Sprite", typeof( Sprite ) ),
					( "Sprite Animation Controller", typeof( SpriteAnimationController ) ),
					( "Rigid Body 2D", typeof( RigidBody2D ) ),
					( "Box Shape 2D", typeof( CollisionShape2D_Box ) ),
					( "Ellipse Shape 2D", typeof( CollisionShape2D_Ellipse ) ),
					( "Capsule Shape 2D", typeof( CollisionShape2D_Capsule ) ),
					( "Mesh Shape 2D", typeof( CollisionShape2D_Mesh ) ),
					( "Revolute Constraint 2D", typeof( Constraint2D_Revolute ) ),
					( "Prismatic Constraint 2D", typeof( Constraint2D_Prismatic ) ),
					( "Distance Constraint 2D", typeof( Constraint2D_Distance ) ),
					( "Weld Constraint 2D", typeof( Constraint2D_Weld ) ),
					( "Fixed Constraint 2D", typeof( Constraint2D_Fixed ) ),
					( "Sensor 2D", typeof( Sensor2D ) ),
					( "Destroying Sensor 2D", typeof( DestroyingSensor2D ) ),
					( "Character 2D Type", typeof( Character2DType ) ),
					( "Character 2D", typeof( Character2D ) ),
					( "Character 2D Input Processing", typeof( Character2DInputProcessing ) ),
					( "Character 2D AI", typeof( Character2DAI ) ),
					( "Character 2D AI Move To Position", typeof( Character2DAITask_MoveToPosition ) ),
					( "Character 2D AI Move To Object", typeof( Character2DAITask_MoveToObject ) ),
					( "Weapon 2D", typeof( Weapon2D ) )
				};

				foreach( var item in items )
					EditorAPI.DocumentationLinksManager_AddNameByType( item.Item2, item.Item1 );
			}
		}
	}
}
#endif