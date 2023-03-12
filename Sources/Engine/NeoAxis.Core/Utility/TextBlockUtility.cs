// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for work with <see cref="NeoAxis.TextBlock"/>.
	/// </summary>
	public static class TextBlockUtility
	{
		/// <summary>
		/// Loads the block from a file of virtual file system.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <param name="fileNotFound"><b>true</b> if file not found.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromVirtualFile( string path, out string errorString, out bool fileNotFound )
		{
			errorString = null;
			fileNotFound = false;

			//!!!!имя файла в ошибке снаружи метода

			try
			{
				var bytes = VirtualFile.ReadAllBytes( path );
				var stream = new MemoryVirtualFileStream( bytes );

				//using( Stream stream = VirtualFile.Open( path ) )
				//{
				using( StreamReader streamReader = new StreamReader( stream ) )
				{
					string error;
					TextBlock textBlock = TextBlock.Parse( streamReader.ReadToEnd(), out error );
					if( textBlock == null )
					{
						errorString = $"Unable to load file \'{path}\'. " + error;
						return null;
					}
					return textBlock;
				}
				//}
			}
			catch( FileNotFoundException )
			{
				errorString = $"Unable to load file \'{path}\'. File not found.";
				fileNotFound = true;
				return null;
			}
			catch( Exception e )
			{
				errorString = $"Unable to load file \'{path}\'. " + e.Message;
				return null;
			}
		}

		/// <summary>
		/// Loads the block from a file of virtual file system.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromVirtualFile( string path, out string errorString )
		{
			bool fileNotFound;
			return LoadFromVirtualFile( path, out errorString, out fileNotFound );
		}

		/// <summary>
		/// Loads the block from a file of virtual file system.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromVirtualFile( string path )
		{
			string errorString;
			TextBlock textBlock = LoadFromVirtualFile( path, out errorString );
			if( textBlock == null )
				Log.Error( errorString );
			return textBlock;
		}

		/// <summary>
		/// Loads the block from a file of real file system.
		/// </summary>
		/// <param name="path">The real file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <param name="fileNotFound"><b>true</b> if file not found.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromRealFile( string path, out string errorString, out bool fileNotFound )
		{
			errorString = null;
			fileNotFound = false;

			try
			{
				var bytes = File.ReadAllBytes( path );
				var stream = new MemoryVirtualFileStream( bytes );

				//using( FileStream stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				//{
				using( StreamReader streamReader = new StreamReader( stream ) )
				{
					string error;
					TextBlock textBlock = TextBlock.Parse( streamReader.ReadToEnd(), out error );
					if( textBlock == null )
					{
						errorString = $"Unable to load file \'{path}\'. " + error;
						return null;
					}
					return textBlock;
				}
				//}
			}
			catch( FileNotFoundException )
			{
				errorString = $"Unable to load file \'{path}\'. File not found.";
				fileNotFound = true;
				return null;
			}
			catch( Exception e )
			{
				errorString = $"Unable to load file \'{path}\'. " + e.Message;
				return null;
			}
		}

		/// <summary>
		/// Loads the block from a file of real file system.
		/// </summary>
		/// <param name="path">The real file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromRealFile( string path, out string errorString )
		{
			bool fileNotFound;
			return LoadFromRealFile( path, out errorString, out fileNotFound );
		}

		/// <summary>
		/// Loads the block from a file of real file system.
		/// </summary>
		/// <param name="path">The real file path.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static TextBlock LoadFromRealFile( string path )
		{
			TextBlock textBlock = LoadFromRealFile( path, out var error );
			if( textBlock == null )
				Log.Error( error );
			return textBlock;
		}

		public static bool SaveToRealFile( TextBlock block, string path, out string error )
		{
			try
			{
				File.WriteAllText( path, block.DumpToString() );
				error = "";
				return true;
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
		}

		public static bool SaveToVirtualFile( TextBlock block, string path, out string error )
		{
			return SaveToRealFile( block, VirtualPathUtility.GetRealPathByVirtual( path ), out error );
		}

		public static bool SaveToRealFile( TextBlock block, string path )
		{
			if( !SaveToRealFile( block, path, out var error ) )
			{
				Log.Error( error );
				return false;
			}
			return true;
		}

		public static bool SaveToVirtualFile( TextBlock block, string path )
		{
			return SaveToRealFile( block, VirtualPathUtility.GetRealPathByVirtual( path ) );
		}
	}
}
