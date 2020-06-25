// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
/*
namespace NeoAxis
{
	//ToDo ? Axis X/Y/Z (возможен выбор нескольких) ?
	//ToDo ? Vertex Group ?

	//<Temp comments>
	//Описание алкоритма: https://docs.blender.org/manual/en/latest/modeling/modifiers/deform/smooth.html
	//quote: Each new vertex position is simply moved towards the average position of all its neighbor vertices (topologically speaking, i.e. the vertices directly connected to it by an edge).
	//Результат сильно зависит от того насколько мелко триангулированы фейсы.
	//Например. Если плоскость разбита на 10 сегментов, и один фейс с Edges по контуру, то сглаживается. А если разбита на отдельные треугольники с Edge внутри плоскости, то плохо.
	//</Temp comments>


	[NewObjectDefaultName( "Smooth" )]
	public class Component_MeshModifier_Smooth : Component_MeshModifier
	{

		/// <summary>
		/// The smoothing factor.
		/// </summary>
		[DefaultValue( 0.4 )]
		[Range( 0, 1 )]
		public Reference<double> Factor
		{
			get { if( _factor.BeginGet() ) Factor = _factor.Get( this ); return _factor.value; }
			set
			{
				if( _factor.BeginSet( ref value ) )
				{
					try
					{
						FactorChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _factor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Factor"/> property value changes.</summary>
		public event Action<Component_MeshModifier_Smooth> FactorChanged;
		ReferenceField<double> _factor = 0.4;


		/// <summary>
		/// The number of smoothing iterations
		/// </summary>
		[DefaultValue( 1 )]
		//[Range(  )]
		public Reference<int> Repeat
		{
			get { if( _repeat.BeginGet() ) Repeat = _repeat.Get( this ); return _repeat.value; }
			set
			{
				if( _repeat.BeginSet( ref value ) )
				{
					try
					{
						RepeatChanged?.Invoke( this );
						ShouldRecompileMesh();
					}
					finally { _repeat.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Repeat"/> property value changes.</summary>
		public event Action<Component_MeshModifier_Smooth> RepeatChanged;
		ReferenceField<int> _repeat = 1;

		protected override void OnApplyToMeshData( Component_Mesh.CompiledData compiledData )
		{
			base.OnApplyToMeshData( compiledData );
		}

		Vector3F[][] Proc( Component_Mesh.ExtractedStructure structure, Vector3F[][] rawPositions, float factor )
		{
			var s = structure.Structure;
			Vector3F[] vertexPositions = new Vector3F[ s.Vertices.Length ];
			foreach( var face in s.Faces )
				foreach( var t in face.Triangles )
				{
					vertexPositions[ t.Vertex ] = rawPositions[ t.RawGeometry ][ t.RawVertex ]; 
				}

			var avgNeighbours = new (Vector3F avg, int count)[ vertexPositions.Length ];
			foreach( var edge in s.Edges )
			{
				avgNeighbours[ edge.Vertex1 ].avg += vertexPositions[ edge.Vertex2 ];
				avgNeighbours[ edge.Vertex1 ].count++;

				avgNeighbours[ edge.Vertex2 ].avg += vertexPositions[ edge.Vertex1 ];
				avgNeighbours[ edge.Vertex2 ].count++;
			}
			for( int i = 0; i < avgNeighbours.Length; i++ )
				if( 0 < avgNeighbours[ i ].count )
				avgNeighbours[ i ].avg /= avgNeighbours[ i ].count;

			var newVertexPositions = new Vector3F[ vertexPositions.Length ];
			for( int i = 0; i < vertexPositions.Length; i++ )
			{
				newVertexPositions[ i ] = 0 < avgNeighbours[ i ].count ?
					Vector3F.Lerp( vertexPositions[ i ], avgNeighbours[ i ].avg, factor ) :
					vertexPositions[ i ] ;
			}


			Vector3F[][] newRawPositions = new Vector3F[ structure.MeshGeometries.Length ][];
			for( int i = 0; i < rawPositions.Length; i++ )
				newRawPositions[ i ] = new Vector3F[ rawPositions[ i ].Length ];
			foreach( var face in s.Faces )
				foreach( var t in face.Triangles )
				{
					newRawPositions[ t.RawGeometry ][ t.RawVertex ] = newVertexPositions[ t.Vertex ]; //t.RawVertex или geom.Indices[ t.RawVertex ];
				}

			return newRawPositions;
		}


		//ToDo ?? Скопировано из Component_MeshGeometry.VerticesExtractChannel, и вынесены параметры.
		public static T[] VerticesExtractChannel<T>( VertexElement[] vertexStructure, byte[] vertices, VertexElementSemantic semantic ) where T : unmanaged
		{
			if( vertexStructure != null )
			{
				vertexStructure.GetInfo( out var vertexSize, out _ );

				if( vertices != null )
				{
					var vertexCount = vertices.Length / vertexSize;

					if( vertexStructure.GetElementBySemantic( semantic, out var element ) )
					{
						unsafe
						{
							if( VertexElement.GetSizeInBytes( element.Type ) == sizeof( T ) )
							{
								T[] result = new T[ vertexCount ];
								fixed ( byte* pVertices = vertices )
								{
									byte* src = pVertices + element.Offset;
									for( int n = 0; n < vertexCount; n++ )
									{
										result[ n ] = *(T*)src;
										src += vertexSize;
									}
								}
								return result;
							}
						}
					}
				}
			}

			return null;
		}


		protected override void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
		{
			base.OnBakeIntoMesh( document, undoMultiAction );

			if( Factor.Value < 0 || 1 < Factor.Value || Repeat.Value < 1 )
				return;  //???

			int repeatCount = Repeat;
			double factor = Factor;

			var mesh = (Component_Mesh)Parent;
			var structure = mesh.ExtractStructure();
			//ToDo ??? Если нет структуры?


			//ToDo  ??? В OnBakeIntoMesh rawPositions можно получить из mesh.GetComponents<Component_MeshGeometry>(), а в OnApplyToMeshData Procedural не раскрыт.
			var geoms = structure.MeshGeometries;
			Vector3F[][] rawPositions = new Vector3F[ geoms.Length ][];
			for( int i = 0; i < geoms.Length; i++ )
				rawPositions[ i ] = VerticesExtractChannel<Vector3F>( geoms[ i ].VertexStructure, geoms[ i ].Vertices, VertexElementSemantic.Position );


			Vector3F[][] newRawPositions = rawPositions;
			for( int i = 0; i < repeatCount; i++ )
			{
				newRawPositions = Proc( structure, newRawPositions, (float)factor );
			}
			

			//-----------------------------------------------------------------------------------------------------------

			var geometries = mesh.GetComponents<Component_MeshGeometry>();

			for( int geomIndex =0; geomIndex<geometries.Length; geomIndex++ )
			{
				var geometry = geometries[ geomIndex ];
				var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
				if( positions != null )
				{
					var vertexStructure = geometry.VertexStructure.Value;
					vertexStructure.GetInfo( out var vertexSize, out _ );

					var oldValue = geometry.Vertices;
					var vertices = geometry.Vertices.Value;
					var vertexCount = vertices.Length / vertexSize;

					var newPositions = newRawPositions[geomIndex];

					var newVertices = (byte[])vertices.Clone();
					if( geometry.VerticesWriteChannel( VertexElementSemantic.Position, newPositions, newVertices ) )
					{
						//update property
						geometry.Vertices = newVertices;

						//undo
						if( undoMultiAction != null )
						{
							var property = (Metadata.Property)geometry.MetadataGetMemberBySignature( "property:Vertices" );
							var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( geometry, property, oldValue ) );
							undoMultiAction.AddAction( undoAction );
						}
					}
				}
			}
		}
	}
}

*/