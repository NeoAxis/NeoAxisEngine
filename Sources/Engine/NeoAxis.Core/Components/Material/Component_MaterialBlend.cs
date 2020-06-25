//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Text;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Reflection;
//using System.IO;

//namespace NeoAxis
//{
//	[EditorNewObjectSettings( typeof( NewObjectSettingsMaterialBlend ) )]
//	public class Component_MaterialBlend : Component_Material
//	{
//		//!!!!ReferenceList?
//		//!!!!material Standard?

//		//Material1
//		ReferenceField<Component_MaterialStandard> _material1 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material1
//		{
//			get { if( _material1.BeginGet() ) Material1 = _material1.Get( this ); return _material1.value; }
//			set { if( _material1.BeginSet( ref value ) ) { try { Material1Changed?.Invoke( this ); } finally { _material1.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material1Changed;

//		//Material2
//		ReferenceField<Component_MaterialStandard> _Material2 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material2
//		{
//			get { if( _Material2.BeginGet() ) Material2 = _Material2.Get( this ); return _Material2.value; }
//			set { if( _Material2.BeginSet( ref value ) ) { try { Material2Changed?.Invoke( this ); } finally { _Material2.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material2Changed;

//		//Mask2
//		ReferenceField<double> _mask2 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask2
//		{
//			get { if( _mask2.BeginGet() ) Mask2 = _mask2.Get( this ); return _mask2.value; }
//			set { if( _mask2.BeginSet( ref value ) ) { try { Mask2Changed?.Invoke( this ); } finally { _mask2.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask2Changed;

//		//Material3
//		ReferenceField<Component_MaterialStandard> _Material3 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material3
//		{
//			get { if( _Material3.BeginGet() ) Material3 = _Material3.Get( this ); return _Material3.value; }
//			set { if( _Material3.BeginSet( ref value ) ) { try { Material3Changed?.Invoke( this ); } finally { _Material3.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material3Changed;

//		//Mask3
//		ReferenceField<double> _mask3 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask3
//		{
//			get { if( _mask3.BeginGet() ) Mask3 = _mask3.Get( this ); return _mask3.value; }
//			set { if( _mask3.BeginSet( ref value ) ) { try { Mask3Changed?.Invoke( this ); } finally { _mask3.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask3Changed;

//		//Material4
//		ReferenceField<Component_MaterialStandard> _Material4 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material4
//		{
//			get { if( _Material4.BeginGet() ) Material4 = _Material4.Get( this ); return _Material4.value; }
//			set { if( _Material4.BeginSet( ref value ) ) { try { Material4Changed?.Invoke( this ); } finally { _Material4.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material4Changed;

//		//Mask4
//		ReferenceField<double> _mask4 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask4
//		{
//			get { if( _mask4.BeginGet() ) Mask4 = _mask4.Get( this ); return _mask4.value; }
//			set { if( _mask4.BeginSet( ref value ) ) { try { Mask4Changed?.Invoke( this ); } finally { _mask4.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask4Changed;

//		//Material5
//		ReferenceField<Component_MaterialStandard> _Material5 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material5
//		{
//			get { if( _Material5.BeginGet() ) Material5 = _Material5.Get( this ); return _Material5.value; }
//			set { if( _Material5.BeginSet( ref value ) ) { try { Material5Changed?.Invoke( this ); } finally { _Material5.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material5Changed;

//		//Mask5
//		ReferenceField<double> _mask5 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask5
//		{
//			get { if( _mask5.BeginGet() ) Mask5 = _mask5.Get( this ); return _mask5.value; }
//			set { if( _mask5.BeginSet( ref value ) ) { try { Mask5Changed?.Invoke( this ); } finally { _mask5.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask5Changed;

//		//Material6
//		ReferenceField<Component_MaterialStandard> _Material6 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material6
//		{
//			get { if( _Material6.BeginGet() ) Material6 = _Material6.Get( this ); return _Material6.value; }
//			set { if( _Material6.BeginSet( ref value ) ) { try { Material6Changed?.Invoke( this ); } finally { _Material6.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material6Changed;

//		//Mask6
//		ReferenceField<double> _mask6 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask6
//		{
//			get { if( _mask6.BeginGet() ) Mask6 = _mask6.Get( this ); return _mask6.value; }
//			set { if( _mask6.BeginSet( ref value ) ) { try { Mask6Changed?.Invoke( this ); } finally { _mask6.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask6Changed;

