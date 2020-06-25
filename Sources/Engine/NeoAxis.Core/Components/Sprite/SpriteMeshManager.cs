// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	public static class SpriteMeshManager
	{
		public static int MaxCacheSize = 100;

		static EDictionary<RectangleF, Item> items = new EDictionary<RectangleF, Item>();

		/////////////////////////////////////////

		class Item
		{
			public RectangleF UV;
			public double EngineTime;
			public Component_Mesh Mesh;

			//

			public Item( RectangleF uv )
			{
				UV = uv;
				EngineTime = Time.Current;

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var positions = new Vector3F[] { new Vector3F( -0.5f, -0.5f, 0 ), new Vector3F( 0.5f, -0.5f, 0 ), new Vector3F( 0.5f, 0.5f, 0 ), new Vector3F( -0.5f, 0.5f, 0 ) };
				var texCoords = new Vector2F[] { uv.LeftTop, uv.RightTop, uv.RightBottom, uv.LeftBottom };

				var vertices = new byte[ vertexSize * positions.Length ];
				unsafe
				{
					fixed ( byte* pVertices = vertices )
					{
						StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

						for( int n = 0; n < positions.Length; n++ )
						{
							pVertex->Position = positions[ n ];
							pVertex->Normal = new Vector3F( 0, 0, 1 );
							pVertex->Tangent = new Vector4F( 1, 0, 0, -1 );
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = texCoords[ n ];

							pVertex++;
						}
					}
				}

				var mesh = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
				var geometry = mesh.CreateComponent<Component_MeshGeometry>();
				geometry.VertexStructure = vertexStructure;
				geometry.Vertices = vertices;
				geometry.Indices = new int[] { 0, 1, 2, 2, 3, 0 };
				mesh.Enabled = true;

				Mesh = mesh;
			}

			public void Dispose()
			{
				Mesh?.Dispose();
			}
		}

		/////////////////////////////////////////

		public static Component_Mesh GetMesh( RectangleF uv )
		{
			//try to get from the cache
			if( items.TryGetValue( uv, out var item ) )
			{
				item.EngineTime = Time.Current;

				items.Remove( uv );
				items.Add( uv, item );

				return item.Mesh;
			}

			//remove old item from the cache
			if( items.Count >= MaxCacheSize )
			{
				foreach( var pair in items )
				{
					if( pair.Value.EngineTime != Time.Current )
					{
						pair.Value.Dispose();
						items.Remove( pair.Key );
						break;
					}
				}
			}

			//create item and add to the cache
			item = new Item( uv );
			items.Add( uv, item );

			return item.Mesh;
		}
	}
}
