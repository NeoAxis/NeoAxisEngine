// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component for compilation shader code.
	/// </summary>
	//[ResourceFileExtension( "shader" )]
	[NewObjectDefaultName( "Shader" )]
	//[EditorSettingsCell( typeof( Component_ShaderScript_SettingsCell ) )]
	[EditorDocumentWindow( "NeoAxis.Editor.Component_ShaderScript_DocumentWindow" )]
	//[EditorPreviewControl( typeof( Component_ShaderScript_PreviewControl ) )]
	public class Component_ShaderScript : Component//!!!!, IFlowGraphRepresentationData, IFlowExecutionComponent
	{
		//bool forceNeedUpdate = true;

		//compiled
		string compiledCode = "";
		CompiledShaderScript compiledScript;
		//List<Metadata.Member> compiledMembers = new List<Metadata.Member>();
		//Dictionary<string, Metadata.Member> compiledMemberBySignature = new Dictionary<string, Metadata.Member>();

		//One method mode
		//CompiledMethodData compiledOneMethod;
		List<MethodParameterPropertyImpl> properties = new List<MethodParameterPropertyImpl>();
		Dictionary<string, MethodParameterPropertyImpl> propertyBySignature = new Dictionary<string, MethodParameterPropertyImpl>();
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();
		List<MethodParameterPropertyImpl> propertyMethodParameters;
		//MethodParameterPropertyImpl propertyMethodReturnParameter;

		//!!!!
		////FlowGraph
		//Component_Image cachedFlowGraphNodeImage;
		//ScriptPrinter cachedFlowGraphNodeImageScriptPrinter;

		//!!!!
		bool temporarilyDisableUpdate;

		/////////////////////////////////////////

		//[DefaultValue( null )]
		//[Serialize]
		//public Reference<ReferenceValueType_Resource> File
		//{
		//	get { if( _file.BeginGet() ) File = _file.Get( this ); return _file.value; }
		//	set { if( _file.BeginSet( ref value ) ) { try { FileChanged?.Invoke( this ); } finally { _file.EndSet(); } } }
		//}
		//public event Action<Component_Script> FileChanged;
		//ReferenceField<ReferenceValueType_Resource> _file;

		//!!!!

		//public enum LanguageEnum
		//{
		//	CSharp,
		//}

		//[DefaultValue( LanguageEnum.CSharp )]
		//[Serialize]
		//public Reference<LanguageEnum> Language
		//{
		//	get { if( _language.BeginGet() ) Language = _language.Get( this ); return _language.value; }
		//	set { if( _language.BeginSet( ref value ) ) { try { LanguageChanged?.Invoke( this ); } finally { _language.EndSet(); } } }
		//}
		//public event Action<Component_Script> LanguageChanged;
		//ReferenceField<LanguageEnum> _language = LanguageEnum.CSharp;

		const string codeDefault = @"/*!public*/ vec4 Method(vec4 parameter1, vec4 parameter2)
{
	return parameter1 * parameter2;
}
";

		/// <summary>
		/// The code of the script.
		/// </summary>
		[DefaultValue( codeDefault )]
		//[DefaultValue( "" )]
		[Serialize]
		[FlowGraphBrowsable( false )]
		//!!!![Editor( typeof( HCItemScript ), typeof( object ) )]
		public Reference<string> Code
		{
			get { if( _code.BeginGet() ) Code = _code.Get( this ); return _code.value; }
			set
			{
				if( _code.BeginSet( ref value ) )
				{
					try
					{
						CodeChanged?.Invoke( this );
						//needUpdate = true;
					}
					finally { _code.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Code"/> property value changes.</summary>
		public event Action<Component_ShaderScript> CodeChanged;
		ReferenceField<string> _code = codeDefault;// "";

		//[Serialize]
		//[Browsable( false )]
		//[DefaultValue( true )]
		//public bool EditorAutoUpdate { get; set; } = true;

		//!!!!for preview texture
		//internal void RaiseCodeChangedEventAndSetNeedUpdate()
		//{
		//	CodeChanged?.Invoke( this );
		//	needUpdate = true;
		//}

		///// <summary>
		///// Whether the script is support flow control.
		///// </summary>
		//[DefaultValue( false )]
		//[Serialize]
		//[FlowGraphBrowsable( false )]
		//[Category( "Invoke Method" )]
		//public Reference<bool> FlowSupport
		//{
		//	get { if( _flowSupport.BeginGet() ) FlowSupport = _flowSupport.Get( this ); return _flowSupport.value; }
		//	set
		//	{
		//		if( _flowSupport.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				FlowSupportChanged?.Invoke( this );
		//				needUpdate = true;
		//			}
		//			finally { _flowSupport.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_ShaderScript> FlowSupportChanged;
		//ReferenceField<bool> _flowSupport;

		///// <summary>
		///// The input of the node.
		///// </summary>
		//[Category( "Invoke Method" )]
		//public FlowInput Entry
		//{
		//	get { return new FlowInput( this, nameof( Entry ) ); }
		//}

		///// <summary>
		///// The exit of the node.
		///// </summary>
		//[Serialize]
		//[Category( "Invoke Method" )]
		//public Reference<FlowInput> Exit
		//{
		//	get { if( _exit.BeginGet() ) Exit = _exit.Get( this ); return _exit.value; }
		//	set { if( _exit.BeginSet( ref value ) ) { try { ExitChanged?.Invoke( this ); } finally { _exit.EndSet(); } } }
		//}
		//public event Action<Component_ShaderScript> ExitChanged;
		//ReferenceField<FlowInput> _exit;

		/////////////////////////////////////////

		/// <summary>
		/// Represents a compiled data of <see cref="Component_ShaderScript"/>.
		/// </summary>
		public class CompiledShaderScript
		{
			public string Body;
			public string MethodName;
			public Parameter[] MethodParameters;

			//

			/// <summary>
			/// Represents a parameter data of <see cref="CompiledShaderScript"/>.
			/// </summary>
			public class Parameter
			{
				public string Name;
				public Metadata.TypeInfo Type;//!!!!structures support?
				public bool ReturnValue;
				public bool Output;
				public bool ByReference;
			}
		}

		/////////////////////////////////////////

		class MethodParameterPropertyImpl : Metadata.Property
		{
			string category;
			string displayName;
			bool referenceSupport;
			CompiledShaderScript.Parameter parameter;
			//Metadata.Parameter parameter;
			bool invoke;

			//

			public MethodParameterPropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, string category, string displayName, bool referenceSupport, CompiledShaderScript.Parameter parameter, bool invoke )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.category = category;
				this.displayName = displayName;
				this.referenceSupport = referenceSupport;
				this.parameter = parameter;
				this.invoke = invoke;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			public CompiledShaderScript.Parameter Parameter
			{
				get { return parameter; }
				set { parameter = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//!!!!может это в Metadata.Property?
				//Category
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}
				//DisplayName
				if( attributeType.IsAssignableFrom( typeof( DisplayNameAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( displayName ) )
						result.Add( new DisplayNameAttribute( displayName ) );
				}

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				//always obj == Owner
				var c = (Component_ShaderScript)obj;

				object value;
				if( c.propertyValues.TryGetValue( Signature, out value ) )
				{
					//update value for Reference type
					if( referenceSupport )
					{
						IReference iReference = value as IReference;
						if( iReference != null )
						{
							//!!!!try catch? где еще так

							//get value by reference
							//!!!!
							value = iReference.GetValue( obj );
							//value = iReference.GetValue( Owner );

							//!!!!?
							if( !ReadOnly )//!!!!new
							{
								//!!!!
								SetValue( obj, value, Indexers );
								//SetValue( Owner, value, Indexers );
							}
						}
					}
					result = value;
				}

				//check the type. result can contains value with another type after change the type of property.
				if( result != null && !Type.IsAssignableFrom( MetadataManager.MetadataGetType( result ) ) )
					result = null;
				if( result == null && Type.GetNetType().IsValueType )
					result = Type.InvokeInstance( null );

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				var c = (Component_ShaderScript)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		public Component_ShaderScript()
		{
		}

		static Dictionary<string, CompiledShaderScript> compiledScriptCache = new Dictionary<string, CompiledShaderScript>();

		static Type GetTypeByString( string value, bool allowVoid )
		{
			switch( value )
			{
			case "bool": return typeof( bool );
			case "int": return typeof( int );
			case "uint": return typeof( uint );
			case "float": return typeof( double );
			case "double": return typeof( double );
			case "vec2": return typeof( Vector2 );
			case "vec3": return typeof( Vector3 );
			case "vec4": return typeof( Vector4 );
			case "mat3": return typeof( Matrix3 );
			case "mat4": return typeof( Matrix4 );
			}

			if( allowVoid && value == "void" )
				return typeof( void );

			return null;
		}

		static CompiledShaderScript GetOrCompileScript( string code, out string error )
		{
			error = "";

			lock( compiledScriptCache )
			{
				if( compiledScriptCache.TryGetValue( code, out var script ) )
					return script;
			}

			var lines = code.Split( new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None );

			var line = "";
			foreach( var line2 in lines )
			{
				if( line2.Contains( @"/*!public*/" ) )
				{
					line = line2;
					break;
				}
			}

			if( string.IsNullOrEmpty( line ) )
			{
				error = "No method is marked public.";
				return null;
			}

			var result = new CompiledShaderScript();
			result.Body = code;

			try
			{
				// /*!public*/ vec3 Method(vec3 parameter1, vec3 parameter2)

				var str1 = line.Substring( line.IndexOf( @"/*!public*/" ) + @"/*!public*/".Length );

				int braceOpen = str1.IndexOf( '(' );
				int braceClose = str1.IndexOf( ')' );

				var str2 = str1.Substring( 0, braceOpen ).Trim();
				var str2Split = str2.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
				if( str2Split.Length != 2 )
					throw new Exception( "" );

				var parameters = new List<CompiledShaderScript.Parameter>();

				//get return type
				var returnTypeStr = str2Split[ 0 ];
				var returnType = GetTypeByString( returnTypeStr, true );
				if( returnType == null )
					throw new Exception( $"Unknown type \"{returnTypeStr}\"." );

				//get method name
				result.MethodName = str2Split[ 1 ];

				//parameters
				str2 = str1.Substring( braceOpen + 1, braceClose - braceOpen - 1 ).Trim();
				str2Split = str2.Split( new char[] { ',' } );
				foreach( var str3 in str2Split )
				{
					var str4 = str3.Trim();

					var str4Split = str4.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
					if( str4Split.Length < 2 || str4Split.Length > 3 )
						throw new Exception( "" );

					//out, ref
					bool isOut = false;
					if( str4Split.Length == 3 )
					{
						switch( str4Split[ 0 ] )
						{
						case "out": throw new Exception( "Output parameters are not supported." );
						//case "out": isOut = true; break;
						case "ref": throw new Exception( "Parameters passed by reference are not supported." );
						default: throw new Exception( "" );
						}
					}

					//get type
					var typeStr = str4Split.Length == 3 ? str4Split[ 1 ] : str4Split[ 0 ];
					var type = GetTypeByString( typeStr, false );
					if( type == null )
						throw new Exception( $"Unknown type \"{typeStr}\"." );

					//get name
					var name = str4Split.Length == 3 ? str4Split[ 2 ] : str4Split[ 1 ];

					//add parameter
					var p = new CompiledShaderScript.Parameter();
					p.Name = name;
					p.Type = MetadataManager.GetTypeOfNetType( type );
					p.Output = isOut;
					parameters.Add( p );
				}

				//add return parameter
				if( returnType != typeof( void ) )
				{
					var p = new CompiledShaderScript.Parameter();
					p.Name = "ReturnValue";
					p.Type = MetadataManager.GetTypeOfNetType( returnType );
					p.ReturnValue = true;
					parameters.Add( p );
				}

				result.MethodParameters = parameters.ToArray();
			}
			catch( Exception e )
			{
				error = "Can't parse method signature.\r\n" + "\"" + line + "\"" + "\r\n\r\n" + e.Message;
				return null;
			}

			//add to cache
			lock( compiledScriptCache )
			{
				if( compiledScriptCache.Count > 300 )
					compiledScriptCache.Clear();
				compiledScriptCache[ code ] = result;
			}

			return result;
		}

		//public void ForceNeedUpdate()
		//{
		//	forceNeedUpdate = true;
		//}

		void Update()
		{
			if( TemporarilyDisableUpdate )
				return;

			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			var newCode = Code.Value;

			//forceNeedUpdate = false;

			//no update when code is not changed
			if( newCode == compiledCode )
				return;
			////no update in the editor when no need update and auto update is disabled
			//if( !forceNeedUpdate && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor && !EditorAutoUpdate )
			//	return;

			//do update

			Clear();
			compiledCode = newCode;
			//forceNeedUpdate = false;

			if( newCode != "" )
			{
				compiledScript = GetOrCompileScript( compiledCode, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( $"Unable to compile shader script.\r\n\r\nComponent \"{GetPathFromRoot( true )}\".\r\n\r\n" + error );
					return;
				}

				//One method mode
				if( compiledScript != null )//&& compiledMembers.Count == 1 )
				{
					//compiledOneMethod = (CompiledMethodData)compiledMembers[ 0 ];
					OneMethod_AddProperties();
				}
			}
		}

		void OneMethod_AddProperties()
		{
			//parameters
			propertyMethodParameters = new List<MethodParameterPropertyImpl>();

			foreach( var parameter in compiledScript.MethodParameters )
			{
				var prop = OneMethod_AddPropertyOfParameter( parameter );

				properties.Add( prop );
				propertyBySignature[ prop.Signature ] = prop;

				////!!!!так? если несколько?
				//if( parameter.ReturnValue )
				//	propertyMethodReturnParameter = prop;
				//else
				propertyMethodParameters.Add( prop );
			}
		}

		MethodParameterPropertyImpl OneMethod_AddPropertyOfParameter( CompiledShaderScript.Parameter parameter )
		{
			////!!!!имя еще как-то фиксить?
			string name = null;
			if( parameter.ReturnValue )
				name = "ReturnValue";
			else
				name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );

			bool readOnly = parameter.Output || parameter.ReturnValue;

			//!!!!
			//var parameterType = GetOverrideParameterType( parameter, out bool error );
			//if( error )
			//{
			//	unableToInit = true;
			//	goto unableToInitLabel;
			//}
			//var parameterType = parameter.Type;

			Metadata.TypeInfo type;
			bool referenceSupport = !readOnly;
			if( referenceSupport )
			{
				Type unrefNetType = parameter.Type.GetNetType();
				var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
				type = MetadataManager.GetTypeOfNetType( refNetType );
			}
			else
				type = parameter.Type;

			bool invoke = parameter.ReturnValue || parameter.ByReference || parameter.Output;

			string namePrefix = "__parameter_";
			string displayName = TypeUtility.DisplayNameAddSpaces( name );

			var prop = new MethodParameterPropertyImpl( this, namePrefix + name, false, type, parameter.Type, new Metadata.Parameter[ 0 ], readOnly, "Method's Parameters", displayName, referenceSupport, parameter, invoke );
			prop.Description = "";
			if( !readOnly )
				prop.Serializable = SerializeType.Enable;

			return prop;
		}

		void Clear()
		{
			compiledCode = "";
			compiledScript = null;
			//compiledMembers.Clear();
			//compiledMemberBySignature.Clear();

			//One method mode
			//compiledOneMethod = null;
			properties.Clear();
			propertyBySignature.Clear();
			propertyMethodParameters = null;
			//propertyMethodReturnParameter = null;

			//!!!!
			//// clear cached texture for flow graph
			//cachedFlowGraphNodeImage?.Dispose();
			//cachedFlowGraphNodeImage = null;
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			//forceNeedUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//forceNeedUpdate = true;
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			//var ins = ComponentUtility.GetResourceInstanceByComponent( this );
			//var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;
			//if( !isResource )
			//{
			Update();

			//foreach( var member in compiledMembers )
			//	yield return member;
			foreach( var member in properties )
				yield return member;
			//}
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			//var ins = ComponentUtility.GetResourceInstanceByComponent( this );
			//var isResource = ins != null && ins.InstanceType == Resource.InstanceType.Resource;
			//if( !isResource )
			//{
			Update();

			//if( compiledMemberBySignature.TryGetValue( signature, out var m ) )
			//	return m;
			if( propertyBySignature.TryGetValue( signature, out var m2 ) )
				return m2;
			//}

			return base.OnMetadataGetMemberBySignature( signature );
		}

		/////////////////////////////////////////
		//Invoke Method

		//!!!!
		//public virtual void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		//{
		//	if( compiledOneMethod != null )
		//	{
		//		//!!!!может по другому
		//		data.NodeTitle = compiledOneMethod.ToString();

		//		if( cachedFlowGraphNodeImageScriptPrinter == null )
		//		{
		//			cachedFlowGraphNodeImageScriptPrinter = new ScriptPrinter() { BackgroundBrush = System.Windows.Media.Brushes.LightGray };
		//			//cachedFlowGraphNodeImageScriptPrinter = new ScriptPrinter();
		//			//cachedFlowGraphNodeImageScriptPrinter = new ScriptPrinter() { FontSize = 10, BackgroundBrush = System.Windows.Media.Brushes.Gray };
		//		}

		//		//!!!!правильнее знать какое точно разрешение. также скрипт может одновременно быть в нескольких разных нодах разных размеров.
		//		if( cachedFlowGraphNodeImage == null )
		//			cachedFlowGraphNodeImage = cachedFlowGraphNodeImageScriptPrinter.PrintToTexture( Code, new System.Windows.Size( 512, 256 ) );
		//		//if( cachedNodeTexture == null )
		//		//	cachedNodeTexture = scriptPrinter.PrintToTexture( Code, new System.Windows.Size( 64, 64 ) );

		//		data.NodeImage = cachedFlowGraphNodeImage;
		//		data.NodeImageView = FlowGraphRepresentationData.NodeImageViewEnum.WideScaled;
		//		data.NodeHeight += 6;

		//		if( FlowSupport )
		//			data.NodeContentType = FlowGraphNodeContentType.Flow;
		//	}
		//}

		[Browsable( false )]
		public CompiledShaderScript CompiledScript
		{
			get { return compiledScript; }
		}

		[Browsable( false )]
		public bool TemporarilyDisableUpdate
		{
			get { return temporarilyDisableUpdate; }
			set { temporarilyDisableUpdate = value; }
		}
	}
}
