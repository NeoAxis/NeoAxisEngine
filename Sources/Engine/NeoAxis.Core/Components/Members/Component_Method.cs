// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component for adding a virtual method to the parent component.
	/// </summary>
	[EditorNewObjectSettings( typeof( NewObjectSettingsMethod ) )]
	public class Component_Method : Component_Member
	{
		Metadata.Method createdMethod;

		/////////////////////////////////////////

		////!!!!!может делегат?
		////Parameters
		//ReferenceField<List<Component_MemberParameter>> _parameters = new ReferenceField<List<Component_MemberParameter>>();
		//[Serialize]
		//public Reference<List<Component_MemberParameter>> Parameters
		//{
		//	get
		//	{
		//		if( _parameters.BeginGet() )
		//			Parameters = _parameters.Get( this );
		//		return _parameters.value;
		//	}
		//	set
		//	{
		//		if( _parameters.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				ParametersChanged?.Invoke( this );

		//				//!!!!как тут?
		//				NeedUpdateParentVirtualMembers();
		//			}
		//			finally { _parameters.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Method> ParametersChanged;

		/// <summary>
		/// The body of the method.
		/// </summary>
		[Serialize]
		public Reference<Component_MethodBody> Body
		{
			get { if( _body.BeginGet() ) Body = _body.Get( this ); return _body.value; }
			set
			{
				if( _body.BeginSet( ref value ) )
				{
					try
					{
						BodyChanged?.Invoke( this );

						//!!!!надо пересоздавать?
						NeedUpdateCreatedMembers();
					}
					finally { _body.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Body"/> property value changes.</summary>
		public event Action<Component_Method> BodyChanged;
		ReferenceField<Component_MethodBody> _body;

		/////////////////////////////////////////

		class MethodImpl : Metadata.Method
		{
			Component_Method creator;

			//

			public MethodImpl( object owner, string name, bool isStatic, Metadata.Parameter[] parameters, bool isConstructor, bool isOperator, Component_Method creator )
				: base( owner, name, isStatic, parameters, isConstructor, isOperator )
			{
				this.creator = creator;
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				//!!!!
				return Array.Empty<object>();
			}

			public override object Invoke( object obj, object[] parameters )
			{
				var body = creator.Body.Value;
				if( body != null )
				{
					var returnValue = body.Invoke( obj, parameters );
					return returnValue;
				}
				else
				{
					//!!!!?
					return base.Invoke( obj, parameters );
				}
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// A set of settings for <see cref="Component_Method"/> creation in the editor.
		/// </summary>
		public class NewObjectSettingsMethod : NewObjectSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			[DisplayName( "Flow Graph" )]
			public bool FlowGraph { get; set; } = true;

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				var method = (Component_Method)context.newObject;

				if( FlowGraph )
					method.NewObjectCreateFlowGraph();

				return base.Creation( context );
			}
		}

		/////////////////////////////////////////

		//!!!!сбрасывать кеш
		//[Serialize]
		//public List<Parameter> Parameters
		//{
		//	get { return parameters; }
		//}

		public Component_Method()
		{
			ComponentsChanged += Component_Method_ComponentsChanged;
		}

		void Component_Method_ComponentsChanged( Component obj )
		{
			NeedUpdateCreatedMembers();
		}

		//!!!!cache?
		public string GetDisplayName()
		{
			//!!!!

			var parameters2 = new List<Component_MemberParameter>();
			foreach( var p in GetComponents<Component_MemberParameter>( false, false ) )
			{
				if( p.Enabled )
					parameters2.Add( p );
			}
			//List<Component_MemberParameter> parameters2 = Parameters;

			StringBuilder b = new StringBuilder();

			//return param
			{
				//!!!!может быть несколько

				Component_MemberParameter returnParam = null;
				foreach( var p in parameters2 )
				{
					if( p.ReturnValue )
					{
						returnParam = p;
						break;
					}
				}
				if( returnParam != null )
					b.Append( returnParam.Type );
				else
					b.Append( "void" );
				b.Append( ' ' );
			}

			b.Append( Name );
			b.Append( '(' );
			bool paramsWasAdded = false;
			for( int n = 0; n < parameters2.Count; n++ )
			{
				var p = parameters2[ n ];
				if( !p.ReturnValue )
				{
					b.Append( paramsWasAdded ? ", " : " " );

					if( p.ByReference )
						b.Append( "ref " );
					else if( p.Output )
						b.Append( "out " );
					//if( p.Output )
					//	b.Append( p.In ? "ref " : "out " );

					b.Append( p.Type );
					b.Append( ' ' );
					b.Append( p.Name );

					//!!!!default value

					paramsWasAdded = true;
				}
			}
			if( paramsWasAdded )
				b.Append( ' ' );
			b.Append( ')' );
			return b.ToString();
		}

		public override string ToString()
		{
			//!!!!по сути надо опредлелять тип. а если ошибка, то показывать

			return GetDisplayName();
		}

		public override void NeedUpdateCreatedMembers()
		{
			base.NeedUpdateCreatedMembers();

			createdMethod = null;
		}

		MethodImpl CreateMethod()
		{
			var parameters = new List<Component_MemberParameter>();
			foreach( var p in GetComponents<Component_MemberParameter>( false, false ) )
			{
				if( p.Enabled )
					parameters.Add( p );
			}

			var parameters2 = new List<Metadata.Parameter>();
			foreach( var p in parameters )
			{
				Metadata.Parameter p2 = p.CreateMetadataParameter();
				if( p2 == null )
				{
					//!!!!
					return null;
				}

				parameters2.Add( p2 );
			}

			//!!!!isConstructor
			//!!!!isOperator
			var method = new MethodImpl( Parent, Name, Static, parameters2.ToArray(), false, false, this );
			method.Description = Description;

			return method;
		}

		protected override void CreateMembers( List<Metadata.Member> created )
		{
			var method = CreateMethod();
			if( method != null )
			{
				createdMethod = method;
				created.Add( method );
			}
		}

		//public override void UpdateParentVirtualMembers( ref List<Metadata.Member> members, ref Dictionary<string, Metadata.Member> memberBySignature )
		//{
		//	base.UpdateParentVirtualMembers( ref members, ref memberBySignature );

		//	xx xx;
		//	//create
		//	if( createdMethod == null && Parent != null )
		//	{
		//		createdMethod = CreateMethod();

		//		xx xx;
		//		//!!!!
		//		//Body.Value?.NeedUpdate();
		//	}

		//	xx xx;
		//	//add to parent
		//	if( createdMethod != null )
		//	{
		//		if( members == null )
		//		{
		//			members = new List<Metadata.Member>();
		//			memberBySignature = new Dictionary<string, Metadata.Member>();
		//		}
		//		members.Add( createdMethod );
		//		memberBySignature[ createdMethod.Signature ] = createdMethod;
		//	}
		//}

		//public override Metadata.Member CreatedMember
		//{
		//	get
		//	{
		//		xx xx;

		//		return createdMethod;
		//	}
		//}

		void NewObjectCreateFlowGraph()
		{
			var graph = CreateComponent<Component_FlowGraph>();
			graph.Name = "Flow Graph";

			Component_MethodBody body;
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();

				body = node.CreateComponent<Component_MethodBody>();
				body.Name = body.BaseType.GetUserFriendlyNameForInstance();

				node.Name = "Node " + body.Name;
				node.Position = new Vector2I( -25, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, body );
			}

			Component_MethodBodyEnd bodyEnd;
			{
				var node = graph.CreateComponent<Component_FlowGraphNode>();

				bodyEnd = node.CreateComponent<Component_MethodBodyEnd>();
				bodyEnd.Name = bodyEnd.BaseType.GetUserFriendlyNameForInstance();

				node.Name = "Node " + bodyEnd.Name;
				node.Position = new Vector2I( 15, -7 );
				node.ControlledObject = ReferenceUtility.MakeThisReference( node, bodyEnd );
			}

			Body = ReferenceUtility.MakeThisReference( this, body );
			body.Definition = ReferenceUtility.MakeThisReference( body, this );
			body.BodyEnd = ReferenceUtility.MakeThisReference( body, bodyEnd );
			bodyEnd.Body = ReferenceUtility.MakeThisReference( bodyEnd, body );

			//!!!!выделять, открывать созданные
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			if( !createdFromNewObjectWindow )
				NewObjectCreateFlowGraph();
		}
	}
}
