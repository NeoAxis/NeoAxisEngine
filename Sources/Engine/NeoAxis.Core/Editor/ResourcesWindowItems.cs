// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Linq;

namespace NeoAxis.Editor
{
	public static class ResourcesWindowItems
	{
		static List<Item> itemsByAttribute = new List<Item>();
		static List<Item> items = new List<Item>();
		static bool itemsPrepared;
		static Dictionary<string, string> groupDescriptions = new Dictionary<string, string>();

		/////////////////////////////////////////

		public class Item
		{
			public string Path;
			public Type Type;
			public double SortOrder;
			public bool Disabled;

			public Item( string path, Type type, double sortOrder = 0, bool disabled = false )
			{
				this.Path = path;
				this.Type = type;
				this.SortOrder = sortOrder;
				this.Disabled = disabled;
			}
		}

		/////////////////////////////////////////

		public static List<Item> Items
		{
			get { return items; }
		}

		public static Dictionary<string, string> GroupDescriptions
		{
			get { return groupDescriptions; }
		}

		public static event Action FixItems;

		public static void PrepareItems()
		{
			if( itemsPrepared )
				return;

			AddBaseItems();
			//AddSolutionItems();
			items.AddRange( itemsByAttribute );

			FixItems?.Invoke();

			itemsPrepared = true;
		}

		static void AddBaseItems()
		{
			//Common
			{
				var group = @"Base\Common";
				//!!!!
				//GroupDescriptions[ group ] = "Description text";
				Items.Add( new Item( group + @"\Component", typeof( Component ) ) );
				Items.Add( new Item( group + @"\Text File", typeof( NewResourceType_TextFile ) ) );

				//!!!!убрать "The feature is not implemented."
				Items.Add( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ), disabled: true ) );
				Items.Add( new Item( group + @"\Executable App", typeof( NewResourceType_ExecutableApp ), disabled: true ) );

				Items.Add( new Item( group + @"\Advanced\Component Host", typeof( Component_ComponentHost ) ) );

				//!!!!Image
			}

			//Scripting
			{
				//Common
				{
					var group = @"Base\Scripting";// Common\";
					Items.Add( new Item( group + @"\C# File", typeof( NewResourceType_CSharpClass ) ) );
					Items.Add( new Item( group + @"\C# Script", typeof( Component_CSharpScript ) ) );
					Items.Add( new Item( group + @"\Flow Graph", typeof( Component_FlowGraph ) ) );
					//group.types.Add( new Item( "C# library", typeof( NewResourceType_CSharpClassLibrary ), true ) );

					//Items.Add( new Item( group + "Event Handler", typeof( Component_EventHandler ) ) );
				}

				//Metadata
				{
					var group = @"Base\Scripting\Metadata";
					Items.Add( new Item( group + @"\Property", typeof( Component_Property ) ) );
					Items.Add( new Item( group + @"\Method", typeof( Component_Method ) ) );
					//Items.Add( new Item( group + @"\Event", typeof( Component_Event ), true ) );
					Items.Add( new Item( group + @"\Member Parameter", typeof( Component_MemberParameter ) ) );
					//Items.Add( new Item( group + @"\Event", typeof( Component_Event ), true ) );
				}

				//Flow scripting
				{
					var group = @"Base\Scripting\Flow scripting";
					Items.Add( new Item( group + @"\Invoke Member", typeof( Component_InvokeMember ) ) );
					Items.Add( new Item( group + @"\Event Handler", typeof( Component_EventHandler ) ) );
					Items.Add( new Item( group + @"\Declare Variable", typeof( Component_DeclareVariable ) ) );
					Items.Add( new Item( group + @"\Set Variable", typeof( Component_SetVariable ) ) );
					Items.Add( new Item( group + @"\If", typeof( Component_If ) ) );
					Items.Add( new Item( group + @"\Switch", typeof( Component_Switch ) ) );
					Items.Add( new Item( group + @"\While", typeof( Component_While ) ) );
					Items.Add( new Item( group + @"\Do While", typeof( Component_DoWhile ) ) );
					Items.Add( new Item( group + @"\Do Number", typeof( Component_DoNumber ) ) );
					Items.Add( new Item( group + @"\For Each", typeof( Component_ForEach ) ) );
					Items.Add( new Item( group + @"\For", typeof( Component_For ) ) );
					Items.Add( new Item( group + @"\Sequence", typeof( Component_Sequence ) ) );
					Items.Add( new Item( group + @"\Flow Start", typeof( Component_FlowStart ) ) );
					Items.Add( new Item( group + @"\Convert To", typeof( Component_ConvertTo ) ) );
				}

				////!!!!nb
				////C# projects
				//{
				//	var group = @"Base\Scripting\C# projects";

				//	{
				//		var item = new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		Items.Add( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		Items.Add( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		Items.Add( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# NeoAxis Addon", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		Items.Add( item );
				//	}

				//	//Items.Add( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) ) );

				//	//Items.Add( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\NeoAxis Editor App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//Items.Add( new Item( group + @"\NeoAxis Simulation App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//}

			}

