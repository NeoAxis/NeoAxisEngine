using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.Resources.Tools
{
	public static class StronglyTypedResourceBuilder
	{
		public static CodeCompileUnit Create(IDictionary resourceList, string baseName, string generatedCodeNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
		{
			throw null;
		}

		public static CodeCompileUnit Create(IDictionary resourceList, string baseName, string generatedCodeNamespace, string resourcesNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
		{
			throw null;
		}

		public static CodeCompileUnit Create(string resxFile, string baseName, string generatedCodeNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
		{
			throw null;
		}

		public static CodeCompileUnit Create(string resxFile, string baseName, string generatedCodeNamespace, string resourcesNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
		{
			throw null;
		}

		public static string VerifyResourceName(string key, CodeDomProvider provider)
		{
			throw null;
		}
	}
}
