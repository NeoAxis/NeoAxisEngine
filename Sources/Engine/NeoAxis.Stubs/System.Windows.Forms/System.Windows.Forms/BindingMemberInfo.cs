using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 8)]
	public struct BindingMemberInfo
	{
		public string BindingPath
		{
			get
			{
				throw null;
			}
		}

		public string BindingField
		{
			get
			{
				throw null;
			}
		}

		public string BindingMember
		{
			get
			{
				throw null;
			}
		}

		public BindingMemberInfo(string dataMember)
		{
			throw null;
		}

		public override bool Equals(object otherObject)
		{
			throw null;
		}

		public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b)
		{
			throw null;
		}

		public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}
	}
}
