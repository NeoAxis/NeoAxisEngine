// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class CharacterSettingsCell : SettingsCellProcedureUI
	{
		ProcedureUI.Button buttonItemTake;

		//

		protected override void OnInit()
		{
			buttonItemTake = ProcedureForm.CreateButton( "Take Item" );
			buttonItemTake.Click += ButtonItemTake_Click;

			ProcedureForm.AddRow( new ProcedureUI.Control[] { buttonItemTake } );
		}

		private void ButtonItemTake_Click( ProcedureUI.Button sender )
		{
			var initData = new SelectTypeWindowInitData();
			initData.DocumentWindow = Provider.DocumentWindow;

			initData.DemandedTypes = new Metadata.TypeInfo[] {
				MetadataManager.GetTypeOfNetType( typeof( WeaponType ) ),
				MetadataManager.GetTypeOfNetType( typeof( ItemType ) ) };

			initData.WasSelected = delegate ( Metadata.TypeInfo selectedType, ref bool cancel )
			{
				var isWeaponType = typeof( WeaponType ).IsAssignableFrom( selectedType.GetNetType() );
				var isItemType = typeof( ItemType ).IsAssignableFrom( selectedType.GetNetType() );

				if( isWeaponType || isItemType )
				{
					try
					{
						var createdComponents = new List<Component>();

						foreach( var character in GetObjects<Character>() )
						{
							ItemInterface item = null;
							if( isWeaponType )
							{
								var weapon = ComponentUtility.CreateComponent<Weapon>( null, false, false );
								weapon.Name = "Weapon";
								weapon.WeaponType = new ReferenceNoValue( selectedType.Name );
								if( weapon.WeaponType.Value == null )
									weapon.WeaponType = new ReferenceNoValue( Weapon.WeaponTypeDefault );
								item = weapon;
							}
							if( isItemType )
							{
								var basicItem = ComponentUtility.CreateComponent<Item>( null, false, false );
								basicItem.Name = "Item";
								basicItem.ItemType = new ReferenceNoValue( selectedType.Name );
								//if( basicItem.ItemType.Value == null )
								//	basicItem.ItemType = new ReferenceNoValue( Item.TypeDefault );
								item = basicItem;
							}

							if( character.ItemTake( null, item ) )
							{
								if( character.GetActiveItem() == null )
									character.ItemActivate( null, item );

								createdComponents.Add( (Component)item );
							}
						}

						//undo
						if( createdComponents.Count != 0 )
						{
							var document = Provider.DocumentWindow.Document;
							var action = new UndoActionComponentCreateDelete( document, createdComponents.ToArray(), true );
							document.UndoSystem.CommitAction( action );
							document.Modified = true;
						}
					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
					}
				}
			};

			EditorAPI.OpenSelectTypeWindow( initData );
		}
	}
}
#endif