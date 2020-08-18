using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Resources
{
	[Serializable]
	public sealed class ResXDataNode : ISerializable
	{
		public string Comment
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

		public string Name
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

		public ResXFileRef FileRef
		{
			get
			{
				throw null;
			}
		}

		public ResXDataNode(string name, object value)
		{
			throw null;
		}

		public ResXDataNode(string name, object value, Func<Type, string> typeNameConverter)
		{
			throw null;
		}

		public ResXDataNode(string name, ResXFileRef fileRef)
		{
			throw null;
		}

		public ResXDataNode(string name, ResXFileRef fileRef, Func<Type, string> typeNameConverter)
		{
			throw null;
		}

		public Point GetNodePosition()
		{
			throw null;
		}

		public string GetValueTypeName(ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public string GetValueTypeName(AssemblyName[] names)
		{
			throw null;
		}

		public object GetValue(ITypeResolutionService typeResolver)
		{
			throw null;
		}

		public object GetValue(AssemblyName[] names)
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}
	}
}
