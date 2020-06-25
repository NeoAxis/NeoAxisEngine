// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

//!!!!так?

//!!!!2 вариант - процедурно
//!!!!!!но ведь будет тормозить
//!!!!!!!!в _InvokeMember прекомпилять через Emit.

namespace NeoAxis
{
	//!!!!

	//public class Component_Transform : Component
	//{
	//	//Position
	//	Reference<Vec3> position;
	//	[Serialize]
	//	[DefaultValue( "0 0 0" )]
	//	public virtual Reference<Vec3> Position
	//	{
	//		get
	//		{
	//			if( !string.IsNullOrEmpty( position.GetByReference ) )
	//				Position = position.GetValue( this );
	//			return position;
	//		}
	//		set
	//		{
	//			if( position == value ) return;
	//			position = value;
	//			PositionChanged?.Invoke( this );
	//		}
	//	}
	//	public event Action<Component_Transform> PositionChanged;


	//	//Rotation
	//	Reference<Quat> rotation = Quat.Identity;
	//	[Serialize]
	//	[DefaultValue( "0 0 0" )]
	//	public virtual Reference<Quat> Rotation
	//	{
	//		get
	//		{
	//			if( !string.IsNullOrEmpty( rotation.GetByReference ) )
	//				Rotation = rotation.GetValue( this );
	//			return rotation;
	//		}
	//		set
	//		{
	//			if( rotation == value ) return;
	//			rotation = value;
	//			RotationChanged?.Invoke( this );
	//		}
	//	}
	//	public event Action<Component_Transform> RotationChanged;


	//	//Scale
	//	Reference<Vec3> scale = Vec3.One;
	//	[Serialize]
	//	[DefaultValue( "1 1 1" )]
	//	public virtual Reference<Vec3> Scale
	//	{
	//		get
	//		{
	//			if( !string.IsNullOrEmpty( scale.GetByReference ) )
	//				Scale = scale.GetValue( this );
	//			return scale;
	//		}
	//		set
	//		{
	//			if( scale == value ) return;
	//			scale = value;
	//			ScaleChanged?.Invoke( this );
	//		}
	//	}
	//	public event Action<Component_Transform> ScaleChanged;


	//	Transform lastResult;
	//	//!!!!name
	//	public Transform Result
	//	{
	//		get
	//		{
	//			//!!!!slowly

	//			//!!!!можно исходные данные кешироать, чтобы каждый раз не считать

	//			//!!!!if( Result == null )
	//			//{
	//			//!!!!!!так? slowly?

	//			//!!!!лишний раз не создавать
	//			//!!!!!slowly?

	//			var newResult = new Transform( Position, Rotation, Scale );
	//			if( lastResult == null || lastResult != newResult )
	//				lastResult = newResult;
	//			return lastResult;

	//			//Result = new Transform( pos, rot, scl );
	//			//}
	//		}
	//	}

	//	//	protected override void OnResultCompile()
	//	//	{
	//	//		//!!!!if( Result == null )

	//	//		//!!!!лишний раз не создавать
	//	//		//!!!!!slowly?
	//	//		var newResult = new Transform( Position, Rotation, Scale );
	//	//		if( Result == null || Result != newResult )
	//	//			Result = newResult;

