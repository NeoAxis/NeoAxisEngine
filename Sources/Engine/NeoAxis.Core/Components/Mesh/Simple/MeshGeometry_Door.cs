// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Mesh geometry in the form of a door.
	/// </summary>
	public class MeshGeometry_Door : MeshGeometry_Procedural
	{
		//!!!!дескрипшены

		/// <summary>
		/// The axis of the door.
		/// </summary>
		[Serialize]
		[DefaultValue( 0 )]
		[Range( 0, 2 )]
		public Reference<int> Axis
		{
			get { if( _axis.BeginGet() ) Axis = _axis.Get( this ); return _axis.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( value > 2 )
					value = new Reference<int>( 2, value.GetByReference );
				if( _axis.BeginSet( this, ref value ) )
				{
					try
					{
						AxisChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _axis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Axis"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> AxisChanged;
		ReferenceField<int> _axis = 0;

		/// <summary>
		/// The width of the door.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Width
		{
			get { if( _width.BeginGet() ) Width = _width.Get( this ); return _width.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _width.BeginSet( this, ref value ) )
				{
					try
					{
						WidthChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _width.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Width"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> WidthChanged;
		ReferenceField<double> _width = 1.0;

		/// <summary>
		/// The height of the door.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _height.BeginSet( this, ref value ) )
				{
					try
					{
						HeightChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _height.EndSet(); }
				}
			}
		}

		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> HeightChanged;
		ReferenceField<double> _height = 1.0;

		/// <summary>
		/// The depth of the door.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Depth
		{
			get { if( _depth.BeginGet() ) Depth = _depth.Get( this ); return _depth.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _depth.BeginSet( this, ref value ) )
				{
					try
					{
						DepthChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _depth.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Depth"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> DepthChanged;
		ReferenceField<double> _depth = 0.2;

		/// <summary>
		/// The inside width of the door.
		/// </summary>
		[DefaultValue( 0.8 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> DoorWidth
		{
			get { if( _doorWidth.BeginGet() ) DoorWidth = _doorWidth.Get( this ); return _doorWidth.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _doorWidth.BeginSet( this, ref value ) )
				{
					try
					{
						DoorWidthChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _doorWidth.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DoorWidth"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> DoorWidthChanged;
		ReferenceField<double> _doorWidth = 0.8;

		/// <summary>
		/// The inside height of the door.
		/// </summary>
		[DefaultValue( 0.8 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> DoorHeight
		{
			get { if( _doorHeight.BeginGet() ) DoorHeight = _doorHeight.Get( this ); return _doorHeight.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _doorHeight.BeginSet( this, ref value ) )
				{
					try
					{
						DoorHeightChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _doorHeight.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DoorHeight"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> DoorHeightChanged;
		ReferenceField<double> _doorHeight = 0.8;

		/// <summary>
		/// Whether the door is flipped.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> InsideOut
		{
			get { if( _insideOut.BeginGet() ) InsideOut = _insideOut.Get( this ); return _insideOut.value; }
			set
			{
				if( _insideOut.BeginSet( this, ref value ) )
				{
					try
					{
						InsideOutChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _insideOut.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="InsideOut"/> property value changes.</summary>
		public event Action<MeshGeometry_Door> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

		/////////////////////////////////////////

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] voxelData, ref byte[] clusterData, ref Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			SimpleMeshGenerator.GenerateDoor( Axis, Width, Height, Depth, DoorWidth, DoorHeight, InsideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

			if( faces != null )
				structure = SimpleMeshGenerator.CreateMeshStructure( faces );

			vertices = new byte[ vertexSize * positions.Length ];
			unsafe
			{
				fixed ( byte* pVertices = vertices )
				{
					StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

					for( int n = 0; n < positions.Length; n++ )
					{
						pVertex->Position = positions[ n ];
						pVertex->Normal = normals[ n ];
						pVertex->Tangent = tangents[ n ];
						pVertex->Color = new ColorValue( 1, 1, 1, 1 );
						pVertex->TexCoord0 = texCoords[ n ];

						pVertex++;
					}
				}
			}
		}
	}
}
