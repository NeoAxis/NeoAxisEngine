using System.Drawing;
using System.Diagnostics;

namespace Internal.ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	///
	/// </summary>
	internal class AdjustableContentValue : IContentValues
	{
		#region Instance Fields
		private KryptonForm _form;
		private bool _showImage;
		private bool _showText;
		#endregion

		#region Identity
		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		/// <param name="showImage"></param>
		/// <param name="showText"></param>
		public AdjustableContentValue(KryptonForm form, bool showImage, bool showText)
		{
			Debug.Assert(form != null);
			_form = form;
			_showImage = showImage;
			_showText = showText;
		}
		#endregion

		#region HasContent
		/// <summary>
		/// Gets a value indicating if the mapping produces any content.
		/// </summary>
		public bool HasContent
		{
			get
			{
				if (_showImage && GetImage(PaletteState.Normal) != null)
					return true;

				if (_showText && (!string.IsNullOrEmpty(GetShortText()) || !string.IsNullOrEmpty(GetLongText())))
					return true;

				return false;
			}
		}
		#endregion

		#region IContentValues
		/// <summary>
		/// Gets the content image.
		/// </summary>
		/// <param name="state">The state for which the image is needed.</param>
		/// <returns>Image value.</returns>
		public Image GetImage(PaletteState state)
		{
			return _showImage ? _form.GetImage(state) : null;
		}

		/// <summary>
		/// Gets the image color that should be transparent.
		/// </summary>
		/// <param name="state">The state for which the image is needed.</param>
		/// <returns>Color value.</returns>
		public Color GetImageTransparentColor(PaletteState state)
		{
			return _showImage ? _form.GetImageTransparentColor(state) : Color.Empty;
		}

		/// <summary>
		/// Gets the content short text.
		/// </summary>
		/// <returns>String value.</returns>
		public string GetShortText()
		{
			return _showText ? _form.GetShortText() : string.Empty;
		}

		/// <summary>
		/// Gets the content long text.
		/// </summary>
		/// <returns>String value.</returns>
		public string GetLongText()
		{
			return _showText ? _form.GetLongText() : string.Empty;
		}
		#endregion
	}
}
