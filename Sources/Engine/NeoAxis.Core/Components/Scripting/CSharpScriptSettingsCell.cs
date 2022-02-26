// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public class CSharpScriptSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonUpdate;
		ProcedureUI.Button buttonInfo;

		//

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "CSharpScript", text );
		}

		protected override void OnInit()
		{
			buttonUpdate = ProcedureForm.CreateButton( Translate( "Update" ) );
			buttonUpdate.Click += ButtonUpdate_Click;

			buttonInfo = ProcedureForm.CreateButton( Translate( "Info" ) );
			buttonInfo.Click += ButtonInfo_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdate, buttonInfo } );
		}

		[Browsable( false )]
		CSharpScript Script
		{
			get { return GetFirstObject<CSharpScript>(); }
		}

		//protected override void OnLoad( EventArgs e )
		//{
		//	base.OnLoad( e );

		//	if( Script != null )
		//	{
		//		kryptonTextBoxCode.Text = Script.Code;
		//		if( !string.IsNullOrEmpty( Script.Code.GetByReference ) )
		//			kryptonTextBoxCode.ReadOnly = true;
		//	}
		//}

		//private void kryptonTextBoxCode_Leave( object sender, EventArgs e )
		//{
		//	//!!!!

		//	if( Script != null && Provider.DocumentWindow?.Document != null )
		//	{
		//		var newCode = kryptonTextBoxCode.Text;
		//		if( newCode != Script.Code.Value )
		//		{
		//			var oldValue = Script.Code;

		//			var undoItem = new UndoActionPropertiesChange.Item( Script, (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( Script ) ).MetadataGetMemberBySignature( "property:Code" ), oldValue, null );
		//			Provider.DocumentWindow.Document.UndoSystem.CommitAction( new UndoActionPropertiesChange( undoItem ) );
		//			Provider.DocumentWindow.Document.Modified = true;
		//		}
		//	}
		//}

		private void ButtonUpdate_Click( ProcedureUI.Button sender )
		{
			foreach( var script in GetObjects<CSharpScript>() )
				script.Update();
		}

		private void ButtonInfo_Click( ProcedureUI.Button sender )
		{
			if( Script != null )
				EditorMessageBox.ShowInfo( GetInfo( Script ) );
		}

		internal static string GetInfo( CSharpScript script )
		{
			var lines = new List<string>();

			if( script.CompiledScript != null )
			{
				//lines.Add( string.Format( "Code type: {0}", TypeUtils.DisplayNameAddSpaces( Script.CompiledCodeType.ToString() ) ) );
				//lines.Add( "" );
				lines.Add( string.Format( Translate( "Members: {0}" ), script.CompiledMembers.Count ) );
				foreach( var member in script.CompiledMembers )
					lines.Add( "- " + member.ToString() );
			}
			else
				lines.Add( Translate( "No compiled data." ) );

			var text = "";
			foreach( var line in lines )
			{
				if( text != "" )
					text += "\n";
				text += line;
			}
			return text;
		}
		//!!!!обновлять Enabled
	}
}
