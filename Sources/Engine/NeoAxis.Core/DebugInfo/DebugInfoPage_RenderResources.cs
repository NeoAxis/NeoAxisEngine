// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using SharpBgfx;
using System.Globalization;

namespace NeoAxis
{
	/// <summary>
	/// Represents a page with information about render resources for Debug Window of the editor.
	/// </summary>
	public class DebugInfoPage_RenderResources : DebugInfoPage
	{
		public override string Title
		{
			get { return "Render: Resources"; }
		}

		public override List<string> Content
		{
			get
			{
				var lines = new List<string>();

				NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
				nfi.NumberGroupSeparator = " ";

				int renderTargets = 0;
				foreach( var t in GpuTexture.Instances )
				{
					if( t.Usage.HasFlag( GpuTexture.Usages.RenderTarget ) )
						renderTargets++;
				}

				lines.Add( Translate( "Textures" ) + ": " + GpuTexture.Instances.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "Render targets" ) + ": " + renderTargets.ToString( "N0", nfi ) );
				lines.Add( Translate( "Vertex buffers" ) + ": " + GpuBufferManager.VertexBuffers.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "Index buffers" ) + ": " + GpuBufferManager.IndexBuffers.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "GPU programs" ) + ": " + GpuProgramManager.Programs.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "GPU linked programs" ) + ": " + GpuProgramManager.LinkedPrograms.Count.ToString( "N0", nfi ) );

				return lines;
			}
		}
	}
}
