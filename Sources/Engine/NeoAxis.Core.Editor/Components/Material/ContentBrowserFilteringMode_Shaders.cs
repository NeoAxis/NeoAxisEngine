#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

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

		//public override bool AddGroupGeneral
		//{
		//	get { return false; }
		//}

		public override bool AddGroupsBaseTypesAddonsProject
		{
			get { return false; }
		}

		public override bool AddGroupFavorites
		{
			get { return false; }
		}

		//public override bool AddSolution
		//{
		//	get { return false; }
		//}

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

			//ShaderTextureSample, ShaderParameter
			var typeItem = item as ContentBrowserItem_Type;
			if( typeItem != null )
			{
				var netType = typeItem.Type.GetNetType();
				if( typeof( ShaderTextureSample ).IsAssignableFrom( netType ) ||
					typeof( ShaderParameter ).IsAssignableFrom( netType ) ||
					typeof( ShaderScript ).IsAssignableFrom( netType ) )
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
		public override void OnRegister()
		{
			ContentBrowser.FilteringModes.Add( new ContentBrowserFilteringMode_Shaders() );
		}
	}
}
#endif