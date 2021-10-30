namespace RageAudio.Memory.Classes
{
    /// <summary>
    /// Contains various information about game window.
    /// </summary>
    internal static class GameWindow
    {
        /// <summary>
        /// Returns True If Game Window is Focused, Otherwise False.
        /// </summary>
        public static bool IsWindowFocused
        {
            get
            {
                return NativeMemory.Get<bool>(NativeMemory.IsGameWindowFocusedAddr);
            }
        }
    }
}
