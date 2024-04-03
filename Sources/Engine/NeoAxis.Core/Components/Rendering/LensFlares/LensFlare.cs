// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A definition of lens flare object.
	/// </summary>
	public class LensFlare : Component
	{
		/// <summary>
		/// The image of the flare.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ImageComponent> Image
		{
			get { if( _image.BeginGet() ) Image = _image.Get( this ); return _image.value; }
			set { if( _image.BeginSet( this, ref value ) ) { try { ImageChanged?.Invoke( this ); } finally { _image.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Image"/> property value changes.</summary>
		public event Action<LensFlare> ImageChanged;
		ReferenceField<ImageComponent> _image = null;

		/// <summary>
		/// The method of drawing the image of the flare on the screen.
		/// </summary>
		[DefaultValue( CanvasRenderer.BlendingType.AlphaAdd )]
		public Reference<CanvasRenderer.BlendingType> Blending
		{
			get { if( _blending.BeginGet() ) Blending = _blending.Get( this ); return _blending.value; }
			set { if( _blending.BeginSet( this, ref value ) ) { try { BlendingChanged?.Invoke( this ); } finally { _blending.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Blending"/> property value changes.</summary>
		public event Action<LensFlare> BlendingChanged;
		ReferenceField<CanvasRenderer.BlendingType> _blending = CanvasRenderer.BlendingType.AlphaAdd;

		/// <summary>
		/// The color of the flare.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<LensFlare> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// The position of the flare.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -10, 1, RangeAttribute.ConvenientDistributionEnum.Exponential, 0.2 )]
		public Reference<double> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( this, ref value ) ) { try { PositionChanged?.Invoke( this ); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<LensFlare> PositionChanged;
		ReferenceField<double> _position = 0.0;

		/// <summary>
		/// The size of the flare. It is indicated as a ratio of screen size vertically.
		/// </summary>
		[DefaultValue( "0.1 0.1" )]
		[Range( 0, 0.5 )]
		public Reference<Vector2> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set { if( _size.BeginSet( this, ref value ) ) { try { SizeChanged?.Invoke( this ); } finally { _size.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Size"/> property value changes.</summary>
		public event Action<LensFlare> SizeChanged;
		ReferenceField<Vector2> _size = new Vector2( 0.1, 0.1 );

		[DefaultValue( false )]
		public Reference<bool> SizeFadeByDistance
		{
			get { if( _sizeFadeByDistance.BeginGet() ) SizeFadeByDistance = _sizeFadeByDistance.Get( this ); return _sizeFadeByDistance.value; }
			set { if( _sizeFadeByDistance.BeginSet( this, ref value ) ) { try { SizeFadeByDistanceChanged?.Invoke( this ); } finally { _sizeFadeByDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SizeFadeByDistance"/> property value changes.</summary>
		public event Action<LensFlare> SizeFadeByDistanceChanged;
		ReferenceField<bool> _sizeFadeByDistance = false;

		[DefaultValue( 0.1 )]
		[Range( 0, 2, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> DepthCheckOffset
		{
			get { if( _depthCheckOffset.BeginGet() ) DepthCheckOffset = _depthCheckOffset.Get( this ); return _depthCheckOffset.value; }
			set { if( _depthCheckOffset.BeginSet( this, ref value ) ) { try { DepthCheckOffsetChanged?.Invoke( this ); } finally { _depthCheckOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DepthCheckOffset"/> property value changes.</summary>
		public event Action<LensFlare> DepthCheckOffsetChanged;
		ReferenceField<double> _depthCheckOffset = 0.1;


		//public enum SizeMeasureEnum
		//{
		//	Screen,
		//	World,
		//}

		//[DefaultValue( SizeMeasureEnum.Screen )]
		//public Reference<SizeMeasureEnum> SizeMeasure
		//{
		//	get { if( _sizeMeasure.BeginGet() ) SizeMeasure = _sizeMeasure.Get( this ); return _sizeMeasure.value; }
		//	set { if( _sizeMeasure.BeginSet( this, ref value ) ) { try { SizeMeasureChanged?.Invoke( this ); } finally { _sizeMeasure.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SizeMeasure"/> property value changes.</summary>
		//public event Action<LensFlare> SizeMeasureChanged;
		//ReferenceField<SizeMeasureEnum> _sizeMeasure = SizeMeasureEnum.Screen;


		//[FieldSerialize( "fadeSpeedFactor" )]
		//float fadeSpeedFactor = 1;
	}
}
