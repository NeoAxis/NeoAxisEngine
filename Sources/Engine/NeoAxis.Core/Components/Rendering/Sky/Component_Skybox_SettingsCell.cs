// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NeoAxis.Editor
{
	public class Component_Skybox_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Edit editLink;
		ProcedureUI.Button buttonImport;
		ProcedureUI.Text importText1;
		ProcedureUI.Text importText2;

		//ProcedureUI.Button buttonHDRIHaven;

		//

		protected override void OnInit()
		{
			//bool easyImport = false;

			//var sky = GetFirstObject<Component_Skybox>();
			//if( sky != null )
			//{
			//	var scene = sky.FindParent<Component_Scene>();
			//	if( scene != null )
			//		easyImport = true;
			//}

			//if( easyImport )
			{
				{
					var textInfo = ProcedureForm.CreateText( "Easy HDR file import" );// Specify web of file path to a file." );
					importText1 = textInfo;
					textInfo.Bold = true;
					ProcedureForm.AddRow( new ProcedureUI.Control[] { textInfo } );
				}

				{
					var textInfo = ProcedureForm.CreateText( "Specify web or file path to a file, click Import." );
					importText2 = textInfo;
					ProcedureForm.AddRow( new ProcedureUI.Control[] { textInfo } );
				}

				editLink = ProcedureForm.CreateEdit( "https://hdrihaven.com/files/hdris/qwantani_4k.hdr" );
				//editLink.TextChanged
				ProcedureForm.AddRow( new ProcedureUI.Control[] { editLink } );

				buttonImport = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Import" ) );
				buttonImport.Click += ButtonImport_Click;
				ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonImport } );
			}

			//{
			//	var textInfo = ProcedureForm.CreateText( "Huge set of 100% Free HDRIs, for Everyone" );// Specify web of file path to a file." );
			//	textInfo.Bold = true;
			//	ProcedureForm.AddRow( new ProcedureUI.Control[] { textInfo } );
			//}

			//buttonHDRIHaven = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Go to HDRI Haven" ), ProcedureUI.Button.SizeEnum.Long );
			//buttonHDRIHaven.Click += ButtonHDRIHaven_Click;
			//ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonHDRIHaven } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			bool easyImport = false;

			var sky = GetFirstObject<Component_Skybox>();
			if( sky != null )
			{
				var scene = sky.FindParent<Component_Scene>();
				if( scene != null )
					easyImport = true;
			}

			if( importText1 != null )
				importText1.Enabled = easyImport;
			if( importText2 != null )
				importText2.Enabled = easyImport;
			if( editLink != null )
				editLink.Enabled = easyImport;
			if( buttonImport != null )
				buttonImport.Enabled = easyImport;
		}

		private void ButtonImport_Click( ProcedureUI.Button sender )
		{
			var sky = GetFirstObject<Component_Skybox>();
			if( sky == null )
				return;
			var scene = sky.FindParent<Component_Scene>();
			if( scene == null )
				return;

			var link = editLink.Text;

			var notification = ScreenNotifications.ShowSticky( "Importing..." );

			try
			{
				string destVirtualFileName;
				{
					string name = sky.GetPathFromRoot();
					foreach( char c in new string( Path.GetInvalidFileNameChars() ) + new string( Path.GetInvalidPathChars() ) )
						name = name.Replace( c.ToString(), "_" );
					name = name.Replace( " ", "_" );
					destVirtualFileName = Path.Combine( ComponentUtility.GetOwnedFileNameOfComponent( scene ) + "_Files", name );

					destVirtualFileName += Path.GetExtension( link );
				}

				var destRealFileName = VirtualPathUtility.GetRealPathByVirtual( destVirtualFileName );


				if( File.Exists( destRealFileName ) )
				{
					if( EditorMessageBox.ShowQuestion( $"Overwrite \'{destRealFileName}\'?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
						return;
				}

				Directory.CreateDirectory( Path.GetDirectoryName( destRealFileName ) );

				if( File.Exists( link ) )
				{
					File.Copy( link, destRealFileName, true );
				}
				else
				{
					//if( Uri.IsWellFormedUriString( link, UriKind.Absolute ) )
					//{
					using( var client = new WebClient() )
						client.DownloadFile( link, destRealFileName );
					//}
				}

				var oldValue = sky.Cubemap;

				sky.Cubemap = ReferenceUtility.MakeReference( destVirtualFileName );

				//undo
				var property = (Metadata.Property)sky.MetadataGetMemberBySignature( "property:Cubemap" );
				var undoItem = new UndoActionPropertiesChange.Item( sky, property, oldValue );
				var undoAction = new UndoActionPropertiesChange( undoItem );
				Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
			}
			finally
			{
				notification.Close();
			}
		}

		//private void ButtonHDRIHaven_Click( ProcedureUI.Button obj )
		//{
		//	Process.Start( new ProcessStartInfo( "https://hdrihaven.com/hdris/" ) { UseShellExecute = true } );
		//}

	}
}