			//UI
			{
				{
					var group = @"Base\UI";
					Items.Add( new Item( group + @"\Control", typeof( UIControl ) ) );
					Items.Add( new Item( group + @"\Window", typeof( UIWindow ) ) );
					Items.Add( new Item( group + @"\Text", typeof( UIText ) ) );
					Items.Add( new Item( group + @"\Image", typeof( UIImage ) ) );
					Items.Add( new Item( group + @"\Button", typeof( UIButton ) ) );
					Items.Add( new Item( group + @"\Check", typeof( UICheck ) ) );
					Items.Add( new Item( group + @"\Edit", typeof( UIEdit ) ) );
					Items.Add( new Item( group + @"\Slider", typeof( UISlider ) ) );
					Items.Add( new Item( group + @"\Progress", typeof( UIProgress ) ) );
					Items.Add( new Item( group + @"\Scroll", typeof( UIScroll ) ) );
					Items.Add( new Item( group + @"\Combo", typeof( UICombo ) ) );
					Items.Add( new Item( group + @"\List", typeof( UIList ) ) );
					//Items.Add( new Item( group + @"\Tree", typeof( UITree ) ) );
					Items.Add( new Item( group + @"\Video", typeof( UIVideo ) ) );
					Items.Add( new Item( group + @"\Web Browser", typeof( UIWebBrowser ) ) );
					Items.Add( new Item( group + @"\Render Target", typeof( UIRenderTarget ) ) );
					//Items.Add( new Item( group + @"\Menu", typeof( UIMenu ) ) );
				}

				//Containers
				{
					var group = @"Base\UI\Containers";
					Items.Add( new Item( group + @"\Tab Control", typeof( UITabControl ) ) );
					//Items.Add( new Item( group + @"\Split Container", typeof( UISplitContainer ) ) );
					Items.Add( new Item( group + @"\Grid", typeof( UIGrid ) ) );
					Items.Add( new Item( group + @"\Toolbar", typeof( UIToolbar ) ) );
				}

				//More
				{
					var group = @"Base\UI\More";
					Items.Add( new Item( group + @"\Style", typeof( UIStyle ) ) );
					Items.Add( new Item( group + @"\Tooltip", typeof( UITooltip ) ) );
				}
			}

