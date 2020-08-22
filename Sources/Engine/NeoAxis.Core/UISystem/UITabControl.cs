// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//!!!!Work in progress

namespace NeoAxis
{
	/// <summary>
	/// Manages a related set of tab pages.
	/// </summary>
	public class UITabControl : UIControl
	{
		[DefaultValue( null )]
		public Reference<UIButton> Button
		{
			get { if( _button.BeginGet() ) Button = _button.Get( this ); return _button.value; }
			set { if( _button.BeginSet( ref value ) ) { try { ButtonChanged?.Invoke( this ); } finally { _button.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Button"/> property value changes.</summary>
		public event Action<UITabControl> ButtonChanged;
		ReferenceField<UIButton> _button = null;

		public enum SideEnum
		{
			None,
			Left,
			Top,
			Right,
			Bottom
		}

		/// <summary>
		/// The side of navigation buttons
		/// </summary>
		[DefaultValue( SideEnum.Top )]
		public Reference<SideEnum> Side
		{
			get { if( _side.BeginGet() ) Side = _side.Get( this ); return _side.value; }
			set { if( _side.BeginSet( ref value ) ) { try { SideChanged?.Invoke( this ); } finally { _side.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Side"/> property value changes.</summary>
		public event Action<UITabControl> SideChanged;
		ReferenceField<SideEnum> _side = SideEnum.Top;

		/// <summary>
		/// The index of the selected page.
		/// </summary>
		[DefaultValue( 0 )]
		public int SelectedIndex
		{
			get
			{
				return selectedIndex;

				//var buttons = GetAllButtons();
				//for( int n = 0; n < buttons.Count; n++ )
				//{
				//	if( buttons[ n ].Highlighted.Value )
				//		return n;
				//}
				//return -1;
			}
			set
			{
				if( value == SelectedIndex )
					return;

				selectedIndex = value;

				//update buttons
				var buttons = GetAllButtons();
				for( int n = 0; n < buttons.Count; n++ )
					buttons[ n ].Highlighted = n == value;

				SelectedIndexChanged?.Invoke( this );
			}
		}
		int selectedIndex;

		public delegate void SelectedIndexChangedDelegate( UITabControl sender );
		public event SelectedIndexChangedDelegate SelectedIndexChanged;

		/// <summary>
		/// Gets a selected page.
		/// </summary>
		[Browsable( false )]
		public UIControl SelectedPage
		{
			get
			{
				var pages = GetAllPages();
				var index = SelectedIndex;
				if( index >= 0 && index < pages.Count )
					return pages[ index ];
				else
					return null;
			}
		}

		/////////////////////////////////////////

		public UITabControl()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 800, 600 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			//!!!!
			//!!!!в стиль?
			BackgroundColor = new ColorValue( 0, 0, 0 );

			for( int n = 1; n <= 3; n++ )
			{
				var page = CreateComponent<UIControl>();
				page.Name = "Page " + n.ToString();
				page.Text = n.ToString();

				if( n == 1 )
					page.BackgroundColor = new ColorValue( 0.5, 0, 0, 1 );
				else if( n == 2 )
					page.BackgroundColor = new ColorValue( 0, 0.5, 0, 1 );
				else
					page.BackgroundColor = new ColorValue( 0, 0, 0.5, 1 );
			}

			UpdateControls();
		}

		Rectangle GetPagesParentRectangle()
		{
			var side = Side.Value;
			if( side != SideEnum.None )
			{
				//!!!!
				UIButton button = null;

				if( button != null )
				{
					var buttonSize = ConvertScreenToLocal( button.GetScreenSize() );

					switch( side )
					{
					case SideEnum.Left: return new Rectangle( buttonSize.X, 0, 1.0 - buttonSize.X, 1.0 );
					case SideEnum.Top: return new Rectangle( 0, buttonSize.Y, 1.0, 1.0 - buttonSize.Y );
					case SideEnum.Right: return new Rectangle( 0, 0, 1.0 - buttonSize.X, 1.0 );
					case SideEnum.Bottom: return new Rectangle( 0, 0, 1.0, 1.0 - buttonSize.Y );
					}

					//var size = GetScreenSize();
					//var buttonSize = button.GetScreenSize();

					//switch( side )
					//{
					//case SideEnum.Left: return new Rectangle( buttonSize.X, 0, size.X - buttonSize.X, size.Y );
					//case SideEnum.Top: return new Rectangle( 0, buttonSize.Y, size.X, size.Y - buttonSize.Y );
					//case SideEnum.Right: return new Rectangle( 0, 0, size.X - buttonSize.X, size.Y );
					//case SideEnum.Bottom: return new Rectangle( 0, 0, size.X, size.Y - buttonSize.Y );
					//}
				}
			}

			return new Rectangle( 0, 0, 1, 1 );
		}

		void UpdateControls()
		{
			var side = Side.Value;
			var pages = GetAllPages();

			//update buttons
			{
				var buttons = GetAllButtons();

				//recreate buttons
				if( buttons.Count != pages.Count )
				{
					//szzzzzzz;

					//!!!!Button property

					//buttons = GetAllButtons();


					//!!!!click events
				}

				//update buttons
				if( pages.Count == buttons.Count && buttons.Count > 0 )
				{
					var buttonSize = buttons[ 0 ].GetScreenSize();

					for( int n = 0; n < buttons.Count; n++ )
					{
						var button = buttons[ n ];

						button.Text = pages[ n ].Text;
						button.Highlighted = n == SelectedIndex;

						//location

						//!!!!Side

						//!!!!UIMeasure.Screen

						button.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, buttonSize.X * n, 0, 0, 0 );
					}
				}
			}

			var selectedIndex = SelectedIndex;
			var pagesRectangle = GetPagesParentRectangle();

			//update pages
			for( int n = 0; n < pages.Count; n++ )
			{
				var page = pages[ n ];

				//visibility
				page.Visible = n == selectedIndex;

				//location
				page.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, pagesRectangle.Left, pagesRectangle.Top, 0, 0 );
				page.Size = new UIMeasureValueVector2( UIMeasure.Parent, pagesRectangle.Size );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//!!!!тут?
			UpdateControls();
		}

		public IList<UIControl> GetAllPages()
		{
			var result = new List<UIControl>();
			foreach( var control in GetComponents<UIControl>( onlyEnabledInHierarchy: true ) )
			{
				if( control.Name.Length >= 4 && control.Name.Substring( 0, 4 ) == "Page" )
					result.Add( control );
			}
			return result;
		}

		public IList<UIButton> GetAllButtons()
		{
			var result = new List<UIButton>();
			foreach( var control in GetComponents<UIButton>( onlyEnabledInHierarchy: true ) )
			{
				if( control.Name.Length >= 7 && control.Name.Substring( 0, 7 ) == "Button " )
					result.Add( control );
			}
			return result;
		}

	}
}
