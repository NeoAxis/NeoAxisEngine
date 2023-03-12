#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	class ObjectInSpaceEditorExtensions : EditorExtensions
	{
		string Translate( string text )
		{
			return EditorLocalization.Translate( "ObjectInSpace", text );
		}

		public override void OnRegister()
		{
			//Attach second selected to first
			{
				const string transformOffsetName = "Attach Transform Offset";

				var a = new EditorAction();
				a.Name = "Attach Second to First";
				a.Description = "Attaches the second, third and next selected objects to the first selected object.";
				a.ImageSmall = Properties.Resources.Attach_16;
				a.ImageBig = Properties.Resources.Attach_32;
				a.RibbonText = ("Attach", "");

				//!!!!выключить где-то?
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow != null )
					{
						object[] selectedObjects = context.ObjectsInFocus.Objects;
						if( selectedObjects.Length > 1 )
						{
							var first = selectedObjects[ 0 ] as ObjectInSpace;
							if( first != null )
							{
								for( int n = 1; n < selectedObjects.Length; n++ )
								{
									var second = selectedObjects[ n ] as ObjectInSpace;
									if( second != null )
									{
										var objectToTransform = ObjectInSpaceUtility.CalculateObjectToTransform( second );
										if( objectToTransform != null )
											second = objectToTransform;

										//!!!!проверять? second.GetComponentByName( transformOffsetName ) as TransformOffset == null 
										if( !second.Transform.ReferenceSpecified && second.GetComponent( transformOffsetName ) as TransformOffset == null )
										{
											context.Enabled = true;
										}
									}

									////!!!!проверять? second.GetComponentByName( transformOffsetName ) as TransformOffset == null 
									//if( second != null && !second.Transform.ReferenceSpecified && second.GetComponent( transformOffsetName ) as TransformOffset == null )
									//{
									//	context.Enabled = true;
									//}
								}
							}
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					object[] selectedObjects = context.ObjectsInFocus.Objects;

					var undoMultiAction = new UndoMultiAction();

					var first = selectedObjects[ 0 ] as ObjectInSpace;
					for( int n = 1; n < selectedObjects.Length; n++ )
					{
						var second = selectedObjects[ n ] as ObjectInSpace;
						if( second != null )
							ObjectInSpaceUtility.Attach( first, second, TransformOffset.ModeEnum.Elements, context.ObjectsInFocus.DocumentWindow.Document, undoMultiAction );
					}

					context.ObjectsInFocus.DocumentWindow.Document.UndoSystem.CommitAction( undoMultiAction );
					context.ObjectsInFocus.DocumentWindow.Document.Modified = true;
					ScreenNotifications.Show( Translate( "The object was attached to another object." ) );

					//var undoActions = new List<UndoSystem.Action>();

					//var first = selectedObjects[ 0 ] as ObjectInSpace;
					//for( int n = 1; n < selectedObjects.Length; n++ )
					//{
					//	var second = selectedObjects[ n ] as ObjectInSpace;
					//	if( second != null )
					//	{
					//		var objectToTransform = ObjectInSpace_Utility.CalculateObjectToTransform( second );
					//		if( objectToTransform != null )
					//			second = objectToTransform;

					//		//create _TransformOffset
					//		TransformOffset transformOffset;
					//		{
					//			transformOffset = second.CreateComponent<TransformOffset>( -1, false );
					//			transformOffset.Name = transformOffsetName;
					//			transformOffset.Source = ReferenceUtility.MakeReference<Transform>( null, ReferenceUtility.CalculateThisReference( transformOffset, first, "Transform" ) );

					//			try
					//			{
					//				var f = first.Transform.Value;
					//				var s = second.Transform.Value;
					//				var offset = f.ToMatrix4().GetInverse() * s.ToMatrix4();
					//				offset.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl );

					//				transformOffset.PositionOffset = pos;
					//				transformOffset.RotationOffset = rot;
					//				transformOffset.ScaleOffset = scl;
					//				//transformOffset.Matrix = offset;

					//				//var offset = new Mat4( s.Rotation.ToMat3(), s.Position ) * new Mat4( f.Rotation.ToMat3(), f.Position ).GetInverse();
					//				//var f = first.Transform.Value;
					//				//var s = second.Transform.Value;
					//				//var offset = new Mat4( s.Rotation.ToMat3(), s.Position ) * new Mat4( f.Rotation.ToMat3(), f.Position ).GetInverse();
					//				////var offset = second.Transform.Value.ToMat4() * first.Transform.Value.ToMat4().GetInverse();
					//				//offset.Decompose( out Vec3 pos, out Quat rot, out Vec3 scl );

					//				//transformOffset.PositionOffset = pos / f.Scale;// / s.Scale;
					//				//transformOffset.RotationOffset = rot;

					//				//transformOffset.ScaleOffset = s.Scale / f.Scale;
					//				////transformOffset.ScaleOffset = scl;

					//			}
					//			catch { }

					//			transformOffset.Enabled = true;

					//			undoActions.Add( new UndoActionComponentCreateDelete( context.ObjectsInFocus.DocumentWindow.Document, new Component[] { transformOffset }, true ) );
					//		}

					//		//change Transform
					//		{
					//			//undo action
					//			var property = (Metadata.Property)second.MetadataGetMemberBySignature( "property:Transform" );
					//			var undoItem = new UndoActionPropertiesChange.Item( second, property, second.Transform, new object[ 0 ] );
					//			undoActions.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );

					//			//configure reference
					//			second.Transform = ReferenceUtility.MakeReference<Transform>( null, ReferenceUtility.CalculateThisReference( second, transformOffset, "Result" ) );
					//		}
					//	}
					//}

					//context.ObjectsInFocus.DocumentWindow.Document.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
					//context.ObjectsInFocus.DocumentWindow.Document.Modified = true;
					//ScreenNotifications.Show( "The object was attached to another object." );
				};

				EditorActions.Register( a );
			}

			//Detach from Another Object
			{
				//const string transformOffsetName = "Attach Transform Offset";

				var a = new EditorAction();
				a.Name = "Detach from Another Object";
				a.Description = "Detaches selected objects from another object.";
				a.ImageSmall = Properties.Resources.Detach_16;
				a.ImageBig = Properties.Resources.Detach_32;
				a.RibbonText = ("Detach", "");

				//!!!!выключить где-то?
				a.QatSupport = true;
				//a.qatAddByDefault = true;
				a.ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.Document;

				a.GetState += delegate ( EditorAction.GetStateContext context )
				{
					if( context.ObjectsInFocus.DocumentWindow != null )
					{
						object[] selectedObjects = context.ObjectsInFocus.Objects;
						if( selectedObjects.Length != 0 && Array.TrueForAll( selectedObjects, obj => obj is ObjectInSpace ) )
						{
							foreach( ObjectInSpace objectInSpace in selectedObjects )
							{
								var objectToDetach = ObjectInSpaceUtility.FindObjectToDetach( objectInSpace );
								if( objectToDetach != null )
								{
									context.Enabled = true;
									break;
								}

								//var objectInSpace = objectInSpace2;
								//var objectToTransform = ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
								//if( objectToTransform != null )
								//	objectInSpace = objectToTransform;

								//if( objectInSpace.GetComponent( transformOffsetName ) as TransformOffset != null )
								//{
								//	context.Enabled = true;
								//	break;
								//}
							}

							//context.Enabled = Array.Exists( selectedObjects,
							//	obj => ( (Component)obj ).GetComponent( transformOffsetName ) as TransformOffset != null );
						}
					}
				};

				a.Click += delegate ( EditorAction.ClickContext context )
				{
					if( EditorMessageBox.ShowQuestion( Translate( "Detach from another object?" ), EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
					{
						var undoMultiAction = new UndoMultiAction();

						foreach( ObjectInSpace objectInSpace in context.ObjectsInFocus.Objects )
						{
							var objectToDetach = ObjectInSpaceUtility.FindObjectToDetach( objectInSpace );
							if( objectToDetach != null )
								ObjectInSpaceUtility.Detach( objectToDetach, context.ObjectsInFocus.DocumentWindow.Document, undoMultiAction );
						}

						if( undoMultiAction.Actions.Count != 0 )
						{
							context.ObjectsInFocus.DocumentWindow.Document.UndoSystem.CommitAction( undoMultiAction );
							context.ObjectsInFocus.DocumentWindow.Document.Modified = true;
							ScreenNotifications.Show( Translate( "The object was detached from another object." ) );
						}


						//var undoActions = new List<UndoSystem.Action>();

						//foreach( ObjectInSpace objectInSpace2 in context.ObjectsInFocus.Objects )
						//{
						//	var objectInSpace = objectInSpace2;
						//	var objectToTransform = ObjectInSpace_Utility.CalculateObjectToTransform( objectInSpace );
						//	if( objectToTransform != null )
						//		objectInSpace = objectToTransform;

						//	var transformOffset = objectInSpace.GetComponent( transformOffsetName ) as TransformOffset;
						//	if( transformOffset != null )
						//	{
						//		//change Transform
						//		{
						//			//undo action
						//			var property = (Metadata.Property)objectInSpace.MetadataGetMemberBySignature( "property:Transform" );
						//			var undoItem = new UndoActionPropertiesChange.Item( objectInSpace, property, objectInSpace.Transform, new object[ 0 ] );
						//			undoActions.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );

						//			//remove reference
						//			objectInSpace.Transform = new Reference<Transform>( objectInSpace.Transform, "" );
						//		}

						//		//delete
						//		undoActions.Add( new UndoActionComponentCreateDelete( context.ObjectsInFocus.DocumentWindow.Document, new Component[] { transformOffset }, false ) );
						//	}
						//}

						//if( undoActions.Count != 0 )
						//{
						//	context.ObjectsInFocus.DocumentWindow.Document.UndoSystem.CommitAction( new UndoMultiAction( undoActions ) );
						//	context.ObjectsInFocus.DocumentWindow.Document.Modified = true;
						//	ScreenNotifications.Show( "The object was detached from another object." );
						//}
					}
				};

				EditorActions.Register( a );
			}

		}
	}
}

#endif