using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace System.Data.Design
{
	public sealed class TypedDataSetGenerator
	{
		public enum GenerateOption
		{
			None,
			HierarchicalUpdate,
			LinqOverTypedDatasets
		}

		public static ICollection<Assembly> ReferencedAssemblies
		{
			get
			{
				throw null;
			}
		}

		public static string GetProviderName(string inputFileContent)
		{
			throw null;
		}

		public static string GetProviderName(string inputFileContent, string tableName)
		{
			throw null;
		}

		public static string Generate(DataSet dataSet, CodeNamespace codeNamespace, CodeDomProvider codeProvider)
		{
			throw null;
		}

		public static void Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, DbProviderFactory specifiedFactory)
		{
			throw null;
		}

		public static void Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, Hashtable customDBProviders)
		{
			throw null;
		}

		public static void Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, Hashtable customDBProviders, GenerateOption option)
		{
			throw null;
		}

		public static void Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, Hashtable customDBProviders, GenerateOption option, string dataSetNamespace)
		{
			throw null;
		}

		public static void Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, Hashtable customDBProviders, GenerateOption option, string dataSetNamespace, string basePath)
		{
			throw null;
		}

		public static string Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider)
		{
			throw null;
		}

		public static string Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, GenerateOption option)
		{
			throw null;
		}

		public static string Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, GenerateOption option, string dataSetNamespace)
		{
			throw null;
		}

		public static string Generate(string inputFileContent, CodeCompileUnit compileUnit, CodeNamespace mainNamespace, CodeDomProvider codeProvider, GenerateOption option, string dataSetNamespace, string basePath)
		{
			throw null;
		}
	}
}
