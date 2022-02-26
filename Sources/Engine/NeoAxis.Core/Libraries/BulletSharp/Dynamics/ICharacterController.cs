using Internal.BulletSharp.Math;

namespace Internal.BulletSharp
{
	public interface ICharacterController : IAction
	{
        bool CanJump { get; }
        bool OnGround { get; }

        void Jump();
        void Jump(BVector3 dir);
        void PlayerStep(CollisionWorld collisionWorld, double deltaTime);
        void PreStep(CollisionWorld collisionWorld);
        void Reset(CollisionWorld collisionWorld);
        void SetUpInterpolate(bool value);
        void SetVelocityForTimeInterval(ref BVector3 velocity, double timeInterval);
        void SetWalkDirection(ref BVector3 walkDirection);
        void Warp(ref BVector3 origin);
	}
}
