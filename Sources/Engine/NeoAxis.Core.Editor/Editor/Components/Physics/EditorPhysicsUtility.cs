#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public static class EditorPhysicsUtility
	{
		public static void AddCollision( DocumentWindow documentWindow, string collisionName )
		{
			const string bodyName = "Collision Definition";

			var mesh = documentWindow.ObjectOfWindow as Mesh;

			var body = mesh.CreateComponent<RigidBody>( enabled: false );
			body.Name = bodyName;
			//collision.CanCreate = false;

			body.MotionType = PhysicsMotionType.Static;

			string error = null;

			//!!!!plugin support to add new or override current implementations

			switch( collisionName )
			{
			case "Box":
				{
					var shape = body.CreateComponent<CollisionShape_Box>();
					shape.Name = "Shape";
					var bounds = mesh.Result.SpaceBounds.BoundingBox;
					shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
					shape.Dimensions = bounds.GetSize();
				}
				break;

			case "Sphere":
				{
					var shape = body.CreateComponent<CollisionShape_Sphere>();
					shape.Name = "Shape";
					var sphere = mesh.Result.SpaceBounds.BoundingSphere;
					shape.LocalTransform = new Transform( sphere.Center, Quaternion.Identity );
					shape.Radius = sphere.Radius;
				}
				break;

			case "Capsule":
				{
					var shape = body.CreateComponent<CollisionShape_Capsule>();
					shape.Name = "Shape";
					var bounds = mesh.Result.SpaceBounds.BoundingBox;
					shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
					shape.Radius = Math.Max( bounds.GetSize().X, bounds.GetSize().Y ) / 2;
					shape.Height = Math.Max( bounds.GetSize().Z - shape.Radius * 2, 0 );
				}
				break;

			case "Cylinder":
				{
					var shape = body.CreateComponent<CollisionShape_Cylinder>();
					shape.Name = "Shape";
					var bounds = mesh.Result.SpaceBounds.BoundingBox;
					shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
					shape.Radius = Math.Max( bounds.GetSize().X, bounds.GetSize().Y ) / 2;
					shape.Height = bounds.GetSize().Z;
				}
				break;

			case "Convex":
				{
					var shape = body.CreateComponent<CollisionShape_Mesh>();
					shape.Name = "Shape";
					shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;
					shape.Mesh = ReferenceUtility.MakeRootReference( mesh );
					//shape.Mesh = ReferenceUtility.MakeThisReference( shape, mesh );
				}
				break;

			case "Convex Decomposition":
				{
					var settings = new ConvexDecomposition.Settings();

					var form = new SpecifyParametersForm( "Convex Decomposition", settings );
					form.CheckHandler = delegate ( ref string error2 )
					{
						//!!!!
						return true;
					};
					if( form.ShowDialog() != DialogResult.OK )
					{
						mesh.RemoveComponent( body, false );
						return;
					}

					var clusters = ConvexDecomposition.Decompose( mesh.Result.ExtractedVerticesPositions, mesh.Result.ExtractedIndices, settings );

					if( clusters == null )
					{
						mesh.RemoveComponent( body, false );
						Log.Warning( "Unable to decompose." );
						return;
					}

					int counter = 1;
					foreach( var cluster in clusters )
					{
						var shape = body.CreateComponent<CollisionShape_Mesh>();
						shape.Name = "Shape " + counter.ToString();
						shape.Vertices = cluster.Vertices;
						shape.Indices = cluster.Indices;
						shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;

						counter++;
					}
				}
				break;

			case "Mesh Worst LOD":
				{
					var selectMesh = mesh;

					var lods = mesh.GetComponents<MeshLevelOfDetail>();
					for( int n = lods.Length - 1; n >= 0; n-- )
					{
						var lod = lods[ n ];
						var meshLOD = lod.GetComponent<Mesh>();
						if( meshLOD != null )
						{
							var geometry = meshLOD.GetComponent<MeshGeometry>();
							if( geometry != null && !geometry.VoxelData.ReferenceOrValueSpecified )
							{
								selectMesh = meshLOD;
								break;
							}
						}
					}

					var shape = body.CreateComponent<CollisionShape_Mesh>();
					shape.Name = "Shape";
					shape.Mesh = ReferenceUtility.MakeRootReference( selectMesh );
				}
				break;

			case "Mesh Best LOD":
				{
					var shape = body.CreateComponent<CollisionShape_Mesh>();
					shape.Name = "Shape";
					shape.Mesh = ReferenceUtility.MakeRootReference( mesh );
					//shape.Mesh = ReferenceUtility.MakeThisReference( shape, mesh );
				}
				break;

			default:
				error = "No implementation.";
				break;
			}

			if( !string.IsNullOrEmpty( error ) )
			{
				mesh.RemoveComponent( body, false );
				Log.Warning( error );
				return;
			}

			body.Enabled = true;

			var undoActions = new List<UndoSystem.Action>();
			undoActions.Add( new UndoActionComponentCreateDelete( documentWindow.Document2, new Component[] { body }, true ) );

			//enable EditorDisplayCollision
			if( !mesh.EditorDisplayCollision )
			{
				mesh.EditorDisplayCollision = true;

				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:EditorDisplayCollision" );
				var undoItem = new UndoActionPropertiesChange.Item( mesh, property, false, new object[ 0 ] );
				undoActions.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );
			}

			documentWindow.Document2.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
			documentWindow.Document2.Modified = true;
		}
	}
}

#endif