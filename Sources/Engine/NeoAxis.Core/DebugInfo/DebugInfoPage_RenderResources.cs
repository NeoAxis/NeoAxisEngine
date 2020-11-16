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

				int textures = 0;
				long texturesSizeManaged = 0;
				long texturesSizeGPU = 0;
				int renderTargets = 0;
				long renderTargetsSizeGPU = 0;

				foreach( var texture in GpuTexture.GetInstances() )
				{
					if( !texture.Usage.HasFlag( GpuTexture.Usages.RenderTarget ) )
					{
						textures++;

						var data = texture.GetData();
						if( data != null )
						{
							foreach( var surfaceData in data )
							{
								if( surfaceData.data != null )
									texturesSizeManaged += surfaceData.data.Length;
							}
						}

						var realObject = texture.GetRealObject( false );
						if( realObject != null )
							texturesSizeGPU += realObject.SizeInBytes;
					}
					else
					{
						renderTargets++;

						var realObject = texture.GetRealObject( false );
						if( realObject != null )
							renderTargetsSizeGPU += realObject.SizeInBytes;
					}
				}

				lines.Add( Translate( "Textures" ) + ": " + textures.ToString( "N0", nfi ) );
				lines.Add( Translate( "Textures managed memory" ) + ": " + string.Format( "{0} MB", ( texturesSizeManaged / 1024 / 1024 ).ToString( "N0" ) ) );
				lines.Add( Translate( "Textures GPU memory" ) + ": " + string.Format( "{0} MB", ( texturesSizeGPU / 1024 / 1024 ).ToString( "N0" ) ) );

				lines.Add( Translate( "Render targets" ) + ": " + renderTargets.ToString( "N0", nfi ) );
				lines.Add( Translate( "Render targets GPU memory" ) + ": " + string.Format( "{0} MB", ( renderTargetsSizeGPU / 1024 / 1024 ).ToString( "N0" ) ) );

				long vertexBuffersSizeManaged = 0;
				long vertexBuffersSizeGPU = 0;
				foreach( var buffer in GpuBufferManager.VertexBuffers )
				{
					if( buffer.Vertices != null )
						vertexBuffersSizeManaged += buffer.Vertices.Length;
					if( buffer.RealObject != null )
						vertexBuffersSizeGPU += buffer.VertexSize * buffer.VertexCount;
				}

				lines.Add( Translate( "Vertex buffers" ) + ": " + GpuBufferManager.VertexBuffers.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "Vertex buffers managed memory" ) + ": " + string.Format( "{0} MB", ( vertexBuffersSizeManaged / 1024 / 1024 ).ToString( "N0" ) ) );
				lines.Add( Translate( "Vertex buffers GPU memory" ) + ": " + string.Format( "{0} MB", ( vertexBuffersSizeGPU / 1024 / 1024 ).ToString( "N0" ) ) );

				long indexBuffersSizeManaged = 0;
				long indexBuffersSizeGPU = 0;
				foreach( var buffer in GpuBufferManager.IndexBuffers )
				{
					if( buffer.Indices != null )
						indexBuffersSizeManaged += buffer.Indices.Length * 4;
					if( buffer.RealObject != null )
						indexBuffersSizeGPU += buffer.IndexCount * ( buffer.RealObject16Bit ? 2 : 4 );
				}

				lines.Add( Translate( "Index buffers" ) + ": " + GpuBufferManager.IndexBuffers.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "Index buffers managed memory" ) + ": " + string.Format( "{0} MB", ( indexBuffersSizeManaged / 1024 / 1024 ).ToString( "N0" ) ) );
				lines.Add( Translate( "Index buffers GPU memory" ) + ": " + string.Format( "{0} MB", ( indexBuffersSizeGPU / 1024 / 1024 ).ToString( "N0" ) ) );

				lines.Add( Translate( "GPU programs" ) + ": " + GpuProgramManager.Programs.Count.ToString( "N0", nfi ) );
				lines.Add( Translate( "GPU linked programs" ) + ": " + GpuProgramManager.LinkedPrograms.Count.ToString( "N0", nfi ) );

				int materials = 0;
				int materialsEnabled = 0;
				foreach( var material in Component_Material.GetInstances() )
				{
					materials++;
					if( material.EnabledInHierarchy )
						materialsEnabled++;
				}

				lines.Add( Translate( "Materials" ) + ": " + materials.ToString( "N0", nfi ) );
				lines.Add( Translate( "Materials enabled" ) + ": " + materialsEnabled.ToString( "N0", nfi ) );

				int meshes = 0;
				int meshesEnabled = 0;
				long meshesSizeManaged = 0;
				long meshesSizeGPUBuffers = 0;

				foreach( var mesh in Component_Mesh.GetInstances() )
				{
					meshes++;
					if( mesh.EnabledInHierarchy )
					{
						meshesEnabled++;

						var result = mesh.Result;
						if( result != null )
						{
							unsafe
							{
								var extractedVertices = result.GetExtractedVertices( false );
								if( extractedVertices != null )
									meshesSizeManaged += extractedVertices.Length * sizeof( StandardVertex );
								if( result.ExtractedVerticesPositions != null )
									meshesSizeManaged += result.ExtractedVerticesPositions.Length * sizeof( Vector3F );
								if( result.ExtractedIndices != null )
									meshesSizeManaged += result.ExtractedIndices.Length * sizeof( int );
							}

							//!!!!ray cast data

							if( result.MeshData != null )
							{
								//!!!!могут повторяться одинаковые

								foreach( var oper in result.MeshData.RenderOperations )
								{
									if( oper.VertexBuffers != null )
									{
										foreach( var buffer in oper.VertexBuffers )
										{
											if( buffer.RealObject != null )
												meshesSizeGPUBuffers += buffer.VertexSize * buffer.VertexCount;
										}
									}

									{
										var buffer = oper.IndexBuffer;
										if( buffer != null && buffer.RealObject != null )
											meshesSizeGPUBuffers += buffer.IndexCount * ( buffer.RealObject16Bit ? 2 : 4 );
									}
								}
							}
						}
					}
				}

				lines.Add( Translate( "Meshes" ) + ": " + meshes.ToString( "N0", nfi ) );
				lines.Add( Translate( "Meshes enabled" ) + ": " + meshesEnabled.ToString( "N0", nfi ) );
				lines.Add( Translate( "Meshes managed memory" ) + ": " + string.Format( "{0} MB", ( meshesSizeManaged / 1024 / 1024 ).ToString( "N0" ) ) );
				lines.Add( Translate( "Meshes GPU buffers memory" ) + ": " + string.Format( "{0} MB", ( meshesSizeGPUBuffers / 1024 / 1024 ).ToString( "N0" ) ) );

				return lines;
			}
		}
	}
}
