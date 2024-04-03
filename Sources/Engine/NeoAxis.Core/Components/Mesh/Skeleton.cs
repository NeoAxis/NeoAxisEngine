// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents mesh skeleton as a collection of bones. Used for a mesh animation.
	/// </summary>
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.SkeletonEditor" )]
	//[Preview( "NeoAxis.Editor.SkeletonPreview" )]
#endif
	public class Skeleton : Component
	{
		SkeletonBone[] bonesCache;
		Dictionary<string, int> boneByNameCache;
		int[] boneParentsCache;

		/////////////////////////////////////////

		///// <summary>
		///// Whether the skeleton has normalized. In this mode Rotation and Scale of bones are applied to key frames.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> Normalized
		//{
		//	get { if( _normalized.BeginGet() ) Normalized = _normalized.Get( this ); return _normalized.value; }
		//	set { if( _normalized.BeginSet( this, ref value ) ) { try { NormalizedChanged?.Invoke( this ); } finally { _normalized.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Normalized"/> property value changes.</summary>
		//public event Action<Skeleton> NormalizedChanged;
		//ReferenceField<bool> _normalized = false;

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
		//	set { if( _skinningMode.BeginSet( this, ref value ) ) { try { SkinningModeChanged?.Invoke( this ); } finally { _skinningMode.EndSet(); } } }
		//}
		//public event Action<Skeleton> SkinningModeChanged;
		//ReferenceField<SkinningModeEnum> _skinningMode = SkinningModeEnum.Auto;

		/////////////////////////////////////////

		public void GetBones( bool forceUpdate, out SkeletonBone[] bones, out Dictionary<string, int> boneByName, out int[] boneParents )
		{
			if( bonesCache == null || forceUpdate )
			{
				bonesCache = GetComponents<SkeletonBone>( checkChildren: true );

				boneByNameCache = new Dictionary<string, int>( bonesCache.Length );
				for( int n = 0; n < bonesCache.Length; n++ )
					boneByNameCache[ bonesCache[ n ].Name ] = n;

				boneParentsCache = new int[ bonesCache.Length ];
				for( int n = 0; n < bonesCache.Length; n++ )
				{
					var parentBone = bonesCache[ n ].Parent as SkeletonBone;
					if( parentBone != null )
					{
						if( !boneByNameCache.TryGetValue( parentBone.Name, out boneParentsCache[ n ] ) )
							boneParentsCache[ n ] = -1;
					}
					else
						boneParentsCache[ n ] = -1;
				}
			}

			bones = bonesCache;
			boneByName = boneByNameCache;
			boneParents = boneParentsCache;
		}

		public SkeletonBone[] GetBones( bool forceUpdate = false )
		{
			GetBones( forceUpdate, out var bones, out _, out _ );
			return bones;
		}
	}
}
