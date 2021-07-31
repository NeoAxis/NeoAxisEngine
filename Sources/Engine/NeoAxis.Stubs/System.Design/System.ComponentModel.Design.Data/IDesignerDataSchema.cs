using System.Collections;

namespace System.ComponentModel.Design.Data
{
	public interface IDesignerDataSchema
	{
		ICollection GetSchemaItems(DesignerDataSchemaClass schemaClass);

		bool SupportsSchemaClass(DesignerDataSchemaClass schemaClass);
	}
}
