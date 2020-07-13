// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using NeoAxis.Editor;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace NeoAxis
{
	public partial class Component_Scene
	{
		/// <summary>
		/// A set of settings for creation <see cref="Component_Scene"/> in the editor.
		/// </summary>
		public class NewObjectSettingsScene : NewObjectSettings, Metadata.IMetadataProvider
		{
			static List<TemplateClass> templates = new List<TemplateClass>();

			//[Category( "Options" )]
			//public ModeEnum Mode { get; set; } = ModeEnum._3D;

			[Editor( typeof( HCItemNewSceneTemplates ), typeof( object ) )]
			[Category( "Options" )]
			public TemplateClass Template { get; set; }

			/////////////////////////////////////

			/// <summary>
			/// Represents a template of a new scene type.
			/// </summary>
			public sealed class TemplateClass
			{
				public string Name { get; }
				public Image Preview { get; }

				public TemplateClass( string name, Image preview )
				{
					Name = name;
					Preview = preview;
				}

				public override string ToString()
				{
					return Name;
				}
			}

			/////////////////////////////////////

			public NewObjectSettingsScene()
			{
				// default select first template.
				Template = GetTemplates().FirstOrDefault();
			}

			public static List<TemplateClass> GetTemplates()
			{
				//init templates
				if( templates.Count == 0 )
				{
					var templatesPath = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\" );
					if( Directory.Exists( templatesPath ) )
					{
						string[] filePaths = Directory.GetFiles( templatesPath, "*.scene", SearchOption.AllDirectories );
						CollectionUtility.InsertionSort( filePaths, string.Compare );

						foreach( var path in filePaths )
						{
							var name = Path.GetFileNameWithoutExtension( path );
							var previewPath = Path.Combine( Path.GetDirectoryName( path ), name + ".png" );
							var preview = File.Exists( previewPath ) ? Image.FromFile( previewPath ) : null;
							templates.Add( new TemplateClass( name, preview ) );
						}
					}
				}

				return templates;
			}

			public override bool ReadyToCreate( out string reason )
			{
				if( Template == null )
				{
					reason = "No template selected.";
					return false;
				}

				return base.ReadyToCreate( out reason );
			}

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				if( Window.IsFileCreation() )
				{
					context.disableFileCreation = true;

					GetCreateCSharpClassInfo( out var csharpRealFileName, out var csharpClassName, out var csharpClassNameWithoutNamespace );

					try
					{
						//main file
						{
							string name = Template.Name + ".scene";
							var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\" + name );

							var text = VirtualFile.ReadAllText( sourceFile );

							if( CreateCSharpClass )
								text = text.Replace( ".component NeoAxis.Component_Scene", ".component " + csharpClassName );

							File.WriteAllText( context.fileCreationRealFileName, text );
						}

						//cs file
						if( CreateCSharpClass )
						{
							string code = @"using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class {Name} : {Base}
	{
	}
}";

							code = code.Replace( "{Name}", csharpClassNameWithoutNamespace );
							code = code.Replace( "{Base}", Window.SelectedType.Name );

							File.WriteAllText( csharpRealFileName, code );
						}
					}
					catch( Exception e )
					{
						EditorMessageBox.ShowWarning( e.Message );
						//Log.Warning( e.Message );
						return false;
					}

					if( CreateCSharpClass )
					{
						//add to Project.csproj
						{
							var toAdd = new List<string>();

							var fileName = Path.Combine( "Assets", VirtualPathUtility.GetVirtualPathByReal( csharpRealFileName ) );
							toAdd.Add( fileName );

							if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
							{
								if( toAdd.Count > 1 )
									Log.Info( EditorLocalization.Translate( "General", "Items have been added to the Project.csproj." ) );
								else
									Log.Info( EditorLocalization.Translate( "General", "The item has been added to the Project.csproj." ) );
							}
							else
							{
								EditorMessageBox.ShowWarning( error );
								//Log.Warning( error );
								return false;
							}
						}

						Window.DisableUnableToCreateReason = true;

						//restart application
						var text = EditorLocalization.Translate( "General", "To apply changes need restart the editor. Restart?\r\n\r\nThe editor must be restarted to compile and enable a new created C# class." );
						if( EditorMessageBox.ShowQuestion( text, MessageBoxButtons.YesNo ) == DialogResult.Yes )
							EditorAPI.BeginRestartApplication();

						Window.DisableUnableToCreateReason = false;
					}
				}

				return true;
			}

			//void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			//{
			//	var p = member as Metadata.Property;
			//	if( p != null )
			//	{
			//		switch( p.Name )
			//		{
			//		//case nameof( Mode ):
			//		//	if( !Window.IsFileCreation() )
			//		//		skip = true;
			//		//	break;

			//		}
			//	}
			//}
		}
	}
}
