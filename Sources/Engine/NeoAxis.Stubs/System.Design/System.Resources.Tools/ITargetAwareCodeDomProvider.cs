namespace System.Resources.Tools
{
	public interface ITargetAwareCodeDomProvider
	{
		bool SupportsProperty(Type type, string propertyName, bool isWritable);
	}
}
