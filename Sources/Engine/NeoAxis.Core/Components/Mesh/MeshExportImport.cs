// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using Fbx;

namespace NeoAxis
{
	static class MeshExportImport
	{
		static FbxColor ToFbxColor( ColorValue v )
		{
			return new FbxColor( v.Red, v.Green, v.Blue, v.Alpha );
		}

		static FbxVector4 ToFbxVector4( Vector3F v )
		{
			return new FbxVector4( v.X, v.Y, v.Z, 0 );
		}

		static FbxVector2 ToFbxVector2( Vector2F v )
		{
			return new FbxVector2( v.X, v.Y );
		}

		public static bool ExportToFBX( Component_Mesh sourceMesh, string realFileName, out string error )
		{

			//!!!!как для Vegetation. оверрайдить в Component_Mesh?

			//get mesh data
			var operations = new List<Component_RenderingPipeline.RenderSceneData.MeshDataRenderOperation>();
			foreach( var geometry in sourceMesh.GetComponents<Component_MeshGeometry>() )
			{
				if( geometry.Enabled )
				{
					geometry.CompileDataOfThisObject( out var operation );
					if( operation != null )
						operations.Add( operation );
				}
			}
			//foreach( var geometry in mesh.Result.MeshData.RenderOperations )
			//{
			//}

			FbxManager manager = null;
			FbxIOSettings setting = null;
			FbxExporter exporter = null;
			FbxScene scene = null;
			try
			{
				//init FBX manager
				manager = FbxManager.Create();
				setting = FbxIOSettings.Create( manager, "IOSRoot" );
				manager.SetIOSettings( setting );

				scene = FbxScene.Create( manager, "scene" );
				scene.GetGlobalSettings().SetAxisSystem( new FbxAxisSystem( FbxAxisSystem.EPreDefinedAxisSystem.eMax ) );
				scene.GetGlobalSettings().SetSystemUnit( new FbxSystemUnit( 100 ) );

				//init FBX scene
				for( int nOper = 0; nOper < operations.Count; nOper++ )
				{
					var oper = operations[ nOper ];

					//get data

					Vector3F[] positions = null;
					Vector3F[] normals = null;
					var texCoords = new List<Vector2F[]>();
					ColorValue[] colors = null;
					Vector3F[] tangents = null;
					Vector3F[] binormals = null;

					//Position
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Position, out VertexElement element ) && element.Type == VertexElementType.Float3 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							positions = buffer.ExtractChannel<Vector3F>( element.Offset );
						}
					}

