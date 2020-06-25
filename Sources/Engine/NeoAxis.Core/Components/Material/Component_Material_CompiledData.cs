// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using SharpBgfx;

namespace NeoAxis
{
	partial class Component_Material
	{
		/// <summary>
		/// Represents precalculated data of <see cref="Component_Material"/>.
		/// </summary>
		public class CompiledMaterialData : IDisposable
		{
			bool disposed;

			public List<GpuMaterialPass> AllPasses = new List<GpuMaterialPass>();
			//bool passesDisposeOnDisposeResultData = true;
			public bool Transparent;
			public ShadingModelEnum ShadingModel;

			public int _currentRenderingFrameIndex = -1;

			//

			//!!!!?
			public virtual void Dispose()
			{
				//if( PassesDisposeOnDisposeResultData )
				//{
				//	foreach( var p in AllPasses )
				//		p.Dispose();
				//}
				AllPasses.Clear();

				disposed = true;
			}

			public bool Disposed
			{
				get { return disposed; }
			}

			//public bool PassesDisposeOnDisposeResultData
			//{
			//	get { return passesDisposeOnDisposeResultData; }
			//	set { passesDisposeOnDisposeResultData = value; }
			//}

			//public virtual Component_RenderingPipeline.ShadowCasterData GetSpecialShadowCaster( Component_RenderingPipeline pipeline )
			//{
			//	return null;
			//}

			//!!!!public

			//need to get non referenced values (dynamic)
			public Component_Material owner;
			public UsageMode usageMode;
			public CompileExtensionData extensionData;

			public ShaderGenerator.ResultData vertexGeneratedCode;
			public ShaderGenerator.ResultData displacementGeneratedCode;
			public ShaderGenerator.ResultData fragmentGeneratedCode;
			public ShaderGenerator.ResultData vertexGeneratedCodeShadowCaster;
			public ShaderGenerator.ResultData fragmentGeneratedCodeShadowCaster;

			/// <summary>
			/// Group of passes for <see cref="CompiledMaterialData"/>.
			/// </summary>
			public class PassGroup
			{
				public GpuMaterialPass passWithoutShadows;
				public GpuMaterialPass passWithShadowsLow;
				public GpuMaterialPass passWithShadowsHigh;
				//public List<GpuMaterialPass> passesWithoutShadows = new List<GpuMaterialPass>();
				//public List<GpuMaterialPass> passesWithShadowsLow = new List<GpuMaterialPass>();
				//public List<GpuMaterialPass> passesWithShadowsHigh = new List<GpuMaterialPass>();
			}
			public PassGroup[] passesByLightType;

			public Component_RenderingPipeline.ShadowCasterData specialShadowCasterData;

			public bool deferredShadingSupport;
			public string deferredShadingSupportReason;
			public GpuMaterialPass deferredShadingPass;

			public bool receiveDecalsSupport;
			public string receiveDecalsSupportReason;

			public bool decalSupport;
			public string decalSupportReason;
			public GpuMaterialPass decalShadingPass;

			public string error;

			//uniforms
			DynamicParametersUniformToUpdate dynamicParametersUniformToUpdate;
			//object dynamicParametersUniformToUpdateUpdatedForFrame;
			//public static Uniform dynamicParametersFragmentUniform;
			public DynamicParametersFragmentUniform dynamicParametersFragmentUniformData;

			//data which actual only during frame rendering
			public int currentFrameIndex = -1;
			/// <summary>
			/// Internal material data for rendering frame.
			/// </summary>
			public class CurrentFrameData
			{
				public OpenList<ValueTuple<Uniform, Vector4F>> Uniforms;
				public ParameterContainer FallbackUniformContainer;
				public OpenList<ViewportRenderingContext.BindTextureData> Textures;

				public void Clear()
				{
					Uniforms?.Clear();
					FallbackUniformContainer = null;
					Textures?.Clear();
				}
			}
			public CurrentFrameData currentFrameData;
			public CurrentFrameData currentFrameDataSpecialShadowCaster;

			/////////////////////////////////////

			//!!!!
			public enum UsageMode
			{
				Usual,
				//MaterialBlendFirst,
				//MaterialBlendNotFirst,
			}

			/////////////////////////////////////

			public enum DynamicParametersUniformToUpdate
			{
				BaseColor = 1 << 0,
				Opacity = 1 << 1,
				OpacityMaskThreshold = 1 << 2,
				Metallic = 1 << 3,
				Roughness = 1 << 4,
				Reflectance = 1 << 5,
				ClearCoat = 1 << 6,
				ClearCoatRoughness = 1 << 7,
				Anisotropy = 1 << 8,
				AnisotropyDirection = 1 << 9,
				Thickness = 1 << 10,
				SubsurfacePower = 1 << 11,
				SubsurfaceColor = 1 << 12,
				SheenColor = 1 << 13,
				RayTracingReflection = 1 << 14,
				Emissive = 1 << 15,
				AnisotropyDirectionBasis = 1 << 16,
				Shininess = 1 << 17,
				DisplacementScale = 1 << 18,
			}

			/////////////////////////////////////

			/// <summary>
			/// Represents a material data for fragment shader.
			/// </summary>
			[StructLayout( LayoutKind.Sequential )]
			public struct DynamicParametersFragmentUniform
			{
				//!!!!какие-то можно склеить чтобы структура меньше была. разные shading model

				//0
				public Vector3F baseColor;
				public float opacity;

				//1
				public Vector3F anisotropyDirection;
				public float opacityMaskThreshold;

