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
	/// <summary>
	/// Represents options for <see cref="ContentBrowser"/>.
	/// </summary>
	public class ContentBrowserOptions : Metadata.IMetadataProvider
	{
		ContentBrowser owner;

		/////////////////////////////////////////

		[Browsable( false )]
		public ContentBrowser.PanelModeEnum PanelMode { get; set; } = ContentBrowser.PanelModeEnum.Tree;

		[DefaultValue( ContentBrowser.PanelModeEnum.TwoPanelsSplitHorizontally )]
		[DisplayName( "Panel Mode" )]
		public ContentBrowser.PanelModeEnum PanelMode_Resources { get { return PanelMode; } set { PanelMode = value; } }

		[DefaultValue( ContentBrowser.PanelModeEnum.Tree )]
		[DisplayName( "Panel Mode" )]
		public ContentBrowser.PanelModeEnum PanelMode_Objects { get { return PanelMode; } set { PanelMode = value; } }

		[DefaultValue( ContentBrowser.PanelModeEnum.TwoPanelsSplitHorizontally )]
		[DisplayName( "Panel Mode" )]
		public ContentBrowser.PanelModeEnum PanelMode_SetReference { get { return PanelMode; } set { PanelMode = value; } }

		//[Browsable( false )]
		//[DefaultValue( true )]
		//public bool OptionsButton { get; set; } = true;

		[DefaultValue( true )]
		public bool FilteringModeButton { get; set; } = true;

		[DefaultValue( true )]
		public bool MembersButton { get; set; } = true;

		[DefaultValue( true )]
		public bool OpenButton { get; set; } = true;

		[DefaultValue( true )]
		public bool EditorButton { get; set; } = true;

		[DefaultValue( true )]
		public bool SettingsButton { get; set; } = true;

		[DefaultValue( true )]
		public bool ButtonsForEditing { get; set; } = true;

		[DefaultValue( true )]
		public bool SearchButton { get; set; } = true;

		//!!!!
		//!!!!вернуть componentsBrowser.Options.SearchBar = false; в SettingsHeader_Components.cs
		//!!!!load config
		[DefaultValue( false )]
		public bool SearchBar { get; } = false;
		//[DefaultValue( true )]
		//public bool SearchBar { get; set; } = true;

		[DefaultValue( ContentBrowser.SortByItems.Name )]
		public ContentBrowser.SortByItems SortFilesBy { get; set; } = ContentBrowser.SortByItems.Name;

		[DefaultValue( true )]
		public bool SortFilesByAscending { get; set; } = true;

		[DefaultValue( true )]
		public bool Breadcrumb { get; set; } = true;

		[DefaultValue( ContentBrowser.ListModeEnum.Auto )]
		public ContentBrowser.ListModeEnum ListMode { get; set; } = ContentBrowser.ListModeEnum.Auto;

		const int ListImageSizeDefault = 26;//32;
		[DefaultValue( ListImageSizeDefault )]
		[Range( 10/*16*/, 128 )]
		public int ListImageSize { get; set; } = ListImageSizeDefault;

		const int ListColumnWidthDefault = 1000;
		[DefaultValue( ListColumnWidthDefault )]
		[Range( 50/*64*/, 2000 )]
		public int ListColumnWidth { get; set; } = ListColumnWidthDefault;

		const int TileImageSizeDefault = 26;//32;//48;
		[DefaultValue( TileImageSizeDefault )]
		[Range( 10/*16*/, 128 )]
		public int TileImageSize { get; set; } = TileImageSizeDefault;

		[Range( 0, 1 )]
		[DefaultValue( 0.6 )]
		public double SplitterPosition { get; set; } = 0.6;

		/////////////////////////////////////////

		public ContentBrowserOptions( ContentBrowser owner )
		{
			this.owner = owner;
		}

		[Browsable( false )]
		public bool DisplayPropertiesEditorSettingsButtons { get; set; } = true;

		[Browsable( false )]
		public bool DisplayPropertiesSortFilesBy { get; set; } = true;

		[Browsable( false )]
		public bool DisplayPropertiesOpenButton { get; set; } = true;

		[Browsable( false )]
		public Metadata.TypeInfo BaseType
		{
			get { return MetadataManager.GetTypeOfNetType( GetType() ); }
		}

		void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( PanelMode_Resources ):
					if( owner.Mode != ContentBrowser.ModeEnum.Resources )
						skip = true;
					break;
				case nameof( PanelMode_Objects ):
					if( owner.Mode != ContentBrowser.ModeEnum.Objects )
						skip = true;
					break;
				case nameof( PanelMode_SetReference ):
					if( owner.Mode != ContentBrowser.ModeEnum.SetReference )
						skip = true;
					break;

				case nameof( ListMode ):
				case nameof( TileImageSize ):
				case nameof( ListImageSize ):
				case nameof( ListColumnWidth ):
				case nameof( Breadcrumb ):
					if( PanelMode == ContentBrowser.PanelModeEnum.Tree )
						skip = true;
					break;

				case nameof( EditorButton ):
				case nameof( SettingsButton ):
					if( !DisplayPropertiesEditorSettingsButtons )
						skip = true;
					break;

				case nameof( FilteringModeButton ):
					if( owner.Mode != ContentBrowser.ModeEnum.Resources )
						skip = true;
					break;

				case nameof( MembersButton ):
					if( owner.Mode != ContentBrowser.ModeEnum.Objects )
						skip = true;
					break;

				case nameof( SortFilesBy ):
				case nameof( SortFilesByAscending ):
					if( !DisplayPropertiesSortFilesBy )
						skip = true;
					break;

				case nameof( OpenButton ):
					if( !DisplayPropertiesOpenButton )
						skip = true;
					break;

				case nameof( SearchButton ):
					if( owner.Mode != ContentBrowser.ModeEnum.Objects )
						skip = true;
					break;

				case nameof( SearchBar ):
					skip = true;
					break;

				case nameof( SplitterPosition ):
					if( PanelMode == ContentBrowser.PanelModeEnum.Tree || PanelMode == ContentBrowser.PanelModeEnum.List )
						skip = true;
					break;
				}
			}
		}

		public IEnumerable<Metadata.Member> MetadataGetMembers( Metadata.GetMembersContext context = null )
		{
			foreach( var m in BaseType.MetadataGetMembers( context ) )
			{
				bool skip = false;
				if( context == null || context.filter )
					MetadataGetMembersFilter( context, m, ref skip );
				if( !skip )
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
				if( block.AttributeExists( nameof( PanelMode ) ) )
					PanelMode = (ContentBrowser.PanelModeEnum)Enum.Parse( typeof( ContentBrowser.PanelModeEnum ), block.GetAttribute( nameof( PanelMode ) ) );
				if( block.AttributeExists( nameof( ListMode ) ) )
					ListMode = (ContentBrowser.ListModeEnum)Enum.Parse( typeof( ContentBrowser.ListModeEnum ), block.GetAttribute( nameof( ListMode ) ) );
				if( block.AttributeExists( nameof( TileImageSize ) ) )
					TileImageSize = int.Parse( block.GetAttribute( nameof( TileImageSize ) ) );
				if( block.AttributeExists( nameof( ListImageSize ) ) )
					ListImageSize = int.Parse( block.GetAttribute( nameof( ListImageSize ) ) );
				if( block.AttributeExists( nameof( ListColumnWidth ) ) )
					ListColumnWidth = int.Parse( block.GetAttribute( nameof( ListColumnWidth ) ) );
				if( block.AttributeExists( nameof( Breadcrumb ) ) )
					Breadcrumb = bool.Parse( block.GetAttribute( nameof( Breadcrumb ) ) );
				if( block.AttributeExists( nameof( SortFilesBy ) ) )
					SortFilesBy = (ContentBrowser.SortByItems)Enum.Parse( typeof( ContentBrowser.SortByItems ), block.GetAttribute( nameof( SortFilesBy ) ) );
				if( block.AttributeExists( nameof( SortFilesByAscending ) ) )
					SortFilesByAscending = bool.Parse( block.GetAttribute( nameof( SortFilesByAscending ) ) );
				if( block.AttributeExists( nameof( FilteringModeButton ) ) )
					FilteringModeButton = bool.Parse( block.GetAttribute( nameof( FilteringModeButton ) ) );
				if( block.AttributeExists( nameof( MembersButton ) ) )
					MembersButton = bool.Parse( block.GetAttribute( nameof( MembersButton ) ) );
				if( block.AttributeExists( nameof( OpenButton ) ) )
					OpenButton = bool.Parse( block.GetAttribute( nameof( OpenButton ) ) );
				if( block.AttributeExists( nameof( EditorButton ) ) )
					EditorButton = bool.Parse( block.GetAttribute( nameof( EditorButton ) ) );
				if( block.AttributeExists( nameof( SettingsButton ) ) )
					SettingsButton = bool.Parse( block.GetAttribute( nameof( SettingsButton ) ) );
				if( block.AttributeExists( nameof( ButtonsForEditing ) ) )
					ButtonsForEditing = bool.Parse( block.GetAttribute( nameof( ButtonsForEditing ) ) );
				if( block.AttributeExists( nameof( SearchButton ) ) )
					SearchButton = bool.Parse( block.GetAttribute( nameof( SearchButton ) ) );
				//!!!!
				//if( block.AttributeExists( nameof( SearchBar ) ) )
				//	SearchBar = bool.Parse( block.GetAttribute( nameof( SearchBar ) ) );

				if( block.AttributeExists( nameof( SplitterPosition ) ) )
					SplitterPosition = double.Parse( block.GetAttribute( nameof( SplitterPosition ) ) );
			}
			catch { }
		}

		public void Save( TextBlock block )
		{
			block.SetAttribute( nameof( PanelMode ), PanelMode.ToString() );
			if( ListMode != ContentBrowser.ListModeEnum.List )
				block.SetAttribute( nameof( ListMode ), ListMode.ToString() );
			if( TileImageSize != TileImageSizeDefault )
				block.SetAttribute( nameof( TileImageSize ), TileImageSize.ToString() );
			if( ListImageSize != ListImageSizeDefault )
				block.SetAttribute( nameof( ListImageSize ), ListImageSize.ToString() );
			if( ListColumnWidth != ListColumnWidthDefault )
				block.SetAttribute( nameof( ListColumnWidth ), ListColumnWidth.ToString() );
			if( !Breadcrumb )
				block.SetAttribute( nameof( Breadcrumb ), Breadcrumb.ToString() );
			if( DisplayPropertiesSortFilesBy )
			{
				if( SortFilesBy != ContentBrowser.SortByItems.Name )
					block.SetAttribute( nameof( SortFilesBy ), SortFilesBy.ToString() );
				if( !SortFilesByAscending )
					block.SetAttribute( nameof( SortFilesByAscending ), SortFilesByAscending.ToString() );
			}
			if( owner.Mode == ContentBrowser.ModeEnum.Resources )
			{
				if( !FilteringModeButton )
					block.SetAttribute( nameof( FilteringModeButton ), FilteringModeButton.ToString() );
			}
			if( owner.Mode == ContentBrowser.ModeEnum.Objects )
			{
				if( !MembersButton )
					block.SetAttribute( nameof( MembersButton ), MembersButton.ToString() );
			}
			if( DisplayPropertiesOpenButton )
			{
				if( !OpenButton )
					block.SetAttribute( nameof( OpenButton ), OpenButton.ToString() );
			}
			if( DisplayPropertiesEditorSettingsButtons )
			{
				if( !EditorButton )
					block.SetAttribute( nameof( EditorButton ), EditorButton.ToString() );
				if( !SettingsButton )
					block.SetAttribute( nameof( SettingsButton ), SettingsButton.ToString() );
			}
			if( !ButtonsForEditing )
				block.SetAttribute( nameof( ButtonsForEditing ), ButtonsForEditing.ToString() );
			if( owner.Mode == ContentBrowser.ModeEnum.Objects )
			{
				if( !SearchButton )
					block.SetAttribute( nameof( SearchButton ), SearchButton.ToString() );
			}

			//!!!!
			if( !SearchBar )
				block.SetAttribute( nameof( SearchBar ), SearchBar.ToString() );

			block.SetAttribute( nameof( SplitterPosition ), SplitterPosition.ToString() );
		}
	}
}
