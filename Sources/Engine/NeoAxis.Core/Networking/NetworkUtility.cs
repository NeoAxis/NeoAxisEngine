// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	public static class NetworkUtility
	{
		static HttpClient httpClient;

		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error;
		}

		///////////////////////////////////////////////

		static HttpClient GetHttpClient()
		{
			if( httpClient == null )
			{
				httpClient = new HttpClient();
				httpClient.Timeout = new TimeSpan( 100, 0, 0 );
			}
			return httpClient;
		}

		///////////////////////////////////////////////

		public delegate void DownloadFileByUrlProgressCallback( int downloadedIncrement, long totalDownloaded, long totalSize );

		/*public */
		static async Task<SimpleResult> DownloadFileByUrlAsync( HttpClient client, string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				using( var response = await client.GetAsync( url, HttpCompletionOption.ResponseHeadersRead, cancellationToken ) )
				{
					response.EnsureSuccessStatusCode();

					long totalBytes = response.Content.Headers.ContentLength ?? -1;
					long totalReadBytes = 0;
					progressCallback?.Invoke( 0, totalReadBytes, totalBytes );

#if UWP
					using( var contentStream = await response.Content.ReadAsStreamAsync() )
#else
					using( var contentStream = await response.Content.ReadAsStreamAsync( cancellationToken ) )
#endif
					{
						using( var fileStream = new FileStream( targetFullPath, FileMode.Create, FileAccess.Write, FileShare.None ) )
						{
							byte[] buffer = new byte[ 8192 * 4 ];
							int bytesRead;
							while( ( bytesRead = await contentStream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 )
							{
								await fileStream.WriteAsync( buffer, 0, bytesRead, cancellationToken );
								totalReadBytes += bytesRead;

								progressCallback?.Invoke( bytesRead, totalReadBytes, totalBytes );
							}
						}
					}
				}

				return new SimpleResult();


				//using( var response = await client.GetAsync( url, HttpCompletionOption.ResponseHeadersRead, cancellationToken ) )
				//{
				//	response.EnsureSuccessStatusCode();

				//	long totalBytes = response.Content.Headers.ContentLength ?? -1;
				//	long totalReadBytes = 0;
				//	progressCallback?.Invoke( 0, totalReadBytes, totalBytes );

				//	using( var contentStream = await response.Content.ReadAsStreamAsync( cancellationToken ) )
				//	{
				//		using( var fileStream = new FileStream( targetFullPath, FileMode.Create, FileAccess.Write, FileShare.None ) )
				//		{
				//			byte[] buffer = new byte[ 8192 * 4 ];
				//			int bytesRead;
				//			while( ( bytesRead = await contentStream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 )
				//			{
				//				await fileStream.WriteAsync( buffer, 0, bytesRead, cancellationToken );
				//				totalReadBytes += bytesRead;

				//				progressCallback?.Invoke( bytesRead, totalReadBytes, totalBytes );
				//			}
				//		}
				//	}
				//}

				//return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> DownloadFileByUrlAsync( string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await DownloadFileByUrlAsync( GetHttpClient(), url, targetFullPath, progressCallback, cancellationToken );

			//using( var client = new HttpClient() )
			//	return await DownloadFileByUrlAsync( client, url, targetFullPath, progressCallback, cancellationToken );
		}

		///////////////////////////////////////////////

		public delegate void UploadFileByUrlProgressCallback( int uploadedIncrement, long totalUploaded, long totalSize );

		/*public*/
		static async Task<SimpleResult> UploadFileByUrlAsync( HttpClient client, string url, string sourceFullPath, bool appendSupport, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				if( appendSupport )
				{
					//!!!!bigger or change limits of max request per minute
					const int chunkSize = 30 * 1024 * 1024; // 30 MB

					long totalSize = new FileInfo( sourceFullPath ).Length;
					long totalUploaded = 0;

					using( var fileStream = new FileStream( sourceFullPath, FileMode.Open, FileAccess.Read ) )
					{
						if( fileStream.Length > 0 )
						{
							byte[] buffer = new byte[ chunkSize ];
							int bytesRead;
							int partNumber = 0;

							while( ( bytesRead = await fileStream.ReadAsync( buffer, 0, chunkSize, cancellationToken ) ) > 0 )
							{
								using( var content = new MultipartFormDataContent() )
								{
									// Create a StreamContent from the read buffer
									var fileContent = new StreamContent( new MemoryStream( buffer, 0, bytesRead ) );

									// Add file content to the multipart form data with the appropriate part name
									content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

									//construct url
									string uploadUrl = url;
									if( partNumber != 0 )
										uploadUrl += "&append=true";
									var isLastPart = totalUploaded + bytesRead >= totalSize;
									if( isLastPart )
										uploadUrl += "&last_part=true";

									//string uploadUrl = partNumber == 0 ? url : $"{url}&append=true";

									// Send the POST request
									var response = await client.PostAsync( uploadUrl, content, cancellationToken );
									response.EnsureSuccessStatusCode(); // Optionally check response status

									// Update total uploaded size
									totalUploaded += bytesRead;

									// Invoke the progress callback if provided
									progressCallback?.Invoke( bytesRead, totalUploaded, totalSize );

									partNumber++;
								}
							}
						}
						else
						{
							using( var content = new MultipartFormDataContent() )
							{
								// Create a StreamContent from the read buffer
								var fileContent = new StreamContent( new MemoryStream( Array.Empty<byte>() ) );

								// Add file content to the multipart form data with the appropriate part name
								content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

								// Modify the URL for appending after the first part
								string uploadUrl = url;
								uploadUrl += "&last_part=true";

								// Send the POST request
								var response = await client.PostAsync( uploadUrl, content, cancellationToken );
								response.EnsureSuccessStatusCode(); // Optionally check response status

								// Invoke the progress callback if provided
								progressCallback?.Invoke( 0, totalUploaded, totalSize );
							}
						}
					}
				}
				else
				{
					//without streaming
					using( var content = new MultipartFormDataContent() )
					{
						// Load the file into a Byte array
						var fileStream = new FileStream( sourceFullPath, FileMode.Open, FileAccess.Read );
						var fileContent = new StreamContent( fileStream );

						// Add file content to the multipart form data
						content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

						// Send the POST request
						var response = await client.PostAsync( url, content, cancellationToken );

						response.EnsureSuccessStatusCode();
					}
				}

				var result = new SimpleResult();

				//var blockString = ""; // Adjust this to handle responses if necessary.
				//var block = TextBlock.Parse( blockString, out var error );
				//if( !string.IsNullOrEmpty( error ) )
				//	throw new Exception( "Error of parsing the response data. " + error );
				//var errorInResultData = block.GetAttribute( "Error" );
				//if( !string.IsNullOrEmpty( errorInResultData ) )
				//	result.Error = errorInResultData;

				return result;
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> UploadFileByUrlAsync( string url, string sourceFullPath, bool appendSupport, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await UploadFileByUrlAsync( GetHttpClient(), url, sourceFullPath, appendSupport, progressCallback, cancellationToken );

			//using( var client = new HttpClient() )
			//	return await UploadFileByUrlAsync( client, url, sourceFullPath, appendSupport, progressCallback, cancellationToken );
		}
	}
}
