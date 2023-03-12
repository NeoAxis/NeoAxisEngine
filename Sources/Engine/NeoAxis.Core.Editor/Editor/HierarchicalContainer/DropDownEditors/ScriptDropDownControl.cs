// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace NeoAxis.Editor
{
	public partial class ScriptDropDownControl : HCDropDownControl
	{
		HCItemProperty itemProperty;
		HCItemProperty propertyItemForUndoSupport;
		bool valueWasChanged;

		//bool valueChanging;

		/////////////////////////////////////////

		class HCItemPropertyScriptAdapter : ScriptDocument
		{
			HCItemProperty itemProperty;

			//public override Type ContextType { get { return typeof( CSharpScriptContext ); } }
			public override bool IsCSharpScript { get { return true; } }

			public HCItemPropertyScriptAdapter( HCItemProperty itemProperty )
			{
				this.itemProperty = itemProperty;
			}

			public override string LoadText()
			{
				string value = (string)ReferenceUtility.GetUnreferencedValue( itemProperty.GetValues()[ 0 ] );
				return value;
			}

			//public override bool SaveText( Microsoft.CodeAnalysis.Text.SourceText text )
			//{
			//	//itemProperty.SetValue( text.ToString(), false );
			//	return true;
			//}
		}

		/////////////////////////////////////////

		public ScriptDropDownControl()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public ScriptDropDownControl( HCItemProperty itemProperty )
		{
			InitializeComponent();

			Resizable = true;
			UseFormDropDownHolder = true;
			//DoubleBuffered = true;
			ResizeRedraw = true;
			//MinimumSize = Size;
			//MaximumSize = new Size( Size.Width * 2, Size.Height * 2 );

			AddOkCancelButtons( out _, out _ );

			this.itemProperty = itemProperty;

			propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
			propertyItemForUndoSupport.SaveValuesToRestore();

			try
			{
				scriptEditorControl.Initialize( new HCItemPropertyScriptAdapter( itemProperty ) );
			}
			catch( Exception exc )
			{
				Log.Warning( "Script editor control init failed: \n\n" + exc.ToString() );
				Enabled = false;
			}

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public override void OnCommitChanges()
		{
			base.OnCommitChanges();

			//if( scriptEditorControl.IsDirty )
			UpdateItemProperty();

			if( valueWasChanged )
				propertyItemForUndoSupport.AddUndoActionWithSavedValuesToRestore();
		}

		public override void OnCancelChanges()
		{
			base.OnCancelChanges();

			if( valueWasChanged )
				propertyItemForUndoSupport.RestoreSavedOldValues();
		}

		void UpdateItemProperty()
		{
			if( itemProperty != null && itemProperty.CanEditValue() )
			{
				if( scriptEditorControl.GetCode( out var code ) )
				{
					if( scriptEditorControl.InitialCode != code )
					{
						itemProperty.SetValue( code, false );
						valueWasChanged = true;
					}
				}
			}

			//if( itemProperty != null && itemProperty.CanEditValue() )
			//{
			//	if( scriptEditorControl.GetCode( out var code ) )
			//		itemProperty.SetValue( code, false );

			//	//scriptEditorControl.SaveScript();
			//}
			//valueWasChanged = true;
		}

		//void runScriptKryptonButton_Click( object sender, EventArgs e )
		//{
		//	//var editor = ScriptEditor.Instance;
		//	//editor.RunScript( scriptEditorControl.Script );
		//}
	}
}
