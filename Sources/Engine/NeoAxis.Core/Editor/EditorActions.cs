#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Representation of an editor action.
	/// </summary>
	public class EditorAction
	{
		public string Name;
		public CommonTypeEnum CommonType = CommonTypeEnum.Document;
		public string Description;

		//!!!!public string text;
		//Button
		public Image ImageSmall;
		public Image ImageBig;
		public Image ImageSmallDark;
		public Image ImageBigDark;

		Image imageSmallResult;
		Image imageBigResult;

		public Keys[] ShortcutKeys;

		//QAT
		public bool QatSupport;
		public bool QatAddByDefault;

		//Ribbon
		public (string, string) RibbonText;

		//Action type
		public enum ActionTypeEnum
		{
			Button,
			DropDown,
			Slider,
			//ComboBox,
			ListBox,
		}
		public ActionTypeEnum ActionType = ActionTypeEnum.Button;
		public KryptonContextMenu DropDownContextMenu;

		public SliderSettings Slider = new SliderSettings();
		//public ComboBoxSettings ComboBox = new ComboBoxSettings();
		public ListBoxSettings ListBox = new ListBoxSettings();

		public EditorContextMenuWinForms.MenuTypeEnum ContextMenuSupport = EditorContextMenuWinForms.MenuTypeEnum.None;
		//public bool ContextMenuSupport;

		public string ContextMenuText;

		public object UserData;

		public bool CompletelyDisabled;

		/////////////////////////////////////////

		public enum CommonTypeEnum
		{
			General,
			/// <summary>
			/// When this option is enabled, the action can be enabled even when the document window is not selected (another dock window is selected). The last selected document window will be used.
			/// </summary>
			Document,
		}

		/////////////////////////////////////////

		public enum HolderEnum
		{
			RibbonQAT,
			ContextMenu,//!!!!потом может еще быть какое именно контекстное меню
			ShortcutKey,
		}

		/////////////////////////////////////////

		public class SliderSettings
		{
			//public int Length = 100;
			//public TickStyle TickStyle = TickStyle.None;
			//public PaletteTrackBarSize TrackBarSize = PaletteTrackBarSize.Medium;
			//public int TickFrequency = 1;
			//public bool VolumeControl;
			//public Orientation Orientation = Orientation.Horizontal;
			public double Minimum;
			public double Maximum = 1;
			public double ExponentialPower;// = 1;
										   //public int SmallChange = 1;
										   //public int LargeChange = 5;

			public double Value;

			//public int Length = 100;
			//public TickStyle TickStyle = TickStyle.None;
			//public PaletteTrackBarSize TrackBarSize = PaletteTrackBarSize.Medium;
			//public int TickFrequency = 1;
			//public bool VolumeControl;
			//public Orientation Orientation = Orientation.Horizontal;
			//public int Minimum;
			//public int Maximum = 10;
			//public int SmallChange = 1;
			//public int LargeChange = 5;
			////!!!!так?
			//public int Value;
		}

		/////////////////////////////////////////

		//public class ComboBoxSettings
		//{
		//	public int Length = 150;
		//	public List<string> Items = new List<string>();
		//	public int SelectedIndex;
		//}

		/////////////////////////////////////////

		public class ListBoxSettings
		{
			public int Length = 150;//137;//172;

			public enum ModeEnum
			{
				List,
				Tiles,
			}
			public ModeEnum Mode = ModeEnum.List;

			public List<(string, Image)> Items = new List<(string, Image)>();
			public int SelectedIndex;

			public int? SelectIndex;

			public bool LastSelectedIndexChangedByUser;
		}

		/////////////////////////////////////////

		public class GetStateContext
		{
			HolderEnum holder;
			ObjectsInFocus objectsInFocus;
			EditorAction action;

			//

			internal GetStateContext( HolderEnum holder, ObjectsInFocus objectsInFocus, EditorAction action )
			{
				this.holder = holder;
				this.objectsInFocus = objectsInFocus;
				this.action = action;
			}

			public HolderEnum Holder
			{
				get { return holder; }
			}

			public ObjectsInFocus ObjectsInFocus
			{
				get { return objectsInFocus; }
			}

			public EditorAction Action
			{
				get { return action; }
			}

			public bool Enabled { get; set; }
			public bool Checked { get; set; }
		}

		/////////////////////////////////////////

		public class ClickContext
		{
			HolderEnum holder;
			ObjectsInFocus objectsInFocus;
			EditorAction action;

			//

			internal ClickContext( HolderEnum holder, ObjectsInFocus objectsInFocus, EditorAction action )
			{
				this.holder = holder;
				this.objectsInFocus = objectsInFocus;
				this.action = action;
			}

			public HolderEnum Holder
			{
				get { return holder; }
			}

			public ObjectsInFocus ObjectsInFocus
			{
				get { return objectsInFocus; }
			}

			public EditorAction Action
			{
				get { return action; }
			}
		}

		/////////////////////////////////////////

		public delegate void GetStateDelegate( GetStateContext context );
		public event GetStateDelegate GetState;
		public void PerformGetState( GetStateContext context )
		{
			GetState?.Invoke( context );
		}

		//!!!!видать будут еще другие действия
		public delegate void ClickDelegate( ClickContext context );
		public event ClickDelegate Click;
		public void PerformClick( ClickContext context )
		{
			Click?.Invoke( context );
		}

		public event ClickDelegate Click2;
		public void PerformClick2( ClickContext context )
		{
			Click2?.Invoke( context );
		}

		public string ToolTip
		{
			get
			{
				string result = "";

				if( !string.IsNullOrEmpty( Description ) )
					result = EditorLocalization.Translate( "EditorAction.Description", Description );
				else
				{
					result = Name;
					//result = EditorLocalization.Translate( "EditorAction.Name", Name );
				}

				var keysString = EditorActions.ConvertShortcutKeysToString( ShortcutKeys );
				if( keysString != "" )
					result += " (" + keysString + ")";

				return result;
			}
		}

		public string GetContextMenuText()
		{
			if( !string.IsNullOrEmpty( ContextMenuText ) )
				return EditorLocalization.Translate( "EditorAction.Name", ContextMenuText );
			return Name;
		}

		internal static Bitmap ResizeImage( Image image, int width, int height )
		{
			Bitmap result = new Bitmap( width, height );
			using( Graphics g = Graphics.FromImage( result ) )
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
				g.DrawImage( image, 0, 0, width, height );
			}
			return result;
		}

		void PrepareImagesResult()
		{
			if( imageSmallResult == null && imageBigResult == null )
			{
				var big = ( EditorAPI.DarkTheme && ImageBigDark != null ) ? ImageBigDark : ImageBig;
				var small = ( EditorAPI.DarkTheme && ImageSmallDark != null ) ? ImageSmallDark : ImageSmall;

				if( small == null && big != null )
					small = ResizeImage( big, 16, 16 );

				imageBigResult = big;
				imageSmallResult = small;
			}
		}

		public Image GetImageBig()
		{
			PrepareImagesResult();
			return imageBigResult;
		}

		public Image GetImageSmall()
		{
			PrepareImagesResult();
			return imageSmallResult;
			//if( ImageSmall == null && ImageBig != null )
			//	ImageSmall = ResizeImage( ImageBig, 16, 16 );
			//return ImageSmall;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorAction_NewResource : EditorAction
	{
		Metadata.TypeInfo type;

		//

		public EditorAction_NewResource( Metadata.TypeInfo type )
		{
			this.type = type;

			ImageSmall = Properties.Resources.New_16;
			ImageBig = Properties.Resources.New_32;
			QatSupport = true;

			GetState += delegate ( EditorAction.GetStateContext context )
			{
				context.Enabled = true;
			};
			Click += delegate ( EditorAction.ClickContext context )
			{
				var initData = new NewObjectWindow.CreationDataClass();

				var window = EditorAPI.FindWindow<ResourcesWindow>();
				string directory = window.ContentBrowser1.GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
				if( !string.IsNullOrEmpty( directory ) )
					initData.initFileCreationDirectory = VirtualPathUtility.GetVirtualPathByReal( directory, true );

				initData.initLockType = type;

				EditorAPI.OpenNewObjectWindow( initData );
			};
		}

		public EditorAction_NewResource( Type type )
			: this( MetadataManager.GetTypeOfNetType( type ) )
		{
		}

		public Metadata.TypeInfo Type
		{
			get { return type; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class EditorAction_NewComponent : EditorAction
	//{
	//	Metadata.TypeInfo type;
	//	string typeName = "";

	//	//

	//	public EditorAction_NewComponent( Metadata.TypeInfo type )
	//	{
	//		this.type = type;

	//		ImageSmall = Properties.Resources.New_16;
	//		ImageBig = Properties.Resources.New_32;
	//		QatSupport = true;

	//		GetState += delegate ( EditorAction.GetStateContext context )
	//		{
	//			context.Enabled = true;
	//		};
	//		Click += delegate ( EditorAction.ClickContext context )
	//		{
	//			//!!!!

	//			//var initData = new NewObjectWindow.CreationDataClass();

	//			//var window = EditorAPI.FindWindow<ResourcesWindow>();
	//			//string directory = window.ContentBrowser1.GetDirectoryPathOfSelectedFileOrParentDirectoryItem();
	//			//if( !string.IsNullOrEmpty( directory ) )
	//			//	initData.initFileCreationDirectory = VirtualPathUtility.GetVirtualPathByReal( directory );

	//			//initData.initLockType = type;

	//			//EditorAPI.OpenNewObjectWindow( initData );
	//		};
	//	}

	//	public EditorAction_NewComponent( Type type )
	//		: this( MetadataManager.GetTypeOfNetType( type ) )
	//	{
	//	}

	//	public EditorAction_NewComponent( string typeName )
	//		: this( (Metadata.TypeInfo)null )
	//	{
	//		this.typeName = typeName;
	//	}

	//	public Metadata.TypeInfo Type
	//	{
	//		get { return type; }
	//	}

	//	public string TypeName
	//	{
	//		get { return typeName; }
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorActionDockWindow : EditorAction
	{
		public Type windowClass;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class EditorActions
	{
		static Dictionary<string, EditorAction> actions = new Dictionary<string, EditorAction>();

		//

		static EditorActions()
		{
			if( EditorForm.Instance != null )
				EditorStandardActions.Register();
		}

		public static ICollection<EditorAction> Actions
		{
			get { return actions.Values; }
		}

		public static EditorAction GetByName( string name )
		{
			actions.TryGetValue( name, out EditorAction action );
			return action;
		}

		//public delegate void RegisterEventDelegate( EditorAction action, ref bool remove );
		//public static event RegisterEventDelegate RegisterEvent;

		public static void Register( EditorAction action )
		{
			//init ribbonText
			if( action.RibbonText.Item1 == null )
			{
				var name = action.Name;

				if( name.Contains( " " ) )
				{
					int index = name.IndexOf( ' ' );
					action.RibbonText = (name.Substring( 0, index ), name.Substring( index + 1 ));
				}
				else
					action.RibbonText = (name, "");
			}

			//bool allowRegister = true;
			//RegisterEvent?.Invoke( action, ref allowRegister );
			//if( !allowRegister )
			//	return;

			actions[ action.Name ] = action;
		}

		public static void Unregister( string name )
		{
			actions.Remove( name );
		}

		public static void CompleteDisable( string name )
		{
			var action = GetByName( name );
			if( action != null )
				action.CompletelyDisabled = true;
		}

		public static void RegisterDockWindowAction( string name, (string, string) ribbonText, Type windowClass )
		{
			var a = new EditorActionDockWindow();
			a.Name = name;
			a.Description = "Shows or hides the " + name + ".";
			a.ImageSmall = Properties.Resources.Window_16;
			a.ImageBig = Properties.Resources.Window_32;
			a.QatSupport = true;
			a.RibbonText = ribbonText;
			a.windowClass = windowClass;
			a.GetState += delegate ( EditorAction.GetStateContext context )
			{
				var action2 = (EditorActionDockWindow)context.Action;
				var window = EditorForm.Instance.WorkspaceController.FindWindow( action2.windowClass );
				if( window != null )
				{
					context.Enabled = true;
					var dockingManager = EditorForm.Instance.WorkspaceController.DockingManager;
					context.Checked = window != null && window.Visible && dockingManager.ContainsPage( window.KryptonPage );
				}
			};
			a.Click += delegate ( EditorAction.ClickContext context )
			{
				var action2 = (EditorActionDockWindow)context.Action;
				var window = EditorForm.Instance.WorkspaceController.FindWindow( action2.windowClass );
				if( window != null )
				{
					EditorForm.Instance.WorkspaceController.SetDockWindowVisibility( window, !window.Visible );

					var dockingManager = EditorForm.Instance.WorkspaceController.DockingManager;
					if( !dockingManager.ContainsPage( window.KryptonPage ) )
						EditorForm.Instance.WorkspaceController.AddDockWindow( window, true, false );
				}
			};
			Register( a );
		}

		internal static string ConvertShortcutKeysToString( Keys[] shortcutKeys, bool onlyFirst = false )
		{
			if( shortcutKeys != null && shortcutKeys.Length != 0 )
			{
				string result = "";

				for( int n = 0; n < shortcutKeys.Length; n++ )
				{
					if( onlyFirst && n != 0 )
						break;

					var keys = shortcutKeys[ n ];
					if( n != 0 )
						result += "; ";

					var str = keys.ToString();

					if( str.Contains( ", Control" ) )
						str = "Ctrl+" + str.Replace( ", Control", "" );
					if( str.Contains( ", Shift" ) )
						str = "Shift+" + str.Replace( ", Shift", "" );
					if( str.Contains( ", Alt" ) )
						str = "Alt+" + str.Replace( ", Alt", "" );
					str = str.Replace( ", ", "+" );

					result += str;
				}

				return result;
			}
			else
				return "";
		}

		public static string GetFirstShortcutKeyString( string actionName )
		{
			var actionItem = ProjectSettings.Get.Shortcuts.ShortcutSettings.GetActionItem( actionName );
			if( actionItem != null )
			{
				var shortcuts = actionItem.ToArray();
				if( shortcuts != null )
					return ConvertShortcutKeysToString( shortcuts, true );
			}
			return null;

			//var action = GetByName( actionName );
			//if( action != null )
			//	return ConvertShortcutKeysToString( action.ShortcutKeys, true );
			//return "";
		}
	}
}

#endif