// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Component representing the settings of the project.
	/// </summary>
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.ObjectSettingsWindow" )]
#endif
	public class ProjectSettingsComponent : Component
	{
		public static ESet<string> HidePropertiesForSpecialAppMode = new ESet<string>();

		//

		protected override void OnDispose()
		{
			base.OnDispose();

			ProjectSettings._SettingsComponentSetNull();
		}

		[Browsable( false )]
		public bool UserMode
		{
			get
			{
#if !DEPLOY
				if( EngineApp.IsEditor )
				{
					var document = EditorAPI.GetDocumentByComponent( this );
					if( document != null && document.SpecialMode == "ProjectSettingsUserMode" )
						return true;
				}
#endif
				return false;
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					if( member.Name == "Name" || member.Name == "Enabled" || member.Name == "ScreenLabel" || member.Name == "NetworkMode" )
						skip = true;
				}
			}
		}

		/////////////////////////////////////////

		[Browsable( false )]
		public ProjectSettingsPage_General General
		{
			get
			{
				if( general == null )
					general = GetComponent<ProjectSettingsPage_General>();
				return general ?? new ProjectSettingsPage_General();
			}
		}
		ProjectSettingsPage_General general;

		[Browsable( false )]
		public ProjectSettingsPage_SceneEditor SceneEditor
		{
			get
			{
				if( sceneEditor == null )
					sceneEditor = GetComponent<ProjectSettingsPage_SceneEditor>();
				return sceneEditor ?? new ProjectSettingsPage_SceneEditor();
			}
		}
		ProjectSettingsPage_SceneEditor sceneEditor;

		[Browsable( false )]
		public ProjectSettingsPage_UIEditor UIEditor
		{
			get
			{
				if( uiEditor == null )
					uiEditor = GetComponent<ProjectSettingsPage_UIEditor>();
				return uiEditor ?? new ProjectSettingsPage_UIEditor();
			}
		}
		ProjectSettingsPage_UIEditor uiEditor;

		[Browsable( false )]
		public ProjectSettingsPage_CSharpEditor CSharpEditor
		{
			get
			{
				if( cSharpEditor == null )
					cSharpEditor = GetComponent<ProjectSettingsPage_CSharpEditor>();
				return cSharpEditor ?? new ProjectSettingsPage_CSharpEditor();
			}
		}
		ProjectSettingsPage_CSharpEditor cSharpEditor;

		[Browsable( false )]
		public ProjectSettingsPage_ShaderEditor ShaderEditor
		{
			get
			{
				if( shaderEditor == null )
					shaderEditor = GetComponent<ProjectSettingsPage_ShaderEditor>();
				return shaderEditor ?? new ProjectSettingsPage_ShaderEditor();
			}
		}
		ProjectSettingsPage_ShaderEditor shaderEditor;

		[Browsable( false )]
		public ProjectSettingsPage_TextEditor TextEditor
		{
			get
			{
				if( textEditor == null )
					textEditor = GetComponent<ProjectSettingsPage_TextEditor>();
				return textEditor ?? new ProjectSettingsPage_TextEditor();
			}
		}
		ProjectSettingsPage_TextEditor textEditor;

		[Browsable( false )]
		public ProjectSettingsPage_Colors Colors
		{
			get
			{
				if( colors == null )
					colors = GetComponent<ProjectSettingsPage_Colors>();
				return colors ?? new ProjectSettingsPage_Colors();
			}
		}
		ProjectSettingsPage_Colors colors;

		[Browsable( false )]
		public ProjectSettingsPage_Preview Preview
		{
			get
			{
				if( preview == null )
					preview = GetComponent<ProjectSettingsPage_Preview>();
				return preview ?? new ProjectSettingsPage_Preview();
			}
		}
		ProjectSettingsPage_Preview preview;

		[Browsable( false )]
		public ProjectSettingsPage_RibbonAndToolbar RibbonAndToolbar
		{
			get
			{
				if( ribbonAndToolbar == null )
					ribbonAndToolbar = GetComponent<ProjectSettingsPage_RibbonAndToolbar>();
				return ribbonAndToolbar ?? new ProjectSettingsPage_RibbonAndToolbar();
			}
		}
		ProjectSettingsPage_RibbonAndToolbar ribbonAndToolbar;


		[Browsable( false )]
		public ProjectSettingsPage_Shortcuts Shortcuts
		{
			get
			{
				if( shortcuts == null )
					shortcuts = GetComponent<ProjectSettingsPage_Shortcuts>();
				return shortcuts ?? new ProjectSettingsPage_Shortcuts();
			}
		}
		ProjectSettingsPage_Shortcuts shortcuts;

		[Browsable( false )]
		public ProjectSettingsPage_Rendering Rendering
		{
			get
			{
				if( rendering == null )
					rendering = GetComponent<ProjectSettingsPage_Rendering>();
				return rendering ?? new ProjectSettingsPage_Rendering();
			}
		}
		ProjectSettingsPage_Rendering rendering;

		[Browsable( false )]
		public ProjectSettingsPage_Repository Repository
		{
			get
			{
				if( repository == null )
					repository = GetComponent<ProjectSettingsPage_Repository>();
				return repository ?? new ProjectSettingsPage_Repository();
			}
		}
		ProjectSettingsPage_Repository repository;
	}
}
