#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Ribbon;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Docking;
using Internal.ComponentFactory.Krypton.Workspace;
using System.IO;
using Internal.Fbx;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a window for selection reference value.
	/// </summary>
	public partial class SetReferenceWindow : DockWindow
	{
		public DocumentWindow documentWindow;
		public ContentBrowser.SetReferenceModeDataClass setReferenceModeData;

		bool contentBrowserItemAfterSelectDisabled;

		bool editBoxTextChangedEnabled = true;

		bool manuallyEdited;
		//!!!!multiselection
		string selectedReference;
		bool selectedReferenceCanSet;

		Color? textBoxCommonBorderColor;

		//

		public SetReferenceWindow()
		{
			InitializeComponent();

			WindowTitle = EditorLocalization2.Translate( "SetReferenceWindow", WindowTitle );
			EditorLocalization2.TranslateForm( "SetReferenceWindow", this );
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( !WinFormsUtility.IsDesignerHosted( this ) )
			{
				contentBrowserItemAfterSelectDisabled = true;
				UpdateContentBrowser( out var selected );
				contentBrowserItemAfterSelectDisabled = false;

				if( selected )
				{
					UpdateTextBox();
				}
				else
				{
					if( GetInitialReference( out var reference ) )
					{
						kryptonTextBox1.Text = reference;

						TextBoxSetError( true );
						kryptonTextBox1.Select();
					}
				}

				if( !manuallyEdited )
					UpdateSelectedReference();
				UpdateSetButtons();

				timer1.Start();
			}
		}

		/// <summary>
		/// Getting from only created items. Not all _Member items can be created (expanded).
		/// </summary>
		/// <param name="reference"></param>
		/// <returns></returns>
		ContentBrowser.Item GetItemByReference( string reference )
		{
			//!!!!slowly

			//!!!!multiselection?

			//_Null
			if( string.IsNullOrEmpty( reference ) )
			{
				foreach( var item in contentBrowser1.Items )
				{
					if( item is ContentBrowserItem_Null )
						return item;
				}
				return null;
			}

			foreach( var item in contentBrowser1.Items )
			{
				item.CalculateReferenceValue( setReferenceModeData.selectedComponents[ 0 ], setReferenceModeData.property.TypeUnreferenced,
					out string itemReferenceValue, out bool itemCanSet );

				if( ReferenceUtility.EqualsReferences( itemReferenceValue, reference ) )
					return item;
			}

			return null;
		}

		bool SelectReference( string reference, bool expand )
		{
			//!!!!slowly

			var item = GetItemByReference( reference );
			if( item == null && !string.IsNullOrEmpty( reference ) )
			{
				//create items by the path

				//get list of references to expand
				List<string> toCheck = new List<string>();
				{
					string current = reference;

again:;

					int splitIndex = current.LastIndexOfAny( new char[] { '\\', '/', '|' } );
					if( splitIndex != -1 )
					{
						var reference2 = current.Substring( 0, splitIndex );
						if( !string.IsNullOrEmpty( reference2 ) )
						{
							current = reference2;
							toCheck.Add( current );
							goto again;
						}
					}
				}

				//process the list (from back)
				foreach( var reference2 in toCheck.GetReverse() )
				{
					var item2 = GetItemByReference( reference2 );
					if( item2 != null )
					{
						contentBrowser1.SelectItems( new ContentBrowser.Item[] { item2 }, true );

						//не надо, т.к. expand делается
						//contentBrowser1.AddChildNodes( item2 );
					}
				}

				//try get again
				item = GetItemByReference( reference );
			}

			if( item != null )
				contentBrowser1.SelectItems( new ContentBrowser.Item[] { item }, expand );

			return item != null;
		}

		bool GetInitialReference( out string reference )
		{
			if( setReferenceModeData.propertyOwners.Length != 0 )
			{
				//!!!!multiselection
				var obj = setReferenceModeData.propertyOwners[ 0 ];

				try
				{
					var value = setReferenceModeData.property.GetValue( obj, setReferenceModeData.propertyIndexes );
					if( value != null )
					{
						var iReference = value as IReference;
						if( iReference != null )
						{
							reference = iReference.GetByReference;
							if( reference == null )
								reference = "";

							return true;
						}
					}
				}
				catch { }
			}

			reference = "";
			return false;
		}

		void UpdateContentBrowser( out bool selected )
		{
			selected = false;

			contentBrowser1.Init( documentWindow, null, /*null, */setReferenceModeData );
			contentBrowser1.UpdateData();

			if( GetInitialReference( out var reference ) )
			{
				//!!!!
				//convert relative file paths
				//!!!!multiselection?
				if( ReferenceUtility.ConvertRelativePathToResource( reference, setReferenceModeData.selectedComponents[ 0 ], out var reference2 ) )
				{
					reference = reference2;
					kryptonCheckBoxCanMakeRelativeFilePath.Checked = true;
				}

				bool expand = false;

				//if reference is empty, then select folder of resource
				if( string.IsNullOrEmpty( reference ) )
				{
					if( setReferenceModeData.propertyOwners.Length != 0 )
					{
						//!!!!multiselection
						var obj = setReferenceModeData.propertyOwners[ 0 ];

						//when inside ReferenceList
						var objType = obj.GetType();
						var isReferenceList = objType.Name == "ReferenceList`1";//typeof( ReferenceList<> ).IsAssignableFrom( objType );
						if( isReferenceList )
							obj = ObjectEx.PropertyGet( obj, "Owner" );

						var component = obj as Component;
						if( component != null )
						{
							var path = ComponentUtility.GetOwnedFileNameOfComponent( component );
							if( !string.IsNullOrEmpty( path ) )
							{
								try
								{
									reference = Path.GetDirectoryName( path );
									expand = true;
								}
								catch { }
							}
						}
					}
				}

				try
				{
					selected = SelectReference( reference, expand );
				}
				catch { }
			}

			//if( setReferenceModeData.propertyOwners.Length != 0 )
			//{
			//	//!!!!multiselection
			//	var obj = setReferenceModeData.propertyOwners[ 0 ];

			//	try
			//	{
			//		var value = setReferenceModeData.property.GetValue( obj, setReferenceModeData.propertyIndexes );
			//		if( value != null )
			//		{
			//			var iReference = value as IReference;
			//			if( iReference != null )
			//			{
			//				var reference = iReference.GetByReference;
			//				if( reference == null )
			//					reference = "";

			//				selected = SelectReference( reference );
			//			}
			//		}
			//	}
			//	catch { }
			//}
		}

		[Browsable( false )]
		public DocumentWindow DocumentWindow
		{
			get { return documentWindow; }
		}

		[Browsable( false )]
		public ContentBrowser.SetReferenceModeDataClass SetReferenceModeData
		{
			get { return setReferenceModeData; }
		}

		[Browsable( false )]
		public string SelectedReference
		{
			get { return selectedReference; }
		}

		[Browsable( false )]
		public bool SelectedReferenceCanSet
		{
			get { return selectedReferenceCanSet; }
		}

		static int GetEqualPart( IList<string> strings )
		{
			var minimumLength = strings.Min( x => x.Length );
			int commonChars;
			for( commonChars = 0; commonChars < minimumLength; commonChars++ )
			{
				if( strings.Select( x => x[ commonChars ] ).Distinct().Count() > 1 )
					break;
			}
			return commonChars;
			//return strings[ 0 ].Substring( 0, commonChars );
		}

		void UpdateSelectedReference()
		{
			//!!!!не только компоненты?

			//!!!!multiselection
			ContentBrowser.Item item = null;
			if( contentBrowser1.SelectedItems.Length != 0 )
				item = contentBrowser1.SelectedItems[ 0 ];

			string newSelection = null;
			bool canSet = false;

			if( item != null )
			{
				//!!!!multiselection?

				item.CalculateReferenceValue( setReferenceModeData.selectedComponents[ 0 ], setReferenceModeData.property.TypeUnreferenced, out newSelection, out canSet );

				//make relative file path
				if( kryptonCheckBoxCanMakeRelativeFilePath.Checked && !string.IsNullOrEmpty( newSelection ) )
				{
					if( !newSelection.Contains( ':' ) )//ignore references
					{
						var fromResource = ComponentUtility.GetOwnedFileNameOfComponent( setReferenceModeData.selectedComponents[ 0 ] );
						var fromResourceFolder = "";
						if( !string.IsNullOrEmpty( fromResource ) )
							fromResourceFolder = Path.GetDirectoryName( fromResource );

						int commonChars = GetEqualPart( new string[] { fromResourceFolder, newSelection } );

						var path = "";

						//go up (dots)
						{
							var c = fromResourceFolder.Substring( commonChars ).Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries ).Length;
							for( int n = 0; n < c; n++ )
								path = Path.Combine( path, ".." );
						}

						//go down
						{
							var s = newSelection.Substring( commonChars );
							if( s.Length != 0 && ( s[ 0 ] == '\\' || s[ 0 ] == '/' ) )
								s = s.Substring( 1 );

							//can't use Path.Combine, invalid character exception
							if( path.Length != 0 && path[ path.Length - 1 ] != '\\' && path[ path.Length - 1 ] != '/' )
								path += "\\";
							path += s;
							//path = Path.Combine( path, s );
						}

						newSelection = "relative:" + path;
					}
				}
			}

			//update
			selectedReference = newSelection;
			selectedReferenceCanSet = canSet;
			UpdateSetButtons();
			UpdateTextBox();
			TextBoxSetError( false );
			////change
			//if( string.Compare( selectedReference, newSelection ) != 0 )
			//{
			//	selectedReference = newSelection;
			//	selectedReferenceCanSet = canSet;
			//	SelectedReferenceChanged();
			//}
		}

		private void contentBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( !contentBrowserItemAfterSelectDisabled )
			{
				manuallyEdited = false;
				UpdateSelectedReference();
			}
		}

		private void contentBrowser1_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( !IsReadyToSet() )
				return;
			ButtonSetAndClose_Click( null, null );
		}

		string GetExtension( Metadata.TypeInfo type )
		{
			//!!!!а как для своих типов расширения добавлять. нужно атрибуты уметь типу добавлять. тогда еще и свои типы атрибутов может быть

			var ar = type.GetNetType().GetCustomAttributes( typeof( ResourceFileExtensionAttribute ), true );
			if( ar.Length != 0 )
			{
				var attr = (ResourceFileExtensionAttribute)ar[ 0 ];
				return attr.Extension;
			}

			//!!!!!
			return "UNKNOWN";
		}

		bool IsReadyToSet()
		{
			//!!!!!что еще проверять

			if( SelectedReference == null )
				return false;
			if( !SelectedReferenceCanSet )
				return false;

			return true;
		}

		void UpdateSetButtons()
		{
			buttonSet.Enabled = IsReadyToSet();
			buttonSetAndClose.Enabled = buttonSet.Enabled;
		}

		void UpdateTextBox()
		{
			editBoxTextChangedEnabled = false;
			kryptonTextBox1.Text = SelectedReference;
			editBoxTextChangedEnabled = true;

			//if( !string.IsNullOrEmpty( SelectedReference ) )
			//	labelSelectedReference.Text = SelectedReference;
			//else
			//	labelSelectedReference.Text = "(Null)";
		}

		private void ButtonSet_Click( object sender, EventArgs e )
		{
			if( !SetProperty() )
				return;
		}

		private void ButtonSetAndClose_Click( object sender, EventArgs e )
		{
			if( !SetProperty() )
				return;

			Close();
		}

		private void ButtonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		bool SetProperty()
		{
			//!!!!multiselection
			//!!!!!!also this support for different objects

			string referenceValue = SelectedReference;

			var netType = setReferenceModeData.property.Type.GetNetType();
			var underlyingType = ReferenceUtility.GetUnderlyingType( netType );

			List<UndoActionPropertiesChange.Item> undoItems = null;
			if( documentWindow != null )
				undoItems = new List<UndoActionPropertiesChange.Item>();


			//!!!!try, catch? где еще

			foreach( var obj in setReferenceModeData.propertyOwners )
			{
				//object oldValue = null;
				//if( undoItems != null )
				var oldValue = setReferenceModeData.property.GetValue( obj, setReferenceModeData.propertyIndexes );

				object value = null;
				if( referenceValue == "" )
				{
					//reset reference with saving value for not components

					var unrefOldValue = ReferenceUtility.GetUnreferencedValue( oldValue );
					//Component specific: set null for Component
					if( unrefOldValue as Component != null )
						unrefOldValue = null;

					value = ReferenceUtility.MakeReference( underlyingType, unrefOldValue, "" );
				}
				else
					value = ReferenceUtility.MakeReference( underlyingType, null, referenceValue );

				setReferenceModeData.property.SetValue( obj, value, setReferenceModeData.propertyIndexes );

				if( undoItems != null )
				{
					var undoItem = new UndoActionPropertiesChange.Item( obj, setReferenceModeData.property, oldValue, setReferenceModeData.propertyIndexes );
					undoItems.Add( undoItem );
				}
			}

			//undo
			if( undoItems != null && undoItems.Count != 0 )
			{
				var action = new UndoActionPropertiesChange( undoItems.ToArray() );
				documentWindow.Document2.UndoSystem.CommitAction( action );
				documentWindow.Document2.Modified = true;
			}

			return true;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			//need periodically check for case when component inside _File item was loaded
			if( !manuallyEdited )
				UpdateSelectedReference();
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( contentBrowser1.DocumentWindow, contentBrowser1.GetSelectedContainedObjects() );
		}

		private void kryptonTextBox1_TextChanged( object sender, EventArgs e )
		{
			if( editBoxTextChangedEnabled )
			{
				manuallyEdited = true;
				selectedReference = kryptonTextBox1.Text;
				selectedReferenceCanSet = true;
				UpdateSetButtons();
			}
		}

		void TextBoxSetError( bool error )
		{
			if( textBoxCommonBorderColor == null )
				textBoxCommonBorderColor = kryptonTextBox1.StateCommon.Border.Color1;

			if( error )
				kryptonTextBox1.StateCommon.Border.Color1 = Color.Red; // HARDCODED: use Krypton themes system.
			else
				kryptonTextBox1.StateCommon.Border.Color1 = textBoxCommonBorderColor.Value;
		}
	}
}

#endif