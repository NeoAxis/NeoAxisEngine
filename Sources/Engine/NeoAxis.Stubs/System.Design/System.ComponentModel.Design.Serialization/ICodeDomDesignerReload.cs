using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
	public interface ICodeDomDesignerReload
	{
		bool ShouldReloadDesigner(CodeCompileUnit newTree);
	}
}
