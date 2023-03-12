#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NeoAxis.Editor
{
	public static class ResourcesWindowItems
	{
		static List<Item> itemsFromAssembliesByAttribute = new List<Item>();

		static bool itemsPrepared;
		static List<Item> items = new List<Item>();
		static Dictionary<Type, Item> itemByType = new Dictionary<Type, Item>();
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
				Path = path;
				Type = type;
				SortOrder = sortOrder;
				Disabled = disabled;
			}

			public override string ToString()
			{
				return Path;
			}
		}

		/////////////////////////////////////////

		public static IReadOnlyList<Item> Items
		{
			get
			{
				PrepareItems();
				return items;
			}
		}

		public static Dictionary<string, string> GroupDescriptions
		{
			get
			{
				PrepareItems();
				return groupDescriptions;
			}
		}

		public static event Action FixItems;

		static void PrepareItems()
		{
			if( itemsPrepared )
				return;

			items.Clear();
			itemByType.Clear();
			groupDescriptions.Clear();

			AddBaseItems();
			//AddSolutionItems();

			//add items by attribute
			CollectionUtility.MergeSort( itemsFromAssembliesByAttribute, delegate ( Item item1, Item item2 )
			{
				if( item1.SortOrder < item2.SortOrder )
					return -1;
				if( item1.SortOrder > item2.SortOrder )
					return 1;

				string s1 = item1.Path + " ";
				string s2 = item2.Path + " ";
				return string.Compare( s1, s2 );
			} );
			items.AddRange( itemsFromAssembliesByAttribute );

			FixItems?.Invoke();

			itemsPrepared = true;
		}

		public static void AddItem( Item item )
		{
			items.Add( item );
			itemByType[ item.Type ] = item;
		}

		static void AddBaseItems()
		{
#if !DEPLOY

			//Common
			{
				var group = @"Base\Common";
				//!!!!
				//GroupDescriptions[ group ] = "Description text";
				AddItem( new Item( group + @"\Component", typeof( Component ) ) );
				AddItem( new Item( group + @"\Text File", typeof( NewResourceType_TextFile ) ) );

				AddItem( new Item( group + @"\Product\Store", typeof( Product_Store ) ) );
				//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.Standalone )
				//{
				AddItem( new Item( group + @"\Product\Windows", typeof( Product_Windows ) ) );
				AddItem( new Item( group + @"\Product\Android", typeof( Product_Android ) ) );
				AddItem( new Item( group + @"\Product\iOS", typeof( Product_iOS ) ) );
				AddItem( new Item( group + @"\Product\UWP", typeof( Product_UWP ) ) );
				//}

				////!!!!убрать "The feature is not implemented."
				//AddItem( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ), disabled: true ) );
				//AddItem( new Item( group + @"\Executable App", typeof( NewResourceType_ExecutableApp ), disabled: true ) );
			}

			//Scripting
			{
				//Common
				{
					var group = @"Base\Scripting";// Common\";
					AddItem( new Item( group + @"\C# File", typeof( NewResourceType_CSharpClass ) ) );
					AddItem( new Item( group + @"\C# Script", typeof( CSharpScript ) ) );
					AddItem( new Item( group + @"\Flow Graph", typeof( FlowGraph ) ) );
					//group.types.Add( new Item( "C# library", typeof( NewResourceType_CSharpClassLibrary ), true ) );

					//AddItem( new Item( group + "Event Handler", typeof( EventHandler ) ) );
				}

				//Metadata
				{
					var group = @"Base\Scripting\Metadata";
					AddItem( new Item( group + @"\Property", typeof( VirtualProperty ) ) );
					AddItem( new Item( group + @"\Method", typeof( VirtualMethod ) ) );
					//AddItem( new Item( group + @"\Event", typeof( VirtualEvent ), true ) );
					AddItem( new Item( group + @"\Member Parameter", typeof( VirtualMemberParameter ) ) );
				}

				//Flow scripting
				{
					var group = @"Base\Scripting\Flow scripting";
					AddItem( new Item( group + @"\Invoke Member", typeof( InvokeMember ) ) );
					AddItem( new Item( group + @"\Event Handler", typeof( EventHandlerComponent ) ) );
					AddItem( new Item( group + @"\Declare Variable", typeof( FlowDeclareVariable ) ) );
					AddItem( new Item( group + @"\Set Variable", typeof( FlowSetVariable ) ) );
					AddItem( new Item( group + @"\If", typeof( FlowIf ) ) );
					AddItem( new Item( group + @"\Switch", typeof( FlowSwitch ) ) );
					AddItem( new Item( group + @"\While", typeof( FlowWhile ) ) );
					AddItem( new Item( group + @"\Do While", typeof( FlowDoWhile ) ) );
					AddItem( new Item( group + @"\Do Number", typeof( FlowDoNumber ) ) );
					AddItem( new Item( group + @"\For Each", typeof( FlowForEach ) ) );
					AddItem( new Item( group + @"\For", typeof( FlowFor ) ) );
					AddItem( new Item( group + @"\Sequence", typeof( FlowSequence ) ) );
					AddItem( new Item( group + @"\Flow Start", typeof( FlowStart ) ) );
					AddItem( new Item( group + @"\Convert To", typeof( FlowConvertTo ) ) );
					AddItem( new Item( group + @"\Sleep", typeof( FlowSleep ) ) );
				}

				//Shader scripting
				{
					var group = @"Base\Scripting\Shader scripting";
					AddItem( new Item( group + @"\Shader Script", typeof( ShaderScript ) ) );
				}

				////!!!!nb
				////C# projects
				//{
				//	var group = @"Base\Scripting\C# projects";

				//	{
				//		var item = new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		AddItem( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		AddItem( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		AddItem( item );
				//	}

				//	{
				//		var item = new Item( group + @"\C# NeoAxis Addon", typeof( NewResourceType_CSharpClassLibrary ) );
				//		item.Disabled = true;
				//		AddItem( item );
				//	}

				//	//AddItem( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) ) );

				//	//AddItem( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\C# Windows Forms App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\C# WPF App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\NeoAxis Editor App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//	//AddItem( new Item( group + @"\NeoAxis Simulation App", typeof( NewResourceType_CSharpClassLibrary ) ) );
				//}

			}

			//UI
			{
				{
					var group = @"Base\UI";
					AddItem( new Item( group + @"\Control", typeof( UIControl ) ) );
					AddItem( new Item( group + @"\Window", typeof( UIWindow ) ) );
					AddItem( new Item( group + @"\Text", typeof( UIText ) ) );
					AddItem( new Item( group + @"\Image", typeof( UIImage ) ) );
					AddItem( new Item( group + @"\Button", typeof( UIButton ) ) );
					AddItem( new Item( group + @"\Check", typeof( UICheck ) ) );
					AddItem( new Item( group + @"\Edit", typeof( UIEdit ) ) );
					AddItem( new Item( group + @"\Slider", typeof( UISlider ) ) );
					AddItem( new Item( group + @"\Progress", typeof( UIProgress ) ) );
					AddItem( new Item( group + @"\Scroll", typeof( UIScroll ) ) );
					AddItem( new Item( group + @"\Combo", typeof( UICombo ) ) );
					AddItem( new Item( group + @"\List", typeof( UIList ) ) );
					//AddItem( new Item( group + @"\Tree", typeof( UITree ) ) );
					AddItem( new Item( group + @"\Video", typeof( UIVideo ) ) );
					AddItem( new Item( group + @"\Web Browser", typeof( UIWebBrowser ) ) );
					AddItem( new Item( group + @"\Render Target", typeof( UIRenderTarget ) ) );
					//AddItem( new Item( group + @"\Menu", typeof( UIMenu ) ) );
				}

				//Containers
				{
					var group = @"Base\UI\Containers";
					AddItem( new Item( group + @"\Tab Control", typeof( UITabControl ) ) );
					//AddItem( new Item( group + @"\Split Container", typeof( UISplitContainer ) ) );
					AddItem( new Item( group + @"\Grid", typeof( UIGrid ) ) );
					AddItem( new Item( group + @"\Toolbar", typeof( UIToolbar ) ) );
				}

				//More
				{
					var group = @"Base\UI\More";
					AddItem( new Item( group + @"\Style", typeof( UIStyle ) ) );
					AddItem( new Item( group + @"\Tooltip", typeof( UITooltip ) ) );
				}
			}

			//Common 3D
			{
				var group = @"Base\Scene common";
				AddItem( new Item( group + @"\Scene", typeof( Scene ) ) );
				AddItem( new Item( group + @"\Material", typeof( Material ) ) );
				AddItem( new Item( group + @"\Multi Material", typeof( MultiMaterial ) ) );
				AddItem( new Item( group + @"\Surface", typeof( Surface ) ) );
				AddItem( new Item( group + @"\Image", typeof( ImageComponent ) ) );
				AddItem( new Item( group + @"\Mesh", typeof( Mesh ) ) );
				AddItem( new Item( group + @"\Mesh In Space Animation Controller", typeof( MeshInSpaceAnimationController ) ) );
				AddItem( new Item( group + @"\Paint Layer", typeof( PaintLayer ) ) );
				AddItem( new Item( group + @"\Rendering Pipeline Basic", typeof( RenderingPipeline_Basic ) ) );
			}

			//Scene objects
			{
				var group = @"Base\Scene objects";

				AddItem( new Item( group + @"\Object In Space", typeof( ObjectInSpace ) ) );
				AddItem( new Item( group + @"\Group Of Objects", typeof( GroupOfObjects ) ) );
				AddItem( new Item( group + @"\Layer", typeof( Layer ) ) );
				AddItem( new Item( group + @"\Skybox", typeof( Skybox ) ) );
				AddItem( new Item( group + @"\Fog", typeof( Fog ) ) );
				AddItem( new Item( group + @"\Terrain", typeof( Terrain ) ) );
				//AddItem( new Item( group + @"\Mesh In Space", typeof( MeshInSpace ) ) );
				//AddItem( new Item( group + @"\Mesh In Space Animation Controller", typeof( MeshInSpaceAnimationController ) ) );
				//AddItem( new Item( group + @"\Particle System In Space", typeof( ParticleSystemInSpace ) ) );
				AddItem( new Item( group + @"\Light", typeof( Light ) ) );
				AddItem( new Item( group + @"\Lens Flares", typeof( LensFlares ) ) );
				AddItem( new Item( group + @"\Camera", typeof( Camera ) ) );
				AddItem( new Item( group + @"\Billboard", typeof( Billboard ) ) );
				AddItem( new Item( group + @"\Decal", typeof( Decal ) ) );
				AddItem( new Item( group + @"\Reflection Probe", typeof( ReflectionProbe ) ) );
				AddItem( new Item( group + @"\Sound Source", typeof( SoundSource ) ) );
				//AddItem( new Item( group + @"\Render Target In Space", typeof( RenderTargetInSpace ) ) );
				//AddItem( new Item( group + @"\Additional\Text 2D", typeof( Text2D ) ) );
				AddItem( new Item( group + @"\Additional\Curve In Space", typeof( CurveInSpace ) ) );
				AddItem( new Item( group + @"\Additional\Curve In Space Objects", typeof( CurveInSpaceObjects ) ) );
			}

			//Primitives
			{
				var group = @"Base\Primitives";

				AddItem( new Item( group + @"\Arch", typeof( MeshGeometry_Arch ) ) );
				AddItem( new Item( group + @"\Box", typeof( MeshGeometry_Box ) ) );
				AddItem( new Item( group + @"\Capsule", typeof( MeshGeometry_Capsule ) ) );
				AddItem( new Item( group + @"\Cone", typeof( MeshGeometry_Cone ) ) );
				AddItem( new Item( group + @"\Cylinder", typeof( MeshGeometry_Cylinder ) ) );
				AddItem( new Item( group + @"\Door", typeof( MeshGeometry_Door ) ) );
				AddItem( new Item( group + @"\Pipe", typeof( MeshGeometry_Pipe ) ) );
				AddItem( new Item( group + @"\Plane", typeof( MeshGeometry_Plane ) ) );
				AddItem( new Item( group + @"\Prism", typeof( MeshGeometry_Prism ) ) );
				AddItem( new Item( group + @"\Sphere", typeof( MeshGeometry_Sphere ) ) );
				AddItem( new Item( group + @"\Stairs", typeof( MeshGeometry_Stairs ) ) );
				AddItem( new Item( group + @"\Torus", typeof( MeshGeometry_Torus ) ) );
				AddItem( new Item( group + @"\Polygon Based Polyhedron", typeof( MeshGeometry_PolygonBasedPolyhedron ) ) );
			}

			//Physics
			{
				var group = @"Base\Physics";

				AddItem( new Item( group + @"\Rigid Body", typeof( RigidBody ) ) );

				AddItem( new Item( group + @"\Box Shape", typeof( CollisionShape_Box ) ) );
				AddItem( new Item( group + @"\Sphere Shape", typeof( CollisionShape_Sphere ) ) );
				AddItem( new Item( group + @"\Cylinder Shape", typeof( CollisionShape_Cylinder ) ) );
				AddItem( new Item( group + @"\Cone Shape", typeof( CollisionShape_Cone ) ) );
				AddItem( new Item( group + @"\Capsule Shape", typeof( CollisionShape_Capsule ) ) );
				AddItem( new Item( group + @"\Mesh Shape", typeof( CollisionShape_Mesh ) ) );
				AddItem( new Item( group + @"\Physical Material", typeof( PhysicalMaterial ) ) );
				//AddItem( new Item( group + @"\Soft Body", typeof( SoftBody ) ) );
				AddItem( new Item( group + @"\Constraint 6DOF", typeof( Constraint_SixDOF ) ) );
				//AddItem( new Item( group + @"\Sensor", typeof( Sensor ) ) );
				//group.Types.Add( new TypeItem( subgroup,"Collision Trigger", typeof( CollisionTrigger ), true ) );
				//group.Types.Add( new TypeItem( subgroup,"Physical Liquid Surface", typeof( PhysicalLiquidSurface ), true ) );
			}

			//Particles
			{
				var groupParticles = @"Base\Particles";
				AddItem( new Item( groupParticles + @"\Particle System", typeof( ParticleSystem ) ) );
				AddItem( new Item( groupParticles + @"\Particle Emitter", typeof( ParticleEmitter ) ) );

				var groupParticlesShapes = @"Base\Particles\Shapes";
				AddItem( new Item( groupParticlesShapes + @"\Box Shape", typeof( ParticleEmitterShape_Box ) ) );
				AddItem( new Item( groupParticlesShapes + @"\Point Shape", typeof( ParticleEmitterShape_Point ) ) );
				AddItem( new Item( groupParticlesShapes + @"\Sphere Shape", typeof( ParticleEmitterShape_Sphere ) ) );
				AddItem( new Item( groupParticlesShapes + @"\Cylinder Shape", typeof( ParticleEmitterShape_Cylinder ) ) );
				AddItem( new Item( groupParticlesShapes + @"\Custom Shape", typeof( ParticleEmitterShape ) ) );

				var groupParticlesModules = @"Base\Particles\Modules";
				AddItem( new Item( groupParticlesModules + @"\Size Multiplier By Time", typeof( ParticleSizeMultiplierByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Color Multiplier By Time", typeof( ParticleColorMultiplierByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Linear Acceleration By Time", typeof( ParticleLinearAccelerationByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Linear Velocity By Time", typeof( ParticleLinearVelocityByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Linear Speed Multiplier By Time", typeof( ParticleLinearSpeedMultiplierByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Angular Acceleration By Time", typeof( ParticleAngularAccelerationByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Angular Velocity By Time", typeof( ParticleAngularVelocityByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Angular Speed Multiplier By Time", typeof( ParticleAngularSpeedMultiplierByTime ) ) );
				AddItem( new Item( groupParticlesModules + @"\Custom Module", typeof( ParticleModuleCustom ) ) );
			}

			//Screen effects
			{
				{
					var group = @"Base\Screen effects";

					var list = new List<(string, string)>();
					list.Add( ("Bloom", "RenderingEffect_Bloom") );
					list.Add( ("Bokeh Blur", "RenderingEffect_BokehBlur") );
					list.Add( ("Chromatic Aberration", "RenderingEffect_ChromaticAberration") );
					list.Add( ("Color Grading", "RenderingEffect_ColorGrading") );
					list.Add( ("Depth Of Field", "RenderingEffect_DepthOfField") );
					list.Add( ("Edge Detection", "RenderingEffect_EdgeDetection") );
					list.Add( ("Gaussian Blur", "RenderingEffect_GaussianBlur") );
					list.Add( ("Grayscale", "RenderingEffect_Grayscale") );
					list.Add( ("Indirect Lighting", "RenderingEffect_IndirectLighting") );
					list.Add( ("Light Shafts", "RenderingEffect_LightShafts") );
					list.Add( ("Noise", "RenderingEffect_Noise") );
					list.Add( ("Outline", "RenderingEffect_Outline") );
					list.Add( ("Pixelate", "RenderingEffect_Pixelate") );
					list.Add( ("Posterize", "RenderingEffect_Posterize") );
					list.Add( ("Radial Blur", "RenderingEffect_RadialBlur") );
					list.Add( ("Reflection", "RenderingEffect_Reflection") );
					list.Add( ("Vignetting", "RenderingEffect_Vignetting") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							AddItem( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}

				{
					var group = @"Base\Screen effects\Added by default";

					var list = new List<(string, string)>();
					list.Add( ("Ambient Occlusion", "RenderingEffect_AmbientOcclusion") );
					list.Add( ("Antialiasing", "RenderingEffect_Antialiasing") );
					list.Add( ("Lens Effects", "RenderingEffect_LensEffects") );
					list.Add( ("Motion Blur", "RenderingEffect_MotionBlur") );
					list.Add( ("Resolution Upscale", "RenderingEffect_ResolutionUpscale") );
					list.Add( ("Sharpen", "RenderingEffect_Sharpen") );
					list.Add( ("To LDR", "RenderingEffect_ToLDR") );
					list.Add( ("Tone Mapping", "RenderingEffect_ToneMapping") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							AddItem( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}

				{
					var group = @"Base\Screen effects\Special";

					var list = new List<(string, string)>();
					list.Add( ("Script", "RenderingEffect_Script") );
					list.Add( ("Show Render Target", "RenderingEffect_ShowRenderTarget") );

					foreach( var item in list )
					{
						var type = MetadataManager.GetType( "NeoAxis." + item.Item2 );
						if( type != null )
							AddItem( new Item( group + "\\" + item.Item1, type.GetNetType() ) );
					}
				}
			}

#endif
		}

		//static void AddSolutionItems()
		//{
		//	{
		//		var group = @"Solution";
		//		//!!!!
		//		//GroupDescriptions[ group ] = "Description text";
		//		AddItem( new Item( group + @"\C# Class Library", typeof( NewResourceType_CSharpClassLibrary ) ) );


		//	}
		//}

		internal static void RegisterAssembly( Type[] exportedTypes )
		{
			var toAdd = new List<Item>();
			foreach( var type in exportedTypes )
			{
				foreach( var attrib in type.GetCustomAttributes<AddToResourcesWindowAttribute>() )
					toAdd.Add( new Item( attrib.Path, type, attrib.SortOrder, attrib.Disabled ) );
			}
			itemsFromAssembliesByAttribute.AddRange( toAdd );

			if( toAdd.Count != 0 )
				itemsPrepared = false;
		}

		internal static void UnregisterAssembly( Assembly assembly )
		{
			var newList = new List<Item>();

			foreach( var item in itemsFromAssembliesByAttribute )
			{
				if( item.Type.Assembly != assembly )
					newList.Add( item );
			}

			if( newList.Count != itemsFromAssembliesByAttribute.Count )
				itemsPrepared = false;

			itemsFromAssembliesByAttribute = newList;
		}

		public static Item GetItemByPath( string path )
		{
			return Items.FirstOrDefault( item => item.Path == path );
		}

		public static Item GetItemByType( Type type )
		{
			PrepareItems();

			itemByType.TryGetValue( type, out var item );
			return item;
		}
	}
}

#endif