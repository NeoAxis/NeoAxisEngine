// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public class Component_ParticleSystemInSpace_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonPlay;
		ProcedureUI.Button buttonStop;

		//

		string Translate( string text )
		{
			return EditorLocalization.Translate( "ParticleSystemInSpace", text );
		}

		protected override void OnInit()
		{
			buttonPlay = ProcedureForm.CreateButton( Translate( "Play" ) );
			buttonPlay.Click += ButtonPlay_Click;

			buttonStop = ProcedureForm.CreateButton( Translate( "Stop" ) );
			buttonStop.Click += ButtonStop_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonPlay, buttonStop } );
		}

		protected override void OnUpdate()
		{
			var obj = GetObject();

			buttonPlay.Enabled = obj != null;
			buttonStop.Enabled = obj != null && obj.Activated;
		}

		Component_ParticleSystemInSpace GetObject()
		{
			foreach( var obj in Provider.SelectedObjects )
			{
				var obj2 = obj as Component_ParticleSystemInSpace;
				if( obj2 != null )
					return obj2;
			}
			return null;
		}

		private void ButtonPlay_Click( ProcedureUI.Button sender )
		{
			var obj = GetObject();
			if( obj == null )
				return;

			var oldValue = obj.Activated;

			//update
			if( obj.Activated )
				obj.RecreateData( false );
			else
				obj.Activated = true;

			//undo
			if( oldValue != obj.Activated )
			{
				var property = (Metadata.Property)obj.MetadataGetMemberBySignature( "property:Activated" );
				var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( obj, property, oldValue, null ) );
				Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
			}
		}

		private void ButtonStop_Click( ProcedureUI.Button sender )
		{
			var obj = GetObject();
			if( obj == null )
				return;

			var oldValue = obj.Activated;

			//update
			obj.Activated = false;

			//undo
			var property = (Metadata.Property)obj.MetadataGetMemberBySignature( "property:Activated" );
			var undoAction = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( obj, property, oldValue, null ) );
			Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
		}
	}
}