//		//Material7
//		ReferenceField<Component_MaterialStandard> _Material7 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material7
//		{
//			get { if( _Material7.BeginGet() ) Material7 = _Material7.Get( this ); return _Material7.value; }
//			set { if( _Material7.BeginSet( ref value ) ) { try { Material7Changed?.Invoke( this ); } finally { _Material7.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material7Changed;

//		//Mask7
//		ReferenceField<double> _mask7 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask7
//		{
//			get { if( _mask7.BeginGet() ) Mask7 = _mask7.Get( this ); return _mask7.value; }
//			set { if( _mask7.BeginSet( ref value ) ) { try { Mask7Changed?.Invoke( this ); } finally { _mask7.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask7Changed;

//		//Material8
//		ReferenceField<Component_MaterialStandard> _Material8 = null;
//		[DefaultValue( null )]
//		[Serialize]
//		public Reference<Component_MaterialStandard> Material8
//		{
//			get { if( _Material8.BeginGet() ) Material8 = _Material8.Get( this ); return _Material8.value; }
//			set { if( _Material8.BeginSet( ref value ) ) { try { Material8Changed?.Invoke( this ); } finally { _Material8.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Material8Changed;

//		//Mask8
//		ReferenceField<double> _mask8 = 1.0;
//		[DefaultValue( 1.0 )]
//		[Serialize]
//		public Reference<double> Mask8
//		{
//			get { if( _mask8.BeginGet() ) Mask8 = _mask8.Get( this ); return _mask8.value; }
//			set { if( _mask8.BeginSet( ref value ) ) { try { Mask8Changed?.Invoke( this ); } finally { _mask8.EndSet(); } } }
//		}
//		public event Action<Component_MaterialBlend> Mask8Changed;

//		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
//		{
//			base.OnMetadataGetMembersFilter( context, member, ref skip );

//			var p = member as Metadata.Property;
//			if( p != null )
//			{
//				switch( p.Name )
//				{
//				case nameof( Mask2 ):
//					if( !Material2.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask3 ):
//					if( !Material3.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask4 ):
//					if( !Material4.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask5 ):
//					if( !Material5.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask6 ):
//					if( !Material6.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask7 ):
//					if( !Material7.ReferenceSpecified )
//						skip = true;
//					break;
//				case nameof( Mask8 ):
//					if( !Material8.ReferenceSpecified )
//						skip = true;
//					break;
//				}
//			}
//		}

//		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//		public class NewObjectSettingsMaterialBlend : NewObjectSettings
//		{
//			bool createFlowchart = true;

//			[DefaultValue( true )]
//			[Category( "Options" )]
//			public bool CreateFlowchart
//			{
//				get { return createFlowchart; }
//				set { createFlowchart = value; }
//			}

//			public override bool Creation( NewObjectCell.ObjectCreationContext context )
//			{
//				var newObject2 = (Component)context.newObject;

//				if( CreateFlowchart )
//				{
//					var flowchart = newObject2.CreateComponent<Component_Flowchart>();
//					flowchart.Name = "Flowchart";
//					flowchart.Specialization = ReferenceUtils.CreateReference<Component_FlowchartSpecialization>( null,
//						MetadataManager.GetTypeOfNetType( typeof( Component_FlowchartSpecialization_Shaders ) ).Name + "|Instance" );

//					var node = flowchart.CreateComponent<Component_FlowchartNode>();
//					node.Name = "Node " + newObject2.Name;
//					node.NodePosition = new Vec2I( 10, -7 );
//					node.ControlledObject = new Reference<Component>( null, ReferenceUtils.CalculateThisReference( node, newObject2 ) );

//					//!!!!выделять, открывать созданные. везде так
//				}

//				return true;
//			}
//		}

//		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//		public class CompiledDataBlend : CompiledData
//		{
//			//!!!!public

//			//public Component_MaterialBlend owner;

//			public Component_MaterialStandard.CompiledDataStandard[] materialsCompiledData;

//			//public string error;

//			//

//			//!!!!надо ли? для GpuMaterialPass не нужно
//			public override void Dispose()
//			{
//				if( materialsCompiledData != null )
//				{
//					foreach( var data in materialsCompiledData )
//						data.Dispose();
//					materialsCompiledData = null;
//				}

//				base.Dispose();
//			}
//		}

