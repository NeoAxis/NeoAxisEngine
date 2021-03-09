// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a layer for the terrain or for the mesh. It defined by a material and a mask.
	/// </summary>
	public class Component_PaintLayer : Component
	{
		Component_Image createdMaskImage;
		bool createdMaskImageNeedUpdate;
		long uniqueMaskDataCounter;

		int cachedSqrt;
		int cachedSqrtValue;

		/////////////////////////////////////////

		/// <summary>
		/// The material of the layer.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); ShouldParentUpdate(); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Component_PaintLayer> MaterialChanged;
		ReferenceField<Component_Material> _material = null;

		//!!!!
		/// <summary>
		/// The surface of the layer.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( null )]
		public Reference<Component_Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set { if( _surface.BeginSet( ref value ) ) { try { SurfaceChanged?.Invoke( this ); ShouldParentUpdate(); } finally { _surface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<Component_PaintLayer> SurfaceChanged;
		ReferenceField<Component_Surface> _surface = null;

		/// <summary>
		/// The editable mask of the layer.
		/// </summary>
		//[Serialize( false )]
		[DefaultValue( null )]
		public Reference<byte[]> Mask
		{
			get { if( _mask.BeginGet() ) Mask = _mask.Get( this ); return _mask.value; }
			set { if( _mask.BeginSet( ref value ) ) { try { MaskChanged?.Invoke( this ); DestroyCreatedMaskImage(); ShouldParentUpdate(); } finally { _mask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mask"/> property value changes.</summary>
		public event Action<Component_PaintLayer> MaskChanged;
		ReferenceField<byte[]> _mask = null;

		/// <summary>
		/// The mask in a form of texture of the layer.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Image> MaskImage
		{
			get { if( _maskImage.BeginGet() ) MaskImage = _maskImage.Get( this ); return _maskImage.value; }
			set { if( _maskImage.BeginSet( ref value ) ) { try { MaskImageChanged?.Invoke( this ); ShouldParentUpdate(); } finally { _maskImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaskImage"/> property value changes.</summary>
		public event Action<Component_PaintLayer> MaskImageChanged;
		ReferenceField<Component_Image> _maskImage = null;

		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); ShouldParentUpdate(); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_PaintLayer> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		public enum BlendModeEnum
		{
			Auto,
			Masked,
			Transparent,
		}

		/// <summary>
		/// The technique of rendering of the layer.
		/// </summary>
		[DefaultValue( BlendModeEnum.Auto )]
		public Reference<BlendModeEnum> BlendMode
		{
			get { if( _blendMode.BeginGet() ) BlendMode = _blendMode.Get( this ); return _blendMode.value; }
			set { if( _blendMode.BeginSet( ref value ) ) { try { BlendModeChanged?.Invoke( this ); ShouldParentUpdate(); } finally { _blendMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlendMode"/> property value changes.</summary>
		public event Action<Component_PaintLayer> BlendModeChanged;
		ReferenceField<BlendModeEnum> _blendMode = BlendModeEnum.Auto;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Material ):
					if( Surface.Value != null )
						skip = true;
					break;

				case nameof( Surface ):
					if( Material.Value != null )
						skip = true;
					break;

				case nameof( Mask ):
					if( MaskImage.Value != null )
						skip = true;
					break;

				case nameof( MaskImage ):
					if( Mask.Value != null && Mask.Value.Length != 0 )
						skip = true;
					break;
				}
			}
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//!!!!Mask хранить в файле

			return true;
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			return true;
		}

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			ShouldParentUpdate();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			ShouldParentUpdate( oldParent );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( !EnabledInHierarchy )
				DestroyCreatedMaskImage();

			ShouldParentUpdate();
		}

		public void DestroyCreatedMaskImage()
		{
			createdMaskImage?.Dispose();
			createdMaskImage = null;
		}

		public bool IsDataAvailable()
		{
			if( MaskImage.Value != null )
				return true;
			else if( Mask.Value != null && Mask.Value.Length != 0 )
				return true;
			return false;
		}

		public Component_Image GetImage( out long uniqueMaskDataCounter )
		{
			uniqueMaskDataCounter = 0;

			if( MaskImage.Value != null )
				return MaskImage;
			else if( Mask.Value != null && Mask.Value.Length != 0 )
			{
				if( EnabledInHierarchy )
				{
					if( createdMaskImage == null )
					{
						int textureSize = (int)Math.Sqrt( Mask.Value.Length );

						//need set ShowInEditor = false before AddComponent
						var texture = ComponentUtility.CreateComponent<Component_Image>( null, false, false );
						texture.DisplayInEditor = false;
						AddComponent( texture, -1 );
						//var texture = CreateComponent<Component_Image>( enabled: false );

						texture.SaveSupport = false;
						texture.CloneSupport = false;

						//!!!!
						bool mipmaps = false;

						texture.CreateType = Component_Image.TypeEnum._2D;
						texture.CreateSize = new Vector2I( textureSize, textureSize );
						texture.CreateMipmaps = mipmaps;
						texture.CreateFormat = PixelFormat.L8;

						var usage = Component_Image.Usages.WriteOnly;
						if( mipmaps )
							usage |= Component_Image.Usages.AutoMipmaps;
						texture.CreateUsage = usage;

						texture.Enabled = true;

						createdMaskImage = texture;
						createdMaskImageNeedUpdate = true;
					}

					//update data
					if( createdMaskImageNeedUpdate )
					{
						GpuTexture gpuTexture = createdMaskImage.Result;
						if( gpuTexture != null )
						{
							var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, Mask.Value ) };
							gpuTexture.SetData( d );
						}

						createdMaskImageNeedUpdate = false;
					}

					uniqueMaskDataCounter = this.uniqueMaskDataCounter;
					return createdMaskImage;
				}
			}
			return null;
		}

		int GetSqrt( int value )
		{
			if( cachedSqrt != value )
			{
				cachedSqrt = value;
				cachedSqrtValue = (int)Math.Sqrt( value );
			}
			return cachedSqrtValue;
		}

		public float GetMaskValue( Vector2I position )//public byte GetMaskValue( Vector2I position )
		{
			//!!!!slowly. когда не проверять

			var mask = Mask.Value;
			if( mask != null )
			{
				int size = GetSqrt( Mask.Value.Length );
				if( size > 0 )
				{
					var position2 = new Vector2I( MathEx.Clamp( position.X, 0, size - 1 ), MathEx.Clamp( position.Y, 0, size - 1 ) );
					float v = mask[ ( size - 1 - position2.Y ) * size + position2.X ];
					return v * ( 1.0f / 255.0f );
				}
			}

			return 0;
		}

		public void SetMaskValue( Vector2I position, float value )
		{
			//!!!!slowly. когда не проверять

			var mask = Mask.Value;
			if( mask != null )
			{
				int size = GetSqrt( Mask.Value.Length );

				if( size > 0 && position.X >= 0 && position.X < size && position.Y >= 0 && position.Y < size )
				{
					float v = value * 255.0f + .5f;
					var i = (int)v;
					mask[ ( size - 1 - position.Y ) * size + position.X ] = (byte)MathEx.Clamp( i, 0, 255 );

					//!!!!не всю обновлять
					createdMaskImageNeedUpdate = true;

					unchecked
					{
						uniqueMaskDataCounter++;
					}
				}
			}
		}

		static int GetMaskSize( byte[] mask )
		{
			return (int)Math.Sqrt( mask.Length );
		}

		static byte[] ResizeMaskBuffersDivide2( byte[] source )
		{
			var size = GetMaskSize( source );
			int newSize = size / 2;

			byte[] newBuffer = new byte[ newSize * newSize ];
			for( int y = 0; y < newSize; y++ )
				for( int x = 0; x < newSize; x++ )
					newBuffer[ y * newSize + x ] = source[ y * 2 * size + x * 2 ];

			return newBuffer;
		}

		static byte[] ResizeMaskBuffersMultiple2( byte[] source )
		{
			var size = GetMaskSize( source );
			int newSize = size * 2;

			byte[] newBuffer = new byte[ newSize * newSize ];
			for( int y = 0; y < newSize; y++ )
				for( int x = 0; x < newSize; x++ )
					newBuffer[ y * newSize + x ] = source[ y / 2 * size + x / 2 ];

			//smooth
			byte[] newBuffer2 = new byte[ newSize * newSize ];

			for( int y = 0; y < newSize; y++ )
			{
				for( int x = 0; x < newSize; x++ )
				{
					int value = 0;

					Vector2I index;
					index = new Vector2I( Math.Max( 0, x - 1 ), y );
					value += newBuffer[ index.Y * newSize + index.X ];
					index = new Vector2I( Math.Min( newSize - 1, x + 1 ), y );
					value += newBuffer[ index.Y * newSize + index.X ];
					index = new Vector2I( x, Math.Max( 0, y - 1 ) );
					value += newBuffer[ index.Y * newSize + index.X ];
					index = new Vector2I( x, Math.Min( newSize - 1, y + 1 ) );
					value += newBuffer[ index.Y * newSize + index.X ];
					value /= 4;

					newBuffer2[ y * newSize + x ] = (byte)value;
				}
			}

			return newBuffer2;
		}

		public static byte[] ResizeMask( byte[] mask, int newSize )
		{
			//!!!!если маска нестандартного размера

			var current = mask;

			int size = GetMaskSize( current );
			while( newSize > size )
			{
				current = ResizeMaskBuffersMultiple2( current );
				size = GetMaskSize( current );
			}
			while( newSize < size )
			{
				current = ResizeMaskBuffersDivide2( current );
				size = GetMaskSize( current );
			}

			return current;
		}

		void ShouldParentUpdate( Component parent = null )
		{
			if( parent == null )
				parent = Parent;

			var meshInSpace = parent as Component_MeshInSpace;
			if( meshInSpace != null )
				meshInSpace.PaintLayersNeedUpdate();

			var mesh = parent as Component_Mesh;
			if( mesh != null )
				mesh.ShouldRecompile = true;
		}
	}
}
