#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis//.Import
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
			public bool updateMeshes = true;
			public bool updateObjectsInSpace = true;
			public bool resetCollision;
			public bool resetEditorSettings;

			public Dictionary<string, string> meshGeometryMaterialsToRestore = new Dictionary<string, string>();
			public Dictionary<string, MultiMaterial> meshGeometryMultiMaterialsToRestore = new Dictionary<string, MultiMaterial>();
			public Dictionary<string, RigidBody> collisionToRestore = new Dictionary<string, RigidBody>();
			public Dictionary<string, MeshEditorSettings> meshEditorSeetingsToRestore = new Dictionary<string, MeshEditorSettings>();

			public Import3D component;
			public string virtualFileName;
			//public bool loadAnimations;
			public double frameStep;
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
			public Material.ShadingModelEnum ShadingModel = Material.ShadingModelEnum.Lit;
			public bool TwoSided;

			//!!!!type, opacity

			public ColorValue? BaseColor;
			public string BaseColorTexture;
			public string MetallicTexture;
			public string MetallicTextureChannel = "R";
			public string RoughnessTexture;
			public string RoughnessTextureChannel = "R";
			public string NormalTexture;
			public string DisplacementTexture;
			public string DisplacementTextureChannel = "R";
			public string AmbientOcclusionTexture;
			public string AmbientOcclusionTextureChannel = "R";
			public string EmissiveTexture;
			public string OpacityTexture;
			public string OpacityTextureChannel = "R";

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
			//create material
			var material = materialsGroup.CreateComponent<Material>( enabled: false );
			material.Name = data.Name;
			material.ShadingModel = data.ShadingModel;
			material.TwoSided = data.TwoSided;

			if( !string.IsNullOrEmpty( data.OpacityTexture ) )
			{
				switch( settings.component.TransparentMaterialBlending.Value )
				{
				case Import3D.TransparentMaterialBlendingEnum.Opaque:
					material.BlendMode = Material.BlendModeEnum.Opaque;
					break;
				case Import3D.TransparentMaterialBlendingEnum.Masked:
					material.BlendMode = Material.BlendModeEnum.Masked;
					break;
				case Import3D.TransparentMaterialBlendingEnum.MaskedDithering:
					material.BlendMode = Material.BlendModeEnum.Masked;
					material.OpacityDithering = true;
					break;
				case Import3D.TransparentMaterialBlendingEnum.Transparent:
					material.BlendMode = Material.BlendModeEnum.Transparent;
					break;
				}
			}

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


			var addedTextures = new Dictionary<string, ShaderTextureSample>();

			ShaderTextureSample GetOrCreateTextureSample( string channelDisplayName, string textureName )
			{
				if( !addedTextures.TryGetValue( textureName, out var sample ) )
				{
					var node = graph.CreateComponent<FlowGraphNode>();
					node.Name = "Node Texture Sample " + channelDisplayName;//"BaseColor";
					node.Position = position;
					position.Y += step;

					sample = node.CreateComponent<ShaderTextureSample>();
					sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
					sample.Texture = new Reference<ImageComponent>( null, textureName );// data.BaseColorTexture );

					node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

					addedTextures[ textureName ] = sample;
				}

				return sample;
			}

			//BaseColor
			if( !string.IsNullOrEmpty( data.BaseColorTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Base Color", data.BaseColorTexture );
				material.BaseColor = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}
			else if( data.BaseColor.HasValue )
				material.BaseColor = data.BaseColor.Value;

			//Metallic
			if( !string.IsNullOrEmpty( data.MetallicTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Metallic", data.MetallicTexture );
				material.Metallic = ReferenceUtility.MakeThisReference( material, sample, data.MetallicTextureChannel );
			}

			//Roughness
			if( !string.IsNullOrEmpty( data.RoughnessTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Roughness", data.RoughnessTexture );
				material.Roughness = ReferenceUtility.MakeThisReference( material, sample, data.RoughnessTextureChannel );
			}

			//Normal
			if( !string.IsNullOrEmpty( data.NormalTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Normal", data.NormalTexture );
				material.Normal = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}

			//Displacement
			if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Displacement", data.DisplacementTexture );
				material.Displacement = ReferenceUtility.MakeThisReference( material, sample, data.DisplacementTextureChannel );
			}

			//AmbientOcclusion
			if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Ambient Occlusion", data.AmbientOcclusionTexture );
				material.AmbientOcclusion = ReferenceUtility.MakeThisReference( material, sample, data.AmbientOcclusionTextureChannel );
			}

			//Emissive
			if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Emissive", data.EmissiveTexture );
				material.Emissive = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}

			//Opacity
			if( !string.IsNullOrEmpty( data.OpacityTexture ) )
			{
				var sample = GetOrCreateTextureSample( "Opacity", data.OpacityTexture );
				material.Opacity = ReferenceUtility.MakeThisReference( material, sample, data.OpacityTextureChannel );
			}

			material.Enabled = true;

			return material;
		}
	}
}
#endif