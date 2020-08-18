using System.Diagnostics;

namespace System.ComponentModel.Design
{
	public interface IComponentDesignerDebugService
	{
		int IndentLevel
		{
			get;
			set;
		}

		TraceListenerCollection Listeners
		{
			get;
		}

		void Assert(bool condition, string message);

		void Fail(string message);

		void Trace(string message, string category);
	}
}
