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

namespace NeoAxis.Editor
{
	public partial class NewObjectWindow : DockWindow
	{
		static ContentBrowserOptions sharedOptions;

		//!!!!обязательно ли в NewObjectWindow
		public CreationDataClass creationData;

		Metadata.TypeInfo selectedType;

		bool firstTick = true;

		/////////////////////////////////////

		public class CreationDataClass
		{
			public string initFileCreationDirectory = "";
			public DocumentWindow initDocumentWindow;
			public List<object> initParentObjects;
			public Metadata.TypeInfo initLockType;
			//!!!!new
			public bool initSupportAutoCreateAndClose;
			//public Metadata.TypeInfo initDemandedType;
			//public bool initDemandedTypeDisableChange;

			//used to select new objects
			public ContentBrowser createdFromContentBrowser;

			//!!!!всегда ли есть окно. если нет, то это Context

			//public Metadata.TypeInfo selectedType;
			//public delegate void ReplaceSelectedTypeDelegate( NewObjectWindow window );//, ref Metadata.TypeInfo type );
			//public ReplaceSelectedTypeDelegate replaceSelectedTypeFunction;

			public delegate bool BeforeCreateObjectsDelegate( NewObjectWindow window, Metadata.TypeInfo selectedType );
			public BeforeCreateObjectsDelegate beforeCreateObjectsFunction;

			public delegate void AdditionActionDelegate( NewObjectWindow window );
			public AdditionActionDelegate additionActionBeforeEnabled;
			public AdditionActionDelegate additionActionAfterEnabled;



			//data during creation. maybe split from initial data?
			public List<object> createdObjects;
			public List<object> createdObjectsToApplySettings = new List<object>();
			public List<Component> createdComponentsOnTopLevel = new List<Component>();

			public void ClearCreatedObjects()
			{
				createdObjects = null;
				createdObjectsToApplySettings = new List<object>();
				createdComponentsOnTopLevel = new List<Component>();
			}

			//public bool IsFileCreation()
			//{
			//	return initParentObjects == null;
			//}
		}

		/////////////////////////////////////

		static NewObjectWindow()
		{
			EngineConfig.SaveEvent += Config_SaveEvent;
		}

		public NewObjectWindow()
		{
			InitializeComponent();

			//KryptonSplitContainer childs layout broken. see comments in kryptonsplitcontainer.cs. Also Anchors works strange in .NET Core. 
			panelName.Width = panelName.Parent.Width - DpiHelper.Default.ScaleValue( 9 );

			//options
			if( sharedOptions != null )
				contentBrowser1.Options = sharedOptions;
			else
			{
				if( !Config_Load() )
				{
					////var data = new ContentBrowser.ResourcesModeDataClass();
					////data.selectionMode = ResourceSelectionMode.None;
					//contentBrowser1.Init( null, null, /*data, */null );
					contentBrowser1.Options.PanelMode = ContentBrowser.PanelModeEnum.TwoPanelsSplitHorizontally;
					contentBrowser1.Options.SplitterPosition = 3.0 / 5.0;
				}

				sharedOptions = contentBrowser1.Options;
			}

			CloseByEscape = true;

			WindowTitle = EditorLocalization.Translate( "NewObjectWindow", WindowTitle );
			EditorLocalization.TranslateForm( "NewObjectWindow", eUserControl1 );

			EditorThemeUtility.ApplyDarkThemeToForm( eUserControl1 );
			EditorThemeUtility.ApplyDarkThemeToForm( panelName );
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			labelName.Visible = IsFileCreation();

			if( IsFileCreation() )
				textBoxName.Text = Path.Combine( creationData.initFileCreationDirectory, "File.ext" );
			else
				textBoxName.Text = "Object";
			//textBoxName.Text = GetStartName();

			if( creationData.initLockType != null )//if( creationData.initDemandedTypeDisableChange )
				contentBrowser1.SetEnabled( false );

			UpdateContentBrowser();

			if( creationData.initLockType != null )//if( creationData.initDemandedTypeDisableChange )
				SelectInitLockType();

			//UpdateContentBrowser();

			SelectedTypeChanged();

			timer1.Start();
		}

