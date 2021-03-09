// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Addon.Pathfinding;

namespace NeoAxis.Editor
{
	public class DocumentationLinks : AssemblyUtility.AssemblyRegistration
	{
		public override void OnRegister()
		{
			var items = new (string, Type)[]
			{
			//=== Common ===
			("Component",typeof(Component)),
			("Text File",typeof(NewResourceType_TextFile)),
			("Component Host",typeof(Component_ComponentHost)),
			("Compute Using Threads",typeof(Component_ComputeUsingThreads)),

			//=== Scripting ===
			("CSharp File",typeof(NewResourceType_CSharpClass)),
			("CSharp Script",typeof(Component_CSharpScript)),
			("Flow Graph",typeof(Component_FlowGraph)),

			////==== Metadata ====
			("Property",typeof(Component_Property)),
			("Method",typeof(Component_Method)),
			("Member Parameter",typeof(Component_MemberParameter)),

			//==== Flow scripting ====
			("Invoke Member",typeof(Component_InvokeMember)),
			("Event Handler",typeof(Component_EventHandler)),
			("Declare Variable",typeof(Component_DeclareVariable)),
			("Set Variable",typeof(Component_SetVariable)),
			("If",typeof(Component_If)),
			("Switch",typeof(Component_Switch)),
			("While",typeof(Component_While)),
			("Do While",typeof(Component_DoWhile)),
			("Do Number",typeof(Component_DoNumber)),
			("For Each",typeof(Component_ForEach)),
			("For",typeof(Component_For)),
			("Sequence",typeof(Component_Sequence)),
			("Flow Start",typeof(Component_FlowStart)),
			("Convert To",typeof(Component_ConvertTo)), 

			//=== UI ===
			("UIControl",typeof(UIControl)),
			("UIWindow",typeof(UIWindow)),
			("UIText",typeof(UIText)),
			("UIImage",typeof(UIImage)),
			("UIButton",typeof(UIButton)),
			("UICheck",typeof(UICheck)),
			("UIEdit",typeof(UIEdit)),
			("UISlider",typeof(UISlider)),
			("UIProgress",typeof(UIProgress)),
			("UIScroll",typeof(UIScroll)),
			("UICombo",typeof(UICombo)),
			("UIList",typeof(UIList)),
			("UIGrid",typeof(UIGrid)),
			("UIVideo",typeof(UIVideo)),
			("UIWebBrowser",typeof(UIWebBrowser)),
			("UIRenderTarget",typeof(UIRenderTarget)),
			("UITabControl",typeof(UITabControl)),
			("UIGrid",typeof(UIGrid)),
			("UIToolbar",typeof(UIToolbar)),
			("UIStyle",typeof(UIStyle)),
			("UITooltip",typeof(UITooltip)), 

			//=== Scene common ===
			("Scene",typeof(Component_Scene)),
			("Material",typeof(Component_Material)),
			("Surface",typeof(Component_Surface)),
			("Image",typeof(Component_Image)),
			("Paint Layer",typeof(Component_PaintLayer)),
			("Rendering Pipeline Default",typeof(Component_RenderingPipeline_Default)),
			("Mesh",typeof(Component_Mesh)),
			("Mesh Modifier",typeof(Component_MeshModifier)), 

			//=== Scene objects ===
			("Object In Space",typeof(Component_ObjectInSpace)),
			("Group Of Objects",typeof(Component_GroupOfObjects)),
			("Layer",typeof(Component_Layer)),
			("Mesh In Space",typeof(Component_MeshInSpace)),
			("Mesh In Space Animation Controller",typeof(Component_MeshInSpaceAnimationController)),
			("Particle System In Space",typeof(Component_ParticleSystemInSpace)),
			("Light",typeof(Component_Light)),
			("Camera",typeof(Component_Camera)),
			("Terrain",typeof(Component_Terrain)),
			("Billboard",typeof(Component_Billboard)),
			("Decal",typeof(Component_Decal)),
			("Reflection Probe",typeof(Component_ReflectionProbe)),
			("Sound Source",typeof(Component_SoundSource)),
			("Render Target In Space",typeof(Component_RenderTargetInSpace)),
			("Skybox",typeof(Component_Skybox)),
			("Fog",typeof(Component_Fog)),
			("Lens Flares",typeof(Component_LensFlares)),
			("Area",typeof(Component_Area)),
			("Surface Area",typeof(Component_SurfaceArea)),
			("Cut Volume",typeof(Component_CutVolume)),
			("Liquid Volume",typeof(Component_LiquidVolume)),
			("Sensor",typeof(Component_Sensor)),
			("Destroying Sensor",typeof(Component_DestroyingSensor)),
			("Text 2D",typeof(Component_Text2D)),
			("Measuring Tool",typeof(Component_MeasuringTool)),
			("Grid",typeof(Component_Grid)),
			("Curve In Space",typeof(Component_CurveInSpace)),
			("Render To File",typeof(Component_RenderToFile)),
			("World Generator",typeof(Component_WorldGenerator)), 

			//=== Primitives ===
			("Arch",typeof(Component_MeshGeometry_Arch)),
			("Box",typeof(Component_MeshGeometry_Box)),
			("Capsule",typeof(Component_MeshGeometry_Capsule)),
			("Cone",typeof(Component_MeshGeometry_Cone)),
			("Cylinder",typeof(Component_MeshGeometry_Cylinder)),
			("Door",typeof(Component_MeshGeometry_Door)),
			("Pipe",typeof(Component_MeshGeometry_Pipe)),
			("Plane",typeof(Component_MeshGeometry_Plane)),
			("Prism",typeof(Component_MeshGeometry_Prism)),
			("Sphere",typeof(Component_MeshGeometry_Sphere)),
			("Stairs",typeof(Component_MeshGeometry_Stairs)),
			("Torus",typeof(Component_MeshGeometry_Torus)),
			("Polygon Based Polyhedron",typeof(Component_MeshGeometry_PolygonBasedPolyhedron)), 

			//=== Physics ===
			("Rigid Body",typeof(Component_RigidBody)),
			("Box Shape",typeof(Component_CollisionShape_Box)),
			("Sphere Shape",typeof(Component_CollisionShape_Sphere)),
			("Cylinder Shape",typeof(Component_CollisionShape_Cylinder)),
			("Cone Shape",typeof(Component_CollisionShape_Cone)),
			("Capsule Shape",typeof(Component_CollisionShape_Capsule)),
			("Mesh Shape",typeof(Component_CollisionShape_Mesh)),
			("Physical Material",typeof(Component_PhysicalMaterial)),
			("Soft Body",typeof(Component_SoftBody)),
			("Constraint",typeof(Component_Constraint)),
			("Sensor",typeof(Component_Sensor)), 

			//=== Particles ===
			("Particle System",typeof(Component_ParticleSystem)),
			("Particle Emitter",typeof(Component_ParticleEmitter)), 

			//==== Shapes ====
			("Particle Emitter Shape Box",typeof(Component_ParticleEmitterShape_Box)),
			("Particle Emitter Shape Point",typeof(Component_ParticleEmitterShape_Point)),
			("Particle Emitter Shape Sphere",typeof(Component_ParticleEmitterShape_Sphere)),
			("Particle Emitter Shape Cylinder",typeof(Component_ParticleEmitterShape_Cylinder)),
			("Particle Emitter Shape",typeof(Component_ParticleEmitterShape)), 

			//==== Modules ====
			("Particle Size Multiplier By Time",typeof(Component_ParticleSizeMultiplierByTime)),
			("Particle Color Multiplier By Time",typeof(Component_ParticleColorMultiplierByTime)),
			("Particle Linear Acceleration By Time",typeof(Component_ParticleLinearAccelerationByTime)),
			("Particle Linear Velocity By Time",typeof(Component_ParticleLinearVelocityByTime)),
			("Particle Linear Speed Multiplier By Time",typeof(Component_ParticleLinearSpeedMultiplierByTime)),
			("Particle Angular Acceleration By Time",typeof(Component_ParticleAngularAccelerationByTime)),
			("Particle Angular Velocity By Time",typeof(Component_ParticleAngularVelocityByTime)),
			("Particle Angular Speed Multiplier By Time",typeof(Component_ParticleAngularSpeedMultiplierByTime)),
			("Particle Module Custom",typeof(Component_ParticleModuleCustom)), 

			//=== Screen effects ===
			("Ambient Occlusion",typeof(Component_RenderingEffect_AmbientOcclusion)),
			("Antialiasing",typeof(Component_RenderingEffect_Antialiasing)),
			("Bloom",typeof(Component_RenderingEffect_Bloom)),
			("Chromatic Aberration",typeof(Component_RenderingEffect_ChromaticAberration)),
			("Color Grading",typeof(Component_RenderingEffect_ColorGrading)),
			("Depth Of Field",typeof(Component_RenderingEffect_DepthOfField)),
			("Edge Detection",typeof(Component_RenderingEffect_EdgeDetection)),
			("Gaussian Blur",typeof(Component_RenderingEffect_GaussianBlur)),
			("Grayscale",typeof(Component_RenderingEffect_Grayscale)),
			("Lens Effects",typeof(Component_RenderingEffect_LensEffects)),
			("Light Shafts",typeof(Component_RenderingEffect_LightShafts)),
			("Motion Blur",typeof(Component_RenderingEffect_MotionBlur)),
			("Noise",typeof(Component_RenderingEffect_Noise)),
			("Outline",typeof(Component_RenderingEffect_Outline)),
			("Pixelate",typeof(Component_RenderingEffect_Pixelate)),
			("Posterize",typeof(Component_RenderingEffect_Posterize)),
			("Radial Blur",typeof(Component_RenderingEffect_RadialBlur)),
			("Screen Space Reflection",typeof(Component_RenderingEffect_ScreenSpaceReflection)),
			("Sharpen",typeof(Component_RenderingEffect_Sharpen)),
			("Show Render Target",typeof(Component_RenderingEffect_ShowRenderTarget)),
			("To LDR",typeof(Component_RenderingEffect_ToLDR)),
			("Tone Mapping",typeof(Component_RenderingEffect_ToneMapping)),
			("Vignetting",typeof(Component_RenderingEffect_Vignetting)),

			//=== Game framework ===
			("Game Mode",typeof(Component_GameMode)),
			("Input Processing",typeof(Component_InputProcessing)),
			("Camera Management",typeof(Component_CameraManagement)),
			("AI",typeof(Component_AI)),
			("Pathfinding",typeof(Component_Pathfinding)),
			("Pathfinding Geometry",typeof(Component_Pathfinding_Geometry)),
			("Pathfinding Geometry Tag",typeof(Component_Pathfinding_GeometryTag)),
			("Button In Space",typeof(Component_ButtonInSpace)),
			("Regulator Switch In Space",typeof(Component_RegulatorSwitchInSpace)), 

			//=== 3D ===
			("Character",typeof(Component_Character)),
			("Character Input Processing",typeof(Component_CharacterInputProcessing)),
			("Character AI",typeof(Component_CharacterAI)),
			("Character AI Move To Position",typeof(Component_CharacterAITask_MoveToPosition)),
			("Character AI Move To Object",typeof(Component_CharacterAITask_MoveToObject)),
			("Weapon",typeof(Component_Weapon)), 

			//=== 2D ===
			("Sprite",typeof(Component_Sprite)),
			("Sprite Animation Controller",typeof(Component_SpriteAnimationController)),
			("Rigid Body 2D",typeof(Component_RigidBody2D)),
			("Box Shape 2D",typeof(Component_CollisionShape2D_Box)),
			("Ellipse Shape 2D",typeof(Component_CollisionShape2D_Ellipse)),
			("Capsule Shape 2D",typeof(Component_CollisionShape2D_Capsule)),
			("Mesh Shape 2D",typeof(Component_CollisionShape2D_Mesh)),
			("Revolute Constraint 2D",typeof(Component_Constraint2D_Revolute)),
			("Prismatic Constraint 2D",typeof(Component_Constraint2D_Prismatic)),
			("Distance Constraint 2D",typeof(Component_Constraint2D_Distance)),
			("Weld Constraint 2D",typeof(Component_Constraint2D_Weld)),
			("Fixed Constraint 2D",typeof(Component_Constraint2D_Fixed)),
			("Sensor 2D",typeof(Component_Sensor2D)),
			("Destroying Sensor 2D",typeof(Component_DestroyingSensor2D)),

			("Character 2D",typeof(Component_Character2D)),
			("Character 2D Input Processing",typeof(Component_Character2DInputProcessing)),
			("Character 2D AI",typeof(Component_Character2DAI)),
			("Character 2D AI Move To Position",typeof(Component_Character2DAITask_MoveToPosition)),
			("Character 2D AI Move To Object",typeof(Component_Character2DAITask_MoveToObject)),
			("Weapon 2D",typeof(Component_Weapon2D))

			};

			foreach( var item in items )
				DocumentationLinksManager.AddNameByType( item.Item2, item.Item1 );
		}
	}
}
