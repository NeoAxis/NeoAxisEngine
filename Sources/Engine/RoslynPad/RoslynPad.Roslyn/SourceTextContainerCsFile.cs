//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.CodeAnalysis.Text;

//namespace RoslynPad.Roslyn
//{
//	//class SourceTextCsFile : SourceText
//	//{
//	//	public string text;

//	//	//

//	//	public override char this[ int position ]
//	//	{
//	//		get { return text[ position ]; }
//	//	}

//	//	public override Encoding Encoding
//	//	{
//	//		get
//	//		{
//	//			xx xx;
//	//		}
//	//	}

//	//	public override int Length
//	//	{
//	//		get { return text.Length; }
//	//	}

//	//	public override void CopyTo( int sourceIndex, char[] destination, int destinationIndex, int count )
//	//	{
//	//		throw new NotImplementedException();
//	//	}
//	//}

//	//!!!!?
//	class SourceTextContainerCsFile : SourceTextContainer
//	{
//		public string text;
//		public SourceText sourceText;

//		public override SourceText CurrentText
//		{
//			get
//			{
//				if( sourceText == null )
//					sourceText = SourceText.From( text );
//				return sourceText;
//			}
//		}

//		public override event EventHandler<TextChangeEventArgs> TextChanged;
//	}
//}
