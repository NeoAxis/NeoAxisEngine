// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// A component to configure texture sampling for material or screen effect. It's used in the visual graph editor.
	/// </summary>
	public class ShaderTextureSample : Component, IFlowGraphRepresentationData
	{
		[DefaultValue( TextureTypeEnum.Usual )]
		[FlowGraphBrowsable( false )]
		public Reference<TextureTypeEnum> TextureType
		{
			get { if( _textureType.BeginGet() ) TextureType = _textureType.Get( this ); return _textureType.value; }
			set { if( _textureType.BeginSet( ref value ) ) { try { TextureTypeChanged?.Invoke( this ); } finally { _textureType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextureType"/> property value changes.</summary>
		public event Action<ShaderTextureSample> TextureTypeChanged;
		ReferenceField<TextureTypeEnum> _textureType = TextureTypeEnum.Usual;

		[Serialize]
		public Reference<ImageComponent> Texture
		{
			get { if( _texture.BeginGet() ) Texture = _texture.Get( this ); return _texture.value; }
			set { if( _texture.BeginSet( ref value ) ) { try { TextureChanged?.Invoke( this ); } finally { _texture.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Texture"/> property value changes.</summary>
		public event Action<ShaderTextureSample> TextureChanged;
		ReferenceField<ImageComponent> _texture;

		/// <summary>
		/// Whether to use a special technique to remove texture tiling.
		/// </summary>
		[DefaultValue( false )]
		[FlowGraphBrowsable( false )]
		public Reference<bool> RemoveTiling
		{
			get { if( _removeTiling.BeginGet() ) RemoveTiling = _removeTiling.Get( this ); return _removeTiling.value; }
			set { if( _removeTiling.BeginSet( ref value ) ) { try { RemoveTilingChanged?.Invoke( this ); } finally { _removeTiling.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RemoveTiling"/> property value changes.</summary>
		public event Action<ShaderTextureSample> RemoveTilingChanged;
		ReferenceField<bool> _removeTiling = false;


		//!!!!sampler state


		[Serialize]
		[DisplayName( "Location" )]
		public Reference<Vector2> Location2
		{
			get { if( _location2.BeginGet() ) Location2 = _location2.Get( this ); return _location2.value; }
			set { if( _location2.BeginSet( ref value ) ) { try { Location2Changed?.Invoke( this ); } finally { _location2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Location2"/> property value changes.</summary>
		public event Action<ShaderTextureSample> Location2Changed;
		ReferenceField<Vector2> _location2;

		[Serialize]
		[DisplayName( "Location" )]
		public Reference<Vector3> Location3
		{
			get { if( _location3.BeginGet() ) Location3 = _location3.Get( this ); return _location3.value; }
			set { if( _location3.BeginSet( ref value ) ) { try { Location3Changed?.Invoke( this ); } finally { _location3.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Location3"/> property value changes.</summary>
		public event Action<ShaderTextureSample> Location3Changed;
		ReferenceField<Vector3> _location3;


		public Vector4 RGBA
		{
			get { return Vector4.Zero; }
		}

		//!!!!показывать как 'L' для L8, L16 форматов
		[ShaderGenerationPropertyPostfix( "r" )]
		public double R
		{
			get { return 0; }
		}

		[ShaderGenerationPropertyPostfix( "g" )]
		public double G
		{
			get { return 0; }
		}

		[ShaderGenerationPropertyPostfix( "b" )]
		public double B
		{
			get { return 0; }
		}

		[ShaderGenerationPropertyPostfix( "a" )]
		public double A
		{
			get { return 0; }
		}

		/////////////////////////////////////////

		public enum TextureTypeEnum
		{
			Usual,
			Mask,
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Texture ):
					if( TextureType.Value == TextureTypeEnum.Mask )
						skip = true;
					break;

				case nameof( Location2 ):
					if( TextureType.Value != TextureTypeEnum.Mask )
					{
						var t = Texture.Value;
						if( t == null || t.Result == null || t.Result.TextureType != ImageComponent.TypeEnum._2D )
						{
							skip = true;
							return;
						}
					}
					break;

				case nameof( Location3 ):
					{
						var t = Texture.Value;
						if( t == null || t.Result == null ||
							( t.Result.TextureType != ImageComponent.TypeEnum.Cube && t.Result.TextureType != ImageComponent.TypeEnum._3D ) )
						{
							skip = true;
							return;
						}
					}
					break;

				case nameof( RGBA ):
				case nameof( G ):
				case nameof( B ):
				case nameof( A ):
					{
						var result = Texture.Value?.Result;
						if( result != null && ( result.ResultFormat == PixelFormat.L8 || result.ResultFormat == PixelFormat.L16 ) )
						//if( t == null || t.Result == null || ( t.Result.ResultFormat == PixelFormat.L8 || t.Result.ResultFormat == PixelFormat.L16 ) )
						{
							skip = true;
							return;
						}
					}
					break;
				}
			}
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeTitle = "Texture Sample";

			if( Texture.ReferenceSpecified )
				data.NodeImage = Texture.Value;
			else
			{
				//!!!!так?
				var res = ResourceManager.LoadResource( @"Base\Images\Grid256.png", true );
				if( res != null )
					data.NodeImage = res.ResultComponent as ImageComponent;
			}

			data.NodeHeight = Math.Max( data.NodeHeight, 1 + 7 );
			//data.nodeMinHeight = 1 + 7;
		}

		//[Browsable( false )]
		//public string FlowchartNodeTitle
		//{
		//	get { return "Texture Sample"; }
		//}

		//[Browsable( false )]
		//public Texture FlowchartNodeRenderTexture
		//{
		//	get
		//	{
		//		if( !Texture.ReferenceSpecified )
		//		{
		//			//!!!!так?
		//			var res = ResourceManager.LoadResource( @"Base\Textures\Grid256.png", true );
		//			if( res != null )
		//				return res.ResultComponent as Texture;
		//		}

		//		return Texture.Value;
		//	}
		//}

		//[Browsable( false )]
		//public FlowchartNodeContentType FlowchartNodeContentType
		//{
		//	get { return FlowchartNodeContentType.Default; }
		//}
	}
}
