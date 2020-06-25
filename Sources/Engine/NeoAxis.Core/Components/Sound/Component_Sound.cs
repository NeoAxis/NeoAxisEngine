// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;
using NeoAxis.Editor;

namespace NeoAxis
{
	//!!!!можно еще .settings для того, чтобы добавить опции. также как импорт или еще что-то

	//!!!!создание (не загрузка). как текстуры

	//!!!!
	//[EditorPreviewControl( typeof( Component_Sound_PreviewControl ) )]

	/// <summary>
	/// The component containing sound data.
	/// </summary>
	[ResourceFileExtension( "sound" )]//!!!!!так?
	[EditorPreviewControl( typeof( Component_Sound_PreviewControl ) )]
	public class Component_Sound : Component_ResultCompile<Component_Sound.CompiledData>
	{
		////CreateFormat
		//ReferenceField<PixelFormat> _createFormat = PixelFormat.Unknown;
		//[Serialize]
		//[DefaultValue( PixelFormat.Unknown )]
		//public Reference<PixelFormat> CreateFormat
		//{
		//	get
		//	{
		//		if( _createFormat.BeginGet() )
		//			CreateFormat = _createFormat.Get( this );
		//		return _createFormat.value;
		//	}
		//	set
		//	{
		//		if( _createFormat.BeginSet( ref value ) )
		//		{
		//			try { CreateFormatChanged?.Invoke( this ); }
		//			finally { _createFormat.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Sound> CreateFormatChanged;

		////CreateSize
		//ReferenceField<Vec2I> _createSize = Vec2I.Zero;
		//[Serialize]
		//[DefaultValue( "0 0" )]
		//public Reference<Vec2I> CreateSize
		//{
		//	get
		//	{
		//		if( _createSize.BeginGet() )
		//			CreateSize = _createSize.Get( this );
		//		return _createSize.value;
		//	}
		//	set
		//	{
		//		if( _createSize.BeginSet( ref value ) )
		//		{
		//			try { CreateSizeChanged?.Invoke( this ); }
		//			finally { _createSize.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Sound> CreateSizeChanged;

		//LoadFile
		ReferenceField<ReferenceValueType_Resource> _loadFile;
		/// <summary>
		/// The name of the file with sound data.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		//[DefaultValue( "" )]
		//[SelectFile]
		public Reference<ReferenceValueType_Resource> LoadFile
		{
			get { if( _loadFile.BeginGet() ) LoadFile = _loadFile.Get( this ); return _loadFile.value; }
			set
			{
				if( _loadFile.BeginSet( ref value ) )
				{
					try
					{
						LoadFileChanged?.Invoke( this );
						//!!!!обновлять? везде так
					}
					finally { _loadFile.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoadFile"/> property value changes.</summary>
		public event Action<Component_Sound> LoadFileChanged;

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				//if( p.Name == "CreateType" || p.Name == "CreateSize" || p.Name == "CreateDepth" || p.Name == "CreateMipmaps" ||
				//	p.Name == "CreateUsage" || p.Name == "CreateFSAA" )
				//{
				//	if( CreateFormat.Value == PixelFormat.Unknown )
				//	{
				//		skip = true;
				//		return;
				//	}
				//}

				//if( p.Name == nameof( LoadFile ) )
				//{
				//	if( CreateFormat.Value != PixelFormat.Unknown )
				//	{
				//		skip = true;
				//		return;
				//	}
				//}
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents a precalculated data of <see cref="Component_Sound"/>.
		/// </summary>
		public class CompiledData : IDisposable
		{
			Component_Sound owner;

			Dictionary<SoundModes, Sound> soundByMode = new Dictionary<SoundModes, Sound>();
			//Sound sound;

			////////////

			public CompiledData( Component_Sound owner )
			{
				this.owner = owner;
			}

			public virtual void Dispose()
			{
				//!!!!threading: как будет, если из другого потока еще юзаем, но меш обновился

				foreach( var sound in soundByMode.Values )
					if( sound != null )
						sound.Dispose();
				soundByMode.Clear();
				//sound?.Dispose();
				//sound = null;
			}

			public Component_Sound Owner
			{
				get { return owner; }
			}

			//public Sound Sound
			//{
			//	get { return sound; }
			//}

			//Sound GetSoundByMode( SoundModes mode )
			//{
			//	soundByMode.TryGetValue( mode, out var sound );
			//	return sound;
			//}

			public Sound LoadSoundByMode( SoundModes mode )
			{
				if( !soundByMode.TryGetValue( mode, out var sound ) )
				{
					//!!!!threading

					var v = owner.LoadFile.Value;
					var resourceName = v != null ? v.ResourceName : "";

					if( VirtualFile.Exists( resourceName ) )
						sound = SoundWorld.SoundCreate( resourceName, mode );

					soundByMode.Add( mode, sound );
				}
				return sound;
			}
		}

		/////////////////////////////////////////

		protected override void OnResultCompile()
		{
			if( Result == null )
			{
				CompiledData result = null;

				//PixelFormat createFormatV = CreateFormat;
				//if( createFormatV != PixelFormat.Unknown )
				//{
				//	//create

				//		string error2;
				//		result = new GpuTexture( type, size, depth, mipmaps, createFormatV, gpuUsage, fsaa, out error2 );
				//		if( !string.IsNullOrEmpty( error2 ) )
				//		{
				//			result.Dispose();
				//			result = null;

				//			//!!!!!
				//			Log.Fatal( "impl" );
				//		}
				//}
				//else
				{
					//load

					//threading

					//каждый раз проверять будет если нет файла. где еще так?

					//var v = LoadFile.Value;
					//var resourceName = v != null ? v.ResourceName : "";

					//if( VirtualFile.Exists( resourceName ) )
					//{
					result = new CompiledData( this );
					//}
				}

				Result = result;
			}
		}
	}
}
