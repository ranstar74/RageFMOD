using GTA;
using GTA.Math;

namespace RageAudio.Helpers
{
    internal static class FmodHelpers
    {
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
