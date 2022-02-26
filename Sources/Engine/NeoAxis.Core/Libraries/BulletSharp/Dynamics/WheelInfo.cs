using System;
using Internal.BulletSharp.Math;

namespace Internal.BulletSharp
{
	public struct WheelInfoConstructionInfo
	{
        public bool IsFrontWheel;
        public BVector3 ChassisConnectionCS;
        public double FrictionSlip;
        public double MaxSuspensionForce;
        public double MaxSuspensionTravelCm;
        public double SuspensionRestLength;
        public double SuspensionStiffness;
        public BVector3 WheelAxleCS;
        public BVector3 WheelDirectionCS;
        public double WheelRadius;
        public double WheelsDampingCompression;
        public double WheelsDampingRelaxation;
	}

    public struct RaycastInfo
    {
        public BVector3 ContactNormalWS;
        public BVector3 ContactPointWS;
        public Object GroundObject;
        public BVector3 HardPointWS;
        public bool IsInContact;
        public double SuspensionLength;
        public BVector3 WheelAxleWS;
        public BVector3 WheelDirectionWS;
    }

    public class WheelInfo
    {
        public WheelInfo(WheelInfoConstructionInfo ci)
        {
            SuspensionRestLength1 = ci.SuspensionRestLength;
            MaxSuspensionTravelCm = ci.MaxSuspensionTravelCm;

            WheelsRadius = ci.WheelRadius;
            SuspensionStiffness = ci.SuspensionStiffness;
            WheelsDampingCompression = ci.WheelsDampingCompression;
            WheelsDampingRelaxation = ci.WheelsDampingRelaxation;
            ChassisConnectionPointCS = ci.ChassisConnectionCS;
            WheelDirectionCS = ci.WheelDirectionCS;
            WheelAxleCS = ci.WheelAxleCS;
            FrictionSlip = ci.FrictionSlip;
            Steering = 0;
            EngineForce = 0;
            Rotation = 0;
            DeltaRotation = 0;
            Brake = 0;
            RollInfluence = 0.1f;
            IsFrontWheel = ci.IsFrontWheel;
            MaxSuspensionForce = ci.MaxSuspensionForce;

            //ClientInfo = IntPtr.Zero;
            //ClippedInvContactDotSuspension = 0;
            WorldTransform = BMatrix.Identity;
            //WheelsSuspensionForce = 0;
            //SuspensionRelativeVelocity = 0;
            //SkidInfo = 0;
            RaycastInfo = new RaycastInfo();
        }

        public void UpdateWheel(RigidBody chassis, RaycastInfo raycastInfo)
        {
            if (raycastInfo.IsInContact)
            {
                double project = BVector3.Dot(raycastInfo.ContactNormalWS, raycastInfo.WheelDirectionWS);
                BVector3 chassis_velocity_at_contactPoint;
                BVector3 relpos = raycastInfo.ContactPointWS - chassis.CenterOfMassPosition;
                chassis_velocity_at_contactPoint = chassis.GetVelocityInLocalPoint(relpos);
                double projVel = BVector3.Dot(raycastInfo.ContactNormalWS, chassis_velocity_at_contactPoint);
                if (project >= -0.1f)
                {
                    SuspensionRelativeVelocity = 0;
                    ClippedInvContactDotSuspension = 1.0f / 0.1f;
                }
                else
                {
                    double inv = -1.0f / project;
                    SuspensionRelativeVelocity = projVel * inv;
                    ClippedInvContactDotSuspension = inv;
                }

            }

            else    // Not in contact : position wheel in a nice (rest length) position
            {
                RaycastInfo.SuspensionLength = SuspensionRestLength;
                SuspensionRelativeVelocity = 0;
                RaycastInfo.ContactNormalWS = -raycastInfo.WheelDirectionWS;
                ClippedInvContactDotSuspension = 1.0f;
            }
        }

        public double SuspensionRestLength
        {
            get { return SuspensionRestLength1; }
        }

        public bool IsFrontWheel;
        public double Brake;
        public BVector3 ChassisConnectionPointCS;
        public IntPtr ClientInfo;
        public double ClippedInvContactDotSuspension;
        public double DeltaRotation;
        public double EngineForce;
        public double FrictionSlip;
        public double MaxSuspensionForce;
        public double MaxSuspensionTravelCm;
        public RaycastInfo RaycastInfo;
        public double RollInfluence;
        public double Rotation;
        public double SkidInfo;
        public double Steering;
        public double SuspensionRelativeVelocity;
        public double SuspensionRestLength1;
        public double SuspensionStiffness;
        public BVector3 WheelAxleCS;
        public BVector3 WheelDirectionCS;
        public double WheelsDampingCompression;
        public double WheelsDampingRelaxation;
        public double WheelsRadius;
        public double WheelsSuspensionForce;
        public BMatrix WorldTransform;
    }
}