		void UpdateContentBrowser()
		{
			var data = new ContentBrowser.SetReferenceModeDataClass();
			data.demandedType = MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) );
			//data.additionalCheckCanSet = delegate ( Metadata.TypeInfo typeToCheck, ref bool canSet )
			//{
			//	if( !MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeToCheck ) )
			//		canSet = false;
			//};
			data.allowNull = false;

			//creationData.initFileCreationDirectory
			data.newObjectWindow = true;
			data.newObjectWindowFileCreation = IsFileCreation();

			contentBrowser1.Init( creationData.initDocumentWindow, null, data );
			contentBrowser1.UpdateData();
			//var data = new ContentBrowser.ResourcesModeDataClass();
			//data.selectionMode = ResourceSelectionMode.Type;
			//data.demandedType = creationData.initDemandedType;
			//contentBrowser1.Init( creationData.initDocumentWindow, null, data, null );
			//contentBrowser1.UpdateData();
		}

		[Browsable( false )]
		public CreationDataClass CreationData
		{
			get { return creationData; }
		}

		[Browsable( false )]
		public Metadata.TypeInfo SelectedType
		{
			get { return selectedType; }
		}

		void UpdateSelectedType()
		{
			Metadata.TypeInfo newType = null;

			ContentBrowser.Item item = null;
			if( contentBrowser1.SelectedItems.Length != 0 )
				item = contentBrowser1.SelectedItems[ 0 ];

			if( item != null )
			{
				item.CalculateReferenceValue( null, MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ),
					out string referenceValue, out bool canSet );

				if( !string.IsNullOrEmpty( referenceValue ) && canSet )
				{
					newType = (Metadata.TypeInfo)MetadataManager.GetValueByReference(
						contentBrowser1.SetReferenceModeData.demandedType.GetNetType(), null, referenceValue );
				}

				//check can be selected
				if( newType != null )
				{
					if( !MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( newType ) &&
						!MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( newType ) )
						newType = null;
				}
			}

			//change
			if( selectedType != newType )
			{
				selectedType = newType;
				SelectedTypeChanged();
			}
		}

		private void contentBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateSelectedType();
		}

		private void contentBrowser1_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			if( !IsReadyToCreate( out string dummy ) )
				return;
			ButtonCreateAndClose_Click( null, null );
		}

		string GetDefaultName()
		{
			//default object name from attribute
			var ar = selectedType.GetNetType().GetCustomAttributes( typeof( NewObjectDefaultNameAttribute ), true );
			if( ar.Length != 0 )
				return ( (NewObjectDefaultNameAttribute)ar[ 0 ] ).Name;

			if( IsFileCreation() )
			{
				return "File.ext";
				//return Path.Combine( creationData.initFileCreationDirectory, "File.ext" );
			}
			else
			{
				//!!!!проверять валидность имени, т.к. слеши нельзя для компонент. для файлов можно

				return selectedType.GetUserFriendlyNameForInstance( true );
			}
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

		public bool IsFileCreation()
		{
			return creationData.initParentObjects == null;
		}

		void UpdateName()
		{
			if( selectedType != null )
			{
				var defaultName = GetDefaultName();

				if( IsFileCreation() )
				{
					var directoryName = "";
					try
					{
						directoryName = Path.GetDirectoryName( textBoxName.Text );
					}
					catch { }
					var name = Path.Combine( directoryName, defaultName );
					string ext = "." + GetExtension( selectedType );
					name = Path.ChangeExtension( name, ext );

					//string ext = "." + GetExtension( selectedType );
					//string name = Path.ChangeExtension( textBoxName.Text, ext );

					//!!!!!если не менялось имя в текстбаре, то сбрасывать (чтобы цифр не было)
					//!!!!чтобы не получалось 22

					//get unique name
					if( VirtualFile.Exists( name ) )
					{
						for( int n = 2; ; n++ )
						{
							var newName = Path.ChangeExtension( name, null ) + n.ToString() + Path.GetExtension( name );
							if( !VirtualFile.Exists( newName ) )
							{
								name = newName;
								break;
							}
						}
					}

					textBoxName.Text = name;
				}
				else
				{
					////!!!!проверять валидность имени, т.к. слеши нельзя для компонент. для файлов можно

					//var name = selectedType.GetUserFriendlyNameForInstance();

					////!!!!тут не так для ресурсных типов

					//{
					//	//!!!!как-то получше вроде как надо

					//	char[] invalidCharacters = new char[] { '/', '\\', ':' };
					//	foreach( var c in invalidCharacters )
					//		name = name.Replace( c, '_' );
					//}

					var name = defaultName;

					//!!!!multiselection
					Component parentComponent = null;
					if( creationData.initParentObjects.Count != 0 )
						parentComponent = creationData.initParentObjects[ 0 ] as Component;

					//get unique name
					if( parentComponent != null && parentComponent.GetComponent( name ) != null )
					{
						for( int n = 2; ; n++ )
						{
							string newName = name + " " + n.ToString();
							if( parentComponent.GetComponent( newName ) == null )
							{
								name = newName;
								break;
							}
						}
					}

					textBoxName.Text = name;
				}

				textBoxName.Enabled = true;
			}
			else
				textBoxName.Enabled = false;

			UpdateCreationPath();
		}

		string Translate( string text )
		{
			return EditorLocalization.Translate( "NewObjectWindow", text );
		}

		bool IsReadyToCreate( out string reason )
		{
			reason = "";

			if( selectedType == null )
				return false;

			if( IsFileCreation() )
			{
				if( string.IsNullOrEmpty( textBoxName.Text ) )
				{
					reason = Translate( "The file name is not specified." );
					return false;
				}

				if( textBoxName.Text.IndexOfAny( Path.GetInvalidPathChars() ) != -1 )
				{
					reason = Translate( "Invalid file name." );
					return false;
				}

				try
				{
					if( textBoxName.Text.Contains( '\\' ) || textBoxName.Text.Contains( '/' ) )
					{
						if( Path.GetFileName( textBoxName.Text ).IndexOfAny( Path.GetInvalidFileNameChars() ) != -1 )
						{
							reason = Translate( "Invalid file name." );
							return false;
						}
					}
				}
				catch { }

				var realPath = VirtualPathUtility.GetRealPathByVirtual( textBoxName.Text );
				if( File.Exists( realPath ) )
				{
					reason = string.Format( Translate( "A file with the name \'{0}\' already exists." ), Path.GetFileName( realPath ) );
					return false;
				}
			}

			foreach( var c in tableLayoutPanel1.Controls )
			{
				NewObjectCell cell = c as NewObjectCell;
				if( cell != null )
				{
					if( !cell.ReadyToCreate( out reason ) )
						return false;
				}
			}

			return true;
		}

		void SelectedTypeChanged()
		{
			UpdateName();
			UpdateCells();

			UpdateControls();
		}

		void UpdateCreationPath()
		{
			//if( !string.IsNullOrEmpty( initFileCreationDirectory ) )
			//{
			//	labelCreationPath.Text = "";
			//}
			//else
			//{
			//	//!!!!!!
			//	Component parent = initParentObject as Component;
			//	if( parent != null )
			//	{
			//		var path = parent.GetNamePathToAccessFromRoot();// + "\\" + textBoxName.Text;
			//		labelCreationPath.Text = "Parent: " + path;
			//	}
			//	else
			//	{
			//		labelCreationPath.Text = "";
			//	}
			//}
		}

		void UpdateControls()
		{
			//KryptonSplitContainer childs layout broken. see comments in kryptonsplitcontainer.cs. Also Anchors works strange in .NET Core. 
			panelName.Width = panelName.Parent.Width - DpiHelper.Default.ScaleValue( 8 );
			panelName.Height = DpiHelper.Default.ScaleValue( 25 );

			if( !labelName.Visible )
			{
				textBoxName.Location = new Point( DpiHelper.Default.ScaleValue( 2 ), DpiHelper.Default.ScaleValue( 2 ) );
				textBoxName.Width = panelName.Width - DpiHelper.Default.ScaleValue( 6 );
			}
			else
			{
				textBoxName.Location = new Point( labelName.Location.X + labelName.Width + DpiHelper.Default.ScaleValue( 2 ), DpiHelper.Default.ScaleValue( 2 ) );
				textBoxName.Width = panelName.Width - labelName.Width - DpiHelper.Default.ScaleValue( 9 );
			}

			tableLayoutPanel1.Location = new Point( panelName.Location.X, panelName.Bounds.Bottom + DpiHelper.Default.ScaleValue( 10 ) );
			tableLayoutPanel1.Size = new Size(
				buttonClose.Bounds.Right - tableLayoutPanel1.Location.X,
				buttonClose.Bounds.Top - DpiHelper.Default.ScaleValue( 10 ) - tableLayoutPanel1.Location.Y );

			buttonCreate.Enabled = IsReadyToCreate( out string reason );
			buttonCreateAndClose.Enabled = buttonCreate.Enabled;

			if( DisableUnableToCreateReason )
				labelError.Text = "";
			else
				labelError.Text = reason;
		}

		public Control AddCell( Type cellClass )//, bool getIfAlreadyCreated )
		{
			//if( getIfAlreadyCreated )
			//{
			//	foreach( Control c in layoutPanel.Controls )
			//	{
			//		if( cellClass.IsAssignableFrom( c.GetType() ) )
			//			return c;
			//	}
			//}

			Control cell = (Control)cellClass.GetConstructor( new Type[ 0 ] ).Invoke( new object[ 0 ] );
			cell.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			tableLayoutPanel1.Controls.Add( cell );

			return cell;
		}

		void UpdateCells()
		{
			//!!!!!

			tableLayoutPanel1.Controls.Clear();

			if( SelectedType != null )
			{
				//add cells by EditorNewObjectSettingsAttribute
				{
					//!!!!

					var ar = SelectedType.GetNetType().GetCustomAttributes( typeof( NewObjectSettingsAttribute ), true );
					if( ar.Length != 0 || IsFileCreation() )
					{
						Type settingsClass = null;
						if( ar.Length != 0 )
							settingsClass = ( (NewObjectSettingsAttribute)ar[ 0 ] ).SettingsClass;
						else
							settingsClass = typeof( NewObjectSettings );

						var cell = (NewObjectSettingsCell)AddCell( typeof( NewObjectSettingsCell ) );
						if( !cell.Init( settingsClass, this ) )// IsFileCreation() );
							tableLayoutPanel1.Controls.Remove( cell );
					}

					//var ar = SelectedType.GetNetType().GetCustomAttributes( typeof( EditorNewObjectSettingsAttribute ), true );
					//if( ar.Length != 0 )
					//{
					//	Type settingsClass = ( (EditorNewObjectSettingsAttribute)ar[ 0 ] ).SettingsClass;
					//	var cell = (NewObjectSettingsCell)AddCell( typeof( NewObjectSettingsCell ) );
					//	if( !cell.Init( settingsClass, this ) )// IsFileCreation() );
					//		tableLayoutPanel1.Controls.Remove( cell );
					//}
				}

				//add cells by EditorNewObjectCellAttribute
				{
					var ar = SelectedType.GetNetType().GetCustomAttributes( typeof( NewObjectCellAttribute ), true );
					foreach( NewObjectCellAttribute attr in ar )
						AddCell( attr.CellClass );
				}

				//!!!!
				//change docking
				if( tableLayoutPanel1.Controls.Count == 1 )
				{
					tableLayoutPanel1.Controls[ 0 ].Size = new Size( 30, 15 );
					tableLayoutPanel1.Controls[ 0 ].Dock = DockStyle.Fill;
				}
			}
		}

		//public void Close()
		//{
		//	var p = Parent;
		//	if( p != null )
		//		p.Dispose();
		//}

		private void TextBoxName_TextChanged( object sender, EventArgs e )
		{
			UpdateCreationPath();
		}

		//!!!!!
		private void ButtonCreate_Click( object sender, EventArgs e )
		{
			if( !CreateObject() )
				return;
		}

		private void ButtonCreateAndClose_Click( object sender, EventArgs e )
		{
			if( !CreateObject() )
				return;

			Close();
		}

		private void ButtonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		//[Browsable( false )]
		//public Component ParentComponent
		//{
		//	get { return initData.parentObject as Component; }
		//}

		public string GetFileCreationRealFileName()
		{
			return IsFileCreation() ? VirtualPathUtility.GetRealPathByVirtual( textBoxName.Text ) : "";
		}

		public bool ApplyCreationSettingsToObject( object createdObject, ref bool disableFileCreation )
		{
			var createdComponent = createdObject as Component;
			var fileCreationRealFileName = GetFileCreationRealFileName();// IsFileCreation() ? VirtualPathUtils.GetRealPathByVirtual( textBoxName.Text ) : "";

			if( !IsFileCreation() && createdComponent != null )
			{
				//!!!!!проверки
				createdComponent.Name = textBoxName.Text;
			}

			createdComponent?.NewObjectSetDefaultConfiguration( true );

			foreach( var c in tableLayoutPanel1.Controls )
			{
				NewObjectCell cell = c as NewObjectCell;
				if( cell != null )
				{
					var context = new NewObjectCell.ObjectCreationContext();
					context.newObject = createdObject;
					context.fileCreationRealFileName = fileCreationRealFileName;
					context.disableFileCreation = disableFileCreation;

					if( !cell.ObjectCreation( context ) )
					//if( !cell.ObjectCreation( createdObject, fileCreationRealFileName, ref disableFileWriting ) )
					{
						//!!!!!

						return false;
					}

					disableFileCreation = context.disableFileCreation;
				}
			}

			return true;
		}

		bool CreateObject()
		{
			creationData.ClearCreatedObjects();

			//!!!!в окнах/окне делать активным после создания

			//creationData.selectedType = SelectedType;
			//creationData.replaceSelectedTypeFunction?.Invoke( this );

			//!!!!
			//create objects
			{
				creationData.beforeCreateObjectsFunction?.Invoke( this, SelectedType );

				//default creation behaviour
				if( creationData.createdObjects == null )
				{
					creationData.createdObjects = new List<object>();

					if( creationData.initParentObjects != null )
					{
						foreach( var parentObject in creationData.initParentObjects )
						{
							var parentComponent = parentObject as Component;

							object obj;
							if( parentComponent != null )
							{
								var insertIndex = EditorUtility.GetNewObjectInsertIndex( parentComponent, SelectedType );
								obj = parentComponent.CreateComponent( SelectedType, insertIndex, false );
							}
							else
								obj = SelectedType.InvokeInstance( null );

							creationData.createdObjects.Add( obj );
							creationData.createdObjectsToApplySettings.Add( obj );
							var c = obj as Component;
							if( c != null )
								creationData.createdComponentsOnTopLevel.Add( c );
						}
					}
					else
					{
						var obj = SelectedType.InvokeInstance( null );

						creationData.createdObjects.Add( obj );
						creationData.createdObjectsToApplySettings.Add( obj );
						var c = obj as Component;
						if( c != null )
							creationData.createdComponentsOnTopLevel.Add( c );
					}
				}
			}

			//!!!!
			//no created objects
			if( creationData.createdObjects.Count == 0 )
			{
				//!!!!
				return false;
			}

			string realFileName = "";
			if( IsFileCreation() )
				realFileName = VirtualPathUtility.GetRealPathByVirtual( textBoxName.Text );

			//create folder for file creation
			if( IsFileCreation() )
			{
				var directoryName = Path.GetDirectoryName( realFileName );
				if( !Directory.Exists( directoryName ) )
				{
					try
					{
						Directory.CreateDirectory( directoryName );
					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
						return false;
					}
				}
			}

			//init settings of objects
			bool disableFileCreation = false;
			foreach( var createdObject in creationData.createdObjectsToApplySettings )
			{
				if( !ApplyCreationSettingsToObject( createdObject, ref disableFileCreation ) )
					return false;
			}

			//action before enabled
			creationData.additionActionBeforeEnabled?.Invoke( this );

			//finalization of creation
			foreach( var component in creationData.createdComponentsOnTopLevel )
				component.Enabled = true;

			creationData.additionActionAfterEnabled?.Invoke( this );
			//foreach( var obj in createdObjects )
			//	creationData.additionActionAfterEnabled?.Invoke( this, obj, newObjectsFromAdditionActions );

			//file creation. save to file
			if( IsFileCreation() )
			{
				//!!!!проверки
				//!!!!!!!overwrite

				if( creationData.createdComponentsOnTopLevel.Count == 1 && !disableFileCreation )
				{
					var createdComponent = creationData.createdComponentsOnTopLevel[ 0 ];
					if( !ComponentUtility.SaveComponentToFile( createdComponent, realFileName, null, out string error ) )
					{
						if( !string.IsNullOrEmpty( error ) )
						{
							//!!!!
							Log.Warning( error );
							return false;
						}
					}
				}

				//Dispose created objects for file creation mode
				foreach( var obj in creationData.createdObjects )
				{
					var d = obj as IDisposable;
					if( d != null )
						d.Dispose();
				}
			}

			//update document
			if( !IsFileCreation() )
			{
				//update document
				//!!!!!
				var document = creationData.initDocumentWindow.Document;
				if( document != null )
				{
					//!!!!только компоненты?

					var action = new UndoActionComponentCreateDelete( document, creationData.createdComponentsOnTopLevel, true );

					//List<Component> created = new List<Component>();
					//created.AddRange( createdComponents );
					//foreach( var obj in newObjectsFromAdditionActions )
					//{
					//	var component = obj as Component;
					//	if( component != null )
					//		created.Add( component );
					//}
					//var action = new UndoActionComponentCreateDelete( created, true );
					document.UndoSystem.CommitAction( action );

					document.Modified = true;
				}
				else
				{
					//!!!!надо ли?
					Log.Warning( "impl" );
				}
			}

			//select and open
			if( IsFileCreation() )
			{
				//!!!!не обязательно основное окно

				EditorAPI.GetRestartApplication( out var needRestart, out _ );
				if( needRestart )
				{
					EditorSettingsSerialization.OpenFileAtStartup = realFileName;
				}
				else
				{
					//select new file in Resources window
					EditorAPI.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );

					//open file
					EditorAPI.OpenFileAsDocument( realFileName, true, true );
				}
			}
			else
			{
				//select created components
				if( creationData.createdFromContentBrowser != null )
				{
					var creator = creationData.createdFromContentBrowser;
					if( creator.IsHandleCreated && !creator.IsDisposed )
						ContentBrowserUtility.SelectComponentItems( creator, creationData.createdComponentsOnTopLevel.ToArray() );
				}
				else
					EditorAPI.SelectComponentsInMainObjectsWindow( creationData.initDocumentWindow, creationData.createdComponentsOnTopLevel.ToArray() );

				//open editor
				if( creationData.createdComponentsOnTopLevel.Count == 1 )
				{
					var component = creationData.createdComponentsOnTopLevel[ 0 ];

					if( !component.EditorReadOnlyInHierarchy )
					{
						//!!!!пока так
						if( component is FlowGraph || component is CSharpScript )
							EditorAPI.OpenDocumentWindowForObject( creationData.initDocumentWindow.Document, component );
					}
					//if( EditorAPI.IsDocumentObjectSupport( component ) && !component.EditorReadOnlyInHierarchy )
					//	EditorAPI.OpenDocumentWindowForObject( creationData.initDocumentWindow.Document, component );
				}
			}

			//finish creation
			creationData.ClearCreatedObjects();

			return true;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !WinFormsUtility.IsControlVisibleInHierarchy( this ) )
				return;

			//need periodically check for case when component inside _File item was loaded
			UpdateSelectedType();

			UpdateControls();

			//!!!!лучше совсем окно не показывать
			//auto create and close
			if( firstTick && creationData.initSupportAutoCreateAndClose )
			{
				if( tableLayoutPanel1.Controls.Count == 0 )
				{
					if( IsReadyToCreate( out _ ) )
						ButtonCreateAndClose_Click( null, null );
				}
			}

			firstTick = false;
		}

		ContentBrowser.Item GetItemByType( Metadata.TypeInfo type )
		{
			//!!!!slowly

			//!!!!учитывает только _Type

			foreach( var item in contentBrowser1.Items )
			{
				var typeItem = item as ContentBrowserItem_Type;
				if( typeItem != null )
				{
					if( typeItem.type == type )
						return item;
				}
			}
			return null;
		}

		void SelectInitLockType()//void SelectInitDemandedType()
		{
			var item = GetItemByType( creationData.initLockType );

			//!!!!если тип в ресурсе, то сложнее поиск


			//if( item == null && !string.IsNullOrEmpty( reference ) )
			//{
			//	//create items by the path

			//	//get list of references to expand
			//	List<string> toCheck = new List<string>();
			//	{
			//		string current = reference;

			//		again:;

			//		int splitIndex = current.LastIndexOfAny( new char[] { '\\', '/', '|' } );
			//		if( splitIndex != -1 )
			//		{
			//			var reference2 = current.Substring( 0, splitIndex );
			//			if( !string.IsNullOrEmpty( reference2 ) )
			//			{
			//				current = reference2;
			//				toCheck.Add( current );
			//				goto again;
			//			}
			//		}
			//	}

			//	//process the list (from back)
			//	foreach( var reference2 in toCheck.GetReverse() )
			//	{
			//		var item2 = GetItemByReference( reference2 );
			//		if( item2 != null )
			//		{
			//			contentBrowser1.SelectItems( new ContentBrowser.Item[] { item2 }, true );

			//			//не надо, т.к. expand делается
			//			//contentBrowser1.AddChildNodes( item2 );
			//		}
			//	}

			//	//try get again
			//	item = GetItemByReference( reference );
			//}

			if( item != null )
				contentBrowser1.SelectItems( new ContentBrowser.Item[] { item }, true );
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( contentBrowser1.DocumentWindow, contentBrowser1.GetSelectedContainedObjects() );
		}

		bool Config_Load()
		{
			var windowBlock = EngineConfig.TextBlock.FindChild( "NewObjectWindow" );
			if( windowBlock != null )
			{
				var browserBlock = windowBlock.FindChild( "ContentBrowser" );
				if( browserBlock != null )
				{
					contentBrowser1.Options.Load( browserBlock );
					return true;
				}
			}

			return false;
		}

		static void Config_SaveEvent()
		{
			if( sharedOptions != null )
			{
				var configBlock = EngineConfig.TextBlock;

				var old = configBlock.FindChild( "NewObjectWindow" );
				if( old != null )
					configBlock.DeleteChild( old );

				var windowBlock = configBlock.AddChild( "NewObjectWindow" );
				var browserBlock = windowBlock.AddChild( "ContentBrowser" );
				sharedOptions.Save( browserBlock );
				//browser.SaveSettings( browserBlock );
			}
		}

		[Browsable( false )]
		public bool DisableUnableToCreateReason { get; set; }

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

	}
}

#endif