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
	/// Represents mesh animation.
	/// </summary>
	public class Component_Animation : Component
	{
		/// <summary>
		/// The length of the animation in the seconds.
		/// </summary>
		[Serialize]
		public Reference<double> Length
		{
			get { if( _length.BeginGet() ) Length = _length.Get( this ); return _length.value; }
			set { if( _length.BeginSet( ref value ) ) { try { LengthChanged?.Invoke( this ); } finally { _length.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Length"/> property value changes.</summary>
		public event Action<Component_Animation> LengthChanged;
		ReferenceField<double> _length;












		//Component_ResultCompile<Component_Animation.CompiledData>

		///////////////////////////////////////////

		//public class CompiledData
		//{
		//	//!!!!
		//	//tracks
		//	//triggers
		//}

		///////////////////////////////////////////

		//protected override void OnResultCompile()
		//{
		//	if( Result == null )
		//	{
		//		//!!!!
		//	}
		//}
	}
}
