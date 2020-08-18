using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;

namespace System.Resources
{
	public class ResXResourceReader : IResourceReader, IEnumerable, IDisposable
	{
		public string BasePath
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool UseResXDataNodes
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ResXResourceReader(string fileName)
		{
			throw null;
		}

		public ResXResourceReader(string fileName, ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public ResXResourceReader(TextReader reader)
		{
			throw null;
		}

		public ResXResourceReader(TextReader reader, ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public ResXResourceReader(Stream stream)
		{
			throw null;
		}

		public ResXResourceReader(Stream stream, ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public ResXResourceReader(Stream stream, AssemblyName[] assemblyNames)
		{
			throw null;
		}

		public ResXResourceReader(TextReader reader, AssemblyName[] assemblyNames)
		{
			throw null;
		}

		public ResXResourceReader(string fileName, AssemblyName[] assemblyNames)
		{
			throw null;
		}

		~ResXResourceReader()
		{
			throw null;
		}

		public void Close()
		{
			throw null;
		}

		void IDisposable.Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public static ResXResourceReader FromFileContents(string fileContents)
		{
			throw null;
		}

		public static ResXResourceReader FromFileContents(string fileContents, ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public static ResXResourceReader FromFileContents(string fileContents, AssemblyName[] assemblyNames)
		{
			throw null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw null;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			throw null;
		}

		public IDictionaryEnumerator GetMetadataEnumerator()
		{
			throw null;
		}
	}
}
