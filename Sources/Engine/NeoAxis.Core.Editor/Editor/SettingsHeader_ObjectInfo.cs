#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsHeader_ObjectInfo : SettingsHeader
	{
		bool? buttonTypeSettingsDefaultValueCurrentEnabled;

		//

		public SettingsHeader_ObjectInfo()
		{
			InitializeComponent();

			buttonTypeSettings.Values.Image = EditorResourcesCache.Type;

			toolTip1.SetToolTip( buttonTypeSettings, EditorLocalization2.Translate( "SettingsWindow", "Type Settings" ) );
			toolTip1.SetToolTip( buttonTypeSettingsDefaultValue, EditorLocalization2.Translate( "SettingsWindow", "Reset Type Settings to default." ) );
		}

		private void buttonMove_Click( object sender, EventArgs e )
		{

		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateInfoControls();
			UpdateControlsLocation();
			UpdateButtons();

			timer1.Start();
		}

		void UpdateInfoControls()
		{
			string text1;
			string text2;

			var objs = SettingsPanel.selectedObjects;
			if( objs.Length == 1 )
			{
				var obj = objs[ 0 ];

				var component = obj as Component;
				if( component != null )
				{
					if( !string.IsNullOrEmpty( component.Name ) )
					{
						text1 = component.Name;
						text2 = component.BaseType.ToString();
					}
					else
					{
						text1 = component.BaseType.ToString();
						text2 = text1;
					}
				}
				else
				{
					text1 = obj.ToString();
					text2 = "";
				}
			}
			else
			{
				text1 = string.Format( EditorLocalization2.Translate( "SettingsWindow", "{0} objects" ), objs.Length );
				text2 = "";
			}

			if( kryptonLabel1.Text != text1 )
				kryptonLabel1.Text = text1;
			if( kryptonLabel2.Text != text2 )
				kryptonLabel2.Text = text2;





			//var box = richTextBox1;

			//var objs = SettingsPanel.selectedObjects;
			//if( objs.Length == 1 )
			//{
			//	var obj = objs[ 0 ];

			//	box.Text = "";
			//	box.SelectionStart = box.TextLength;
			//	box.SelectionLength = 0;

			//	var originalFont = box.SelectionFont;

			//	//!!!!так ли
			//	var titleFont = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold,
			//		System.Drawing.GraphicsUnit.Point, ( (byte)( 204 ) ) );

			//	box.SelectionFont = titleFont;// new Font( box.SelectionFont, FontStyle.Bold );
			//	box.AppendText( obj.ToString() );
			//	box.SelectionFont = originalFont;

			//	//!!!!не так. надо тут уже Component проверять
			//	//box.AppendText( " type" );

			//	var component = obj as Component;
			//	if( component != null )
			//	{
			//		box.AppendText( "\r\n" );
			//		box.AppendText( "type: " + component.BaseType.ToString() );

			//		if( !string.IsNullOrEmpty( component.Name ) )
			//		{
			//			box.AppendText( "\r\n" );
			//			box.AppendText( "name: " + component.Name );
			//		}
			//	}

			//	//box.SelectionFont = new Font( box.SelectionFont, FontStyle.Bold );
			//	//box.AppendText( obj.ToString() + "\r\n" );

			//	//box.SelectionFont = originalFont;
			//	//box.AppendText( "Second line\r\n" );

			//	//box.AppendText( "\r\n" );
			//	//box.AppendText( "Third line" );


			//	//box.SelectionColor = Color.Green;
			//	//box.AppendText( " green" );
			//	//box.SelectionColor = box.ForeColor;
			//}
			//else
			//{
			//	//!!!!!
			//	box.Text = string.Format( "{0} objects", objs.Length );
			//}


			//string text;
			//var objs = Provider.SelectedObjects;
			//if( objs.Length == 1 )
			//{
			//	xx xx;

			//	text = objs[ 0 ].ToString();
			//}
			//else
			//{
			//	text = string.Format( "{0} objects", objs.Length );
			//}
			//labelName.Text = text;
		}

		Component GetTypeSettingsComponent()
		{
			//!!!!multiselection

			if( SettingsPanel.documentWindow?.Document2 != null && SettingsPanel.selectedObjects.Length == 1 )
				return SettingsPanel.selectedObjects[ 0 ] as Component;
			else
				return null;
		}

		void UpdateControlsLocation()
		{
			buttonTypeSettings.Location = new Point( ClientRectangle.Right - buttonTypeSettings.Width, 0 );
			buttonTypeSettingsDefaultValue.Location = new Point( buttonTypeSettings.Location.X - buttonTypeSettingsDefaultValue.Width - 2, 5 );

			kryptonLabel1.Width = buttonTypeSettingsDefaultValue.Location.X - 2 - kryptonLabel1.Location.X;
			kryptonLabel2.Width = kryptonLabel1.Width;
		}

		void UpdateButtons()
		{
			var component = GetTypeSettingsComponent();

			bool canReset = component != null && component.TypeSettingsPrivateObjects != null && EditorUtility.AllowConfigureComponentTypeSettings;
			if( buttonTypeSettingsDefaultValue.Enabled != canReset )
				buttonTypeSettingsDefaultValue.Enabled = canReset;
			if( buttonTypeSettingsDefaultValue.Visible != canReset )
				buttonTypeSettingsDefaultValue.Visible = canReset;
			if( buttonTypeSettingsDefaultValueCurrentEnabled != canReset )
			{
				buttonTypeSettingsDefaultValueCurrentEnabled = canReset;

				if( EditorAPI2.DarkTheme )
					buttonTypeSettingsDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI2.DPIScale >= 2.0 ? "DefaultValueCircle_Big_Dark" : "DefaultValueCircle3_Dark" ) : null;
				else
					buttonTypeSettingsDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI2.DPIScale >= 2.0 ? "DefaultValueCircle_Big" : "DefaultValueCircle3" ) : null;
			}

			var enabled = component != null && EditorUtility.AllowConfigureComponentTypeSettings;
			if( buttonTypeSettings.Enabled != enabled )
				buttonTypeSettings.Enabled = enabled;

			var visible = EditorUtility.AllowConfigureComponentTypeSettings;
			if( buttonTypeSettings.Visible != visible )
				buttonTypeSettings.Visible = visible;

			//object obj = null;
			//if( SettingsPanel.selectedObjects.Length == 1 )
			//	obj = SettingsPanel.selectedObjects[ 0 ];

			//buttonEdit.Enabled = obj != null &&
			//	SettingsPanel != null && SettingsPanel.documentWindow != null && SettingsPanel.documentWindow.Document != null &&
			//	EditorForm.Instance.IsDocumentObjectSupport( obj );
		}

		//private void buttonSettingsWindow_Click( object sender, EventArgs e )
		//{
		//	object obj = null;
		//	if( SettingsPanel.selectedObjects.Length == 1 )
		//		obj = SettingsPanel.selectedObjects[ 0 ];

		//	if( obj != null )
		//	{
		//		bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );

		//		EditorForm.Instance.ShowObjectSettingsWindow( SettingsPanel.documentWindow.Document, obj, canUseAlreadyOpened );
		//	}
		//}

		//private void buttonEdit_Click( object sender, EventArgs e )
		//{
		//	object obj = null;
		//	if( SettingsPanel.selectedObjects.Length == 1 )
		//		obj = SettingsPanel.selectedObjects[ 0 ];

		//	if( obj != null )
		//	{
		//		bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );

		//		EditorForm.Instance.OpenDocumentWindowForObject( SettingsPanel.documentWindow.Document, obj, canUseAlreadyOpened );
		//	}
		//}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			UpdateInfoControls();
			UpdateButtons();
		}

		private void buttonTypeSettings_Click( object sender, EventArgs e )
		{
			var component = GetTypeSettingsComponent();
			if( component != null )
			{
				var form = new ComponentTypeSettingsForm( SettingsPanel.documentWindow.Document2, component );
				EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
				{
					form.ShowDialog();
				} );
			}
		}

		private void buttonTypeSettingsDefaultValue_Click( object sender, EventArgs e )
		{
			var component = GetTypeSettingsComponent();
			if( component != null )
			{
				var text = EditorLocalization2.Translate( "SettingsWindow", "Reset to default?" );
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
				{
					var oldValue = component.TypeSettingsPrivateObjects;

					component.TypeSettingsPrivateObjects = null;

					var undoItem = new UndoActionPropertiesChange.Item( component, (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( Component ) ).MetadataGetMemberBySignature( "property:TypeSettingsPrivateObjects" ), oldValue, null );
					SettingsPanel.documentWindow.Document2.UndoSystem.CommitAction( new UndoActionPropertiesChange( undoItem ) );
					SettingsPanel.documentWindow.Document2.Modified = true;
				}
			}
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( Created )
				UpdateControlsLocation();
		}
	}
}

#endif