#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class SimulatePhysicsWorkareaMode : SceneEditorWorkareaMode
	{
		ESet<IPhysicalObject> allPhysicalObjects = new ESet<IPhysicalObject>();
		ESet<IPhysicalObject> selectedPhysicalObjects = new ESet<IPhysicalObject>();

		List<(RigidBody, Reference<PhysicsMotionType>)> restoreRigidBodyMotionType = new List<(RigidBody, Reference<PhysicsMotionType>)>();
		List<(RigidBody2D, Reference<RigidBody2D.MotionTypeEnum>)> restoreRigidBody2DMotionType = new List<(RigidBody2D, Reference<RigidBody2D.MotionTypeEnum>)>();

		Dictionary<RigidBody, Reference<Transform>> restoreTransformRigidBody = new Dictionary<RigidBody, Reference<Transform>>();
		Dictionary<RigidBody2D, Reference<Transform>> restoreTransformRigidBody2D = new Dictionary<RigidBody2D, Reference<Transform>>();
		Dictionary<SoftBody, Reference<Transform>> restoreTransformSoftBody = new Dictionary<SoftBody, Reference<Transform>>();
		Dictionary<MeshInSpace, Reference<Transform>> restoreTransformMeshInSpace = new Dictionary<MeshInSpace, Reference<Transform>>();

		Dictionary<Constraint_SixDOF, Reference<Transform>> restoreTransformConstraint = new Dictionary<Constraint_SixDOF, Reference<Transform>>();
		Dictionary<Constraint2D, Reference<Transform>> restoreTransformConstraint2D = new Dictionary<Constraint2D, Reference<Transform>>();

		List<(RigidBody, Reference<Vector3>)> restoreRigidBodyLinearVelocity = new List<(RigidBody, Reference<Vector3>)>();
		List<(RigidBody, Reference<Vector3>)> restoreRigidBodyAngularVelocity = new List<(RigidBody, Reference<Vector3>)>();
		List<(RigidBody2D, Reference<Vector2>)> restoreRigidBody2DLinearVelocity = new List<(RigidBody2D, Reference<Vector2>)>();
		List<(RigidBody2D, Reference<double>)> restoreRigidBody2DAngularVelocity = new List<(RigidBody2D, Reference<double>)>();

		List<(MeshInSpace, Reference<Vector3>)> restoreMeshInSpacePhysicalBodyLinearVelocity = new List<(MeshInSpace, Reference<Vector3>)>();
		List<(MeshInSpace, Reference<Vector3>)> restoreMeshInSpacePhysicalBodyAngularVelocity = new List<(MeshInSpace, Reference<Vector3>)>();

		double remainingDelta;

		bool cancel;

		//

		public SimulatePhysicsWorkareaMode( SceneEditor documentWindow )
			: base( documentWindow )
		{
			Start();
		}

		protected override void OnDestroy()
		{
			if( cancel )
				Cancel();
			else
				Apply();

			base.OnDestroy();
		}

		Scene Scene
		{
			get { return DocumentWindow.Scene; }
		}

		void Start()
		{
			DocumentWindow.Document.AllowUndoRedo = false;

			//get all physical objects
			foreach( var physicalObject in Scene.GetComponents<IPhysicalObject>( checkChildren: true ) )
			{
				if( ( (Component)physicalObject ).EnabledInHierarchy )
					allPhysicalObjects.AddWithCheckAlreadyContained( physicalObject );
			}

			//get selected physical objects
			foreach( var obj in DocumentWindow.SelectedObjects )
			{
				var physicalObject = obj as IPhysicalObject;
				if( physicalObject != null && ( (Component)physicalObject ).EnabledInHierarchy )
					selectedPhysicalObjects.AddWithCheckAlreadyContained( physicalObject );

				var objectInSpace = obj as ObjectInSpace;
				if( objectInSpace != null && objectInSpace.EnabledInHierarchy )
				{
					foreach( var physicalObject2 in objectInSpace.GetComponents<IPhysicalObject>( checkChildren: true ) )
						if( ( (Component)physicalObject2 ).EnabledInHierarchy )
							selectedPhysicalObjects.AddWithCheckAlreadyContained( physicalObject2 );
				}
			}

			//add linked constraints of selected bodies as selected
			foreach( var obj in allPhysicalObjects )
			{
				{
					var constraint = obj as Constraint_SixDOF;
					if( constraint != null )
					{
						var bodyA = constraint.BodyA.Value as IPhysicalObject;
						if( bodyA != null && selectedPhysicalObjects.Contains( bodyA ) )
							selectedPhysicalObjects.AddWithCheckAlreadyContained( obj );
						var bodyB = constraint.BodyB.Value as IPhysicalObject;
						if( bodyB != null && selectedPhysicalObjects.Contains( bodyB ) )
							selectedPhysicalObjects.AddWithCheckAlreadyContained( obj );
					}
				}

				{
					var constraint = obj as Constraint2D;
					if( constraint != null )
					{
						var bodyA = constraint.BodyA.Value;
						if( bodyA != null && selectedPhysicalObjects.Contains( bodyA ) )
							selectedPhysicalObjects.AddWithCheckAlreadyContained( obj );
						var bodyB = constraint.BodyB.Value;
						if( bodyB != null && selectedPhysicalObjects.Contains( bodyB ) )
							selectedPhysicalObjects.AddWithCheckAlreadyContained( obj );
					}
				}
			}

			//make static and save properties not selected objects
			foreach( var obj in allPhysicalObjects )
			{
				if( !selectedPhysicalObjects.Contains( obj ) )
				{
					{
						var rigidBody = obj as RigidBody;
						if( rigidBody != null )
						{
							if( rigidBody.MotionType.Value == PhysicsMotionType.Dynamic )
							{
								restoreRigidBodyMotionType.Add( (rigidBody, rigidBody.MotionType) );
								restoreRigidBodyLinearVelocity.Add( (rigidBody, rigidBody.LinearVelocity) );
								restoreRigidBodyAngularVelocity.Add( (rigidBody, rigidBody.AngularVelocity) );

								rigidBody.MotionType = PhysicsMotionType.Static;
							}
						}
					}

					{
						var rigidBody = obj as RigidBody2D;
						if( rigidBody != null )
						{
							if( rigidBody.MotionType.Value == RigidBody2D.MotionTypeEnum.Dynamic )
							{
								restoreRigidBody2DMotionType.Add( (rigidBody, rigidBody.MotionType) );
								restoreRigidBody2DLinearVelocity.Add( (rigidBody, rigidBody.LinearVelocity) );
								restoreRigidBody2DAngularVelocity.Add( (rigidBody, rigidBody.AngularVelocity) );

								rigidBody.MotionType = RigidBody2D.MotionTypeEnum.Static;
							}
						}
					}

					{
						var meshInSpace = obj as MeshInSpace;
						if( meshInSpace != null )
						{
							if( meshInSpace.Collision )
							{
								//!!!!
								//right now is recreating
								restoreTransformMeshInSpace[ meshInSpace ] = meshInSpace.Transform;

								restoreMeshInSpacePhysicalBodyLinearVelocity.Add( (meshInSpace, meshInSpace.PhysicalBodyLinearVelocity) );
								restoreMeshInSpacePhysicalBodyAngularVelocity.Add( (meshInSpace, meshInSpace.PhysicalBodyAngularVelocity) );

								//!!!!
								//right now is recreating
								meshInSpace.Enabled = false;
								meshInSpace.Enabled = true;
							}
						}
					}
				}
			}

			//!!!!soft body can't be static
			//save properties all soft bodies
			foreach( var obj in allPhysicalObjects )
			{
				{
					var softBody = obj as SoftBody;
					if( softBody != null )
						restoreTransformSoftBody[ softBody ] = softBody.Transform;
				}
			}

			//save properties of selected objects
			foreach( var obj in selectedPhysicalObjects )
			{
				{
					var rigidBody = obj as RigidBody;
					if( rigidBody != null )
					{
						restoreTransformRigidBody[ rigidBody ] = rigidBody.Transform;
						restoreRigidBodyLinearVelocity.Add( (rigidBody, rigidBody.LinearVelocity) );
						restoreRigidBodyAngularVelocity.Add( (rigidBody, rigidBody.AngularVelocity) );
					}
				}

				{
					var rigidBody = obj as RigidBody2D;
					if( rigidBody != null )
					{
						restoreTransformRigidBody2D[ rigidBody ] = rigidBody.Transform;
						restoreRigidBody2DLinearVelocity.Add( (rigidBody, rigidBody.LinearVelocity) );
						restoreRigidBody2DAngularVelocity.Add( (rigidBody, rigidBody.AngularVelocity) );
					}
				}

				{
					var meshInSpace = obj as MeshInSpace;
					if( meshInSpace != null )
					{
						if( meshInSpace.Collision )
						{
							restoreTransformMeshInSpace[ meshInSpace ] = meshInSpace.Transform;
							restoreMeshInSpacePhysicalBodyLinearVelocity.Add( (meshInSpace, meshInSpace.PhysicalBodyLinearVelocity) );
							restoreMeshInSpacePhysicalBodyAngularVelocity.Add( (meshInSpace, meshInSpace.PhysicalBodyAngularVelocity) );

							//!!!!
							//right now is recreating

							meshInSpace.Enabled = false;
							meshInSpace.Enabled = true;
						}
					}
				}

				{
					var constraint = obj as Constraint_SixDOF;
					if( constraint != null )
						restoreTransformConstraint[ constraint ] = constraint.Transform;
				}

				{
					var constraint = obj as Constraint2D;
					if( constraint != null )
						restoreTransformConstraint2D[ constraint ] = constraint.Transform;
				}
			}

			//wake up selected objects
			foreach( var physicalObject in selectedPhysicalObjects )
			{
				var rigidBody = physicalObject as RigidBody;
				if( rigidBody != null )
					rigidBody.Active = true;//Activate();

				var rigidBody2D = physicalObject as RigidBody2D;
				if( rigidBody2D != null )
					rigidBody2D.Active = true;
			}
		}

		void RestoreObjects()
		{
			//restore properties
			foreach( var pair in restoreRigidBodyMotionType )
				pair.Item1.MotionType = pair.Item2;
			foreach( var pair in restoreRigidBody2DMotionType )
				pair.Item1.MotionType = pair.Item2;
			foreach( var pair in restoreRigidBodyLinearVelocity )
				pair.Item1.LinearVelocity = pair.Item2;
			foreach( var pair in restoreRigidBodyAngularVelocity )
				pair.Item1.AngularVelocity = pair.Item2;
			foreach( var pair in restoreRigidBody2DLinearVelocity )
				pair.Item1.LinearVelocity = pair.Item2;
			foreach( var pair in restoreRigidBody2DAngularVelocity )
				pair.Item1.AngularVelocity = pair.Item2;
			foreach( var pair in restoreMeshInSpacePhysicalBodyLinearVelocity )
				pair.Item1.PhysicalBodyLinearVelocity = pair.Item2;
			foreach( var pair in restoreMeshInSpacePhysicalBodyAngularVelocity )
				pair.Item1.PhysicalBodyAngularVelocity = pair.Item2;

			//!!!!recreate meshInSpace. can't be static
			foreach( var pair in restoreTransformMeshInSpace )
			{
				pair.Key.Enabled = false;
				pair.Key.Enabled = true;
			}

			//!!!!soft body can't be static
			foreach( var pair in restoreTransformSoftBody )
			{
				pair.Key.Enabled = false;
				pair.Key.Transform = pair.Value;
				pair.Key.Enabled = true;
			}

			//wake up objects
			foreach( var physicalObject in allPhysicalObjects )
			{
				var rigidBody = physicalObject as RigidBody;
				if( rigidBody != null )
					rigidBody.Active = true;//Activate();

				var rigidBody2D = physicalObject as RigidBody2D;
				if( rigidBody2D != null )
					rigidBody2D.Active = true;
			}

			DocumentWindow.Document.AllowUndoRedo = true;
		}

		void Cancel()
		{
			RestoreObjects();

			//restore old transform of selected objects
			foreach( var pair in restoreTransformRigidBody )
				pair.Key.Transform = pair.Value;
			foreach( var pair in restoreTransformMeshInSpace )
				pair.Key.Transform = pair.Value;
			foreach( var pair in restoreTransformRigidBody2D )
				pair.Key.Transform = pair.Value;
			foreach( var pair in restoreTransformConstraint )
			{
				pair.Key.Enabled = false;
				pair.Key.Transform = pair.Value;
				pair.Key.Enabled = true;
			}
			foreach( var pair in restoreTransformConstraint2D )
			{
				pair.Key.Enabled = false;
				pair.Key.Transform = pair.Value;
				pair.Key.Enabled = true;
			}
		}

		void Apply()
		{
			RestoreObjects();

			//undo
			{
				var undoItems = new List<UndoActionPropertiesChange.Item>();

				foreach( var obj in selectedPhysicalObjects )
				{
					{
						var rigidBody = obj as RigidBody;
						if( rigidBody != null )
						{
							if( restoreTransformRigidBody.TryGetValue( rigidBody, out var oldValue ) )
							{
								var p = rigidBody.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
								if( p != null )
									undoItems.Add( new UndoActionPropertiesChange.Item( obj, p, oldValue, null ) );
							}
						}
					}

					{
						var meshInSpace = obj as MeshInSpace;
						if( meshInSpace != null )
						{
							if( restoreTransformMeshInSpace.TryGetValue( meshInSpace, out var oldValue ) )
							{
								var p = meshInSpace.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
								if( p != null )
									undoItems.Add( new UndoActionPropertiesChange.Item( obj, p, oldValue, null ) );
							}
						}
					}

					{
						var rigidBody = obj as RigidBody2D;
						if( rigidBody != null )
						{
							if( restoreTransformRigidBody2D.TryGetValue( rigidBody, out var oldValue ) )
							{
								var p = rigidBody.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
								if( p != null )
									undoItems.Add( new UndoActionPropertiesChange.Item( obj, p, oldValue, null ) );
							}
						}
					}

					{
						var constraint = obj as Constraint_SixDOF;
						if( constraint != null )
						{
							if( restoreTransformConstraint.TryGetValue( constraint, out var oldValue ) )
							{
								var p = constraint.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
								if( p != null )
									undoItems.Add( new UndoActionPropertiesChange.Item( obj, p, oldValue, null ) );
							}
						}
					}

					{
						var constraint = obj as Constraint2D;
						if( constraint != null )
						{
							if( restoreTransformConstraint2D.TryGetValue( constraint, out var oldValue ) )
							{
								var p = constraint.MetadataGetMemberBySignature( "property:Transform" ) as Metadata.Property;
								if( p != null )
									undoItems.Add( new UndoActionPropertiesChange.Item( obj, p, oldValue, null ) );
							}
						}
					}
				}

				if( undoItems.Count != 0 )
				{
					var action = new UndoActionPropertiesChange( undoItems.ToArray() );
					DocumentWindow.Document.CommitUndoAction( action );
				}
			}
		}

		protected override bool OnKeyDown( Viewport viewport, KeyEvent e )
		{
			if( e.Key == EKeys.Return || e.Key == EKeys.Space )
			{
				DocumentWindow.ResetWorkareaMode();
				return true;
			}

			if( e.Key == EKeys.Escape )
			{
				cancel = true;
				DocumentWindow.ResetWorkareaMode();
				return true;
			}

			return base.OnKeyDown( viewport, e );
		}

		//!!!!
		string Translate( string text )
		{
			return text;
			//return ToolsLocalization.Translate( "SimulatePhysicsWorkareaMode", text );
		}

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			lines.Add( Translate( "Physics simulation mode" ) );
			lines.Add( "" );
			lines.Add( Translate( "Press Space or Return to apply changes, press Escape to cancel." ) );
		}

		protected override void OnTick( Viewport viewport, double delta )
		{
			base.OnTick( viewport, delta );

			//simulate
			remainingDelta += delta;
			while( remainingDelta > ProjectSettings.Get.General.SimulationStepsPerSecondInv )
			{
				remainingDelta -= ProjectSettings.Get.General.SimulationStepsPerSecondInv;
				DocumentWindow.Scene.PhysicsSimulate( true, selectedPhysicalObjects );
				DocumentWindow.Scene.Physics2DSimulate( true, selectedPhysicalObjects );
			}
		}
	}
}

#endif