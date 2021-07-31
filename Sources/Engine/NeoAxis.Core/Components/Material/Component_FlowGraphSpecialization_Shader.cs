// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// Flow graph specialization for shaders visual creation. Specialization affects workflow with graph in editor.
	/// </summary>
	public class Component_FlowGraphSpecialization_Shader : Component_FlowGraphSpecialization
	{
		static Component_FlowGraphSpecialization_Shader instance;
		/// <summary>
		/// The singleton object of a component.
		/// </summary>
		public static Component_FlowGraphSpecialization_Shader Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new Component_FlowGraphSpecialization_Shader();
					instance.Name = "Shader";
				}
				return instance;
			}
		}

		/////////////////////////////////////////

		public override void DragDropObjectCreateInitNode( Component_FlowGraphNode node, DragDropObjectCreateInitNodeContext context, ref bool handled )
		{
			//Component_ShaderTextureSample
			if( context.createComponentType != null &&
				MetadataManager.GetTypeOfNetType( typeof( Component_Image ) ).IsAssignableFrom( context.createComponentType ) )
			{
				var obj = node.CreateComponent<Component_ShaderTextureSample>();
				obj.Name = "Texture Sample";

				//!!!!если не из ресурса
				obj.Texture = ReferenceUtility.MakeReference<Component_Image>( null, context.createComponentType.Name );

				context.controlledObject = obj;
				handled = true;
			}


		}
	}
}
