// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
			set { if( _keyFrames.BeginSet( ref value ) ) { try { KeyFramesChanged?.Invoke( this ); } finally { _keyFrames.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="KeyFrames"/> property value changes.</summary>
		public event Action<SkeletonAnimationTrack> KeyFramesChanged;
		ReferenceField<byte[]> _keyFrames;

		/////////////////////////////////////////

		/// <summary>
		/// Represents key frame data of <see cref="SkeletonAnimationTrack"/>.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
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
		}

		//!!!!is not used
		//!!!!slowly, can be cached
		public unsafe (float min, float max) GetTimeRange()
		{
			var keyFrames2 = KeyFrames.Value;
			if( keyFrames2 == null )
				return (0, 0);

			Debug.Assert( keyFrames2.Length % sizeof( KeyFrame ) == 0 );
			int keyFramesCount = keyFrames2.Length / sizeof( KeyFrame );
			if( keyFramesCount == 0 )
				return (0, 0);

			fixed ( byte* pKeyFrames = keyFrames2 )
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

		public unsafe void CalculateBoneTransforms( double time, CalculateBoneTransformsItem[] result )
		{
			//!!!!slowly
			//?? Можно сделать BinarySearch - при сравнении старший компонет boneIndex, младший time. Но его надо повторять для каждой кости - будет быстрее только при большом числе фреймов на кость.

			var keyFrames2 = KeyFrames.Value;
			if( keyFrames2 == null )
				return;

			if( keyFrames2.Length % sizeof( KeyFrame ) != 0 )
				Log.Fatal( "SkeletonAnimationTrack: CalculateBoneTransforms: keyFrames2.Length % sizeof( KeyFrame ) != 0." );

			int keyFramesCount = keyFrames2.Length / sizeof( KeyFrame );
			if( keyFramesCount == 0 )
				return;

			fixed ( byte* pKeyFrames = keyFrames2 )
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

					if( foundIndex == -1 ) //time after the last keyframe.
						GetBoneTransformsItem( ref keyFrames[ curIndex - 1 ], out result[ curBoneIndex ] );
					else
					{
						if( foundExact )
							GetBoneTransformsItem( ref keyFrames[ foundIndex ], out result[ curBoneIndex ] );
						else
						{
							if( foundIndex == indexOfFirstFrameOfBone )
							{
								//time is before the first frame. Then return the found frame (?? or another possible option - Identity )
								//result[ curBoneIndex ] = new CalculateBoneTransformsItem { Position = Vector3F.Zero, Rotation = QuaternionF.Identity, Scale = Vector3F.One };
								GetBoneTransformsItem( ref keyFrames[ foundIndex ], out result[ curBoneIndex ] );
							}
							else
								Interpolate( ref keyFrames[ foundIndex - 1 ], ref keyFrames[ foundIndex ], time, out result[ curBoneIndex ] );
						}
					}

					//skip remaining for curBoneIndex
					for( ; curIndex < keyFramesCount && keyFrames[ curIndex ].BoneIndex == curBoneIndex; curIndex++ ) ;
				}
			}
		}

		static void Interpolate( ref KeyFrame keyFrame1, ref KeyFrame keyFrame2, double time, out CalculateBoneTransformsItem result )
		{
			float t = (float)( ( time - keyFrame1.Time ) / ( keyFrame2.Time - keyFrame1.Time ) );
			result.Position = Vector3F.Lerp( keyFrame1.Position, keyFrame2.Position, t );
			result.Rotation = QuaternionF.Slerp( keyFrame1.Rotation, keyFrame2.Rotation, t );
			result.Scale = Vector3F.Lerp( keyFrame1.Scale, keyFrame2.Scale, t );
		}

		static void GetBoneTransformsItem( ref KeyFrame keyFrame, out CalculateBoneTransformsItem result )
		{
			result.Position = keyFrame.Position;
			result.Rotation = keyFrame.Rotation;
			result.Scale = keyFrame.Scale;
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
		//	set { if( _keyFrames.BeginSet( ref value ) ) { try { KeyFramesChanged?.Invoke( this ); } finally { _keyFrames.EndSet(); } } }
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
