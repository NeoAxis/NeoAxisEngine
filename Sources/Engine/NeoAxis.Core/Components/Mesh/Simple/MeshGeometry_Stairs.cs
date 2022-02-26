// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Mesh geometry in the form of a stairs.
	/// </summary>
	public class MeshGeometry_Stairs : MeshGeometry_Procedural
	{
		/// <summary>
		/// The axis of the stairs.
		/// </summary>
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
				if( _axis.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Stairs> AxisChanged;
		ReferenceField<int> _axis = 0;

		/// <summary>
		/// The width of the stairs.
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
				if( _width.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Stairs> WidthChanged;
		ReferenceField<double> _width = 1.0;

		/// <summary>
		/// The height of the stairs.
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
				if( _height.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Stairs> HeightChanged;
		ReferenceField<double> _height = 1.0;

		/// <summary>
		/// The depth of the stairs.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Depth
		{
			get { if( _depth.BeginGet() ) Depth = _depth.Get( this ); return _depth.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _depth.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Stairs> DepthChanged;
		ReferenceField<double> _depth = 1.0;

		/// <summary>
		/// The quantity of stairs.
		/// </summary>
		[DefaultValue( 10 )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> Steps
		{
			get { if( _steps.BeginGet() ) Steps = _steps.Get( this ); return _steps.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _steps.BeginSet( ref value ) )
				{
					try
					{
						StepsChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _steps.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Steps"/> property value changes.</summary>
		public event Action<MeshGeometry_Stairs> StepsChanged;
		ReferenceField<int> _steps = 10;

		/// <summary>
		/// Degree of the stairs twist.
		/// </summary>
		[DefaultValue( "0" )]
		[Range( -360, 360 )]
		public Reference<Degree> Curvature
		{
			get { if( _curvature.BeginGet() ) Curvature = _curvature.Get( this ); return _curvature.value; }
			set
			{
				if( value < new Degree( -360 ) )
					value = new Reference<Degree>( -360, value.GetByReference );
				if( value > new Degree( 360 ) )
					value = new Reference<Degree>( 360, value.GetByReference );
				if( _curvature.BeginSet( ref value ) )
				{
					try
					{
						CurvatureChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _curvature.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Curvature"/> property value changes.</summary>
		public event Action<MeshGeometry_Stairs> CurvatureChanged;
		ReferenceField<Degree> _curvature = new Degree( 0 );

		/// <summary>
		/// The radius of the stairs
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				if( _radius.BeginSet( ref value ) )
				{
					try
					{
						RadiusChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<MeshGeometry_Stairs> RadiusChanged;
		ReferenceField<double> _radius = 1.0;

		/// <summary>
		/// Whether to create faces on the sides of the stairs.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Sides
		{
			get { if( _sides.BeginGet() ) Sides = _sides.Get( this ); return _sides.value; }
			set
			{
				if( _sides.BeginSet( ref value ) )
				{
					try
					{
						SidesChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _sides.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Sides"/> property value changes.</summary>
		public event Action<MeshGeometry_Stairs> SidesChanged;
		ReferenceField<bool> _sides = true;

		/// <summary>
		/// Whether the stairs are flipped.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> InsideOut
		{
			get { if( _insideOut.BeginGet() ) InsideOut = _insideOut.Get( this ); return _insideOut.value; }
			set
			{
				if( _insideOut.BeginSet( ref value ) )
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
		public event Action<MeshGeometry_Stairs> InsideOutChanged;
		ReferenceField<bool> _insideOut = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Radius ):
					if( Curvature.Value == 0 )
						skip = true;
					break;
				}
			}
		}

		public override void GetProceduralGeneratedData( ref VertexElement[] vertexStructure, ref byte[] vertices, ref int[] indices, ref Material material, ref byte[] billboardData, ref Mesh.StructureClass structure )
		{
			vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );
			unsafe
			{
				if( vertexSize != sizeof( StandardVertex.StaticOneTexCoord ) )
					Log.Fatal( "vertexSize != sizeof( StandardVertexF )" );
			}

			SimpleMeshGenerator.GenerateStairs( Axis, Width, Height, Depth, Steps, Curvature, Radius, Sides, InsideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out indices, out var faces );

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
