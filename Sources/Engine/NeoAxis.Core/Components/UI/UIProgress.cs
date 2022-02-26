// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// UI element for visualization progress.
	/// </summary>
	public class UIProgress : UIControl
	{
		/// <summary>
		/// Specifies the maximum position.
		/// </summary>
		[DefaultValue( 100.0 )]
		public Reference<double> Maximum
		{
			get { if( _maximum.BeginGet() ) Maximum = _maximum.Get( this ); return _maximum.value; }
			set { if( _maximum.BeginSet( ref value ) ) { try { MaximumChanged?.Invoke( this ); } finally { _maximum.EndSet(); } } }
		}
		public event Action<UIProgress> MaximumChanged;
		ReferenceField<double> _maximum = 100.0;

		/// <summary>
		/// Specifies the current position.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 100 )]
		public Reference<double> Value
		{
			get { if( _value.BeginGet() ) Value = _value.Get( this ); return _value.value; }
			set { if( _value.BeginSet( ref value ) ) { try { ValueChanged?.Invoke( this ); } finally { _value.EndSet(); } } }
		}
		public event Action<UIProgress> ValueChanged;
		ReferenceField<double> _value = 0.0;

		///////////////////////////////////////////

		public UIProgress()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 40 );
		}

		//protected override void OnRenderUI( CanvasRenderer renderer )
		//{
		//	base.OnRenderUI( renderer );

		//var textControl = GetTextControl();
		//if( textControl != null )
		//{
		//	//!!!!
		//	//textControl.SupportLocalization = false;

		//	//textControl.ScreenClipRectangle = textControl.GetScreenRectangle();

		//	//string text = Text;
		//	//if( Focused )
		//	//	text += "_";
		//	//OnUpdatingTextControl( ref text );
		//	//textControl.Text = text;

		//	//update textHorizontalAlignment
		//	{
		//		//!!!!не всегда нужно менять. может стиль какой-нибудь особый

		//		EHorizontalAlignment horizontalAlign = EHorizontalAlignment.Left;

		//		if( Focused )
		//		{
		//			var font = textControl.Font.Value;
		//			if( font == null )
		//				font = renderer.DefaultFont;
		//			if( font != null )
		//			{
		//				var fontSize = textControl.GetFontSizeScreen();

		//				float textLength = font.GetTextLength( fontSize, renderer, textControl.Text );
		//				if( textLength > textControl.GetScreenSize().X )
		//					horizontalAlign = EHorizontalAlignment.Right;
		//			}
		//		}

		//		textControl.TextHorizontalAlignment = horizontalAlign;
		//	}
		//}
		//}

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//	base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

		//	var obj = CreateComponent<UIText>();
		//	obj.Name = "Text";
		//	obj.Text = ReferenceUtility.MakeThisReference( obj, this, "DisplayText" );//"Text" );
		//	obj.TextHorizontalAlignment = EHorizontalAlignment.Left;
		//	//obj.TextHorizontalAlignment = ReferenceUtility.MakeThisReference( obj, this, "TextHorizontalAlignment" );
		//	obj.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );
		//	obj.CanBeSelected = false;
		//	obj.HorizontalAlignment = EHorizontalAlignment.Stretch;
		//	obj.VerticalAlignment = EVerticalAlignment.Stretch;
		//	//obj.Offset = new UIMeasureValueVector2( UIMeasure.Units, 4, 0 );
		//	obj.ClipRectangle = true;

		//	//UIStyle.EditTextMargin
		//	obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 4, 2, 4, 2 );
		//	//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );
		//}

		//public UIText GetTextControl()
		//{
		//	return GetComponentByPath( "Text" ) as UIText;
		//}

		//[Browsable( false )]
		//public string DisplayText
		//{
		//	get
		//	{
		//		string text = Text;
		//		if( Focused )
		//			text += "_";
		//		//OnUpdatingTextControl( ref text );
		//		return text;
		//	}
		//}

		//[Browsable( false )]
		//public EHorizontalAlignment TextHorizontalAlignment
		//{
		//	get { return textHorizontalAlignment; }
		//}
	}
}
