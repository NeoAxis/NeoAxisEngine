#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Provides read-only access to <see cref="DocumentWindow"/>.
	/// </summary>
	public interface IDocumentWindow
	{
		DocumentInstance Document { get; }
		bool IsDocumentSaved();
	}
}

#endif