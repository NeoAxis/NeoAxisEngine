// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents the track of a skeleton animation. Defines how mesh skeleton bones will move in space.
	/// </summary>
	public class SkeletonAnimationTrack : Component
	{
		//KeyFrames must be groupped by BoneIndex, and inside the group must be ordered by time. The bone groups ordered like Skeleton.GetBones().
		/// <summary>
		/// Data of the track.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Shallow )]
		public Reference<byte[]> KeyFrames
		{
			get { if( _keyFrames.BeginGet() ) KeyFrames = _keyFrames.Get( this ); return _keyFrames.value; }
			set { if( _keyFrames.BeginSet( this, ref value ) ) { try { KeyFramesChanged?.Invoke( this ); } finally { _keyFrames.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyFrames"/> property value changes.</summary>
		public event Action<SkeletonAnimationTrack> KeyFramesChanged;
		ReferenceField<byte[]> _keyFrames;

		/////////////////////////////////////////

		/// <summary>
		/// Represents key frame data of <see cref="SkeletonAnimationTrack"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct KeyFrame
		{
			public float Time;
			public int BoneIndex;
			public Vector3F Position;
			public QuaternionF Rotation;
			public Vector3F Scale;

			//

			public KeyFrame( float time, int boneIndex, Vector3F position, QuaternionF rotation, Vector3F scale )
			{
				this.Time = time;
				this.BoneIndex = boneIndex;
				this.Position = position;
				this.Rotation = rotation;
				this.Scale = scale;
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Represents an output item for bone transforms calculation.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct CalculateBoneTransformsItem
		{
			public Vector3F Position;
			public QuaternionF Rotation;
			public Vector3F Scale;
			public FlagsEnum Flags;
			public bool NeedUpdate;

			//

			[Flags]
			public enum FlagsEnum
			{
				GlobalPosition,
				GlobalRotation,
				GlobalScale,
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void ToMatrix( out Matrix4F result )
			{
				Rotation.ToMatrix3( out var rot );
				Matrix3F.FromScale( ref Scale, out var scl );
				Matrix3F.Multiply( ref rot, ref scl, out var rot2 );
				result = new Matrix4F( rot2, Position );
			}
		}

		//!!!!is not used
		//!!!!slowly, can be cached
		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe (float min, float max) GetTimeRange()
		{
			var keyFrames2 = KeyFrames.Value;
			if( keyFrames2 == null )
				return (0, 0);

			Debug.Assert( keyFrames2.Length % sizeof( KeyFrame ) == 0 );
			int keyFramesCount = keyFrames2.Length / sizeof( KeyFrame );
			if( keyFramesCount == 0 )
				return (0, 0);

			fixed( byte* pKeyFrames = keyFrames2 )
			{
				var keyFrames = (KeyFrame*)pKeyFrames;
				float minTime = keyFrames[ 0 ].Time;
				float maxTime = keyFrames[ 0 ].Time;
				for( int i = 1; i < keyFramesCount; i++ )
				{
					if( maxTime < keyFrames[ i ].Time )
						maxTime = keyFrames[ i ].Time;
					if( keyFrames[ i ].Time < minTime )
						minTime = keyFrames[ i ].Time;
				}
				return (minTime, maxTime);
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void CalculateBoneTransforms( SkeletonBone[] bones, Dictionary<string, int> boneByName, SkeletonAnimation animation, double time, CalculateBoneTransformsItem* output )
		{
			//!!!!slowly
			//?? Можно сделать BinarySearch - при сравнении старший компонет boneIndex, младший time. Но его надо повторять для каждой кости - будет быстрее только при большом числе фреймов на кость.

			var originalSkeleton = animation.OriginalSkeleton.Value;
			var retargeting = originalSkeleton != null;

			CalculateBoneTransformsItem* output2;
			//int[] sourceBoneIndexToResultBoneIndex = null;

			if( retargeting )
			{
				originalSkeleton.GetBones( false, out var originalBones, out var originalBoneByName, out var originalBoneParents );

				var result3 = stackalloc CalculateBoneTransformsItem[ originalBones.Length ];
				output2 = result3;

				//var sourceBoneIndexToResultBoneIndex = new int[ qqq ];
			}
			else
				output2 = output;


			var keyFrames2 = KeyFrames.Value;
			if( keyFrames2 == null )
				return;

			if( keyFrames2.Length % sizeof( KeyFrame ) != 0 )
				Log.Fatal( "SkeletonAnimationTrack: CalculateBoneTransforms: keyFrames2.Length % sizeof( KeyFrame ) != 0." );

			int keyFramesCount = keyFrames2.Length / sizeof( KeyFrame );
			if( keyFramesCount == 0 )
				return;

			fixed( byte* pKeyFrames = keyFrames2 )
			{
				var keyFrames = (KeyFrame*)pKeyFrames;
				//var boneCount = result.Length;

				int curIndex = 0;

				while( curIndex < keyFramesCount )
				{
					int curBoneIndex = keyFrames[ curIndex ].BoneIndex;
					int indexOfFirstFrameOfBone = curIndex;

					int foundIndex = -1;
					bool foundExact = false;
					for( ; curIndex < keyFramesCount && keyFrames[ curIndex ].BoneIndex == curBoneIndex; curIndex++ )
					{
						double t = keyFrames[ curIndex ].Time;
						if( time <= t )
						{
							foundIndex = curIndex;
							foundExact = time == t;
							break;
						}
					}

					if( curBoneIndex < bones.Length )
					{
						if( foundIndex == -1 ) //time after the last keyframe.
							GetBoneTransformsItem( ref keyFrames[ curIndex - 1 ], out output2[ curBoneIndex ] );
						else
						{
							if( foundExact )
								GetBoneTransformsItem( ref keyFrames[ foundIndex ], out output2[ curBoneIndex ] );
							else
							{
								if( foundIndex == indexOfFirstFrameOfBone )
								{
									//time is before the first frame. Then return the found frame (?? or another possible option - Identity )
									//result[ curBoneIndex ] = new CalculateBoneTransformsItem { Position = Vector3F.Zero, Rotation = QuaternionF.Identity, Scale = Vector3F.One };
									GetBoneTransformsItem( ref keyFrames[ foundIndex ], out output2[ curBoneIndex ] );
								}
								else
									Interpolate( ref keyFrames[ foundIndex - 1 ], ref keyFrames[ foundIndex ], time, out output2[ curBoneIndex ] );
							}
						}
					}
					else
					{
						//format error
						break;
					}

					//skip remaining for curBoneIndex
					for( ; curIndex < keyFramesCount && keyFrames[ curIndex ].BoneIndex == curBoneIndex; curIndex++ ) ;
				}
			}

			if( retargeting )
			{
				originalSkeleton.GetBones( false, out var originalBones, out var originalBoneByName, out var originalBoneParents );

				//!!!!hardcoded
				var boneNameToCalculateRetargeting = "mixamorig:LeftHand";

				var heightFactor = 1.0;
				if( boneByName.TryGetValue( boneNameToCalculateRetargeting, out var leftHandIndex ) )
					heightFactor = bones[ leftHandIndex ].Transform.Value.Position.Z;

				var originalHeightFactor = 1.0;
				if( originalBoneByName.TryGetValue( boneNameToCalculateRetargeting, out var originalLeftHandIndex ) )
					originalHeightFactor = originalBones[ originalLeftHandIndex ].Transform.Value.Position.Z;


				for( int n = 0; n < bones.Length; n++ )
				{
					var outputBone = bones[ n ];

					if( originalBoneByName.TryGetValue( outputBone.Name, out var originalBoneIndex ) )
					{
						//var sourceBoneIndex = sourceBoneIndexToResultBoneIndex[ n ];
						//if( sourceBoneIndex != -1 )
						//{

						output[ n ] = output2[ originalBoneIndex ];

						//}
					}
					else
					{
						//no such bone. need to get from t-pose?

						ref var v = ref output[ n ];

						v.Position = Vector3F.Zero;
						v.Rotation = QuaternionF.Identity;
						v.Scale = Vector3F.One;

						//!!!!impl

						//if( EngineApp._DebugCapsLock )
						//{
						//	var parentBone = outputBone.Parent as SkeletonBone;
						//	if( parentBone != null )
						//	{
						//		var offset = outputBone.TransformV.ToMatrix4() * parentBone.TransformV.ToMatrix4().GetInverse();

						//		offset.Decompose( out var pos, out Quaternion rot, out var scl );

						//		v.Position = pos.ToVector3F();
						//		v.Rotation = rot.ToQuaternionF();
						//		v.Scale = scl.ToVector3F();

						//	}
						//}
					}
				}

				//apply scale difference
				if( originalHeightFactor != 0 )
				{
					var scaleFactor = (float)( heightFactor / originalHeightFactor );
					ref var output0 = ref output[ 0 ];
					output0.Scale *= scaleFactor;
					output0.Position.Z += scaleFactor - 1.0f;
				}
			}
		}


		//[MethodImpl( (MethodImplOptions)512 )]
		//public unsafe void CalculateBoneTransforms( double time, CalculateBoneTransformsItem* result ) //CalculateBoneTransformsItem[] result )
		//{
		//	//!!!!slowly
		//	//?? Можно сделать BinarySearch - при сравнении старший компонет boneIndex, младший time. Но его надо повторять для каждой кости - будет быстрее только при большом числе фреймов на кость.

		//	var keyFrames2 = KeyFrames.Value;
		//	if( keyFrames2 == null )
		//		return;

		//	if( keyFrames2.Length % sizeof( KeyFrame ) != 0 )
		//		Log.Fatal( "SkeletonAnimationTrack: CalculateBoneTransforms: keyFrames2.Length % sizeof( KeyFrame ) != 0." );

		//	int keyFramesCount = keyFrames2.Length / sizeof( KeyFrame );
		//	if( keyFramesCount == 0 )
		//		return;

		//	fixed( byte* pKeyFrames = keyFrames2 )
		//	{
		//		var keyFrames = (KeyFrame*)pKeyFrames;
		//		//var boneCount = result.Length;

		//		int curIndex = 0;

		//		while( curIndex < keyFramesCount )
		//		{
		//			int curBoneIndex = keyFrames[ curIndex ].BoneIndex;
		//			int indexOfFirstFrameOfBone = curIndex;

		//			int foundIndex = -1;
		//			bool foundExact = false;
		//			for( ; curIndex < keyFramesCount && keyFrames[ curIndex ].BoneIndex == curBoneIndex; curIndex++ )
		//			{
		//				double t = keyFrames[ curIndex ].Time;
		//				if( time <= t )
		//				{
		//					foundIndex = curIndex;
		//					foundExact = time == t;
		//					break;
		//				}
		//			}

		//			if( foundIndex == -1 ) //time after the last keyframe.
		//				GetBoneTransformsItem( ref keyFrames[ curIndex - 1 ], out result[ curBoneIndex ] );
		//			else
		//			{
		//				if( foundExact )
		//					GetBoneTransformsItem( ref keyFrames[ foundIndex ], out result[ curBoneIndex ] );
		//				else
		//				{
		//					if( foundIndex == indexOfFirstFrameOfBone )
		//					{
		//						//time is before the first frame. Then return the found frame (?? or another possible option - Identity )
		//						//result[ curBoneIndex ] = new CalculateBoneTransformsItem { Position = Vector3F.Zero, Rotation = QuaternionF.Identity, Scale = Vector3F.One };
		//						GetBoneTransformsItem( ref keyFrames[ foundIndex ], out result[ curBoneIndex ] );
		//					}
		//					else
		//						Interpolate( ref keyFrames[ foundIndex - 1 ], ref keyFrames[ foundIndex ], time, out result[ curBoneIndex ] );
		//				}
		//			}

		//			//skip remaining for curBoneIndex
		//			for( ; curIndex < keyFramesCount && keyFrames[ curIndex ].BoneIndex == curBoneIndex; curIndex++ ) ;
		//		}
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static void Interpolate( ref KeyFrame keyFrame1, ref KeyFrame keyFrame2, double time, out CalculateBoneTransformsItem result )
		{
			float t = (float)( ( time - keyFrame1.Time ) / ( keyFrame2.Time - keyFrame1.Time ) );
			Vector3F.Lerp( ref keyFrame1.Position, ref keyFrame2.Position, t, out result.Position );
			QuaternionF.Slerp( ref keyFrame1.Rotation, ref keyFrame2.Rotation, t, out result.Rotation );
			Vector3F.Lerp( ref keyFrame1.Scale, ref keyFrame2.Scale, t, out result.Scale );
			result.Flags = 0;
			result.NeedUpdate = false;

			//result.Position = Vector3F.Lerp( keyFrame1.Position, keyFrame2.Position, t );
			//result.Rotation = QuaternionF.Slerp( keyFrame1.Rotation, keyFrame2.Rotation, t );
			//result.Scale = Vector3F.Lerp( keyFrame1.Scale, keyFrame2.Scale, t );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static void GetBoneTransformsItem( ref KeyFrame keyFrame, out CalculateBoneTransformsItem result )
		{
			result.Position = keyFrame.Position;
			result.Rotation = keyFrame.Rotation;
			result.Scale = keyFrame.Scale;
			result.Flags = 0;
			result.NeedUpdate = false;
		}

		public static unsafe byte[] ToBytes( List<KeyFrame> keyframes )
		{
			byte[] data = new byte[ keyframes.Count * sizeof( KeyFrame ) ];
			int index = 0;
			for( int i = 0; i < keyframes.Count; i++ )
			{
				KeyFrame cur = keyframes[ i ];
				byte* pKeyFrame = (byte*)&cur;
				for( int j = 0; j < sizeof( KeyFrame ); j++ )
					data[ index++ ] = pKeyFrame[ j ];
			}
			return data;
		}




		//static const unsigned char CHANNEL_POSITION = 0x1;
		//static const unsigned char CHANNEL_ROTATION = 0x2;
		//static const unsigned char CHANNEL_SCALE = 0x4;
		///// Bitmask of included data (position, rotation, scale.)
		//unsigned char channelMask_;

		//[Serialize]
		//public Reference<KeyFrame[]> KeyFrames
		//{
		//	get { if( _keyFrames.BeginGet() ) KeyFrames = _keyFrames.Get( this ); return _keyFrames.value; }
		//	set { if( _keyFrames.BeginSet( this, ref value ) ) { try { KeyFramesChanged?.Invoke( this ); } finally { _keyFrames.EndSet(); } } }
		//}
		//public event Action<SkeletonAnimationTrack> KeyFramesChanged;
		//ReferenceField<KeyFrame[]> _keyFrames;

		///// Assign keyframe at index.
		//void SetKeyFrame( unsigned index, const AnimationKeyFrame& keyFrame);
		///// Add a keyframe at the end.
		//void AddKeyFrame(const AnimationKeyFrame& keyFrame);
		///// Insert a keyframe at index.
		//void InsertKeyFrame( unsigned index, const AnimationKeyFrame& keyFrame);
		///// Remove a keyframe at index.
		//void RemoveKeyFrame( unsigned index );
		///// Remove all keyframes.
		//void RemoveAllKeyFrames();

		///// Return keyframe at index, or null if not found.
		//AnimationKeyFrame* GetKeyFrame( unsigned index );
		///// Return number of keyframes.
		//unsigned GetNumKeyFrames() const { return keyFrames_.Size(); }
		///// Return keyframe index based on time and previous index.
		//void GetKeyFrameIndex( float time, unsigned& index) const;
	}
}
