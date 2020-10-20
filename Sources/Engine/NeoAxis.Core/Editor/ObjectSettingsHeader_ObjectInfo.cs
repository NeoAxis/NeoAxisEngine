// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class ObjectSettingsHeader_ObjectInfo : ObjectSettingsHeader
	{
		bool? buttonTypeSettingsDefaultValueCurrentEnabled;

		//

		public ObjectSettingsHeader_ObjectInfo()
		{
			InitializeComponent();

			buttonTypeSettings.Values.Image = EditorResourcesCache.Type;

			if( IsDesignerHosted )
				return;

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			toolTip1.SetToolTip( buttonTypeSettings, EditorLocalization.Translate( "SettingsWindow", "Type Settings" ) );
			toolTip1.SetToolTip( buttonTypeSettingsDefaultValue, EditorLocalization.Translate( "SettingsWindow", "Reset Type Settings to default." ) );
		}

		private void buttonMove_Click( object sender, EventArgs e )
		{

		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;
			if( ObjectSettingsWindow == null )
				return;
			if( ObjectSettingsWindow.ObjectOfWindow == null )
				return;

			timer1.Start();

			UpdateInfoControls();
			UpdateControlsLocation();
			UpdateButtons();
		}

		void UpdateInfoControls()
		{
			var obj = ObjectSettingsWindow.ObjectOfWindow;


			string text1;

			//var objs = SettingsPanel.selectedObjects;
			//if( objs.Length == 1 )
			//{
			//var obj = objs[ 0 ];

			var component = obj as Component;
			if( component != null )
			{
				if( !string.IsNullOrEmpty( component.Name ) )
					text1 = string.Format( "{0} - {1}", component.Name, component.BaseType.ToString() );
				else
					text1 = component.BaseType.ToString();
			}
			else
				text1 = obj.ToString();
			//}
			//else
			//{
			//	text1 = string.Format( "{0} objects", objs.Length );
			//	text2 = "";
			//}

			if( kryptonLabel1.Text != text1 )
				kryptonLabel1.Text = text1;





			//string text1;
			//string text2;

			////var objs = SettingsPanel.selectedObjects;
			////if( objs.Length == 1 )
			////{
			////var obj = objs[ 0 ];

			//var component = obj as Component;
			//if( component != null )
			//{
			//	if( !string.IsNullOrEmpty( component.Name ) )
			//	{
			//		text1 = component.Name;
			//		text2 = component.BaseType.ToString();
			//	}
			//	else
			//	{
			//		text1 = component.BaseType.ToString();
			//		text2 = text1;
			//	}
			//}
			//else
			//{
			//	text1 = obj.ToString();
			//	text2 = "";
			//}
			////}
			////else
			////{
			////	text1 = string.Format( "{0} objects", objs.Length );
			////	text2 = "";
			////}

			//if( kryptonLabel1.Text != text1 )
			//	kryptonLabel1.Text = text1;
			//if( kryptonLabel2.Text != text2 )
			//	kryptonLabel2.Text = text2;






			//var box = richTextBox1;

			//var obj = ObjectSettingsWindow.ObjectOfWindow;

			//box.Text = "";
			//box.SelectionStart = box.TextLength;
			//box.SelectionLength = 0;

			//var originalFont = box.SelectionFont;

			////!!!!так ли
			//var titleFont = new System.Drawing.Font( "Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold,
			//	System.Drawing.GraphicsUnit.Point, ( (byte)( 204 ) ) );

			//box.SelectionFont = titleFont;// new Font( box.SelectionFont, FontStyle.Bold );
			//box.AppendText( obj.ToString() );
			//box.SelectionFont = originalFont;

			////!!!!не так. надо тут уже Component проверять
			////box.AppendText( " type" );

			//var component = obj as Component;
			//if( component != null )
			//{
			//	box.AppendText( "\r\n" );
			//	box.AppendText( "type: " + component.BaseType.ToString() );

			//	if( !string.IsNullOrEmpty( component.Name ) )
			//	{
			//		box.AppendText( "\r\n" );
			//		box.AppendText( "name: " + component.Name );
			//	}
			//}


			//box.SelectionFont = new Font( box.SelectionFont, FontStyle.Bold );
			//box.AppendText( obj.ToString() + "\r\n" );

			//box.SelectionFont = originalFont;
			//box.AppendText( "Second line\r\n" );

			//box.AppendText( "\r\n" );
			//box.AppendText( "Third line" );


			//box.SelectionColor = Color.Green;
			//box.AppendText( " green" );
			//box.SelectionColor = box.ForeColor;





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
			if( ObjectSettingsWindow?.Document != null )
				return ObjectSettingsWindow.ObjectOfWindow as Component;
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

				if( EditorAPI.DarkTheme )
					buttonTypeSettingsDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "DefaultValueCircle_Big_Dark" : "DefaultValueCircle3_Dark" ) : null;
				else
					buttonTypeSettingsDefaultValue.Values.Image = canReset ? EditorResourcesCache.GetImage( EditorAPI.DPIScale >= 2.0 ? "DefaultValueCircle_Big" : "DefaultValueCircle3" ) : null;
			}

			var enabled = component != null && EditorUtility.AllowConfigureComponentTypeSettings;
			if( buttonTypeSettings.Enabled != enabled )
				buttonTypeSettings.Enabled = enabled;

			bool projectSettings = ObjectSettingsWindow?.Document?.SpecialMode == "ProjectSettingsUserMode";
			var visible = !projectSettings && EditorUtility.AllowConfigureComponentTypeSettings;
			if( buttonTypeSettings.Visible != visible )
				buttonTypeSettings.Visible = visible;
		}

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
				var form = new ComponentTypeSettingsForm( ObjectSettingsWindow.Document, component );
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
				var text = EditorLocalization.Translate( "SettingsWindow", "Reset to default?" );
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
				{
					var oldValue = component.TypeSettingsPrivateObjects;

					component.TypeSettingsPrivateObjects = null;

					var undoItem = new UndoActionPropertiesChange.Item( component, (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( Component ) ).MetadataGetMemberBySignature( "property:TypeSettingsPrivateObjects" ), oldValue, null );
					ObjectSettingsWindow.Document.UndoSystem.CommitAction( new UndoActionPropertiesChange( undoItem ) );
					ObjectSettingsWindow.Document.Modified = true;
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
