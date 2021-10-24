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
        /// Converts <see cref="Vector3"/> to <see cref="FMOD.VECTOR"/>.
        /// </summary>
        /// <param name="vector">Vector to convert.</param>
        /// <returns><see cref="Vector3"/> converterd to <see cref="FMOD.VECTOR"/>.</returns>
        public static FMOD.VECTOR ToFmodVector(this Vector3 vector)
        {
            // FMod uses Y axis as vertical, GTA - Z, so we swap them
            return new FMOD.VECTOR()
            {
                x = vector.X,
                y = vector.Z,
                z = vector.Y,
            };
        }
    }
}
