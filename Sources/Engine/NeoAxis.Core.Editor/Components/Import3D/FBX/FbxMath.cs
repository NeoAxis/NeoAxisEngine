// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using Internal.Fbx;

namespace NeoAxis.Import.FBX
{
	static class FbxMath
	{
		public static void ConvertBoneAssignment( ref BoneAssignment boneAssignment, out Vector4I indices, out Vector4F weights )
		{
			indices = new Vector4I( -1, -1, -1, -1 );
			weights = new Vector4F();
			if( 0 < boneAssignment.count )
			{
				indices.X = boneAssignment.boneIndex0;
				weights.X = (float)boneAssignment.weight0;
			}
			if( 1 < boneAssignment.count )
			{
				indices.Y = boneAssignment.boneIndex1;
				weights.Y = (float)boneAssignment.weight1;
			}
			if( 2 < boneAssignment.count )
			{
				indices.Z = boneAssignment.boneIndex2;
				weights.Z = (float)boneAssignment.weight2;
			}
			if( 3 < boneAssignment.count )
			{
				indices.W = boneAssignment.boneIndex3;
				weights.W = (float)boneAssignment.weight3;
			}
		}

		public static bool IsSpecialFloat( float value )
		{
			return Single.IsInfinity( value ) || Single.IsNaN( value );
		}

		public static float ComputePositionEpsilon( VertexInfo[] vertices )
		{
			const float epsilon = 1e-4f;
			// calculate the position bounds so we have a reliable epsilon to check position differences against
			Vector3F maxVec = new Vector3F( -1e10f, -1e10f, -1e10f );
			Vector3F minVec = new Vector3F( 1e10f, 1e10f, 1e10f );
			for( int i = 1; i < vertices.Length; i++ )
			{
				minVec = Vector3F.Min( minVec, vertices[ i ].Vertex.Position );
				maxVec = Vector3F.Max( maxVec, vertices[ i ].Vertex.Position );
			}
			return ( maxVec - minVec ).Length() * epsilon;
		}


		//get the global position of a node for a given time in the current animation stack.
		//From : GetPosition.cxx (FBX SDK samples):
		// Get the global position of the node for the current pose.
		// If the specified node is not part of the pose or no pose is specified, get its
		// global position at the current time.
		/*
		public static FbxAMatrix GetGlobalPosition(FbxNode node, FbxTime time, FbxPose pose, FbxAMatrix pParentGlobalPosition = null)
		{
			FbxAMatrix globalPosition = null;

			if (pose != null)
			{
				int lNodeIndex = pose.Find(node);

				if (lNodeIndex > -1)
				{
					// The bind pose is always a global matrix.
					// If we have a rest pose, we need to check if it is
					// stored in global or local space.
					if (pose.IsBindPose() || !pose.IsLocalMatrix(lNodeIndex))
					{
						globalPosition = GetPoseMatrix(pose, lNodeIndex);
					}
					else
					{
						// We have a local matrix, we need to convert it to
						// a global space matrix.
						FbxAMatrix lParentGlobalPosition;

						if (pParentGlobalPosition != null)
						{
							lParentGlobalPosition = pParentGlobalPosition;
						}
						else
						{
							if (node.GetParent() != null)
							{
								lParentGlobalPosition = GetGlobalPosition(node.GetParent(), time, pose);
							}
							else
							{
								lParentGlobalPosition = new FbxAMatrix();
								lParentGlobalPosition.SetIdentity();
							}
						}

						FbxAMatrix localPosition = GetPoseMatrix(pose, lNodeIndex);
						globalPosition = lParentGlobalPosition.mul(localPosition);
					}

					return globalPosition;
				}
			}

			// ToDo : ??? Можно ли заменить node.EvaluateGlobalTransform, на ручную реализацию для наследования RrSs, Rrs ?
			// There is no pose entry for that node, get the current global position instead.
			// Ideally this would use parent global position and local position to compute the global position.
			// Unfortunately the equation 
			//    lGlobalPosition = pParentGlobalPosition * lLocalPosition
			// does not hold when inheritance type is other than "Parent" (RSrs).
			// To compute the parent rotation and scaling is tricky in the RrSs and Rrs cases.
			globalPosition = node.EvaluateGlobalTransform(time);

			return globalPosition;
		}
		*/

