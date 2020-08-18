using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ImageAttributes : ICloneable, IDisposable
	{
		public ImageAttributes()
		{
			throw null;
		}

		public void Dispose()
		{
			throw null;
		}

		~ImageAttributes()
		{
			throw null;
		}

		public object Clone()
		{
			throw null;
		}

		public void SetColorMatrix(ColorMatrix newColorMatrix)
		{
			throw null;
		}

		public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags)
		{
			throw null;
		}

		public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag mode, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearColorMatrix()
		{
			throw null;
		}

		public void ClearColorMatrix(ColorAdjustType type)
		{
			throw null;
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
		{
			throw null;
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
		{
			throw null;
		}

		public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
		{
			throw null;
		}

		public void SetThreshold(float threshold)
		{
			throw null;
		}

		public void SetThreshold(float threshold, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearThreshold()
		{
			throw null;
		}

		public void ClearThreshold(ColorAdjustType type)
		{
			throw null;
		}

		public void SetGamma(float gamma)
		{
			throw null;
		}

		public void SetGamma(float gamma, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearGamma()
		{
			throw null;
		}

		public void ClearGamma(ColorAdjustType type)
		{
			throw null;
		}

		public void SetNoOp()
		{
			throw null;
		}

		public void SetNoOp(ColorAdjustType type)
		{
			throw null;
		}

		public void ClearNoOp()
		{
			throw null;
		}

		public void ClearNoOp(ColorAdjustType type)
		{
			throw null;
		}

		public void SetColorKey(Color colorLow, Color colorHigh)
		{
			throw null;
		}

		public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearColorKey()
		{
			throw null;
		}

		public void ClearColorKey(ColorAdjustType type)
		{
			throw null;
		}

		public void SetOutputChannel(ColorChannelFlag flags)
		{
			throw null;
		}

		public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearOutputChannel()
		{
			throw null;
		}

		public void ClearOutputChannel(ColorAdjustType type)
		{
			throw null;
		}

		public void SetOutputChannelColorProfile(string colorProfileFilename)
		{
			throw null;
		}

		public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearOutputChannelColorProfile()
		{
			throw null;
		}

		public void ClearOutputChannelColorProfile(ColorAdjustType type)
		{
			throw null;
		}

		public void SetRemapTable(ColorMap[] map)
		{
			throw null;
		}

		public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
		{
			throw null;
		}

		public void ClearRemapTable()
		{
			throw null;
		}

		public void ClearRemapTable(ColorAdjustType type)
		{
			throw null;
		}

		public void SetBrushRemapTable(ColorMap[] map)
		{
			throw null;
		}

		public void ClearBrushRemapTable()
		{
			throw null;
		}

		public void SetWrapMode(WrapMode mode)
		{
			throw null;
		}

		public void SetWrapMode(WrapMode mode, Color color)
		{
			throw null;
		}

		public void SetWrapMode(WrapMode mode, Color color, bool clamp)
		{
			throw null;
		}

		public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
		{
			throw null;
		}
	}
}
