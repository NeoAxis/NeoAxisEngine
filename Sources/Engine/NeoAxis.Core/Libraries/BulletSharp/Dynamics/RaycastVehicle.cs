#define ROLLING_INFLUENCE_FIX

using System;
using System.Diagnostics;
using Internal.BulletSharp.Math;

namespace Internal.BulletSharp
{
    public class VehicleTuning
    {
        public double SuspensionStiffness;
        public double SuspensionCompression;
        public double SuspensionDamping;
        public double MaxSuspensionTravelCm;
        public double FrictionSlip;
        public double MaxSuspensionForce;

        public VehicleTuning()
        {
            SuspensionStiffness = 5.88f;
            SuspensionCompression = 0.83f;
            SuspensionDamping = 0.88f;
            MaxSuspensionTravelCm = 500.0f;
            FrictionSlip = 10.5f;
            MaxSuspensionForce = 6000.0f;
        }
    }
    
    public class RaycastVehicle : IAction
	{
        WheelInfo[] wheelInfo = new WheelInfo[0];

        BVector3[] forwardWS = new BVector3[0];
        BVector3[] axle = new BVector3[0];
        double[] forwardImpulse = new double[0];
        double[] sideImpulse = new double[0];

        public BMatrix ChassisWorldTransform
        {
            get
            {
                /*if (RigidBody.MotionState != null)
                {
                    return RigidBody.MotionState.WorldTransform;
                }*/
                return RigidBody.CenterOfMassTransform;
            }
        }

        double currentVehicleSpeedKmHour;

        public int NumWheels
        {
            get { return wheelInfo.Length; }
        }

        int indexRightAxis = 0;
        public int RightAxis
        {
            get { return indexRightAxis; }
        }

        int indexUpAxis = 2;
        int indexForwardAxis = 1;

        RigidBody chassisBody;
        public RigidBody RigidBody
        {
            get { return chassisBody; }
        }

        IVehicleRaycaster vehicleRaycaster;

        static RigidBody fixedBody;

        public void SetBrake(double brake, int wheelIndex)
        {
            Debug.Assert((wheelIndex >= 0) && (wheelIndex < NumWheels));
            GetWheelInfo(wheelIndex).Brake = brake;
        }

        public double GetSteeringValue(int wheel)
        {
            return GetWheelInfo(wheel).Steering;
        }

        public void SetSteeringValue(double steering, int wheel)
        {
            Debug.Assert(wheel >= 0 && wheel < NumWheels);

            WheelInfo wheelInfo = GetWheelInfo(wheel);
            wheelInfo.Steering = steering;
        }

        public void SetCoordinateSystem(int rightIndex, int upIndex, int forwardIndex)
        {
            indexRightAxis = rightIndex;
            indexUpAxis = upIndex;
            indexForwardAxis = forwardIndex;
        }

        public BMatrix GetWheelTransformWS(int wheelIndex)
        {
            Debug.Assert(wheelIndex < NumWheels);
            return wheelInfo[wheelIndex].WorldTransform;
        }

        static RaycastVehicle()
        {
            using (var ci = new RigidBodyConstructionInfo(0, null, null))
            {
                fixedBody = new RigidBody(ci);
                fixedBody.SetMassProps(0, BVector3.Zero);
            }
        }

        public RaycastVehicle(VehicleTuning tuning, RigidBody chassis, IVehicleRaycaster raycaster)
        {
            chassisBody = chassis;
            vehicleRaycaster = raycaster;
        }