			//Common 3D
			{
				var group = @"Base\Scene common";
				//var group = @"Base\Common 3D";
				Items.Add( new Item( group + @"\Scene", typeof( Component_Scene ) ) );
				Items.Add( new Item( group + @"\Material", typeof( Component_Material ) ) );
				Items.Add( new Item( group + @"\Surface", typeof( Component_Surface ) ) );
				Items.Add( new Item( group + @"\Cubemap", typeof( Component_Image ) ) );
				Items.Add( new Item( group + @"\Paint Layer", typeof( Component_PaintLayer ) ) );

				if( MetadataManager.GetType( "NeoAxis.Component_RenderingPipeline_Default" ) != null )
					Items.Add( new Item( group + @"\Rendering Pipeline Default", MetadataManager.GetType( "NeoAxis.Component_RenderingPipeline_Default" ).GetNetType() ) );

				Items.Add( new Item( group + @"\Mesh", typeof( Component_Mesh ) ) );

				////Mesh modifiers
				//var groupMeshModifiers = @"Base\Scene common\Mesh modifiers";
				//Items.Add( new Item( groupMeshModifiers + @"\Box UV", typeof( Component_MeshModifier_BoxUV ), disabled: true ) );
				//Items.Add( new Item( groupMeshModifiers + @"\Spherical UV", typeof( Component_MeshModifier_SphericalUV ), disabled: true ) );
				//Items.Add( new Item( groupMeshModifiers + @"\Tiling UV", typeof( Component_MeshModifier_TilingUV ), disabled: true ) );
				////Items.Add( new Item( groupMeshModifiers + @"\Unwrap", typeof( Component_MeshModifier_Unwrap ), disabled: true ) );
				////Items.Add( new Item( groupMeshModifiers + @"\Smooth", typeof( Component_MeshModifier_Smooth ), disabled: true ) );

				//Items.Add( new Item( group + @"\Paint Layer", typeof( Component_PaintLayer ) ) );
			}

