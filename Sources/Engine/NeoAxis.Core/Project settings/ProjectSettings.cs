// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		static Component_ProjectSettings settingsComponent;

		//

		public static string FileName
		{
			get { return "Base\\ProjectSettings.component"; }
		}

		public static Component_ProjectSettings.ThemeEnum DefaultThemeWhenNoFile = Component_ProjectSettings.ThemeEnum.Dark;

		/// <summary>
		/// Gets instance of the settings.
		/// </summary>
		public static Component_ProjectSettings Get
		{
			get
			{
				if( settingsComponent == null )
				{
					//create default settings
					if( !VirtualFile.Exists( FileName ) )
					{
						var settingsComponent2 = ComponentUtility.CreateComponent<Component_ProjectSettings>( null, true, true );
						settingsComponent2.Name = "Root";

						//default theme
						settingsComponent2.Theme = DefaultThemeWhenNoFile;

						//detect language
						try
						{
							var ci = CultureInfo.InstalledUICulture;

							if( ci.EnglishName.Contains( "Russian" ) )
								settingsComponent2.Language = Component_ProjectSettings.LanguageEnum.Russian;

						}
						catch { }

						var pageNames = new List<string>();
						pageNames.Add( "General" );
						pageNames.Add( "Scene Editor" );
						pageNames.Add( "UI Editor" );
						pageNames.Add( "C# Editor" );
						pageNames.Add( "Shader Editor" );
						pageNames.Add( "Text Editor" );
						pageNames.Add( "Ribbon and Toolbar" );
						pageNames.Add( "Shortcuts" );
						pageNames.Add( "Custom Splash Screen" );

						foreach( var name in pageNames )
						{
							var page = settingsComponent2.CreateComponent<Component_ProjectSettings_PageBasic>();
							page.Name = name;
						}

						var realFileName = VirtualPathUtility.GetRealPathByVirtual( FileName );
						if( !ComponentUtility.SaveComponentToFile( settingsComponent2, realFileName, null, out var error ) )
							Log.Warning( "Unable to write project settings file. " + error );

						settingsComponent2.Dispose();
					}

					//load
					if( VirtualFile.Exists( FileName ) )
					{
						try
						{
							settingsComponent = ResourceManager.LoadResource<Component_ProjectSettings>( FileName );
						}
						catch( Exception )
						{
							settingsComponent = ComponentUtility.CreateComponent<Component_ProjectSettings>( null, true, true );
						}
					}
					else
						settingsComponent = ComponentUtility.CreateComponent<Component_ProjectSettings>( null, true, true );
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
