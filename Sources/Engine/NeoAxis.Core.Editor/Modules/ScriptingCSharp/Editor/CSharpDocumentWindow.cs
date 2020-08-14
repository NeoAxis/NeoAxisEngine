// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using NeoAxis.Editor;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using System.Linq;

namespace NeoAxis.Editor
{
	public partial class CSharpDocumentWindow : DocumentWindow
	{
		[EngineConfig( "AddPropertyCode", "ClassName" )]
		public static string addPropertyCode_ClassName = "Component_";

		[EngineConfig( "AddPropertyCode", "PropertyName" )]
		public static string addPropertyCode_PropertyName = "Name";

		[EngineConfig( "AddPropertyCode", "PropertyType" )]
		public static string addPropertyCode_PropertyType = "double";

		[EngineConfig( "AddPropertyCode", "DefaultValue" )]
		public static string addPropertyCode_DefaultValue = "";

		[EngineConfig( "AddPropertyCode", "ReferenceSupport" )]
		public static bool addPropertyCode_ReferenceSupport = true;

		//[EngineConfig( "AddPropertyCode", "AddEvent" )]
		//public static bool addPropertyCode_AddEvent = true;

		[EngineConfig( "AddPropertyCode", "AddComments" )]
		public static bool addPropertyCode_AddComments = false;

		TextSpan? needSelectAndScrollToSpan;

		/////////////////////////////////////////

		class ScriptDocumentForCSharpFile : ScriptDocument
		{
			DocumentInstance document;

			public ScriptDocumentForCSharpFile( DocumentInstance document )
			{
				this.document = document;
			}

			public override bool IsCSharpScript
			{
				get { return false; }
			}

			public override string CsFileProjectPath
			{
				get
				{
					//!!!!
					return document.RealFileName;
					//var virtualPath = VirtualPathUtility.GetVirtualPathByReal( document.RealFileName );
					//return Path.Combine( "Assets", virtualPath );
				}
			}

			public override string WorkingDirectory
			{
				get { return Path.GetDirectoryName( document.RealFileName ); }
			}

			public override string LoadText()
			{
				try
				{
					using( var fileStream = File.OpenText( document.RealFileName ) )
						return fileStream.ReadToEnd();
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return "";
				}
			}

			//public override bool SaveText( Microsoft.CodeAnalysis.Text.SourceText text )
			//{
			//	using( var writer = File.CreateText( document.RealFileName ) )
			//	{
			//		for( int lineIndex = 0; lineIndex < text.Lines.Count - 1; ++lineIndex )
			//			writer.WriteLine( text.Lines[ lineIndex ].ToString() );
			//		writer.Write( text.Lines[ text.Lines.Count - 1 ].ToString() );
			//	}
			//	return true;
			//}
		}

		/////////////////////////////////////////

		public class AddPropertyCodeSettings
		{
			[DefaultValue( "Component_" )]
			[Category( "Settings" )]
			//[Description( "" )]
			public string ClassName
			{
				get { return className; }
				set { className = value; }
			}
			string className = "Component_";

			[DefaultValue( "Name" )]
			[Category( "Settings" )]
			//[Description( "" )]
			public string PropertyName
			{
				get { return propertyName; }
				set { propertyName = value; }
			}
			string propertyName = "Name";

			[DefaultValue( "double" )]
			[Category( "Settings" )]
			//[Description( "" )]
			public string PropertyType
			{
				get { return propertyType; }
				set { propertyType = value; }
			}
			string propertyType = "double";

			[DefaultValue( "" )]
			[Category( "Settings" )]
			//[Description( "" )]
			public string DefaultValue
			{
				get { return defaultValue; }
				set { defaultValue = value; }
			}
			string defaultValue = "";

			[DefaultValue( true )]
			[Category( "Settings" )]
			//[Description( "" )]
			public bool ReferenceSupport
			{
				get { return referenceSupport; }
				set { referenceSupport = value; }
			}
			bool referenceSupport = true;

			//[DefaultValue( true )]
			//[Category( "Settings" )]
			////[Description( "" )]
			//public bool AddEvent
			//{
			//	get { return addEvent; }
			//	set { addEvent = value; }
			//}
			//bool addEvent = true;

			[DefaultValue( false )]
			[Category( "Settings" )]
			//[Description( "" )]
			public bool AddComments
			{
				get { return addComments; }
				set { addComments = value; }
			}
			bool addComments = false;
		}

		/////////////////////////////////////////

		public CSharpDocumentWindow()
		{
			InitializeComponent();

			EngineConfig.RegisterClassParameters( typeof( CSharpDocumentWindow ) );
		}

		public override void InitDocumentWindow( DocumentInstance document, object objectOfWindow, bool openAsSettings, Dictionary<string, object> windowTypeSpecificOptions )
		{
			base.InitDocumentWindow( document, objectOfWindow, openAsSettings, windowTypeSpecificOptions );

			document.SaveEvent += Document_SaveEvent;
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.SaveEvent -= Document_SaveEvent;

			base.OnDestroy();
		}

