//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	//!!!!в транк
//	//!!!!в base types
//	//!!!!в доки

//	//!!!!name

//	/// <summary>
//	/// Specifies a modifier for the material of the mesh.
//	/// </summary>
//	public class PaintMaterialData : Component
//	{
//		/// <summary>
//		/// The intensity of the modifier.
//		/// </summary>
//		[DefaultValue( 1.0 )]
//		[Range( 0, 1 )]
//		public Reference<double> Intensity
//		{
//			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
//			set { if( _intensity.BeginSet( this, ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
//		public event Action<PaintMaterialData> IntensityChanged;
//		ReferenceField<double> _intensity = 1.0;

//		/// <summary>
//		/// The data of the modifier.
//		/// </summary>
//		[DefaultValue( null )]
//		public Reference<byte[]> Data
//		{
//			get { if( _data.BeginGet() ) Data = _data.Get( this ); return _data.value; }
//			set { if( _data.BeginSet( this, ref value ) ) { try { DataChanged?.Invoke( this ); } finally { _data.EndSet(); } } }
//		}
//		/// <summary>Occurs when the <see cref="Data"/> property value changes.</summary>
//		public event Action<PaintMaterialData> DataChanged;
//		ReferenceField<byte[]> _data = null;
//	}
//}
