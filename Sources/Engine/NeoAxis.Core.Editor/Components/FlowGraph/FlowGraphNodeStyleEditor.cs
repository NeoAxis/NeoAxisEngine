// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class FlowchartNodeStyle_Circle : FlowchartNodeStyle
	//{
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Rectangular flow graph nodes style.
	/// </summary>
	public class FlowGraphNodeStyle_Rectangle : FlowGraphNodeStyle
	{
#if !DEPLOY
		static FlowGraphNodeStyle_Rectangle instance;
		/// <summary>
		/// Singleton object of the style.
		/// </summary>
		public static FlowGraphNodeStyle_Rectangle Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new FlowGraphNodeStyle_Rectangle();
					instance.Name = "Rectangle";
				}
				return instance;
			}
		}

		/////////////////////////////////////////

		public override Vector2 GetSocketPositionInUnits( FlowGraphNode.Representation.Item socket, bool input )
		{
			var rep = socket.Owner;
			var node = rep.Owner;

			if( input )
				return node.Position.ToVector2() + new Vector2( 0, (double)socket.Index + 0.5 + (double)rep.GetTextsOffsetY() );
			else
				return node.Position.ToVector2() + new Vector2( rep.Size.X, (double)socket.Index + 0.5 + (double)rep.GetTextsOffsetY() );
			//if( input )
			//	return node.NodePosition.ToVec2() + new Vec2( 0, (double)socket.Index + 1.5f + (double)rep.GetTextsOffsetY() );
			//else
			//	return node.NodePosition.ToVec2() + new Vec2( rep.Size.X, (double)socket.Index + 1.5f + (double)rep.GetTextsOffsetY() );
		}

		public override void RenderNodeReferences( IFlowGraphEditor window2, FlowGraphNode node,
			Dictionary<Component, List<FlowGraphNode>> objectToNodes,
			Dictionary<FlowGraphNode.Representation.Item, RenderSelectionState> referenceSelectionStates,
			out FlowGraphNode.Representation.Item outMouseOverReference )
		{
			var window = (FlowGraphEditor)window2;

			outMouseOverReference = null;

			var representation = node.GetRepresentation();
			var renderer = window.ViewportControl2.Viewport.CanvasRenderer;

			for( int nItem = 0; nItem < representation.Items.Count; nItem++ )
			{
				var item = representation.Items[ nItem ];

				//references from input
				if( item.Input != null )
				{
					var itemProperty = item as FlowGraphNode.Representation.ItemProperty;
					if( itemProperty != null && ReferenceUtility.IsReferenceType( itemProperty.Property.Type.GetNetType() ) &&
						!MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( itemProperty.Property.TypeUnreferenced ) )
					{
						Component obj = itemProperty.Owner.Owner.ControlledObject;
						if( obj != null )
						{
							var v = itemProperty.Property.GetValue( obj, null );
							IReference iReference = v as IReference;
							if( iReference != null && !string.IsNullOrEmpty( iReference.GetByReference ) )
							{
								iReference.GetMember( obj, out object destObject, out Metadata.Member destMember );
								var destProperty = destMember as Metadata.Property;
								if( destProperty != null )
								{
									//reference to property

									//!!!!только компоненты? статичные свойства тоже нельзя?
									var destComponent = destObject as Component;
									if( destComponent != null && objectToNodes.TryGetValue( destComponent, out List<FlowGraphNode> destNodes ) )
									{
										foreach( var destNode in destNodes )
										{
											var destRep = destNode.GetRepresentation();
											FlowGraphNode.Representation.ItemProperty destSocket;
											if( destRep.ItemByProperty.TryGetValue( destProperty, out destSocket ) )
											{
												if( destSocket.Output != null )
												{
													var from = GetSocketPositionInUnits( item, true );
													var to = GetSocketPositionInUnits( destSocket, false );

													if( !referenceSelectionStates.TryGetValue( item, out RenderSelectionState state ) )
														state = RenderSelectionState.None;

													ColorValue color;
													if( state != RenderSelectionState.None )
														color = GetColorMultiplierSelectionState( state );
													else
														color = new ColorValue( 0.5, 0.5, 0.5 );
													//ColorValue color = GetColorMultiplierSelectionState( state );

													window.GetFlowGraphStyle().RenderReference( window, from, true, to, color, out bool mouseOver );

													if( mouseOver && outMouseOverReference == null )
														outMouseOverReference = item;
												}
											}
										}
									}
								}
								else
								{
									//reference to Component

									var unrefValue = ReferenceUtility.GetUnreferencedValue( iReference.GetValue( obj ) );
									var destComponent = unrefValue as Component;
									if( destComponent != null && objectToNodes.TryGetValue( destComponent, out List<FlowGraphNode> destNodes ) )
									{
										foreach( var destNode in destNodes )
										{
											var destRep = destNode.GetRepresentation();
											var destSocket = destRep.ItemObject;
											if( destSocket != null )
											{
												var from = GetSocketPositionInUnits( item, true );
												var to = GetSocketPositionInUnits( destSocket, false );

												if( !referenceSelectionStates.TryGetValue( item, out RenderSelectionState state ) )
													state = RenderSelectionState.None;

												ColorValue color;
												if( state != RenderSelectionState.None )
													color = GetColorMultiplierSelectionState( state );
												else
													color = new ColorValue( 0.5, 0.5, 0.5 );
												//ColorValue color = GetColorMultiplierSelectionState( state );

												window.GetFlowGraphStyle().RenderReference( window, from, true, to, color, out bool mouseOver );

												if( mouseOver && outMouseOverReference == null )
													outMouseOverReference = item;
											}
										}
									}

									//!!!!invalid reference
								}
							}
						}
					}
				}

				//references from output (FlowInput)
				if( item.Output != null )
				{
					var itemProperty = item as FlowGraphNode.Representation.ItemProperty;
					if( itemProperty != null && ReferenceUtility.IsReferenceType( itemProperty.Property.Type.GetNetType() ) &&
						MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( itemProperty.Property.TypeUnreferenced ) )
					{
						Component obj = itemProperty.Owner.Owner.ControlledObject;
						if( obj != null )
						{
							var v = itemProperty.Property.GetValue( obj, null );
							IReference iReference = v as IReference;
							if( iReference != null && !string.IsNullOrEmpty( iReference.GetByReference ) )
							{
								iReference.GetMember( obj, out object destObject, out Metadata.Member destMember );

								var destProperty = destMember as Metadata.Property;
								if( destProperty != null )
								{
									//!!!!только компоненты? статичные свойства тоже нельзя?
									var destComponent = destObject as Component;
									if( destComponent != null && objectToNodes.TryGetValue( destComponent, out List<FlowGraphNode> destNodes ) )
									{
										foreach( var destNode in destNodes )
										{
											var destRep = destNode.GetRepresentation();
											FlowGraphNode.Representation.ItemProperty destSocket;
											if( destRep.ItemByProperty.TryGetValue( destProperty, out destSocket ) )
											{
												if( destSocket.Input != null )
												{
													var from = GetSocketPositionInUnits( item, false );
													var to = GetSocketPositionInUnits( destSocket, true );

													if( !referenceSelectionStates.TryGetValue( item, out RenderSelectionState state ) )
														state = RenderSelectionState.None;

													ColorValue color;
													if( state != RenderSelectionState.None )
														color = GetColorMultiplierSelectionState( state );
													else
														color = new ColorValue( 0.3, 0.3, 1 );
													//ColorValue color = GetColorMultiplierSelectionState( state );

													window.GetFlowGraphStyle().RenderReference( window, from, false, to, color, out bool mouseOver );

													if( mouseOver && outMouseOverReference == null )
														outMouseOverReference = item;
												}
											}
										}
									}
								}
								else
								{
									//!!!!invalid reference
								}
							}
						}
					}
				}
			}
		}

		ColorValue GetColorMultiplierSelectionState( RenderSelectionState selectionState )
		{
			switch( selectionState )
			{
			case RenderSelectionState.CanSelect: return new ColorValue( 1, 1, 0 );
			case RenderSelectionState.Selected: return new ColorValue( 0, 1, 0 );
			}
			return new ColorValue( 1, 1, 1 );
		}

		public override void RenderNode( IFlowGraphEditor window2, FlowGraphNode node,
			RenderSelectionState selectionStateNode, RenderSelectionState selectionStateControlledObject, object mouseOverObject,
			FlowGraphNode.Representation.Connector referenceCreationSocketFrom, IDragDropSetReferenceData dragDropSetReferenceData )
		{
			var window = (FlowGraphEditor)window2;

			var representation = node.GetRepresentation();
			var viewport = window.ViewportControl2.Viewport;
			var mouse = viewport.MousePosition;
			var renderer = viewport.CanvasRenderer;

			FlowGraphRepresentationData representationData = new FlowGraphRepresentationData();
			{
				var iRepresentationData = node.ControlledObject.Value as IFlowGraphRepresentationData;
				if( iRepresentationData != null )
					iRepresentationData.GetFlowGraphRepresentationData( representationData );
			}

			Vector2I nodeSize = representation.Size;
			RectangleI nodeRectWithOneBorder = new RectangleI( node.Position - new Vector2I( 1, 1 ), node.Position + nodeSize + new Vector2I( 1, 1 ) );

			//!!!!slowly
			var visibleCells = window.GetVisibleCells();
			if( !visibleCells.Intersects( nodeRectWithOneBorder ) )
				return;

			window.GetFontSizes( renderer, out var nodeFontSize, out var nodeFontSizeComment );

			Rectangle nodeRectInUnits = new RectangleI( node.Position, node.Position + nodeSize ).ToRectangle();
			Rectangle nodeRect = window.ConvertUnitToScreen( nodeRectInUnits );
			double cellHeight = window.ConvertUnitToScreenY( node.Position.Y + 1 ) - window.ConvertUnitToScreenY( node.Position.Y );



			//ColorValue totalNodeColorMultiplier = new ColorValue( 1, 1, 1 );
			//{
			//	//if( settings.Error != null )
			//	//	nodeColorMultiplier *= new ColorValue( 1, 0, 0 );

			//	//BlueprintGraphNodeObjectsEvent event_ = node as BlueprintGraphNodeObjectsEvent;
			//	//if( event_ != null && !event_.Enabled )
			//	//	nodeColorMultiplier = new ColorValue( .5f, .5f, .5f );
			//}

			ColorValue nodeColorMultiplierWithSelection = /*totalNodeColorMultiplier*/ GetColorMultiplierSelectionState( selectionStateNode );
			ColorValue objColorMultiplierWithSelection = /*totalNodeColorMultiplier*/GetColorMultiplierSelectionState( selectionStateControlledObject );

			//node selection
			if( selectionStateNode != RenderSelectionState.None )
			{
				//node selection rectangle
				Rectangle nodeSelectionRectInUnits = nodeRectInUnits;
				nodeSelectionRectInUnits.Expand( .4 );
				Rectangle nodeSelectionRect = window.ConvertUnitToScreen( nodeSelectionRectInUnits );

				renderer.AddQuad( nodeSelectionRect, new ColorValue( .3, .3, .3 ) * nodeColorMultiplierWithSelection );
				renderer.AddRectangle( nodeSelectionRect, new ColorValue( .6, .6, .6 ) * nodeColorMultiplierWithSelection );
			}

			//window
			{
				double titleHeight = cellHeight;
				//float titleHeight = ConvertUnitToScreenY( node.NodePosition.Y + 1 ) - ConvertUnitToScreenY( node.NodePosition.Y );
				Rectangle titleRect = new Rectangle( nodeRect.Left, nodeRect.Top, nodeRect.Right, nodeRect.Top + titleHeight );
				Rectangle bodyRect = new Rectangle( nodeRect.Left, nodeRect.Top + titleHeight, nodeRect.Right, nodeRect.Bottom );

				//!!!!
				ColorValue contentTypeColor;
				ColorValue? titleColor = null;
				switch( representationData.NodeContentType )
				{
				case FlowGraphNodeContentType.Flow: contentTypeColor = new ColorValue( 0.16 / 1.2, 0.34 / 1.2, 0.6 / 1.2 ); break;
				case FlowGraphNodeContentType.MethodBody: contentTypeColor = new ColorValue( 0.45, 0.1, 0.1 ); break;
				case FlowGraphNodeContentType.FlowStart: contentTypeColor = new ColorValue( 0.45, 0.1, 0.1 ); break;
				//case FlowchartNodeContentType.Flow: contentTypeColor = new ColorValue( 0.15, 0.15, 0.4 ); break;
				//case FlowchartNodeContentType.MethodBody: contentTypeColor = new ColorValue( 0.4, 0.1, 0.1 ); break;

				//!!!!
				default:
					//contentTypeColor = new ColorValue( 0.3, 0.3, 0.3 );
					//titleColor = new ColorValue( 0.36, 0.36, 0.36 );
					contentTypeColor = new ColorValue( 0.26, 0.26, 0.26 );
					//contentTypeColor = new ColorValue( 0.24, 0.24, 0.24 );
					titleColor = new ColorValue( 0.3, 0.3, 0.3 );
					break;
					//default: contentTypeColor = new ColorValue( 0.3, 0.3, 0.3 ); break;

				}

				if( titleColor == null )
					titleColor = contentTypeColor;

				//!!!!
				//object is disabled
				{
					var c = node.ControlledObject.Value;
					if( c != null && !c.EnabledInHierarchy )
						contentTypeColor *= new ColorValue( 0.5, 0.5, 0.5, 1 );
				}

				if( selectionStateControlledObject != RenderSelectionState.None )
					titleColor = new ColorValue( 0.3, 0.3, 0.3 ) * objColorMultiplierWithSelection;

				//ColorValue titleColor;
				//if( selectionStateControlledObject == EditorRenderSelectionState.None )
				//	titleColor = contentTypeColor;
				//else
				//	titleColor = new ColorValue( 0.3, 0.3, 0.3 ) * objColorMultiplierWithSelection;

				renderer.AddQuad( titleRect, titleColor.Value );
				//renderer.AddQuad( titleRect, new ColorValue( .3, .3, .3 ) * objColorMultiplierWithSelection );

				renderer.AddQuad( bodyRect, contentTypeColor /* totalNodeColorMultiplier*/ );
				//renderer.AddQuad( bodyRect, new ColorValue( .3, .3, .3 ) * totalNodeColorMultiplier );

				//renderer.AddQuad( bodyRect, new ColorValue( 1, 1, 1 ) /*new ColorValue( .3, .3, .3 )*/ * nodeColorMultiplier );
				//renderer.AddQuad( bodyRect, settings.BodyColor /*new ColorValue( .3, .3, .3 )*/ * nodeColorMultiplier );

				//renderer.AddQuad( rect, new ColorValue( .3, .3, .3 ) * nodeColorMultiplierWithSelection );
				renderer.AddRectangle( nodeRect, new ColorValue( .6, .6, .6 ) * objColorMultiplierWithSelection );
				renderer.AddLine(
					window.ConvertUnitToScreen( node.Position.ToVector2() + new Vector2( 0, 1 ) ),
					window.ConvertUnitToScreen( node.Position.ToVector2() + new Vector2( nodeSize.X, 1 ) ),
					new ColorValue( .6, .6, .6 ) * objColorMultiplierWithSelection );

				//if( titleRect.IsContainsPoint( mouse ) )
				//	mouseOverObjects.AddWithCheckAlreadyContained( node );
			}

			//!!!!было
			////BlueprintGraphNodeComment: draw comment
			//if( node is BlueprintGraphNodeComment && !string.IsNullOrEmpty( node.NodeComment ) )
			//{
			//	renderer.AddText( fontComment, node.NodeComment, rectNode.LeftTop, HorizontalAlign.Left, VerticalAlign.Bottom,
			//		settings.CommentColor );
			//}

			if( window.GetZoom() > .25f )
			{
				//Comment
				if( !string.IsNullOrEmpty( node.Comment ) )//!!!!было && !( node is BlueprintGraphNodeComment ) )
				{
					//!!!!style
					var color = new ColorValue( 1, 1, 1 );//settings.CommentColor
					renderer.AddText( window.NodeFontComment, nodeFontSizeComment, node.Comment, nodeRect.LeftTop, EHorizontalAlignment.Left, EVerticalAlignment.Bottom,
						color );
				}

				//title, texts
				{
					renderer.PushClipRectangle( nodeRect );

					//title
					{
						ColorValue titleColorMultiplier = new ColorValue( 1, 1, 1 );

						//!!!!было
						//if( node.IsFlowEntry() && !IsObjectSelected( node ) )
						//	titleColorMultiplier = new ColorValue( 1, 0, 0 );

						Vector2 pos = window.ConvertUnitToScreen( node.Position.ToVector2() + new Vector2( .1f, 0 ) );

						renderer.AddText( window.NodeFont, nodeFontSize, representation.Title, pos, EHorizontalAlignment.Left, EVerticalAlignment.Top,
							titleColorMultiplier * objColorMultiplierWithSelection );
					}

					//!!!!
					////texts
					//if( settings.Texts != null )
					//{
					//	for( int n = 0; n < settings.Texts.Length; n++ )
					//	{
					//		string text = settings.Texts[ n ];
					//		Vec2 pos2 = ConvertUnitToScreen( node.NodePosition.ToVec2() + new Vec2( .3, (float)n + 1 ) );
					//		renderer.AddText( fontValue, text, pos2 + new Vec2( 0, cellHeight * .05f ), EHorizontalAlign.Left,
					//			EVerticalAlign.Top, new ColorValue( .7f, .7f, .7f ) * nodeColorMultiplierWithSelection );
					//	}
					//}

					renderer.PopClipRectangle();
				}

				//client area
				{
					//!!!!

					var texture = representationData.NodeImage;
					if( texture != null )
					{
						Rectangle rect;
						if( representationData.NodeImageView == FlowGraphRepresentationData.NodeImageViewEnum.WideScaled )
						{
							double size = 6;
							rect = new Rectangle(
								node.Position.ToVector2() + new Vector2( 0, representation.Size.Y - size ),
								node.Position.ToVector2() + new Vector2( representation.Size.X, representation.Size.Y ) );

							double border = 0.2;
							rect.Expand( -border );

							Rectangle rectInScreen = window.ConvertUnitToScreen( rect );
							var texCoords = new Rectangle( 0, 0, 1, 1 );

							var textureSize = texture.Result.ResultSize;
							var s = textureSize.ToVector2() / viewport.SizeInPixels.ToVector2();
							var rect2 = new Rectangle( rectInScreen.Left, rectInScreen.Top, rectInScreen.Left + s.X, rectInScreen.Top + s.Y );

							renderer.PushClipRectangle( rectInScreen );
							renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );
							renderer.AddQuad( rect2, new Rectangle( 0, 0, 1, 1 ), texture );
							renderer.PopTextureFilteringMode();
							renderer.PopClipRectangle();
						}
						else
						{
							double size = 5;
							rect = new Rectangle(
								node.Position.ToVector2() + new Vector2( 0, representation.Size.Y - size ),
								node.Position.ToVector2() + new Vector2( size, representation.Size.Y ) );

							double border = 0.2;
							rect.Expand( -border );

							Rectangle rectInScreen = window.ConvertUnitToScreen( rect );
							renderer.AddQuad( rectInScreen, new Rectangle( 0, 0, 1, 1 ), texture );
						}

						//double size = 5;
						//double border = 0.2;
						//Rectangle rect = new Rectangle(
						//	node.Position.ToVector2() + new Vector2( 0, representation.Size.Y - size ),
						//	node.Position.ToVector2() + new Vector2( size, representation.Size.Y ) );
						//rect.Expand( -border );
						//Rectangle rectInScreen = window.ConvertUnitToScreen( rect );

						//renderer.AddQuad( rectInScreen, new Rectangle( 0, 0, 1, 1 ), texture );
					}
				}

				var mouseOverSocket = mouseOverObject as FlowGraphNode.Representation.Connector;

				//items
				for( int nItem = 0; nItem < representation.Items.Count; nItem++ )
				{
					var item = representation.Items[ nItem ];

					ColorValue circleColorNotSpecified = new ColorValue( 0.6, 0.6, 0.6 );
					ColorValue circleColorSpecified = new ColorValue( 1, 1, 1 );
					ColorValue circleColorMouseOver = new ColorValue( 1, 1, 0 );
					ColorValue circleColorCanConnect = new ColorValue( 1, 1, 0 );

					bool isFlow = false;
					{
						var item2 = item as FlowGraphNode.Representation.ItemProperty;
						//!!!!может кешировать, часто бывает
						if( item2 != null && MetadataManager.GetTypeOfNetType( typeof( FlowInput ) ).IsAssignableFrom( item2.Property.TypeUnreferenced ) )
							isFlow = true;
					}

					//draw input circle
					if( item.Input != null )
					{
						Vector2 center = GetSocketPositionInUnits( item, true );
						Rectangle rect = window.ConvertUnitToScreen( new Rectangle( center - new Vector2( .25, .25 ), center + new Vector2( .25, .25 ) ) );

						ColorValue circleColor = circleColorNotSpecified;

						//check reference specified
						var itemProperty = item as FlowGraphNode.Representation.ItemProperty;
						if( itemProperty != null && ReferenceUtility.IsReferenceType( itemProperty.Property.Type.GetNetType() ) )
						{
							Component obj = itemProperty.Owner.Owner.ControlledObject;
							if( obj != null )
							{
								var v = itemProperty.Property.GetValue( obj, null );
								var iReference = v as IReference;
								if( iReference != null && !string.IsNullOrEmpty( iReference.GetByReference ) )
								{
									//!!!!инвалидная может быть

									circleColor = circleColorSpecified;
								}
							}
						}

						if( mouseOverSocket != null && mouseOverSocket.item == item && mouseOverSocket.input && referenceCreationSocketFrom == null &&
							dragDropSetReferenceData == null )
							circleColor = circleColorMouseOver;
						if( referenceCreationSocketFrom != null && window.CanCreateReference( item.Input, referenceCreationSocketFrom ) )
							circleColor = circleColorCanConnect;
						if( isFlow && dragDropSetReferenceData != null && window.CanCreateReferenceDragDropSetReference( (DragDropSetReferenceData)dragDropSetReferenceData, item.Input, out string[] dummy ) )
						{
							circleColor = circleColorCanConnect;
						}

						//circleColor *= totalNodeColorMultiplier;

						//!!!!
						//if( !nodeMove && !IsSelectionByRectangleActivated() )
						//{
						//	if( ( referenceCreationSocketFrom != null && IsAllowMakeLink( referenceCreationSocketFrom, socket ) ) ||
						//		( referenceCreationSocketFrom == null && mouseOverObject == socket ) || referenceCreationSocketFrom == socket )
						//		circleColor = allowMakeLinkColorForPinCircle;
						//}

						string textureName;
						if( isFlow )
							textureName = "Base\\Tools\\FlowGraphEditor\\FlowPin.png";
						else
							textureName = "Base\\Tools\\FlowGraphEditor\\CirclePin.png";
						var texture = ResourceManager.LoadResource<ImageComponent>( textureName );
						renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture, circleColor );

						//!!!!
						//if( !nodeMove )
						//{
						//	if( rect.IsContainsPoint( mouse ) )
						//		mouseOverObjects.Add( socket );
						//}
					}

					//draw output circle
					if( item.Output != null )
					{
						Vector2 center = GetSocketPositionInUnits( item, false );
						Rectangle rect = window.ConvertUnitToScreen( new Rectangle( center - new Vector2( .25, .25 ), center + new Vector2( .25, .25 ) ) );

						ColorValue circleColor = circleColorNotSpecified;

						//check reference specified
						if( isFlow )
						{
							var itemProperty = item as FlowGraphNode.Representation.ItemProperty;
							if( itemProperty != null && ReferenceUtility.IsReferenceType( itemProperty.Property.Type.GetNetType() ) )
							{
								Component obj = itemProperty.Owner.Owner.ControlledObject;
								if( obj != null )
								{
									var v = itemProperty.Property.GetValue( obj, null );
									var iReference = v as IReference;
									if( iReference != null && !string.IsNullOrEmpty( iReference.GetByReference ) )
									{
										//!!!!инвалидная может быть

										circleColor = circleColorSpecified;
									}
								}
							}
						}

						if( mouseOverSocket != null && mouseOverSocket.item == item && !mouseOverSocket.input && referenceCreationSocketFrom == null &&
							dragDropSetReferenceData == null )
							circleColor = circleColorMouseOver;
						if( referenceCreationSocketFrom != null && window.CanCreateReference( item.Output, referenceCreationSocketFrom ) )
							circleColor = circleColorCanConnect;
						if( !isFlow && dragDropSetReferenceData != null && window.CanCreateReferenceDragDropSetReference( (DragDropSetReferenceData)dragDropSetReferenceData, item.Output, out string[] dummy ) )
						{
							circleColor = circleColorCanConnect;
						}

						//circleColor *= totalNodeColorMultiplier;

						//!!!!
						//if( !nodeMove && !IsSelectionByRectangleActivated() )
						//{
						//	if( ( referenceCreationSocketFrom != null && IsAllowMakeLink( referenceCreationSocketFrom, socket ) ) ||
						//		( referenceCreationSocketFrom == null && mouseOverObject == socket ) || referenceCreationSocketFrom == socket )
						//		circleColor = allowMakeLinkColorForPinCircle;
						//}

						string textureName;
						if( isFlow )
							textureName = "Base\\Tools\\FlowGraphEditor\\FlowPin.png";
						else
							textureName = "Base\\Tools\\FlowGraphEditor\\CirclePin.png";
						var texture = ResourceManager.LoadResource<ImageComponent>( textureName );
						renderer.AddQuad( rect, new Rectangle( 0, 0, 1, 1 ), texture, circleColor );

						//!!!!
						//if( !nodeMove )
						//{
						//	if( rect.IsContainsPoint( mouse ) )
						//		mouseOverObjects.Add( socket );
						//}
					}

					//draw text
					//!!!!
					//if( !( socket.Name == "Flow" && socket.Type == "Flow" ) )
					var itemDisplayName = item.DisplayName;
					if( !string.IsNullOrEmpty( itemDisplayName ) )
					{
						renderer.PushClipRectangle( nodeRect );

						//!!!!
						string pinText = TypeUtility.DisplayNameAddSpaces( itemDisplayName );
						//string pinText = TypeUtils.DisplayNameAddSpaces( socket.DisplayName ) + " ";
						//string socketText = ( ( socket.DisplayName != null ) ? socket.DisplayName : socket.Name ) + " ";

						if( item.Input != null )
						{
							var pinText2 = pinText;
							//display property values
							//if( !isFlow )
							//{
							//	var itemProperty = item as FlowGraphNode.Representation.ItemProperty;
							//	if( itemProperty != null && itemProperty.Property.Indexers.Length == 0 )
							//	{
							//		try
							//		{
							//			var value = itemProperty.Property.GetValue( itemProperty.Owner.Owner.ControlledObject.Value, new object[ 0 ] );
							//			value = ReferenceUtility.GetUnreferencedValue( value );
							//			pinText2 += string.Format( " ({0})", value != null ? value.ToString() : "null" );
							//		}
							//		catch { }
							//	}
							//}

							//input
							Vector2 pos = window.ConvertUnitToScreen( node.Position.ToVector2() +
								new Vector2( .3, (double)item.Index/* nItem*/ + /*1 +*/ representation.GetTextsOffsetY() ) );
							renderer.AddText( window.NodeFont, nodeFontSize, pinText2, pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) /* totalNodeColorMultiplier */);
						}
						else
						{
							//output
							Vector2 pos = window.ConvertUnitToScreen( node.Position.ToVector2() +
								new Vector2( representation.Size.X - .3, (float)item.Index/*nItem*/ + /*1 +*/ representation.GetTextsOffsetY() ) );
							renderer.AddText( window.NodeFont, nodeFontSize, pinText, pos, EHorizontalAlignment.Right, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) /* totalNodeColorMultiplier */);
						}

						renderer.PopClipRectangle();
					}
				}
			}
		}

		public override object GetMouseOverObject( IFlowGraphEditor window2, FlowGraphNode node )
		{
			var window = (FlowGraphEditor)window2;

			var representation = node.GetRepresentation();
			var viewport = window.ViewportControl2.Viewport;
			var mouse = viewport.MousePosition;
			var renderer = viewport.CanvasRenderer;

			Vector2I nodeSize = representation.Size;
			RectangleI nodeRectWithOneBorder = new RectangleI( node.Position - new Vector2I( 1, 1 ), node.Position + nodeSize + new Vector2I( 1, 1 ) );

			//!!!!slowly
			var visibleCells = window.GetVisibleCells();
			if( !visibleCells.Intersects( nodeRectWithOneBorder ) )
				return null;

			Rectangle nodeRectInUnits = new RectangleI( node.Position, node.Position + nodeSize ).ToRectangle();
			Rectangle nodeRect = window.ConvertUnitToScreen( nodeRectInUnits );
			double cellHeight = window.ConvertUnitToScreenY( node.Position.Y + 1 ) - window.ConvertUnitToScreenY( node.Position.Y );

			//sockets
			if( window.GetZoom() > .25f )
			{
				for( int nSocket = 0; nSocket < representation.Items.Count; nSocket++ )
				{
					var socket = representation.Items[ nSocket ];

					Vector2 size = new Vector2( .8, .8 );

					//input circle
					if( socket.Input != null )
					{
						Vector2 center = GetSocketPositionInUnits( socket, true );
						Rectangle rect = window.ConvertUnitToScreen( new Rectangle( center - size / 2, center + size / 2 ) );
						if( rect.Contains( mouse ) )
							return new FlowGraphNode.Representation.Connector( socket, true );
					}

					//output circle
					if( socket.Output != null )
					{
						Vector2 center = GetSocketPositionInUnits( socket, false );
						Rectangle rect = window.ConvertUnitToScreen( new Rectangle( center - size / 2, center + size / 2 ) );
						if( rect.Contains( mouse ) )
							return new FlowGraphNode.Representation.Connector( socket, false );
					}
				}
			}

			//node
			{
				Rectangle titleRect = new Rectangle( nodeRect.Left, nodeRect.Top, nodeRect.Right, nodeRect.Top + cellHeight );
				if( titleRect.Contains( mouse ) )
					return node;
			}

			return null;
		}

		public override bool IsIntersectsWithRectangle( IFlowGraphEditor window2, FlowGraphNode node, Rectangle rectInUnits )
		{
			var window = (FlowGraphEditor)window2;

			//!!!!slowly?

			var representation = node.GetRepresentation();
			var viewport = window.ViewportControl2.Viewport;
			var mouse = viewport.MousePosition;
			var renderer = viewport.CanvasRenderer;

			Vector2I nodeSize = representation.Size;
			RectangleI nodeRectWithOneBorder = new RectangleI( node.Position - new Vector2I( 1, 1 ), node.Position + nodeSize + new Vector2I( 1, 1 ) );

			//!!!!slowly
			var visibleCells = window.GetVisibleCells();
			if( !visibleCells.Intersects( nodeRectWithOneBorder ) )
				return false;

			Rectangle nodeRectInUnits = new RectangleI( node.Position, node.Position + nodeSize ).ToRectangle();

			return rectInUnits.Intersects( nodeRectInUnits );
		}
#endif
	}
}
