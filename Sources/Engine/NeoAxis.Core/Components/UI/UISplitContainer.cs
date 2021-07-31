// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Control the sizing of two panels.
	/// </summary>
	public class UISplitContainer : UIControl
	{
		[DefaultValue( "Units 8" )]
		public Reference<UIMeasureValueDouble> SplitterSize
		{
			get { if( _splitterSize.BeginGet() ) SplitterSize = _splitterSize.Get( this ); return _splitterSize.value; }
			set { if( _splitterSize.BeginSet( ref value ) ) { try { SplitterSizeChanged?.Invoke( this ); } finally { _splitterSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SplitterSize"/> property value changes.</summary>
		public event Action<UISplitContainer> SplitterSizeChanged;
		ReferenceField<UIMeasureValueDouble> _splitterSize = new UIMeasureValueDouble( UIMeasure.Units, 8 );

		[DefaultValue( true )]
		public Reference<bool> Vertical
		{
			get { if( _vertical.BeginGet() ) Vertical = _vertical.Get( this ); return _vertical.value; }
			set
			{
				if( _vertical.BeginSet( ref value ) )
				{
					try
					{
						VerticalChanged?.Invoke( this );

						//!!!!
						//!!!!где еще
					}
					finally { _vertical.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Vertical"/> property value changes.</summary>
		public event Action<UISplitContainer> VerticalChanged;
		ReferenceField<bool> _vertical = true;

		public enum FixedPanelEnum
		{
			None,
			Panel1,
			Panel2,
		}

		[DefaultValue( FixedPanelEnum.None )]
		public Reference<FixedPanelEnum> FixedPanel
		{
			get { if( _fixedPanel.BeginGet() ) FixedPanel = _fixedPanel.Get( this ); return _fixedPanel.value; }
			set { if( _fixedPanel.BeginSet( ref value ) ) { try { FixedPanelChanged?.Invoke( this ); } finally { _fixedPanel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FixedPanel"/> property value changes.</summary>
		public event Action<UISplitContainer> FixedPanelChanged;
		ReferenceField<FixedPanelEnum> _fixedPanel = FixedPanelEnum.None;

		[DefaultValue( "Units 10" )]
		public Reference<UIMeasureValueDouble> Panel1MinSize
		{
			get { if( _panel1MinSize.BeginGet() ) Panel1MinSize = _panel1MinSize.Get( this ); return _panel1MinSize.value; }
			set { if( _panel1MinSize.BeginSet( ref value ) ) { try { Panel1MinSizeChanged?.Invoke( this ); } finally { _panel1MinSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Panel1MinSize"/> property value changes.</summary>
		public event Action<UISplitContainer> Panel1MinSizeChanged;
		ReferenceField<UIMeasureValueDouble> _panel1MinSize = new UIMeasureValueDouble( UIMeasure.Units, 10 );

		[DefaultValue( "Units 10" )]
		public Reference<UIMeasureValueDouble> Panel2MinSize
		{
			get { if( _panel2MinSize.BeginGet() ) Panel2MinSize = _panel2MinSize.Get( this ); return _panel2MinSize.value; }
			set { if( _panel2MinSize.BeginSet( ref value ) ) { try { Panel2MinSizeChanged?.Invoke( this ); } finally { _panel2MinSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Panel2MinSize"/> property value changes.</summary>
		public event Action<UISplitContainer> Panel2MinSizeChanged;
		ReferenceField<UIMeasureValueDouble> _panel2MinSize = new UIMeasureValueDouble( UIMeasure.Units, 10 );

		[Range( 0.0, 1.0 )]
		[DefaultValue( 0.5 )]
		public Reference<double> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( ref value ) ) { try { PositionChanged?.Invoke( this ); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<UISplitContainer> PositionChanged;
		ReferenceField<double> _position = 0.5;

		/////////////////////////////////////////

		public UISplitContainer()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 400 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			UpdatePanels();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdatePanels();
		}

		//Rectangle GetPagesParentRectangle()
		//{
		//	var side = Side.Value;

		//	if( side == SideEnum.None )
		//		return new Rectangle( 0, 0, 1.0, 1.0 );
		//	else
		//	{
		//		var buttons = GetAllButtons();
		//		if( buttons.Count > 0 )
		//		{
		//			var buttonSize = ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, buttons[ 0 ].GetScreenSize() ), UIMeasure.Parent );

		//			switch( side )
		//			{
		//			case SideEnum.Left: return new Rectangle( buttonSize.X, 0, 1.0, 1.0 );
		//			case SideEnum.Top: return new Rectangle( 0, buttonSize.Y, 1.0, 1.0 );
		//			case SideEnum.Right: return new Rectangle( 0, 0, 1.0 - buttonSize.X, 1.0 );
		//			case SideEnum.Bottom: return new Rectangle( 0, 0, 1.0, 1.0 - buttonSize.Y );
		//			}
		//		}
		//	}

		//	return new Rectangle( 0, 0, 0, 0 );
		//}

		public UIControl GetPanel1()
		{
			return GetComponent<UIControl>( "Panel 1" );
		}

		public UIControl GetPanel2()
		{
			return GetComponent<UIControl>( "Panel 2" );
		}

		void UpdatePanels()
		{
			//create panels if not exists
			if( GetPanel1() == null )
			{
				var panel = CreateComponent<UIControl>( enabled: false );
				panel.Name = "Panel 1";
				panel.BackgroundColor = new ColorValue( 0.5, 0.5, 0.5 );
				panel.Enabled = true;
			}
			if( GetPanel2() == null )
			{
				var panel = CreateComponent<UIControl>( enabled: false );
				panel.Name = "Panel 2";
				panel.BackgroundColor = new ColorValue( 0.5, 0.5, 0.5 );
				panel.Enabled = true;
			}

			//!!!!все свойства учитывать

			var splitterSize = SplitterSize.Value;

			var position = Math.Clamp( Position.Value, 0, 1 );

			if( Vertical )
			{
				GetScreenRectangle( out var rect );

				var splitterSizeScreen = GetScreenOffsetByValueX( splitterSize );

				var rect1 = rect;
				rect1.Right -= rect1.Size.X * position + splitterSizeScreen / 2;

				var panel1 = GetPanel1();
				panel1.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, rect1 );
				panel1.Size = new UIMeasureValueVector2( UIMeasure.Screen, rect1.Size );

				var rect2 = rect;
				rect2.Left += rect2.Size.X * ( 1.0 - position ) + splitterSizeScreen / 2;

				var panel2 = GetPanel2();
				panel2.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, rect2 );
				panel2.Size = new UIMeasureValueVector2( UIMeasure.Screen, rect2.Size );

				//Position.v


				//!!!!





			}
			else
			{
				//!!!!
			}


			//var side = Side.Value;
			//var pages = GetAllPages();

			////create panels
			//{
			//	zzzzzzz;

			//	var buttons = GetAllButtons();
			//	if( buttons.Count != pages.Count || needRecreateButtons )
			//	{
			//		foreach( var b in buttons )
			//			b.Dispose();

			//		if( side != SideEnum.None )
			//		{
			//			for( int n = 0; n < pages.Count; n++ )
			//			{
			//				var page = pages[ n ];

			//				UIButton button;
			//				if( Button.Value != null )
			//				{
			//					button = (UIButton)Button.Value.Clone();
			//					AddComponent( button );
			//				}
			//				else
			//					button = CreateComponent<UIButton>( enabled: false );

			//				button.Name = "Button " + page.Name;
			//				button.CanBeSelected = false;
			//				button.SaveSupport = false;
			//				button.CloneSupport = false;
			//				button.AnyData = n;
			//				button.Enabled = true;

			//				button.Click += delegate ( UIButton sender )
			//				{
			//					SelectedIndex = (int)sender.AnyData;
			//				};
			//			}
			//		}

			//		needRecreateButtons = false;
			//	}
			//}

			////update pages
			//{
			//	zzzzzzz;

			//	var pagesRectangle = GetPagesParentRectangle();

			//	for( int n = 0; n < pages.Count; n++ )
			//	{
			//		var page = pages[ n ];

			//		page.Visible = n == SelectedIndex;
			//		page.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, pagesRectangle.Left, pagesRectangle.Top, 0, 0 );
			//		page.Size = new UIMeasureValueVector2( UIMeasure.Parent, pagesRectangle.Size );
			//	}
			//}
		}

	}
}
