// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a drop down combo box.
	/// </summary>
	public class UICombo : UIControl
	{
		bool disableListSelectedIndexChangedEvent;

		double remainingTimeToClose;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to stay the list open.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AlwaysOpen
		{
			get { if( _alwaysOpen.BeginGet() ) AlwaysOpen = _alwaysOpen.Get( this ); return _alwaysOpen.value; }
			set { if( _alwaysOpen.BeginSet( ref value ) ) { try { AlwaysOpenChanged?.Invoke( this ); } finally { _alwaysOpen.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysOpen"/> property value changes.</summary>
		public event Action<UICombo> AlwaysOpenChanged;
		ReferenceField<bool> _alwaysOpen = false;

		/// <summary>
		/// Default value of the text box when none of the items selected.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> TextWhenNoSelectedItems
		{
			get { if( _textWhenNoSelectedItems.BeginGet() ) TextWhenNoSelectedItems = _textWhenNoSelectedItems.Get( this ); return _textWhenNoSelectedItems.value; }
			set { if( _textWhenNoSelectedItems.BeginSet( ref value ) ) { try { TextWhenNoSelectedItemsChanged?.Invoke( this ); } finally { _textWhenNoSelectedItems.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextWhenNoSelectedItems"/> property value changes.</summary>
		public event Action<UICombo> TextWhenNoSelectedItemsChanged;
		ReferenceField<string> _textWhenNoSelectedItems = "";

		//!!!!string? но нужен редактор
		/// <summary>
		/// The list of items.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public List<string> Items { get; set; } = new List<string>();

		/// <summary>
		/// The index of the selected item.
		/// </summary>
		[DefaultValue( 0 )]
		//[Browsable( false )]
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				if( selectedIndex == value )
					return;
				selectedIndex = value;
				SelectedIndexChanged?.Invoke( this );
			}
		}
		int selectedIndex;//= -1;

		public delegate void SelectedIndexChangedDelegate( UICombo sender );
		public event SelectedIndexChangedDelegate SelectedIndexChanged;

		/// <summary>
		/// The selected item.
		/// </summary>
		[Browsable( false )]
		public string SelectedItem
		{
			get
			{
				if( SelectedIndex >= 0 && SelectedIndex < Items.Count )
					return Items[ SelectedIndex ];
				return null;
			}
		}

		/////////////////////////////////////////

		public UICombo()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 40 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			{
				var obj = CreateComponent<UIEdit>();
				obj.Name = "Text Control";
				obj.Text = ReferenceUtility.MakeThisReference( obj, this, "DisplayText" );
				//obj.TextHorizontalAlignment = EHorizontalAlignment.Left;
				obj.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );
				obj.CanBeSelected = false;
				obj.HorizontalAlignment = EHorizontalAlignment.Stretch;
				obj.VerticalAlignment = EVerticalAlignment.Stretch;
				obj.ReadOnly = true;
				//obj.Offset = new UIMeasureValueVector2( UIMeasure.Units, 4, 0 );
				//obj.ClipRectangle = true;

				//UIStyle.EditTextMargin
				//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 4, 2, 4, 2 );
				//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );

				obj.NewObjectSetDefaultConfiguration();

				obj.MouseDown += TextControl_MouseDown;
				obj.Touch += TextControl_Touch;

				//create down button
				{
					var image = obj.CreateComponent<UIImage>();
					image.SourceImage = new Reference<ImageComponent>( null, @"Base\UI\Images\ComboBoxButton.png" );
					image.HorizontalAlignment = EHorizontalAlignment.Right;
					image.Size = new UIMeasureValueVector2( UIMeasure.Screen, new Vector2( 0, 0 ) );
					image.CanBeSelected = false;
				}
			}

			//{
			//	var obj = CreateComponent<UIButton>();
			//	obj.Name = "Down Button";

			//	//is not work. strange
			//	//obj.Text = ReferenceUtility.MakeThisReference( obj, this, "DisplayText" );//"Text" );

			//	//obj.Margin = new UIMeasureValueRectangle(UIMeasure.Parent, 
			//	//obj.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );

			//	obj.CanBeSelected = false;
			//	//obj.HorizontalAlignment = EHorizontalAlignment.Stretch;
			//	//obj.VerticalAlignment = EVerticalAlignment.Stretch;
			//	//obj.Offset = new UIMeasureValueVector2( UIMeasure.Units, 4, 0 );

			//	//UIStyle.EditTextMargin
			//	//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 4, 2, 4, 2 );
			//	//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );

			//}

			{
				var list = CreateComponent<UIList>();
				list.Name = "Down List";
				list.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, 0, 1, 1, 1 );
				list.Size = new UIMeasureValueVector2( UIMeasure.Screen, new Vector2( 0, 0 ) );
				list.CanBeSelected = false;
				list.Visible = false;
				list.TopMost = true;
				//obj.ZOrder = 1;

				list.NewObjectSetDefaultConfiguration();

				list.SelectedIndexChanged += List_SelectedIndexChanged;
				list.KeyDown += List_KeyDown;
				list.ItemMouseClick += List_ItemMouseClick;

				//control to hide the list when clicked outside control
				{
					var obj2 = list.CreateComponent<UIControl>();
					obj2.Name = "Cover";
					obj2.CanBeSelected = false;
					obj2.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, 0, 0, 1, 1 );
					obj2.Size = new UIMeasureValueVector2( UIMeasure.Screen, 1, 1 );
					obj2.MouseDown += ListCover_MouseDown;
					obj2.Touch += ListCover_Touch;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var textControl = GetTextControl();
			if( textControl != null )
			{
				if( EnabledInHierarchy )
				{
					textControl.MouseDown += TextControl_MouseDown;
					textControl.Touch += TextControl_Touch;
				}
				else
				{
					textControl.MouseDown -= TextControl_MouseDown;
					textControl.Touch -= TextControl_Touch;
				}
			}

			var list = GetDownList();
			if( list != null )
			{
				if( EnabledInHierarchy )
				{
					list.SelectedIndexChanged += List_SelectedIndexChanged;
					list.KeyDown += List_KeyDown;
					list.ItemMouseClick += List_ItemMouseClick;

					var cover = list.GetComponent( "Cover" ) as UIControl;
					if( cover != null )
					{
						cover.MouseDown += ListCover_MouseDown;
						cover.Touch += ListCover_Touch;
					}
				}
				else
				{
					list.SelectedIndexChanged -= List_SelectedIndexChanged;
					list.KeyDown -= List_KeyDown;
					list.ItemMouseClick -= List_ItemMouseClick;

					var cover = list.GetComponent( "Cover" ) as UIControl;
					if( cover != null )
					{
						cover.MouseDown -= ListCover_MouseDown;
						cover.Touch -= ListCover_Touch;
					}
				}
			}
		}

		/// <summary>
		/// Whether control can be focused.
		/// </summary>
		[Browsable( false )]
		public override bool CanFocus
		{
			get { return EnabledInHierarchy && VisibleInHierarchy && !ReadOnlyInHierarchy; }
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( Focused )
			{
				switch( e.Key )
				{
				case EKeys.Space:
				case EKeys.Right:
					{
						var list = GetDownList();
						if( list != null )
						{
							var show = !list.Visible;
							list.Visible = show;
							if( list.Visible )
								list.Focus();
						}
					}
					return true;

				case EKeys.Up:
					{
						var index = SelectedIndex - 1;
						if( index < 0 )
							index = 0;
						SelectedIndex = index;
					}
					return true;

				case EKeys.Down:
					{
						var index = SelectedIndex + 1;
						if( index >= Items.Count )
							index = Items.Count - 1;
						SelectedIndex = index;
					}
					return true;

				case EKeys.Home:
					SelectedIndex = 0;
					return true;

				case EKeys.End:
					SelectedIndex = Items.Count - 1;
					return true;
				}
			}

			return base.OnKeyDown( e );
		}

		private void TextControl_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		{
			if( button == EMouseButtons.Left )
			{
				Focus();

				var list = GetDownList();
				if( list != null )
				{
					var show = !list.Visible && !ReadOnlyInHierarchy;
					list.Visible = show;
					//list.Capture = show;
				}
			}
		}

		private void TextControl_Touch( UIControl sender, TouchData e, ref bool handled )
		{
			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( VisibleInHierarchy && EnabledInHierarchy )//&& !ReadOnlyInHierarchy )//&& touchDown == null )
				{
					GetScreenRectangle( out var rect );
					var rectInPixels = rect * ParentContainer.Viewport.SizeInPixels.ToVector2();
					var distanceInPixels = rectInPixels.GetPointDistance( e.PositionInPixels.ToVector2() );

					var item = new TouchData.TouchDownRequestToProcessTouch( this, 0, distanceInPixels, null,
						delegate ( UIControl sender2, TouchData touchData, object anyData )
						{
							//touch
							var list = GetDownList();
							if( list != null )
							{
								var show = !list.Visible && !ReadOnlyInHierarchy;
								list.Visible = show;
								//list.Capture = show;
							}
						} );
					e.TouchDownRequestToControlActions.Add( item );
				}
				break;

				//case TouchData.ActionEnum.Up:
				//	break;

				//case TouchData.ActionEnum.Move:
				//	break;

				//case TouchData.ActionEnum.Cancel:
				//	break;

				//case TouchData.ActionEnum.Outside:
				//	break;
			}
		}

		private void List_SelectedIndexChanged( UIList sender )
		{
			if( disableListSelectedIndexChangedEvent )
				return;

			if( SelectedIndex != sender.SelectedIndex )
				SelectedIndex = sender.SelectedIndex;
		}

		private void List_KeyDown( UIControl sender, KeyEvent e, ref bool handled )
		{
			if( sender.Focused )
			{
				if( e.Key == EKeys.Left || e.Key == EKeys.Return || e.Key == EKeys.Space )
				{
					Focus();
					if( remainingTimeToClose == 0 )
						remainingTimeToClose = 0.2;
					handled = true;
				}
			}
		}

		private void List_ItemMouseClick( UIControl sender, EMouseButtons button, ref bool handled )
		{
			var list = GetDownList();
			if( list != null )
			{
				list.Visible = false;
				Focus();
				handled = true;
			}
		}

		private void ListCover_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		{
			if( button == EMouseButtons.Left || button == EMouseButtons.Right )
			{
				var list = GetDownList();
				if( list != null && !new Rectangle( 0, 0, 1, 1 ).Contains( list.MousePosition ) )
				{
					list.Visible = false;
					handled = true;
				}
			}
		}

		private void ListCover_Touch( UIControl sender, TouchData e, ref bool handled )
		{
			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( VisibleInHierarchy && EnabledInHierarchy )//&& !ReadOnlyInHierarchy && touchDown == null )
				{
					var list = GetDownList();
					if( list != null && !list.GetScreenRectangle().Contains( e.Position ) )
					{
						var item = new TouchData.TouchDownRequestToProcessTouch( this, 10, 0, null,
							delegate ( UIControl sender2, TouchData touchData, object anyData )
							{
								list.Visible = false;
							} );
						e.TouchDownRequestToControlActions.Add( item );
					}
				}
				break;

				//case TouchData.ActionEnum.Up:
				//	break;

				//case TouchData.ActionEnum.Move:
				//	break;

				//case TouchData.ActionEnum.Cancel:
				//	break;

				//case TouchData.ActionEnum.Outside:
				//	break;
			}
		}

		public UIEdit GetTextControl()
		{
			return GetComponentByPath( "Text Control" ) as UIEdit;
		}

		//public UIButton GetDownButton()
		//{
		//	return GetComponentByPath( "Down Button" ) as UIButton;
		//}

		public UIList GetDownList()
		{
			return GetComponentByPath( "Down List" ) as UIList;
		}

		[Browsable( false )]
		public string DisplayText
		{
			get
			{
				var text = SelectedItem;
				if( text == null )
					text = TextWhenNoSelectedItems;
				return text;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( remainingTimeToClose > 0 )
			{
				remainingTimeToClose -= delta;
				if( remainingTimeToClose <= 0 )
				{
					remainingTimeToClose = 0;

					var list = GetDownList();
					if( list != null )
					{
						list.Visible = false;
						//list.Capture = false;
					}
				}
			}
		}

		public void RenderDefaultStyle( CanvasRenderer renderer )
		{
			//update Down Button image
			var textControl = GetTextControl();
			if( textControl != null )
			{
				var image = textControl.GetComponent<UIImage>();
				if( image != null )
				{
					textControl.GetScreenRectangle( out var rect );
					image.Size = new UIMeasureValueVector2( UIMeasure.Screen, new Vector2( rect.GetSize().Y / ParentContainer.AspectRatio, rect.GetSize().Y ) );
				}
			}

			var list = GetDownList();
			if( list != null )
			{
				GetScreenRectangle( out var rect );

				if( AlwaysOpen )
					list.Visible = true;

				//update items
				disableListSelectedIndexChangedEvent = true;
				try
				{
					list.Items = new List<string>( Items );
					list.SelectedIndex = SelectedIndex;
				}
				finally
				{
					disableListSelectedIndexChangedEvent = false;
				}


				//!!!!когда внизу на экране не влазит, то сверху показывать

				var itemSize = list.ConvertOffsetY( list.ItemSize, UIMeasure.Screen );

				var height = itemSize * ( list.Items.Count + 0.5 );

				//!!!!
				if( height > 0.3 )
					height = 0.3;

				list.Size = new UIMeasureValueVector2( UIMeasure.Screen, rect.Size.X, height );
			}
		}




		/////////////////////////////////////////////

		///// <summary>
		///// The control associated with item list in the combo box.
		///// </summary>
		//[Serialize]
		//[Browsable( false )]
		//public UIControl DownListControl
		//{
		//	get { return downListControl; }
		//	set
		//	{
		//		if( downListControl != null )
		//			RemoveComponent( downListControl, false );
		//		downListControl = value;
		//		if( downListControl != null )
		//		{
		//			downListControl.Enabled = Enabled;
		//			if( downListControl.Parent == null )
		//				AddComponent( downListControl );
		//			downListControl.Visible = false;
		//			downListControl.TopMost = true;
		//		}
		//	}
		//}

		//void DownButton_Click( object sender )
		//{
		//	if( downListControl == null || itemButton == null )
		//		return;

		//	downListControl.Visible = !downListControl.Visible;

		//	if( downListControl.Visible )
		//	{
		//		if( items.Count != itemButtons.Count )
		//		{
		//			DestroyItemButtons();

		//			double itemScreenSizeY = ItemButton.GetScreenSize().Y;
		//			double screenSizeY = itemScreenSizeY * items.Count;

		//			//set position and size

		//			Vec2 screenPos = GetScreenRectangle().LeftBottom;
		//			if( screenPos.Y + screenSizeY > .99f )
		//			{
		//				screenPos.Y = GetScreenPosition().Y - screenSizeY;
		//				if( screenPos.Y < 0 )
		//					screenPos.Y = .99f - screenSizeY;
		//			}
		//			downListControl.Position = new ScaleValue( ScaleType.Screen, screenPos );
		//			//downListControl.Position = new ScaleValue( ScaleType.Parent, new Vec2( 0, 1 ) );

		//			downListControl.Size = new ScaleValue( ScaleType.Screen,
		//				new Vec2( GetScreenSize().X, screenSizeY ) );

		//			//create items

		//			double screenPositionY = downListControl.GetScreenPosition().Y;

		//			for( int index = 0; index < items.Count; index++ )
		//			{
		//				UIButton button = (UIButton)itemButton.Clone();
		//				//button.FileNameCreated = null;
		//				//button.FileNameDeclared = null;

		//				button.Enabled = true;
		//				button.UserData = index;
		//				button.Position = new ScaleValue( ScaleType.Screen,
		//					new Vec2( itemButton.GetScreenPosition().X, screenPositionY ) );
		//				button.Click += ItemButton_Click;
		//				button.TopMost = true;
		//				button.SaveSupport = false;
		//				button.CanClone = false;

		//				AddComponent( button );

		//				itemButtons.Add( button );

		//				screenPositionY += itemScreenSizeY;
		//			}
		//		}

		//		for( int n = 0; n < items.Count; n++ )
		//		{
		//			UIButton button = itemButtons[ n ];

		//			button.Visible = true;
		//			button.Text = items[ n ].ToString();
		//		}

		//		Capture = true;
		//	}
		//	else
		//	{
		//		for( int n = 0; n < itemButtons.Count; n++ )
		//			itemButtons[ n ].Visible = false;

		//		Capture = false;
		//	}

		//}

		//protected override bool OnMouseDown( EMouseButtons button )
		//{
		//	if( Capture && downListControl.Visible )
		//	{
		//		if( !new Rect( 0, 0, 1, 1 ).Contains( MousePosition ) &&
		//			!new Rect( 0, 0, 1, 1 ).Contains( DownListControl.MousePosition ) )
		//		{
		//			DownButton_Click( null );
		//			Capture = false;
		//		}
		//	}
		//	return base.OnMouseDown( button );
		//}

		public int SelectItem( string text )
		{
			for( int n = 0; n < Items.Count; n++ )
			{
				if( Items[ n ] == text )
				{
					SelectedIndex = n;
					return n;
				}
			}
			return -1;
		}

	}
}