        public WheelInfo AddWheel(BVector3 connectionPointCS, BVector3 wheelDirectionCS0, BVector3 wheelAxleCS, double suspensionRestLength, double wheelRadius, VehicleTuning tuning, bool isFrontWheel)
        {
            var ci = new WheelInfoConstructionInfo()
            {
                ChassisConnectionCS = connectionPointCS,
                WheelDirectionCS = wheelDirectionCS0,
                WheelAxleCS = wheelAxleCS,
                SuspensionRestLength = suspensionRestLength,
                WheelRadius = wheelRadius,
                IsFrontWheel = isFrontWheel,
                SuspensionStiffness = tuning.SuspensionStiffness,
                WheelsDampingCompression = tuning.SuspensionCompression,
                WheelsDampingRelaxation = tuning.SuspensionDamping,
                FrictionSlip = tuning.FrictionSlip,
                MaxSuspensionTravelCm = tuning.MaxSuspensionTravelCm,
                MaxSuspensionForce = tuning.MaxSuspensionForce
            };

            Array.Resize(ref wheelInfo, wheelInfo.Length + 1);
            var wheel = new WheelInfo(ci);
            wheelInfo[wheelInfo.Length - 1] = wheel;

            UpdateWheelTransformsWS(wheel, false);
            UpdateWheelTransform(NumWheels - 1, false);
            return wheel;
        }

        public void ApplyEngineForce(double force, int wheel)
        {
            Debug.Assert(wheel >= 0 && wheel < NumWheels);
            WheelInfo wheelInfo = GetWheelInfo(wheel);
            wheelInfo.EngineForce = force;
        }

        double CalcRollingFriction(RigidBody body0, RigidBody body1, BVector3 contactPosWorld, BVector3 frictionDirectionWorld, double maxImpulse)
        {
            double denom0 = body0.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
            double denom1 = body1.ComputeImpulseDenominator(contactPosWorld, frictionDirectionWorld);
            const double relaxation = 1.0f;
            double jacDiagABInv = relaxation / (denom0 + denom1);

            double j1;

            BVector3 rel_pos1 = contactPosWorld - body0.CenterOfMassPosition;
            BVector3 rel_pos2 = contactPosWorld - body1.CenterOfMassPosition;

            BVector3 vel1 = body0.GetVelocityInLocalPoint(rel_pos1);
            BVector3 vel2 = body1.GetVelocityInLocalPoint(rel_pos2);
            BVector3 vel = vel1 - vel2;

            double vrel;
            BVector3.Dot(ref frictionDirectionWorld, ref vel, out vrel);

            // calculate j that moves us to zero relative velocity
            j1 = -vrel * jacDiagABInv;
            j1 = System.Math.Min(j1, maxImpulse);
            j1 = System.Math.Max(j1, -maxImpulse);

            return j1;
        }

        BVector3 blue = new BVector3(0, 0, 1);
        BVector3 magenta = new BVector3(1, 0, 1);
        public void DebugDraw(IDebugDraw debugDrawer)
        {
            for (int v = 0; v < NumWheels; v++)
            {
                WheelInfo wheelInfo = GetWheelInfo(v);

                BVector3 wheelColor;
                if (wheelInfo.RaycastInfo.IsInContact)
                {
                    wheelColor = blue;
                }
                else
                {
                    wheelColor = magenta;
                }

                BMatrix transform = wheelInfo.WorldTransform;
                BVector3 wheelPosWS = transform.Origin;

                BVector3 axle = new BVector3(
                    transform[0, RightAxis],
                    transform[1, RightAxis],
                    transform[2, RightAxis]);

                BVector3 to1 = wheelPosWS + axle;
                BVector3 to2 = GetWheelInfo(v).RaycastInfo.ContactPointWS;

                //debug wheels (cylinders)
                debugDrawer.DrawLine(ref wheelPosWS, ref to1, ref wheelColor);
                debugDrawer.DrawLine(ref wheelPosWS, ref to2, ref wheelColor);

            }
        }

        public WheelInfo GetWheelInfo(int index)
        {
            Debug.Assert((index >= 0) && (index < NumWheels));

            return wheelInfo[index];
        }

