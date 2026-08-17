// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using NeoAxis.Networking;
using NeoAxis.Cloud;
using System.IO.Compression;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Chat AI window.
	/// </summary>
	public partial class ChatWindow : DockWindow
	{
		const bool EnableAI = false;

		public static readonly bool EnableNeoXForgeStore = EnableAI;
		public static readonly bool EnableChatAssets = EnableAI;
		public static readonly bool EnableChatDocument = EnableAI;

		static ChatWindow instance;

		static long lastChatID;

		//public static bool needOpenOptions;

		///////////////////////////////////////////////

		public enum ChatModeEnum
		{
			Assets,
			Document,
		}

		///////////////////////////////////////////////

		public ChatWindow()
		{
			instance = this;

			InitializeComponent();

			toolStripButtonOptions.Image = EditorResourcesCache.Options;
			//toolStripButtonRefresh.Image = EditorResourcesCache.Refresh;

			foreach( var item in toolStripForTreeView.Items )
			{
				var button = item as ToolStripButton;
				if( button != null )
					button.Text = EditorLocalization2.Translate( "ChatWindow", button.Text );

				var button2 = item as ToolStripDropDownButton;
				if( button2 != null )
					button2.Text = EditorLocalization2.Translate( "ChatWindow", button2.Text );
			}

			toolStripForTreeView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();

			Config_Load();
			EngineConfig.SaveEvent += Config_SaveEvent;

			WindowTitle = EditorLocalization2.Translate( "Windows", WindowTitle );

			//toolStripButtonSearch.TextChanged += toolStripButtonSearch_TextChanged;
			toolStripForTreeView.Layout += ToolStripForTreeView_Layout;
			Resize += ChatWindow_Resize;

			//welcome text
			if( EnableChatDocument )
			{
				textBox1.Text = "1. To generate resources, use NeoX Forge in the NeoX app. Generated content will appear in the Stores window of the editor.\r\n\r\n2. To generate assets, use the context menu in the Resources window.\r\n\r\n3. Use the context menu in the document workspace to open the Chat AI form.";
			}
			else
				textBox1.Text = "Chat AI features are disabled.";
		}

		public override bool HideOnRemoving { get { return true; } }

		private void ChatWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			timer1.Start();

			//update toolstrip sizes
			{
				var dpiScale = Math.Min( EditorAPI2.DPIScale, 2 );

				void UpdateSize( ToolStripItem item )
				{
					int width = 20;
					if( item is ToolStripDropDownButton )
						width = 28;
					item.Size = new Size( (int)( width * dpiScale ), (int)( 20 * dpiScale + 2 ) );
				}

				toolStripForTreeView.Padding = new Padding( (int)dpiScale );
				toolStripForTreeView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );

				foreach( var item in toolStripForTreeView.Items )
				{
					var button = item as ToolStripButton;
					if( button != null )
						UpdateSize( button );

					var button2 = item as ToolStripDropDownButton;
					if( button2 != null )
						UpdateSize( button2 );
				}


				toolStripForTreeView.Padding = new Padding( (int)dpiScale );
				toolStripForTreeView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );
			}

			UpdateControls();

			EditorAPI.ClosingApplicationChanged += EditorAPI_ClosingApplicationChanged;
		}

		protected override void OnDestroy()
		{
			EditorAPI.ClosingApplicationChanged -= EditorAPI_ClosingApplicationChanged;

			base.OnDestroy();
		}

		[Browsable( false )]
		public NeoAxis.Editor.EngineTextBox TextBox1
		{
			get { return textBox1; }
		}

		//public override ObjectsInFocus GetObjectsInFocus()
		//{
		//	return new ObjectsInFocus( null, contentBrowser1.SelectedItems );
		//}

		void Config_Load()
		{
			var windowBlock = EngineConfig.TextBlock.FindChild( nameof( ChatWindow ) );
			if( windowBlock != null )
			{
				//var filterBlock = windowBlock.FindChild( "Filter" );
				//if( filterBlock != null )
				//	filterSettings.Load( filterBlock );

				//SelectedStoreChanged();
			}
		}

		void Config_SaveEvent()
		{
			var configBlock = EngineConfig.TextBlock;

			var old = configBlock.FindChild( nameof( ChatWindow ) );
			if( old != null )
				configBlock.DeleteChild( old );

			//var filterBlock = windowBlock.AddChild( "Filter" );
			//filterSettings.Save( filterBlock );
		}

		void UpdateControls()
		{
			var parentSize = textBox1.Parent.ClientSize;
			var border = (int)( 4 * EditorAPI2.DPIScale );
			textBox1.Location = new Point( border, toolStripForTreeView.Height + border );
			textBox1.Size = new Size( parentSize.Width - border * 2, parentSize.Height - toolStripForTreeView.Height - border * 2 );
		}

		private void ToolStripForTreeView_Layout( object sender, LayoutEventArgs e )
		{
			int width = toolStripForTreeView.DisplayRectangle.Width;

			//foreach( ToolStripItem item in toolStripForTreeView.Items )
			//{
			//	if( !( item == toolStripButtonSearch ) )
			//	{
			//		width -= item.Width;
			//		width -= item.Margin.Horizontal;
			//	}
			//}

			//toolStripButtonSearch.Width = Math.Max( 0, width - toolStripButtonSearch.Margin.Horizontal - 1 );
		}

		private void ChatWindow_Resize( object sender, EventArgs e )
		{
			UpdateControls();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateControls();

			//var currentTime = EngineApp.GetSystemTime();

			//if( needOpenOptions )
			//{
			//	needOpenOptions = false;
			//	toolStripButtonOptions_Click( null, null );
			//}
		}

		private void toolStripButtonOptions_Click( object sender, EventArgs e )
		{
			var form = new ChatWindowOptionsForm();// contentBrowser1 );

			if( EditorForm.Instance == null )
				form.ShowDialog();
			else
			{
				EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
				{
					form.ShowDialog();
				} );
			}
		}

		//private void toolStripButtonRefresh_Click( object sender, EventArgs e )
		//{
		//}

		static string Translate( string text )
		{
			return EditorLocalization2.Translate( "ChatWindow", text );
		}

		private void EditorAPI_ClosingApplicationChanged()
		{
			if( EditorAPI.ClosingApplication )
			{
			}
		}

		public void SetText( string text )
		{
			textBox1.Text = text;
		}

		public void AddText( string text )
		{
			textBox1.Text += text;
		}

		static async Task DeleteItemAndChatAsync( string itemID, long chatID )
		{
			var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
			var client2 = await NeoXForgeImplementation.Instance.GetOrConnectClientAsync( cts.Token );

			//delete item
			{
				var result = await client2.CallMethodAsync( "CloudServerImplementation", "DeleteItemsAndTasks", cts.Token, (object)new string[] { itemID } );
				if( !string.IsNullOrEmpty( result.Error ) && !result.Error.StartsWith( "Item not found" ) )
					throw new Exception( result.Error );
			}

			//delete chat
			if( chatID != 0 )
			{
				var deleteChatResult = await client2.CallMethodAsync( "Chats", "UpdateChat", cts.Token, chatID, "Deleted", null, null );
				if( !string.IsNullOrEmpty( deleteChatResult.Error ) )
					throw new Exception( deleteChatResult.Error );
			}
		}

		public static void ShowChatPromptFormForDocument( IDocumentWindow documentWindow, object[] selectedObjects )
		{
			if( instance == null )
			{
				Log.Warning( "ChatWindow.ShowChatPromptForm: ChatWindow instance is null." );
				return;
			}
			if( NeoXForgeImplementation.Instance == null )
			{
				Log.Warning( "ChatWindow.ShowChatPromptForm: NeoXForgeImplementation instance is null." );
				return;
			}

			var rootComponent = documentWindow.ObjectOfWindow as Component;
			if( rootComponent == null )
			{
				Log.Warning( "ChatWindow.ShowChatPromptForm: Root component is null." );
				return;
			}


			//show form, get prompt

			var selectedObjects2 = selectedObjects.OfType<Component>().ToArray();

			var form = new ChatPromptForm( ChatModeEnum.Document, documentWindow, selectedObjects2, null );
			if( form.ShowDialog( EditorForm.Instance ) != DialogResult.OK )
				return;

			var textPrompt = form.textBoxName.Text.Trim();
			var editOnlySelectedObjects = form.checkBoxEditSelectedOnly.Checked;

			instance.SetText( $"Prompt: {textPrompt}\r\nEdit only selected objects: {editOnlySelectedObjects}\r\nSelected objects count: {selectedObjects2.Length}\r\n\r\nProcessing..." );


			//start processing

			var cts = new CancellationTokenSource( new TimeSpan( 0, 10, 0 ) );

			var newTaskID = Guid.NewGuid().ToString();
			var chatID = 0L;

			//create processing form
			ProcessingForm processingForm = null;

			Task.Run( async delegate ()
			{
				try
				{
					//get client and user ID
					var client = await NeoXForgeImplementation.Instance.GetOrConnectClientAsync( cts.Token );
					var userID = GetThisUserID( client );
					if( userID == 0 )
						throw new Exception( "User ID is not defined." );

					//start task
					var taskStartResult = await TaskStartAsync( client, userID, documentWindow, selectedObjects2, newTaskID, textPrompt, editOnlySelectedObjects, cts.Token );
					chatID = taskStartResult.ChatID;

					//wait for answer message
					var message = await WaitAnswerMessageAndEndedTaskAsync( client, userID, chatID, cts.Token );
					if( message.AnyData == null )
						throw new Exception( "Answer message doesn't contain AnyData." );
					if( cts.Token.IsCancellationRequested )
						throw new OperationCanceledException( cts.Token );

					//parse answer message
					var anyDataBlock = TextBlock.Parse( message.AnyData, out var error );
					if( anyDataBlock == null )
						throw new Exception( "Failed to parse AnyData of answer message. " + error );
					if( anyDataBlock.GetAttribute( "MessageType" ) != "Answer" )
						throw new Exception( "MessageType of answer message is not 'Answer'." );
					var status = anyDataBlock.GetAttribute( "Status" );
					if( status != "Finished" )
						throw new Exception( $"Task finished with status '{status}'." );

					//process answer message
					await ProcessAnswerMessageWithFinishedTaskAsync( documentWindow, selectedObjects2, editOnlySelectedObjects, taskStartResult.OutputStorageFileName, cts.Token );
				}
				catch( Exception e )
				{
					//show error message in chat and message box
					EngineThreading.ExecuteFromMainThreadLater( delegate ()
					{
						instance.AddText( $"\r\n\r\nError: {e.Message}" );

						if( processingForm == null || !processingForm.CancelledByUser )
							EditorMessageBox.ShowWarning( $"Error: {e.Message}" );
					} );
				}

				//close processing form
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					try
					{
						processingForm?.CloseOK();
					}
					catch { }
				} );
			} );

			async void CancelHandler()
			{
				//cancel task
				try
				{
					cts.Cancel();
					await DeleteItemAndChatAsync( newTaskID, chatID );
				}
				catch( Exception e )
				{
					//show error message in chat
					EngineThreading.ExecuteFromMainThreadLater( delegate ()
					{
						instance.AddText( $"\r\n\r\nError when cancelling: {e.Message}" );
					} );
				}

				//show cancelled message in chat
				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					instance.AddText( "\r\n\r\nCancelled by user." );
				} );
			}

			//show processing form
			processingForm = new ProcessingForm( "NeoX Forge", CancelHandler );
			processingForm.ShowDialog( EditorForm.Instance );
		}

		static long GetThisUserID( CloudFunctionsClient client )
		{
			return client.ConnectionNode?.Users.ThisUser?.UserID ?? 0;
		}

		static string GetChatNameFromText( string text )
		{
			var chatName = text;
			chatName = chatName.Replace( "\r", "" ).Replace( "\n", " " );

			if( chatName.Length > 195 )
				chatName = chatName.Substring( 0, 195 ) + "...";

			return chatName;
		}

		class TaskStartResult
		{
			public long ChatID;
			public string OutputStorageFileName;
		}

		static async Task<TaskStartResult> TaskStartAsync( CloudFunctionsClient client, long userID, IDocumentWindow documentWindow, Component[] selectedObjects, string newTaskID, string text, bool editOnlySelectedObjects, CancellationToken cancellationToken )
		{
			var storageDirectoryName = $"{userID}/{NeoXForgeImplementation.StorageDirectory}/{newTaskID}";
			var inputStorageFileName = $"{storageDirectoryName}/Input.zip";
			var outputStorageFileName = $"{storageDirectoryName}/{newTaskID}.zip";

			var tempZipFilePath = Path.Combine( Path.GetTempPath(), "_forgeSource_" + Guid.NewGuid().ToString() + ".zip" );
			try
			{
				//prepare zip archive with input data
				using( var zipArchive = ZipFile.Open( tempZipFilePath, ZipArchiveMode.Create ) )
				{
					//Document.block
					{
						var rootComponent = documentWindow.ObjectOfWindow as Component;
						var rootBlock = ComponentUtility.SaveComponentToTextBlock( rootComponent, null, out var error );
						if( !string.IsNullOrEmpty( error ) )
							throw new Exception( "Save component to TextBlock failed. " + error );

						//create entry (with utc timestamp)
						var entry = zipArchive.CreateEntry( "Document.block", CompressionLevel.Optimal );
						entry.LastWriteTime = DateTime.UtcNow;
						using( var entryStream = entry.Open() )
						using( var writer = new StreamWriter( entryStream ) )
							writer.Write( rootBlock.DumpToString() );
					}

					//SelectedObjects.block
					if( selectedObjects.Length > 0 )
					{
						var rootBlock = new TextBlock();
						foreach( var obj in selectedObjects )
						{
							var block = rootBlock.AddChild( "Component" );
							block.SetAttribute( "Path", obj.GetPathFromRoot() );
						}

						//create entry (with utc timestamp)
						var entry = zipArchive.CreateEntry( "SelectedObjects.block", CompressionLevel.Optimal );
						entry.LastWriteTime = DateTime.UtcNow;
						using( var entryStream = entry.Open() )
						using( var writer = new StreamWriter( entryStream ) )
							writer.Write( rootBlock.DumpToString() );
					}
				}

				//auto created
				////create directory if not exists
				//{
				//	var createResult = await CloudServiceFunctions.StorageCreateDirectoryAsync( storageDirectoryName, null, cancellationToken );
				//	if( !string.IsNullOrEmpty( createResult.Error ) )
				//		throw new Exception( createResult.Error );
				//	if( cancellationToken.IsCancellationRequested )
				//		throw new OperationCanceledException( cancellationToken );
				//}

				//upload input file
				{
					//get content url
					var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( inputStorageFileName, true, false, "", cancellationToken );
					if( !string.IsNullOrEmpty( getContentUrlResult.Error ) )
						throw new Exception( getContentUrlResult.Error );
					if( cancellationToken.IsCancellationRequested )
						throw new OperationCanceledException( cancellationToken );

					//upload file
					var uploadResult = await NetworkUtility.UploadFileByUrlAsync( getContentUrlResult.Url, tempZipFilePath, true, null, cancellationToken );
					if( !string.IsNullOrEmpty( uploadResult.Error ) )
						throw new Exception( uploadResult.Error );
					if( cancellationToken.IsCancellationRequested )
						throw new OperationCanceledException( cancellationToken );
				}
			}
			finally
			{
				//delete temp file
				if( File.Exists( tempZipFilePath ) )
					File.Delete( tempZipFilePath );
			}

			var contentType = "NeoAxisDocument";
			var quality = "High";
			var model = "Auto";


			var textInChat = $"{text}\r\n\r\nContent type: {contentType}; Quality: {quality}; Model: {model}";

			var promptBlock = new TextBlock();
			{
				promptBlock.SetAttribute( "MessageType", "Request" );
				promptBlock.SetAttribute( "Id", newTaskID );
				promptBlock.SetAttribute( "Text", text );
				promptBlock.SetAttribute( "ContentType", contentType );
				//promptBlock.SetAttribute( "Resolution", resolution );
				promptBlock.SetAttribute( "Quality", quality );
				promptBlock.SetAttribute( "Amount", "1" );
				promptBlock.SetAttribute( "Model", model );

				//add input zip file
				{
					var fileBlock = promptBlock.AddChild( "File" );

					//get content url to download input file by server
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( inputStorageFileName, false, false, "", cts.Token );
					if( !string.IsNullOrEmpty( getContentUrlResult.Error ) )
						throw new Exception( getContentUrlResult.Error );
					fileBlock.SetAttribute( "ContentUrlToDownload", getContentUrlResult.Url );
				}

				//content url for result
				{
					//get content url
					var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( outputStorageFileName, true, false, "", cts.Token );
					if( !string.IsNullOrEmpty( getContentUrlResult.Error ) )
						throw new Exception( getContentUrlResult.Error );

					promptBlock.SetAttribute( "OutputContentUrl", getContentUrlResult.Url );
				}
			}


			//delete old items, chats
			{
				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var getChatsResult = await client.CallMethodAsync<Chats.Chat[]>( "Chats", "GetChats", cts.Token, null, new[] { userID }, new[] { "Editor" }, new[] { "Open" } );
				if( !string.IsNullOrEmpty( getChatsResult.Error ) )
					throw new Exception( getChatsResult.Error );

				var utcNow = DateTime.UtcNow;

				foreach( var chat in getChatsResult.Value )
				{
					if( chat.Id == lastChatID || ( utcNow - chat.CreationTime ).TotalHours > 1 )
					{
						if( !string.IsNullOrEmpty( chat.AnyData ) )
						{
							var itemID = chat.AnyData;

							try
							{
								await DeleteItemAndChatAsync( itemID, chat.Id );
							}
							catch( Exception e )
							{
								//show error message in chat
								EngineThreading.ExecuteFromMainThreadLater( delegate ()
								{
									instance.AddText( $"\r\n\r\nError when deleting old item and chat: {e.Message}" );
								} );
							}
						}
					}
				}
			}


			var attachments = "";
			var anyData = promptBlock.DumpToString( false );

			long chatID;

			//var isFirstMessage = ListChatMessages.GetItems().Length == 0;

			//var selectedChatID = GetSelectedChatID();
			//if( selectedChatID != 0 )
			//{
			//	//add message to the chat
			//	using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
			//	var newMessageResult = await client2.CallMethodAsync( "Chats", "NewMessage", cts.Token, selectedChatID, textInChat, attachments, anyData );
			//	if( !string.IsNullOrEmpty( newMessageResult.Error ) )
			//	{
			//		Log( "Error: " + newMessageResult.Error, true, true );
			//		return;
			//	}

			//	//rename the chat when first message sent
			//	if( isFirstMessage )
			//	{
			//		var chatName = GetChatNameFromText( text );

			//		var renameChatResult = await client2.CallMethodAsync( "Chats", "UpdateChat", cts.Token, selectedChatID, null, chatName, null );
			//		if( !string.IsNullOrEmpty( renameChatResult.Error ) )
			//		{
			//			Log( "Error: " + renameChatResult.Error, true );
			//			return;
			//		}
			//	}
			//}
			//else
			{
				//new chat when text from maintenance page

				var chatName = GetChatNameFromText( text );

				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var newChatResult = await client.CallMethodAsync<long>( "Chats", "NewChat", cts.Token, chatName, "Editor", newTaskID );
				if( !string.IsNullOrEmpty( newChatResult.Error ) )
					throw new Exception( "Create new chat failed. " + newChatResult.Error );
				chatID = newChatResult.Value;
				lastChatID = chatID;

				//add message to the chat
				using var cts2 = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				var newMessageResult = await client.CallMethodAsync( "Chats", "NewMessage", cts2.Token, chatID, textInChat, attachments, anyData );
				if( !string.IsNullOrEmpty( newMessageResult.Error ) )
					throw new Exception( "Add message to chat failed. " + newMessageResult.Error );
			}

			return new TaskStartResult()
			{
				ChatID = chatID,
				OutputStorageFileName = outputStorageFileName
			};
		}

		static async Task<Chats.Message> WaitAnswerMessageAndEndedTaskAsync( CloudFunctionsClient client, long userID, long chatID, CancellationToken cancellationToken )
		{
			var result = await client.CallMethodAsync<Chats.Message>( "CloudServerImplementation", "WaitAnswerMessageAndEndedTask", cancellationToken, chatID );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
			return result.Value;
		}

		static async Task ProcessAnswerMessageWithFinishedTaskAsync( IDocumentWindow documentWindow, Component[] selectedObjects, bool editOnlySelectedObjects, string outputStorageFileName, CancellationToken cancellationToken )
		{
			//get content url
			var getContentUrlResult = await CloudServiceFunctions.StorageGetContentUrlAsync( outputStorageFileName, false, false, "", cancellationToken );
			if( !string.IsNullOrEmpty( getContentUrlResult.Error ) )
				throw new Exception( getContentUrlResult.Error );
			if( cancellationToken.IsCancellationRequested )
				throw new OperationCanceledException( cancellationToken );

			TextBlock textBlock;

			var tempZipFullPath = Path.Combine( Path.GetTempPath(), "_forgeOutput_" + Guid.NewGuid().ToString() + ".zip" );
			try
			{
				//download file
				var downloadResult = await NetworkUtility.DownloadFileByUrlAsync( getContentUrlResult.Url, tempZipFullPath, null, cancellationToken );
				if( !string.IsNullOrEmpty( downloadResult.Error ) )
					throw new Exception( downloadResult.Error );

				//read Output.block from zip archive to memory and parse it
				using( var archive = ZipFile.OpenRead( tempZipFullPath ) )
				{
					var entryFileName = "Output.block";
					var entry = archive.GetEntry( entryFileName );
					if( entry == null )
						throw new Exception( $"File '{entryFileName}' not found in zip archive." );

					using( var entryStream = entry.Open() )
					using( var streamReader = new StreamReader( entryStream ) )
					{
						var fileContent = streamReader.ReadToEnd();

						textBlock = TextBlock.Parse( fileContent, out var error );
						if( textBlock == null )
							throw new Exception( "Failed to parse output file. " + error );
					}
				}
			}
			finally
			{
				//delete temp file
				if( File.Exists( tempZipFullPath ) )
					File.Delete( tempZipFullPath );
			}

			//process data from TextBlock and apply changes to document
			{
				var answerText = textBlock.GetAttribute( "AnswerText" );

				EngineThreading.ExecuteFromMainThreadLater( delegate ()
				{
					instance.AddText( $"\r\n\r\nAnswer: {answerText}" );
				} );
			}
		}

		public static void ShowChatPromptFormForAssets( string fullPath )
		{
			var virtualPath = VirtualPathUtility.GetVirtualPathByReal( fullPath );

			if( instance == null )
			{
				Log.Warning( "ChatWindow.ShowChatPromptForm: ChatWindow instance is null." );
				return;
			}
			if( NeoXForgeImplementation.Instance == null )
			{
				Log.Warning( "ChatWindow.ShowChatPromptForm: NeoXForgeImplementation instance is null." );
				return;
			}

			//show form, get prompt

			var form = new ChatPromptForm( ChatModeEnum.Assets, null, null, fullPath );
			if( form.ShowDialog( EditorForm.Instance ) != DialogResult.OK )
				return;

			var textPrompt = form.textBoxName.Text.Trim();
			var editOnlySelectedObjects = form.checkBoxEditSelectedOnly.Checked;

			instance.SetText( $"Prompt: {textPrompt}\r\nDirectory: {fullPath}\r\n\r\nProcessing..." );


			//!!!!
			instance.SetText( $"\r\n\r\nIMPL" );

		}
	}
}