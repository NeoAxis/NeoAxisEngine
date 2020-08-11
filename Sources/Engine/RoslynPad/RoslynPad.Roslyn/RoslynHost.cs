using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
//using Microsoft.CodeAnalysis.MSBuild;
using RoslynPad.Roslyn.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.IO;

namespace RoslynPad.Roslyn
{
	public class RoslynHost : IRoslynHost
	{
		static RoslynHost instance;

		List<WorkspaceImpl> workspaces = new List<WorkspaceImpl>();

		readonly ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>> _diagnosticsUpdatedNotifiers;
		int _documentNumber;

		readonly NuGetConfiguration _nuGetConfiguration;

		readonly IDocumentationProviderService _documentationProviderService;
		readonly CompositionHost _compositionContext;
		public HostServices HostServices { get; }

		//ImmutableArray<MetadataReference> DefaultReferences { get; }

		/////////////////////////////////////////

		public static RoslynHost Instance
		{
			get { return instance; }
		}

		static RoslynHost()
		{
			//WorkaroundForDesktopShim( typeof( Compilation ) );
			//WorkaroundForDesktopShim( typeof( TaggedText ) );
		}

		//static void WorkaroundForDesktopShim( Type typeInAssembly )
		//{
		//	// DesktopShim doesn't work on Linux, so we hack around it

		//	typeInAssembly.GetTypeInfo().Assembly
		//		.GetType( "Roslyn.Utilities.DesktopShim+FileNotFoundException" )
		//		?.GetRuntimeFields().FirstOrDefault( f => f.Name == "s_fusionLog" )
		//		?.SetValue( null, typeof( Exception ).GetRuntimeProperty( nameof( Exception.InnerException ) ) );
		//}

		public RoslynHost( NuGetConfiguration nuGetConfiguration )
		{
			instance = this;

			_nuGetConfiguration = nuGetConfiguration;
			_diagnosticsUpdatedNotifiers = new ConcurrentDictionary<DocumentId, Action<DiagnosticsUpdatedArgs>>();

			//!!!!
			var assemblies = ImmutableArray.Create(
				// Microsoft.CodeAnalysis.Workspaces
				typeof( WorkspacesResources ).GetTypeInfo().Assembly,
				// Microsoft.CodeAnalysis.CSharp.Workspaces
				typeof( CSharpWorkspaceResources ).GetTypeInfo().Assembly,
				// Microsoft.CodeAnalysis.Features
				typeof( FeaturesResources ).GetTypeInfo().Assembly,
				// Microsoft.CodeAnalysis.CSharp.Features
				typeof( CSharpFeaturesResources ).GetTypeInfo().Assembly,
				// RoslynPad.Roslyn
				typeof( RoslynHost ).GetTypeInfo().Assembly );

			//!!!!
			try
			{
				var additionalAssemblies = ImmutableArray.Create(
					Assembly.Load( new AssemblyName( "RoslynPad.Roslyn.Windows" ) ),
					Assembly.Load( new AssemblyName( "RoslynPad.Editor.Windows" ) )
				);
				assemblies = assemblies.Concat( additionalAssemblies );
			}
			catch { }

			var partTypes = assemblies.SelectMany( x => x.DefinedTypes ).Select( x => x.AsType() );
			_compositionContext = new ContainerConfiguration().WithParts( partTypes ).CreateContainer();
			HostServices = MefHostServices.Create( _compositionContext );

			_documentationProviderService = GetService<IDocumentationProviderService>();

			GetService<IDiagnosticService>().DiagnosticsUpdated += OnDiagnosticsUpdated;
		}

		ParseOptions CreateDefaultParseOptions( bool isCSharpScript )
		{
			//static readonly string[] PreprocessorSymbols = new string[ 0 ];// "TRACE", "DEBUG" } );

			return new CSharpParseOptions(
				kind: isCSharpScript ? SourceCodeKind.Script : SourceCodeKind.Regular,
				languageVersion: LanguageVersion.Latest );
		}

