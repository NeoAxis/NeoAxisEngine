// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	[StructLayout( LayoutKind.Sequential )]
	struct GUID
	{
		uint data1;
		ushort data2;
		ushort data3;
		byte data40;
		byte data41;
		byte data42;
		byte data43;
		byte data44;
		byte data45;
		byte data46;
		byte data47;

		//

		public GUID( uint data1, ushort data2, ushort data3, byte data40, byte data41, 
			byte data42, byte data43, byte data44, byte data45, byte data46, byte data47 )
		{
			this.data1 = data1;
			this.data2 = data2;
			this.data3 = data3;
			this.data40 = data40;
			this.data41 = data41;
			this.data42 = data42;
			this.data43 = data43;
			this.data44 = data44;
			this.data45 = data45;
			this.data46 = data46;
			this.data47 = data47;
		}

		public override string ToString()
		{
			return string.Format(
				"{0:x4}-{1:x4}-{2:x4}-{3:x4}{4:x4}-{5:x4}{6:x4}{7:x4}{8:x4}{9:x4}{10:x4}",
				data1, data2, data3, data40, data41, data42, data43, data44, data45, data46, data47 );
		}

		public static bool operator ==( GUID g1, GUID g2 )
		{
			return (
				g1.data1 == g2.data1 &&
				g1.data2 == g2.data2 &&
				g1.data3 == g2.data3 &&
				g1.data40 == g2.data40 &&
				g1.data41 == g2.data41 &&
				g1.data42 == g2.data42 &&
				g1.data43 == g2.data43 &&
				g1.data44 == g2.data44 &&
				g1.data45 == g2.data45 &&
				g1.data46 == g2.data46 &&
				g1.data47 == g2.data47 );
		}

		public static bool operator !=( GUID g1, GUID g2 )
		{
			return (
				g1.data1 != g2.data1 ||
				g1.data2 != g2.data2 ||
				g1.data3 != g2.data3 ||
				g1.data40 != g2.data40 ||
				g1.data41 != g2.data41 ||
				g1.data42 != g2.data42 ||
				g1.data43 != g2.data43 ||
				g1.data44 != g2.data44 ||
				g1.data45 != g2.data45 ||
				g1.data46 != g2.data46 ||
				g1.data47 != g2.data47 );
		}

		public override bool Equals( object obj )
		{
			return ( obj is GUID && this == (GUID)obj );
		}

		public override int GetHashCode()
		{
			return ( data1.GetHashCode() ^
				data2.GetHashCode() ^
				data3.GetHashCode() ^
				data40.GetHashCode() ^
				data41.GetHashCode() ^
				data42.GetHashCode() ^
				data43.GetHashCode() ^
				data44.GetHashCode() ^
				data45.GetHashCode() ^
				data46.GetHashCode() ^
				data47.GetHashCode() );
		}
	}
}
