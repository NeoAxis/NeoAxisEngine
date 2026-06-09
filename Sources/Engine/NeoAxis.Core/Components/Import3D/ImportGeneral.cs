// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Base class for implementation content import.
	/// </summary>
	public class ImportGeneral
	{
		/// <summary>
		/// Represents an import settings.
		/// </summary>
		public class Settings
		{
			public bool updateMaterials = true;
			public bool updateMaterialsOnMeshes = true;
			//public bool updateMeshes = true;
			//public bool updateMeshLODs = true;
			//public bool updateObjectsInSpace = true;
			public bool resetCollision;
			public bool resetEditorSettings;

			public Dictionary<string, string> meshGeometryMaterialsToRestore = new Dictionary<string, string>();
			public Dictionary<string, MultiMaterial> meshGeometryMultiMaterialsToRestore = new Dictionary<string, MultiMaterial>();
			public Dictionary<string, RigidBody> collisionToRestore = new Dictionary<string, RigidBody>();
			public Dictionary<string, MeshEditorSettings> meshEditorSeetingsToRestore = new Dictionary<string, MeshEditorSettings>();

			public Import3D component;
			public string virtualFileName;
			//public bool loadAnimations;
			//public double frameStep;
			//public Mat4 globalTransform;

			public bool disableDeletionUnusedMaterials;

			//

			public class MeshEditorSettings
			{
				public bool EditorDisplayPivot;
				public bool EditorDisplayBounds;
				public bool EditorDisplayTriangles;
				public bool EditorDisplayVertices;
				public bool EditorDisplayNormals;
				public bool EditorDisplayTangents;
				public bool EditorDisplayBinormals;
				public bool EditorDisplayVertexColor;
				public int EditorDisplayUV;
				public bool EditorDisplayProxyMesh;
				public int EditorDisplayLOD;
				public bool EditorDisplayCollision;
				public bool EditorDisplaySkeleton;
				public string EditorPlayAnimation;
				public Transform EditorCameraTransform;
			}
		}

		///////////////////////////////////////////////

		/// <summary>
		/// The data to import material.
		/// </summary>
		public class MaterialData
		{
			public int Index;
			public string Name;
			public Material.BlendModeEnum BlendMode = Material.BlendModeEnum.Opaque;
			public Material.ShadingModelEnum ShadingModel = Material.ShadingModelEnum.Lit;
			public bool TwoSided;

			public string BaseColorTexture;
			public int BaseColorTextureUVIndex;
			public TextureUVTransform BaseColorTextureUVTransform;
			public ColorValue? BaseColor;

			public string MetallicTexture;
			public string MetallicTextureChannel = "R";
			public int MetallicTextureUVIndex;
			public TextureUVTransform MetallicTextureUVTransform;
			public double Metallic;

			public string RoughnessTexture;
			public string RoughnessTextureChannel = "R";
			public int RoughnessTextureUVIndex;
			public TextureUVTransform RoughnessTextureUVTransform;
			public double Roughness = 0.5;

			public string NormalTexture;
			public int NormalTextureUVIndex;
			public TextureUVTransform NormalTextureUVTransform;

			public string DisplacementTexture;
			public string DisplacementTextureChannel = "R";
			public int DisplacementTextureUVIndex;
			public TextureUVTransform DisplacementTextureUVTransform;

			public string AmbientOcclusionTexture;
			public string AmbientOcclusionTextureChannel = "R";
			public int AmbientOcclusionTextureUVIndex;
			public TextureUVTransform AmbientOcclusionTextureUVTransform;

			public string EmissiveTexture;
			public int EmissiveTextureUVIndex;
			public TextureUVTransform EmissiveTextureUVTransform;
			public ColorValue? EmissionColor;

			public string OpacityTexture;
			public string OpacityTextureChannel = "R";
			public int OpacityTextureUVIndex;
			public TextureUVTransform OpacityTextureUVTransform;
			public double Opacity = 1;
			public double OpacityMaskThreshold = 0.5;


			//Clearcoat

			public double Clearcoat;
			public string ClearcoatTexture;
			public string ClearcoatTextureChannel = "R";
			public int ClearcoatTextureUVIndex;
			public TextureUVTransform ClearcoatTextureUVTransform;

			public string ClearcoatRoughnessTexture;
			public string ClearcoatRoughnessTextureChannel = "R";
			public int ClearcoatRoughnessTextureUVIndex;
			public TextureUVTransform ClearcoatRoughnessTextureUVTransform;
			public double ClearcoatRoughness;

			public string ClearcoatNormalTexture;
			public int ClearcoatNormalTextureUVIndex;
			public TextureUVTransform ClearcoatNormalTextureUVTransform;


			//Sheen
			public ColorValue? SheenColor;
			public string SheenColorTexture;
			public int SheenColorTextureUVIndex;
			public TextureUVTransform SheenColorTextureUVTransform;


			//

			public int GetTextureUsedCount()
			{
				var names = new string[] { BaseColorTexture, MetallicTexture, RoughnessTexture, NormalTexture, DisplacementTexture, AmbientOcclusionTexture, EmissiveTexture, OpacityTexture };

				var added = new ESet<string>();
				foreach( var name in names )
				{
					if( !string.IsNullOrEmpty( name ) )
						added.AddWithCheckAlreadyContained( name );
				}
				return added.Count;
			}
		}

		///////////////////////////////////////////////

		public class TextureUVTransform
		{
			public Vector2F Translation;
			public Vector2F Scale = new Vector2F( 1, 1 );
			public float Rotation;
		}

		///////////////////////////////////////////////

		public static string GetFixedName( string name )
		{
			char[] invalidChars = Path.GetInvalidFileNameChars();
			string trimmedName = name.Trim();
			//string trimmedName = name.Trim().Trim( invalidChars );
			StringBuilder builder = new StringBuilder();
			foreach( char c in trimmedName )
			{
				char fixedChar = c;
				if( Array.IndexOf<char>( invalidChars, fixedChar ) != -1 )
					fixedChar = '_';
				builder.Append( fixedChar );
			}
			return builder.ToString();
		}

		public static Material CreateMaterial( Settings settings, Component materialsGroup, MaterialData data )
		{
			//rounding
			data.Metallic = Math.Round( data.Metallic, 5 );
			data.Roughness = Math.Round( data.Roughness, 5 );
			data.Opacity = Math.Round( data.Opacity, 5 );
			data.OpacityMaskThreshold = Math.Round( data.OpacityMaskThreshold, 5 );
			data.Clearcoat = Math.Round( data.Clearcoat, 5 );
			data.ClearcoatRoughness = Math.Round( data.ClearcoatRoughness, 5 );
			if( data.BaseColor != null )
			{
				data.BaseColor = new ColorValue( Math.Round( data.BaseColor.Value.Red, 5 ), Math.Round( data.BaseColor.Value.Green, 5 ), Math.Round( data.BaseColor.Value.Blue, 5 ), Math.Round( data.BaseColor.Value.Alpha, 5 ) );
			}
			if( data.EmissionColor != null )
			{
				data.EmissionColor = new ColorValue( Math.Round( data.EmissionColor.Value.Red, 5 ), Math.Round( data.EmissionColor.Value.Green, 5 ), Math.Round( data.EmissionColor.Value.Blue, 5 ), Math.Round( data.EmissionColor.Value.Alpha, 5 ) );
			}
			if( data.SheenColor != null )
			{
				data.SheenColor = new ColorValue( Math.Round( data.SheenColor.Value.Red, 5 ), Math.Round( data.SheenColor.Value.Green, 5 ), Math.Round( data.SheenColor.Value.Blue, 5 ), Math.Round( data.SheenColor.Value.Alpha, 5 ) );
			}

			//create material
			var material = materialsGroup.CreateComponent<Material>( enabled: false );
			material.Name = data.Name;
			material.BlendMode = data.BlendMode;
			material.ShadingModel = data.ShadingModel;
			material.TwoSided = data.TwoSided;
			material.OpacityMaskThreshold = data.OpacityMaskThreshold;

			//blend mode
			if( !string.IsNullOrEmpty( data.OpacityTexture ) || data.Opacity < 1 )
			{
				switch( settings.component.TransparentMaterialBlending.Value )
				{
				case Import3D.TransparentMaterialBlendingEnum.Auto:
					break;
				case Import3D.TransparentMaterialBlendingEnum.Opaque:
					material.BlendMode = Material.BlendModeEnum.Opaque;
					break;
				case Import3D.TransparentMaterialBlendingEnum.Masked:
					material.BlendMode = Material.BlendModeEnum.Masked;
					break;
				case Import3D.TransparentMaterialBlendingEnum.Transparent:
					material.BlendMode = Material.BlendModeEnum.Transparent;
					break;
				}
			}

			if( material.BlendMode.Value == Material.BlendModeEnum.Masked )
				material.OpacityDithering = settings.component.MaskedMaterialDithering;

			//create shader graph
			FlowGraph graph;
			{
				graph = material.CreateComponent<FlowGraph>();
				graph.Name = material.Name + " shader graph";
				graph.Specialization = ReferenceUtility.MakeReference<FlowGraphSpecialization>( null,
					MetadataManager.GetTypeOfNetType( typeof( FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

				var node = graph.CreateComponent<FlowGraphNode>();
				node.Name = "Node " + material.Name;
				node.Position = new Vector2I( 10, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, material );
			}

			const int step = 9;
			Vector2I position = new Vector2I( -20, -data.GetTextureUsedCount() * step / 2 );
			var addedTextures = new Dictionary<string, Component>();

			Component GetOrCreateTextureSample( string channelDisplayName, string textureName, int uvIndex, TextureUVTransform uvTransform, ColorValue? colorMultiplier = null, double? floatMultiplier = null, string floatMultiplierChannel = null )
			{
				//rounding
				if( uvTransform != null )
				{
					uvTransform.Translation.X = (float)Math.Round( uvTransform.Translation.X, 5 );
					uvTransform.Translation.Y = (float)Math.Round( uvTransform.Translation.Y, 5 );
					uvTransform.Rotation = (float)Math.Round( uvTransform.Rotation, 5 );
					uvTransform.Scale.X = (float)Math.Round( uvTransform.Scale.X, 5 );
					uvTransform.Scale.Y = (float)Math.Round( uvTransform.Scale.Y, 5 );
				}

				var key = $"{textureName}_{uvIndex}";
				if( colorMultiplier.HasValue && colorMultiplier.Value != ColorValue.One )
					key += $"_{colorMultiplier.Value}";
				if( floatMultiplier.HasValue && floatMultiplier.Value != 1 )
					key += $"_{floatMultiplier.Value}";

				if( !addedTextures.TryGetValue( key, out var endComponent ) )
				{
					InvokeMember texCoordInvokeMember = null;
					InvokeMember uvTransformInvokeMember = null;

					if( uvTransform != null )
					{
						//UVTransform is not identity

						//TexCoord block
						{
							var node = graph.CreateComponent<FlowGraphNode>();
							node.Name = $"Node Tex Coord {channelDisplayName}";
							node.Position = position + new Vector2I( -13 * 2, 0 );

							var invokeMember = node.CreateComponent<InvokeMember>();
							invokeMember.Name = ComponentUtility.GetNewObjectUniqueName( invokeMember );
							invokeMember.Member = new Reference<ReferenceValueType_Member>( null, $"NeoAxis.ShaderConstants|property:TexCoord{uvIndex}" );

							node.ControlledObject = ReferenceUtility.MakeThisReference( node, invokeMember );

							texCoordInvokeMember = invokeMember;
						}

						//UVTransform function
						{
							var node = graph.CreateComponent<FlowGraphNode>();
							node.Name = $"Node UV Transform {channelDisplayName}";
							node.Position = position + new Vector2I( -13, 0 );

							var invokeMember = node.CreateComponent<InvokeMember>();
							invokeMember.Name = ComponentUtility.GetNewObjectUniqueName( invokeMember );
							invokeMember.Member = new Reference<ReferenceValueType_Member>( null, "NeoAxis.ShaderFunctions|method:UVTransform(NeoAxis.Vector2,System.Single,NeoAxis.Vector2,NeoAxis.Vector2)" );
							invokeMember.SetPropertyValue( "property:__parameter_Translation", new Reference<Vector2>( uvTransform.Translation ) );
							invokeMember.SetPropertyValue( "property:__parameter_Rotation", new Reference<float>( uvTransform.Rotation ) );
							invokeMember.SetPropertyValue( "property:__parameter_Scale", new Reference<Vector2>( uvTransform.Scale ) );
							invokeMember.SetPropertyValue( "property:__parameter_TexCoord", ReferenceUtility.MakeThisReference( invokeMember, texCoordInvokeMember, "__value_Value" ) );

							node.ControlledObject = ReferenceUtility.MakeThisReference( node, invokeMember );

							uvTransformInvokeMember = invokeMember;
						}
					}
					else
					{
						//TexCoord not zero
						if( uvIndex >= 1 && uvIndex <= 2 )
						{
							var node = graph.CreateComponent<FlowGraphNode>();
							node.Name = $"Node Tex Coord {channelDisplayName}";
							node.Position = position + new Vector2I( -13, 0 );

							var invokeMember = node.CreateComponent<InvokeMember>();
							invokeMember.Name = ComponentUtility.GetNewObjectUniqueName( invokeMember );
							invokeMember.Member = new Reference<ReferenceValueType_Member>( null, $"NeoAxis.ShaderConstants|property:TexCoord{uvIndex}" );

							node.ControlledObject = ReferenceUtility.MakeThisReference( node, invokeMember );

							texCoordInvokeMember = invokeMember;
						}
					}

					//Texture Sample
					FlowGraphNode sampleNode;
					{
						var node = graph.CreateComponent<FlowGraphNode>();
						sampleNode = node;
						node.Name = $"Node Texture Sample {channelDisplayName}";
						node.Position = position;

						var sample = node.CreateComponent<ShaderTextureSample>();
						sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
						sample.Texture = new Reference<ImageComponent>( null, textureName );
						if( uvTransformInvokeMember != null )
							sample.Location2 = ReferenceUtility.MakeThisReference( sample, uvTransformInvokeMember, "__parameter_ReturnValue" );
						else if( texCoordInvokeMember != null )
							sample.Location2 = ReferenceUtility.MakeThisReference( sample, texCoordInvokeMember, "__value_Value" );

						node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

						endComponent = sample;
						addedTextures[ key ] = endComponent;
					}

					//combined texture and color multiplier
					if( colorMultiplier.HasValue && colorMultiplier.Value != ColorValue.One )
					{
						var node = graph.CreateComponent<FlowGraphNode>();
						node.Name = $"Node Multiply {channelDisplayName}";
						node.Position = position + new Vector2I( 15, 0 );

						var invokeMember = node.CreateComponent<InvokeMember>();
						invokeMember.Name = ComponentUtility.GetNewObjectUniqueName( invokeMember );
						invokeMember.Member = new Reference<ReferenceValueType_Member>( null, "NeoAxis.ColorValue|method:op_Multiply(NeoAxis.ColorValue,NeoAxis.ColorValue)" );

						invokeMember.SetPropertyValue( "property:__parameter_V1", ReferenceUtility.MakeReference( @$"this:..\..\${sampleNode.Name}\$Shader Texture Sample\RGBA" ) );
						invokeMember.SetPropertyValue( "property:__parameter_V2", new Reference<ColorValue>( colorMultiplier.Value ) );

						node.ControlledObject = ReferenceUtility.MakeThisReference( node, invokeMember );

						endComponent = invokeMember;
						addedTextures[ key ] = endComponent;
					}

					//combined texture and float multiplier
					if( floatMultiplier.HasValue && floatMultiplier.Value != 1 )
					{
						var node = graph.CreateComponent<FlowGraphNode>();
						node.Name = $"Node Multiply {channelDisplayName}";
						node.Position = position + new Vector2I( 15, 0 );

						var invokeMember = node.CreateComponent<InvokeMember>();
						invokeMember.Name = ComponentUtility.GetNewObjectUniqueName( invokeMember );
						invokeMember.Member = new Reference<ReferenceValueType_Member>( null, "NeoAxis.MathEx|method:Multiply(System.Double,System.Double)" );

						invokeMember.SetPropertyValue( "property:__parameter_V1", ReferenceUtility.MakeReference( @$"this:..\..\${sampleNode.Name}\$Shader Texture Sample\{floatMultiplierChannel}" ) );
						invokeMember.SetPropertyValue( "property:__parameter_V2", new Reference<double>( floatMultiplier.Value ) );

						node.ControlledObject = ReferenceUtility.MakeThisReference( node, invokeMember );

						endComponent = invokeMember;
						addedTextures[ key ] = endComponent;
					}

					position.Y += step;
				}

				return endComponent;
			}

			//BaseColor
			if( !string.IsNullOrEmpty( data.BaseColorTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "Base Color", data.BaseColorTexture, data.BaseColorTextureUVIndex, data.BaseColorTextureUVTransform, data.BaseColor );
				if( endComponent is ShaderTextureSample )
					material.BaseColor = ReferenceUtility.MakeThisReference( material, endComponent, "RGBA" );
				else
					material.BaseColor = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else if( data.BaseColor.HasValue )
				material.BaseColor = data.BaseColor.Value;

			//Metallic
			data.Metallic = MathEx.Saturate( data.Metallic );
			if( !string.IsNullOrEmpty( data.MetallicTexture ) && data.Metallic > 0 )
			{
				var endComponent = GetOrCreateTextureSample( "Metallic", data.MetallicTexture, data.MetallicTextureUVIndex, data.MetallicTextureUVTransform, floatMultiplier: data.Metallic, floatMultiplierChannel: data.MetallicTextureChannel );
				if( endComponent is ShaderTextureSample )
					material.Metallic = ReferenceUtility.MakeThisReference( material, endComponent, data.MetallicTextureChannel );
				else
					material.Metallic = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else
				material.Metallic = data.Metallic;

			//Roughness
			data.Roughness = MathEx.Saturate( data.Roughness );
			if( !string.IsNullOrEmpty( data.RoughnessTexture ) && data.Roughness > 0 )
			{
				var endComponent = GetOrCreateTextureSample( "Roughness", data.RoughnessTexture, data.RoughnessTextureUVIndex, data.RoughnessTextureUVTransform, floatMultiplier: data.Roughness, floatMultiplierChannel: data.RoughnessTextureChannel );
				if( endComponent is ShaderTextureSample )
					material.Roughness = ReferenceUtility.MakeThisReference( material, endComponent, data.RoughnessTextureChannel );
				else
					material.Roughness = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else
				material.Roughness = data.Roughness;

			//Normal
			if( !string.IsNullOrEmpty( data.NormalTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Normal", data.NormalTexture, data.NormalTextureUVIndex, data.NormalTextureUVTransform );
				material.Normal = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}

			//Displacement
			if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Displacement", data.DisplacementTexture, data.DisplacementTextureUVIndex, data.DisplacementTextureUVTransform );
				material.Displacement = ReferenceUtility.MakeThisReference( material, sample, data.DisplacementTextureChannel );
			}

			//AmbientOcclusion
			if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Ambient Occlusion", data.AmbientOcclusionTexture, data.AmbientOcclusionTextureUVIndex, data.AmbientOcclusionTextureUVTransform );
				material.AmbientOcclusion = ReferenceUtility.MakeThisReference( material, sample, data.AmbientOcclusionTextureChannel );
			}

			//Emissive
			if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "Emissive", data.EmissiveTexture, data.EmissiveTextureUVIndex, data.EmissiveTextureUVTransform, data.EmissionColor );
				if( endComponent is ShaderTextureSample )
					material.Emissive = ReferenceUtility.MakeThisReference( material, endComponent, "RGBA" );
				else
					material.Emissive = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else if( data.EmissionColor.HasValue )
				material.Emissive = new ColorValuePowered( data.EmissionColor.Value );

			//Opacity
			if( !string.IsNullOrEmpty( data.OpacityTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "Opacity", data.OpacityTexture, data.OpacityTextureUVIndex, data.OpacityTextureUVTransform, floatMultiplier: data.Opacity, floatMultiplierChannel: data.OpacityTextureChannel );
				if( endComponent is ShaderTextureSample )
					material.Opacity = ReferenceUtility.MakeThisReference( material, endComponent, data.OpacityTextureChannel );
				else
					material.Opacity = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else if( data.Opacity >= 0 && data.Opacity < 1 )
				material.Opacity = data.Opacity;


			//Clearcoat

			data.Clearcoat = MathEx.Saturate( data.Clearcoat );

			if( !string.IsNullOrEmpty( data.ClearcoatTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "Clearcoat", data.ClearcoatTexture, data.ClearcoatTextureUVIndex, data.ClearcoatTextureUVTransform, floatMultiplier: data.Clearcoat, floatMultiplierChannel: data.ClearcoatTextureChannel );
				if( endComponent is ShaderTextureSample )
					material.Clearcoat = ReferenceUtility.MakeThisReference( material, endComponent, data.ClearcoatTextureChannel );
				else
					material.Clearcoat = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else
				material.Clearcoat = data.Clearcoat;

			if( !string.IsNullOrEmpty( data.ClearcoatRoughnessTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "ClearcoatRoughness", data.ClearcoatRoughnessTexture, data.ClearcoatRoughnessTextureUVIndex, data.ClearcoatNormalTextureUVTransform, floatMultiplier: data.ClearcoatRoughness, floatMultiplierChannel: data.ClearcoatRoughnessTextureChannel );
				if( endComponent is ShaderTextureSample )
					material.ClearcoatRoughness = ReferenceUtility.MakeThisReference( material, endComponent, data.ClearcoatRoughnessTextureChannel );
				else
					material.ClearcoatRoughness = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
			}
			else
				material.ClearcoatRoughness = data.ClearcoatRoughness;

			if( !string.IsNullOrEmpty( data.ClearcoatNormalTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "ClearcoatNormal", data.ClearcoatNormalTexture, data.ClearcoatNormalTextureUVIndex, data.ClearcoatNormalTextureUVTransform );
				material.ClearcoatNormal = ReferenceUtility.MakeThisReference( material, endComponent, "RGBA" );
			}


			//Sheen

			if( !string.IsNullOrEmpty( data.SheenColorTexture ) )
			{
				var endComponent = GetOrCreateTextureSample( "SheenColor", data.SheenColorTexture, data.SheenColorTextureUVIndex, data.SheenColorTextureUVTransform, data.SheenColor );
				if( endComponent is ShaderTextureSample )
					material.SheenColor = ReferenceUtility.MakeThisReference( material, endComponent, "RGBA" );
				else
					material.SheenColor = ReferenceUtility.MakeThisReference( material, endComponent, "__parameter_ReturnValue" );
				material.ShadingModel = Material.ShadingModelEnum.Cloth;

				//!!!!
				material.SubsurfaceColor = material.SheenColor;
			}
			else if( data.SheenColor.HasValue )
			{
				material.SheenColor = data.SheenColor.Value;
				material.ShadingModel = Material.ShadingModelEnum.Cloth;

				//!!!!
				material.SubsurfaceColor = material.SheenColor;
			}


			material.Enabled = true;

			return material;
		}
	}
}