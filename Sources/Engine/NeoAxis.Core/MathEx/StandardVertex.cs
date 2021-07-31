// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// The declaration of the standard vertex. This structure of the mesh vertex data can be used in most cases.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct StandardVertex
	{
		public Vector3F Position;
		public Vector3F Normal;
		public Vector4F Tangent;
		public ColorValue Color;
		public Vector2F TexCoord0;
		public Vector2F TexCoord1;
		public Vector2F TexCoord2;
		public Vector2F TexCoord3;
		public Vector4I BlendIndices;
		public Vector4F BlendWeights;

		//Vec4F specialChannel0;
		//Vec4F specialChannel1;

		///////////////////////////////////////////

		/// <summary>
		/// The enumeration to specify vertex structure.
		/// </summary>
		[Flags]
		public enum Components
		{
			Position = 1,
			Normal = 2,
			Tangent = 4,
			Color = 8,
			TexCoord0 = 16,
			TexCoord1 = 32,
			TexCoord2 = 64,
			TexCoord3 = 128,
			BlendIndices = 256,
			BlendWeights = 512,
			All = Position | Normal | Tangent | Color | TexCoord0 | TexCoord1 | TexCoord2 | TexCoord3 | BlendIndices | BlendWeights,

			//!!!!так?
			StaticOneTexCoord = Position | Normal | Tangent | Color | TexCoord0,
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents a simplified version of <see cref="StandardVertex"/> with one channel of texture coordinates and without animation data.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct StaticOneTexCoord
		{
			public Vector3F Position;
			public Vector3F Normal;
			public Vector4F Tangent;
			public ColorValue Color;
			public Vector2F TexCoord0;
		}

		///////////////////////////////////////////

		/// <summary>
		/// The constructor of the vertex.
		/// </summary>
		/// <param name="position">The position of the vertex.</param>
		/// <param name="normal">The normal of the vertex.</param>
		/// <param name="tangent">The tangent vector of the vertex.</param>
		/// <param name="color">The color of the vertex.</param>
		/// <param name="texCoord0">The texture coordinate 0 of the vertex.</param>
		/// <param name="texCoord1">The texture coordinate 1 of the vertex.</param>
		/// <param name="texCoord2">The texture coordinate 2 of the vertex.</param>
		/// <param name="texCoord3">The texture coordinate 3 of the vertex.</param>
		public StandardVertex( Vector3F position, Vector3F normal, Vector4F tangent, ColorValue color, Vector2F texCoord0, Vector2F texCoord1, Vector2F texCoord2, Vector2F texCoord3, Vector4I blendIndices, Vector4F blendWeights )
		{
			this.Position = position;
			this.Normal = normal;
			this.Tangent = Vector4F.Zero;
			this.Color = color;
			this.TexCoord0 = texCoord0;
			this.TexCoord1 = texCoord1;
			this.TexCoord2 = texCoord2;
			this.TexCoord3 = texCoord3;
			this.BlendIndices = blendIndices;
			this.BlendWeights = blendWeights;
		}

		/// <summary>
		/// The constructor of the vertex.
		/// </summary>
		/// <param name="position">The position of the vertex.</param>
		/// <param name="normal">The normal of the vertex.</param>
		/// <param name="tangent">The tangent vector of the vertex.</param>
		/// <param name="texCoord0">The texture coordinate 0 of the vertex.</param>
		public StandardVertex( Vector3F position, Vector3F normal, Vector4F tangent, Vector2F texCoord0 )
		{
			this.Position = position;
			this.Normal = normal;
			this.Tangent = tangent;
			this.Color = new ColorValue( 1, 1, 1, 1 );
			this.TexCoord0 = texCoord0;
			this.TexCoord1 = Vector2F.Zero;
			this.TexCoord2 = Vector2F.Zero;
			this.TexCoord3 = Vector2F.Zero;
			this.BlendIndices = Vector4I.Zero;
			this.BlendWeights = Vector4F.Zero;
		}

		/// <summary>
		/// The constructor of the vertex.
		/// </summary>
		/// <param name="position">The position of the vertex.</param>
		/// <param name="normal">The normal of the vertex.</param>
		/// <param name="tangent">The tangent vector of the vertex.</param>
		public StandardVertex( Vector3F position, Vector3F normal, Vector4F tangent )
		{
			this.Position = position;
			this.Normal = normal;
			this.Tangent = tangent;
			this.Color = new ColorValue( 1, 1, 1, 1 );
			this.TexCoord0 = Vector2F.Zero;
			this.TexCoord1 = Vector2F.Zero;
			this.TexCoord2 = Vector2F.Zero;
			this.TexCoord3 = Vector2F.Zero;
			this.BlendIndices = Vector4I.Zero;
			this.BlendWeights = Vector4F.Zero;
		}

		/// <summary>
		/// The constructor of the vertex.
		/// </summary>
		/// <param name="position">The position of the vertex.</param>
		public StandardVertex( Vector3F position )
		{
			this.Position = position;
			this.Normal = Vector3F.Zero;
			this.Tangent = Vector4F.Zero;
			this.Color = new ColorValue( 1, 1, 1, 1 );
			this.TexCoord0 = Vector2F.Zero;
			this.TexCoord1 = Vector2F.Zero;
			this.TexCoord2 = Vector2F.Zero;
			this.TexCoord3 = Vector2F.Zero;
			this.BlendIndices = Vector4I.Zero;
			this.BlendWeights = Vector4F.Zero;
		}

		public static int GetOffsetInBytes( Components component )
		{
			switch( component )
			{
			case Components.Position: return 0;
			case Components.Normal: return 12;
			case Components.Color: return 24;
			case Components.Tangent: return 40;
			case Components.TexCoord0: return 56;
			case Components.TexCoord1: return 64;
			case Components.TexCoord2: return 72;
			case Components.TexCoord3: return 80;
			case Components.BlendIndices: return 88;
			case Components.BlendWeights: return 104;
			}

			return 0;
		}

		public static int GetSizeInBytes( Components component )
		{
			switch( component )
			{
			case Components.Position: return 12;
			case Components.Normal: return 12;
			case Components.Tangent: return 16;
			case Components.Color: return 16;
			case Components.TexCoord0: return 8;
			case Components.TexCoord1: return 8;
			case Components.TexCoord2: return 8;
			case Components.TexCoord3: return 8;
			case Components.BlendIndices: return 16;
			case Components.BlendWeights: return 16;
			}
			return 0;
		}

		public static object ExtractOneComponentArray( StandardVertex[] vertices, Components component )
		{
			switch( component )
			{
			case Components.Position:
				{
					Vector3F[] array = new Vector3F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].Position;
					return array;
				}

			case Components.Normal:
				{
					Vector3F[] array = new Vector3F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].Normal;
					return array;
				}

			case Components.Tangent:
				{
					Vector4F[] array = new Vector4F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].Tangent;
					return array;
				}

			case Components.Color:
				{
					ColorValue[] array = new ColorValue[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].Color;
					return array;
				}

			case Components.TexCoord0:
				{
					Vector2F[] array = new Vector2F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].TexCoord0;
					return array;
				}

			case Components.TexCoord1:
				{
					Vector2F[] array = new Vector2F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].TexCoord1;
					return array;
				}

			case Components.TexCoord2:
				{
					Vector2F[] array = new Vector2F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].TexCoord2;
					return array;
				}

			case Components.TexCoord3:
				{
					Vector2F[] array = new Vector2F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].TexCoord3;
					return array;
				}

			case Components.BlendIndices:
				{
					Vector4I[] array = new Vector4I[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].BlendIndices;
					return array;
				}

			case Components.BlendWeights:
				{
					Vector4F[] array = new Vector4F[ vertices.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						array[ n ] = vertices[ n ].BlendWeights;
					return array;
				}
			}

			Log.Fatal( "StandardVertex: ExtractOneComponentArray: Invalid requested component." );
			return null;
		}

		public static VertexElement[] MakeStructure( Components components, bool addPositionAnyway, out int vertexSize )
		{
			if( addPositionAnyway )
				components |= Components.Position;

			List<VertexElement> structure = new List<VertexElement>();
			vertexSize = 0;

			{
				Components c = Components.Position;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float3, VertexElementSemantic.Position ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.Normal;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float3, VertexElementSemantic.Normal ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.Tangent;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float4, VertexElementSemantic.Tangent ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.Color;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float4, VertexElementSemantic.Color0 ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.TexCoord0;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate0 ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.TexCoord1;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate1 ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.TexCoord2;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate2 ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.TexCoord3;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float2, VertexElementSemantic.TextureCoordinate3 ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.BlendIndices;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Integer4, VertexElementSemantic.BlendIndices ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			{
				Components c = Components.BlendWeights;
				if( ( components & c ) != 0 )
				{
					structure.Add( new VertexElement( 0, vertexSize, VertexElementType.Float4, VertexElementSemantic.BlendWeights ) );
					vertexSize += GetSizeInBytes( c );
				}
			}

			return structure.ToArray();
		}

		public static Components[] GetValuesArray()
		{
			return new Components[]
			{
				Components.Position,
				Components.Normal,
				Components.Tangent,
				Components.Color,
				Components.TexCoord0,
				Components.TexCoord1,
				Components.TexCoord2,
				Components.TexCoord3,
				Components.BlendIndices,
				Components.BlendWeights,
			};
		}

		public static Components[] Split( Components flags )
		{
			List<Components> l = new List<Components>();
			foreach( var v in GetValuesArray() )
			{
				if( ( v & flags ) != 0 )
					l.Add( v );
			}
			return l.ToArray();
		}

		public override bool Equals( object obj )
		{
			return ( obj is StandardVertex && this == (StandardVertex)obj );
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ Tangent.GetHashCode() ^ Color.GetHashCode() ^ TexCoord0.GetHashCode() ^ TexCoord1.GetHashCode() ^ TexCoord2.GetHashCode() ^ TexCoord3.GetHashCode() ^ BlendIndices.GetHashCode() ^ BlendWeights.GetHashCode();
		}

		public static bool operator ==( StandardVertex v1, StandardVertex v2 )
		{
			return ( v1.Position == v2.Position && v1.Normal == v2.Normal && v1.Tangent == v2.Tangent && v1.Color == v2.Color && v1.TexCoord0 == v2.TexCoord0 && v1.TexCoord1 == v2.TexCoord1 && v1.TexCoord2 == v2.TexCoord2 && v1.TexCoord3 == v2.TexCoord3 && v1.BlendIndices == v2.BlendIndices && v1.BlendWeights == v2.BlendWeights );
		}

		public static bool operator !=( StandardVertex v1, StandardVertex v2 )
		{
			return ( v1.Position != v2.Position || v1.Normal != v2.Normal || v1.Tangent != v2.Tangent || v1.Color != v2.Color || v1.TexCoord0 != v2.TexCoord0 || v1.TexCoord1 != v2.TexCoord1 || v1.TexCoord2 != v2.TexCoord2 || v1.TexCoord3 != v2.TexCoord3 || v1.BlendIndices != v2.BlendIndices || v1.BlendWeights != v2.BlendWeights );
		}

		public bool Equals( ref StandardVertex v, float epsilon )
		{
			if( !Position.Equals( ref v.Position, epsilon ) )
				return false;
			if( !Normal.Equals( ref v.Normal, epsilon ) )
				return false;
			if( !Tangent.Equals( ref v.Tangent, epsilon ) )
				return false;
			if( !Color.Equals( ref v.Color, epsilon ) )
				return false;
			if( !TexCoord0.Equals( ref v.TexCoord0, epsilon ) )
				return false;
			if( !TexCoord1.Equals( ref v.TexCoord1, epsilon ) )
				return false;
			if( !TexCoord2.Equals( ref v.TexCoord2, epsilon ) )
				return false;
			if( !TexCoord3.Equals( ref v.TexCoord3, epsilon ) )
				return false;
			if( BlendIndices != v.BlendIndices )
				return false;
			if( !BlendWeights.Equals( ref v.BlendWeights, epsilon ) )
				return false;
			return true;
		}

		public bool Equals( StandardVertex v, float epsilon )
		{
			return Equals( ref v, epsilon );
		}
	}
}
