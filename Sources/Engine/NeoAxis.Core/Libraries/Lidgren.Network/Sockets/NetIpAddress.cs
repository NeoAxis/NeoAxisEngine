#if !UWP
using System;
using System.Runtime.InteropServices;

namespace Internal.Lidgren.Network
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetIpAddress : IEquatable<NetIpAddress>
	{
		[FieldOffset(0)] public NetIpv4Address V4;
		[FieldOffset(0)] public NetIpv6Address V6;

		[FieldOffset(16)] public readonly NetIpAddressFamily Family;

		public NetIpAddress(in NetIpv4Address v4)
		{
			V6 = default;
			V4 = v4;
			
			Family = NetIpAddressFamily.V4;
		}
		
		public NetIpAddress(in NetIpv6Address v6)
		{
			V4 = default;
			V6 = v6;
			
			Family = NetIpAddressFamily.V6;
		}

		public /*!!!!android readonly*/ bool Equals(NetIpAddress other)
		{
			if (Family == NetIpAddressFamily.V4)
			{
				if (other.Family != NetIpAddressFamily.V4)
					return false;

				return V4.Equals(other.V4);
			}
			else
			{
				if (other.Family != NetIpAddressFamily.V6)
					return false;

				return V6.Equals(other.V6);
			}
		}

		public static implicit operator NetIpAddress(in NetIpv4Address v4) => new NetIpAddress(v4);
		public static implicit operator NetIpAddress(in NetIpv6Address v6) => new NetIpAddress(v6);
	}

	internal struct NetIpv4Address : IEquatable<NetIpv4Address>
	{
		public unsafe fixed byte Address[4];

		public unsafe NetIpv6Address ToIpv6Mapped()
		{
			NetIpv6Address v6 = default;
			v6.Address[10] = 0xFF;
			v6.Address[11] = 0xFF;
			v6.Address[12] = Address[0];
			v6.Address[13] = Address[1];
			v6.Address[14] = Address[2];
			v6.Address[15] = Address[3];
			return v6;
		}
		
		public unsafe bool Equals(NetIpv4Address other)
		{
			var spanA = MemoryMarshal.CreateReadOnlySpan(ref Address[0], 4);
			var spanB = MemoryMarshal.CreateReadOnlySpan(ref other.Address[0], 4);

			return spanA.SequenceEqual(spanB);
		}
	}

	internal struct NetIpv6Address : IEquatable<NetIpv6Address>
	{
		public unsafe fixed byte Address[16];
		
		public unsafe bool Equals(NetIpv6Address other)
		{
			var spanA = MemoryMarshal.CreateReadOnlySpan(ref Address[0], 16);
			var spanB = MemoryMarshal.CreateReadOnlySpan(ref other.Address[0], 16);

			return spanA.SequenceEqual(spanB);
		}
	}

	internal enum NetIpAddressFamily : byte
	{
		V4,
		V6
	}
}
#endif