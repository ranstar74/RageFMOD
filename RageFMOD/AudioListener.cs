using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMOD;
using GTA;
using GTA.Math;
using RageAudio.Helpers;

namespace RageAudio
{
    /// <summary>
    /// Represents audio listener in 3D space. For now it supports only 1 listener, which is main player.
    /// It automatically switches between gameplay camera / rendering camera and gets its position and velocity.
    /// </summary>
    internal static class AudioListener
    {
        /// <summary>
        /// Position of the <see cref="AudioListener"/>.
        /// </summary>
        public static Vector3 Position { get; private set; }

        /// <summary>
        /// Forward vector of the <see cref="AudioListener"/>.
        /// </summary>
        public static Vector3 ForwardVector { get; private set; }

        /// <summary>
        /// Up vector of the <see cref="AudioListener"/>.
        /// </summary>
        public static Vector3 UpVector { get; private set; }

        /// <summary>
        /// Velocity vector of the <see cref="AudioListener"/> per second.
        /// </summary>
        public static Vector3 Velocity { get; private set; }

        /// <summary>
        /// <see cref="ATTRIBUTES_3D"/> of the <see cref="AudioListener"/>.
        /// </summary>
        public static ATTRIBUTES_3D Fmod3dAttributes { get;private set; }

        /// <summary>
        /// Previous frame <see cref="Position"/>. Used to calculate velocity.
        /// </summary>
        private static Vector3 previousPosition;

        /// <summary>
        /// Updates current listener position. Must be called every tick.
        /// </summary>
        public static void Update()
        {
            if (World.RenderingCamera.IsActive)
            {
                Position = World.RenderingCamera.Position;
                UpVector = World.RenderingCamera.UpVector;
                ForwardVector = World.RenderingCamera.ForwardVector;
            }
            else
            {
                Position = GameplayCamera.Position;
                UpVector = GameplayCamera.UpVector;
                ForwardVector = GameplayCamera.ForwardVector;
            }

            /*
             * For velocity, remember to use units per second, and not units per frame. 
             * This is a common mistake and will make the doppler effect sound wrong if 
             *  velocity is based on movement per frame rather than a fixed time period.
             * If velocity per frame is calculated, it can be converted to velocity 
             *  per second by dividing it by the time taken between frames as a fraction of a second.
             *  i.e.
             *   velocity_units_per_second = (position_currentframe - position_lastframe) / time_taken_since_last_frame_in_seconds.
             *   At 60fps the formula would look like velocity_units_per_second = (position_currentframe - position_lastframe) / 0.0166667.
             */
            Velocity = (Position - previousPosition) / Game.LastFrameTime;

            // Use vehicle position to prevent "doppler" bugs,
            // like when you move camera and it affects its velocity
            if (Game.Player.Character.IsInVehicle())
            {
                Velocity = Game.Player.Character.CurrentVehicle.Velocity;
            }

            Fmod3dAttributes = new ATTRIBUTES_3D()
            {
                forward = ForwardVector.ToFmodVector(),
                position = Position.ToFmodVector(),
                up = UpVector.ToFmodVector(),
                velocity = Velocity.ToFmodVector()
            };

            previousPosition = Position;
        }
    }
}
