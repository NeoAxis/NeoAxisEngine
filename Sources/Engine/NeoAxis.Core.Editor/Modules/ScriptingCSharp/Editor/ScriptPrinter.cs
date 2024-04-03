// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal.ComponentFactory.Krypton.Toolkit;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeoAxis.Editor
{
	internal class ScriptPrinter : EditorAssemblyInterface.IScriptPrinter
	{
		static TextEditor editor;

		static IHighlightingDefinition loadedHighlightingDefinition;

		internal FontFamily FontFamily { get; set; } = new FontFamily( "Consolas" );
		internal double FontSize { get; set; } = 12;
		internal Thickness Padding { get; set; } = new Thickness( 4, 4, 4, 4 );
		internal int IndentationSize { get; set; } = 4;
		internal Size? PageSize { get; set; } = null;
		//internal Brush BackgroundBrush { get; set; } = Brushes.White;

		static TextEditor GetOrCreateTextEditor( string script, FontFamily fontFamily, double fontSize, int indent )
		{
			if( editor == null )
			{
				editor = new TextEditor();
				editor.ShowLineNumbers = false;

				if( loadedHighlightingDefinition == null )
				{
					string path;
					if( EditorAPI2.DarkTheme )
						path = @"Base\Tools\Highlighting\CSharpDark.xshd";
					else
						path = @"Base\Tools\Highlighting\CSharpLight.xshd";

					try
					{
						var fullPath = VirtualPathUtility.GetRealPathByVirtual( path );
						if( File.Exists( fullPath ) )
							loadedHighlightingDefinition = HighlightingManager.Instance.LoadFromFile( fullPath );
					}
					catch( Exception e )
					{
						Log.Warning( "Updating highlighting scheme error. " + e.Message );
					}
				}

				if( loadedHighlightingDefinition != null )
					editor.SyntaxHighlighting = loadedHighlightingDefinition;

				////TODO: define custom highlighting rules. see CSharp-Mode.xshd
				//editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.
				//	HighlightingManager.Instance.GetDefinitionByExtension( ".cs" );

				////!!!!
				//// or we can patch existing rule:
				//editor.SyntaxHighlighting.GetNamedColor( "ValueTypeKeywords" ).Foreground =
				//	new ICSharpCode.AvalonEdit.Highlighting.SimpleHighlightingBrush( Colors.DarkCyan );
			}

			editor.FontFamily = fontFamily;
			editor.FontSize = fontSize;
			editor.Options.IndentationSize = indent;

			//TODO: fix this workaround - Options.IndentationSize is not taken into account when printing text
			script = script.Replace( "\t", new string( ' ', editor.Options.IndentationSize ) );
			//

			editor.Text = script;

			return editor;
		}

		public BitmapSource PrintToBitmap( string script, Size size )
		{
			var editor = GetOrCreateTextEditor( script, FontFamily, FontSize, IndentationSize );

			var document = DocumentPrinter.CreateFlowDocumentForEditor( editor );
			document.PagePadding = Padding;

			if( loadedHighlightingDefinition != null )
			{
				var defaultNamedColor = loadedHighlightingDefinition.GetNamedColor( "Default" );
				if( defaultNamedColor != null )
					document.Foreground = defaultNamedColor.Foreground.GetBrush( null );
			}

			return FlowDocumentToBitmap( document, size );
		}

		public void PrintToPngFile( string script, Size size, string fileName )
		{
			var bitmap = PrintToBitmap( script, size );
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add( BitmapFrame.Create( bitmap ) );
			using( var stream = new FileStream( fileName, FileMode.Create ) )
				encoder.Save( stream );
		}

		public byte[] PrintToByteArray( string script, Size size )
		{
			var bitmap = PrintToBitmap( script, size );

			int rawStride = ( bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7 ) / 8;
			byte[] result = new byte[ rawStride * bitmap.PixelHeight ];
			bitmap.CopyPixels( result, rawStride, 0 );
			return result;
		}

		public ImageComponent PrintToTexture( string script, Vector2I size )
		{
			var data = PrintToByteArray( script, new Size( size.X, size.Y ) );

			var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

			texture.CreateType = ImageComponent.TypeEnum._2D;
			texture.CreateSize = new Vector2I( (int)size.X, (int)size.Y );
			texture.CreateFormat = PixelFormat.A8R8G8B8;
			texture.CreateUsage = ImageComponent.Usages.WriteOnly;
			texture.Enabled = true;

			texture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) } );
			return texture;
		}

		BitmapSource FlowDocumentToBitmap( FlowDocument document, Size size )
		{
			//document = CloneDocument(document);

			var paginator = ( (IDocumentPaginatorSource)document ).DocumentPaginator;
			if( PageSize != null )
				paginator.PageSize = PageSize.Value; // PageSize allow align text to page size
			document.ColumnWidth = paginator.PageSize.Width; // test it.

			var visual = new DrawingVisual();
			using( var drawingContext = visual.RenderOpen() )
			{
				var color = EditorAPI2.DarkTheme ? ProjectSettings.Get.CSharpEditor.CSharpEditorBackgroundColorDarkTheme : ProjectSettings.Get.CSharpEditor.CSharpEditorBackgroundColorLightTheme;

				var packed = color.Value.ToColorPacked();
				var brush = new SolidColorBrush( Color.FromArgb( packed.Alpha, packed.Red, packed.Green, packed.Blue ) );
				drawingContext.DrawRectangle( brush, null, new Rect( size ) );// draw background

				//drawingContext.DrawRectangle( BackgroundBrush, null, new Rect( size ) );// draw background
			}
			visual.Children.Add( paginator.GetPage( 0 ).Visual );

			var dpi = 96.0 * DpiHelper.Default.DpiScaleFactor;
			var bitmap = new RenderTargetBitmap( (int)size.Width, (int)size.Height, dpi, dpi, PixelFormats.Pbgra32 );

			bitmap.Render( visual );
			return bitmap;
		}

		//FlowDocument CloneDocument(FlowDocument document)
		//{
		//    var copy = new FlowDocument();
		//    var sourceRange = new TextRange(document.ContentStart, document.ContentEnd);
		//    var targetRange = new TextRange(copy.ContentStart, copy.ContentEnd);

		//    using (var stream = new MemoryStream())
		//    {
		//        sourceRange.Save(stream, DataFormats.XamlPackage);
		//        targetRange.Load(stream, DataFormats.XamlPackage);
		//    }

		//    return copy;
		//}
	}
}
