// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a dialogue message block of flow graph.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Flow Dialogue Message", -2990 )]
	public class FlowDialogueMessage : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		static long uniqueMessageIdentifierCounter;

		static long GetUniqueMessageIdentifier()
		{
			unchecked
			{
				uniqueMessageIdentifierCounter++;
			}
			return uniqueMessageIdentifierCounter;
		}

		//

		bool needUpdate = true;
		int initializedCount;
		List<PropertyImpl> properties = new List<PropertyImpl>();
		Dictionary<string, PropertyImpl> propertyBySignature = new Dictionary<string, PropertyImpl>();
		Dictionary<string, object> propertyValues = new Dictionary<string, object>();

		/////////////////////////////////////////

		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The message from the dialogue creator
		/// </summary>
		[DefaultValue( "" )]
#if !DEPLOY
		[Editor( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) )]
#endif
		public Reference<string> Message
		{
			get { if( _message.BeginGet() ) Message = _message.Get( this ); return _message.value; }
			set { if( _message.BeginSet( this, ref value ) ) { try { MessageChanged?.Invoke( this ); } finally { _message.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Message"/> property value changes.</summary>
		public event Action<FlowDialogueMessage> MessageChanged;
		ReferenceField<string> _message = "";

		/// <summary>
		/// The number of answers.
		/// </summary>
		[DefaultValue( 3 )]
		[FlowGraphBrowsable( false )]
		public int AnswerCount { get; set; } = 3;

		/////////////////////////////////////////

		enum ParameterType
		{
			Text,
			Body,
		}

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			FlowDialogueMessage creator;
			string category;
			string displayName;
			internal int parameterIndex;
			internal ParameterType parameterType;

			////////////

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, FlowDialogueMessage creator, string category, string displayName, int parameterIndex, ParameterType parameterType )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.creator = creator;
				this.category = category;
				this.displayName = displayName;
				this.parameterIndex = parameterIndex;
				this.parameterType = parameterType;
			}

			public string Category
			{
				get { return category; }
				set { category = value; }
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				List<object> result = new List<object>();

				//Category attribute
				if( attributeType.IsAssignableFrom( typeof( CategoryAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( category ) )
						result.Add( new CategoryAttribute( category ) );
				}
				//DisplayName attribute
				if( attributeType.IsAssignableFrom( typeof( DisplayNameAttribute ) ) )
				{
					if( !string.IsNullOrEmpty( displayName ) )
						result.Add( new DisplayNameAttribute( displayName ) );
				}

				//Editor attribute
#if !DEPLOY
				if( parameterType == ParameterType.Text )
				{
					if( attributeType.IsAssignableFrom( typeof( EditorAttribute ) ) )
						result.Add( new EditorAttribute( "NeoAxis.Editor.HCItemTextBoxDropMultiline", typeof( object ) ) );
				}
#endif

				return result.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				object result = null;

				//always obj == Owner
				var c = (FlowDialogueMessage)obj;

				object value;
				if( c.propertyValues.TryGetValue( Signature, out value ) )
				{
					IReference iReference = value as IReference;
					if( iReference != null )
					{
						value = iReference.GetValue( obj );
						SetValue( obj, value, Indexers );
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
				var c = (FlowDialogueMessage)obj;

				if( value != null )
					c.propertyValues[ Signature ] = value;
				else
					c.propertyValues.Remove( Signature );
			}
		}

		/////////////////////////////////////////

		public FlowDialogueMessage()
		{
		}

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var member in base.OnMetadataGetMembers() )
				yield return member;

			Update();
			foreach( var p in properties )
				yield return p;
		}

		protected override Metadata.Member OnMetadataGetMemberBySignature( string signature )
		{
			Update();
			if( propertyBySignature.TryGetValue( signature, out PropertyImpl p ) )
				return p;

			return base.OnMetadataGetMemberBySignature( signature );
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			needUpdate = true;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			needUpdate = true;
		}

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void Update()
		{
			//check disabled
			if( !Enabled )
			{
				Clear();
				return;
			}

			//check for updates
			if( !needUpdate && initializedCount != AnswerCount )
				needUpdate = true;

			//nothing to update
			if( !needUpdate )
				return;

			//do update

			Clear();
			initializedCount = AnswerCount;
			needUpdate = false;

			//Cases
			for( int nAnswer = 0; nAnswer < AnswerCount; nAnswer++ )
			{
				{
					var p = new PropertyImpl( this, "AnswerText" + nAnswer.ToString(), false, MetadataManager.GetTypeOfNetType( typeof( Reference<string> ) ), MetadataManager.GetTypeOfNetType( typeof( string ) ), new Metadata.Parameter[ 0 ], false, this, "Answers", "Answer Text " + ( nAnswer + 1 ).ToString(), nAnswer, ParameterType.Text );
					p.Description = "";
					p.Serializable = SerializeType.Enable;

					properties.Add( p );
					propertyBySignature[ p.Signature ] = p;
				}

				{
					var p = new PropertyImpl( this, "AnswerBody" + nAnswer.ToString(), false, MetadataManager.GetTypeOfNetType( typeof( Reference<FlowInput> ) ), MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ), new Metadata.Parameter[ 0 ], false, this, "Answers", "Answer Body " + ( nAnswer + 1 ).ToString(), nAnswer, ParameterType.Body );
					p.Description = "";
					p.Serializable = SerializeType.Enable;

					properties.Add( p );
					propertyBySignature[ p.Signature ] = p;
				}
			}
		}

		void Clear()
		{
			properties.Clear();
			propertyBySignature.Clear();
		}

		//PropertyImpl GetProperty( int index, ParameterType type )
		//{
		//	foreach( var p in properties )
		//	{
		//		if( p.parameterIndex == index && p.parameterType == type )
		//			return p;
		//	}
		//	return null;
		//}

		public class MessageData
		{
			public string Message;
			public List<Answer> Answers = new List<Answer>();

			public class Answer
			{
				public int SourceID;
				public string Text;
			}
		}

		public delegate void PrepareMessageDelegate( FlowDialogueMessage sender, MessageData messageData );
		public event PrepareMessageDelegate PrepareMessage;

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			flow.Variables.TryGetValue( "_Interaction", out var interaction2 );
			var interaction = interaction2 as ContinuousInteraction;
			if( interaction == null )
			{
				Log.Warning( "FlowDialogueMessage: FlowExecution: _Interaction variable is null." );
				return;
			}

			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );
			if( isEntry )
			{
				//update current dialogue state

				//prepare message data
				var messageData = new MessageData();
				messageData.Message = Message.Value;
				for( int n = 0; n < AnswerCount; n++ )
				{
					var text = ObjectEx.PropertyGet<string>( this, $"AnswerText{n}" );
					if( text == null )
						text = "";
					var answer = new MessageData.Answer();
					answer.SourceID = n;
					answer.Text = text;
					messageData.Answers.Add( answer );
				}
				PrepareMessage?.Invoke( this, messageData );

				//save message data
				flow.InternalVariables[ this ] = messageData;

				//set data to the interaction
				var block = new TextBlock();
				block.SetAttribute( "MessageID", GetUniqueMessageIdentifier().ToString() );
				block.SetAttribute( "Message", messageData.Message );
				for( int n = 0; n < messageData.Answers.Count; n++ )
				{
					var answer = messageData.Answers[ n ];
					block.SetAttribute( $"Answer {n + 1}", answer.Text );
				}
				interaction.CurrentMessageFromCreator = block.DumpToString();
				interaction.CurrentMessageFromParticipant = "";

				//start sleeping

				//reply after sleeping
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

				//add to the sleeping list
				if( flow.Owner != null )
					flow.Owner.AddSleepingFlow( flow );
				else
					Flow.AddGlobalSleepingFlow( flow );

				//to exit from execution stack loop
				sleep = true;
			}
			else
			{
				//check for an answer to continue

				var answerText = interaction.CurrentMessageFromParticipant;
				if( string.IsNullOrEmpty( answerText ) )
				{
					//still no answer, continue sleeping

					//reply after sleeping
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

					//add to the sleeping list
					if( flow.Owner != null )
						flow.Owner.AddSleepingFlow( flow );
					else
						Flow.AddGlobalSleepingFlow( flow );

					//to exit from execution stack loop
					sleep = true;
				}
				else
				{
					//got answer

					//get message data
					flow.InternalVariables.TryGetValue( this, out var messageData2 );
					var messageData = messageData2 as MessageData;
					if( messageData == null )
					{
						Log.Warning( "FlowDialogueMessage: FlowExecution: MessageData is null." );
						return;
					}

					//remove the message data from the flow
					flow.InternalVariables.Remove( this );


					//parse the answer
					var block2 = TextBlock.Parse( answerText, out var error );
					if( block2 == null )
					{
						Log.Warning( "FlowDialogueMessage: FlowExecution: Invalid answer format. " + error );
						return;
					}

					var answerString = block2.GetAttribute( "Answer" );
					if( !int.TryParse( answerString, out var answerNumber ) )
					{
						//invalid answer identifier
						return;
					}

					var answerIndex = answerNumber - 1;
					if( answerIndex < 0 || answerIndex >= messageData.Answers.Count )
					{
						//invalid answer identifier
						return;
					}

					var answer = messageData.Answers[ answerIndex ];

					var next = ObjectEx.PropertyGet<FlowInput>( this, $"AnswerBody{answer.SourceID}" );
					if( next != null )
						flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
				}
			}
		}
	}
}
