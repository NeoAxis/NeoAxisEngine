// Copyright (c) 2018 Kastellanos Nikolaos

namespace Internal.nkast.Aether.Physics2D.Dynamics
{
    public struct SolverIterations
    {
        /// <summary>The number of velocity iterations used in the solver.</summary>
        public int VelocityIterations;

        /// <summary>The number of position iterations used in the solver.</summary>
        public int PositionIterations;

        /// <summary>The number of velocity iterations in the TOI solver</summary>
        public int TOIVelocityIterations;

        /// <summary>The number of position iterations in the TOI solver</summary>
        public int TOIPositionIterations;

        /// By default, forces are cleared automatically after each call to Step.
        /// The default behavior is modified with this setting.
        /// The purpose of this setting is to support sub-stepping. Sub-stepping is often used to maintain
        /// a fixed sized time step under a variable frame-rate.
        /// When you perform sub-stepping you should disable auto clearing of forces and instead call
        /// ClearForces after all sub-steps are complete in one pass of your game loop.
        /// </summary>
        public bool AutoClearForces;
    }
}
