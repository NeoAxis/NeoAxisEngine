// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Declares modifier of the mesh. Using mesh modifiers, you can change the output data provided by the mesh; they do not change the original mesh data.
	/// </summary>
	[EditorSettingsCell( typeof( Component_MeshModifier_SettingsCell ) )]
	[AddToResourcesWindow( @"Base\Scene common\Mesh modifiers\Mesh Modifier", -10000 )]
	public class Component_MeshModifier : Component
	{
		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var mesh = FindParent<Component_Mesh>();
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

		public delegate void ApplyToMeshDataEventDelegate( Component_MeshModifier sender, Component_Mesh.CompiledData compiledData );
		public event ApplyToMeshDataEventDelegate ApplyToMeshDataEvent;

		protected virtual void OnApplyToMeshData( Component_Mesh.CompiledData compiledData )
		{
		}

		public void ApplyToMeshData( Component_Mesh.CompiledData compiledData )
		{
			OnApplyToMeshData( compiledData );
			ApplyToMeshDataEvent?.Invoke( this, compiledData );
		}

		//void Mesh_MeshCompilePostProcessEvent( Component_Mesh sender, Component_Mesh.CompiledData compiledData )
		//{
		//	OnApplyToData( compiledData );
		//	ApplyToDataEvent?.Invoke( this, compiledData );
		//}

		public void ShouldRecompileMesh()
		{
			if( EnabledInHierarchy )
			{
				var mesh = FindParent<Component_Mesh>();
				if( mesh != null )
					mesh.ShouldRecompile = true;
			}
		}

		protected virtual void OnBakeIntoMesh( DocumentInstance document, UndoMultiAction undoMultiAction )
		{
		}

		public delegate void BakeIntoMeshEventDelegate( Component_MeshModifier sender, DocumentInstance document, UndoMultiAction undoMultiAction );
		public event BakeIntoMeshEventDelegate BakeIntoMeshEvent;

		static void ConvertProceduralMeshGeometries( DocumentInstance document, Component_Mesh mesh, UndoMultiAction undoMultiAction, ref bool needUndoForNextActions )
		{
			//needUndoForNextActions = true;

			var meshGeometries = mesh.GetComponents<Component_MeshGeometry>();
			if( meshGeometries.Any( g => g is Component_MeshGeometry_Procedural ) )
			{
				//!!!!?
				bool hasOrdinary = meshGeometries.Any( g => !( g is Component_MeshGeometry_Procedural ) );
				if( !hasOrdinary )
					needUndoForNextActions = false; //??? Если были и обычные geometry и procedural? Как правильно needUndoForNextActions? Пока так: undo не нужен только если все procedural

				//!!!!right? !needUndoForNextActions
				if( undoMultiAction != null && !needUndoForNextActions )
				{
					//add structure update to undo
					var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
					undoMultiAction.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure?.Clone() ) ) );
				}

				for( int i = 0; i < meshGeometries.Length; i++ )
				{
					var meshGeometry = meshGeometries[ i ];

					//convert to usual Component_MeshGeometry
					if( meshGeometry is Component_MeshGeometry_Procedural meshGeometryProcedural )
					{
						VertexElement[] vertexStructure = null;
						byte[] vertices = null;
						int[] indices = null;
						Component_Material material = null;
						Component_Mesh.StructureClass structure = null;
						meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref structure );

						var insertIndex = meshGeometryProcedural.Parent.Components.IndexOf( meshGeometryProcedural );

						var meshGeometryNew = mesh.CreateComponent<Component_MeshGeometry>( insertIndex );
						meshGeometryNew.Name = meshGeometry.Name;
						meshGeometryNew.VertexStructure = vertexStructure;
						meshGeometryNew.Vertices = vertices;
						meshGeometryNew.Indices = indices;

						meshGeometryNew.Material = meshGeometryProcedural.Material;

						//concut structures. If the geometry is procedural it is not in a structure yet.
						mesh.Structure = Component_Mesh.StructureClass.Concat( mesh.Structure, structure, i );

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
			var mesh = Parent as Component_Mesh;
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
