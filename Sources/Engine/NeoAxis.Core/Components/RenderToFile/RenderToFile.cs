// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

//!!!!рендеринть в большую картинку, потом уменьшать

//!!!!HDR. сохранять видимо в .hdr файл

namespace NeoAxis
{
	/// <summary>
	/// A tool for rendering a scene to a file. It intended to create screenshots and to create materials.
	/// </summary>
#if !DEPLOY
	[SettingsCell( typeof( RenderToFileSettingsCell ) )]
#endif
	[AddToResourcesWindow( @"Base\Scene objects\Additional\Render To File", 0 )]
	public class RenderToFile : Component
	{
		public enum ModeEnum
		{
			Screenshot,
			Video,
			Material,
		}

		/// <summary>
		/// The type of generated data.
		/// </summary>
		[DefaultValue( ModeEnum.Screenshot )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<RenderToFile> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Screenshot;

		/// <summary>
		/// The size of the resulting image.
		/// </summary>
		[DefaultValue( "1920 1080" )]
		[Range( 10, 10000 )]
		public Reference<Vector2I> Resolution
		{
			get { if( _resolution.BeginGet() ) Resolution = _resolution.Get( this ); return _resolution.value; }
			set { if( _resolution.BeginSet( ref value ) ) { try { ResolutionChanged?.Invoke( this ); } finally { _resolution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Resolution"/> property value changes.</summary>
		public event Action<RenderToFile> ResolutionChanged;
		ReferenceField<Vector2I> _resolution = new Vector2I( 1920, 1080 );// 4096, 3072 );

		/// <summary>
		/// The file name to be output.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> OutputFileName
		{
			get { if( _outputFileName.BeginGet() ) OutputFileName = _outputFileName.Get( this ); return _outputFileName.value; }
			set { if( _outputFileName.BeginSet( ref value ) ) { try { OutputFileNameChanged?.Invoke( this ); } finally { _outputFileName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OutputFileName"/> property value changes.</summary>
		public event Action<RenderToFile> OutputFileNameChanged;
		ReferenceField<string> _outputFileName = "";

		/// <summary>
		/// The camera for which the display is being performed. If no camera is specified, the editor's current camera is used.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Camera> Camera
		{
			get { if( _camera.BeginGet() ) Camera = _camera.Get( this ); return _camera.value; }
			set { if( _camera.BeginSet( ref value ) ) { try { CameraChanged?.Invoke( this ); } finally { _camera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Camera"/> property value changes.</summary>
		public event Action<RenderToFile> CameraChanged;
		ReferenceField<Camera> _camera = null;

		/// <summary>
		/// The rendering pipeline to override the default pipeline of the scene.
		/// </summary>
		[DefaultValue( null )]
		public Reference<RenderingPipeline> RenderingPipeline
		{
			get { if( _renderingPipeline.BeginGet() ) RenderingPipeline = _renderingPipeline.Get( this ); return _renderingPipeline.value; }
			set { if( _renderingPipeline.BeginSet( ref value ) ) { try { RenderingPipelineChanged?.Invoke( this ); } finally { _renderingPipeline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RenderingPipeline"/> property value changes.</summary>
		public event Action<RenderToFile> RenderingPipelineChanged;
		ReferenceField<RenderingPipeline> _renderingPipeline = null;

		/// <summary>
		/// Whether to visualize development data of the scene.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DisplayDevelopmentData
		{
			get { if( _displayDevelopmentData.BeginGet() ) DisplayDevelopmentData = _displayDevelopmentData.Get( this ); return _displayDevelopmentData.value; }
			set { if( _displayDevelopmentData.BeginSet( ref value ) ) { try { DisplayDevelopmentDataChanged?.Invoke( this ); } finally { _displayDevelopmentData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayDevelopmentData"/> property value changes.</summary>
		public event Action<RenderToFile> DisplayDevelopmentDataChanged;
		ReferenceField<bool> _displayDevelopmentData = false;

		/// <summary>
		/// A template for a generated material.
		/// </summary>
		[Category( "Material" )]
		[DefaultValue( null )]
		public Reference<Material> Template
		{
			get { if( _template.BeginGet() ) Template = _template.Get( this ); return _template.value; }
			set { if( _template.BeginSet( ref value ) ) { try { TemplateChanged?.Invoke( this ); } finally { _template.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Template"/> property value changes.</summary>
		public event Action<RenderToFile> TemplateChanged;
		ReferenceField<Material> _template = null;

		/// <summary>
		/// Whether to fill transparent pixels of generated textures by near pixels to make mipmapping work good.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Material" )]
		public Reference<bool> FillTransparentPixelsByNearPixels
		{
			get { if( _fillTransparentPixelsByNearPixels.BeginGet() ) FillTransparentPixelsByNearPixels = _fillTransparentPixelsByNearPixels.Get( this ); return _fillTransparentPixelsByNearPixels.value; }
			set { if( _fillTransparentPixelsByNearPixels.BeginSet( ref value ) ) { try { FillTransparentPixelsByNearPixelsChanged?.Invoke( this ); } finally { _fillTransparentPixelsByNearPixels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FillTransparentPixelsByNearPixels"/> property value changes.</summary>
		public event Action<RenderToFile> FillTransparentPixelsByNearPixelsChanged;
		ReferenceField<bool> _fillTransparentPixelsByNearPixels = false;

		/// <summary>
		/// The number of frames per second.
		/// </summary>
		[DefaultValue( 60 )]
		public Reference<int> FramesPerSecond
		{
			get { if( _framesPerSecond.BeginGet() ) FramesPerSecond = _framesPerSecond.Get( this ); return _framesPerSecond.value; }
			set { if( _framesPerSecond.BeginSet( ref value ) ) { try { FramesPerSecondChanged?.Invoke( this ); } finally { _framesPerSecond.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FramesPerSecond"/> property value changes.</summary>
		public event Action<RenderToFile> FramesPerSecondChanged;
		ReferenceField<int> _framesPerSecond = 60;

		/// <summary>
		/// The length of the video in seconds.
		/// </summary>
		[DefaultValue( 10.0 )]
		public Reference<double> Length
		{
			get { if( _length.BeginGet() ) Length = _length.Get( this ); return _length.value; }
			set { if( _length.BeginSet( ref value ) ) { try { LengthChanged?.Invoke( this ); } finally { _length.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Length"/> property value changes.</summary>
		public event Action<RenderToFile> LengthChanged;
		ReferenceField<double> _length = 10.0;

		public enum FormatEnum
		{
			NoCompression,

			[DisplayNameEnum( "Lagarith Lossless [LAGS]" )]
			LagarithLosslessLAGS,

			Other,
		}

		/// <summary>
		/// The compression format of the video.
		/// </summary>
		[DefaultValue( FormatEnum.NoCompression )]
		public Reference<FormatEnum> Format
		{
			get { if( _format.BeginGet() ) Format = _format.Get( this ); return _format.value; }
			set { if( _format.BeginSet( ref value ) ) { try { FormatChanged?.Invoke( this ); } finally { _format.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Format"/> property value changes.</summary>
		public event Action<RenderToFile> FormatChanged;
		ReferenceField<FormatEnum> _format = FormatEnum.NoCompression;

		/// <summary>
		/// The compression format specified by FourCC.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> FormatFourCC
		{
			get { if( _formatFourCC.BeginGet() ) FormatFourCC = _formatFourCC.Get( this ); return _formatFourCC.value; }
			set { if( _formatFourCC.BeginSet( ref value ) ) { try { FormatFourCCChanged?.Invoke( this ); } finally { _formatFourCC.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FormatFourCC"/> property value changes.</summary>
		public event Action<RenderToFile> FormatFourCCChanged;
		ReferenceField<string> _formatFourCC = "";

		public enum CaptureMethodEnum
		{
			CaptureFromScreen,
		}

		[DefaultValue( CaptureMethodEnum.CaptureFromScreen )]
		public Reference<CaptureMethodEnum> CaptureMethod
		{
			get { if( _captureMethod.BeginGet() ) CaptureMethod = _captureMethod.Get( this ); return _captureMethod.value; }
			set { if( _captureMethod.BeginSet( ref value ) ) { try { CaptureMethodChanged?.Invoke( this ); } finally { _captureMethod.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CaptureMethod"/> property value changes.</summary>
		public event Action<RenderToFile> CaptureMethodChanged;
		ReferenceField<CaptureMethodEnum> _captureMethod = CaptureMethodEnum.CaptureFromScreen;

		//

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Template ):
				case nameof( FillTransparentPixelsByNearPixels ):
					if( Mode.Value != ModeEnum.Material )
						skip = true;
					break;

				case nameof( RenderingPipeline ):
					if( Mode.Value != ModeEnum.Screenshot && Mode.Value != ModeEnum.Video )
						skip = true;
					break;

				case nameof( DisplayDevelopmentData ):
					if( Mode.Value != ModeEnum.Screenshot && Mode.Value != ModeEnum.Material )
						skip = true;
					break;

				case nameof( FramesPerSecond ):
				case nameof( Length ):
				case nameof( CaptureMethod ):
				case nameof( Format ):
					if( Mode.Value != ModeEnum.Video )
						skip = true;
					break;

				case nameof( FormatFourCC ):
					if( Mode.Value != ModeEnum.Video )
						skip = true;
					if( Format.Value != FormatEnum.Other )
						skip = true;
					break;
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "RenderToFile", true );
		}
	}
}