        private double RayCast(WheelInfo wheel)
        {
            UpdateWheelTransformsWS(wheel, false);

            double depth = -1;
            double raylen = wheel.SuspensionRestLength + wheel.WheelsRadius;

            BVector3 rayvector = wheel.RaycastInfo.WheelDirectionWS * raylen;
            BVector3 source = wheel.RaycastInfo.HardPointWS;
            wheel.RaycastInfo.ContactPointWS = source + rayvector;
            BVector3 target = wheel.RaycastInfo.ContactPointWS;

            double param = 0;
            VehicleRaycasterResult rayResults = new VehicleRaycasterResult();

            Debug.Assert(vehicleRaycaster != null);
            object obj = vehicleRaycaster.CastRay(ref source, ref target, rayResults);

            wheel.RaycastInfo.GroundObject = null;

            if (obj != null)
            {
                param = rayResults.DistFraction;
                depth = raylen * rayResults.DistFraction;
                wheel.RaycastInfo.ContactNormalWS = rayResults.HitNormalInWorld;
                wheel.RaycastInfo.IsInContact = true;

                wheel.RaycastInfo.GroundObject = fixedBody;///@todo for driving on dynamic/movable objects!;
                /////wheel.RaycastInfo.GroundObject = object;

                double hitDistance = param * raylen;
                wheel.RaycastInfo.SuspensionLength = hitDistance - wheel.WheelsRadius;
                //clamp on max suspension travel

                double minSuspensionLength = wheel.SuspensionRestLength - wheel.MaxSuspensionTravelCm * 0.01f;
                double maxSuspensionLength = wheel.SuspensionRestLength + wheel.MaxSuspensionTravelCm * 0.01f;
                if (wheel.RaycastInfo.SuspensionLength < minSuspensionLength)
                {
                    wheel.RaycastInfo.SuspensionLength = minSuspensionLength;
                }
                if (wheel.RaycastInfo.SuspensionLength > maxSuspensionLength)
                {
                    wheel.RaycastInfo.SuspensionLength = maxSuspensionLength;
                }

                wheel.RaycastInfo.ContactPointWS = rayResults.HitPointInWorld;

                double denominator = BVector3.Dot(wheel.RaycastInfo.ContactNormalWS, wheel.RaycastInfo.WheelDirectionWS);

                BVector3 chassis_velocity_at_contactPoint;
                BVector3 relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

                chassis_velocity_at_contactPoint = RigidBody.GetVelocityInLocalPoint(relpos);

                double projVel = BVector3.Dot(wheel.RaycastInfo.ContactNormalWS, chassis_velocity_at_contactPoint);

                if (denominator >= -0.1f)
                {
                    wheel.SuspensionRelativeVelocity = 0;
                    wheel.ClippedInvContactDotSuspension = 1.0f / 0.1f;
                }
                else
                {
                    double inv = -1.0f / denominator;
                    wheel.SuspensionRelativeVelocity = projVel * inv;
                    wheel.ClippedInvContactDotSuspension = inv;
                }
            }
            else
            {
                //put wheel info as in rest position
                wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
                wheel.SuspensionRelativeVelocity = 0.0f;
                wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
                wheel.ClippedInvContactDotSuspension = 1.0f;
            }

            return depth;
        }

        void ResetSuspension()
        {
	        for (int i = 0; i < NumWheels; i++)
	        {
		        WheelInfo wheel = GetWheelInfo(i);
		        wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
		        wheel.SuspensionRelativeVelocity = 0;

		        wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
		        //wheel.ContactFriction = 0;
		        wheel.ClippedInvContactDotSuspension = 1;
	        }
        }

