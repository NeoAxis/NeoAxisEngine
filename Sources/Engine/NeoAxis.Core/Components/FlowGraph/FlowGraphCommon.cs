// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The list of flow graph node types.
	/// </summary>
	public enum FlowGraphNodeContentType
	{
		Default,
		Flow,
		MethodBody,
		FlowStart,
		//!!!!что еще?
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a data provided to flow graph editor by objects.
	/// </summary>
	public class FlowGraphRepresentationData
	{
		public string NodeTitle;
		public ImageComponent NodeImage;

		public enum NodeImageViewEnum
		{
			Small,
			WideScaled,//so special
		}
		public NodeImageViewEnum NodeImageView = NodeImageViewEnum.Small;

		public FlowGraphNodeContentType NodeContentType;
		public int NodeHeight;
	}

	/// <summary>
	/// An interface to provide <see cref="FlowGraphRepresentationData"/>.
	/// </summary>
	public interface IFlowGraphRepresentationData
	{
		void GetFlowGraphRepresentationData( FlowGraphRepresentationData data );

		//string FlowchartNodeTitle { get; }

		////!!!!пока так
		//Texture FlowchartNodeRenderTexture { get; }
		////void FlowchartNodeRenderClientArea();

		//FlowchartNodeContentType FlowchartNodeContentType { get; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Attribute to configure the visibility of an type in flow graph editor.
	/// </summary>
	public class FlowGraphBrowsable : Attribute
	{
		bool browsable;

		public FlowGraphBrowsable( bool browsable )
		{
			this.browsable = browsable;
		}

		public bool Browsable
		{
			get { return browsable; }
		}
	}
}
