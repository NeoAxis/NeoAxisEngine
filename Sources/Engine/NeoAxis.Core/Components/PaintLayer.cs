// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Specifies a layer for the mesh or for the terrain.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.PaintLayerSettingsCell" )]
#endif
	public class PaintLayer : Component
	{
		ImageComponent createdMaskImage;
		bool createdMaskImageNeedUpdate;
		long uniqueMaskDataCounter;

		int cachedSqrt;
		int cachedSqrtValue;

		/////////////////////////////////////////

		//!!!!impl
		/// <summary>
		/// The format of the mask data.
		/// </summary>
		[Browsable( false )]//!!!!
		[DefaultValue( MaskFormatEnum.Image )]
		public Reference<MaskFormatEnum> MaskFormat
		{
			get { if( _maskFormat.BeginGet() ) MaskFormat = _maskFormat.Get( this ); return _maskFormat.value; }
			set { if( _maskFormat.BeginSet( this, ref value ) ) { try { MaskFormatChanged?.Invoke( this ); DestroyCreatedMaskImage(); NeedUpdateParent(); } finally { _maskFormat.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaskFormat"/> property value changes.</summary>
		public event Action<PaintLayer> MaskFormatChanged;
		ReferenceField<MaskFormatEnum> _maskFormat = MaskFormatEnum.Image;

		/// <summary>
		/// The editable mask of the layer.
		/// </summary>
		//[Serialize( false )]
		[DefaultValue( null )]
		public Reference<byte[]> Mask
		{
			get { if( _mask.BeginGet() ) Mask = _mask.Get( this ); return _mask.value; }
			set { if( _mask.BeginSet( this, ref value ) ) { try { MaskChanged?.Invoke( this ); DestroyCreatedMaskImage(); NeedUpdateParent(); } finally { _mask.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mask"/> property value changes.</summary>
		public event Action<PaintLayer> MaskChanged;
		ReferenceField<byte[]> _mask = null;

		/// <summary>
		/// The mask in a form of texture.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ImageComponent> MaskImage
		{
			get { if( _maskImage.BeginGet() ) MaskImage = _maskImage.Get( this ); return _maskImage.value; }
			set { if( _maskImage.BeginSet( this, ref value ) ) { try { MaskImageChanged?.Invoke( this ); NeedUpdateParent(); } finally { _maskImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaskImage"/> property value changes.</summary>
		public event Action<PaintLayer> MaskImageChanged;
		ReferenceField<ImageComponent> _maskImage = null;

		/// <summary>
		/// The material of the layer.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( this, ref value ) ) { try { MaterialChanged?.Invoke( this ); NeedUpdateParent(); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<PaintLayer> MaterialChanged;
		ReferenceField<Material> _material = null;

		/// <summary>
		/// The base color and opacity multiplier of the material.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> MaterialColor
		{
			get { if( _materialColor.BeginGet() ) MaterialColor = _materialColor.Get( this ); return _materialColor.value; }
			set { if( _materialColor.BeginSet( this, ref value ) ) { try { MaterialColorChanged?.Invoke( this ); NeedUpdateParent(); } finally { _materialColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialColor"/> property value changes.</summary>
		public event Action<PaintLayer> MaterialColorChanged;
		ReferenceField<ColorValue> _materialColor = ColorValue.One;

		/// <summary>
		/// The multiplier of texture coordinates for texture sampling.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "UV Scale" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> UVScale
		{
			get { if( _uvScale.BeginGet() ) UVScale = _uvScale.Get( this ); return _uvScale.value; }
			set { if( _uvScale.BeginSet( this, ref value ) ) { try { UVScaleChanged?.Invoke( this ); NeedUpdateParent(); } finally { _uvScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVScale"/> property value changes.</summary>
		public event Action<PaintLayer> UVScaleChanged;
		ReferenceField<double> _uvScale = 1.0;

		/// <summary>
		/// The technique of rendering of the layer. NoBlend mode is useful when you need mix surface objects of several layers.
		/// </summary>
		[DefaultValue( BlendModeEnum.Auto )]
		public Reference<BlendModeEnum> BlendMode
		{
			get { if( _blendMode.BeginGet() ) BlendMode = _blendMode.Get( this ); return _blendMode.value; }
			set { if( _blendMode.BeginSet( this, ref value ) ) { try { BlendModeChanged?.Invoke( this ); NeedUpdateParent(); } finally { _blendMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlendMode"/> property value changes.</summary>
		public event Action<PaintLayer> BlendModeChanged;
		ReferenceField<BlendModeEnum> _blendMode = BlendModeEnum.Auto;

		/// <summary>
		/// The surface of the layer.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set { if( _surface.BeginSet( this, ref value ) ) { try { SurfaceChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceChanged;
		ReferenceField<Surface> _surface = null;

		/// <summary>
		/// Whether to create objects of the surface.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SurfaceObjects
		{
			get { if( _surfaceObjects.BeginGet() ) SurfaceObjects = _surfaceObjects.Get( this ); return _surfaceObjects.value; }
			set { if( _surfaceObjects.BeginSet( this, ref value ) ) { try { SurfaceObjectsChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjects"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsChanged;
		ReferenceField<bool> _surfaceObjects = true;

		/// <summary>
		/// The scale the distribution of surface objects.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> SurfaceObjectsDistribution
		{
			get { if( _surfaceObjectsDistribution.BeginGet() ) SurfaceObjectsDistribution = _surfaceObjectsDistribution.Get( this ); return _surfaceObjectsDistribution.value; }
			set { if( _surfaceObjectsDistribution.BeginSet( this, ref value ) ) { try { SurfaceObjectsDistributionChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsDistribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsDistribution"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsDistributionChanged;
		ReferenceField<double> _surfaceObjectsDistribution = 1.0;

		/// <summary>
		/// The scale of surface objects size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> SurfaceObjectsScale
		{
			get { if( _surfaceObjectsScale.BeginGet() ) SurfaceObjectsScale = _surfaceObjectsScale.Get( this ); return _surfaceObjectsScale.value; }
			set { if( _surfaceObjectsScale.BeginSet( this, ref value ) ) { try { SurfaceObjectsScaleChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsScale"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsScaleChanged;
		ReferenceField<double> _surfaceObjectsScale = 1.0;

		/// <summary>
		/// The base color and opacity multiplier of the surface objects.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> SurfaceObjectsColor
		{
			get { if( _surfaceObjectsColor.BeginGet() ) SurfaceObjectsColor = _surfaceObjectsColor.Get( this ); return _surfaceObjectsColor.value; }
			set { if( _surfaceObjectsColor.BeginSet( this, ref value ) ) { try { SurfaceObjectsColorChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsColor"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsColorChanged;
		ReferenceField<ColorValue> _surfaceObjectsColor = ColorValue.One;

		/// <summary>
		/// The factor of maximum visibility distance of surface objects. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 1.0 )]
		public Reference<double> SurfaceObjectsVisibilityDistanceFactor
		{
			get { if( _surfaceObjectsVisibilityDistanceFactor.BeginGet() ) SurfaceObjectsVisibilityDistanceFactor = _surfaceObjectsVisibilityDistanceFactor.Get( this ); return _surfaceObjectsVisibilityDistanceFactor.value; }
			set { if( _surfaceObjectsVisibilityDistanceFactor.BeginSet( this, ref value ) ) { try { SurfaceObjectsVisibilityDistanceFactorChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsVisibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsVisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsVisibilityDistanceFactorChanged;
		ReferenceField<double> _surfaceObjectsVisibilityDistanceFactor = 1.0;

		/// <summary>
		/// Whether to cast shadows on the other surfaces of surface objects.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> SurfaceObjectsCastShadows
		{
			get { if( _surfaceObjectsCastShadows.BeginGet() ) SurfaceObjectsCastShadows = _surfaceObjectsCastShadows.Get( this ); return _surfaceObjectsCastShadows.value; }
			set { if( _surfaceObjectsCastShadows.BeginSet( this, ref value ) ) { try { SurfaceObjectsCastShadowsChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsCastShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsCastShadows"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsCastShadowsChanged;
		ReferenceField<bool> _surfaceObjectsCastShadows = true;

		/// <summary>
		/// Whether to enable a collision detection. A collision definition of the mesh is used.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> SurfaceObjectsCollision
		{
			get { if( _surfaceObjectsCollision.BeginGet() ) SurfaceObjectsCollision = _surfaceObjectsCollision.Get( this ); return _surfaceObjectsCollision.value; }
			set { if( _surfaceObjectsCollision.BeginSet( this, ref value ) ) { try { SurfaceObjectsCollisionChanged?.Invoke( this ); NeedUpdateParent(); } finally { _surfaceObjectsCollision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SurfaceObjectsCollision"/> property value changes.</summary>
		public event Action<PaintLayer> SurfaceObjectsCollisionChanged;
		ReferenceField<bool> _surfaceObjectsCollision = false;


		//!!!!
		//LODScale
		//public Material replaceMaterial;
		//public bool receiveDecals;
		//public float motionBlurFactor;

		/////////////////////////////////////////

		//!!!!names
		public enum MaskFormatEnum
		{
			Image,
			Triangles,
		}

		/////////////////////////////////////////

		public enum BlendModeEnum
		{
			Auto,
			Masked,
			Transparent,
			NoBlend,
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
				case nameof( Mask ):
					if( MaskImage.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( MaskImage ):
					if( Mask.Value != null && Mask.Value.Length != 0 )
						skip = true;
					break;

				case nameof( SurfaceObjects ):
					if( !Surface.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( SurfaceObjectsDistribution ):
				case nameof( SurfaceObjectsScale ):
				case nameof( SurfaceObjectsColor ):
				case nameof( SurfaceObjectsVisibilityDistanceFactor ):
				case nameof( SurfaceObjectsCastShadows ):
				case nameof( SurfaceObjectsCollision ):
					if( !Surface.ReferenceOrValueSpecified || !SurfaceObjects )
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

			NeedUpdateParent();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			NeedUpdateParent( oldParent );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( !EnabledInHierarchy )
				DestroyCreatedMaskImage();

			NeedUpdateParent();
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

		public ImageComponent GetMaskImage( out long uniqueMaskDataCounter )
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
						//!!!!проверять если фигня всякая или формат маски TriangleData

						//!!!!

						//switch( MaskFormat.Value )
						//{
						//case MaskFormatEnum.Image:
						//	{
						//		//!!!!


						//	}
						//	break;

						//case MaskFormatEnum.Triangles:
						//	{
						//		//!!!!

						//		//zzzzz;
						//	}
						//	break;
						//}

						int textureSize = (int)Math.Sqrt( Mask.Value.Length );

						//need set DisplayInEditor = false before AddComponent
						var texture = ComponentUtility.CreateComponent<ImageComponent>( null, false, false );
						texture.NetworkMode = NetworkModeEnum.False;
						texture.DisplayInEditor = false;
						AddComponent( texture, -1 );
						//var texture = CreateComponent<ImageComponent>( enabled: false );

						texture.SaveSupport = false;
						texture.CloneSupport = false;

						bool mipmaps = false;

						texture.CreateType = ImageComponent.TypeEnum._2D;
						texture.CreateSize = new Vector2I( textureSize, textureSize );
						texture.CreateMipmaps = mipmaps;
						texture.CreateFormat = PixelFormat.L8;

						var usage = ImageComponent.Usages.WriteOnly;
						if( mipmaps )
							usage |= ImageComponent.Usages.AutoMipmaps;
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
							//!!!!

							var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( Mask.Value ) };
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

		void NeedUpdateParent( Component parent = null )
		{
			if( parent == null )
				parent = Parent;

			var meshInSpace = parent as MeshInSpace;
			if( meshInSpace != null )
				meshInSpace.PaintLayersNeedUpdate();

			var mesh = parent as Mesh;
			if( mesh != null )
				mesh.ShouldRecompile = true;
		}

		public Material GetMaterial()
		{
			var result = Surface.Value?.Material.Value;
			if( result == null )
				result = Material.Value;
			return result;

			//var material = Material;
			//if( material.ReferenceSpecified || material.Value != null )
			//	return material;
			//else
			//	return Surface.Value?.Material.Value;
		}

		public static bool LoadMask( string realFileName, out byte[] mask, out string error )
		{
			mask = null;
			error = "";

			if( !ImageUtility.LoadFromRealFile( realFileName, out var data, out var size, out _, out var format, out _, out _, out error ) )
				return false;

			if( size.X != size.Y )
			{
				error = "Invalid mask format. The image must be square.";
				return false;
			}

			if( format == PixelFormat.L8 )
			{
				mask = data;
			}
			else if( format == PixelFormat.R8G8B8 )
			{
				mask = new byte[ size.X * size.Y ];
				for( int n = 0; n < mask.Length; n++ )
					mask[ n ] = data[ n * 3 ];
			}
			else
			{
				error = $"Can't to read the image of this format \"{format}\". Must be {PixelFormat.L8} or {PixelFormat.R8G8B8}.";
				return false;
			}

			return true;
		}

		public bool SaveMask( string realFileName, out string error )
		{
			var mask = Mask.Value;

			if( mask != null )
			{
				var sideSize = (int)Math.Sqrt( mask.Length );

				if( !ImageUtility.Save( realFileName, mask, new Vector2I( sideSize, sideSize ), 1, PixelFormat.L8, 1, 0, out error ) )
					return false;
			}

			error = "";
			return true;
		}

	}
}
