// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	static class EditorAssembly
	{
		public static void Init()
		{
			var assembly = AssemblyUtility.LoadAssemblyByRealFileName( "NeoAxis.Core.Editor.dll", false, false );

			var type = assembly.GetType( "NeoAxis.Editor.EditorAssemblyInterfaceImpl" );
			if( type == null )
				Log.Fatal( "EditorAssembly: Init: Type \"NeoAxis.Editor.EditorAssemblyInterfaceImpl\" is not exists." );

			var initMethod = type.GetMethod( "Init", BindingFlags.Public | BindingFlags.Static );
			if( initMethod == null )
				Log.Fatal( "EditorAssembly: Init: \"Init\" method of \"NeoAxis.Editor.EditorAssemblyInterfaceImpl\" type is not exists." );

			initMethod.Invoke( null, new object[ 0 ] );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	internal abstract class EditorAssemblyInterface
	{
		static EditorAssemblyInterface instance;
		public static EditorAssemblyInterface Instance { get { return instance; } }
		protected EditorAssemblyInterface() { instance = this; }

		/////////////////////////////////////////

		public abstract void SetDarkTheme();

		public abstract IScriptPrinter ScriptPrinterNew();

		public abstract Dictionary<string, Type> DocumentWindowClassByFileExtension { get; }

		public abstract void UpdateProjectFileForCSharpEditor( ICollection<string> addFiles, ICollection<string> removeFiles );

		public abstract void InitializeWPFApplicationAndScriptEditor();

		public abstract Type GetTypeByName( string typeName );

		public interface ITextEditorControl
		{
			string EditorText { get; set; }
			bool EditorReadOnly { get; set; }
			bool EditorWordWrap { get; set; }
			bool Border { get; set; }

			int SelectionStart { get; set; }
			int SelectionLength { get; set; }
			void Select( int start, int length );

			void ScrollToHome();
			void ScrollToEnd();
		}

		public abstract ITextEditorControl CreateTextEditorControl();

		/////////////////////////////////////////

		public interface IScriptPrinter
		{
			Component_Image PrintToTexture( string code, Vector2I size );
		}
	}
}
