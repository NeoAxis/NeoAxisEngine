// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class to work with <see cref="Component_ObjectInSpace"/>.
	/// </summary>
	public static class Component_ObjectInSpace_Utility
	{
		/// <summary>
		/// Get object to transform. find by reference. must be child of selected object.
		/// </summary>
		/// <param name="selectedObject"></param>
		/// <returns></returns>
		public static Component_ObjectInSpace CalculateObjectToTransform( Component_ObjectInSpace selectedObject )
		{
			Component_ObjectInSpace objectToTransform = null;

			//get object to transform. find by reference. must be child of selected object.
			//Component_TransformOffset specific

			Component current = selectedObject;
			while( current != null )
			{
				//check outside parent
				{
					bool inside = current == selectedObject || ComponentUtility.IsChildInHierarchy( selectedObject, current );
					if( !inside )
						break;
				}

				//_ObjectInSpace
				var objectInSpace = current as Component_ObjectInSpace;
				if( objectInSpace != null )
				{
					//check found result
					if( !objectInSpace.Transform.ReferenceSpecified )
					{
						//found
						objectToTransform = objectInSpace;
						break;
					}

					objectInSpace.Transform.GetMember( current, out var outObject, out var outMember );
					current = outObject as Component;
					continue;
				}

				//_TransformOffset
				var transformOffset = current as Component_TransformOffset;
				if( transformOffset != null )
				{
					if( transformOffset.Source.ReferenceSpecified )
					{
						transformOffset.Source.GetMember( current, out var outObject, out var outMember );
						current = outObject as Component;
						continue;
					}
				}

				//stop
				break;
			}

			return objectToTransform;
		}

		public static Component_TransformOffset Attach( Component_ObjectInSpace attachTo, Component_ObjectInSpace objectToAttach, DocumentInstance documentforUndoRedo = null, UndoMultiAction undoMultiAction = null )
		{
			var objectToTransform = CalculateObjectToTransform( objectToAttach );
			if( objectToTransform != null )
				objectToAttach = objectToTransform;

			//create _TransformOffset
			Component_TransformOffset transformOffset;
			{
				const string transformOffsetName = "Attach Transform Offset";

				transformOffset = objectToAttach.CreateComponent<Component_TransformOffset>( enabled: false );
				transformOffset.Name = transformOffsetName;
				transformOffset.Source = ReferenceUtility.MakeReference<Transform>( null, ReferenceUtility.CalculateThisReference( transformOffset, attachTo, "Transform" ) );

				try
				{
					var f = attachTo.Transform.Value;
					var s = objectToAttach.Transform.Value;
					var offset = f.ToMatrix4().GetInverse() * s.ToMatrix4();
					offset.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl );

					transformOffset.PositionOffset = pos;
					transformOffset.RotationOffset = rot;
					transformOffset.ScaleOffset = scl;
					//transformOffset.Matrix = offset;

					//var offset = new Mat4( s.Rotation.ToMat3(), s.Position ) * new Mat4( f.Rotation.ToMat3(), f.Position ).GetInverse();
					//var f = first.Transform.Value;
					//var s = second.Transform.Value;
					//var offset = new Mat4( s.Rotation.ToMat3(), s.Position ) * new Mat4( f.Rotation.ToMat3(), f.Position ).GetInverse();
					////var offset = second.Transform.Value.ToMat4() * first.Transform.Value.ToMat4().GetInverse();
					//offset.Decompose( out Vec3 pos, out Quat rot, out Vec3 scl );

					//transformOffset.PositionOffset = pos / f.Scale;// / s.Scale;
					//transformOffset.RotationOffset = rot;

					//transformOffset.ScaleOffset = s.Scale / f.Scale;
					////transformOffset.ScaleOffset = scl;

				}
				catch { }

				transformOffset.Enabled = true;

				if( undoMultiAction != null )
					undoMultiAction.AddAction( new UndoActionComponentCreateDelete( documentforUndoRedo, new Component[] { transformOffset }, true ) );
			}

			//change Transform
			{
				//undo action
				if( undoMultiAction != null )
				{
					var property = (Metadata.Property)objectToAttach.MetadataGetMemberBySignature( "property:Transform" );
					var undoItem = new UndoActionPropertiesChange.Item( objectToAttach, property, objectToAttach.Transform, new object[ 0 ] );
					undoMultiAction.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );
				}

				//configure reference
				objectToAttach.Transform = ReferenceUtility.MakeReference<Transform>( null, ReferenceUtility.CalculateThisReference( objectToAttach, transformOffset, "Result" ) );
			}

			return transformOffset;
		}

		public static Component_ObjectInSpace FindObjectToDetach( Component_ObjectInSpace selectedObject )
		{
			const string transformOffsetName = "Attach Transform Offset";

			Component_ObjectInSpace objectToDetach = null;

			//get object to transform. find by reference. must be child of selected object.
			//Component_TransformOffset specific

			Component current = selectedObject;
			while( current != null )
			{
				//check outside parent
				{
					bool inside = current == selectedObject || ComponentUtility.IsChildInHierarchy( selectedObject, current );
					if( !inside )
						break;
				}

				//_ObjectInSpace
				var objectInSpace = current as Component_ObjectInSpace;
				if( objectInSpace != null )
				{
					//check found result
					if( objectInSpace.GetComponent( transformOffsetName ) as Component_TransformOffset != null )
					{
						//found
						objectToDetach = objectInSpace;
						break;
					}
					//check not found result
					if( !objectInSpace.Transform.ReferenceSpecified )
					{
						//not found
						break;
					}

					objectInSpace.Transform.GetMember( current, out var outObject, out var outMember );
					current = outObject as Component;
					continue;
				}

				//_TransformOffset
				var transformOffset = current as Component_TransformOffset;
				if( transformOffset != null )
				{
					if( transformOffset.Source.ReferenceSpecified )
					{
						transformOffset.Source.GetMember( current, out var outObject, out var outMember );
						current = outObject as Component;
						continue;
					}
				}

				//stop
				break;
			}

			return objectToDetach;
		}

		public static void Detach( Component_ObjectInSpace objectToDetach, DocumentInstance documentforUndoRedo = null, UndoMultiAction undoMultiAction = null )
		{
			const string transformOffsetName = "Attach Transform Offset";

			var objectInSpace = objectToDetach;
			//var objectToTransform = CalculateObjectToTransform( objectInSpace );
			//if( objectToTransform != null )
			//	objectInSpace = objectToTransform;

			var transformOffset = objectInSpace.GetComponent( transformOffsetName ) as Component_TransformOffset;
			if( transformOffset != null )
			{
				//change Transform
				{
					//undo action
					if( undoMultiAction != null )
					{
						var property = (Metadata.Property)objectInSpace.MetadataGetMemberBySignature( "property:Transform" );
						var undoItem = new UndoActionPropertiesChange.Item( objectInSpace, property, objectInSpace.Transform, new object[ 0 ] );
						undoMultiAction.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } ) );
					}

					//remove reference
					objectInSpace.Transform = new Reference<Transform>( objectInSpace.Transform, "" );
				}

				//delete
				if( undoMultiAction != null )
					undoMultiAction.AddAction( new UndoActionComponentCreateDelete( documentforUndoRedo, new Component[] { transformOffset }, false ) );
				else
					transformOffset.Dispose();
			}
		}

	}
}