		//public MetadataReference CreateMetadataReference( string location )
		//{
		//    return MetadataReference.CreateFromFile( location,
		//        documentation: _documentationProviderService.GetDocumentationProvider( location ) );
		//}

		void OnDiagnosticsUpdated( object sender, DiagnosticsUpdatedArgs diagnosticsUpdatedArgs )
		{
			var documentId = diagnosticsUpdatedArgs?.DocumentId;
			if( documentId == null )
				return;

			if( _diagnosticsUpdatedNotifiers.TryGetValue( documentId, out var notifier ) )
				notifier( diagnosticsUpdatedArgs );
		}

		public TService GetService<TService>()
		{
			return _compositionContext.GetExport<TService>();
		}

		//internal void AddMetadataReference( ProjectId projectId, AssemblyIdentity assemblyIdentity )
		//{
		//}

		WorkspaceImpl GetWorkspace( DocumentId documentId )
		{
			lock( workspaces )
			{
				foreach( var workspaceItem in workspaces )
				{
					if( workspaceItem.documents.Contains( documentId ) )
						return workspaceItem;
				}
				return null;
			}
		}

		public Document GetDocument( DocumentId documentId )
		{
			if( documentId == null )
				throw new ArgumentNullException( nameof( documentId ) );

			var workspace = GetWorkspace( documentId );
			if( workspace != null )
				return workspace.CurrentSolution.GetDocument( documentId );

			return null;
		}

		public void UpdateDocument( Document document )
		{
			if( document == null )
				throw new ArgumentNullException( nameof( document ) );

			var workspace = GetWorkspace( document.Id );
			if( workspace != null )
				workspace.TryApplyChanges( document.Project.Solution );
		}

		//!!!!used for NuGets
		public bool HasReference( DocumentId documentId, string text )
		{
			if( documentId == null )
				throw new ArgumentNullException( nameof( documentId ) );

			var workspace = GetWorkspace( documentId );
			if( workspace != null )
			{
				if( workspace.CurrentSolution.GetDocument( documentId ).Project.TryGetCompilation( out var compilation ) )
					return compilation.ReferencedAssemblyNames.Any( a => a.Name == text );
			}

			return false;

			//if( !_workspaces.TryGetValue( documentId, out var workspace ) )
			//	return false;
			//if( workspace.CurrentSolution.GetDocument( documentId ).Project.TryGetCompilation( out var compilation ) )
			//	return compilation.ReferencedAssemblyNames.Any( a => a.Name == text );
			//return false;
		}

		public void CloseDocument( DocumentId documentId )
		{
			_diagnosticsUpdatedNotifiers.TryRemove( documentId, out _ );

			var workspace = GetWorkspace( documentId );
			if( workspace != null )
			{
				workspace.openedDocuments.Remove( documentId );

				if( workspace.workspaceType == WorkspaceImpl.WorkspaceType.Project )
				{
					//only detach editor for cs file in project
					workspace.CallDocumentClosingAndClosed_CsFile( documentId );
				}
				else
				{
					//delete workspace for CSharp Scripts

					workspace.documents.Remove( documentId );
					workspace.CloseDocument( documentId );

					//remove document
					var document = workspace.CurrentSolution.GetDocument( documentId );
					if( document != null )
					{
						var solution = document.Project.RemoveDocument( documentId ).Solution;
						workspace.SetCurrentSolutionPublic( solution );
					}

					lock( workspaces )
						workspaces.Remove( workspace );
					workspace.Dispose();
				}
			}
		}

		CompilationOptions CreateCompilationOptions( DocumentCreationArgs args )
		{
			List<string> usingNamespaces;
			if( !args.IsCSharpScript )
				usingNamespaces = new List<string>();
			else
				usingNamespaces = NeoAxisCoreExchange.Instance.CSharpScriptUsingNamespaces;

			var result = new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				usings: usingNamespaces,
				allowUnsafe: true,
				sourceReferenceResolver: new SourceFileResolver( ImmutableArray<string>.Empty, args.WorkingDirectory ),
				metadataReferenceResolver: new NuGetScriptMetadataResolver( _nuGetConfiguration, args.WorkingDirectory, useCache: true ) );