//		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//		Component_MaterialStandard GetMaterialByIndex( int index )
//		{
//			switch( index )
//			{
//			case 1: return Material1;
//			case 2: return Material2;
//			case 3: return Material3;
//			case 4: return Material4;
//			case 5: return Material5;
//			case 6: return Material6;
//			case 7: return Material7;
//			case 8: return Material8;
//			}
//			return null;
//		}

//		//IList<Component_MaterialStandard> GetMaterials()
//		//{
//		//	var result = new List<Component_MaterialStandard>();
//		//	for( int n = 1; n <= 8; n++ )
//		//	{
//		//		var material = GetMaterialByIndex( n );
//		//		if( material != null )
//		//			result.Add( material );
//		//	}
//		//	return result;
//		//}

//		Metadata.Property GetMaskPropertyByIndex( int index )
//		{
//			if( index >= 2 && index <= 8 )
//				return (Metadata.Property)MetadataGetMemberBySignature( "property:Mask" + index.ToString() );
//			return null;
//		}

//		protected override void OnResultCompile()
//		{
//			if( Result != null )
//				return;

//			var result = new CompiledDataBlend();
//			var materialsCompiledDataList = new List<Component_MaterialStandard.CompiledDataStandard>();

//			xx xx;//маск 1 должна быть 1.0 - маск2

//			xx xx;//а что если три материала. как тут маску?

//			for( int nMaterial = 1; nMaterial <= 8; nMaterial++ )
//			{
//				var material = GetMaterialByIndex( nMaterial );
//				if( material != null )
//				{
//					var maskProperty = GetMaskPropertyByIndex( nMaterial );

//					var compileMode = materialsCompiledDataList.Count == 0 ? Component_MaterialStandard.CompiledDataStandard.UsageMode.MaterialBlendFirst : Component_MaterialStandard.CompiledDataStandard.UsageMode.MaterialBlendNotFirst;

//					var extensionData = new Component_MaterialStandard.CompileExtensionData();
//					extensionData.fragmentShaderProperties.Add( (this, maskProperty) );

//					xx xx;//если нет ReferenceSpecified у маски

//					//!!!!special shadow caster

//					var materialCompiledData = material.Compile( compileMode, extensionData );

//					materialsCompiledDataList.Add( materialCompiledData );
//				}
//			}

//			result.materialsCompiledData = materialsCompiledDataList.ToArray();

//			xx xx;
//			//var materials = GetMaterials();


//			Component_MaterialStandard m;

//			xx xx;
//			m.Compile( Component_MaterialStandard.CompiledDataStandard.UsageMode.MaterialBlendFirst );

//			xx xx;
//			//materialsCompiledData


//			//!!!!

//			//var material2 = Material1.Value;
//			//if( material2 != null )//!!!! && ( Mask2.ReferenceSpecified || Mask2.Value != 1 ) )
//			//{
//			//	xx xx;

//			//	material2.Compile( Component_MaterialStandard.CompiledDataStandard.CompileMode.MaterialBlendFirst );
//			//}

//			//var material2 = Material1.Value;
//			//if( material2 != null )//!!!! && ( Mask2.ReferenceSpecified || Mask2.Value != 1 ) )
//			//{
//			//	xx xx;//? ссылки сломаются при клонировании. получается нужно создать новый и сослаться всеми свойствами на оригинал?
//			//	xx xx;//где еще так
//			//	xx xx;//ТУТ проблема посерьезнее. как это всё
//			//	xx xx;//импорт тоже подобно видать будет

//			//	var context = new Metadata.CloneContext();
//			//	context.typeOfCloning = Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType;
//			//	var newMaterial = (Component_MaterialStandard)material2.Clone( context );

//			//	//!!!!name?

//			//	newMaterial.Enabled = false;
//			//	newMaterial.ShowInEditor = false;
//			//	newMaterial.SaveSupport = false;
//			//	newMaterial.CloneSupport = false;
//			//	AddComponent( newMaterial );

//			//	xx xx;
//			//	newMaterial.BlendSupportMask = xxx;

//			//	newMaterial.Enabled = true;

//			//	newMaterial.xx = xx;
//			//}


//			//!!!!

//			////!!!!temp? надо ли знать о создателе? пока сделано, чтобы параметры материала получить
//			//result.owner = this;

//			////!!!!другие тоже
//			//result.Transparent = BlendMode.Value == BlendModeEnum.Translucent;

