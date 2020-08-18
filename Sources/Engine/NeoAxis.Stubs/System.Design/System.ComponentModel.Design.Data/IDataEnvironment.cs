using System.CodeDom;
using System.Collections;
using System.Data.Common;
using System.Windows.Forms;

namespace System.ComponentModel.Design.Data
{
	public interface IDataEnvironment
	{
		ICollection Connections
		{
			get;
		}

		DesignerDataConnection BuildConnection(IWin32Window owner, DesignerDataConnection initialConnection);

		string BuildQuery(IWin32Window owner, DesignerDataConnection connection, QueryBuilderMode mode, string initialQueryText);

		DesignerDataConnection ConfigureConnection(IWin32Window owner, DesignerDataConnection connection, string name);

		IDesignerDataSchema GetConnectionSchema(DesignerDataConnection connection);

		DbConnection GetDesignTimeConnection(DesignerDataConnection connection);

		CodeExpression GetCodeExpression(DesignerDataConnection connection);
	}
}