			//Scene objects
			{
				var group = @"Base\Scene objects";


				Items.Add( new Item( group + @"\Object In Space", typeof( Component_ObjectInSpace ) ) );
				Items.Add( new Item( group + @"\Group Of Objects", typeof( Component_GroupOfObjects ) ) );
				Items.Add( new Item( group + @"\Layer", typeof( Component_Layer ) ) );
				Items.Add( new Item( group + @"\Mesh In Space", typeof( Component_MeshInSpace ) ) );
				Items.Add( new Item( group + @"\Mesh In Space Animation Controller", typeof( Component_MeshInSpaceAnimationController ) ) );
				Items.Add( new Item( group + @"\Particle System In Space", typeof( Component_ParticleSystemInSpace ) ) );
				Items.Add( new Item( group + @"\Light", typeof( Component_Light ) ) );
				Items.Add( new Item( group + @"\Camera", typeof( Component_Camera ) ) );
				Items.Add( new Item( group + @"\Terrain", typeof( Component_Terrain ) ) );
				Items.Add( new Item( group + @"\Billboard", typeof( Component_Billboard ) ) );
				Items.Add( new Item( group + @"\Decal", typeof( Component_Decal ) ) );
				Items.Add( new Item( group + @"\Reflection Probe", typeof( Component_ReflectionProbe ) ) );
				Items.Add( new Item( group + @"\Sound Source", typeof( Component_SoundSource ) ) );

				if( MetadataManager.GetType( "NeoAxis.Component_RenderTargetInSpace" ) != null )
					Items.Add( new Item( group + @"\Render Target In Space", MetadataManager.GetType( "NeoAxis.Component_RenderTargetInSpace" ).GetNetType() ) );

				if( MetadataManager.GetType( "NeoAxis.Component_Skybox" ) != null )
					Items.Add( new Item( group + @"\Environment\Skybox", MetadataManager.GetType( "NeoAxis.Component_Skybox" ).GetNetType() ) );
				if( MetadataManager.GetType( "NeoAxis.Component_Fog" ) != null )
					Items.Add( new Item( group + @"\Environment\Fog", MetadataManager.GetType( "NeoAxis.Component_Fog" ).GetNetType() ) );
				if( MetadataManager.GetType( "NeoAxis.Component_LensFlares" ) != null )
					Items.Add( new Item( group + @"\Environment\Lens Flares", MetadataManager.GetType( "NeoAxis.Component_LensFlares" ).GetNetType() ) );

				Items.Add( new Item( group + @"\Areas\Area", typeof( Component_Area ) ) );
				Items.Add( new Item( group + @"\Areas\Surface Area", typeof( Component_SurfaceArea ) ) );

				Items.Add( new Item( group + @"\Volumes\Cut Volume", typeof( Component_CutVolume ) ) );
				if( MetadataManager.GetType( "NeoAxis.Component_LiquidVolume" ) != null )
					Items.Add( new Item( group + @"\Volumes\Liquid Volume", MetadataManager.GetType( "NeoAxis.Component_LiquidVolume" ).GetNetType() ) );

				Items.Add( new Item( group + @"\Sensors\Sensor", typeof( Component_Sensor ) ) );
				if( MetadataManager.GetType( "NeoAxis.Component_DestroyingSensor" ) != null )
					Items.Add( new Item( group + @"\Sensors\Destroying Sensor", MetadataManager.GetType( "NeoAxis.Component_DestroyingSensor" ).GetNetType() ) );

				Items.Add( new Item( group + @"\Additional\Text 2D", typeof( Component_Text2D ) ) );
				if( MetadataManager.GetType( "NeoAxis.Component_MeasuringTool" ) != null )
					Items.Add( new Item( group + @"\Additional\Measuring Tool", MetadataManager.GetType( "NeoAxis.Component_MeasuringTool" ).GetNetType() ) );
				if( MetadataManager.GetType( "NeoAxis.Component_Grid" ) != null )
					Items.Add( new Item( group + @"\Additional\Grid", MetadataManager.GetType( "NeoAxis.Component_Grid" ).GetNetType() ) );
				if( MetadataManager.GetType( "NeoAxis.Component_CurveInSpace" ) != null )
					Items.Add( new Item( group + @"\Additional\Curve In Space", MetadataManager.GetType( "NeoAxis.Component_CurveInSpace" ).GetNetType() ) );

				//Items.Add( new Item( group + @"\Object In Space", typeof( Component_ObjectInSpace ) ) );
				//Items.Add( new Item( group + @"\Group Of Objects", typeof( Component_GroupOfObjects ) ) );
				//Items.Add( new Item( group + @"\Layer", typeof( Component_Layer ) ) );
				//Items.Add( new Item( group + @"\Mesh In Space", typeof( Component_MeshInSpace ) ) );
				//Items.Add( new Item( group + @"\Mesh In Space Animation Controller", typeof( Component_MeshInSpaceAnimationController ) ) );

				//Items.Add( new Item( group + @"\Particle System In Space", typeof( Component_ParticleSystemInSpace ) ) );

				//Items.Add( new Item( group + @"\Light", typeof( Component_Light ) ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_Skybox" ) != null )
				//	Items.Add( new Item( group + @"\Skybox", MetadataManager.GetType( "NeoAxis.Component_Skybox" ).GetNetType() ) );
				//if( MetadataManager.GetType( "NeoAxis.Component_Fog" ) != null )
				//	Items.Add( new Item( group + @"\Fog", MetadataManager.GetType( "NeoAxis.Component_Fog" ).GetNetType() ) );

				//Items.Add( new Item( group + @"\Terrain", typeof( Component_Terrain ) ) );
				//Items.Add( new Item( group + @"\Billboard", typeof( Component_Billboard ) ) );
				//Items.Add( new Item( group + @"\Decal", typeof( Component_Decal ) ) );
				//Items.Add( new Item( group + @"\Camera", typeof( Component_Camera ) ) );
				//Items.Add( new Item( group + @"\Reflection Probe", typeof( Component_ReflectionProbe ) ) );
				//Items.Add( new Item( group + @"\Sound Source", typeof( Component_SoundSource ) ) );
				//Items.Add( new Item( group + @"\Sensor", typeof( Component_Sensor ) ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_CurveInSpace" ) != null )
				//	Items.Add( new Item( group + @"\Curve In Space", MetadataManager.GetType( "NeoAxis.Component_CurveInSpace" ).GetNetType() ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_LensFlares" ) != null )
				//	Items.Add( new Item( group + @"\Lens Flares", MetadataManager.GetType( "NeoAxis.Component_LensFlares" ).GetNetType() ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_Grid" ) != null )
				//	Items.Add( new Item( group + @"\Grid", MetadataManager.GetType( "NeoAxis.Component_Grid" ).GetNetType() ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_DestroyingSensor" ) != null )
				//	Items.Add( new Item( group + @"\Destroying Sensor", MetadataManager.GetType( "NeoAxis.Component_DestroyingSensor" ).GetNetType() ) );

				//Items.Add( new Item( group + @"\Area", typeof( Component_Area ) ) );
				//Items.Add( new Item( group + @"\Surface Area", typeof( Component_SurfaceArea ) ) );
				//Items.Add( new Item( group + @"\Text 2D", typeof( Component_Text2D ) ) );

				//if( MetadataManager.GetType( "NeoAxis.Component_RenderTargetInSpace" ) != null )
				//	Items.Add( new Item( group + @"\Render Target In Space", MetadataManager.GetType( "NeoAxis.Component_RenderTargetInSpace" ).GetNetType() ) );



				//Items.Add( new Item( group + @"\Particle Emitter", typeof( Component_ParticleEmitter ), true ) );
				//Items.Add( new Item( group + @"\Particle System", typeof( Component_ParticleSystem ), true ) );
				//Items.Add( new Item( group + @"\Light Probe", typeof( Component_LightProbe ), true ) );
				//Items.Add( new Item( group + @"\Liquid Surface", typeof( Component_LiquidSurface ), true ) );
			}