        private void ResolveSingleBilateral(RigidBody body1, BVector3 pos1, RigidBody body2, BVector3 pos2, double distance, BVector3 normal, ref double impulse, double timeStep)
        {
            double normalLenSqr = normal.LengthSquared;
            Debug.Assert(System.Math.Abs(normalLenSqr) < 1.1f);
            if (normalLenSqr > 1.1f)
            {
                impulse = 0;
                return;
            }
            BVector3 rel_pos1 = pos1 - body1.CenterOfMassPosition;
            BVector3 rel_pos2 = pos2 - body2.CenterOfMassPosition;

            BVector3 vel1 = body1.GetVelocityInLocalPoint(rel_pos1);
            BVector3 vel2 = body2.GetVelocityInLocalPoint(rel_pos2);
            BVector3 vel = vel1 - vel2;

            BMatrix world2A = BMatrix.Transpose(body1.CenterOfMassTransform.Basis);
            BMatrix world2B = BMatrix.Transpose(body2.CenterOfMassTransform.Basis);
            BVector3 m_aJ = BVector3.TransformCoordinate(BVector3.Cross(rel_pos1, normal), world2A);
            BVector3 m_bJ = BVector3.TransformCoordinate(BVector3.Cross(rel_pos2, -normal), world2B);
            BVector3 m_0MinvJt = body1.InvInertiaDiagLocal * m_aJ;
            BVector3 m_1MinvJt = body2.InvInertiaDiagLocal * m_bJ;
            double dot0, dot1;
            BVector3.Dot(ref m_0MinvJt, ref m_aJ, out dot0);
            BVector3.Dot(ref m_1MinvJt, ref m_bJ, out dot1);
            double jacDiagAB = body1.InvMass + dot0 + body2.InvMass + dot1;
            double jacDiagABInv = 1.0f / jacDiagAB;

            double rel_vel;
            BVector3.Dot(ref normal, ref vel, out rel_vel);

            //todo: move this into proper structure
            const double contactDamping = 0.2f;

#if ONLY_USE_LINEAR_MASS
	        double massTerm = 1.0f / (body1.InvMass + body2.InvMass);
	        impulse = - contactDamping * rel_vel * massTerm;
#else
            double velocityImpulse = -contactDamping * rel_vel * jacDiagABInv;
            impulse = velocityImpulse;
#endif
        }

        public void UpdateAction(CollisionWorld collisionWorld, double deltaTimeStep)
        {
            UpdateVehicle(deltaTimeStep);
        }

