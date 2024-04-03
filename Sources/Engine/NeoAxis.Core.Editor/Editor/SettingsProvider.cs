#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace NeoAxis.Editor
{
	////!!!!
	//public interface ISettingsProvider
	//{
	//	void GetSettings( SettingsProvider provider, int objectIndex );

	//	//!!!!
	//	//!!!!!или соединить методы эти и еще какие-то или что-то
	//	//!!!!name. а может в эвента провайдера такое делать. т.е. GetSettings() только было бы
	//	//!!!!!!!event: EnumerateProperty
	//	void GetSettingsProperty( SettingsProvider provider, Metadata.Property property, ref bool skip );
	//}

	public class SettingsProvider : ISettingsProvider
	{
		//!!!!public

		//!!!!!возможность заоверрайдить получение мемберов

		//!!!!!редактировать меш полноценно как ресурс при этом он будет внутри сцены или другого объекта

		//!!!!

		DocumentWindow documentWindow;
		object[] selectedObjects;
		TableLayoutPanel layoutPanel;
		object anyData;

		////////////////

		public delegate void CreateProviderDelegate( DocumentWindow documentWindow, object[] selectedObjects, TableLayoutPanel layoutPanel, object anyData, ref SettingsProvider provider );
		public static event CreateProviderDelegate CreateProvider;

		public delegate void UpdateDelegate( SettingsProvider provider );
		public event UpdateDelegate UpdateBegin;
		public static event UpdateDelegate AllProviders_UpdateBegin;
		public event UpdateDelegate UpdateEnd;
		public static event UpdateDelegate AllProviders_UpdateEnd;

		//

		public SettingsProvider( DocumentWindow documentWindow, object[] selectedObjects, TableLayoutPanel layoutPanel, object anyData )
		{
			this.selectedObjects = selectedObjects;
			this.documentWindow = documentWindow;
			this.layoutPanel = layoutPanel;
			this.anyData = anyData;

			this.layoutPanel.Tag = this;
		}

		public static SettingsProvider Create( DocumentWindow documentWindow, object[] selectedObjects, TableLayoutPanel layoutPanel, object anyData, bool update )
		{
			//create special
			SettingsProvider provider = null;
			CreateProvider?.Invoke( documentWindow, selectedObjects, layoutPanel, anyData, ref provider );

			//create default
			if( provider == null )
				provider = new SettingsProvider( documentWindow, selectedObjects, layoutPanel, anyData );

			if( update )
			{
				try
				{
					//!!!!
					layoutPanel.SuspendLayout();

					provider.PerformUpdate( false );
				}
				finally
				{
					layoutPanel.ResumeLayout();
				}
			}

			return provider;
		}

		public IDocumentWindow DocumentWindow
		{
			get { return documentWindow; }
		}

		public DocumentWindow DocumentWindow2
		{
			get { return documentWindow; }
		}

		public object[] SelectedObjects
		{
			get { return selectedObjects; }
		}

		public TableLayoutPanel LayoutPanel
		{
			get { return layoutPanel; }
		}

		public object AnyData
		{
			get { return anyData; }
		}

		public void Clear()
		{
			if( layoutPanel != null )
			{
				layoutPanel.RowStyles.Clear();
				layoutPanel.Controls.Clear();
			}

			//!!!!!что-то еще делать может
		}

		static Type GetSettingsCellType( SettingsCellAttribute attrib )
		{
			if( !string.IsNullOrEmpty( attrib.SettingsCellClassName ) )
			{
				var type = EditorUtility.GetTypeByName( attrib.SettingsCellClassName );
				if( type == null )
					Log.Warning( $"SettingsProvider: GetSettingsCellType: Class with name \"{attrib.SettingsCellClassName}\" is not found." );
				return type;
			}
			else
				return attrib.SettingsCellClass;
		}

		protected virtual void OnUpdate()
		{
			for( int n = 0; n < selectedObjects.Length; n++ )
			{
				var obj = selectedObjects[ n ];

				//EditorSettingsCellAttribute
				foreach( var attr in obj.GetType().GetCustomAttributes<SettingsCellAttribute>( true ).Reverse() )
				{
					if( selectedObjects.Length == 1 || attr.MultiselectionSupport )
					{
						AddCell( GetSettingsCellType( attr ), true );
						//AddCell( attr.SettingsCellClass, true );
					}
				}

				//!!!!

				////SettingsCell
				//var component = obj as Component;
				//if( component != null )
				//{
				//	var type = component.BaseType;
				//	while( type as Metadata.ComponentTypeInfo != null )
				//	{
				//		var componentType = (Metadata.ComponentTypeInfo)type;
				//		if( componentType.BasedOnObject != null )
				//		{
				//			foreach( var cellComponent in componentType.BasedOnObject.GetComponents<SettingsCell>( onlyEnabledInHierarchy: true ) )
				//			{
				//				if( selectedObjects.Length == 1 /*|| attr.MultiselectionSupport*/ )
				//				{
				//					var cell = new SettingsCell.SettingsCellImpl( cellComponent );
				//					AddCell( cell );
				//				}
				//			}
				//		}

				//		type = type.BaseType;
				//	}

				//	foreach( var cellComponent in component.GetComponents<SettingsCell>( onlyEnabledInHierarchy: true ) )
				//	{
				//		if( selectedObjects.Length == 1 /*|| attr.MultiselectionSupport*/ )
				//		{
				//			var cell = new SettingsCell.SettingsCellImpl( cellComponent );
				//			//var cell = new SettingsCell.SettingsCellImpl( cellComponent );
				//			AddCell( cell );
				//		}
				//	}
				//}
			}

			//for( int n = 0; n < selectedObjects.Length; n++ )
			//{
			//	var obj = selectedObjects[ n ];

			//	ISettingsProvider provider = obj as ISettingsProvider;
			//	if( provider != null )
			//		provider.GetSettings( this, n );
			//}

			//AddCell( typeof( SettingsCell_Properties ), true );

			//!!!!тут ли. так ли
			//init properties
			foreach( Control control in layoutPanel.Controls )
			{
				SettingsCell_Properties properties = control as SettingsCell_Properties;
				if( properties != null )
					properties.UpdateData();
			}
		}

		//!!!!может есть опции обновления какие-то
		public void PerformUpdate( bool clear )
		{
			if( clear )
				Clear();

			UpdateBegin?.Invoke( this );
			AllProviders_UpdateBegin?.Invoke( this );

			OnUpdate();

			//!!!!внутри suspend update видать тут что-то

			//sort by priority
			{
				List<Tuple<Control, float>> list = new List<Tuple<Control, float>>();
				foreach( Control control in layoutPanel.Controls )
				{
					float priority = 0;
					SettingsCell cell = control as SettingsCell;
					if( cell != null )
						priority = cell.CellsSortingPriority;
					list.Add( new Tuple<Control, float>( control, priority ) );
				}

				CollectionUtility.MergeSort( list, delegate ( Tuple<Control, float> c1, Tuple<Control, float> c2 )
				{
					if( c1.Item2 < c2.Item2 )
						return -1;
					if( c1.Item2 > c2.Item2 )
						return 1;
					return 0;
				} );

				for( int n = 0; n < list.Count; n++ )
				{
					var t = list[ n ];
					layoutPanel.Controls.SetChildIndex( t.Item1, n );
				}
			}

			////change docking
			//if( layoutPanel.Controls.Count == 1 && layoutPanel.Controls[ 0 ] is SettingsCell_Properties )
			//{
			//	//only Properties. no another controls
			//	layoutPanel.Controls[0].Size = new System.Drawing.Size(30, 15);
			//	layoutPanel.Controls[0].Dock = DockStyle.Fill;
			//}
			//else
			//{
			//	//!!!!!temp

			//	foreach( Control c in layoutPanel.Controls )
			//	{
			//		var p = c as SettingsCell_Properties;
			//		if( p != null )
			//		{
			//			//p.Size = new System.Drawing.Size( 30, 15 );
			//			//p.Dock = DockStyle.Fill;
			//			break;
			//		}
			//	}

			//	//!!!!!
			//	//layoutPanel.AutoScroll = true;
			//	//layoutPanel.HorizontalScroll.Enabled = false;
			//}

			UpdateEnd?.Invoke( this );
			AllProviders_UpdateEnd?.Invoke( this );
		}

		public Control AddCell( Type cellClass, bool useIfAlreadyCreated )
		{
			if( useIfAlreadyCreated )
			{
				foreach( Control c in layoutPanel.Controls )
				{
					if( cellClass.IsAssignableFrom( c.GetType() ) )
						return c;

					if( typeof( SettingsCellProcedureUI ).IsAssignableFrom( cellClass ) )
					{
						var container = c as SettingsCellProcedureUI_Container;
						if( container != null && cellClass.IsAssignableFrom( container.procedureUI.GetType() ) )
							return c;
					}
				}
			}

			SettingsCell cell;
			if( typeof( SettingsCellProcedureUI ).IsAssignableFrom( cellClass ) )
			{
				var procedureUI = (SettingsCellProcedureUI)Activator.CreateInstance( cellClass );
				var container = new SettingsCellProcedureUI_Container();
				container.procedureUI = procedureUI;
				procedureUI.container = container;
				cell = container;
			}
			else
				cell = (SettingsCell)cellClass.GetConstructor( new Type[ 0 ] ).Invoke( new object[ 0 ] );

			AddCell( cell );
			return cell;
		}

		public Control AddCell( SettingsCell cell )
		{
			cell.PerformInit();

			if( cell.SizeType != FormSizeType.AutoSize )
				cell.SetBounds( 0, 0, layoutPanel.Width, layoutPanel.Height );
			cell.Dock = DockStyle.Fill;

			if( cell.SizeType == FormSizeType.AutoSize )
				layoutPanel.RowStyles.Add( new RowStyle() { SizeType = SizeType.AutoSize } );
			else
				layoutPanel.RowStyles.Add( new RowStyle() { SizeType = SizeType.Percent, Height = 100 } );

			layoutPanel.RowCount = layoutPanel.RowStyles.Count;
			layoutPanel.Controls.Add( cell );

			return cell;
		}

	}
}

#endif