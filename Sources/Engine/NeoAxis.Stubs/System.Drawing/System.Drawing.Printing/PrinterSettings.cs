using System.Collections;
using System.Drawing.Imaging;

namespace System.Drawing.Printing
{
	[Serializable]
	public class PrinterSettings : ICloneable
	{
		public class PaperSizeCollection : ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			public virtual PaperSize this[int index]
			{
				get
				{
					throw null;
				}
			}

			int ICollection.Count
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			public PaperSizeCollection(PaperSize[] array)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw null;
			}

			public void CopyTo(PaperSize[] paperSizes, int index)
			{
				throw null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw null;
			}

			public int Add(PaperSize paperSize)
			{
				throw null;
			}
		}

		public class PaperSourceCollection : ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			public virtual PaperSource this[int index]
			{
				get
				{
					throw null;
				}
			}

			int ICollection.Count
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			public PaperSourceCollection(PaperSource[] array)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw null;
			}

			public void CopyTo(PaperSource[] paperSources, int index)
			{
				throw null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw null;
			}

			public int Add(PaperSource paperSource)
			{
				throw null;
			}
		}

		public class PrinterResolutionCollection : ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			public virtual PrinterResolution this[int index]
			{
				get
				{
					throw null;
				}
			}

			int ICollection.Count
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			public PrinterResolutionCollection(PrinterResolution[] array)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw null;
			}

			public void CopyTo(PrinterResolution[] printerResolutions, int index)
			{
				throw null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw null;
			}

			public int Add(PrinterResolution printerResolution)
			{
				throw null;
			}
		}

		public class StringCollection : ICollection, IEnumerable
		{
			public int Count
			{
				get
				{
					throw null;
				}
			}

			public virtual string this[int index]
			{
				get
				{
					throw null;
				}
			}

			int ICollection.Count
			{
				get
				{
					throw null;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					throw null;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					throw null;
				}
			}

			public StringCollection(string[] array)
			{
				throw null;
			}

			public IEnumerator GetEnumerator()
			{
				throw null;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				throw null;
			}

			public void CopyTo(string[] strings, int index)
			{
				throw null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw null;
			}

			public int Add(string value)
			{
				throw null;
			}
		}

		public bool CanDuplex
		{
			get
			{
				throw null;
			}
		}

		public short Copies
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool Collate
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PageSettings DefaultPageSettings
		{
			get
			{
				throw null;
			}
		}

		public Duplex Duplex
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int FromPage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public static StringCollection InstalledPrinters
		{
			get
			{
				throw null;
			}
		}

		public bool IsDefaultPrinter
		{
			get
			{
				throw null;
			}
		}

		public bool IsPlotter
		{
			get
			{
				throw null;
			}
		}

		public bool IsValid
		{
			get
			{
				throw null;
			}
		}

		public int LandscapeAngle
		{
			get
			{
				throw null;
			}
		}

		public int MaximumCopies
		{
			get
			{
				throw null;
			}
		}

		public int MaximumPage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public int MinimumPage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public string PrintFileName
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PaperSizeCollection PaperSizes
		{
			get
			{
				throw null;
			}
		}

		public PaperSourceCollection PaperSources
		{
			get
			{
				throw null;
			}
		}

		public PrintRange PrintRange
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public bool PrintToFile
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public string PrinterName
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PrinterResolutionCollection PrinterResolutions
		{
			get
			{
				throw null;
			}
		}

		public bool SupportsColor
		{
			get
			{
				throw null;
			}
		}

		public int ToPage
		{
			get
			{
				throw null;
			}
			set
			{
				throw null;
			}
		}

		public PrinterSettings()
		{
			throw null;
		}

		public bool IsDirectPrintingSupported(ImageFormat imageFormat)
		{
			throw null;
		}

		public bool IsDirectPrintingSupported(Image image)
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public Graphics CreateMeasurementGraphics()
		{
			throw null;
		}

		public Graphics CreateMeasurementGraphics(bool honorOriginAtMargins)
		{
			throw null;
		}

		public Graphics CreateMeasurementGraphics(PageSettings pageSettings)
		{
			throw null;
		}

		public Graphics CreateMeasurementGraphics(PageSettings pageSettings, bool honorOriginAtMargins)
		{
			throw null;
		}

		public IntPtr GetHdevmode()
		{
			throw null;
		}

		public IntPtr GetHdevmode(PageSettings pageSettings)
		{
			throw null;
		}

		public IntPtr GetHdevnames()
		{
			throw null;
		}

		public void SetHdevmode(IntPtr hdevmode)
		{
			throw null;
		}

		public void SetHdevnames(IntPtr hdevnames)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