        const double sideFrictionStiffness2 = 1.0f;
        public void UpdateFriction(double timeStep)
        {
            //calculate the impulse, so that the wheels don't move sidewards
            int numWheel = NumWheels;
            if (numWheel == 0)
                return;

            Array.Resize(ref forwardWS, numWheel);
            Array.Resize(ref axle, numWheel);
            Array.Resize(ref forwardImpulse, numWheel);
            Array.Resize(ref sideImpulse, numWheel);

            int numWheelsOnGround = 0;

            //collapse all those loops into one!
            for (int i = 0; i < NumWheels; i++)
            {
                RigidBody groundObject = wheelInfo[i].RaycastInfo.GroundObject as RigidBody;
                if (groundObject != null)
                    numWheelsOnGround++;
                sideImpulse[i] = 0;
                forwardImpulse[i] = 0;
            }

            for (int i = 0; i < NumWheels; i++)
            {
                WheelInfo wheel = wheelInfo[i];

                RigidBody groundObject = wheel.RaycastInfo.GroundObject as RigidBody;
                if (groundObject != null)
                {
                    BMatrix wheelTrans = GetWheelTransformWS(i);

                    axle[i] = new BVector3(
                        wheelTrans[0, indexRightAxis],
                        wheelTrans[1, indexRightAxis],
                        wheelTrans[2, indexRightAxis]);

                    BVector3 surfNormalWS = wheel.RaycastInfo.ContactNormalWS;
                    double proj;
                    BVector3.Dot(ref axle[i], ref surfNormalWS, out proj);
                    axle[i] -= surfNormalWS * proj;
                    axle[i].Normalize();

                    BVector3.Cross(ref surfNormalWS, ref axle[i], out forwardWS[i]);
                    forwardWS[i].Normalize();

                    ResolveSingleBilateral(chassisBody, wheel.RaycastInfo.ContactPointWS,
                              groundObject, wheel.RaycastInfo.ContactPointWS,
                              0, axle[i], ref sideImpulse[i], timeStep);

                    sideImpulse[i] *= sideFrictionStiffness2;
                }
            }

            const double sideFactor = 1.0f;
            const double fwdFactor = 0.5f;

            bool sliding = false;

            for (int i = 0; i < NumWheels; i++)
            {
                WheelInfo wheel = wheelInfo[i];
                RigidBody groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

                double rollingFriction = 0.0f;

                if (groundObject != null)
                {
                    if (wheel.EngineForce != 0.0f)
                    {
                        rollingFriction = wheel.EngineForce * timeStep;
                    }
                    else
                    {
                        double defaultRollingFrictionImpulse = 0.0f;
                        double maxImpulse = (wheel.Brake != 0) ? wheel.Brake : defaultRollingFrictionImpulse;
                        rollingFriction = CalcRollingFriction(chassisBody, groundObject, wheel.RaycastInfo.ContactPointWS, forwardWS[i], maxImpulse);
                    }
                }

                //switch between active rolling (throttle), braking and non-active rolling friction (no throttle/break)

                forwardImpulse[i] = 0;
                wheelInfo[i].SkidInfo = 1.0f;

                if (groundObject != null)
                {
                    wheelInfo[i].SkidInfo = 1.0f;

                    double maximp = wheel.WheelsSuspensionForce * timeStep * wheel.FrictionSlip;
                    double maximpSide = maximp;

                    double maximpSquared = maximp * maximpSide;


                    forwardImpulse[i] = rollingFriction;//wheel.EngineForce* timeStep;

                    double x = forwardImpulse[i] * fwdFactor;
                    double y = sideImpulse[i] * sideFactor;

                    double impulseSquared = (x * x + y * y);

                    if (impulseSquared > maximpSquared)
                    {
                        sliding = true;

                        double factor = maximp / (double)System.Math.Sqrt(impulseSquared);

                        wheelInfo[i].SkidInfo *= factor;
                    }
                }
            }

            if (sliding)
            {
                for (int wheel = 0; wheel < NumWheels; wheel++)
                {
                    if (sideImpulse[wheel] != 0)
                    {
                        if (wheelInfo[wheel].SkidInfo < 1.0f)
                        {
                            forwardImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
                            sideImpulse[wheel] *= wheelInfo[wheel].SkidInfo;
                        }
                    }
                }
            }

            // apply the impulses
            for (int i = 0; i < NumWheels; i++)
            {
                WheelInfo wheel = wheelInfo[i];

                BVector3 rel_pos = wheel.RaycastInfo.ContactPointWS -
                        chassisBody.CenterOfMassPosition;

                if (forwardImpulse[i] != 0)
                {
                    chassisBody.ApplyImpulse(forwardWS[i] * forwardImpulse[i], rel_pos);
                }
                if (sideImpulse[i] != 0)
                {
                    RigidBody groundObject = wheel.RaycastInfo.GroundObject as RigidBody;

                    BVector3 rel_pos2 = wheel.RaycastInfo.ContactPointWS -
                        groundObject.CenterOfMassPosition;


                    BVector3 sideImp = axle[i] * sideImpulse[i];

#if ROLLING_INFLUENCE_FIX // fix. It only worked if car's up was along Y - VT.
                    //Vector4 vChassisWorldUp = RigidBody.CenterOfMassTransform.get_Columns(indexUpAxis);
                    BVector3 vChassisWorldUp = new BVector3(
                        RigidBody.CenterOfMassTransform.Row1[indexUpAxis],
                        RigidBody.CenterOfMassTransform.Row2[indexUpAxis],
                        RigidBody.CenterOfMassTransform.Row3[indexUpAxis]);
                    double dot;
                    BVector3.Dot(ref vChassisWorldUp, ref rel_pos, out dot);
                    rel_pos -= vChassisWorldUp * (dot * (1.0f - wheel.RollInfluence));
#else
                    rel_pos[indexUpAxis] *= wheel.RollInfluence;
#endif
                    chassisBody.ApplyImpulse(sideImp, rel_pos);

                    //apply friction impulse on the ground
                    groundObject.ApplyImpulse(-sideImp, rel_pos2);
                }
            }
        }

