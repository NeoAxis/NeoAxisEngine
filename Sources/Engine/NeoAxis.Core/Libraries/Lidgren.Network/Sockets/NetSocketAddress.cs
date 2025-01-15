//#if !UWP && LIDGREN //&& !ANDROID
//using System;
//using System.Diagnostics;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;

//// ReSharper disable NonReadonlyMemberInGetHashCode

//namespace Internal.Lidgren.Network
//{
//	// Ok so basically I got really enthusiastic and
//	// originally wanted to rewrite all of Lidgren to use custom address types.
//	// So that's why this is more advanced than it probably needs to be.
	
//	// SocketAddress and IPAddress API inspired by Rust's.

//	/// <summary>
//	/// Represents an internet socket address, either IPv4 or IPv6.
//	/// </summary>
//	[StructLayout(LayoutKind.Explicit)]
//	internal struct NetSocketAddress : IEquatable<NetSocketAddress>
//	{
//		[FieldOffset(0)] public NetSocketAddressV4 V4;
//		[FieldOffset(0)] public NetSocketAddressV6 V6;

//		// Fit the tag after the addresses so the struct is 28 bytes with alignment.
//		[FieldOffset(26)] public readonly NetIpAddressFamily Family;

//		public NetSocketAddress(NetIpAddress address, ushort port)
//		{
//			this = default;
//			Family = address.Family;

//			if (address.Family == NetIpAddressFamily.V4)
//			{
//				V4.Port = port;
//				V4.Address = address.V4;
//			}
//			else
//			{
//				V6.Port = port;
//				V6.Address = address.V6;
//			}
//		}

//		public NetSocketAddress(in NetSocketAddressV4 v4)
//		{
//			V6 = default;
//			V4 = v4;

//			Family = NetIpAddressFamily.V4;
//		}

//		public NetSocketAddress(in NetSocketAddressV6 v6)
//		{
//			V4 = default;
//			V6 = v6;

//			Family = NetIpAddressFamily.V6;
//		}

//		public  /*!!!!android readonly*/  bool IsIpv4 => Family == NetIpAddressFamily.V4;

//		public ushort Port
//		{
//			/*!!!!android readonly*/ get => IsIpv4 ? V4.Port : V6.Port;
//			set
//			{
//				if (IsIpv4)
//					V4.Port = value;
//				else
//					V6.Port = value;
//			}
//		}

//		public  /*!!!!android readonly*/  NetIpAddress IpAddress =>
//			Family == NetIpAddressFamily.V4 ? new NetIpAddress(V4.Address) : new NetIpAddress(V6.Address);

//		public static explicit operator NetSocketAddress(IPEndPoint endPoint)
//		{
//			if (endPoint.AddressFamily == AddressFamily.InterNetwork)
//			{
//				NetIpv4Address address = default;
//				var asBytes = MemoryMarshal.CreateSpan(ref Unsafe.As<NetIpv4Address, byte>(ref address), 4);
//				endPoint.Address.TryWriteBytes(asBytes, out _);
//				return new NetSocketAddress(address, (ushort)endPoint.Port);
//			}
//			else
//			{
//				Debug.Assert(endPoint.AddressFamily == AddressFamily.InterNetworkV6);

//				NetIpv6Address address = default;
//				var asBytes = MemoryMarshal.CreateSpan(ref Unsafe.As<NetIpv6Address, byte>(ref address), 16);
//				endPoint.Address.TryWriteBytes(asBytes, out _);
//				return new NetSocketAddressV6(address, (ushort)endPoint.Port, (uint) endPoint.Address.ScopeId);
//			}
//		}

//		public static explicit operator IPEndPoint(in NetSocketAddress address)
//		{
//			if (address.IsIpv4)
//				return (IPEndPoint)address.V4;
//			else 
//				return (IPEndPoint)address.V6;
//		}
		
//		public static implicit operator NetSocketAddress(in NetSocketAddressV4 v4) => new NetSocketAddress(v4);
//		public static implicit operator NetSocketAddress(in NetSocketAddressV6 v6) => new NetSocketAddress(v6);

//		public  /*!!!!android readonly*/  bool Equals(NetSocketAddress other)
//		{
//			if (Family == NetIpAddressFamily.V4)
//			{
//				if (other.Family != NetIpAddressFamily.V4)
//					return false;

//				return V4.Equals(other.V4);
//			}
//			else
//			{
//				if (other.Family != NetIpAddressFamily.V6)
//					return false;

//				return V6.Equals(other.V6);
//			}
//		}

//		public override  /*!!!!android readonly*/  int GetHashCode()
//		{
//			if (IsIpv4)
//				return HashCode.Combine(Family, V4.GetHashCode());
//			else
//				return HashCode.Combine(Family, V6.GetHashCode());
//		}
//	}

//	internal struct NetSocketAddressV4 : IEquatable<NetSocketAddressV4>
//	{
//		public ushort Port;
//		public NetIpv4Address Address;

//		public NetSocketAddressV4(NetIpv4Address address, ushort port)
//		{
//			Address = address;
//			Port = port;
//		}
		
//		public bool Equals(NetSocketAddressV4 other)
//		{
//			return Port == other.Port && Address.Equals(other.Address);
//		}

//		public override bool Equals(object obj)
//		{
//			return obj is NetSocketAddressV4 other && Equals(other);
//		}

//		public override int GetHashCode()
//		{
//			return HashCode.Combine(Port, Address);
//		}

//		public static bool operator ==(NetSocketAddressV4 left, NetSocketAddressV4 right)
//		{
//			return left.Equals(right);
//		}

//		public static bool operator !=(NetSocketAddressV4 left, NetSocketAddressV4 right)
//		{
//			return !left.Equals(right);
//		}
		
//		public static unsafe explicit operator IPEndPoint(in NetSocketAddressV4 address)
//		{
//			var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in address).Address.Address[0], 4);
//			return new IPEndPoint(new IPAddress(span), address.Port);
//		}
//	}

//	internal struct NetSocketAddressV6 : IEquatable<NetSocketAddressV6>
//	{
//		public ushort Port;
//		public NetIpv6Address Address;
//		public uint ScopeId;

//		public NetSocketAddressV6(NetIpv6Address address, ushort port, uint scopeId)
//		{
//			Address = address;
//			Port = port;
//			ScopeId = scopeId;
//		}
		
//		public bool Equals(NetSocketAddressV6 other)
//		{
//			return Port == other.Port && Address.Equals(other.Address) && ScopeId == other.ScopeId;
//		}

//		public override bool Equals(object obj)
//		{
//			return obj is NetSocketAddressV6 other && Equals(other);
//		}

//		public override int GetHashCode()
//		{
//			return HashCode.Combine(Port, Address, ScopeId);
//		}

//		public static bool operator ==(NetSocketAddressV6 left, NetSocketAddressV6 right)
//		{
//			return left.Equals(right);
//		}

//		public static bool operator !=(NetSocketAddressV6 left, NetSocketAddressV6 right)
//		{
//			return !left.Equals(right);
//		}
		
//		public static unsafe explicit operator IPEndPoint(in NetSocketAddressV6 address)
//		{
//			var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in address).Address.Address[0], 16);
//			return new IPEndPoint(new IPAddress(span, address.ScopeId), address.Port);
//		}
//	}
//}
//#endif