#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class ContentBrowserSearchForm : EngineForm
	{
		public ContentBrowserSearchForm()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
			Text = EditorLocalization.Translate( "ContentBrowser.SearchForm", Text );
			EditorLocalization.TranslateForm( "ContentBrowser.SearchForm", this );
		}

		[Browsable( false )]
		public ContentBrowser Browser { get; set; }

		private void ContentBrowserOptionsForm_Load( object sender, EventArgs e )
		{
			UpdateControls();
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		static bool IsSearchPatternMatch( string str, string searchPattern )
		{
			if( string.IsNullOrEmpty( searchPattern ) )
				return true;

			return str.ToLower().Contains( searchPattern.ToLower() );

			//if( searchPattern == "*" || searchPattern == "*.*" )
			//	return true;

			//string tempStr = str.ToLower();
			//string tempPattern = searchPattern.ToLower();

			//int strIndex = 0;
			//int patIndex = 0;
			//int lastWildCardIt = tempPattern.Length;
			//while( strIndex != tempStr.Length && patIndex != tempPattern.Length )
			//{
			//	if( tempPattern[ patIndex ] == '*' )
			//	{
			//		lastWildCardIt = patIndex;
			//		// Skip over looking for next character
			//		patIndex++;
			//		if( patIndex == tempPattern.Length )
			//		{
			//			// Skip right to the end since * matches the entire rest of the string
			//			strIndex = tempStr.Length;
			//		}
			//		else
			//		{
			//			// scan until we find next pattern character
			//			while( strIndex != tempStr.Length && tempStr[ strIndex ] != tempPattern[ patIndex ] )
			//				strIndex++;
			//		}
			//	}
			//	else
			//	{
			//		if( tempPattern[ patIndex ] != tempStr[ strIndex ] )
			//		{
			//			if( lastWildCardIt != tempPattern.Length )
			//			{
			//				// The last wildcard can match this incorrect sequence
			//				// rewind pattern to wildcard and keep searching
			//				patIndex = lastWildCardIt;
			//				lastWildCardIt = tempPattern.Length;
			//			}
			//			else
			//			{
			//				// no wildwards left
			//				return false;
			//			}
			//		}
			//		else
			//		{
			//			patIndex++;
			//			strIndex++;
			//		}
			//	}
			//}

			//// If we reached the end of both the pattern and the string, we succeeded
			//if( patIndex == tempPattern.Length && strIndex == tempStr.Length )
			//	return true;
			//else
			//	return false;
		}

		private void kryptonButtonSearch_Click( object sender, EventArgs e )
		{
			var rootComponent = Browser.RootObject as Component;
			if( rootComponent == null )
				return;

			string namePattern = kryptonTextBoxFilterByName.Text.Trim();
			//if( namePattern.Length > 0 && namePattern[ namePattern.Length - 1 ] != '*' )
			//	namePattern += '*';

			var toSelect = new ESet<Component>();

			if( ModifierKeys.HasFlag( Keys.Shift ) )
			{
				foreach( var item in Browser.SelectedItems.OfType<ContentBrowserItem_Component>() )
					toSelect.Add( item.Component );
			}

			//EntityType selectedType = TypeSelected;

			var components = new List<Component>( 256 );
			components.Add( rootComponent );
			components.AddRange( rootComponent.GetComponents( checkChildren: true ) );

			//var equalNamePattern = namePattern;
			//if( equalNamePattern.Length > 0 )
			//	equalNamePattern = equalNamePattern.Substring( 0, equalNamePattern.Length - 1 );

			foreach( var component in components )
			{
				//filter by name
				if( IsSearchPatternMatch( component.Name, namePattern ) )//|| component.Name == equalNamePattern )
				{
					////filter by type
					//if( selectedType != null && entity.Type != selectedType )
					//	continue;

					////filter by class
					//if( comboBoxClass.SelectedItem != null && selectedType == null )
					//{
					//	ClassTypeItem classTypeItem = (ClassTypeItem)comboBoxClass.SelectedItem;
					//	if( !classTypeItem.classType.IsAssignableFrom( entity.GetType() ) )
					//		continue;
					//}

					toSelect.AddWithCheckAlreadyContained( component );
				}
			}

			ContentBrowserUtility.SelectComponentItems( Browser, toSelect.ToArray() );
		}

		void UpdateControls()
		{
			buttonClose.Location = new Point( ClientSize.Width - buttonClose.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonClose.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			kryptonButtonSearch.Location = new Point( buttonClose.Location.X - kryptonButtonSearch.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonClose.Location.Y );
			kryptonTextBoxFilterByName.Width = ClientSize.Width - kryptonTextBoxFilterByName.Location.X - DpiHelper.Default.ScaleValue( 12 );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

	}
}

#endif