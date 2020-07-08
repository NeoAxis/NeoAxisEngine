// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a window that makes up an application's user interface.
	/// </summary>
	public class UIWindow : UIControl
	{
		/// <summary>
		/// Enables window header displaying.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> TitleBar
		{
			get { if( _titleBar.BeginGet() ) TitleBar = _titleBar.Get( this ); return _titleBar.value; }
			set { if( _titleBar.BeginSet( ref value ) ) { try { TitleBarChanged?.Invoke( this ); } finally { _titleBar.EndSet(); } } }
		}
		public event Action<UIWindow> TitleBarChanged;
		ReferenceField<bool> _titleBar = true;

		//[DefaultValue( true )]
		//public Reference<bool> CloseButton
		//{
		//	get { if( _closeButton.BeginGet() ) CloseButton = _closeButton.Get( this ); return _closeButton.value; }
		//	set { if( _closeButton.BeginSet( ref value ) ) { try { CloseButtonChanged?.Invoke( this ); } finally { _closeButton.EndSet(); } } }
		//}
		//public event Action<UIWindow> CloseButtonChanged;
		//ReferenceField<bool> _closeButton = true;

		public UIWindow()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 800, 600 );
		}

		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.OnlyBehind; }
		}
	}
}
