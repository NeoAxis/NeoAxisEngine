namespace System.Windows.Forms.Design.Behavior
{
	public sealed class SnapLine
	{
		public string Filter
		{
			get
			{
				throw null;
			}
		}

		public bool IsHorizontal
		{
			get
			{
				throw null;
			}
		}

		public bool IsVertical
		{
			get
			{
				throw null;
			}
		}

		public int Offset
		{
			get
			{
				throw null;
			}
		}

		public SnapLinePriority Priority
		{
			get
			{
				throw null;
			}
		}

		public SnapLineType SnapLineType
		{
			get
			{
				throw null;
			}
		}

		public SnapLine(SnapLineType type, int offset)
		{
			throw null;
		}

		public SnapLine(SnapLineType type, int offset, string filter)
		{
			throw null;
		}

		public SnapLine(SnapLineType type, int offset, SnapLinePriority priority)
		{
			throw null;
		}

		public SnapLine(SnapLineType type, int offset, string filter, SnapLinePriority priority)
		{
			throw null;
		}

		public void AdjustOffset(int adjustment)
		{
			throw null;
		}

		public static bool ShouldSnap(SnapLine line1, SnapLine line2)
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
