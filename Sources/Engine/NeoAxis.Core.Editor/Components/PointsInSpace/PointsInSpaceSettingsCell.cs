// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

namespace NeoAxis.Editor
{
	public class PointsInSpaceSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonImport;
		ProcedureUI.Button buttonGenerate;

		//

		protected override void OnInit()
		{
			buttonImport = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Import" ) );
			buttonImport.Click += ButtonImport_Click;

			buttonGenerate = ProcedureForm.CreateButton( EditorLocalization.Translate( "General", "Generate" ) );
			buttonGenerate.Click += ButtonGenerate_Click;
			ProcedureForm.AddRow( new[] { buttonImport, buttonGenerate } );
		}

		private void ButtonImport_Click( ProcedureUI.Button sender )
		{
			var pointsInSpace = GetFirstObject<PointsInSpace>();
			if( pointsInSpace != null )
			{
				var filters = new[]
				{
					("All Supported Files (*.xyz;*.txt;*.pts)", "*.xyz;*.txt;*.pts"),
					("XYZ Files (*.xyz)", "*.xyz"),
					("PTS Files (*.pts)", "*.pts"),
					("TXT Files (*.txt)", "*.txt"),
				};

				var initialDirectory = "";
				try
				{
					var fileName = ComponentUtility.GetOwnedFileNameOfComponent( pointsInSpace );
					if( !string.IsNullOrEmpty( fileName ) )
						initialDirectory = Path.GetDirectoryName( VirtualPathUtility.GetRealPathByVirtual( fileName ) );
				}
				catch { }

				if( EditorUtility2.ShowOpenFileDialog( false, initialDirectory, filters, EditorForm.Instance.Handle, out string sourceRealFileName ) )
				{
					var inputData = new PointCloudImport.InputData();
					inputData.PointLimit = pointsInSpace.PointLimit;
					inputData.SourceRealFileName = sourceRealFileName;

					var result = PointCloudImport.Load( inputData, out var error );

					if( !string.IsNullOrEmpty( error ) )
					{
						Log.Warning( error );
						ScreenNotifications.Show( error, true );
					}
					else
					{
						var undoItems = new List<UndoActionPropertiesChange.Item>();

						//Positions
						{
							var oldValue = pointsInSpace.Positions;
							pointsInSpace.Positions = result.Positions;
							var property = (Metadata.Property)pointsInSpace.MetadataGetMemberBySignature( "property:Positions" );
							undoItems.Add( new UndoActionPropertiesChange.Item( pointsInSpace, property, oldValue ) );
						}

						//Colors
						{
							var oldValue = pointsInSpace.Colors;
							pointsInSpace.Colors = result.Colors.Length > 0 ? result.Colors : null;
							var property = (Metadata.Property)pointsInSpace.MetadataGetMemberBySignature( "property:Colors" );
							undoItems.Add( new UndoActionPropertiesChange.Item( pointsInSpace, property, oldValue ) );
						}

						var undoAction = new UndoActionPropertiesChange( undoItems );
						Provider.DocumentWindow.Document.CommitUndoAction( undoAction );

						ScreenNotifications.Show( "Points imported successfully." );
					}
				}
			}
		}

		private void ButtonGenerate_Click( ProcedureUI.Button sender )
		{
			var pointsInSpace = GetFirstObject<PointsInSpace>();
			if( pointsInSpace != null )
			{
				var text = "Generate random points?";
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.OK )
				{
					var count = pointsInSpace.PointLimit.Value;
					var garabites = new Bounds( -0.5, -0.5, -0.5, 0.5, 0.5, 0.5 );

					var random = new FastRandom();

					var positions = new Vector3F[ count ];
					var colors = new ColorByte[ count ];

					for( int n = 0; n < count; n++ )
					{
						var position = new Vector3(
							random.Next( garabites.Minimum.X, garabites.Maximum.X ),
							random.Next( garabites.Minimum.Y, garabites.Maximum.Y ),
							random.Next( garabites.Minimum.Z, garabites.Maximum.Z ) );
						var color = new ColorValue( random.NextFloat(), random.NextFloat(), random.NextFloat() );

						positions[ n ] = position.ToVector3F();
						colors[ n ] = color.ToColorPacked();
					}

					var undoItems = new List<UndoActionPropertiesChange.Item>();

					//Positions
					{
						var oldValue = pointsInSpace.Positions;
						pointsInSpace.Positions = positions;
						var property = (Metadata.Property)pointsInSpace.MetadataGetMemberBySignature( "property:Positions" );
						undoItems.Add( new UndoActionPropertiesChange.Item( pointsInSpace, property, oldValue ) );
					}

					//Colors
					{
						var oldValue = pointsInSpace.Colors;
						pointsInSpace.Colors = colors;
						var property = (Metadata.Property)pointsInSpace.MetadataGetMemberBySignature( "property:Colors" );
						undoItems.Add( new UndoActionPropertiesChange.Item( pointsInSpace, property, oldValue ) );
					}

					var undoAction = new UndoActionPropertiesChange( undoItems );
					Provider.DocumentWindow.Document.CommitUndoAction( undoAction );
				}
			}
		}
	}
}