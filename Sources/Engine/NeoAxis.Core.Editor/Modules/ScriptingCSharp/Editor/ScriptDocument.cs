// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	internal abstract class ScriptDocument
	{
		public abstract bool IsCSharpScript { get; }
		public virtual string CsFileProjectPath { get { return ""; } }
		public virtual string WorkingDirectory { get { return Path.GetTempPath(); } }
		//public virtual Type ContextType { get { return null; } }
		public abstract string LoadText();
		//public abstract bool SaveText( Microsoft.CodeAnalysis.Text.SourceText text ); //TODO: change text arg type to string?

		public event EventHandler CodeChanged;
		protected void RaiseCodeChanged( EventArgs e )
		{
			CodeChanged?.Invoke( this, e );
		}
	}

	//internal class TextScriptAdapter : ScriptDocument
	//{
	//	string text;

	//	public override Type ContextType => typeof(ScriptContext);
	//	public override bool IsEmbeddedScript => true;
	//	public TextScriptAdapter( string text )
	//	{
	//		this.text = text;
	//	}

	//	public override string LoadText()
	//	{
	//		return text;
	//	}

	//	public override Task SaveText( Microsoft.CodeAnalysis.Text.SourceText text )
	//	{
	//		this.text = text.ToString();
	//		return Task.CompletedTask;
	//	}
	//}
}
