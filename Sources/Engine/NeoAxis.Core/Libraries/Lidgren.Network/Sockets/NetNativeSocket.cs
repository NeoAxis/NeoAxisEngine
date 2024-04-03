//#if !UWP
//using System;
//using System.Buffers.Binary;
//using System.Runtime.InteropServices;

//#pragma warning disable CS8981

//namespace Internal.Lidgren.Network
//{
//	internal static unsafe class NetNativeSocket
//	{
//		internal static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
//		internal static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

//#pragma warning disable 649
//		// ReSharper disable InconsistentNaming
//		// ReSharper disable IdentifierTypo
//		// ReSharper disable StringLiteralTypo

//		internal static readonly ushort AF_INET = 2;
//		internal static readonly ushort AF_INET6 = (ushort) (IsWindows ? 23 : 10);
		
//		internal struct sockaddr
//		{
//			public ushort sa_family;
//			public fixed byte sa_data[14];
//		}

//		internal struct sockaddr_in
//		{
//			public ushort sin_family;
//			public ushort sin_port;
//			public in_addr sin_addr;
//			public fixed byte sin_zero[8];
//		}

//		internal struct in_addr
//		{
//			public uint s_addr;
//		}

//		internal struct sockaddr_in6
//		{
//			public ushort sin6_family;
//			public ushort sin6_port;
//			public uint sin6_flowinfo;
//			public in6_addr sin6_addr;
//			public uint sin6_scope_id;
//		}

//		internal struct in6_addr
//		{
//			public fixed byte s6_addr[16];
//		}

//		public static ushort htons(ushort hostshort) =>
//			BitConverter.IsLittleEndian
//				? BinaryPrimitives.ReverseEndianness(hostshort)
//				: hostshort;

//		public static uint htonl(uint hostlong) =>
//			BitConverter.IsLittleEndian
//				? BinaryPrimitives.ReverseEndianness(hostlong)
//				: hostlong;

//		public static ushort ntohs(ushort netshort) =>
//			BitConverter.IsLittleEndian
//				? BinaryPrimitives.ReverseEndianness(netshort)
//				: netshort;

//		public static uint ntohl(uint netlong) =>
//			BitConverter.IsLittleEndian
//				? BinaryPrimitives.ReverseEndianness(netlong)
//				: netlong;

//		[DllImport("libc", EntryPoint = "sendto")]
//		internal static extern IntPtr sendto_linux(
//			int sockfd,
//			void* buf,
//			IntPtr len,
//			int flags,
//			sockaddr* dest_addr,
//			uint addrlen);

//		[DllImport("Ws2_32.dll", EntryPoint = "sendto")]
//		internal static extern int sendto_win32(
//			IntPtr s,
//			byte* buf,
//			int len,
//			int flags,
//			sockaddr* to,
//			int tolen);

//		[DllImport("libc", EntryPoint = "recvfrom")]
//		internal static extern IntPtr recvfrom_linux(
//			int sockfd,
//			void* buf,
//			IntPtr len,
//			int flags,
//			sockaddr* src_addr,
//			uint* addrlen);

//		[DllImport("Ws2_32.dll", EntryPoint = "recvfrom")]
//		internal static extern int recvfrom_win32(
//			IntPtr s,
//			byte* buf,
//			int len,
//			int flags,
//			sockaddr* from,
//			int* fromlen);

		
//		[DllImport("Ws2_32.dll")]
//		internal static extern int WSAGetLastError();
//		// ReSharper restore InconsistentNaming
//		// ReSharper restore IdentifierTypo
//		// ReSharper restore StringLiteralTypo
//#pragma warning restore 649
//	}
//}
//#endif