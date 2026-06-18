// Copyright 2006–2026 Ivan Efimov. All rights reserved.
//#if CLOUD
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;

namespace Project
{
	public class AvatarWindow : NeoAxis.UIWindow
	{
		class ListStyle : UIStyleSimple
		{
			protected override void OnRenderListItem( UIList control, CanvasRenderer renderer, int itemIndex, Rectangle itemRectangle, FontComponent font, double fontSize )
			{
				var item = control.Items[ itemIndex ];

				if( itemIndex == control.SelectedIndex )
				{
					var color2 = new ColorValue( 0.1, 0.1, 0.8 );
					renderer.AddQuad( itemRectangle, color2 );
				}

				var image = ResourceManager.LoadResource<ImageComponent>( $@"Base\UI\Images\Avatar logos\{item}.png" );

				var positionX = itemRectangle.Left + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 0 ) ).X;

				var imageSizeY = itemRectangle.Size.Y * 0.9;//fontSize * 1.5;
				var imageSizeX = renderer.AspectRatioInv * imageSizeY;

				if( image != null )
				{
					var centerY = itemRectangle.GetCenter().Y;
					var r = new Rectangle( positionX, centerY - imageSizeY * 0.5, positionX + imageSizeX, centerY + imageSizeY * 0.5 );
					renderer.AddQuad( r, new RectangleF( 0, 0, 1, 1 ), image, new ColorValue( 1, 1, 1 ), true );
				}

				positionX += imageSizeX + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 8, 0 ) ).X;

				renderer.AddText( font, fontSize, item.Value.ToString(), new Vector2( positionX, itemRectangle.GetCenter().Y ), EHorizontalAlignment.Left, EVerticalAlignment.Center, new ColorValue( 1, 1, 1 ) );

				//var positionX = itemRectangle.Left + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 0 ) ).X;
				//renderer.AddText( font, fontSize, item, new Vector2( positionX, itemRectangle.GetCenter().Y ), EHorizontalAlignment.Left, EVerticalAlignment.Center, new ColorValue( 1, 1, 1 ) );
			}
		}

		/////////////////////////////////////////

		//UIRenderTarget GetRenderTargetPreview() { return GetComponent( "Render Target Preview" ) as UIRenderTarget; }
		UIList GetList() { return GetComponent( "List" ) as UIList; }
		UIButton GetButtonOK() { return GetComponent( "Button OK" ) as UIButton; }

		/////////////////////////////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			GetList().Style = new ListStyle();

			//request avatar settings from the server
			if( SimulationAppClient.ConnectionNode != null )
			{
				SimulationAppClient.ConnectionNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
				SimulationAppClient.ConnectionNode.Messages.SendToServer( "RequestAvatarSettings", "" );
			}
			else
				UpdateList();
		}

		protected override void OnDisabledInSimulation()
		{
			if( SimulationAppClient.ConnectionNode != null )
				SimulationAppClient.ConnectionNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;

			base.OnDisabledInSimulation();
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				Close( false );
				return true;
			}
			if( e.Key == EKeys.Return )
			{
				Close( true );
				return true;
			}

			return base.OnKeyDown( e );
		}

		public void List_ItemMouseDoubleClick( NeoAxis.UIControl sender, NeoAxis.EMouseButtons button, ref bool handled )
		{
			if( button == EMouseButtons.Left )
			{
				Close( true );
				handled = true;
			}
		}

		private void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			if( message == "AvatarSettings" )
			{
				UpdateList();

				var block = TextBlock.Parse( data, out var error );
				if( !string.IsNullOrEmpty( error ) )
					Log.Warning( "Unable to parse avatar settings. " + error );

				if( block != null )
				{
					var settings = new AvatarSettings();
					if( settings.Load( block ) )
						UpdateControlsBySettings( settings );
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			GetButtonOK().ReadOnly = GetList().SelectedItem == null;
		}

		void Close( bool applyChanges )
		{
			if( applyChanges )
			{
				var settings = GetSettingsFromControls();

				var block = new TextBlock();
				settings.Save( block );

				SimulationAppClient.ConnectionNode?.Messages.SendToServer( "SetAvatarSettings", block.DumpToString() );
			}

			RemoveFromParent( true );
		}

		public void ButtonOK_Click( NeoAxis.UIButton sender )
		{
			Close( true );
		}

		public void ButtonCancel_Click( NeoAxis.UIButton sender )
		{
			Close( false );
		}

		void UpdateControlsBySettings( AvatarSettings settings )
		{
			var list = GetList();
			if( list.SelectItemByValue( settings.NamedCharacter ) == -1 )
				list.SelectedIndex = 0;
			//if( !list.SelectItem( settings.NamedCharacter ) )
			//	list.SelectedIndex = 0;
		}

		AvatarSettings GetSettingsFromControls()
		{
			var result = new AvatarSettings();

			var list = GetList();
			if( list.SelectedIndex != -1 )
				result.NamedCharacter = list.SelectedItem.Value.ToString();

			return result;
		}

		void UpdateList()
		{
			var list = GetList();

			list.AddItem( "Bryce" );

			//list.Items.Add( "Swat Guy" );
			//list.Items.Add( "Kachujin" );
			//list.Items.Add( "Kate" );
		}
	}
}
//#endif