			//Primitives
			{
				var group = @"Base\Primitives";//"Simple meshes"

				Items.Add( new Item( group + @"\Arch", typeof( Component_MeshGeometry_Arch ) ) );
				Items.Add( new Item( group + @"\Box", typeof( Component_MeshGeometry_Box ) ) );
				Items.Add( new Item( group + @"\Capsule", typeof( Component_MeshGeometry_Capsule ) ) );
				Items.Add( new Item( group + @"\Cone", typeof( Component_MeshGeometry_Cone ) ) );
				Items.Add( new Item( group + @"\Cylinder", typeof( Component_MeshGeometry_Cylinder ) ) );
				Items.Add( new Item( group + @"\Door", typeof( Component_MeshGeometry_Door ) ) );
				Items.Add( new Item( group + @"\Pipe", typeof( Component_MeshGeometry_Pipe ) ) );
				Items.Add( new Item( group + @"\Plane", typeof( Component_MeshGeometry_Plane ) ) );
				Items.Add( new Item( group + @"\Prism", typeof( Component_MeshGeometry_Prism ) ) );
				Items.Add( new Item( group + @"\Sphere", typeof( Component_MeshGeometry_Sphere ) ) );
				Items.Add( new Item( group + @"\Stairs", typeof( Component_MeshGeometry_Stairs ) ) );
				Items.Add( new Item( group + @"\Torus", typeof( Component_MeshGeometry_Torus ) ) );
				Items.Add( new Item( group + @"\Polygon Based Polyhedron", typeof( Component_MeshGeometry_PolygonBasedPolyhedron ) ) );
			}