					//Normal
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Normal, out VertexElement element ) && element.Type == VertexElementType.Float3 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							normals = buffer.ExtractChannel<Vector3F>( element.Offset );
						}
					}

					//TexCoord
					for( var channel = VertexElementSemantic.TextureCoordinate0; channel <= VertexElementSemantic.TextureCoordinate3; channel++ )
					{
						if( oper.VertexStructure.GetElementBySemantic( channel, out VertexElement element ) && element.Type == VertexElementType.Float2 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							texCoords.Add( buffer.ExtractChannel<Vector2F>( element.Offset ) );
						}
					}

					//Color
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out VertexElement element ) )
						{
							if( element.Type == VertexElementType.Float4 )
							{
								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<Vector4F>( element.Offset );
								colors = new ColorValue[ positions.Length ];
								int destIndex = 0;
								foreach( var p in values )
									colors[ destIndex++ ] = p.ToColorValue();
							}
							else if( element.Type == VertexElementType.ColorABGR )
							{
								//!!!!check

								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<uint>( element.Offset );
								colors = new ColorValue[ positions.Length ];
								int destIndex = 0;
								foreach( var p in values )
									colors[ destIndex++ ] = new ColorValue( ColorByte.FromABGR( p ) );
							}
							else if( element.Type == VertexElementType.ColorARGB )
							{
								//!!!!check

								var buffer = oper.VertexBuffers[ element.Source ];
								var values = buffer.ExtractChannel<uint>( element.Offset );
								colors = new ColorValue[ positions.Length ];
								int destIndex = 0;
								foreach( var p in values )
									colors[ destIndex++ ] = new ColorValue( ColorByte.FromARGB( p ) );
							}
						}
					}

					//Tangent, Binormal
					if( normals != null )
					{
						if( oper.VertexStructure.GetElementBySemantic( VertexElementSemantic.Tangent, out VertexElement element ) && element.Type == VertexElementType.Float4 )
						{
							var buffer = oper.VertexBuffers[ element.Source ];
							var tangents4 = buffer.ExtractChannel<Vector4F>( element.Offset );

							tangents = new Vector3F[ tangents4.Length ];
							binormals = new Vector3F[ tangents4.Length ];

							int destIndex = 0;
							foreach( var p in tangents4 )
							{
								tangents[ destIndex ] = p.ToVector3F();
								binormals[ destIndex ] = Vector3F.Cross( p.ToVector3F(), normals[ destIndex ] ) * p.W;
								destIndex++;
							}
						}
					}

					//indices
					int[] indices = null;
					if( oper.IndexBuffer != null )
						indices = oper.IndexBuffer.Indices;


					//create geometry

					var geometryName = "Geometry " + nOper.ToString();
					var mesh = FbxMesh.Create( scene, geometryName );

					mesh.InitControlPoints( positions.Length );

					FbxLayerElementNormal elementNormals = null;
					if( normals != null )
					{
						elementNormals = mesh.CreateElementNormal();
						elementNormals.SetMappingMode( FbxLayerElement.EMappingMode.eByControlPoint );
						elementNormals.SetReferenceMode( FbxLayerElement.EReferenceMode.eDirect );
					}

					FbxLayerElementVertexColor elementColors = null;
					if( colors != null )
					{
						elementColors = mesh.CreateElementVertexColor();
						elementColors.SetMappingMode( FbxLayerElement.EMappingMode.eByControlPoint );
						elementColors.SetReferenceMode( FbxLayerElement.EReferenceMode.eDirect );
					}

					FbxLayerElementTangent elementTangents = null;
					if( tangents != null )
					{
						elementTangents = mesh.CreateElementTangent();
						elementTangents.SetMappingMode( FbxLayerElement.EMappingMode.eByControlPoint );
						elementTangents.SetReferenceMode( FbxLayerElement.EReferenceMode.eDirect );
					}

					FbxLayerElementBinormal elementBinormals = null;
					if( binormals != null )
					{
						elementBinormals = mesh.CreateElementBinormal();
						elementBinormals.SetMappingMode( FbxLayerElement.EMappingMode.eByControlPoint );
						elementBinormals.SetReferenceMode( FbxLayerElement.EReferenceMode.eDirect );
					}

					var uvElements = new List<FbxLayerElementUV>();
					for( int uvIndex = 0; uvIndex < texCoords.Count; uvIndex++ )
					{
						var pUVElement = mesh.CreateElementUV( "texcoord" + uvIndex.ToString() );
						pUVElement.SetMappingMode( FbxLayerElement.EMappingMode.eByControlPoint );
						pUVElement.SetReferenceMode( FbxLayerElement.EReferenceMode.eDirect );

						uvElements.Add( pUVElement );
					}

					for( int n = 0; n < positions.Length; n++ )
					{
						mesh.SetControlPointAt( ToFbxVector4( positions[ n ] ), n );

						if( normals != null )
							elementNormals.GetDirectArray().Add( ToFbxVector4( normals[ n ] ) );

						for( int uvIndex = 0; uvIndex < texCoords.Count; uvIndex++ )
						{
							var texCoord = texCoords[ uvIndex ][ n ];
							texCoord.Y = 1.0f - texCoord.Y;
							uvElements[ uvIndex ].GetDirectArray().Add( ToFbxVector2( texCoord ) );
						}

						if( colors != null )
							elementColors.GetDirectArray().Add( ToFbxColor( colors[ n ] ) );

						if( tangents != null )
							elementTangents.GetDirectArray().Add( ToFbxVector4( tangents[ n ] ) );

						if( binormals != null )
							elementBinormals.GetDirectArray().Add( ToFbxVector4( binormals[ n ] ) );
					}

					if( normals != null )
						mesh.GetLayer( 0 ).SetNormals( elementNormals );
					if( colors != null )
						mesh.GetLayer( 0 ).SetVertexColors( elementColors );
					if( tangents != null )
						mesh.GetLayer( 0 ).SetTangents( elementTangents );
					if( binormals != null )
						mesh.GetLayer( 0 ).SetBinormals( elementBinormals );


					int polygonCount = indices.Length / 3;
					for( int i = 0; i < polygonCount; i++ )
					{
						mesh.BeginPolygon( -1, -1, -1, false );
						for( int j = 0; j < 3; j++ )
						{
							int currentIndex = i * 3 + j;
							int vertexIndex = indices[ currentIndex ];
							mesh.AddPolygon( vertexIndex );
						}
						mesh.EndPolygon();
					}

					var node = FbxNode.Create( scene, geometryName );
					node.SetNodeAttribute( mesh );

					scene.GetRootNode().AddChild( mesh.GetNode() );
				}

				//save

				exporter = FbxExporter.Create( manager, "" );
				if( !exporter.Initialize( realFileName, -1, manager.GetIOSettings() ) )
				{
					error = "Can't initialize FBX exporter.";
					return false;
				}

				if( !exporter.Export( scene ) )
				{
					error = "Export to FBX failed.";
					return false;
				}

			}
			finally
			{
				try { scene?.Destroy(); } catch { }
				try { exporter?.Destroy(); } catch { }
				try { setting?.Destroy(); } catch { }
				try { manager?.Destroy(); } catch { }
			}

			foreach( var op in operations )
				op.DisposeBuffers();

			error = "";
			return true;
		}
	}
}
