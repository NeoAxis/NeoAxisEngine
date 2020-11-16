// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A tool for rendering a scene to a file.
	/// </summary>
	[EditorSettingsCell( typeof( Component_RenderToFile_SettingsCell ) )]
	public class Component_RenderToFile : Component
	{
		//!!!!рендеринть в большую картинку, потом уменьшать

		//!!!!выставлять параметры в пайплайне. лучше качество

		//!!!!HDR. сохранять видимо в .hdr файл

		//!!!!выбирать из списка + ManualResolution? для видео полезно
		//!!!!default
		//[DefaultValue( "4096 3072" )]

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
		public event Action<Component_RenderToFile> ResolutionChanged;
		ReferenceField<Vector2I> _resolution = new Vector2I( 1920, 1080 );// 4096, 3072 );

		/// <summary>
		/// The file name to be output.
		/// </summary>
		[DefaultValue( "" )]//[DefaultValue( @"C:\_Test\Test.png" )]
		public Reference<string> OutputFileName
		{
			get { if( _outputFileName.BeginGet() ) OutputFileName = _outputFileName.Get( this ); return _outputFileName.value; }
			set { if( _outputFileName.BeginSet( ref value ) ) { try { OutputFileNameChanged?.Invoke( this ); } finally { _outputFileName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OutputFileName"/> property value changes.</summary>
		public event Action<Component_RenderToFile> OutputFileNameChanged;
		ReferenceField<string> _outputFileName = "";//@"C:\_Test\Test.png";

		/// <summary>
		/// The camera for which the display is being performed. If no camera is specified, the editor's current camera is used.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Camera> Camera
		{
			get { if( _camera.BeginGet() ) Camera = _camera.Get( this ); return _camera.value; }
			set { if( _camera.BeginSet( ref value ) ) { try { CameraChanged?.Invoke( this ); } finally { _camera.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Camera"/> property value changes.</summary>
		public event Action<Component_RenderToFile> CameraChanged;
		ReferenceField<Component_Camera> _camera = null;

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
		public event Action<Component_RenderToFile> DisplayDevelopmentDataChanged;
		ReferenceField<bool> _displayDevelopmentData = false;

		//

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "RenderToFile", true );
		}
	}
}
