#if !NO_SERVER
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using NeoAxis;

namespace NeoAxis.CloudServer
{
	public class RepositoryIndexServer_ReadOnly
	{
		public EDictionary<string, Item> Items = new EDictionary<string, Item>();

		///////////////////////////////////////////////

		public class Item
		{
			public string Name;
			public bool Added;
			public string AccessTags;
		}

		///////////////////////////////////////////////

		static RepositoryIndexServer_ReadOnly Load( string fullPath, string projectDirectory, out string error )
		{
			error = null;

			try
			{
				var block = TextBlockUtility.LoadFromRealFile( fullPath, out error );
				if( !string.IsNullOrEmpty( error ) )
					return null;

				var index = new RepositoryIndexServer_ReadOnly();

				var nowUtc = DateTime.UtcNow;

				foreach( var itemBlock in block.Children )
				{
					if( itemBlock.Name == "Item" )
					{
						var item = new Item();
						item.Name = itemBlock.GetAttribute( "Name" );

						var added = itemBlock.GetAttribute( "Added" );
						if( !string.IsNullOrEmpty( added ) )
							bool.TryParse( added, out item.Added );

						item.AccessTags = itemBlock.GetAttribute( "AccessTags" );

						index.Items[ item.Name ] = item;
					}
				}

				return index;
			}
			catch( Exception e )
			{
				error = e.Message;
				return null;
			}
		}

		public static RepositoryIndexServer_ReadOnly Load( string projectDirectory, out string error )
		{
			try
			{
				var fullPath = Path.Combine( projectDirectory, ".internal", "Repository.index" );
				if( File.Exists( fullPath ) )
					return Load( fullPath, projectDirectory, out error );
				else
				{
					error = null;
					return new RepositoryIndexServer_ReadOnly();
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return null;
			}
		}

		public Item GetItem( string name )
		{
			if( Items.TryGetValue( name, out var item ) )
				return item;
			return null;
		}
	}
}
#endif