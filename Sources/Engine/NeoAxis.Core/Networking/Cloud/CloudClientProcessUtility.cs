// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NeoAxis.Networking
{
	public static class CloudClientProcessUtility
	{
		static bool? initialized;
		static long projectID;
		static string appDirectory;
		static string loginForSecureMode;
		static string verificationCodeForSecureMode;
		static string projectCurrency;

		///////////////////////////////////////////////

		public static string CloudDataDirectory
		{
			get { return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), EngineInfo.CloudServiceName ); }
		}

		static bool GetProjectAppAndAppDirectoryFromExecutable()
		{
			try
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					var appsDirectory = Path.Combine( CloudDataDirectory, "Apps" );
					var location = Assembly.GetExecutingAssembly().Location;

					if( location.Contains( appsDirectory ) )
					{
						var subPath = location.Substring( appsDirectory.Length );
						var split = subPath.Split( Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );

						if( split.Length > 0 && long.TryParse( split[ 0 ], out var projectID2 ) )
						{
							var appDirectory2 = Path.Combine( appsDirectory, projectID2.ToString() );
							if( location.Contains( appDirectory2 ) )
							{
								projectID = projectID2;
								appDirectory = appDirectory2;
							}
						}
					}
				}
				else
				{
					Log.Fatal( "CloudClientProcessUtility: GetProjectAppAndAppDirectoryFromExecutable: No implementation." );
					return false;
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		static bool GetProjectAppAndAppDirectoryFromCommandLineParameters()
		{
			try
			{
				//get projectID
				if( SystemSettings.CommandLineParameters.TryGetValue( "-projectID", out var projectIDString ) )
					if( long.TryParse( projectIDString, out var projectID2 ) )
						projectID = projectID2;

				//get appDirectory
				if( SystemSettings.CommandLineParameters.TryGetValue( "-appDirectory", out var appDirectory2 ) )
					appDirectory = appDirectory2;

				//get loginForSecureMode
				if( SystemSettings.CommandLineParameters.TryGetValue( "-loginForSecureMode", out var loginForSecureMode2 ) )
					loginForSecureMode = loginForSecureMode2;

				//get verificationCodeForSecureMode
				if( SystemSettings.CommandLineParameters.TryGetValue( "-verificationCodeForSecureMode", out var verificationCodeForSecureMode2 ) )
					verificationCodeForSecureMode = verificationCodeForSecureMode2;

				//get projectCurrency
				if( SystemSettings.CommandLineParameters.TryGetValue( "-projectCurrency", out var projectCurrency2 ) )
					projectCurrency = projectCurrency2;

				return projectID != 0 && !string.IsNullOrEmpty( appDirectory );
			}
			catch
			{
				return false;
			}
		}

		static void Init()
		{
			if( initialized == null )
			{
				if( GetProjectAppAndAppDirectoryFromCommandLineParameters() )
					initialized = true;
				else if( GetProjectAppAndAppDirectoryFromExecutable() )
					initialized = true;
				else
					initialized = false;
			}
		}

		public static long ProjectID
		{
			get
			{
				Init();
				return projectID;
			}
		}

		public static string AppDirectory
		{
			get
			{
				Init();
				return appDirectory;
			}
		}

		public static string LoginForSecureMode
		{
			get
			{
				Init();
				return loginForSecureMode;
			}
		}

		public static string VerificationCodeForSecureMode
		{
			get
			{
				Init();
				return verificationCodeForSecureMode;
			}
		}

		public static string ProjectCurrency
		{
			get
			{
				Init();
				return projectCurrency;
			}
		}
	}
}