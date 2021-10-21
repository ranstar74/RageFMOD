using GTA;
using GTA.Math;

namespace RageAudio.Helpers
{
    internal static class FmodHelpers
    {
        /// <summary>
        /// Gets entity position and velocity and creates 3d attributes from them.
        /// </summary>
        /// <param name="entity">Entity context.</param>
        /// <returns>3d attributes of given entity.</returns>
        public static FMOD.ATTRIBUTES_3D Get3DAttributes(this Entity entity)
        {
            return new FMOD.ATTRIBUTES_3D
            {
                forward = entity.ForwardVector.ToFmodVector(),
                position = entity.Position.ToFmodVector(),
                up = entity.UpVector.ToFmodVector(),
                velocity = entity.Velocity.ToFmodVector()
            };
        }

        /// <summary>
        /// Gets camera position and velocity and creates 3d attributes from them.
        /// </summary>
        /// <param name="camera">Camera context.</param>
        /// <returns>3d attributes of given camera.</returns>
        public static FMOD.ATTRIBUTES_3D Get3DAttributes(this Camera camera)
        {
            // TODO: Add correct velocity for camera
            return new FMOD.ATTRIBUTES_3D
            {
                forward = camera.ForwardVector.ToFmodVector(),
                position = camera.Position.ToFmodVector(),
                up = camera.UpVector.ToFmodVector(),
                velocity = Vector3.Zero.ToFmodVector()
            };
        }

        /// <summary>
        /// Gets 3d attributes from gameplay camera and player or player vehicle velocity.
        /// </summary>
        /// <returns>3d attributes of player.</returns>
        public static FMOD.ATTRIBUTES_3D GetGamePlayer3DAttributes()
        {
            Vector3 velocitySource = Game.Player.Character.IsInVehicle() ? 
                Game.Player.Character.CurrentVehicle.Velocity : Game.Player.Character.Velocity;

            return new FMOD.ATTRIBUTES_3D
            {
                forward = GameplayCamera.ForwardVector.ToFmodVector(),
                position = GameplayCamera.Position.ToFmodVector(),
                up = GameplayCamera.UpVector.ToFmodVector(),
                velocity = velocitySource.ToFmodVector()
            };
        }

        /// <summary>
        /// Converts <see cref="Vector3"/> to <see cref="FMOD.VECTOR"/>.
        /// </summary>
        /// <param name="vector">Vector to convert.</param>
        /// <returns>Converter <see cref="FMOD.VECTOR"/>.</returns>
        public static FMOD.VECTOR ToFmodVector(this Vector3 vector)
        {
            return new FMOD.VECTOR()
            {
                x = vector.X,
                y = vector.Y,
                z = vector.Z,
            };
        }
    }
}