		public static void Swap<T>( ref T val1, ref T val2 ) where T : struct
		{
			T temp = val1;
			val1 = val2;
			val2 = temp;
		}

		public static bool IsNotEmptyVector( Vector3F vec )
		{
			return Vector3F.AnyNonZero( vec ) && !IsSpecialFloat( vec.X ) && !IsSpecialFloat( vec.Y ) && !IsSpecialFloat( vec.Z );
		}

		public static bool IsNotEmptyVector( Vector4F vec )
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			return !IsSpecialFloat( vec.X ) && !IsSpecialFloat( vec.Y ) && !IsSpecialFloat( vec.Z ) && ( vec.X != 0 || vec.Y != 0 || vec.Z != 0 );
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		//Get the geometry offset to a node. It is never inherited by the children.
		//The geometric offset is a local transformation that is applied to a node attribute only.
		//This transformation is applied to the node attribute after the node transformations. This transformation is not inherited across the node hierarchy.
		public static FbxAMatrix GetGeometryOffset( FbxNode pNode )
		{
			FbxVector4 lT = pNode.GetGeometricTranslation( FbxNode.EPivotSet.eSourcePivot );
			FbxVector4 lR = pNode.GetGeometricRotation( FbxNode.EPivotSet.eSourcePivot );
			FbxVector4 lS = pNode.GetGeometricScaling( FbxNode.EPivotSet.eSourcePivot );

			return new FbxAMatrix( lT, lR, lS );
		}


		// Get the matrix of the given pose
		static FbxAMatrix GetPoseMatrix( FbxPose pPose, int pNodeIndex )
		{
			FbxMatrix lMatrix = pPose.GetMatrix( pNodeIndex );

			FbxAMatrix lPoseMatrix = new FbxAMatrix();
			//memcpy((double*)lPoseMatrix, (double*)lMatrix, sizeof(lMatrix.mData));
			lPoseMatrix.SetRow( 0, lMatrix.GetRow( 0 ) );
			lPoseMatrix.SetRow( 1, lMatrix.GetRow( 1 ) );
			lPoseMatrix.SetRow( 2, lMatrix.GetRow( 2 ) );
			lPoseMatrix.SetRow( 3, lMatrix.GetRow( 3 ) );
			return lPoseMatrix;
		}

		public static FbxAMatrix EvaluateGlobalTransform( FbxNode node )
		{
			return node.EvaluateGlobalTransform();

			//Warning: Эквивалент node.EvaluateGlobalTransform(). Но обнаружилось отличие на модели BaseMesh_Anim.fbx, эта модель в MaxAxisSystem не происходит конверсия. 
			// На этой модели в контексте использования в Skeleton работает корректно. А в контексте использования координат Mesh не корректно: модель повернута на 90 градусов.
			// Видимо еще что-то надо учитывать кроме AxisSystem. Либо в CalculateGlobalTransform AxisSystem учитывается не полностью.
			// Сейчас в этой функции нет необходимости, достаточно node.EvaluateGlobalTransform() т.к. только в момент импорта все вычисляется.
			//return CalculateGlobalTransform(node);
		}