//			////deferred shading
//			////!!!!какие еще ограничения?
//			////!!!!!unlit?
//			////!!!!!невлезаемость в deferred
//			////!!!!!shading mode
//			////!!!!
//			////if( false )
//			//result.deferredShadingSupport = !result.Transparent;

//			////!!!!
//			//EngineThreading.ExecuteFromMainThreadWait( delegate ()
//			//{

//			//	//!!!!что еще?
//			//	bool needSpecialShadowCaster = PositionOffset.ReferenceSpecified || BlendMode.Value == BlendModeEnum.Masked || BlendMode.Value == BlendModeEnum.Translucent;

//			//	//shader generation
//			//	if( shaderGenerationCompile )
//			//	{
//			//		if( !GenerateCode( result, needSpecialShadowCaster ) )
//			//			return;
//			//	}


//			//	//base material

//			//	bool unlit = ShadingMode.Value == ShadingModeEnum.Unlit;
//			//	if( unlit )
//			//		result.passesByLightType = new CompiledDataBlend.PassGroup[ 1 ];
//			//	else
//			//	{
//			//		//!!!! 4
//			//		result.passesByLightType = new CompiledDataBlend.PassGroup[ 4 ];
//			//	}

//			//	foreach( Component_Light.TypeEnum lightType in Enum.GetValues( typeof( Component_Light.TypeEnum ) ) )
//			//	{
//			//		if( unlit && lightType != Component_Light.TypeEnum.Ambient )
//			//			break;

//			//		CompiledDataBlend.PassGroup group = new CompiledDataBlend.PassGroup();
//			//		result.passesByLightType[ (int)lightType ] = group;

//			//		//one or two iterations depending Ambient light source, ReceiveShadows
//			//		int shadowsSupportIterations = 1;
//			//		if( lightType != Component_Light.TypeEnum.Ambient && ReceiveShadows.Value )
//			//			shadowsSupportIterations = 2;
//			//		for( int nShadowsSupportCounter = 0; nShadowsSupportCounter < shadowsSupportIterations; nShadowsSupportCounter++ )
//			//		{
//			//			//generate compile arguments
//			//			var vertexDefines = new List<Tuple<string, string>>();
//			//			var fragmentDefines = new List<Tuple<string, string>>();
//			//			{
//			//				var generalDefines = new List<Tuple<string, string>>();
//			//				generalDefines.Add( Tuple.Create( "LIGHTTYPE_" + lightType.ToString().ToUpper(), "" ) );
//			//				generalDefines.Add( Tuple.Create( "BLENDMODE_" + BlendMode.Value.ToString().ToUpper(), "" ) );
//			//				generalDefines.Add( Tuple.Create( "SHADINGMODE_" + ShadingMode.Value.ToString().ToUpper(), "" ) );
//			//				if( TwoSided )
//			//					generalDefines.Add( Tuple.Create(XX "TWO_SIDED", "" ) );

//			//				//!!!!new
//			//				if( ClearCoat )
//			//					generalDefines.Add( Tuple.Create( "CLEAR_COAT", "" ) );
//			//				if( SubsurfaceScattering )
//			//					generalDefines.Add( Tuple.Create( "SUBSURFACE_SCATTERING", "" ) );

//			//				//receive shadows support
//			//				if( nShadowsSupportCounter != 0 )
//			//				{
//			//					generalDefines.Add( Tuple.Create( "SHADOW_MAP", "" ) );

//			//					//!!!!
//			//					generalDefines.Add( Tuple.Create( "SHADOW_MAP_LOW", "" ) );
//			//				}

//			//				vertexDefines.AddRange( generalDefines );
//			//				fragmentDefines.AddRange( generalDefines );

//			//				if( shaderGenerationEnable )
//			//				{
//			//					//vertex
//			//					var vertexCode = result.vertexGeneratedCode;
//			//					if( vertexCode != null )
//			//					{
//			//						if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_PARAMETERS", vertexCode.parametersBody ) );
//			//						if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_SAMPLERS", vertexCode.samplersBody ) );
//			//						if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_BODY", vertexCode.codeBody ) );
//			//					}

//			//					//fragment
//			//					var fragmentCode = result.fragmentGeneratedCode;
//			//					if( fragmentCode != null )
//			//					{
//			//						if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody ) );
//			//						if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody ) );
//			//						if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_BODY", fragmentCode.codeBody ) );
//			//					}
//			//				}
//			//			}

