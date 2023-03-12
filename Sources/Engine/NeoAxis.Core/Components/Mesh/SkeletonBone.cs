// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Represents a bone of a mesh skeleton, how it is positioned in space.
	/// </summary>
	public class SkeletonBone : Component
	{
		Skeleton getCachedBoneIndex_Skeleton;
		int getCachedBoneIndex_Index;

		bool transformMatrixInverseHasValue;
		Matrix4F transformMatrixInverseValue;

		/////////////////////////////////////////

		//!!!!TransformF?
		//!!!!name: InitialTransform?
		/// <summary>
		/// The position, rotation and scale of the bone.
		/// </summary>
		[Serialize]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> Transform
		{
			get { if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value; }
			set
			{
				if( _transform.BeginSet( ref value ) )
				{
					try
					{
						TransformChanged?.Invoke( this );
						transformMatrixInverseHasValue = false;
					}
					finally { _transform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<SkeletonBone> TransformChanged;
		ReferenceField<Transform> _transform = NeoAxis.Transform.Identity;

		/////////////////////////////////////////

		public int GetCachedBoneIndex( Skeleton skeleton )
		{
			//!!!!threading

			if( getCachedBoneIndex_Skeleton != skeleton )
			{
				getCachedBoneIndex_Skeleton = skeleton;
				getCachedBoneIndex_Index = Array.IndexOf( skeleton.GetBones(), this );
			}
			return getCachedBoneIndex_Index;
		}

		public ref Matrix4F GetTransformMatrixInverse()
		{
			if( !transformMatrixInverseHasValue )
			{
				transformMatrixInverseValue = Transform.Value.ToMatrix4().GetInverse().ToMatrix4F();
				transformMatrixInverseHasValue = true;
			}
			return ref transformMatrixInverseValue;
		}

		//[Browsable( false )]
		//public Transform TransformV
		//{
		//	get { return Transform.Value; }
		//	set { Transform = value; }
		//}




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

		////!!!!так ли?
		////OffsetMatrix
		//[Serialize]
		//public Reference<Mat4F> OffsetMatrix
		//{
		//	get { if( _offsetMatrix.BeginGet() ) OffsetMatrix = _offsetMatrix.Get( this ); return _offsetMatrix.value; }
		//	set { if( _offsetMatrix.BeginSet( ref value ) ) { try { OffsetMatrixChanged?.Invoke( this ); } finally { _offsetMatrix.EndSet(); } } }
		//}
		//public event Action<Bone> OffsetMatrixChanged;
		//ReferenceField<Mat4F> _offsetMatrix;

		//!!!!надо ли
		//[Browsable( false )]
		//public Bone ParentNode
		//{
		//	get { return Parent as Bone; }
		//}

		//[Serialize]
		//public Reference<int> BoneIndex
		//{
		//	get { if( _boneIndex.BeginGet() ) BoneIndex = _boneIndex.Get( this ); return _boneIndex.value; }
		//	set { if( _boneIndex.BeginSet( ref value ) ) { try { BoneIndexChanged?.Invoke( this ); } finally { _boneIndex.EndSet(); } } }
		//}
		//public event Action<Bone> BoneIndexChanged;
		//ReferenceField<int> _boneIndex;

	}
}
