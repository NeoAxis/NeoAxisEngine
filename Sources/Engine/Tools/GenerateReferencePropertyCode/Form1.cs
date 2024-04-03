using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenerateReferencePropertyCode
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load( object sender, EventArgs e )
		{
			Generate();
		}

		private void textBoxClass_TextChanged( object sender, EventArgs e )
		{
			Generate();
		}

		private void textBoxPropertyName_TextChanged( object sender, EventArgs e )
		{
			Generate();
		}

		private void textBoxPropertyType_TextChanged( object sender, EventArgs e )
		{
			Generate();
		}

		private void textBoxDefaultValue_TextChanged( object sender, EventArgs e )
		{
			Generate();
		}

		private void checkBoxHowToExpand_CheckedChanged( object sender, EventArgs e )
		{
			Generate();
		}

		private void checkBoxCompact_CheckedChanged( object sender, EventArgs e )
		{
			Generate();
		}

		void Generate()
		{
			//[Metadata.Serialize]


			string templateWithoutHelp =
				@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get
	{
		if( {FieldName}.BeginGet() )
			{PropertyName} = {FieldName}.Get( this );
		return {FieldName}.value;
	}
	set
	{
		if( {FieldName}.BeginSet( this, ref value ) )
		{
{CheckValue}
			try { {PropertyName}Changed?.Invoke( this ); }
			finally { {FieldName}.EndSet(); }
			{AdditionalActionsWrong}
		}
	}
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
";

			//[Metadata.Serialize]

			string templateCompact =
	@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get { if( {FieldName}.BeginGet() ) {PropertyName} = {FieldName}.Get( this ); return {FieldName}.value; }
	set { if( {FieldName}.BeginSet( this, ref value ) ) { {CheckValue} try { {PropertyName}Changed?.Invoke( this ); } finally { {FieldName}.EndSet(); } {AdditionalActionsWrong} } }
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
";

			//[Metadata.Serialize]

			string templateWithHelp =
	@"{DefaultValueAttribute}
public Reference<{TypeName}> {PropertyName}
{
	get
	{
		if( {FieldName}.BeginGet() )
			{PropertyName} = {FieldName}.Get( this );
		return {FieldName}.value;
	}
	set
	{
{CheckValue}
		if( {FieldName}.BeginSet( this, ref value ) )
		{
			try
			{
				{PropertyName}Changed?.Invoke( this );
				{AdditionalActions}
			}
			finally { {FieldName}.EndSet(); }
			{AdditionalActionsWrong}
		}
	}
}
/// <summary>Occurs when the <see cref={Quote}{PropertyName}{Quote}/> property value changes.</summary>
public event Action<{ClassName}> {PropertyName}Changed;
ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
				";

			//			string templateWithoutHelp =
			//				@"//{PropertyName}
			//ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
			//{DefaultValueAttribute}
			//[Metadata.Serialize]
			//public Reference<{TypeName}> {PropertyName}
			//{
			//	get
			//	{
			//		if( {FieldName}.BeginGet() )
			//			{PropertyName} = {FieldName}.Get( this );
			//		return {FieldName}.value;
			//	}
			//	set
			//	{
			//		if( {FieldName}.BeginSet( ref value ) )
			//		{
			//{CheckValue}
			//			try { {PropertyName}Changed?.Invoke( this ); }
			//			finally { {FieldName}.EndSet(); }
			//			{AdditionalActionsWrong}
			//		}
			//	}
			//}
			//public event Action<{ClassName}> {PropertyName}Changed;
			//				";

			//			string templateCompact =
			//	@"//{PropertyName}
			//ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
			//{DefaultValueAttribute}
			//[Metadata.Serialize]
			//public Reference<{TypeName}> {PropertyName}
			//{
			//	get { if( {FieldName}.BeginGet() ) {PropertyName} = {FieldName}.Get( this ); return {FieldName}.value; }
			//	set { if( {FieldName}.BeginSet( ref value ) ) { {CheckValue} try { {PropertyName}Changed?.Invoke( this ); } finally { {FieldName}.EndSet(); } {AdditionalActionsWrong} } }
			//}
			//public event Action<{ClassName}> {PropertyName}Changed;
			//				";

			//			string templateWithHelp =
			//	@"//{PropertyName}
			//ReferenceField<{TypeName}> {FieldName}{DefaultValueSet};
			//{DefaultValueAttribute}
			//[Metadata.Serialize]
			//public Reference<{TypeName}> {PropertyName}
			//{
			//	get
			//	{
			//		if( {FieldName}.BeginGet() )
			//			{PropertyName} = {FieldName}.Get( this );
			//		return {FieldName}.value;
			//	}
			//	set
			//	{
			//		if( {FieldName}.BeginSet( ref value ) )
			//		{
			//{CheckValue}
			//			try
			//			{
			//				{PropertyName}Changed?.Invoke( this );
			//				{AdditionalActions}
			//			}
			//			finally { {FieldName}.EndSet(); }
			//			{AdditionalActionsWrong}
			//		}
			//	}
			//}
			//public event Action<{ClassName}> {PropertyName}Changed;
			//				";

			string className = textBoxClass.Text;
			var propertyName = textBoxPropertyName.Text;
			string fieldName = "";
			if( propertyName.Length > 0 )
				fieldName = "_" + propertyName.Substring( 0, 1 ).ToLower() + propertyName.Substring( 1 );
			string typeName = textBoxPropertyType.Text;
			string defaultValue = textBoxDefaultValue.Text;
			bool help = checkBoxHowToExpand.Checked;
			var compact = checkBoxCompact.Checked;

			string text = help ? templateWithHelp : ( compact ? templateCompact : templateWithoutHelp );

			text = text.Replace( "{ClassName}", className );
			text = text.Replace( "{PropertyName}", propertyName );
			text = text.Replace( "{FieldName}", fieldName );
			text = text.Replace( "{TypeName}", typeName );
			text = text.Replace( "{Quote}", "\"" );

			if( !string.IsNullOrEmpty( defaultValue ) )
			{
				text = text.Replace( "{DefaultValueSet}", string.Format( " = {0}", defaultValue ) );
				text = text.Replace( "{DefaultValueAttribute}", string.Format( "[DefaultValue( {0} )]", defaultValue ) );
			}
			else
			{
				text = text.Replace( "{DefaultValueSet}", "" );
				text = text.Replace( "{DefaultValueAttribute}", "" );
			}

			if( help )
			{
				string checkValueText =
@"		@@@ Do here the fixing of 'value'. Don't use another properties.
		@@@ Example:
		@@@ if( value < 3 )
		@@@ 	value = new Reference<int>( 3, value.GetByReference );";

				text = text.Replace( "{CheckValue}", checkValueText );
				text = text.Replace( "{AdditionalActions}", "@@@ Add here additional actions. Update internal object, reset cache, etc." );
				text = text.Replace( "{AdditionalActionsWrong}", "@@@ Don't add additional actions here." );
			}
			else
			{
				text = text.Replace( "{CheckValue}", "" );
				text = text.Replace( "{AdditionalActions}", "" );
				text = text.Replace( "{AdditionalActionsWrong}", "" );
			}

			text = text.Replace( "\r\n\r\n", "\r\n" );
			text = text.Replace( "\r\n			\r\n", "\r\n" );
			text = text.Replace( "\r\n				\r\n", "\r\n" );

			richTextBoxOutput.Text = text;
		}
	}
}