//			//			{
//			//				string error2;

//			//				//vertex program
//			//				GpuProgram vertexProgram = GpuProgramManager.Instance.GetProgram( "Standard_Vertex_",
//			//					GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_vs.sc", vertexDefines, out error2 );
//			//				if( !string.IsNullOrEmpty( error2 ) )
//			//				{
//			//					result.error = GetGpuProgramCompilationErrorText( error2, vertexDefines, result.vertexGeneratedCode );

//			//					EditorAPI.ShowDockWindow<LogWindow>();
//			//					Log.Info( result.error );

//			//					result.Dispose();
//			//					return;
//			//				}

//			//				//fragment program
//			//				GpuProgram fragmentProgram = GpuProgramManager.Instance.GetProgram( "Standard_Fragment_",
//			//					GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_fs.sc", fragmentDefines, out error2 );
//			//				if( !string.IsNullOrEmpty( error2 ) )
//			//				{
//			//					result.error = GetGpuProgramCompilationErrorText( error2, fragmentDefines, result.fragmentGeneratedCode );

//			//					EditorAPI.ShowDockWindow<LogWindow>();
//			//					Log.Info( result.error );

//			//					result.Dispose();
//			//					return;
//			//				}

//			//				var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
//			//				result.AllPasses.Add( pass );
//			//				if( nShadowsSupportCounter != 0 )
//			//					group.passesWithShadows.Add( pass );
//			//				else
//			//					group.passesWithoutShadows.Add( pass );

//			//				if( BlendMode.Value == BlendModeEnum.Opaque || BlendMode.Value == BlendModeEnum.Masked )
//			//				{
//			//					if( lightType == Component_Light.TypeEnum.Ambient )
//			//					{
//			//						pass.DepthWrite = true;
//			//						pass.SourceBlendFactor = SceneBlendFactor.One;
//			//						pass.DestinationBlendFactor = SceneBlendFactor.Zero;
//			//					}
//			//					else
//			//					{
//			//						pass.DepthWrite = false;
//			//						pass.SourceBlendFactor = SceneBlendFactor.One;
//			//						pass.DestinationBlendFactor = SceneBlendFactor.One;
//			//					}
//			//				}
//			//				else if( BlendMode.Value == BlendModeEnum.Translucent )
//			//				{
//			//					if( lightType == Component_Light.TypeEnum.Ambient )
//			//					{
//			//						pass.DepthWrite = false;
//			//						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
//			//						pass.DestinationBlendFactor = SceneBlendFactor.OneMinusSourceAlpha;
//			//					}
//			//					else
//			//					{
//			//						pass.DepthWrite = false;
//			//						pass.SourceBlendFactor = SceneBlendFactor.SourceAlpha;
//			//						pass.DestinationBlendFactor = SceneBlendFactor.One;
//			//					}
//			//				}
//			//				else
//			//				{
//			//					//!!!!
//			//				}

//			//				if( TwoSided )
//			//					pass.CullingMode = CullingMode.None;

//			//				//Component_Texture baseTextureV = BaseTexture;
//			//				////!!!!ниже проверять незагурежнность? есть инвалидные текстуры для показа ошибки, а есть белые или еще какие-то, которые для замены
//			//				//if( baseTextureV == null )
//			//				//	baseTextureV = ResourceUtils.GetWhiteTexture2D();

//			//				//GpuMaterialPass.TextureParameterValue textureValue = new GpuMaterialPass.TextureParameterValue( baseTextureV,
//			//				//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
//			//				//pass.ConstantParameterValues.Set( "baseTexture", textureValue, ParameterType.Texture2D );
//			//			}
//			//		}
//			//	}


//			//	//special shadow caster material
//			//	if( needSpecialShadowCaster )
//			//	{
//			//		result.specialShadowCasterData = new Component_RenderingPipeline.ShadowCasterData();

//			//		//!!!! 4
//			//		result.specialShadowCasterData.passByLightType = new GpuMaterialPass[ 4 ];

//			//		foreach( Component_Light.TypeEnum lightType in Enum.GetValues( typeof( Component_Light.TypeEnum ) ) )
//			//		{
//			//			if( lightType == Component_Light.TypeEnum.Ambient )
//			//				continue;

