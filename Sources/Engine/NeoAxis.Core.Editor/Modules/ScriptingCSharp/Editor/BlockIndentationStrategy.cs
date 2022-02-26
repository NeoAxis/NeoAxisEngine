// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace NeoAxis.Editor
{
	internal class BlockIndentationStrategy : IIndentationStrategy
	{
		/// <summary>
		/// Handles indentation by copying the indentation from the previous line.
		/// Does not support indenting multiple lines.
		/// Inserting indentation symbol after "{"
		/// </summary>
		public virtual void IndentLine( TextDocument document, DocumentLine line )
		{
			if( document == null )
				throw new ArgumentNullException( "document" );
			if( line == null )
				throw new ArgumentNullException( "line" );
			DocumentLine previousLine = line.PreviousLine;
			if( previousLine != null )
			{
				ISegment indentationSegment = TextUtilities.GetWhitespaceAfter( document, previousLine.Offset );
				string indentation = document.GetText( indentationSegment );

				var str = document.Text.Substring( previousLine.Offset, previousLine.Length );
				str = str.Trim();

				if( str.EndsWith( "{" ) )
					indentation += "\t";

				// Сopy indentation to line
				indentationSegment = TextUtilities.GetWhitespaceAfter( document, line.Offset );
				document.Replace( indentationSegment.Offset, indentationSegment.Length, indentation, OffsetChangeMappingType.RemoveAndInsert );

				// OffsetChangeMappingType.RemoveAndInsert guarantees the caret moves behind the new indentation
			}
		}

		public virtual void IndentLines( TextDocument document, int beginLine, int endLine )
		{
		}
	}
}
