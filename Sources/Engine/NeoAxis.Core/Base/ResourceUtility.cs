// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with project base resources.
	/// </summary>
	public static class ResourceUtility
	{
		static Component_Image blackTexture2D;
		public static Component_Image BlackTexture2D
		{
			get
			{
				if( blackTexture2D == null )
				{
					blackTexture2D = ResourceManager.LoadResource<Component_Image>( @"Base\Images\Black.jpg" );
					if( blackTexture2D == null )
						Log.Fatal( "ResourceUtility: BlackTexture2D: blackTexture2D == null." );
				}
				return blackTexture2D;
			}
		}

		static Component_Image whiteTexture2D;
		public static Component_Image WhiteTexture2D
		{
			get
			{
				if( whiteTexture2D == null )
				{
					whiteTexture2D = ResourceManager.LoadResource<Component_Image>( @"Base\Images\White.jpg" );
					if( whiteTexture2D == null )
						Log.Fatal( "ResourceUtility: WhiteTexture2D: whiteTexture2D == null." );
				}
				return whiteTexture2D;
			}
		}

		static Component_Image grayTexture2D;
		public static Component_Image GrayTexture2D
		{
			get
			{
				if( grayTexture2D == null )
				{
					grayTexture2D = ResourceManager.LoadResource<Component_Image>( @"Base\Images\Gray.jpg" );
					if( grayTexture2D == null )
						Log.Fatal( "ResourceUtility: GrayTexture2D: grayTexture2D == null." );
				}
				return grayTexture2D;
			}
		}

		static Component_Image blackTextureCube;
		public static Component_Image BlackTextureCube
		{
			get
			{
				if( blackTextureCube == null )
				{
					blackTextureCube = ResourceManager.LoadResource<Component_Image>( @"Base\Images\BlackCube.image" );
					if( blackTextureCube == null )
						Log.Fatal( "ResourceUtility: BlackTextureCube: blackTextureCube == null." );
				}
				return blackTextureCube;
			}
		}

		static Component_Image whiteTextureCube;
		public static Component_Image WhiteTextureCube
		{
			get
			{
				if( whiteTextureCube == null )
				{
					whiteTextureCube = ResourceManager.LoadResource<Component_Image>( @"Base\Images\WhiteCube.image" );
					if( whiteTextureCube == null )
						Log.Fatal( "ResourceUtility: WhiteTextureCube: whiteTextureCube == null." );
				}
				return whiteTextureCube;
			}
		}

		static Component_Image grayTextureCube;
		public static Component_Image GrayTextureCube
		{
			get
			{
				if( grayTextureCube == null )
				{
					grayTextureCube = ResourceManager.LoadResource<Component_Image>( @"Base\Images\GrayCube.image" );
					if( grayTextureCube == null )
						Log.Fatal( "ResourceUtility: GrayTextureCube: grayTextureCube == null." );
				}
				return grayTextureCube;
			}
		}

		static Component_Material materialNull;
		public static Component_Material MaterialNull
		{
			get
			{
				if( materialNull == null )
				{
					materialNull = ResourceManager.LoadResource<Component_Material>( @"Base\Materials\Null.material" );
					if( materialNull == null )
						Log.Fatal( "ResourceUtility: MaterialStandardNull: materialStandardNull == null." );
				}
				return materialNull;
			}
		}

		static Component_Material materialInvalid;
		public static Component_Material MaterialInvalid
		{
			get
			{
				if( materialInvalid == null )
				{
					materialInvalid = ResourceManager.LoadResource<Component_Material>( @"Base\Materials\Invalid.material" );
					if( materialInvalid == null )
						Log.Fatal( "ResourceUtility: MaterialStandardInvalid: materialStandardInvalid == null." );
				}
				return materialInvalid;
			}
		}

		static Component_Mesh meshInvalid;
		public static Component_Mesh MeshInvalid
		{
			get
			{
				if( meshInvalid == null )
				{
					meshInvalid = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
					var geometry = meshInvalid.CreateComponent<Component_MeshGeometry_Box>();
					geometry.Material = MaterialInvalid;
					meshInvalid.Enabled = true;
				}
				return meshInvalid;
			}
		}

		//public static Component_Image EnvironmentDefaultTexture
		//{
		//	get
		//	{
		//		var texture = ResourceManager.LoadResource<Component_Image>( @"Base\Images\EnvironmentDefault.image" );
		//		if( texture == null )
		//			Log.Fatal( "ResourceUtility: EnvironmentDefaultTexture: texture == null." );
		//		return texture;
		//	}
		//}

		//public static GpuTexture GetTextureCompiledData( Component_Texture texture )
		//{
		//	if( texture != null )
		//		return texture.Result;
		//	return null;
		//}

		//public static Component_Material.CompiledData GetMaterialCompiledData( Component_Material material )
		//{
		//	if( material != null )
		//		return material.Result;
		//	return null;
		//}

		//public static Component_Mesh.CompiledData GetMeshCompiledData( Component_Mesh mesh )
		//{
		//	if( mesh != null )
		//		return mesh.Result;
		//	return null;
		//}

		//public static ResourceSelectionMode GetSelectionModeByPropertyAttributes( Metadata.Property property )
		//{
		//	if( property.GetCustomAttributes( typeof( SelectFileAttribute ), true ).Length != 0 )
		//		return ResourceSelectionMode.File;
		//	if( property.GetCustomAttributes( typeof( SelectTypeAttribute ), true ).Length != 0 )
		//		return ResourceSelectionMode.Type;
		//	if( property.GetCustomAttributes( typeof( SelectMemberAttribute ), true ).Length != 0 )
		//		return ResourceSelectionMode.Member;

		//	return ResourceSelectionMode.None;
		//}
	}
}
