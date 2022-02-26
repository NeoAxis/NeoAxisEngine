using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class CollisionDispatcherMultiThreaded : CollisionDispatcher
	{
		public CollisionDispatcherMultiThreaded(CollisionConfiguration configuration, int grainSize = 40)
			: base(btCollisionDispatcherMt_new(configuration.Native, grainSize))
		{
			_collisionConfiguration = configuration;
		}
	}
}