	//	//		//Result = new Transform( Position, Rotation, Scale );
	//	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents object transformation. Can be used in pair with other transformations to calculate resulting offset of position, rotation, scale.
	/// </summary>
	public class Component_TransformOffset : Component
	{
		/// <summary>
		/// Source transformation.
		/// </summary>
		public Reference<Transform> Source
		{
			get { if( _source.BeginGet() ) Source = _source.Get( this ); return _source.value; }
			set { if( _source.BeginSet( ref value ) ) { try { SourceChanged?.Invoke( this ); } finally { _source.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Source"/> property value changes.</summary>
		public event Action<Component_TransformOffset> SourceChanged;
		ReferenceField<Transform> _source = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		////Source
		//Reference<Transform> source = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		///// <summary>
		///// Source transformation.
		///// </summary>
		//[Serialize]
		////[DefaultValue( "0 0 0" )]//!!!!!
		//public virtual Reference<Transform> Source
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( source.GetByReference ) )
		//			Source = source.GetValue( this );
		//		return source;
		//	}
		//	set
		//	{
		//		if( source == value ) return;
		//		source = value;
		//		SourceChanged?.Invoke( this );
		//	}
		//}
		///// <summary>Occurs when the <see cref="Source"/> property value changes.</summary>
		//public event Action<Component_TransformOffset> SourceChanged;

		/// <summary>
		/// Amount of position offset as a vector.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> PositionOffset
		{
			get { if( _positionOffset.BeginGet() ) PositionOffset = _positionOffset.Get( this ); return _positionOffset.value; }
			set { if( _positionOffset.BeginSet( ref value ) ) { try { PositionOffsetChanged?.Invoke( this ); } finally { _positionOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		public event Action<Component_TransformOffset> PositionOffsetChanged;
		ReferenceField<Vector3> _positionOffset;

		////!!!!name
		////PositionOffset
		//Reference<Vector3> positionOffset;

		///// <summary>
		///// Amount of position offset as a vector.
		///// </summary>
		//[Serialize]
		//[DefaultValue( "0 0 0" )]
		//public virtual Reference<Vector3> PositionOffset
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( positionOffset.GetByReference ) )
		//			PositionOffset = positionOffset.GetValue( this );
		//		return positionOffset;
		//	}
		//	set
		//	{
		//		if( positionOffset == value ) return;
		//		positionOffset = value;
		//		PositionOffsetChanged?.Invoke( this );
		//	}
		//}
		///// <summary>Occurs when the <see cref="PositionOffset"/> property value changes.</summary>
		//public event Action<Component_TransformOffset> PositionOffsetChanged;

		/// <summary>
		/// Amount of rotation offset as a quaternion.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		public Reference<Quaternion> RotationOffset
		{
			get { if( _rotationOffset.BeginGet() ) RotationOffset = _rotationOffset.Get( this ); return _rotationOffset.value; }
			set { if( _rotationOffset.BeginSet( ref value ) ) { try { RotationOffsetChanged?.Invoke( this ); } finally { _rotationOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffset"/> property value changes.</summary>
		public event Action<Component_TransformOffset> RotationOffsetChanged;
		ReferenceField<Quaternion> _rotationOffset = Quaternion.Identity;

		////RotationOffset
		//Reference<Quaternion> rotationOffset = Quaternion.Identity;
		///// <summary>
		///// Amount of rotation offset as a quaternion.
		///// </summary>
		//[Serialize]
		//[DefaultValue( "0 0 0 1" )]
		//public virtual Reference<Quaternion> RotationOffset
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( rotationOffset.GetByReference ) )
		//			RotationOffset = rotationOffset.GetValue( this );
		//		return rotationOffset;
		//	}
		//	set
		//	{
		//		if( rotationOffset == value ) return;
		//		rotationOffset = value;
		//		RotationOffsetChanged?.Invoke( this );
		//	}
		//}
		///// <summary>Occurs when the <see cref="RotationOffset"/> property value changes.</summary>
		//public event Action<Component_TransformOffset> RotationOffsetChanged;

		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> ScaleOffset
		{
			get { if( _scaleOffset.BeginGet() ) ScaleOffset = _scaleOffset.Get( this ); return _scaleOffset.value; }
			set { if( _scaleOffset.BeginSet( ref value ) ) { try { ScaleOffsetChanged?.Invoke( this ); } finally { _scaleOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ScaleOffset"/> property value changes.</summary>
		public event Action<Component_TransformOffset> ScaleOffsetChanged;
		ReferenceField<Vector3> _scaleOffset = Vector3.One;

		////ScaleOffset
		//Reference<Vector3> scaleOffset = Vector3.One;
		///// <summary>
		///// Amount of scale offset as a vector.
		///// </summary>
		//[Serialize]
		//[DefaultValue( "1 1 1" )]
		//public virtual Reference<Vector3> ScaleOffset
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( scaleOffset.GetByReference ) )
		//			ScaleOffset = scaleOffset.GetValue( this );
		//		return scaleOffset;
		//	}
		//	set
		//	{
		//		if( scaleOffset.Equals( value ) ) return;
		//		scaleOffset = value;
		//		ScaleOffsetChanged?.Invoke( this );
		//	}
		//}
		///// <summary>Occurs when the <see cref="ScaleOffset"/> property value changes.</summary>
		//public event Action<Component_TransformOffset> ScaleOffsetChanged;

		//!!!!
		////Matrix
		//ReferenceField<Mat4> _matrix = Mat4.Zero;
		//[DefaultValue( "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0" )]
		//[Serialize]
		//public Reference<Mat4> Matrix
		//{
		//	get
		//	{
		//		if( _matrix.BeginGet() )
		//			Matrix = _matrix.Get( this );
		//		return _matrix.value;
		//	}
		//	set
		//	{
		//		if( _matrix.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MatrixChanged?.Invoke( this );

		//				//!!!!
		//			}
		//			finally { _matrix.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_TransformOffset> MatrixChanged;


		Transform lastResult;
		/// <summary>
		/// The resut of a calculation.
		/// </summary>
		public Transform Result
		{
			get
			{
				//!!!!slowly

				//!!!!можно исходные данные кешироать, чтобы каждый раз не считать

				//!!!!if( Result == null )
				//{
				//!!!!!!так? slowly?

				Transform s = Source;
				////!!!!new
				//if( s == null )
				//	s = Transform.Identity;

				//Mat4 mat = s.ToMat4() * new Mat4( RotationOffset.Value.ToMat3() * Mat3.FromScale( ScaleOffset ), PositionOffset );
				//mat.Decompose( out Vec3 pos, out Quat rot, out Vec3 scl );
				//var newResult = new Transform( pos, rot, scl );

				//!!!!
				Transform newResult = null;
				//var matrix = Matrix.Value;
				//if( matrix != Mat4.Zero )
				//{
				//	var r = s.ToMat4() * matrix;

				//	//Mat3 rotM;
				//	//Vec3 scl;
				//	//Vec3 dummy;
				//	//r.ToMat3().QDUDecomposition( out rotM, out scl, out dummy );
				//	//Quat rot = rotM.ToQuat();
				//	//Vec3 pos = r.GetTranslation();

				//	r.Decompose( out Vec3 pos, out Quat rot, out Vec3 scl );

				//	newResult = new Transform( pos, rot, scl );
				//}
				//else
				//{
				if( s != null )
					newResult = s.ApplyOffset( PositionOffset, RotationOffset, ScaleOffset );
				//}

				//Transform newResult = s.ApplyOffset( PositionOffset, RotationOffset, ScaleOffset );

				////!!!!лишний раз не создавать
				////!!!!!slowly?
				//var newResult = new Transform( pos, rot, scl );
				if( lastResult == null || lastResult != newResult )
					lastResult = newResult;
				return lastResult;

				//Result = new Transform( pos, rot, scl );
				//}

			}
		}
	}
}
