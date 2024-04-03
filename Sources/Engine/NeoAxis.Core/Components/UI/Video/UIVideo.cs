// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using OggDecoder;
using MyOggDecoder;

namespace NeoAxis
{
	//!!!!пропадает картинка при паузе

	/// <summary>
	/// The control for video playback. OGV format is supported.
	/// </summary>
	//[Editor.AddToResourcesWindow( @"Base\UI\Video", 0 )]
	public class UIVideo : UIControl
	{
		OggFile oggFile;
		bool firstTick = true;
		bool noLogErrorAtLoading;
		VideoBuffer videoBuffer = new VideoBuffer();
		static bool nativeLibrariesLoaded;

		///////////////////////////////////////////

		/// <summary>
		/// The source video file. OGV format is supported.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ReferenceValueType_Resource> FileName
		{
			get { if( _fileName.BeginGet() ) FileName = _fileName.Get( this ); return _fileName.value; }
			set
			{
				if( _fileName.BeginSet( this, ref value ) )
				{
					try
					{
						FileNameChanged?.Invoke( this );

						if( EnabledInHierarchyAndIsInstance && ParentContainer != null )
							CreateOggFile();

						if( oggFile == null )
							videoBuffer.Clear();
					}
					finally { _fileName.EndSet(); }
				}
			}
		}
		public event Action<UIVideo> FileNameChanged;
		ReferenceField<ReferenceValueType_Resource> _fileName;

		/// <summary>
		/// Whether to repeat video playback.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Loop
		{
			get { if( _loop.BeginGet() ) Loop = _loop.Get( this ); return _loop.value; }
			set { if( _loop.BeginSet( this, ref value ) ) { try { LoopChanged?.Invoke( this ); } finally { _loop.EndSet(); } } }
		}
		public event Action<UIVideo> LoopChanged;
		ReferenceField<bool> _loop = false;

		/// <summary>
		/// Whether the video paused.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Pause
		{
			get { if( _pause.BeginGet() ) Pause = _pause.Get( this ); return _pause.value; }
			set
			{
				if( _pause.BeginSet( this, ref value ) )
				{
					try
					{
						PauseChanged?.Invoke( this );

						if( oggFile != null )
						{
							if( !_pause.value.Value && !Loop.Value && oggFile.EndOfFile )
								Rewind();

							oggFile.Pause = _pause.value;
						}
					}
					finally { _pause.EndSet(); }
				}
			}
		}
		public event Action<UIVideo> PauseChanged;
		ReferenceField<bool> _pause = false;

		/// <summary>
		/// The audio volume of the video.
		/// </summary>
		[Range( 0, 1 )]
		[DefaultValue( 1.0 )]
		public Reference<double> Volume
		{
			get { if( _volume.BeginGet() ) Volume = _volume.Get( this ); return _volume.value; }
			set
			{
				if( _volume.BeginSet( this, ref value ) )
				{
					try
					{
						VolumeChanged?.Invoke( this );

						if( oggFile != null )
							oggFile.Volume = _volume.value;
					}
					finally { _volume.EndSet(); }
				}
			}
		}
		public event Action<UIVideo> VolumeChanged;
		ReferenceField<double> _volume = 1.0;

		///////////////////////////////////////////

		void PreloadNativeLibraries()
		{
			if( !nativeLibrariesLoaded )
			{
				nativeLibrariesLoaded = true;
				//NativeLibraryManager.PreloadLibrary( "libogg" );
				//NativeLibraryManager.PreloadLibrary( "libvorbis" );
				//NativeLibraryManager.PreloadLibrary( "libtheora" );
				//NativeLibraryManager.PreloadLibrary( "libvorbisfile" );
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				PreloadNativeLibraries();

				if( ParentContainer != null && oggFile == null )
					CreateOggFile();
			}
			else
			{
				DestroyOggFile();
				videoBuffer.Clear();
			}
		}

		void CreateOggFile()
		{
			PreloadNativeLibraries();

			DestroyOggFile();

			var fileName = FileName.Value?.ResourceName;

			if( string.IsNullOrEmpty( fileName ) )
				return;

			VirtualFileStream stream = null;

			try
			{
				stream = VirtualFile.Open( fileName );

				oggFile = new OggFile();
				oggFile.Volume = Volume;

				bool sound3D = ParentContainer.Transform3D != null;// is UIContainer3D;
				oggFile.Init( stream, new MyVideoDriver( videoBuffer ), new MyAudioDriver(), sound3D );
			}
			catch( Exception ex )
			{
				if( !noLogErrorAtLoading )
					Log.Warning( string.Format( "UIVideo: CreateOggFile: Error: {0} ({1}).", ex.Message, fileName ) );

				if( oggFile != null )
				{
					oggFile.Dispose();
					oggFile = null;
				}

				if( stream != null )
					stream.Dispose();

				return;
			}

			if( oggFile.VideoDriver == null )
			{
				oggFile.Dispose();
				oggFile = null;
			}

			if( oggFile == null )
				return;

			UpdateOggFileSoundPosition();

			oggFile.Pause = Pause;
		}