		private void CSharpDocumentWindow_Load( object sender, EventArgs e )
		{
			try
			{
				scriptEditorControl.Initialize( new ScriptDocumentForCSharpFile( Document ) );
				scriptEditorControl.Editor.TextChanged += Editor_TextChanged;
			}
			catch( Exception exc )
			{
				Log.Warning( "Script editor control init failed: \n\n" + exc.ToString() );
				Enabled = false;
			}
		}

		private void Document_SaveEvent( DocumentInstance document, string saveAsFileName, ref bool handled, ref bool result )
		{
			if( Destroyed )
				return;

			if( !scriptEditorControl.GetCode( out var text ) )
			{
				Log.Warning( "Unable to get code from control." );
				result = false;
				handled = true;
				return;
			}

			try
			{
				if( !string.IsNullOrEmpty( saveAsFileName ) )
					File.WriteAllText( saveAsFileName, text );
				else
					File.WriteAllText( document.RealFileName, text );

				//using( var writer = File.CreateText( saveAsFileName ) )
				//{
				//	for( int lineIndex = 0; lineIndex < text.Lines.Count - 1; ++lineIndex )
				//		writer.WriteLine( text.Lines[ lineIndex ].ToString() );
				//	writer.Write( text.Lines[ text.Lines.Count - 1 ].ToString() );
				//}

				result = true;
			}
			catch( Exception e )
			{
				Log.Warning( e.Message );
				result = false;
			}

			//var saveResult = scriptEditorControl.SaveScript();

			handled = true;
			//result = saveResult;
		}

		[Browsable( false )]
		public ScriptEditorControl ScriptEditorControl
		{
			get { return scriptEditorControl; }
		}

		public override void EditorActionGetState( EditorAction.GetStateContext context )
		{
			switch( context.Action.Name )
			{
			case "Undo":
				context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanUndo;
				return;

			case "Redo":
				context.Enabled = scriptEditorControl.Editor != null && scriptEditorControl.Editor.CanRedo;
				return;

			case "Comment Selection":
				context.Enabled = true;
				break;

			case "Uncomment Selection":
				context.Enabled = true;
				break;

			case "Rename":
				context.Enabled = scriptEditorControl.GetRenameSymbol() != null;
				break;

			case "Format Document":
				context.Enabled = true;
				break;

			case "Add Property Code":
				context.Enabled = true;
				break;

			case "Go To Definition":
				context.Enabled = scriptEditorControl.CanGoToDefinition( out _ );
				break;
			}

			base.EditorActionGetState( context );
		}

		public override void EditorActionClick( EditorAction.ClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Undo":
				scriptEditorControl.Editor.Undo();
				return;

			case "Redo":
				scriptEditorControl.Editor.Redo();
				return;

			case "Comment Selection":
				scriptEditorControl.CommentUncommentSelection( true );
				return;

			case "Uncomment Selection":
				scriptEditorControl.CommentUncommentSelection( false );
				return;

			case "Rename":
				scriptEditorControl.TryShowRenameDialog();
				return;

			case "Format Document":
				scriptEditorControl.FormatDocument();
				return;

			case "Add Property Code":
				AddPropertyCode();
				return;

			case "Go To Definition":
				scriptEditorControl.TryGoToDefinition();
				return;
			}

			base.EditorActionClick( context );
		}

		private void Editor_TextChanged( object sender, EventArgs e )
		{
			if( !string.IsNullOrEmpty( Document.RealFileName ) )
				Document.Modified = true;
		}

