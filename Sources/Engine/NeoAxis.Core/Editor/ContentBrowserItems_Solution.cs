#if !DEPLOY
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
using System.Linq;

namespace NeoAxis.Editor
{
	public class ContentBrowserItem_Solution : ContentBrowser.Item
	{
		List<ContentBrowser.Item> children = new List<ContentBrowser.Item>();

		//

		public ContentBrowserItem_Solution( ContentBrowser owner, ContentBrowser.Item parent )
			: base( owner, parent )
		{
		}

		public override void Dispose()
		{
			if( children != null )
			{
				foreach( var item in children )
					item.Dispose();
			}
		}

		public override string Text
		{
			get { return "Solution"; }
		}

		void UpdateChildren()
		{
			//!!!!

			////get members
			//var members = new List<Metadata.Member>( 256 );
			//{
			//	foreach( var member in type.MetadataGetMembers() )
			//	{
			//		bool allow;
			//		if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
			//		{
			//			if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
			//				allow = false;
			//			else
			//			{
			//				var demandedType = Owner.SetReferenceModeData.DemandedType;
			//				allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true );
			//			}
			//		}
			//		else
			//		{
			//			if( member.Owner == type )
			//				allow = true;
			//			else
			//				allow = false;
			//			//allow = true;
			//		}

			//		if( allow )
			//			members.Add( member );
			//	}

			//	//sort
			//	ContentBrowserUtility.SortMemberItems( members );
			//}

			//create member items
			children.Clear();

			//{
			//	var item = new ContentBrowserItem_SolutionNew( Owner, this, "New" );
			//	item.imageKey = "New";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}


			////!!!!
			//VisualStudioSolutionUtility.ReadSolutionData( out var error, out var projects );

			//if( projects != null )
			//{
			//	foreach( var name in projects )
			//	{
			//		var item = new ContentBrowserItem_CSharpProject( Owner, this, name );
			//		item.imageKey = "CSharpProject";
			//		//!!!!
			//		//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//		children.Add( item );
			//		//else
			//		//	item.Dispose();
			//	}
			//}


			//{
			//	var item = new ContentBrowserItem_CSharpProject( Owner, this, "Project 1" );
			//	item.imageKey = "CSharpProject";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}

			//{
			//	var item = new ContentBrowserItem_CSharpProject( Owner, this, "Project 2" );
			//	item.imageKey = "CSharpProject";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}

			//!!!!FilteringMode
			//foreach( var member in members )
			//{
			//	var item = new ContentBrowserItem_Member( Owner, this, member );

			//	if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//		children.Add( item );
			//	else
			//		item.Dispose();
			//}
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				//if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//{
				//if( !memberCreationDisable )
				UpdateChildren();
				//}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( children.Count );
			foreach( var item in children )
				result.Add( item );
			return result;
		}

		//!!!!
		//public override string GetDescription()
		//{
		//	return Description;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!name
	//public class ContentBrowserItem_CSharpProject : ContentBrowserItem_Project
	public class ContentBrowserItem_CSharpProject : ContentBrowser.Item
	{
		//!!!!
		string text;
		List<ContentBrowser.Item> children = new List<ContentBrowser.Item>();

		//

		public ContentBrowserItem_CSharpProject( ContentBrowser owner, ContentBrowser.Item parent, string text )
			: base( owner, parent )
		{
			this.text = text;
		}

		public override void Dispose()
		{
			if( children != null )
			{
				foreach( var item in children )
					item.Dispose();
			}
		}

		public override string Text
		{
			get { return text; }
		}

