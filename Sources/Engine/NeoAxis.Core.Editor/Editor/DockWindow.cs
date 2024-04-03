#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Ribbon;
using System.Xml;
using System.Linq;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a docking window of the editor.
	/// </summary>
	public partial class DockWindow : EUserControl, IDockWindow
	{
		string windowTitle = "";
		Image windowImage;

		[Browsable( false )]
		public EDialogResult? ShowDialogAndSaveDocumentAutoAnswer { get; set; }

		//!!!!deleted
		//bool closing;

		//!!!!
		//System.Drawing.Font richTextBox1FontOriginal;
		//string richTextBox1FontCurrent = "";

		//

		public DockWindow()
		{
			InitializeComponent();

			if( IsDesignerHosted )
				return;

			EditorThemeUtility.ApplyDarkThemeToForm( this );
			//if( EditorAPI.DarkTheme )
			//{
			//	BackColor = Color.FromArgb( 54, 54, 54 );
			//}
		}

		// for separate hiding/showing logic
		public virtual bool HideOnRemoving { get { return false; } }

		// for separate serialization logic
		internal virtual bool IsSystemWindow { get { return /*can be changed in the future.*/ HideOnRemoving; } }

		public bool CloseByReturn { get; set; }
		public bool CloseByEscape { get; set; }

		protected override void OnDestroy()
		{
			//closing = true;

			if( timer1 != null )
			{
				timer1.Dispose();
				timer1 = null;
			}

			base.OnDestroy();
		}

		[Browsable( false )]
		internal KryptonPage KryptonPage
		{
			get { return Parent as KryptonPage; }
		}

		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );

			if( KryptonPage != null )
			{
				if( Parent != null )
					UpdateWindowTitle();

				KryptonPage.Text = windowTitle;
				KryptonPage.TextTitle = windowTitle;
				//KryptonPage.TextDescription = windowTitle;
				KryptonPage.VisibleChanged += ( s, _ ) => { Visible = ( (KryptonPage)s ).LastVisibleSet; };
				KryptonPage.ParentChanged += KryptonPage_ParentChanged;
			}
		}

		public string WindowTitle
		{
			get { return windowTitle; }
			set
			{
				if( windowTitle == value )
					return;
				windowTitle = value;

				if( KryptonPage != null )
				{
					KryptonPage.Text = windowTitle;
					KryptonPage.TextTitle = windowTitle;
					//KryptonPage.TextDescription = windowTitle;
				}
			}
		}

		protected virtual string GetResultWindowTitle()
		{
			return WindowTitle;
		}

		public void UpdateWindowTitle()
		{
			WindowTitle = GetResultWindowTitle();
		}

		public virtual Image WindowImage
		{
			get { return windowImage; }
			set
			{
				if( windowImage == value )
					return;
				windowImage = value;

				//!!!!!
			}
		}

		internal protected virtual void OnShowTitleContextMenu( KryptonContextMenuItems items )
		{
		}

		private void DockWindow_Load( object sender, EventArgs e )
		{
			if( IsDesignerHosted )
				return;

			timer1?.Start();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			UpdateWindowTitle();
		}

		internal void CalculateBigSizeForFloatingWindowDependingScreenSize( out Point position, out Size size )
		{
			//!!!!так? multimonitor

			var display = SystemSettings.AllDisplays.FirstOrDefault( d => d.Primary ) ?? SystemSettings.AllDisplays[ 0 ];
			var screenSize = new Vector2I( display.WorkingArea.Size.X, display.WorkingArea.Size.Y );
			//var screenSize = SystemSettings.AllDisplays[ 0 ].Bounds.Size;

			double aspect = 1.45;//1.4
			double heightPercent = 0.8;

			//!!!!
			if( this is SetReferenceWindow || this is SelectTypeWindow )
			{
				aspect = 1;
				heightPercent = .7;
			}
			//if( this is TipsWindow )
			//{
			//	aspect = 1.4;
			//	heightPercent = .5;
			//}

			size = new Size( (int)( (double)screenSize.Y * aspect * heightPercent ), (int)( (double)screenSize.Y * heightPercent ) );
			var posVec = ( screenSize - new Vector2I( size.Width, size.Height ) ) / 2;
			position = new Point( posVec.X, posVec.Y );

			//var size = new Size( (int)( screenSize.Height /* Height???*/ * aspect * heightPercent ),
			//				 (int)( screenSize.Height * heightPercent ) );
			//var location = new Point( ( screenSize.Width - size.Width ) / 2, ( screenSize.Height - size.Height ) / 2 );
			//return new System.Drawing.Rectangle( location , size );
		}

		////!!!!не заюзано
		//public void UpdateFonts( string fontForm )
		//{
		//	if( richTextBox1FontOriginal == null )
		//		richTextBox1FontOriginal = richTextBox1.Font;

		//	if( richTextBox1FontCurrent != fontForm )
		//	{
		//		if( !string.IsNullOrEmpty( fontForm ) && fontForm[ 0 ] != '(' )
		//		{
		//			try
		//			{
		//				System.Drawing.FontConverter fontConverter = new System.Drawing.FontConverter();
		//				richTextBox1.Font = (System.Drawing.Font)fontConverter.ConvertFromString( fontForm );
		//			}
		//			catch { }
		//		}
		//		else
		//			richTextBox1.Font = richTextBox1FontOriginal;

		//		richTextBox1FontCurrent = fontForm;
		//	}
		//}

		////!!!!не заюзано
		//public delegate void ProcessCmdKeyEventDelegate( ResourcesWindow sender, ref Message msg, Keys keyData, ref bool handled );
		//public event ProcessCmdKeyEventDelegate ProcessCmdKeyEvent;

		//protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		//{
		//	//if( ProcessCmdKeyEvent != null )
		//	//{
		//	//	bool handled = false;
		//	//	ProcessCmdKeyEvent( this, ref msg, keyData, ref handled );
		//	//	if( handled )
		//	//		return true;
		//	//}

		//	return base.ProcessCmdKey( ref msg, keyData );
		//}

		public override string ToString()
		{
			return this.GetType().Name + ": " + GetResultWindowTitle();
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( CloseByReturn && keyData == Keys.Return )
			{
				ShowDialogAndSaveDocumentAutoAnswer = EDialogResult.Yes;
				ActiveControl = null;
				Close();
				return true;
			}

			if( CloseByEscape && keyData == Keys.Escape )
			{
				ActiveControl = null;
				Close();
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		public void Close()
		{
			EditorForm.Instance?.WorkspaceController.CloseDockWindow( this );
		}

		public virtual ObjectsInFocus GetObjectsInFocus()
		{
			return null;
		}

		//internal virtual void OnSaving( XmlWriter xmlWriter )
		//{
		//}

		//internal virtual void OnLoading( XmlReader xmlReader )
		//{
		//}

		protected virtual void OnKryptonPageParentChanged() { }

		private void KryptonPage_ParentChanged( object sender, EventArgs e )
		{
			OnKryptonPageParentChanged();
		}

		public virtual Vector2I DefaultAutoHiddenSlideSize
		{
			get { return ( new Vector2( 280, 200 ) * EditorAPI2.DPIScale ).ToVector2I(); }
		}
	}
}

#endif