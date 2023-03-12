#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class ComponentTypeSettingsForm : EngineForm
	{
		DocumentInstance document;
		Component component;
		ObjectImpl objectImpl = new ObjectImpl();

		/////////////////////////////////////

		public enum ObjectTypeEnum
		{
			Component,
			Property
		}

		/////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			public ObjectTypeEnum objectType;
			public Component component;
			//IList<Attribute> attributes;
			string category;
			object value;

			//

			public PropertyImpl( ComponentTypeSettingsForm owner, ObjectTypeEnum objectType, Component component, string name, Metadata.TypeInfo type, /*IList<Attribute> attributes,*/ string category, object value )
				: base( owner, name, false, type, type, new Metadata.Parameter[ 0 ], false )
			{
				this.objectType = objectType;
				this.component = component;
				//this.attributes = attributes;
				this.category = category;
				this.value = value;
			}

			//public IList<Attribute> Attributes
			//{
			//	get { return attributes; }
			//	set { attributes = value; }
			//}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			public object Value
			{
				get { return value; }
				set { this.value = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//if( attributes != null )
				//{
				//	foreach( var a in attributes )
				//	{
				//		if( attributeType.IsAssignableFrom( a.GetType() ) )
				//			result.Add( a );
				//	}
				//}

				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				return value;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				this.value = value;
			}
		}

		/////////////////////////////////////

		class ObjectImpl : Metadata.IMetadataProvider
		{
			public List<PropertyImpl> properties = new List<PropertyImpl>();

			//

			public Metadata.TypeInfo BaseType
			{
				get { return MetadataManager.GetTypeOfNetType( GetType() ); }
			}

			public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
			{
				foreach( var p in properties )
					yield return p;
			}

			public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
			{
				foreach( var p in properties )
				{
					if( p.Signature == signature )
						return p;
				}
				return null;
			}
		}

		/////////////////////////////////////

		public ComponentTypeSettingsForm( DocumentInstance document, Component component )
		{
			//debug = true;

			this.document = document;
			this.component = component;

			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			hierarchicalContainer1.OverrideGroupDisplayName += HierarchicalContainer1_OverrideGroupDisplayName;

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			Text = EditorLocalization.Translate( "TypeSettingsForm", Text );
			EditorLocalization.TranslateForm( "TypeSettingsForm", this );
		}

		private void ContentTypeSettingsForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateControls();

			//ESet<string> addedComponentNames = new ESet<string>();

			foreach( var child in component.Components )
			{
				if( child.DisplayInEditor && !child.TypeOnly && !string.IsNullOrEmpty( child.Name ) && child.TypeSettingsIsPublic() )//&& !addedComponentNames.Contains( child.Name ) )
				{
					//bool skip = false;

					//Type Settings filter
					//var baseComponentType = component.BaseType as Metadata.ComponentTypeInfo;
					//if( baseComponentType != null && component.TypeSettingsIsPrivateObject( child ) )
					//	skip = true;

					//if( !skip )
					//{

					bool value = !ComponentUtility.TypeSettingsPrivateObjectsContains( component.TypeSettingsPrivateObjects, child );
					//bool value = !component._TypeSettingsIsPrivateObject( child, false );
					//List<Attribute> attributes = new List<Attribute>();

					var propertyImpl = new PropertyImpl( this, ObjectTypeEnum.Component, child, child.Name, MetadataManager.GetTypeOfNetType( typeof( bool ) ), "Components", value );
					propertyImpl.DefaultValueSpecified = true;
					propertyImpl.DefaultValue = true;

					objectImpl.properties.Add( propertyImpl );

					//addedComponentNames.Add( child.Name );
					//}
				}
			}

			foreach( var member in MetadataManager.MetadataGetMembers( component ) )
			{
				var property = member as Metadata.Property;

				//!!!!что еще? от базового класс Type Settings

				if( property != null && EditorUtility.IsMemberVisible( component, property ) )
				{
					bool value = !ComponentUtility.TypeSettingsPrivateObjectsContains( component.TypeSettingsPrivateObjects, property );
					//bool value = !component._TypeSettingsIsPrivateObject( property, false );
					//List<Attribute> attributes = new List<Attribute>();

					var propertyImpl = new PropertyImpl( this, ObjectTypeEnum.Property, null, property.Name, MetadataManager.GetTypeOfNetType( typeof( bool ) ), "Properties", value );
					propertyImpl.DefaultValueSpecified = true;
					propertyImpl.DefaultValue = true;

					objectImpl.properties.Add( propertyImpl );
				}
			}

			hierarchicalContainer1.SetData( null, new object[] { objectImpl } );

			//UpdateControlsBounds();
		}

		private void buttonOK_Click( object sender, EventArgs e )
		{
			Close();

			//get new value
			string[] newValue = null;
			{
				var list = new List<string>();

				foreach( var property in objectImpl.properties )
				{
					if( !(bool)property.Value )
					{
						switch( property.objectType )
						{
						case ObjectTypeEnum.Component:
							list.Add( property.component.GetPathFromParent() );
							//list.Add( "$" + property.Name );
							break;

						case ObjectTypeEnum.Property:
							list.Add( property.Name );
							break;
						}
					}
				}

				if( list.Count != 0 )
					newValue = list.ToArray();
			}

			var oldValue = component.TypeSettingsPrivateObjects;

			//update property
			component.TypeSettingsPrivateObjects = newValue;

			//undo
			var undoItem = new UndoActionPropertiesChange.Item( component, (Metadata.Property)MetadataManager.GetTypeOfNetType( typeof( Component ) ).MetadataGetMemberBySignature( "property:TypeSettingsPrivateObjects" ), oldValue, null );
			document.UndoSystem.CommitAction( new UndoActionPropertiesChange( undoItem ) );
			document.Modified = true;
		}

		private void buttonCancel_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void kryptonButtonReset_Click( object sender, EventArgs e )
		{
			foreach( var property in objectImpl.properties )
				property.Value = true;
		}

		private void HierarchicalContainer1_OverrideGroupDisplayName( HierarchicalContainer sender, HCItemGroup group, ref string displayName )
		{
			displayName = EditorLocalization.Translate( "TypeSettingsForm", displayName );
		}

		void UpdateControls()
		{
			kryptonButtonCancel.Location = new Point( ClientSize.Width - kryptonButtonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - kryptonButtonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			kryptonButtonOK.Location = new Point( kryptonButtonCancel.Location.X - kryptonButtonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), kryptonButtonCancel.Location.Y );
			kryptonButtonReset.Location = new Point( kryptonButtonReset.Location.X, kryptonButtonOK.Location.Y );
			hierarchicalContainer1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - hierarchicalContainer1.Location.X, kryptonButtonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - hierarchicalContainer1.Location.Y );
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