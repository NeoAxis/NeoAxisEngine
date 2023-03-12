// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A component for making creatures and humans.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Creature\Creature Maker", 11001 )]
#if !DEPLOY
	//[EditorControl( typeof( CreatureMakerEditor ) )]
	//[Preview( typeof( CreatureMakerPreview ) )]
	//[PreviewImage( typeof( CreatureMakerPreviewImage ) )]
	[SettingsCell( typeof( CreatureMakerSettingsCell ) )]
#endif
	public class CreatureMaker : Component
	{
		bool needUpdate;

		/// <summary>
		/// The height of the creature.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.8 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( ref value ) ) { try { HeightChanged?.Invoke( this ); NeedUpdate(); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<CreatureMaker> HeightChanged;
		ReferenceField<double> _height = 1.8;

		/// <summary>
		/// The color multiplier.
		/// </summary>
		[DefaultValue( "0.1 0.7 0.1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); NeedUpdate(); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<CreatureMaker> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 0.1, 0.7, 0.1 );

		/// <summary>
		/// The reference to output mesh.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( null )]
		public Reference<Mesh> WriteToMesh
		{
			get { if( _writeToMesh.BeginGet() ) WriteToMesh = _writeToMesh.Get( this ); return _writeToMesh.value; }
			set { if( _writeToMesh.BeginSet( ref value ) ) { try { WriteToMeshChanged?.Invoke( this ); } finally { _writeToMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WriteToMesh"/> property value changes.</summary>
		public event Action<CreatureMaker> WriteToMeshChanged;
		ReferenceField<Mesh> _writeToMesh = null;

		//!!!!lods

		///////////////////////////////////////////////

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( Components.Count == 0 )
			{
				var mesh = CreateComponent<Mesh>();
				mesh.Name = "Mesh";


				//!!!!need serialize data of mesh? no sense because it will regenerated


				WriteToMesh = ReferenceUtility.MakeThisReference( this, mesh );
			}
		}

		void UpdateMeshIfNeed()
		{
			if( needUpdate )
			{
				UpdateMesh( false, out _ );
				needUpdate = false;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				needUpdate = true;
				UpdateMeshIfNeed();
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateMeshIfNeed();
		}

		public void NeedUpdate()
		{
			needUpdate = true;
		}

		public delegate void UpdateMeshEventDelegate( CreatureMaker sender, Mesh mesh, ref bool handled );
		public event UpdateMeshEventDelegate UpdateMeshEvent;

		protected virtual void OnUpdateMesh( Mesh mesh, ref bool handled )
		{
		}

		public bool UpdateMesh( bool clearOnly, out string error )
		{
			error = "";

			var mesh = WriteToMesh.Value;
			if( mesh == null )
			{
				error = "WriteToMesh is not configured.";
				return false;
			}

			var restoreEnabled = mesh.Enabled;
			if( restoreEnabled )
				mesh.Enabled = false;

			//!!!!maybe also reset some mesh properties
			//clear
			//delete everything except CreatureMaker for a case when it inside the mesh
			foreach( var c in mesh.GetComponents( true ) )
			{
				if( !( c is CreatureMaker ) )
					c.RemoveFromParent( false );
			}

			//fill
			if( !clearOnly )
			{
				var handled = false;
				OnUpdateMesh( mesh, ref handled );
				if( !handled )
					UpdateMeshEvent?.Invoke( this, mesh, ref handled );
				if( !handled )
					UpdateMeshDefaultImplementation( mesh );
			}

			if( restoreEnabled )
				mesh.Enabled = true;

			return true;
		}

		void UpdateMeshDefaultImplementation( Mesh mesh )
		{
			var material = mesh.CreateComponent<Material>();
			//!!!!make shader graph?
			//material.NewObjectSetDefaultConfiguration( false );

			material.Name = "Material";
			material.BaseColor = Color;

			var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

			var boxes = new Bounds[ 8 ];
			boxes[ 0 ] = new Bounds( -0.06, -0.13, 0.4, 0.06, 0.13, 0.8 );
			boxes[ 1 ] = new Bounds( -0.08, -0.08, 0.8, 0.08, 0.08, 1.0 );
			boxes[ 2 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.04, 0.04 - 0.05, 0.4 );
			boxes[ 3 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.04, 0.04 + 0.05, 0.4 );
			boxes[ 4 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.10, 0.04 - 0.05, 0.05 );
			boxes[ 5 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.10, 0.04 + 0.05, 0.05 );
			boxes[ 6 ] = new Bounds( -0.04, -0.04 - 0.17, 0.40, 0.04, 0.04 - 0.17, 0.75 );
			boxes[ 7 ] = new Bounds( -0.04, -0.04 + 0.17, 0.40, 0.04, 0.04 + 0.17, 0.75 );

			//scale depending Height
			for( int n = 0; n < boxes.Length; n++ )
			{
				ref var b = ref boxes[ n ];
				b.Minimum *= Height.Value;
				b.Maximum *= Height.Value;
			}

			var totalVertices = 24 * boxes.Length;
			var totalIndices = 36 * boxes.Length;

			var vertices = new byte[ vertexSize * totalVertices ];
			var indices = new int[ totalIndices ];

			unsafe
			{
				fixed( byte* pVertices = vertices )
				{
					StandardVertex.StaticOneTexCoord* pVertices2 = (StandardVertex.StaticOneTexCoord*)pVertices;
					var currentVertex = 0;
					var currentIndex = 0;

					for( int nBox = 0; nBox < boxes.Length; nBox++ )
					{
						var b = boxes[ nBox ];

						SimpleMeshGenerator.GenerateBox( b.GetSize(), false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices2, out _ );

						var center = b.GetCenter().ToVector3F();

						var indexOffset = currentVertex;

						for( int n = 0; n < positions.Length; n++ )
						{
							var pVertex = pVertices2 + currentVertex;

							pVertex->Position = positions[ n ] + center;
							pVertex->Normal = normals[ n ];
							pVertex->Tangent = tangents[ n ];
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = texCoords[ n ];

							currentVertex++;
						}

						for( int n = 0; n < indices2.Length; n++ )
							indices[ currentIndex++ ] = indices2[ n ] + indexOffset;
					}
				}
			}


			//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

			//var size = new Vector3( Height / 6, Height / 3, Height );
			//SimpleMeshGenerator.GenerateBox( size, false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices, out _ );

			//var vertices = new byte[ vertexSize * positions.Length ];
			//unsafe
			//{
			//	fixed( byte* pVertices = vertices )
			//	{
			//		StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

			//		for( int n = 0; n < positions.Length; n++ )
			//		{
			//			pVertex->Position = positions[ n ] + new Vector3F( 0, 0, (float)Height.Value / 2 );
			//			pVertex->Normal = normals[ n ];
			//			pVertex->Tangent = tangents[ n ];
			//			pVertex->Color = new ColorValue( 1, 1, 1, 1 );
			//			pVertex->TexCoord0 = texCoords[ n ];

			//			pVertex++;
			//		}
			//	}
			//}

			var geometry = mesh.CreateComponent<MeshGeometry>();
			geometry.Name = "Mesh Geometry";
			geometry.VertexStructure = vertexStructure;
			geometry.Vertices = vertices;
			geometry.Indices = indices;
			geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );
		}
	}
}
