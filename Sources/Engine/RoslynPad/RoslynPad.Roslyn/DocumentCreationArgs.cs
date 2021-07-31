using System;
using Microsoft.CodeAnalysis.Text;
using RoslynPad.Roslyn.Diagnostics;

namespace RoslynPad.Roslyn
{
    public class DocumentCreationArgs
    {
        public SourceTextContainer SourceTextContainer { get; }
		public string CsFileProjectPath { get; }
		public string WorkingDirectory { get; }
        public Action<DiagnosticsUpdatedArgs> OnDiagnosticsUpdated { get; }
        public Action<SourceText> OnTextUpdated { get; }
        public string Name { get; }
        public bool IsCSharpScript { get; }
        public Type ContextType { get; }

        public DocumentCreationArgs(SourceTextContainer sourceTextContainer, string csFileProjectPath, string workingDirectory, 
            Action<DiagnosticsUpdatedArgs> onDiagnosticsUpdated = null, Action<SourceText> onTextUpdated = null, 
            string name = null, bool isCSharpScript = true, Type contextType = null)
        {
            SourceTextContainer = sourceTextContainer;
			CsFileProjectPath = csFileProjectPath;
			WorkingDirectory = workingDirectory;
            OnDiagnosticsUpdated = onDiagnosticsUpdated;
            OnTextUpdated = onTextUpdated;
            Name = name;
            IsCSharpScript = isCSharpScript;
            ContextType = contextType;
        }
    }
}