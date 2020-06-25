// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents an event item for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemEvent : HCItemMember
	{
		Metadata.Event _event;
		string description;
		string displayName;

		//

		public HCItemEvent( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Event _event )
			: base( owner, parent, controlledObjects )
		{
			this._event = _event;
		}

		public Metadata.Event Event
		{
			get { return _event; }
		}

		public override Metadata.Member Member
		{
			get { return _event; }
		}

		public string DisplayName
		{
			get
			{
				if( displayName == null )
				{
					var dnAttr = Event.GetCustomAttribute<DisplayNameAttribute>( true );
					if( dnAttr != null )
						displayName = dnAttr.DisplayName ?? string.Empty;
					else
						displayName = TypeUtility.DisplayNameAddSpaces( Event.Name );
				}
				return displayName;
			}
		}

		public string Description
		{
			get
			{
				if( description == null )
				{
					var descrAttr = Event.GetCustomAttribute<DescriptionAttribute>( true );
					if( descrAttr != null )
						description = descrAttr.Description;

					if( description == null )
					{
						var id = XmlDocumentationFiles.GetMemberId( Event );
						if( !string.IsNullOrEmpty( id ) )
							description = XmlDocumentationFiles.GetMemberSummary( id );
					}

					if( description == null )
						description = "";

					Owner.PerformOverrideMemberDescription( this, ref description );
				}
				return description;
			}
		}

		public override UserControl CreateControlImpl()
		{
			var control = new HCGridEvent();

			if( control.ButtonAddEventHandler != null )
				control.ButtonAddEventHandler.Click += ButtonAddEventHandler_Click;
			if( control.ButtonEditEventHandlers != null )
				control.ButtonEditEventHandlers.Click += ButtonEditEventHandlers_Click;

			return control;
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCEvent)CreatedControl;

			if( control.LabelName != null )
			{
				control.LabelName.Text = DisplayName;
				control.LabelNameSetToolTip( Description );
			}

			if( control.ButtonEditEventHandlers != null )
			{
				var enable = GetEventHandlersToEdit().Count != 0;
				if( control.ButtonEditEventHandlers.Visible != enable )
					control.ButtonEditEventHandlers.Visible = enable;
			}
		}

		List<Component_EventHandler> GetEventHandlersToEdit()
		{
			var result = new List<Component_EventHandler>();

			var c = GetOneControlledObject<Component>();
			if( c != null )
			{
				//!!!!slowly?

				foreach( var handler in c.ParentRoot.GetComponents<Component_EventHandler>( checkChildren: true ) )
				{
					var eventValue = handler.Event.Value;
					if( eventValue != null )
					{
						if( eventValue.Object == c && eventValue.Member != null && eventValue.Member.Name == _event.Name )
							result.Add( handler );
					}
				}

				//var valueToCheck = "this:..\\event:" + _event.Name;
				//foreach( var handler in c.GetComponents<Component_EventHandler>() )
				//{
				//	if( handler.Event.GetByReference == valueToCheck )
				//		result.Add( handler );
				//}
			}

			return result;
		}

		private void ButtonEditEventHandlers_Click( object sender, EventArgs e )
		{
			var handlers = GetEventHandlersToEdit();
			if( handlers.Count == 0 )
				return;

			var items = new List<KryptonContextMenuItemBase>();

			foreach( var handler in handlers )
			{
				var text = "\'" + handler.GetPathFromRoot( true ) + "\'";
				var item = new KryptonContextMenuItem( text, null, delegate ( object s, EventArgs e2 )
				{
					var componentToSelect = (Component)( (KryptonContextMenuItem)s ).Tag;

					//if component in the flow graph, then activate flow graph window
					var parentNode = componentToSelect.Parent as Component_FlowGraphNode;
					if( parentNode != null )
					{
						var graph = parentNode.Parent as Component_FlowGraph;
						if( graph != null )
							EditorAPI.OpenDocumentWindowForObject( Owner.DocumentWindow.Document, graph );
					}

					//select object
					Owner.DocumentWindow.SelectObjects( new object[] { componentToSelect } );
				} );
				item.Tag = handler;
				items.Add( item );
			}

			EditorContextMenu.Show( items, Owner );
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "SettingsWindow", text );
		}

		Component_FlowGraphNode AddFlowGraphNode( Component_FlowGraph graph, Component subscribeTo )
		{
			var node = graph.CreateComponent<Component_FlowGraphNode>( enabled: false );
			node.Name = node.BaseType.GetUserFriendlyNameForInstance();
			node.Position = graph.EditorScrollPosition.ToVector2I();

			var handler = node.CreateComponent<Component_EventHandler>();
			handler.Name = handler.BaseType.GetUserFriendlyNameForInstance() + " " + _event.Name;
			handler.Event = ReferenceUtility.MakeReference<ReferenceValueType_Event>(
				null, ReferenceUtility.CalculateThisReference( handler, subscribeTo, _event.Signature ) );

			node.ControlledObject = ReferenceUtility.MakeReference<Component>( null, ReferenceUtility.CalculateThisReference( node, handler ) );
			node.Enabled = true;

			return node;
		}

		//void AddEventHandlerMethodToCSharp( string fileName, string className, string lineWithoutTabsAndBrackets )
		//{
		//	//!!!!

		//	//C# файл может быть уже открыт. может быть Modified
		//	//если не открыт, лучше открыть

		//	//проблема в том, куда именно вставить код. т.е. найти место. именно этот класс, внизу перед скобками.
		//	//по простому - это 2 скобки снизу пропустить.
		//	//может быть в где-то в интернете или рослине есть функциональность добавления подобного в код.
		//	//более универсально: нужна возможность добавлять мемберов в такой-то C# класс внутри файла. типа как интерпретировать чтоли. уже почти WinForms Designer.


		//}

		void GetMethodInfo( Component subscribeTo, out ParameterInfo[] parameters, out string parametersSignature )
		{
			//!!!!.NET events only
			parameters = _event.EventHandlerType.GetNetType().GetMethod( "Invoke" ).GetParameters();

			//get signature of parameters
			{
				var b = new StringBuilder();

				bool paramsWasAdded = false;
				for( int n = 0; n < parameters.Length; n++ )
				{
					var p = parameters[ n ];
					if( !p.IsRetval )
					{
						if( paramsWasAdded )
							b.Append( ',' );

						if( p.IsOut )
							b.Append( "out " );
						else if( p.ParameterType.IsByRef )
							b.Append( "ref " );

						//!!!!может лучше использовать движковые метаданные

						var type = p.ParameterType;
						if( p.ParameterType.IsByRef )
							type = p.ParameterType.GetElementType();
						b.Append( MetadataManager.GetNetTypeName( type, false, true ) );

						//b.Append( MetadataManager.GetNetTypeName( p.ParameterType, false, true ) );
						//b.Append( p.ParameterType.Name );

						paramsWasAdded = true;
					}
				}

				parametersSignature = b.ToString();
			}
		}

		void AddHandlerToSharpClass( Component subscribeTo, string csharpFileName, string className )
		{
			GetMethodInfo( subscribeTo, out var parameters, out var parametersSignature );

			//get unique method name
			string methodName;
			string methodSignature;
			{
				var methodNameNotUnique = subscribeTo.Name.Replace( " ", "" ) + "_" + _event.Name;

				for( int n = 1; ; n++ )
				{
					methodName = methodNameNotUnique;
					if( n != 1 )
						methodName += n.ToString();
					methodSignature = $"method:{methodName}({parametersSignature})";

					if( subscribeTo.ParentRoot.MetadataGetMemberBySignature( methodSignature ) == null )
						break;
				}
			}

			var newObjects = new List<Component>();

			//create handler
			var handler = subscribeTo.CreateComponent<Component_EventHandler>( enabled: false );
			handler.Name = handler.BaseType.GetUserFriendlyNameForInstance() + " " + _event.Name;
			handler.Event = ReferenceUtility.MakeReference<ReferenceValueType_Event>( null,
				ReferenceUtility.CalculateThisReference( handler, subscribeTo, _event.Signature ) );
			handler.HandlerMethod = ReferenceUtility.MakeReference<ReferenceValueType_Method>( null,
				ReferenceUtility.CalculateThisReference( handler, subscribeTo.ParentRoot, methodSignature ) );
			handler.Enabled = true;
			newObjects.Add( handler );

			//undo for handler
			var document = Owner.DocumentWindow.Document;
			var action = new UndoActionComponentCreateDelete( document, newObjects, true );
			document.UndoSystem.CommitAction( action );
			document.Modified = true;

			Owner.DocumentWindow.SelectObjects( newObjects.ToArray() );

			//open cs file and add method
			var documentWindow = EditorAPI.OpenFileAsDocument( csharpFileName, true, true ) as CSharpDocumentWindow;
			if( documentWindow != null )
			{
				if( !documentWindow.ScriptEditorControl.AddMethod( methodName, parameters, out var error ) )
				{
					Log.Warning( "Unable to add a code of the method. " + error );
					//!!!!
				}
			}
			else
			{
				Log.Warning( "Unable to add a code of the method. Document file is not opened." );
				//!!!!
			}
		}

		private void ButtonAddEventHandler_Click( object sender, EventArgs e )
		{
			var items = new List<KryptonContextMenuItemBase>();

			var subscribeTo = GetOneControlledObject<Component>();
			if( subscribeTo == null )
				return;

			//add handler to C# class
			{
				bool enable = false;
				string className = null;
				string csharpFileName = null;

				var fileName = subscribeTo.ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
				if( !string.IsNullOrEmpty( fileName ) )
				{
					className = subscribeTo.ParentRoot.GetType().Name;

					try
					{
						//find by same class name
						if( string.Compare( Path.GetFileNameWithoutExtension( fileName ), className, true ) == 0 )
						//if( Path.GetFileNameWithoutExtension( fileName ) == className )
						{
							csharpFileName = VirtualPathUtility.GetRealPathByVirtual( Path.ChangeExtension( fileName, "cs" ) );
							if( File.Exists( csharpFileName ) )
								enable = true;
						}

						//find by same file name
						if( !enable )
						{
							csharpFileName = VirtualPathUtility.GetRealPathByVirtual( Path.Combine( Path.GetDirectoryName( fileName ), className + ".cs" ) );
							if( File.Exists( csharpFileName ) )
								enable = true;
						}
					}
					catch { }
				}

				var item = new KryptonContextMenuItem( Translate( "Add Handler to C# File" ), null, delegate ( object s, EventArgs e2 )
				{
					AddHandlerToSharpClass( subscribeTo, csharpFileName, className );
				} );
				item.Enabled = enable;
				items.Add( item );
			}

			//add handler to C# script
			{
				var groupItem = new KryptonContextMenuItem( Translate( "Add Handler to C# Script" ), null );
				var childItems = new List<KryptonContextMenuItemBase>();

				//create new C# script
				{
					var itemsData = new List<(string, Component)>();
					itemsData.Add( (Translate( "Add C# Script in This Component" ), subscribeTo) );
					if( subscribeTo != subscribeTo.ParentRoot )
						itemsData.Add( (Translate( "Add C# Script in the Root Component" ), subscribeTo.ParentRoot) );

					foreach( var itemData in itemsData )
					{
						var childItem = new KryptonContextMenuItem( itemData.Item1, null, delegate ( object s, EventArgs e2 )
						{
							var parent = (Component)( (KryptonContextMenuItem)s ).Tag;
							var document = Owner.DocumentWindow.Document;

							//create script
							var script = parent.CreateComponent<Component_CSharpScript>( enabled: false );
							script.Name = script.BaseType.GetUserFriendlyNameForInstance();
							script.Code = "class _Temp{\r\n}";
							script.Enabled = true;

							//activate flow graph window
							var scriptDocumentWindow = EditorAPI.OpenDocumentWindowForObject( document, script ) as Component_CSharpScript_DocumentWindow;

							Owner.DocumentWindow.SelectObjects( new object[] { script } );

							GetMethodInfo( subscribeTo, out var parameters, out var parametersSignature );
							var methodNameNotUnique = subscribeTo.Name.Replace( " ", "" ) + "_" + _event.Name;
							var methodName = methodNameNotUnique;
							var methodSignature = $"method:{methodName}({parametersSignature})";

							if( !scriptDocumentWindow.ScriptEditorControl.AddMethod( methodName, parameters, out var error ) )
							{
								Log.Warning( "Unable to add a code of the method. " + error );
								//!!!!
							}

							//fix code
							try
							{
								scriptDocumentWindow.ScriptEditorControl.GetCode( out var code );

								code = code.Replace( "class _Temp{", "" );

								var index = code.LastIndexOf( '}' );
								code = code.Substring( 0, index );

								string newCode = "";
								var lines = code.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
								foreach( var line in lines )
									newCode += line.Trim() + "\r\n";

								script.Code = newCode;
							}
							catch( Exception e3 )
							{
								Log.Warning( "Unable to fix code of the method. " + e3.Message );
								//!!!!
							}

							//create event handler
							{
								var handler = script.CreateComponent<Component_EventHandler>( enabled: false );
								handler.Name = handler.BaseType.GetUserFriendlyNameForInstance() + " " + _event.Name;
								handler.Event = ReferenceUtility.MakeReference<ReferenceValueType_Event>( null,
									ReferenceUtility.CalculateThisReference( handler, subscribeTo, _event.Signature ) );
								handler.HandlerMethod = ReferenceUtility.MakeReference<ReferenceValueType_Method>( null,
									ReferenceUtility.CalculateThisReference( handler, script, methodSignature ) );
								handler.Enabled = true;
							}

							//undo
							var action = new UndoActionComponentCreateDelete( document, new Component[] { script }, true );
							document.UndoSystem.CommitAction( action );
							document.Modified = true;

						} );
						childItem.Tag = itemData.Item2;
						childItems.Add( childItem );
					}
				}

				//add handler to one of already created C# scripts
				foreach( var script in subscribeTo.ParentRoot.GetComponents<Component_CSharpScript>( false, true ) )
				{
					if( script.TypeSettingsIsPublic() )
					{
						var text = string.Format( Translate( "Add Handler to \'{0}\'" ), script.GetPathFromRoot( true ) );
						var item = new KryptonContextMenuItem( text, null, delegate ( object s, EventArgs e2 )
						{
							var script2 = (Component_CSharpScript)( (KryptonContextMenuItem)s ).Tag;
							var document = Owner.DocumentWindow.Document;

							var oldCode = script2.Code;

							script2.Code = "class _Temp{\r\n}";

							//activate flow graph window
							var scriptDocumentWindow = EditorAPI.OpenDocumentWindowForObject( document, script2 ) as Component_CSharpScript_DocumentWindow;

							Owner.DocumentWindow.SelectObjects( new object[] { script2 } );

							GetMethodInfo( subscribeTo, out var parameters, out var parametersSignature );

							//get unique method name
							string methodName;
							string methodSignature;
							{
								var methodNameNotUnique = subscribeTo.Name.Replace( " ", "" ) + "_" + _event.Name;

								for( int n = 1; ; n++ )
								{
									methodName = methodNameNotUnique;
									if( n != 1 )
										methodName += n.ToString();
									methodSignature = $"method:{methodName}({parametersSignature})";

									bool found = false;
									if( script2.CompiledScript != null )
									{
										foreach( var m in script2.CompiledScript.Methods )
											if( m.Name == methodName )
												found = true;
									}
									if( !found )
										break;
									//if( subscribeTo.ParentRoot.MetadataGetMemberBySignature( methodSignature ) == null )
									//	break;
								}
							}

							//GetMethodInfo( subscribeTo, out var parameters, out var methodName, out var methodSignature );

							if( !scriptDocumentWindow.ScriptEditorControl.AddMethod( methodName, parameters, out var error ) )
							{
								Log.Warning( "Unable to add a code of the method. " + error );
								//!!!!
							}

							//set Code
							try
							{
								scriptDocumentWindow.ScriptEditorControl.GetCode( out var code );

								code = code.Replace( "class _Temp{", "" );

								var index = code.LastIndexOf( '}' );
								code = code.Substring( 0, index );

								string newCode = "";
								var lines = code.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
								foreach( var line in lines )
									newCode += line.Trim() + "\r\n";

								script2.Code = oldCode + "\r\n" + newCode;
							}
							catch( Exception e3 )
							{
								Log.Warning( "Unable to fix code of the method. " + e3.Message );
								//!!!!
							}

							//create event handler
							Component_EventHandler handler;
							{
								handler = script2.CreateComponent<Component_EventHandler>( enabled: false );
								handler.Name = handler.BaseType.GetUserFriendlyNameForInstance() + " " + _event.Name;
								handler.Event = ReferenceUtility.MakeReference<ReferenceValueType_Event>( null,
									ReferenceUtility.CalculateThisReference( handler, subscribeTo, _event.Signature ) );
								handler.HandlerMethod = ReferenceUtility.MakeReference<ReferenceValueType_Method>( null,
									ReferenceUtility.CalculateThisReference( handler, script2, methodSignature ) );
								handler.Enabled = true;
							}

							//undo
							{
								var property = (Metadata.Property)script2.MetadataGetMemberBySignature( "property:Code" );
								var undoItem = new UndoActionPropertiesChange.Item( script2, property, oldCode, new object[ 0 ] );
								var action1 = new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item[] { undoItem } );

								var action2 = new UndoActionComponentCreateDelete( document, new Component[] { handler }, true );

								document.UndoSystem.CommitAction( new UndoMultiAction( new UndoSystem.Action[] { action1, action2 } ) );
								document.Modified = true;
							}
						} );
						item.Tag = script;
						childItems.Add( item );
					}
				}

				groupItem.Items.Add( new KryptonContextMenuItems( childItems.ToArray() ) );
				items.Add( groupItem );
			}

			//Add Handler to Flow Graph
			{
				var groupItem = new KryptonContextMenuItem( Translate( "Add Handler to Flow Graph" ), null );
				var childItems = new List<KryptonContextMenuItemBase>();

				//create new flow graph
				{
					var itemsData = new List<(string, Component)>();
					itemsData.Add( (Translate( "Add Flow Graph in This Component" ), subscribeTo) );
					if( subscribeTo != subscribeTo.ParentRoot )
						itemsData.Add( (Translate( "Add Flow Graph in the Root Component" ), subscribeTo.ParentRoot) );

					foreach( var itemData in itemsData )
					{
						var childItem = new KryptonContextMenuItem( itemData.Item1, null, delegate ( object s, EventArgs e2 )
						{
							var parent = (Component)( (KryptonContextMenuItem)s ).Tag;

							//create flow graph
							var graph = parent.CreateComponent<Component_FlowGraph>( enabled: false );
							graph.Name = graph.BaseType.GetUserFriendlyNameForInstance();
							graph.Enabled = true;

							//create node with handler
							var node = AddFlowGraphNode( graph, subscribeTo );
							node.Position = new Vector2I( -20, -10 );

							//undo
							var document = Owner.DocumentWindow.Document;
							var action = new UndoActionComponentCreateDelete( document, new Component[] { graph }, true );
							document.UndoSystem.CommitAction( action );
							document.Modified = true;

							//activate flow graph window
							EditorAPI.OpenDocumentWindowForObject( document, graph );
						} );
						childItem.Tag = itemData.Item2;
						childItems.Add( childItem );
					}
				}

				//add handler to one of already created flow graph
				foreach( var graph in subscribeTo.ParentRoot.GetComponents<Component_FlowGraph>( false, true ) )
				{
					if( graph.TypeSettingsIsPublic() )
					{
						var text = string.Format( Translate( "Add Handler to \'{0}\'" ), graph.GetPathFromRoot( true ) );
						var item = new KryptonContextMenuItem( text, null, delegate ( object s, EventArgs e2 )
						{
							var graph2 = (Component_FlowGraph)( (KryptonContextMenuItem)s ).Tag;

							//create node with handler
							var node = AddFlowGraphNode( graph2, subscribeTo );

							//undo
							var document = Owner.DocumentWindow.Document;
							var action = new UndoActionComponentCreateDelete( document, new Component[] { node }, true );
							document.UndoSystem.CommitAction( action );
							document.Modified = true;

							//activate flow graph window
							EditorAPI.OpenDocumentWindowForObject( document, graph2 );
						} );
						item.Tag = graph;
						childItems.Add( item );
					}
				}

				groupItem.Items.Add( new KryptonContextMenuItems( childItems.ToArray() ) );
				items.Add( groupItem );
			}

			////add handler (component only)
			//{
			//	var item = new KryptonContextMenuItem( Translate( "Add handler (component only)" ), null, delegate ( object s, EventArgs e2 )
			//	{
			//		var newObjects = new List<Component>();

			//		var handler = subscribeTo.CreateComponent<Component_EventHandler>( enable: false );
			//		handler.Name = handler.BaseType.GetUserFriendlyNameForInstance() + " " + _event.Name;
			//		handler.Event = ReferenceUtility.MakeReference<ReferenceValueType_Event>( null,
			//			ReferenceUtility.CalculateThisReference( handler, subscribeTo, _event.Signature ) );
			//		handler.Enabled = true;
			//		newObjects.Add( handler );

			//		var document = Owner.DocumentWindow.Document;
			//		var action = new UndoActionComponentCreateDelete( document, newObjects, true );
			//		document.UndoSystem.CommitAction( action );
			//		document.Modified = true;

			//		Owner.DocumentWindow.SelectObjects( newObjects.ToArray() );
			//	} );
			//	item.Enabled = GetOneControlledObject<Component>() != null;
			//	items.Add( item );
			//}

			EditorContextMenu.Show( items, Owner );
		}
	}
}