//			//			//generate compile arguments
//			//			var vertexDefines = new List<Tuple<string, string>>();
//			//			var fragmentDefines = new List<Tuple<string, string>>();
//			//			{
//			//				var generalDefines = new List<Tuple<string, string>>();
//			//				generalDefines.Add( Tuple.Create( "LIGHTTYPE_" + lightType.ToString().ToUpper(), "" ) );
//			//				generalDefines.Add( Tuple.Create( "BLENDMODE_" + BlendMode.Value.ToString().ToUpper(), "" ) );

//			//				vertexDefines.AddRange( generalDefines );
//			//				fragmentDefines.AddRange( generalDefines );

//			//				if( shaderGenerationEnable )
//			//				{
//			//					//vertex
//			//					var vertexCode = result.vertexGeneratedCodeShadowCaster;
//			//					if( vertexCode != null )
//			//					{
//			//						if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_PARAMETERS", vertexCode.parametersBody ) );
//			//						if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_SAMPLERS", vertexCode.samplersBody ) );
//			//						if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
//			//							vertexDefines.Add( Tuple.Create( "VERTEX_CODE_BODY", vertexCode.codeBody ) );
//			//					}

//			//					//fragment
//			//					var fragmentCode = result.fragmentGeneratedCodeShadowCaster;
//			//					if( fragmentCode != null )
//			//					{
//			//						if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody ) );
//			//						if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody ) );
//			//						if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
//			//							fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_BODY", fragmentCode.codeBody ) );
//			//					}
//			//				}
//			//			}

//			//			{
//			//				string error2;

//			//				//vertex program
//			//				GpuProgram vertexProgram = GpuProgramManager.Instance.GetProgram( "Standard_ShadowCaster_Vertex_",
//			//					GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_ShadowCaster_vs.sc", vertexDefines, out error2 );
//			//				if( !string.IsNullOrEmpty( error2 ) )
//			//				{
//			//					result.error = GetGpuProgramCompilationErrorText( error2, vertexDefines, result.vertexGeneratedCodeShadowCaster );

//			//					EditorAPI.ShowDockWindow<LogWindow>();
//			//					Log.Info( result.error );

//			//					result.Dispose();
//			//					return;
//			//				}

//			//				//fragment program
//			//				GpuProgram fragmentProgram = GpuProgramManager.Instance.GetProgram( "Standard_ShadowCaster_Fragment_",
//			//					GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_ShadowCaster_fs.sc", fragmentDefines, out error2 );
//			//				if( !string.IsNullOrEmpty( error2 ) )
//			//				{
//			//					result.error = GetGpuProgramCompilationErrorText( error2, fragmentDefines, result.fragmentGeneratedCodeShadowCaster );

//			//					EditorAPI.ShowDockWindow<LogWindow>();
//			//					Log.Info( result.error );

//			//					result.Dispose();
//			//					return;
//			//				}

//			//				var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
//			//				result.specialShadowCasterData.passByLightType[ (int)lightType ] = pass;

//			//				if( TwoSided )
//			//					pass.CullingMode = CullingMode.None;
//			//			}
//			//		}
//			//	}

//			//	//deferred shading pass
//			//	if( result.deferredShadingSupport )
//			//	{
//			//		//generate compile arguments
//			//		var vertexDefines = new List<Tuple<string, string>>();
//			//		var fragmentDefines = new List<Tuple<string, string>>();
//			//		{
//			//			var generalDefines = new List<Tuple<string, string>>();
//			//			generalDefines.Add( Tuple.Create( "BLENDMODE_" + BlendMode.Value.ToString().ToUpper(), "" ) );
//			//			generalDefines.Add( Tuple.Create( "SHADINGMODE_" + ShadingMode.Value.ToString().ToUpper(), "" ) );
//			//			if( TwoSided )
//			//				generalDefines.Add( Tuple.Create(XX "TWO_SIDED", "" ) );

//			//			//!!!!new
//			//			if( ClearCoat )
//			//				generalDefines.Add( Tuple.Create( "CLEAR_COAT", "" ) );
//			//			if( SubsurfaceScattering )
//			//				generalDefines.Add( Tuple.Create( "SUBSURFACE_SCATTERING", "" ) );

//			//			////receive shadows support
//			//			//if( nShadowsSupportCounter != 0 )
//			//			//{
//			//			//	generalDefines.Add( Tuple.Create( "SHADOW_MAP", "" ) );

//			//			//	//!!!!
//			//			//	generalDefines.Add( Tuple.Create( "SHADOW_MAP_LOW", "" ) );
//			//			//}