		//Calculates global transform of the node.
		//This function was taken from FBX SDK/Smaples/Transformations. Originaly it did not take into account AxisSystem, so the corrections to use AxisSystem was added.
		//Has the fbx sdk equivalent pNode.EvaluateGlobalTransform();
		//
		//Note: FBX Documentation for ConvertScene states - "The adjustment will affect the translation animation curves and the objects pivots values 
		//(the rotation transformation is applied as a pre-rotation transform therefore the rotation animation curves do not need to be transformed)"
		//But ConvertScene, does not realy change node.PreRotation, these changes are in the matrix : node.GetScene().GetGlobalSettings().GetAxisSystem().GetMatrix
		//
		/*
		Terminology:
		Suffix "M" means this is a matrix, suffix "V" means it is a vector.
		T is translation.
		R is rotation.
		S is scaling.
		SH is shear.
		GlobalRM(x) means the Global Rotation Matrix of node "x".
		GlobalRM(P(x)) means the Global Rotation Matrix of the parent node of node "x".
		All other transforms are described in the similar way.

		The algorithm description:
		To calculate global transform of a node x according to different InheritType, 
		we need to calculate GlobalTM(x) and [GlobalRM(x) * (GlobalSHM(x) * GlobalSM(x))] separately.
		GlobalM(x) = GlobalTM(x) * [GlobalRM(x) * (GlobalSHM(x) * GlobalSM(x))];

		InhereitType = RrSs:
		GlobalRM(x) * (GlobalSHM(x) * GlobalSM(x)) = GlobalRM(P(x)) * LocalRM(x) * [GlobalSHM(P(x)) * GlobalSM(P(x))] * LocalSM(x);

		InhereitType = RSrs:
		GlobalRM(x) * (GlobalSHM(x) * GlobalSM(x)) = GlobalRM(P(x)) * [GlobalSHM(P(x)) * GlobalSM(P(x))] * LocalRM(x) * LocalSM(x);

		InhereitType = Rrs:
		GlobalRM(x) * (GlobalSHM(x) * GlobalSM(x)) = GlobalRM(P(x)) * LocalRM(x) * LocalSM(x);

		LocalM(x)= TM(x) * RoffsetM(x)  * RpivotM(x) * RpreM(x) * RM(x) * RpostM(x) * RpivotM(x)^-1 * SoffsetM(x) *SpivotM(x) * SM(x) * SpivotM(x)^-1
		LocalTWithAllPivotAndOffsetInformationV(x) = Local(x).GetT();
		GlobalTV(x) = GlobalM(P(x)) * LocalTWithAllPivotAndOffsetInformationV(x);

		Notice: FBX SDK does not support shear yet, so all local transform won't have shear.
		However, global transform might bring in shear by combine the global transform of node in higher hierarchy.
		For example, if you scale the parent by a non-uniform scale and then rotate the child node, then a shear will
		be generated on the child node's global transform.
		In this case, we always compensates shear and store it in the scale matrix too according to following formula:
		Shear*Scaling = RotationMatrix.Inverse * TranslationMatrix.Inverse * WholeTranformMatrix
		*/
		// ReSharper disable InconsistentNaming
		static FbxAMatrix CalculateGlobalTransform( FbxNode node )
		{
			if( node == null )
			{
				var ret = new FbxAMatrix();
				ret.SetIdentity();
				return ret;
			}
			var lTranlationM = new FbxAMatrix();
			var lScalingM = new FbxAMatrix();
			var lScalingPivotM = new FbxAMatrix();
			var lScalingOffsetM = new FbxAMatrix();
			var lRotationOffsetM = new FbxAMatrix();
			var lRotationPivotM = new FbxAMatrix();
			var lPreRotationM = new FbxAMatrix();
			var lRotationM = new FbxAMatrix();
			var lPostRotationM = new FbxAMatrix();

			FbxAMatrix lParentGX = new FbxAMatrix();
			FbxAMatrix lGlobalT = new FbxAMatrix();
			FbxAMatrix lGlobalRS = new FbxAMatrix();

			// Construct translation matrix
			FbxVector4 lTranslation = new FbxVector4( node.LclTranslation.Get() ); //The fourth component of this object is assigned 1.
			lTranlationM.SetT( lTranslation );

			// Construct rotation matrices
			FbxVector4 lRotation = new FbxVector4( node.LclRotation.Get() );
			FbxVector4 lPreRotation = new FbxVector4( node.PreRotation.Get() );
			FbxVector4 lPostRotation = new FbxVector4( node.PostRotation.Get() );
			lRotationM.SetR( lRotation );
			lPreRotationM.SetR( lPreRotation );
			lPostRotationM.SetR( lPostRotation );

			// Construct scaling matrix
			FbxVector4 lScaling = new FbxVector4( node.LclScaling.Get() );
			lScalingM.SetS( lScaling );

			// Construct offset and pivot matrices
			FbxVector4 lScalingOffset = new FbxVector4( node.ScalingOffset.Get() );
			FbxVector4 lScalingPivot = new FbxVector4( node.ScalingPivot.Get() );
			FbxVector4 lRotationOffset = new FbxVector4( node.RotationOffset.Get() );
			FbxVector4 lRotationPivot = new FbxVector4( node.RotationPivot.Get() );
			lScalingOffsetM.SetT( lScalingOffset );
			lScalingPivotM.SetT( lScalingPivot );
			lRotationOffsetM.SetT( lRotationOffset );
			lRotationPivotM.SetT( lRotationPivot );

			// Calculate the global transform matrix of the parent node
			FbxNode lParentNode = node.GetParent();
			if( lParentNode != null )
			{
				//Children of the root node must take into account the axis matrix.
				//Warning: this function CalculateGlobalTransform was taken from FBX SDK/Smaples/Transformations. Originaly it did not take into account AxisSystem
				if( lParentNode.GetParent() == null )
				{
					FbxAMatrix axisMarix = new FbxAMatrix();
					node.GetScene().GetGlobalSettings().GetAxisSystem().GetMatrix( axisMarix );
					lPreRotationM = axisMarix.mul( lPreRotationM );
				}

				lParentGX = CalculateGlobalTransform( lParentNode );
			}
			else
			{
				lParentGX.SetIdentity();
			}

			//Construct Global Rotation
			FbxAMatrix lParentGRM = new FbxAMatrix();
			FbxVector4 lParentGR = lParentGX.GetR();
			lParentGRM.SetR( lParentGR );
			var lLRM = lPreRotationM.mul( lRotationM ).mul( lPostRotationM );

			//Construct Global Shear*Scaling
			//FBX SDK does not support shear, to patch this, we use:
			//Shear*Scaling = RotationMatrix.Inverse * TranslationMatrix.Inverse * WholeTranformMatrix
			FbxAMatrix lParentTM = new FbxAMatrix();
			FbxVector4 lParentGT = lParentGX.GetT();
			lParentTM.SetT( lParentGT );
			var lParentGRSM = lParentTM.Inverse().mul( lParentGX );
			var lParentGSM = lParentGRM.Inverse().mul( lParentGRSM );
			var lLSM = lScalingM;


			//Do not consider translation now
			FbxTransform.EInheritType lInheritType = node.InheritType.Get();
			if( lInheritType == FbxTransform.EInheritType.eInheritRrSs )
			{
				lGlobalRS = lParentGRM.mul( lLRM ).mul( lParentGSM ).mul( lLSM );
			}
			else if( lInheritType == FbxTransform.EInheritType.eInheritRSrs )
			{
				lGlobalRS = lParentGRM.mul( lParentGSM ).mul( lLRM ).mul( lLSM );
			}
			else if( lInheritType == FbxTransform.EInheritType.eInheritRrs )
			{
				if( lParentNode != null )
				{
					FbxAMatrix lParentLSM = new FbxAMatrix();
					FbxVector4 lParentLS = new FbxVector4( lParentNode.LclScaling.Get() );
					lParentLSM.SetS( lParentLS );
					FbxAMatrix lParentGSM_noLocal = lParentGSM.mul( lParentLSM.Inverse() );
					lGlobalRS = lParentGRM.mul( lLRM ).mul( lParentGSM_noLocal ).mul( lLSM );
				}
				else
				{
					lGlobalRS = lParentGRM.mul( lLRM ).mul( lLSM );
				}
			}
			else
			{
				FbxImportLog.LogError( node, "error, unknown inherit type!" );
			}

			// Construct translation matrix
			// Calculate the local transform matrix
			var lTransform = lTranlationM.mul( lRotationOffsetM ).mul( lRotationPivotM ).mul( lPreRotationM ).mul( lRotationM ).mul( lPostRotationM )
				.mul( lRotationPivotM.Inverse() ).mul( lScalingOffsetM ).mul( lScalingPivotM ).mul( lScalingM ).mul( lScalingPivotM.Inverse() );
			FbxVector4 lLocalTWithAllPivotAndOffsetInfo = lTransform.GetT();
			// Calculate global translation vector according to: 
			// GlobalTranslation = ParentGlobalTransform * LocalTranslationWithPivotAndOffsetInfo
			FbxVector4 lGlobalTranslation = lParentGX.MultT( lLocalTWithAllPivotAndOffsetInfo );
			lGlobalT.SetT( lGlobalTranslation );

			//Construct the whole global transform
			lTransform = lGlobalT.mul( lGlobalRS );

			return lTransform;
		}
		// ReSharper restore InconsistentNaming

	}
}