        public void UpdateSuspension(double step)
        {
            double chassisMass = 1.0f / chassisBody.InvMass;

            for (int w_it = 0; w_it < NumWheels; w_it++)
            {
                WheelInfo wheel_info = wheelInfo[w_it];

                if (wheel_info.RaycastInfo.IsInContact)
                {
                    double force;
                    //	Spring
                    {
                        double susp_length = wheel_info.SuspensionRestLength;
                        double current_length = wheel_info.RaycastInfo.SuspensionLength;

                        double length_diff = (susp_length - current_length);

                        force = wheel_info.SuspensionStiffness
                            * length_diff * wheel_info.ClippedInvContactDotSuspension;
                    }

                    // Damper
                    {
                        double projected_rel_vel = wheel_info.SuspensionRelativeVelocity;
                        {
                            double susp_damping;
                            if (projected_rel_vel < 0.0f)
                            {
                                susp_damping = wheel_info.WheelsDampingCompression;
                            }
                            else
                            {
                                susp_damping = wheel_info.WheelsDampingRelaxation;
                            }
                            force -= susp_damping * projected_rel_vel;
                        }
                    }

                    // RESULT
                    wheel_info.WheelsSuspensionForce = force * chassisMass;
                    if (wheel_info.WheelsSuspensionForce < 0)
                    {
                        wheel_info.WheelsSuspensionForce = 0;
                    }
                }
                else
                {
                    wheel_info.WheelsSuspensionForce = 0;
                }
            }
        }

        public void UpdateVehicle(double step)
        {
            for (int i = 0; i < wheelInfo.Length; i++)
            {
                UpdateWheelTransform(i, false);
            }

            currentVehicleSpeedKmHour = 3.6f * RigidBody.LinearVelocity.Length;

            BMatrix chassisTrans = ChassisWorldTransform;

            BVector3 forwardW = new BVector3(
                chassisTrans[0, indexForwardAxis],
                chassisTrans[1, indexForwardAxis],
                chassisTrans[2, indexForwardAxis]);

            if (BVector3.Dot(forwardW, RigidBody.LinearVelocity) < 0)
            {
                currentVehicleSpeedKmHour *= -1.0f;
            }

            // Simulate suspension
            for (int i = 0; i < wheelInfo.Length; i++)
            {
                //double depth = 
                RayCast(wheelInfo[i]);
            }


            UpdateSuspension(step);

            for (int i = 0; i < wheelInfo.Length; i++)
            {
                //apply suspension force
                WheelInfo wheel = wheelInfo[i];

                double suspensionForce = wheel.WheelsSuspensionForce;

                if (suspensionForce > wheel.MaxSuspensionForce)
                {
                    suspensionForce = wheel.MaxSuspensionForce;
                }
                BVector3 impulse = wheel.RaycastInfo.ContactNormalWS * suspensionForce * step;
                BVector3 relpos = wheel.RaycastInfo.ContactPointWS - RigidBody.CenterOfMassPosition;

                RigidBody.ApplyImpulse(impulse, relpos);
            }


            UpdateFriction(step);

            for (int i = 0; i < wheelInfo.Length; i++)
            {
                WheelInfo wheel = wheelInfo[i];
                BVector3 relpos = wheel.RaycastInfo.HardPointWS - RigidBody.CenterOfMassPosition;
                BVector3 vel = RigidBody.GetVelocityInLocalPoint(relpos);

                if (wheel.RaycastInfo.IsInContact)
                {
                    BMatrix chassisWorldTransform = ChassisWorldTransform;

                    BVector3 fwd = new BVector3(
                        chassisWorldTransform[0, indexForwardAxis],
                        chassisWorldTransform[1, indexForwardAxis],
                        chassisWorldTransform[2, indexForwardAxis]);

                    double proj = BVector3.Dot(fwd, wheel.RaycastInfo.ContactNormalWS);
                    fwd -= wheel.RaycastInfo.ContactNormalWS * proj;

                    double proj2;
                    BVector3.Dot(ref fwd, ref vel, out proj2);

                    wheel.DeltaRotation = (proj2 * step) / (wheel.WheelsRadius);
                    wheel.Rotation += wheel.DeltaRotation;
                }
                else
                {
                    wheel.Rotation += wheel.DeltaRotation;
                }

                wheel.DeltaRotation *= 0.99f;//damping of rotation when not in contact
            }
        }

