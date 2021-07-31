using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
	public class AxImporter
	{
		public sealed class Options
		{
			public string outputName;

			public string outputDirectory;

			public byte[] publicKey;

			public StrongNameKeyPair keyPair;

			public string keyFile;

			public string keyContainer;

			public bool genSources;

			public bool msBuildErrors;

			public bool noLogo;

			public bool silentMode;

			public bool verboseMode;

			public bool delaySign;

			public bool overwriteRCW;

			public IReferenceResolver references;

			public bool ignoreRegisteredOcx;

			public Options()
			{
				throw null;
			}
		}

		public interface IReferenceResolver
		{
			string ResolveManagedReference(string assemName);

			string ResolveComReference(UCOMITypeLib typeLib);

			string ResolveComReference(AssemblyName name);

			string ResolveActiveXReference(UCOMITypeLib typeLib);
		}

		public string[] GeneratedAssemblies
		{
			get
			{
				throw null;
			}
		}

		public TYPELIBATTR[] GeneratedTypeLibAttributes
		{
			get
			{
				throw null;
			}
		}

		public string[] GeneratedSources
		{
			get
			{
				throw null;
			}
		}

		public AxImporter(Options options)
		{
			throw null;
		}

		public static string GetFileOfTypeLib(ref TYPELIBATTR tlibattr)
		{
			throw null;
		}

		public string GenerateFromFile(FileInfo file)
		{
			throw null;
		}

		public string GenerateFromTypeLibrary(UCOMITypeLib typeLib)
		{
			throw null;
		}

		public string GenerateFromTypeLibrary(UCOMITypeLib typeLib, Guid clsid)
		{
			throw null;
		}
	}
}
