// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	public static class NetworkUtility
	{
		//hardcoded value based on the internet experience.
		const double maxNoConnectionTimeInSeconds = 30;

		static HttpClient httpClient;

		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error;
		}

		///////////////////////////////////////////////

		public static HttpClient GetHttpClient()
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

		static async Task<SimpleResult> DownloadFileByUrlAsync( HttpClient client, string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			// The code with attempts to restore connection and continue downloading.

			//Get info about the ability to resume downloading and the total file size.
			//Do serveral attemps when error.
			//The error may be caused before first download or during downloading. In the first case we will try to start downloading several times. In the second case we will try to restore connection and continue downloading several times.
			//maxNoConnectionTimeInSeconds is a maximal time of no connection period. If the connection will be restored during this time, maxNoConnectionTimeInSeconds must be reset for another no connection period. If the connection will not be restored during this time, the downloading will be stopped with error.


			static bool IsTransient( Exception e, CancellationToken ct )
			{
				// If caller requested cancellation, don't retry.
				if( ct.IsCancellationRequested )
					return false;

				// HttpClient timeout typically surfaces as TaskCanceledException/OperationCanceledException.
				// If token wasn't canceled -> treat as transient.
				if( e is OperationCanceledException )
					return true;

				// network errors / timeouts / IO errors are usually transient
				return e is HttpRequestException || e is IOException || e is TaskCanceledException;
			}

			static async Task<long> TryGetRemoteFileSizeAsync( HttpClient client2, string url2, CancellationToken ct )
			{
				// Prefer HEAD, fallback to range GET (bytes=0-0).
				try
				{
					using( var head = new HttpRequestMessage( HttpMethod.Head, url2 ) )
					using( var resp = await client2.SendAsync( head, HttpCompletionOption.ResponseHeadersRead, ct ) )
					{
						if( resp.IsSuccessStatusCode )
							return resp.Content.Headers.ContentLength ?? -1;
					}
				}
				catch
				{
					// ignore and fallback
				}

				try
				{
					using( var req = new HttpRequestMessage( HttpMethod.Get, url2 ) )
					{
						req.Headers.Range = new RangeHeaderValue( 0, 0 );
						using( var resp = await client2.SendAsync( req, HttpCompletionOption.ResponseHeadersRead, ct ) )
						{
							if( resp.StatusCode == System.Net.HttpStatusCode.PartialContent )
							{
								var cr = resp.Content.Headers.ContentRange;
								if( cr != null && cr.Length.HasValue )
									return cr.Length.Value;
							}
							return resp.Content.Headers.ContentLength ?? -1;
						}
					}
				}
				catch
				{
					return -1;
				}
			}

			static async Task<bool> ProbeResumeSupportAsync( HttpClient client2, string url2, CancellationToken ct )
			{
				try
				{
					using( var req = new HttpRequestMessage( HttpMethod.Get, url2 ) )
					{
						req.Headers.Range = new RangeHeaderValue( 0, 0 );
						using( var resp = await client2.SendAsync( req, HttpCompletionOption.ResponseHeadersRead, ct ) )
						{
							// If server returns 206 and Content-Range -> supports resume.
							return resp.StatusCode == System.Net.HttpStatusCode.PartialContent &&
								   resp.Content.Headers.ContentRange != null;
						}
					}
				}
				catch
				{
					return false;
				}
			}

			// Get info (best-effort): resume ability + total size.
			long totalBytes = await TryGetRemoteFileSizeAsync( client, url, cancellationToken );
			bool canResume = await ProbeResumeSupportAsync( client, url, cancellationToken );

			long existingSize = 0;
			if( canResume && File.Exists( targetFullPath ) )
			{
				try { existingSize = new FileInfo( targetFullPath ).Length; }
				catch { existingSize = 0; }

				// If existing file is bigger than remote (or remote unknown), don't resume.
				if( totalBytes >= 0 && existingSize > totalBytes )
				{
					existingSize = 0;
					canResume = false;
				}
			}

			long totalReadBytes = existingSize;
			progressCallback?.Invoke( 0, totalReadBytes, totalBytes );

			var buffer = new byte[ 8192 * 4 ];

			var startNoConnectionTime = DateTime.UtcNow;
			while( true )
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					using( var request = new HttpRequestMessage( HttpMethod.Get, url ) )
					{
						// resume from already downloaded bytes if supported
						if( canResume && totalReadBytes > 0 )
							request.Headers.Range = new RangeHeaderValue( totalReadBytes, null );

						using( var response = await client.SendAsync( request, HttpCompletionOption.ResponseHeadersRead, cancellationToken ) )
						{
							// If asked for range but server ignored it, restart from 0 and truncate.
							if( canResume && totalReadBytes > 0 && response.StatusCode == System.Net.HttpStatusCode.OK )
							{
								totalReadBytes = 0;
								existingSize = 0;
								canResume = false;

								try
								{
									if( File.Exists( targetFullPath ) )
										File.Delete( targetFullPath );
								}
								catch
								{
									// fall through; FileMode.Create below will still overwrite if possible
								}

								progressCallback?.Invoke( 0, totalReadBytes, totalBytes );
							}

							response.EnsureSuccessStatusCode();

							// update totalBytes from headers when possible
							if( response.StatusCode == System.Net.HttpStatusCode.PartialContent )
							{
								var cr = response.Content.Headers.ContentRange;
								if( cr != null && cr.Length.HasValue )
									totalBytes = cr.Length.Value;
							}
							else
							{
								var cl = response.Content.Headers.ContentLength;
								if( cl.HasValue )
								{
									if( totalReadBytes == 0 )
										totalBytes = cl.Value;
									else if( totalBytes < 0 )
										totalBytes = totalReadBytes + cl.Value;
								}
							}

#if NETSTANDARD2_1
							using( var contentStream = await response.Content.ReadAsStreamAsync() )
#else
							using( var contentStream = await response.Content.ReadAsStreamAsync( cancellationToken ) )
#endif
							{
								// Append if resuming, otherwise create new.
								var fileMode = ( canResume && totalReadBytes > 0 ) ? FileMode.Append : FileMode.Create;

								using( var fileStream = new FileStream( targetFullPath, fileMode, FileAccess.Write, FileShare.None ) )
								{
									int bytesRead;
									while( ( bytesRead = await contentStream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 )
									{
										await fileStream.WriteAsync( buffer, 0, bytesRead, cancellationToken );
										totalReadBytes += bytesRead;

										// Connection is alive, reset the no-connection timer.
										startNoConnectionTime = DateTime.UtcNow;

										progressCallback?.Invoke( bytesRead, totalReadBytes, totalBytes );
									}
								}
							}
						}
					}

					return new SimpleResult();
				}
				catch( Exception e )
				{
					if( e is OperationCanceledException && cancellationToken.IsCancellationRequested )
						return new SimpleResult() { Error = e.Message };

					if( !IsTransient( e, cancellationToken ) )
						return new SimpleResult() { Error = e.Message };

					// transient failure: try to restore connection and continue until maxNoConnectionTimeInSeconds exceeded
					var noConnectionTime = ( DateTime.UtcNow - startNoConnectionTime ).TotalSeconds;
					if( noConnectionTime > maxNoConnectionTimeInSeconds )
						return new SimpleResult() { Error = e.Message };

					// Re-check current file length (best effort) to continue from what was persisted.
					if( canResume && File.Exists( targetFullPath ) )
					{
						try
						{
							var len = new FileInfo( targetFullPath ).Length;
							if( len >= 0 )
								totalReadBytes = len;
						}
						catch
						{
							// keep current totalReadBytes
						}
					}
					else
					{
						// if cannot resume, restart from scratch on next attempt
						totalReadBytes = 0;
					}

					// small backoff
					try
					{
						await Task.Delay( 2000, cancellationToken );
					}
					catch
					{
						return new SimpleResult() { Error = e.Message };
					}
				}
			}
		}

		public static async Task<SimpleResult> DownloadFileByUrlAsync( string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			//var dir = Path.GetDirectoryName( targetFullPath );
			//if( !string.IsNullOrEmpty( dir ) && !Directory.Exists( dir ) )
			//	Directory.CreateDirectory( dir );

			return await DownloadFileByUrlAsync( GetHttpClient(), url, targetFullPath, progressCallback, cancellationToken );
		}


		//		public delegate void DownloadFileByUrlProgressCallback( int downloadedIncrement, long totalDownloaded, long totalSize );

		//		static async Task<SimpleResult> DownloadFileByUrlAsync( HttpClient client, string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//		{
		//			try
		//			{
		//				using( var response = await client.GetAsync( url, HttpCompletionOption.ResponseHeadersRead, cancellationToken ) )
		//				{
		//					response.EnsureSuccessStatusCode();

		//					long totalBytes = response.Content.Headers.ContentLength ?? -1;
		//					long totalReadBytes = 0;
		//					progressCallback?.Invoke( 0, totalReadBytes, totalBytes );

		//#if UWP
		//					using( var contentStream = await response.Content.ReadAsStreamAsync() )
		//#else
		//					using( var contentStream = await response.Content.ReadAsStreamAsync( cancellationToken ) )
		//#endif
		//					{
		//						using( var fileStream = new FileStream( targetFullPath, FileMode.Create, FileAccess.Write, FileShare.None ) )
		//						{
		//							byte[] buffer = new byte[ 8192 * 4 ];
		//							int bytesRead;
		//							while( ( bytesRead = await contentStream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 )
		//							{
		//								await fileStream.WriteAsync( buffer, 0, bytesRead, cancellationToken );
		//								totalReadBytes += bytesRead;

		//								progressCallback?.Invoke( bytesRead, totalReadBytes, totalBytes );
		//							}
		//						}
		//					}
		//				}

		//				return new SimpleResult();


		//				//using( var response = await client.GetAsync( url, HttpCompletionOption.ResponseHeadersRead, cancellationToken ) )
		//				//{
		//				//	response.EnsureSuccessStatusCode();

		//				//	long totalBytes = response.Content.Headers.ContentLength ?? -1;
		//				//	long totalReadBytes = 0;
		//				//	progressCallback?.Invoke( 0, totalReadBytes, totalBytes );

		//				//	using( var contentStream = await response.Content.ReadAsStreamAsync( cancellationToken ) )
		//				//	{
		//				//		using( var fileStream = new FileStream( targetFullPath, FileMode.Create, FileAccess.Write, FileShare.None ) )
		//				//		{
		//				//			byte[] buffer = new byte[ 8192 * 4 ];
		//				//			int bytesRead;
		//				//			while( ( bytesRead = await contentStream.ReadAsync( buffer, 0, buffer.Length, cancellationToken ) ) > 0 )
		//				//			{
		//				//				await fileStream.WriteAsync( buffer, 0, bytesRead, cancellationToken );
		//				//				totalReadBytes += bytesRead;

		//				//				progressCallback?.Invoke( bytesRead, totalReadBytes, totalBytes );
		//				//			}
		//				//		}
		//				//	}
		//				//}

		//				//return new SimpleResult();
		//			}
		//			catch( Exception e )
		//			{
		//				return new SimpleResult() { Error = e.Message };
		//			}
		//		}

		//		public static async Task<SimpleResult> DownloadFileByUrlAsync( string url, string targetFullPath, DownloadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//		{
		//			return await DownloadFileByUrlAsync( GetHttpClient(), url, targetFullPath, progressCallback, cancellationToken );

		//			//using( var client = new HttpClient() )
		//			//	return await DownloadFileByUrlAsync( client, url, targetFullPath, progressCallback, cancellationToken );
		//		}

		///////////////////////////////////////////////


		public delegate void UploadFileByUrlProgressCallback( int uploadedIncrement, long totalUploaded, long totalSize );

		static async Task<SimpleResult> UploadFileByUrlWithAppendSupportAsync( HttpClient client, string url, string sourceFullPath, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				// The code with attempts to restore connection and continue uploading.

				//The error may be caused before first upload or during uploading. In the first case we will try to start uploading several times. In the second case we will try to restore connection and continue uploading several times.
				//maxNoConnectionTimeInSeconds is a maximal time of no connection period. If the connection will be restored during this time, maxNoConnectionTimeInSeconds must be reset for another no connection period. If the connection will not be restored during this time, the uploading will be stopped with error.


				static bool IsTransient( Exception e, CancellationToken ct )
				{
					// If caller requested cancellation, don't retry.
					if( ct.IsCancellationRequested )
						return false;

					// HttpClient timeout typically surfaces as TaskCanceledException/OperationCanceledException.
					// If token wasn't canceled -> treat as transient.
					if( e is OperationCanceledException )
						return true;

					// network errors / timeouts / IO errors are usually transient
					return e is HttpRequestException || e is IOException || e is TaskCanceledException;
				}

				const int chunkSize = 30 * 1024 * 1024; // 30 MB

				// Get files size.
				long totalSize;
				try
				{
					totalSize = new FileInfo( sourceFullPath ).Length;
				}
				catch( Exception e )
				{
					return new SimpleResult() { Error = e.Message };
				}

				long totalUploaded = 0;
				progressCallback?.Invoke( 0, totalUploaded, totalSize );

				var startNoConnectionTime = DateTime.UtcNow;
				var partIndex = 0;

				while( true )
				{
					cancellationToken.ThrowIfCancellationRequested();

					try
					{
						// Re-open stream on each attempt to ensure we can continue after a broken connection.
						using( var fileStream = new FileStream( sourceFullPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
						{
							if( totalSize > 0 )
							{
								if( totalUploaded > 0 )
									fileStream.Seek( totalUploaded, SeekOrigin.Begin );

								byte[] buffer = new byte[ chunkSize ];

								while( totalUploaded < totalSize )
								{
									cancellationToken.ThrowIfCancellationRequested();

									int toRead = (int)Math.Min( buffer.Length, totalSize - totalUploaded );
									int bytesRead = await fileStream.ReadAsync( buffer, 0, toRead, cancellationToken );
									if( bytesRead <= 0 )
										break;

									using( var content = new MultipartFormDataContent() )
									{
										var fileContent = new StreamContent( new MemoryStream( buffer, 0, bytesRead, writable: false ) );
										content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

										// construct url based on already uploaded size
										string uploadUrl = url;
										if( totalUploaded != 0 )
											uploadUrl += "&append=true";

										var isLastPart = totalUploaded + bytesRead >= totalSize;
										if( isLastPart )
											uploadUrl += "&last_part=true";

										uploadUrl += $"&part_index={partIndex}";

										var response = await client.PostAsync( uploadUrl, content, cancellationToken );
										response.EnsureSuccessStatusCode();

										totalUploaded += bytesRead;
										partIndex++;

										// Connection is alive, reset the no-connection timer.
										startNoConnectionTime = DateTime.UtcNow;

										progressCallback?.Invoke( bytesRead, totalUploaded, totalSize );
									}
								}
							}
							else
							{
								// zero-length file
								using( var content = new MultipartFormDataContent() )
								{
									var fileContent = new StreamContent( new MemoryStream( Array.Empty<byte>(), writable: false ) );
									content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

									string uploadUrl = url + "&last_part=true";

									var response = await client.PostAsync( uploadUrl, content, cancellationToken );
									response.EnsureSuccessStatusCode();

									startNoConnectionTime = DateTime.UtcNow;
									progressCallback?.Invoke( 0, totalUploaded, totalSize );
								}
							}
						}

						return new SimpleResult();
					}
					catch( Exception e )
					{
						if( e is OperationCanceledException && cancellationToken.IsCancellationRequested )
							return new SimpleResult() { Error = e.Message };

						if( !IsTransient( e, cancellationToken ) )
							return new SimpleResult() { Error = e.Message };

						var noConnectionTime = ( DateTime.UtcNow - startNoConnectionTime ).TotalSeconds;
						if( noConnectionTime > maxNoConnectionTimeInSeconds )
							return new SimpleResult() { Error = e.Message };

						// Keep totalUploaded as is (already acknowledged parts).
						// small backoff
						try
						{
							await Task.Delay( 2000, cancellationToken );
						}
						catch
						{
							return new SimpleResult() { Error = e.Message };
						}
					}
				}
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		static async Task<SimpleResult> UploadFileByUrlWithoutAppendSupportAsync( HttpClient client, string url, string sourceFullPath, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				//maybe add attempts to restore connection and continue uploading for this case too. But it is more complex because we need to track already uploaded size and send it to server in some way. So for now just one attempt without resume support.

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

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> UploadFileByUrlAsync( string url, string sourceFullPath, bool appendSupport, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			if( appendSupport )
				return await UploadFileByUrlWithAppendSupportAsync( GetHttpClient(), url, sourceFullPath, progressCallback, cancellationToken );
			else
				return await UploadFileByUrlWithoutAppendSupportAsync( GetHttpClient(), url, sourceFullPath, progressCallback, cancellationToken );
		}



		//public delegate void UploadFileByUrlProgressCallback( int uploadedIncrement, long totalUploaded, long totalSize );

		//static async Task<SimpleResult> UploadFileByUrlWithAppendSupportAsync( HttpClient client, string url, string sourceFullPath, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//{
		//	try
		//	{
		//		//Do serveral attemps when error.
		//		//The error may be caused before first upload or during uploading. In the first case we will try to start uploading several times. In the second case we will try to restore connection and continue uploading several times.
		//		//maxNoConnectionTimeInSeconds is a maximal time of no connection period. If the connection will be restored during this time, maxNoConnectionTimeInSeconds must be reset for another no connection period. If the connection will not be restored during this time, the uploading will be stopped with error.

		//		//!!!!bigger or change limits of max request per minute
		//		const int chunkSize = 30 * 1024 * 1024; // 30 MB

		//		long totalSize = new FileInfo( sourceFullPath ).Length;
		//		long totalUploaded = 0;

		//		using( var fileStream = new FileStream( sourceFullPath, FileMode.Open, FileAccess.Read ) )
		//		{
		//			if( fileStream.Length > 0 )
		//			{
		//				byte[] buffer = new byte[ chunkSize ];
		//				int bytesRead;
		//				int partNumber = 0;

		//				while( ( bytesRead = await fileStream.ReadAsync( buffer, 0, chunkSize, cancellationToken ) ) > 0 )
		//				{
		//					using( var content = new MultipartFormDataContent() )
		//					{
		//						// Create a StreamContent from the read buffer
		//						var fileContent = new StreamContent( new MemoryStream( buffer, 0, bytesRead ) );

		//						// Add file content to the multipart form data with the appropriate part name
		//						content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

		//						//construct url
		//						string uploadUrl = url;
		//						if( partNumber != 0 )
		//							uploadUrl += "&append=true";
		//						var isLastPart = totalUploaded + bytesRead >= totalSize;
		//						if( isLastPart )
		//							uploadUrl += "&last_part=true";

		//						// Send the POST request
		//						var response = await client.PostAsync( uploadUrl, content, cancellationToken );

		//						// Check response status
		//						response.EnsureSuccessStatusCode();

		//						// Update total uploaded size
		//						totalUploaded += bytesRead;

		//						// Invoke the progress callback if provided
		//						progressCallback?.Invoke( bytesRead, totalUploaded, totalSize );

		//						partNumber++;
		//					}
		//				}
		//			}
		//			else
		//			{
		//				using( var content = new MultipartFormDataContent() )
		//				{
		//					// Create a StreamContent from the read buffer
		//					var fileContent = new StreamContent( new MemoryStream( Array.Empty<byte>() ) );

		//					// Add file content to the multipart form data with the appropriate part name
		//					content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

		//					// Modify the URL for appending after the first part
		//					string uploadUrl = url;
		//					uploadUrl += "&last_part=true";

		//					// Send the POST request
		//					var response = await client.PostAsync( uploadUrl, content, cancellationToken );

		//					// Check response status
		//					response.EnsureSuccessStatusCode();

		//					// Invoke the progress callback if provided
		//					progressCallback?.Invoke( 0, totalUploaded, totalSize );
		//				}
		//			}
		//		}

		//		return new SimpleResult();
		//	}
		//	catch( Exception e )
		//	{
		//		return new SimpleResult() { Error = e.Message };
		//	}
		//}

		//static async Task<SimpleResult> UploadFileByUrlWithoutAppendSupportAsync( HttpClient client, string url, string sourceFullPath, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//{
		//	try
		//	{
		//		using( var content = new MultipartFormDataContent() )
		//		{
		//			// Load the file into a Byte array
		//			var fileStream = new FileStream( sourceFullPath, FileMode.Open, FileAccess.Read );
		//			var fileContent = new StreamContent( fileStream );

		//			// Add file content to the multipart form data
		//			content.Add( fileContent, "file", Path.GetFileName( sourceFullPath ) );

		//			// Send the POST request
		//			var response = await client.PostAsync( url, content, cancellationToken );

		//			response.EnsureSuccessStatusCode();
		//		}

		//		return new SimpleResult();
		//	}
		//	catch( Exception e )
		//	{
		//		return new SimpleResult() { Error = e.Message };
		//	}
		//}

		//public static async Task<SimpleResult> UploadFileByUrlAsync( string url, string sourceFullPath, bool appendSupport, UploadFileByUrlProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//{
		//	if( appendSupport )
		//		return await UploadFileByUrlWithAppendSupportAsync( GetHttpClient(), url, sourceFullPath, progressCallback, cancellationToken );
		//	else
		//		return await UploadFileByUrlWithoutAppendSupportAsync( GetHttpClient(), url, sourceFullPath, progressCallback, cancellationToken );
		//}

	}
}