			return result;
		}

		Project CreateProject( WorkspaceImpl workspace, DocumentCreationArgs args )
		{
			string name;
			if( workspace.workspaceType == WorkspaceImpl.WorkspaceType.Project )
				name = "Project";
			else
				name = args.Name ?? "Program" + Interlocked.Increment( ref _documentNumber );

			var id = ProjectId.CreateNewId( name );

			//create parse options
			var parseOptions = CreateDefaultParseOptions( args.IsCSharpScript );

			//create compilation options
			var compilationOptions = CreateCompilationOptions( args );
			if( args.IsCSharpScript )
				compilationOptions = compilationOptions.WithScriptClassName( name );


			//get references
			List<string> referenceNames;
			if( workspace.workspaceType == WorkspaceImpl.WorkspaceType.Project )
			{
				referenceNames = new List<string>();
				if( NeoAxisCoreExchange.Instance.GetProjectData( out _, out var referenceNames2 ) )
				{
					foreach( var referenceName in referenceNames2 )
					{
						var fullPath = NeoAxisCoreExchange.Instance.ResolveAssemblyName( referenceName );
						if( File.Exists( fullPath ) )
							referenceNames.Add( fullPath );
					}
				}
			}
			else
				referenceNames = NeoAxisCoreExchange.Instance.CSharpScriptReferenceAssemblies;

			//!!!!RoslynHostReferences class is too complex
			var references1 = RoslynHostReferences.Default.With( assemblyPathReferences: referenceNames );
			var references = references1.GetReferences( _documentationProviderService.GetDocumentationProvider );


			var newSolution = workspace.CurrentSolution.AddProject( ProjectInfo.Create(
				id,
				VersionStamp.Create(),
				name,
				name,
				LanguageNames.CSharp,
				isSubmission: args.IsCSharpScript,
				parseOptions: parseOptions,
				compilationOptions: compilationOptions,
				metadataReferences: /*previousProject != null ? ImmutableArray<MetadataReference>.Empty : */references,
				//projectReferences: /*previousProject != null ? new[] { new ProjectReference( previousProject.Id ) } :*/ null,
				hostObjectType: args.ContextType
			) );

			var project = newSolution.GetProject( id );
			return project;
		}

		WorkspaceImpl GetProjectWorkspace()
		{
			lock( workspaces )
			{
				foreach( var workspaceItem in workspaces )
					if( workspaceItem.workspaceType == WorkspaceImpl.WorkspaceType.Project )
						return workspaceItem;
			}
			return null;
		}

		public void OnAddCsFileToProject( string fullPath )
		{
			var workspace = GetProjectWorkspace();
			if( workspace != null )
			{
				var documentId = DocumentId.CreateNewId( workspace.project );

				string text = "";
				try
				{
					text = File.ReadAllText( fullPath );
				}
				catch { }

				var solution = workspace.CurrentSolution.AddDocument( documentId, fullPath, text, null, fullPath );
				workspace.SetCurrentSolutionPublic( solution );

				workspace.documents.Add( documentId );
				workspace.documentByFileName[ fullPath ] = documentId;
			}
		}

		public void OnRemoveCsFileFromProject( string fullPath )
		{
			var workspace = GetProjectWorkspace();
			if( workspace != null )
			{
				if( workspace.documentByFileName.TryGetValue( fullPath, out var documentId ) )
				{
					//remove only when document is closed
					if( !workspace.openedDocuments.ContainsKey( documentId ) )
					{
						var solution = workspace.CurrentSolution.RemoveDocument( documentId );
						workspace.SetCurrentSolutionPublic( solution );

						workspace.documents.Remove( documentId );

						foreach( var pair in workspace.documentByFileName )
						{
							if( pair.Value == documentId )
							{
								workspace.documentByFileName.Remove( pair.Key );
								break;
							}
						}
					}
				}
			}
		}

