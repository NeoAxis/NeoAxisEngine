using System.Collections.Specialized;
using System.Drawing;
using System.IO;

namespace System.Windows.Forms
{
	public sealed class Clipboard
	{
		public static void SetDataObject(object data)
		{
			throw null;
		}

		public static void SetDataObject(object data, bool copy)
		{
			throw null;
		}

		public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
		{
			throw null;
		}

		public static IDataObject GetDataObject()
		{
			throw null;
		}

		public static void Clear()
		{
			throw null;
		}

		public static bool ContainsAudio()
		{
			throw null;
		}

		public static bool ContainsData(string format)
		{
			throw null;
		}

		public static bool ContainsFileDropList()
		{
			throw null;
		}

		public static bool ContainsImage()
		{
			throw null;
		}

		public static bool ContainsText()
		{
			throw null;
		}

		public static bool ContainsText(TextDataFormat format)
		{
			throw null;
		}

		public static Stream GetAudioStream()
		{
			throw null;
		}

		public static object GetData(string format)
		{
			throw null;
		}

		public static StringCollection GetFileDropList()
		{
			throw null;
		}

		public static Image GetImage()
		{
			throw null;
		}

		public static string GetText()
		{
			throw null;
		}

		public static string GetText(TextDataFormat format)
		{
			throw null;
		}

		public static void SetAudio(byte[] audioBytes)
		{
			throw null;
		}

		public static void SetAudio(Stream audioStream)
		{
			throw null;
		}

		public static void SetData(string format, object data)
		{
			throw null;
		}

		public static void SetFileDropList(StringCollection filePaths)
		{
			throw null;
		}

		public static void SetImage(Image image)
		{
			throw null;
		}

		public static void SetText(string text)
		{
			throw null;
		}

		public static void SetText(string text, TextDataFormat format)
		{
			throw null;
		}
	}
}
