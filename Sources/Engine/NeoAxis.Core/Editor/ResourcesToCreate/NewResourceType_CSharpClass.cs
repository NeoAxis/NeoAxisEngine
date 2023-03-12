#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	[NewObjectSettings( typeof( NewObjectSettingsCSharpClass ) )]
	[ResourceFileExtension( "cs" )]
	[NewObjectDefaultName( "MyClass" )]
	public class NewResourceType_CSharpClass : NewResourceType
	{
		/// <summary>
		/// A set of settings for creation <see cref="NewResourceType_CSharpClass"/> in the editor.
		/// </summary>
		public class NewObjectSettingsCSharpClass : NewObjectSettings
		{
			[DefaultValue( "NeoAxis.Component" )]
			[Category( "Options" )]
			[DisplayName( "Base class" )]
			public string BaseClass { get; set; } = "NeoAxis.Component";

			[DefaultValue( true )]
			[Category( "Options" )]
			[DisplayName( "Add to Project.csproj" )]
			public bool AddToProjectCsproj { get; set; } = true;

			[DefaultValue( false )]
			[Category( "Options" )]
			[DisplayName( "Add example properties" )]
			public bool AddExampleProperties { get; set; } = false;

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				string code = @"using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;
using NeoAxis.Editor;

namespace Project
{
	public class {Name}{BasedOnComponentClass}
	{
{Body}
	}
}";

				//{Name}
				var className = Path.GetFileNameWithoutExtension( context.fileCreationRealFileName );
				code = code.Replace( "{Name}", className );

				//{BasedOnComponentClass}
				if( !string.IsNullOrEmpty( BaseClass ) )
					code = code.Replace( "{BasedOnComponentClass}", " : " + BaseClass );
				else
					code = code.Replace( "{BasedOnComponentClass}", "" );

				//{Body}
				{
					var body = new List<string>();

					if( AddExampleProperties )
					{
						body.Add( "[DefaultValue( 1 )]" );
						body.Add( "[Range( 0, 2 )]" );
						body.Add( "public double Power { get; set; } = 1;" );
						body.Add( "" );
						body.Add( "[DefaultValue( \"1 1 1\" )]" );
						body.Add( "public ColorValue Color { get; set; } = new ColorValue( 1, 1, 1 );" );

						//!!!!пример ссылки на компоненту
						//!!!!пример ссылки на файл
					}

					var isComponent = false;
					if( !string.IsNullOrEmpty( BaseClass ) )
					{
						if( BaseClass == "NeoAxis.Component" )
							isComponent = true;
						var type = MetadataManager.GetType( BaseClass );
						if( type != null && typeof( Component ).IsAssignableFrom( type.GetNetType() ) )
							isComponent = true;
						var type2 = MetadataManager.GetType( "NeoAxis." + BaseClass );
						if( type2 != null && typeof( Component ).IsAssignableFrom( type2.GetNetType() ) )
							isComponent = true;
					}

					if( isComponent )
					{
						if( body.Count != 0 )
							body.Add( "" );

						body.Add( "protected override void OnEnabledInSimulation()" );
						body.Add( "{" );
						body.Add( "}" );
						body.Add( "" );

						body.Add( "protected override void OnUpdate( float delta )" );
						body.Add( "{" );
						body.Add( "}" );
						body.Add( "" );

						body.Add( "protected override void OnSimulationStep()" );
						body.Add( "{" );
						body.Add( "}" );
					}

					var bodyStr = "";
					foreach( var line in body )
					{
						if( bodyStr != "" )
							bodyStr += "\r\n";
						bodyStr += "\t\t" + line;
					}
					code = code.Replace( "{Body}", bodyStr );
				}

				//write file
				try
				{
					File.WriteAllText( context.fileCreationRealFileName, code );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return false;
				}

				//add to Project.csproj
				if( AddToProjectCsproj )
				{
					var toAdd = new List<string>();

					var fileName = VirtualPathUtility.GetProjectPathByReal( context.fileCreationRealFileName );
					//var fileName = Path.Combine( "Assets", VirtualPathUtility.GetVirtualPathByReal( context.fileCreationRealFileName ) );
					toAdd.Add( fileName );

					if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
					{
						if( toAdd.Count > 1 )
							Log.Info( EditorLocalization.Translate( "General", "Items have been added to the Project.csproj." ) );
						else
							Log.Info( EditorLocalization.Translate( "General", "The item has been added to the Project.csproj." ) );
					}
					else
						Log.Warning( error );
				}

				return true;
			}
		}
	}
}
#endif