		void DestroyOggFile()
		{
			if( oggFile != null )
			{
				oggFile.Dispose();
				oggFile = null;
			}
		}

		void UpdateOggFileSoundPosition()
		{
			if( ParentContainer.Transform3D != null )
				oggFile.SoundPosition = ParentContainer.Transform3D.Position;

			//UIContainer3D in3dControlManager = ParentContainer as UIContainer3D;
			//if( in3dControlManager != null )
			//	oggFile.SoundPosition = in3dControlManager.Position3D;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( firstTick )
			{
				if( oggFile == null )
					CreateOggFile();
				firstTick = false;
			}

			if( oggFile != null && !Pause )
			{
				oggFile.Update( delta );

				UpdateOggFileSoundPosition();

				if( Loop && oggFile.EndOfFile )
					Rewind();
			}
		}

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			//base.OnRenderUI( renderer );

			var videoTexture = videoBuffer.GetUpdatedTexture();
			var texture = videoTexture?.Result;

			//!!!!
			//bool backColorZero = BackColor == new ColorValue( 0, 0, 0, 0 );

			{
				//!!!!
				Rectangle texCoord = new Rectangle( 0, 0, 1, 1 );
				//Rect texCoord = BackTextureCoord;

				//correct tex coord round2 texture
				if( texture != null )
				{
					Vector2 textureSize = texture.ResultSize.ToVector2F();
					Vector2 videoSize = oggFile.VideoDriver.GetSize().ToVector2F();
					Vector2 coef = videoSize / textureSize;

					texCoord.Right *= coef.X;
					texCoord.Bottom *= coef.Y;
				}

				//!!!!было
				//if( BackTextureTile && texture != null )
				//{
				//	Vec2 tileCount = GetScreenTextureBaseSize() / texture.Size.ToVec2F() * GetScreenSize();
				//	texCoord = new Rect( -tileCount * .5f, tileCount * .5f ) + new Vec2( .5f, .5f );
				//}

				//!!!!
				ColorValue color = new ColorValue( 1, 1, 1 );
				//ColorValue color = backColorZero ? new ColorValue( 1, 1, 1 ) : BackColor;
				if( texture == null )
					color = new ColorValue( 0, 0, 0, color.Alpha );
				//color *= GetTotalColorMultiplier();

				color.Clamp( new ColorValue( 0, 0, 0, 0 ), new ColorValue( 1, 1, 1, 1 ) );

				GetScreenRectangle( out var rect );

				//!!!!
				renderer.AddQuad( rect, texCoord, videoTexture, color, true );
				//renderer.AddQuad( rect, texCoord, videoTexture, color, BackTextureTile ? false : true );
			}
		}

		[Browsable( false )]
		public bool Loaded
		{
			get { return oggFile != null; }
		}

		[Browsable( false )]
		public bool EndOfFile
		{
			get
			{
				if( oggFile == null )
					return false;
				return oggFile.EndOfFile;
			}
		}

		public void Rewind()
		{
			if( ParentContainer != null )
				CreateOggFile();
		}

		//[Browsable( false )]
		//public float Length
		//{
		//   get
		//   {
		//      if( oggFile == null )
		//         return 0;
		//   }
		//}

		/// <summary>
		/// The sampling rate of the audio.
		/// </summary>
		[Browsable( false )]
		public int AudioSamplingRate
		{
			get
			{
				if( oggFile == null || oggFile.AudioDriver == null )
					return 0;
				return oggFile.AudioDriver.GetRate();
			}
		}

		/// <summary>
		/// The channels of the audio.
		/// </summary>
		[Browsable( false )]
		public int AudioChannels
		{
			get
			{
				if( oggFile == null || oggFile.AudioDriver == null )
					return 0;
				return oggFile.AudioDriver.GetChannels();
			}
		}

		/// <summary>
		/// The resolution of the video.
		/// </summary>
		[Browsable( false )]
		public Vector2I VideoSize
		{
			get
			{
				if( oggFile == null || oggFile.VideoDriver == null )
					return Vector2I.Zero;
				return oggFile.VideoDriver.GetSize();
			}
		}

		/// <summary>
		/// The frame rate of the video.
		/// </summary>
		[Browsable( false )]
		public float VideoFPS
		{
			get
			{
				if( oggFile == null || oggFile.VideoDriver == null )
					return 0;
				return oggFile.VideoDriver.GetFPS();
			}
		}

		/// <summary>
		/// Current position of the video playback in seconds.
		/// </summary>
		[Browsable( false )]
		public double CurrentTime
		{
			get
			{
				if( oggFile == null )
					return 0;
				return oggFile.currentTime;
			}
		}

		/// <summary>
		/// Whether to log video loading errors.
		/// </summary>
		[Browsable( false )]
		public bool NoLogErrorAtLoading
		{
			get { return noLogErrorAtLoading; }
			set { noLogErrorAtLoading = value; }
		}
	}
}
