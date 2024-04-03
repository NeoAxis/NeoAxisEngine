//// Copyright (c) 2015-2016 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
//// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//using SharpNav.Collections;
//using SharpNav.Geometry;
//using SharpNav.Pathfinding;

//#if MONOGAME
//using Vector3 = Microsoft.Xna.Framework.Vector3;
//#elif OPENTK
//using Vector3 = OpenTK.Vector3;
//#elif SHARPDX
//using Vector3 = SharpDX.Vector3;
//#endif

//namespace SharpNav.IO.Binary
//{
//    public class NavMeshBinarySerializer : NavMeshSerializer
//    {
//        private static byte[] Magic = { 0x73, 0x6E, 0x62, 0x63 }; // snbc
//        private const int FormatVersion = 1;

//		//!!!!betauser
//		public void Serialize( MemoryStream memoryStream, TiledNavMesh mesh )
//		{
//			using( var binaryWriter = new BinaryWriter( memoryStream, System.Text.Encoding.Default, true ) )
//			{
//				binaryWriter.Write( mesh.Origin.X );
//				binaryWriter.Write( mesh.Origin.Y );
//				binaryWriter.Write( mesh.Origin.Z );

//				binaryWriter.Write( mesh.TileWidth );
//				binaryWriter.Write( mesh.TileHeight );
//				binaryWriter.Write( mesh.MaxTiles );
//				binaryWriter.Write( mesh.MaxPolys );

//				var tiles = new List<byte[]>();
//				var navTiles = new List<NavTile>( mesh.Tiles );
//				binaryWriter.Write( navTiles.Count );

//				foreach( var tile in navTiles )
//				{
//					var baseRef = mesh.GetTileRef( tile );
//					tiles.Add( SerializeMeshTile( tile, baseRef ) );
//				}

//				foreach( var tile in tiles )
//					binaryWriter.Write( tile );
//			}
//		}

//		public override void Serialize(string path, TiledNavMesh mesh)
//        {
//            var memoryStream = new MemoryStream();

//            using (var binaryWriter = new BinaryWriter(memoryStream))
//            {
//                binaryWriter.Write(mesh.Origin.X);
//                binaryWriter.Write(mesh.Origin.Y);
//                binaryWriter.Write(mesh.Origin.Z);

//                binaryWriter.Write(mesh.TileWidth);
//                binaryWriter.Write(mesh.TileHeight);
//                binaryWriter.Write(mesh.MaxTiles);
//                binaryWriter.Write(mesh.MaxPolys);

//                var tiles = new List<byte[]>();
//                var navTiles = new List<NavTile>(mesh.Tiles);
//                binaryWriter.Write(navTiles.Count);

//                foreach (var tile in navTiles)
//                {
//                    var baseRef = mesh.GetTileRef(tile);
//                    tiles.Add(SerializeMeshTile(tile, baseRef));
//                }

//                foreach (var tile in tiles)
//                    binaryWriter.Write(tile);
//            }

//            using (var binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
//            {
//                binaryWriter.Write(Magic);
//                binaryWriter.Write(FormatVersion);

//                var tiledNavMesh = memoryStream.ToArray();
//                binaryWriter.Write(tiledNavMesh);
//            }
//        }

//		//!!!!betauser
//		public TiledNavMesh Deserialize( MemoryStream memoryStream )
//		{
//			using( var binaryReader = new BinaryReader( memoryStream, System.Text.Encoding.Default, true ) )
//			{
//				var x = binaryReader.ReadSingle();
//				var y = binaryReader.ReadSingle();
//				var z = binaryReader.ReadSingle();
//				var origin = new Vector3( x, y, z );

//				var tileWidth = binaryReader.ReadSingle();
//				var tileHeight = binaryReader.ReadSingle();
//				var maxTiles = (int)binaryReader.ReadSingle();
//				var maxPolys = (int)binaryReader.ReadSingle();
//				var tiledNavMesh = new TiledNavMesh( origin, tileWidth, tileHeight, maxTiles, maxPolys );

//				var navTileCount = binaryReader.ReadInt32();

//				for( var i = 0; i < navTileCount; i++ )
//				{
//					NavPolyId baseRef;
//					var stream = binaryReader.BaseStream;
//					var tile = DeserializeMeshTile( ref stream, tiledNavMesh.IdManager, out baseRef );
//					tiledNavMesh.AddTileAt( tile, baseRef );
//				}

