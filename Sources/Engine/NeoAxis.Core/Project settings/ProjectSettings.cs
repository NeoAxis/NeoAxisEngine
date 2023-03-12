// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NeoAxis
{
	/// <summary>
	/// Represents engine project settings.
	/// </summary>
	public static class ProjectSettings
	{
		static ProjectSettingsComponent settingsComponent;

		//

		public static string FileName
		{
			get { return "Base\\ProjectSettings.component"; }
		}

		public static ProjectSettingsPage_General.ThemeEnum DefaultThemeWhenNoFile = ProjectSettingsPage_General.ThemeEnum.Dark;

		public static bool WriteDefaultProjectSettingsFile( string realFileName, string autorunScene, out string error )
		{
			error = "";

			var fileBlock = new TextBlock();

			var rootBlock = fileBlock.AddChild( ".component", "NeoAxis.ProjectSettingsComponent" );
			rootBlock.SetAttribute( "Name", "Root" );

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_General" );
				block.SetAttribute( "Name", "General" );

				//default theme
				block.SetAttribute( "Theme", DefaultThemeWhenNoFile.ToString() );

				//detect language
				try
				{
					var ci = CultureInfo.InstalledUICulture;

					if( ci.EnglishName.Contains( "Russian" ) )
						block.SetAttribute( "Language", ProjectSettingsPage_General.LanguageEnum.Russian.ToString() );
				}
				catch { }

				if( !string.IsNullOrEmpty( autorunScene ) )
				{
					var b = block.AddChild( "AutorunScene" );
					b.SetAttribute( "GetByReference", autorunScene );
				}
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_SceneEditor" );
				block.SetAttribute( "Name", "Scene Editor" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_UIEditor" );
				block.SetAttribute( "Name", "UI Editor" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_CSharpEditor" );
				block.SetAttribute( "Name", "C# Editor" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_ShaderEditor" );
				block.SetAttribute( "Name", "Shader Editor" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_TextEditor" );
				block.SetAttribute( "Name", "Text Editor" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_Colors" );
				block.SetAttribute( "Name", "Colors" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_Preview" );
				block.SetAttribute( "Name", "Preview" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_RibbonAndToolbar" );
				block.SetAttribute( "Name", "Ribbon and Toolbar" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_Shortcuts" );
				block.SetAttribute( "Name", "Shortcuts" );
			}

			{
				var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_Rendering" );
				block.SetAttribute( "Name", "Rendering" );
			}

			//{
			//	var block = rootBlock.AddChild( ".component", "NeoAxis.ProjectSettingsPage_CustomSplashScreen" );
			//	block.SetAttribute( "Name", "Custom Splash Screen" );
			//}

			if( !TextBlockUtility.SaveToRealFile( fileBlock, realFileName, out error ) )
				return false;

			////var realFileName = VirtualPathUtility.GetRealPathByVirtual( FileName );
			//if( !ComponentUtility.SaveComponentToFile( settingsComponent2, realFileName, null, out error ) )
			//{
			//	return false;
			//	//Log.Warning( "Unable to write project settings file. " + error );
			//}

			return true;
		}

		/// <summary>
		/// Gets instance of the settings.
		/// </summary>
		public static ProjectSettingsComponent Get
		{
			get
			{
				if( settingsComponent == null )
				{
					//create default settings
					if( !VirtualFile.Exists( FileName ) )
					{
						var realFileName = VirtualPathUtility.GetRealPathByVirtual( FileName );
						if( !WriteDefaultProjectSettingsFile( realFileName, "", out var error ) )
						{
						}

						//var settingsComponent2 = ComponentUtility.CreateComponent<ProjectSettingsComponent>( null, true, true );
						//settingsComponent2.Name = "Root";

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_General>();
						//	page.Name = "General";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_SceneEditor>();
						//	page.Name = "Scene Editor";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_UIEditor>();
						//	page.Name = "UI Editor";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_CSharpEditor>();
						//	page.Name = "C# Editor";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_ShaderEditor>();
						//	page.Name = "Shader Editor";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_TextEditor>();
						//	page.Name = "Text Editor";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_RibbonAndToolbar>();
						//	page.Name = "Ribbon and Toolbar";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_Shortcuts>();
						//	page.Name = "Shortcuts";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_Rendering>();
						//	page.Name = "Rendering";
						//}

						//{
						//	var page = settingsComponent2.CreateComponent<ProjectSettingsPage_CustomSplashScreen>();
						//	page.Name = "Custom Splash Screen";
						//}


						////default theme
						//settingsComponent2.General.Theme = DefaultThemeWhenNoFile;

						////detect language
						//try
						//{
						//	var ci = CultureInfo.InstalledUICulture;

						//	if( ci.EnglishName.Contains( "Russian" ) )
						//		settingsComponent2.General.Language = ProjectSettingsPage_General.LanguageEnum.Russian;
						//}
						//catch { }


						//var realFileName = VirtualPathUtility.GetRealPathByVirtual( FileName );
						//if( !ComponentUtility.SaveComponentToFile( settingsComponent2, realFileName, null, out var error ) )
						//{
						//	//Log.Warning( "Unable to write project settings file. " + error );
						//}

						//settingsComponent2.Dispose();
					}

					//load
					if( VirtualFile.Exists( FileName ) )
					{
						try
						{
							settingsComponent = ResourceManager.LoadResource<ProjectSettingsComponent>( FileName );
						}
						catch( Exception )
						{
							settingsComponent = ComponentUtility.CreateComponent<ProjectSettingsComponent>( null, true, true );
						}
					}
					else
						settingsComponent = ComponentUtility.CreateComponent<ProjectSettingsComponent>( null, true, true );
				}

				return settingsComponent;
			}
		}

		internal static void _SettingsComponentSetNull()
		{
			settingsComponent = null;
		}

		public static void SaveToFileAndUpdate()
		{
			try
			{
				var fileName = ComponentUtility.GetOwnedFileNameOfComponent( Get );
				var realPath = VirtualPathUtility.GetRealPathByVirtual( fileName );

				string error;
				if( ComponentUtility.SaveComponentToFile( Get, realPath, null, out error ) )
				{
					//return true;
				}
				else
				{
					//Log.Error( error );
					//return false;
				}

				//reload
				_SettingsComponentSetNull();

			}
			catch { }
		}

		/// <summary>
		/// Reads a value from \'ProjectSettings.component\' without loading it as component. The method can be used to load data before engine initialized.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static string ReadParameterFromFile( string parameter, string defaultValue = "" )
		{
			if( VirtualFile.Exists( FileName ) )
			{
				try
				{
					var key = parameter + " = ";

					foreach( var line in VirtualFile.ReadAllLines( FileName ) )
					{
						var index = line.IndexOf( key );
						if( index != -1 )
							return line.Substring( index + key.Length );
					}
				}
				catch { }
			}
			return defaultValue;
		}
	}
}
