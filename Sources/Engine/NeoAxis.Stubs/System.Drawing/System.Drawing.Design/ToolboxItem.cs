using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Drawing.Design
{
	[Serializable]
	public class ToolboxItem : ISerializable
	{
		public AssemblyName AssemblyName
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public AssemblyName[] DependentAssemblies
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public Bitmap OriginalBitmap
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public string Company
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual string ComponentType
		{
			get
			{
				throw null;
			}
		}

		public string Description
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public string DisplayName
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public ICollection Filter
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool IsTransient
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual bool Locked
		{
			get
			{
				throw null;
			}
		}

		public IDictionary Properties
		{
			get
			{
				throw null;
			}
		}

		public string TypeName
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public virtual string Version
		{
			get
			{
				throw null;
			}
		}

		public event ToolboxComponentsCreatedEventHandler ComponentsCreated
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public event ToolboxComponentsCreatingEventHandler ComponentsCreating
		{
			add
			{
				throw null;
			}
			remove
			{
				throw null;
			}
		}

		public ToolboxItem()
		{
			throw null;
		}

		public ToolboxItem(Type toolType)
		{
			throw null;
		}

		protected void CheckUnlocked()
		{
			throw null;
		}

		public IComponent[] CreateComponents()
		{
			throw null;
		}

		public IComponent[] CreateComponents(IDesignerHost host)
		{
			throw null;
		}

		public IComponent[] CreateComponents(IDesignerHost host, IDictionary defaultValues)
		{
			throw null;
		}

		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
		{
			throw null;
		}

		protected virtual IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
		{
			throw null;
		}

		protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		protected virtual object FilterPropertyValue(string propertyName, object value)
		{
			throw null;
		}

		public Type GetType(IDesignerHost host)
		{
			throw null;
		}

		protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
		{
			throw null;
		}

		public virtual void Initialize(Type type)
		{
			throw null;
		}

		public virtual void Lock()
		{
			throw null;
		}

		protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
		{
			throw null;
		}

		protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
		{
			throw null;
		}

		protected virtual void Serialize(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		protected void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
		{
			throw null;
		}

		protected virtual object ValidatePropertyValue(string propertyName, object value)
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw null;
		}
	}
}
