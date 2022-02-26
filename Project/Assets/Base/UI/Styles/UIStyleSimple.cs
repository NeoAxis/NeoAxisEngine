// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class UIStyleSimple : UIStyleDefault
	{
		protected override void OnRenderButton( UIButton control, CanvasRenderer renderer )
		{
			if( control.Parent as UIContextMenu != null )
			{
				//context menu button

				var styleColor = ColorValue.Zero;
				switch( control.State )
				{
				case UIButton.StateEnum.Normal: styleColor = new ColorValue( 0.1, 0.1, 0.7 ); break;
				case UIButton.StateEnum.Hover: styleColor = new ColorValue( 0.175, 0.175, 0.75 ); break;
				case UIButton.StateEnum.Pushed: styleColor = new ColorValue( 0.25, 0.25, 0.9 ); break;
				case UIButton.StateEnum.Highlighted: styleColor = new ColorValue( 0.6, 0.6, 0 ); break;
				case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.1, 0.1, 0.7 ); break;//new ColorValue( 0.5, 0.5, 0.5 ); break;
				}

				control.GetScreenRectangle( out var rect );
				var color = styleColor.GetSaturate();
				if( color.Alpha > 0 )
				{
					//back
					renderer.AddQuad( rect, color );

					//!!!!image

					//text
					var position = new Vector2( rect.Left + control.GetScreenOffsetByValueX( new UIMeasureValueDouble( UIMeasure.Units, 10 ) ), rect.GetCenter().Y ) + new Vector2( 0, renderer.DefaultFontSize / 10 );
					var textColor = control.State == UIButton.StateEnum.Disabled ? new ColorValue( 0.7, 0.7, 0.7 ) : new ColorValue( 1, 1, 1 );
					renderer.AddText( control.Text, position, EHorizontalAlignment.Left, EVerticalAlignment.Center, textColor );
				}
			}
			else
			{
				//usual button

				var styleColor = ColorValue.Zero;
				switch( control.State )
				{
				case UIButton.StateEnum.Normal: styleColor = new ColorValue( 0.1, 0.1, 0.6 ); break;
				case UIButton.StateEnum.Hover: styleColor = new ColorValue( 0.175, 0.175, 0.75 ); break;
				case UIButton.StateEnum.Pushed: styleColor = new ColorValue( 0.25, 0.25, 0.9 ); break;
				case UIButton.StateEnum.Highlighted: styleColor = new ColorValue( 0.6, 0.6, 0 ); break;
				case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.5, 0.5, 0.5 ); break;
				}

				control.GetScreenRectangle( out var rect );
				var color = styleColor.GetSaturate();
				if( color.Alpha > 0 )
				{
					//back
					renderer.AddQuad( rect, color );

					//image
					if( control.Image.Value != null )
					{
						var image = control.Image.Value;
						if( control.ReadOnly && control.ImageDisabled.Value != null )
							image = control.ImageDisabled.Value;

						var imageRect = rect;
						imageRect.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );
						renderer.AddQuad( imageRect, new Rectangle( 0, 0, 1, 1 ), image, new ColorValue( 1, 1, 1 ), true );
					}

					//text
					var position = rect.GetCenter() + new Vector2( 0, renderer.DefaultFontSize / 10 );
					var textColor = control.State == UIButton.StateEnum.Disabled ? new ColorValue( 0.7, 0.7, 0.7 ) : new ColorValue( 1, 1, 1 );
					renderer.AddText( control.Text, position, EHorizontalAlignment.Center, EVerticalAlignment.Center, textColor );
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRenderCheck( UICheck control, CanvasRenderer renderer )
		{
			base.OnRenderCheck( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderEdit( UIEdit control, CanvasRenderer renderer )
		{
			base.OnRenderEdit( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderText( UIText control, CanvasRenderer renderer )
		{
			base.OnRenderText( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderScroll( UIScroll control, CanvasRenderer renderer )
		{
			base.OnRenderScroll( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderList( UIList control, CanvasRenderer renderer )
		{
			base.OnRenderList( control, renderer );
		}

		public override int GetListItemIndexByScreenPosition( UIList control, Vector2 position )
		{
			return base.GetListItemIndexByScreenPosition( control, position );
		}

		/////////////////////////////////////////

		protected override void OnRenderWindow( UIWindow control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0.05, 0.05, 0.25 ) );

			var rect2 = rect;
			rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );

			var color = new ColorValue( 0.25, 0.25, 0.75 );
			renderer.AddQuad( new Rectangle( rect.Left, rect.Top, rect2.Left, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect2.Left, rect.Top, rect2.Right, rect2.Top ), color );
			renderer.AddQuad( new Rectangle( rect2.Right, rect.Top, rect.Right, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect.Left, rect2.Bottom, rect2.Right, rect.Bottom ), color );

			if( control.TitleBar.Value )
			{
				double titleBarHeight = 30;
				double screenY = rect.Top + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 0, titleBarHeight ) ).Y;
				double screenY2 = screenY + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 0, 4 ) ).Y;

				var rect3 = new Rectangle( rect2.Left, rect2.Top, rect2.Right, screenY2 );
				renderer.AddQuad( rect3, color );

				if( !string.IsNullOrEmpty( control.Text ) )
				{
					var pos = new Vector2( rect.GetCenter().X, ( rect2.Top + screenY ) / 2 );
					renderer.AddText( control.Text, pos, EHorizontalAlignment.Center, EVerticalAlignment.Center, new ColorValue( 1, 1, 1 ) );
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRenderProgress( UIProgress control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0.05, 0.05, 0.3 ) );

			if( control.Maximum.Value != 0 )
			{
				double progress = control.Value.Value / control.Maximum.Value;
				if( progress > 0 )
				{
					var rect2 = rect;
					rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );
					rect2.Right = MathEx.Lerp( rect2.Left, rect2.Right, progress );

					renderer.AddQuad( rect2, new ColorValue( 1, 1, 1 ) );
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnRenderSlider( UISlider control, CanvasRenderer renderer )
		{
			base.OnRenderSlider( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderGrid( UIGrid control, CanvasRenderer renderer )
		{
			base.OnRenderGrid( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderCombo( UICombo control, CanvasRenderer renderer )
		{
			base.OnRenderCombo( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderTooltip( UITooltip tooltip, CanvasRenderer renderer )
		{
			base.OnRenderTooltip( tooltip, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderContextMenu( UIContextMenu menu, CanvasRenderer renderer )
		{
			base.OnRenderContextMenu( menu, renderer );

			//draw background
			renderer.AddQuad( menu.GetScreenRectangle(), new ColorValue( 0.1, 0.1, 0.7 ) );
		}

		/////////////////////////////////////////

		protected override void OnRenderToolbar( UIToolbar control, CanvasRenderer renderer )
		{
			base.OnRenderToolbar( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderSplitContainer( UISplitContainer control, CanvasRenderer renderer )
		{
			base.OnRenderSplitContainer( control, renderer );
		}

		/////////////////////////////////////////

		protected override void OnRenderTabControl( UITabControl control, CanvasRenderer renderer )
		{
			base.OnRenderTabControl( control, renderer );
		}

	}
}