		void AddPropertyCode()
		{
			var settings = new AddPropertyCodeSettings();

			//load settings
			//!!!!подставлять из текущего класса
			settings.ClassName = addPropertyCode_ClassName;
			settings.PropertyName = addPropertyCode_PropertyName;
			settings.PropertyType = addPropertyCode_PropertyType;
			settings.DefaultValue = addPropertyCode_DefaultValue;
			settings.ReferenceSupport = addPropertyCode_ReferenceSupport;
			//settings.AddEvent = addPropertyCode_AddEvent;
			settings.AddComments = addPropertyCode_AddComments;

			var form = new SpecifyParametersForm( "Add Property Code", settings );
			form.CheckHandler = delegate ( ref string error2 )
			{
				return true;
			};
			if( form.ShowDialog() != DialogResult.OK )
				return;

			string templateWithoutHelp =
				@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get
	{
		if( {FieldName}.BeginGet() )
			{PropertyName} = {FieldName}.Get( this );
		return {FieldName}.value;
	}
	set
	{
		if( {FieldName}.BeginSet( ref value ) )
		{
{CheckValue}
			try { {PropertyName}Changed?.Invoke( this ); }
			finally { {FieldName}.EndSet(); }
			{AdditionalActionsWrong}
		}
	}
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
";

			//[Metadata.Serialize]

			string templateCompact =
	@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get { if( {FieldName}.BeginGet() ) {PropertyName} = {FieldName}.Get( this ); return {FieldName}.value; }
	set { if( {FieldName}.BeginSet( ref value ) ) { {CheckValue} try { {PropertyName}Changed?.Invoke( this ); } finally { {FieldName}.EndSet(); } {AdditionalActionsWrong} } }
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
";

			//[Metadata.Serialize]

			string templateWithHelp =
	@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get
	{
		if( {FieldName}.BeginGet() )
			{PropertyName} = {FieldName}.Get( this );
		return {FieldName}.value;
	}
	set
	{
{CheckValue}
		if( {FieldName}.BeginSet( ref value ) )
		{
			try
			{
				{PropertyName}Changed?.Invoke( this );
				{AdditionalActions}
			}
			finally { {FieldName}.EndSet(); }
			{AdditionalActionsWrong}
		}
	}
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
				";


			string templateCompactNoReference =
	@"{DefaultValueAttribute}
public {TypeName} {PropertyName}
{
	get { return {FieldName}; }
	set { {FieldName} = value; }
}
{TypeName} {FieldName}{DefaultValueSet};
";


			string className = settings.ClassName;// textBoxClass.Text;
			var propertyName = settings.PropertyName;// textBoxPropertyName.Text;
			string fieldName = "";
			if( propertyName.Length > 0 )
				fieldName = "_" + propertyName.Substring( 0, 1 ).ToLower() + propertyName.Substring( 1 );
			string typeName = settings.PropertyType;// textBoxPropertyType.Text;
			string defaultValue = settings.DefaultValue;// textBoxDefaultValue.Text;
			bool help = settings.AddComments;// checkBoxHowToExpand.Checked;
			var compact = true;// checkBoxCompact.Checked;

			string text;

			if( settings.ReferenceSupport )
				text = help ? templateWithHelp : ( compact ? templateCompact : templateWithoutHelp );
			else
			{
				//!!!!

				text = templateCompactNoReference;
			}

			text = text.Replace( "{ClassName}", className );
			text = text.Replace( "{PropertyName}", propertyName );
			text = text.Replace( "{FieldName}", fieldName );
			text = text.Replace( "{TypeName}", typeName );
			text = text.Replace( "{Quote}", "\"" );

			if( !string.IsNullOrEmpty( defaultValue ) )
			{
				text = text.Replace( "{DefaultValueSet}", string.Format( " = {0}", defaultValue ) );
				text = text.Replace( "{DefaultValueAttribute}", string.Format( "[DefaultValue( {0} )]", defaultValue ) );
			}
			else
			{
				text = text.Replace( "{DefaultValueSet}", "" );
				text = text.Replace( "{DefaultValueAttribute}", "" );
			}

			if( help )
			{
				string checkValueText =
@"		@@@ Do here the fixing of 'value'. Don't use another properties.
		@@@ Example:
		@@@ if( value < 3 )
		@@@ 	value = new Reference<int>( 3, value.GetByReference );";

				text = text.Replace( "{CheckValue}", checkValueText );
				text = text.Replace( "{AdditionalActions}", "@@@ Add here additional actions. Update internal object, reset cache, etc." );
				text = text.Replace( "{AdditionalActionsWrong}", "@@@ Don't add additional actions here." );
			}
			else
			{
				text = text.Replace( "{CheckValue}", "" );
				text = text.Replace( "{AdditionalActions}", "" );
				text = text.Replace( "{AdditionalActionsWrong}", "" );
			}

			text = text.Replace( "\r\n\r\n", "\r\n" );
			text = text.Replace( "\r\n			\r\n", "\r\n" );
			text = text.Replace( "\r\n				\r\n", "\r\n" );

			//!!!!табы добавить или пробелы если пробелы

			//var secondLinePrefix = "";
			//!!!!
			//secondLinePrefix = "\t\t";
			//text = text.Replace( "\r\n", secondLinePrefix + "\r\n" );

			scriptEditorControl.AddCodeToCurrentPosition( text );

			//save settings
			addPropertyCode_ClassName = settings.ClassName;
			addPropertyCode_PropertyName = settings.PropertyName;
			addPropertyCode_PropertyType = settings.PropertyType;
			addPropertyCode_DefaultValue = settings.DefaultValue;
			addPropertyCode_ReferenceSupport = settings.ReferenceSupport;
			//addPropertyCode_AddEvent = settings.AddEvent;
			addPropertyCode_AddComments = settings.AddComments;
		}

		[Browsable( false )]
		public TextSpan? NeedSelectAndScrollToSpan
		{
			get { return needSelectAndScrollToSpan; }
			set { needSelectAndScrollToSpan = value; }
		}

		protected override void OnTimer10MsTick()
		{
			base.OnTimer10MsTick();

			if( NeedSelectAndScrollToSpan.HasValue )
			{
				var span = NeedSelectAndScrollToSpan.Value;

				try
				{
					var editor2 = ScriptEditorControl.Editor;

					editor2.CaretOffset = span.Start;
					editor2.Select( span.Start, span.Length );

					var text = editor2.Document.Text;
					if( text.Length >= span.Start )
					{
						var text2 = text.Substring( 0, span.Start );
						int line = text2.Count( f => f == '\n' ) + 1;
						editor2.ScrollToLine( line );
					}

					//location.GetLineSpan()
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}

				NeedSelectAndScrollToSpan = null;
			}
		}

	}
}
