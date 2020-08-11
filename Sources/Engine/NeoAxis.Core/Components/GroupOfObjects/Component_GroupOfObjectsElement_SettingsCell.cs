// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_GroupOfObjectsElement_SettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClearItems;
		ProcedureUI.Button buttonUpdateAlignment;
		ProcedureUI.Button buttonUpdateVariations;
		ProcedureUI.Button buttonRandomizeGroups;
		ProcedureUI.Button buttonResetColors;

		//

		string Translate( string text )
		{
			return EditorLocalization.Translate( "GroupOfObjectsElement", text );
		}

		protected override void OnInit()
		{
			buttonClearItems = ProcedureForm.CreateButton( Translate( "Clear Objects" ), ProcedureUI.Button.SizeEnum.Long );
			buttonClearItems.Click += ButtonClearObjects_Click;

			buttonUpdateAlignment = ProcedureForm.CreateButton( Translate( "Update Alignment" ), ProcedureUI.Button.SizeEnum.Long );
			buttonUpdateAlignment.Click += ButtonUpdateAlignment_Click;

			buttonUpdateVariations = ProcedureForm.CreateButton( Translate( "Update Variations" ), ProcedureUI.Button.SizeEnum.Long );
			buttonUpdateVariations.Click += ButtonUpdateVariations_Click;

			buttonRandomizeGroups = ProcedureForm.CreateButton( Translate( "Random Groups" ), ProcedureUI.Button.SizeEnum.Long );
			buttonRandomizeGroups.Click += ButtonRandomizeGroups_Click;

			buttonResetColors = ProcedureForm.CreateButton( Translate( "Reset Colors" ), ProcedureUI.Button.SizeEnum.Long );
			buttonResetColors.Click += ButtonResetColors_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonClearItems, buttonUpdateAlignment } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdateVariations, buttonRandomizeGroups } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonResetColors } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var elements = GetObjects<Component_GroupOfObjectsElement>();
			buttonClearItems.Enabled = elements.Any( obj => obj.ObjectsExists() );
			buttonUpdateAlignment.Enabled = buttonClearItems.Enabled;

			buttonUpdateVariations.Enabled = buttonClearItems.Enabled && GetObjects<Component_GroupOfObjectsElement_Surface>().Length != 0;
			buttonRandomizeGroups.Enabled = buttonUpdateVariations.Enabled;

			buttonResetColors.Enabled = buttonClearItems.Enabled;
		}

		private void ButtonClearObjects_Click( ProcedureUI.Button obj )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects of the element?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var element in GetObjects<Component_GroupOfObjectsElement>() )
			{
				var groupOfObjects = element.Parent as Component_GroupOfObjects;
				if( groupOfObjects != null )
				{
					var indexes = element.GetObjectsOfElement();
					if( indexes.Count != 0 )
					{
						var action = new Component_GroupOfObjects_Editor.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
						undoMultiAction.AddAction( action );
					}
				}
			}

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		private unsafe void ButtonUpdateAlignment_Click( ProcedureUI.Button sender )
		{
			var undoMultiAction = new UndoMultiAction();
			var groupsOfObjectToUpdate = new ESet<Component_GroupOfObjects>();

			foreach( var element in GetObjects<Component_GroupOfObjectsElement>() )
			{
				var groupOfObjects = element.Parent as Component_GroupOfObjects;
				if( groupOfObjects != null )
				{
					var elementMesh = element as Component_GroupOfObjectsElement_Mesh;
					if( elementMesh != null )
					{
						elementMesh.UpdateAlignment( undoMultiAction );
						groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
					}

					var elementSurface = element as Component_GroupOfObjectsElement_Surface;
					if( elementSurface != null )
					{
						elementSurface.UpdateAlignment( undoMultiAction );
						groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
					}
				}
			}

			foreach( var groupOfObjects in groupsOfObjectToUpdate )
				groupOfObjects.CreateSectors();

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		void UpdateSurfaceElementVariations( bool randomizeGroups )
		{
			var undoMultiAction = new UndoMultiAction();
			var groupsOfObjectToUpdate = new ESet<Component_GroupOfObjects>();

			foreach( var elementSurface in GetObjects<Component_GroupOfObjectsElement_Surface>() )
			{
				var groupOfObjects = elementSurface.Parent as Component_GroupOfObjects;
				if( groupOfObjects != null )
				{
					elementSurface.UpdateVariations( randomizeGroups, undoMultiAction );
					groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
				}
			}

			foreach( var groupOfObjects in groupsOfObjectToUpdate )
				groupOfObjects.CreateSectors();

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		void ResetColors()
		{
			var undoMultiAction = new UndoMultiAction();
			var groupsOfObjectToUpdate = new ESet<Component_GroupOfObjects>();

			foreach( var elementSurface in GetObjects<Component_GroupOfObjectsElement>() )
			{
				var groupOfObjects = elementSurface.Parent as Component_GroupOfObjects;
				if( groupOfObjects != null )
				{
					elementSurface.ResetColors( undoMultiAction );
					groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
				}
			}

			foreach( var groupOfObjects in groupsOfObjectToUpdate )
				groupOfObjects.CreateSectors();

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		private unsafe void ButtonUpdateVariations_Click( ProcedureUI.Button sender )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Update variations of the objects?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			UpdateSurfaceElementVariations( false );
		}

		private void ButtonRandomizeGroups_Click( ProcedureUI.Button obj )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Refresh surface groups randomly?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			UpdateSurfaceElementVariations( true );
		}

		private void ButtonResetColors_Click( ProcedureUI.Button obj )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Reset color of the objects to \'1 1 1\'?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			ResetColors();
		}
	}
}
