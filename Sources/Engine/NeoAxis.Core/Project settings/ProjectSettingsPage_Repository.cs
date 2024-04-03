// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Repository page of the project settings.
	/// </summary>
	public class ProjectSettingsPage_Repository : ProjectSettingsPage
	{
		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode && member is Metadata.Property )
			{
				//Cloud project mode
				if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
				{
					if( member.Name == nameof( PageInfo ) )
						skip = true;
				}
				else
				{
					if( member.Name == nameof( CompileScriptsBeforeCommit ) )
						skip = true;
					if( member.Name == nameof( SkipPaths ) )
						skip = true;
					else if( member.Name == nameof( SkipFoldersWithName ) )
						skip = true;
					else if( member.Name == nameof( SkipFilesWithExtensionWhenPlay ) )
						skip = true;
					else if( member.Name == nameof( ClearFilesWithExtensionWhenPlay ) )
						skip = true;
					else if( member.Name == nameof( StopServerDuringCommit ) )
						skip = true;
				}
			}
		}

		public string PageInfo
		{
			get { return "The page is only used for cloud projects."; }
		}

		/// <summary>
		/// Whether to compile Project.dll, Project.Client.dll and C# scripts before commit to the server.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CompileScriptsBeforeCommit
		{
			get { if( _compileScriptsBeforeCommit.BeginGet() ) CompileScriptsBeforeCommit = _compileScriptsBeforeCommit.Get( this ); return _compileScriptsBeforeCommit.value; }
			set { if( _compileScriptsBeforeCommit.BeginSet( this, ref value ) ) { try { CompileScriptsBeforeCommitChanged?.Invoke( this ); } finally { _compileScriptsBeforeCommit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CompileScriptsBeforeCommit"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> CompileScriptsBeforeCommitChanged;
		ReferenceField<bool> _compileScriptsBeforeCommit = true;

		//!!!!impl
		/// <summary>
		/// Whether to stop the project server before commit starts and run when it finished.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> StopServerDuringCommit
		{
			get { if( _stopServerDuringCommit.BeginGet() ) StopServerDuringCommit = _stopServerDuringCommit.Get( this ); return _stopServerDuringCommit.value; }
			set { if( _stopServerDuringCommit.BeginSet( this, ref value ) ) { try { StopServerDuringCommitChanged?.Invoke( this ); } finally { _stopServerDuringCommit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StopServerDuringCommit"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> StopServerDuringCommitChanged;
		ReferenceField<bool> _stopServerDuringCommit = true;

		public const string SkipPathsDefault = "Project\\Caches\\Files\r\nProject\\Caches\\ShaderCache\r\nProject\\User settings";

		/// <summary>
		/// The list of folders that are not synced with the server, both for editor and play. Items are separated by return or semicolon.
		/// </summary>
		[DefaultValue( SkipPathsDefault )]
		[Category( "Repository" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> SkipPaths
		{
			get { if( _skipPaths.BeginGet() ) SkipPaths = _skipPaths.Get( this ); return _skipPaths.value; }
			set { if( _skipPaths.BeginSet( this, ref value ) ) { try { SkipPathsChanged?.Invoke( this ); } finally { _skipPaths.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkipPaths"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> SkipPathsChanged;
		ReferenceField<string> _skipPaths = SkipPathsDefault;

		public const string SkipFoldersWithNameDefault = "obj";

		/// <summary>
		/// The list of folder names that are not synced with the server, both for editor and play. Items are separated by return or semicolon.
		/// </summary>
		[DefaultValue( SkipFoldersWithNameDefault )]
		[Category( "Repository" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> SkipFoldersWithName
		{
			get { if( _skipFoldersWithName.BeginGet() ) SkipFoldersWithName = _skipFoldersWithName.Get( this ); return _skipFoldersWithName.value; }
			set { if( _skipFoldersWithName.BeginSet( this, ref value ) ) { try { SkipFoldersWithNameChanged?.Invoke( this ); } finally { _skipFoldersWithName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkipFoldersWithName"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> SkipFoldersWithNameChanged;
		ReferenceField<string> _skipFoldersWithName = SkipFoldersWithNameDefault;

		public const string SkipFilesWithExtensionWhenPlayDefault = "blend;blend1;bin;cs";

		/// <summary>
		/// The list of file extensions that are not sent to players. Items are separated by return or semicolon.
		/// </summary>
		[DefaultValue( SkipFilesWithExtensionWhenPlayDefault )]
		[Category( "Play" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> SkipFilesWithExtensionWhenPlay
		{
			get { if( _skipFilesWithExtensionWhenPlay.BeginGet() ) SkipFilesWithExtensionWhenPlay = _skipFilesWithExtensionWhenPlay.Get( this ); return _skipFilesWithExtensionWhenPlay.value; }
			set { if( _skipFilesWithExtensionWhenPlay.BeginSet( this, ref value ) ) { try { SkipFilesWithExtensionWhenPlayChanged?.Invoke( this ); } finally { _skipFilesWithExtensionWhenPlay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkipFilesWithExtensionWhenPlay"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> SkipFilesWithExtensionWhenPlayChanged;
		ReferenceField<string> _skipFilesWithExtensionWhenPlay = SkipFilesWithExtensionWhenPlayDefault;

		public const string ClearFilesWithExtensionWhenPlayDefault = "fbx;3d;3ds;ac;ac3d;acc;ase;ask;b3d;bvh;cob;csm;dae;dxf;enff;hmp;ifc;lwo;lws;lxo;mot;ms3d;ndo;nff;obj;off;pk3;ply;x;q3d;q3s;gltf;glb";

		/// <summary>
		/// The list of file extensions to clear when send to players. Items are separated by return or semicolon. Clearing of files is used for source 3D models, because the actual data of 3D models is stored in settings files. It is enough to save empty original files.
		/// </summary>
		[DefaultValue( ClearFilesWithExtensionWhenPlayDefault )]
		[Category( "Play" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> ClearFilesWithExtensionWhenPlay
		{
			get { if( _clearFilesWithExtensionWhenPlay.BeginGet() ) ClearFilesWithExtensionWhenPlay = _clearFilesWithExtensionWhenPlay.Get( this ); return _clearFilesWithExtensionWhenPlay.value; }
			set { if( _clearFilesWithExtensionWhenPlay.BeginSet( this, ref value ) ) { try { ClearFilesWithExtensionWhenPlayChanged?.Invoke( this ); } finally { _clearFilesWithExtensionWhenPlay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClearFilesWithExtensionWhenPlay"/> property value changes.</summary>
		public event Action<ProjectSettingsPage_Repository> ClearFilesWithExtensionWhenPlayChanged;
		ReferenceField<string> _clearFilesWithExtensionWhenPlay = ClearFilesWithExtensionWhenPlayDefault;
	}
}
