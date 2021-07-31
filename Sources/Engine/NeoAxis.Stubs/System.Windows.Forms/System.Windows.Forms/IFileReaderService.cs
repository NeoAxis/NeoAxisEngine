using System.IO;

namespace System.Windows.Forms
{
	public interface IFileReaderService
	{
		Stream OpenFileFromSource(string relativePath);
	}
}
