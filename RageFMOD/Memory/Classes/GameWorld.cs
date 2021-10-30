using GTA.Math;

namespace RageAudio.Memory.Classes
{
    /// <summary>
    /// Provides various game world information.
    /// </summary>
    internal static class GameWorld
    {
        /// <summary>
        /// Position of current rendering camera, it could be one of these:
        /// GameplayCamera, Custom Camera, Cinematic Camera
        /// </summary>
        public static Vector3 RenderingCamPosition
        {
            get
            {
                return Vector3.Zero;//NativeMemory.Get<Vector3>(NativeMemory.CViewportGameAddr);
            }
        }
    }
}
