// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using NeoAxis;

namespace Project
{
	public class MessageBoxWindow : NeoAxis.UIWindow
	{
		public class ResultData
		{
			public DialogResult Result = DialogResult.None;
		}

		class ButtonData
		{
			public DialogResult DialogResult;
			public ResultData ResultData;
		}

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

		public void Button_Click(NeoAxis.UIButton sender)
        {
			//close window
			RemoveFromParent(true);

			var data = (ButtonData)sender.AnyData;
			data.ResultData.Result = data.DialogResult;
		}

		static void ConfigureButtons(UIWindow window, DialogResult[] results, ResultData resultData )
		{
			var allButtonNames = new List<string>();
			allButtonNames.Add("Button 1 Set 1");			
			allButtonNames.Add("Button 2 Set 1");
			allButtonNames.Add("Button 2 Set 2");			
			allButtonNames.Add("Button 3 Set 1");
			allButtonNames.Add("Button 3 Set 2");
			allButtonNames.Add("Button 3 Set 3");

			var uiButtons = new List<UIButton>(); 

			foreach(var name in allButtonNames)
			{
				var b = window.GetComponent(name) as UIButton;
				if (b != null)
				{
					var thisSet = name.Contains($"Button {results.Length} Set");					
					b.Enabled = b.Visible = thisSet;
					if(thisSet)
						uiButtons.Add(b);
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

		static UIWindow CreateWindow(UIControl parent, string text, string caption)
		{
			var resourse = ResourceManager.LoadSeparateInstance(@"Base\UI\Windows\MessageBoxWindow.ui", true, false, true);
			if (resourse == null)
				return null;
			
			var window = resourse.ResultComponent as UIWindow;
			
			window.Text = caption;

			var textControl = window.GetComponent("Text") as UIText;
			if(textControl != null)
				textControl.Text = text;

			return window;
		}

		public static ResultData Show(UIControl parent, string text, string caption, MessageBoxButtons buttons )
		{
			var window = CreateWindow( parent, text, caption);
			if( window == null )
				return null;

			DialogResult[] results = null;
			switch( buttons )
			{
			case MessageBoxButtons.OK:
				results = new DialogResult[] { DialogResult.OK };
				break;
			case MessageBoxButtons.OKCancel:
				results = new DialogResult[] { DialogResult.OK, DialogResult.Cancel };
				break;
			case MessageBoxButtons.AbortRetryIgnore:
				results = new DialogResult[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore };
				break;
			case MessageBoxButtons.YesNoCancel:
				results = new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel};
				break;
			case MessageBoxButtons.YesNo:
				results = new DialogResult[] { DialogResult.Yes, DialogResult.No };
				break;
			case MessageBoxButtons.RetryCancel:
				results = new DialogResult[] { DialogResult.Retry, DialogResult.Cancel };
				break;
			}

			var resultData = new ResultData();

			ConfigureButtons( window, results, resultData);

			//add to parent and enable
			parent.AddComponent(window);

			return resultData;
		}

		public static void ShowInfo(UIControl parent, string text, string caption)
		{
			Show( parent, text, caption, MessageBoxButtons.OK );
		}
    }
}