//			//			vertexDefines.AddRange( generalDefines );
//			//			fragmentDefines.AddRange( generalDefines );

//			//			if( shaderGenerationEnable )
//			//			{
//			//				//!!!!может тут попроще для деферреда

//			//				//vertex
//			//				var vertexCode = result.vertexGeneratedCode;
//			//				if( vertexCode != null )
//			//				{
//			//					if( !string.IsNullOrEmpty( vertexCode.parametersBody ) )
//			//						vertexDefines.Add( Tuple.Create( "VERTEX_CODE_PARAMETERS", vertexCode.parametersBody ) );
//			//					if( !string.IsNullOrEmpty( vertexCode.samplersBody ) )
//			//						vertexDefines.Add( Tuple.Create( "VERTEX_CODE_SAMPLERS", vertexCode.samplersBody ) );
//			//					if( !string.IsNullOrEmpty( vertexCode.codeBody ) )
//			//						vertexDefines.Add( Tuple.Create( "VERTEX_CODE_BODY", vertexCode.codeBody ) );
//			//				}

//			//				//fragment
//			//				var fragmentCode = result.fragmentGeneratedCode;
//			//				if( fragmentCode != null )
//			//				{
//			//					if( !string.IsNullOrEmpty( fragmentCode.parametersBody ) )
//			//						fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_PARAMETERS", fragmentCode.parametersBody ) );
//			//					if( !string.IsNullOrEmpty( fragmentCode.samplersBody ) )
//			//						fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_SAMPLERS", fragmentCode.samplersBody ) );
//			//					if( !string.IsNullOrEmpty( fragmentCode.codeBody ) )
//			//						fragmentDefines.Add( Tuple.Create( "FRAGMENT_CODE_BODY", fragmentCode.codeBody ) );
//			//				}
//			//			}
//			//		}

//			//		{
//			//			string error2;

//			//			//vertex program
//			//			GpuProgram vertexProgram = GpuProgramManager.Instance.GetProgram( "Standard_Deferred_Vertex_",
//			//				GpuProgramType.Vertex, @"Base\Shaders\MaterialStandard_Deferred_vs.sc", vertexDefines, out error2 );
//			//			if( !string.IsNullOrEmpty( error2 ) )
//			//			{
//			//				result.error = GetGpuProgramCompilationErrorText( error2, vertexDefines, result.vertexGeneratedCode );

//			//				EditorAPI.ShowDockWindow<LogWindow>();
//			//				Log.Info( result.error );

//			//				result.Dispose();
//			//				return;
//			//			}

//			//			//fragment program
//			//			GpuProgram fragmentProgram = GpuProgramManager.Instance.GetProgram( "Standard_Deferred_Fragment_",
//			//				GpuProgramType.Fragment, @"Base\Shaders\MaterialStandard_Deferred_fs.sc", fragmentDefines, out error2 );
//			//			if( !string.IsNullOrEmpty( error2 ) )
//			//			{
//			//				result.error = GetGpuProgramCompilationErrorText( error2, fragmentDefines, result.fragmentGeneratedCode );

//			//				EditorAPI.ShowDockWindow<LogWindow>();
//			//				Log.Info( result.error );

//			//				result.Dispose();
//			//				return;
//			//			}

//			//			var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
//			//			result.AllPasses.Add( pass );
//			//			result.deferredShadingPass = pass;
//			//			//if( nShadowsSupportCounter != 0 )
//			//			//	group.passesWithShadows.Add( pass );
//			//			//else
//			//			//	group.passesWithoutShadows.Add( pass );

//			//			if( TwoSided )
//			//				pass.CullingMode = CullingMode.None;

//			//			//Component_Texture baseTextureV = BaseTexture;
//			//			////!!!!ниже проверять незагурежнность? есть инвалидные текстуры для показа ошибки, а есть белые или еще какие-то, которые для замены
//			//			//if( baseTextureV == null )
//			//			//	baseTextureV = ResourceUtils.GetWhiteTexture2D();

//			//			//GpuMaterialPass.TextureParameterValue textureValue = new GpuMaterialPass.TextureParameterValue( baseTextureV,
//			//			//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
//			//			//pass.ConstantParameterValues.Set( "baseTexture", textureValue, ParameterType.Texture2D );
//			//		}

//			//	}

//			//} );

//			Result = result;
//		}
//	}
//}
