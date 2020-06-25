// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace NeoAxis.Import
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

			public Component_Import3D component;
			public string virtualFileName;
			//public bool loadAnimations;
			public double frameStep;
			//public Mat4 globalTransform;

			public bool disableDeletionUnusedMaterials;
		}

		///////////////////////////////////////////////

		/// <summary>
		/// The data to import material.
		/// </summary>
		public class MaterialData
		{
			public int Index;
			public string Name;
			public Component_Material.ShadingModelEnum ShadingModel = Component_Material.ShadingModelEnum.Lit;
			public bool TwoSided;

			//!!!!type, opacity

			public ColorValue? BaseColor;
			public string BaseColorTexture;
			public string MetallicTexture;
			public string RoughnessTexture;
			public string NormalTexture;
			public string DisplacementTexture;
			public string AmbientOcclusionTexture;
			public string EmissiveTexture;
			public string OpacityTexture;

			public int GetTextureCount()
			{
				int result = 0;
				if( !string.IsNullOrEmpty( BaseColorTexture ) )
					result++;
				if( !string.IsNullOrEmpty( MetallicTexture ) )
					result++;
				if( !string.IsNullOrEmpty( RoughnessTexture ) )
					result++;
				if( !string.IsNullOrEmpty( NormalTexture ) )
					result++;
				if( !string.IsNullOrEmpty( DisplacementTexture ) )
					result++;
				if( !string.IsNullOrEmpty( AmbientOcclusionTexture ) )
					result++;
				if( !string.IsNullOrEmpty( EmissiveTexture ) )
					result++;
				if( !string.IsNullOrEmpty( OpacityTexture ) )
					result++;
				return result;
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

		public static Component_Material CreateMaterial( Component materialsGroup, MaterialData data )
		{
			//create material
			var material = materialsGroup.CreateComponent<Component_Material>( enabled: false );
			material.Name = data.Name;
			material.ShadingModel = data.ShadingModel;
			material.TwoSided = data.TwoSided;
			if( !string.IsNullOrEmpty( data.OpacityTexture ) )
				material.BlendMode = Component_Material.BlendModeEnum.Masked;

			//create shader graph
			Component_FlowGraph graph;
			{
				graph = material.CreateComponent<Component_FlowGraph>();
				graph.Name = material.Name + " shader graph";
				graph.Specialization = ReferenceUtility.MakeReference<Component_FlowGraphSpecialization>( null,
					MetadataManager.GetTypeOfNetType( typeof( Component_FlowGraphSpecialization_Shader ) ).Name + "|Instance" );

				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node " + material.Name;
				node.Position = new Vector2I( 10, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, material );
			}

			const int step = 9;
			Vector2I position = new Vector2I( -20, -data.GetTextureCount() * step / 2 );

			//BaseColor
			if( !string.IsNullOrEmpty( data.BaseColorTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "BaseColor";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.BaseColorTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.BaseColor = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}
			else if( data.BaseColor.HasValue )
				material.BaseColor = data.BaseColor.Value;

			//Metallic
			if( !string.IsNullOrEmpty( data.MetallicTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Metallic";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.MetallicTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Metallic = ReferenceUtility.MakeThisReference( material, sample, "R" );
			}

			//Roughness
			if( !string.IsNullOrEmpty( data.RoughnessTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Roughness";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.RoughnessTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Roughness = ReferenceUtility.MakeThisReference( material, sample, "R" );
			}

			//Normal
			if( !string.IsNullOrEmpty( data.NormalTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Normal";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.NormalTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Normal = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}

			//Displacement
			if( !string.IsNullOrEmpty( data.DisplacementTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Displacement";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.DisplacementTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Displacement = ReferenceUtility.MakeThisReference( material, sample, "R" );
			}

			//AmbientOcclusion
			if( !string.IsNullOrEmpty( data.AmbientOcclusionTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "AmbientOcclusion";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.AmbientOcclusionTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.AmbientOcclusion = ReferenceUtility.MakeThisReference( material, sample, "R" );
			}

			//Emissive
			if( !string.IsNullOrEmpty( data.EmissiveTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Emissive";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.EmissiveTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Emissive = ReferenceUtility.MakeThisReference( material, sample, "RGBA" );
			}

			//Opacity
			if( !string.IsNullOrEmpty( data.OpacityTexture ) )
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();
				node.Name = "Node Texture Sample " + "Opacity";
				node.Position = position;
				position.Y += step;

				var sample = node.CreateComponent<Component_ShaderTextureSample>();
				sample.Name = ComponentUtility.GetNewObjectUniqueName( sample );
				sample.Texture = new Reference<Component_Image>( null, data.OpacityTexture );

				node.ControlledObject = ReferenceUtility.MakeThisReference( node, sample );

				material.Opacity = ReferenceUtility.MakeThisReference( material, sample, "R" );
			}

			material.Enabled = true;

			return material;
		}
	}
}
