// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Represents a bone of a mesh skeleton, how it is positioned in space.
	/// </summary>
	public class SkeletonBone : Component
	{
		//not used
		//Skeleton getCachedBoneIndex_Skeleton;
		//int getCachedBoneIndex_Index;

		bool transformMatrixInverseHasValue;
		Matrix4F transformMatrixInverseValue;

		//bool transformMatrixInverseHasValueWithoutRotationScale;
		//Matrix4F transformMatrixInverseValueWithoutRotationScale;

		////when animation state == null
		//bool transformMatrixInverseHasValue;
		//Matrix4F transformMatrixInverseValue;

		////when animation state != null
		//bool transformMatrixInverseHasValue2;
		//Matrix4F transformMatrixInverseValue2;

		/////////////////////////////////////////

		/// <summary>
		/// The position, rotation and scale of the bone. By default only position are stored in the property, rotation and scale are applied to key frames.
		/// </summary>
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> Transform
		{
			get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
			set
			{
				if( _transform.BeginSet( this, ref value ) )
				{
					try
					{
						TransformChanged?.Invoke( this );
						transformMatrixInverseHasValue = false;
						//transformMatrixInverseHasValueWithoutRotationScale = false;
						//transformMatrixInverseHasValue = false;
						//transformMatrixInverseHasValue2 = false;
					}
					finally { _transform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<SkeletonBone> TransformChanged;
		ReferenceField<Transform> _transform = NeoAxis.Transform.Identity;

		///// <summary>
		///// The rotation of the bone before normalization.
		///// </summary>
		//[DefaultValue( Quaternion.IdentityAsString )]
		//public Reference<Quaternion> RotationBeforeNormalization
		//{
		//	get { if( _rotationBeforeNormalization.BeginGet() ) RotationBeforeNormalization = _rotationBeforeNormalization.Get( this ); return _rotationBeforeNormalization.value; }
		//	set
		//	{
		//		if( _rotationBeforeNormalization.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				RotationBeforeNormalizationChanged?.Invoke( this );
		//				transformMatrixInverseHasValue = false;
		//				transformMatrixInverseHasValue2 = false;
		//			}
		//			finally { _rotationBeforeNormalization.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="RotationBeforeNormalization"/> property value changes.</summary>
		//public event Action<SkeletonBone> RotationBeforeNormalizationChanged;
		//ReferenceField<Quaternion> _rotationBeforeNormalization = Quaternion.Identity;

		///// <summary>
		///// The scale of the bone before normalization.
		///// </summary>
		//[DefaultValue( Vector3.OneAsString )]
		//public Reference<Vector3> ScaleBeforeNormalization
		//{
		//	get { if( _scaleBeforeNormalization.BeginGet() ) ScaleBeforeNormalization = _scaleBeforeNormalization.Get( this ); return _scaleBeforeNormalization.value; }
		//	set
		//	{
		//		if( _scaleBeforeNormalization.BeginSet( this, ref value ) )
		//		{
		//			try
		//			{
		//				ScaleBeforeNormalizationChanged?.Invoke( this );
		//				transformMatrixInverseHasValue = false;
		//				transformMatrixInverseHasValue2 = false;
		//			}
		//			finally { _scaleBeforeNormalization.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="ScaleBeforeNormalization"/> property value changes.</summary>
		//public event Action<SkeletonBone> ScaleBeforeNormalizationChanged;
		//ReferenceField<Vector3> _scaleBeforeNormalization = Vector3.One;

		/////////////////////////////////////////

		//not used
		//public int GetCachedBoneIndex( Skeleton skeleton )
		//{
		//	threading

		//	if( getCachedBoneIndex_Skeleton != skeleton )
		//	{
		//		getCachedBoneIndex_Skeleton = skeleton;
		//		getCachedBoneIndex_Index = Array.IndexOf( skeleton.GetBones(), this );
		//	}
		//	return getCachedBoneIndex_Index;
		//}

		public ref Matrix4F GetTransformMatrixInverse()// bool skeletonNormalized, bool applyRotationScaleBeforeNormalization )
		{
			//if( skeletonNormalized )
			//{
			//if( applyRotationScaleBeforeNormalization )
			//{

			if( !transformMatrixInverseHasValue )
			{
				TransformV.ToMatrix4().GetInverse( out var inverse );
				inverse.ToMatrix4F( out transformMatrixInverseValue );
				transformMatrixInverseHasValue = true;
			}
			return ref transformMatrixInverseValue;

			//}

			////else
			////{
			////	//don't apply Rotation and Scale
			////	if( !transformMatrixInverseHasValueWithoutRotationScale )
			////	{
			////		var tr = new Transform( TransformV.Position, Quaternion.Identity, Vector3.One );
			////		tr.ToMatrix4().GetInverse( out var inverse );
			////		inverse.ToMatrix4F( out transformMatrixInverseValueWithoutRotationScale );
			////		transformMatrixInverseHasValueWithoutRotationScale = true;
			////	}
			////	return ref transformMatrixInverseValueWithoutRotationScale;
			////}

			//}
			//else
			//{
			//	if( !transformMatrixInverseHasValueWithRotationScale )
			//	{
			//		TransformV.ToMatrix4().GetInverse( out var inverse );
			//		inverse.ToMatrix4F( out transformMatrixInverseValueWithRotationScale );
			//		transformMatrixInverseHasValueWithRotationScale = true;
			//	}
			//	return ref transformMatrixInverseValueWithRotationScale;
			//}


			//if( skeletonNormalized  )
			//{
			//	if( applyRotationScaleBeforeNormalization )
			//	{
			//	}
			//	else
			//	{
			//	}

			//	if( !transformMatrixInverseHasValue2 )
			//	{
			//		var rotationScale = new Transform( Vector3.Zero, RotationBeforeNormalization.Value, ScaleBeforeNormalization.Value );

			//		Matrix4.Multiply( ref TransformV.ToMatrix4(), ref rotationScale.ToMatrix4(), out var transform );

			//		transform.GetInverse( out var inverse );
			//		inverse.ToMatrix4F( out transformMatrixInverseValue2 );
			//		transformMatrixInverseHasValue2 = true;
			//	}
			//	return ref transformMatrixInverseValue2;
			//}
			//else
			//{
			//	if( !transformMatrixInverseHasValue )
			//	{
			//		TransformV.ToMatrix4().GetInverse( out var inverse );
			//		inverse.ToMatrix4F( out transformMatrixInverseValue );
			//		transformMatrixInverseHasValue = true;
			//	}
			//	return ref transformMatrixInverseValue;

			//	//Transform.Value.ToMatrix4().GetInverse( out var inverse );
			//	//inverse.ToMatrix4F( out transformMatrixInverseValue );
			//	//transformMatrixInverseHasValue = true;
			//}

			//if( skeletonNormalized && applyRotationScaleBeforeNormalization )
			//{
			//	if( !transformMatrixInverseHasValue2 )
			//	{
			//		var rotationScale = new Transform( Vector3.Zero, RotationBeforeNormalization.Value, ScaleBeforeNormalization.Value );

			//		Matrix4.Multiply( ref TransformV.ToMatrix4(), ref rotationScale.ToMatrix4(), out var transform );

			//		transform.GetInverse( out var inverse );
			//		inverse.ToMatrix4F( out transformMatrixInverseValue2 );
			//		transformMatrixInverseHasValue2 = true;
			//	}
			//	return ref transformMatrixInverseValue2;
			//}
			//else
			//{
			//	if( !transformMatrixInverseHasValue )
			//	{
			//		TransformV.ToMatrix4().GetInverse( out var inverse );
			//		inverse.ToMatrix4F( out transformMatrixInverseValue );
			//		transformMatrixInverseHasValue = true;
			//	}
			//	return ref transformMatrixInverseValue;

			//	//Transform.Value.ToMatrix4().GetInverse( out var inverse );
			//	//inverse.ToMatrix4F( out transformMatrixInverseValue );
			//	//transformMatrixInverseHasValue = true;
			//}
		}

		[Browsable( false )]
		public Transform TransformV
		{
			get { return Transform.Value; }
			set { Transform = value; }
		}



		///// Animation enable flag.
		//bool animated_;
		///// Supported collision types.
		//unsigned char collisionMask_;
		///// Radius.
		//float radius_;
		///// Local-space bounding box.
		//BoundingBox boundingBox_;
		///// Scene node.
		//WeakPtr<Node> node_;

		//надо ли
		//[Browsable( false )]
		//public Bone ParentNode
		//{
		//	get { return Parent as Bone; }
		//}

		//[Serialize]
		//public Reference<int> BoneIndex
		//{
		//	get { if( _boneIndex.BeginGet() ) BoneIndex = _boneIndex.Get( this ); return _boneIndex.value; }
		//	set { if( _boneIndex.BeginSet( this, ref value ) ) { try { BoneIndexChanged?.Invoke( this ); } finally { _boneIndex.EndSet(); } } }
		//}
		//public event Action<Bone> BoneIndexChanged;
		//ReferenceField<int> _boneIndex;

	}
}
