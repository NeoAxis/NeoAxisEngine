// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The component for adding C# scripts.
	/// </summary>
	[ResourceFileExtension( "csscript" )]
	[NewObjectDefaultName( "C# Script" )]
	[SettingsCell( typeof( CSharpScriptSettingsCell ) )]
	[EditorControl( "NeoAxis.Editor.CSharpScriptEditor" )]
	[Preview( "NeoAxis.Editor.CSharpScriptPreview" )]
	public class CSharpScript : Component, IFlowGraphRepresentationData, IFlowExecutionComponent
	{
		////!!!!is not used
		//bool needUpdate = true;

		//compiled
		string compiledCodeOriginal = string.Empty;
		string compiledCode = string.Empty;
		CompiledScript compiledScript;
		List<Metadata.Member> compiledMembers = new List<Metadata.Member>();
		Dictionary<string, Metadata.Member> compiledMemberBySignature = new Dictionary<string, Metadata.Member>();
		object scriptInstance;

		//One method mode
		MethodImpl compiledOneMethod;
		List<MethodParameterPropertyImpl> properties = new List<MethodParameterPropertyImpl>();
		Dictionary<string, MethodParameterPropertyImpl> propertyBySignature = new Dictionary<string, MethodParameterPropertyImpl>();
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();
		List<MethodParameterPropertyImpl> propertyMethodParameters;
		MethodParameterPropertyImpl propertyMethodReturnParameter;

		//FlowGraph
		ImageComponent cachedFlowGraphNodeImage;
		EditorAssemblyInterface.IScriptPrinter cachedFlowGraphNodeImageScriptPrinter;

		//!!!!
		bool disableUpdate;

		/////////////////////////////////////////

		//[DefaultValue( null )]
		//[Serialize]
		//public Reference<ReferenceValueType_Resource> File
		//{
		//	get { if( _file.BeginGet() ) File = _file.Get( this ); return _file.value; }
		//	set { if( _file.BeginSet( ref value ) ) { try { FileChanged?.Invoke( this ); } finally { _file.EndSet(); } } }
		//}
		//public event Action<Script> FileChanged;
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
		//public event Action<Script> LanguageChanged;
		//ReferenceField<LanguageEnum> _language = LanguageEnum.CSharp;

		const string codeDefault = @"int Method( int a, int b )
{
	return a + b;
}
";

		/// <summary>
		/// The code of the script.
		/// </summary>
		[DefaultValue( codeDefault )]
		//[DefaultValue( "" )]
		[Serialize]
		[FlowGraphBrowsable( false )]
		[Editor( "NeoAxis.Editor.HCItemScript", typeof( object ) )]//[Editor( typeof( HCItemScript ), typeof( object ) )]
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
		public event Action<CSharpScript> CodeChanged;
		ReferenceField<string> _code = codeDefault;// "";

		internal void RaiseCodeChangedEvent()
		//internal void RaiseCodeChangedEventAndSetNeedUpdate()
		{
			CodeChanged?.Invoke( this );
			//needUpdate = true;
		}

		//!!!!
		//[DefaultValue( false )]
		//[Serialize]
		//public Reference<bool> AddMembersToParent
		//{
		//	get { if( _addMembersToParent.BeginGet() ) AddMembersToParent = _addMembersToParent.Get( this ); return _addMembersToParent.value; }
		//	set { if( _addMembersToParent.BeginSet( ref value ) ) { try { AddMembersToParentChanged?.Invoke( this ); } finally { _addMembersToParent.EndSet(); } } }
		//}
		//public event Action<Script> AddMembersToParentChanged;
		//ReferenceField<bool> _addMembersToParent = false;

		/// <summary>
		/// Whether the script is support flow control.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[FlowGraphBrowsable( false )]
		[Category( "Invoke Method" )]
		public Reference<bool> FlowSupport
		{
			get { if( _flowSupport.BeginGet() ) FlowSupport = _flowSupport.Get( this ); return _flowSupport.value; }
			set
			{
				if( _flowSupport.BeginSet( ref value ) )
				{
					try
					{
						FlowSupportChanged?.Invoke( this );
						//needUpdate = true;
					}
					finally { _flowSupport.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="FlowSupport"/> property value changes.</summary>
		public event Action<CSharpScript> FlowSupportChanged;
		ReferenceField<bool> _flowSupport;

		/// <summary>
		/// The input of the node.
		/// </summary>
		[Category( "Invoke Method" )]
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The exit of the node.
		/// </summary>
		[Serialize]
		[Category( "Invoke Method" )]
		public Reference<FlowInput> Exit
		{
			get { if( _exit.BeginGet() ) Exit = _exit.Get( this ); return _exit.value; }
			set { if( _exit.BeginSet( ref value ) ) { try { ExitChanged?.Invoke( this ); } finally { _exit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<CSharpScript> ExitChanged;
		ReferenceField<FlowInput> _exit;

		/////////////////////////////////////////

		/// <summary>
		/// Scripting context variables container.
		/// </summary>
		public class Context
		{
			public CSharpScript Owner { get; set; }
		}

		/////////////////////////////////////////

		internal class PropertyImpl : Metadata.Property
		{
			CompiledScript compiledScript;
			MemberInfo compiledMember;

			//

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, CompiledScript compiledScript, MemberInfo compiledMember )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.compiledScript = compiledScript;
				this.compiledMember = compiledMember;
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				//!!!!?
				return Array.Empty<object>();
			}

			public override object GetValue( object obj, object[] index )
			{
				var script = (CSharpScript)Owner;

				//// set updated context vars before invoke to compiled script.
				//foreach( var contextVar in script.ContextVars )
				//{
				//	compiledScript.SetField( contextVar.Name, contextVar.GetValue( script.Context ), out var error1 );
				//	if( !string.IsNullOrEmpty( error1 ) )
				//		Log.Error( $"Unable to set context variable \"{script.GetPathFromRoot( true )}\". " + error1 );
				//}

				object result = null;

				var field = compiledMember as FieldInfo;
				if( field != null )
				{
					try
					{
						//!!!!
						result = field.GetValue( !field.IsStatic ? script.ScriptInstance : null );
						//result = field.GetValue( !field.IsStatic ? compiledScript.Instance : null );
					}
					catch( Exception e )
					{
						Log.Error( $"Unable to get field value of object \"{script.GetPathFromRoot( true )}\". " + e.Message );
					}
				}

				var property = compiledMember as PropertyInfo;
				if( property != null )
				{
					try
					{
						result = property.GetValue( !ReflectionUtility.PropertyIsStatic( property ) ? script.ScriptInstance : null, index );
						//result = property.GetValue( !property.GetAccessors()[ 0 ].IsStatic ? compiledScript.Instance : null, index );
					}
					catch( Exception e )
					{
						Log.Error( $"Unable to get property value of object \"{script.GetPathFromRoot( true )}\". " + e.Message );
					}
				}

				return result;
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				var script = (CSharpScript)Owner;

				//// set updated context vars before invoke to compiled script.
				//foreach( var contextVar in script.ContextVars )
				//{
				//	compiledScript.SetField( contextVar.Name, contextVar.GetValue( script.Context ), out var error1 );
				//	if( !string.IsNullOrEmpty( error1 ) )
				//		Log.Error( $"Unable to set context variable \"{script.GetPathFromRoot( true )}\". " + error1 );
				//}

				var field = compiledMember as FieldInfo;
				if( field != null )
				{
					try
					{
						//!!!!
						field.SetValue( !field.IsStatic ? script.ScriptInstance : null, value );
						//field.SetValue( !field.IsStatic ? compiledScript.Instance : null, value );
					}
					catch( Exception e )
					{
						Log.Error( $"Unable to set field value of object \"{script.GetPathFromRoot( true )}\". " + e.Message );
					}
				}

				var property = compiledMember as PropertyInfo;
				if( property != null )
				{
					try
					{
						property.SetValue( !ReflectionUtility.PropertyIsStatic( property ) ? script.ScriptInstance : null, value, index );
						//property.SetValue( !property.GetAccessors()[ 0 ].IsStatic ? compiledScript.Instance : null, value, index );
					}
					catch( Exception e )
					{
						Log.Error( $"Unable to set property value of object \"{script.GetPathFromRoot( true )}\". " + e.Message );
					}
				}
			}

			[Browsable( false )]
			public MemberInfo CompiledMember
			{
				get { return compiledMember; }
			}
		}

		/////////////////////////////////////////

		internal class MethodImpl : Metadata.Method
		{
			CompiledScript compiledScript;
			MethodInfo compiledMethod;

			//

			public MethodImpl( object owner, string name, bool isStatic,
				Metadata.Parameter[] parameters, bool isConstructor, bool isOperator, CompiledScript compiledScript, MethodInfo compiledMethod )
				: base( owner, name, isStatic, parameters, isConstructor, isOperator )
			{
				this.compiledScript = compiledScript;
				this.compiledMethod = compiledMethod;
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				//!!!!?
				return Array.Empty<object>();
			}

			public override object Invoke( object obj, object[] parameters )
			{
				if( compiledMethod != null )
				{
					var script = (CSharpScript)Owner;

					//// set updated context vars before invoke to compiled script.
					//foreach( var contextVar in script.ContextVars )
					//{
					//	compiledScript.SetField( contextVar.Name, contextVar.GetValue( script.Context ), out var error1 );
					//	if( !string.IsNullOrEmpty( error1 ) )
					//		Log.Error( $"Unable to set context variable \"{script.GetPathFromRoot( true )}\". " + error1 );
					//}

					object result = null;

					try
					{
						//!!!!
						result = compiledMethod.Invoke( !compiledMethod.IsStatic ? script.ScriptInstance : null, parameters );
						//result = compiledMethod.Invoke( !compiledMethod.IsStatic ? compiledScript.Instance : null, parameters );
					}
					catch( Exception e )
					{
						Log.Error( $"Unable to invoke method of object \"{script.GetPathFromRoot( true )}\". " + e.Message );
					}
					//var result = compiledScript.InvokeMethod( compiledMethod, parameters, out var error );
					//if( !string.IsNullOrEmpty( error ) )
					//	Log.Error( $"Unable to invoke method of object \'{script.GetPathFromRoot( true )}\'.\r\n\r\n" + error );

					return result;
				}

				return null;
			}

			[Browsable( false )]
			public MethodInfo CompiledMethod
			{
				get { return compiledMethod; }
			}
		}

		/////////////////////////////////////////

		class MethodParameterPropertyImpl : Metadata.Property
		{
			string category;
			string displayName;
			bool referenceSupport;
			Metadata.Parameter parameter;
			bool invoke;

			//

			public MethodParameterPropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, string category, string displayName, bool referenceSupport, Metadata.Parameter parameter, bool invoke )
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

			public Metadata.Parameter Parameter
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

			public object GetValue( object obj, object[] index, bool allowInvoke )
			{
				object result = null;

				//always obj == Owner
				var c = (CSharpScript)obj;

				//!!!!так?
				if( allowInvoke && invoke && !c.FlowSupport )
					c.Invoke();

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

			public override object GetValue( object obj, object[] index )
			{
				return GetValue( obj, index, true );
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				var c = (CSharpScript)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		internal class ContextVar
		{
			MemberInfo source;

			public ContextVar( MemberInfo source )
			{
				this.source = source;
			}

			internal string Name
			{
				get { return source.Name; }
			}

			internal object GetValue( object context )
			{
				return source.GetValue( context );
			}
		}

		/////////////////////////////////////////

		//// context vars container.
		//object context;

		//internal object Context
		//{
		//	get { return context; }
		//	private set
		//	{
		//		context = value;

		//		// update vars:
		//		ContextVars.Clear();
		//		if( context != null )
		//		{
		//			foreach( var member in ScriptContextHelper.GetContextMembers( context.GetType() ) )
		//				ContextVars.Add( new ContextVar( member ) );
		//		}
		//	}
		//}

		//internal List<ContextVar> ContextVars { get; set; } = new List<ContextVar>();

		public CSharpScript()
		{
			//Context = new CSharpScriptContext() { Owner = this };
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				//case nameof( Code ):
				//	case nameof( Language ):
				//	if( !string.IsNullOrEmpty( File.GetByReference ) )
				//		skip = true;
				//	break;

				case nameof( FlowSupport ):
					if( compiledOneMethod == null )
						skip = true;
					break;

				case nameof( Entry ):
				case nameof( Exit ):
					if( !FlowSupport )
						skip = true;
					if( compiledOneMethod == null )
						skip = true;
					break;
				}
			}
		}

		//!!!!new
		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			Update();
		}

		public void Update()
		{
			if( DisableUpdate )
				return;

			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			var newCodeOriginal = Code.Value;

			if( ReferenceEquals( newCodeOriginal, compiledCodeOriginal ) || newCodeOriginal == compiledCodeOriginal )
				return;

			////remove '\r' to make equal string on any devices
			//var newCode = newCodeOriginal.Replace( "\r", "" );

			////needUpdate = false;
			//if( newCodeOriginal == compiledCodeOriginal )
			//	return;
			////check for updates
			//if( !needUpdate && newCode != compiledCode )
			//	needUpdate = true;
			////nothing to update
			//if( !needUpdate )
			//	return;

			//do update

			Clear();

			compiledCodeOriginal = newCodeOriginal;

			//remove '\r' to make equal string on any devices
			compiledCode = compiledCodeOriginal.Replace( "\r", "" );
			//needUpdate = false;

			if( !string.IsNullOrEmpty( compiledCode ) )
			{
				compiledScript = ScriptingCSharpEngine.GetOrCompileScript( compiledCode, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( $"Unable to compile script.\r\n\r\nComponent \"{GetPathFromRoot( true )}\".\r\n\r\n" + error );
					return;
				}

				CreateScriptInstance();

				//fields
				foreach( var netField in compiledScript.Fields )
				{
					if( netField.IsPublic && netField.Name != "Owner" )
					{
						var type = MetadataManager.GetTypeOfNetType( netField.FieldType );
						var member = new PropertyImpl( this, netField.Name, netField.IsStatic, type, type, Array.Empty<Metadata.Parameter>(), false, compiledScript, netField );

						compiledMembers.Add( member );
						compiledMemberBySignature[ member.Signature ] = member;
					}
				}

				//properties
				foreach( var netProperty in compiledScript.Properties )
				{
					if( netProperty.GetMethod != null && netProperty.GetMethod.IsPublic )
					{
						var type = MetadataManager.GetTypeOfNetType( netProperty.PropertyType );
						var unrefType = ReferenceUtility.GetUnreferencedType( type );

						var netParameters = netProperty.GetIndexParameters();

						var indexers = new List<Metadata.Parameter>();
						for( int n = 0; n < netParameters.Length; n++ )
						{
							var netParam = netParameters[ n ];

							var netType = netParam.ParameterType;
							bool isByReference = netType.IsByRef;
							if( isByReference )
								netType = netType.GetElementType();

							var type2 = MetadataManager.GetTypeOfNetType( netType );

							var p = new Metadata.Parameter( netParam.Name, type2, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval,
								netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
							indexers.Add( p );
						}

						var member = new PropertyImpl( this, netProperty.Name, ReflectionUtility.PropertyIsStatic( netProperty ), type, unrefType, indexers.Count != 0 ? indexers.ToArray() : Array.Empty<Metadata.Parameter>(), !netProperty.CanWrite, compiledScript, netProperty );

						compiledMembers.Add( member );
						compiledMemberBySignature[ member.Signature ] = member;
					}
				}

				//methods
				foreach( var netMethod in compiledScript.Methods )
				{
					if( netMethod.IsSpecialName )
					{
						if( netMethod.Name.Length > 4 && ( netMethod.Name.Substring( 0, 4 ) == "get_" || netMethod.Name.Substring( 0, 4 ) == "set_" ) )
							continue;
						if( netMethod.Name.Length > 4 && netMethod.Name.Substring( 0, 4 ) == "add_" )
							continue;
						if( netMethod.Name.Length > 7 && netMethod.Name.Substring( 0, 7 ) == "remove_" )
							continue;
					}

					if( netMethod.GetBaseDefinition() != netMethod )
						continue;

					var netParams = netMethod.GetParameters();

					var parameters = new List<Metadata.Parameter>();
					for( int n = 0; n < netParams.Length; n++ )
					{
						var netParam = netParams[ n ];

						var netType = netParam.ParameterType;
						bool isByReference = netType.IsByRef;
						if( isByReference )
							netType = netType.GetElementType();

						var type = MetadataManager.GetTypeOfNetType( netType );

						var p = new Metadata.Parameter( netParam.Name, type, isByReference, netParam.IsIn, netParam.IsOut, netParam.IsRetval,
							netParam.IsOptional, netParam.HasDefaultValue, netParam.DefaultValue );
						parameters.Add( p );
					}

					if( netMethod.ReturnType != null && netMethod.ReturnType != typeof( void ) )
					{
						var netType = netMethod.ReturnType;
						bool isRef = netType.IsByRef;
						if( isRef )
							netType = netType.GetElementType();

						var paramType = MetadataManager.GetTypeOfNetType( netType );
						//"_return"
						var p = new Metadata.Parameter( "ReturnValue", paramType, isRef, false, true, true, false, false, null );
						parameters.Add( p );
					}

					bool isOperator = netMethod.IsSpecialName && netMethod.Name.Length > 3 && netMethod.Name.Substring( 0, 3 ) == "op_";

					var member = new MethodImpl( this, netMethod.Name, netMethod.IsStatic, parameters.ToArray(), false, isOperator, compiledScript, netMethod );

					compiledMembers.Add( member );
					compiledMemberBySignature[ member.Signature ] = member;
				}

				//One method mode
				if( compiledScript != null && compiledMembers.Count == 1 )
				{
					compiledOneMethod = (MethodImpl)compiledMembers[ 0 ];
					OneMethod_AddProperties( compiledOneMethod );

					//var parameters = new List<Metadata.Parameter>();
					//parameters.Add( new Metadata.Parameter( "a", MetadataManager.GetTypeOfNetType( typeof( int ) ), false, true, false, false, false, false, null ) );
					//parameters.Add( new Metadata.Parameter( "ReturnValue", MetadataManager.GetTypeOfNetType( typeof( int ) ), false, true, false, true, false, false, null ) );
					//xx xx;
					//var method = new MethodImpl( this, "Method", true, parameters.ToArray(), false, false, compiledScript, compiledScript.PublicMethods[ 0 ] );

					//compiledMembers.Add( method );
					//compiledMemberBySignature[ method.Signature ] = method;
					//compiledOneMethod = method;

					////create properties
					//if( method != null )
					//{
					//	//parameters
					//	propertyMethodParameters = new List<PropertyImpl>();
					//	for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
					//	{
					//		var parameter = method.Parameters[ nParameter ];

					//		////!!!!имя еще как-то фиксить?
					//		//string name = null;
					//		//!!!!
					//		//if( parameter.ReturnValue )
					//		//	name = "ReturnValue";
					//		//if( name == null )
					//		var name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );

					//		//!!!!поддержать ByReference. еще какие?
					//		bool readOnly = parameter.Output || parameter.ReturnValue;

					//		//!!!!
					//		//var parameterType = GetOverrideParameterType( parameter, out bool error );
					//		//if( error )
					//		//{
					//		//	unableToInit = true;
					//		//	goto unableToInitLabel;
					//		//}
					//		//var parameterType = parameter.Type;

					//		Metadata.TypeInfo type;
					//		bool referenceSupport = !readOnly;
					//		if( referenceSupport )
					//		{
					//			Type unrefNetType = parameter.Type.GetNetType();
					//			var refNetType = typeof( Reference<> ).MakeGenericType( unrefNetType );
					//			type = MetadataManager.GetTypeOfNetType( refNetType );
					//		}
					//		else
					//			type = parameter.Type;

					//		bool invoke = parameter.ReturnValue || parameter.ByReference || parameter.Output;

					//		string namePrefix = "__parameter_";
					//		string displayName = TypeUtils.DisplayNameAddSpaces( name );

					//		var p = new PropertyImpl( this, namePrefix + name, false, type, parameter.Type, new Metadata.Parameter[ 0 ], readOnly, "Method's Parameters", displayName, referenceSupport, parameter, invoke );
					//		p.Description = "";
					//		if( !readOnly )
					//			p.Serializable = true;

					//		properties.Add( p );
					//		propertyBySignature[ p.Signature ] = p;

					//		//!!!!так? если несколько?
					//		if( parameter.ReturnValue )
					//			propertyMethodReturnParameter = p;
					//		else
					//			propertyMethodParameters.Add( p );
					//	}
					//}
				}

				//update child handlers
				try
				{
					foreach( var handler in GetComponents<EventHandlerComponent>() )
						handler.UpdateSubscription();
				}
				catch { }
			}
		}

		void OneMethod_AddProperties( MethodImpl method )
		{
			//parameters
			propertyMethodParameters = new List<MethodParameterPropertyImpl>();

			foreach( var parameter in method.Parameters )
			{
				var prop = OneMethod_AddPropertyOfParameter( parameter );

				properties.Add( prop );
				propertyBySignature[ prop.Signature ] = prop;

				//!!!!так? если несколько?
				if( parameter.ReturnValue )
					propertyMethodReturnParameter = prop;
				else
					propertyMethodParameters.Add( prop );
			}
		}

		MethodParameterPropertyImpl OneMethod_AddPropertyOfParameter( Metadata.Parameter parameter )
		{
			////!!!!имя еще как-то фиксить?
			string name = null;
			if( parameter.ReturnValue )
				name = "ReturnValue";
			else
				name = parameter.Name.Substring( 0, 1 ).ToUpper() + parameter.Name.Substring( 1 );

			//!!!!поддержать ByReference. еще какие?
			bool readOnly = parameter.Output || parameter.ReturnValue || parameter.ByReference;
			//bool readOnly = parameter.Output || parameter.ReturnValue;

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
			if( !string.IsNullOrEmpty( compiledCode ) )
			{
				compiledCodeOriginal = string.Empty;
				compiledCode = string.Empty;
				compiledScript = null;
				compiledMembers.Clear();
				compiledMemberBySignature.Clear();

				//One method mode
				compiledOneMethod = null;
				properties.Clear();
				propertyBySignature.Clear();
				propertyMethodParameters = null;
				propertyMethodReturnParameter = null;

				// clear cached texture for flow graph
				cachedFlowGraphNodeImage?.Dispose();
				cachedFlowGraphNodeImage = null;
			}
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			//needUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//needUpdate = true;
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			//!!!!temp UWP
			//if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
			//{
			Update();
			//}

			//#if W__!!__INDOWS_UWP
			//			// Scripting APIs can't be used within Universal Windows Applications and .NET Native since the application model doesn't support loading code generated at runtime
			//#else
			//			Update();
			//#endif

			foreach( var member in compiledMembers )
				yield return member;
			foreach( var member in properties )
				yield return member;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			//!!!!temp UWP
			//if( SystemSettings.CurrentPlatform != SystemSettings.Platform.UWP )
			//{
			Update();
			//}

			//#if W__!!__INDOWS_UWP
			//				// Scripting APIs can't be used within Universal Windows Applications and .NET Native since the application model doesn't support loading code generated at runtime
			//#else
			//			Update();
			//#endif

			if( compiledMemberBySignature.TryGetValue( signature, out var m ) )
				return m;
			if( propertyBySignature.TryGetValue( signature, out var m2 ) )
				return m2;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		/////////////////////////////////////////
		//Invoke Method

		public virtual void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			if( compiledOneMethod != null )
			{
				//!!!!может по другому
				data.NodeTitle = compiledOneMethod.ToString();

				if( cachedFlowGraphNodeImageScriptPrinter == null )
				{
					cachedFlowGraphNodeImageScriptPrinter = EditorAssemblyInterface.Instance.ScriptPrinterNew();
					//cachedFlowGraphNodeImageScriptPrinter = new ScriptPrinter();
					//cachedFlowGraphNodeImageScriptPrinter = new ScriptPrinter() { FontSize = 10, BackgroundBrush = System.Windows.Media.Brushes.Gray };
				}

				//!!!!правильнее знать какое точно разрешение. также скрипт может одновременно быть в нескольких разных нодах разных размеров.
				if( cachedFlowGraphNodeImage == null )
					cachedFlowGraphNodeImage = cachedFlowGraphNodeImageScriptPrinter.PrintToTexture( Code, new Vector2I( 512, 256 ) );
				//if( cachedNodeTexture == null )
				//	cachedNodeTexture = scriptPrinter.PrintToTexture( Code, new System.Windows.Size( 64, 64 ) );

				data.NodeImage = cachedFlowGraphNodeImage;
				data.NodeImageView = FlowGraphRepresentationData.NodeImageViewEnum.WideScaled;
				data.NodeHeight += 6;

				if( FlowSupport )
					data.NodeContentType = FlowGraphNodeContentType.Flow;
			}
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			if( compiledOneMethod != null )
			{
				Invoke();

				FlowInput next = Exit;
				if( next != null )
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
			}
		}

		void Invoke()
		{
			//!!!!

			//!!!!try catch

			//One method
			if( compiledOneMethod != null )
			{
				object obj = null;
				//if( !method.Static && propertyThis != null )
				//{
				//	obj = ReferenceUtils.GetUnreferencedValue( propertyThis.GetValue( this, null ) );

				//	//if( obj == null && !( method.Owner is Metadata.TypeInfo ) )
				//	//{
				//	//	obj = method.Owner;
				//	//	//Metadata.TypeInfo creatorType = method.Creator as Metadata.TypeInfo;
				//	//	//if( creatorType == null )
				//	//	//	creatorType = MetadataManager.MetadataGetType( method.Creator );
				//	//}
				//}

				object[] parameters = new object[ propertyMethodParameters.Count ];
				for( int n = 0; n < parameters.Length; n++ )
				{
					var value = ReferenceUtility.GetUnreferencedValue( propertyMethodParameters[ n ].GetValue( this, null, false ) );

					//подвисает только в дебаггере.
					////!!!!чтобы не подвисало. даже с try, catch. на int.Parse
					//{
					//	var type = propertyMethodParameters[ n ].TypeUnreferenced;

					//	if( value != null && !type.IsAssignableFrom( MetadataManager.MetadataGetType( value ) ) )
					//		value = null;
					//	if( value == null && type.GetNetType().IsValueType )
					//		value = type.InvokeInstance( null );
					//}

					parameters[ n ] = value;
				}

				//if( compiledOneMethod.Static )//|| method.Constructor || obj != null )
				{
					//!!!!пока так
					try
					{
						var result = compiledOneMethod.Invoke( obj, parameters );

						//ref, out
						for( int n = 0; n < parameters.Length; n++ )
						{
							var p = propertyMethodParameters[ n ];
							if( ( p.Parameter.ByReference || p.Parameter.Output ) && !p.Parameter.ReturnValue )
								p.SetValue( this, parameters[ n ], null );
						}

						//return
						propertyMethodReturnParameter?.SetValue( this, result, null );
					}
					catch
					{
						//!!!!
					}
				}
			}
		}

		[Browsable( false )]
		internal CompiledScript CompiledScript
		{
			get { return compiledScript; }
		}

		[Browsable( false )]
		public List<Metadata.Member> CompiledMembers
		{
			get { return compiledMembers; }
		}

		[Browsable( false )]
		internal MethodImpl CompiledOneMethod
		{
			get { return compiledOneMethod; }
		}

		[Browsable( false )]
		public bool DisableUpdate
		{
			get { return disableUpdate; }
			set { disableUpdate = value; }
		}

		void CreateScriptInstance()
		{
			if( compiledScript != null )
			{
				scriptInstance = Activator.CreateInstance( compiledScript.Type );

				compiledScript.SetField( scriptInstance, "Owner", this, out var error );
				if( !string.IsNullOrEmpty( error ) )
					Log.Error( $"Unable to set context variable \"{GetPathFromRoot( true )}\". " + error );
			}
			else
				scriptInstance = null;
		}

		[Browsable( false )]
		public object ScriptInstance
		{
			get { return scriptInstance; }
		}

		public Metadata.Member GetCompiledMember( string name )
		{
			for( int n = 0; n < CompiledMembers.Count; n++ )
			{
				var member = CompiledMembers[ n ];
				if( member.Name == name )
					return member;
			}
			return null;
		}

		public T GetCompiledPropertyValue<T>( string name, object[] index = null )
		{
			T result = default;

			var property = GetCompiledMember( name ) as Metadata.Property;
			if( property != null )
			{
				var value = property.GetValue( ScriptInstance, index );
				if( value != null )
				{
					var demandedType = typeof( T );
					if( demandedType == value.GetType() )
						result = (T)value;
					else if( demandedType.IsAssignableFrom( value.GetType() ) )
						result = (T)Convert.ChangeType( value, demandedType );
					else
						result = (T)MetadataManager.AutoConvertValue( value, demandedType );
				}
			}

			return result;
		}

		public void SetCompiledPropertyValue( string name, object value, object[] index = null )
		{
			var property = GetCompiledMember( name ) as Metadata.Property;
			if( property != null )
			{
				var value2 = value;

				var demandedType = property.Type.GetNetType();

				if( value2 != null )
				{
					if( demandedType != value2.GetType() )
					{
						if( demandedType.IsAssignableFrom( value2.GetType() ) )
							value2 = Convert.ChangeType( value2, demandedType );
						else
							value2 = MetadataManager.AutoConvertValue( value2, demandedType );
					}
				}

				if( demandedType.IsValueType && value2 == null )
					value2 = Activator.CreateInstance( demandedType );

				property.SetValue( ScriptInstance, value2, index );
			}
		}
	}
}
