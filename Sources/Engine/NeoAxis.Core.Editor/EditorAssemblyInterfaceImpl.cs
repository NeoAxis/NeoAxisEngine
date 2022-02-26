// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using RoslynPad.Roslyn;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using NeoAxis.Import;

namespace NeoAxis.Editor
{
	//NeoAxis.Core.Editor.dll is need to separate WPF, RoslynPad classes from NeoAxis.Core.dll.

	class EditorAssemblyInterfaceImpl : EditorAssemblyInterface
	{
		public static void Init()
		{
			new EditorAssemblyInterfaceImpl();

			RoslynPadNeoAxisCoreExchangeImpl.Init();
		}

		/////////////////////////////////////////

		public override IScriptPrinter ScriptPrinterNew()
		{
			return new ScriptPrinter();// { BackgroundBrush = System.Windows.Media.Brushes.LightGray };
		}

		public override Dictionary<string, Type> DocumentWindowClassByFileExtension
		{
			get
			{
				return new Dictionary<string, Type>()
				{
					{ "txt", typeof( TextEditorDocumentWindow ) },
					{ "settings", typeof( TextEditorDocumentWindow ) },
					{ "cs", typeof( CSharpDocumentWindow ) }
				};
			}
		}

		public override void UpdateProjectFileForCSharpEditor( ICollection<string> addFiles, ICollection<string> removeFiles )
		{
			if( RoslynHost.Instance != null )
			{
				if( addFiles != null )
				{
					foreach( var file in addFiles )
					{
						string fullPath = file;
						if( !Path.IsPathRooted( fullPath ) )
							fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fullPath );
						try
						{
							RoslynHost.Instance.OnAddCsFileToProject( fullPath );
						}
						catch { }
					}
				}
				if( removeFiles != null )
				{
					foreach( var file in removeFiles )
					{
						string fullPath = file;
						if( !Path.IsPathRooted( fullPath ) )
							fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fullPath );
						try
						{
							RoslynHost.Instance.OnRemoveCsFileFromProject( fullPath );
						}
						catch { }
					}
				}
			}
		}

		public override void InitializeWPFApplicationAndScriptEditor()
		{
			if( System.Windows.Application.Current == null )
			{
				// create the Application object for WPF support
				new WPFApp() { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
			}

			ScriptEditorEngine.Instance.Initialize();
		}

		public override Type GetTypeByName( string typeName )
		{
			return Assembly.GetExecutingAssembly().GetType( typeName );
		}

		public override void SetDarkTheme()
		{
			AvalonEditDarkThemeUtility.DarkTheme = true;
		}

		public override ITextEditorControl CreateTextEditorControl()
		{
			var control = new TextEditorControl();

			control.ManageColorsFromTextEditorProjectSettings = true;
			control.ManageFontFromTextEditorProjectSettings = true;

			control.InstallSearchReplacePanel();

			return control;
		}

		public override void ImportFBX( ImportGeneral.Settings settings, out string error )
		{
			Import.FBX.ImportFBX.DoImport( settings, out error );
		}

		public override void ImportAssimp( ImportGeneral.Settings settings, out string error )
		{
			Import.ImportAssimp.DoImport( settings, out error );
		}

		public override bool ExportToFBX( Mesh sourceMesh, string realFileName, out string error )
		{
			return MeshExportImport.ExportToFBX( sourceMesh, realFileName, out error );
		}

		public override HCDropDownControl CreateColorValuePoweredSelectControl( HCItemProperty itemProperty )
		{
			return new ColorValuePoweredSelectControl( itemProperty );
		}

		public override bool ColorValuePoweredSelectFormShowDialog( Point location, ColorValuePowered initialColor, out ColorValuePowered resultColor )
		{
			var form = new ColorValuePoweredSelectForm();
			form.StartPosition = FormStartPosition.Manual;
			form.Location = location;
			form.Init( initialColor, false, false, null, false );

			if( form.ShowDialog( EditorForm.Instance ) == DialogResult.OK )
			{
				resultColor = form.CurrentValue;
				return true;
			}
			resultColor = ColorValuePowered.Zero;
			return false;
		}
	}
}
