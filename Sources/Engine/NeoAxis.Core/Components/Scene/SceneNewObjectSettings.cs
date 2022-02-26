// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using NeoAxis.Editor;
using System.Drawing;
using System.Linq;

namespace NeoAxis
{
	public partial class Scene
	{
		/// <summary>
		/// A set of settings for creation <see cref="Scene"/> in the editor.
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

					Window.DisableUnableToCreateReason = true;
					try
					{

						try
						{
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
									return false;
								}
							}

							EditorAPI.BuildProjectSolution( false );
						}

						try
						{
							//main file
							{
								string name = Template.Name + ".scene";
								var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\" + name );

								//copy scene file

								var text = File.ReadAllText( sourceFile );

								if( CreateCSharpClass )
									text = text.Replace( ".component NeoAxis.Scene", ".component " + csharpClassName );

								File.WriteAllText( context.fileCreationRealFileName, text );

								//copy additional folder if exist
								var sourceFolderPath = sourceFile + "_Files";
								if( Directory.Exists( sourceFolderPath ) )
								{
									var destFolderPath = context.fileCreationRealFileName + "_Files";
									IOUtility.CopyDirectory( sourceFolderPath, destFolderPath );
								}
							}
						}
						catch( Exception e )
						{
							EditorMessageBox.ShowWarning( e.Message );
							return false;
						}

					}
					finally
					{
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
