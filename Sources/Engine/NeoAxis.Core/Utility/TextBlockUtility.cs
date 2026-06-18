// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Text.Json;

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

			//!!!!им€ файла в ошибке снаружи метода

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

		///////////////////////////////////////////////

		//json conversion is not finished

		//public static Dictionary<string, object> ConvertTextBlockToJson( TextBlock textBlock )
		//{
		//	var result = new Dictionary<string, object>();

		//	foreach( var attr in textBlock.Attributes )
		//		result[ attr.Name ] = attr.Value;

		//	//"runtime": {
		//	//	"library_path": ".\\MixedRealityRuntime.dll"

		//	var counter = 1;

		//	foreach( var childBlock in textBlock.Children )
		//	{
		//		var attributes = ConvertTextBlockToJson( childBlock );

		//		if( !string.IsNullOrEmpty( childBlock.Data ) )
		//			attributes[ "_data" ] = childBlock.Data;

		//		result[ childBlock.Name + counter.ToString() ] = attributes;
		//		counter++;

		//		//result[ childBlock.Name ] = attributes;


		//		//!!!!sub blocks?

		//		//var attributes = new Dictionary<string, object>();

		//		//var childData = ConvertTextBlockToJson( childBlock );
		//		//children.Add( childData );
		//	}


		//	//if( textBlock.Children.Count != 0 )
		//	//{
		//	//	var children = new List<Dictionary<string, object>>();

		//	//	foreach( var childBlock in textBlock.Children )
		//	//	{

		//	//		//childBlock.Name
		//	//		//childBlock.Data


		//	//		//!!!!lists, elements?

		//	//		var childData = ConvertTextBlockToJson( childBlock );
		//	//		children.Add( childData );
		//	//	}

		//	//	result[ "children" ] = children;
		//	//}

		//	return result;
		//}

		//public static string ConvertTextBlockToJsonSerialized( TextBlock textBlock )
		//{
		//	var jsonData = ConvertTextBlockToJson( textBlock );
		//	var serialized = JsonSerializer.Serialize( jsonData, new JsonSerializerOptions { WriteIndented = true } );
		//	return serialized;
		//}

		//public static TextBlock ConvertJsonToTextBlock( Dictionary<string, object> json )
		//{
		//	var result = new TextBlock();

		//	foreach( var pair in json )
		//	{
		//		if( pair.Value is JsonElement element )
		//		{
		//			switch( element.ValueKind )
		//			{
		//			case JsonValueKind.Object:
		//				{
		//					// Child block
		//					var childDict = JsonSerializer.Deserialize<Dictionary<string, object>>( element.GetRawText() );
		//					var childBlock = ConvertJsonToTextBlock( childDict );
		//					childBlock.Name = pair.Key;
		//					result.AddChild( childBlock );
		//					break;
		//				}
		//			case JsonValueKind.Array:
		//				{
		//					// Array of child blocks or values
		//					foreach( var item in element.EnumerateArray() )
		//					{
		//						if( item.ValueKind == JsonValueKind.Object )
		//						{
		//							var childDict = JsonSerializer.Deserialize<Dictionary<string, object>>( item.GetRawText() );
		//							var childBlock = ConvertJsonToTextBlock( childDict );
		//							childBlock.Name = pair.Key;
		//							result.AddChild( childBlock );
		//						}
		//						else
		//						{
		//							// Array of primitive values as attributes
		//							result.SetAttribute( pair.Key, item.ToString() );
		//						}
		//					}
		//					break;
		//				}
		//			default:
		//				{
		//					// Attribute or special _data
		//					if( pair.Key == "_data" )
		//						result.Data = element.ToString();
		//					else
		//						result.SetAttribute( pair.Key, element.ToString() );
		//					break;
		//				}
		//			}
		//		}
		//		else if( pair.Value is Dictionary<string, object> dict )
		//		{
		//			// Child block
		//			var childBlock = ConvertJsonToTextBlock( dict );
		//			childBlock.Name = pair.Key;
		//			result.AddChild( childBlock );
		//		}
		//		//else if( pair.Key == "_data" )
		//		//{
		//		//	result.Data = pair.Value?.ToString();
		//		//}
		//		//else if( pair.Value is System.Text.Json.JsonElement je )
		//		//{
		//		//	result.SetAttribute( pair.Key, je.ToString() );
		//		//}
		//		else
		//		{
		//			// Attribute
		//			if( pair.Value != null )
		//				result.SetAttribute( pair.Key, pair.Value.ToString() );
		//		}
		//	}

		//	return result;
		//}

		//public static TextBlock ConvertJsonToTextBlock( string json )
		//{
		//	//!!!!ExpandoObject

		//	var json2 = JsonSerializer.Deserialize<Dictionary<string, object>>( json, new JsonSerializerOptions() );
		//	return ConvertJsonToTextBlock( json2 );
		//}

		////public static TextBlock ConvertJsonToTextBlock( Dictionary<string, object> json )
		////{
		////	var result = new TextBlock();

		////	foreach( var pair in json )
		////	{
		////		if( pair.Value != null )
		////			result.SetAttribute( pair.Key, pair.Value.ToString() );
		////	}

		////	//!!!!

		////	return result;
		////}

		////public static TextBlock ConvertJsonToTextBlock( string json )
		////{

		////	//!!!!ExpandoObject

		////	var json2 = JsonSerializer.Deserialize<Dictionary<string, object>>( json, new JsonSerializerOptions() );
		////	return ConvertJsonToTextBlock( json2 );
		////}
	}
}
