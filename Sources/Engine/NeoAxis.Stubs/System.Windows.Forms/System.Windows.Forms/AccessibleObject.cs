using Accessibility;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;

namespace System.Windows.Forms
{
	public class AccessibleObject : StandardOleMarshalObject, IReflect//, IAccessible
	{
		public virtual Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public virtual string DefaultAction
		{
			get
			{
				throw null;
			}
		}

		public virtual string Description
		{
			get
			{
				throw null;
			}
		}

		public virtual string Help
		{
			get
			{
				throw null;
			}
		}

		public virtual string KeyboardShortcut
		{
			get
			{
				throw null;
			}
		}

		public virtual string Name
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

		public virtual AccessibleObject Parent
		{
			get
			{
				throw null;
			}
		}

		public virtual AccessibleRole Role
		{
			get
			{
				throw null;
			}
		}

		public virtual AccessibleStates State
		{
			get
			{
				throw null;
			}
		}

		public virtual string Value
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

		//int IAccessible.accChildCount
		//{
		//	get
		//	{
		//		throw null;
		//	}
		//}

		//object IAccessible.accFocus
		//{
		//	get
		//	{
		//		throw null;
		//	}
		//}

		//object IAccessible.accParent
		//{
		//	get
		//	{
		//		throw null;
		//	}
		//}

		//object IAccessible.accSelection
		//{
		//	get
		//	{
		//		throw null;
		//	}
		//}

		Type IReflect.UnderlyingSystemType
		{
			get
			{
				throw null;
			}
		}

		public object accChild
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string accName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string accValue
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string accDescription
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object accRole
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object accState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string accHelp
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int accHelpTopic
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string accKeyboardShortcut
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string accDefaultAction
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public AccessibleObject()
		{
			throw null;
		}

		public virtual AccessibleObject GetChild(int index)
		{
			throw null;
		}

		public virtual int GetChildCount()
		{
			throw null;
		}

		public virtual AccessibleObject GetFocused()
		{
			throw null;
		}

		public virtual int GetHelpTopic(out string fileName)
		{
			throw null;
		}

		public virtual AccessibleObject GetSelected()
		{
			throw null;
		}

		public virtual AccessibleObject HitTest(int x, int y)
		{
			throw null;
		}

		//void IAccessible.accDoDefaultAction(object childID)
		//{
		//	throw null;
		//}

		//object IAccessible.accHitTest(int xLeft, int yTop)
		//{
		//	throw null;
		//}

		//void IAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object childID)
		//{
		//	throw null;
		//}

		//object IAccessible.accNavigate(int navDir, object childID)
		//{
		//	throw null;
		//}

		//void IAccessible.accSelect(int flagsSelect, object childID)
		//{
		//	throw null;
		//}

		public virtual void DoDefaultAction()
		{
			throw null;
		}

		//object IAccessible.get_accChild(object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accDefaultAction(object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accDescription(object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accHelp(object childID)
		//{
		//	throw null;
		//}

		//int IAccessible.get_accHelpTopic(out string pszHelpFile, object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accKeyboardShortcut(object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accName(object childID)
		//{
		//	throw null;
		//}

		//object IAccessible.get_accRole(object childID)
		//{
		//	throw null;
		//}

		//object IAccessible.get_accState(object childID)
		//{
		//	throw null;
		//}

		//string IAccessible.get_accValue(object childID)
		//{
		//	throw null;
		//}

		//void IAccessible.set_accName(object childID, string newName)
		//{
		//	throw null;
		//}

		//void IAccessible.set_accValue(object childID, string newValue)
		//{
		//	throw null;
		//}

		public virtual AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			throw null;
		}

		public virtual void Select(AccessibleSelection flags)
		{
			throw null;
		}

		protected void UseStdAccessibleObjects( IntPtr handle )
		{
			throw null;
		}

		protected void UseStdAccessibleObjects( IntPtr handle, int objid)
		{
			throw null;
		}

		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			throw null;
		}

		MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
		{
			throw null;
		}

		MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
		{
			throw null;
		}

		FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
		{
			throw null;
		}

		FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
		{
			throw null;
		}

		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
		{
			throw null;
		}

		PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw null;
		}

		PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
		{
			throw null;
		}

		MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
		{
			throw null;
		}

		MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
		{
			throw null;
		}

		object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw null;
		}

		public bool RaiseAutomationNotification(AutomationNotificationKind notificationKind, AutomationNotificationProcessing notificationProcessing, string notificationText)
		{
			throw null;
		}

		public virtual bool RaiseLiveRegionChanged()
		{
			throw null;
		}
	}
}
