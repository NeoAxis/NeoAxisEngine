#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using Internal.tainicom.Aether.Physics2D.Dynamics;

namespace NeoAxis.Editor
{
	public class SceneSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonInfo;

		//

		protected override void OnInit()
		{
			buttonInfo = ProcedureForm.CreateButton( EditorLocalization2.Translate( "General", "Info" ) );
			buttonInfo.Click += ButtonInfo_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonInfo } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		private void ButtonInfo_Click( ProcedureUI.Button sender )
		{
			var scenes = GetObjects<Scene>();
			if( scenes.Length != 1 )
				return;
			var scene = scenes[ 0 ];

			List<string> lines = new List<string>();

			var components = 0;
			var meshesInSpace = 0;
			var rigidBodies = 0;
			var groupOfObjectsItems = 0L;
			scene.GetComponents( false, true, false, false, delegate ( Component c )
			{
				components++;
				if( c is MeshInSpace )
					meshesInSpace++;
				else if( c is RigidBody )
					rigidBodies++;
				else if( c is GroupOfObjects groupOfObjects )
					groupOfObjectsItems += groupOfObjects.ObjectsGetCount();
			} );
			lines.Add( "Components: " + components.ToString( "N0" ) );
			lines.Add( "MeshInSpace components: " + meshesInSpace.ToString( "N0" ) );
			lines.Add( "GroupOfObjects items: " + groupOfObjectsItems.ToString( "N0" ) );
			lines.Add( "RigidBody components: " + rigidBodies.ToString( "N0" ) );

			if( scene.PhysicsWorld != null )
			{
				scene.PhysicsWorld.GetStatistics( out var shapes, out var allBodies, out var kinematicDynamicBodies, out var activeBodies );
				if( allBodies != 0 )
				{
					//lines.Add( "" );
					lines.Add( "Physics shapes: " + shapes.ToString( "N0" ) );
					lines.Add( "Physics bodies: " + allBodies.ToString( "N0" ) );
					lines.Add( "Physics kinematic and dynamic bodies: " + kinematicDynamicBodies.ToString( "N0" ) );
				}
			}

			var physicsWorld2D = scene.Physics2DGetWorld( false );
			if( physicsWorld2D != null )
			{
				var rigidBodyCount = 0;
				var dynamicRigidBodyCount = 0;

				foreach( var worldBody in physicsWorld2D.BodyList )
				{
					rigidBodyCount++;
					if( worldBody.BodyType == BodyType.Dynamic )
						dynamicRigidBodyCount++;
				}

				if( rigidBodyCount != 0 )
				{
					//lines.Add( "" );
					lines.Add( "Physics 2D rigid bodies: " + rigidBodyCount.ToString( "N0" ) );
					lines.Add( "Physics 2D dynamic rigid bodies: " + dynamicRigidBodyCount.ToString( "N0" ) );
				}
			}

			{
				if( scene.GetOctreeStatistics( Scene.SceneObjectFlags.Logic, out var objectCount, out var octreeBounds, out var octreeNodeCount, out var timeSinceLastFullRebuild ) )
				{
					//lines.Add( "" );
					lines.Add( "Octree logic objects: " + objectCount.ToString( "N0" ) );
					lines.Add( "Octree logic nodes: " + octreeNodeCount.ToString( "N0" ) );
					var size = octreeBounds.GetSize();
					lines.Add( "Octree logic bounds size: " + size.X.ToString( "N1" ) + " " + size.Y.ToString( "N1" ) + " " + size.Z.ToString( "N1" ) );
					lines.Add( "Octree logic since last full rebuild: " + timeSinceLastFullRebuild.ToString( "F1" ) );
				}
			}

			{
				if( scene.GetOctreeStatistics( Scene.SceneObjectFlags.Visual, out var objectCount, out var octreeBounds, out var octreeNodeCount, out var timeSinceLastFullRebuild ) )
				{
					//lines.Add( "" );
					lines.Add( "Octree visual objects: " + objectCount.ToString( "N0" ) );
					lines.Add( "Octree visual nodes: " + octreeNodeCount.ToString( "N0" ) );
					var size = octreeBounds.GetSize();
					lines.Add( "Octree visual bounds size: " + size.X.ToString( "N1" ) + " " + size.Y.ToString( "N1" ) + " " + size.Z.ToString( "N1" ) );
					lines.Add( "Octree visual since last full rebuild: " + timeSinceLastFullRebuild.ToString( "F1" ) );
				}
			}

			{
				if( scene.GetOctreeStatistics( Scene.SceneObjectFlags.Occluder, out var objectCount, out var octreeBounds, out var octreeNodeCount, out var timeSinceLastFullRebuild ) )
				{
					//lines.Add( "" );
					lines.Add( "Octree occluder objects: " + objectCount.ToString( "N0" ) );
					lines.Add( "Octree occluder nodes: " + octreeNodeCount.ToString( "N0" ) );
					var size = octreeBounds.GetSize();
					lines.Add( "Octree occluder bounds size: " + size.X.ToString( "N1" ) + " " + size.Y.ToString( "N1" ) + " " + size.Z.ToString( "N1" ) );
					lines.Add( "Octree occluder since last full rebuild: " + timeSinceLastFullRebuild.ToString( "F1" ) );
				}
			}


			//if( scene.GetOctreeStatistics( out var objectCount, out var octreeBounds, out var octreeNodeCount, out var timeSinceLastFullRebuild ) )
			//{
			//	//lines.Add( "" );
			//	lines.Add( "Octree objects: " + objectCount.ToString( "N0" ) );
			//	lines.Add( "Octree nodes: " + octreeNodeCount.ToString( "N0" ) );
			//	var size = octreeBounds.GetSize();
			//	lines.Add( "Octree bounds size: " + size.X.ToString( "N1" ) + " " + size.Y.ToString( "N1" ) + " " + size.Z.ToString( "N1" ) );
			//	lines.Add( "Octree since last full rebuild: " + timeSinceLastFullRebuild.ToString( "F1" ) );
			//}

			string text = "";
			foreach( var line in lines )
				text += line + "\r\n";

			EditorMessageBox.ShowInfo( text, "Scene Info" );
		}
	}
}

#endif