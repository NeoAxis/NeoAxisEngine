#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public static class HCExtensions
	{
		public delegate void OverridePropertyReadOnlyDelegate( HCItemProperty itemProperty, ref bool? readOnly );
		public static event OverridePropertyReadOnlyDelegate OverridePropertyReadOnly;
		//public delegate void OverridePropertyReadOnlyDelegate( HCItemProperty itemProperty, ref bool propertyReadOnly, ref bool handled );
		//public static event OverridePropertyReadOnlyDelegate OverridePropertyReadOnly;

		public static void PerformOverridePropertyReadOnly( HCItemProperty itemProperty, ref bool? readOnly )
		//public static void PerformOverridePropertyReadOnly( HCItemProperty itemProperty, ref bool propertyReadOnly )
		{
			//var handled = false;
			OverridePropertyReadOnly?.Invoke( itemProperty, ref readOnly );
			//, ref handled );
			//if( handled )
			//	return;

			var property = itemProperty.Property;

			//Transform: Position, Rotation, Scale
			if( property.Name == "Position" || property.Name == "Rotation" || property.Name == "Scale" )
			{
				var ownerType = property.Owner as Metadata.NetTypeInfo;
				if( ownerType != null && ownerType.Type == typeof( Transform ) )
				{
					readOnly = false;
					//handled = true;
				}
			}

			//array: Length
			if( property.Name == "Length" )
			{
				var ownerType = property.Owner as Metadata.TypeInfo;
				if( ownerType != null && ownerType.GetNetType() == typeof( Array ) )// HCItemProperty.IsOneDimensionArray( ownerType.GetNetType() ) )
				{
					var parentItemProperty = itemProperty.Parent as HCItemProperty;
					if( parentItemProperty != null && !parentItemProperty.Property.ReadOnly )
					{
						readOnly = false;
						//handled = true;
					}
				}
			}

			//List, ReferenceList: Count
			if( property.Name == "Count" )
			{
				var ownerType = property.Owner as Metadata.TypeInfo;
				if( ownerType != null && HCItemProperty.IsListType( ownerType.GetNetType() ) )
				{
					readOnly = false;
					//handled = true;
				}
			}
		}

		/////////////////////////////////////////

		public delegate void OverridePropertySetValueDelegate( HCItemProperty.PropertySetValueData data );
		public static event OverridePropertySetValueDelegate OverridePropertySetValue;

		public static void PerformOverridePropertySetValue( HCItemProperty.PropertySetValueData data )
		{
			OverridePropertySetValue?.Invoke( data );

			var property = data.itemProperty.Property;

			//Transform: Position, Rotation, Scale
			if( data.parentItemProperty != null && ReferenceUtility.GetUnreferencedType( data.parentItemProperty.Property.Type.GetNetType() ) == typeof( Transform ) )
			{
				for( int n = 0; n < data.itemProperty.ControlledObjects.Length; n++ )
				{
					var old = data.itemProperty.ControlledObjects[ n ];
					if( property.Name == "Position" )
						data.itemProperty.ControlledObjects[ n ] = old.GetType().GetMethod( "UpdatePosition" ).Invoke( old, new object[] { data.value } );
					else if( property.Name == "Rotation" )
						data.itemProperty.ControlledObjects[ n ] = old.GetType().GetMethod( "UpdateRotation" ).Invoke( old, new object[] { data.value } );
					else if( property.Name == "Scale" )
						data.itemProperty.ControlledObjects[ n ] = old.GetType().GetMethod( "UpdateScale" ).Invoke( old, new object[] { data.value } );
				}

				data.setValueHandled = true;
			}

			//array: Length
			if( property.Name == "Length" )
			{
				var ownerType = property.Owner as Metadata.TypeInfo;
				if( ownerType != null && ownerType.GetNetType() == typeof( Array ) )// HCItemProperty.IsOneDimensionArray( ownerType.GetNetType() ) )
				{
					int newLength = (int)data.unrefValue;

					for( int nCollectedObject = 0; nCollectedObject < data.parentItemProperty.ControlledObjects.Length; nCollectedObject++ )
					{
						var array = (IList)ReferenceUtility.GetUnreferencedValue( data.itemProperty.ControlledObjects[ nCollectedObject ] );
						if( array != null && array.Count != newLength )
						{
							var newArray = Array.CreateInstance( array.GetType().GetElementType(), newLength );
							Array.Copy( (Array)array, newArray, Math.Min( newLength, array.Count ) );

							//!!!!multiselection. вместе undo все

							data.parentItemProperty.SetValue( newArray, data.addUndo );
						}
					}

					data.setValueHandled = true;
					data.addUndoHandled = true;
					data.updateParentPropertyHandled = true;
				}
			}

			//List: Count
			if( property.Name == "Count" )
			{
				var ownerType = property.Owner as Metadata.TypeInfo;
				if( ownerType != null && HCItemProperty.IsListType( ownerType.GetNetType() ) )
				{
					int newCount = (int)data.unrefValue;

					for( int nCollectedObject = 0; nCollectedObject < data.parentItemProperty.ControlledObjects.Length; nCollectedObject++ )
					{
						var list = (IList)ReferenceUtility.GetUnreferencedValue( data.itemProperty.ControlledObjects[ nCollectedObject ] );
						if( list != null )
						{
							if( newCount < list.Count )
							{
								//remove items

								if( !data.addUndoHandled && data.addUndo && data.itemProperty.Owner.DocumentWindow?.Document2 != null )
								{
									//remove with undo

									var indexes = new List<int>();
									for( int n = newCount; n < list.Count; n++ )
										indexes.Add( n );
									var undoAction = new UndoActionListAddRemove( list, indexes, false );

									DocumentInstance document = data.itemProperty.Owner.DocumentWindow.Document2;
									document.UndoSystem.CommitAction( undoAction );
									document.Modified = true;
								}
								else
								{
									//remove without undo
									while( list.Count > newCount )
										list.RemoveAt( list.Count - 1 );
								}
							}
							else if( newCount > list.Count )
							{
								//add items

								var indexes = new List<int>();
								for( int n = list.Count; n < newCount; n++ )
									indexes.Add( n );

								var elementType = list.GetType().GetGenericArguments()[ 0 ];
								while( list.Count < newCount )
								{
									object itemValue = null;
									var referenceList = list as IReferenceList;
									if( referenceList != null )
										itemValue = Activator.CreateInstance( referenceList.GetItemType() );// referenceList.CreateItemValue();
									else if( elementType.IsValueType )
										itemValue = Activator.CreateInstance( elementType );
									list.Add( itemValue );
								}

								//undo
								if( !data.addUndoHandled && data.addUndo && data.itemProperty.Owner.DocumentWindow?.Document2 != null )
								{
									var undoAction = new UndoActionListAddRemove( list, indexes, true );

									DocumentInstance document = data.itemProperty.Owner.DocumentWindow.Document2;
									document.UndoSystem.CommitAction( undoAction );
									document.Modified = true;
								}
							}
						}
					}

					data.setValueHandled = true;
					data.addUndoHandled = true;
					data.updateParentPropertyHandled = true;
				}
			}
		}
	}
}

#endif