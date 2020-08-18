using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.ComponentModel.Design
{
	//[ComImport]
	public interface IDesignTimeAssemblyLoader
	{
		string GetTargetAssemblyPath(AssemblyName runtimeOrTargetAssemblyName, string suggestedAssemblyPath, FrameworkName targetFramework);

		Assembly LoadRuntimeAssembly(AssemblyName targetAssemblyName);
	}
}
