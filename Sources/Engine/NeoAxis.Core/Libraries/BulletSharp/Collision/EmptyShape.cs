using static Internal.BulletSharp.UnsafeNativeMethods;
namespace Internal.BulletSharp
{
	public class EmptyShape : ConcaveShape
	{
		public EmptyShape()
			: base(btEmptyShape_new())
		{
		}
	}
}