//				return tiledNavMesh;
//			}
//		}

//		public override TiledNavMesh Deserialize(string path)
//        {
//            using (var binaryReader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
//            {
//                byte[] magic = binaryReader.ReadBytes(4);
//                if (!magic.SequenceEqual(Magic))
//                    throw new InvalidDataException("Invalid file magic!");

//                int version = binaryReader.ReadInt32();
//                if (version != FormatVersion)
//                    throw new InvalidDataException("Invalid file version!");

//                var x = binaryReader.ReadSingle();
//                var y = binaryReader.ReadSingle();
//                var z = binaryReader.ReadSingle();
//                var origin = new Vector3(x, y, z);

//                var tileWidth = binaryReader.ReadSingle();
//                var tileHeight = binaryReader.ReadSingle();
//                var maxTiles = (int) binaryReader.ReadSingle();
//                var maxPolys = (int) binaryReader.ReadSingle();
//                var tiledNavMesh = new TiledNavMesh(origin, tileWidth, tileHeight, maxTiles, maxPolys);

//                var navTileCount = binaryReader.ReadInt32();

//                for (var i = 0; i < navTileCount; i++)
//                {
//                    NavPolyId baseRef;
//                    var stream = binaryReader.BaseStream;
//                    var tile = DeserializeMeshTile(ref stream, tiledNavMesh.IdManager, out baseRef);
//                    tiledNavMesh.AddTileAt(tile, baseRef);
//                }

//                return tiledNavMesh;
//            }
//        }

//        private byte[] SerializeMeshTile(NavTile tile, NavPolyId baseRef)
//        {
//            var memoryStream = new MemoryStream();

//            using (var binaryWriter = new BinaryWriter(memoryStream))
//            {
//                binaryWriter.Write(baseRef.Id);

//                binaryWriter.Write(tile.Location.X);
//                binaryWriter.Write(tile.Location.Y);

//                binaryWriter.Write(tile.Layer);
//                binaryWriter.Write(tile.Salt);

//                binaryWriter.Write(tile.Bounds.Min.X);
//                binaryWriter.Write(tile.Bounds.Min.Y);
//                binaryWriter.Write(tile.Bounds.Min.Z);
//                binaryWriter.Write(tile.Bounds.Max.X);
//                binaryWriter.Write(tile.Bounds.Max.Y);
//                binaryWriter.Write(tile.Bounds.Max.Z);

//                var polys = new List<NavPoly>(tile.Polys);
//                binaryWriter.Write(polys.Count);

//                foreach (var poly in polys)
//                {
//                    binaryWriter.Write((byte) poly.PolyType);

//                    var polyLinks = new List<Link>(poly.Links);
//                    binaryWriter.Write(polyLinks.Count);

//                    foreach (var polyLink in polyLinks)
//                    {
//                        binaryWriter.Write(polyLink.Reference.Id);
//                        binaryWriter.Write(polyLink.Edge);
//                        binaryWriter.Write((byte) polyLink.Side);
//                        binaryWriter.Write(polyLink.BMin);
//                        binaryWriter.Write(polyLink.BMax);
//                    }

//                    var polyVerts = new List<int>(poly.Verts);
//                    binaryWriter.Write(polyVerts.Count);

//                    foreach (var polyVert in polyVerts)
//                        binaryWriter.Write(polyVert);

//                    var polyNeis = new List<int>(poly.Neis);
//                    binaryWriter.Write(polyNeis.Count);

//                    foreach (var polyNei in polyNeis)
//                        binaryWriter.Write(polyNei);

//                    if (poly.Tag == null)
//                        binaryWriter.Write((byte) 0xFE);
//                    else
//                        binaryWriter.Write((byte) poly.Tag);

//                    binaryWriter.Write(poly.VertCount);
//                    binaryWriter.Write(poly.Area.Id);
//                }

//                var verts = new List<Vector3>(tile.Verts);
//                binaryWriter.Write(verts.Count);

//                foreach (var vert in verts)
//                {
//                    binaryWriter.Write(vert.X);
//                    binaryWriter.Write(vert.Y);
//                    binaryWriter.Write(vert.Z);
//                }

//                var detailMeshes = new List<PolyMeshDetail.MeshData>(tile.DetailMeshes);
//                binaryWriter.Write(detailMeshes.Count);

