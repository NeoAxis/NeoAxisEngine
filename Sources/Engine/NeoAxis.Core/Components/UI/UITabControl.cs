// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a control that contains multiple items that share the same space on the screen.
	/// </summary>
	public class UITabControl : UIControl
	{
		bool needRecreateButtons = true;

		//

		/// <summary>
		/// The class of the buttons. Use 'this:' reference to fix potential issues.
		/// </summary>
		[DefaultValue( null )]
		public Reference<UIButton> Button
		{
			get { if( _button.BeginGet() ) Button = _button.Get( this ); return _button.value; }
			set
			{
				if( _button.BeginSet( ref value ) )
				{
					try
					{
						ButtonChanged?.Invoke( this );
						needRecreateButtons = true;
					}
					finally { _button.EndSet(); }
				}
			}
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
		/// Indents between buttons.
		/// </summary>
		[DefaultValue( "Units 4 4" )]
		public Reference<UIMeasureValueVector2> ButtonIndents
		{
			get { if( _buttonIndents.BeginGet() ) ButtonIndents = _buttonIndents.Get( this ); return _buttonIndents.value; }
			set { if( _buttonIndents.BeginSet( ref value ) ) { try { ButtonIndentsChanged?.Invoke( this ); } finally { _buttonIndents.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonIndents"/> property value changes.</summary>
		public event Action<UITabControl> ButtonIndentsChanged;
		ReferenceField<UIMeasureValueVector2> _buttonIndents = new UIMeasureValueVector2( UIMeasure.Units, 4, 4 );

		/// <summary>
		/// The index of the selected page.
		/// </summary>
		[DefaultValue( 0 )]
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				if( value == SelectedIndex )
					return;

				selectedIndex = value;

				UpdateControls();

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

			//create pages by default
			for( int n = 1; n <= 3; n++ )
			{
				var page = CreateComponent<UIControl>();
				page.Name = "Page " + n.ToString();
				page.Text = n.ToString();

				if( n == 1 )
					page.BackgroundColor = new ColorValue( 0.5, 0, 0 );
				else if( n == 2 )
					page.BackgroundColor = new ColorValue( 0, 0.5, 0 );
				else
					page.BackgroundColor = new ColorValue( 0, 0, 0.5 );
			}

			UpdateControls();
		}

		Rectangle GetPagesParentRectangle()
		{
			var side = Side.Value;

			if( side == SideEnum.None )
				return new Rectangle( 0, 0, 1.0, 1.0 );
			else
			{
				var buttons = GetAllButtons();
				if( buttons.Count > 0 )
				{
					var buttonSize = ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, buttons[ 0 ].GetScreenSize() ), UIMeasure.Parent );
					var buttonIndents = ConvertOffset( ButtonIndents, UIMeasure.Parent );

					switch( side )
					{
					case SideEnum.Left: return new Rectangle( buttonSize.X + buttonIndents.X, 0, 1.0, 1.0 );
					case SideEnum.Top: return new Rectangle( 0, buttonSize.Y + buttonIndents.Y, 1.0, 1.0 );
					case SideEnum.Right: return new Rectangle( 0, 0, 1.0 - buttonSize.X - buttonIndents.X, 1.0 );
					case SideEnum.Bottom: return new Rectangle( 0, 0, 1.0, 1.0 - buttonSize.Y - buttonIndents.Y );
					}
				}
			}

			return new Rectangle( 0, 0, 0, 0 );
		}

		public void UpdateControls()
		{
			var side = Side.Value;
			var pages = GetAllPages();

			//recreate buttons
			{
				var buttons = GetAllButtons();
				if( buttons.Count != pages.Count || needRecreateButtons )
				{
					foreach( var b in buttons )
						b.Dispose();

					if( side != SideEnum.None )
					{
						for( int n = 0; n < pages.Count; n++ )
						{
							var page = pages[ n ];

							UIButton button;
							if( Button.Value != null )
							{
								button = (UIButton)Button.Value.Clone();
								AddComponent( button );
							}
							else
								button = CreateComponent<UIButton>( enabled: false );

							button.Name = "Button " + page.Name;
							button.CanBeSelected = false;
							button.SaveSupport = false;
							button.CloneSupport = false;
							button.AnyData = n;
							button.Enabled = true;

							button.Click += delegate ( UIButton sender )
							{
								SelectedIndex = (int)sender.AnyData;
							};
						}
					}

					needRecreateButtons = false;
				}
			}

			//update buttons
			{
				var buttons = GetAllButtons();
				if( buttons.Count == pages.Count && buttons.Count > 0 )
				{
					var buttonSize = ConvertOffset( new UIMeasureValueVector2( UIMeasure.Screen, buttons[ 0 ].GetScreenSize() ), UIMeasure.Parent );
					var buttonIndents = ConvertOffset( ButtonIndents, UIMeasure.Parent );

					for( int n = 0; n < buttons.Count; n++ )
					{
						var button = buttons[ n ];
						var page = pages[ n ];

						var text = page.Text;
						if( string.IsNullOrEmpty( text ) )
							text = page.Name;
						button.Text = text;

						button.Highlighted = n == SelectedIndex;

						switch( side )
						{
						case SideEnum.Left:
							button.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, 0, ( buttonSize.Y + buttonIndents.Y ) * n, 0, 0 );
							break;
						case SideEnum.Top:
							button.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, ( buttonSize.X + buttonIndents.X ) * n, 0, 0, 0 );
							break;
						case SideEnum.Right:
							button.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, 1.0 - buttonSize.X, ( buttonSize.Y + buttonIndents.Y ) * n, 0, 0 );
							break;
						case SideEnum.Bottom:
							button.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, ( buttonSize.X + buttonIndents.X ) * n, 1.0 - buttonSize.Y, 0, 0 );
							break;
						}
					}
				}
			}

			//update pages
			{
				var pagesRectangle = GetPagesParentRectangle();

				for( int n = 0; n < pages.Count; n++ )
				{
					var page = pages[ n ];

					page.Visible = n == SelectedIndex;
					page.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, pagesRectangle.Left, pagesRectangle.Top, 0, 0 );
					page.Size = new UIMeasureValueVector2( UIMeasure.Parent, pagesRectangle.Size );
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

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
