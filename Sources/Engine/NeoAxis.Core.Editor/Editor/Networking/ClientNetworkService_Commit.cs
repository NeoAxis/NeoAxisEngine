#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	public class ClientNetworkService_Commit : ClientNetworkService
	{
		public const int FileBlockSize = 1 * 1024 * 1024;

		public int SendFilesMaxSize { get; set; } = 5 * 1024 * 1024;
		public bool ZipData = true;

		FileItem[] allFilesToCommit;
		Queue<FileItem> remainsFilesToCommit;
		volatile StatusClass status = new StatusClass( StatusEnum.NotInitialized, "" );
		bool packetSent;
		int packedSentCount;
		long packedSentSize;
		//public long RemainsSizeToUpdate;

		///////////////////////////////////////////

		public delegate void StatusChangedDelegate( ClientNetworkService_Commit sender );
		public event StatusChangedDelegate StatusChanged;

		///////////////////////////////////////////

		public class FileItem
		{
			public string FileName;
			public bool Delete;

			//when add
			public RepositorySyncMode SyncMode;
			public string FullPath;
			public long Length;

			public int SentBlocks;

			//

			public int BlockCount
			{
				get { return (int)( ( Length + FileBlockSize - 1 ) / FileBlockSize ); }
			}

			public long SendRemainingLength
			{
				get { return Math.Min( Length, ( BlockCount - SentBlocks ) * FileBlockSize ); }
			}
		}

		///////////////////////////////////////////

		public enum StatusEnum
		{
			NotInitialized,
			Updating,
			Success,
			Error,
		}

		///////////////////////////////////////////

		public class StatusClass
		{
			public StatusEnum Status { get; }
			public string Error { get; } = "";

			public StatusClass( StatusEnum status, string error )
			{
				Status = status;
				Error = error;
			}
		}

		///////////////////////////////////////////

		public ClientNetworkService_Commit()
			: base( "Commit", 6 )
		{
			//register message types
			//RegisterMessageType( "BeginTransactionToServer", 1 );
			RegisterMessageType( "SendFilesToServer", 2 );
			//RegisterMessageType( "EndTransactionToServer", 3 );
			RegisterMessageType( "SendFilesResultToClient", 4, ReceiveMessage_SendFilesResultToClient );
			//RegisterMessageType( "EndTransactionResultToClient", 5, ReceiveMessage_EndTransactionResultToClient );
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		public StatusClass Status
		{
			get { return status; }
		}

		void SetStatus( StatusClass status )
		{
			this.status = status;
			StatusChanged?.Invoke( this );
		}

		bool ReceiveMessage_SendFilesResultToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var error = reader.ReadString();

			if( !reader.Complete() )
			{
				SetStatus( new StatusClass( StatusEnum.Error, "Error getting send files result." ) );
				return false;
			}

			if( !string.IsNullOrEmpty( error ) )
			{
				SetStatus( new StatusClass( StatusEnum.Error, error ) );
				return true;
			}

			packetSent = false;
			packedSentCount = 0;
			packedSentSize = 0;

			return true;
		}

		//bool ReceiveMessage_EndTransactionResultToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		//{
		//	if( !reader.Complete() )
		//	{
		//		//SetStatus( new StatusClass( StatusEnum.Error, "Error getting files." ) );
		//		return false;
		//	}

		//	return true;
		//}

		public void SetFilesToCommit( ICollection<FileItem> filesToCommit )
		{
			allFilesToCommit = filesToCommit.ToArray();
			remainsFilesToCommit = new Queue<FileItem>( allFilesToCommit );

			//UpdateRemainsToUpdate();
		}

		protected internal override void OnUpdate()
		{
			base.OnUpdate();

			if( Status != null && Status.Status == StatusEnum.Updating )
			{
				if( !packetSent && remainsFilesToCommit.Count != 0 )
				{
					var writer = BeginMessage( GetMessageType( "SendFilesToServer" ) );
					var totalSentCount = 0;
					var totalSentSize = 0L;

					writer.WriteVariableInt32( FileBlockSize );

					while( remainsFilesToCommit.Count != 0 )
					{
						var fileItem = remainsFilesToCommit.Dequeue();

						writer.Write( true );
						writer.Write( fileItem.FileName );
						writer.Write( fileItem.Delete );

						if( !fileItem.Delete )
						{
							try
							{
								byte[] data;
								using( FileStream fs = new FileStream( fileItem.FullPath, FileMode.Open ) )
								{
									var offset = fileItem.SentBlocks * FileBlockSize;
									fs.Seek( offset, SeekOrigin.Begin );

									var size = (int)Math.Min( fileItem.Length - offset, FileBlockSize );

									data = new byte[ size ];
									if( fs.Read( data, 0, size ) != size )
										throw new Exception( "File reading failed." );
								}
								//var data = File.ReadAllBytes( fileItem.FullPath );

								writer.WriteVariableInt32( fileItem.SentBlocks );
								writer.Write( data.Length );
								//writer.Write( (int)fileItem.Length );

								var zipData = ZipData && data.Length > 200;
								writer.Write( zipData );
								if( zipData )
								{
									var zippedData = IOUtility.Zip( data, System.IO.Compression.CompressionLevel.Fastest );
									writer.Write( zippedData.Length );
									writer.Write( zippedData );
								}
								else
									writer.Write( data );

								writer.Write( (byte)fileItem.SyncMode );

								fileItem.SentBlocks++;
								totalSentSize += data.Length;

								if( fileItem.SentBlocks < fileItem.BlockCount )
									remainsFilesToCommit.Enqueue( fileItem );
							}
							catch( Exception e )
							{
								SetStatus( new StatusClass( StatusEnum.Error, "Error reading a file. " + e.Message ) );
								return;
							}
						}

						totalSentCount++;
						//if( !fileItem.Delete )
						//	totalSentSize += fileItem.Length;

						if( totalSentSize > SendFilesMaxSize )
							break;
					}

					writer.Write( false );
					EndMessage();

					//UpdateRemainsToUpdate();

					packetSent = true;
					packedSentCount = totalSentCount;
					packedSentSize = totalSentSize;
				}

				if( !packetSent && remainsFilesToCommit.Count == 0 )
					SetStatus( new StatusClass( StatusEnum.Success, "" ) );
			}
		}

		//void UpdateRemainsToUpdate()
		//{
		//	var size = 0L;
		//	foreach( var fileInfo in remainsFilesToCommit )
		//		size += fileInfo.Length;
		//	RemainsSizeToUpdate = size;
		//}

		public void BeginUpload()
		{
			SetStatus( new StatusClass( StatusEnum.Updating, "" ) );

			//UpdateRemainsToUpdate();
		}

		public int GetRemainsCount()
		{
			return remainsFilesToCommit.Count + packedSentCount;
		}

		public long GetRemainsSize()
		{
			var size = 0L;
			foreach( var fileInfo in remainsFilesToCommit )
				size += fileInfo.SendRemainingLength;//Length;
			size += packedSentSize;
			return size;
		}

		//public int RemainsFilesToCommitCount
		//{
		//	get { return remainsFilesToCommit.Count; }
		//}
	}
}
#endif
#endif