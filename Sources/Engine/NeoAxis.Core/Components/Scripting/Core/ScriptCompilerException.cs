// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.Serialization;

namespace NeoAxis
{
	/// <summary>
	///
	/// </summary>
	[Serializable]
	class ScriptCompilerException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptCompilerException"/> class.
		/// </summary>
		public ScriptCompilerException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptCompilerException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public ScriptCompilerException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptCompilerException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ScriptCompilerException( string message )
			: base( message )
		{
		}
	}
}