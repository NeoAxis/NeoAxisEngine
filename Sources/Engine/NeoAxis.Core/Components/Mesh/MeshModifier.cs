// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Declares modifier of the mesh. Using mesh modifiers, you can change the output data provided by the mesh; they do not change the original mesh data.
	/// </summary>
#if !DEPLOY
	[SettingsCell( typeof( MeshModifierSettingsCell ) )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Mesh Modifier", -10000 )]
#endif
	public class MeshModifier : Component
	{
		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var mesh = ParentMesh;//FindParent<Mesh>();
			if( mesh != null )
			{
				//can be wrong order for multiple modifiers
				//if( EnabledInHierarchy )
				//	mesh.MeshCompilePostProcessEvent += Mesh_MeshCompilePostProcessEvent;
				//else
				//	mesh.MeshCompilePostProcessEvent -= Mesh_MeshCompilePostProcessEvent;

				mesh.ShouldRecompile = true;
			}
		}

		public delegate void ApplyToMeshDataEventDelegate( MeshModifier sender, Mesh.CompiledData compiledData );
		public event ApplyToMeshDataEventDelegate ApplyToMeshDataEvent;

		protected virtual void OnApplyToMeshData( Mesh.CompiledData compiledData )
		{
		}

		public void ApplyToMeshData( Mesh.CompiledData compiledData )
		{
			OnApplyToMeshData( compiledData );
			ApplyToMeshDataEvent?.Invoke( this, compiledData );
		}

		//void Mesh_MeshCompilePostProcessEvent( Mesh sender, Mesh.CompiledData compiledData )
		//{
		//	OnApplyToData( compiledData );
		//	ApplyToDataEvent?.Invoke( this, compiledData );
		//}

		[Browsable( false )]
		public Mesh ParentMesh
		{
			get { return Parent as Mesh; }
		}

		public void ShouldRecompileMesh()
		{
			if( EnabledInHierarchy )
			{
				var mesh = ParentMesh;//FindParent<Mesh>();
				if( mesh != null )
					mesh.ShouldRecompile = true;
			}
		}

		protected virtual void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
		{
		}

		public delegate void BakeIntoMeshEventDelegate( MeshModifier sender, DocumentInstance document, UndoMultiAction undoMultiAction );
		public event BakeIntoMeshEventDelegate BakeIntoMeshEvent;

		static void ConvertProceduralMeshGeometries( DocumentInstance document, Mesh mesh, UndoMultiAction undoMultiAction, ref bool needUndoForNextActions )
		{
			//needUndoForNextActions = true;

			var meshGeometries = mesh.GetComponents<MeshGeometry>();
			if( meshGeometries.Any( g => g is MeshGeometry_Procedural ) )
			{
				//!!!!?
				bool hasOrdinary = meshGeometries.Any( g => !( g is MeshGeometry_Procedural ) );
				if( !hasOrdinary )
					needUndoForNextActions = false; //??? Если были и обычные geometry и procedural? Как правильно needUndoForNextActions? Пока так: undo не нужен только если все procedural

				//!!!!right? !needUndoForNextActions
				if( undoMultiAction != null && !needUndoForNextActions )
				{
					//add structure update to undo
					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
					undoMultiAction.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure?.Clone() ) ) );
				}

				for( int i = 0; i < meshGeometries.Length; i++ )
				{
					var meshGeometry = meshGeometries[ i ];

					//convert to usual MeshGeometry
					if( meshGeometry is MeshGeometry_Procedural meshGeometryProcedural )
					{
						VertexElement[] vertexStructure = null;
						byte[] vertices = null;
						int[] indices = null;
						Material material = null;
						byte[] voxelData = null;
						byte[] clusterData = null;
						Mesh.StructureClass structure = null;
						meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref voxelData, ref clusterData, ref structure );

						var insertIndex = meshGeometryProcedural.Parent.Components.IndexOf( meshGeometryProcedural );

						var meshGeometryNew = mesh.CreateComponent<MeshGeometry>( insertIndex );
						meshGeometryNew.Name = meshGeometry.Name;
						meshGeometryNew.VertexStructure = vertexStructure;
						meshGeometryNew.Vertices = vertices;
						meshGeometryNew.Indices = indices;

						//!!!!voxels?

						meshGeometryNew.Material = meshGeometryProcedural.Material;

						//concut structures. If the geometry is procedural it is not in a structure yet.
						mesh.Structure = Mesh.StructureClass.Concat( mesh.Structure, structure, i );

						//delete old mesh geometry
						if( undoMultiAction != null )
							undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, new Component[] { meshGeometry }, create: false ) );
						else
							meshGeometry.Dispose();

						//add created geometry to undo
						if( undoMultiAction != null )
							undoMultiAction.AddAction( new UndoActionComponentCreateDelete( document, new Component[] { meshGeometryNew }, create: true ) );
					}
				}
			}
		}

		public void BakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
		{
			var mesh = Parent as Mesh;
			if( mesh != null )
			{
				bool meshWasEnabled = mesh.Enabled;
				try
				{
					//disable mesh
					mesh.Enabled = false;

					//convert mesh geometries
					bool needUndoForNextActions = true;
					ConvertProceduralMeshGeometries( document, mesh, needUndoForNextActions ? undoMultiAction : null, ref needUndoForNextActions );

					var undoMultiAction2 = needUndoForNextActions ? undoMultiAction : null;
					OnBakeIntoMesh( document, undoMultiAction2 );
					BakeIntoMeshEvent?.Invoke( this, document, undoMultiAction2 );

				}
				finally
				{
					//enable mesh
					mesh.Enabled = meshWasEnabled;
				}
			}
		}
	}
}
