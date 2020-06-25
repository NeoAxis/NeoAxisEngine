// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for work with <see cref="NeoAxis.DataBlock"/>.
	/// </summary>
	public static class DataBlockUtility
	{
		/// <summary>
		/// Loads the block from a file of virtual file system.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static DataBlock LoadFromVirtualFile( string path, out string errorString )
		{
			errorString = null;

			if( !VirtualFile.Exists( path ) )
			{
				errorString = string.Format( "File not found \"{0}\".", path );
				return null;
			}

			try
			{
				byte[] data = VirtualFile.ReadAllBytes( path );

				string error;
				DataBlock dataBlock = DataBlock.Parse( data, false, out error );
				if( dataBlock == null )
				{
					errorString = string.Format( "Parsing data block failed \"{0}\" ({1}).",
						path, error );
				}

				return dataBlock;
			}
			catch( Exception )
			{
				errorString = string.Format( "Reading file failed \"{0}\".", path );
				return null;
			}
		}

		/// <summary>
		/// Loads the block from a file of virtual file system.
		/// </summary>
		/// <param name="path">The virtual file path.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static DataBlock LoadFromVirtualFile( string path )
		{
			string errorString;
			DataBlock dataBlock = LoadFromVirtualFile( path, out errorString );
			if( dataBlock == null )
				Log.Error( errorString );
			return dataBlock;
		}

		/// <summary>
		/// Loads the block from a file of real file system.
		/// </summary>
		/// <param name="path">The real file path.</param>
		/// <param name="errorString">The information on an error.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static DataBlock LoadFromRealFile( string path, out string errorString )
		{
			errorString = null;

			if( !File.Exists( path ) )
			{
				errorString = string.Format( "File not found \"{0}\".", path );
				return null;
			}

			try
			{
				byte[] data = File.ReadAllBytes( path );

				string error;
				DataBlock dataBlock = DataBlock.Parse( data, false, out error );
				if( dataBlock == null )
				{
					errorString = string.Format( "Parsing data block failed \"{0}\" ({1}).",
						path, error );
				}

				return dataBlock;
			}
			catch( Exception )
			{
				errorString = string.Format( "Reading file failed \"{0}\".", path );
				return null;
			}
		}

		/// <summary>
		/// Loads the block from a file of real file system.
		/// </summary>
		/// <param name="path">The real file path.</param>
		/// <returns><see cref="NeoAxis.DataBlock"/> if the block has been loaded; otherwise, <b>null</b>.</returns>
		public static DataBlock LoadFromRealFile( string path )
		{
			string errorString;
			DataBlock dataBlock = LoadFromRealFile( path, out errorString );
			if( dataBlock == null )
				Log.Error( errorString );
			return dataBlock;
		}
	}
}