//                foreach (var detailMesh in detailMeshes)
//                {
//                    binaryWriter.Write(detailMesh.VertexIndex);
//                    binaryWriter.Write(detailMesh.VertexCount);
//                    binaryWriter.Write(detailMesh.TriangleIndex);
//                    binaryWriter.Write(detailMesh.TriangleCount);
//                }

//                var detailVerts = new List<Vector3>(tile.DetailVerts);
//                binaryWriter.Write(detailVerts.Count);

//                foreach (var detailVert in detailVerts)
//                {
//                    binaryWriter.Write(detailVert.X);
//                    binaryWriter.Write(detailVert.Y);
//                    binaryWriter.Write(detailVert.Z);
//                }

//                var detailTris = new List<PolyMeshDetail.TriangleData>(tile.DetailTris);
//                binaryWriter.Write(detailTris.Count);

//                foreach (var detailTri in detailTris)
//                {
//                    binaryWriter.Write(detailTri.VertexHash0);
//                    binaryWriter.Write(detailTri.VertexHash1);
//                    binaryWriter.Write(detailTri.VertexHash2);
//                    binaryWriter.Write(detailTri.Flags);
//                }

//                var offMeshConnections = new List<OffMeshConnection>(tile.OffMeshConnections);
//                binaryWriter.Write(offMeshConnections.Count);

//                foreach (var offMeshConnection in offMeshConnections)
//                {
//                }

//                binaryWriter.Write(tile.BVTree.Count);

//                for (var i = 0; i < tile.BVTree.Count; i++)
//                {
//                    var node = tile.BVTree[i];

//                    binaryWriter.Write(node.Bounds.Min.X);
//                    binaryWriter.Write(node.Bounds.Min.Y);
//                    binaryWriter.Write(node.Bounds.Min.Z);
//                    binaryWriter.Write(node.Bounds.Max.X);
//                    binaryWriter.Write(node.Bounds.Max.Y);
//                    binaryWriter.Write(node.Bounds.Max.Z);
//                    binaryWriter.Write(node.Index);
//                }

//                binaryWriter.Write(tile.BvQuantFactor);
//                binaryWriter.Write(tile.BvNodeCount);
//                binaryWriter.Write(tile.WalkableClimb);
//            }

//            return memoryStream.ToArray();
//        }

//        private NavTile DeserializeMeshTile(ref Stream stream, NavPolyIdManager manager, out NavPolyId baseRef)
//        {
//            NavTile tile;

//            using (var binaryReader = new BinaryReader(stream))
//            {
//                var id = binaryReader.ReadInt32();
//                baseRef = new NavPolyId(id);

//                var x = binaryReader.ReadInt32();
//                var y = binaryReader.ReadInt32();
//                var location = new Vector2i(x, y);

//                var layer = binaryReader.ReadInt32();

//                tile = new NavTile(location, layer, manager, baseRef);
//                tile.Salt = binaryReader.ReadInt32();

//                var minX = binaryReader.ReadSingle();
//                var minY = binaryReader.ReadSingle();
//                var minZ = binaryReader.ReadSingle();
//                var maxX = binaryReader.ReadSingle();
//                var maxY = binaryReader.ReadSingle();
//                var maxZ = binaryReader.ReadSingle();
//                tile.Bounds = new BBox3(minX, minY, minZ, maxX, maxY, maxZ);

//                var polysCount = binaryReader.ReadInt32();
//                var polys = new NavPoly[polysCount];

//                for (var i = 0; i < polysCount; i++)
//                {
//                    var poly = new NavPoly();
//                    poly.PolyType = (NavPolyType) binaryReader.ReadByte();

//                    var polyLinksCount = binaryReader.ReadInt32();

//                    for (var j = 0; j < polyLinksCount; j++)
//                    {
//                        var navPolyId = binaryReader.ReadInt32();

//                        var link = new Link();
//                        link.Reference = new NavPolyId(navPolyId);
//                        link.Edge = binaryReader.ReadInt32();
//                        link.Side = (BoundarySide) binaryReader.ReadByte();
//                        link.BMin = binaryReader.ReadInt32();
//                        link.BMax = binaryReader.ReadInt32();

//                        poly.Links.Add(link);
//                    }

//                    var polyVertsCount = binaryReader.ReadInt32();
//                    poly.Verts = new int[polyVertsCount];

//                    for (var j = 0; j < polyVertsCount; j++)
//                        poly.Verts[j] = binaryReader.ReadInt32();

