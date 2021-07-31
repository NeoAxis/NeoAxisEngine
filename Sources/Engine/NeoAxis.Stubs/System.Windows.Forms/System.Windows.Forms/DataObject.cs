using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms
{
	public class DataObject : IDataObject, System.Runtime.InteropServices.ComTypes.IDataObject
	{
		public DataObject()
		{
			throw null;
		}

		public DataObject(object data)
		{
			throw null;
		}

		public DataObject(string format, object data)
		{
			throw null;
		}

		public virtual object GetData(string format, bool autoConvert)
		{
			throw null;
		}

		public virtual object GetData(string format)
		{
			throw null;
		}

		public virtual object GetData(Type format)
		{
			throw null;
		}

		public virtual bool GetDataPresent(Type format)
		{
			throw null;
		}

		public virtual bool GetDataPresent(string format, bool autoConvert)
		{
			throw null;
		}

		public virtual bool GetDataPresent(string format)
		{
			throw null;
		}

		public virtual string[] GetFormats(bool autoConvert)
		{
			throw null;
		}

		public virtual string[] GetFormats()
		{
			throw null;
		}

		public virtual bool ContainsAudio()
		{
			throw null;
		}

		public virtual bool ContainsFileDropList()
		{
			throw null;
		}

		public virtual bool ContainsImage()
		{
			throw null;
		}

		public virtual bool ContainsText()
		{
			throw null;
		}

		public virtual bool ContainsText(TextDataFormat format)
		{
			throw null;
		}

		public virtual Stream GetAudioStream()
		{
			throw null;
		}

		public virtual StringCollection GetFileDropList()
		{
			throw null;
		}

		public virtual Image GetImage()
		{
			throw null;
		}

		public virtual string GetText()
		{
			throw null;
		}

		public virtual string GetText(TextDataFormat format)
		{
			throw null;
		}

		public virtual void SetAudio(byte[] audioBytes)
		{
			throw null;
		}

		public virtual void SetAudio(Stream audioStream)
		{
			throw null;
		}

		public virtual void SetFileDropList(StringCollection filePaths)
		{
			throw null;
		}

		public virtual void SetImage(Image image)
		{
			throw null;
		}

		public virtual void SetText(string textData)
		{
			throw null;
		}

		public virtual void SetText(string textData, TextDataFormat format)
		{
			throw null;
		}

		int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
		{
			throw null;
		}

		void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise(int dwConnection)
		{
			throw null;
		}

		int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
		{
			throw null;
		}

		IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection)
		{
			throw null;
		}

		int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut)
		{
			throw null;
		}

		void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium)
		{
			throw null;
		}

		void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium)
		{
			throw null;
		}

		int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc)
		{
			throw null;
		}

		void System.Runtime.InteropServices.ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease)
		{
			throw null;
		}

		public virtual void SetData(string format, bool autoConvert, object data)
		{
			throw null;
		}

		public virtual void SetData(string format, object data)
		{
			throw null;
		}

		public virtual void SetData(Type format, object data)
		{
			throw null;
		}

		public virtual void SetData(object data)
		{
			throw null;
		}
	}
}
