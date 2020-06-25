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

				//create down button
				{
					var image = obj.CreateComponent<UIImage>();
					image.SourceImage = new Reference<Component_Image>( null, @"Base\UI\Images\ComboBoxButton.png" );
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
				var obj = CreateComponent<UIList>();
				obj.Name = "Down List";
				obj.Margin = new UIMeasureValueRectangle( UIMeasure.Parent, 0, 1, 1, 1 );
				obj.Size = new UIMeasureValueVector2( UIMeasure.Screen, new Vector2( 0, 0 ) );
				obj.CanBeSelected = false;
				obj.Visible = false;
				obj.TopMost = true;
				//obj.ZOrder = 1;

				obj.NewObjectSetDefaultConfiguration();

				obj.SelectedIndexChanged += List_SelectedIndexChanged;
				//obj.MouseDown += List_MouseDown;

				//control to hide the list when clicked outside control
				{
					var obj2 = obj.CreateComponent<UIControl>();
					obj2.Name = "Cover";
					obj2.CanBeSelected = false;
					obj2.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, 0, 0, 1, 1 );
					obj2.Size = new UIMeasureValueVector2( UIMeasure.Screen, 1, 1 );
					obj2.MouseDown += ListCover_MouseDown;
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
					textControl.MouseDown += TextControl_MouseDown;
				else
					textControl.MouseDown -= TextControl_MouseDown;
			}

			var list = GetDownList();
			if( list != null )
			{
				if( EnabledInHierarchy )
				{
					list.SelectedIndexChanged += List_SelectedIndexChanged;
					//list.MouseDown += List_MouseDown;

					var cover = list.GetComponent( "Cover" ) as UIControl;
					if( cover != null )
						cover.MouseDown += ListCover_MouseDown;
				}
				else
				{
					list.SelectedIndexChanged -= List_SelectedIndexChanged;
					//list.MouseDown -= List_MouseDown;

					var cover = list.GetComponent( "Cover" ) as UIControl;
					if( cover != null )
						cover.MouseDown -= ListCover_MouseDown;
				}
			}

		}

		private void TextControl_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		{
			if( button == EMouseButtons.Left )
			{
				var list = GetDownList();
				if( list != null )
				{
					var show = !list.Visible;
					list.Visible = show;
					//list.Capture = show;
				}
			}
		}

		private void List_SelectedIndexChanged( UIList sender )
		{
			if( disableListSelectedIndexChangedEvent )
				return;

			SelectedIndex = sender.SelectedIndex;
			if( remainingTimeToClose == 0 )
				remainingTimeToClose = 0.2;
			//sender.Visible = false;
		}

		//private void List_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		//{
		//if( button == EMouseButtons.Left && !new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
		//{
		//	var list = GetDownList();
		//	if( list != null )
		//	{
		//		list.Visible = false;
		//		//list.Capture = false;
		//	}
		//}
		//}

		private void ListCover_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		{
			if( button == EMouseButtons.Left || button == EMouseButtons.Right )
			{
				var list = GetDownList();
				if( list != null )
				{
					if( !new Rectangle( 0, 0, 1, 1 ).Contains( list.MousePosition ) )
					{
						list.Visible = false;
						handled = true;
					}
				}
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


	}
}
