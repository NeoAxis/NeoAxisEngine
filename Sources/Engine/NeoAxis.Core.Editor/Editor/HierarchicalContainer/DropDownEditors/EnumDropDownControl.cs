#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class EnumDropDownControl : HCDropDownControl
	{
		HCItemEnumDropDown itemProperty;
		HCItemProperty propertyItemForUndoSupport;
		bool valueWasChanged;
		object currentValue;
		bool isFlag;
		Type propertyType;
		Type enumDataType;
		int dropdownHeight;

		///////////////////////////////////////////////

		class EnumInfo
		{
			public bool SetByCode;
			public object Value;

			public EnumInfo( object value )
			{
				Value = value;
			}

			public string DisplayName
			{
				get { return EnumUtility.GetValueDisplayName( Value ); }
			}

			public string Description
			{
				get { return EnumUtility.GetValueDescription( Value ); }
			}
		}

		///////////////////////////////////////////////

		public int DropDownHeight
		{
			get { return dropdownHeight; }
			set { dropdownHeight = value; }
		}

		//not used
		public EnumDropDownControl()
		{
			InitializeComponent();
		}

		public EnumDropDownControl( HCItemEnumDropDown itemProperty )
		{
			dropdownHeight = (int)( (float)200 * EditorAPI2.DPIScale );

			InitializeComponent();

			var mode = (EngineListView.DefaultListMode)listViewEnum.Mode;
			mode.DisplayImages = false;

			this.itemProperty = itemProperty;

			propertyType = ReferenceUtility.GetUnreferencedType( itemProperty.Property.Type.GetNetType() );
			if( propertyType.IsEnum )
			{
				enumDataType = Enum.GetUnderlyingType( propertyType );
				isFlag = propertyType.IsDefined( typeof( FlagsAttribute ), false );
			}

			currentValue = ReferenceUtility.GetUnreferencedValue( itemProperty.GetValues()[ 0 ] );
			propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
			propertyItemForUndoSupport.SaveValuesToRestore();

			var items = new List<EngineListView.Item>();

			listViewEnum.CheckBoxes = isFlag;

			foreach( var val in Enum.GetValues( propertyType ) )
			{
				var info = new EnumInfo( val );

				var displayName = info.DisplayName;
				var description = info.Description;
				itemProperty.Owner.PerformOverridePropertyEnumItem( itemProperty, ref displayName, ref description );

				var item = new EngineListView.Item( listViewEnum );
				item.Text = displayName;
				item.Tag = info;
				if( !string.IsNullOrEmpty( description ) )
				{
					item.Description = description;
					item.ShowTooltip = true;
				}
				items.Add( item );
			}

			listViewEnum.SetItems( items );

			UpdateCheckState();

			int itemCount = listViewEnum.Items.Count;
			var height = itemCount * ( Font.Height + DpiHelper.Default.ScaleValue( 4 ) ) + 3;
			var width = ( (IHCProperty)itemProperty.CreatedControl ).EditorControl.Width;
			Size = new Size( (int)Math.Max( width, EditorAPI2.DPIScale * 150 ), Math.Min( dropdownHeight, height ) );
		}

		void SelectCurrentItem()
		{
			// make initial selection after the parent shows
			if( !Visible )
				return;

			//select the first checked one
			if( isFlag )
			{
				foreach( var item in listViewEnum.CheckedItems )
				{
					listViewEnum.SelectedItem = item;
					listViewEnum.CurrentItem = item;
					listViewEnum.EnsureVisible( item );
					break;
				}
			}
			else
			{
				foreach( var item in listViewEnum.Items )
				{
					EnumInfo info = item.Tag as EnumInfo;
					if( info.Value.Equals( currentValue ) )
					{
						listViewEnum.SelectedItem = item;
						listViewEnum.CurrentItem = item;
						listViewEnum.EnsureVisible( item );
						break;
					}
				}
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			var keyCode = keyData & Keys.KeyCode;

			if( keyCode == Keys.Return )
			{
				ParentHolder.Close( true );
				return true;
			}
			else if( keyCode == Keys.Escape )
			{
				ParentHolder.Close( false );
				return true;
			}

			return base.ProcessCmdKey( ref msg, keyData );
		}

		public override void OnHolderOpened()
		{
			base.OnHolderOpened();

			listViewEnum.Focus();
			SelectCurrentItem();
		}

		public override void OnCommitChanges()
		{
			base.OnCommitChanges();

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
				itemProperty.SetValue( currentValue, false );
			valueWasChanged = true;
		}

		private void listViewEnum_ItemCheckedChanged( EngineListView sender, EngineListView.Item item )
		{
			var info = item.Tag as EnumInfo;

			if( info.SetByCode )
			{
				info.SetByCode = false;
				return;
			}

			if( item.Checked )
			{
				if( IsZero( enumDataType, info.Value ) ) //user is checking the item with zero value (None)
					currentValue = Enum.ToObject( currentValue.GetType(), 0 );
				else
					AddBits( enumDataType, ref currentValue, info.Value );
			}
			else
			{
				object prevValue = currentValue;
				RemoveBits( enumDataType, ref currentValue, info.Value );

				if( IsZeroValueSituation() )
					currentValue = prevValue;
			}

			UpdateCheckState();
			UpdateItemProperty();
		}

		bool IsZeroValueSituation()
		{
			if( IsZero( enumDataType, currentValue ) )
			{
				if( !Enum.IsDefined( propertyType, currentValue ) )
					return true;
			}
			return false;
		}

		private void listViewEnum_SelectedItemsChanged( EngineListView sender )
		{
			if( isFlag )
				return;

			if( listViewEnum.SelectedItems.Count == 0 )
				return;

			var info = sender.SelectedItems[ 0 ].Tag as EnumInfo;
			if( !info.Value.Equals( currentValue ) )
			{
				currentValue = info.Value;
				UpdateItemProperty();
			}
		}

		private void listViewEnum_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			ParentHolder.Close( true );
		}

		private void listViewEnum_MouseClick( object sender, MouseEventArgs e )
		{
			if( !isFlag )
				ParentHolder.Close( true );
		}

		void UpdateCheckState()
		{
			if( !isFlag )
				return;

			foreach( var item in listViewEnum.Items )
			{
				var info = item.Tag as EnumInfo;
				var bitExist = DoBitsExist( Enum.GetUnderlyingType( propertyType ), currentValue, info.Value );
				if( item.Checked != bitExist )
				{
					info.SetByCode = true;
					item.Checked = bitExist;
				}
			}
		}

		bool DoBitsExist( Type enumDataType, object value, object bits )
		{
			// zero needs special treatment, because you cannot do bitwise operations using zeros
			bool valueIsZero = IsZero( enumDataType, value );
			bool bitsIsZero = IsZero( enumDataType, bits );

			if( valueIsZero && bitsIsZero )
			{
				return true;
			}
			else if( valueIsZero && !bitsIsZero )
			{
				return false;
			}
			else if( !valueIsZero && bitsIsZero )
			{
				return false;
			}

			// otherwise (!valueIsZero && !bitsIsZero)

			if( enumDataType == typeof( Int16 ) )
			{
				Int16 val = Convert.ToInt16( value );
				Int16 bts = Convert.ToInt16( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt16 val = Convert.ToUInt16( value );
				UInt16 bts = Convert.ToUInt16( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				SByte val = Convert.ToSByte( value );
				SByte bts = Convert.ToSByte( bits );
				return ( ( val & bts ) == bts );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Byte val = Convert.ToByte( value );
				Byte bts = Convert.ToByte( bits );
				return ( ( val & bts ) == bts );
			}
			return false;
		}

		void RemoveBits( Type enumDataType, ref object value, object bits )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val &= ~( bts );
				value = Enum.ToObject( value.GetType(), val );
			}
		}

		void AddBits( Type enumDataType, ref object value, object bits )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				UInt32 bts = Convert.ToUInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				Int64 bts = Convert.ToInt64( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				UInt64 bts = Convert.ToUInt64( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Int32 val = Convert.ToInt32( value );
				Int32 bts = Convert.ToInt32( bits );
				val |= bts;
				value = Enum.ToObject( value.GetType(), val );
			}
		}

		bool IsZero( Type enumDataType, object value )
		{
			if( enumDataType == typeof( Int16 ) )
			{
				Int16 val = Convert.ToInt16( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt16 ) )
			{
				UInt16 val = Convert.ToUInt16( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Int32 ) )
			{
				Int32 val = Convert.ToInt32( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt32 ) )
			{
				UInt32 val = Convert.ToUInt32( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Int64 ) )
			{
				Int64 val = Convert.ToInt64( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( UInt64 ) )
			{
				UInt64 val = Convert.ToUInt64( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( SByte ) )
			{
				SByte val = Convert.ToSByte( value );
				return ( val == 0 );
			}
			else if( enumDataType == typeof( Byte ) )
			{
				Byte val = Convert.ToByte( value );
				return ( val == 0 );
			}
			return false;
		}
	}
}

#endif