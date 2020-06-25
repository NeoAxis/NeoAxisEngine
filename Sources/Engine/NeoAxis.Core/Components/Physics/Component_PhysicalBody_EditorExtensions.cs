// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis.Editor
{
	class Component_PhysicalBody_EditorExtensions : EditorExtensions
	{
		public override void Register()
		{
			//Add Constraint
			{
				var a = new EditorAction();
				//!!!!"New Constraint"?
				a.Name = "Add Constraint";
				//a.imageSmall = Properties.Resources.Save_16;
				//a.imageBig = Properties.Resources.Save_32;

				//!!!!выключить где-то?
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenu.MenuTypeEnum.Document;

				Component_PhysicalBody GetBody( object obj )
				{
					if( obj is Component_PhysicalBody body )
						return body;

					var c = obj as Component;
					if( c != null )
					{
						var body2 = c.GetComponent<Component_PhysicalBody>();
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
							//if( selectedObjects[ 0 ] is Component_PhysicalBody && selectedObjects[ 1 ] is Component_PhysicalBody )
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
						//var bodyA = (Component_PhysicalBody)context.ObjectsInFocus.Objects[ 0 ];
						//var bodyB = (Component_PhysicalBody)context.ObjectsInFocus.Objects[ 1 ];

						var parent = ComponentUtility.FindNearestCommonParent( new Component[] { bodyA, bodyB } );
						if( parent != null )
						{
							var data = new NewObjectWindow.CreationDataClass();

							data.initDocumentWindow = context.ObjectsInFocus.DocumentWindow;
							data.initParentObjects = new List<object>();
							data.initParentObjects.Add( parent );
							data.initLockType = MetadataManager.GetTypeOfNetType( typeof( Component_Constraint ) );
							//data.initDemandedTypeDisableChange = true;

							data.additionActionBeforeEnabled = delegate ( NewObjectWindow window )
							{
								var constraint = (Component_Constraint)data.createdComponentsOnTopLevel[ 0 ];

								constraint.BodyA = ReferenceUtility.MakeReference<Component_PhysicalBody>(
									null, ReferenceUtility.CalculateThisReference( constraint, bodyA ) );
								constraint.BodyB = ReferenceUtility.MakeReference<Component_PhysicalBody>(
									null, ReferenceUtility.CalculateThisReference( constraint, bodyB ) );

								var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
								constraint.Transform = new Transform( pos, Quaternion.Identity );
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
