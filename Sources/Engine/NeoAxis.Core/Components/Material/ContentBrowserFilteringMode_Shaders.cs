// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace NeoAxis.Editor
{
	public class ContentBrowserFilteringMode_Shaders : ContentBrowserFilteringMode
	{
		public ContentBrowserFilteringMode_Shaders()
		{
		}

		public override string Name
		{
			get { return "Shaders"; }
		}

		public override bool AddGroupGeneral
		{
			get { return false; }
		}

		public override bool AddGroupsBaseTypesAddonsProject
		{
			get { return false; }
		}

		public override bool AddGroupsFavorites
		{
			get { return true; }
		}

		public override bool AddSolution
		{
			get { return false; }
		}

		public override bool AddGroupAllTypes
		{
			get { return true; }
		}

		public override bool AddFiles
		{
			get { return true; }
		}

		public override bool AddItem( ContentBrowser.Item item )
		{
			//members with ShaderGenerationFunctionAttribute
			var memberItem = item as ContentBrowserItem_Member;
			if( memberItem != null )
			{
				if( memberItem.Member.GetCustomAttributes( typeof( ShaderGenerationFunctionAttribute ) ).Length != 0 )
					return true;
				else
					return false;
			}

			//Component_ShaderTextureSample, Component_ShaderParameter
			var typeItem = item as ContentBrowserItem_Type;
			if( typeItem != null )
			{
				var netType = typeItem.Type.GetNetType();
				if( typeof( Component_ShaderTextureSample ).IsAssignableFrom( netType ) ||
					typeof( Component_ShaderParameter ).IsAssignableFrom( netType ) ||
					typeof( Component_ShaderScript ).IsAssignableFrom( netType ) )
				{
					return true;
				}
				else
					return false;
			}

			return false;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class EditorExtensions_Shaders : EditorExtensions
	{
		public override void Register()
		{
			ContentBrowser.FilteringModes.Add( new ContentBrowserFilteringMode_Shaders() );
		}
	}
}