				//2
				public Vector3F subsurfaceColor;
				public float metallic;

				//3
				public Vector3F sheenColor;
				public float roughness;

				//4
				public Vector3F emissive;
				public float reflectance;

				//5
				public float clearCoat;
				public float clearCoatRoughness;
				public float anisotropy;
				public float thickness;

				//6
				public float subsurfacePower;
				public float rayTracingReflection;
				public float anisotropyDirectionBasis;
				public float shininess;

				//7
				public float displacementScale;
				public float receiveDecals;
				public float useVertexColor;
				public float unused3;
			}

			/////////////////////////////////////

			//public override Component_RenderingPipeline.ShadowCasterData GetSpecialShadowCaster( Component_RenderingPipeline pipeline )
			//{
			//	return specialShadowCasterData;
			//}

			//public override void Dispose()
			//{
			//	//specialShadowCasterData?.Dispose();
			//	specialShadowCasterData = null;

			//	base.Dispose();
			//}

			public void InitDynamicParametersUniformToUpdate()
			{
				if( !owner.BaseColor.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.BaseColor;

				if( owner.BlendMode.Value != BlendModeEnum.Opaque && !owner.Opacity.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Opacity;
				if( ( owner.BlendMode.Value == BlendModeEnum.Masked /*|| owner.BlendMode.Value == BlendModeEnum.MaskedLayer*/ ) && !owner.OpacityMaskThreshold.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.OpacityMaskThreshold;
				if( !owner.DisplacementScale.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.DisplacementScale;

				if( owner.ShadingModel.Value != ShadingModelEnum.Unlit )
				{
					if( !owner.Metallic.ReferenceSpecified )
						dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Metallic;
					if( !owner.Roughness.ReferenceSpecified )
						dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Roughness;
					if( !owner.Reflectance.ReferenceSpecified )
						dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Reflectance;

					if( owner.ShadingModel.Value == ShadingModelEnum.Lit )
					{
						if( !owner.ClearCoat.ReferenceSpecified && owner.ClearCoat.Value != 0 )
						{
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.ClearCoat;
							if( !owner.ClearCoatRoughness.ReferenceSpecified )
								dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.ClearCoatRoughness;
						}

						if( !owner.Anisotropy.ReferenceSpecified && owner.Anisotropy.Value != 0 )
						{
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Anisotropy;
							if( !owner.AnisotropyDirection.ReferenceSpecified )
								dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.AnisotropyDirection;
							if( !owner.AnisotropyDirectionBasis.ReferenceSpecified )
								dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.AnisotropyDirectionBasis;
						}
					}
					else if( owner.ShadingModel.Value == ShadingModelEnum.Subsurface )
					{
						if( !owner.Thickness.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Thickness;
						if( !owner.SubsurfacePower.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.SubsurfacePower;
						if( !owner.SubsurfaceColor.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.SubsurfaceColor;
					}
					else if( owner.ShadingModel.Value == ShadingModelEnum.Cloth )
					{
						if( !owner.SheenColor.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.SheenColor;
						if( !owner.SubsurfaceColor.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.SubsurfaceColor;
					}
					else if( owner.ShadingModel.Value == ShadingModelEnum.Specular )
					{
						if( !owner.Shininess.ReferenceSpecified )
							dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Shininess;
					}
				}

				if( !owner.RayTracingReflection.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.RayTracingReflection;
				if( !owner.Emissive.ReferenceSpecified )
					dynamicParametersUniformToUpdate |= DynamicParametersUniformToUpdate.Emissive;
			}

			public void UpdateDynamicParametersFragmentUniformData( ViewportRenderingContext context )
			{
				////update uniform data (per frame)
				//if( dynamicParametersUniformToUpdateUpdatedForFrame != context.uniquePerFrameObjectToDetectNewFrame )
				//{
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.BaseColor ) != 0 )
					dynamicParametersFragmentUniformData.baseColor = owner.BaseColor.Value.ToVector3().ToVector3F();
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Opacity ) != 0 )
					dynamicParametersFragmentUniformData.opacity = (float)owner.Opacity;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.OpacityMaskThreshold ) != 0 )
					dynamicParametersFragmentUniformData.opacityMaskThreshold = (float)owner.OpacityMaskThreshold;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Metallic ) != 0 )
					dynamicParametersFragmentUniformData.metallic = (float)owner.Metallic;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Roughness ) != 0 )
					dynamicParametersFragmentUniformData.roughness = (float)owner.Roughness;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Reflectance ) != 0 )
					dynamicParametersFragmentUniformData.reflectance = (float)owner.Reflectance;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.ClearCoat ) != 0 )
					dynamicParametersFragmentUniformData.clearCoat = (float)owner.ClearCoat;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.ClearCoatRoughness ) != 0 )
					dynamicParametersFragmentUniformData.clearCoatRoughness = (float)owner.ClearCoatRoughness;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Anisotropy ) != 0 )
					dynamicParametersFragmentUniformData.anisotropy = (float)owner.Anisotropy;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.AnisotropyDirection ) != 0 )
					dynamicParametersFragmentUniformData.anisotropyDirection = owner.AnisotropyDirection.Value.ToVector3F();
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.AnisotropyDirectionBasis ) != 0 )
				{
					dynamicParametersFragmentUniformData.anisotropyDirectionBasis = owner.AnisotropyDirectionBasis.Value == AnisotropyDirectionBasisEnum.Bitangent ? 1 : -1;
				}
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Thickness ) != 0 )
					dynamicParametersFragmentUniformData.thickness = (float)owner.Thickness;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SubsurfacePower ) != 0 )
					dynamicParametersFragmentUniformData.subsurfacePower = (float)owner.SubsurfacePower;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SheenColor ) != 0 )
					dynamicParametersFragmentUniformData.sheenColor = owner.SheenColor.Value.ToVector3().ToVector3F();
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SubsurfaceColor ) != 0 )
					dynamicParametersFragmentUniformData.subsurfaceColor = owner.SubsurfaceColor.Value.ToVector3().ToVector3F();

				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Shininess ) != 0 )
					dynamicParametersFragmentUniformData.shininess = (float)owner.Shininess.Value;

				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.RayTracingReflection ) != 0 )
					dynamicParametersFragmentUniformData.rayTracingReflection = (float)owner.RayTracingReflection;
				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Emissive ) != 0 )
					dynamicParametersFragmentUniformData.emissive = owner.Emissive.Value.ToVector3().ToVector3F();

				if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.DisplacementScale ) != 0 )
					dynamicParametersFragmentUniformData.displacementScale = (float)owner.DisplacementScale.Value;

				dynamicParametersFragmentUniformData.receiveDecals = owner.ReceiveDecals ? 1 : 0;
				dynamicParametersFragmentUniformData.useVertexColor = owner.UseVertexColor ? 1 : 0;

				//dynamicParametersUniformToUpdateUpdatedForFrame = context.uniquePerFrameObjectToDetectNewFrame;
				//}
			}

			//public unsafe void Bind( ViewportRenderingContext context, bool bindSpecialShadowCaster )
			//{
			//	//update uniform data (per frame)
			//	if( dynamicParametersUniformToUpdateUpdatedForFrame != context.uniquePerFrameObjectToDetectNewFrame )
			//	{
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.BaseColor ) != 0 )
			//			dynamicParametersFragmentUniformData.baseColor = owner.BaseColor.Value.ToVector3().ToVector3F();
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Opacity ) != 0 )
			//			dynamicParametersFragmentUniformData.opacity = (float)owner.Opacity;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.OpacityMaskThreshold ) != 0 )
			//			dynamicParametersFragmentUniformData.opacityMaskThreshold = (float)owner.OpacityMaskThreshold;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Metallic ) != 0 )
			//			dynamicParametersFragmentUniformData.metallic = (float)owner.Metallic;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Roughness ) != 0 )
			//			dynamicParametersFragmentUniformData.roughness = (float)owner.Roughness;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Reflectance ) != 0 )
			//			dynamicParametersFragmentUniformData.reflectance = (float)owner.Reflectance;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.ClearCoat ) != 0 )
			//			dynamicParametersFragmentUniformData.clearCoat = (float)owner.ClearCoat;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.ClearCoatRoughness ) != 0 )
			//			dynamicParametersFragmentUniformData.clearCoatRoughness = (float)owner.ClearCoatRoughness;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Anisotropy ) != 0 )
			//			dynamicParametersFragmentUniformData.anisotropy = (float)owner.Anisotropy;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.AnisotropyDirection ) != 0 )
			//			dynamicParametersFragmentUniformData.anisotropyDirection = owner.AnisotropyDirection.Value.ToVector3F();
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.AnisotropyDirectionBasis ) != 0 )
			//		{
			//			dynamicParametersFragmentUniformData.anisotropyDirectionBasis = owner.AnisotropyDirectionBasis.Value == AnisotropyDirectionBasisEnum.Bitangent ? 1 : -1;
			//		}
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Thickness ) != 0 )
			//			dynamicParametersFragmentUniformData.thickness = (float)owner.Thickness;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SubsurfacePower ) != 0 )
			//			dynamicParametersFragmentUniformData.subsurfacePower = (float)owner.SubsurfacePower;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SheenColor ) != 0 )
			//			dynamicParametersFragmentUniformData.sheenColor = owner.SheenColor.Value.ToVector3().ToVector3F();
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.SubsurfaceColor ) != 0 )
			//			dynamicParametersFragmentUniformData.subsurfaceColor = owner.SubsurfaceColor.Value.ToVector3().ToVector3F();

			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Shininess ) != 0 )
			//			dynamicParametersFragmentUniformData.shininess = (float)owner.Shininess.Value;

			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.RayTracingReflection ) != 0 )
			//			dynamicParametersFragmentUniformData.rayTracingReflection = (float)owner.RayTracingReflection;
			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.Emissive ) != 0 )
			//			dynamicParametersFragmentUniformData.emissive = owner.Emissive.Value.ToVector3().ToVector3F();

			//		if( ( dynamicParametersUniformToUpdate & DynamicParametersUniformToUpdate.DisplacementScale ) != 0 )
			//			dynamicParametersFragmentUniformData.displacementScale = (float)owner.DisplacementScale.Value;

			//		dynamicParametersUniformToUpdateUpdatedForFrame = context.uniquePerFrameObjectToDetectNewFrame;
			//	}

			//	//set dynamic parameters fragment uniform
			//	{
			//		int vec4Count = sizeof( DynamicParametersFragmentUniform ) / sizeof( Vector4F );
			//		if( vec4Count != 8 )
			//			Log.Fatal( "Component_MaterialStandard.CompiledDataStandard: SetDynamicParametersUniform: vec4Count != 8." );
			//		if( dynamicParametersFragmentUniform == Uniform.Invalid )
			//			dynamicParametersFragmentUniform = GpuProgramManager.RegisterUniform( "u_materialStandardFragment", UniformType.Vector4, vec4Count );
			//		fixed ( DynamicParametersFragmentUniform* p = &dynamicParametersFragmentUniformData )
			//			Bgfx.SetUniform( dynamicParametersFragmentUniform, p, vec4Count );
			//	}

			//	//!!!!пока биндится выше
			//	////bind shader generated parameters
			//	//BindShaderGeneratedParameters( context, bindSpecialShadowCaster );
			//}

			/////////////////////////////////////

			enum TypeIndex
			{
				Vector4F,
				Vector4,
				Vector4I,
				Vector3F,
				Vector3,
				Vector3I,
				Vector2F,
				Vector2,
				Vector2I,
				Int,
				Float,
				Double,
				ColorValue,
				ColorValuePowered,
				Quaternion,
				QuaternionF,
				PlaneF,
				Plane,
				Radian,
				RadianF,
				Degree,
				DegreeF,
				Rectangle,
				RectangleF,
				RectangleI,
				Range,
				RangeF,
				RangeI,
				SphericalDirection,
				SphericalDirectionF,
				Angles,
				AnglesF,
			}
			static Dictionary<Type, TypeIndex> typeIndexes;

			//public unsafe void BindShaderGeneratedParameters( ViewportRenderingContext context, bool bindSpecialShadowCaster )
			//{
			//	//!!!!пока так, по имени через ParameterContainer

			//	//!!!!для shader caster что-то может не надо биндить

			//	//!!!!
			//	ParameterContainer container = null;
			//	//var container = new ParameterContainer();

			//	if( shaderGenerationEnable )
			//	{
			//		//!!!!склеивается два шейдера. пока так

			//		var codes = new List<ShaderGenerator.ResultData>();
			//		if( bindSpecialShadowCaster )
			//		{
			//			if( vertexGeneratedCodeShadowCaster != null )
			//				codes.Add( vertexGeneratedCodeShadowCaster );
			//			if( fragmentGeneratedCodeShadowCaster != null )
			//				codes.Add( fragmentGeneratedCodeShadowCaster );
			//		}
			//		else
			//		{
			//			if( vertexGeneratedCode != null )
			//				codes.Add( vertexGeneratedCode );
			//			if( displacementGeneratedCode != null )
			//				codes.Add( displacementGeneratedCode );
			//			if( fragmentGeneratedCode != null )
			//				codes.Add( fragmentGeneratedCode );
			//		}

			//		for( int nCode = 0; nCode < codes.Count; nCode++ )//foreach( var code in codes )
			//		{
			//			var code = codes[ nCode ];

			//			if( code.parameters != null )
			//			{
			//				for( int nItem = 0; nItem < code.parameters.Count; nItem++ )//foreach( var item in code.parameters )
			//				{
			//					var item = code.parameters[ nItem ];
			//					//!!!!надо ссылки сконвертить или как-то так? чтобы работало, если это в типе

			//					var value = item.component.GetValue();
			//					if( value != null )
			//					{
			//						var type = value.GetType();

			//						bool fallback = true;

			//						{
			//							//vectors, ColorValue, ColorValuePowered

			//							Vector4F v = Vector4F.Zero;
			//							bool add = true;

			//							if( typeIndexes == null )
			//							{
			//								//!!!!bool? что еще?

			//								typeIndexes = new Dictionary<Type, TypeIndex>();
			//								typeIndexes[ typeof( Vector4F ) ] = TypeIndex.Vector4F;
			//								typeIndexes[ typeof( Vector4 ) ] = TypeIndex.Vector4;
			//								typeIndexes[ typeof( Vector4I ) ] = TypeIndex.Vector4I;
			//								typeIndexes[ typeof( Vector3F ) ] = TypeIndex.Vector3F;
			//								typeIndexes[ typeof( Vector3 ) ] = TypeIndex.Vector3;
			//								typeIndexes[ typeof( Vector3I ) ] = TypeIndex.Vector3I;
			//								typeIndexes[ typeof( Vector2F ) ] = TypeIndex.Vector2F;
			//								typeIndexes[ typeof( Vector2 ) ] = TypeIndex.Vector2;
			//								typeIndexes[ typeof( Vector2I ) ] = TypeIndex.Vector2I;
			//								typeIndexes[ typeof( int ) ] = TypeIndex.Int;
			//								typeIndexes[ typeof( float ) ] = TypeIndex.Float;
			//								typeIndexes[ typeof( double ) ] = TypeIndex.Double;
			//								typeIndexes[ typeof( ColorValue ) ] = TypeIndex.ColorValue;
			//								typeIndexes[ typeof( ColorValuePowered ) ] = TypeIndex.ColorValuePowered;
			//								typeIndexes[ typeof( Quaternion ) ] = TypeIndex.Quaternion;
			//								typeIndexes[ typeof( QuaternionF ) ] = TypeIndex.QuaternionF;
			//								typeIndexes[ typeof( PlaneF ) ] = TypeIndex.PlaneF;
			//								typeIndexes[ typeof( Plane ) ] = TypeIndex.Plane;
			//								typeIndexes[ typeof( Radian ) ] = TypeIndex.Radian;
			//								typeIndexes[ typeof( RadianF ) ] = TypeIndex.RadianF;
			//								typeIndexes[ typeof( Degree ) ] = TypeIndex.Degree;
			//								typeIndexes[ typeof( DegreeF ) ] = TypeIndex.DegreeF;
			//								typeIndexes[ typeof( Rectangle ) ] = TypeIndex.Rectangle;
			//								typeIndexes[ typeof( RectangleF ) ] = TypeIndex.RectangleF;
			//								typeIndexes[ typeof( RectangleI ) ] = TypeIndex.RectangleI;
			//								typeIndexes[ typeof( Range ) ] = TypeIndex.Range;
			//								typeIndexes[ typeof( RangeF ) ] = TypeIndex.RangeF;
			//								typeIndexes[ typeof( RangeI ) ] = TypeIndex.RangeI;
			//								typeIndexes[ typeof( SphericalDirection ) ] = TypeIndex.SphericalDirection;
			//								typeIndexes[ typeof( SphericalDirectionF ) ] = TypeIndex.SphericalDirectionF;
			//								typeIndexes[ typeof( Angles ) ] = TypeIndex.Angles;
			//								typeIndexes[ typeof( AnglesF ) ] = TypeIndex.AnglesF;
			//							}

			//							if( typeIndexes.TryGetValue( type, out var index ) )
			//							{
			//								switch( index )
			//								{
			//								case TypeIndex.Vector4F: v = (Vector4F)value; break;
			//								case TypeIndex.Vector4: v = ( (Vector4)value ).ToVector4F(); break;
			//								case TypeIndex.Vector4I: v = ( (Vector4I)value ).ToVector4F(); break;
			//								case TypeIndex.Vector3F: v = new Vector4F( (Vector3F)value, 0 ); break;
			//								case TypeIndex.Vector3: v = new Vector4F( ( (Vector3)value ).ToVector3F(), 0 ); break;
			//								case TypeIndex.Vector3I: v = new Vector4F( ( (Vector3I)value ).ToVector3F(), 0 ); break;
			//								case TypeIndex.Vector2F:
			//									{
			//										var t = (Vector2F)value;
			//										v = new Vector4F( t.X, t.Y, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.Vector2:
			//									{
			//										var t = (Vector2)value;
			//										v = new Vector4F( (float)t.X, (float)t.Y, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.Vector2I:
			//									{
			//										var t = (Vector2I)value;
			//										v = new Vector4F( t.X, t.Y, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.Int: v = new Vector4F( (int)value, 0, 0, 0 ); break;
			//								case TypeIndex.Float: v = new Vector4F( (float)value, 0, 0, 0 ); break;
			//								case TypeIndex.Double: v = new Vector4F( (float)(double)value, 0, 0, 0 ); break;
			//								case TypeIndex.ColorValue: v = ( (ColorValue)value ).ToVector4F(); break;
			//								case TypeIndex.ColorValuePowered: v = ( (ColorValuePowered)value ).ToVector4F(); break;
			//								case TypeIndex.Quaternion:
			//									{
			//										var t = (Quaternion)value;
			//										v = new Vector4F( (float)t.X, (float)t.Y, (float)t.Z, (float)t.W );
			//									}
			//									break;
			//								case TypeIndex.QuaternionF:
			//									{
			//										var t = (QuaternionF)value;
			//										v = new Vector4F( t.X, t.Y, t.Z, t.W );
			//									}
			//									break;
			//								case TypeIndex.PlaneF: v = ( (PlaneF)value ).ToVector4F(); break;
			//								case TypeIndex.Plane: v = ( (Plane)value ).ToPlaneF().ToVector4F(); break;
			//								case TypeIndex.Radian: v = new Vector4F( (float)(Radian)value, 0, 0, 0 ); break;
			//								case TypeIndex.RadianF: v = new Vector4F( (RadianF)value, 0, 0, 0 ); break;
			//								case TypeIndex.Degree: v = new Vector4F( (float)(Degree)value, 0, 0, 0 ); break;
			//								case TypeIndex.DegreeF: v = new Vector4F( (DegreeF)value, 0, 0, 0 ); break;
			//								case TypeIndex.Rectangle:
			//									{
			//										var t = (Rectangle)value;
			//										v = new Vector4F( (float)t.Left, (float)t.Top, (float)t.Right, (float)t.Bottom );
			//									}
			//									break;
			//								case TypeIndex.RectangleF:
			//									{
			//										var t = (RectangleF)value;
			//										v = new Vector4F( t.Left, t.Top, t.Right, t.Bottom );
			//									}
			//									break;
			//								case TypeIndex.RectangleI:
			//									{
			//										var t = (RectangleI)value;
			//										v = new Vector4F( t.Left, t.Top, t.Right, t.Bottom );
			//									}
			//									break;
			//								case TypeIndex.Range:
			//									{
			//										var t = (Range)value;
			//										v = new Vector4F( (float)t.Minimum, (float)t.Maximum, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.RangeF:
			//									{
			//										var t = (RangeF)value;
			//										v = new Vector4F( t.Minimum, t.Maximum, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.RangeI:
			//									{
			//										var t = (RangeI)value;
			//										v = new Vector4F( t.Minimum, t.Maximum, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.SphericalDirection:
			//									{
			//										var t = (SphericalDirection)value;
			//										v = new Vector4F( (float)t.Horizontal, (float)t.Vertical, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.SphericalDirectionF:
			//									{
			//										var t = (SphericalDirectionF)value;
			//										v = new Vector4F( t.Horizontal, t.Vertical, 0, 0 );
			//									}
			//									break;
			//								case TypeIndex.Angles:
			//									{
			//										var t = (Angles)value;
			//										v = new Vector4F( (float)t.Roll, (float)t.Pitch, (float)t.Yaw, 0 );
			//									}
			//									break;
			//								case TypeIndex.AnglesF:
			//									{
			//										var t = (AnglesF)value;
			//										v = new Vector4F( t.Roll, t.Pitch, t.Yaw, 0 );
			//									}
			//									break;
			//								default: Log.Fatal( "Component_Material: CompiledMaterialData: Internal error." ); break;
			//								}
			//							}
			//							else
			//								add = false;

			//							if( add )
			//							{
			//								if( item.uniform == null || item.uniformType == typeof( Vector4F ) )
			//								{
			//									if( item.uniform == null )
			//									{
			//										item.uniform = GpuProgramManager.RegisterUniform( item.nameInShader, UniformType.Vector4, 1 );
			//										item.uniformType = typeof( Vector4F );
			//									}

			//									Bgfx.SetUniform( item.uniform.Value, &v, 1 );

			//									fallback = false;
			//								}
			//							}
			//						}

			//						if( fallback )
			//						{
			//							//!!!!slowly

			//							//!!!!
			//							//convert to float type
			//							value = SimpleTypes.ConvertDoubleToFloat( value );
			//							//ColorValuePowered: convert to ColorValue
			//							if( value is ColorValuePowered )
			//								value = ( (ColorValuePowered)value ).ToColorValue();

			//							if( container == null )
			//								container = new ParameterContainer();

			//							container.Set( item.nameInShader, value );
			//						}
			//					}
			//				}
			//			}

			//			if( code.textures != null )
			//			{
			//				for( int nItem = 0; nItem < code.textures.Count; nItem++ )
			//				{
			//					var item = code.textures[ nItem ];

			//					Component_Image texture = item.component.Texture;
			//					//!!!!не только 2D
			//					if( texture == null )
			//						texture = ResourceUtility.WhiteTexture2D;

			//					FilterOption minMag = EngineSettings.Init.AnisotropicFiltering ? FilterOption.Anisotropic : FilterOption.Linear;

			//					var textureValue = new ViewportRenderingContext.BindTextureData( item.textureRegister, texture, TextureAddressingMode.Wrap, minMag, minMag, FilterOption.Linear );
			//					context.BindTexture( ref textureValue );
			//					//container.Set( ref textureValue );
			//				}
			//			}
			//		}
			//	}

			//	if( container != null )
			//		context.BindParameterContainer( container );
			//}

			public unsafe void PrepareCurrentFrameData( ViewportRenderingContext context, bool specialShadowCaster )
			{
				CurrentFrameData data;
				if( specialShadowCaster )
				{
					if( currentFrameDataSpecialShadowCaster == null )
						currentFrameDataSpecialShadowCaster = new CurrentFrameData();
					data = currentFrameDataSpecialShadowCaster;
				}
				else
				{
					if( currentFrameData == null )
						currentFrameData = new CurrentFrameData();
					data = currentFrameData;
				}

				//!!!!для shader caster что-то может не надо биндить

				if( shaderGenerationEnable )
				{
					//!!!!склеивается два шейдера. пока так

					var codes = new List<ShaderGenerator.ResultData>();
					if( specialShadowCaster )
					{
						if( vertexGeneratedCodeShadowCaster != null )
							codes.Add( vertexGeneratedCodeShadowCaster );
						if( fragmentGeneratedCodeShadowCaster != null )
							codes.Add( fragmentGeneratedCodeShadowCaster );
					}
					else
					{
						if( vertexGeneratedCode != null )
							codes.Add( vertexGeneratedCode );
						if( displacementGeneratedCode != null )
							codes.Add( displacementGeneratedCode );
						if( fragmentGeneratedCode != null )
							codes.Add( fragmentGeneratedCode );
					}

					for( int nCode = 0; nCode < codes.Count; nCode++ )//foreach( var code in codes )
					{
						var code = codes[ nCode ];

						if( code.parameters != null )
						{
							for( int nItem = 0; nItem < code.parameters.Count; nItem++ )//foreach( var item in code.parameters )
							{
								var item = code.parameters[ nItem ];
								//!!!!надо ссылки сконвертить или как-то так? чтобы работало, если это в типе

								var value = item.component.GetValue();
								if( value != null )
								{
									var type = value.GetType();

									bool fallback = true;

									{
										//vectors, ColorValue, ColorValuePowered

										Vector4F v = Vector4F.Zero;
										bool add = true;

										if( typeIndexes == null )
										{
											//!!!!bool? что еще?

											typeIndexes = new Dictionary<Type, TypeIndex>();
											typeIndexes[ typeof( Vector4F ) ] = TypeIndex.Vector4F;
											typeIndexes[ typeof( Vector4 ) ] = TypeIndex.Vector4;
											typeIndexes[ typeof( Vector4I ) ] = TypeIndex.Vector4I;
											typeIndexes[ typeof( Vector3F ) ] = TypeIndex.Vector3F;
											typeIndexes[ typeof( Vector3 ) ] = TypeIndex.Vector3;
											typeIndexes[ typeof( Vector3I ) ] = TypeIndex.Vector3I;
											typeIndexes[ typeof( Vector2F ) ] = TypeIndex.Vector2F;
											typeIndexes[ typeof( Vector2 ) ] = TypeIndex.Vector2;
											typeIndexes[ typeof( Vector2I ) ] = TypeIndex.Vector2I;
											typeIndexes[ typeof( int ) ] = TypeIndex.Int;
											typeIndexes[ typeof( float ) ] = TypeIndex.Float;
											typeIndexes[ typeof( double ) ] = TypeIndex.Double;
											typeIndexes[ typeof( ColorValue ) ] = TypeIndex.ColorValue;
											typeIndexes[ typeof( ColorValuePowered ) ] = TypeIndex.ColorValuePowered;
											typeIndexes[ typeof( Quaternion ) ] = TypeIndex.Quaternion;
											typeIndexes[ typeof( QuaternionF ) ] = TypeIndex.QuaternionF;
											typeIndexes[ typeof( PlaneF ) ] = TypeIndex.PlaneF;
											typeIndexes[ typeof( Plane ) ] = TypeIndex.Plane;
											typeIndexes[ typeof( Radian ) ] = TypeIndex.Radian;
											typeIndexes[ typeof( RadianF ) ] = TypeIndex.RadianF;
											typeIndexes[ typeof( Degree ) ] = TypeIndex.Degree;
											typeIndexes[ typeof( DegreeF ) ] = TypeIndex.DegreeF;
											typeIndexes[ typeof( Rectangle ) ] = TypeIndex.Rectangle;
											typeIndexes[ typeof( RectangleF ) ] = TypeIndex.RectangleF;
											typeIndexes[ typeof( RectangleI ) ] = TypeIndex.RectangleI;
											typeIndexes[ typeof( Range ) ] = TypeIndex.Range;
											typeIndexes[ typeof( RangeF ) ] = TypeIndex.RangeF;
											typeIndexes[ typeof( RangeI ) ] = TypeIndex.RangeI;
											typeIndexes[ typeof( SphericalDirection ) ] = TypeIndex.SphericalDirection;
											typeIndexes[ typeof( SphericalDirectionF ) ] = TypeIndex.SphericalDirectionF;
											typeIndexes[ typeof( Angles ) ] = TypeIndex.Angles;
											typeIndexes[ typeof( AnglesF ) ] = TypeIndex.AnglesF;
										}

										if( typeIndexes.TryGetValue( type, out var index ) )
										{
											switch( index )
											{
											case TypeIndex.Vector4F: v = (Vector4F)value; break;
											case TypeIndex.Vector4: v = ( (Vector4)value ).ToVector4F(); break;
											case TypeIndex.Vector4I: v = ( (Vector4I)value ).ToVector4F(); break;
											case TypeIndex.Vector3F: v = new Vector4F( (Vector3F)value, 0 ); break;
											case TypeIndex.Vector3: v = new Vector4F( ( (Vector3)value ).ToVector3F(), 0 ); break;
											case TypeIndex.Vector3I: v = new Vector4F( ( (Vector3I)value ).ToVector3F(), 0 ); break;
											case TypeIndex.Vector2F:
												{
													var t = (Vector2F)value;
													v = new Vector4F( t.X, t.Y, 0, 0 );
												}
												break;
											case TypeIndex.Vector2:
												{
													var t = (Vector2)value;
													v = new Vector4F( (float)t.X, (float)t.Y, 0, 0 );
												}
												break;
											case TypeIndex.Vector2I:
												{
													var t = (Vector2I)value;
													v = new Vector4F( t.X, t.Y, 0, 0 );
												}
												break;
											case TypeIndex.Int: v = new Vector4F( (int)value, 0, 0, 0 ); break;
											case TypeIndex.Float: v = new Vector4F( (float)value, 0, 0, 0 ); break;
											case TypeIndex.Double: v = new Vector4F( (float)(double)value, 0, 0, 0 ); break;
											case TypeIndex.ColorValue: v = ( (ColorValue)value ).ToVector4F(); break;
											case TypeIndex.ColorValuePowered: v = ( (ColorValuePowered)value ).ToVector4F(); break;
											case TypeIndex.Quaternion:
												{
													var t = (Quaternion)value;
													v = new Vector4F( (float)t.X, (float)t.Y, (float)t.Z, (float)t.W );
												}
												break;
											case TypeIndex.QuaternionF:
												{
													var t = (QuaternionF)value;
													v = new Vector4F( t.X, t.Y, t.Z, t.W );
												}
												break;
											case TypeIndex.PlaneF: v = ( (PlaneF)value ).ToVector4F(); break;
											case TypeIndex.Plane: v = ( (Plane)value ).ToPlaneF().ToVector4F(); break;
											case TypeIndex.Radian: v = new Vector4F( (float)(Radian)value, 0, 0, 0 ); break;
											case TypeIndex.RadianF: v = new Vector4F( (RadianF)value, 0, 0, 0 ); break;
											case TypeIndex.Degree: v = new Vector4F( (float)(Degree)value, 0, 0, 0 ); break;
											case TypeIndex.DegreeF: v = new Vector4F( (DegreeF)value, 0, 0, 0 ); break;
											case TypeIndex.Rectangle:
												{
													var t = (Rectangle)value;
													v = new Vector4F( (float)t.Left, (float)t.Top, (float)t.Right, (float)t.Bottom );
												}
												break;
											case TypeIndex.RectangleF:
												{
													var t = (RectangleF)value;
													v = new Vector4F( t.Left, t.Top, t.Right, t.Bottom );
												}
												break;
											case TypeIndex.RectangleI:
												{
													var t = (RectangleI)value;
													v = new Vector4F( t.Left, t.Top, t.Right, t.Bottom );
												}
												break;
											case TypeIndex.Range:
												{
													var t = (Range)value;
													v = new Vector4F( (float)t.Minimum, (float)t.Maximum, 0, 0 );
												}
												break;
											case TypeIndex.RangeF:
												{
													var t = (RangeF)value;
													v = new Vector4F( t.Minimum, t.Maximum, 0, 0 );
												}
												break;
											case TypeIndex.RangeI:
												{
													var t = (RangeI)value;
													v = new Vector4F( t.Minimum, t.Maximum, 0, 0 );
												}
												break;
											case TypeIndex.SphericalDirection:
												{
													var t = (SphericalDirection)value;
													v = new Vector4F( (float)t.Horizontal, (float)t.Vertical, 0, 0 );
												}
												break;
											case TypeIndex.SphericalDirectionF:
												{
													var t = (SphericalDirectionF)value;
													v = new Vector4F( t.Horizontal, t.Vertical, 0, 0 );
												}
												break;
											case TypeIndex.Angles:
												{
													var t = (Angles)value;
													v = new Vector4F( (float)t.Roll, (float)t.Pitch, (float)t.Yaw, 0 );
												}
												break;
											case TypeIndex.AnglesF:
												{
													var t = (AnglesF)value;
													v = new Vector4F( t.Roll, t.Pitch, t.Yaw, 0 );
												}
												break;
											default: Log.Fatal( "Component_Material: CompiledMaterialData: Internal error." ); break;
											}
										}
										else
											add = false;

										if( add )
										{
											if( item.uniform == null || item.uniformType == typeof( Vector4F ) )
											{
												if( item.uniform == null )
												{
													item.uniform = GpuProgramManager.RegisterUniform( item.nameInShader, UniformType.Vector4, 1 );
													item.uniformType = typeof( Vector4F );
												}

												if( data.Uniforms == null )
													data.Uniforms = new OpenList<(Uniform, Vector4F)>();
												data.Uniforms.Add( (item.uniform.Value, v) );

												//Bgfx.SetUniform( item.uniform.Value, &v, 1 );

												fallback = false;
											}
										}
									}

									if( fallback )
									{
										//!!!!slowly

										//!!!!
										//convert to float type
										value = SimpleTypes.ConvertDoubleToFloat( value );
										//ColorValuePowered: convert to ColorValue
										if( value is ColorValuePowered )
											value = ( (ColorValuePowered)value ).ToColorValue();

										if( data.FallbackUniformContainer == null )
											data.FallbackUniformContainer = new ParameterContainer();
										data.FallbackUniformContainer.Set( item.nameInShader, value );

										//if( container == null )
										//	container = new ParameterContainer();
										//container.Set( item.nameInShader, value );
									}
								}
							}
						}

						if( code.textures != null )//&& bindTextures )
						{
							for( int nItem = 0; nItem < code.textures.Count; nItem++ )
							{
								var item = code.textures[ nItem ];

								Component_Image texture = item.component.Texture;
								//!!!!не только 2D
								if( texture == null )
									texture = ResourceUtility.WhiteTexture2D;

								var minMag = EngineSettings.Init.AnisotropicFiltering ? FilterOption.Anisotropic : FilterOption.Linear;
								var textureValue = new ViewportRenderingContext.BindTextureData( item.textureRegister, texture, TextureAddressingMode.Wrap, minMag, minMag, FilterOption.Linear );

								if( data.Textures == null )
									data.Textures = new OpenList<ViewportRenderingContext.BindTextureData>();
								data.Textures.Add( ref textureValue );

								//context.BindTexture( ref textureValue );
							}
						}
					}
				}

				//if( container != null )
				//{
				//}
			}

			public unsafe void BindCurrentFrameData( ViewportRenderingContext context, bool specialShadowCaster, bool setUniforms, bool bindTextures )
			{
				CurrentFrameData data;
				if( specialShadowCaster )
					data = currentFrameDataSpecialShadowCaster;
				else
					data = currentFrameData;

				if( setUniforms )
				{
					var uniforms = data.Uniforms;
					if( uniforms != null )
					{
						for( int n = 0; n < uniforms.Count; n++ )
						{
							ref var pair = ref uniforms.Data[ n ];
							var uniform = pair.Item1;
							var value = pair.Item2;
							Bgfx.SetUniform( uniform, &value, 1 );
						}
					}

					if( data.FallbackUniformContainer != null )
						context.BindParameterContainer( data.FallbackUniformContainer );
				}

				if( bindTextures )
				{
					var textures = data.Textures;
					if( textures != null )
					{
						for( int n = 0; n < textures.Count; n++ )
							context.BindTexture( ref textures.Data[ n ] );
					}
				}
			}

			public void BindCurrentFrameData_MaskTextures( ViewportRenderingContext context, Component_Image mask )
			{
				if( mask?.Result != null )
				{
					for( int nCode = 0; nCode < 3; nCode++ )
					{
						ShaderGenerator.ResultData generatedCode = null;
						switch( nCode )
						{
						case 0: generatedCode = vertexGeneratedCode; break;
						case 1: generatedCode = displacementGeneratedCode; break;
						case 2: generatedCode = fragmentGeneratedCode; break;
						}

						if( generatedCode != null )
						{
							var texturesMask = generatedCode.texturesMask;
							if( texturesMask != null )
							{
								for( int n = 0; n < texturesMask.Count; n++ )
								{
									var item = texturesMask[ n ];

									var minMag = EngineSettings.Init.AnisotropicFiltering ? FilterOption.Anisotropic : FilterOption.Linear;
									var textureValue = new ViewportRenderingContext.BindTextureData( item.textureRegister, mask, TextureAddressingMode.Wrap, minMag, minMag, FilterOption.Linear );

									context.BindTexture( ref textureValue );
								}
							}
						}
					}
				}
			}

		}
	}
}
