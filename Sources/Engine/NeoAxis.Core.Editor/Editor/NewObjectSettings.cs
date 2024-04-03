#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace NeoAxis.Editor
{
	public class NewObjectSettings : Metadata.IMetadataProvider
	{
		NewObjectWindow window;
		//bool fileCreation;

		//[Browsable( false )]
		//public bool FileCreation
		//{
		//	get { return fileCreation; }
		//}

		public virtual bool Init( NewObjectWindow window )// bool fileCreation )
		{
			this.window = window;
			//this.fileCreation = fileCreation;
			return true;
		}

		[Browsable( false )]
		public NewObjectWindow Window
		{
			get { return window; }
		}

		public void GetCreateCSharpClassInfo( out string realFileName, out string className, out string csharpClassNameWithoutNamespace )
		{
			realFileName = Path.ChangeExtension( Window.GetFileCreationRealFileName(), "cs" );
			className = "Project." + Path.GetFileNameWithoutExtension( Window.GetFileCreationRealFileName() );
			csharpClassNameWithoutNamespace = Path.GetFileNameWithoutExtension( Window.GetFileCreationRealFileName() );
		}

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "NewObjectWindow", text );
		}

		public virtual bool ReadyToCreate( out string reason )
		{
			//CreateCSharpClass. check cs file already exists.
			if( Window.IsFileCreation() && CreateCSharpClass )
			{
				GetCreateCSharpClassInfo( out var realFileName, out _, out _ );
				if( File.Exists( realFileName ) )
				{
					reason = string.Format( Translate( "A file with the name \'{0}\' already exists." ), Path.GetFileName( realFileName ) );
					return false;
				}
			}

			reason = "";
			return true;
		}

		public virtual bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			if( Window.IsFileCreation() && CreateCSharpClass )
			{
				context.disableFileCreation = true;

				GetCreateCSharpClassInfo( out var csharpRealFileName, out var csharpClassName, out var csharpClassNameWithoutNamespace );

				Window.DisableUnableToCreateReason = true;
				try
				{

					try
					{
						//cs file
						//if( CreateCSharpClass )
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

					//if( CreateCSharpClass )
					{
						//add to Project.csproj
						{
							var toAdd = new List<string>();

							var fileName = VirtualPathUtility.GetProjectPathByReal( csharpRealFileName );
							//var fileName = Path.Combine( "Assets", VirtualPathUtility.GetVirtualPathByReal( csharpRealFileName ) );
							toAdd.Add( fileName );

							if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error ) )
							{
								if( toAdd.Count > 1 )
									Log.Info( EditorLocalization2.Translate( "General", "Items have been added to the Project.csproj." ) );
								else
									Log.Info( EditorLocalization2.Translate( "General", "The item has been added to the Project.csproj." ) );
							}
							else
							{
								EditorMessageBox.ShowWarning( error );
								return false;
							}
						}

						EditorAPI2.BuildProjectSolution( false );
					}

					try
					{
						//main file
						{
							string className = csharpClassName;
							//string className = CreateCSharpClass ? csharpClassName : Window.SelectedType.Name;
							var text = ".component " + className + "\r\n{\r\n}";

							File.WriteAllText( context.fileCreationRealFileName, text );
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
		//public abstract bool Creation( NewObjectCell.ObjectCreationContext context );
		//public abstract bool Creation( object newObject, ref bool disableFileWriting );

		/// <summary>
		/// A base C# class will be created for the resource.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Options" )]
		[DisplayName( "Create C# class" )]
		public bool CreateCSharpClass { get; set; }

		[Browsable( false )]
		public Metadata.TypeInfo BaseType
		{
			get { return MetadataManager.GetTypeOfNetType( GetType() ); }
		}

		protected virtual void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( CreateCSharpClass ):
					var type = Window.SelectedType;
					if( !Window.IsFileCreation() || type == null || !typeof( Component ).IsAssignableFrom( type.GetNetType() ) || typeof( CSharpScript ).IsAssignableFrom( type.GetNetType() ) )
						skip = true;
					break;
				}
			}
		}

		public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
		{
			var members = new List<Metadata.Member>( BaseType.MetadataGetMembers( context ) );

			//move CreateCSharpClass to bottom
			{
				var member = BaseType.MetadataGetMemberBySignature( "property:" + nameof( CreateCSharpClass ) );
				if( member != null )
				{
					if( members.Remove( member ) )
						members.Add( member );
				}
			}

			foreach( var m in members )
			//foreach( var m in BaseType.MetadataGetMembers( context ) )
			{
				bool skip = false;
				if( context == null || context.Filter )
					OnMetadataGetMembersFilter( context, m, ref skip );
				if( !skip )
					yield return m;
			}
		}

		public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
		{
			return BaseType.MetadataGetMemberBySignature( signature, context );
		}
	}
}

#endif