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

namespace NeoAxis.Editor
{
	public class MessageLogOptions : Metadata.IMetadataProvider
	{
		[DefaultValue( Orientation.Vertical )]
		public Orientation SplitterOrientation { get; set; } = Orientation.Vertical;

		//

		public MessageLogOptions()
		{
		}

		[Browsable( false )]
		public Metadata.TypeInfo BaseType
		{
			get { return MetadataManager.GetTypeOfNetType( GetType() ); }
		}

		//void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		//{
		//	var p = member as Metadata.Property;
		//	if( p != null )
		//	{
		//		switch( p.Name )
		//		{
		//		case nameof( PanelMode_Resources ):
		//			if( owner.Mode != ContentBrowser.ModeEnum.Resources )
		//				skip = true;
		//			break;
		//		case nameof( PanelMode_Objects ):
		//			if( owner.Mode != ContentBrowser.ModeEnum.Objects )
		//				skip = true;
		//			break;
		//		case nameof( PanelMode_SetReference ):
		//			if( owner.Mode != ContentBrowser.ModeEnum.SetReference )
		//				skip = true;
		//			break;

		//		case nameof( ListMode ):
		//		case nameof( ListIconSize ):
		//		case nameof( Breadcrumb ):
		//			if( PanelMode == ContentBrowser.PanelModeEnum.Tree )
		//				skip = true;
		//			break;

		//		case nameof( EditorButton ):
		//		case nameof( SettingsButton ):
		//			if( !DisplayPropertiesEditorSettingsButtons )
		//				skip = true;
		//			break;

		//		case nameof( FilteringModeButton ):
		//			if( owner.Mode != ContentBrowser.ModeEnum.Resources )
		//				skip = true;
		//			break;

		//		case nameof( MembersButton ):
		//			if( owner.Mode != ContentBrowser.ModeEnum.Objects )
		//				skip = true;
		//			break;

		//		case nameof( SortFilesBy ):
		//		case nameof( SortFilesByAscending ):
		//			if( !DisplayPropertiesSortFilesBy )
		//				skip = true;
		//			break;

		//		case nameof( OpenButton ):
		//			if( !DisplayPropertiesOpenButton )
		//				skip = true;
		//			break;
		//		}
		//	}
		//}

		public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
		{
			foreach( var m in BaseType.MetadataGetMembers( context ) )
			{
				//bool skip = false;
				//if( context == null || context.filter )
				//	MetadataGetMembersFilter( context, m, ref skip );
				//if( !skip )
					yield return m;
			}
		}

		public Metadata.Member MetadataGetMemberBySignature( string signature, Metadata.GetMembersContext context = null )
		{
			return BaseType.MetadataGetMemberBySignature( signature, context );
		}

		public void Load( TextBlock block )
		{
			try
			{
				if( block.AttributeExists( nameof( SplitterOrientation ) ) )
					SplitterOrientation = (Orientation)Enum.Parse( typeof( Orientation ), block.GetAttribute( nameof( SplitterOrientation ) ) );
			}
			catch { }
		}

		public void Save( TextBlock block )
		{
			block.SetAttribute( nameof( SplitterOrientation ), SplitterOrientation.ToString() );
		}
	}
}

#endif