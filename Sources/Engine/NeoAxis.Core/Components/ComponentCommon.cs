// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;

namespace NeoAxis
{
	public interface IVisibleInHierarchy
	{
		[Browsable( false )]
		bool VisibleInHierarchy { get; }
	}

	public interface ICanBeSelectedInHierarchy
	{
		[Browsable( false )]
		bool CanBeSelectedInHierarchy { get; }
	}

	public delegate void IUIWebBrowser_LoadStartDelegate( IUIWebBrowser sender, object cefFrame );
	public delegate void IUIWebBrowser_LoadEndDelegate( IUIWebBrowser sender, object cefFrame, int httpStatusCode );

	public interface IUIWebBrowser
	{
		[DefaultValue( "" )]
		[Serialize]
		Reference<string> StartFile
		{
			get;
			set;
		}

		//public delegate void IUIWebBrowser_LoadStartDelegate( IUIWebBrowser sender, object cefFrame );
		event IUIWebBrowser_LoadStartDelegate IUIWebBrowser_LoadStart;

		//public delegate void IUIWebBrowser_LoadEndDelegate( IUIWebBrowser sender, object cefFrame, int httpStatusCode );
		event IUIWebBrowser_LoadEndDelegate IUIWebBrowser_LoadEnd;
	}
}
