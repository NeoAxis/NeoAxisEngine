// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public partial class SpecifyParametersForm : EngineForm
	{
		ObjectImpl objectImpl = new ObjectImpl();

		public delegate bool CheckDelegate( ref string error );
		CheckDelegate checkHandler;

		public delegate bool OKDelegate( ref string error );
		OKDelegate okHandler;

		bool loaded;

		/////////////////////////////////////

		public class PropertyImpl : Metadata.Property
		{
			IList<Attribute> attributes;
			string category;
			object value;

			//

			public PropertyImpl( SpecifyParametersForm owner, string name, Metadata.TypeInfo type, IList<Attribute> attributes, string category, object value )
				: base( owner, name, false, type, type, new Metadata.Parameter[ 0 ], false )
			{
				this.attributes = attributes;
				this.category = category;
				this.value = value;
			}

			public IList<Attribute> Attributes
			{
				get { return attributes; }
				set { attributes = value; }
			}

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

				if( attributes != null )
				{
					foreach( var a in attributes )
					{
						if( attributeType.IsAssignableFrom( a.GetType() ) )
							result.Add( a );
					}
				}

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
				bool changed = !Equals( this.value, value );

				this.value = value;

				if( changed )
				{
					var form = (SpecifyParametersForm)Owner;
					form.ValueChanged();
				}
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

		public SpecifyParametersForm( string caption, object optionalObjectProvider, CheckDelegate checkHandler = null, OKDelegate okHandler = null )
		{
			this.checkHandler = checkHandler;
			this.okHandler = okHandler;

			InitializeComponent();

			if( optionalObjectProvider != null )
				hierarchicalContainer1.SetData( null, new object[] { optionalObjectProvider } );
			else
				hierarchicalContainer1.SetData( null, new object[] { objectImpl } );

			if( string.IsNullOrEmpty( caption ) )
				Text = EngineInfo.NameWithVersion;
			else
				Text = caption;

			labelError.Text = "";

			EditorThemeUtility.ApplyDarkThemeToForm( this );
			labelError.ForeColor = Color.Red;
		}

		private void OKCancelTextBoxForm_Load( object sender, EventArgs e )
		{
			UpdateControls();

			loaded = true;

			//Translate();
		}

		[Browsable( false )]
		public List<PropertyImpl> Properties
		{
			get
			{
				if( objectImpl != null )
					return objectImpl.properties;
				else
					return new List<PropertyImpl>();
			}
		}

		[Browsable( false )]
		public CheckDelegate CheckHandler
		{
			get { return checkHandler; }
			set { checkHandler = value; }
		}

		[Browsable( false )]
		public OKDelegate OKHandler
		{
			get { return okHandler; }
			set { okHandler = value; }
		}

		public PropertyImpl AddProperty( string name, string category, object value, IList<Attribute> attributes = null )
		{
			var property = new PropertyImpl( this, StringUtility.ToUpperFirstCharacter( name ), MetadataManager.GetTypeOfNetType( value.GetType() ), attributes, category, value );
			property.DefaultValueSpecified = true;
			property.DefaultValue = value;

			Properties.Add( property );

			return property;
		}

		void ValueChanged()
		{
			if( !loaded )
				return;

			string error = "";
			if( checkHandler != null && !checkHandler( ref error ) )
			{
				labelError.Text = error;
				buttonOK.Enabled = false;
			}
			else
			{
				labelError.Text = "";
				buttonOK.Enabled = true;
			}
		}

		private void RenameResourceDialog_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( DialogResult == DialogResult.OK )
			{
				string error = "";
				if( okHandler != null && !okHandler( ref error ) )
				{
					e.Cancel = true;
					labelError.Text = error;
					return;
				}
			}
		}

		void UpdateControls()
		{
			buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			buttonOK.Location = new Point( buttonCancel.Location.X - buttonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonCancel.Location.Y );
			labelError.Location = new Point( labelError.Location.X, buttonOK.Location.Y + DpiHelper.Default.ScaleValue( 3 ) );
			hierarchicalContainer1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - hierarchicalContainer1.Location.X, buttonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - hierarchicalContainer1.Location.Y );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		//void Translate()
		//{
		//	buttonOK.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonOK.Text );
		//	buttonCancel.Text = ToolsLocalization.Translate( "OKCancelTextBoxDialog", buttonCancel.Text );
		//}

		//public void UpdateFonts( string fontForm )
		//{
		//	if( !string.IsNullOrEmpty( fontForm ) && fontForm[ 0 ] != '(' )
		//	{
		//		try
		//		{
		//			System.Drawing.FontConverter fontConverter = new System.Drawing.FontConverter();
		//			Font = (System.Drawing.Font)fontConverter.ConvertFromString( fontForm );
		//		}
		//		catch { }
		//	}
		//}
	}
}