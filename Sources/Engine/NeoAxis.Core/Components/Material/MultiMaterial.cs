// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A combined material that includes several usual materials.
	/// </summary>
	public class MultiMaterial : Material
	{
		/// <summary>
		/// The list of materials.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[FlowGraphBrowsable( false )]
		public ReferenceList<Material> Materials
		{
			get { return _materials; }
		}
		public delegate void MaterialsChangedDelegate( MultiMaterial sender );
		public event MaterialsChangedDelegate MaterialsChanged;
		ReferenceList<Material> _materials;

		/// <summary>
		/// The parameter to determine behavior of material index selection during rasterization.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> MaterialIndex
		{
			get { if( _materialIndex.BeginGet() ) MaterialIndex = _materialIndex.Get( this ); return _materialIndex.value; }
			set { if( _materialIndex.BeginSet( ref value ) ) { try { MaterialIndexChanged?.Invoke( this ); } finally { _materialIndex.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialIndex"/> property value changes.</summary>
		public event Action<MultiMaterial> MaterialIndexChanged;
		ReferenceField<int> _materialIndex = 0;

		/////////////////////////////////////////

		public MultiMaterial()
		{
			_materials = new ReferenceList<Material>( this, () => MaterialsChanged?.Invoke( this ) );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				var type = member.Owner as Metadata.NetTypeInfo;
				if( type != null && type.GetNetType() == typeof( Material ) )
					skip = true;
			}
		}

		class CombineGroup
		{
			public int StartIndex;
			public List<Material> Materials = new List<Material>();
			public List<CompiledMaterialData> MaterialsSeparatePasses = new List<CompiledMaterialData>();
			public int TextureCount;
		}

		static int GetTextureCount( CompiledMaterialData materialData )
		{
			var count1 = 0;
			if( materialData.vertexGeneratedCode != null )
				count1 += materialData.vertexGeneratedCode.GetTextureCount();
			if( materialData.materialIndexGeneratedCode != null )
				count1 += materialData.materialIndexGeneratedCode.GetTextureCount();
			if( materialData.displacementGeneratedCode != null )
				count1 += materialData.displacementGeneratedCode.GetTextureCount();
			if( materialData.fragmentGeneratedCode != null )
				count1 += materialData.fragmentGeneratedCode.GetTextureCount();

			//shadow caster
			var count2 = 0;
			if( materialData.shadowCasterVertexGeneratedCode != null )
				count2 += materialData.shadowCasterVertexGeneratedCode.GetTextureCount();
			if( materialData.shadowCasterMaterialIndexGeneratedCode != null )
				count2 += materialData.shadowCasterMaterialIndexGeneratedCode.GetTextureCount();
			if( materialData.shadowCasterFragmentGeneratedCode != null )
				count2 += materialData.shadowCasterFragmentGeneratedCode.GetTextureCount();

			return Math.Max( count1, count2 );
		}

		public override CompiledMaterialData Compile( CompiledMaterialData.SpecialMode specialMode, CompileExtensionData extensionData, int multiMaterialStartIndexOfCombinedGroup, CompiledMaterialData[] multiMaterialSeparateMaterialsOfCombinedGroup, Material[] multiMaterialSourceMaterialsToGetProperties, int multiSubMaterialSeparatePassIndex )
		{
			//get material list
			var materials = new Material[ Math.Max( Materials.Count, 1 ) ];
			if( Materials.Count != 0 )
			{
				for( int n = 0; n < materials.Length; n++ )
				{
					materials[ n ] = Materials[ n ].Value;
					if( materials[ n ] == null )
						materials[ n ] = ResourceUtility.MaterialNull;
				}
			}
			else
				materials[ 0 ] = ResourceUtility.MaterialNull;


			var result = new CompiledMaterialData();
			result.owner = this;
			result.specialMode = specialMode;
			result.extensionData = extensionData;
			result.multiMaterial = true;

			result.Transparent = Array.Exists( materials, m => m.BlendMode.Value == BlendModeEnum.Transparent || m.BlendMode.Value == BlendModeEnum.Add );


			var resultSeparatePasses = new List<CompiledMaterialData>();
			var textureCounts = new List<int>( materials.Length );

			//compile separate passes
			foreach( var material in materials )
			{
				var extensionData2 = new CompileExtensionData();
				extensionData2.materialIndexShaderProperties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( MaterialIndex ) )) );
				extensionData2.shadowCasterMaterialIndexShaderProperties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( MaterialIndex ) )) );

				var separatePassIndex = resultSeparatePasses.Count;

				var compiled = material.Compile( CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass, extensionData2, 0, null, null, separatePassIndex );
				//error
				if( compiled == null )
					compiled = ResourceUtility.MaterialInvalid.Result;

				resultSeparatePasses.Add( compiled );
				textureCounts.Add( GetTextureCount( compiled ) );
			}

			//calculate combine groups
			var groups = new List<CombineGroup>();
			{
				for( int nMaterial = 0; nMaterial < materials.Length; nMaterial++ )
				{
					var material = materials[ nMaterial ];

					var lastGroup = groups.Count != 0 ? groups[ groups.Count - 1 ] : null;
					var lastGroupMaterial0 = lastGroup?.Materials[ 0 ];

					bool canCombineWithLastGroup = false;
					if( lastGroup != null && string.IsNullOrEmpty( lastGroupMaterial0.PerformCheckDeferredShadingSupport() ) && !lastGroupMaterial0.AdvancedBlending )
					{
						if( string.IsNullOrEmpty( material.PerformCheckDeferredShadingSupport() ) && !material.AdvancedBlending )
						{
							var twoSided1 = lastGroupMaterial0.TwoSided && !lastGroupMaterial0.TwoSidedFlipNormals;
							var twoSided2 = material.TwoSided && !material.TwoSidedFlipNormals;
							if( twoSided1 == twoSided2 )
							{
								var transparent1 = material.BlendMode == BlendModeEnum.Transparent || material.BlendMode == BlendModeEnum.Add;
								if( !transparent1 )
								{
									if( lastGroup.Materials.Count < 32 && lastGroup.TextureCount + textureCounts[ nMaterial ] < 114 )
										canCombineWithLastGroup = true;
								}
							}
						}
					}

					if( canCombineWithLastGroup )
					{
						lastGroup.Materials.Add( material );
						lastGroup.MaterialsSeparatePasses.Add( resultSeparatePasses[ nMaterial ] );
						lastGroup.TextureCount += textureCounts[ nMaterial ];
					}
					else
					{
						var newGroup = new CombineGroup();
						newGroup.StartIndex = nMaterial;
						newGroup.Materials.Add( material );
						newGroup.MaterialsSeparatePasses.Add( resultSeparatePasses[ nMaterial ] );
						newGroup.TextureCount += textureCounts[ nMaterial ];
						groups.Add( newGroup );
					}
				}
			}


			var resultCombinedPasses = new List<CompiledMaterialData>();

			foreach( var group in groups )
			{
				if( group.Materials.Count > 1 )
				{
					//compile combined pass

					var combinedMaterial = new Material();

					combinedMaterial.BlendMode = group.Materials.Exists( m => m.BlendMode.Value == BlendModeEnum.Masked ) ? BlendModeEnum.Masked : BlendModeEnum.Opaque;
					combinedMaterial.TwoSided = group.Materials.Exists( m => m.TwoSided );

					if( group.Materials.Exists( m => !m.TwoSidedFlipNormals ) )
						combinedMaterial.TwoSidedFlipNormals = false;

					//supported shading models by deferred: Lit, Subsurface (if no Emissive), Simple.
					combinedMaterial.ShadingModel = ShadingModelEnum.Lit;
					if( group.Materials.Exists( m => m.ShadingModel.Value == ShadingModelEnum.Subsurface ) )
						combinedMaterial.ShadingModel = ShadingModelEnum.Subsurface;
					if( group.Materials.All( m => m.ShadingModel.Value == ShadingModelEnum.Simple ) )
						combinedMaterial.ShadingModel = ShadingModelEnum.Simple;

					combinedMaterial.OpacityDithering = group.Materials.Exists( m => m.OpacityDithering );
					if( group.Materials.Exists( m => m.ClearCoat.Value != 0 ) )
						combinedMaterial.ClearCoat = 1;
					if( group.Materials.Exists( m => m.Anisotropy.Value != 0 ) )
						combinedMaterial.Anisotropy = 1;
					if( group.Materials.Exists( m => m.RayTracingReflection.Value != 0 ) )
						combinedMaterial.RayTracingReflection = 1;
					combinedMaterial.ReceiveDecals = group.Materials.Exists( m => m.ReceiveDecals );
					combinedMaterial.UseVertexColor = group.Materials.Exists( m => m.UseVertexColor );



					//no Displacement support
					//combinedMaterial.Displacement = ;


					//AdvancedBlending can't combined. maybe combine when all parameters are equal. or use first material settings


					//CustomParameters


					var extensionData2 = new CompileExtensionData();
					extensionData2.materialIndexShaderProperties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( MaterialIndex ) )) );
					extensionData2.shadowCasterMaterialIndexShaderProperties.Add( (this, 0, (Metadata.Property)MetadataGetMemberBySignature( "property:" + nameof( MaterialIndex ) )) );

					//var referencedSeparateMaterials = new CompiledMaterialData[ group.Materials.Count ];
					//for( int n = 0; n < group.Materials.Count; n++ )
					//	referencedSeparateMaterials[ n ] = group.MaterialsSeparatePasses[ n ];

					var compiled = combinedMaterial.Compile( CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass, extensionData2, group.StartIndex, group.MaterialsSeparatePasses.ToArray()/*referencedSeparateMaterials*/, group.Materials.ToArray(), 0 );

					//error
					if( compiled == null )
						compiled = ResourceUtility.MaterialInvalid.Result;

					resultCombinedPasses.Add( compiled );
				}
				else
				{
					//use separate material when one material in group
					resultCombinedPasses.Add( group.MaterialsSeparatePasses[ 0 ] );
				}
			}

			result.multiSeparatePasses = resultSeparatePasses.ToArray();
			result.multiCombinedPasses = resultCombinedPasses.ToArray();

			return result;
		}

		protected override string OnCheckDeferredShadingSupport( CompiledMaterialData compiledData )
		{
			return base.OnCheckDeferredShadingSupport( compiledData );
		}
	}
}
