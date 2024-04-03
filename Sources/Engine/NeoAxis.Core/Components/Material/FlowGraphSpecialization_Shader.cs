// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Flow graph specialization for shaders visual creation. Specialization affects workflow with graph in editor.
	/// </summary>
	public class FlowGraphSpecialization_Shader : FlowGraphSpecialization
	{
		static FlowGraphSpecialization_Shader instance;
		/// <summary>
		/// The singleton object of a component.
		/// </summary>
		public static FlowGraphSpecialization_Shader Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new FlowGraphSpecialization_Shader();
					instance.Name = "Shader";
				}
				return instance;
			}
		}

		/////////////////////////////////////////

		public override void DragDropObjectCreateInitNode( FlowGraphNode node, DragDropObjectCreateInitNodeContext context, ref bool handled )
		{
			//ShaderTextureSample
			if( context.createComponentType != null &&
				MetadataManager.GetTypeOfNetType( typeof( ImageComponent ) ).IsAssignableFrom( context.createComponentType ) )
			{
				var obj = node.CreateComponent<ShaderTextureSample>();
				obj.Name = "Texture Sample";

				//!!!!если не из ресурса
				obj.Texture = ReferenceUtility.MakeReference<ImageComponent>( null, context.createComponentType.Name );

				context.controlledObject = obj;
				handled = true;
			}
		}
	}
}
