// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

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

		/// <summary>
		/// Gets instance of the settings.
		/// </summary>
		public static Component_ProjectSettings Get
		{
			get
			{
				//!!!!

				//теперь в OnDispose очищается
				//if( settingsComponent != null && settingsComponent.Disposed )
				//	settingsComponent = null;

				if( settingsComponent == null )
				{
					if( VirtualFile.Exists( FileName ) )
					{
						try//!!!!new
						{
							//!!!!или separate
							settingsComponent = ResourceManager.LoadResource<Component_ProjectSettings>( FileName );
						}
						catch( Exception )
						{
							settingsComponent = ComponentUtility.CreateComponent<Component_ProjectSettings>( null, true, true );
						}
					}
					else
					{
						//!!!!default

						settingsComponent = ComponentUtility.CreateComponent<Component_ProjectSettings>( null, true, true );

						//!!!!!need save
					}
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
		public static string ReadParameterFromFile( string parameter )
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
			return "";
		}
	}
}