        public void UpdateWheelTransform(int wheelIndex, bool interpolatedTransform)
        {
            WheelInfo wheel = wheelInfo[wheelIndex];
            UpdateWheelTransformsWS(wheel, interpolatedTransform);
            BVector3 up = -wheel.RaycastInfo.WheelDirectionWS;
            BVector3 right = wheel.RaycastInfo.WheelAxleWS;
            BVector3 fwd = BVector3.Cross(up, right);
            fwd.Normalize();
            //up = Vector3.Cross(right, fwd);
            //up.Normalize();

            //rotate around steering over the wheelAxleWS
            BMatrix steeringMat = BMatrix.RotationAxis(up, wheel.Steering);
            BMatrix rotatingMat = BMatrix.RotationAxis(right, -wheel.Rotation);

            BMatrix basis2 = new BMatrix();
            basis2.M11 = right[0];
            basis2.M12 = fwd[0];
            basis2.M13 = up[0];
            basis2.M21 = right[1];
            basis2.M22 = fwd[1];
            basis2.M23 = up[1];
            basis2.M31 = right[2];
            basis2.M32 = fwd[2];
            basis2.M13 = up[2];

            BMatrix transform = steeringMat * rotatingMat * basis2;
            transform.Origin = wheel.RaycastInfo.HardPointWS + wheel.RaycastInfo.WheelDirectionWS * wheel.RaycastInfo.SuspensionLength;
            wheel.WorldTransform = transform;
        }

        void UpdateWheelTransformsWS(WheelInfo wheel, bool interpolatedTransform)
        {
            wheel.RaycastInfo.IsInContact = false;

            BMatrix chassisTrans = ChassisWorldTransform;
            if (interpolatedTransform && RigidBody.MotionState != null)
            {
                chassisTrans = RigidBody.MotionState.WorldTransform;
            }

            wheel.RaycastInfo.HardPointWS = BVector3.TransformCoordinate(wheel.ChassisConnectionPointCS, chassisTrans);
            BMatrix chassisTransBasis = chassisTrans.Basis;
            wheel.RaycastInfo.WheelDirectionWS = BVector3.TransformCoordinate(wheel.WheelDirectionCS, chassisTransBasis);
            wheel.RaycastInfo.WheelAxleWS = BVector3.TransformCoordinate(wheel.WheelAxleCS, chassisTransBasis);
        }
	}

    public class DefaultVehicleRaycaster : IVehicleRaycaster
    {
        private DynamicsWorld _dynamicsWorld;

        public DefaultVehicleRaycaster(DynamicsWorld world)
        {
            _dynamicsWorld = world;
        }

        public object CastRay(ref BVector3 from, ref BVector3 to, VehicleRaycasterResult result)
        {
            //	RayResultCallback& resultCallback;
            using (var rayCallback = new ClosestRayResultCallback(ref from, ref to))
            {
                _dynamicsWorld.RayTestRef(ref from, ref to, rayCallback);

                if (rayCallback.HasHit)
                {
                    var body = RigidBody.Upcast(rayCallback.CollisionObject);
                    if (body != null && body.HasContactResponse)
                    {
                        result.HitPointInWorld = rayCallback.HitPointWorld;
                        BVector3 hitNormalInWorld = rayCallback.HitNormalWorld;
                        hitNormalInWorld.Normalize();
                        result.HitNormalInWorld = hitNormalInWorld;
                        result.DistFraction = rayCallback.ClosestHitFraction;
                        return body;
                    }
                }
            }
            return null;
        }
    }
}
