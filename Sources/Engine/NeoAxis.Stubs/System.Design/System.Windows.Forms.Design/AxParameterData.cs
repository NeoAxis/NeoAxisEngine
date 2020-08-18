using System.CodeDom;
using System.Reflection;

namespace System.Windows.Forms.Design
{
	public class AxParameterData
	{
		public FieldDirection Direction
		{
			get
			{
				throw null;
			}
		}

		public bool IsByRef
		{
			get
			{
				throw null;
			}
		}

		public bool IsIn
		{
			get
			{
				throw null;
			}
		}

		public bool IsOut
		{
			get
			{
				throw null;
			}
		}

		public bool IsOptional
		{
			get
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

		public Type ParameterType
		{
			get
			{
				throw null;
			}
		}

		public string TypeName
		{
			get
			{
				throw null;
			}
		}

		public AxParameterData(string inname, string typeName)
		{
			throw null;
		}

		public AxParameterData(string inname, Type type)
		{
			throw null;
		}

		public AxParameterData(ParameterInfo info)
		{
			throw null;
		}

		public AxParameterData(ParameterInfo info, bool ignoreByRefs)
		{
			throw null;
		}

		public static AxParameterData[] Convert(ParameterInfo[] infos)
		{
			throw null;
		}

		public static AxParameterData[] Convert(ParameterInfo[] infos, bool ignoreByRefs)
		{
			throw null;
		}
	}
}
