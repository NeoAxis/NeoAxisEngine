// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace RoslynPad.Roslyn
{
	public class WorkspaceImpl : Workspace
	{
		public enum WorkspaceType
		{
			Project,
			FreeCsFile,
			CSharpScript,
		}
		public WorkspaceType workspaceType;
		//public bool isCSharpScript;

		public ProjectId project;
		public List<DocumentId> documents = new List<DocumentId>();
		public Dictionary<string, DocumentId> documentByFileName = new Dictionary<string, DocumentId>();
		public Dictionary<DocumentId, int> openedDocuments = new Dictionary<DocumentId, int>();

		//

		public WorkspaceImpl( HostServices hostServices, WorkspaceType workspaceType )//bool isCSharpScript )
			: base( hostServices, WorkspaceKind.Host )
		{
			this.workspaceType = workspaceType;
			//this.isCSharpScript = isCSharpScript;

			DiagnosticProvider.Enable( this, DiagnosticProvider.Options.Semantic );
		}

		public void SetCurrentSolutionPublic( Solution solution )
		{
			var oldSolution = CurrentSolution;
			var newSolution = SetCurrentSolution( solution );
			RaiseWorkspaceChangedEventAsync( WorkspaceChangeKind.SolutionChanged, oldSolution, newSolution );
		}

		public override bool CanOpenDocuments { get { return true; } }

		public override bool CanApplyChange( ApplyChangesKind feature )
		{
			switch( feature )
			{
			case ApplyChangesKind.ChangeDocument:
				return true;
			default:
				return false;
			}
		}

		public void CallDocumentOpenedAndContextUpdated( DocumentId documentId, SourceTextContainer textContainer )
		{
			OnDocumentOpened( documentId, textContainer );
			OnDocumentContextUpdated( documentId );
		}

		public void CallDocumentClosingAndClosed_CsFile( DocumentId documentId )
		{
			OnDocumentClosing( documentId );

			var document = CurrentSolution.GetDocument( documentId );
			if( document != null && document.TryGetText( out var text ) )
				OnDocumentClosed( documentId, TextLoader.From( TextAndVersion.Create( text, VersionStamp.Default, document.FilePath ) ) );
		}

		public event Action<DocumentId, SourceText> ApplyingTextChange;

		protected override void Dispose( bool finalize )
		{
			base.Dispose( finalize );

			ApplyingTextChange = null;

			DiagnosticProvider.Disable( this );
		}

		protected override void ApplyDocumentTextChanged( DocumentId document, SourceText newText )
		{
			if( !documents.Contains( document ) )
				return;

			ApplyingTextChange?.Invoke( document, newText );

			OnDocumentTextChanged( document, newText, PreservationMode.PreserveIdentity );
		}
	}
}
