// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// The node of the flow graph.
	/// </summary>
	public class Component_FlowGraphNode : Component
	{
		//!!!!Reference support?
		Vector2I position;
		//!!!!Reference support?
		string comment = "";

		//!!!!может это от стиля что-то. тогда тут нелогично
		int width = 11;

		Representation representation;
		bool representationNeedUpdate = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		ReferenceField<Component> _controlledObject;
		/// <summary>
		/// An object, represented by node.
		/// </summary>
		[Serialize]
		public Reference<Component> ControlledObject
		{
			get
			{
				if( _controlledObject.BeginGet() )
					ControlledObject = _controlledObject.Get( this );
				return _controlledObject.value;
			}
			set
			{
				if( _controlledObject.BeginSet( ref value ) )
				{
					try
					{
						ControlledObjectChanged?.Invoke( this );
						RepresentationNeedUpdate();
					}
					finally { _controlledObject.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ControlledObject"/> property value changes.</summary>
		public event Action<Component_FlowGraphNode> ControlledObjectChanged;


		//Mode
		public enum ModeEnum
		{
			Default,
			Inputs,
			Outputs,
		}
		ReferenceField<ModeEnum> _mode = ModeEnum.Default;
		/// <summary>
		/// The mode of the node.
		/// </summary>
		[Serialize]
		[DefaultValue( ModeEnum.Default )]
		public Reference<ModeEnum> Mode
		{
			get
			{
				if( _mode.BeginGet() )
					Mode = _mode.Get( this );
				return _mode.value;
			}
			set
			{
				if( _mode.BeginSet( ref value ) )
				{
					try
					{
						ModeChanged?.Invoke( this );
						RepresentationNeedUpdate();
					}
					finally { _mode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Component_FlowGraphNode> ModeChanged;


		//Style
		ReferenceField<Component_FlowGraphNodeStyle> _style;
		/// <summary>
		/// The style of the node.
		/// </summary>
		[Serialize]
		public Reference<Component_FlowGraphNodeStyle> Style
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
		public event Action<Component_FlowGraphNode> StyleChanged;


		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a cached data for visualization of <see cref="Component_FlowGraphNode"/> in the editor.
		/// </summary>
		public class Representation
		{
			Component_FlowGraphNode owner;
			List<Item> items = new List<Item>();
			int itemsHeight;
			ItemThisObject itemObject;
			Dictionary<Metadata.Property, ItemProperty> itemByProperty = new Dictionary<Metadata.Property, ItemProperty>();

			//int height;
			//!!!!
			//preview data. texture

			/////////////////////////////////////

			/// <summary>
			/// Represents a data item for <see cref="Representation"/>.
			/// </summary>
			public abstract class Item
			{
				Representation owner;
				int index;
				Connector input;
				Connector output;

				//

				protected Item( Representation owner )//, int index, bool input, bool output )
				{
					this.owner = owner;
					//this.index = index;
					//if( input )
					//	this.input = new Connector( this, true );
					//if( output )
					//	this.output = new Connector( this, false );
				}

				public Representation Owner
				{
					get { return owner; }
				}

				public int Index
				{
					get { return index; }
					set { index = value; }
				}

				public Connector Input
				{
					get { return input; }
					set { input = value; }
				}

				public Connector Output
				{
					get { return output; }
					set { output = value; }
				}

				//public bool IsInput
				//{
				//	get
				//	{
				//		if( isInput == null )
				//			isInput = owner.Inputs.Contains( this );
				//		return isInput.Value;
				//	}
				//}

				//public int Index
				//{
				//	get
				//	{
				//		if( index == null )
				//		{
				//			index = owner.Sockets.IndexOf( this );
				//			//if( IsInput )
				//			//	index = owner.Inputs.IndexOf( this );
				//			//else
				//			//	index = owner.Outputs.IndexOf( this );
				//		}
				//		return index.Value;
				//	}
				//}

				public abstract string DisplayName
				{
					get;
				}
			}

			/////////////////////////////////////

			/// <summary>
			/// Represents a connection slot of <see cref="Component_FlowGraphNode"/>.
			/// </summary>
			public class Connector
			{
				public Item item;
				public bool input;

				public Connector( Item item, bool input )
				{
					this.item = item;
					this.input = input;
				}
			}

			/////////////////////////////////////

			/// <summary>
			/// Represents a data item for <see cref="Representation"/> with info about this object.
			/// </summary>
			public class ItemThisObject : Item
			{
				public ItemThisObject( Representation owner )
					: base( owner )
				{
				}

				public override string ToString()
				{
					//!!!!так?
					return string.Format( "Reference for {0}", "Object" );
				}

				public override string DisplayName
				{
					get { return ""; }
				}
			}

			/////////////////////////////////////

			/// <summary>
			/// Represents a data item for <see cref="Representation"/> with info about a property.
			/// </summary>
			public class ItemProperty : Item
			{
				Metadata.Property property;
				string displayName;

				public ItemProperty( Representation owner, /*int index, bool input, bool output, */Metadata.Property property )
					: base( owner )//, index, input, output )
				{
					this.property = property;

					//calculate displayName
					var ar = property.GetCustomAttributes( typeof( DisplayNameAttribute ), true );
					if( ar.Length != 0 )
					{
						var attrib = (DisplayNameAttribute)ar[ 0 ];
						displayName = attrib.DisplayName;
					}
					if( displayName == null )
						displayName = property.Name;
				}

				public Metadata.Property Property
				{
					get { return property; }
				}

				public override string ToString()
				{
					//!!!!так?
					return string.Format( "Reference for {0}", property.Name );
				}

				public override string DisplayName
				{
					get { return displayName; }
				}
			}

			/////////////////////////////////////

			public Representation( Component_FlowGraphNode owner )
			{
				this.owner = owner;
			}

			[Browsable( false )]
			public Component_FlowGraphNode Owner
			{
				get { return owner; }
			}

			public List<Item> Items
			{
				get { return items; }
			}

			public int ItemsHeight
			{
				get { return itemsHeight; }
				set { itemsHeight = value; }
			}

			public ItemThisObject ItemObject
			{
				get { return itemObject; }
				set { itemObject = value; }
			}

			public Dictionary<Metadata.Property, ItemProperty> ItemByProperty
			{
				get { return itemByProperty; }
			}

			//!!!!
			public string Title
			{
				get
				{
					Component obj = owner.ControlledObject;
					if( obj != null )
					{
						//!!!!может иконку показывать

						string result = null;

						var iRepresentationData = obj as IFlowGraphRepresentationData;
						if( iRepresentationData != null )
						{
							var data = new FlowGraphRepresentationData();
							iRepresentationData.GetFlowGraphRepresentationData( data );
							result = data.NodeTitle;
							//result = iRepresentationData.FlowchartNodeTitle;
						}
						if( result == null )
							result = obj.ToString();

						return result;
					}
					else
						return "(null)";
				}
			}

			public Vector2I Size
			{
				get
				{
					int height = 1 + itemsHeight/*Items.Count*/ + GetTextsOffsetY();
					//int h = 1 + Math.Max( Inputs.Count, Outputs.Count ) + GetTextsOffsetY();

					//minimal height
					{
						Component obj = owner.ControlledObject;
						if( obj != null )
						{
							var iRepresentationData = obj as IFlowGraphRepresentationData;
							if( iRepresentationData != null )
							{
								var data = new FlowGraphRepresentationData();
								data.NodeHeight = height;
								iRepresentationData.GetFlowGraphRepresentationData( data );
								height = data.NodeHeight;
								//h = Math.Max( h, data.nodeMinHeight );
							}
						}
					}

					return new Vector2I( Owner.Width, height );

					//int height;
					//if( heightOverride != 0 )
					//	height = heightOverride;
					//else
					//	height = 1 + Math.Max( inputs.Length, outputs.Length ) + GetTextsOffsetY();
					//return new Vec2I( owner.width, height );
				}
			}

			public int GetTextsOffsetY()
			{
				//!!!!
				//if( texts != null )
				//{
				//	if( texts.Length == 1 && Inputs.Length != 0 && Inputs[ 0 ].Type == "Flow" && Inputs[ 0 ].Name == "Flow" )
				//		return 0;
				//	return texts.Length;
				//}
				return 0;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public abstract class Socket2
		//{
		//	CompiledRepresentation owner;

		//	//

		//	protected Socket( CompiledRepresentation owner )
		//	{
		//		this.owner = owner;
		//	}

		//	[Browsable( false )]
		//	public CompiledRepresentation Owner
		//	{
		//		get { return owner; }
		//	}

		//	public abstract string Name
		//	{
		//		get;
		//	}

		//	//!!!!!
		//	//public abstract string Type
		//	public abstract Metadata.TypeInfo Type
		//	{
		//		get;
		//	}

		//	public abstract bool ReadOnly
		//	{
		//		get;
		//	}

		//	public abstract object GetValue();

		//	public abstract void SetValue( object value );

		//	//!!!!
		//	//internal bool isInput;
		//	//internal int index;
		//	//string name = "";
		//	//string type = "";
		//	////null means display name is not specified
		//	//string displayName;
		//	////Types type = Types.Value;
		//	////ValueTypes valueType = ValueTypes.String;
		//	////bool allowInputValue;
		//	////string inputValueText;
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class SocketProperty2 : Socket
		//{
		//	Metadata.Property property;
		//	object obj;

		//	//

		//	public SocketProperty( CompiledRepresentation owner, Metadata.Property property, object obj )
		//		: base( owner )
		//	{
		//		this.property = property;
		//		this.obj = obj;
		//	}

		//	public Metadata.Property Property
		//	{
		//		get { return property; }
		//	}

		//	public object Obj
		//	{
		//		get { return obj; }
		//	}

		//	public override string Name
		//	{
		//		get { return property.Name; }
		//	}

		//	public override bool ReadOnly
		//	{
		//		get { return property.ReadOnly; }
		//	}

		//	public override Metadata.TypeInfo Type
		//	{
		//		get { return property.Type; }
		//	}

		//	public override object GetValue()
		//	{
		//		return property.GetValue( obj, null );
		//	}

		//	public override void SetValue( object value )
		//	{
		//		property.SetValue( obj, value, null );
		//	}

		//	public override string ToString()
		//	{
		//		return property.ToString();
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//public class Settings_Old
		//{
		//	Component_FlowchartNode owner;
		//	string title;
		//	Pin[] inputs;
		//	Pin[] outputs;
		//	string[] buttons;
		//	string[] texts;
		//	bool sizable;
		//	string error;
		//	int heightOverride;
		//	ColorValue bodyColor = new ColorValue( .3f, .3f, .3f );
		//	ColorValue commentColor = new ColorValue( 1, 1, 1 );

		//	//

		//	public Settings_Old( Component_FlowchartNode owner, string title, Pin[] inputs, Pin[] outputs, string[] buttons, string[] texts, string error )
		//	{
		//		this.owner = owner;
		//		this.title = title;
		//		this.inputs = inputs;
		//		this.outputs = outputs;
		//		this.buttons = buttons;
		//		this.texts = texts;
		//		this.error = error;
		//	}

		//	public Settings_Old( Component_FlowchartNode owner, string title, Pin[] inputs, Pin[] outputs, string[] buttons, string[] texts )
		//	{
		//		this.owner = owner;
		//		this.title = title;
		//		this.inputs = inputs;
		//		this.outputs = outputs;
		//		this.buttons = buttons;
		//		this.texts = texts;
		//		this.error = null;
		//	}

		//	public Settings_Old( Component_FlowchartNode owner, string title, Pin[] inputs, Pin[] outputs, string[] buttons )
		//	{
		//		this.owner = owner;
		//		this.title = title;
		//		this.inputs = inputs;
		//		this.outputs = outputs;
		//		this.buttons = buttons;
		//		this.texts = null;
		//		this.error = null;
		//	}

		//	public Component_FlowchartNode Owner
		//	{
		//		get { return owner; }
		//	}

		//	public string Title
		//	{
		//		get { return title; }
		//	}

		//	public Pin[] Inputs
		//	{
		//		get { return inputs; }
		//	}

		//	public Pin[] Outputs
		//	{
		//		get { return outputs; }
		//	}

		//	public string[] Buttons
		//	{
		//		get { return buttons; }
		//	}

		//	public string[] Texts
		//	{
		//		get { return texts; }
		//	}

		//	public bool Sizable
		//	{
		//		get { return sizable; }
		//		set { sizable = value; }
		//	}

		//	public string Error
		//	{
		//		get { return error; }
		//	}

		//	public int HeightOverride
		//	{
		//		get { return heightOverride; }
		//		set { heightOverride = value; }
		//	}

		//	public ColorValue BodyColor
		//	{
		//		get { return bodyColor; }
		//		set { bodyColor = value; }
		//	}

		//	public ColorValue CommentColor
		//	{
		//		get { return commentColor; }
		//		set { commentColor = value; }
		//	}

		//	public int GetInputPinByName( string name )
		//	{
		//		for( int n = 0; n < inputs.Length; n++ )
		//		{
		//			Pin pin = inputs[ n ];
		//			if( pin.Name == name )
		//				return n;
		//		}
		//		return -1;
		//	}

		//	public int GetOutputPinByName( string name )
		//	{
		//		for( int n = 0; n < outputs.Length; n++ )
		//		{
		//			Pin pin = outputs[ n ];
		//			if( pin.Name == name )
		//				return n;
		//		}
		//		return -1;
		//	}

		//	public Vec2I Size
		//	{
		//		get
		//		{
		//			int height;
		//			if( heightOverride != 0 )
		//				height = heightOverride;
		//			else
		//				height = 1 + Math.Max( inputs.Length, outputs.Length ) + GetTextsOffsetY();
		//			return new Vec2I( owner.width, height );
		//		}
		//	}

		//	public int GetTextsOffsetY()
		//	{
		//		if( texts != null )
		//		{
		//			if( texts.Length == 1 && Inputs.Length != 0 && Inputs[ 0 ].Type == "Flow" && Inputs[ 0 ].Name == "Flow" )
		//				return 0;
		//			return texts.Length;
		//		}
		//		return 0;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//public class Pin
		//{
		//	Component_FlowchartNode owner;
		//	internal bool isInput;
		//	internal int index;
		//	string name = "";
		//	string type = "";
		//	//null means display name is not specified
		//	string displayName;
		//	//Types type = Types.Value;
		//	//ValueTypes valueType = ValueTypes.String;
		//	//bool allowInputValue;
		//	//string inputValueText;

		//	//

		//	public Pin( Component_FlowchartNode owner, string name, string type, string displayName = null )
		//	{
		//		this.owner = owner;
		//		this.name = name;
		//		this.type = type;
		//		this.displayName = displayName;
		//	}

		//	public Component_FlowchartNode Owner
		//	{
		//		get { return owner; }
		//	}

		//	public bool IsInput
		//	{
		//		get { return isInput; }
		//	}

		//	public bool IsOutput
		//	{
		//		get { return !isInput; }
		//	}

		//	public int Index
		//	{
		//		get { return index; }
		//	}

		//	//public Connector( string name, Types type, ValueTypes valueType )
		//	//{
		//	//   this.name = name;
		//	//   this.type = type;
		//	//   this.valueType = valueType;
		//	//}

		//	//public Connector( string name, Types type )
		//	//{
		//	//   this.name = name;
		//	//   this.type = type;
		//	//}

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public string Type
		//	{
		//		get { return type; }
		//	}

		//	public string DisplayName
		//	{
		//		get { return displayName; }
		//	}

		//	//public Types Type
		//	//{
		//	//   get { return type; }
		//	//}

		//	//public ValueTypes ValueType
		//	//{
		//	//   get { return valueType; }
		//	//}

		//	//public bool AllowInputValue
		//	//{
		//	//   get { return allowInputValue; }
		//	//}

		//	//public string InputValueText
		//	//{
		//	//   get { return inputValueText; }
		//	//}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//public class Link
		//{
		//	//!!!!было
		//	//Component_FlowchartNode owner;

		//	string outputName = "";
		//	string inputNode = "";
		//	string inputName = "";

		//	//

		//	//!!!!было
		//	//public Link( Component_FlowchartNode owner )
		//	//{
		//	//	this.owner = owner;
		//	//}

		//	//!!!!было
		//	//[Browsable( false )]
		//	//public Component_FlowchartNode Owner
		//	//{
		//	//	get { return owner; }
		//	//}

		//	[Browsable( false )]
		//	[Serialize]
		//	public string OutputName
		//	{
		//		get { return outputName; }
		//		set { outputName = value; }
		//	}

		//	[Browsable( false )]
		//	[Serialize]
		//	public string InputNode
		//	{
		//		get { return inputNode; }
		//		set { inputNode = value; }
		//	}

		//	[Browsable( false )]
		//	[Serialize]
		//	public string InputName
		//	{
		//		get { return inputName; }
		//		set { inputName = value; }
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!
		//[Browsable( false )]
		//public string NodeTypeName
		//{
		//	get { return nodeTypeName; }
		//}

		//!!!!!
		[Browsable( false )]
		public Component_FlowGraph ParentFlowGraph
		{
			get { return Parent as Component_FlowGraph; }
		}

		//public void _Init( string nodeTypeName, Graph owner )
		//{
		//   this.nodeTypeName = nodeTypeName;
		//   this.owner = owner;
		//}

		//[Browsable( false )]//!!!!
		/// <summary>
		/// The location of the node.
		/// </summary>
		[Serialize]
		public Vector2I Position
		{
			get { return position; }
			set { position = value; }
		}

		//!!!!reference. why not? maybe want set for all nodes
		/// <summary>
		/// The width of the node.
		/// </summary>
		[Serialize]
		[Range( 4, 20 )]
		[DefaultValue( 11 )]
		public int Width
		{
			get { return width; }
			set
			{
				if( value < 1 )
					value = 1;
				if( width == value )
					return;
				width = value;
				RepresentationNeedUpdate();
			}
		}

		//[Category( "General" )]//!!!!везде категории
		//[DefaultValue( 11 )]
		//[Editor( typeof( IntegerValueEditor ), typeof( UITypeEditor ) )]
		//[EditorLimitsRangeI( 4, 50 )]
		//[Serialize]
		//public int Width
		//{
		//	get { return width; }
		//	set
		//	{
		//		if( width < 4 )
		//			width = 4;
		//		if( width == value )
		//			return;
		//		width = value;

		//		SettingsNeedUpdate();
		//	}
		//}

		//!!!!reference. why not?
		//!!!![Category( "General" )]
		/// <summary>
		/// User comment of the node.
		/// </summary>
		[DefaultValue( "" )]
		[Serialize]
		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		//!!!!удалить несуществующие связи после загрузки

		//public virtual bool Load( TextBlock block, out string error )
		//{
		//	TextBlock parameterBlock = block.FindChild( "parameters" );
		//	if( parameterBlock != null )
		//	{
		//		foreach( TextBlock.Attribute attribute in parameterBlock.Attributes )
		//			SetParameter( attribute.Name, attribute.Value );
		//	}

		//	error = "";
		//	return true;
		//}

		//public virtual void Save( TextBlock block )
		//{
		//	if( parameters.Count != 0 )
		//	{
		//		TextBlock parameterBlock = block.AddChild( "parameters" );
		//		foreach( KeyValuePair<string, string> pair in parameters )
		//			parameterBlock.SetAttribute( pair.Key, pair.Value );
		//	}
		//}

		//!!!!было
		//public virtual void SettingsClickButton( string button ) { }

		//!!!!было

		//public void AddLink( string thisNodeOutputPin, Component_FlowchartNode nodeTo, string pinTo )
		//{
		//	Link link = new Link();//!!!!было this );
		//	link.OutputName = thisNodeOutputPin;
		//	link.InputNode = nodeTo.Name;
		//	link.InputName = pinTo;
		//	links.Add( link );

		//	ParentFlowchart.backwardLinkCache = null;
		//}

		//public void DeleteLink( Link link )
		//{
		//	links.Remove( link );

		//	ParentFlowchart.backwardLinkCache = null;
		//}

		//!!!!было

		////!!!!как помечать о том, что надо обновить. может версия внутри дикшенери
		//[Browsable( false )]
		//public Dictionary<string, string> InternalParameters
		//{
		//	get { return internalParameters; }
		//}

		//!!!!было

		//public bool IsInternalParameterDefined( string name )
		//{
		//	if( string.IsNullOrEmpty( name ) )
		//		Log.Fatal( "Component_FlowchartNode: IsInternalParameterDefined: The name can't be empty." );

		//	return internalParameters.ContainsKey( name );
		//}

		//public string GetInternalParameter( string name )
		//{
		//	if( string.IsNullOrEmpty( name ) )
		//		Log.Fatal( "Component_FlowchartNode: GetInternalParameter: The name can't be empty." );

		//	string value;
		//	if( internalParameters.TryGetValue( name, out value ) )
		//		return value;
		//	return "";
		//}

		//public void SetInternalParameter( string name, string value )
		//{
		//	if( string.IsNullOrEmpty( name ) )
		//		Log.Fatal( "Component_FlowchartNode: SetInternalParameter: The name can't be empty." );
		//	internalParameters[ name ] = value;
		//}

		//public void ResetInternalParameter( string name )
		//{
		//	if( string.IsNullOrEmpty( name ) )
		//		Log.Fatal( "Component_FlowchartNode: ResetInternalParameter: The name can't be empty." );
		//	internalParameters.Remove( name );
		//}

		//public string GetPinInputValue( string pinName )
		//{
		//	return GetInternalParameter( "input_value_" + pinName );
		//}

		//public void SetPinInputValue( string pinName, string value )
		//{
		//	SetInternalParameter( "input_value_" + pinName, value );
		//	SettingsNeedUpdate();
		//}

		//public void ResetPinInputValue( string pinName )
		//{
		//	ResetInternalParameter( "input_value_" + pinName );
		//	SettingsNeedUpdate();
		//}

		//!!!!было
		//public List<Link> GetAllOutputLinks( string outputPinName )
		//{
		//	List<Link> list = new List<Link>();
		//	foreach( Link link in links )
		//	{
		//		if( link.OutputName == outputPinName )
		//			list.Add( link );
		//	}
		//	return list;
		//}

		//public Link GetFirstOutputLink( string outputPinName )
		//{
		//   foreach( Link link in links )
		//   {
		//      if( link.OutputName == outputPinName )
		//         return link;
		//   }
		//   return null;
		//}

		//!!!!было
		//public bool GetFirstValidOutputLink( string outputPinName, out Link link, out Component_FlowchartNode inputNode, out int inputPinIndex )
		//{
		//	foreach( Link link2 in links )
		//	{
		//		if( link2.OutputName == outputPinName )
		//		{
		//			var inputNode2 = ParentFlowchart.GetComponent( link2.InputNode ) as Component_FlowchartNode;
		//			if( inputNode2 != null )
		//			{
		//				int inputPinIndex2 = inputNode2.GetSettings().GetInputPinByName( link2.InputName );
		//				if( inputPinIndex2 != -1 )
		//				{
		//					link = link2;
		//					inputNode = inputNode2;
		//					inputPinIndex = inputPinIndex2;
		//					return true;
		//				}
		//			}
		//		}
		//	}

		//	link = null;
		//	inputNode = null;
		//	inputPinIndex = 0;
		//	return false;
		//}

		//!!!!
		//public virtual BlueprintExecutionManager.ExecutionResult ExecuteBegin( BlueprintExecutionManager.Flow flow, string inputPinName )
		//{
		//	Log.Fatal( "Component_FlowchartNode: Graph: ExecuteBegin: No implementation for \"{0}\".7", ToString() );
		//	return null;
		//}

		//!!!!
		//public virtual BlueprintExecutionManager.ExecutionResult ExecuteContinue( BlueprintExecutionManager.Flow flow )
		//{
		//	Log.Fatal( "Component_FlowchartNode: Graph: Node: ExecuteContinue: No implementation for \"{0}\".", ToString() );
		//	return null;
		//}

		//!!!!
		//public virtual object Calculate( BlueprintExecutionManager.Flow flow, string outputPinName )
		//{
		//	Log.Fatal( "Component_FlowchartNode: Graph: Node: Calculate: No implementation for \"{0}\".", ToString() );
		//	return null;
		//}

		//!!!!где вызывать?
		//	SettingsNeedUpdate();

		//protected override void OnClone( Metadata.CloneContext context, Component newObject )
		//{
		//	base.OnClone( context, newObject );
		//}
		//public virtual void OnClone( Node source )
		//{
		//	nodePosition = source.nodePosition;
		//	nodeComment = source.nodeComment;
		//	foreach( KeyValuePair<string, string> pair in source.parameters )
		//		parameters[ pair.Key ] = pair.Value;
		//	width = source.width;

		//	SettingsNeedUpdate();
		//}

		//!!!!было
		//public virtual bool IsFlowEntry()
		//{
		//	return false;
		//}

		protected virtual void RepresentationUpdate()
		{
			Component obj = ControlledObject;
			if( obj != null )
			{
				ModeEnum mode2 = Mode;

				//update Items

				var itemObjectOld = representation.ItemObject;
				var itemByPropertyOld = new Dictionary<Metadata.Property, Representation.ItemProperty>( representation.ItemByProperty );
				representation.Items.Clear();
				representation.ItemsHeight = 0;
				representation.ItemByProperty.Clear();

				int lastIndex = -1;
				bool previousIsFlowAndReadOnly = false;

				//object item
				{
					var item = itemObjectOld;
					if( item == null )
						item = new Representation.ItemThisObject( representation );

					lastIndex++;
					item.Index = lastIndex;

					item.Output = new Representation.Connector( item, false );

					representation.Items.Add( item );
					representation.ItemObject = item;
				}

				//property items
				foreach( var m in obj.MetadataGetMembers() )
				{
					var p = m as Metadata.Property;

					//!!!!унифицировать это всё. в том же гриде и COntentBrowser подобно
					if( p != null && p.Browsable && !p.Static )
					{
						bool add = true;

						//!!!!

						//skip properties of Component class
						var type = p.Owner as Metadata.NetTypeInfo;
						if( type != null && type.Type == typeof( Component ) )
							add = false;

						//FlowGraphBrowsable attribute
						if( add )
						{
							var ar = p.GetCustomAttributes( typeof( FlowGraphBrowsable ) );
							foreach( FlowGraphBrowsable attr in ar )
							{
								if( !attr.Browsable )
									add = false;
							}
						}

						//Type Settings filter
						if( add && !obj.TypeSettingsIsPublicMember( m ) )
							add = false;
						//if( add && !obj.TypeSettingsIsPublic() )
						//	add = false;
						//var baseComponentType = obj.BaseType as Metadata.ComponentTypeInfo;
						//if( baseComponentType != null && ComponentUtility.TypeSettingsCheckHideObject( baseComponentType.BasedOnObject, true, p ) )
						//	add = false;
						////if( baseComponentType != null && ComponentUtils.TypeSettingsCheckHideObject( obj, true, p ) )
						////	add = false;

						if( add )
						{
							bool input = false;
							bool output = false;

							bool isFlow = MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( p.TypeUnreferenced );
							if( isFlow )
							{
								//!!!!так? всегда на правую сторону
								if( p.ReadOnly )
								{
									if( mode2 == ModeEnum.Default || mode2 == ModeEnum.Inputs )
										input = true;
								}
								else
								{
									if( mode2 == ModeEnum.Default || mode2 == ModeEnum.Outputs )
										output = true;
								}
							}
							else
							{
								if( mode2 == ModeEnum.Default || mode2 == ModeEnum.Outputs )
									output = true;
								if( !p.ReadOnly )
								{
									if( mode2 == ModeEnum.Default || mode2 == ModeEnum.Inputs )
										input = true;
								}
							}

							if( input || output )
							{
								Representation.ItemProperty item = null;
								//check already exists
								itemByPropertyOld.TryGetValue( p, out item );

								if( item == null )
									item = new Representation.ItemProperty( representation, p );

								//merge index
								bool mergeIndexWithPreviousItem = false;
								if( representation.Items.Count != 0 )
								{
									//!!!!можно атрибут юзать

									if( isFlow && !p.ReadOnly && previousIsFlowAndReadOnly )
										mergeIndexWithPreviousItem = true;
								}

								if( !mergeIndexWithPreviousItem )
									lastIndex++;
								item.Index = lastIndex;
								//item.Index = representation.Items.Count;

								item.Input = input ? new Representation.Connector( item, true ) : null;
								item.Output = output ? new Representation.Connector( item, false ) : null;

								representation.Items.Add( item );
								representation.ItemByProperty[ p ] = item;

								previousIsFlowAndReadOnly = isFlow && p.ReadOnly;
							}
						}
					}
				}

				representation.ItemsHeight = lastIndex;// + 1;
			}
			else
			{
				//clean
				representation.Items.Clear();
				representation.ItemsHeight = 0;
				representation.ItemByProperty.Clear();
			}
		}

		public void RepresentationNeedUpdate()
		{
			representationNeedUpdate = true;
		}

		//protected virtual bool IsRepresentationNeedUpdate() { return compiledRepresentationNeedUpdate; }

		public Representation GetRepresentation()
		{
			//if( !compiledRepresentationNeedUpdate && IsRepresentationNeedUpdate() )
			//	compiledRepresentationNeedUpdate = true;

			if( representationNeedUpdate )
			{
				if( representation == null )
					representation = new Representation( this );
				RepresentationUpdate();

				//!!!!было
				//for( int n = 0; n < settingsCached.Inputs.Length; n++ )
				//{
				//	settingsCached.Inputs[ n ].index = n;
				//	settingsCached.Inputs[ n ].isInput = true;
				//}
				//for( int n = 0; n < settingsCached.Outputs.Length; n++ )
				//{
				//	settingsCached.Outputs[ n ].index = n;
				//	settingsCached.Outputs[ n ].isInput = false;
				//}

				representationNeedUpdate = false;
			}
			return representation;
		}

		public Component_FlowGraphNodeStyle GetResultStyle( Component_FlowGraph ownerFlowGraph )
		{
			Component_FlowGraphNodeStyle style = Style;
			if( style == null )
				style = ownerFlowGraph.NodesStyle;
			if( style == null )
			{
				style = Component_FlowGraphNodeStyle_Rectangle.Instance;
				//style = (Component_FlowchartNodeStyle)MetadataManager.GetTypeOfNetType( typeof( Component_FlowchartNodeStyle_Rectangle ) ).AutoCreatedInstance;
			}

			return style;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//old version compatibility
			if( block.AttributeExists( "NodePosition" ) )
				Position = Vector2I.Parse( block.GetAttribute( "NodePosition" ) );

			return true;
		}
	}
}