			//Physics
			{
				var group = @"Base\Physics";

				Items.Add( new Item( group + @"\Rigid Body", typeof( Component_RigidBody ) ) );

				Items.Add( new Item( group + @"\Box Shape", typeof( Component_CollisionShape_Box ) ) );
				Items.Add( new Item( group + @"\Sphere Shape", typeof( Component_CollisionShape_Sphere ) ) );
				Items.Add( new Item( group + @"\Cylinder Shape", typeof( Component_CollisionShape_Cylinder ) ) );
				Items.Add( new Item( group + @"\Cone Shape", typeof( Component_CollisionShape_Cone ) ) );
				Items.Add( new Item( group + @"\Capsule Shape", typeof( Component_CollisionShape_Capsule ) ) );
				Items.Add( new Item( group + @"\Mesh Shape", typeof( Component_CollisionShape_Mesh ) ) );
				Items.Add( new Item( group + @"\Physical Material", typeof( Component_PhysicalMaterial ) ) );
				Items.Add( new Item( group + @"\Soft Body", typeof( Component_SoftBody ) ) );
				Items.Add( new Item( group + @"\Constraint", typeof( Component_Constraint ) ) );
				Items.Add( new Item( group + @"\Sensor", typeof( Component_Sensor ) ) );
				//group.Types.Add( new TypeItem( subgroup,"Collision Trigger", typeof( Component_CollisionTrigger ), true ) );
				//group.Types.Add( new TypeItem( subgroup,"Physical Liquid Surface", typeof( Component_PhysicalLiquidSurface ), true ) );
			}

