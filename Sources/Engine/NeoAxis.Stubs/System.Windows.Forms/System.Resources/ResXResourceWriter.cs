using System.IO;
using System.Reflection;

namespace System.Resources
{
	public class ResXResourceWriter : IResourceWriter, IDisposable
	{
		public static readonly string BinSerializedObjectMimeType;

		public static readonly string SoapSerializedObjectMimeType;

		public static readonly string DefaultSerializedObjectMimeType;

		public static readonly string ByteArraySerializedObjectMimeType;

		public static readonly string ResMimeType;

		public static readonly string Version;

		public static readonly string ResourceSchema;

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

		public ResXResourceWriter(string fileName)
		{
			throw null;
		}

		public ResXResourceWriter(string fileName, Func<Type, string> typeNameConverter)
		{
			throw null;
		}

		public ResXResourceWriter(Stream stream)
		{
			throw null;
		}

		public ResXResourceWriter(Stream stream, Func<Type, string> typeNameConverter)
		{
			throw null;
		}

		public ResXResourceWriter(TextWriter textWriter)
		{
			throw null;
		}

		public ResXResourceWriter(TextWriter textWriter, Func<Type, string> typeNameConverter)
		{
			throw null;
		}

		~ResXResourceWriter()
		{
			throw null;
		}

		public virtual void AddAlias(string aliasName, AssemblyName assemblyName)
		{
			throw null;
		}

		public void AddMetadata(string name, byte[] value)
		{
			throw null;
		}

		public void AddMetadata(string name, string value)
		{
			throw null;
		}

		public void AddMetadata(string name, object value)
		{
			throw null;
		}

		public void AddResource(string name, byte[] value)
		{
			throw null;
		}

		public void AddResource(string name, object value)
		{
			throw null;
		}

		public void AddResource(string name, string value)
		{
			throw null;
		}

		public void AddResource(ResXDataNode node)
		{
			throw null;
		}

		public void Close()
		{
			throw null;
		}

		public virtual void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		public void Generate()
		{
			throw null;
		}
	}
}
