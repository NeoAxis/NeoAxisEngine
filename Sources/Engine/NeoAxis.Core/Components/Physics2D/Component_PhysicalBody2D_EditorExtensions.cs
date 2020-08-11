// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	class Component_PhysicalBody2D_EditorExtensions : EditorExtensions
	{
		public override void Register()
		{
			var types = new List<Metadata.TypeInfo>();
			types.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Constraint2D_Revolute ) ) );
			types.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Constraint2D_Prismatic ) ) );
			types.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Constraint2D_Distance ) ) );
			types.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Constraint2D_Weld ) ) );
			types.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Constraint2D_Fixed ) ) );

			foreach( var type in types )
			{
				var displayName = TypeUtility.GetUserFriendlyNameForInstanceOfType( type.GetNetType() );

				var a = new EditorAction();
				a.Name = "Add " + displayName;
				//a.ImageSmall = Properties.Resources.New_16;
				//a.ImageBig = Properties.Resources.New_32;

				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				Component_PhysicalBody2D GetBody( object obj )
				{
					if( obj is Component_PhysicalBody2D body )
						return body;

					var c = obj as Component;
					if( c != null )
					{
						var body2 = c.GetComponent<Component_PhysicalBody2D>();
						if( body2 != null )
							return body2;
					}

					return null;
				}

				a.GetState += delegate ( EditorAction.GetStateContext context )
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
							//if( selectedObjects[ 0 ] is Component_PhysicalBody2D && selectedObjects[ 1 ] is Component_PhysicalBody2D )
							//	context.Enabled = true;
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					object[] selectedObjects = context.ObjectsInFocus.Objects;
					if( selectedObjects.Length == 2 )
					{
						var bodyA = GetBody( selectedObjects[ 0 ] );
						var bodyB = GetBody( selectedObjects[ 1 ] );
						//var bodyA = (Component_PhysicalBody2D)context.ObjectsInFocus.Objects[ 0 ];
						//var bodyB = (Component_PhysicalBody2D)context.ObjectsInFocus.Objects[ 1 ];

						var parent = ComponentUtility.FindNearestCommonParent( new Component[] { bodyA, bodyB } );
						if( parent != null )
						{
							var data = new NewObjectWindow.CreationDataClass();

							data.initDocumentWindow = context.ObjectsInFocus.DocumentWindow;
							data.initParentObjects = new List<object>();
							data.initParentObjects.Add( parent );
							data.initLockType = type;

							data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
							{
								var constraint = (Component_Constraint2D)data.createdComponentsOnTopLevel[ 0 ];

								constraint.BodyA = ReferenceUtility.MakeReference<Component_PhysicalBody2D>(
									null, ReferenceUtility.CalculateThisReference( constraint, bodyA ) );
								constraint.BodyB = ReferenceUtility.MakeReference<Component_PhysicalBody2D>(
									null, ReferenceUtility.CalculateThisReference( constraint, bodyB ) );

								if( constraint is Component_Constraint2D_Distance )
								{
									var pos = bodyA.Transform.Value.Position;
									var posB = bodyB.Transform.Value.Position;

									var distance = ( posB - pos ).Length();

									var rot = Quaternion.FromDirectionZAxisUp( ( posB - pos ).GetNormalize() );
									var scl = new Vector3( distance, distance, distance );

									constraint.Transform = new Transform( pos, rot, scl );
								}
								else
								{
									var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
									constraint.Transform = new Transform( pos, Quaternion.Identity );
								}
							};

							EditorAPI.OpenNewObjectWindow( data );
						}
					}
				};

				EditorActions.Register( a );
			}
		}
	}
}
