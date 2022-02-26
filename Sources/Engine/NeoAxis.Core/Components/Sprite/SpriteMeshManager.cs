// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
			public Mesh Mesh;
			public double LastUsedTime;

			//

			public Item( RectangleF uv )
			{
				UV = uv;

				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var positions = new Vector3F[] { new Vector3F( -0.5f, -0.5f, 0 ), new Vector3F( 0.5f, -0.5f, 0 ), new Vector3F( 0.5f, 0.5f, 0 ), new Vector3F( -0.5f, 0.5f, 0 ) };
				var texCoords = new Vector2F[] { uv.LeftTop, uv.RightTop, uv.RightBottom, uv.LeftBottom };

				var vertices = new byte[ vertexSize * positions.Length ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
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

				var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
				var geometry = mesh.CreateComponent<MeshGeometry>();
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

		static SpriteMeshManager()
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				MaxCacheSize = 200;
		}

		public static Mesh GetMesh( RectangleF uv )
		{
			//try to get from the cache
			if( items.TryGetValue( uv, out var item ) )
			{
				item.LastUsedTime = Time.Current;
				return item.Mesh;
			}

			//remove oldest item from the cache
			if( items.Count >= MaxCacheSize )
			{
				Item oldestItem = null;

				foreach( var pair in items )
				{
					if( pair.Value.LastUsedTime != Time.Current )
					{
						if( oldestItem == null || pair.Value.LastUsedTime < oldestItem.LastUsedTime )
							oldestItem = pair.Value;
					}
				}

				if( oldestItem != null )
				{
					oldestItem.Dispose();
					items.Remove( oldestItem.UV );
				}
			}

			//create item and add to the cache
			item = new Item( uv );
			items.Add( uv, item );

			item.LastUsedTime = Time.Current;

			return item.Mesh;
		}
	}
}
