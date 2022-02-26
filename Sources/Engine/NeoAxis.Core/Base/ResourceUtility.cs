// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A helper class for working with base resources of the project.
	/// </summary>
	public static class ResourceUtility
	{
		static ImageComponent blackTexture2D;
		public static ImageComponent BlackTexture2D
		{
			get
			{
				if( blackTexture2D == null )
				{
					blackTexture2D = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\Black.jpg" );
					if( blackTexture2D == null )
						Log.Fatal( "ResourceUtility: BlackTexture2D: blackTexture2D == null." );
				}
				return blackTexture2D;
			}
		}

		static ImageComponent whiteTexture2D;
		public static ImageComponent WhiteTexture2D
		{
			get
			{
				if( whiteTexture2D == null )
				{
					whiteTexture2D = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\White.jpg" );
					if( whiteTexture2D == null )
						Log.Fatal( "ResourceUtility: WhiteTexture2D: whiteTexture2D == null." );
				}
				return whiteTexture2D;
			}
		}

		static ImageComponent grayTexture2D;
		public static ImageComponent GrayTexture2D
		{
			get
			{
				if( grayTexture2D == null )
				{
					grayTexture2D = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\Gray.jpg" );
					if( grayTexture2D == null )
						Log.Fatal( "ResourceUtility: GrayTexture2D: grayTexture2D == null." );
				}
				return grayTexture2D;
			}
		}

		static ImageComponent blackTextureCube;
		public static ImageComponent BlackTextureCube
		{
			get
			{
				if( blackTextureCube == null )
				{
					blackTextureCube = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\BlackCube.image" );
					if( blackTextureCube == null )
						Log.Fatal( "ResourceUtility: BlackTextureCube: blackTextureCube == null." );
				}
				return blackTextureCube;
			}
		}

		static ImageComponent whiteTextureCube;
		public static ImageComponent WhiteTextureCube
		{
			get
			{
				if( whiteTextureCube == null )
				{
					whiteTextureCube = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\WhiteCube.image" );
					if( whiteTextureCube == null )
						Log.Fatal( "ResourceUtility: WhiteTextureCube: whiteTextureCube == null." );
				}
				return whiteTextureCube;
			}
		}

		static ImageComponent grayTextureCube;
		public static ImageComponent GrayTextureCube
		{
			get
			{
				if( grayTextureCube == null )
				{
					grayTextureCube = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\GrayCube.image" );
					if( grayTextureCube == null )
						Log.Fatal( "ResourceUtility: GrayTextureCube: grayTextureCube == null." );
				}
				return grayTextureCube;
			}
		}

		static Material materialNull;
		public static Material MaterialNull
		{
			get
			{
				if( materialNull == null )
				{
					materialNull = ResourceManager.LoadResource<Material>( @"Base\Materials\Null.material" );
					if( materialNull == null )
						Log.Fatal( "ResourceUtility: MaterialStandardNull: materialStandardNull == null." );
				}
				return materialNull;
			}
		}

		static Material materialInvalid;
		public static Material MaterialInvalid
		{
			get
			{
				if( materialInvalid == null )
				{
					materialInvalid = ResourceManager.LoadResource<Material>( @"Base\Materials\Invalid.material" );
					if( materialInvalid == null )
						Log.Fatal( "ResourceUtility: MaterialStandardInvalid: materialStandardInvalid == null." );
				}
				return materialInvalid;
			}
		}

		static Mesh meshInvalid;
		public static Mesh MeshInvalid
		{
			get
			{
				if( meshInvalid == null )
				{
					meshInvalid = ComponentUtility.CreateComponent<Mesh>( null, true, false );
					var geometry = meshInvalid.CreateComponent<MeshGeometry_Box>();
					geometry.Material = MaterialInvalid;
					meshInvalid.Enabled = true;
				}
				return meshInvalid;
			}
		}

		//public static Image EnvironmentDefaultTexture
		//{
		//	get
		//	{
		//		var texture = ResourceManager.LoadResource<Image>( @"Base\Images\EnvironmentDefault.image" );
		//		if( texture == null )
		//			Log.Fatal( "ResourceUtility: EnvironmentDefaultTexture: texture == null." );
		//		return texture;
		//	}
		//}

		//public static GpuTexture GetTextureCompiledData( Texture texture )
		//{
		//	if( texture != null )
		//		return texture.Result;
		//	return null;
		//}

		//public static Material.CompiledData GetMaterialCompiledData( Material material )
		//{
		//	if( material != null )
		//		return material.Result;
		//	return null;
		//}

		//public static Mesh.CompiledData GetMeshCompiledData( Mesh mesh )
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
