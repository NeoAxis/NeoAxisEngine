using System.CodeDom;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class CodeDomSerializerBase
	{
		protected virtual object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer)
		{
			throw null;
		}

		protected static TypeDescriptionProvider GetTargetFrameworkProvider(IServiceProvider provider, object instance)
		{
			throw null;
		}

		protected static Type GetReflectionTypeFromTypeHelper(IDesignerSerializationManager manager, Type type)
		{
			throw null;
		}

		protected static Type GetReflectionTypeHelper(IDesignerSerializationManager manager, object instance)
		{
			throw null;
		}

		protected static PropertyDescriptorCollection GetPropertiesHelper(IDesignerSerializationManager manager, object instance, Attribute[] attributes)
		{
			throw null;
		}

		protected static EventDescriptorCollection GetEventsHelper(IDesignerSerializationManager manager, object instance, Attribute[] attributes)
		{
			throw null;
		}

		protected static AttributeCollection GetAttributesHelper(IDesignerSerializationManager manager, object instance)
		{
			throw null;
		}

		protected static AttributeCollection GetAttributesFromTypeHelper(IDesignerSerializationManager manager, Type type)
		{
			throw null;
		}

		protected object DeserializeExpression(IDesignerSerializationManager manager, string name, CodeExpression expression)
		{
			throw null;
		}

		protected void DeserializePropertiesFromResources(IDesignerSerializationManager manager, object value, Attribute[] filter)
		{
			throw null;
		}

		protected void DeserializeStatement(IDesignerSerializationManager manager, CodeStatement statement)
		{
			throw null;
		}

		protected CodeExpression GetExpression(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected CodeDomSerializer GetSerializer(IDesignerSerializationManager manager, Type valueType)
		{
			throw null;
		}

		protected bool IsSerialized(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected bool IsSerialized(IDesignerSerializationManager manager, object value, bool honorPreset)
		{
			throw null;
		}

		protected CodeExpression SerializeCreationExpression(IDesignerSerializationManager manager, object value, out bool isComplete)
		{
			throw null;
		}

		protected string GetUniqueName(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected void SerializeEvent(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, EventDescriptor descriptor)
		{
			throw null;
		}

		protected void SerializeEvents(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, params Attribute[] filter)
		{
			throw null;
		}

		protected void SerializeProperties(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[] filter)
		{
			throw null;
		}

		protected void SerializePropertiesToResources(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[] filter)
		{
			throw null;
		}

		protected void SerializeProperty(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, PropertyDescriptor propertyToSerialize)
		{
			throw null;
		}

		protected void SerializeResource(IDesignerSerializationManager manager, string resourceName, object value)
		{
			throw null;
		}

		protected void SerializeResourceInvariant(IDesignerSerializationManager manager, string resourceName, object value)
		{
			throw null;
		}

		protected CodeExpression SerializeToExpression(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value)
		{
			throw null;
		}

		protected CodeExpression SerializeToResourceExpression(IDesignerSerializationManager manager, object value, bool ensureInvariant)
		{
			throw null;
		}

		protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression)
		{
			throw null;
		}

		protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression, bool isPreset)
		{
			throw null;
		}
	}
}
