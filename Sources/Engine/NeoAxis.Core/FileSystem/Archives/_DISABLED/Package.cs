// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace NeoAxis
{
	//!!!!!скорее нужен некий abstract virtual resource container
	//!!!!threading

	//public abstract class Package
	//{
	//	string realFileName;
	//	InfoClass info = new InfoClass();

	//	/////////////////////////////////////////

	//	public class InfoClass
	//	{
	//		bool loadAtStartup = true;
	//		float loadingPriority = .5f;

	//		public bool LoadAtStartup
	//		{
	//			get { return loadAtStartup; }
	//			set { loadAtStartup = value; }
	//		}

	//		public float LoadingPriority
	//		{
	//			get { return loadingPriority; }
	//			set { loadingPriority = value; }
	//		}
	//	}

	//	/////////////////////////////////////////

	//	public struct FileInfo
	//	{
	//		string fileName;
	//		long length;

	//		public FileInfo( string fileName, long length )
	//		{
	//			this.fileName = fileName;
	//			this.length = length;
	//		}

	//		public string FileName
	//		{
	//			get { return fileName; }
	//		}

	//		public long Length
	//		{
	//			get { return length; }
	//		}
	//	}

	//	/////////////////////////////////////////

	//	public string RealFileName
	//	{
	//		get { return realFileName; }
	//	}

	//	public InfoClass Info
	//	{
	//		get { return info; }
	//	}

	//	/// <summary>Releases the resources that are used by the object.</summary>
	//	public virtual void Dispose() { }

	//	protected Package( string realFileName, bool loadInfoOnly, out string error )
	//	{
	//		this.realFileName = realFileName;
	//		error = "";
	//	}

	//	protected internal abstract void OnGetDirectoryAndFileList( out string[] directories, out FileInfo[] files );

	//	protected internal abstract VirtualFileStream OnFileOpen( string inArchiveFileName );

	//	public void GetDirectoryAndFileList( out string[] directories, out FileInfo[] files )
	//	{
	//		OnGetDirectoryAndFileList( out directories, out files );
	//	}
	//}
}
