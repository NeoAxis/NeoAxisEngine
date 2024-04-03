// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Editor;
using Internal.SharpBgfx;

namespace NeoAxis.Editor
{
	/// <summary>
	/// A set of settings for <see cref="Material"/> creation in the editor.
	/// </summary>
	public class MaterialNewObjectSettings : NewObjectSettings
	{
		Material.NewMaterialData newMaterialData;
		double newMaterialDataLastUpdate;

		[DefaultValue( true )]
		[Category( "Options" )]
		[DisplayName( "Shader graph" )]
		public bool ShaderGraph { get; set; } = true;

		/// <summary>
		/// The mode for automatic adjustment textures of a material. The texture files must be in the folder of a material. The mode does not always work, for example, when there are several files suitable for one channel texture.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Configure textures from the folder" )]
		public bool ConfigureTexturesFromFolder { get; set; } = true;

		/// <summary>
		/// The mode for automatic adjustment textures of a material. The texture files must be in the folder of a material. The mode does not always work, for example, when there are several files suitable for one channel texture.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Configure textures from the folder" )]
		public bool ConfigureTexturesFromFolderDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool BaseColor { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Base Color" )]
		public bool BaseColorDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Metallic { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Metallic" )]
		public bool MetallicDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Roughness { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Roughness" )]
		public bool RoughnessDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool AmbientOcclusion { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Ambient Occlusion" )]
		public bool AmbientOcclusionDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Emissive { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Emissive" )]
		public bool EmissiveDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Opacity { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Opacity" )]
		public bool OpacityDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Normal { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Normal" )]
		public bool NormalDisabled { get; } = false;

		[DefaultValue( true )]
		[Category( "Automatic Tuning" )]
		public bool Displacement { get; set; } = true;
		[DefaultValue( false )]
		[Category( "Automatic Tuning" )]
		[DisplayName( "Displacement" )]
		public bool DisplacementDisabled { get; } = false;

		//

		public override bool Init( NewObjectWindow window )
		{
			if( !base.Init( window ) )
				return false;

			UpdateNewMaterialData();

			return true;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//update new material data
			if( EngineApp.EngineTime > newMaterialDataLastUpdate + 1.0 )
				UpdateNewMaterialData();

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( ConfigureTexturesFromFolder ):
					if( !ShaderGraph || newMaterialData == null )
						skip = true;
					break;

				case nameof( ConfigureTexturesFromFolderDisabled ):
					if( !ShaderGraph || newMaterialData != null )
						skip = true;
					break;

				case nameof( BaseColor ):
				case nameof( Metallic ):
				case nameof( Roughness ):
				case nameof( Normal ):
				case nameof( Displacement ):
				case nameof( AmbientOcclusion ):
				case nameof( Emissive ):
				case nameof( Opacity ):
					if( !ShaderGraph || !ConfigureTexturesFromFolder || newMaterialData == null )
						skip = true;
					else if( newMaterialData != null && string.IsNullOrEmpty( newMaterialData.GetTextureValueByName( member.Name ) ) )
						skip = true;
					break;

				case nameof( BaseColorDisabled ):
				case nameof( MetallicDisabled ):
				case nameof( RoughnessDisabled ):
				case nameof( NormalDisabled ):
				case nameof( DisplacementDisabled ):
				case nameof( AmbientOcclusionDisabled ):
				case nameof( EmissiveDisabled ):
				case nameof( OpacityDisabled ):
					if( !ShaderGraph || !ConfigureTexturesFromFolder || newMaterialData == null )
						skip = true;
					else if( newMaterialData != null && !string.IsNullOrEmpty( newMaterialData.GetTextureValueByName( member.Name.Replace( "Disabled", "" ) ) ) )
						skip = true;
					break;
				}
			}
		}

		string DetectTextureTypeNameByFileName( string name )
		{
			if( name.Contains( "_albedo" ) || name.Contains( "_diffuse" ) || name.Contains( "_diff_" ) || name.Contains( "_color" ) || name.Contains( "_base_color" ) || name.Contains( "_basecolor" ) )
				return "BaseColor";

			if( name.Contains( "_metallic" ) || name.Contains( "_metalness" ) )
				return "Metallic";

			if( name.Contains( "_roughness" ) || ( name.Contains( "_rough_" ) && !name.Contains( "_rough_ao_" ) ) )
				return "Roughness";

			if( name.Contains( "_normal" ) || name.Contains( "_nor_" ) )
				return "Normal";

			if( name.Contains( "_displacement" ) || name.Contains( "_disp_" ) || name.Contains( "_height" ) )
				return "Displacement";

			if( name.Contains( "_ambientocclusion" ) || ( !name.Contains( "_rough_ao_" ) && name.Contains( "_ao" ) ) )
				return "AmbientOcclusion";

			if( name.Contains( "_emissive" ) || name.Contains( "_emission" ) )
				return "Emissive";

			if( name.Contains( "_opacity" ) )
				return "Opacity";

			return "";
		}

		void UpdateNewMaterialData()
		{
			Material.NewMaterialData data = null;

			try
			{
				var realFileName = Window.GetFileCreationRealFileName();
				if( !string.IsNullOrEmpty( realFileName ) )
				{
					var virtualDirectory = VirtualPathUtility.GetVirtualPathByReal( PathUtility.GetDirectoryName( realFileName ) );
					var paths = VirtualDirectory.GetFiles( virtualDirectory );

					var imageType = ResourceManager.GetTypeByName( "Image" );

					var extensions = new ESet<string>();
					foreach( var ext in imageType.FileExtensions )
						extensions.AddWithCheckAlreadyContained( ext );

					foreach( var path in paths )
					{
						var lowerName = Path.GetFileName( path ).ToLower();
						var extension = Path.GetExtension( lowerName ).Replace( ".", "" );
						if( extensions.Contains( extension ) )
						{
							var textureTypeName = DetectTextureTypeNameByFileName( lowerName );
							if( !string.IsNullOrEmpty( textureTypeName ) )
							{
								if( data == null )
									data = new Material.NewMaterialData();

								//check already exists
								if( !string.IsNullOrEmpty( data.GetTextureValueByName( textureTypeName ) ) )
								{
									data = null;
									goto end;
								}

								data.SetTextureValueByName( textureTypeName, path );
							}
						}
					}

end:;

					if( data != null && data.GetTextureCount() == 0 )
						data = null;
				}
			}
			catch { }

			newMaterialDataLastUpdate = EngineApp.EngineTime;
			newMaterialData = data;
		}

		public override bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			var newObject2 = (Material)context.newObject;

			if( ShaderGraph )
			{
				UpdateNewMaterialData();

				Material.NewMaterialData data = null;
				if( ConfigureTexturesFromFolder && newMaterialData != null )
				{
					data = newMaterialData.Clone();

					if( !BaseColor )
						data.SetTextureValueByName( "BaseColor", "" );
					if( !Metallic )
						data.SetTextureValueByName( "Metallic", "" );
					if( !Roughness )
						data.SetTextureValueByName( "Roughness", "" );
					if( !Normal )
						data.SetTextureValueByName( "Normal", "" );
					if( !Displacement )
						data.SetTextureValueByName( "Displacement", "" );
					if( !AmbientOcclusion )
						data.SetTextureValueByName( "AmbientOcclusion", "" );
					if( !Emissive )
						data.SetTextureValueByName( "Emissive", "" );
					if( !Opacity )
						data.SetTextureValueByName( "Opacity", "" );
				}

				newObject2.NewObjectCreateShaderGraph( data );
			}

			return base.Creation( context );
		}
	}
}
#endif