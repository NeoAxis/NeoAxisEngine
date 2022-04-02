// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
						var settingsComponent2 = ComponentUtility.CreateComponent<ProjectSettingsComponent>( null, true, true );
						settingsComponent2.Name = "Root";

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_General>();
							page.Name = "General";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_SceneEditor>();
							page.Name = "Scene Editor";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_UIEditor>();
							page.Name = "UI Editor";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_CSharpEditor>();
							page.Name = "C# Editor";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_ShaderEditor>();
							page.Name = "Shader Editor";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_TextEditor>();
							page.Name = "Text Editor";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_RibbonAndToolbar>();
							page.Name = "Ribbon and Toolbar";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_Shortcuts>();
							page.Name = "Shortcuts";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_Rendering>();
							page.Name = "Rendering";
						}

						{
							var page = settingsComponent2.CreateComponent<ProjectSettingsPage_CustomSplashScreen>();
							page.Name = "Custom Splash Screen";
						}


						//default theme
						settingsComponent2.General.Theme = DefaultThemeWhenNoFile;

						//detect language
						try
						{
							var ci = CultureInfo.InstalledUICulture;

							if( ci.EnglishName.Contains( "Russian" ) )
								settingsComponent2.General.Language = ProjectSettingsPage_General.LanguageEnum.Russian;
						}
						catch { }


						var realFileName = VirtualPathUtility.GetRealPathByVirtual( FileName );
						if( !ComponentUtility.SaveComponentToFile( settingsComponent2, realFileName, null, out var error ) )
						{
							//Log.Warning( "Unable to write project settings file. " + error );
						}

						settingsComponent2.Dispose();
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
