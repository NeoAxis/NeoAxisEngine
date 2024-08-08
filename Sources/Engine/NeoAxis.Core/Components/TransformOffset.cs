// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents the transformation of an object. It can be used in combination with other transformations to calculate the resulting position offset, rotation, scale. 
	/// </summary>
	public class TransformOffset : Component
	{
		/// <summary>
		/// Source transformation.
		/// </summary>
		public Reference<Transform> Source
		{
			get { if( _source.BeginGet() ) Source = _source.Get( this ); return _source.value; }
			set { if( _source.BeginSet( this, ref value ) ) { try { SourceChanged?.Invoke( this ); } finally { _source.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Source"/> property value changes.</summary>
		public event Action<TransformOffset> SourceChanged;
		ReferenceField<Transform> _source = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		public enum ModeEnum
		{
			Elements,
			Matrix,
		}

		/// <summary>
		/// The mode of the component.
		/// </summary>
		[DefaultValue( ModeEnum.Elements )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( this, ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<TransformOffset> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Elements;

		/// <summary>
		/// Amount of position offset as a vector.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> PositionOffset
		{
			get { if( _positionOffset.BeginGet() ) PositionOffset = _positionOffset.Get( this ); return _positionOffset.value; }
			set { if( _positionOffset.BeginSet( this, ref value ) ) { try { PositionOffsetChanged?.Invoke( this ); } finally { _positionOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		public event Action<TransformOffset> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset;

		/// <summary>
		/// Amount of rotation offset as a quaternion.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		public Reference<Quaternion> RotationOffset
		{
			get { if( _rotationOffset.BeginGet() ) RotationOffset = _rotationOffset.Get( this ); return _rotationOffset.value; }
			set { if( _rotationOffset.BeginSet( this, ref value ) ) { try { RotationOffsetChanged?.Invoke( this ); } finally { _rotationOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffset"/> property value changes.</summary>
		public event Action<TransformOffset> RotationOffsetChanged;
		ReferenceField<Quaternion> _rotationOffset = Quaternion.Identity;

		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> ScaleOffset
		{
			get { if( _scaleOffset.BeginGet() ) ScaleOffset = _scaleOffset.Get( this ); return _scaleOffset.value; }
			set { if( _scaleOffset.BeginSet( this, ref value ) ) { try { ScaleOffsetChanged?.Invoke( this ); } finally { _scaleOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleOffset"/> property value changes.</summary>
		public event Action<TransformOffset> ScaleOffsetChanged;
		ReferenceField<Vector3> _scaleOffset = Vector3.One;

		[DefaultValue( Matrix4.IdentityAsString )]
		public Reference<Matrix4> Matrix
		{
			get { if( _matrix.BeginGet() ) Matrix = _matrix.Get( this ); return _matrix.value; }
			set { if( _matrix.BeginSet( this, ref value ) ) { try { MatrixChanged?.Invoke( this ); } finally { _matrix.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Matrix"/> property value changes.</summary>
		public event Action<TransformOffset> MatrixChanged;
		ReferenceField<Matrix4> _matrix = Matrix4.Identity;

		/////////////////////////////////////////

		Transform lastResult;
		/// <summary>
		/// The result of a calculation.
		/// </summary>
		public Transform Result
		{
			get
			{
				Transform s = Source;

				if( s != null )
				{
					Vector3 pos;
					Quaternion rot;
					Vector3 scl;

					if( Mode.Value == ModeEnum.Elements )
					{
						pos = s.Position;
						rot = s.Rotation;
						scl = s.Scale;

						pos += rot * ( PositionOffset * scl );
						rot *= RotationOffset;
						scl *= ScaleOffset;

						//newResult = s.ApplyOffset( PositionOffset, RotationOffset, ScaleOffset );
					}
					else
					{
						var r = s.ToMatrix4() * Matrix.Value;
						r.Decompose( out pos, out rot, out scl );
					}

					if( lastResult == null || lastResult.Position != pos || lastResult.Rotation != rot || lastResult.Scale != scl )
						lastResult = new Transform( pos, rot, scl );
				}
				else
					lastResult = null;

				return lastResult;


				//Transform s = Source;

				//Transform newResult = null;
				//if( s != null )
				//{
				//	if( Mode.Value == ModeEnum.Elements )
				//		newResult = s.ApplyOffset( PositionOffset, RotationOffset, ScaleOffset );
				//	else
				//	{
				//		var r = s.ToMatrix4() * Matrix.Value;
				//		r.Decompose( out var position, out Quaternion rotation, out var scale );
				//		newResult = new Transform( position, rotation, scale );
				//	}
				//}

				//if( lastResult == null || lastResult != newResult )
				//	lastResult = newResult;
				//return lastResult;
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( PositionOffset ):
				case nameof( RotationOffset ):
				case nameof( ScaleOffset ):
					if( Mode.Value != ModeEnum.Elements )
						skip = true;
					break;

				case nameof( Matrix ):
					if( Mode.Value != ModeEnum.Matrix )
						skip = true;
					break;
				}
			}
		}
	}
}
