// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;

namespace NeoAxis
{
	/// <summary>
	/// Represents a page with memory using information for Debug Window of the editor.
	/// </summary>
	public class DebugInfoPage_Memory : DebugInfoPage
	{
		public override string Title
		{
			get { return "Memory"; }
		}

		public override List<string> Content
		{
			get
			{
				var lines = new List<string>();

				NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
				nfi.NumberGroupSeparator = " ";

				lines.Add( Translate( ".NET memory" ) + ": " + GC.GetTotalMemory( false ).ToString( "N0", nfi ) );

				//!!!!пока не нужно
				//{
				//	NativeMemoryManager.GetStatistics( NativeUtility.MemoryAllocationType.Count, out var size, out var allocations );
				//	lines.Add( Translate( "Native allocator memory used by app" ) + ": " + size.ToString( "N0", nfi ) );
				//	//lines.Add( string.Format( "Native allocator requested size: " + size.ToString( "N0", nfi ) ) );
				//	//lines.Add( string.Format( "Native allocator requested allocations: " + allocations.ToString( "N0", nfi ) ) );
				//}

				{
					NativeMemoryManager.GetCRTStatistics( out var size, out var allocations );
					lines.Add( Translate( "Native allocator actual memory used" ) + ": " + size.ToString( "N0", nfi ) );
					//lines.Add( string.Format( "Native allocator CRT size: " + size.ToString( "N0", nfi ) ) );
					//lines.Add( string.Format( "Native allocator CRT allocations: " + allocations.ToString( "N0", nfi ) ) );
				}

				//{
				//	long totalSize = 0;
				//	int totalAllocations = 0;
				//	var sizeByType = new long[ (int)NativeUtility.MemoryAllocationType.Count ];
				//	var allocationByType = new int[ (int)NativeUtility.MemoryAllocationType.Count ];

				//	for( int n = 0; n < (int)NativeUtility.MemoryAllocationType.Count; n++ )
				//	{
				//		NativeMemoryManager.GetStatistics( (NativeUtility.MemoryAllocationType)n, out var size, out var allocations );
				//		sizeByType[ n ] = size;
				//		allocationByType[ n ] = allocations;
				//		totalSize += size;
				//		totalAllocations += allocations;
				//	}

				//	for( int n = 0; n < (int)NativeUtility.MemoryAllocationType.Count; n++ )
				//	{
				//		var type = (NativeUtility.MemoryAllocationType)n;

				//		var size = sizeByType[ n ];
				//		var allocations = allocationByType[ n ];
				//		lines.Add( string.Format( "Native allocator " + type.ToString() + " size: " + size.ToString( "N0", nfi ) ) );
				//		lines.Add( string.Format( "Native allocator " + type.ToString() + " allocations: " + allocations.ToString( "N0", nfi ) ) );
				//	}
				//}

				return lines;
			}
		}
	}
}
