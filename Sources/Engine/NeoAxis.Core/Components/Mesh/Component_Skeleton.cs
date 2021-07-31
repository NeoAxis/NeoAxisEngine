// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// Represents mesh skeleton as a collection of bones. Used for a mesh animation.
	/// </summary>
	public class Component_Skeleton : Component
	{
		Component_SkeletonBone[] bonesCache;

		/////////////////////////////////////////

		//public enum SkinningModeEnum
		//{
		//	/// <summary>
		//	/// Chooses DualQuaternion if there is no scaling in the bones transformation, otherwise Linear.
		//	/// </summary>
		//	Auto,
		//	Linear,
		//	DualQuaternion
		//}

		///// <summary>
		///// Skin deformation algorithm.
		///// </summary>
		//[DefaultValue( SkinningModeEnum.Auto )]
		//public Reference<SkinningModeEnum> SkinningMode
		//{
		//	get { if( _skinningMode.BeginGet() ) SkinningMode = _skinningMode.Get( this ); return _skinningMode.value; }
		//	set { if( _skinningMode.BeginSet( ref value ) ) { try { SkinningModeChanged?.Invoke( this ); } finally { _skinningMode.EndSet(); } } }
		//}
		//public event Action<Component_Skeleton> SkinningModeChanged;
		//ReferenceField<SkinningModeEnum> _skinningMode = SkinningModeEnum.Auto;

		/////////////////////////////////////////

		public Component_SkeletonBone[] GetBones( bool forceUpdate = false )
		{
			if( bonesCache == null || forceUpdate )
				bonesCache = GetComponents<Component_SkeletonBone>( checkChildren: true );
			return bonesCache;
		}

		//_ResultCompile<Component_Skeleton.CompiledData>

		/////////////////////////////////////////

		//public class CompiledData// : IDisposable
		//{
		//	//cached bones with indexes
		//	//index in the array correspond to Component_Bone.BoneIndex
		//	public Component_SkeletonBone[] bones;
		//}

		/////////////////////////////////////////

		//protected override void OnResultCompile()
		//{
		//	if( Result == null )
		//	{
		//		var compiledData = new CompiledData();
		//		compiledData.bones = GetComponents<Component_SkeletonBone>( false, true );
		//		//compiledData.bones = GetComponents<Component_SkeletonBone>( false, true, true );
		//		Result = compiledData;
		//	}
		//}

		////!!!!надо ли
		//public Component_Bone[] GetAllBones()
		//{
		//	//!!!!slowly

		//	return GetComponents<Component_Bone>( false, true );
		//}

		//public Component_Bone GetBoneByIndex( int index )
		//{
		//	//!!!!slowly

		//	var all = GetAllBones();
		//	if( index >= 0 || index < all.Length )
		//		return all[ index ];
		//	return null;
		//}

		////!!!!надо ли
		//public Component_Animation FindAnimation( string name )
		//{
		//	//!!!!slowly. кешировать
		//	//!!!!если !Enabled, то что тогда. с костями тоже так

		//	return GetComponentByName( name ) as Component_Animation;
		//}

		//public BoneTransformItem[] CalculateBoneTransforms( AnimationStateItem[] optionalAnimationState )
		//{
		//	return null;
		//}
	}
}
