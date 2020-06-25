// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Workspace;
using System.IO;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a window for selection type.
	/// </summary>
	public partial class SelectTypeWindow : DockWindow
	{
		public CreationDataClass creationData;

		Metadata.TypeInfo selectedType;
		bool selectedTypeCanSelect;

		/////////////////////////////////////

		public class CreationDataClass
		{
			public DocumentWindow initDocumentWindow;
			public Metadata.TypeInfo initDemandedType;
			public bool initCanSelectNull;
			public bool initCanSelectAbstractClass;

			public delegate void WasSelectedDelegate( SelectTypeWindow window, Metadata.TypeInfo selectedType, ref bool cancel );
			public WasSelectedDelegate WasSelected;
		}

		/////////////////////////////////////

		public SelectTypeWindow()
		{
			InitializeComponent();

			WindowTitle = EditorLocalization.Translate( "SelectTypeWindow", WindowTitle );
			EditorLocalization.TranslateForm( "SelectTypeWindow", this );
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			UpdateContentBrowser();
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

			//!!!!если это не .NET тип, то всё ветку Types не нужно.

			data.allowNull = creationData.initCanSelectNull;
			data.selectTypeWindow = true;
			data.selectTypeDemandedType = creationData.initDemandedType;
			data.selectTypeWindowCanSelectAbstractClass = creationData.initCanSelectAbstractClass;

			contentBrowser1.Init( creationData.initDocumentWindow, null, data );
			contentBrowser1.UpdateData();
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

		[Browsable( false )]
		public bool SelectedTypeCanSelect
		{
			get { return selectedTypeCanSelect; }
		}

		void UpdateSelectedType()
		{
			Metadata.TypeInfo newType = null;
			bool newTypeCanSelect = false;

			ContentBrowser.Item item = null;
			if( contentBrowser1.SelectedItems.Length != 0 )
				item = contentBrowser1.SelectedItems[ 0 ];

			if( item != null )
			{
				item.CalculateReferenceValue( null, MetadataManager.GetTypeOfNetType( typeof( Metadata.TypeInfo ) ),
					out string referenceValue, out bool canSet );

				if( canSet )
				{
					if( !string.IsNullOrEmpty( referenceValue ) )
					{
						newType = (Metadata.TypeInfo)MetadataManager.GetValueByReference(
							contentBrowser1.SetReferenceModeData.demandedType.GetNetType(), null, referenceValue );
						newTypeCanSelect = true;
					}
					else
						newTypeCanSelect = true;
				}

				//check can be selected
				if( newType != null && !creationData.initDemandedType.IsAssignableFrom( newType ) )
				{
					newType = null;
					newTypeCanSelect = false;
				}
			}

			//change
			if( selectedType != newType || selectedTypeCanSelect != newTypeCanSelect )
			{
				selectedType = newType;
				selectedTypeCanSelect = newTypeCanSelect;
				SelectedTypeChanged();
			}
		}

		private void contentBrowser1_ItemAfterSelect( ContentBrowser sender, IList<ContentBrowser.Item> items, bool selectedByUser, ref bool handled )
		{
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			UpdateSelectedType();
		}

		private void contentBrowser1_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( EditorUtility.IsDesignerHosted( this ) )
				return;

			if( !IsReadyToSelect() )
				return;
			ButtonSelect_Click( null, null );
		}

		bool IsReadyToSelect()//out string reason )
		{
			//reason = "";

			if( !creationData.initCanSelectNull && selectedType == null )
				return false;

			return selectedTypeCanSelect;
		}

		void SelectedTypeChanged()
		{
			UpdateControls();
		}

		void UpdateControls()
		{
			buttonSelect.Enabled = IsReadyToSelect();// out string reason );
		}

		private void ButtonSelect_Click( object sender, EventArgs e )
		{
			if( !DoSelect() )
				return;

			Close();
		}

		private void ButtonCancel_Click( object sender, EventArgs e )
		{
			Close();
		}

		bool DoSelect()
		{
			bool cancel = false;
			creationData.WasSelected?.Invoke( this, selectedType, ref cancel );
			if( cancel )
				return false;
			return true;
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;
			if( !EditorUtility.IsControlVisibleInHierarchy( this ) )
				return;

			//need periodically check for case when component inside _File item was loaded
			UpdateSelectedType();

			UpdateControls();
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

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( contentBrowser1.DocumentWindow, contentBrowser1.GetSelectedContainedObjects() );
		}
	}
}
