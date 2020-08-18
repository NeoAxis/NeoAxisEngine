namespace System.Drawing.Design
{
	public interface IToolboxUser
	{
		bool GetToolSupported(ToolboxItem tool);

		void ToolPicked(ToolboxItem tool);
	}
}
