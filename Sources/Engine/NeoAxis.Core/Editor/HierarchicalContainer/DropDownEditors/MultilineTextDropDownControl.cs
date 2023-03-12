#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class MultilineTextDropDownControl : HCDropDownControl
	{
		HCItemProperty itemProperty;
		HCItemProperty propertyItemForUndoSupport;
		string initialValue;
		bool valueWasChanged;

		KryptonButton okButton;
		KryptonButton cancelButton;

		/////////////////////////////////////////

		public MultilineTextDropDownControl()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public MultilineTextDropDownControl( HCItemProperty itemProperty )
		{
			InitializeComponent();

			Resizable = true;
			UseFormDropDownHolder = true;
			//DoubleBuffered = true;
			ResizeRedraw = true;
			//MinimumSize = Size;
			//MaximumSize = new Size( Size.Width * 2, Size.Height * 2 );

			AddOkCancelButtons( out okButton, out cancelButton );

			this.itemProperty = itemProperty;

			object obj = ReferenceUtility.GetUnreferencedValue( itemProperty.GetValues()[ 0 ] );
			var str = obj as string;
			if( str != null )
				engineTextBox.Text = str;
			initialValue = engineTextBox.Text;

			propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
			propertyItemForUndoSupport.SaveValuesToRestore();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public override void OnCommitChanges()
		{
			base.OnCommitChanges();

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
				if( engineTextBox.Text != initialValue )
				{
					itemProperty.SetValue( engineTextBox.Text, false );
					valueWasChanged = true;
				}
			}
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			if( engineTextBox != null && okButton != null )
			{
				engineTextBox.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 5 ) * 2, okButton.Location.Y - DpiHelper.Default.ScaleValue( 4 ) * 2 );
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			var keyCode = keyData & Keys.KeyCode;

			if( keyCode == Keys.Return && !ModifierKeys.HasFlag( Keys.Shift ) )
			{
				ParentHolder.Close( true );
				return true;
			}
			else if( keyCode == Keys.Return && ModifierKeys.HasFlag( Keys.Shift ) )
			{
				try
				{
					var insertText = "\r\n";
					var selectionIndex = engineTextBox.SelectionStart;
					engineTextBox.Text = engineTextBox.Text.Insert( selectionIndex, insertText );
					engineTextBox.SelectionStart = selectionIndex + insertText.Length;

					//!!!!undo? changed?
				}
				catch { }
				return true;
			}
			else if( keyCode == Keys.Escape )
			{
				ParentHolder.Close( false );
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}
	}
}

#endif