using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace System.Drawing
{
	[Serializable]
	public abstract class Image : MarshalByRefObject, ISerializable, ICloneable, IDisposable
	{
		public delegate bool GetThumbnailImageAbort();

		public object Tag
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

		public SizeF PhysicalDimension
		{
			get
			{
				throw null;
			}
		}

		public Size Size
		{
			get
			{
				throw null;
			}
		}

		public int Width
		{
			get
			{
				throw null;
			}
		}

		public int Height
		{
			get
			{
				throw null;
			}
		}

		public float HorizontalResolution
		{
			get
			{
				throw null;
			}
		}

		public float VerticalResolution
		{
			get
			{
				throw null;
			}
		}

		public int Flags
		{
			get
			{
				throw null;
			}
		}

		public ImageFormat RawFormat
		{
			get
			{
				throw null;
			}
		}

		public PixelFormat PixelFormat
		{
			get
			{
				throw null;
			}
		}

		public ColorPalette Palette
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

		public Guid[] FrameDimensionsList
		{
			get
			{
				throw null;
			}
		}

		public int[] PropertyIdList
		{
			get
			{
				throw null;
			}
		}

		public PropertyItem[] PropertyItems
		{
			get
			{
				throw null;
			}
		}

		public static Image FromFile(string filename)
		{
			throw null;
		}

		public static Image FromFile(string filename, bool useEmbeddedColorManagement)
		{
			throw null;
		}

		public static Image FromStream(Stream stream)
		{
			throw null;
		}

		public static Image FromStream(Stream stream, bool useEmbeddedColorManagement)
		{
			throw null;
		}

		public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		protected virtual void Dispose(bool disposing)
		{
			throw null;
		}

		~Image()
		{
			throw null;
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw null;
		}

		public EncoderParameters GetEncoderParameterList(Guid encoder)
		{
			throw null;
		}

		public void Save(string filename)
		{
			throw null;
		}

		public void Save(string filename, ImageFormat format)
		{
			throw null;
		}

		public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
		{
			throw null;
		}

		public void Save(Stream stream, ImageFormat format)
		{
			throw null;
		}

		public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
		{
			throw null;
		}

		public void SaveAdd(EncoderParameters encoderParams)
		{
			throw null;
		}

		public void SaveAdd(Image image, EncoderParameters encoderParams)
		{
			throw null;
		}

		public RectangleF GetBounds(ref GraphicsUnit pageUnit)
		{
			throw null;
		}

		public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
		{
			throw null;
		}

		public int GetFrameCount(FrameDimension dimension)
		{
			throw null;
		}

		public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
		{
			throw null;
		}

		public void RotateFlip(RotateFlipType rotateFlipType)
		{
			throw null;
		}

		public PropertyItem GetPropertyItem(int propid)
		{
			throw null;
		}

		public void RemovePropertyItem(int propid)
		{
			throw null;
		}

		public void SetPropertyItem(PropertyItem propitem)
		{
			throw null;
		}

		public static Bitmap FromHbitmap(IntPtr hbitmap)
		{
			throw null;
		}

		public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
		{
			throw null;
		}

		public static int GetPixelFormatSize(PixelFormat pixfmt)
		{
			throw null;
		}

		public static bool IsAlphaPixelFormat(PixelFormat pixfmt)
		{
			throw null;
		}

		public static bool IsExtendedPixelFormat(PixelFormat pixfmt)
		{
			throw null;
		}

		public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
		{
			throw null;
		}
	}
}