		void UpdateChildren()
		{
			//!!!!

			////get members
			//var members = new List<Metadata.Member>( 256 );
			//{
			//	foreach( var member in type.MetadataGetMembers() )
			//	{
			//		bool allow;
			//		if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
			//		{
			//			if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
			//				allow = false;
			//			else
			//			{
			//				var demandedType = Owner.SetReferenceModeData.DemandedType;
			//				allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true );
			//			}
			//		}
			//		else
			//		{
			//			if( member.Owner == type )
			//				allow = true;
			//			else
			//				allow = false;
			//			//allow = true;
			//		}

			//		if( allow )
			//			members.Add( member );
			//	}

			//	//sort
			//	ContentBrowserUtility.SortMemberItems( members );
			//}

			//create member items
			children.Clear();

			{
				var item = new ContentBrowserItem_Virtual( Owner, this, "Properties" );
				item.imageKey = "Property";
				//!!!!
				//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
				children.Add( item );
				//else
				//	item.Dispose();
			}

			{
				var item = new ContentBrowserItem_CSharpProjectReferences( Owner, this, "References" );
				item.imageKey = "Attach";
				//!!!!
				//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
				children.Add( item );
				//else
				//	item.Dispose();
			}

			{
				var item = new ContentBrowserItem_Virtual( Owner, this, "File.cs" );
				item.imageKey = "CSharp";
				//!!!!
				//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
				children.Add( item );
				//else
				//	item.Dispose();
			}

			{
				var item = new ContentBrowserItem_Virtual( Owner, this, "File2.cs" );
				item.imageKey = "CSharp";
				//!!!!
				//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
				children.Add( item );
				//else
				//	item.Dispose();
			}


			//!!!!FilteringMode
			//foreach( var member in members )
			//{
			//	var item = new ContentBrowserItem_Member( Owner, this, member );

			//	if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//		children.Add( item );
			//	else
			//		item.Dispose();
			//}
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				//if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//{
				//if( !memberCreationDisable )
				UpdateChildren();
				//}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( children.Count );
			foreach( var item in children )
				result.Add( item );
			return result;
		}

