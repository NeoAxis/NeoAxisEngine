#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	class PhysicalBodyEditorExtensions : EditorExtensions
	{
		public override void OnRegister()
		{
			//Add Constraint
			{
				var a = new EditorAction();
				//!!!!"New Constraint"?
				a.Name = "Add Constraint 6DOF";
				//a.imageSmall = Properties.Resources.Save_16;
				//a.imageBig = Properties.Resources.Save_32;

				//!!!!выключить где-то?
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				ObjectInSpace GetBody( object obj )
				{
					if( obj is PhysicalBody body )
						return body;

					var c = obj as Component;
					if( c != null )
					{
						var body2 = c.GetComponent<PhysicalBody>();
						if( body2 != null )
							return body2;
					}

					var meshInSpace = obj as MeshInSpace;
					if( meshInSpace != null && meshInSpace.Collision )
						return meshInSpace;

					return null;
				}

				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow != null )
					{
						object[] selectedObjects = context.ObjectsInFocus.Objects;
						if( selectedObjects.Length == 2 )
						{
							var bodyA = GetBody( selectedObjects[ 0 ] );
							var bodyB = GetBody( selectedObjects[ 1 ] );

							if( bodyA != null && bodyB != null )
								context.Enabled = true;
							//if( selectedObjects[ 0 ] is PhysicalBody && selectedObjects[ 1 ] is PhysicalBody )
							//	context.Enabled = true;
						}
					}
				};

				a.Click += delegate ( EditorActionClickContext context )
				{
					object[] selectedObjects = context.ObjectsInFocus.Objects;
					if( selectedObjects.Length == 2 )
					{
						var bodyA = GetBody( selectedObjects[ 0 ] );
						var bodyB = GetBody( selectedObjects[ 1 ] );
						//var bodyA = (PhysicalBody)context.ObjectsInFocus.Objects[ 0 ];
						//var bodyB = (PhysicalBody)context.ObjectsInFocus.Objects[ 1 ];

						var parent = ComponentUtility.FindNearestCommonParent( new Component[] { bodyA, bodyB } );
						if( parent != null )
						{
							var data = new NewObjectWindow.CreationDataClass();

							data.initDocumentWindow = (DocumentWindow)context.ObjectsInFocus.DocumentWindow;
							data.initParentObjects = new List<object>();
							data.initParentObjects.Add( parent );
							data.initLockType = MetadataManager.GetTypeOfNetType( typeof( Constraint_SixDOF ) );
							//data.initDemandedTypeDisableChange = true;

							data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
							{
								var constraint = (Constraint_SixDOF)data.createdComponentsOnTopLevel[ 0 ];

								constraint.BodyA = ReferenceUtility.MakeReference<ObjectInSpace>( null, ReferenceUtility.CalculateThisReference( constraint, bodyA ) );
								constraint.BodyB = ReferenceUtility.MakeReference<ObjectInSpace>( null, ReferenceUtility.CalculateThisReference( constraint, bodyB ) );

								//constraint.BodyA = ReferenceUtility.MakeReference<PhysicalBody>( null, ReferenceUtility.CalculateThisReference( constraint, bodyA ) );
								//constraint.BodyB = ReferenceUtility.MakeReference<PhysicalBody>( null, ReferenceUtility.CalculateThisReference( constraint, bodyB ) );

								var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
								constraint.Transform = new Transform( pos, Quaternion.Identity );
							};

							EditorAPI2.OpenNewObjectWindow( data );
						}
					}
				};

				EditorActions.Register( a );
			}
		}
	}
}

#endif