//                    var polyNeisCount = binaryReader.ReadInt32();
//                    poly.Neis = new int[polyNeisCount];

//                    for (var j = 0; j < polyNeisCount; j++)
//                        poly.Neis[j] = binaryReader.ReadInt32();

//                    var polyTag = binaryReader.ReadByte();

//                    if (polyTag == 0xFE)
//                        poly.Tag = null;
//                    else
//                        poly.Tag = (OffMeshConnectionFlags) polyTag;

//                    poly.VertCount = binaryReader.ReadInt32();

//                    var areaId = binaryReader.ReadByte();
//                    poly.Area = new Area(areaId);

//                    polys[i] = poly;
//                }

//                tile.Polys = polys;
//                tile.PolyCount = polysCount;

//                var vertsCount = binaryReader.ReadInt32();
//                var verts = new Vector3[vertsCount];

//                for (var i = 0; i < vertsCount; i++)
//                {
//                    var vx = binaryReader.ReadSingle();
//                    var vy = binaryReader.ReadSingle();
//                    var vz = binaryReader.ReadSingle();
//                    var vert = new Vector3(vx, vy, vz);

//                    verts[i] = vert;
//                }

//                tile.Verts = verts;

//                var detailMeshesCount = binaryReader.ReadInt32();
//                var detailMeshes = new PolyMeshDetail.MeshData[detailMeshesCount];

//                for (var i = 0; i < detailMeshesCount; i++)
//                {
//                    var detailMesh = new PolyMeshDetail.MeshData();
//                    detailMesh.VertexIndex = binaryReader.ReadInt32();
//                    detailMesh.VertexCount = binaryReader.ReadInt32();
//                    detailMesh.TriangleIndex = binaryReader.ReadInt32();
//                    detailMesh.TriangleCount = binaryReader.ReadInt32();

//                    detailMeshes[i] = detailMesh;
//                }

//                tile.DetailMeshes = detailMeshes;

//                var detailVertsCount = binaryReader.ReadInt32();
//                var detailVerts = new Vector3[detailVertsCount];

//                for (var i = 0; i < detailVertsCount; i++)
//                {
//                    var vx = binaryReader.ReadSingle();
//                    var vy = binaryReader.ReadSingle();
//                    var vz = binaryReader.ReadSingle();
//                    var detailVert = new Vector3(vx, vy, vz);

//                    detailVerts[i] = detailVert;
//                }

//                tile.DetailVerts = detailVerts;

//                var detailTrisCount = binaryReader.ReadInt32();
//                var detailTris = new PolyMeshDetail.TriangleData[detailTrisCount];

//                for (var i = 0; i < detailTrisCount; i++)
//                {
//                    var hash0 = binaryReader.ReadInt32();
//                    var hash1 = binaryReader.ReadInt32();
//                    var hash2 = binaryReader.ReadInt32();
//                    var flags = binaryReader.ReadInt32();
//                    var detailTri = new PolyMeshDetail.TriangleData(hash0, hash1, hash2, flags);

//                    detailTris[i] = detailTri;
//                }

//                tile.DetailTris = detailTris;

//                var offMeshConnectionsCount = binaryReader.ReadInt32();

//                for (var i = 0; i < offMeshConnectionsCount; i++)
//                {
//                }

//                var nodesCount = binaryReader.ReadInt32();
//                var nodes = new BVTree.Node[nodesCount];

//                for (var i = 0; i < nodesCount; i++)
//                {
//                    var node = new BVTree.Node();
//                    node.Bounds.Min.X = binaryReader.ReadInt32();
//                    node.Bounds.Min.Y = binaryReader.ReadInt32();
//                    node.Bounds.Min.Z = binaryReader.ReadInt32();
//                    node.Bounds.Max.X = binaryReader.ReadInt32();
//                    node.Bounds.Max.Y = binaryReader.ReadInt32();
//                    node.Bounds.Max.Z = binaryReader.ReadInt32();
//                    node.Index = binaryReader.ReadInt32();

//                    nodes[i] = node;
//                }

//                tile.BVTree = new BVTree(nodes);

//                tile.BvQuantFactor = binaryReader.ReadSingle();
//                tile.BvNodeCount = binaryReader.ReadInt32();
//                tile.WalkableClimb = binaryReader.ReadSingle();
//            }

//            return tile;
//        }
//    }
//}