		//!!!!
		//public override string GetDescription()
		//{
		//	return Description;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContentBrowserItem_CSharpProjectReferences : ContentBrowser.Item
	{
		//!!!!
		string text;
		List<ContentBrowser.Item> children = new List<ContentBrowser.Item>();

		//

		public ContentBrowserItem_CSharpProjectReferences( ContentBrowser owner, ContentBrowser.Item parent, string text )
			: base( owner, parent )
		{
			this.text = text;
		}

		public override void Dispose()
		{
			if( children != null )
			{
				foreach( var item in children )
					item.Dispose();
			}
		}

		public override string Text
		{
			get { return text; }
		}

		void UpdateChildren()
		{
			//!!!!

			////get members
			//var members = new List<Metadata.Member>( 256 );
			//{
			//	foreach( var member in type.MetadataGetMembers() )
			//	{
			//		bool allow;
			//		if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
			//		{
			//			if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
			//				allow = false;
			//			else
			//			{
			//				var demandedType = Owner.SetReferenceModeData.DemandedType;
			//				allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true );
			//			}
			//		}
			//		else
			//		{
			//			if( member.Owner == type )
			//				allow = true;
			//			else
			//				allow = false;
			//			//allow = true;
			//		}

			//		if( allow )
			//			members.Add( member );
			//	}

			//	//sort
			//	ContentBrowserUtility.SortMemberItems( members );
			//}

			//create member items
			children.Clear();

			{
				var item = new ContentBrowserItem_Virtual( Owner, this, "System" );
				//!!!!
				item.imageKey = "Attach";
				children.Add( item );
			}

			{
				var item = new ContentBrowserItem_Virtual( Owner, this, "NeoAxis.Core" );
				//!!!!
				item.imageKey = "Attach";
				children.Add( item );
			}

			//{
			//	var item = new ContentBrowserItem_Virtual( Owner, this, "References" );
			//	item.imageKey = "Link";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}

			//{
			//	var item = new ContentBrowserItem_Virtual( Owner, this, "File.cs" );
			//	item.imageKey = "CSharp";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}

			//{
			//	var item = new ContentBrowserItem_Virtual( Owner, this, "File2.cs" );
			//	item.imageKey = "CSharp";
			//	//!!!!
			//	//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//	children.Add( item );
			//	//else
			//	//	item.Dispose();
			//}


			//!!!!FilteringMode
			//foreach( var member in members )
			//{
			//	var item = new ContentBrowserItem_Member( Owner, this, member );

			//	if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
			//		children.Add( item );
			//	else
			//		item.Dispose();
			//}
		}

		public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
		{
			if( !onlyAlreadyCreated )
			{
				//if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
				//{
				//if( !memberCreationDisable )
				UpdateChildren();
				//}
			}

			//prepare result
			List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( children.Count );
			foreach( var item in children )
				result.Add( item );
			return result;
		}

		//!!!!
		//public override string GetDescription()
		//{
		//	return Description;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public class ContentBrowserItem_SolutionNew : ContentBrowser.Item
	//{
	//	//!!!!
	//	string text;
	//	List<ContentBrowser.Item> children = new List<ContentBrowser.Item>();

	//	//

	//	public ContentBrowserItem_SolutionNew( ContentBrowser owner, ContentBrowser.Item parent, string text )
	//		: base( owner, parent )
	//	{
	//		this.text = text;
	//	}

	//	public override void Dispose()
	//	{
	//		if( children != null )
	//		{
	//			foreach( var item in children )
	//				item.Dispose();
	//		}
	//	}

	//	public override string Text
	//	{
	//		get { return text; }
	//	}

	//	void UpdateChildren()
	//	{
	//		//!!!!

	//		////get members
	//		//var members = new List<Metadata.Member>( 256 );
	//		//{
	//		//	foreach( var member in type.MetadataGetMembers() )
	//		//	{
	//		//		bool allow;
	//		//		if( Owner.Mode == ContentBrowser.ModeEnum.SetReference )
	//		//		{
	//		//			if( Owner.SetReferenceModeData.newObjectWindow || Owner.SetReferenceModeData.selectTypeWindow )
	//		//				allow = false;
	//		//			else
	//		//			{
	//		//				var demandedType = Owner.SetReferenceModeData.DemandedType;
	//		//				allow = ContentBrowserUtility.ContentBrowserSetReferenceModeCheckAllowAddMember( demandedType, member, true );
	//		//			}
	//		//		}
	//		//		else
	//		//		{
	//		//			if( member.Owner == type )
	//		//				allow = true;
	//		//			else
	//		//				allow = false;
	//		//			//allow = true;
	//		//		}

	//		//		if( allow )
	//		//			members.Add( member );
	//		//	}

	//		//	//sort
	//		//	ContentBrowserUtility.SortMemberItems( members );
	//		//}

	//		//create member items
	//		children.Clear();

	//		var strings = new List<string>();
	//		strings.Add( "C# Windows Forms App" );
	//		strings.Add( "C# WPF App" );
	//		strings.Add( "C# Class Library" );
	//		strings.Add( "NeoAxis Editor App" );
	//		strings.Add( "NeoAxis Simulation App" );
	//		//strings.Add( "Create C# Windows Forms App" );
	//		//strings.Add( "Create C# WPF App" );
	//		//strings.Add( "Create C# Class Library" );
	//		//strings.Add( "Create NeoAxis Editor App" );
	//		//strings.Add( "Create NeoAxis Simulation App" );

	//		foreach( var str in strings )
	//		{
	//			var item = new ContentBrowserItem_Virtual( Owner, this, str );
	//			item.imageKey = "Default";
	//			//!!!!
	//			//if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
	//			children.Add( item );
	//			//else
	//			//	item.Dispose();
	//		}

	//		//!!!!FilteringMode
	//		//foreach( var member in members )
	//		//{
	//		//	var item = new ContentBrowserItem_Member( Owner, this, member );

	//		//	if( Owner.FilteringMode == null || Owner.FilteringMode.AddItem( item ) )
	//		//		children.Add( item );
	//		//	else
	//		//		item.Dispose();
	//		//}
	//	}

	//	public override IList<ContentBrowser.Item> GetChildren( bool onlyAlreadyCreated )
	//	{
	//		if( !onlyAlreadyCreated )
	//		{
	//			//if( Owner.Mode == ContentBrowser.ModeEnum.Resources || Owner.Mode == ContentBrowser.ModeEnum.SetReference )
	//			//{
	//			//if( !memberCreationDisable )
	//			UpdateChildren();
	//			//}
	//		}

	//		//prepare result
	//		List<ContentBrowser.Item> result = new List<ContentBrowser.Item>( children.Count );
	//		foreach( var item in children )
	//			result.Add( item );
	//		return result;
	//	}

	//	//!!!!
	//	//public override string GetDescription()
	//	//{
	//	//	return Description;
	//	//}
	//}

}

#endif