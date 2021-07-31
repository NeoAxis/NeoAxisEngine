using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
	[Serializable]
	public class TreeNode : MarshalByRefObject, ICloneable, ISerializable
	{
		public Color BackColor
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

		public Rectangle Bounds
		{
			get
			{
				throw null;
			}
		}

		public bool Checked
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

		public virtual ContextMenu ContextMenu
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

		public virtual ContextMenuStrip ContextMenuStrip
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

		public TreeNode FirstNode
		{
			get
			{
				throw null;
			}
		}

		public Color ForeColor
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

		public string FullPath
		{
			get
			{
				throw null;
			}
		}

		public IntPtr Handle
		{
			get
			{
				throw null;
			}
		}

		public int ImageIndex
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

		public string ImageKey
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

		public int Index
		{
			get
			{
				throw null;
			}
		}

		public bool IsEditing
		{
			get
			{
				throw null;
			}
		}

		public bool IsExpanded
		{
			get
			{
				throw null;
			}
		}

		public bool IsSelected
		{
			get
			{
				throw null;
			}
		}

		public bool IsVisible
		{
			get
			{
				throw null;
			}
		}

		public TreeNode LastNode
		{
			get
			{
				throw null;
			}
		}

		public int Level
		{
			get
			{
				throw null;
			}
		}

		public TreeNode NextNode
		{
			get
			{
				throw null;
			}
		}

		public TreeNode NextVisibleNode
		{
			get
			{
				throw null;
			}
		}

		public Font NodeFont
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

		public TreeNodeCollection Nodes
		{
			get
			{
				throw null;
			}
		}

		public TreeNode Parent
		{
			get
			{
				throw null;
			}
		}

		public TreeNode PrevNode
		{
			get
			{
				throw null;
			}
		}

		public TreeNode PrevVisibleNode
		{
			get
			{
				throw null;
			}
		}

		public int SelectedImageIndex
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

		public string SelectedImageKey
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

		public string StateImageKey
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

		public int StateImageIndex
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

		public object Tag
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

		public string Text
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

		public string ToolTipText
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

		public string Name
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

		public TreeView TreeView
		{
			get
			{
				throw null;
			}
		}

		public TreeNode()
		{
			throw null;
		}

		public TreeNode(string text)
		{
			throw null;
		}

		public TreeNode(string text, TreeNode[] children)
		{
			throw null;
		}

		public TreeNode(string text, int imageIndex, int selectedImageIndex)
		{
			throw null;
		}

		public TreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children)
		{
			throw null;
		}

		protected TreeNode(SerializationInfo serializationInfo, StreamingContext context)
		{
			throw null;
		}

		public static TreeNode FromHandle(TreeView tree, IntPtr handle)
		{
			throw null;
		}

		public void BeginEdit()
		{
			throw null;
		}

		public virtual object Clone()
		{
			throw null;
		}

		public void Collapse(bool ignoreChildren)
		{
			throw null;
		}

		public void Collapse()
		{
			throw null;
		}

		protected virtual void Deserialize(SerializationInfo serializationInfo, StreamingContext context)
		{
			throw null;
		}

		public void EndEdit(bool cancel)
		{
			throw null;
		}

		public void EnsureVisible()
		{
			throw null;
		}

		public void Expand()
		{
			throw null;
		}

		public void ExpandAll()
		{
			throw null;
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			throw null;
		}

		public void Remove()
		{
			throw null;
		}

		protected virtual void Serialize(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}

		public void Toggle()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}
	}
}
