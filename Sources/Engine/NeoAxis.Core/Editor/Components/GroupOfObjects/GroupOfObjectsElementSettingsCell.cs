#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class GroupOfObjectsElementSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonClearItems;
		ProcedureUI.Button buttonUpdateAlignment;
		ProcedureUI.Button buttonUpdateVariations;
		ProcedureUI.Button buttonRandomizeGroups;
		ProcedureUI.Button buttonSetColors;
		//ProcedureUI.Button buttonResetColors;

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

			buttonSetColors = ProcedureForm.CreateButton( Translate( "Set Colors" ), ProcedureUI.Button.SizeEnum.Long );
			buttonSetColors.Click += ButtonSetColors_Click;

			//buttonResetColors = ProcedureForm.CreateButton( Translate( "Reset Colors" ), ProcedureUI.Button.SizeEnum.Long );
			//buttonResetColors.Click += ButtonResetColors_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonClearItems, buttonUpdateAlignment } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonUpdateVariations, buttonRandomizeGroups } );
			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonSetColors } );//, buttonResetColors } );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			var elements = GetObjects<GroupOfObjectsElement>();
			buttonClearItems.Enabled = elements.Any( obj => obj.ObjectsExists() );
			buttonUpdateAlignment.Enabled = buttonClearItems.Enabled;

			buttonUpdateVariations.Enabled = buttonClearItems.Enabled && GetObjects<GroupOfObjectsElement_Surface>().Length != 0;
			buttonRandomizeGroups.Enabled = buttonUpdateVariations.Enabled;

			buttonSetColors.Enabled = buttonClearItems.Enabled;
			//buttonResetColors.Enabled = buttonClearItems.Enabled;
		}

		private void ButtonClearObjects_Click( ProcedureUI.Button obj )
		{
			if( EditorMessageBox.ShowQuestion( Translate( "Delete all objects of the element?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return;

			var undoMultiAction = new UndoMultiAction();

			foreach( var element in GetObjects<GroupOfObjectsElement>() )
			{
				var groupOfObjects = element.Parent as GroupOfObjects;
				if( groupOfObjects != null )
				{
					var indexes = element.GetObjectsOfElement();
					if( indexes.Count != 0 )
					{
						var action = new GroupOfObjectsUndo.UndoActionCreateDelete( groupOfObjects, indexes.ToArray(), false, true );
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
			var groupsOfObjectToUpdate = new ESet<GroupOfObjects>();

			foreach( var element in GetObjects<GroupOfObjectsElement>() )
			{
				var groupOfObjects = element.Parent as GroupOfObjects;
				if( groupOfObjects != null )
				{
					var elementMesh = element as GroupOfObjectsElement_Mesh;
					if( elementMesh != null )
					{
						elementMesh.UpdateAlignment( undoMultiAction );
						groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
					}

					var elementSurface = element as GroupOfObjectsElement_Surface;
					if( elementSurface != null )
					{
						elementSurface.UpdateVariationsAndTransform( false, undoMultiAction, false, true );
						//elementSurface.UpdateAlignment( undoMultiAction );
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
			var groupsOfObjectToUpdate = new ESet<GroupOfObjects>();

			foreach( var elementSurface in GetObjects<GroupOfObjectsElement_Surface>() )
			{
				var groupOfObjects = elementSurface.Parent as GroupOfObjects;
				if( groupOfObjects != null )
				{
					elementSurface.UpdateVariationsAndTransform( randomizeGroups, undoMultiAction, true, true );
					//elementSurface.UpdateVariations( randomizeGroups, undoMultiAction );
					groupsOfObjectToUpdate.AddWithCheckAlreadyContained( groupOfObjects );
				}
			}

			foreach( var groupOfObjects in groupsOfObjectToUpdate )
				groupOfObjects.CreateSectors();

			if( undoMultiAction.Actions.Count != 0 )
				Provider.DocumentWindow.Document.CommitUndoAction( undoMultiAction );
		}

		void SetColors( ColorValue color )
		{
			var undoMultiAction = new UndoMultiAction();
			var groupsOfObjectToUpdate = new ESet<GroupOfObjects>();

			foreach( var elementSurface in GetObjects<GroupOfObjectsElement>() )
			{
				var groupOfObjects = elementSurface.Parent as GroupOfObjects;
				if( groupOfObjects != null )
				{
					elementSurface.SetColors( undoMultiAction, color );
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

		public class SetColorsSettings
		{
			ColorValue color = new ColorValue( 1, 1, 1 );
			//[ DefaultValue( 1 )]
			[Category( "Basic" )]
			public ColorValue Color
			{
				get { return color; }
				set { color = value; }
			}
		}

		private void ButtonSetColors_Click( ProcedureUI.Button obj )
		{
			//!!!!set colors interval or function

			var settings = new SetColorsSettings();

			var form = new SpecifyParametersForm( "Set Colors", settings );
			form.CheckHandler = delegate ( ref string error2 )
			{
				return true;
			};
			if( form.ShowDialog() != DialogResult.OK )
				return;

			SetColors( settings.Color );
		}

		//private void ButtonResetColors_Click( ProcedureUI.Button obj )
		//{
		//	if( EditorMessageBox.ShowQuestion( Translate( "Reset color of the objects to \'1 1 1\'?" ), EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
		//		return;

		//	SetColors( ColorValue.One );
		//	//ResetColors();
		//}
	}
}

#endif