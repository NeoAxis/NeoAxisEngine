#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	class MeshInSpaceEditorExtensions : EditorExtensions
	{
		string Translate( string text )
		{
			return EditorLocalization2.Translate( "MeshInSpace", text );
		}

		public override void OnRegister()
		{
			//Add Collision
			{
				const string bodyName = "Collision Body";

				var a = new EditorAction();
				a.Name = "Add Collision";
				a.Description = "Adds a collision body to selected objects. For meshes in the scene the collision is also can be enabled by Collision property.";
				a.ImageSmall = Properties.Resources.Add_16;
				a.ImageBig = Properties.Resources.MeshCollision_32;
				a.ActionType = EditorAction.ActionTypeEnum.DropDown;
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow != null )
					{
						object[] selectedObjects = context.ObjectsInFocus.Objects;
						if( selectedObjects.Length != 0 && Array.TrueForAll( selectedObjects, obj => obj is MeshInSpace ) )
						{
							context.Enabled = Array.Exists( selectedObjects, delegate ( object obj )
							{
								var c = ( (Component)obj ).GetComponent( bodyName );
								if( c != null )
								{
									if( c is RigidBody )
										return false;
									if( c is RigidBody2D )
										return false;
								}
								return true;
							} );
						}

						a.DropDownContextMenu.Tag = (context.ObjectsInFocus.DocumentWindow.Document, selectedObjects);
					}
				};

				//context menu
				{
					a.DropDownContextMenu = new KryptonContextMenu();

					a.DropDownContextMenu.Opening += delegate ( object sender, CancelEventArgs e )
					{
						var menu = (KryptonContextMenu)sender;
						var tuple = ((IDocumentInstance, object[]))menu.Tag;

						//"Collision Body of the Mesh"
						{
							var items2 = (KryptonContextMenuItems)menu.Items[ 0 ];
							var item2 = (KryptonContextMenuItem)items2.Items[ 0 ];

							bool enabled = false;

							foreach( var obj in tuple.Item2 )
							{
								var meshInSpace = obj as MeshInSpace;
								if( meshInSpace != null )
								{
									RigidBody collisionDefinition = null;
									{
										var mesh = meshInSpace.Mesh.Value;
										if( mesh != null )
											collisionDefinition = mesh.GetComponent( "Collision Definition" ) as RigidBody;
									}

									if( collisionDefinition != null )
										enabled = true;
								}
							}

							item2.Enabled = enabled;
						}

					};

					System.EventHandler clickHandler = delegate ( object s, EventArgs e2 )
					{
						var item = (KryptonContextMenuItem)s;
						var itemTag = ((KryptonContextMenu, string))item.Tag;
						var menu = itemTag.Item1;
						var collisionName = itemTag.Item2;

						var menuTag = ((IDocumentInstance, object[]))menu.Tag;
						var document = menuTag.Item1;
						var selectedObjects = menuTag.Item2;

						List<UndoSystem.Action> undoActions = new List<UndoSystem.Action>();

						foreach( var obj in selectedObjects )
						{
							if( obj is MeshInSpace meshInSpace && meshInSpace.GetComponent( bodyName ) as RigidBody == null && meshInSpace.GetComponent( bodyName ) as RigidBody2D == null )
							{
								var mesh = meshInSpace.MeshOutput;
								if( mesh == null )
									continue;

								Component body = null;
								bool skip = false;

								if( collisionName == "Use Collision of the Mesh" )
								{
									var collisionDefinition = mesh.GetComponent( "Collision Definition" ) as RigidBody;
									if( collisionDefinition != null )
									{
										var context = new Metadata.CloneContext();
										//to convert root references to fullpath 'resource' references
										context.TypeOfCloning = Metadata.CloneContext.TypeOfCloningEnum.CreateInstanceOfType;
										var body2 = (RigidBody)collisionDefinition.Clone( context );

										body = body2;
										body2.Enabled = false;
										body2.Name = bodyName;
										//body2.MotionType = RigidBody.MotionTypeEnum.Static;
										body2.Transform = meshInSpace.Transform;

										meshInSpace.AddComponent( body2 );

										//!!!!make references to Vertices, Indices?

									}
									else
										skip = true;
								}
								else
								{
									RigidBody CreateRigidBody()
									{
										var body2 = meshInSpace.CreateComponent<RigidBody>( enabled: false );
										body2.Name = bodyName;
										body2.Transform = meshInSpace.Transform;
										return body2;
									}

									RigidBody2D CreateRigidBody2D()
									{
										var body2 = meshInSpace.CreateComponent<RigidBody2D>( enabled: false );
										body2.Name = bodyName;
										body2.Transform = meshInSpace.Transform;
										return body2;
									}

									switch( collisionName )
									{
									case "Box":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Box>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
											shape.Dimensions = bounds.GetSize();
										}
										break;

									case "Sphere":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Sphere>();
											var sphere = mesh.Result.SpaceBounds.BoundingSphere;
											shape.LocalTransform = new Transform( sphere.Center, Quaternion.Identity );
											shape.Radius = sphere.Radius;
										}
										break;

									case "Capsule":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Capsule>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
											shape.Radius = Math.Max( bounds.GetSize().X, bounds.GetSize().Y ) / 2;
											shape.Height = Math.Max( bounds.GetSize().Z - shape.Radius * 2, 0 );
										}
										break;

									case "Cylinder":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Cylinder>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.LocalTransform = new Transform( bounds.GetCenter(), Quaternion.Identity );
											shape.Radius = Math.Max( bounds.GetSize().X, bounds.GetSize().Y ) / 2;
											shape.Height = bounds.GetSize().Z;
										}
										break;

									case "Convex":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Mesh>();
											shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;
											shape.Mesh = ReferenceUtility.MakeThisReference( shape, meshInSpace, "Mesh" );
										}
										break;

									case "Convex Decomposition":
										{
											body = CreateRigidBody();

											var settings = new ConvexDecomposition.Settings();

											var form = new SpecifyParametersForm( "Convex Decomposition", settings );
											form.CheckHandler = delegate ( ref string error2 )
											{
												return true;
											};
											if( form.ShowDialog() != DialogResult.OK )
												skip = true;
											else
											{
												var clusters = ConvexDecomposition.Decompose( mesh.Result.ExtractedVerticesPositions, mesh.Result.ExtractedIndices, settings );

												if( clusters == null )
												{
													Log.Warning( "Unable to decompose." );
													skip = true;
												}
												else
												{
													foreach( var cluster in clusters )
													{
														var shape = body.CreateComponent<CollisionShape_Mesh>();
														shape.Vertices = cluster.Vertices;
														shape.Indices = cluster.Indices;
														shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;
													}
												}
											}
										}
										break;

									case "Mesh":
										{
											body = CreateRigidBody();
											var shape = body.CreateComponent<CollisionShape_Mesh>();
											shape.Mesh = ReferenceUtility.MakeThisReference( shape, meshInSpace, "Mesh" );
										}
										break;

									case "Box 2D":
										{
											body = CreateRigidBody2D();
											var shape = body.CreateComponent<CollisionShape2D_Box>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );
											shape.Dimensions = bounds.GetSize().ToVector2();
										}
										break;

									case "Circle 2D":
										{
											body = CreateRigidBody2D();
											var shape = body.CreateComponent<CollisionShape2D_Ellipse>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );
											var size = bounds.GetSize().ToVector2().MaxComponent();
											shape.Dimensions = new Vector2( size, size );
										}
										break;

									case "Ellipse 2D":
										{
											body = CreateRigidBody2D();
											var shape = body.CreateComponent<CollisionShape2D_Ellipse>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );
											shape.Dimensions = bounds.GetSize().ToVector2();
										}
										break;

									case "Capsule 2D":
										{
											body = CreateRigidBody2D();
											var shape = body.CreateComponent<CollisionShape2D_Capsule>();
											var bounds = mesh.Result.SpaceBounds.BoundingBox;
											shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );

											var size = bounds.GetSize();

											if( size.X > size.Y )
											{
												shape.Axis = 0;
												shape.Radius = size.Y / 2;
												shape.Height = Math.Max( size.X - shape.Radius * 2, 0 );
											}
											else
											{
												shape.Axis = 0;
												shape.Radius = size.X / 2;
												shape.Height = Math.Max( size.Y - shape.Radius * 2, 0 );
											}
										}
										break;

									case "Convex 2D":
										{
											body = CreateRigidBody2D();

											var meshPoints = new Vector2[ mesh.Result.ExtractedVerticesPositions.Length ];
											for( int n = 0; n < meshPoints.Length; n++ )
												meshPoints[ n ] = mesh.Result.ExtractedVerticesPositions[ n ].ToVector2();
											var points = MathAlgorithms.GetConvexByPoints( meshPoints );

											var vertices = new Vector3F[ points.Count ];
											var indices = new int[ ( points.Count - 2 ) * 3 ];
											{
												for( int n = 0; n < points.Count; n++ )
													vertices[ n ] = new Vector3F( points[ n ].ToVector2F(), 0 );

												for( int nTriangle = 0; nTriangle < points.Count - 2; nTriangle++ )
												{
													indices[ nTriangle * 3 + 0 ] = 0;
													indices[ nTriangle * 3 + 1 ] = nTriangle + 1;
													indices[ nTriangle * 3 + 2 ] = nTriangle + 2;
												}
											}

											var shape = body.CreateComponent<CollisionShape2D_Mesh>();
											shape.Vertices = vertices;
											shape.Indices = indices;
											shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.Convex;

											//var polygons = new List<List<Vector2>>();
											//{
											//	var currentList = new List<Vector2>();

											//	for( int vertex = 0; vertex < points.Count; vertex++ )
											//	{
											//		currentList.Add( points[ vertex ] );

											//		if( currentList.Count == Settings.MaxPolygonVertices )
											//		{
											//			polygons.Add( currentList );

											//			currentList = new List<Vector2>();
											//			currentList.Add( points[ 0 ] );
											//			currentList.Add( points[ vertex ] );
											//		}
											//	}

											//	if( currentList.Count >= 3 )
											//		polygons.Add( currentList );
											//}

											//foreach( var points2 in polygons )
											//{
											//	var vertices = new Vector3F[ points2.Count ];
											//	var indices = new int[ ( points2.Count - 2 ) * 3 ];
											//	{
											//		for( int n = 0; n < points2.Count; n++ )
											//			vertices[ n ] = new Vector3F( points2[ n ].ToVector2F(), 0 );

											//		for( int nTriangle = 0; nTriangle < points2.Count - 2; nTriangle++ )
											//		{
											//			indices[ nTriangle * 3 + 0 ] = 0;
											//			indices[ nTriangle * 3 + 1 ] = nTriangle + 1;
											//			indices[ nTriangle * 3 + 2 ] = nTriangle + 2;
											//		}
											//	}

											//	var shape = body.CreateComponent<CollisionShape2D_Mesh>();
											//	shape.Vertices = vertices;
											//	shape.Indices = indices;
											//	shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.Convex;
											//}
										}
										break;

									//case "Convex Decomposition 2D":
									//	{
									//		body = CreateRigidBody2D();

									//		var settings = new ConvexDecomposition.Settings();

									//		var form = new SpecifyParametersForm( "Convex Decomposition 2D", settings );
									//		form.CheckHandler = delegate ( ref string error2 )
									//		{
									//			return true;
									//		};
									//		if( form.ShowDialog() != DialogResult.OK )
									//			skip = true;
									//		else
									//		{
									//			//var sourceVertices = (Vector3F[])mesh.Result.ExtractedVerticesPositions.Clone();
									//			////reset Z
									//			//for( int n = 0; n < sourceVertices.Length; n++ )
									//			//	sourceVertices[ n ] = new Vector3F( sourceVertices[ n ].ToVector2(), 0 );

									//			//var sourceIndices = mesh.Result.ExtractedIndices;

									//			//var epsilon = 0.0001f;
									//			//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, out var processedVertices, out var processedIndices, out var processedTrianglesToSourceIndex );

									//			//var vertices = new Vector3F[ mesh.Result.ExtractedVerticesPositions.Length ];
									//			////reset Z
									//			//for( int n = 0; n < vertices.Length; n++ )
									//			//	vertices[ n ] = new Vector3F( mesh.Result.ExtractedVerticesPositions[ n ].ToVector2(), 0 );

									//			//var clusters = ConvexDecomposition.Decompose( processedVertices, processedIndices, settings );


									//			var vertices = new Vector3F[ mesh.Result.ExtractedVerticesPositions.Length ];
									//			//reset Z
									//			for( int n = 0; n < vertices.Length; n++ )
									//				vertices[ n ] = new Vector3F( mesh.Result.ExtractedVerticesPositions[ n ].ToVector2(), 0 );

									//			var clusters = ConvexDecomposition.Decompose( vertices, mesh.Result.ExtractedIndices, settings );

									//			if( clusters == null )
									//			{
									//				Log.Warning( "Unable to decompose." );
									//				skip = true;
									//			}
									//			else
									//			{
									//				foreach( var cluster in clusters )
									//				{
									//					var shape = body.CreateComponent<CollisionShape2D_Mesh>();
									//					shape.Vertices = cluster.Vertices;
									//					shape.Indices = cluster.Indices;
									//					shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.Convex;
									//				}
									//			}
									//		}



									//		//var sourceVertices = new Vertices();
									//		//foreach( var p in mesh.Result.ExtractedVerticesPositions )
									//		//	sourceVertices.Add( Physics2DUtility.Convert( p.ToVector2() ) );

									//		//var list = Triangulate.ConvexPartition( sourceVertices, TriangulationAlgorithm.Seidel, tolerance: 0.001f );

									//		//body = CreateRigidBody2D();

									//		//foreach( var convexVertices in list )
									//		//{
									//		//	var shape = body.CreateComponent<CollisionShape2D_Mesh>();

									//		//	var points = new List<Vector2>();
									//		//	foreach( var p in convexVertices )
									//		//		points.Add( Physics2DUtility.Convert( p ) );

									//		//	//var meshPoints = new Vector2[ mesh.Result.ExtractedVerticesPositions.Length ];
									//		//	//for( int n = 0; n < meshPoints.Length; n++ )
									//		//	//	meshPoints[ n ] = mesh.Result.ExtractedVerticesPositions[ n ].ToVector2();
									//		//	//var points = MathAlgorithms.GetConvexByPoints( meshPoints );

									//		//	var vertices = new Vector3F[ points.Count + 1 ];
									//		//	var indices = new int[ points.Count * 3 ];
									//		//	{
									//		//		var center = Vector2.Zero;
									//		//		foreach( var p in points )
									//		//			center += p;
									//		//		center /= points.Count;
									//		//		vertices[ 0 ] = new Vector3F( center.ToVector2F(), 0 );

									//		//		for( int n = 0; n < points.Count; n++ )
									//		//		{
									//		//			vertices[ 1 + n ] = new Vector3F( points[ n ].ToVector2F(), 0 );

									//		//			indices[ n * 3 + 0 ] = 0;
									//		//			indices[ n * 3 + 1 ] = 1 + n;
									//		//			indices[ n * 3 + 2 ] = 1 + ( ( n + 1 ) % points.Count );
									//		//		}
									//		//	}

									//		//	shape.Vertices = vertices;
									//		//	shape.Indices = indices;
									//		//	shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.Convex;
									//		//}



									//		//body = CreateRigidBody2D();
									//		//var shape = body.CreateComponent<CollisionShape2D_Mesh>();

									//		//var meshPoints = new Vector2[ mesh.Result.ExtractedVerticesPositions.Length ];
									//		//for( int n = 0; n < meshPoints.Length; n++ )
									//		//	meshPoints[ n ] = mesh.Result.ExtractedVerticesPositions[ n ].ToVector2();
									//		//var points = MathAlgorithms.GetConvexByPoints( meshPoints );

									//		//var vertices = new Vector3F[ points.Count + 1 ];
									//		//var indices = new int[ points.Count * 3 ];
									//		//{
									//		//	var center = Vector2.Zero;
									//		//	foreach( var p in points )
									//		//		center += p;
									//		//	center /= points.Count;
									//		//	vertices[ 0 ] = new Vector3F( center.ToVector2F(), 0 );

									//		//	for( int n = 0; n < points.Count; n++ )
									//		//	{
									//		//		vertices[ 1 + n ] = new Vector3F( points[ n ].ToVector2F(), 0 );

									//		//		indices[ n * 3 + 0 ] = 0;
									//		//		indices[ n * 3 + 1 ] = 1 + n;
									//		//		indices[ n * 3 + 2 ] = 1 + ( ( n + 1 ) % points.Count );
									//		//	}
									//		//}

									//		//shape.Vertices = vertices;
									//		//shape.Indices = indices;
									//		//shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.Convex;

									//		//var bounds = mesh.Result.SpaceBounds.CalculatedBoundingBox;
									//		//shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );

									//		//shape.Mesh = ReferenceUtility.MakeThisReference( shape, meshInSpace, "Mesh" );

									//		//var shapeVertices = new Vector3F[ points.Count ];
									//		//for( int n = 0; n < shapeVertices.Length; n++ )
									//		//	shapeVertices[ n ] = new Vector3F( points[ n ].ToVector2F(), 0 );
									//		//shape.Vertices = shapeVertices;
									//	}
									//	break;

									case "Mesh 2D":
										{
											body = CreateRigidBody2D();
											var shape = body.CreateComponent<CollisionShape2D_Mesh>();
											shape.Mesh = ReferenceUtility.MakeThisReference( shape, meshInSpace, "Mesh" );
											shape.ShapeType = CollisionShape2D_Mesh.ShapeTypeEnum.TriangleMesh;

											//var bounds = mesh.Result.SpaceBounds.CalculatedBoundingBox;
											//shape.TransformRelativeToParent = new Transform( bounds.GetCenter(), Quaternion.Identity );

											//var halfSize = bounds.GetSize().ToVector2() * 0.5;

											//var meshPoints = new List<Vector2>( mesh.Result.ExtractedVerticesPositions.Length );
											//foreach( var p in mesh.Result.ExtractedVerticesPositions )
											//	meshPoints.Add( p.ToVector2() );
											//var convexPoints = MathAlgorithms.GetConvexByPoints( meshPoints );

											//var points = shape.PropertyGet( "Points" );
											//foreach( var p in convexPoints )
											//	points.MethodInvoke( "Add", p );

											//points.MethodInvoke( "Add", new Vector2( -halfSize.X, -halfSize.Y ) );
											//points.MethodInvoke( "Add", new Vector2( halfSize.X, -halfSize.Y ) );
											//points.MethodInvoke( "Add", new Vector2( halfSize.X, halfSize.Y ) );
											//points.MethodInvoke( "Add", new Vector2( -halfSize.X, halfSize.Y ) );
										}
										break;

									//case "Polygon 2D":
									//	{
									//		body = CreateRigidBody2D();
									//		if( body != null )
									//		{
									//			var shapeType = MetadataManager.GetType( "NeoAxis.CollisionShape2D_Polygon" );
									//			if( shapeType != null )
									//			{
									//				var shape = body.CreateComponent( shapeType );
									//				var bounds = mesh.Result.SpaceBounds.CalculatedBoundingBox;
									//				shape.PropertySet( "TransformRelativeToParent", new Transform( bounds.GetCenter(), Quaternion.Identity ) );

									//				var halfSize = bounds.GetSize().ToVector2() * 0.5;

									//				var meshPoints = new List<Vector2>( mesh.Result.ExtractedVerticesPositions.Length );
									//				foreach( var p in mesh.Result.ExtractedVerticesPositions )
									//					meshPoints.Add( p.ToVector2() );
									//				var convexPoints = MathAlgorithms.GetConvexByPoints( meshPoints );

									//				var points = shape.PropertyGet( "Points" );
									//				foreach( var p in convexPoints )
									//					points.MethodInvoke( "Add", p );

									//				//points.MethodInvoke( "Add", new Vector2( -halfSize.X, -halfSize.Y ) );
									//				//points.MethodInvoke( "Add", new Vector2( halfSize.X, -halfSize.Y ) );
									//				//points.MethodInvoke( "Add", new Vector2( halfSize.X, halfSize.Y ) );
									//				//points.MethodInvoke( "Add", new Vector2( -halfSize.X, halfSize.Y ) );
									//			}
									//			else
									//				skip = true;
									//		}
									//	}
									//	break;

									default:
										Log.Warning( "No implementation." );
										skip = true;
										continue;
									}
								}

								if( skip )
								{
									body?.Dispose();
									continue;
								}

								if( body != null )
								{
									body.Enabled = true;

									undoActions.Add( new UndoActionComponentCreateDelete( document, new Component[] { body }, true ) );

									//change Transform
									{
										//undo action
										var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:Transform" );
										var undoItem = new UndoActionPropertiesChange.Item( meshInSpace, property, meshInSpace.Transform, new object[ 0 ] );
										undoActions.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );

										//configure reference
										meshInSpace.Transform = ReferenceUtility.MakeReference<Transform>( null, ReferenceUtility.CalculateThisReference( meshInSpace, body, "Transform" ) );
									}
								}
							}
						}

						if( undoActions.Count != 0 )
						{
							document.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
							document.Modified = true;
							ScreenNotifications2.Show( Translate( "The collision was added successfully." ) );
						}

					};

					var items = new List<KryptonContextMenuItemBase>();
					var names = new string[] { "Use Collision of the Mesh", "", "Box", "Sphere", "Capsule", "Cylinder", "Convex", "Convex Decomposition", "Mesh", "", "Box 2D", "Circle 2D", "Ellipse 2D", "Capsule 2D", "Convex 2D", /*"Convex Decomposition 2D", */"Mesh 2D" };
					foreach( var name in names )
					{
						if( name == "" )
							items.Add( new KryptonContextMenuSeparator() );
						else
						{
							var item = new KryptonContextMenuItem( name, null, clickHandler );
							item.Tag = (a.DropDownContextMenu, name);
							items.Add( item );
						}
					}

					a.DropDownContextMenu.Items.Add( new KryptonContextMenuItems( items.ToArray() ) );
				}

				EditorActions.Register( a );
			}

			//Delete Collision
			{
				const string bodyName = "Collision Body";

				var a = new EditorAction();
				a.Name = "Delete Collision";
				a.Description = "Deletes the collision body of selected objects.";
				a.ImageSmall = Properties.Resources.Delete_16;
				a.ImageBig = Properties.Resources.Delete_32;
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorActionGetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow != null )
					{
						object[] selectedObjects = context.ObjectsInFocus.Objects;
						if( selectedObjects.Length != 0 && Array.TrueForAll( selectedObjects, obj => obj is MeshInSpace ) )
						{
							context.Enabled = Array.Exists( selectedObjects, delegate ( object obj )
							{
								var c = ( (Component)obj ).GetComponent( bodyName );
								if( c != null )
								{
									if( c is RigidBody )
										return true;
									if( c is RigidBody2D )
										return true;
								}
								return false;
							} );
						}
					}
				};

				a.Click += delegate ( EditorActionClickContext context )
				{
					var text = string.Format( Translate( "Delete \'{0}\'?" ), bodyName );
					if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
					{
						List<UndoSystem.Action> undoActions = new List<UndoSystem.Action>();

						foreach( MeshInSpace meshInSpace in context.ObjectsInFocus.Objects )
						{
							Component body = null;
							{
								var c = meshInSpace.GetComponent( bodyName );
								if( c != null && ( c is RigidBody || c is RigidBody2D ) )
									body = c;
							}

							if( body != null )
							{
								var restoreValue = meshInSpace.Transform;

								undoActions.Add( new UndoActionComponentCreateDelete( context.ObjectsInFocus.DocumentWindow.Document, new Component[] { body }, false ) );

								//change Transform
								if( meshInSpace.Transform.GetByReference == string.Format( "this:${0}\\Transform", bodyName ) )
								{
									//undo action
									var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:Transform" );
									var undoItem = new UndoActionPropertiesChange.Item( meshInSpace, property, restoreValue, new object[ 0 ] );
									undoActions.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );

									//reset reference
									meshInSpace.Transform = restoreValue.Value;
								}
							}
						}

						if( undoActions.Count != 0 )
						{
							context.ObjectsInFocus.DocumentWindow.Document.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
							context.ObjectsInFocus.DocumentWindow.Document.Modified = true;
							ScreenNotifications2.Show( Translate( "The collision was deleted." ) );
						}
					}
				};

				EditorActions.Register( a );
			}

		}
	}
}

#endif