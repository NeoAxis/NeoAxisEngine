// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for Windows.
	/// </summary>
	public class Component_Product_Windows : Component_Product
	{
		///// <summary>
		///// Whether to include build tools that are intended to compile C# scripts.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Files" )]
		//public Reference<bool> BuildTools
		//{
		//	get { if( _buildTools.BeginGet() ) BuildTools = _buildTools.Get( this ); return _buildTools.value; }
		//	set { if( _buildTools.BeginSet( ref value ) ) { try { BuildToolsChanged?.Invoke( this ); } finally { _buildTools.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BuildTools"/> property value changes.</summary>
		//public event Action<Component_Product_Windows> BuildToolsChanged;
		//ReferenceField<bool> _buildTools = false;

		/// <summary>
		/// Whether to include files for debugging (xml, pdb).
		/// </summary>
		[DefaultValue( false )]
		[Category( "Files" )]
		public Reference<bool> DebugFiles
		{
			get { if( _debugFiles.BeginGet() ) DebugFiles = _debugFiles.Get( this ); return _debugFiles.value; }
			set { if( _debugFiles.BeginSet( ref value ) ) { try { DebugFilesChanged?.Invoke( this ); } finally { _debugFiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugFiles"/> property value changes.</summary>
		public event Action<Component_Product_Windows> DebugFilesChanged;
		ReferenceField<bool> _debugFiles = false;

		/// <summary>
		/// Whether to include files to support UIWebBrowser control.
		/// </summary>
		[Category( "Files" )]
		[DisplayName( "UIWebBrowser" )]
		[DefaultValue( false )]
		public Reference<bool> UIWebBrowser
		{
			get { if( _uIWebBrowser.BeginGet() ) UIWebBrowser = _uIWebBrowser.Get( this ); return _uIWebBrowser.value; }
			set { if( _uIWebBrowser.BeginSet( ref value ) ) { try { UIWebBrowserChanged?.Invoke( this ); } finally { _uIWebBrowser.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIWebBrowser"/> property value changes.</summary>
		public event Action<Component_Product_Windows> UIWebBrowserChanged;
		ReferenceField<bool> _uIWebBrowser = false;

		/////////////////////////////////////////

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Windows; }
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			List<string> folders = new List<string>();
			folders.Add( "Assets" );
			folders.Add( "Binaries" );
			folders.Add( "Caches" );

			//copy files
			try
			{
				var percentStep = 0.99 / folders.Count;
				var currentPercent = 0.0;

				for( int nFolder = 0; nFolder < folders.Count; nFolder++ )
				{
					var folder = folders[ nFolder ];
					var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, folder );
					var destFolder = Path.Combine( buildInstance.DestinationFolder, folder );

					//!!!!проценты от размера папки
					var percentRange = new Range( currentPercent, currentPercent + percentStep );

					bool skipDefaultBehavior = false;

					var assetsExcludePaths = new List<string>();
					assetsExcludePaths.Add( Path.Combine( sourceFolder, @"Base\Tools" ) );

					if( folder == "Assets" )
					{
						var values = SelectedAssets.Value.Trim();
						if( !string.IsNullOrEmpty( values ) )
						{
							foreach( var v in values.Split( new char[] { ';' } ) )
							{
								var v2 = v.Trim();
								if( !string.IsNullOrEmpty( v2 ) )
								{
									var sourceFolder2 = Path.Combine( VirtualFileSystem.Directories.Project, folder, v2 );
									var destFolder2 = Path.Combine( buildInstance.DestinationFolder, folder, v2 );
									var percentRange2 = new Range( percentRange.Minimum, percentRange.Minimum );

									CopyFolder( sourceFolder2, destFolder2, buildInstance, percentRange2, assetsExcludePaths );

									if( CheckCancel( buildInstance ) )
										return;
								}
							}

							skipDefaultBehavior = true;
						}
					}

					if( !skipDefaultBehavior )
					{
						var excludePaths = new List<string>();
						if( folder == "Binaries" )
						{
							excludePaths.AddRange( GetPlatformsExcludePaths() );

							excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
							excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
							excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );

							excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe" ) );
							excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe.config" ) );
							excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe" ) );
							excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe.config" ) );
							excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe" ) );
							excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe.config" ) );

							excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\dotnet" ) );
							//if( !BuildTools )
							//{
							//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
							//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
							//}

							if( !UIWebBrowser )
								excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CefGlue" ) );

							if( !DebugFiles )
							{
								foreach( var fileName in Directory.GetFiles( sourceFolder, "*.pdb", SearchOption.TopDirectoryOnly ) )
									excludePaths.Add( fileName );
								foreach( var fileName in Directory.GetFiles( sourceFolder, "*.xml", SearchOption.TopDirectoryOnly ) )
									excludePaths.Add( fileName );
								foreach( var fileName in Directory.GetFiles( sourceFolder, "*.mdb", SearchOption.TopDirectoryOnly ) )
									excludePaths.Add( fileName );
							}
						}
						if( folder == "Caches" )
						{
							if( !ShaderCache )
								excludePaths.Add( @"Caches\ShaderCache" );
							if( !FileCache )
								excludePaths.Add( @"Caches\Files" );
						}
						if( folder == "Assets" )
							excludePaths.AddRange( assetsExcludePaths );

						CopyFolder( sourceFolder, destFolder, buildInstance, percentRange, excludePaths );

						if( CheckCancel( buildInstance ) )
							return;
					}

					currentPercent += percentStep;
				}
			}
			catch( Exception e )
			{
				buildInstance.Error = e.Message;
				buildInstance.State = ProductBuildInstance.StateEnum.Error;
				return;
			}

			////write License.cert
			//if( EngineApp.IsProPlan )
			//{
			//	var realFileName = Path.Combine( buildInstance.DestinationFolder, "License.cert" );

			//	var date = DateTime.UtcNow;
			//	//add 10 years
			//	date = date.Add( new TimeSpan( 365 * 10, 0, 0, 0, 0 ) );

			//	if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
			//	{
			//		if( !LoginUtility.SaveLicenseCertificate( realFileName, email, EngineInfo.Version, EngineApp.License, "", date, out string error ) )
			//		{
			//			//!!!!
			//		}
			//	}
			//}

			//done
			buildInstance.Progress = 1;
			buildInstance.State = ProductBuildInstance.StateEnum.Success;

			//run
			if( buildInstance.Run )
			{
				string executableFileName = Path.Combine( buildInstance.DestinationFolder, "Binaries", ExecutableName + ".exe" );
				Process.Start( executableFileName, "" );
			}


			//string destBinariesDirectory = Path.Combine( buildInstance.DestinationFolder, "Files" );

			////copy files
			//try
			//{
			//	var excludePaths = GetPlatformsExcludePaths();
			//	CopyFolder( VirtualFileSystem.Directories.Binaries, destBinariesDirectory, buildInstance, new Range( 0, 0.99 ), excludePaths );
			//}
			//catch( Exception e )
			//{
			//	buildInstance.Error = e.Message;
			//	buildInstance.State = ProductBuildInstance.StateEnum.Error;
			//	return;
			//}

			////done
			//buildInstance.Progress = 1;
			//buildInstance.State = ProductBuildInstance.StateEnum.Success;

			////run
			//if( buildInstance.Run )
			//{
			//	string executableFileName = Path.Combine( destBinariesDirectory, ExecutableName + ".exe" );
			//	var runMapProcess = Process.Start( executableFileName, "" );
			//}

			if( CheckCancel( buildInstance ) )
				return;

			ShowSuccessScreenNotification();
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return true; }
		}
	}
}