		public DocumentId AddDocument( DocumentCreationArgs args )
		{
			if( args == null )
				throw new ArgumentNullException( nameof( args ) );
			if( args.SourceTextContainer == null )
				throw new ArgumentNullException( nameof( args.SourceTextContainer ) );

			if( !args.IsCSharpScript )
			{
				var workspace = GetProjectWorkspace();

				//check workspace is already created for project
				if( workspace == null )
				{
					//create workspace
					workspace = new WorkspaceImpl( HostServices, WorkspaceImpl.WorkspaceType.Project );
					lock( workspaces )
						workspaces.Add( workspace );

					//create project
					var project = CreateProject( workspace, args );
					workspace.SetCurrentSolutionPublic( project.Solution );
					workspace.project = project.Id;

					//create project file documents
					if( NeoAxisCoreExchange.Instance.GetProjectData( out var csFiles, out var references ) )
					{
						foreach( var csFile in csFiles )
							OnAddCsFileToProject( csFile );
					}
				}

				//open document editor
				{
					if( workspace.documentByFileName.TryGetValue( args.CsFileProjectPath, out var documentId ) )
					{
						//project contains the file

						//open document
						workspace.CallDocumentOpenedAndContextUpdated( documentId, args.SourceTextContainer );

						//configure editor
						_diagnosticsUpdatedNotifiers.TryAdd( documentId, args.OnDiagnosticsUpdated );
						workspace.ApplyingTextChange += ( d, s ) =>
						{
							if( documentId == d )
								args.OnTextUpdated( s );
						};

						workspace.openedDocuments[ documentId ] = 1;

						return documentId;
					}
					else
					{
						//project not contains the file

						//create workspace
						workspace = new WorkspaceImpl( HostServices, WorkspaceImpl.WorkspaceType.FreeCsFile );
						lock( workspaces )
							workspaces.Add( workspace );

						//create project
						var project = CreateProject( workspace, args );
						workspace.SetCurrentSolutionPublic( project.Solution );
						workspace.project = project.Id;

						//create document
						documentId = DocumentId.CreateNewId( project.Id );
						var solution = project.Solution.AddDocument( documentId, args.Name ?? project.Name, args.SourceTextContainer.CurrentText );
						var document = solution.GetDocument( documentId );
						workspace.SetCurrentSolutionPublic( document.Project.Solution );
						workspace.documents.Add( documentId );

						//open document
						workspace.CallDocumentOpenedAndContextUpdated( documentId, args.SourceTextContainer );

						//configure editor
						_diagnosticsUpdatedNotifiers.TryAdd( documentId, args.OnDiagnosticsUpdated );
						workspace.ApplyingTextChange += ( d, s ) =>
						{
							if( documentId == d )
								args.OnTextUpdated( s );
						};

						workspace.openedDocuments[ documentId ] = 1;

						return documentId;
					}
				}
			}
			else
			{
				//create workspace
				var workspace = new WorkspaceImpl( HostServices, WorkspaceImpl.WorkspaceType.CSharpScript );
				lock( workspaces )
					workspaces.Add( workspace );

				//create project
				var project = CreateProject( workspace, args );
				workspace.SetCurrentSolutionPublic( project.Solution );
				workspace.project = project.Id;

				//create document
				var documentId = DocumentId.CreateNewId( project.Id );
				var solution = project.Solution.AddDocument( documentId, args.Name ?? project.Name, args.SourceTextContainer.CurrentText );
				var document = solution.GetDocument( documentId );
				workspace.SetCurrentSolutionPublic( document.Project.Solution );
				workspace.documents.Add( documentId );

				//open document
				workspace.CallDocumentOpenedAndContextUpdated( documentId, args.SourceTextContainer );

				//configure editor
				_diagnosticsUpdatedNotifiers.TryAdd( documentId, args.OnDiagnosticsUpdated );
				workspace.ApplyingTextChange += ( d, s ) =>
				{
					if( documentId == d )
						args.OnTextUpdated( s );
				};

				workspace.openedDocuments[ documentId ] = 1;

				return documentId;
			}
		}
	}
}
