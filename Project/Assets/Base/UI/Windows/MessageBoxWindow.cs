// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NeoAxis;
using System.Threading.Tasks;

namespace Project
{
	public class MessageBoxWindow : NeoAxis.UIWindow
	{
		ResultDelegate resultHandler;
		object resultHandlerAnyData;

		public delegate void ResultDelegate( MessageBoxWindow sender, EDialogResult result, object anyData );

		/////////////////////////////////////////

		public class ResultData
		{
			public MessageBoxWindow Window;
			public volatile EDialogResult Result = EDialogResult.None;
		}

		/////////////////////////////////////////

		class ButtonData
		{
			public EDialogResult DialogResult;
			public ResultData ResultData;
		}

		/////////////////////////////////////////

		protected override void OnRenderUI( CanvasRenderer renderer )
		{
			//draw background
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 0, 0, 0, 0.6 ) );

			base.OnRenderUI( renderer );
		}

		//disable all controls behind
		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.AllPreviousInHierarchy; }
		}

		public void Button_Click( NeoAxis.UIButton sender )
		{
			//close window
			RemoveFromParent( true );

			var data = (ButtonData)sender.AnyData;
			data.ResultData.Result = data.DialogResult;

			resultHandler?.Invoke( this, data.ResultData.Result, resultHandlerAnyData );
		}

		static void ConfigureButtons( UIWindow window, EDialogResult[] results, ResultData resultData )
		{
			var allButtonNames = new List<string>();
			allButtonNames.Add( "Button 1 Set 1" );
			allButtonNames.Add( "Button 2 Set 1" );
			allButtonNames.Add( "Button 2 Set 2" );
			allButtonNames.Add( "Button 3 Set 1" );
			allButtonNames.Add( "Button 3 Set 2" );
			allButtonNames.Add( "Button 3 Set 3" );

			var uiButtons = new List<UIButton>();

			foreach( var name in allButtonNames )
			{
				var b = window.GetComponent( name ) as UIButton;
				if( b != null )
				{
					var thisSet = name.Contains( $"Button {results.Length} Set" );
					b.Enabled = b.Visible = thisSet;
					if( thisSet )
						uiButtons.Add( b );
				}
			}

			for( int n = 0; n < uiButtons.Count; n++ )
			{
				var uiButton = uiButtons[ n ];
				uiButton.Text = results[ n ].ToString();

				var data = new ButtonData();
				data.DialogResult = results[ n ];
				data.ResultData = resultData;
				uiButton.AnyData = data;
			}
		}

		static MessageBoxWindow CreateWindow( UIControl parent, string text, string caption )
		{
			var resourse = ResourceManager.LoadSeparateInstance( @"Base\UI\Windows\MessageBoxWindow.ui", true, false, true );
			if( resourse == null )
				return null;

			var window = resourse.ResultComponent as MessageBoxWindow;

			window.Text = caption;

			var textControl = window.GetComponent( "Text" ) as UIText;
			if( textControl != null )
				textControl.Text = text;

			return window;
		}

		UIButton FindButtonByResult( EDialogResult result )
		{
			foreach( var button in GetComponents<UIButton>( checkChildren: true ) )
			{
				var buttonData = button.AnyData as ButtonData;
				if( buttonData != null && buttonData.DialogResult == result )
					return button;
			}
			return null;
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			switch( e.Key )
			{
			case EKeys.Escape:
				{
					var button = FindButtonByResult( EDialogResult.Cancel ) ?? FindButtonByResult( EDialogResult.No ) ?? FindButtonByResult( EDialogResult.OK );
					if( button != null )
					{
						Button_Click( button );
						return true;
					}
				}
				break;

			case EKeys.Return:
				{
					var button = FindButtonByResult( EDialogResult.OK ) ?? FindButtonByResult( EDialogResult.Yes );
					if( button != null )
					{
						Button_Click( button );
						return true;
					}
				}
				break;
			}

			return base.OnKeyDown( e );
		}

		public static ResultData Show( UIControl parent, string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK, EMessageBoxIcon icon = EMessageBoxIcon.None, UIStyle overrideStyle = null, ResultDelegate resultHandler = null, object resultHandlerAnyData = null )
		{
			//!!!!impl icon

			var window = CreateWindow( parent, text, caption );
			if( window == null )
				return null;

			if( overrideStyle != null )
				window.Style = overrideStyle;

			window.resultHandler = resultHandler;
			window.resultHandlerAnyData = resultHandlerAnyData;

			EDialogResult[] results = null;
			switch( buttons )
			{
			case EMessageBoxButtons.OK:
				results = new EDialogResult[] { EDialogResult.OK };
				break;
			case EMessageBoxButtons.OKCancel:
				results = new EDialogResult[] { EDialogResult.OK, EDialogResult.Cancel };
				break;
			case EMessageBoxButtons.AbortRetryIgnore:
				results = new EDialogResult[] { EDialogResult.Abort, EDialogResult.Retry, EDialogResult.Ignore };
				break;
			case EMessageBoxButtons.YesNoCancel:
				results = new EDialogResult[] { EDialogResult.Yes, EDialogResult.No, EDialogResult.Cancel };
				break;
			case EMessageBoxButtons.YesNo:
				results = new EDialogResult[] { EDialogResult.Yes, EDialogResult.No };
				break;
			case EMessageBoxButtons.RetryCancel:
				results = new EDialogResult[] { EDialogResult.Retry, EDialogResult.Cancel };
				break;
			case EMessageBoxButtons.Cancel:
				results = new EDialogResult[] { EDialogResult.Cancel };
				break;
			}

			var resultData = new ResultData();
			resultData.Window = window;

			ConfigureButtons( window, results, resultData );

			//add to parent and enable
			parent.AddComponent( window );

			return resultData;
		}

		public static async Task<EDialogResult> ShowAsync( UIControl parent, string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK, EMessageBoxIcon icon = EMessageBoxIcon.None, UIStyle overrideStyle = null )
		{
			var result = Show( parent, text, caption, buttons, icon, overrideStyle );

			while( result.Result == EDialogResult.None )
				await Task.Delay( 10 );

			return result.Result;
		}
	}
}