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
		/// The size of the border.
		/// </summary>
		[DefaultValue( "Units 4 4" )]
		public Reference<UIMeasureValueVector2> BorderSize
		{
			get { if( _borderSize.BeginGet() ) BorderSize = _borderSize.Get( this ); return _borderSize.value; }
			set { if( _borderSize.BeginSet( ref value ) ) { try { BorderSizeChanged?.Invoke( this ); } finally { _borderSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BorderSize"/> property value changes.</summary>
		public event Action<UIWindow> BorderSizeChanged;
		ReferenceField<UIMeasureValueVector2> _borderSize = new UIMeasureValueVector2( UIMeasure.Units, 4, 4 );

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

		/// <summary>
		/// The height of the title bar.
		/// </summary>
		[DefaultValue( "Units 34" )]
		public Reference<UIMeasureValueDouble> TitleBarHeight
		{
			get { if( _titleBarHeight.BeginGet() ) TitleBarHeight = _titleBarHeight.Get( this ); return _titleBarHeight.value; }
			set { if( _titleBarHeight.BeginSet( ref value ) ) { try { TitleBarHeightChanged?.Invoke( this ); } finally { _titleBarHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TitleBarHeight"/> property value changes.</summary>
		public event Action<UIWindow> TitleBarHeightChanged;
		ReferenceField<UIMeasureValueDouble> _titleBarHeight = new UIMeasureValueDouble( UIMeasure.Units, 34 );

		/// <summary>
		/// The font size of the title bar.
		/// </summary>
		[DefaultValue( "Units 24" )]
		public Reference<UIMeasureValueDouble> TitleBarFontSize
		{
			get { if( _titleBarFontSize.BeginGet() ) TitleBarFontSize = _titleBarFontSize.Get( this ); return _titleBarFontSize.value; }
			set { if( _titleBarFontSize.BeginSet( ref value ) ) { try { TitleBarFontSizeChanged?.Invoke( this ); } finally { _titleBarFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TitleBarFontSize"/> property value changes.</summary>
		public event Action<UIWindow> TitleBarFontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _titleBarFontSize = new UIMeasureValueDouble( UIMeasure.Units, 24 );

		//[DefaultValue( true )]
		//public Reference<bool> CloseButton
		//{
		//	get { if( _closeButton.BeginGet() ) CloseButton = _closeButton.Get( this ); return _closeButton.value; }
		//	set { if( _closeButton.BeginSet( ref value ) ) { try { CloseButtonChanged?.Invoke( this ); } finally { _closeButton.EndSet(); } } }
		//}
		//public event Action<UIWindow> CloseButtonChanged;
		//ReferenceField<bool> _closeButton = true;

		///////////////////////////////////////////////

		public UIWindow()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 800, 600 );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( TitleBarHeight ):
					if( !TitleBar )
						skip = true;
					break;
				}
			}
		}

		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.OnlyBehind; }
		}

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			//unfocus controls
			ParentContainer?.FocusedControl?.Unfocus();
		}
	}
}
