// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component for creation of visual workflow.
	/// </summary>
	[ResourceFileExtension( "flowgraph" )]
	[EditorDocumentWindow( typeof( Component_FlowGraph_DocumentWindow ) )]
	public class Component_FlowGraph : Component
	{
		//!!!!!

		//public enum GraphTypes
		//{
		//	EventGraph,
		//	Function,
		//}
		//GraphTypes graphType = GraphTypes.EventGraph;

		//!!!!!было
		//string description = "";

		//чилдами же
		//List<Component_FlowchartNode> nodes = new List<Component_FlowchartNode>();

		//!!!!internal
		internal Dictionary<Component_FlowGraphNode, List<BackwardLinkCacheItem>> backwardLinkCache;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!
		//public class PropertyTypeAttribute : Attribute
		//{
		//	PropertyType type;

		//	public PropertyTypeAttribute( PropertyType type )
		//	{
		//		this.type = type;
		//	}

		//	public PropertyType Type
		//	{
		//		get { return type; }
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		public enum PropertyType
		{
			//!!!!None/Hide?
			Usual,
			Flow,
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Specialization
		ReferenceField<Component_FlowGraphSpecialization> _specialization;
		/// <summary>
		/// The specialization of the flow graph. Specialization affects graph workflow in editor.
		/// </summary>
		[Serialize]
		public Reference<Component_FlowGraphSpecialization> Specialization
		{
			get
			{
				if( _specialization.BeginGet() )
					Specialization = _specialization.Get( this );
				return _specialization.value;
			}
			set
			{
				if( _specialization.BeginSet( ref value ) )
				{
					try { SpecializationChanged?.Invoke( this ); }
					finally { _specialization.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Specialization"/> property value changes.</summary>
		public event Action<Component_FlowGraph> SpecializationChanged;

		//!!!!можно было бы default value указывать вот так: "Component_FlowchartStyle_Default.Instance"
		//Style
		ReferenceField<Component_FlowGraphStyle> _style;
		/// <summary>
		/// The graphical style of the flow graph.
		/// </summary>
		[Serialize]
		public Reference<Component_FlowGraphStyle> Style
		{
			get
			{
				if( _style.BeginGet() )
					Style = _style.Get( this );
				return _style.value;
			}
			set
			{
				if( _style.BeginSet( ref value ) )
				{
					try { StyleChanged?.Invoke( this ); }
					finally { _style.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Style"/> property value changes.</summary>
		public event Action<Component_FlowGraph> StyleChanged;

		//!!!!можно было бы default value указывать вот так: "Component_FlowchartNodeStyle_Rectangle.Instance"
		//NodesStyle
		ReferenceField<Component_FlowGraphNodeStyle> _nodesStyle;
		/// <summary>
		/// The graphical style of the flow graph nodes.
		/// </summary>
		[Serialize]
		public Reference<Component_FlowGraphNodeStyle> NodesStyle
		{
			get
			{
				if( _nodesStyle.BeginGet() )
					NodesStyle = _nodesStyle.Get( this );
				return _nodesStyle.value;
			}
			set
			{
				if( _nodesStyle.BeginSet( ref value ) )
				{
					try { NodesStyleChanged?.Invoke( this ); }
					finally { _nodesStyle.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="NodesStyle"/> property value changes.</summary>
		public event Action<Component_FlowGraph> NodesStyleChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!internal
		internal class BackwardLinkCacheItem
		{
			public string inputPinName;
			public Component_FlowGraphNode outputNode;
			public string outputPinName;
		}

		//!!!!
		//[Browsable( false )]
		//[DefaultValue( GraphTypes.EventGraph )]
		//public GraphTypes GraphType
		//{
		//	get { return graphType; }
		//	set { graphType = value; }
		//}

		//!!!!было. хотя такое можно и для компоненты?
		//[DefaultValue( "" )]
		//[Category( "General" )]
		//[Serialize]
		//public string Description
		//{
		//	get { return description; }
		//	set { description = value; }
		//}

		[Browsable( false )]
		[Serialize]
		[DefaultValue( typeof( Vector2 ), "0 0" )]
		public Vector2 EditorScrollPosition { get; set; }

		[Browsable( false )]
		[Serialize]
		[DefaultValue( 8 )]
		public int EditorZoomIndex { get; set; } = 8;

		//[Browsable( false )]
		//public List<Component_FlowchartNode> Nodes
		//{
		//	get { return nodes; }
		//}

		//public virtual bool Load( TextBlock block, out string error )
		//{
		//	foreach( TextBlock child in block.Children )
		//	{
		//		switch( child.Name )
		//		{
		//		case "node":
		//			{
		//				string typeName = child.GetAttribute( "type" );
		//				if( string.IsNullOrEmpty( typeName ) )
		//				{
		//					//!!!!!
		//					error = string.Format( "\"type\" is not specified." );
		//					return false;
		//				}

		//				//!!!!

		//				BlueprintManager.NodeTypeItem item = BlueprintManager.GetNodeTypeByName( typeName );
		//				if( item == null )
		//				{
		//					//!!!!!error
		//					error = string.Format( "Node type with name \"{0}\" is not registered.", typeName );
		//					return false;
		//				}

		//				Node node = BlueprintManager.CreateNodeClassInstance( item, this );
		//				if( !node.Load( child, out error ) )
		//					return false;

		//				nodes.Add( node );
		//			}
		//			break;
		//		}
		//	}

		//	error = "";
		//	return true;
		//}

		//public virtual void Save( TextBlock block )
		//{
		//	foreach( Component_FlowchartNode node in nodes )
		//	{
		//		TextBlock nodeBlock = block.AddChild( "node" );
		//		nodeBlock.SetAttribute( "type", node.NodeTypeName );//GetType().FullName );
		//		node.Save( nodeBlock );
		//	}
		//}

		//!!!!!было, не надо уже
		//public Component_FlowchartNode GetNodeByName( string name )
		//{
		//	//!!!!slowly
		//	foreach( Component_FlowchartNode node in nodes )
		//	{
		//		if( node.Name == name )
		//			return node;
		//	}
		//	return null;
		//}

		//!!!!было
		//public Component_FlowchartNode CreateNode( BlueprintManager.NodeTypeItem type )
		//{
		//	Blueprint.Graph.Node node = BlueprintManager.CreateNodeClassInstance( type, this );

		//	string name = "";
		//	for( int n = 1; ; n++ )
		//	{
		//		name = string.Format( "Node{0}", n );
		//		if( GetNodeByName( name ) == null )
		//			break;
		//	}
		//	node.Name = name;

		//	Nodes.Add( node );

		//	return node;
		//}

		//!!!!было
		//public void DeleteNode( Component_FlowchartNode node )
		//{
		//	nodes.Remove( node );

		//	backwardLinkCache = null;
		//}

		void GenerateBackwardLinkCache()
		{
			//!!!!!
			//backwardLinkCache = new Dictionary<Component_FlowchartNode, List<BackwardLinkCacheItem>>();
			//foreach( Component_FlowchartNode node in GetComponents<Component_FlowchartNode>( false ) )
			//{
			//	foreach( Component_FlowchartNode.Link link in node.Links )
			//	{
			//		if( node.GetSettings().GetOutputPinByName( link.OutputName ) != -1 )
			//		{
			//			Component_FlowchartNode inputNode = GetComponent( link.InputNode ) as Component_FlowchartNode;
			//			//Component_FlowchartNode inputNode = GetNodeByName( link.InputNode );

			//			if( inputNode != null && inputNode.GetSettings().GetInputPinByName( link.InputName ) != -1 )
			//			{
			//				List<BackwardLinkCacheItem> list;
			//				if( !backwardLinkCache.TryGetValue( inputNode, out list ) )
			//				{
			//					list = new List<BackwardLinkCacheItem>();
			//					backwardLinkCache.Add( inputNode, list );
			//				}

			//				BackwardLinkCacheItem item = new BackwardLinkCacheItem();
			//				item.inputPinName = link.InputName;
			//				item.outputNode = node;
			//				item.outputPinName = link.OutputName;
			//				list.Add( item );
			//			}
			//		}
			//	}
			//}
		}

		/// <summary>
		/// Represents an item for <see cref="GetAllLinksFromInputPin(Component_FlowGraphNode, string)"/> method.
		/// </summary>
		public class GetAllLinksFromInputPin_Result
		{
			internal Component_FlowGraphNode outputNode;
			internal string outputPinName;

			public Component_FlowGraphNode OutputNode
			{
				get { return outputNode; }
			}
			public string OutputPinName
			{
				get { return outputPinName; }
			}
		}

		public List<GetAllLinksFromInputPin_Result> GetAllLinksFromInputPin( Component_FlowGraphNode inputNode, string inputPinName )
		{
			List<GetAllLinksFromInputPin_Result> resultList = new List<GetAllLinksFromInputPin_Result>();

			//generate cache
			if( backwardLinkCache == null )
				GenerateBackwardLinkCache();

			//get from the cache
			List<BackwardLinkCacheItem> list;
			if( backwardLinkCache.TryGetValue( inputNode, out list ) )
			{
				foreach( BackwardLinkCacheItem item in list )
				{
					if( item.inputPinName == inputPinName )
					{
						GetAllLinksFromInputPin_Result result = new GetAllLinksFromInputPin_Result();
						result.outputNode = item.outputNode;
						result.outputPinName = item.outputPinName;
						resultList.Add( result );
					}
				}
			}

			return resultList;
		}

		public bool GetFirstLinkFromInputPin( Component_FlowGraphNode inputNode, string inputPinName, out Component_FlowGraphNode outputNode,
			out string outputPinName )
		{
			//generate cache
			if( backwardLinkCache == null )
				GenerateBackwardLinkCache();

			//get from the cache
			List<BackwardLinkCacheItem> list;
			if( backwardLinkCache.TryGetValue( inputNode, out list ) )
			{
				foreach( BackwardLinkCacheItem item in list )
				{
					if( item.inputPinName == inputPinName )
					{
						outputNode = item.outputNode;
						outputPinName = item.outputPinName;
						return true;
					}
				}
			}

			outputNode = null;
			outputPinName = null;
			return false;
		}

		public void NeedUpdateBackwardLinkCache()
		{
			backwardLinkCache = null;
		}
	}
}
