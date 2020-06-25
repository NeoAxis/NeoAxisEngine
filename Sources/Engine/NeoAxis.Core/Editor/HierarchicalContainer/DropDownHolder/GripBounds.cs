// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Drawing;

namespace NeoAxis.Editor
{
	internal struct GripBounds
	{
		private const int GripSize = 6;
		private const int CornerGripSize = GripSize << 1;

		public GripBounds( System.Drawing.Rectangle clientRectangle )
		{
			this.clientRectangle = clientRectangle;
		}

		private System.Drawing.Rectangle clientRectangle;
		public System.Drawing.Rectangle ClientRectangle
		{
			get { return clientRectangle; }
			//set { clientRectangle = value; }
		}

		public System.Drawing.Rectangle Bottom
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Y = rect.Bottom - GripSize + 1;
				rect.Height = GripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle BottomRight
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Y = rect.Bottom - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle Top
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Height = GripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle TopRight
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle Left
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Width = GripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle BottomLeft
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Width = CornerGripSize;
				rect.Y = rect.Height - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle Right
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.X = rect.Right - GripSize + 1;
				rect.Width = GripSize;
				return rect;
			}
		}

		public System.Drawing.Rectangle TopLeft
		{
			get
			{
				System.Drawing.Rectangle rect = ClientRectangle;
				rect.Width = CornerGripSize;
				rect.Height = CornerGripSize;
				return rect;
			}
		}
	}
}