			//Particles
			{
				var groupParticles = @"Base\Particles";
				Items.Add( new Item( groupParticles + @"\Particle System", typeof( Component_ParticleSystem ) ) );
				Items.Add( new Item( groupParticles + @"\Particle Emitter", typeof( Component_ParticleEmitter ) ) );

				var groupParticlesShapes = @"Base\Particles\Shapes";
				Items.Add( new Item( groupParticlesShapes + @"\Box Shape", typeof( Component_ParticleEmitterShape_Box ) ) );
				Items.Add( new Item( groupParticlesShapes + @"\Point Shape", typeof( Component_ParticleEmitterShape_Point ) ) );
				Items.Add( new Item( groupParticlesShapes + @"\Sphere Shape", typeof( Component_ParticleEmitterShape_Sphere ) ) );
				Items.Add( new Item( groupParticlesShapes + @"\Cylinder Shape", typeof( Component_ParticleEmitterShape_Cylinder ) ) );
				Items.Add( new Item( groupParticlesShapes + @"\Custom Shape", typeof( Component_ParticleEmitterShape ) ) );

				var groupParticlesModules = @"Base\Particles\Modules";
				Items.Add( new Item( groupParticlesModules + @"\Size Multiplier By Time", typeof( Component_ParticleSizeMultiplierByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Color Multiplier By Time", typeof( Component_ParticleColorMultiplierByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Linear Acceleration By Time", typeof( Component_ParticleLinearAccelerationByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Linear Velocity By Time", typeof( Component_ParticleLinearVelocityByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Linear Speed Multiplier By Time", typeof( Component_ParticleLinearSpeedMultiplierByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Angular Acceleration By Time", typeof( Component_ParticleAngularAccelerationByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Angular Velocity By Time", typeof( Component_ParticleAngularVelocityByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Angular Speed Multiplier By Time", typeof( Component_ParticleAngularSpeedMultiplierByTime ) ) );
				Items.Add( new Item( groupParticlesModules + @"\Custom Module", typeof( Component_ParticleModuleCustom ) ) );
			}

			//Screen effects
			{
				{
					var group = @"Base\Screen effects";

					var list = new List<(string, string)>();
					//list.Add( ("Ambient Occlusion", "Component_RenderingEffect_AmbientOcclusion") );
					//list.Add( ("Antialiasing", "Component_RenderingEffect_Antialiasing") );
					//list.Add( ("Bloom", "Component_RenderingEffect_Bloom") );
					list.Add( ("Chromatic Aberration", "Component_RenderingEffect_ChromaticAberration") );
					list.Add( ("Color Grading", "Component_RenderingEffect_ColorGrading") );
					list.Add( ("Depth Of Field", "Component_RenderingEffect_DepthOfField") );
					list.Add( ("Edge Detection", "Component_RenderingEffect_EdgeDetection") );
					list.Add( ("Gaussian Blur", "Component_RenderingEffect_GaussianBlur") );
					list.Add( ("Grayscale", "Component_RenderingEffect_Grayscale") );
					//list.Add( ("Lens Effects", "Component_RenderingEffect_LensEffects") );
					list.Add( ("Light Shafts", "Component_RenderingEffect_LightShafts") );
					//list.Add( ("Motion Blur", "Component_RenderingEffect_MotionBlur") );
					list.Add( ("Noise", "Component_RenderingEffect_Noise") );
					list.Add( ("Pixelate", "Component_RenderingEffect_Pixelate") );
					list.Add( ("Posterize", "Component_RenderingEffect_Posterize") );
					list.Add( ("Radial Blur", "Component_RenderingEffect_RadialBlur") );
					list.Add( ("Screen Space Reflection", "Component_RenderingEffect_ScreenSpaceReflection") );
					//list.Add( ("Sharpen", "Component_RenderingEffect_Sharpen") );
					//list.Add( ("Show Render Target", "Component_RenderingEffect_ShowRenderTarget") );
					//list.Add( ("To LDR", "Component_RenderingEffect_ToLDR") );
					//list.Add( ("Tone Mapping", "Component_RenderingEffect_ToneMapping") );
					list.Add( ("Vignetting", "Component_RenderingEffect_Vignetting") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							Items.Add( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}

				{
					var group = @"Base\Screen effects\Added by default";

					var list = new List<(string, string)>();
					list.Add( ("Ambient Occlusion", "Component_RenderingEffect_AmbientOcclusion") );
					list.Add( ("Antialiasing", "Component_RenderingEffect_Antialiasing") );
					list.Add( ("Bloom", "Component_RenderingEffect_Bloom") );
					list.Add( ("Lens Effects", "Component_RenderingEffect_LensEffects") );
					list.Add( ("Motion Blur", "Component_RenderingEffect_MotionBlur") );
					list.Add( ("Sharpen", "Component_RenderingEffect_Sharpen") );
					list.Add( ("To LDR", "Component_RenderingEffect_ToLDR") );
					list.Add( ("Tone Mapping", "Component_RenderingEffect_ToneMapping") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							Items.Add( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}

				{
					var group = @"Base\Screen effects\Special";

					var list = new List<(string, string)>();
					list.Add( ("Show Render Target", "Component_RenderingEffect_ShowRenderTarget") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							Items.Add( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}

			}
		}

		//static void AddSolutionItems()
		//{
		//	{
		//		var group = @"Solution";
		//		//!!!!
		//		//GroupDescriptions[ group ] = "Description text";
		//		Items.Add( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );


		//	}
		//}

		internal static void RegisterAssembly( Assembly assembly )
		{
			var toAdd = new List<Item>();

			foreach( var type in assembly.GetExportedTypes() )
			{
				foreach( var attrib in type.GetCustomAttributes<AddToResourcesWindowAttribute>() )
					toAdd.Add( new Item( attrib.Path, type, attrib.SortOrder, attrib.Disabled ) );
			}

			itemsByAttribute.AddRange( toAdd );

			CollectionUtility.MergeSort( itemsByAttribute, delegate ( Item item1, Item item2 )
			{
				if( item1.SortOrder < item2.SortOrder )
					return -1;
				if( item1.SortOrder > item2.SortOrder )
					return 1;

				string s1 = item1.Path + " ";
				string s2 = item2.Path + " ";
				return string.Compare( s1, s2 );
			} );

			//CollectionUtility.MergeSort( toAdd, delegate ( Item item1, Item item2 )
			//{
			//	if( item1.SortOrder < item2.SortOrder )
			//		return -1;
			//	if( item1.SortOrder > item2.SortOrder )
			//		return 1;

			//	string s1 = item1.Path + " ";
			//	string s2 = item2.Path + " ";
			//	return string.Compare( s1, s2 );
			//} );

			//itemsByAttribute.AddRange( toAdd );
		}

		public static Item GetItemByPath( string path )
		{
			return Items.FirstOrDefault( item => item.Path == path );
		}
	}
}
